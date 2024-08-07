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
    public class CashVoucherEntryBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        public const string MODULE_TRAN_SEQ_NO = "03";
        public const string MODULE_ACC_VOUCHER_SEQ_NO = "1";
        #endregion

        #region Methods
        public int GetLastVoucherNo(string companyCode, string branchCode)
        {
            return db.KSTU_ACC_VOUCHER_TRANSACTIONS.Where(ac => ac.company_code == companyCode && ac.branch_code == branchCode && (ac.trans_type == "PAY" || ac.trans_type == "CAS") && ac.acc_type == "C").Max(a => a.voucher_no);
        }
        public dynamic GetListOfEntry(string companyCode, string branchCode, string tranType, int accCodeMaster, DateTime voutcherDate)
        {
            var data = (from av in db.KSTU_ACC_VOUCHER_TRANSACTIONS
                        join ad in db.KSTU_ACC_LEDGER_MASTER
                        on new { Company = av.company_code, Branch = av.branch_code, AccCode = av.acc_code }
                        equals new { Company = ad.company_code, Branch = ad.branch_code, AccCode = ad.acc_code }
                        where av.acc_type == "C" && av.contra_seq_no == 0
                                                 && av.acc_code_master == accCodeMaster
                                                 && av.trans_type == tranType
                                                 && System.Data.Entity.DbFunctions.TruncateTime(av.voucher_date) == System.Data.Entity.DbFunctions.TruncateTime(voutcherDate)
                                                 && av.cflag == "N"
                                                 && av.company_code == companyCode
                                                 && av.branch_code == branchCode
                        select new
                        {
                            ObjID = av.obj_id,
                            VoucherNo = av.voucher_no,
                            Date = av.voucher_date,
                            AccName = ad.acc_name,
                            Debit = av.dr_amount,
                            Credit = av.cr_amount,
                            Narration = av.narration,
                            RefNo = av.receipt_no,
                            AccCode = av.acc_code,
                            AccCodeMaster = av.acc_code_master,
                            ChequNo = av.chq_no,
                            ChequDate = av.chq_date,
                            AccType = av.acc_type
                        }).ToList();
            return data.OrderBy(s => s.VoucherNo);
        }
        public AccVoucherTransactionsVM SaveCashVourcherDetails(AccVoucherTransactionsVM v, out ErrorVM error)
        {
            error = null;
            if (v == null) {
                error = new ErrorVM()
                {
                    description = "Invalid data.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return null;
            }

            int finYear = SIGlobals.Globals.GetFinancialYear(db, v.CompanyCode, v.BranchCode);
            DateTime appDate = SIGlobals.Globals.GetApplicationDate(v.CompanyCode, v.BranchCode);
            int tranSeqNo = SIGlobals.Globals.GetAccountSeqNo(db, MODULE_TRAN_SEQ_NO, v.CompanyCode, v.BranchCode);
            int voucherNo = SIGlobals.Globals.GetAccountVoucherSeqNo(db, MODULE_ACC_VOUCHER_SEQ_NO, v.CompanyCode, v.BranchCode);

            KSTU_TOLERANCE_MASTER tolMaster = db.KSTU_TOLERANCE_MASTER.Where(tol => tol.obj_id == 6).FirstOrDefault();

            if (v.DebitAmount > 0 && v.DebitAmount >= tolMaster.Max_Val) {
                error = new ErrorVM()
                {
                    description = "Cash Payment is not allowed " + tolMaster.Max_Val + " and above",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                };
                return null;
            }

            KSTU_ACC_VOUCHER_TRANSACTIONS kavt = new KSTU_ACC_VOUCHER_TRANSACTIONS();
            string objID = "KSTU_ACC_VOUCHER_TRANSACTIONS" +
                           SIGlobals.Globals.Separator + Convert.ToString(voucherNo) +
                           SIGlobals.Globals.Separator + "C" +
                           SIGlobals.Globals.Separator + Convert.ToString(v.AccountCodeMaster) +
                           SIGlobals.Globals.Separator + v.TransType +
                           SIGlobals.Globals.Separator + v.CompanyCode +
                           SIGlobals.Globals.Separator + v.BranchCode;
            var partyName = v.Partyname;
            if (string.IsNullOrEmpty(v.Partyname)) {
                var accLedgerMaster = db.KSTU_ACC_LEDGER_MASTER.Where(x => x.company_code == v.CompanyCode && x.branch_code == v.BranchCode
                    && x.acc_code == v.AccountCode).FirstOrDefault();
                if (accLedgerMaster != null)
                    partyName = accLedgerMaster.acc_name;
            }

            kavt.obj_id = SIGlobals.Globals.GetHashcode(objID);
            kavt.company_code = v.CompanyCode;
            kavt.branch_code = v.BranchCode;
            kavt.txt_seq_no = tranSeqNo;
            kavt.voucher_no = voucherNo;
            kavt.voucher_seq_no = 1;
            kavt.acc_code = v.AccountCode;
            kavt.acc_code_master = v.AccountCodeMaster;
            kavt.acc_type = "C";
            kavt.approved_by = v.ApprovedBy;
            kavt.voucher_date = appDate;
            kavt.approved_date = appDate;
            kavt.Authorized_By = "";
            kavt.Authorized_Date = appDate;
            kavt.Authorized_Remarks = "";
            kavt.Cancelled_by = v.CancelledBy;
            kavt.Cancelled_remarks = v.CancelledRemarks;
            kavt.cflag = "N";
            kavt.chq_date = v.ChequeDate;
            kavt.chq_no = v.ChequeNo == null ? "" : v.ChequeNo;
            kavt.contra_seq_no = 0;
            kavt.cr_amount = v.CreditAmount;
            kavt.currency_type = "INR";
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
            kavt.New_voucher_no = voucherNo;
            kavt.party_name = partyName;
            kavt.receipt_no = "";
            kavt.reconsile_by = "";
            kavt.reconsile_date = SIGlobals.Globals.GetDateTime();
            kavt.reconsile_flag = "N";
            kavt.section_id = v.SectionID;
            kavt.subledger_acc_code = 0;
            kavt.TDS_amount = Convert.ToDecimal(0.00);
            kavt.trans_type = v.TransType;
            kavt.UpdateOn = SIGlobals.Globals.GetDateTime();
            kavt.UniqRowID = Guid.NewGuid();
            kavt.Verified_By = "";
            kavt.Verified_Date = SIGlobals.Globals.GetDateTime();
            kavt.Verified_Remarks = "";
            kavt.voucher_date = appDate;
            kavt.voucher_type = "";
            db.KSTU_ACC_VOUCHER_TRANSACTIONS.Add(kavt);
            //saving Narration if its New
            new AccountsCommonBL().SaveNewNarration(v.CompanyCode, v.BranchCode, v.Narration);
            try {
                SIGlobals.Globals.UpdateAccountSeqenceNumber(db, MODULE_TRAN_SEQ_NO, v.CompanyCode, v.BranchCode);
                SIGlobals.Globals.UpdateAccountVourcherSeqenceNumber(db, MODULE_ACC_VOUCHER_SEQ_NO, v.CompanyCode, v.BranchCode);
                db.SaveChanges();
                return new AccVoucherTransactionsVM()
                {
                    VoucherNo = kavt.voucher_no,
                    AccountCodeMaster = kavt.acc_code_master,
                    AccountCode = kavt.acc_code,
                    TransType = kavt.trans_type,
                    AccountType = kavt.acc_type
                };
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }
        public AccVoucherTransactionsVM GetCashVoucherDetails(string companyCode, string branchCode, int voucherNo, int accCode, int accCodeMaster, out ErrorVM error)
        {
            error = null;
            if (voucherNo == 0) {
                error = new ErrorVM()
                {
                    description = "Invalid Voucher No",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return null;
            }
            AccVoucherTransactionsVM data = (from av in db.KSTU_ACC_VOUCHER_TRANSACTIONS
                                             join ad in db.KSTU_ACC_LEDGER_MASTER
                                             on new { Company = av.company_code, Branch = av.branch_code, AccCode = av.acc_code }
                                             equals new { Company = ad.company_code, Branch = ad.branch_code, AccCode = ad.acc_code }
                                             where av.acc_type == "C" && av.contra_seq_no == 0
                                                                      && av.voucher_no == voucherNo
                                                                      && av.acc_code == accCode
                                                                      && av.acc_code_master == accCodeMaster
                                                                      && av.company_code == companyCode
                                                                      && av.branch_code == branchCode
                                             select new AccVoucherTransactionsVM()
                                             {
                                                 CompanyCode = av.company_code,
                                                 BranchCode = av.branch_code,
                                                 TxtSeqNo = av.txt_seq_no,
                                                 VoucherSeqNo = av.New_voucher_no,
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

            if (data == null) {
                error = new ErrorVM()
                {
                    description = "Voucher details not found.",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return null;
            }
            else if (data.Cflag == "Y") {
                error = new ErrorVM() { description = "Voucher details already cancelled.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return null;
            }
            return data;


        }
        public AccVoucherTransactionsVM GetCashVoucherDetails(string companyCode, string branchCode, int voucherNo, int accCodeMaster, string transType, out ErrorVM error)
        {
            error = null;
            AccVoucherTransactionsVM data = (from av in db.KSTU_ACC_VOUCHER_TRANSACTIONS
                                             join ad in db.KSTU_ACC_LEDGER_MASTER
                                             on new { Company = av.company_code, Branch = av.branch_code, AccCode = av.acc_code }
                                             equals new { Company = ad.company_code, Branch = ad.branch_code, AccCode = ad.acc_code }
                                             where av.acc_type == "C" && av.contra_seq_no == 0
                                                                      && av.voucher_no == voucherNo
                                                                      && av.acc_code_master == accCodeMaster
                                                                      && av.trans_type == transType
                                                                      && av.company_code == companyCode
                                                                      && av.branch_code == branchCode
                                             select new AccVoucherTransactionsVM()
                                             {
                                                 CompanyCode = av.company_code,
                                                 BranchCode = av.branch_code,
                                                 TxtSeqNo = av.txt_seq_no,
                                                 VoucherSeqNo = av.New_voucher_no,
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
                                             }).FirstOrDefault();

            if (data == null) {
                error = new ErrorVM()
                {
                    description = "Voucher details not found.",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return null;
            }
            else if (data.Cflag == "Y") {
                error = new ErrorVM()
                {
                    description = "Voucher details already cancelled.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return null;
            }
            return data;
        }
        #endregion
    }
}
