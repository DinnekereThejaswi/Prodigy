using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.BL.ViewModel.Payment;
using ProdigyAPI.BL.ViewModel.Repair;
using ProdigyAPI.BL.ViewModel.Sales;
using ProdigyAPI.Model.MagnaDb;
using ProdigyAPI.SIGlobals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Core.Objects;
using ProdigyAPI.BL.ViewModel.Master;

namespace ProdigyAPI.BL.BusinessLayer.Masters
{
    public class BarcodeBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Methods
        public SalesEstDetailsVM GetBarcodeDetWithCalculation(string companyCode, string branchCode, string barcodeNo, out ErrorVM error)
        {
            error = null;
            try {
                // Tried with this bellow procedure but throwing Error which is not cathable.
                //int validate = db.usp_ValidateBarcodeNo(barcodeNo, 0, Common.CompanyCode, Common.BranchCode);

                //Validation
                string message = ValidateBarcodeDetails(barcodeNo, companyCode, branchCode);
                if (message != "") {
                    error = new ErrorVM { index = 0, field = "", description = message, ErrorStatusCode = System.Net.HttpStatusCode.NotFound };
                    return null;
                }

                // If Valid
                ObjectParameter errorMsg = new ObjectParameter("errorMsg", typeof(string));
                var barcodeInfo = db.usp_getBarcodeInformation(companyCode, branchCode, barcodeNo, "0", 0, false, 0, 0, errorMsg).ToArray();
                if (errorMsg.Value.ToString() != "") {
                    error = new ErrorVM { index = 0, field = "", description = errorMsg.ToString() };
                    return null;
                }
                return new SalesEstDetailsVM()
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
                };
            }
            catch (Exception excp) {
                error.description = excp.Message;
                error.field = "Barcode";
                error.ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError;
                return null;
            }
        }

        public List<SalesEstStoneVM> GetBarcodeStoneInfo(string companyCode, string branchCode, string barcodeNo)
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
            return lstSalEstStone;
        }

        public SalesEstDetailsVM GetBarcodeWithStone(string companyCode, string branchCode, string barcodeNo, string orderNo, int isInterstate, int isOfferCoin, int skipVal, out ErrorVM error)
        {
            error = null;
            try {
                // Tried with this bellow procedure but throwing Error which is not cathable.
                //int validate = db.usp_ValidateBarcodeNo(barcodeNo, 0, Common.CompanyCode, Common.BranchCode);

                //Validation
                if (skipVal == 0) {
                    string message = ValidateBarcodeDetails(barcodeNo, companyCode, branchCode, Convert.ToInt32(orderNo));
                    if (message != "") {
                        error = new ErrorVM
                        {
                            index = 0,
                            field = "Barcode",
                            description = message,
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return null;
                    }

                    #region Group Barcode
                    // Validate barcode is group barcode or not
                    KTTU_BARCODE_MASTER barcodeMaster = db.KTTU_BARCODE_MASTER.Where(bar => bar.company_code == companyCode
                                                                                    && bar.branch_code == branchCode
                                                                                    && bar.barcode_no == barcodeNo
                                                                                    && bar.sold_flag == "N").FirstOrDefault();
                    if (barcodeMaster != null && barcodeMaster.Tagging_Type == "G") {
                        //Validate Group Barcode
                        if (barcodeMaster.qty == 0 && barcodeMaster.gwt == 0) {
                            error = new ErrorVM
                            {
                                index = 0,
                                field = "",
                                description = "Barcode No already Billed.",
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                            };
                            return null;
                        }
                        SalesEstDetailsVM barcode = new SalesEstDetailsVM();
                        barcode.CompanyCode = barcodeMaster.company_code;
                        barcode.BranchCode = barcodeMaster.branch_code;
                        barcode.BarcodeNo = barcodeMaster.barcode_no;
                        barcode.ItemName = barcodeMaster.item_name;
                        barcode.TaggingType = barcodeMaster.Tagging_Type;
                        barcode.GsCode = barcodeMaster.gs_code;
                        barcode.Karat = barcodeMaster.karat;
                        barcode.CounterCode = barcodeMaster.counter_code;
                        barcode.ItemQty = barcodeMaster.qty;
                        barcode.Grosswt = Convert.ToDecimal(barcodeMaster.gwt);
                        barcode.Netwt = Convert.ToDecimal(barcodeMaster.nwt);
                        barcode.Stonewt = Convert.ToDecimal(barcodeMaster.swt);
                        barcode.StoneCharges = Convert.ToDecimal(barcodeMaster.stone_amount);
                        barcode.DiamondCharges = barcodeMaster.daimond_amount;
                        barcode.McType = Convert.ToInt32(barcodeMaster.mc_type);
                        barcode.McPercent = barcodeMaster.mc_percent;
                        barcode.Rate = SIGlobals.Globals.GetRate(db, companyCode, branchCode, barcodeMaster.gs_code, barcodeMaster.karat);
                        barcode.GSTGroupCode = SIGlobals.Globals.GetGSTGroupCode(db, barcodeMaster.gs_code, companyCode, branchCode);
                        return barcode;
                    }
                    #endregion
                }
                // If Valid
                ObjectParameter errorMsg = new ObjectParameter("errorMsg", typeof(string));
                var barcodeInfo = db.usp_getBarcodeInformation(companyCode, branchCode, barcodeNo, orderNo == null ? "0" : orderNo, 0, isInterstate == 1 ? true : false, 0, 0, errorMsg).ToArray();
                if (errorMsg.Value.ToString() != "") {
                    error = new ErrorVM
                    { 
                        index = 0,
                        field = "",
                        description = errorMsg.ToString(),
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }

                // Taking Stone Details
                List<SalesEstStoneVM> lstSalEstStone = new List<SalesEstStoneVM>();
                ObjectParameter errorMsgStone = new ObjectParameter("errorMsg", typeof(string));
                var stoneInfo = db.usp_getBarcodeStoneInformation(companyCode, branchCode, barcodeNo, errorMsgStone).ToList();
                decimal totalStoneAmount = 0;
                decimal totalOnlyStoneAmount = 0;
                decimal totalOnlyDiamondAmount = 0;
                for (int i = 0; i < stoneInfo.Count(); i++) {
                    // Logic taken by magna code.
                    string dmName = stoneInfo[i].name;
                    string type = stoneInfo[i].type;
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
                    if (type == "D") {
                        totalOnlyDiamondAmount = totalOnlyDiamondAmount + ses.Amount;
                    }
                    else {
                        totalOnlyStoneAmount = totalOnlyStoneAmount + ses.Amount;
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
                //totalOnlyDiamondAmount = Math.Round(totalOnlyDiamondAmount, 0, MidpointRounding.ToEven);
                //totalOnlyStoneAmount = Math.Round(totalOnlyStoneAmount, 0, MidpointRounding.ToEven);
                return new SalesEstDetailsVM()
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
                    StoneCharges = totalOnlyStoneAmount,
                    DiamondCharges = totalOnlyDiamondAmount,
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
                    IsEDApplicable = isOfferCoin == 1 ? "O" : barcodeInfo.FirstOrDefault().isEDApplicable,
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
                    SGSTPercent = Convert.ToDecimal(barcodeInfo.FirstOrDefault().SGST_Percent),
                    SGSTAmount = Convert.ToDecimal(barcodeInfo.FirstOrDefault().SGST_Amount),
                    CGSTPercent = Convert.ToDecimal(barcodeInfo.FirstOrDefault().CGST_Percent),
                    CGSTAmount = Convert.ToDecimal(barcodeInfo.FirstOrDefault().CGST_Amount),
                    IGSTPercent = Convert.ToDecimal(barcodeInfo.FirstOrDefault().IGST_Percent),
                    IGSTAmount = Convert.ToDecimal(barcodeInfo.FirstOrDefault().IGST_Amount),
                    HSN = barcodeInfo.FirstOrDefault().HSN,
                    PieceRate = barcodeInfo.FirstOrDefault().Piece_Rate,
                    DeductSWt = barcodeInfo.FirstOrDefault().DeductSWt,
                    OrdDiscountAmt = barcodeInfo.FirstOrDefault().Ord_Discount_Amt,
                    DedCounter = barcodeInfo.FirstOrDefault().ded_counter,
                    DedItem = barcodeInfo.FirstOrDefault().ded_item,
                    isInterstate = isInterstate,
                    salesEstStoneVM = lstSalEstStone
                };
            }
            catch (Exception excp) {
                error = new ErrorVM
                {
                    index = 0,
                    field = "",
                    description = "Barcode doest not exist",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return null;
            }
        }

        public SalesEstDetailsVM GetBarcodeWithStoneForSales(string companyCode, string branchCode, string barcodeNo, string orderNo, out ErrorVM error)
        {
            error = null;
            try {
                ObjectParameter errorMsg = new ObjectParameter("errorMsg", typeof(string));
                var barcodeInfo = db.usp_getBarcodeInformation(companyCode, branchCode, barcodeNo, orderNo == "" ? "0" : orderNo, 0, false, 0, 0, errorMsg).ToArray();
                if (errorMsg.Value.ToString() != "") {
                    error = new ErrorVM { index = 0, field = "", description = errorMsg.ToString() };
                    return null;
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
                                                                                                && d.karat_from <= totalDiamondKarat
                                                                                                && d.company_code == companyCode
                                                                                                && d.branch_code == branchCode).OrderBy(d => d.karat_from).ToList();
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
                return new SalesEstDetailsVM()
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
                };
            }
            catch (Exception excp) {
                error.description = excp.Message;
                error.field = "Barcode";
                error.ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError;
                return null;
            }
        }

        public SalesEstDetailsVM BarcodeWithStoneForAddWt(string companyCode, string branchCode, string barcodeNo, string addWtBarcode, out ErrorVM error)
        {
            error = null;
            try {
                if (barcodeNo == addWtBarcode) {
                    error = new ErrorVM
                    {
                        index = 0,
                        field = "",
                        description = "Invalid barcode.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }
                //Validation
                string message = ValidateBarcodeDetails(barcodeNo, companyCode, branchCode);
                if (message != "") {
                    error = new ErrorVM
                    {
                        index = 0,
                        field = "",
                        description = message,
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }

                // If Valid
                ObjectParameter errorMsg = new ObjectParameter("errorMsg", typeof(string));
                var barcodeInfo = db.usp_getBarcodeInformation(companyCode, branchCode, barcodeNo, "0", 0, false, 0, 0, errorMsg).ToArray();
                if (errorMsg.Value.ToString() != "") {
                    error = new ErrorVM { index = 0, field = "", description = errorMsg.ToString() };
                    return null;
                }

                KTTU_BARCODE_MASTER barcode1 = db.KTTU_BARCODE_MASTER.Where(bar => bar.barcode_no == barcodeNo).FirstOrDefault();
                KTTU_BARCODE_MASTER barcode2 = db.KTTU_BARCODE_MASTER.Where(bar => bar.barcode_no == addWtBarcode).FirstOrDefault();

                if (barcode1.gs_code != barcode2.gs_code) {
                    error = new ErrorVM
                    {
                        index = 0,
                        field = "",
                        description = "Barcode No: " + addWtBarcode + " is of " + barcode2.gs_code + ", It cannot be added.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
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
                    List<KSTU_DIAMOND_RATE_MASTER> lstOfkdrm = db.KSTU_DIAMOND_RATE_MASTER.Where(d => d.dm_name == dmName
                                                                                                && d.karat_from <= totalDiamondKarat
                                                                                                && d.company_code == companyCode
                                                                                                && d.branch_code == branchCode).OrderBy(d => d.karat_from).ToList();
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
                return new SalesEstDetailsVM()
                {
                    ObjID = addBarcodeInfo.FirstOrDefault().obj_id,
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
                };
            }
            catch (Exception excp) {
                error.description = excp.Message;
                error.field = "Barcode";
                error.ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError;
                return null;
            }
        }

        public dynamic GetBarcodeSearch(string companyCode, string branchCode, string barcodeNo, out ErrorVM error)
        {
            error = null;
            try {
                BarcodeMasterVM barcode = db.KTTU_BARCODE_MASTER.Where(b => b.company_code == companyCode
                                                                    && b.branch_code == branchCode
                                                                    && b.barcode_no == barcodeNo).Select(b => new BarcodeMasterVM()
                                                                    {
                                                                        ObjID = b.obj_id,
                                                                        CompanyCode = b.company_code,
                                                                        BranchCode = b.branch_code,
                                                                        BarcodeNo = b.barcode_no,
                                                                        BatchNo = b.batch_no,
                                                                        SalCode = b.sal_code,
                                                                        OperatorCode = b.operator_code,
                                                                        Date = b.date,
                                                                        CounterCode = b.counter_code,
                                                                        GSCode = b.gs_code,
                                                                        ItemName = b.item_name,
                                                                        Gwt = b.gwt,
                                                                        Swt = b.swt,
                                                                        Nwt = b.nwt,
                                                                        Grade = b.grade,
                                                                        CatalogID = b.catalog_id,
                                                                        MakingChargePerRs = b.making_charge_per_rs,
                                                                        WastPercent = b.wast_percent,
                                                                        Qty = b.qty,
                                                                        ItemSize = b.item_size,
                                                                        DesignNo = b.design_no,
                                                                        PieceRate = b.piece_rate,
                                                                        DaimondAmount = b.daimond_amount,
                                                                        StoneAmount = b.stone_amount,
                                                                        OrderNo = b.order_no,
                                                                        SoldFlag = b.sold_flag,
                                                                        ProductCode = b.product_code,
                                                                        HallmarkCharges = b.hallmark_charges,
                                                                        Remarks = b.remarks,
                                                                        SupplierCode = b.supplier_code,
                                                                        OrderedCompanyCode = b.ordered_company_code,
                                                                        OrderedBranchCode = b.ordered_branch_code,
                                                                        Karat = b.karat,
                                                                        McAmount = b.mc_amount,
                                                                        WastageGrams = b.wastage_grms,
                                                                        McPercent = b.mc_percent,
                                                                        McType = b.mc_type,
                                                                        OldBarcodeNo = b.old_barcode_no,
                                                                        ProdIda = b.prod_ida,
                                                                        ProdTagNo = b.prod_tagno,
                                                                        UpdateOn = b.UpdateOn,
                                                                        LotNo = b.Lot_No,
                                                                        TagWt = b.tag_wt,
                                                                        IsConfirmed = b.isConfirmed,
                                                                        ConfirmedBy = b.confirmedBy,
                                                                        ConfirmedDate = b.confirmedDate,
                                                                        CurrentWt = b.current_wt,
                                                                        MCFor = b.MC_For,
                                                                        DiamondNo = b.diamond_no,
                                                                        BatchID = b.batch_id,
                                                                        AddWt = b.add_wt,
                                                                        WeightRead = b.weightRead,
                                                                        ConfirmedWeightRead = b.confirmedweightRead,
                                                                        PartyName = b.party_name,
                                                                        DesignName = b.design_name,
                                                                        ItemSizeName = b.item_size_name,
                                                                        MasterDesignCode = b.master_design_code,
                                                                        MasterDesignName = b.master_design_name,
                                                                        VendorModelNo = b.vendor_model_no,
                                                                        PurMcGram = b.pur_mc_gram,
                                                                        McPerPiece = b.mc_per_piece,
                                                                        TaggingType = b.Tagging_Type,
                                                                        BReceiptNo = b.BReceiptNo,
                                                                        BSNo = b.BSNo,
                                                                        IssueTo = b.Issue_To,
                                                                        PurMcAmount = b.pur_mc_amount,
                                                                        PurMcType = b.pur_mc_type,
                                                                        PurRate = b.pur_rate,
                                                                        SrBatchId = b.sr_batch_id,
                                                                        TotalSellingMc = b.total_selling_mc,
                                                                        PurDiamondAmount = b.pur_diamond_amount,
                                                                        TotalPurchaseMc = b.total_purchase_mc,
                                                                        PurStoneAmount = b.pur_stone_amount,
                                                                        PurPurityPercentage = b.pur_purity_percentage,
                                                                        PurWastageType = b.pur_wastage_type,
                                                                        PurWastageTypeValue = b.pur_wastage_type_value,
                                                                        CertificationNo = b.certification_no,
                                                                        RefNo = b.ref_no,
                                                                        ReceiptType = b.receipt_type
                                                                    }).FirstOrDefault();
                if (barcode == null) {
                    error = new ErrorVM()
                    {
                        description = "Invalid Barcode",
                        ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError
                    };
                    return null;
                }
                List<BarcodeStoneVM> barcodeStone = db.KTTU_BARCODE_STONE_DETAILS.Where(st => st.company_code == companyCode
                                                                                                && st.branch_code == branchCode
                                                                                                && st.barcode_no == barcodeNo).Select(st => new BarcodeStoneVM()
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
                                                                                                }).ToList();

                barcode.BarcodeStoneDetails = barcodeStone;

                //TODO: Need work on that bellow functionality.
                barcode.BarcodeTransactionDetails = null;

                return null;
            }
            catch (Exception excp) {
                error = new ErrorVM()
                {
                    description = excp.Message,
                    ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError
                };
                return null;
            }
        }

        public string BarcodePrint(SalesEstDetailsVM barcode, out ErrorVM error)
        {
            error = null;
            try {
                StringBuilder sb = new StringBuilder();
                int width = 92;
                StringBuilder strdoubleTransLine = new StringBuilder();
                strdoubleTransLine.Append('=', width);
                StringBuilder strTransLine = new StringBuilder();
                strTransLine.Append('-', width);
                decimal Spaces = 0M;
                StringBuilder strSpaces = new StringBuilder();
                Spaces = ((width - strTransLine.Length) / 3);
                string esc = Convert.ToChar(27).ToString();
                sb.Append(esc + Convert.ToChar(14).ToString() + Convert.ToChar(14).ToString());
                sb.Append(esc + Convert.ToChar(64).ToString());
                sb.Append(esc + Convert.ToChar(77).ToString());
                sb.AppendLine(string.Format("{0,-10} {1,5}", barcode.CounterCode, barcode.ItemName));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));
                if (Convert.ToDecimal(barcode.CESSAmount) > 0) {
                    sb.AppendLine(string.Format("{0,-5}{1,8}{2,8}{3,8}{4,8}{5,8}{6,10}{7,10}{8,8}{9,8}{10,10}",
                        "Gr.Wt", "St.Wt", "N.Wt", "St.Amt", "VA%", "VA", "Gld Amt", "Cess", "S+CGST", "IGST", " Total "));
                }
                else {
                    sb.AppendLine(string.Format("{0,-5}{1,8}{2,8}{3,8}{4,8}{5,10}{6,8}{7,10}{8,8}{9,8}{10,10}",
                        "Gr.Wt", "St.Wt", "N.Wt", "St.Amt", "VA%", "Wast.Wt", "VA", "Gld Amt", "S+CGST", "IGST", " Total "));
                }
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));
                sb.Append(string.Format("{0,-5}", barcode.Grosswt));
                sb.Append(string.Format("{0,8}", barcode.Stonewt));
                sb.Append(string.Format("{0,8}", barcode.Netwt));
                sb.Append(string.Format("{0,8}", Convert.ToDecimal(Convert.ToDecimal(barcode.StoneCharges) + Convert.ToDecimal(barcode.DiamondCharges)).ToString("0.00")));
                sb.Append(string.Format("{0,8}", Convert.ToDecimal(barcode.McPercent).ToString("0.00")));
                if (barcode.CESSAmount == null || barcode.CESSAmount == 0) {
                    sb.Append(string.Format("{0,8}", barcode.WastageGrms));
                }
                sb.Append(string.Format("{0,10}", barcode.VaAmount));
                sb.Append(string.Format("{0,10}", barcode.GoldValue));
                if (Convert.ToDecimal(barcode.CESSAmount) > 0) {
                    sb.Append(string.Format("{0,8}", barcode.CESSAmount));
                }
                sb.Append(string.Format("{0,8}", barcode.SGSTAmount + barcode.CGSTAmount));
                sb.Append(string.Format("{0,8}", barcode.IGSTAmount));
                sb.Append(string.Format("{0,10}", barcode.ItemFinalAmount));
                sb.AppendLine();
                sb.AppendLine();
                sb.Append(Convert.ToChar(18).ToString());
                sb.Append(esc + Convert.ToChar(64).ToString());
                return sb.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM()
                {
                    description = excp.Message,
                    ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError
                };
                return "";
            }
        }

        public bool GetBarcodeAge(string companyCode, string branchCode, string barcodeNo, out BarcodeAgeInfo barcodeAgeInfo, out ErrorVM error)
        {
            error = null;
            barcodeAgeInfo = null;
            var barcodeData = db.KTTU_BARCODE_MASTER.Where(x => x.company_code == companyCode
                && x.branch_code == branchCode && x.sold_flag != "Y"
                && (x.barcode_no == barcodeNo || x.old_barcode_no == barcodeNo)).FirstOrDefault();
            if (barcodeData != null) {
                barcodeAgeInfo = new BarcodeAgeInfo();
                barcodeAgeInfo.BarcodeNo = barcodeNo;
                DateTime applicationDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode).Date;
                var days = Convert.ToInt32((applicationDate.Date - barcodeData.date).Value.TotalDays);
                barcodeAgeInfo.AgeText = "B " + days.ToString();
                barcodeAgeInfo.AgeInDays = days;
                barcodeAgeInfo.ItemAge = days.ToString() + " - Age (Product)";
                barcodeAgeInfo.AgeInBranch = days.ToString() + " - Age (In Branch)";
                if (days < 50) {
                    barcodeAgeInfo.ForegroundColour = "White";
                    barcodeAgeInfo.BackgroundColour = "Green";
                }
                else if (days >= 50 && days < 100) {
                    barcodeAgeInfo.ForegroundColour = "Black";
                    barcodeAgeInfo.BackgroundColour = "Yellow";
                }
                else {
                    barcodeAgeInfo.ForegroundColour = "White";
                    barcodeAgeInfo.BackgroundColour = "Red";
                }

                var barcodeReceiptLine = db.KTTU_BARCODED_RECEIPT_DETAILS.Where(x => x.company_code == companyCode
                    && x.branch_code == branchCode && x.cflag != "Y"
                    && x.barcode_no == barcodeNo).FirstOrDefault();
                if (barcodeReceiptLine != null) {
                    var ageInStore = Convert.ToInt32((applicationDate.Date - barcodeReceiptLine.barcode_date).Value.TotalDays);
                    barcodeAgeInfo.AgeInBranch = ageInStore.ToString() + " - Age (In Branch)";
                }

            }
            return true;
        }

        #endregion

        #region Private Methods
        public string ValidateBarcodeDetails(string barcodeNo, string companyCode, string branchCode)
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
            if (Message == "" && barcode.sold_flag == "Y" && barcode.ExitDocType == "BI") {
                Message = "TagNo: " + barcodeNo + " already Issued.";
                return Message;
            }
            if (Message == "" && barcode.sold_flag == "Y" && barcode.ExitDocType == "S") {
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

        public string ValidateBarcodeDetails(string barcodeNo, string companyCode, string branchCode, int orderNo)
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
                if (barcode.order_no != orderNo) {
                    Message = "TagNo already attached to the order no " + barcode.order_no;
                    return Message;
                }
            }
            if (Message == "" && barcode.sold_flag == "Y" && barcode.ExitDocType == "BI") {
                Message = "TagNo: " + barcodeNo + " already Issued.";
                return Message;
            }
            if (Message == "" && barcode.sold_flag == "Y" && barcode.ExitDocType == "S") {
                Message = "TagNo: " + barcodeNo + " already Billed.";
                return Message;
            }
            if (Message == "" && barcode.order_no != 0) {
                if (barcode.order_no != orderNo) {
                    KTTU_ORDER_MASTER kom = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == barcode.order_no && ord.company_code == companyCode && ord.branch_code == branchCode).FirstOrDefault();
                    Message = "TagNo: " + barcodeNo + " has done reserved order to Customer: " + kom.cust_name + " on " + kom.order_date.ToString("dd/MM/yyyy");
                    return Message;
                }
            }
            return Message;
        }

        public string ValidateBarcodeDetails(string barcodeNo, string companyCode, string branchCode, List<int> orderNo)
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
            if (Message == "" && barcode.sold_flag == "Y" && barcode.ExitDocType == "BI") {
                Message = "TagNo: " + barcodeNo + " already Issued.";
                return Message;
            }
            if (Message == "" && barcode.sold_flag == "Y" && barcode.ExitDocType == "S") {
                Message = "TagNo: " + barcodeNo + " already Billed.";
                return Message;
            }
            return Message;
        }
        #endregion
    }
}
