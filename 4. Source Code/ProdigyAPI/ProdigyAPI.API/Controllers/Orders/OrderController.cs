using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.BL.ViewModel.Payment;
using ProdigyAPI.BL.ViewModel.Sales;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.OData.Query;
using System.Net.Http;
using System.Threading.Tasks;
using ProdigyAPI.BL.BusinessLayer.Order;
using System.Web;
using System.IO;
using System.Configuration;
using System.Web.Http.Description;
using ProdigyAPI.BL.ViewModel.Marketplace;
using ProdigyAPI.BL.BusinessLayer.Common;

namespace ProdigyAPI.Controllers.Orders
{
    /// <summary>    
    /// Order controller contains all API's related to Orders, Order Attachment etc.
    /// </summary> 
    [Authorize]
    [RoutePrefix("api/order")]
    public class OrderController : SIBaseApiController<OrderMasterVM>, IBaseMasterActionController<OrderMasterVM, OrderMasterVM>
    {
        #region Order Controller Methods

        #region Default Controller Methods
        /// <summary>
        /// List of All Orders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllOrders/{companyCode}/{branchCode}")]
        public IQueryable<OrderMasterVM> List(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            IQueryable<OrderMasterVM> om = new OrderBL().GetOrderAllOrders(companyCode, branchCode, out error);
            if (error != null) {
                return null;
            }
            return om;
        }

        public IHttpActionResult Count(ODataQueryOptions<OrderMasterVM> oDataOptions)
        {
            throw new NotImplementedException();
        }
        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Post(OrderMasterVM t)
        {
            throw new NotImplementedException();
        }

        //public IHttpActionResult Put(int id, OrderMasterVM t)
        //{
        //    throw new NotImplementedException();
        //}

        public IHttpActionResult Delete([FromBody] int id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Orders
        /// <summary>
        /// Manager List
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ManagerList/{companyCode}/{branchCode}")]
        [Route("ManagerList")]
        public IHttpActionResult GetManagerList(string companyCode, string branchCode)
        {
            var data = new OrderBL().GetManagerList(companyCode, branchCode);
            return Ok(data);
        }

        /// <summary>
        /// Get Order Booking Type
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("BookingType/{companyCode}/{branchCode}")]
        [Route("BookingType")]
        public IHttpActionResult GetBookingType(string companyCode, string branchCode)
        {
            var data = new OrderBL().GetBookingType(companyCode, branchCode);
            return Ok(data);
        }

        /// <summary>
        /// Get Order Item Type
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("OrderItemType/{companyCode}/{branchCode}")]
        [Route("OrderItemType")]
        public IHttpActionResult GetOrderItemType(string companyCode, string branchCode)
        {
            var data = new OrderBL().GetOrderItemType(companyCode, branchCode);
            return Ok(data);
        }

        /// <summary>
        /// Get all Billed Branches.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("BilledBranch/{companyCode}/{branchCode}")]
        [Route("BilledBranch")]
        public IHttpActionResult GetBilledBranches(string companyCode, string branchCode)
        {
            List<BilledBranchVM> lstOfBB = new OrderBL().GetBilledBranches(companyCode, branchCode);
            return Ok(lstOfBB);
        }

        /// <summary>
        /// Get GS Types (only for Orders).
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("OrderGSType/{companyCode}/{branchCode}")]
        [Route("OrderGSType")]
        public IHttpActionResult GetOrderGSType(string companyCode, string branchCode)
        {
            List<OrderGSTypeVM> lstOrderGSType = new OrderBL().GetGSTTypes(companyCode, branchCode);
            return Ok(lstOrderGSType);
        }

        /// <summary>
        /// Get Payment mode Types.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("PayMode/{companyCode}/{branchCode}")]
        [Route("PayMode")]
        public IHttpActionResult GetPayMode(string companyCode, string branchCode)
        {
            List<OrderPayModeVM> lstOrderGSType = new OrderBL().GetPaymentModes(companyCode, branchCode);
            return Ok(lstOrderGSType);
        }

        /// <summary>
        /// Get All Bank Details.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Bank/{companyCode}/{branchCode}")]
        [Route("Bank")]
        public IHttpActionResult GetBank(string companyCode, string branchCode)
        {
            List<OrderBankVM> bank = new OrderBL().GetBank(companyCode, branchCode);
            return Ok(bank);
        }

        /// <summary>
        /// Get Bank Name by Bank Code
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("BankCode/{companyCode}/{branchCode}/{bankCode}")]
        [Route("BankCode")]
        public IHttpActionResult GetBankCode(string companyCode, string branchCode, string bankCode)
        {
            OrderBankVM bank = new OrderBL().GetBankCode(companyCode, branchCode, bankCode);
            return Ok(bank);
        }

        /// <summary>
        /// Get rate based on GS Code and Karat.
        /// </summary>
        /// <param name="gsCode"></param>
        /// <param name="karat"></param>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Rate/{companyCode}/{branchCode}/{bankCode}")]
        [Route("Rate")]
        public IHttpActionResult GetRate(string companyCode, string branchCode, string gsCode, string karat)
        {
            decimal rate = new OrderBL().GetRate(companyCode, branchCode, gsCode, karat);
            return Ok(new { rate = rate });
        }

        /// <summary>
        /// Get Reserved order barcode information.
        /// </summary>
        /// <param name="barcodeNo"></param>        
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ReservedOrder/{companyCode}/{branchCode}/{barcodeNo}")]
        [Route("ReservedOrder")]
        public IHttpActionResult GetReservedOrderBarocdeInfo(string companyCode, string branchCode, string barcodeNo)
        {
            ErrorVM error = new ErrorVM();
            OrderItemDetailsVM om = new OrderBL().GetReservedOrderBarocdeInfo(companyCode, branchCode, barcodeNo, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(om);
        }

        /// <summary>
        /// Get couters.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Counter/{companyCode}/{branchCode}")]
        [Route("Counter")]
        public IHttpActionResult GetCounter(string companyCode, string branchCode)
        {
            List<OrderCounterVM> lstOrderCounter = new OrderBL().GetCounter(companyCode, branchCode);
            return Ok(lstOrderCounter);
        }

        /// <summary>
        /// Get items by Counter code and GS code.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="counterCode"></param>
        /// <param name="gsCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Items/{companyCode}/{branchCode}/{counterCode}/{gsCode}")]
        [Route("Items")]
        public IHttpActionResult GetItem(string companyCode, string branchCode, string counterCode, string gsCode)
        {
            List<OrderItemVM> lstOfOrderItem = new OrderBL().GetItems(companyCode, branchCode, counterCode, gsCode);
            return Ok(lstOfOrderItem);
        }

        /// <summary>
        /// Get List of Banks Having Cheque Details
        /// Value Field: acc_code
        /// Text Field: acc_name
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ChequeBank/{companyCode}/{branchCode}")]
        [Route("ChequeBank")]
        public IHttpActionResult GetChequeBankDetails(string companyCode, string branchCode)
        {
            var accLedger = new OrderBL().GetChequeBankDetails(companyCode, branchCode);
            return Ok(accLedger);
        }

        /// <summary>
        /// To get the cheque details by sending Bank Name.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="accCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ChequeList/{companyCode}/{branchCode}/{accCode}")]
        [Route("ChequeList")]
        public IHttpActionResult GetChequeListByBank(string companyCode, string branchCode, int accCode)
        {
            List<KSTU_ACC_CHEQUE_MASTER> lstOfCheque = new OrderBL().GetChequeListByBank(companyCode, branchCode, accCode);
            return Ok(lstOfCheque);
        }

        /// <summary>
        /// Get Attached order information by Estimation No.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAttachedOrder/{companyCode}/{branchCode}/{EstNo}")]
        [Route("GetAttachedOrder")]
        public IHttpActionResult GetSearchedOrder(string companyCode, string branchCode, int estNo)
        {
            List<AllOrdersVM> lstAllOrders = new OrderBL().GetSearchedOrder(companyCode, branchCode, estNo);
            return Ok(lstAllOrders);
        }

        /// <summary>
        /// Gets the order Purchase Plans
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="orderRateType">Order rate type. Can be one of the following: Delivery, Flexi, Fixed</param>
        /// <returns></returns>
        [HttpGet]
        [Route("PurchasePlan/{companyCode}/{branchCode}/{orderRateType}")]
        [Route("PurchasePlan")]
        [ResponseType(typeof(List<OrderPurchasePlanTypeVM>))]
        public IHttpActionResult GetPurchasePlan(string companyCode, string branchCode, string orderRateType)
        {
            ErrorVM error = null;
            List<OrderPurchasePlanTypeVM> bookingTypeList = new OrderBL().GetPurchasePlan(companyCode, branchCode, orderRateType, out error);
            if (error == null)
                return Ok(bookingTypeList);
            else
                return Content(error.ErrorStatusCode, error);
        }

        /// <summary>
        /// Gets order purchase plan detail
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="purchasePlanCode">Purchase plan codes return by PurchasePlan API</param>
        /// <returns></returns>
        [HttpGet]
        [Route("PurchasePlanDetail/{companyCode}/{branchCode}/{purchasePlanCode}")]
        [Route("PurchasePlanDetail")]
        [ResponseType(typeof(List<OrderPurchasePlanDetailVM>))]
        public IHttpActionResult GetPurchasePlanDetail(string companyCode, string branchCode, string purchasePlanCode)
        {
            ErrorVM error = null;
            List<OrderPurchasePlanDetailVM> ppDetail = new OrderBL().GetPurchasePlanDetail(companyCode, branchCode, purchasePlanCode, out error);
            if (error == null)
                return Ok(ppDetail);
            else
                return Content(error.ErrorStatusCode, error);
        }

        /// <summary>
        /// Get Order information by order No.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Get/{companyCode}/{branchCode}/{orderNo}")]
        [Route("Get")]
        public IHttpActionResult GetOrderInfo(string companyCode, string branchCode, int orderNo)
        {
            ErrorVM error = new ErrorVM();
            OrderMasterVM om = new OrderBL().GetOrderInfo(companyCode, branchCode, orderNo, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(om);
        }

        /// <summary>
        /// Get Order information by order No (For viewing without any Validations)
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetViewOrder/{companyCode}/{branchCode}/{orderNo}")]
        [Route("GetViewOrder")]
        public IHttpActionResult GetViewOrderInfo(string companyCode, string branchCode, int orderNo)
        {
            ErrorVM error = new ErrorVM();
            OrderMasterVM om = new OrderBL().GetViewOrderInfo(companyCode, branchCode, orderNo, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(om);
        }

        /// <summary>
        /// Get Order information by order No (For viewing without any Validations)
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetViewOrderWithClosedInfo/{companyCode}/{branchCode}/{orderNo}")]
        [Route("GetViewOrderWithClosedInfo")]
        public IHttpActionResult GetViewOrderInfoForClosedOrder(string companyCode, string branchCode, int orderNo)
        {
            List<PaymentVM> lstOfPayment = new OrderBL().GetViewOrderInfoForClosedOrder(companyCode, branchCode, orderNo);
            return Ok(lstOfPayment);
        }

        /// <summary>
        /// Get Order Information with Cancel Orer validation.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetCancelOrderView/{companyCode}/{branchCode}/{orderNo}")]
        [Route("GetCancelOrderView")]
        public IHttpActionResult GetCancelOrderView(string companyCode, string branchCode, int orderNo)
        {
            ErrorVM error = new ErrorVM();
            OrderMasterVM om = new OrderBL().GetCancelOrderView(companyCode, branchCode, orderNo, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(om);
        }

        /// <summary>
        /// Save order information
        /// </summary>
        ///  <param name="order"></param>
        /// <returns>Order No and  Receipt No</returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult SaveOrderInfo([FromBody] OrderMasterVM order) //string companyCode, string branchCode, 
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            //order.CompanyCode = companyCode;
            //order.BranchCode = branchCode;
            int orderNo, receiptNo;
            ErrorVM error = new ErrorVM();
            OrderMasterVM savedOrder = new OrderBL().SaveOrderInfoWithTran(order, out orderNo, out receiptNo, out error);
            if (savedOrder != null) {
                return Ok(new { OrderNo = orderNo, ReceiptNo = receiptNo });
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Get Receipt details by Receipt No.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="receiptNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ReceiptDet/{companyCode}/{branchCode}/{receiptNo}")]
        [Route("ReceiptDet")]
        public IHttpActionResult GetReceiptDetails(string companyCode, string branchCode, int receiptNo)
        {
            ErrorVM error = new ErrorVM();
            List<PaymentVM> lstOfPayment = new OrderBL().GetReceiptDetails(companyCode, branchCode, receiptNo, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(lstOfPayment);
        }

        /// <summary>
        /// Update Order information by order No with Order details.
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put")]
        public IHttpActionResult UpdateOrderInfo(int orderNo, [FromBody] OrderMasterVM order)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            ErrorVM error = new ErrorVM();
            bool saved = new OrderBL().UpdateOrderInfo(orderNo, order, out error);
            if (saved) {
                return Ok(new { OrderNo = orderNo, ReceiptNo = order.lstOfPayment.FirstOrDefault().ReceiptNo });
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Get Order receipt details by Order No.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("OrderReceiptGet/{companyCode}/{branchCode}/{orderNo}")]
        [Route("OrderReceiptGet")]
        public IHttpActionResult GetOrderReceiptGetDetails(string companyCode, string branchCode, int orderNo)
        {
            ErrorVM error = new ErrorVM();
            OrderMasterVM om = new OrderBL().GetOrderReceiptGetDetails(companyCode, branchCode, orderNo, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(om);
        }

        /// <summary>
        /// Save order receipt Details.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("OrderReceipt")]
        public IHttpActionResult SaveOrderReceiptDetails(OrderMasterVM order)
        {
            //if (!ModelState.IsValid) {
            //    return BadRequest(ModelState);
            //}
            ErrorVM error = new ErrorVM();
            int receiptNo = new OrderBL().SaveOrderReceiptDetails(order, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(new { ReceiptNo = receiptNo });
        }

        /// <summary>
        /// Cancel order by Order No.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CancelOrder")]
        public IHttpActionResult CancelOrder(OrderMasterVM order)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            bool receiptNo = new OrderBL().CancelOrder(order, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok();
        }

        /// <summary>
        /// Close order by Order details.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CloseOrder")]
        public IHttpActionResult CloseOrder(OrderMasterVM order)
        {
            //if (!ModelState.IsValid) {
            //    return BadRequest(ModelState);
            //}
            ErrorVM error = new ErrorVM();
            bool receiptNo = new OrderBL().CloseOrder(order, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok();
        }

        /// <summary>
        /// Get receipt Details for Cancel Receipt
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="receiptNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("CancelReceiptGet/{companyCode}/{branchCode}/{receiptNo}")]
        [Route("CancelReceiptGet")]
        public IHttpActionResult CancelReceiptGet(string companyCode, string branchCode, int receiptNo)
        {
            ErrorVM error = new ErrorVM();
            PaymentVM payment = new OrderBL().CancelReceiptGet(companyCode, branchCode, receiptNo, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(payment);
        }

        /// <summary>
        /// Cance receipt by order details.
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CancelReceipt")]
        public IHttpActionResult OrderCancelReceipt(PaymentVM payment)
        {
            //if (!ModelState.IsValid) {
            //    return BadRequest(ModelState);
            //}
            ErrorVM error = new ErrorVM();
            bool saved = new OrderBL().OrderCancelReceipt(payment, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok();
        }

        /// <summary>
        /// Closed order in Other Branch.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CloseOrderInOtherBranch")]
        public IHttpActionResult ClosedOrderInOtherBranch(OrderMasterVM order)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            bool saved = new OrderBL().ClosedOrderInOtherBranch(order, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok();
        }

        [HttpPut]
        [Route("UploadOrderImage2/{companyCode}/{branchCode}/{orderNo}/{slno}")]
        public async Task<IHttpActionResult> UploadOrderImage2(string companyCode, string branchCode, int orderNo, int slno)
        {
            OrderItemDetailsVM order = new OrderItemDetailsVM();
            order.CompanyCode = companyCode;
            order.BranchCode = branchCode;
            order.OrderNo = orderNo;
            order.SlNo = slno;
            string destFolder = ConfigurationManager.AppSettings["OrdImgFolder"].ToString();
            try {
                string filePath = "";
                string name = string.Empty;
                string newFileName = string.Empty;
                List<string> savedFilePath = new List<string>();
                // Check if the request contains multipart/form-data
                if (!Request.Content.IsMimeMultipartContent()) {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }
                //Get the path of folder where we want to upload all files.
                string rootPath = HttpContext.Current.Server.MapPath("~/" + destFolder);
                var provider = new MultipartFileStreamProvider(rootPath);
                await Request.Content.ReadAsMultipartAsync(provider);
                // Read the form data.
                foreach (MultipartFileData dataitem in provider.FileData) {
                    try {
                        //Replace / from file name
                        name = dataitem.Headers.ContentDisposition.FileName.Replace("\"", "");
                        //Create New file name using Branch,OrderNo and SlNo like magna
                        newFileName = order.BranchCode + "-" + order.OrderNo + "_" + order.SlNo + Path.GetExtension(name);
                        //Move file from current location to target folder.
                        File.Move(dataitem.LocalFileName, Path.Combine(rootPath, newFileName));
                        //File path returning to the URL
                        filePath = HttpContext.Current.Request.Url.OriginalString.Replace(HttpContext.Current.Request.Url.LocalPath, "") + HttpContext.Current.Request.ApplicationPath + "/" + destFolder + "/" + newFileName;

                        //Updating to corresponding Order
                        ErrorVM error = new ErrorVM();
                        bool saved = new OrderBL().UpdatOrderImageURL(order.CompanyCode, order.BranchCode, order.OrderNo, order.SlNo, "\\" + newFileName, out error);
                        if (error != null) {
                            return Content(error.ErrorStatusCode, error);
                        }
                    }
                    catch (Exception ex) {
                        string message = ex.Message;
                        return Content(HttpStatusCode.InternalServerError, ex.Message);
                    }
                }
                return Ok(new { Path = filePath });
            }
            catch (Exception excp) {
                return Content(HttpStatusCode.InternalServerError, excp.Message);
            }
        }

        [AllowAnonymous]
        [HttpPut]
        [Route("UploadOrderImage/{companyCode}/{branchCode}/{salesmanCode}/{managerCode}")]
        public async Task<IHttpActionResult> UploadOrderImage(string companyCode, string branchCode, string salesmanCode, string managerCode)
        {
            OrderItemDetailsVM order = new OrderItemDetailsVM();
            order.CompanyCode = companyCode;
            order.BranchCode = branchCode;
            string destFolder = ConfigurationManager.AppSettings["OrdImgFolder"].ToString();
            try {
                string filePath = "";
                string name = string.Empty;
                string newFileName = string.Empty;
                List<string> savedFilePath = new List<string>();
                // Check if the request contains multipart/form-data
                if (!Request.Content.IsMimeMultipartContent()) {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }
                //Get the path of folder where we want to upload all files.
                string rootPath = HttpContext.Current.Server.MapPath("~/" + destFolder);
                var provider = new MultipartFileStreamProvider(rootPath);
                await Request.Content.ReadAsMultipartAsync(provider);
                // Read the form data.
                foreach (MultipartFileData dataitem in provider.FileData) {
                    try {
                        //Replace / from file name
                        name = dataitem.Headers.ContentDisposition.FileName.Replace("\"", "");
                        //Create New file name using Branch,OrderNo and SlNo like magna
                        newFileName = order.BranchCode + "_" + salesmanCode + "_" + managerCode + "_" + SIGlobals.Globals.GetNewGUID() + Path.GetExtension(name);
                        //Move file from current location to target folder.
                        File.Move(dataitem.LocalFileName, Path.Combine(rootPath, newFileName));
                        //File path returning to the URL
                        filePath = HttpContext.Current.Request.Url.OriginalString.Replace(HttpContext.Current.Request.Url.LocalPath, "") + HttpContext.Current.Request.ApplicationPath + "/" + destFolder + "/" + newFileName;
                    }
                    catch (Exception ex) {
                        string message = ex.Message;
                        return Content(HttpStatusCode.InternalServerError, ex.Message);
                    }
                }
                return Ok(new { Path = filePath });
            }
            catch (Exception excp) {
                return Content(HttpStatusCode.InternalServerError, excp.Message);
            }
        }

        /// <summary>
        /// Download the order image by Sending Order Number
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="orderNo"></param>
        /// <param name="slNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("DownloadOrderImage/{companyCode}/{branchCode}/{orderNo}/{slNo}")]
        public HttpResponseMessage DownloadOrderImage(string companyCode, string branchCode, int orderNo, int slNo)
        {
            string destFolder = ConfigurationManager.AppSettings["OrdImgFolder"].ToString();
            HttpResponseMessage result = null;
            string imgName = new OrderBL().SendImagePath(companyCode, branchCode, orderNo, slNo);
            var localFilePath = HttpContext.Current.Server.MapPath("~/" + destFolder + imgName);
            if (!File.Exists(localFilePath)) {
                result = Request.CreateResponse(HttpStatusCode.NotFound);
            }
            else {
                // Serve the file to the client
                result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new StreamContent(new FileStream(localFilePath, FileMode.Open, FileAccess.Read));
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = "OrderImage" + companyCode + branchCode + orderNo;
            }
            return result;
        }

        /// <summary>
        /// This method is used to validate the Estimation Metal and Order Metal both are same or not.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ValAttachOrderVal/{companyCode}/{branchCode}/{estNo}/{orderNo}")]
        public IHttpActionResult ValidateAttachOrderWithGSandKaratWithEstNo(string companyCode, string branchCode, int estNo, int orderNo)
        {
            ErrorVM error = new ErrorVM();
            List<SalesEstDetailsVM> lstSalesDet = new List<SalesEstDetailsVM>();
            bool saved = new OrderBL().GetValidateAttachOrderWithGSandKaratWithEstNo(companyCode, branchCode, estNo, orderNo, out error, out lstSalesDet);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(lstSalesDet);
        }

        /// <summary>
        /// Get Receipt Details By Order No.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ReceptDetByOrder/{companyCode}/{branchCode}/{orderNo}")]
        public IHttpActionResult GetReceiptDetailsByOrderNo(string companyCode, string branchCode, int orderNo)
        {
            ErrorVM error = new ErrorVM();
            var data = new OrderBL().GetReceiptDetailsByOrderNo(companyCode, branchCode, orderNo, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(data);
        }

        #endregion

        #region Order Search

        /// <summary>
        /// Get Orders Search Parameters.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("OrderSearchParams")]
        public IHttpActionResult GetOrderDetailsSearchParams(string companyCode, string branchCode)
        {
            List<SearchParamVM> lstSearchParams = new OrderBL().GetOrderDetailsSearchParams(companyCode, branchCode);
            return Ok(lstSearchParams);
        }

        /// <summary>
        /// Get Searched result of orders by search Param
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="searchBy"></param>
        /// <param name="searchParam"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("OrderSearch/{companyCode}/{branchCode}/{searchBy}/{searchParam}")]
        [Route("OrderSearch")]
        public IHttpActionResult GetOrderDetailsBySearchParameters(string companyCode, string branchCode, string searchBy, string searchParam)
        {
            List<KTTU_ORDER_MASTER> lstOrderMaster = new OrderBL().GetOrderDetailsBySearchParameters(companyCode, branchCode, searchBy, searchParam);
            return Ok(lstOrderMaster);
        }
        #endregion

        #region Order Attachment
        /// <summary>
        /// Get all order details.        
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("AllOrders/{companyCode}/{branchCode}")]
        [Route("AllOrders")]
        //Sample usage of OData Query:
        //api/order/AllOrders/BH/JNR?$top=5&$skip=0&$orderby=OrderNo desc&$filter=FixedWt eq 20 and Customer eq 'BHIMA'
        public IQueryable<AllOrdersVM> GetAllOrder(string companyCode, string branchCode)
        {
            return new OrderBL().GetAllOrder(companyCode, branchCode).AsQueryable().OrderByDescending(o => o.OrderNo);
        }

        /// <summary>
        /// Get All orders Count For Order Attachment.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("AllOrdersCount/{companyCode}/{branchCode}")]
        [Route("AllOrdersCount")]
        public IHttpActionResult GetAllOrdersCount(string companyCode, string branchCode)
        {
            return Ok(new { RecordCount = GetAllOrder(companyCode, branchCode).Count() });
        }

        /// <summary>
        /// Search parameters for Orders.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SearchParams/{companyCode}/{branchCode}")]
        public IHttpActionResult GetAllSearchParameters(string companyCode, string branchCode)
        {
            List<SearchParamVM> lstSearchParams = new List<SearchParamVM>();
            lstSearchParams.Add(new SearchParamVM() { Value = "ORDERNO", Key = "Order No" });
            lstSearchParams.Add(new SearchParamVM() { Value = "Customer", Key = "Customer" });
            lstSearchParams.Add(new SearchParamVM() { Value = "DATE", Key = "Date" });
            lstSearchParams.Add(new SearchParamVM() { Value = "RATE", Key = "Rate" });
            lstSearchParams.Add(new SearchParamVM() { Value = "TYPE", Key = "Type" });
            lstSearchParams.Add(new SearchParamVM() { Value = "ID", Key = "ID" });
            lstSearchParams.Add(new SearchParamVM() { Value = "STAFF", Key = "Staff" });
            lstSearchParams.Add(new SearchParamVM() { Value = "KARAT", Key = "Karat" });
            return Ok(lstSearchParams);
        }

        /// <summary>
        /// Get searched results by searching orders.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="searchType"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Search/{companyCode}/{branchCode}")]
        [Route("Search")]
        public IQueryable<AllOrdersVM> GetSearchOrder(string companyCode, string branchCode, string searchType, string searchValue)
        {
            IQueryable<AllOrdersVM> lstAllOrderVM = new OrderBL().GetSearchOrder(companyCode, branchCode, searchType, searchValue);
            return lstAllOrderVM;
        }

        /// <summary>
        /// Get Attached order details by Sales Estimation Number.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAttachment/{companyCode}/{branchCode}/{estNo}")]
        [Route("GetAttachment")]
        public IHttpActionResult GetAttachedOrder(string companyCode, string branchCode, int estNo)
        {
            List<OrderMasterVM> lstOfOrderMaster = new OrderBL().GetAttachedOrder(companyCode, branchCode, estNo);
            return Ok(lstOfOrderMaster);
        }

        /// <summary>
        /// Get Global Order Attached Amount
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GlobalOrderAmount/{companyCode}/{branchCode}/{estNo}")]
        public IHttpActionResult GetGlobalAttachedOrderAmount(string companyCode, string branchCode, int estNo)
        {
            decimal globalAmount = new OrderBL().GetGlobalAttachedOrderAmount(companyCode, branchCode, estNo);
            return Ok(globalAmount);
        }

        /// <summary>
        /// Save or Attach Order details.
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostAttachment")]
        public IHttpActionResult SaveAttachedOrder(List<PaymentVM> payment)
        {
            ErrorVM error = new ErrorVM();
            List<SalesEstDetailsVM> lstSalesDet = new List<SalesEstDetailsVM>();
            bool saved = new OrderBL().SaveAttachedOrder(payment, out error, out lstSalesDet);
            if (saved) {
                return Ok(lstSalesDet);
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Remove Attachments to Estimation Number
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("RemoveAttachment/{companyCode}/{branchCode}/{estNo}")]
        [Route("RemoveAttachment")]
        public IHttpActionResult RemoveOrderAttachement(string companyCode, string branchCode, int estNo)
        {
            ErrorVM error = new ErrorVM();
            bool removed = new OrderBL().RemoveOrderAttachement(companyCode, branchCode, estNo, out error);
            if (removed) {
                return Ok();
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }
        #endregion

        #region Print Orders

        #region Methods For UI Printing.
        /// <summary>
        /// Get order detals for Printing.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetOrderPrint/{companyCode}/{branchCode}/{orderNo}")]
        [Route("GetOrderPrint")]
        public IHttpActionResult GetOrderInfoForPrint(string companyCode, string branchCode, int orderNo)
        {
            ErrorVM error = new ErrorVM();
            OrderMasterVM orderMaster = new OrderBL().GetOrderInfoForPrint(companyCode, branchCode, orderNo, out error);
            if (error == null) {
                return Ok(orderMaster);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get Total for Print order
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetOrderReceiptTotalPrint/{companyCode}/{branchCode}/{orderNo}")]
        [Route("GetOrderReceiptTotalPrint")]
        public IHttpActionResult GetOrderReceiptTotalPrint(string companyCode, string branchCode, int orderNo)
        {
            ErrorVM error = new ErrorVM();
            KTTU_PAYMENT_DETAILS payment = new OrderBL().GetOrderReceiptTotalPrint(companyCode, branchCode, orderNo, out error);
            if (error == null) {
                return Ok(payment);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get Order Formt Total (Quantity and Weight) Print
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetOrderFormTotalPrint/{companyCode}/{branchCode}/{orderNo}")]
        [Route("GetOrderFormTotalPrint")]
        public IHttpActionResult GetOrderFormTotalPrint(string companyCode, string branchCode, int orderNo)
        {
            ErrorVM error = new ErrorVM();
            OrderItemDetailsVM orderDetails = new OrderBL().GetOrderFormTotalPrint(companyCode, branchCode, orderNo, out error);
            if (error == null) {
                return Ok(orderDetails);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get Total for closed order Print
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetClosedOrderReceiptTotalPrint/{companyCode}/{branchCode}/{orderNo}")]
        [Route("GetClosedOrderReceiptTotalPrint")]
        public IHttpActionResult GetClosedOrderReceiptTotalPrint(string companyCode, string branchCode, int orderNo)
        {
            ErrorVM error = new ErrorVM();
            KTTU_PAYMENT_DETAILS payment = new OrderBL().GetClosedOrderReceiptTotalPrint(companyCode, branchCode, orderNo, out error);
            if (error == null) {
                return Ok(payment);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get Receipt details by Receipt No for printing.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="receiptNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ReceiptDetPrint/{companyCode}/{branchCode}/{receiptNo}")]
        [Route("ReceiptDetPrint")]
        public IHttpActionResult GetReceiptDetailsPrint(string companyCode, string branchCode, int receiptNo)
        {
            ErrorVM error = new ErrorVM();
            List<PaymentVM> payment = new OrderBL().GetReceiptDetailsPrint(companyCode, branchCode, receiptNo, out error);
            if (error == null) {
                return Ok(payment);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get Receipt Details Totals for Printing.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="receiptNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetReceiptDetTotalPrint/{companyCode}/{branchCode}/{receiptNo}")]
        [Route("GetReceiptDetTotalPrint")]
        public IHttpActionResult GetReceiptDetTotalPrint(string companyCode, string branchCode, int receiptNo)
        {
            ErrorVM error = new ErrorVM();
            KTTU_PAYMENT_DETAILS payment = new OrderBL().GetReceiptDetTotalPrint(companyCode, branchCode, receiptNo, out error);
            if (error == null) {
                return Ok(payment);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }



        /// <summary>
        /// Get Order details for Reprint orders for the selected date
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="date"></param>
        /// <param name="isCancelled"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("OrderPrintOrders/{companyCode}/{branchCode}/{date}/{isCancelled}")]
        [Route("OrderPrintOrders")]
        public IHttpActionResult GetOrderDetails(string companyCode, string branchCode, DateTime date, bool isCancelled)
        {
            List<KTTU_ORDER_MASTER> lstOfOrders = new OrderBL().GetOrderDetails(companyCode, branchCode, date, isCancelled);
            return Ok(lstOfOrders.Select(ord => new OrderMasterVM()
            {
                OrderNo = ord.order_no,
                CustName = ord.cust_name
            }));
        }

        /// <summary>
        /// Get Receipt details by Date.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="date"></param>
        /// <param name="isCancelled"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("OrderPrintReceiptDetails/{companyCode}/{branchCode}/{date}/{isCancelled}")]
        [Route("OrderPrintReceiptDetails")]
        public IHttpActionResult GetReceiptDetailsForPrint(string companyCode, string branchCode, DateTime date, bool isCancelled)
        {
            dynamic result = new OrderBL().GetReceiptDetailsForPrint(companyCode, branchCode, date, isCancelled);
            return Ok(result);
        }

        /// <summary>
        /// Get Order details for Reprint orders for the selected date
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("OrderPrintClosedOrders/{companyCode}/{branchCode}/{date}")]
        [Route("OrderPrintClosedOrders")]
        public IHttpActionResult GetClosedOrderDetails(string companyCode, string branchCode, DateTime date)
        {
            List<KTTU_ORDER_MASTER> lstOfOrders = new OrderBL().GetClosedOrderDetails(companyCode, branchCode, date);
            return Ok(lstOfOrders.Select(ord => new OrderMasterVM()
            {
                OrderNo = ord.order_no,
                CustName = ord.cust_name
            }));
        }

        #endregion

        #region Dotmatrix and HTML
        /// <summary>
        /// To Get HTML Printing for Order
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="orderNo"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("OrderPrint/{companyCode}/{branchCode}/{orderNo}/{orderType}")]
        [Route("OrderPrint")]
        public IHttpActionResult OrderPrint(string companyCode, string branchCode, int orderNo, string orderType)
        {
            ErrorVM error = new ErrorVM();
            BL.ViewModel.Print.ProdigyPrintVM print = new OrderBL().GetOrderPrint(companyCode, branchCode, orderNo, orderType, out error);
            if (error == null) {
                return Ok(print);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get Receipt details by Receipt No for HTML printing.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="receiptNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("OrderReceiptPrint/{companyCode}/{branchCode}/{receiptNo}")]
        [Route("OrderReceiptPrint")]
        public IHttpActionResult GetReceiptDetailsPrintDotMatrix(string companyCode, string branchCode, int receiptNo)
        {
            ErrorVM error = new ErrorVM();
            BL.ViewModel.Print.ProdigyPrintVM print = new OrderBL().GetOrderReceiptPrint(companyCode, branchCode, receiptNo, out error);
            if (error == null) {
                return Ok(print);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Close Order Dotmatrix Print.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ClosedOrderPrint/{companyCode}/{branchCode}/{orderNo}")]
        [Route("ClosedOrderPrint")]
        public IHttpActionResult ClosedOrderPrint(string companyCode, string branchCode, int orderNo)
        {
            ErrorVM error = new ErrorVM();
            BL.ViewModel.Print.ProdigyPrintVM print = new OrderBL().GetClosedOrderPrint(companyCode, branchCode, orderNo, out error);
            if (error == null) {
                return Ok(print);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        #endregion

        #endregion

        #region Chit Adjustment
        /// <summary>
        /// Get All branches for Chit closur
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("chit/branch/{currentBranch}")]
        public IHttpActionResult GetBranch(string currentBranch)
        {
            List<ChitClosureVM> lstOfCC = new OrderBL().GetBranch(currentBranch);
            return Ok(lstOfCC);
        }

        /// <summary>
        /// Get Scheme code details by Branch
        /// </summary>
        /// <param name="branchCode"></param>
        /// <param name="currentBranch"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("chit/schemeCode/{branchCode}/{currentBranch}")]
        public IHttpActionResult GetSchemeCode(string branchCode, string currentBranch)
        {
            List<ChitClosureVM> lstOfCC = new OrderBL().GetSchemeCode(branchCode, currentBranch);
            return Ok(lstOfCC);
        }

        /// <summary>
        /// Get Group code for Chit
        /// </summary>
        /// <param name="branchCode"></param>
        /// <param name="schemeCode"></param>
        /// <param name="currentBranch"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("chit/groupCode/{branchCode}/{schemeCode}/{currentBranch}")]
        public IHttpActionResult GetGroupCode(string branchCode, string schemeCode, string currentBranch)
        {
            List<ChitClosureVM> lstOfCC = new OrderBL().GetGroupCode(branchCode, schemeCode, currentBranch);
            return Ok(lstOfCC);
        }

        /// <summary>
        /// Get Group code for Chit
        /// </summary>
        /// <param name="branchCode"></param>
        /// <param name="schemeCode"></param>
        /// <param name="groupCode"></param>
        /// <param name="currentBranch"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("chit/startMSNNo/{branchCode}/{schemeCode}/{groupCode}/{currentBranch}")]
        public IHttpActionResult GetStartMSNNo(string branchCode, string schemeCode, string groupCode, string currentBranch)
        {
            List<ChitClosureVM> lstOfCC = new OrderBL().GetStartMSNNo(branchCode, schemeCode, groupCode, currentBranch);
            return Ok(lstOfCC);
        }

        /// <summary>
        /// Get Chit closure Amount
        /// </summary>
        /// <param name="branchCode"></param>
        /// <param name="schemeCode"></param>
        /// <param name="groupCode"></param>
        /// <param name="startMSNNo"></param>
        /// <param name="endMSNNo"></param>
        /// <param name="currentBranch"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("chit/chitAmount/{branchCode}/{schemeCode}/{groupCode}/{startMSNNo}/{endMSNNo}/{currentBranch}")]
        public IHttpActionResult GetChitAmount(string branchCode, string schemeCode, string groupCode, int startMSNNo, int endMSNNo, string currentBranch)
        {
            ChitAdjustVM ca = new OrderBL().GetChitAmount(branchCode, schemeCode, groupCode, startMSNNo, endMSNNo, currentBranch);
            return Ok(ca);
        }

        /// <summary>
        /// Get Chit closure Details
        /// </summary>
        /// <param name="branchCode"></param>
        /// <param name="schemeCode"></param>
        /// <param name="groupCode"></param>
        /// <param name="startMSNNo"></param>
        /// <param name="endMSNNo"></param>
        /// <param name="currentBranch"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("chit/chitDetails/{branchCode}/{schemeCode}/{groupCode}/{startMSNNo}/{endMSNNo}/{currentBranch}")]
        public IHttpActionResult GetChitClosureDetails(string branchCode, string schemeCode, string groupCode, int startMSNNo, int endMSNNo, string currentBranch)
        {
            List<PaymentVM> lstOfPayment = new OrderBL().GetChitClosureDetails(branchCode, schemeCode, groupCode, startMSNNo, endMSNNo, currentBranch);
            return Ok(lstOfPayment);
        }
        #endregion

        //#region Marketplace Integration

        //#region Received Orders
        ///// <summary>
        ///// Provides All Online Orders
        ///// </summary>
        ///// <param name="order"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("OnlineOrders")]
        //public IHttpActionResult GetAllReceivedOrders([FromBody] ReceiveOrderVM order)
        //{
        //    ErrorVM error = new ErrorVM();
        //    var data = new OrderBL().GetAllReceivedOrders(order.CompanyCode, order.BranchCode, order.FromDate, order.ToDate, order.MarketplaceID, order.Type, out error);
        //    if (error.ErrorStatusCode == HttpStatusCode.OK) {
        //        return Ok(data);
        //    }
        //    else {
        //        return Content(error.ErrorStatusCode, error);
        //    }
        //}

        ///// <summary>
        ///// Save Items to Pick List
        ///// </summary>
        ///// <param name="order"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("PickList")]
        //public IHttpActionResult SaveItemsToPickList([FromBody] List<ReceivedOrderForGridVM> order)
        //{
        //    ErrorVM error = new ErrorVM();
        //    string user = base.GetUserId();
        //    int asingnmentNo = new OrderBL().SaveItemsToPickList(order, user, out error);
        //    if (error.ErrorStatusCode == HttpStatusCode.OK) {
        //        return Ok(new { AssignmentNo = asingnmentNo });
        //    }
        //    else {
        //        return Content(error.ErrorStatusCode, error);
        //    }
        //}

        ///// <summary>
        ///// Get Pick List Item for Print.
        ///// </summary>
        ///// <param name="companyCode"></param>
        ///// <param name="branchCode"></param>
        ///// <param name="assignmentNo"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("PrintPickList/{companyCode}/{branchCode}/{assignmentNo}")]
        //[Route("PrintPickList")]
        //public IHttpActionResult GetPickList(string companyCode, string branchCode, int assignmentNo)
        //{
        //    ErrorVM error = new ErrorVM();
        //    var data = new OrderBL().PrintOrderItemPickList(companyCode, branchCode, assignmentNo, out error);
        //    if (error.ErrorStatusCode == HttpStatusCode.OK) {
        //        return Ok(data);
        //    }
        //    else {
        //        return Content(error.ErrorStatusCode, error);
        //    }
        //}
        //#endregion

        //#region Item Picking

        ///// <summary>
        ///// Get Pick List No.
        ///// </summary>
        ///// <param name="companyCode"></param>
        ///// <param name="branchCode"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("PickListNo/{companyCode}/{branchCode}")]
        //[Route("PickListNo")]
        //public IHttpActionResult GetPickList(string companyCode, string branchCode)
        //{
        //    ErrorVM error = new ErrorVM();
        //    var data = new OrderBL().GetPickListNo(companyCode, branchCode, out error);
        //    if (error.ErrorStatusCode == HttpStatusCode.OK) {
        //        return Ok(data);
        //    }
        //    else {
        //        return Content(error.ErrorStatusCode, error);
        //    }
        //}
        ///// <summary>
        ///// Get Picked List Details by passing assignment Number.
        ///// </summary>
        ///// <param name="companyCode"></param>
        ///// <param name="branchCode"></param>
        ///// <param name="assignmentNo"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("PickListDet/{companyCode}/{branchCode}/{assignmentNo}")]
        //[Route("PickListDet")]
        //public IHttpActionResult GetPickListDetails(string companyCode, string branchCode, int assignmentNo)
        //{
        //    ErrorVM error = new ErrorVM();
        //    var data = new OrderBL().GetPickListDetByAssignmentNo(companyCode, branchCode, assignmentNo, out error);
        //    if (error.ErrorStatusCode == HttpStatusCode.OK) {
        //        return Ok(data);
        //    }
        //    else {
        //        return Content(error.ErrorStatusCode, error);
        //    }
        //}

        ///// <summary>
        ///// Update pick-list details by barcode no
        ///// </summary>
        ///// <param name="pickedItems"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("UpdatePickListDet")]
        //public IHttpActionResult UpdatePickListDetails(List<OrderItemPickListVM> pickedItems)
        //{
        //    ErrorVM error = new ErrorVM();
        //    var data = new OrderBL().UpdatePickedItem(pickedItems, out error);
        //    if (error.ErrorStatusCode == HttpStatusCode.OK) {
        //        return Ok(data);
        //    }
        //    else {
        //        return Content(error.ErrorStatusCode, error);
        //    }
        //}

        ///// <summary>
        ///// Maps a barcode number with a picklist line after validation
        ///// </summary>
        ///// <param name="companyCode"></param>
        ///// <param name="branchCode"></param>
        ///// <param name="assignmentNo"></param>
        ///// <param name="barcodeNo"></param>
        ///// <returns></returns>
        //[HttpPut]
        //[Route("assign-barcode/{companyCode}/{branchCode}/{assignmentNo}/{barcodeNo}")]
        //[Route("assign-barcode")]
        //public IHttpActionResult AssignBarcode(string companyCode, string branchCode, int assignmentNo, string barcodeNo)
        //{
        //    var userId = base.GetUserId();
        //    ErrorVM error = new ErrorVM();
        //    List<OrderItemPickListVM> orderPicklist = new List<OrderItemPickListVM>();
        //    var success = new MarketplaceBL().AssignBarcodeToSKU(companyCode, branchCode, assignmentNo, barcodeNo, userId, out orderPicklist, out error);
        //    if (success) {
        //        return Ok(orderPicklist);
        //    }
        //    else {
        //        return Content(error.ErrorStatusCode, error);
        //    }
        //}


        //#endregion

        //#region Item Packing
        ///// <summary>
        ///// Get Items for Packing
        ///// </summary>
        ///// <param name="companyCode"></param>
        ///// <param name="branchCode"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("ItemPacking/{companyCode}/{branchCode}")]
        //[Route("ItemPacking")]
        //public IHttpActionResult GetItemPakingDetails(string companyCode, string branchCode)
        //{
        //    ErrorVM error = new ErrorVM();
        //    var data = new OrderBL().GetItemDetailsForPacking(companyCode, branchCode, out error);
        //    if (error.ErrorStatusCode == HttpStatusCode.OK) {
        //        return Ok(data);
        //    }
        //    else {
        //        return Content(error.ErrorStatusCode, error);
        //    }
        //}

        ///// <summary>
        ///// Get Package Details.
        ///// </summary>
        ///// <param name="companyCode"></param>
        ///// <param name="branchCode"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("Package/{companyCode}/{branchCode}")]
        //[Route("Package")]
        //public IHttpActionResult GetPckageDetails(string companyCode, string branchCode)
        //{
        //    ErrorVM error = new ErrorVM();
        //    var data = new OrderBL().GetPackageDetails(companyCode, branchCode, out error);
        //    if (error.ErrorStatusCode == HttpStatusCode.OK) {
        //        return Ok(data);
        //    }
        //    else {
        //        return Content(error.ErrorStatusCode, error);
        //    }
        //}

        ///// <summary>
        ///// Get Packing Details by sending Package Code.
        ///// </summary>
        ///// <param name="companyCode"></param>
        ///// <param name="branchCode"></param>
        ///// <param name="packageCode"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("PackingDet/{companyCode}/{branchCode}/{packageCode}")]
        //[Route("PackingDet")]
        //public IHttpActionResult GetPackingDetails(string companyCode, string branchCode, string packageCode)
        //{
        //    ErrorVM error = new ErrorVM();
        //    var data = new OrderBL().GetPackingDetails(companyCode, branchCode, packageCode, out error);
        //    if (data != null) {
        //        return Ok(data);
        //    }
        //    else {
        //        return Content(error.ErrorStatusCode, error);
        //    }
        //}

        ///// <summary>
        ///// Create Package
        ///// </summary>
        ///// <param name="packing"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("CreatePackage")]
        //public IHttpActionResult CreatePackage(List<PackingItemVM> packing)
        //{
        //    ErrorVM error = new ErrorVM();
        //    var data = new OrderBL().CreatePackage(packing, out error);
        //    if (error.ErrorStatusCode == HttpStatusCode.OK) {
        //        return Ok(data);
        //    }
        //    else {
        //        return Content(error.ErrorStatusCode, error);
        //    }
        //}
        
        ///// <summary>
        ///// Generates invoice
        ///// </summary>
        ///// <param name="companyCode">Company Code</param>
        ///// <param name="branchCode">Branch Code</param>
        ///// <param name="orderToBeInvoiced">Order line to be invoiced</param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("create-invoice")]
        //[ResponseType(typeof(InvoiceOutputVM))]
        //public IHttpActionResult GenerateInvoice(string companyCode, string branchCode, OrdersToBeInvoiced orderToBeInvoiced)
        //{
        //    ErrorVM error = new ErrorVM();
        //    InvoiceOutputVM invoiceOutputVM = null;
        //    var success = new MarketplaceBL().GenerateInvoice( companyCode, branchCode, orderToBeInvoiced, out  invoiceOutputVM, out error);
        //    if (success) {
        //        return Ok(invoiceOutputVM);
        //    }
        //    else {
        //        return Content(error.ErrorStatusCode, error);
        //    }
        //}

        //#endregion

        //#endregion

        #endregion

        #region Private Methods
        private bool DoImageReplacement(OrderMasterVM order)
        {
            string branchCode = order.BranchCode;
            int orderNo = order.OrderNo;
            string destFolder = ConfigurationManager.AppSettings["OrdImgFolder"].ToString();
            string originalImagePath = HttpContext.Current.Server.MapPath("~/" + destFolder);
            OrderMasterVM originalOrderDet = order;
            List<OrderItemDetailsVM> orginalDet = originalOrderDet.lstOfOrderItemDetailsVM;
            foreach (OrderItemDetailsVM ordDet in orginalDet) {
                if (ordDet.ImgID != null && ordDet.ImgID != "") {
                    int slNo = 1;// ordDet.SlNo;
                    string url = ordDet.ImgID;
                    string orgFileName = Path.GetFileName(url);
                    string newFileName = order.BranchCode + "-" + orderNo + "_" + slNo + Path.GetExtension(url);
                    string copyFrom = originalImagePath + "\\" + orgFileName;
                    File.Copy(copyFrom, Path.Combine(originalImagePath, newFileName));

                    //Updating to corresponding Order
                    ErrorVM error = new ErrorVM();
                    bool saved = new OrderBL().UpdatOrderImageURL(order.CompanyCode, order.BranchCode, order.OrderNo, slNo, "\\" + newFileName, out error);
                    if (error != null) {
                        return false;
                    }
                    // after updating to Database do delete the file
                    File.Delete(copyFrom);
                }
            }
            return true;
        }

        IQueryable<OrderMasterVM> IBaseMasterActionController<OrderMasterVM, OrderMasterVM>.List()
        {
            throw new NotImplementedException();
        }

        IHttpActionResult IBaseMasterActionController<OrderMasterVM, OrderMasterVM>.Count(ODataQueryOptions<OrderMasterVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        IHttpActionResult IBaseMasterActionController<OrderMasterVM, OrderMasterVM>.Get(int id)
        {
            throw new NotImplementedException();
        }

        IHttpActionResult IBaseMasterActionController<OrderMasterVM, OrderMasterVM>.Post(OrderMasterVM t)
        {
            throw new NotImplementedException();
        }

        IHttpActionResult IBaseMasterActionController<OrderMasterVM, OrderMasterVM>.Put(int id, OrderMasterVM t)
        {
            throw new NotImplementedException();
        }

        IHttpActionResult IBaseMasterActionController<OrderMasterVM, OrderMasterVM>.Delete(int id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region User defined Methods / Unused Important code
        //private ErrorVM OrderAccountPosting(List<PaymentVM> payments, string customer)
        //{
        //    string narrationPrefix = "Order Advance", vType = "OREC", party_name = customer, errorMessage, memberShip;
        //    decimal taxPer = 0, percentage = 0, sgstAmount = 0, igstAmount = 0, cgstAmount = 0, cessAmount = 0, sgstPer = 0, igstPer = 0, cgstPer = 0, cessPercent = 0;
        //    int accountCode = 7, sgstAccountCode = 0, igstAccountCode = 0, cgstAccountCode = 0, accCode = 0, cessAccountCode = 0;
        //    DateTime dtVoucherDate = Common.GetDateTime();
        //    int FinYear = db.KSTU_ACC_FY_MASTER.FirstOrDefault().fin_year;
        //    int FinPeriod = Convert.ToInt32(db.KSTU_ACC_FY_MASTER.FirstOrDefault().fin_period);

        //    if (payments != null && payments.Count > 0) {
        //        for (int i = 0; i < payments.Count; i++) {
        //            string receiptNo = string.Empty;
        //            decimal Amount, gstPer, gstAmount, finalGSTAmount;
        //            int taxid;
        //            string payMode = payments[i].PayMode;
        //            int seriesNo = payments[i].SeriesNo;
        //            int refNo = payments[i].ReceiptNo;
        //            string branch = payMode.Substring(0, 1);

        //            accCode = 0;
        //            sgstPer = Convert.ToDecimal(payments[i].SGSTPercent);
        //            cgstPer = Convert.ToDecimal(payments[i].CGSTPercent);
        //            igstPer = Convert.ToDecimal(payments[i].IGSTPercent);
        //            int orderNo = payments[i].SeriesNo;

        //            string gstGroupCode = payments[i].GSTGroupCode.ToString();
        //            GSTPostingSetup gstPostSetupSGSTAccountCode = db.GSTPostingSetups.Where(gst => gst.GSTGroupCode == gstGroupCode && gst.GSTPercent == sgstPer && gst.IsRegistered == true && gst.GSTComponentCode == "SGST").FirstOrDefault();
        //            sgstAccountCode = gstPostSetupSGSTAccountCode == null ? 0 : Convert.ToInt32(gstPostSetupSGSTAccountCode.PayableAccount);

        //            GSTPostingSetup gstPostSetupCGSTAccountCode = db.GSTPostingSetups.Where(gst => gst.GSTGroupCode == gstGroupCode && gst.GSTPercent == sgstPer && gst.IsRegistered == true && gst.GSTComponentCode == "CGST").FirstOrDefault();
        //            cgstAccountCode = gstPostSetupCGSTAccountCode == null ? 0 : Convert.ToInt32(gstPostSetupCGSTAccountCode.PayableAccount);

        //            GSTPostingSetup gstPostSetupIGSTAccountCode = db.GSTPostingSetups.Where(gst => gst.GSTGroupCode == gstGroupCode && gst.GSTPercent == sgstPer && gst.IsRegistered == true && gst.GSTComponentCode == "IGST").FirstOrDefault();
        //            igstAccountCode = gstPostSetupIGSTAccountCode == null ? 0 : Convert.ToInt32(gstPostSetupIGSTAccountCode.PayableAccount);

        //            receiptNo = seriesNo.ToString();
        //            receiptNo = refNo > 0 ? seriesNo + "," + refNo : "";
        //            Amount = Convert.ToDecimal(payments[i].PayAmount);

        //            if (string.IsNullOrEmpty(payments[i].CurrencyType)) {
        //                payments[i].CurrencyType = db.KSTU_COMPANY_MASTER.FirstOrDefault().default_currency_code;
        //            }
        //            if (vType == "OREC") {
        //                KTTU_ORDER_RECEIPT_DETAILS kord = db.KTTU_ORDER_RECEIPT_DETAILS.Where(ord => ord.order_no == orderNo
        //                  && ord.company_code == Common.CompanyCode
        //                  && ord.bracnch_code == Common.BranchCode).Distinct().FirstOrDefault();
        //                taxPer = Convert.ToDecimal(kord != null ? kord.tax_percentage : 0);
        //            }
        //            else {
        //                taxPer = 7;
        //            }
        //            taxid = db.KSTS_SALETAX_MASTER.Where(ksm => ksm.tax == taxPer
        //                                                    && ksm.company_code == Common.CompanyCode
        //                                                    && ksm.branch_code == Common.BranchCode).FirstOrDefault().tax_Id;
        //            gstPer = cgstPer + sgstPer + igstPer;
        //            gstAmount = decimal.Round(((Amount) - (((Amount) * 100) / (100 + gstPer))), 2);
        //            finalGSTAmount = gstAmount / 2;
        //            if (igstPer > 0) {
        //                igstAmount = gstAmount;
        //            }
        //            else {
        //                cgstAmount = finalGSTAmount;
        //                sgstAmount = finalGSTAmount;
        //            }

        //            switch (payMode) {
        //                case "C": //Tested
        //                    #region Cash Payment
        //                    KSTU_ACC_VOUCHER_TRANSACTIONS voucherTransDummy = GetAccountVourcherTranObject(vType, FinPeriod, FinYear, dtVoucherDate, receiptNo, accCode, party_name);
        //                    KSTU_ACC_VOUCHER_TRANSACTIONS voucherTrans = GetCashAccountVourcherTranObject(voucherTransDummy, payments[i].CurrencyType, narrationPrefix + " (Cash) ");
        //                    decimal FinalAmount;
        //                    string accountCodeMaster = string.Empty;
        //                    FinalAmount = Amount - cgstAmount - sgstAmount - igstAmount;

        //                    if (string.Compare(vType, "BREC") == 0) {
        //                        accountCodeMaster = voucherTrans.acc_code_master.ToString();
        //                        voucherTrans.cr_amount = Amount;
        //                        InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                        if (errorMessage != "") {
        //                            return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                        }
        //                    }
        //                    else {
        //                        accountCodeMaster = voucherTrans.acc_code_master.ToString();
        //                        for (int k = 0; k < 4; k++) {
        //                            if (k == 0) {
        //                                voucherTrans.cr_amount = FinalAmount;
        //                            }
        //                            else if (k == 1 && sgstAmount > 0) {
        //                                voucherTrans.acc_code = Convert.ToInt32(db.GSTPostingSetups.Where(gst => gst.GSTGroupCode == payments[i].GSTGroupCode && gst.GSTPercent == sgstPer && gst.IsRegistered == true && gst.GSTComponentCode == "SGST").FirstOrDefault().PayableAccount);
        //                                voucherTrans.cr_amount = sgstAmount;
        //                            }
        //                            else if (k == 2 && cgstAmount > 0) {
        //                                voucherTrans.acc_code = Convert.ToInt32(db.GSTPostingSetups.Where(gst => gst.GSTGroupCode == payments[i].GSTGroupCode && gst.GSTPercent == cgstPer && gst.IsRegistered == true && gst.GSTComponentCode == "CGST").FirstOrDefault().PayableAccount);
        //                                voucherTrans.cr_amount = cgstAmount;
        //                            }
        //                            else if (k == 3 && igstAmount > 0) {
        //                                voucherTrans.acc_code = Convert.ToInt32(db.GSTPostingSetups.Where(gst => gst.GSTGroupCode == payments[i].GSTGroupCode && gst.GSTPercent == igstPer && gst.IsRegistered == true && gst.GSTComponentCode == "IGST").FirstOrDefault().PayableAccount);
        //                                voucherTrans.cr_amount = igstAmount;
        //                            }
        //                            else {
        //                                voucherTrans.cr_amount = 0;
        //                            }
        //                            voucherTrans.dr_amount = 0;
        //                            //voucherTrans.txt_seq_no = db.KSTS_ACC_SEQ_NOS.Where(kasn => kasn.obj_id == "03").FirstOrDefault().nextno;
        //                            if (voucherTrans.cr_amount > 0) {
        //                                if (voucherTrans.cr_amount > 0) {
        //                                    InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                                    if (errorMessage != "") {
        //                                        return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                    Common.UpdateAccountVourcherSeqenceNumber(db, accountCodeMaster);
        //                    #endregion
        //                    break;
        //                //case "PB":
        //                //    #region Purchase Bill
        //                //    decimal taxVal = 0;
        //                //    voucherTrans.acc_type = "O";
        //                //    voucherTrans.acc_code_master = 0;
        //                //    voucherTrans.cr_amount = Amount;
        //                //    voucherTrans.dr_amount = 0;
        //                //    voucherTrans.voucher_no = db.KSTS_ACC_SEQ_NOS.Where(kasn => kasn.obj_id == "20").FirstOrDefault().nextno;
        //                //    voucherTrans.narration = narrationPrefix + " (P BILL)";
        //                //    FinalAmount = Amount - cgstAmount - sgstAmount - igstAmount;

        //                //    if (string.Compare(vType, "BREC") == 0)
        //                //    {
        //                //        voucherTrans.cr_amount = Amount;
        //                //        InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                //        if (errorMessage != "")
        //                //        {
        //                //            return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                //        }
        //                //    }
        //                //    else
        //                //    {
        //                //        voucherTrans.cr_amount = FinalAmount;
        //                //        if (sgstAmount > 0)
        //                //        {
        //                //            voucherTrans.acc_code = Convert.ToInt32(db.GSTPostingSetups.Where(gst => gst.GSTGroupCode == payments[i].GSTGroupCode && gst.GSTPercent == sgstPer && gst.IsRegistered == true && gst.GSTComponentCode == "SGST").FirstOrDefault().PayableAccount);
        //                //            voucherTrans.cr_amount = sgstAmount;
        //                //        }
        //                //        else
        //                //        {
        //                //            voucherTrans.cr_amount = 0;
        //                //        }

        //                //        if (cgstAmount > 0)
        //                //        {
        //                //            voucherTrans.acc_code = Convert.ToInt32(db.GSTPostingSetups.Where(gst => gst.GSTGroupCode == payments[i].GSTGroupCode && gst.GSTPercent == cgstPer && gst.IsRegistered == true && gst.GSTComponentCode == "CGST").FirstOrDefault().PayableAccount);
        //                //            voucherTrans.cr_amount = cgstAmount;
        //                //        }
        //                //        else
        //                //        {
        //                //            voucherTrans.cr_amount = 0;
        //                //        }
        //                //        if (igstAmount > 0)
        //                //        {
        //                //            voucherTrans.acc_code = Convert.ToInt32(db.GSTPostingSetups.Where(gst => gst.GSTGroupCode == payments[i].GSTGroupCode && gst.GSTPercent == igstPer && gst.IsRegistered == true && gst.GSTComponentCode == "IGST").FirstOrDefault().PayableAccount);
        //                //            voucherTrans.cr_amount = igstAmount;
        //                //        }
        //                //        else
        //                //        {
        //                //            voucherTrans.cr_amount = 0;
        //                //        }

        //                //        voucherTrans.dr_amount = 0;
        //                //        if (voucherTrans.cr_amount > 0)
        //                //        {
        //                //            if (voucherTrans.cr_amount > 0)
        //                //            {
        //                //                InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                //                if (errorMessage != "")
        //                //                {
        //                //                    return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                //                }
        //                //            }
        //                //        }
        //                //    }
        //                //    var lstPurchase = (from kpd in db.KTTU_PURCHASE_DETAILS
        //                //                       where kpd.bill_no == Convert.ToInt32(payments[i].RefBillNo)
        //                //                       group kpd by new
        //                //                       {
        //                //                           kpd.gs_code,
        //                //                           kpd.itemwise_tax_percentage,
        //                //                           kpd.SGST_Percent,
        //                //                           kpd.CGST_Percent,
        //                //                           kpd.IGST_Percent
        //                //                       } into g
        //                //                       select new
        //                //                       {
        //                //                           Amount = g.Sum(x => x.itemwise_purchase_amount),
        //                //                           TaxAmount = g.Sum(x => x.itemwise_tax_amount),
        //                //                           TaxPer = g.Key.itemwise_tax_percentage,
        //                //                           SGSTPercent = g.Key.SGST_Percent,
        //                //                           SGSTAmount = g.Sum(x => x.SGST_Amount),
        //                //                           CGSTPercent = g.Key.CGST_Percent,
        //                //                           CGSTAmount = g.Sum(x => x.CGST_Amount),
        //                //                           IGSTPercent = g.Key.IGST_Percent,
        //                //                           IGSTAmount = g.Sum(x => x.IGST_Amount),
        //                //                           GSCode = g.Key.gs_code
        //                //                       }).ToList();
        //                //    if (lstPurchase != null && lstPurchase.Count > 0)
        //                //    {
        //                //        decimal taxPercent = Convert.ToInt32(lstPurchase[i].TaxPer);
        //                //        int refBillNo = Convert.ToInt32(payments[i].RefBillNo);
        //                //        string gsCode = db.KTTU_PURCHASE_MASTER.Where(kpm => kpm.bill_no == refBillNo && kpm.company_code == Common.CompanyCode
        //                //                                      && kpm.branch_code == Common.BranchCode).FirstOrDefault().pur_item;
        //                //        accCode = db.KSTS_ACC_POSTING_SETUP.Where(kaps => kaps.gs_code == lstPurchase[i].GSCode && kaps.trans_type == "P").FirstOrDefault().acc_code;
        //                //        var diamondAmout = from kpd in db.KTTU_PURCHASE_DETAILS
        //                //                           group kpd by 1 into g
        //                //                           select new
        //                //                           {
        //                //                               diamondAmount = g.Sum(x => x.diamond_amount)
        //                //                           };
        //                //        decimal diaAmount = Convert.ToDecimal(diamondAmout.FirstOrDefault().diamondAmount);
        //                //        if (accCode == 0)
        //                //        {
        //                //            return new ErrorVM() { field = "", index = 0, description = "Failed to update accounts: Account Code" };
        //                //        }
        //                //        voucherTrans.acc_code = accCode;

        //                //        if (db.APP_CONFIG_TABLE.Where(act => act.obj_id == "2001").FirstOrDefault().value == 1)
        //                //        {
        //                //            voucherTrans.dr_amount = Math.Abs(Amount - diaAmount);
        //                //        }
        //                //        else
        //                //        {
        //                //            voucherTrans.dr_amount = Math.Abs(Amount);
        //                //        }

        //                //        voucherTrans.cr_amount = 0;
        //                //        voucherTrans.voucher_seq_no = i + 1;

        //                //        if (voucherTrans.dr_amount > 0)
        //                //        {
        //                //            InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                //            if (errorMessage != "")
        //                //            {
        //                //                return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                //            }
        //                //        }

        //                //        if (db.APP_CONFIG_TABLE.Where(act => act.obj_id == "2001").FirstOrDefault().value == 1)
        //                //        {
        //                //            if (diaAmount > 0)
        //                //            {
        //                //                accCode = db.KSTS_ACC_POSTING_SETUP.Where(kaps => kaps.gs_code == "OD" && kaps.trans_type == "P").FirstOrDefault().acc_code;
        //                //                voucherTrans.acc_code = accCode;
        //                //                voucherTrans.cr_amount = 0;
        //                //                voucherTrans.dr_amount = diaAmount;
        //                //                voucherTrans.voucher_seq_no = i + 1;
        //                //                if (voucherTrans.dr_amount > 0)
        //                //                {
        //                //                    InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                //                    if (errorMessage != "")
        //                //                    {
        //                //                        return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                //                    }
        //                //                }
        //                //            }
        //                //        }
        //                //        Common.UpdateAccountSeqenceNumber(db, "20");
        //                //    }
        //                //    #endregion
        //                //    break;
        //                //case "SR":
        //                //    #region SalesReturn
        //                //    taxVal = 0;
        //                //    FinalAmount = Amount - cgstAmount - sgstAmount - igstAmount;
        //                //    voucherTrans.acc_type = "O";
        //                //    voucherTrans.acc_code_master = 0;
        //                //    voucherTrans.voucher_no = db.KSTS_ACC_SEQ_NOS.Where(kasn => kasn.obj_id == "20").FirstOrDefault().nextno;
        //                //    voucherTrans.narration = narrationPrefix + " (SR BILL)";

        //                //    if (string.Compare(vType, "BREC") == 0)
        //                //    {
        //                //        voucherTrans.cr_amount = Amount;
        //                //        InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                //        if (errorMessage != "")
        //                //        {
        //                //            return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                //        }
        //                //    }
        //                //    else
        //                //    {
        //                //        for (int k = 0; i < 4; i++)
        //                //        {
        //                //            if (k == 0)
        //                //            {
        //                //                voucherTrans.cr_amount = FinalAmount;
        //                //            }

        //                //            else if (k == 1 && sgstAmount > 0)
        //                //            {
        //                //                voucherTrans.acc_code = sgstAccountCode;
        //                //                voucherTrans.cr_amount = sgstAmount;
        //                //            }

        //                //            else if (k == 2 && cgstAmount > 0)
        //                //            {
        //                //                voucherTrans.acc_code = cgstAccountCode;
        //                //                voucherTrans.cr_amount = cgstAmount;
        //                //            }

        //                //            else if (k == 3 && igstAmount > 0)
        //                //            {
        //                //                voucherTrans.acc_code = igstAccountCode;
        //                //                voucherTrans.cr_amount = igstAmount;
        //                //            }
        //                //            else
        //                //                voucherTrans.cr_amount = 0;

        //                //            voucherTrans.dr_amount = 0;
        //                //            voucherTrans.voucher_seq_no = i + 1;

        //                //            if (voucherTrans.dr_amount > 0)
        //                //            {
        //                //                InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                //                if (errorMessage != "")
        //                //                {
        //                //                    return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                //                }
        //                //            }
        //                //        }
        //                //    }

        //                //    var lstSRS = (from ksd in db.KTTU_SR_DETAILS
        //                //                  where ksd.sales_bill_no == orderNo
        //                //                  group ksd by new
        //                //                  {
        //                //                      ksd.gs_code,
        //                //                      ksd.tax_percentage,
        //                //                      ksd.GSTGroupCode,
        //                //                      ksd.CGST_Percent,
        //                //                      ksd.SGST_Percent,
        //                //                      ksd.IGST_Percent,
        //                //                      cessPercent
        //                //                  } into g
        //                //                  select new
        //                //                  {
        //                //                      Amount = g.Sum(x => x.item_total_after_discount),
        //                //                      TaxAmount = g.Sum(x => x.tax_amount),
        //                //                      FinalAmount = g.Sum(x => x.item_final_amount),
        //                //                      TaxPer = g.Key.tax_percentage,
        //                //                      SGSTPercent = g.Key.SGST_Percent,
        //                //                      SGSTAmount = g.Sum(x => x.SGST_Amount),
        //                //                      CGSTPercent = g.Key.CGST_Percent,
        //                //                      CGSTAmount = g.Sum(x => x.CGST_Amount),
        //                //                      IGSTPercent = g.Key.IGST_Percent,
        //                //                      IGSTAmount = g.Sum(x => x.IGST_Amount),
        //                //                      GSCode = g.Key.gs_code,
        //                //                      GSTGroupCode = g.Key.GSTGroupCode,
        //                //                      CessPercent = g.Key.cessPercent,
        //                //                      CessAmount = g.Sum(x => x.CESSAmount)
        //                //                  }).ToList();
        //                //    for (int k = 0; k < lstSRS.Count; k++)
        //                //    {
        //                //        taxPer = Convert.ToDecimal(lstSRS[k].TaxPer);
        //                //        int taxids = db.KSTS_SALETAX_MASTER.Where(ksm => ksm.company_code == Common.CompanyCode && ksm.branch_code == Common.BranchCode).FirstOrDefault().tax_Id;
        //                //        accCode = db.KSTS_ACC_POSTING_SETUP.Where(kaps => kaps.gs_code == lstSRS[k].GSCode
        //                //                                                    && kaps.trans_type == "RS"
        //                //                                                    && kaps.company_code == Common.CompanyCode
        //                //                                                    && kaps.branch_code == Common.BranchCode).FirstOrDefault().acc_code;

        //                //        if (accCode == 0)
        //                //        {
        //                //            return new ErrorVM() { field = "", index = 0, description = "SR Bill ledger mapping is not done , please map the ledger in accounts posting setup" };
        //                //        }

        //                //        voucherTrans.acc_code = accCode;
        //                //        string gstCode = lstSRS[k].GSTGroupCode;
        //                //        Amount = Math.Round(Convert.ToDecimal(lstSRS[k].FinalAmount), 0);

        //                //        sgstPer = Convert.ToDecimal(lstSRS[k].SGSTPercent);
        //                //        cgstPer = Convert.ToDecimal(lstSRS[k].CGSTPercent);
        //                //        igstPer = Convert.ToDecimal(lstSRS[k].IGSTPercent);
        //                //        cessPercent = Convert.ToDecimal(lstSRS[k].CessPercent);

        //                //        sgstAccountCode = GetGSTAccountCodePayable(gstCode, sgstPer, 1, "SGST");
        //                //        cgstAccountCode = GetGSTAccountCodePayable(gstCode, cgstPer, 1, "CGST");
        //                //        igstAccountCode = GetGSTAccountCodePayable(gstCode, igstPer, 1, "IGST");
        //                //        cessAccountCode = GetGSTAccountCodePayable(gstCode, cessPercent, 1, "CESS");

        //                //        sgstAmount = Convert.ToDecimal(lstSRS[k].SGSTAmount);
        //                //        cgstAmount = Convert.ToDecimal(lstSRS[k].CGSTAmount);
        //                //        igstAmount = Convert.ToDecimal(lstSRS[k].IGSTAmount);
        //                //        cessAmount = Convert.ToDecimal(lstSRS[k].CessAmount);

        //                //        decimal gstVal = decimal.Round(((Amount) - (((Amount) * 100) / (100 + taxPer))), 2);
        //                //        decimal TotalAmount = Amount - gstVal - sgstAmount - cgstAmount - igstAmount - cessAmount;

        //                //        for (int j = 0; j < 6; j++)
        //                //        {
        //                //            if (j == 0)
        //                //            {
        //                //                voucherTrans.dr_amount = TotalAmount;
        //                //                voucherTrans.voucher_seq_no = 3;
        //                //            }
        //                //            else if (j == 1 && sgstAmount > 0)
        //                //            {
        //                //                voucherTrans.acc_code = sgstAccountCode;
        //                //                voucherTrans.dr_amount = sgstAmount;
        //                //            }
        //                //            else if (j == 2 && cgstAmount > 0)
        //                //            {
        //                //                voucherTrans.acc_code = cgstAccountCode;
        //                //                voucherTrans.dr_amount = cgstAmount;
        //                //            }
        //                //            else if (j == 3 && igstAmount > 0)
        //                //            {
        //                //                voucherTrans.acc_code = igstAccountCode;
        //                //                voucherTrans.dr_amount = igstAmount;
        //                //            }
        //                //            else if (j == 4 && cessAmount > 0)
        //                //            {
        //                //                voucherTrans.acc_code = cessAccountCode;
        //                //                voucherTrans.dr_amount = cessAmount;
        //                //            }
        //                //            else if (j == 5)
        //                //            {
        //                //                voucherTrans.acc_code = GetAccountCode("T", "P", taxids.ToString());
        //                //                voucherTrans.dr_amount = Convert.ToDecimal(lstSRS[k].TaxAmount);
        //                //                voucherTrans.voucher_seq_no = 4;
        //                //            }
        //                //            else
        //                //                voucherTrans.dr_amount = 0;

        //                //            voucherTrans.cr_amount = 0;
        //                //            if (voucherTrans.dr_amount > 0)
        //                //            {
        //                //                InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                //                if (errorMessage != "")
        //                //                {
        //                //                    return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                //                }
        //                //            }
        //                //        }
        //                //    }
        //                //    Common.UpdateAccountSeqenceNumber(db, "20");
        //                //    #endregion
        //                //    break;
        //                //case "CT":
        //                //    #region Chit Bill
        //                //    FinalAmount = Amount - cgstAmount - sgstAmount - igstAmount;
        //                //    voucherTrans.acc_type = "O";
        //                //    voucherTrans.acc_code_master = 0;
        //                //    voucherTrans.voucher_no = db.KSTS_ACC_SEQ_NOS.Where(kasn => kasn.obj_id == "20").FirstOrDefault().nextno;
        //                //    voucherTrans.voucher_seq_no = 1;
        //                //    voucherTrans.cr_amount = Amount;
        //                //    voucherTrans.dr_amount = 0;
        //                //    voucherTrans.narration = narrationPrefix + " (Scheme)";

        //                //    for (int k = 0; k < 4; k++)
        //                //    {
        //                //        if (k == 0)
        //                //        {
        //                //            voucherTrans.cr_amount = FinalAmount;
        //                //        }
        //                //        else if (k == 1 && sgstAmount > 0)
        //                //        {
        //                //            voucherTrans.acc_code = sgstAccountCode;
        //                //            voucherTrans.cr_amount = sgstAmount;
        //                //        }

        //                //        else if (k == 2 && cgstAmount > 0)
        //                //        {
        //                //            voucherTrans.acc_code = cgstAccountCode;
        //                //            voucherTrans.cr_amount = cgstAmount;
        //                //        }

        //                //        else if (k == 3 && igstAmount > 0)
        //                //        {
        //                //            voucherTrans.acc_code = igstAccountCode;
        //                //            voucherTrans.cr_amount = igstAmount;
        //                //        }
        //                //        else
        //                //        {
        //                //            voucherTrans.cr_amount = 0;
        //                //        }

        //                //        voucherTrans.dr_amount = 0;
        //                //        voucherTrans.txt_seq_no = db.KSTS_ACC_SEQ_NOS.Where(kasn => kasn.obj_id == "03").FirstOrDefault().nextno;
        //                //        if (voucherTrans.cr_amount > 0)
        //                //        {
        //                //            InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                //            if (errorMessage != "")
        //                //            {
        //                //                return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                //            }
        //                //        }
        //                //    }

        //                //    if (string.Compare(payments[i].PartyCode.ToString(), Common.BranchCode) == 0)
        //                //    {
        //                //        string schemeCode = payments[i].SchemeCode;
        //                //        accCode = 0;
        //                //        if (GetConfigurationValue("30012019") == 1) // bhima CPC
        //                //        {
        //                //            accCode = GetMagnaAccountCode(60);
        //                //        }
        //                //        else
        //                //        {
        //                //            accCode = db.KSTU_ACC_LEDGER_MASTER.Where(kalm => kalm.scheme_code == schemeCode && kalm.company_code == Common.CompanyCode && kalm.branch_code == Common.BranchCode).FirstOrDefault().acc_code;
        //                //        }

        //                //        if (accCode == 0)
        //                //        {
        //                //            return new ErrorVM() { field = "", index = 0, description = "Failed to update accounts" };
        //                //        }

        //                //        voucherTrans.voucher_seq_no = 2;
        //                //        voucherTrans.cr_amount = 0;
        //                //        voucherTrans.dr_amount = Convert.ToDecimal(Amount - payments[i].WinAmt - payments[i].BonusAmt);
        //                //        voucherTrans.acc_code = accCode;
        //                //        voucherTrans.txt_seq_no = db.KSTS_ACC_SEQ_NOS.Where(kasn => kasn.obj_id == "03").FirstOrDefault().nextno;

        //                //        InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                //        if (errorMessage != "")
        //                //        {
        //                //            return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                //        }

        //                //        if (payments[i].BonusAmt > 0)
        //                //        {
        //                //            voucherTrans.voucher_seq_no = 3;
        //                //            voucherTrans.cr_amount = 0;
        //                //            voucherTrans.dr_amount = Convert.ToDecimal(payments[i].BonusAmt);
        //                //            voucherTrans.acc_code = 12;
        //                //            voucherTrans.txt_seq_no = db.KSTS_ACC_SEQ_NOS.Where(kasn => kasn.obj_id == "03").FirstOrDefault().nextno;
        //                //            InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                //            if (errorMessage != "")
        //                //            {
        //                //                return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                //            }
        //                //        }

        //                //        if (payments[i].WinAmt > 0)
        //                //        {
        //                //            voucherTrans.voucher_seq_no = 4;
        //                //            voucherTrans.cr_amount = 0;
        //                //            voucherTrans.dr_amount = Convert.ToDecimal(payments[i].WinAmt);
        //                //            voucherTrans.acc_code = 13;
        //                //            voucherTrans.txt_seq_no = db.KSTS_ACC_SEQ_NOS.Where(kasn => kasn.obj_id == "03").FirstOrDefault().nextno;
        //                //            InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                //            if (errorMessage != "")
        //                //            {
        //                //                return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                //            }
        //                //        }
        //                //    }
        //                //    else
        //                //    {
        //                //        accCode = Convert.ToInt32(db.KSTU_ACC_LEDGER_MASTER.Where(kalm => kalm.party_code == payments[i].PartyCode && kalm.company_code == Common.CompanyCode && kalm.branch_code == Common.BranchCode));
        //                //        if (accCode == 0)
        //                //        {
        //                //            return new ErrorVM() { field = "", index = 0, description = "Failed to update accounts" };
        //                //        }

        //                //        voucherTrans.voucher_seq_no = 2;
        //                //        voucherTrans.cr_amount = 0;
        //                //        voucherTrans.dr_amount = Convert.ToDecimal(payments[i].PayAmount);
        //                //        voucherTrans.acc_code = accCode;
        //                //        voucherTrans.txt_seq_no = db.KSTS_ACC_SEQ_NOS.Where(kasn => kasn.obj_id == "03").FirstOrDefault().nextno;
        //                //        memberShip = " " + (payments[i].SchemeCode + "/" + payments[i].GroupCode + "/ Mem No: " + payments[i].RefBillNo + "/" + payments[i].PartyCode);
        //                //        voucherTrans.narration = " Branch " + payments[i].PartyCode + " Scheme Adjusted in " + Common.BranchCode + "Order " + memberShip;
        //                //        voucherTrans.trans_type = "OREC";
        //                //        InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                //        if (errorMessage != "")
        //                //        {
        //                //            return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                //        }
        //                //    }
        //                //    Common.UpdateAccountSeqenceNumber(db, "20");
        //                //    #endregion
        //                //    break;
        //                case "Q": //Tested
        //                case "D": // Tested
        //                    #region Cheque or DD
        //                    KSTU_ACC_VOUCHER_TRANSACTIONS chqDDTransDummy = GetAccountVourcherTranObject(vType, FinPeriod, FinYear, dtVoucherDate, receiptNo, accCode, party_name);
        //                    KSTU_ACC_VOUCHER_TRANSACTIONS chqDDVouchTrans = GetChequeOrDDAccountVourcherTranObject(chqDDTransDummy, Amount, payments[i].ChequeNo, Convert.ToDateTime(payments[i].ChequeDate), narrationPrefix);
        //                    FinalAmount = Amount - cgstAmount - sgstAmount - igstAmount;

        //                    if (string.Compare(vType, "BREC") == 0) {

        //                        InsertVoucherTransactions(out errorMessage, chqDDVouchTrans);
        //                        if (errorMessage != "") {
        //                            return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                        }
        //                    }
        //                    else {
        //                        for (int k = 0; k < 4; k++) {
        //                            if (k == 0) {
        //                                chqDDVouchTrans.cr_amount = FinalAmount;
        //                            }

        //                            else if (k == 1 && sgstAmount > 0) {
        //                                chqDDVouchTrans.acc_code = sgstAccountCode;
        //                                chqDDVouchTrans.cr_amount = sgstAmount;
        //                            }

        //                            else if (k == 2 && cgstAmount > 0) {
        //                                chqDDVouchTrans.acc_code = cgstAccountCode;
        //                                chqDDVouchTrans.cr_amount = cgstAmount;
        //                            }

        //                            else if (k == 3 && igstAmount > 0) {
        //                                chqDDVouchTrans.acc_code = igstAccountCode;
        //                                chqDDVouchTrans.cr_amount = igstAmount;
        //                            }
        //                            else {
        //                                chqDDVouchTrans.cr_amount = 0;
        //                            }
        //                            chqDDVouchTrans.dr_amount = 0;
        //                            if (chqDDVouchTrans.cr_amount > 0) {
        //                                InsertVoucherTransactions(out errorMessage, chqDDVouchTrans);
        //                                if (errorMessage != "") {
        //                                    return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                                }
        //                            }
        //                        }
        //                    }

        //                    chqDDVouchTrans.voucher_seq_no = 2;
        //                    chqDDVouchTrans.cr_amount = 0;
        //                    chqDDVouchTrans.dr_amount = Amount;
        //                    accCode = GetAccountPostingSetup("PM", "Q");
        //                    if (accCode == 0) {
        //                        return new ErrorVM() { field = "", index = 0, description = "ledger mapping is not done , please map the ledger in accounts posting setup" };
        //                    }
        //                    chqDDVouchTrans.acc_code = accCode;
        //                    InsertVoucherTransactions(out errorMessage, chqDDVouchTrans);
        //                    if (errorMessage != "") {
        //                        return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                    }
        //                    Common.UpdateAccountSeqenceNumber(db, "20");
        //                    #endregion
        //                    break;
        //                case "R":
        //                    //#region Card
        //                    //voucherTrans.acc_type = "O";
        //                    //voucherTrans.acc_code_master = 0;
        //                    //voucherTrans.voucher_no = db.KSTS_ACC_SEQ_NOS.Where(kasn => kasn.obj_id == "20").FirstOrDefault().nextno;
        //                    //voucherTrans.voucher_seq_no = 1;
        //                    //voucherTrans.narration = narrationPrefix + " (Card)";

        //                    //FinalAmount = Amount - cgstAmount - sgstAmount - igstAmount;
        //                    //if (string.Compare(vType, "BREC") == 0)
        //                    //{
        //                    //    voucherTrans.cr_amount = Amount;

        //                    //    InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                    //    if (errorMessage != "")
        //                    //    {
        //                    //        return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                    //    }
        //                    //}
        //                    //else
        //                    //{
        //                    //    for (int k = 0; k < 4; k++)
        //                    //    {
        //                    //        if (k == 0)
        //                    //        {
        //                    //            voucherTrans.cr_amount = FinalAmount;
        //                    //        }

        //                    //        else if (k == 1 && sgstAmount > 0)
        //                    //        {
        //                    //            voucherTrans.acc_code = sgstAccountCode;//CGlobals.GetAccountCode("T", "S", taxid.ToString());
        //                    //            voucherTrans.cr_amount = sgstAmount;
        //                    //        }

        //                    //        else if (k == 2 && cgstAmount > 0)
        //                    //        {
        //                    //            voucherTrans.acc_code = cgstAccountCode;
        //                    //            voucherTrans.cr_amount = cgstAmount;
        //                    //        }

        //                    //        else if (k == 3 && igstAmount > 0)
        //                    //        {
        //                    //            voucherTrans.acc_code = igstAccountCode;
        //                    //            voucherTrans.cr_amount = igstAmount;
        //                    //        }
        //                    //        else
        //                    //            voucherTrans.cr_amount = 0;

        //                    //        voucherTrans.dr_amount = 0;

        //                    //        if (voucherTrans.cr_amount > 0)
        //                    //        {
        //                    //            InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                    //            if (errorMessage != "")
        //                    //            {
        //                    //                return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                    //            }
        //                    //        }
        //                    //    }
        //                    //}
        //                    //accCode = Convert.ToInt32(payments[i].ChequeNo);
        //                    //if (accCode == 0)
        //                    //{
        //                    //    return new ErrorVM() { field = "", index = 0, description = "Failed to update accounts" };
        //                    //}
        //                    //voucherTrans.voucher_seq_no = 2;
        //                    //voucherTrans.cr_amount = 0;
        //                    //voucherTrans.dr_amount = Amount;
        //                    //voucherTrans.acc_code = accCode;

        //                    //InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                    //if (errorMessage != "")
        //                    //{
        //                    //    return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                    //}
        //                    //Common.UpdateAccountSeqenceNumber(db, "20");
        //                    //#endregion
        //                    break;
        //                    //case "EP":
        //                    //case "FC":
        //                    //    #region E-payment
        //                    //    if (string.Compare(payments[i].CurrencyType, GetDefaultCurrencyCode()) != 0)
        //                    //    {
        //                    //        voucherTrans.acc_type = "O";
        //                    //        voucherTrans.acc_code_master = 0;
        //                    //        voucherTrans.voucher_no = db.KSTS_ACC_SEQ_NOS.Where(kasn => kasn.obj_id == "20").FirstOrDefault().nextno;
        //                    //        voucherTrans.voucher_seq_no = 1;
        //                    //        voucherTrans.cr_amount = Amount;
        //                    //        voucherTrans.dr_amount = 0;
        //                    //        if (string.Compare(payMode, "EP") == 0)
        //                    //            voucherTrans.narration = narrationPrefix + " (E PAY)";
        //                    //        else
        //                    //        {
        //                    //            voucherTrans.narration = narrationPrefix + string.Format(" (Exchange Currency) - '{0}'", payments[i].CurrencyType);
        //                    //            voucherTrans.currency_type = payments[i].CurrencyType;
        //                    //        }

        //                    //        FinalAmount = Amount - cgstAmount - sgstAmount - igstAmount;

        //                    //        if (string.Compare(vType, "BREC") == 0)
        //                    //        {
        //                    //            voucherTrans.cr_amount = Amount;
        //                    //            InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                    //            if (errorMessage != "")
        //                    //            {
        //                    //                return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                    //            }
        //                    //        }
        //                    //        else
        //                    //        {
        //                    //            for (int k = 0; k < 4; k++)
        //                    //            {
        //                    //                if (k == 0)
        //                    //                {
        //                    //                    voucherTrans.cr_amount = FinalAmount;
        //                    //                }

        //                    //                else if (k == 1 && sgstAmount > 0)
        //                    //                {
        //                    //                    voucherTrans.acc_code = sgstAccountCode;
        //                    //                    voucherTrans.cr_amount = sgstAmount;
        //                    //                }

        //                    //                else if (k == 2 && cgstAmount > 0)
        //                    //                {
        //                    //                    voucherTrans.acc_code = cgstAccountCode;
        //                    //                    voucherTrans.cr_amount = cgstAmount;
        //                    //                }

        //                    //                else if (k == 3 && igstAmount > 0)
        //                    //                {
        //                    //                    voucherTrans.acc_code = igstAccountCode;
        //                    //                    voucherTrans.cr_amount = igstAmount;
        //                    //                }
        //                    //                else
        //                    //                    voucherTrans.cr_amount = 0;

        //                    //                voucherTrans.dr_amount = 0;

        //                    //                if (voucherTrans.cr_amount > 0)
        //                    //                {
        //                    //                    InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                    //                    if (errorMessage != "")
        //                    //                    {
        //                    //                        return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                    //                    }
        //                    //                }
        //                    //            }
        //                    //        }
        //                    //        voucherTrans.voucher_seq_no = 2;
        //                    //        voucherTrans.cr_amount = 0;
        //                    //        voucherTrans.dr_amount = Amount;

        //                    //        if (string.Compare(payMode, "EP") == 0)
        //                    //            voucherTrans.acc_code = accCode = GetAccountPostingSetup("PM", payMode);
        //                    //        else
        //                    //            voucherTrans.acc_code = GetMagnaAccountCode(50);

        //                    //        if (accCode == 0)
        //                    //        {
        //                    //            return new ErrorVM() { field = "", index = 0, description = "ledger mapping is not done , please map the ledger in accounts posting setup" };
        //                    //        }

        //                    //        InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                    //        if (errorMessage != "")
        //                    //        {
        //                    //            return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                    //        }
        //                    //        Common.UpdateAccountSeqenceNumber(db, "20");
        //                    //    }
        //                    //    #endregion
        //                    //    break;
        //                    //case "GV":
        //                    //    #region Gift Voucher
        //                    //    voucherTrans.acc_type = "O";
        //                    //    voucherTrans.acc_code_master = 0;
        //                    //    voucherTrans.voucher_no = db.KSTS_ACC_SEQ_NOS.Where(kasn => kasn.obj_id == "20").FirstOrDefault().nextno;
        //                    //    voucherTrans.voucher_seq_no = 1;
        //                    //    voucherTrans.cr_amount = Amount;
        //                    //    voucherTrans.dr_amount = 0;
        //                    //    voucherTrans.narration = narrationPrefix + " (GV)";

        //                    //    FinalAmount = Amount - cgstAmount - sgstAmount - igstAmount;
        //                    //    if (string.Compare(vType, "BREC") == 0)
        //                    //    {
        //                    //        voucherTrans.cr_amount = Amount;
        //                    //        InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                    //        if (errorMessage != "")
        //                    //        {
        //                    //            return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                    //        }
        //                    //    }

        //                    //    else
        //                    //    {
        //                    //        for (int k = 0; k < 4; k++)
        //                    //        {
        //                    //            if (k == 0)
        //                    //            {
        //                    //                voucherTrans.cr_amount = FinalAmount;
        //                    //            }

        //                    //            else if (k == 1 && sgstAmount > 0)
        //                    //            {
        //                    //                voucherTrans.acc_code = sgstAccountCode;
        //                    //                voucherTrans.cr_amount = sgstAmount;
        //                    //            }

        //                    //            else if (k == 2 && cgstAmount > 0)
        //                    //            {
        //                    //                voucherTrans.acc_code = cgstAccountCode;
        //                    //                voucherTrans.cr_amount = cgstAmount;
        //                    //            }

        //                    //            else if (k == 3 && igstAmount > 0)
        //                    //            {
        //                    //                voucherTrans.acc_code = igstAccountCode;
        //                    //                voucherTrans.cr_amount = igstAmount;
        //                    //            }
        //                    //            else
        //                    //            {
        //                    //                voucherTrans.cr_amount = 0;
        //                    //            }

        //                    //            voucherTrans.dr_amount = 0;
        //                    //            if (voucherTrans.cr_amount > 0)
        //                    //            {
        //                    //                InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                    //                if (errorMessage != "")
        //                    //                {
        //                    //                    return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                    //                }
        //                    //            }
        //                    //        }
        //                    //    }
        //                    //    voucherTrans.voucher_seq_no = 2;
        //                    //    voucherTrans.cr_amount = 0;
        //                    //    voucherTrans.dr_amount = Amount;
        //                    //    accCode = GetAccountPostingSetup("PM", payMode);
        //                    //    if (accCode == 0)
        //                    //    {
        //                    //        return new ErrorVM() { field = "", index = 0, description = "ledger mapping is not done , please map the ledger in accounts posting setup" };
        //                    //    }
        //                    //    voucherTrans.acc_code = accCode;
        //                    //    InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                    //    if (errorMessage != "")
        //                    //    {
        //                    //        return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                    //    }
        //                    //    Common.UpdateAccountSeqenceNumber(db, "20");
        //                    //    #endregion
        //                    //    break;
        //                    //case "OGS":
        //                    //    #region Old Gold Scheme Kavitha
        //                    //    voucherTrans.acc_type = "O";
        //                    //    voucherTrans.acc_code_master = 0;
        //                    //    voucherTrans.voucher_no = db.KSTS_ACC_SEQ_NOS.Where(kasn => kasn.obj_id == "20").FirstOrDefault().nextno;
        //                    //    voucherTrans.voucher_seq_no = 1;
        //                    //    voucherTrans.cr_amount = Amount;
        //                    //    voucherTrans.dr_amount = 0;
        //                    //    voucherTrans.narration = narrationPrefix + " (GV)";

        //                    //    FinalAmount = Amount - cgstAmount - sgstAmount - igstAmount;
        //                    //    if (string.Compare(vType, "BREC") == 0)
        //                    //    {
        //                    //        voucherTrans.cr_amount = Amount;
        //                    //        InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                    //        if (errorMessage != "")
        //                    //        {
        //                    //            return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                    //        }
        //                    //    }
        //                    //    else
        //                    //    {
        //                    //        for (int k = 0; k < 4; k++)
        //                    //        {
        //                    //            if (k == 0)
        //                    //            {
        //                    //                voucherTrans.cr_amount = FinalAmount;
        //                    //            }

        //                    //            else if (k == 1 && sgstAmount > 0)
        //                    //            {
        //                    //                voucherTrans.acc_code = sgstAccountCode;
        //                    //                voucherTrans.cr_amount = sgstAmount;
        //                    //            }

        //                    //            else if (k == 2 && cgstAmount > 0)
        //                    //            {
        //                    //                voucherTrans.acc_code = cgstAccountCode;
        //                    //                voucherTrans.cr_amount = cgstAmount;
        //                    //            }

        //                    //            else if (k == 3 && igstAmount > 0)
        //                    //            {
        //                    //                voucherTrans.acc_code = igstAccountCode;
        //                    //                voucherTrans.cr_amount = igstAmount;
        //                    //            }
        //                    //            else
        //                    //            {
        //                    //                voucherTrans.cr_amount = 0;
        //                    //            }

        //                    //            voucherTrans.dr_amount = 0;

        //                    //            if (voucherTrans.cr_amount > 0)
        //                    //            {
        //                    //                InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                    //                if (errorMessage != "")
        //                    //                {
        //                    //                    return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                    //                }
        //                    //            }
        //                    //        }
        //                    //    }
        //                    //    voucherTrans.voucher_seq_no = 2;
        //                    //    voucherTrans.cr_amount = 0;
        //                    //    voucherTrans.dr_amount = Amount;

        //                    //    accCode = GetAccountPostingSetup("PM", payments[i].PayMode);
        //                    //    if (accCode == 0)
        //                    //    {
        //                    //        return new ErrorVM() { field = "", index = 0, description = "ledger mapping is not done , please map the ledger in accounts posting setup" };
        //                    //    }
        //                    //    voucherTrans.acc_code = accCode;

        //                    //    InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                    //    if (errorMessage != "")
        //                    //    {
        //                    //        return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                    //    }
        //                    //    Common.UpdateAccountSeqenceNumber(db, "20");
        //                    //    #endregion
        //                    //    break;
        //                    //case "B":
        //                    //    #region Inter-Branch
        //                    //    voucherTrans.acc_type = "O";
        //                    //    voucherTrans.acc_code_master = 0;
        //                    //    voucherTrans.voucher_no = db.KSTS_ACC_SEQ_NOS.Where(kasn => kasn.obj_id == "20").FirstOrDefault().nextno;
        //                    //    voucherTrans.voucher_seq_no = 1;
        //                    //    voucherTrans.cr_amount = Amount;
        //                    //    voucherTrans.dr_amount = 0;

        //                    //    if (string.Compare(payments[i].PayMode.ToString(), "BC") == 0)
        //                    //    {
        //                    //        voucherTrans.narration = "OLD SUVARNADHARA SCHEME - " + payments[i].PayDetails.ToString();
        //                    //        voucherTrans.voucher_type = "IB";
        //                    //    }
        //                    //    else
        //                    //    {
        //                    //        voucherTrans.narration = "Branch " + Common.BranchCode + "  " + narrationPrefix + " (Inter-Branch) Collected at - " + payments[i].PartyCode.ToString();
        //                    //    }

        //                    //    voucherTrans.txt_seq_no = db.KSTS_ACC_SEQ_NOS.Where(kasn => kasn.obj_id == "03").FirstOrDefault().nextno;

        //                    //    InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                    //    if (errorMessage != "")
        //                    //    {
        //                    //        return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                    //    }
        //                    //    Common.UpdateAccountSeqenceNumber(db, "20");

        //                    //    voucherTrans.voucher_seq_no = 2;
        //                    //    voucherTrans.cr_amount = 0;
        //                    //    voucherTrans.dr_amount = Amount;

        //                    //    accCode = GetAccountPostingSetup("PM", payments[i].PayMode);
        //                    //    if (accCode == 0)
        //                    //    {
        //                    //        return new ErrorVM() { field = "", index = 0, description = "ledger mapping is not done , please map the ledger in accounts posting setup" };
        //                    //    }
        //                    //    voucherTrans.acc_code = accCode;

        //                    //    voucherTrans.txt_seq_no = db.KSTS_ACC_SEQ_NOS.Where(kasn => kasn.obj_id == "03").FirstOrDefault().nextno;

        //                    //    InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                    //    if (errorMessage != "")
        //                    //    {
        //                    //        return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                    //    }
        //                    //    Common.UpdateAccountSeqenceNumber(db, "20");
        //                    //    #endregion
        //                    //    break;
        //                    //default:
        //                    //    #region Other Pay mode Payments
        //                    //    voucherTrans.acc_type = "O";
        //                    //    voucherTrans.acc_code_master = 0;
        //                    //    voucherTrans.voucher_no = db.KSTS_ACC_SEQ_NOS.Where(kasn => kasn.obj_id == "20").FirstOrDefault().nextno;
        //                    //    voucherTrans.voucher_seq_no = 1;
        //                    //    voucherTrans.cr_amount = Amount;
        //                    //    voucherTrans.dr_amount = 0;
        //                    //    voucherTrans.narration = payMode + " Adjusted";
        //                    //    FinalAmount = Amount - cgstAmount - sgstAmount - igstAmount;

        //                    //    if (string.Compare(vType, "BREC") == 0)
        //                    //    {
        //                    //        voucherTrans.cr_amount = Amount;
        //                    //        InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                    //        if (errorMessage != "")
        //                    //        {
        //                    //            return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                    //        }
        //                    //    }
        //                    //    else
        //                    //    {
        //                    //        for (int k = 0; i < 4; i++)
        //                    //        {
        //                    //            if (k == 0)
        //                    //            {
        //                    //                voucherTrans.cr_amount = FinalAmount;
        //                    //            }

        //                    //            else if (k == 1 && sgstAmount > 0)
        //                    //            {
        //                    //                voucherTrans.acc_code = sgstAccountCode;//CGlobals.GetAccountCode("T", "S", taxid.ToString());
        //                    //                voucherTrans.cr_amount = sgstAmount;
        //                    //            }

        //                    //            else if (k == 2 && cgstAmount > 0)
        //                    //            {
        //                    //                voucherTrans.acc_code = cgstAccountCode;
        //                    //                voucherTrans.cr_amount = cgstAmount;
        //                    //            }

        //                    //            else if (k == 3 && igstAmount > 0)
        //                    //            {
        //                    //                voucherTrans.acc_code = igstAccountCode;
        //                    //                voucherTrans.cr_amount = igstAmount;
        //                    //            }
        //                    //            else
        //                    //            {
        //                    //                voucherTrans.cr_amount = 0;
        //                    //            }

        //                    //            voucherTrans.dr_amount = 0;

        //                    //            if (voucherTrans.cr_amount > 0)
        //                    //            {
        //                    //                InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                    //                if (errorMessage != "")
        //                    //                {
        //                    //                    return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                    //                }
        //                    //            }
        //                    //        }
        //                    //    }
        //                    //    voucherTrans.voucher_seq_no = 2;
        //                    //    voucherTrans.cr_amount = 0;
        //                    //    voucherTrans.dr_amount = Amount;

        //                    //    accCode = GetAccountPostingSetup("PM", payMode);
        //                    //    if (accCode == 0)
        //                    //    {
        //                    //        return new ErrorVM() { field = "", index = 0, description = "ledger mapping is not done , please map the ledger in accounts posting setup" };
        //                    //    }
        //                    //    voucherTrans.acc_code = accCode;

        //                    //    InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                    //    if (errorMessage != "")
        //                    //    {
        //                    //        return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                    //    }
        //                    //    Common.UpdateAccountSeqenceNumber(db, "20");
        //                    //    #endregion
        //                    //    break;
        //            }
        //            // Discount is based on Customers So code is commented. Check with Accouts and un-comment if necessary.
        //            #region Discount
        //            //if (payments[i].AddDisc > 0 && string.Compare(vType, "BREC") == 0)
        //            //{
        //            //    voucherTrans.acc_type = "O";
        //            //    voucherTrans.acc_code_master = 0;
        //            //    voucherTrans.voucher_no = db.KSTS_ACC_SEQ_NOS.Where(kasn => kasn.obj_id == "20").FirstOrDefault().nextno;
        //            //    voucherTrans.voucher_seq_no = 5;
        //            //    voucherTrans.acc_code = 8;
        //            //    voucherTrans.cr_amount = Convert.ToDecimal(payments[i].AddDisc);
        //            //    voucherTrans.dr_amount = 0;
        //            //    voucherTrans.chq_no = payments[i].ChequeNo;
        //            //    voucherTrans.chq_date = Convert.ToDateTime(payments[i].ChequeDate);
        //            //    voucherTrans.narration = narrationPrefix + " Additional Discount given";

        //            //    voucherTrans.txt_seq_no = db.KSTS_ACC_SEQ_NOS.Where(kasn => kasn.obj_id == "03").FirstOrDefault().nextno;

        //            //    InsertVoucherTransactions(out errorMessage, voucherTrans);
        //            //    if (errorMessage != "")
        //            //    {
        //            //        return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //            //    }
        //            //    Common.UpdateAccountSeqenceNumber(db, "03");

        //            //    voucherTrans.voucher_seq_no = 6;
        //            //    voucherTrans.cr_amount = 0;
        //            //    voucherTrans.dr_amount = payments[k].Add_disc;
        //            //    voucherTrans.acc_code = 50;
        //            //    voucherTrans.txt_seq_no = db.KSTS_ACC_SEQ_NOS.Where(kasn => kasn.obj_id == "03").FirstOrDefault().nextno;

        //            //    InsertVoucherTransactions(out errorMessage, voucherTrans);
        //            //    if (errorMessage != "")
        //            //    {
        //            //        return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //            //    }
        //            //    Common.UpdateAccountSeqenceNumber(db, "20");
        //            //}
        //            #endregion
        //        }
        //    }
        //    return new ErrorVM();
        //}

        //private ErrorVM OrderAccountPostingWithProedure(int OrderNo, int ReceiptNo)
        //{
        //    int retValue = 0;
        //    try {
        //        retValue = db.usp_CreateAccountVoucher(OrderNo, ReceiptNo, Common.BranchCode, Common.CompanyCode);
        //        if (retValue < 0) {
        //            return new ErrorVM() { description = "Error Occurred while Updating Accounts.", field = "Account Update", index = 0 };
        //        }
        //    }
        //    catch (Exception excp) {
        //        return new ErrorVM() { description = excp.Message, field = "Account Update", index = 0 };
        //    }
        //    return null;
        //}

        //private ErrorVM OrderClosureAccountPosting(int OrderNo)
        //{
        //    int retValue = 0;
        //    try {
        //        retValue = db.usp_CreateAccountVoucherForOrderClosure(OrderNo, Common.BranchCode, Common.CompanyCode);
        //        if (retValue < 0) {
        //            return new ErrorVM() { description = "Error Occurred while Updating Accounts.", field = "Account Update", index = 0 };
        //        }
        //    }
        //    catch (Exception excp) {
        //        return new ErrorVM() { description = excp.Message, field = "Account Update", index = 0 };
        //    }
        //    return null;
        //}

        //private bool InsertVoucherTransactions(out string errorMessage, KSTU_ACC_VOUCHER_TRANSACTIONS voucherTrans)
        //{
        //    errorMessage = "";
        //    try {
        //        voucherTrans.txt_seq_no = db.KSTS_ACC_SEQ_NOS.Where(kasn => kasn.obj_id == "03").FirstOrDefault().nextno;
        //        if (voucherTrans.txt_seq_no < 1) {
        //            return false;
        //        }
        //        //voucherTrans.obj_id = Common.GetNewGUID();
        //        voucherTrans.UniqRowID = Guid.NewGuid();
        //        db.KSTU_ACC_VOUCHER_TRANSACTIONS.Add(voucherTrans);
        //        Common.UpdateAccountSeqenceNumber(db, "03");
        //    }
        //    catch (Exception ex) {
        //        errorMessage = "Failed to update accounts";
        //        throw ex;
        //    }
        //    return true;
        //}

        //private int GetGSTAccountCodePayable(string HSN, decimal GSTPercent, int registered, string componentCode)
        //{
        //    GSTPostingSetup gstPostingSetup = db.GSTPostingSetups.Where(gst => gst.GSTGroupCode == HSN
        //                               && gst.GSTPercent == GSTPercent
        //                               && gst.IsRegistered == (registered == 1 ? true : false)
        //                               && gst.GSTComponentCode == componentCode).FirstOrDefault();
        //    int retValue = gstPostingSetup == null ? 0 : Convert.ToInt32(gstPostingSetup.PayableAccount);
        //    return retValue;
        //}

        //private int GetAccountCode(string type, string AccTransType, string RefCode)
        //{
        //    string query = string.Format(" select dbo.[ufn_GetAccountCode] ('{0}','{1}','{2}','{3}','{4}')"
        //        , type, AccTransType, RefCode, Common.CompanyCode, Common.BranchCode);
        //    int accCode = Convert.ToInt32(Common.ExecuteQuery(query).Rows[0][0]);
        //    return accCode;
        //}

        //private int GetConfigurationValue(string objID)
        //{
        //    return Convert.ToInt32(db.APP_CONFIG_TABLE.Where(act => act.obj_id == objID).FirstOrDefault().value);
        //}

        //private int GetMagnaAccountCode(int objID)
        //{
        //    return Convert.ToInt32(db.KSTS_ACC_CODE_MASTER.Where(kacm => kacm.obj_id == objID && kacm.company_code == Common.CompanyCode && kacm.branch_code == Common.BranchCode).FirstOrDefault().acc_code);
        //}

        //private int GetAccountPostingSetup(string gsCode, string transType)
        //{
        //    return db.KSTS_ACC_POSTING_SETUP.Where(kaps => kaps.gs_code == gsCode && kaps.trans_type == transType && kaps.company_code == Common.CompanyCode && kaps.branch_code == Common.BranchCode).FirstOrDefault().acc_code;
        //}

        //private string GetDefaultCurrencyCode()
        //{
        //    return db.KSTU_COMPANY_MASTER.Where(kcm => kcm.company_code == Common.CompanyCode && kcm.branch_code == Common.BranchCode).FirstOrDefault().default_currency_code;
        //}

        //private KSTU_ACC_VOUCHER_TRANSACTIONS GetAccountVourcherTranObject(string vType, int finPeriod, int finyear, DateTime voucherDate, string receiptNo, int accCode, string partyName)
        //{
        //    KSTU_ACC_VOUCHER_TRANSACTIONS voucherTrans = new KSTU_ACC_VOUCHER_TRANSACTIONS();
        //    voucherTrans.trans_type = vType;
        //    voucherTrans.branch_code = Common.BranchCode;
        //    voucherTrans.company_code = Common.CompanyCode;
        //    voucherTrans.contra_seq_no = 0;
        //    voucherTrans.fin_period = finPeriod;
        //    voucherTrans.fin_year = finyear;
        //    voucherTrans.reconsile_date = voucherDate;
        //    voucherTrans.reconsile_flag = "N";
        //    voucherTrans.voucher_date = voucherDate;
        //    voucherTrans.cflag = "N";
        //    voucherTrans.receipt_no = receiptNo;
        //    voucherTrans.chq_no = string.Empty;
        //    voucherTrans.chq_date = voucherDate == DateTime.MinValue ? DateTime.Now : voucherDate;
        //    voucherTrans.acc_code = accCode;
        //    voucherTrans.party_name = partyName;
        //    voucherTrans.voucher_type = "CR";
        //    voucherTrans.inserted_on = voucherDate;
        //    voucherTrans.UpdateOn = voucherDate;
        //    voucherTrans.approved_date = voucherDate;
        //    voucherTrans.Verified_Date = voucherDate;
        //    voucherTrans.Authorized_Date = voucherDate;
        //    return voucherTrans;
        //}

        //private KSTU_ACC_VOUCHER_TRANSACTIONS GetCashAccountVourcherTranObject(KSTU_ACC_VOUCHER_TRANSACTIONS kavt, string currencyType, string Narration)
        //{
        //    kavt.acc_type = "C";
        //    kavt.acc_code_master = 1;
        //    kavt.voucher_no = Convert.ToInt32(db.KSTU_ACC_FY_MASTER.FirstOrDefault().fin_year.ToString().Remove(0, 1) + db.KSTS_ACC_VOUCHER_SEQ_NOS.Where(kavsn => kavsn.obj_id == kavt.acc_code_master.ToString()).FirstOrDefault().nextno);
        //    kavt.voucher_seq_no = 1;
        //    kavt.currency_type = currencyType;
        //    kavt.narration = Narration;
        //    return kavt;
        //}

        //private KSTU_ACC_VOUCHER_TRANSACTIONS GetChequeOrDDAccountVourcherTranObject(KSTU_ACC_VOUCHER_TRANSACTIONS kavt,
        //                                            decimal amount,
        //                                            string chqNo,
        //                                            DateTime chqDate,
        //                                            string narrationPrefix)
        //{
        //    kavt.acc_type = "O";
        //    kavt.acc_code_master = 0;
        //    kavt.voucher_no = Convert.ToInt32(db.KSTU_ACC_FY_MASTER.FirstOrDefault().fin_year.ToString().Remove(0, 1) + db.KSTS_ACC_SEQ_NOS.Where(kasn => kasn.obj_id == "20").FirstOrDefault().nextno);
        //    kavt.voucher_seq_no = 1;
        //    kavt.cr_amount = amount;
        //    kavt.dr_amount = 0;
        //    kavt.chq_no = chqNo == null ? "" : chqNo;
        //    kavt.chq_date = chqDate == DateTime.MinValue ? DateTime.Now : Convert.ToDateTime(chqDate);
        //    kavt.narration = narrationPrefix + " (Cheque/DD)";
        //    return kavt;
        //}
        //private ErrorVM OrderAccountPostingNew(List<PaymentVM> payments, string customer)
        //{
        //    string narrationPrefix = "Order Advance", vType = "OREC", party_name = customer, errorMessage, memberShip;
        //    decimal taxPer = 0, percentage = 0, sgstAmount = 0, igstAmount = 0, cgstAmount = 0, cessAmount = 0, cessPercent = 0;
        //    int accountCode = 7, sgstAccountCode = 0, igstAccountCode = 0, cgstAccountCode = 0, accCode = 0, cessAccountCode = 0;
        //    DateTime dtVoucherDate = Common.GetDateTime();
        //    int FinYear = db.KSTU_ACC_FY_MASTER.FirstOrDefault().fin_year;
        //    int FinPeriod = Convert.ToInt32(db.KSTU_ACC_FY_MASTER.FirstOrDefault().fin_period);
        //    if (payments != null && payments.Count > 0) {
        //        for (int i = 0; i < payments.Count; i++) {
        //            string voucherOrderReceiptNo = string.Empty;
        //            decimal amount, gstPer, gstAmount, finalGSTAmount;
        //            int taxID;

        //            string payMode = payments[i].PayMode;
        //            int receiptNo = payments[i].ReceiptNo;
        //            string branch = payMode.Substring(0, 1);
        //            int orderNo = payments[i].SeriesNo;
        //            decimal sgstPer = Convert.ToDecimal(payments[i].SGSTPercent);
        //            decimal cgstPer = Convert.ToDecimal(payments[i].CGSTPercent);
        //            decimal igstPer = Convert.ToDecimal(payments[i].IGSTPercent);

        //            string gstGroupCode = payments[i].GSTGroupCode.ToString();
        //            sgstAccountCode = GetGSTPostingSetup(gstGroupCode, sgstPer, true, "SGST");
        //            cgstAccountCode = GetGSTPostingSetup(gstGroupCode, sgstPer, true, "CGST");
        //            igstAccountCode = GetGSTPostingSetup(gstGroupCode, sgstPer, true, "IGST");

        //            voucherOrderReceiptNo = orderNo.ToString();
        //            voucherOrderReceiptNo = receiptNo > 0 ? orderNo + "," + receiptNo : "";
        //            amount = Convert.ToDecimal(payments[i].PayAmount);

        //            if (string.IsNullOrEmpty(payments[i].CurrencyType)) {
        //                payments[i].CurrencyType = db.KSTU_COMPANY_MASTER.FirstOrDefault().default_currency_code;
        //            }

        //            KTTU_ORDER_RECEIPT_DETAILS kord = db.KTTU_ORDER_RECEIPT_DETAILS.Where(ord => ord.order_no == orderNo
        //              && ord.company_code == Common.CompanyCode
        //              && ord.bracnch_code == Common.BranchCode).Distinct().FirstOrDefault();

        //            taxPer = Convert.ToDecimal(kord != null ? kord.tax_percentage : 0);
        //            taxID = db.KSTS_SALETAX_MASTER.Where(ksm => ksm.tax == taxPer
        //                                                    && ksm.company_code == Common.CompanyCode
        //                                                    && ksm.branch_code == Common.BranchCode).FirstOrDefault().tax_Id;
        //            gstPer = cgstPer + sgstPer + igstPer;
        //            gstAmount = decimal.Round(((amount) - (((amount) * 100) / (100 + gstPer))), 2);
        //            finalGSTAmount = gstAmount / 2;
        //            if (igstPer > 0) {
        //                igstAmount = gstAmount;
        //            }
        //            else {
        //                cgstAmount = finalGSTAmount;
        //                sgstAmount = finalGSTAmount;
        //            }
        //            decimal finalAmout = amount - cgstAmount - sgstAmount - igstAmount;
        //            switch (payMode) {
        //                case "C": //Tested
        //                    #region Cash Payment
        //                    KSTU_ACC_VOUCHER_TRANSACTIONS voucherTransDummy = GetAccountVourcherTranObject(vType, FinPeriod, FinYear, dtVoucherDate, voucherOrderReceiptNo, accCode, party_name);
        //                    KSTU_ACC_VOUCHER_TRANSACTIONS voucherTrans = GetCashAccountVourcherTranObject(voucherTransDummy, payments[i].CurrencyType, narrationPrefix + " (Cash) ");
        //                    string accountCodeMaster = voucherTrans.acc_code_master.ToString();

        //                    voucherTrans.obj_id = Common.GetNewGUID();
        //                    voucherTrans.UniqRowID = Guid.NewGuid();
        //                    voucherTrans.dr_amount = 0;
        //                    // Inserting Amount
        //                    voucherTrans.cr_amount = finalAmout;

        //                    if (voucherTrans.cr_amount > 0) {
        //                        InsertVoucherTransactions(out errorMessage, voucherTrans);
        //                        if (errorMessage != "") {
        //                            return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                        }
        //                    }

        //                    //Inserting SGST
        //                    if (sgstAmount > 0) {
        //                        KSTU_ACC_VOUCHER_TRANSACTIONS voucherTransSGST = GetCashAccountVourcherTranObject(voucherTransDummy, payments[i].CurrencyType, narrationPrefix + " (Cash) ");
        //                        voucherTransSGST.acc_code = sgstAccountCode;
        //                        voucherTransSGST.cr_amount = sgstAmount;
        //                        if (voucherTrans.cr_amount > 0) {
        //                            InsertVoucherTransactions(out errorMessage, voucherTransSGST);
        //                            if (errorMessage != "") {
        //                                return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                            }
        //                        }
        //                    }

        //                    //Inserting CGST
        //                    if (cgstAmount > 0) {
        //                        KSTU_ACC_VOUCHER_TRANSACTIONS voucherTransCGST = GetCashAccountVourcherTranObject(voucherTransDummy, payments[i].CurrencyType, narrationPrefix + " (Cash) ");
        //                        voucherTransCGST.acc_code = cgstAccountCode;
        //                        voucherTransCGST.cr_amount = cgstAmount;
        //                        if (voucherTrans.cr_amount > 0) {
        //                            InsertVoucherTransactions(out errorMessage, voucherTransCGST);
        //                            if (errorMessage != "") {
        //                                return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                            }
        //                        }
        //                    }

        //                    //Inserting IGST
        //                    if (igstAmount > 0) {
        //                        KSTU_ACC_VOUCHER_TRANSACTIONS voucherTransIGST = GetCashAccountVourcherTranObject(voucherTransDummy, payments[i].CurrencyType, narrationPrefix + " (Cash) ");
        //                        voucherTransIGST.acc_code = igstAccountCode;
        //                        voucherTransIGST.cr_amount = igstAmount;
        //                        if (voucherTrans.cr_amount > 0) {
        //                            InsertVoucherTransactions(out errorMessage, voucherTransIGST);
        //                            if (errorMessage != "") {
        //                                return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                            }
        //                        }
        //                    }
        //                    Common.UpdateAccountVourcherSeqenceNumber(db, accountCodeMaster);
        //                    #endregion
        //                    break;
        //                case "Q": //Tested
        //                case "D": // Tested
        //                    #region Cheque or DD
        //                    KSTU_ACC_VOUCHER_TRANSACTIONS chqDDTransDummy = GetAccountVourcherTranObject(vType, FinPeriod, FinYear, dtVoucherDate, voucherOrderReceiptNo, accCode, party_name);
        //                    KSTU_ACC_VOUCHER_TRANSACTIONS chqDDVouchTrans = GetChequeOrDDAccountVourcherTranObject(chqDDTransDummy, amount, payments[i].ChequeNo, Convert.ToDateTime(payments[i].ChequeDate), narrationPrefix);

        //                    chqDDVouchTrans.obj_id = Common.GetNewGUID();
        //                    chqDDVouchTrans.dr_amount = 0;
        //                    // Inserting Amount
        //                    chqDDVouchTrans.cr_amount = finalAmout;
        //                    if (chqDDVouchTrans.cr_amount > 0) {
        //                        InsertVoucherTransactions(out errorMessage, chqDDVouchTrans);
        //                        if (errorMessage != "") {
        //                            return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                        }
        //                    }

        //                    //Inserting SGST
        //                    if (sgstAmount > 0) {
        //                        KSTU_ACC_VOUCHER_TRANSACTIONS chqDDVouchTransSGST = GetCashAccountVourcherTranObject(chqDDTransDummy, payments[i].CurrencyType, narrationPrefix + " (Cash) ");
        //                        chqDDVouchTransSGST.acc_code = sgstAccountCode;
        //                        chqDDVouchTransSGST.cr_amount = sgstAmount;
        //                        chqDDVouchTransSGST.obj_id = chqDDVouchTrans.obj_id;
        //                        if (chqDDVouchTransSGST.cr_amount > 0) {
        //                            InsertVoucherTransactions(out errorMessage, chqDDVouchTransSGST);
        //                            if (errorMessage != "") {
        //                                return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                            }
        //                        }
        //                    }

        //                    //Inserting CGST
        //                    if (cgstAmount > 0) {
        //                        KSTU_ACC_VOUCHER_TRANSACTIONS chqDDVouchTransCGST = GetCashAccountVourcherTranObject(chqDDTransDummy, payments[i].CurrencyType, narrationPrefix + " (Cash) ");
        //                        chqDDVouchTransCGST.acc_code = cgstAccountCode;
        //                        chqDDVouchTransCGST.cr_amount = cgstAmount;
        //                        chqDDVouchTransCGST.obj_id = chqDDVouchTrans.obj_id;
        //                        if (chqDDVouchTransCGST.cr_amount > 0) {
        //                            InsertVoucherTransactions(out errorMessage, chqDDVouchTransCGST);
        //                            if (errorMessage != "") {
        //                                return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                            }
        //                        }
        //                    }

        //                    //Inserting IGST
        //                    if (igstAmount > 0) {
        //                        KSTU_ACC_VOUCHER_TRANSACTIONS chqDDVouchTransIGST = GetCashAccountVourcherTranObject(chqDDTransDummy, payments[i].CurrencyType, narrationPrefix + " (Cash) ");
        //                        chqDDVouchTransIGST.acc_code = igstAccountCode;
        //                        chqDDVouchTransIGST.cr_amount = igstAmount;
        //                        chqDDVouchTransIGST.obj_id = chqDDVouchTrans.obj_id;
        //                        if (chqDDVouchTransIGST.cr_amount > 0) {
        //                            InsertVoucherTransactions(out errorMessage, chqDDVouchTransIGST);
        //                            if (errorMessage != "") {
        //                                return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                            }
        //                        }
        //                    }

        //                    KSTU_ACC_VOUCHER_TRANSACTIONS chqDDVouchTransDebit = GetChequeOrDDAccountVourcherTranObject(chqDDTransDummy, amount, payments[i].ChequeNo, Convert.ToDateTime(payments[i].ChequeDate), narrationPrefix);
        //                    chqDDVouchTransDebit.voucher_seq_no = 2;
        //                    chqDDVouchTransDebit.cr_amount = 0;
        //                    chqDDVouchTransDebit.dr_amount = amount;
        //                    chqDDVouchTransDebit.obj_id = chqDDVouchTrans.obj_id;

        //                    accCode = GetAccountPostingSetup("PM", "Q");
        //                    if (accCode == 0) {
        //                        return new ErrorVM() { field = "", index = 0, description = "ledger mapping is not done , please map the ledger in accounts posting setup" };
        //                    }
        //                    chqDDVouchTransDebit.acc_code = accCode;
        //                    InsertVoucherTransactions(out errorMessage, chqDDVouchTransDebit);
        //                    if (errorMessage != "") {
        //                        return new ErrorVM() { field = "", index = 0, description = errorMessage };
        //                    }
        //                    Common.UpdateAccountSeqenceNumber(db, "20");
        //                    break;
        //                    #endregion
        //            }
        //        }
        //    }
        //    return new ErrorVM();
        //}

        //private int GetGSTPostingSetup(string gstGroupCode, decimal gstPer, bool IsRegisterd, string gstComponent)
        //{
        //    int payableAccount = 0;
        //    GSTPostingSetup gstPostSetupSGSTAccountCode = db.GSTPostingSetups.Where(gst => gst.GSTGroupCode == gstGroupCode && gst.GSTPercent == gstPer && gst.IsRegistered == true && gst.GSTComponentCode == gstComponent).FirstOrDefault();
        //    payableAccount = gstPostSetupSGSTAccountCode == null ? 0 : Convert.ToInt32(gstPostSetupSGSTAccountCode.PayableAccount);
        //    return payableAccount;
        //}

        //private ErrorVM CreateAccountVoucherPaidAmountSGSTAndCGSTAndIGST(KSTU_ACC_VOUCHER_TRANSACTIONS vourcherTran, decimal drAmount, decimal crAmount)
        //{
        //    return new ErrorVM();
        //}

        //private decimal GetTillNowPaidOrderAmount(int CustID)
        //{
        //    int finYear = db.KSTU_ACC_FY_MASTER.FirstOrDefault().fin_year;
        //    var tillNowPaidAmount = (from komt in db.KTTU_ORDER_MASTER
        //                             join kpdt in db.KTTU_PAYMENT_DETAILS on komt.order_no equals kpdt.series_no
        //                             where komt.cflag == "N"
        //                             && komt.closed_flag == "N"
        //                             && komt.Cust_Id == CustID
        //                             && kpdt.cflag == "N"
        //                             && kpdt.pay_mode == "C"
        //                             && kpdt.fin_year == finYear
        //                             && kpdt.trans_type == "O"
        //                             && kpdt.company_code == Common.CompanyCode
        //                             && kpdt.branch_code == Common.BranchCode
        //                             group kpdt by 1 into g
        //                             select new { Amount = g.Sum(x => x.pay_amt) });
        //    return Convert.ToDecimal(tillNowPaidAmount.FirstOrDefault() == null ? 0 : tillNowPaidAmount.FirstOrDefault().Amount);
        //}
        #endregion
    }
}
