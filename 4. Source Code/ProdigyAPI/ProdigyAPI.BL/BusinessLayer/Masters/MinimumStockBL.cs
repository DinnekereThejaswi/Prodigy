using ProdigyAPI.BL.BusinessLayer.PrintConfig;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Print;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Author: Mustureswara M M (Eshwar)
/// Date: 02-06-2021
/// </summary>
namespace ProdigyAPI.BL.BusinessLayer.Masters
{
    /// <summary>
    /// This Business Layer Provides Methods related to Minimum Stock Master (Reorder List Master or ROL Master)
    /// </summary>
    public class MinimumStockBL
    {
        #region Declaration
        MagnaDbEntities dbContext = null;
        #endregion

        #region Constructor
        public MinimumStockBL()
        {
            dbContext = new MagnaDbEntities();
        }
        public MinimumStockBL(MagnaDbEntities db)
        {
            dbContext = db;
        }
        #endregion

        #region Methods
        public dynamic GetGS(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                string[] measureType = { "P", "W" };
                var data = dbContext.KSTS_GS_ITEM_ENTRY.Where(gs => gs.company_code == companyCode
                                                                && gs.branch_code == branchCode
                                                                && gs.object_status == "O"
                                                                && gs.bill_type == "S"
                                                                && measureType.Contains(gs.measure_type))
                                                                .OrderBy(gs => gs.display_order)
                                                                .Select(gs => new
                                                                {
                                                                    Name = gs.item_level1_name,
                                                                    Code = gs.gs_code
                                                                }).ToList();
                return data;
            }
            catch (Exception excp) {

                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }

        }

        public dynamic GetItem(string companyCode, string branchCode, string gsCode, out ErrorVM error)
        {
            error = null;
            try {
                var data = dbContext.ITEM_MASTER.Where(item => item.company_code == companyCode
                                                       && item.branch_code == branchCode
                                                       && item.gs_code == gsCode).Select(item => new
                                                       {
                                                           Code = item.Item_code,
                                                           Name = item.Item_Name
                                                       }).OrderBy(ord => ord.Name).ToList();
                return data;
            }
            catch (Exception excp) {

                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public dynamic GetDesigns(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                var data = dbContext.KTTU_BARCODE_MASTER.Where(item => item.company_code == companyCode
                                                      && item.branch_code == branchCode).Select(item => new
                                                      {
                                                          Code = item.design_no,
                                                          Name = item.design_name
                                                      }).Distinct().ToList();
                return data;
            }
            catch (Exception excp) {

                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public dynamic GetCounter(string companyCode, string branchCode, string gsCode, string itemCode, out ErrorVM error)
        {
            error = null;
            try {
                var kil = dbContext.KSTS_ITEM_COUNTER_LIST.Where(il => il.company_code == companyCode
                                                                    && il.branch_code == branchCode
                                                                    && il.gs_code == gsCode && il.item_name == itemCode).Select(il => il.counter_code).ToList();

                var data = dbContext.KSTU_COUNTER_MASTER.Where(cntr => cntr.company_code == companyCode
                                                      && cntr.branch_code == branchCode
                                                      && kil.Contains(cntr.counter_code)).Select(cntr => new
                                                      {
                                                          Code = cntr.counter_code,
                                                          Name = cntr.counter_name
                                                      }).Distinct().ToList();
                return data;
            }
            catch (Exception excp) {

                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public IEnumerable<MinStcokVM> GetMinStock(string companyCode, string branchCode, string gsCode, string itemCode, out ErrorVM error)
        {
            error = null;
            try {
                IEnumerable<MinStcokVM> data = dbContext.KSTU_MIN_STOCK_MASTER.Where(cntr => cntr.company_code == companyCode
                                                      && cntr.branch_code == branchCode
                                                      && cntr.gs_code == gsCode
                                                      && cntr.category_code == itemCode).Select(cntr => new MinStcokVM()
                                                      {
                                                          ObjID = cntr.obj_id,
                                                          CompanyCode = cntr.company_code,
                                                          BranchCode = cntr.branch_code,
                                                          GSCode = cntr.gs_code,
                                                          CategoryCode = cntr.category_code,
                                                          CategoryName = cntr.category_name,
                                                          DesignCode = cntr.design_code,
                                                          DesignName = cntr.design_name,
                                                          CounterCode = cntr.counter_code,
                                                          CounterName = cntr.counter_name,
                                                          MinQty = cntr.min_qty,
                                                          MinWt = cntr.min_wt,
                                                          MaxQty = cntr.max_qty,
                                                          MaxWt = cntr.max_wt,
                                                          IsSved = cntr.is_saved,
                                                      }).ToList();
                return data;
            }
            catch (Exception excp) {

                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public bool Save(MinStcokVM rolm, out ErrorVM error)
        {
            error = null;
            if (rolm == null) {
                error = new ErrorVM()
                {
                    description = "Invalid Data",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            KSTU_MIN_STOCK_MASTER existMinStock = dbContext.KSTU_MIN_STOCK_MASTER.Where(st => st.company_code == rolm.CompanyCode
                                                                                         && st.branch_code == rolm.BranchCode
                                                                                         && st.gs_code == rolm.GSCode
                                                                                         && st.category_code == rolm.CategoryCode
                                                                                         && st.design_code == rolm.DesignCode).FirstOrDefault();
            if (existMinStock != null) {
                error = new ErrorVM()
                {
                    description = "Details already Exist",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            try {
                int seqNo = SIGlobals.Globals.GetNewTransactionSerialNo(dbContext, "4210", rolm.CompanyCode, rolm.BranchCode);
                string temp = "KSTU_MIN_STOCK_MASTER" + SIGlobals.Globals.Separator
                                                      + rolm.CompanyCode + SIGlobals.Globals.Separator
                                                      + rolm.BranchCode + SIGlobals.Globals.Separator
                                                      + seqNo;
                KSTU_MIN_STOCK_MASTER minStock = new KSTU_MIN_STOCK_MASTER();
                minStock.obj_id = SIGlobals.Globals.GetHashcode(temp);
                minStock.company_code = rolm.CompanyCode;
                minStock.branch_code = rolm.BranchCode;
                minStock.gs_code = rolm.GSCode;
                minStock.category_code = rolm.CategoryCode;
                minStock.category_name = rolm.CategoryName;
                minStock.design_code = rolm.DesignCode;
                minStock.design_name = rolm.DesignName;
                minStock.counter_code = rolm.CounterCode;
                minStock.counter_name = rolm.CounterName;
                minStock.min_qty = rolm.MinQty;
                minStock.min_wt = rolm.MinWt;
                minStock.max_qty = rolm.MaxQty;
                minStock.max_wt = rolm.MaxWt;
                minStock.is_saved = rolm.IsSved;
                minStock.updated_on = SIGlobals.Globals.GetDateTime();
                minStock.UniqRowID = Guid.NewGuid();
                dbContext.KSTU_MIN_STOCK_MASTER.Add(minStock);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception excp) {

                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public bool Update(string objID, MinStcokVM rolm, out ErrorVM error)
        {
            error = null;
            if (rolm == null) {
                error = new ErrorVM()
                {
                    description = "Invalid Data",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
            try {
                KSTU_MIN_STOCK_MASTER minStock = dbContext.KSTU_MIN_STOCK_MASTER.Where(st => st.company_code == rolm.CompanyCode
                                                                                        && st.branch_code == rolm.BranchCode
                                                                                        && st.obj_id == objID).FirstOrDefault();
                if (minStock == null) {
                    error = new ErrorVM()
                    {
                        description = "Invalid Data",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                }
                minStock.obj_id = rolm.ObjID;
                minStock.company_code = rolm.CompanyCode;
                minStock.branch_code = rolm.BranchCode;
                //minStock.gs_code = rolm.GSCode;
                //minStock.category_code = rolm.CategoryCode;
                //minStock.category_name = rolm.CategoryName;
                //minStock.design_code = rolm.DesignCode;
                //minStock.design_name = rolm.DesignName;
                //minStock.counter_code = rolm.CounterCode;
                //minStock.counter_name = rolm.CounterName;
                minStock.min_qty = rolm.MinQty;
                minStock.min_wt = rolm.MinWt;
                minStock.max_qty = rolm.MaxQty;
                minStock.max_wt = rolm.MaxWt;
                minStock.is_saved = "";
                minStock.updated_on = SIGlobals.Globals.GetDateTime();
                minStock.UniqRowID = Guid.NewGuid();
                dbContext.Entry(minStock).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception excp) {

                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public bool Delete(string objID, string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            if (objID == string.Empty) {
                error = new ErrorVM()
                {
                    description = "Invalid Data",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
            try {
                KSTU_MIN_STOCK_MASTER minStock = dbContext.KSTU_MIN_STOCK_MASTER.Where(st => st.company_code == companyCode
                                                                                        && st.branch_code == branchCode
                                                                                        && st.obj_id == objID).FirstOrDefault();
                dbContext.KSTU_MIN_STOCK_MASTER.Remove(minStock);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception excp) {

                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public string Print(string companyCode, string branchCode, string gsCode, string itemCode, out ErrorVM error)
        {
            error = null;
            try {
                //var data = dbContext.KSTU_MIN_STOCK_MASTER.Where(st => st.company_code == companyCode
                //                                                    && st.branch_code == branchCode
                //                                                    && st.gs_code == gsCode && st.category_code == itemCode).ToList();

                DataTable dt = SIGlobals.Globals.ExecuteQuery("SELECT gs_code AS GS, " +
                                                              " counter_code AS Counter," +
                                                              " category_code AS Item," +
                                                              " design_name AS Design," +
                                                              " min_qty AS[Min Qty]," +
                                                              " min_wt AS[Min Wt]" +
                                                        " FROM KSTU_MIN_STOCK_MASTER" +
                                                        " WHERE company_code = '" + companyCode + "'" +
                                                        "      AND branch_code = '" + branchCode + "'" +
                                                        "      AND gs_code = '" + gsCode + "'" +
                                                         "      AND category_code = '" + itemCode + "'" +
                                                        " ORDER BY gs_code," +
                                                        "         counter_code," +
                                                        "         category_code," +
                                                        "         design_name; ");

                int columnCount = dt.Columns.Count;
                string[] Alignment = new string[columnCount];
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<HTML>");
                sb.AppendLine("<HEAD>");

                sb.AppendLine(SIGlobals.Globals.GetStyle());
                sb.AppendLine("</HEAD>");
                sb.AppendLine("<BODY>");

                if (columnCount <= 10) {
                    sb.AppendLine(string.Format("<table style=\"width:{0}%\" bgcolor=\"#FDFDF0\" class=\"boldText\" frame=\"border\" border=\"1\" align=\"LEFT\" >"
                        , columnCount * 10));
                }
                else {
                    sb.AppendLine(string.Format("<table style=\"width:100%\" bgcolor=\"#FDFDF0\" class=\"boldText\" frame=\"border\" border=\"1\" align=\"LEFT\" >"));
                }
                sb.AppendLine("<tr valign=\"bottom\" >");
                sb.AppendLine(string.Format("<ph/><th colspan={1} bgcolor=\"EBE7C6\"><h4 style=\"color:maroon\">{0}</h4></th>"
                    , SIGlobals.Globals.FillCompanyDetails("Company_Name", companyCode, branchCode), columnCount));
                sb.AppendLine("</tr>");
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<ph/><td class=\"noborder\" colspan={1} align=\"CENTER\" ><b>{0}</b> </td>", "Minimum Stock List", columnCount));
                sb.AppendLine("</TR>");
                sb.AppendLine("<tr bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\" class = \"fullborderStyle\">");
                string align = "CENTER";
                for (int t = 0; t < columnCount; t++) {
                    if (t == 0) {
                        align = "LEFT";
                    }
                    else {
                        if (string.Compare(dt.Columns[t].DataType.FullName.ToString(), "System.String") == 0) {

                            align = "LEFT";
                        }
                        else {
                            align = "RIGHT";
                        }
                    }
                    sb.AppendLine(string.Format("<pth/><td  ALIGN=\"{0}\" class=\"fullborderStyle\"> <b>{1}</b> </td>", align, dt.Columns[t].ColumnName));
                    Alignment[t] = align;
                }
                sb.AppendLine("</tr>");
                bool IsTotal = false;
                int RowCount = dt.Rows.Count;
                if (string.Compare(dt.Rows[RowCount - 1][0].ToString().ToUpper(), "TOTALS") == 0) {
                    IsTotal = true;
                }
                for (int m = 0; m < RowCount; m++) {
                    sb.AppendLine("<TR>");
                    if (m == RowCount - 1 && IsTotal == true) {
                        if (m % 10 == 0)
                            sb.AppendLine("<p></p>");

                        sb.AppendLine("<TR>");
                        for (int j = 0; j < columnCount; j++) {
                            if (Alignment[j].Equals("RIGHT") && j > 0) {
                                decimal value = 0.00m;
                                value = Convert.ToDecimal(dt.Rows[m][j]);
                                if (value == 0) {
                                    sb.AppendLine(string.Format("<td ALIGN=\"RIGHT\"> {0}{1} </td>", "&nbsp", "&nbsp"));
                                }
                                else {
                                    if (string.Compare(dt.Columns[j].DataType.FullName.ToString(), "System.Decimal") == 0) {
                                        sb.AppendLine(string.Format("<td ALIGN=\"RIGHT\"> <b>{0}<b> </td>", value.ToString("#,0.00"), "&nbsp"));
                                    }
                                    else
                                        sb.AppendLine(string.Format("<td ALIGN=\"RIGHT\"> <b>{0}<b> </td>", dt.Rows[m][j], "&nbsp"));
                                }
                            }
                            else {
                                sb.AppendLine(string.Format("<td ALIGN=\"{0}\"> <b>{1}{2}<b></td>", Alignment[j], dt.Rows[m][j], "&nbsp"));
                            }
                        }
                        sb.AppendLine("</TR>");
                    }
                    else {
                        for (int j = 0; j < columnCount; j++) {
                            if (Alignment[j].Equals("RIGHT") && j > 0) {
                                if (Convert.ToDecimal(dt.Rows[m][j]) == 0) {
                                    sb.AppendLine(string.Format("<td ALIGN=\"{0}\"> {1}{2} </td>", Alignment[j], "&nbsp", "&nbsp"));
                                }
                                else {
                                    sb.AppendLine(string.Format("<td ALIGN=\"{0}\"> {1}{2} </td>", Alignment[j], dt.Rows[m][j], "&nbsp"));
                                }
                            }
                            else {
                                sb.AppendLine(string.Format("<td ALIGN=\"{0}\"> {1}{2} </td>", Alignment[j], dt.Rows[m][j], "&nbsp"));
                            }
                        }
                        sb.AppendLine("</TR>");
                    }
                }

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD ALIGN=\"LEFT\" colspan={1} bgcolor=\"EBE7C6\" ><b style=\"color:'maroon'\"> {0}(Run Date: {2}) </b></TD>", "", columnCount, SIGlobals.Globals.GetDateTime().ToString("dd/MM/yyyy hh:mm:ss tt")));
                sb.AppendLine("</TR>");
                sb.AppendLine("</table>");
                sb.AppendLine("</BODY>");
                sb.AppendLine("</HTML>");
                return sb.ToString();
            }
            catch (Exception excp) {

                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        public ProdigyPrintVM PrintMinStock(string companyCode, string branchCode, string gsCode, string itemCode, out ErrorVM error)
        {
            error = null;
            string printData = Print(companyCode, branchCode, gsCode, itemCode, out error);
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            printObject.PrintType = "HTML";
            printObject.ContinueNextPrint = true;
            printObject.Data = new PrintConfiguration().Base64Encode(printData);
            return printObject;
        }
        #endregion
    }
}
