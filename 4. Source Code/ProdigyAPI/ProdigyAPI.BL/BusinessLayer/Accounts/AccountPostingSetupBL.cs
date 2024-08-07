using ProdigyAPI.BL.ViewModel.Accounts;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Author: Mustureswara M M (Eshwar)
/// Date: 2021-05-17
/// </summary>
namespace ProdigyAPI.BL.BusinessLayer.Accounts
{
    public class AccountPostingSetupBL
    {
        #region Declaration
        MagnaDbEntities dbContext = null;
        #endregion

        #region Constructors
        public AccountPostingSetupBL()
        {
            dbContext = new MagnaDbEntities();
        }

        public AccountPostingSetupBL(MagnaDbEntities db)
        {
            dbContext = db;
        }
        #endregion

        #region Public  Methods
        /// <summary>
        /// This method returns GSCode Name and GS Codes.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public dynamic GetGS(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                var gsCode = dbContext.KSTS_GS_ITEM_ENTRY.Where(gs => gs.company_code == companyCode
                                                   && gs.branch_code == branchCode).Select(g => new
                                                   {
                                                       gsCode = g.gs_code,
                                                       Name = g.item_level1_name
                                                   }).ToList().OrderBy(gs => gs.Name);
                return gsCode;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        /// <summary>
        /// This method returns Transaction Types and there Code.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public dynamic GetTransactionTypes(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                string[] types = { "S", "P", "RS", "I", "R", "SI", "SR", "SD" };
                var transType = dbContext.KTTS_TRANSACTION_TYPES_MASTER.Where(t => t.company_code == companyCode
                                                                                && t.branch_code == branchCode
                                                                                && types.Contains(t.trans_code))
                                                                       .Select(t => new
                                                                       {
                                                                           Code = t.trans_code,
                                                                           Name = t.trans_name
                                                                       }).ToList();
                return transType;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        /// <summary>
        /// This Method returns GSCodes,TransactionType and Account Code.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public dynamic AllInOne(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            List<AccountPostingSetupVM> lstOfAccountCode = new List<AccountPostingSetupVM>();
            try {
                var gsData = dbContext.KSTS_GS_ITEM_ENTRY.Where(gs => gs.company_code == companyCode
                                                  && gs.branch_code == branchCode).Select(g => new
                                                  {
                                                      gsCode = g.gs_code,
                                                      Name = g.item_level1_name
                                                  }).ToList().OrderBy(gs => gs.Name);

                foreach (var d in gsData) {
                    string[] types = { "S", "P", "RS", "I", "R", "SI", "SR", "SD" };
                    var transType = dbContext.KTTS_TRANSACTION_TYPES_MASTER.Where(t => t.company_code == companyCode
                                                                                    && t.branch_code == branchCode
                                                                                    && types.Contains(t.trans_code))
                                                                           .Select(t => new
                                                                           {
                                                                               Code = t.trans_code,
                                                                               Name = t.trans_name
                                                                           }).ToList();
                    foreach (var tp in transType) {
                        var setup = dbContext.KSTS_ACC_POSTING_SETUP.Where(p => p.company_code == companyCode
                                                                                        && p.branch_code == branchCode
                                                                                        && p.gs_code == d.gsCode
                                                                                        && p.trans_type == tp.Code).ToList();
                        foreach (var t in setup) {
                            AccountPostingSetupVM posting = new AccountPostingSetupVM()
                            {
                                CompanyCode = t.company_code,
                                BranchCode = t.branch_code,
                                GSCode = t.gs_code,
                                TransType = t.trans_type,
                                AccountCode = t.acc_code
                            };
                            lstOfAccountCode.Add(posting);
                        }
                    }
                }
                return lstOfAccountCode;
            }
            catch (Exception excp) {

                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        /// <summary>
        /// This Method returns Ledgers related to Transaction Type.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="tranType"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public dynamic GetLedger(string companyCode, string branchCode, string tranType, out ErrorVM error)
        {
            error = null;
            try {
                if (tranType == "PM") {
                    var ledger = dbContext.KSTU_ACC_LEDGER_MASTER.Where(led => led.company_code == companyCode
                                                                               && led.branch_code == branchCode
                                                                               && led.V_TYPE == "D")
                                                                 .Select(led => new
                                                                 {
                                                                     Code = led.acc_code,
                                                                     Name = led.acc_name,
                                                                     GSCode = led.gs_code
                                                                 })
                                                                 .ToList();
                    return ledger;
                }
                else {
                    var ledger = dbContext.KSTU_ACC_LEDGER_MASTER.Where(led => led.company_code == companyCode
                                                                                 && led.branch_code == branchCode
                                                                                 && led.transType == tranType)
                                                                   .Select(led => new
                                                                   {
                                                                       Code = led.acc_code,
                                                                       Name = led.acc_name,
                                                                       GSCode = led.gs_code
                                                                   })
                                                                   .ToList();
                    return ledger;
                }
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        /// <summary>
        /// This method returns All Account Posting Setup of specified TransactionType.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="transType"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public List<AccountPostingSetupVM> GetAccountPostingSetup(string companyCode, string branchCode, string transType, out ErrorVM error)
        {
            error = null;
            List<AccountPostingSetupVM> lstOfAccountCode = new List<AccountPostingSetupVM>();
            try {
                var gsData = dbContext.KSTS_GS_ITEM_ENTRY.Where(gs => gs.company_code == companyCode
                                                  && gs.branch_code == branchCode).Select(g => new
                                                  {
                                                      gsCode = g.gs_code,
                                                      Name = g.item_level1_name
                                                  }).ToList().OrderBy(gs => gs.Name);
                foreach (var d in gsData) {
                    KSTS_ACC_POSTING_SETUP setup = dbContext.KSTS_ACC_POSTING_SETUP.Where(p => p.company_code == companyCode
                                                                                        && p.branch_code == branchCode
                                                                                        && p.gs_code == d.gsCode
                                                                                        && p.trans_type == transType).FirstOrDefault();

                    AccountPostingSetupVM posting = new AccountPostingSetupVM()
                    {
                        CompanyCode = setup.company_code,
                        BranchCode = setup.branch_code,
                        GSCode = setup.gs_code,
                        TransType = setup.trans_type,
                        AccountCode = setup.acc_code
                    };
                    lstOfAccountCode.Add(posting);
                }
                return lstOfAccountCode;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        /// <summary>
        /// Save or Update Account Postingn Setup.
        /// </summary>
        /// <param name="setup"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool SaveAccountPostingSetup(AccountPostingSetupVM setup, out ErrorVM error)
        {
            error = null;
            KSTS_ACC_POSTING_SETUP accSetup = null;

            try {
                List<KSTU_ACC_VOUCHER_TRANSACTIONS> voucherTran = dbContext.KSTU_ACC_VOUCHER_TRANSACTIONS.Where(acc => acc.acc_code == setup.OldAccountCode
                                                                                                                && acc.company_code == setup.CompanyCode
                                                                                                                && acc.branch_code == setup.BranchCode).ToList();
                if (voucherTran.Count > 0) {
                    error = new ErrorVM()
                    {
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                        description = "This ledger has Transactions,cannot be Modified."
                    };
                    return false;
                }

                if (setup.TransType == "PM") {
                    //Payment Modes Posting.
                    accSetup = dbContext.KSTS_ACC_POSTING_SETUP.Where(st => st.company_code == setup.CompanyCode
                                                                   && st.branch_code == setup.BranchCode
                                                                   && st.gs_code == "PM"
                                                                   && st.acc_code == setup.OldAccountCode).FirstOrDefault();
                }
                else {
                    // Tansaction Posting.
                    accSetup = dbContext.KSTS_ACC_POSTING_SETUP.Where(st => st.company_code == setup.CompanyCode
                                                                   && st.branch_code == setup.BranchCode
                                                                   && st.gs_code == setup.GSCode
                                                                   && st.trans_type == setup.TransType).FirstOrDefault();
                }
                if (accSetup == null) {
                    accSetup = new KSTS_ACC_POSTING_SETUP();
                    accSetup.company_code = setup.CompanyCode;
                    accSetup.branch_code = setup.BranchCode;
                    accSetup.gs_code = setup.GSCode;
                    accSetup.trans_type = setup.TransType;
                    accSetup.acc_code = setup.AccountCode;
                    dbContext.KSTS_ACC_POSTING_SETUP.Add(accSetup);
                }
                else {
                    accSetup.acc_code = setup.AccountCode;
                    dbContext.Entry(accSetup).State = System.Data.Entity.EntityState.Modified;
                }
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }


        /// <summary>
        /// Get All PayModes
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public dynamic GetPayModes(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                var data = (from aps in dbContext.KSTS_ACC_POSTING_SETUP
                            join pm in dbContext.KTTS_PAYMENT_MASTER on
                            new
                            {
                                CompanyCode = aps.company_code,
                                BranchCode = aps.branch_code,
                                AccountCode = aps.trans_type
                            }
                            equals new
                            {
                                CompanyCode = pm.company_code,
                                BranchCode = pm.branch_code,
                                AccountCode = pm.payment_code
                            }
                            join lm in dbContext.KSTU_ACC_LEDGER_MASTER on
                            new
                            {
                                CompanyCode = aps.company_code,
                                BranchCode = aps.branch_code,
                                AccountCode = aps.acc_code
                            }
                            equals new
                            {
                                CompanyCode = lm.company_code,
                                BranchCode = lm.branch_code,
                                AccountCode = lm.acc_code
                            }
                            where aps.gs_code == "PM" && aps.company_code == companyCode && aps.branch_code == branchCode
                            select new
                            {
                                PayMode = pm.payment_name,
                                LedgerName = lm.acc_name,
                                AccountCode = aps.acc_code
                            }).ToList();
                return data;
            }
            catch (Exception excp) {

                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        #endregion
    }
}
