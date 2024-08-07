using NumberToWordsINR;
using ProdigyAPI.BL.BusinessLayer.Accounts;
using ProdigyAPI.BL.ViewModel.Accounts;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ProdigyAPI.Controllers.Accounts
{
    public class AccountsCommonBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Account Common Methods
        public bool UpdateVourcherDetails(int voucherNo, AccVoucherTransactionsVM voucherDet, out ErrorVM error)
        {
            error = null;
            KSTU_ACC_VOUCHER_TRANSACTIONS voucher = db.KSTU_ACC_VOUCHER_TRANSACTIONS.Where(acc => acc.company_code == voucherDet.CompanyCode
                                                                                            && acc.branch_code == voucherDet.BranchCode
                                                                                            && acc.voucher_no == voucherDet.VoucherNo
                                                                                            && acc.trans_type == voucherDet.TransType
                                                                                            && acc.acc_code_master == voucherDet.AccountCodeMaster).FirstOrDefault();
            if (voucher == null) {
                error = new ErrorVM()
                {
                    description = "Details not found.",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return false;
            }
            voucher.acc_code = voucherDet.AccountCode;
            voucher.dr_amount = voucherDet.DebitAmount;
            voucher.cr_amount = voucherDet.CreditAmount;
            voucher.narration = voucherDet.Narration;
            voucher.chq_no = voucherDet.ChequeNo;
            voucher.chq_date = voucherDet.ChequeDate;
            voucher.receipt_no = voucherDet.ReceiptNo;
            voucher.UpdateOn = SIGlobals.Globals.GetDateTime();
            try {
                db.Entry(voucher).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public AccVoucherTransactionsVM UpdateVourcherDetails(int voucherNo, List<AccVoucherTransactionsVM> v, out ErrorVM error)
        {
            error = null;
            AccVoucherTransactionsVM retObj = new AccVoucherTransactionsVM();
            foreach (AccVoucherTransactionsVM voucherDet in v) {
                KSTU_ACC_VOUCHER_TRANSACTIONS voucher = db.KSTU_ACC_VOUCHER_TRANSACTIONS.Where(acc => acc.company_code == voucherDet.CompanyCode
                                                                                                && acc.branch_code == voucherDet.BranchCode
                                                                                                && acc.voucher_no == voucherDet.VoucherNo
                                                                                                && acc.txt_seq_no == voucherDet.TxtSeqNo
                                                                                                && acc.trans_type == voucherDet.TransType
                                                                                                && acc.acc_code == voucherDet.AccountCode
                                                                                                && acc.acc_code_master == voucherDet.AccountCodeMaster).FirstOrDefault();
                if (voucher == null) {
                    error = new ErrorVM()
                    {
                        description = "Details not found.",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                    return null;
                }
                voucher.acc_code = voucherDet.AccountCode;
                voucher.acc_code_master = voucherDet.AccountCodeMaster;
                voucher.dr_amount = voucherDet.DebitAmount;
                voucher.cr_amount = voucherDet.CreditAmount;
                voucher.narration = voucherDet.Narration;
                voucher.chq_no = voucherDet.ChequeNo;
                voucher.chq_date = voucherDet.ChequeDate;
                voucher.receipt_no = voucherDet.ReceiptNo;
                voucher.UpdateOn = SIGlobals.Globals.GetDateTime();
                db.Entry(voucher).State = System.Data.Entity.EntityState.Modified;
                retObj = new AccVoucherTransactionsVM() { VoucherNo = voucher.voucher_no, AccountCodeMaster = voucher.acc_code_master, AccountCode = voucher.acc_code, TransType = voucher.trans_type, AccountType = voucher.acc_type };
            }
            try {
                db.SaveChanges();
                return retObj;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public bool DeleteVourcherDetails(AccVoucherTransactionsVM voucherDet, out ErrorVM error)
        {
            error = null;
            KSTU_ACC_VOUCHER_TRANSACTIONS voucher = db.KSTU_ACC_VOUCHER_TRANSACTIONS.Where(acc => acc.company_code == voucherDet.CompanyCode
                                                                                            && acc.branch_code == voucherDet.BranchCode
                                                                                            && acc.voucher_no == voucherDet.VoucherNo
                                                                                            && acc.trans_type == voucherDet.TransType
                                                                                            && acc.acc_code == voucherDet.AccountCode
                                                                                            && acc.acc_code_master == voucherDet.AccountCodeMaster).FirstOrDefault();
            if (voucher == null) {
                error = new ErrorVM()
                {
                    description = "Details not found.",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return false;
            }
            if (voucher.cflag == "Y") {
                error = new ErrorVM()
                {
                    description = "Voucher cancelled already.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            if(SIGlobals.Globals.GetApplicationDate().Date != voucher.voucher_date.Date) {
                error = new ErrorVM { description = "Voucher with posted in the current date can only be cancelled.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return false;
            }
            voucher.cflag = "Y";
            voucher.Cancelled_by = voucherDet.CancelledBy;
            voucher.Cancelled_remarks = voucherDet.CancelledRemarks;
            voucher.UpdateOn = SIGlobals.Globals.GetDateTime();
            try {
                db.Entry(voucher).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public bool DeleteVourcherDetails(List<AccVoucherTransactionsVM> voucherDet, out ErrorVM error)
        {
            error = null;
            AccVoucherTransactionsVM retObj = new AccVoucherTransactionsVM();
            try {
                string cancelledRemarks = voucherDet[0].CancelledRemarks;
                string cancelledBy = voucherDet[0].CancelledBy;
                foreach (AccVoucherTransactionsVM v in voucherDet) {
                    KSTU_ACC_VOUCHER_TRANSACTIONS voucher = db.KSTU_ACC_VOUCHER_TRANSACTIONS.Where(acc => acc.company_code == v.CompanyCode
                                                                                                && acc.branch_code == v.BranchCode
                                                                                                && acc.voucher_no == v.VoucherNo
                                                                                                && acc.trans_type == v.TransType
                                                                                                && acc.acc_code == v.AccountCode
                                                                                                && acc.acc_code_master == v.AccountCodeMaster).FirstOrDefault();
                    if (voucher == null) {
                        error = new ErrorVM()
                        {
                            description = "Details not found.",
                            ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                        };
                        return false;
                    }
                    if (voucher.cflag == "Y") {
                        error = new ErrorVM()
                        {
                            description = "Vouche cancelled already.",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }

                    voucher.cflag = "Y";
                    voucher.Cancelled_by = cancelledBy;
                    voucher.Cancelled_remarks = cancelledRemarks;
                    voucher.UpdateOn = SIGlobals.Globals.GetDateTime();
                    db.Entry(voucher).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public string Print(string companyCode, string branchCode, int voucherNo, int accCodeMaster, string accType, string tranType, out ErrorVM error)
        {
            // For Contra accType="N"
            // For Cash Voucher Entry accType="C"
            // For Bank Voucher Entry accType="B" 

            error = null;
            var data = (from av in db.KSTU_ACC_VOUCHER_TRANSACTIONS
                        join ad in db.KSTU_ACC_LEDGER_MASTER
                        on new { Company = av.company_code, Branch = av.branch_code, AccCode = av.acc_code }
                        equals new { Company = ad.company_code, Branch = ad.branch_code, AccCode = ad.acc_code }
                        where av.acc_type == accType && av.contra_seq_no == 0
                                                    && av.acc_code_master == accCodeMaster
                                                    && av.voucher_no == voucherNo
                                                    && av.trans_type == tranType
                                                    && av.company_code == companyCode
                                                    && av.branch_code == branchCode
                        select new { av, ad }).ToList();

            if (data.Count == 0) {
                error = new ErrorVM()
                {
                    description = "Invalid Voucher Details",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return "";
            }

            int accCode = data[0].av.acc_code;
            string accountName = db.KSTU_ACC_LEDGER_MASTER.Where(acc => acc.company_code == companyCode
                                                                    && acc.branch_code == branchCode
                                                                    && acc.acc_code == accCode).FirstOrDefault().acc_name;
            string accCodeMasterName = db.KSTU_ACC_LEDGER_MASTER.Where(acc => acc.company_code == companyCode
                                                                     && acc.branch_code == branchCode
                                                                     && acc.acc_code == accCodeMaster).FirstOrDefault().acc_name;
            KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode && c.branch_code == branchCode).FirstOrDefault();
            StringBuilder sb = new StringBuilder();
            try {

                if (accType == "B") {
                    sb.AppendLine("<html>");
                    sb.AppendLine("<head>");
                    sb.AppendLine(SIGlobals.Globals.GetStyleForBill());
                    sb.AppendLine("</head>");
                    sb.AppendLine("<body>");

                    sb.AppendLine("<TABLE WIDTH = 550 >");
                    sb.AppendLine(string.Format("<TR class = \"boldText\"><TH><H5><b ALIGN = \"RIGHT\" >{0}</b></H5></TH></TR>", SIGlobals.Globals.GetcompanyDetailsForPrint(db, companyCode, branchCode)));
                    sb.AppendLine("</TABLE>");

                    sb.AppendLine("<TABLE class = \"boldtable\">");
                    sb.AppendLine(string.Format("<TR><TD WIDTH = 400 ALIGN = \"LEFT\">{0}</TD>", ""));
                    sb.AppendLine(string.Format("<TD WIDTH = 200 ALIGN = \"RIGHT\">No : {0}</TD></TR>", voucherNo));
                    sb.AppendLine(string.Format("<TR><TD WIDTH = 400 ALIGN = \"LEFT\">{0}</TD>", ""));
                    sb.AppendLine(string.Format("<TD WIDTH = 200 ALIGN = \"RIGHT\">Date: {0}</TD></TR>", data[0].av.voucher_date.ToString("dd/MM/yyyy").Trim()));
                    sb.AppendLine("</TABLE>");

                    sb.AppendLine("<TABLE class = \"boldtable\">");
                    sb.AppendLine("<TR><TD NOWRAP WIDTH = 610 ALIGN = \"CENTER\" class = \"billstyle\">BANK VOUCHER - " + branchCode + "</TD></TR>");
                    sb.AppendLine("</TABLE>");
                    sb.AppendLine("<TABLE border =\"0\" class = \"boldtable\">");
                    sb.AppendLine("<TR><Td NOWRAP WIDTH = 300 ALIGN = \"LEFT\"><B>DESCRIPTION</B></Td>");
                    sb.AppendLine("<Td NOWRAP WIDTH = 100 ALIGN = \"RIGHT\"><B>Narration<B></Td>");
                    sb.AppendLine("<Td NOWRAP WIDTH = 200 ALIGN = \"RIGHT\"><B>Amount</B></Td></TR>");
                    decimal TotalDr = 0, TotalCr = 0, Amount = 0;
                    for (int i = 0; i < data.Count; i++) {
                        TotalDr += Convert.ToDecimal(data[i].av.dr_amount);
                        TotalCr += Convert.ToDecimal(data[i].av.cr_amount);
                    }
                    Amount = TotalDr + TotalCr;
                    sb.AppendLine("</TABLE>");
                    sb.AppendLine("<TABLE class = \"boldtable\">");
                    sb.AppendLine("<TR><TD WIDTH = 610 ALIGN = \"LEFT\" class = \"billstyle\">&nbsp</TD></TR>");
                    sb.AppendLine("</TABLE>");

                    string name = string.Empty, narration = string.Empty;
                    string accountNames = string.Empty, narr = string.Empty;
                    for (int i = 0; i < data.Count; i++) {
                        List<string> lstAccountName = new List<string>();
                        List<string> lstNarration = new List<string>();
                        accountNames = data[i].ad.acc_name.ToString();
                        narration = data[i].av.narration.ToString();

                        sb.AppendLine("<TABLE class = \"boldtable\">");
                        if (accountNames.Length <= 29)
                            sb.AppendLine(string.Format("<TR><TD WIDTH = 300 ALIGN = \"LEFT\" >{0}</TD>", accountNames));
                        else {
                            lstAccountName = SIGlobals.Globals.SplitStringAt(29, accountNames);

                            for (int k = 0; k < lstAccountName.Count; k++) {
                                name += lstAccountName[k] + " ";
                            }
                            sb.AppendLine(string.Format("<TR> <TD WIDTH = 300 ALIGN = \"LEFT\" >{0}</TD>", name));
                        }

                        if (narration.Length <= 29)
                            sb.AppendLine(string.Format("<TD WIDTH = 200 ALIGN = \"LEFT\" >{0}</TD>", narration));
                        else {
                            narr = string.Empty;
                            lstNarration = SIGlobals.Globals.SplitStringAt(45, narration);

                            for (int k = 0; k < lstNarration.Count; k++) {
                                narr += lstNarration[k] + " ";
                            }
                            sb.AppendLine(string.Format("<TD WIDTH = 200 ALIGN = \"LEFT\" >{0}</TD>", narr));
                        }
                        if (Convert.ToDecimal(data[i].av.dr_amount) > 0)
                            sb.AppendLine(string.Format("<TD WIDTH = 100 ALIGN = \"RIGHT\" >{0}</TD></TR>", data[i].av.dr_amount.ToString()));
                        else
                            sb.AppendLine(string.Format("<TD WIDTH = 100 ALIGN = \"RIGHT\" >{0}</TD></TR>", data[i].av.cr_amount.ToString()));

                    }
                    sb.AppendLine("<TR><TD ALIGN = \"RIGHT\" colspan=\"3\">-----------</TD></TR>");
                    sb.AppendLine(string.Format("<TR><TD WIDTH = 300 ALIGN = \"RIGHT\" >{0}</TD>", ""));
                    sb.AppendLine(string.Format("<TD WIDTH = 100 ALIGN = \"RIGHT\" >TOTAL</TD>", ""));
                    sb.AppendLine(string.Format("<TD WIDTH = 100 ALIGN = \"RIGHT\" >{0}</TD></TR>", Amount.ToString("N")));
                    sb.AppendLine("<TR><TD ALIGN = \"RIGHT\" colspan=\"3\">-----------</TD></TR>");

                    if (data[0].av.acc_type.Equals("C"))
                        sb.AppendLine(string.Format("<TR><TD colspan=\"3\">{0}</TD> </TR>", data[0].ad.acc_name));
                    else {
                        sb.AppendLine(string.Format("<TR><TD colspan=\"3\">{0}</TD> </TR>", accCodeMasterName));
                        if (string.Compare(data[0].av.trans_type, "CONT") != 0) {
                            sb.AppendLine(string.Format("<TR><TD colspan=\"3\">Cheque No: {0}</TD>  </TR>", data[0].av.chq_no));
                        }
                    }
                    //sb.AppendLine(string.Format("<TR><TD colspan=\"3\">Narration :{0}</TD> </TR>", data[0].av.narration));
                    sb.AppendLine("</TABLE>");
                    sb.AppendLine("<TABLE class = \"boldtable\" >");
                    sb.AppendLine("<TR><TD WIDTH = 610 ALIGN = \"LEFT\" class = \"billstyle\">&nbsp</TD></TR>");
                    sb.AppendLine("</TABLE>");

                    sb.AppendLine("<TABLE class = \"boldtable\">");
                    sb.AppendLine(string.Format("<TR><TD WIDTH = 600 ALIGN = \"LEFT\">{0}</TD>", SIGlobals.Globals.FillNotes(db, "PUB", companyCode, branchCode)));
                    //sb.AppendLine(string.Format("<TR><TD WIDTH = 600 ALIGN = \"LEFT\"> TIN No:{0}/CST No:{1}</TD></TR>", company.tin_no, company.cst_no));
                    sb.AppendLine("</TABLE>");
                    sb.AppendLine("<BR/><BR/>");
                    sb.AppendLine("<TABLE class = \"boldtable\">");
                    sb.AppendLine("<TR><TD WIDTH = 200 ALIGN = \"LEFT\"><b>RECEIVED BY</b></TD><TD WIDTH = 200 ALIGN = \"CENTER\"><b>VERIFIED BY</b></TD><TD WIDTH = 200 ALIGN = \"RIGHT\"><b>APPROVED BY</b></TD></TR>");
                    sb.AppendLine("</TABLE>");
                    sb.AppendLine("</body>");
                    sb.AppendLine("<html>");
                }
                else if (accType == "C") {
                    sb.AppendLine("<html>");
                    sb.AppendLine("<head>");
                    sb.AppendLine(SIGlobals.Globals.GetStyleForBill());
                    sb.AppendLine("</head>");
                    sb.AppendLine("<body>");
                    sb.AppendLine("<Table style=\"border-collapse:collapse\" frame=\"border\" border=\"1\" width=\"900\"  >");
                    sb.AppendLine(string.Format("<TR ><TH colspan=2 style=\"border-bottom:thin;\" ALIGN = \"center\" ><b>{0}</b></TH></TR>", SIGlobals.Globals.GetcompanyDetailsForPrint(db, companyCode, branchCode)));
                    sb.AppendLine(string.Format("<TR ><TH colspan=2 style=\"border-top:thin;\" ALIGN = \"center\" ><b>{0}</b></TH></TR>", ""));
                    sb.AppendLine(string.Format("<TR><TD style=\"border-bottom:thin;border-right:thin\" ALIGN = \"LEFT\">Branch : {0}</TD>", branchCode));
                    sb.AppendLine(string.Format("<TD style=\"border-bottom:thin;border-left:thin\" ALIGN = \"RIGHT\">Voucher No : {0}</TD></TR>", voucherNo));
                    if (string.Compare(Convert.ToString(data[0].av.trans_type), "REC") == 0)
                        sb.AppendLine(string.Format("<TR><TD style=\"border-top:thin;border-bottom:thin;border-right:thin\"  ALIGN = \"LEFT\">Credit </TD>"));
                    else if (string.Compare(Convert.ToString(data[0].av.trans_type), "PAY") == 0 || string.Compare(Convert.ToString(data[0].av.trans_type), "JOU") == 0)
                        sb.AppendLine(string.Format("<TR><TD style=\"border-top:thin;border-bottom:thin;border-right:thin\" ALIGN = \"LEFT\">Debit </TD>"));
                    sb.AppendLine(string.Format("<TD style=\"border-top:thin;border-bottom:thin;border-left:thin\" ALIGN = \"RIGHT\">Date : {0}</TD></TR>", data[0].av.voucher_date.ToString("dd/MM/yyyy")));
                    sb.AppendLine("<br><br><br>");
                    if (accType.Equals("C")) {
                        if (accCodeMaster == 1) {
                            if (string.Compare(Convert.ToString(data[0].av.trans_type), "REC") == 0)
                                sb.AppendLine("<TR><TD  colspan=2 style=\"border-top:thin;border-bottom:thin\" ALIGN = \"CENTER\"><b><u>CASH CREDIT VOUCHER</u><br><br><br></b></TD></TR>");
                            else if (string.Compare(Convert.ToString(data[0].av.trans_type), "PAY") == 0 || string.Compare(Convert.ToString(data[0].av.trans_type), "JOU") == 0)
                                sb.AppendLine("<TR><TD colspan=2 style=\"border-top:thin;border-bottom:thin\" ALIGN = \"CENTER\"><b><u>CASH VOUCHER</u> <br><br><br> </b></TD></TR>");
                        }
                        else {
                            if (string.Compare(Convert.ToString(data[0].av.trans_type), "REC") == 0)
                                sb.AppendLine(string.Format("<TR><TD colspan=2 style=\"border-top:thin;border-bottom:thin\" ALIGN = \"CENTER\"><b><u>{0} </u></b></TD></TR>", accCodeMasterName));


                            else if (string.Compare(Convert.ToString(data[0].av.trans_type), "PAY") == 0 || string.Compare(Convert.ToString(data[0].av.trans_type), "JOU") == 0)
                                sb.AppendLine(string.Format("<TR><TD colspan=2 style=\"border-top:thin;border-bottom:thin\" ALIGN = \"CENTER\"><b><u>{0} <br><br><br> </u></b></TD></TR>", accCodeMasterName));
                        }
                    }
                    sb.AppendLine(string.Format("<TR><TD style=\"border-top:thin;border-bottom:thin;border-right:thin\" ALIGN = \"LEFT\">Head Off. A/c : {0}</TD>", data[0].av.party_name));
                    decimal amount = 0.0M;
                    if (string.Compare(Convert.ToString(data[0].av.trans_type), "REC") == 0)
                        amount = Convert.ToDecimal(data[0].av.cr_amount);
                    else if (string.Compare(Convert.ToString(data[0].av.trans_type), "PAY") == 0 || string.Compare(Convert.ToString(data[0].av.trans_type), "JOU") == 0)
                        amount = Convert.ToDecimal(data[0].av.dr_amount);
                    sb.AppendLine(string.Format("<TD style=\"border-top:thin;border-bottom:thin;border-left:thin\" ALIGN = \"RIGHT\"><h4> <u>Amount : {0} </u> </h4></TD></TR>", amount.ToString("N")));
                    string strWords = string.Empty; ;

                    NumberToWords objNWClass = new NumberToWords();
                    objNWClass.ConvertNumberToWords(Convert.ToDouble(amount), out strWords);
                    strWords = strWords.Replace("Rupees", "");
                    strWords = "Rupees " + strWords;
                    sb.AppendLine(string.Format("<TR><TD colspan=2 style=\"border-top:thin;border-bottom:thin\"  ALIGN = \"left\">&nbsp</TD></TR>"));
                    sb.AppendLine(string.Format("<TR><TD colspan=2 style=\"border-top:thin;border-bottom:thin\" ALIGN = \"left\"> {0}</TD></TR>", strWords));
                    sb.AppendLine(string.Format("<TR><TD colspan=2 style=\"border-top:thin;border-bottom:thin\"   ALIGN = \"left\">&nbsp</TD></TR>"));


                    sb.AppendLine(string.Format("<TR><TD colspan=2 style=\"border-top:thin;border-bottom:thin\"  ALIGN = \"left\">towards : {0}</TD></TR>", data[0].av.narration));
                    sb.AppendLine(string.Format("<TR><TD colspan=2 style=\"border-top:thin;border-bottom:thin\"  ALIGN = \"left\">&nbsp</TD></TR>"));
                    decimal TotalDr = 0, TotalCr = 0, Amount = 0;
                    for (int i = 0; i < data.Count; i++) {
                        TotalDr += Convert.ToDecimal(data[i].av.dr_amount);
                        TotalCr += Convert.ToDecimal(data[i].av.cr_amount);
                    }
                    Amount = TotalDr + TotalCr;
                    sb.AppendLine("</TABLE>");
                    sb.AppendLine("<Table style=\"border-collapse:collapse\" frame=\"border\" border=\"1\" width=\"900\"  >");

                    if (string.Compare(Convert.ToString(data[0].av.trans_type), "REC") == 0) {
                        sb.AppendLine("<TR><TD width=\"300\" ALIGN = \"center\"><b><br><br><br>ENTERED BY</b></TD><TD ALIGN = \"center\" width=\"300\" ><br><br><br><b>RECEIVED FROM</b></TD><TD   width=\"300\" ALIGN = \"center\"><b><br><br><br> PASSING OFFICIAL/STORE INCHARGE</b></TD></TR>");
                    }
                    else if (string.Compare(Convert.ToString(data[0].av.trans_type), "PAY") == 0 || string.Compare(Convert.ToString(data[0].av.trans_type), "JOU") == 0) {
                        sb.AppendLine("<TR><TD  width=\"300\" ALIGN = \"center\"><br><br><br><b>ENTERED BY</b></TD><TD ALIGN = \"center\" width=\"300\" ><b><br><br><br>RECEIVED BY</b></TD><TD  width=\"300\" ALIGN = \"center\"><b><br><br><br> PASSING OFFICIAL/STORE INCHARGE</b></TD></TR>");
                    }
                    sb.AppendLine("</TABLE>");
                    sb.AppendLine("</body>");
                    sb.AppendLine("</html>");
                }
                else if (accType == "N") {
                    sb.AppendLine("<html>");
                    sb.AppendLine("<head>");
                    sb.AppendLine(SIGlobals.Globals.GetStyleForBill());
                    sb.AppendLine("</head>");
                    sb.AppendLine("<body>");

                    sb.AppendLine("<TABLE WIDTH = 550 >");
                    sb.AppendLine(string.Format("<TR class = \"boldText\"><TH><H2><b ALIGN = \"RIGHT\" >{0}</b></H2></TH></TR>", SIGlobals.Globals.GetcompanyDetailsForPrint(db, companyCode, branchCode)));
                    sb.AppendLine("</TABLE>");

                    sb.AppendLine("<TABLE class = \"boldtable\">");
                    sb.AppendLine(string.Format("<TR><TD WIDTH = 400 ALIGN = \"LEFT\">{0}</TD>", ""));
                    sb.AppendLine(string.Format("<TD WIDTH = 200 ALIGN = \"RIGHT\">No : {0}/{1}</TD></TR>", branchCode, voucherNo));
                    sb.AppendLine(string.Format("<TR><TD WIDTH = 400 ALIGN = \"LEFT\">{0}</TD>", ""));
                    sb.AppendLine(string.Format("<TD WIDTH = 200 ALIGN = \"RIGHT\">Date: {0}</TD></TR>", data[0].av.voucher_date.ToString("dd/MM/yyyy").Trim()));
                    sb.AppendLine("</TABLE>");

                    sb.AppendLine("<TABLE class = \"boldtable\">");
                    if (accType.Equals("C"))
                        sb.AppendLine("<TR><TD NOWRAP WIDTH = 610 ALIGN = \"CENTER\" class = \"billstyle\">CASH VOUCHER</TD></TR>");
                    else if (accType.Equals("N")) {
                        sb.AppendLine("<TR><TD NOWRAP WIDTH = 610 ALIGN = \"CENTER\" class = \"billstyle\">CONTRA ENTRY</TD></TR>");
                    }
                    else
                        sb.AppendLine("<TR><TD NOWRAP WIDTH = 610 ALIGN = \"CENTER\" class = \"billstyle\">BANK VOUCHER</TD></TR>");
                    sb.AppendLine("</TABLE>");


                    sb.AppendLine("<TABLE border =\"0\" class = \"boldtable\">");

                    sb.AppendLine("<TR><Td NOWRAP WIDTH = 300 ALIGN = \"LEFT\"><B>DESCRIPTION</B></Td>");
                    sb.AppendLine("<Td NOWRAP WIDTH = 100 ALIGN = \"RIGHT\"><B>Narration<B></Td>");
                    sb.AppendLine("<Td NOWRAP WIDTH = 200 ALIGN = \"RIGHT\"><B>Amount</B></Td></TR>");


                    decimal TotalDr = 0, TotalCr = 0, Amount = 0;
                    for (int i = 0; i < data.Count; i++) {
                        TotalDr += Convert.ToDecimal(data[i].av.dr_amount);
                        TotalCr += Convert.ToDecimal(data[i].av.cr_amount);
                    }
                    Amount = TotalDr + TotalCr;  //Its added assuming either any one present

                    sb.AppendLine("</TABLE>");

                    sb.AppendLine("<TABLE class = \"boldtable\">");
                    sb.AppendLine("<TR><TD WIDTH = 610 ALIGN = \"LEFT\" class = \"billstyle\">&nbsp</TD></TR>");
                    sb.AppendLine("</TABLE>");

                    string name = string.Empty, narration = string.Empty;
                    string accountNames = string.Empty, narr = string.Empty;
                    for (int i = 0; i < data.Count; i++) {
                        List<string> lstAccountName = new List<string>();
                        List<string> lstNarration = new List<string>();
                        accountNames = data[i].ad.acc_name.ToString();
                        narration = data[i].av.narration.ToString();

                        sb.AppendLine("<TABLE class = \"boldtable\">");
                        if (accountNames.Length <= 29)
                            sb.AppendLine(string.Format("<TR><TD WIDTH = 300 ALIGN = \"LEFT\" >{0}</TD>", accountNames));
                        else {
                            lstAccountName = SIGlobals.Globals.SplitStringAt(29, accountNames);

                            for (int k = 0; k < lstAccountName.Count; k++) {
                                name += lstAccountName[k] + " ";
                            }
                            sb.AppendLine(string.Format("<TR> <TD WIDTH = 300 ALIGN = \"LEFT\" >{0}</TD>", name));
                        }

                        if (narration.Length <= 29)
                            sb.AppendLine(string.Format("<TD WIDTH = 200 ALIGN = \"LEFT\" >{0}</TD>", narration));
                        else {
                            narr = string.Empty;
                            lstNarration = SIGlobals.Globals.SplitStringAt(45, narration);

                            for (int k = 0; k < lstNarration.Count; k++) {
                                narr += lstNarration[k] + " ";
                            }
                            sb.AppendLine(string.Format("<TD WIDTH = 200 ALIGN = \"LEFT\" >{0}</TD>", narr));
                        }
                        if (Convert.ToDecimal(data[i].av.dr_amount) > 0)
                            sb.AppendLine(string.Format("<TD WIDTH = 100 ALIGN = \"RIGHT\" >{0}</TD></TR>", data[i].av.dr_amount.ToString()));
                        else
                            sb.AppendLine(string.Format("<TD WIDTH = 100 ALIGN = \"RIGHT\" >{0}</TD></TR>", data[i].av.cr_amount.ToString()));

                    }
                    sb.AppendLine("<TR><TD ALIGN = \"RIGHT\" colspan=\"3\">-----------</TD></TR>");
                    sb.AppendLine(string.Format("<TR><TD WIDTH = 300 ALIGN = \"RIGHT\" >{0}</TD>", ""));
                    sb.AppendLine(string.Format("<TD WIDTH = 100 ALIGN = \"RIGHT\" >TOTAL</TD>", ""));
                    sb.AppendLine(string.Format("<TD WIDTH = 100 ALIGN = \"RIGHT\" >{0}</TD></TR>", Amount.ToString("N")));
                    sb.AppendLine("<TR><TD ALIGN = \"RIGHT\" colspan=\"3\">-----------</TD></TR>");


                    sb.AppendLine(string.Format("<TR><TD colspan=\"3\">{0}</TD> </TR>", accCodeMasterName));
                    if (string.Compare(data[0].av.trans_type, "CONT") != 0) {
                        sb.AppendLine(string.Format("<TR><TD colspan=\"3\">Cheque No: {0}</TD>  </TR>", data[0].av.chq_no));
                    }



                    sb.AppendLine(string.Format("<TR><TD colspan=\"3\">Narration :{0}</TD> </TR>", data[0].av.narration));
                    sb.AppendLine("</TABLE>");


                    sb.AppendLine("<TABLE class = \"boldtable\" >");
                    sb.AppendLine("<TR><TD WIDTH = 610 ALIGN = \"LEFT\" class = \"billstyle\">&nbsp</TD></TR>");
                    sb.AppendLine("</TABLE>");

                    sb.AppendLine("<TABLE class = \"boldtable\">");
                    sb.AppendLine(string.Format("<TR><TD WIDTH = 600 ALIGN = \"LEFT\">{0}</TD>", SIGlobals.Globals.FillNotes(db, "PUB", companyCode, branchCode)));
                    sb.AppendLine(string.Format("<TR><TD WIDTH = 600 ALIGN = \"LEFT\"> TIN No:{0}/CST No:{1}</TD></TR>", company.tin_no, company.cst_no));
                    sb.AppendLine("</TABLE>");

                    sb.AppendLine("<BR/><BR/>");

                    sb.AppendLine("<TABLE class = \"boldtable\">");
                    sb.AppendLine("<TR><TD WIDTH = 200 ALIGN = \"LEFT\"><b>RECEIVED BY</b></TD><TD WIDTH = 200 ALIGN = \"CENTER\"><b>VERIFIED BY</b></TD><TD WIDTH = 200 ALIGN = \"RIGHT\"><b>APPROVED BY</b></TD></TR>");
                    sb.AppendLine("</TABLE>");

                    sb.AppendLine("</body>");
                    sb.AppendLine("</html>");
                }
                return sb.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        public void SaveNewNarration(string companyCode, string branchCode, string narration)
        {
            NarrationVM newNarration = new NarrationVM();
            ErrorVM narrationError = new ErrorVM();
            newNarration.CompanyCode = companyCode;
            newNarration.BranchCode = branchCode;
            newNarration.Narration = narration;
            new NarrationBL().SaveNarrationDetails(newNarration, out narrationError);
        }
        #endregion
    }
}