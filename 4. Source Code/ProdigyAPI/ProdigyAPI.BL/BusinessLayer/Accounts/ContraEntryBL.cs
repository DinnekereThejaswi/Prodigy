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
    public class ContraEntryBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        public const string MODULE_TRAN_SEQ_NO = "03";
        public const string MODULE_ACC_VOUCHER_SEQ_NO = "06";
        #endregion

        #region Methods
        /// <summary>
        /// Retruns List of Type of Transactions.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        public dynamic GetType(string companyCode, string branchCode)
        {
            return new List<dynamic>() {
                new { Code = "C", Name = "CASH DEPOSIT" },
                new { Code = "C", Name = "CASH WITHDRAWAL" },
                new { Code = "B", Name = "FUNDS TRANSFERRED" }
            };
        }

        /// <summary>
        /// Get Master Ledger Details based on Type
        /// "C" For Cash withdrawl and Cash Deposit
        /// "B" For Fund Transfer
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public dynamic GetMasterLedger(string companyCode, string branchCode, string type)
        {
            // Type "C" For Cash Withrdrawl and Cash Deposit
            // Type "B" For Fund Transfer

            if (type == "C") {
                var data = db.KSTU_ACC_LEDGER_MASTER.Where(acc => (acc.acc_type == "C" || acc.acc_type == type)
                                                          && acc.obj_status != "C"
                                                          && acc.company_code == companyCode
                                                          && acc.branch_code == branchCode)
                      .OrderBy(acc => acc.acc_name)
                      .Select(acc => new
                      {
                          Code = acc.acc_code,
                          Name = acc.acc_name
                      });
                return data;
            }
            else if (type == "B") {
                var data = db.KSTU_ACC_LEDGER_MASTER.Where(acc => (acc.acc_type == type)
                                                          && acc.obj_status != "C"
                                                          && acc.company_code == companyCode
                                                          && acc.branch_code == branchCode)
                       .OrderBy(acc => acc.acc_name)
                       .Select(acc => new
                       {
                           Code = acc.acc_code,
                           Name = acc.acc_name
                       });
                return data;
            }
            return null;
        }

        /// <summary>
        /// Get List of Account Names for Contra Entry
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public dynamic GetListOfAccountName(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                var data = db.KSTU_ACC_LEDGER_MASTER.Where(ac => ac.acc_type == "B"
                                                       && ac.company_code == companyCode
                                                       && ac.branch_code == branchCode)
                                       .OrderBy(ac => ac.acc_name)
                                       .Select(ac => new
                                       {
                                           Code = ac.acc_code,
                                           Name = ac.acc_name
                                       });
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public dynamic GetNarrationDetails(string companyCode, string branchCode)
        {
            return db.KSTU_ACC_NARRATION_MASTER.Where(acc => acc.company_code == companyCode && acc.branch_code == branchCode)
                .OrderBy(acc => acc.narr_id)
                .Select(acc => new
                {
                    Code = acc.narr_id,
                    Name = acc.narration
                });
        }

        public IQueryable GetListOfEntry(string companyCode, string branchCode, int accCodeMaster, DateTime voutcherDate)
        {
            return from av in db.KSTU_ACC_VOUCHER_TRANSACTIONS
                   join ad in db.KSTU_ACC_LEDGER_MASTER
                   on new { Company = av.company_code, Branch = av.branch_code, AccCode = av.acc_code }
                   equals new { Company = ad.company_code, Branch = ad.branch_code, AccCode = ad.acc_code }
                   where av.acc_type == "N" && av.contra_seq_no == 0
                                          && av.acc_code_master == accCodeMaster
                                          && System.Data.Entity.DbFunctions.TruncateTime(av.voucher_date) == System.Data.Entity.DbFunctions.TruncateTime(voutcherDate)
                                          && av.cflag == "N"
                                          && av.company_code == companyCode
                                          && av.branch_code == branchCode
                   select new
                   {
                       ObjID = av.obj_id,
                       VoucherNo = av.voucher_no,
                       AccCode = av.acc_code,
                       AccCodeMaster = av.acc_code_master,
                       Date = av.voucher_date,
                       AccName = ad.acc_name,
                       Debit = av.dr_amount,
                       Credit = av.cr_amount,
                       Narration = av.narration,
                       RefNo = av.receipt_no,
                       AccType = av.acc_type
                   };
        }

        public IQueryable GetListOfEntry(string companyCode, string branchCode, DateTime voutcherDate)
        {
            return from av in db.KSTU_ACC_VOUCHER_TRANSACTIONS
                   join ad in db.KSTU_ACC_LEDGER_MASTER
                   on new { Company = av.company_code, Branch = av.branch_code, AccCode = av.acc_code }
                   equals new { Company = ad.company_code, Branch = ad.branch_code, AccCode = ad.acc_code }
                   where av.acc_type == "N" && av.contra_seq_no == 0
                                          && av.trans_type == "CONT"
                                          && System.Data.Entity.DbFunctions.TruncateTime(av.voucher_date) == System.Data.Entity.DbFunctions.TruncateTime(voutcherDate)
                                          && av.cflag == "N"
                                          && av.company_code == companyCode
                                          && av.branch_code == branchCode
                   select new
                   {
                       ObjID = av.obj_id,
                       VoucherNo = av.voucher_no,
                       AccCode = av.acc_code,
                       AccCodeMaster = av.acc_code_master,
                       Date = av.voucher_date,
                       AccName = ad.acc_name,
                       Debit = av.dr_amount,
                       Credit = av.cr_amount,
                       Narration = av.narration,
                       RefNo = av.receipt_no,
                       AccType = av.acc_type
                   };
        }

        public AccVoucherTransactionsVM SaveVourcherDetails(AccVoucherTransactionsVM voucherDet, out ErrorVM error)
        {
            error = null;
            if (voucherDet == null) {
                error = new ErrorVM()
                {
                    description = "Invalid Voucher Details",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return null;
            }
            int finYear = SIGlobals.Globals.GetFinancialYear(db, voucherDet.CompanyCode, voucherDet.BranchCode);
            DateTime appDate = SIGlobals.Globals.GetApplicationDate(voucherDet.CompanyCode, voucherDet.BranchCode);
            int tranSeqNo = Convert.ToInt32(finYear.ToString().Remove(0, 1) +
                            db.KSTS_ACC_SEQ_NOS.Where(sq => sq.obj_id == MODULE_TRAN_SEQ_NO
                                                        && sq.company_code == voucherDet.CompanyCode
                                                        && sq.branch_code == voucherDet.BranchCode).FirstOrDefault().nextno);

            int voucherNo = Convert.ToInt32(finYear.ToString().Remove(0, 1) +
                            db.KSTS_ACC_SEQ_NOS.Where(sq => sq.obj_id == MODULE_ACC_VOUCHER_SEQ_NO
                                                        && sq.company_code == voucherDet.CompanyCode
                                                        && sq.branch_code == voucherDet.BranchCode).FirstOrDefault().nextno);

            KSTU_ACC_VOUCHER_TRANSACTIONS voucher = new KSTU_ACC_VOUCHER_TRANSACTIONS();
            string temp = "KSTU_ACC_VOUCHER_TRANSACTIONS" + SIGlobals.Globals.Separator
                    + voucherNo + SIGlobals.Globals.Separator
                    + voucherDet.AccountType + SIGlobals.Globals.Separator
                    + voucherDet.AccountCodeMaster + SIGlobals.Globals.Separator
                    + voucherDet.TransType + SIGlobals.Globals.Separator
                    + voucherDet.CompanyCode + SIGlobals.Globals.Separator
                    + voucherDet.BranchCode;

            var partyName = voucherDet.Partyname;
            if (string.IsNullOrEmpty(voucherDet.Partyname)) {
                var accLedgerMaster = db.KSTU_ACC_LEDGER_MASTER.Where(x => x.company_code == voucherDet.CompanyCode && x.branch_code == voucherDet.BranchCode
                    && x.acc_code == voucherDet.AccountCode).FirstOrDefault();
                if (accLedgerMaster != null)
                    partyName = accLedgerMaster.acc_name;
            }

            voucher.obj_id = SIGlobals.Globals.GetHashcode(temp);
            voucher.branch_code = voucherDet.BranchCode;
            voucher.company_code = voucherDet.CompanyCode;
            voucher.txt_seq_no = tranSeqNo;
            voucher.voucher_no = voucherNo;
            voucher.voucher_seq_no = 1;
            voucher.acc_type = "N";
            voucher.voucher_date = appDate;
            voucher.acc_code = voucherDet.AccountCode;
            voucher.dr_amount = voucherDet.DebitAmount;
            voucher.cr_amount = voucherDet.CreditAmount;
            voucher.chq_no = voucherDet.ChequeNo;
            voucher.chq_date = voucherDet.ChequeDate;
            voucher.fin_year = finYear;
            voucher.fin_period = voucherDet.FinalPeriod;
            voucher.acc_code_master = voucherDet.AccountCodeMaster;
            voucher.narration = voucherDet.Narration;
            voucher.contra_seq_no = 0;
            voucher.reconsile_flag = "N";
            voucher.reconsile_date = appDate;
            voucher.receipt_no = voucherDet.ReceiptNo;
            voucher.trans_type = "CONT";
            voucher.inserted_on = SIGlobals.Globals.GetDateTime();
            voucher.cflag = "N";
            voucher.UpdateOn = SIGlobals.Globals.GetDateTime();
            voucher.approved_by = "";
            voucher.approved_date = appDate;
            voucher.is_approved = "N";
            voucher.subledger_acc_code = 0;
            voucher.party_name = partyName;
            voucher.Cancelled_by = null;
            voucher.Cancelled_remarks = null;
            voucher.voucher_type = "";
            voucher.reconsile_by = "";
            voucher.currency_type = "INR";
            voucher.New_voucher_no = voucherNo;
            voucher.section_id = null;
            voucher.is_tds = "N";
            voucher.TDS_amount = 0;
            voucher.Is_Verified = "N";
            voucher.Verified_By = "";
            voucher.Verified_Date = appDate;
            voucher.Verified_Remarks = "";
            voucher.Is_Authorized = "N";
            voucher.Authorized_By = "";
            voucher.Authorized_Date = appDate;
            voucher.Authorized_Remarks = "";
            voucher.exchange_rate = 0;
            voucher.currency_value = 0;
            voucher.UniqRowID = Guid.NewGuid();

            //saving Narration if its New
            new AccountsCommonBL().SaveNewNarration(voucher.company_code, voucher.branch_code, voucher.narration);
            try {
                db.KSTU_ACC_VOUCHER_TRANSACTIONS.Add(voucher);
                SIGlobals.Globals.UpdateAccountSeqenceNumber(db, MODULE_TRAN_SEQ_NO, voucherDet.CompanyCode, voucherDet.BranchCode);
                SIGlobals.Globals.UpdateAccountSeqenceNumber(db, MODULE_ACC_VOUCHER_SEQ_NO, voucherDet.CompanyCode, voucherDet.BranchCode);
                db.SaveChanges();
                return new AccVoucherTransactionsVM()
                {
                    VoucherNo = voucher.voucher_no,
                    AccountCodeMaster = voucher.acc_code_master,
                    AccountCode = voucher.acc_code,
                    TransType = voucher.trans_type,
                    AccountType = voucher.acc_type
                };
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public AccVoucherTransactionsVM GetVoucherDetails(string companyCode, string branchCode, int voucherNo, int accCode, int accCodeMaster, out ErrorVM error)
        {
            // Incase if they did't pass Account Code and Account Code Master 
            error = null;
            AccVoucherTransactionsVM data = new AccVoucherTransactionsVM();
            if (accCode == 0 && accCodeMaster == 0) {
                data = (from av in db.KSTU_ACC_VOUCHER_TRANSACTIONS
                        join ad in db.KSTU_ACC_LEDGER_MASTER
                        on new { Company = av.company_code, Branch = av.branch_code, AccCode = av.acc_code }
                        equals new { Company = ad.company_code, Branch = ad.branch_code, AccCode = ad.acc_code }
                        where av.voucher_no == voucherNo
                                         && av.cflag == "N"
                                         && av.acc_type == "N"
                                         && av.trans_type == "CONT"
                                         && av.company_code == companyCode
                                         && av.branch_code == branchCode
                        select new AccVoucherTransactionsVM()
                        {
                            ObjID = av.obj_id,
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
                            Partyname = av.party_name,
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
                        }).FirstOrDefault();
            }
            else {
                data = (from av in db.KSTU_ACC_VOUCHER_TRANSACTIONS
                        join ad in db.KSTU_ACC_LEDGER_MASTER
                        on new { Company = av.company_code, Branch = av.branch_code, AccCode = av.acc_code }
                        equals new { Company = ad.company_code, Branch = ad.branch_code, AccCode = ad.acc_code }
                        where av.voucher_no == voucherNo
                                         && av.cflag == "N"
                                         && av.acc_type == "N"
                                         && av.acc_code_master == accCodeMaster
                                         && av.acc_code == accCode
                                         && av.trans_type == "CONT"
                                         && av.company_code == companyCode
                                          && av.branch_code == branchCode
                        select new AccVoucherTransactionsVM()
                        {
                            ObjID = av.obj_id,
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
                            Partyname = av.party_name,
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
                        }).FirstOrDefault();
            }
            if (data == null) {
                error = new ErrorVM()
                {
                    description = "Voucher Details Not Found",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return null;
            }
            return data;
        }

        public bool UpdateVourcherDetails(int voucherNo, AccVoucherTransactionsVM voucherDet, out ErrorVM error)
        {
            error = null;
            if (voucherNo == 0) {
                error = new ErrorVM()
                {
                    description = "Invalid Voucher Number",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return false;
            }
            if (voucherDet == null) {
                error = new ErrorVM()
                {
                    description = "Invalid Voucher Details",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return false;
            }

            KSTU_ACC_VOUCHER_TRANSACTIONS voucher = db.KSTU_ACC_VOUCHER_TRANSACTIONS.Where(acc => acc.company_code == voucherDet.CompanyCode
                                                                                            && acc.branch_code == voucherDet.BranchCode
                                                                                            && acc.voucher_no == voucherDet.VoucherNo
                                                                                            && acc.trans_type == voucherDet.TransType
                                                                                            && acc.acc_code == voucherDet.AccountCode
                                                                                            && acc.acc_code_master == voucherDet.AccountCodeMaster).FirstOrDefault();
            if (voucher == null) {
                error = new ErrorVM()
                {
                    description = "Invalid Voucher No",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return false;
            }
            else if (voucher.cflag == "Y") {
                error = new ErrorVM()
                {
                    description = "Voucher already Cancelled",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return false;
            }
            voucher.acc_code = voucherDet.AccountCode;
            voucher.dr_amount = voucherDet.DebitAmount;
            voucher.cr_amount = voucherDet.CreditAmount;
            voucher.chq_no = voucherDet.ChequeNo;
            voucher.chq_date = voucherDet.ChequeDate;
            voucher.narration = voucherDet.Narration;
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
        #endregion
    }
}
