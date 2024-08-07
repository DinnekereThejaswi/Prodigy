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
/// Date: 20th April 2021
/// Module Type: Business Layer
/// </summary>
namespace ProdigyAPI.BL.BusinessLayer.Accounts
{
    /// <summary>
    /// This Provides Methods related to Account Code Settings Module.
    /// </summary>
    public class AccountCodeSettingBL
    {
        #region Declaration
        MagnaDbEntities dbContext = null;
        #endregion

        #region Constructor
        public AccountCodeSettingBL()
        {
            dbContext = new MagnaDbEntities();
        }

        public AccountCodeSettingBL(MagnaDbEntities db)
        {
            dbContext = db;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method Provides Get All Account Ledgers.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public dynamic GetAccountLedger(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                var ledMaster = dbContext.KSTU_ACC_LEDGER_MASTER.Where(ld => ld.company_code == companyCode
                                                                   && ld.branch_code == branchCode)
                                                                   .Select(ld => new { Code = ld.acc_code, Name = ld.acc_name })
                                                                   .OrderBy(ld => ld.Name)
                                                                   .ToList();
                return ledMaster;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        /// <summary>
        /// Method Provides Get All Account Code Master Details.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public List<AccountCodeMasterVM> GetAccountCodeMaster(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                return dbContext.KSTS_ACC_CODE_MASTER.Where(ac => ac.company_code == companyCode
                                                                 && ac.branch_code == branchCode
                                                                 && ac.acc_code > 0)
                                                                  .Select(ac => new AccountCodeMasterVM()
                                                                  {
                                                                      ObjID = ac.obj_id,
                                                                      CompanyCode = ac.company_code,
                                                                      BranchCode = ac.branch_code,
                                                                      AccCode = ac.acc_code,
                                                                      AccName = ac.acc_name,
                                                                      SubGroupID = ac.sub_group_id,
                                                                      SubGroupName = ac.sub_group_name,
                                                                      GroupID = ac.group_id,
                                                                      GroupName = ac.group_name,
                                                                      Description = ac.Description,
                                                                      ObjectStatus = ac.object_status
                                                                  }).OrderBy(ac => ac.AccCode).ToList();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        /// <summary>
        /// This Method provides to save Acccount Code Master Details.
        /// </summary>
        /// <param name="accCodeMaster"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool SaveAccountCodeMaster(AccountCodeMasterVM accCodeMaster, out ErrorVM error)
        {
            #region Declaration
            error = null;
            string companyCode = string.Empty;
            string branchCode = string.Empty;
            KSTS_ACC_CODE_MASTER checkAccCodeMaster = new KSTS_ACC_CODE_MASTER();
            #endregion

            #region Validation
            if (accCodeMaster == null) {
                error = new ErrorVM()
                {
                    description = "Invalid Details",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            companyCode = accCodeMaster.CompanyCode;
            branchCode = accCodeMaster.BranchCode;

            // Checking Object ID Exist or Not
            checkAccCodeMaster = dbContext.KSTS_ACC_CODE_MASTER.Where(ac => ac.company_code == companyCode
                                                   && ac.branch_code == branchCode
                                                   && ac.obj_id == accCodeMaster.ObjID).FirstOrDefault();
            if (checkAccCodeMaster != null) {
                error = new ErrorVM()
                {
                    description = "Object ID already Exists.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }

            // Checking Account Name already Exist or Not.
            checkAccCodeMaster = dbContext.KSTS_ACC_CODE_MASTER.Where(ac => ac.company_code == companyCode
                                                     && ac.branch_code == branchCode
                                                     && ac.acc_code == accCodeMaster.AccCode).FirstOrDefault();

            if (checkAccCodeMaster != null) {
                error = new ErrorVM()
                {
                    description = "Account Name already Exists.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }

            // Checking Account Code Exist or Not in KSTU_ACC_LEDGER_MASTER
            KSTU_ACC_LEDGER_MASTER ledMaster = dbContext.KSTU_ACC_LEDGER_MASTER.Where(led => led.company_code == companyCode
                                                                                        && led.branch_code == branchCode
                                                                                        && led.acc_code == accCodeMaster.AccCode).FirstOrDefault();
            if (ledMaster == null) {
                error = new ErrorVM()
                {
                    description = "Invalid Account Code.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }

            if (ledMaster.acc_name.Trim() != accCodeMaster.AccName) {
                error = new ErrorVM()
                {
                    description = string.Format("For the selected account code {0} Account name is {1}", accCodeMaster.AccCode, ledMaster.acc_name),
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }

            #endregion

            #region Save
            KSTS_ACC_CODE_MASTER accCode = new KSTS_ACC_CODE_MASTER();
            accCode.obj_id = accCodeMaster.ObjID;
            accCode.company_code = accCodeMaster.CompanyCode;
            accCode.branch_code = accCode.branch_code;
            accCode.acc_code = accCode.acc_code;
            accCode.acc_name = ledMaster.acc_name.Trim();
            accCode.sub_group_id = accCode.sub_group_id;
            accCode.sub_group_name = accCode.sub_group_name;
            accCode.group_id = accCode.group_id;
            accCode.group_name = accCode.group_name;
            accCode.Description = accCode.Description;
            accCode.object_status = accCode.object_status;
            dbContext.KSTS_ACC_CODE_MASTER.Add(accCode);
            try {
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
            #endregion
        }

        /// <summary>
        /// This Method Provides Update the Account Code Master
        /// </summary>
        /// <param name="accCodeMaster"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool UpdateAccountCodeMaster(int objID, AccountCodeMasterVM accCodeMaster, out ErrorVM error)
        {
            #region Declaration and Assignment
            error = null;
            #endregion

            #region Validation
            if (objID == 0) {
                error = new ErrorVM()
                {
                    description = "Invalide Object ID."
                };
                return false;
            }

            if (accCodeMaster == null) {
                error = new ErrorVM()
                {
                    description = "Invalid Account Code Master Details."
                };
                return false;
            }
            KSTS_ACC_CODE_MASTER accCode = dbContext.KSTS_ACC_CODE_MASTER.Where(ac => ac.obj_id == objID
                                                                                && ac.company_code == accCodeMaster.CompanyCode
                                                                                && ac.branch_code == accCodeMaster.BranchCode).FirstOrDefault();
            if (accCode == null) {
                if (accCodeMaster == null) {
                    error = new ErrorVM()
                    {
                        description = "Account code details not found."
                    };
                    return false;
                }
            }
            #endregion

            #region Update
            accCode.obj_id = accCodeMaster.ObjID;
            accCode.company_code = accCodeMaster.CompanyCode;
            accCode.branch_code = accCodeMaster.BranchCode;
            accCode.acc_code = accCodeMaster.AccCode;
            accCode.acc_name = accCodeMaster.AccName;
            accCode.sub_group_id = accCodeMaster.SubGroupID;
            accCode.sub_group_name = accCodeMaster.SubGroupName;
            accCode.group_id = accCodeMaster.GroupID;
            accCode.group_name = accCodeMaster.GroupName;
            accCode.Description = accCodeMaster.Description;
            accCode.object_status = accCodeMaster.ObjectStatus;
            dbContext.Entry(accCode).State = System.Data.Entity.EntityState.Modified;
            try {
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
            #endregion
        }
        #endregion
    }
}

