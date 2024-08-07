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
    public class BankVoucherEntryBL
    {
        #region Declaration
        MagnaDbEntities db = new MagnaDbEntities();
        public const string MODULE_TRAN_SEQ_NO = "03";
        public const string MODULE_ACC_VOUCHER_SEQ_NO = "2";
        #endregion

        #region Methods
        public dynamic Type()
        {
            List<VoucherTypesVM> voucherTypes = new List<VoucherTypesVM>();
            voucherTypes.Add(new VoucherTypesVM()
            {
                Code = "PAY",
                Name = "Payment"
            });
            voucherTypes.Add(new VoucherTypesVM()
            {
                Code = "REC",
                Name = "Receipt"
            });

            return voucherTypes.ToList();
        }

        public int GetLastVoucherNo(string companyCode, string branchCode)
        {
            return db.KSTU_ACC_VOUCHER_TRANSACTIONS.Where(ac => ac.company_code == companyCode
                                                           && ac.branch_code == branchCode
                                                           && (ac.trans_type == "PAY" || ac.trans_type == "CAS") && ac.acc_type == "B").Max(a => a.voucher_no);
        }

        public dynamic GetListOfAccountName(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                var data = db.KSTU_ACC_LEDGER_MASTER.Where(ac => ac.company_code == companyCode
                                                       && ac.branch_code == branchCode
                                                       && (ac.acc_type == "O" || ac.acc_type == "R" || ac.acc_type == "C"))
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

        public dynamic List(string companyCode, string branchCode, int accCodeMaster, string transactionType, DateTime date)
        {
            var data = (from av in db.KSTU_ACC_VOUCHER_TRANSACTIONS
                        join ad in db.KSTU_ACC_LEDGER_MASTER
                        on new { Company = av.company_code, Branch = av.branch_code, AccCode = av.acc_code }
                        equals new { Company = ad.company_code, Branch = ad.branch_code, AccCode = ad.acc_code }
                        where av.acc_type == "B" && av.trans_type == transactionType
                                                 && av.contra_seq_no == 0
                                                 && av.acc_code_master == accCodeMaster
                                                 && System.Data.Entity.DbFunctions.TruncateTime(av.voucher_date) == System.Data.Entity.DbFunctions.TruncateTime(date)
                                                 && av.cflag == "N"
                                                 && av.company_code == companyCode
                                                 && av.branch_code == branchCode
                        select new
                        {
                            ObjID = av.obj_id,
                            VoucherNo = av.voucher_no,
                            AccCode = av.acc_code,
                            Date = av.voucher_date,
                            AccName = ad.acc_name,
                            Debit = av.dr_amount,
                            Credit = av.cr_amount,
                            Narration = av.narration,
                            RefNo = av.receipt_no,
                            AccCodeMaster = av.acc_code_master,
                            ChequeNo = av.chq_no,
                            ChequeDate = av.chq_date,
                            AccType = av.acc_type,
                            TranType = av.trans_type
                        }).ToList();
            return data.OrderBy(s => s.VoucherNo);
        }

        public IQueryable PrintList(string companyCode, string branchCode, string transactionType, DateTime date)
        {
            return from av in db.KSTU_ACC_VOUCHER_TRANSACTIONS
                   join ad in db.KSTU_ACC_LEDGER_MASTER
                   on new { Company = av.company_code, Branch = av.branch_code, AccCode = av.acc_code }
                   equals new { Company = ad.company_code, Branch = ad.branch_code, AccCode = ad.acc_code }
                   where av.acc_type == "B" && av.trans_type == transactionType
                                            && av.contra_seq_no == 0
                                            && System.Data.Entity.DbFunctions.TruncateTime(av.voucher_date) == System.Data.Entity.DbFunctions.TruncateTime(date)
                                            && av.cflag == "N"
                                            && av.company_code == companyCode
                                            && av.branch_code == branchCode
                   select new
                   {
                       ObjID = av.obj_id,
                       VoucherNo = av.voucher_no,
                       AccCode = av.acc_code,
                       Date = av.voucher_date,
                       AccName = ad.acc_name,
                       Debit = av.dr_amount,
                       Credit = av.cr_amount,
                       Narration = av.narration,
                       RefNo = av.receipt_no,
                       AccCodeMaster = av.acc_code_master,
                       ChequeNo = av.chq_no,
                       ChequeDate = av.chq_date,
                       AccType = av.acc_type
                   };
        }

        public AccVoucherTransactionsVM SaveBankVoucherEntry(List<AccVoucherTransactionsVM> voucherDet, out ErrorVM error)
        {
            error = null;
            if (voucherDet.Count == 0) {
                error = new ErrorVM()
                {
                    description = "Invalid data.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return null;
            }

            string companyCode = voucherDet[0].CompanyCode;
            string branchCode = voucherDet[0].BranchCode;

            int finYear = SIGlobals.Globals.GetFinancialYear(db, companyCode, branchCode);
            DateTime appDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
            int voucherNo = SIGlobals.Globals.GetAccountVoucherSeqNo(db, MODULE_ACC_VOUCHER_SEQ_NO, companyCode, branchCode);

            AccVoucherTransactionsVM retObj = new AccVoucherTransactionsVM();
            try {
                int voucherSeqNo = 1;
                foreach (AccVoucherTransactionsVM v in voucherDet) {
                    if (string.IsNullOrEmpty(v.ChequeNo)) {
                        error = new ErrorVM { description = "Cheque number cannot be null or empty for bank voucher transactions.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                        return null;
                    }
                    if (v.CreditAmount > 0 && v.DebitAmount > 0) {
                        error = new ErrorVM { description = "one of the credit/debit should be posted, but not both.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                        return null;
                    }
                    int tranSeqNo = Convert.ToInt32(finYear.ToString().Remove(0, 1)
                        + db.KSTS_ACC_SEQ_NOS.Where(sq => sq.obj_id == MODULE_TRAN_SEQ_NO
                        && sq.company_code == companyCode
                        && sq.branch_code == branchCode).FirstOrDefault().nextno);

                    KSTU_ACC_VOUCHER_TRANSACTIONS kavt = new KSTU_ACC_VOUCHER_TRANSACTIONS();

                    string objID = "KSTU_ACC_VOUCHER_TRANSACTIONS" +
                                SIGlobals.Globals.Separator + Convert.ToString(voucherNo) +
                                 SIGlobals.Globals.Separator + v.AccountType +
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
                    kavt.voucher_seq_no = voucherSeqNo;
                    kavt.acc_code = v.AccountCode;
                    kavt.acc_code_master = v.AccountCodeMaster;
                    kavt.acc_type = "B";
                    kavt.approved_by = v.ApprovedBy;
                    kavt.voucher_date = appDate;
                    kavt.approved_date = appDate;
                    kavt.Authorized_By = v.AuthorizedBy;
                    kavt.Authorized_Date = appDate;
                    kavt.Authorized_Remarks = v.AuthorizedRemarks;
                    kavt.Cancelled_by = v.CancelledBy;
                    kavt.Cancelled_remarks = v.CancelledRemarks;
                    kavt.cflag = "N";
                    kavt.chq_date = v.ChequeDate == null ? appDate : v.ChequeDate;
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
                    kavt.trans_type = v.TransType;
                    kavt.UpdateOn = SIGlobals.Globals.GetDateTime();
                    kavt.UniqRowID = Guid.NewGuid();
                    kavt.Verified_By = v.VerifiedBy;
                    kavt.Verified_Date = SIGlobals.Globals.GetDateTime();
                    kavt.Verified_Remarks = v.VerifiedRemarks;
                    kavt.voucher_type = v.VoucherType;
                    db.KSTU_ACC_VOUCHER_TRANSACTIONS.Add(kavt);
                    voucherSeqNo += 1;
                    retObj = new AccVoucherTransactionsVM()
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
                SIGlobals.Globals.UpdateAccountSeqenceNumber(db, MODULE_TRAN_SEQ_NO, companyCode, branchCode);
                SIGlobals.Globals.UpdateAccountVourcherSeqenceNumber(db, MODULE_ACC_VOUCHER_SEQ_NO, companyCode, branchCode);
                db.SaveChanges();
                return retObj;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public dynamic GetVoucherDetails(string companyCode, string branchCode, int voucherNo, int accCodeMaster, string tranType)
        {
            return (from av in db.KSTU_ACC_VOUCHER_TRANSACTIONS
                    join ad in db.KSTU_ACC_LEDGER_MASTER
                    on new { Company = av.company_code, Branch = av.branch_code, AccCode = av.acc_code }
                    equals new { Company = ad.company_code, Branch = ad.branch_code, AccCode = ad.acc_code }
                    where av.acc_type == "B" && av.trans_type == tranType
                                             && av.contra_seq_no == 0
                                             && av.acc_code_master == accCodeMaster
                                             && av.voucher_no == voucherNo
                                             && av.cflag == "N"
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
                    });
        }
        #endregion
    }
}


