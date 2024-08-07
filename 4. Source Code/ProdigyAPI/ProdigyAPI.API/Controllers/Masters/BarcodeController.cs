using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Sales;
using ProdigyAPI.BL.ViewModel.Stock;
using ProdigyAPI.Framework;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData.Query;
using System.Web.Http.Results;

namespace ProdigyAPI.Controllers.Masters
{
    /// <summary>
    /// Barcode controller provides API's for barcode related information.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Masters/Barcode")]
    public class BarcodeController : SIBaseApiController<CustomerMasterVM>, IBaseMasterActionController<SalesEstStoneVM, SalesEstStoneVM>
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Controller Methods
        /// <summary>
        /// Get Barcode information by barcode no (without stone details).
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="barcodeNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetBarcodeInfo/{companyCode}/{branchCode}/{barcodeNo}")]
        [Route("GetBarcodeInfo")]
        public IHttpActionResult GetBarcodeDetWithCalculation(string companyCode, string branchCode, string barcodeNo)
        {
            try {
                // Tried with this bellow procedure but throwing Error which is not cathable.
                //int validate = db.usp_ValidateBarcodeNo(barcodeNo, 0, Common.CompanyCode, Common.BranchCode);

                //Validation
                string message = ValidateBarcodeDetails(barcodeNo, companyCode, branchCode);
                if (message != "") {
                    return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = message, ErrorStatusCode = HttpStatusCode.BadRequest });
                }

                // If Valid
                ObjectParameter errorMsg = new ObjectParameter("errorMsg", typeof(string));
                var barcodeInfo = db.usp_getBarcodeInformation(companyCode, branchCode, barcodeNo, "0", 0, false, 0, 0, errorMsg).ToArray();
                if (errorMsg.Value.ToString() != "") {
                    return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = errorMsg.ToString(), ErrorStatusCode = HttpStatusCode.BadRequest });
                }
                return Ok(new SalesEstDetailsVM()
                {
                    ObjID = barcodeInfo.FirstOrDefault().obj_id,
                    CompanyCode = barcodeInfo.FirstOrDefault().company_code,
                    BranchCode = barcodeInfo.FirstOrDefault().branch_code,
                    EstNo = barcodeInfo.FirstOrDefault().est_no,
                    SlNo = barcodeInfo.FirstOrDefault().sl_no,
                    BillNo = barcodeInfo.FirstOrDefault().bill_no,
                    BarcodeNo = barcodeInfo.FirstOrDefault().barcode_no,
                    SalCode = Convert.ToString(barcodeInfo.FirstOrDefault().Sal_Code),
                    CounterCode = barcodeInfo.FirstOrDefault().counter_code,
                    ItemName = barcodeInfo.FirstOrDefault().item_name,
                    ItemQty = barcodeInfo.FirstOrDefault().qty,
                    Grosswt = Convert.ToDecimal(barcodeInfo.FirstOrDefault().gwt),
                    Stonewt = Convert.ToDecimal(barcodeInfo.FirstOrDefault().swt),
                    Netwt = Convert.ToDecimal(barcodeInfo.FirstOrDefault().nwt),
                    AddWt = barcodeInfo.FirstOrDefault().AddWt,
                    DeductWt = barcodeInfo.FirstOrDefault().DedWt,
                    MakingChargePerRs = Convert.ToDecimal(barcodeInfo.FirstOrDefault().making_charge_per_rs),
                    WastPercent = Convert.ToDecimal(barcodeInfo.FirstOrDefault().wast_percent),
                    GoldValue = Convert.ToDecimal(barcodeInfo.FirstOrDefault().gold_value),
                    VaAmount = Convert.ToDecimal(barcodeInfo.FirstOrDefault().va_amount),
                    StoneCharges = Convert.ToDecimal(barcodeInfo.FirstOrDefault().stone_charges),
                    DiamondCharges = barcodeInfo.FirstOrDefault().diamond_charges,
                    TotalAmount = Convert.ToDecimal(barcodeInfo.FirstOrDefault().total_amount),
                    Hallmarkarges = barcodeInfo.FirstOrDefault().hallmarcharges,
                    McAmount = barcodeInfo.FirstOrDefault().mc_amount,
                    WastageGrms = barcodeInfo.FirstOrDefault().wastage_grms,
                    McPercent = barcodeInfo.FirstOrDefault().mc_percent,
                    AddQty = barcodeInfo.FirstOrDefault().Addqty,
                    DeductQty = barcodeInfo.FirstOrDefault().DedQty,
                    OfferValue = barcodeInfo.FirstOrDefault().offer_value,
                    UpdateOn = barcodeInfo.FirstOrDefault().UpdateOn,
                    GsCode = barcodeInfo.FirstOrDefault().gs_code,
                    Rate = barcodeInfo.FirstOrDefault().rate,
                    Karat = barcodeInfo.FirstOrDefault().karat,
                    AdBarcode = barcodeInfo.FirstOrDefault().ad_barcode,
                    AdCounter = barcodeInfo.FirstOrDefault().ad_counter,
                    AdItem = barcodeInfo.FirstOrDefault().ad_item,
                    IsEDApplicable = barcodeInfo.FirstOrDefault().isEDApplicable,
                    McType = Convert.ToInt32(barcodeInfo.FirstOrDefault().mc_type),
                    Fin_Year = barcodeInfo.FirstOrDefault().Fin_Year,
                    NewBillNo = Convert.ToString(barcodeInfo.FirstOrDefault().New_Bill_No),
                    ItemTotalAfterDiscount = barcodeInfo.FirstOrDefault().item_total_after_discount,
                    ItemAdditionalDiscount = barcodeInfo.FirstOrDefault().item_additional_discount,
                    TaxPercentage = barcodeInfo.FirstOrDefault().tax_percentage,
                    TaxAmount = barcodeInfo.FirstOrDefault().tax_amount,
                    ItemFinalAmount = barcodeInfo.FirstOrDefault().item_final_amount,
                    SupplierCode = barcodeInfo.FirstOrDefault().supplier_code,
                    ItemSize = barcodeInfo.FirstOrDefault().item_size,
                    ImgID = barcodeInfo.FirstOrDefault().img_id,
                    DesignCode = barcodeInfo.FirstOrDefault().design_code,
                    DesignName = barcodeInfo.FirstOrDefault().design_name,
                    BatchID = barcodeInfo.FirstOrDefault().batch_id,
                    Rf_ID = barcodeInfo.FirstOrDefault().rf_id,
                    McPerPiece = barcodeInfo.FirstOrDefault().mc_per_piece,
                    DiscountMc = barcodeInfo.FirstOrDefault().Discount_Mc,
                    TotalSalesMc = barcodeInfo.FirstOrDefault().Total_sales_mc,
                    McDiscountAmt = barcodeInfo.FirstOrDefault().Mc_Discount_Amt,
                    purchaseMc = barcodeInfo.FirstOrDefault().purchase_mc,
                    GSTGroupCode = barcodeInfo.FirstOrDefault().GSTGroupCode,
                    SGSTPercent = barcodeInfo.FirstOrDefault().SGST_Percent,
                    SGSTAmount = barcodeInfo.FirstOrDefault().SGST_Amount,
                    CGSTPercent = barcodeInfo.FirstOrDefault().CGST_Percent,
                    CGSTAmount = barcodeInfo.FirstOrDefault().CGST_Amount,
                    IGSTPercent = barcodeInfo.FirstOrDefault().IGST_Percent,
                    IGSTAmount = barcodeInfo.FirstOrDefault().IGST_Amount,
                    HSN = barcodeInfo.FirstOrDefault().HSN,
                    PieceRate = barcodeInfo.FirstOrDefault().Piece_Rate,
                    DeductSWt = barcodeInfo.FirstOrDefault().DeductSWt,
                    OrdDiscountAmt = barcodeInfo.FirstOrDefault().Ord_Discount_Amt,
                    DedCounter = barcodeInfo.FirstOrDefault().ded_counter,
                    DedItem = barcodeInfo.FirstOrDefault().ded_item,
                });
            }
            catch (Exception excp) {
                throw excp;
            }
        }

        /// <summary>
        /// Get Stone details by barcode no (only stone details).
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="barcodeNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetBarcodeStoneInfo/{companyCode}/{branchCode}/{barcodeNo}")]
        [Route("GetBarcodeStoneInfo")]
        public IHttpActionResult GetBarcodeStoneInfo(string companyCode, string branchCode, string barcodeNo)
        {
            List<SalesEstStoneVM> lstSalEstStone = new List<SalesEstStoneVM>();
            ObjectParameter errorMsg = new ObjectParameter("errorMsg", typeof(string));
            var stoneInfo = db.usp_getBarcodeStoneInformation(companyCode, branchCode, barcodeNo, errorMsg).ToList();

            for (int i = 0; i < stoneInfo.Count(); i++) {
                SalesEstStoneVM ses = new SalesEstStoneVM();
                ses.ObjID = stoneInfo[i].obj_id;
                ses.CompanyCode = stoneInfo[i].company_code;
                ses.BranchCode = stoneInfo[i].branch_code;
                ses.BillNo = stoneInfo[i].bill_no;
                ses.SlNo = stoneInfo[i].sl_no;
                ses.EstNo = stoneInfo[i].est_no;
                ses.EstSrNo = stoneInfo[i].est_smo;
                ses.BarcodeNo = stoneInfo[i].barcode_no;
                ses.Type = stoneInfo[i].type;
                ses.Name = stoneInfo[i].name;
                ses.Qty = Convert.ToInt32(stoneInfo[i].qty);
                ses.Carrat = Convert.ToDecimal(stoneInfo[i].carrat);
                ses.StoneWt = stoneInfo[i].stone_wt;
                ses.Rate = stoneInfo[i].rate;
                ses.Amount = Convert.ToDecimal(stoneInfo[i].amount);
                ses.Tax = stoneInfo[i].tax;
                ses.TaxAmount = stoneInfo[i].tax_amount;
                ses.TotalAmount = Convert.ToDecimal(stoneInfo[i].total_amount);
                ses.BillType = Convert.ToString(stoneInfo[i].bill_type);
                ses.DealerSalesNo = stoneInfo[i].dealer_sales_no;
                ses.BillDet11PK = stoneInfo[i].BILL_DET11PK;
                ses.UpdateOn = stoneInfo[i].UpdatedON;
                ses.FinYear = stoneInfo[i].Fin_Year;
                ses.Color = stoneInfo[i].color;
                ses.Clarity = stoneInfo[i].clarity;
                ses.Shape = stoneInfo[i].shape;
                ses.Cut = stoneInfo[i].cut;
                ses.Polish = stoneInfo[i].polish;
                ses.Symmetry = stoneInfo[i].symmetry;
                ses.Fluorescence = stoneInfo[i].fluorescence;
                ses.Certificate = stoneInfo[i].CERTIFICATE;
                lstSalEstStone.Add(ses);
            }
            return Ok(lstSalEstStone);
        }

        /// <summary>
        /// Get barcode and there stone details by Barcode Number.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="barcodeNo"></param>
        /// <param name="orderNo"></param>
        /// <param name="isInterstate"></param>
        /// <param name="isOfferCoin"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("BarcodeWithStone/{companyCode}/{branchCode}/{barcodeNo}/{orderNo}/{isInterstate}/{isOfferCoin}")]
        [Route("BarcodeWithStone")]
        public IHttpActionResult GetBarcodeWithStone(string companyCode, string branchCode, string barcodeNo, string orderNo, int isInterstate, int isOfferCoin)
        {

            ErrorVM error = new ErrorVM();
            SalesEstDetailsVM barcodeDetails = new BarcodeBL().GetBarcodeWithStone(companyCode, branchCode, barcodeNo, orderNo, isInterstate, isOfferCoin, 0, out error);
            if (error == null) {
                return Ok(barcodeDetails);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }

            //TODO: Once barcode functionality working fine delete the bellow code.
            //try {
            //    // Tried with this bellow procedure but throwing Error which is not cathable.
            //    //int validate = db.usp_ValidateBarcodeNo(barcodeNo, 0, Common.CompanyCode, Common.BranchCode);

            //    //Validation
            //    string message = ValidateBarcodeDetails(barcodeNo, companyCode, branchCode);
            //    if (message != "") {
            //        return Content(HttpStatusCode.NotFound, new ErrorVM { index = 0, field = "", description = message });
            //    }

            //    #region Group Barcode
            //    // Validate barcode is group barcode or not
            //    KTTU_BARCODE_MASTER barcodeMaster = db.KTTU_BARCODE_MASTER.Where(bar => bar.company_code == companyCode
            //                                                                    && bar.branch_code == branchCode
            //                                                                    && bar.barcode_no == barcodeNo
            //                                                                    && bar.sold_flag == "N").FirstOrDefault();
            //    if (barcodeMaster != null && barcodeMaster.Tagging_Type == "G") {
            //        //Validate Group Barcode
            //        if (barcodeMaster.qty == 0 && barcodeMaster.gwt == 0) {
            //            return Content(HttpStatusCode.NotFound, new ErrorVM
            //            {
            //                index = 0,
            //                field = "",
            //                description = "Barcode No already Billed."
            //            });
            //        }
            //        SalesEstDetailsVM barcode = new SalesEstDetailsVM();
            //        barcode.CompanyCode = barcodeMaster.company_code;
            //        barcode.BranchCode = barcodeMaster.branch_code;
            //        barcode.BarcodeNo = barcodeMaster.barcode_no;
            //        barcode.ItemName = barcodeMaster.item_name;
            //        barcode.TaggingType = barcodeMaster.Tagging_Type;
            //        barcode.GsCode = barcodeMaster.gs_code;
            //        barcode.Karat = barcodeMaster.karat;
            //        barcode.CounterCode = barcodeMaster.counter_code;
            //        barcode.ItemQty = barcodeMaster.qty;
            //        barcode.Grosswt = Convert.ToDecimal(barcodeMaster.gwt);
            //        barcode.Netwt = Convert.ToDecimal(barcodeMaster.nwt);
            //        barcode.Stonewt = Convert.ToDecimal(barcodeMaster.swt);
            //        barcode.StoneCharges = Convert.ToDecimal(barcodeMaster.stone_amount);
            //        barcode.DiamondCharges = barcodeMaster.daimond_amount;
            //        barcode.McType = Convert.ToInt32(barcodeMaster.mc_type);
            //        barcode.McPercent = barcodeMaster.mc_percent;
            //        barcode.Rate = SIGlobals.Globals.GetRate(db, companyCode, branchCode, barcodeMaster.gs_code, barcodeMaster.karat);
            //        barcode.GSTGroupCode = SIGlobals.Globals.GetGSTGroupCode(db, barcodeMaster.gs_code, companyCode, branchCode);
            //        return Ok(barcode);
            //    }
            //    #endregion

            //    // If Valid
            //    ObjectParameter errorMsg = new ObjectParameter("errorMsg", typeof(string));
            //    var barcodeInfo = db.usp_getBarcodeInformation(companyCode, branchCode, barcodeNo, orderNo == null ? "0" : orderNo, 0, isInterstate == 1 ? true : false, 0, 0, errorMsg).ToArray();
            //    if (errorMsg.Value.ToString() != "") {
            //        return Content(HttpStatusCode.NotFound, new ErrorVM { index = 0, field = "", description = errorMsg.ToString() });
            //    }

            //    // Taking Stone Details
            //    List<SalesEstStoneVM> lstSalEstStone = new List<SalesEstStoneVM>();
            //    ObjectParameter errorMsgStone = new ObjectParameter("errorMsg", typeof(string));
            //    var stoneInfo = db.usp_getBarcodeStoneInformation(companyCode, branchCode, barcodeNo, errorMsgStone).ToList();
            //    decimal totalStoneAmount = 0;
            //    decimal totalOnlyStoneAmount = 0;
            //    decimal totalOnlyDiamondAmount = 0;
            //    for (int i = 0; i < stoneInfo.Count(); i++) {

            //        // Logic taken by magna code.
            //        string dmName = stoneInfo[i].name;
            //        decimal karat = Convert.ToDecimal(stoneInfo[i].carrat);
            //        int diamondQty = Convert.ToInt32(stoneInfo[i].qty);
            //        decimal stoneRate = 0;
            //        decimal totalDiamondKarat = karat;
            //        if (diamondQty > 0) {
            //            totalDiamondKarat = Math.Round(karat / diamondQty, 3);
            //        }
            //        List<KSTU_DIAMOND_RATE_MASTER> lstOfkdrm = db.KSTU_DIAMOND_RATE_MASTER.Where(d => d.dm_name == dmName
            //        && d.karat_from <= totalDiamondKarat && d.company_code == companyCode && d.branch_code == branchCode).OrderBy(d => d.karat_from).ToList();
            //        foreach (KSTU_DIAMOND_RATE_MASTER kdr in lstOfkdrm) {
            //            if (kdr.karat_to >= totalDiamondKarat) {
            //                stoneRate = kdr.rate;
            //            }
            //        }
            //        SalesEstStoneVM ses = new SalesEstStoneVM();
            //        ses.ObjID = stoneInfo[i].obj_id;
            //        ses.CompanyCode = stoneInfo[i].company_code;
            //        ses.BranchCode = stoneInfo[i].branch_code;
            //        ses.BillNo = stoneInfo[i].bill_no;
            //        ses.SlNo = stoneInfo[i].sl_no;
            //        ses.EstNo = stoneInfo[i].est_no;
            //        ses.EstSrNo = stoneInfo[i].est_smo;
            //        ses.BarcodeNo = stoneInfo[i].barcode_no;
            //        ses.Type = stoneInfo[i].type == "S" ? "STN" : "DMD";
            //        ses.Name = stoneInfo[i].name;
            //        ses.Qty = Convert.ToInt32(stoneInfo[i].qty);
            //        ses.Carrat = Convert.ToDecimal(stoneInfo[i].carrat);

            //        if (lstOfkdrm != null && stoneRate != 0) {
            //            ses.Rate = stoneRate;
            //            ses.Amount = Convert.ToDecimal(stoneInfo[i].carrat) * stoneRate;
            //        }
            //        else {
            //            ses.Rate = stoneInfo[i].rate;
            //            ses.Amount = Convert.ToDecimal(stoneInfo[i].amount);
            //        }
            //        if (dmName == "DM") {
            //            totalOnlyDiamondAmount = totalDiamondKarat + ses.Amount;
            //        }
            //        else {
            //            totalOnlyStoneAmount = totalOnlyStoneAmount + ses.Amount;
            //        }
            //        totalStoneAmount = totalStoneAmount + ses.Amount;
            //        ses.StoneWt = stoneInfo[i].carrat / 5;
            //        ses.Tax = stoneInfo[i].tax;
            //        ses.TaxAmount = stoneInfo[i].tax_amount;
            //        ses.TotalAmount = Convert.ToDecimal(stoneInfo[i].total_amount);
            //        ses.BillType = Convert.ToString(stoneInfo[i].bill_type);
            //        ses.DealerSalesNo = stoneInfo[i].dealer_sales_no;
            //        ses.BillDet11PK = stoneInfo[i].BILL_DET11PK;
            //        ses.UpdateOn = stoneInfo[i].UpdatedON;
            //        ses.FinYear = stoneInfo[i].Fin_Year;
            //        ses.Color = stoneInfo[i].color;
            //        ses.Clarity = stoneInfo[i].clarity;
            //        ses.Shape = stoneInfo[i].shape;
            //        ses.Cut = stoneInfo[i].cut;
            //        ses.Polish = stoneInfo[i].polish;
            //        ses.Symmetry = stoneInfo[i].symmetry;
            //        ses.Fluorescence = stoneInfo[i].fluorescence;
            //        ses.Certificate = stoneInfo[i].CERTIFICATE;
            //        lstSalEstStone.Add(ses);
            //    }
            //    return Ok(new SalesEstDetailsVM()
            //    {
            //        ObjID = barcodeInfo.FirstOrDefault().obj_id,
            //        CompanyCode = barcodeInfo.FirstOrDefault().company_code,
            //        BranchCode = barcodeInfo.FirstOrDefault().branch_code,
            //        EstNo = barcodeInfo.FirstOrDefault().est_no,
            //        SlNo = barcodeInfo.FirstOrDefault().sl_no,
            //        BillNo = barcodeInfo.FirstOrDefault().bill_no,
            //        BarcodeNo = barcodeInfo.FirstOrDefault().barcode_no,
            //        SalCode = Convert.ToString(barcodeInfo.FirstOrDefault().Sal_Code),
            //        CounterCode = barcodeInfo.FirstOrDefault().counter_code,
            //        ItemName = barcodeInfo.FirstOrDefault().item_name,
            //        ItemQty = barcodeInfo.FirstOrDefault().qty,
            //        Grosswt = Convert.ToDecimal(barcodeInfo.FirstOrDefault().gwt),
            //        Stonewt = Convert.ToDecimal(barcodeInfo.FirstOrDefault().swt),
            //        Netwt = Convert.ToDecimal(barcodeInfo.FirstOrDefault().nwt),
            //        AddWt = barcodeInfo.FirstOrDefault().AddWt,
            //        DeductWt = barcodeInfo.FirstOrDefault().DedWt,
            //        MakingChargePerRs = Convert.ToDecimal(barcodeInfo.FirstOrDefault().making_charge_per_rs),
            //        WastPercent = Convert.ToDecimal(barcodeInfo.FirstOrDefault().wast_percent),
            //        GoldValue = Convert.ToDecimal(barcodeInfo.FirstOrDefault().gold_value),
            //        VaAmount = Convert.ToDecimal(barcodeInfo.FirstOrDefault().va_amount),
            //        StoneCharges = totalOnlyStoneAmount, //totalStoneAmount,// Convert.ToDecimal(barcodeInfo.FirstOrDefault().stone_charges),
            //        DiamondCharges = totalOnlyDiamondAmount, //barcodeInfo.FirstOrDefault().diamond_charges,
            //        TotalAmount = Convert.ToDecimal(barcodeInfo.FirstOrDefault().total_amount),
            //        Hallmarkarges = barcodeInfo.FirstOrDefault().hallmarcharges,
            //        McAmount = barcodeInfo.FirstOrDefault().mc_amount,
            //        WastageGrms = barcodeInfo.FirstOrDefault().wastage_grms,
            //        McPercent = barcodeInfo.FirstOrDefault().mc_percent,
            //        AddQty = barcodeInfo.FirstOrDefault().Addqty,
            //        DeductQty = barcodeInfo.FirstOrDefault().DedQty,
            //        OfferValue = barcodeInfo.FirstOrDefault().offer_value,
            //        UpdateOn = barcodeInfo.FirstOrDefault().UpdateOn,
            //        GsCode = barcodeInfo.FirstOrDefault().gs_code,
            //        Rate = barcodeInfo.FirstOrDefault().rate,
            //        Karat = barcodeInfo.FirstOrDefault().karat,
            //        AdBarcode = barcodeInfo.FirstOrDefault().ad_barcode,
            //        AdCounter = barcodeInfo.FirstOrDefault().ad_counter,
            //        AdItem = barcodeInfo.FirstOrDefault().ad_item,
            //        IsEDApplicable = isOfferCoin == 1 ? "O" : barcodeInfo.FirstOrDefault().isEDApplicable,
            //        McType = Convert.ToInt32(barcodeInfo.FirstOrDefault().mc_type),
            //        Fin_Year = barcodeInfo.FirstOrDefault().Fin_Year,
            //        NewBillNo = Convert.ToString(barcodeInfo.FirstOrDefault().New_Bill_No),
            //        ItemTotalAfterDiscount = barcodeInfo.FirstOrDefault().item_total_after_discount,
            //        ItemAdditionalDiscount = barcodeInfo.FirstOrDefault().item_additional_discount,
            //        TaxPercentage = barcodeInfo.FirstOrDefault().tax_percentage,
            //        TaxAmount = barcodeInfo.FirstOrDefault().tax_amount,
            //        ItemFinalAmount = barcodeInfo.FirstOrDefault().item_final_amount,
            //        SupplierCode = barcodeInfo.FirstOrDefault().supplier_code,
            //        ItemSize = barcodeInfo.FirstOrDefault().item_size,
            //        ImgID = barcodeInfo.FirstOrDefault().img_id,
            //        DesignCode = barcodeInfo.FirstOrDefault().design_code,
            //        DesignName = barcodeInfo.FirstOrDefault().design_name,
            //        BatchID = barcodeInfo.FirstOrDefault().batch_id,
            //        Rf_ID = barcodeInfo.FirstOrDefault().rf_id,
            //        McPerPiece = barcodeInfo.FirstOrDefault().mc_per_piece,
            //        DiscountMc = barcodeInfo.FirstOrDefault().Discount_Mc,
            //        TotalSalesMc = barcodeInfo.FirstOrDefault().Total_sales_mc,
            //        McDiscountAmt = barcodeInfo.FirstOrDefault().Mc_Discount_Amt,
            //        purchaseMc = barcodeInfo.FirstOrDefault().purchase_mc,
            //        GSTGroupCode = barcodeInfo.FirstOrDefault().GSTGroupCode,
            //        SGSTPercent = Convert.ToDecimal(barcodeInfo.FirstOrDefault().SGST_Percent),
            //        SGSTAmount = Convert.ToDecimal(barcodeInfo.FirstOrDefault().SGST_Amount),
            //        CGSTPercent = Convert.ToDecimal(barcodeInfo.FirstOrDefault().CGST_Percent),
            //        CGSTAmount = Convert.ToDecimal(barcodeInfo.FirstOrDefault().CGST_Amount),
            //        IGSTPercent = Convert.ToDecimal(barcodeInfo.FirstOrDefault().IGST_Percent),
            //        IGSTAmount = Convert.ToDecimal(barcodeInfo.FirstOrDefault().IGST_Amount),
            //        HSN = barcodeInfo.FirstOrDefault().HSN,
            //        PieceRate = barcodeInfo.FirstOrDefault().Piece_Rate,
            //        DeductSWt = barcodeInfo.FirstOrDefault().DeductSWt,
            //        OrdDiscountAmt = barcodeInfo.FirstOrDefault().Ord_Discount_Amt,
            //        DedCounter = barcodeInfo.FirstOrDefault().ded_counter,
            //        DedItem = barcodeInfo.FirstOrDefault().ded_item,
            //        isInterstate = isInterstate,
            //        salesEstStoneVM = lstSalEstStone
            //    });
            //}
            //catch (Exception excp) {
            //    throw excp;
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="barcodeNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("BarcodeWithStoneWithoutValidation/{companyCode}/{branchCode}/{barcodeNo}")]
        [Route("BarcodeWithStoneWithoutValidation")]
        public IHttpActionResult GetBarcodeWithStoneWithoutValidation(string companyCode, string branchCode, string barcodeNo)
        {

            ErrorVM error = new ErrorVM();
            SalesEstDetailsVM barcodeDetails = new BarcodeBL().GetBarcodeWithStone(companyCode, branchCode, barcodeNo, "", 0, 0, 1, out error);
            if (error == null) {
                return Ok(barcodeDetails);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Barcode details for Sales Module.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="barcodeNo"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("BarcodeWithStoneForSales/{companyCode}/{branchCode}/{barcodeNo}/{orderNo}")]
        [Route("BarcodeWithStoneForSales")]
        public IHttpActionResult GetBarcodeWithStoneForSales(string companyCode, string branchCode, string barcodeNo, string orderNo)
        {
            try {
                ObjectParameter errorMsg = new ObjectParameter("errorMsg", typeof(string));
                var barcodeInfo = db.usp_getBarcodeInformation(companyCode, branchCode, barcodeNo, orderNo == "" ? "0" : orderNo, 0, false, 0, 0, errorMsg).ToArray();
                if (errorMsg.Value.ToString() != "") {
                    return Content(HttpStatusCode.NotFound, new ErrorVM { index = 0, field = "", description = errorMsg.ToString() });
                }

                // Taking Stone Details
                List<SalesEstStoneVM> lstSalEstStone = new List<SalesEstStoneVM>();
                ObjectParameter errorMsgStone = new ObjectParameter("errorMsg", typeof(string));
                var stoneInfo = db.usp_getBarcodeStoneInformation(companyCode, branchCode, barcodeNo, errorMsgStone).ToList();

                for (int i = 0; i < stoneInfo.Count(); i++) {

                    // Logic taken by magna code.
                    string dmName = stoneInfo[i].name;
                    decimal karat = Convert.ToDecimal(stoneInfo[i].carrat);
                    int diamondQty = Convert.ToInt32(stoneInfo[i].qty);
                    decimal stoneRate = 0;
                    decimal totalDiamondKarat = karat;
                    if (diamondQty > 0) {
                        totalDiamondKarat = Math.Round(karat / diamondQty, 3);
                    }
                    List<KSTU_DIAMOND_RATE_MASTER> lstOfkdrm = db.KSTU_DIAMOND_RATE_MASTER.Where(d => d.dm_name == dmName
                    && d.karat_from <= totalDiamondKarat && d.company_code == companyCode && d.branch_code == branchCode).OrderBy(d => d.karat_from).ToList();
                    foreach (KSTU_DIAMOND_RATE_MASTER kdr in lstOfkdrm) {
                        if (kdr.karat_to >= totalDiamondKarat) {
                            stoneRate = kdr.rate;
                        }
                    }
                    SalesEstStoneVM ses = new SalesEstStoneVM();
                    ses.ObjID = stoneInfo[i].obj_id;
                    ses.CompanyCode = stoneInfo[i].company_code;
                    ses.BranchCode = stoneInfo[i].branch_code;
                    ses.BillNo = stoneInfo[i].bill_no;
                    ses.SlNo = stoneInfo[i].sl_no;
                    ses.EstNo = stoneInfo[i].est_no;
                    ses.EstSrNo = stoneInfo[i].est_smo;
                    ses.BarcodeNo = stoneInfo[i].barcode_no;
                    ses.Type = stoneInfo[i].type == "S" ? "STN" : "DMD";
                    ses.Name = stoneInfo[i].name;
                    ses.Qty = Convert.ToInt32(stoneInfo[i].qty);
                    ses.Carrat = Convert.ToDecimal(stoneInfo[i].carrat);

                    if (lstOfkdrm != null && stoneRate != 0) {
                        ses.Rate = stoneRate;
                        ses.Amount = Convert.ToDecimal(stoneInfo[i].carrat) * stoneRate;
                    }
                    else {
                        ses.Rate = stoneInfo[i].rate;
                        ses.Amount = Convert.ToDecimal(stoneInfo[i].amount);
                    }
                    ses.StoneWt = stoneInfo[i].carrat / 5;
                    ses.Tax = stoneInfo[i].tax;
                    ses.TaxAmount = stoneInfo[i].tax_amount;
                    ses.TotalAmount = Convert.ToDecimal(stoneInfo[i].total_amount);
                    ses.BillType = Convert.ToString(stoneInfo[i].bill_type);
                    ses.DealerSalesNo = stoneInfo[i].dealer_sales_no;
                    ses.BillDet11PK = stoneInfo[i].BILL_DET11PK;
                    ses.UpdateOn = stoneInfo[i].UpdatedON;
                    ses.FinYear = stoneInfo[i].Fin_Year;
                    ses.Color = stoneInfo[i].color;
                    ses.Clarity = stoneInfo[i].clarity;
                    ses.Shape = stoneInfo[i].shape;
                    ses.Cut = stoneInfo[i].cut;
                    ses.Polish = stoneInfo[i].polish;
                    ses.Symmetry = stoneInfo[i].symmetry;
                    ses.Fluorescence = stoneInfo[i].fluorescence;
                    ses.Certificate = stoneInfo[i].CERTIFICATE;
                    lstSalEstStone.Add(ses);
                }
                return Ok(new SalesEstDetailsVM()
                {
                    ObjID = barcodeInfo.FirstOrDefault().obj_id,
                    CompanyCode = barcodeInfo.FirstOrDefault().company_code,
                    BranchCode = barcodeInfo.FirstOrDefault().branch_code,
                    EstNo = barcodeInfo.FirstOrDefault().est_no,
                    SlNo = barcodeInfo.FirstOrDefault().sl_no,
                    BillNo = barcodeInfo.FirstOrDefault().bill_no,
                    BarcodeNo = barcodeInfo.FirstOrDefault().barcode_no,
                    SalCode = Convert.ToString(barcodeInfo.FirstOrDefault().Sal_Code),
                    CounterCode = barcodeInfo.FirstOrDefault().counter_code,
                    ItemName = barcodeInfo.FirstOrDefault().item_name,
                    ItemQty = barcodeInfo.FirstOrDefault().qty,
                    Grosswt = Convert.ToDecimal(barcodeInfo.FirstOrDefault().gwt),
                    Stonewt = Convert.ToDecimal(barcodeInfo.FirstOrDefault().swt),
                    Netwt = Convert.ToDecimal(barcodeInfo.FirstOrDefault().nwt),
                    AddWt = barcodeInfo.FirstOrDefault().AddWt,
                    DeductWt = barcodeInfo.FirstOrDefault().DedWt,
                    MakingChargePerRs = Convert.ToDecimal(barcodeInfo.FirstOrDefault().making_charge_per_rs),
                    WastPercent = Convert.ToDecimal(barcodeInfo.FirstOrDefault().wast_percent),
                    GoldValue = Convert.ToDecimal(barcodeInfo.FirstOrDefault().gold_value),
                    VaAmount = Convert.ToDecimal(barcodeInfo.FirstOrDefault().va_amount),
                    StoneCharges = Convert.ToDecimal(barcodeInfo.FirstOrDefault().stone_charges),
                    DiamondCharges = barcodeInfo.FirstOrDefault().diamond_charges,
                    TotalAmount = Convert.ToDecimal(barcodeInfo.FirstOrDefault().total_amount),
                    Hallmarkarges = barcodeInfo.FirstOrDefault().hallmarcharges,
                    McAmount = barcodeInfo.FirstOrDefault().mc_amount,
                    WastageGrms = barcodeInfo.FirstOrDefault().wastage_grms,
                    McPercent = barcodeInfo.FirstOrDefault().mc_percent,
                    AddQty = barcodeInfo.FirstOrDefault().Addqty,
                    DeductQty = barcodeInfo.FirstOrDefault().DedQty,
                    OfferValue = barcodeInfo.FirstOrDefault().offer_value,
                    UpdateOn = barcodeInfo.FirstOrDefault().UpdateOn,
                    GsCode = barcodeInfo.FirstOrDefault().gs_code,
                    Rate = barcodeInfo.FirstOrDefault().rate,
                    Karat = barcodeInfo.FirstOrDefault().karat,
                    AdBarcode = barcodeInfo.FirstOrDefault().ad_barcode,
                    AdCounter = barcodeInfo.FirstOrDefault().ad_counter,
                    AdItem = barcodeInfo.FirstOrDefault().ad_item,
                    IsEDApplicable = barcodeInfo.FirstOrDefault().isEDApplicable,
                    McType = Convert.ToInt32(barcodeInfo.FirstOrDefault().mc_type),
                    Fin_Year = barcodeInfo.FirstOrDefault().Fin_Year,
                    NewBillNo = Convert.ToString(barcodeInfo.FirstOrDefault().New_Bill_No),
                    ItemTotalAfterDiscount = barcodeInfo.FirstOrDefault().item_total_after_discount,
                    ItemAdditionalDiscount = barcodeInfo.FirstOrDefault().item_additional_discount,
                    TaxPercentage = barcodeInfo.FirstOrDefault().tax_percentage,
                    TaxAmount = barcodeInfo.FirstOrDefault().tax_amount,
                    ItemFinalAmount = barcodeInfo.FirstOrDefault().item_final_amount,
                    SupplierCode = barcodeInfo.FirstOrDefault().supplier_code,
                    ItemSize = barcodeInfo.FirstOrDefault().item_size,
                    ImgID = barcodeInfo.FirstOrDefault().img_id,
                    DesignCode = barcodeInfo.FirstOrDefault().design_code,
                    DesignName = barcodeInfo.FirstOrDefault().design_name,
                    BatchID = barcodeInfo.FirstOrDefault().batch_id,
                    Rf_ID = barcodeInfo.FirstOrDefault().rf_id,
                    McPerPiece = barcodeInfo.FirstOrDefault().mc_per_piece,
                    DiscountMc = barcodeInfo.FirstOrDefault().Discount_Mc,
                    TotalSalesMc = barcodeInfo.FirstOrDefault().Total_sales_mc,
                    McDiscountAmt = barcodeInfo.FirstOrDefault().Mc_Discount_Amt,
                    purchaseMc = barcodeInfo.FirstOrDefault().purchase_mc,
                    GSTGroupCode = barcodeInfo.FirstOrDefault().GSTGroupCode,
                    SGSTPercent = barcodeInfo.FirstOrDefault().SGST_Percent,
                    SGSTAmount = barcodeInfo.FirstOrDefault().SGST_Amount,
                    CGSTPercent = barcodeInfo.FirstOrDefault().CGST_Percent,
                    CGSTAmount = barcodeInfo.FirstOrDefault().CGST_Amount,
                    IGSTPercent = barcodeInfo.FirstOrDefault().IGST_Percent,
                    IGSTAmount = barcodeInfo.FirstOrDefault().IGST_Amount,
                    HSN = barcodeInfo.FirstOrDefault().HSN,
                    PieceRate = barcodeInfo.FirstOrDefault().Piece_Rate,
                    DeductSWt = barcodeInfo.FirstOrDefault().DeductSWt,
                    OrdDiscountAmt = barcodeInfo.FirstOrDefault().Ord_Discount_Amt,
                    DedCounter = barcodeInfo.FirstOrDefault().ded_counter,
                    DedItem = barcodeInfo.FirstOrDefault().ded_item,
                    salesEstStoneVM = lstSalEstStone
                });
            }
            catch (Exception excp) {
                throw excp;
            }
        }

        /// <summary>
        /// This api return Barcode information, which you can use at the time of Addwt. This compares both barcode and addwt barcode gs types.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="barcodeNo"></param>
        /// <param name="addWtBarcode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("BarcodeWithStoneForAddWt/{companyCode}/{branchCode}/{barcodeNo}/{addWtBarcode}")]
        [Route("BarcodeWithStoneForAddWt")]
        public IHttpActionResult BarcodeWithStoneForAddWt(string companyCode, string branchCode, string barcodeNo, string addWtBarcode)
        {
            try {
                if (barcodeNo == addWtBarcode) {
                    return Content(HttpStatusCode.NotFound, new ErrorVM { index = 0, field = "", description = "Invalid barcode." });
                }
                //Validation
                string message = ValidateBarcodeDetails(barcodeNo, companyCode, branchCode);
                if (message != "") {
                    return Content(HttpStatusCode.NotFound, new ErrorVM { index = 0, field = "", description = message });
                }

                message = ValidateBarcodeDetails(addWtBarcode, companyCode, branchCode);
                if (message != "") {
                    return Content(HttpStatusCode.NotFound, new ErrorVM { index = 0, field = "", description = message });
                }

                // If Valid
                ObjectParameter errorMsg = new ObjectParameter("errorMsg", typeof(string));
                //var barcodeInfo = db.usp_getBarcodeInformation(companyCode, branchCode, barcodeNo, "0", 0, false, 0, 0, errorMsg).ToArray();
                //if (errorMsg.Value.ToString() != "") {
                //    return Content(HttpStatusCode.NotFound, new ErrorVM { index = 0, field = "", description = errorMsg.ToString() });
                //}

                KTTU_BARCODE_MASTER barcode1 = db.KTTU_BARCODE_MASTER.Where(bar => bar.barcode_no == barcodeNo && bar.company_code == companyCode && bar.branch_code == branchCode).FirstOrDefault();
                KTTU_BARCODE_MASTER barcode2 = db.KTTU_BARCODE_MASTER.Where(bar => bar.barcode_no == addWtBarcode && bar.company_code == companyCode && bar.branch_code == branchCode).FirstOrDefault();

                if (barcode1.gs_code != barcode2.gs_code) {
                    return Content(HttpStatusCode.NotFound, new ErrorVM { index = 0, field = "", description = "Barcode No: " + addWtBarcode + " is of " + barcode2.gs_code + ", It cannot be added." });
                }

                // Taking Stone Details
                List<SalesEstStoneVM> lstSalEstStone = new List<SalesEstStoneVM>();
                ObjectParameter errorMsgStone = new ObjectParameter("errorMsg", typeof(string));
                var addBarcodeInfo = db.usp_getBarcodeInformation(companyCode, branchCode, addWtBarcode, "0", 0, false, 0, 0, errorMsg).ToArray();
                var stoneInfo = db.usp_getBarcodeStoneInformation(companyCode, branchCode, addWtBarcode, errorMsgStone).ToList();
                decimal totalStoneAmount = 0;
                for (int i = 0; i < stoneInfo.Count(); i++) {

                    // Logic taken by magna code.
                    string dmName = stoneInfo[i].name;
                    decimal karat = Convert.ToDecimal(stoneInfo[i].carrat);
                    int diamondQty = Convert.ToInt32(stoneInfo[i].qty);
                    decimal stoneRate = 0;
                    decimal totalDiamondKarat = karat;
                    if (diamondQty > 0) {
                        totalDiamondKarat = Math.Round(karat / diamondQty, 3);
                    }
                    List<KSTU_DIAMOND_RATE_MASTER> lstOfkdrm = db.KSTU_DIAMOND_RATE_MASTER.Where(d => d.dm_name == dmName && d.karat_from <= totalDiamondKarat
                    && d.company_code == companyCode && d.branch_code == branchCode).OrderBy(d => d.karat_from).ToList();
                    foreach (KSTU_DIAMOND_RATE_MASTER kdr in lstOfkdrm) {
                        if (kdr.karat_to >= totalDiamondKarat) {
                            stoneRate = kdr.rate;
                        }
                    }
                    SalesEstStoneVM ses = new SalesEstStoneVM();
                    ses.ObjID = stoneInfo[i].obj_id;
                    ses.CompanyCode = stoneInfo[i].company_code;
                    ses.BranchCode = stoneInfo[i].branch_code;
                    ses.BillNo = stoneInfo[i].bill_no;
                    ses.SlNo = stoneInfo[i].sl_no;
                    ses.EstNo = stoneInfo[i].est_no;
                    ses.EstSrNo = stoneInfo[i].est_smo;
                    ses.BarcodeNo = stoneInfo[i].barcode_no;
                    ses.Type = stoneInfo[i].type == "S" ? "STN" : "DMD";
                    ses.Name = stoneInfo[i].name;
                    ses.Qty = Convert.ToInt32(stoneInfo[i].qty);
                    ses.Carrat = Convert.ToDecimal(stoneInfo[i].carrat);

                    if (lstOfkdrm != null && stoneRate != 0) {
                        ses.Rate = stoneRate;
                        ses.Amount = Convert.ToDecimal(stoneInfo[i].carrat) * stoneRate;
                    }
                    else {
                        ses.Rate = stoneInfo[i].rate;
                        ses.Amount = Convert.ToDecimal(stoneInfo[i].amount);
                    }
                    totalStoneAmount = totalStoneAmount + ses.Amount;
                    ses.StoneWt = stoneInfo[i].carrat / 5;
                    ses.Tax = stoneInfo[i].tax;
                    ses.TaxAmount = stoneInfo[i].tax_amount;
                    ses.TotalAmount = Convert.ToDecimal(stoneInfo[i].total_amount);
                    ses.BillType = Convert.ToString(stoneInfo[i].bill_type);
                    ses.DealerSalesNo = stoneInfo[i].dealer_sales_no;
                    ses.BillDet11PK = stoneInfo[i].BILL_DET11PK;
                    ses.UpdateOn = stoneInfo[i].UpdatedON;
                    ses.FinYear = stoneInfo[i].Fin_Year;
                    ses.Color = stoneInfo[i].color;
                    ses.Clarity = stoneInfo[i].clarity;
                    ses.Shape = stoneInfo[i].shape;
                    ses.Cut = stoneInfo[i].cut;
                    ses.Polish = stoneInfo[i].polish;
                    ses.Symmetry = stoneInfo[i].symmetry;
                    ses.Fluorescence = stoneInfo[i].fluorescence;
                    ses.Certificate = stoneInfo[i].CERTIFICATE;
                    lstSalEstStone.Add(ses);
                }
                return Ok(new SalesEstDetailsVM()
                {
                    //ObjID = addBarcodeInfo.FirstOrDefault().obj_id,
                    CompanyCode = addBarcodeInfo.FirstOrDefault().company_code,
                    BranchCode = addBarcodeInfo.FirstOrDefault().branch_code,
                    EstNo = addBarcodeInfo.FirstOrDefault().est_no,
                    SlNo = addBarcodeInfo.FirstOrDefault().sl_no,
                    BillNo = addBarcodeInfo.FirstOrDefault().bill_no,
                    BarcodeNo = addBarcodeInfo.FirstOrDefault().barcode_no,
                    SalCode = Convert.ToString(addBarcodeInfo.FirstOrDefault().Sal_Code),
                    CounterCode = addBarcodeInfo.FirstOrDefault().counter_code,
                    ItemName = addBarcodeInfo.FirstOrDefault().item_name,
                    ItemQty = addBarcodeInfo.FirstOrDefault().qty,
                    Grosswt = Convert.ToDecimal(addBarcodeInfo.FirstOrDefault().gwt),
                    Stonewt = Convert.ToDecimal(addBarcodeInfo.FirstOrDefault().swt),
                    Netwt = Convert.ToDecimal(addBarcodeInfo.FirstOrDefault().nwt),
                    AddWt = addBarcodeInfo.FirstOrDefault().AddWt,
                    DeductWt = addBarcodeInfo.FirstOrDefault().DedWt,
                    MakingChargePerRs = Convert.ToDecimal(addBarcodeInfo.FirstOrDefault().making_charge_per_rs),
                    WastPercent = Convert.ToDecimal(addBarcodeInfo.FirstOrDefault().wast_percent),
                    GoldValue = Convert.ToDecimal(addBarcodeInfo.FirstOrDefault().gold_value),
                    VaAmount = Convert.ToDecimal(addBarcodeInfo.FirstOrDefault().va_amount),
                    StoneCharges = totalStoneAmount,// Convert.ToDecimal(addBarcodeInfo.FirstOrDefault().stone_charges),
                    DiamondCharges = addBarcodeInfo.FirstOrDefault().diamond_charges,
                    TotalAmount = Convert.ToDecimal(addBarcodeInfo.FirstOrDefault().total_amount),
                    Hallmarkarges = addBarcodeInfo.FirstOrDefault().hallmarcharges,
                    McAmount = addBarcodeInfo.FirstOrDefault().mc_amount,
                    WastageGrms = addBarcodeInfo.FirstOrDefault().wastage_grms,
                    McPercent = addBarcodeInfo.FirstOrDefault().mc_percent,
                    AddQty = addBarcodeInfo.FirstOrDefault().Addqty,
                    DeductQty = addBarcodeInfo.FirstOrDefault().DedQty,
                    OfferValue = addBarcodeInfo.FirstOrDefault().offer_value,
                    UpdateOn = addBarcodeInfo.FirstOrDefault().UpdateOn,
                    GsCode = addBarcodeInfo.FirstOrDefault().gs_code,
                    Rate = addBarcodeInfo.FirstOrDefault().rate,
                    Karat = addBarcodeInfo.FirstOrDefault().karat,
                    AdBarcode = addBarcodeInfo.FirstOrDefault().ad_barcode,
                    AdCounter = addBarcodeInfo.FirstOrDefault().ad_counter,
                    AdItem = addBarcodeInfo.FirstOrDefault().ad_item,
                    IsEDApplicable = addBarcodeInfo.FirstOrDefault().isEDApplicable,
                    McType = Convert.ToInt32(addBarcodeInfo.FirstOrDefault().mc_type),
                    Fin_Year = addBarcodeInfo.FirstOrDefault().Fin_Year,
                    NewBillNo = Convert.ToString(addBarcodeInfo.FirstOrDefault().New_Bill_No),
                    ItemTotalAfterDiscount = addBarcodeInfo.FirstOrDefault().item_total_after_discount,
                    ItemAdditionalDiscount = addBarcodeInfo.FirstOrDefault().item_additional_discount,
                    TaxPercentage = addBarcodeInfo.FirstOrDefault().tax_percentage,
                    TaxAmount = addBarcodeInfo.FirstOrDefault().tax_amount,
                    ItemFinalAmount = addBarcodeInfo.FirstOrDefault().item_final_amount,
                    SupplierCode = addBarcodeInfo.FirstOrDefault().supplier_code,
                    ItemSize = addBarcodeInfo.FirstOrDefault().item_size,
                    ImgID = addBarcodeInfo.FirstOrDefault().img_id,
                    DesignCode = addBarcodeInfo.FirstOrDefault().design_code,
                    DesignName = addBarcodeInfo.FirstOrDefault().design_name,
                    BatchID = addBarcodeInfo.FirstOrDefault().batch_id,
                    Rf_ID = addBarcodeInfo.FirstOrDefault().rf_id,
                    McPerPiece = addBarcodeInfo.FirstOrDefault().mc_per_piece,
                    DiscountMc = addBarcodeInfo.FirstOrDefault().Discount_Mc,
                    TotalSalesMc = addBarcodeInfo.FirstOrDefault().Total_sales_mc,
                    McDiscountAmt = addBarcodeInfo.FirstOrDefault().Mc_Discount_Amt,
                    purchaseMc = addBarcodeInfo.FirstOrDefault().purchase_mc,
                    GSTGroupCode = addBarcodeInfo.FirstOrDefault().GSTGroupCode,
                    SGSTPercent = addBarcodeInfo.FirstOrDefault().SGST_Percent,
                    SGSTAmount = addBarcodeInfo.FirstOrDefault().SGST_Amount,
                    CGSTPercent = addBarcodeInfo.FirstOrDefault().CGST_Percent,
                    CGSTAmount = addBarcodeInfo.FirstOrDefault().CGST_Amount,
                    IGSTPercent = addBarcodeInfo.FirstOrDefault().IGST_Percent,
                    IGSTAmount = addBarcodeInfo.FirstOrDefault().IGST_Amount,
                    HSN = addBarcodeInfo.FirstOrDefault().HSN,
                    PieceRate = addBarcodeInfo.FirstOrDefault().Piece_Rate,
                    DeductSWt = addBarcodeInfo.FirstOrDefault().DeductSWt,
                    OrdDiscountAmt = addBarcodeInfo.FirstOrDefault().Ord_Discount_Amt,
                    DedCounter = addBarcodeInfo.FirstOrDefault().ded_counter,
                    DedItem = addBarcodeInfo.FirstOrDefault().ded_item,
                    salesEstStoneVM = lstSalEstStone
                });
            }
            catch (Exception excp) {
                throw excp;
            }
        }

        /// <summary>
        /// Barcode Information.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="barcodeNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("BarcodeSerach/{companyCode}/{branchCode}/{barcodeNo}")]
        public IHttpActionResult BarcodeSearch(string companyCode, string branchCode, string barcodeNo)
        {
            ErrorVM error = new ErrorVM();
            SalesEstDetailsVM barcodeDetails = new BarcodeBL().GetBarcodeSearch(companyCode, branchCode, barcodeNo, out error);
            if (error == null) {
                return Ok(barcodeDetails);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Barcode Information in Dotmatrix Print for View Barcode.
        /// </summary>
        /// <param name="barcode">SalesEstDetailsVM</param>
        /// <returns></returns>

        [HttpPost]
        [Route("BarcodePrint")]
        public IHttpActionResult BarcodePrint([FromBody]SalesEstDetailsVM barcode)
        {
            ErrorVM error = new ErrorVM();
            string print = new BarcodeBL().BarcodePrint(barcode, out error);
            if (error == null) {
                return Ok(print);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        #region Stock
        /// <summary>
        /// Get all stock Taken details.
        /// </summary>
        /// <param name="counterCode"></param>
        /// <param name="itemName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("StockTaking")]
        public IHttpActionResult StockTaking(string counterCode, string itemName)
        {
            List<KTTU_STOCK_TAKING> lstOfStock = new List<KTTU_STOCK_TAKING>();
            List<StockTakingVM> lstOfStockTakingVM = new List<StockTakingVM>();

            lstOfStock = db.KTTU_STOCK_TAKING.Where(s => s.counter_code == counterCode && s.item_name == itemName).ToList();
            if (lstOfStock != null && lstOfStock.Count > 0) {
                foreach (KTTU_STOCK_TAKING kst in lstOfStock) {
                    StockTakingVM st = new StockTakingVM();
                    st.CompanyCode = kst.company_code;
                    st.BranchCode = kst.branch_code;
                    st.ItemName = kst.item_name;
                    st.CounterCode = kst.counter_code;
                    st.Qty = kst.units;
                    st.GrossWt = kst.gwt;
                    st.NetWt = kst.nwt;
                    st.SalCode = kst.sal_code;
                    st.BarcodeNo = kst.barcode_no;
                    st.UpdateOn = kst.UpdateOn;
                    st.Dcts = kst.D_cts;
                    st.BatchNo = kst.batch_no;
                    lstOfStockTakingVM.Add(st);
                }
            }
            return Ok(lstOfStockTakingVM);
        }

        /// <summary>
        /// Get Stock information.
        /// </summary>
        /// <param name="couterCode"></param>
        /// <param name="itemName"></param>
        /// <param name="gsCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Stock")]
        public IHttpActionResult Stock(string couterCode, string itemName, string gsCode)
        {
            List<KTTU_BARCODE_MASTER> lstKBM = new List<KTTU_BARCODE_MASTER>();
            List<BarcodeMasterVM> lstBarcodeVM = new List<BarcodeMasterVM>();

            lstKBM = db.KTTU_BARCODE_MASTER.Where(b => b.counter_code == couterCode
                                                  && b.item_name == itemName
                                                  && b.gs_code == gsCode
                                                  && b.sold_flag == "N")
                                           .ToList();

            foreach (KTTU_BARCODE_MASTER kbm in lstKBM) {
                BarcodeMasterVM bmvm = new BarcodeMasterVM();
                bmvm.ObjID = kbm.obj_id;
                bmvm.CompanyCode = kbm.company_code;
                bmvm.BranchCode = kbm.branch_code;
                bmvm.BarcodeNo = kbm.barcode_no;
                bmvm.BatchNo = kbm.batch_no;
                bmvm.SalCode = kbm.sal_code;
                bmvm.OperatorCode = kbm.operator_code;
                bmvm.Date = kbm.date;
                bmvm.CounterCode = kbm.counter_code;
                bmvm.GSCode = kbm.gs_code;
                bmvm.ItemName = kbm.item_name;
                bmvm.Gwt = kbm.gwt;
                bmvm.Swt = kbm.swt;
                bmvm.Nwt = kbm.nwt;
                bmvm.Grade = kbm.grade;
                bmvm.CatalogID = kbm.catalog_id;
                bmvm.MakingChargePerRs = kbm.making_charge_per_rs;
                bmvm.WastPercent = kbm.wast_percent;
                bmvm.Qty = kbm.qty;
                bmvm.ItemSize = kbm.item_size;
                bmvm.DesignNo = kbm.design_no;
                bmvm.PieceRate = kbm.piece_rate;
                bmvm.DaimondAmount = kbm.daimond_amount;
                bmvm.StoneAmount = kbm.stone_amount;
                bmvm.OrderNo = kbm.order_no;
                bmvm.SoldFlag = kbm.sold_flag;
                bmvm.ProductCode = kbm.product_code;
                bmvm.HallmarkCharges = kbm.hallmark_charges;
                bmvm.Remarks = kbm.remarks;
                bmvm.SupplierCode = kbm.supplier_code;
                bmvm.OrderedCompanyCode = kbm.ordered_company_code;
                bmvm.OrderedBranchCode = kbm.ordered_branch_code;
                bmvm.Karat = kbm.karat;
                bmvm.McAmount = kbm.mc_amount;
                bmvm.WastageGrams = kbm.wastage_grms;
                bmvm.McPercent = kbm.mc_percent;
                bmvm.McType = kbm.mc_type;
                bmvm.OldBarcodeNo = kbm.old_barcode_no;
                bmvm.ProdIda = kbm.prod_ida;
                bmvm.ProdTagNo = kbm.prod_tagno;
                bmvm.UpdateOn = kbm.UpdateOn;
                bmvm.LotNo = kbm.Lot_No;
                bmvm.TagWt = kbm.tag_wt;
                bmvm.IsConfirmed = kbm.isConfirmed;
                bmvm.ConfirmedBy = kbm.confirmedBy;
                bmvm.ConfirmedDate = kbm.confirmedDate;
                bmvm.CurrentWt = kbm.current_wt;
                bmvm.MCFor = kbm.MC_For;
                bmvm.DiamondNo = kbm.diamond_no;
                bmvm.BatchID = kbm.batch_id;
                bmvm.AddWt = kbm.add_wt;
                bmvm.WeightRead = kbm.weightRead;
                bmvm.ConfirmedWeightRead = kbm.confirmedweightRead;
                bmvm.PartyName = kbm.party_name;
                bmvm.DesignName = kbm.design_name;
                bmvm.ItemSizeName = kbm.item_size_name;
                bmvm.MasterDesignCode = kbm.master_design_code;
                bmvm.MasterDesignName = kbm.master_design_name;
                bmvm.VendorModelNo = kbm.vendor_model_no;
                bmvm.PurMcGram = kbm.pur_mc_gram;
                bmvm.McPerPiece = kbm.mc_per_piece;
                bmvm.TaggingType = kbm.Tagging_Type;
                bmvm.BReceiptNo = kbm.BReceiptNo;
                bmvm.BSNo = kbm.BSNo;
                bmvm.IssueTo = kbm.Issue_To;
                bmvm.PurMcAmount = kbm.pur_mc_amount;
                bmvm.PurMcType = kbm.pur_mc_type;
                bmvm.PurRate = kbm.pur_rate;
                bmvm.SrBatchId = kbm.sr_batch_id;
                bmvm.TotalSellingMc = kbm.total_selling_mc;
                bmvm.PurDiamondAmount = kbm.pur_diamond_amount;
                bmvm.TotalPurchaseMc = kbm.total_purchase_mc;
                bmvm.PurStoneAmount = kbm.pur_stone_amount;
                bmvm.PurPurityPercentage = kbm.pur_purity_percentage;
                bmvm.PurWastageType = kbm.pur_wastage_type;
                bmvm.PurWastageTypeValue = kbm.pur_wastage_type_value;
                bmvm.CertificationNo = kbm.certification_no;
                bmvm.RefNo = kbm.ref_no;
                bmvm.ReceiptType = kbm.receipt_type;
                lstBarcodeVM.Add(bmvm);
            }
            return Ok(lstBarcodeVM);
        }
        #endregion

        [HttpGet]
        [Route("BarcodeAge/{companyCode}/{branchCode}/{barcodeNo}")]
        public IHttpActionResult GetBarcodeAge(string companyCode, string branchCode, string barcodeNo)
        {
            ErrorVM error = new ErrorVM();
            BarcodeAgeInfo barcodeAgeInfo = null;
            bool isSuccess = new BarcodeBL().GetBarcodeAge(companyCode, branchCode, barcodeNo, out barcodeAgeInfo, out error);
            if (isSuccess) {
                return Ok(barcodeAgeInfo);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        public IQueryable<SalesEstStoneVM> List()
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Count(ODataQueryOptions<SalesEstStoneVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Post(SalesEstStoneVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, SalesEstStoneVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Delete([FromBody] int id)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region User defined Methods
        protected string ValidateBarcodeDetails(string barcodeNo, string companyCode, string branchCode)
        {
            string Message = string.Empty;
            if (barcodeNo == "") {
                Message = "Invalid Tag No";
                return Message;
            }
            KTTU_BARCODE_MASTER barcode = db.KTTU_BARCODE_MASTER.Where(b => b.barcode_no == barcodeNo && b.company_code == companyCode && b.branch_code == branchCode).FirstOrDefault();
            if (Message == "" && barcode == null) {
                Message = "Barcode details not exist";
                return Message;
            }
            if (barcode.order_no != 0) {
                Message = "TagNo already attached to the order no " + barcode.order_no;
                return Message;
            }
            if (Message == "" && barcode.sold_flag == "Y") {
                Message = "TagNo: " + barcodeNo + " already Billed.";
                return Message;
            }
            if (Message == "" && barcode.order_no != 0) {
                KTTU_ORDER_MASTER kom = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == barcode.order_no && ord.company_code == companyCode && ord.branch_code == branchCode).FirstOrDefault();
                Message = "TagNo: " + barcodeNo + " has done reserved order to Customer: " + kom.cust_name + " on " + kom.order_date.ToString("dd/MM/yyyy");
                return Message;
            }
            return Message;
        }
        #endregion
    }
}