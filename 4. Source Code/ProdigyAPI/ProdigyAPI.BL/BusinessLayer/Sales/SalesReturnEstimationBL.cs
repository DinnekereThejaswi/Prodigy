using NumberToWordsINR;
using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.BL.BusinessLayer.PrintConfig;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.BL.ViewModel.Payment;
using ProdigyAPI.BL.ViewModel.Print;
using ProdigyAPI.BL.ViewModel.Sales;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Sales
{
    public class SalesReturnEstimationBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        string MODULE_SEQ_NO = "3";
        private const string TABLE_NAME = "KTTU_SR_MASTER";
        #endregion

        #region Sales Return

        public bool ValidateSalesReturnEstimation(int estNo, string companyCode, string branchCode, string type, int refNo, out ErrorVM error)
        {
            // Alternate for this [dbo].[usp_ValidateSREstNo] Procedure
            error = null;
            if (estNo > 0) {
                string strEstNo = Convert.ToString(estNo);
                KTTU_SR_MASTER srMaster = db.KTTU_SR_MASTER.Where(sr => sr.est_no == estNo
                                                                                    && sr.company_code == companyCode
                                                                                    && sr.branch_code == branchCode).FirstOrDefault();
                if (srMaster == null) {
                    error.description = string.Format("SR Est. No: {0} is invalid.", estNo);
                    error.ErrorStatusCode = System.Net.HttpStatusCode.NotFound;
                    return false;
                }

                if (srMaster.sales_bill_no != 0) {
                    error.description = string.Format("'SR Est. No: {0} is already billed. Bill No:{0} ", estNo, srMaster.sales_bill_no);
                    error.ErrorStatusCode = System.Net.HttpStatusCode.NotFound;
                    return false;
                }

                if (type == "RE") {
                    var payments = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.Ref_BillNo == strEstNo
                                                                           && pay.company_code == companyCode
                                                                           && pay.branch_code == branchCode
                                                                           && pay.trans_type == "A"
                                                                           && pay.pay_mode == "SE"
                                                                           && pay.cflag == "N").FirstOrDefault();
                    if (payments.series_no != 0 && srMaster.est_no != refNo) {
                        error.description = string.Format("'sr Est. No: {0} is already is already adjusted towards Sales Est. No:{1} ", estNo, payments.series_no);
                        error.ErrorStatusCode = System.Net.HttpStatusCode.NotFound;
                        return false;
                    }
                }
            }
            return true;
        }
        public dynamic GetBilledBrach(string companyCode, string branchCode)
        {
            var data = db.KSTU_SUPPLIER_MASTER.Where(ksm => ksm.obj_status == "O"
             && ksm.company_code == companyCode && ksm.branch_code == branchCode && ksm.voucher_code == "VB" && ksm.company_code == companyCode
                                                         && ksm.branch_code == branchCode).Select(ksm => new BilledBranchVM()
                                                         {
                                                             PartyCode = ksm.party_code,
                                                             PartyName = ksm.party_name
                                                         });
            return data;
        }

        public SalesReturnMasterVM GetSalesReturnEstimationDetails(string companyCode, string branchCode, int billNo, out ErrorVM error)
        {
            error = null;
            try {
                KTTU_SALES_MASTER ksm = db.KTTU_SALES_MASTER.Where(sale => sale.bill_no == billNo && sale.company_code == companyCode
                                                            && sale.branch_code == branchCode).FirstOrDefault();
                if (ksm == null) {
                    error = new ErrorVM
                    {
                        index = 0,
                        field = "",
                        description = "Invalid Bill Number.",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                    return null;
                }
                SalesReturnMasterVM srvm = new SalesReturnMasterVM();
                srvm.ObjID = ksm.obj_id;
                srvm.CompanyCode = ksm.company_code;
                srvm.BranchCode = ksm.branch_code;
                srvm.VotureBillNo = ksm.bill_no;
                srvm.SalesBillNo = 0;
                srvm.EstNo = 0;
                srvm.CustomerID = Convert.ToInt32(ksm.cust_Id);
                srvm.BillDate = ksm.bill_date;
                srvm.SalesReturnDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                srvm.GSType = ksm.gstype;
                srvm.TaxAmount = ksm.total_tax_amount;
                srvm.TotalSRAmount = Convert.ToDecimal(ksm.grand_total);
                srvm.OperatorCode = ksm.operator_code;
                srvm.SalCode = "";
                srvm.BillCounter = ksm.bill_counter;
                srvm.Remarks = ksm.remarks;
                srvm.UpdateOn = ksm.UpdateOn;
                srvm.ISIns = ksm.is_ins;
                srvm.Rate = ksm.rate;
                srvm.BilledBranch = "";
                srvm.CFlag = ksm.cflag;
                srvm.ExciseDutyPercent = ksm.excise_duty_percent;
                srvm.EDCessPercent = ksm.ed_cess_percent;
                srvm.HEDCessPercent = ksm.hed_cess_percent;
                srvm.ExciseDutyAmount = ksm.excise_duty_amount;
                srvm.EDCessAmount = ksm.ed_cess;
                srvm.HEDCessAmount = ksm.hed_cess;
                srvm.DiscountAmount = ksm.discount_amount;
                srvm.IsAdjusted = ksm.item_set;
                srvm.CancelledBy = ksm.cancelled_by;
                srvm.CancelledRemarks = ksm.cancelled_remarks;
                srvm.NewBillNo = "0";
                srvm.RoundOff = ksm.round_off;
                srvm.ShiftID = ksm.ShiftID;
                srvm.InvoiceTypeID = 0;
                srvm.TotalCSTAmount = 0;
                srvm.VPAmount = 0;
                srvm.PayableAmount = 0;

                List<SalesReturnDetailsVM> lstSRD = new List<SalesReturnDetailsVM>();
                List<KTTU_SALES_DETAILS> ksd = db.KTTU_SALES_DETAILS.Where(sale => sale.bill_no == billNo
                                                                            && sale.company_code == companyCode
                                                                            && sale.branch_code == branchCode).ToList();
                foreach (KTTU_SALES_DETAILS ksdb in ksd) {
                    SalesReturnDetailsVM srd = new SalesReturnDetailsVM();
                    srd.ObjID = ksdb.obj_id;
                    srd.CompanyCode = ksdb.company_code;
                    srd.BranchCode = ksdb.branch_code;
                    srd.SalesBillNo = 0;
                    srd.SlNo = ksdb.sl_no;
                    srd.EstNo = 0;
                    srd.SalCode = ksdb.sal_code;
                    srd.ItemName = ksdb.item_name;
                    srd.CounterCode = ksdb.counter_code;
                    srd.Quantity = ksdb.item_no;
                    srd.GrossWt = Convert.ToDecimal(ksdb.gwt);
                    srd.StoneWt = ksdb.swt;
                    srd.NetWt = ksdb.nwt;
                    srd.WastePercent = ksdb.wast_percent;
                    srd.MakingChargePerRs = ksdb.making_charge_per_rs;
                    srd.VAAmount = ksdb.va_amount;
                    srd.SRAmount = ksdb.gold_value;
                    srd.StoneCharges = ksdb.stone_charges;
                    srd.DiamondCharges = ksdb.diamond_charges;
                    srd.NetAmount = ksdb.total_amount;
                    srd.AddQty = ksdb.Addqty;
                    srd.AddWt = ksdb.AddWt;
                    srd.DeductQty = ksdb.Deductqty;
                    srd.DeductWt = ksdb.DeductWt;
                    srd.UpdateOn = ksdb.UpdateOn;
                    srd.VAPecent = ksdb.mc_percent;
                    srd.GSCode = ksdb.gs_code;
                    srd.FinYear = ksdb.Fin_Year;
                    srd.BarcodeNo = ksdb.barcode_no;
                    srd.SupplierCode = ksdb.supplier_code;
                    srd.ItemAdditionalDiscount = ksdb.item_additional_discount;
                    srd.ItemTotalAfterDiscount = ksdb.item_total_after_discount;
                    srd.TaxPercentage = ksdb.tax_percentage;
                    srd.TaxAmount = ksdb.tax_amount;
                    srd.ItemSize = ksdb.item_size;
                    srd.ImageID = ksdb.img_id;
                    srd.DesignCode = ksdb.design_code;
                    srd.DesignName = ksdb.design_name;
                    srd.BatchID = ksdb.batch_id;
                    srd.RFID = ksdb.rf_id;
                    srd.MCPerPiece = ksdb.mc_per_piece;
                    srd.OfferValue = ksdb.offer_value;
                    srd.ItemFinalAmount = ksdb.item_final_amount;
                    srd.MCType = ksdb.mc_type;
                    srd.RoundOff = ksdb.round_off;
                    srd.ItemFinalAmountAfterRoundOff = ksdb.item_final_amount_after_roundoff;
                    srd.OriginalSalesBillNo = null;
                    srd.ItemType = ksdb.item_type;
                    srd.CSTAmount = 0;
                    srd.Dcts = 0;
                    srd.GSTGroupCode = ksdb.GSTGroupCode;
                    srd.SGSTPercent = ksdb.SGST_Percent;
                    srd.SGSTAmount = ksdb.SGST_Amount;
                    srd.CGSTAmount = ksdb.CGST_Amount;
                    srd.CGSTPercent = ksdb.CGST_Percent;
                    srd.IGSTAmount = ksdb.IGST_Amount;
                    srd.IGSTPercent = ksdb.IGST_Percent;
                    srd.HSN = ksdb.HSN;
                    srd.DiscountMc = ksdb.Discount_Mc;
                    srd.McDiscountAmt = ksdb.Mc_Discount_Amt;

                    List<SalesReturnStoneDetailsVM> lstSRStone = new List<SalesReturnStoneDetailsVM>();
                    List<KTTU_SALES_STONE_DETAILS> kssd = db.KTTU_SALES_STONE_DETAILS.Where(sale => sale.bill_no == billNo
                                                                                            && sale.barcode_no == srd.BarcodeNo
                                                                                            && sale.company_code == companyCode
                                                                                            && sale.branch_code == branchCode).ToList();
                    foreach (KTTU_SALES_STONE_DETAILS std in kssd) {
                        SalesReturnStoneDetailsVM ses = new SalesReturnStoneDetailsVM();
                        ses.ObjID = std.obj_id;
                        ses.CompanyCode = std.company_code;
                        ses.BranchCode = std.branch_code;
                        ses.SalesBillNo = std.bill_no;
                        ses.Sno = std.sl_no;
                        ses.EstNo = std.est_no;
                        ses.BarcodeNo = std.barcode_no;
                        ses.Type = std.type;
                        ses.Name = std.name;
                        ses.Qty = std.qty;
                        ses.Carrat = std.carrat;
                        ses.StoneWt = std.stone_wt;
                        ses.Rate = Convert.ToDecimal(std.rate);
                        ses.Amount = std.amount;
                        ses.UpdateOn = std.UpdateOn;
                        ses.FinYear = std.Fin_Year;
                        ses.BarcodeNo = std.barcode_no;
                        ses.UniqRowID = std.UniqRowID;
                        lstSRStone.Add(ses);
                    }
                    srd.lstOfStoneDetails = lstSRStone;
                    lstSRD.Add(srd);
                }
                srvm.lstOfSalesReturnDetails = lstSRD;
                return srvm;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public SalesReturnMasterVM GetAttachedSalesReturnEstimationDetailsForPrint(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            SalesReturnMasterVM srvm = new SalesReturnMasterVM();
            try {
                List<KTTU_PAYMENT_DETAILS> payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == estNo
                                                                                    && pay.pay_mode == "SE"
                                                                                    && pay.cflag == "N"
                                                                                    && pay.company_code == companyCode
                                                                                    && pay.branch_code == branchCode).ToList();
                List<SalesReturnDetailsVM> lstSRD = new List<SalesReturnDetailsVM>();
                foreach (KTTU_PAYMENT_DETAILS pay in payment) {
                    int srEstNo = Convert.ToInt32(pay.Ref_BillNo);
                    KTTU_SR_MASTER ksm = db.KTTU_SR_MASTER.Where(sale => sale.est_no == srEstNo && sale.company_code == companyCode
                                                        && sale.branch_code == branchCode).FirstOrDefault();

                    srvm.ObjID = ksm.obj_id;
                    srvm.CompanyCode = ksm.company_code;
                    srvm.BranchCode = ksm.branch_code;
                    srvm.VotureBillNo = ksm.voture_bill_no;
                    srvm.SalesBillNo = ksm.sales_bill_no;
                    srvm.EstNo = ksm.est_no;
                    srvm.CustomerID = ksm.cust_id;
                    srvm.BillDate = ksm.bill_date;
                    srvm.SalesReturnDate = ksm.sr_date;
                    srvm.GSType = ksm.gs_type;
                    srvm.TaxAmount = ksm.tax_amt;
                    srvm.TotalSRAmount = ksm.total_sramount;
                    srvm.OperatorCode = ksm.operator_code;
                    srvm.SalCode = ksm.sal_code;
                    srvm.BillCounter = ksm.bill_counter;
                    srvm.Remarks = ksm.remarks;
                    srvm.UpdateOn = ksm.UpdateOn;
                    srvm.ISIns = ksm.is_ins;
                    srvm.Rate = ksm.rate;
                    srvm.BilledBranch = ksm.billed_branch;
                    srvm.CFlag = ksm.cflag;
                    srvm.ExciseDutyPercent = ksm.excise_duty_percent;
                    srvm.EDCessPercent = ksm.ed_cess_percent;
                    srvm.HEDCessPercent = ksm.hed_cess_percent;
                    srvm.ExciseDutyAmount = ksm.excise_duty_amount;
                    srvm.EDCessAmount = ksm.ed_cess_amount;
                    srvm.HEDCessAmount = ksm.hed_cess_amount;
                    srvm.DiscountAmount = ksm.discount_amt;
                    srvm.IsAdjusted = ksm.is_adjusted;
                    srvm.CancelledBy = ksm.cancelled_by;
                    srvm.CancelledRemarks = ksm.cancelled_remarks;
                    srvm.NewBillNo = ksm.New_Bill_No;
                    srvm.RoundOff = ksm.round_off;
                    srvm.ShiftID = ksm.ShiftID;
                    srvm.InvoiceTypeID = ksm.invoice_type_id;
                    srvm.TotalCSTAmount = ksm.total_cst_amount;
                    srvm.VPAmount = ksm.vp_amt;
                    srvm.PayableAmount = ksm.payable_amt;


                    List<KTTU_SR_DETAILS> ksd = db.KTTU_SR_DETAILS.Where(sale => sale.est_no == srEstNo
                                                                            && sale.company_code == companyCode
                                                                            && sale.branch_code == branchCode).ToList();
                    foreach (KTTU_SR_DETAILS ksdb in ksd) {
                        SalesReturnDetailsVM srd = new SalesReturnDetailsVM();
                        srd.ObjID = ksdb.obj_id;
                        srd.CompanyCode = ksdb.company_code;
                        srd.BranchCode = ksdb.branch_code;
                        srd.SalesBillNo = ksdb.sales_bill_no;
                        srd.SlNo = ksdb.sl_no;
                        srd.EstNo = ksdb.est_no;
                        srd.SalCode = ksdb.sal_code;
                        srd.ItemName = ksdb.item_name;
                        srd.CounterCode = ksdb.counter_code;
                        srd.Quantity = ksdb.quantity;
                        srd.GrossWt = Convert.ToDecimal(ksdb.gwt);
                        srd.StoneWt = ksdb.swt;
                        srd.NetWt = ksdb.nwt;
                        srd.WastePercent = ksdb.wast_percent;
                        srd.MakingChargePerRs = ksdb.making_charge_per_rs;
                        srd.VAAmount = ksdb.va_amount;
                        srd.SRAmount = ksdb.sr_amount;
                        srd.StoneCharges = ksdb.stone_charges;
                        srd.DiamondCharges = ksdb.diamond_charges;
                        srd.NetAmount = ksdb.net_amount;
                        srd.AddQty = ksdb.Addqty;
                        srd.AddWt = ksdb.AddWt;
                        srd.DeductQty = ksdb.Deductqty;
                        srd.DeductWt = ksdb.DeductWt;
                        srd.UpdateOn = ksdb.UpdateOn;
                        srd.VAPecent = ksdb.va_percent;
                        srd.GSCode = ksdb.gs_code;
                        srd.FinYear = ksdb.Fin_Year;
                        srd.BarcodeNo = ksdb.barcode_no;
                        srd.SupplierCode = ksdb.supplier_code;
                        srd.ItemAdditionalDiscount = ksdb.item_additional_discount;
                        srd.ItemTotalAfterDiscount = ksdb.item_total_after_discount;
                        srd.TaxPercentage = ksdb.tax_percentage;
                        srd.TaxAmount = ksdb.tax_amount;
                        srd.ItemSize = ksdb.item_size;
                        srd.ImageID = ksdb.img_id;
                        srd.DesignCode = ksdb.design_code;
                        srd.DesignName = ksdb.design_name;
                        srd.BatchID = ksdb.batch_id;
                        srd.RFID = ksdb.rf_id;
                        srd.MCPerPiece = ksdb.mc_per_piece;
                        srd.OfferValue = ksdb.offer_value;
                        srd.ItemFinalAmount = ksdb.item_final_amount;
                        srd.MCType = ksdb.mc_type;
                        srd.RoundOff = ksdb.round_off;
                        srd.ItemFinalAmountAfterRoundOff = ksdb.item_final_amount_after_roundoff;
                        srd.OriginalSalesBillNo = null;
                        srd.ItemType = ksdb.item_type;
                        srd.CSTAmount = ksdb.cst_amount;
                        srd.Dcts = ksdb.Dcts;
                        srd.GSTGroupCode = ksdb.GSTGroupCode;
                        srd.SGSTPercent = ksdb.SGST_Percent;
                        srd.SGSTAmount = ksdb.SGST_Amount;
                        srd.CGSTAmount = ksdb.CGST_Amount;
                        srd.CGSTPercent = ksdb.CGST_Percent;
                        srd.IGSTAmount = ksdb.IGST_Amount;
                        srd.IGSTPercent = ksdb.IGST_Percent;
                        srd.HSN = ksdb.HSN;
                        srd.DiscountMc = ksdb.Discount_Mc;
                        srd.McDiscountAmt = ksdb.Mc_Discount_Amt;
                        srd.lstOfStoneDetails = null;
                        lstSRD.Add(srd);
                    }
                    srvm.lstOfSalesReturnDetails = lstSRD;
                }
                return srvm;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public SalesReturnMasterVM GetAttachedSalesReturnEstimationDetailsTotalForPrint(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            SalesReturnMasterVM srvm = new SalesReturnMasterVM();
            SalesReturnDetailsVM totalSR = new SalesReturnDetailsVM();
            try {
                List<KTTU_PAYMENT_DETAILS> payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == estNo
                                                                                    && pay.pay_mode == "SE"
                                                                                    && pay.cflag == "N"
                                                                                    && pay.company_code == companyCode
                                                                                    && pay.branch_code == branchCode).ToList();

                foreach (KTTU_PAYMENT_DETAILS pay in payment) {
                    int srEstNo = Convert.ToInt32(pay.Ref_BillNo);
                    KTTU_SR_MASTER ksm = db.KTTU_SR_MASTER.Where(sale => sale.est_no == srEstNo && sale.company_code == companyCode
                                                        && sale.branch_code == branchCode).FirstOrDefault();

                    srvm.ObjID = ksm.obj_id;
                    srvm.CompanyCode = ksm.company_code;
                    srvm.BranchCode = ksm.branch_code;
                    srvm.VotureBillNo = ksm.voture_bill_no;
                    srvm.SalesBillNo = ksm.sales_bill_no;
                    srvm.EstNo = ksm.est_no;
                    srvm.CustomerID = ksm.cust_id;
                    srvm.BillDate = ksm.bill_date;
                    srvm.SalesReturnDate = ksm.sr_date;
                    srvm.GSType = ksm.gs_type;
                    srvm.TaxAmount = ksm.tax_amt;
                    srvm.TotalSRAmount = ksm.total_sramount;
                    srvm.OperatorCode = ksm.operator_code;
                    srvm.SalCode = ksm.sal_code;
                    srvm.BillCounter = ksm.bill_counter;
                    srvm.Remarks = ksm.remarks;
                    srvm.UpdateOn = ksm.UpdateOn;
                    srvm.ISIns = ksm.is_ins;
                    srvm.Rate = ksm.rate;
                    srvm.BilledBranch = ksm.billed_branch;
                    srvm.CFlag = ksm.cflag;
                    srvm.ExciseDutyPercent = ksm.excise_duty_percent;
                    srvm.EDCessPercent = ksm.ed_cess_percent;
                    srvm.HEDCessPercent = ksm.hed_cess_percent;
                    srvm.ExciseDutyAmount = ksm.excise_duty_amount;
                    srvm.EDCessAmount = ksm.ed_cess_amount;
                    srvm.HEDCessAmount = ksm.hed_cess_amount;
                    srvm.DiscountAmount = ksm.discount_amt;
                    srvm.IsAdjusted = ksm.is_adjusted;
                    srvm.CancelledBy = ksm.cancelled_by;
                    srvm.CancelledRemarks = ksm.cancelled_remarks;
                    srvm.NewBillNo = ksm.New_Bill_No;
                    srvm.RoundOff = ksm.round_off;
                    srvm.ShiftID = ksm.ShiftID;
                    srvm.InvoiceTypeID = ksm.invoice_type_id;
                    srvm.TotalCSTAmount = ksm.total_cst_amount;
                    srvm.VPAmount = ksm.vp_amt;
                    srvm.PayableAmount = ksm.payable_amt;

                    List<SalesReturnDetailsVM> lstSRD = new List<SalesReturnDetailsVM>();
                    List<KTTU_SR_DETAILS> ksd = db.KTTU_SR_DETAILS.Where(sale => sale.est_no == srEstNo
                                                                         && sale.company_code == companyCode
                                                                         && sale.branch_code == branchCode).ToList();
                    foreach (KTTU_SR_DETAILS ksdb in ksd) {
                        SalesReturnDetailsVM srd = new SalesReturnDetailsVM();
                        totalSR.Quantity = totalSR.Quantity + ksdb.quantity;
                        totalSR.GrossWt = totalSR.GrossWt + Convert.ToDecimal(ksdb.gwt);
                        totalSR.StoneWt = totalSR.StoneWt + ksdb.swt;
                        totalSR.NetWt = totalSR.NetWt + ksdb.nwt;
                        totalSR.VAAmount = totalSR.VAAmount + ksdb.va_amount;
                        totalSR.SRAmount = totalSR.SRAmount + ksdb.sr_amount;
                        totalSR.StoneCharges = Convert.ToDecimal(totalSR.StoneCharges) + ksdb.stone_charges;
                        totalSR.DiamondCharges = Convert.ToDecimal(totalSR.DiamondCharges) + ksdb.diamond_charges;
                        totalSR.NetAmount = totalSR.NetAmount + ksdb.net_amount;
                        totalSR.ItemTotalAfterDiscount = Convert.ToDecimal(totalSR.ItemTotalAfterDiscount) + ksdb.item_total_after_discount;
                        totalSR.ItemFinalAmount = Convert.ToDecimal(totalSR.ItemFinalAmount) + ksdb.item_final_amount;
                        totalSR.RoundOff = Convert.ToDecimal(totalSR.RoundOff) + ksdb.round_off;
                        totalSR.ItemFinalAmountAfterRoundOff = Convert.ToDecimal(totalSR.ItemFinalAmountAfterRoundOff) + ksdb.item_final_amount_after_roundoff;
                        totalSR.SGSTPercent = Convert.ToDecimal(ksdb.SGST_Percent);
                        totalSR.SGSTAmount = Convert.ToDecimal(totalSR.SGSTAmount) + ksdb.SGST_Amount;
                        totalSR.CGSTPercent = Convert.ToDecimal(ksdb.CGST_Percent);
                        totalSR.CGSTAmount = Convert.ToDecimal(totalSR.CGSTAmount) + ksdb.CGST_Amount;
                        totalSR.IGSTPercent = Convert.ToDecimal(ksdb.IGST_Percent);
                        totalSR.IGSTAmount = Convert.ToDecimal(totalSR.IGSTAmount) + ksdb.IGST_Amount;
                    }
                    totalSR.ItemFinalAmount = Math.Round(Convert.ToDecimal(totalSR.ItemFinalAmount), 0, MidpointRounding.ToEven);
                    lstSRD.Add(totalSR);
                    srvm.lstOfSalesReturnDetails = lstSRD;
                }
                return srvm;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public SalesReturnMasterVM GetSavedSRInfo(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            if (estNo == 0) {
                error = new ErrorVM()
                {
                    description = "Invalid Bill No",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return null;
            }
            try {
                KTTU_SR_MASTER ksm = db.KTTU_SR_MASTER.Where(sale => sale.est_no == estNo
                                                            && sale.company_code == companyCode
                                                            && sale.branch_code == branchCode).FirstOrDefault();
                SalesReturnMasterVM srvm = new SalesReturnMasterVM();
                srvm.ObjID = ksm.obj_id;
                srvm.CompanyCode = ksm.company_code;
                srvm.BranchCode = ksm.branch_code;
                srvm.VotureBillNo = ksm.voture_bill_no;
                srvm.SalesBillNo = ksm.sales_bill_no;
                srvm.EstNo = ksm.est_no;
                srvm.CustomerID = ksm.cust_id;
                srvm.BillDate = ksm.bill_date;
                srvm.SalesReturnDate = ksm.sr_date;
                srvm.GSType = ksm.gs_type;
                srvm.TaxAmount = ksm.tax_amt;
                srvm.TotalSRAmount = ksm.total_sramount;
                srvm.OperatorCode = ksm.operator_code;
                srvm.SalCode = ksm.sal_code;
                srvm.BillCounter = ksm.bill_counter;
                srvm.Remarks = ksm.remarks;
                srvm.UpdateOn = ksm.UpdateOn;
                srvm.ISIns = ksm.is_ins;
                srvm.Rate = ksm.rate;
                srvm.BilledBranch = ksm.billed_branch;
                srvm.CFlag = ksm.cflag;
                srvm.ExciseDutyPercent = ksm.excise_duty_percent;
                srvm.EDCessPercent = ksm.ed_cess_percent;
                srvm.HEDCessPercent = ksm.hed_cess_percent;
                srvm.ExciseDutyAmount = ksm.excise_duty_amount;
                srvm.EDCessAmount = ksm.ed_cess_amount;
                srvm.HEDCessAmount = ksm.hed_cess_amount;
                srvm.DiscountAmount = ksm.discount_amt;
                srvm.IsAdjusted = ksm.is_adjusted;
                srvm.CancelledBy = ksm.cancelled_by;
                srvm.CancelledRemarks = ksm.cancelled_remarks;
                srvm.NewBillNo = ksm.New_Bill_No;
                srvm.RoundOff = ksm.round_off;
                srvm.ShiftID = ksm.ShiftID;
                srvm.InvoiceTypeID = ksm.invoice_type_id;
                srvm.TotalCSTAmount = ksm.total_cst_amount;
                srvm.VPAmount = ksm.vp_amt;
                srvm.PayableAmount = ksm.payable_amt;

                List<SalesReturnDetailsVM> lstSRD = new List<SalesReturnDetailsVM>();
                List<KTTU_SR_DETAILS> ksd = db.KTTU_SR_DETAILS.Where(sale => sale.est_no == estNo
                                                            && sale.company_code == companyCode
                                                            && sale.branch_code == branchCode).ToList();
                foreach (KTTU_SR_DETAILS ksdb in ksd) {
                    SalesReturnDetailsVM srd = new SalesReturnDetailsVM();
                    srd.ObjID = ksdb.obj_id;
                    srd.CompanyCode = ksdb.company_code;
                    srd.BranchCode = ksdb.branch_code;
                    srd.SalesBillNo = ksdb.sales_bill_no;
                    srd.SlNo = ksdb.sl_no;
                    srd.EstNo = ksdb.est_no;
                    srd.SalCode = ksdb.sal_code;
                    srd.ItemName = ksdb.item_name;
                    srd.CounterCode = ksdb.counter_code;
                    srd.Quantity = ksdb.quantity;
                    srd.GrossWt = Convert.ToDecimal(ksdb.gwt);
                    srd.StoneWt = ksdb.swt;
                    srd.NetWt = ksdb.nwt;
                    srd.WastePercent = ksdb.wast_percent;
                    srd.MakingChargePerRs = ksdb.making_charge_per_rs;
                    srd.VAAmount = ksdb.va_amount;
                    srd.SRAmount = ksdb.sr_amount;
                    srd.StoneCharges = ksdb.stone_charges;
                    srd.DiamondCharges = ksdb.diamond_charges;
                    srd.NetAmount = ksdb.net_amount;
                    srd.AddQty = ksdb.Addqty;
                    srd.AddWt = ksdb.AddWt;
                    srd.DeductQty = ksdb.Deductqty;
                    srd.DeductWt = ksdb.DeductWt;
                    srd.UpdateOn = ksdb.UpdateOn;
                    srd.VAPecent = ksdb.va_percent;
                    srd.GSCode = ksdb.gs_code;
                    srd.FinYear = ksdb.Fin_Year;
                    srd.BarcodeNo = ksdb.barcode_no;
                    srd.SupplierCode = ksdb.supplier_code;
                    srd.ItemAdditionalDiscount = ksdb.item_additional_discount;
                    srd.ItemTotalAfterDiscount = ksdb.item_total_after_discount;
                    srd.TaxPercentage = ksdb.tax_percentage;
                    srd.TaxAmount = ksdb.tax_amount;
                    srd.ItemSize = ksdb.item_size;
                    srd.ImageID = ksdb.img_id;
                    srd.DesignCode = ksdb.design_code;
                    srd.DesignName = ksdb.design_name;
                    srd.BatchID = ksdb.batch_id;
                    srd.RFID = ksdb.rf_id;
                    srd.MCPerPiece = ksdb.mc_per_piece;
                    srd.OfferValue = ksdb.offer_value;
                    srd.ItemFinalAmount = ksdb.item_final_amount;
                    srd.MCType = ksdb.mc_type;
                    srd.RoundOff = ksdb.round_off;
                    srd.ItemFinalAmountAfterRoundOff = ksdb.item_final_amount_after_roundoff;
                    srd.OriginalSalesBillNo = null;
                    srd.ItemType = ksdb.item_type;
                    srd.CSTAmount = ksdb.cst_amount;
                    srd.Dcts = ksdb.Dcts;
                    srd.GSTGroupCode = ksdb.GSTGroupCode;
                    srd.SGSTPercent = ksdb.SGST_Percent;
                    srd.SGSTAmount = ksdb.SGST_Amount;
                    srd.CGSTAmount = ksdb.CGST_Amount;
                    srd.CGSTPercent = ksdb.CGST_Percent;
                    srd.IGSTAmount = ksdb.IGST_Amount;
                    srd.IGSTPercent = ksdb.IGST_Percent;
                    srd.HSN = ksdb.HSN;
                    srd.DiscountMc = ksdb.Discount_Mc;
                    srd.McDiscountAmt = ksdb.Mc_Discount_Amt;

                    List<SalesReturnStoneDetailsVM> lstSRStone = new List<SalesReturnStoneDetailsVM>();
                    List<KTTU_SR_STONE_DETAILS> kssd = db.KTTU_SR_STONE_DETAILS.Where(sale => sale.est_no == estNo
                                                            && sale.barcode_no == srd.BarcodeNo
                                                            && sale.company_code == companyCode
                                                            && sale.branch_code == branchCode).ToList();
                    foreach (KTTU_SR_STONE_DETAILS std in kssd) {
                        SalesReturnStoneDetailsVM ses = new SalesReturnStoneDetailsVM();
                        ses.ObjID = std.obj_id;
                        ses.CompanyCode = std.company_code;
                        ses.BranchCode = std.branch_code;
                        ses.SalesBillNo = std.sales_bill_no;
                        ses.Sno = std.sno;
                        ses.EstNo = std.est_no;
                        ses.BarcodeNo = std.barcode_no;
                        ses.Type = std.type;
                        ses.Name = std.name;
                        ses.Qty = std.qty;
                        ses.Carrat = std.carrat;
                        ses.StoneWt = std.stone_wt;
                        ses.Rate = Convert.ToDecimal(std.rate);
                        ses.Amount = std.amount;
                        ses.UpdateOn = std.UpdateOn;
                        ses.FinYear = std.Fin_Year;
                        ses.BarcodeNo = std.barcode_no;
                        ses.UniqRowID = std.UniqRowID;
                        lstSRStone.Add(ses);
                    }
                    //srd.lstOfStoneDetails = lstSRStone;
                    srd.lstOfStoneDetails = null;
                    lstSRD.Add(srd);
                }
                srvm.lstOfSalesReturnDetails = lstSRD;
                return srvm;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public int SaveSalesReturn(SalesReturnMasterVM salesReturn, out ErrorVM error)
        {
            error = null;

            if (salesReturn == null) {
                error = new ErrorVM()
                {
                    description = "Invalid Data",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return 0;
            }

            KSTU_CUSTOMER_MASTER customer = db.KSTU_CUSTOMER_MASTER.Where(cust => cust.cust_id == salesReturn.CustomerID
                                                                            && cust.company_code == salesReturn.CompanyCode
                                                                            && cust.branch_code == cust.branch_code).FirstOrDefault();
            try {
                KTTU_SALES_MASTER ksmv = db.KTTU_SALES_MASTER.Where(sale => sale.bill_no == salesReturn.VotureBillNo
                                                                    && sale.company_code == salesReturn.CompanyCode
                                                                    && sale.branch_code == salesReturn.BranchCode).FirstOrDefault();
                int estNo = db.KSTS_SEQ_NOS.Where(sq => sq.obj_id == MODULE_SEQ_NO
                                                      && sq.company_code == salesReturn.CompanyCode
                                                      && sq.branch_code == salesReturn.BranchCode).FirstOrDefault().nextno;
                string objectID = SIGlobals.Globals.GetMagnaGUID(TABLE_NAME, estNo, salesReturn.CompanyCode, salesReturn.BranchCode);
                KTTU_SR_MASTER ksm = new KTTU_SR_MASTER();
                ksm.obj_id = objectID;
                ksm.company_code = salesReturn.CompanyCode;
                ksm.branch_code = salesReturn.BranchCode;
                ksm.voture_bill_no = salesReturn.VotureBillNo;
                ksm.sales_bill_no = 0;
                ksm.est_no = estNo;
                ksm.cust_id = salesReturn.CustomerID;
                ksm.bill_date = ksmv.bill_date;
                ksm.sr_date = SIGlobals.Globals.GetApplicationDate(salesReturn.CompanyCode, salesReturn.BranchCode);
                ksm.gs_type = ksmv.gstype;
                ksm.tax_amt = ksmv.total_tax_amount;
                ksm.total_sramount = Convert.ToDecimal(ksmv.grand_total);
                ksm.operator_code = ksmv.operator_code;
                ksm.sal_code = salesReturn.SalCode;
                ksm.bill_counter = ksmv.bill_counter;
                ksm.remarks = ksmv.remarks;
                ksm.UpdateOn = SIGlobals.Globals.GetDateTime();
                ksm.is_ins = ksmv.is_ins;
                ksm.rate = ksmv.rate;
                ksm.billed_branch = salesReturn.BilledBranch;
                ksm.cflag = ksmv.cflag;
                ksm.excise_duty_percent = ksmv.excise_duty_percent;
                ksm.ed_cess_percent = ksmv.ed_cess_percent;
                ksm.hed_cess_percent = ksmv.hed_cess_percent;
                ksm.excise_duty_amount = ksmv.excise_duty_amount;
                ksm.ed_cess_amount = ksmv.ed_cess;
                ksm.hed_cess_amount = ksmv.hed_cess;
                ksm.discount_amt = ksmv.discount_amount;
                ksm.is_adjusted = ksmv.item_set;
                ksm.cancelled_by = ksmv.cancelled_by;
                ksm.cancelled_remarks = ksmv.cancelled_remarks;
                ksm.New_Bill_No = "0";
                ksm.round_off = ksmv.round_off;
                ksm.ShiftID = ksmv.ShiftID;
                ksm.invoice_type_id = 0;
                ksm.total_cst_amount = 0;
                ksm.vp_amt = 0;
                ksm.payable_amt = 0;
                ksm.cust_name = customer.cust_name;
                ksm.address1 = customer.address1;
                ksm.address2 = customer.address2;
                ksm.address3 = customer.address3;
                ksm.city = customer.city;
                ksm.pin_code = customer.pin_code;
                ksm.mobile_no = customer.mobile_no;
                ksm.state = customer.state;
                ksm.state_code = customer.state_code;
                ksm.tin = customer.tin;
                ksm.pan_no = customer.pan_no;
                ksm.UniqRowID = Guid.NewGuid();
                ksm.TotalDiscountVoucherAmt = Convert.ToDecimal(salesReturn.TotalDiscVoucherAmt);
                db.KTTU_SR_MASTER.Add(ksm);

                foreach (SalesReturnDetailsVM srd in salesReturn.lstOfSalesReturnDetails) {
                    KTTU_SR_DETAILS ksd = new KTTU_SR_DETAILS();
                    ksd.obj_id = objectID;
                    ksd.company_code = srd.CompanyCode;
                    ksd.branch_code = srd.BranchCode;
                    ksd.sales_bill_no = 0;
                    ksd.sl_no = srd.SlNo;
                    ksd.est_no = ksm.est_no;
                    ksd.sal_code = srd.SalCode;
                    ksd.item_name = srd.ItemName;
                    ksd.counter_code = srd.CounterCode;
                    ksd.quantity = srd.Quantity;
                    ksd.gwt = srd.GrossWt;
                    ksd.swt = srd.StoneWt;
                    ksd.nwt = srd.NetWt;
                    ksd.wast_percent = srd.WastePercent;
                    ksd.making_charge_per_rs = srd.MakingChargePerRs;
                    ksd.va_amount = srd.VAAmount;
                    ksd.sr_amount = srd.SRAmount;
                    ksd.stone_charges = srd.StoneCharges;
                    ksd.diamond_charges = srd.DiamondCharges;
                    ksd.net_amount = srd.NetAmount;
                    ksd.Addqty = srd.AddQty;
                    ksd.Deductqty = srd.DeductQty;
                    ksd.AddWt = srd.AddWt;
                    ksd.DeductWt = srd.DeductWt;
                    ksd.UpdateOn = SIGlobals.Globals.GetDateTime();
                    ksd.va_percent = srd.VAPecent;
                    ksd.gs_code = srd.GSCode;
                    ksd.Fin_Year = srd.FinYear;
                    ksd.barcode_no = srd.BarcodeNo;
                    ksd.supplier_code = srd.SupplierCode;
                    ksd.item_additional_discount = srd.ItemAdditionalDiscount;
                    ksd.item_total_after_discount = srd.ItemTotalAfterDiscount;
                    ksd.tax_percentage = srd.TaxPercentage;
                    ksd.tax_amount = srd.TaxAmount;
                    ksd.item_size = srd.ItemSize;
                    ksd.img_id = srd.ImageID;
                    ksd.design_code = srd.DesignCode;
                    ksd.design_name = srd.DesignName;
                    ksd.batch_id = srd.BatchID;
                    ksd.rf_id = srd.RFID;
                    ksd.mc_per_piece = srd.MCPerPiece;
                    ksd.offer_value = srd.OfferValue;
                    ksd.item_final_amount = srd.ItemFinalAmount;
                    ksd.mc_type = srd.MCType;
                    ksd.round_off = srd.RoundOff;
                    ksd.item_final_amount_after_roundoff = srd.ItemFinalAmountAfterRoundOff;
                    ksd.original_sales_bill_no = srd.OriginalSalesBillNo;
                    ksd.item_type = srd.ItemType;
                    ksd.cst_amount = srd.CSTAmount;
                    ksd.Dcts = srd.Dcts;
                    ksd.GSTGroupCode = srd.GSTGroupCode;
                    ksd.SGST_Percent = srd.SGSTPercent;
                    ksd.SGST_Amount = srd.SGSTAmount;
                    ksd.CGST_Percent = srd.CGSTPercent;
                    ksd.CGST_Amount = srd.CGSTAmount;
                    ksd.IGST_Percent = srd.IGSTPercent;
                    ksd.IGST_Amount = srd.IGSTAmount;
                    ksd.HSN = srd.HSN;
                    ksd.Discount_Mc = srd.DiscountMc;
                    ksd.Mc_Discount_Amt = srd.McDiscountAmt;
                    ksd.UniqRowID = Guid.NewGuid();
                    ksd.DiscountVoucherAmt = Convert.ToDecimal(srd.DiscVoucherAmt);
                    db.KTTU_SR_DETAILS.Add(ksd);

                    int SlNo = 1;
                    foreach (SalesReturnStoneDetailsVM srsd in srd.lstOfStoneDetails) {
                        KTTU_SR_STONE_DETAILS kssd = new KTTU_SR_STONE_DETAILS();
                        kssd.obj_id = objectID;
                        kssd.company_code = srsd.CompanyCode;
                        kssd.branch_code = srsd.BranchCode;
                        kssd.sales_bill_no = 0;
                        kssd.est_no = ksm.est_no;
                        kssd.item_sno = srsd.ItemSno;
                        kssd.sno = SlNo;
                        kssd.type = srsd.Type;
                        kssd.name = srsd.Name;
                        kssd.qty = srsd.Qty;
                        kssd.carrat = srsd.Carrat;
                        kssd.stone_wt = srsd.StoneWt;
                        kssd.rate = srsd.Rate;
                        kssd.amount = srsd.Amount;
                        kssd.UpdateOn = SIGlobals.Globals.GetDateTime();
                        kssd.Fin_Year = srsd.FinYear;
                        kssd.barcode_no = srsd.BarcodeNo;
                        kssd.UniqRowID = Guid.NewGuid();
                        db.KTTU_SR_STONE_DETAILS.Add(kssd);
                        SlNo = SlNo + 1;
                    }
                }
                // Updating Sequence Number
                SIGlobals.Globals.UpdateSeqenceNumber(db, MODULE_SEQ_NO, salesReturn.CompanyCode, salesReturn.BranchCode);
                db.SaveChanges();
                return ksm.est_no;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return 0;
            }
        }

        public string EstimationPrint(string companyCode, string branchCode, int srEstNo, out ErrorVM error)
        {
            error = null;
            StringBuilder sb = new StringBuilder();
            KTTU_SR_MASTER master = db.KTTU_SR_MASTER.Where(sr => sr.est_no == srEstNo
                                                            && sr.company_code == companyCode
                                                            && sr.branch_code == branchCode
                                                            && sr.cflag != "Y").FirstOrDefault();
            List<KTTU_SR_DETAILS> details = db.KTTU_SR_DETAILS.Where(sr => sr.est_no == srEstNo
                                                              && sr.company_code == companyCode
                                                              && sr.branch_code == branchCode).ToList();
            if (master == null) {
                error = new ErrorVM()
                {
                    field = "",
                    description = "SR Est.No " + srEstNo + " is invalid",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return null;
            }
            StringBuilder strLine = new StringBuilder();
            int width = 80;
            strLine.Append('-', width);
            StringBuilder strdoubleTransLine = new StringBuilder();
            strdoubleTransLine.Append('=', width);
            StringBuilder strTransLine = new StringBuilder();
            strTransLine.Append('-', width);
            decimal Spaces = 0M;
            StringBuilder strSpaces = new StringBuilder();
            Spaces = ((width - strTransLine.Length) / 3);
            strSpaces.Append(' ', Convert.ToInt32(Spaces));
            try {

                sb.AppendLine(string.Format("{0,-30}{1}", "", "Sales Return Estimation"));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));
                KSTS_GS_ITEM_ENTRY gs = db.KSTS_GS_ITEM_ENTRY.Where(g => g.gs_code == master.gs_type && g.company_code == companyCode && g.branch_code == branchCode).FirstOrDefault();
                string metaltype = gs.metal_type;
                string billDate = string.Format("{0:dd/MM/yyyy} {0:T}", Convert.ToDateTime(master.bill_date));
                string srDate = string.Format("{0:dd/MM/yyyy} {0:T}", Convert.ToDateTime(master.sr_date));
                sb.AppendLine(string.Format("{0,10}{1,-10}{2,58}"
                                   , "Sales Bill No", ": " + master.voture_bill_no.ToString()
                                   , "SR Date " + ":" + srDate));
                sb.Append(string.Format("{0,-10}{1} ", "SR Est.No", " : " + master.est_no.ToString()));
                if (string.Compare(metaltype, "GL") == 0)
                    sb.Append(string.Format("{0,25}{1}{2}", "Gold Rate : ", master.rate.ToString(), "/gm"));
                else if (string.Compare(metaltype, "SL") == 0)
                    sb.Append(string.Format("{0,25}{1}{2}", "Silver Rate : ", master.rate.ToString(), "/gm"));
                else if (string.Compare(metaltype, "PT") == 0)
                    sb.Append(string.Format("{0,25}{1}{2}", "Rate : ", master.rate.ToString(), "/gm"));
                else if (string.Compare(metaltype, "DM") == 0)
                    sb.Append(string.Format("{0,25}{1}{2}", "Rate : ", master.rate.ToString(), "   "));
                sb.AppendLine(string.Format("{0,29} ", ":#" + master.operator_code.ToString()));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));
                sb.AppendLine(string.Format("{0,-13}{1,3}{2,8}{3,8}{4,8}{5,10}{6,10}{7,10}{8,10}", "Stone", "Qty", "Gr.Wt", "St.Wt", "Net Wt", "VA Amt", "St.Amt", "Gold Amt", "Amount"));
                sb.AppendLine(string.Format("{0,-13}{1,3}{2,8}{3,8}{4,8}{5,10}{6,10}{7,10}{8,10}", "Name", "   ", "(gms)", "(gms)", "(gms)", "(Rs.Ps)", "(Rs.Ps)", "(Rs.Ps)", "(Rs.Ps)"));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));

                decimal Amount = 0;
                int Pcs = 0;
                decimal Gwt = 0, Swt = 0, Nwt = 0, GoldValue = 0, StoneChg = 0, ValueAmt = 0, additionalDiscount = 0, mcdiscount = 0;
                decimal GrossAmount = 0, TotalGrossAmount = 0, TotalTaxable = 0;
                decimal tax = 0;
                if (master.tax_amt.Equals(DBNull.Value))
                    tax = 0;
                else
                    tax = Convert.ToDecimal(master.tax_amt.ToString());
                for (int i = 0; i < details.Count; i++) {

                    string gsCode = details[i].gs_code;
                    string name = details[i].item_name;
                    ITEM_MASTER itemMaster = db.ITEM_MASTER.Where(it => it.gs_code == gsCode && it.Item_code == name && it.company_code == companyCode && it.branch_code == it.branch_code).FirstOrDefault();
                    string aliasName = itemMaster == null ? "" : itemMaster.Item_Name;

                    sb.Append(string.Format("{0,-13}", aliasName));
                    sb.Append(string.Format("{0,3}", details[i].quantity.ToString()));
                    sb.Append(string.Format("{0,8}", details[i].gwt.ToString()));
                    sb.Append(string.Format("{0,8}", details[i].swt.ToString()));
                    sb.Append(string.Format("{0,8}", details[i].nwt.ToString()));

                    decimal MCAmount = 0;
                    if (Convert.ToDecimal(details[i].making_charge_per_rs) > 0) {
                        MCAmount = Convert.ToDecimal(details[i].nwt) * Convert.ToDecimal(details[i].making_charge_per_rs);
                    }
                    else if (Convert.ToDecimal(details[i].va_percent) > 0) {
                        MCAmount = (Convert.ToDecimal(details[i].sr_amount) * Convert.ToDecimal(details[i].va_percent)) / 100;
                    }
                    else {
                        MCAmount = Convert.ToDecimal(details[i].va_amount);
                    }
                    MCAmount = decimal.Round(MCAmount, 2);


                    sb.Append(string.Format("{0,10}", MCAmount));
                    sb.Append(string.Format("{0,10}", details[i].stone_charges.ToString()));
                    sb.Append(string.Format("{0,10}", details[i].sr_amount.ToString()));
                    GrossAmount = MCAmount + Convert.ToDecimal(details[i].stone_charges) + (Convert.ToDecimal(details[i].diamond_charges)) + Convert.ToDecimal(details[i].sr_amount);
                    sb.AppendLine(string.Format("{0,10}", GrossAmount));
                    TotalGrossAmount += GrossAmount;
                    TotalTaxable += Convert.ToDecimal(details[i].item_total_after_discount);
                    Pcs += Convert.ToInt32(details[i].quantity);
                    Gwt += Convert.ToDecimal((details[i].gwt + details[i].AddWt) - details[i].DeductWt);
                    Swt += Convert.ToDecimal(details[i].swt);
                    Nwt += Convert.ToDecimal((details[i].nwt + details[i].AddWt) - details[i].DeductWt);
                    ValueAmt += MCAmount;
                    StoneChg += Convert.ToDecimal(details[i].stone_charges);
                    GoldValue += Convert.ToDecimal(details[i].sr_amount);
                    additionalDiscount += Convert.ToDecimal(details[i].item_additional_discount);
                    mcdiscount += Convert.ToDecimal(details[i].Mc_Discount_Amt);
                    Amount = TotalGrossAmount;
                }
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                sb.AppendLine(string.Format("{0,-13}{1,3}{2,8}{3,8}{4,8}{5,10}{6,10}{7,10}{8,10}", "Total", Pcs, Gwt, Swt, Nwt, ValueAmt, StoneChg, GoldValue, TotalGrossAmount));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));

                if (additionalDiscount > 0) {
                    sb.AppendLine(string.Format("{0,65}{1,15}", "Less  :", additionalDiscount));
                    Amount = Amount - additionalDiscount;
                }
                if (mcdiscount > 0) {
                    sb.AppendLine(string.Format("{0,65}{1,15}", "Less  :", decimal.Round(mcdiscount, 2)));
                    Amount = Amount - additionalDiscount;
                }
                if (Convert.ToDecimal(master.excise_duty_amount) > 0) {
                    sb.AppendLine(string.Format("{0,65}{1,15}", "ED Amount :", master.excise_duty_amount.ToString()));
                    Amount = Amount + Convert.ToDecimal(master.excise_duty_amount);
                    if (Convert.ToDecimal(master.ed_cess_amount) > 0) {
                        sb.AppendLine(string.Format("{0,65}{1,15}", "ED Cess :", master.ed_cess_amount.ToString()));
                        Amount = Amount + Convert.ToDecimal(master.ed_cess_amount);
                    }
                    if (Convert.ToDecimal(master.hed_cess_amount) > 0) {
                        sb.AppendLine(string.Format("{0,65}{1,15}", "HED Cess :", master.hed_cess_amount.ToString()));
                        Amount = Amount + Convert.ToDecimal(master.hed_cess_amount);
                    }
                }
                var gstDetails = (from sr in db.KTTU_SR_DETAILS
                                  where sr.est_no == srEstNo && sr.company_code == companyCode && sr.branch_code == branchCode
                                  group sr by new
                                  {
                                      srEstNo
                                  } into g
                                  select new
                                  {
                                      cgstAmount = g.Sum(x => x.CGST_Amount),
                                      sgstAmount = g.Sum(x => x.SGST_Amount),
                                      igstAmount = g.Sum(x => x.IGST_Amount)
                                  }).FirstOrDefault();
                sb.AppendLine(string.Format("{0,65}{1,15}", "Taxable Value :", TotalTaxable));
                decimal cgstAmount = Convert.ToDecimal(gstDetails.cgstAmount);
                decimal sgstAmount = Convert.ToDecimal(gstDetails.sgstAmount);
                decimal igstAmount = Convert.ToDecimal(gstDetails.igstAmount);
                decimal finalAmount = Math.Round(TotalTaxable + sgstAmount + cgstAmount + igstAmount, 0, MidpointRounding.AwayFromZero);

                sb.AppendLine(string.Format("{0,65}{1,15}", " CGST " + details[0].CGST_Percent + " %:", cgstAmount));
                Amount = Amount + cgstAmount;
                sb.AppendLine(string.Format("{0,65}{1,15}", " SGST " + details[0].SGST_Percent + " %:", sgstAmount));
                Amount = Amount + sgstAmount;
                sb.AppendLine(string.Format("{0,65}{1,15}", " IGST " + details[0].IGST_Percent + " %:", igstAmount));
                Amount = Amount + igstAmount;

                Amount = Math.Round(Amount, 2);
                sb.AppendLine(string.Format("{0,65}{1,15}", "Invoice Amount" + " :", finalAmount));
                sb.AppendLine();
                string strWords = string.Empty;
                NumberToWords objNWClass = new NumberToWords();
                objNWClass.ConvertNumberToWords(Convert.ToDouble(Amount), out strWords);
                strWords = strWords.Replace("Rupees", "");
                strWords = "Rupees " + strWords;
                sb.AppendLine(string.Format("{0,-80}", "" + strWords));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));
                sb.AppendLine();
                return sb.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        public string EstimationDotMatrixPrint(string companyCode, string branchCode, int srEstNo, out ErrorVM error)
        {

            error = null;
            try {
                KTTU_SR_MASTER srMaster = db.KTTU_SR_MASTER.Where(master => master.est_no == srEstNo && master.company_code == companyCode && master.branch_code == branchCode).FirstOrDefault();
                if (srMaster == null) return "";
                List<KTTU_SR_DETAILS> srDet = db.KTTU_SR_DETAILS.Where(master => master.est_no == srEstNo && master.company_code == companyCode && master.branch_code == branchCode).ToList();

                StringBuilder strLine = new StringBuilder();
                int width = 80;
                strLine.Append('-', width);
                StringBuilder strdoubleTransLine = new StringBuilder();
                strdoubleTransLine.Append('=', width);
                StringBuilder strTransLine = new StringBuilder();
                strTransLine.Append('-', width);
                decimal Spaces = 0M;
                StringBuilder strSpaces = new StringBuilder();
                Spaces = ((width - strTransLine.Length) / 3);
                strSpaces.Append(' ', Convert.ToInt32(Spaces));
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("{0,-30}{1}", "", "Sales Return Estimation"));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));

                string gsCode = srMaster.gs_type.ToString();
                KSTS_GS_ITEM_ENTRY itemEntry = db.KSTS_GS_ITEM_ENTRY.Where(item => item.gs_code == gsCode && item.company_code == companyCode && item.branch_code == branchCode).FirstOrDefault();

                string metaltype = itemEntry.metal_type;
                string DateTime = string.Format("{0:dd/MM/yyyy} {0:T}", Convert.ToDateTime(srMaster.bill_date));
                sb.AppendLine(string.Format("{0,10}{1,-10}{2,58}"
                                   , "Sales Bill No", ": " + srMaster.voture_bill_no.ToString()
                                   , "SR Date " + ":" + DateTime));
                sb.Append(string.Format("{0,-10}{1} ", "SR Est.No", " : " + srMaster.est_no.ToString()));
                if (string.Compare(metaltype, "GL") == 0)
                    sb.Append(string.Format("{0,25}{1}{2}", "Gold Rate : ", srMaster.rate.ToString(), "/gm"));
                else if (string.Compare(metaltype, "SL") == 0)
                    sb.Append(string.Format("{0,25}{1}{2}", "Silver Rate : ", srMaster.rate.ToString(), "/gm"));
                else if (string.Compare(metaltype, "PT") == 0)
                    sb.Append(string.Format("{0,25}{1}{2}", "Rate : ", srMaster.rate.ToString(), "/gm"));
                else if (string.Compare(metaltype, "DM") == 0)
                    sb.Append(string.Format("{0,25}{1}{2}", "Rate : ", srMaster.rate.ToString(), "   "));
                sb.AppendLine(string.Format("{0,29} ", ":#" + srMaster.operator_code.ToString()));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));

                sb.AppendLine(string.Format("{0,-13}{1,3}{2,8}{3,8}{4,8}{5,10}{6,10}{7,10}{8,10}", "Stone", "Qty", "Gr.Wt", "St.Wt", "Net Wt", "VA Amt", "St.Amt", "Gold Amt", "Amount"));
                sb.AppendLine(string.Format("{0,-13}{1,3}{2,8}{3,8}{4,8}{5,10}{6,10}{7,10}{8,10}", "Name", "   ", "(gms)", "(gms)", "(gms)", "(Rs.Ps)", "(Rs.Ps)", "(Rs.Ps)", "(Rs.Ps)"));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));

                decimal Amount = 0;
                int Pcs = 0;
                decimal Gwt = 0, Swt = 0, Nwt = 0, GoldValue = 0, StoneChg = 0, ValueAmt = 0, additionalDiscount = 0, mcdiscount = 0;
                decimal GrossAmount = 0, TotalGrossAmount = 0, TotalTaxable = 0;

                decimal tax = 0;
                if (srMaster.tax_amt == 0) {
                    tax = 0;
                }
                else {
                    decimal netAmount = Convert.ToDecimal(srDet.Sum(sr => sr.net_amount));
                    netAmount = (netAmount / Convert.ToDecimal(srMaster.tax_amt)) / 100;
                    tax = netAmount;
                }
                for (int i = 0; i < srDet.Count; i++) {
                    sb.Append(string.Format("{0,-13}", SIGlobals.Globals.GetAliasName(db, srMaster.gs_type.ToString(), srDet[i].item_name.ToString(), companyCode, branchCode)));
                    sb.Append(string.Format("{0,3}", srDet[i].quantity.ToString()));
                    sb.Append(string.Format("{0,8}", ((srDet[i].gwt + srDet[i].AddWt) - srDet[i].DeductWt).ToString()));
                    sb.Append(string.Format("{0,8}", srDet[i].swt.ToString()));
                    sb.Append(string.Format("{0,8}", ((srDet[i].nwt + srDet[i].AddWt) - srDet[i].DeductWt).ToString()));

                    decimal MCAmount = 0;
                    if (Convert.ToDecimal(srDet[i].making_charge_per_rs) > 0) {
                        MCAmount = Convert.ToDecimal((srDet[i].nwt) * Convert.ToDecimal((srDet[i].making_charge_per_rs)));
                    }
                    else if (Convert.ToDecimal(srDet[i].va_percent) > 0) {
                        MCAmount = Convert.ToDecimal(srDet[i].sr_amount) * Convert.ToDecimal(srDet[i].va_percent) / 100;
                    }
                    else {
                        MCAmount = Convert.ToDecimal(srDet[i].va_amount);
                    }
                    MCAmount = decimal.Round(MCAmount, 2);
                    sb.Append(string.Format("{0,10}", MCAmount));
                    sb.Append(string.Format("{0,10}", srDet[i].stone_charges.ToString()));
                    sb.Append(string.Format("{0,10}", srDet[i].sr_amount.ToString()));
                    GrossAmount = MCAmount + Convert.ToDecimal(srDet[i].stone_charges) + Convert.ToDecimal(srDet[i].sr_amount);
                    sb.AppendLine(string.Format("{0,10}", GrossAmount));
                    TotalGrossAmount += GrossAmount;

                    TotalTaxable += Convert.ToDecimal(srDet[i].item_total_after_discount);
                    Pcs += Convert.ToInt32(srDet[i].quantity);
                    Gwt += Convert.ToDecimal(srDet[i].gwt);
                    Swt += Convert.ToDecimal(srDet[i].swt);
                    Nwt += Convert.ToDecimal(srDet[i].nwt);
                    ValueAmt += MCAmount;
                    StoneChg += Convert.ToDecimal(srDet[i].stone_charges);
                    GoldValue += Convert.ToDecimal(srDet[i].sr_amount);
                    additionalDiscount += Convert.ToDecimal(srDet[i].item_additional_discount);
                    mcdiscount += Convert.ToDecimal(srDet[i].Mc_Discount_Amt);
                    Amount = TotalGrossAmount;
                }
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                sb.AppendLine(string.Format("{0,-13}{1,3}{2,8}{3,8}{4,8}{5,10}{6,10}{7,10}{8,10}", "Total", Pcs, Gwt, Swt, Nwt, ValueAmt, StoneChg, GoldValue, TotalGrossAmount));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                if (additionalDiscount > 0) {
                    sb.AppendLine(string.Format("{0,65}{1,15}", "Less  :", additionalDiscount));
                    Amount = Amount - additionalDiscount;
                }
                if (mcdiscount > 0) {
                    sb.AppendLine(string.Format("{0,65}{1,15}", "Less  :", decimal.Round(mcdiscount, 2)));
                    Amount = Amount - additionalDiscount;
                }
                if (Convert.ToDecimal(srMaster.excise_duty_amount) > 0) {
                    sb.AppendLine(string.Format("{0,65}{1,15}", "ED Amount :", srMaster.excise_duty_amount.ToString()));
                    Amount = Amount + Convert.ToDecimal(srMaster.excise_duty_amount);
                    if (Convert.ToDecimal(srMaster.ed_cess_amount) > 0) {
                        sb.AppendLine(string.Format("{0,65}{1,15}", "ED Cess :", srMaster.ed_cess_amount.ToString()));
                        Amount = Amount + Convert.ToDecimal(srMaster.ed_cess_amount);
                    }
                    if (Convert.ToDecimal(srMaster.hed_cess_amount) > 0) {
                        sb.AppendLine(string.Format("{0,65}{1,15}", "HED Cess :", srMaster.hed_cess_amount.ToString()));
                        Amount = Amount + Convert.ToDecimal(srMaster.hed_cess_amount);
                    }
                }
                sb.AppendLine(string.Format("{0,65}{1,15}", "Taxable Value :", TotalTaxable));
                decimal cgstAmount = Convert.ToDecimal(srDet.Sum(sr => sr.CGST_Amount));
                decimal sgstAmount = Convert.ToDecimal(srDet.Sum(sr => sr.SGST_Amount));
                decimal igstAmount = Convert.ToDecimal(srDet.Sum(sr => sr.IGST_Amount));

                decimal finalAmount = Math.Round(TotalTaxable + sgstAmount + cgstAmount + igstAmount, 0, MidpointRounding.AwayFromZero);
                sb.AppendLine(string.Format("{0,65}{1,15}", " CGST " + srDet[0].CGST_Percent + " %:", cgstAmount));
                Amount = Amount + cgstAmount;
                sb.AppendLine(string.Format("{0,65}{1,15}", " SGST " + srDet[0].SGST_Percent + " %:", sgstAmount));
                Amount = Amount + sgstAmount;
                sb.AppendLine(string.Format("{0,65}{1,15}", " IGST " + srDet[0].IGST_Percent + " %:", igstAmount));
                Amount = Amount + igstAmount;
                Amount = Math.Round(Amount, 2);
                sb.AppendLine(string.Format("{0,65}{1,15}", "Invoice Amount" + " :", finalAmount));
                sb.AppendLine();
                string strWords = string.Empty;
                NumberToWords objNWClass = new NumberToWords();
                objNWClass.ConvertNumberToWords(Convert.ToDouble(Amount), out strWords);
                strWords = strWords.Replace("Rupees", "");
                strWords = "Rupees " + strWords;
                sb.AppendLine(string.Format("{0,-80}", "" + strWords));

                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));
                sb.AppendLine();
                return sb.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        public string GetThermalPrint40Column(string companyCode, string branchCode, int estNo, bool fromSales, out ErrorVM error)
        {
            error = null;
            string reportString = string.Empty;
            try {

                if (fromSales) {
                    List<KTTU_PAYMENT_DETAILS> srAttachments = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.company_code == companyCode
                                                                          && pay.branch_code == branchCode
                                                                          && pay.pay_mode == "SE"
                                                                          && pay.series_no == estNo).ToList();
                    if (srAttachments == null || srAttachments.Count == 0) {
                        return "";
                    }
                    int srEstNo;
                    for (int i = 0; i < srAttachments.Count; i++) {
                        srEstNo = Convert.ToInt32(srAttachments[i].Ref_BillNo.ToString());
                        reportString = reportString + GenerateHTMLPrint40Column(companyCode, branchCode, srEstNo);
                    }
                }
                else {
                    reportString = GenerateHTMLPrint40Column(companyCode, branchCode, estNo);
                }
                return reportString;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        public bool DeleteSREstimation(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            try {
                KTTU_SR_MASTER srMaster = db.KTTU_SR_MASTER.Where(est => est.est_no == estNo
                                                                && est.company_code == companyCode
                                                                && est.branch_code == branchCode).FirstOrDefault();
                if (srMaster == null) {
                    error = new ErrorVM()
                    {
                        description = "Invalid Sales Return Estimation No.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }

                if (srMaster.sales_bill_no != 0) {
                    error = new ErrorVM()
                    {
                        description = string.Format("SR Estimation already confirmed with SR Bill No {0}, so cannott be delete", srMaster.sales_bill_no),
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
                List<KTTU_SR_DETAILS> srDet = db.KTTU_SR_DETAILS.Where(est => est.est_no == estNo
                                                                       && est.company_code == companyCode
                                                                       && est.branch_code == branchCode).ToList();
                db.Entry(srMaster).State = System.Data.Entity.EntityState.Deleted;
                db.KTTU_SR_DETAILS.RemoveRange(srDet);
                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public dynamic GetAllSREstimations(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                DateTime appDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                List<string> filters = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.company_code == companyCode
                                                                        && pay.branch_code == branchCode
                                                                        && pay.trans_type == "A"
                                                                        && pay.trans_type == "SE").Select(r => r.Ref_BillNo).ToList();

                List<int> myStringList = filters.Select(s => int.Parse(s)).ToList();
                List<KTTU_SR_MASTER> srMaster = db.KTTU_SR_MASTER.Where(sr => sr.company_code == companyCode
                                                                        && sr.branch_code == branchCode
                                                                        && sr.cflag == "N"
                                                                        && !myStringList.Contains(sr.est_no)
                                                                        && sr.sales_bill_no == 0
                                                                        && System.Data.Entity.DbFunctions.TruncateTime(sr.sr_date) == System.Data.Entity.DbFunctions.TruncateTime(appDate)).ToList();
                return srMaster.Select(sr => new SalesReturnMasterVM()
                {
                    EstNo = sr.est_no,
                    SalesBillNo = sr.sales_bill_no,
                    VotureBillNo = sr.voture_bill_no,
                    CustomerName = sr.cust_name
                });
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public ProdigyPrintVM GetSREstimatePrint(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            PrintConfiguration pc = new PrintConfiguration();
            var printConfig = pc.GetPrintConfigurationForDraftDocuments(companyCode, branchCode, "SRT_EST");
            if (printConfig == "HTML") {
                var htmlPrintData = GetThermalPrint40Column(companyCode, branchCode, estNo, false, out error);
                printObject.ContinueNextPrint = true;
                printObject.Data = new PrintConfiguration().Base64Encode(htmlPrintData);
                printObject.PrintType = "HTML";
            }
            else {
                var dotMatrixPrintData = EstimationPrint(companyCode, branchCode, estNo, out error);
                printObject.ContinueNextPrint = true;
                printObject.Data = dotMatrixPrintData;
                printObject.PrintType = "RAW";
            }

            return printObject;
        }
        #endregion

        #region Sales Return Attachment
        public IQueryable<AllSalesReturnVM> GetSRDetails(string companyCode, string branchCode)
        {
            try {
                List<AllSalesReturnVM> data = (from ksm in db.KTTU_SR_MASTER
                                               join ksd in db.KTTU_SR_DETAILS
                                               on new { CompanyCode = ksm.company_code, BranchCode = ksm.branch_code, EstNo = ksm.est_no }
                                               equals new { CompanyCode = ksd.company_code, BranchCode = ksd.branch_code, EstNo = ksd.est_no }
                                               where ksm.sales_bill_no == 0 && ksm.company_code == companyCode && ksm.branch_code == branchCode
                                               group ksd by new { ksd.quantity, ksd.gwt, ksm.est_no, ksm.sr_date, ksm.total_sramount, ksm.cust_id, ksd.tax_amount } into g
                                               select new AllSalesReturnVM
                                               {
                                                   SrEstNo = g.Key.est_no,
                                                   Date = g.Key.sr_date.ToString(),
                                                   Amount = g.Key.total_sramount,
                                                   GrossWt = g.Sum(ksd => ksd.gwt),
                                                   Quantity = g.Key.quantity,
                                                   TaxAmount = g.Key.tax_amount.ToString(),
                                                   CustID = g.Key.cust_id,
                                               }).ToList();
                return data.AsQueryable<AllSalesReturnVM>();
            }
            catch (Exception excp) {
                return null;
            }
        }

        public int GetAllOrdersCount(string companyCode, string branchCode)
        {
            List<AllSalesReturnVM> lstAllSR = GetSRDetails(companyCode, branchCode).ToList();
            return lstAllSR.Count();
        }

        public List<AllSalesReturnVM> GetAttachedSalesReturnDetails(string companyCode, string branchCode, int estNo)
        {
            //List<AllSalesReturnVM> lstAllSR = new List<AllSalesReturnVM>();
            //List<KTTU_PAYMENT_DETAILS> payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == EstNo && pay.pay_mode == "SE").ToList();
            //foreach (KTTU_PAYMENT_DETAILS kpd in payment)
            //{
            //    string Query = "SELECT ksm.est_no, " +
            //    "       ksm.sr_date, " +
            //    "       ksm.total_sramount, " +
            //    "       ksm.tax_amt, " +
            //    "       t.gwt, " +
            //    "       t.quantity, " +
            //    "       ksm.cust_id" +
            //    " FROM dbo.KTTU_SR_MASTER ksm" +
            //    "     INNER JOIN" +
            //    "(" +
            //    "    SELECT ksd.est_no, " +
            //    "           SUM(ksd.gwt) AS gwt, " +
            //    "           SUM(ksd.quantity) AS quantity" +
            //    "    FROM dbo.KTTU_SR_DETAILS ksd" +
            //    "    WHERE ksd.est_no = " + kpd.Ref_BillNo + "" +
            //    "    GROUP BY ksd.est_no" +
            //    ") AS t ON ksm.est_no = t.est_no; ";

            //    DataTable tblData = Common.ExecuteQuery(Query);
            //    AllSalesReturnVM sales = new AllSalesReturnVM()
            //    {
            //        SrEstNo = Convert.ToInt32(tblData.Rows[0]["est_no"]),
            //        Date = Convert.ToString(tblData.Rows[0]["sr_date"]),
            //        Amount = Convert.ToDecimal(tblData.Rows[0]["total_sramount"]),
            //        GrossWt = Convert.ToDecimal(tblData.Rows[0]["gwt"]),
            //        Quantity = Convert.ToInt32(tblData.Rows[0]["quantity"]),
            //        TaxAmount = Convert.ToString(tblData.Rows[0]["tax_amt"]),
            //        CustID = Convert.ToInt32(tblData.Rows[0]["cust_id"]),
            //    };
            //    lstAllSR.Add(sales);
            //}
            //return Ok(lstAllSR);

            // OR
            List<KTTU_PAYMENT_DETAILS> payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == estNo && pay.pay_mode == "SE").ToList();
            List<AllSalesReturnVM> lstAllSR = new List<AllSalesReturnVM>();
            foreach (KTTU_PAYMENT_DETAILS payDet in payment) {
                int srEstNo = Convert.ToInt32(payDet.Ref_BillNo);
                AllSalesReturnVM result = (from ksm in db.KTTU_SR_MASTER
                                           join ksd in db.KTTU_SR_DETAILS
                                             on new { CompanyCode = ksm.company_code, BranchCode = ksm.branch_code, EstNo = ksm.est_no }
                                             equals new { CompanyCode = ksd.company_code, BranchCode = ksd.branch_code, EstNo = ksd.est_no }
                                           where ksm.sales_bill_no == 0 && ksm.est_no == srEstNo && ksm.company_code == companyCode && ksm.branch_code == branchCode
                                           group ksd by new
                                           {
                                               ksm.est_no,
                                               ksm.sr_date,
                                               ksm.total_sramount,
                                               ksm.cust_id,
                                               ksm.tax_amt
                                           } into g
                                           select new AllSalesReturnVM
                                           {
                                               Quantity = g.Sum(x => x.quantity),
                                               GrossWt = g.Sum(x => x.gwt),
                                               SrEstNo = g.Key.est_no,
                                               Date = g.Key.sr_date.ToString(),
                                               Amount = g.Key.total_sramount,
                                               CustID = g.Key.cust_id,
                                               TaxAmount = g.Key.tax_amt.ToString()
                                           }).FirstOrDefault<AllSalesReturnVM>();
                AllSalesReturnVM sales = new AllSalesReturnVM()
                {
                    SrEstNo = result.SrEstNo,
                    Date = result.Date,
                    Amount = result.Amount,
                    GrossWt = result.GrossWt,
                    Quantity = result.Quantity,
                    TaxAmount = result.TaxAmount,
                    CustID = result.CustID
                };
                lstAllSR.Add(sales);
            }
            return lstAllSR;
        }

        public List<SearchParamVM> GetAllSearchParameters()
        {
            List<SearchParamVM> lstSearchParams = new List<SearchParamVM>();
            lstSearchParams.Add(new SearchParamVM() { Key = "SrEstNo", Value = "SRESTNO" });
            lstSearchParams.Add(new SearchParamVM() { Key = "Date", Value = "DATE" });
            lstSearchParams.Add(new SearchParamVM() { Key = "Amount", Value = "AMOUNT" });
            lstSearchParams.Add(new SearchParamVM() { Key = "GrossWt", Value = "GROSSWT" });
            lstSearchParams.Add(new SearchParamVM() { Key = "Quantity", Value = "QUANTITY" });
            lstSearchParams.Add(new SearchParamVM() { Key = "TaxAmount", Value = "TAXAMOUNT" });
            lstSearchParams.Add(new SearchParamVM() { Key = "CustID", Value = "CUSTID" });
            return lstSearchParams;
        }

        public IQueryable<AllSalesReturnVM> GetSearchOrder(string companyCode, string branchCode, string searchType, string searchValue)
        {
            List<AllSalesReturnVM> lstAllSR = GetSRDetails(companyCode, branchCode).ToList();
            switch (searchType.ToUpper()) {
                case "SRESTNO":
                    return lstAllSR.Where(search => search.SrEstNo == Convert.ToInt32(searchValue)).AsQueryable<AllSalesReturnVM>();
                case "AMOUNT":
                    return lstAllSR.Where(search => search.Amount.ToString().Contains(searchValue)).AsQueryable<AllSalesReturnVM>();
                case "DATE":
                    return lstAllSR.Where(search => search.Date.ToString() == Convert.ToString(searchValue)).AsQueryable<AllSalesReturnVM>();
                case "GROSSWT":
                    return lstAllSR.Where(search => search.GrossWt.ToString() == Convert.ToString(searchValue)).AsQueryable<AllSalesReturnVM>();
                case "QUANTITY":
                    return lstAllSR.Where(search => search.Quantity.ToString() == Convert.ToString(searchValue)).AsQueryable<AllSalesReturnVM>();
                case "TAXAMOUNT":
                    return lstAllSR.Where(search => search.TaxAmount.ToString() == Convert.ToString(searchValue)).AsQueryable<AllSalesReturnVM>();
                case "CUSTID":
                    return lstAllSR.Where(search => search.CustID.ToString() == Convert.ToString(searchValue)).AsQueryable<AllSalesReturnVM>();
            }
            return lstAllSR.AsQueryable<AllSalesReturnVM>();
        }

        public List<SalesReturnMasterVM> GetAttachedOrder(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            List<KTTU_PAYMENT_DETAILS> payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == estNo
                                                                                && pay.pay_mode == "SE"
                                                                                && pay.company_code == companyCode
                                                                                && pay.branch_code == branchCode).ToList();
            List<SalesReturnMasterVM> lstOfSRMaster = new List<SalesReturnMasterVM>();
            foreach (KTTU_PAYMENT_DETAILS pay in payment) {
                SalesReturnMasterVM om = new SalesReturnMasterVM();
                int refBillNo = Convert.ToInt32(pay.Ref_BillNo);
                om = new SalesReturnEstimationBL().GetSavedSRInfo(companyCode, branchCode, estNo, out error);
                lstOfSRMaster.Add(om);
            }
            return lstOfSRMaster;
        }

        public bool PostSRAttachment(List<PaymentVM> payment, out ErrorVM error)
        {
            error = null;
            if (payment != null) {
                error = new ErrorVM()
                {
                    description = "Invalid Sales return details",
                    field = "",
                    index = 0,
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            try {
                int estNo = payment[0].SeriesNo;
                string companyCode = payment[0].CompanyCode;
                string branchCode = payment[0].BranchCode;

                if (estNo == 0) {
                    error = new ErrorVM()
                    {
                        description = "Invalid Estimation No.",
                        field = "",
                        index = 0,
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
                KTTU_SALES_EST_MASTER salEstimation = db.KTTU_SALES_EST_MASTER.Where(est => est.est_no == estNo
                                                                                        && est.company_code == companyCode
                                                                                        && est.branch_code == branchCode).FirstOrDefault();
                if (salEstimation == null) {
                    error = new ErrorVM()
                    {
                        description = "SR Details Not Found",
                        field = "",
                        index = 0,
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                    return false;
                }


                List<KTTU_PAYMENT_DETAILS> paymentDel = db.KTTU_PAYMENT_DETAILS.Where(est => est.series_no == estNo
                                                                                        && est.pay_mode == "SE"
                                                                                        && est.company_code == companyCode
                                                                                        && est.branch_code == branchCode).ToList();
                foreach (KTTU_PAYMENT_DETAILS pay in paymentDel) {
                    db.KTTU_PAYMENT_DETAILS.Remove(pay);
                    db.SaveChanges();
                }

                int paySlNo = 1;
                string objID = salEstimation.obj_id;
                foreach (PaymentVM pay in payment) {
                    int refBillNo = Convert.ToInt32(pay.RefBillNo);
                    KTTU_SR_MASTER salesReturn = db.KTTU_SR_MASTER.Where(sr => sr.est_no == refBillNo
                                                                        && sr.company_code == companyCode
                                                                        && sr.branch_code == branchCode).FirstOrDefault();
                    KTTU_PAYMENT_DETAILS kpd = new KTTU_PAYMENT_DETAILS();
                    kpd.obj_id = objID;
                    kpd.company_code = pay.CompanyCode;
                    kpd.branch_code = pay.BranchCode;
                    kpd.series_no = pay.SeriesNo; // Estimation Number
                    kpd.receipt_no = 0;
                    kpd.sno = paySlNo;
                    kpd.trans_type = "A";
                    kpd.pay_mode = "SE";
                    kpd.pay_details = "";
                    kpd.pay_date = salesReturn.sr_date;
                    kpd.pay_amt = salesReturn.payable_amt;
                    kpd.Ref_BillNo = pay.RefBillNo; // Order Number
                    kpd.party_code = null;
                    kpd.bill_counter = null;
                    kpd.is_paid = "Y";
                    kpd.bank = "";
                    kpd.bank_name = "";
                    kpd.cheque_date = salesReturn.sr_date;
                    kpd.card_type = "";
                    kpd.expiry_date = pay.ExpiryDate;
                    kpd.cflag = "N";
                    kpd.card_app_no = "";
                    kpd.scheme_code = null;
                    kpd.sal_bill_type = null;
                    kpd.operator_code = pay.OperatorCode; // Need to send from JSON
                    kpd.session_no = 0;
                    kpd.UpdateOn = SIGlobals.Globals.GetDateTime();
                    kpd.group_code = null;
                    kpd.amt_received = 0;
                    kpd.bonus_amt = 0;
                    kpd.win_amt = 0;
                    kpd.ct_branch = null;
                    kpd.fin_year = 0;
                    kpd.CardCharges = 0;
                    kpd.cheque_no = "0";
                    kpd.New_Bill_No = pay.SeriesNo.ToString();
                    kpd.Add_disc = Convert.ToDecimal(0.00);
                    kpd.isOrdermanual = "N";
                    kpd.currency_value = Convert.ToDecimal(0.00);
                    kpd.exchange_rate = Convert.ToDecimal(0.00);
                    kpd.currency_type = null;
                    kpd.tax_percentage = Convert.ToDecimal(0.00);
                    kpd.cancelled_by = "";
                    kpd.cancelled_remarks = "";
                    kpd.cancelled_date = DateTime.MinValue.ToString();
                    kpd.isExchange = "N";
                    kpd.exchangeNo = 0;
                    kpd.new_receipt_no = "0";
                    kpd.Gift_Amount = Convert.ToDecimal(0.00);
                    kpd.cardSwipedBy = "";
                    kpd.version = 0;
                    kpd.GSTGroupCode = null;
                    kpd.SGST_Percent = Convert.ToDecimal(0.00);
                    kpd.CGST_Percent = Convert.ToDecimal(0.00);
                    kpd.IGST_Percent = Convert.ToDecimal(0.00);
                    kpd.HSN = null;
                    kpd.SGST_Amount = Convert.ToDecimal(0.00);
                    kpd.CGST_Amount = Convert.ToDecimal(0.00);
                    kpd.IGST_Amount = Convert.ToDecimal(0.00);
                    kpd.pay_amount_before_tax = salesReturn.payable_amt;
                    kpd.pay_tax_amount = Convert.ToDecimal(0.00);
                    kpd.UniqRowID = Guid.NewGuid();
                    db.KTTU_PAYMENT_DETAILS.Add(kpd);
                    paySlNo = paySlNo + 1;
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public bool RemoveSRAttachement(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            if (estNo == 0) {
                error = new ErrorVM()
                {
                    description = "Invalid Estimation No.",
                    field = "",
                    index = 0,
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            try {
                List<KTTU_PAYMENT_DETAILS> paymentDel = db.KTTU_PAYMENT_DETAILS.Where(est => est.series_no == estNo
                                                                                        && est.pay_mode == "SE"
                                                                                        && est.company_code == companyCode
                                                                                        && est.branch_code == branchCode).ToList();
                foreach (KTTU_PAYMENT_DETAILS pay in paymentDel) {
                    db.KTTU_PAYMENT_DETAILS.Remove(pay);
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }
        #endregion

        #region Private Methods
        private string GenerateHTMLPrint40Column(string companyCode, string branchCode, int srEstNo)
        {
            StringBuilder sbStart = new StringBuilder();
            StringBuilder strTransLine = new StringBuilder();
            strTransLine.Append('-', 60);
            decimal Spaces = 0M;
            StringBuilder strSpaces = new StringBuilder();
            Spaces = ((60 - strTransLine.Length) / 3);
            strSpaces.Append(' ', Convert.ToInt32(Spaces));
            string esc = Convert.ToChar(27).ToString();



            KTTU_SR_MASTER srMaster = db.KTTU_SR_MASTER.Where(sr => sr.company_code == companyCode
                                                              && sr.branch_code == branchCode
                                                              && sr.est_no == srEstNo).FirstOrDefault();
            if (srMaster != null) {
                string smetaltype = SIGlobals.Globals.GetMetalType(db, srMaster.gs_type, companyCode, branchCode);
                List<KTTU_SR_DETAILS> srDetails = db.KTTU_SR_DETAILS.Where(d => d.company_code == companyCode
                                                                               && d.branch_code == branchCode
                                                                               && d.est_no == srEstNo).ToList();

                decimal Amount = 0;
                int PCss = 0;
                decimal G_wt = 0, S_wt = 0, N_wt = 0, Gold_Value = 0, Stone_Chg = 0, Value_Amt = 0, additionalDiscount = 0, mcdiscount = 0;
                decimal GrossAmount = 0, TotalGrossAmount = 0, TotalTaxable = 0;
                decimal tax = 0;
                int MaxPage_Row = 1;

                if (srMaster.tax_amt == 0) {
                    tax = 0;
                }
                else {
                    tax = srDetails.Sum(d => d.net_amount);
                }

                sbStart.AppendLine("<Table  font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse; width: 366px;\">");
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD  style=\"border-right:thin; border-bottom:thin solid; border-left:thin\" colspan=9 ALIGN = \"CENTER\"><b>SALES RETURN ESTIMATION <br></b></TD>"));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<TR>");

                sbStart.AppendLine(string.Format("<TH  style=\"border-top:0; border-right:thin; border-bottom:thin; border-left:thin\" colspan=5 ALIGN = \"LEFT\"><b>Sales Bill No: {0}<br></b></TD>", srMaster.voture_bill_no));
                sbStart.AppendLine(string.Format("<TH  style=\"border-top:0; border-right:thin; border-bottom:thin; border-left:thin\" colspan=4 ALIGN = \"RIGHT\"><b>Bill Date: {0}<br></b></TD>", Convert.ToDateTime(srMaster.bill_date).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)));
                sbStart.AppendLine("</TR>");

                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TH  style=\"border-top:0; border-right:thin; border-bottom:thin; border-left:thin\" colspan=5 ALIGN = \"LEFT\"><b>SR Est.No: {0}<br></b></TD>", srMaster.est_no));
                if ((string.Compare(smetaltype, "GL") == 0)) {
                    sbStart.AppendLine(string.Format("<TH  style=\"border-top:0; border-right:thin; border-bottom:thin; border-left:thin\" colspan=4 ALIGN = \"RIGHT\" ><b> Gold Rate : {0}<br></b></TD>", srMaster.rate));
                }
                else if (string.Compare(smetaltype, "SL") == 0) {
                    sbStart.AppendLine(string.Format("<TH  style=\"border-top:0; border-right:thin; border-bottom:thin; border-left:thin\" colspan=4 ALIGN = \"RIGHT\" ><b> Silver Rate : {0}<br></b></TD>", srMaster.rate));
                }
                else if (string.Compare(smetaltype, "PT") == 0) {
                    sbStart.AppendLine(string.Format("<TH  style=\"border-top:0; border-right:thin; border-bottom:thin; border-left:thin\" colspan=4 ALIGN = \"RIGHT\" ><b> Rate : {0}<br></b></TD>", srMaster.rate));
                }
                else if (string.Compare(smetaltype, "DM") == 0) {
                    sbStart.AppendLine(string.Format("<TH  style=\"border-top:0; border-right:thin; border-bottom:thin; border-left:thin\" colspan=4 ALIGN = \"RIGHT\" ><b>Rate : {0}<br></b></TD>", srMaster.rate));
                }
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TH  style=\"border-top:0; border-right:thin; border-bottom:thin; border-left:thin\" colspan=7 ALIGN = \"LEFT\"><b>SR Date: {0}<br></b></TD>", Convert.ToDateTime(srMaster.sr_date).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)));
                sbStart.AppendLine("</TR>");

                sbStart.AppendLine("<TR bgcolor= WHITE style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sbStart.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b>Item</b></TH>");
                sbStart.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b>Qty</b></TH>");
                sbStart.AppendLine("<TH colspan = 3 ALIGN = \"CENTER\"><b>[Weight in Grams]</b></TH>");
                sbStart.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b>VA</b></TH>");
                sbStart.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b>Gold</b></TH>");
                sbStart.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b>St/Dmd</b></TH>");
                sbStart.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<TR bgcolor=WHITE style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid ; border-top:thin \" ALIGN = \"CENTER\"><b>Code</b></TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid\" ALIGN = \"CENTER\"><b>Gr.Wt</b></TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid\" ALIGN = \"CENTER\"><b>St.Wt</b></TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid\" ALIGN = \"CENTER\"><b>Nt.Wt</b></TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b>Amount</b></TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\"><b>Amount</b></TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid ; border-top:thin\" ALIGN = \"CENTER\"><b>Amount</b></TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid ; border-top:thin\" ALIGN = \"CENTER\"><b>Amount</b></TH>");
                sbStart.AppendLine("</TR>");

                for (int k = 0; k < srDetails.Count; k++) {
                    decimal gwt = Convert.ToDecimal(srDetails[k].gwt) + Convert.ToDecimal(srDetails[k].AddWt) - Convert.ToDecimal(srDetails[k].DeductWt);
                    decimal nwt = Convert.ToDecimal(srDetails[k].nwt) + Convert.ToDecimal(srDetails[k].AddWt) - Convert.ToDecimal(srDetails[k].DeductWt);
                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"LEFT\"><b>{0}</b></TD>", srDetails[k].item_name));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"LEFT\"><b>{0}</b></TD>", srDetails[k].quantity));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", gwt));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", srDetails[k].swt));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", nwt));

                    decimal MCAmount = 0;
                    if (Convert.ToDecimal(srDetails[k].making_charge_per_rs) > 0) {
                        MCAmount = Convert.ToDecimal(nwt) * Convert.ToDecimal(srDetails[k].making_charge_per_rs);
                    }
                    else if (Convert.ToDecimal(srDetails[k].va_percent) > 0) {
                        MCAmount = (Convert.ToDecimal(srDetails[k].sr_amount) * Convert.ToDecimal(srDetails[k].va_percent)) / 100;
                    }
                    else {
                        MCAmount = Convert.ToDecimal(srDetails[k].va_amount);
                    }
                    MCAmount = decimal.Round(MCAmount, 2);
                    Value_Amt += MCAmount;

                    decimal stoneCharges = Convert.ToDecimal(srDetails[k].stone_charges) + Convert.ToDecimal(srDetails[k].diamond_charges);
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  ALIGN = \"RIGHT\"><b>{0}</b></TD>", MCAmount));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  ALIGN = \"RIGHT\"><b>{0}</b></TD>", stoneCharges));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  ALIGN = \"RIGHT\"><b>{0}</b></TD>", srDetails[k].sr_amount));

                    GrossAmount = MCAmount + Convert.ToDecimal(stoneCharges) + Convert.ToDecimal(srDetails[k].sr_amount);

                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  ALIGN = \"RIGHT\"><b>{0}</b></TD>", GrossAmount));
                    sbStart.AppendLine("</TR>");
                    TotalGrossAmount += GrossAmount;

                    TotalTaxable += Convert.ToDecimal(srDetails[k].item_total_after_discount);
                    PCss += Convert.ToInt32(srDetails[k].quantity);
                    G_wt += gwt;
                    S_wt += Convert.ToDecimal(srDetails[k].swt);
                    N_wt += nwt;
                    Stone_Chg += stoneCharges;
                    Gold_Value += Convert.ToDecimal(srDetails[k].sr_amount);
                    additionalDiscount += Convert.ToDecimal(srDetails[k].item_additional_discount);
                    mcdiscount += Convert.ToDecimal(srDetails[k].Mc_Discount_Amt);
                    Amount = TotalGrossAmount;

                }

                for (int j = 0; j < MaxPage_Row; j++) {
                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TH style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TH>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TH style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TH>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TH style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TH>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TH style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TH>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TH style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TH>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TH style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TH>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TH style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TH>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TH style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TH>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TH style=\"border-top:thin; border-bottom:thin\"  ALIGN = \"RIGHT\"><b>{0} </b></TH>", "&nbsp"));
                    sbStart.AppendLine("</TR>");
                }
                sbStart.AppendLine("<TR  bgcolor=WHITE  style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sbStart.AppendLine("<TH  style=\"border-top:thin solid; border-bottom:thin solid\" colspan=1 ALIGN = \"LEFT\"><b>Totals</b></TH>");

                sbStart.AppendLine(string.Format("<TH style=\"border-top:thin solid; border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}</b></TH>", PCss));
                sbStart.AppendLine(string.Format("<TH style=\"border-top:thin solid; border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}</b></TH>", G_wt));
                sbStart.AppendLine(string.Format("<TH style=\"border-top:thin solid; border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}</b></TH>", S_wt));
                sbStart.AppendLine(string.Format("<TH style=\"border-top:thin solid; border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}</b></TH>", N_wt));
                sbStart.AppendLine(string.Format("<TH style=\"border-top:thin solid; border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}</b></TH>", Value_Amt));
                sbStart.AppendLine(string.Format("<TH style=\"border-top:thin solid; border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}</b></TH>", Stone_Chg));
                sbStart.AppendLine(string.Format("<TH style=\"border-top:thin solid; border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}</b></TH>", Gold_Value));
                sbStart.AppendLine(string.Format("<TH style=\"border-top:thin solid; border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}</b></TH>", TotalGrossAmount));
                sbStart.AppendLine("</TR>");

                if (additionalDiscount > 0) {
                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TH style=\"border-bottom:thin; border-top:thin \"  colspan = 8 ALIGN = \"RIGHT\"><b>Less </b></TH>"));
                    sbStart.AppendLine(string.Format("<TH style=\"border-bottom:thin; border-top:thin \"  ALIGN = \"RIGHT\"><b>{0}</b></TH>", additionalDiscount));
                    sbStart.AppendLine("</TR>");
                    Amount = Amount - additionalDiscount;
                }
                if (mcdiscount > 0) {
                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TH style=\"border-bottom:thin; border-top:thin \"  colspan = 8 ALIGN = \"RIGHT\"><b>Less </b></TH>"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  ALIGN = \"RIGHT\"><b>{0}</b></TD>", decimal.Round(mcdiscount, 2)));
                    sbStart.AppendLine("</TR>");
                    Amount = Amount - mcdiscount;
                }
                if (Convert.ToDecimal(srMaster.excise_duty_amount) > 0) {
                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  colspan = 8 ALIGN = \"RIGHT\"><b>ED Amount </b></TD>"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  ALIGN = \"RIGHT\"><b>{0}</b></TD>", srMaster.excise_duty_amount));
                    sbStart.AppendLine("</TR>");
                    Amount = Amount + Convert.ToDecimal(srMaster.excise_duty_amount);
                    if (Convert.ToDecimal(srMaster.ed_cess_amount) > 0) {
                        sbStart.AppendLine("<TR>");
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  colspan = 8 ALIGN = \"RIGHT\"><b>ED Cess</b></TD>"));
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  ALIGN = \"RIGHT\"><b>{0}</b></TD>", srMaster.ed_cess_amount));
                        sbStart.AppendLine("</TR>");
                        Amount = Amount + Convert.ToDecimal(srMaster.ed_cess_amount);
                    }
                    if (Convert.ToDecimal(srMaster.hed_cess_amount) > 0) {
                        sbStart.AppendLine("<TR>");
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  colspan = 8 ALIGN = \"RIGHT\"><b>HED Cess</b></TD>"));
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  ALIGN = \"RIGHT\"><b>{0}</b></TD>", srMaster.hed_cess_amount));
                        sbStart.AppendLine("</TR>");
                        Amount = Amount + Convert.ToDecimal(srMaster.hed_cess_amount);
                    }
                }
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  colspan = 8 ALIGN = \"RIGHT\"><b>Taxable Value</b></TD>"));
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  ALIGN = \"RIGHT\"><b>{0}</b></TD>", TotalTaxable));
                sbStart.AppendLine("</TR>");

                decimal cgstAmount = Convert.ToDecimal(srDetails.Sum(gst => gst.CGST_Amount));
                decimal sgstAmount = Convert.ToDecimal(srDetails.Sum(gst => gst.SGST_Amount));
                decimal igstAmount = Convert.ToDecimal(srDetails.Sum(gst => gst.IGST_Amount));
                decimal finalAmount = Math.Round(TotalTaxable + sgstAmount + cgstAmount + igstAmount, 0, MidpointRounding.AwayFromZero);
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  colspan = 8 ALIGN = \"RIGHT\"><b>CGST {0}%</b></TD>", srDetails[0].CGST_Percent));
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  ALIGN = \"RIGHT\"><b>{0}</b></TD>", cgstAmount));
                sbStart.AppendLine("</TR>");
                Amount = Amount + cgstAmount;
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  colspan = 8 ALIGN = \"RIGHT\"><b>SGST {0}%</b></TD>", srDetails[0].SGST_Percent));
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  ALIGN = \"RIGHT\"><b>{0}</b></TD>", sgstAmount));
                sbStart.AppendLine("</TR>");
                Amount = Amount + sgstAmount;
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  colspan = 8 ALIGN = \"RIGHT\"><b>IGST {0}%</b></TD>", srDetails[0].IGST_Percent));
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  ALIGN = \"RIGHT\"><b>{0}</b></TD>", igstAmount));
                sbStart.AppendLine("</TR>");
                Amount = Amount + igstAmount;

                Amount = Math.Round(Amount, 0, MidpointRounding.ToEven);
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin solid\"  colspan = 8 ALIGN = \"RIGHT\"><b>Invoice Amount</b></TD>"));
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin solid\"  ALIGN = \"RIGHT\"><b>{0}</b></TD>", finalAmount));
                sbStart.AppendLine("</TR>");
                string strWords = string.Empty;
                NumberToWords objNWClass = new NumberToWords();
                objNWClass.ConvertNumberToWords(Convert.ToDouble(Amount), out strWords);
                strWords = strWords.Replace("Rupees", "");
                strWords = "Rupees " + strWords;
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin solid; \" colspan = 9 ALIGN = \"LEFT\"><b>{0}</b></TD>", strWords));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin solid; \" colspan = 9 ALIGN = \"LEFT\"><b>Sal : {0} / :# {1}</b></TD>", srMaster.sal_code, srMaster.operator_code));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("</TABLE>");
            }
            return sbStart.ToString();
        }
        #endregion
    }
}
