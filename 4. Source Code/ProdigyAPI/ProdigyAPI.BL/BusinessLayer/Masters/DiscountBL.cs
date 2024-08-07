using ProdigyAPI.BL.BusinessLayer.Masters;
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
    public class DiscountBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Methods
        public dynamic GetDiscountPeriod(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            DiscountMasterVM discount = new DiscountMasterVM();
            KSTS_DISCOUNT_PERIOD dp = db.KSTS_DISCOUNT_PERIOD.FirstOrDefault();
            if (dp != null) {
                discount.ObjID = dp.obj_id;
                discount.CompanyCode = dp.company_code;
                discount.BranchCode = dp.branch_code;
                discount.SRDiscount = dp.sr_discount;
                discount.StWtDeduction = dp.st_wt_deduction;
                discount.PurDiscount = dp.pur_discount;
                discount.OrderDiscount = dp.order_discount;
                discount.DisDCounter = dp.dis_dcounter;
                discount.DisRCounter = dp.dis_rcounter;
                discount.StartDate = dp.start_date;
                discount.EndDate = dp.end_date;
                discount.DiscountType = dp.discount_type;
            }
            return discount;
        }

        public bool SaveDiscountPeriodDetails(DiscountMasterVM discount, out ErrorVM error)
        {
            error = null;
            try {
                KSTS_DISCOUNT_PERIOD dp = db.KSTS_DISCOUNT_PERIOD.FirstOrDefault();
                dp.company_code = discount.CompanyCode;
                dp.branch_code = discount.BranchCode;
                dp.sr_discount = discount.SRDiscount;
                dp.st_wt_deduction = discount.StWtDeduction;
                dp.pur_discount = discount.PurDiscount;
                dp.order_discount = discount.OrderDiscount;
                dp.dis_dcounter = discount.DisDCounter;
                dp.dis_rcounter = discount.DisRCounter;
                dp.start_date = discount.StartDate;
                dp.end_date = discount.EndDate;
                dp.discount_type = discount.DiscountType;
                db.Entry(dp).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM()
                {
                    description = excp.Message,
                    ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError
                };
                return false;
            }
        }
        #endregion
    }
}
