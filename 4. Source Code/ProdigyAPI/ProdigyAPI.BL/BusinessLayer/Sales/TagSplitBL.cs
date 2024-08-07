using ProdigyAPI.BL.BusinessLayer.Repair;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Sales;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Sales
{
    public class TagSplitBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        private const string MODULE_SEQ_NO = "4";
        #endregion

        #region Methods

        public List<GenComboVM> GetDesign(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            List<GenComboVM> lstGenCombo = new List<GenComboVM>();
            try {
                //var data = db.usp_LoadDesignMaster(companyCode, branchCode, "B");
                var data = db.KTTU_DESIGN_MASTER.Where(kdm => kdm.company_code == companyCode && kdm.branch_code == branchCode && kdm.obj_status == "O").ToList();
                if (data != null && data.Count() > 0) {
                    foreach (var kim in data) {
                        GenComboVM genComobo = new GenComboVM();
                        genComobo.Code = kim.design_code;
                        genComobo.Name = kim.design_name;
                        lstGenCombo.Add(genComobo);
                    }
                }
                return lstGenCombo;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public List<GenComboVM> GetSize(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            List<GenComboVM> lstGenCombo = new List<GenComboVM>();
            try {
                var data = db.KSTS_ITEMSIZE_MASTER.Where(kim => kim.company_code == companyCode && kim.branch_code == branchCode).ToList();
                if (data != null && data.Count > 0) {
                    foreach (KSTS_ITEMSIZE_MASTER kim in data) {
                        GenComboVM genComobo = new GenComboVM();
                        genComobo.Code = kim.item_code;
                        genComobo.Name = kim.item_name;
                        lstGenCombo.Add(genComobo);
                    }
                }
                return lstGenCombo;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public List<GenComboVM> GetStoneGSType(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            List<GenComboVM> lstGenCombo = new List<GenComboVM>();
            try {
                string[] ary = { "ST", "DM" };
                var data = db.KSTS_GS_ITEM_ENTRY.Where(kg => kg.bill_type == "S"
                                                        && ary.Contains(kg.metal_type) //(kg.metal_type == "ST" || kg.metal_type == "DM")
                                                        && kg.company_code == companyCode
                                                        && kg.branch_code == branchCode
                                                        && kg.measure_type == "C").ToList();
                if (data != null && data.Count > 0) {
                    foreach (KSTS_GS_ITEM_ENTRY kim in data) {
                        GenComboVM genComobo = new GenComboVM();
                        genComobo.Code = kim.gs_code;
                        genComobo.Name = kim.item_level1_name;
                        lstGenCombo.Add(genComobo);
                    }
                }
                return lstGenCombo;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public List<GenComboVM> GetStoneName(string companyCode, string branchCode, string stoneGSType, string stoneType, out ErrorVM error)
        {
            error = null;
            List<GenComboVM> lstGenCombo = new List<GenComboVM>();
            try {
                if (stoneGSType == "STN") {
                    lstGenCombo = (from st in db.KSTU_STONE_DIAMOND_MASTER
                                   join it in db.KSTS_GS_ITEM_ENTRY
                                   on new { GSCode = st.type, CompanyCode = st.company_code, BranchCode = st.branch_code }
                                   equals new { GSCode = it.gs_code, CompanyCode = it.company_code, BranchCode = it.branch_code }
                                   where it.metal_type == "ST" && st.stone_types == stoneType && it.company_code == companyCode && it.branch_code == branchCode
                                   select new GenComboVM()
                                   {
                                       Code = st.code,
                                       Name = st.stone_name
                                   }).OrderBy(lt => lt.Name).ToList();
                }
                else if (stoneGSType == "DMD") {
                    lstGenCombo = (from st in db.KSTU_STONE_DIAMOND_MASTER
                                   join it in db.KSTS_GS_ITEM_ENTRY
                                   on new { GSCode = st.type, CompanyCode = st.company_code, BranchCode = st.branch_code }
                                   equals new { GSCode = it.gs_code, CompanyCode = it.company_code, BranchCode = it.branch_code }
                                   where st.type == "DMD"
                                            && it.metal_type == "DM"
                                            && it.company_code == companyCode
                                            && it.branch_code == branchCode

                                   select new GenComboVM()
                                   {
                                       Code = st.code,
                                       Name = st.stone_name
                                   }).OrderBy(lt => lt.Name).ToList();
                }
                return lstGenCombo;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public List<TagVM> GetTagDetails(string companyCode, string branchCode, string tagNo, out ErrorVM error)
        {
            error = null;
            List<TagVM> lstOfTags = new List<TagVM>();
            KTTU_BARCODE_MASTER barcode = db.KTTU_BARCODE_MASTER.Where(b => b.barcode_no == tagNo && b.company_code == companyCode && b.branch_code == branchCode).FirstOrDefault();

            if (barcode.sold_flag == "Y") {
                error = new ErrorVM()
                {
                    index = 0,
                    field = "Barcode",
                    description = "Invalid Barcode Number.",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return null;
            }

            TagVM tag = new TagVM();
            tag.ObjID = barcode.obj_id;
            tag.CompanyCode = barcode.company_code;
            tag.BranchCode = barcode.branch_code;
            tag.BarcodeNo = barcode.barcode_no;
            tag.BatchNo = barcode.batch_no;
            tag.SalCode = barcode.sal_code;
            tag.OperatorCode = barcode.operator_code;
            tag.Date = barcode.date;
            tag.CounterCode = barcode.counter_code;
            tag.GSCode = barcode.gs_code;
            tag.ItemName = barcode.item_name;
            tag.Gwt = barcode.gwt;
            tag.Swt = barcode.swt;
            tag.Nwt = barcode.nwt;
            tag.Grade = barcode.grade;
            tag.CatalogID = barcode.catalog_id;
            tag.MakingChargePerRs = barcode.making_charge_per_rs;
            tag.WastePercent = barcode.wast_percent;
            tag.Qty = barcode.qty;
            tag.ItemSize = barcode.item_size;
            tag.DesignNo = barcode.design_no;
            tag.PieceRate = barcode.piece_rate;
            tag.DiamondAmount = barcode.daimond_amount;
            tag.StoneAmount = barcode.stone_amount;
            tag.OrderNo = barcode.order_no;
            tag.SoldFlag = barcode.sold_flag;
            tag.ProductCode = barcode.product_code;
            tag.HallmarkCharges = barcode.hallmark_charges;
            tag.Remarks = barcode.remarks;
            tag.SupplierCode = barcode.supplier_code;
            tag.OrderedCompanyCode = barcode.ordered_company_code;
            tag.OrderedBranchCode = barcode.ordered_branch_code;
            tag.Karat = barcode.karat;
            tag.McAmount = barcode.mc_amount;
            tag.WastageGrams = barcode.wastage_grms;
            tag.McPercent = barcode.mc_percent;
            tag.McType = barcode.mc_type;
            tag.OldBarcodeNo = barcode.old_barcode_no;
            tag.ProdIda = barcode.prod_ida;
            tag.ProdTagno = barcode.prod_tagno;
            tag.UpdateOn = barcode.UpdateOn;
            tag.LotNo = barcode.Lot_No;
            tag.TagWt = barcode.tag_wt;
            tag.IsConfirmed = barcode.isConfirmed;
            tag.ConfirmedBy = barcode.confirmedBy;
            tag.ConfirmedDate = barcode.confirmedDate;
            tag.CurrentWt = barcode.current_wt;
            tag.MCFor = barcode.MC_For;
            tag.DiamondNo = barcode.diamond_no;
            tag.BatchId = barcode.batch_id;
            tag.AddWt = barcode.add_wt;
            tag.WeightRead = barcode.weightRead;
            tag.ConfirmedweightRead = barcode.confirmedweightRead;
            tag.PartyName = barcode.party_name;
            tag.DesignName = barcode.design_name;
            tag.ItemSizeName = barcode.item_size_name;
            tag.MasterDesignCode = barcode.master_design_code;
            tag.MasterDesignName = barcode.master_design_name;
            tag.VendorModelNo = barcode.vendor_model_no;
            tag.PurMcGram = barcode.pur_mc_gram;
            tag.McPerPiece = barcode.mc_per_piece;
            tag.TaggingType = barcode.Tagging_Type;
            tag.BReceiptNo = barcode.BReceiptNo;
            tag.BSNo = barcode.BSNo;
            tag.IssueTo = barcode.Issue_To;
            tag.PurMcAmount = barcode.pur_mc_amount;
            tag.PurMcType = barcode.pur_mc_type;
            tag.PurRate = barcode.pur_rate;
            tag.SRBatchID = barcode.sr_batch_id;
            tag.TotalSellingMC = barcode.total_selling_mc;
            tag.PurDiamondAmount = barcode.pur_diamond_amount;
            tag.TotalPurchaseMc = barcode.total_purchase_mc;
            tag.PurStoneAmount = barcode.pur_stone_amount;
            tag.PurPurityPercentage = barcode.pur_purity_percentage;
            tag.PurWastageType = barcode.pur_wastage_type;
            tag.PurWastageTypeValue = barcode.pur_wastage_type_value;
            tag.UniqRowID = barcode.UniqRowID;
            tag.CertificationNo = barcode.certification_no;
            tag.RefNo = barcode.ref_no;
            tag.ReceiptType = barcode.receipt_type;
            tag.EntryDocType = barcode.EntryDocType;
            tag.EntryDate = barcode.EntryDate;
            tag.EntryDocNo = barcode.EntryDocNo;
            tag.ExitDocType = barcode.ExitDocType;
            tag.ExitDate = barcode.ExitDate;
            tag.ExitDocNo = barcode.ExitDocNo;
            tag.OnlineStock = barcode.OnlineStock;
            tag.IsShuffled = barcode.is_shuffled;
            tag.Shuffled_date = barcode.shuffled_date;
            tag.Collections = barcode.Collections;

            // Taking Stone Details
            List<SalesEstStoneVM> lstSalEstStone = new List<SalesEstStoneVM>();
            List<KTTU_BARCODE_STONE_DETAILS> stoneInfo = db.KTTU_BARCODE_STONE_DETAILS.Where(st => st.barcode_no == tag.BarcodeNo && st.company_code == companyCode && st.branch_code == branchCode).ToList();
            for (int i = 0; i < stoneInfo.Count(); i++) {
                SalesEstStoneVM ses = new SalesEstStoneVM();
                ses.ObjID = stoneInfo[i].obj_id;
                ses.CompanyCode = stoneInfo[i].company_code;
                ses.BranchCode = stoneInfo[i].branch_code;
                ses.SlNo = stoneInfo[i].sl_no;
                ses.BarcodeNo = stoneInfo[i].barcode_no;
                ses.Type = stoneInfo[i].type == "S" ? "STN" : "DMD";
                ses.Name = stoneInfo[i].name;
                ses.Qty = Convert.ToInt32(stoneInfo[i].qty);
                ses.Carrat = Convert.ToDecimal(stoneInfo[i].carrat);
                ses.Rate = stoneInfo[i].rate;
                ses.Amount = Convert.ToDecimal(stoneInfo[i].amount);
                ses.StoneWt = stoneInfo[i].carrat / 5;
                ses.FinYear = stoneInfo[i].Fin_Year;
                ses.Color = stoneInfo[i].color;
                ses.Clarity = stoneInfo[i].clarity;
                ses.Shape = stoneInfo[i].shape;
                ses.Cut = stoneInfo[i].cut;
                ses.Polish = stoneInfo[i].polish;
                ses.Symmetry = stoneInfo[i].symmetry;
                ses.Fluorescence = stoneInfo[i].fluorescence;
                ses.Certificate = stoneInfo[i].certificate;
                lstSalEstStone.Add(ses);
            }
            tag.lstOfStone = lstSalEstStone;
            lstOfTags.Add(tag);
            return lstOfTags;
        }

        public List<TagVM> SplitBarcodeDetailsWithCalculation(string companyCode, string branchCode, List<TagVM> tag, out ErrorVM error)
        {
            error = null;
            List<TagVM> returnTag = new List<TagVM>();
            ErrorVM orginalTagError = new ErrorVM();
            TagVM originalTag = new TagVM();
            TagVM firstTag = new TagVM();
            TagVM secondTag = new TagVM();

            originalTag = GetTagDetails(companyCode, branchCode, tag[0].BarcodeNo, out orginalTagError).FirstOrDefault();
            if (tag.Count == 1) {
                firstTag = tag[0]; //GetTagDetails(companyCode, branchCode, tag[0].BarcodeNo, out orginalTagError).FirstOrDefault();
                secondTag = GetTagDetails(companyCode, branchCode, tag[0].BarcodeNo, out orginalTagError).FirstOrDefault();
                secondTag.lstOfStone = new List<SalesEstStoneVM>();
            }
            else {
                firstTag = tag[0];
                secondTag = tag[1];
            }

            //First Tag Calculation
            firstTag.Nwt = Convert.ToDecimal(firstTag.Gwt) - Convert.ToDecimal(firstTag.Swt);

            //Second Tag Calculation
            secondTag.Gwt = Convert.ToDecimal(originalTag.Gwt) - Convert.ToDecimal(firstTag.Gwt);
            secondTag.Swt = Convert.ToDecimal(originalTag.Swt) - Convert.ToDecimal(firstTag.Swt);
            secondTag.Nwt = Convert.ToDecimal(originalTag.Nwt) - Convert.ToDecimal(firstTag.Nwt);

            returnTag.Add(firstTag);
            returnTag.Add(secondTag);
            return returnTag;
        }

        public List<string> SplitAndSaveBarcode(List<TagVM> tag, out ErrorVM error)
        {
            error = null;
            string companyCode = tag[0].CompanyCode;
            string branchCode = tag[0].BranchCode;
            ErrorVM orginalTagError = new ErrorVM();
            TagVM originalTag = new TagVM();
            TagVM firstTag = new TagVM();
            TagVM secondTag = new TagVM();
            List<SalesEstStoneVM> firstTagStone = new List<SalesEstStoneVM>();
            List<SalesEstStoneVM> secondTagStone = new List<SalesEstStoneVM>();
            List<string> newBarcodeNo = new List<string>();

            #region Validation
            if (tag.Count != 2) {
                error = new ErrorVM()
                {
                    field = "Split Tag",
                    description = "Invalid Tag Details.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return null;
            }

            originalTag = GetTagDetails(companyCode, branchCode, tag[0].BarcodeNo, out orginalTagError).FirstOrDefault();
            if (orginalTagError != null) {
                error = orginalTagError;
                return null;
            }
            firstTag = tag[0];
            firstTagStone = firstTag.lstOfStone;

            secondTag = tag[1];
            secondTagStone = secondTag.lstOfStone;

            if (firstTag.Gwt > originalTag.Gwt) {
                error = new ErrorVM()
                {
                    field = "Gross Wt (Gwt)",
                    description = "Gross weight should not be more than Original Weight",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return null;
            }

            if (firstTag.Swt > firstTag.Gwt) {
                error = new ErrorVM()
                {
                    field = "Gross Wt (Gwt)",
                    description = "Stone weight should not be more than Gross Weight in Tag 1",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return null;
            }

            if (secondTag.Swt > secondTag.Gwt) {
                error = new ErrorVM()
                {
                    field = "Gross Wt (Gwt)",
                    description = "Stone weight should not be more than Gross Weight in Tag 2",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return null;
            }

            if (originalTag.Gwt != (firstTag.Gwt + secondTag.Gwt)) {
                error = new ErrorVM()
                {
                    field = "Gross Wt (Gwt)",
                    description = "Sum of Tag 1 Gross Weight (" +
                    firstTag.Gwt + ") and Tag 2 Gross Weight (" +
                    secondTag.Gwt + ") should be equal to Orginal Tag Gross Weight" + originalTag.Gwt,
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return null;
            }

            decimal firstTagStoneKarat = 0;
            foreach (SalesEstStoneVM s in firstTagStone) {
                firstTagStoneKarat = firstTagStoneKarat + s.Carrat;
            }

            decimal secondTagStoneKarat = 0;
            foreach (SalesEstStoneVM s in secondTagStone) {
                secondTagStoneKarat = secondTagStoneKarat + s.Carrat;
            }

            decimal totalStoneKarat = firstTagStoneKarat + secondTagStoneKarat;
            decimal totalStoneWt = totalStoneKarat * Convert.ToDecimal(0.2);
            if (totalStoneWt != originalTag.Swt) {
                error = new ErrorVM()
                {
                    field = "Stone Weight",
                    description = "Sum of Tag 1 Stone Karat (" + firstTagStoneKarat
                    + ") and Tag 2 Stone Karat (" + secondTagStoneKarat
                    + ") should be equal to Orginal Tag Stone Karat"
                    + (originalTag.Swt / Convert.ToDecimal(0.2)),
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return null;
            }
            #endregion

            using (var transaction = db.Database.BeginTransaction()) {
                try {

                    #region Issue to Counter Stock
                    KTTU_COUNTER_STOCK kcsd = db.KTTU_COUNTER_STOCK.Where(kcs => kcs.gs_code == originalTag.GSCode
                                        && kcs.item_name == originalTag.ItemName
                                        && kcs.counter_code == originalTag.CounterCode
                                        && kcs.company_code == companyCode
                                        && kcs.branch_code == branchCode).FirstOrDefault();
                    if (kcsd != null) {
                        kcsd.issues_units += Convert.ToInt32(originalTag.Qty);
                        kcsd.issues_gwt += originalTag.Gwt;
                        kcsd.issues_swt += originalTag.Swt;
                        kcsd.issues_nwt += Convert.ToDecimal(originalTag.Nwt);
                        kcsd.closing_units -= Convert.ToInt32(originalTag.Qty);
                        kcsd.closing_gwt -= originalTag.Gwt;
                        kcsd.closing_swt -= originalTag.Swt;
                        kcsd.closing_nwt -= Convert.ToDecimal(originalTag.Nwt);
                        db.Entry(kcsd).State = System.Data.Entity.EntityState.Modified;
                    }
                    else {
                        KTTU_COUNTER_STOCK counterStock = new KTTU_COUNTER_STOCK();
                        counterStock.obj_id = SIGlobals.Globals.GetNewGUID();
                        counterStock.gs_code = originalTag.GSCode;
                        counterStock.item_name = originalTag.ItemName;
                        counterStock.counter_code = originalTag.CounterCode;
                        counterStock.date = SIGlobals.Globals.GetDateTime();
                        counterStock.company_code = companyCode;
                        counterStock.branch_code = branchCode;
                        counterStock.op_units = 0;
                        counterStock.op_gwt = 0.000M;
                        counterStock.op_nwt = 0.000M;
                        counterStock.op_swt = 0.000M;
                        counterStock.barcoded_units = 0;
                        counterStock.barcoded_gwt = 0.000M;
                        counterStock.barcoded_swt = 0.000M;
                        counterStock.barcoded_nwt = 0.000M;
                        counterStock.sales_units = 0;
                        counterStock.sales_gwt = 0.000M;
                        counterStock.sales_swt = 0.000M;
                        counterStock.sales_nwt = 0.000M;
                        counterStock.receipt_units = Convert.ToInt32(originalTag.Qty);
                        counterStock.receipt_gwt = originalTag.Gwt;
                        counterStock.receipt_swt = originalTag.Swt;
                        counterStock.receipt_nwt = Convert.ToDecimal(originalTag.Nwt);
                        counterStock.issues_units = 0;
                        counterStock.issues_gwt = 0.000M;
                        counterStock.issues_swt = 0.000M;
                        counterStock.issues_nwt = 0.000M;
                        counterStock.closing_units = Convert.ToInt32(originalTag.Qty);
                        counterStock.closing_gwt = originalTag.Gwt;
                        counterStock.closing_swt = originalTag.Swt;
                        counterStock.closing_nwt = Convert.ToDecimal(originalTag.Nwt);
                        db.KTTU_COUNTER_STOCK.Add(counterStock);
                    }
                    #endregion

                    #region Save New Barcode Information
                    KTTU_BARCODE_MASTER kbm = db.KTTU_BARCODE_MASTER.Where(bm => bm.barcode_no == originalTag.BarcodeNo
                                                                            && bm.company_code == companyCode
                                                                            && bm.branch_code == branchCode).FirstOrDefault();
                    kbm.sold_flag = "Y";
                    kbm.ExitDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                    kbm.ExitDocNo = "0";
                    kbm.ExitDocType = "TS";
                    db.Entry(kbm).State = System.Data.Entity.EntityState.Modified;
                    foreach (TagVM t in tag) {
                        string getBarcode = db.KSTS_BARCODE_SEQ_NOS.Where(bs => bs.company_code == companyCode
                                                                            && bs.branch_code == branchCode).FirstOrDefault().prev_barcode;
                        int newGenBarcode = Convert.ToInt32(getBarcode) + 1;
                        string newBarcode = db.Database.SqlQuery<string>(string.Format("select dbo.getencode({0},'" + companyCode + "','" + branchCode + "')", newGenBarcode)).FirstOrDefault();
                        newBarcodeNo.Add(newBarcode);
                        KTTU_BARCODE_MASTER barcode = new KTTU_BARCODE_MASTER();
                        barcode.obj_id = SIGlobals.Globals.GetNewGUID();
                        barcode.company_code = t.CompanyCode;
                        barcode.branch_code = t.BranchCode;
                        barcode.barcode_no = newBarcode;
                        barcode.batch_no = t.BatchNo;
                        barcode.sal_code = t.SalCode;
                        barcode.operator_code = t.OperatorCode;
                        barcode.date = t.Date;
                        barcode.counter_code = t.CounterCode;
                        barcode.gs_code = t.GSCode;
                        barcode.item_name = t.ItemName;
                        barcode.gwt = t.Gwt;
                        barcode.swt = t.Swt;
                        barcode.nwt = t.Nwt;
                        barcode.grade = t.Grade;
                        barcode.catalog_id = t.CatalogID;
                        barcode.making_charge_per_rs = t.MakingChargePerRs;
                        barcode.wast_percent = t.WastePercent;
                        barcode.qty = t.Qty;
                        barcode.item_size = t.ItemSize;
                        barcode.design_no = t.DesignNo;
                        barcode.piece_rate = t.PieceRate;
                        barcode.daimond_amount = t.DiamondAmount;
                        barcode.stone_amount = t.StoneAmount;
                        barcode.order_no = t.OrderNo;
                        barcode.sold_flag = t.SoldFlag;
                        barcode.product_code = t.ProductCode;
                        barcode.hallmark_charges = t.HallmarkCharges;
                        barcode.remarks = t.Remarks;
                        barcode.supplier_code = t.SupplierCode;
                        barcode.ordered_company_code = t.OrderedCompanyCode;
                        barcode.ordered_branch_code = t.OrderedBranchCode;
                        barcode.karat = t.Karat;
                        barcode.mc_amount = t.McAmount;
                        barcode.wastage_grms = t.WastageGrams;
                        barcode.mc_percent = t.McPercent;
                        barcode.mc_type = t.McType;
                        barcode.old_barcode_no = t.OldBarcodeNo;
                        barcode.prod_ida = t.ProdIda;
                        barcode.prod_tagno = t.ProdTagno;
                        barcode.UpdateOn = SIGlobals.Globals.GetDateTime();
                        barcode.Lot_No = t.LotNo;
                        barcode.tag_wt = t.TagWt;
                        barcode.isConfirmed = t.IsConfirmed;
                        barcode.current_wt = t.CurrentWt;
                        barcode.MC_For = t.MCFor;
                        barcode.diamond_no = t.DiamondNo;
                        barcode.batch_id = t.BatchId;
                        barcode.add_wt = t.AddWt;
                        barcode.weightRead = t.WeightRead;
                        barcode.confirmedweightRead = t.ConfirmedweightRead;
                        barcode.party_name = t.PartyName;
                        barcode.design_name = t.DesignName;
                        barcode.item_size_name = t.ItemSizeName;
                        barcode.master_design_code = t.MasterDesignCode;
                        barcode.master_design_name = t.MasterDesignName;
                        barcode.vendor_model_no = t.VendorModelNo;
                        barcode.pur_mc_gram = t.PurMcGram;
                        barcode.mc_per_piece = t.McPerPiece;
                        barcode.Tagging_Type = t.TaggingType;
                        barcode.BReceiptNo = t.BReceiptNo;
                        barcode.BSNo = t.BSNo;
                        barcode.Issue_To = t.IssueTo;
                        barcode.pur_mc_amount = t.PurMcAmount;
                        barcode.pur_mc_type = t.PurMcType;
                        barcode.pur_rate = t.PurRate;
                        barcode.sr_batch_id = t.SRBatchID;
                        barcode.total_selling_mc = t.TotalSellingMC;
                        barcode.pur_diamond_amount = t.PurDiamondAmount;
                        barcode.total_purchase_mc = t.TotalPurchaseMc;
                        barcode.pur_stone_amount = t.PurStoneAmount;
                        barcode.pur_purity_percentage = t.PurPurityPercentage;
                        barcode.pur_wastage_type = t.PurWastageType;
                        barcode.pur_wastage_type_value = t.PurWastageTypeValue;
                        barcode.UniqRowID = Guid.NewGuid();
                        barcode.certification_no = t.CertificationNo;
                        barcode.ref_no = t.RefNo;
                        barcode.receipt_type = t.ReceiptType;
                        barcode.EntryDocType = t.EntryDocType;
                        barcode.EntryDocNo = t.EntryDocNo;
                        barcode.ExitDocType = t.ExitDocType;
                        barcode.ExitDocNo = t.ExitDocNo;
                        barcode.OnlineStock = t.OnlineStock;
                        barcode.is_shuffled = t.IsShuffled;
                        barcode.Collections = t.Collections;
                        barcode.barcode_no = newBarcode;
                        barcode.obj_id = SIGlobals.Globals.GetNewGUID();
                        barcode.confirmedDate = SIGlobals.Globals.GetDateTime();
                        barcode.confirmedBy = originalTag.OperatorCode;
                        barcode.shuffled_date = SIGlobals.Globals.GetDateTime();
                        barcode.EntryDate = SIGlobals.Globals.GetDateTime();
                        barcode.ExitDate = SIGlobals.Globals.GetDateTime();
                        db.KTTU_BARCODE_MASTER.Add(barcode);

                        int slno = 1;
                        foreach (SalesEstStoneVM s in t.lstOfStone) {
                            KTTU_BARCODE_STONE_DETAILS ses = new KTTU_BARCODE_STONE_DETAILS();
                            ses.obj_id = SIGlobals.Globals.GetNewGUID();
                            ses.company_code = companyCode;
                            ses.branch_code = branchCode;
                            ses.sl_no = slno;
                            ses.barcode_no = newBarcode;
                            ses.type = s.Type == "STN" ? "S" : "D";
                            ses.name = s.Name;
                            ses.qty = s.Qty;
                            ses.carrat = s.Carrat;
                            ses.rate = s.Rate;
                            ses.amount = s.Amount;
                            ses.carrat = s.Carrat;
                            ses.Fin_Year = s.FinYear;
                            ses.color = s.Color;
                            ses.clarity = s.Clarity;
                            ses.shape = s.Shape;
                            ses.cut = s.Cut;
                            ses.polish = s.Polish;
                            ses.symmetry = s.Symmetry;
                            ses.fluorescence = s.Fluorescence;
                            ses.certificate = s.Certificate;
                            ses.UniqRowID = Guid.NewGuid();
                            db.KTTU_BARCODE_STONE_DETAILS.Add(ses);
                            slno = slno + 1;
                        }

                        KSTS_BARCODE_SEQ_NOS kbsn = db.KSTS_BARCODE_SEQ_NOS.Where(bs => bs.company_code == companyCode
                                                                              && bs.branch_code == branchCode).FirstOrDefault();
                        kbsn.prev_barcode = Convert.ToString(Convert.ToInt32(kbsn.prev_barcode) + 1);
                        db.Entry(kbsn).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    #endregion

                    db.SaveChanges();
                    transaction.Commit();
                    return newBarcodeNo;
                }
                catch (Exception excp) {
                    transaction.Rollback();
                    error = new ErrorVM().GetErrorDetails(excp);
                };
                return null;
            }
        }
        #endregion
    }
}
