using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Marketplace;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.Model.MagnaDb;
using ProdigyAPI.Marketplace;
using ProdigyAPI.Marketplace.Models;
using ProdigyAPI.SIGlobals;
using System.Data.Entity;

namespace ProdigyAPI.BL.BusinessLayer.Marketplace
{
    class AmazonMarketplaceBL : OnlineMarketpaceBase, IMarketplace
    {
        public AmazonMarketplaceBL(): base()
        {
            base.db = new MagnaDbEntities(true);
        }
        public AmazonMarketplaceBL(MagnaDbEntities dbContext) : base()
        {
            base.db = dbContext;
        }
        public bool CreateMarketplacePackage(PackingItemVM pack, out ErrorVM error)
        {
            try {
                var packingDet = (from p in db.KSTU_PACKING_DETAILS
                                  where p.company_code == pack.CompanyCode & p.branch_code == pack.BranchCode && p.p_code == pack.PackageCode
                                  select new
                                  {
                                      Length = p.m_length,
                                      LenthUOM = p.m_length_uom,
                                      Width = p.m_width,
                                      WithUOM = p.m_width_uom,
                                      Height = p.m_height,
                                      HeightUOM = p.m_height_uom,
                                      Weight = p.m_weight,
                                      WeithtUOM = p.m_weight_uom,
                                  }).FirstOrDefault();
                if (packingDet == null) {
                    error = new ErrorVM { description = "Package details not found for the package Id: " + pack.PackageCode };
                    return false;
                }

                string marketplaceOrderStatus = GetOrderStatus(pack.OrderReferenceNo, out error);
                if (string.IsNullOrEmpty(marketplaceOrderStatus)) {
                    error = new ErrorVM { description = string.Format($"Unable to get status for order No. {pack.OrderNo} and therefore cannot proceed further.") };
                    return false;
                }
                if (marketplaceOrderStatus == "CANCELLED") {
                    error.description = "This order is cancelled at market place";
                    error.ErrorStatusCode = System.Net.HttpStatusCode.BadRequest;
                    return false;
                }
                if (marketplaceOrderStatus == "INVOICE_GENERATED" || marketplaceOrderStatus == "SHIPLABEL_GENERATED" || marketplaceOrderStatus == "SHIPPED") {
                    error.description = "Invoice already generated for this order, Cannot update the package details.";
                    error.ErrorStatusCode = System.Net.HttpStatusCode.BadRequest;
                    return false;
                }

                PackageHeader ph = new PackageHeader
                {
                    company_code = pack.CompanyCode,
                    branch_code = pack.BranchCode,
                    PackageCode = pack.PackageCode,
                    OrderID = pack.OrderNo,
                    MarketPlaceOrderID = pack.OrderReferenceNo,
                    OrderLineItemNo = Convert.ToString(pack.MarketplaceSlNo),
                    DoP = SIGlobals.Globals.GetApplicationDate(pack.CompanyCode, pack.BranchCode),
                    OTLNos = pack.OTLNo,
                    PackageID = pack.PackageID == "" ? Guid.NewGuid().ToString() : pack.PackageID,
                    qty = 1,
                    Length = Convert.ToDecimal(packingDet.Length),
                    LengthDimensionUnit = Convert.ToString(packingDet.LenthUOM),
                    Height = Convert.ToDecimal(packingDet.Height),
                    HeightDimensionUnit = Convert.ToString(packingDet.HeightUOM),
                    Weight = Convert.ToDecimal(packingDet.Weight) > pack.Weight ? Convert.ToDecimal(packingDet.Weight) : pack.Weight,
                    WeightDimensionUnit = Convert.ToString(packingDet.WeithtUOM),
                    Width = Convert.ToDecimal(packingDet.Width),
                    WidthDimensionUnit = Convert.ToString(packingDet.WithUOM),
                    PackageCreatedStatus = 1
                };

                Package itempackage = new Package();
                itempackage.id = ph.PackageID;

                List<PackageDimension> packDimension = new List<PackageDimension>();
                itempackage.length = new PackageDimension();
                itempackage.length.dimensionUnit = ph.LengthDimensionUnit;
                itempackage.length.value = ph.Length;

                itempackage.width = new PackageDimension();
                itempackage.width.dimensionUnit = ph.WidthDimensionUnit;
                itempackage.width.value = ph.Width;

                itempackage.height = new PackageDimension();
                itempackage.height.dimensionUnit = ph.HeightDimensionUnit;
                itempackage.height.value = ph.Height;

                itempackage.weight = new PackageWeight();
                itempackage.weight.weightUnit = ph.WeightDimensionUnit;
                itempackage.weight.value = ph.Weight;

                itempackage.hazmatLabels = new List<string>();
                itempackage.packagedLineItems = new List<PackagedLineItem>();

                List<PackagedLineItem> packageLineItemList = new List<PackagedLineItem>();
                PackagedLineItem plineItem = new PackagedLineItem();
                plineItem.lineItem = new OrderLineItem();
                plineItem.lineItem.id = "0";
                plineItem.quantity = 1;

                packageLineItemList.Add(plineItem);
                itempackage.packagedLineItems = packageLineItemList;
                plineItem.serialNumbers = new List<SerialNumber>();


                List<Package> lstOfPackage = new List<Package>();
                lstOfPackage.Add(itempackage);

                Packaging package = new Packaging();
                package.packages = lstOfPackage;

                AmazonMarketplace os = new AmazonMarketplace();
                ErrorInfo errorInfo = null;
                if (marketplaceOrderStatus == "PACKAGE_CREATED" || marketplaceOrderStatus == "PICKUP_SLOT_RETRIEVED") {
                    bool updatePackageSuccessfull = os.UpdatePackage("AMAZON", package, pack.OrderReferenceNo, out errorInfo);
                    if (!updatePackageSuccessfull) {
                        error = new ErrorVM { description = errorInfo.Description };
                        return false;
                    }
                }
                else {
                    bool packagingSuceess = os.CreatePackage("AMAZON", package, pack.OrderReferenceNo, out errorInfo);
                    if (!packagingSuceess) {
                        error = new ErrorVM { description = errorInfo.Description };
                        return false;
                    }
                    db.PackageHeaders.Add(ph);
                }
                bool pickupSlotSuceess = os.RetrievePickupSlots("AMAZON", pack.OrderReferenceNo, out errorInfo);
                if (!pickupSlotSuceess) {
                    error = new ErrorVM { description = "Pickup slot retrieval error: " + errorInfo.Description };
                    return false;
                }

            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        private string GetOrderStatus(string marketplaceOrderId, out ErrorVM error)
        {
            error = null;
            string orderStatus = null;
            ErrorInfo apiError = new ErrorInfo();
            try {
                orderStatus = new AmazonMarketplace().GetOrderStatus(marketplaceOrderId, out apiError);
                if (apiError != null) {
                    error = new ErrorVM { description = apiError.Description };
                    orderStatus = string.Empty;
                }
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
            }
            return orderStatus;
        }

        public bool GenerateMarketplaceInvoice(string companyCode, string branchCode, MarketplaceOrdersToBeProcessed vm, out DocumentInfoOutputVM invoiceOutputVM, out ErrorVM error)
        {
            error = null;
            invoiceOutputVM = null;
            if (vm == null) {
                error = new ErrorVM { description = "There is nothing to invoice." };
                return false;
            }
            try {
                var orderHeader = db.KTTU_ORDER_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.order_no == vm.OrderNo).FirstOrDefault();
                if (orderHeader == null) {
                    error = new ErrorVM { description = string.Format($"Order No. {vm.OrderNo} is not found.") };
                    return false;
                }
                int posInvoiceNo = 0;

                AmazonMarketplace am = new AmazonMarketplace();
                Invoice invoice = null;
                ErrorInfo azError = null;
                string invoiceFileData = string.Empty;
                string posInvoiceFileData = string.Empty;

                if (orderHeader.bill_no <= 0) {
                    //Invoke fresh billing
                    var marketplaceInvSuceeded = am.GenerateInvoice("AMAZON", vm.OrderReferenceNo, out invoice, out azError);
                    if (!marketplaceInvSuceeded) {
                        error = new ErrorVM { description = "Failed to generate Amazon invoice. Error: " + azError.Description };
                        return false;
                    }
                    if (invoice.fileData == null) {
                        if (invoice.fileData.encryptedContent == null)
                            error = new ErrorVM { description = "Failed to generate Amazon invoice. Invoice information is not found" };
                        return false;

                    }
                    invoiceFileData = AESCrypto.DecryptString(invoice.fileData.encryptedContent.value);

                    bool posInvoicingSucceeded = base.CreatePoSInvoice(vm.SKU, vm.TotalAmount, Convert.ToDecimal(vm.GSTAmt), "Y", vm.OrderNo, 0, out posInvoiceNo);
                    if (!posInvoicingSucceeded) {
                        error = new ErrorVM { description = "Failed to generate PoS invoice." };
                        return false;
                    }
                    string errorMessage = string.Empty;
                    string htmlInvoiceData = string.Empty;
                    string pdfFilePath = string.Empty;
                    if (!GetInvoiceInPdf(companyCode, branchCode, posInvoiceNo, out htmlInvoiceData, out pdfFilePath, out errorMessage)) {
                        error = new ErrorVM { description = errorMessage };
                        return false;
                    }

                    posInvoiceFileData = Globals.Base64Encode(htmlInvoiceData);
                    invoiceOutputVM = new DocumentInfoOutputVM
                    {
                        Data = new List<DocumentInformationVM> {
                            new DocumentInformationVM
                            {
                                DocumentNo = posInvoiceNo.ToString(),
                                FileData = posInvoiceFileData,
                                Name = "PoS Invoice",
                                FileFormat = "html"
                            },
                            new DocumentInformationVM
                            {
                                DocumentNo = vm.OrderReferenceNo,
                                FileData = invoiceFileData,
                                Name = "AMAZON",
                                FileFormat = "pdf"//invoice.fileData.format
                            }
                        }
                    };
                }
                else {
                    //Call reprint
                    var marketplaceInvSuceeded = am.RetrieveInvoice("AMAZON", vm.OrderReferenceNo, out invoice, out azError);
                    if (!marketplaceInvSuceeded) {
                        error = new ErrorVM { description = "Failed to retrieve Amazon invoice. Error: " + azError.Description };
                        return false;
                    }
                    if (invoice.fileData == null) {
                        if (invoice.fileData.encryptedContent == null)
                            error = new ErrorVM { description = "Failed to generate Amazon invoice. Invoice information is not found" };
                        return false;

                    }
                    invoiceFileData = AESCrypto.DecryptString(invoice.fileData.encryptedContent.value);
                    string errorMessage = string.Empty;
                    string htmlInvoiceData = string.Empty;
                    string pdfFilePath = string.Empty;
                    posInvoiceNo = orderHeader.bill_no;
                    if (!base.GetInvoiceInPdf(companyCode, branchCode, posInvoiceNo, out htmlInvoiceData, out pdfFilePath, out errorMessage)) {
                        error = new ErrorVM { description = errorMessage };
                        return false;
                    }
                    posInvoiceFileData = Globals.Base64Encode(htmlInvoiceData);
                    invoiceOutputVM = new DocumentInfoOutputVM
                    {
                        Data = new List<DocumentInformationVM>
                        {
                            new DocumentInformationVM
                            {
                                DocumentNo = posInvoiceNo.ToString(),
                                FileData = posInvoiceFileData,
                                Name = "PoS-Invoice",
                                FileFormat = "html"
                            },
                            new DocumentInformationVM
                            {
                                DocumentNo = vm.OrderReferenceNo,
                                FileData = invoiceFileData,
                                Name = "Amazon-Invoice",
                                FileFormat = "pdf"//invoice.fileData.format
                            }
                        }
                    };
                }

            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        public bool GenerateMarketplaceShiplabel(string companyCode, string branchCode, string userID, MarketplaceOrdersToBeProcessed vm, out DocumentInfoOutputVM shiplabelOutput, out ErrorVM error)
        {
            error = null;
            shiplabelOutput = null;
            if (vm == null) {
                error = new ErrorVM { description = "There is nothing to create shiplabel." };
                return false;
            }
            try {
                if (!IsOrderInvoiced(companyCode, branchCode, vm.OrderNo, out error)) {
                    return false;
                }
                var orderDetail = db.KTTU_ORDER_DETAILS.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.order_no == vm.OrderNo).FirstOrDefault();
                if (orderDetail == null) {
                    error = new ErrorVM { description = string.Format($"Order No. {vm.OrderNo} is not found.") };
                    return false;
                }
                bool isShiplabelGenerated = (orderDetail.isScheduledForPickUp == null || orderDetail.isScheduledForPickUp == false) ? false : true;

                var packageHeader = db.PackageHeaders.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.OrderID == orderDetail.order_no).FirstOrDefault();
                if (packageHeader == null) {
                    error = new ErrorVM { description = string.Format($"Could not fetch package details for the Order No. {vm.OrderNo}.") };
                    return false;
                }
                string packageId = packageHeader.PackageID;

                AmazonMarketplace am = new AmazonMarketplace();
                ShipLabel shipLabel = null;
                ErrorInfo azError = null;
                string marketplaceFileData = string.Empty;

                if (!isShiplabelGenerated) {
                    var marketplaceInvSuceeded = am.GenerateShipLabel("AMAZON", vm.OrderReferenceNo, packageId, out shipLabel, out azError);
                    if (!marketplaceInvSuceeded) {
                        error = new ErrorVM { description = "Failed to generate Amazon Ship-label. Error: " + azError.Description };
                        return false;
                    }
                    if (shipLabel.fileData == null) {
                        if (shipLabel.fileData.encryptedContent == null)
                            error = new ErrorVM { description = "Failed to generate Amazon Ship-label. Ship-label information is not found" };
                        return false;

                    }
                    marketplaceFileData = AESCrypto.DecryptString(shipLabel.fileData.encryptedContent.value);


                    shiplabelOutput = new DocumentInfoOutputVM
                    {
                        Data = new List<DocumentInformationVM>
                        {
                            new DocumentInformationVM
                            {
                                DocumentNo = orderDetail.order_no.ToString(),
                                FileData = marketplaceFileData,
                                Name = "Amazon-ship-label",
                                FileFormat = shipLabel.fileData.format
                            }
                        }
                    };
                    OrderItemShipmentLabel osl = new OrderItemShipmentLabel
                    {
                        company_code = companyCode,
                        branch_code = branchCode,
                        bill_no = vm.BillNo,
                        isScheduledforPickUp = "Y",
                        order_no = vm.OrderNo,
                        order_ref_no = vm.OrderReferenceNo,
                        operator_code = userID,
                        shipLabelCreatedDate = Globals.GetDateTime(),
                        shipLabel = shipLabel.shipLabelMetadata.trackingId,
                        ShipLabelPDFObject = marketplaceFileData,

                    };
                    db.OrderItemShipmentLabels.Add(osl);

                    orderDetail.isScheduledForPickUp = true;
                    orderDetail.UpdateOn = Globals.GetDateTime();
                    orderDetail.scheduled_for_pickup_date = Globals.GetApplicationDate();
                    db.Entry(orderDetail).State = EntityState.Modified;

                    db.SaveChanges();
                }
                else {
                    var marketplaceInvSuceeded = am.RetrieveShipLabel("AMAZON", vm.OrderReferenceNo, packageId, out shipLabel, out azError);
                    if (!marketplaceInvSuceeded) {
                        error = new ErrorVM { description = "Failed to retrieve Amazon Ship-label. Error: " + azError.Description };
                        return false;
                    }
                    if (shipLabel.fileData == null) {
                        if (shipLabel.fileData.encryptedContent == null)
                            error = new ErrorVM { description = "Failed to generate Amazon Ship-label. Ship-label information is not found" };
                        return false;

                    }
                    marketplaceFileData = AESCrypto.DecryptString(shipLabel.fileData.encryptedContent.value);
                    shiplabelOutput = new DocumentInfoOutputVM
                    {
                        Data = new List<DocumentInformationVM>
                        {
                            new DocumentInformationVM
                            {
                                DocumentNo = orderDetail.order_no.ToString(),
                                FileData = marketplaceFileData,
                                Name = "Amazon-ship-label",
                                FileFormat = shipLabel.fileData.format
                            }
                        }
                    };
                }
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        public bool ShipMarketplaceOrder(string companyCode, string branchCode, MarketplaceOrdersToBeProcessed order, int shipmentNo, out ErrorVM error)
        {
            error = null;
            if (order == null) {
                error = new ErrorVM { description = "Invalid order details." };
                return false;
            }
            try {
                var orderDetail = db.KTTU_ORDER_DETAILS.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                && x.order_no == order.OrderNo).FirstOrDefault();
                if (orderDetail == null) {
                    error = new ErrorVM { field = order.OrderNo.ToString(), description = string.Format($"Order No. {order.OrderNo} is not found.") };
                    return false;
                }
                var packageHeader = db.PackageHeaders.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.OrderID == order.OrderNo).FirstOrDefault();
                if (packageHeader == null) {
                    error = new ErrorVM { field = order.OrderNo.ToString(), description = string.Format($"Package details for order No. {order.OrderNo} is not found.") };
                    return false;
                }
                string mpOrderStatus = GetOrderStatus(packageHeader.MarketPlaceOrderID, out error);
                if (string.IsNullOrEmpty(mpOrderStatus)) {
                    error = new ErrorVM { field = order.OrderNo.ToString(), description = string.Format($"Unable to get status for order No. {order.OrderNo} and therefore cannot proceed further. API response: {error.description}") };
                    return false;
                }
                switch (mpOrderStatus) {
                    case "SHIPPED":
                    case "DELIVERED":
                        error = new ErrorVM { field = order.OrderNo.ToString(), description = string.Format($"The Order #{order.OrderNo} is already shipped. The same is updated.") };
                        UpdateShippedStatus(orderDetail, shipmentNo, packageHeader.PackageID);
                        return true;
                    case "SHIPLABEL_GENERATED":
                        AmazonMarketplace amp = new AmazonMarketplace();
                        ErrorInfo errorInfo = null;
                        if (!amp.Ship("Amazon", packageHeader.MarketPlaceOrderID, out errorInfo)) {
                            error = new ErrorVM { field = order.OrderNo.ToString(), description = string.Format($"Failed to ship order {order.OrderNo}. Error: {errorInfo.Description}") };
                            return false;
                        }
                        UpdateShippedStatus(orderDetail, shipmentNo, packageHeader.PackageID);
                        return true;
                    default:
                        error = new ErrorVM { field = order.OrderNo.ToString(), description = string.Format($"The status of the order #{order.OrderNo} must be SHIPLABEL_CREATED to enable shipping. But the present status is {mpOrderStatus}.") };
                        return false;
                }
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                error.field = order.OrderNo.ToString();
                return false;
            }
        }

        public bool CancelMarketplaceOrder(string companyCode, string branchCode, int marketplaceID, int orderNo, string marketplaceOrderNo, string userId, string cancelRemarks, out ErrorVM error)
        {
            error = null;
            string orderStatusAtMarketplace = GetOrderStatus(marketplaceOrderNo, out error);
            if (string.IsNullOrEmpty(orderStatusAtMarketplace)) {
                error = new ErrorVM
                {
                    description = "Failed to get marketplace order status. Please contact administrator."
                    + error != null ? " Error: " + error.description : string.Empty
                };
                return false;
            }
            if (orderStatusAtMarketplace == "CANCELLED") {
                error = new ErrorVM { description = "The order is already cancelled at marketplace" };
                return false;
            }
            if (orderStatusAtMarketplace == "SHIPLABEL_GENERATED" || orderStatusAtMarketplace == "SHIPPED" || orderStatusAtMarketplace == "DELEVERED") {
                error = new ErrorVM { description = string.Format($"Since the order status at marketplace is {orderStatusAtMarketplace}, the order cannot be cancelled.") };
                return false;
            }

            ErrorInfo errorInfo = null;
            bool apiCallSuceeded = new AmazonMarketplace().RejectOrder("AMAZON", marketplaceOrderNo, out errorInfo);
            if (!apiCallSuceeded) {
                error = new ErrorVM { description = errorInfo.Description };
            }
            return apiCallSuceeded;
        }
    }
}
