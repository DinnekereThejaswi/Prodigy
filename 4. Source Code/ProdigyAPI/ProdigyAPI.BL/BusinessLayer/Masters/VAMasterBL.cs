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

namespace ProdigyAPI.BL.BusinessLayer.Masters
{
    public sealed class VAMasterBL
    {
        #region Declaration
        private readonly MagnaDbEntities dbContext = new MagnaDbEntities();
        private static readonly Lazy<VAMasterBL> vaInstance = new Lazy<VAMasterBL>(() => new VAMasterBL());
        public static VAMasterBL GetInstance
        {
            get
            {
                return vaInstance.Value;
            }
        }
        #endregion

        #region Constructor
        private VAMasterBL() { }
        #endregion

        #region Methods
        /// <summary>
        /// This method provides Vendor Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchcode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public dynamic GetVendor(string companyCode, string branchcode, out ErrorVM error)
        {
            error = null;
            try {
                string[] types = { "VD", "VS" };
                var data = dbContext.KSTU_SUPPLIER_MASTER.Where(sup => sup.company_code == companyCode
                                                                && sup.branch_code == branchcode
                                                                && types.Contains(sup.voucher_code)).OrderBy(sup => sup.party_name)
                                                                                                    .Select(sup => new
                                                                                                    {
                                                                                                        Code = sup.party_code,
                                                                                                        Name = sup.party_name + "-" + sup.party_code
                                                                                                    });
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        /// <summary>
        /// This Method Provides GS
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchcode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public dynamic GetGS(string companyCode, string branchcode, out ErrorVM error)
        {
            error = null;
            try {
                var data = dbContext.usp_LoadGS(companyCode, branchcode, "SW");
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        /// <summary>
        /// Get Items
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchcode"></param>
        /// <param name="gsCode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public dynamic GetItems(string companyCode, string branchcode, string gsCode, out ErrorVM error)
        {
            error = null;
            try {
                var data = dbContext.ITEM_MASTER.Where(sup => sup.company_code == companyCode
                                                       && sup.branch_code == branchcode
                                                       && sup.gs_code == gsCode).OrderBy(sup => sup.Item_code)
                                                                                .Select(it => new
                                                                                {
                                                                                    Code = it.Item_code,
                                                                                    Name = it.Item
                                                                                });
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        /// <summary>
        /// Get Designs
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchcode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public dynamic GetDesgins(string companyCode, string branchcode, out ErrorVM error)
        {
            error = null;
            try {
                var data = dbContext.KTTU_DESIGN_MASTER.Where(d => d.company_code == companyCode
                                                                && d.branch_code == branchcode
                                                                && d.obj_status == "O").OrderBy(d => d.design_name)
                                                                                       .Select(d => new
                                                                                       {
                                                                                           Code = d.design_code,
                                                                                           Name = d.design_name
                                                                                       });
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }
        /// <summary>
        /// Get MC Types
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchcode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public dynamic GetMCTypes(string companyCode, string branchcode, out ErrorVM error)
        {
            error = null;
            try {
                var data = dbContext.usp_LoadMCTypes(companyCode, branchcode, "V");
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        /// <summary>
        /// Get VA Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchcode"></param>
        /// <param name="gsCode"></param>
        /// <param name="itemCode"></param>
        /// <param name="designCode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public dynamic GetVADetails(string companyCode, string branchcode, string gsCode, string itemCode, string designCode, out ErrorVM error)
        {
            error = null;
            try {
                var data = dbContext.KSTU_VA_MASTER.Where(va => va.company_code == companyCode
                                                            && va.branch_code == branchcode
                                                            && va.gs_code == gsCode
                                                            && va.category_code == itemCode);
                if (designCode != null) {
                    data = data.Where(va => va.design_code == designCode);
                }
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        /// <summary>
        /// Save VA Details
        /// </summary>
        /// <param name="va"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool Save(List<VAMasterVM> va, out ErrorVM error)
        {
            error = null;
            if (va == null) {
                error = new ErrorVM()
                {
                    description = "Invalid data",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
            try {
                foreach (VAMasterVM v in va) {
                    KSTU_VA_MASTER vaMaster = new KSTU_VA_MASTER();
                    vaMaster.obj_id = SIGlobals.Globals.GetMagnaGUID("KSTU_VA_MASTER", 0, v.CompanyCode, v.BranchCode);
                    vaMaster.sno = v.SNO;
                    vaMaster.company_code = v.CompanyCode;
                    vaMaster.branch_code = v.BranchCode;
                    vaMaster.gs_code = v.GSCode;
                    vaMaster.category_code = v.CategoryCode;
                    vaMaster.from_wt = v.FromWt;
                    vaMaster.to_wt = v.ToWt;
                    vaMaster.mc_amount = v.McAmount;
                    vaMaster.mc_per_gram = v.McPerGram;
                    vaMaster.wastage_grms = v.WastageGrms;
                    vaMaster.wast_percentage = v.WastPercentage;
                    vaMaster.type_id = v.TypeID;
                    vaMaster.range = v.range;
                    vaMaster.mc_percent = v.McPercent;
                    vaMaster.party_code = v.PartyCode;
                    vaMaster.design_code = v.DesignCode;
                    vaMaster.obj_status = v.ObjectStatus;
                    vaMaster.mc_per_piece = v.McPerPiece;
                    vaMaster.valueadded = v.ValueAdded;
                }
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return true;
            }
        }

        public bool Copy(VAMasterCopyVM vaCopy, out ErrorVM error)
        {
            error = null;
            KSTU_SUPPLIER_MASTER chkExist = dbContext.KSTU_SUPPLIER_MASTER.Where(va => va.party_code == vaCopy.FromSupplierCode
                                                                    && va.company_code == vaCopy.CompanyCode
                                                                    && va.branch_code == vaCopy.BranchCode).FirstOrDefault();
            if (chkExist == null) {
                error = new ErrorVM()
                {
                    description = "Supplier Details not Exist." + vaCopy.FromSupplierCode,
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return false;
            }
            chkExist = null;
            chkExist = dbContext.KSTU_SUPPLIER_MASTER.Where(va => va.party_code == vaCopy.ToSupplierCode
                                                                   && va.company_code == vaCopy.CompanyCode
                                                                   && va.branch_code == vaCopy.BranchCode).FirstOrDefault();
            if (chkExist == null) {
                error = new ErrorVM()
                {
                    description = "Supplier Details not Exist." + vaCopy.ToSupplierCode,
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return false;
            }
            try {
                var data = dbContext.KSTU_VA_MASTER.Where(va => va.company_code == vaCopy.CompanyCode
                                                  && va.branch_code == vaCopy.BranchCode
                                                  && va.party_code == vaCopy.FromSupplierCode);
                if (!string.IsNullOrEmpty(vaCopy.GSCode)) {
                    data = data.Where(va => va.gs_code == vaCopy.GSCode);
                }
                if (!string.IsNullOrEmpty(vaCopy.ItemCode)) {
                    data = data.Where(va => va.category_code == vaCopy.ItemCode);
                }
                if (!string.IsNullOrEmpty(vaCopy.DesignCode)) {
                    data = data.Where(va => va.design_code == vaCopy.DesignCode);
                }
                List<KSTU_VA_MASTER> vaMaster = data.ToList();

                foreach (KSTU_VA_MASTER v in vaMaster) {
                    KSTU_VA_MASTER newVAMaster = new KSTU_VA_MASTER();
                    newVAMaster.obj_id = SIGlobals.Globals.GetMagnaGUID("KSTU_VA_MASTER", 0, v.company_code, v.branch_code);
                    newVAMaster.sno = GetAndUpdateNextNo(v.company_code, v.branch_code);
                    newVAMaster.company_code = v.company_code;
                    newVAMaster.branch_code = v.branch_code;
                    newVAMaster.gs_code = v.gs_code;
                    newVAMaster.category_code = v.category_code;
                    newVAMaster.from_wt = v.from_wt;
                    newVAMaster.to_wt = v.to_wt;
                    newVAMaster.mc_amount = v.mc_amount;
                    newVAMaster.mc_per_gram = v.mc_per_gram;
                    newVAMaster.wastage_grms = v.wastage_grms;
                    newVAMaster.wast_percentage = v.wast_percentage;
                    newVAMaster.type_id = v.type_id;
                    newVAMaster.range = v.range;
                    newVAMaster.mc_percent = v.mc_percent;
                    newVAMaster.party_code = vaCopy.ToSupplierCode;
                    newVAMaster.design_code = v.design_code;
                    newVAMaster.obj_status = v.obj_status;
                    newVAMaster.mc_per_piece = v.mc_per_piece;
                    newVAMaster.valueadded = v.valueadded;
                    dbContext.KSTU_VA_MASTER.Add(newVAMaster);
                }
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return true;
            }
        }

        /// <summary>
        /// Provides Print of VA Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        /// 
        public ProdigyPrintVM Print(VAMasterPrintVM va, out ErrorVM error)
        {
            error = null;
            string printData = PrintVADet(va, out error);
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            printObject.PrintType = "HTML";
            printObject.ContinueNextPrint = true;
            printObject.Data = new PrintConfiguration().Base64Encode(printData);
            return printObject;
        }

        /// <summary>
        /// Provides Print of VA Details Itemwise
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public ProdigyPrintVM PrintItemWise(VAMasterPrintVM va, out ErrorVM error)
        {
            error = null;
            string printData = PrintItemWiseDet(va, out error);
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            printObject.PrintType = "HTML";
            printObject.ContinueNextPrint = true;
            printObject.Data = new PrintConfiguration().Base64Encode(printData);
            return printObject;
        }

        /// <summary>
        /// Provides Print of VA Details supplierwise
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public ProdigyPrintVM PrintSupplierwise(VAMasterPrintVM va, out ErrorVM error)
        {
            error = null;
            string printData = PrintSupplierwiseDet(va, out error);
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            printObject.PrintType = "HTML";
            printObject.ContinueNextPrint = true;
            printObject.Data = new PrintConfiguration().Base64Encode(printData);
            return printObject;
        }

        #region Private Methods
        private string PrintVADet(VAMasterPrintVM va, out ErrorVM error)
        {
            error = null;
            StringBuilder sb = new StringBuilder();
            string supplierCode = string.IsNullOrEmpty(va.SupplierCode) ? "ALL" : va.SupplierCode;
            string gsCode = string.IsNullOrEmpty(va.GSCode) ? "ALL" : va.GSCode;
            string itemCode = string.IsNullOrEmpty(va.ItemCode) ? "ALL" : va.ItemCode;
            string designCode = string.IsNullOrEmpty(va.DesignCode) ? "ALL" : va.DesignCode;
            bool Cflag = false;
            try {
                string vaQuery = string.Format("EXEC usp_VAPrint '{0}','{1}','{2}','{3}','{4}','{5}'",
                 va.CompanyCode, va.BranchCode, supplierCode, gsCode, itemCode, designCode);

                DataTable dtGrid = SIGlobals.Globals.ExecuteQuery(vaQuery);
                if (dtGrid == null || dtGrid.Rows.Count <= 0) {
                    error = new ErrorVM()
                    {
                        description = "No VA for this Item",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return "";
                }

                sb.AppendLine("<style = \"text/css\">\n");
                sb.AppendLine(".boldText, .boldtable TD, .boldtable TH\n");
                sb.AppendLine("{\n");
                sb.AppendLine("font-family: 'Times New Roman';\n");
                sb.AppendLine("font-size:10pt;\n");
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
                sb.AppendLine("border-top-style:1px solid\n");
                sb.AppendLine("}\n");
                sb.AppendLine(".noborder\n");
                sb.AppendLine("{\n");
                sb.AppendLine("border-left-style:none;;\n");
                sb.AppendLine("border-right-style:none;\n");
                sb.AppendLine("border-top-style:none;;\n");
                sb.AppendLine("border-bottom-style:none;\n");
                sb.AppendLine("}\n");
                sb.AppendLine("</style>\n");
                sb.AppendLine("<HTML>");
                sb.AppendLine("<BODY>");
                sb.AppendLine("<table bgcolor=\"#FDFDF0\" class=\"boldText\" frame=\"border\" border=\"2\" width=\"1000\" align=\"LEFT\" >");
                sb.AppendLine("<TR height=\"3px\">");
                sb.AppendLine("</TR>");
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD class=\"noborder\" colspan=11 align=\"CENTER\" ><H5>VA MASTER OF {0} </H5> </TD>", SIGlobals.Globals.GetItemName(dbContext, gsCode, itemCode, va.CompanyCode, va.BranchCode)));
                sb.AppendLine("</TR>");

                sb.AppendLine("<tr bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\" class = \"fullborderStyle\">");
                if (Cflag == false) {
                    sb.AppendLine("<td align=\"LEFT\"><b>Smith</b></td>");
                }
                sb.AppendLine("<td align=\"LEFT\"><b>GS Code</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>Category</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>From WT</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>To WT</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>Design</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>MC/Grm</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>MC/Piece</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>Wast.%</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>MC Amount</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>Wast.Gms</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>MC %</b></td>");
                sb.AppendLine("</tr>");

                for (int i = 0; i < dtGrid.Rows.Count; i++) {
                    sb.AppendLine("<TR>");
                    if (Cflag == false) {
                        sb.AppendLine(string.Format("<TD ALIGN = \"LEFT\">{0}</TD>", dtGrid.Rows[i]["party_name"]));
                    }
                    sb.AppendLine(string.Format("<TD ALIGN = \"LEFT\">{0}</TD>", dtGrid.Rows[i]["gs_code"]));
                    sb.AppendLine(string.Format("<TD ALIGN = \"LEFT\">{0}</TD>", SIGlobals.Globals.GetItemName(dbContext, dtGrid.Rows[i]["category_code"].ToString(), dtGrid.Rows[i]["gs_code"].ToString(), va.CompanyCode, va.BranchCode)));
                    sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", dtGrid.Rows[i]["from_wt"]));
                    sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", dtGrid.Rows[i]["to_wt"]));
                    sb.AppendLine(string.Format("<TD ALIGN = \"LEFT\">{0}</TD>", dtGrid.Rows[i]["design_name"]));
                    sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", dtGrid.Rows[i]["mc_per_gram"]));
                    sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", dtGrid.Rows[i]["mc_per_piece"]));
                    sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", dtGrid.Rows[i]["wast_percentage"]));
                    sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", dtGrid.Rows[i]["mc_amount"]));
                    sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", dtGrid.Rows[i]["wastage_grms"]));
                    sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", dtGrid.Rows[i]["mc_percent"]));
                    sb.AppendLine("</TR>");
                }
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

        private string PrintItemWiseDet(VAMasterPrintVM va, out ErrorVM error)
        {
            error = null;
            StringBuilder sb = new StringBuilder();
            string supplierCode = string.IsNullOrEmpty(va.SupplierCode) ? "ALL" : va.SupplierCode;
            string gsCode = string.IsNullOrEmpty(va.GSCode) ? "ALL" : va.GSCode;
            string itemCode = string.IsNullOrEmpty(va.ItemCode) ? "ALL" : va.ItemCode;
            string designCode = string.IsNullOrEmpty(va.DesignCode) ? "ALL" : va.DesignCode;
            try {
                var data = (from vam in dbContext.KSTU_VA_MASTER
                            join sup in dbContext.KSTU_SUPPLIER_MASTER on new { CompanyCode = vam.company_code, BranchCode = vam.branch_code, PartyCode = vam.party_code } equals new { CompanyCode = sup.company_code, BranchCode = sup.branch_code, PartyCode = sup.party_code }
                            join dsm in dbContext.KTTU_DESIGN_MASTER on new { CompanyCode = vam.company_code, BranchCode = vam.branch_code, DesignCode = vam.design_code } equals new { CompanyCode = dsm.company_code, BranchCode = dsm.branch_code, DesignCode = dsm.design_code }
                            where vam.company_code == va.CompanyCode && vam.branch_code == va.BranchCode && vam.gs_code == va.GSCode && vam.category_code == va.ItemCode
                            select new
                            {
                                sup.party_name,
                                vam.gs_code,
                                vam.type_id,
                                vam.category_code,
                                dsm.design_name,
                                vam.from_wt,
                                vam.to_wt,
                                vam.mc_amount,
                                vam.mc_per_gram,
                                vam.mc_per_piece,
                                vam.wastage_grms,
                                vam.wast_percentage,
                                vam.mc_percent,
                                vam.obj_id,
                                vam.sno,
                                vam.range,
                                vam.design_code
                            }).ToList();

                if (data == null || data.Count == 0) {
                    error = new ErrorVM()
                    {
                        description = "No VA for this Item",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return "";
                }

                sb.AppendLine("<style = \"text/css\">\n");
                sb.AppendLine(".boldText, .boldtable TD, .boldtable TH\n");
                sb.AppendLine("{\n");
                sb.AppendLine("font-family: 'Times New Roman';\n");
                sb.AppendLine("font-size:10pt;\n");
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
                sb.AppendLine("border-top-style:1px solid\n");
                sb.AppendLine("}\n");
                sb.AppendLine(".noborder\n");
                sb.AppendLine("{\n");
                sb.AppendLine("border-left-style:none;;\n");
                sb.AppendLine("border-right-style:none;\n");
                sb.AppendLine("border-top-style:none;;\n");
                sb.AppendLine("border-bottom-style:none;\n");
                sb.AppendLine("}\n");
                sb.AppendLine("</style>\n");
                sb.AppendLine("<HTML>");
                sb.AppendLine("<BODY>");
                sb.AppendLine("<table bgcolor=\"#FDFDF0\" class=\"boldText\" frame=\"border\" border=\"2\" width=\"1000\" align=\"LEFT\" >");
                sb.AppendLine("<TR height=\"3px\">");
                sb.AppendLine("</TR>");
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD class=\"noborder\" colspan=12 align=\"CENTER\" ><H5>VA Details of Category : {0}  </H5> </TD>", SIGlobals.Globals.GetItemName(dbContext, gsCode, itemCode, va.CompanyCode, va.BranchCode)));
                sb.AppendLine("</TR>");
                sb.AppendLine("<tr bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\" class = \"fullborderStyle\">");
                sb.AppendLine("<td align=\"LEFT\"><b>Smith</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>GS Code</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>Category</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>From WT</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>To WT</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>Design</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>MC/Grm</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>MC/Piece</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>Wast.%</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>MC Amount</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>Wast.Gms</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>MC %</b></td>");
                sb.AppendLine("</tr>");
                sb.AppendLine(string.Format("<TR><TD  class=\"border\" colspan=12 align=\"LEFT\"><B>SUPPLIER :{0}<B> </td></TR>", data[0].party_name));
                sb.AppendLine(string.Format("<TR><TD  class=\"border\" colspan=12 align=\"LEFT\"><B>{0}<B> </td></TR>", data[0].design_name));

                for (int i = 0; i < data.Count(); i++) {
                    sb.AppendLine("<TR>");
                    if (i > 0) {
                        if (string.Compare(data[i].design_name, data[i].design_name.ToString()) != 0) {
                            sb.AppendLine(string.Format("<TR><TD  class=\"border\" colspan=12 align=\"LEFT\"><B>{0}<B> </td></TR>", data[i].design_name.ToString()));

                        }
                        if (string.Compare(data[i - 1].party_name.ToString(), data[i].party_name.ToString()) != 0) {
                            sb.AppendLine(string.Format("<TR><TD  class=\"border\" colspan=12 align=\"LEFT\"><B>SUPPLIER :{0}<B> </td></TR>", data[i].party_name.ToString()));

                        }
                    }
                    sb.AppendLine(string.Format("<TD ALIGN = \"LEFT\">{0}</TD>", data[i].party_name));
                    sb.AppendLine(string.Format("<TD ALIGN = \"LEFT\">{0}</TD>", data[i].gs_code));
                    sb.AppendLine(string.Format("<TD ALIGN = \"LEFT\">{0}</TD>", SIGlobals.Globals.GetItemName(dbContext, data[i].gs_code, data[i].category_code, va.CompanyCode, va.BranchCode)));
                    sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", data[i].from_wt));
                    sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", data[i].to_wt));
                    sb.AppendLine(string.Format("<TD ALIGN = \"LEFT\">{0}</TD>", data[i].design_name));
                    sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", data[i].mc_per_gram));
                    sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", data[i].mc_per_piece));
                    sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", data[i].wast_percentage));
                    sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", data[i].mc_amount));
                    sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", data[i].wastage_grms));
                    sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", data[i].mc_percent));
                    sb.AppendLine("</TR>");
                }

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

        private string PrintSupplierwiseDet(VAMasterPrintVM va, out ErrorVM error)
        {
            error = null;
            StringBuilder sb = new StringBuilder();
            string supplierCode = string.IsNullOrEmpty(va.SupplierCode) ? "ALL" : va.SupplierCode;
            string gsCode = string.IsNullOrEmpty(va.GSCode) ? "ALL" : va.GSCode;
            string itemCode = string.IsNullOrEmpty(va.ItemCode) ? "ALL" : va.ItemCode;
            string designCode = string.IsNullOrEmpty(va.DesignCode) ? "ALL" : va.DesignCode;
            try {
                var data = (from vam in dbContext.KSTU_VA_MASTER
                            join sup in dbContext.KSTU_SUPPLIER_MASTER
                            on new { CompanyCode = vam.company_code, BranchCode = vam.branch_code, PartyCode = vam.party_code } equals new { CompanyCode = sup.company_code, BranchCode = sup.branch_code, PartyCode = sup.party_code }
                            join dsm in dbContext.KTTU_DESIGN_MASTER
                            on new { CompanyCode = vam.company_code, BranchCode = vam.branch_code, DesignCode = vam.design_code } equals new { CompanyCode = dsm.company_code, BranchCode = dsm.branch_code, DesignCode = dsm.design_code }
                            where vam.company_code == va.CompanyCode && vam.branch_code == va.BranchCode && vam.party_code == va.SupplierCode
                            select new
                            {
                                sup.party_name,
                                vam.gs_code,
                                vam.type_id,
                                vam.category_code,
                                dsm.design_name,
                                vam.from_wt,
                                vam.to_wt,
                                vam.mc_amount,
                                vam.mc_per_gram,
                                vam.mc_per_piece,
                                vam.wastage_grms,
                                vam.wast_percentage,
                                vam.mc_percent,
                                vam.obj_id,
                                vam.sno,
                                vam.range,
                                vam.design_code
                            }).ToList();

                if (data == null || data.Count == 0) {
                    error = new ErrorVM()
                    {
                        description = "No VA for this Item",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return "";
                }
                sb.AppendLine("<style = \"text/css\">\n");

                sb.AppendLine(".boldText, .boldtable TD, .boldtable TH\n");
                sb.AppendLine("{\n");
                sb.AppendLine("font-family: 'Times New Roman';\n");
                sb.AppendLine("font-size:10pt;\n");
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
                sb.AppendLine("border-top-style:1px solid\n");
                sb.AppendLine("}\n");


                sb.AppendLine(".noborder\n");
                sb.AppendLine("{\n");
                sb.AppendLine("border-left-style:none;;\n");
                sb.AppendLine("border-right-style:none;\n");
                sb.AppendLine("border-top-style:none;;\n");
                sb.AppendLine("border-bottom-style:none;\n");
                sb.AppendLine("}\n");
                sb.AppendLine("</style>\n");
                sb.AppendLine("<HTML>");
                sb.AppendLine("<BODY>");
                sb.AppendLine("<table bgcolor=\"#FDFDF0\" class=\"boldText\" frame=\"border\" border=\"2\" width=\"1000\" align=\"LEFT\" >");
                sb.AppendLine("<TR height=\"3px\">");
                sb.AppendLine("</TR>");
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD class=\"noborder\" colspan=12 align=\"CENTER\" ><H5>VA Details of Category : {0}  </H5> </TD>", SIGlobals.Globals.GetItemName(dbContext, gsCode, itemCode, va.CompanyCode, va.BranchCode)));
                sb.AppendLine("</TR>");
                sb.AppendLine("<tr bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\" class = \"fullborderStyle\">");
                sb.AppendLine("<td align=\"LEFT\"><b>GS Code</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>Category</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>From WT</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>To WT</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>Design</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>MC/Grm</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>MC/Piece</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>Wast.%</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>MC Amount</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>Wast.Gms</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>VA %</b></td>");

                sb.AppendLine(string.Format("<TR><TD  class=\"border\" colspan=11 align=\"LEFT\"><B>{0}<B> </td></TR>", SIGlobals.Globals.GetItemName(dbContext, data[0].gs_code, data[0].category_code, va.CompanyCode, va.BranchCode)));
                sb.AppendLine("</tr>");
                for (int i = 0; i < data.Count(); i++) {
                    sb.AppendLine("<TR>");
                    if (i > 0) {
                        if (string.Compare(data[i - 1].category_code.ToString(), data[i].category_code.ToString()) != 0) {
                            sb.AppendLine(string.Format("<tr><TD  class=\"border\" colspan=11 align=\"LEFT\"><B>{0}<B> </td></TR>", SIGlobals.Globals.GetItemName(dbContext, data[i].category_code, data[i].gs_code, va.CompanyCode, va.BranchCode)));
                        }
                    }
                    sb.AppendLine(string.Format("<TD ALIGN = \"LEFT\">{0}</TD>", data[i].gs_code));
                    sb.AppendLine(string.Format("<TD ALIGN = \"LEFT\">{0}</TD>", SIGlobals.Globals.GetItemName(dbContext, data[i].gs_code, data[i].category_code.ToString(), va.CompanyCode, va.BranchCode)));
                    sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", data[i].from_wt));
                    sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", data[i].to_wt));
                    sb.AppendLine(string.Format("<TD ALIGN = \"LEFT\">{0}</TD>", data[i].design_name));
                    sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", data[i].mc_per_gram));
                    sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", data[i].mc_per_piece));
                    sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", data[i].wast_percentage));
                    sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", data[i].mc_amount));
                    sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", data[i].wastage_grms));
                    sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", data[i].mc_percent));
                    sb.AppendLine("</TR>");
                }

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

        private int GetAndUpdateNextNo(string companyCode, string branchCode)
        {
            var kstsSeqNos = dbContext.KSTS_SEQ_NOS.Where(
                                       rs => rs.company_code == companyCode
                                       && rs.branch_code == branchCode
                                       && rs.obj_id == "34").FirstOrDefault();
            if (kstsSeqNos == null)
                return 0;

            int nextBatchNo = SIGlobals.Globals.GetFinancialYear(dbContext, companyCode, branchCode) + kstsSeqNos.nextno;
            kstsSeqNos.nextno = nextBatchNo + 1;
            dbContext.Entry(kstsSeqNos).State = System.Data.Entity.EntityState.Modified;

            return nextBatchNo;
        }
        #endregion

        #endregion
    }
}
