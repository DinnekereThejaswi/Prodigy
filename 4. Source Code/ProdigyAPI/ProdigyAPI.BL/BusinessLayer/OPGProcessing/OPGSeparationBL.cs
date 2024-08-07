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

namespace ProdigyAPI.BL.BusinessLayer.OPGProcessing
{
    public class OPGSeparationBL
    {
        MagnaDbEntities db = null;

        public OPGSeparationBL()
        {
            db = new MagnaDbEntities(true);
        }

        public OPGSeparationBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }

        public OPGSeparationOutputVM GetOPGDetails(OPGSeparationInputVM OPGSeparationInput, out ErrorVM error)
        {
            error = null;
            if (OPGSeparationInput == null) {
                error = new ErrorVM { description = "Nothing to query.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return null;
            }
            var endDate = OPGSeparationInput.ToDate.Date.AddSeconds(59).AddSeconds(59).AddHours(23);
            OPGSeparationOutputVM oPGSeparationOutput = new OPGSeparationOutputVM();
            var opgItems = db.usp_OPG_StockIssue_details_Branch(OPGSeparationInput.CompanyCode, OPGSeparationInput.BranchCode,
                OPGSeparationInput.FromDate.Date, endDate, OPGSeparationInput.GSCode).ToList();
            if (opgItems == null || opgItems.Count <= 0) {
                error = new ErrorVM { description = "No OPG item details found.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return null;
            }

            var opgStones = db.usp_OPG_StockIssue_stone_details_Branch(OPGSeparationInput.CompanyCode, OPGSeparationInput.BranchCode,
                OPGSeparationInput.FromDate, endDate, OPGSeparationInput.GSCode).ToList();
            oPGSeparationOutput.CompanyCode = OPGSeparationInput.CompanyCode;
            oPGSeparationOutput.BranchCode = OPGSeparationInput.BranchCode;

            oPGSeparationOutput.LineDetails = new List<OPGSeparationLineVM>();
            decimal totalDcts = 0;
            foreach (var x in opgItems) {
                OPGSeparationLineVM LineItem = new OPGSeparationLineVM
                {
                    BillNo = x.bill_no,
                    SlNo = x.sl_no,
                    GSCode = x.gs_code,
                    ItemCode = x.item_name,
                    Qty = Convert.ToInt32(x.item_no),
                    GrossWt = Convert.ToDecimal(x.gwt),
                    StoneWt = x.swt,
                    NetWt = Convert.ToDecimal(x.nwt),
                    MeltingPercent = Convert.ToDecimal(x.melting_percent),
                    MeltingLossWeight = Convert.ToDecimal(x.melting_loss),
                    PurchaseRate = Convert.ToDecimal(x.purchase_rate),
                    DiamondAmount = Convert.ToDecimal(x.diamond_amount),
                    GoldAmount = Convert.ToDecimal(x.gold_amount),
                    LineAmount = Convert.ToDecimal(x.item_amount),
                    StoneDetails = new List<OPGSeparationStoneDetailVM>()
                };

                if (opgStones != null) {
                    var itemStones = opgStones.Where(m => m.bill_no == x.bill_no && m.item_sno == x.sl_no).ToList();
                    if (itemStones != null) {
                        foreach (var its in itemStones) {
                            OPGSeparationStoneDetailVM _sd = new OPGSeparationStoneDetailVM
                            {
                                SlNo = its.sno,
                                GS = its.gs_code,
                                Name = its.name,
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
                oPGSeparationOutput.LineDetails.Add(LineItem);
            }

            oPGSeparationOutput.Qty = oPGSeparationOutput.LineDetails.Sum(p => p.Qty);
            oPGSeparationOutput.GrossWt = oPGSeparationOutput.LineDetails.Sum(p => p.GrossWt);
            oPGSeparationOutput.StoneWt = oPGSeparationOutput.LineDetails.Sum(p => p.StoneWt);
            oPGSeparationOutput.NetWt = oPGSeparationOutput.LineDetails.Sum(p => p.NetWt);
            oPGSeparationOutput.Dcts = totalDcts;
            oPGSeparationOutput.ToleranceWt = 0;

            #region Tolerance to be filled
            var toleranceInfo = db.KSTU_TOLERANCE_MASTER.Where(x => x.company_code == OPGSeparationInput.CompanyCode
                && x.branch_code == OPGSeparationInput.BranchCode && x.obj_id == 50).FirstOrDefault();
            if (toleranceInfo != null)
                oPGSeparationOutput.ToleranceWt = toleranceInfo.Max_Val;
            #endregion

            return oPGSeparationOutput;
        }

        public List<ListOfValue> GetMetalGS(string companyCode, string branchCode)
        {
            var metalGS = (from gsEntry in db.KSTS_GS_ITEM_ENTRY
                           join gsGrp in db.KSTU_GS_GROUPING
                           on new { CC = gsEntry.company_code, BC = gsEntry.branch_code, GS = gsEntry.gs_code }
                           equals new { CC = gsGrp.company_code, BC = gsGrp.branch_code, GS = gsGrp.gs_code }
                           where gsEntry.object_status != "C" && gsGrp.obj_status != "C"
                            && gsEntry.company_code == companyCode && gsEntry.branch_code == branchCode
                            && gsGrp.ir_code == "IO"
                           select new ListOfValue
                           {
                               Code = gsEntry.gs_code,
                               Name = gsEntry.item_level1_name
                           }).ToList();
            return metalGS;
        }

        public bool SaveOGSeparation(OPGSeparationInputVM OPGSeparationInput, string userID, out int segregationNo, out ErrorVM error)
        {
            error = null;
            segregationNo = 0;
            try {
                #region Basic Validations
                if (OPGSeparationInput == null) {
                    error = new ErrorVM { description = "Nothing to save.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return false;
                }
                if (string.IsNullOrEmpty(OPGSeparationInput.SalesmanCode)) {
                    error = new ErrorVM { description = "Saleman code is required.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return false;
                }
                if (OPGSeparationInput.OPGSeparationInputLines == null || OPGSeparationInput.OPGSeparationInputLines.Count <= 0) {
                    error = new ErrorVM { description = "There is no line detail to save.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return false;
                }
                #endregion

                #region Tolerance Check
                decimal separatedGrossWt = Convert.ToDecimal(OPGSeparationInput.OPGSeparationInputLines.Sum(x => x.GrossWeight));
                decimal separatedStoneWt = Convert.ToDecimal(OPGSeparationInput.OPGSeparationInputLines.Sum(x => x.StoneWeight));
                decimal separatedDcts = Convert.ToDecimal(OPGSeparationInput.OPGSeparationInputLines.Sum(x => x.DiamondCaretWeight));

                OPGSeparationOutputVM opgSeparationOutput = GetOPGDetails(OPGSeparationInput, out error);
                if (error != null) {
                    return false;
                }

                if (opgSeparationOutput.LineDetails == null || opgSeparationOutput.LineDetails.Count <= 0) {
                    error = new ErrorVM { description = "OPG invoice line detail is not found.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return false;

                }
                decimal oldGoldGrossWt = Convert.ToDecimal(opgSeparationOutput.LineDetails.Sum(x => x.GrossWt));
                decimal oldGoldStoneWt = Convert.ToDecimal(opgSeparationOutput.LineDetails.Sum(x => x.StoneWt));
                decimal oldGoldDcts = 0;
                foreach (var line in opgSeparationOutput.LineDetails) {
                    if (line.StoneDetails != null) {
                        var dcts = line.StoneDetails.Where(y => y.GS.Contains("D")).Sum(z => z.Carrat);
                        oldGoldDcts = oldGoldDcts + Convert.ToDecimal(dcts);
                    }
                }

                //If tolerance weight is Zero, set it to 0.50
                if (opgSeparationOutput.ToleranceWt <= 0)
                    opgSeparationOutput.ToleranceWt = 0.50M;
                //For Gross Weight
                if (Math.Abs(oldGoldGrossWt - separatedGrossWt) > opgSeparationOutput.ToleranceWt) {
                    error = new ErrorVM
                    {
                        description = string.Format("The maximum tolerance allowed for Separated Gross weight is {0} grams, but it is {1}.",
                        opgSeparationOutput.ToleranceWt, Math.Abs(oldGoldGrossWt - separatedGrossWt)),
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
                //for Stone Weight - is it required?
                if (Math.Abs(oldGoldStoneWt - separatedStoneWt) > opgSeparationOutput.ToleranceWt) {
                    error = new ErrorVM
                    {
                        description = string.Format("The maximum tolerance allowed for Separated Stone weight is {0} grams, but it is {1}.",
                        opgSeparationOutput.ToleranceWt, Math.Abs(oldGoldStoneWt - separatedStoneWt)),
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
                //for Dcts - is it required?
                if (Math.Abs(oldGoldDcts - separatedDcts) > opgSeparationOutput.ToleranceWt) {
                    error = new ErrorVM
                    {
                        description = string.Format("The maximum tolerance allowed for Separated Diamond Caret weight is {0} grams, but it is {1}.",
                        opgSeparationOutput.ToleranceWt, Math.Abs(oldGoldDcts - separatedDcts)),
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
                #endregion

                #region 1. Post to Segregation Master & Segregation Detail Tables
                #region -1.1 OPG Segregation Master
                string companyCode = OPGSeparationInput.CompanyCode;
                string branchCode = OPGSeparationInput.BranchCode;
                var applicationDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                var updatedTimestamp = SIGlobals.Globals.GetDateTime();

                var storeLocationId = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode
                    && c.branch_code == branchCode).FirstOrDefault().store_location_id;
                segregationNo = SIGlobals.Globals.GetDocumentNo(db, companyCode, branchCode, "103", true);
                string masterObjectID = SIGlobals.Globals.GetMagnaGUID("KTTU_OPG_STOCK_SEGGREGATED_MASTER", segregationNo, companyCode, branchCode);
                KTTU_OPG_STOCK_SEGGREGATED_MASTER segMaster = new KTTU_OPG_STOCK_SEGGREGATED_MASTER
                {
                    company_code = companyCode,
                    branch_code = branchCode,
                    gs_code = OPGSeparationInput.GSCode,
                    cancelled_by = "",
                    cancelled_remarks = "",
                    cflag = "N",
                    obj_id = masterObjectID,
                    operator_code = userID,
                    seggregated_by = OPGSeparationInput.SalesmanCode,
                    seggregated_date = applicationDate,
                    seggregation_no = segregationNo,
                    ShiftID = 0,
                    total_gwt = opgSeparationOutput.GrossWt,
                    total_qty = opgSeparationOutput.Qty,
                    total_swt = opgSeparationOutput.StoneWt,
                    total_nwt = opgSeparationOutput.NetWt,
                    total_amount = 0,
                    UniqRowID = Guid.NewGuid(),
                    UpdateOn = updatedTimestamp,
                    New_Bill_No = segregationNo.ToString()
                };
                db.KTTU_OPG_STOCK_SEGGREGATED_MASTER.Add(segMaster);
                #endregion

                #region -1.2 OPG Segregation Detail
                var finYear = Globals.GetFinancialYear(db, companyCode, branchCode);
                foreach (var ln in opgSeparationOutput.LineDetails) {
                    KTTU_OPG_STOCK_SEGGREGATED_DETAILS segDetail = new KTTU_OPG_STOCK_SEGGREGATED_DETAILS
                    {
                        obj_id = masterObjectID,
                        company_code = companyCode,
                        branch_code = branchCode,
                        seggregation_no = segregationNo,
                        bill_no = ln.BillNo,
                        sl_no = ln.SlNo,
                        gs_code = ln.GSCode,
                        item_name = ln.ItemCode,
                        item_no = ln.Qty,
                        gwt = ln.GrossWt,
                        swt = ln.StoneWt,
                        nwt = ln.NetWt,
                        melting_percent = ln.MeltingPercent,
                        melting_loss = ln.MeltingLossWeight,
                        purchase_rate = ln.PurchaseRate,
                        diamond_amount = ln.DiamondAmount,
                        gold_amount = ln.GoldAmount,
                        item_amount = ln.LineAmount,
                        Fin_Year = finYear,
                        purity_per = 91.60M,//TODO: Check this out.
                        UniqRowID = Guid.NewGuid()
                    };
                    db.KTTU_OPG_STOCK_SEGGREGATED_DETAILS.Add(segDetail);
                }
                #endregion

                #region 1.3 Increment Serial No.143

                SIGlobals.Globals.IncrementDocumentNo(db, companyCode, branchCode, "103");
                #endregion 
                #endregion

                #region 2. Post to Issue Master & Issue Detail Records

                #region -2.1 Issue Header
                updatedTimestamp = SIGlobals.Globals.GetDateTime();
                int issueNo = SIGlobals.Globals.GetDocumentNo(db, companyCode, branchCode, "14", true);
                string issueMasterObjectID = SIGlobals.Globals.GetMagnaGUID("KTTU_ISSUE_MASTER", issueNo, companyCode, branchCode);
                KTTU_ISSUE_MASTER im = new KTTU_ISSUE_MASTER
                {
                    obj_id = issueMasterObjectID,
                    company_code = companyCode,
                    branch_code = branchCode,
                    issue_no = issueNo,
                    issue_date = applicationDate,
                    sal_code = OPGSeparationInput.SalesmanCode,
                    operator_code = userID,
                    gs_type = "",
                    party_name = branchCode,
                    issue_type = "IO",
                    obj_status = "O",
                    cflag = "N",
                    cancelled_by = "",
                    type = "IO",
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
                    received_from = "",
                    Version = 0,
                    stk_type = "N",
                    old_issue_no = null,
                    import_data_id = null,
                    import_content = null,
                    UniqRowID = Guid.NewGuid(),
                    U_obj_id = null,
                    isReceived = "N",
                    expected_pure_wt = 0,
                    new_no = null,
                    store_location_id = storeLocationId
                };
                db.KTTU_ISSUE_MASTER.Add(im);
                #endregion

                #region -2.2 Issue Detail
                int slNo = 1;
                foreach (var ld in OPGSeparationInput.OPGSeparationInputLines) {
                    updatedTimestamp = SIGlobals.Globals.GetDateTime();

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
                        gwt = ld.GrossWeight,
                        swt = ld.StoneWeight,
                        nwt = ld.GrossWeight - ld.StoneWeight,
                        alloy = 0,
                        receipt_gs_desc = "",
                        purity = ld.PurityPercent,
                        std_weight = 0,
                        wastage = 0,
                        UpdateOn = updatedTimestamp,
                        oldIssueNo = null,
                        gs_code = ld.GSCode,
                        batch_id = "",
                        opg_receipt_no = 0,
                        counter_code = "",
                        no_tags = 0,
                        tag_wt = 0,
                        dcts = ld.DiamondCaretWeight,
                        design_code = "",
                        batch_name = "",
                        barcode_no = "",
                        ref_no = 0,
                        item_value = 0,
                        Fin_Year = finYear,
                        receipt_no = 0,
                        total_wt = ld.GrossWeight,
                        wastageInActualWeight = 0,
                        Rate = 0,
                        Version = 0,
                        mc_amount = 0,
                        Amount = 0,
                        mc_for = "",
                        GSTGroupCode = "",
                        SGST_Percent = 0,
                        SGST_Amount = 0,
                        CGST_Percent = 0,
                        CGST_Amount = 0,
                        IGST_Percent = 0,
                        IGST_Amount = 0,
                        HSN = null,
                        IsScrap = null,
                        UniqRowID = Guid.NewGuid(),
                        TID = null,
                        U_obj_id = null,
                        isReceived = "N",
                        kova_wt = 0,
                        stn_gs_code = ld.StoneGSCode,
                        dm_gs_code = ld.DiamondGSCode
                    };
                    db.KTTU_ISSUE_DETAILS.Add(id);
                    slNo++;
                }
                #endregion

                #region -2.3 Post GS Issue Stock
                bool stockUpdated = UpdateOPGGSStock(companyCode, branchCode, db, opgSeparationOutput.LineDetails, segregationNo, postReverse: false, stockTransactionType: StockTransactionType.Issue, error: out error);
                if (!stockUpdated) {
                    return false;
                }
                #endregion

                #region -2.4 Increment issue series No.
                SIGlobals.Globals.IncrementDocumentNo(db, companyCode, branchCode, "14");
                #endregion

                #endregion

                #region 3. Post Receipt Master and Receipt Detail Records

                #region -3.1 Receipt Master
                var receiptNo = SIGlobals.Globals.GetDocumentNo(db, companyCode, branchCode, "15", true);
                string receiptMasterObjectID = SIGlobals.Globals.GetMagnaGUID("KTTU_RECEIPTS_MASTER", receiptNo, companyCode, branchCode);
                KTTU_RECEIPTS_MASTER rm = new KTTU_RECEIPTS_MASTER
                {
                    obj_id = receiptMasterObjectID,
                    company_code = companyCode,
                    branch_code = branchCode,
                    receipt_no = receiptNo,
                    receipt_date = applicationDate,
                    gs_type = OPGSeparationInput.GSCode,
                    sal_code = OPGSeparationInput.SalesmanCode,
                    operator_code = userID,
                    receipt_type = "RO",
                    Ref_no = segregationNo.ToString(),
                    cflag = "N",
                    cancelled_by = "",
                    grand_total = 0,
                    charges_acc_code = 0,
                    type = "CPC",
                    UpdateOn = updatedTimestamp,
                    remarks = "",
                    issue_no = issueNo,
                    party_name = branchCode,
                    no_of_tags = 0,
                    tag_weight = 0,
                    batch_id = "",
                    lot_ref_id = null,
                    DC_No = "",
                    DC_Date = applicationDate.Date,
                    Inv_Date = applicationDate.Date,
                    Is_hallmarked = "Y",
                    hallmarked_by = "",
                    inv_no = "",
                    cancelled_remarks = "",
                    ShiftID = 0,
                    New_Bill_No = "",
                    Ref_Receipt_No = receiptNo.ToString(),
                    isFixed = null,
                    new_receipt_no = receiptNo.ToString(),
                    adv_type = "R",
                    Invoice_Type_Id = 0,
                    C_Form = "N",
                    TDSPerc = 0,
                    Net_Amount = 0,
                    TDS_Amount = 0,
                    version = 0,
                    stk_type = null,
                    old_receipt_no = null,
                    import_data_id = null,
                    import_content = null,
                    final_amount = 0,
                    GSTGroupCode = null,
                    SGST_Percent = 0,
                    SGST_Amount = 0,
                    CGST_Percent = 0,
                    CGST_Amount = 0,
                    IGST_Percent = 0,
                    IGST_Amount = 0,
                    HSN = null,
                    round_off = 0,
                    pan_no = "",
                    tin_no = "",
                    UniqRowID = Guid.NewGuid(),
                    isReceived = "N",
                    expected_pure_wt = 0,
                    isConfirm = "N",
                    fin_year = finYear,
                    isDealer = "Y",
                    isNwtBased = "Y",
                    isCaratBased = "Y",
                    TCS_Percent = 0,
                    TCS_Amount = 0,
                    store_location_id = storeLocationId
                };
                db.KTTU_RECEIPTS_MASTER.Add(rm);
                #endregion

                #region -3.2 Receipt Detail Table
                decimal averageRate = 0;
                averageRate = opgSeparationOutput.LineDetails.Sum(segLin => segLin.PurchaseRate * segLin.NetWt) /
                    opgSeparationOutput.LineDetails.Sum(segLin => segLin.NetWt);

                int recLineSlNo = 1;
                foreach (var rl in OPGSeparationInput.OPGSeparationInputLines) {
                    string batchID = rm.party_name
                       + "/" + rm.Ref_no
                       + "/" + rm.receipt_date.ToString("dd-MM-yyyy")
                       + "/" + rl.GSCode
                       + "/" + recLineSlNo.ToString();
                    updatedTimestamp = SIGlobals.Globals.GetDateTime();
                    KTTU_RECEIPTS_DETAILS rd = new KTTU_RECEIPTS_DETAILS
                    {
                        obj_id = receiptMasterObjectID,
                        company_code = companyCode,
                        branch_code = branchCode,
                        receipt_no = receiptNo,
                        sl_no = recLineSlNo,
                        item_name = rl.GSCode,
                        code = "0",
                        counter_code = "",
                        item_no = 0,
                        gwt = rl.GrossWeight,
                        swt = rl.StoneWeight,
                        nwt = rl.GrossWeight,
                        wastage = 0,
                        item_amount = 0,
                        std_weight = rl.PureWeight,
                        purity = rl.PurityPercent,
                        UpdateOn = updatedTimestamp,
                        oldReceiptNo = null,
                        gs_code = "OGO",
                        batch_id = batchID,
                        no_tags = 0,
                        tag_wt = 0,
                        dcts = 0,
                        design_code = "",
                        result = "",
                        KOVA_wt = 0,
                        batch_name = "",
                        supp_dcts = 0,
                        hLoss = 0,
                        supp_swt = 0,
                        Fin_Year = finYear,
                        barcode_no = "",
                        dcts_amount = 0,
                        sales_no = 0,
                        adj_date = updatedTimestamp,
                        mc_type = 0,
                        mc_per_gram = 0,
                        mc_amount = 0,
                        tax_amount = 0,
                        item_description = "",
                        stone_amount = 0,
                        piece_rate = 0,
                        type = "R",
                        isHallMarked = "",
                        wastage_type_id = 0,
                        wastage_type_value = 0,
                        wastageInActualWeight = 0,
                        isScrap = "N",
                        Rate = averageRate,
                        Hallmark_charges = 0,
                        certification_charges = 0,
                        other_charges = 0,
                        version = 0,
                        mc_for = null,
                        GSTGroupCode = null,
                        SGST_Percent = 0,
                        SGST_Amount = 0,
                        CGST_Percent = 0,
                        CGST_Amount = 0,
                        IGST_Percent = 0,
                        IGST_Amount = 0,
                        HSN = "",
                        //TID = 0,//identity column
                        UniqRowID = Guid.NewGuid(),
                        wastagedcts = 0,
                        isReceived = "N",
                        isDiamondCalculationFlag = "",
                        mc_percent = 0,
                        mc_per_piece = 0,
                        amount = 0,
                        AddWt = 0,
                        DeductWt = 0,
                        stn_gs_code = rl.StoneGSCode,
                        dm_gs_code = rl.DiamondGSCode,
                    };
                    recLineSlNo++;
                    db.KTTU_RECEIPTS_DETAILS.Add(rd);
                }
                #endregion

                #region -3.3 Increment Receipt Series No.
                SIGlobals.Globals.IncrementDocumentNo(db, companyCode, branchCode, "15"); 
                #endregion

                #region -3.4 Post Receipt Stock
                stockUpdated = UpdateOPGGSStock(companyCode, branchCode, db, opgSeparationOutput.LineDetails, segregationNo, postReverse: false, stockTransactionType: StockTransactionType.Receipt, error: out error);
                if (!stockUpdated) {
                    return false;
                }
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

        public bool UpdateOPGGSStock(string companyCode, string branchCode, MagnaDbEntities dbContext, List<OPGSeparationLineVM> segLines, int documentNo, bool postReverse, StockTransactionType stockTransactionType, out ErrorVM error)
        {
            error = null;
            #region Post GS Stock
            var generalStockJournal =
                       from sl in segLines
                       group sl by new
                       {
                           CompanyCode = companyCode,
                           BranchCode = branchCode,
                           GS = sl.GSCode,
                           Counter = "M",
                           Item = sl.ItemCode
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
                var issueView = (from im in db.KTTU_ISSUE_MASTER
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
                                 where im.issue_no == issueNo && im.company_code == companyCode && im.branch_code == branchCode
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

                var issueDetails = SIGlobals.Globals.ExecuteQuery("SELECT units, " +
                                                                "       iD.gs_code AS item_name, " +
                                                                "       gwt," +
                                                                "       alloy," +
                                                                "       std_weight," +
                                                                "       swt," +
                                                                "       nwt," +
                                                                "       dcts," +
                                                                "       batch_id," +
                                                                "       sl_no," +
                                                                "       ISNULL(dcts, 0) AS[carrat]," +
                                                                "       id.gs_code," +
                                                                "       item_value," +
                                                                "       kova_wt," +
                                                                "       ISNULL(SGST_Amount, 0) AS SGST_Amount," +
                                                                "       ISNULL(CGST_Amount, 0) AS CGST_Amount," +
                                                                "       ISNULL(IGST_Amount, 0) AS IGST_Amount," +
                                                                "       ISNULL(SGST_Percent, 0) AS SGST_Percent," +
                                                                "       ISNULL(CGST_Percent, 0) AS CGST_Percent," +
                                                                "       ISNULL(IGST_Percent, 0) AS IGST_Percent," +
                                                                " (" +
                                                                " SELECT DISTINCT" +
                                                                "       HSN" +
                                                                " FROM item_master IA" +
                                                                " WHERE ID.gs_code = IA.gs_code" +
                                                                "      AND company_code = 'BH'" +
                                                                "      AND branch_code = 'JNR'" +
                                                                " ) AS hsn " +


                                                                " FROM KTTU_ISSUE_DETAILS ID" +
                                                                " LEFT JOIN dbo.ITEM_MASTER im ON id.gs_code = im.gs_code" +
                                                                " WHERE issue_no = " + issueNo + "" +
                                                                "      AND id.company_code = '" + companyCode + "'" +
                                                                "      AND id.branch_code = '" + branchCode + "'" +
                                                                " ORDER BY sl_no; ");

                KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode && c.branch_code == branchCode).FirstOrDefault();
                KSTU_SUPPLIER_MASTER supplier = SIGlobals.Globals.GetSupplier(db, companyCode, branchCode, issueView.party_code);
                KSTS_STATE_MASTER state = db.KSTS_STATE_MASTER.Where(st => st.state_name == supplier.state).FirstOrDefault();
                string stateCode = Convert.ToString(state.tinno);

                decimal Grosswt = 0, Netwt = 0, dcts = 0, StnWt = 0, Totatlwt = 0, PureWt = 0, Value = 0, Qty = 0, itemvalue = 0, kovawt = 0;
                decimal TotalSGST = 0, TotalCGST = 0, TotalIGST = 0;

                object Itemval = issueDetails.Compute("Sum(item_value)", "");
                if (Itemval != null && Itemval != DBNull.Value)
                    Value = Convert.ToDecimal(Itemval);

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
                sb.AppendLine(string.Format("<TD style=\" border-bottom:thin;\" colspan = 11 ALIGN = \"left\"><b>GSTIN :{0}</b></TD>", company.tin_no));
                sb.AppendLine("</TR>");
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=11 ALIGN = \"CENTER\"><b>{0}</b></TD>", company.company_name));
                sb.AppendLine("</TR>");
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=11 ALIGN = \"CENTER\"><b>{0},{1},{2}-{3}</b></TD>", company.address1, company.address2, company.address3, company.pin_code));
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
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" ></td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >Ref No &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + issueView.issue_no + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >Issue Date &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + issueView.issue_date.Date + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" > Place of supply &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + supplier.state + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" >Issued By &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + issueView.sal_name + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" >Issue Type &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + "Purification Issue" + "</td>");
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
                sb.AppendLine(string.Format("<TD style=\" border-bottom:thin;\" colspan = 7  ALIGN = \"CENTER\"><b>DELIVERY CHALLAN</b></TD>"));
                sb.AppendLine("</TR>");

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=7 ALIGN = \"CENTER\"> ISSUED TO JOB WORKER(NOT AS SUPPLY) </TD>"));
                sb.AppendLine("</TR>");

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD  style=\"border-right:thin\" colspan=7 ALIGN = \"CENTER\">ORIGINAL / DUPLICATE<br></TD>"));
                sb.AppendLine("</TR>");

                sb.AppendLine("</table>");
                sb.AppendLine("</td>");

                sb.AppendLine("<Table font-size=14pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse;\" width=\"800\">");  //FRAME=BOX RULES=NONE

                sb.AppendLine("<TR bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sb.AppendLine("<th style=\"border-top:none; \" align = \"center\">S.No</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">Description</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">HSN</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">Batch ID</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">Gr.Wt(g)</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">N.Wt(g)</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">Dcts</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">Kova.Wt</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">TaxableValue</th>");
                sb.AppendLine("</TR>");

                for (int i = 0; i < issueDetails.Rows.Count; i++) {
                    sb.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"CENTER\">{0}{1}</TD>", issueDetails.Rows[i]["sl_no"].ToString(), "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"CENTER\">{0}{1}</TD>", SIGlobals.Globals.GetBranchGSName(db, companyCode, branchCode, issueDetails.Rows[i]["item_name"].ToString()), "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"CENTER\">{0}{1}</TD>", issueDetails.Rows[i]["HSN"].ToString(), "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"CENTER\">{0}{1}</TD>", issueDetails.Rows[i]["batch_id"].ToString(), "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", issueDetails.Rows[i]["gwt"].ToString(), "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", issueDetails.Rows[i]["nwt"].ToString(), "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", issueDetails.Rows[i]["dcts"].ToString(), "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", issueDetails.Rows[i]["kova_wt"].ToString(), "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", issueDetails.Rows[i]["item_value"].ToString(), "&nbsp"));
                    sb.AppendLine("</TR>");
                    Grosswt += Convert.ToDecimal(issueDetails.Rows[i]["gwt"]);
                    StnWt += Convert.ToDecimal(issueDetails.Rows[i]["swt"]);
                    Netwt += Convert.ToDecimal(issueDetails.Rows[i]["nwt"]);
                    dcts += Convert.ToDecimal(issueDetails.Rows[i]["dcts"]);
                    kovawt += Convert.ToDecimal(issueDetails.Rows[i]["kova_wt"]);
                    itemvalue += Convert.ToDecimal(issueDetails.Rows[i]["item_value"]);
                    PureWt += Convert.ToDecimal(issueDetails.Rows[i]["std_weight"]);
                    TotalIGST += Convert.ToDecimal(issueDetails.Rows[i]["IGST_Amount"]);
                }
                for (int i = 0; i < 8 - issueDetails.Rows.Count - 1; i++) {
                    if (i == 0) {
                        sb.AppendLine("<TR>");
                        sb.AppendLine(string.Format("<TD  style=\"border-left: thin ; border-top: thin soild; border-bottom: thin ; border-right: thin \";  style=font-size=\"14pt\"  colspan = {1}>{0}</TD>", "&nbsp", issueDetails.Columns.Count));
                        sb.AppendLine("</TR>");
                    }
                    else {
                        sb.AppendLine("<TR>");
                        sb.AppendLine(string.Format("<TD  style=\"border-left: thin ; border-top: thin ; border-bottom: thin; border-right: thin \"; style=font-size=\"14pt\" colspan = {1}>{0}</TD>", "&nbsp", issueDetails.Columns.Count));
                        sb.AppendLine("</TR>");
                    }
                }
                sb.AppendLine("<TR  style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" colspan = 4 ALIGN = \"left\"><b>TOTAL </b></TD><TD  ALIGN = \"RIGHT\" ><b>{0}</b></TD> <TD  ALIGN = \"RIGHT\" ><b>{1}</b></TD><TD  ALIGN = \"RIGHT\" ><b>{2}</b></TD><TD  ALIGN = \"RIGHT\" ><b>{3}</b></TD><TD  ALIGN = \"RIGHT\" ><b>{4}</b></TD>",
                           Grosswt, Netwt, dcts, kovawt, itemvalue));
                sb.AppendLine("</TR>");

                if (TotalIGST > 0) {
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD  colspan=7 ALIGN = \"right\"><b>IGST Amt @ {0} % : </b></TD>", issueDetails.Rows[0]["IGST_Percent"].ToString()));
                    sb.AppendLine(string.Format("<TD  style=\"border-right:thin\" ALIGN = \"right\"><b>{0}</b></TD>", TotalIGST));
                    sb.AppendLine("</TR>");
                }
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD  colspan=8 ALIGN = \"right\"><b>{0}  : </b></TD>", "Invoice value "));
                sb.AppendLine(string.Format("<TD  style=\"border-right:thin\" ALIGN = \"right\"><b>{0}</b></TD>", TotalIGST + itemvalue));
                sb.AppendLine("</TR>");

                decimal Inword = itemvalue + TotalIGST;
                string strWords = string.Empty;
                strWords = SIGlobals.Globals.ConvertNumbertoWordsinRupees(Inword);
                if (!string.IsNullOrEmpty(issueView.remarks)) {
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD style=\"border-right:thin \"; colspan=9 ALIGN = \"left\">{0}{1}{1}</TD>", "Remarks  :" + issueView.remarks, "&nbsp"));
                    sb.AppendLine("</TR>");
                }
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-right:thin \"; colspan=9 ALIGN = \"left\">{0}{1}{1}</TD>", strWords, "&nbsp"));
                sb.AppendLine("</TR>");
                sb.AppendLine("<TR>");
                sb.AppendLine("<tr>");
                sb.AppendLine(string.Format("<td  colspan=5  align = \"left\"  style=\"border-right:thin\" ><b>{0}&nbsp<br><br><br>{1}&nbsp</b></td>", "&nbsp", "Signature of Consignee "));
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
