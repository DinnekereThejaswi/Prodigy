using Newtonsoft.Json;
using NumberToWordsINR;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.SIGlobals
{
    public static class Globals
    {
        private static string _separater = "$";
        public static string Separator
        {
            get { return _separater; }
            set { _separater = value; }
        }

        /// <summary>
        /// Method is used to Get Date Time from the Server
        /// </summary>
        /// <returns></returns>
        public static DateTime GetApplicationDate()
        {
            //DataTable tblDate = Common.ExecuteQuery("SELECT [dbo].[GetDateTime] ()");
            //return (Convert.ToDateTime(tblDate.Rows[0][0]));
            Model.MagnaDb.MagnaDbEntities db = new Model.MagnaDb.MagnaDbEntities();
            DateTime dt = DateTime.ParseExact(db.KSTU_RATE_MASTER.ToList()[0].ondate.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime resultDate = dt.Add(DateTime.Now.TimeOfDay);
            return resultDate;
        }

        /// <summary>
        /// Method is used to Get Date Time from the Server
        /// </summary>
        /// <returns></returns>
        public static DateTime GetApplicationDate(string companyCode, string branchCode)
        {
            Model.MagnaDb.MagnaDbEntities db = new Model.MagnaDb.MagnaDbEntities();
            DateTime dt = DateTime.ParseExact(db.KSTU_RATE_MASTER.Where(r => r.company_code == companyCode && r.branch_code == branchCode).ToList()[0].ondate.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime resultDate = dt.Add(DateTime.Now.TimeOfDay);
            return resultDate;
        }

        /// <summary>
        /// Method returns the Actual date and Time
        /// </summary>
        /// <returns></returns>
        public static DateTime GetDateTime()
        {
            DataTable tblDate = Globals.ExecuteQuery("SELECT [mw].[ufn_GetDate]()");
            return (Convert.ToDateTime(tblDate.Rows[0][0]));
        }
        /// <summary>
        /// Method is used to Execute Query 
        /// </summary>
        /// <param name="Query"></param>
        /// <returns>DataTable</returns>
        public static DataTable ExecuteQuery(string Query)
        {
            var table = new DataTable();
            using (var ctx = new MagnaDbEntities()) {
                var cmd = ctx.Database.Connection.CreateCommand();
                cmd.CommandText = Query;

                cmd.Connection.Open();
                table.Load(cmd.ExecuteReader());
                cmd.Connection.Close();
            }
            return table;
        }

        /// <summary>
        /// Executes SQL and get the records affected
        /// </summary>
        /// <param name="sql">SQL query to be executed</param>
        /// <param name="db">Database context, pass null if no context is required</param>
        /// <returns>Returns records affected</returns>
        public static int ExecuteSQL(string sql, MagnaDbEntities db, object[] parameters)
        {
            try {
                if (db == null)
                    db = new MagnaDbEntities();
                var trans = db.Database.CurrentTransaction;
                int recordsAffected = db.Database.ExecuteSqlCommand(sql, parameters);
                return recordsAffected;
            }
            catch (Exception) {
                throw;
            }
        }

        public static void ExecuteSQL(string sql, MagnaDbEntities db)
        {
            try {
                if(db == null) {
                    throw new Exception("The database connections is closed.");
                }
                db.Database.ExecuteSqlCommand(sql);
            }
            catch (Exception) {
                throw;
            }
        }

        public static void UpdateSeqenceNumber(string objID)
        {
            Model.MagnaDb.MagnaDbEntities db = new Model.MagnaDb.MagnaDbEntities();
            KSTS_SEQ_NOS sequeceNo = db.KSTS_SEQ_NOS.Where(sq => sq.obj_id == objID).FirstOrDefault();
            sequeceNo.nextno = sequeceNo.nextno + 1;
            db.Entry(sequeceNo).State = System.Data.Entity.EntityState.Modified;
            try {
                db.SaveChanges();
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public static void UpdateSeqenceNumber(Model.MagnaDb.MagnaDbEntities db, string objID, string companyCode, string branchCode)
        {
            try {
                KSTS_SEQ_NOS sequeceNo = db.KSTS_SEQ_NOS.Where(sq => sq.obj_id == objID && sq.company_code == companyCode && sq.branch_code == branchCode).FirstOrDefault();
                sequeceNo.nextno = sequeceNo.nextno + 1;
                db.Entry(sequeceNo).State = System.Data.Entity.EntityState.Modified;
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public static int GetNewTransactionSerialNo(MagnaDbEntities db, string objID, string companyCode, string branchCode)
        {
            try {
                KSTS_SEQ_NOS sequeceNo = db.KSTS_SEQ_NOS.Where(sq => sq.obj_id == objID && sq.company_code == companyCode && sq.branch_code == branchCode).FirstOrDefault();
                return sequeceNo.nextno;
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public static void UpdateAccountSeqenceNumber(Model.MagnaDb.MagnaDbEntities db, string ObjID, string companyCode, string branchCode)
        {
            try {
                KSTS_ACC_SEQ_NOS sequeceNo = db.KSTS_ACC_SEQ_NOS.Where(sq => sq.obj_id == ObjID && sq.company_code == companyCode && sq.branch_code == branchCode).FirstOrDefault();
                sequeceNo.nextno = sequeceNo.nextno + 1;
                db.Entry(sequeceNo).State = System.Data.Entity.EntityState.Modified;
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public static void UpdateAccountVourcherSeqenceNumber(Model.MagnaDb.MagnaDbEntities db, string ObjID, string companyCode, string branchCode)
        {
            try {
                KSTS_ACC_VOUCHER_SEQ_NOS sequeceNo = db.KSTS_ACC_VOUCHER_SEQ_NOS.Where(sq => sq.obj_id == ObjID
                                                    && sq.company_code == companyCode
                                                    && sq.branch_code == branchCode).FirstOrDefault();
                sequeceNo.nextno = sequeceNo.nextno + 1;
                db.Entry(sequeceNo).State = System.Data.Entity.EntityState.Modified;
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public static int GetFinancialYear(Model.MagnaDb.MagnaDbEntities db, string companyCode, string branchCode)
        {
            return db.KSTU_ACC_FY_MASTER.Where(f => f.company_code == companyCode && f.branch_code == branchCode).FirstOrDefault().fin_year;
        }

        public static string GetNewGUID()
        {
            return Guid.NewGuid().ToString().Replace("-", "").ToUpper();
        }

        public static string PaymentMode(string payMode)
        {
            string retValue = string.Empty;
            switch (payMode) {
                case "C": retValue = "Cash"; break;
                case "Q": retValue = "Cheque"; break;
                case "D": retValue = "Demand Draft"; break;
                case "R": retValue = "Card"; break;
                case "CT": retValue = "Scheme.Adj"; break;
                case "BC": retValue = "OGS"; break;
                case "CN": retValue = "CREDIT NOTE"; break;
                case "EP": retValue = "E-Payment"; break;
                case "GV": retValue = "Gift Vou"; break;
                case "SP": retValue = "Sales Promotion"; break;
                case "UPI": retValue = "UPI Payment"; break;
                default: retValue = ""; break;
            }
            return retValue;
        }

        public static string OrderType(string orderType)
        {
            string retValue = string.Empty;
            switch (orderType) {
                case "R": retValue = "Reserved Order"; break;
                case "O": retValue = "Customized Order"; break;
                case "A": retValue = "Order Advance"; break;
                default: retValue = ""; break;
            }
            return retValue;
        }

        public static string GetReportStyle()
        {
            StringBuilder sbStyle = new StringBuilder();
            if (string.IsNullOrEmpty(sbStyle.ToString())) {
                sbStyle.AppendLine("<style = 'text/css'>\n");
                sbStyle.AppendLine(".boldText, .boldtable TD, .boldtable TH\n");
                sbStyle.AppendLine("{\n");
                sbStyle.AppendLine("font-family: 'Times New Roman' ,'Arial Narrow', Arial, monospace;\n");
                sbStyle.AppendLine("font-size:10pt;\n");
                sbStyle.AppendLine("}\n");
                sbStyle.AppendLine(".tableStyle\n");
                sbStyle.AppendLine("{\n");
                sbStyle.AppendLine("border-left: 0px solid black;\n");
                sbStyle.AppendLine("border-right: 0px solid black;\n");
                sbStyle.AppendLine("border-top-style:none;\n");
                sbStyle.AppendLine("border-bottom-style:none;\n");
                sbStyle.AppendLine("border-collapse: collapse;\n");
                sbStyle.AppendLine("}\n");
                sbStyle.AppendLine(".table,TD,TH\n");
                sbStyle.AppendLine("{\n");
                sbStyle.AppendLine("border-left-style:none;\n");
                sbStyle.AppendLine("border-right: 1px solid black;\n");
                sbStyle.AppendLine("border-bottom-style:none;\n");
                sbStyle.AppendLine("border-top-style:1px solid\n");
                sbStyle.AppendLine("border-collapse: collapse;\n");
                sbStyle.AppendLine("}\n");
                sbStyle.AppendLine(".noborder\n");
                sbStyle.AppendLine("{\n");
                sbStyle.AppendLine("border-left-style:none;\n");
                sbStyle.AppendLine("border-right-style:none;\n");
                sbStyle.AppendLine("border-top-style:none;;\n");
                sbStyle.AppendLine("border-bottom-style:none;\n");
                sbStyle.AppendLine("border-collapse: collapse;\n");
                sbStyle.AppendLine("}\n");
                sbStyle.AppendLine("</style>\n");
            }
            return sbStyle.ToString();
        }

        public static string GetStyleForBill()
        {
            StringBuilder sbStyleForBill = new StringBuilder();
            if (string.IsNullOrEmpty(sbStyleForBill.ToString())) {
                #region Fill Style
                sbStyleForBill.AppendLine("<style = \"text/css\">\n");
                sbStyleForBill.AppendLine(".billstyle\n");
                sbStyleForBill.AppendLine("{\n");
                sbStyleForBill.AppendLine("border-bottom: 1px Solid Black;\n");
                sbStyleForBill.AppendLine("border-left:none;\n");
                sbStyleForBill.AppendLine("border-right:none;\n");
                sbStyleForBill.AppendLine("}\n");

                sbStyleForBill.AppendLine(".box\n");
                sbStyleForBill.AppendLine("{\n");
                sbStyleForBill.AppendLine("border-style: solid;\n");
                sbStyleForBill.AppendLine("border-width:1px thin;\n");
                sbStyleForBill.AppendLine("border-radius:20px;\n");
                sbStyleForBill.AppendLine("float:left;\n");
                //sbStart.AppendLine("padding:10px;\n");
                sbStyleForBill.AppendLine("margin-right:10px;\n");
                sbStyleForBill.AppendLine(" width:300px;\n");
                sbStyleForBill.AppendLine(" height:100px;\n");
                sbStyleForBill.AppendLine("}\n");

                sbStyleForBill.AppendLine(".boldtable, .boldtable TD, .boldtable TH\n");
                sbStyleForBill.AppendLine("{\n");
                sbStyleForBill.AppendLine("font-family: 'Courier New', Courier, monospace;\n");
                sbStyleForBill.AppendLine("font-size:10pt;\n");
                sbStyleForBill.AppendLine("font-weight: bold;\n");
                sbStyleForBill.AppendLine("}\n");

                sbStyleForBill.AppendLine(".boldLine, .boldLine TD, .boldLine TH\n");
                sbStyleForBill.AppendLine("{\n");
                sbStyleForBill.AppendLine("font-family: 'Courier New', Courier, monospace;\n");
                sbStyleForBill.AppendLine("font-size:2pt;\n");
                sbStyleForBill.AppendLine("font-weight: bold;\n");
                sbStyleForBill.AppendLine("}\n");

                sbStyleForBill.AppendLine(".boldText\n");
                sbStyleForBill.AppendLine("{\n");
                sbStyleForBill.AppendLine("font-family: 'Courier New', Courier, monospace;\n");
                sbStyleForBill.AppendLine("font-size:12pt;\n");
                sbStyleForBill.AppendLine("font-weight: bold;\n");
                sbStyleForBill.AppendLine("}\n");

                sbStyleForBill.AppendLine(".normaltable, .normaltable TD, .normaltable TH\n");
                sbStyleForBill.AppendLine("{\n");
                sbStyleForBill.AppendLine("font-family: 'Courier New', Courier, monospace;\n");
                sbStyleForBill.AppendLine("font-size:7pt;\n");
                sbStyleForBill.AppendLine("font-weight: regular;\n");
                sbStyleForBill.AppendLine("}\n");

                sbStyleForBill.AppendLine("</style>\n");
                #endregion
            }
            return sbStyleForBill.ToString();
        }

        public static string MasterGroupType(string Code)
        {
            string retValue = string.Empty;
            switch (Code) {
                case "A": retValue = "Asset"; break;
                case "L": retValue = "Liabality"; break;
                case "I": retValue = "Income"; break;
                case "E": retValue = "Expanse"; break;
                default: retValue = ""; break;
            }
            return retValue;
        }

        public static List<string> SplitStringAt(int number, string contents)
        {
            List<string> lsStr = new List<string>();

            if ((number == 0) || (number < 0) || (number > contents.Length)) {
                lsStr.Add(contents.PadRight(number));
                return lsStr;
            }
            else {
                int count = 0;

                StringBuilder _string = new StringBuilder();
                foreach (char c in contents.ToCharArray()) {
                    if (count == number) {
                        count = 0;

                        lsStr.Add(_string.ToString());
                        _string = new StringBuilder();

                        _string.Append(c);
                    }
                    else {
                        _string.Append(c);
                        count++;
                    }
                }
                if (count != number)
                    lsStr.Add(_string.ToString());
                return lsStr;
            }
        }

        public static StringBuilder FillNotes(Model.MagnaDb.MagnaDbEntities db, string noteType, string companyCode, string branchCode)
        {

            List<KTTU_NOTE> note = db.KTTU_NOTE.Where(n => n.company_code == companyCode && n.branch_code == branchCode && n.note_type == noteType).ToList();
            StringBuilder NoteDetails = new StringBuilder();
            decimal Spaces = 0M;
            StringBuilder strSpaces = new StringBuilder();

            Spaces = 0;
            strSpaces.Append(' ', Convert.ToInt32(Spaces));
            if (note != null && note.Count != 0) {

                NoteDetails.AppendLine(string.Format("{0}{1}", strSpaces, note[0].line1.ToString()));
                NoteDetails.AppendLine(string.Format("{0}{1}", strSpaces, note[0].line2.ToString()));
                NoteDetails.AppendLine(string.Format("{0}{1}", strSpaces, note[0].line3.ToString()));
                NoteDetails.AppendLine(string.Format("{0}{1}", strSpaces, note[0].line4.ToString()));
                NoteDetails.AppendLine(string.Format("{0}{1}", strSpaces, note[0].line5.ToString()));
                NoteDetails.AppendLine(string.Format("{0}{1}", strSpaces, note[0].line6.ToString()));
                NoteDetails.AppendLine(string.Format("{0}{1}", strSpaces, note[0].line7.ToString()));
                NoteDetails.AppendLine(string.Format("{0}{1}", strSpaces, note[0].line8.ToString()));
                NoteDetails.AppendLine(string.Format("{0}{1}", strSpaces, note[0].line9.ToString()));
                NoteDetails.AppendLine(string.Format("{0}{1}", strSpaces, note[0].line10.ToString()));

            }
            return NoteDetails;
        }

        public static string ComputeHash(string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();
            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);
            return Convert.ToBase64String(byteHash);
        }

        public static string GetMagnaGUID(string tableName, int tranNum, string companyCode, string branchCode)
        {
            return Globals.GetHashcode(tableName + SIGlobals.Globals.Separator + tranNum + SIGlobals.Globals.Separator + companyCode + SIGlobals.Globals.Separator + branchCode);
        }

        public static string GetMagnaGUID(string[] values, string companyCode, string branchCode)
        {
            return Globals.GetHashcode(values, companyCode, branchCode);
        }

        public static string GetHashcode(string value)
        {
            value = value.ToUpper();
            try {
                // First we need to convert the string into bytes, which
                // means using a text encoder.
                Encoder enc = System.Text.Encoding.Unicode.GetEncoder();

                // Create a buffer large enough to hold the string
                byte[] unicodeText = new byte[value.Length * 2];
                enc.GetBytes(value.ToCharArray(), 0, value.Length, unicodeText, 0, true);

                // Now that we have a byte array we can ask the CSP to hash it
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] result = md5.ComputeHash(unicodeText);

                // Build the final string by converting each byte
                // into hex and appending it to a StringBuilder
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < result.Length; i++) {
                    sb.Append(result[i].ToString("X2"));
                }

                // And return it
                return sb.ToString();
            }
            catch {
                return string.Empty;
            }
        }

        public static string GetHashcode(string[] values, string companyCode, string branchCode)
        {
            string temp = string.Empty;

            if (values != null) {
                for (int i = 0; i < values.Length; i++) {
                    temp += values[i].ToString() + Separator;
                }

                temp += companyCode + Separator + branchCode;

                //temp = temp.Remove(temp.Length - 1, Separator.Length);
            }
            temp = temp.ToUpper();
            try {
                // First we need to convert the string into bytes, which
                // means using a text encoder.
                Encoder enc = System.Text.Encoding.Unicode.GetEncoder();

                // Create a buffer large enough to hold the string
                byte[] unicodeText = new byte[temp.Length * 2];
                enc.GetBytes(temp.ToCharArray(), 0, temp.Length, unicodeText, 0, true);

                // Now that we have a byte array we can ask the CSP to hash it
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] result = md5.ComputeHash(unicodeText);

                // Build the final string by converting each byte
                // into hex and appending it to a StringBuilder
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < result.Length; i++) {
                    sb.Append(result[i].ToString("X2"));
                }

                // And return it
                return sb.ToString();
            }
            catch {
                return string.Empty;
            }
        }

        public static string GetcompanyDetailsForPrint(Model.MagnaDb.MagnaDbEntities db, string companyCode, string branchCode)
        {
            KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode && c.branch_code == branchCode).FirstOrDefault();
            string CompanyAddress = string.Empty;
            CompanyAddress = @"<font size=""4"">" + company.company_name.ToString();
            if (company.address1.ToString() != string.Empty)
                CompanyAddress = @"<font size=""3"">" + "<b>" + CompanyAddress + "</b>" + "</font>" + "<br>" + company.address1.ToString();
            if (company.address2.ToString() != string.Empty)
                CompanyAddress = CompanyAddress + "," + company.address2.ToString();
            if (company.address3.ToString() != string.Empty)
                CompanyAddress = CompanyAddress + "," + company.address3.ToString();
            if (company.city.ToString() != string.Empty)
                CompanyAddress = CompanyAddress + "<br>" + company.city.ToString() + "- " + company.pin_code.ToString();
            if (company.phone_no.ToString() != string.Empty)
                CompanyAddress = CompanyAddress + "<br>" + "Phone : " + company.phone_no.ToString();
            if (company.fax_no.ToString() != string.Empty)
                CompanyAddress = CompanyAddress + "Fax : " + company.fax_no.ToString();
            if (company.email_id.ToString() != string.Empty)
                CompanyAddress = CompanyAddress + " E-mail : " + company.email_id.ToString();
            if (company.tin_no.ToString() != string.Empty)
                CompanyAddress = CompanyAddress + " <br>GSTIN : " + company.tin_no.ToString();
            return CompanyAddress;
        }

        public static string GetCompanyDetailsForHTMLPrint(Model.MagnaDb.MagnaDbEntities db, string companyCode, string branchCode)
        {
            KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode && c.branch_code == branchCode).FirstOrDefault();
            if (company != null) {
                StringBuilder sb = new StringBuilder();
                string esc = Convert.ToChar(27).ToString();
                sb.Append(Convert.ToChar(18).ToString());// Cancel Condensed
                sb.Append(esc + Convert.ToChar(77).ToString()); //12CPI
                sb.Append(esc + Convert.ToChar(120).ToString() + Convert.ToChar(48).ToString());//Draft Mode
                sb.AppendLine(string.Format("{0,-80}", "GSTIN : " + company.tin_no.ToString()));
                sb.Append(esc + Convert.ToChar(14).ToString() + Convert.ToChar(14).ToString());
                sb.Append(esc + Convert.ToChar(52).ToString());
                sb.Append(string.Format("{0}{1,30}{2}", esc + Convert.ToChar(69).ToString(), company.company_name.ToString(), esc + Convert.ToChar(70).ToString()));
                sb.Append(esc + Convert.ToChar(53).ToString());
                sb.Append(Convert.ToChar(20).ToString() + Convert.ToChar(10).ToString());
                if (company.Header1.ToString() != string.Empty)
                    sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), company.Header1.ToString()));
                if (company.Header2.ToString() != string.Empty)
                    sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), company.Header2.ToString()));
                if (company.Header3.ToString() != string.Empty)
                    sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), company.Header3.ToString()));
                if (company.Header4.ToString() != string.Empty)
                    sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), company.Header4.ToString()));
                if (company.Header5.ToString() != string.Empty)
                    sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), company.Header5.ToString()));
                if (company.Header6.ToString() != string.Empty)
                    sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), company.Header6.ToString()));
                if (company.Header7.ToString() != string.Empty)
                    sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), company.Header7.ToString()));
                return sb.ToString();
            }
            return "";
        }

        public static string TransactionType(string tranType)
        {
            string transactionType = string.Empty;
            switch (tranType) {
                case
                    "A":
                    transactionType = ""; break;
                case
                    "CO":
                    transactionType = ""; break;
                case
                    "O":
                    transactionType = "Order"; break;
                case
                    "P":
                    transactionType = "Purchase"; break;
                case
                    "PC":
                    transactionType = ""; break;
                case
                    "RO":
                    transactionType = ""; break;
                case
                    "RS":
                    transactionType = ""; break;
                case
                    "S":
                    transactionType = "Sales"; break;
                case
                    "SS":
                    transactionType = ""; break;
            }
            return transactionType;
        }

        public static string PaymentModes(string payMode)
        {
            string paymentMode = string.Empty;
            switch (payMode) {
                case "BC":
                    paymentMode = "OGS";
                    break;
                case "BD":
                    paymentMode = "DKN";
                    break;
                case "BH":
                    paymentMode = "HBR";
                    break;
                case "BJ":
                    paymentMode = "JNR";
                    break;
                case "BK":
                    paymentMode = "KRM";
                    break;
                case "BM":
                    paymentMode = "MNG";
                    break;
                case "BMT":
                    paymentMode = "MRT";
                    break;
                case "BN":
                    paymentMode = "HSN";
                    break;
                case "BP":
                    paymentMode = "PNX";
                    break;
                case "BR":
                    paymentMode = "RJR";
                    break;
                case "BS":
                    paymentMode = "DKNBS";
                    break;
                case "BSM":
                    paymentMode = "SMG";
                    break;
                case "BU":
                    paymentMode = "UDP";
                    break;
                case "C":
                    paymentMode = "Cash";
                    break;
                case "CN":
                    paymentMode = "CREDIT NOTE";
                    break;
                case "CT":
                    paymentMode = "Scheme Adj.";
                    break;
                case "D":
                    paymentMode = "DD";
                    break;
                case "EP":
                    paymentMode = "E-Payment";
                    break;
                case "GV":
                    paymentMode = "Gift Vou";
                    break;
                case "OP":
                    paymentMode = "Order Adj.";
                    break;
                case "PB":
                    paymentMode = "PUR. Bill";
                    break;
                case "PE":
                    paymentMode = "PUR. Est.";
                    break;
                case "Q":
                    paymentMode = "Cheque";
                    break;
                case "R":
                    paymentMode = "Card";
                    break;
                case "SE":
                    paymentMode = "SR EST";
                    break;
                case "SP":
                    paymentMode = "Sales Promotion";
                    break;
                case "SR":
                    paymentMode = "SR Bill";
                    break;
                case "UPI":
                    paymentMode = "UPI Payment";
                    break;
            }
            return paymentMode;
        }

        public static KSTU_CUSTOMER_MASTER GetCustomerDetails(Model.MagnaDb.MagnaDbEntities db, int custID, string companyCode, string branchCode)
        {
            KSTU_CUSTOMER_MASTER c = new KSTU_CUSTOMER_MASTER();
            //var key = ConfigurationManager.AppSettings["UseConsolidatedCustomers"];
            //bool UseConsolidatedCustomers = Convert.ToInt32(key) == 0 ? false : true;
            //if (UseConsolidatedCustomers) {
            //    Customer customers = new Customer();
            //    customers = db.Customers.Where(cust => cust.CustID == custID).FirstOrDefault();
            //    if (customers != null) {
            //        c.obj_id = "";
            //        c.company_code = customers.CompanyCode;
            //        c.branch_code = customers.BranchCode;
            //        c.cust_id = customers.CustID;
            //        c.cust_name = customers.Name;
            //        c.address1 = customers.Address1;
            //        c.address2 = customers.Address2;
            //        c.address3 = customers.Address3;
            //        c.city = customers.City;
            //        c.state = customers.State;
            //        c.pin_code = customers.PinCode;
            //        c.mobile_no = customers.MobileNo;
            //        c.phone_no = customers.PhoneNo;
            //        c.dob = customers.DateOfBirth;
            //        c.wedding_date = customers.WeddingAnnivarsaryDate;
            //        c.customer_type = customers.CustomerType;
            //        c.object_status = customers.ObjectStatus;
            //        c.spouse_name = "";
            //        c.child_name1 = "";
            //        c.child_name2 = "";
            //        c.child_name3 = "";
            //        c.spouse_dob = null;
            //        c.child1_dob = null;
            //        c.child2_dob = null;
            //        c.child3_dob = null;
            //        c.pan_no = "";
            //        c.id_type = "";
            //        c.id_details = "";
            //        c.UpdateOn = customers.UpdatedDate;
            //        c.Email_ID = customers.EmailID;
            //        c.Created_Date = customers.CreatedDate;
            //        c.salutation = customers.Salutation;
            //        c.country_code = customers.CountryCode;
            //        c.loyality_id = customers.LocalityID;
            //        c.ic_no = "";
            //        c.passport_no = "";
            //        c.pr_no = "";
            //        c.privilege_id = "";
            //        c.age = customers.Age;
            //        c.Country_name = customers.CountryName;
            //        c.cust_code = customers.CustCode;
            //        c.cust_credit_limit = null;
            //        c.tin = customers.GSTTIN;
            //        c.state_code = customers.StateCode;
            //        c.Corparate_ID = "";
            //        c.Corporate_Branch_ID = "";
            //        c.Employee_Id = "";
            //        c.Registered_MN = "";
            //        c.profession_ID = "";
            //        c.Empcorp_Email_ID = "";
            //        c.imageid_path = "";
            //        c.corp_imageid_path = "";
            //        c.imageid_path2 = "";
            //        c.AccHolderName = "";
            //        c.Accsalutation = "";
            //        c.RepoCustId = 0;
            //        c.UpdatedDate = customers.UpdatedDate;
            //    }
            //    return c;
            //}
            //else {
            //    c = db.KSTU_CUSTOMER_MASTER.Where(cust => cust.cust_id == custID && cust.company_code == companyCode && cust.branch_code == branchCode).FirstOrDefault();
            //    return c;
            //}

            // Checking the Customer Exist in mis.Customer Table.
            Customer customers = new Customer();
            customers = db.Customers.Where(cust => cust.CustID == custID).FirstOrDefault();

            // Checking Customer Exist in the Local Mater Table by using Mobile Number and Branch.
            KSTU_CUSTOMER_MASTER custMaster = db.KSTU_CUSTOMER_MASTER.Where(cust => cust.mobile_no == customers.MobileNo && cust.branch_code == customers.BranchCode).FirstOrDefault();

            // If Exists Send That customer Information.
            if (custMaster != null) {
                return custMaster;
            }
            else {
                return null;
            }
        }

        public static string ValidateBarcodeDetails(Model.MagnaDb.MagnaDbEntities db, string barcodeNo, string companyCode, string branchCode)
        {
            string Message = string.Empty;
            if (barcodeNo == "") {
                Message = "Invalid Tag No";
                return Message;
            }
            KTTU_BARCODE_MASTER barcode = db.KTTU_BARCODE_MASTER.Where(b => b.barcode_no == barcodeNo && b.company_code == companyCode && b.branch_code == branchCode).FirstOrDefault();
            if (Message == "" && barcode == null) {
                Message = "Barcode details not exist";
                return Message;
            }
            if (barcode.order_no != 0) {
                Message = "TagNo already attached to the order no " + barcode.order_no;
                return Message;
            }
            if (Message == "" && barcode.sold_flag == "Y") {
                Message = "TagNo: " + barcodeNo + " already Billed.";
                return Message;
            }
            if (Message == "" && barcode.order_no != 0) {
                KTTU_ORDER_MASTER kom = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == barcode.order_no && ord.company_code == companyCode && ord.branch_code == branchCode).FirstOrDefault();
                Message = "TagNo: " + barcodeNo + " has done reserved order to Customer: " + kom.cust_name + " on " + kom.order_date.ToString("dd/MM/yyyy");
                return Message;
            }
            return Message;
        }

        public static string ValidateBarcodeDetails(Model.MagnaDb.MagnaDbEntities db, string barcodeNo, string companyCode, string branchCode, string orderNo)
        {
            string Message = string.Empty;
            if (barcodeNo == "") {
                Message = "Invalid Tag No";
                return Message;
            }
            KTTU_BARCODE_MASTER barcode = db.KTTU_BARCODE_MASTER.Where(b => b.barcode_no == barcodeNo && b.company_code == companyCode && b.branch_code == branchCode).FirstOrDefault();
            if (Message == "" && barcode == null) {
                Message = "Barcode details not exist";
                return Message;
            }
            if (!string.IsNullOrEmpty(orderNo)) {
                if (barcode.order_no != 0) {
                    if (barcode.order_no.ToString() != orderNo) {
                        Message = "TagNo already attached to the order no " + barcode.order_no;
                        return Message;
                    }
                }
            }
            
            if (Message == "" && barcode.sold_flag == "Y") {
                Message = "TagNo: " + barcodeNo + " already Billed.";
                return Message;
            }
            //if (Message == "" && barcode.order_no != 0) {
            //    KTTU_ORDER_MASTER kom = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == barcode.order_no && ord.company_code == companyCode && ord.branch_code == branchCode).FirstOrDefault();
            //    Message = "TagNo: " + barcodeNo + " has done reserved order to Customer: " + kom.cust_name + " on " + kom.order_date.ToString("dd/MM/yyyy");
            //    return Message;
            //}
            return Message;
        }

        public static string GetOrderType(string OrderRateType)
        {
            string returnString = string.Empty;
            if (OrderRateType == "Delivery") {
                returnString = "Delivery";
            }
            else if (OrderRateType == "Today") {
                returnString = "Fixed";
            }
            else if (OrderRateType == "Fixed") {
                returnString = "Fixed";
            }
            else if (OrderRateType == "Flexi") {
                returnString = "Flexi";
            }
            return returnString;
        }

        public static string GetGSTGroupCode(Model.MagnaDb.MagnaDbEntities db, string gsCode, string companyCode, string branchCode)
        {
            string gstGroupCode = string.Empty;
            ITEM_MASTER itemMaster = db.ITEM_MASTER.Where(item => item.gs_code == gsCode && item.company_code == companyCode && item.branch_code == branchCode).FirstOrDefault();
            if (itemMaster != null) {
                gstGroupCode = itemMaster.GSTGoodsGroupCode;
            }
            return gstGroupCode;
        }

        public static string GetHSN(Model.MagnaDb.MagnaDbEntities db, string gsCode, string companyCode, string branchCode)
        {
            string HSN = string.Empty;
            ITEM_MASTER itemMaster = db.ITEM_MASTER.Where(item => item.gs_code == gsCode && item.company_code == companyCode && item.branch_code == branchCode).FirstOrDefault();
            if (itemMaster != null) {
                HSN = itemMaster.HSN;
            }
            return HSN;
        }

        public static string GetItemName(Model.MagnaDb.MagnaDbEntities db, string gsCode, string itemCode, string companyCode, string branchCode)
        {
            string itemName = string.Empty;
            ITEM_MASTER itemMaster = db.ITEM_MASTER.Where(item => item.gs_code == gsCode
                                                            && item.Item_code == itemCode
                                                            && item.company_code == companyCode
                                                            && item.branch_code == branchCode).FirstOrDefault();
            if (itemMaster != null) {
                itemName = itemMaster.Item;
            }
            return itemName;
        }

        public static string GetOrderImagePath(Model.MagnaDb.MagnaDbEntities db)
        {
            string ordImagePath = ConfigurationManager.AppSettings["OrderImagePath"].ToString();
            return ordImagePath;
        }

        public static string GetWebConfigValue(string webConfigKey = null)
        {
            string imageUrl = string.Empty;
            if (string.IsNullOrEmpty(webConfigKey))
                webConfigKey = "ImageUrl";
            if (ConfigurationManager.AppSettings[webConfigKey] != null)
                imageUrl = ConfigurationManager.AppSettings[webConfigKey].ToString();
            return imageUrl;
        }

        public static int? GetApplicationConfigurationSettnigs(Model.MagnaDb.MagnaDbEntities db, string companyCode, string branchCode, string objectID)
        {
            int? value = 0;
            APP_CONFIG_TABLE appConfig = db.APP_CONFIG_TABLE.Where(config => config.company_code == companyCode && config.branch_code == branchCode && config.obj_id == objectID).FirstOrDefault();
            if (appConfig != null) {
                value = appConfig.value;
            }
            return value;
        }

        public static StringBuilder FillCompanyDetails(string parameter, string companyCode, string branchCode)
        {
            DataTable dtFillCompanyDetails = Globals.ExecuteQuery(string.Format("select {0} from KSTU_COMPANY_MASTER Where company_code = '{1}' and branch_code = '{2}'"
                , parameter, companyCode, branchCode));
            StringBuilder CompanyDetails = new StringBuilder();
            decimal Spaces = 0M;
            StringBuilder strSpaces = new StringBuilder();
            Spaces = 0;
            strSpaces.Append(' ', Convert.ToInt32(Spaces));
            CompanyDetails.Append(string.Format("{0}{1}", strSpaces, dtFillCompanyDetails.Rows[0][0].ToString().Trim()));
            return CompanyDetails;
        }

        public static StringBuilder GetCompanyname(Model.MagnaDb.MagnaDbEntities db, string companyCode, string branchCode)
        {
            KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(com => com.company_code == companyCode && com.branch_code == branchCode).FirstOrDefault();
            StringBuilder sb = new StringBuilder();
            string esc = Convert.ToChar(27).ToString();
            sb.Append(Convert.ToChar(18).ToString());
            sb.Append(esc + Convert.ToChar(77).ToString());
            sb.Append(esc + Convert.ToChar(120).ToString() + Convert.ToChar(48).ToString());
            sb.AppendLine(string.Format("{0,-80}", "GSTIN : " + company.tin_no.ToString()));
            sb.Append(esc + Convert.ToChar(14).ToString() + Convert.ToChar(14).ToString());
            sb.Append(esc + Convert.ToChar(52).ToString());
            sb.Append(string.Format("{0}{1,30}{2}", esc + Convert.ToChar(69).ToString(), company.company_name.ToString(), esc + Convert.ToChar(70).ToString()));
            sb.Append(esc + Convert.ToChar(53).ToString());
            sb.Append(Convert.ToChar(20).ToString() + Convert.ToChar(10).ToString());
            if (company.Header1.ToString() != string.Empty)
                sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), company.Header1.ToString()));
            if (company.Header2.ToString() != string.Empty)
                sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), company.Header2.ToString()));
            if (company.Header3.ToString() != string.Empty)
                sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), company.Header3.ToString()));
            if (company.Header4.ToString() != string.Empty)
                sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), company.Header4.ToString()));
            if (company.Header5.ToString() != string.Empty)
                sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), company.Header5.ToString()));
            if (company.Header6.ToString() != string.Empty)
                sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), company.Header6.ToString()));
            if (company.Header7.ToString() != string.Empty)
                sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), company.Header7.ToString()));
            return sb;
        }

        public static String GetAliasName(Model.MagnaDb.MagnaDbEntities db, string gsCode, string itemName, string companyCode, string branchCode)
        {
            //ITEM_MASTER itemMaster = new ITEM_MASTER();
            //itemMaster = db.ITEM_MASTER.Where(item => item.gs_code == gsCode
            //                                    && item.Item_code == itemName
            //                                    && item.company_code == companyCode
            //                                    && item.branch_code == branchCode).FirstOrDefault();
            //return itemMaster == null ? "" : itemMaster.Item_Name.ToString();

            // The above LINQ query returns wrong object, So written bellow query on 27/02/2021
            string name = string.Empty;
            DataTable tblItemMaster = SIGlobals.Globals.ExecuteQuery("SELECT  im.[Item Name] FROM dbo.ITEM_MASTER im WHERE im.Item_code='"
                + itemName + "' AND im.gs_code='"
                + gsCode + "' AND im.company_code='"
                + companyCode + "' AND im.branch_code='"
                + branchCode + "'");
            if (tblItemMaster != null && tblItemMaster.Rows.Count > 0) {
                name = tblItemMaster.Rows[0][0].ToString();
            }
            return name;
        }

        public static string GetMetalType(Model.MagnaDb.MagnaDbEntities db, string gsType, string companyCode, string branchCode)
        {
            KSTS_GS_ITEM_ENTRY itemEntry = db.KSTS_GS_ITEM_ENTRY.Where(item => item.gs_code == gsType && item.company_code == companyCode && item.branch_code == branchCode).FirstOrDefault();
            return itemEntry == null ? "" : itemEntry.metal_type;
        }

        public static string IsPeiceItem(Model.MagnaDb.MagnaDbEntities db, string gsType, string itemName, string companyCode, string branchCode)
        {
            ITEM_MASTER itemMaster = db.ITEM_MASTER.Where(item => item.gs_code == gsType && item.Item_code == itemName && item.company_code == companyCode && item.branch_code == branchCode).FirstOrDefault();
            return itemMaster == null ? "N" : itemMaster.IsPiece;
        }

        public static decimal GetRate(Model.MagnaDb.MagnaDbEntities db, string companyCode, string branchCode, string gsCode, string karat)
        {
            KSTU_RATE_MASTER rateMaster = db.KSTU_RATE_MASTER.Where(r => r.gs_code == gsCode
                                                                    && (r.karat == karat
                                                                    || r.karat == "NA"
                                                                    || r.karat == "")
                                                                    && r.company_code == companyCode
                                                                    && r.branch_code == branchCode).FirstOrDefault();
            return rateMaster == null ? 0 : rateMaster.rate;
        }

        public static DataTable ConvertListToDataTable<T>(List<T> list)
        {
            var json = JsonConvert.SerializeObject(list);
            DataTable dataTable = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
            return dataTable;
        }

        public static DataTable ConvertDataTableToList(DataTable table)
        {
            var json = JsonConvert.SerializeObject(table);
            DataTable dataTable = (DataTable)JsonConvert.DeserializeObject(json, typeof(List<dynamic>));
            return dataTable;
        }

        public static string GetBank(Model.MagnaDb.MagnaDbEntities db, string bank, string payMode, string companyCode, string branchCode)
        {
            string bankName = string.Empty;
            if (bank == "") {
                return "";
            }
            int accCode = Convert.ToInt32(bank);
            KTTS_CARD_COMMISSION cardCommission = new KTTS_CARD_COMMISSION();
            if (payMode == "R") {
                cardCommission = db.KTTS_CARD_COMMISSION.Where(cc => cc.company_code == companyCode
                                                     && cc.branch_code == branchCode
                                                     && cc.acc_code == accCode).FirstOrDefault();
            }
            return bankName = cardCommission == null ? "" : cardCommission.bank;
        }

        public static int GetNewSerialNo(Model.MagnaDb.MagnaDbEntities db, string objID, string companyCode, string branchCode)
        {
            return db.KSTS_ACC_SEQ_NOS.Where(ac => ac.obj_id == objID && ac.company_code == companyCode && ac.branch_code == branchCode).FirstOrDefault().nextno;
        }

        public static KSTU_TOLERANCE_MASTER GetTollerance(MagnaDbEntities db, int objID, string companyCode, string branchCode)
        {
            KSTU_TOLERANCE_MASTER tollerance = db.KSTU_TOLERANCE_MASTER.Where(tol => tol.obj_id == objID
                                                     && tol.company_code == companyCode
                                                     && tol.branch_code == branchCode).FirstOrDefault();
            return tollerance;
        }

        public static int GetConfigurationValue(MagnaDbEntities db, string objID, string companyCode, string branchCode)
        {
            APP_CONFIG_TABLE appConfig = db.APP_CONFIG_TABLE.Where(config => config.company_code == companyCode
                                                                    && config.branch_code == branchCode
                                                                    && config.obj_id == objID).FirstOrDefault();
            return appConfig == null ? 0 : Convert.ToInt32(appConfig.value);
        }

        public static string Base64Encode(string plainText)
        {
            string encodedValue = string.Empty;
            try {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
                encodedValue = System.Convert.ToBase64String(plainTextBytes);
            }
            catch (Exception) {
            }
            return encodedValue;
        }
        public static string Base64decode(string plainText)
        {
            string encodedValue = string.Empty;
            try {
                byte[] toEncryptArray = Convert.FromBase64String(plainText);
                encodedValue = UTF8Encoding.UTF8.GetString(toEncryptArray).ToString();
            }
            catch (Exception) {
            }
            return encodedValue;
        }

        public static int GetLetterCount(string text)
        {
            if (text == null)
                return 0;
            int numberOfTextChars = text.Length - text.Count(Char.IsWhiteSpace);
            return numberOfTextChars;
        }

        public static int GetAccountSeqNo(MagnaDbEntities db, string moduleSeqNo, string companyCode, string branchCode)
        {
            int finYear = SIGlobals.Globals.GetFinancialYear(db, companyCode, branchCode);
            int seqNo = Convert.ToInt32(finYear.ToString().Remove(0, 1) +
                              db.KSTS_ACC_SEQ_NOS.Where(sq => sq.obj_id == moduleSeqNo
                                                                     && sq.company_code == companyCode
                                                                     && sq.branch_code == branchCode).FirstOrDefault().nextno);
            return seqNo;
        }
        public static int GetAccountVoucherSeqNo(MagnaDbEntities db, string moduleSeqNo, string companyCode, string branchCode)
        {
            int finYear = SIGlobals.Globals.GetFinancialYear(db, companyCode, branchCode);
            int seqNo = Convert.ToInt32(finYear.ToString().Remove(0, 1) +
                              db.KSTS_ACC_VOUCHER_SEQ_NOS.Where(sq => sq.obj_id == moduleSeqNo
                                                                     && sq.company_code == companyCode
                                                                     && sq.branch_code == branchCode).FirstOrDefault().nextno);
            return seqNo;

        }

        //Credit to Habib for better answer. https://stackoverflow.com/questions/11412956/what-is-the-best-way-of-validating-an-ip-address
        /// <summary>
        /// Validates IP Address Version 4
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static bool ValidateIPv4(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress)) {
                return false;
            }

            string[] splitValues = ipAddress.Split('.');
            if (splitValues.Length != 4) {
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }
        public static string LeftN(string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            int lengthToTruncate = maxLength;
            if (str.Length < maxLength)
                lengthToTruncate = str.Length;
            return str.Substring(0, lengthToTruncate-1);
        }

        public static int GetAccCodeFromAccPostingSetup(MagnaDbEntities db, string gsCode, string tranType, string companyCode, string branchCode)
        {
            KSTS_ACC_POSTING_SETUP accPostingSetup = db.KSTS_ACC_POSTING_SETUP.Where(p => p.company_code == companyCode
                                                                                    && p.branch_code == branchCode
                                                                                    && p.gs_code == gsCode
                                                                                    && p.trans_type == tranType).FirstOrDefault();
            return accPostingSetup == null ? 0 : accPostingSetup.acc_code;
        }

        public static string GetLedgerName(MagnaDbEntities db, int accCode, string companyCode, string branhCode)
        {
            KSTU_ACC_LEDGER_MASTER ledgerMaster = db.KSTU_ACC_LEDGER_MASTER.Where(led => led.acc_code == accCode
                                                                                    && led.company_code == companyCode
                                                                                    && led.branch_code == branhCode).FirstOrDefault();
            return ledgerMaster == null ? "" : ledgerMaster.acc_name;
        }

        public static string GetStyleNewforBH()
        {
            StringBuilder sbStyle = new StringBuilder();
            if (string.IsNullOrEmpty(sbStyle.ToString())) {
                #region Fill Style
                sbStyle.AppendLine("<style = 'text/css'>\n");
                sbStyle.AppendLine(".boldText, .boldtable TD, .boldtable TH\n");
                sbStyle.AppendLine("{\n");
                sbStyle.AppendLine("margin: 10px 0px 20px 0px;\n");
                sbStyle.AppendLine("font-family: 'Times New Roman' ,'Arial Narrow', Arial, monospace;\n");
                sbStyle.AppendLine("font-size:8.5pt;\n");
                sbStyle.AppendLine("}\n");
                sbStyle.AppendLine(".tableStyle\n");
                sbStyle.AppendLine("{\n");
                sbStyle.AppendLine("border-left: 0px solid black;\n");
                sbStyle.AppendLine("border-right: 0px solid black;\n");
                sbStyle.AppendLine("border-top-style:none;\n");
                sbStyle.AppendLine("border-bottom-style:none;\n");
                sbStyle.AppendLine("border-collapse: collapse;\n");
                sbStyle.AppendLine("}\n");
                sbStyle.AppendLine(".table,TD,TH\n");
                sbStyle.AppendLine("{\n");
                sbStyle.AppendLine("border-left: 1px solid black;\n");
                sbStyle.AppendLine("border-right: 1px solid black;\n");
                sbStyle.AppendLine("border-bottom-style:none;\n");
                sbStyle.AppendLine("border-top-style:1px solid\n");
                sbStyle.AppendLine("border-collapse: collapse;\n");
                sbStyle.AppendLine("}\n");
                sbStyle.AppendLine(".noborder\n");
                sbStyle.AppendLine("{\n");
                sbStyle.AppendLine("border-left-style:none;\n");
                sbStyle.AppendLine("border-right-style:none;\n");
                sbStyle.AppendLine("border-top-style:none;;\n");
                sbStyle.AppendLine("border-bottom-style:none;\n");
                sbStyle.AppendLine("border-collapse: collapse;\n");
                sbStyle.AppendLine("}\n");
                sbStyle.AppendLine("</style>\n");
                #endregion
            }
            return sbStyle.ToString();
        }

        public static string GetVAcode(decimal VAPercent)
        {
            string VAcode = string.Empty;
            string[] MC = { "R", "B", "H", "I", "M", "A", "T", "O", "W", "E" };
            string VA = Convert.ToString(VAPercent);
            for (int i = 0; i < VA.Length; i++) {
                string x = VA.Substring(i, 1);
                if (string.Compare(x, ".") == 0) {
                    VAcode = VAcode + "*";
                }
                else {
                    int y = Convert.ToInt32(x);
                    VAcode = VAcode + MC[y];
                }
            }
            return VAcode + "#";
        }

        public static string GetStyleOrder()
        {
            StringBuilder sbStyle = new StringBuilder();
            if (string.IsNullOrEmpty(sbStyle.ToString())) {
                #region Fill Style

                sbStyle.AppendLine("<style = 'text/css'>\n");
                sbStyle.AppendLine(".boldText, .boldtable TD, .boldtable TH\n");
                sbStyle.AppendLine("{\n");
                sbStyle.AppendLine("font-family: 'Times New Roman' ,'Arial Narrow', Arial, monospace;\n");
                sbStyle.AppendLine("font-size:10pt;\n");
                sbStyle.AppendLine("}\n");
                sbStyle.AppendLine(".tableStyle\n");
                sbStyle.AppendLine("{\n");
                sbStyle.AppendLine("border-left: 0px solid black;\n");
                sbStyle.AppendLine("border-right: 0px solid black;\n");
                sbStyle.AppendLine("border-top-style:none;\n");
                sbStyle.AppendLine("border-bottom-style:none;\n");
                sbStyle.AppendLine("border-collapse: collapse;\n");
                sbStyle.AppendLine("}\n");
                sbStyle.AppendLine(".table,TD,TH\n");
                sbStyle.AppendLine("{\n");
                sbStyle.AppendLine("border-left-style:none;\n");
                sbStyle.AppendLine("border-right: 1px solid black;\n");
                sbStyle.AppendLine("border-bottom-style:none;\n");
                sbStyle.AppendLine("border-top-style:1px solid\n");
                sbStyle.AppendLine("border-collapse: collapse;\n");
                sbStyle.AppendLine("}\n");
                sbStyle.AppendLine(".noborder\n");
                sbStyle.AppendLine("{\n");
                sbStyle.AppendLine("border-left-style:none;\n");
                sbStyle.AppendLine("border-right-style:none;\n");
                sbStyle.AppendLine("border-top-style:none;;\n");
                sbStyle.AppendLine("border-bottom-style:none;\n");
                sbStyle.AppendLine("border-collapse: collapse;\n");
                sbStyle.AppendLine("}\n");
                sbStyle.AppendLine("</style>\n");
                #endregion
            }
            return sbStyle.ToString();
        }

        public static int GetDocumentNo(MagnaDbEntities dbContext, string companyCode, string branchCode, string docSeqNo, bool prefixFinYear = true)
        {
            int documentNo = 0;
            var seqNos = dbContext.KSTS_SEQ_NOS.Where(sq => sq.obj_id == docSeqNo
                                    && sq.company_code == companyCode
                                    && sq.branch_code == branchCode).FirstOrDefault();
            if (seqNos == null) {
                throw new Exception("Document series " + docSeqNo + " does not exist.");
            }
            int nextNo = seqNos.nextno;

            if (prefixFinYear == true) {
                string finYear = Globals.GetFinancialYear(dbContext, companyCode, branchCode).ToString().Remove(0, 1);
                documentNo = Convert.ToInt32(finYear + nextNo.ToString());
            }
            else {
                documentNo = nextNo;
            }
            return documentNo;
        }

        public static void IncrementDocumentNo(MagnaDbEntities dbContext, string companyCode, string branchCode, string docSeqNo)
        {
            var seqNos = dbContext.KSTS_SEQ_NOS.Where(sq => sq.obj_id == docSeqNo
                                    && sq.company_code == companyCode
                                    && sq.branch_code == branchCode).FirstOrDefault();
            if (seqNos == null) {
                throw new Exception("Document series " + docSeqNo + " does not exist.");
            }
            seqNos.nextno = seqNos.nextno + 1;
            dbContext.Entry(seqNos).State = System.Data.Entity.EntityState.Modified;
        }

        public static int GetGSTAccountCodeReceived(MagnaDbEntities db, string HSN, decimal GSTPercent, bool? registered, string componentCode, string branchCode, string companyCode)
        {
            GSTPostingSetup gstPostingSetup = db.GSTPostingSetups.Where(gst => gst.company_code == companyCode
                                                                        && gst.branch_code == branchCode
                                                                        && gst.GSTGroupCode == HSN
                                                                        && gst.GSTPercent == GSTPercent
                                                                        && gst.IsRegistered == registered).FirstOrDefault();

            if (gstPostingSetup == null) {
                return 0;
            }
            else {
                return Convert.ToInt32(gstPostingSetup.ReceivableAccount);
            }
        }

        public static int GetAccountCodeForTDSExpense(MagnaDbEntities db, int accCode, string companyCode, string branchCode)
        {
            KSTU_ACC_LEDGER_MASTER accLedger = db.KSTU_ACC_LEDGER_MASTER.Where(acc => acc.company_code == companyCode
                                                                                && acc.branch_code == branchCode
                                                                                && acc.acc_code == accCode).FirstOrDefault();
            return accLedger == null ? 0 : accLedger.acc_code;
        }

        public static int GetMagnaAccountCode(MagnaDbEntities db, int obj_id, string companyCode, string branchCode)
        {
            KSTS_ACC_CODE_MASTER accCodeMaster = db.KSTS_ACC_CODE_MASTER.Where(acc => acc.company_code == companyCode
                                                                                && acc.branch_code == branchCode
                                                                                && acc.obj_id == obj_id).FirstOrDefault();
            return accCodeMaster == null ? 0 : accCodeMaster.acc_code;
        }

        public static DateTime GetDefaultDate()
        {
            return new DateTime(1901, 1, 1);
        }

        public static DataTable GetDataTable(string sql)
        {
            using (var context = new MagnaDbEntities()) {
                var dt = new DataTable();
                var conn = context.Database.Connection;
                var connectionState = conn.State;
                try {
                    if (connectionState != ConnectionState.Open)
                        conn.Open();
                    using (var cmd = conn.CreateCommand()) {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.Text;
                        using (var reader = cmd.ExecuteReader()) {
                            dt.Load(reader);
                        }
                    }
                }
                catch (Exception) {
                    throw;
                }
                finally {
                    if (connectionState != ConnectionState.Closed)
                        conn.Close();
                }
                return dt;
            }
        }

        public static string ConvertNumbertoWordsinRupees(decimal ItemAmount)
        {
            string strFrWords = string.Empty;
            string strWhWordss = string.Empty;
            ItemAmount = decimal.Round(ItemAmount, 2, MidpointRounding.ToEven);
            string[] arrNumber = ItemAmount.ToString().Split('.');
            long fractionPart = 0;
            // Get the whole number text
            Double wholePart = long.Parse(arrNumber[0]);
            NumberToWords objNWClass = new NumberToWords();
            objNWClass.ConvertNumberToWords(Convert.ToDouble(wholePart), out strWhWordss);
            strWhWordss = strWhWordss.Replace("Rupees Only", "");
            strWhWordss = "Rupees " + strWhWordss;
            if (arrNumber.Length > 1) {
                fractionPart = long.Parse((arrNumber[1].Length == 1 ? arrNumber[1] + "0" : arrNumber[1]));

            }
            if (arrNumber.Length > 1 && (Convert.ToInt32(arrNumber[1]) != 0)) {
                // If the length of the fractional element is only 1, add a 0 so that the text returned isn't,
                // 'One', 'Two', etc but 'Ten', 'Twenty', etc.

                if (fractionPart > 0) {
                    objNWClass.ConvertNumberToWords(Convert.ToDouble(fractionPart), out strFrWords);
                    strFrWords = strFrWords.Replace(" Rupees", "");
                    strWhWordss += (fractionPart == 1 ? " and Paisa " + strFrWords : " and Paise " + strFrWords);
                }
            }
            else {
                strWhWordss = strWhWordss + " Only";
            }
            return strWhWordss;
        }

        public static string GetVendor(MagnaDbEntities db, string party, string companyCode, string branchCode)
        {
            KSTU_SUPPLIER_MASTER supplier = db.KSTU_SUPPLIER_MASTER.Where(sp => sp.company_code == companyCode
                                                                        && sp.branch_code == branchCode
                                                                        && sp.party_code == party).FirstOrDefault();
            return supplier == null ? "" : supplier.party_name;

        }

        public static KSTU_SUPPLIER_MASTER GetSupplier(MagnaDbEntities db, string companyCode, string branchCode, string partyCode)
        {
            KSTU_SUPPLIER_MASTER supplier = db.KSTU_SUPPLIER_MASTER.Where(s => s.company_code == companyCode
                                                                                && s.branch_code == branchCode
                                                                                && s.party_code == partyCode).FirstOrDefault();
            return supplier;
        }

        public static string GetBranchGSName(MagnaDbEntities db, string companyCode, string branchCode, string gsCode)
        {
            KSTS_GS_ITEM_ENTRY gsItemEntry = db.KSTS_GS_ITEM_ENTRY.Where(gs => gs.gs_code == gsCode && gs.company_code == companyCode && gs.branch_code == branchCode).FirstOrDefault();
            return gsItemEntry == null ? "" : gsItemEntry.item_level1_name;
        }


        #region FTP upload & encryption/decryption
        public static string Encrypt(string plainText,
                                     string passPhrase,
                                     string saltValue,
                                     string hashAlgorithm,
                                     int passwordIterations,
                                     string initVector,
                                     int keySize)
        {
            // Convert strings into byte arrays.
            // Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8 
            // encoding.
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            // Convert our plaintext into a byte array.
            // Let us assume that plaintext contains UTF8-encoded characters.
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // First, we must create a password, from which the key will be derived.
            // This password will be generated from the specified passphrase and 
            // salt value. The password will be created using the specified hash 
            // algorithm. Password creation can be done in several iterations.
            PasswordDeriveBytes password = new PasswordDeriveBytes(
                                                            passPhrase,
                                                            saltValueBytes,
                                                            hashAlgorithm,
                                                            passwordIterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = password.GetBytes(keySize / 8);

            // Create uninitialized Rijndael encryption object.
            RijndaelManaged symmetricKey = new RijndaelManaged();

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;

            // Generate encryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key 
            // bytes.
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(
                                                             keyBytes,
                                                             initVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = new MemoryStream();

            // Define cryptographic stream (always use Write mode for encryption).
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                         encryptor,
                                                         CryptoStreamMode.Write);
            // Start encrypting.
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

            // Finish encrypting.
            cryptoStream.FlushFinalBlock();

            // Convert our encrypted data from a memory stream into a byte array.
            byte[] cipherTextBytes = memoryStream.ToArray();

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert encrypted data into a base64-encoded string.
            string cipherText = Convert.ToBase64String(cipherTextBytes);

            // Return encrypted string.
            return cipherText;
        }

        /// <summary>
        /// Decrypts specified ciphertext using Rijndael symmetric key algorithm.
        /// </summary>
        /// <param name="cipherText">
        /// Base64-formatted ciphertext value.
        /// </param>
        /// <param name="passPhrase">
        /// Passphrase from which a pseudo-random password will be derived. The
        /// derived password will be used to generate the encryption key.
        /// Passphrase can be any string. In this example we assume that this
        /// passphrase is an ASCII string.
        /// </param>
        /// <param name="saltValue">
        /// Salt value used along with passphrase to generate password. Salt can
        /// be any string. In this example we assume that salt is an ASCII string.
        /// </param>
        /// <param name="hashAlgorithm">
        /// Hash algorithm used to generate password. Allowed values are: "MD5" and
        /// "SHA1". SHA1 hashes are a bit slower, but more secure than MD5 hashes.
        /// </param>
        /// <param name="passwordIterations">
        /// Number of iterations used to generate password. One or two iterations
        /// should be enough.
        /// </param>
        /// <param name="initVector">
        /// Initialization vector (or IV). This value is required to encrypt the
        /// first block of plaintext data. For RijndaelManaged class IV must be
        /// exactly 16 ASCII characters long.
        /// </param>
        /// <param name="keySize">
        /// Size of encryption key in bits. Allowed values are: 128, 192, and 256.
        /// Longer keys are more secure than shorter keys.
        /// </param>
        /// <returns>
        /// Decrypted string value.
        /// </returns>
        /// <remarks>
        /// Most of the logic in this function is similar to the Encrypt
        /// logic. In order for decryption to work, all parameters of this function
        /// - except cipherText value - must match the corresponding parameters of
        /// the Encrypt function which was called to generate the
        /// ciphertext.
        /// </remarks>
        public static string Decrypt(string cipherText,
                                     string passPhrase,
                                     string saltValue,
                                     string hashAlgorithm,
                                     int passwordIterations,
                                     string initVector,
                                     int keySize)
        {
            // Convert strings defining encryption key characteristics into byte
            // arrays. Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8
            // encoding.
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            // Convert our ciphertext into a byte array.
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            // First, we must create a password, from which the key will be 
            // derived. This password will be generated from the specified 
            // passphrase and salt value. The password will be created using
            // the specified hash algorithm. Password creation can be done in
            // several iterations.
            PasswordDeriveBytes password = new PasswordDeriveBytes(
                                                            passPhrase,
                                                            saltValueBytes,
                                                            hashAlgorithm,
                                                            passwordIterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = password.GetBytes(keySize / 8);

            // Create uninitialized Rijndael encryption object.
            RijndaelManaged symmetricKey = new RijndaelManaged();

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;

            // Generate decryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key 
            // bytes.
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(
                                                             keyBytes,
                                                             initVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            // Define cryptographic stream (always use Read mode for encryption).
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                          decryptor,
                                                          CryptoStreamMode.Read);

            // Since at this point we don't know what the size of decrypted data
            // will be, allocate the buffer long enough to hold ciphertext;
            // plaintext is never longer than ciphertext.
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            // Start decrypting.
            int decryptedByteCount = cryptoStream.Read(plainTextBytes,
                                                       0,
                                                       plainTextBytes.Length);

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert decrypted data into a string. 
            // Let us assume that the original plaintext string was UTF8-encoded.
            string plainText = Encoding.UTF8.GetString(plainTextBytes,
                                                       0,
                                                       decryptedByteCount);

            // Return decrypted string.   
            return plainText;
        }
        public static bool GetFTPValues(int ftpSlNo, out string host, out string userName, out string password, out string folder)
        {
            host = "";
            userName = "";
            password = "";
            folder = "";

            using (var dbContext = new MagnaDbEntities()) {
                var ftpDetails = dbContext.KSTU_FTP_CONFIG.Where(x => x.sl_no == ftpSlNo).FirstOrDefault();
                if (ftpDetails != null) {
                    host = DecryptString(ftpDetails.Host);
                    userName = DecryptString(ftpDetails.UserName);
                    password = DecryptString(ftpDetails.Password);
                    folder = DecryptString(ftpDetails.Folder);
                    return true;
                }
                else
                    return false;
            }
        }

        public static bool Upload(string filename, int ftpSlNo, out string errorMessage)
        {
            errorMessage = string.Empty;
            Stream strm = null;
            FileStream fs = null;
            try {
                string ftpHost, userName, password, folderName;
                if (!GetFTPValues(ftpSlNo, out ftpHost, out userName, out password, out folderName)) {
                    errorMessage = "Failed to get FTP values.";
                    return false;
                }
                FileInfo fileInf = new FileInfo(filename);

                //string uri = "ftp://" + tifp + "/" + @ftpFolderName + "/" + fileInf.Name;
                string uri = string.Format("{0}//{1}//{2}//{3} ", "ftp:", ftpHost, folderName, fileInf.Name);
                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));

                // Provide the WebPermission Credintials
                reqFTP.Proxy = null;
                reqFTP.Credentials = new NetworkCredential(userName, password);

                // By default KeepAlive is true, where the control connection
                // is not closed after a command is executed.
                reqFTP.KeepAlive = false;

                // Specify the command to be executed.
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

                // Specify the data transfer type.
                reqFTP.UseBinary = true;

                // Notify the server about the size of the uploaded file
                reqFTP.ContentLength = fileInf.Length;

                // The buffer size is set to 2kb
                int buffLength = 2048;
                byte[] buff = new byte[buffLength];
                int contentLen;

                // Opens a file stream (System.IO.FileStream) to read the file
                // to be uploaded
                fs = fileInf.OpenRead();

                // Stream to which the file to be upload is written
                strm = reqFTP.GetRequestStream();

                // Read from the file stream 2kb at a time
                contentLen = fs.Read(buff, 0, buffLength);

                // Till Stream content ends
                while (contentLen != 0) {
                    // Write Content from the file stream to the FTP Upload
                    // Stream
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }

            }
            catch (Exception ex) {
                errorMessage = ex.Message;
                return false;
            }
            finally {
                if (strm != null)
                    strm.Close();
                if (fs != null)
                    fs.Close();
            }
            return true;
        }

        public static bool Download(string filePath, string fileName, int ftpSlNo, out string errorMsg)
        {
            errorMsg = string.Empty;
            FtpWebRequest reqFTP;
            FileStream outputStream = null;
            FtpWebResponse response = null;
            Stream ftpStream = null;
            try {


                string ftpHost, userName, password, ftpFolderName;

                try {
                    if (!GetFTPValues(ftpSlNo, out ftpHost, out userName, out password, out ftpFolderName)) {
                        errorMsg = "Failed to get FTP values.";
                        return false;
                    }

                    outputStream = new FileStream(filePath + "\\" +
                      fileName, FileMode.Create);

                    //reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://"+ tifp + "/" + ftpFolderName + "/" + fileName));
                    string uri = string.Format("{0}//{1}//{2}//{3} ", "ftp:", ftpHost, ftpFolderName, fileName);

                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                    reqFTP.Proxy = null;
                    reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                    reqFTP.UseBinary = true;
                    reqFTP.Credentials = new NetworkCredential(userName,
                                                               password);
                    response = (FtpWebResponse)reqFTP.GetResponse();
                    ftpStream = response.GetResponseStream();
                    long cl = response.ContentLength;
                    int bufferSize = 2048;
                    int readCount;
                    byte[] buffer = new byte[bufferSize];

                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0) {
                        outputStream.Write(buffer, 0, readCount);
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                    }
                }
                catch (Exception ex) {
                    outputStream.Close();
                    if (ex.Message.Contains("(505)"))
                        errorMsg = string.Format("{0}{1}{2}", "File by name : \"", fileName, "\" not found");
                    else if (ex.Message.Contains("(530)"))
                        errorMsg = string.Format("{0}", "Unable to login to FTP,\n 'set the UserId and Password'");
                    else if (ex.Message.Contains("(550)"))
                        errorMsg = "File Not found";
                    else
                        errorMsg = "Download Error";// ShowMessage(string.Format("{0}{1}", ex.Message, "Download Error"), MessageType.Error);
                    return false;
                }
            }
            catch {
                return false;
            }
            finally {
                if (ftpStream != null)
                    ftpStream.Close();
                if (outputStream != null)
                    outputStream.Close();
                if (response != null)
                    response.Close();
            }
            return true;
        }

        public static string DecryptString(string Text)
        {
            string passPhrase = "Pas5pr@se";        // can be any string
            string saltValue = "s@1tValue";        // can be any string
            string hashAlgorithm = "SHA1";             // can be "MD5"
            int passwordIterations = 2;                  // can be any number
            string initVector = "@1B2c3D4e5F6g7H8"; // must be 16 bytes
            int keySize = 256;                // can be 192 or 128
            string cipherText = Decrypt(Text, passPhrase, saltValue, hashAlgorithm, passwordIterations, initVector, keySize);
            return cipherText;
        }

        public static string EncryptString(string Text)
        {
            string passPhrase = "Pas5pr@se";        // can be any string
            string saltValue = "s@1tValue";        // can be any string
            string hashAlgorithm = "SHA1";             // can be "MD5"
            int passwordIterations = 2;                  // can be any number
            string initVector = "@1B2c3D4e5F6g7H8"; // must be 16 bytes
            int keySize = 256;                // can be 192 or 128

            string cipherText = Encrypt(Text, passPhrase, saltValue, hashAlgorithm, passwordIterations, initVector, keySize);
            return cipherText;
        }
        #endregion

        public static void CreateDirectoryIfNotExist(string path)
        {
            if (!Directory.Exists(path)) {
                DirectoryInfo dir = new DirectoryInfo(path);
                dir.Create();
            }
        }

        public static string GetStyle()
        {
            StringBuilder sbStyle = new StringBuilder();
            #region Fill Style
            sbStyle.AppendLine("<style = 'text/css'>\n");

            sbStyle.AppendLine(".boldText, .boldtable TD, .boldtable TH\n");
            sbStyle.AppendLine("{\n");
            //sbStyle.AppendLine("position: fixed; \n");
            //sbStyle.AppendLine("font-family: 'Courier New', Courier, monospace;\n");
            sbStyle.AppendLine("font-family: 'Times New Roman';\n");
            sbStyle.AppendLine("font-size:12pt;\n");
            //sbStyle.AppendLine("font-weight: bold;\n");          
            sbStyle.AppendLine("}\n");

            sbStyle.AppendLine(".tableStyle\n");
            sbStyle.AppendLine("{\n");
            sbStyle.AppendLine("border-left: 1px solid black;\n");
            sbStyle.AppendLine("border-right: 1px solid black;\n");
            //border-left-style:none;
            sbStyle.AppendLine("border-top-style:none;\n");
            sbStyle.AppendLine("border-bottom-style:none;\n");

            sbStyle.AppendLine("border-collapse: collapse;\n");
            sbStyle.AppendLine("}\n");

            sbStyle.AppendLine(".table,TD,TH\n");
            sbStyle.AppendLine("{\n");
            sbStyle.AppendLine("border-left-style:none;\n");
            sbStyle.AppendLine("border-right: 1px solid black;\n");

            sbStyle.AppendLine("border-bottom-style:none;\n");
            sbStyle.AppendLine("border-top-style:1px solid;\n");
            sbStyle.AppendLine("border-collapse: collapse;\n");
            sbStyle.AppendLine("}\n");


            sbStyle.AppendLine(".noborder\n");
            sbStyle.AppendLine("{\n");
            sbStyle.AppendLine("border-left-style:none;\n");
            sbStyle.AppendLine("border-right-style:none;\n");
            sbStyle.AppendLine("border-top-style:none;;\n");
            sbStyle.AppendLine("border-bottom-style:none;\n");
            sbStyle.AppendLine("border-collapse: collapse;\n");
            sbStyle.AppendLine("}\n");

            sbStyle.AppendLine(".fullborder\n");
            sbStyle.AppendLine("{\n");
            //sbStyle.AppendLine("border-left: 1px solid black;\n");
            sbStyle.AppendLine("border-left-style:none;;\n");
            sbStyle.AppendLine("border-right: 1px solid black;\n");
            sbStyle.AppendLine("border-bottom: 1px solid black;\n");
            sbStyle.AppendLine("border-top: 1px solid black;\n");
            sbStyle.AppendLine("border-collapse: collapse;\n");
            sbStyle.AppendLine("}\n");

            sbStyle.AppendLine("</style>\n");
            #endregion

            return sbStyle.ToString();
        }

        public static string GetIRTypeName(MagnaDbEntities db, string companyCode, string branchCode, string irCode)
        {
            KSTU_ISSUERECEIPTS_TYPES irTypes = db.KSTU_ISSUERECEIPTS_TYPES.Where(ir => ir.company_code == companyCode && ir.branch_code == branchCode && ir.ir_code == irCode).FirstOrDefault();
            return irTypes == null ? "" : irTypes.ir_name;
        }

        public static bool IsValidBranch(string branchCode)
        {
            if (string.IsNullOrEmpty(branchCode))
                return false;
            return new MagnaDbEntities().KSTU_COMPANY_MASTER.Where(x => x.branch_code == branchCode).Count() > 0 ? true : false;
        }

        public static bool IsValidCompany(string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return false;
            return new MagnaDbEntities().KSTU_COMPANY_MASTER.Where(x => x.company_code == companyCode).Count() > 0 ? true : false;
        }        

        public static string Base64Decode(string base64EncodedData)
        {
            string decodedString = string.Empty;
            try {
                var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                decodedString = Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch (Exception) {
            }
            return decodedString;
        }

        public static DateTime GetEndingTimeOfDay(DateTime date)
        {
            DateTime endingTimeofDay = date.Date;
            endingTimeofDay = endingTimeofDay.AddSeconds(59).AddMinutes(59).AddHours(23);
            return endingTimeofDay;
        }

        public static void WriteTransactionLog(string userID, string logMessage, string transType, MagnaDbEntities db = null)
        {
            try {
                DateTime updatedDate = GetDateTime();
                KSTS_LOG_DETAILS logDetail = new KSTS_LOG_DETAILS
                {
                    date = updatedDate,
                    Description = logMessage,
                    operator_code = userID,
                    Fin_Year = GetIndianFinYear(updatedDate),
                    machine_name = "::",
                    trans_type = "N"
                };
                if (db == null)
                    db = new MagnaDbEntities(true);
                db.KSTS_LOG_DETAILS.Add(logDetail);
                db.SaveChanges();
            }
            catch (Exception) {
                ;
            }
        }

        public static int GetIndianFinYear(DateTime date)
        {
            if (date.Month > 3)
                return date.Year;
            else
                return date.Year - 1;
        }

        public static T GetScalarValue<T>(string sql)
        {
            T result = default(T);
            var dt = new DataTable();
            using (var dbContext = new MagnaDbEntities(true)) {
                var conn = dbContext.Database.Connection;
                var connectionState = conn.State;
                try {
                    if (connectionState != ConnectionState.Open)
                        conn.Open();
                    using (var cmd = conn.CreateCommand()) {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.Text;
                        object scalar = cmd.ExecuteScalar();
                        if (scalar != null && scalar != DBNull.Value) {
                            result = (T)Convert.ChangeType(scalar, typeof(T));
                        }
                    }
                }
                catch (Exception) {
                    throw;
                }
            }
            return result;

        }

        public static T GetScalarValue<T>(string sql, MagnaDbEntities dbContext)
        {
            T result = default(T);
            var dt = new DataTable();
            var conn = dbContext.Database.Connection;
            var connectionState = conn.State;
            try {
                if (connectionState != ConnectionState.Open)
                    conn.Open();
                using (var cmd = conn.CreateCommand()) {
                    var tran = conn.BeginTransaction();
                    cmd.Transaction = tran;
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;
                    object scalar = cmd.ExecuteScalar();
                    if (scalar != null && scalar != DBNull.Value) {
                        result = (T)Convert.ChangeType(scalar, typeof(T));
                    }
                    tran.Commit();
                }
                
            }
            catch (Exception) {
                throw;
            }

            return result;

        }

    }
    public enum TransactionsType
    {
        Sales = 'S',
        Order = 'O',
        BranchIssue = 'B',
        BranchReceipt = 'R'
    }

    public enum ActionType
    {
        Block = 'B',
        UnBlock = 'U'
    }

    public enum MarketPlace
    {
        Store = 0,
        Amazon = 1,
        Instore = 2,
        BhimaEcom = 3
    }

    public enum OrderType
    {
        ReservedOrder = 'R',
        CustomizedOrder = 'O',
        OrderAdvance = 'A'
    }

    public enum TransactionEntryType
    {

    }
}
