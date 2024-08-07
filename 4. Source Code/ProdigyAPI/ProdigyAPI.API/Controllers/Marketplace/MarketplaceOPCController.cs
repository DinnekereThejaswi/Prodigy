using ProdigyAPI.BL.BusinessLayer.Common;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Marketplace;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.Handlers;
using ProdigyAPI.Marketplace.Models.BJEComm;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace ProdigyAPI.Controllers
{
    /// <summary>
    /// Provides API for Marketplace Order Processing System (OPS). OPS includes
    /// 1. Item picking, 2. Package creation, 3. Invoice generation, 4. Ship-label generation and 5. Shipping
    /// </summary>
    [Authorize]
    [RoutePrefix("api/marketplace-ops")]
    public class MarketplaceOPCController : SIBaseApiController<OrderMasterVM>
    {
        BL.BusinessLayer.Marketplace.OnlineMarketplaceBL marketplaceBL;
        //MarketplaceBL marketplaceBL;
        public MarketplaceOPCController()
        {
            marketplaceBL = new BL.BusinessLayer.Marketplace.OnlineMarketplaceBL();
            //marketplaceBL = new MarketplaceBL();
        }
        #region Marketplace Integration

        /// <summary>
        /// Gets the market-place list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("marketplaces")]
        [ResponseType(typeof(List<MarketPlaceMaster>))]
        public IHttpActionResult GetMarketplaces()
        {
            return Ok(marketplaceBL.GetMarketPlaces());
        }

        /// <summary>
        /// Gets the list of order states. The states can be: OPEN, UNDERPROCESS, PICKED, PACKED, READY, SHIPPED, CANCELLED, ALL
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("order-stages")]
        [ResponseType(typeof(string[]))]
        public IHttpActionResult GetOrderStates()
        {
            return Ok(marketplaceBL.GetOrderStates());
        }

        /// <summary>
        /// Gets the count of pending orders which are not processed
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("pending-order-count/{companyCode}/{branchCode}")]
        [Route("pending-order-count")]
        public IHttpActionResult GetOrderCount(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = marketplaceBL.GetOrderCount(companyCode, branchCode, out error);
            if (data != null) {
                return Ok(data);
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        #region Received Orders

        /// <summary>
        /// Gets All Online Orders which are received at the store
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-online-orders")]
        public IHttpActionResult GetAllReceivedOrders([FromBody] ReceiveOrderVM order)
        {
            ErrorVM error = new ErrorVM();
            var data = marketplaceBL.GetAllReceivedOrders(order.CompanyCode, order.BranchCode, order.FromDate, order.ToDate, order.MarketplaceID, order.Type, out error);
            if (error.ErrorStatusCode == HttpStatusCode.OK) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Creates pick-list
        /// </summary>
        /// <param name="order">List of order items</param>
        /// <returns></returns>
        [HttpPost]
        [Route("create-picklist")]
        public IHttpActionResult SaveItemsToPickList([FromBody] List<ReceivedOrderForGridVM> order)
        {
            ErrorVM error = new ErrorVM();
            string user = base.GetUserId();
            int asingnmentNo = marketplaceBL.SaveItemsToPickList(order, user, out error);
            if (error.ErrorStatusCode == HttpStatusCode.OK) {
                return Ok(new { AssignmentNo = asingnmentNo });
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get Pick List Item for Print for the given assignment number
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="assignmentNo">Assign Number</param>
        /// <returns>Print data</returns>
        [HttpGet]
        [Route("picklist/{companyCode}/{branchCode}/{assignmentNo}")]
        [Route("picklist")]
        public IHttpActionResult GetPickList(string companyCode, string branchCode, int assignmentNo)
        {
            ErrorVM error = new ErrorVM();
            var data = marketplaceBL.PrintOrderItemPickList(companyCode, branchCode, assignmentNo, out error);
            if (error.ErrorStatusCode == HttpStatusCode.OK) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }
        #endregion

        #region Item Picking

        /// <summary>
        /// Get Pick List No. The orderStage must either be pick, re-pick, pack or invoice.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="orderStage">The orderStage must either be pick, re-pick pack or invoice</param>
        /// <returns></returns>
        [HttpGet]
        [Route("picklist-number/{companyCode}/{branchCode}/{orderStage}")]
        [Route("picklist-number")]
        public IHttpActionResult GetPickListNumbers(string companyCode, string branchCode, string orderStage)
        {
            ErrorVM error = null;
            List<DocumentInfoVM> documentInfo;
            var success = marketplaceBL.GetPickListNo(companyCode, branchCode, orderStage, out documentInfo, out error);
            if (success) {
                return Ok(documentInfo);
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }
        /// <summary>
        /// Get Picked List Details by passing assignment Number.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="assignmentNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("picklist-detail/{companyCode}/{branchCode}/{assignmentNo}")]
        [Route("picklist-detail")]
        public IHttpActionResult GetPickListDetails(string companyCode, string branchCode, int assignmentNo)
        {
            ErrorVM error = new ErrorVM();
            var data = marketplaceBL.GetPickListDetailByAssignmentNo(companyCode, branchCode, assignmentNo, out error);
            if (error.ErrorStatusCode == HttpStatusCode.OK) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        ///// <summary>
        ///// Update pick list details by barcode number
        ///// </summary>
        ///// <param name="pickedItems"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("update-picklist-detail")]
        //public IHttpActionResult UpdatePickListDetails(List<OrderItemPickListVM> pickedItems)
        //{
        //    ErrorVM error = new ErrorVM();
        //    var data = marketplaceBL.UpdatePickedItem(pickedItems, out error);
        //    if (error.ErrorStatusCode == HttpStatusCode.OK) {
        //        return Ok(data);
        //    }
        //    else {
        //        return Content(error.ErrorStatusCode, error);
        //    }
        //}

        /// <summary>
        /// Maps a barcode number with a picklist line after validation
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="assignmentNo"></param>
        /// <param name="barcodeNo"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("assign-barcode/{companyCode}/{branchCode}/{assignmentNo}/{barcodeNo}")]
        [Route("assign-barcode")]
        public IHttpActionResult AssignBarcode(string companyCode, string branchCode, int assignmentNo, string barcodeNo)
        {
            var userId = base.GetUserId();
            ErrorVM error = new ErrorVM();
            List<OrderItemPickListVM> orderPicklist = new List<OrderItemPickListVM>();
            var success = marketplaceBL.AssignBarcodeToSKU(companyCode, branchCode, assignmentNo, barcodeNo, userId, out orderPicklist, out error);
            if (success) {
                return Ok(orderPicklist);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Removes barcode mapping. This is the reverse of assign-barcode API call.
        /// </summary>
        /// <param name="orderItemPickListVM"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("un-assign-barcode")]
        public IHttpActionResult UnAssignBarcode(OrderItemPickListVM orderItemPickListVM)
        {
            var userId = base.GetUserId();
            ErrorVM error = new ErrorVM();
            var success = marketplaceBL.UnAssignBarcodeFromSKU(orderItemPickListVM, userId, out error);
            if (success) {
                return Ok();
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }


        #endregion

        #region Item Packing
        /// <summary>
        /// Get Items for Packing. Pass null if all the order lines are required or else, pass the selected assignment Number.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="assignmentNo">The assignment number in question</param>
        /// <returns></returns>
        [HttpGet]
        [Route("orders-for-packing/{companyCode}/{branchCode}/{assignmentNo}")]
        [Route("orders-for-packing")]
        public IHttpActionResult GetItemPakingDetails(string companyCode, string branchCode, int? assignmentNo = null)
        {
            ErrorVM error = new ErrorVM();
            var data = marketplaceBL.GetItemDetailsForPacking(companyCode, branchCode, out error, assignmentNo);
            if (error.ErrorStatusCode == HttpStatusCode.OK) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get packing master
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("package-master/{companyCode}/{branchCode}")]
        [Route("package-master")]
        public IHttpActionResult GetPackageMaster(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = marketplaceBL.GetPackageDetails(companyCode, branchCode, out error);
            if (error.ErrorStatusCode == HttpStatusCode.OK) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get Packing Details by sending Package Code.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="packageCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("package-master-attributes/{companyCode}/{branchCode}/{packageCode}")]
        [Route("package-master-attributes")]
        public IHttpActionResult GetPackageDetails(string companyCode, string branchCode, string packageCode)
        {
            ErrorVM error = new ErrorVM();
            var data = marketplaceBL.GetPackingDetails(companyCode, branchCode, packageCode, out error);
            if (data != null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Creates a Package
        /// </summary>
        /// <param name="packing"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create-package")]
        public IHttpActionResult CreatePackage(PackingItemVM packing)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            ErrorVM error = new ErrorVM();
            var success = marketplaceBL.CreatePackage(packing, out error);
            if (success) {
                return Ok();
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Gets the list of orders for invoicing. Use this API in two ways:
        /// 1. Either query the data without assignment number (pass null or zero for assignmentNo)
        /// 2. Or query the data with assignment number. In this case fromShipDate and toShipDate will be ignored. But a value must be passed by the caller.
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="fromShipDate">From shipping date</param>
        /// <param name="toShipDate">To ShippingDate</param>
        /// <param name="assignmentNo">Assignment number in question.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("orders-tobe-invoiced/{companyCode}/{branchCode}/{fromShipDate}/{toShipDate}/{assignmentNo}")]
        [Route("orders-tobe-invoiced")]
        [ResponseType(typeof(List<MarketplaceOrdersToBeProcessed>))]
        public IHttpActionResult GetOrdersToBeInvoiced(string companyCode, string branchCode, DateTime fromShipDate, DateTime toShipDate, int? assignmentNo = null)
        {
            ErrorVM error = null;
            List<MarketplaceOrdersToBeProcessed> ordersTobeProcessed = null;
            var success = marketplaceBL.GetOrdersToBeInvoiced(companyCode, branchCode, fromShipDate, toShipDate, out ordersTobeProcessed, out error, assignmentNo);
            if (success) {
                return Ok(ordersTobeProcessed);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Generates invoice
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="orderToBeInvoiced">Order line to be invoiced</param>
        /// <returns></returns>
        [HttpPost]
        [Route("generate-invoice")]
        [ResponseType(typeof(DocumentInfoOutputVM))]
        public IHttpActionResult GenerateInvoice(string companyCode, string branchCode, [FromBody] MarketplaceOrdersToBeProcessed orderToBeInvoiced)
        {
            ErrorVM error = new ErrorVM();
            DocumentInfoOutputVM invoiceOutputVM = null;
            var success = marketplaceBL.GenerateInvoice(companyCode, branchCode, orderToBeInvoiced, out invoiceOutputVM, out error);
            if (success) {
                return Ok(invoiceOutputVM);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Generates Ship-label
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="orderToBeInvoiced">Order line to be invoiced</param>
        /// <returns></returns>
        [HttpPost]
        [Route("generate-shiplabel")]
        [ResponseType(typeof(DocumentInfoOutputVM))]
        public IHttpActionResult GenerateShiplabel(string companyCode, string branchCode, MarketplaceOrdersToBeProcessed orderToBeInvoiced)
        {
            ErrorVM error = new ErrorVM();
            DocumentInfoOutputVM invoiceOutputVM = null;
            string userID = base.GetUserId();
            var success = marketplaceBL.GenerateShiplabel(companyCode, branchCode, userID, orderToBeInvoiced, out invoiceOutputVM, out error);
            if (success) {
                return Ok(invoiceOutputVM);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }
        
        /// <summary>
        /// Gets the list of orders which are to be shipped
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="marketplaceCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("orders-tobe-shipped/{companyCode}/{branchCode}/{marketplaceCode}")]
        [Route("orders-tobe-shipped")]
        [ResponseType(typeof(List<MarketplaceOrdersToBeProcessed>))]
        public IHttpActionResult GetOrdersToBeShipped(string companyCode, string branchCode, int marketplaceCode)
        {
            ErrorVM error = null;
            List<MarketplaceOrdersToBeProcessed> ordersTobeProcessed = null;
            var success = marketplaceBL.GetOrdersToBeShipped(companyCode, branchCode, marketplaceCode, out ordersTobeProcessed, out error);
            if (success) {
                return Ok(ordersTobeProcessed);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }
        
        /// <summary>
        /// Ships the given orders.
        /// </summary>
        /// <param name="shipOrderInput"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ship")]
        public IHttpActionResult ShipOrder(ShipOrderInput shipOrderInput)
        {
            List<ErrorVM> errorList = new List<ErrorVM>();
            string userID = base.GetUserId();
            var success = marketplaceBL.ShipOrder(shipOrderInput, userID, out errorList);
            if (success) {
                return Ok();
            }
            else {
                string errorDescription = string.Empty;
                foreach (var err in errorList)
                    errorDescription += err.description + Environment.NewLine;
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = errorDescription });
            }
        }

        #endregion

        #endregion

        #region Order Cancellation API
        /// <summary>
        /// Get the list of orders than can be cancelled
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="marketplaceId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("orders-canbe-cancelled/{companyCode}/{branchCode}/{marketplaceId}")]
        [Route("orders-canbe-cancelled")]
        [ResponseType(typeof(List<ListOfValue>))]
        public IHttpActionResult GetOrdersToBeCancelled(string companyCode, string branchCode, int marketplaceId)
        {
            ErrorVM error = null;
            List<ListOfValue> ordersCanBeCancelled = null;
            var success = marketplaceBL.GetOrderListToBeCancelled(companyCode, branchCode, marketplaceId, out ordersCanBeCancelled, out error);
            if (success) {
                return Ok(ordersCanBeCancelled);
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }
        /// <summary>
        /// Cancels the order at marketplace and reciprocates the same to the PoS database
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="marketplaceId"></param>
        /// <param name="orderNo"></param>
        /// <param name="cancelRemarks"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("cancel-order/{companyCode}/{branchCode}/{marketplaceId}/{orderNo}/{cancelRemarks}")]
        [Route("cancel-order")]
        public IHttpActionResult CancelOrder(string companyCode, string branchCode, int marketplaceId, int orderNo, string cancelRemarks)
        {
            string userId = base.GetUserId();
            ErrorVM error = null;
            var success = marketplaceBL.CancelOrder(companyCode, branchCode, marketplaceId, orderNo, userId, cancelRemarks, out error);
            if (success) {
                return Ok();
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }
        #endregion

        #region AWB API
        /// <summary>
        /// Get AWB number for the given order. Please note that only orders for marketplace "BHIMA" would provide you AWB number.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-awb/{companyCode}/{branchCode}/{orderNo}")]
        [Route("get-awb")]
        [ResponseType(typeof(List<DocumentCreationVM>))]
        public IHttpActionResult GetAWBForOrder(string companyCode, string branchCode, int orderNo)
        {
            ErrorVM error = null;
            string awbNo = string.Empty;
            var success = marketplaceBL.GetAWBFromOrderNo(companyCode, branchCode, orderNo, out awbNo, out error);
            if (success) {
                DocumentCreationVM doc = new DocumentCreationVM { DocumentNo = awbNo, Message = "AWB number for the given order number is " + awbNo.ToString() };
                return Ok(doc);
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Gets the pdf print content for the order for which AWB is already created. Please note that only orders for marketplace "BHIMA" would provide you AWB number.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-shiplabel-content/{companyCode}/{branchCode}/{orderNo}")]
        [Route("get-shiplabel-content")]
        [ResponseType(typeof(DocumentInfoOutputVM))]
        public IHttpActionResult GetAWBPrintContent(string companyCode, string branchCode, int orderNo)
        {
            ErrorVM error = null;
            DocumentInfoOutputVM shipLabelContent = null;
            var success = marketplaceBL.GetAWBPrintContent(companyCode, branchCode, orderNo, out shipLabelContent, out error);
            if (success) {
                return Ok(shipLabelContent);
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Registers a pickup. Please ensure that valid date with time is provided in the PickupDate of pickupRegistrationInput object. 
        /// The pickup time shall be between 10:00 hours to 18:30 hours
        /// </summary>
        /// <param name="pickupRegistrationInput"></param>
        /// <returns>Token number is the output</returns>
        [HttpPost]
        [Route("register-pickup")]
        [ResponseType(typeof(List<DocumentCreationVM>))]
        public IHttpActionResult RegisterPickup(PickupRegistrationInputVM pickupRegistrationInput)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            ErrorVM error = null;
            string tokenNo = null;
            var success = marketplaceBL.PickupRegistration(pickupRegistrationInput, out tokenNo, out error);
            if (success) {
                DocumentCreationVM doc = new DocumentCreationVM { DocumentNo = tokenNo.ToString(), Message = "Token number for the given order number is " + tokenNo.ToString() };
                return Ok(doc);
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Cancells the previously registered pickup
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="tokenNo"></param>
        /// <param name="remarks"></param>
        /// <param name="registrationDate"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("cancel-register-pickup")]
        public IHttpActionResult CancelPickupRegistration(string companyCode, string branchCode, int tokenNo, string remarks, DateTime registrationDate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            ErrorVM error = null;
            var success = marketplaceBL.CancelPickupRegistration(companyCode, branchCode, tokenNo, remarks, registrationDate, out error);
            if (success) {
                return Ok();
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Cancells the AWB
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("cancel-awb")]
        public IHttpActionResult CancelAWB(string companyCode, string branchCode, int orderNo)
        {
            ErrorVM error = null;
            string awbNo = string.Empty;
            var success = marketplaceBL.CancelAwb(companyCode, branchCode, orderNo, out error);
            if (success) {
                return Ok();
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }
        #endregion
        /// <summary>
        /// Print sales invoice
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="billNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("print")]
        public IHttpActionResult PrintSalesInvoice(string companyCode, string branchCode, int billNo)
        {
            string errorMessage = string.Empty;
            try {
                string html = new DocumentPrintBL().PrintSalesBillWithHeaderFooter(companyCode, branchCode, billNo, out errorMessage);
                if (html != null) {
                    return Ok(html);
                }
                else {
                    ErrorVM error = new ErrorVM { ErrorStatusCode = HttpStatusCode.BadRequest, description = errorMessage };
                    return Content(HttpStatusCode.BadRequest, error);
                }
            }
            catch (Exception ex) {
                return Content(HttpStatusCode.BadRequest, new ErrorVM().GetErrorDetails(ex));
            }
        }

    }
}
