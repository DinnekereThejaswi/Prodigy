using ProdigyAPI.BL.BusinessLayer.Stock;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Issues;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Model.MagnaDb;
using ProdigyAPI.SIGlobals;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace ProdigyAPI.BL.BusinessLayer.Issues
{
    public class NonTagIssueBL
    {
        MagnaDbEntities db = null;

        public NonTagIssueBL()
        {
            db = new MagnaDbEntities(true);
        }

        public NonTagIssueBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }

        public dynamic GetIssueToList(string companyCode, string branchCode)
        {
            var partyList = from sm in db.KSTU_SUPPLIER_MASTER
                            join sg in db.KSTU_SUPPLIER_GROUP
                            on new { CC = sm.company_code, BC = sm.branch_code, Party = sm.party_code }
                            equals new { CC = sg.company_code, BC = sg.branch_code, Party = sg.party_code }
                            where sm.company_code == companyCode && sm.branch_code == branchCode
                            && sm.voucher_code == "VB" && sm.party_code != "HO" && sm.obj_status != "C"
                            && sg.ir_code == "IB"
                            select new
                            {
                                Code = sm.party_code,
                                Name = sm.party_code + "-" + sm.party_name
                            };
            return partyList.ToList();
        }

        public dynamic GetItems(string companyCode, string branchCode, string gsCode, string counterCode)
        {
            var itemList = from i in db.KTTU_COUNTER_STOCK
                           where i.company_code == companyCode && i.branch_code == branchCode
                           && i.counter_code == counterCode && i.gs_code == gsCode
                           && (i.op_gwt > 0 || i.op_units > 0
                           || i.barcoded_units > 0 || i.barcoded_gwt > 0 || i.sales_units > 0 || i.sales_gwt > 0
                           || i.issues_units > 0 || i.issues_gwt > 0 || i.receipt_units > 0 || i.receipt_gwt > 0
                           || i.closing_gwt > 0 || i.closing_units > 0)
                           orderby i.item_name
                           select new
                           {
                               ItemName = i.item_name,
                               ItemCode = i.item_name
                           };
            return itemList.ToList();
        }

        public ClosingStockVM GetClosingCounterStock(string companyCode, string branchCode, string gSCode, string counterCode, string itemCode)
        {
            var clsStock = from cs in db.KTTU_COUNTER_STOCK
                           where cs.company_code == companyCode && cs.branch_code == branchCode
                           && cs.gs_code == gSCode && cs.item_name == itemCode && cs.counter_code == counterCode
                           select new ClosingStockVM
                           {
                               Qty = cs.closing_units,
                               GrossWt = cs.closing_gwt,
                               NetWt = cs.closing_nwt
                           };
            return clsStock.FirstOrDefault();
        }

        public bool PostIssue(NonTagIssueVM nonTagIssueVM, string userID, out int issueNo, out ErrorVM error)
        {
            error = null;
            issueNo = 0;
            if (nonTagIssueVM == null) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "There is nothing to post." };
                return false;
            }

            if (nonTagIssueVM.IssueLines == null || nonTagIssueVM.IssueLines.Count <= 0) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Issue line details are not found. At least one line is required to post." };
                return false;
            }

            #region Counter Stock Validation after consolidation. Note that if I don't consolidate, validation cannot be done correctly.
            var netIssueStock = from nti in nonTagIssueVM.IssueLines
                                group new { nti } by new { GS = nti.GSCode, Counter = nti.CounterCode, Item = nti.ItemCode }
                                    into g
                                select new
                                {
                                    GS = g.Key.GS,
                                    Counter = g.Key.Counter,
                                    Item = g.Key.Item,
                                    Qty = g.Sum(m => m.nti.Qty),
                                    GrossWt = g.Sum(m => m.nti.GrossWt),
                                    NetWt = g.Sum(m => m.nti.NetWt)
                                };
            if (netIssueStock != null) {
                foreach (var stk in netIssueStock) {
                    var existingStock = GetClosingCounterStock(nonTagIssueVM.CompanyCode, nonTagIssueVM.BranchCode,
                        stk.GS, stk.Counter, stk.Item);
                    if (existingStock == null) {
                        string errorDesc = string.Format("Counter stock is not found for GS: {0}, Counter: {1} and Item: {2}"
                            , stk.GS, stk.Counter, stk.Item);
                        error = new ErrorVM { description = errorDesc, ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                        return false;
                    }
                    if (stk.Qty > existingStock.Qty || stk.GrossWt > existingStock.GrossWt
                        || stk.NetWt > existingStock.NetWt) {
                        string errorDesc = string.Format("Counter stock is not sufficient for GS: {0}, Counter: {1} and Item: {2}"
                            , stk.GS, stk.Counter, stk.Item);
                        error = new ErrorVM { description = errorDesc, ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                        return false;
                    }
                }
            } 
            #endregion

            var functionResult = SaveIssue(nonTagIssueVM, userID, out issueNo, out error);
            if (!functionResult) {
                return false;
            }

            return true;
        }

        private bool SaveIssue(NonTagIssueVM nonTagIssueVM, string userID, out int issueNo, out ErrorVM error)
        {
            error = null;
            issueNo = 0;
            try {
                string companyCode = nonTagIssueVM.CompanyCode;
                string branchCode = nonTagIssueVM.BranchCode;
                var applicationDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                var updatedTimestamp = SIGlobals.Globals.GetDateTime();

                var storeLocationId = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode
                    && c.branch_code == branchCode).FirstOrDefault().store_location_id;

                #region Issue Header
                issueNo = SIGlobals.Globals.GetDocumentNo(db, companyCode, branchCode, "28", true);
                string issueMasterObjectID = SIGlobals.Globals.GetMagnaGUID("KTTU_BARCODED_ISSUE_MASTER", issueNo, companyCode, branchCode);
                KTTU_BARCODED_ISSUE_MASTER im = new KTTU_BARCODED_ISSUE_MASTER();
                im.obj_id = issueMasterObjectID;
                im.company_code = companyCode;
                im.branch_code = branchCode;
                im.issue_no = issueNo;
                im.issue_type = "IB";
                im.issue_date = applicationDate;
                im.issue_to = nonTagIssueVM.IssueTo;
                im.sal_code = userID;
                im.gs_type = "";
                im.operator_code = userID;
                im.obj_status = "O";
                im.cflag = "N";
                im.cancelled_by = "";
                im.UpdateOn = updatedTimestamp;
                im.is_approved = "N";
                im.approved_by = "";
                im.approver_name = "";
                im.approved_date = updatedTimestamp;
                //im.total_amount = x.total_amount;
                im.tolerance_percent = 0;
                //im.final_amount = x.final_amount;
                im.IsConfirmed = "Y";
                im.remarks = nonTagIssueVM.Remarks;
                im.ShiftID = 0;
                im.New_Bill_No = issueNo;
                im.bmu_charges = nonTagIssueVM.MakingChargesRs;
                im.total_wastage = nonTagIssueVM.WastageGrams;
                im.hallmark_charges = nonTagIssueVM.HallmarkingChargesRs;
                im.Transfer_Type = "";
                im.SGST_Percent = 0;
                im.IGST_Percent = 0;
                im.CGST_Percent = 0;
                im.IGST_Amount = 0;
                im.CGST_Amount = 0;
                im.SGST_Amount = 0;
                im.GSTGroupCode = "";
                im.HSN = "";
                im.total_mc = 0;
                im.TDSPerc = 0;
                im.TDS_Amount = 0;
                im.Net_Amount = 0;
                im.stone_charges = 0;
                im.diamond_charges = 0;
                im.grand_total = 0;
                im.material_type = "";
                im.other_charges = 0;
                im.UniqRowID = Guid.NewGuid();
                im.round_off = 0;
                im.isReservedIssue = "N";
                im.Cancelled_Remarks = "";
                im.store_location_id = storeLocationId;

                //Since there is no GS code in the header, I need to get the GScode of the first line unfortunately.
                if (nonTagIssueVM.IssueLines != null) {
                    if (nonTagIssueVM.IssueLines.Count > 0) {
                        im.gs_type = nonTagIssueVM.IssueLines[0].GSCode;
                    }
                }
                #endregion

                #region Issue Detail
                int slNo = 1;
                decimal totalAmount = 0;
                var finYear = Globals.GetFinancialYear(db, companyCode, branchCode);
                List<NonTagIssueLineVM> issueLines = nonTagIssueVM.IssueLines;
                foreach (var x in nonTagIssueVM.IssueLines) {
                    updatedTimestamp = SIGlobals.Globals.GetDateTime();

                    KTTU_BARCODED_ISSUE_DETAILS id = new KTTU_BARCODED_ISSUE_DETAILS();
                    id.obj_id = issueMasterObjectID;
                    id.company_code = companyCode;
                    id.branch_code = branchCode;
                    id.issue_no = issueNo;
                    id.sno = slNo;
                    id.barcode_no = "";
                    id.item_name = x.ItemCode;
                    id.quantity = Convert.ToInt32(x.Qty);
                    id.gwt = Convert.ToDecimal(x.GrossWt);
                    id.swt = Convert.ToDecimal(x.StoneWt);
                    id.nwt = Convert.ToDecimal(x.NetWt);
                    id.counter_code = x.CounterCode;
                    id.cflag = "N";
                    id.UpdateOn = updatedTimestamp;
                    id.gs_code = x.GSCode;
                    id.amount = x.Rate * x.NetWt;
                    id.rate = x.Rate;
                    id.dcts = x.Dcts;
                    id.batch_id = null;
                    id.supplier_code = null;
                    id.Fin_Year = finYear;
                    id.Batch_no = 0;
                    id.BReceiptNo = "0";
                    id.pur_mc_type = 0;
                    id.pur_mc_rate = 0;
                    id.pur_rate = 0;
                    id.barcode_date = updatedTimestamp;
                    id.BSno = 0;
                    id.design_code = "";
                    id.purity_perc = x.PurityPercent;
                    id.pure_wt = x.PurityPercent * x.GrossWt / 100;
                    id.baud_rate = 0;
                    id.pur_wastage_perc = 0;
                    id.pur_wastage_wt = 0;
                    id.pur_making_charges = 0;
                    id.CGST_Amount = 0;
                    id.CGST_Percent = 0;
                    id.IGST_Amount = 0;
                    id.IGST_Percent = 0;
                    id.SGST_Amount = 0;
                    id.SGST_Percent = 0;
                    id.stone_charges = 0;
                    id.diamond_charges = 0;
                    id.hallmark_charges = 0;
                    id.UniqRowID = Guid.NewGuid();
                    id.ReservedEstNo = 0;
                    id.Reserved_Salcode = null;
                    id.Reserved_VA = 0;
                    id.bin_no = 0;
                    id.tolerance_amount = 0;
                    totalAmount = totalAmount + (x.Rate * x.NetWt);
                    db.KTTU_BARCODED_ISSUE_DETAILS.Add(id);

                    int stoneSerialNo = 1;
                    if (x.StoneDetails != null) {
                        #region Stone Stock Update
                        bool stoneStockUpdate = UpdateStoneStock(companyCode, branchCode, db, x.StoneDetails, issueNo, 
                            postReverse: false, error: out error);
                        if (!stoneStockUpdate) {
                            return false;
                        } 
                        #endregion

                        foreach (var sd in x.StoneDetails) {
                            KTTU_ISSUE_RECEIPTS_STONE_DETAILS stoneDet = new KTTU_ISSUE_RECEIPTS_STONE_DETAILS
                            {
                                obj_id = issueMasterObjectID,
                                company_code = companyCode,
                                branch_code = branchCode,
                                issue_no = issueNo,
                                sno = stoneSerialNo,
                                receipt_no = 0,
                                item_sno = slNo,
                                type = "I",
                                name = sd.Name,
                                qty = sd.Qty,
                                carrat = sd.Carrat,
                                rate = sd.Rate,
                                amount = sd.Amount,
                                gs_code = sd.Type,
                                UpdateOn = updatedTimestamp,
                                Fin_Year = finYear,
                                IR_Type = "IB",
                                party_code = nonTagIssueVM.IssueTo,
                                swt = sd.Weight,
                                stone_damage_carat = 0,
                                stone_damage_Gms = 0,
                                sub_Lot_No = 0,
                                UniqRowID = Guid.NewGuid(),
                                stonetype = null,
                                color = null,
                                cut = null,
                                symmetry = null,
                                fluorescence = null,
                                CertificateNo = null,
                                seiveSize = null,
                                clarity = null,
                                polish = null,
                                shape = null
                            };

                            db.KTTU_ISSUE_RECEIPTS_STONE_DETAILS.Add(stoneDet);
                            stoneSerialNo++;
                        }
                    }
                    slNo++;
                }
                im.total_amount = totalAmount;
                im.final_amount = totalAmount;
                db.KTTU_BARCODED_ISSUE_MASTER.Add(im);
                #endregion

                #region Increment Serial No.
                SIGlobals.Globals.IncrementDocumentNo(db, companyCode, branchCode, "28");
                #endregion

                #region Stock Posting
                bool stockUpdated = UpdateItemStock(companyCode, branchCode, db, issueLines, issueNo, out error);
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

        private bool UpdateItemStock(string companyCode, string branchCode, MagnaDbEntities dbContext, List<NonTagIssueLineVM> issueLines, int issueNo, out ErrorVM error)
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
            bool counterStockPostSuccess = stockPost.CounterStockPost(dbContext, generalStockJournal.ToList(), false, out errorMessage);
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
            bool gsStockPostSuccess = stockPost.GSStockPost(dbContext, summarizedGSStockJournal.ToList(), false, out errorMessage);
            if (!gsStockPostSuccess) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = errorMessage };
                return false;
            }
            #endregion

            return true;
        }

        public bool UpdateStoneStock(string companyCode, string branchCode, MagnaDbEntities dbContext, List<NonTagIssueStoneDetailVM> stoneLines, int issueNo, bool postReverse, out ErrorVM error)
        {
            error = null;
            if(stoneLines == null) {
                return true;
            }
            foreach (var sl in stoneLines) {
                if (postReverse) {
                    sl.Qty = sl.Qty * -1;
                    sl.Carrat = sl.Carrat * -1;
                }
                var stoneStockLine = dbContext.KTTU_STONE_COUNTER_STOCK.Where(x => x.company_code == companyCode
                    && x.branch_code == branchCode && x.gs_code == sl.Type && x.stone_name == sl.Name).FirstOrDefault();
                if(stoneStockLine == null) {
                    error = new ErrorVM { description = "Stone counter stock does not exist for stone " + sl.Name, ErrorStatusCode = System.Net.HttpStatusCode.NotFound };
                    return false;
                    #region Insert stone stock line if it doesn't exist, code is commented becaise it doesn't make sense
                    //KTTU_STONE_COUNTER_STOCK stoneCounterStock = new KTTU_STONE_COUNTER_STOCK();
                    //string[] objIdString = new string[] { "KTTU_STONE_COUNTER_STOCK", sl.Type, sl.Name, "M" };
                    //string objId = SIGlobals.Globals.GetMagnaGUID(objIdString, companyCode, branchCode);
                    //stoneCounterStock.obj_id = objId;
                    //stoneCounterStock.gs_code = sl.Type;
                    //stoneCounterStock.stone_name = sl.Name;
                    //stoneCounterStock.counter_code = "M";
                    //stoneCounterStock.counter_date = SIGlobals.Globals.GetDateTime();
                    //stoneCounterStock.company_code = companyCode;
                    //stoneCounterStock.branch_code = branchCode;
                    //stoneCounterStock.op_qty = 0;
                    //stoneCounterStock.op_carat = 0.000M;
                    //stoneCounterStock.barcoded_qty = 0;
                    //stoneCounterStock.barcoded_carat = 0.000M;
                    //stoneCounterStock.sal_qty = 0;
                    //stoneCounterStock.sal_carat = 0.000M;
                    //stoneCounterStock.receipt_qty = 0;
                    //stoneCounterStock.receipt_carat = 0.000M;
                    //stoneCounterStock.issued_qty = sl.Qty;
                    //stoneCounterStock.issued_carat = sl.Carrat;
                    //stoneCounterStock.closing_qty = sl.Qty;
                    //stoneCounterStock.closing_carat = sl.Carrat; 
                    #endregion
                }
                if(sl.Qty > stoneStockLine.closing_qty || sl.Carrat > stoneStockLine.closing_carat) {
                    error = new ErrorVM { description = "There is not enough stone counter stock for stone " + sl.Name, ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return false;
                }
               
                stoneStockLine.issued_qty = stoneStockLine.issued_qty + sl.Qty;
                stoneStockLine.issued_carat = stoneStockLine.issued_carat + sl.Carrat;
                stoneStockLine.closing_qty = stoneStockLine.closing_qty - sl.Qty;
                stoneStockLine.closing_carat = stoneStockLine.closing_carat - sl.Carrat;
                db.Entry(stoneStockLine).State = System.Data.Entity.EntityState.Modified;
            }

            return true;
        }

        public bool GenerateXMLFile(string companyCode, string branchCode, int issueNo, out string errorMessage)
        {
            errorMessage = string.Empty;
            try {
                DataSet dsSRIssue = new DataSet();
                DataTable dtIssueMaster = Globals.GetDataTable(string.Format("SELECT * FROM KTTU_BARCODED_ISSUE_MASTER AS im WHERE im.company_code = '{0}' AND im.branch_code = '{1}' AND im.issue_no = '{2}'",
                    companyCode, branchCode, issueNo));
                DataTable dtIssueDetails = Globals.GetDataTable(string.Format("SELECT * FROM KTTU_BARCODED_ISSUE_DETAILS AS id WHERE id.company_code = '{0}' AND id.branch_code = '{1}' AND id.issue_no = '{2}'",
                    companyCode, branchCode, issueNo));

               
                dtIssueMaster.TableName = "KTTU_BARCODED_ISSUE_MASTER";
                dtIssueDetails.TableName = "KTTU_BARCODED_ISSUE_DETAILS";

                if (dtIssueMaster == null || dtIssueMaster.Rows.Count <= 0
                    || dtIssueDetails == null || dtIssueDetails.Rows.Count <= 0) {
                    errorMessage = "Issue Details/barcodes are not found.";
                    return false;
                }

                string issueTo = string.Empty;
                if (dtIssueMaster != null) {
                    dsSRIssue.Tables.Add(dtIssueMaster);
                    issueTo = dtIssueMaster.Rows[0]["issue_to"].ToString();
                }
                if (dtIssueDetails != null)
                    dsSRIssue.Tables.Add(dtIssueDetails);

                string fpath = string.Format(@"~\App_Data" + @"\Xmls\{3}\{0}{1}{2}{3}{4}{5}", "NonTagIssueXML_",
                    companyCode, branchCode, issueTo, issueNo, ".xml");
                string filePath = System.Web.HttpContext.Current.Request.MapPath(fpath);

                string folderPath = string.Format(@"~\App_Data" + @"\Xmls\{0}", issueTo);
                Globals.CreateDirectoryIfNotExist(System.Web.HttpContext.Current.Request.MapPath(folderPath));

                if (System.IO.File.Exists(filePath)) {
                    File.SetAttributes(filePath, FileAttributes.Normal);
                    FileInfo file = new FileInfo(filePath);
                    file.Delete();
                }
                dsSRIssue.WriteXml(filePath, XmlWriteMode.IgnoreSchema);
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
