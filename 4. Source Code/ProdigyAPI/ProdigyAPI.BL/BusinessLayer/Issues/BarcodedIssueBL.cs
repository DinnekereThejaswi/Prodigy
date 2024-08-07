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
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Issues
{
    public class BarcodedIssueBL
    {
        MagnaDbEntities db = null;

        public BarcodedIssueBL()
        {
            db = new MagnaDbEntities(true);
        }

        public BarcodedIssueBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }

        public dynamic GetIssueToList(string companyCode, string branchCode)
        {
            var partyList = db.KSTU_SUPPLIER_MASTER
                .Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.voucher_code == "VB" && x.obj_status != "C")
                .OrderBy(y => y.party_name)
                .Select(z => new { Code = z.party_code, Name = z.party_name }).ToList();
            return partyList;
        }

        public bool GetBarcodeInfoforIssue(string companyCode, string branchCode, string issueTo, string gsCode, string barcodeNo, out BarcodeIssueLineVM barcodeIssueLine, out ErrorVM error)
        {
            barcodeIssueLine = null;
            error = null;

            var barcodeDetail = db.KTTU_BARCODE_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                && x.barcode_no == barcodeNo && x.sold_flag != "Y").FirstOrDefault();
            if (barcodeDetail == null) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = string.Format("Barcode No. {0} is not found.", barcodeNo) };
                return false;
            }
            if (barcodeDetail.gs_code != gsCode) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Different GS cannot be clubbed." };
                return false;
            }

            var barcodeStones = db.KTTU_BARCODE_STONE_DETAILS.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                && x.barcode_no == barcodeNo).ToList();
            string errorMsg = string.Empty;
            decimal diamondCarets = 0.00M;
            decimal rate = 0.00M;
            decimal itemValue = GetBarcodeValue(barcodeDetail, barcodeStones, out diamondCarets, out rate, out errorMsg);
            if (!string.IsNullOrEmpty(errorMsg)) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = errorMsg };
                return false;
            }

            List<BarcodeIssueStoneDetailVM> bsdList = new List<BarcodeIssueStoneDetailVM>();
            foreach (var item in barcodeStones) {
                var bsd = new BarcodeIssueStoneDetailVM
                {
                    BarcodeNo = item.barcode_no,
                    SlNo = item.sl_no,
                    Type = item.type,
                    Name = item.name,
                    Qty = item.qty,
                    Rate = item.rate,
                    Carrat = item.carrat,
                    Amount = item.amount
                };
                bsdList.Add(bsd);
            }

            barcodeIssueLine = new BarcodeIssueLineVM
            {
                ItemCode = barcodeDetail.item_name,
                CounterCode = barcodeDetail.counter_code,
                GSCode = barcodeDetail.gs_code,
                BarcodeNo = barcodeNo,
                Qty = Convert.ToInt32(barcodeDetail.qty),
                GrossWt = Convert.ToDecimal(barcodeDetail.gwt),
                NetWt = Convert.ToDecimal(barcodeDetail.nwt),
                StoneWt = Convert.ToDecimal(barcodeDetail.swt),
                TagWeight = Convert.ToDecimal(barcodeDetail.tag_wt),
                Dcts = diamondCarets,
                Rate = rate,
                AmountBeforeTax = itemValue,
                CGSTAmount = 0,
                SGSTAmount = 0,
                IGSTAmount = 0,
                AmountAfterTax = itemValue,
                StoneDetails = bsdList
            };

            return true;
        }

        public bool PostIssue(BarcodeIssueVM barcodeissueVM, string userID, out int issueNo, out ErrorVM error)
        {
            error = null;
            issueNo = 0;
            if (barcodeissueVM == null) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "There is nothing to post." };
                return false;
            }

            if (barcodeissueVM.IssueLines == null || barcodeissueVM.IssueLines.Count <= 0) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Issue line details are not found. At least one line is required to post." };
                return false;
            }

            var functionResult = SaveIssue(barcodeissueVM, userID, out issueNo, out error);
            if (!functionResult) {
                return false;
            }

            return true;
        }

        private bool SaveIssue(BarcodeIssueVM barcodeissueVM, string userID, out int issueNo, out ErrorVM error)
        {
            error = null;
            issueNo = 0;
            try {
                #region Issue Header
                string companyCode = barcodeissueVM.CompanyCode;
                string branchCode = barcodeissueVM.BranchCode;
                var applicationDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                var updatedTimestamp = SIGlobals.Globals.GetDateTime();

                var storeLocationId = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode
                    && c.branch_code == branchCode).FirstOrDefault().store_location_id;

                issueNo = SIGlobals.Globals.GetDocumentNo(db, companyCode, branchCode, "28", true);
                string receiptMasterObjID = SIGlobals.Globals.GetMagnaGUID("KTTU_BARCODED_ISSUE_MASTER", issueNo, companyCode, branchCode);
                KTTU_BARCODED_ISSUE_MASTER im = new KTTU_BARCODED_ISSUE_MASTER();
                im.obj_id = receiptMasterObjID;
                im.company_code = companyCode;
                im.branch_code = branchCode;
                im.issue_no = issueNo;
                im.issue_type = "IB";
                im.issue_date = applicationDate;
                im.issue_to = barcodeissueVM.IssueTo;
                im.sal_code = userID;
                im.gs_type = barcodeissueVM.GSCode;
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
                im.tolerance_percent = 10; //TODO: need to get this from db
                //im.final_amount = x.final_amount;
                im.IsConfirmed = "Y";
                im.remarks = barcodeissueVM.Remarks;
                im.ShiftID = 0;
                im.New_Bill_No = issueNo;
                im.bmu_charges = 0;
                im.total_wastage = 0;
                im.hallmark_charges = 0;
                im.Transfer_Type = "S";
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
                #endregion

                #region Issue Detail
                int slNo = 1;
                decimal totalAmount = 0;
                var finYear = Globals.GetFinancialYear(db, companyCode, branchCode);
                List<BarcodeIssueLineVM> _barcodeIssueLines = new List<BarcodeIssueLineVM>();
                foreach (var x in barcodeissueVM.IssueLines) {
                    updatedTimestamp = SIGlobals.Globals.GetDateTime();
                    var barcodeDetail = db.KTTU_BARCODE_MASTER.Where(bm => bm.company_code == companyCode && bm.branch_code == branchCode
                        && bm.barcode_no == x.BarcodeNo && bm.sold_flag != "Y").FirstOrDefault();
                    if (barcodeDetail == null) {
                        error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = string.Format("Barcode No. {0} is not found.", x.BarcodeNo) };
                        return false;
                    }

                    //Since the data sent by the UI cannot be believed to be true, we need get the 
                    //data again and that data to be used for posting.                    
                    BarcodeIssueLineVM bil;
                    error = null;
                    var functionResult = new BarcodedIssueBL().GetBarcodeInfoforIssue(companyCode, branchCode, barcodeissueVM.IssueTo, barcodeissueVM.GSCode, x.BarcodeNo,
                        out bil, out error);
                    if (functionResult != true) {
                        return false;
                    }
                    _barcodeIssueLines.Add(bil);

                    KTTU_BARCODED_ISSUE_DETAILS id = new KTTU_BARCODED_ISSUE_DETAILS();
                    id.obj_id = receiptMasterObjID;
                    id.company_code = companyCode;
                    id.branch_code = branchCode;
                    id.issue_no = issueNo;
                    id.sno = slNo;
                    id.barcode_no = bil.BarcodeNo;
                    id.item_name = barcodeDetail.item_name;
                    id.quantity = Convert.ToInt32(barcodeDetail.qty);
                    id.gwt = Convert.ToDecimal(barcodeDetail.gwt);
                    id.swt = Convert.ToDecimal(barcodeDetail.swt);
                    id.nwt = Convert.ToDecimal(barcodeDetail.nwt);
                    id.counter_code = barcodeDetail.counter_code;
                    id.cflag = "N";
                    id.UpdateOn = updatedTimestamp;
                    id.gs_code = barcodeDetail.gs_code;
                    id.amount = bil.AmountBeforeTax;
                    id.rate = bil.Rate;
                    id.dcts = bil.Dcts;
                    id.batch_id = null;
                    id.supplier_code = null;
                    id.Fin_Year = finYear;
                    id.Batch_no = 0;
                    id.BReceiptNo = barcodeDetail.BReceiptNo;
                    id.pur_mc_type = barcodeDetail.pur_mc_type;
                    id.pur_mc_rate = barcodeDetail.pur_mc_gram;
                    id.pur_rate = barcodeDetail.pur_rate;
                    id.barcode_date = barcodeDetail.date;
                    id.BSno = 0;
                    id.design_code = "";
                    id.purity_perc = barcodeDetail.pur_purity_percentage;
                    id.pure_wt = 0;
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
                    slNo++;
                    totalAmount = totalAmount + bil.AmountBeforeTax;
                    db.KTTU_BARCODED_ISSUE_DETAILS.Add(id);

                    #region Mark Barcodes
                    barcodeDetail.sold_flag = "Y";
                    barcodeDetail.ExitDocType = "BI";
                    barcodeDetail.ExitDate = applicationDate;
                    barcodeDetail.ExitDocNo = issueNo.ToString();
                    db.Entry(barcodeDetail).State = System.Data.Entity.EntityState.Modified;
                    #endregion
                }
                im.total_amount = totalAmount;
                im.final_amount = totalAmount;
                db.KTTU_BARCODED_ISSUE_MASTER.Add(im);
                #endregion

                #region Increment Serial No.
                SIGlobals.Globals.IncrementDocumentNo(db, companyCode, branchCode, "28");
                #endregion

                #region Stock Posting
                bool stockUpdated = UpdateStock(companyCode, branchCode, db, _barcodeIssueLines, issueNo, false, out error);
                if (!stockUpdated) {
                    return false;
                }
                #endregion

                #region Delete from Stock Taking Table if barcode is available
                //The very drawback of entity framework is here, unless I select the entity, I cannot update/delete. So this hard work I've to do.
                //A very simple query like this would have solved it in ADO.NET.
                //DELETE FROM KTTU_STOCK_TABKING 
                //WHERE barcode_no IN (SELECT barcode_no FROM KTTU_BARCODED_ISSUE_DETAILS WHERE issue_no = 1234)
                foreach (var issLine in barcodeissueVM.IssueLines) {
                    var stockTkg = db.KTTU_STOCK_TAKING.Where(st => st.company_code == companyCode
                        && st.branch_code == branchCode && st.barcode_no == issLine.BarcodeNo).FirstOrDefault();
                    if (stockTkg != null)
                        db.Entry(stockTkg).State = System.Data.Entity.EntityState.Deleted;
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

        private bool UpdateStock(string companyCode, string branchCode, MagnaDbEntities dbContext, List<BarcodeIssueLineVM> issueLines, int issueNo, bool postReverse, out ErrorVM error)
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
                           StockTransType = StockTransactionType.BarcodeIssue,
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
                        StockTransType = StockTransactionType.BarcodeIssue,
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

        private decimal GetBarcodeValue(KTTU_BARCODE_MASTER barcodeDetail, List<KTTU_BARCODE_STONE_DETAILS> barcodeStones, out decimal dcts, out decimal rate, out string errorMessage)
        {
            dcts = 0.00M;
            errorMessage = string.Empty;
            rate = 0;
            decimal barcodeValue = 0;
            string companyCode = barcodeDetail.company_code;
            string branchCode = barcodeDetail.branch_code;
            string mcType, gscode, pieceRateItem, karat, grade;
            mcType = gscode = pieceRateItem = karat = string.Empty;
            try {
                gscode = barcodeDetail.gs_code;
                decimal pieceRate = Convert.ToDecimal(barcodeDetail.piece_rate);
                if (pieceRate > 0) {
                    decimal piRate = GetMaximumTolerance(companyCode, branchCode, 26092019);
                    pieceRateItem = "P";
                    pieceRate = (pieceRate * (piRate / 100));
                    barcodeValue = pieceRate;
                    return Math.Round(barcodeValue, 2, MidpointRounding.ToEven);
                }

                decimal toalStoneAmount = 0, totalDiamondAmount = 0, totalDiamondCarat = 0;
                grade = Convert.ToString(barcodeDetail.grade);
                karat = Convert.ToString(barcodeDetail.karat);

                if (barcodeStones != null) {
                    toalStoneAmount = Convert.ToDecimal(barcodeStones.Where(x => x.type.StartsWith("S")).Sum(y => y.amount));
                    totalDiamondAmount = Convert.ToDecimal(barcodeStones.Where(x => x.type.StartsWith("D")).Sum(y => y.amount));
                    totalDiamondCarat = dcts = Convert.ToDecimal(barcodeStones.Where(x => x.type.StartsWith("D")).Sum(y => y.carrat));
                }

                var rateTable = db.KSTU_RATE_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode && x.gs_code == gscode
                    && (x.karat == karat || x.karat == "NA" || x.karat == "")).FirstOrDefault();
                if (rateTable == null) {
                    errorMessage = string.Format("Failed to calculate barcode value because rate is not found for GS: {0}, karat: {1}",
                        gscode, karat);
                    return 0;
                }
                rate = Convert.ToDecimal(rateTable.rate);
                decimal nwt = Convert.ToDecimal(barcodeDetail.nwt);
                decimal valueAmount = 0;
                decimal itemAmount = 0;
                decimal goldAmount = nwt * rate;
                decimal va_Per = 0;
                decimal diamondCaratValue = 0;
                decimal stonePercentage = 0;
                if (string.Compare(gscode, "SL") == 0) {
                    va_Per = GetMaximumTolerance(companyCode, branchCode, 27092019);
                    if (va_Per > 0) {
                        valueAmount = Convert.ToDecimal(nwt * va_Per);
                    }
                }
                else {
                    va_Per = GetMaximumTolerance(companyCode, branchCode, 25092019);
                    if (va_Per > 0) {
                        valueAmount = Convert.ToDecimal(goldAmount * (va_Per / 100));
                    }
                }
                diamondCaratValue = GetMaximumTolerance(companyCode, branchCode, 3102019);
                totalDiamondAmount = Convert.ToDecimal(totalDiamondCarat * diamondCaratValue);
                stonePercentage = GetMaximumTolerance(companyCode, branchCode, 4102019);
                toalStoneAmount = Convert.ToDecimal(toalStoneAmount * (stonePercentage / 100));

                decimal hallMarkCharges = Convert.ToDecimal(barcodeDetail.hallmark_charges);

                int setting = 0;
                APP_CONFIG_TABLE config = db.APP_CONFIG_TABLE.Where(c => c.obj_id == "30" && c.company_code == companyCode && c.branch_code == branchCode).FirstOrDefault();
                if (config != null)
                    setting = Convert.ToInt32(config.value);

                if (setting == 0) {
                    itemAmount = goldAmount + valueAmount + toalStoneAmount + totalDiamondAmount + hallMarkCharges;
                }
                else {
                    itemAmount = goldAmount + toalStoneAmount + totalDiamondAmount;
                }
                barcodeValue = Math.Round(itemAmount, 2, MidpointRounding.ToEven);

                return barcodeValue;
            }
            catch (Exception ex) {
                var error = new ErrorVM().GetErrorDetails(ex);
                errorMessage = error.description;
                return 0;
            }
        }

        public decimal GetMaximumTolerance(string companyCode, string branchCode, int id)
        {
            var toler = db.KSTU_TOLERANCE_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                && x.obj_id == id).FirstOrDefault();
            if (toler == null)
                return 0;
            else
                return Convert.ToDecimal(toler.Max_Val);
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

                string barcodeSql = string.Format("SELECT bm.* \n"
                           + "FROM   KTTU_BARCODED_ISSUE_DETAILS  AS id \n"
                           + "       JOIN KTTU_BARCODE_MASTER     AS bm \n"
                           + "            ON  bm.company_code = id.company_code \n"
                           + "            AND bm.branch_code = id.branch_code \n"
                           + "            AND bm.barcode_no = id.barcode_no \n"
                           + "WHERE  bm.sold_flag = 'Y' \n"
                           + "       AND id.company_code = '{0}' \n"
                           + "       AND id.branch_code = '{1}' \n"
                           + "       AND id.issue_no = '{2}' \n"
                           + "       AND bm.barcode_no != ''",
                           companyCode, branchCode, issueNo);
                DataTable dtBarcodes = Globals.GetDataTable(barcodeSql);

                string barcodeStoneSql = string.Format("SELECT bm.*  \n"
                           + "FROM   KTTU_BARCODED_ISSUE_DETAILS      AS id \n"
                           + "       JOIN KTTU_BARCODE_STONE_DETAILS  AS bm \n"
                           + "            ON  bm.company_code = id.company_code \n"
                           + "            AND bm.branch_code = id.branch_code \n"
                           + "            AND bm.barcode_no = id.barcode_no \n"
                           + "WHERE  id.company_code = '{0}' \n"
                           + "       AND id.branch_code = '{1}' \n"
                           + "       AND id.issue_no = '{2}' \n"
                           + "       AND bm.barcode_no != ''",
                           companyCode, branchCode, issueNo);
                DataTable dtBarcodeStones = Globals.GetDataTable(barcodeStoneSql);
                dtIssueMaster.TableName = "KTTU_BARCODED_ISSUE_MASTER";
                dtIssueDetails.TableName = "KTTU_BARCODED_ISSUE_DETAILS";
                dtBarcodes.TableName = "KTTU_BARCODE_MASTER";
                dtBarcodeStones.TableName = "KTTU_BARCODE_STONE_DETAILS";

                if (dtIssueMaster == null || dtIssueMaster.Rows.Count <= 0
                    || dtIssueDetails == null || dtIssueDetails.Rows.Count <= 0
                    || dtBarcodes == null || dtBarcodes.Rows.Count <= 0) {
                    errorMessage = "Issue Details/barcodes are not found.";
                    return false;
                }
                if (dtIssueDetails.Rows.Count != dtBarcodes.Rows.Count) {
                    errorMessage = string.Format("There are {0} issue lines, but barcodes lines are {1}. Unless there is match, xml file cannot be generated.",
                        dtIssueDetails.Rows.Count, dtBarcodes.Rows.Count);
                    return false;
                }

                string issueTo = string.Empty;
                if (dtIssueMaster != null) {
                    dsSRIssue.Tables.Add(dtIssueMaster);
                    issueTo = dtIssueMaster.Rows[0]["issue_to"].ToString();
                }
                if (dtIssueDetails != null)
                    dsSRIssue.Tables.Add(dtIssueDetails);
                if (dtBarcodes != null)
                    dsSRIssue.Tables.Add(dtBarcodes);
                if (dtBarcodeStones != null)
                    dsSRIssue.Tables.Add(dtBarcodeStones);


                string fpath = string.Format(@"~\App_Data" + @"\Xmls\{3}\{0}{1}{2}{3}{4}{5}", "BarcodedIssueXML_",
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

        public bool CancelIssue(string companyCode, string branchCode, int issueNo, string userID, string cancelRemarks, out ErrorVM error)
        {
            error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };

            try {
                var updatedTimestamp = SIGlobals.Globals.GetDateTime();
                #region Issue Master
                var issueMaster = db.KTTU_BARCODED_ISSUE_MASTER.Where(x => x.company_code == companyCode
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
                issueMaster.Cancelled_Remarks = cancelRemarks;
                #endregion

                #region Issue Detail
                var issueLines = db.KTTU_BARCODED_ISSUE_DETAILS.Where(x => x.company_code == companyCode
                                        && x.branch_code == branchCode && x.issue_no == issueNo).ToList();
                if (issueLines == null) {
                    error.description = "Issue details is not found for issue number: " + issueNo.ToString();
                    return false;
                }
                updatedTimestamp = SIGlobals.Globals.GetDateTime();
                foreach (var il in issueLines) {
                    il.cflag = "Y";
                    il.UpdateOn = updatedTimestamp;
                    db.Entry(il).State = System.Data.Entity.EntityState.Modified;
                    if (!string.IsNullOrEmpty(il.barcode_no)) {
                        var barcodeMaster = db.KTTU_BARCODE_MASTER.Where(bar => bar.company_code == il.company_code
                            && bar.branch_code == il.branch_code && bar.barcode_no == il.barcode_no).FirstOrDefault();
                        if (barcodeMaster.sold_flag != "Y") {
                            error.description = string.Format("Barcode No. {0} is not marked as issued.", barcodeMaster.barcode_no);
                            return false;
                        }
                        barcodeMaster.sold_flag = "N";
                        barcodeMaster.is_shuffled = "N";
                        barcodeMaster.UpdateOn = updatedTimestamp;
                        db.Entry(barcodeMaster).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                #endregion

                #region Item Stock Reversal
                //To Reuse existing code, I need to fill the ViewModel and call the same stock update method.
                var barcodeIssLines = from iLine in issueLines
                                      select new BarcodeIssueLineVM
                                      {
                                          BarcodeNo = iLine.barcode_no,
                                          GSCode = iLine.gs_code,
                                          CounterCode = iLine.counter_code,
                                          ItemCode = iLine.item_name,
                                          Qty = iLine.quantity,
                                          GrossWt = iLine.gwt,
                                          StoneWt = iLine.swt,
                                          NetWt = iLine.nwt
                                      };
                bool stockUpdated = UpdateStock(companyCode, branchCode, db, barcodeIssLines.ToList(), issueNo, true, out error);
                if (!stockUpdated) {
                    return false;
                }
                #endregion

                #region Stone Stock Reversal
                //UpdateStoneStock
                var stStockDetail = db.KTTU_ISSUE_RECEIPTS_STONE_DETAILS.Where(ss => ss.company_code == companyCode
                    && ss.branch_code == branchCode && ss.issue_no == issueNo && ss.type == "I").ToList();
                if (stStockDetail != null) {
                    //Because swt is nullable, I've to first select the data and then get the list.
                    var stoneStockVM = stStockDetail
                        .Select(st => new NonTagIssueStoneDetailVM
                        {
                            Type = st.gs_code,
                            Name = st.name,
                            Weight = Convert.ToDecimal(st.swt),
                            Qty = st.qty,
                            Carrat = st.carrat,
                            Amount = st.amount,
                            Rate = st.rate
                        });
                    //Note the use of postReverse, it is true in this context since we have to reverse the stock posting.
                    var stockStockPostingSucceeded = new NonTagIssueBL(db).UpdateStoneStock(companyCode, branchCode,
                        db, stoneStockVM.ToList(), issueNo, postReverse: true, error: out error);
                    if (!stockStockPostingSucceeded) {
                        return false;
                    }
                }
                #endregion

                #region Reverse Account posting
                //TODO: To be implemented later
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
                //Bellow Linq query can get result of both tagged and non-tagged also
                // But we have used to take both.
                var data = (from bim in db.KTTU_BARCODED_ISSUE_MASTER
                            join bid in db.KTTU_BARCODED_ISSUE_DETAILS
                            on new { CompanyCode = bim.company_code, BranchCode = bim.branch_code, IssueNo = bim.issue_no }
                            equals new { CompanyCode = bid.company_code, BranchCode = bid.branch_code, IssueNo = bid.issue_no }
                            join sm in db.KSTU_SUPPLIER_MASTER
                            on new { CompanyCode = bim.company_code, BranchCode = bim.branch_code, PartyCode = bim.issue_to }
                            equals new { CompanyCode = sm.company_code, BranchCode = sm.branch_code, PartyCode = sm.party_code }
                            where bim.company_code == companyCode && bim.branch_code == branchCode && bim.cflag != "Y" && DbFunctions.TruncateTime(bim.issue_date) == DbFunctions.TruncateTime(date)
                            select new { bim, bid, sm });
                var result = data.Distinct().Select(d => new { IssueNo = d.bid.issue_no, Name = d.sm.party_name }).OrderByDescending(d => d.IssueNo);
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
            string printData = PrintIssue(companyCode, branchCode, issueNo, out error);
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            printObject.PrintType = "HTML";
            printObject.ContinueNextPrint = true;
            printObject.Data = new PrintConfiguration().Base64Encode(printData);
            return printObject;
        }

        protected string PrintIssue(string companyCode, string branchCode, int issueNo, out ErrorVM error)
        {
            error = null;
            string partyCode = string.Empty;
            string supplierDetails = string.Empty;
            DateTime issueDate = DateTime.Now;

            StringBuilder sb = new StringBuilder();
            try {
                KTTU_BARCODED_ISSUE_MASTER master = db.KTTU_BARCODED_ISSUE_MASTER.Where(b => b.company_code == companyCode
                                                                                        && b.branch_code == branchCode
                                                                                        && b.issue_no == issueNo).FirstOrDefault();
                if (master == null) {
                    error = new ErrorVM()
                    {
                        description = "Invalid Issue Number",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return "";
                }

                KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode && c.branch_code == branchCode).FirstOrDefault();
                partyCode = master.issue_to;
                issueDate = master.issue_date;

                KSTU_SUPPLIER_MASTER supplier = SIGlobals.Globals.GetSupplier(db, companyCode, branchCode, partyCode);
                KSTS_STATE_MASTER state = db.KSTS_STATE_MASTER.Where(st => st.state_name == supplier.state).FirstOrDefault();

                if(supplier == null) {
                    error = new ErrorVM { description = $"Details of Vendor/Branch/Supplier {partyCode} is not found.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return string.Empty;
                }

                if (state == null) {
                    error = new ErrorVM { description = $"State details for state {supplier.state} is not found.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return string.Empty;
                }

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
                sb.AppendLine("<Table font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\" border-collapse:collapse; \" width=\"800\">");
                sb.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\" border-bottom:thin;border-right:thin\"  ALIGN = \"left\"><b>GSTIN {0}</b></TD>", company.tin_no));
                sb.AppendLine(string.Format("<TD style=\" border-bottom:thin;\"  ALIGN = \"right\"><b>PAN {0}</b></TD>", company.pan_no.ToString()));
                sb.AppendLine("</TR>");

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"CENTER\"><b>{0}</b></TD>", company.company_name));
                sb.AppendLine("</TR>");
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"CENTER\"><b>{0},{1},{2} {3}-{4}</b></TD>",
                    company.address1, company.address2, company.address3, company.city, company.pin_code));
                sb.AppendLine("</TR>");


                sb.AppendLine("<TR  style=\"color:black; text-decoration:bold;\" align=\"CENTER\"  style=\"border-left:thin\">");
                sb.AppendLine(string.Format("<TD width=\"400\" ALIGN = \"LEFT\"><b> DETAILS OF RECIPIENT </b></TD>"));
                sb.AppendLine(string.Format("<TD width=\"400\" ALIGN = \"LEFT\"><b>{0}</b></TD>", "ISSUE DETAILS"));
                sb.AppendLine("</TR>");


                sb.AppendLine("<tr>");
                sb.AppendLine("<td>");
                sb.AppendLine("<Table>");
                sb.AppendLine("<Table font-size=12pt;  class=\"boldText\"  style=\"border-collapse:collapse;\" >");

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
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" >City &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + supplier.city + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" >State &nbsp&nbsp</td>");
                if (!string.IsNullOrEmpty(supplier.pincode)) {
                    sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + supplier.state + ',' + supplier.pincode + "</td>");
                }
                else {
                    sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + supplier.state + "</td>");
                }
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                if (!string.IsNullOrEmpty(supplier.mobile) && !string.IsNullOrEmpty(supplier.phone)) {
                    sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" >Mobile/Phone No &nbsp&nbsp</td>");
                    sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + supplier.mobile + ',' + supplier.phone + "</td>");
                }
                else if (!string.IsNullOrEmpty(supplier.mobile)) {
                    sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" >Mobile/Phone No &nbsp&nbsp</td>");
                    sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + supplier.mobile + "</td>");
                }
                else if (!string.IsNullOrEmpty(supplier.phone)) {
                    sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" >Mobile/Phone No &nbsp&nbsp</td>");
                    sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + supplier.phone + "</td>");
                }
                sb.AppendLine("</tr>");


                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" >State Code &nbsp&nbsp&nbsp </td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + state.state_name + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>PAN &nbsp&nbsp</b></td>");
                sb.AppendLine(string.Format("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>{0}</b></td>", supplier.pan_no));
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>GSTIN &nbsp&nbsp</b></td>");
                sb.AppendLine(string.Format("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>{0}</b></td>", supplier.TIN));
                sb.AppendLine("</tr>");

                sb.AppendLine("</table>");
                sb.AppendLine("</td>");


                sb.AppendLine("<td>");
                sb.AppendLine("<Table font-size=12pt;   class=\"boldText\"  style=\"border-collapse:collapse;\" >");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >Isuue No &nbsp&nbsp</td>");
                sb.AppendLine(string.Format("<td style=\"border-right:thin ; border-top:thin\" >{0}</td>", master.New_Bill_No));
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >Ref No &nbsp&nbsp</td>");
                sb.AppendLine(string.Format("<td style=\"border-right:thin ; border-top:thin\" >{0}</td>", issueNo));
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >Issue Date &nbsp&nbsp</td>");
                sb.AppendLine(string.Format("<td style=\"border-right:thin ; border-top:thin\" >{0}</td>", issueDate.ToString("dd/MM/yyyy")));
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" > Place of supply &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + supplier.state + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" >Issue Type &nbsp&nbsp</td>");
                KSTU_ISSUERECEIPTS_TYPES types = db.KSTU_ISSUERECEIPTS_TYPES.Where(it => it.company_code == companyCode
                                                                                    && it.branch_code == branchCode
                                                                                    && it.ir_code == master.issue_type).FirstOrDefault();

                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + types == null ? "" : types.ir_name + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" >Issued By &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + master.sal_code + "</td>");
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
                sb.AppendLine(string.Format("<TD style=\" border-bottom:thin;\" colspan = 10 ALIGN = \"CENTER\"><b>Tax Invoice</b></TD>"));//DELIVERY CHALLAN
                sb.AppendLine("</TR>");
                if (string.Compare(company.state, state.state_name) == 0) {
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"CENTER\"> INTRA-STATE STOCK TRANSFER</TD>"));
                    sb.AppendLine("</TR>");
                }
                else {
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"CENTER\"> INTER-STATE STOCK TRANSFER</TD>"));
                    sb.AppendLine("</TR>");
                }
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD  style=\"border-right:thin\" colspan=10 ALIGN = \"CENTER\"><b>Original for consignee/Duplicate for transporter/Triplicate for consigner</b><br></TD>"));//ORIGINAL/DUPLICATE
                sb.AppendLine("</TR>");

                sb.AppendLine("</table>");
                sb.AppendLine("</td>");

                string strIssueBillDetails = string.Format("EXEC [usp_IssueReceipts_HTMLPrints] '{0}', '{1}', '{2}', '{3}'",
                            issueNo, companyCode, branchCode, 'I');
                DataTable dtIssueBillDetails = SIGlobals.Globals.ExecuteQuery(strIssueBillDetails);

                if (dtIssueBillDetails != null && dtIssueBillDetails.Rows.Count > 1) {
                    sb.AppendLine("<Table font-size=9pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse;\" width=\"800\">");
                    sb.AppendLine("<TR bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                    sb.AppendLine(string.Format("<TH width=\"80\" style=font-size=\"12pt\" ALIGN = \"CENTER\"><b>{0}</b></TH>", dtIssueBillDetails.Columns[0].ColumnName));
                    sb.AppendLine(string.Format("<TH style=font-size=\"12pt\" ALIGN = \"CENTER\"><b>{0}</b></TH>", dtIssueBillDetails.Columns[1].ColumnName));
                    sb.AppendLine(string.Format("<TH style=font-size=\"12pt\" ALIGN = \"CENTER\"><b>{0}</b></TH>", dtIssueBillDetails.Columns[2].ColumnName));
                    sb.AppendLine(string.Format("<TH style=font-size=\"12pt\" ALIGN = \"CENTER\"><b>{0}</b></TH>", dtIssueBillDetails.Columns[3].ColumnName));
                    sb.AppendLine(string.Format("<TH style=font-size=\"12pt\" ALIGN = \"CENTER\"><b>{0}</b></TH>", dtIssueBillDetails.Columns[4].ColumnName));
                    sb.AppendLine(string.Format("<TH style=font-size=\"12pt\" ALIGN = \"CENTER\"><b>{0}</b></TH>", dtIssueBillDetails.Columns[5].ColumnName));
                    sb.AppendLine(string.Format("<TH style=font-size=\"12pt\" ALIGN = \"CENTER\"><b>{0}</b></TH>", dtIssueBillDetails.Columns[6].ColumnName));
                    sb.AppendLine(string.Format("<TH style=font-size=\"12pt\" ALIGN = \"CENTER\"><b>{0}</b></TH>", dtIssueBillDetails.Columns[7].ColumnName));
                    sb.AppendLine(string.Format("<TH style=font-size=\"12pt\" ALIGN = \"CENTER\"><b>{0}</b></TH>", dtIssueBillDetails.Columns[8].ColumnName));
                    sb.AppendLine(string.Format("<TH style=font-size=\"12pt\" ALIGN = \"CENTER\"><b>{0}</b></TH>", dtIssueBillDetails.Columns[9].ColumnName));
                    sb.AppendLine(string.Format("<TH style=font-size=\"12pt\" ALIGN = \"CENTER\"><b>{0}</b></TH>", dtIssueBillDetails.Columns[10].ColumnName));
                    sb.AppendLine("</TR>");

                    for (int i = 0; i < dtIssueBillDetails.Rows.Count - 1; i++) {
                        sb.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");

                        for (int j = 0; j < dtIssueBillDetails.Columns.Count; j++) {
                            if (string.Compare(dtIssueBillDetails.Columns[j].DataType.FullName.ToString(), "System.String") == 0 || j == 0)
                                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"CENTER\">{0}{1}</TD>", dtIssueBillDetails.Rows[i][j], "&nbsp"));
                            else {
                                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", dtIssueBillDetails.Rows[i][j], "&nbsp"));
                            }
                        }
                        sb.AppendLine("</TR>");
                    }
                    for (int i = 0; i < 10 - dtIssueBillDetails.Rows.Count - 1; i++) {
                        if (i == 0) {
                            sb.AppendLine("<TR>");
                            sb.AppendLine(string.Format("<TD colspan = {1}>{0}</TD>", "&nbsp", dtIssueBillDetails.Columns.Count));
                            sb.AppendLine("</TR>");
                        }
                        else {
                            sb.AppendLine("<TR>");
                            sb.AppendLine(string.Format("<TD style=\"border:0\";  colspan = {1}>{0}</TD>", "&nbsp", dtIssueBillDetails.Columns.Count));
                            sb.AppendLine("</TR>");
                        }
                    }
                }

                sb.AppendLine("<TR bgcolor='#FFFACD'  style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                int columnCOunt = dtIssueBillDetails.Columns.Count;
                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" colspan = 5 ALIGN = \"left\"><b>Total</b></TD>"));
                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>"
                 , dtIssueBillDetails.Rows[dtIssueBillDetails.Rows.Count - 1][columnCOunt - 6], "&nbsp", columnCOunt - 5));
                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>"
                     , dtIssueBillDetails.Rows[dtIssueBillDetails.Rows.Count - 1][columnCOunt - 5], "&nbsp", columnCOunt - 4));
                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>"
                         , dtIssueBillDetails.Rows[dtIssueBillDetails.Rows.Count - 1][columnCOunt - 4], "&nbsp", columnCOunt - 3));
                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>"
                         , dtIssueBillDetails.Rows[dtIssueBillDetails.Rows.Count - 1][columnCOunt - 3], "&nbsp", columnCOunt - 2));
                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>"
                         , dtIssueBillDetails.Rows[dtIssueBillDetails.Rows.Count - 1][columnCOunt - 2], "&nbsp"));
                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>"
                         , dtIssueBillDetails.Rows[dtIssueBillDetails.Rows.Count - 1][columnCOunt - 1], "&nbsp"));
                sb.AppendLine("</TR>");

                if (Convert.ToDecimal(master.IGST_Amount) > 0) {
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD  colspan=11 ALIGN = \"right\"><b>IGST Amt @ {0} % : </b></TD>", master.IGST_Percent));
                    sb.AppendLine(string.Format("<TD  style=\"border-right:thin\" ALIGN = \"right\"><b>{0}</b></TD>", master.IGST_Amount));
                    sb.AppendLine("</TR>");
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD  colspan=11 ALIGN = \"right\"><b>Invoice Value : </b></TD>"));//Final Amount
                    sb.AppendLine(string.Format("<TD  style=\"border-right:thin\"  ALIGN = \"right\"><b>{0}</b></TD>", master.final_amount));
                    sb.AppendLine("</TR>");
                }


                decimal Inword = 0.0M;
                string strWords = string.Empty;
                Inword = Convert.ToDecimal(master.final_amount);
                strWords = SIGlobals.Globals.ConvertNumbertoWordsinRupees(Inword);
                if (!string.IsNullOrEmpty(master.remarks)) {
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD style=\"border-right:thin ; border-bottom:0 \"; colspan=11 ALIGN = \"left\"><b>{0}{1}{1}</b></TD>",
                        "Remarks : " + master.remarks, "&nbsp", dtIssueBillDetails.Columns.Count));
                    sb.AppendLine("</TR>");
                }
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-right:thin ; border-bottom:0 \"; colspan=11 ALIGN = \"left\"><b>{0}{1}{1}</b></TD>",
                    strWords, "&nbsp", dtIssueBillDetails.Columns.Count));
                sb.AppendLine("</TR>");
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\" border-bottom:thin;\" colspan = 11 ALIGN = \"LEFT\"><b>Whether tax is payable on reverse charge basis? - No</b></TD>"));
                sb.AppendLine("</TR>");
                sb.AppendLine("<tr>");
                sb.AppendLine(string.Format("<td style=\"border-right:thin\"  colspan=6  align = \"left\"  ><b>{0}&nbsp<br><br><br>{1}&nbsp</b></td>", "&nbsp", "Signature of Consignee"));
                sb.AppendLine(string.Format("<td  colspan=5  align = \"right\"  ><b>For {0}&nbsp<br><br><br>{1}&nbsp</b></td>", company.company_name, "Authorized Signatory"));
                sb.AppendLine("</tr>");
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
