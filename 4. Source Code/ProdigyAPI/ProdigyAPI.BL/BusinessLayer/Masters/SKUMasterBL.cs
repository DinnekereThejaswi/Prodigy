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

/// <summary>
/// Author: Mustureswara M M (Eshwar)
/// Date: 5th July 2021
/// </summary>
namespace ProdigyAPI.BL.BusinessLayer.Masters
{
    /// <summary>
    /// This Business Layer Provides Methods related to SKU Master.
    /// </summary>
    public sealed class SKUMasterBL
    {
        #region Declaration
        MagnaDbEntities dbContext = new MagnaDbEntities();
        VAMasterBL vaMaster = VAMasterBL.GetInstance;
        public static readonly Lazy<SKUMasterBL> skuMaster = new Lazy<SKUMasterBL>(() => new SKUMasterBL());
        public static SKUMasterBL GetInstace
        {
            get
            {
                return skuMaster.Value;
            }
        }

        #endregion

        #region Constructor
        private SKUMasterBL() { }
        #endregion

        #region Public Methods
        public IEnumerable<SKUVM> GetSKUMasterDetails(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                var data = dbContext.KSTU_SKU_MASTER.Where(sk => sk.company_code == companyCode
                                                            && sk.branch_code == branchCode)
                                                            .Select(s => new SKUVM()
                                                            {
                                                                ObjID = s.obj_id,
                                                                CompanyCode = s.company_code,
                                                                BranchCode = s.branch_code,
                                                                GSCode = s.gs_code,
                                                                SKUID = s.SKU_ID,
                                                                DesignCode = s.design_code,
                                                                Weight = s.weight,
                                                                ItemCode = s.item_code,
                                                                ObjStatus = s.obj_status
                                                            }).ToList();
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public dynamic GetGS(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                return vaMaster.GetGS(companyCode, branchCode, out error);
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public dynamic GetItem(string companyCode, string branchcode, string gsCode, out ErrorVM error)
        {
            error = null;
            try {
                return vaMaster.GetItems(companyCode, branchcode, gsCode, out error);
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public dynamic GetDesigns(string companyCode, string branchcode, string gsCode, string itemCode, out ErrorVM error)
        {
            error = null;
            try {
                var data = dbContext.KTTU_BARCODE_MASTER.Where(b => b.company_code == companyCode
                                                                && b.branch_code == branchcode
                                                                && b.gs_code == gsCode
                                                                && b.item_name == itemCode).Select(b => new
                                                                {
                                                                    Code = b.design_no,
                                                                    Name = b.design_name
                                                                }).Distinct().ToList();
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public bool Save(SKUVM skuMaster, out ErrorVM error)
        {
            error = null;
            KSTU_SKU_MASTER sku = dbContext.KSTU_SKU_MASTER.Where(sk => sk.company_code == skuMaster.CompanyCode
                                                                    && sk.branch_code == sk.branch_code
                                                                    && sk.gs_code == skuMaster.GSCode
                                                                    && sk.item_code == skuMaster.ItemCode
                                                                    && sk.design_code == skuMaster.DesignCode
                                                                    && sk.SKU_ID == skuMaster.SKUID).FirstOrDefault();
            if (sku != null) {
                error = new ErrorVM()
                {
                    description = "Details already Found.",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
            }
            try {
                sku = new KSTU_SKU_MASTER();
                string temp = "KSTU_SKU_MASTER" + SIGlobals.Globals.Separator + skuMaster.SKUID
                                                + SIGlobals.Globals.Separator + skuMaster.CompanyCode
                                                + SIGlobals.Globals.Separator + skuMaster.BranchCode;
                sku.obj_id = SIGlobals.Globals.GetHashcode(temp);
                sku.company_code = skuMaster.CompanyCode;
                sku.branch_code = skuMaster.BranchCode;
                sku.gs_code = skuMaster.GSCode;
                sku.SKU_ID = skuMaster.SKUID;
                sku.design_code = skuMaster.DesignCode;
                sku.weight = skuMaster.Weight;
                sku.item_code = skuMaster.ItemCode;
                sku.obj_status = skuMaster.ObjStatus;
                sku.UniqRowID = Guid.NewGuid();
                dbContext.KSTU_SKU_MASTER.Add(sku);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public bool Update(SKUVM skuMaster, out ErrorVM error)
        {
            error = null;
            KSTU_SKU_MASTER sku = dbContext.KSTU_SKU_MASTER.Where(sk => sk.company_code == skuMaster.CompanyCode
                                                                    && sk.branch_code == sk.branch_code
                                                                    && sk.gs_code == skuMaster.GSCode
                                                                    && sk.item_code == skuMaster.ItemCode
                                                                    && sk.design_code == skuMaster.DesignCode
                                                                    && sk.SKU_ID == skuMaster.SKUID).FirstOrDefault();
            if (sku == null) {
                error = new ErrorVM()
                {
                    description = "Details not Found.",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
            }
            try {
                sku.obj_id = skuMaster.ObjID;
                sku.company_code = skuMaster.CompanyCode;
                sku.branch_code = skuMaster.BranchCode;
                sku.gs_code = skuMaster.GSCode;
                sku.SKU_ID = skuMaster.SKUID;
                sku.design_code = skuMaster.DesignCode;
                sku.weight = skuMaster.Weight;
                sku.item_code = skuMaster.ItemCode;
                sku.obj_status = skuMaster.ObjStatus;
                sku.UniqRowID = Guid.NewGuid();
                dbContext.Entry(sku).State = System.Data.Entity.EntityState.Modified;
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
            KSTU_SKU_MASTER sku = dbContext.KSTU_SKU_MASTER.Where(sk => sk.company_code == companyCode
                                                                    && sk.branch_code == branchCode
                                                                    && sk.obj_id == objID).FirstOrDefault();
            if (sku == null) {
                error = new ErrorVM()
                {
                    description = "Details not Found.",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
            }
            try {
                dbContext.KSTU_SKU_MASTER.Remove(sku);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public ProdigyPrintVM Print(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            string printData = PrintSKUMaster(companyCode, branchCode, out error);
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            printObject.PrintType = "HTML";
            printObject.ContinueNextPrint = true;
            printObject.Data = new PrintConfiguration().Base64Encode(printData);
            return printObject;
        }
        #endregion

        #region Private Methods
        private string PrintSKUMaster(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            StringBuilder sb = new StringBuilder();
            string ReportFooter = "SKU Master";
            try {
                string query = string.Format("exec [usp_SKUPrint] @Companycode = {0}, @Branchcode = {1}"
                                       , companyCode, branchCode);
                DataTable dt = SIGlobals.Globals.ExecuteQuery(query);

                int columnCount = dt.Columns.Count;
                string[] Alignment = new string[columnCount];
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
                sb.AppendLine(string.Format("<ph/><td class=\"noborder\" colspan={1} align=\"CENTER\" ><b>{0}</b> </td>", ReportFooter, columnCount));
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
                sb.AppendLine(string.Format("<TD ALIGN=\"LEFT\" colspan={1} bgcolor=\"EBE7C6\" ><b style=\"color:'maroon'\"> {0}(Run Date: {2}) </b></TD>", ReportFooter, columnCount, SIGlobals.Globals.GetDateTime()));
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
        #endregion
    }
}
