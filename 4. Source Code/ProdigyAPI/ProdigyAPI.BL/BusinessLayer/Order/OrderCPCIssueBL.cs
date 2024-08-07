using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.Model.MagnaDb;
using ProdigyAPI.SIGlobals;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Order
{
    public class OrderCPCIssueBL
    {
        MagnaDbEntities db = null;
        public OrderCPCIssueBL()
        {
            db = new MagnaDbEntities(true);
        }

        public OrderCPCIssueBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }

        public List<ListOfValue> GetMetalCodes(string companyCode, string branchCode)
        {
            var metals = db.KSTU_METALS_MASTER.Where(m => m.company_code == companyCode && m.branch_code == branchCode
                && (m.metal_code == "GL" || m.metal_code == "SL" || m.metal_code == "PT")).Select(x => new ListOfValue { Code = x.metal_code, Name = x.metal_name }).ToList();
            return metals;

        }

        public List<ListOfValue> GetGSForMetalCode(string companyCode, string branchCode, string metalCode)
        {
            var gs = db.KSTS_GS_ITEM_ENTRY.Where(g => g.company_code == companyCode && g.branch_code == branchCode
                && g.bill_type == "S" && g.object_status != "C"
                && (g.measure_type == "W" || g.measure_type == "P"));
            if (metalCode == "ALL" || string.IsNullOrEmpty(metalCode))
                return gs.Select(x => new ListOfValue
                {
                    Code = x.gs_code,
                    Name = x.item_level1_name
                }).Distinct().ToList();
            else
                return gs.Where(x => x.metal_type == metalCode)
                .Select(x => new ListOfValue
                {
                    Code = x.gs_code,
                    Name = x.item_level1_name
                }).Distinct().ToList();
        }

        public bool GetOrderDetail(string companyCode, string branchCode, string orderType, string gsCode,
            string counterCode, string karat, out List<OrderCPCIssueLineVM> orderList, out string errorMessage)
        {
            errorMessage = string.Empty;
            orderList = new List<OrderCPCIssueLineVM>();
            //var orderItems = from om in db.KTTU_ORDER_MASTER
            //                 join od in db.KTTU_ORDER_DETAILS
            //                 on new {CC = om.company_code, BC = om.branch_code, OrderNo = om.order_no}
            //                 equals new { CC = od.company_code, BC = od.branch_code, OrderNo = od.order_no }
            //                 where om.company_code + "$" + om.branch_code + "$" + om.order_no.ToString()
            try {
                DateTime startDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode).Date;
                DateTime endDate = startDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                var orderItems = db.usp_getorderissuedetails(startDate, endDate, companyCode, branchCode, gsCode, orderType, counterCode, karat).ToList();
                if (orderItems == null) {
                    errorMessage = "No orders found for the selected criteria.";
                    return false;
                }
                orderList = orderItems.Select(od => new OrderCPCIssueLineVM
                {
                    OrderNo = od.order_no,
                    SubOrderNo = od.slno,
                    GSCode = od.gs_code,
                    Counter = od.counter_code,
                    Item = od.item_name,
                    Description = od.description,
                    CustomerName = od.cust_name,
                    MobileNo = od.mobile_no,
                    DueDate = od.delivery_date.Date,
                    Qty = od.quantity,
                    AdvanceAmount = Convert.ToDecimal(od.advance_ord_amount),
                    ItemType = od.item_type,
                    OrderDate = od.orderdate,
                    Salesman = od.sal_code,
                    Rate = Convert.ToDecimal(od.order_day_rate),
                    Karat = od.karat,
                    FromWeight = od.from_gwt,
                    ToWeight = od.to_gwt,
                    DiaCarets = od.dcts,
                    StoneWeight = od.swt,
                    NetWeight = od.from_gwt
                }).ToList();
            }
            catch (Exception ex) {
                errorMessage = new ErrorVM().GetErrorDetails(ex).customDescription;
                return false;
            }

            return true;
        }

        public bool SaveOrderIssue(OrderCPCIssuesVM orderIssue, string userID, out int documentNo, out string errorMessage)
        {
            errorMessage = string.Empty;
            documentNo = 0;
            if (orderIssue == null) {
                errorMessage = "Nothing to save.";
                return false;
            }
            if (orderIssue.issueLines == null || orderIssue.issueLines.Count <= 0) {
                errorMessage = "There is no line detail to save.";
                return false;
            }
            string companyCode = orderIssue.CompanyCode;
            string branchCode = orderIssue.BranchCode;
            try {
                var applicationDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);

                List<OrderCPCIssueLineVM> dbOrderLines = null;
                var isSuccess = GetOrderDetail(orderIssue.CompanyCode, orderIssue.BranchCode, "O", orderIssue.GSCode, orderIssue.Counter,
                    orderIssue.Karat, out dbOrderLines, out errorMessage);
                if (!isSuccess)
                    return false;
                if (dbOrderLines == null) {
                    errorMessage = "No order data available for the selected critieria. Please reload and submit again.";
                    return false;
                }

                //Revalidate and check if the batch is valid.
                documentNo = SIGlobals.Globals.GetDocumentNo(db, companyCode, branchCode, "28", true);
                string masterObjId = SIGlobals.Globals.GetMagnaGUID("KTTU_ORDER_PRODUCTION_ISSUE_DETAILS", documentNo, companyCode, branchCode);

                int slNo = 1;
                foreach (var line in orderIssue.issueLines) {
                    var od = dbOrderLines.Where(x => x.OrderNo == line.OrderNo && x.SubOrderNo == line.SubOrderNo).FirstOrDefault();
                    if (od == null) {
                        errorMessage = string.Format("No order data available the Order No. {0}/Sub-Order No. {1}.", line.OrderNo, line.SubOrderNo);
                        return false;
                    }
                    KTTU_ORDER_PRODUCTION_ISSUE_DETAILS opi = new KTTU_ORDER_PRODUCTION_ISSUE_DETAILS
                    {
                        obj_id = masterObjId,
                        issue_no = documentNo,
                        slno = slNo,
                        order_no = od.OrderNo,
                        company_code = companyCode,
                        branch_code = branchCode,
                        order_date = od.OrderDate,
                        order_due_date = od.DueDate,
                        order_gwt = od.FromWeight,
                        order_qty = od.Qty,
                        branch_due_date = od.DueDate,
                        cflag = "N",
                        description = od.Description,
                        gs_code = od.GSCode,
                        img_id = "",
                        issue_date = applicationDate,
                        issued_by = orderIssue.IssuedUser,
                        item_name = od.Item,
                        operator_code = userID,
                        remarks = od.Remarks == null ? string.Empty : od.Remarks,
                        karat = od.Karat,
                        rate = od.Rate,
                        order_type = orderIssue.OrderType,
                        salesman = od.Salesman,
                        counter = od.Counter,
                        advanceamount = od.AdvanceAmount,
                        sampleitem = "",
                        order_swt = od.StoneWeight,
                        orderDCarrat = od.DiaCarets,
                        order_nwt = od.FromWeight,
                        item_size = "NA",
                        refNo = 0,
                        order_slno = od.SubOrderNo,
                        UniqRowID = Guid.NewGuid(),
                        item_type = od.ItemType,
                        app_swt = 0,
                        cdate = applicationDate,
                        isReceiveatCPC = "N",
                        issue_to = "CPC",
                        mobile_no = od.MobileNo,
                        cust_name = od.CustomerName
                    };
                    slNo++;
                    db.KTTU_ORDER_PRODUCTION_ISSUE_DETAILS.Add(opi);
                }
                SIGlobals.Globals.IncrementDocumentNo(db, companyCode, branchCode, "28");

                db.SaveChanges();
            }
            catch (Exception ex) {
                errorMessage = new ErrorVM().GetErrorDetails(ex).customDescription;
                return false;
            }
            return true;
        }

        public bool GenerateXMLFile(string companyCode, string branchCode, int issueNo, out string errorMessage)
        {
            errorMessage = string.Empty;
            try {
                DataSet dsOrderIssue = new DataSet();
                DataTable dtIssueMaster = Globals.GetDataTable(string.Format("SELECT * FROM KTTU_ORDER_PRODUCTION_ISSUE_DETAILS AS im WHERE im.company_code = '{0}' AND im.branch_code = '{1}' AND im.issue_no = '{2}' AND cflag !='Y'",
                    companyCode, branchCode, issueNo));
               
                dtIssueMaster.TableName = "KTTU_ORDER_PRODUCTION_ISSUE_DETAILS";

                string issueTo = string.Empty;
                if (dtIssueMaster != null) {
                    dsOrderIssue.Tables.Add(dtIssueMaster);
                    issueTo = dtIssueMaster.Rows[0]["issue_to"].ToString();
                }

                string fpath = string.Format(@"~\App_Data\" + @"Xmls\{0}{1}{2}{3}{4}{5}", "ORDERIssueXML_",
                    companyCode, branchCode, issueTo, issueNo, ".xml");
                string filePath = System.Web.HttpContext.Current.Request.MapPath(fpath);
                string folderPath = string.Format(@"~\App_Data" + @"\Xmls");
                Globals.CreateDirectoryIfNotExist(System.Web.HttpContext.Current.Request.MapPath(folderPath));

                if (System.IO.File.Exists(filePath)) {
                    File.SetAttributes(filePath, FileAttributes.Normal);
                    FileInfo file = new FileInfo(filePath);
                    file.Delete();
                }
                dsOrderIssue.WriteXml(filePath, XmlWriteMode.WriteSchema);
                File.SetAttributes(filePath, FileAttributes.ReadOnly);
                if (Globals.Upload(filePath, 1, out errorMessage)) {
                    return true;
                }
                else {
                    return false;
                }
            }
            catch (Exception ex) {
                errorMessage = ex.Message;
                return false;
            }
        }
    }
}
