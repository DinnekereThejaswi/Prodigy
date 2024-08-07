using ProdigyAPI.BL.BusinessLayer.Common;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Marketplace;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.Marketplace;
using ProdigyAPI.Marketplace.Models;
using ProdigyAPI.Marketplace.Models.BJEComm;
using ProdigyAPI.Model.MagnaDb;
using ProdigyAPI.SIGlobals;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;

namespace ProdigyAPI.BL.BusinessLayer.Marketplace
{
    public abstract class OnlineMarketpaceBase
    {
        protected MagnaDbEntities db;
        AmazonMarketplace amazon = new AmazonMarketplace();
        BhimaECommerce bhima = new BhimaECommerce();

        public OnlineMarketpaceBase()
        {
        }
        

        #region Inventory Updation API Methods
        public bool UpdateMarketplaceInventory(string companyCode, string branchCode, int docNo, TransactionsType transType, ActionType actionType, out ErrorVM error)
        {
            error = new ErrorVM();
            // Checking Oniline MarketPlace Updation Enabled for All the marketplaces,
            // TODO: Incase of any other marketplace comes, need to add that method here.
            if (!(DoAmazonUpdate(companyCode, branchCode) || DoBhimaEComUpdate(companyCode, branchCode))) {
                return true;
            }

            try {
                switch (transType) {
                    case TransactionsType.Order:
                        var orderData = (from orderDet in db.KTTU_ORDER_DETAILS
                                         join inverntory in db.KTTU_ONLINE_INVENTARY_DETAILS
                                         on new { Companycode = orderDet.company_code, BranchCode = orderDet.branch_code, BarcodeNo = orderDet.barcode_no }
                                         equals new { Companycode = inverntory.company_code, BranchCode = inverntory.branch_code, BarcodeNo = inverntory.barcode_no }
                                         where orderDet.company_code == companyCode && orderDet.branch_code == branchCode && orderDet.order_no == docNo
                                         group new { orderDet, inverntory } by new { orderDet.barcode_no, inverntory.portal_id } into g
                                         select new
                                         {
                                             BarcodeNo = g.Key.barcode_no,
                                             PortalID = (int)g.Key.portal_id,
                                             Qty = 1
                                         }).ToList();
                        if (orderData != null && orderData.Count() > 0) {
                            foreach (var data in orderData) {
                                int portalID = data.PortalID;
                                switch ((SIGlobals.MarketPlace)portalID) {
                                    case SIGlobals.MarketPlace.Amazon:
                                        if (DoAmazonUpdate(companyCode, branchCode)) {
                                            bool updated = UpdateAmazonMarketplaceInventory(data.BarcodeNo, branchCode, data.Qty, actionType, out error);
                                            if (!updated) {
                                                return false;
                                            }
                                        }
                                        break;
                                    case SIGlobals.MarketPlace.BhimaEcom:
                                        if (DoBhimaEComUpdate(companyCode, branchCode)) {
                                            bool updated = UpdateBhimaEComMarketplaceInventory(companyCode, data.BarcodeNo, branchCode, data.Qty, actionType, out error);
                                            if (!updated) {
                                                return false;
                                            }
                                        }
                                        break;
                                    case SIGlobals.MarketPlace.Instore:
                                        break;
                                }
                            }
                        }
                        break;
                    case TransactionsType.Sales:
                        var salesData = (from salesDet in db.KTTU_SALES_DETAILS
                                         join inverntory in db.KTTU_ONLINE_INVENTARY_DETAILS
                                         on new { Companycode = salesDet.company_code, BranchCode = salesDet.branch_code, BarcodeNo = salesDet.barcode_no }
                                         equals new { Companycode = inverntory.company_code, BranchCode = inverntory.branch_code, BarcodeNo = inverntory.barcode_no }
                                         where salesDet.company_code == companyCode && salesDet.branch_code == branchCode && inverntory.isActive == "Y" && salesDet.bill_no == docNo
                                         group new { salesDet, inverntory } by new { salesDet.barcode_no, inverntory.portal_id } into g
                                         select new
                                         {
                                             BarcodeNo = g.Key.barcode_no,
                                             PortalID = (int)g.Key.portal_id,
                                             Qty = 1
                                         }).ToList();
                        if (salesData != null && salesData.Count() > 0) {
                            foreach (var data in salesData) {
                                int portalID = data.PortalID;
                                switch ((SIGlobals.MarketPlace)portalID) {
                                    case SIGlobals.MarketPlace.Amazon:
                                        if (DoAmazonUpdate(companyCode, branchCode)) {
                                            //bool updated = UpdateAmazonMarketplaceInventory(data.BarcodeNo, branchCode, data.Qty, actionType, out error);
                                            bool updated = UpdateAmazonMarketplaceInventory("TestSku1", branchCode, 1, actionType, out error);
                                            if (!updated) {
                                                return false;
                                            }
                                        }
                                        break;
                                    case SIGlobals.MarketPlace.BhimaEcom:
                                        if (DoBhimaEComUpdate(companyCode, branchCode)) {
                                            bool updated = UpdateBhimaEComMarketplaceInventory(companyCode, branchCode, data.BarcodeNo, data.Qty, actionType, out error);
                                            if (!updated) {
                                                return false;
                                            }
                                        }
                                        break;
                                    case SIGlobals.MarketPlace.Instore:
                                        break;
                                }
                            }
                        }
                        break;
                    case TransactionsType.BranchIssue:
                        break;
                    case TransactionsType.BranchReceipt:
                        break;
                }
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
        private bool DoAmazonUpdate(string companyCode, string branchCode)
        {
            if (SIGlobals.Globals.GetApplicationConfigurationSettnigs(db, companyCode, branchCode, "72020").Value == 1) {
                return true;
            }
            else {
                return false;
            }
        }
        private bool UpdateAmazonMarketplaceInventory(string barcodeNo, string branchCode, int qty, ActionType actionType, out ErrorVM error)
        {
            ErrorCls apiError = new ErrorCls();
            error = null;

            try {
                // Getting SKU Information
                SKUInfo skuInfo = amazon.GetSKUInfo(barcodeNo, SIGlobals.MarketPlace.Amazon, out apiError);
                if (skuInfo == null) {
                    error = new ErrorVM()
                    {
                        ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError,
                        description = apiError.errorInfo.description,
                    };
                    return false;
                }

                // Checking Exist or Not
                if (skuInfo.MarketPlaceAsin != "") {
                    MarketplaceInventories inventory = amazon.GetMarketplaceInventory(barcodeNo, branchCode, out apiError);
                    if (inventory == null) {
                        return false;
                    }

                    // If Exists,Updating the Market place inventory
                    if (actionType == ActionType.Block) {
                        if (inventory.SellableQuantity > 0) {
                            int presentInventory = inventory.SellableQuantity + inventory.ReservedQuantity;
                            bool updated = amazon.UpdateMarketplaceInventory(barcodeNo, branchCode, (presentInventory - qty), out apiError);
                            if (!updated) {
                                error = new ErrorVM()
                                {
                                    ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError,
                                    description = apiError.errorInfo.description + apiError.statusCode,
                                };
                                return false;
                            }
                        }
                    }
                    else {
                        int presentInventory = inventory.SellableQuantity + inventory.ReservedQuantity;
                        bool updated = amazon.UpdateMarketplaceInventory(barcodeNo, branchCode, (presentInventory + qty), out apiError);
                        if (!updated) {
                            error = new ErrorVM()
                            {
                                ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError,
                                description = apiError.errorInfo.description,
                            };
                            return false;
                        }
                    }
                }
            }
            catch {
                return false;
            }
            return true;
        }
        private bool DoBhimaEComUpdate(string companyCode, string branchCode)
        {
            if (SIGlobals.Globals.GetApplicationConfigurationSettnigs(db, companyCode, branchCode, "6112020").Value == 1) {
                return true;
            }
            else {
                return false;
            }
        }
        private bool UpdateBhimaEComMarketplaceInventory(string companyCode, string branchCode, string barcodeNo, int qty, ActionType actionType, out ErrorVM error)
        {
            error = null;
            bool updated = true;
            ErrorCls apiError = new ErrorCls();
            try {
                if (actionType == ActionType.Block) {
                    updated = bhima.UpdateBhimaEComMarketplaceInventory(companyCode, branchCode, barcodeNo, 0, 0, "Inventory", out apiError);
                }
                else {
                    updated = bhima.UpdateBhimaEComMarketplaceInventory(companyCode, branchCode, barcodeNo, 1, 1, "Inventory", out apiError);
                }
                if (!updated) {
                    error = new ErrorVM()
                    {
                        ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError,
                        description = apiError.errorInfo.description,
                    };
                    return false;
                }
            }
            catch {
                return false;
            }
            return true;
        }
        #endregion
        

        #region Invoicing Methods accross market-places
        protected bool GetInvoiceInPdf(string companyCode, string branchCode, int invoiceNo, out string htmlContent, out string pdfFilePath, out string errorMessage)
        {
            //1. Get Invoice HTML. 
            //2. And write that HTML to PDF and get the pdf file path.
            //3. Upload that pdf file to to BJEcomm
            //4. Get byte array and send it to API.
            errorMessage = string.Empty;
            pdfFilePath = string.Format($"sale_invoice-{branchCode}-{invoiceNo}.pdf");
            htmlContent = string.Empty;
            try {
                DocumentPrintBL docPrinter = new DocumentPrintBL();
                htmlContent = docPrinter.PrintSalesBillWithHeaderFooter(companyCode, branchCode, invoiceNo, out errorMessage);
                if (string.IsNullOrEmpty(htmlContent)) {
                    errorMessage = "Failed to get HTML of invoice. However invoice is generated. Error: " + errorMessage;
                    return false;
                }

                string invoiceMapPath = System.Web.HttpContext.Current.Request.MapPath(@"~\App_Data\invoices\");
                if (!Directory.Exists(invoiceMapPath))
                    Directory.CreateDirectory(invoiceMapPath);
                pdfFilePath = invoiceMapPath + pdfFilePath;
                if (!docPrinter.ConvertHtmlToPdf(htmlContent, pdfFilePath)) {
                    errorMessage = "Unable to convert Html to Pdf.";
                    return false;
                }
            }
            catch (Exception ex) {
                errorMessage = "Failed to convert html to pdf. Error: " + new ErrorVM().GetErrorDetails(ex).description;
                return false;
            }
            return true;
        }
        protected bool CreatePoSInvoice(string barcodeNo, decimal totalAmount, decimal gstAmount, string isInterstate, int orderNo, decimal vaAmount, out int billNo)
        {
            billNo = 0;
            try {
                using (var transaction = db.Database.BeginTransaction()) {
                    string query = string.Format($"EXEC [usp_GenerateSalesBill] '{barcodeNo}',{totalAmount},{gstAmount},'{"N"}',{orderNo},{vaAmount}");
                    billNo = Globals.GetScalarValue<int>(query);
                    if (billNo > 0)
                        transaction.Commit();
                    else {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
            catch (Exception) {
                throw;
            }
            return true;
        }

        #endregion

        #region Ship-label Generation API methods
        protected DocumentInfoOutputVM GetShipLabelContent(int orderNo, string shipLabelContent, string contentName)
        {
            return new DocumentInfoOutputVM
            {
                Data = new List<DocumentInformationVM>
                {
                    new DocumentInformationVM
                    {
                        DocumentNo = orderNo.ToString(),
                        FileData = shipLabelContent,
                        FileFormat = "pdf",
                        Name = contentName
                    }
                }
            };
        }
        #endregion

        #region Shipping API methods
        
        protected void UpdateShippedStatus(KTTU_ORDER_DETAILS orderDetail, int shipmentNo, string packageId)
        {
            orderDetail.isShipped = true;
            orderDetail.shipped_date = Globals.GetDateTime();
            db.Entry(orderDetail).State = EntityState.Modified;

            ShipmentDetail sd = new ShipmentDetail
            {
                company_code = orderDetail.company_code,
                branch_code = orderDetail.branch_code,
                shipment_no = shipmentNo,
                order_no = orderDetail.order_no,
                barcode_no = orderDetail.barcode_no,
                item_sl_no = 1,
                shipment_id = "NA",
                package_id = packageId
            };
            db.ShipmentDetails.Add(sd);
            db.SaveChanges();
        }

        #endregion

        #region Order Cancellation API methods
        public bool GetOrderListToBeCancelled(string companyCode, string branchCode, int marketplaceID, out List<ListOfValue> ordersCanBeCancelled, out ErrorVM error)
        {
            error = null;
            ordersCanBeCancelled = null;
            #region Test code for Bhima Ecomm shipping API - Pls ignore and don't delete.
            //new BhimaECommerce().GenerateBluedartEwayBillRequest(companyCode, branchCode, "21399");
            //new BhimaECommerce().CancelAwb(companyCode, branchCode, "89183253402");
            //ShipmentVM shipment = null;
            //string errorMessage = "";
            //var result = new BhimaECommerce().GenerateBluedartEwayBill(companyCode, branchCode, "21399", out shipment, out errorMessage);
            //var pickupDate = new DateTime(2021, 08, 9, 15, 30, 0, 0);
            //new BhimaECommerce().RegisterPickup(companyCode, branchCode, "21399", "89183254440", "Nageshwara", "9980181897", pickupDate);

            //var pickupDate = new DateTime(2021, 08, 9, 15, 30, 0, 0);
            //new BhimaECommerce().CancelPickupRegistration(companyCode, branchCode, 13372, "Testing", pickupDate, out errorMessage);
            //return false; 
            #endregion
            try {
                db.Configuration.UseDatabaseNullSemantics = true;
                var orders =
                    (from om in db.KTTU_ORDER_MASTER
                     join od in db.KTTU_ORDER_DETAILS
                        on new { CompanyCode = om.company_code, BranchCode = om.branch_code, OrderNo = om.order_no }
                            equals new { CompanyCode = od.company_code, BranchCode = od.branch_code, OrderNo = od.order_no }
                     where om.company_code == companyCode && om.branch_code == branchCode
                         && om.order_source == marketplaceID
                         && om.cflag != "Y"
                         && (om.is_lock != "Y" || om.is_lock == null) //is this needed? TODO: Check this
                         && od.isScheduledForPickUp != true
                     orderby om.order_no
                     select new
                     {
                         StoreOrderNo = om.order_no,
                         MarketplaceOrderNo = om.order_reference_no
                     }).Distinct().ToList();
                if (orders == null || orders.Count <= 0) {
                    error = new ErrorVM { description = "No orders found (for cancellation) for the marketplace." };
                    return false;
                }
                ordersCanBeCancelled = orders.Select(x => new ListOfValue
                {
                    Code = x.StoreOrderNo.ToString(),
                    Name = x.MarketplaceOrderNo
                }).ToList();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }

            return true;
        }
        
        private bool ExecuteCancelOrderProc(string companyCode, string branchCode, int marketplaceID, int orderNo, string userId, string cancelRemarks, out ErrorVM error)
        {
            error = null;
            string canellationDate = Globals.GetDateTime().ToString("yyyy-MMM-dd hh:mm:ss");
            string proc =
                string.Format($"EXEC usp_CancelOnlineOrders @companyCode = '{companyCode}', @branchCode = '{branchCode}', @remarks = '{cancelRemarks}', @canelledBy = '{userId}', @marketPlaceID = '{marketplaceID}', @cancelledDate = '{canellationDate}', @orderNo = '{orderNo}'");

            using (var trans = db.Database.BeginTransaction()) {
                try {
                    Globals.ExecuteSQL(proc, db);
                    trans.Commit();
                }
                catch (Exception ex) {
                    trans.Rollback();
                    error = new ErrorVM().GetErrorDetails(ex);
                    return false;
                }
            }
            return true;
        }

        private bool ValidateOrderTobeCancelled(string companyCode, string branchCode, int marketplaceID, int orderNo, out string marketplaceOrderNo, out ErrorVM error)
        {
            error = null;
            marketplaceOrderNo = string.Empty;
            try {
                var orderInfo =
                    (from om in db.KTTU_ORDER_MASTER
                     join od in db.KTTU_ORDER_DETAILS
                        on new { CompanyCode = om.company_code, BranchCode = om.branch_code, OrderNo = om.order_no }
                            equals new { CompanyCode = od.company_code, BranchCode = od.branch_code, OrderNo = od.order_no }
                     where om.company_code == companyCode && om.branch_code == branchCode
                         && om.order_source == marketplaceID
                         && om.order_no == orderNo
                     //&& om.cflag != "Y"
                     //&& (om.is_lock != "Y" || om.is_lock == null) //is this needed? TODO: Check this
                     //&& od.isScheduledForPickUp != true
                     select new
                     {
                         StoreOrderNo = om.order_no,
                         MarketplaceOrderNo = om.order_reference_no,
                         CancelFlag = om.cflag,
                         ScheduledForPickup = od.isScheduledForPickUp
                     }).Distinct().FirstOrDefault();
                if (orderInfo == null) {
                    error = new ErrorVM { description = "No orders found (for cancellation) for the marketplace." };
                    return false;
                }
                if (orderInfo.CancelFlag == "Y") {
                    error = new ErrorVM { description = string.Format($"The order {orderInfo.StoreOrderNo} is already cancelled.") };
                    return false;
                }
                if (orderInfo.ScheduledForPickup == true) {
                    error = new ErrorVM { description = string.Format($"The order {orderInfo.StoreOrderNo} is already scheduled for pick-up and therefore cannot be cancelled.") };
                    return false;
                }
                marketplaceOrderNo = orderInfo.MarketplaceOrderNo;
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        #endregion

        #region Auxilliary Methods
        protected bool IsOrderCancelled(string companyCode, string branchCode, int orderNo, out ErrorVM error)
        {
            error = null;
            var orderMaster = db.KTTU_ORDER_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.order_no == orderNo).FirstOrDefault();
            if (orderMaster == null) {
                error = new ErrorVM() { description = "Order details are not found for order #" + orderNo.ToString() };
                return true;
            }
            if (orderMaster.cflag == "Y") {
                error = new ErrorVM() { description = string.Format($"The order {orderNo} is cancelled.") };
                return true;
            }
            return false;
        }
        protected bool IsOrderInvoiced(string companyCode, string branchCode, int orderNo, out ErrorVM error)
        {
            error = null;
            var orderMaster = db.KTTU_ORDER_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.order_no == orderNo).FirstOrDefault();
            if (orderMaster == null) {
                error = new ErrorVM() { description = "Order details are not found for order #" + orderNo.ToString() };
                return false;
            }
            if (orderMaster.cflag == "Y") {
                error = new ErrorVM() { description = string.Format($"The order {orderNo} is cancelled.") };
                return false;
            }
            if (orderMaster.bill_no <= 0) {
                error = new ErrorVM() { description = string.Format($"The order {orderNo} is not yet invoiced.") };
                return false;
            }
            return true;
        }
        #endregion

        #region Shipment handling methods - includes Get PDf Content, Register Pickup, Cancel Awb, Cancel pickup Registration
        public bool GetAWBFromOrderNo(string companyCode, string branchCode, int orderNo, out string awbNo, out ErrorVM error)
        {
            error = null;
            awbNo = string.Empty;
            try {
                var shipment = db.OrderItemShipmentLabels.Where(x => x.company_code == companyCode
                    && x.branch_code == branchCode && x.order_no == orderNo).FirstOrDefault();
                if (shipment == null) {
                    error = new ErrorVM { description = "There is no AWB number for the given order number: " + orderNo.ToString() };
                    return false;
                }
                awbNo = shipment.shipLabel;

            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        public bool GetAWBPrintContent(string companyCode, string branchCode, int orderNo, out DocumentInfoOutputVM shipLabelContent, out ErrorVM error)
        {
            error = null;
            shipLabelContent = null;
            try {
                var shipment = db.OrderItemShipmentLabels.Where(x => x.company_code == companyCode
                    && x.branch_code == branchCode && x.order_no == orderNo).FirstOrDefault();
                if (shipment == null) {
                    error = new ErrorVM { description = "There is no AWB number/shiplabel content for the given order number: " + orderNo.ToString() };
                    return false;
                }
                if (shipment.ShipLabelPDFObject != null) {
                    shipLabelContent = GetShipLabelContent(orderNo, shipment.ShipLabelPDFObject, "Bhima-Ship label");
                    return true;
                }
                else {
                    error = new ErrorVM { description = "There is no AWB number/shiplabel content for the given order number: " + orderNo.ToString() };
                    return false;
                }

            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
        }

        public bool PickupRegistration(PickupRegistrationInputVM vm, out string tokenNo, out ErrorVM error)
        {
            error = null;
            string awbNo = string.Empty;
            tokenNo = string.Empty;
            try {
                var shipment = db.OrderItemShipmentLabels.Where(x => x.company_code == vm.CompanyCode
                    && x.branch_code == vm.BranchCode && x.order_no == vm.OrderNo).FirstOrDefault();
                if (shipment == null) {
                    error = new ErrorVM { description = "There is no AWB number for the given order number: " + vm.OrderNo.ToString() };
                    return false;
                }
                awbNo = shipment.shipLabel;

                BhimaECommerce ecomm = new BhimaECommerce();

                string statusMessage = string.Empty;
                bool apiSucceeded = ecomm.RegisterPickup(vm, awbNo, out tokenNo, out statusMessage);
                if (!apiSucceeded) {
                    error = new ErrorVM { description = "Failed to register pickup. Error: " + statusMessage };
                    return false;
                }

            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        public bool CancelPickupRegistration(string companyCode, string branchCode, int tokenNo, string remarks, DateTime registrationDate, out ErrorVM error)
        {
            error = null;
            try {
                BhimaECommerce ecomm = new BhimaECommerce();
                string statusMessage = string.Empty;
                bool apiSucceeded = ecomm.CancelPickupRegistration(companyCode, branchCode, tokenNo, remarks, registrationDate, out statusMessage);
                if (!apiSucceeded) {
                    error = new ErrorVM { description = "Failed to register pickup. Error: " + statusMessage };
                    return false;
                }
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        public bool CancelAwb(string companyCode, string branchCode, int orderNo, out ErrorVM error)
        {
            error = null;
            try {
                var shipment = db.OrderItemShipmentLabels.Where(x => x.company_code == companyCode
                    && x.branch_code == branchCode && x.order_no == orderNo).FirstOrDefault();
                if (shipment == null) {
                    error = new ErrorVM { description = "There is no AWB number for the given order number: " + orderNo.ToString() };
                    return false;
                }
                string awbNo = shipment.shipLabel;

                BhimaECommerce ecomm = new BhimaECommerce();
                string statusMessage = string.Empty;
                bool apiSucceeded = ecomm.CancelBluedartEwayBill(companyCode, branchCode, awbNo, out statusMessage);
                if (!apiSucceeded) {
                    error = new ErrorVM { description = "Failed to cancel AWB. Error: " + statusMessage };
                    return false;
                }

            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }
        #endregion
    }
}
