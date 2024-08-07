using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.BL.ViewModel.Payment;
using ProdigyAPI.BL.ViewModel.Sales;
using ProdigyAPI.Framework;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.OData.Query;
using ProdigyAPI.BL.BusinessLayer.Sales;
using System.Data.Entity.SqlServer;
using ProdigyAPI.BL.BusinessLayer.Masters;

namespace ProdigyAPI.Controllers.Sales
{
    /// <summary>
    /// Sales Return controller provieds API's for Sales Return Estimation, SR Attachment etc.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/SalesReturnEst")]
    public class SalesReturnEstimationController : SIBaseApiController<SalesReturnMasterVM>, IBaseMasterActionController<SalesReturnMasterVM, SalesReturnMasterVM>
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        string ModuleSeqNo = "3";
        private const string TABLE_NAME = "KTTU_SR_MASTER";
        #endregion

        #region Controler Methods

        #region Sales Return
        /// <summary>
        /// Get Billed Branch Information.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("BilledBranch/{companyCode}/{branchCode}")]
        [Route("BilledBranch")]
        public IHttpActionResult GetBilledBrach(string companyCode, string branchCode)
        {
            //List<KSTU_SUPPLIER_MASTER> ksmd = db.KSTU_SUPPLIER_MASTER.Where(ksm => ksm.obj_status == "O" && ksm.company_code == Common.CompanyCode && ksm.branch_code == Common.BranchCode && ksm.voucher_code == "VB").ToList();
            //return Ok(ksmd);

            return Ok(db.KSTU_SUPPLIER_MASTER.Where(ksm => ksm.obj_status == "O"
            && ksm.company_code == companyCode && ksm.branch_code == branchCode && ksm.voucher_code == "VB" && ksm.company_code == companyCode
                                                        && ksm.branch_code == branchCode).Select(ksm => new BilledBranchVM()
                                                        {
                                                            PartyCode = ksm.party_code,
                                                            PartyName = ksm.party_name
                                                        }));
        }

        /// <summary>
        /// Get Sales Return Estimatio details by Estimation No.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="billNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Get/{companyCode}/{branchCode}/{billNo}")]
        [Route("Get")]
        public IHttpActionResult GetSalesReturnEstimationDetails(string companyCode, string branchCode, int billNo)
        {
            KTTU_SALES_MASTER ksm = new KTTU_SALES_MASTER();
            List<KTTU_SALES_DETAILS> ksd = new List<KTTU_SALES_DETAILS>();
            int orgItemsCount = 0;

            #region Validation
            // Checking Sales Details for the Selected Bill Number.
            #region 
            ksm = db.KTTU_SALES_MASTER.Where(sale => sale.bill_no == billNo && sale.company_code == companyCode
                                                        && sale.branch_code == branchCode).FirstOrDefault();
            if (ksm == null) {
                return Content(
                    HttpStatusCode.NotFound, new ErrorVM
                    {
                        index = 0,
                        field = "",
                        description = "Invalid Bill Number.",
                        ErrorStatusCode = HttpStatusCode.NotFound
                    });
            }
            else if (ksm.cflag == "Y") {
                return Content(
                   HttpStatusCode.NotFound, new ErrorVM
                   {
                       index = 0,
                       field = "",
                       description = "Bill already cancelled",
                       ErrorStatusCode = HttpStatusCode.NotFound
                   });
            }
            else {
                ksd = db.KTTU_SALES_DETAILS.Where(sale => sale.bill_no == billNo && sale.company_code == companyCode && sale.branch_code == branchCode).ToList();
                if (ksd.Count > 0) {
                    orgItemsCount = ksd.Count;
                }
            }
            #endregion

            // Checking SR alreday done for the Bill Number, If done checking all the items are done SR If not then we are sending back only the remaining items.
            List<KTTU_SR_MASTER> salesReturn = db.KTTU_SR_MASTER.Where(sr => sr.voture_bill_no == billNo
                                                                && sr.company_code == companyCode
                                                                && sr.branch_code == branchCode
                                                                && sr.cflag == "N").ToList();
            foreach (KTTU_SR_MASTER srRet in salesReturn) {
                if (salesReturn != null) {
                    List<KTTU_SR_DETAILS> srDet = db.KTTU_SR_DETAILS.Where(sr => sr.est_no == srRet.est_no
                                                                        && sr.company_code == companyCode
                                                                        && sr.branch_code == branchCode).ToList();
                    if (srDet.Count == orgItemsCount) {
                        return Content(
                            HttpStatusCode.NotFound, new ErrorVM
                            {
                                index = 0,
                                field = "",
                                description = "No items to return",
                                ErrorStatusCode = HttpStatusCode.NotFound
                            });
                    }
                    else {
                        foreach (KTTU_SR_DETAILS srd in srDet) {
                            if (srd.barcode_no == "") {
                                ksd.RemoveAll(s => s.sl_no == srd.sl_no);
                            }
                            else {
                                ksd.RemoveAll(s => s.barcode_no == srd.barcode_no);
                            }
                        }
                    }
                }
            }

            // Sales Bill Age Calculation and Validation
            KSTU_TOLERANCE_MASTER tollerance = db.KSTU_TOLERANCE_MASTER.Where(toll => toll.company_code == companyCode && toll.branch_code == branchCode && toll.obj_id == 20072018).FirstOrDefault();
            TimeSpan noOfDays = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode) - ksm.bill_date;
            if (Convert.ToInt32(noOfDays.Days) > tollerance.Max_Val) {
                return Content(
                    HttpStatusCode.NotFound, new ErrorVM
                    {
                        index = 0,
                        field = "",
                        description = "The Bill is Too old, Cannot do Sales Return (" + Convert.ToInt32(noOfDays.Days) + " days)",
                        ErrorStatusCode = HttpStatusCode.NotFound
                    });
            }

            #endregion

            //Checking whether its a credit bill or not to enable order in the UI
            #region Credit Bill Details
            bool isCreditBill = false;
            decimal masterBalaceAmt = 0;
            decimal partialPaidAmt = 0;
            isCreditBill = new ConfirmSalesReturnBL().CheckIsCreditBillWithBalance(companyCode, branchCode, billNo, out masterBalaceAmt, out partialPaidAmt);
            #endregion

            int customerID = Convert.ToInt32(ksm.cust_Id);
            SalesReturnMasterVM srvm = new SalesReturnMasterVM();
            srvm.ObjID = ksm.obj_id;
            srvm.CompanyCode = ksm.company_code;
            srvm.BranchCode = ksm.branch_code;
            srvm.VotureBillNo = ksm.bill_no;
            srvm.SalesBillNo = 0;
            srvm.EstNo = 0;
            srvm.CustomerID = customerID;
            srvm.MobileNo = db.KSTU_CUSTOMER_MASTER.Where(cust => cust.cust_id == customerID && cust.company_code == companyCode && cust.branch_code == branchCode).FirstOrDefault().mobile_no;
            srvm.BillDate = ksm.bill_date;
            srvm.SalesReturnDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
            srvm.GSType = ksm.gstype;
            srvm.TaxAmount = ksm.total_tax_amount;
            srvm.TotalSRAmount = Convert.ToDecimal(ksm.grand_total);
            srvm.OperatorCode = ksm.operator_code; // Need to send From UI
            srvm.SalCode = "";// Need to send from UI
            srvm.BillCounter = ksm.bill_counter;
            srvm.Remarks = ksm.remarks;
            srvm.UpdateOn = ksm.UpdateOn;
            srvm.ISIns = ksm.is_ins;
            srvm.Rate = ksm.rate;
            srvm.BilledBranch = ""; // Need to Send From UI
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
            srvm.IsCreditBill = isCreditBill;
            srvm.VPAmount = masterBalaceAmt;

            List<SalesReturnDetailsVM> lstSRD = new List<SalesReturnDetailsVM>();
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
                List<KTTU_SALES_STONE_DETAILS> kssd = db.KTTU_SALES_STONE_DETAILS.Where(sale => sale.bill_no == billNo && sale.barcode_no == srd.BarcodeNo && sale.company_code == companyCode
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
            return Ok(srvm);
        }

        /// <summary>
        /// Get Saved Sales return Information.
        /// </summary>
        /// <param name="estNo"></param>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSavedSR/{companyCode}/{branchCode}/{estNo}")]
        [Route("GetSavedSR")]
        public IHttpActionResult GetSavedSRInfo(string companyCode, string branchCode, int estNo)
        {
            KTTU_SR_MASTER ksm = db.KTTU_SR_MASTER.Where(sale => sale.est_no == estNo && sale.company_code == companyCode
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
            List<KTTU_SR_DETAILS> ksd = db.KTTU_SR_DETAILS.Where(sale => sale.est_no == estNo && sale.company_code == companyCode
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
                List<KTTU_SR_STONE_DETAILS> kssd = db.KTTU_SR_STONE_DETAILS.Where(sale => sale.est_no == estNo && sale.barcode_no == srd.BarcodeNo
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
            return Ok(srvm);
        }

        /// <summary>
        /// Get Sales Return Estimatio details by Estimation No.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAttachedSRForPrint/{companyCode}/{branchCode}/{estNo}")]
        [Route("GetAttachedSRForPrint")]
        public IHttpActionResult GetAttachedSalesReturnEstimationDetailsForPrint(string companyCode, string branchCode, int estNo)
        {
            ErrorVM error = new ErrorVM();
            SalesReturnMasterVM srm = new SalesReturnEstimationBL().GetAttachedSalesReturnEstimationDetailsForPrint(companyCode, branchCode, estNo, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(srm);
        }

        /// <summary>
        /// Get Sales Return Estimatio details by Estimation No.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAttachedSRTotalForPrint/{companyCode}/{branchCode}/{estNo}")]
        [Route("GetAttachedSRTotalForPrint")]
        public IHttpActionResult GetAttachedSalesReturnEstimationDetailsTotalForPrint(string companyCode, string branchCode, int estNo)
        {
            ErrorVM error = new ErrorVM();
            SalesReturnMasterVM srm = new SalesReturnEstimationBL().GetAttachedSalesReturnEstimationDetailsTotalForPrint(companyCode, branchCode, estNo, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(srm);
        }

        /// <summary>
        /// Save sales return information.
        /// </summary>
        /// <param name="salesReturn"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult saveSalesReturn([FromBody] SalesReturnMasterVM salesReturn)
        {
            KSTU_CUSTOMER_MASTER customer = new CustomerBL().GetActualCustomerDetails(salesReturn.CustomerID, salesReturn.MobileNo, salesReturn.CompanyCode, salesReturn.BranchCode);
            KTTU_SALES_MASTER ksmv = db.KTTU_SALES_MASTER.Where(sale => sale.bill_no == salesReturn.VotureBillNo
                                             && sale.company_code == salesReturn.CompanyCode
                                             && sale.branch_code == salesReturn.BranchCode).FirstOrDefault();

            decimal totalSRAmount = Convert.ToDecimal(salesReturn.lstOfSalesReturnDetails.Sum(ret => ret.ItemFinalAmountAfterRoundOff));
            decimal totalTaxAmount = Convert.ToDecimal(salesReturn.lstOfSalesReturnDetails.Sum(ret => ret.CGSTAmount + ret.SGSTAmount + ret.IGSTAmount));

            if (ksmv == null) {
                return Content(HttpStatusCode.NotFound, new ErrorVM { index = 0, field = "", description = "Sales Details Not Found", ErrorStatusCode = HttpStatusCode.NotFound });
            }
            using (var transaction = db.Database.BeginTransaction()) {
                try {
                    int estNo = db.KSTS_SEQ_NOS.Where(sq => sq.obj_id == ModuleSeqNo && sq.company_code == salesReturn.CompanyCode && sq.branch_code == salesReturn.BranchCode).FirstOrDefault().nextno;
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
                    ksm.tax_amt = totalTaxAmount;// ksmv.total_tax_amount;
                    ksm.total_sramount = totalSRAmount;// Convert.ToDecimal(ksmv.grand_total);
                    ksm.operator_code = ksmv.operator_code;
                    ksm.sal_code = salesReturn.SalCode;
                    ksm.bill_counter = ksmv.bill_counter;
                    ksm.remarks = ksmv.remarks;
                    ksm.UpdateOn = SIGlobals.Globals.GetDateTime();
                    ksm.is_ins = ksmv.is_ins;
                    ksm.rate = ksmv.rate;
                    ksm.billed_branch = salesReturn.BilledBranch == null ? salesReturn.BranchCode : salesReturn.BilledBranch;
                    ksm.cflag = ksmv.cflag;
                    ksm.excise_duty_percent = ksmv.excise_duty_percent;
                    ksm.ed_cess_percent = ksmv.ed_cess_percent;
                    ksm.hed_cess_percent = ksmv.hed_cess_percent;
                    ksm.excise_duty_amount = ksmv.excise_duty_amount;
                    ksm.ed_cess_amount = ksmv.ed_cess;
                    ksm.hed_cess_amount = ksmv.hed_cess;
                    ksm.discount_amt = 0;
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
                        ksd.item_additional_discount = srd.OfferValue + srd.ItemAdditionalDiscount;
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
                        ksd.CESSAmount = Convert.ToDecimal(srd.CessAmount);
                        ksd.CESSPercent = Convert.ToDecimal(srd.CessPercent);
                        ksd.isBarcoded = "N";
                        db.KTTU_SR_DETAILS.Add(ksd);

                        int SlNo = 1;
                        foreach (SalesReturnStoneDetailsVM srsd in srd.lstOfStoneDetails) {
                            KTTU_SR_STONE_DETAILS kssd = new KTTU_SR_STONE_DETAILS();
                            kssd.obj_id = objectID;
                            kssd.company_code = srsd.CompanyCode;
                            kssd.branch_code = srsd.BranchCode;
                            kssd.sales_bill_no = 0;
                            kssd.est_no = ksm.est_no;
                            kssd.item_sno = srd.SlNo; //srsd.ItemSno;
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
                    SIGlobals.Globals.UpdateSeqenceNumber(db, ModuleSeqNo, salesReturn.CompanyCode, salesReturn.BranchCode);
                    db.SaveChanges();
                    transaction.Commit();
                    return Ok(new { SREstimationNo = ksm.est_no });
                }
                catch (Exception excp) {
                    transaction.Rollback();
                    throw excp;
                }
            }
        }

        /// <summary>
        /// Print Estimation
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="srEstNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Print/{companyCode}/{branchCode}/{srEstNo}")]
        public IHttpActionResult SREstiamtionPrint(string companyCode, string branchCode, int srEstNo)
        {
            ErrorVM error = new ErrorVM();
            BL.ViewModel.Print.ProdigyPrintVM print = new SalesReturnEstimationBL().GetSREstimatePrint(companyCode, branchCode, srEstNo, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(print);
        }

        /// <summary>
        /// Delete Sales Return Estimation
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllSREst/{companyCode}/{branchCode}")]
        public IHttpActionResult GetAllSREstimations(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            dynamic data = new SalesReturnEstimationBL().GetAllSREstimations(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Delete Sales Return Estimation
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Delete/{companyCode}/{branchCode}/{estNo}")]
        public IHttpActionResult DeleteEstimation(string companyCode, string branchCode, int estNo)
        {
            ErrorVM error = new ErrorVM();
            bool print = new SalesReturnEstimationBL().DeleteSREstimation(companyCode, branchCode, estNo, out error);
            if (error == null) {
                return Ok();
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }
        public IQueryable<SalesReturnMasterVM> List()
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Count(ODataQueryOptions<SalesReturnMasterVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Post(SalesReturnMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, SalesReturnMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Delete([FromBody] int id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Attach SR Estimation
        /// <summary>
        /// Get all sales return estimaton details.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllSRAttach/{companyCode}/{branchCode}")]
        [Route("GetAllSRAttach")]
        public IQueryable<AllSalesReturnVM> GetSRDetails(string companyCode, string branchCode)
        {
            List<AllSalesReturnVM> data = (from ksm in db.KTTU_SR_MASTER
                                           join ksd in db.KTTU_SR_DETAILS
                                           on new { CC = ksm.company_code, BC = ksm.branch_code, EstNo = ksm.est_no }
                                            equals new { CC = ksd.company_code, BC = ksd.branch_code, EstNo = ksd.est_no }
                                           where ksm.sales_bill_no == 0 && ksm.company_code == companyCode && ksm.branch_code == branchCode
                                           group ksd by new { ksm.est_no, ksm.sr_date, ksm.total_sramount, ksm.cust_id } into g
                                           select new AllSalesReturnVM
                                           {
                                               SrEstNo = g.Key.est_no,
                                               Date = g.Key.sr_date.ToString(),
                                               Amount = g.Key.total_sramount,
                                               GrossWt = g.Sum(ksd => ksd.gwt),
                                               Quantity = g.Sum(ksd => ksd.quantity),
                                               TaxAmount = g.Sum(ksd => ksd.tax_amount).ToString(),
                                               CustID = g.Key.cust_id,
                                           }).OrderBy(e => e.SrEstNo).ToList();
            //return data.AsQueryable<AllSalesReturnVM>();
            // Code added on 17/04/2020. removing already attached SR Estimation numbers.
            var attachedSR = db.KTTU_PAYMENT_DETAILS.Where(p => p.company_code == companyCode && p.branch_code == branchCode && p.trans_type == "A" && p.pay_mode == "SE").Select(s => s.Ref_BillNo).ToList();
            var intAttachedSr = ConvertList(attachedSR);
            var result = data.Where(r => !intAttachedSr.Contains(r.SrEstNo));
            return result.AsQueryable<AllSalesReturnVM>();
        }

        /// <summary>
        /// Get Sales Return Estimation Count.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllSRAttachCount/{companyCode}/{branchCode}")]
        [Route("GetAllSRAttachCount")]
        public IHttpActionResult GetAllOrdersCount(string companyCode, string branchCode)
        {
            List<AllSalesReturnVM> lstAllSR = GetSRDetails(companyCode, branchCode).ToList();
            return Ok(new { RecordCount = lstAllSR.Count() });
        }

        /// <summary>
        /// Get attached sales return estimation information.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAttachedSR/{companyCode}/{branchCode}/{estNo}")]
        [Route("GetAttachedSR")]
        public IHttpActionResult GetAttachedSalesReturnDetails(string companyCode, string branchCode, int estNo)
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
            List<KTTU_PAYMENT_DETAILS> payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.company_code == companyCode && pay.branch_code == branchCode
                 && pay.series_no == estNo && pay.pay_mode == "SE" && pay.trans_type == "A").ToList();
            List<AllSalesReturnVM> lstAllSR = new List<AllSalesReturnVM>();
            foreach (KTTU_PAYMENT_DETAILS payDet in payment) {
                int srEstNo = Convert.ToInt32(payDet.Ref_BillNo);
                AllSalesReturnVM sr = (from ksm in db.KTTU_SR_MASTER
                                       join ksd in db.KTTU_SR_DETAILS
                                       on new { CC = ksm.company_code, BC = ksm.branch_code, EstNo = ksm.est_no }
                                       equals new { CC = ksd.company_code, BC = ksd.branch_code, EstNo = ksd.est_no }
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
                                       }).FirstOrDefault();
                if (sr != null)
                    lstAllSR.Add(sr);
            }
            return Ok(lstAllSR);
        }

        /// <summary>
        /// Searching parameters for seles return estimations.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("searchParams")]
        public IHttpActionResult GetAllSearchParameters()
        {
            List<SearchParamVM> lstSearchParams = new List<SearchParamVM>();
            lstSearchParams.Add(new SearchParamVM() { Key = "SrEstNo", Value = "SRESTNO" });
            lstSearchParams.Add(new SearchParamVM() { Key = "Date", Value = "DATE" });
            lstSearchParams.Add(new SearchParamVM() { Key = "Amount", Value = "AMOUNT" });
            lstSearchParams.Add(new SearchParamVM() { Key = "GrossWt", Value = "GROSSWT" });
            lstSearchParams.Add(new SearchParamVM() { Key = "Quantity", Value = "QUANTITY" });
            lstSearchParams.Add(new SearchParamVM() { Key = "TaxAmount", Value = "TAXAMOUNT" });
            lstSearchParams.Add(new SearchParamVM() { Key = "CustID", Value = "CUSTID" });
            return Ok(lstSearchParams);
        }

        /// <summary>
        /// Get sercherd sales return estimations by searching parameters.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="searchType"></param>
        /// <param name="branchCode"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSRAttachSearch/{companyCode}/{branchCode}/{searchType}/{searchValue}")]
        [Route("GetSRAttachSearch")]
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

        /// <summary>
        /// Get Attached Sales Return Estimation Details by Sales Estimation Number.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSRAttachment/{companyCode}/{branchCode}/{estNo}")]
        [Route("GetSRAttachment")]
        public IHttpActionResult GetAttachedOrder(string companyCode, string branchCode, int estNo)
        {
            List<KTTU_PAYMENT_DETAILS> payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == estNo && pay.pay_mode == "SE" && pay.trans_type == "A"
                                                    && pay.company_code == companyCode && pay.branch_code == branchCode).ToList();
            List<SalesReturnMasterVM> lstOfSRMaster = new List<SalesReturnMasterVM>();
            foreach (KTTU_PAYMENT_DETAILS pay in payment) {
                SalesReturnMasterVM om = new SalesReturnMasterVM();
                int refBillNo = Convert.ToInt32(pay.Ref_BillNo);
                IHttpActionResult actionResult = GetSavedSRInfo(companyCode, branchCode, Convert.ToInt32(refBillNo));
                var contentResult = actionResult as OkNegotiatedContentResult<SalesReturnMasterVM>;
                om = (SalesReturnMasterVM)contentResult.Content;
                lstOfSRMaster.Add(om);
            }
            return Ok(lstOfSRMaster);
        }

        /// <summary>
        /// Save/Attach Sales Return Estimation attachment details.
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostSRAttachment")]
        public IHttpActionResult postSRAttachment(List<PaymentVM> payment)
        {
            if (payment == null) {
                return Content(HttpStatusCode.BadRequest, new ErrorVM()
                {
                    description = "Invalid Sales return details",
                    ErrorStatusCode = HttpStatusCode.BadRequest
                });
            }

            try {
                int estNo = payment[0].SeriesNo;
                string companyCode = payment[0].CompanyCode;
                string branchCode = payment[0].BranchCode;

                if (estNo == 0) {
                    return Content(HttpStatusCode.BadRequest, new ErrorVM()
                    {
                        description = "Invalid Sales Return Estimation No",
                        ErrorStatusCode = HttpStatusCode.BadRequest
                    });
                }

                List<KTTU_PAYMENT_DETAILS> paymentDel = db.KTTU_PAYMENT_DETAILS.Where(est => est.series_no == estNo
                                && est.pay_mode == "SE"
                                && est.trans_type == "A"
                                && est.company_code == companyCode
                                && est.branch_code == branchCode).ToList();
                foreach (KTTU_PAYMENT_DETAILS pay in paymentDel) {
                    db.KTTU_PAYMENT_DETAILS.Remove(pay);
                }

                var payDetailSlNo = db.KTTU_PAYMENT_DETAILS.Where(est => est.series_no == estNo
                                                                    && est.company_code == companyCode
                                                                    && est.branch_code == branchCode
                                                                    && est.trans_type == "A")
                                                           .Select(m => m.sno).DefaultIfEmpty(0).Max();
                int paySlNo = 1;
                if (payDetailSlNo > 0) {
                    paySlNo = payDetailSlNo + 1;
                }

                KTTU_SALES_EST_MASTER salEstimation = db.KTTU_SALES_EST_MASTER.Where(est => est.est_no == estNo
                                                                                    && est.company_code == companyCode
                                                                                    && est.branch_code == branchCode).FirstOrDefault();
                if (salEstimation == null) {
                    var error = new ErrorVM()
                    {
                        description = "The underlying sales estimation does not exist.",
                        ErrorStatusCode = HttpStatusCode.BadRequest
                    };
                    return Content(HttpStatusCode.BadRequest, error);
                }
                if (salEstimation.bill_no != 0) {
                    var error = new ErrorVM()
                    {
                        description = "The Sales Estimation is billed already. The bill number is :" + salEstimation.bill_no.ToString(),
                        ErrorStatusCode = HttpStatusCode.BadRequest
                    };
                    return Content(HttpStatusCode.BadRequest, error);
                }
                string objID = salEstimation.obj_id;
                foreach (PaymentVM pay in payment) {
                    int refBillNo = Convert.ToInt32(pay.RefBillNo);
                    KTTU_SR_MASTER salesReturn = db.KTTU_SR_MASTER.Where(sr => sr.est_no == refBillNo
                        && sr.company_code == companyCode && sr.branch_code == branchCode).FirstOrDefault();
                    if (salesReturn == null) {
                        var error = new ErrorVM()
                        {
                            description = "Invalid SR Estimation. The SR estimation does not exist.",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return Content(HttpStatusCode.BadRequest, error);
                    }
                    if (salesReturn.sales_bill_no != 0) {
                        var error = new ErrorVM()
                        {
                            description = "The SR Estimation is billed already. The bill number is :" + salesReturn.sales_bill_no.ToString(),
                            ErrorStatusCode = HttpStatusCode.BadRequest
                        };
                        return Content(HttpStatusCode.BadRequest, error);
                    }
                    KTTU_PAYMENT_DETAILS kpd = new KTTU_PAYMENT_DETAILS();
                    kpd.obj_id = objID;
                    kpd.company_code = companyCode;
                    kpd.branch_code = branchCode;
                    kpd.series_no = pay.SeriesNo;
                    kpd.receipt_no = 0;
                    kpd.sno = paySlNo;
                    kpd.trans_type = "A";
                    kpd.pay_mode = "SE";
                    kpd.pay_details = "";
                    kpd.pay_date = salesReturn.sr_date;
                    kpd.pay_amt = salesReturn.total_sramount;
                    kpd.Ref_BillNo = pay.RefBillNo;
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
                    kpd.operator_code = pay.OperatorCode;
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
                salEstimation.UpdateOn = SIGlobals.Globals.GetDateTime();
                db.Entry(salEstimation).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception excp) {
                return Content(HttpStatusCode.BadRequest, new ErrorVM().GetErrorDetails(excp));
            }
            return Ok();
        }

        /// <summary>
        /// This is used to Remove SR attachments for the particular Estimation.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("RemoveAttachment/{companyCode}/{branchCode}/{estNo}")]
        [Route("RemoveAttachment")]
        public IHttpActionResult RemoveSRAttachement(string companyCode, string branchCode, int estNo)
        {
            if (estNo == 0) {
                return Content(HttpStatusCode.BadRequest, new ErrorVM()
                {
                    description = "Invalid Sales Return Estimation Number",
                    ErrorStatusCode = HttpStatusCode.BadRequest
                });
            }
            try {
                List<KTTU_PAYMENT_DETAILS> paymentDel = db.KTTU_PAYMENT_DETAILS.Where(est => est.series_no == estNo
                                                                                        && est.pay_mode == "SE"
                                                                                        && est.trans_type == "A"
                                                                                        && est.company_code == companyCode
                                                                                        && est.branch_code == branchCode).ToList();
                foreach (KTTU_PAYMENT_DETAILS pay in paymentDel) {
                    db.KTTU_PAYMENT_DETAILS.Remove(pay);
                }
                db.SaveChanges();
                return Ok();
            }
            catch (Exception excp) {
                return Content(HttpStatusCode.BadRequest, new ErrorVM().GetErrorDetails(excp));
            }

        }
        #endregion

        #endregion

        #region Private Methods
        List<int> ConvertList(List<string> stringList)
        {
            List<int> intList = new List<int>();

            for (int i = 0; i < stringList.Count; i++) {
                intList.Add(int.Parse(stringList[i]));
            }
            return intList;
        }

        #endregion
    }
}
