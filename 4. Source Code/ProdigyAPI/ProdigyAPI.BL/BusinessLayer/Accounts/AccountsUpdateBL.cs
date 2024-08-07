using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Accounts
{
    public class AccountsUpdateBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();

        #endregion
        public bool FinalUpdate(string companyCode, string branchCode, DateTime startDate, DateTime endDate, out ErrorVM error)
        {
            error = FinalAccountPostingWithProedure(companyCode, branchCode, startDate, endDate);
            return error == null ? true : false;
        }

        private ErrorVM FinalAccountPostingWithProedure(string companyCode, string branchCode, DateTime startDate, DateTime endDate)
        {
            ErrorVM error = new ErrorVM();
            try {
                var retOj = db.usp_acc_SubsidiaryToSummaryAccountsPosting(startDate, endDate, companyCode, branchCode);
                error = null;
                return error;
            }
            catch (Exception excp) {
                return new ErrorVM() { description = excp.Message, field = "Account Update", index = 0 };
            }
        }
    }
}
