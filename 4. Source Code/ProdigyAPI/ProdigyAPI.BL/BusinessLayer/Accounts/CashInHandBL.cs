using ProdigyAPI.BL.ViewModel.Accounts;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Accounts
{
    public class CashInHandBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        private const string MODULE_SEQ_NO = "52";
        #endregion

        #region Methods
        public dynamic GetCashInHand(string companyCode, string branchCode, DateTime date)
        {
            DateTime fromDate = Convert.ToDateTime(date.ToShortDateString() + " 00:00:00.000");
            DateTime toDate = Convert.ToDateTime(date.ToShortDateString() + " 23:59:59.998");

            return db.CashInHand(fromDate, toDate, companyCode, branchCode).Select(s => new
            {
                Ctype = s.CTYPE,
                C2 = s.C2
            });
        }

        public bool Save(CashInHandVM cash, out ErrorVM error)
        {
            error = null;
            if (cash == null) {
                error = new ErrorVM()
                {
                    description = "Invalid Data",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }

            int finYear = SIGlobals.Globals.GetFinancialYear(db, cash.CompanyCode, cash.BranchCode);
            try {
                KTTU_CASH_IN_HAND_DETAILS cashDel = db.KTTU_CASH_IN_HAND_DETAILS.Where(ca => ca.company_code == cash.CompanyCode
                                                                                        && ca.branch_code == cash.BranchCode
                                                                                        && System.Data.Entity.DbFunctions.TruncateTime(ca.bill_date) == System.Data.Entity.DbFunctions.TruncateTime(cash.BillDate)).FirstOrDefault();
                if (cashDel != null) {
                    db.KTTU_CASH_IN_HAND_DETAILS.Remove(cashDel);
                }
                KTTU_CASH_IN_HAND_DETAILS c = new KTTU_CASH_IN_HAND_DETAILS();
                c.obj_id = cash.ObjID;
                c.branch_code = cash.BranchCode;
                c.company_code = cash.CompanyCode;
                c.sl_no = cash.SlNo;
                c.Cash_Balance = cash.CashBalance;
                c.Cash_In_Hand = cash.CashInHand;
                c.bill_date = cash.BillDate;
                c.Fin_Year = finYear;
                db.KTTU_CASH_IN_HAND_DETAILS.Add(c);
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
