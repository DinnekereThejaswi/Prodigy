using NumberToWordsINR;
using ProdigyAPI.BL.BusinessLayer.Common;
using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.BL.BusinessLayer.PrintConfig;
using ProdigyAPI.BL.BusinessLayer.Sales;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.BL.ViewModel.Payment;
using ProdigyAPI.BL.ViewModel.Print;
using ProdigyAPI.BL.ViewModel.Repair;
using ProdigyAPI.BL.ViewModel.Sales;
using ProdigyAPI.Model.MagnaDb;
using ProdigyAPI.SIGlobals;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

/// <summary>
/// Author: Mustureswara M M (Eshwar)
/// Started Date: 09/11/2019
/// </summary>
namespace ProdigyAPI.BL.BusinessLayer.Order
{
    public class OrderBL
    {
        #region Declaration
        MagnaDbEntities db = null;
        string MODULE_SEQ_NO = "2";
        string RECEIPT_SEQ_NO = "11";
        private const string TABLE_NAME = "KTTU_ORDER_MASTER";
        #endregion

        #region Constructors
        public OrderBL()
        {
            db = new MagnaDbEntities(true);
        }

        public OrderBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }
        #endregion

        #region Methods

        #region Orders
        public IQueryable<OrderMasterVM> GetOrderAllOrders(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                //List<OrderMasterVM> lstOfOrders = new List<OrderMasterVM>();
                //List<KTTU_ORDER_MASTER> kom = new List<KTTU_ORDER_MASTER>();
                //kom = db.KTTU_ORDER_MASTER.Where(ord => ord.company_code == companyCode
                //                                    && ord.branch_code == branchCode && ord.cflag == "N").ToList();
                //if (kom == null || kom.Count == 0) {
                //    error = new ErrorVM()
                //    {
                //        description = "No orders available.",
                //        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                //    };
                //    return null;
                //}
                //foreach (KTTU_ORDER_MASTER om in kom) {
                //    OrderMasterVM m = new OrderMasterVM();
                //    m.ObjID = om.obj_id;
                //    m.CompanyCode = om.company_code;
                //    m.BranchCode = om.branch_code;
                //    m.OrderNo = om.order_no;
                //    m.CustID = om.Cust_Id;
                //    m.CustName = om.cust_name;
                //    m.OrderType = om.order_type;
                //    m.Remarks = om.remarks;
                //    m.OrderDate = om.order_date;
                //    m.OperatorCode = om.operator_code;
                //    m.SalCode = om.sal_code;
                //    m.DeliveryDate = om.delivery_date;
                //    m.OrderRateType = om.order_rate_type;
                //    m.gsCode = om.gs_code;
                //    m.Rate = om.rate;
                //    m.AdvacnceOrderAmount = om.advance_ord_amount;
                //    m.GrandTotal = om.grand_total;
                //    m.ObjectStatus = om.object_status;
                //    m.BillNo = om.bill_no;
                //    m.CFlag = om.cflag;
                //    m.CancelledBy = om.cancelled_by;
                //    m.BillCounter = om.bill_counter;
                //    m.UpdateOn = om.UpdateOn;
                //    m.ClosedBranch = om.closed_branch;
                //    m.ClosedFlag = om.closed_flag;
                //    m.ClosedBy = om.closed_by;
                //    m.ClosedDate = om.closed_date;
                //    m.Karat = om.karat;
                //    m.OrderDayRate = om.order_day_rate;
                //    m.NewBillNo = om.New_Bill_No;
                //    m.TaxPercentage = om.tax_percentage;
                //    m.TotalAmountBeforeTax = om.total_amount_before_tax;
                //    m.TotalTaxAmount = om.total_tax_amount;
                //    m.ShiftID = om.ShiftID;
                //    m.IsPAN = om.isPAN;
                //    m.OldOrderNo = om.old_order_no;
                //    m.Address1 = om.address1;
                //    m.Address2 = om.address2;
                //    m.Address3 = om.address3;
                //    m.City = om.city;
                //    m.PinCode = om.pin_code;
                //    m.MobileNo = om.mobile_no;
                //    m.State = om.state;
                //    m.StateCode = om.state_code;
                //    m.TIN = om.tin;
                //    m.PANNo = om.pan_no;
                //    m.ESTNo = om.est_no;
                //    m.IDType = om.id_type;
                //    m.IDDetails = om.id_details;
                //    m.BookingType = om.customer_type;
                //    m.PhoneNo = om.phone_no;
                //    m.EmailID = om.Email_ID;
                //    m.Salutation = om.salutation;
                //    m.ManagerCode = om.manager_code;
                //    m.OrderAdvanceRateType = om.booking_type;
                //    m.AdvancePercent = om.adv_percent;
                //    m.RateValidityTill = om.rate_fixed_till;
                //    lstOfOrders.Add(m);
                //}
                //return lstOfOrders.AsQueryable();

                db.Configuration.UseDatabaseNullSemantics = true;
                var orderDetails = from od in db.KTTU_ORDER_DETAILS
                                   group new { od } by new
                                   {
                                       CompanyCode = od.company_code,
                                       BranchCode = od.branch_code,
                                       OrderNo = od.order_no
                                   } into g
                                   select new
                                   {
                                       CompanyCode = g.Key.CompanyCode,
                                       BranchCode = g.Key.BranchCode,
                                       OrderNo = g.Key.OrderNo,
                                       Weight = g.Sum(e => e.od.from_gwt)
                                   };
                var paymentDetails = from p in db.KTTU_PAYMENT_DETAILS
                                     where p.company_code == companyCode && p.branch_code == branchCode && p.pay_mode == "OP"
                                        && p.trans_type == "A" && p.cflag != "Y"
                                     select new
                                     {
                                         OrderKey = p.company_code + "$" + p.branch_code + "$" + p.Ref_BillNo.ToString()
                                     };

                var orderList = (from om in db.KTTU_ORDER_MASTER
                                 join od in orderDetails
                                 on new { CompanyCode = om.company_code, BranchCode = om.branch_code, OrderNo = om.order_no } equals
                                 new { CompanyCode = od.CompanyCode, BranchCode = od.BranchCode, OrderNo = od.OrderNo }
                                 where om.company_code == companyCode && om.branch_code == branchCode
                                    && om.cflag != "Y" && om.bill_no == 0 && om.closed_flag != "Y"
                                    && !paymentDetails.Any(p => p.OrderKey == om.company_code + "$" + om.branch_code + "$" + om.order_no.ToString())
                                 orderby om.order_no descending
                                 select new OrderMasterVM()
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
                return orderList.AsQueryable();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public dynamic GetManagerList(string companyCode, string branchCode)
        {
            var data = db.KSTU_SALESMAN_MASTER.Where(s => s.company_code == companyCode && s.branch_code == branchCode && s.obj_status != "C").Select(x => new { x.sal_code, x.sal_name }).OrderBy(s => s.sal_name).ToList();
            return data;
        }

        public dynamic GetBookingType(string companyCode, string branchCode)
        {
            var data = new List<BookingType>() { new BookingType() { Type = "W", Name = "Wedding" }, new BookingType() { Type = "N", Name = "Normal" } };
            return data;
        }

        public dynamic GetOrderItemType(string companyCode, string branchCode)
        {
            var data = new List<BookingType>() { new BookingType() { Type = "P", Name = "Plain" }, new BookingType() { Type = "S", Name = "Studded" } };
            return data;
        }

        public List<BilledBranchVM> GetBilledBranches(string companyCode, string branchCode)
        {
            List<BilledBranchVM> lstOfBB = new List<BilledBranchVM>();
            List<KSTU_SUPPLIER_MASTER> lstOfKSM = db.KSTU_SUPPLIER_MASTER.Where(ksm => ksm.voucher_code == "VB"
                                                                                && ksm.company_code == companyCode
                                                                                && ksm.branch_code == branchCode).ToList();
            foreach (KSTU_SUPPLIER_MASTER ksm in lstOfKSM) {
                BilledBranchVM bb = new BilledBranchVM();
                bb.PartyCode = ksm.party_code;
                bb.PartyName = ksm.party_name;
                lstOfBB.Add(bb);
            }
            return lstOfBB;
        }

        public List<OrderGSTypeVM> GetGSTTypes(string companyCode, string branchCode)
        {
            List<OrderGSTypeVM> lstOrderGSType = new List<OrderGSTypeVM>();
            var data = db.KSTS_GS_ITEM_ENTRY.Where(item => item.bill_type == "S"
             && item.object_status == "O"
             && item.company_code == companyCode
             && item.branch_code == branchCode
             && (item.measure_type == "P" || item.measure_type == "W")).OrderBy(x => x.display_order).ToList();

            foreach (KSTS_GS_ITEM_ENTRY kgie in data) {
                OrderGSTypeVM ogt = new OrderGSTypeVM();
                ogt.gsCode = kgie.gs_code;
                ogt.Name = kgie.item_level1_name;
                lstOrderGSType.Add(ogt);
            }
            return lstOrderGSType;
        }

        public List<OrderPayModeVM> GetPaymentModes(string companyCode, string branchCode)
        {
            List<OrderPayModeVM> lstOrderGSType = new List<OrderPayModeVM>();

            List<string> filters = new List<string>() { "PE", "OP", "SE", "CT" };
            var data = db.KTTS_PAYMENT_MASTER.Where(pay => !filters.Contains(pay.payment_code)
                                                    && (!pay.payment_code.Contains("B") || pay.payment_code == "BC")
                                                    && pay.company_code == companyCode
                                                    && pay.branch_code == branchCode).ToList();
            foreach (KTTS_PAYMENT_MASTER kgie in data) {
                OrderPayModeVM ogt = new OrderPayModeVM();
                ogt.PayMode = kgie.payment_code;
                ogt.PayName = kgie.payment_name;
                lstOrderGSType.Add(ogt);
            }
            return lstOrderGSType;
        }

        public List<OrderBankVM> GetBank(string companyCode, string branchCode)
        {
            //var data = from d in db.KTTS_CARD_COMMISSION
            //           join m in db.KSTU_ACC_LEDGER_MASTER
            //           on new { CompanyCode = d.company_code, BranchCode = d.branch_code, AccCode = d.acc_code }
            //           equals new { CompanyCode = m.company_code, BranchCode = m.branch_code, AccCode = m.acc_code }
            //           where d.company_code == companyCode && d.branch_code == branchCode
            //           select new OrderBankVM() { Bank = m.acc_name, BankName = m.acc_name };

            var data = from d in db.KTTS_CARD_COMMISSION
                       join m in db.KSTU_ACC_LEDGER_MASTER
                       on new { CompanyCode = d.company_code, BranchCode = d.branch_code, AccCode = d.acc_code }
                       equals new { CompanyCode = m.company_code, BranchCode = m.branch_code, AccCode = m.acc_code }
                       where d.company_code == companyCode && d.branch_code == branchCode
                       select new OrderBankVM() { Code = d.acc_code, BankName = m.acc_name };

            List<OrderBankVM> lstOrderBank = new List<OrderBankVM>();
            foreach (OrderBankVM order in data) {
                OrderBankVM orderBank = new OrderBankVM();
                orderBank.Bank = data.FirstOrDefault().Bank;
                orderBank.BankName = data.FirstOrDefault().BankName;
                lstOrderBank.Add(order);
            }
            return lstOrderBank;
        }

        public OrderBankVM GetBankCode(string companyCode, string branchCode, string bankCode)
        {
            var data = from d in db.KTTS_CARD_COMMISSION
                       join m in db.KSTU_ACC_LEDGER_MASTER
                       on new { CompanyCode = d.company_code, BranchCode = d.branch_code, AccCode = d.acc_code }
                       equals new { CompanyCode = m.company_code, BranchCode = m.branch_code, AccCode = m.acc_code }
                       where m.acc_name == bankCode && m.company_code == companyCode && m.branch_code == branchCode
                       select new OrderBankVM() { Bank = d.bank };
            OrderBankVM orderBank = new OrderBankVM();
            orderBank.Bank = data.FirstOrDefault().Bank;
            return orderBank;
        }

        public decimal GetRate(string companyCode, string branchCode, string gsCode, string karat)
        {
            KSTU_RATE_MASTER rateMaster = db.KSTU_RATE_MASTER.Where(r => r.gs_code == gsCode
                                                                    && (r.karat == karat
                                                                    || r.karat == "NA"
                                                                    || r.karat == "")
                                                                    && r.company_code == companyCode
                                                                    && r.branch_code == branchCode).FirstOrDefault();
            return rateMaster == null ? 0 : rateMaster.rate;
        }

        public List<OrderPurchasePlanTypeVM> GetPurchasePlan(string companyCode, string branchCode, string orderRateType, out ErrorVM error)
        {
            error = null;

            if (orderRateType.ToUpper() == "TODAY") {
                orderRateType = "Fixed";
            }
            switch (orderRateType) {
                case "Delivery":
                    var deliveryOrderRateTypeList = from rt in db.ORDER_TYPE_ACC_POSTING
                                                    where rt.company_code == companyCode && rt.branch_code == branchCode && rt.obj_status == "O" && rt.booking_code != "DM"
                                                    && (rt.order_rate_type == "All")
                                                    orderby rt.booking_name
                                                    select new OrderPurchasePlanTypeVM { BookingTypeCode = rt.booking_code, Name = rt.booking_name };
                    return deliveryOrderRateTypeList.ToList();
                case "Flexi":
                case "Fixed":
                    var orderTypes = from rt in db.ORDER_TYPE_ACC_POSTING
                                     where rt.company_code == companyCode && rt.branch_code == branchCode && rt.obj_status == "O" && rt.booking_code != "DM"
                                     && (rt.order_rate_type == "All" || rt.order_rate_type == orderRateType)
                                     orderby rt.booking_name
                                     select new OrderPurchasePlanTypeVM { BookingTypeCode = rt.booking_code, Name = rt.booking_name };
                    return orderTypes.ToList();
                default:
                    error = new ErrorVM();
                    error.description = "Booking type should be one of these {Delivery, Flexi, Fixed}";
                    error.ErrorStatusCode = System.Net.HttpStatusCode.BadRequest;
                    return null;

            }
        }

        public List<OrderPurchasePlanDetailVM> GetPurchasePlanDetail(string companyCode, string branchCode, string orderRateType, out ErrorVM error)
        {
            error = null;
            try {
                var purPlan = from pp in db.KSTU_ORDER_RATE_MASTER
                              where pp.company_code == companyCode && pp.branch_code == branchCode && pp.cflag == "O"
                              && pp.booking_name == orderRateType
                              orderby pp.adv_amount_per
                              select new OrderPurchasePlanDetailVM
                              {
                                  BookingTypeCode = pp.booking_name,
                                  Description = pp.description,
                                  AdvanceAmountPercent = pp.adv_amount_per,
                                  FixedDays = pp.fixed_days,
                                  MinimumWeightGrams = pp.min_weight
                              };
                return purPlan.ToList();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public OrderItemDetailsVM GetReservedOrderBarocdeInfo(string companyCode, string branchCode, string barcodeNo, out ErrorVM error)
        {
            error = null;
            SalesEstDetailsVM barcode = new BarcodeBL().GetBarcodeDetWithCalculation(companyCode, branchCode, barcodeNo, out error);
            if (barcode == null) {
                return null;
            }
            OrderItemDetailsVM kod = new OrderItemDetailsVM();
            kod.ItemName = barcodeNo;
            kod.Quantity = Convert.ToInt32(barcode.ItemQty);
            kod.Description = barcode.ItemName;
            kod.FromGrossWt = Convert.ToDecimal(barcode.Grosswt);
            kod.ToGrossWt = Convert.ToDecimal(barcode.Grosswt);
            kod.MCPer = Convert.ToDecimal(barcode.McPercent);
            kod.WastagePercent = Convert.ToDecimal(barcode.WastPercent);
            kod.Amount = Convert.ToDecimal(barcode.McAmount);
            kod.SFlag = "N";
            kod.SParty = "";
            kod.BillNo = 0;
            kod.ImgID = barcode.ImgID;
            kod.UpdateOn = SIGlobals.Globals.GetDateTime();
            kod.VAPercent = Convert.ToDecimal("0.00");
            kod.ItemAmount = barcode.TotalAmount;
            kod.GSCode = barcode.GsCode;
            kod.FinYear = 0;
            kod.ItemwiseTaxPercentage = Convert.ToDecimal("0.00");
            kod.ItemwiseTaxAmount = Convert.ToDecimal("0.00");
            kod.MCPerPiece = Convert.ToDecimal("0.00");
            kod.ProductID = "";
            kod.IsIssued = "";
            kod.MCPercent = barcode.McPercent;
            kod.MCType = barcode.McType;
            kod.EstNo = 0;
            kod.CounterCode = barcode.CounterCode;
            kod.SalCode = barcode.SalCode;
            return kod;
        }

        public List<OrderCounterVM> GetCounter(string companyCode, string branchCode)
        {
            var data = db.KSTU_COUNTER_MASTER.Where(kcm => kcm.company_code == companyCode
                                                   && kcm.branch_code == branchCode).OrderBy(c => c.counter_code).ToList();
            List<OrderCounterVM> lstOrderCounter = new List<OrderCounterVM>();
            foreach (KSTU_COUNTER_MASTER kcm in data) {
                OrderCounterVM ogt = new OrderCounterVM();
                ogt.CounterCode = kcm.counter_code;
                ogt.CounterName = kcm.counter_name;
                lstOrderCounter.Add(ogt);
            }
            return lstOrderCounter;
        }

        public List<OrderItemVM> GetItems(string companyCode, string branchCode, string counterCode, string gsCode)
        {
            var data = db.KTTU_COUNTER_STOCK.Where(cs => cs.counter_code == counterCode
             && cs.gs_code == gsCode
             && cs.company_code == companyCode
             && cs.branch_code == branchCode && (cs.op_gwt > 0
             || cs.op_units > 0
             || cs.barcoded_units > 0
             || cs.barcoded_gwt > 0
             || cs.sales_units > 0
             || cs.sales_gwt > 0
             || cs.issues_units > 0
             || cs.issues_gwt > 0
             || cs.receipt_gwt > 0
             || cs.closing_gwt > 0
             || cs.closing_units > 0)).ToList();

            List<OrderItemVM> lstOfOrderItem = new List<OrderItemVM>();
            foreach (KTTU_COUNTER_STOCK kcs in data) {
                OrderItemVM oiv = new OrderItemVM();
                oiv.GSCode = kcs.gs_code;
                oiv.ItemName = kcs.item_name;
                lstOfOrderItem.Add(oiv);
            }
            return lstOfOrderItem;
        }

        public dynamic GetChequeBankDetails(string companyCode, string branchCode)
        {
            int[] lstCheckMaster = db.KSTU_ACC_CHEQUE_MASTER.Where(kac => kac.company_code == companyCode && kac.branch_code == branchCode).Select(kacm => kacm.acc_code).ToArray();
            var lstAccLedger = db.KSTU_ACC_LEDGER_MASTER.Where(kalm => lstCheckMaster.Contains(kalm.acc_code) && kalm.company_code == companyCode && kalm.branch_code == branchCode);
            return lstAccLedger;
        }

        public List<KSTU_ACC_CHEQUE_MASTER> GetChequeListByBank(string companyCode, string branchCode, int accCode)
        {
            List<KSTU_ACC_CHEQUE_MASTER> lstOfCheque = db.KSTU_ACC_CHEQUE_MASTER.Where(kacm => kacm.acc_code == accCode
                                                                                        && kacm.chq_issued == "N"
                                                                                        && kacm.chq_closed == "N"
                                                                                        && kacm.company_code == companyCode
                                                                                        && kacm.branch_code == branchCode).OrderBy(kacm => kacm.chq_no).ToList();
            return lstOfCheque;
        }

        public List<AllOrdersVM> GetSearchedOrder(string companyCode, string branchCode, int estNo)
        {
            List<KTTU_PAYMENT_DETAILS> payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == estNo
                                                                               && pay.pay_mode == "OP"
                                                                               && pay.trans_type == "A"
                                                                               && pay.company_code == companyCode
                                                                               && pay.branch_code == branchCode).ToList();
            List<AllOrdersVM> lstAllOrders = new List<AllOrdersVM>();
            foreach (KTTU_PAYMENT_DETAILS payDet in payment) {
                int orderNo = Convert.ToInt32(payDet.Ref_BillNo);
                KTTU_ORDER_MASTER order = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == orderNo && ord.company_code == companyCode && ord.branch_code == branchCode).FirstOrDefault();
                AllOrdersVM ordersDet = new AllOrdersVM()
                {
                    OrderNo = order.order_no,
                    Customer = order.cust_name,
                    Date = order.order_date,
                    Amount = order.advance_ord_amount,
                    GoldRate = order.rate,
                    Type = order.order_rate_type,
                    CustomerID = order.order_no,
                    Staff = order.sal_code,
                    Karat = order.karat
                };
                lstAllOrders.Add(ordersDet);
            }
            return lstAllOrders;
        }

        public OrderMasterVM GetOrderInfo(string companyCode, string branchCode, int orderNo, out ErrorVM error)
        {
            try {
                KTTU_ORDER_MASTER kom = new KTTU_ORDER_MASTER();
                List<KTTU_ORDER_DETAILS> kod = new List<KTTU_ORDER_DETAILS>();
                List<KTTU_PAYMENT_DETAILS> kpd = new List<KTTU_PAYMENT_DETAILS>();

                kom = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == orderNo
                                                    && ord.company_code == companyCode
                                                    && ord.branch_code == branchCode).FirstOrDefault();
                kod = db.KTTU_ORDER_DETAILS.Where(ord => ord.order_no == orderNo
                                                    && ord.company_code == companyCode
                                                    && ord.branch_code == branchCode).ToList();
                kpd = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.series_no == orderNo
                                                    && ord.cflag == "N"
                                                    && ord.trans_type == "O"
                                                    && ord.company_code == companyCode
                                                    && ord.branch_code == branchCode).ToList().OrderBy(ord => ord.pay_date).ToList();

                error = null;
                if (kom == null) {
                    //error = new ErrorVM()
                    //{
                    //    field = "Order Number",
                    //    index = 0,
                    //    description = "Invalid order number",
                    //    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    //};
                    return null;
                }

                if (kom.cflag == "Y") {
                    error = new ErrorVM()
                    {
                        field = "Order Number",
                        index = 0,
                        description = "Order NO: " + orderNo + " is already cancelled.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }

                if (kom.closed_flag == "Y") {
                    error = new ErrorVM()
                    {
                        field = "Order Number",
                        index = 0,
                        description = "Order NO: " + orderNo + " is already closed.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }

                if (kom.bill_no != 0) {
                    error = new ErrorVM()
                    {
                        field = "Order Number",
                        index = 0,
                        description = "Order NO: " + orderNo + " is already adjusted towords Bill No: " + kom.bill_no + " Updated date: " + Convert.ToDateTime(kom.UpdateOn).ToString("dd/MM/yyyy") + "",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }

                #region Order Master Details
                OrderMasterVM om = new OrderMasterVM();
                om.ObjID = kom.obj_id;
                om.CompanyCode = kom.company_code;
                om.BranchCode = kom.branch_code;
                om.OrderNo = kom.order_no;
                om.CustID = kom.Cust_Id;
                om.CustName = kom.cust_name;
                om.OrderType = kom.order_type;
                om.Remarks = kom.remarks;
                om.OrderDate = kom.order_date;
                om.OperatorCode = kom.operator_code;
                om.SalCode = kom.sal_code;
                om.DeliveryDate = kom.delivery_date;
                om.OrderRateType = kom.order_rate_type;
                om.gsCode = kom.gs_code;
                om.Rate = kom.rate;
                om.AdvacnceOrderAmount = kom.advance_ord_amount;
                om.GrandTotal = kom.grand_total;
                om.ObjectStatus = kom.object_status;
                om.BillNo = kom.bill_no;
                om.CFlag = kom.cflag;
                om.CancelledBy = kom.cancelled_by;
                om.BillCounter = kom.bill_counter;
                om.UpdateOn = kom.UpdateOn;
                om.ClosedBranch = kom.closed_branch;
                om.ClosedFlag = kom.closed_flag;
                om.ClosedBy = kom.closed_by;
                om.ClosedDate = kom.closed_date;
                om.Karat = kom.karat;
                om.OrderDayRate = kom.order_day_rate;
                om.NewBillNo = kom.New_Bill_No;
                om.TaxPercentage = kom.tax_percentage;
                om.TotalAmountBeforeTax = kom.total_amount_before_tax;
                om.TotalTaxAmount = kom.total_tax_amount;
                om.ShiftID = kom.ShiftID;
                om.IsPAN = kom.isPAN;
                om.OldOrderNo = kom.old_order_no;
                om.Address1 = kom.address1;
                om.Address2 = kom.address2;
                om.Address3 = kom.address3;
                om.City = kom.city;
                om.PinCode = kom.pin_code;
                om.MobileNo = kom.mobile_no;
                om.State = kom.state;
                om.StateCode = kom.state_code;
                om.TIN = kom.tin;
                om.PANNo = kom.pan_no;
                om.ESTNo = kom.est_no;
                om.IDType = kom.id_type;
                om.IDDetails = kom.id_details;
                om.BookingType = kom.customer_type;
                om.PhoneNo = kom.phone_no;
                om.EmailID = kom.Email_ID;
                om.Salutation = kom.salutation;
                om.ManagerCode = kom.manager_code;
                om.OrderAdvanceRateType = kom.booking_type;
                om.AdvancePercent = kom.adv_percent;
                om.RateValidityTill = kom.rate_fixed_till;
                #endregion

                #region Order Details
                List<OrderItemDetailsVM> lstOfOrderDetils = new List<OrderItemDetailsVM>();
                foreach (KTTU_ORDER_DETAILS orderDet in kod) {
                    OrderItemDetailsVM oid = new OrderItemDetailsVM();
                    oid.ObjID = orderDet.obj_id;
                    oid.CompanyCode = orderDet.company_code;
                    oid.BranchCode = orderDet.branch_code;
                    oid.OrderNo = orderDet.order_no;
                    oid.SlNo = orderDet.slno;
                    oid.ItemName = orderDet.item_name;
                    oid.Quantity = orderDet.quantity;
                    oid.Description = orderDet.description;
                    oid.FromGrossWt = orderDet.from_gwt;
                    oid.ToGrossWt = orderDet.to_gwt;
                    oid.MCPer = orderDet.mc_per;
                    oid.WastagePercent = orderDet.wst_per;
                    oid.Amount = orderDet.amt;
                    oid.SFlag = orderDet.s_flag;
                    oid.SParty = orderDet.s_party;
                    oid.BillNo = orderDet.bill_no;
                    oid.ImgID = SIGlobals.Globals.GetOrderImagePath(db) + orderDet.img_id;
                    oid.UpdateOn = orderDet.UpdateOn;
                    oid.VAPercent = orderDet.va_percent;
                    oid.ItemAmount = orderDet.item_amt;
                    oid.GSCode = orderDet.gs_code;
                    oid.FinYear = orderDet.Fin_Year;
                    oid.ItemwiseTaxPercentage = orderDet.itemwise_tax_percentage;
                    oid.ItemwiseTaxAmount = orderDet.itemwise_tax_amount;
                    oid.MCPerPiece = orderDet.mcperpiece;
                    oid.ProductID = orderDet.productid;
                    oid.IsIssued = orderDet.is_Issued;
                    oid.CounterCode = orderDet.counter_code;
                    oid.SalCode = orderDet.sal_code;
                    oid.MCPercent = orderDet.mc_percent;
                    oid.MCType = orderDet.mc_type;
                    oid.EstNo = orderDet.est_no;
                    oid.ItemType = orderDet.item_type;
                    oid.AppSwt = orderDet.app_swt;
                    lstOfOrderDetils.Add(oid);
                }
                om.lstOfOrderItemDetailsVM = lstOfOrderDetils;
                #endregion

                #region Payment Details
                List<PaymentVM> lstOfPayment = new List<PaymentVM>();
                foreach (KTTU_PAYMENT_DETAILS paymentDet in kpd) {
                    PaymentVM payment = new PaymentVM();
                    payment.ObjID = paymentDet.obj_id;
                    payment.CompanyCode = paymentDet.company_code;
                    payment.BranchCode = paymentDet.branch_code;
                    payment.SeriesNo = paymentDet.series_no;
                    payment.ReceiptNo = paymentDet.receipt_no;
                    payment.SNo = paymentDet.sno;
                    payment.TransType = paymentDet.trans_type;
                    payment.PayMode = paymentDet.pay_mode;
                    payment.PayDetails = paymentDet.pay_details;
                    payment.PayDate = paymentDet.pay_date;
                    payment.PayAmount = paymentDet.pay_amt;
                    payment.RefBillNo = paymentDet.Ref_BillNo;
                    payment.PartyCode = paymentDet.party_code;
                    payment.BillCounter = paymentDet.bill_counter;
                    payment.IsPaid = paymentDet.is_paid;
                    payment.Bank = paymentDet.bank;
                    payment.BankName = paymentDet.bank_name;
                    payment.ChequeDate = paymentDet.cheque_date;
                    payment.CardType = paymentDet.card_type;
                    payment.ExpiryDate = paymentDet.expiry_date;
                    payment.CFlag = paymentDet.cflag;
                    payment.CardAppNo = paymentDet.card_app_no;
                    payment.SchemeCode = paymentDet.scheme_code;
                    payment.SalBillType = paymentDet.sal_bill_type;
                    payment.OperatorCode = paymentDet.operator_code;
                    payment.SessionNo = paymentDet.session_no;
                    payment.UpdateOn = paymentDet.UpdateOn;
                    payment.GroupCode = paymentDet.group_code;
                    payment.AmtReceived = paymentDet.amt_received;
                    payment.BonusAmt = paymentDet.bonus_amt;
                    payment.WinAmt = paymentDet.win_amt;
                    payment.CTBranch = paymentDet.ct_branch;
                    payment.FinYear = paymentDet.fin_year;
                    payment.CardCharges = paymentDet.CardCharges;
                    payment.ChequeNo = paymentDet.cheque_no;
                    payment.NewBillNo = paymentDet.new_receipt_no;
                    payment.AddDisc = paymentDet.Add_disc;
                    payment.IsOrderManual = paymentDet.isOrdermanual;
                    payment.CurrencyValue = paymentDet.currency_value;
                    payment.ExchangeRate = paymentDet.exchange_rate;
                    payment.CurrencyType = paymentDet.currency_type;
                    payment.TaxPercentage = paymentDet.tax_percentage;
                    payment.CancelledBy = paymentDet.cancelled_by;
                    payment.CancelledRemarks = paymentDet.cancelled_remarks;
                    payment.CancelledDate = paymentDet.cancelled_date;
                    payment.IsExchange = paymentDet.isExchange;
                    payment.ExchangeNo = paymentDet.exchangeNo;
                    payment.NewReceiptNo = paymentDet.new_receipt_no;
                    payment.GiftAmount = paymentDet.Gift_Amount;
                    payment.CardSwipedBy = paymentDet.cardSwipedBy;
                    payment.Version = paymentDet.version;
                    payment.GSTGroupCode = paymentDet.GSTGroupCode;
                    payment.SGSTPercent = paymentDet.SGST_Percent;
                    payment.CGSTPercent = paymentDet.CGST_Percent;
                    payment.IGSTPercent = paymentDet.IGST_Percent;
                    payment.HSN = paymentDet.HSN;
                    payment.SGSTAmount = paymentDet.SGST_Amount;
                    payment.CGSTAmount = paymentDet.CGST_Amount;
                    payment.IGSTAmount = paymentDet.IGST_Amount;
                    payment.PayAmountBeforeTax = paymentDet.pay_amount_before_tax;
                    payment.PayTaxAmount = paymentDet.pay_tax_amount;
                    lstOfPayment.Add(payment);
                }
                om.lstOfPayment = lstOfPayment;
                #endregion

                return om;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public OrderMasterVM GetViewOrderInfo(string companyCode, string branchCode, int orderNo, out ErrorVM error)
        {
            try {
                KTTU_ORDER_MASTER kom = new KTTU_ORDER_MASTER();
                List<KTTU_ORDER_DETAILS> kod = new List<KTTU_ORDER_DETAILS>();
                List<KTTU_PAYMENT_DETAILS> kpd = new List<KTTU_PAYMENT_DETAILS>();

                kom = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == orderNo
                                                    && ord.company_code == companyCode
                                                    && ord.branch_code == branchCode).FirstOrDefault();
                kod = db.KTTU_ORDER_DETAILS.Where(ord => ord.order_no == orderNo
                                                    && ord.company_code == companyCode
                                                    && ord.branch_code == branchCode).ToList();
                kpd = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.series_no == orderNo
                                                    && ord.trans_type != "CO"
                                                    && ord.cflag == "N"
                                                    && ord.trans_type == "O"
                                                    && ord.company_code == companyCode
                                                    && ord.branch_code == branchCode).ToList().OrderBy(ord => ord.pay_date).ToList();

                error = null;
                if (kom == null) {
                    error = new ErrorVM()
                    {
                        field = "Order Number",
                        index = 0,
                        description = "Invalid order number",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                    return null;
                }

                if (kom.bill_no != 0) {
                    error = new ErrorVM()
                    {
                        field = "Order Number",
                        index = 0,
                        description = "Order NO: " + orderNo + " is already adjusted towords Bill No: " + kom.bill_no + " Updated date: " + Convert.ToDateTime(kom.UpdateOn).ToString("dd/MM/yyyy") + "",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }

                #region Order Master Details
                OrderMasterVM om = new OrderMasterVM();
                om.ObjID = kom.obj_id;
                om.CompanyCode = kom.company_code;
                om.BranchCode = kom.branch_code;
                om.OrderNo = kom.order_no;
                om.CustID = kom.Cust_Id;
                om.CustName = kom.cust_name;
                om.OrderType = kom.order_type;
                om.Remarks = kom.remarks;
                om.OrderDate = kom.order_date;
                om.OperatorCode = kom.operator_code;
                om.SalCode = kom.sal_code;
                om.DeliveryDate = kom.delivery_date;
                om.OrderRateType = SIGlobals.Globals.GetOrderType(kom.order_rate_type);
                om.gsCode = kom.gs_code;
                om.Rate = kom.rate;
                om.AdvacnceOrderAmount = kom.advance_ord_amount;
                om.GrandTotal = kom.grand_total;
                om.ObjectStatus = kom.object_status;
                om.BillNo = kom.bill_no;
                om.CFlag = kom.cflag;
                om.CancelledBy = kom.cancelled_by;
                om.BillCounter = kom.bill_counter;
                om.UpdateOn = kom.UpdateOn;
                om.ClosedBranch = kom.closed_branch;
                om.ClosedFlag = kom.closed_flag;
                om.ClosedBy = kom.closed_by;
                om.ClosedDate = kom.closed_date;
                om.Karat = kom.karat;
                om.OrderDayRate = kom.order_day_rate;
                om.NewBillNo = kom.New_Bill_No;
                om.TaxPercentage = kom.tax_percentage;
                om.TotalAmountBeforeTax = kom.total_amount_before_tax;
                om.TotalTaxAmount = kom.total_tax_amount;
                om.ShiftID = kom.ShiftID;
                om.IsPAN = kom.isPAN;
                om.OldOrderNo = kom.old_order_no;
                om.Address1 = kom.address1;
                om.Address2 = kom.address2;
                om.Address3 = kom.address3;
                om.City = kom.city;
                om.PinCode = kom.pin_code;
                om.MobileNo = kom.mobile_no;
                om.State = kom.state;
                om.StateCode = kom.state_code;
                om.TIN = kom.tin;
                om.PANNo = kom.pan_no;
                om.ESTNo = kom.est_no;
                om.IDType = kom.id_type;
                om.IDDetails = kom.id_details;
                om.BookingType = kom.customer_type;
                om.PhoneNo = kom.phone_no;
                om.EmailID = kom.Email_ID;
                om.Salutation = kom.salutation;
                om.ManagerCode = kom.manager_code;
                om.OrderAdvanceRateType = kom.booking_type;
                om.AdvancePercent = kom.adv_percent;
                om.RateValidityTill = kom.rate_fixed_till;
                #endregion

                #region Order Details
                List<OrderItemDetailsVM> lstOfOrderDetils = new List<OrderItemDetailsVM>();
                foreach (KTTU_ORDER_DETAILS orderDet in kod) {
                    OrderItemDetailsVM oid = new OrderItemDetailsVM();
                    oid.ObjID = orderDet.obj_id;
                    oid.CompanyCode = orderDet.company_code;
                    oid.BranchCode = orderDet.branch_code;
                    oid.OrderNo = orderDet.order_no;
                    oid.SlNo = orderDet.slno;
                    oid.ItemName = orderDet.item_name;
                    oid.Quantity = orderDet.quantity;
                    oid.Description = orderDet.description;
                    oid.FromGrossWt = orderDet.from_gwt;
                    oid.ToGrossWt = orderDet.to_gwt;
                    oid.MCPer = orderDet.mc_per;
                    oid.WastagePercent = orderDet.wst_per;
                    oid.Amount = orderDet.amt;
                    oid.SFlag = orderDet.s_flag;
                    oid.SParty = orderDet.s_party;
                    oid.BillNo = orderDet.bill_no;
                    oid.ImgID = SIGlobals.Globals.GetOrderImagePath(db) + orderDet.img_id;
                    oid.UpdateOn = orderDet.UpdateOn;
                    oid.VAPercent = orderDet.va_percent;
                    oid.ItemAmount = orderDet.item_amt;
                    oid.GSCode = orderDet.gs_code;
                    oid.FinYear = orderDet.Fin_Year;
                    oid.ItemwiseTaxPercentage = orderDet.itemwise_tax_percentage;
                    oid.ItemwiseTaxAmount = orderDet.itemwise_tax_amount;
                    oid.MCPerPiece = orderDet.mcperpiece;
                    oid.ProductID = orderDet.productid;
                    oid.IsIssued = orderDet.is_Issued;
                    oid.CounterCode = orderDet.counter_code;
                    oid.SalCode = orderDet.sal_code;
                    oid.MCPercent = orderDet.mc_percent;
                    oid.MCType = orderDet.mc_type;
                    oid.EstNo = orderDet.est_no;
                    oid.ItemType = orderDet.item_type;
                    oid.AppSwt = orderDet.app_swt;
                    lstOfOrderDetils.Add(oid);
                }
                om.lstOfOrderItemDetailsVM = lstOfOrderDetils;
                #endregion

                #region Payment Details
                List<PaymentVM> lstOfPayment = new List<PaymentVM>();
                foreach (KTTU_PAYMENT_DETAILS paymentDet in kpd) {
                    PaymentVM payment = new PaymentVM();
                    payment.ObjID = paymentDet.obj_id;
                    payment.CompanyCode = paymentDet.company_code;
                    payment.BranchCode = paymentDet.branch_code;
                    payment.SeriesNo = paymentDet.series_no;
                    payment.ReceiptNo = paymentDet.receipt_no;
                    payment.SNo = paymentDet.sno;
                    payment.TransType = paymentDet.trans_type;
                    payment.PayMode = paymentDet.pay_mode;
                    payment.PayDetails = paymentDet.pay_details;
                    payment.PayDate = paymentDet.pay_date;
                    payment.PayAmount = paymentDet.pay_amt;
                    payment.RefBillNo = paymentDet.Ref_BillNo;
                    payment.PartyCode = paymentDet.party_code;
                    payment.BillCounter = paymentDet.bill_counter;
                    payment.IsPaid = paymentDet.is_paid;
                    payment.Bank = paymentDet.bank;
                    payment.BankName = paymentDet.bank_name;
                    payment.ChequeDate = paymentDet.cheque_date;
                    payment.CardType = paymentDet.card_type;
                    payment.ExpiryDate = paymentDet.expiry_date;
                    payment.CFlag = paymentDet.cflag;
                    payment.CardAppNo = paymentDet.card_app_no;
                    payment.SchemeCode = paymentDet.scheme_code;
                    payment.SalBillType = paymentDet.sal_bill_type;
                    payment.OperatorCode = paymentDet.operator_code;
                    payment.SessionNo = paymentDet.session_no;
                    payment.UpdateOn = paymentDet.UpdateOn;
                    payment.GroupCode = paymentDet.group_code;
                    payment.AmtReceived = paymentDet.amt_received;
                    payment.BonusAmt = paymentDet.bonus_amt;
                    payment.WinAmt = paymentDet.win_amt;
                    payment.CTBranch = paymentDet.ct_branch;
                    payment.FinYear = paymentDet.fin_year;
                    payment.CardCharges = paymentDet.CardCharges;
                    payment.ChequeNo = paymentDet.cheque_no;
                    payment.NewBillNo = paymentDet.new_receipt_no;
                    payment.AddDisc = paymentDet.Add_disc;
                    payment.IsOrderManual = paymentDet.isOrdermanual;
                    payment.CurrencyValue = paymentDet.currency_value;
                    payment.ExchangeRate = paymentDet.exchange_rate;
                    payment.CurrencyType = paymentDet.currency_type;
                    payment.TaxPercentage = paymentDet.tax_percentage;
                    payment.CancelledBy = paymentDet.cancelled_by;
                    payment.CancelledRemarks = paymentDet.cancelled_remarks;
                    payment.CancelledDate = paymentDet.cancelled_date;
                    payment.IsExchange = paymentDet.isExchange;
                    payment.ExchangeNo = paymentDet.exchangeNo;
                    payment.NewReceiptNo = paymentDet.new_receipt_no;
                    payment.GiftAmount = paymentDet.Gift_Amount;
                    payment.CardSwipedBy = paymentDet.cardSwipedBy;
                    payment.Version = paymentDet.version;
                    payment.GSTGroupCode = paymentDet.GSTGroupCode;
                    payment.SGSTPercent = paymentDet.SGST_Percent;
                    payment.CGSTPercent = paymentDet.CGST_Percent;
                    payment.IGSTPercent = paymentDet.IGST_Percent;
                    payment.HSN = paymentDet.HSN;
                    payment.SGSTAmount = paymentDet.SGST_Amount;
                    payment.CGSTAmount = paymentDet.CGST_Amount;
                    payment.IGSTAmount = paymentDet.IGST_Amount;
                    payment.PayAmountBeforeTax = paymentDet.pay_amount_before_tax;
                    payment.PayTaxAmount = paymentDet.pay_tax_amount;
                    lstOfPayment.Add(payment);
                }
                om.lstOfPayment = lstOfPayment;
                #endregion

                return om;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public List<PaymentVM> GetViewOrderInfoForClosedOrder(string companyCode, string branchCode, int orderNo)
        {
            List<KTTU_PAYMENT_DETAILS> kpd = new List<KTTU_PAYMENT_DETAILS>();
            kpd = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.series_no == orderNo
                                                && ord.trans_type == "CO"
                                                && ord.company_code == companyCode
                                                && ord.branch_code == branchCode).ToList();
            #region Payment Details
            List<PaymentVM> lstOfPayment = new List<PaymentVM>();
            foreach (KTTU_PAYMENT_DETAILS paymentDet in kpd) {
                PaymentVM payment = new PaymentVM();
                payment.ObjID = paymentDet.obj_id;
                payment.CompanyCode = paymentDet.company_code;
                payment.BranchCode = paymentDet.branch_code;
                payment.SeriesNo = paymentDet.series_no;
                payment.ReceiptNo = paymentDet.receipt_no;
                payment.SNo = paymentDet.sno;
                payment.TransType = paymentDet.trans_type;
                payment.PayMode = paymentDet.pay_mode;
                payment.PayDetails = paymentDet.pay_details;
                payment.PayDate = paymentDet.pay_date;
                payment.PayAmount = paymentDet.pay_amt;
                payment.RefBillNo = paymentDet.Ref_BillNo;
                payment.PartyCode = paymentDet.party_code;
                payment.BillCounter = paymentDet.bill_counter;
                payment.IsPaid = paymentDet.is_paid;
                payment.Bank = paymentDet.bank;
                payment.BankName = paymentDet.bank_name;
                payment.ChequeDate = paymentDet.cheque_date;
                payment.CardType = paymentDet.card_type;
                payment.ExpiryDate = paymentDet.expiry_date;
                payment.CFlag = paymentDet.cflag;
                payment.CardAppNo = paymentDet.card_app_no;
                payment.SchemeCode = paymentDet.scheme_code;
                payment.SalBillType = paymentDet.sal_bill_type;
                payment.OperatorCode = paymentDet.operator_code;
                payment.SessionNo = paymentDet.session_no;
                payment.UpdateOn = paymentDet.UpdateOn;
                payment.GroupCode = paymentDet.group_code;
                payment.AmtReceived = paymentDet.amt_received;
                payment.BonusAmt = paymentDet.bonus_amt;
                payment.WinAmt = paymentDet.win_amt;
                payment.CTBranch = paymentDet.ct_branch;
                payment.FinYear = paymentDet.fin_year;
                payment.CardCharges = paymentDet.CardCharges;
                payment.ChequeNo = paymentDet.cheque_no;
                payment.NewBillNo = paymentDet.new_receipt_no;
                payment.AddDisc = paymentDet.Add_disc;
                payment.IsOrderManual = paymentDet.isOrdermanual;
                payment.CurrencyValue = paymentDet.currency_value;
                payment.ExchangeRate = paymentDet.exchange_rate;
                payment.CurrencyType = paymentDet.currency_type;
                payment.TaxPercentage = paymentDet.tax_percentage;
                payment.CancelledBy = paymentDet.cancelled_by;
                payment.CancelledRemarks = paymentDet.cancelled_remarks;
                payment.CancelledDate = paymentDet.cancelled_date;
                payment.IsExchange = paymentDet.isExchange;
                payment.ExchangeNo = paymentDet.exchangeNo;
                payment.NewReceiptNo = paymentDet.new_receipt_no;
                payment.GiftAmount = paymentDet.Gift_Amount;
                payment.CardSwipedBy = paymentDet.cardSwipedBy;
                payment.Version = paymentDet.version;
                payment.GSTGroupCode = paymentDet.GSTGroupCode;
                payment.SGSTPercent = paymentDet.SGST_Percent;
                payment.CGSTPercent = paymentDet.CGST_Percent;
                payment.IGSTPercent = paymentDet.IGST_Percent;
                payment.HSN = paymentDet.HSN;
                payment.SGSTAmount = paymentDet.SGST_Amount;
                payment.CGSTAmount = paymentDet.CGST_Amount;
                payment.IGSTAmount = paymentDet.IGST_Amount;
                payment.PayAmountBeforeTax = paymentDet.pay_amount_before_tax;
                payment.PayTaxAmount = paymentDet.pay_tax_amount;
                lstOfPayment.Add(payment);
            }
            #endregion
            return lstOfPayment;
        }

        public OrderMasterVM GetCancelOrderView(string companyCode, string branchCode, int orderNo, out ErrorVM error)
        {
            error = null;
            try {

                #region Data Fetch
                KTTU_ORDER_MASTER kom = new KTTU_ORDER_MASTER();
                List<KTTU_ORDER_DETAILS> kod = new List<KTTU_ORDER_DETAILS>();
                List<KTTU_PAYMENT_DETAILS> kpd = new List<KTTU_PAYMENT_DETAILS>();

                kom = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == orderNo
                                                    && ord.company_code == companyCode
                                                    && ord.branch_code == branchCode).FirstOrDefault();
                kod = db.KTTU_ORDER_DETAILS.Where(ord => ord.order_no == orderNo
                                                    && ord.company_code == companyCode
                                                    && ord.branch_code == branchCode).ToList();
                kpd = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.series_no == orderNo
                                                    && ord.cflag == "N"
                                                    && ord.trans_type == "O"
                                                    && ord.company_code == companyCode
                                                    && ord.branch_code == branchCode).ToList().OrderBy(ord => ord.pay_date).ToList();
                #endregion

                #region Validations
                if (kom == null) {
                    error = new ErrorVM()
                    {
                        field = "Order Number",
                        index = 0,
                        description = "Invalid order number",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                    return null;
                }
                if (kom.cflag == "Y") {
                    error = new ErrorVM()
                    {
                        field = "Order Number",
                        index = 0,
                        description = "Order NO: " + orderNo + " is already cancelled.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }
                if (kom.closed_flag == "Y") {
                    error = new ErrorVM()
                    {
                        field = "Order Number",
                        index = 0,
                        description = "Order NO: " + orderNo + " is already closed.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }
                if (kom.bill_no != 0) {
                    error = new ErrorVM()
                    {
                        field = "Order Number",
                        index = 0,
                        description = "Order NO: " + orderNo + " is already adjusted towords Bill No: " + kom.bill_no + " Updated date: " + Convert.ToDateTime(kom.UpdateOn).ToString("dd/MM/yyyy") + "",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }

                string applicationDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode).ToString("yyyy-MM-dd");
                if (kom.order_date.ToString("yyyy-MM-dd") != applicationDate) {
                    error = new ErrorVM()
                    {
                        field = "Order",
                        index = 0,
                        description = "Only Today's Order can be cancelled.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }
                #endregion

                #region Order Master Details
                OrderMasterVM om = new OrderMasterVM();
                om.ObjID = kom.obj_id;
                om.CompanyCode = kom.company_code;
                om.BranchCode = kom.branch_code;
                om.OrderNo = kom.order_no;
                om.CustID = kom.Cust_Id;
                om.CustName = kom.cust_name;
                om.OrderType = kom.order_type;
                om.Remarks = kom.remarks;
                om.OrderDate = kom.order_date;
                om.OperatorCode = kom.operator_code;
                om.SalCode = kom.sal_code;
                om.DeliveryDate = kom.delivery_date;
                om.OrderRateType = kom.order_rate_type;
                om.gsCode = kom.gs_code;
                om.Rate = kom.rate;
                om.AdvacnceOrderAmount = kom.advance_ord_amount;
                om.GrandTotal = kom.grand_total;
                om.ObjectStatus = kom.object_status;
                om.BillNo = kom.bill_no;
                om.CFlag = kom.cflag;
                om.CancelledBy = kom.cancelled_by;
                om.BillCounter = kom.bill_counter;
                om.UpdateOn = kom.UpdateOn;
                om.ClosedBranch = kom.closed_branch;
                om.ClosedFlag = kom.closed_flag;
                om.ClosedBy = kom.closed_by;
                om.ClosedDate = kom.closed_date;
                om.Karat = kom.karat;
                om.OrderDayRate = kom.order_day_rate;
                om.NewBillNo = kom.New_Bill_No;
                om.TaxPercentage = kom.tax_percentage;
                om.TotalAmountBeforeTax = kom.total_amount_before_tax;
                om.TotalTaxAmount = kom.total_tax_amount;
                om.ShiftID = kom.ShiftID;
                om.IsPAN = kom.isPAN;
                om.OldOrderNo = kom.old_order_no;
                om.Address1 = kom.address1;
                om.Address2 = kom.address2;
                om.Address3 = kom.address3;
                om.City = kom.city;
                om.PinCode = kom.pin_code;
                om.MobileNo = kom.mobile_no;
                om.State = kom.state;
                om.StateCode = kom.state_code;
                om.TIN = kom.tin;
                om.PANNo = kom.pan_no;
                om.ESTNo = kom.est_no;
                om.IDType = kom.id_type;
                om.IDDetails = kom.id_details;
                om.BookingType = kom.customer_type;
                om.PhoneNo = kom.phone_no;
                om.EmailID = kom.Email_ID;
                om.Salutation = kom.salutation;
                om.ManagerCode = kom.manager_code;
                om.OrderAdvanceRateType = kom.booking_type;
                om.AdvancePercent = kom.adv_percent;
                om.RateValidityTill = kom.rate_fixed_till;
                #endregion

                #region Order Details
                List<OrderItemDetailsVM> lstOfOrderDetils = new List<OrderItemDetailsVM>();
                foreach (KTTU_ORDER_DETAILS orderDet in kod) {
                    OrderItemDetailsVM oid = new OrderItemDetailsVM();
                    oid.ObjID = orderDet.obj_id;
                    oid.CompanyCode = orderDet.company_code;
                    oid.BranchCode = orderDet.branch_code;
                    oid.OrderNo = orderDet.order_no;
                    oid.SlNo = orderDet.slno;
                    oid.ItemName = orderDet.item_name;
                    oid.Quantity = orderDet.quantity;
                    oid.Description = orderDet.description;
                    oid.FromGrossWt = orderDet.from_gwt;
                    oid.ToGrossWt = orderDet.to_gwt;
                    oid.MCPer = orderDet.mc_per;
                    oid.WastagePercent = orderDet.wst_per;
                    oid.Amount = orderDet.amt;
                    oid.SFlag = orderDet.s_flag;
                    oid.SParty = orderDet.s_party;
                    oid.BillNo = orderDet.bill_no;
                    oid.ImgID = orderDet.img_id;
                    oid.UpdateOn = orderDet.UpdateOn;
                    oid.VAPercent = orderDet.va_percent;
                    oid.ItemAmount = orderDet.item_amt;
                    oid.GSCode = orderDet.gs_code;
                    oid.FinYear = orderDet.Fin_Year;
                    oid.ItemwiseTaxPercentage = orderDet.itemwise_tax_percentage;
                    oid.ItemwiseTaxAmount = orderDet.itemwise_tax_amount;
                    oid.MCPerPiece = orderDet.mcperpiece;
                    oid.ProductID = orderDet.productid;
                    oid.IsIssued = orderDet.is_Issued;
                    oid.CounterCode = orderDet.counter_code;
                    oid.SalCode = orderDet.sal_code;
                    oid.MCPercent = orderDet.mc_percent;
                    oid.MCType = orderDet.mc_type;
                    oid.EstNo = orderDet.est_no;
                    lstOfOrderDetils.Add(oid);
                }
                om.lstOfOrderItemDetailsVM = lstOfOrderDetils;
                #endregion

                #region Payment Details
                List<PaymentVM> lstOfPayment = new List<PaymentVM>();
                foreach (KTTU_PAYMENT_DETAILS paymentDet in kpd) {
                    PaymentVM payment = new PaymentVM();
                    payment.ObjID = paymentDet.obj_id;
                    payment.CompanyCode = paymentDet.company_code;
                    payment.BranchCode = paymentDet.branch_code;
                    payment.SeriesNo = paymentDet.series_no;
                    payment.ReceiptNo = paymentDet.receipt_no;
                    payment.SNo = paymentDet.sno;
                    payment.TransType = paymentDet.trans_type;
                    payment.PayMode = paymentDet.pay_mode;
                    payment.PayDetails = paymentDet.pay_details;
                    payment.PayDate = paymentDet.pay_date;
                    payment.PayAmount = paymentDet.pay_amt;
                    payment.RefBillNo = paymentDet.Ref_BillNo;
                    payment.PartyCode = paymentDet.party_code;
                    payment.BillCounter = paymentDet.bill_counter;
                    payment.IsPaid = paymentDet.is_paid;
                    payment.Bank = paymentDet.bank;
                    payment.BankName = paymentDet.bank_name;
                    payment.ChequeDate = paymentDet.cheque_date;
                    payment.CardType = paymentDet.card_type;
                    payment.ExpiryDate = paymentDet.expiry_date;
                    payment.CFlag = paymentDet.cflag;
                    payment.CardAppNo = paymentDet.card_app_no;
                    payment.SchemeCode = paymentDet.scheme_code;
                    payment.SalBillType = paymentDet.sal_bill_type;
                    payment.OperatorCode = paymentDet.operator_code;
                    payment.SessionNo = paymentDet.session_no;
                    payment.UpdateOn = paymentDet.UpdateOn;
                    payment.GroupCode = paymentDet.group_code;
                    payment.AmtReceived = paymentDet.amt_received;
                    payment.BonusAmt = paymentDet.bonus_amt;
                    payment.WinAmt = paymentDet.win_amt;
                    payment.CTBranch = paymentDet.ct_branch;
                    payment.FinYear = paymentDet.fin_year;
                    payment.CardCharges = paymentDet.CardCharges;
                    payment.ChequeNo = paymentDet.cheque_no;
                    payment.NewBillNo = paymentDet.new_receipt_no;
                    payment.AddDisc = paymentDet.Add_disc;
                    payment.IsOrderManual = paymentDet.isOrdermanual;
                    payment.CurrencyValue = paymentDet.currency_value;
                    payment.ExchangeRate = paymentDet.exchange_rate;
                    payment.CurrencyType = paymentDet.currency_type;
                    payment.TaxPercentage = paymentDet.tax_percentage;
                    payment.CancelledBy = paymentDet.cancelled_by;
                    payment.CancelledRemarks = paymentDet.cancelled_remarks;
                    payment.CancelledDate = paymentDet.cancelled_date;
                    payment.IsExchange = paymentDet.isExchange;
                    payment.ExchangeNo = paymentDet.exchangeNo;
                    payment.NewReceiptNo = paymentDet.new_receipt_no;
                    payment.GiftAmount = paymentDet.Gift_Amount;
                    payment.CardSwipedBy = paymentDet.cardSwipedBy;
                    payment.Version = paymentDet.version;
                    payment.GSTGroupCode = paymentDet.GSTGroupCode;
                    payment.SGSTPercent = paymentDet.SGST_Percent;
                    payment.CGSTPercent = paymentDet.CGST_Percent;
                    payment.IGSTPercent = paymentDet.IGST_Percent;
                    payment.HSN = paymentDet.HSN;
                    payment.SGSTAmount = paymentDet.SGST_Amount;
                    payment.CGSTAmount = paymentDet.CGST_Amount;
                    payment.IGSTAmount = paymentDet.IGST_Amount;
                    payment.PayAmountBeforeTax = paymentDet.pay_amount_before_tax;
                    payment.PayTaxAmount = paymentDet.pay_tax_amount;
                    lstOfPayment.Add(payment);
                }
                om.lstOfPayment = lstOfPayment;
                #endregion

                return om;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        

        public OrderMasterVM SaveOrderInfoWithTran(OrderMasterVM order, out int genOrderNo, out int genReceiptNO, out ErrorVM error)
        {
            error = null;
            bool isOwnTransaction = false;
            if (db.Database.CurrentTransaction == null) {
                db.Database.BeginTransaction();
                isOwnTransaction = true;
            }

            OrderMasterVM ord = SaveOrderInfo(order, out genOrderNo, out genReceiptNO, out error);
            if (ord == null) {
                if (isOwnTransaction)
                    db.Database.CurrentTransaction.Rollback();
            }
            else if (genOrderNo == 0 && genReceiptNO == 0) {
                if (isOwnTransaction)
                    db.Database.CurrentTransaction.Rollback();
            }
            else {
                if (isOwnTransaction)
                    db.Database.CurrentTransaction.Commit();
            }
            return ord;
        }

        private OrderMasterVM SaveOrderInfo(OrderMasterVM order, out int genOrderNo, out int genReceiptNo, out ErrorVM error)
        {
            error = null;
            genOrderNo = 0;
            genReceiptNo = 0;
            if (order == null) {
                error = new ErrorVM()
                {
                    field = "Order Number",
                    index = 0,
                    description = "Invalid Order Details.",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return null;
            }

            decimal cashPaid = 0;
            decimal orderAdvanceAmount = 0;
            int finYear = SIGlobals.Globals.GetFinancialYear(db, order.CompanyCode, order.BranchCode);
            KTTU_ORDER_MASTER kom = new KTTU_ORDER_MASTER();
            KSTU_CUSTOMER_MASTER customer = new CustomerBL().GetActualCustomerDetails(order.CustID, order.MobileNo, order.CompanyCode, order.BranchCode);
            KSTU_RATE_MASTER rateDet = db.KSTU_RATE_MASTER.Where(r => r.company_code == order.CompanyCode && r.branch_code == order.BranchCode).FirstOrDefault();
            int receiptNo = 0;

            try {

                #region Calculating Payment Amount
                if (order.lstOfPayment != null) {
                    foreach (PaymentVM pvm in order.lstOfPayment) {
                        KTTU_PAYMENT_DETAILS kpd = new KTTU_PAYMENT_DETAILS();
                        kpd.pay_amt = pvm.PayAmount;
                        orderAdvanceAmount = orderAdvanceAmount + Convert.ToDecimal(pvm.PayAmount);
                        if (pvm.PayMode == "C") {
                            cashPaid = cashPaid + Convert.ToDecimal(pvm.PayAmount);
                        }
                    }
                }
                else {
                    #region Payment details validation
                    //Based on settings we are validating the payment details.
                    int? value = SIGlobals.Globals.GetApplicationConfigurationSettnigs(db, order.CompanyCode, order.BranchCode, "11052020");
                    //Need to validate payment
                    if (value != null && value == 0) {
                        if (order.lstOfPayment == null || order.lstOfPayment.Count == 0) {
                            error = new ErrorVM()
                            {
                                field = "Amount",
                                index = 0,
                                description = "Payment Details required.",
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                            };
                            return null;
                        }
                    }
                    #endregion
                }
                #endregion

                #region Flexi order and Fixed order Validation
                // Validating Flexi order Payment validation. Take Max_Value for calculation (Ram sir informed on 30/06/2020).
                if (order.OrderRateType == "Flexi" && orderAdvanceAmount > 0) {
                    KSTU_TOLERANCE_MASTER tolerance = db.KSTU_TOLERANCE_MASTER.Where(tol => tol.obj_id == 27032020).FirstOrDefault();
                    if (tolerance != null) {
                        decimal totalOrderWeight = order.lstOfOrderItemDetailsVM.Sum(det => det.FromGrossWt);
                        decimal rate = order.Rate;
                        decimal totalActualOrderValue = totalOrderWeight * rate;
                        decimal minPayAmount = (totalActualOrderValue * tolerance.Max_Val) / 100;

                        if (orderAdvanceAmount < minPayAmount) {
                            error = new ErrorVM()
                            {
                                field = "Amount",
                                index = 0,
                                description = "Insufficient advance amount for this flexi order, Minimum order booking amount is: " + Math.Round(minPayAmount, 0, MidpointRounding.ToEven) + " /-",
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                            };
                            return null;
                        }
                    }
                }
                // Validating Flexi order Payment validation. Take Max_Value for calculation (Ram sir informed on 30/06/2020).
                if (order.OrderRateType == "Fixed" && orderAdvanceAmount > 0 && Convert.ToDecimal(order.AdvancePercent) == 0) {
                    KSTU_TOLERANCE_MASTER tolerance = db.KSTU_TOLERANCE_MASTER.Where(tol => tol.obj_id == 200).FirstOrDefault();
                    if (tolerance != null) {
                        decimal totalOrderWeight = order.lstOfOrderItemDetailsVM.Sum(det => det.FromGrossWt);
                        decimal rate = order.Rate;
                        decimal totalActualOrderValue = totalOrderWeight * rate;
                        decimal minPayAmount = (totalActualOrderValue * tolerance.Max_Val) / 100;

                        if (orderAdvanceAmount < minPayAmount) {
                            error = new ErrorVM()
                            {
                                field = "Amount",
                                index = 0,
                                description = "Insufficient advance amount for this Fixed order, Minimum order booking amount is: " + Math.Round(minPayAmount, 0, MidpointRounding.ToEven) + " /-",
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                            };
                            return null;
                        }
                    }
                }

                //Checking Schme Validation
                decimal totalOrderValue = order.lstOfOrderItemDetailsVM.Sum(ord => ord.Amount);
                if (totalOrderValue > 0 && orderAdvanceAmount > 0 && order.AdvancePercent > 0) {
                    decimal minAmt = (Convert.ToDecimal(totalOrderValue) * Convert.ToDecimal(order.AdvancePercent)) / 100;
                    if (orderAdvanceAmount < minAmt) {
                        error = new ErrorVM()
                        {
                            field = "Amount",
                            index = 0,
                            description = "Insufficient advance amount for this order, Minimum booking amount is: " + Math.Round(minAmt, 0, MidpointRounding.ToEven) + " /-",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return null;
                    }
                }
                #endregion

                #region Validation FinYear Payment
                if (cashPaid > 0) {
                    decimal cashPaidInFinYear = GetTillNowPaidOrderAmount(order.CustID, order.CompanyCode, order.BranchCode) + cashPaid;
                    if (cashPaidInFinYear >= db.KSTU_TOLERANCE_MASTER.Where(ktm => ktm.obj_id == 500).FirstOrDefault().Max_Val && (customer.pan_no == "" || customer.pan_no == null)) {
                        if (order.PANNo == null || order.PANNo == "") {
                            error = new ErrorVM()
                            {
                                field = "PAN Number",
                                index = 0,
                                description = "Please update PAN No to continue",
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                            };
                            return null;
                        }
                        else {
                            customer.pan_no = order.PANNo;
                            db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                }
                #endregion

                #region Save Order Information
                int orderNo = Convert.ToInt32(SIGlobals.Globals.GetFinancialYear(db, order.CompanyCode, order.BranchCode).ToString().Remove(0, 1) + db.KSTS_SEQ_NOS.Where(sq => sq.obj_id == MODULE_SEQ_NO && sq.company_code == order.CompanyCode && sq.branch_code == order.BranchCode).FirstOrDefault().nextno);
                string objectID = SIGlobals.Globals.GetMagnaGUID(TABLE_NAME, orderNo, order.CompanyCode, order.BranchCode);
                kom.obj_id = objectID;
                kom.company_code = order.CompanyCode;
                kom.branch_code = order.BranchCode;
                kom.order_no = orderNo;
                order.OrderNo = kom.order_no;
                kom.Cust_Id = customer.cust_id;
                kom.cust_name = customer.cust_name;
                kom.order_type = order.OrderType;
                kom.remarks = order.Remarks == null ? "" : order.Remarks;
                kom.order_date = SIGlobals.Globals.GetApplicationDate(order.CompanyCode, order.BranchCode);
                kom.operator_code = order.OperatorCode;
                kom.sal_code = order.SalCode;
                kom.delivery_date = order.DeliveryDate;
                kom.order_rate_type = SIGlobals.Globals.GetOrderType(order.OrderRateType);
                kom.gs_code = order.gsCode;
                kom.rate = order.Rate;
                kom.advance_ord_amount = orderAdvanceAmount;
                kom.grand_total = order.GrandTotal;
                kom.object_status = "O";
                kom.bill_no = 0;
                kom.cflag = "N";
                kom.cancelled_by = "";// order.CancelledBy;
                kom.bill_counter = "FF";// order.BillCounter;
                kom.UpdateOn = SIGlobals.Globals.GetDateTime();
                kom.closed_branch = "";// order.ClosedBranch;
                kom.closed_flag = "N";
                kom.closed_by = "";// order.ClosedBy;
                kom.closed_date = SIGlobals.Globals.GetApplicationDate(order.CompanyCode, order.BranchCode);
                kom.karat = order.Karat;
                kom.order_day_rate = order.OrderDayRate;
                kom.New_Bill_No = order.NewBillNo;
                kom.tax_percentage = order.TaxPercentage;
                kom.total_amount_before_tax = order.TotalAmountBeforeTax;
                kom.total_tax_amount = order.TotalTaxAmount;
                kom.ShiftID = order.ShiftID;
                kom.isPAN = customer.pan_no == null ? "N" : "Y";
                kom.old_order_no = order.OldOrderNo;
                kom.UniqRowID = Guid.NewGuid();
                kom.address1 = customer.address1;
                kom.address2 = customer.address2;
                kom.address3 = customer.address3;
                kom.city = customer.city;
                kom.pin_code = customer.pin_code;
                kom.mobile_no = customer.mobile_no;
                kom.state = customer.state;
                kom.state_code = customer.state_code;
                kom.tin = customer.tin;
                kom.pan_no = customer.pan_no;
                kom.est_no = order.ESTNo;
                kom.id_type = order.IDType;
                kom.id_details = order.IDDetails;
                kom.booking_type = order.OrderAdvanceRateType;
                kom.phone_no = customer.phone_no;
                kom.Email_ID = customer.Email_ID;
                kom.salutation = customer.salutation;
                kom.manager_code = order.ManagerCode;
                kom.customer_type = order.BookingType;
                kom.adv_percent = order.AdvancePercent;
                kom.cancelled_date = SIGlobals.Globals.GetApplicationDate(order.CompanyCode, order.BranchCode);
                kom.rate_fixed_till = order.RateValidityDays == null ? kom.order_date : kom.order_date.AddDays(Convert.ToInt32(order.RateValidityDays));
                kom.store_location_id = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == order.CompanyCode
                                                                                && c.branch_code == order.BranchCode).FirstOrDefault().store_location_id;
                db.KTTU_ORDER_MASTER.Add(kom);
                #endregion

                #region Save Order Details and Payment
                string gstGroupCode = string.Empty;
                if (order.lstOfOrderItemDetailsVM != null) {
                    int orderDetSlNo = 1;
                    foreach (OrderItemDetailsVM oid in order.lstOfOrderItemDetailsVM) {
                        gstGroupCode = SIGlobals.Globals.GetGSTGroupCode(db, oid.GSCode, oid.CompanyCode, oid.BranchCode);
                        KTTU_ORDER_DETAILS kod = new KTTU_ORDER_DETAILS();
                        kod.obj_id = objectID;
                        kod.company_code = order.CompanyCode;
                        kod.branch_code = order.BranchCode;
                        kod.order_no = kom.order_no;
                        kod.slno = orderDetSlNo;
                        kod.item_name = oid.ItemName;
                        kod.quantity = oid.Quantity;
                        kod.description = oid.Description;
                        kod.from_gwt = oid.FromGrossWt;
                        kod.to_gwt = oid.ToGrossWt;
                        kod.mc_per = oid.MCPer;
                        kod.wst_per = oid.WastagePercent;
                        kod.amt = oid.Amount;
                        kod.s_flag = "N";
                        kod.s_party = "";
                        kod.bill_no = oid.BillNo;
                        if (oid.ImgID != null) {
                            kod.img_id = DoImageReplacement(order.BranchCode, orderNo, orderDetSlNo, oid.ImgID);
                        }
                        kod.UpdateOn = SIGlobals.Globals.GetDateTime();
                        kod.va_percent = oid.VAPercent;
                        kod.item_amt = oid.ItemAmount;
                        kod.gs_code = oid.GSCode;
                        kod.Fin_Year = oid.FinYear;
                        kod.itemwise_tax_percentage = oid.ItemwiseTaxPercentage;
                        kod.itemwise_tax_amount = oid.ItemwiseTaxAmount;
                        kod.mcperpiece = oid.MCPerPiece;
                        kod.productid = oid.ProductID;
                        kod.is_Issued = oid.IsIssued;
                        kod.counter_code = oid.CounterCode;
                        kod.sal_code = oid.SalCode;
                        kod.mc_percent = oid.MCPercent;
                        kod.mc_type = oid.MCType;
                        kod.est_no = oid.EstNo;
                        kod.UniqRowID = Guid.NewGuid();
                        kod.item_type = oid.ItemType;
                        kod.app_swt = Convert.ToDecimal(oid.AppSwt);
                        db.KTTU_ORDER_DETAILS.Add(kod);
                        orderDetSlNo = orderDetSlNo + 1;

                        if (kom.order_type == "R") {
                            KTTU_BARCODE_MASTER updBarcode = db.KTTU_BARCODE_MASTER.Where(bar => bar.barcode_no == oid.ItemName).FirstOrDefault();
                            updBarcode.order_no = kom.order_no;
                            db.Entry(updBarcode).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                }
                else {
                    error = new ErrorVM()
                    {
                        field = "PAN Number",
                        index = 0,
                        description = "There is no Order Details",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                    return null;
                }

                // Updating Order Sequence Number
                SIGlobals.Globals.UpdateSeqenceNumber(db, MODULE_SEQ_NO, order.CompanyCode, order.BranchCode);
                #endregion

                #region Payment Information 
                if (order.lstOfPayment != null && order.lstOfPayment.Count > 0) {
                    int paySlNo = 1;
                    string payGUID = objectID;
                    foreach (PaymentVM pvm in order.lstOfPayment) {
                        // Assigning GSTGroupCode is neccessary for Account posting.
                        pvm.GSTGroupCode = db.KSTS_GS_ITEM_ENTRY.Where(kgie => kgie.gs_code == kom.gs_code && kgie.company_code == order.CompanyCode && kgie.branch_code == order.BranchCode).FirstOrDefault().GSTGoodsGroupCode;

                        KTTU_PAYMENT_DETAILS kpd = new KTTU_PAYMENT_DETAILS();
                        receiptNo = Convert.ToInt32(SIGlobals.Globals.GetFinancialYear(db, order.CompanyCode, order.BranchCode).ToString().Remove(0, 1) + db.KSTS_SEQ_NOS.Where(sq => sq.obj_id == RECEIPT_SEQ_NO && sq.company_code == order.CompanyCode && sq.branch_code == order.BranchCode).FirstOrDefault().nextno);
                        kpd.obj_id = payGUID;
                        kpd.company_code = order.CompanyCode;
                        kpd.branch_code = order.BranchCode;
                        kpd.series_no = kom.order_no;
                        kpd.receipt_no = receiptNo;
                        pvm.ReceiptNo = kpd.receipt_no;
                        pvm.SeriesNo = kom.order_no;
                        kpd.sno = paySlNo;
                        kpd.trans_type = "O";
                        kpd.pay_mode = pvm.PayMode;
                        kpd.pay_details = pvm.PayDetails == null ? "" : pvm.PayDetails;
                        kpd.pay_date = SIGlobals.Globals.GetApplicationDate(order.CompanyCode, order.BranchCode);
                        kpd.pay_amt = pvm.PayAmount;
                        orderAdvanceAmount = orderAdvanceAmount + Convert.ToDecimal(pvm.PayAmount);
                        kpd.Ref_BillNo = pvm.RefBillNo == null ? "0" : pvm.RefBillNo;
                        kpd.party_code = pvm.PartyCode == null ? "" : pvm.PartyCode;
                        kpd.bill_counter = "FF";// pvm.BillCounter;
                        kpd.is_paid = pvm.IsPaid == null ? "N" : pvm.IsPaid;
                        kpd.bank = SIGlobals.Globals.GetBank(db, pvm.Bank, pvm.PayMode, pvm.CompanyCode, pvm.BranchCode);
                        kpd.bank_name = pvm.BankName == null ? "" : pvm.BankName;
                        kpd.cheque_date = pvm.ChequeDate == null ? SIGlobals.Globals.GetApplicationDate(order.CompanyCode, order.BranchCode) : pvm.ChequeDate;
                        kpd.card_type = pvm.CardType == null ? "" : pvm.CardType;
                        kpd.expiry_date = pvm.ExpiryDate == null ? SIGlobals.Globals.GetApplicationDate(order.CompanyCode, order.BranchCode) : pvm.ExpiryDate;
                        kpd.cflag = "N";
                        kpd.card_app_no = pvm.CardAppNo == null ? "" : pvm.CardAppNo;
                        kpd.scheme_code = pvm.SchemeCode == null ? "" : pvm.SchemeCode;
                        kpd.sal_bill_type = pvm.SalBillType;
                        kpd.operator_code = pvm.OperatorCode;
                        kpd.session_no = pvm.SessionNo == null ? 0 : pvm.SessionNo;
                        kpd.UpdateOn = SIGlobals.Globals.GetDateTime();
                        kpd.group_code = pvm.GroupCode;
                        kpd.amt_received = pvm.AmtReceived == null ? 0 : pvm.AmtReceived;
                        kpd.bonus_amt = pvm.BonusAmt == null ? 0 : pvm.BonusAmt;
                        kpd.win_amt = pvm.WinAmt == null ? 0 : pvm.BonusAmt; ;
                        kpd.ct_branch = pvm.CTBranch;
                        kpd.fin_year = finYear;
                        kpd.CardCharges = pvm.CardCharges == null ? 0 : pvm.CardCharges;
                        kpd.cheque_no = pvm.ChequeNo == null ? "" : pvm.ChequeNo;
                        kpd.cheque_no = pvm.PayMode == "R" ? pvm.Bank : pvm.ChequeNo;
                        kpd.New_Bill_No = pvm.NewBillNo;
                        kpd.Add_disc = pvm.AddDisc == null ? 0 : pvm.AddDisc;
                        kpd.isOrdermanual = pvm.IsOrderManual == null ? "N" : pvm.IsOrderManual;
                        kpd.currency_value = pvm.CurrencyValue == null ? 0 : pvm.CurrencyValue;
                        kpd.exchange_rate = pvm.ExchangeRate == null ? 0 : pvm.ExchangeRate;
                        kpd.currency_type = pvm.CurrencyType == null ? "INR" : pvm.CurrencyType;
                        kpd.tax_percentage = pvm.TaxPercentage == null ? 0 : pvm.TaxPercentage;
                        kpd.cancelled_by = pvm.CancelledBy == null ? "" : pvm.CancelledBy;
                        kpd.cancelled_remarks = pvm.CancelledRemarks == null ? "" : pvm.CancelledRemarks;
                        kpd.cancelled_date = SIGlobals.Globals.GetApplicationDate(order.CompanyCode, order.BranchCode).ToString();
                        kpd.isExchange = pvm.IsExchange == null ? "N" : pvm.IsExchange;
                        kpd.exchangeNo = pvm.ExchangeNo == null ? 0 : pvm.ExchangeNo;
                        kpd.new_receipt_no = pvm.NewReceiptNo;
                        kpd.Gift_Amount = pvm.GiftAmount == null ? 0 : pvm.GiftAmount;
                        kpd.cardSwipedBy = pvm.CardSwipedBy == null ? "" : pvm.CardSwipedBy;
                        kpd.version = pvm.Version == null ? 0 : pvm.Version;
                        kpd.GSTGroupCode = gstGroupCode;// pvm.GSTGroupCode;
                        kpd.SGST_Percent = pvm.SGSTPercent == null ? 0 : pvm.SGSTPercent;
                        kpd.CGST_Percent = pvm.CGSTPercent == null ? 0 : pvm.CGSTPercent;
                        kpd.IGST_Percent = pvm.IGSTPercent == null ? 0 : pvm.IGSTPercent;
                        kpd.HSN = pvm.HSN;
                        kpd.SGST_Amount = pvm.SGSTAmount == null ? 0 : pvm.SGSTAmount;
                        kpd.CGST_Amount = pvm.CGSTAmount == null ? 0 : pvm.CGSTAmount;
                        kpd.IGST_Amount = pvm.IGSTAmount == null ? 0 : pvm.IGSTAmount;
                        kpd.pay_amount_before_tax = pvm.PayAmountBeforeTax == null ? pvm.PayAmount : pvm.PayAmountBeforeTax;
                        kpd.pay_tax_amount = pvm.PayTaxAmount == null ? 0 : pvm.PayTaxAmount;
                        kpd.UniqRowID = Guid.NewGuid();
                        db.KTTU_PAYMENT_DETAILS.Add(kpd);
                        paySlNo = paySlNo + 1;

                        if (kpd.pay_mode == "CT") {
                            int memebershipNo = Convert.ToInt32(kpd.Ref_BillNo);
                            CHTU_CHIT_CLOSURE cc = db.CHTU_CHIT_CLOSURE.Where(ccl => ccl.Scheme_Code == kpd.scheme_code && ccl.Group_Code == kpd.group_code && ccl.Closed_At == kpd.ct_branch && ccl.Chit_MembShipNo == memebershipNo).FirstOrDefault();
                            kpd.amt_received = cc.Amt_Received;
                            kpd.bonus_amt = cc.Bonus_Amt;
                            kpd.win_amt = cc.win_amt;
                            cc.Bill_Number = kom.order_no.ToString();
                            db.Entry(cc).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                    // Updating Order Receipt Sequence Number
                    SIGlobals.Globals.UpdateSeqenceNumber(db, RECEIPT_SEQ_NO, order.CompanyCode, order.BranchCode);

                    #region Account Posting
                    //ErrorVM error = OrderAccountPosting(order.lstOfPayment, order.CustName);
                    //ErrorVM error = OrderAccountPostingNew(order.lstOfPayment, order.CustName);
                    //if (error != null && error.description != null && error.description != "")
                    //    throw new Exception(error.description);
                    db.SaveChanges();
                    //Account Posting by OrderNo then ReceiptNo should be pass as 0(Zero).
                    // Old Code calling the procedure which Eshwar created.
                    //ErrorVM procedureError = OrderAccountPostingWithProedure(kom.order_no, 0, order.CompanyCode, order.BranchCode);
                    //if (procedureError != null) {
                    //    error = procedureError;
                    //    error.ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError;
                    //    return false;
                    //}

                    // New Code calling the procedure created by Uday as on 29/06/2020
                    ErrorVM procedureError = OrderAccountPostingWithProedureNew(kom.order_no, receiptNo, order.CompanyCode, order.BranchCode);
                    if (procedureError != null) {
                        error = procedureError;
                        error.ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError;
                        return null;
                    }
                    #endregion
                }
                #endregion

                #region Saving
                db.SaveChanges();
                genOrderNo = kom.order_no;
                genReceiptNo = receiptNo;
                #endregion

                #region Market Place Updation
                if (order.OrderType == SIGlobals.OrderType.ReservedOrder.ToString()) {
                    bool updated = new Common.MarketplaceBL().UpdateMarketplaceInventory(order.CompanyCode,
                                            order.BranchCode, orderNo, SIGlobals.TransactionsType.Order, SIGlobals.ActionType.Block, out error);
                    error = null;
                }
                #endregion

                #region Document Creation Posting, error will not be checked
                //Post to DocumentCreationLog Table
                DateTime billDate = SIGlobals.Globals.GetApplicationDate(order.CompanyCode, order.BranchCode);
                new Common.CommonBL().PostDocumentCreation(order.CompanyCode, order.BranchCode, 4, orderNo, billDate, order.OperatorCode);
                #endregion

                return order;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                genOrderNo = 0;
                genReceiptNo = 0;
                return null;
            }
        }

        public List<PaymentVM> GetReceiptDetails(string companyCode, string branchCode, int receiptNo, out ErrorVM error)
        {
            error = null;
            try {
                List<KTTU_PAYMENT_DETAILS> kpd = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.receipt_no == receiptNo
                                                                                && ord.trans_type == "O"
                                                                                && ord.company_code == companyCode
                                                                                && ord.branch_code == branchCode)
                                                                                .OrderBy(ord => ord.pay_date)
                                                                                .ToList();
                if (kpd == null) {
                    error = new ErrorVM() { field = "Receipt Number", index = 0, description = "Invalid receipt number" };
                }
                List<PaymentVM> lstOfPayment = new List<PaymentVM>();
                foreach (KTTU_PAYMENT_DETAILS paymentDet in kpd) {
                    if (paymentDet.cflag == "Y") {
                        error = new ErrorVM()
                        {
                            field = "Receipt Number",
                            index = 0,
                            description = "Receipt No: " + receiptNo + " already cancelled.",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return null;
                    }
                    PaymentVM payment = new PaymentVM();
                    payment.ObjID = paymentDet.obj_id;
                    payment.CompanyCode = paymentDet.company_code;
                    payment.BranchCode = paymentDet.branch_code;
                    payment.SeriesNo = paymentDet.series_no;
                    payment.ReceiptNo = paymentDet.receipt_no;
                    payment.SNo = paymentDet.sno;
                    payment.TransType = paymentDet.trans_type;
                    payment.PayMode = paymentDet.pay_mode;
                    payment.PayDetails = paymentDet.pay_details;
                    payment.PayDate = paymentDet.pay_date;
                    payment.PayAmount = paymentDet.pay_amt;
                    payment.RefBillNo = paymentDet.Ref_BillNo;
                    payment.PartyCode = paymentDet.party_code;
                    payment.BillCounter = paymentDet.bill_counter;
                    payment.IsPaid = paymentDet.is_paid;
                    payment.Bank = paymentDet.bank;
                    payment.BankName = paymentDet.bank_name;
                    payment.ChequeDate = paymentDet.cheque_date;
                    payment.CardType = paymentDet.card_type;
                    payment.ExpiryDate = paymentDet.expiry_date;
                    payment.CFlag = paymentDet.cflag;
                    payment.CardAppNo = paymentDet.card_app_no;
                    payment.SchemeCode = paymentDet.scheme_code;
                    payment.SalBillType = paymentDet.sal_bill_type;
                    payment.OperatorCode = paymentDet.operator_code;
                    payment.SessionNo = paymentDet.session_no;
                    payment.UpdateOn = paymentDet.UpdateOn;
                    payment.GroupCode = paymentDet.group_code;
                    payment.AmtReceived = paymentDet.amt_received;
                    payment.BonusAmt = paymentDet.bonus_amt;
                    payment.WinAmt = paymentDet.win_amt;
                    payment.CTBranch = paymentDet.ct_branch;
                    payment.FinYear = paymentDet.fin_year;
                    payment.CardCharges = paymentDet.CardCharges;
                    payment.ChequeNo = paymentDet.cheque_no;
                    payment.NewBillNo = paymentDet.new_receipt_no;
                    payment.AddDisc = paymentDet.Add_disc;
                    payment.IsOrderManual = paymentDet.isOrdermanual;
                    payment.CurrencyValue = paymentDet.currency_value;
                    payment.ExchangeRate = paymentDet.exchange_rate;
                    payment.CurrencyType = paymentDet.currency_type;
                    payment.TaxPercentage = paymentDet.tax_percentage;
                    payment.CancelledBy = paymentDet.cancelled_by;
                    payment.CancelledRemarks = paymentDet.cancelled_remarks;
                    payment.CancelledDate = paymentDet.cancelled_date;
                    payment.IsExchange = paymentDet.isExchange;
                    payment.ExchangeNo = paymentDet.exchangeNo;
                    payment.NewReceiptNo = paymentDet.new_receipt_no;
                    payment.GiftAmount = paymentDet.Gift_Amount;
                    payment.CardSwipedBy = paymentDet.cardSwipedBy;
                    payment.Version = paymentDet.version;
                    payment.GSTGroupCode = paymentDet.GSTGroupCode;
                    payment.SGSTPercent = paymentDet.SGST_Percent;
                    payment.CGSTPercent = paymentDet.CGST_Percent;
                    payment.IGSTPercent = paymentDet.IGST_Percent;
                    payment.HSN = paymentDet.HSN;
                    payment.SGSTAmount = paymentDet.SGST_Amount;
                    payment.CGSTAmount = paymentDet.CGST_Amount;
                    payment.IGSTAmount = paymentDet.IGST_Amount;
                    payment.PayAmountBeforeTax = paymentDet.pay_amount_before_tax;
                    payment.PayTaxAmount = paymentDet.pay_tax_amount;
                    lstOfPayment.Add(payment);
                }
                return lstOfPayment;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public bool UpdateOrderInfo(int orderNo, OrderMasterVM order, out ErrorVM error)
        {
            error = null;

            KTTU_ORDER_MASTER kom = new KTTU_ORDER_MASTER();
            KSTU_CUSTOMER_MASTER customer = db.KSTU_CUSTOMER_MASTER.Where(cust => cust.cust_id == order.CustID
                                                                    && cust.company_code == order.CompanyCode
                                                                    && cust.branch_code == order.BranchCode)
                                                                   .FirstOrDefault();
            int ReceiptNo = 0;
            using (var transaction = db.Database.BeginTransaction()) {
                try {
                    #region Delating Existing Order
                    KTTU_ORDER_MASTER delkom = db.KTTU_ORDER_MASTER.Where(od => od.order_no == orderNo
                                                                            && od.company_code == order.CompanyCode
                                                                            && od.branch_code == order.BranchCode).FirstOrDefault();
                    db.KTTU_ORDER_MASTER.Remove(delkom);

                    List<KTTU_ORDER_DETAILS> delkod = db.KTTU_ORDER_DETAILS.Where(od => od.order_no == orderNo
                                                                                    && od.company_code == order.CompanyCode
                                                                                    && od.branch_code == order.BranchCode).ToList();
                    foreach (KTTU_ORDER_DETAILS orderDet in delkod) {
                        db.KTTU_ORDER_DETAILS.Remove(orderDet);
                    }

                    List<KTTU_PAYMENT_DETAILS> delkpd = db.KTTU_PAYMENT_DETAILS.Where(od => od.series_no == orderNo
                                                                                        && od.company_code == order.CompanyCode
                                                                                        && od.branch_code == order.BranchCode).ToList();
                    foreach (KTTU_PAYMENT_DETAILS payment in delkpd) {
                        db.KTTU_PAYMENT_DETAILS.Remove(payment);
                    }
                    #endregion

                    #region Save Order Information
                    string objectID = SIGlobals.Globals.GetMagnaGUID(TABLE_NAME, orderNo, order.CompanyCode, order.BranchCode);
                    kom.obj_id = objectID;
                    kom.company_code = order.CompanyCode;
                    kom.branch_code = order.BranchCode;
                    kom.order_no = order.OrderNo;
                    kom.Cust_Id = order.CustID;
                    kom.cust_name = customer.cust_name;
                    kom.order_type = order.OrderType;
                    kom.remarks = order.Remarks;
                    kom.order_date = order.OrderDate;
                    kom.operator_code = order.OperatorCode;
                    kom.sal_code = order.SalCode;
                    kom.delivery_date = order.DeliveryDate;
                    kom.order_rate_type = order.OrderRateType;
                    kom.gs_code = order.gsCode;
                    kom.rate = order.Rate;
                    kom.advance_ord_amount = order.AdvacnceOrderAmount;
                    kom.grand_total = order.GrandTotal;
                    kom.object_status = "O";
                    kom.bill_no = 0;
                    kom.cflag = "N";
                    kom.cancelled_by = order.CancelledBy;
                    kom.bill_counter = order.BillCounter;
                    kom.UpdateOn = SIGlobals.Globals.GetDateTime();
                    kom.closed_branch = order.ClosedBranch;
                    kom.closed_flag = order.ClosedFlag;
                    kom.closed_by = order.ClosedBy;
                    kom.closed_date = order.ClosedDate;
                    kom.karat = order.Karat;
                    kom.order_day_rate = order.OrderDayRate;
                    kom.New_Bill_No = order.NewBillNo;
                    kom.tax_percentage = order.TaxPercentage;
                    kom.total_amount_before_tax = order.TotalAmountBeforeTax;
                    kom.total_tax_amount = order.TotalTaxAmount;
                    kom.ShiftID = order.ShiftID;
                    kom.isPAN = customer.pan_no == null ? "N" : "Y";
                    kom.old_order_no = order.OldOrderNo;
                    kom.UniqRowID = Guid.NewGuid();
                    kom.address1 = customer.address1;
                    kom.address2 = customer.address2;
                    kom.address3 = customer.address3;
                    kom.city = customer.city;
                    kom.pin_code = customer.pin_code;
                    kom.mobile_no = customer.mobile_no;
                    kom.state = customer.state;
                    kom.state_code = customer.state_code;
                    kom.tin = customer.tin;
                    kom.pan_no = customer.pan_no;
                    kom.est_no = order.ESTNo;
                    kom.id_type = order.IDType;
                    kom.id_details = order.IDDetails;
                    kom.customer_type = order.BookingType;
                    kom.phone_no = customer.phone_no;
                    kom.Email_ID = customer.Email_ID;
                    kom.salutation = customer.salutation;
                    kom.manager_code = order.ManagerCode;
                    kom.booking_type = order.OrderAdvanceRateType;
                    kom.adv_percent = order.AdvancePercent;
                    kom.rate_fixed_till = order.RateValidityDays == null ? kom.order_date : kom.order_date.AddDays(Convert.ToInt32(order.RateValidityDays));
                    db.KTTU_ORDER_MASTER.Add(kom);
                    #endregion

                    #region Save Order Details and Payment
                    int orderDetSlNo = 1;
                    foreach (OrderItemDetailsVM oid in order.lstOfOrderItemDetailsVM) {
                        KTTU_ORDER_DETAILS kod = new KTTU_ORDER_DETAILS();
                        kod.obj_id = objectID;
                        kod.company_code = oid.CompanyCode;
                        kod.branch_code = oid.BranchCode;
                        kod.order_no = oid.OrderNo;
                        kod.slno = orderDetSlNo;
                        kod.item_name = oid.ItemName;
                        kod.quantity = oid.Quantity;
                        kod.description = oid.Description;
                        kod.from_gwt = oid.FromGrossWt;
                        kod.to_gwt = oid.ToGrossWt;
                        kod.mc_per = oid.MCPer;
                        kod.wst_per = oid.WastagePercent;
                        kod.amt = oid.Amount;
                        kod.s_flag = "N";
                        kod.s_party = "";
                        kod.bill_no = oid.BillNo;
                        //kod.img_id = oid.ImgID;
                        if (oid.ImgID != null) {
                            kod.img_id = DoImageReplacement(order.BranchCode, orderNo, orderDetSlNo, oid.ImgID);
                        }
                        kod.UpdateOn = SIGlobals.Globals.GetDateTime();
                        kod.va_percent = oid.VAPercent;
                        kod.item_amt = oid.ItemAmount;
                        kod.gs_code = oid.GSCode;
                        kod.Fin_Year = oid.FinYear;
                        kod.itemwise_tax_percentage = oid.ItemwiseTaxPercentage;
                        kod.itemwise_tax_amount = oid.ItemwiseTaxAmount;
                        kod.mcperpiece = oid.MCPerPiece;
                        kod.productid = oid.ProductID;
                        kod.is_Issued = oid.IsIssued;
                        kod.counter_code = oid.CounterCode;
                        kod.sal_code = oid.SalCode;
                        kod.mc_percent = oid.MCPercent;
                        kod.mc_type = oid.MCType;
                        kod.est_no = oid.EstNo;
                        kod.UniqRowID = Guid.NewGuid();
                        db.KTTU_ORDER_DETAILS.Add(kod);
                        orderDetSlNo = orderDetSlNo + 1;

                        if (kom.order_type == "R") {
                            KTTU_BARCODE_MASTER updBarcode = db.KTTU_BARCODE_MASTER.Where(bar => bar.barcode_no == oid.ItemName).FirstOrDefault();
                            updBarcode.order_no = kom.order_no;
                            db.Entry(updBarcode).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                    #endregion

                    #region Payment Information 
                    int paySlNo = 1;
                    string payGUID = objectID;

                    foreach (PaymentVM pvm in order.lstOfPayment) {
                        KTTU_PAYMENT_DETAILS kpd = new KTTU_PAYMENT_DETAILS();
                        kpd.obj_id = payGUID;
                        kpd.company_code = pvm.CompanyCode;
                        kpd.branch_code = pvm.BranchCode;
                        kpd.series_no = kom.order_no;
                        kpd.receipt_no = pvm.ReceiptNo;
                        ReceiptNo = pvm.ReceiptNo;
                        kpd.sno = paySlNo;
                        kpd.trans_type = pvm.TransType;
                        kpd.pay_mode = pvm.PayMode;
                        kpd.pay_details = pvm.PayDetails;
                        kpd.pay_date = SIGlobals.Globals.GetDateTime();
                        kpd.pay_amt = pvm.PayAmount;
                        kpd.Ref_BillNo = pvm.RefBillNo;
                        kpd.party_code = pvm.PartyCode;
                        kpd.bill_counter = pvm.BillCounter;
                        kpd.is_paid = pvm.IsPaid;
                        kpd.bank = pvm.Bank;
                        kpd.bank_name = pvm.BankName;
                        kpd.cheque_date = pvm.ChequeDate;
                        kpd.card_type = pvm.CardType;
                        kpd.expiry_date = pvm.ExpiryDate;
                        kpd.cflag = pvm.CFlag;
                        kpd.card_app_no = pvm.CardAppNo;
                        kpd.scheme_code = pvm.SchemeCode;
                        kpd.sal_bill_type = pvm.SalBillType;
                        kpd.operator_code = pvm.OperatorCode;
                        kpd.session_no = pvm.SessionNo;
                        kpd.UpdateOn = SIGlobals.Globals.GetDateTime();
                        kpd.group_code = pvm.GroupCode;
                        kpd.amt_received = pvm.AmtReceived;
                        kpd.bonus_amt = pvm.BonusAmt;
                        kpd.win_amt = pvm.WinAmt;
                        kpd.ct_branch = pvm.CTBranch;
                        kpd.fin_year = pvm.FinYear;
                        kpd.CardCharges = pvm.CardCharges;
                        kpd.cheque_no = pvm.ChequeNo;
                        kpd.New_Bill_No = pvm.NewBillNo;
                        kpd.Add_disc = pvm.AddDisc;
                        kpd.isOrdermanual = pvm.IsOrderManual;
                        kpd.currency_value = pvm.CurrencyValue;
                        kpd.exchange_rate = pvm.ExchangeRate;
                        kpd.currency_type = pvm.CurrencyType;
                        kpd.tax_percentage = pvm.TaxPercentage;
                        kpd.cancelled_by = pvm.CancelledBy;
                        kpd.cancelled_remarks = pvm.CancelledRemarks;
                        kpd.cancelled_date = pvm.CancelledDate;
                        kpd.isExchange = pvm.IsExchange;
                        kpd.exchangeNo = pvm.ExchangeNo;
                        kpd.new_receipt_no = pvm.NewReceiptNo;
                        kpd.Gift_Amount = pvm.GiftAmount;
                        kpd.cardSwipedBy = pvm.CardSwipedBy;
                        kpd.version = pvm.Version;
                        kpd.GSTGroupCode = pvm.GSTGroupCode;
                        kpd.SGST_Percent = pvm.SGSTPercent;
                        kpd.CGST_Percent = pvm.CGSTPercent;
                        kpd.IGST_Percent = pvm.IGSTPercent;
                        kpd.HSN = pvm.HSN;
                        kpd.SGST_Amount = pvm.SGSTAmount;
                        kpd.CGST_Amount = pvm.CGSTAmount;
                        kpd.IGST_Amount = pvm.IGSTAmount;
                        kpd.pay_amount_before_tax = pvm.PayAmountBeforeTax;
                        kpd.pay_tax_amount = pvm.PayTaxAmount;
                        kpd.UniqRowID = Guid.NewGuid();
                        db.KTTU_PAYMENT_DETAILS.Add(kpd);
                        paySlNo = paySlNo + 1;
                    }
                    #endregion

                    db.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception excp) {
                    transaction.Rollback();
                    error = new ErrorVM().GetErrorDetails(excp);
                    return false;
                }
            }
        }

        public OrderMasterVM GetOrderReceiptGetDetails(string companyCode, string branchCode, int orderNo, out ErrorVM error)
        {
            error = null;
            try {
                decimal totalPaidAmount = 0;
                KTTU_ORDER_MASTER kom = new KTTU_ORDER_MASTER();
                List<KTTU_ORDER_DETAILS> kod = new List<KTTU_ORDER_DETAILS>();
                List<KTTU_PAYMENT_DETAILS> kpd = new List<KTTU_PAYMENT_DETAILS>();

                kom = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == orderNo
                                                    && ord.company_code == companyCode
                                                    && ord.branch_code == branchCode).FirstOrDefault();
                kod = db.KTTU_ORDER_DETAILS.Where(ord => ord.order_no == orderNo
                                                    && ord.company_code == companyCode
                                                    && ord.branch_code == branchCode).ToList();
                kpd = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.series_no == orderNo
                                                    && ord.trans_type == "O"
                                                    && ord.cflag == "N" && ord.company_code == companyCode
                                                    && ord.branch_code == branchCode).ToList();
                if (kom == null) {
                    error = new ErrorVM()
                    {
                        field = "Order Number",
                        index = 0,
                        description = "Invalid order number",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                    return null;
                }

                if (kom.cflag == "Y") {
                    error = new ErrorVM()
                    {
                        field = "Order Number",
                        index = 0,
                        description = "Order No: " + orderNo + " is already cancelled.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }

                if (kom.closed_flag == "Y") {
                    error = new ErrorVM()
                    {
                        field = "Order Number",
                        index = 0,
                        description = "Order NO: " + orderNo + " is already closed.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }

                if (kom.bill_no != 0) {
                    error = new ErrorVM()
                    {
                        field = "Order Number",
                        index = 0,
                        description = "Order NO: " + orderNo + " is already adjusted towords Bill No: " + kom.bill_no + " Updated date: " + Convert.ToDateTime(kom.UpdateOn).ToString("dd/MM/yyyy") + "",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }

                #region Order Master Details
                OrderMasterVM om = new OrderMasterVM();
                om.ObjID = kom.obj_id;
                om.CompanyCode = kom.company_code;
                om.BranchCode = kom.branch_code;
                om.OrderNo = kom.order_no;
                om.CustID = kom.Cust_Id;
                om.CustName = kom.cust_name;
                om.OrderType = kom.order_type;
                om.Remarks = kom.remarks;
                om.OrderDate = kom.order_date;
                om.OperatorCode = kom.operator_code;
                om.SalCode = kom.sal_code;
                om.DeliveryDate = kom.delivery_date;
                om.OrderRateType = kom.order_rate_type;
                om.gsCode = kom.gs_code;
                om.Rate = kom.rate;
                om.AdvacnceOrderAmount = kom.advance_ord_amount;
                om.GrandTotal = kom.grand_total;
                om.ObjectStatus = kom.object_status;
                om.BillNo = kom.bill_no;
                om.CFlag = kom.cflag;
                om.CancelledBy = kom.cancelled_by;
                om.BillCounter = kom.bill_counter;
                om.UpdateOn = kom.UpdateOn;
                om.ClosedBranch = kom.closed_branch;
                om.ClosedFlag = kom.closed_flag;
                om.ClosedBy = kom.closed_by;
                om.ClosedDate = kom.closed_date;
                om.Karat = kom.karat;
                om.OrderDayRate = kom.order_day_rate;
                om.NewBillNo = kom.New_Bill_No;
                om.TaxPercentage = kom.tax_percentage;
                om.TotalAmountBeforeTax = kom.total_amount_before_tax;
                om.TotalTaxAmount = kom.total_tax_amount;
                om.ShiftID = kom.ShiftID;
                om.IsPAN = kom.isPAN;
                om.OldOrderNo = kom.old_order_no;
                om.Address1 = kom.address1;
                om.Address2 = kom.address2;
                om.Address3 = kom.address3;
                om.City = kom.city;
                om.PinCode = kom.pin_code;
                om.MobileNo = kom.mobile_no;
                om.State = kom.state;
                om.StateCode = kom.state_code;
                om.TIN = kom.tin;
                om.PANNo = kom.pan_no;
                om.ESTNo = kom.est_no;
                om.IDType = kom.id_type;
                om.IDDetails = kom.id_details;
                om.BookingType = kom.customer_type;
                om.PhoneNo = kom.phone_no;
                om.EmailID = kom.Email_ID;
                om.Salutation = kom.salutation;
                om.OrderAdvanceRateType = kom.booking_type;
                om.AdvancePercent = kom.adv_percent;
                om.RateValidityTill = kom.rate_fixed_till;
                #endregion

                #region Payment Details
                List<PaymentVM> lstOfPayment = new List<PaymentVM>();
                foreach (KTTU_PAYMENT_DETAILS paymentDet in kpd) {
                    PaymentVM payment = new PaymentVM();
                    payment.PayAmount = paymentDet.pay_amt;
                    totalPaidAmount = Convert.ToInt32(totalPaidAmount + payment.PayAmount);
                }

                #region Order Details
                List<OrderItemDetailsVM> lstOfOrderDetils = new List<OrderItemDetailsVM>();
                OrderItemDetailsVM oid = new OrderItemDetailsVM();
                oid.CompanyCode = companyCode;
                oid.BranchCode = branchCode;
                oid.Amount = totalPaidAmount;
                lstOfOrderDetils.Add(oid);
                om.lstOfOrderItemDetailsVM = lstOfOrderDetils;
                #endregion
                om.lstOfPayment = lstOfPayment;
                #endregion

                return om;
            }
            catch (Exception excp) {
                error = new ErrorVM()
                {
                    field = "Internal Server Error",
                    index = 0,
                    description = excp.Message,
                    ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError
                };
                return null;
            }
        }

        public int SaveOrderReceiptDetails(OrderMasterVM order, out ErrorVM error)
        {
            if (order == null) {
                error = new ErrorVM() { field = "", index = 0, description = "Invalid Data.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return 0;
            }

            error = null;
            decimal cashPaid = 0;
            decimal cashPaidInFinYear = 0;
            int finYear = SIGlobals.Globals.GetFinancialYear(db, order.CompanyCode, order.BranchCode);
            KSTU_CUSTOMER_MASTER customer = new CustomerBL().GetActualCustomerDetails(order.CustID, order.MobileNo, order.CompanyCode, order.BranchCode);

            #region Flexi order payment Validation
            //Checking Configuration for Allow receipt for Fixed Rate or Not.
            if (order.OrderRateType == "Today") {
                APP_CONFIG_TABLE config = db.APP_CONFIG_TABLE.Where(c => c.obj_id == "28082018" && c.company_code == order.CompanyCode && c.branch_code == order.BranchCode).FirstOrDefault();
                if (config != null) {
                    if (config.value == 1) {
                        error = new ErrorVM() { field = "", index = 0, description = "Fixed order cannot create order receipt. Please generate new order.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                        return 0;
                    }
                }
            }
            #endregion

            #region Calculating Payment Amount
            if (order.lstOfPayment != null) {
                foreach (PaymentVM pvm in order.lstOfPayment) {
                    KTTU_PAYMENT_DETAILS kpd = new KTTU_PAYMENT_DETAILS();
                    if (pvm.PayMode == "C") {
                        cashPaid = cashPaid + Convert.ToDecimal(pvm.PayAmount);
                    }
                }
            }
            else {
                error = new ErrorVM()
                {
                    field = "",
                    index = 0,
                    description = "Receipt Details Should containe Payment Details.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return 0;
            }
            #endregion

            #region Validation FinYear Payment
            cashPaidInFinYear = GetTillNowPaidOrderAmount(order.CustID, order.CompanyCode, order.BranchCode) + cashPaid;
            if (cashPaidInFinYear >= db.KSTU_TOLERANCE_MASTER.Where(ktm => ktm.obj_id == 500
                                                                        && ktm.company_code == order.CompanyCode
                                                                        && ktm.branch_code == order.BranchCode
                                                                    ).FirstOrDefault().Max_Val
                                                                        && (customer.pan_no == "" || customer.pan_no == null)) {
                if (order.PANNo == null || order.PANNo == "") {
                    error = new ErrorVM()
                    {
                        field = "",
                        index = 0,
                        description = "",
                        customDescription = "Please update PAN No to continue",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return 0;
                }
                else {
                    customer.pan_no = order.PANNo;
                    db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                }
            }
            #endregion

            #region Save Receipt Details
            //using (var transaction = db.Database.BeginTransaction()) {
            try {
                int receiptNo = 0;
                if (order.lstOfPayment != null) {
                    int paySlNo = 1;
                    string payGUID = SIGlobals.Globals.GetNewGUID();
                    foreach (PaymentVM pvm in order.lstOfPayment) {
                        receiptNo = Convert.ToInt32(SIGlobals.Globals.GetFinancialYear(db, order.CompanyCode, order.BranchCode).ToString().Remove(0, 1) + db.KSTS_SEQ_NOS.Where(sq => sq.obj_id == RECEIPT_SEQ_NO && sq.company_code == order.CompanyCode && sq.branch_code == order.BranchCode).FirstOrDefault().nextno);
                        KTTU_PAYMENT_DETAILS kpd = new KTTU_PAYMENT_DETAILS();
                        kpd.obj_id = payGUID;
                        kpd.company_code = pvm.CompanyCode;
                        kpd.branch_code = pvm.BranchCode;
                        kpd.series_no = order.OrderNo;
                        kpd.receipt_no = receiptNo;
                        kpd.sno = paySlNo;
                        kpd.trans_type = "O";
                        kpd.pay_mode = pvm.PayMode;
                        kpd.pay_details = pvm.PayDetails == null ? "" : pvm.PayDetails;
                        kpd.pay_date = SIGlobals.Globals.GetApplicationDate(order.CompanyCode, order.BranchCode);
                        kpd.pay_amt = pvm.PayAmount;
                        kpd.Ref_BillNo = pvm.RefBillNo == null ? "0" : pvm.RefBillNo;
                        kpd.party_code = pvm.PartyCode == null ? "" : pvm.PartyCode;
                        kpd.bill_counter = "FF";// pvm.BillCounter;
                        kpd.is_paid = pvm.IsPaid == null ? "N" : pvm.IsPaid;
                        kpd.bank = SIGlobals.Globals.GetBank(db, pvm.Bank, pvm.PayMode, pvm.CompanyCode, pvm.BranchCode);
                        kpd.bank_name = pvm.BankName == null ? "" : pvm.BankName;
                        kpd.cheque_date = pvm.ChequeDate == null ? SIGlobals.Globals.GetApplicationDate(order.CompanyCode, order.BranchCode) : pvm.ChequeDate;
                        kpd.card_type = pvm.CardType == null ? "" : pvm.CardType;
                        kpd.expiry_date = pvm.ExpiryDate == null ? SIGlobals.Globals.GetApplicationDate(order.CompanyCode, order.BranchCode) : pvm.ExpiryDate;
                        kpd.cflag = "N";
                        kpd.card_app_no = pvm.CardAppNo == null ? "" : pvm.CardAppNo;
                        kpd.scheme_code = pvm.SchemeCode == null ? "" : pvm.SchemeCode;
                        kpd.sal_bill_type = pvm.SalBillType;
                        kpd.operator_code = pvm.OperatorCode;
                        kpd.session_no = pvm.SessionNo == null ? 0 : pvm.SessionNo;
                        kpd.UpdateOn = SIGlobals.Globals.GetDateTime();
                        kpd.group_code = pvm.GroupCode;
                        kpd.amt_received = pvm.AmtReceived == null ? 0 : pvm.AmtReceived;
                        kpd.bonus_amt = pvm.BonusAmt;
                        kpd.win_amt = pvm.WinAmt;
                        kpd.ct_branch = pvm.CTBranch;
                        kpd.fin_year = finYear;
                        kpd.CardCharges = pvm.CardCharges == null ? 0 : pvm.CardCharges;
                        kpd.cheque_no = pvm.PayMode == "R" ? pvm.Bank : pvm.ChequeNo;
                        kpd.New_Bill_No = pvm.NewBillNo;
                        kpd.Add_disc = pvm.AddDisc == null ? 0 : pvm.AddDisc;
                        kpd.isOrdermanual = pvm.IsOrderManual == null ? "N" : pvm.IsOrderManual;
                        kpd.currency_value = pvm.CurrencyValue == null ? 0 : pvm.CurrencyValue;
                        kpd.exchange_rate = pvm.ExchangeRate == null ? 0 : pvm.ExchangeRate;
                        kpd.currency_type = pvm.CurrencyType == null ? "INR" : pvm.CurrencyType;
                        kpd.tax_percentage = pvm.TaxPercentage == null ? 0 : pvm.TaxPercentage;
                        kpd.cancelled_by = pvm.CancelledBy == null ? "" : pvm.CancelledBy;
                        kpd.cancelled_remarks = pvm.CancelledRemarks == null ? "" : pvm.CancelledRemarks;
                        kpd.cancelled_date = SIGlobals.Globals.GetApplicationDate(order.CompanyCode, order.BranchCode).ToString();
                        kpd.isExchange = pvm.IsExchange == null ? "N" : pvm.IsExchange;
                        kpd.exchangeNo = pvm.ExchangeNo == null ? 0 : pvm.ExchangeNo;
                        kpd.new_receipt_no = pvm.NewReceiptNo;
                        kpd.Gift_Amount = pvm.GiftAmount == null ? 0 : pvm.GiftAmount;
                        kpd.cardSwipedBy = pvm.CardSwipedBy == null ? "" : pvm.CardSwipedBy;
                        kpd.version = pvm.Version == null ? 0 : pvm.Version;
                        kpd.GSTGroupCode = pvm.GSTGroupCode;
                        kpd.SGST_Percent = pvm.SGSTPercent == null ? 0 : pvm.SGSTPercent;
                        kpd.CGST_Percent = pvm.CGSTPercent == null ? 0 : pvm.CGSTPercent;
                        kpd.IGST_Percent = pvm.IGSTPercent == null ? 0 : pvm.IGSTPercent;
                        kpd.HSN = pvm.HSN;
                        kpd.SGST_Amount = pvm.SGSTAmount == null ? 0 : pvm.SGSTAmount;
                        kpd.CGST_Amount = pvm.CGSTAmount == null ? 0 : pvm.CGSTAmount;
                        kpd.IGST_Amount = pvm.IGSTAmount == null ? 0 : pvm.IGSTAmount;
                        kpd.pay_amount_before_tax = pvm.PayAmountBeforeTax == null ? pvm.PayAmount : pvm.PayAmountBeforeTax;
                        kpd.pay_tax_amount = pvm.PayTaxAmount == null ? 0 : pvm.PayTaxAmount;
                        kpd.UniqRowID = Guid.NewGuid();
                        db.KTTU_PAYMENT_DETAILS.Add(kpd);
                        paySlNo = paySlNo + 1;
                    }
                    // Updating Order Receipt Sequence Number
                    SIGlobals.Globals.UpdateSeqenceNumber(db, RECEIPT_SEQ_NO, order.CompanyCode, order.BranchCode);
                    db.SaveChanges();

                    // Getting All payment details and Calculate Total Advance Payment.
                    List<KTTU_PAYMENT_DETAILS> lstOfPayment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == order.OrderNo
                                                                                                && pay.company_code == order.CompanyCode
                                                                                                && pay.branch_code == order.BranchCode
                                                                                                && pay.trans_type == "O"
                                                                                            ).ToList();
                    decimal totalAdvancePayment = 0;
                    foreach (KTTU_PAYMENT_DETAILS pay in lstOfPayment) {
                        totalAdvancePayment = Convert.ToDecimal(totalAdvancePayment + pay.pay_amt);
                    }

                    // Saving Total Advance Amount to Master Table.
                    KTTU_ORDER_MASTER kom = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == order.OrderNo
                                                                        && ord.company_code == order.CompanyCode
                                                                        && ord.branch_code == order.BranchCode
                                                                      ).FirstOrDefault();
                    kom.advance_ord_amount = totalAdvancePayment;
                    db.Entry(kom).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    // Account Posting by ReceiptNo then OrderNo should be pass as 0.
                    //ErrorVM accError = OrderAccountPostingWithProedure(0, receiptNo, order.CompanyCode, order.BranchCode);
                    //if (accError != null) {
                    //    throw new Exception(error.description);
                    //}
                    ErrorVM accError = OrderAccountPostingWithProedureNew(order.OrderNo, receiptNo, order.CompanyCode, order.BranchCode);
                    if (accError != null) {
                        throw new Exception(error.description);
                    }
                }
                db.SaveChanges();
                //transaction.Commit();
                return receiptNo;
            }
            catch (Exception excp) {
                //transaction.Rollback();
                error = new ErrorVM().GetErrorDetails(excp);
                return 0;
            }
            //}
            #endregion
        }

        public bool CancelOrder(OrderMasterVM order, out ErrorVM error)
        {
            error = null;
            KTTU_ORDER_MASTER kom = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == order.OrderNo
                                                                && ord.company_code == order.CompanyCode
                                                                && ord.branch_code == order.BranchCode
                                                               ).FirstOrDefault();
            if(kom == null) {
                error = new ErrorVM { description = "Order details are not found", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return false;
            }
            if(kom.cflag == "Y") {
                error = new ErrorVM { description = "Order is already cancelled.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return false;
            }
            if (kom.is_lock == "Y") {
                error = new ErrorVM { description = "Order is dormant.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return false;
            }
            if (kom.closed_flag == "Y") {
                error = new ErrorVM { description = "Order is already closed.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return false;
            }
            try {
                kom.cflag = "Y";
                kom.UpdateOn = SIGlobals.Globals.GetDateTime();
                db.Entry(kom).State = System.Data.Entity.EntityState.Modified;

                List<KTTU_PAYMENT_DETAILS> lstOfkpd = db.KTTU_PAYMENT_DETAILS.Where(kpdd => kpdd.series_no == order.OrderNo
                                                                                        && kpdd.trans_type == "O"
                                                                                        && kpdd.company_code == order.CompanyCode
                                                                                        && kpdd.branch_code == order.BranchCode
                                                                                   ).ToList();
                foreach (KTTU_PAYMENT_DETAILS kpd in lstOfkpd) {
                    kpd.cflag = "Y";
                    kpd.cancelled_date = SIGlobals.Globals.GetApplicationDate(order.CompanyCode, order.BranchCode).ToString();
                    db.Entry(kpd).State = System.Data.Entity.EntityState.Modified;

                    // Chit Handled
                    if (kpd.pay_mode == "CT") {
                        int chitMembershipNo = Convert.ToInt32(kpd.Ref_BillNo);
                        CHTU_CHIT_CLOSURE cc = db.CHTU_CHIT_CLOSURE.Where(ccc => ccc.branch_code == kpd.party_code
                                                                            && ccc.Scheme_Code == kpd.scheme_code
                                                                            && ccc.Group_Code == kpd.group_code
                                                                            && ccc.Chit_MembShipNo == chitMembershipNo).FirstOrDefault();
                        cc.Bill_Number = Convert.ToString("0");
                        db.Entry(cc).State = System.Data.Entity.EntityState.Modified;
                    }

                    //Account Posting
                    List<KSTU_ACC_VOUCHER_TRANSACTIONS> lstOfKAVT = db.KSTU_ACC_VOUCHER_TRANSACTIONS.Where(kavt => kavt.receipt_no == order.OrderNo + "," + kpd.receipt_no
                                                                                        && kavt.company_code == order.CompanyCode
                                                                                        && kavt.branch_code == order.BranchCode).ToList();
                    foreach (KSTU_ACC_VOUCHER_TRANSACTIONS kavt in lstOfKAVT) {
                        kavt.cflag = "Y";
                        db.Entry(kavt).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                db.SaveChanges();

                #region Marketplace Update
                bool updated = new Common.MarketplaceBL().UpdateMarketplaceInventory(order.CompanyCode,
                                                                                     order.BranchCode,
                                                                                     order.OrderNo,
                                                                                     TransactionsType.Order, ActionType.UnBlock, out error);
                error = null;
                #endregion

                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public bool CloseOrder(OrderMasterVM order, out ErrorVM error)
        {
            error = null;
            decimal totalPayAmount = 0;

            //Validation
            if (order == null) {
                error = new ErrorVM()
                {
                    field = "Order",
                    index = 0,
                    description = "Invalid Order Details.",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return false;
            }
            KTTU_ORDER_MASTER kom = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == order.OrderNo && ord.company_code == order.CompanyCode && ord.branch_code == ord.branch_code).FirstOrDefault();
            if (kom == null) {
                error = new ErrorVM()
                {
                    field = "Order",
                    index = 0,
                    description = "Invalid Order Number.",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return false;
            }
            if (order.lstOfPayment != null && order.lstOfPayment.Count > 0) {
                foreach (PaymentVM pvm in order.lstOfPayment) {
                    if (pvm.PayMode == "C") {
                        KSTU_TOLERANCE_MASTER ktm = db.KSTU_TOLERANCE_MASTER.Where(kt => kt.obj_id == 6
                                                                                    && kt.company_code == order.CompanyCode
                                                                                    && kt.branch_code == order.BranchCode).FirstOrDefault();
                        if (pvm.PayAmount > ktm.Max_Val) {
                            error = new ErrorVM()
                            {
                                field = "Amount",
                                index = 0,
                                description = string.Format("Cash should not be paid more than {0:0.00}/-", ktm.Max_Val),
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                            };
                            return false;
                        }
                    }
                    totalPayAmount = totalPayAmount + Convert.ToDecimal(pvm.PayAmount);
                }
            }

            decimal diffAmount = Convert.ToDecimal(kom.advance_ord_amount) - totalPayAmount;

            if (diffAmount < -1 || diffAmount > 1) {
                error = new ErrorVM()
                {
                    field = "Amount",
                    index = 0,
                    description = "Closure amount should be equal to Advance Amount.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            //if (totalPayAmount > kom.advance_ord_amount || totalPayAmount < kom.advance_ord_amount) {
            //    error = new ErrorVM() { field = "Amount", index = 0, description = "Closure amount is Less/More than the Advance Amount.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
            //    return false;
            //}
            // Transaction
            using (var transaction = db.Database.BeginTransaction()) {
                try {
                    kom.closed_flag = "Y";
                    kom.closed_date = SIGlobals.Globals.GetApplicationDate(order.CompanyCode, order.BranchCode);
                    db.Entry(kom).State = System.Data.Entity.EntityState.Modified;

                    if (order.lstOfPayment != null) {
                        int paySlNo = 1;
                        string payGUID = SIGlobals.Globals.GetNewGUID();
                        foreach (PaymentVM pvm in order.lstOfPayment) {
                            string bankName = string.Empty;
                            if (pvm.Bank != null) {
                                int bankCode = Convert.ToInt32(pvm.Bank);
                                bankName = db.KSTU_ACC_LEDGER_MASTER.Where(kalm => kalm.acc_code == bankCode && kalm.company_code == order.CompanyCode && kalm.branch_code == order.BranchCode).FirstOrDefault().acc_name;
                            }

                            KTTU_PAYMENT_DETAILS kpd = new KTTU_PAYMENT_DETAILS();
                            kpd.obj_id = payGUID;
                            kpd.company_code = pvm.CompanyCode;
                            kpd.branch_code = pvm.BranchCode;
                            kpd.series_no = kom.order_no;
                            kpd.receipt_no = 0;
                            kpd.sno = paySlNo;
                            kpd.trans_type = "CO";
                            kpd.pay_mode = pvm.PayMode;
                            kpd.pay_details = pvm.PayDetails;
                            kpd.pay_date = SIGlobals.Globals.GetApplicationDate(order.CompanyCode, order.BranchCode);
                            kpd.pay_amt = pvm.PayAmount;
                            kpd.Ref_BillNo = pvm.RefBillNo;
                            kpd.party_code = pvm.PartyCode;
                            kpd.bill_counter = pvm.BillCounter;
                            kpd.is_paid = pvm.IsPaid;
                            kpd.bank = pvm.BankName;
                            kpd.bank_name = bankName;
                            kpd.cheque_date = pvm.ChequeDate;
                            kpd.card_type = pvm.CardType;
                            kpd.expiry_date = pvm.ExpiryDate;
                            kpd.cflag = "N";
                            kpd.card_app_no = pvm.CardAppNo;
                            kpd.scheme_code = pvm.SchemeCode;
                            kpd.sal_bill_type = pvm.SalBillType;
                            kpd.operator_code = pvm.OperatorCode;
                            kpd.session_no = pvm.SessionNo;
                            kpd.UpdateOn = SIGlobals.Globals.GetDateTime();
                            kpd.group_code = pvm.GroupCode;
                            kpd.amt_received = pvm.AmtReceived;
                            kpd.bonus_amt = pvm.BonusAmt;
                            kpd.win_amt = pvm.WinAmt;
                            kpd.ct_branch = pvm.CTBranch;
                            kpd.fin_year = pvm.FinYear;
                            kpd.CardCharges = pvm.CardCharges;
                            kpd.cheque_no = pvm.ChequeNo;
                            kpd.New_Bill_No = pvm.NewBillNo;
                            kpd.Add_disc = pvm.AddDisc;
                            kpd.isOrdermanual = pvm.IsOrderManual;
                            kpd.currency_value = pvm.CurrencyValue;
                            kpd.exchange_rate = pvm.ExchangeRate;
                            kpd.currency_type = pvm.CurrencyType;
                            kpd.tax_percentage = pvm.TaxPercentage;
                            kpd.cancelled_by = pvm.CancelledBy;
                            kpd.cancelled_remarks = pvm.CancelledRemarks;
                            kpd.cancelled_date = pvm.CancelledDate;
                            kpd.isExchange = pvm.IsExchange;
                            kpd.exchangeNo = pvm.ExchangeNo;
                            kpd.new_receipt_no = pvm.NewReceiptNo;
                            kpd.Gift_Amount = pvm.GiftAmount;
                            kpd.cardSwipedBy = pvm.CardSwipedBy;
                            kpd.version = pvm.Version;
                            kpd.GSTGroupCode = pvm.GSTGroupCode;
                            kpd.SGST_Percent = pvm.SGSTPercent;
                            kpd.CGST_Percent = pvm.CGSTPercent;
                            kpd.IGST_Percent = pvm.IGSTPercent;
                            kpd.HSN = pvm.HSN;
                            kpd.SGST_Amount = pvm.SGSTAmount;
                            kpd.CGST_Amount = pvm.CGSTAmount;
                            kpd.IGST_Amount = pvm.IGSTAmount;
                            kpd.pay_amount_before_tax = pvm.PayAmountBeforeTax;
                            kpd.pay_tax_amount = pvm.PayTaxAmount;
                            kpd.UniqRowID = Guid.NewGuid();
                            db.KTTU_PAYMENT_DETAILS.Add(kpd);
                            paySlNo = paySlNo + 1;

                            //Update to Cheque Master
                            if (pvm.PayMode == "Q") {
                                int accCode = Convert.ToInt32(pvm.BankName);
                                int chqNo = Convert.ToInt32(pvm.ChequeNo);
                                KSTU_ACC_CHEQUE_MASTER kacm = db.KSTU_ACC_CHEQUE_MASTER.Where(kac => kac.acc_code == accCode
                                                                                                && kac.chq_no == chqNo
                                                                                                && kac.company_code == order.CompanyCode
                                                                                                && kac.branch_code == order.BranchCode
                                                                                              ).FirstOrDefault();
                                if(kacm == null) {
                                    error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = $"Cheque information for cheque No. {chqNo} and acc code {accCode} is not found." };
                                    transaction.Rollback();
                                    return false;
                                }
                                kacm.chq_issued = "Y";
                                db.Entry(kacm).State = System.Data.Entity.EntityState.Modified;
                            }
                        }
                    }
                    db.SaveChanges();
                    // Account Posting
                    ErrorVM accError = OrderClosureAccountPosting(order.OrderNo, order.CompanyCode, order.BranchCode);
                    if (accError != null) {
                        //throw new Exception(error.description);
                        error = accError;
                        return false;
                    }
                    db.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception excp) {
                    transaction.Rollback();
                    error = new ErrorVM().GetErrorDetails(excp);
                    return false;
                }
            }
        }

        public PaymentVM CancelReceiptGet(string companyCode, string branchCode, int receiptNo, out ErrorVM error)
        {
            error = null;
            try {
                List<KTTU_PAYMENT_DETAILS> kpd = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.receipt_no == receiptNo
                                                                               && ord.company_code == companyCode
                                                                               && ord.branch_code == branchCode).ToList();
                if (kpd == null || kpd.Count == 0) {
                    error = new ErrorVM()
                    {
                        field = "Receipt Number",
                        index = 0,
                        description = "Invalid receipt number",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                    return null;
                }
                int orderNo = kpd[0].series_no;
                decimal totalReceiptAmount = 0;
                DateTime receiptDate = DateTime.Now;
                KTTU_ORDER_MASTER komd = db.KTTU_ORDER_MASTER.Where(kom => kom.order_no == orderNo
                                                                        && kom.company_code == companyCode
                                                                        && kom.branch_code == branchCode).FirstOrDefault();

                if (komd.closed_flag == "Y") {
                    error = new ErrorVM()
                    {
                        field = "Receipt Number",
                        index = 0,
                        description = "Invalid receipt number",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                    return null;
                }

                foreach (KTTU_PAYMENT_DETAILS paymentDet in kpd) {
                    if (paymentDet.cflag == "Y") {
                        error = new ErrorVM()
                        {
                            field = "Receipt Number",
                            index = 0,
                            description = "Order Receipt No: " + receiptNo + " already cancelled.",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return null;
                    }
                    PaymentVM payment = new PaymentVM();
                    payment.PayAmount = paymentDet.pay_amt;
                    totalReceiptAmount = Convert.ToDecimal(totalReceiptAmount + paymentDet.pay_amt);
                    receiptDate = Convert.ToDateTime(paymentDet.pay_date);
                }
                return new PaymentVM() { SNo = komd.Cust_Id, PayAmount = totalReceiptAmount, PayDate = receiptDate, ReceiptNo = receiptNo };
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public bool OrderCancelReceipt(PaymentVM payment, out ErrorVM error)
        {
            error = null;
            try {
                decimal totalCancelledAmount = 0;
                List<KTTU_PAYMENT_DETAILS> lstOfkpd = db.KTTU_PAYMENT_DETAILS.Where(kpdd => kpdd.receipt_no == payment.ReceiptNo
                                                                                    && kpdd.company_code == payment.CompanyCode
                                                                                    && kpdd.branch_code == payment.BranchCode
                                                                                    && kpdd.trans_type == "O"
                                                                                    && kpdd.cflag != "Y"
                                                                                   ).ToList();
                if (lstOfkpd == null || lstOfkpd.Count == 0) {
                    error = new ErrorVM()
                    {
                        field = "Order Receipt",
                        index = 0,
                        description = "The order receipt details are not found or it is already cancelled.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }

                int orderNo = lstOfkpd[0].series_no;
                var originalOrderAmount = Convert.ToDecimal(db.KTTU_PAYMENT_DETAILS
                    .Where(x => x.series_no == orderNo
                        && x.company_code == payment.CompanyCode
                        && x.branch_code == payment.BranchCode
                        && x.trans_type == "O"
                        && x.cflag != "Y"
                        ).Sum(s => (s.pay_amt)));

                foreach (KTTU_PAYMENT_DETAILS kpd in lstOfkpd) {
                    kpd.cflag = "Y";
                    kpd.cancelled_date = SIGlobals.Globals.GetDateTime().ToString();
                    kpd.cancelled_remarks = payment.CancelledRemarks;
                    totalCancelledAmount += Convert.ToDecimal(kpd.pay_amt);
                    db.Entry(kpd).State = System.Data.Entity.EntityState.Modified;

                    // Chit Handled
                    if (kpd.pay_mode == "CT") {
                        int chitMembershipNo = Convert.ToInt32(kpd.Ref_BillNo);
                        CHTU_CHIT_CLOSURE cc = db.CHTU_CHIT_CLOSURE.Where(ccc => ccc.branch_code == kpd.party_code
                                                                            && ccc.Scheme_Code == kpd.scheme_code
                                                                            && ccc.Group_Code == kpd.group_code
                                                                            && ccc.Chit_MembShipNo == chitMembershipNo).FirstOrDefault();
                        cc.Bill_Number = Convert.ToString("0");
                        db.Entry(cc).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                // Updating the Order Amount to Master by Minus the cancelled receipt Amount.

                KTTU_ORDER_MASTER kom = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == orderNo
                                                                    && ord.company_code == payment.CompanyCode
                                                                    && ord.branch_code == payment.BranchCode
                                                                    ).FirstOrDefault();
                if(kom == null) {
                    error = new ErrorVM { description = "The order details does not exist.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return false;
                }
                if (kom.cflag == "Y") {
                    error = new ErrorVM { description = "Order is already cancelled.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return false;
                }
                if (kom.is_lock == "Y") {
                    error = new ErrorVM { description = "Order is dormant.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return false;
                }
                if (kom.closed_flag == "Y") {
                    error = new ErrorVM { description = "Order is already closed.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return false;
                }

                kom.advance_ord_amount = originalOrderAmount - totalCancelledAmount;
                db.Entry(kom).State = System.Data.Entity.EntityState.Modified;

                // Incase If the Order having only one receipt and we cancelled that receipt then we should cancel that order.
                List<KTTU_PAYMENT_DETAILS> lstOfkpdOriginal = db.KTTU_PAYMENT_DETAILS.Where(kpdd => kpdd.series_no == orderNo
                                                                                            && kpdd.company_code == payment.CompanyCode
                                                                                            && kpdd.branch_code == payment.BranchCode
                                                                                            && kpdd.trans_type == "O"
                                                                                            && kpdd.cflag != "Y").ToList();
                if (lstOfkpdOriginal.Select(kpd => kpd.receipt_no).Distinct().Count() == 1) {
                    kom.cflag = "Y";
                    kom.UpdateOn = SIGlobals.Globals.GetDateTime();
                    db.Entry(kom).State = System.Data.Entity.EntityState.Modified;
                }

                //Account Posting
                List<KSTU_ACC_VOUCHER_TRANSACTIONS> lstOfKAVT = db.KSTU_ACC_VOUCHER_TRANSACTIONS.Where(kavt => kavt.receipt_no == orderNo + "," + payment.ReceiptNo
                                                                                            && kavt.company_code == payment.CompanyCode
                                                                                            && kavt.branch_code == payment.BranchCode).ToList();
                foreach (KSTU_ACC_VOUCHER_TRANSACTIONS kavt in lstOfKAVT) {
                    kavt.cflag = "Y";
                    db.Entry(kavt).State = System.Data.Entity.EntityState.Modified;
                }

                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public bool ClosedOrderInOtherBranch(OrderMasterVM order, out ErrorVM error)
        {
            error = null;
            try {
                KTTU_ORDER_MASTER kom = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == order.OrderNo
                                                                    && ord.company_code == order.CompanyCode
                                                                    && ord.branch_code == order.BranchCode
                                                                   ).FirstOrDefault();
                kom.closed_flag = "Y";
                kom.closed_date = SIGlobals.Globals.GetDateTime();
                kom.bill_no = order.BillNo;
                kom.closed_branch = order.ClosedBranch;
                db.Entry(kom).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public bool UpdatOrderImageURL(string companyCode, string branchCode, int orderNo, int slNo, string imageName, out ErrorVM error)
        {
            error = null;
            try {
                KTTU_ORDER_DETAILS details = db.KTTU_ORDER_DETAILS.Where(det => det.company_code == companyCode
                                                                            && det.branch_code == branchCode && det.order_no == orderNo
                                                                            && det.slno == slNo).FirstOrDefault();
                details.img_id = imageName;
                db.Entry(details).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public string SendImagePath(string companyCode, string branchCode, int orderNo, int slNo)
        {
            KTTU_ORDER_DETAILS details = db.KTTU_ORDER_DETAILS.Where(det => det.company_code == companyCode
                                                                            && det.branch_code == branchCode && det.order_no == orderNo
                                                                            && det.slno == slNo).FirstOrDefault();
            if (details != null) {
                return details.img_id;
            }
            else {
                return "/NoImage.jpg";
            }
        }

        public bool GetValidateAttachOrderWithGSandKaratWithEstNo(string companyCode, string branchCode, int estNo, int orderNo, out ErrorVM error, out List<SalesEstDetailsVM> salesDet)
        {
            error = null;
            salesDet = null;
            List<SalesEstDetailsVM> barcodeDet = new List<SalesEstDetailsVM>();
            try {
                KTTU_SALES_EST_MASTER estMaster = db.KTTU_SALES_EST_MASTER.Where(master => master.company_code == companyCode
                                                                                    && master.branch_code == branchCode
                                                                                    && master.est_no == estNo).FirstOrDefault();
                List<KTTU_SALES_EST_DETAILS> estDet = db.KTTU_SALES_EST_DETAILS.Where(est => est.company_code == companyCode
                                                                                    && est.branch_code == branchCode
                                                                                    && est.est_no == estNo).ToList();
                KTTU_ORDER_MASTER orderMaster = db.KTTU_ORDER_MASTER.Where(ord => ord.company_code == companyCode
                                                                                    && ord.branch_code == branchCode
                                                                                    && ord.order_no == orderNo).FirstOrDefault();
                string gsCode = estDet[0].gs_code;
                KSTS_GS_ITEM_ENTRY ItemEntry = db.KSTS_GS_ITEM_ENTRY.Where(item => item.company_code == companyCode
                                                                      && item.branch_code == branchCode
                                                                      && item.gs_code == gsCode
                                                                      && item.bill_type == "S").FirstOrDefault();

                if (estDet == null) {
                    error = new ErrorVM()
                    {
                        description = "Invalid Estimation No.",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                    return false;
                }
                if (orderMaster == null) {
                    error = new ErrorVM()
                    {
                        description = "Invalid Order No.",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                    return false;
                }

                //Different Metal Validation for Barcoded Item
                if (ItemEntry.metal_type != new SalesEstimationBL().GetMetalTypeFromGS(orderMaster.gs_code, companyCode, branchCode)) {
                    error = new ErrorVM()
                    {
                        description = "Different Metal Cannot be billed together",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                    return false;
                }

                //This block handled both GSCode and Karat - commented because not validating in Magna.
                //if (estDet[0].gs_code != orderMaster.gs_code && estDet[0].karat != orderMaster.karat) {
                //    error = new ErrorVM()
                //    {
                //        description = "Different Metal Cannot be billed together",
                //        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                //    };
                //    return false;
                //}

                if (orderMaster.order_type == "R") {
                    List<KTTU_ORDER_DETAILS> orderDet = db.KTTU_ORDER_DETAILS.Where(ord => ord.order_no == orderNo
                                                                                   && ord.company_code == companyCode
                                                                                   && ord.branch_code == branchCode).ToList();
                    ErrorVM orderError = new ErrorVM();
                    foreach (KTTU_ORDER_DETAILS od in orderDet) {
                        barcodeDet.Add(new BarcodeBL().GetBarcodeWithStone(companyCode, branchCode, od.item_name, orderNo.ToString(), 0, 0, 0, out orderError));
                        if (orderError != null) {
                            error = orderError;
                            return false;
                        }

                    }
                    salesDet = barcodeDet;
                }
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public dynamic GetReceiptDetailsByOrderNo(string companyCode, string branchCode, int orderNo, out ErrorVM error)
        {
            error = null;
            OrderInfoVM receiptDet = new OrderInfoVM();
            receiptDet.ReceiptNo = new List<int>();
            try {

                List<int> data = (from d in db.KTTU_PAYMENT_DETAILS
                                  where d.company_code == companyCode && d.branch_code == branchCode && d.series_no == orderNo && d.cflag == "N" && d.trans_type == "O"
                                  select new { ReceiptNo = d.receipt_no }).Select(p => p.ReceiptNo).ToList();
                receiptDet.ReceiptNo = data;
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }
        #endregion

        #region Order Search
        public List<SearchParamVM> GetOrderDetailsSearchParams(string companyCode, string branchCode)
        {
            List<SearchParamVM> lstSearchParams = new List<SearchParamVM>();
            lstSearchParams.Add(new SearchParamVM() { Value = "Order No", Key = "ORDERNO" });
            lstSearchParams.Add(new SearchParamVM() { Value = "Name", Key = "NAME" });
            lstSearchParams.Add(new SearchParamVM() { Value = "Mobile No", Key = "MOBILENO" });
            lstSearchParams.Add(new SearchParamVM() { Value = "PAN", Key = "PAN" });
            return lstSearchParams;
        }
        public List<KTTU_ORDER_MASTER> GetOrderDetailsBySearchParameters(string companyCode, string branchCode, string searchBy, string searchParam)
        {
            List<KTTU_ORDER_MASTER> lstOrderMaster = new List<KTTU_ORDER_MASTER>();
            switch (searchBy.ToUpper()) {
                case "ORDERNO":
                    int orderNo = Convert.ToInt32(searchParam);
                    lstOrderMaster = db.KTTU_ORDER_MASTER.Where(search => search.order_no == orderNo
                                                                && search.branch_code == branchCode
                                                                && search.company_code == companyCode
                                                                ).ToList();
                    break;
                case "NAME":
                    lstOrderMaster = db.KTTU_ORDER_MASTER.Where(search => search.cust_name.Contains(searchParam)
                                                                && search.branch_code == branchCode
                                                                && search.company_code == companyCode).Take(1000).ToList();
                    break;
                case "MOBILENO":
                    lstOrderMaster = db.KTTU_ORDER_MASTER.Where(search => search.mobile_no == searchParam
                                                                && search.branch_code == branchCode
                                                                && search.company_code == companyCode).ToList();
                    break;
                case "PAN":
                    lstOrderMaster = db.KTTU_ORDER_MASTER.Where(search => search.pan_no == searchParam
                                                                && search.branch_code == branchCode
                                                                && search.company_code == companyCode).ToList();
                    break;
            }
            return lstOrderMaster;
        }
        #endregion

        #region Order Attachment
        public IQueryable<AllOrdersVM> GetAllOrder(string companyCode, string branchCode)
        {
            #region Old Code
            //var data = SIGlobals.Globals.ExecuteQuery("SELECT m.order_no AS[Order No]," +
            //                                "       New_bill_no AS[Ref No], " +
            //                                "       m.cust_name AS[Customer], " +
            //                                "       CONVERT(VARCHAR, m.order_date, 103) AS[Date], " +
            //                                "       m.advance_ord_amount AS[Amount], " +
            //                                "       rate AS[Rate / Gram], " +
            //                                "       order_rate_type AS[Type], " +
            //                                "       m.cust_id AS[ID], " +
            //                                "       m.sal_code AS[Staff], " +
            //                                "       karat, " +
            //                                " (" +
            //                                "    SELECT isnull(SUM(from_gwt), 0)" +
            //                                "   FROM KTTU_ORDER_DETAILS d" +
            //                                "    WHERE d.order_no = m.order_no" +
            //                                "          AND d.company_code = m.company_code" +
            //                                "          AND d.branch_code = m.branch_code" +
            //                                " ) AS fixed_wt" +
            //                                " FROM KTTU_ORDER_MASTER m " +
            //                                " WHERE m.order_no NOT IN" +
            //                                " (" +
            //                                    "SELECT Ref_BillNo" +
            //                                "    FROM KTTU_PAYMENT_DETAILS " +
            //                                "    WHERE pay_mode = 'OP'" +
            //                                          "AND TRANS_TYPE = 'A'" +
            //                                          "AND cflag != 'Y'" +
            //                                " )" +
            //                                "      AND m.bill_no = 0" +
            //                                "      AND m.cflag != 'Y'" +
            //                                "      AND m.closed_flag != 'Y'" +
            //                                "      AND m.company_code='" + companyCode + "'" +
            //                                "      AND m.branch_code='" + branchCode + "'" +
            //                                " ORDER BY m.order_no DESC;");
            //List<AllOrdersVM> lstAllOrderVM = new List<AllOrdersVM>();
            //if (data != null) {
            //    for (int i = 0; i < data.Rows.Count; i++) {
            //        AllOrdersVM order = new AllOrdersVM();
            //        order.OrderNo = Convert.ToInt32(data.Rows[i]["Order No"]);
            //        order.Customer = Convert.ToString(data.Rows[i]["Customer"]);
            //        order.Date = Convert.ToString(data.Rows[i]["Date"]);
            //        order.Amount = Convert.ToDecimal(data.Rows[i]["Amount"]);
            //        order.GoldRate = Convert.ToDecimal(data.Rows[i]["Rate / Gram"]);
            //        order.Type = Convert.ToString(data.Rows[i]["Type"]);
            //        order.CustomerID = Convert.ToInt32(data.Rows[i]["ID"]);
            //        order.Staff = Convert.ToString(data.Rows[i]["Staff"]);
            //        order.Karat = Convert.ToString(data.Rows[i]["karat"]);
            //        lstAllOrderVM.Add(order);
            //    }
            //}

            //List<AllOrdersVM> lstAllOrderVM = new List<AllOrdersVM>();
            //var data = db.usp_getAllOrdersForAttach(companyCode, branchCode);

            //if (data != null) {
            //    foreach (usp_getAllOrdersForAttach_Result result in data) {
            //        AllOrdersVM order = new AllOrdersVM();
            //        order.OrderNo = Convert.ToInt32(result.Order_No);
            //        order.Customer = Convert.ToString(result.Customer);
            //        order.Date = Convert.ToString(result.Date);
            //        order.Amount = Convert.ToDecimal(result.Amount);
            //        order.GoldRate = Convert.ToDecimal(result.Rate___Gram);
            //        order.Type = Convert.ToString(result.Type);
            //        order.CustomerID = Convert.ToInt32(result.ID);
            //        order.Staff = Convert.ToString(result.Staff);
            //        order.Karat = Convert.ToString(result.karat);
            //        lstAllOrderVM.Add(order);
            //    }
            //} 
            #endregion

            db.Configuration.UseDatabaseNullSemantics = true;
            var orderDetails = from od in db.KTTU_ORDER_DETAILS
                               group new { od } by new
                               {
                                   CompanyCode = od.company_code,
                                   BranchCode = od.branch_code,
                                   OrderNo = od.order_no
                               } into g
                               select new
                               {
                                   CompanyCode = g.Key.CompanyCode,
                                   BranchCode = g.Key.BranchCode,
                                   OrderNo = g.Key.OrderNo,
                                   Weight = g.Sum(e => e.od.from_gwt)
                               };
            var paymentDetails = from p in db.KTTU_PAYMENT_DETAILS
                                 where p.company_code == companyCode && p.branch_code == branchCode && p.pay_mode == "OP"
                                    && p.trans_type == "A" && p.cflag != "Y"
                                 select new
                                 {
                                     OrderKey = p.company_code + "$" + p.branch_code + "$" + p.Ref_BillNo.ToString()
                                 };

            var orderList = (from om in db.KTTU_ORDER_MASTER
                             join od in orderDetails
                             on new { CompanyCode = om.company_code, BranchCode = om.branch_code, OrderNo = om.order_no } equals
                             new { CompanyCode = od.CompanyCode, BranchCode = od.BranchCode, OrderNo = od.OrderNo }
                             where om.company_code == companyCode && om.branch_code == branchCode
                                && om.cflag != "Y" && om.bill_no == 0 && om.closed_flag != "Y"
                                && (om.is_lock != "Y" || om.is_lock == null)
                                && !paymentDetails.Any(p => p.OrderKey == om.company_code + "$" + om.branch_code + "$" + om.order_no.ToString())
                             orderby om.order_no descending
                             select new AllOrdersVM
                             {
                                 OrderNo = om.order_no,
                                 Customer = om.cust_name,
                                 RefNo = om.New_Bill_No,
                                 Amount = om.advance_ord_amount,
                                 Date = om.order_date,
                                 Type = om.order_rate_type,
                                 GoldRate = om.rate,
                                 CustomerID = om.Cust_Id,
                                 Staff = om.sal_code,
                                 Karat = om.karat,
                                 FixedWt = od.Weight
                             });

            return orderList;
        }

        public IQueryable<AllOrdersVM> GetSearchOrder(string companyCode, string branchCode, string searchType, string searchValue)
        {
            List<AllOrdersVM> lstAllOrderVM = GetAllOrder(companyCode, branchCode).ToList();
            switch (searchType.ToUpper()) {
                case "ORDERNO":
                    return lstAllOrderVM.Where(search => search.OrderNo == Convert.ToInt32(searchValue)).AsQueryable<AllOrdersVM>();
                case "CUSTOMER":
                    return lstAllOrderVM.Where(search => search.Customer.Contains(searchValue)).AsQueryable<AllOrdersVM>();
                case "Date":
                    return lstAllOrderVM.Where(search => search.Date == Convert.ToDateTime(searchValue)).AsQueryable<AllOrdersVM>();
                case "RATE":
                    return lstAllOrderVM.Where(search => search.GoldRate == Convert.ToDecimal(searchValue)).AsQueryable<AllOrdersVM>();
                case "TYPE":
                    return lstAllOrderVM.Where(search => search.Type == Convert.ToString(searchValue)).AsQueryable<AllOrdersVM>();
                case "ID":
                    return lstAllOrderVM.Where(search => search.CustomerID == Convert.ToInt32(searchValue)).AsQueryable<AllOrdersVM>();
                case "STAFF":
                    return lstAllOrderVM.Where(search => search.Staff == Convert.ToString(searchValue)).AsQueryable<AllOrdersVM>();
                case "KARAT":
                    return lstAllOrderVM.Where(search => search.Karat == Convert.ToString(searchValue)).AsQueryable<AllOrdersVM>();
                case "FIXEDWT":
                    return lstAllOrderVM.Where(search => search.FixedWt == Convert.ToDecimal(searchValue)).AsQueryable<AllOrdersVM>();
            }
            return lstAllOrderVM.AsQueryable<AllOrdersVM>();
        }

        public List<OrderMasterVM> GetAttachedOrder(string companyCode, string branchCode, int estNo)
        {
            List<KTTU_PAYMENT_DETAILS> payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == estNo
                                                                                && pay.pay_mode == "OP"
                                                                                && pay.trans_type == "A"
                                                                                && pay.branch_code == companyCode
                                                                                && pay.company_code == branchCode).ToList();
            List<OrderMasterVM> lstOfOrderMaster = new List<OrderMasterVM>();
            foreach (KTTU_PAYMENT_DETAILS pay in payment) {
                OrderMasterVM om = new OrderMasterVM();
                ErrorVM error = new ErrorVM();
                om = GetOrderInfo(companyCode, branchCode, Convert.ToInt32(pay.Ref_BillNo), out error);
                lstOfOrderMaster.Add(om);
            }
            return lstOfOrderMaster;
        }

        public bool SaveAttachedOrder(List<PaymentVM> payment, out ErrorVM error, out List<SalesEstDetailsVM> salesDet)
        {
            List<SalesEstDetailsVM> barcodeDet = new List<SalesEstDetailsVM>();
            error = null;
            salesDet = null;

            if (payment == null) {
                error = new ErrorVM() { description = "Invalid payment details.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return false;
            }

            #region Validation
            int estNo = payment[0].SeriesNo;
            string companyCode = payment[0].CompanyCode;
            string branchCode = payment[0].BranchCode;

            if (estNo == 0) {
                error = new ErrorVM()
                {
                    description = "Invalid Estimation Number.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            #endregion
            try {
                #region Validate Underlying Sales Estimation
                KTTU_SALES_EST_MASTER salEstimation = db.KTTU_SALES_EST_MASTER.Where(est => est.est_no == estNo
                                                                                        && est.company_code == companyCode
                                                                                        && est.branch_code == branchCode).FirstOrDefault();
                if (salEstimation == null) {
                    error = new ErrorVM() { description = "The underlying sales estimation does not exist.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return false;
                }
                if (salEstimation.bill_no != 0) {
                    error = new ErrorVM() { description = "The Sales Estimation is billed already. The bill number is :" + salEstimation.bill_no.ToString(), ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return false;
                }
                #endregion

                #region Check if the orders in the list are attached globally to the same estimate.
                foreach (PaymentVM pay in payment.OrderBy(ord => ord.SNo)) {
                    int OrderNo = Convert.ToInt32(pay.RefBillNo);
                    if (salEstimation.order_no == OrderNo) {
                        error = new ErrorVM()
                        {
                            description = string.Format("Cannot attach This Order {0}, Becuase already this order attached Globally.", salEstimation.order_no),
                            ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError
                        };
                        return false;
                    }
                }
                #endregion

                #region Remove Existing order payments and add again.
                List<KTTU_PAYMENT_DETAILS> paymentDel = db.KTTU_PAYMENT_DETAILS.Where(est => est.series_no == estNo
                           && est.pay_mode == "OP" && est.trans_type == "A" && est.branch_code == branchCode
                           && est.company_code == companyCode).ToList();

                foreach (KTTU_PAYMENT_DETAILS pay in paymentDel) {
                    db.KTTU_PAYMENT_DETAILS.Remove(pay);
                }
                #endregion

                #region Get the Serial No. of the payments
                var payDetailSlNo = db.KTTU_PAYMENT_DETAILS.Where(pd => pd.series_no == estNo
                                                                    && pd.trans_type == "A"
                                                                    && pd.company_code == companyCode
                                                                    && pd.branch_code == branchCode)
                                                                    .Select(x => x.sno).DefaultIfEmpty(0).Max();
                int paySlNo = 1;
                if (payDetailSlNo > 0) {
                    paySlNo = payDetailSlNo + 1;
                }
                #endregion

                #region Saving
                string objID = salEstimation.obj_id;
                foreach (PaymentVM pay in payment.OrderBy(ord => ord.SNo)) {
                    int OrderNo = Convert.ToInt32(pay.RefBillNo);
                    KTTU_ORDER_MASTER order = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == OrderNo
                                                                            && ord.company_code == companyCode
                                                                            && ord.branch_code == branchCode).FirstOrDefault();
                    if (order == null) {
                        error = new ErrorVM()
                        {
                            description = "Invalid Order " + OrderNo.ToString() + ". The Order does not exist.",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                    if (order.bill_no != 0) {
                        error = new ErrorVM()
                        {
                            description = "The Order No. " + OrderNo.ToString() + " is already billed. The bill number is :" + order.bill_no.ToString(),
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }

                    if (order.order_type == "R") {
                        List<KTTU_ORDER_DETAILS> orderDet = db.KTTU_ORDER_DETAILS.Where(ord => ord.order_no == OrderNo
                                                                                       && ord.company_code == companyCode
                                                                                       && ord.branch_code == branchCode).ToList();
                        ErrorVM orderError = new ErrorVM();
                        foreach (KTTU_ORDER_DETAILS od in orderDet) {
                            barcodeDet.Add(new BarcodeBL().GetBarcodeWithStone(companyCode, branchCode, od.item_name, OrderNo.ToString(), 0, 0, 0, out orderError));
                            if (orderError != null) {
                                error = orderError;
                                return false;
                            }

                        }
                        salesDet = barcodeDet;
                    }
                    KTTU_PAYMENT_DETAILS kpd = new KTTU_PAYMENT_DETAILS();
                    kpd.obj_id = objID;
                    kpd.company_code = pay.CompanyCode;
                    kpd.branch_code = pay.BranchCode;
                    kpd.series_no = pay.SeriesNo; // Estimation Number
                    kpd.receipt_no = 0;
                    kpd.sno = paySlNo;
                    kpd.trans_type = "A";
                    kpd.pay_mode = "OP";
                    kpd.pay_details = "";
                    kpd.pay_date = order.order_date;
                    kpd.pay_amt = order.advance_ord_amount;
                    kpd.Ref_BillNo = pay.RefBillNo; // Order Number
                    kpd.party_code = null;
                    kpd.bill_counter = null;
                    kpd.is_paid = "Y";
                    kpd.bank = "";
                    kpd.bank_name = "";
                    kpd.cheque_date = order.order_date;
                    kpd.card_type = "";
                    kpd.expiry_date = pay.ExpiryDate;
                    kpd.cflag = "N";
                    kpd.card_app_no = "";
                    kpd.scheme_code = null;
                    kpd.sal_bill_type = null;
                    kpd.operator_code = pay.OperatorCode; // Need to send from JSON
                    kpd.session_no = 0;
                    kpd.UpdateOn = SIGlobals.Globals.GetDateTime();
                    kpd.group_code = null;
                    kpd.amt_received = 0;
                    kpd.bonus_amt = 0;
                    kpd.win_amt = 0;
                    kpd.ct_branch = null;
                    kpd.fin_year = 0;
                    kpd.CardCharges = 0;
                    kpd.cheque_no = "0";
                    kpd.New_Bill_No = pay.SeriesNo.ToString();
                    kpd.Add_disc = Convert.ToDecimal(0.00);
                    kpd.isOrdermanual = "N";
                    kpd.currency_value = Convert.ToDecimal(0.00);
                    kpd.exchange_rate = Convert.ToDecimal(0.00);
                    kpd.currency_type = null;
                    kpd.tax_percentage = Convert.ToDecimal(0.00);
                    kpd.cancelled_by = "";
                    kpd.cancelled_remarks = "";
                    kpd.cancelled_date = DateTime.MinValue.ToString();
                    kpd.isExchange = "N";
                    kpd.exchangeNo = 0;
                    kpd.new_receipt_no = "0";
                    kpd.Gift_Amount = Convert.ToDecimal(0.00);
                    kpd.cardSwipedBy = "";
                    kpd.version = 0;
                    kpd.GSTGroupCode = null;
                    kpd.SGST_Percent = Convert.ToDecimal(0.00);
                    kpd.CGST_Percent = Convert.ToDecimal(0.00);
                    kpd.IGST_Percent = Convert.ToDecimal(0.00);
                    kpd.HSN = null;
                    kpd.SGST_Amount = Convert.ToDecimal(0.00);
                    kpd.CGST_Amount = Convert.ToDecimal(0.00);
                    kpd.IGST_Amount = Convert.ToDecimal(0.00);
                    kpd.pay_amount_before_tax = order.advance_ord_amount;
                    kpd.pay_tax_amount = Convert.ToDecimal(0.00);
                    kpd.UniqRowID = Guid.NewGuid();
                    db.KTTU_PAYMENT_DETAILS.Add(kpd);
                    db.SaveChanges();
                    //Updating CustomerID to Estimation
                    if (paySlNo == 1) {
                        salEstimation.Cust_Id = order.Cust_Id;
                    }
                    paySlNo = paySlNo + 1;
                    salEstimation.UpdateOn = SIGlobals.Globals.GetDateTime();
                    db.Entry(salEstimation).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
                #endregion

                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public bool RemoveOrderAttachement(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            if (estNo == 0) {
                error = new ErrorVM()
                {
                    description = "Invalid Estimation Number",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return false;
            }
            try {
                List<KTTU_PAYMENT_DETAILS> paymentDel = db.KTTU_PAYMENT_DETAILS.Where(est => est.series_no == estNo
                                                                                        && est.pay_mode == "OP"
                                                                                        && est.trans_type == "A"
                                                                                        && est.company_code == companyCode
                                                                                        && est.branch_code == branchCode).ToList();
                foreach (KTTU_PAYMENT_DETAILS pay in paymentDel) {
                    db.KTTU_PAYMENT_DETAILS.Remove(pay);
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public decimal GetGlobalAttachedOrderAmount(string companyCode, string branchCode, int estNo)
        {
            decimal globalOrderAmount = 0;
            KTTU_SALES_EST_MASTER salesDet = db.KTTU_SALES_EST_MASTER.Where(est => est.company_code == companyCode
                                                                            && est.branch_code == branchCode
                                                                            && est.est_no == estNo).FirstOrDefault();
            globalOrderAmount = salesDet == null ? 0 : salesDet.order_amount;
            return globalOrderAmount;
        }
        #endregion

        #region Print Orders

        #region Methods For UI Printing.
        public List<KTTU_ORDER_MASTER> GetOrderDetails(string companyCode, string branchCode, DateTime date, bool isCancelled)
        {
            string cFlag = isCancelled == true ? "Y" : "N";
            DateTime dateTime = Convert.ToDateTime(date);
            List<KTTU_ORDER_MASTER> lstOfOrders = db.KTTU_ORDER_MASTER.Where(ord => System.Data.Entity.DbFunctions.TruncateTime(ord.order_date) == System.Data.Entity.DbFunctions.TruncateTime(dateTime)
                                                    && ord.cflag == cFlag
                                                    && ord.company_code == companyCode
                                                    && ord.branch_code == branchCode).OrderByDescending(ord => ord.order_no).ToList();
            return lstOfOrders;
        }

        public dynamic GetReceiptDetailsForPrint(string companyCode, string branchCode, DateTime date, bool isCancelled)
        {
            string cFlag = isCancelled == true ? "Y" : "N";
            DateTime dateTime = Convert.ToDateTime(date);
            var result = (from kom in db.KTTU_ORDER_MASTER
                          join kpd in db.KTTU_PAYMENT_DETAILS //on kom.order_no equals kpd.series_no
                          on new { CompanyCode = kom.company_code, BranchCode = kom.branch_code, OrderNo = kom.order_no }
                          equals new { CompanyCode = kpd.company_code, BranchCode = kpd.branch_code, OrderNo = kpd.series_no }
                          where kpd.cflag == cFlag //kom.cflag == cFlag && kpd.cflag == cFlag
                          && kom.bill_no == 0
                          && kpd.trans_type == "O"
                          && System.Data.Entity.DbFunctions.TruncateTime(kpd.pay_date) == System.Data.Entity.DbFunctions.TruncateTime(date)
                          && kom.company_code == companyCode
                          && kom.branch_code == branchCode
                          group kom by new { kpd.receipt_no, kom.cust_name } into g
                          select new
                          {
                              ReceiptNo = g.Key.receipt_no,
                              CustomerName = g.Key.cust_name
                          }).OrderByDescending(x => x.ReceiptNo);
            return result;
        }

        public List<KTTU_ORDER_MASTER> GetClosedOrderDetails(string companyCode, string branchCode, DateTime date)
        {
            DateTime dateTime = Convert.ToDateTime(date);
            List<KTTU_ORDER_MASTER> lstOfOrders = db.KTTU_ORDER_MASTER.Where(ord => System.Data.Entity.DbFunctions.TruncateTime(ord.closed_date) == System.Data.Entity.DbFunctions.TruncateTime(dateTime)
            && ord.closed_flag == "Y"
            && ord.cflag != "Y"
            && ord.company_code == companyCode
            && ord.branch_code == branchCode).OrderByDescending(ord => ord.order_no).ToList();
            return lstOfOrders;
        }

        public OrderMasterVM GetOrderInfoForPrint(string companyCode, string branchCode, int orderNo, out ErrorVM error)
        {
            error = null;
            OrderMasterVM om = new OrderMasterVM();
            KTTU_ORDER_MASTER kom = new KTTU_ORDER_MASTER();
            List<KTTU_ORDER_DETAILS> kod = new List<KTTU_ORDER_DETAILS>();
            List<KTTU_PAYMENT_DETAILS> kpd = new List<KTTU_PAYMENT_DETAILS>();

            try {
                kom = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == orderNo
                                                    && ord.branch_code == branchCode
                                                    && ord.company_code == companyCode).FirstOrDefault();
                kod = db.KTTU_ORDER_DETAILS.Where(ord => ord.order_no == orderNo
                                                    && ord.branch_code == branchCode
                                                    && ord.company_code == companyCode).ToList();

                if (kom == null) {
                    error = new ErrorVM()
                    {
                        field = "Order Number",
                        index = 0,
                        description = "Invalid order number",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }

                if (kom.closed_flag == "Y") {
                    kpd = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.series_no == orderNo
                                                            && ord.trans_type == "CO"
                                                            && ord.company_code == companyCode
                                                            && ord.branch_code == branchCode).ToList().OrderBy(ord => ord.pay_date).ToList();
                }
                else {
                    kpd = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.series_no == orderNo
                                                            && ord.trans_type == "O"
                                                            && ord.cflag == "N"
                                                            && ord.company_code == companyCode
                                                            && ord.branch_code == branchCode).ToList();
                }

                KSTU_CUSTOMER_ID_PROOF_DETAILS customerIDProof = db.KSTU_CUSTOMER_ID_PROOF_DETAILS.Where(cust => cust.cust_id == kom.Cust_Id && cust.company_code == companyCode && cust.branch_code == branchCode).FirstOrDefault();
                if (customerIDProof != null) {
                    om.CustDocType = customerIDProof.Doc_code;
                    om.CustDocValue = customerIDProof.Doc_No;
                }

                #region Order Master Details

                om.ObjID = kom.obj_id;
                om.CompanyCode = kom.company_code;
                om.BranchCode = kom.branch_code;
                om.OrderNo = kom.order_no;
                om.CustID = kom.Cust_Id;
                om.CustName = kom.cust_name;
                om.OrderType = SIGlobals.Globals.OrderType(kom.order_type) == null ? kom.order_type : SIGlobals.Globals.OrderType(kom.order_type);
                om.Remarks = kom.remarks;
                om.OrderDate = kom.order_date;
                om.OperatorCode = kom.operator_code;
                om.SalCode = kom.sal_code;
                om.DeliveryDate = kom.delivery_date;
                om.OrderRateType = kom.order_rate_type == "Today" ? "Rate Accepted" : "Delivery Rate";
                om.gsCode = kom.gs_code;
                om.Rate = kom.rate;
                om.AdvacnceOrderAmount = kom.advance_ord_amount;
                om.GrandTotal = kom.grand_total;
                om.ObjectStatus = kom.object_status;
                om.BillNo = kom.bill_no;
                om.CFlag = kom.cflag;
                om.CancelledBy = kom.cancelled_by;
                om.BillCounter = kom.bill_counter;
                om.UpdateOn = kom.UpdateOn;
                om.ClosedBranch = kom.closed_branch;
                om.ClosedFlag = kom.closed_flag;
                om.ClosedBy = kom.closed_by;
                om.ClosedDate = kom.closed_date;
                om.Karat = kom.karat;
                om.OrderDayRate = kom.order_day_rate;
                om.NewBillNo = kom.New_Bill_No;
                om.TaxPercentage = kom.tax_percentage;
                om.TotalAmountBeforeTax = kom.total_amount_before_tax;
                om.TotalTaxAmount = kom.total_tax_amount;
                om.ShiftID = kom.ShiftID;
                om.IsPAN = kom.isPAN;
                om.OldOrderNo = kom.old_order_no;
                om.Address1 = kom.address1;
                om.Address2 = kom.address2;
                om.Address3 = kom.address3;
                om.City = kom.city;
                om.PinCode = kom.pin_code;
                om.MobileNo = kom.mobile_no;
                om.State = kom.state;
                om.StateCode = kom.state_code;
                om.TIN = kom.tin;
                om.PANNo = kom.pan_no;
                om.ESTNo = kom.est_no;
                om.IDType = kom.id_type;
                om.IDDetails = kom.id_details;
                om.BookingType = kom.customer_type;
                om.PhoneNo = kom.phone_no;
                om.EmailID = kom.Email_ID;
                om.Salutation = kom.salutation;
                om.OrderAdvanceRateType = kom.booking_type;
                om.AdvancePercent = kom.adv_percent;
                om.RateValidityTill = kom.rate_fixed_till;

                #endregion

                #region Order Details
                List<OrderItemDetailsVM> lstOfOrderDetils = new List<OrderItemDetailsVM>();
                foreach (KTTU_ORDER_DETAILS orderDet in kod) {
                    OrderItemDetailsVM oid = new OrderItemDetailsVM();
                    oid.ObjID = orderDet.obj_id;
                    oid.CompanyCode = orderDet.company_code;
                    oid.BranchCode = orderDet.branch_code;
                    oid.OrderNo = orderDet.order_no;
                    oid.SlNo = orderDet.slno;
                    oid.ItemName = orderDet.item_name;
                    oid.Quantity = orderDet.quantity;
                    oid.Description = orderDet.description;
                    oid.FromGrossWt = orderDet.from_gwt;
                    oid.ToGrossWt = orderDet.to_gwt;
                    oid.MCPer = orderDet.mc_per;
                    oid.WastagePercent = orderDet.wst_per;
                    oid.Amount = orderDet.amt;
                    oid.SFlag = orderDet.s_flag;
                    oid.SParty = orderDet.s_party;
                    oid.BillNo = orderDet.bill_no;
                    oid.ImgID = orderDet.img_id;
                    oid.UpdateOn = orderDet.UpdateOn;
                    oid.VAPercent = orderDet.va_percent;
                    oid.ItemAmount = orderDet.item_amt;
                    oid.GSCode = orderDet.gs_code;
                    oid.FinYear = orderDet.Fin_Year;
                    oid.ItemwiseTaxPercentage = orderDet.itemwise_tax_percentage;
                    oid.ItemwiseTaxAmount = orderDet.itemwise_tax_amount;
                    oid.MCPerPiece = orderDet.mcperpiece;
                    oid.ProductID = orderDet.productid;
                    oid.IsIssued = orderDet.is_Issued;
                    oid.CounterCode = orderDet.counter_code;
                    oid.SalCode = orderDet.sal_code;
                    oid.MCPercent = orderDet.mc_percent;
                    oid.MCType = orderDet.mc_type;
                    oid.EstNo = orderDet.est_no;
                    lstOfOrderDetils.Add(oid);
                }
                om.lstOfOrderItemDetailsVM = lstOfOrderDetils;
                #endregion

                #region Payment Details
                List<PaymentVM> lstOfPayment = new List<PaymentVM>();
                foreach (KTTU_PAYMENT_DETAILS paymentDet in kpd) {
                    PaymentVM payment = new PaymentVM();
                    payment.ObjID = paymentDet.obj_id;
                    payment.CompanyCode = paymentDet.company_code;
                    payment.BranchCode = paymentDet.branch_code;
                    payment.SeriesNo = paymentDet.series_no;
                    payment.ReceiptNo = paymentDet.receipt_no;
                    payment.SNo = paymentDet.sno;
                    payment.TransType = paymentDet.trans_type;
                    payment.PayMode = paymentDet.pay_mode; /*SIGlobals.Globals.PaymentMode(paymentDet.pay_mode) == "" ? paymentDet.pay_mode : SIGlobals.Globals.PaymentMode(paymentDet.pay_mode);*/
                    payment.PayDetails = paymentDet.pay_details;
                    payment.PayDate = paymentDet.pay_date;
                    payment.PayAmount = paymentDet.pay_amt;
                    payment.RefBillNo = paymentDet.Ref_BillNo;
                    payment.PartyCode = paymentDet.party_code;
                    payment.BillCounter = paymentDet.bill_counter;
                    payment.IsPaid = paymentDet.is_paid;
                    payment.Bank = paymentDet.bank;
                    payment.BankName = paymentDet.bank_name;
                    payment.ChequeDate = paymentDet.cheque_date;
                    payment.CardType = paymentDet.card_type;
                    payment.ExpiryDate = paymentDet.expiry_date;
                    payment.CFlag = paymentDet.cflag;
                    payment.CardAppNo = paymentDet.card_app_no;
                    payment.SchemeCode = paymentDet.scheme_code;
                    payment.SalBillType = paymentDet.sal_bill_type;
                    payment.OperatorCode = paymentDet.operator_code;
                    payment.SessionNo = paymentDet.session_no;
                    payment.UpdateOn = paymentDet.UpdateOn;
                    payment.GroupCode = paymentDet.group_code;
                    payment.AmtReceived = paymentDet.amt_received;
                    payment.BonusAmt = paymentDet.bonus_amt;
                    payment.WinAmt = paymentDet.win_amt;
                    payment.CTBranch = paymentDet.ct_branch;
                    payment.FinYear = paymentDet.fin_year;
                    payment.CardCharges = paymentDet.CardCharges;
                    payment.ChequeNo = paymentDet.cheque_no;
                    payment.NewBillNo = paymentDet.new_receipt_no;
                    payment.AddDisc = paymentDet.Add_disc;
                    payment.IsOrderManual = paymentDet.isOrdermanual;
                    payment.CurrencyValue = paymentDet.currency_value;
                    payment.ExchangeRate = paymentDet.exchange_rate;
                    payment.CurrencyType = paymentDet.currency_type;
                    payment.TaxPercentage = paymentDet.tax_percentage;
                    payment.CancelledBy = paymentDet.cancelled_by;
                    payment.CancelledRemarks = paymentDet.cancelled_remarks;
                    payment.CancelledDate = paymentDet.cancelled_date;
                    payment.IsExchange = paymentDet.isExchange;
                    payment.ExchangeNo = paymentDet.exchangeNo;
                    payment.NewReceiptNo = paymentDet.new_receipt_no;
                    payment.GiftAmount = paymentDet.Gift_Amount;
                    payment.CardSwipedBy = paymentDet.cardSwipedBy;
                    payment.Version = paymentDet.version;
                    payment.GSTGroupCode = paymentDet.GSTGroupCode;
                    payment.SGSTPercent = paymentDet.SGST_Percent;
                    payment.CGSTPercent = paymentDet.CGST_Percent;
                    payment.IGSTPercent = paymentDet.IGST_Percent;
                    payment.HSN = paymentDet.HSN;
                    payment.SGSTAmount = paymentDet.SGST_Amount;
                    payment.CGSTAmount = paymentDet.CGST_Amount;
                    payment.IGSTAmount = paymentDet.IGST_Amount;
                    payment.PayAmountBeforeTax = paymentDet.pay_amount_before_tax;
                    payment.PayTaxAmount = paymentDet.pay_tax_amount;
                    lstOfPayment.Add(payment);
                }
                om.lstOfPayment = lstOfPayment;
                #endregion
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
            return om;
        }

        public KTTU_PAYMENT_DETAILS GetOrderReceiptTotalPrint(string companyCode, string branchCode, int orderNo, out ErrorVM error)
        {
            error = null;
            KTTU_PAYMENT_DETAILS kpdm = new KTTU_PAYMENT_DETAILS();
            try {
                KTTU_ORDER_MASTER kom = new KTTU_ORDER_MASTER();
                kom = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == orderNo
                                                    && ord.branch_code == branchCode
                                                    && ord.company_code == companyCode).FirstOrDefault();
                if (kom == null) {
                    error = new ErrorVM() { field = "Order Number", index = 0, description = "Invalid order number", ErrorStatusCode = System.Net.HttpStatusCode.NotFound };
                    return null;
                }

                List<KTTU_PAYMENT_DETAILS> paymentDet = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.series_no == orderNo
                                                                                        && ord.company_code == companyCode
                                                                                        && ord.branch_code == branchCode
                                                                                        && ord.trans_type == "O").ToList();
                if (paymentDet != null && paymentDet.Count <= 0) {
                    error = new ErrorVM() { field = "Order Number", index = 0, description = "No payment details for the selected order.", ErrorStatusCode = System.Net.HttpStatusCode.NotFound };
                    return null;
                }

                kpdm.SGST_Percent = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.series_no == orderNo && ord.company_code == companyCode && ord.branch_code == branchCode && ord.trans_type == "O").FirstOrDefault().SGST_Percent;
                kpdm.CGST_Percent = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.series_no == orderNo && ord.company_code == companyCode && ord.branch_code == branchCode && ord.trans_type == "O").FirstOrDefault().CGST_Percent;
                kpdm.IGST_Percent = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.series_no == orderNo && ord.company_code == companyCode && ord.branch_code == branchCode && ord.trans_type == "O").FirstOrDefault().IGST_Percent;

                kpdm.SGST_Percent = kpdm.SGST_Percent == null ? 0 : kpdm.SGST_Percent;
                kpdm.CGST_Percent = kpdm.CGST_Percent == null ? 0 : kpdm.CGST_Percent;
                kpdm.IGST_Percent = kpdm.IGST_Percent == null ? 0 : kpdm.IGST_Percent;

                kpdm.SGST_Amount = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.series_no == orderNo && ord.company_code == companyCode && ord.branch_code == branchCode && ord.trans_type == "O").Sum(ord => ord.SGST_Amount);
                kpdm.CGST_Amount = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.series_no == orderNo && ord.company_code == companyCode && ord.branch_code == branchCode && ord.trans_type == "O").Sum(ord => ord.CGST_Amount);
                kpdm.IGST_Amount = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.series_no == orderNo && ord.company_code == companyCode && ord.branch_code == branchCode && ord.trans_type == "O").Sum(ord => ord.IGST_Amount);

                kpdm.SGST_Amount = kpdm.SGST_Amount == null ? 0 : kpdm.SGST_Amount;
                kpdm.CGST_Amount = kpdm.CGST_Amount == null ? 0 : kpdm.CGST_Amount;
                kpdm.IGST_Amount = kpdm.IGST_Amount == null ? 0 : kpdm.IGST_Amount;

                // For orders there is no GST right now, incase in future need to change the bellow line of code.
                kpdm.pay_amount_before_tax = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.series_no == orderNo && ord.company_code == companyCode && ord.branch_code == branchCode && ord.trans_type == "O").Sum(ord => ord.pay_amt);
                kpdm.pay_amt = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.series_no == orderNo && ord.company_code == companyCode && ord.branch_code == branchCode && ord.trans_type == "O").Sum(ord => ord.pay_amt);
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
            return kpdm;
        }

        public OrderItemDetailsVM GetOrderFormTotalPrint(string companyCode, string branchCode, int orderNo, out ErrorVM error)
        {
            error = null;
            OrderItemDetailsVM orderDetails = new OrderItemDetailsVM();
            KTTU_ORDER_MASTER kom = new KTTU_ORDER_MASTER();
            List<KTTU_ORDER_DETAILS> kod = new List<KTTU_ORDER_DETAILS>();
            List<KTTU_PAYMENT_DETAILS> kpd = new List<KTTU_PAYMENT_DETAILS>();

            try {
                kom = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == orderNo
                                                    && ord.branch_code == branchCode
                                                    && ord.company_code == companyCode).FirstOrDefault();
                kod = db.KTTU_ORDER_DETAILS.Where(ord => ord.order_no == orderNo
                                                    && ord.branch_code == branchCode
                                                    && ord.company_code == companyCode).ToList();
                kpd = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.series_no == orderNo
                                                    && ord.branch_code == branchCode
                                                    && ord.company_code == companyCode).ToList().OrderBy(ord => ord.pay_date).ToList();

                if (kom == null) {
                    error = new ErrorVM()
                    {
                        field = "Order Number",
                        index = 0,
                        description = "Invalid order number",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }

                #region Order Details

                foreach (KTTU_ORDER_DETAILS orderDet in kod) {
                    OrderItemDetailsVM oid = new OrderItemDetailsVM();
                    orderDetails.Quantity = orderDetails.Quantity + orderDet.quantity;
                    orderDetails.FromGrossWt = orderDetails.FromGrossWt + orderDet.from_gwt;
                    orderDetails.ToGrossWt = orderDetails.ToGrossWt + orderDet.to_gwt;
                }
                #endregion
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
            return orderDetails;
        }

        public KTTU_PAYMENT_DETAILS GetClosedOrderReceiptTotalPrint(string companyCode, string branchCode, int orderNo, out ErrorVM error)
        {
            error = null;
            KTTU_PAYMENT_DETAILS kpdm = new KTTU_PAYMENT_DETAILS();
            KTTU_ORDER_MASTER kom = new KTTU_ORDER_MASTER();
            List<KTTU_PAYMENT_DETAILS> kpd = new List<KTTU_PAYMENT_DETAILS>();
            try {
                kom = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == orderNo
                                                    && ord.branch_code == branchCode
                                                    && ord.company_code == companyCode).FirstOrDefault();
                kpd = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.series_no == orderNo
                                                    && ord.trans_type == "CO"
                                                    && ord.branch_code == branchCode
                                                    && ord.company_code == companyCode).OrderBy(ord => ord.pay_date).ToList();
                if (kom == null) {
                    error = new ErrorVM()
                    {
                        field = "Order Number",
                        index = 0,
                        description = "Invalid order number",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }

                kpdm.pay_amt = 0;
                kpdm.pay_amount_before_tax = 0;
                foreach (KTTU_PAYMENT_DETAILS kpdd in kpd) {
                    kpdm.pay_amt = kpdm.pay_amt + kpdd.pay_amt;
                    kpdm.SGST_Percent = kpdm.SGST_Percent == null ? 0 : kpdm.SGST_Percent;
                    kpdm.CGST_Percent = kpdm.CGST_Percent == null ? 0 : kpdm.CGST_Percent;
                    kpdm.IGST_Percent = kpdm.IGST_Percent == null ? 0 : kpdm.IGST_Percent;
                    kpdm.SGST_Amount = kpdm.SGST_Amount == null ? 0 : kpdm.SGST_Amount;
                    kpdm.CGST_Amount = kpdm.CGST_Amount == null ? 0 : kpdm.CGST_Amount;
                    kpdm.IGST_Amount = kpdm.IGST_Amount == null ? 0 : kpdm.IGST_Amount;
                    // For orders there is no GST right now, incase in future need to change the bellow line of code.
                    kpdm.pay_amount_before_tax = kpdm.pay_amt;
                }
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
            return kpdm;
        }

        public List<PaymentVM> GetReceiptDetailsPrint(string companyCode, string branchCode, int receiptNo, out ErrorVM error)
        {
            error = null;
            try {
                List<KTTU_PAYMENT_DETAILS> kpd = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.receipt_no == receiptNo
                                                                                && ord.branch_code == branchCode
                                                                                && ord.company_code == companyCode
                                                                                && ord.trans_type == "O").OrderBy(ord => ord.pay_date).ToList();
                if (kpd == null) {
                    error = new ErrorVM()
                    {
                        field = "Receipt Number",
                        index = 0,
                        description = "Invalid receipt number",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }
                List<PaymentVM> lstOfPayment = new List<PaymentVM>();
                foreach (KTTU_PAYMENT_DETAILS paymentDet in kpd) {
                    PaymentVM payment = new PaymentVM();
                    payment.ObjID = paymentDet.obj_id;
                    payment.CompanyCode = paymentDet.company_code;
                    payment.BranchCode = paymentDet.branch_code;
                    payment.SeriesNo = paymentDet.series_no;
                    payment.ReceiptNo = paymentDet.receipt_no;
                    payment.SNo = paymentDet.sno;
                    payment.TransType = paymentDet.trans_type;
                    payment.PayMode = paymentDet.pay_mode; //SIGlobals.Globals.PaymentMode(paymentDet.pay_mode) == "" ? paymentDet.pay_mode : SIGlobals.Globals.PaymentMode(paymentDet.pay_mode);
                    payment.PayDetails = paymentDet.pay_details;
                    payment.PayDate = paymentDet.pay_date;
                    payment.PayAmount = paymentDet.pay_amt;
                    payment.RefBillNo = paymentDet.Ref_BillNo;
                    payment.PartyCode = paymentDet.party_code;
                    payment.BillCounter = paymentDet.bill_counter;
                    payment.IsPaid = paymentDet.is_paid;
                    payment.Bank = paymentDet.bank;
                    payment.BankName = paymentDet.bank_name;
                    payment.ChequeDate = paymentDet.cheque_date;
                    payment.CardType = paymentDet.card_type;
                    payment.ExpiryDate = paymentDet.expiry_date;
                    payment.CFlag = paymentDet.cflag;
                    payment.CardAppNo = paymentDet.card_app_no;
                    payment.SchemeCode = paymentDet.scheme_code;
                    payment.SalBillType = paymentDet.sal_bill_type;
                    payment.OperatorCode = paymentDet.operator_code;
                    payment.SessionNo = paymentDet.session_no;
                    payment.UpdateOn = paymentDet.UpdateOn;
                    payment.GroupCode = paymentDet.group_code;
                    payment.AmtReceived = paymentDet.amt_received;
                    payment.BonusAmt = paymentDet.bonus_amt;
                    payment.WinAmt = paymentDet.win_amt;
                    payment.CTBranch = paymentDet.ct_branch;
                    payment.FinYear = paymentDet.fin_year;
                    payment.CardCharges = paymentDet.CardCharges;
                    payment.ChequeNo = paymentDet.cheque_no;
                    payment.NewBillNo = paymentDet.new_receipt_no;
                    payment.AddDisc = paymentDet.Add_disc;
                    payment.IsOrderManual = paymentDet.isOrdermanual;
                    payment.CurrencyValue = paymentDet.currency_value;
                    payment.ExchangeRate = paymentDet.exchange_rate;
                    payment.CurrencyType = paymentDet.currency_type;
                    payment.TaxPercentage = paymentDet.tax_percentage;
                    payment.CancelledBy = paymentDet.cancelled_by;
                    payment.CancelledRemarks = paymentDet.cancelled_remarks;
                    payment.CancelledDate = paymentDet.cancelled_date;
                    payment.IsExchange = paymentDet.isExchange;
                    payment.ExchangeNo = paymentDet.exchangeNo;
                    payment.NewReceiptNo = paymentDet.new_receipt_no;
                    payment.GiftAmount = paymentDet.Gift_Amount;
                    payment.CardSwipedBy = paymentDet.cardSwipedBy;
                    payment.Version = paymentDet.version;
                    payment.GSTGroupCode = paymentDet.GSTGroupCode;
                    payment.SGSTPercent = paymentDet.SGST_Percent;
                    payment.CGSTPercent = paymentDet.CGST_Percent;
                    payment.IGSTPercent = paymentDet.IGST_Percent;
                    payment.HSN = paymentDet.HSN;
                    payment.SGSTAmount = paymentDet.SGST_Amount;
                    payment.CGSTAmount = paymentDet.CGST_Amount;
                    payment.IGSTAmount = paymentDet.IGST_Amount;
                    payment.PayAmountBeforeTax = paymentDet.pay_amount_before_tax;
                    payment.PayTaxAmount = paymentDet.pay_tax_amount;
                    lstOfPayment.Add(payment);
                }
                return lstOfPayment;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public KTTU_PAYMENT_DETAILS GetReceiptDetTotalPrint(string companyCode, string branchCode, int receiptNo, out ErrorVM error)
        {
            error = null;
            KTTU_PAYMENT_DETAILS kpdm = new KTTU_PAYMENT_DETAILS();
            try {
                kpdm.SGST_Percent = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.receipt_no == receiptNo
                                                                    && ord.branch_code == branchCode
                                                                    && ord.company_code == companyCode && ord.trans_type == "O").FirstOrDefault().SGST_Percent;
                kpdm.CGST_Percent = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.receipt_no == receiptNo
                                                                    && ord.branch_code == branchCode
                                                                    && ord.company_code == companyCode && ord.trans_type == "O").FirstOrDefault().CGST_Percent;
                kpdm.IGST_Percent = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.receipt_no == receiptNo
                                                                    && ord.branch_code == branchCode
                                                                    && ord.company_code == companyCode && ord.trans_type == "O").FirstOrDefault().IGST_Percent;

                kpdm.SGST_Percent = kpdm.SGST_Percent == null ? 0 : kpdm.SGST_Percent;
                kpdm.CGST_Percent = kpdm.CGST_Percent == null ? 0 : kpdm.CGST_Percent;
                kpdm.IGST_Percent = kpdm.IGST_Percent == null ? 0 : kpdm.IGST_Percent;

                kpdm.SGST_Amount = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.receipt_no == receiptNo
                                                                    && ord.branch_code == branchCode
                                                                    && ord.company_code == companyCode && ord.trans_type == "O").Sum(ord => ord.SGST_Amount);
                kpdm.CGST_Amount = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.receipt_no == receiptNo
                                                                    && ord.branch_code == branchCode
                                                                    && ord.company_code == companyCode && ord.trans_type == "O").Sum(ord => ord.CGST_Amount);
                kpdm.IGST_Amount = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.receipt_no == receiptNo
                                                                    && ord.branch_code == branchCode
                                                                    && ord.company_code == companyCode && ord.trans_type == "O").Sum(ord => ord.IGST_Amount);

                kpdm.SGST_Amount = kpdm.SGST_Amount == null ? 0 : kpdm.SGST_Amount;
                kpdm.CGST_Amount = kpdm.CGST_Amount == null ? 0 : kpdm.CGST_Amount;
                kpdm.IGST_Amount = kpdm.IGST_Amount == null ? 0 : kpdm.IGST_Amount;

                // For orders there is no GST right now, incase in future need to change the bellow line of code.
                kpdm.pay_amount_before_tax = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.receipt_no == receiptNo && ord.branch_code == branchCode
                                                                    && ord.company_code == companyCode).Sum(ord => ord.pay_amt);
                kpdm.pay_amt = db.KTTU_PAYMENT_DETAILS.Where(ord => ord.receipt_no == receiptNo && ord.branch_code == branchCode
                                                                    && ord.company_code == companyCode).Sum(ord => ord.pay_amt);

            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
            return kpdm;
        }

        #endregion

        #region Generic Print Handling
        public ProdigyPrintVM GetOrderPrint(string companyCode, string branchCode, int orderNo, string orderType, out ErrorVM error)
        {
            error = null;
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            PrintConfiguration pc = new PrintConfiguration();
            var printConfig = pc.GetPrintConfigurationForDraftDocuments(companyCode, branchCode, "ORD_FRM");
            if (printConfig == "HTML") {
                var htmlPrintData = GetOrdePrintHTML(companyCode, branchCode, orderNo, orderType, out error);
                printObject.ContinueNextPrint = false;
                printObject.Data = new PrintConfiguration().Base64Encode(htmlPrintData);
                printObject.PrintType = "HTML";
            }
            else {
                var dotMatrixPrintData = GetOrderPrintDotmatrix(companyCode, branchCode, orderNo, out error);
                printObject.ContinueNextPrint = true;
                printObject.Data = dotMatrixPrintData;
                printObject.PrintType = "RAW";
            }
            return printObject;
        }

        public ProdigyPrintVM GetOrderReceiptPrint(string companyCode, string branchCode, int receiptNo, out ErrorVM error)
        {
            error = null;
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            PrintConfiguration pc = new PrintConfiguration();
            var printConfig = pc.GetPrintConfigurationForDraftDocuments(companyCode, branchCode, "ORD_REC");
            if (printConfig == "HTML") {
                var htmlPrintData = GetReceiptDetailsPrintHTML(companyCode, branchCode, receiptNo, out error);
                printObject.ContinueNextPrint = false;
                printObject.Data = new PrintConfiguration().Base64Encode(htmlPrintData);
                printObject.PrintType = "HTML";
            }
            else {
                var dotMatrixPrintData = GetReceiptDetailsPrintDotMatrix(companyCode, branchCode, receiptNo, out error);
                printObject.ContinueNextPrint = true;
                printObject.Data = dotMatrixPrintData;
                printObject.PrintType = "RAW";
            }
            return printObject;
        }

        public ProdigyPrintVM GetClosedOrderPrint(string companyCode, string branchCode, int orderNo, out ErrorVM error)
        {
            error = null;
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            PrintConfiguration pc = new PrintConfiguration();
            var printConfig = pc.GetPrintConfigurationForDraftDocuments(companyCode, branchCode, "ORD_CLS");
            if (printConfig == "HTML") {
                var htmlPrintData = GetClosedOrderPrintHTML(companyCode, branchCode, orderNo, out error);
                printObject.ContinueNextPrint = false;
                printObject.Data = new PrintConfiguration().Base64Encode(htmlPrintData);
                printObject.PrintType = "HTML";
            }
            else {
                var dotMatrixPrintData = GetClosedOrderPrintDotMatrix(companyCode, branchCode, orderNo, out error);
                printObject.ContinueNextPrint = true;
                printObject.Data = dotMatrixPrintData;
                printObject.PrintType = "RAW";
            }
            return printObject;
        }

        #endregion

        #region Dotmatrix and HTML Printing
        public string GetOrdePrintHTML(string companyCode, string branchCode, int orderNo, string reprintOrderType, out ErrorVM error)
        {
            error = null;
            KSTU_COMPANY_MASTER companyMaster = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode && c.branch_code == branchCode).FirstOrDefault();
            KTTU_ORDER_MASTER order = db.KTTU_ORDER_MASTER.Where(ord => ord.company_code == companyCode
                                                                    && ord.branch_code == branchCode
                                                                    && ord.order_no == orderNo).FirstOrDefault();

            if (order == null) {
                error = new ErrorVM()
                {
                    description = "Invalid Order Number",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return "";
            }
            List<KTTU_ORDER_DETAILS> orderDet = db.KTTU_ORDER_DETAILS.Where(ord => ord.company_code == companyCode
                                                                            && ord.branch_code == branchCode
                                                                            && ord.order_no == order.order_no).ToList();
            List<KTTU_PAYMENT_DETAILS> payments = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.company_code == companyCode
                                                                            && pay.branch_code == branchCode
                                                                            && pay.series_no == order.order_no
                                                                            && pay.trans_type == "O").ToList();
            KSTU_CUSTOMER_MASTER customer = new CustomerBL().GetActualCustomerDetails(order.Cust_Id, order.mobile_no, companyCode, branchCode);
            KSTU_CUSTOMER_ID_PROOF_DETAILS proof = db.KSTU_CUSTOMER_ID_PROOF_DETAILS.Where(cust => cust.cust_id == customer.cust_id
                                                                                            && cust.company_code == companyCode
                                                                                            && cust.branch_code == branchCode).FirstOrDefault();
            string CompanyAddress = SIGlobals.Globals.GetcompanyDetailsForPrint(db, companyCode, branchCode);

            StringBuilder sbStart = new StringBuilder();
            sbStart.AppendLine("<html>");
            sbStart.AppendLine("<head>");
            sbStart.AppendLine(SIGlobals.Globals.GetStyleOrder());
            sbStart.AppendLine("</head>");
            sbStart.AppendLine("<body>");
            string orderkarat;
            if (string.Compare(order.karat, "NA") != 0)
                orderkarat = " karat :" + Convert.ToString(order.karat);
            else
                orderkarat = "";

            string OrderDate = string.Format("{0:dd/MM/yyyy} ", order.order_date);
            string DeliveryDate = string.Format("{0:dd/MM/yyyy} ", order.delivery_date);
            string OrderRateType = string.Empty;
            string DisplayType = string.Empty;
            if (string.Compare(order.order_rate_type, "Today") == 0) {
                OrderRateType = "Fixed";
                DisplayType = "Fixed Rate Accepted";
            }
            else if (string.Compare(order.order_rate_type, "Delivery") == 0) {
                OrderRateType = "Delivery";
                DisplayType = "Delivery Rate Accepted";
            }
            else if (string.Compare(order.order_rate_type, "Flexi") == 0) {
                OrderRateType = "Flexi";
                DisplayType = "Flexi Rate Accepted";
            }
            string Sultation = order.salutation;
            string Name = order.cust_name;
            string Address1 = order.address1;
            string Address2 = order.address2;
            string Address3 = order.address3;
            string state = order.state;
            string code = db.KSTS_STATE_MASTER.Where(st => st.state_name == state).FirstOrDefault().tinno.ToString();
            string Mobile = order.mobile_no;
            string pan = order.pan_no;
            string Sal_Code = order.sal_code;
            string operator_code = order.operator_code;
            string CustGSTIN = order.tin;
            string City = order.city;
            string Address = string.Empty;
            if (Address1 != string.Empty)
                Address = Address + "<br>" + Address1;
            if (Address2 != string.Empty)
                Address = Address + "<br>" + Address2;
            if (Address3 != string.Empty)
                Address = Address + "<br>" + Address3;
            if (!string.IsNullOrEmpty(Sultation))
                Name = Sultation + " " + Name;

            string ordertype = order.order_type;
            string type = string.Empty;
            if (string.Compare(ordertype, "R") == 0)
                type = "Reserved Order";
            else if (string.Compare(ordertype, "O") == 0)
                type = "Customised Order";
            else if (string.Compare(ordertype, "A") == 0)
                type = "Advance Order";

            sbStart.AppendLine("<Table frame=\"border\" border=\"0\" width=\"750\" style=\"border-collapse:collapse\" >");
            for (int j = 0; j < 12; j++) {
                sbStart.AppendLine("<TR style=border:0>");
                sbStart.AppendLine(string.Format("<TD style=border:0 ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                sbStart.AppendLine("</TR>");
            }
            sbStart.AppendLine("<tr style=\"border-right:thin ; border-bottom:thin\";  \"color:black; text-decoration:bold;\" align=\"center\">");
            if (string.Compare(order.cflag, "Y") == 0) {
                sbStart.AppendLine("<TD style=\"border-right:thin ; font-size:12pt\" colspan=3 ALIGN = \"center\"><b>ORDER FORM/CANCELLED</b></TD>");
            }
            else {
                sbStart.AppendLine("<TD style=\"border-right:thin ; font-size:12pt\"; colspan=3 ALIGN = \"center\"><b>ORDER FORM</b></TD>");
            }
            sbStart.AppendLine("</TR>");
            for (int j = 0; j < 1; j++) {
                sbStart.AppendLine("<TR style=border:0>");
                sbStart.AppendLine(string.Format("<TD style=border:0 ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                sbStart.AppendLine("</TR>");
            }
            sbStart.AppendLine("<Table  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\" border-collapse:collapse; font-size=9pt;\" width=\"750\">");
            sbStart.AppendLine("<TR  bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\"  style=\"border-left:thin\">");
            sbStart.AppendLine(string.Format("<TD width=\"200\" ALIGN = \"LEFT\"><b>CUSTOMER DETAILS</b></TD>"));
            sbStart.AppendLine(string.Format("<TD width=\"200\" ALIGN = \"CENTER\"><b>GSTIN : {0}</b></TD>", companyMaster.tin_no.ToString()));
            sbStart.AppendLine(string.Format("<TD width=\"150\" ALIGN = \"LEFT\"><b>{0}</b></TD>", "SHOWROOM ADDRESS"));
            sbStart.AppendLine("</TR>");
            sbStart.AppendLine("<tr>");
            sbStart.AppendLine("<td>");
            sbStart.AppendLine("<Table>");
            sbStart.AppendLine("<Table   class=\"boldText\"  style=\"border-collapse:collapse; font-size=9pt;\" >");
            sbStart.AppendLine("<tr style=\"border-right:0\"  >");
            sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left \" ><b>Name &nbsp&nbsp</b></td>");
            sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + Name + "</b></td>");
            sbStart.AppendLine("</tr>");
            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
            sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" ><b>Address &nbsp&nbsp </b></td>");
            sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + Address1 + "</b></td>");
            sbStart.AppendLine("</tr>");
            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
            sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
            sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + Address2 + "</b></td>");
            sbStart.AppendLine("</tr>");
            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
            sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
            sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + Address3 + "</b></td>");
            sbStart.AppendLine("</tr>");
            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
            sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" ><b>State &nbsp&nbsp </b></td>");
            sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + state + "</b></td>");
            sbStart.AppendLine("</tr>");

            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
            sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>Code &nbsp&nbsp&nbsp </b></td>");
            sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + code + "</b></td>");
            sbStart.AppendLine("</tr>");

            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
            sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>Phone &nbsp&nbsp </b></td>");
            sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + Mobile + "</b></td>");
            sbStart.AppendLine("</tr>");
            if (!string.IsNullOrEmpty(pan)) {
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>PAN &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + pan + "</b></td>");
                sbStart.AppendLine("</tr>");
            }
            else {
                if (proof != null) {
                    if (!string.IsNullOrEmpty(proof.Doc_code)) {
                        sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                        sbStart.AppendLine(string.Format("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>{0} &nbsp&nbsp</b></td>", proof.Doc_code));
                        sbStart.AppendLine(string.Format("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>{0}</b></td>", proof.Doc_code));
                        sbStart.AppendLine("</tr>");
                    }
                }
            }
            if (!string.IsNullOrEmpty(CustGSTIN)) {
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>GSTIN &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + CustGSTIN + "</b></td>");
                sbStart.AppendLine("</tr>");
            }

            sbStart.AppendLine("</table>");
            sbStart.AppendLine("</td>");


            string DateTime = string.Format("{0:dd/MM/yyyy} {0:T}", Convert.ToDateTime(order.order_date));

            sbStart.AppendLine("<td>");
            sbStart.AppendLine("<Table  class=\"boldText\"  style=\"border-collapse:collapse; font-size=9pt;  \" >");

            sbStart.AppendLine("<tr style=\"border-right:0\">");
            sbStart.AppendLine("<td style=\"border-right:thin ; border-top:thin\" align=\"LEFT \" ><b>&nbsp</b> ");
            sbStart.Append(string.Format("<img src=\"{0}\" align=\"center \"/>", ""));
            sbStart.AppendLine("</td>");
            sbStart.AppendLine("</tr>");

            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
            sbStart.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" ><b>Order No &nbsp&nbsp</b></td>");
            sbStart.AppendLine("<td style=\"border-right:thin ; border-top:thin\" ><b>" + branchCode + "/" + "O" + "/" + orderNo + "</b></td>");
            sbStart.AppendLine("</tr>");

            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
            sbStart.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" ><b>Order Date &nbsp&nbsp</b></td>");
            sbStart.AppendLine("<td style=\"border-right:thin ; border-top:thin\" ><b>" + OrderDate + "</b></td>");
            sbStart.AppendLine("</tr>");

            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
            sbStart.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" ><b> Place of supply &nbsp&nbsp</b></td>");
            sbStart.AppendLine("<td style=\"border-right:thin;border-top:thin\" ><b>" + companyMaster.state + "</b></td>");
            sbStart.AppendLine("</tr>");

            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
            sbStart.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" ><b> Tax Payment Mode &nbsp&nbsp</b></td>");
            sbStart.AppendLine("<td style=\"border-right:thin;border-top:thin\" ><b>" + "Normal" + "</b></td>");
            sbStart.AppendLine("</tr>");


            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
            sbStart.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" ><b>Rate &nbsp&nbsp</b></td>");
            sbStart.AppendLine("<td style=\"border-right:thin ; border-top:thin\" ><b>" + order.rate.ToString() + "</b></td>");
            sbStart.AppendLine("</tr>");


            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
            sbStart.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" ><b>Delivery Date &nbsp&nbsp</b></td>");
            sbStart.AppendLine("<td style=\"border-right:thin ; border-top:thin\" ><b>" + DeliveryDate + "</b></td>");
            sbStart.AppendLine("</tr>");

            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
            sbStart.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" ><b>Order Type &nbsp&nbsp</b></td>");
            sbStart.AppendLine("<td style=\"border-right:thin ; border-top:thin\" ><b>" + type + "</b></td>");
            sbStart.AppendLine("</tr>");

            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
            sbStart.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" ><b>PAN &nbsp&nbsp</b></td>");
            sbStart.AppendLine("<td style=\"border-right:thin ; border-top:thin\" ><b>" + companyMaster.pan_no + "</b></td>");
            sbStart.AppendLine("</tr>");

            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
            sbStart.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" ><b>Rate Type &nbsp&nbsp</b></td>");
            sbStart.AppendLine("<td style=\"border-right:thin ; border-top:thin\" ><b>" + OrderRateType + "</b></td>");
            sbStart.AppendLine("</tr>");


            if (!string.IsNullOrEmpty(order.booking_type)) {
                ORDER_TYPE_ACC_POSTING orderType = db.ORDER_TYPE_ACC_POSTING.Where(ord => ord.company_code == companyCode
                                                                                    && ord.branch_code == branchCode
                                                                                    && ord.booking_code == order.booking_type).FirstOrDefault();
                string BookingName = ordertype == null ? "" : orderType.booking_name;
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" ><b>Purchase Plan &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin ; border-top:thin\" ><b>" + BookingName + "</b></td>");
                sbStart.AppendLine("</tr>");
            }

            sbStart.AppendLine("</table>");
            sbStart.AppendLine("</td>");

            sbStart.AppendLine("<td>");
            sbStart.AppendLine("<Table    class=\"boldText\"  style=\"border-collapse:collapse; font-size=9pt;\" >");

            sbStart.AppendLine("<tr style=\"border-right:0\"  >");
            sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left \" ><b>Name &nbsp&nbsp</b></td>");
            sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + companyMaster.company_name + "</b></td>");
            sbStart.AppendLine("</tr>");

            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
            sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>Address &nbsp&nbsp</b></td>");
            sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + companyMaster.address1 + "</b></td>");
            sbStart.AppendLine("</tr>");
            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
            sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
            sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + companyMaster.address2 + "</b></td>");
            sbStart.AppendLine("</tr>");
            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
            sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
            sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + companyMaster.address3 + "</b></td>");
            sbStart.AppendLine("</tr>");

            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
            sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>City &nbsp&nbsp</b></td>");
            sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + companyMaster.city + "</b></td>");
            sbStart.AppendLine("</tr>");

            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
            sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>State &nbsp&nbsp</b></td>");
            sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + companyMaster.state + "</b></td>");
            sbStart.AppendLine("</tr>");


            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
            sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>Code &nbsp&nbsp&nbsp</b></td>");
            sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + companyMaster.state_code + "</b></td>");
            sbStart.AppendLine("</tr>");

            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
            sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>Phone &nbsp&nbsp</b></td>");
            sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + companyMaster.phone_no + "</b></td>");
            sbStart.AppendLine("</tr>");
            sbStart.AppendLine("</table>");
            sbStart.AppendLine("</td>");

            sbStart.AppendLine("</TR>");

            sbStart.AppendLine("</Table>");



            sbStart.AppendLine("<Table bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse; font-size=9pt;\" width=\"750\">");

            sbStart.AppendLine("<TR>");
            sbStart.AppendLine(string.Format("<TD style=\"border-top:thin\" width=\"750\" colspan = 3 ALIGN = \"CENTER\"><b>{0}<br></b></TD>", reprintOrderType));
            sbStart.AppendLine("</TR>");

            sbStart.AppendLine("<TR bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
            sbStart.AppendLine("<TH  style=\"border-bottom:thin solid\" width=\"250\" ALIGN = \"CENTER\"><b>Item Description</b></TH>");
            sbStart.AppendLine("<TH  style=\"border-bottom:thin solid\" width=\"150\" ALIGN = \"RIGHT\"><b>Quantity</b></TH>");
            sbStart.AppendLine("<TH  style=\"border-bottom:thin solid\" width=\"150\" ALIGN = \"RIGHT\"><b>Weight</b></TH>");
            sbStart.AppendLine("</TR>");

            int Pcs = 0;
            decimal Gwt = 0, Nwt = 0, Amount = 0;
            string ItemName = string.Empty;
            string VA = string.Empty;

            int MaxPageRow = 4;
            int ItemCount = orderDet.Count;
            List<string> lstItemName = null;
            if (ItemCount <= 3) {
                for (int i = 0; i < ItemCount; i++) {
                    sbStart.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");

                    string ItemName1 = orderDet[i].item_name;
                    string Descp = orderDet[i].description;
                    if (Descp.Length <= 55)
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ;border-top:thin\" width=\"250\"  ALIGN = \"LEFT\"><b>{0}{1}{1}</b></TD>", Descp, "&nbsp"));
                    else {
                        string[] Descwtspace = Descp.Split(' ');
                        if (Descwtspace.Length == 1) {
                            lstItemName = SIGlobals.Globals.SplitStringAt(55, Descp);
                            if (lstItemName != null && lstItemName.Count > 1) {
                                string ItemNames = string.Empty;
                                for (int x = 0; x < lstItemName.Count; x++)
                                    ItemNames = ItemNames + " " + lstItemName[x].Trim();
                                lstItemName = null;
                                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ;border-top:thin\"  width=\"250\"   ALIGN = \"LEFT\"><b>{0}{1}{1}</b></TD>", ItemNames, "&nbsp"));
                            }
                        }
                        else {
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ;border-top:thin\"  width=\"250\"  ALIGN = \"LEFT\"><b>{0}{1}{1}</b></TD>", Descp, "&nbsp"));
                        }
                    }

                    if (orderDet[i].quantity > 0) {
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ;border-top:thin\"  width=\"150\" ALIGN = \"RIGHT\"><b>{0}{1} </b></TD>", orderDet[i].quantity.ToString(), "&nbsp"));
                    }
                    else {
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ;border-top:thin\"  width=\"150\" ALIGN = \"RIGHT\"><b>{0}{1} </b></TD>", "1", "&nbsp"));
                    }
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ;border-top:thin\" width=\"150\" ALIGN = \"RIGHT\"><b>{0}{1} </b></TD>", orderDet[i].from_gwt, "&nbsp"));

                    Pcs += orderDet[i].quantity;
                    Gwt += orderDet[i].from_gwt;
                    sbStart.AppendLine("</TR>");
                }


                for (int j = 0; j < MaxPageRow - ItemCount; j++) {
                    sbStart.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ;border-top:thin\"  width=\"250\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD  style=\"border-bottom:thin ;border-top:thin\" width=\"150\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ;border-top:thin\" width=\"150\"  ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine("</TR>");
                }
            }
            else if (ItemCount > 3) {
                for (int i = 0; i < ItemCount; i++) {
                    sbStart.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");

                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ;border-top:thin\"  ALIGN = \"LEFT\"><b>{0}{1}</b></TD>", orderDet[i].description, "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ;border-top:thin\" width=\"150\" ALIGN = \"RIGHT\"><b>{0}{1} </b></TD>", orderDet[i].quantity.ToString(), "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ;border-top:thin \" width=\"150\" ALIGN = \"RIGHT\"><b>{0}{1} </b></TD>", orderDet[i].from_gwt.ToString(), "&nbsp"));

                    sbStart.AppendLine("</TR>");
                    Pcs += orderDet[i].quantity;
                    Gwt += orderDet[i].from_gwt;
                }


                for (int j = 0; j < 10 - ItemCount; j++) {
                    sbStart.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                    sbStart.AppendLine(string.Format("<TD  style=\"border-bottom:thin ;border-top:thin\" width=\"250\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ;border-top:thin \" width=\"150\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ;border-top:thin \" width=\"150\"  ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine("</TR>");
                }
            }

            sbStart.AppendLine("<TR  bgcolor='#FFFACD'  style=\"color:black; text-decoration:bold;\">");
            sbStart.AppendLine("<TH  width=\"250\"  ALIGN = \"LEFT\"><b>Totals</b></TH>");
            if (Pcs > 0) {
                sbStart.AppendLine(string.Format("<TH  width=\"150\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TH>", Pcs, "&nbsp"));
            }
            else {
                sbStart.AppendLine(string.Format("<TH  width=\"150\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TH>", "1", "&nbsp"));

            }
            sbStart.AppendLine(string.Format("<TH  width=\"150\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TH>", Gwt, "&nbsp"));

            sbStart.AppendLine("</TR>");
            sbStart.AppendLine("</TABLE>");


            for (int j = 0; j < 2; j++) {
                sbStart.AppendLine("<TR style=border:0>");
                sbStart.AppendLine(string.Format("<TD style=border:0 ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                sbStart.AppendLine("</TR>");
            }

            int settingID = SIGlobals.Globals.GetConfigurationValue(db, "20072019", companyCode, branchCode);
            if (settingID == 0) {
                #region Payments
                if (payments != null && payments.Count > 0) {
                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin\" width=\"750\" colspan = 4 ALIGN = \"Left\"><b>{0}<br></b></TD>", "Payment Details"));
                    sbStart.AppendLine("</TR>");
                    sbStart.AppendLine("<Table font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse;\" width=\"750\">");
                    sbStart.AppendLine("<TR  bgcolor='#FFFACD'>");
                    sbStart.AppendLine("<TH style=\"border-bottom:thin solid\" colspan=2 ALIGN = \"CENTER\">Mode</TH>");
                    if (payments[0].pay_mode.Equals("Q")) {
                        sbStart.AppendLine("<TH style=\"border-bottom:thin solid\" colspan=2  ALIGN = \"CENTER\">QNo/Bank</TH>");
                    }
                    else {
                        sbStart.AppendLine("<TH style=\"border-bottom:thin solid\" colspan=2  ALIGN = \"CENTER\">Details</TH>");
                    }
                    sbStart.AppendLine("<TH style=\"border-bottom:thin solid\" colspan=4 ALIGN = \"CENTER\">Amount</TH>");
                    sbStart.AppendLine("</TR>");

                    var chitPayments = payments.Where(pay => pay.pay_mode == "CT").ToList();

                    bool print = false;
                    decimal chitAmount = 0, CardCharges = 0;
                    string definition = string.Empty;
                    string schemeCode = string.Empty, ChitNo = string.Empty, ChitAmt = string.Empty;
                    for (int k = 0; k < chitPayments.Count; k++) {
                        definition += string.Format("{0}/{1}/{2} ", chitPayments[k].scheme_code, chitPayments[k].group_code, chitPayments[k].Ref_BillNo);
                        chitAmount += Convert.ToDecimal(chitPayments[k].pay_amt);

                        if (chitPayments.Count == 1 || (chitPayments.Count > 1 && k > 0)) {
                            if ((chitPayments.Count == 1) || (chitPayments.Count == k + 1) || !(chitPayments[k - 1].receipt_no.Equals(chitPayments[k].receipt_no))) {
                                print = true;
                            }
                        }

                        if (k < chitPayments.Count - 1 && (Convert.ToInt32(chitPayments[k].Ref_BillNo) != (Convert.ToInt32(chitPayments[k + 1].Ref_BillNo) - 1)
                            || !(chitPayments[k + 1].scheme_code.Equals(chitPayments[k].scheme_code)) || !(chitPayments[k + 1].group_code.Equals(chitPayments[k].group_code)))) {
                            print = true;
                        }


                        if (print) {
                            string[] chiNoPatch = definition.Split(' ');
                            definition = chiNoPatch[0];
                            if (chiNoPatch.Length > 2) {
                                definition += "-" + chiNoPatch[chiNoPatch.Length - 2];
                            }
                            string payMode = chitPayments[k].pay_mode;
                            KTTS_PAYMENT_MASTER payMaster = db.KTTS_PAYMENT_MASTER.Where(pn => pn.company_code == companyCode && pn.branch_code == branchCode && pn.payment_code == payMode).FirstOrDefault();
                            sbStart.AppendLine("<TR style=\"color:black; text-decoration:bold;\">");
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}{1}{1}</b></TD>", payMaster.payment_name, "&nbsp"));

                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}</b></TD>", definition));
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=4 ALIGN = \"RIGHT\"><b>{0}</b></TD>", chitAmount));
                            sbStart.AppendLine("<TR>");
                            definition = schemeCode = ChitNo = ChitAmt = string.Empty;
                            chitAmount = 0;
                            print = false;
                        }

                    }

                    string patch = string.Empty;
                    int ModeCount = payments.Count;
                    if (ModeCount <= 10) {
                        for (int j = 0; j < ModeCount; j++) {

                            if (!payments[j].pay_mode.Equals("CT")) {
                                sbStart.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                                string payMode = payments[j].pay_mode;
                                KTTS_PAYMENT_MASTER payMaster = db.KTTS_PAYMENT_MASTER.Where(pn => pn.company_code == companyCode && pn.branch_code == branchCode && pn.payment_code == payMode).FirstOrDefault();
                                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}{1}{1}</b></TD>", payMaster.payment_name, "&nbsp"));
                                if (payments[j].pay_mode.Equals("R")) {
                                    string cardDetails = string.Empty;

                                    if (!(string.IsNullOrEmpty(payments[j].Ref_BillNo)) && Convert.ToDecimal(payments[j].Ref_BillNo) > 0)
                                        cardDetails = payments[j].Ref_BillNo;
                                    if (!string.IsNullOrEmpty(payments[j].bank_name))
                                        cardDetails += "/" + payments[j].bank_name;
                                    if (!string.IsNullOrEmpty(payments[j].bank))
                                        cardDetails += "/" + payments[j].bank;
                                    if (!string.IsNullOrEmpty(payments[j].card_app_no))
                                        cardDetails += "/" + payments[j].card_app_no;

                                    if (cardDetails.Length < 44)

                                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}{1}{1}</b></TD>", cardDetails, "&nbsp"));

                                    else {
                                        List<string> lstCardDetails = SIGlobals.Globals.SplitStringAt(35, cardDetails);
                                        if ((lstCardDetails != null && lstCardDetails.Count > 0) && lstCardDetails == null)
                                            for (int x = 0; x < lstCardDetails.Count; x++)
                                                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"> {0} <br></TD>", lstCardDetails[x]));

                                        lstCardDetails = null;
                                    }
                                }
                                else if (payments[j].pay_mode.Equals("Q")) {
                                    string chequeDetails = string.Empty;
                                    if (!string.IsNullOrEmpty(payments[j].bank.ToString()))

                                        chequeDetails = "/" + payments[j].bank;

                                    if (!(string.IsNullOrEmpty(payments[j].cheque_no)) && Convert.ToDecimal(payments[j].cheque_no) > 0)
                                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}</b></TD>", payments[j].cheque_no + chequeDetails, " &nbsp"));

                                    else
                                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}</b></TD>", "&nbsp"));

                                }
                                else if (payments[j].pay_mode.Equals("C")) {
                                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}</b></TD>", payments[j].pay_details));
                                }
                                else {
                                    if (!(string.IsNullOrEmpty(payments[j].Ref_BillNo.ToString())) && Convert.ToDecimal(payments[j].Ref_BillNo) > 0)

                                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}</b></TD>", branchCode + "/" + payments[j].Ref_BillNo, " &nbsp"));
                                    else
                                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}</b></TD>", payments[j].pay_details));
                                }

                                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=4  ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", payments[j].pay_amt, "&nbsp"));
                            }
                            sbStart.AppendLine("</TR>");
                        }

                        for (int j = 0; j < MaxPageRow - ModeCount; j++) {
                            sbStart.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                            sbStart.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-top:thin\"  colspan=2 ALIGN = \"LEFT\"><b>{0} </b></TD>", "&nbsp"));
                            sbStart.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-top:thin\"  colspan=2 ALIGN = \"LEFT\"><b>{0} </b></TD>", "&nbsp"));
                            sbStart.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-top:thin\"  colspan=4 ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                            sbStart.AppendLine("</TR>");
                        }
                    }



                    decimal TotalAmt = Convert.ToDecimal(payments.Sum(pay => pay.pay_amt));
                    sbStart.AppendLine("<TR  bgcolor='#FFFACD'  style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                    sbStart.AppendLine("<TH  style=\"border-top:thin solid \" colspan=4 ALIGN = \"LEFT\"><b>Totals :</b></TH>");
                    sbStart.AppendLine(string.Format("<TH  style=\"border-top:thin solid \" colspan=4 ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TH>", TotalAmt, "&nbsp"));
                    sbStart.AppendLine("</TR>");




                    decimal cgst = 0, sgst = 0, igst = 0, amt = 0, TaxableAmount = 0;
                    if (payments.Count > 0) {
                        cgst = Math.Round(Convert.ToDecimal(payments.Sum(pay => pay.CGST_Amount)), 2);
                        sgst = Math.Round(Convert.ToDecimal(payments.Sum(pay => pay.SGST_Amount)), 2);
                        igst = Math.Round(Convert.ToDecimal(payments.Sum(pay => pay.IGST_Amount)), 2);
                        amt = Math.Round(Convert.ToDecimal(payments.Sum(pay => pay.pay_amt)), 2);
                        TaxableAmount = amt - (cgst + sgst + igst);
                    }

                    sbStart.AppendLine("</TR>");
                    sbStart.AppendLine("</Table >");
                    sbStart.AppendLine("<Table bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\" border-collapse:collapse;  font-size=9pt;   \" width=\"750\">");
                    sbStart.AppendLine("<TR>");
                    if (TaxableAmount > 0) {
                        //sbStart.AppendLine(string.Format("<TD colspan=4  ALIGN = \"Left\"><b></b></TD>"));
                        sbStart.AppendLine(string.Format("<TD colspan=4   ALIGN = \"right\"><b>Taxable Amount : {0} </b></TD>", TaxableAmount));
                    }

                    sbStart.AppendLine("</TR>");
                    sbStart.AppendLine("</TR>");
                    Amount = TaxableAmount;

                    CardCharges = Convert.ToDecimal(payments.Sum(pay => pay.CardCharges));
                    if (CardCharges > 0) {
                        sbStart.AppendLine("<TR>");
                        sbStart.AppendLine(string.Format("<TD  colspan=4  ALIGN = \"RIGHT\"><b>{0}</b></TD><TD  colspan=1  ALIGN = \"RIGHT\"><b>{1}</b></TD> ", "Card Charges : ", CardCharges + "&nbsp"));
                        sbStart.AppendLine("</TR>");
                    }
                    sbStart.AppendLine("</TABLE>");
                }

                #endregion
            }

            string strWord = string.Empty;
            NumberToWords objNWClas = new NumberToWords();
            objNWClas.ConvertNumberToWords(Convert.ToDouble(Amount), out strWord);

            sbStart.AppendLine("<Table font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse;\" width=\"750\">");

            sbStart.AppendLine("<TR>");
            sbStart.AppendLine(string.Format("<TD style=\" border-bottom:thin;\"   colspan=5 ALIGN = \"LEFT\"><b>OPR: {0}  , Sal Code: {1} </b></TD>", operator_code, Sal_Code));
            sbStart.AppendLine("</TR>");



            sbStart.AppendLine("<TR>");
            sbStart.AppendLine(string.Format("<TD  style=\" border-bottom:thin; border-top:thin\"   colspan=5 ALIGN = \"LEFT\"><b>Amount: {0}{1}{1}</b></TD>", strWord, "&nbsp"));
            sbStart.AppendLine("</TR>");

            if (!string.IsNullOrEmpty(order.remarks)) {
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\" border-bottom:thin;\"   colspan=5 ALIGN = \"LEFT\"><b>Remarks: {0} </b></TD>", order.remarks.ToString()));
                sbStart.AppendLine("</TR>");
            }
            sbStart.AppendLine("<TR>");
            sbStart.AppendLine(string.Format("<TD style=\"border-right:thin\" colspan = 4 ALIGN = \"LEFT\"><b>{0}<br><br><br>{1}</b></TD>", DisplayType, "Customer Signature"));
            sbStart.AppendLine(string.Format("<TD style=\"border-left:thin\" colspan = 4 ALIGN = \"RIGHT\"><b>For {0}<br><br><br>{1}</b></TD>", companyMaster.company_name, "Authorized Signatory"));
            sbStart.AppendLine("</TR>");
            sbStart.AppendLine("</TABLE>");
            sbStart.AppendLine("</body>");
            sbStart.AppendLine("</html>");
            sbStart.AppendLine();
            return sbStart.ToString();
        }

        public string GetOrderPrintDotmatrix(string companyCode, string branchCode, int orderNo, out ErrorVM error)
        {
            error = null;
            KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(com => com.company_code == companyCode && com.branch_code == branchCode).FirstOrDefault();
            KTTU_ORDER_MASTER order = db.KTTU_ORDER_MASTER.Where(ord => ord.company_code == companyCode && ord.branch_code == branchCode && ord.order_no == orderNo).FirstOrDefault();
            if (order == null || order.order_no == 0) {
                return "";
            }
            List<KTTU_ORDER_DETAILS> orderDet = db.KTTU_ORDER_DETAILS.Where(ord => ord.order_no == orderNo && ord.branch_code == branchCode && ord.company_code == companyCode).ToList();
            KSTU_CUSTOMER_MASTER customer = db.KSTU_CUSTOMER_MASTER.Where(cust => cust.cust_id == order.Cust_Id && cust.company_code == companyCode && cust.branch_code == branchCode).FirstOrDefault();
            try {
                StringBuilder sb = new StringBuilder();
                sb.Append(Convert.ToChar(18).ToString());
                int width = 90;

                StringBuilder strdoubleTransLine = new StringBuilder();
                strdoubleTransLine.Append('=', width);
                StringBuilder strTransLine = new StringBuilder();
                strTransLine.Append('-', width);
                decimal Spaces = 0M;
                StringBuilder strSpaces = new StringBuilder();
                Spaces = ((width - strTransLine.Length) / 3);
                strSpaces.Append(' ', Convert.ToInt32(Spaces));
                sb.AppendLine();
                sb.Append(string.Format("{0}{1}", "TIN: ", company.tin_no));
                string esc = Convert.ToChar(27).ToString();

                string dupliCateOrderString = string.Empty;
                if (!order.order_date.Equals(SIGlobals.Globals.GetApplicationDate(companyCode, branchCode))) {
                    dupliCateOrderString = "(Duplicate)";
                }

                sb.Append(SIGlobals.Globals.GetCompanyDetailsForHTMLPrint(db, companyCode, branchCode));
                if (string.Compare(order.cflag, "Y") == 0) {
                    sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), "ORDER FORM/CANCELLED " + dupliCateOrderString));
                }
                else {
                    sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), "ORDER FORM " + dupliCateOrderString));
                }
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));

                if (string.Compare("1", "1") == 0) {
                    sb.AppendLine(string.Format("{0,-50}{1,40}", "Order No      " + ": " + companyCode + "/" + order.order_no.ToString()
                        , "Order Date :" + order.order_date.ToString("dd/M/yyyy")));
                }
                else {
                    sb.AppendLine(string.Format("{0,-50}{1,40}", "Order No      " + ": " + order.order_no.ToString()
                       , "Order Date :" + order.order_date.ToString("dd/M/yyyy")));
                }
                sb.AppendLine(string.Format("{0,-50}{1,40}", "Received from " + ": " + customer.salutation.ToString() + customer.cust_name.ToString()
                    , "Due Date :" + order.delivery_date.ToString("dd/M/yyyy")));
                if (!string.IsNullOrEmpty(customer.address1))
                    sb.AppendLine(string.Format("{0,-90}", "Address       " + ": " + customer.address1.ToString().Trim()));
                if (!string.IsNullOrEmpty(customer.address2))
                    sb.AppendLine(string.Format("{0,-90}", "              " + ": " + customer.address2.ToString().Trim()));
                if (!string.IsNullOrEmpty(customer.address3))
                    sb.AppendLine(string.Format("{0,-90}", "              " + ": " + customer.address3.ToString().Trim()));

                if (!string.IsNullOrEmpty(customer.phone_no) && !string.IsNullOrEmpty(customer.mobile_no.ToString())) {
                    sb.Append(string.Format("{0,-45}", "PH            " + ": " + customer.phone_no.ToString()));
                    sb.Append(string.Format("{0,45}", "Mobile        " + ": " + customer.mobile_no.ToString()));
                    sb.AppendLine();
                }
                else {
                    if (!string.IsNullOrEmpty(customer.phone_no))
                        sb.Append(string.Format("{0,-90}", "PH            " + ": " + customer.phone_no.ToString()));

                    else if (!string.IsNullOrEmpty(customer.mobile_no))
                        sb.Append(string.Format("{0,-90}", "Mobile        " + ": " + customer.mobile_no.ToString()));
                    sb.AppendLine();
                }

                if (!string.IsNullOrEmpty(order.remarks.ToString()))
                    sb.AppendLine(string.Format("{0,-90}", "Remarks       : " + order.remarks.ToString()));

                sb.AppendLine();

                if (!string.IsNullOrEmpty(order.rate.ToString()))
                    sb.Append(string.Format(new CultureInfo(company.Currency_code), "{0:N,-15}", "Rate: " + order.rate.ToString()));
                else
                    sb.Append(string.Format("{0,-15}", ""));

                sb.Append(string.Format(new CultureInfo(company.Currency_code), "{0:N,-15}", "Advance: " + order.advance_ord_amount.ToString()));
                sb.Append(string.Format("{0,10}", " OPR: " + order.operator_code.ToString()));
                sb.Append(string.Format("{0,-10}", ", SAL: " + order.sal_code.ToString()));

                if (string.Compare(order.karat.ToString(), "NA") != 0)
                    sb.Append(string.Format("{0,-10}", ", (" + order.karat.ToString() + ")"));
                else
                    sb.Append(string.Format("{0,-10}", ""));

                if (Convert.ToDecimal(order.rate.ToString()) > 0)
                    sb.AppendLine(string.Format(new CultureInfo(company.Currency_code), "{0:N,20}", "Rate Accepted"));
                else
                    sb.AppendLine(string.Format(new CultureInfo(company.Currency_code), "{0:N,20}", "Delivery Rate"));


                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));

                string itemName = string.Empty;
                List<string> lstItemName = null;
                for (int i = 0; i < orderDet.Count; i++) {
                    string Descp = orderDet[i].description.ToString();
                    itemName = orderDet[i].item_name.ToString();

                    if (Descp.Length <= 89)
                        sb.Append(string.Format("{0,-90}", i + 1 + ")" + Descp));
                    else {
                        string[] Descwtspace = Descp.Split(' ');
                        if (Descwtspace.Length == 1) {
                            lstItemName = SIGlobals.Globals.SplitStringAt(89, Descp);
                            if (lstItemName != null && lstItemName.Count > 1) {
                                string ItemNames = string.Empty;
                                for (int x = 0; x < lstItemName.Count; x++)
                                    ItemNames = ItemNames + " " + lstItemName[x].Trim();
                                lstItemName = null;
                                sb.Append(string.Format("{0,-90}", i + 1 + ")" + ItemNames));
                            }
                        }
                        else {
                            sb.Append(string.Format("{0,-90}", i + 1 + ")" + Descp));
                        }
                    }

                    sb.AppendLine();
                    if (lstItemName != null && lstItemName.Count != 0) {
                        for (int x = 1; x < lstItemName.Count; x++)
                            sb.AppendLine(string.Format("-{0,-89}", i + 1 + ")" + lstItemName[x].Trim()));
                        lstItemName = null;
                        itemName = string.Empty;
                        sb.AppendLine();
                    }
                }

                sb.AppendLine();

                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                sb.AppendLine(string.Format(new CultureInfo(company.Currency_code), "{0,-12}{1,-12}{2,-12}{3,-36}{4:N,18}", "Date", "Receipt No.", "Mode", "Details", "Amount"));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));

                List<KTTU_PAYMENT_DETAILS> payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == orderNo && pay.trans_type == "O" && pay.company_code == companyCode && pay.branch_code == branchCode).ToList();
                #region ChitPrint.
                List<KTTU_PAYMENT_DETAILS> chitPay = payment.Where(pay => pay.pay_mode == "CT").ToList();
                bool print = false;
                decimal chitAmount = 0;
                string definition = string.Empty;
                string schemeCode = string.Empty, ChitNo = string.Empty, ChitAmt = string.Empty;
                for (int k = 0; k < chitPay.Count; k++) {
                    definition += string.Format("{0}/{1}/{2} ", chitPay[k].scheme_code, chitPay[k].group_code, chitPay[k].Ref_BillNo);
                    chitAmount += Convert.ToDecimal(chitPay[k].pay_amt);

                    if (chitPay.Count == 1 || (chitPay.Count > 1 && k > 0)) {
                        if ((chitPay.Count == 1) || (chitPay.Count == k + 1) || !(chitPay[k - 1].receipt_no.Equals(chitPay[k].receipt_no))) {
                            print = true;
                        }
                    }

                    if (k < chitPay.Count - 1 && (Convert.ToInt32(chitPay[k].Ref_BillNo) != (Convert.ToInt32(chitPay[k + 1].Ref_BillNo) - 1)  //)
                        || !(chitPay[k + 1].scheme_code.Equals(chitPay[k].scheme_code)) || !(chitPay[k + 1].group_code.Equals(chitPay[k].group_code)))) {
                        print = true;
                    }
                    if (print) {
                        string[] chiNoPatch = definition.Split(' ');
                        definition = chiNoPatch[0];
                        if (chiNoPatch.Length > 2) {
                            definition += "-" + chiNoPatch[chiNoPatch.Length - 2];
                        }
                        sb.Append(string.Format("{0,-14}", chitPay[k].pay_date));
                        sb.Append(string.Format("{0,-10}", chitPay[k].receipt_no));
                        sb.Append(string.Format("{0,-12}", SIGlobals.Globals.PaymentMode(chitPay[k].pay_mode)));
                        sb.Append(string.Format("{0,-36}", definition));
                        sb.AppendLine(string.Format(new CultureInfo(company.Currency_code), "{0,18}", chitAmount));

                        definition = schemeCode = ChitNo = ChitAmt = string.Empty;
                        chitAmount = 0;
                        print = false;
                    }
                }
                #endregion
                string patch = string.Empty;
                for (int j = 0; j < payment.Count; j++) {
                    if (!payment[j].pay_mode.Equals("CT")) {
                        sb.Append(string.Format("{0,-12}", Convert.ToDateTime(payment[j].pay_date).ToString("dd/M/yyyy")));
                        sb.Append(string.Format("{0,-12}", companyCode + "/" + payment[j].receipt_no));
                        sb.Append(string.Format("{0,-12}", SIGlobals.Globals.PaymentMode(payment[j].pay_mode)));

                        if (payment[j].pay_mode.Equals("R")) {
                            string cardDetails = string.Empty;

                            if (!(string.IsNullOrEmpty(payment[j].Ref_BillNo.ToString())) && Convert.ToDecimal(payment[j].Ref_BillNo) > 0)
                                cardDetails = payment[j].Ref_BillNo.ToString();
                            if (!string.IsNullOrEmpty(payment[j].bank_name.ToString()))
                                cardDetails += "/" + payment[j].bank_name;
                            if (!string.IsNullOrEmpty(payment[j].card_app_no.ToString()))
                                cardDetails += "/" + payment[j].card_app_no;
                            if (cardDetails.Length < 35)
                                sb.Append(string.Format("{0,-36}", cardDetails));
                            else {
                                List<string> lstCardDetails = SIGlobals.Globals.SplitStringAt(5, cardDetails);
                                if (lstCardDetails != null && lstCardDetails.Count > 0) {
                                    sb.AppendLine(string.Format("{0,-36}-{1,-35}", lstCardDetails[0], ""));
                                    for (int x = 1; x < lstCardDetails.Count; x++)
                                        sb.Append(string.Format("{0,-36}-{1,-35}", "", lstCardDetails[x]));
                                }
                                lstCardDetails = null;
                            }
                        }
                        else if (payment[j].pay_mode.Equals("Q")) {
                            string chequeDetails = string.Empty;
                            if (!string.IsNullOrEmpty(payment[j].bank))
                                chequeDetails = "/" + payment[j].bank;

                            if (!(string.IsNullOrEmpty(payment[j].cheque_no.ToString())) && Convert.ToDecimal(payment[j].cheque_no) > 0)
                                sb.Append(string.Format("{0,-36}", payment[j].cheque_no + chequeDetails));
                            else
                                sb.Append(string.Format("{0,-36}", ""));
                        }


                        else {
                            if (!(string.IsNullOrEmpty(payment[j].Ref_BillNo)) && Convert.ToDecimal(payment[j].Ref_BillNo) > 0)
                                sb.Append(string.Format("{0,-36}", companyCode + "/" + payment[j].Ref_BillNo.ToString()));
                            else
                                sb.Append(string.Format("{0,-36}", ""));
                        }

                        sb.AppendLine(string.Format(new CultureInfo(company.Currency_code), "{0,18}", payment[j].pay_amt));
                    }
                }

                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                sb.Append(Convert.ToChar(27).ToString() + Convert.ToChar(69).ToString());

                decimal advancePaid = 0, CardCharges = 0;
                object total = payment.Sum(p => p.pay_amt);
                if (Convert.ToDecimal(total) != 0 && total != null)
                    advancePaid = Convert.ToDecimal(total);

                sb.AppendLine(string.Format(new CultureInfo(company.Currency_code), "{0,3}{1,-21}{2,12}{3,36}{4,18}"
                    , "", "Total Advance", "", "", advancePaid));
                sb.Append(Convert.ToChar(27).ToString() + Convert.ToChar(70).ToString());
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));

                //string gstAmount = string.Format("select isnull(sum(sgst_amount),0) as sgst_amount,isnull(sum(cgst_amount),0) as cgst_amount,isnull(sum(igst_amount),0) as igst_amount, isnull(sum(pay_amt),0) as amt  from KTTU_PAYMENT_DETAILS where trans_type='O'and series_no={0} and company_code='{1}' and branch_code='{2}' ", OrderNo, CGlobals.CompanyCode, CGlobals.BranchCode);
                //DataTable dtGst = CGlobals.GetDataTable(gstAmount);

                decimal cgst = 0, sgst = 0, igst = 0, amt = 0, TaxableAmount = 0;
                if (payment.Count > 0) {
                    cgst = Math.Round(Convert.ToDecimal(payment.Sum(p => p.CGST_Amount)), 2);
                    sgst = Math.Round(Convert.ToDecimal(payment.Sum(p => p.SGST_Amount)), 2);
                    igst = Math.Round(Convert.ToDecimal(payment.Sum(p => p.IGST_Amount)), 2);
                    amt = Math.Round(Convert.ToDecimal(payment.Sum(p => p.pay_amt)), 2);
                    TaxableAmount = amt - (cgst + sgst + igst);
                }
                sb.Append(string.Format("{0}{1}", "Taxable Amt:", TaxableAmount + " "));
                sb.Append(string.Format("{0}{1}", "CGST 1.5%:", cgst + " "));
                sb.Append(string.Format("{0}{1}", "SGST 1.5%:", sgst + " "));
                sb.Append(string.Format("{0}{1}", "IGST 3%:", igst + " "));
                sb.AppendLine(string.Format("{0}{1}", "Total Amount:", amt));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));

                object temp1 = payment.Sum(p => p.CardCharges);
                if (temp1 != null && temp1 != DBNull.Value)
                    CardCharges = Convert.ToDecimal(temp1);
                if (CardCharges > 0) {
                    sb.AppendLine(string.Format(new CultureInfo(company.Currency_code), "{0:N,-90}", "Card Charges : " + CardCharges));
                }
                sb.Append(Convert.ToChar(27).ToString() + Convert.ToChar(69).ToString());
                string strWords = string.Empty;
                NumberToWords objNWClass = new NumberToWords();
                objNWClass.ConvertNumberToWords(Convert.ToDouble(advancePaid), out strWords);
                sb.AppendLine(string.Format("{0,3}{1,-87}", "", strWords));
                sb.Append(Convert.ToChar(27).ToString() + Convert.ToChar(70).ToString());

                string Notes = string.Format("select line1,line2,line3,line4,line5,line6,line7,line8,line9,line10 from KTTU_NOTE Where note_type = 'ORD' and company_code = '{0}' and branch_code = '{1}'", companyCode, branchCode);
                DataTable dtFillNotes = SIGlobals.Globals.ExecuteQuery(Notes);
                if (dtFillNotes != null && dtFillNotes.Rows.Count > 0) {
                    for (int i = 0; i < 1; i++) {
                        if (dtFillNotes.Rows[0]["line1"].ToString() != string.Empty)
                            sb.AppendLine(string.Format("{0,-90}", dtFillNotes.Rows[i]["line1"]));
                        sb.AppendLine();
                        if (Convert.ToInt32(order.bill_no) > 0)
                            sb.AppendLine(string.Format("{0,-90}", "Order is Already Closed towords Bill NO :" + Convert.ToInt32(order.bill_no)));
                        sb.AppendLine();
                        sb.AppendLine();
                        // sb.AppendLine(string.Format("{0,90}", "Your Faithfully"));                    
                        sb.Append(string.Format("{0,-45}", " Authorised Signature"));
                        sb.Append(string.Format("{0,45}", "(Customer Signature)"));
                        sb.AppendLine();
                        sb.AppendLine();
                        if (dtFillNotes.Rows[0]["line1"].ToString() != string.Empty)
                            sb.AppendLine(string.Format("{0,-90}", dtFillNotes.Rows[i]["line2"]));
                        if (dtFillNotes.Rows[0]["line2"].ToString() != string.Empty)
                            sb.AppendLine(string.Format("{0,-90}", dtFillNotes.Rows[i]["line3"]));
                        if (dtFillNotes.Rows[0]["line3"].ToString() != string.Empty)
                            sb.AppendLine(string.Format("{0,-90}", dtFillNotes.Rows[i]["line4"]));
                        if (dtFillNotes.Rows[0]["line4"].ToString() != string.Empty)
                            sb.AppendLine(string.Format("{0,-90}", dtFillNotes.Rows[i]["line5"]));
                        if (dtFillNotes.Rows[0]["line5"].ToString() != string.Empty)
                            sb.AppendLine(string.Format("{0,-90}", dtFillNotes.Rows[i]["line6"]));
                        if (dtFillNotes.Rows[0]["line6"].ToString() != string.Empty)
                            sb.AppendLine(string.Format("{0,-90}", dtFillNotes.Rows[i]["line7"]));
                    }
                }
                sb.Append(Convert.ToChar(18).ToString());
                return sb.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        public string GetReceiptDetailsPrintHTML(string companyCode, string branchCode, int receiptNo, out ErrorVM error)
        {

            error = null;
            try {
                KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode
                                                                            && c.branch_code == branchCode).FirstOrDefault();
                List<KTTU_PAYMENT_DETAILS> receiptDet = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.company_code == companyCode
                                                                                 && pay.branch_code == branchCode
                                                                                 && pay.receipt_no == receiptNo
                                                                                 && pay.trans_type == "O").ToList();
                int orderNo = receiptDet[0].series_no;
                KTTU_ORDER_MASTER master = db.KTTU_ORDER_MASTER.Where(ord => ord.company_code == companyCode
                                                                       && ord.branch_code == branchCode
                                                                       && ord.order_no == orderNo).FirstOrDefault();
                if (receiptDet == null && receiptDet.Count == 0) {
                    error = new ErrorVM()
                    {
                        description = "Invalid receipt Number",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                    return "";
                }

                decimal TotalAmt = 0;
                string TotalAmtInWords = string.Empty;
                string pay_mode = string.Empty;
                NumberToWords objNWClass = new NumberToWords();
                objNWClass.ConvertNumberToWords(Convert.ToDouble(TotalAmt), out TotalAmtInWords);

                string CompanyAddress = string.Empty;
                CompanyAddress = company.address1;
                CompanyAddress = CompanyAddress + "<br>" + company.address2;
                CompanyAddress = CompanyAddress + "<br>" + company.address3;
                CompanyAddress = CompanyAddress + "<br>" + "Phone no: " + company.phone_no;
                CompanyAddress = CompanyAddress + "<br> E-mail : " + company.email_id;
                CompanyAddress = CompanyAddress + "<br> Website : " + company.website;

                StringBuilder sbStart = new StringBuilder();
                sbStart.AppendLine("<html>");
                sbStart.AppendLine("<head>");
                sbStart.AppendLine(SIGlobals.Globals.GetStyleOrder());
                sbStart.AppendLine("</head>");
                sbStart.AppendLine("<body>");

                string strWords = string.Empty;
                string DateTime = string.Format("{0:dd/MM/yyyy} {0:T}", Convert.ToDateTime(receiptDet[0].pay_date));
                string OrderDate = string.Format("{0:dd/MM/yyyy} {0:T}", Convert.ToDateTime(master.order_date));

                sbStart.AppendLine("<Table frame=\"border\" border=\"0\" width=\"700\">");
                for (int j = 0; j < 3; j++) {
                    sbStart.AppendLine("<TR style=border:0>");
                    sbStart.AppendLine(string.Format("<TD style=border:0 colspan = 10 ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    sbStart.AppendLine("</TR>");
                }
                sbStart.AppendLine("</Table>");
                sbStart.AppendLine("<Table font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"0\"; border-collapse:collapse; width=\"750\">");

                string Sultation = master.salutation;
                string Name = master.cust_name;
                string Address1 = master.address1;
                string Address2 = master.address2;
                string Address3 = master.address3;
                string Email = master.Email_ID;
                string Mobile = master.phone_no;
                string Phone = master.phone_no;
                string PAN = master.pan_no;
                string state = master.state;
                string stateCode = Convert.ToString(master.state_code);
                string CustGSTIN = Convert.ToString(master.tin);

                if (!string.IsNullOrEmpty(Mobile) && !string.IsNullOrEmpty(Phone))
                    Mobile = Mobile + "/" + Phone;
                if (string.IsNullOrEmpty(Mobile))
                    Mobile = Phone;

                string City = master.city;
                string Address = string.Empty;
                if (Address1 != string.Empty)
                    Address = Address + "<br>" + Address1;
                if (Address2 != string.Empty)
                    Address = Address + "<br>" + Address2;
                if (Address3 != string.Empty)
                    Address = Address + "<br>" + Address3;
                if (Email != string.Empty)
                    Email = Email + "<br>";


                sbStart.AppendLine("<TR align=\"CENTER\">");
                if (string.Compare(master.cflag, "Y") == 0) {
                    sbStart.AppendLine("<TD style=\"border-right:thin\" ALIGN = \"CENTER\"><b>ORDER RECEIPT/CANCELLED</b></TD>");
                }
                else {
                    sbStart.AppendLine("<TD style=\"border-right:thin\" ALIGN = \"CENTER\"><b>ORDER RECEIPT</b></TD>");
                }
                sbStart.AppendLine("</TR>");
                for (int j = 0; j < 1; j++) {
                    sbStart.AppendLine("<TR style=border:0>");
                    sbStart.AppendLine(string.Format("<TD style=border:0 colspan = 10 ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    sbStart.AppendLine("</TR>");
                }

                sbStart.AppendLine("<Table font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\" border-collapse:collapse; \" width=\"750\">");
                sbStart.AppendLine("<TR  bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\"  style=\"border-left:thin\">");
                sbStart.AppendLine(string.Format("<TD width=\"250\" ALIGN = \"LEFT\"><b>CUSTOMER DETAILS</b></TD>"));
                sbStart.AppendLine(string.Format("<TD width=\"200\" ALIGN = \"CENTER\"><b>GSTIN : {0}</b></TD>", company.tin_no));
                sbStart.AppendLine(string.Format("<TD width=\"150\" ALIGN = \"LEFT\"><b>{0}</b></TD>", "SHOWROOM ADDRESS"));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<tr>");
                sbStart.AppendLine("<td>");
                sbStart.AppendLine("<Table>");
                sbStart.AppendLine("<Table font-size=12pt;  class=\"boldText\"  style=\"border-collapse:collapse;\" >");
                sbStart.AppendLine("<tr style=\"border-right:0\"  >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left \" ><b>Name &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + Name + "</b></td>");
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" ><b>Address &nbsp&nbsp </b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + Address1 + "</b></td>");
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + Address2 + "</b></td>");
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + Address3 + "</b></td>");
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" ><b>State &nbsp&nbsp </b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + state + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>Code &nbsp&nbsp&nbsp </b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + stateCode + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>Phone &nbsp&nbsp </b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + Mobile + "</b></td>");
                sbStart.AppendLine("</tr>");
                if (!string.IsNullOrEmpty(PAN)) {
                    sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sbStart.AppendLine("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>PAN &nbsp&nbsp</b></td>");
                    sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + PAN + "</b></td>");
                    sbStart.AppendLine("</tr>");
                }
                else {
                    var customerIdProof = db.KSTU_CUSTOMER_ID_PROOF_DETAILS.Where(cust => cust.cust_id == master.Cust_Id
                                                                                                               && cust.company_code == companyCode
                                                                                                               && cust.branch_code == branchCode).ToList();
                    if (customerIdProof.Count > 0) {
                        if (!string.IsNullOrEmpty(customerIdProof[0].Doc_code)) {
                            sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                            sbStart.AppendLine(string.Format("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>{0} &nbsp&nbsp</b></td>", customerIdProof[0].Doc_code));
                            sbStart.AppendLine(string.Format("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>{0}</b></td>", customerIdProof[0].Doc_code));
                            sbStart.AppendLine("</tr>");
                        }
                    }
                }
                if (!string.IsNullOrEmpty(CustGSTIN)) {
                    sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sbStart.AppendLine("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>GSTIN &nbsp&nbsp</b></td>");
                    sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + CustGSTIN + "</b></td>");
                    sbStart.AppendLine("</tr>");
                }

                sbStart.AppendLine("</table>");
                sbStart.AppendLine("</td>");
                sbStart.AppendLine("<td>");
                sbStart.AppendLine("<Table font-size=12pt;   class=\"boldText\"  style=\"border-collapse:collapse;\" >");


                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" ><b>Receipt No &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin ; border-top:thin\" ><b>" + receiptNo + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" ><b>Receipt Date &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin ; border-top:thin\" ><b>" + DateTime + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" ><b>Order No &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin ; border-top:thin\" ><b>" + branchCode + "/" + "O" + "/" + orderNo + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" ><b>Order Date &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin ; border-top:thin\" ><b>" + OrderDate + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("</table>");
                sbStart.AppendLine("</td>");

                sbStart.AppendLine("<td>");
                sbStart.AppendLine("<Table font-size=12pt;   class=\"boldText\"  style=\"border-collapse:collapse;\" >");

                sbStart.AppendLine("<tr style=\"border-right:0\"  >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left \" ><b>Name &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.company_name + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>Address &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.address1 + "</b></td>");
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.address2 + "</b></td>");
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.address3 + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>City &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.city + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>State &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + company.state + "</b></td>");
                sbStart.AppendLine("</tr>");


                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>Code &nbsp&nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + Convert.ToString(company.state_code) + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>Phone &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + company.phone_no + "</b></td>");
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("</table>");
                sbStart.AppendLine("</td>");
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("</Table>");
                sbStart.AppendLine("<Table font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse;\" width=\"750\">");  //FRAME=BOX RULES=NONE
                sbStart.AppendLine("<TR  bgcolor='#FFFACD'>");
                sbStart.AppendLine("<TH style=\"border-bottom:thin solid\" colspan=2 ALIGN = \"CENTER\">Mode</TH>");

                if (receiptDet[0].pay_mode.Equals("Q")) {
                    sbStart.AppendLine("<TH style=\"border-bottom:thin solid\" colspan=2  ALIGN = \"CENTER\">QNo/Bank</TH>");
                }
                else {
                    sbStart.AppendLine("<TH style=\"border-bottom:thin solid\" colspan=2  ALIGN = \"CENTER\">Details</TH>");
                }
                sbStart.AppendLine("<TH style=\"border-bottom:thin solid\" colspan=4 ALIGN = \"CENTER\">Amount</TH>");
                sbStart.AppendLine("</TR>");

                int MaxPageRow = 4;
                var drChits = receiptDet.Where(pay => pay.pay_mode == "CT").ToList();
                bool print = false;
                decimal chitAmount = 0, CardCharges = 0;
                string definition = string.Empty;
                string schemeCode = string.Empty, ChitNo = string.Empty, ChitAmt = string.Empty;
                for (int k = 0; k < drChits.Count; k++) {
                    definition += string.Format("{0}/{1}/{2} ", drChits[k].scheme_code, drChits[k].group_code, drChits[k].Ref_BillNo);
                    chitAmount += Convert.ToDecimal(drChits[k].pay_amt);

                    if (drChits.Count == 1 || (drChits.Count > 1 && k > 0)) {
                        if ((drChits.Count == 1) || (drChits.Count == k + 1) || !(drChits[k - 1].receipt_no.Equals(drChits[k].receipt_no))) {
                            print = true;
                        }
                    }

                    if (k < drChits.Count - 1 && (Convert.ToInt32(drChits[k].Ref_BillNo) != (Convert.ToInt32(drChits[k + 1].Ref_BillNo) - 1)
                        || !(drChits[k + 1].scheme_code.Equals(drChits[k].scheme_code)) || !(drChits[k + 1].group_code.Equals(drChits[k].group_code)))) {
                        print = true;
                    }
                    if (print) {
                        string[] chiNoPatch = definition.Split(' ');
                        definition = chiNoPatch[0];
                        if (chiNoPatch.Length > 2) {
                            definition += "-" + chiNoPatch[chiNoPatch.Length - 2];
                        }
                        string payMode = drChits[k].pay_mode;
                        KTTS_PAYMENT_MASTER payMaster = db.KTTS_PAYMENT_MASTER.Where(pn => pn.company_code == companyCode && pn.branch_code == branchCode && pn.payment_code == payMode).FirstOrDefault();
                        sbStart.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}{1}{1}</b></TD>", payMaster.payment_name, "&nbsp"));
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}</b></TD>", definition + "&nbsp"));
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=4 ALIGN = \"RIGHT\"><b>{0}</b></TD>", chitAmount + "&nbsp"));
                        sbStart.AppendLine("<TR>");
                        definition = schemeCode = ChitNo = ChitAmt = string.Empty;
                        chitAmount = 0;
                        print = false;
                    }

                }

                string patch = string.Empty;
                int ModeCount = receiptDet.Count;
                if (ModeCount <= 10) {
                    for (int j = 0; j < ModeCount; j++) {

                        if (!receiptDet[j].pay_mode.Equals("CT")) {
                            string payMode = receiptDet[j].pay_mode;
                            KTTS_PAYMENT_MASTER payMaster = db.KTTS_PAYMENT_MASTER.Where(pn => pn.company_code == companyCode && pn.branch_code == branchCode && pn.payment_code == payMode).FirstOrDefault();
                            sbStart.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}{1}{1}</b></TD>", payMaster.payment_name, "&nbsp"));
                            if (receiptDet[j].pay_mode.Equals("R")) {
                                string cardDetails = string.Empty;

                                if (!(string.IsNullOrEmpty(receiptDet[j].Ref_BillNo.ToString())) && Convert.ToDecimal(receiptDet[j].Ref_BillNo.ToString()) > 0)
                                    cardDetails = receiptDet[j].Ref_BillNo.ToString();
                                if (!string.IsNullOrEmpty(receiptDet[j].bank_name.ToString()))
                                    cardDetails += "/" + receiptDet[j].bank_name;
                                if (!string.IsNullOrEmpty(receiptDet[j].bank.ToString()))
                                    cardDetails += "/" + receiptDet[j].bank;
                                if (!string.IsNullOrEmpty(receiptDet[j].card_app_no.ToString()))
                                    cardDetails += "/" + receiptDet[j].card_app_no;

                                if (cardDetails.Length < 44)

                                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}{1}{1}</b></TD>", cardDetails, "&nbsp"));

                                else {
                                    List<string> lstCardDetails = SIGlobals.Globals.SplitStringAt(35, cardDetails);
                                    if ((lstCardDetails != null && lstCardDetails.Count > 0) && lstCardDetails == null)
                                        for (int x = 0; x < lstCardDetails.Count; x++)
                                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"> {0} <br></TD>", lstCardDetails[x]));
                                    lstCardDetails = null;
                                }
                            }
                            else if (receiptDet[j].pay_mode.Equals("Q")) {
                                string chequeDetails = string.Empty;
                                if (!string.IsNullOrEmpty(receiptDet[j].bank.ToString()))
                                    chequeDetails = "/" + receiptDet[j].bank;

                                if (!(string.IsNullOrEmpty(receiptDet[j].cheque_no.ToString())) && Convert.ToDecimal(receiptDet[j].cheque_no.ToString()) > 0)
                                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}</b></TD>", receiptDet[j].cheque_no + chequeDetails + "&nbsp"));

                                else
                                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}</b></TD>", "&nbsp"));

                            }
                            else if (receiptDet[j].pay_mode.Equals("C")) {
                                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}</b></TD>", receiptDet[j].pay_details.ToString()));
                            }
                            else {
                                if (!(string.IsNullOrEmpty(receiptDet[j].Ref_BillNo.ToString())) && Convert.ToDecimal(receiptDet[j].Ref_BillNo.ToString()) > 0)

                                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}</b></TD>", branchCode + "/" + receiptDet[j].Ref_BillNo + "&nbsp"));
                                else
                                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}</b></TD>", receiptDet[j].pay_details.ToString()));
                            }

                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=4  ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", receiptDet[j].pay_amt, "&nbsp"));
                        }
                        sbStart.AppendLine("</TR>");
                    }

                    for (int j = 0; j < MaxPageRow - ModeCount; j++) {
                        sbStart.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                        sbStart.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-top:thin\"  colspan=2 ALIGN = \"LEFT\"><b>{0} </b></TD>", "&nbsp"));
                        sbStart.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-top:thin\"  colspan=2 ALIGN = \"LEFT\"><b>{0} </b></TD>", "&nbsp"));
                        sbStart.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-top:thin\"  colspan=4 ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                        sbStart.AppendLine("</TR>");
                    }
                }


                object receiptAmount = receiptDet.Sum(pay => pay.pay_amt);
                TotalAmt = Convert.ToDecimal(receiptAmount);
                sbStart.AppendLine("<TR  bgcolor='#FFFACD'  style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sbStart.AppendLine("<TH  style=\"border-top:thin solid \" colspan=4 ALIGN = \"LEFT\"><b>Totals :</b></TH>");
                sbStart.AppendLine(string.Format("<TH  style=\"border-top:thin solid \" colspan=4 ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TH>", TotalAmt, "&nbsp"));
                sbStart.AppendLine("</TR>");

                decimal cgst = 0, sgst = 0, igst = 0, amt = 0, TaxableAmount = 0;
                if (receiptDet.Count > 0) {
                    cgst = Math.Round(Convert.ToDecimal(receiptDet.Sum(pay => pay.CGST_Amount)), 2);
                    sgst = Math.Round(Convert.ToDecimal(receiptDet.Sum(pay => pay.SGST_Amount)), 2);
                    igst = Math.Round(Convert.ToDecimal(receiptDet.Sum(pay => pay.IGST_Amount)), 2);
                    amt = Math.Round(Convert.ToDecimal(receiptDet.Sum(pay => pay.pay_amt)), 2);
                    TaxableAmount = amt - (cgst + sgst + igst);
                }
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("</Table >");
                sbStart.AppendLine("<Table bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\" border-collapse:collapse;  font-size=9pt;   \" width=\"750\">");
                sbStart.AppendLine("<TR>");
                if (TaxableAmount > 0) {
                    sbStart.AppendLine(string.Format("<TD ALIGN = \"right\"><b>Taxable Amount : {0} </b></TD>", TaxableAmount));
                }
                {
                    sbStart.AppendLine(string.Format("<TD style=\" border-top:thin\"  ALIGN = \"right\"><b>CGST @ 1.5 % : {0} </b></TD>"
                           , cgst));
                }
                {
                    sbStart.AppendLine(string.Format("<TD style=\" border-top:thin\"  ALIGN = \"right\"><b>SGST @ 1.5 % : {0} </b></TD>"
                            , sgst));
                }
                {
                    sbStart.AppendLine(string.Format("<TD style=\" border-top:thin\"  ALIGN = \"right\"><b>IGST @ 3 % : {0} </b></TD>"
                                          , igst));
                }
                if (amt > 0) {
                    sbStart.AppendLine(string.Format("<TD style=\" border-top:thin\"   ALIGN = \"right\"><b>Total Amount : {0} </b></TD>"
                           , amt));
                }
                sbStart.AppendLine("</TR>");
                object temp1 = receiptDet.Sum(pay => pay.CardCharges);
                if (temp1 != null && temp1 != DBNull.Value)
                    CardCharges = Convert.ToDecimal(temp1);
                if (CardCharges > 0) {
                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TD  colspan=4  ALIGN = \"RIGHT\"><b>{0}</b></TD><TD  colspan=1  ALIGN = \"RIGHT\"><b>{1}</b></TD> ", "Card Charges : ", CardCharges + "&nbsp"));
                    sbStart.AppendLine("</TR>");
                }
                string strWord = string.Empty;
                NumberToWords objNWClas = new NumberToWords();
                objNWClass.ConvertNumberToWords(Convert.ToDouble(TotalAmt), out strWord);

                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\" border-bottom:thin;\"   colspan=5 ALIGN = \"LEFT\"><b>OPR: {0}{1}{1}</b></TD>", receiptDet[0].operator_code.ToString(), "&nbsp"));
                sbStart.AppendLine("</TR>");

                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD  style=\" border-bottom:thin; border-top:thin\"   colspan=5 ALIGN = \"LEFT\"><b>{0}{1}{1}</b></TD>", strWord, "&nbsp"));
                sbStart.AppendLine("</TR>");

                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\"border-right:thin\" colspan = 4 ALIGN = \"LEFT\"><b><br><br><br>{0}</b></TD>", "Customer Signature"));
                sbStart.AppendLine(string.Format("<TD style=\"border-left:thin\" colspan = 4 ALIGN = \"RIGHT\"><b>For {0}<br><br><br>{1}</b></TD>", company.company_name, "Authorized Signatory"));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("</TABLE>");
                return sbStart.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        public string GetReceiptDetailsPrintDotMatrix(string companyCode, string branchCode, int receiptNo, out ErrorVM error)
        {
            error = null;
            KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(com => com.company_code == companyCode && com.branch_code == branchCode).FirstOrDefault();
            List<KTTU_PAYMENT_DETAILS> payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.receipt_no == receiptNo && pay.trans_type == "O" && pay.company_code == companyCode && pay.branch_code == branchCode).ToList();
            if (payment.Count <= 0) {
                return "";
            }
            int orderNo = payment[0].series_no;
            KTTU_ORDER_MASTER order = db.KTTU_ORDER_MASTER.Where(ord => ord.company_code == companyCode && ord.branch_code == branchCode && ord.order_no == orderNo).FirstOrDefault();
            KSTU_CUSTOMER_MASTER customer = db.KSTU_CUSTOMER_MASTER.Where(cust => cust.cust_id == order.Cust_Id && cust.company_code == companyCode && cust.branch_code == branchCode).FirstOrDefault();
            try {
                int OrderNo = Convert.ToInt32(order.order_no);
                StringBuilder strLine = new StringBuilder();
                int maxWidth = 90;
                StringBuilder strdoubleTransLine = new StringBuilder();
                strdoubleTransLine.Append('=', maxWidth);
                string tempString = string.Empty;
                strLine.Append('-', maxWidth);
                StringBuilder strTransLine1 = new StringBuilder();
                strTransLine1.Append('-', maxWidth);
                decimal Spaces = 0M;
                StringBuilder strSpaces1 = new StringBuilder();
                Spaces = ((maxWidth - strTransLine1.Length) / 3);
                strSpaces1.Append(' ', Convert.ToInt32(Spaces));
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("{0,-80}", "TIN:" + company.tin_no.ToString()));
                string strSpaces = strSpaces1.ToString();
                string strTransLine = strTransLine1.ToString();
                string esc = Convert.ToChar(27).ToString();
                sb.Append(Convert.ToChar(27).ToString() + Convert.ToChar(69).ToString());
                if (string.Compare(order.cflag.ToString(), "Y") == 0) {
                    sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), "ORDER RECEIPT FORM/CANCELLED"));
                }
                else {
                    sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), "ORDER RECEIPT FORM"));
                }

                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));
                string DateTime = string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(payment[0].pay_date));
                string DateTime1 = string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(order.order_date));
                if (string.Compare("1", "1") == 0) {
                    sb.AppendLine(string.Format("{0,-20}{1,-10}{2, 48}"
                                       , "Receipt No", ": " + companyCode + "/" + payment[0].receipt_no.ToString()
                                       , "Receipt Date:" + DateTime));
                }
                else {
                    sb.AppendLine(string.Format("{0,-20}{1,-10}{2, 50}"
                                       , "Receipt No", ": " + payment[0].receipt_no.ToString()
                                       , "Receipt Date:" + DateTime));
                }
                sb.AppendLine(string.Format("{0,-20}{1,-60}"
                               , "Received From", ": " + customer.salutation.ToString() + customer.cust_name.ToString()));


                sb.AppendLine(string.Format("{0,-20}{1,-60}", "Address ", ": " + customer.address1.ToString().Trim()));
                if (!string.IsNullOrEmpty(customer.address2))
                    sb.AppendLine(string.Format("{0,-20}{1,-60}", "", ": " + customer.address2.ToString().Trim()));
                if (!string.IsNullOrEmpty(customer.address3))
                    sb.AppendLine(string.Format("{0,-20}{1,-60}", "", ": " + customer.address3.ToString().Trim()));

                if (!string.IsNullOrEmpty(customer.city) && !string.IsNullOrEmpty(customer.pin_code))
                    sb.AppendLine(string.Format("{0,-20}{1,-70}", "City", ": " + customer.city.ToString().Trim() + " - " + customer.pin_code.ToString().Trim()));
                if (!string.IsNullOrEmpty(customer.city.ToString()) && string.IsNullOrEmpty(customer.pin_code))
                    sb.AppendLine(string.Format("{0,-20}{1,-70}", "City", ": " + customer.city.ToString().Trim()));


                if (!string.IsNullOrEmpty(order.state))
                    sb.AppendLine(string.Format("{0,-20}{1,-70}", "State", ": " + order.state.ToString().Trim()));
                if (!string.IsNullOrEmpty(order.state_code.ToString()))
                    sb.AppendLine(string.Format("{0,-20}{1,-70}", "State Code", ": " + order.state_code.ToString().Trim()));

                if (!string.IsNullOrEmpty(order.phone_no))
                    sb.AppendLine(string.Format("{0,-20}{1,-60}", "", ": " + order.phone_no.ToString().Trim()));

                if (!string.IsNullOrEmpty(order.pan_no)) {
                    sb.AppendLine(string.Format("{0,-20}{1,-70}", "PAN", ": " + order.pan_no.ToString().Trim()));
                }
                else {
                    //string idquery = string.Format("select top(1) Doc_code,Doc_No from KSTU_CUSTOMER_ID_PROOF_DETAILS where cust_id='{0}' and company_code='{1}' and branch_code='{2}'", dtOrder.Rows[0]["cust_id"].ToString(), CGlobals.CompanyCode, CGlobals.BranchCode);
                    //DataTable IdDt = CGlobals.GetDataTable(idquery);
                    KSTU_CUSTOMER_ID_PROOF_DETAILS customerIdProof = db.KSTU_CUSTOMER_ID_PROOF_DETAILS.Where(cust => cust.cust_id == customer.cust_id
                                                                                                            && cust.company_code == companyCode
                                                                                                            && cust.branch_code == branchCode).FirstOrDefault();
                    if (customerIdProof != null) {
                        if (!string.IsNullOrEmpty(customerIdProof.Doc_code)) {
                            sb.AppendLine(string.Format("{0,-20}{1,-76}", customerIdProof.Doc_code.ToString(), ": " + customerIdProof.Doc_No.ToString()));
                        }
                    }
                }

                if (!string.IsNullOrEmpty(order.tin))
                    sb.AppendLine(string.Format("{0,-20}{1,-70}", "GSTIN", ": " + order.tin.ToString().Trim()));

                sb.AppendLine(string.Format("{0,-20}{1,-10}{2,50}", "Order No", ": " + branchCode + "/" + order.order_no.ToString().Trim(), "Order Date:" + DateTime1));

                #region Payment details
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));
                sb.Append(string.Format(new CultureInfo(company.Currency_code), "{0,-5}{1,-12}{2,-55}{3,18}", "SNo", "Mode", "Details", " Amount"));
                sb.AppendLine();
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));
                decimal TotalAmt = 0;
                int sno = 1;

                #region ChitPrint
                List<KTTU_PAYMENT_DETAILS> drChits = payment.Where(pay => pay.pay_mode == "CT").ToList();
                bool print = false;
                decimal chitAmount = 0, CardCharges = 0;
                string definition = string.Empty;
                string schemeCode = string.Empty, ChitNo = string.Empty, ChitAmt = string.Empty;
                for (int k = 0; k < drChits.Count; k++) {
                    definition += string.Format("{0}/{1}/{2} ", drChits[k].scheme_code, drChits[k].group_code, drChits[k].Ref_BillNo);
                    chitAmount += Convert.ToDecimal(drChits[k].pay_amt);

                    if (drChits.Count == 1 || (drChits.Count > 1 && k > 0)) {
                        if ((drChits.Count == 1) || (drChits.Count == k + 1) || !(drChits[k - 1].receipt_no.Equals(drChits[k].receipt_no))) {
                            print = true;
                        }
                    }
                    if (k < drChits.Count - 1 && (Convert.ToInt32(drChits[k].Ref_BillNo) != (Convert.ToInt32(drChits[k + 1].Ref_BillNo) - 1)  //)
                        || !(drChits[k + 1].scheme_code.Equals(drChits[k].scheme_code)) || !(drChits[k + 1].group_code.Equals(drChits[k].group_code)))) {
                        print = true;
                    }
                    if (print) {
                        string[] chiNoPatch = definition.Split(' ');
                        definition = chiNoPatch[0];
                        if (chiNoPatch.Length > 2) {
                            definition += "-" + chiNoPatch[chiNoPatch.Length - 2];
                        }

                        //sb.Append(string.Format("{0,-14}", drChits[k]["pay_date"]));
                        //sb.Append(string.Format("{0,-10}", drChits[k]["receipt_no"]));
                        sb.Append(string.Format("{0,-5}", sno)); sno++;
                        sb.Append(string.Format("{0,-12}", SIGlobals.Globals.PaymentMode(drChits[k].pay_mode)));
                        sb.Append(string.Format("{0,-55}", definition));
                        sb.AppendLine(string.Format(new CultureInfo(company.Currency_code), "{0,28}", chitAmount.ToString()));

                        definition = schemeCode = ChitNo = ChitAmt = string.Empty;
                        chitAmount = 0;
                        print = false;
                    }

                }
                #endregion

                #region Other Paymodes

                string patch = string.Empty;
                for (int j = 0; j < payment.Count; j++) {
                    if (!payment[j].pay_mode.Equals("CT")) {
                        sb.Append(string.Format("{0,-5}", sno)); sno++;
                        sb.Append(string.Format("{0,-12}", SIGlobals.Globals.PaymentMode(payment[j].pay_mode)));

                        if (payment[j].pay_mode.Equals("R")) {
                            string cardDetails = string.Empty;

                            if (!(string.IsNullOrEmpty(payment[j].Ref_BillNo)) && Convert.ToDecimal(payment[j].Ref_BillNo.ToString()) > 0)
                                cardDetails = payment[j].Ref_BillNo.ToString();
                            if (!string.IsNullOrEmpty(payment[j].bank_name))
                                cardDetails += "/" + payment[j].bank_name;
                            if (!string.IsNullOrEmpty(payment[j].bank))
                                cardDetails += "/" + payment[j].bank;
                            if (!string.IsNullOrEmpty(payment[j].card_app_no))
                                cardDetails += "/" + payment[j].card_app_no;

                            if (cardDetails.Length < 44)
                                sb.Append(string.Format("{0,-55}", cardDetails));
                            else {
                                List<string> lstCardDetails = SIGlobals.Globals.SplitStringAt(35, cardDetails);
                                if ((lstCardDetails != null && lstCardDetails.Count > 0) && lstCardDetails == null)
                                    for (int x = 0; x < lstCardDetails.Count; x++)
                                        sb.AppendLine(string.Format("{0,-17}-{1,-55}", "", lstCardDetails[x]));
                                lstCardDetails = null;
                            }
                        }
                        else if (payment[j].pay_mode.Equals("Q")) {
                            string chequeDetails = string.Empty;
                            if (!string.IsNullOrEmpty(payment[j].bank))
                                chequeDetails = "/" + payment[j].bank;

                            if (!(string.IsNullOrEmpty(payment[j].cheque_no)) && Convert.ToDecimal(payment[j].cheque_no.ToString()) > 0)
                                sb.Append(string.Format("{0,-55}", payment[j].cheque_no + chequeDetails));
                            else
                                sb.Append(string.Format("{0,-55}", ""));
                        }
                        else if (payment[j].pay_mode.Equals("BC")) {
                            sb.Append(string.Format("{0,-55}", payment[j].pay_details.ToString()));
                        }

                        else {
                            if (!(string.IsNullOrEmpty(payment[j].Ref_BillNo)) && Convert.ToDecimal(payment[j].Ref_BillNo.ToString()) > 0)
                                sb.Append(string.Format("{0,-55}", branchCode + "/" + payment[j].Ref_BillNo.ToString()));
                            else
                                sb.Append(string.Format("{0,-55}", payment[j].pay_details));
                        }

                        sb.AppendLine(string.Format(new CultureInfo(company.Currency_code), "{0,18}", payment[j].pay_amt.ToString()));
                    }

                }

                #endregion

                object receiptAmount = payment.Sum(pay => pay.pay_amt);
                TotalAmt = Convert.ToDecimal(receiptAmount);
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));
                sb.AppendLine(string.Format(new CultureInfo(company.Currency_code), "{0,90}", "Total :" + receiptAmount));

                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                decimal cgst = 0, sgst = 0, igst = 0, amt = 0, TaxableAmount = 0;
                if (payment.Count > 0) {
                    cgst = Math.Round(Convert.ToDecimal(payment.Sum(p => p.CGST_Amount)), 2);
                    sgst = Math.Round(Convert.ToDecimal(payment.Sum(p => p.SGST_Amount)), 2);
                    igst = Math.Round(Convert.ToDecimal(payment.Sum(p => p.IGST_Amount)), 2);
                    amt = Math.Round(Convert.ToDecimal(payment.Sum(p => p.pay_amt)), 2);
                    TaxableAmount = amt - (cgst + sgst + igst);
                }
                sb.Append(string.Format("{0}{1}", "Taxable Amt:", TaxableAmount + " "));
                sb.Append(string.Format("{0}{1}", "CGST 1.5%:", cgst + " "));
                sb.Append(string.Format("{0}{1}", "SGST 1.5%:", sgst + " "));
                sb.Append(string.Format("{0}{1}", "IGST 3%:", igst + " "));
                sb.AppendLine(string.Format("{0}{1}", "Total Amount:", amt));
                #endregion

                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                object temp1 = payment.Sum(pay => pay.CardCharges);
                if (temp1 != null && temp1 != DBNull.Value)
                    CardCharges = Convert.ToDecimal(temp1);
                if (CardCharges > 0) {
                    sb.AppendLine(string.Format(new CultureInfo(company.Currency_code), "{0:N,-80}", "Card Charges : " + CardCharges));
                }
                string strWords = string.Empty;
                NumberToWords objNWClass = new NumberToWords();
                objNWClass.ConvertNumberToWords(Convert.ToDouble(TotalAmt), out strWords);
                sb.AppendLine(string.Format("{0,-80}", "" + strWords));
                sb.AppendLine();
                sb.AppendLine("I hereby agree to take delivery of the article on payment of amount of your bill");
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine(string.Format("{0,80}", "Authorized Signatory"));
                return sb.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        public string GetClosedOrderPrintHTML(string companyCode, string branchCode, int orderNo, out ErrorVM error)
        {
            error = null;
            KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(com => com.company_code == companyCode
                                                                            && com.branch_code == branchCode).FirstOrDefault();
            KTTU_ORDER_MASTER order = db.KTTU_ORDER_MASTER.Where(ord => ord.company_code == companyCode
                                                                            && ord.branch_code == branchCode
                                                                            && ord.order_no == orderNo).FirstOrDefault();
            KSTU_CUSTOMER_MASTER customer = db.KSTU_CUSTOMER_MASTER.Where(cust => cust.cust_id == order.Cust_Id
                                                                            && cust.company_code == companyCode
                                                                            && cust.branch_code == branchCode).FirstOrDefault();
            List<KTTU_PAYMENT_DETAILS> payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == orderNo
                                                                            && pay.trans_type == "CO"
                                                                            && pay.company_code == companyCode
                                                                            && pay.branch_code == branchCode).ToList();
            try {
                decimal TotalAmt = 0;
                string TotalAmtInWords = string.Empty;
                string pay_mode = string.Empty;
                NumberToWords objNWClass = new NumberToWords();
                objNWClass.ConvertNumberToWords(Convert.ToDouble(TotalAmt), out TotalAmtInWords);

                string CompanyAddress = string.Empty;
                CompanyAddress = company.address1;
                CompanyAddress = CompanyAddress + "<br>" + company.address2;
                CompanyAddress = CompanyAddress + "<br>" + company.address3;
                CompanyAddress = CompanyAddress + "<br>" + "Phone no: " + company.phone_no;
                CompanyAddress = CompanyAddress + "<br> E-mail : " + company.email_id;
                CompanyAddress = CompanyAddress + "<br> Website : " + company.website;

                StringBuilder sbStart = new StringBuilder();
                sbStart.AppendLine("<html>");
                sbStart.AppendLine("<head>");
                sbStart.AppendLine(SIGlobals.Globals.GetStyleOrder());
                sbStart.AppendLine("</head>");
                sbStart.AppendLine("<body>");

                string strWords = string.Empty;
                string DateTime = string.Format("{0:dd/MM/yyyy} {0:T}", Convert.ToDateTime(order.closed_date));
                string OrderDate = string.Format("{0:dd/MM/yyyy} {0:T}", Convert.ToDateTime(order.order_date));
                sbStart.AppendLine("<Table frame=\"border\" border=\"0\" width=\"700\">");

                for (int j = 0; j < 3; j++) {
                    sbStart.AppendLine("<TR style=border:0>");
                    sbStart.AppendLine(string.Format("<TD style=border:0 colspan = 10 ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    sbStart.AppendLine("</TR>");
                }
                sbStart.AppendLine("</Table>");

                sbStart.AppendLine("<Table font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"0\"; border-collapse:collapse; width=\"750\">");  //FRAME=BOX RULES=NONE



                string Sultation = order.salutation;
                string Name = order.cust_name;
                string Address1 = order.address1;
                string Address2 = order.address2;
                string Address3 = order.address3;
                string Email = order.Email_ID;
                string Mobile = order.phone_no;
                string Phone = order.phone_no;
                string PAN = order.pan_no;
                string state = order.state;
                string stateCode = Convert.ToString(order.state_code);

                if (!string.IsNullOrEmpty(Mobile) && !string.IsNullOrEmpty(Phone))
                    Mobile = Mobile + "/" + Phone;
                if (string.IsNullOrEmpty(Mobile))
                    Mobile = Phone;

                string City = order.city;
                string pincode = order.pin_code;

                if (!string.IsNullOrEmpty(pincode))
                    City = City + " - " + pincode;
                string Address = string.Empty;
                if (Address1 != string.Empty)
                    Address = Address + "<br>" + Address1;
                if (Address2 != string.Empty)
                    Address = Address + "<br>" + Address2;
                if (Address3 != string.Empty)
                    Address = Address + "<br>" + Address3;
                if (Email != string.Empty)
                    Email = Email + "<br>";

                sbStart.AppendLine("<TR align=\"CENTER\">");

                sbStart.AppendLine("<TD style=\"border-right:thin\" ALIGN = \"CENTER\"><b>CLOSED ORDER PAYMENT VOUCHER</b></TD>");

                sbStart.AppendLine("</TR>");
                for (int j = 0; j < 1; j++) {
                    sbStart.AppendLine("<TR style=border:0>");
                    sbStart.AppendLine(string.Format("<TD style=border:0 colspan = 10 ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    sbStart.AppendLine("</TR>");
                }

                sbStart.AppendLine("<Table font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\" border-collapse:collapse; \" width=\"750\">");
                sbStart.AppendLine("<TR  bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\"  style=\"border-left:thin\">");
                sbStart.AppendLine(string.Format("<TD width=\"250\" ALIGN = \"LEFT\"><b>CUSTOMER DETAILS</b></TD>"));
                sbStart.AppendLine(string.Format("<TD width=\"200\" ALIGN = \"CENTER\"><b>GSTIN : {0}</b></TD>", company.tin_no.ToString()));
                sbStart.AppendLine(string.Format("<TD width=\"150\" ALIGN = \"LEFT\"><b>{0}</b></TD>", "SHOWROOM ADDRESS"));
                sbStart.AppendLine("</TR>");

                sbStart.AppendLine("<tr>");
                sbStart.AppendLine("<td>");
                sbStart.AppendLine("<Table>");
                sbStart.AppendLine("<Table font-size=12pt;  class=\"boldText\"  style=\"border-collapse:collapse;\" >");

                sbStart.AppendLine("<tr style=\"border-right:0\"  >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left \" ><b>Name &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + Name + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" ><b>Address &nbsp&nbsp </b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + Address1 + "</b></td>");
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + Address2 + "</b></td>");
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + Address3 + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>City&nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + City + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" ><b>State &nbsp&nbsp </b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + state + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>Code &nbsp&nbsp&nbsp </b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + stateCode + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>Phone &nbsp&nbsp </b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + Mobile + "</b></td>");
                sbStart.AppendLine("</tr>");
                if (!string.IsNullOrEmpty(PAN)) {
                    sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sbStart.AppendLine("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>PAN &nbsp&nbsp</b></td>");
                    sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + PAN + "</b></td>");
                    sbStart.AppendLine("</tr>");
                }
                sbStart.AppendLine("</table>");
                sbStart.AppendLine("</td>");

                sbStart.AppendLine("<td>");
                sbStart.AppendLine("<Table font-size=12pt;   class=\"boldText\"  style=\"border-collapse:collapse;\" >");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" ><b>Closed Date &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin ; border-top:thin\" ><b>" + DateTime + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" ><b>Order No &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin ; border-top:thin\" ><b>" + branchCode + "/" + order.order_no.ToString() + "</b></td>");
                sbStart.AppendLine("</tr>");


                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" ><b>Order Date &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin ; border-top:thin\" ><b>" + OrderDate + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("</table>");
                sbStart.AppendLine("</td>");

                sbStart.AppendLine("<td>");
                sbStart.AppendLine("<Table font-size=12pt;   class=\"boldText\"  style=\"border-collapse:collapse;\" >");

                sbStart.AppendLine("<tr style=\"border-right:0\"  >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left \" ><b>Name &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.company_name + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>Address &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.address1 + "</b></td>");
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.address2 + "</b></td>");
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.address3 + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>City &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.city + " - " + company.pin_code + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>State &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + company.state + "</b></td>");
                sbStart.AppendLine("</tr>");


                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>Code &nbsp&nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + company.state_code.ToString() + "</b></td>");
                sbStart.AppendLine("</tr>");

                sbStart.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>Phone &nbsp&nbsp</b></td>");
                sbStart.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + company.phone_no + "</b></td>");
                sbStart.AppendLine("</tr>");
                sbStart.AppendLine("</table>");
                sbStart.AppendLine("</td>");

                sbStart.AppendLine("</TR>");

                sbStart.AppendLine("</Table>");

                sbStart.AppendLine("<Table font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse;\" width=\"750\">");  //FRAME=BOX RULES=NONE

                sbStart.AppendLine("<TR  bgcolor='#FFFACD'>");
                sbStart.AppendLine("<TH style=\"border-bottom:thin solid\" colspan=2 ALIGN = \"CENTER\">Mode</TH>");

                if (payment[0].pay_mode.Equals("Q")) {
                    sbStart.AppendLine("<TH style=\"border-bottom:thin solid\" colspan=2  ALIGN = \"CENTER\">QNo/Bank</TH>");
                }
                else {
                    sbStart.AppendLine("<TH style=\"border-bottom:thin solid\" colspan=2  ALIGN = \"CENTER\">Details</TH>");
                }
                sbStart.AppendLine("<TH style=\"border-bottom:thin solid\" colspan=4 ALIGN = \"CENTER\">Amount</TH>");
                sbStart.AppendLine("</TR>");
                int MaxPageRow = 4;
                var drChits = payment.Where(pay => pay.pay_mode == "CT").ToList();

                bool print = false;
                decimal chitAmount = 0, CardCharges = 0;
                string definition = string.Empty;
                string schemeCode = string.Empty, ChitNo = string.Empty, ChitAmt = string.Empty;
                for (int k = 0; k < drChits.Count; k++) {
                    definition += string.Format("{0}/{1}/{2} ", drChits[k].scheme_code, drChits[k].group_code, drChits[k].Ref_BillNo);
                    chitAmount += Convert.ToDecimal(drChits[k].pay_amt);

                    if (drChits.Count == 1 || (drChits.Count > 1 && k > 0)) {
                        if ((drChits.Count == 1) || (drChits.Count == k + 1) || !(drChits[k - 1].receipt_no.Equals(drChits[k].receipt_no))) {
                            print = true;
                        }
                    }

                    if (k < drChits.Count - 1 && (Convert.ToInt32(drChits[k].Ref_BillNo) != (Convert.ToInt32(drChits[k + 1].Ref_BillNo) - 1)
                        || !(drChits[k + 1].scheme_code.Equals(drChits[k].scheme_code)) || !(drChits[k + 1].group_code.Equals(drChits[k].group_code)))) {
                        print = true;
                    }
                    if (print) {
                        string[] chiNoPatch = definition.Split(' ');
                        definition = chiNoPatch[0];
                        if (chiNoPatch.Length > 2) {
                            definition += "-" + chiNoPatch[chiNoPatch.Length - 2];
                        }

                        string payMode = drChits[k].pay_mode;
                        KTTS_PAYMENT_MASTER payMaster = db.KTTS_PAYMENT_MASTER.Where(pn => pn.company_code == companyCode && pn.branch_code == branchCode && pn.payment_code == payMode).FirstOrDefault();
                        sbStart.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}{1}{1}</b></TD>", payMaster.payment_name, "&nbsp"));

                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}</b></TD>", definition + "&nbsp"));
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=4 ALIGN = \"RIGHT\"><b>{0}</b></TD>", chitAmount + "&nbsp"));
                        sbStart.AppendLine("<TR>");
                        definition = schemeCode = ChitNo = ChitAmt = string.Empty;
                        chitAmount = 0;
                        print = false;
                    }

                }

                string patch = string.Empty;
                int ModeCount = payment.Count;
                if (ModeCount <= 10) {
                    for (int j = 0; j < ModeCount; j++) {

                        if (!payment[j].pay_mode.Equals("CT")) {
                            sbStart.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");

                            string payMode = payment[j].pay_mode;
                            KTTS_PAYMENT_MASTER payMaster = db.KTTS_PAYMENT_MASTER.Where(pn => pn.company_code == companyCode && pn.branch_code == branchCode && pn.payment_code == payMode).FirstOrDefault();
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}{1}{1}</b></TD>", payMaster.payment_name, "&nbsp"));
                            if (payment[j].pay_mode.Equals("R")) {
                                string cardDetails = string.Empty;

                                if (!(string.IsNullOrEmpty(payment[j].Ref_BillNo.ToString())) && Convert.ToDecimal(payment[j].Ref_BillNo.ToString()) > 0)
                                    cardDetails = payment[j].Ref_BillNo.ToString();
                                if (!string.IsNullOrEmpty(payment[j].bank_name.ToString()))
                                    cardDetails += "/" + payment[j].bank_name;
                                if (!string.IsNullOrEmpty(payment[j].bank.ToString()))
                                    cardDetails += "/" + payment[j].bank;
                                if (!string.IsNullOrEmpty(payment[j].card_app_no.ToString()))
                                    cardDetails += "/" + payment[j].card_app_no;

                                if (cardDetails.Length < 44)

                                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}{1}{1}</b></TD>", cardDetails, "&nbsp"));

                                else {
                                    List<string> lstCardDetails = SIGlobals.Globals.SplitStringAt(35, cardDetails);
                                    if ((lstCardDetails != null && lstCardDetails.Count > 0) && lstCardDetails == null)
                                        for (int x = 0; x < lstCardDetails.Count; x++)
                                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"> {0} <br></TD>", lstCardDetails[x]));

                                    lstCardDetails = null;
                                }
                            }
                            else if (payment[j].pay_mode.Equals("Q")) {
                                string chequeDetails = string.Empty;
                                if (!string.IsNullOrEmpty(payment[j].bank.ToString()))
                                    chequeDetails = "/" + payment[j].bank;

                                if (!(string.IsNullOrEmpty(payment[j].cheque_no.ToString())) && Convert.ToDecimal(payment[j].cheque_no.ToString()) > 0)
                                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}</b></TD>", payment[j].cheque_no + chequeDetails + "&nbsp"));

                                else
                                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}</b></TD>", "&nbsp"));

                            }
                            else if (payment[j].pay_mode.Equals("C")) {
                                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}</b></TD>", payment[j].pay_details.ToString()));
                            }
                            else {
                                if (!(string.IsNullOrEmpty(payment[j].Ref_BillNo)) && Convert.ToDecimal(payment[j].Ref_BillNo) > 0)

                                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}</b></TD>", branchCode + "/" + payment[j].Ref_BillNo + "&nbsp"));
                                else
                                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=2 ALIGN = \"LEFT\"><b>{0}</b></TD>", payment[j].pay_details.ToString()));
                            }

                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=4  ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", payment[j].pay_amt, "&nbsp"));
                        }
                        sbStart.AppendLine("</TR>");
                    }

                    for (int j = 0; j < MaxPageRow - ModeCount; j++) {
                        sbStart.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                        sbStart.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-top:thin\"  colspan=2 ALIGN = \"LEFT\"><b>{0} </b></TD>", "&nbsp"));
                        sbStart.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-top:thin\"  colspan=2 ALIGN = \"LEFT\"><b>{0} </b></TD>", "&nbsp"));
                        sbStart.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-top:thin\"  colspan=4 ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                        sbStart.AppendLine("</TR>");
                    }
                }

                object receiptAmount = payment.Sum(pay => pay.pay_amt);
                TotalAmt = Convert.ToDecimal(receiptAmount);
                sbStart.AppendLine("<TR  bgcolor='#FFFACD'  style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sbStart.AppendLine("<TH  style=\"border-top:thin solid \" colspan=4 ALIGN = \"LEFT\"><b>Totals :</b></TH>");
                sbStart.AppendLine(string.Format("<TH  style=\"border-top:thin solid \" colspan=4 ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TH>", TotalAmt, "&nbsp"));
                sbStart.AppendLine("</TR>");

                decimal cgst = 0, sgst = 0, igst = 0, amt = 0, TaxableAmount = 0;
                if (payment.Count > 0) {
                    cgst = Math.Round(Convert.ToDecimal(payment.Sum(pay => pay.CGST_Amount)), 2);
                    sgst = Math.Round(Convert.ToDecimal(payment.Sum(pay => pay.SGST_Amount)), 2);
                    igst = Math.Round(Convert.ToDecimal(payment.Sum(pay => pay.IGST_Amount)), 2);
                    amt = Math.Round(Convert.ToDecimal(payment.Sum(pay => pay.pay_amt)), 2);
                    TaxableAmount = amt - (cgst + sgst + igst);
                }

                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("</Table >");
                sbStart.AppendLine("<Table bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\" border-collapse:collapse;  font-size=9pt;   \" width=\"750\">");
                sbStart.AppendLine("<TR>");
                if (TaxableAmount > 0) {
                    sbStart.AppendLine(string.Format("<TD ALIGN = \"right\"><b>Taxable Amount : {0} </b></TD>", TaxableAmount));
                }
                {
                    sbStart.AppendLine(string.Format("<TD style=\" border-top:thin\"  ALIGN = \"right\"><b>CGST @ 1.5 % : {0} </b></TD>"
                           , cgst));
                }
                {
                    sbStart.AppendLine(string.Format("<TD style=\" border-top:thin\"  ALIGN = \"right\"><b>SGST @ 1.5 % : {0} </b></TD>"
                            , sgst));
                }
                {
                    sbStart.AppendLine(string.Format("<TD style=\" border-top:thin\"  ALIGN = \"right\"><b>IGST @ 3 % : {0} </b></TD>"
                                          , igst));
                }
                if (amt > 0) {
                    sbStart.AppendLine(string.Format("<TD style=\" border-top:thin\"   ALIGN = \"right\"><b>Total Amount : {0} </b></TD>"
                           , amt));
                }

                sbStart.AppendLine("</TR>");
                object temp1 = payment.Sum(pay => pay.CardCharges);
                if (temp1 != null && temp1 != DBNull.Value)
                    CardCharges = Convert.ToDecimal(temp1);
                if (CardCharges > 0) {
                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TD  colspan=4  ALIGN = \"RIGHT\"><b>{0}</b></TD><TD  colspan=1  ALIGN = \"RIGHT\"><b>{1}</b></TD> ", "Card Charges : ", CardCharges + "&nbsp"));
                    sbStart.AppendLine("</TR>");
                }
                string strWord = string.Empty;
                NumberToWords objNWClas = new NumberToWords();
                objNWClass.ConvertNumberToWords(Convert.ToDouble(TotalAmt), out strWord);
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\" border-bottom:thin;\"   colspan=5 ALIGN = \"LEFT\"><b>Closed By: {0}{1}{1}</b></TD>", order.closed_by.ToString(), "&nbsp"));
                sbStart.AppendLine("</TR>");

                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD  style=\" border-bottom:thin; border-top:thin\"   colspan=5 ALIGN = \"LEFT\"><b>{0}{1}{1}</b></TD>", strWord, "&nbsp"));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\"border-right:thin\" colspan = 4 ALIGN = \"LEFT\"><b><br><br><br>{0}</b></TD>", "Customer Signature"));
                sbStart.AppendLine(string.Format("<TD style=\"border-left:thin\" colspan = 4 ALIGN = \"RIGHT\"><b>For {0}<br><br><br>{1}</b></TD>", company.company_name, "Authorized Signatory"));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("</TABLE>");
                return sbStart.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        public string GetClosedOrderPrintDotMatrix(string companyCode, string branchCode, int orderNo, out ErrorVM error)
        {
            error = null;
            KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(com => com.company_code == companyCode && com.branch_code == branchCode).FirstOrDefault();
            KTTU_ORDER_MASTER order = db.KTTU_ORDER_MASTER.Where(ord => ord.company_code == companyCode && ord.branch_code == branchCode && ord.order_no == orderNo).FirstOrDefault();
            if (order == null) return "";
            List<KTTU_ORDER_DETAILS> orderDet = db.KTTU_ORDER_DETAILS.Where(ord => ord.company_code == companyCode && ord.branch_code == branchCode && ord.order_no == orderNo).ToList();
            KSTU_CUSTOMER_MASTER customer = db.KSTU_CUSTOMER_MASTER.Where(cust => cust.cust_id == order.Cust_Id && cust.company_code == companyCode && cust.branch_code == branchCode).FirstOrDefault();
            try {
                StringBuilder sb = new StringBuilder();
                sb.Append(Convert.ToChar(18).ToString());
                int width = 90;
                StringBuilder strdoubleTransLine = new StringBuilder();
                strdoubleTransLine.Append('=', width);
                StringBuilder strTransLine = new StringBuilder();
                strTransLine.Append('-', width);
                decimal Spaces = 0M;
                StringBuilder strSpaces = new StringBuilder();
                Spaces = ((width - strTransLine.Length) / 3);
                strSpaces.Append(' ', Convert.ToInt32(Spaces));
                sb.AppendLine();
                sb.Append(string.Format("{0}{1}", "TIN: ", SIGlobals.Globals.FillCompanyDetails(company.tin_no, companyCode, branchCode)));
                string esc = Convert.ToChar(27).ToString();

                string dupliCateOrderString = string.Empty;
                if (!order.order_date.ToShortDateString().Equals(SIGlobals.Globals.GetApplicationDate(companyCode, branchCode))) {
                    dupliCateOrderString = "(Duplicate)";
                }

                sb = SIGlobals.Globals.GetCompanyname(db, companyCode, branchCode);
                if (string.Compare(order.cflag.ToString(), "Y") == 0) {
                    sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), "CLOSED ORDER " + dupliCateOrderString));
                }
                else {
                    sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), "CLOSED ORDER FORM " + dupliCateOrderString));
                }
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                if (string.Compare("1", "1") == 0) {
                    sb.AppendLine(string.Format("{0,-50}{1,40}", "ClosedOrder No      " + ": " + branchCode + "/" + "O" + "/" + order.order_no.ToString()
                        , "Closed Date :" + order.order_date.ToShortDateString()));
                }
                else {
                    sb.AppendLine(string.Format("{0,-50}{1,40}", "Order No      " + ": " + order.order_no.ToString()
                       , "CLosed Date :" + order.order_date.ToShortDateString()));
                }
                sb.AppendLine(string.Format("{0,-50}{1,40}", "Received from " + ": " + customer.salutation.ToString() + customer.cust_name.ToString()
                    , "Due Date :" + order.delivery_date.ToShortDateString()));
                if (!string.IsNullOrEmpty(order.address1.ToString()))
                    sb.AppendLine(string.Format("{0,-90}", "Address       " + ": " + order.address1.ToString().Trim()));
                if (!string.IsNullOrEmpty(order.address2.ToString()))
                    sb.AppendLine(string.Format("{0,-90}", "              " + ": " + order.address2.ToString().Trim()));
                if (!string.IsNullOrEmpty(order.address3.ToString()))
                    sb.AppendLine(string.Format("{0,-90}", "              " + ": " + order.address3.ToString().Trim()));

                if (!string.IsNullOrEmpty(order.city.ToString()) && !string.IsNullOrEmpty(order.pin_code.ToString()))
                    sb.AppendLine(string.Format("{0,-90}", "City          : " + order.city.ToString() + " - " + order.pin_code.ToString()));

                else if (!string.IsNullOrEmpty(order.city.ToString()))
                    sb.AppendLine(string.Format("{0,-90}", "City          : " + order.city.ToString()));

                if (!string.IsNullOrEmpty(order.state.ToString()))
                    sb.AppendLine(string.Format("{0,-90}", "State         : " + order.state.ToString()));
                if (!string.IsNullOrEmpty(order.state_code.ToString()))
                    sb.AppendLine(string.Format("{0,-90}", "State Code    : " + order.state_code.ToString()));

                if (!string.IsNullOrEmpty(order.phone_no) && !string.IsNullOrEmpty(order.mobile_no.ToString())) {
                    sb.Append(string.Format("{0,-45}", "PH            " + ": " + order.phone_no.ToString()));
                    sb.Append(string.Format("{0,45}", "Mobile        " + ": " + order.mobile_no.ToString()));
                    sb.AppendLine();
                }
                else {
                    if (!string.IsNullOrEmpty(order.phone_no))
                        sb.Append(string.Format("{0,-90}", "PH            " + ": " + order.phone_no.ToString()));

                    else if (!string.IsNullOrEmpty(order.mobile_no))
                        sb.Append(string.Format("{0,-90}", "Mobile        " + ": " + order.mobile_no.ToString()));
                    sb.AppendLine();
                }
                if (!string.IsNullOrEmpty(order.pan_no))
                    sb.AppendLine(string.Format("{0,-90}", "PAN       : " + order.pan_no.ToString()));

                if (!string.IsNullOrEmpty(order.tin))
                    sb.AppendLine(string.Format("{0,-90}", "GSTIN       : " + order.tin.ToString()));
                if (!string.IsNullOrEmpty(order.remarks.ToString()))
                    sb.AppendLine(string.Format("{0,-90}", "Remarks       : " + order.remarks.ToString()));
                sb.AppendLine();
                if (!string.IsNullOrEmpty(order.rate.ToString()))
                    sb.Append(string.Format(new CultureInfo(company.Currency_code), "{0,-15}", "Rate: " + order.rate.ToString()));
                else
                    sb.Append(string.Format("{0,-15}", ""));

                sb.Append(string.Format(new CultureInfo(company.Currency_code), "{0:N,-15}", "Advance: " + order.advance_ord_amount.ToString()));
                sb.Append(string.Format("{0,10}", " OPR: " + order.operator_code.ToString()));
                sb.Append(string.Format("{0,-10}", ", SAL: " + order.sal_code.ToString()));

                if (string.Compare(order.karat.ToString(), "NA") != 0)
                    sb.Append(string.Format("{0,-10}", ", (" + order.karat.ToString() + ")"));
                else
                    sb.Append(string.Format("{0,-10}", ""));

                if (Convert.ToDecimal(order.rate.ToString()) > 0)
                    sb.AppendLine(string.Format(new CultureInfo(company.Currency_code), "{0,20}", "Rate Accepted"));
                else
                    sb.AppendLine(string.Format(new CultureInfo(company.Currency_code), "{0,20}", "Delivery Rate"));

                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));

                string itemName = string.Empty;
                List<string> lstItemName = null;
                for (int i = 0; i < orderDet.Count; i++) {
                    string Descp = orderDet[i].description.ToString();
                    itemName = orderDet[i].item_name.ToString();
                    if (Descp.Length <= 89)
                        sb.Append(string.Format("{0,-90}", i + 1 + ")" + Descp));
                    else {
                        string[] Descwtspace = Descp.Split(' ');
                        if (Descwtspace.Length == 1) {
                            lstItemName = SIGlobals.Globals.SplitStringAt(89, Descp);
                            if (lstItemName != null && lstItemName.Count > 1) {
                                string ItemNames = string.Empty;
                                for (int x = 0; x < lstItemName.Count; x++)
                                    ItemNames = ItemNames + " " + lstItemName[x].Trim();
                                lstItemName = null;
                                sb.Append(string.Format("{0,-90}", i + 1 + ")" + ItemNames));
                            }
                        }
                        else {
                            sb.Append(string.Format("{0,-90}", i + 1 + ")" + Descp));
                        }
                    }
                    sb.AppendLine();
                }
                sb.AppendLine();
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                sb.AppendLine(string.Format(new CultureInfo(company.Currency_code), "{0,-12}{1,-12}{2,-12}{3,-36}{4:N,18}", "Date", "Receipt No.", "Mode", "Details", "Amount"));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));

                List<KTTU_PAYMENT_DETAILS> paymentDet = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == orderNo && pay.trans_type == "O" && pay.company_code == companyCode && pay.branch_code == branchCode).ToList();

                #region ChitPrint
                List<KTTU_PAYMENT_DETAILS> chitPay = paymentDet.Where(pay => pay.pay_mode == "CT").ToList();
                bool print = false;
                decimal chitAmount = 0;
                string definition = string.Empty;
                string schemeCode = string.Empty, ChitNo = string.Empty, ChitAmt = string.Empty;
                for (int k = 0; k < chitPay.Count; k++) {
                    definition += string.Format("{0}/{1}/{2} ", chitPay[k].scheme_code, chitPay[k].group_code, chitPay[k].Ref_BillNo);
                    chitAmount += Convert.ToDecimal(chitPay[k].pay_amt);

                    if (chitPay.Count == 1 || (chitPay.Count > 1 && k > 0)) {
                        if ((chitPay.Count == 1) || (chitPay.Count == k + 1) || !(chitPay[k - 1].receipt_no.Equals(chitPay[k].receipt_no))) {
                            print = true;
                        }
                    }

                    if (k < chitPay.Count - 1 && (Convert.ToInt32(chitPay[k].Ref_BillNo) != (Convert.ToInt32(chitPay[k + 1].Ref_BillNo) - 1)  //)
                        || !(chitPay[k + 1].scheme_code.Equals(chitPay[k].scheme_code)) || !(chitPay[k + 1].group_code.Equals(chitPay[k].group_code)))) {
                        print = true;
                    }
                    if (print) {
                        string[] chiNoPatch = definition.Split(' ');
                        definition = chiNoPatch[0];
                        if (chiNoPatch.Length > 2) {
                            definition += "-" + chiNoPatch[chiNoPatch.Length - 2];
                        }

                        sb.Append(string.Format("{0,-14}", chitPay[k].pay_date));
                        sb.Append(string.Format("{0,-10}", chitPay[k].receipt_no));
                        sb.Append(string.Format("{0,-12}", SIGlobals.Globals.PaymentMode(chitPay[k].pay_mode)));
                        sb.Append(string.Format("{0,-36}", definition));
                        sb.AppendLine(string.Format(new CultureInfo(company.Currency_code), "{0,18}", chitAmount));

                        definition = schemeCode = ChitNo = ChitAmt = string.Empty;
                        chitAmount = 0;
                        print = false;
                    }

                }


                #endregion

                #region Other Payment Modes
                string patch = string.Empty;
                for (int j = 0; j < paymentDet.Count; j++) {
                    if (!paymentDet[j].pay_mode.Equals("CT")) {
                        sb.Append(string.Format("{0,-12}", Convert.ToDateTime(paymentDet[j].pay_date).ToShortDateString()));
                        sb.Append(string.Format("{0,-12}", branchCode + "/" + paymentDet[j].receipt_no));
                        sb.Append(string.Format("{0,-12}", SIGlobals.Globals.PaymentMode(paymentDet[j].pay_mode)));

                        if (paymentDet[j].pay_mode.Equals("R")) {
                            string cardDetails = string.Empty;
                            if (!(string.IsNullOrEmpty(paymentDet[j].Ref_BillNo.ToString())) && Convert.ToDecimal(paymentDet[j].Ref_BillNo) > 0)
                                cardDetails = paymentDet[j].Ref_BillNo.ToString();
                            if (!string.IsNullOrEmpty(paymentDet[j].bank_name.ToString()))
                                cardDetails += "/" + paymentDet[j].bank_name;
                            if (!string.IsNullOrEmpty(paymentDet[j].card_app_no.ToString()))
                                cardDetails += "/" + paymentDet[j].card_app_no;
                            if (cardDetails.Length < 35)
                                sb.Append(string.Format("{0,-36}", cardDetails));
                            else {
                                List<string> lstCardDetails = SIGlobals.Globals.SplitStringAt(35, cardDetails);
                                if (lstCardDetails != null && lstCardDetails.Count > 0) {
                                    sb.AppendLine(string.Format("{0,-36}-{1,-35}", lstCardDetails[0], ""));
                                    for (int x = 1; x < lstCardDetails.Count; x++)
                                        sb.Append(string.Format("{0,-36}-{1,-35}", "", lstCardDetails[x]));
                                }
                                lstCardDetails = null;
                            }
                        }
                        else if (paymentDet[j].pay_mode.Equals("Q")) {
                            string chequeDetails = string.Empty;
                            if (!string.IsNullOrEmpty(paymentDet[j].bank.ToString()))
                                chequeDetails = "/" + paymentDet[j].bank;

                            if (!(string.IsNullOrEmpty(paymentDet[j].cheque_no.ToString())) && Convert.ToDecimal(paymentDet[j].cheque_no) > 0)
                                sb.Append(string.Format("{0,-36}", paymentDet[j].cheque_no + chequeDetails));
                            else
                                sb.Append(string.Format("{0,-36}", ""));
                        }
                        else {
                            if (!(string.IsNullOrEmpty(paymentDet[j].Ref_BillNo.ToString())) && Convert.ToDecimal(paymentDet[j].Ref_BillNo) > 0)
                                sb.Append(string.Format("{0,-36}", branchCode + "/" + paymentDet[j].Ref_BillNo.ToString()));
                            else
                                sb.Append(string.Format("{0,-36}", ""));
                        }
                        sb.AppendLine(string.Format(new CultureInfo(company.Currency_code), "{0,18}", paymentDet[j].pay_amt));
                    }
                }
                #endregion

                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                sb.Append(Convert.ToChar(27).ToString() + Convert.ToChar(69).ToString());

                decimal advancePaid = 0, CardCharges = 0;
                object total = paymentDet.Sum(pay => pay.pay_amt);
                if (Convert.ToDecimal(total) != 0 && total != null)
                    advancePaid = Convert.ToDecimal(total);

                sb.AppendLine(string.Format(new CultureInfo(company.Currency_code), "{0,3}{1,-21}{2,12}{3,36}{4,18}"
                    , "", "Total Advance", "", "", advancePaid));
                sb.Append(Convert.ToChar(27).ToString() + Convert.ToChar(70).ToString());
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));

                decimal cgst = 0, sgst = 0, igst = 0, amt = 0, TaxableAmount = 0;
                if (paymentDet.Count > 0) {
                    cgst = Math.Round(Convert.ToDecimal(paymentDet.Sum(pay => pay.CGST_Amount)), 2);
                    sgst = Math.Round(Convert.ToDecimal(paymentDet.Sum(pay => pay.SGST_Amount)), 2);
                    igst = Math.Round(Convert.ToDecimal(paymentDet.Sum(pay => pay.IGST_Amount)), 2);
                    amt = Math.Round(Convert.ToDecimal(paymentDet.Sum(pay => pay.pay_amt)), 2);
                    TaxableAmount = amt - (cgst + sgst + igst);
                }
                sb.Append(string.Format("{0}{1}", "Taxable Amt:", TaxableAmount + " "));
                sb.Append(string.Format("{0}{1}", "CGST 1.5%:", cgst + " "));
                sb.Append(string.Format("{0}{1}", "SGST 1.5%:", sgst + " "));
                sb.Append(string.Format("{0}{1}", "IGST 3%:", igst + " "));
                sb.AppendLine(string.Format("{0}{1}", "Total Amount:", amt));

                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));
                object temp1 = paymentDet.Sum(pay => pay.CardCharges);
                if (temp1 != null && temp1 != DBNull.Value)
                    CardCharges = Convert.ToDecimal(temp1);
                if (CardCharges > 0) {
                    sb.AppendLine(string.Format(new CultureInfo(company.Currency_code), "{0,-90}", "Card Charges : " + CardCharges));
                }
                sb.Append(Convert.ToChar(27).ToString() + Convert.ToChar(69).ToString());
                string strWords = string.Empty;
                NumberToWords objNWClass = new NumberToWords();
                objNWClass.ConvertNumberToWords(Convert.ToDouble(advancePaid), out strWords);
                sb.AppendLine(string.Format("{0,3}{1,-87}", "", strWords));
                sb.Append(Convert.ToChar(27).ToString() + Convert.ToChar(70).ToString());

                List<KTTU_NOTE> notes = db.KTTU_NOTE.Where(note => note.note_type == "ORD" && note.company_code == companyCode && note.branch_code == branchCode).ToList();
                if (company != null) {
                    for (int i = 0; i < 1; i++) {
                        if (notes[i].line1.ToString() != string.Empty)
                            sb.AppendLine(string.Format("{0,-90}", notes[i].line1));
                        sb.AppendLine();
                        if (Convert.ToInt32(order.bill_no) > 0)
                            sb.AppendLine(string.Format("{0,-90}", "Order is Already Closed towords Bill NO :" + Convert.ToInt32(order.bill_no)));
                        sb.AppendLine();
                        sb.AppendLine();
                        sb.Append(string.Format("{0,-45}", " Authorised Signature"));
                        sb.Append(string.Format("{0,45}", "(Customer Signature)"));
                        sb.AppendLine();
                        sb.AppendLine();
                        if (notes[0].line1.ToString() != string.Empty)
                            sb.AppendLine(string.Format("{0,-90}", notes[i].line2));
                        if (notes[0].line2.ToString() != string.Empty)
                            sb.AppendLine(string.Format("{0,-90}", notes[i].line3));
                        if (notes[0].line3.ToString() != string.Empty)
                            sb.AppendLine(string.Format("{0,-90}", notes[i].line4));
                        if (notes[0].line4.ToString() != string.Empty)
                            sb.AppendLine(string.Format("{0,-90}", notes[i].line5));
                        if (notes[0].line5.ToString() != string.Empty)
                            sb.AppendLine(string.Format("{0,-90}", notes[i].line6));
                        if (notes[0].line6.ToString() != string.Empty)
                            sb.AppendLine(string.Format("{0,-90}", notes[i].line7));
                    }
                }
                sb.Append(Convert.ToChar(18).ToString());
                return sb.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        #endregion

        #endregion

        #region ChitAdjustment

        public List<ChitClosureVM> GetBranch(string currentBranch)
        {
            List<ChitClosureVM> lstOfCC = new List<ChitClosureVM>();
            var result = (from cc in db.CHTU_CHIT_CLOSURE where cc.Closed_At == currentBranch orderby cc.branch_code select cc.branch_code).Distinct().ToList();
            foreach (string str in result) {
                ChitClosureVM cc = new ChitClosureVM();
                cc.BranchCode = str;
                lstOfCC.Add(cc);
            }
            return lstOfCC;
        }

        public List<ChitClosureVM> GetSchemeCode(string branchCode, string currentBranch)
        {
            List<ChitClosureVM> lstOfCC = new List<ChitClosureVM>();
            var result = (from cc in db.CHTU_CHIT_CLOSURE
                          where cc.Bill_Number == "0" && cc.branch_code == branchCode && cc.Closed_At == currentBranch
                          orderby cc.Scheme_Code
                          select cc.Scheme_Code).Distinct().ToList();
            foreach (string str in result) {
                ChitClosureVM cc = new ChitClosureVM();
                cc.SchemeCode = str;
                lstOfCC.Add(cc);
            }
            return lstOfCC;
        }

        public List<ChitClosureVM> GetGroupCode(string branchCode, string schemeCode, string currentBranch)
        {
            List<ChitClosureVM> lstOfCC = new List<ChitClosureVM>();
            var result = (from cc in db.CHTU_CHIT_CLOSURE
                          where cc.Bill_Number == "0" && cc.branch_code == branchCode && cc.Scheme_Code == schemeCode && cc.Closed_At == currentBranch
                                                                                   && (cc.Closing_Mode == "B"
                                                                                        || cc.Closing_Mode == "O")
                          orderby cc.Group_Code
                          select cc.Group_Code).Distinct().ToList();

            foreach (string str in result) {
                ChitClosureVM cc = new ChitClosureVM();
                cc.GroupCode = str;
                lstOfCC.Add(cc);
            }
            return lstOfCC;
        }

        public List<ChitClosureVM> GetStartMSNNo(string branchCode, string schemeCode, string groupCode, string currentBranch)
        {
            List<ChitClosureVM> lstOfCC = new List<ChitClosureVM>();
            List<CHTU_CHIT_CLOSURE> lstOfChitClosure = db.CHTU_CHIT_CLOSURE.Where(ccc => ccc.Bill_Number == "0"
                                                                                   && ccc.branch_code == branchCode
                                                                                   && ccc.Scheme_Code == schemeCode
                                                                                   && ccc.Group_Code == groupCode
                                                                                   && ccc.Closed_At == currentBranch
                                                                                   && (ccc.Closing_Mode == "B"
                                                                                        || ccc.Closing_Mode == "O")
                                                                                  ).OrderBy(cc => cc.Chit_MembShipNo).ToList();
            foreach (CHTU_CHIT_CLOSURE chitClosure in lstOfChitClosure) {
                ChitClosureVM cc = new ChitClosureVM();
                cc.ChitMembShipNo = chitClosure.Chit_MembShipNo;
                lstOfCC.Add(cc);
            }
            return lstOfCC;
        }

        public ChitAdjustVM GetChitAmount(string branchCode, string schemeCode, string groupCode, int startMSNNo, int endMSNNo, string currentBranch)
        {
            List<CHTU_CHIT_CLOSURE> chitClosure = db.CHTU_CHIT_CLOSURE.Where(ccc => ccc.Bill_Number == "0"
                                                                                     && ccc.branch_code == branchCode
                                                                                     && ccc.Scheme_Code == schemeCode
                                                                                     && ccc.Group_Code == groupCode
                                                                                     && ccc.Closed_At == currentBranch
                                                                                     && (ccc.Closing_Mode == "B"
                                                                                          || ccc.Closing_Mode == "O")
                                                                                     && ccc.Chit_MembShipNo >= startMSNNo
                                                                                     && ccc.Chit_MembShipNo <= endMSNNo
                                                                                 ).ToList();
            ChitAdjustVM ca = new ChitAdjustVM();
            foreach (CHTU_CHIT_CLOSURE cc in chitClosure) {
                ca.ChitAmount = ca.ChitAmount + Convert.ToDecimal(cc.Amt_Received);
                ca.BonusAmount = ca.BonusAmount + Convert.ToDecimal(cc.Bonus_Amt);
                ca.WinnerAmount = ca.WinnerAmount + Convert.ToDecimal(cc.win_amt);
                ca.TotalAmount = ca.TotalAmount + Convert.ToDecimal(cc.Amt_Received + cc.win_amt + cc.Bonus_Amt);
            }
            return ca;
        }

        public List<PaymentVM> GetChitClosureDetails(string branchCode, string schemeCode, string groupCode, int startMSNNo, int endMSNNo, string currentBranch)
        {
            List<CHTU_CHIT_CLOSURE> chitClosure = db.CHTU_CHIT_CLOSURE.Where(ccc => ccc.Bill_Number == "0"
                                                                                     && ccc.branch_code == branchCode
                                                                                     && ccc.Scheme_Code == schemeCode
                                                                                     && ccc.Group_Code == groupCode
                                                                                     && ccc.Closed_At == currentBranch
                                                                                     && (ccc.Closing_Mode == "B"
                                                                                          || ccc.Closing_Mode == "O")
                                                                                     && ccc.Chit_MembShipNo >= startMSNNo
                                                                                     && ccc.Chit_MembShipNo <= endMSNNo
                                                                                 ).ToList();
            List<PaymentVM> lstOfPayment = new List<PaymentVM>();
            foreach (CHTU_CHIT_CLOSURE cc in chitClosure) {
                PaymentVM payment = new PaymentVM();
                payment.ObjID = "";
                payment.CompanyCode = "";
                payment.BranchCode = "";
                payment.SeriesNo = 0;
                payment.ReceiptNo = 0;
                payment.SNo = 0;
                payment.TransType = "O";
                payment.PayMode = "CT";
                payment.PayDetails = "";
                payment.PayDate = DateTime.Now;
                payment.PayAmount = cc.Chit_Amt;
                payment.RefBillNo = cc.Chit_MembShipNo.ToString();
                payment.PartyCode = cc.branch_code;
                payment.BillCounter = "";
                payment.IsPaid = "";
                payment.Bank = "";
                payment.BankName = "";
                payment.ChequeDate = SIGlobals.Globals.GetDateTime();
                payment.CardType = "";
                payment.ExpiryDate = SIGlobals.Globals.GetDateTime();
                payment.CFlag = "N";
                payment.CardAppNo = "";
                payment.SchemeCode = cc.Scheme_Code;
                payment.SalBillType = "";
                payment.OperatorCode = "";
                payment.SessionNo = 0;
                payment.UpdateOn = SIGlobals.Globals.GetDateTime();
                payment.GroupCode = cc.Group_Code;
                payment.AmtReceived = cc.Amt_Received;
                payment.BonusAmt = cc.Bonus_Amt;
                payment.WinAmt = cc.win_amt;
                payment.CTBranch = cc.branch_code;
                payment.FinYear = null;
                payment.CardCharges = null;
                payment.ChequeNo = "";
                payment.NewBillNo = "";
                payment.AddDisc = 0;
                payment.IsOrderManual = "";
                payment.CurrencyValue = 0;
                payment.ExchangeRate = 0;
                payment.CurrencyType = "";
                payment.TaxPercentage = 0;
                payment.CancelledBy = "";
                payment.CancelledRemarks = "";
                payment.CancelledDate = "";
                payment.IsExchange = "";
                payment.ExchangeNo = 0;
                payment.NewReceiptNo = "";
                payment.GiftAmount = cc.Gift_Amount;
                payment.CardSwipedBy = "";
                payment.Version = 0;
                payment.GSTGroupCode = "";
                payment.SGSTPercent = null;
                payment.CGSTPercent = null;
                payment.IGSTPercent = null;
                payment.HSN = "";
                payment.SGSTAmount = 0;
                payment.CGSTAmount = 0;
                payment.IGSTAmount = 0;
                payment.PayAmountBeforeTax = 0;
                payment.PayTaxAmount = 0;
                lstOfPayment.Add(payment);
            }
            return lstOfPayment;
        }
        #endregion

        #region Fixed Order
        public List<FixedOrderVM> GetAllOrderAccountPosting(string companyCode, string branchCode, out ErrorVM error)
        {
            error = new ErrorVM();
            try {
                var data = (from oap in db.ORDER_TYPE_ACC_POSTING
                            join acc in db.KSTU_ACC_LEDGER_MASTER
                            on new { CompanyCode = oap.company_code, BranchCode = oap.branch_code, AccCode = oap.acc_code }
                            equals new { CompanyCode = acc.company_code, BranchCode = acc.branch_code, AccCode = acc.acc_code }
                            where oap.company_code == companyCode && oap.branch_code == branchCode
                            group new { oap, acc } by new
                            {
                                ObjID = oap.obj_id,
                                CompanyCode = oap.company_code,
                                BranchCode = oap.branch_code,
                                BookingCode = oap.booking_code,
                                BookingName = oap.booking_name,
                                AccCode = oap.acc_code,
                                AccName = acc.acc_name,
                                ObjStatus = oap.obj_status,
                                OrderRateType = oap.order_rate_type
                            } into gr
                            select new FixedOrderVM()
                            {
                                ObjID = gr.Key.ObjID,
                                CompanyCode = gr.Key.CompanyCode,
                                BranchCode = gr.Key.BranchCode,
                                BookingCode = gr.Key.BookingCode,
                                BookingName = gr.Key.BookingName,
                                AccCode = gr.Key.AccCode,
                                AccName = gr.Key.AccName,
                                ObjStatus = gr.Key.ObjStatus,
                                OrderRateType = gr.Key.OrderRateType
                            }).ToList();
                error.ErrorStatusCode = System.Net.HttpStatusCode.OK;
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }
        public dynamic GetRateType(string companyCode, string branchCode, out ErrorVM error)
        {
            error = new ErrorVM();
            var data = new List<dynamic>();
            try {
                data.Add(new { Code = "Delivery", Name = "Delivery" });
                data.Add(new { Code = "Fixed", Name = "Fixed" });
                data.Add(new { Code = "Flexi", Name = "Flexi" });
                data.Add(new { Code = "All", Name = "All" });
                error.ErrorStatusCode = System.Net.HttpStatusCode.OK;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
            return data;
        }
        public dynamic GetLedgerNames(string companyCode, string branchcode, out ErrorVM error)
        {
            error = new ErrorVM();
            try {
                var data = db.KSTU_ACC_LEDGER_MASTER.Where(led => led.company_code == companyCode
                                                            && led.branch_code == branchcode
                                                            && led.ledger_type == "GN"
                                                            && led.obj_status == "O")
                                                .Select(ld => new { AccCode = ld.acc_code, AccName = ld.acc_name })
                                                .ToList().OrderBy(ld => ld.AccName);
                error.ErrorStatusCode = System.Net.HttpStatusCode.OK;
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }
        public bool SaveOrderTypeAccountPosting(FixedOrderVM order, out ErrorVM error)
        {
            error = new ErrorVM();
            ORDER_TYPE_ACC_POSTING orderTypeAccPos = db.ORDER_TYPE_ACC_POSTING.Where(ord => ord.company_code == order.CompanyCode
                                                        && ord.branch_code == order.BranchCode
                                                        && ord.booking_code == order.BookingCode).FirstOrDefault();
            if (orderTypeAccPos != null) {
                error.description = string.Format("Plan Code already Exist. {0}  ", order.BookingCode);
                error.ErrorStatusCode = System.Net.HttpStatusCode.BadRequest;
                return false;
            }
            else {
                orderTypeAccPos = new ORDER_TYPE_ACC_POSTING();
            }
            orderTypeAccPos.obj_id = SIGlobals.Globals.GetNewGUID();
            orderTypeAccPos.company_code = order.CompanyCode;
            orderTypeAccPos.branch_code = order.BranchCode;
            orderTypeAccPos.booking_code = order.BookingCode;
            orderTypeAccPos.booking_name = order.BookingName;
            order.OperatorCode = order.OperatorCode;
            orderTypeAccPos.acc_code = order.AccCode;
            orderTypeAccPos.UpdateOn = SIGlobals.Globals.GetDateTime();
            orderTypeAccPos.obj_status = order.ObjStatus;
            orderTypeAccPos.order_rate_type = order.OrderRateType;
            db.ORDER_TYPE_ACC_POSTING.Add(orderTypeAccPos);
            try {
                db.SaveChanges();
                error.ErrorStatusCode = System.Net.HttpStatusCode.OK;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
            return true;
        }
        public bool UpdateOrderTypeAccountPosting(string objID, FixedOrderVM order, out ErrorVM error)
        {
            error = new ErrorVM();
            ORDER_TYPE_ACC_POSTING orderTypeAccPos = db.ORDER_TYPE_ACC_POSTING.Where(ord => ord.obj_id == objID
                                                                                    && ord.company_code == order.CompanyCode
                                                                                    && ord.branch_code == ord.branch_code).FirstOrDefault();
            if (orderTypeAccPos == null) {
                error.description = string.Format("Invalid Account Posting Details", order.BookingName);
                error.ErrorStatusCode = System.Net.HttpStatusCode.BadRequest;
                return false;
            }
            try {
                orderTypeAccPos.booking_name = order.BookingName;
                order.OperatorCode = order.OperatorCode;
                orderTypeAccPos.acc_code = order.AccCode;
                orderTypeAccPos.UpdateOn = SIGlobals.Globals.GetDateTime();
                orderTypeAccPos.obj_status = order.ObjStatus;
                orderTypeAccPos.order_rate_type = order.OrderRateType;
                db.Entry(orderTypeAccPos).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                error.ErrorStatusCode = System.Net.HttpStatusCode.OK;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
            return true;
        }
        #endregion

        #region Fixed Order Type Master
        public dynamic GetPlanNames(string companyCode, string branchCode, out ErrorVM error)
        {
            error = new ErrorVM();
            try {
                var otap = db.ORDER_TYPE_ACC_POSTING.Where(ot => ot.company_code == companyCode
                                                            && ot.branch_code == branchCode
                                                            && ot.obj_status == "O" && ot.booking_name != "NORMAL")
                                                    .Select(ot => new { Code = ot.booking_code, Name = ot.booking_name })
                                                    .ToList();
                error.ErrorStatusCode = System.Net.HttpStatusCode.OK;
                return otap;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }
        public dynamic GetOrderTypeDetails(string companyCode, string branchCode, string type, out ErrorVM error)
        {
            error = new ErrorVM();
            try {
                var data = (from korm in db.KSTU_ORDER_RATE_MASTER
                            join otap in db.ORDER_TYPE_ACC_POSTING on
                            new { CompanyCode = companyCode, BranchCode = branchCode, BookingCode = korm.booking_name }
                            equals new { CompanyCode = companyCode, BranchCode = branchCode, BookingCode = otap.booking_code }
                            where korm.company_code == companyCode && korm.branch_code == branchCode && korm.booking_name == type
                            group new
                            {
                                korm,
                                otap
                            } by new
                            {
                                ID = korm.Id,
                                CompanyCode = korm.company_code,
                                BranchCode = korm.branch_code,
                                BookingCode = korm.booking_name,
                                BookingName = otap.booking_name,
                                Description = korm.description,
                                AdvAmountPer = korm.adv_amount_per,
                                FixedDays = korm.fixed_days,
                                Cflag = korm.cflag,
                                OperatorCode = korm.operator_code,
                                MinWeight = (decimal)(korm.min_weight),
                            } into gr
                            select new OrderRateMasterVM()
                            {
                                ID = gr.Key.ID,
                                CompanyCode = gr.Key.CompanyCode,
                                BranchCode = gr.Key.BranchCode,
                                BookingCode = gr.Key.BookingCode,
                                BookingName = gr.Key.BookingName,
                                Description = gr.Key.Description,
                                AdvAmountPer = gr.Key.AdvAmountPer,
                                FixedDays = gr.Key.FixedDays,
                                Cflag = gr.Key.Cflag,
                                OperatorCode = gr.Key.OperatorCode,
                                MinWeight = gr.Key.MinWeight,

                            }).ToList();

                error.ErrorStatusCode = System.Net.HttpStatusCode.OK;
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }
        public bool SaveOrderTypeDetails(OrderRateMasterVM order, out ErrorVM error)
        {
            error = new ErrorVM();
            try {
                KSTU_ORDER_RATE_MASTER orm = db.KSTU_ORDER_RATE_MASTER.Where(ot => ot.company_code == order.CompanyCode
                                                            && ot.branch_code == order.BranchCode && ot.booking_name == order.BookingCode).FirstOrDefault();
                if (orm != null) {
                    error.description = string.Format("Advance percent already exist for {0}  ", order.BookingName);
                    error.ErrorStatusCode = System.Net.HttpStatusCode.BadRequest;
                    return false;
                }
                else {
                    orm = new KSTU_ORDER_RATE_MASTER();
                }
                orm.company_code = order.CompanyCode;
                orm.branch_code = order.BranchCode;
                orm.booking_name = order.BookingCode;
                orm.description = order.Description;
                orm.adv_amount_per = order.AdvAmountPer;
                orm.fixed_days = order.FixedDays;
                orm.cflag = order.Cflag;
                orm.operator_code = order.OperatorCode;
                orm.UpdateOn = SIGlobals.Globals.GetDateTime();
                orm.min_weight = order.MinWeight;
                db.KSTU_ORDER_RATE_MASTER.Add(orm);
                db.SaveChanges();
                error.ErrorStatusCode = System.Net.HttpStatusCode.OK;
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }
        public bool UpdateOrderTypeDetails(int id, OrderRateMasterVM order, out ErrorVM error)
        {
            error = new ErrorVM();
            KSTU_ORDER_RATE_MASTER orm = db.KSTU_ORDER_RATE_MASTER.Where(ot => ot.company_code == order.CompanyCode
                                                            && ot.branch_code == order.BranchCode
                                                            && ot.Id == id).FirstOrDefault();
            if (orm == null) {
                error.description = string.Format("Invalid Order Details", order.BookingName);
                error.ErrorStatusCode = System.Net.HttpStatusCode.BadRequest;
                return false;
            }
            try {
                orm.description = order.Description;
                orm.fixed_days = order.FixedDays;
                orm.cflag = order.Cflag;
                orm.operator_code = order.OperatorCode;
                orm.UpdateOn = SIGlobals.Globals.GetDateTime();
                orm.min_weight = order.MinWeight;
                db.Entry(orm).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                error.ErrorStatusCode = System.Net.HttpStatusCode.OK;
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }
        #endregion

        #region Marketplace Integration

        #region Received Order
        public dynamic GetAllReceivedOrders(string companyCode, string branchCode, DateTime fromDate, DateTime toDate, int marketPlaceID, string type, out ErrorVM error)
        {
            error = new ErrorVM();
            List<ReceivedOrderForGridVM> grdData = new List<ReceivedOrderForGridVM>();
            int?[] orderSource = { 1, 3 }; //0->Store,1-> Amazon,2->Instore,3->Bhima E-Commerce
            try {

                var data = (from om in db.KTTU_ORDER_MASTER
                            join od in db.KTTU_ORDER_DETAILS
                            on new { CompanyCode = om.company_code, BranchCode = om.branch_code, Order = om.order_no }
                            equals new { CompanyCode = od.company_code, BranchCode = od.branch_code, Order = od.order_no }
                            where om.company_code == companyCode
                                  && om.branch_code == branchCode
                                  && System.Data.Entity.DbFunctions.TruncateTime(od.due_date_for_shipment) >= System.Data.Entity.DbFunctions.TruncateTime(fromDate)
                                  && System.Data.Entity.DbFunctions.TruncateTime(od.due_date_for_shipment) <= System.Data.Entity.DbFunctions.TruncateTime(toDate)
                                  && od.due_date_for_shipment != null
                                  && orderSource.AsQueryable().Contains(om.order_source)
                            select new ReceivedOrderForGridVM()
                            {
                                CompanyCode = om.company_code,
                                BranchCode = om.branch_code,
                                SlNo = od.slno,
                                OrderNo = om.order_no,
                                OrderRefNo = om.order_reference_no,
                                OrderSourceMarket = om.order_source,
                                OrderDate = om.order_date,
                                ShipmentDate = od.due_date_for_shipment,
                                ItemName = od.item_name,
                                BarcodeNo = od.barcode_no,
                                Gwt = od.from_gwt,
                                Cflag = om.cflag,
                                IsShipped = od.isShipped,
                                IsScheduleForPickUp = od.isScheduledForPickUp,
                                IsPacked = od.isPacked,
                                IsPicked = od.isPicked,
                                IsProcessed = od.isProcessed,
                                CentralRefNo = om.central_ref_no,
                                Qty = od.quantity,
                                ItemCode = ""
                            }).OrderBy(d => d.OrderNo);

                switch (type) {
                    case "ALL":
                        grdData = AddItemCode(data.ToList(), marketPlaceID, companyCode, branchCode);
                        break;
                    case "OPEN":
                        grdData = AddItemCode(data.Where(d => d.IsProcessed == false
                                                        && d.IsPicked == false
                                                        && d.IsPacked == false
                                                        && d.IsScheduleForPickUp == false
                                                        && d.IsShipped == false
                                                        && d.Cflag != "Y").ToList(), marketPlaceID, companyCode, branchCode);
                        break;
                    case "UNDERPROCESS":
                        grdData = AddItemCode(data.Where(d => d.IsProcessed == true).ToList(), marketPlaceID, companyCode, branchCode);
                        break;
                    case "PICKED":
                        grdData = AddItemCode(data.Where(d => d.IsPicked == true).ToList(), marketPlaceID, companyCode, branchCode);
                        break;
                    case "PACKED":
                        grdData = AddItemCode(data.Where(d => d.IsPacked == true).ToList(), marketPlaceID, companyCode, branchCode);
                        break;
                    case "READY":
                        grdData = AddItemCode(data.Where(d => d.IsScheduleForPickUp == true).ToList(), marketPlaceID, companyCode, branchCode);
                        break;
                    case "SHIPPED":
                        grdData = AddItemCode(data.Where(d => d.IsShipped == true).ToList(), marketPlaceID, companyCode, branchCode);
                        break;
                    case "CANCELLED":
                        grdData = AddItemCode(data.Where(d => d.Cflag == "Y").ToList(), marketPlaceID, companyCode, branchCode);
                        break;
                }
                error.ErrorStatusCode = System.Net.HttpStatusCode.OK;
                return grdData;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }
        public int SaveItemsToPickList(List<ReceivedOrderForGridVM> data, string user, out ErrorVM error)
        {
            error = new ErrorVM();
            if (data.Count == 0) {
                error.description = "Pick the Items to generate Picklist.";
                error.ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError;
                return 0;
            }
            string companyCode = data[0].CompanyCode;
            string branchCode = data[0].BranchCode;
            int assignmentNo = Convert.ToInt32(SIGlobals.Globals.GetFinancialYear(db, companyCode, branchCode).ToString().Remove(0, 1)
                + db.KSTS_SEQ_NOS.Where(sq => sq.obj_id == "2020" && sq.company_code == companyCode && sq.branch_code == branchCode).FirstOrDefault().nextno);
            try {
                int slno = 1;
                foreach (ReceivedOrderForGridVM d in data) {
                    for (int i = 0; i < d.Qty; i++) {
                        OrderItemPickList pickList = new OrderItemPickList();
                        pickList.obj_id = SIGlobals.Globals.GetMagnaGUID("OrderItemPickList", assignmentNo, companyCode, branchCode);
                        pickList.company_code = companyCode;
                        pickList.branch_code = branchCode;
                        pickList.assignment_no = assignmentNo;
                        pickList.order_no = d.OrderNo;
                        pickList.order_item_sl_no = slno;
                        pickList.gs_code = d.GSCode;
                        pickList.item_name = d.ItemName;
                        pickList.counter_code = d.CounterCode;
                        pickList.barcode_no = "";
                        pickList.gwt = d.Gwt;
                        pickList.isPicked = false;
                        pickList.picked_date = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                        pickList.picked_by = user;
                        pickList.sku = d.BarcodeNo;
                        db.OrderItemPickLists.Add(pickList);
                        slno++;
                    }
                    List<KTTU_ORDER_DETAILS> orderDetails = db.KTTU_ORDER_DETAILS.Where(ord => ord.company_code == companyCode
                                                                                && ord.branch_code == branchCode
                                                                                && ord.order_no == d.OrderNo).ToList();
                    foreach (KTTU_ORDER_DETAILS ord in orderDetails) {
                        ord.isProcessed = true;
                        db.Entry(ord).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                SIGlobals.Globals.UpdateSeqenceNumber(db, "2020", companyCode, branchCode);


                error.ErrorStatusCode = System.Net.HttpStatusCode.OK;
                db.SaveChanges();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return 0;
            }
            return assignmentNo;
        }
        public string PrintOrderItemPickList(string companyCode, string branchCode, int assignmentNo, out ErrorVM error)
        {
            error = new ErrorVM();
            try {
                string Heading = string.Format("{0}- Pick list details for Assignment no : {1}", branchCode, assignmentNo);
                KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode && c.branch_code == branchCode).FirstOrDefault();
                var data = GetPickListItem(companyCode, branchCode, assignmentNo);
                string[] header = { "SlNo", "AssignmentNo", "OrderNo", "Counter", "Item", "SKU", "Barcode", "Gross Wt", "Status" };
                int columnCount = 9;
                string[] Alignment = new string[columnCount];
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<HTML>");
                sb.AppendLine("<HEAD>");
                sb.AppendLine("<style = 'text/css'>\n");
                sb.AppendLine(".boldText, .boldtable TD, .boldtable TH\n");
                sb.AppendLine("{\n");
                sb.AppendLine("font-family: 'Times New Roman';\n");
                sb.AppendLine("font-size:12pt;\n");
                sb.AppendLine("}\n");
                sb.AppendLine(".tableStyle\n");
                sb.AppendLine("{\n");
                sb.AppendLine("border-left: 1px solid black;\n");
                sb.AppendLine("border-right: 1px solid black;\n");
                sb.AppendLine("border-top-style:none;\n");
                sb.AppendLine("border-bottom-style:none;\n");
                sb.AppendLine("border-collapse: collapse;\n");
                sb.AppendLine("}\n");
                sb.AppendLine(".table,TD,TH\n");
                sb.AppendLine("{\n");
                sb.AppendLine("border-left-style:none;\n");
                sb.AppendLine("border-right: 1px solid black;\n");
                sb.AppendLine("border-bottom-style:none;\n");
                sb.AppendLine("border-top-style:1px solid;\n");
                sb.AppendLine("border-collapse: collapse;\n");
                sb.AppendLine("}\n");
                sb.AppendLine(".noborder\n");
                sb.AppendLine("{\n");
                sb.AppendLine("border-left-style:none;\n");
                sb.AppendLine("border-right-style:none;\n");
                sb.AppendLine("border-top-style:none;;\n");
                sb.AppendLine("border-bottom-style:none;\n");
                sb.AppendLine("border-collapse: collapse;\n");
                sb.AppendLine("}\n");
                sb.AppendLine(".fullborder\n");
                sb.AppendLine("{\n");
                sb.AppendLine("border-left-style:none;;\n");
                sb.AppendLine("border-right: 1px solid black;\n");
                sb.AppendLine("border-bottom: 1px solid black;\n");
                sb.AppendLine("border-top: 1px solid black;\n");
                sb.AppendLine("border-collapse: collapse;\n");
                sb.AppendLine("}\n");
                sb.AppendLine("</style>\n");
                sb.AppendLine("</HEAD>");
                sb.AppendLine("<BODY>");
                if (columnCount <= 10) {
                    sb.AppendLine(string.Format("<table style=\"width:{0}%\" bgcolor=\"#FDFDF0\" class=\"boldText\" frame=\"border\" border=\"1\" align=\"LEFT\" >"
                        , columnCount * 15));
                }
                else {
                    sb.AppendLine(string.Format("<table style=\"width:80%\" bgcolor=\"#FDFDF0\" class=\"boldText\" frame=\"border\" border=\"1\" align=\"LEFT\" >"));
                }
                sb.AppendLine("<tr valign=\"bottom\" >");
                sb.AppendLine(string.Format("<ph/><th colspan=30 bgcolor=\"EBE7C6\"><h4 style=\"color:maroon\">{0}</h4></th>"
                    , Convert.ToString(company.company_name)));
                sb.AppendLine("</tr>");
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<ph/><td class=\"noborder\" colspan=30 align=\"CENTER\" ><b>{0}</b> </td>", Heading));
                sb.AppendLine("</TR>");
                sb.AppendLine("<tr bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\" class = \"fullborderStyle\">");
                string align = "CENTER";
                for (int t = 0; t < columnCount; t++) {
                    sb.AppendLine(string.Format("<pth/><td ALIGN=\"{0}\" class=\"fullborderStyle\"> <b>{1}</b> </td>", align, header[t]));
                    Alignment[t] = align;
                }
                sb.AppendLine("</tr>");
                int slno = 1;
                foreach (var d in data) {
                    sb.AppendLine("<tr>");
                    sb.AppendLine(string.Format("<td ALIGN=\"RIGHT\"> <b>{0}<b> </td>", slno, "&nbsp"));
                    sb.AppendLine(string.Format("<td ALIGN=\"RIGHT\"> <b>{0}<b> </td>", d.AssignmentNo, "&nbsp"));
                    sb.AppendLine(string.Format("<td ALIGN=\"RIGHT\"> <b>{0}<b> </td>", d.OrderNo, "&nbsp"));
                    sb.AppendLine(string.Format("<td ALIGN=\"RIGHT\"> <b>{0}<b> </td>", d.CounterCode, "&nbsp"));
                    sb.AppendLine(string.Format("<td ALIGN=\"RIGHT\"> <b>{0}<b> </td>", d.ItemName, "&nbsp"));
                    sb.AppendLine(string.Format("<td ALIGN=\"RIGHT\"> <b>{0}<b> </td>", d.SKU, "&nbsp"));
                    sb.AppendLine(string.Format("<td ALIGN=\"RIGHT\"> <b>{0}<b> </td>", d.BarcodeNo, "&nbsp"));
                    sb.AppendLine(string.Format("<td ALIGN=\"RIGHT\"> <b>{0}<b> </td>", d.Gwt, "&nbsp"));
                    sb.AppendLine(string.Format("<td ALIGN=\"RIGHT\"> <b>{0}<b> </td>", d.IsPicked == false ? "Pending" : "Picked", "&nbsp"));
                    sb.AppendLine("</tr>");
                    slno = slno + 1;
                }
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD ALIGN=\"LEFT\" colspan={1} bgcolor=\"EBE7C6\" ><b style=\"color:'maroon'\"> {0}(Run Date: {2}) </b></TD>", "", columnCount, SIGlobals.Globals.GetDateTime().ToString("dd/MM/yyyy hh:mm:ss tt")));
                sb.AppendLine("</TR>");
                sb.AppendLine("</table>");
                sb.AppendLine("</BODY>");
                sb.AppendLine("</HTML>");
                error.ErrorStatusCode = System.Net.HttpStatusCode.OK;
                return sb.ToString();
            }
            catch (Exception excp) {
                error.description = excp.Message;
                error.ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError;
                return null;
            }
        }
        #endregion

        #region Item Pick List
        public dynamic GetPickListNo(string companyCode, string branchCode, out ErrorVM error)
        {
            error = new ErrorVM();
            try {
                var data = (from item in db.OrderItemPickLists
                            join od in db.KTTU_ORDER_MASTER on
                            new { CompanyCode = item.company_code, BranchCode = item.branch_code == branchCode, OrderNo = item.order_no }
                            equals new { CompanyCode = od.company_code, BranchCode = od.branch_code == branchCode, OrderNo = od.order_no }
                            join det in db.KTTU_ORDER_DETAILS on
                             new { CompanyCode = od.company_code, BranchCode = od.branch_code == branchCode, OrderNo = od.order_no }
                            equals new { CompanyCode = det.company_code, BranchCode = det.branch_code == branchCode, OrderNo = det.order_no }
                            where det.isPacked == false && (det.isPicked == false || det.isPacked == null)
                            select new
                            {
                                AssignmentCode = item.assignment_no,
                                AssignmentNo = item.assignment_no
                            }).ToList().Select(x => new { x.AssignmentCode, x.AssignmentNo }).Distinct();

                error.ErrorStatusCode = System.Net.HttpStatusCode.OK;
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }
        public List<OrderItemPickListVM> GetPickListDetByAssignmentNo(string companyCode, string branchCode, int assignmentNo, out ErrorVM error)
        {
            error = new ErrorVM();
            var data = GetPickListItem(companyCode, branchCode, assignmentNo);
            if (data == null) {
                error.description = "Details not found.";
                error.ErrorStatusCode = System.Net.HttpStatusCode.BadRequest;
                return null;
            }
            error.ErrorStatusCode = System.Net.HttpStatusCode.OK;
            return data;
        }
        public bool UpdatePickedItem(List<OrderItemPickListVM> pickedItems, out ErrorVM error)
        {
            error = new ErrorVM();
            try {
                foreach (OrderItemPickListVM pickedItem in pickedItems) {
                    OrderItemPickList picked = db.OrderItemPickLists.Where(p => p.company_code == pickedItem.CompanyCode
                                                                            && p.branch_code == pickedItem.BranchCode
                                                                            && p.assignment_no == pickedItem.AssignmentNo
                                                                            && p.order_item_sl_no == pickedItem.OrderItemSlno).FirstOrDefault();
                    picked.barcode_no = pickedItem.BarcodeNo;
                    picked.isPicked = true;
                    db.Entry(picked).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
                error.ErrorStatusCode = System.Net.HttpStatusCode.OK;
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }
        #endregion

        #region Item Packing
        public List<PackingItemVM> GetItemDetailsForPacking(string companyCode, string branchCode, out ErrorVM error)
        {
            error = new ErrorVM();
            List<PackingItemVM> lstPackingItem = new List<PackingItemVM>();
            try {

                var data = db.GetOrderItemsForPacking(companyCode, branchCode, 0);

                if (data != null) {
                    foreach (var d in data) {
                        PackingItemVM packingItem = new PackingItemVM();
                        packingItem.CompanyCode = companyCode;
                        packingItem.BranchCode = branchCode;
                        packingItem.OrderNo = d.order_no;
                        packingItem.OrderReferenceNo = d.order_reference_no;
                        packingItem.ItemName = d.item_name;
                        packingItem.BarcodeNo = d.barcode_no;
                        packingItem.Qty = d.quantity;
                        packingItem.MarketplaceSlNo = Convert.ToInt32(d.MarketPlaceSNo);
                        packingItem.CentralRefNo = Convert.ToInt32(d.central_ref_no);
                        packingItem.PackageCode = d.package_code;
                        packingItem.OTLNo = d.OTLNos;
                        packingItem.Length = d.length;
                        packingItem.LengthUom = d.length_uom;
                        packingItem.Width = d.width;
                        packingItem.WidthUom = d.width_uom;
                        packingItem.Height = d.height;
                        packingItem.HeightUom = d.height_uom;
                        packingItem.Weight = d.weight;
                        packingItem.WeightUom = d.weight_uom;
                        packingItem.PackageID = d.package_id;
                        packingItem.OrderSource = d.order_source;
                        lstPackingItem.Add(packingItem);
                    }
                }
                error.ErrorStatusCode = System.Net.HttpStatusCode.OK;
                return lstPackingItem;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }

        }
        public dynamic GetPackageDetails(string companyCode, string branchCode, out ErrorVM error)
        {
            error = new ErrorVM();
            try {

                var data = db.KSTU_PACKING_DETAILS.Where(p => p.company_code == companyCode
                                                            && p.branch_code == branchCode
                                                            && p.obj_status == "O")
                                                  .Select(d => new { Code = d.p_code, Name = d.p_name })
                                                  .ToList();
                error.ErrorStatusCode = System.Net.HttpStatusCode.OK;
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public dynamic GetPackingDetails(string companyCode, string branchCode, string packageCode, out ErrorVM error)
        {
            error = null;
            try {
                //var data = (from p in db.KSTU_PACKING_DETAILS
                //            where p.company_code == companyCode & p.branch_code == branchCode && p.p_code == packageCode
                //            select new
                //            {
                //                Length = p.m_length + " " + p.m_length_uom,
                //                Width = p.m_width + " " + p.m_width_uom,
                //                Height = p.m_height + " " + p.m_height_uom,
                //                Weight = p.m_weight + " " + p.m_weight_uom,
                //            }).ToList();
                var data = (from p in db.KSTU_PACKING_DETAILS
                            where p.company_code == companyCode & p.branch_code == branchCode && p.p_code == packageCode
                            select new
                            {
                                PackageId = p.p_code,
                                PackageName = p.p_name,
                                Length = p.m_length,
                                LenghUoM = p.m_length_uom,
                                Width = p.m_width,
                                WidthUoM = p.m_width_uom,
                                Height = p.m_height,
                                HeightUoM = p.m_height_uom,
                                Weight = p.m_weight,
                                WeightUoM = p.m_weight_uom,
                            }).FirstOrDefault();
                if(data == null) {
                    error = new ErrorVM { description = "No package information found"};
                    return null;

                }
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }        
        #endregion

        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        /// Order No -> Which you are going to Validate.
        /// type->Sales Estimation then send "SE", Sales Bill then send "SB", Order then send "OR", if you pass "" (empty) then its do simple validation.
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="type"></param>
        /// <param name="refNo"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool ValidateOrder(int orderNo, string companyCode, string branchCode, string type, int refNo, out ErrorVM error)
        {
            // Alternate to [dbo].[usp_validateOrderNo] Procedure
            error = null;
            if (orderNo == 0) {
                return true;
            }
            KTTU_ORDER_MASTER order = db.KTTU_ORDER_MASTER.Where(ord => ord.company_code == companyCode
                                                                    && ord.branch_code == branchCode
                                                                    && ord.order_no == orderNo).FirstOrDefault();
            if (order == null) {
                error = new ErrorVM();
                error.description = string.Format("Order No: {0} is invalid", orderNo);
                error.ErrorStatusCode = System.Net.HttpStatusCode.NotFound;
                return false;
            }

            if (order.closed_flag == "Y") {
                error = new ErrorVM();
                error.description = string.Format("Order No: {0} is already Cancelled.", orderNo);
                error.ErrorStatusCode = System.Net.HttpStatusCode.BadRequest;
                return false;
            }

            if (order.closed_flag == "Y") {
                error = new ErrorVM();
                error.description = string.Format("Order No: {0} is already Closed.", orderNo);
                error.ErrorStatusCode = System.Net.HttpStatusCode.BadRequest;
                return false;
            }

            if (order.is_lock == "Y") {
                error = new ErrorVM();
                error.description = string.Format("Order No: {0} is Locked. Contact Admin.", orderNo);
                error.ErrorStatusCode = System.Net.HttpStatusCode.BadRequest;
                return false;
            }

            if (order.bill_no != 0) {
                error = new ErrorVM();
                error.description = string.Format("Order No: {0} is already adjusted towards Bill No: {1}", orderNo, order.bill_no, Convert.ToDateTime(order.closed_date).ToString("dd/MMM/yyy"));
                error.ErrorStatusCode = System.Net.HttpStatusCode.BadRequest;
                return false;
            }

            if (type == "SE") {
                KTTU_SALES_EST_MASTER salesEst = db.KTTU_SALES_EST_MASTER.Where(est => est.order_no == orderNo
                                                                                && est.company_code == companyCode
                                                                                && est.branch_code == branchCode).FirstOrDefault();
                if (salesEst != null && salesEst.est_no != refNo) {
                    error.description = string.Format("Order No: {0} is already adjusted towards Est No: {1}", orderNo, refNo, Convert.ToDateTime(order.closed_date).ToString("dd/MMM/yyy"));
                    error.ErrorStatusCode = System.Net.HttpStatusCode.BadRequest;
                    return false;
                }

                int finYear = SIGlobals.Globals.GetFinancialYear(db, companyCode, branchCode);
                string strOrderNo = Convert.ToString(orderNo);
                KTTU_PAYMENT_DETAILS payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.Ref_BillNo == strOrderNo
                                                                            && pay.trans_type == "A"
                                                                            && pay.pay_mode == "OP"
                                                                            && pay.cflag == "N"
                                                                            && pay.fin_year == finYear
                                                                            && pay.company_code == companyCode
                                                                            && pay.branch_code == branchCode).FirstOrDefault();
                if (payment != null && payment.series_no != refNo) {
                    error.description = string.Format("Order No: {0} is already adjusted towards Est No: {1}", orderNo, refNo, Convert.ToDateTime(order.closed_date).ToString("dd/MMM/yyy"));
                    error.ErrorStatusCode = System.Net.HttpStatusCode.BadRequest;
                    return false;
                }
            }
            if (type == "SB" || type == "OR") {
                // This block not required because its This validation is come under basic validation
                // This is the implementation of Magna Prcoedure "[dbo].[usp_validateOrderNo]"
            }
            return true;
        }
        public bool OrderCashValidation(MagnaDbEntities db, List<PaymentVM> payments, int custID, int orderNo, string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            decimal orderCash = 0;
            decimal totalCashPaid = 0;
            decimal maxCash = 0;
            maxCash = SIGlobals.Globals.GetTollerance(db, 201, companyCode, branchCode).Max_Val;

            totalCashPaid = Convert.ToDecimal(payments.Where(pay => pay.PayMode == "C").Sum(pay => pay.PayAmount));
            if (orderNo != 0) {
                orderCash = Convert.ToDecimal(db.KTTU_PAYMENT_DETAILS.Where(ord => ord.company_code == companyCode
                                                                               && ord.branch_code == branchCode
                                                                               && ord.series_no == orderNo
                                                                               && ord.cflag != "Y"
                                                                               && ord.trans_type == "O"
                                                                               && ord.pay_mode == "C").Sum(amt => amt.pay_amt));
            }

            if (payments.Count > 0) {
                foreach (PaymentVM payment in payments) {
                    if (payment.PayMode == "OP") {
                        int refBillNo = Convert.ToInt32(payment.RefBillNo);
                        orderCash = Convert.ToDecimal(db.KTTU_PAYMENT_DETAILS.Where(pay => pay.pay_mode == "C"
                                                                    && pay.trans_type == "O"
                                                                    && pay.series_no == refBillNo
                                                                    && pay.cflag != "Y").Sum(p => p.pay_amt));
                    }
                }
            }

            if (orderCash >= maxCash) {
                error = new ErrorVM()
                {
                    description = string.Format("Attached Orders are created through cash above {0} .Cash receipt is not allowed for more than {0}", maxCash),
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            totalCashPaid = totalCashPaid + orderCash;
            if (totalCashPaid >= maxCash) {
                error = new ErrorVM()
                {
                    description = string.Format("Cash Order and Cash receipt is not allowed for more than {0}", maxCash),
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            return true;
        }
        #endregion

        #region Private Metods
        private decimal GetTillNowPaidOrderAmount(int CustID, string companyCode, string branchCode)
        {
            int finYear = SIGlobals.Globals.GetFinancialYear(db, companyCode, branchCode);
            var tillNowPaidAmount = (from komt in db.KTTU_ORDER_MASTER
                                     join kpdt in db.KTTU_PAYMENT_DETAILS //on komt.order_no equals kpdt.series_no
                                     on new { CompanyCode = komt.company_code, BranchCode = komt.branch_code, OrderNo = komt.order_no }
                                     equals new { CompanyCode = kpdt.company_code, BranchCode = kpdt.branch_code, OrderNo = kpdt.series_no }
                                     where komt.cflag == "N"
                                     && komt.closed_flag == "N"
                                     && komt.Cust_Id == CustID
                                     && kpdt.cflag == "N"
                                     && kpdt.pay_mode == "C"
                                     && kpdt.fin_year == finYear
                                     && kpdt.trans_type == "O"
                                     && kpdt.company_code == companyCode
                                     && kpdt.branch_code == branchCode
                                     group kpdt by 1 into g
                                     select new { Amount = g.Sum(x => x.pay_amt) });
            return Convert.ToDecimal(tillNowPaidAmount.FirstOrDefault() == null ? 0 : tillNowPaidAmount.FirstOrDefault().Amount);
        }
        private ErrorVM OrderAccountPostingWithProedure(int OrderNo, int ReceiptNo, string companyCode, string branchCode)
        {
            int retValue = 0;
            try {
                retValue = db.usp_CreateAccountVoucher(OrderNo, ReceiptNo, branchCode, companyCode);
                if (retValue < 0) {
                    return new ErrorVM() { description = "Error Occurred while Updating Accounts.", field = "Account Update", index = 0 };
                }
            }
            catch (Exception excp) {
                return new ErrorVM() { description = excp.Message, field = "Account Update", index = 0 };
            }
            return null;
        }
        private ErrorVM OrderAccountPostingWithProedureNew(int OrderNo, int ReceiptNo, string companyCode, string branchCode)
        {
            ////int retValue = 0;
            //try {
            //    //ObjectParameter errorMessage = new ObjectParameter("errorMessage", typeof(string));
            //    //var result = db.usp_createOrderPostingVouchers(companyCode, branchCode, Convert.ToString(OrderNo), Convert.ToString(ReceiptNo), errorMessage);
            //    //if (errorMessage.Value != null) {
            //    //    return new ErrorVM() { description = "Error Occurred while Updating Accounts.", field = "Account Update", index = 0 };
            //    //}

            //    string sql = "EXEC [dbo].[usp_createOrderPostingVouchers] \n"
            //                + "  @p0, \n"
            //                + "  @p1,\n"
            //                + "  @p2,\n"
            //                + "  @p3,\n"
            //                + "  @p4";
            //    List<object> parameterList = new List<object>();
            //    parameterList.Add(companyCode);
            //    parameterList.Add(branchCode);
            //    parameterList.Add(OrderNo);
            //    parameterList.Add(ReceiptNo);
            //    parameterList.Add("");
            //    object[] parametersArray = parameterList.ToArray();
            //    int recordsAffected = SIGlobals.Globals.ExecuteSQL(sql, db, parametersArray);
            //}
            //catch (Exception excp) {
            //    return new ErrorVM() { description = excp.Message, field = "Account Update", index = 0 };
            //}
            //return null;

            try {
                string errorFromProc = string.Empty;
                ObjectParameter errorMessage = new ObjectParameter("errorMsg", typeof(string));
                ObjectParameter outputVal = new ObjectParameter("outValue", typeof(int));
                var result = db.usp_createOrderPostingVouchers_FuncImport(companyCode, branchCode, Convert.ToString(OrderNo), Convert.ToString(ReceiptNo), outputVal, errorMessage);
                return new CommonBL().HandleAccountPostingProcs(outputVal, errorMessage);
            }
            catch (Exception excp) {
                return new ErrorVM() { description = excp.InnerException.Message, field = "Account Update", index = 0 };
            }
        }
        private ErrorVM OrderClosureAccountPosting(int OrderNo, string companyCode, string branchCode)
        {
            int retValue = 0;
            try {
                retValue = db.usp_CreateAccountVoucherForOrderClosure(OrderNo, branchCode, companyCode);
                if (retValue < 0) {
                    return new ErrorVM() { description = "Error Occurred while Updating Accounts.", field = "Account Update", index = 0 };
                }
            }
            catch (Exception excp) {
                return new ErrorVM() { description = excp.Message, field = "Account Update", index = 0 };
            }
            return null;
        }
        private string DoImageReplacement(string branchCode, int orderNo, int slNo, string imgUrl)
        {
            string destFolder = ConfigurationManager.AppSettings["OrdImgFolder"].ToString();
            string originalImagePath = HttpContext.Current.Server.MapPath("~/" + destFolder);
            string orgFileName = Path.GetFileName(imgUrl);
            string newFileName = branchCode + "-" + orderNo + "_" + slNo + Path.GetExtension(imgUrl);
            string copyFrom = originalImagePath + "\\" + orgFileName;
            if (File.Exists(copyFrom)) {
                File.Copy(copyFrom, Path.Combine(originalImagePath, newFileName));
                File.Delete(copyFrom);
            }
            else {
                newFileName = "";
            }
            return string.IsNullOrEmpty(newFileName) ? "" : "\\" + newFileName;
        }
        private ReceivedOrderForGridVM GetITemCode(ReceivedOrderForGridVM grid, string companyCode, string branchCode)
        {
            string itemCode = string.Empty;
            KSTU_SKU_MASTER skuMaster = db.KSTU_SKU_MASTER.Where(s => s.SKU_ID == grid.BarcodeNo
                                                                    && s.branch_code == branchCode
                                                                    && s.company_code == companyCode).FirstOrDefault();

            if (skuMaster == null || skuMaster.item_code == null || skuMaster.item_code == "") {
                KTTU_BARCODE_MASTER barcodeMster = db.KTTU_BARCODE_MASTER.Where(b => b.barcode_no == grid.BarcodeNo
                                                                                && b.company_code == companyCode
                                                                                && b.branch_code == branchCode).FirstOrDefault();
                if (barcodeMster != null) {
                    grid.ItemCode = barcodeMster.item_name;
                    grid.GSCode = barcodeMster.gs_code;
                    grid.DesignCode = barcodeMster.design_name;
                    grid.CounterCode = barcodeMster.counter_code;
                }
            }
            else {
                grid.ItemCode = skuMaster.SKU_ID;
                grid.GSCode = "";
                grid.DesignCode = "";
                grid.CounterCode = "";
            }
            return grid;
        }
        private dynamic AddItemCode(List<ReceivedOrderForGridVM> data, int marketPlaceID, string companyCode, string branchCode)
        {
            List<ReceivedOrderForGridVM> newData = new List<ReceivedOrderForGridVM>();
            if (marketPlaceID != 0) {
                newData = data.Where(d => d.OrderSourceMarket == marketPlaceID).ToList();
            }
            else {
                newData = data;
            }
            foreach (var d in newData) {
                ReceivedOrderForGridVM retData = GetITemCode(d, companyCode, branchCode);
                d.ItemCode = retData.ItemName;
                d.GSCode = retData.GSCode;
                d.DesignCode = retData.DesignCode;
                d.CounterCode = retData.CounterCode;
                if (d.Cflag == "Y") {
                    d.Status = "Open";
                }
                else if (d.IsShipped == true) {
                    d.Status = "Shipped";
                }
                else if (d.IsScheduleForPickUp == true) {
                    d.Status = "Ready";
                }
                else if (d.IsPacked == true) {
                    d.Status = "Packed";
                }
                else if (d.IsProcessed == true) {
                    d.Status = "Under Process";
                }
                else {
                    d.Status = "Open";
                }
                if (d.OrderSourceMarket == 1) {
                    d.OrderSource = "Amazon";
                }
                else {
                    d.OrderSource = "Bhima";
                }
            }
            return newData;
        }
        private List<OrderItemPickListVM> GetPickListItem(string companyCode, string branchCode, int assignmentNo)
        {
            var data = db.OrderItemPickLists.Where(d => d.company_code == companyCode
                                                        && d.branch_code == branchCode
                                                        && d.assignment_no == assignmentNo)
                                                .Select(s => new OrderItemPickListVM()
                                                {
                                                    CompanyCode = s.company_code,
                                                    BranchCode = s.branch_code,
                                                    AssignmentNo = s.assignment_no,
                                                    OrderNo = s.order_no,
                                                    OrderItemSlno = s.order_item_sl_no,
                                                    GSCode = s.gs_code,
                                                    ItemName = s.item_name,
                                                    CounterCode = s.counter_code,
                                                    BarcodeNo = s.barcode_no,
                                                    Gwt = s.gwt,
                                                    IsPicked = s.isPicked,
                                                    PickedDate = s.picked_date,
                                                    PickedBy = s.picked_by,
                                                    SKU = s.sku,
                                                }).ToList();
            return data;
        }
        #endregion
    }
}
