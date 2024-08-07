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
    public class OnlineMarketplaceBL
    {
        MagnaDbEntities db;
        public OnlineMarketplaceBL()
        {
            db = new MagnaDbEntities(true);
        }

        public OnlineMarketplaceBL(MagnaDbEntities dbContext)
        {
            this.db = dbContext;
        }
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
                                  //&& orderSource.AsQueryable().Contains(om.order_source)
                                  && om.order_source == marketPlaceID
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
        public bool GetOrderListToBeCancelled(string companyCode, string branchCode, int marketplaceID, out List<ListOfValue> ordersCanBeCancelled, out ErrorVM error)
        {
            error = null;
            ordersCanBeCancelled = null;
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
                    d.Status = "Cancelled";
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
                else if (d.OrderSourceMarket == 3) {
                    d.OrderSource = "Bhima";
                }
                else {
                    d.OrderSource = "Flipkart";
                }
            }
            return newData;
        }

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
                    orderDetail.isPicked = false;
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

        #region Packing Info
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

        #region Barcode assignment & un-assignment
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
                if (ordDet.isPacked == true) {
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
                if (barcodeMast.sold_flag == "Y") {
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
        #endregion        

        public bool CreatePackage(PackingItemVM packing, out ErrorVM error)
        {
            error = new ErrorVM();
            IMarketplace marketplace = new OnlineMarketplaceFactory().GetInstanceOf(packing.OrderSource.ToUpper(), db);
            try {
                if(marketplace == null) {
                    error.description = "Invalid marketplace " + packing.OrderSource.ToUpper();
                    return false;
                }
                if(!marketplace.CreateMarketplacePackage(packing, out error)) {
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
        public bool GenerateInvoice(string companyCode, string branchCode, MarketplaceOrdersToBeProcessed vm, out DocumentInfoOutputVM invoiceOutputVM, out ErrorVM error)
        {
            error = null;
            invoiceOutputVM = null;
            if (vm == null) {
                error = new ErrorVM { description = "There is nothing to invoice." };
                return false;
            }
            IMarketplace marketplace = new OnlineMarketplaceFactory().GetInstanceOf(vm.OrderSource.ToUpper(), db);
            if (marketplace == null) {
                error = new ErrorVM { description = "Invalid marketplace " + vm.OrderSource.ToUpper() };
                return false;
            }

            return marketplace.GenerateMarketplaceInvoice(companyCode, branchCode, vm, out invoiceOutputVM, out error);
        }
        public bool GenerateShiplabel(string companyCode, string branchCode, string userId, MarketplaceOrdersToBeProcessed vm, out DocumentInfoOutputVM shipLabelOutput, out ErrorVM error)
        {
            error = null;
            shipLabelOutput = null;
            if (vm == null) {
                error = new ErrorVM { description = "There is nothing to invoice." };
                return false;
            }
            IMarketplace marketplace = new OnlineMarketplaceFactory().GetInstanceOf(vm.OrderSource.ToUpper(), db);
            if (marketplace == null) {
                error = new ErrorVM { description = "Invalid marketplace " + vm.OrderSource.ToUpper() };
                return false;
            }

            return marketplace.GenerateMarketplaceShiplabel(companyCode, branchCode, userId, vm, out shipLabelOutput, out error);
        }
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
            string marketPlaceCode = shipOrderInput.OrderList[0].OrderSource.ToUpper();
            
            ErrorVM error = null;
            int shipmentNo = PostShipmentMaster(marketPlaceCode, shipOrderInput, userId, out error);
            if (shipmentNo <= 0) {
                var err = new ErrorVM { description = "Failed to post to shipment master. Error: " + error.description };
                errorList.Add(error);
                return false;
            }

            IMarketplace marketplace = new OnlineMarketplaceFactory().GetInstanceOf(marketPlaceCode, db);
            if (marketplace == null) {
                errorList.Add(new ErrorVM { description = "Invalid marketplace " + marketPlaceCode });
                return false;
            }

            foreach (var order in shipOrderInput.OrderList) {
                error = null;
                if (!marketplace.ShipMarketplaceOrder(companyCode, branchCode, order, shipmentNo, out error)) {
                    failedApiCalls++;
                    errorList.Add(error);
                }
                else {
                    successApiCalls++;
                }
            }

            if (failedApiCalls > 0) {
                return false;
            }
            return true;
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
                Globals.IncrementDocumentNo(db, shipOrderInput.CompanyCode, shipOrderInput.BranchCode, "383");
                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
            }
            return shipmentNo;
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
            #endregion
            try {
                IMarketplace marketplace = new OnlineMarketplaceFactory().GetInstanceOf(marketplaceName, db);
                bool success = marketplace.CancelMarketplaceOrder(companyCode, branchCode, marketplaceID, orderNo, marketplaceOrderNo, userId, cancelRemarks, out error);
                if (!success)
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
    }
}
