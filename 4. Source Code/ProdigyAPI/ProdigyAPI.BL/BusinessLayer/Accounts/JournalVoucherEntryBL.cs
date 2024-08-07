using ProdigyAPI.BL.ViewModel.Accounts;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Controllers.Accounts;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Accounts
{
    public class JournalVoucherEntryBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        public const string MODULE_TRAN_SEQ_NO = "03";
        #endregion

        #region Methods
        public dynamic GetAccountName(string companyCode, string branchCode)
        {
            return db.KSTU_ACC_LEDGER_MASTER.Where(ac => ac.company_code == companyCode
                                                    && ac.branch_code == branchCode
                                                    && (ac.acc_type == "O" || ac.acc_type == "R")).OrderBy(ac => ac.acc_name);
        }

        public AccVoucherTransactionsVM SaveJournalVoucherEntry(List<AccVoucherTransactionsVM> VoucherDet, out ErrorVM error)
        {
            if (VoucherDet == null || VoucherDet.Count == 0) {
                error = new ErrorVM()
                {
                    description = "Invalid Voucher Details",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return null;
            }
            error = null;

            string companyCode = VoucherDet[0].CompanyCode;
            string branchCode = VoucherDet[0].BranchCode;
            int finYear = SIGlobals.Globals.GetFinancialYear(db, companyCode, branchCode);
            DateTime appDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);

            decimal debitAmount = VoucherDet.Sum(d => d.DebitAmount);
            decimal creditAmount = VoucherDet.Sum(d => d.CreditAmount);

            if (creditAmount != debitAmount) {
                error = new ErrorVM()
                {
                    description = "Cannot Save Entry Until Difference is Zero",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return null;
            }
            AccVoucherTransactionsVM data = new AccVoucherTransactionsVM();
            try {
                int voucherNo = GetJournalVoucherNo(db, finYear, companyCode, branchCode, out error);
                foreach (AccVoucherTransactionsVM v in VoucherDet) {
                    int tranSeqNo = Convert.ToInt32(finYear.ToString().Remove(0, 1) + db.KSTS_ACC_SEQ_NOS.Where(sq => sq.obj_id == MODULE_TRAN_SEQ_NO && sq.company_code == companyCode && sq.branch_code == branchCode).FirstOrDefault().nextno);
                    var partyName = v.Partyname;
                    if (string.IsNullOrEmpty(v.Partyname)) {
                        var accLedgerMaster = db.KSTU_ACC_LEDGER_MASTER.Where(x => x.company_code == v.CompanyCode && x.branch_code == v.BranchCode
                            && x.acc_code == v.AccountCode).FirstOrDefault();
                        if (accLedgerMaster != null)
                            partyName = accLedgerMaster.acc_name;
                    }
                    KSTU_ACC_VOUCHER_TRANSACTIONS kavt = new KSTU_ACC_VOUCHER_TRANSACTIONS();
                    kavt.obj_id = SIGlobals.Globals.GetNewGUID();
                    kavt.company_code = v.CompanyCode;
                    kavt.branch_code = v.BranchCode;
                    kavt.txt_seq_no = tranSeqNo;
                    kavt.voucher_no = voucherNo;
                    kavt.voucher_seq_no = 1;
                    kavt.acc_code = v.AccountCode;
                    kavt.acc_code_master = v.AccountCodeMaster;
                    kavt.acc_type = "J";
                    kavt.voucher_date = appDate;
                    kavt.approved_by = v.ApprovedBy;
                    kavt.approved_date = appDate;
                    kavt.Authorized_By = v.AuthorizedBy;
                    kavt.Authorized_Date = appDate;
                    kavt.Authorized_Remarks = v.AuthorizedRemarks;
                    kavt.Cancelled_by = v.CancelledBy;
                    kavt.Cancelled_remarks = v.CancelledRemarks;
                    kavt.cflag = "N";
                    kavt.chq_date = appDate;
                    kavt.chq_no = v.ChequeNo;
                    kavt.contra_seq_no = 0;
                    kavt.cr_amount = v.CreditAmount;
                    kavt.currency_type = v.CurrencyType;
                    kavt.currency_value = v.CurrencyValue;
                    kavt.dr_amount = v.DebitAmount;
                    kavt.exchange_rate = v.ExchangeRate;
                    kavt.fin_period = v.FinalPeriod;
                    kavt.fin_year = finYear;
                    kavt.inserted_on = SIGlobals.Globals.GetDateTime();
                    kavt.Is_Authorized = "N";
                    kavt.is_approved = "N";
                    kavt.is_tds = "N";
                    kavt.Is_Verified = "N";
                    kavt.narration = v.Narration;
                    kavt.New_voucher_no = v.NewVoucherNo;
                    kavt.party_name = partyName;
                    kavt.receipt_no = v.ReceiptNo;
                    kavt.reconsile_by = v.ReconsileBy;
                    kavt.reconsile_date = SIGlobals.Globals.GetDateTime();
                    kavt.reconsile_flag = "N";
                    kavt.section_id = v.SectionID;
                    kavt.subledger_acc_code = 0;
                    kavt.TDS_amount = Convert.ToDecimal(0.00);
                    kavt.trans_type = "JOU";
                    kavt.UpdateOn = SIGlobals.Globals.GetDateTime();
                    kavt.UniqRowID = Guid.NewGuid();
                    kavt.Verified_By = v.VerifiedBy;
                    kavt.Verified_Date = SIGlobals.Globals.GetDateTime();
                    kavt.Verified_Remarks = v.VerifiedRemarks;
                    kavt.voucher_type = v.VoucherType;
                    db.KSTU_ACC_VOUCHER_TRANSACTIONS.Add(kavt);
                    SIGlobals.Globals.UpdateAccountSeqenceNumber(db, MODULE_TRAN_SEQ_NO, companyCode, branchCode);
                    data = new AccVoucherTransactionsVM()
                    {
                        VoucherNo = kavt.voucher_no,
                        AccountCodeMaster = kavt.acc_code_master,
                        AccountCode = kavt.acc_code,
                        TransType = kavt.trans_type,
                        AccountType = kavt.acc_type
                    };
                    //saving Narration if its New
                    new AccountsCommonBL().SaveNewNarration(kavt.company_code, kavt.branch_code, kavt.narration);
                }
                db.SaveChanges();
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public List<AccVoucherTransactionsVM> GetJournalVoucherDetails(string companyCode, string branchCode, int voucherNo, out ErrorVM error)
        {
            error = null;
            if (voucherNo == 0) {
                error = new ErrorVM()
                {
                    description = "Invalid Voucher No.",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return null;
            }
            List<AccVoucherTransactionsVM> data = new List<AccVoucherTransactionsVM>();
            data = (from av in db.KSTU_ACC_VOUCHER_TRANSACTIONS
                    join ad in db.KSTU_ACC_LEDGER_MASTER
                    on new { Company = av.company_code, Branch = av.branch_code, AccCode = av.acc_code }
                    equals new { Company = ad.company_code, Branch = ad.branch_code, AccCode = ad.acc_code }
                    where av.acc_type == "J" && av.trans_type == "JOU"
                                             && av.voucher_no == voucherNo
                                             && av.company_code == companyCode
                                             && av.branch_code == branchCode
                    select new AccVoucherTransactionsVM()
                    {
                        CompanyCode = av.company_code,
                        BranchCode = av.branch_code,
                        TxtSeqNo = av.txt_seq_no,
                        VoucherSeqNo = av.voucher_seq_no,
                        VoucherNo = av.voucher_no,
                        VoucherDate = av.voucher_date,
                        AccountCode = av.acc_code,
                        AccountType = av.acc_type,
                        DebitAmount = av.dr_amount,
                        CreditAmount = av.cr_amount,
                        ChequeNo = av.chq_no,
                        ChequeDate = av.chq_date,
                        FinalYear = av.fin_year,
                        FinalPeriod = av.fin_period,
                        AccountCodeMaster = av.acc_code_master,
                        Narration = av.narration,
                        ReceiptNo = av.receipt_no,
                        TransType = av.trans_type,
                        ApprovedBy = av.approved_by,
                        CancelledRemarks = av.Cancelled_remarks,
                        CancelledBy = av.Cancelled_by,
                        Partyname = ad.acc_name,
                        VoucherType = av.voucher_type,
                        ReconsileBy = av.reconsile_by,
                        CurrencyType = av.currency_type,
                        NewVoucherNo = av.New_voucher_no,
                        SectionID = av.section_id,
                        VerifiedBy = av.Verified_By,
                        VerifiedRemarks = av.Verified_Remarks,
                        AuthorizedBy = av.Authorized_By,
                        AuthorizedRemarks = av.Authorized_Remarks,
                        ExchangeRate = av.exchange_rate,
                        CurrencyValue = av.currency_value,
                        ContraSeqNo = av.contra_seq_no,
                        ReconsileFlag = av.reconsile_flag,
                        Cflag = av.cflag,
                        IsApproved = av.is_approved,
                        SubledgerAccCode = av.subledger_acc_code
                    }).ToList();
            if (data == null) {
                error = new ErrorVM()
                {
                    description = "Voucher details not found.",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return null;
            }
            else if (data[0].Cflag == "Y") {
                error = new ErrorVM()
                {
                    description = "Voucher already Cancelled.",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return null;
            }
            return data;
        }

        public List<AccVoucherTransactionsVM> GetJournalVoucherDetails(string companyCode, string branchCode, DateTime date, out ErrorVM error)
        {
            error = null;
            List<AccVoucherTransactionsVM> data = new List<AccVoucherTransactionsVM>();
            data = (from av in db.KSTU_ACC_VOUCHER_TRANSACTIONS
                    join ad in db.KSTU_ACC_LEDGER_MASTER
                    on new { Company = av.company_code, Branch = av.branch_code, AccCode = av.acc_code }
                    equals new { Company = ad.company_code, Branch = ad.branch_code, AccCode = ad.acc_code }
                    where av.acc_type == "J"
                                && av.trans_type == "JOU"
                                && System.Data.Entity.DbFunctions.TruncateTime(av.voucher_date) == System.Data.Entity.DbFunctions.TruncateTime(date)
                                && av.company_code == companyCode
                                && av.branch_code == branchCode
                    select new AccVoucherTransactionsVM()
                    {
                        CompanyCode = av.company_code,
                        BranchCode = av.branch_code,
                        TxtSeqNo = av.txt_seq_no,
                        VoucherSeqNo = av.voucher_seq_no,
                        VoucherNo = av.voucher_no,
                        VoucherDate = av.voucher_date,
                        AccountCode = av.acc_code,
                        AccountType = av.acc_type,
                        DebitAmount = av.dr_amount,
                        CreditAmount = av.cr_amount,
                        ChequeNo = av.chq_no,
                        ChequeDate = av.chq_date,
                        FinalYear = av.fin_year,
                        FinalPeriod = av.fin_period,
                        AccountCodeMaster = av.acc_code_master,
                        Narration = av.narration,
                        ReceiptNo = av.receipt_no,
                        TransType = av.trans_type,
                        ApprovedBy = av.approved_by,
                        CancelledRemarks = av.Cancelled_remarks,
                        CancelledBy = av.Cancelled_by,
                        Partyname = ad.acc_name,
                        VoucherType = av.voucher_type,
                        ReconsileBy = av.reconsile_by,
                        CurrencyType = av.currency_type,
                        NewVoucherNo = av.New_voucher_no,
                        SectionID = av.section_id,
                        VerifiedBy = av.Verified_By,
                        VerifiedRemarks = av.Verified_Remarks,
                        AuthorizedBy = av.Authorized_By,
                        AuthorizedRemarks = av.Authorized_Remarks,
                        ExchangeRate = av.exchange_rate,
                        CurrencyValue = av.currency_value,
                        ContraSeqNo = av.contra_seq_no,
                        ReconsileFlag = av.reconsile_flag,
                        Cflag = av.cflag,
                        IsApproved = av.is_approved,
                        SubledgerAccCode = av.subledger_acc_code
                    }).ToList();
            if (data == null) {
                error = new ErrorVM() { description = "Voucher details not found.", ErrorStatusCode = System.Net.HttpStatusCode.NotFound };
                return null;
            }
            else if (data[0].Cflag == "Y") {
                error = new ErrorVM() { description = "Voucher already Cancelled.", ErrorStatusCode = System.Net.HttpStatusCode.NotFound };
                return null;
            }
            return data;
        }

        public string Print(string companyCode, string branchCode, int voucherNo, string accType, out ErrorVM error)
        {
            error = null;
            if (voucherNo == 0) {
                error = new ErrorVM()
                {
                    description = "Invalid Voucher No.",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return "";
            }
            KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode && c.branch_code == branchCode).FirstOrDefault();
            var data = (from av in db.KSTU_ACC_VOUCHER_TRANSACTIONS
                        join ad in db.KSTU_ACC_LEDGER_MASTER
                        on new { Company = av.company_code, Branch = av.branch_code, AccCode = av.acc_code }
                        equals new { Company = ad.company_code, Branch = ad.branch_code, AccCode = ad.acc_code }
                        where av.acc_type == "J" && av.contra_seq_no == 0
                                                    && av.acc_code_master == 0
                                                    && av.voucher_no == voucherNo
                                                    && av.trans_type == "JOU"
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

            StringBuilder sb = new StringBuilder();
            try {
                sb.AppendLine("<html>");
                sb.AppendLine("<head>");
                sb.AppendLine(SIGlobals.Globals.GetStyleForBill());
                sb.AppendLine("</head>");
                sb.AppendLine("<body>");

                sb.AppendLine("<TABLE WIDTH = 500 >");
                sb.AppendLine(string.Format("<TR class = \"boldText\"><TH><H5><b ALIGN = \"RIGHT\" >{0}</b></H5></TH></TR>", SIGlobals.Globals.GetcompanyDetailsForPrint(db, companyCode, branchCode)));
                sb.AppendLine("</TABLE>");

                sb.AppendLine("<TABLE class = \"boldtable\">");
                sb.AppendLine(string.Format("<TR><TD WIDTH = 400 ALIGN = \"LEFT\">{0}</TD>", ""));
                sb.AppendLine(string.Format("<TD WIDTH = 200 ALIGN = \"RIGHT\">No : {0}</TD></TR>", voucherNo));
                sb.AppendLine(string.Format("<TR><TD WIDTH = 400 ALIGN = \"LEFT\">{0}</TD>", ""));
                sb.AppendLine(string.Format("<TD WIDTH = 200 ALIGN = \"RIGHT\">Date: {0}</TD></TR>", data[0].av.voucher_date.ToString().Trim()));
                sb.AppendLine("</TABLE>");

                sb.AppendLine("<TABLE class = \"boldtable\">");
                if (accType.Equals("C"))
                    sb.AppendLine("<TR><TD NOWRAP WIDTH = 610 ALIGN = \"CENTER\" class = \"billstyle\">CASH VOUCHER</TD></TR>");
                else
                    sb.AppendLine("<TR><TD NOWRAP WIDTH = 610 ALIGN = \"CENTER\" class = \"billstyle\">JOURNAL VOUCHER</TD></TR>");
                sb.AppendLine("</TABLE>");
                sb.AppendLine("<TABLE border =\"0\" class = \"boldtable\">");

                sb.AppendLine("<TR><Td NOWRAP WIDTH = 200 ALIGN = \"LEFT\"><B>DESCRIPTION</B></Td>");
                sb.AppendLine("<Td NOWRAP WIDTH = 150 ALIGN = \"LEFT\"><B>Narration<B></Td>");
                sb.AppendLine("<Td NOWRAP WIDTH = 120 ALIGN = \"RIGHT\"><B>Dr</B></Td>");
                sb.AppendLine("<Td NOWRAP WIDTH = 120 ALIGN = \"RIGHT\"><B>Cr</B></Td></TR>");
                decimal TotalDr = 0, TotalCr = 0, Amount = 0;
                sb.AppendLine("</TABLE>");

                string name = string.Empty, narration = string.Empty;
                string accountNames = string.Empty, narr = string.Empty;
                for (int i = 0; i < data.Count; i++) {

                    TotalDr += Convert.ToDecimal(data[i].av.dr_amount);
                    TotalCr += Convert.ToDecimal(data[i].av.cr_amount);

                    List<string> lstAccountName = new List<string>();
                    List<string> lstNarration = new List<string>();
                    accountNames = data[i].ad.acc_name.ToString();
                    narration = data[i].av.narration == null ? "" : data[i].av.narration.ToString();

                    sb.AppendLine("<TABLE class = \"boldtable\">");
                    if (accountNames.Length <= 20)
                        sb.AppendLine(string.Format("<TR><TD WIDTH = 200 ALIGN = \"LEFT\" >{0}</TD>", accountNames));
                    else {
                        lstAccountName = SIGlobals.Globals.SplitStringAt(20, accountNames);
                        name = string.Empty;
                        for (int k = 0; k < lstAccountName.Count; k++) {
                            name += lstAccountName[k] + " ";
                        }
                        sb.AppendLine(string.Format("<TR> <TD WIDTH = 200 ALIGN = \"LEFT\" >{0}</TD>", name));
                    }

                    if (narration.Length <= 20)
                        sb.AppendLine(string.Format("<TD WIDTH = 150 ALIGN = \"LEFT\" >{0}</TD>", narration));
                    else {
                        narr = string.Empty;
                        lstNarration = SIGlobals.Globals.SplitStringAt(20, narration);

                        for (int k = 0; k < lstNarration.Count; k++) {
                            narr += lstNarration[k] + " ";
                        }
                        sb.AppendLine(string.Format("<TD WIDTH = 150 ALIGN = \"LEFT\" >{0}</TD>", narr));
                    }
                    if (Convert.ToDecimal(data[i].av.dr_amount) == 0)
                        sb.AppendLine(string.Format("<TD WIDTH = 100 ALIGN = \"RIGHT\" >{1}</TD> <TD WIDTH = 100 ALIGN = \"RIGHT\" >{0}</TD></TR>", data[i].av.cr_amount.ToString(), ""));
                    else
                        sb.AppendLine(string.Format("<TD WIDTH = 100 ALIGN = \"RIGHT\" >{0}</TD></TR>", data[i].av.dr_amount.ToString()));

                }

                sb.AppendLine("<TR><TD ALIGN = \"RIGHT\" colspan=\"4\">-----------------------</TD></TR>");
                sb.AppendLine(string.Format("<TR><TD WIDTH = 200 ALIGN = \"LEFT\" >{0}</TD> <TD WIDTH = 200 ALIGN = \"RIGHT\" >TOTAL</TD>", ""));
                sb.AppendLine(string.Format("<TD WIDTH = 100 ALIGN = \"RIGHT\" >{0}</TD> <TD WIDTH = 100 ALIGN = \"RIGHT\" >{1}</TD></TR>", TotalDr.ToString("N"), TotalCr.ToString("N")));
                sb.AppendLine("<TR><TD ALIGN = \"RIGHT\" colspan=\"4\">-----------------------</TD></TR>");


                Amount = TotalDr + TotalCr;  //Its added assuming either any one present
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

                return sb.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }

        }

        #endregion

        #region public Method
        public int GetJournalVoucherNo(MagnaDbEntities db, int finYear, string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                int voucherNo = 0;
                DateTime startDate = Convert.ToDateTime(finYear + "/04/01");
                DateTime endDate = Convert.ToDateTime(SIGlobals.Globals.GetApplicationDate(companyCode, branchCode).ToShortDateString());
                string days = Math.Abs(Convert.ToInt32((endDate - startDate).TotalDays) + 1).ToString();
                string month = Convert.ToString(SIGlobals.Globals.GetApplicationDate(companyCode, branchCode).Month);
                string finCode = finYear.ToString().Substring(2, 2);
                KSTS_ACC_JOURNAL_SEQ kajs = db.KSTS_ACC_JOURNAL_SEQ.Where(ac => ac.obj_id == days && ac.company_code == companyCode && ac.branch_code == branchCode).FirstOrDefault();
                voucherNo = Convert.ToInt32(days + month + finCode + kajs.nextno);
                kajs.nextno = kajs.nextno + 1;
                db.Entry(kajs).State = System.Data.Entity.EntityState.Modified;
                return voucherNo;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                error.customDescription = "Error while creating Journal Voucher No";
                return 0;
            }
        }
        #endregion
    }
}
