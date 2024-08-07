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
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Common
{
    public class MarketplaceBL
    {
        #region Declaration
        MagnaDbEntities db = null;
        AmazonMarketplace amazon = new AmazonMarketplace();
        BhimaECommerce bhima = new BhimaECommerce();
        #endregion

        #region Constructors
        public MarketplaceBL()
        {
            db = new MagnaDbEntities();
        }
        public MarketplaceBL(MagnaDbEntities db)
        {
            this.db = db;
        }
        #endregion

        public List<MarketPlaceMaster> GetMarketPlaces()
        {
            return new MarketPlaceMaster().InitializeMarketPlaces();
        }
        public string[] GetOrderStates()
        {
            string[] orderStages = new string[] { "All", "Open", "Under Process", "Picked", "Packed", "Ready", "Shipped", "Cancelled" };
            return orderStages;
        }

        public dynamic GetOrderCount(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                db.Configuration.UseDatabaseNullSemantics = true;
                var data = (from om in db.KTTU_ORDER_MASTER
                            join od in db.KTTU_ORDER_DETAILS
                            on new { CompanyCode = om.company_code, BranchCode = om.branch_code, Order = om.order_no }
                            equals new { CompanyCode = od.company_code, BranchCode = od.branch_code, Order = od.order_no }
                            where om.company_code == companyCode
                                  && om.branch_code == branchCode                                  
                                  && om.cflag == "N"
                                  && od.isProcessed == false
                                  && od.barcode_no != "Advance"
                                  && om.order_source != null
                            group om by new { om.order_source } into orderGroup
                            select new 
                            {
                                MarketPlace = orderGroup.Key.order_source,
                                Count = orderGroup.Count()
                            }).ToList();
                if (data == null) {
                    error = new ErrorVM { description = "No details found" };
                    return null;
                }
                return data;
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return null;
            }
        }
        public bool GetOrdersToBeInvoiced(string companyCode, string branchCode, DateTime fromShipDate, DateTime toShipDate, out List<MarketplaceOrdersToBeProcessed> ordersToBeInvoiced, out ErrorVM error, int? assignmentNo = null)
        {
            error = null;
            ordersToBeInvoiced = new List<MarketplaceOrdersToBeProcessed>();
            int _assignmentNo = Convert.ToInt32(assignmentNo);
            try {
                var endingShipDate = Globals.GetEndingTimeOfDay(toShipDate);
                var result = db.usp_OrderItems_ShippingDetails(companyCode, branchCode, fromShipDate.Date, endingShipDate, _assignmentNo).ToList();
                if (result == null || result.Count <= 0) {
                    error = new ErrorVM { description = "No details found for the selected data range." };
                    return false;
                }
                ordersToBeInvoiced = result.Select(r => new MarketplaceOrdersToBeProcessed
                {
                    OrderSource = r.order_source,
                    OrderReferenceNo = r.order_reference_no,
                    CentralRefNo = r.central_ref_no,
                    OrderNo = r.order_no,
                    BillNo = r.bill_no,
                    SKU = r.item_name,
                    GSTAmt = r.tax_amt,
                    TotalAmount = r.total_amt
                }).ToList();

            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }        
        public bool GetOrdersToBeShipped(string companyCode, string branchCode, int marketplaceCode, out List<MarketplaceOrdersToBeProcessed> ordersToBeShipped, out ErrorVM error)
        {
            error = null;
            ordersToBeShipped = new List<MarketplaceOrdersToBeProcessed>();
            try {
                var result = db.usp_GetOrdersTobeShipped(companyCode, branchCode, marketplaceCode).ToList();
                if (result == null || result.Count <= 0) {
                    error = new ErrorVM { description = "No details found for the selected marketplace." };
                    return false;
                }
                ordersToBeShipped = result.Select(r => new MarketplaceOrdersToBeProcessed
                {
                    OrderSource = r.order_source,
                    OrderReferenceNo = r.order_reference_no,
                    CentralRefNo = r.central_ref_no,
                    OrderNo = r.order_no,
                    BillNo = r.bill_no,
                    SKU = r.item_name,
                    GSTAmt = r.tax_amt,
                    TotalAmount = r.total_amt,
                    PackageWeight = r.package_weight,
                    AwbNo = r.AWBNo
                }).ToList();

            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        #region Fetch Received Orders
        public dynamic GetAllReceivedOrders(string companyCode, string branchCode, DateTime fromDate, DateTime toDate, int marketPlaceID, string type, out ErrorVM error)
        {
            error = new ErrorVM();
            List<ReceivedOrderForGridVM> grdData = new List<ReceivedOrderForGridVM>();
            int?[] orderSource = { 1, 3, 5 }; //0->Store,1-> Amazon,2->Instore,3->Bhima E-Commerce
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

                switch (type.ToUpper()) {
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
                    case "UNDER PROCESS":
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
                error.ErrorStatusCode = System.Net.HttpStatusCode.BadRequest;
                return 0;
            }
            string companyCode = data[0].CompanyCode;
            string branchCode = data[0].BranchCode;
            int assignmentNo = Convert.ToInt32(SIGlobals.Globals.GetFinancialYear(db, companyCode, branchCode).ToString().Remove(0, 1)
                + db.KSTS_SEQ_NOS.Where(sq => sq.obj_id == "2020" && sq.company_code == companyCode && sq.branch_code == branchCode).FirstOrDefault().nextno);
            try {
                int slno = 1;
                foreach (ReceivedOrderForGridVM d in data) {
                    var orderDetail = db.KTTU_ORDER_DETAILS.Where(ord => ord.company_code == d.CompanyCode
                                                                    && ord.branch_code == d.BranchCode
                                                                    && ord.order_no == d.OrderNo).FirstOrDefault();
                    if (orderDetail == null) {
                        error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "There is no order lines found for the order# " + d.OrderNo.ToString() };
                        return 0;
                    }

                    for (int i = 0; i < orderDetail.quantity; i++) {
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

                    orderDetail.isProcessed = true;
                    orderDetail.processed_date = Globals.GetDateTime();
                    db.Entry(orderDetail).State = EntityState.Modified;
                }

                SIGlobals.Globals.UpdateSeqenceNumber(db, "2020", companyCode, branchCode);
                error.ErrorStatusCode = System.Net.HttpStatusCode.OK;
                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
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

        #region Item Pick List API methods
        public bool GetPickListNo(string companyCode, string branchCode, string orderStage, out List<DocumentInfoVM> data, out ErrorVM error)
        {
            error = null;
            data = new List<DocumentInfoVM>();
            try {
                //Understand the power of Linq and multi-step quering.
                db.Configuration.UseDatabaseNullSemantics = true;
                var query = from pl in db.OrderItemPickLists
                            join om in db.KTTU_ORDER_MASTER on
                                new { CompanyCode = pl.company_code, BranchCode = pl.branch_code, OrderNo = pl.order_no }
                                equals new { CompanyCode = om.company_code, BranchCode = om.branch_code, OrderNo = om.order_no }
                            join od in db.KTTU_ORDER_DETAILS on
                                new { CompanyCode = om.company_code, BranchCode = om.branch_code, OrderNo = om.order_no }
                                equals new { CompanyCode = od.company_code, BranchCode = od.branch_code, OrderNo = od.order_no }
                            where om.company_code == companyCode && om.branch_code == branchCode && om.cflag != "Y"
                            select new { od, pl };
                switch (orderStage) {
                    case "pick":
                        data = (from q in query
                                where q.od.isPacked != true
                                && (q.od.isPicked == false || q.od.isPacked == null)
                                select new
                                {
                                    No = q.pl.assignment_no
                                }).Distinct().ToList().Select(x => new DocumentInfoVM { No = x.No, Name = x.No.ToString() }).ToList();
                        break;
                    case "re-pick":
                        data = (from q in query
                                where q.od.isPacked != true
                                && q.od.isPicked == true
                                select new
                                {
                                    No = q.pl.assignment_no
                                }).Distinct().ToList().Select(x => new DocumentInfoVM { No = x.No, Name = x.No.ToString() }).ToList();
                        break;
                    case "pack":
                        data = (from q in query
                                where q.od.isPicked == true
                                && q.od.isProcessed == true
                                && (q.od.isScheduledForPickUp == false || q.od.isScheduledForPickUp == null)
                                select new
                                {
                                    No = q.pl.assignment_no
                                }).Distinct().ToList().Select(x => new DocumentInfoVM { No = x.No, Name = x.No.ToString() }).ToList();
                        break;
                    case "invoice":
                        data = (from q in query
                                 where q.od.isPicked == true
                                 && q.od.isProcessed == true
                                 && q.od.isPacked == true
                                 && (q.od.isShipped == false || q.od.isShipped == null)
                                 select new
                                 {
                                     No = q.pl.assignment_no
                                 }).Distinct().ToList().Select(x => new DocumentInfoVM { No = x.No, Name = x.No.ToString() }).ToList();
                        break;
                    default:
                        error = new ErrorVM { description = "The orderStage must be either pick, pack or invoice." };
                        return false;
                }
                
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }
        public List<OrderItemPickListVM> GetPickListDetailByAssignmentNo(string companyCode, string branchCode, int assignmentNo, out ErrorVM error)
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

        #endregion

        #region Item Packing
        public List<PackingItemVM> GetItemDetailsForPacking(string companyCode, string branchCode, out ErrorVM error, int? assignmentNo = null)
        {
            error = new ErrorVM();
            List<PackingItemVM> lstPackingItem = new List<PackingItemVM>();
            try {
                int _assignmentNo = Convert.ToInt32(assignmentNo);
                var data = db.GetOrderItemsForPacking(companyCode, branchCode, _assignmentNo);

                if (data != null) {
                    foreach (var d in data) {
                        PackingItemVM packingItem = new PackingItemVM();
                        packingItem.CompanyCode = companyCode;
                        packingItem.BranchCode = branchCode;
                        packingItem.OrderNo = d.order_no;
                        packingItem.OrderReferenceNo = d.order_reference_no;
                        packingItem.ItemName = d.barcode_no; //check the underlying procedure for this.
                        packingItem.BarcodeNo = d.item_name;
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
                if (data == null) {
                    error = new ErrorVM { description = "No package information found" };
                    return null;

                }
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }
        public bool AssignBarcodeToSKU(string companyCode, string branchCode, int assignmentNo, string barcodeNo, string userID, out List<OrderItemPickListVM> orderPickList, out ErrorVM error)
        {
            orderPickList = new List<OrderItemPickListVM>();
            error = null;
            try {
                var skuMaster = db.KSTU_SKU_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.SKU_ID == barcodeNo).FirstOrDefault();
                if (skuMaster != null) {
                    error = new ErrorVM { description = "Invalid barcode number." };
                    return false;
                }

                string skuFromBarcode = string.Format($"SELECT mis.ufn_GetSKUFromBarcode('{barcodeNo}','{companyCode}','{branchCode}')");
                string sku = Globals.GetScalarValue<string>(skuFromBarcode);
                if (string.IsNullOrEmpty(sku)) {
                    error = new ErrorVM { description = "Invalid barcode number. Unable to get SKU for the barcode " + barcodeNo };
                    return false;
                }

                #region Check if the SKU exist in the picklist
                var itemPickList = db.OrderItemPickLists.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.assignment_no == assignmentNo && x.isPicked == false && x.sku == sku).FirstOrDefault();
                if (itemPickList == null) {
                    error = new ErrorVM { description = "No mapping can be done for the barcode: " + barcodeNo };
                    return false;
                }
                #endregion

                #region Check if the barcode is already billed, if not, then add that to picklist
                var barcodeMast = db.KTTU_BARCODE_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.barcode_no == barcodeNo && x.sold_flag != "Y" && x.isConfirmed == "Y"
                    && (x.order_no == 0 || x.order_no == itemPickList.order_no)).FirstOrDefault();
                if (barcodeMast == null) {
                    error = new ErrorVM { description = "Barcode number is invalid or it is sold." };
                    return false;
                }
                #endregion

                var ordDet = db.KTTU_ORDER_DETAILS.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.order_no == itemPickList.order_no && x.barcode_no == sku).FirstOrDefault();
                if (ordDet == null) {
                    error = new ErrorVM { description = "Failed to get order information." };
                    return false;
                }

                itemPickList.barcode_no = barcodeNo;
                itemPickList.isPicked = true;
                itemPickList.picked_date = Globals.GetDateTime();
                itemPickList.picked_by = userID;
                db.Entry(itemPickList).State = System.Data.Entity.EntityState.Modified;

                barcodeMast.order_no = itemPickList.order_no;
                barcodeMast.UpdateOn = Globals.GetDateTime();
                db.Entry(barcodeMast).State = System.Data.Entity.EntityState.Modified;

                ordDet.item_name = barcodeNo;
                ordDet.barcode_no = sku;
                ordDet.isPicked = true;
                ordDet.picked_date = Globals.GetDateTime();
                db.Entry(ordDet).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();
                orderPickList = GetPickListItem(companyCode, branchCode, assignmentNo);
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }
        public bool UnAssignBarcodeFromSKU(OrderItemPickListVM vm, string userID, out ErrorVM error)
        {
            error = null;
            try {
                var ordDet = db.KTTU_ORDER_DETAILS.Where(x => x.company_code == vm.CompanyCode && x.branch_code == vm.BranchCode
                    && x.order_no == vm.OrderNo && x.item_name == vm.BarcodeNo).FirstOrDefault();
                if (ordDet == null) {
                    error = new ErrorVM { description = "Failed to get order information." };
                    return false;
                }
                if(ordDet.isPacked == true) {
                    error = new ErrorVM { description = "The order is already packed and hence re-picking is not permitted." };
                    return false;
                }

                #region Check if the SKU exist in the picklist
                var itemPickList = db.OrderItemPickLists.Where(x => x.company_code == vm.CompanyCode && x.branch_code == vm.BranchCode
                    && x.assignment_no == vm.AssignmentNo && x.order_no == vm.OrderNo && x.order_item_sl_no == vm.OrderItemSlno).FirstOrDefault();
                if (itemPickList == null) {
                    error = new ErrorVM { description = "Picked status is not found." };
                    return false;
                }
                #endregion

                #region Check if the barcode is linked to the same order
                var barcodeMast = db.KTTU_BARCODE_MASTER.Where(x => x.company_code == vm.CompanyCode && x.branch_code == vm.BranchCode
                    && x.barcode_no == vm.BarcodeNo).FirstOrDefault();
                if (barcodeMast == null) {
                    error = new ErrorVM { description = "Barcode number is invalid or it is not found." };
                    return false;
                }
                if(barcodeMast.sold_flag == "Y") {
                    error = new ErrorVM { description = "The barcode is already sold." };
                    return false;
                }
                if (barcodeMast.order_no != vm.OrderNo) {
                    error = new ErrorVM { description = string.Format($"The barcode {barcodeMast.barcode_no} is attached to order no. {barcodeMast.order_no} and therefore it cannot be repicked.") };
                    return false;
                }
                #endregion

                itemPickList.barcode_no = string.Empty;
                itemPickList.isPicked = false;
                itemPickList.picked_date = Globals.GetDateTime();
                itemPickList.picked_by = userID;
                db.Entry(itemPickList).State = EntityState.Modified;

                barcodeMast.order_no = 0;
                barcodeMast.UpdateOn = Globals.GetDateTime();
                db.Entry(barcodeMast).State = EntityState.Modified;

                ordDet.barcode_no = vm.SKU;
                ordDet.item_name = vm.SKU;
                ordDet.isPicked = false;
                ordDet.picked_date = Globals.GetDateTime();
                db.Entry(ordDet).State = EntityState.Modified;

                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }
        private ReceivedOrderForGridVM GetItemCode(ReceivedOrderForGridVM grid, string companyCode, string branchCode)
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
                ReceivedOrderForGridVM retData = GetItemCode(d, companyCode, branchCode);
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

        #region Create Packgage API Methods
        public bool CreatePackage(PackingItemVM packing, out ErrorVM error)
        {
            error = new ErrorVM();
            bool apiSuccessful = false;
            try {
                switch (packing.OrderSource.ToUpper()) {
                    case "AMAZON":
                        apiSuccessful = CreateAmazonPacking(packing, out error);
                        break;
                    case "BHIMA":
                        apiSuccessful = CreateOtherMarketplacePacking(packing, true, out error);
                        break;
                    case "FLIPKART":
                        apiSuccessful = CreateOtherMarketplacePacking(packing, false, out error);
                        apiSuccessful = true;
                        break;
                    default:
                        error.description = "Invalid marketplace " + packing.OrderSource.ToUpper();
                        apiSuccessful = false;
                        break;
                }

                if (!apiSuccessful) {
                    return false;
                }

                var orderDetail = db.KTTU_ORDER_DETAILS.Where(x => x.company_code == packing.CompanyCode
                    && x.branch_code == packing.BranchCode && x.order_no == packing.OrderNo).FirstOrDefault();
                if (orderDetail != null) {
                    orderDetail.isPacked = true;
                    orderDetail.packed_date = Globals.GetDateTime();
                    db.Entry(orderDetail).State = EntityState.Modified;
                }
                db.SaveChanges();

                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }
        public bool CreateAmazonPacking(PackingItemVM pack, out ErrorVM error)
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

                string marketplaceOrderStatus = GetAmazonOrderStatus(pack.OrderReferenceNo, out error);
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
        public bool CreateOtherMarketplacePacking(PackingItemVM pack, bool bhimaEComm, out ErrorVM error)
        {
            error = null;
            try {
                if (IsOrderCancelled(pack.CompanyCode, pack.BranchCode, pack.OrderNo, out error))
                    return false;

                if (bhimaEComm) {
                    OrderVM order = new OrderVM
                    {
                        branchorderno = pack.OrderNo.ToString(),
                        orderreferanceno = pack.OrderReferenceNo,
                        status = "PACKAGE_CREATED",
                        awno = "",
                        cancelled_by = "",
                        comments = ""
                    };
                    BhimaECommerce bjEcomm = new BhimaECommerce();
                    string errorMessage = string.Empty;
                    bool succeeded = bjEcomm.PostOrderStatus(pack.CompanyCode, pack.BranchCode, order, "pack", out errorMessage);
                    if (!succeeded) {
                        error = new ErrorVM { description = "Failed to update package status. Error: " + errorMessage };
                        return false;
                    }
                }
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
        #endregion

        #region Invoicing Methods accross market-places
        public bool GenerateInvoice(string companyCode, string branchCode, MarketplaceOrdersToBeProcessed vm, out DocumentInfoOutputVM invoiceOutputVM, out ErrorVM error)
        {
            error = null;
            invoiceOutputVM = null;
            if (vm == null) {
                error = new ErrorVM { description = "There is nothing to invoice." };
                return false;
            }
            bool apiSuccessful = false;
            switch (vm.OrderSource.ToUpper()) {
                case "AMAZON":
                    apiSuccessful = GenerateAmazonInvoice(companyCode, branchCode, vm, out invoiceOutputVM, out error);
                    break;
                case "BHIMA":
                    apiSuccessful = GenerateBhimaECommInvoice(companyCode, branchCode, vm, out invoiceOutputVM, out error);
                    break;
                case "FLIPKART":
                    apiSuccessful = GenerateFlipkartInvoice(companyCode, branchCode, vm, out invoiceOutputVM, out error);
                    break;
                default:
                    error.description = "Invalid marketplace " + vm.OrderSource.ToUpper();
                    apiSuccessful = false;
                    break;
            }

            return apiSuccessful;
        }
        private bool GenerateAmazonInvoice(string companyCode, string branchCode, MarketplaceOrdersToBeProcessed vm, out DocumentInfoOutputVM invoiceOutputVM, out ErrorVM error)
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

                    bool posInvoicingSucceeded = CreatePoSInvoice(vm.SKU, vm.TotalAmount, Convert.ToDecimal(vm.GSTAmt), "Y", vm.OrderNo, 0, out posInvoiceNo);
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
                    if (!GetInvoiceInPdf(companyCode, branchCode, posInvoiceNo, out htmlInvoiceData, out pdfFilePath, out errorMessage)) {
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
        private bool GenerateBhimaECommInvoice(string companyCode, string branchCode, MarketplaceOrdersToBeProcessed vm, out DocumentInfoOutputVM invoiceOutputVM, out ErrorVM error)
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
                    //1. Get Invoice HTML. 
                    //2. And write that HTML to PDF and get the pdf file path.
                    //3. Upload that pdf file to to BJEcomm
                    //4. Get byte array and send it to API.

                    //string errorMessage = string.Empty;
                    //DocumentPrintBL docPrinter = new DocumentPrintBL();
                    //string htmlInvoiceData = docPrinter.PrintSalesBillWithHeaderFooter(companyCode, branchCode, posInvoiceNo, out errorMessage);
                    //if (string.IsNullOrEmpty(htmlInvoiceData)) {
                    //    error = new ErrorVM { description = "Failed to get HTML of invoice. However invoice is generated. Error: " + errorMessage };
                    //    return false;
                    //}
                    //string invoiceMapPath = System.Web.HttpContext.Current.Request.MapPath(@"~\App_Data\invoices\");
                    //if (!Directory.Exists(invoiceMapPath))
                    //    Directory.CreateDirectory(invoiceMapPath);
                    //string pdfFilePath = string.Format($"sale_invoice-{branchCode}-{posInvoiceNo}.pdf");
                    //if(!docPrinter.ConvertHtmlToPdf(htmlInvoiceData, pdfFilePath)) {
                    //    error = new ErrorVM { description = "Unable to convert Html to Pdf." };
                    //    return false;
                    //}

                    string errorMessage = string.Empty;
                    string htmlInvoiceData = string.Empty;
                    string pdfFilePath = string.Empty;
                    if (!GetInvoiceInPdf(companyCode, branchCode, posInvoiceNo, out htmlInvoiceData, out pdfFilePath, out errorMessage)) {
                        error = new ErrorVM { description = errorMessage };
                        return false;
                    }

                    BhimaECommerce bjEcomm = new BhimaECommerce();
                    var marketplaceInvSuceeded = bjEcomm.GenerateEcomInvoice(companyCode, branchCode, vm.OrderNo.ToString(), vm.OrderReferenceNo, posInvoiceNo, pdfFilePath, out errorMessage);
                    if (!marketplaceInvSuceeded) {
                        error = new ErrorVM { description = "Failed to upload invoice to marketplace. Error: " + errorMessage };
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
                                Name = "BHIMA-eComm",
                                FileFormat = "pdf"
                            }
                        }
                    };
                }
                else {
                    //Call reprint
                    string errorMessage = string.Empty;
                    string htmlInvoiceData = string.Empty;
                    string pdfFilePath = string.Empty;
                    posInvoiceNo = orderHeader.bill_no;
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
                                Name = "BHIMA-eComm",
                                FileFormat = "pdf"
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
        private bool GenerateFlipkartInvoice(string companyCode, string branchCode, MarketplaceOrdersToBeProcessed vm, out DocumentInfoOutputVM invoiceOutputVM, out ErrorVM error)
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
        private bool GetInvoiceInPdf(string companyCode, string branchCode, int invoiceNo, out string htmlContent, out string pdfFilePath, out string errorMessage)
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
        private bool CreatePoSInvoice(string barcodeNo, decimal totalAmount, decimal gstAmount, string isInterstate, int orderNo, decimal vaAmount, out int billNo)
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
        public bool GenerateShiplabel(string companyCode, string branchCode, string userId, MarketplaceOrdersToBeProcessed vm, out DocumentInfoOutputVM shipLabelOutput, out ErrorVM error)
        {
            error = null;
            shipLabelOutput = null;
            if (vm == null) {
                error = new ErrorVM { description = "There is nothing to invoice." };
                return false;
            }
            bool apiSuccessful = false;
            switch (vm.OrderSource.ToUpper()) {
                case "AMAZON":
                    apiSuccessful = GenerateShiplabelForAmazon(companyCode, branchCode, userId, vm, out shipLabelOutput, out error);
                    break;
                case "BHIMA":
                    apiSuccessful = GenerateShiplabelForBhimaEcom(companyCode, branchCode, userId, vm, out shipLabelOutput, out error);
                    break;
                case "FLIPKART":
                    apiSuccessful = GenerateShiplabelForFlipkart(companyCode, branchCode, userId, vm, out shipLabelOutput, out error);
                    break;
                default:
                    error.description = "Invalid marketplace " + vm.OrderSource.ToUpper();
                    apiSuccessful = false;
                    break;
            }

            return apiSuccessful;
        }
        private bool GenerateShiplabelForAmazon(string companyCode, string branchCode, string userID, MarketplaceOrdersToBeProcessed vm, out DocumentInfoOutputVM shiplabelOutput, out ErrorVM error)
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
        private bool GenerateShiplabelForBhimaEcom(string companyCode, string branchCode, string userID, MarketplaceOrdersToBeProcessed vm, out DocumentInfoOutputVM shipLabelOutput, out ErrorVM error)
        {
            error = null;
            shipLabelOutput = null;
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
                        shipLabelOutput = GetShipLabelContent(vm.OrderNo, orderShipment.ShipLabelPDFObject, "Bhima-Ship label");
                    }
                    return true;
                }

                BhimaECommerce bjEcomm = new BhimaECommerce();
                string statusCode = string.Empty, statusInfo = string.Empty, awbNo = string.Empty, printContent = string.Empty;
                string isPickRegistered = string.Empty;
                string pickUpTokenNo = string.Empty;
                //bool isError;
                //bool success = bjEcomm.GetWaybillGenerationResponse(db, companyCode, branchCode, vm.OrderNo.ToString(), out statusCode, out statusInfo, out awbNo, out printContent, out isPickRegistered, out pickUpTokenNo, out isError);
                //if (statusCode != "Valid") {
                //    error = new ErrorVM { description = "Failed to generate the Ship Label at Blue Dart. Error status code: " + statusCode + "; Error Info: " + statusInfo };
                //    return false;
                //}

                ShipmentVM shipmentObject = null;
                string errorMessage = string.Empty;
                bool succeeded = bjEcomm.GenerateBluedartEwayBill(companyCode, branchCode, vm.OrderNo.ToString(), out shipmentObject, out errorMessage);
                if (!succeeded) {
                    error = new ErrorVM { description = "Failed to generate the Ship Label at Blue Dart. Error: " + errorMessage };
                    return false;
                }
                awbNo = shipmentObject.AWBNo;
                printContent = shipmentObject.AWBPdfPrintContent;
                pickUpTokenNo = shipmentObject.PickUpTokenNo;

                OrderVM order = new OrderVM
                {
                    branchorderno = vm.OrderNo.ToString(),
                    orderreferanceno = vm.OrderReferenceNo,
                    status = "SHIPLABEL_GENERATED",
                    awno = awbNo,
                    cancelled_by = "",
                    comments = ""
                };
                shipLabelOutput = GetShipLabelContent(vm.OrderNo, printContent, "Bhima-Ship label");

                OrderItemShipmentLabel shipment = new OrderItemShipmentLabel();
                shipment.company_code = companyCode;
                shipment.branch_code = branchCode;
                shipment.order_ref_no = vm.OrderReferenceNo;
                shipment.order_no = vm.OrderNo;
                shipment.bill_no = vm.BillNo;
                shipment.shipLabel = awbNo;
                shipment.operator_code = userID;
                shipment.ShipLabelPDFObject = printContent;
                shipment.shipLabelCreatedDate = Globals.GetDateTime();
                shipment.isScheduledforPickUp = isPickRegistered;
                shipment.PickUpTokenNo = pickUpTokenNo;

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
        private bool GenerateShiplabelForFlipkart(string companyCode, string branchCode, string userID, MarketplaceOrdersToBeProcessed vm, out DocumentInfoOutputVM shipLabelOutput, out ErrorVM error)
        {
            error = null;
            shipLabelOutput = null;
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
                        shipLabelOutput = GetShipLabelContent(vm.OrderNo, orderShipment.ShipLabelPDFObject, "Bhima-Ship label");
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
        private DocumentInfoOutputVM GetShipLabelContent(int orderNo, string shipLabelContent, string contentName)
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
        public bool ShipOrder(ShipOrderInput shipOrderInput, string userId, out List<ErrorVM> errorList)
        {
            errorList = new List<ErrorVM>();
            if (shipOrderInput == null) {
                errorList.Add(new ErrorVM { description = "There are no orders to ship." });
                return false;
            }
            if (shipOrderInput.OrderList == null || shipOrderInput.OrderList.Count <= 0) {
                errorList.Add(new ErrorVM { description = "Order list to ship is not available." });
                return false;
            }
            string companyCode = shipOrderInput.CompanyCode;
            string branchCode = shipOrderInput.BranchCode;
            int successApiCalls = 0;
            int failedApiCalls = 0;
            string marketPlace = string.Empty;

            ErrorVM error = null;
            int shipmentNo = PostShipmentMaster(marketPlace, shipOrderInput, userId, out error);
            if (shipmentNo <= 0) {
                var err = new ErrorVM { description = "Failed to post to shipment master. Error: " + error.description };
                errorList.Add(error);
                return false;
            }

            foreach (var order in shipOrderInput.OrderList) {
                error = null;
                marketPlace = order.OrderSource.ToUpper();
                switch (marketPlace) {
                    case "AMAZON":
                        if (!ShipAmazonOrder(companyCode, branchCode, order, shipmentNo, out error)) {
                            failedApiCalls++;
                            errorList.Add(error);
                        }
                        else {
                            successApiCalls++;
                        }
                        break;
                    case "BHIMA":
                        if (!ShipOtherMarketplaceOrder(companyCode, branchCode, order, shipmentNo, isBJECommOrder: true, error: out error)) {
                            failedApiCalls++;
                            errorList.Add(error);
                        }
                        else {
                            successApiCalls++;
                        }
                        break;
                    case "FLIPKART":
                        if (!ShipOtherMarketplaceOrder(companyCode, branchCode, order, shipmentNo, isBJECommOrder: false, error: out error)) {
                            failedApiCalls++;
                            errorList.Add(error);
                        }
                        else {
                            successApiCalls++;
                        }
                        break;
                }
            }

            if (failedApiCalls > 0) {
                return false;
            }
            #region Commented code
            //int errorCount = 0;
            //List<string> errorList = new List<string>();
            //try {
            //    foreach(var order in ordersToBeShpped) {
            //        var orderDetail = db.KTTU_ORDER_DETAILS.Where(x => x.company_code == companyCode && x.branch_code == branchCode
            //        && x.order_no == order.OrderNo).FirstOrDefault();
            //        if (orderDetail == null) {
            //            errorList.Add(string.Format($"Order No. {order.OrderNo} is not found."));
            //            errorCount++;
            //            continue;
            //        }
            //        var packageHeader = db.PackageHeaders.Where(x => x.company_code == companyCode && x.branch_code == branchCode
            //            && x.OrderID == order.OrderNo).FirstOrDefault();
            //        if (packageHeader == null) {
            //            errorList.Add(string.Format($"Package details for order No. {order.OrderNo} is not found."));
            //            errorCount++;
            //            continue;
            //        }
            //        string mpOrderStatus = GetAmazonOrderStatus(packageHeader.MarketPlaceOrderID, out error);
            //        if (string.IsNullOrEmpty(mpOrderStatus)) {
            //            errorList.Add(string.Format($"Unable to get status for order No. {order.OrderNo} and therefore cannot proceed further."));
            //            errorCount++;
            //            continue;
            //        }
            //        switch (mpOrderStatus) {
            //            case "SHIPPED":
            //            case "DELIVERED":
            //                errorList.Add(string.Format($"The Order #{order.OrderNo} is already shipped. The same is updated."));
            //                UpdateShippedStatus(orderDetail);
            //                errorCount++;
            //                continue;
            //            case "SHIPLABEL_CREATED":
            //                AmazonMarketplace amp = new AmazonMarketplace();
            //                ErrorInfo errorInfo = null;
            //                if(!amp.Ship("Amazon", packageHeader.MarketPlaceOrderID, out errorInfo)) {
            //                    errorList.Add(string.Format($"Failed to ship order {order.OrderNo}. Error: {errorInfo.Description}"));
            //                    errorCount++;
            //                    continue;
            //                }
            //                break;
            //            default:
            //                errorList.Add(string.Format($"The status of the order #{order.OrderNo} must be SHIPLABEL_CREATED to enable shipping. But the present status is {mpOrderStatus}."));
            //                errorCount++;
            //                continue;
            //        }
            //    }

            //}
            //catch (Exception ex) {
            //    error = new ErrorVM().GetErrorDetails(ex);
            //    return false;
            //} 
            #endregion
            return true;
        }
        private bool ShipOtherMarketplaceOrder(string companyCode, string branchCode, MarketplaceOrdersToBeProcessed marketPlaceOrder, int shipmentNo, bool isBJECommOrder, out ErrorVM error)
        {
            error = null;
            if (marketPlaceOrder == null) {
                error = new ErrorVM { description = "Invalid order details." };
                return false;
            }
            try {
                var orderDetail = db.KTTU_ORDER_DETAILS.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                && x.order_no == marketPlaceOrder.OrderNo).FirstOrDefault();
                if (orderDetail == null) {
                    error = new ErrorVM { field = marketPlaceOrder.OrderNo.ToString(), description = string.Format($"Order No. {marketPlaceOrder.OrderNo} is not found.") };
                    return false;
                }
                if (orderDetail.isScheduledForPickUp != true) {
                    error = new ErrorVM { field = marketPlaceOrder.OrderNo.ToString(), description = string.Format($"Order No. {marketPlaceOrder.OrderNo} is not ready to be shipped.") };
                    return false;

                }
                string awbNo = string.Empty;
                var orderItemShipmentUpdate = db.OrderItemShipmentLabels.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.order_no == marketPlaceOrder.OrderNo).FirstOrDefault();
                if (orderItemShipmentUpdate != null)
                    awbNo = orderItemShipmentUpdate.shipLabel;

                string packageId = string.Empty;
                var packageHeader = db.PackageHeaders.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.OrderID == marketPlaceOrder.OrderNo).FirstOrDefault();
                if (packageHeader != null) {
                    packageId = packageHeader.PackageID;
                }

                OrderVM order = new OrderVM
                {
                    branchorderno = marketPlaceOrder.OrderNo.ToString(),
                    orderreferanceno = marketPlaceOrder.OrderReferenceNo,
                    status = "SHIPPED",
                    awno = awbNo,
                    cancelled_by = "",
                    comments = ""
                };
                #region In case of BJ Ecomm order, an API to be called to update order status
                if (isBJECommOrder) {
                    BhimaECommerce bjEcomm = new BhimaECommerce();
                    string errorMessage = string.Empty;
                    bool succeeded = bjEcomm.PostOrderStatus(companyCode, branchCode, order, "dispatch", out errorMessage);
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
                error.field = marketPlaceOrder.OrderNo.ToString();
                return false;
            }
            return true;
        }
        private bool ShipAmazonOrder(string companyCode, string branchCode, MarketplaceOrdersToBeProcessed order, int shipmentNo, out ErrorVM error)
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
                string mpOrderStatus = GetAmazonOrderStatus(packageHeader.MarketPlaceOrderID, out error);
                if (string.IsNullOrEmpty(mpOrderStatus)) {
                    error = new ErrorVM { field = order.OrderNo.ToString(), description = string.Format($"Unable to get status for order No. {order.OrderNo} and therefore cannot proceed further. API response: {error.description}") };
                    return false;
                }
                switch (mpOrderStatus) {
                    case "SHIPPED":
                    case "DELIVERED":
                        error = new ErrorVM { field = order.OrderNo.ToString(), description = string.Format($"The Order #{order.OrderNo} is already shipped. The same is updated.") };
                        UpdateShippedStatus(orderDetail, shipmentNo, packageHeader.PackageID);
                        return false;
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
        private int PostShipmentMaster(string marketPlace, ShipOrderInput shipOrderInput, string userId, out ErrorVM error)
        {
            error = null;
            int shipmentNo = 0;
            try {
                shipmentNo = Globals.GetDocumentNo(db, shipOrderInput.CompanyCode, shipOrderInput.BranchCode, "383", prefixFinYear: true);
                ShipmentMaster sm = new ShipmentMaster
                {
                    company_code = shipOrderInput.CompanyCode,
                    branch_code = shipOrderInput.BranchCode,
                    cflag = "N",
                    logistics_code = marketPlace,
                    logistic_person_mobile = shipOrderInput.PickupAgentMobileNo,
                    logistic_person_name = shipOrderInput.PickupAgentName,
                    operator_code = userId,
                    remarks = shipOrderInput.PickupRemarks,
                    MarketPlaceCode = marketPlace,
                    shiped_date = Globals.GetApplicationDate(),
                    shipment_no = shipmentNo,
                    cancelled_date = Globals.GetApplicationDate()
                };
                db.ShipmentMasters.Add(sm);
                //foreach(var line in shipOrderInput.OrderList) {
                //    ShipmentDetail sd = new ShipmentDetail
                //    {
                //        company_code = shipOrderInput.CompanyCode,
                //        branch_code = shipOrderInput.BranchCode,
                //        shipment_no = shipmentNo,
                //        order_no = line.OrderNo,
                //        barcode_no = line.SKU,
                //        item_sl_no = 1,
                //        shipment_id = "NA",
                //        package_id = "NA"
                //    };
                //    db.ShipmentDetails.Add(sd);
                //}
                Globals.IncrementDocumentNo(db, shipOrderInput.CompanyCode, shipOrderInput.BranchCode, "383");
                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
            }
            return shipmentNo;
        }
        private void UpdateShippedStatus(KTTU_ORDER_DETAILS orderDetail, int shipmentNo, string packageId)
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

        public bool CancelOrder(string companyCode, string branchCode, int marketplaceID, int orderNo, string userId, string cancelRemarks, out ErrorVM error)
        {
            error = null;
            string marketplaceOrderNo = string.Empty;
            cancelRemarks = cancelRemarks == null ? string.Empty : cancelRemarks;
            #region Get Marketplace Name from Marketplace Id
            var mp = GetMarketPlaces().Where(m => m.Id == marketplaceID).FirstOrDefault();
            if (mp == null) {
                error = new ErrorVM { description = "No marketplace found for the selected marketplace Id" + marketplaceID.ToString() };
                return false;
            }
            string marketplaceName = mp.Name.ToUpper();
            #endregion

            #region Validate order
            if (!ValidateOrderTobeCancelled(companyCode, branchCode, marketplaceID, orderNo, out marketplaceOrderNo, out error))
                return false;
            if (string.IsNullOrEmpty(marketplaceOrderNo)) {
                error = new ErrorVM { description = "Failed to fetch marketplace order reference number." };
                return false;
            }
            if (string.IsNullOrEmpty(marketplaceOrderNo)) {
                error = new ErrorVM { description = "Unable to get marketplace order reference number." };
                return false;
            }
            #endregion
            try {
                bool apiCallSuccessfull = false;
                switch (marketplaceName) {
                    case "AMAZON":
                        apiCallSuccessfull = CancelAmazonOrder(companyCode, branchCode, orderNo, marketplaceOrderNo, userId, cancelRemarks, out error);
                        break;
                    case "BHIMA":
                        apiCallSuccessfull = CancelBhimaEcommerceOrder(companyCode, branchCode, orderNo, marketplaceOrderNo, userId, cancelRemarks, out error);
                        break;
                    case "FLIPKART":
                        apiCallSuccessfull = true;
                        break;
                    default:
                        error = new ErrorVM { description = "No marketplace found for the selected marketplace Id" + marketplaceID.ToString() };
                        return false;
                }
                if (!apiCallSuccessfull)
                    return false;
                bool dbUpdationSuccessfull = ExecuteCancelOrderProc(companyCode, branchCode, marketplaceID, orderNo, userId, cancelRemarks, out error);
                if (!dbUpdationSuccessfull) {
                    var errorMsg = "Order cancelled at marketplace. But database updation failed and returned the error. " + error.description
                        + " This might lead to inconsitency. Please contact your admistrator.";
                    error = new ErrorVM { description = errorMsg };
                    return false;
                }
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        private bool CancelAmazonOrder(string companyCode, string branchCode, int orderNo, string marketplaceOrderNo, string userId, string cancelRemarks, out ErrorVM error)
        {
            error = null;
            string orderStatusAtMarketplace = GetAmazonOrderStatus(marketplaceOrderNo, out error);
            if (string.IsNullOrEmpty(orderStatusAtMarketplace)) {
                error = new ErrorVM
                {
                    description = "Failed to get marketplace order status. Please contact administrator."
                    + error != null ? " Error: " + error.description : string.Empty
                };
                return false;
            }
            if(orderStatusAtMarketplace == "CANCELLED") {
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

        private bool CancelBhimaEcommerceOrder(string companyCode, string branchCode, int storeOrderNo, string marketplaceOrderNo, string userId, string cancelRemarks, out ErrorVM error)
        {
            error = null;
            OrderVM order = new OrderVM
            {
                branchorderno = storeOrderNo.ToString(),
                orderreferanceno = marketplaceOrderNo,
                status = "CANCELLED",
                awno = "",
                cancelled_by = userId,
                comments = cancelRemarks
            };

            BhimaECommerce bjEcomm = new BhimaECommerce();
            string errorMessage = string.Empty;
            bool apiCallSuceeded = bjEcomm.PostOrderStatus(companyCode, branchCode, order, "Cancel", out errorMessage);
            if (!apiCallSuceeded) {
                error = new ErrorVM { description = "Failed to cancel order. Error: " + errorMessage };
                return false;
            }
            return apiCallSuceeded;
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
                if(orderInfo == null) {
                    error = new ErrorVM { description = "No orders found (for cancellation) for the marketplace." };
                    return false;
                }
                if(orderInfo.CancelFlag == "Y") {
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
        private string GetAmazonOrderStatus(string marketplaceOrderId, out ErrorVM error)
        {
            error = null;
            string orderStatus = null;
            ErrorInfo apiError = new ErrorInfo();
            try {
                orderStatus = amazon.GetOrderStatus(marketplaceOrderId, out apiError);
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
        private bool IsOrderCancelled(string companyCode, string branchCode, int orderNo, out ErrorVM error)
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
        private bool IsOrderInvoiced(string companyCode, string branchCode, int orderNo, out ErrorVM error)
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
                if(shipment == null) {
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
