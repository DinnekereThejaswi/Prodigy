using ProdigyAPI.BL.BusinessLayer.PrintConfig;
using ProdigyAPI.BL.BusinessLayer.Stock;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.OPGProcessing;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Print;
using ProdigyAPI.Model.MagnaDb;
using ProdigyAPI.SIGlobals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using ProdigyAPI.BL.BusinessLayer.Sales;

namespace ProdigyAPI.BL.BusinessLayer.OPGProcessing
{
    public class OPGCPCIssueBL
    {
        MagnaDbEntities db = null;

        public OPGCPCIssueBL()
        {
            db = new MagnaDbEntities(true);
        }

        public OPGCPCIssueBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }

        public List<ListOfValue> GetIssueToList(string companyCode, string branchCode)
        {
            var partyList = from sm in db.KSTU_SUPPLIER_MASTER
                            join sg in db.KSTU_SUPPLIER_GROUP
                            on new { CC = sm.company_code, BC = sm.branch_code, GS = sm.party_code }
                                equals new { CC = sg.company_code, BC = sg.branch_code, GS = sg.party_code }
                            where sm.company_code == companyCode && sm.branch_code == branchCode
                                && sm.party_code != "HO" && sm.obj_status != "C"
                                && sg.ir_code == "IB"
                            orderby sm.party_name
                            select new ListOfValue
                            {
                                Code = sm.party_code,
                                Name = sm.party_name
                            };

            return partyList.ToList();
        }
        
        public List<ListOfValue> GetReceiptList(string companyCode, string branchCode, out string errorMessage)
        {
            errorMessage = string.Empty;
            try {
                string sql = string.Format(
                    "SELECT DISTINCT receipt_no from vOGBatchForCPC  \n"
                   + "	WHERE company_code = '{0}' and branch_code = '{1}'  \n"
                   + "	ORDER BY receipt_no DESC",
                                   companyCode, branchCode);
                DataTable dt = Globals.GetDataTable(sql);
                if (dt == null || dt.Rows.Count <= 0) {
                    errorMessage = "No details found..";
                    return null;
                }
                
                List<ListOfValue> opgBatchList = new List<ListOfValue>();
                foreach (DataRow row in dt.Rows) {
                    ListOfValue bd = new ListOfValue
                    {
                        Code = row["receipt_no"].ToString(),
                        Name = row["receipt_no"].ToString(),
                    };
                    opgBatchList.Add(bd);
                }

                return opgBatchList;
            }
            catch (Exception ex) {
                errorMessage = new ErrorVM().GetErrorDetails(ex).customDescription;
                return null;
            }
        }

        public List<OPGCPCIssueLineVM> GetBatchList(string companyCode, string branchCode, int documentNo, out string errorMessage)
        {
            errorMessage = string.Empty;
            try {
                string sql = string.Format("SELECT * FROM vOGBatchForCPC WHERE company_code = '{0}' AND branch_code = '{1}' AND receipt_no = {2}",
                                   companyCode, branchCode, documentNo);
                DataTable dt = Globals.GetDataTable(sql);
                if (dt == null || dt.Rows.Count <= 0) {
                    errorMessage = "No batch details found.";
                    return null;
                }

                List<OPGCPCIssueLineVM> opgBatchList = new List<OPGCPCIssueLineVM>();
                foreach (DataRow row in dt.Rows) {
                    var GSCode = row["gs_code"].ToString();
                    var HSNCode = db.KSTS_GS_ITEM_ENTRY.Where(gse => gse.company_code == companyCode && gse.branch_code == branchCode
                        && gse.gs_code == GSCode).Select(x => x.HSN).FirstOrDefault();
                    OPGCPCIssueLineVM bd = new OPGCPCIssueLineVM
                    {
                        BatchId = row["batch_id"].ToString(),
                        GSCode = row["gs_code"].ToString(),
                        GrossWt = Convert.ToDecimal(row["gwt"]),
                        StoneWt = Convert.ToDecimal(row["swt"]),
                        NetWt = Convert.ToDecimal(row["nwt"]),
                        AverageRate = Convert.ToDecimal(row["avg_purchase_rate"]),
                        DiamondGS = row["dm_gs_code"].ToString(),
                        StoneGS = row["stn_gs_code"].ToString(),
                        DiamondCarets = Convert.ToDecimal(row["dcts"]),
                        KovaWeight = Convert.ToDecimal(row["kova_wt"]),
                        WastageWeight = Convert.ToDecimal(row["wastage"]),
                        PurityPercent = Convert.ToDecimal(row["purity"]),
                        Amount = Convert.ToDecimal(row["item_amount"]),
                        HSN = HSNCode
                    };
                    opgBatchList.Add(bd);
                }

                return opgBatchList;
            }
            catch (Exception ex) {
                errorMessage = new ErrorVM().GetErrorDetails(ex).customDescription;
                return null;
            }
        }

        //public OPGCPCIssueLineVM GetBatchInfo(string companyCode, string branchCode, string batchID, out string errorMessage)
        //{
        //    errorMessage = string.Empty;
        //    try {
        //        var dbBatchInfo = db.vConversionBatchIds.Where(x => x.company_code == companyCode && x.branch_code == branchCode
        //                && x.BatchId == batchID).FirstOrDefault();

        //        if (dbBatchInfo == null) {
        //            errorMessage = "No details found for the batch ID: " + batchID;
        //            return null;
        //        }

        //        decimal purchaseRate = 0;
        //        var rateMaster = db.KSTU_RATE_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
        //            && x.gs_code == dbBatchInfo.gs_code && x.karat == "24K").FirstOrDefault();
        //        if (rateMaster != null) {
        //            if (rateMaster.bill_type == "P")
        //                purchaseRate = rateMaster.exchange_rate;
        //            else
        //                purchaseRate = rateMaster.rate;
        //        }

        //        var batchInfo = new OPGCPCIssueLineVM
        //        {
        //            GSCode = dbBatchInfo.gs_code,
        //            BatchId = dbBatchInfo.BatchId,
        //            GrossWt = Convert.ToDecimal(dbBatchInfo.gwt),
        //            StoneWt = Convert.ToDecimal(dbBatchInfo.swt),
        //            NetWt = Convert.ToDecimal(dbBatchInfo.nwt),
        //            Av = purchaseRate,
        //            Amount = purchaseRate * Convert.ToDecimal(dbBatchInfo.nwt)
        //        };
        //        return batchInfo;
        //    }
        //    catch (Exception ex) {
        //        errorMessage = new ErrorVM().GetErrorDetails(ex).customDescription;
        //        return null;
        //    }

        //}

        public OPGCPCIssueLineVM GetBatchInfo(string companyCode, string branchCode, int documentNo, string batchId, out string errorMessage)
        {
            errorMessage = string.Empty;
            try {
                string sql = string.Format("SELECT * FROM vOGBatchForCPC WHERE company_code = '{0}' AND branch_code = '{1}' AND receipt_no = '{2}' AND batch_id = '{3}'",
                                   companyCode, branchCode, documentNo, batchId);
                DataTable dt = Globals.GetDataTable(sql);
                if (dt == null || dt.Rows.Count <= 0) {
                    errorMessage = "No batch details found.";
                    return null;
                }
                else {
                    var row = dt.Rows[0];
                    var GSCode = row["gs_code"].ToString();
                    var HSNCode = db.KSTS_GS_ITEM_ENTRY.Where(gse => gse.company_code == companyCode && gse.branch_code == branchCode
                        && gse.gs_code == GSCode).Select(x => x.HSN).FirstOrDefault();
                    OPGCPCIssueLineVM bd = new OPGCPCIssueLineVM
                    {
                        BatchId = row["batch_id"].ToString(),
                        GSCode = row["gs_code"].ToString(),
                        GrossWt = Convert.ToDecimal(row["gwt"]),
                        StoneWt = Convert.ToDecimal(row["swt"]),
                        NetWt = Convert.ToDecimal(row["nwt"]),
                        AverageRate = Convert.ToDecimal(row["avg_purchase_rate"]),
                        DiamondGS = row["dm_gs_code"].ToString(),
                        StoneGS = row["stn_gs_code"].ToString(),
                        DiamondCarets = Convert.ToDecimal(row["dcts"]),
                        KovaWeight = Convert.ToDecimal(row["kova_wt"]),
                        WastageWeight = Convert.ToDecimal(row["wastage"]),
                        PurityPercent = Convert.ToDecimal(row["purity"]),
                        Amount = Convert.ToDecimal(row["item_amount"]),
                        HSN = HSNCode
                    };
                    return bd;
                }

            }
            catch (Exception ex) {
                errorMessage = new ErrorVM().GetErrorDetails(ex).customDescription;
                return null;
            }
        }

        public bool SaveIssue(OPGCPCIssueHeaderVM cpcIssue, string userID, out int issueNo, out ErrorVM error)
        {
            error = null;
            issueNo = 0;
            try {
                #region 1. Basic Validations
                if (cpcIssue == null) {
                    error = new ErrorVM { description = "Nothing to save.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return false;
                }
                if (cpcIssue.OPGIssueLines == null || cpcIssue.OPGIssueLines.Count <= 0) {
                    error = new ErrorVM { description = "There is no line detail to save.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return false;
                }
                string companyCode = cpcIssue.CompanyCode;
                string branchCode = cpcIssue.BranchCode;
                string errorMessage = string.Empty;

                //Revalidate and check if the batch is valid.
                List<OPGCPCIssueLineVM> dbLineDetails = GetBatchList(companyCode, branchCode, cpcIssue.DocumentNo, out errorMessage);
                foreach (var bd in cpcIssue.OPGIssueLines) {
                    var dbBatchInfo = dbLineDetails.Where(ln => ln.BatchId == bd.BatchId).FirstOrDefault();
                    if (dbBatchInfo == null) {
                        error = new ErrorVM
                        {
                            description = string.Format("The BatchId {0} is invalid or it is already issued.", bd.BatchId),
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                    if(bd.GrossWt != dbBatchInfo.GrossWt || bd.StoneWt != dbBatchInfo.StoneWt
                        || bd.NetWt != dbBatchInfo.NetWt || bd.Amount != dbBatchInfo.Amount) {
                        error = new ErrorVM
                        {
                            description = string.Format("Either GrossWt, StoneWt, NetWt or Amount attribute for BatchId {0} is not correct.", bd.BatchId),
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                    #region Can we skip these validations? YES - since we can directly save the Batch detail from DB.
                    //if (bd.AverageRate <= 0) {
                    //    error = new ErrorVM
                    //    {
                    //        description = string.Format("The Rate attribute is required for BatchId {0}", bd.BatchId),
                    //        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    //    };
                    //    return false;
                    //}
                    //if (bd.Amount <= 0) {
                    //    error = new ErrorVM
                    //    {
                    //        description = string.Format("The Amount attribute is required for BatchId {0}", bd.BatchId),
                    //        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    //    };
                    //    return false;
                    //}
                    //if (bd.GrossWt != dbBatchInfo.GrossWt || bd.NetWt != dbBatchInfo.NetWt || bd.StoneWt != dbBatchInfo.StoneWt) {
                    //    error = new ErrorVM
                    //    {
                    //        description = string.Format("The Gross weight, stone weight and net weight values should be {0}, {1}  and {2} respectively for BatchId {3}",
                    //           dbBatchInfo.GrossWt, dbBatchInfo.StoneWt, dbBatchInfo.NetWt, bd.BatchId),
                    //        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    //    };
                    //    return false;
                    //} 
                    #endregion
                }
                #endregion

                #region 2. Post to Issue Master & Issue Detail Records
                var applicationDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                var updatedTimestamp = SIGlobals.Globals.GetDateTime();

                var storeLocationId = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode
                    && c.branch_code == branchCode).FirstOrDefault().store_location_id;
                var finYear = Globals.GetFinancialYear(db, companyCode, branchCode);
                #region -2.1 Issue Header
                updatedTimestamp = SIGlobals.Globals.GetDateTime();
                issueNo = SIGlobals.Globals.GetDocumentNo(db, companyCode, branchCode, "14", true);
                string issueMasterObjectID = SIGlobals.Globals.GetMagnaGUID("KTTU_ISSUE_MASTER", issueNo, companyCode, branchCode);
                decimal expectedPureWeight = GetExpectedPureweight(companyCode, branchCode, cpcIssue.DocumentNo);
                KTTU_ISSUE_MASTER im = new KTTU_ISSUE_MASTER
                {
                    obj_id = issueMasterObjectID,
                    company_code = companyCode,
                    branch_code = branchCode,
                    issue_no = issueNo,
                    issue_date = applicationDate,
                    sal_code = userID,
                    operator_code = userID,
                    gs_type = cpcIssue.OPGIssueLines[0].GSCode,
                    party_name = cpcIssue.IssueTo,
                    issue_type = "IB",
                    obj_status = "O",
                    cflag = "N",
                    cancelled_by = "",
                    type = "CPC",
                    UpdateOn = updatedTimestamp,
                    remarks = cpcIssue.Remarks,
                    batch_id = "",
                    no_of_tags = 0,
                    tag_weight = 0,
                    FT_Ref_No = 0,
                    grn_no = 0,
                    cancelled_remarks = "",
                    total_value = dbLineDetails.Sum(xm => xm.Amount),
                    Advance_type = "",
                    new_bill_no = issueNo.ToString(),
                    ShiftID = 0,
                    received_from = branchCode,
                    Version = 0,
                    stk_type = "N",
                    old_issue_no = null,
                    import_data_id = null,
                    import_content = null,
                    UniqRowID = Guid.NewGuid(),
                    U_obj_id = "",
                    isReceived = "N",
                    expected_pure_wt = expectedPureWeight,
                    new_no = null,
                    store_location_id = storeLocationId
                };
                
                db.KTTU_ISSUE_MASTER.Add(im);
                #endregion

                #region -2.2 Issue Detail
                int slNo = 1;
                foreach (var ld in dbLineDetails) {
                    updatedTimestamp = SIGlobals.Globals.GetDateTime();
                    GSTAttributeBreakup gsBreackup = new GSTAttributeBreakup();
                    var gsCodeForGSTCalculation = "NOT-REQD";// ld.GSCode; //since GST is not required, I've set some unknown GSCode. You can uncomment this.
                    GetGSTValues(companyCode, branchCode, cpcIssue.IssueTo, gsCodeForGSTCalculation, Convert.ToDecimal(ld.Amount), out gsBreackup);

                    KTTU_ISSUE_DETAILS id = new KTTU_ISSUE_DETAILS
                    {
                        obj_id = issueMasterObjectID,
                        company_code = companyCode,
                        branch_code = branchCode,
                        issue_no = issueNo,
                        sl_no = slNo,
                        item_name = ld.GSCode,
                        code = "0",
                        units = 0,
                        gwt = ld.GrossWt,
                        swt = ld.StoneWt,
                        nwt = ld.NetWt,
                        alloy = 0,
                        receipt_gs_desc = "",
                        purity = ld.PurityPercent,
                        std_weight = 0,
                        wastage = ld.WastageWeight,
                        UpdateOn = updatedTimestamp,
                        oldIssueNo = null,
                        gs_code = ld.GSCode,
                        batch_id = ld.BatchId,
                        opg_receipt_no = 0,
                        counter_code = "",
                        no_tags = 0,
                        tag_wt = 0,
                        dcts = ld.DiamondCarets,
                        design_code = "",
                        batch_name = "",
                        barcode_no = "",
                        ref_no = 0,
                        item_value = ld.Amount,
                        Fin_Year = finYear,
                        receipt_no = 0,
                        total_wt = ld.GrossWt,
                        wastageInActualWeight = 0,
                        Rate = ld.AverageRate,
                        Version = 0,
                        mc_amount = 0,
                        Amount = gsBreackup.AmountAfterTax,
                        mc_for = "",
                        GSTGroupCode = gsBreackup.GSTGroupCode,
                        SGST_Percent = gsBreackup.SGSTPercent,
                        SGST_Amount = gsBreackup.SGSTAmount,
                        CGST_Percent = gsBreackup.CessPercent,
                        CGST_Amount = gsBreackup.CGSTAmount,
                        IGST_Percent = gsBreackup.IGSTPercent,
                        IGST_Amount = gsBreackup.IGSTAmount,
                        HSN = ld.HSN,
                        IsScrap = "",
                        UniqRowID = Guid.NewGuid(),
                        TID = null,
                        U_obj_id = null,
                        isReceived = "N",
                        kova_wt = ld.KovaWeight,
                        stn_gs_code = ld.StoneGS,
                        dm_gs_code = ld.DiamondGS
                    };
                    db.KTTU_ISSUE_DETAILS.Add(id);
                    slNo++;
                }
                #endregion

                #region -2.3 Post GS Issue Stock
                bool stockUpdated = UpdateOPGGSStock(companyCode, branchCode, db, cpcIssue.OPGIssueLines, issueNo, postReverse: false, stockTransactionType: StockTransactionType.Issue, error: out error);
                if (!stockUpdated) {
                    return false;
                }
                #endregion

                #region -2.4 Increment issue series No.
                SIGlobals.Globals.IncrementDocumentNo(db, companyCode, branchCode, "14");
                #endregion

                #endregion

                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        private decimal GetExpectedPureweight(string companyCode, string branchCode, int documentNo)
        {
            decimal expectedPurity = 0;
            try {
                string sql = string.Format("SELECT dbo.[GetExpectedPureWt]('{0}', '{1}', {2}) AS ExpectedPureWt",
                        companyCode, branchCode, documentNo);
                DataTable dt = Globals.GetDataTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                    expectedPurity = Convert.ToDecimal(dt.Rows[0]["ExpectedPureWt"]);
            }
            catch (Exception) {
                throw;
            }
            return expectedPurity;
        }

        private bool GetGSTValues(string companyCode, string branchCode, string supplierCode, string gsCode, decimal calculationValue, out GSTAttributeBreakup gstBreakup)
        {
            gstBreakup = new GSTAttributeBreakup();
            gstBreakup.AmountBeforeTax = calculationValue;
            var companyStateCode = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode && c.branch_code == branchCode).Select(ct => ct.state_code).FirstOrDefault();
            var supplierStateCode = db.KSTU_SUPPLIER_MASTER.Where(c => c.company_code == companyCode && c.branch_code == branchCode
                && c.party_code == supplierCode).Select(ct => ct.state_code).FirstOrDefault();
            bool isInterstate = false;
            if (supplierStateCode != null && supplierStateCode != null && companyStateCode != supplierStateCode) {
                isInterstate = true;
            }

            var gsItemEntry = db.KSTS_GS_ITEM_ENTRY.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                && x.gs_code == gsCode).Select(xs => new
                {
                    GSTGoodsGroupCode = xs.GSTGoodsGroupCode,
                    HSNCode = xs.HSN
                }).FirstOrDefault();
            if (gsItemEntry == null) {
                return false;
            }

            string gstGroupCode = gsItemEntry.GSTGoodsGroupCode;
            string HSNCode = gsItemEntry.HSNCode;
            if (string.IsNullOrEmpty(gstGroupCode))
                return false;

            SalesEstimationBL salesBL = new SalesEstimationBL();
            decimal sgstPercent, sgstAmt, cgstPercent, cgstAmt, igstPercent, igstAmt, cessPercent, cessAmt;
            if (salesBL.GetGSTComponentValues(gstGroupCode, calculationValue, isInterstate, out sgstPercent, out sgstAmt
                , out cgstPercent, out cgstAmt, out igstPercent, out igstAmt, out cessPercent, out cessAmt, companyCode, branchCode)) {
                gstBreakup = new GSTAttributeBreakup
                {
                    GSTGroupCode = gstGroupCode,
                    AmountBeforeTax = calculationValue,
                    SGSTPercent = sgstPercent,
                    SGSTAmount = sgstAmt,
                    CGSTPercent = cgstPercent,
                    CGSTAmount = cgstAmt,
                    IGSTPercent = igstPercent,
                    IGSTAmount = igstAmt,
                    CessPercent = cessPercent,
                    CessAmount = cessAmt,
                    HSN = HSNCode
                };
            }
            return true;
        }

        private bool UpdateOPGGSStock(string companyCode, string branchCode, MagnaDbEntities dbContext, List<OPGCPCIssueLineVM> opgLines, int documentNo, bool postReverse, StockTransactionType stockTransactionType, out ErrorVM error)
        {
            error = null;
            #region Post GS Stock
            var generalStockJournal =
                       from sl in opgLines
                       group sl by new
                       {
                           CompanyCode = companyCode,
                           BranchCode = branchCode,
                           GS = sl.GSCode,
                           Counter = "M",
                           Item = sl.GSCode
                       } into g
                       select new StockJournalVM
                       {
                           StockTransType = stockTransactionType,
                           CompanyCode = g.Key.CompanyCode,
                           BranchCode = g.Key.BranchCode,
                           DocumentNo = documentNo,
                           GS = g.Key.GS,
                           Counter = g.Key.Counter,
                           Item = g.Key.Item,
                           Qty = 0,
                           GrossWt = g.Sum(x => x.GrossWt),
                           StoneWt = g.Sum(x => x.StoneWt),
                           NetWt = g.Sum(x => x.NetWt),
                       };
            StockPostBL stockPost = new StockPostBL();
            string errorMessage = string.Empty;

            var summarizedGSStockJournal =
                    from cj in generalStockJournal
                    group cj by new { Company = cj.CompanyCode, Branch = cj.BranchCode, GS = cj.GS, DocumentNo = cj.DocumentNo } into g
                    select new StockJournalVM
                    {
                        StockTransType = stockTransactionType,
                        CompanyCode = g.Key.Company,
                        BranchCode = g.Key.Branch,
                        DocumentNo = g.Key.DocumentNo,
                        GS = g.Key.GS,
                        Counter = "",
                        Item = "",
                        Qty = g.Sum(x => x.Qty),
                        GrossWt = g.Sum(x => x.GrossWt),
                        StoneWt = g.Sum(x => x.StoneWt),
                        NetWt = g.Sum(x => x.NetWt)
                    };
            bool gsStockPostSuccess = stockPost.GSStockPost(dbContext, summarizedGSStockJournal.ToList(), postReverse, out errorMessage);
            if (!gsStockPostSuccess) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = errorMessage };
                return false;
            }
            #endregion

            return true;
        }

        public bool GenerateXMLFile(string companyCode, string branchCode, int issueNo, out string errorMessage)
        {
            errorMessage = string.Empty;
            try {
                DataSet dsOPGIssueDetails = new DataSet();
                DataTable dtIssueMaster = Globals.GetDataTable(string.Format("SELECT * FROM KTTU_ISSUE_MASTER AS im WHERE im.company_code = '{0}' AND im.branch_code = '{1}' AND im.issue_no = '{2}'",
                    companyCode, branchCode, issueNo));
                DataTable dtIssueDetails = Globals.GetDataTable(string.Format("SELECT * FROM KTTU_ISSUE_DETAILS AS id WHERE id.company_code = '{0}' AND id.branch_code = '{1}' AND id.issue_no = '{2}'",
                    companyCode, branchCode, issueNo));
                dtIssueMaster.TableName = "KTTU_ISSUE_MASTER";
                dtIssueDetails.TableName = "KTTU_ISSUE_DETAILS";

                string issueTo = string.Empty;
                if (dtIssueMaster != null) {
                    dsOPGIssueDetails.Tables.Add(dtIssueMaster);
                    issueTo = dtIssueMaster.Rows[0]["party_name"].ToString();
                }
                if (dtIssueDetails != null)
                    dsOPGIssueDetails.Tables.Add(dtIssueDetails);

                string fpath = string.Format(@"~\App_Data\" + @"Xmls\{0}{1}{2}{3}{4}{5}", "MG_BranchIssueXML_",
                    companyCode, branchCode, issueNo, issueNo, ".xml");
                string filePath = System.Web.HttpContext.Current.Request.MapPath(fpath);
                string folderPath = string.Format(@"~\App_Data" + @"\Xmls");
                Globals.CreateDirectoryIfNotExist(System.Web.HttpContext.Current.Request.MapPath(folderPath));

                if (System.IO.File.Exists(filePath)) {
                    File.SetAttributes(filePath, FileAttributes.Normal);
                    FileInfo file = new FileInfo(filePath);
                    file.Delete();
                }
                dsOPGIssueDetails.WriteXml(filePath, XmlWriteMode.WriteSchema);
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

        public bool CancelIssue(string companyCode, string branchCode, int issueNo, string userID, string cancelRemarks, out ErrorVM error)
        {
            error = null;
            try {
                string issueType = "IB";
                var issueMaster = db.KTTU_ISSUE_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.issue_no == issueNo && x.issue_type == issueType).FirstOrDefault();
                if (issueMaster == null) {
                    error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "The document number is not found." };
                    return false;
                }
                if (issueMaster.cflag == "Y") {
                    error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "The document is already cancelled." };
                    return false;
                }
                var applDate = Globals.GetApplicationDate(companyCode, branchCode);
                if (issueMaster.issue_date.Date < applDate.Date) {
                    error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Only today issues can be cancelled." };
                    return false;
                }

                issueMaster.cancelled_by = userID;
                issueMaster.cancelled_remarks = cancelRemarks;
                issueMaster.cflag = "Y";
                issueMaster.UpdateOn = Globals.GetDateTime();
                db.Entry(issueMaster).State = EntityState.Modified;

                #region Post GS Issue Stock
                var issueDetail = db.KTTU_ISSUE_DETAILS.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.issue_no == issueNo).ToList();
                if (issueDetail == null || issueDetail.Count <= 0) {
                    error = new ErrorVM
                    {
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                        description = string.Format($"No issue deails found for the issue no {issueNo}.")
                    };
                    return false;
                }
                List<OPGCPCIssueLineVM> cpcIssueBatchLines = issueDetail.Select(p => new OPGCPCIssueLineVM
                {
                    GSCode = p.gs_code,
                    GrossWt = Convert.ToDecimal(p.gwt),
                    StoneWt = Convert.ToDecimal(p.swt),
                    NetWt = Convert.ToDecimal(p.nwt)
                }).ToList();
                bool stockUpdated = UpdateOPGGSStock(companyCode, branchCode, db, cpcIssueBatchLines, issueNo, postReverse: true, stockTransactionType: StockTransactionType.Issue, error: out error);
                if (!stockUpdated) {
                    return false;
                }
                #endregion

                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }

            return true;
        }


        public ProdigyPrintVM Print(string companyCode, string branchCode, int issueNo, out ErrorVM error)
        {
            error = null;
            string printData = PrintOPGTOCPCIssue(companyCode, branchCode, issueNo, out error);
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            printObject.PrintType = "HTML";
            printObject.ContinueNextPrint = true;
            printObject.Data = new PrintConfiguration().Base64Encode(printData);
            return printObject;
        }
        protected string PrintOPGTOCPCIssue(string companyCode, string branchCode, int issueNo, out ErrorVM error)
        {
            error = null;
            StringBuilder sb = new StringBuilder();
            try {
//                string strReprintIssue = string.Empty;
//                string new_no = CGlobals.GetNewIssueNumber(Convert.ToString(IssueNo));
//                strReprintIssue = string.Format("select issue_no, issue_date, Salesman,(select sal_name from kstu_salesman_master where sal_code = Salesman and company_code = '{1}' and branch_code = '{2}') as Sname, party_name, issuedFor,\n"
//                         + "party_name,party_code,address1,address2,address3,city,TIN,remarks,company_name,Header1,Header2,tin_no from vIssueMaster \n"
//                         + "where issue_no = {0} and company_code = '{1}' and branch_code = '{2}' and IssueAt = 'CPC'",
//                         IssueNo, CGlobals.CompanyCode, CGlobals.BranchCode);    //Smith Issue

//                DataTable dtIssue = CGlobals.GetDataTable(strReprintIssue);
//                if (dtIssue == null || dtIssue.Rows.Count == 0) {
//                    sbHTML.Append("Issue No does not exists");
//                    return false;
//                }
//                string strIssueDetails = string.Empty;


//                strIssueDetails = string.Format(@"select units,iD.gs_code as item_name,gwt,alloy,std_weight,swt,nwt,dcts ,batch_id,sl_no,isnull(dcts,0) AS [carrat],id.gs_code,item_value,kova_wt,
// isnull(SGST_Amount,0) as SGST_Amount, isnull(CGST_Amount,0) as CGST_Amount, isnull(IGST_Amount,0) as IGST_Amount ,
//    isnull(SGST_Percent,0) as SGST_Percent, isnull(CGST_Percent,0) as CGST_Percent, isnull(IGST_Percent,0) as IGST_Percent , 
//(select distinct HSN from item_master IA where ID.gs_code=IA.gs_code and company_code = 'BH' and branch_code = 'JNR' ) as hsn
// from KTTU_ISSUE_DETAILS ID  where issue_no = {0} and  id.company_code = '{1}' and id.branch_code='{2}' order by sl_no ", IssueNo, CGlobals.CompanyCode, CGlobals.BranchCode);
//                DataTable dtIssueDetails = CGlobals.GetDataTable(strIssueDetails);





//                string comp = string.Format("select cm.company_name,tin_no,pin_code,cst_no,cm.pan_no,cm.state,cm.address1,cm.address2,cm.address3,cm.state_code,cm.city,cm.Header1,cm.phone_no,cm.Header2,cm.Header3,cm.Header4,cm.Header5,cm.Header6,cm.Header7, \n"
//                                + "cm.Footer1,cm.Footer2, email_id,website from KSTU_COMPANY_MASTER cm WHERE cm.company_code = '{0}' and cm.branch_code = '{1}'", CGlobals.CompanyCode, CGlobals.BranchCode);
//                DataTable dtCompanyDetails = CGlobals.GetDataTable(comp);

//                StringBuilder sb = new StringBuilder();
//                string CompanyAddress = CGlobals.GetcompanyDetailsForPrint();
//                string party_code = dtIssue.Rows[0]["party_code"].ToString();
//                string supplierDetails = GetSupplierDetails(party_code);
//                DataTable DtSupplierDetails = CGlobals.GetDataTable(supplierDetails);

//                string Code = string.Format("select tinno from KSTS_STATE_MASTER where state_name='{0}' ", DtSupplierDetails.Rows[0]["state"].ToString());
//                string stateCode = CGlobals.GetStringValue(Code);

//                decimal Grosswt = 0, Netwt = 0, dcts = 0, StnWt = 0, Totatlwt = 0, PureWt = 0, Value = 0, Qty = 0, itemvalue = 0, kovawt = 0;
//                decimal TotalSGST = 0, TotalCGST = 0, TotalIGST = 0;
//                object Itemval = dtIssueDetails.Compute("Sum(item_value)", "");
//                if (Itemval != null && Itemval != DBNull.Value)
//                    Value = Convert.ToDecimal(Itemval);

//                sb.AppendLine("<html>");
//                sb.AppendLine("<head>");
//                sb.AppendLine(GetStyle());
//                sb.AppendLine("</head>");
//                sb.AppendLine("<body>");

//                for (int j = 0; j < 3; j++) {
//                    sb.AppendLine("<TR style=border:0>");
//                    sb.AppendLine(string.Format("<TD style=border:0 ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
//                    sb.AppendLine("</TR>");
//                }


//                sb.AppendLine("<Table frame=\"border\" border=\"0\" width=\"800\" style=\"border-collapse:collapse\" >");
//                sb.AppendLine("<Table font-size=14pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\" border-collapse:collapse; \" width=\"800\">");
//                sb.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");

//                sb.AppendLine("<TR>");
//                sb.AppendLine(string.Format("<TD style=\" border-bottom:thin;\" colspan = 11 ALIGN = \"left\"><b>GSTIN :{0}</b></TD>", dtCompanyDetails.Rows[0]["tin_no"].ToString()));
//                sb.AppendLine("</TR>");

//                sb.AppendLine("<TR>");
//                sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=11 ALIGN = \"CENTER\"><b>{0}</b></TD>", dtCompanyDetails.Rows[0]["company_name"].ToString()));
//                sb.AppendLine("</TR>");
//                sb.AppendLine("<TR>");
//                sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=11 ALIGN = \"CENTER\"><b>{0},{1},{2}-{3}</b></TD>", dtCompanyDetails.Rows[0]["address1"].ToString(), dtCompanyDetails.Rows[0]["address2"].ToString(), dtCompanyDetails.Rows[0]["address3"].ToString(), dtCompanyDetails.Rows[0]["pin_code"].ToString()));
//                sb.AppendLine("</TR>");


//                sb.AppendLine("<TR  style=\"color:black; text-decoration:bold;\" align=\"CENTER\"  style=\"border-left:thin\">");
//                sb.AppendLine(string.Format("<TD width=\"400\" ALIGN = \"LEFT\"><b>DETAILS OF CONSIGNEE </b></TD>"));
//                sb.AppendLine(string.Format("<TD width=\"400\" ALIGN = \"LEFT\"><b>{0}</b></TD>", "ISSUE DETAILS"));
//                sb.AppendLine("</TR>");


//                sb.AppendLine("<tr>");
//                sb.AppendLine("<td>");
//                sb.AppendLine("<Table>");
//                sb.AppendLine("<Table font-size=14pt;  class=\"boldText\"  style=\"border-collapse:collapse;\" >");

//                sb.AppendLine("<tr style=\"border-right:0\"  >");
//                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left \" >Name &nbsp&nbsp</td>");
//                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + DtSupplierDetails.Rows[0]["party_name"].ToString() + "</td>");
//                sb.AppendLine("</tr>");

//                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
//                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" >Address &nbsp&nbsp </td>");
//                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + DtSupplierDetails.Rows[0]["Address1"].ToString() + "</td>");
//                sb.AppendLine("</tr>");
//                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
//                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
//                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + DtSupplierDetails.Rows[0]["Address2"].ToString() + "</td>");
//                sb.AppendLine("</tr>");
//                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
//                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
//                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + DtSupplierDetails.Rows[0]["Address3"].ToString() + "</td>");
//                sb.AppendLine("</tr>");

//                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
//                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" >GSTIN &nbsp&nbsp</td>");
//                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + DtSupplierDetails.Rows[0]["TIN"].ToString() + "</td>");
//                sb.AppendLine("</tr>");

//                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
//                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" >State &nbsp&nbsp</td>");
//                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + DtSupplierDetails.Rows[0]["state"].ToString() + "</td>");
//                sb.AppendLine("</tr>");

//                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
//                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" >State Code &nbsp&nbsp&nbsp </td>");
//                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + stateCode + "</td>");
//                sb.AppendLine("</tr>");

//                sb.AppendLine("</table>");
//                sb.AppendLine("</td>");


//                sb.AppendLine("<td>");
//                sb.AppendLine("<Table font-size=14pt;   class=\"boldText\"  style=\"border-collapse:collapse;\" >");
//                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
//                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >Issue No &nbsp&nbsp</td>");
//                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + new_no + "</td>");
//                sb.AppendLine("</tr>");

//                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
//                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >Ref No &nbsp&nbsp</td>");
//                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + dtIssue.Rows[0]["issue_no"].ToString() + "</td>");
//                sb.AppendLine("</tr>");

//                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
//                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >Issue Date &nbsp&nbsp</td>");
//                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + dtIssue.Rows[0]["issue_date"].ToString() + "</td>");
//                sb.AppendLine("</tr>");

//                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
//                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" > Place of supply &nbsp&nbsp</td>");
//                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + DtSupplierDetails.Rows[0]["state"].ToString() + "</td>");
//                sb.AppendLine("</tr>");

//                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
//                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" >Issued By &nbsp&nbsp</td>");
//                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + dtIssue.Rows[0]["sname"].ToString() + "</td>");
//                sb.AppendLine("</tr>");

//                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
//                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" >Issue Type &nbsp&nbsp</td>");
//                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + "Purification Issue" + "</td>");
//                sb.AppendLine("</tr>");

//                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
//                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >State &nbsp&nbsp</td>");
//                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + dtCompanyDetails.Rows[0]["state"].ToString() + "</td>");
//                sb.AppendLine("</tr>");

//                string StCode = string.Format("select tinno from KSTS_STATE_MASTER where state_name='{0}' ", dtCompanyDetails.Rows[0]["state"].ToString());
//                string DtStCode = CGlobals.GetStringValue(StCode);

//                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
//                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >State Code &nbsp&nbsp</td>");
//                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + DtStCode + "</td>");
//                sb.AppendLine("</tr>");


//                sb.AppendLine("</table>");
//                sb.AppendLine("</td>");
//                sb.AppendLine("</tr>");


//                sb.AppendLine("<TR>");
//                sb.AppendLine(string.Format("<TD style=\" border-bottom:thin;\" colspan = 7  ALIGN = \"CENTER\"><b>DELIVERY CHALLAN</b></TD>"));
//                sb.AppendLine("</TR>");

//                sb.AppendLine("<TR>");
//                sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=7 ALIGN = \"CENTER\"> ISSUED TO JOB WORKER(NOT AS SUPPLY) </TD>"));
//                sb.AppendLine("</TR>");

//                sb.AppendLine("<TR>");
//                sb.AppendLine(string.Format("<TD  style=\"border-right:thin\" colspan=7 ALIGN = \"CENTER\">ORIGINAL / DUPLICATE<br></TD>"));
//                sb.AppendLine("</TR>");

//                sb.AppendLine("</table>");
//                sb.AppendLine("</td>");

//                sb.AppendLine("<Table font-size=14pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse;\" width=\"800\">");  //FRAME=BOX RULES=NONE

//                sb.AppendLine("<TR bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
//                sb.AppendLine("<th style=\"border-top:none; \" align = \"center\">S.No</th>");
//                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">Description</th>");
//                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">HSN</th>");
//                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">Batch ID</th>");
//                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">Gr.Wt(g)</th>");
//                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">N.Wt(g)</th>");
//                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">Dcts</th>");
//                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">Kova.Wt</th>");
//                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">TaxableValue</th>");

//                sb.AppendLine("</TR>");

//                for (int i = 0; i < dtIssueDetails.Rows.Count; i++) {
//                    sb.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
//                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"CENTER\">{0}{1}</TD>", dtIssueDetails.Rows[i]["sl_no"].ToString(), "&nbsp"));
//                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"CENTER\">{0}{1}</TD>", CGlobals.GetBranchGsName(dtIssueDetails.Rows[i]["item_name"].ToString()), "&nbsp"));
//                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"CENTER\">{0}{1}</TD>", dtIssueDetails.Rows[i]["HSN"].ToString(), "&nbsp"));
//                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"CENTER\">{0}{1}</TD>", dtIssueDetails.Rows[i]["batch_id"].ToString(), "&nbsp"));
//                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", dtIssueDetails.Rows[i]["gwt"].ToString(), "&nbsp"));
//                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", dtIssueDetails.Rows[i]["nwt"].ToString(), "&nbsp"));
//                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", dtIssueDetails.Rows[i]["dcts"].ToString(), "&nbsp"));
//                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", dtIssueDetails.Rows[i]["kova_wt"].ToString(), "&nbsp"));
//                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", dtIssueDetails.Rows[i]["item_value"].ToString(), "&nbsp"));
//                    sb.AppendLine("</TR>");
//                    Grosswt += Convert.ToDecimal(dtIssueDetails.Rows[i]["gwt"]);
//                    StnWt += Convert.ToDecimal(dtIssueDetails.Rows[i]["swt"]);
//                    Netwt += Convert.ToDecimal(dtIssueDetails.Rows[i]["nwt"]);
//                    dcts += Convert.ToDecimal(dtIssueDetails.Rows[i]["dcts"]);
//                    kovawt += Convert.ToDecimal(dtIssueDetails.Rows[i]["kova_wt"]);
//                    itemvalue += Convert.ToDecimal(dtIssueDetails.Rows[i]["item_value"]);
//                    PureWt += Convert.ToDecimal(dtIssueDetails.Rows[i]["std_weight"]);
//                    TotalIGST += Convert.ToDecimal(dtIssueDetails.Rows[i]["IGST_Amount"]);
//                }
//                for (int i = 0; i < 8 - dtIssueDetails.Rows.Count - 1; i++) {
//                    if (i == 0) {
//                        sb.AppendLine("<TR>");
//                        sb.AppendLine(string.Format("<TD  style=\"border-left: thin ; border-top: thin soild; border-bottom: thin ; border-right: thin \";  style=font-size=\"14pt\"  colspan = {1}>{0}</TD>", "&nbsp", dtIssueDetails.Columns.Count));
//                        sb.AppendLine("</TR>");
//                    }
//                    else {
//                        sb.AppendLine("<TR>");
//                        sb.AppendLine(string.Format("<TD  style=\"border-left: thin ; border-top: thin ; border-bottom: thin; border-right: thin \"; style=font-size=\"14pt\" colspan = {1}>{0}</TD>", "&nbsp", dtIssueDetails.Columns.Count));
//                        sb.AppendLine("</TR>");
//                    }
//                }
//                sb.AppendLine("<TR  style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
//                sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" colspan = 4 ALIGN = \"left\"><b>TOTAL </b></TD><TD  ALIGN = \"RIGHT\" ><b>{0}</b></TD> <TD  ALIGN = \"RIGHT\" ><b>{1}</b></TD><TD  ALIGN = \"RIGHT\" ><b>{2}</b></TD><TD  ALIGN = \"RIGHT\" ><b>{3}</b></TD><TD  ALIGN = \"RIGHT\" ><b>{4}</b></TD>",
//                           Grosswt, Netwt, dcts, kovawt, itemvalue));
//                sb.AppendLine("</TR>");

//                if (TotalIGST > 0)
//                {
//                    sb.AppendLine("<TR>");
//                    sb.AppendLine(string.Format("<TD  colspan=7 ALIGN = \"right\"><b>IGST Amt @ {0} % : </b></TD>", dtIssueDetails.Rows[0]["IGST_Percent"].ToString()));
//                    sb.AppendLine(string.Format("<TD  style=\"border-right:thin\" ALIGN = \"right\"><b>{0}</b></TD>", TotalIGST));
//                    sb.AppendLine("</TR>");



//                }
//                sb.AppendLine("<TR>");
//                sb.AppendLine(string.Format("<TD  colspan=8 ALIGN = \"right\"><b>{0}  : </b></TD>", "Invoice value "));
//                sb.AppendLine(string.Format("<TD  style=\"border-right:thin\" ALIGN = \"right\"><b>{0}</b></TD>", TotalIGST + itemvalue));
//                sb.AppendLine("</TR>");

//                decimal Inword = itemvalue + TotalIGST;
//                string strWords = string.Empty;
//                strWords = CGlobals.ConvertNumbertoWordsinRupees(Inword);
//                if (!string.IsNullOrEmpty(dtIssue.Rows[0]["remarks"].ToString())) {
//                    sb.AppendLine("<TR>");
//                    sb.AppendLine(string.Format("<TD style=\"border-right:thin \"; colspan=9 ALIGN = \"left\">{0}{1}{1}</TD>", "Remarks  :" + dtIssue.Rows[0]["remarks"].ToString(), "&nbsp"));
//                    sb.AppendLine("</TR>");
//                }

//                sb.AppendLine("<TR>");
//                sb.AppendLine(string.Format("<TD style=\"border-right:thin \"; colspan=9 ALIGN = \"left\">{0}{1}{1}</TD>", strWords, "&nbsp"));
//                sb.AppendLine("</TR>");
//                sb.AppendLine("<TR>");
 
//                sb.AppendLine("<tr>");
//                sb.AppendLine(string.Format("<td  colspan=5  align = \"left\"  style=\"border-right:thin\" ><b>{0}&nbsp<br><br><br>{1}&nbsp</b></td>", "&nbsp", "Signature of Consignee "));
//                sb.AppendLine(string.Format("<td  colspan=4  align = \"right\"  ><b>For {0}&nbsp<br><br><br>{1}&nbsp</b></td>", CGlobals.CompanyName, "Authorized Signatory"));
//                sb.AppendLine("</tr>");
//                sb.AppendLine("</table >");
//                sb.AppendLine("</TR>");

//                sb.AppendLine("</Table>");
//                sb.AppendLine("</body>");
//                sb.AppendLine("</html>");
                return sb.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

    }
}
