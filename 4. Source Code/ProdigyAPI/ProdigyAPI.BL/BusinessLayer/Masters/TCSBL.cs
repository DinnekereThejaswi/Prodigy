using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Masters
{
    #region Interface
    public interface ITCSBL
    {
        dynamic GetAccountName(string companyCode, string branchCode, out ErrorVM error);
        dynamic GetKYC(string companyCode, string branchCode, out ErrorVM error);
        dynamic GetTransType(string companyCode, string branchCode, out ErrorVM error);
        dynamic GetCalculatedOn(string companyCode, string branchCode, out ErrorVM error);
        IEnumerable<TCSVM> GetTCS(string companyCode, string branchCode, out ErrorVM error);
        bool Save(TCSVM tcs, out ErrorVM error);
        bool Update(TCSVM tcs, out ErrorVM error);
        bool Delete(string companyCode, string branchCode, string isWithKyc, string transType, out ErrorVM error);
    }
    #endregion

    #region Class

    public sealed class TCSBL : ITCSBL
    {

        #region Declaration
        private MagnaDbEntities dbContext;

        public static readonly Lazy<TCSBL> tcsBL = new Lazy<TCSBL>(() => new TCSBL());
        public static TCSBL GetInstance { get { return tcsBL.Value; } }
        #endregion

        #region Constructor
        private TCSBL()
        {
            dbContext = new MagnaDbEntities();
        }
        #endregion

        #region Public Methods
        public dynamic GetAccountName(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                var data = dbContext.KSTU_ACC_LEDGER_MASTER.Where(tc => tc.company_code == companyCode
                                                           && tc.branch_code == branchCode)
                                                           .Select(t => new
                                                           {
                                                               Code = t.acc_code,
                                                               Name = t.acc_name
                                                           }).OrderBy(o => o.Name).ToList();
                return data;
            }
            catch (Exception ex) { error = new ErrorVM().GetErrorDetails(ex); return null; }
        }

        public dynamic GetCalculatedOn(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                var data = new[] { new { Code = "G", Name = "GrossAmt" }, new { Code = "N", Name = "Taxable Amt" } };
                return data;
            }
            catch (Exception ex) { error = new ErrorVM().GetErrorDetails(ex); return null; }
        }

        public dynamic GetKYC(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                var data = new[] { new { Code = "Y", Name = "Yes" }, new { Code = "N", Name = "No" } };
                return data;
            }
            catch (Exception ex) { error = new ErrorVM().GetErrorDetails(ex); return null; }
        }

        public dynamic GetTransType(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                var data = new[] { new { Code = "S", Name = "Sales" }, new { Code = "P", Name = "Purcahse" } };
                return data;
            }
            catch (Exception ex) { error = new ErrorVM().GetErrorDetails(ex); return null; }
        }

        public IEnumerable<TCSVM> GetTCS(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                var data = dbContext.KSTU_TCS_MASTER.Where(tcs => tcs.company_code == companyCode
                                                            && tcs.branch_code == branchCode)
                                                            .Select(tcs => new TCSVM
                                                            {
                                                                CompanyCode = tcs.company_code,
                                                                BranchCode = tcs.branch_code,
                                                                IsWithKYC = tcs.is_withKYC,
                                                                AmountLimit = tcs.amount_limt,
                                                                TCSPercent = tcs.tcs_percent,
                                                                AccCode = tcs.acc_code,
                                                                AccName = tcs.acc_name,
                                                                CalculatedOn = tcs.calculated_on,
                                                                EffectiveDate = tcs.effective_date,
                                                                TransactionType = tcs.transaction_type,
                                                                IsTDS = tcs.is_tds
                                                            }).ToList();
                return data;
            }
            catch (Exception ex) { error = new ErrorVM().GetErrorDetails(ex); return null; }
        }

        public bool Save(TCSVM tcs, out ErrorVM error)
        {
            error = null;
            if (tcs == null) {
                error = new ErrorVM()
                {
                    description = "Invalid data",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }

            KSTU_TCS_MASTER tc = dbContext.KSTU_TCS_MASTER.Where(t => t.company_code == tcs.CompanyCode 
                                                                && t.branch_code == tcs.BranchCode 
                                                                && t.tcs_percent == tcs.TCSPercent 
                                                                && t.acc_code == tcs.AccCode 
                                                                && t.transaction_type == tcs.TransactionType).FirstOrDefault();
            if (tcs != null) {
                error = new ErrorVM()
                {
                    description = "Details already exist!",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            try {

                string temp = "KSTU_TCS_MASTER" + SIGlobals.Globals.Separator + tcs.IsWithKYC + SIGlobals.Globals.Separator
                                                + tcs.TransactionType + SIGlobals.Globals.Separator + tcs.CompanyCode
                                                + SIGlobals.Globals.Separator + tcs.BranchCode;
                tc = new KSTU_TCS_MASTER()
                {
                    obj_id = SIGlobals.Globals.ComputeHash(temp),
                    company_code = tcs.CompanyCode,
                    branch_code = tcs.BranchCode,
                    is_withKYC = tcs.IsWithKYC,
                    amount_limt = tcs.AmountLimit,
                    tcs_percent = tcs.TCSPercent,
                    acc_code = tcs.AccCode,
                    acc_name = tcs.AccName,
                    calculated_on = tcs.CalculatedOn,
                    effective_date = tcs.EffectiveDate,
                    transaction_type = tcs.TransactionType,
                    is_tds = tcs.IsTDS,
                    UpdateOn = SIGlobals.Globals.GetDateTime()
                };
                dbContext.KSTU_TCS_MASTER.Add(tc);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex) { error = new ErrorVM().GetErrorDetails(ex); return false; }
        }

        public bool Update(TCSVM tcs, out ErrorVM error)
        {
            error = null;
            if (tcs == null) {
                error = new ErrorVM()
                {
                    description = "Invalid data",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            try {
                KSTU_TCS_MASTER tc = dbContext.KSTU_TCS_MASTER.Where(t => t.company_code == tcs.CompanyCode
                                                                     && t.branch_code == tcs.BranchCode
                                                                     && t.is_withKYC == tcs.IsWithKYC
                                                                     && t.transaction_type == tcs.TransactionType).FirstOrDefault();
                if (tc == null) {
                    error = new ErrorVM { description = "The entry you are about the update is not found." };
                    return false;
                }
                //tc.is_withKYC = tcs.IsWithKYC;
                //tc.transaction_type = tcs.TransactionType;
                tc.amount_limt = tcs.AmountLimit;
                tc.tcs_percent = tcs.TCSPercent;
                tc.acc_code = tcs.AccCode;
                tc.acc_name = tcs.AccName;
                tc.calculated_on = tcs.CalculatedOn;
                tc.effective_date = tcs.EffectiveDate;
                tc.is_tds = tcs.IsTDS;
                tc.UpdateOn = SIGlobals.Globals.GetDateTime();
                dbContext.KSTU_TCS_MASTER.Add(tc);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
        }

        public bool Delete(string companyCode, string branchCode, string isWithKyc, string transType, out ErrorVM error)
        {
            error = null;
            try {
                KSTU_TCS_MASTER tc = dbContext.KSTU_TCS_MASTER.Where(t => t.company_code == companyCode
                                                                    && t.branch_code == branchCode
                                                                    && t.is_withKYC == isWithKyc
                                                                    && t.transaction_type == transType).FirstOrDefault();
                if (tc == null) {
                    error = new ErrorVM { description = "The entry you are about the update is not found." };
                    return false;
                }
                dbContext.KSTU_TCS_MASTER.Remove(tc);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex) { error = new ErrorVM().GetErrorDetails(ex); return false; }
        }
        #endregion

        #region Private Methods
        #endregion
    }
    #endregion
}
