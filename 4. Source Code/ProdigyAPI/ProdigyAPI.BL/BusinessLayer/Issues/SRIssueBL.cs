using ProdigyAPI.BL.BusinessLayer.PrintConfig;
using ProdigyAPI.BL.BusinessLayer.Stock;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Issues;
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

namespace ProdigyAPI.BL.BusinessLayer.Issues
{
    public class SRIssueBL
    {
        MagnaDbEntities db = null;

        public SRIssueBL()
        {
            db = new MagnaDbEntities(true);
        }

        public SRIssueBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }

        public dynamic GetIssueToList(string companyCode, string branchCode)
        {
            var partyList = db.KSTU_SUPPLIER_MASTER
                .Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.voucher_code == "VB" && x.obj_status != "C" && x.party_code == "CPC")
                .OrderBy(y => y.party_name)
                .Select(z => new { Code = z.party_code, Name = z.party_name }).ToList();
            return partyList;
        }

        public SRIssueVM GetSRDetails(SRIssueQueryVM srIssueQuery, out ErrorVM error)
        {
            error = null;
            if (srIssueQuery == null) {
                error = new ErrorVM { description = "Nothing to query.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return null;
            }
            SRIssueVM srIssue = new SRIssueVM();
            var srItems = db.usp_GetSRItemsForIssue1(srIssueQuery.CompanyCode, srIssueQuery.BranchCode, srIssueQuery.GSCode,
                srIssueQuery.FromDate, srIssueQuery.ToDate).ToList();
            if (srItems == null) {
                error = new ErrorVM { description = "No SR item details found.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return null;
            }

            var srStones = db.usp_GetSRStoneItemsForIssue1(srIssueQuery.CompanyCode, srIssueQuery.BranchCode, srIssueQuery.GSCode,
                srIssueQuery.FromDate, srIssueQuery.ToDate).ToList();
            srIssue.CompanyCode = srIssueQuery.CompanyCode;
            srIssue.BranchCode = srIssueQuery.BranchCode;
            srIssue.IssueTo = srIssueQuery.IssueTo;
            srIssue.GSCode = srIssueQuery.GSCode;
            srIssue.IssueTo = srIssueQuery.IssueTo;

            srIssue.IssueLines = new List<SRIssueLineVM>();
            foreach (var x in srItems) {
                SRIssueLineVM LineItem = new SRIssueLineVM
                {
                    BillNo = x.sales_bill_no,
                    SlNo = x.sl_no,
                    GSCode = x.gs_code,
                    CounterCode = x.counter_code,
                    BarcodeNo = x.Barcode,
                    ItemCode = x.item_name,
                    Qty = Convert.ToInt32(x.quantity),
                    GrossWt = Convert.ToDecimal(x.gwt),
                    NetWt = Convert.ToDecimal(x.nwt),
                    StoneWt = x.swt,
                    AmountBeforeTax = x.net_amount,
                    AmountAfterTax = x.net_amount,
                    OtherLineAttributes = new OtherSRLineAttributes
                    {
                        BatchID = x.batch_id,
                        DesignCode = x.design_code,
                        ItemSize = x.item_size,
                        Supplier = x.Supplier,
                        PurchaseRate = x.pur_rate,
                        StoneCharges = x.stone_charges,
                        DiamondCharges = x.diamond_charges,
                        VAAmount = x.va_amount,
                        PurchaseMCGram = x.pur_mc_gram,
                        PurchaseMCType = x.pur_mc_type,
                        PurchaseWastageType = x.pur_wastage_type,
                        PurchaseWastageValue = x.pur_wastage_type_value
                    },
                    StoneDetails = new List<SRIssueStoneDetailVM>()
                };

                if (srStones != null) {
                    var itemStones = srStones.Where(m => m.sales_bill_no == x.sales_bill_no && m.item_sno == x.sl_no).ToList();
                    if (itemStones != null) {
                        foreach (var its in itemStones) {
                            SRIssueStoneDetailVM _sd = new SRIssueStoneDetailVM
                            {
                                SlNo = its.sno,
                                BillNo = its.sales_bill_no,
                                ItemSlNo = its.item_sno,
                                GS = its.Type,
                                Name = its.Name,
                                Rate = its.rate,
                                Qty = its.qty,
                                Carrat = its.carrat,
                                Amount = its.amount
                            };
                            LineItem.StoneDetails.Add(_sd);
                        }
                    }
                }
                srIssue.IssueLines.Add(LineItem);
            }

            srIssue.Qty = srIssue.IssueLines.Sum(p => p.Qty);
            srIssue.GrossWt = srIssue.IssueLines.Sum(p => p.GrossWt);
            srIssue.StoneWt = srIssue.IssueLines.Sum(p => p.StoneWt);
            srIssue.NetWt = srIssue.IssueLines.Sum(p => p.NetWt);
            srIssue.AmountBeforeTax = srIssue.IssueLines.Sum(p => p.AmountBeforeTax);
            srIssue.AmountAfterTax = srIssue.IssueLines.Sum(p => p.AmountAfterTax);

            return srIssue;
        }

        public bool PostIssue(SRIssueQueryVM srQuery, string userID, out int issueNo, out ErrorVM error)
        {
            error = null;
            issueNo = 0;
            var srDetail = GetSRDetails(srQuery, out error);
            if (error != null) {
                return false;
            }
            if (srDetail.IssueLines == null || srDetail.IssueLines.Count <= 0) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Issue line details are not found. At least one line is required to post." };
                return false;
            }

            var functionResult = SaveIssue(srDetail, userID, out issueNo, out error);
            if (!functionResult) {
                return false;
            }

            return true;
        }

        private bool SaveIssue(SRIssueVM issueDetail, string userID, out int issueNo, out ErrorVM error)
        {
            error = null;
            issueNo = 0;
            try {
                string companyCode = issueDetail.CompanyCode;
                string branchCode = issueDetail.BranchCode;
                var applicationDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                var updatedTimestamp = SIGlobals.Globals.GetDateTime();

                var storeLocationId = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode
                    && c.branch_code == branchCode).FirstOrDefault().store_location_id;

                #region Issue Header
                issueNo = SIGlobals.Globals.GetDocumentNo(db, companyCode, branchCode, "28", true);
                string issueMasterObjectID = SIGlobals.Globals.GetMagnaGUID("KTTU_SR_ISSUE_MASTER", issueNo, companyCode, branchCode);
                KTTU_SR_ISSUE_MASTER im = new KTTU_SR_ISSUE_MASTER();
                im.obj_id = issueMasterObjectID;
                im.company_code = companyCode;
                im.branch_code = branchCode;
                im.issue_no = issueNo;
                im.issued_to = issueDetail.IssueTo;
                im.issued_by = userID;
                im.issued_date = applicationDate;
                im.operator_code = userID;
                im.gs_code = issueDetail.GSCode;
                im.cflag = "N";
                im.cancelled_by = "";
                im.UpdateOn = updatedTimestamp;
                im.cancelled_remarks = "";
                im.New_Bill_No = issueNo;
                im.ShiftID = 0;
                im.UniqRowID = Guid.NewGuid();
                im.amount = issueDetail.AmountBeforeTax;
                im.final_amount = issueDetail.AmountAfterTax;
                im.SGST_Percent = 0;
                im.IGST_Percent = 0;
                im.CGST_Percent = 0;
                im.IGST_Amount = issueDetail.IGSTAmount;
                im.CGST_Amount = 0;
                im.SGST_Amount = 0;
                im.GSTGroupCode = "";

                #endregion

                #region Issue Detail
                int slNo = 1;
                var finYear = Globals.GetFinancialYear(db, companyCode, branchCode);
                List<SRIssueLineVM> issueLines = issueDetail.IssueLines;
                foreach (var x in issueDetail.IssueLines) {
                    updatedTimestamp = SIGlobals.Globals.GetDateTime();

                    KTTU_SR_ISSUE_DETAILS id = new KTTU_SR_ISSUE_DETAILS();
                    id.obj_id = issueMasterObjectID;
                    id.company_code = companyCode;
                    id.branch_code = branchCode;
                    id.issue_no = issueNo;
                    id.sl_no = slNo;
                    id.sales_bill_no = x.BillNo;
                    id.gs_code = x.GSCode;
                    id.barcode_no = x.BarcodeNo;
                    id.item_name = x.ItemCode;
                    id.item_no = Convert.ToInt32(x.Qty);
                    id.gwt = Convert.ToDecimal(x.GrossWt);
                    id.swt = Convert.ToDecimal(x.StoneWt);
                    id.nwt = Convert.ToDecimal(x.NetWt);
                    id.counter_code = x.CounterCode;
                    id.stone_charges = Convert.ToDecimal(x.OtherLineAttributes.StoneCharges);
                    id.diamond_charges = Convert.ToDecimal(x.OtherLineAttributes.DiamondCharges);
                    id.net_amount = x.AmountBeforeTax;
                    id.va_amount = Convert.ToDecimal(x.OtherLineAttributes.VAAmount);
                    id.Fin_Year = finYear;
                    id.supplier_code = x.OtherLineAttributes.Supplier;
                    id.batch_id = x.OtherLineAttributes.BatchID;
                    id.design_code = x.OtherLineAttributes.DesignCode;
                    id.counter_code = x.CounterCode;
                    id.item_size_code = x.OtherLineAttributes.ItemSize;
                    id.pur_rate = x.OtherLineAttributes.PurchaseRate;
                    id.pur_wastage_type = x.OtherLineAttributes.PurchaseWastageType;
                    id.pur_wastage_type_value = x.OtherLineAttributes.PurchaseRate;
                    id.pur_mc_type = x.OtherLineAttributes.PurchaseMCType;
                    id.pur_mc_gram = x.OtherLineAttributes.PurchaseMCGram;
                    id.UniqRowID = Guid.NewGuid();

                    db.KTTU_SR_ISSUE_DETAILS.Add(id);

                    int stoneSerialNo = 1;
                    if (x.StoneDetails != null) {

                        foreach (var sd in x.StoneDetails) {
                            KTTU_SR_ISSUE_STONE_DETAILS stoneDet = new KTTU_SR_ISSUE_STONE_DETAILS
                            {
                                obj_id = issueMasterObjectID,
                                company_code = companyCode,
                                branch_code = branchCode,
                                issue_no = issueNo,
                                sales_bill_no = x.BillNo,
                                item_slno = slNo,
                                sl_no = stoneSerialNo,
                                gs_code = sd.GS,
                                item_name = sd.Name,
                                qty = Convert.ToInt32(sd.Qty),
                                carrat = Convert.ToDecimal(sd.Carrat),
                                rate = Convert.ToDecimal(sd.Rate),
                                amount = Convert.ToDecimal(sd.Amount),
                                Fin_Year = finYear,
                                barcode_no = x.BarcodeNo,
                                UniqRowID = Guid.NewGuid()
                            };

                            db.KTTU_SR_ISSUE_STONE_DETAILS.Add(stoneDet);
                            stoneSerialNo++;
                        }
                    }
                    slNo++;
                }
                db.KTTU_SR_ISSUE_MASTER.Add(im);
                #endregion

                #region Increment Serial No.
                SIGlobals.Globals.IncrementDocumentNo(db, companyCode, branchCode, "28");
                #endregion

                #region Stock + Counter Issue posting
                var stockUpdateReqd = SIGlobals.Globals.GetApplicationConfigurationSettnigs(db, companyCode, branchCode, "24032018");
                if (stockUpdateReqd != null && stockUpdateReqd == 1) {
                    bool stockUpdated = UpdateItemStock(companyCode, branchCode, db, issueLines, issueNo, postReverse: false, error: out error);
                    if (!stockUpdated) {
                        return false;
                    }

                    bool issuePosted = PostCounterIssue(companyCode, branchCode, db, issueLines, userID, issueNo, out error);
                    if (!issuePosted) {
                        return false;
                    }
                }
                #endregion

                #region Accounts Posting
                var accPostingReq = SIGlobals.Globals.GetApplicationConfigurationSettnigs(db, companyCode, branchCode, "2003");
                var accValuePostingReq = SIGlobals.Globals.GetApplicationConfigurationSettnigs(db, companyCode, branchCode, "20200915");
                //TODO: Check this and post as per the configuration. Pending
                #endregion

                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        private bool UpdateItemStock(string companyCode, string branchCode, MagnaDbEntities dbContext, List<SRIssueLineVM> issueLines, int issueNo, bool postReverse, out ErrorVM error)
        {
            error = null;
            #region Post Counter Stock
            var generalStockJournal =
                       from sl in issueLines
                       group sl by new
                       {
                           CompanyCode = companyCode,
                           BranchCode = branchCode,
                           GS = sl.GSCode,
                           Counter = sl.CounterCode,
                           Item = sl.ItemCode
                       } into g
                       select new StockJournalVM
                       {
                           StockTransType = StockTransactionType.Issue,
                           CompanyCode = g.Key.CompanyCode,
                           BranchCode = g.Key.BranchCode,
                           DocumentNo = issueNo,
                           GS = g.Key.GS,
                           Counter = g.Key.Counter,
                           Item = g.Key.Item,
                           Qty = g.Sum(x => x.Qty),
                           GrossWt = g.Sum(x => x.GrossWt),
                           StoneWt = g.Sum(x => x.StoneWt),
                           NetWt = g.Sum(x => x.NetWt),
                       };
            StockPostBL stockPost = new StockPostBL();
            string errorMessage = string.Empty;
            bool counterStockPostSuccess = stockPost.CounterStockPost(dbContext, generalStockJournal.ToList(), postReverse, out errorMessage);
            if (!counterStockPostSuccess) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = errorMessage };
                return false;
            }
            #endregion

            #region Post GS Stock
            var summarizedGSStockJournal =
                    from cj in generalStockJournal
                    group cj by new { Company = cj.CompanyCode, Branch = cj.BranchCode, GS = cj.GS, DocumentNo = cj.DocumentNo } into g
                    select new StockJournalVM
                    {
                        StockTransType = StockTransactionType.Issue,
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

        public bool PostCounterIssue(string companyCode, string branchCode, MagnaDbEntities dbContext, List<SRIssueLineVM> issueLines, string userID, int issueNo, out ErrorVM error)
        {
            error = null;
            var applicationDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode); ;
            foreach (var issLine in issueLines) {
                int cntrIssueNo = SIGlobals.Globals.GetDocumentNo(dbContext, companyCode, branchCode, "26", true);
                if (cntrIssueNo <= 0) {
                    error = new ErrorVM { description = "Failed to get counter issue document number.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return false;
                }
                KTTU_COUNTER_ISSUE counterIssue = new KTTU_COUNTER_ISSUE();
                string objID = SIGlobals.Globals.GetMagnaGUID("KTTU_COUNTER_ISSUE", cntrIssueNo, companyCode, branchCode);
                counterIssue.obj_id = objID;
                counterIssue.issue_no = cntrIssueNo;
                counterIssue.company_code = companyCode;
                counterIssue.branch_code = branchCode;
                counterIssue.gs_code = issLine.GSCode;
                counterIssue.item_name = issLine.ItemCode;
                counterIssue.counter_code = issLine.CounterCode;
                counterIssue.issues_gwt = issLine.GrossWt;
                counterIssue.issues_nwt = issLine.NetWt;
                counterIssue.issues_swt = issLine.StoneWt;
                counterIssue.issues_units = issLine.Qty;
                counterIssue.operator_code = userID;
                var updatedTime = SIGlobals.Globals.GetDateTime();
                counterIssue.issued_date = applicationDate;
                counterIssue.UpdateOn = updatedTime;
                counterIssue.UniqRowID = Guid.NewGuid();
                counterIssue.remarks = string.Format("S/R WEIGHT SHORT TO CPC/{0:dd-MMM-yyyy hh:mm:ss} /REF:{1}", updatedTime, issueNo);

                dbContext.KTTU_COUNTER_ISSUE.Add(counterIssue);
                SIGlobals.Globals.IncrementDocumentNo(dbContext, companyCode, branchCode, "26");

            }
            return true;
        }

        public bool PostCounterReceipt(string companyCode, string branchCode, MagnaDbEntities dbContext, List<SRIssueLineVM> issueLines, string userID, int issueNo, out ErrorVM error)
        {
            error = null;
            var applicationDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode); ;
            foreach (var issLine in issueLines) {
                int cntrIssueNo = SIGlobals.Globals.GetDocumentNo(dbContext, companyCode, branchCode, "33", true);
                if (cntrIssueNo <= 0) {
                    error = new ErrorVM { description = "Failed to get counter issue document number.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return false;
                }
                KTTU_COUNTER_RECEIPT counterReceipt = new KTTU_COUNTER_RECEIPT();
                string objID = SIGlobals.Globals.GetMagnaGUID("KTTU_COUNTER_RECEIPT", cntrIssueNo, companyCode, branchCode);
                counterReceipt.obj_id = objID;
                counterReceipt.receipt_no = cntrIssueNo;
                counterReceipt.company_code = companyCode;
                counterReceipt.branch_code = branchCode;
                counterReceipt.gs_code = issLine.GSCode;
                counterReceipt.item_name = issLine.ItemCode;
                counterReceipt.counter_code = issLine.CounterCode;
                counterReceipt.receipt_gwt = issLine.GrossWt;
                counterReceipt.receipt_nwt = issLine.NetWt;
                counterReceipt.receipt_swt = issLine.StoneWt;
                counterReceipt.receipt_units = issLine.Qty;
                counterReceipt.operator_code = userID;
                var updatedTime = SIGlobals.Globals.GetDateTime();
                counterReceipt.receipt_date = applicationDate;
                counterReceipt.UpdateOn = updatedTime;
                counterReceipt.UniqRowID = Guid.NewGuid();
                counterReceipt.remarks = string.Format("SR ISSUE CANCEL EXCESS TO COUNTER ON:{0:dd-MMM-yyyy hh:mm:ss} REF:{1}", updatedTime, issueNo);

                dbContext.KTTU_COUNTER_RECEIPT.Add(counterReceipt);
                SIGlobals.Globals.IncrementDocumentNo(dbContext, companyCode, branchCode, "33");

            }
            return true;
        }

        public bool GenerateSRXMLFile(string companyCode, string branchCode, int issueNo, out string errorMessage)
        {
            errorMessage = string.Empty;
            try {
                DataSet dsSRIssue = new DataSet();
                DataTable dtMasterDetails = Globals.GetDataTable(string.Format("SELECT * FROM KTTU_SR_ISSUE_MASTER AS im WHERE im.company_code = '{0}' AND im.branch_code = '{1}' AND im.issue_no = '{2}'",
                    companyCode, branchCode, issueNo));
                DataTable dtIssueDetails = Globals.GetDataTable(string.Format("SELECT * FROM KTTU_SR_ISSUE_DETAILS AS id WHERE id.company_code = '{0}' AND id.branch_code = '{1}' AND id.issue_no = '{2}'",
                    companyCode, branchCode, issueNo));
                DataTable dtStoneDetails = Globals.GetDataTable(string.Format("SELECT * FROM KTTU_SR_ISSUE_STONE_DETAILS AS id WHERE id.company_code = '{0}' AND id.branch_code = '{1}' AND id.issue_no = '{2}'",
                    companyCode, branchCode, issueNo));
                dtMasterDetails.TableName = "KTTU_SR_ISSUE_MASTER";
                dtIssueDetails.TableName = "KTTU_SR_ISSUE_DETAILS";
                dtStoneDetails.TableName = "KTTU_SR_ISSUE_STONE_DETAILS";

                string issueTo = string.Empty;
                if (dtMasterDetails != null) {
                    dsSRIssue.Tables.Add(dtMasterDetails);
                    issueTo = dtMasterDetails.Rows[0]["issued_to"].ToString();
                }
                if (dtIssueDetails != null)
                    dsSRIssue.Tables.Add(dtIssueDetails);
                if (dtStoneDetails != null)
                    dsSRIssue.Tables.Add(dtStoneDetails);


                string fpath = string.Format(@"~\App_Data\" + @"Xmls\{0}{1}{2}{3}{4}{5}", "SRISSUEXML_",
                companyCode, branchCode, issueTo, issueNo, ".xml");
                string filePath = System.Web.HttpContext.Current.Request.MapPath(fpath);

                string folderPath = string.Format(@"~\App_Data" + @"\Xmls");
                Globals.CreateDirectoryIfNotExist(System.Web.HttpContext.Current.Request.MapPath(folderPath));

                if (System.IO.File.Exists(filePath)) {
                    File.SetAttributes(filePath, FileAttributes.Normal);
                    FileInfo file = new FileInfo(filePath);
                    file.Delete();
                }
                dsSRIssue.WriteXml(filePath, XmlWriteMode.WriteSchema);
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
            error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };

            try {
                var updatedTimestamp = SIGlobals.Globals.GetDateTime();
                #region Issue Master
                var issueMaster = db.KTTU_SR_ISSUE_MASTER.Where(x => x.company_code == companyCode
                                && x.branch_code == branchCode && x.issue_no == issueNo).FirstOrDefault();
                if (issueMaster == null) {
                    error.description = "Issue information is not found for issue number: " + issueNo.ToString();
                    return false;
                }
                if (issueMaster.cflag == "Y") {
                    error.description = string.Format("The issue number {0} is already cancelled.", issueNo);
                    return false;
                }

                issueMaster.cflag = "Y";
                issueMaster.cancelled_by = userID;
                issueMaster.UpdateOn = updatedTimestamp;
                issueMaster.cancelled_remarks = cancelRemarks;
                #endregion

                #region SR Stock Reverse Posting
                var stockUpdateReqd = SIGlobals.Globals.GetApplicationConfigurationSettnigs(db, companyCode, branchCode, "24032018");
                if (stockUpdateReqd != null && stockUpdateReqd == 1) {
                    var issueLines = db.KTTU_SR_ISSUE_DETAILS.Where(x => x.company_code == companyCode
                                       && x.branch_code == branchCode && x.issue_no == issueNo).ToList();
                    if (issueLines == null) {
                        error.description = "Issue detail is not found for issue number: " + issueNo.ToString();
                        return false;
                    }
                    var srIssueLines = from iLine in issueLines
                                       select new SRIssueLineVM
                                       {
                                           GSCode = iLine.gs_code,
                                           CounterCode = iLine.counter_code,
                                           ItemCode = iLine.item_name,
                                           Qty = iLine.item_no,
                                           GrossWt = iLine.gwt,
                                           StoneWt = iLine.swt,
                                           NetWt = iLine.nwt
                                       };
                    bool stockUpdated = UpdateItemStock(companyCode, branchCode, db, srIssueLines.ToList(), issueNo,
                        postReverse: true, error: out error);
                    if (!stockUpdated) {
                        return false;
                    }

                    bool issuePosted = PostCounterReceipt(companyCode, branchCode, db, srIssueLines.ToList(), userID, issueNo, out error);
                    if (!issuePosted) {
                        return false;
                    }

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

        public dynamic List(string companyCode, string branchCode, DateTime date, out ErrorVM error)
        {
            error = null;
            try {
                var data = (from bim in db.KTTU_SR_ISSUE_MASTER
                            join sm in db.KSTU_SUPPLIER_MASTER
                            on new { CompanyCode = bim.company_code, BranchCode = bim.branch_code, PartyCode = bim.issued_to }
                            equals new { CompanyCode = sm.company_code, BranchCode = sm.branch_code, PartyCode = sm.party_code }
                            where bim.company_code == companyCode
                            && bim.branch_code == branchCode
                            && bim.cflag != "Y"
                            && DbFunctions.TruncateTime(bim.issued_date) == DbFunctions.TruncateTime(date)
                            select new { bim, sm });
                var result = data.Distinct().Select(d => new { IssueNo = d.bim.issue_no, Name = d.sm.party_name }).OrderByDescending(d => d.IssueNo);
                return result;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        #region Print

        public ProdigyPrintVM Print(string companyCode, string branchCode, int issueNo, out ErrorVM error)
        {
            error = null;
            string printData = PrintSRIssue(companyCode, branchCode, issueNo, out error);
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            printObject.PrintType = "HTML";
            printObject.ContinueNextPrint = true;
            printObject.Data = new PrintConfiguration().Base64Encode(printData);
            return printObject;
        }

        protected string PrintSRIssue(string companyCode, string branchCode, int issueNo, out ErrorVM error)
        {
            error = null;
            StringBuilder sb = new StringBuilder();
            try {
                KTTU_SR_ISSUE_MASTER srMaster = db.KTTU_SR_ISSUE_MASTER.Where(sr => sr.company_code == companyCode
                                                                            && sr.branch_code == branchCode
                                                                            && sr.issue_no == issueNo).FirstOrDefault();
                if (srMaster == null) {
                    error = new ErrorVM()
                    {
                        description = "Invalid SR Number",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return "";
                }

                string CompanyAddress = SIGlobals.Globals.GetCompanyDetailsForHTMLPrint(db, companyCode, branchCode);
                string DateTime = string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(srMaster.issued_date));
                string name = SIGlobals.Globals.GetVendor(db, srMaster.issued_to, companyCode, branchCode);
                int IssueNo = issueNo;
                string party_code = srMaster.issued_to;
                KSTU_SUPPLIER_MASTER DtSupplierDetails = SIGlobals.Globals.GetSupplier(db, companyCode, branchCode, party_code);
                KSTS_STATE_MASTER stateMaster = db.KSTS_STATE_MASTER.Where(st => st.state_name == DtSupplierDetails.state).FirstOrDefault();
                KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode && c.branch_code == branchCode).FirstOrDefault();
                string stateCode = Convert.ToString(stateMaster.tinno);

                sb.AppendLine("<html>");
                sb.AppendLine("<head>");
                sb.AppendLine(SIGlobals.Globals.GetStyle());
                sb.AppendLine("</head>");
                sb.AppendLine("<body>");

                sb.AppendLine("<Table  style=\"border-collapse:collapse;\" style=\"border-bottom:0\" frame=\"border\" border=\"1\" width=\"800\">");

                sb.AppendLine("<Table style=\"border-bottom:0\" frame=\"border\" border=\"1\" width=\"800\"  style=\"border-collapse:collapse;\" >");

                sb.AppendLine("<Table frame=\"border\" border=\"0\" width=\"800\" style=\"border-collapse:collapse\" >");
                sb.AppendLine("<Table font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\" border-collapse:collapse; \" width=\"800\">");
                sb.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-right:thin;border-top:thin;\" ALIGN = \"left\"><b>GSTIN {0}</b></TD>", company.tin_no));
                sb.AppendLine(string.Format("<TD style=\" border-bottom:thin;border-top:thin;\" ALIGN = \"right\"><b>PAN {0}</b></TD>", company.pan_no));
                sb.AppendLine("</TR>");

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"CENTER\"><b>{0}</b></TD>", company.company_name));
                sb.AppendLine("</TR>");
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"CENTER\"><b>{0},{1},{2}-{3}</b></TD>", company.address1, company.address2, company.address3, company.pin_code));
                sb.AppendLine("</TR>");


                sb.AppendLine("<TR  style=\"color:black; text-decoration:bold;\" align=\"CENTER\"  style=\"border-left:thin\">");
                sb.AppendLine(string.Format("<TD width=\"400\" ALIGN = \"LEFT\"><b>DETAILS OF CONSIGNEE </b></TD>"));
                sb.AppendLine(string.Format("<TD width=\"400\" ALIGN = \"LEFT\"><b>{0}</b></TD>", "VOUCHER DETAILS"));
                sb.AppendLine("</TR>");


                sb.AppendLine("<tr>");
                sb.AppendLine("<td>");
                sb.AppendLine("<Table>");
                sb.AppendLine("<Table font-size=12pt;  class=\"boldText\"  style=\"border-collapse:collapse;\" >");

                sb.AppendLine("<tr style=\"border-right:0\"  >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left \" >Name &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + DtSupplierDetails.party_name + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" >Address &nbsp&nbsp </td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + DtSupplierDetails.address1 + "</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + DtSupplierDetails.address2 + "</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + DtSupplierDetails.address3 + "</td>");
                sb.AppendLine("</tr>");


                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" >State &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + DtSupplierDetails.state + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" >State Code &nbsp&nbsp&nbsp </td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + stateCode + "</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" >PAN &nbsp&nbsp&nbsp </td>");
                sb.AppendLine(string.Format("<td style=\"border-right:thin \"  style=\"border-top:thin\">{0}</td>", DtSupplierDetails.pan_no.ToString()));
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" >GSTIN &nbsp&nbsp&nbsp </td>");
                sb.AppendLine(string.Format("<td style=\"border-right:thin \"  style=\"border-top:thin\">{0}</td>", DtSupplierDetails.TIN.ToString()));
                sb.AppendLine("</tr>");
                sb.AppendLine("</table>");
                sb.AppendLine("</td>");


                sb.AppendLine("<td>");
                sb.AppendLine("<Table font-size=12pt;   class=\"boldText\"  style=\"border-collapse:collapse;\" >");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >Issue No &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + issueNo + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >Issue Date &nbsp&nbsp</td>");
                sb.AppendLine(string.Format("<td style=\"border-right:thin ; border-top:thin\" >{0}</td>", DateTime));
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" > Place of supply &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + DtSupplierDetails.state + "</td>");// Place of supply is CONSIGNEE State name.
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" >Issue Type &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + "SR Issue" + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >State &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + company.state + "</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >State Code &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + company.state_code + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("</table>");
                sb.AppendLine("</td>");
                sb.AppendLine("</tr>");


                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\" border-bottom:thin;\" colspan = 10 ALIGN = \"CENTER\"><b>DELIVERY CHALLAN</b></TD>"));
                sb.AppendLine("</TR>");

                if (string.Compare(company.state_code.ToString(), stateCode) == 0) {
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"CENTER\"> INTRA-STATE STOCK TRANSFER</TD>"));
                    sb.AppendLine("</TR>");
                }
                else {
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"CENTER\"> INTER-STATE STOCK TRANSFER-TAX INVOICE</TD>"));
                    sb.AppendLine("</TR>");
                }

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD  style=\"border-right:thin;border-bottom:thin\" colspan=10 ALIGN = \"CENTER\">ORIGINAL/DUPLICATE<br></TD>"));
                sb.AppendLine("</TR>");

                sb.AppendLine("</table>");
                sb.AppendLine("</td>");


                sb.AppendLine("</Table>");
                sb.AppendLine("</td>");
                sb.AppendLine("</TR>");

                string strIssueBillDetails = string.Format("EXEC [usp_IssueReceipts_HTMLPrints] '{0}', '{1}', '{2}', '{3}'",
                                    IssueNo, companyCode, branchCode, "SRS");
                DataTable dtIssueBillDetails = SIGlobals.Globals.ExecuteQuery(strIssueBillDetails);

                if (dtIssueBillDetails != null) {
                    sb.AppendLine("<Table style=\"border-collapse:collapse;\" style=border-top:0 ; frame=\"border\" border=\"1\" width=\"800\">");
                    sb.AppendLine("<TR bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                    for (int i = 0; i < dtIssueBillDetails.Columns.Count; i++) {
                        sb.AppendLine(string.Format("<TH style=\" border-bottom: thin solid; \" style=font-size=\"12pt\" width=\"250\" ALIGN = \"CENTER\"><b>{0}</b></TH>", dtIssueBillDetails.Columns[i].ColumnName));
                    }
                    sb.AppendLine("</TR>");

                    for (int i = 0; i < dtIssueBillDetails.Rows.Count - 1; i++) {
                        sb.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");

                        for (int j = 0; j < dtIssueBillDetails.Columns.Count; j++) {
                            if (string.Compare(dtIssueBillDetails.Columns[j].DataType.FullName.ToString(), "System.String") == 0 || j == 0)
                                sb.AppendLine(string.Format("<TD style=\"border-top: thin ; border-bottom: thin; \" style=font-size=\"12pt\" ALIGN = \"CENTER\">{0}{1}</TD>", dtIssueBillDetails.Rows[i][j], "&nbsp"));
                            else {
                                sb.AppendLine(string.Format("<TD style=\"border-top: thin ; border-bottom: thin; \" style=font-size=\"12pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", dtIssueBillDetails.Rows[i][j], "&nbsp"));
                            }
                        }
                        sb.AppendLine("</TR>");
                    }
                }

                sb.AppendLine("<TR bgcolor='#FFFACD'  style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                int columnCOunt = dtIssueBillDetails.Columns.Count;
                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" colspan = 4 ALIGN = \"left\"><b>Total</b></TD>"));

                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>"
               , dtIssueBillDetails.Rows[dtIssueBillDetails.Rows.Count - 1][columnCOunt - 6], "&nbsp", columnCOunt - 5));
                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>"
                 , dtIssueBillDetails.Rows[dtIssueBillDetails.Rows.Count - 1][columnCOunt - 5], "&nbsp", columnCOunt - 4));
                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>"
                     , dtIssueBillDetails.Rows[dtIssueBillDetails.Rows.Count - 1][columnCOunt - 4], "&nbsp", columnCOunt - 3));
                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>"
                         , dtIssueBillDetails.Rows[dtIssueBillDetails.Rows.Count - 1][columnCOunt - 3], "&nbsp", columnCOunt - 2));
                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>"
                         , dtIssueBillDetails.Rows[dtIssueBillDetails.Rows.Count - 1][columnCOunt - 2], "&nbsp", columnCOunt - 1));
                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>"
                         , dtIssueBillDetails.Rows[dtIssueBillDetails.Rows.Count - 1][columnCOunt - 1], "&nbsp"));
                sb.AppendLine("</TR>");

                string GSquery = string.Empty;
                object Amt, Stwt, SAmt, DAmt, Dcarat, va;
                decimal carat = 0;
                decimal StoneAmnt = 0;
                decimal DiamondAmnt = 0;
                decimal Gtotal = 0;
                for (int k = 0; k < 2; k++) {
                    if (k == 0)
                        GSquery = string.Format(" exec [usp_SRStoneDiamondIssueSummary_Prints] '{0}', '{1}', '{2}', '{3}'",
                                    IssueNo, companyCode, branchCode, "S");
                    else
                        GSquery = string.Format(" exec [usp_SRStoneDiamondIssueSummary_Prints] '{0}', '{1}', '{2}', '{3}'",
                                   IssueNo, companyCode, branchCode, "D");
                    DataTable gscodeDetail = SIGlobals.Globals.ExecuteQuery(GSquery);
                    if (gscodeDetail.Rows.Count > 0) {

                        sb.AppendLine("<Table style=\"border-collapse:collapse;\" style=border-top:0 ; frame=\"border\" border=\"0\" width=\"800\">");
                        for (int j = 0; j < 2; j++) {
                            sb.AppendLine("<TR>");
                            sb.AppendLine(string.Format("<TD style=\"border-right:thin;border-left:thin;border-top:thin;border-bottom:thin\" colspan = 10 ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                            sb.AppendLine("</TR>");
                        }
                        sb.AppendLine("</Table>");
                        sb.AppendLine("<Table style=\"border-collapse:collapse;\" style=border-top:0 ; frame=\"border\" border=\"1\" width=\"800\">");
                        if (k == 0) {
                            sb.AppendLine("<TR style=border:0;\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                            sb.AppendLine(string.Format("<TD style=\"border-right:thin;border-left:thin\" style=font-size=\"12pt\" style=border:0 width=\"900\" colspan=7 ALIGN = \"Left\"> SR STONE DETAILS &nbsp&nbsp&nbsp </TD>"));
                            sb.AppendLine("</TR>");
                        }
                        else {
                            sb.AppendLine("<TR style=border:0;\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                            sb.AppendLine(string.Format("<TD style=\"border-right:thin;border-left:thin\" style=font-size=\"12pt\" style=border:0 width=\"900\" colspan=7 ALIGN = \"Left\"> SR DIAMOND DETAILS &nbsp&nbsp&nbsp </TD>"));
                            sb.AppendLine("</TR>");

                        }

                        sb.AppendLine("<TR bgcolor='#FFFACD'  style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                        sb.AppendLine(string.Format("<TH style=\" border-bottom: thin solid; \" style=font-size=\"12pt\"  ALIGN = \"CENTER\"><b>{0}</b></TH>", "Sl.No"));
                        sb.AppendLine(string.Format("<TH style=\" border-bottom: thin solid; \" style=font-size=\"12pt\"  ALIGN = \"CENTER\"><b>{0}</b></TH>", "Bill No"));
                        sb.AppendLine(string.Format("<TH style=\" border-bottom: thin solid; \" style=font-size=\"12pt\"  ALIGN = \"CENTER\"><b>{0}</b></TH>", "Item Name"));
                        sb.AppendLine(string.Format("<TH style=\" border-bottom: thin solid; \" style=font-size=\"12pt\"  ALIGN = \"CENTER\"><b>{0}</b></TH>", "Qty"));
                        sb.AppendLine(string.Format("<TH style=\" border-bottom: thin solid; \" style=font-size=\"12pt\" ALIGN = \"CENTER\"><b>{0}</b></TH>", "Carat"));
                        sb.AppendLine(string.Format("<TH style=\" border-bottom: thin solid; \" style=font-size=\"12pt\"  ALIGN = \"CENTER\"><b>{0}</b></TH>", "Rate/ ct"));
                        sb.AppendLine(string.Format("<TH style=\" border-bottom: thin solid; \" style=font-size=\"12pt\"  ALIGN = \"CENTER\"><b>{0}</b></TH>", "Value"));
                        sb.AppendLine("</TR>");


                        for (int i = 0; i < gscodeDetail.Rows.Count; i++) {
                            sb.AppendLine("<TR>");
                            sb.AppendLine(string.Format("<td style=\" border-bottom: thin; border-top:thin; \" style=font-size=\"12pt\"  ALIGN = \"center\">{0}</TH>", gscodeDetail.Rows[i]["sno"].ToString() + "&nbsp"));
                            sb.AppendLine(string.Format("<td style=\" border-bottom: thin; border-top:thin; \" style=font-size=\"12pt\"  ALIGN = \"right\">{0}</TH>", gscodeDetail.Rows[i]["sales_bill_no"].ToString() + "&nbsp"));
                            sb.AppendLine(string.Format("<td style=\" border-bottom: thin; border-top:thin; \" style=font-size=\"12pt\"  ALIGN = \"CENTER\">{0}</TH>", gscodeDetail.Rows[i]["item_name"].ToString() + "&nbsp"));
                            sb.AppendLine(string.Format("<td style=\" border-bottom: thin; border-top:thin; \" style=font-size=\"12pt\"  ALIGN = \"right\">{0}</TH>", gscodeDetail.Rows[i]["qty"].ToString() + "&nbsp"));
                            sb.AppendLine(string.Format("<td style=\" border-bottom: thin; border-top:thin; \" style=font-size=\"12pt\"  ALIGN = \"right\">{0}</TH>", gscodeDetail.Rows[i]["Carat"].ToString() + "&nbsp"));
                            sb.AppendLine(string.Format("<td style=\" border-bottom: thin; border-top:thin; \" style=font-size=\"12pt\"  ALIGN = \"right\">{0}</TH>", gscodeDetail.Rows[i]["rate"].ToString() + "&nbsp"));
                            sb.AppendLine(string.Format("<td style=\" border-bottom: thin; border-top:thin; \" style=font-size=\"12pt\"  ALIGN = \"right\">{0}</TH>", gscodeDetail.Rows[i]["Amount"].ToString() + "&nbsp"));

                            sb.AppendLine("</TR>");
                        }
                        Dcarat = gscodeDetail.Compute("sum(carat)", "");
                        if (Dcarat != null && Dcarat != DBNull.Value)
                            carat = Convert.ToDecimal(Dcarat);

                        SAmt = gscodeDetail.Compute("sum(qty)", "");
                        if (SAmt != null && SAmt != DBNull.Value)
                            StoneAmnt = Convert.ToDecimal(SAmt);

                        DAmt = gscodeDetail.Compute("sum(amount)", "");
                        if (DAmt != null && DAmt != DBNull.Value)
                            DiamondAmnt = Convert.ToDecimal(DAmt);
                        Gtotal += DiamondAmnt;

                        sb.AppendLine("<TR bgcolor='#FFFACD'  style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                        sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" colspan = 3 ALIGN = \"left\"><b>Total</b></TD>"));
                        sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"right\"><b>{0}</b></TD>", SAmt + "&nbsp"));
                        sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"right\"><b>{0}</b></TD>", carat + "&nbsp"));
                        sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"left\"><b>{0}</b></TD>", ""));
                        sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"right\"><b>{0}</b></TD>", DiamondAmnt.ToString("F") + "&nbsp"));
                        sb.AppendLine("</TR>");
                        sb.AppendLine("</table>");
                        carat = 0; StoneAmnt = 0; DiamondAmnt = 0;

                    }
                }

                sb.AppendLine("</table>");
                sb.AppendLine("<Table style=\"border-collapse:collapse;\" style=border-top:0 ; frame=\"border\" border=\"0\" width=\"800\">");
                sb.AppendLine("<TR>"); sb.AppendLine(string.Format("<td style=\"border-right:thin\" >{0}</td>", "&nbsp"));
                sb.AppendLine("</TR>");
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-right:thin; border-left:thin;border-top:thin;border-bottom:thin\" style=font-size=\"12pt\" ALIGN = \"RIGHT\"><b>For {0}{2}<br><br><br>{1}{2}</b></TD>", company.company_name, "Authorized Signatory", "&nbsp"));
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
        #endregion
    }
}
