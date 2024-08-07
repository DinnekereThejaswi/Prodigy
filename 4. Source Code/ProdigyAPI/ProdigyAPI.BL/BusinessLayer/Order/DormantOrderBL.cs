using ProdigyAPI.BL.BusinessLayer.Order;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Order
{
    #region Interfaces
    interface IDormant
    {
        bool LockOrder(List<OrderMasterVM> orders, out ErrorVM error);
    }

    interface IUnlockDormant
    {
        bool UnlockOrder(List<OrderMasterVM> orders, out ErrorVM error);
        OrderMasterVM GetOrderToBeUnlocked(string companyCode, string branchCode, int orderNo, out ErrorVM error);
    }
    #endregion

    public sealed class DormantOrderBL : IDormant, IUnlockDormant
    {
        #region Declaration
        private readonly MagnaDbEntities dbContext = null;

        public static readonly Lazy<DormantOrderBL> dormant = new Lazy<DormantOrderBL>(() => new DormantOrderBL(new MagnaDbEntities()));
        public static DormantOrderBL GetInstance
        {
            get
            {
                return dormant.Value;
            }
        }
        #endregion

        #region Constructor
        private DormantOrderBL(MagnaDbEntities db)
        {
            dbContext = db;
        }
        #endregion

        #region Methods

        #region Create Dormant Order
        public List<OrderMasterVM> GetOrderList(string companyCode, string brachCode, DateTime date, out ErrorVM error)
        {
            error = null;
            DateTime validateDate = Convert.ToDateTime(SIGlobals.Globals.ExecuteQuery("select * from v_DormandOrderDateValidattion").Rows[0][0].ToString());
            if (date > validateDate) {
                error = new ErrorVM()
                {
                    description = string.Format("Date Should be below {0}", validateDate),
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return null;
            }
            try {
                var data = dbContext.KTTU_ORDER_MASTER
                    .Where(ord => ord.company_code == companyCode
                        && ord.branch_code == brachCode
                        && ord.cflag != "Y" && ord.closed_flag != "Y" && (ord.is_lock != "Y" || ord.is_lock == null) && ord.bill_no == 0
                        && DbFunctions.TruncateTime(ord.order_date) <= DbFunctions.TruncateTime(date)).Select(om => new OrderMasterVM()
                        {
                            ObjID = om.obj_id,
                            CompanyCode = om.company_code,
                            BranchCode = om.branch_code,
                            OrderNo = om.order_no,
                            CustID = om.Cust_Id,
                            CustName = om.cust_name,
                            OrderType = om.order_type,
                            Remarks = om.remarks,
                            OrderDate = om.order_date,
                            OperatorCode = om.operator_code,
                            SalCode = om.sal_code,
                            DeliveryDate = om.delivery_date,
                            OrderRateType = om.order_rate_type,
                            gsCode = om.gs_code,
                            Rate = om.rate,
                            AdvacnceOrderAmount = om.advance_ord_amount,
                            GrandTotal = om.grand_total,
                            ObjectStatus = om.object_status,
                            BillNo = om.bill_no,
                            CFlag = om.cflag,
                            CancelledBy = om.cancelled_by,
                            BillCounter = om.bill_counter,
                            UpdateOn = om.UpdateOn,
                            ClosedBranch = om.closed_branch,
                            ClosedFlag = om.closed_flag,
                            ClosedBy = om.closed_by,
                            ClosedDate = om.closed_date,
                            Karat = om.karat,
                            OrderDayRate = om.order_day_rate,
                            NewBillNo = om.New_Bill_No,
                            TaxPercentage = om.tax_percentage,
                            TotalAmountBeforeTax = om.total_amount_before_tax,
                            TotalTaxAmount = om.total_tax_amount,
                            ShiftID = om.ShiftID,
                            IsPAN = om.isPAN,
                            OldOrderNo = om.old_order_no,
                            Address1 = om.address1,
                            Address2 = om.address2,
                            Address3 = om.address3,
                            City = om.city,
                            PinCode = om.pin_code,
                            MobileNo = om.mobile_no,
                            State = om.state,
                            StateCode = om.state_code,
                            TIN = om.tin,
                            PANNo = om.pan_no,
                            ESTNo = om.est_no,
                            IDType = om.id_type,
                            IDDetails = om.id_details,
                            BookingType = om.customer_type,
                            PhoneNo = om.phone_no,
                            EmailID = om.Email_ID,
                            Salutation = om.salutation,
                            ManagerCode = om.manager_code,
                            OrderAdvanceRateType = om.booking_type,
                            AdvancePercent = om.adv_percent,
                            RateValidityTill = om.rate_fixed_till,
                        }).ToList();
                return data;
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return null;
            }
        }

        public OrderMasterVM GetOrder(string companyCode, string brachCode, int orderNo, out ErrorVM error)
        {
            error = null;           
            try {
                var data = dbContext.KTTU_ORDER_MASTER
                    .Where(ord => ord.company_code == companyCode && ord.branch_code == brachCode
                        && ord.cflag != "Y" && ord.closed_flag != "Y" && (ord.is_lock != "Y" || ord.is_lock == null) 
                        && ord.bill_no == 0
                        && ord.order_no == orderNo)
                        .Select(om => new OrderMasterVM()
                        {
                            ObjID = om.obj_id,
                            CompanyCode = om.company_code,
                            BranchCode = om.branch_code,
                            OrderNo = om.order_no,
                            CustID = om.Cust_Id,
                            CustName = om.cust_name,
                            OrderType = om.order_type,
                            Remarks = om.remarks,
                            OrderDate = om.order_date,
                            OperatorCode = om.operator_code,
                            SalCode = om.sal_code,
                            DeliveryDate = om.delivery_date,
                            OrderRateType = om.order_rate_type,
                            gsCode = om.gs_code,
                            Rate = om.rate,
                            AdvacnceOrderAmount = om.advance_ord_amount,
                            GrandTotal = om.grand_total,
                            ObjectStatus = om.object_status,
                            BillNo = om.bill_no,
                            CFlag = om.cflag,
                            CancelledBy = om.cancelled_by,
                            BillCounter = om.bill_counter,
                            UpdateOn = om.UpdateOn,
                            ClosedBranch = om.closed_branch,
                            ClosedFlag = om.closed_flag,
                            ClosedBy = om.closed_by,
                            ClosedDate = om.closed_date,
                            Karat = om.karat,
                            OrderDayRate = om.order_day_rate,
                            NewBillNo = om.New_Bill_No,
                            TaxPercentage = om.tax_percentage,
                            TotalAmountBeforeTax = om.total_amount_before_tax,
                            TotalTaxAmount = om.total_tax_amount,
                            ShiftID = om.ShiftID,
                            IsPAN = om.isPAN,
                            OldOrderNo = om.old_order_no,
                            Address1 = om.address1,
                            Address2 = om.address2,
                            Address3 = om.address3,
                            City = om.city,
                            PinCode = om.pin_code,
                            MobileNo = om.mobile_no,
                            State = om.state,
                            StateCode = om.state_code,
                            TIN = om.tin,
                            PANNo = om.pan_no,
                            ESTNo = om.est_no,
                            IDType = om.id_type,
                            IDDetails = om.id_details,
                            BookingType = om.customer_type,
                            PhoneNo = om.phone_no,
                            EmailID = om.Email_ID,
                            Salutation = om.salutation,
                            ManagerCode = om.manager_code,
                            OrderAdvanceRateType = om.booking_type,
                            AdvancePercent = om.adv_percent,
                            RateValidityTill = om.rate_fixed_till,
                        }).FirstOrDefault();
                if(data == null) {
                    error = new ErrorVM { description = "Invalid order No. " + orderNo.ToString() };
                    return null;
                }
                return data;
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return null;
            }
        }

        public bool LockOrder(List<OrderMasterVM> orders, out ErrorVM error)
        {
            error = null;
            if (orders == null || orders.Count == 0) {
                error = new ErrorVM()
                {
                    description = "Invalid Order Details",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            foreach (OrderMasterVM ord in orders) {
                new OrderBL().ValidateOrder(ord.OrderNo, ord.CompanyCode, ord.BranchCode, "", 0, out error);
                if (error != null) return false;
            }
            try {
                foreach (OrderMasterVM ord in orders) {
                    KTTU_ORDER_MASTER orderMaster = dbContext.KTTU_ORDER_MASTER.Where(od => od.order_no == ord.OrderNo
                                                                                    && od.company_code == ord.CompanyCode
                                                                                    && od.branch_code == ord.BranchCode).FirstOrDefault();
                    orderMaster.is_lock = "Y";
                    orderMaster.booking_type = "DM";
                    orderMaster.UpdateOn = SIGlobals.Globals.GetDateTime();
                    dbContext.Entry(orderMaster).State = System.Data.Entity.EntityState.Modified;


                    KTTU_TRAN_MODI_LOG tml = new KTTU_TRAN_MODI_LOG();
                    tml.doc_type = "LO";
                    tml.doc_no = Convert.ToString(ord.OrderNo);
                    tml.doc_date = orderMaster.order_date;
                    tml.doc_description = "Order No-" + ord.OrderNo + " is locked by " + ord.OperatorCode;
                    tml.doc_remarks = ord.Remarks;
                    tml.operator_code = ord.OperatorCode;
                    tml.company_code = ord.CompanyCode;
                    tml.branch_code = ord.BranchCode;
                    dbContext.KTTU_TRAN_MODI_LOG.Add(tml);

                    dbContext.SaveChanges();
                }
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }
        #endregion

        #region Unlock Dormant Order

        public bool UnlockOrder(List<OrderMasterVM> orders, out ErrorVM error)
        {
            error = null;
            try {
                foreach (OrderMasterVM ord in orders) {
                   var orderVM = GetOrderToBeUnlocked(ord.CompanyCode, ord.BranchCode, ord.OrderNo, out error);
                   if (orderVM == null)
                        return false;
                    KTTU_ORDER_MASTER orderMaster = dbContext.KTTU_ORDER_MASTER.Where(od => od.order_no == ord.OrderNo
                                                                                    && od.company_code == ord.CompanyCode
                                                                                    && od.branch_code == ord.BranchCode).FirstOrDefault();
                    orderMaster.is_lock = "N";
                    orderMaster.remarks = ord.Remarks;
                    orderMaster.UpdateOn = SIGlobals.Globals.GetDateTime();
                    dbContext.Entry(orderMaster).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    KTTU_TRAN_MODI_LOG tml = new KTTU_TRAN_MODI_LOG();
                    tml.doc_type = "UO";
                    tml.doc_no = Convert.ToString(ord.OrderNo);
                    tml.doc_date = orderMaster.order_date;
                    tml.doc_description = "Order No-" + ord.OrderNo + " is Unlocked by " + ord.OperatorCode;
                    tml.doc_remarks = ord.Remarks;
                    tml.operator_code = ord.OperatorCode;
                    tml.company_code = ord.CompanyCode;
                    tml.branch_code = ord.BranchCode;
                    dbContext.KTTU_TRAN_MODI_LOG.Add(tml);
                }
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public OrderMasterVM GetOrderToBeUnlocked(string companyCode, string branchCode, int orderNo, out ErrorVM error)
        {
            error = null;
            try {
                var lockedOrder = dbContext.KTTU_ORDER_MASTER
                        .Where(ord => ord.company_code == companyCode
                            && ord.branch_code == branchCode
                            && ord.cflag != "Y" && ord.closed_flag != "Y" && ord.is_lock == "Y" && ord.bill_no == 0
                            && ord.order_no == orderNo
                            ).Select(om => new OrderMasterVM()
                            {
                                ObjID = om.obj_id,
                                CompanyCode = om.company_code,
                                BranchCode = om.branch_code,
                                OrderNo = om.order_no,
                                CustID = om.Cust_Id,
                                CustName = om.cust_name,
                                OrderType = om.order_type,
                                Remarks = om.remarks,
                                OrderDate = om.order_date,
                                OperatorCode = om.operator_code,
                                SalCode = om.sal_code,
                                DeliveryDate = om.delivery_date,
                                OrderRateType = om.order_rate_type,
                                gsCode = om.gs_code,
                                Rate = om.rate,
                                AdvacnceOrderAmount = om.advance_ord_amount,
                                GrandTotal = om.grand_total,
                                ObjectStatus = om.object_status,
                                BillNo = om.bill_no,
                                CFlag = om.cflag,
                                CancelledBy = om.cancelled_by,
                                BillCounter = om.bill_counter,
                                UpdateOn = om.UpdateOn,
                                ClosedBranch = om.closed_branch,
                                ClosedFlag = om.closed_flag,
                                ClosedBy = om.closed_by,
                                ClosedDate = om.closed_date,
                                Karat = om.karat,
                                OrderDayRate = om.order_day_rate,
                                NewBillNo = om.New_Bill_No,
                                TaxPercentage = om.tax_percentage,
                                TotalAmountBeforeTax = om.total_amount_before_tax,
                                TotalTaxAmount = om.total_tax_amount,
                                ShiftID = om.ShiftID,
                                IsPAN = om.isPAN,
                                OldOrderNo = om.old_order_no,
                                Address1 = om.address1,
                                Address2 = om.address2,
                                Address3 = om.address3,
                                City = om.city,
                                PinCode = om.pin_code,
                                MobileNo = om.mobile_no,
                                State = om.state,
                                StateCode = om.state_code,
                                TIN = om.tin,
                                PANNo = om.pan_no,
                                ESTNo = om.est_no,
                                IDType = om.id_type,
                                IDDetails = om.id_details,
                                BookingType = om.customer_type,
                                PhoneNo = om.phone_no,
                                EmailID = om.Email_ID,
                                Salutation = om.salutation,
                                ManagerCode = om.manager_code,
                                OrderAdvanceRateType = om.booking_type,
                                AdvancePercent = om.adv_percent,
                                RateValidityTill = om.rate_fixed_till,
                            }).FirstOrDefault();
                if(lockedOrder == null) {
                    error = new ErrorVM
                    {
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                        description = "Order detail is not found for the order No. " + orderNo.ToString()
                    };
                    return null;
                }
                return lockedOrder;
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return null;
            }
        }
        #endregion
        #endregion
    }
}
