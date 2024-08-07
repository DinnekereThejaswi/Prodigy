using System;
using System.Collections.Generic;
using System.Linq;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Model.MagnaDb;
using System.Data;
using ProdigyAPI.BL.ViewModel.BatchPosting;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;

namespace ProdigyAPI.BL.BusinessLayer.BatchPosting
{
    public class DayEndProcessingBL
    {
        MagnaDbEntities db = null;

        public DayEndProcessingBL()
        {
            db = new MagnaDbEntities(true);
        }

        public DayEndProcessingBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }

        public bool PostDayEnd(string companyCode, string branchCode, string userID, DateTime newTxnDate, out ErrorVM error)
        {
            error = null;
            var rateMaster = db.KSTU_RATE_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode).FirstOrDefault();
            if(rateMaster == null) {
                error = new ErrorVM { description = "There is no rate set." };
                return false;
            }
            var applicationDate = rateMaster.ondate.Date;
            if (newTxnDate.Date < applicationDate) {
                error = new ErrorVM { description = "It is not possible to set lower date than " + rateMaster.ondate.ToString("dd-MMM-yyyy") };
                return false;
            }
            if(string.IsNullOrEmpty(rateMaster.isDayEnd) || rateMaster.isDayEnd.ToUpper() == "N") {
                ;
            }
            else {
                error = new ErrorVM { description = "Day end is already performed for this date." };
                return false;
            }

            if (!CheckForPendingSR(companyCode, branchCode, applicationDate, out error))
                return false;
            if (!CheckForPendingPurchases(companyCode, branchCode, applicationDate, out error))
                return false;
            using (var transaction = db.Database.BeginTransaction()) {
                try {
                    if (!ExecuteDayEndSqlProcedure(companyCode, branchCode, applicationDate, out error)) {
                        transaction.Rollback();
                        return false;
                    }
                    if (!ExecuteGSStockUpdationSqlProcedure(companyCode, branchCode, applicationDate, out error)) {
                        transaction.Rollback();
                        return false;
                    }
                    transaction.Commit();
                    SIGlobals.Globals.WriteTransactionLog(userID, "Day end done for date " + applicationDate.ToString("dd-MM-yyyy"), null, db);
                }
                catch (Exception ex) {
                    if(transaction != null)
                        transaction.Rollback();
                    error = new ErrorVM().GetErrorDetails(ex);
                    return false;
                }
            }

            return true;
        }

        private bool CheckForPendingSR(string companyCode, string branchCode, DateTime applicationDate, out ErrorVM error)
        {
            error = null;
            try {
                string billInfo = string.Empty;
                string sql = string.Format("exec usp_validateSR_PURCHASE '{0} 00:00:00.000','{0} 23:59:59.998','{1}','{2}','{3}'",
                    applicationDate.ToString("MM/dd/yyyy"), "SR", companyCode, branchCode);
                DataTable pendingTransactions = SIGlobals.Globals.GetDataTable(sql);
                if (pendingTransactions != null && pendingTransactions.Rows.Count > 0) {
                    billInfo = string.Empty;
                    foreach (DataRow srRow in pendingTransactions.Rows)
                        billInfo = billInfo + "," + srRow[0].ToString();
                    billInfo = billInfo.TrimStart(',');
                    error = new ErrorVM
                    {
                        description = string.Format("Sales Return bill(s) - ({0}) are not Adjusted. These bills either have to be cancelled "
                        + "or adjusted before Day-end", billInfo)
                    };
                    return false;
                }
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        private bool CheckForPendingPurchases(string companyCode, string branchCode, DateTime applicationDate, out ErrorVM error)
        {
            error = null;
            try {
                string billInfo = string.Empty;
                string sql = string.Format("exec usp_validateSR_PURCHASE '{0} 00:00:00.000','{0} 23:59:59.998','{1}','{2}','{3}'",
                    applicationDate.ToString("MM/dd/yyyy"), "PR", companyCode, branchCode);
                DataTable pendingTransactions = SIGlobals.Globals.GetDataTable(sql);
                if (pendingTransactions != null && pendingTransactions.Rows.Count > 0) {
                    billInfo = string.Empty;
                    foreach (DataRow srRow in pendingTransactions.Rows)
                        billInfo = billInfo + "," + srRow[0].ToString();
                    billInfo = billInfo.TrimStart(',');
                    error = new ErrorVM
                    {
                        description = string.Format("Purchase(s) - ({0}) are not Adjusted. These bills either have to be cancelled "
                        + "or adjusted before Day-end", billInfo)
                    };
                    return false;
                }
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        private bool ExecuteDayEndSqlProcedure(string companyCode, string branchCode, DateTime applicationDate, out ErrorVM error)
        {
            try {
                error = new ErrorVM();
                SqlParameter pCompanyCode = new SqlParameter("@companycode", companyCode);
                SqlParameter pBranchCode = new SqlParameter("@branchcode", branchCode);
                SqlParameter pDate = new SqlParameter("@date", applicationDate);
                List<SqlParameter> paramList = new List<SqlParameter>();
                paramList.Add(pCompanyCode);
                paramList.Add(pBranchCode);
                paramList.Add(pDate);
                var result = SIGlobals.Globals.ExecuteSQL("EXECUTE [dbo].[dayendprocess] @companycode, @branchcode, @date", db, paramList.ToArray());
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        private bool ExecuteGSStockUpdationSqlProcedure(string companyCode, string branchCode, DateTime applicationDate, out ErrorVM error)
        {
            try {
                error = new ErrorVM();
                string procSql = "EXECUTE [dbo].[usp_GSStockUpdate]  \n"
                               + "   @company_code \n"
                               + "  ,@branch_code \n"
                               + "  ,@startdate \n"
                               + "  ,@enddate";
                SqlParameter pCompanyCode = new SqlParameter("@company_code", companyCode);
                SqlParameter pBranchCode = new SqlParameter("@branch_code", branchCode);
                SqlParameter pStartDate = new SqlParameter("@startdate", applicationDate);
                SqlParameter pEndDate = new SqlParameter("@enddate", SIGlobals.Globals.GetEndingTimeOfDay(applicationDate));
                List<SqlParameter> paramList = new List<SqlParameter>();
                paramList.Add(pCompanyCode);
                paramList.Add(pBranchCode);
                paramList.Add(pStartDate);
                paramList.Add(pEndDate);
                var result = SIGlobals.Globals.ExecuteSQL(procSql, db, paramList.ToArray());
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }
    }
}
