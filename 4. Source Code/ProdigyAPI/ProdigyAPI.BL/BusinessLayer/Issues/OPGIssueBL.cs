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
    public class OPGIssueBL
    {
        MagnaDbEntities db = null;

        public OPGIssueBL()
        {
            db = new MagnaDbEntities(true);
        }

        public OPGIssueBL(MagnaDbEntities _dbContext)
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

        public OPGIssueVM GetOPGDetails(OPGIssueQueryVM opgIssueQuery, out ErrorVM error)
        {
            error = null;
            if (opgIssueQuery == null) {
                error = new ErrorVM { description = "Nothing to query.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return null;
            }
            OPGIssueVM oPGIssue = new OPGIssueVM();
            var opgItems = db.usp_GetOPGItemsForIssue(opgIssueQuery.CompanyCode, opgIssueQuery.BranchCode, opgIssueQuery.GSCode,
                opgIssueQuery.FromDate, opgIssueQuery.ToDate).ToList();
            if (opgItems == null) {
                error = new ErrorVM { description = "No OPG item details found.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return null;
            }

            var opgStones = db.usp_GetOPGStoneItemsForIssue(opgIssueQuery.CompanyCode, opgIssueQuery.BranchCode, opgIssueQuery.GSCode,
                opgIssueQuery.FromDate, opgIssueQuery.ToDate).ToList();
            oPGIssue.CompanyCode = opgIssueQuery.CompanyCode;
            oPGIssue.BranchCode = opgIssueQuery.BranchCode;
            oPGIssue.IssueTo = opgIssueQuery.IssueTo;
            oPGIssue.GSCode = opgIssueQuery.GSCode;
            oPGIssue.IssueTo = opgIssueQuery.IssueTo;

            oPGIssue.IssueLines = new List<OPGIssueLineVM>();
            decimal totalDcts = 0;
            foreach (var x in opgItems) {
                OPGIssueLineVM LineItem = new OPGIssueLineVM
                {
                    BillNo = x.bill_no,
                    SlNo = x.sl_no,
                    GSCode = x.gs_code,
                    CounterCode = "",
                    ItemCode = x.item_name,
                    Qty = Convert.ToInt32(x.item_no),
                    GrossWt = Convert.ToDecimal(x.gwt),
                    NetWt = Convert.ToDecimal(x.nwt),
                    StoneWt = x.swt,
                    AmountBeforeTax = x.item_amount,
                    AmountAfterTax = x.item_amount,
                    OtherLineAttributes = new OtherOPGLineAttributes
                    {
                        CategoryType = x.category_type,
                        ItemType = x.item_type,
                        PurityPercent = Convert.ToDecimal(x.purity_per),
                        PurchaseRate = x.purchase_rate,
                        MeltingLoss = x.melting_loss,
                        MeltingPercent = x.melting_percent,
                        DiamondAmount = x.diamond_amount,
                        ItemAmount = x.item_amount
                    },
                    StoneDetails = new List<OPGIssueStoneDetailVM>()
                };

                if (opgStones != null) {
                    var itemStones = opgStones.Where(m => m.bill_no == x.bill_no && m.item_sno == x.sl_no).ToList();
                    if (itemStones != null) {
                        foreach (var its in itemStones) {
                            OPGIssueStoneDetailVM _sd = new OPGIssueStoneDetailVM
                            {
                                SlNo = its.sno,
                                GS = its.gs_code,
                                Name = its.Name,
                                Rate = its.rate,
                                Qty = its.qty,
                                Carrat = its.carrat,
                                Amount = its.amount
                            };
                            if (its.gs_code.Contains("D"))
                                totalDcts = totalDcts + its.carrat;
                            LineItem.StoneDetails.Add(_sd);
                        }
                    }
                }
                oPGIssue.IssueLines.Add(LineItem);
            }

            oPGIssue.Qty = oPGIssue.IssueLines.Sum(p => p.Qty);
            oPGIssue.GrossWt = oPGIssue.IssueLines.Sum(p => p.GrossWt);
            oPGIssue.StoneWt = oPGIssue.IssueLines.Sum(p => p.StoneWt);
            oPGIssue.NetWt = oPGIssue.IssueLines.Sum(p => p.NetWt);
            oPGIssue.Dcts = totalDcts;
            oPGIssue.MeltingLossWt = Convert.ToDecimal(oPGIssue.IssueLines.Sum(p => p.OtherLineAttributes.MeltingLoss));
            oPGIssue.AmountBeforeTax = oPGIssue.IssueLines.Sum(p => p.AmountBeforeTax);
            oPGIssue.AmountAfterTax = oPGIssue.IssueLines.Sum(p => p.AmountAfterTax);

            return oPGIssue;
        }

        public bool PostIssue(OPGIssueQueryVM opgQuery, string userID, out int issueNo, out ErrorVM error)
        {
            error = null;
            issueNo = 0;
            var opgDetail = GetOPGDetails(opgQuery, out error);
            if (error != null) {
                return false;
            }
            if (opgDetail.IssueLines == null || opgDetail.IssueLines.Count <= 0) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Issue line details are not found. At least one line is required to post." };
                return false;
            }

            var functionResult = SaveIssue(opgDetail, userID, out issueNo, out error);
            if (!functionResult) {
                return false;
            }

            return true;
        }

        private bool SaveIssue(OPGIssueVM issueDetail, string userID, out int issueNo, out ErrorVM error)
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
                string issueMasterObjectID = SIGlobals.Globals.GetMagnaGUID("KTTU_OPG_STOCK_ISSUE_MASTER", issueNo, companyCode, branchCode);
                KTTU_OPG_STOCK_ISSUE_MASTER im = new KTTU_OPG_STOCK_ISSUE_MASTER
                {
                    obj_id = issueMasterObjectID,
                    company_code = companyCode,
                    branch_code = branchCode,
                    issue_no = issueNo,
                    issued_to = issueDetail.IssueTo,
                    issued_by = userID,
                    issued_date = applicationDate,
                    operator_code = userID,
                    gs_code = issueDetail.GSCode,
                    issue_type = "IB",
                    cflag = "N",
                    cancelled_by = "",
                    UpdateOn = updatedTimestamp,
                    cancelled_remarks = "",
                    New_Bill_No = issueNo,
                    ShiftID = 0,
                    UniqRowID = Guid.NewGuid(),
                    store_location_id = storeLocationId,
                    SGST_Percent = 0,
                    IGST_Percent = 0,
                    CGST_Percent = 0,
                    IGST_Amount = issueDetail.IGSTAmount,
                    CGST_Amount = 0,
                    SGST_Amount = 0,
                    total_qty = issueDetail.Qty,
                    total_gwt = issueDetail.GrossWt,
                    total_swt = issueDetail.StoneWt,
                    total_nwt = issueDetail.NetWt,
                    total_amount = issueDetail.AmountBeforeTax
                };

                #endregion

                #region Issue Detail
                int slNo = 1;
                var finYear = Globals.GetFinancialYear(db, companyCode, branchCode);
                List<OPGIssueLineVM> issueLines = issueDetail.IssueLines;
                foreach (var x in issueDetail.IssueLines) {
                    updatedTimestamp = SIGlobals.Globals.GetDateTime();

                    KTTU_OPG_STOCK_ISSUE_DETAILS id = new KTTU_OPG_STOCK_ISSUE_DETAILS
                    {
                        obj_id = issueMasterObjectID,
                        company_code = companyCode,
                        branch_code = branchCode,
                        issue_no = issueNo,
                        sl_no = slNo,
                        bill_no = x.BillNo,
                        gs_code = x.GSCode,
                        item_name = x.ItemCode,
                        item_no = Convert.ToInt32(x.Qty),
                        gwt = Convert.ToDecimal(x.GrossWt),
                        swt = Convert.ToDecimal(x.StoneWt),
                        nwt = Convert.ToDecimal(x.NetWt),
                        melting_percent = Convert.ToDecimal(x.OtherLineAttributes.MeltingPercent),
                        melting_loss = Convert.ToDecimal(x.OtherLineAttributes.MeltingLoss),
                        diamond_amount = Convert.ToDecimal(x.OtherLineAttributes.DiamondAmount),
                        item_amount = x.AmountBeforeTax,
                        category_type = x.OtherLineAttributes.CategoryType,
                        item_type = x.OtherLineAttributes.ItemType,
                        purity_per = Convert.ToDecimal(x.OtherLineAttributes.PurityPercent),
                        Fin_Year = finYear,
                        gold_amount = x.NetWt * Convert.ToDecimal(x.OtherLineAttributes.PurchaseRate),
                        purchase_rate = Convert.ToDecimal(x.OtherLineAttributes.PurchaseRate),
                        UniqRowID = Guid.NewGuid()
                    };

                    db.KTTU_OPG_STOCK_ISSUE_DETAILS.Add(id);

                    int stoneSerialNo = 1;
                    if (x.StoneDetails != null) {
                        foreach (var sd in x.StoneDetails) {
                            KTTU_OPG_STOCK_ISSUE_STONE_DETAILS stoneDet = new KTTU_OPG_STOCK_ISSUE_STONE_DETAILS
                            {
                                obj_id = issueMasterObjectID,
                                company_code = companyCode,
                                branch_code = branchCode,
                                issue_no = issueNo,
                                bill_no = x.BillNo,
                                item_slno = slNo,
                                sl_no = stoneSerialNo,
                                gs_code = sd.GS,
                                item_name = sd.Name,
                                qty = Convert.ToInt32(sd.Qty),
                                carrat = Convert.ToDecimal(sd.Carrat),
                                rate = Convert.ToDecimal(sd.Rate),
                                amount = Convert.ToDecimal(sd.Amount),
                                Fin_Year = finYear,
                                UniqRowID = Guid.NewGuid()
                            };

                            db.KTTU_OPG_STOCK_ISSUE_STONE_DETAILS.Add(stoneDet);
                            stoneSerialNo++;
                        }
                    }
                    slNo++;
                }

                db.KTTU_OPG_STOCK_ISSUE_MASTER.Add(im);
                #endregion

                #region Increment Serial No.
                SIGlobals.Globals.IncrementDocumentNo(db, companyCode, branchCode, "28");
                #endregion

                #region GS Stock Posting for Issue
                var stockUpdateReqd = SIGlobals.Globals.GetApplicationConfigurationSettnigs(db, companyCode, branchCode, "24032018");
                if (stockUpdateReqd != null && stockUpdateReqd == 1) {
                    bool stockUpdated = UpdateOPGGSStock(companyCode, branchCode, db, issueLines, issueNo, postReverse: false, error: out error);
                    if (!stockUpdated) {
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

        private bool UpdateOPGGSStock(string companyCode, string branchCode, MagnaDbEntities dbContext, List<OPGIssueLineVM> issueLines, int issueNo, bool postReverse, out ErrorVM error)
        {
            error = null;
            #region Post GS Stock
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

        public bool GenerateOPGXMLFile(string companyCode, string branchCode, int issueNo, out string errorMessage)
        {
            errorMessage = string.Empty;
            try {
                DataSet dsOPGIssueDetails = new DataSet();
                DataTable dtMasterDetails = Globals.GetDataTable(string.Format("SELECT * FROM KTTU_OPG_STOCK_ISSUE_MASTER AS im WHERE im.company_code = '{0}' AND im.branch_code = '{1}' AND im.issue_no = '{2}'",
                    companyCode, branchCode, issueNo));
                DataTable dtIssueDetails = Globals.GetDataTable(string.Format("SELECT * FROM KTTU_OPG_STOCK_ISSUE_DETAILS AS id WHERE id.company_code = '{0}' AND id.branch_code = '{1}' AND id.issue_no = '{2}'",
                    companyCode, branchCode, issueNo));
                DataTable dtStoneDetails = Globals.GetDataTable(string.Format("SELECT * FROM KTTU_OPG_STOCK_ISSUE_STONE_DETAILS AS id WHERE id.company_code = '{0}' AND id.branch_code = '{1}' AND id.issue_no = '{2}'",
                    companyCode, branchCode, issueNo));
                dtMasterDetails.TableName = "KTTU_OPG_STOCK_ISSUE_MASTER";
                dtIssueDetails.TableName = "KTTU_OPG_STOCK_ISSUE_DETAILS";
                dtStoneDetails.TableName = "KTTU_OPG_STOCK_ISSUE_STONE_DETAILS";

                string issueTo = string.Empty;
                if (dtMasterDetails != null) {
                    dsOPGIssueDetails.Tables.Add(dtMasterDetails);
                    issueTo = dtMasterDetails.Rows[0]["issued_to"].ToString();
                }
                if (dtIssueDetails != null)
                    dsOPGIssueDetails.Tables.Add(dtIssueDetails);
                if (dtStoneDetails != null)
                    dsOPGIssueDetails.Tables.Add(dtStoneDetails);


                string fpath = string.Format(@"~\App_Data\" + @"Xmls\{0}{1}{2}{3}{4}{5}", "OPGIssueXML_",
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
            error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };

            try {
                var updatedTimestamp = SIGlobals.Globals.GetDateTime();
                #region Issue Master
                var issueMaster = db.KTTU_OPG_STOCK_ISSUE_MASTER.Where(x => x.company_code == companyCode
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

                #region GS Stock Reverse Posting
                var stockUpdateReqd = SIGlobals.Globals.GetApplicationConfigurationSettnigs(db, companyCode, branchCode, "24032018");
                if (stockUpdateReqd != null && stockUpdateReqd == 1) {
                    var issueLines = db.KTTU_OPG_STOCK_ISSUE_DETAILS.Where(x => x.company_code == companyCode
                                       && x.branch_code == branchCode && x.issue_no == issueNo).ToList();
                    if (issueLines == null) {
                        error.description = "Issue detail is not found for issue number: " + issueNo.ToString();
                        return false;
                    }
                    var opgIssueLines = from iLine in issueLines
                                        select new OPGIssueLineVM
                                        {
                                            GSCode = iLine.gs_code,
                                            CounterCode = "",
                                            ItemCode = iLine.item_name,
                                            Qty = iLine.item_no,
                                            GrossWt = iLine.gwt,
                                            StoneWt = iLine.swt,
                                            NetWt = iLine.nwt
                                        };
                    bool stockUpdated = UpdateOPGGSStock(companyCode, branchCode, db, opgIssueLines.ToList(), issueNo,
                        postReverse: true, error: out error);
                    if (!stockUpdated) {
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
                var data = (from bim in db.KTTU_OPG_STOCK_ISSUE_MASTER
                            join sm in db.KSTU_SUPPLIER_MASTER
                            on new { CompanyCode = bim.company_code, BranchCode = bim.branch_code, PartyCode = bim.issued_to }
                            equals new { CompanyCode = sm.company_code, BranchCode = sm.branch_code, PartyCode = sm.party_code }
                            where bim.company_code == companyCode && bim.branch_code == branchCode && bim.cflag != "Y" && DbFunctions.TruncateTime(bim.issued_date) == DbFunctions.TruncateTime(date)
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
            string printData = PrintOPGIssue(companyCode, branchCode, issueNo, true, out error);
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            printObject.PrintType = "HTML";
            printObject.ContinueNextPrint = true;
            printObject.Data = new PrintConfiguration().Base64Encode(printData);
            return printObject;
        }

        protected string PrintOPGIssue(string companyCode, string branchCode, int issueNo, bool isIssue, out ErrorVM error)
        {
            error = null;
            StringBuilder sb = new StringBuilder();
            KTTU_OPG_STOCK_ISSUE_MASTER opgStock = null;
            try {
                KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode && c.branch_code == branchCode).FirstOrDefault();
                string opgstockmaster = string.Empty;
                if (isIssue) {
                    //opgstockmaster = string.Format("select OD.gs_code,OM.issued_by,OD.item_name,(select karat from Item_master where gs_code =OD.gs_code and item_code = OD.item_name and company_code ='{1}' and branch_Code = '{2}') as 'Purity',OD.item_no as 'Qty',OD.bill_no,OD.gwt as 'Gross Wt',OD.issue_no,OD.swt as 'Stone Wt',OD.nwt AS 'Net Wt',OD.melting_percent,OD.purchase_rate,OD.gold_amount as 'Value',OM.issued_date as 'Issue date', OM.issued_to,OM.branch_code,ISNULL(OD.purity_per,0) as 'Purity_per', om.issue_type as issue_type\n"
                    //    + "from KTTU_OPG_STOCK_ISSUE_DETAILS OD,KTTU_OPG_STOCK_ISSUE_MASTER OM where OD.issue_no =OM.issue_no and OD.issue_no={0} and OD.company_code ='{1}'and OD.branch_Code = '{2}'and OM.company_code ='{1}'and OM.branch_Code = '{2}' ORDER BY OD.bill_no,OD.item_name", issueNo, companyCode, branchCode);

                    opgStock = db.KTTU_OPG_STOCK_ISSUE_MASTER.Where(st => st.company_code == companyCode
                                                                                                && st.branch_code == branchCode
                                                                                                && st.issue_no == issueNo).FirstOrDefault();
                    if (opgStock == null) {
                        error = new ErrorVM()
                        {
                            description = "Invalid OPG Issue Number",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return "";
                    }
                }
                else {
                    //opgstockmaster = string.Format("select OD.gs_code,OM.received_by,OD.item_name,(select karat from Item_master where gs_code =OD.gs_code and item_code = OD.item_name and company_code ='{1}' and branch_Code = '{2}') as 'Purity',OD.bill_no,OD.item_no as 'Qty',OD.gwt as 'Gross Wt',OD.receipt_no,OD.swt as 'Stone Wt',OD.nwt as 'Net Wt',OD.melting_percent,OD.purchase_rate,OD.gold_amount as 'Value',OM.receipt_Date as  'Receipt date',OM.receipt_from,OM.branch_code,ISNULL(OD.purity_per,0) as 'Purity_per' \n"
                    //    + "from KTTU_OPG_STOCK_RECEIPT_DETAILS OD,KTTU_OPG_STOCK_RECEIPT_MASTER OM where OD.receipt_no =OM.receipt_no AND OD.receipt_no={0} and OD.company_code ='{1}'and OD.branch_Code = '{2}'and OM.company_code ='{1}'and OM.branch_Code = '{2}' ORDER BY OD.bill_no,OD.item_name", issueNo, companyCode, branchCode);
                    error = new ErrorVM()
                    {
                        description = "Invalid OPG Receipt Number",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return "";
                }

                string CompanyAddress = SIGlobals.Globals.GetCompanyDetailsForHTMLPrint(db, companyCode, branchCode);
                string name = string.Empty;
                string DateTime = string.Empty;
                string party_code = string.Empty;
                if (isIssue) {
                    name = SIGlobals.Globals.GetVendor(db, opgStock.issued_to, companyCode, branchCode);
                    DateTime = string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(opgStock.issued_date));
                    party_code = (opgStock.issued_to);
                }
                else {
                    //name = SIGlobals.Globals.GetVendor(db, dtopgstock.Rows[0]["receipt_from"].ToString(), companyCode, branchCode);
                    //DateTime = string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(dtopgstock.Rows[0]["Receipt date"]));
                    //party_code = dtopgstock.Rows[0]["receipt_from"].ToString();
                }

                sb.AppendLine("<html>");
                sb.AppendLine("<head>");
                sb.AppendLine(SIGlobals.Globals.GetStyle());
                sb.AppendLine("</head>");
                sb.AppendLine("<body>");

                //string comp = string.Format("select cm.company_name,tin_no,pin_code,cst_no,cm.pan_no,cm.state,cm.address1,cm.address2,cm.address3,cm.state_code,cm.city,cm.Header1,cm.phone_no,cm.Header2,cm.Header3,cm.Header4,cm.Header5,cm.Header6,cm.Header7, \n"
                //             + "cm.Footer1,cm.Footer2, email_id,website from KSTU_COMPANY_MASTER cm WHERE cm.company_code = '{0}' and cm.branch_code = '{1}'", companyCode, branchCode);
                //DataTable dtCompanyDetails = SIGlobals.Globals.ExecuteQuery(comp);

                KSTU_SUPPLIER_MASTER DtSupplierDetails = SIGlobals.Globals.GetSupplier(db, companyCode, branchCode, party_code);
                KSTS_STATE_MASTER state = db.KSTS_STATE_MASTER.Where(st => st.state_name == DtSupplierDetails.state).FirstOrDefault();

                sb.AppendLine("<Table style=\"border-bottom:0\" frame=\"border\" border=\"1\" width=\"800\"  style=\"border-collapse:collapse;\" >");
                if (isIssue) {
                    sb.AppendLine("<Table frame=\"border\" border=\"0\" width=\"800\" style=\"border-collapse:collapse\" >");
                    sb.AppendLine("<Table font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\" border-collapse:collapse; \" width=\"800\">");
                    sb.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD style=\" border-bottom:thin;border-right:thin\"  ALIGN = \"left\"><b>GSTIN &nbsp&nbsp&nbsp{0}</b></TD>", company.tin_no));
                    sb.AppendLine(string.Format("<TD style=\" border-bottom:thin;\"  ALIGN = \"right\"><b> {0}</b></TD>", "ORIGINAL"));
                    sb.AppendLine("</TR>");
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"LEFT\"><b>PAN &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp{0}</b></TD>", company.pan_no));
                    sb.AppendLine("</TR>");
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"CENTER\"><b>{0}</b></TD>", company.company_name));
                    sb.AppendLine("</TR>");
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"CENTER\"><b>{0},{1},{2}-{3}</b></TD>", company.address1, company.address2, company.address3, company.pin_code));
                    sb.AppendLine("</TR>");
                    sb.AppendLine("<TR  style=\"color:black; text-decoration:bold;\" align=\"CENTER\"  style=\"border-left:thin\">");
                    sb.AppendLine(string.Format("<TD width=\"400\" ALIGN = \"LEFT\"><b>DETAILS OF RECIPIENT </b></TD>"));
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
                    sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" >Area &nbsp</td>");
                    sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + DtSupplierDetails.address3.ToString() + "</td>");
                    sb.AppendLine("</tr>");
                    sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" >City &nbsp</td>");
                    sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + DtSupplierDetails.city.ToString() + "</td>");
                    sb.AppendLine("</tr>");
                    sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" >State &nbsp&nbsp</td>");

                    if (!string.IsNullOrEmpty(DtSupplierDetails.pincode)) {
                        sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + DtSupplierDetails.state + ',' + DtSupplierDetails.pincode.ToString() + "</td>");
                    }
                    else {
                        sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + DtSupplierDetails.state.ToString() + "</td>");
                    }
                    sb.AppendLine("</tr>");

                    sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    if (!string.IsNullOrEmpty(DtSupplierDetails.mobile) && !string.IsNullOrEmpty(DtSupplierDetails.phone)) {
                        sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" >Mobile/Phn.No </td>");
                        sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + "&nbsp" + DtSupplierDetails.mobile + ',' + DtSupplierDetails.phone + "</td>");
                    }
                    else if (!string.IsNullOrEmpty(DtSupplierDetails.mobile.ToString())) {
                        sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" >Mobile/Phn.No </td>");
                        sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + "&nbsp" + DtSupplierDetails.mobile + "</td>");
                    }
                    else if (!string.IsNullOrEmpty(DtSupplierDetails.phone)) {
                        sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" >Mobile/Phn.No </td>");
                        sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + "&nbsp" + DtSupplierDetails.phone + "</td>");
                    }
                    sb.AppendLine("</tr>");
                    sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" >StateCode </td>");
                    sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + DtSupplierDetails.state_code + "</td>");
                    sb.AppendLine("</tr>");
                    sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sb.AppendLine("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>PAN &nbsp&nbsp</b></td>");
                    sb.AppendLine(string.Format("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>{0}</b></td>", DtSupplierDetails.pan_no));
                    sb.AppendLine("</tr>");
                    sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sb.AppendLine("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>GSTIN &nbsp&nbsp</b></td>");
                    sb.AppendLine(string.Format("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>{0}</b></td>", DtSupplierDetails.TIN));
                    sb.AppendLine("</tr>");
                    sb.AppendLine("</table>");
                    sb.AppendLine("</td>");
                    sb.AppendLine("<td>");
                    sb.AppendLine("<Table font-size=12pt;   class=\"boldText\"  style=\"border-collapse:collapse;\" >");
                    sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >Invoice No &nbsp&nbsp</td>");
                    sb.AppendLine(string.Format("<td style=\"border-right:thin ; border-top:thin\" >{0}</td>", issueNo));
                    sb.AppendLine("</tr>");
                    sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >Ref No &nbsp&nbsp</td>");
                    sb.AppendLine(string.Format("<td style=\"border-right:thin ; border-top:thin\" >{0}</td>", issueNo));
                    sb.AppendLine("</tr>");
                    sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" > Date Of Issue &nbsp&nbsp</td>");
                    sb.AppendLine(string.Format("<td style=\"border-right:thin ; border-top:thin\" >{0}</td>", DateTime));
                    sb.AppendLine("</tr>");
                    sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" > Place of supply &nbsp&nbsp</td>");
                    sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + DtSupplierDetails.state + "</td>");
                    sb.AppendLine("</tr>");
                    sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" >Issue Type &nbsp&nbsp</td>");
                    sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + "OPG Issue" + "</td>");
                    sb.AppendLine("</tr>");
                    sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >State &nbsp&nbsp</td>");
                    sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + company.state + "</td>");
                    sb.AppendLine("</tr>");
                    sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >State Code &nbsp&nbsp</td>");
                    sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + company.state_code + "</td>");
                    sb.AppendLine("</tr>");
                    sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sb.AppendLine("<td style=\"border-right:thin; border-top:thin\"colspan=5 align=\"left\" ><b>Whether tax is payable on reverse charge basis? - No &nbsp&nbsp</b></td>");
                    sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + "&nbsp &nbsp" + "</td>");
                    sb.AppendLine("</tr>");
                    sb.AppendLine("</tr>");
                    sb.AppendLine("</table>");
                    sb.AppendLine("</td>");
                    sb.AppendLine("</tr>");
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD style=\" border-bottom:thin;\" colspan = 10 ALIGN = \"CENTER\"><b>TAX INVOICE </b></TD>"));//DELIVERY CHALLAN
                    sb.AppendLine("</TR>");
                    if (string.Compare(company.state_code.ToString(), state.state_name) == 0) {
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
                    sb.AppendLine(string.Format("<TD  style=\"border-bottom:Bold\" colspan=10 ALIGN = \"CENTER\"><b></b><br></TD>"));
                    sb.AppendLine("</TR>");
                    sb.AppendLine("</table>");
                    sb.AppendLine("</td>");
                }
                else {
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
                    sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"CENTER\"><b>{0},{1},{2} {3}-{4}</b></TD>", company.address1, company.address2, company.address3, company.city, company.pin_code));
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
                    sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + DtSupplierDetails.address1.ToString() + "</td>");
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
                    sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + state.state_name + "</td>");
                    sb.AppendLine("</tr>");
                    sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" >PAN &nbsp&nbsp&nbsp </td>");
                    sb.AppendLine(string.Format("<td style=\"border-right:thin \"  style=\"border-top:thin\">{0}</td>", DtSupplierDetails.pan_no));
                    sb.AppendLine("</tr>");
                    sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" >GSTIN &nbsp&nbsp&nbsp </td>");
                    sb.AppendLine(string.Format("<td style=\"border-right:thin \"  style=\"border-top:thin\">{0}</td>", DtSupplierDetails.TIN));
                    sb.AppendLine("</tr>");
                    sb.AppendLine("</table>");
                    sb.AppendLine("</td>");
                    sb.AppendLine("<td>");
                    sb.AppendLine("<Table font-size=12pt;   class=\"boldText\"  style=\"border-collapse:collapse;\" >");
                    sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >Receipt No &nbsp&nbsp</td>");
                    sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + issueNo + "</td>");
                    sb.AppendLine("</tr>");
                    sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >Receipt Date &nbsp&nbsp</td>");
                    sb.AppendLine(string.Format("<td style=\"border-right:thin ; border-top:thin\" >{0}</td>", DateTime));
                    sb.AppendLine("</tr>");
                    sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" > Place of supply &nbsp&nbsp</td>");
                    sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + company.state + "</td>");
                    sb.AppendLine("</tr>");
                    sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" >Receipt Type &nbsp&nbsp</td>");
                    sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + "OPG Receipt" + "</td>");
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
                    if (string.Compare(company.state_code.ToString(), state.state_name) == 0) {
                        sb.AppendLine("<TR>");
                        sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"CENTER\"> INTRA-STATE STOCK RECEIPT</TD>"));
                        sb.AppendLine("</TR>");
                    }
                    else {
                        sb.AppendLine("<TR>");
                        sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"CENTER\"> INTER-STATE STOCK RECEIPT</TD>"));
                        sb.AppendLine("</TR>");
                    }
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD  style=\"border-right:thin;border-bottom:thin\" colspan=10 ALIGN = \"CENTER\">ORIGINAL/DUPLICATE<br></TD>"));
                    sb.AppendLine("</TR>");
                    sb.AppendLine("</table>");
                    sb.AppendLine("</td>");
                }
                sb.AppendLine("</Table>");
                sb.AppendLine("</td>");
                sb.AppendLine("</TR>");

                string strIssueBillDetails = string.Empty;
                if (isIssue) {
                    strIssueBillDetails = string.Format("EXEC [usp_IssueReceipts_HTMLPrints] '{0}', '{1}', '{2}', '{3}'",
                                        issueNo, companyCode, branchCode, "OG");
                }
                else {
                    strIssueBillDetails = string.Format("EXEC [usp_IssueReceipts_HTMLPrints] '{0}', '{1}', '{2}', '{3}'",
                                        issueNo, companyCode, branchCode, "OGR");
                }
                DataTable dtIssueBillDetails = SIGlobals.Globals.ExecuteQuery(strIssueBillDetails);


                if (dtIssueBillDetails != null) {
                    sb.AppendLine("<Table font-size=12pt;   style=\"border-collapse:collapse;\" style=border-top:0 ; frame=\"border\" border=\"1\" width=\"800\" >");
                    sb.AppendLine("<TR bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                    for (int i = 0; i < dtIssueBillDetails.Columns.Count; i++) {
                        sb.AppendLine(string.Format("<TH style=\"border-bottom:thin solid;border-top:thin;\" style=font-size=\"12pt\" width=\"250\" ALIGN = \"CENTER\"><b>{0}</b></TH>", dtIssueBillDetails.Columns[i].ColumnName));
                    }
                    sb.AppendLine("</TR>");

                    for (int i = 0; i < dtIssueBillDetails.Rows.Count - 1; i++) {
                        sb.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");

                        for (int j = 0; j < dtIssueBillDetails.Columns.Count; j++) {
                            if (string.Compare(dtIssueBillDetails.Columns[j].DataType.FullName.ToString(), "System.String") == 0 || j == 0)
                                sb.AppendLine(string.Format("<TD style=\"border-top:thin;border-bottom:thin\" style=font-size=\"12pt\" ALIGN = \"CENTER\">{0}{1}</TD>", dtIssueBillDetails.Rows[i][j], "&nbsp"));
                            else {
                                sb.AppendLine(string.Format("<TD style=\"border-top:thin;border-bottom:thin\"  style=font-size=\"12pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", dtIssueBillDetails.Rows[i][j], "&nbsp"));
                            }
                        }
                        sb.AppendLine("</TR>");
                    }
                    string columnSpace = string.Empty;
                    for (int i = 0; i < 10 - dtIssueBillDetails.Rows.Count - 1; i++) {
                        if (i == 0) {
                            columnSpace = string.Format("<TD style=\"border-top: thin ; border-bottom: thin; \">{0}</TD>", "&nbsp");
                            sb.AppendLine("<TR>");
                            for (int s = 0; s < dtIssueBillDetails.Columns.Count; s++)
                                sb.AppendLine(string.Format("{0}", columnSpace));
                            sb.AppendLine("</TR>");
                        }
                        else {
                            columnSpace = string.Format("<TD style=\"border-top: thin ; border-bottom: thin; \">{0}</TD>", "&nbsp");
                            sb.AppendLine("<TR>");
                            for (int s = 0; s < dtIssueBillDetails.Columns.Count; s++)
                                sb.AppendLine(string.Format("{0}", columnSpace));
                            sb.AppendLine("</TR>");
                        }
                    }
                }

                sb.AppendLine("<TR bgcolor='#FFFACD'  style=\"border-top: thin ;text-decoration:bold;\" align=\"CENTER\">");
                int columnCOunt = dtIssueBillDetails.Columns.Count;
                if (isIssue)
                    sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" colspan = 4 ALIGN = \"left\"><b>Total</b></TD>"));
                else
                    sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" colspan =2 ALIGN = \"left\"><b>Total</b></TD>"));

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

                string OPGMaster = string.Empty;
                DataTable dtOPGMAster = null;
                decimal IGSTAmount = 0, IGSTPercent = 0, TotalAmount = 0;
                KTTU_OPG_STOCK_ISSUE_MASTER opgStockIssue = new KTTU_OPG_STOCK_ISSUE_MASTER();
                if (isIssue) {
                    //OPGMaster = string.Format("select isnull(Total_amount,0) as Total_amount,isnull(IGST_Percent,0) as IGST_Percent,isnull(IGST_amount,0) as IGST_amount from KTTU_OPG_STOCK_ISSUE_MASTER where issue_no='{0}' and company_code='{1}' and branch_code='{2}'", issueNo, companyCode, branchCode);
                    //dtOPGMAster = SIGlobals.Globals.ExecuteQuery(OPGMaster);

                    opgStockIssue = db.KTTU_OPG_STOCK_ISSUE_MASTER.Where(opg => opg.company_code == companyCode
                                                                        && opg.branch_code == branchCode
                                                                        && opg.issue_no == issueNo).FirstOrDefault();

                    IGSTAmount = Convert.ToInt32(opgStockIssue.IGST_Amount);
                    IGSTPercent = Convert.ToInt32(opgStockIssue.IGST_Percent);
                    TotalAmount = Convert.ToInt32(opgStockIssue.total_amount);
                }
                else {
                    //OPGMaster = string.Format("select isnull(Total_amount,0) as Total_amount,isnull(IGST_Percent,0) as IGST_Percent,isnull(IGST_amount,0) as IGST_amount from KTTU_OPG_STOCK_RECEIPT_MASTER where receipt_no='{0}' and company_code='{1}' and branch_code='{2}'", issueNo, companyCode, branchCode);
                    //dtOPGMAster = SIGlobals.Globals.ExecuteQuery(OPGMaster);

                    //IGSTAmount = Convert.ToDecimal(dtOPGMAster.Rows[0]["IGST_amount"].ToString());
                    //IGSTPercent = Convert.ToDecimal(dtOPGMAster.Rows[0]["IGST_Percent"].ToString());
                    //TotalAmount = Convert.ToDecimal(dtOPGMAster.Rows[0]["Total_amount"].ToString());
                }

                if (Convert.ToDecimal(IGSTAmount) > 0) {
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD  style=font-size=\"12pt\" colspan=9 ALIGN = \"right\"><b>IGST Amt @ {0} % : </b></TD>", IGSTPercent));
                    sb.AppendLine(string.Format("<TD  style=font-size=\"12pt\" style=\"border-right:thin\" ALIGN = \"right\"><b>{0}</b></TD>", IGSTAmount));
                    sb.AppendLine("</TR>");
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD  style=font-size=\"12pt\" colspan=9 ALIGN = \"right\"><b>Invoice Value : </b></TD>"));
                    sb.AppendLine(string.Format("<TD  style=font-size=\"12pt\" style=\"border-right:thin\"  ALIGN = \"right\"><b>{0}</b></TD>", TotalAmount));
                    sb.AppendLine("</TR>");
                }
                decimal Inword = 0.0M;
                string strWords = string.Empty;
                Inword = Convert.ToDecimal(TotalAmount);
                strWords = SIGlobals.Globals.ConvertNumbertoWordsinRupees(Math.Round(Inword, 0, MidpointRounding.ToEven));

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" style=\"border-right:thin ; border-bottom:0 \"; colspan=10 ALIGN = \"left\"><b>{0}{1}{1}</b></TD>", strWords, "&nbsp", dtIssueBillDetails.Columns.Count));
                sb.AppendLine("</TR>");

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" style=\" border-bottom:thin;\" colspan = 10 ALIGN = \"LEFT\"><b>Original for consignee/Duplicate for transporter/Triplicate for consigner</b></TD>"));//DELIVERY CHALLAN
                sb.AppendLine("</TR>");
                sb.AppendLine("<tr>");
                sb.AppendLine(string.Format("<td style=font-size=\"12pt\" style=\"border-right:thin\"  colspan=5  align = \"left\"  ><b>{0}&nbsp<br><br><br>{1}&nbsp</b></td>", "&nbsp", "Signature of Consignee"));
                sb.AppendLine(string.Format("<td  colspan=5  align = \"right\"  ><b>For {0}&nbsp<br><br><br>{1}&nbsp</b></td>", company.company_name, "Authorized Signatory"));
                sb.AppendLine("</tr>");
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
