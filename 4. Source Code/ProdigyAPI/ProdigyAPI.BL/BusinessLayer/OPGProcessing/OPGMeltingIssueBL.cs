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
    public class OPGMeltingIssueBL
    {
        MagnaDbEntities db = null;

        public OPGMeltingIssueBL()
        {
            db = new MagnaDbEntities(true);
        }

        public OPGMeltingIssueBL(MagnaDbEntities _dbContext)
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
                                && sg.ir_code == "IL"
                            orderby sm.party_name
                            select new ListOfValue
                            {
                                Code = sm.party_code,
                                Name = sm.party_name
                            };

            return partyList.ToList();
        }

        public List<ListOfValue> GetMetalGS(string companyCode, string branchCode, out string errorMessage)
        {
            errorMessage = string.Empty;
            try {
                var metalGS = (from gsEntry in db.KSTS_GS_ITEM_ENTRY
                               join gsGrp in db.KSTU_GS_GROUPING
                               on new { CC = gsEntry.company_code, BC = gsEntry.branch_code, GS = gsEntry.gs_code }
                               equals new { CC = gsGrp.company_code, BC = gsGrp.branch_code, GS = gsGrp.gs_code }
                               where gsEntry.object_status != "C" && gsGrp.obj_status != "C"
                                && gsEntry.company_code == companyCode && gsEntry.branch_code == branchCode
                                && gsGrp.ir_code == "IL"
                               orderby gsEntry.item_level1_name
                               select new ListOfValue
                               {
                                   Code = gsEntry.gs_code,
                                   Name = gsEntry.item_level1_name
                               }).ToList();
                return metalGS;
            }
            catch (Exception ex) {
                errorMessage = new ErrorVM().GetErrorDetails(ex).customDescription;
                return null;
            }
        }

        public List<OPGMeltingIssueBatchDetailVM> GetBatchList(string companyCode, string branchCode, string gsCode, out string errorMessage)
        {
            errorMessage = string.Empty;
            try {
                #region Though this is perfect Linq, this wont' work since there is no key on the View since EF cannot track the entity. I've to discard this code unfortunately.
                //var batchIDList = db.vConversionBatchIds.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                //        && x.gs_code == gsCode).ToList();                
                //if(batchIDList == null || batchIDList.Count <= 0) {
                //    errorMessage = "No batch details found.";
                //    return null;
                //}

                //var batchInfoList = batchIDList.Select(m => new OPGBatchDetailVM
                //{
                //    GSCode = gsCode,
                //    BatchId = m.BatchId,
                //    GrossWt = Convert.ToDecimal(m.gwt),
                //    StoneWt = Convert.ToDecimal(m.swt),
                //    NetWt = Convert.ToDecimal(m.nwt)
                //});

                //return batchInfoList.ToList(); 
                #endregion

                #region Working Code
                string sql = string.Format("SELECT cb.company_code, \n"
                                   + "       cb.branch_code, \n"
                                   + "       cb.gs_code, \n"
                                   + "       cb.gwt, \n"
                                   + "       cb.swt, \n"
                                   + "       cb.nwt, \n"
                                   + "       cb.received_from, \n"
                                   + "       cb.BatchId \n"
                                   + "FROM   [dbo].[vConversionBatchIds] AS cb \n"
                                   + "WHERE  cb.company_code = '{0}' \n"
                                   + "       AND cb.branch_code = '{1}' \n"
                                   + "       AND cb.gs_code = '{2}' ORDER BY cb.BatchId",
                                   companyCode, branchCode, gsCode);
                DataTable dt = Globals.GetDataTable(sql);
                if (dt == null || dt.Rows.Count <= 0) {
                    errorMessage = "No batch details found.";
                    return null;
                }

                decimal purchaseRate = 0;
                var rateMaster = db.KSTU_RATE_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.gs_code == gsCode && x.karat == "24K").FirstOrDefault();
                if (rateMaster != null) {
                    if (rateMaster.bill_type == "P")
                        purchaseRate = rateMaster.exchange_rate;
                    else
                        purchaseRate = rateMaster.rate;
                }

                List<OPGMeltingIssueBatchDetailVM> opgBatchList = new List<OPGMeltingIssueBatchDetailVM>();
                foreach (DataRow row in dt.Rows) {
                    OPGMeltingIssueBatchDetailVM bd = new OPGMeltingIssueBatchDetailVM
                    {
                        BatchId = row["BatchId"].ToString(),
                        GSCode = row["gs_code"].ToString(),
                        GrossWt = Convert.ToDecimal(row["gwt"]),
                        StoneWt = Convert.ToDecimal(row["swt"]),
                        NetWt = Convert.ToDecimal(row["nwt"]),
                        Rate = purchaseRate,
                        Amount = purchaseRate * Convert.ToDecimal(row["nwt"])
                    };
                    opgBatchList.Add(bd);
                }

                return opgBatchList;
                #endregion
            }
            catch (Exception ex) {
                errorMessage = new ErrorVM().GetErrorDetails(ex).customDescription;
                return null;
            }
        }

        public OPGMeltingIssueBatchDetailVM GetBatchInfo(string companyCode, string branchCode, string batchID, out string errorMessage)
        {
            errorMessage = string.Empty;
            try {
                var dbBatchInfo = db.vConversionBatchIds.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                        && x.BatchId == batchID).FirstOrDefault();

                if (dbBatchInfo == null) {
                    errorMessage = "No details found for the batch ID: " + batchID;
                    return null;
                }

                decimal purchaseRate = 0;
                var rateMaster = db.KSTU_RATE_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.gs_code == dbBatchInfo.gs_code && x.karat == "24K").FirstOrDefault();
                if (rateMaster != null) {
                    if (rateMaster.bill_type == "P")
                        purchaseRate = rateMaster.exchange_rate;
                    else
                        purchaseRate = rateMaster.rate;
                }

                var batchInfo = new OPGMeltingIssueBatchDetailVM
                {
                    GSCode = dbBatchInfo.gs_code,
                    BatchId = dbBatchInfo.BatchId,
                    GrossWt = Convert.ToDecimal(dbBatchInfo.gwt),
                    StoneWt = Convert.ToDecimal(dbBatchInfo.swt),
                    NetWt = Convert.ToDecimal(dbBatchInfo.nwt),
                    Rate = purchaseRate,
                    Amount = purchaseRate * Convert.ToDecimal(dbBatchInfo.nwt)
                };
                return batchInfo;
            }
            catch (Exception ex) {
                errorMessage = new ErrorVM().GetErrorDetails(ex).customDescription;
                return null;
            }

        }

        public bool SaveIssue(OPGMeltingIssueHeaderVM meltingIssue, string userID, out int issueNo, out ErrorVM error)
        {
            error = null;
            issueNo = 0;
            try {
                #region 1. Basic Validations
                if (meltingIssue == null) {
                    error = new ErrorVM { description = "Nothing to save.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return false;
                }
                if (meltingIssue.OPGBatchLines == null || meltingIssue.OPGBatchLines.Count <= 0) {
                    error = new ErrorVM { description = "There is no line detail to save.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return false;
                }

                //var distinctCnt = from mi in meltingIssue.OPGBatchLines
                //                  group 
                string companyCode = meltingIssue.CompanyCode;
                string branchCode = meltingIssue.BranchCode;
                string errorMessage = string.Empty;

                //Revalidate and check if the batch is valid.
                foreach (var bd in meltingIssue.OPGBatchLines) {
                    var dbBatchInfo = GetBatchInfo(companyCode, branchCode, bd.BatchId, out errorMessage);
                    if (dbBatchInfo == null) {
                        error = new ErrorVM
                        {
                            description = string.Format("The BatchId {0} is invalid or it is already issued.", bd.BatchId),
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                    if (bd.Rate == null || bd.Rate <= 0) {
                        error = new ErrorVM
                        {
                            description = string.Format("The Rate attribute is required for BatchId {0}", bd.BatchId),
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                    if (bd.Amount == null || bd.Amount <= 0) {
                        error = new ErrorVM
                        {
                            description = string.Format("The Amount attribute is required for BatchId {0}", bd.BatchId),
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                    if (bd.GrossWt != dbBatchInfo.GrossWt || bd.NetWt != dbBatchInfo.NetWt || bd.StoneWt != dbBatchInfo.StoneWt) {
                        error = new ErrorVM
                        {
                            description = string.Format("The Gross weight, stone weight and net weight values should be {0}, {1}  and {2} respectively for BatchId {3}",
                               dbBatchInfo.GrossWt, dbBatchInfo.StoneWt, dbBatchInfo.NetWt, bd.BatchId),
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                    bd.Amount = bd.NetWt * bd.Rate;
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
                KTTU_ISSUE_MASTER im = new KTTU_ISSUE_MASTER
                {
                    obj_id = issueMasterObjectID,
                    company_code = companyCode,
                    branch_code = branchCode,
                    issue_no = issueNo,
                    issue_date = applicationDate,
                    sal_code = userID,
                    operator_code = userID,
                    gs_type = meltingIssue.OPGBatchLines[0].GSCode,
                    party_name = meltingIssue.IssueTo,
                    issue_type = "IL",
                    obj_status = "O",
                    cflag = "N",
                    cancelled_by = "",
                    type = "CPC",
                    UpdateOn = updatedTimestamp,
                    remarks = "",
                    batch_id = "",
                    no_of_tags = 0,
                    tag_weight = 0,
                    FT_Ref_No = 0,
                    grn_no = 0,
                    cancelled_remarks = "",
                    total_value = 0,
                    Advance_type = "M",
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
                    expected_pure_wt = 0,
                    new_no = null,
                    store_location_id = storeLocationId
                };
                db.KTTU_ISSUE_MASTER.Add(im);
                #endregion

                #region -2.2 Issue Detail
                int slNo = 1;
                foreach (var ld in meltingIssue.OPGBatchLines) {
                    updatedTimestamp = SIGlobals.Globals.GetDateTime();
                    GSTAttributeBreakup gsBreackup = new GSTAttributeBreakup();
                    GetGSTValues(companyCode, branchCode, meltingIssue.IssueTo, ld.GSCode, Convert.ToDecimal(ld.Amount), out gsBreackup);

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
                        purity = 0,
                        std_weight = 0,
                        wastage = 0,
                        UpdateOn = updatedTimestamp,
                        oldIssueNo = null,
                        gs_code = ld.GSCode,
                        batch_id = ld.BatchId,
                        opg_receipt_no = 0,
                        counter_code = "",
                        no_tags = 0,
                        tag_wt = 0,
                        dcts = 0,
                        design_code = "",
                        batch_name = "",
                        barcode_no = "",
                        ref_no = 0,
                        item_value = ld.Amount,
                        Fin_Year = finYear,
                        receipt_no = 0,
                        total_wt = ld.GrossWt,
                        wastageInActualWeight = 0,
                        Rate = ld.Rate,
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
                        HSN = gsBreackup.HSN,
                        IsScrap = "",
                        UniqRowID = Guid.NewGuid(),
                        TID = null,
                        U_obj_id = null,
                        isReceived = "N",
                        kova_wt = 0,
                        stn_gs_code = "",
                        dm_gs_code = ""
                    };
                    db.KTTU_ISSUE_DETAILS.Add(id);
                    slNo++;
                }
                #endregion

                #region -2.3 Post GS Issue Stock
                bool stockUpdated = UpdateOPGGSStock(companyCode, branchCode, db, meltingIssue.OPGBatchLines, issueNo, postReverse: false, stockTransactionType: StockTransactionType.Issue, error: out error);
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

        private bool GetGSTValues(string companyCode, string branchCode, string supplierCode, string gsCode, decimal calculationValue, out GSTAttributeBreakup gstBreakup)
        {
            gstBreakup = new GSTAttributeBreakup();
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

        public bool UpdateOPGGSStock(string companyCode, string branchCode, MagnaDbEntities dbContext, List<OPGMeltingIssueBatchDetailVM> opgLines, int documentNo, bool postReverse, StockTransactionType stockTransactionType, out ErrorVM error)
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

        public bool CancelIssue(string companyCode, string branchCode, int issueNo, string userID, string cancelRemarks, out ErrorVM error)
        {
            error = null;
            try {
                string issueType = "IL";
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

                string receiptDocType = "RL";
                var recDocMast = db.KTTU_RECEIPTS_MASTER.Where(y => y.company_code == companyCode && y.branch_code == branchCode
                    && y.issue_no == issueNo && y.receipt_type == receiptDocType && y.cflag == "N").FirstOrDefault();
                if (recDocMast != null) {
                    error = new ErrorVM
                    {
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                        description = string.Format($"The issue no {issueNo} has been received already via receipt No {recDocMast.receipt_no}.")
                    };
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
                List<OPGMeltingIssueBatchDetailVM> opgBatchLines = issueDetail.Select(p => new OPGMeltingIssueBatchDetailVM
                {
                    GSCode = p.gs_code,
                    GrossWt = Convert.ToDecimal(p.gwt),
                    StoneWt = Convert.ToDecimal(p.swt),
                    NetWt = Convert.ToDecimal(p.nwt)
                }).ToList();
                bool stockUpdated = UpdateOPGGSStock(companyCode, branchCode, db, opgBatchLines, issueNo, postReverse: true, stockTransactionType: StockTransactionType.Issue, error: out error);
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
            string printData = PrintOPGIssue(companyCode, branchCode, issueNo, out error);
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            printObject.PrintType = "HTML";
            printObject.ContinueNextPrint = true;
            printObject.Data = new PrintConfiguration().Base64Encode(printData);
            return printObject;
        }
        protected string PrintOPGIssue(string companyCode, string branchCode, int issueNo, out ErrorVM error)
        {
            error = null;
            StringBuilder sb = new StringBuilder();
            try {
                var dtIssue = (from im in db.KTTU_ISSUE_MASTER
                               join sm in db.KSTU_SUPPLIER_MASTER on new
                               {
                                   CompanyCode = im.company_code,
                                   BranchCode = im.branch_code,
                                   PartyCode = im.party_name
                               } equals new
                               {
                                   CompanyCode = sm.company_code,
                                   BranchCode = sm.branch_code,
                                   PartyCode = sm.party_code
                               }
                               join ir in db.KSTU_ISSUERECEIPTS_TYPES on new
                               {
                                   CompanyCode = im.company_code,
                                   BranchCode = im.branch_code,
                                   IRCode = im.issue_type
                               } equals new
                               {
                                   CompanyCode = ir.company_code,
                                   BranchCode = ir.branch_code,
                                   IRCode = ir.ir_code
                               }
                               join cp in db.KSTU_COMPANY_MASTER on new
                               {
                                   CompanyCode = im.company_code,
                                   BranchCode = im.branch_code
                               } equals new
                               {
                                   CompanyCode = cp.company_code,
                                   BranchCode = cp.branch_code
                               }
                               join smn in db.KSTU_SALESMAN_MASTER on new
                               {
                                   CompanyCode = im.company_code,
                                   BranchCode = im.branch_code,
                                   SalCode = im.sal_code
                               } equals new
                               {
                                   CompanyCode = smn.company_code,
                                   BranchCode = smn.branch_code,
                                   SalCode = smn.sal_code
                               }
                               into viewDetails
                               from smn in viewDetails.DefaultIfEmpty()
                               where im.issue_no == issueNo && im.company_code == companyCode && im.branch_code == branchCode && im.type=="CPC"
                               select new
                               {
                                   im.issue_no,
                                   im.issue_date,
                                   im.issue_type,
                                   im.sal_code,
                                   im.gs_type,
                                   ir.ir_name,
                                   sm.party_name,
                                   sm.party_code,
                                   sm.address1,
                                   sm.address2,
                                   sm.address3,
                                   sm.city,
                                   sm.TIN,
                                   im.remarks,
                                   im.company_code,
                                   im.branch_code,
                                   im.type,
                                   cp.company_name,
                                   cp.Header1,
                                   cp.Header2,
                                   cp.tin_no,
                                   cp.cst_no,
                                   cp.store_location_id,
                                   smn.sal_name
                               }).FirstOrDefault();
                var dtIssueDetails = db.KTTU_ISSUE_DETAILS.Where(i => i.company_code == companyCode
                                                                && i.branch_code == branchCode
                                                                && i.issue_no == issueNo).OrderBy(i => i.sl_no).ToList();
                decimal Grosswt = 0, Netwt = 0, StnWt = 0, Totatlwt = 0, PureWt = 0, Value = 0, Qty = 0, itemvalue = 0;
                object Itemval = dtIssueDetails.Sum(i => i.item_value);
                if (Itemval != null && Itemval != DBNull.Value)
                    Value = Convert.ToDecimal(Itemval);

                KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode && c.branch_code == branchCode).FirstOrDefault();
                KSTU_SUPPLIER_MASTER supplier = SIGlobals.Globals.GetSupplier(db, companyCode, branchCode, dtIssue.party_code);
                KSTS_STATE_MASTER state = db.KSTS_STATE_MASTER.Where(st => st.state_name == supplier.state).FirstOrDefault();
                string stateCode = Convert.ToString(state.tinno);

                sb.AppendLine("<html>");
                sb.AppendLine("<head>");
                sb.AppendLine(SIGlobals.Globals.GetStyle());
                sb.AppendLine("</head>");
                sb.AppendLine("<body>");

                for (int j = 0; j < 3; j++) {
                    sb.AppendLine("<TR style=border:0>");
                    sb.AppendLine(string.Format("<TD style=border:0 ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    sb.AppendLine("</TR>");
                }

                sb.AppendLine("<Table frame=\"border\" border=\"0\" width=\"800\" style=\"border-collapse:collapse\" >");
                sb.AppendLine("<Table font-size=14pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\" border-collapse:collapse; \" width=\"800\">");
                sb.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\" border-bottom:thin;\" colspan = 10 ALIGN = \"left\"><b>GSTIN :{0}</b></TD>", company.tin_no));
                sb.AppendLine("</TR>");

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"CENTER\"><b>{0}</b></TD>", company.company_name));
                sb.AppendLine("</TR>");
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"CENTER\"><b>{0},{1},{2}-{3}</b></TD>", company.address1, company.address2, company.address3, company.pin_code));
                sb.AppendLine("</TR>");


                sb.AppendLine("<TR  style=\"color:black; text-decoration:bold;\" align=\"CENTER\"  style=\"border-left:thin\">");
                sb.AppendLine(string.Format("<TD width=\"400\" ALIGN = \"LEFT\"><b>DETAILS OF CONSIGNEE </b></TD>"));
                sb.AppendLine(string.Format("<TD width=\"400\" ALIGN = \"LEFT\"><b>{0}</b></TD>", "ISSUE DETAILS"));
                sb.AppendLine("</TR>");


                sb.AppendLine("<tr>");
                sb.AppendLine("<td>");
                sb.AppendLine("<Table>");
                sb.AppendLine("<Table font-size=14pt;  class=\"boldText\"  style=\"border-collapse:collapse;\" >");

                sb.AppendLine("<tr style=\"border-right:0\"  >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left \" >Name &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + supplier.party_name + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" >Address &nbsp&nbsp </td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + supplier.address1 + "</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + supplier.address2 + "</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + supplier.address3 + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" >GSTIN &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + supplier.TIN + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" >State &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + supplier.state + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" >State Code &nbsp&nbsp&nbsp </td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + stateCode + "</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("</table>");

                sb.AppendLine("</td>");
                sb.AppendLine("<td>");
                sb.AppendLine("<Table font-size=14pt;   class=\"boldText\"  style=\"border-collapse:collapse;\" >");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >Issue No &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + issueNo + "</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >Issue Date &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + dtIssue.issue_date + "</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" > Place of supply &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + supplier.state + "</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" >Issued By &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + dtIssue.sal_name + "</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" >Issue Type &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + "Melting Issue" + "</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >State &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + company.state + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >State Code &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + stateCode + "</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("</table>");
                sb.AppendLine("</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\" border-bottom:thin;\" colspan = 6  ALIGN = \"CENTER\"><b>DELIVERY CHALLAN</b></TD>"));
                sb.AppendLine("</TR>");
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=6 ALIGN = \"CENTER\"> ISSUED TO JOB WORKER(NOT AS SUPPLY) </TD>"));
                sb.AppendLine("</TR>");
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD  style=\"border-right:thin\" colspan=6 ALIGN = \"CENTER\">ORIGINAL / DUPLICATE<br></TD>"));
                sb.AppendLine("</TR>");
                sb.AppendLine("</table>");
                sb.AppendLine("</td>");
                sb.AppendLine("<Table font-size=14pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse;\" width=\"800\">");
                sb.AppendLine("<TR bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sb.AppendLine("<th style=\"border-top:none; \" align = \"center\">S.No</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">Description</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">HSN</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">Batch Id</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">Gr.Wt(g)</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">N.Wt(g)</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">Value</th>");
                sb.AppendLine("</TR>");
                for (int i = 0; i < dtIssueDetails.Count; i++) {
                    sb.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"CENTER\">{0}{1}</TD>", dtIssueDetails[i].sl_no, "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"CENTER\">{0}{1}</TD>", SIGlobals.Globals.GetBranchGSName(db, companyCode, branchCode, dtIssueDetails[i].item_name.ToString()), "&nbsp"));

                    //string hsn = string.Format("select hsn from ITEM_MASTER where gs_code='{0}' and company_code = '{1}' and branch_code = '{2}'", dtIssueDetails.Rows[i]["item_name"].ToString(), CGlobals.CompanyCode, CGlobals.BranchCode);
                    //string hsnValue = CGlobals.GetStringValue(hsn);

                    string hsnValue = SIGlobals.Globals.GetHSN(db, dtIssueDetails[i].item_name, companyCode, branchCode);
                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"CENTER\">{0}{1}</TD>", hsnValue, "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", dtIssueDetails[i].batch_id.ToString(), "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", dtIssueDetails[i].gwt.ToString(), "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", dtIssueDetails[i].nwt.ToString(), "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", dtIssueDetails[i].item_value.ToString(), "&nbsp"));
                    sb.AppendLine("</TR>");
                    Grosswt += Convert.ToDecimal(dtIssueDetails[i].gwt);
                    StnWt += Convert.ToDecimal(dtIssueDetails[i].swt);
                    Netwt += Convert.ToDecimal(dtIssueDetails[i].nwt);
                    itemvalue += Convert.ToDecimal(dtIssueDetails[i].item_value);
                    PureWt += Convert.ToDecimal(dtIssueDetails[i].std_weight);
                }
                for (int i = 0; i < 10 - dtIssueDetails.Count - 1; i++) {
                    if (i == 0) {
                        sb.AppendLine("<TR>");
                        sb.AppendLine(string.Format("<TD  style=\"border-left: thin ; border-top: thin soild; border-bottom: thin ; border-right: thin \";  style=font-size=\"14pt\"  colspan = {1}>{0}</TD>", "&nbsp", 12));
                        sb.AppendLine("</TR>");
                    }
                    else {
                        sb.AppendLine("<TR>");
                        sb.AppendLine(string.Format("<TD  style=\"border-left: thin ; border-top: thin ; border-bottom: thin; border-right: thin \"; style=font-size=\"14pt\" colspan = {1}>{0}</TD>", "&nbsp", 12));
                        sb.AppendLine("</TR>");
                    }
                }
                sb.AppendLine("<TR  style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" colspan = 4 ALIGN = \"left\"><b>TOTAL </b></TD><TD  ALIGN = \"RIGHT\" ><b>{0}</b></TD> <TD  ALIGN = \"RIGHT\" ><b>{1}</b></TD><TD  ALIGN = \"RIGHT\" ><b>{2}</b></TD>",
                           Grosswt, Netwt, itemvalue));
                sb.AppendLine("</TR>");


                decimal Inword = itemvalue;
                string strWords = string.Empty;
                strWords = SIGlobals.Globals.ConvertNumbertoWordsinRupees(Inword);
                if (!string.IsNullOrEmpty(dtIssue.remarks.ToString())) {
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD style=\"border-right:thin \"; colspan=7 ALIGN = \"left\">{0}{1}{1}</TD>", "Remarks : " + dtIssue.remarks.ToString(), "&nbsp"));
                    sb.AppendLine("</TR>");
                }
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-right:thin \"; colspan=7 ALIGN = \"left\">{0}{1}{1}</TD>", strWords, "&nbsp"));
                sb.AppendLine("</TR>");
                sb.AppendLine("<TR>");
                sb.AppendLine("<tr>");
                sb.AppendLine(string.Format("<td   colspan=4  align = \"left\"  style=\"border-right:thin\" ><b>{0}&nbsp<br><br><br>{1}&nbsp</b></td>", "&nbsp", "Signature of Consignee "));
                sb.AppendLine(string.Format("<td  colspan=4  align = \"right\"  ><b>For {0}&nbsp<br><br><br>{1}&nbsp</b></td>", company.company_name, "Authorized Signatory"));
                sb.AppendLine("</tr>");
                sb.AppendLine("</table >");
                sb.AppendLine("</TR>");
                sb.AppendLine("</Table>");
                sb.AppendLine("</body>");
                sb.AppendLine("</html>");
                return sb.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }
    }
}
