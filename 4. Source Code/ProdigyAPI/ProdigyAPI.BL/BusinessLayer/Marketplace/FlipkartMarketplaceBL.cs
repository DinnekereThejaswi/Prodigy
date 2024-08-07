using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Marketplace;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.Model.MagnaDb;
using System.IO;
using ProdigyAPI.SIGlobals;
using ProdigyAPI.Marketplace;
using ProdigyAPI.Marketplace.Models.BJEComm;

namespace ProdigyAPI.BL.BusinessLayer.Marketplace
{
    class FlipkartMarketplaceBL : OnlineMarketpaceBase, IMarketplace
    {
        public FlipkartMarketplaceBL(): base()
        {
            base.db = new MagnaDbEntities(true);
        }
        public FlipkartMarketplaceBL(MagnaDbEntities dbContext) : base()
        {
            base.db = dbContext;
        }
        public bool CreateMarketplacePackage(PackingItemVM pack, out ErrorVM error)
        {
            error = null;
            try {
                if (base.IsOrderCancelled(pack.CompanyCode, pack.BranchCode, pack.OrderNo, out error))
                    return false;
                
                var existingPackage = db.PackageHeaders.Where(x => x.company_code == pack.CompanyCode
                    && x.branch_code == pack.BranchCode && x.OrderID == pack.OrderNo).FirstOrDefault();
                if (existingPackage == null) {
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
                    db.PackageHeaders.Add(ph);
                }
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
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
                if (orderHeader.cflag == "Y") {
                    error = new ErrorVM { description = string.Format($"The Order # {vm.OrderNo} is cancelled.") };
                    return false;
                }
                int posInvoiceNo = 0;

                string invoiceFileData = string.Empty;
                string posInvoiceFileData = string.Empty;

                if (orderHeader.bill_no <= 0) {
                    bool posInvoicingSucceeded = CreatePoSInvoice(vm.SKU, vm.TotalAmount, Convert.ToDecimal(vm.GSTAmt), "Y", vm.OrderNo, 0, out posInvoiceNo);
                    if (!posInvoicingSucceeded) {
                        error = new ErrorVM { description = "Failed to generate PoS invoice." };
                        return false;
                    }
                }
                else
                    posInvoiceNo = orderHeader.bill_no;

                string errorMessage = string.Empty;
                string htmlInvoiceData = string.Empty;
                string pdfFilePath = string.Empty;
                if (!GetInvoiceInPdf(companyCode, branchCode, posInvoiceNo, out htmlInvoiceData, out pdfFilePath, out errorMessage)) {
                    error = new ErrorVM { description = errorMessage };
                    return false;
                }

                var fileData = File.ReadAllBytes(pdfFilePath);
                invoiceFileData = Convert.ToBase64String(fileData);
                posInvoiceFileData = Globals.Base64Encode(htmlInvoiceData);
                invoiceOutputVM = new DocumentInfoOutputVM
                {
                    Data = new List<DocumentInformationVM> {
                        new DocumentInformationVM
                        {
                            DocumentNo = vm.OrderNo.ToString(),
                            FileData = posInvoiceFileData,
                            Name = "Pos-Invoice",
                            FileFormat = "html"
                        },
                        new DocumentInformationVM
                        {
                            DocumentNo = vm.OrderReferenceNo,
                            FileData = invoiceFileData,
                            Name = "Flipkart-eComm",
                            FileFormat = "pdf"
                        }
                    }
                };
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
            try {
                if (!IsOrderInvoiced(companyCode, branchCode, vm.OrderNo, out error)) {
                    return false;
                }
                var orderDetail = db.KTTU_ORDER_DETAILS.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                        && x.order_no == vm.OrderNo).FirstOrDefault();
                if (orderDetail == null) {
                    error = new ErrorVM { description = "Order detail is not found for the order number: " + vm.OrderNo.ToString() };
                    return false;
                }
                var orderShipment = db.OrderItemShipmentLabels.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.order_no == vm.OrderNo).FirstOrDefault();
                if (orderShipment != null) {
                    if (!string.IsNullOrEmpty(orderShipment.ShipLabelPDFObject)) {
                        shiplabelOutput = GetShipLabelContent(vm.OrderNo, orderShipment.ShipLabelPDFObject, "Bhima-Ship label");
                    }
                    return true;
                }

                OrderItemShipmentLabel shipment = new OrderItemShipmentLabel();
                shipment.company_code = companyCode;
                shipment.branch_code = branchCode;
                shipment.order_ref_no = vm.OrderReferenceNo;
                shipment.order_no = vm.OrderNo;
                shipment.bill_no = vm.BillNo;
                shipment.shipLabel = string.Empty;
                shipment.operator_code = userID;
                shipment.ShipLabelPDFObject = string.Empty;
                shipment.shipLabelCreatedDate = Globals.GetDateTime();
                shipment.isScheduledforPickUp = string.Empty;
                shipment.PickUpTokenNo = string.Empty;

                db.OrderItemShipmentLabels.Add(shipment);
                orderDetail.isScheduledForPickUp = true;
                orderDetail.scheduled_for_pickup_date = Globals.GetDateTime();
                db.SaveChanges();
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
                if (orderDetail.isScheduledForPickUp != true) {
                    error = new ErrorVM { field = order.OrderNo.ToString(), description = string.Format($"Order No. {order.OrderNo} is not ready to be shipped.") };
                    return false;

                }
                string awbNo = string.Empty;
                var orderItemShipmentUpdate = db.OrderItemShipmentLabels.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.order_no == order.OrderNo).FirstOrDefault();
                if (orderItemShipmentUpdate != null)
                    awbNo = orderItemShipmentUpdate.shipLabel;

                string packageId = string.Empty;
                var packageHeader = db.PackageHeaders.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.OrderID == order.OrderNo).FirstOrDefault();
                if (packageHeader != null) {
                    packageId = packageHeader.PackageID;
                }

                OrderVM orderVM = new OrderVM
                {
                    branchorderno = order.OrderNo.ToString(),
                    orderreferanceno = order.OrderReferenceNo,
                    status = "SHIPPED",
                    awno = awbNo,
                    cancelled_by = "",
                    comments = ""
                };
                #region In case of BJ Ecomm order, an API to be called to update order status
                bool isBJECommOrder = false;
                if (isBJECommOrder) {
                    BhimaECommerce bjEcomm = new BhimaECommerce();
                    string errorMessage = string.Empty;
                    bool succeeded = bjEcomm.PostOrderStatus(companyCode, branchCode, orderVM, "dispatch", out errorMessage);
                    if (!succeeded) {
                        error = new ErrorVM { description = "Failed to update package status. Error: " + errorMessage };
                        return false;
                    }
                }
                #endregion
                UpdateShippedStatus(orderDetail, shipmentNo, packageId);
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                error.field = order.OrderNo.ToString();
                return false;
            }
            return true;
        }

        public bool CancelMarketplaceOrder(string companyCode, string branchCode, int marketplaceID, int orderNo, string marketplaceOrderNo, string userId, string cancelRemarks, out ErrorVM error)
        {
            error = null;
            return true;
        }
    }
}
