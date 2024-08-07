using ProdigyAPI.BL.BusinessLayer.Stock;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.SRBarcode;
using ProdigyAPI.Model.MagnaDb;
using ProdigyAPI.SIGlobals;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace ProdigyAPI.BL.BusinessLayer.SRBarcode
{
    public class SRBarcodingBL
    {

        /*
         * To interchange the assignments
         * https://stackoverflow.com/questions/1012807/invert-assignment-direction-in-visual-studio
         In Visual Studio 2015+ after selecting code block press Ctrl + H:

        Find: (\w+.\w+) = (\w+);

        Replace: $2 = $1;

        For example:

        entity.CreateDate = CreateDate;
        changes to:

        CreateDate = entity.CreateDate;*/
        MagnaDbEntities db = null;

        public SRBarcodingBL()
        {
            db = new MagnaDbEntities(true);
        }

        public SRBarcodingBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }

        public bool GetSRItemsToBeBarcoded(string companyCode, string branchCode, out List<SRItemToBeBarcodedVM> srItemToBeBarcodedList, out string errorMessage)
        {
            errorMessage = string.Empty;
            srItemToBeBarcodedList = new List<SRItemToBeBarcodedVM>();
            try {
                var srItems = db.usp_GetSRItemsToBeBarcoded(companyCode, branchCode).ToList();
                if (srItems != null && srItems.Count() > 0) {
                    srItemToBeBarcodedList =
                        (from sri in srItems
                         select new SRItemToBeBarcodedVM
                         {
                             CompanyCode = sri.company_code,
                             BranchCode = sri.branch_code,
                             SalesBillNo = sri.sales_bill_no,
                             SlNo = sri.sl_no,
                             BarcodeNo = sri.barcode_no,
                             GSCode = sri.gs_code,
                             CounterCode = sri.counter_code,
                             ItemCode = sri.item_name,
                             GrossWt = sri.GrossWt,
                             StoneWt = sri.StoneWt,
                             NetWt = sri.NetWt
                         }).ToList();

                }
                else {
                    errorMessage = "No SR details to barcode.";
                    return false;
                }
            }
            catch (Exception ex) {
                new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        private bool CheckIfBarcodeIsValid(string companyCode, string branchCode, int billNo, string barcodeNo,
            out usp_GetSRItemsToBeBarcoded_Result srLine, out KTTU_BARCODE_MASTER barcodeMast, out string errorMessage)
        {
            errorMessage = string.Empty;
            srLine = null;
            barcodeMast = null;
            List<SRItemToBeBarcodedVM> srItemsToBeBarcodedList = new List<SRItemToBeBarcodedVM>();
            try {
                var srItems = db.usp_GetSRItemsToBeBarcoded(companyCode, branchCode).ToList();
                if (srItems != null && srItems.Count() > 0) {
                    srLine = srItems.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                        && x.sales_bill_no == billNo && x.barcode_no == barcodeNo).FirstOrDefault();
                    if (srLine == null) {
                        errorMessage = "The provided SR Line is invalid.";
                        return false;
                    }

                    barcodeMast = db.KTTU_BARCODE_MASTER.Where(bm => bm.company_code == companyCode
                        && bm.branch_code == branchCode && bm.barcode_no == barcodeNo).OrderByDescending(bm => bm.UpdateOn)
                        .FirstOrDefault();
                    if (barcodeMast == null) {
                        errorMessage = "No barcode to re-barcode.";
                        return false;
                    }
                }
                else {
                    errorMessage = "No SR details to barcode. The selected barcode is invalid.";
                    return false;
                }
            }
            catch (Exception ex) {
                new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        public bool GetBarcodeDetailForReBarcoding(string companyCode, string branchCode, SRItemToBeBarcodedVM srItemsToBeBarcoded,
            out BarcodeMasterVM barcodeVM, out string errorMessage)
        {
            errorMessage = string.Empty;
            barcodeVM = null;
            try {
                if (srItemsToBeBarcoded == null) {
                    errorMessage = "SR item should be provided.";
                    return false;
                }
                if (srItemsToBeBarcoded.SalesBillNo == 0 || string.IsNullOrEmpty(srItemsToBeBarcoded.BranchCode) || string.IsNullOrEmpty(srItemsToBeBarcoded.BarcodeNo)) {
                    errorMessage = "A value must be provided for BranchCode, SalesBillNo & BarcodeNo.";
                    return false;
                }

                usp_GetSRItemsToBeBarcoded_Result srLine = null;
                KTTU_BARCODE_MASTER bm = null;
                if (!CheckIfBarcodeIsValid(companyCode, branchCode, srItemsToBeBarcoded.SalesBillNo, srItemsToBeBarcoded.BarcodeNo, out srLine, out bm, out errorMessage)) {
                    return false;
                }
                barcodeVM = new BarcodeMasterVM
                {
                    ObjID = bm.obj_id,
                    CompanyCode = bm.company_code,
                    BranchCode = bm.branch_code,
                    BarcodeNo = bm.barcode_no,
                    BatchNo = bm.batch_no,
                    SalCode = bm.sal_code,
                    OperatorCode = bm.operator_code,
                    Date = bm.date,
                    CounterCode = srLine.counter_code,
                    GSCode = srLine.gs_code,
                    ItemName = srLine.item_name,
                    Gwt = srLine.GrossWt,
                    Swt = bm.swt,
                    Nwt = srLine.NetWt,
                    Grade = bm.grade,
                    CatalogID = bm.catalog_id,
                    MakingChargePerRs = bm.making_charge_per_rs,
                    WastPercent = bm.wast_percent,
                    Qty = bm.qty,
                    ItemSize = bm.item_size,
                    DesignNo = bm.design_no,
                    PieceRate = bm.piece_rate,
                    DaimondAmount = bm.daimond_amount,
                    StoneAmount = bm.stone_amount,
                    OrderNo = bm.order_no,
                    SoldFlag = bm.sold_flag,
                    ProductCode = bm.product_code,
                    HallmarkCharges = bm.hallmark_charges,
                    Remarks = bm.remarks,
                    SupplierCode = bm.supplier_code,
                    OrderedCompanyCode = bm.ordered_company_code,
                    OrderedBranchCode = bm.ordered_branch_code,
                    Karat = bm.karat,
                    McAmount = bm.mc_amount,
                    WastageGrams = bm.wastage_grms,
                    McPercent = bm.mc_percent,
                    McType = bm.mc_type,
                    OldBarcodeNo = bm.old_barcode_no,
                    ProdIda = bm.prod_ida,
                    ProdTagNo = bm.prod_tagno,
                    UpdateOn = bm.UpdateOn,
                    LotNo = bm.Lot_No,
                    TagWt = bm.tag_wt,
                    IsConfirmed = bm.isConfirmed,
                    ConfirmedBy = bm.confirmedBy,
                    ConfirmedDate = bm.confirmedDate,
                    CurrentWt = bm.current_wt,
                    MCFor = bm.MC_For,
                    DiamondNo = bm.diamond_no,
                    BatchID = bm.batch_id,
                    AddWt = bm.add_wt,
                    WeightRead = bm.weightRead,
                    ConfirmedWeightRead = bm.confirmedweightRead,
                    PartyName = bm.party_name,
                    DesignName = bm.design_name,
                    ItemSizeName = bm.item_size_name,
                    MasterDesignCode = bm.master_design_code,
                    MasterDesignName = bm.master_design_name,
                    VendorModelNo = bm.vendor_model_no,
                    PurMcGram = bm.pur_mc_gram,
                    McPerPiece = bm.mc_per_piece,
                    TaggingType = bm.Tagging_Type,
                    BReceiptNo = bm.BReceiptNo,
                    BSNo = bm.BSNo,
                    IssueTo = bm.Issue_To,
                    PurMcAmount = bm.pur_mc_amount,
                    PurMcType = bm.pur_mc_type,
                    PurRate = bm.pur_rate,
                    SrBatchId = bm.sr_batch_id,
                    TotalSellingMc = bm.total_selling_mc,
                    PurDiamondAmount = bm.pur_diamond_amount,
                    TotalPurchaseMc = bm.total_purchase_mc,
                    PurStoneAmount = bm.pur_stone_amount,
                    PurPurityPercentage = bm.pur_purity_percentage,
                    PurWastageType = bm.pur_wastage_type,
                    PurWastageTypeValue = bm.pur_wastage_type_value,
                    CertificationNo = bm.certification_no,
                    RefNo = bm.ref_no,
                    ReceiptType = bm.receipt_type,
                    BarcodeStoneDetails = new List<BarcodeStoneVM>()
                };
                var barcodeStones = db.KTTU_BARCODE_STONE_DETAILS.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.barcode_no == bm.barcode_no).ToList();
                if(barcodeStones != null) {
                    barcodeVM.BarcodeStoneDetails = new List<BarcodeStoneVM>();
                    foreach (var st in barcodeStones) {
                        BarcodeStoneVM bs = new BarcodeStoneVM
                        {
                            ObjID = st.obj_id,
                            CompanyCode = st.company_code,
                            BranchCode = st.branch_code,
                            SlNo = st.sl_no,
                            BarcodeNo = st.barcode_no,
                            Type = st.type,
                            Name = st.name,
                            Qty = st.qty,
                            Carrat = st.carrat,
                            Rate = st.rate,
                            Amount = st.amount,
                            Clarity = st.clarity,
                            Color = st.color,
                            ProdIDA = st.prod_ida,
                            ProdTagNo = st.prod_tagno,
                            OldBarcodeNo = st.old_barcode_no,
                            UpdateOn = st.UpdateOn,
                            StoneType = st.stone_type,
                            StoneGSType = st.stone_gs_type,
                            Fin_Year = st.Fin_Year,
                            UOM = st.uom,
                            PurCost = st.pur_cost,
                            StoneCode = st.stone_code,
                            Shape = st.shape,
                            Cut = st.cut,
                            Polish = st.polish,
                            Symmetry = st.symmetry,
                            Fluorescence = st.fluorescence,
                            Certificate = st.certificate,
                            PurRate = st.pur_rate,
                            Size = st.Size
                        };
                        barcodeVM.BarcodeStoneDetails.Add(bs);
                    }
                }
            }
            catch (Exception ex) {
                errorMessage = new ErrorVM().GetErrorDetails(ex).description;
                return false;
            }

            return true;
        }

        public bool PostBarcode(string companyCode, string branchCode, int billNo, string barcodeNo, BarcodeMasterVM bm, string userID, out string newBarcodeNo, out string errorMessage)
        {
            errorMessage = string.Empty;
            newBarcodeNo = string.Empty;
            SRItemToBeBarcodedVM srItemsToBeBarcoded = new SRItemToBeBarcodedVM { CompanyCode = companyCode, BranchCode = branchCode, BarcodeNo = barcodeNo, SalesBillNo = billNo };
            BarcodeMasterVM dbBarcodeInfo = null;

            try {
                #region Object Validation
                if (!GetBarcodeDetailForReBarcoding(companyCode, branchCode, srItemsToBeBarcoded, out dbBarcodeInfo, out errorMessage)) {
                    return false;
                }
                if (!ValidateBarcodeItem(db, bm, out errorMessage)) {
                    return false;
                }

                if (!ValidateMinMC(db, bm, out errorMessage)) {
                    return false;
                }

                if (!ValidateStoneDetails(bm, out errorMessage)) {
                    return false;
                }
                #endregion

                #region Get New Barcode Number
                var barcodeSeq = db.KSTS_BARCODE_SEQ_NOS.Where(seq => seq.company_code == companyCode && seq.branch_code == branchCode).FirstOrDefault();
                if (barcodeSeq == null) {
                    errorMessage = "Barcode sequence number is not found.";
                    return false;
                }
                int newIntBarcodeNo = Convert.ToInt32(barcodeSeq.prev_barcode);
                newIntBarcodeNo = newIntBarcodeNo + 1;
                string query = string.Format("SELECT dbo.getencode({0}, '{1}','{2}')", barcodeSeq.prev_barcode, companyCode, branchCode);
                newBarcodeNo = db.Database.SqlQuery<string>(query).Single().ToString();
                barcodeSeq.prev_barcode = newIntBarcodeNo.ToString();
                db.Entry(barcodeSeq).State = System.Data.Entity.EntityState.Modified;
                #endregion

                #region Auto-confirm flag
                string autoConfirmFlag = "Y";
                var confirmSetting = db.KSTU_SETTINGS.Where(s => s.obj_id == "2" && s.company_code == companyCode
                    && s.branch_code == branchCode).FirstOrDefault();
                if (confirmSetting != null) {
                    if (Convert.ToInt32(confirmSetting.setting) == 0)
                        autoConfirmFlag = "N";
                }
                #endregion

                #region Barcode master insert
                string[] objIdString = new string[] { "KTTU_BARCODE_MASTER", newBarcodeNo.ToString(), companyCode, branchCode };
                string barcodeMastObjID = SIGlobals.Globals.GetMagnaGUID(objIdString, companyCode, branchCode);
                var applicationDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                var updatedTimestamp = SIGlobals.Globals.GetDateTime();
                string refNo = billNo.ToString() + "-" + barcodeNo;
                bm.OperatorCode = userID;
                KTTU_BARCODE_MASTER barcodeMaster = new KTTU_BARCODE_MASTER
                {
                    obj_id = barcodeMastObjID,
                    company_code = bm.CompanyCode,
                    branch_code = bm.BranchCode,
                    barcode_no = newBarcodeNo.ToString(),
                    batch_no = bm.BatchNo,
                    sal_code = bm.OperatorCode,
                    operator_code = bm.OperatorCode,
                    date = applicationDate,
                    counter_code = bm.CounterCode,
                    gs_code = bm.GSCode,
                    item_name = bm.ItemName,
                    gwt = bm.Gwt,
                    swt = bm.Swt,
                    nwt = bm.Nwt,
                    grade = bm.Grade,
                    catalog_id = newBarcodeNo.ToString() + ".jpg",
                    making_charge_per_rs = bm.MakingChargePerRs,
                    wast_percent = bm.WastPercent,
                    qty = bm.Qty,
                    item_size = bm.ItemSize,
                    design_no = bm.DesignNo,
                    piece_rate = bm.PieceRate,
                    daimond_amount = bm.DaimondAmount,
                    stone_amount = bm.StoneAmount,
                    order_no = bm.OrderNo,
                    sold_flag = "N",
                    product_code = bm.ProductCode,
                    hallmark_charges = bm.HallmarkCharges,
                    remarks = bm.Remarks,
                    supplier_code = dbBarcodeInfo.SupplierCode,
                    ordered_company_code = bm.CompanyCode,
                    ordered_branch_code = "",
                    karat = bm.Karat,
                    mc_amount = bm.McAmount,
                    wastage_grms = bm.WastageGrams,
                    mc_percent = bm.McPercent,
                    mc_type = bm.McType,
                    old_barcode_no = null,
                    prod_ida = bm.ProdIda,
                    prod_tagno = bm.ProdTagNo,
                    UpdateOn = updatedTimestamp,
                    Lot_No = 0,
                    tag_wt = bm.TagWt,
                    isConfirmed = autoConfirmFlag, //decided through settings
                    confirmedBy = bm.OperatorCode,
                    confirmedDate = applicationDate,
                    current_wt = srItemsToBeBarcoded.GrossWt + bm.TagWt,
                    MC_For = "N",
                    diamond_no = bm.DiamondNo,
                    batch_id = null,
                    add_wt = 0,
                    weightRead = "M",
                    confirmedweightRead = "A",
                    party_name = bm.PartyName,
                    design_name = bm.DesignName,
                    item_size_name = bm.ItemSizeName,
                    master_design_code = dbBarcodeInfo.MasterDesignCode,
                    master_design_name = dbBarcodeInfo.MasterDesignName,
                    vendor_model_no = dbBarcodeInfo.VendorModelNo,
                    pur_mc_gram = dbBarcodeInfo.PurMcGram,
                    mc_per_piece = bm.McPerPiece,
                    Tagging_Type = dbBarcodeInfo.TaggingType,
                    BReceiptNo = dbBarcodeInfo.BReceiptNo,
                    BSNo = dbBarcodeInfo.BSNo,
                    Issue_To = dbBarcodeInfo.IssueTo,
                    pur_mc_amount = dbBarcodeInfo.PurMcAmount,
                    pur_mc_type = dbBarcodeInfo.PurMcType,
                    pur_rate = dbBarcodeInfo.PurRate,
                    sr_batch_id = bm.SrBatchId,
                    total_selling_mc = bm.TotalSellingMc,
                    pur_diamond_amount = dbBarcodeInfo.PurDiamondAmount,
                    total_purchase_mc = dbBarcodeInfo.TotalPurchaseMc,
                    pur_stone_amount = dbBarcodeInfo.PurStoneAmount,
                    pur_purity_percentage = dbBarcodeInfo.PurPurityPercentage,
                    pur_wastage_type = dbBarcodeInfo.PurWastageType,
                    pur_wastage_type_value = dbBarcodeInfo.PurWastageTypeValue,
                    certification_no = dbBarcodeInfo.CertificationNo,
                    ref_no = refNo,
                    receipt_type = "S",
                    EntryDocType = "SRB",
                    EntryDocNo = billNo.ToString(),
                    EntryDate = applicationDate,
                    UniqRowID = Guid.NewGuid()
                };
                #endregion

                #region Barcode Stone details
                var finYear = Globals.GetFinancialYear(db, companyCode, branchCode);
                int stoneSlNo = 1;
                if (bm.BarcodeStoneDetails != null) {
                    var stoneAmount = Convert.ToDecimal(bm.BarcodeStoneDetails.Where(x => x.Type == "S").Sum(xs => xs.Amount));
                    var diamondAmount = Convert.ToDecimal(bm.BarcodeStoneDetails.Where(x => x.Type == "D").Sum(xs => xs.Amount));
                    barcodeMaster.stone_amount = stoneAmount;
                    barcodeMaster.daimond_amount = diamondAmount;
                    foreach (var st in bm.BarcodeStoneDetails) {
                        BarcodeStoneVM stonePurData = null;
                        if (dbBarcodeInfo.BarcodeStoneDetails != null) {
                            stonePurData = dbBarcodeInfo.BarcodeStoneDetails.Where(x => x.SlNo == st.SlNo).FirstOrDefault();
                        }

                        KTTU_BARCODE_STONE_DETAILS barcodeStone = new KTTU_BARCODE_STONE_DETAILS
                        {
                            obj_id = barcodeMastObjID,
                            company_code = st.CompanyCode,
                            branch_code = st.BranchCode,
                            sl_no = stoneSlNo,
                            barcode_no = newBarcodeNo.ToString(),
                            type = st.Type, //S or D
                            name = st.StoneCode, //
                            qty = st.Qty,
                            carrat = st.Carrat,
                            rate = st.Rate,
                            amount = st.Amount,
                            clarity = "",
                            color = st.Color,
                            prod_ida = null,
                            prod_tagno = null,
                            old_barcode_no = null,
                            UpdateOn = st.UpdateOn,
                            stone_type = st.StoneType, //Precious, Ordinary, etc
                            stone_gs_type = st.StoneGSType, //STN, DMD,
                            Fin_Year = finYear,
                            uom = "C",
                            stone_code = st.StoneCode,
                            shape = st.Shape,
                            cut = st.Cut,
                            polish = st.Polish,
                            symmetry = st.Symmetry,
                            fluorescence = st.Fluorescence,
                            certificate = st.Certificate,
                            pur_cost = st.PurCost,
                            /*stone_code = "",
                            shape = "",
                            cut = "",
                            polish = "",
                            symmetry = "",
                            fluorescence = "",
                            certificate = "",*/
                            pur_rate = stonePurData != null ? stonePurData.PurRate : 0,
                            Size = "",
                            UniqRowID = Guid.NewGuid()
                        };
                        stoneSlNo++;
                        db.KTTU_BARCODE_STONE_DETAILS.Add(barcodeStone);
                    }
                }
                db.KTTU_BARCODE_MASTER.Add(barcodeMaster);
                #endregion

                #region Insert to Auto-Confirm Table
                if (autoConfirmFlag == "Y") {
                    KTTU_CONFIRMED_BARCODE_MASTER confirmedBM = new KTTU_CONFIRMED_BARCODE_MASTER
                    {
                        obj_id = barcodeMastObjID,
                        company_code = companyCode,
                        branch_code = branchCode,
                        barcode_no = newBarcodeNo,
                        gwt = barcodeMaster.gwt,
                        swt = barcodeMaster.swt,
                        nwt = barcodeMaster.nwt,
                        date = applicationDate,
                        obj_status = "N",
                        BReceiptNo = barcodeMaster.BReceiptNo,
                        batch_id = barcodeMaster.batch_id,
                        qty = barcodeMaster.qty,
                        carrat = 0
                    };
                    db.KTTU_CONFIRMED_BARCODE_MASTER.Add(confirmedBM);
                }
                #endregion

                #region Stock Posting
                bool stockUpdated = UpdateItemStock(companyCode, branchCode, db, barcodeMaster, newIntBarcodeNo, postReverse: false, errorMessage: out errorMessage);
                if (!stockUpdated) {
                    return false;
                }
                #endregion

                #region Update IsBarcoded flag in KTTU_SR_DETAILS table
                var srDetailsTbl = db.KTTU_SR_DETAILS.Where(sri => sri.company_code == companyCode && sri.branch_code == branchCode
                    && sri.sales_bill_no == billNo && sri.barcode_no == barcodeNo).FirstOrDefault();
                if (srDetailsTbl == null) {
                    errorMessage = string.Format("The barcode {0} for which new barcode has to be generated is not found.", barcodeNo);
                    return false;
                }
                srDetailsTbl.isBarcoded = "Y";
                srDetailsTbl.UpdateOn = SIGlobals.Globals.GetDateTime();
                db.Entry(srDetailsTbl).State = System.Data.Entity.EntityState.Modified;
                #endregion

                db.SaveChanges();
            }
            catch (Exception ex) {
                errorMessage = new ErrorVM().GetErrorDetails(ex).description;
                return false;
            }
            return true;
        }

        private bool UpdateItemStock(string companyCode, string branchCode, MagnaDbEntities dbContext, KTTU_BARCODE_MASTER bm, int barcodeNo, bool postReverse, out string errorMessage)
        {
            errorMessage = string.Empty;
            #region Post Counter Stock
            var gsj = new StockJournalVM
            {
                StockTransType = StockTransactionType.BarcodeReceipt,
                CompanyCode = bm.company_code,
                BranchCode = bm.branch_code,
                DocumentNo = barcodeNo,
                GS = bm.gs_code,
                Counter = bm.counter_code,
                Item = bm.item_name,
                Qty = Convert.ToInt32(bm.qty),
                GrossWt = Convert.ToDecimal(bm.gwt),
                StoneWt = Convert.ToDecimal(bm.swt),
                NetWt = Convert.ToDecimal(bm.nwt),
            };
            List<StockJournalVM> generalStockJournal = new List<StockJournalVM>();
            generalStockJournal.Add(gsj);

            StockPostBL stockPost = new StockPostBL();
            bool counterStockPostSuccess = stockPost.CounterStockPost(dbContext, generalStockJournal.ToList(), postReverse, out errorMessage);
            if (!counterStockPostSuccess) {return false;
            }
            #endregion

            #region Post GS Stock - Not required
            //var summarizedGSStockJournal =
            //        from cj in generalStockJournal
            //        group cj by new { Company = cj.CompanyCode, Branch = cj.BranchCode, GS = cj.GS, DocumentNo = cj.DocumentNo } into g
            //        select new StockJournalVM
            //        {
            //            StockTransType = StockTransactionType.BarcodeReceipt,
            //            CompanyCode = g.Key.Company,
            //            BranchCode = g.Key.Branch,
            //            DocumentNo = g.Key.DocumentNo,
            //            GS = g.Key.GS,
            //            Counter = "",
            //            Item = "",
            //            Qty = g.Sum(x => x.Qty),
            //            GrossWt = g.Sum(x => x.GrossWt),
            //            StoneWt = g.Sum(x => x.StoneWt),
            //            NetWt = g.Sum(x => x.NetWt)
            //        };
            //bool gsStockPostSuccess = stockPost.GSStockPost(dbContext, summarizedGSStockJournal.ToList(), postReverse, out errorMessage);
            //if (!gsStockPostSuccess) {
            //    return false;
            //}
            #endregion

            return true;
        }

        public bool ValidateBarcodeItem(MagnaDbEntities dbContext, BarcodeMasterVM barcodeVM, out string errorMessage)
        {
            errorMessage = string.Empty;
            bool isPieceItem = barcodeVM.PieceRate > 0 ? true : false;
            if (barcodeVM.Qty < 1) {
                errorMessage = "Please enter the Qty";
                return false;
            }

            if (string.IsNullOrEmpty(barcodeVM.ItemName)) {
                errorMessage = "Please select the item name";
                return false;
            }
            if (string.IsNullOrEmpty(barcodeVM.DesignNo)) {
                errorMessage = "Please select the proper design";
                return false;
            }
            if (string.IsNullOrEmpty(barcodeVM.CounterCode)) {
                errorMessage = "Please select the counter ";
                return false;
            }

            if (Convert.ToDecimal(barcodeVM.TagWt) <= 0) {
                errorMessage = "Please enter the the Tag Weight";
                return false;
            }

            if (!isPieceItem) {
                if ((Convert.ToDecimal(barcodeVM.Gwt) <= 0)) {
                    errorMessage = "Please enter the Gross weight";
                    return false;
                }
            }
            if (Convert.ToDecimal(barcodeVM.Swt) > 0) {
                if (Convert.ToDecimal(barcodeVM.Gwt) <= Convert.ToDecimal(barcodeVM.Swt)) {
                    errorMessage = "Stone Weight cannot be greater than Gross Weight";
                    return false;
                }
            }
                                    
            if (isPieceItem) {
                if (barcodeVM.PieceRate < (barcodeVM.StoneAmount)) {
                    errorMessage = "Piece Rate should be Greater than the stone Amount";
                    return false;
                }
            }
            if (string.IsNullOrEmpty(barcodeVM.ItemSize)) {
                errorMessage = "Please select the item size";
                return false;
            }

            if (!isPieceItem) {
                int MCType = Convert.ToInt32(barcodeVM.McType);
                if (MCType == 1) {
                    if (Convert.ToDouble(barcodeVM.MakingChargePerRs) <= 0) {

                        errorMessage = "Please Enter MC/Gram";
                        return false;
                    }
                }
                else if (MCType == 2) {
                    if (Convert.ToDouble(barcodeVM.McAmount) <= 0) {
                        errorMessage = "Please Enter MC Amount";
                        return false;
                    }
                }
                else if (MCType == 3) {
                    if (Convert.ToDouble(barcodeVM.McAmount) <= 0) {
                        errorMessage = "Please Enter MC Amount";
                        return false;
                    }
                }
                else if (MCType == 4) {
                    if (Convert.ToDouble(barcodeVM.McAmount) <= 0) {
                        errorMessage = "Please Enter MC Amount";
                        return false;
                    }
                }
                else if (MCType == 5) {
                    if (Convert.ToDouble(barcodeVM.McPercent) <= 0) {
                        errorMessage = "Please Enter MC%";
                        return false;
                    }
                }
                else if (MCType == 6) {

                    if (Convert.ToDouble(barcodeVM.McPerPiece) <= 0) {
                        errorMessage = "Please Enter  MC/Piece";
                        return false;
                    }
                }
            }
            return true;
        }

        private bool ValidateStoneDetails(BarcodeMasterVM barcodeVM, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (barcodeVM.BarcodeStoneDetails == null)
                return true;
            int lineNo = 1;
            foreach (var bs in barcodeVM.BarcodeStoneDetails) {
                if (string.IsNullOrEmpty(bs.StoneGSType)) {
                    errorMessage = "Please select the stone GS Type for stone line number: " + lineNo.ToString();
                    return false;
                }
                if (string.IsNullOrEmpty(bs.StoneType)) {
                    errorMessage = "Please select the stone type for stone line number: " + lineNo.ToString();
                    return false;
                }
                if (bs.StoneType != "S" || bs.StoneType != "D") {
                    errorMessage = "Stone type must either be S or D for stone line number: " + lineNo.ToString();
                    return false;
                }
                if (string.IsNullOrEmpty(bs.StoneCode)) {
                    errorMessage = "Please select the stone code for stone line number: " + lineNo.ToString();
                    return false;
                }
                if ((Convert.ToUInt32(bs.Qty) <= 0)) {
                    errorMessage = "Please enter the quantity for stone line number: " + lineNo.ToString();
                    return false;
                }

                //if (!chkPieces.Checked) {
                //    if (string.IsNullOrEmpty(txtstonecarat.Text) || Convert.ToDecimal(txtstonecarat.Text) <= 0) {

                //        errorMessage = "Please enter the carat";
                //        return false;
                //    }
                //}
                if (Convert.ToDecimal(bs.Amount) <= 0) {
                    errorMessage = "Please enter the Selling Amountfor stone line number: " + lineNo.ToString();
                    return false;
                }
                if (Convert.ToDecimal(bs.PurCost) > Convert.ToDecimal(bs.Amount)) {
                    errorMessage = "Stone purchase amount cannot be greater than selling amount for stone line number: " + lineNo.ToString();
                    return false;
                }
            }
            return true;
        }

        private bool ValidateMinMC(MagnaDbEntities dbContext, BarcodeMasterVM barcodeVM, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (Convert.ToDecimal(barcodeVM.PieceRate) == 0) {
                return true;
            }

            if(barcodeVM == null) {
                errorMessage = "Barcode detail is null.";
                return false;
            }

            decimal minMC = 0.00M;
            decimal enteredMC = 0.00M;
            decimal toleranceMC = 0.00M;
            if (string.Compare(barcodeVM.McType, "1") == 0) {
                minMC = GetMinProfitPercentage(barcodeVM.CompanyCode, barcodeVM.BranchCode, barcodeVM.GSCode, barcodeVM.ItemName);
                enteredMC = Convert.ToDecimal(barcodeVM.MakingChargePerRs);
                toleranceMC = enteredMC * Convert.ToDecimal(barcodeVM.Nwt);
            }
            else if (string.Compare(barcodeVM.McType, "6") == 0) {
                minMC = GetMinProfitPercentage(barcodeVM.CompanyCode, barcodeVM.BranchCode, barcodeVM.GSCode, barcodeVM.ItemName);
                enteredMC = Convert.ToDecimal(barcodeVM.McPerPiece);
                toleranceMC = enteredMC * Convert.ToDecimal(barcodeVM.Qty);
            }

            else if (string.Compare(barcodeVM.McType, "5") == 0) {
                minMC = GetMinProfitPercentage(barcodeVM.CompanyCode, barcodeVM.BranchCode, barcodeVM.GSCode, barcodeVM.ItemName);
                enteredMC = Convert.ToDecimal(barcodeVM.McPercent);
                toleranceMC = enteredMC * Convert.ToDecimal(barcodeVM.Qty);
            }

            if (enteredMC < minMC) {
                errorMessage = string.Format("MC entered {0} is less than the minimum MC {1} set in masters.",
                    enteredMC, minMC);
                return false;
            }

            return true;

        }

        private decimal GetMinProfitPercentage(string companyCode, string branchCode, string gSCode, string itemName)
        {
            var minProfit = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(ff => ff.company_code == companyCode && ff.branch_code == branchCode
                && ff.gs_code == gSCode && ff.ischild == "Y" && (ff.item_level6_name == itemName ||
                ff.item_level5_name == itemName || ff.item_level4_name == itemName ||
                ff.item_level3_name == itemName || ff.item_level2_name == itemName)
                ).Select(fy => fy.min_profit_percent).FirstOrDefault();
            return Convert.ToDecimal(minProfit);            
        }
    }
}
