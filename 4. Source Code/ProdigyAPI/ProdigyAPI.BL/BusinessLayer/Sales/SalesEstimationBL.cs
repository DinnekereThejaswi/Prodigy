using NumberToWordsINR;
using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.BL.BusinessLayer.Purchase;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Payment;
using ProdigyAPI.BL.ViewModel.Print;
using ProdigyAPI.BL.ViewModel.Sales;
using ProdigyAPI.BL.BusinessLayer.PrintConfig;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Globalization;

namespace ProdigyAPI.BL.BusinessLayer.Sales
{
    public class SalesEstimationBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities(true);
        string MODULE_SEQ_NO = "3";
        public bool isInterState = false;
        private const string TABLE_NAME = "KTTU_SALES_EST_MASTER";
        #endregion

        #region User Defined Methods
        public SalesEstMasterVM GetEstimationDetails(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            string message = string.Empty;
            bool isReservedOrders = false;
            List<int> reservedOrders = new List<int>();
            decimal totalBillAmount = 0;
            decimal totalOfferValue = 0;
            try {
                SalesEstMasterVM salesEst = new SalesEstMasterVM();
                List<SalesEstDetailsVM> lstOfSalesEstDet = new List<SalesEstDetailsVM>();

                KTTU_SALES_EST_MASTER ksem = new KTTU_SALES_EST_MASTER();
                List<KTTU_SALES_EST_DETAILS> ksed = new List<KTTU_SALES_EST_DETAILS>();
                List<KTTU_SALES_STONE_DETAILS> kssd = new List<KTTU_SALES_STONE_DETAILS>();

                // Checking valid Estimation Number or Not
                ksem = db.KTTU_SALES_EST_MASTER.Where(est => est.est_no == estNo
                                                        && est.company_code == companyCode
                                                        && est.branch_code == branchCode).FirstOrDefault();

                // Checking is there any Reserved order attached for this estimation.
                List<KTTU_PAYMENT_DETAILS> lstOfReservedOrder = new List<KTTU_PAYMENT_DETAILS>();
                lstOfReservedOrder = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == estNo
                                                                    && pay.company_code == companyCode
                                                                    && pay.branch_code == branchCode
                                                                    && pay.trans_type == "A"
                                                                    && pay.pay_mode == "OP"
                                                                    && pay.cflag == "N").ToList();
                if (lstOfReservedOrder.Count > 0) {
                    isReservedOrders = true;
                    foreach (KTTU_PAYMENT_DETAILS pay in lstOfReservedOrder) {
                        reservedOrders.Add(Convert.ToInt32(pay.Ref_BillNo));
                    }
                }

                if (ksem == null) {
                    error = new ErrorVM
                    {
                        index = 0,
                        field = "",
                        description = "Invalid estimation number.",
                        ErrorStatusCode = HttpStatusCode.NotFound
                    };
                    return null;
                }

                //This validation is required for Estimation, but If we through this error we can't display the details, so at the time of billing we handled it.
                if (ksem.bill_no != 0) {
                    error = new ErrorVM
                    {
                        index = 0,
                        field = "",
                        description = "Sales Est. No: " + estNo + " already Billed. Bill No: " + ksem.bill_no,
                        ErrorStatusCode = HttpStatusCode.NotFound
                    };
                    return null;
                }

                salesEst.ObjID = ksem.obj_id;
                salesEst.CompanyCode = ksem.company_code;
                salesEst.BranchCode = ksem.branch_code;
                salesEst.EstNo = ksem.est_no;
                salesEst.OrderNo = ksem.order_no;
                salesEst.CustID = ksem.Cust_Id;
                salesEst.CustName = ksem.Cust_Name;
                salesEst.OrderDate = ksem.order_date;
                salesEst.EstDate = ksem.est_date;
                salesEst.OperatorCode = ksem.operator_code;
                salesEst.Karat = ksem.karat;
                salesEst.Rate = ksem.rate;
                salesEst.Tax = ksem.tax;
                salesEst.TotalTaxAmount = ksem.total_tax_amount;
                salesEst.TotalEstAmount = ksem.total_est_amount;
                salesEst.GrandTotal = ksem.grand_total;
                salesEst.OrderAmount = ksem.order_amount;
                salesEst.PurchaseAmount = ksem.purchase_amount;
                salesEst.SPurchaseAmount = ksem.spurchase_amount;
                salesEst.SType = ksem.s_type;
                salesEst.BillNo = ksem.bill_no;
                salesEst.GSType = ksem.gstype;
                salesEst.DiscountAmount = ksem.discount_amount;
                salesEst.OfferDiscountAmount = Convert.ToDecimal(ksem.TotalDiscountVoucherAmt);
                salesEst.OfferCode = ksem.offerType;
                salesEst.ApprovedBy = ksem.approved_by;
                salesEst.IsIns = ksem.is_ins;
                salesEst.SrBillNo = ksem.sr_billno;
                salesEst.ItemSet = ksem.item_set;
                salesEst.Remarks = ksem.remarks;
                salesEst.ApproveFlag = ksem.approve_flag;
                salesEst.UpdateOn = ksem.UpdateOn;
                salesEst.EdCess = ksem.ed_cess;
                salesEst.HedCEss = ksem.hed_cess;
                salesEst.EdCessPercent = ksem.ed_cess_percent;
                salesEst.HedCessPercent = ksem.hed_cess_percent;
                salesEst.ExciseDutyPercent = ksem.excise_duty_percent;
                salesEst.ExciseDutyAmount = ksem.excise_duty_amount;
                salesEst.HiSchemeAmount = ksem.hi_scheme_amount;
                salesEst.HiSchemeNumber = ksem.hi_scheme_no;
                salesEst.HiBonusAmount = ksem.hi_bonus_amount;
                salesEst.Salutation = ksem.salutation;
                salesEst.NewBillNo = ksem.New_Bill_No;
                salesEst.IsPAN = ksem.isPAN;
                salesEst.RoundOff = ksem.round_off;
                salesEst.StateCode = ksem.state_code;
                salesEst.Pos = ksem.pos;
                salesEst.CorparateID = ksem.Corparate_ID;
                salesEst.CorporateBranchID = ksem.Corporate_Branch_ID;
                salesEst.EmployeeId = ksem.Employee_Id;
                salesEst.RegisteredMN = ksem.Registered_MN;
                salesEst.ProfessionID = ksem.profession_ID;
                salesEst.EmpcorpEmailID = ksem.Empcorp_Email_ID;
                salesEst.PhoneNo = ksem.phone_no;
                salesEst.EmailID = ksem.Email_ID;
                salesEst.IdType = ksem.id_type;
                salesEst.IdDetails = ksem.id_details;
                salesEst.PANNo = ksem.pan_no;
                salesEst.TIN = ksem.tin;
                salesEst.Pincode = ksem.pin_code;
                salesEst.MobileNo = ksem.mobile_no;
                salesEst.City = ksem.city;
                salesEst.State = ksem.state;
                salesEst.Address2 = ksem.address2;
                salesEst.Address3 = ksem.address3;
                salesEst.Address1 = ksem.address1;
                salesEst.RowRevisionString = ksem.RowVersionString;
                salesEst.Overwrite = false;

                ksed = db.KTTU_SALES_EST_DETAILS.Where(est => est.est_no == estNo
                                                        && est.company_code == companyCode
                                                        && est.branch_code == branchCode).OrderBy(x => x.sl_no).ToList();
                foreach (KTTU_SALES_EST_DETAILS sd in ksed) {
                    SalesEstDetailsVM sedvm = new SalesEstDetailsVM();
                    List<SalesEstStoneVM> salesEstStone = new List<SalesEstStoneVM>();
                    if (sd.barcode_no != "") {
                        kssd = db.KTTU_SALES_STONE_DETAILS.Where(est => est.est_no == estNo
                                                                    && est.barcode_no == sd.barcode_no
                                                                    && est.est_Srno == sd.sl_no
                                                                    && est.company_code == companyCode
                                                                    && est.branch_code == branchCode).ToList();
                    }
                    else {
                        kssd = db.KTTU_SALES_STONE_DETAILS.Where(est => est.est_no == estNo
                                                                    && est.est_Srno == sd.sl_no
                                                                    && est.company_code == companyCode
                                                                    && est.branch_code == branchCode).ToList();
                    }

                    // Added Barcode Level Validation on 25/09/2020

                    if (!string.IsNullOrEmpty(sd.barcode_no)) {
                        if (isReservedOrders) {
                            message = new BarcodeBL().ValidateBarcodeDetails(sd.barcode_no, companyCode, branchCode, reservedOrders);
                            if (!string.IsNullOrEmpty(message)) {
                                error = new ErrorVM
                                {
                                    index = 0,
                                    field = "",
                                    description = message,
                                    ErrorStatusCode = HttpStatusCode.BadRequest
                                };
                                return null;
                            }
                        }
                        else {
                            message = new BarcodeBL().ValidateBarcodeDetails(sd.barcode_no, companyCode, branchCode, ksem.order_no);
                            if (!string.IsNullOrEmpty(message)) {
                                error = new ErrorVM
                                {
                                    index = 0,
                                    field = "",
                                    description = message,
                                    ErrorStatusCode = HttpStatusCode.BadRequest
                                };
                                return null;
                            }
                        }
                    }
                    sedvm.ObjID = sd.obj_id;
                    sedvm.CompanyCode = sd.company_code;
                    sedvm.BranchCode = sd.branch_code;
                    sedvm.EstNo = sd.est_no;
                    sedvm.SlNo = sd.sl_no;
                    sedvm.BillNo = 0;
                    sedvm.BarcodeNo = sd.barcode_no;
                    sedvm.SalCode = sd.sal_code;
                    sedvm.CounterCode = sd.counter_code;
                    sedvm.ItemName = sd.item_name;
                    sedvm.ItemNo = sd.item_no;
                    sedvm.ItemQty = sd.item_no;
                    sedvm.Grosswt = sd.gwt;
                    sedvm.Stonewt = sd.swt;
                    sedvm.Netwt = sd.nwt;
                    sedvm.AddWt = sd.AddWt;
                    sedvm.DeductWt = sd.DeductWt;
                    sedvm.MakingChargePerRs = sd.making_charge_per_rs;
                    sedvm.WastPercent = sd.wast_percent;
                    sedvm.GoldValue = sd.gold_value;
                    sedvm.VaAmount = sd.va_amount;
                    sedvm.StoneCharges = sd.stone_charges;
                    sedvm.DiamondCharges = sd.diamond_charges;
                    sedvm.TotalAmount = sd.total_amount;
                    sedvm.Hallmarkarges = sd.hallmarcharges;
                    sedvm.McAmount = sd.mc_amount;
                    sedvm.WastageGrms = sd.wastage_grms;
                    sedvm.McPercent = sd.mc_percent;
                    sedvm.AddQty = sd.Addqty;
                    sedvm.DeductQty = sd.Deductqty;
                    sedvm.OfferValue = sd.offer_value;
                    sedvm.UpdateOn = sd.UpdateOn;
                    sedvm.GsCode = sd.gs_code;
                    sedvm.Rate = sd.rate;
                    sedvm.Karat = sd.karat;
                    sedvm.AdBarcode = sd.ad_barcode;
                    sedvm.AdCounter = sd.ad_counter;
                    sedvm.AdItem = sd.ad_item;
                    sedvm.IsEDApplicable = sd.isEDApplicable;
                    sedvm.McType = sd.mc_type;
                    sedvm.Fin_Year = sd.Fin_Year;
                    sedvm.NewBillNo = sd.New_Bill_No;
                    sedvm.ItemTotalAfterDiscount = sd.item_total_after_discount;
                    sedvm.ItemAdditionalDiscount = sd.item_additional_discount;
                    sedvm.TaxPercentage = sd.tax_percentage;
                    sedvm.TaxAmount = sd.tax_amount;
                    sedvm.ItemFinalAmount = sd.item_final_amount;
                    sedvm.SupplierCode = sd.supplier_code;
                    sedvm.ItemSize = sd.item_size;
                    sedvm.ImgID = sd.img_id;
                    sedvm.DesignCode = sd.design_code;
                    sedvm.DesignName = sd.design_name;
                    sedvm.BatchID = sd.batch_id;
                    sedvm.Rf_ID = sd.rf_id;
                    sedvm.McPerPiece = sd.mc_per_piece;
                    sedvm.DiscountMc = sd.Discount_Mc;
                    sedvm.TotalSalesMc = sd.Total_sales_mc;
                    sedvm.McDiscountAmt = sd.Mc_Discount_Amt;
                    sedvm.purchaseMc = sd.purchase_mc;
                    sedvm.GSTGroupCode = sd.GSTGroupCode;
                    sedvm.SGSTPercent = sd.SGST_Percent;
                    sedvm.SGSTAmount = sd.SGST_Amount;
                    sedvm.CGSTPercent = sd.SGST_Percent;
                    sedvm.CGSTAmount = sd.CGST_Amount;
                    sedvm.IGSTPercent = sd.IGST_Percent;
                    sedvm.IGSTAmount = sd.IGST_Amount;
                    sedvm.HSN = sd.HSN;
                    sedvm.PieceRate = sd.Piece_Rate;
                    sedvm.DeductSWt = sd.DeductSWt;
                    sedvm.OrdDiscountAmt = sd.Ord_Discount_Amt;
                    sedvm.DedCounter = sd.ded_counter;
                    sedvm.DedItem = sd.ded_item;
                    sedvm.VaporLossWeight = Convert.ToDecimal(sd.vaporLoss);
                    sedvm.VaporLossAmount = Convert.ToDecimal(sd.vaporAmount);
                    //sedvm.Huid = sd.HUID;

                    totalBillAmount = totalBillAmount + sd.total_amount;
                    totalOfferValue = totalOfferValue + Convert.ToDecimal(sd.offer_value);

                    foreach (KTTU_SALES_STONE_DETAILS std in kssd) {
                        SalesEstStoneVM ses = new SalesEstStoneVM();
                        ses.ObjID = std.obj_id;
                        ses.CompanyCode = std.company_code;
                        ses.BranchCode = std.branch_code;
                        ses.BillNo = std.bill_no;
                        ses.SlNo = std.sl_no;
                        ses.EstNo = std.est_no;
                        ses.EstSrNo = std.est_Srno;
                        ses.BarcodeNo = std.barcode_no;
                        ses.Type = std.type;
                        ses.Name = std.name;
                        ses.Qty = std.qty;
                        ses.Carrat = std.carrat;
                        ses.StoneWt = std.stone_wt;
                        ses.Rate = std.rate;
                        ses.Amount = std.amount;
                        ses.Tax = std.tax;
                        ses.TaxAmount = std.tax_amount;
                        ses.TotalAmount = std.total_amount;
                        ses.BillType = std.bill_type;
                        ses.DealerSalesNo = std.dealer_sales_no;
                        ses.BillDet11PK = std.BILL_DET11PK;
                        ses.UpdateOn = std.UpdateOn;
                        ses.FinYear = std.Fin_Year;
                        ses.Color = std.color;
                        ses.Clarity = std.clarity;
                        ses.Shape = std.shape;
                        ses.Cut = std.cut;
                        ses.Polish = std.polish;
                        ses.Symmetry = std.symmetry;
                        ses.Fluorescence = std.fluorescence;
                        ses.Certificate = std.certificate;
                        salesEstStone.Add(ses);
                    }
                    sedvm.salesEstStoneVM = salesEstStone;
                    lstOfSalesEstDet.Add(sedvm);
                }
                salesEst.salesEstimatonVM = lstOfSalesEstDet;

                if (totalOfferValue > 0) {
                    decimal discountPercent = Math.Round((totalOfferValue / totalBillAmount) * 100, 2);
                    salesEst.offerDiscount = new SalesInvoiceAttributeVM() { DiscountAmount = totalOfferValue, DiscountPercent = discountPercent };
                }

                //Get FOD
                if (ksem.offerType == "FOD") {
                    var payments = new List<PaymentVM>();
                    PaymentVM pymt = new PaymentVM { PayMode = "FOD", AmtReceived = Convert.ToDecimal(ksem.TotalDiscountVoucherAmt) };
                    payments.Add(pymt);
                    salesEst.paymentVM = payments;
                }

                return salesEst;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public SalesEstMasterVM GetEstimationDetailsForPrint(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            try {
                SalesEstMasterVM salesEst = new SalesEstMasterVM();
                List<SalesEstDetailsVM> lstOfSalesEstDet = new List<SalesEstDetailsVM>();

                KTTU_SALES_EST_MASTER ksem = new KTTU_SALES_EST_MASTER();
                List<KTTU_SALES_EST_DETAILS> ksed = new List<KTTU_SALES_EST_DETAILS>();
                List<KTTU_SALES_STONE_DETAILS> kssd = new List<KTTU_SALES_STONE_DETAILS>();

                // Checking valid Estimation Number or Not
                ksem = db.KTTU_SALES_EST_MASTER.Where(est => est.est_no == estNo
                                                        && est.company_code == companyCode
                                                        && est.branch_code == branchCode).FirstOrDefault();
                if (ksem == null) {
                    error = new ErrorVM
                    {
                        index = 0,
                        field = "",
                        description = "Invalid estimation number.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }
                salesEst.ObjID = ksem.obj_id;
                salesEst.CompanyCode = ksem.company_code;
                salesEst.BranchCode = ksem.branch_code;
                salesEst.EstNo = ksem.est_no;
                salesEst.OrderNo = ksem.order_no;
                salesEst.CustID = ksem.Cust_Id;
                salesEst.CustName = ksem.Cust_Name;
                salesEst.OrderDate = ksem.order_date;
                salesEst.EstDate = ksem.est_date;
                salesEst.OperatorCode = ksem.operator_code;
                salesEst.Karat = ksem.karat;
                salesEst.Rate = ksem.rate;
                salesEst.Tax = ksem.tax;
                salesEst.TotalTaxAmount = ksem.total_tax_amount;
                salesEst.TotalEstAmount = ksem.total_est_amount;
                salesEst.GrandTotal = ksem.grand_total;
                salesEst.OrderAmount = ksem.order_amount;
                salesEst.PurchaseAmount = ksem.purchase_amount;
                salesEst.SPurchaseAmount = ksem.spurchase_amount;
                salesEst.SType = ksem.s_type;
                salesEst.BillNo = ksem.bill_no;
                salesEst.GSType = ksem.gstype;
                salesEst.DiscountAmount = ksem.discount_amount;
                salesEst.ApprovedBy = ksem.approved_by;
                salesEst.IsIns = ksem.is_ins;
                salesEst.SrBillNo = ksem.sr_billno;
                salesEst.ItemSet = ksem.item_set;
                salesEst.Remarks = ksem.remarks;
                salesEst.ApproveFlag = ksem.approve_flag;
                salesEst.UpdateOn = ksem.UpdateOn;
                salesEst.EdCess = ksem.ed_cess;
                salesEst.HedCEss = ksem.hed_cess;
                salesEst.EdCessPercent = ksem.ed_cess_percent;
                salesEst.HedCessPercent = ksem.hed_cess_percent;
                salesEst.ExciseDutyPercent = ksem.excise_duty_percent;
                salesEst.ExciseDutyAmount = ksem.excise_duty_amount;
                salesEst.HiSchemeAmount = ksem.hi_scheme_amount;
                salesEst.HiSchemeNumber = ksem.hi_scheme_no;
                salesEst.HiBonusAmount = ksem.hi_bonus_amount;
                salesEst.Salutation = ksem.salutation;
                salesEst.NewBillNo = ksem.New_Bill_No;
                salesEst.IsPAN = ksem.isPAN;
                salesEst.RoundOff = ksem.round_off;
                salesEst.StateCode = ksem.state_code;
                salesEst.Pos = ksem.pos;
                salesEst.CorparateID = ksem.Corparate_ID;
                salesEst.CorporateBranchID = ksem.Corporate_Branch_ID;
                salesEst.EmployeeId = ksem.Employee_Id;
                salesEst.RegisteredMN = ksem.Registered_MN;
                salesEst.ProfessionID = ksem.profession_ID;
                salesEst.EmpcorpEmailID = ksem.Empcorp_Email_ID;
                salesEst.PhoneNo = ksem.phone_no;
                salesEst.EmailID = ksem.Email_ID;
                salesEst.IdType = ksem.id_type;
                salesEst.IdDetails = ksem.id_details;
                salesEst.PANNo = ksem.pan_no;
                salesEst.TIN = ksem.tin;
                salesEst.Pincode = ksem.pin_code;
                salesEst.MobileNo = ksem.mobile_no;
                salesEst.City = ksem.city;
                salesEst.State = ksem.state;
                salesEst.Address2 = ksem.address2;
                salesEst.Address3 = ksem.address3;
                salesEst.Address1 = ksem.address1;

                ksed = db.KTTU_SALES_EST_DETAILS.Where(est => est.est_no == estNo
                                                        && est.company_code == companyCode
                                                        && est.branch_code == branchCode).ToList();

                foreach (KTTU_SALES_EST_DETAILS sd in ksed) {
                    SalesEstDetailsVM sedvm = new SalesEstDetailsVM();
                    List<SalesEstStoneVM> salesEstStone = new List<SalesEstStoneVM>();
                    if (sd.barcode_no != "") {
                        kssd = db.KTTU_SALES_STONE_DETAILS.Where(est => est.est_no == estNo
                                                                    && est.barcode_no == sd.barcode_no
                                                                    && est.est_Srno == sd.sl_no
                                                                    && est.company_code == companyCode
                                                                    && est.branch_code == branchCode).ToList();
                    }
                    else {
                        kssd = db.KTTU_SALES_STONE_DETAILS.Where(est => est.est_no == estNo
                                                                    && est.est_Srno == sd.sl_no
                                                                    && est.company_code == companyCode
                                                                    && est.branch_code == branchCode).ToList();
                    }
                    sedvm.ObjID = sd.obj_id;
                    sedvm.CompanyCode = sd.company_code;
                    sedvm.BranchCode = sd.branch_code;
                    sedvm.EstNo = sd.est_no;
                    sedvm.SlNo = sd.sl_no;
                    sedvm.BillNo = sd.bill_no;
                    sedvm.BarcodeNo = sd.barcode_no;

                    KSTU_SALESMAN_MASTER salesman = db.KSTU_SALESMAN_MASTER.Where(sal => sal.sal_code == sd.sal_code
                                                                    && sal.company_code == companyCode
                                                                    && sal.branch_code == branchCode).FirstOrDefault();
                    sedvm.SalCode = salesman == null ? "" : salesman.sal_name;

                    sedvm.CounterCode = sd.counter_code;

                    ITEM_MASTER itemMaster = db.ITEM_MASTER.Where(item => item.gs_code == sd.gs_code
                                                                     && item.Item_code == sd.item_name
                                                                     && item.company_code == companyCode
                                                                     && item.branch_code == branchCode).FirstOrDefault();
                    sedvm.ItemName = itemMaster == null ? "" : itemMaster.Item_Name;
                    sedvm.ItemNo = sd.item_no;
                    sedvm.ItemQty = sd.item_no;
                    sedvm.Grosswt = sd.gwt;
                    sedvm.Stonewt = sd.swt;
                    sedvm.Netwt = sd.nwt;
                    sedvm.AddWt = sd.AddWt;
                    sedvm.DeductWt = sd.DeductWt;
                    sedvm.MakingChargePerRs = sd.making_charge_per_rs;
                    sedvm.WastPercent = sd.wast_percent;
                    sedvm.GoldValue = sd.gold_value;
                    sedvm.VaAmount = sd.va_amount + Convert.ToDecimal(sd.Mc_Discount_Amt);
                    sedvm.StoneCharges = sd.stone_charges;
                    sedvm.DiamondCharges = sd.diamond_charges;
                    sedvm.TotalAmount = sd.total_amount;
                    sedvm.Hallmarkarges = sd.hallmarcharges;
                    sedvm.McAmount = sd.mc_amount;
                    sedvm.WastageGrms = sd.wastage_grms;
                    sedvm.McPercent = sd.mc_percent;
                    sedvm.AddQty = sd.Addqty;
                    sedvm.DeductQty = sd.Deductqty;
                    sedvm.OfferValue = sd.offer_value;
                    sedvm.UpdateOn = sd.UpdateOn;
                    sedvm.GsCode = sd.gs_code;
                    sedvm.Rate = sd.rate;
                    sedvm.Karat = sd.karat;
                    sedvm.AdBarcode = sd.ad_barcode;
                    sedvm.AdCounter = sd.ad_counter;
                    sedvm.AdItem = sd.ad_item;
                    sedvm.IsEDApplicable = sd.isEDApplicable;
                    sedvm.McType = sd.mc_type;
                    sedvm.Fin_Year = sd.Fin_Year;
                    sedvm.NewBillNo = sd.New_Bill_No;
                    sedvm.ItemTotalAfterDiscount = sd.item_total_after_discount + Convert.ToDecimal(sd.Mc_Discount_Amt); ;
                    sedvm.ItemAdditionalDiscount = sd.item_additional_discount;
                    sedvm.TaxPercentage = sd.tax_percentage;
                    sedvm.TaxAmount = sd.tax_amount;
                    sedvm.ItemFinalAmount = sd.item_final_amount;
                    sedvm.SupplierCode = sd.supplier_code;
                    sedvm.ItemSize = sd.item_size;
                    sedvm.ImgID = sd.img_id;
                    sedvm.DesignCode = sd.design_code;
                    sedvm.DesignName = sd.design_name;
                    sedvm.BatchID = sd.batch_id;
                    sedvm.Rf_ID = sd.rf_id;
                    sedvm.McPerPiece = sd.mc_per_piece;
                    sedvm.DiscountMc = sd.Discount_Mc;
                    sedvm.TotalSalesMc = sd.Total_sales_mc;
                    sedvm.McDiscountAmt = sd.Mc_Discount_Amt;
                    sedvm.purchaseMc = sd.purchase_mc;
                    sedvm.GSTGroupCode = sd.GSTGroupCode;
                    sedvm.SGSTPercent = sd.SGST_Percent;
                    sedvm.SGSTAmount = sd.SGST_Amount;
                    sedvm.CGSTPercent = sd.SGST_Percent;
                    sedvm.CGSTAmount = sd.CGST_Amount;
                    sedvm.IGSTPercent = sd.IGST_Percent;
                    sedvm.IGSTAmount = sd.IGST_Amount;
                    sedvm.HSN = sd.HSN;
                    sedvm.PieceRate = sd.Piece_Rate;
                    sedvm.DeductSWt = sd.DeductSWt;
                    sedvm.OrdDiscountAmt = sd.Ord_Discount_Amt;
                    sedvm.DedCounter = sd.ded_counter;
                    sedvm.DedItem = sd.ded_item;
                    sedvm.VaporLossWeight = Convert.ToDecimal(sd.vaporLoss);
                    sedvm.VaporLossAmount = Convert.ToDecimal(sd.vaporAmount);

                    foreach (KTTU_SALES_STONE_DETAILS std in kssd) {
                        SalesEstStoneVM ses = new SalesEstStoneVM();
                        ses.ObjID = std.obj_id;
                        ses.CompanyCode = std.company_code;
                        ses.BranchCode = std.branch_code;
                        ses.BillNo = std.bill_no;
                        ses.SlNo = std.sl_no;
                        ses.EstNo = std.est_no;
                        ses.EstSrNo = std.est_Srno;
                        ses.BarcodeNo = std.barcode_no;
                        ses.Type = std.type;
                        ses.Name = std.name;
                        ses.Qty = std.qty;
                        ses.Carrat = std.carrat;
                        ses.StoneWt = std.stone_wt;
                        ses.Rate = std.rate;
                        ses.Amount = std.amount;
                        ses.Tax = std.tax;
                        ses.TaxAmount = std.tax_amount;
                        ses.TotalAmount = std.total_amount;
                        ses.BillType = std.bill_type;
                        ses.DealerSalesNo = std.dealer_sales_no;
                        ses.BillDet11PK = std.BILL_DET11PK;
                        ses.UpdateOn = std.UpdateOn;
                        ses.FinYear = std.Fin_Year;
                        ses.Color = std.color;
                        ses.Clarity = std.clarity;
                        ses.Shape = std.shape;
                        ses.Cut = std.cut;
                        ses.Polish = std.polish;
                        ses.Symmetry = std.symmetry;
                        ses.Fluorescence = std.fluorescence;
                        ses.Certificate = std.certificate;
                        salesEstStone.Add(ses);
                    }
                    sedvm.salesEstStoneVM = salesEstStone;
                    lstOfSalesEstDet.Add(sedvm);
                }
                salesEst.salesEstimatonVM = lstOfSalesEstDet;
                return salesEst;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public SalesEstDetailsVM GetEstimationDetailsForPrintTotal(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            try {
                SalesEstMasterVM salesEst = new SalesEstMasterVM();
                List<SalesEstDetailsVM> lstOfSalesEstDet = new List<SalesEstDetailsVM>();
                SalesEstDetailsVM totalSalesDet = new SalesEstDetailsVM();

                KTTU_SALES_EST_MASTER ksem = new KTTU_SALES_EST_MASTER();
                List<KTTU_SALES_EST_DETAILS> ksed = new List<KTTU_SALES_EST_DETAILS>();
                List<KTTU_SALES_STONE_DETAILS> kssd = new List<KTTU_SALES_STONE_DETAILS>();

                ksem = db.KTTU_SALES_EST_MASTER.Where(est => est.est_no == estNo
                                                        && est.company_code == companyCode
                                                        && est.branch_code == branchCode).FirstOrDefault();
                if (ksem == null) {
                    error = new ErrorVM
                    {
                        index = 0,
                        field = "",
                        description = "Invalid estimation number."
                    };
                    return null;
                }
                salesEst.DiscountAmount = ksem.discount_amount;
                ksed = db.KTTU_SALES_EST_DETAILS.Where(est => est.est_no == estNo
                                                        && est.company_code == companyCode
                                                        && est.branch_code == branchCode).ToList();
                decimal taxableValue = 0;
                foreach (KTTU_SALES_EST_DETAILS sd in ksed) {
                    if (sd.barcode_no != "") {
                        kssd = db.KTTU_SALES_STONE_DETAILS.Where(est => est.est_no == estNo
                                                                && est.barcode_no == sd.barcode_no
                                                                && est.est_Srno == sd.sl_no
                                                                && est.company_code == companyCode
                                                                && est.branch_code == branchCode).ToList();
                    }
                    else {
                        kssd = db.KTTU_SALES_STONE_DETAILS.Where(est => est.est_no == estNo
                                                                && est.est_Srno == sd.sl_no
                                                                && est.company_code == companyCode
                                                                && est.branch_code == branchCode).ToList();
                    }
                    totalSalesDet.ItemQty = Convert.ToInt32(totalSalesDet.ItemQty) + sd.item_no;
                    totalSalesDet.Grosswt = totalSalesDet.Grosswt + sd.gwt;
                    totalSalesDet.Stonewt = totalSalesDet.Stonewt + sd.swt;
                    totalSalesDet.Netwt = totalSalesDet.Netwt + sd.nwt;
                    totalSalesDet.AddWt = Convert.ToDecimal(totalSalesDet.AddWt) + sd.AddWt;
                    totalSalesDet.WastPercent = totalSalesDet.WastPercent + sd.wast_percent;
                    totalSalesDet.GoldValue = totalSalesDet.GoldValue + sd.gold_value;

                    //totalSalesDet.VaAmount = totalSalesDet.VaAmount + sd.va_amount;
                    //Above line is commented because Additional Discuount they show separately so display purpose am adding that discount amount to VA Amount.
                    totalSalesDet.VaAmount = Convert.ToDecimal(totalSalesDet.VaAmount) + sd.va_amount + Convert.ToDecimal(sd.Mc_Discount_Amt);

                    totalSalesDet.StoneCharges = totalSalesDet.StoneCharges + sd.stone_charges;
                    totalSalesDet.DiamondCharges = Convert.ToDecimal(totalSalesDet.DiamondCharges) + sd.diamond_charges;
                    totalSalesDet.TotalAmount = totalSalesDet.TotalAmount + sd.total_amount;
                    totalSalesDet.McAmount = Convert.ToDecimal(totalSalesDet.McAmount) + sd.mc_amount;
                    totalSalesDet.WastageGrms = Convert.ToDecimal(totalSalesDet.WastageGrms) + sd.wastage_grms;
                    totalSalesDet.OfferValue = Convert.ToDecimal(totalSalesDet.OfferValue) + sd.offer_value;

                    //totalSalesDet.ItemTotalAfterDiscount = Convert.ToDecimal(totalSalesDet.ItemTotalAfterDiscount) + sd.item_total_after_discount;
                    //Above line is commented because Additional Discuount they show separately so display purpose am addign that discount amount to VA Amount.
                    taxableValue = taxableValue + Convert.ToDecimal(sd.item_total_after_discount);
                    totalSalesDet.ItemTotalAfterDiscount = Convert.ToDecimal(totalSalesDet.ItemTotalAfterDiscount) + sd.item_total_after_discount + sd.Mc_Discount_Amt;

                    totalSalesDet.ItemAdditionalDiscount = Convert.ToDecimal(totalSalesDet.ItemAdditionalDiscount) + sd.item_additional_discount;
                    totalSalesDet.TaxPercentage = Convert.ToDecimal(totalSalesDet.TaxPercentage) + sd.tax_percentage;
                    totalSalesDet.TaxAmount = Convert.ToDecimal(totalSalesDet.TaxAmount) + sd.tax_amount;
                    totalSalesDet.ItemFinalAmount = Convert.ToDecimal(totalSalesDet.ItemFinalAmount) + sd.item_final_amount;
                    totalSalesDet.SGSTPercent = sd.SGST_Percent;
                    totalSalesDet.SGSTAmount = Convert.ToDecimal(totalSalesDet.SGSTAmount) + sd.SGST_Amount;
                    totalSalesDet.CGSTPercent = sd.SGST_Percent;
                    totalSalesDet.CGSTAmount = Convert.ToDecimal(totalSalesDet.CGSTAmount) + sd.CGST_Amount;
                    totalSalesDet.IGSTPercent = sd.IGST_Percent;
                    totalSalesDet.IGSTAmount = Convert.ToDecimal(totalSalesDet.IGSTAmount) + sd.IGST_Amount;
                    totalSalesDet.McDiscountAmt = Convert.ToDecimal(totalSalesDet.McDiscountAmt) + sd.Mc_Discount_Amt;
                }
                //totalSalesDet.RoundingOfAmount = Convert.ToInt32(Math.Round(Convert.ToDecimal(totalSalesDet.ItemFinalAmount), 0, MidpointRounding.ToEven));
                //totalSalesDet.RoundingOfValue = totalSalesDet.RoundingOfAmount - Convert.ToDecimal(totalSalesDet.ItemFinalAmount);

                // Current Sales Amout Details.
                if (totalSalesDet.McDiscountAmt > 0) {
                    string discountDesc = "Discount Amount:";
                    decimal discountAmount = Convert.ToDecimal(totalSalesDet.McDiscountAmt);
                    SalesEstPrintTotals discountSales = new SalesEstPrintTotals() { Description = discountDesc, Amount = discountAmount };
                    totalSalesDet.lstOfSalesEstPrintTotals.Add(discountSales);
                }

                if (salesEst.DiscountAmount > 0) {
                    string discountDesc = "Discount Amount:";
                    decimal discountAmount = Convert.ToDecimal(salesEst.DiscountAmount);
                    SalesEstPrintTotals discountSales = new SalesEstPrintTotals() { Description = discountDesc, Amount = discountAmount };
                    totalSalesDet.lstOfSalesEstPrintTotals.Add(discountSales);
                }

                // Taxable Value
                if (taxableValue > 0) {
                    string taxableValueDesc = "Taxable Value:";
                    decimal taxableAmount = taxableValue;
                    SalesEstPrintTotals taxableSales = new SalesEstPrintTotals() { Description = taxableValueDesc, Amount = taxableAmount };
                    totalSalesDet.lstOfSalesEstPrintTotals.Add(taxableSales);
                }

                //Tax Calculation
                string sgst = "SGST " + totalSalesDet.SGSTPercent + "% :";
                decimal sgstAmount = Convert.ToDecimal(totalSalesDet.SGSTAmount);
                SalesEstPrintTotals sgstSales = new SalesEstPrintTotals() { Description = sgst, Amount = sgstAmount };
                totalSalesDet.lstOfSalesEstPrintTotals.Add(sgstSales);

                string cgst = "CGST " + totalSalesDet.SGSTPercent + "% :";
                decimal cgstAmount = Convert.ToDecimal(totalSalesDet.CGSTAmount);
                SalesEstPrintTotals cgstSales = new SalesEstPrintTotals() { Description = cgst, Amount = cgstAmount };
                totalSalesDet.lstOfSalesEstPrintTotals.Add(cgstSales);

                string igst = "IGST " + totalSalesDet.SGSTPercent + "% :";
                decimal igstAmount = Convert.ToDecimal(totalSalesDet.IGSTAmount);
                SalesEstPrintTotals igstSales = new SalesEstPrintTotals() { Description = igst, Amount = igstAmount };
                totalSalesDet.lstOfSalesEstPrintTotals.Add(igstSales);


                // Current Sales Amout Details.
                string currentSalesDesc = "Sales Total:";
                decimal currentSalesTotal = Convert.ToDecimal(totalSalesDet.ItemFinalAmount);
                SalesEstPrintTotals currentSales = new SalesEstPrintTotals() { Description = currentSalesDesc, Amount = currentSalesTotal };
                totalSalesDet.lstOfSalesEstPrintTotals.Add(currentSales);

                // Inherent Purchase Details
                decimal purchaseAmount = 0;
                List<KTTU_PURCHASE_EST_DETAILS> kped = db.KTTU_PURCHASE_EST_DETAILS.Where(est => est.est_no == estNo
                                                                             && est.company_code == companyCode
                                                                             && est.branch_code == branchCode).ToList();
                if (kped != null && kped.Count > 0) {
                    purchaseAmount = db.KTTU_PURCHASE_EST_DETAILS.Where(est => est.est_no == estNo
                                                                                && est.company_code == companyCode
                                                                                && est.branch_code == branchCode).Sum(et => et.item_amount);
                    string purchaseAmountDesc = "Less Purchase (As Per Est No: " + estNo + ")";
                    SalesEstPrintTotals inherentPurchase = new SalesEstPrintTotals() { Description = purchaseAmountDesc, Amount = purchaseAmount };
                    totalSalesDet.lstOfSalesEstPrintTotals.Add(inherentPurchase);
                }

                // Attached Purchase Estimation Deails
                decimal totalAttachedPurchaseAmount = 0;
                List<KTTU_PAYMENT_DETAILS> lstOfAttachedEst = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == estNo
                                                               && pay.company_code == companyCode
                                                               && pay.branch_code == branchCode
                                                               && pay.pay_mode == "PE"
                                                               && pay.cflag == "N").ToList();
                if (lstOfAttachedEst != null && lstOfAttachedEst.Count > 0) {
                    foreach (KTTU_PAYMENT_DETAILS kpd in lstOfAttachedEst) {
                        string attachedEstNo = Convert.ToString(kpd.Ref_BillNo);
                        string atachedPurchaseAmountDesc = "Less Purchase (As Per Est No: " + attachedEstNo + ")";
                        decimal attachedPurchaseAmount = Convert.ToDecimal(kpd.pay_amt);
                        SalesEstPrintTotals attachedPurchase = new SalesEstPrintTotals() { Description = atachedPurchaseAmountDesc, Amount = attachedPurchaseAmount };
                        totalSalesDet.lstOfSalesEstPrintTotals.Add(attachedPurchase);
                        totalAttachedPurchaseAmount = totalAttachedPurchaseAmount + attachedPurchaseAmount;
                    }
                }

                // Attached Sales Return Estimation Details.
                decimal totalAttachedSRAmount = 0;
                List<KTTU_PAYMENT_DETAILS> payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == estNo
                                                                                    && pay.pay_mode == "SE" && pay.cflag == "N"
                                                                                    && pay.company_code == companyCode
                                                                                    && pay.branch_code == branchCode).ToList();
                if (payment != null && payment.Count > 0) {
                    foreach (KTTU_PAYMENT_DETAILS kpd in payment) {
                        string srEstNo = Convert.ToString(kpd.Ref_BillNo);
                        string atachedSRAmountDesc = "Less Sales Return (As Per Est No: " + srEstNo + ")";
                        decimal attachedSRAmount = Convert.ToDecimal(kpd.pay_amt);
                        SalesEstPrintTotals attachedPurchase = new SalesEstPrintTotals() { Description = atachedSRAmountDesc, Amount = attachedSRAmount };
                        totalSalesDet.lstOfSalesEstPrintTotals.Add(attachedPurchase);
                        totalAttachedSRAmount = totalAttachedSRAmount + attachedSRAmount;
                    }
                }

                // Attached Order Details
                decimal totalAttachedOrderAmount = 0;
                List<KTTU_PAYMENT_DETAILS> orderPayment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == estNo
                                                                                   && pay.pay_mode == "OP"
                                                                                   && pay.cflag == "N"
                                                                                   && pay.company_code == companyCode
                                                                                   && pay.branch_code == branchCode).ToList();
                if (orderPayment != null && orderPayment.Count > 0) {
                    foreach (KTTU_PAYMENT_DETAILS kpd in orderPayment) {
                        string orderNo = Convert.ToString(kpd.Ref_BillNo);
                        decimal attachedOrderAmount = attachedOrderAmount = Convert.ToDecimal(kpd.pay_amt);
                        string attachedOrderDesc = "Less Advance (As Per Order No: " + kpd.Ref_BillNo + ")";
                        SalesEstPrintTotals attachedPurchase = new SalesEstPrintTotals() { Description = attachedOrderDesc, Amount = attachedOrderAmount };
                        totalSalesDet.lstOfSalesEstPrintTotals.Add(attachedPurchase);
                        totalAttachedOrderAmount = totalAttachedOrderAmount + attachedOrderAmount;
                    }
                }

                // Attached Order Information from Sales Estimation Screen (On Top)
                decimal separateOrderAmount = 0;
                KTTU_SALES_EST_MASTER separateOrderFromScreen = db.KTTU_SALES_EST_MASTER.Where(est => est.est_no == estNo
                                                                        && est.company_code == companyCode
                                                                        && est.branch_code == branchCode).FirstOrDefault();
                if (separateOrderFromScreen != null && separateOrderFromScreen.order_no != 0) {
                    string separateOrderDesc = "Less Advance (As Per Order No: " + separateOrderFromScreen.order_no + " Dated: " + separateOrderFromScreen.order_date.ToString("dd/MM/yyyy") + ")";
                    separateOrderAmount = separateOrderFromScreen.order_amount;
                    SalesEstPrintTotals separateOrder = new SalesEstPrintTotals() { Description = separateOrderDesc, Amount = separateOrderAmount };
                    totalSalesDet.lstOfSalesEstPrintTotals.Add(separateOrder);
                }

                decimal totalAmountWithoutRoundingOff = currentSalesTotal - (purchaseAmount + totalAttachedPurchaseAmount + totalAttachedSRAmount + totalAttachedOrderAmount + separateOrderAmount);
                int totalAmountWithRoudingOff = Convert.ToInt32(Math.Round(totalAmountWithoutRoundingOff, 0, MidpointRounding.ToEven));
                //decimal roundingOfPaisa = totalAmountWithoutRoundingOff - totalAmountWithRoudingOff;
                decimal roundingOfPaisa = totalAmountWithRoudingOff - totalAmountWithoutRoundingOff;

                string roudingOfDescription = totalAmountWithRoudingOff < 0 ? "Amount Payable" : " Amount Receivable";
                SalesEstPrintTotals roundingOffValues = new SalesEstPrintTotals()
                {
                    Description = roudingOfDescription + " (Round.Off " + roundingOfPaisa + " Ps.)",
                    Amount = totalAmountWithRoudingOff
                };
                totalSalesDet.lstOfSalesEstPrintTotals.Add(roundingOffValues);

                salesEst.salesEstimatonVM = lstOfSalesEstDet;
                return totalSalesDet;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }        

        public int SaveEstimation(SalesEstMasterVM saleEst, out ErrorVM error)
        {
            error = null;
            decimal totalTaxAmount = 0;
            decimal totalEstAmount = 0;
            decimal grandTotal = 0;
            decimal totalAmount = 0;
            decimal totalMetalWeight = 0;
            decimal totalStoneWeight = 0;
            string gsCode = string.Empty;

            // Bellow is working code written for validation of Estimation details
            //ErrorVM valError = new ErrorVM();
            //bool isTrue = DoEstimationValidation(saleEst, out valError);
            //if (!isTrue && valError != null) {
            //    error = new ErrorVM()
            //    {
            //        description = valError.description,
            //        ErrorStatusCode = HttpStatusCode.BadRequest
            //    };
            //    return 0;
            //}

            KTTU_SALES_EST_MASTER saleEstMaster = new KTTU_SALES_EST_MASTER();
            List<SalesEstDetailsVM> salEstVm = saleEst.salesEstimatonVM;
            List<PaymentVM> payVm = saleEst.paymentVM;
            KSTU_CUSTOMER_MASTER customer = new KSTU_CUSTOMER_MASTER();

            decimal rate = salEstVm.Count > 0 ? Convert.ToDecimal(salEstVm[0].Rate) : 0;
            if (rate == 0) {
                KSTU_RATE_MASTER rateMaster = db.KSTU_RATE_MASTER.Where(r => r.bill_type == "S"
                                         && r.gs_code == saleEst.GSType
                                         && r.company_code == saleEst.CompanyCode
                                         && r.branch_code == saleEst.BranchCode).FirstOrDefault();
                rate = rateMaster == null ? 0 : rateMaster.rate;
            }

            KTTU_PURCHASE_EST_MASTER purchase = db.KTTU_PURCHASE_EST_MASTER.Where(p => p.company_code == saleEst.CompanyCode
                                                                            && p.branch_code == saleEst.BranchCode
                                                                            && p.est_no == saleEst.EstNo).FirstOrDefault();
            DateTime appDate = SIGlobals.Globals.GetApplicationDate(saleEst.CompanyCode, saleEst.BranchCode);
            db.Database.CommandTimeout = 5;
            using (var transaction = db.Database.BeginTransaction()) {
                try {
                    if (saleEst.CustID != 0) {
                        customer = new CustomerBL().GetActualCustomerDetails(saleEst.CustID, saleEst.MobileNo, saleEst.CompanyCode, saleEst.BranchCode);
                    }

                    #region Calculation
                    foreach (SalesEstDetailsVM sede in salEstVm) {
                        totalTaxAmount = totalTaxAmount + Convert.ToDecimal(sede.SGSTAmount)
                                                                          + Convert.ToDecimal(sede.CGSTAmount)
                                                                          + Convert.ToDecimal(sede.IGSTAmount);
                        totalEstAmount = Convert.ToDecimal(totalEstAmount + sede.ItemFinalAmount);
                        grandTotal = Convert.ToDecimal(grandTotal + sede.ItemFinalAmount);

                        // Sale Amount without GST for Coin offer Calculation
                        totalAmount = Convert.ToDecimal(totalAmount + sede.TotalAmount);
                        totalMetalWeight = Convert.ToDecimal(totalMetalWeight + sede.Netwt);
                        totalStoneWeight = Convert.ToDecimal(totalStoneWeight + sede.Stonewt);
                        gsCode = sede.GsCode;
                    }
                    #endregion

                    #region Offer Discount (Coin)
                    if (saleEst.IsMergeEst == false && saleEst.IsOfferApplied == false && saleEst.IsOfferSkipped == false) {
                        decimal totalCoinOffer = CoinOfferCalculation(saleEst.CompanyCode, saleEst.BranchCode, totalAmount, totalMetalWeight, totalStoneWeight, gsCode);
                        if (totalCoinOffer > 0) {
                            error = new ErrorVM() { ErrorStatusCode = HttpStatusCode.Forbidden, Value = totalCoinOffer };
                            return 0;
                        }
                    }
                    else {
                        // Todo Incase Coin offer Applied 
                        foreach (SalesEstDetailsVM sede in salEstVm) {
                            decimal coinGSTAmt = 0;
                            if (sede.IsEDApplicable == "O") {
                                coinGSTAmt = coinGSTAmt + Convert.ToDecimal(sede.SGSTAmount)
                                                  + Convert.ToDecimal(sede.CGSTAmount)
                                                  + Convert.ToDecimal(sede.IGSTAmount);
                            }
                        }
                    }
                    #endregion

                    #region Save Sales Estimation Master
                    saleEstMaster.gstype = saleEst.GSType;
                    saleEstMaster.operator_code = saleEst.OperatorCode;
                    int estNumber = db.KSTS_SEQ_NOS.Where(sq => sq.obj_id == MODULE_SEQ_NO
                                                            && sq.company_code == saleEst.CompanyCode
                                                            && sq.branch_code == saleEst.BranchCode).FirstOrDefault().nextno;
                    saleEstMaster.obj_id = SIGlobals.Globals.GetMagnaGUID(TABLE_NAME, estNumber, saleEst.CompanyCode, saleEst.BranchCode);
                    saleEstMaster.company_code = saleEst.CompanyCode;
                    saleEstMaster.branch_code = saleEst.BranchCode;
                    saleEstMaster.est_no = estNumber;
                    saleEstMaster.order_no = saleEst.OrderNo;
                    saleEstMaster.order_date = appDate;
                    saleEstMaster.est_date = appDate;
                    saleEstMaster.karat = salEstVm[0].Karat;
                    saleEstMaster.rate = rate;
                    saleEstMaster.tax = 0;
                    //-------------Important Columns------------------------
                    saleEstMaster.total_tax_amount = totalTaxAmount;
                    saleEstMaster.total_est_amount = totalEstAmount;
                    saleEstMaster.grand_total = grandTotal;

                    // Saving order Amount
                    //decimal orderAmount = 0;
                    KTTU_ORDER_MASTER order = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == saleEst.OrderNo
                                                                                     && ord.company_code == saleEst.CompanyCode
                                                                                     && ord.branch_code == saleEst.BranchCode).FirstOrDefault();
                    saleEstMaster.order_amount = order == null ? 0 : Convert.ToDecimal(order.advance_ord_amount);  //saleEst.OrderAmount;
                    saleEstMaster.purchase_amount = purchase == null ? saleEst.PurchaseAmount : Convert.ToDecimal(purchase.total_purchase_amount);
                    saleEstMaster.spurchase_amount = saleEst.SPurchaseAmount;
                    //-------------------------------------------------------
                    saleEstMaster.s_type = "S";
                    saleEstMaster.bill_no = 0;
                    // Bellow are the Null Columns
                    saleEstMaster.discount_amount = saleEst.DiscountAmount;
                    saleEstMaster.approved_by = null;
                    saleEstMaster.is_ins = "N";
                    saleEstMaster.sr_billno = 0;
                    saleEstMaster.item_set = "N";
                    saleEstMaster.remarks = null;
                    saleEstMaster.approve_flag = "Y";
                    saleEstMaster.UpdateOn = SIGlobals.Globals.GetDateTime();
                    saleEstMaster.ed_cess = 0;
                    saleEstMaster.hed_cess = 0;
                    saleEstMaster.ed_cess_percent = 0;
                    saleEstMaster.hed_cess_percent = 0;
                    saleEstMaster.excise_duty_percent = 0;
                    saleEstMaster.excise_duty_amount = 0;
                    saleEstMaster.hi_scheme_amount = 0;
                    saleEstMaster.hi_scheme_no = 0;
                    saleEstMaster.hi_bonus_amount = 0;
                    saleEstMaster.New_Bill_No = null;
                    saleEstMaster.round_off = saleEst.RoundOff;
                    saleEstMaster.state_code = customer.state_code;
                    saleEstMaster.pos = saleEst.Pos;
                    saleEstMaster.Corparate_ID = null;
                    saleEstMaster.Corporate_Branch_ID = null;
                    saleEstMaster.Employee_Id = null;
                    saleEstMaster.Registered_MN = null;
                    saleEstMaster.profession_ID = "";
                    saleEstMaster.Empcorp_Email_ID = null;
                    saleEstMaster.phone_no = "";
                    saleEstMaster.Email_ID = "";
                    saleEstMaster.id_type = "";
                    saleEstMaster.id_details = "";
                    saleEstMaster.TotalDiscountVoucherAmt = saleEst.OfferDiscountAmount;
                    saleEstMaster.offerType = string.IsNullOrEmpty(saleEst.OfferCode) ? string.Empty : saleEst.OfferCode;

                    if (customer != null) {
                        saleEstMaster.Cust_Id = customer.cust_id;
                        saleEstMaster.Cust_Name = customer.cust_name == null ? "" : customer.cust_name;
                        saleEstMaster.salutation = customer.salutation;
                        saleEstMaster.isPAN = customer.pan_no == null ? "N" : "Y";
                        saleEstMaster.tin = customer.tin == null ? "" : customer.tin;
                        saleEstMaster.pin_code = customer.pin_code;
                        saleEstMaster.mobile_no = customer.mobile_no;
                        saleEstMaster.city = customer.city;
                        saleEstMaster.state = customer.state;
                        saleEstMaster.address2 = customer.address2;
                        saleEstMaster.address3 = customer.address3;
                        saleEstMaster.address1 = customer.address1;
                        saleEstMaster.id_type = customer.id_type;
                        saleEstMaster.id_details = customer.id_details;
                        saleEstMaster.state = customer.state;
                        saleEstMaster.phone_no = customer.phone_no;
                        saleEstMaster.Email_ID = customer.Email_ID;
                    }
                    db.KTTU_SALES_EST_MASTER.Add(saleEstMaster);
                    #endregion

                    #region Saving Saless Estimation Details
                    int SlNo = 1;
                    var totalLineAmt = salEstVm.Sum(x => x.ItemTotalAfterDiscount);
                    decimal? offerDiscPercent = 0;
                    if(totalLineAmt > 0)
                        offerDiscPercent = saleEstMaster.TotalDiscountVoucherAmt / totalLineAmt;
                    foreach (SalesEstDetailsVM sed in salEstVm) {
                        KTTU_SALES_EST_DETAILS salEstDet = new KTTU_SALES_EST_DETAILS();
                        salEstDet.obj_id = SIGlobals.Globals.GetMagnaGUID(TABLE_NAME, estNumber, saleEst.CompanyCode, saleEst.BranchCode);
                        salEstDet.company_code = saleEst.CompanyCode;
                        salEstDet.branch_code = saleEst.BranchCode;
                        salEstDet.est_no = saleEstMaster.est_no;
                        salEstDet.sl_no = SlNo;
                        salEstDet.bill_no = 0;
                        salEstDet.barcode_no = sed.BarcodeNo == null ? "" : sed.BarcodeNo;
                        salEstDet.sal_code = sed.SalCode;
                        salEstDet.counter_code = sed.CounterCode;
                        salEstDet.item_name = sed.ItemName;
                        salEstDet.item_no = Convert.ToInt32(sed.ItemQty);
                        salEstDet.gwt = sed.Grosswt;
                        salEstDet.swt = sed.Stonewt;
                        salEstDet.nwt = sed.Netwt;
                        salEstDet.AddWt = sed.AddWt == null ? 0 : sed.AddWt;
                        salEstDet.DeductWt = sed.DeductWt == null ? 0 : sed.DeductWt;
                        salEstDet.making_charge_per_rs = sed.MakingChargePerRs;
                        salEstDet.wast_percent = sed.WastPercent;
                        salEstDet.gold_value = sed.GoldValue;
                        salEstDet.va_amount = sed.VaAmount;
                        salEstDet.stone_charges = sed.StoneCharges;
                        salEstDet.diamond_charges = sed.DiamondCharges;
                        salEstDet.total_amount = sed.GoldValue + sed.VaAmount + sed.StoneCharges + Convert.ToDecimal(sed.DiamondCharges);
                        salEstDet.hallmarcharges = sed.Hallmarkarges == null ? 0 : sed.Hallmarkarges;
                        salEstDet.mc_amount = sed.McAmount == null ? 0 : sed.McAmount;
                        salEstDet.wastage_grms = sed.WastageGrms == null ? 0 : sed.WastageGrms;
                        salEstDet.mc_percent = sed.McPercent;
                        salEstDet.Addqty = sed.AddQty == null ? 0 : sed.AddQty;
                        salEstDet.Deductqty = sed.DeductQty == null ? 0 : sed.DeductQty;
                        salEstDet.offer_value = sed.OfferValue == null ? 0 : sed.OfferValue;
                        salEstDet.UpdateOn = SIGlobals.Globals.GetDateTime();
                        salEstDet.gs_code = sed.GsCode;
                        salEstDet.rate = sed.Rate;
                        salEstDet.karat = sed.Karat;
                        salEstDet.ad_barcode = sed.AdBarcode == null ? "" : sed.AdBarcode;
                        salEstDet.ad_counter = sed.AdCounter == null ? "" : sed.AdCounter;
                        salEstDet.ad_item = sed.AdItem == null ? "" : sed.AdItem;
                        salEstDet.isEDApplicable = sed.IsEDApplicable == null ? "" : sed.IsEDApplicable;
                        salEstDet.mc_type = sed.McType;
                        salEstDet.Fin_Year = SIGlobals.Globals.GetFinancialYear(db, saleEst.CompanyCode, saleEst.BranchCode);
                        salEstDet.New_Bill_No = null;
                        salEstDet.item_total_after_discount = sed.ItemTotalAfterDiscount;
                        salEstDet.item_additional_discount = sed.ItemAdditionalDiscount == null ? 0 : sed.ItemAdditionalDiscount;
                        salEstDet.tax_percentage = sed.TaxPercentage == null ? 0 : sed.TaxPercentage;
                        salEstDet.tax_amount = sed.TaxAmount == null ? 0 : sed.TaxAmount;
                        salEstDet.item_final_amount = sed.ItemFinalAmount;
                        salEstDet.supplier_code = sed.SupplierCode == null ? "" : sed.SupplierCode;
                        salEstDet.item_size = sed.ItemSize == null ? "" : sed.ItemSize;
                        salEstDet.img_id = sed.ImgID == null ? "" : sed.ImgID;
                        salEstDet.design_code = sed.DesignCode == null ? "" : sed.DesignCode;
                        salEstDet.design_name = sed.DesignName == null ? "" : sed.DesignName;
                        salEstDet.batch_id = sed.BatchID == null ? "" : sed.BatchID;
                        salEstDet.rf_id = sed.Rf_ID == null ? "" : sed.Rf_ID;
                        salEstDet.mc_per_piece = sed.McPerPiece == null ? 0 : sed.McPerPiece;
                        salEstDet.Discount_Mc = sed.DiscountMc == null ? 0 : sed.DiscountMc;
                        salEstDet.Total_sales_mc = sed.TotalSalesMc;
                        salEstDet.Mc_Discount_Amt = sed.McDiscountAmt;
                        salEstDet.purchase_mc = sed.TotalSalesMc == null ? 0 : sed.TotalSalesMc;
                        salEstDet.GSTGroupCode = sed.GSTGroupCode;
                        salEstDet.SGST_Percent = sed.SGSTPercent == null ? 0 : sed.SGSTPercent;
                        salEstDet.SGST_Amount = sed.SGSTAmount == null ? 0 : sed.SGSTAmount;
                        salEstDet.CGST_Percent = sed.CGSTPercent == null ? 0 : sed.CGSTPercent;
                        salEstDet.CGST_Amount = sed.CGSTAmount == null ? 0 : sed.CGSTAmount;
                        salEstDet.IGST_Percent = sed.IGSTPercent == null ? 0 : sed.IGSTPercent;
                        salEstDet.IGST_Amount = sed.IGSTAmount == null ? 0 : sed.IGSTAmount;
                        salEstDet.CESSPercent = sed.CESSPercent == null ? 0 : sed.CESSPercent;
                        salEstDet.CESSAmount = sed.CESSAmount == null ? 0 : sed.CESSAmount;
                        salEstDet.HSN = Convert.ToString(db.usp_GetGSTHSNCode(sed.CompanyCode, sed.BranchCode, sed.GsCode, sed.ItemName).FirstOrDefault());
                        salEstDet.Piece_Rate = sed.PieceRate;
                        salEstDet.DeductSWt = sed.DeductSWt == null ? 0 : sed.DeductSWt;
                        salEstDet.Ord_Discount_Amt = sed.OrdDiscountAmt == null ? 0 : sed.OrdDiscountAmt;
                        salEstDet.ded_counter = sed.DedCounter == null ? "" : sed.DedCounter;
                        salEstDet.ded_item = sed.DedItem == null ? "" : sed.DedItem;
                        salEstDet.vaporLoss = sed.VaporLossWeight == null ? 0 : sed.VaporLossWeight;
                        salEstDet.vaporAmount = sed.VaporLossAmount == null ? 0 : sed.VaporLossAmount; //shouldn't we recalculate?
                        salEstDet.DiscountVoucherAmt = Convert.ToDecimal(Math.Round(Convert.ToDecimal(offerDiscPercent * sed.ItemTotalAfterDiscount), 2));
                        //salEstDet.HUID = sed.Huid;
                        db.KTTU_SALES_EST_DETAILS.Add(salEstDet);

                        #region Saving Stone Details
                        if (sed.salesEstStoneVM != null) {
                            List<SalesEstStoneVM> salEstStoneDet = sed.salesEstStoneVM;
                            int stoneSlNo = 1;
                            foreach (SalesEstStoneVM ses in salEstStoneDet) {
                                KTTU_SALES_STONE_DETAILS ssd = new KTTU_SALES_STONE_DETAILS();
                                ssd.obj_id = SIGlobals.Globals.GetMagnaGUID(TABLE_NAME, estNumber, saleEst.CompanyCode, saleEst.BranchCode);
                                ssd.company_code = saleEst.CompanyCode;
                                ssd.branch_code = saleEst.BranchCode;
                                ssd.bill_no = ses.BillNo;
                                ssd.sl_no = stoneSlNo;
                                ssd.est_no = salEstDet.est_no;
                                ssd.est_Srno = SlNo;
                                ssd.barcode_no = ses.BarcodeNo == null ? "" : ses.BarcodeNo;
                                ssd.type = ses.Type;
                                ssd.name = ses.Name;
                                ssd.qty = ses.Qty;
                                ssd.carrat = ses.Carrat;
                                ssd.stone_wt = ses.StoneWt;
                                ssd.rate = ses.Rate;
                                ssd.amount = ses.Amount;
                                ssd.tax = ses.Tax;
                                ssd.tax_amount = ses.TaxAmount;
                                ssd.total_amount = ses.TotalAmount;
                                ssd.bill_type = ses.BillType;
                                ssd.dealer_sales_no = ses.DealerSalesNo == null ? 0 : ses.DealerSalesNo;
                                ssd.BILL_DET11PK = ses.BillDet11PK;
                                ssd.UpdateOn = SIGlobals.Globals.GetDateTime();
                                ssd.Fin_Year = SIGlobals.Globals.GetFinancialYear(db, saleEst.CompanyCode, saleEst.BranchCode);
                                ssd.color = ses.Color == null ? "" : ses.Color;
                                ssd.clarity = ses.Clarity == null ? "" : ses.Clarity;
                                ssd.shape = ses.Shape == null ? "" : ses.Shape;
                                ssd.cut = ses.Cut == null ? "" : ses.Cut;
                                ssd.polish = ses.Polish == null ? "" : ses.Polish;
                                ssd.symmetry = ses.Symmetry == null ? "" : ses.Symmetry;
                                ssd.fluorescence = ses.Fluorescence == null ? "" : ses.Fluorescence;
                                ssd.certificate = ses.Certificate == null ? "" : ses.Certificate;
                                ssd.UniqRowID = Guid.NewGuid();
                                db.KTTU_SALES_STONE_DETAILS.Add(ssd);
                                stoneSlNo = stoneSlNo + 1;
                            }
                        }
                        #endregion

                        SlNo = SlNo + 1;
                    }
                    #endregion

                    // Updating Sequence Number
                    SIGlobals.Globals.UpdateSeqenceNumber(db, MODULE_SEQ_NO, saleEst.CompanyCode, saleEst.BranchCode);
                    db.SaveChanges();
                    transaction.Commit();
                    return saleEstMaster.est_no;
                }
                catch (Exception excp) {
                    transaction.Rollback();
                    error = new ErrorVM().GetErrorDetails(excp);
                    return 0;
                }
            }
        }

        private bool CheckIfEstimationLinesAreModified(SalesEstMasterVM saleEst)
        {
            #region Item-wise comparision,let's try this later.
            //var estimateLineObjSummary = (from sd in saleEst.salesEstimatonVM
            //                              group sd by new
            //                              {
            //                                  GS = sd.GsCode,
            //                                  CounterCode = sd.CounterCode,
            //                                  ItemCode = sd.ItemName
            //                              } into g
            //                              select new
            //                              {
            //                                  GS = g.Key.GS,
            //                                  Counter = g.Key.CounterCode,
            //                                  Item = g.Key.ItemCode,
            //                                  Count = g.Count(),
            //                                  Qty = g.Sum(e => e.ItemQty),
            //                                  GrossWt = g.Sum(e => e.Grosswt),
            //                                  StoneWt = g.Sum(e => e.Stonewt),
            //                                  NetWt = g.Sum(e => e.Netwt),
            //                                  TotalAmount = g.Sum(e => e.TotalAmount)
            //                              }).ToList();
            //var estimateLineDbSummary = (from sd in db.KTTU_SALES_EST_DETAILS
            //                             where sd.company_code == saleEst.CompanyCode && sd.branch_code == saleEst.BranchCode
            //                             && sd.est_no == saleEst.EstNo
            //                             group sd by new
            //                             {
            //                                 GS = sd.gs_code,
            //                                 CounterCode = sd.counter_code,
            //                                 ItemCode = sd.item_name
            //                             } into g
            //                             select new
            //                             {
            //                                 GS = g.Key.GS,
            //                                 Counter = g.Key.CounterCode,
            //                                 Item = g.Key.ItemCode,
            //                                 Count = g.Count(),
            //                                 Qty = g.Sum(e => e.item_no),
            //                                 GrossWt = g.Sum(e => e.gwt),
            //                                 StoneWt = g.Sum(e => e.swt),
            //                                 NetWt = g.Sum(e => e.nwt),
            //                                 TotalAmount = g.Sum(e => e.total_amount)
            //                             }).ToList();
            //var x = from lc in estimateLineObjSummary
            //        join db in estimateLineDbSummary
            //        on new { GS = lc.GS, Cntr = lc.Counter, Item = lc.Item }
            //        equals new { GS = db.GS, Cntr = db.Counter, Item = db.Item } into leftOuterJoin
            //        from db in leftOuterJoin.DefaultIfEmpty(); 
            #endregion

            try {
                if (saleEst == null)
                    return false;
                var estimateLineObjSummary = (from sd in saleEst.salesEstimatonVM
                                              group sd by new
                                              {
                                                  EstNo = 1
                                              } into g
                                              select new
                                              {
                                                  Count = g.Count(),
                                                  Qty = g.Sum(e => e.ItemQty),
                                                  GrossWt = g.Sum(e => e.Grosswt),
                                                  StoneWt = g.Sum(e => e.Stonewt),
                                                  NetWt = g.Sum(e => e.Netwt),
                                                  TotalAmount = g.Sum(e => e.TotalAmount)
                                              }).ToList();
                var estimateLineDbSummary = (from sd in db.KTTU_SALES_EST_DETAILS
                                             where sd.company_code == saleEst.CompanyCode && sd.branch_code == saleEst.BranchCode
                                             && sd.est_no == saleEst.EstNo
                                             group sd by new
                                             {
                                                 EstNo = sd.est_no
                                             } into g
                                             select new
                                             {
                                                 Count = g.Count(),
                                                 Qty = g.Sum(e => e.item_no),
                                                 GrossWt = g.Sum(e => e.gwt),
                                                 StoneWt = g.Sum(e => e.swt),
                                                 NetWt = g.Sum(e => e.nwt),
                                                 TotalAmount = g.Sum(e => e.total_amount)
                                             }).ToList();
                if (estimateLineDbSummary == null || estimateLineObjSummary == null)
                    return true;

                var dbLine = estimateLineDbSummary[0];
                var lcLine = estimateLineObjSummary[0];
                if (dbLine.Count != lcLine.Count || dbLine.Qty != lcLine.Qty || dbLine.GrossWt != lcLine.GrossWt
                    || dbLine.StoneWt != lcLine.StoneWt || dbLine.NetWt != lcLine.NetWt || dbLine.TotalAmount != lcLine.TotalAmount) {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception) {
                return false;
            }
        }

        public int UpdateEstimation(int estNo, SalesEstMasterVM saleEst, out ErrorVM error)
        {
            error = null;
            KTTU_SALES_EST_MASTER saleEstMaster = new KTTU_SALES_EST_MASTER();
            List<SalesEstDetailsVM> salEstVm = saleEst.salesEstimatonVM;
            KSTU_CUSTOMER_MASTER customer = new KSTU_CUSTOMER_MASTER();

            decimal totalTaxAmount = 0;
            decimal totalEstAmount = 0;
            decimal grandTotal = 0;
            decimal totalAmount = 0;
            decimal totalMetalWeight = 0;
            decimal totalStoneWeight = 0;
            string gsCode = string.Empty;

            KTTU_SALES_EST_MASTER ksem = db.KTTU_SALES_EST_MASTER.Where(est => est.est_no == estNo
                                                                    && est.company_code == saleEst.CompanyCode
                                                                    && est.branch_code == saleEst.BranchCode).FirstOrDefault();
            if (ksem == null) {
                error = new ErrorVM
                {
                    index = 0,
                    field = "",
                    description = "Invalid Estimation Number: " + ksem.bill_no,
                    ErrorStatusCode = HttpStatusCode.BadRequest
                };
                return 0;
            }            
            if (ksem.bill_no != 0) {
                error = new ErrorVM
                {
                    index = 0,
                    field = "",
                    description = "Sales Est. No: " + ksem.est_no + " already Billed. Bill No: " + ksem.bill_no,
                    ErrorStatusCode = HttpStatusCode.BadRequest
                };
                return 0;
            }

            // Implemented Dirty Read Concept Here.
            if (saleEst.Overwrite == false && saleEst.RowRevisionString != ksem.RowVersionString) {
                error = new ErrorVM
                {
                    index = 0,
                    field = "",
                    description = "Somebody altered the Estimation, Do want to overwrite?",
                    ErrorStatusCode = HttpStatusCode.BadRequest
                };
                return 0;
            }

            bool isEstimationLineChanged = CheckIfEstimationLinesAreModified(saleEst);
            decimal offerDiscountValue = 0;
            if (saleEst != null) {
                offerDiscountValue = Convert.ToDecimal(saleEst.salesEstimatonVM.Sum(x => x.OfferValue));
            }

            if (saleEst.CustID != 0) {
                customer = new CustomerBL().GetActualCustomerDetails(saleEst.CustID, saleEst.MobileNo, saleEst.CompanyCode, saleEst.BranchCode);
            }
            KSTU_RATE_MASTER rateMaster = db.KSTU_RATE_MASTER.Where(r => r.bill_type == "S"
                                                                            && r.gs_code == saleEst.GSType
                                                                            && r.company_code == saleEst.CompanyCode
                                                                            && r.branch_code == saleEst.BranchCode).FirstOrDefault();
            KTTU_PURCHASE_EST_MASTER purchase = db.KTTU_PURCHASE_EST_MASTER.Where(p => p.company_code == saleEst.CompanyCode
                                                                             && p.branch_code == saleEst.BranchCode
                                                                             && p.est_no == saleEst.EstNo).FirstOrDefault();
            DateTime appDate = SIGlobals.Globals.GetApplicationDate(saleEst.CompanyCode, saleEst.BranchCode);
            decimal rate = rateMaster == null ? 0 : rateMaster.rate;
            using (var transaction = db.Database.BeginTransaction()) {
                try {

                    #region Calculation
                    foreach (SalesEstDetailsVM sede in salEstVm) {
                        totalTaxAmount = totalTaxAmount + Convert.ToDecimal(sede.SGSTAmount)
                                                                           + Convert.ToDecimal(sede.CGSTAmount)
                                                                           + Convert.ToDecimal(sede.IGSTAmount);
                        totalEstAmount = Convert.ToDecimal(totalEstAmount + sede.ItemFinalAmount);
                        grandTotal = Convert.ToDecimal(grandTotal + sede.ItemFinalAmount);
                        // Sale Amount without GST for Coin offer Calculation
                        totalAmount = Convert.ToDecimal(totalAmount + sede.TotalAmount);
                        totalMetalWeight = Convert.ToDecimal(totalMetalWeight + sede.Netwt);
                        totalStoneWeight = Convert.ToDecimal(totalStoneWeight + sede.Stonewt);
                        gsCode = sede.GsCode;
                    }
                    #endregion

                    #region Offer Discount (Coin)
                    if (saleEst.IsMergeEst == false && saleEst.IsOfferApplied == false && saleEst.IsOfferSkipped == false) {
                        decimal totalCoinOffer = CoinOfferCalculation(saleEst.CompanyCode, saleEst.BranchCode, totalAmount, totalMetalWeight, totalStoneWeight, gsCode);
                        if (totalCoinOffer > 0) {
                            error = new ErrorVM() { ErrorStatusCode = HttpStatusCode.Forbidden, Value = totalCoinOffer };
                            return 0;
                        }
                    }
                    #endregion

                    #region Delete Estimation Details
                    //Deleting Existing Records
                    DeleteEstimation(saleEst.CompanyCode, saleEst.BranchCode, estNo);
                    #endregion

                    #region Estimation Master
                    // Saving Sales Estimation Master Details
                    saleEstMaster.Cust_Id = saleEst.CustID;
                    saleEstMaster.gstype = saleEst.GSType;
                    saleEstMaster.operator_code = saleEst.OperatorCode;

                    saleEstMaster.obj_id = SIGlobals.Globals.GetMagnaGUID(TABLE_NAME, estNo, saleEst.CompanyCode, saleEst.BranchCode);
                    saleEstMaster.company_code = saleEst.CompanyCode;
                    saleEstMaster.branch_code = saleEst.BranchCode;
                    saleEstMaster.est_no = estNo;
                    saleEstMaster.Cust_Name = customer == null ? "" : customer.cust_name == null ? "" : customer.cust_name; //customer == null ? customer.cust_name == null ? "" : customer.cust_name : "";
                    saleEstMaster.order_no = saleEst.OrderNo;
                    saleEstMaster.order_amount = saleEst.OrderAmount;
                    saleEstMaster.order_date = appDate;
                    saleEstMaster.est_date = appDate;
                    saleEstMaster.karat = salEstVm[0].Karat;
                    saleEstMaster.rate = rate;
                    saleEstMaster.tax = 0;
                    //Important Columns
                    saleEstMaster.total_tax_amount = totalTaxAmount;
                    saleEstMaster.total_est_amount = totalEstAmount;
                    saleEstMaster.grand_total = grandTotal;
                    saleEstMaster.purchase_amount = purchase == null ? saleEst.PurchaseAmount : Convert.ToDecimal(purchase.total_purchase_amount);
                    saleEstMaster.spurchase_amount = saleEst.SPurchaseAmount;
                    //End
                    saleEstMaster.s_type = "S";
                    saleEstMaster.bill_no = 0;
                    // Bellow are the Null Columns
                    saleEstMaster.discount_amount = saleEst.DiscountAmount;
                    saleEstMaster.approved_by = null;
                    saleEstMaster.is_ins = "N";
                    saleEstMaster.sr_billno = 0;
                    saleEstMaster.item_set = "N";
                    saleEstMaster.remarks = null;
                    saleEstMaster.approve_flag = "Y";
                    saleEstMaster.UpdateOn = SIGlobals.Globals.GetDateTime();
                    saleEstMaster.ed_cess = 0;
                    saleEstMaster.hed_cess = 0;
                    saleEstMaster.ed_cess_percent = 0;
                    saleEstMaster.hed_cess_percent = 0;
                    saleEstMaster.excise_duty_percent = 0;
                    saleEstMaster.excise_duty_amount = 0;
                    saleEstMaster.hi_scheme_amount = 0;
                    saleEstMaster.hi_scheme_no = 0;
                    saleEstMaster.hi_bonus_amount = 0;
                    saleEstMaster.salutation = customer.salutation;
                    saleEstMaster.New_Bill_No = null;
                    saleEstMaster.isPAN = customer.pan_no == null ? "N" : "Y";
                    saleEstMaster.round_off = saleEst.RoundOff;
                    saleEstMaster.state_code = customer.state_code;
                    saleEstMaster.pos = saleEst.Pos;
                    saleEstMaster.Corparate_ID = null;
                    saleEstMaster.Corporate_Branch_ID = null;
                    saleEstMaster.Employee_Id = null;
                    saleEstMaster.Registered_MN = null;
                    saleEstMaster.profession_ID = null;
                    saleEstMaster.Empcorp_Email_ID = null;
                    saleEstMaster.phone_no = null;
                    saleEstMaster.Email_ID = null;
                    saleEstMaster.id_type = null;
                    saleEstMaster.id_details = null;
                    saleEstMaster.pan_no = customer.pan_no;
                    saleEstMaster.tin = customer.tin;
                    saleEstMaster.pin_code = customer.pin_code;
                    saleEstMaster.mobile_no = customer.mobile_no;
                    saleEstMaster.city = customer.city;
                    saleEstMaster.state = customer.state;
                    saleEstMaster.address2 = customer.address2;
                    saleEstMaster.address3 = customer.address3;
                    saleEstMaster.address1 = customer.address1;
                    db.KTTU_SALES_EST_MASTER.Add(saleEstMaster);
                    db.SaveChanges();
                    #endregion

                    #region Saving Saless Estimation Details
                    int SlNo = 1;
                    foreach (SalesEstDetailsVM sed in salEstVm) {
                        KTTU_SALES_EST_DETAILS salEstDet = new KTTU_SALES_EST_DETAILS();
                        salEstDet.obj_id = SIGlobals.Globals.GetMagnaGUID(TABLE_NAME, estNo, saleEst.CompanyCode, saleEst.BranchCode);
                        salEstDet.company_code = saleEst.CompanyCode;
                        salEstDet.branch_code = saleEst.BranchCode;
                        salEstDet.est_no = saleEstMaster.est_no;
                        salEstDet.sl_no = SlNo;
                        salEstDet.bill_no = 0;
                        salEstDet.barcode_no = sed.BarcodeNo == null ? "" : sed.BarcodeNo;
                        salEstDet.sal_code = sed.SalCode;
                        salEstDet.counter_code = sed.CounterCode;
                        salEstDet.item_name = sed.ItemName;
                        salEstDet.item_no = Convert.ToInt32(sed.ItemQty);
                        salEstDet.gwt = sed.Grosswt;
                        salEstDet.swt = sed.Stonewt;
                        salEstDet.nwt = sed.Netwt;
                        salEstDet.AddWt = sed.AddWt == null ? 0 : sed.AddWt;
                        salEstDet.DeductWt = sed.DeductWt == null ? 0 : sed.DeductWt;
                        salEstDet.making_charge_per_rs = sed.MakingChargePerRs;
                        salEstDet.wast_percent = sed.WastPercent;
                        salEstDet.gold_value = sed.GoldValue;
                        salEstDet.va_amount = sed.VaAmount;
                        salEstDet.stone_charges = sed.StoneCharges;
                        salEstDet.diamond_charges = sed.DiamondCharges;
                        salEstDet.total_amount = sed.TotalAmount;
                        salEstDet.hallmarcharges = sed.Hallmarkarges == null ? 0 : sed.Hallmarkarges;
                        salEstDet.mc_amount = sed.McAmount == null ? 0 : sed.McAmount;
                        salEstDet.wastage_grms = sed.WastageGrms == null ? 0 : sed.WastageGrms;
                        salEstDet.mc_percent = sed.McPercent;
                        salEstDet.Addqty = sed.AddQty == null ? 0 : sed.AddQty;
                        salEstDet.Deductqty = sed.DeductQty == null ? 0 : sed.DeductQty;
                        salEstDet.offer_value = sed.OfferValue == null ? 0 : sed.OfferValue;
                        salEstDet.UpdateOn = SIGlobals.Globals.GetDateTime();
                        salEstDet.gs_code = sed.GsCode;
                        salEstDet.rate = sed.Rate;
                        salEstDet.karat = sed.Karat;
                        salEstDet.ad_barcode = sed.AdBarcode == null ? "" : sed.AdBarcode;
                        salEstDet.ad_counter = sed.AdCounter == null ? "" : sed.AdCounter;
                        salEstDet.ad_item = sed.AdItem == null ? "" : sed.AdItem;
                        salEstDet.isEDApplicable = sed.IsEDApplicable == null ? "" : sed.IsEDApplicable;
                        salEstDet.mc_type = sed.McType;
                        salEstDet.Fin_Year = SIGlobals.Globals.GetFinancialYear(db, saleEst.CompanyCode, saleEst.BranchCode);
                        salEstDet.New_Bill_No = null;
                        salEstDet.item_total_after_discount = sed.ItemTotalAfterDiscount;
                        salEstDet.item_additional_discount = sed.ItemAdditionalDiscount == null ? 0 : sed.ItemAdditionalDiscount;
                        salEstDet.tax_percentage = sed.TaxPercentage == null ? 0 : sed.TaxPercentage;
                        salEstDet.tax_amount = sed.TaxAmount == null ? 0 : sed.TaxAmount;
                        salEstDet.item_final_amount = sed.ItemFinalAmount;
                        salEstDet.supplier_code = sed.SupplierCode == null ? "" : sed.SupplierCode;
                        salEstDet.item_size = sed.ItemSize == null ? "" : sed.ItemSize;
                        salEstDet.img_id = sed.ImgID == null ? "" : sed.ImgID;
                        salEstDet.design_code = sed.DesignCode == null ? "" : sed.DesignCode;
                        salEstDet.design_name = sed.DesignName == null ? "" : sed.DesignName;
                        salEstDet.batch_id = sed.BatchID == null ? "" : sed.BatchID;
                        salEstDet.rf_id = sed.Rf_ID == null ? "" : sed.Rf_ID;
                        salEstDet.mc_per_piece = sed.McPerPiece == null ? 0 : sed.McPerPiece;
                        salEstDet.Discount_Mc = sed.DiscountMc == null ? 0 : sed.DiscountMc;
                        salEstDet.Total_sales_mc = sed.TotalSalesMc;
                        salEstDet.Mc_Discount_Amt = sed.McDiscountAmt;
                        salEstDet.purchase_mc = sed.purchaseMc == null ? 0 : sed.purchaseMc;
                        salEstDet.GSTGroupCode = sed.GSTGroupCode;
                        salEstDet.SGST_Percent = sed.SGSTPercent == null ? 0 : sed.SGSTPercent;
                        salEstDet.SGST_Amount = sed.SGSTAmount == null ? 0 : sed.SGSTAmount;
                        salEstDet.CGST_Percent = sed.CGSTPercent == null ? 0 : sed.CGSTPercent;
                        salEstDet.CGST_Amount = sed.CGSTAmount == null ? 0 : sed.CGSTAmount;
                        salEstDet.IGST_Percent = sed.IGSTPercent == null ? 0 : sed.IGSTPercent;
                        salEstDet.IGST_Amount = sed.IGSTAmount == null ? 0 : sed.IGSTAmount;
                        salEstDet.CESSPercent = sed.CESSPercent == null ? 0 : sed.CESSPercent;
                        salEstDet.CESSAmount = sed.CESSAmount == null ? 0 : sed.CESSAmount;
                        salEstDet.HSN = Convert.ToString(db.usp_GetGSTHSNCode(sed.CompanyCode, sed.BranchCode, sed.GsCode, sed.ItemName).FirstOrDefault());
                        salEstDet.Piece_Rate = sed.PieceRate;
                        salEstDet.DeductSWt = sed.DeductSWt == null ? 0 : sed.DeductSWt;
                        salEstDet.Ord_Discount_Amt = sed.OrdDiscountAmt == null ? 0 : sed.OrdDiscountAmt;
                        salEstDet.ded_counter = sed.DedCounter == null ? "" : sed.DedCounter;
                        salEstDet.ded_item = sed.DedItem == null ? "" : sed.DedItem;
                        salEstDet.vaporLoss = sed.VaporLossWeight == null ? 0 : sed.VaporLossWeight;
                        salEstDet.vaporAmount = sed.VaporLossAmount == null ? 0 : sed.VaporLossAmount;
                        //salEstDet.HUID = sed.Huid;
                        db.KTTU_SALES_EST_DETAILS.Add(salEstDet);

                        #region Saving Stone Details
                        // Saving Stone details
                        List<SalesEstStoneVM> salEstStoneDet = sed.salesEstStoneVM;
                        int stoneSlNo = 1;
                        foreach (SalesEstStoneVM ses in salEstStoneDet) {
                            KTTU_SALES_STONE_DETAILS ssd = new KTTU_SALES_STONE_DETAILS();
                            ssd.obj_id = SIGlobals.Globals.GetMagnaGUID(TABLE_NAME, estNo, saleEst.CompanyCode, saleEst.BranchCode);
                            ssd.company_code = saleEst.CompanyCode;
                            ssd.branch_code = saleEst.BranchCode;
                            ssd.bill_no = ses.BillNo;
                            ssd.sl_no = stoneSlNo;
                            ssd.est_no = salEstDet.est_no;
                            ssd.est_Srno = SlNo;
                            ssd.barcode_no = ses.BarcodeNo == null ? "" : ses.BarcodeNo;
                            ssd.type = ses.Type;
                            ssd.name = ses.Name;
                            ssd.qty = ses.Qty;
                            ssd.carrat = ses.Carrat;
                            ssd.stone_wt = ses.StoneWt;
                            ssd.rate = ses.Rate;
                            ssd.amount = ses.Amount;
                            ssd.tax = ses.Tax;
                            ssd.tax_amount = ses.TaxAmount;
                            ssd.total_amount = ses.TotalAmount;
                            ssd.bill_type = ses.BillType;
                            ssd.dealer_sales_no = ses.DealerSalesNo == null ? 0 : ses.DealerSalesNo;
                            ssd.BILL_DET11PK = ses.BillDet11PK;
                            ssd.UpdateOn = SIGlobals.Globals.GetDateTime();
                            ssd.Fin_Year = SIGlobals.Globals.GetFinancialYear(db, saleEst.CompanyCode, saleEst.BranchCode);
                            ssd.color = ses.Color == null ? "" : ses.Color;
                            ssd.clarity = ses.Clarity == null ? "" : ses.Clarity;
                            ssd.shape = ses.Shape == null ? "" : ses.Shape;
                            ssd.cut = ses.Cut == null ? "" : ses.Cut;
                            ssd.polish = ses.Polish == null ? "" : ses.Polish;
                            ssd.symmetry = ses.Symmetry == null ? "" : ses.Symmetry;
                            ssd.fluorescence = ses.Fluorescence == null ? "" : ses.Fluorescence;
                            ssd.certificate = ses.Certificate == null ? "" : ses.Certificate;
                            ssd.UniqRowID = Guid.NewGuid();
                            db.KTTU_SALES_STONE_DETAILS.Add(ssd);
                            stoneSlNo = stoneSlNo + 1;
                        }
                        #endregion

                        SlNo = SlNo + 1;
                        #endregion

                    }
                    //Delete Merged Estimation
                    if (saleEst.IsMergeEst == true && saleEst.MergedEstNo != 0) {
                        DeleteEstimation(saleEst.CompanyCode, saleEst.BranchCode, saleEst.MergedEstNo);
                    }
                    db.SaveChanges();
                    transaction.Commit();
                    #region This block would remove the offer discount if the estimate is edited.
                    if (isEstimationLineChanged && offerDiscountValue > 0) {
                        //Recalculate/remove offer discount
                        GetCancelOfferDiscountF12(saleEst.CompanyCode, saleEst.BranchCode, saleEstMaster.est_no, out error);
                    }
                    #endregion
                }
                catch (Exception excp) {
                    transaction.Rollback();
                    error = new ErrorVM().GetErrorDetails(excp);
                    return 0;
                }
            }
            return estNo;
        }

        public dynamic GetStoneType(string companyCode, string branchCode)
        {
            return db.usp_LoadGS(companyCode, branchCode, "SC");
        }

        public List<KSTU_STONE_DIAMOND_MASTER> GetStoneOrDiamondDetails(string companyCode, string branchCode, string StoneType)
        {
            List<KSTU_STONE_DIAMOND_MASTER> lstOfStoneDiamond = db.KSTU_STONE_DIAMOND_MASTER.Where(ksdm => ksdm.type == StoneType
                                                                                                    && ksdm.obj_status == "O"
                                                                                                    && ksdm.company_code == companyCode
                                                                                                    && ksdm.branch_code == branchCode)
                                                                                            .OrderBy(ksdm => ksdm.stone_name).ToList();
            return lstOfStoneDiamond;
        }

        public SalesEstMasterVM GetOrderForSales(string companyCode, string branchCode, int orderNo, out ErrorVM error)
        {
            error = null;
            SalesEstMasterVM salesEst = new SalesEstMasterVM();
            try {
                string barcodeNo = string.Empty;
                KTTU_ORDER_MASTER kom = new KTTU_ORDER_MASTER();
                List<KTTU_ORDER_DETAILS> kod = new List<KTTU_ORDER_DETAILS>();
                List<SalesEstDetailsVM> lstOfSalesEstDet = new List<SalesEstDetailsVM>();

                kom = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == orderNo
                                                       && ord.company_code == companyCode
                                                       && ord.branch_code == branchCode).FirstOrDefault();
                if (kom == null) {
                    error = new ErrorVM
                    {
                        index = 0,
                        field = "",
                        description = "Invalid Order Number."
                    };
                    return null;
                }
                salesEst.CustID = kom.Cust_Id;
                salesEst.ObjID = kom.obj_id;
                salesEst.CompanyCode = kom.company_code;
                salesEst.BranchCode = kom.branch_code;
                salesEst.OrderNo = kom.order_no;
                salesEst.CustID = kom.Cust_Id;
                salesEst.CustName = kom.cust_name;
                salesEst.Remarks = kom.remarks;
                salesEst.OrderDate = kom.order_date;
                salesEst.OperatorCode = kom.operator_code;
                salesEst.GSType = kom.gs_code;
                salesEst.Rate = kom.rate;
                salesEst.GrandTotal = kom.grand_total;
                salesEst.BillNo = kom.bill_no;
                salesEst.UpdateOn = kom.UpdateOn;
                salesEst.Karat = kom.karat;
                salesEst.NewBillNo = kom.New_Bill_No;
                salesEst.TotalTaxAmount = Convert.ToDecimal(kom.total_tax_amount);
                salesEst.IsPAN = kom.isPAN;
                salesEst.Address1 = kom.address1;
                salesEst.Address2 = kom.address2;
                salesEst.Address3 = kom.address3;
                salesEst.City = kom.city;
                salesEst.Pincode = kom.pin_code;
                salesEst.MobileNo = kom.mobile_no;
                salesEst.State = kom.state;
                salesEst.StateCode = kom.state_code;
                salesEst.TIN = kom.tin;
                salesEst.PANNo = kom.pan_no;
                salesEst.PhoneNo = kom.phone_no;
                salesEst.EmailID = kom.Email_ID;
                salesEst.Salutation = kom.salutation;
                if (kom != null && kom.order_type == "R") {
                    kod = db.KTTU_ORDER_DETAILS.Where(ord => ord.order_no == orderNo
                                                           && ord.company_code == companyCode
                                                           && ord.branch_code == branchCode).ToList();
                    if (kod != null) {
                        foreach (KTTU_ORDER_DETAILS orderDet in kod) {
                            barcodeNo = orderDet.item_name;
                            SalesEstDetailsVM barcode = new BarcodeBL().GetBarcodeWithStoneForSales(companyCode, branchCode, barcodeNo, orderNo.ToString(), out error);
                            barcode.OrderNo = orderNo;
                            lstOfSalesEstDet.Add(barcode);
                        }
                    }
                    salesEst.salesEstimatonVM = lstOfSalesEstDet;
                }
                return salesEst;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public SalesEstDetailsVM BarcodeCalculation(SalesEstDetailsVM sales, string operatorCode, out ErrorVM error)
        {
            error = null;

            #region Group Barcode
            if (sales.TaggingType == "G") {
                KTTU_BARCODE_MASTER barcodeMaster = db.KTTU_BARCODE_MASTER.Where(bar => bar.company_code == sales.CompanyCode
                                                                        && bar.branch_code == sales.BranchCode
                                                                        && bar.barcode_no == sales.BarcodeNo).FirstOrDefault();

                if (Convert.ToInt32(sales.Quantity) < 0) {
                    error = new ErrorVM()
                    {
                        description = "Please enter the billing Quantity.",
                        ErrorStatusCode = HttpStatusCode.BadRequest
                    };
                    return null;
                }
                else if (sales.Quantity > barcodeMaster.qty) {
                    error = new ErrorVM()
                    {
                        description = "Not enough quantity available to bill.",
                        ErrorStatusCode = HttpStatusCode.BadRequest
                    };
                    return null;
                }

                if (Convert.ToDecimal(sales.BillingGrossWt) == 0) {
                    error = new ErrorVM()
                    {
                        description = "Please enter the Gross Weight.",
                        ErrorStatusCode = HttpStatusCode.BadRequest
                    };
                    return null;
                }
                else if (Convert.ToDecimal(sales.BillingGrossWt) > barcodeMaster.gwt) {
                    error = new ErrorVM()
                    {
                        description = "Not enough Gross Weight available to Bill.",
                        ErrorStatusCode = HttpStatusCode.BadRequest
                    };
                    return null;
                }
                if (Convert.ToDecimal(sales.BillingStoneWt) > barcodeMaster.swt) {
                    error = new ErrorVM()
                    {
                        description = "Not enough Stone weight available to Bill.",
                        ErrorStatusCode = HttpStatusCode.BadRequest
                    };
                    return null;
                }
                sales.ItemQty = sales.Quantity;
                sales.Grosswt = sales.BillingGrossWt;
                sales.Netwt = sales.BillingNetWt;
                sales.Stonewt = sales.BillingStoneWt;
            }
            #endregion

            ITEM_MASTER itemMaster = db.ITEM_MASTER.Where(i => i.company_code == sales.CompanyCode
                                                            && i.branch_code == sales.BranchCode
                                                            && i.gs_code == sales.GsCode
                                                            && i.Item_code == sales.ItemName).FirstOrDefault();

            #region Operator Discount on MC Validation
            SDTU_OPERATOR_MASTER opMaster = db.SDTU_OPERATOR_MASTER.Where(op => op.company_code == sales.CompanyCode
                                                                 && op.branch_code == sales.BranchCode
                                                                 && op.OperatorCode == operatorCode).FirstOrDefault();

            // Checking Operator Discuount MC Validation.
            if (sales.DiscountMc != null && sales.DiscountMc != 0) {
                if (sales.DiscountMc > opMaster.discount_percent) {
                    error = new ErrorVM()
                    {
                        description = "Discount MC percentage is greater than operator discount",
                        ErrorStatusCode = HttpStatusCode.BadRequest
                    };
                    return null;
                }
            }

            // Checking Valid Discount Percentage.
            if (sales.DiscountMc != 0 && sales.McPercent != 0) {
                if (sales.DiscountMc >= sales.McPercent) {
                    error = new ErrorVM()
                    {
                        description = "Enter valid MC Discount Amount",
                        ErrorStatusCode = HttpStatusCode.BadRequest
                    };
                    return null;
                }
            }
            #endregion

            #region Calculation

            decimal billingNetWeight = 0;
            decimal SGST = 0.00M, CGST = 0.00M, IGST = 0.00M, SGST_Per = 0.00M, CGST_Per = 0.00M, IGST_Per = 0.00M, Cess_Per = 0.00M, Cess_amt = 0.00M;
            decimal stoneCharges = 0, diamondCharges = 0, wastageAmount = 0;

            List<SalesEstStoneVM> lstSalEstStone = new List<SalesEstStoneVM>();
            lstSalEstStone = sales.salesEstStoneVM;

            /// Gross Weight and stone weight is calculated at UI, because Gross wt had stone wt already.
            /// becaue of the above, in  UI they should calculate Gross Weight because if they add new stone, gross wt will change.
            /// Billing net weight both UI and We should calculate. because there is no column in db for billing net weight.

            /// Removing Stone details if stonewt is zero
            if (sales.Stonewt == 0) {
                sales.StoneCharges = 0;
                if (sales.salesEstStoneVM != null) {
                    sales.salesEstStoneVM.RemoveAll(item => item.Type == "STN");
                }
            }

            // Calculating stone charges and diamond charges.
            if (lstSalEstStone != null) {
                foreach (SalesEstStoneVM stn in lstSalEstStone) {
                    if (stn.Type == "STN") {
                        stoneCharges = stoneCharges + stn.Amount;
                    }
                    else if (stn.Type == "DMD") {
                        diamondCharges = diamondCharges + stn.Amount;
                    }
                }
            }

            sales.Netwt = sales.Grosswt - sales.Stonewt;
            billingNetWeight = Convert.ToDecimal((sales.Netwt - (sales.DeductWt == null ? 0 : sales.DeductWt) + (sales.AddWt == null ? 0 : sales.AddWt)));
            #region Vapor Loss Calculation Block
            decimal vaporLossWeight = 0.00M;
            sales.VaporLossAmount = 0.00M;
            if (sales.VaporLossWeight != null && Convert.ToDecimal(sales.VaporLossWeight) > 0) {
                vaporLossWeight = Convert.ToDecimal(sales.VaporLossWeight);
                if (vaporLossWeight < 0) {
                    error = new ErrorVM
                    {
                        ErrorStatusCode = HttpStatusCode.BadRequest,
                        description = string.Format("Vapor loss weight {0} cannot be negative.}", vaporLossWeight)
                    };
                    return null;
                }
                decimal vapourLossTolerance = 0.00M;
                var vapLoss = db.KSTU_TOLERANCE_MASTER.Where(x => x.company_code == sales.CompanyCode
                    && x.branch_code == sales.BranchCode && x.obj_id == 1032021).FirstOrDefault();
                if (vapLoss != null) {
                    vapourLossTolerance = vapLoss.Max_Val;
                }
                if (vapourLossTolerance > 0) {
                    if (vaporLossWeight > vapourLossTolerance) {
                        error = new ErrorVM
                        {
                            ErrorStatusCode = HttpStatusCode.BadRequest,
                            description = string.Format("Vapor loss weight {0} exceeds the allowed vapor loss tolerance {1}", vaporLossWeight, vapourLossTolerance)
                        };
                        return null;
                    }
                }
                billingNetWeight = billingNetWeight - vaporLossWeight;

                decimal lossInVADueToVaporLoss = GetVAAmount(sales.McType.ToString(), Convert.ToDecimal(sales.Rate), vaporLossWeight, sales.Quantity,
                    0, Convert.ToDecimal(sales.WastPercent), Convert.ToDecimal(sales.McAmount), Convert.ToDecimal(sales.WastageGrms), Convert.ToDecimal(sales.McPercent), Convert.ToDecimal(sales.McPerPiece),
                     Convert.ToDecimal(sales.TotalSalesMc), Convert.ToDecimal(sales.DiscountMc));
                sales.VaporLossAmount = Convert.ToDecimal(sales.Rate) * vaporLossWeight + lossInVADueToVaporLoss;
            }
            #endregion
            if (sales.PieceRate != "Y") {
                sales.GoldValue = Convert.ToDecimal(billingNetWeight * sales.Rate);

                switch (sales.McType) {
                    case 5: // MC%
                        if (sales.McPercent < itemMaster.Min_VA_) {
                            error = new ErrorVM()
                            {
                                description = "MC Amount less than Min MC",
                                ErrorStatusCode = HttpStatusCode.BadRequest
                            };
                            return null;
                        }
                        sales.TotalSalesMc = sales.McPercent - (sales.DiscountMc == null ? 0 : sales.DiscountMc);
                        sales.VaAmount = Math.Round(Convert.ToDecimal((sales.GoldValue * sales.TotalSalesMc) / 100), 2, MidpointRounding.ToEven);
                        sales.McDiscountAmt = ((sales.GoldValue * sales.McPercent) / 100) - sales.VaAmount;
                        sales.McDiscountAmt = sales.McDiscountAmt > 1 ? sales.McDiscountAmt : 0; // Caluclated MCdisount amount come some negetive or some decimal values.
                        break;
                    case 1: // MC Per gram

                        //decimal mcPercent = (sales.GoldValue / Convert.ToDecimal(sales.McPercent)) / 100;
                        decimal mcPercent = (sales.GoldValue / Convert.ToDecimal(sales.MakingChargePerRs)) / 100;
                        if (mcPercent < itemMaster.Min_VA_) {
                            error = new ErrorVM()
                            {
                                description = "MC Amount less than Min MC",
                                ErrorStatusCode = HttpStatusCode.BadRequest
                            };
                            return null;
                        }

                        sales.TotalSalesMc = sales.MakingChargePerRs - (sales.DiscountMc == null ? 0 : sales.DiscountMc);
                        sales.VaAmount = Math.Round(Convert.ToDecimal(billingNetWeight * sales.TotalSalesMc), 2, MidpointRounding.ToEven);
                        wastageAmount = Convert.ToDecimal(((billingNetWeight * (sales.WastPercent)) / 100) * sales.Rate);
                        // Wastage Amount we are assigning to MCDiscountAmt for displaying in UI.
                        sales.McDiscountAmt = wastageAmount;
                        //Add wastage amount to VA Amount
                        sales.VaAmount = sales.VaAmount + wastageAmount;
                        break;
                    case 4: // MC Amount and Wastage
                        sales.TotalSalesMc = 0;
                        sales.VaAmount = Math.Round(Convert.ToDecimal(sales.McAmount + (Convert.ToDecimal(sales.WastageGrms) * sales.Rate)), 2, MidpointRounding.ToEven);
                        sales.McDiscountAmt = 0;
                        break;
                    case 6: // MC Per piece
                        sales.TotalSalesMc = 0;
                        sales.VaAmount = Math.Round(Convert.ToDecimal(sales.McPerPiece), 2, MidpointRounding.ToEven);
                        sales.McDiscountAmt = 0;
                        break;
                }
            }
            sales.StoneCharges = stoneCharges;
            sales.DiamondCharges = diamondCharges;
            sales.TotalAmount = Convert.ToDecimal(sales.GoldValue + sales.VaAmount + sales.StoneCharges + sales.DiamondCharges);
            sales.ItemTotalAfterDiscount = sales.TotalAmount - (sales.OfferValue == null ? 0 : sales.OfferValue);
            if (sales.BarcodeNo == "" || sales.BarcodeNo == null) {
                sales.GSTGroupCode = db.usp_GetGSTGoodsGroupCode(sales.CompanyCode, sales.BranchCode, sales.GsCode, sales.ItemName).FirstOrDefault();
            }
            // Bellow is the method where we send values for GST calculation.
            GetGSTComponentValues(sales.GSTGroupCode, Convert.ToDecimal(sales.ItemTotalAfterDiscount), sales.isInterstate == 1 ? true : false, out SGST_Per, out SGST, out CGST_Per, out CGST, out IGST_Per, out IGST, out Cess_Per, out Cess_amt, sales.CompanyCode, sales.BranchCode);
            sales.SGSTAmount = Convert.ToDecimal(SGST);
            sales.SGSTPercent = Convert.ToDecimal(SGST_Per);
            sales.CGSTAmount = Convert.ToDecimal(CGST);
            sales.CGSTPercent = Convert.ToDecimal(CGST_Per);
            sales.IGSTAmount = Convert.ToDecimal(IGST);
            sales.IGSTPercent = Convert.ToDecimal(IGST_Per);
            sales.ItemFinalAmount = Math.Round(Convert.ToDecimal(sales.ItemTotalAfterDiscount + SGST + CGST + IGST), 2, MidpointRounding.ToEven);
            return sales;
            #endregion
        }

        private decimal GetVAAmount(string mcType, decimal rate, decimal weight, int qty, decimal mcPerGrm,
            decimal wastPercentage, decimal mcAmt, decimal wastGrms, decimal mcPercent, decimal mcperPiece,
            decimal totalSalesMc, decimal discountMc)
        {
            decimal totalva = 0.00M; ;
            try {
                //mcPerGrm = Convert.ToDecimal(mcPerGrm);
                //wastPercentage = Convert.ToDecimal(wastPercentage);
                //mcAmt = Convert.ToDecimal(mcAmt);
                //wastGrms = Convert.ToDecimal(wastGrms);
                //mcPercent = Convert.ToDecimal(mcPercent);
                //mcperPiece = Convert.ToDecimal(mcperPiece);

                //totalSalesMc = Convert.ToDecimal(totalSalesMc);
                //discountMc = Convert.ToDecimal(discountMc);
                decimal McDiscountAmount = 0, DiscmcAmt = 0;

                if (string.Compare(mcType, "1") == 0)//MC per Gram, Wast%
                {
                    //  mcAmt = (netwt * mcPerGrm);
                    mcAmt = (weight * totalSalesMc);
                    wastGrms = Math.Round((wastPercentage * weight) / 100, 3, MidpointRounding.ToEven);
                    totalva = mcAmt + (wastGrms * rate);


                    DiscmcAmt = (weight * discountMc);
                    McDiscountAmount = DiscmcAmt + (wastGrms * rate);
                }
                else if (string.Compare(mcType, "2") == 0) // MC Amount
                {
                    totalva = mcAmt;
                }
                else if (string.Compare(mcType, "3") == 0)// Wastage % MC Amount
                {
                    wastGrms = Math.Round((wastPercentage * weight) / 100, 3, MidpointRounding.ToEven);
                    totalva = mcAmt + wastGrms * rate;
                }
                else if (string.Compare(mcType, "4") == 0)//MC%
                {
                    totalva = mcAmt + (wastGrms * rate);
                }
                else if (string.Compare(mcType, "5") == 0)//MC%
                {
                    totalva = (weight * rate) * (totalSalesMc / 100);

                    McDiscountAmount = ((weight * rate) * (discountMc / 100));
                }
                else if (string.Compare(mcType, "6") == 0)//MC per piece
                {
                    //totalva = (qty * mcperPiece);
                    totalva = (qty * totalSalesMc);

                    McDiscountAmount = (qty * discountMc);
                }

                totalva = Math.Round(totalva, 2, MidpointRounding.ToEven);
            }
            catch (Exception) {
            }
            return totalva;
        }

        public bool GetValidateMinMc(string companyCode, string branchCode, string gsCode, string item, decimal mcPercent, out ErrorVM error)
        {
            error = null;
            try {
                KSTU_ITEM_LIST_GROUP_MASTER kilgm = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(ki => ki.company_code == companyCode
                    && ki.branch_code == branchCode
                    && ki.gs_code == gsCode
                    && (ki.item_level1_name == item
                    || ki.item_level2_name == item
                    || ki.item_level3_name == item
                    || ki.item_level4_name == item
                    || ki.item_level5_name == item
                    || ki.item_level6_name == item)).FirstOrDefault();
                if (mcPercent < kilgm.min_profit_percent) {
                    error = new ErrorVM()
                    {
                        description = "MC amount is less than Min MC.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
                else {
                    return true;
                }
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public SalesEstMasterVM GetOfferDiscountCalculationModel(SalesEstMasterVM sales)
        {
            decimal totalNetWt = 0;
            decimal totalInvoiceKarat = 0;
            bool isInterstate = false;

            KTTU_SALES_EST_MASTER saleEstMaster = new KTTU_SALES_EST_MASTER();
            List<SalesEstDetailsVM> salEstVm = sales.salesEstimatonVM;

            // Calculating Total Invoice Karat
            foreach (SalesEstDetailsVM sed in salEstVm) {
                List<SalesEstStoneVM> salEstStoneDet = sed.salesEstStoneVM;
                totalNetWt = totalNetWt + sed.Netwt;
                foreach (SalesEstStoneVM ses in salEstStoneDet) {
                    if (ses.Type == "D" || ses.Type == "DMD" || ses.Type == "DM") {
                        totalInvoiceKarat += ses.Carrat;
                    }
                }
            }

            //Calculation By Itemwise
            foreach (SalesEstDetailsVM sed in salEstVm) {
                decimal itemTotalKarat = 0;
                int itemTotalDiamondQty = 0;
                decimal sDiamondCarrat = 0;
                decimal itemWiseDiscount = 0;

                List<SalesEstStoneVM> salEstStoneDet = sed.salesEstStoneVM;
                foreach (SalesEstStoneVM ses in salEstStoneDet) {
                    if (ses.Type == "D" || ses.Type == "DMD" || ses.Type == "DM") {
                        itemTotalKarat += ses.Carrat;
                        itemTotalDiamondQty += ses.Qty;
                    }
                }
                sed.ItemAdditionalDiscount = itemWiseDiscount;
                if (itemTotalKarat > 0 && itemTotalDiamondQty > 0) {
                    sDiamondCarrat = itemTotalKarat / itemTotalDiamondQty;
                }
                itemWiseDiscount = GetItemWiseDiscount(sed.GsCode, sed.ItemName, sed.Netwt, sed.TotalAmount, itemTotalKarat,
                    sed.StoneCharges, sed.CounterCode, sed.VaAmount, totalInvoiceKarat, totalNetWt, sed.BarcodeNo, sDiamondCarrat, Convert.ToDecimal(sed.DiamondCharges), sales.CompanyCode, sales.BranchCode);

                sed.OfferValue = itemWiseDiscount;
                sed.ItemTotalAfterDiscount = Math.Round(Convert.ToDecimal(sed.TotalAmount - sed.OfferValue), 2, MidpointRounding.ToEven);

                decimal SGST = 0.00M, CGST = 0.00M, IGST = 0.00M, SGST_Per = 0.00M, CGST_Per = 0.00M, IGST_Per = 0.00M, Cess_Per = 0.00M, Cess_amt = 0.00M;
                GetGSTComponentValues(sed.GSTGroupCode, Convert.ToDecimal(sed.ItemTotalAfterDiscount), isInterstate, out SGST_Per, out SGST, out CGST_Per, out CGST, out IGST_Per, out IGST, out Cess_Per, out Cess_amt, sales.CompanyCode, sales.BranchCode);
                sed.SGSTAmount = SGST;
                sed.SGSTPercent = SGST_Per;
                sed.CGSTAmount = CGST;
                sed.CGSTPercent = CGST;
                sed.IGSTAmount = IGST;
                sed.IGSTPercent = IGST_Per;
                sed.ItemFinalAmount = Math.Round(Convert.ToDecimal(sed.ItemTotalAfterDiscount + SGST + CGST + IGST), 2, MidpointRounding.ToEven);
            }
            return sales;
        }

        public decimal GetAddWightItemGrossWeight(string companyCode, string branchCode, string gsCode, string counterCode, string itemName)
        {
            KTTU_COUNTER_STOCK counterStock = db.KTTU_COUNTER_STOCK.Where(stock => stock.company_code == companyCode && stock.branch_code == branchCode && stock.gs_code == gsCode && stock.counter_code == counterCode && stock.item_name == itemName).FirstOrDefault();
            return counterStock == null ? 0 : Convert.ToDecimal(counterStock.closing_gwt);
        }

        public string EstimationDotMatrxiPrint(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            KTTU_SALES_EST_MASTER salesEstMaster = db.KTTU_SALES_EST_MASTER.Where(est => est.est_no == estNo
                                                                            && est.company_code == companyCode
                                                                            && est.branch_code == branchCode).FirstOrDefault();
            if (salesEstMaster == null) return "";
            List<KTTU_SALES_EST_DETAILS> salesEstDet = db.KTTU_SALES_EST_DETAILS.Where(est => est.est_no == estNo
                                                                                        && est.company_code == companyCode
                                                                                        && est.branch_code == branchCode).ToList();
            KTTU_PURCHASE_EST_MASTER purchase = db.KTTU_PURCHASE_EST_MASTER.Where(est => est.est_no == estNo
                                                                                        && est.company_code == companyCode
                                                                                        && est.branch_code == branchCode).FirstOrDefault();
            try {
                StringBuilder sb = new StringBuilder();
                StringBuilder strLine = new StringBuilder();
                strLine.Append('-', 40);
                StringBuilder strTransLine = new StringBuilder();
                strTransLine.Append('-', 40);
                decimal Spaces = 0M;
                StringBuilder strSpaces = new StringBuilder();
                Spaces = ((40 - strTransLine.Length) / 3);
                strSpaces.Append(' ', Convert.ToInt32(Spaces));
                sb.AppendLine(string.Format("{0,10}{1,-30}", "", "SALES ESTIMATION"));

                sb.AppendLine();
                sb.AppendLine(string.Format("{0,-8}{1,-12}{2,20}"
                    , salesEstDet[0].sal_code.ToString()
                    , salesEstMaster.est_date.ToString()
                    , "EST - " + salesEstMaster.est_no)
                    );

                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                sb.Append(string.Format("{0,-16}{1,-4}{2,10}{3,10}", "Item", "Qty", "N.wt(g)", "Amount"));
                sb.AppendLine();
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));

                decimal totGrosswt = 0, TotalAmt = 0, offervalue = 0, totNetWt = 0;
                decimal tax = Convert.ToDecimal(salesEstMaster.tax);

                for (int i = 0; i < salesEstDet.Count; i++) {
                    int sl_no = Convert.ToInt32(salesEstDet[i].sl_no);
                    //int Pcs = Convert.ToInt32(salesEstDet[i]); //todo
                    string mc_type = salesEstDet[i].mc_type.ToString();
                    decimal itemGrswt = Convert.ToDecimal(salesEstDet[i].gwt);
                    decimal netwt = Convert.ToDecimal(salesEstDet[i].nwt);

                    decimal WstPercent = Convert.ToDecimal(salesEstDet[i].wast_percent);
                    decimal WastWt = Convert.ToDecimal(salesEstDet[i].wastage_grms);

                    decimal makingCharge = Convert.ToDecimal(salesEstDet[i].making_charge_per_rs);
                    decimal McAmount = Convert.ToDecimal(salesEstDet[i].mc_amount);
                    decimal McPercentage = Convert.ToDecimal(salesEstDet[i].mc_percent);
                    decimal McPerPiece = Convert.ToDecimal(salesEstDet[i].mc_per_piece);
                    decimal VAAmount = Convert.ToDecimal(salesEstDet[i].va_amount);

                    decimal StoneCharges = Convert.ToDecimal(salesEstDet[i].stone_charges);
                    decimal DiamondCharges = Convert.ToDecimal(salesEstDet[i].diamond_charges);
                    decimal Hallmark = Convert.ToDecimal(salesEstDet[i].hallmarcharges);

                    decimal itemAmount = Convert.ToDecimal(salesEstDet[i].item_final_amount);

                    string itemName = salesEstDet[i].item_name;
                    string gsCode = salesEstDet[i].gs_code;
                    ITEM_MASTER item = db.ITEM_MASTER.Where(it => it.company_code == companyCode && it.branch_code == branchCode
                                                                && it.Item_code == itemName && it.gs_code == gsCode).FirstOrDefault();
                    sb.AppendLine(string.Format("{0}", item.Item));

                    string Barcode = string.Empty;
                    string VACode = string.Empty;
                    //VACode = " [" + CGlobals.GetEstimationItemMCCode(EstNo, sl_no) + "] "; //Todo
                    if (!string.IsNullOrEmpty(salesEstDet[i].barcode_no.ToString())) {
                        Barcode = salesEstDet[i].barcode_no.ToString() + VACode;
                        //sb.Append(string.Format("{0,-16}{1,-4}", Barcode, Pcs)); //Todo
                    }
                    else {
                        //sb.Append(string.Format("{0,-16}{1,-4}", VACode, Pcs));
                    }
                    sb.Append(string.Format("{0,10}", salesEstDet[i].nwt));
                    sb.AppendLine(string.Format("{0,10}", itemAmount.ToString("N")));

                    if (VAAmount > 0)
                        sb.AppendLine(string.Format("{0,-15}", Convert.ToInt32(VAAmount)));

                    totGrosswt += itemGrswt;
                    totNetWt += netwt;
                    TotalAmt += itemAmount;
                    offervalue += Convert.ToDecimal(salesEstDet[i].offer_value);
                    if (StoneCharges > 0) {
                        sb.AppendLine(string.Format("ST : {0}", StoneCharges));
                    }
                    if (DiamondCharges > 0) {
                        sb.AppendLine(string.Format("DM : {0}", DiamondCharges));
                    }
                    sb.AppendLine();
                }

                if ((Convert.ToDecimal(salesEstMaster.purchase_amount) > 0)
                    || (Convert.ToDecimal(salesEstMaster.order_amount) > 0)
                    || (Convert.ToDecimal(salesEstMaster.spurchase_amount) > 0)) {
                    sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                    sb.AppendLine(string.Format("{0,-15}{1,25}", "Sales Amount : ", TotalAmt));
                    if (Convert.ToDecimal(salesEstMaster.order_amount) > 0) {
                        sb.AppendLine(string.Format("{0,-15}{1,25}", "Order Adj : ", Convert.ToDecimal(salesEstMaster.order_amount)));
                        TotalAmt -= Convert.ToDecimal(salesEstMaster.order_amount);
                    }
                    if (Convert.ToDecimal(salesEstMaster.purchase_amount) > 0) {
                        sb.AppendLine(string.Format("{0,-15}{1,25}", "TIG. Amount : ", Convert.ToDecimal(salesEstMaster.purchase_amount)));
                        TotalAmt -= Convert.ToDecimal(salesEstMaster.purchase_amount);
                    }
                    if (Convert.ToDecimal(salesEstMaster.spurchase_amount) > 0) {
                        sb.AppendLine(string.Format("{0,-15}{1,25}", "SR. Amount : ", Convert.ToDecimal(salesEstMaster.spurchase_amount)));
                        TotalAmt -= Convert.ToDecimal(salesEstMaster.spurchase_amount);
                    }
                }
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));

                TotalAmt = Math.Round(TotalAmt, 2, MidpointRounding.ToEven);
                sb.AppendLine(string.Format("{0,-20}{1,10}{2,10}", "TOTAL", totNetWt, TotalAmt.ToString("N")));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));

                decimal salesGSt = Convert.ToDecimal(salesEstMaster.total_tax_amount);
                decimal purchaseGSt = Convert.ToDecimal(purchase == null ? 0 : purchase.total_tax_amount);
                decimal finalTax = salesGSt - purchaseGSt;
                finalTax = Math.Round(finalTax, 2, MidpointRounding.ToEven);

                if (finalTax > 0)
                    sb.AppendLine(string.Format("{0,-20}{1,10}{2,10}", "GST" + " (Inclusive)", "", finalTax.ToString("N")));

                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                string esc = Convert.ToChar(27).ToString();

                if (offervalue > 0) {
                    sb.AppendLine(string.Format("{0,-15}{1,25}", "Discount : ", offervalue));
                }

                KSTU_RATE_MASTER rateMaster = db.KSTU_RATE_MASTER.Where(rate => rate.gs_code == "NGO" && rate.company_code == companyCode
                                                                            && rate.branch_code == branchCode
                                                                            && (rate.karat == salesEstMaster.karat || rate.karat == "NA" || rate.karat == "")).FirstOrDefault();

                decimal GoldRate = rateMaster == null ? 0 : rateMaster.rate;
                if (GoldRate > 0)
                    sb.AppendLine(string.Format("{0,-20}{1,20}", "Gold Rate (" + salesEstMaster.karat.ToString() + ") :"
                        , GoldRate.ToString("N") + "/Grm."));
                sb.Append(esc + Convert.ToChar(100).ToString() + Convert.ToChar(9).ToString());
                sb.AppendLine(esc + Convert.ToChar(105).ToString());
                return sb.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        public string EstimationDotMatrxiPrint80(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            KTTU_SALES_EST_MASTER salesEstMaster = db.KTTU_SALES_EST_MASTER.Where(est => est.est_no == estNo
                                                                            && est.company_code == companyCode
                                                                            && est.branch_code == branchCode).FirstOrDefault();
            if (salesEstMaster == null) {
                error = new ErrorVM()
                {
                    description = "Invalid Estimation Number",
                    ErrorStatusCode = HttpStatusCode.NotFound
                };
                return "";
            }
            List<KTTU_SALES_EST_DETAILS> salesEstDet = db.KTTU_SALES_EST_DETAILS.Where(est => est.est_no == estNo
                                                                            && est.company_code == companyCode
                                                                            && est.branch_code == branchCode).ToList();
            KTTU_PURCHASE_EST_MASTER purchase = db.KTTU_PURCHASE_EST_MASTER.Where(est => est.est_no == estNo
                                                                            && est.company_code == companyCode
                                                                            && est.branch_code == branchCode).FirstOrDefault();
            try {
                StringBuilder sb = new StringBuilder();
                int width = 97;
                StringBuilder strdoubleTransLine = new StringBuilder();
                strdoubleTransLine.Append('=', width);

                StringBuilder strTransLine = new StringBuilder();
                strTransLine.Append('-', width);
                decimal Spaces = 0M;
                StringBuilder strSpaces = new StringBuilder();
                Spaces = ((width - strTransLine.Length) / 3);
                strSpaces.Append(' ', Convert.ToInt32(Spaces));

                string esc = Convert.ToChar(27).ToString();
                sb.Append(Convert.ToChar(18).ToString());
                sb.Append(esc + Convert.ToChar(14).ToString() + Convert.ToChar(14).ToString());
                sb.Append(esc + Convert.ToChar(77).ToString());
                sb.Append(Convert.ToChar(27).ToString() + Convert.ToChar(69).ToString());
                sb.Append(string.Format("{0,-6}{1,-10}{2,4}{3,6}{4,1}{5,6}{6,8}{7,1}{8,1}{9,2}{10,1}{11,-1}", salesEstMaster.Cust_Name.ToString(), "", "Sales Estimate : " + estNo, "", "", "", "", salesEstMaster.karat.ToString(), "", "", "", "Rate:" + salesEstMaster.rate.ToString()));
                sb.Append(Convert.ToChar(27).ToString() + Convert.ToChar(70).ToString());
                sb.AppendLine();
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));
                sb.AppendLine(string.Format("{0,-7}{1,-12}{2,3}{3,8}{4,8}{5,8}{6,8}{7,10}{8,10}{9,10}{10,10}"
                  , "Item", "Item", "Nos", "Gr.Wt", "St.Wt", "Net Wt", "Wst.Wt", "J.Value", "St.Amt", "MC.Amt", "Amount"));
                sb.AppendLine(string.Format("{0,-7}{1,-12}{2,3}{3,8}{4,8}{5,8}{6,8}{7,10}{8,10}{9,10}{10,10}"
                    , "Code", "Name", "", "(Grams)", "(Grams)", "(Grams)", "(Grams)", "(Rs.Ps)", "(Rs.Ps)", "(Rs.Ps)", "(Rs.Ps)"));

                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));
                string metaltype = SIGlobals.Globals.GetMetalType(db, salesEstMaster.gstype.ToString(), companyCode, branchCode);// CGlobals.GetMetalType(dtEstimation.Rows[0]["gstype"].ToString());

                //string StrEstdetails = string.Format("Select sl_no,barcode_no,item_name,item_no+Addqty-Deductqty as Pcs,\n"
                //                        + "gwt+AddWt-DeductWt as gwt,swt,nwt+AddWt-DeductWt as nwt ,gold_value,total_amount,sal_code,item_total_after_discount,\n"
                //                        + "diamond_charges,hallmarcharges,stone_charges,diamond_charges,va_amount,mc_type,gs_code,wastage_grms,counter_code,mc_per_piece, rate,\n"
                //                        + "making_charge_per_rs,mc_percent,mc_amount,isnull(Discount_Mc,0) as Discount_Mc,Total_sales_mc,item_final_amount,mc_discount_amt,SGST_Percent,CGST_Percent,IGST_Percent\n"
                //                        + "from KTTU_SALES_EST_DETAILS where est_no = {0} and company_code = '{1}' and branch_code = '{2}' order by sl_no"
                //, estNo, companyCode, branchCode);
                //DataTable dtEstDetails = SIGlobals.Globals.ExecuteQuery(StrEstdetails);

                //string GstAmount = string.Format("select sum(cgst_amount) as cgst, sum(sgst_amount) as sgst, sum(igst_amount) as igst,cgst_percent,sgst_percent,igst_percent from KTTU_SALES_EST_DETAILS where est_no='{0}' and company_code = '{1}' and branch_code = '{2}' group by cgst_percent,sgst_percent,igst_percent", estNo, companyCode, branchCode);
                //DataTable dtGst = SIGlobals.Globals.ExecuteQuery(GstAmount);

                int Pcs = 0;
                decimal Gwt = 0, Swt = 0, Nwt = 0, GoldValue = 0, StoneChg = 0, ValueAmt = 0, TotalAmt = 0, VAPercent = 0, RoundOffAmt = 0, Wst = 0;
                decimal dicountMc = 0, totalTaxableAmount = 0;
                string ItemName = string.Empty;
                decimal ItemAmt = 0;
                decimal VaAmt = 0;
                decimal discountVA = 0, totalMC = 0, totalJvalue = 0;


                List<string> lstItemName = null;

                for (int i = 0; i < salesEstDet.Count; i++) {
                    decimal GANew = 0;
                    discountVA = 0;
                    string IsPieceItem = SIGlobals.Globals.IsPeiceItem(db, salesEstDet[i].gs_code.ToString(), salesEstDet[i].item_name.ToString(), companyCode, branchCode);
                    decimal pieceamount = Convert.ToDecimal(salesEstDet[i].mc_per_piece.ToString());
                    bool IsPiese = false;

                    if (pieceamount > 0) {
                        IsPiese = true;
                    }
                    else {
                        IsPiese = false;
                    }
                    if (salesEstDet[i].mc_type.ToString() == "5" || salesEstDet[i].mc_type.ToString() == "4" || salesEstDet[i].mc_type.ToString() == "1") {

                        decimal VA = Convert.ToDecimal(salesEstDet[i].va_amount);
                        decimal GA = Convert.ToDecimal(salesEstDet[i].gold_value);
                        if (salesEstDet[i].mc_type.ToString() == "5") {
                            GANew = Convert.ToDecimal(salesEstDet[i].gold_value) + Convert.ToDecimal(salesEstDet[i].stone_charges) + Convert.ToDecimal(salesEstDet[i].diamond_charges);
                            VaAmt = (GA * Convert.ToDecimal(salesEstDet[i].mc_percent)) / 100;
                        }

                        if (salesEstDet[i].mc_type.ToString() == "4" || salesEstDet[i].mc_type.ToString() == "1") {
                            GANew = Convert.ToDecimal(salesEstDet[i].gold_value) + Convert.ToDecimal(salesEstDet[i].stone_charges) + Convert.ToDecimal(salesEstDet[i].diamond_charges);
                            VaAmt = VA;
                        }
                        if (VA > 0) {

                            decimal VA_Percent = decimal.Round((VA / GA) * 100, 2);
                            VAPercent = decimal.Round(VA_Percent, 2);
                        }
                        else {
                            decimal VA_Percent = 0.00M;
                            VAPercent = decimal.Round(VA_Percent, 2);
                        }
                    }
                    else {
                        decimal VA_Percent = 0.00M;
                        VAPercent = decimal.Round(VA_Percent, 2);

                    }
                    string VAp = string.Empty;
                    if (Convert.ToDecimal(salesEstDet[i].mc_percent) > 0)
                        VAp = Convert.ToDecimal(salesEstDet[i].mc_percent).ToString().Replace(".", "#");
                    else
                        VAp = VAPercent.ToString().Replace(".", "#");
                    sb.Append(string.Format("{0,-7}", VAp));

                    ItemName = SIGlobals.Globals.GetAliasName(db, salesEstDet[i].gs_code.ToString(), salesEstDet[i].item_name.ToString(), companyCode, branchCode);
                    if (ItemName.Length <= 12)
                        sb.Append(string.Format("{0,-12}", ItemName));
                    else {
                        lstItemName = SIGlobals.Globals.SplitStringAt(12, ItemName);
                        sb.Append(string.Format("{0,-12}", lstItemName[0]));
                    }
                    sb.Append(string.Format("{0,3}", salesEstDet[i].item_no + salesEstDet[i].Addqty - salesEstDet[i].Deductqty));


                    decimal MCAmount = 0;
                    if (Convert.ToDecimal(salesEstDet[i].making_charge_per_rs) > 0) {
                        MCAmount = Convert.ToDecimal(salesEstDet[i].nwt + salesEstDet[i].AddWt - salesEstDet[i].DeductWt) * Convert.ToDecimal(salesEstDet[i].making_charge_per_rs);
                    }
                    else if (Convert.ToDecimal(salesEstDet[i].mc_percent) > 0) {
                        MCAmount = (Convert.ToDecimal(salesEstDet[i].gold_value) * Convert.ToDecimal(salesEstDet[i].mc_percent)) / 100;
                    }
                    else {
                        MCAmount = Convert.ToDecimal(salesEstDet[i].mc_amount);
                    }
                    MCAmount = decimal.Round(MCAmount, 2);
                    decimal goldValue = Convert.ToDecimal(salesEstDet[i].gold_value);
                    ItemAmt = Convert.ToDecimal(salesEstDet[i].total_amount.ToString());
                    if (IsPieceItem.Equals("Y")) {
                        goldValue = 0;
                        GoldValue += goldValue;
                    }

                    if (salesEstDet[i].mc_type.ToString() == "0" || salesEstDet[i].mc_type.ToString() == "6") {
                        ItemAmt = Convert.ToDecimal(salesEstDet[i].total_amount.ToString());
                    }
                    else {
                        decimal wastAmt = 0;
                        decimal wastGrams = Convert.ToDecimal(salesEstDet[i].wastage_grms);
                        if (wastGrams > 0) {
                            wastAmt = decimal.Round((wastGrams * Convert.ToDecimal(salesEstDet[i].rate)), 2);
                            goldValue += wastAmt;
                        }
                        GoldValue += goldValue;
                    }
                    totalTaxableAmount += Convert.ToDecimal(salesEstDet[i].item_total_after_discount);
                    sb.Append(string.Format("{0,8}", salesEstDet[i].gwt + salesEstDet[i].AddWt - salesEstDet[i].DeductWt));
                    sb.Append(string.Format("{0,8}", salesEstDet[i].swt));
                    sb.Append(string.Format("{0,8}", salesEstDet[i].nwt + salesEstDet[i].AddWt - salesEstDet[i].DeductWt));
                    sb.Append(string.Format("{0,8}", salesEstDet[i].wastage_grms));
                    sb.Append(string.Format("{0,10}", goldValue));
                    sb.Append(string.Format("{0,10}", Convert.ToDecimal(salesEstDet[i].stone_charges) + Convert.ToDecimal(salesEstDet[i].diamond_charges) + Convert.ToDecimal(salesEstDet[i].hallmarcharges)));
                    dicountMc += Convert.ToDecimal(salesEstDet[i].Mc_Discount_Amt);

                    Gwt += Convert.ToDecimal(salesEstDet[i].gwt + salesEstDet[i].AddWt - salesEstDet[i].DeductWt);
                    Swt += Convert.ToDecimal(salesEstDet[i].swt);
                    Nwt += Convert.ToDecimal(salesEstDet[i].nwt + salesEstDet[i].AddWt - salesEstDet[i].DeductWt);
                    Wst += Convert.ToDecimal(salesEstDet[i].wastage_grms);
                    totalJvalue += goldValue;

                    StoneChg += Convert.ToDecimal(salesEstDet[i].stone_charges) + Convert.ToDecimal(salesEstDet[i].diamond_charges) + Convert.ToDecimal(salesEstDet[i].hallmarcharges);
                    ValueAmt += MCAmount;

                    if (salesEstDet[i].mc_type.ToString() == "5") {
                        discountVA = Convert.ToDecimal(salesEstDet[i].mc_percent) - Convert.ToDecimal(salesEstDet[i].Discount_Mc);
                    }
                    sb.Append(string.Format("{0,10}", MCAmount));
                    totalMC += MCAmount;
                    if (IsPieceItem.Equals("Y"))
                        ItemAmt = Convert.ToDecimal(salesEstDet[i].gold_value) + MCAmount;
                    else

                        ItemAmt = MCAmount + goldValue + Convert.ToDecimal(salesEstDet[i].stone_charges) + Convert.ToDecimal(salesEstDet[i].diamond_charges) + Convert.ToDecimal(salesEstDet[i].hallmarcharges);
                    sb.Append(string.Format("{0,10}", ItemAmt));
                    if (lstItemName != null && lstItemName.Count > 1) {
                        for (int x = 1; x < lstItemName.Count; x++)
                            sb.Append(string.Format("{0,-6}{1,-90}", "", "-" + lstItemName[x].Trim()));

                        ItemName = string.Empty;
                        lstItemName = null;
                    }
                    Pcs += Convert.ToInt32(salesEstDet[i].item_no + salesEstDet[i].Addqty - salesEstDet[i].Deductqty);
                    TotalAmt += ItemAmt;
                    sb.AppendLine(string.Format("{0}", " " + salesEstDet[i].counter_code));
                }
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                sb.AppendLine(string.Format("{0,-7}{1,-12}{2,3}{3,8}{4,8}{5,8}{6,8}{7,10}{8,10}{9,10}{10,10}"
                    , "", "", Pcs, Gwt, Swt, Nwt, Wst, totalJvalue, StoneChg, totalMC, TotalAmt));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));

                dicountMc += Convert.ToDecimal(salesEstMaster.discount_amount.ToString());
                if (dicountMc > 0) {
                    decimal totalAmount = Math.Round((TotalAmt), 2);
                    sb.AppendLine(string.Format("{0,78}{1,16}", "Discount Amount :", dicountMc));
                }

                if (Convert.ToDecimal(salesEstMaster.excise_duty_amount) > 0) {
                    sb.AppendLine(string.Format("{0,78}{1,16}"
                        , "Excise Duty @ " + salesEstMaster.excise_duty_percent.ToString() + "%" + " :"
                        , salesEstMaster.excise_duty_amount.ToString()));
                    TotalAmt += Convert.ToDecimal(salesEstMaster.excise_duty_amount);

                    if (Convert.ToDecimal(salesEstMaster.ed_cess) > 0) {
                        sb.AppendLine(string.Format("{0,78}{1,16}"
                            , "ED Cess @ " + salesEstMaster.ed_cess_percent.ToString() + "%" + " :"
                            , salesEstMaster.ed_cess.ToString()));
                        TotalAmt += Convert.ToDecimal(salesEstMaster.ed_cess);
                    }
                    if (Convert.ToDecimal(salesEstMaster.hed_cess) > 0) {
                        sb.AppendLine(string.Format("{0,78}{1,16}"
                            , "HED Cess @ " + salesEstMaster.hed_cess_percent.ToString() + "%" + " :"
                            , salesEstMaster.hed_cess.ToString()));
                        TotalAmt += Convert.ToDecimal(salesEstMaster.hed_cess);
                    }
                }

                sb.AppendLine(string.Format("{0,78}{1,16}", "Taxable Value :", totalTaxableAmount));
                sb.AppendLine(string.Format("{0,78}{1,16}", "SGST " + salesEstDet[0].SGST_Percent + " %:", Convert.ToDecimal(salesEstDet.Sum(gst => gst.SGST_Amount).ToString())));
                sb.AppendLine(string.Format("{0,78}{1,16}", "CGST " + salesEstDet[0].CGST_Percent + " %:", Convert.ToDecimal(salesEstDet.Sum(gst => gst.CGST_Amount).ToString())));
                sb.AppendLine(string.Format("{0,78}{1,16}", "IGST " + salesEstDet[0].IGST_Percent + " %:", Convert.ToDecimal(salesEstDet.Sum(gst => gst.IGST_Amount).ToString())));
                if (Convert.ToDecimal(salesEstMaster.tax) > 0) {
                    sb.AppendLine(string.Format("{0,-38}{1,40}{2,16}"
                                                , ""
                                                , "GST" + salesEstMaster.tax.ToString() + "%" + " :"
                                                , salesEstMaster.total_tax_amount.ToString()));
                    TotalAmt += Convert.ToDecimal(salesEstMaster.total_tax_amount);
                }
                else {

                }

                TotalAmt = totalTaxableAmount + Convert.ToDecimal(salesEstDet.Sum(gst => gst.SGST_Amount))
                    + Convert.ToDecimal(salesEstDet.Sum(gst => gst.CGST_Amount))
                    + Convert.ToDecimal(salesEstDet.Sum(gst => gst.IGST_Amount));
                sb.AppendLine(string.Format("{0,-5}{1,-35}{2,38}{3,16}"
                                            , ""
                                            , ""
                                            , "Sales Total :"
                                            , TotalAmt));
                if (Convert.ToDecimal(salesEstMaster.TotalDiscountVoucherAmt) > 0 && salesEstMaster.offerType != "SOD") {
                    sb.AppendLine(string.Format("{0,78}{1,16}", "Offer Discount :", Convert.ToDecimal(salesEstMaster.TotalDiscountVoucherAmt)));
                }
                if (Convert.ToDecimal(salesEstMaster.TotalDiscountVoucherAmt) > 0 && salesEstMaster.offerType == "SOD") {
                    sb.AppendLine(string.Format("{0,78}{1,16}", "Discount on JPP :", Convert.ToDecimal(salesEstMaster.TotalDiscountVoucherAmt)));
                }
                TotalAmt -= Convert.ToDecimal(salesEstMaster.TotalDiscountVoucherAmt);

                if (Convert.ToDecimal(salesEstMaster.purchase_amount) > 0) {
                    KTTU_PURCHASE_EST_MASTER purchaseMater = db.KTTU_PURCHASE_EST_MASTER.Where(pur => pur.est_no == estNo && pur.company_code == companyCode && pur.branch_code == branchCode).FirstOrDefault();
                    decimal NewPurAmount = purchaseMater == null ? 0 : purchaseMater.grand_total;

                    if (NewPurAmount > 0) {
                        sb.AppendLine(string.Format("{0,78}{1,16}", "Less Purchase (As Per Est No : " + estNo + ") :", NewPurAmount));
                        TotalAmt -= NewPurAmount;
                    }
                    //string ExistingPur = string.Format("select Ref_BillNo,pay_amt from KTTU_PAYMENT_DETAILS where trans_type = 'A' \n"
                    //                                    + "and pay_mode = 'PE' and series_no = {0} and Company_code='{1}' and branch_code='{2}'"
                    //                                    , estNo, companyCode, branchCode);
                    //DataTable dtExistingPur = SIGlobals.Globals.ExecuteQuery(ExistingPur);

                    List<KTTU_PAYMENT_DETAILS> existingPurchase = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.trans_type == "A" && pay.pay_mode == "PE" && pay.series_no == estNo && pay.company_code == companyCode && pay.branch_code == branchCode).ToList();

                    if (existingPurchase != null && existingPurchase.Count > 0) {
                        for (int i = 0; i < existingPurchase.Count; i++) {
                            sb.AppendLine(string.Format("{0,78}{1,16}",
                                "Less Purchase(As Per Est No:" + existingPurchase[i].Ref_BillNo.ToString() + ") :"
                                , existingPurchase[i].pay_amt.ToString()));
                            TotalAmt -= Convert.ToDecimal(existingPurchase[i].pay_amt);
                        }
                    }
                }
                //string ExistingSR = string.Format("select Ref_BillNo,pay_amt from KTTU_PAYMENT_DETAILS where trans_type = 'A' \n"
                //                                    + "and pay_mode = 'SE' and series_no = {0} and Company_code='{1}' and branch_code='{2}'"
                //                                    , estNo, companyCode, branchCode);
                //DataTable dtExistingSR = SIGlobals.Globals.ExecuteQuery(ExistingSR);

                List<KTTU_PAYMENT_DETAILS> existingSR = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.trans_type == "A" && pay.pay_mode == "SE" && pay.series_no == estNo && pay.company_code == companyCode && pay.branch_code == branchCode).ToList();
                if (existingSR != null && existingSR.Count > 0) {
                    for (int i = 0; i < existingSR.Count; i++) {
                        sb.AppendLine(string.Format("{0,78}{1,16}"
                            , "Less SR (As Per Est No: " + existingSR[i].Ref_BillNo.ToString() + ") :"
                            , existingSR[i].pay_amt.ToString()));
                        TotalAmt -= Convert.ToDecimal(existingSR[i].pay_amt);
                    }
                }
                if (Convert.ToDecimal(salesEstMaster.order_amount) > 0) {
                    sb.AppendLine(string.Format("{0,78}{1,16}"
                        , "Less Advance (As Per Order No:" + salesEstMaster.order_no.ToString() + " Dated : " + salesEstMaster.order_date.ToString() + ") :"
                        , salesEstMaster.order_amount.ToString()));
                    TotalAmt -= Convert.ToDecimal(salesEstMaster.order_amount);
                }


                //string MultipleOrder = string.Format("select Ref_BillNo,pay_amt from KTTU_PAYMENT_DETAILS where trans_type = 'A' \n"
                //                                    + "and pay_mode = 'OP' and series_no = {0} and Company_code='{1}' and branch_code='{2}'"
                //                                    , estNo, companyCode, branchCode);
                //DataTable dtMultipleOrd = SIGlobals.Globals.ExecuteQuery(MultipleOrder);

                List<KTTU_PAYMENT_DETAILS> multipleOrder = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.trans_type == "A" && pay.pay_mode == "OP" && pay.series_no == estNo && pay.company_code == companyCode && pay.branch_code == branchCode).ToList();
                if (multipleOrder != null && multipleOrder.Count > 0) {
                    for (int i = 0; i < multipleOrder.Count; i++) {
                        sb.AppendLine(string.Format("{0,78}{1,16}"
                            , "Less Advance (As Per Order No:" + multipleOrder[i].Ref_BillNo.ToString() + ") :"
                            , multipleOrder[i].pay_amt.ToString()));
                        TotalAmt -= Convert.ToDecimal(multipleOrder[i].pay_amt);
                    }
                }

                TotalAmt = Math.Round(TotalAmt, 0, MidpointRounding.ToEven);
                RoundOffAmt = Convert.ToDecimal(salesEstMaster.round_off);
                //string salesman = SIGlobals.Globals.ExecuteQuery(string.Format("select (sal_code + '-' + sal_name) as CodeName from KSTU_SALESMAN_MASTER where  sal_code='{0}' and company_code = '{1}' and branch_code = '{2}'", dtEstDetails.Rows[0]["sal_code"].ToString(), companyCode, branchCode)).Rows[0]["CodeName"].ToString();
                string saleCode = Convert.ToString(salesEstDet[0].sal_code);
                KSTU_SALESMAN_MASTER salesmanMaster = db.KSTU_SALESMAN_MASTER.Where(sal => sal.sal_code == saleCode && sal.company_code == companyCode && sal.branch_code == branchCode).FirstOrDefault();
                string salesman = salesmanMaster == null ? "" : salesmanMaster.sal_code + "-" + salesmanMaster.sal_name;
                if (TotalAmt > 0) {
                    if (RoundOffAmt != 0) {
                        if (RoundOffAmt >= 0) {
                            sb.AppendLine(string.Format("{0,-12}{1,-17}{2,-15}{3,15}{4,12}"
                                 , "", ""
                                   , "", "Amount Receivable(Round.Off " + RoundOffAmt + "Ps.) :"
                                   , TotalAmt.ToString("F")));
                            sb.AppendLine(string.Format("{0,-12}{1,-17}{2,-15}{3,15}{4,12}"
                                 , salesEstMaster.est_date.ToString()
                                   , salesman
                                   , salesEstMaster.operator_code.ToString()
                                   , ""
                                   , ""));
                        }
                        else {
                            sb.AppendLine(string.Format("{0,-12}{1,-17}{2,-15}{3,15}{4,12}"
                                , "", ""
                                , "", "Amount Receivable(Round.Off " + RoundOffAmt + "Ps.) :"
                                , TotalAmt.ToString("F")));
                            sb.AppendLine(string.Format("{0,-12}{1,-17}{2,-15}{3,15}{4,12}"
                               , salesEstMaster.est_date.ToString()
                               , salesman
                               , salesEstMaster.operator_code.ToString()
                               , ""
                               , ""));
                        }
                    }
                    else {
                        sb.AppendLine(string.Format("{0,-12}{1,-17}{2,-15}{3,15}{4,12}",
                              "", ""
                            , ""
                            , "                    Amount Receivable :"
                            , TotalAmt.ToString("F")));

                        sb.AppendLine(string.Format("{0,-12}{1,-17}{2,-15}{3,15}{4,12}",
                             salesEstMaster.est_date.ToString()
                              , salesman
                           , salesEstMaster.operator_code.ToString()
                           , "  "
                           , ""));
                    }
                }
                else {
                    if (RoundOffAmt != 0) {
                        if (RoundOffAmt >= 0) {
                            sb.AppendLine(string.Format("{0,-12}{1,-19}{2,-17}{3,15}{4,12}"
                                , ""
                                , ""
                                , ""
                                , "Amount Payable(Round.Off" + RoundOffAmt + "Ps.) :"
                                , TotalAmt.ToString("F")));

                            sb.AppendLine(string.Format("{0,-12}{1,-19}{2,-17}{3,15}{4,12}"
                               , salesEstMaster.est_date.ToString()
                               , salesman
                               , salesEstMaster.operator_code.ToString()
                               , ""
                               , ""));
                        }
                        else {
                            sb.AppendLine(string.Format("{0,-12}{1,-19}{2,-17}{3,15}{4,12}"
                                , ""
                               , ""
                                , ""
                                , "Amount Payable(Round.Off" + RoundOffAmt + "Ps.) :"
                                , TotalAmt.ToString("F")));
                            sb.AppendLine(string.Format("{0,-12}{1,-19}{2,-17}{3,15}{4,12}"
                               , salesEstMaster.est_date.ToString()
                              , salesman
                               , salesEstMaster.operator_code.ToString()
                               , ""
                               , ""));
                        }
                    }
                    else {
                        sb.AppendLine(string.Format("{0,-12}{1,-19}{2,-17}{3,15}{4,12}"
                            , ""
                            , ""
                            , ""
                            , "Amount Payable :"
                            , TotalAmt.ToString("F")));

                        sb.AppendLine(string.Format("{0,-12}{1,-19}{2,-17}{3,15}{4,12}"
                           , salesEstMaster.est_date.ToString()
                           , salesman
                           , salesEstMaster.operator_code.ToString()
                           , ""
                           , ""));
                    }
                }
                sb.AppendLine();

                //string query = string.Format("select est_Srno,qty,type,name,carrat,stone_wt,rate,amount \n"
                //                           + "from KTTU_SALES_STONE_DETAILS where est_no = {0} and company_code = '{1}' and branch_code = '{2}' \n"
                //                           + "order by est_Srno"
                //                           , estNo, companyCode, branchCode);
                //DataTable dtStoneDetails = SIGlobals.Globals.ExecuteQuery(query);

                List<KTTU_SALES_STONE_DETAILS> salesStoneDet = db.KTTU_SALES_STONE_DETAILS.Where(stone => stone.est_no == estNo && stone.company_code == companyCode && stone.branch_code == branchCode).OrderBy(st => st.est_Srno).ToList();

                if (salesStoneDet != null && salesStoneDet.Count > 0) {
                    List<string> Stonelst = null;
                    string stone = string.Empty;
                    for (int j = 0; j < salesStoneDet.Count; j++) {
                        string StoneName = salesStoneDet[j].name.ToString();
                        StoneName = StoneName.Trim();
                        string[] arr = StoneName.Split('-');
                        if (j == 0 || (!salesStoneDet[j].est_Srno.Equals(salesStoneDet[j - 1].est_Srno))) {
                            stone += string.Format("{0}{1}{2}{3}{4}{5}{6} ", salesStoneDet[j].est_Srno, ")", arr[0], "\\", salesStoneDet[j].rate, "*", salesStoneDet[j].carrat);
                        }
                        else {
                            stone += string.Format("{0}{1}{2}{3}{4} ", arr[0], "\\", salesStoneDet[j].rate, "*", salesStoneDet[j].carrat);
                        }
                    }
                    sb.Append(esc + Convert.ToChar(64).ToString());
                    sb.Append(esc + Convert.ToChar(80).ToString() + Convert.ToChar(15).ToString());
                    if (stone.Length <= 84)
                        sb.AppendLine(string.Format("{0,-136}", stone));
                    else {
                        Stonelst = SIGlobals.Globals.SplitStringAt(84, stone);
                        sb.AppendLine(string.Format("{0,-136}", Stonelst[0]));
                    }
                    if (Stonelst != null && Stonelst.Count > 1) {
                        for (int x = 1; x < Stonelst.Count; x++)
                            sb.AppendLine(string.Format("{0,2}{1,-134}", "- ", Stonelst[x].Trim()));

                        stone = string.Empty;
                        Stonelst = null;
                    }
                    sb.AppendLine();

                    //string query1 = string.Format("select ISNULL(sum(carrat),0) as carrat from KTTU_SALES_STONE_DETAILS where type like 'D%' and est_no={0} and company_code='{1}' and branch_Code='{2}'", estNo, companyCode, branchCode);
                    //DataTable dttotalcarats = SIGlobals.Globals.ExecuteQuery(query1);
                    //decimal carrat = Convert.ToDecimal(dttotalcarats.Rows[0]["carrat"]);

                    decimal carrat = Convert.ToDecimal(salesStoneDet.Where(stn => stn.type.Contains("D")).Sum(stn => stn.carrat));
                    if (carrat > 0) {
                        sb.AppendLine(string.Format("{0, 1}{1,-136}", "Total Diamond Cts=", carrat));

                        sb.AppendLine();
                    }
                    sb.Append(Convert.ToChar(18).ToString() + esc + Convert.ToChar(77).ToString());
                }
                sb.AppendLine();
                sb.AppendLine(string.Format("{0,-8}{1,27}{2,10}{3,-8}{4,27}", "Name", ":------------------------------", "", "Email Id", ":------------------------------"));
                sb.AppendLine();
                sb.AppendLine(string.Format("{0,-8}{1,27}{2,10}{3,-8}{4,27}", "Address", ":------------------------------", "", "D.O.B", ":------------------------------"));
                sb.AppendLine();
                sb.AppendLine(string.Format("{0,-8}{1,27}{2,10}{3,-8}{4,27}", "", " ------------------------------", "", "Phone No", ":------------------------------"));
                sb.AppendLine();
                sb.AppendLine(string.Format("{0,-8}{1,27}{2,10}{3,-8}{4,27}", "", " ------------------------------", "", "PAN", ":------------------------------"));
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine();
                return sb.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        public string Estimation60ColumnThermalPrint(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            StringBuilder sbStart = new StringBuilder();
            StringBuilder strTransLine = new StringBuilder();
            strTransLine.Append('-', 60);
            decimal Spaces = 0M;
            StringBuilder strSpaces = new StringBuilder();
            Spaces = ((60 - strTransLine.Length) / 3);
            strSpaces.Append(' ', Convert.ToInt32(Spaces));
            string esc = Convert.ToChar(27).ToString();
            string strEst = string.Empty;

            try {
                KTTU_SALES_EST_MASTER master = db.KTTU_SALES_EST_MASTER.Where(s => s.company_code == companyCode
                                                                                && s.branch_code == branchCode
                                                                                && s.est_no == estNo).FirstOrDefault();
                if (master == null) {
                    error = new ErrorVM()
                    {
                        description = "Estimation Number does not Exists.",
                        ErrorStatusCode = HttpStatusCode.NotFound
                    };
                    return "";
                }

                sbStart.AppendLine("<html>");
                sbStart.AppendLine("<head>");
                sbStart.AppendLine(SIGlobals.Globals.GetStyleNewforBH());
                sbStart.AppendLine("</head>");
                sbStart.AppendLine("<body>");

                List<KTTU_SALES_EST_DETAILS> details = db.KTTU_SALES_EST_DETAILS.Where(d => d.company_code == companyCode
                                                                                        && d.branch_code == branchCode
                                                                                        && d.est_no == estNo).ToList();
                if (details == null || details.Count == 0) {
                    error = new ErrorVM()
                    {
                        description = "Estimation Details Not found.",
                        ErrorStatusCode = HttpStatusCode.NotFound
                    };
                    return "";
                }
                string metalType = SIGlobals.Globals.GetMetalType(db, details[0].gs_code, companyCode, branchCode);

                int Pcs = 0;
                decimal RoundGoldValue = 0;
                decimal RoundMCAmount = 0;
                decimal RoundStoneAmt = 0;
                decimal RoundItemGrosAmount = 0;
                decimal RoundInvoiceAmount = 0;
                decimal RoundtotalTaxableAmount = 0;
                decimal RounddicountMc = 0;
                decimal RoundTotalGoldValue = 0;
                decimal RoundValueAmt = 0;
                decimal RoundStoneChg = 0;
                decimal RoundtotalGrosAmount = 0;

                decimal Gwt = 0;
                decimal Nwt = 0;
                decimal GoldValue = 0;
                decimal StoneChg = 0;
                decimal ValueAmt = 0;
                decimal VAPercent = 0;
                decimal RoundOffAmt = 0;
                decimal Wst = 0;
                decimal dicountMc = 0, totalTaxableAmount = 0;
                string ItemName = string.Empty;
                decimal ItemAmt = 0;
                decimal VaAmt = 0;
                decimal discountVA = 0;
                decimal totalMC = 0;
                decimal totalJvalue = 0;

                sbStart.AppendLine("<Table  font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse;\" width=\"250\">");
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD  style=\"border-right:thin; border-bottom:thin; border-left:thin\" colspan=9 ALIGN = \"CENTER\"><b>SALES ESTIMATION <br></b></TD>"));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TH  style=\"border-top:0; border-right:thin; border-bottom:thin solid; border-left:thin solid\" colspan=6 ALIGN = \"LEFT\"><b>Name: {0}<br></b></TH>", master.Cust_Name));
                sbStart.AppendLine(string.Format("<TH  style=\"border-top:0; border-right:thin; border-bottom:thin solid; border-left:thin\" colspan=1 ><b><br></b></TH>"));
                sbStart.AppendLine(string.Format("<TH  style=\"border-top:0; border-right:thin; border-bottom:thin solid; border-left:thin\" colspan=2 ALIGN = \"RIGHT\"><b><br></b></TH>"));
                sbStart.AppendLine("</TR>");

                var rateMaster = db.KSTU_RATE_MASTER.Where(rate => rate.company_code == companyCode && rate.branch_code == branchCode && rate.bill_type == "S");
                if (rateMaster == null && rateMaster.Count() == 0) {
                    error = new ErrorVM()
                    {
                        description = "Rate Details Not found.",
                        ErrorStatusCode = HttpStatusCode.NotFound
                    };
                    return "";
                }
                if ((string.Compare(metalType, "GL") == 0)) {
                    var rate18k = rateMaster.Where(r => r.karat == "18K").FirstOrDefault();
                    if (rate18k != null) {
                        sbStart.AppendLine(string.Format("<TH  style=\"border-top:0; border-right:thin; border-bottom:thin solid; border-left:thin\" colspan=3 ALIGN = \"CENTER\"><b>{0}  Rate: {1} <br></b></TH>", rate18k.karat, rate18k.rate));
                    }
                    else {
                        sbStart.AppendLine(string.Format("<TH  style=\"border-top:0; border-right:thin; border-bottom:thin solid; border-left:thin\" colspan=3 ALIGN = \"CENTER\"><b>{0}<br></b></TH>", "&nbsp"));
                    }

                    var rate22k = rateMaster.Where(r => r.karat == "22K").FirstOrDefault();
                    if (rate22k != null) {
                        sbStart.AppendLine(string.Format("<TH  style=\"border-top:0; border-right:thin; border-bottom:thin solid; border-left:thin\" colspan=3 ALIGN = \"CENTER\"><b>{0}  Rate: {1} <br></b></TH>", rate22k.karat, rate22k.rate));
                    }
                    else {
                        sbStart.AppendLine(string.Format("<TH  style=\"border-top:0; border-right:thin; border-bottom:thin solid; border-left:thin\" colspan=3 ALIGN = \"CENTER\"><b>{0}<br></b></TH>", "&nbsp"));
                    }
                    var rate24k = rateMaster.Where(r => r.karat == "24K").FirstOrDefault();
                    if (rate24k != null) {
                        sbStart.AppendLine(string.Format("<TH  style=\"border-top:0; border-right:thin; border-bottom:thin solid; border-left:thin\" colspan=3 ALIGN = \"CENTER\"><b>{0}  Rate: {1} <br></b></TH>", rate24k.karat, rate24k.rate));
                    }
                    else {
                        sbStart.AppendLine(string.Format("<TH  style=\"border-top:0; border-right:thin solid; border-bottom:thin solid; border-left:thin\" colspan=3 ALIGN = \"CENTER\"><b>{0}<br></b></TH>", "&nbsp"));
                    }
                }

                sbStart.AppendLine("<TR bgcolor= WHITE style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sbStart.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b>Product</b></TH>");
                sbStart.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                sbStart.AppendLine("<TH colspan = 3 ALIGN = \"CENTER\"><b>[Weight in Grams]</b></TH>");
                sbStart.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b>Jewel</b></TH>");
                sbStart.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b>VA</b></TH>");
                sbStart.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b>St/Dmd</b></TH>");
                sbStart.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b>Gross</b></TH>");
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<TR bgcolor=WHITE style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid ; border-top:thin \" ALIGN = \"CENTER\"><b></b></TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b>Qty</b></TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid\" ALIGN = \"CENTER\"><b>Gr.Wt</b></TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid\" ALIGN = \"CENTER\"><b>Net Wt</b></TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid\" ALIGN = \"CENTER\"><b>Wst.Wt</b></TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b>Amount</b></TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\"><b>Amount</b></TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid ; border-top:thin\" ALIGN = \"CENTER\"><b>Amount</b></TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid ; border-top:thin\" ALIGN = \"CENTER\"><b>Amount</b></TH>");
                sbStart.AppendLine("</TR>");

                decimal InvoiceAmount = 0;
                string VA = string.Empty;
                decimal TotalSGST = 0;
                decimal TotalCGST = 0;
                decimal TotalIGST = 0;
                decimal totalGrosAmount = 0;
                decimal ItemGrosAmount = 0;
                decimal offerDiscountAmount = 0;
                int MaxPageRow = 3;

                for (int i = 0; i < details.Count; i++) {
                    decimal GANew = 0;
                    discountVA = 0;
                    bool IsPiese = false;
                    decimal MCAmount = 0;

                    string IsPieceItem = SIGlobals.Globals.IsPeiceItem(db, details[i].gs_code, details[i].item_name, companyCode, branchCode);
                    decimal pieceamount = Convert.ToDecimal(details[i].mc_per_piece);

                    if (pieceamount > 0) {
                        IsPiese = true;
                    }
                    if (Convert.ToString(details[i].mc_type) == "5" || Convert.ToString(details[i].mc_type) == "4" || Convert.ToString(details[i].mc_type) == "1") {
                        decimal VAm = Convert.ToDecimal(details[i].va_amount);
                        decimal GA = Convert.ToDecimal(details[i].gold_value);

                        GANew = Convert.ToDecimal(details[i].gold_value) + Convert.ToDecimal(details[i].stone_charges) + Convert.ToDecimal(details[i].diamond_charges);
                        if (Convert.ToString(details[i].mc_type) == "5") {
                            VaAmt = (GA * Convert.ToDecimal(details[i].mc_percent)) / 100;
                        }

                        if (Convert.ToString(details[i].mc_type) == "4" || Convert.ToString(details[i].mc_type) == "1") {
                            VaAmt = VAm;
                        }
                        if (VAm > 0) {

                            decimal VA_Percent = decimal.Round((VAm / GA) * 100, 2);
                            VAPercent = decimal.Round(VA_Percent, 2);
                        }
                        else {
                            decimal VA_Percent = 0.00M;
                            VAPercent = decimal.Round(VA_Percent, 2);
                        }
                    }
                    else {
                        decimal VA_Percent = 0.00M;
                        VAPercent = decimal.Round(VA_Percent, 2);

                    }
                    string VAp = string.Empty;
                    if (Convert.ToDecimal(details[i].mc_percent) > 0) {
                        VAp = Convert.ToDecimal(details[i].mc_percent).ToString().Replace(".", "#");
                    }
                    else {
                        VAp = VAPercent.ToString().Replace(".", "#");
                    }

                    ItemName = details[i].item_name;
                    if (Convert.ToDecimal(details[i].making_charge_per_rs) > 0) {
                        MCAmount = Convert.ToDecimal(details[i].nwt) * Convert.ToDecimal(details[i].making_charge_per_rs);
                    }
                    else if (Convert.ToDecimal(details[i].mc_percent) > 0) {
                        MCAmount = (Convert.ToDecimal(details[i].gold_value) * Convert.ToDecimal(details[i].mc_percent)) / 100;
                    }
                    else {
                        MCAmount = Convert.ToDecimal(details[i].mc_amount);
                    }
                    MCAmount = decimal.Round(MCAmount, 2);
                    RoundMCAmount = Math.Round(MCAmount, 2);
                    decimal goldValue = Convert.ToDecimal(details[i].gold_value);
                    ItemAmt = Convert.ToDecimal(details[i].total_amount);


                    if (IsPieceItem.Equals("Y")) {
                        goldValue = 0;
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  ALIGN = \"RIGHT\"><b>{0}</b></TD>", goldValue));
                        GoldValue += goldValue;
                        RoundGoldValue = Math.Round(GoldValue, 2);
                    }

                    if (Convert.ToString(details[i].mc_type) == "0" || Convert.ToString(details[i].mc_type) == "6") {
                        ItemAmt = Convert.ToDecimal(details[i].total_amount);
                    }
                    else {


                        decimal wastAmt = 0;
                        decimal wastGrams = Convert.ToDecimal(details[i].wastage_grms);
                        if (wastGrams > 0) {
                            wastAmt = decimal.Round((wastGrams * Convert.ToDecimal(details[i].rate)), 2);
                            goldValue += wastAmt;

                        }
                        GoldValue += goldValue;
                        RoundGoldValue = Math.Round(goldValue, 2);

                    }

                    if (Convert.ToDecimal(details[i].va_amount) > 0) {
                        VAPercent = (Convert.ToDecimal(details[i].va_amount) * 100) / Convert.ToDecimal(details[i].gold_value);
                        VAPercent = decimal.Round(VAPercent, 2);
                    }
                    else {
                        VAPercent = 0.00M;
                    }
                    VA = SIGlobals.Globals.GetVAcode(VAPercent);
                    int qty = Convert.ToInt32(details[i].item_no);

                    int pcs = Convert.ToInt32(details[i].item_no) + Convert.ToInt32(details[i].Addqty) - Convert.ToInt32(details[i].Deductqty);
                    decimal gwt = (Convert.ToDecimal(details[i].gwt) + Convert.ToDecimal(details[i].AddWt) - Convert.ToDecimal(details[i].DeductWt));
                    decimal nwt = Convert.ToDecimal(details[i].nwt) + Convert.ToDecimal(details[i].AddWt) - Convert.ToDecimal(details[i].DeductWt);
                    sbStart.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"LEFT\"><b>{0}</b></TD>", ItemName));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"LEFT\"><b>{0}</b></TD>", pcs));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", gwt));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", nwt));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", details[i].wastage_grms));
                    decimal vaAmount = Convert.ToDecimal(details[i].va_amount);
                    if (IsPieceItem.Equals("Y")) {
                        decimal goldValue1 = 0;
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  ALIGN = \"RIGHT\"><b>{0}</b></TD>", Math.Round(RoundGoldValue)));
                        GoldValue += goldValue1;
                    }
                    else {
                        decimal wastAmt = 0;
                        decimal wastGrams = Convert.ToDecimal(details[i].wastage_grms);
                        if (wastGrams > 0) {
                            wastAmt = decimal.Round((wastGrams * Convert.ToDecimal(details[i].rate)), 2);
                            goldValue += wastAmt;
                            vaAmount -= wastAmt;
                        }
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", Math.Round(RoundGoldValue))); // dtEstDetails.Rows[i]["gold_value"],
                    }

                    decimal StoneAmt = Convert.ToDecimal(details[i].stone_charges)
                        + Convert.ToDecimal(details[i].diamond_charges)
                        + Convert.ToDecimal(details[i].hallmarcharges);

                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", Math.Round(RoundMCAmount)));
                    RoundStoneAmt = Math.Round(StoneAmt, 2);

                    if (RoundStoneAmt > 0)
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", Math.Round(RoundStoneAmt)));
                    else
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>", "0.00", "&nbsp"));

                    Gwt += Convert.ToDecimal(gwt);
                    Nwt += Convert.ToDecimal(nwt);
                    Wst += Convert.ToDecimal(details[i].wastage_grms);
                    dicountMc += Convert.ToDecimal(details[i].Mc_Discount_Amt);

                    StoneChg += StoneAmt;
                    ValueAmt += MCAmount;
                    if (IsPieceItem.Equals("Y"))
                        ItemGrosAmount = RoundGoldValue + MCAmount;
                    else

                        ItemGrosAmount = RoundGoldValue + StoneAmt + MCAmount;

                    RoundItemGrosAmount = Math.Round(ItemGrosAmount, 2);
                    totalGrosAmount += RoundItemGrosAmount;
                    offerDiscountAmount += Convert.ToDecimal(details[i].offer_value);
                    totalTaxableAmount += Convert.ToDecimal(details[i].item_total_after_discount);
                    TotalSGST += Convert.ToDecimal(details[i].SGST_Amount);
                    TotalCGST += Convert.ToDecimal(details[i].CGST_Amount);
                    TotalIGST += Convert.ToDecimal(details[i].IGST_Amount);
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", Math.Round(RoundItemGrosAmount)));
                    Pcs += Convert.ToInt32(details[i].item_no);
                    InvoiceAmount += Convert.ToDecimal(details[i].item_total_after_discount);
                    sbStart.AppendLine("</TR>");
                }

                for (int j = 0; j < MaxPageRow; j++) {
                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\"  ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine("</TR>");
                }
                sbStart.AppendLine("<TR  bgcolor=WHITE  style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sbStart.AppendLine("<TH  style=\"border-top:thin solid; border-bottom:thin solid\" colspan=1 ALIGN = \"LEFT\"><b>Totals</b></TH>");

                sbStart.AppendLine(string.Format("<TH style=\"border-top:thin solid; border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}</b></TH>", Pcs));
                sbStart.AppendLine(string.Format("<TH style=\"border-top:thin solid; border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}</b></TH>", Gwt));
                sbStart.AppendLine(string.Format("<TH style=\"border-top:thin solid; border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}</b></TH>", Nwt));
                sbStart.AppendLine(string.Format("<TH style=\"border-top:thin solid; border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}</b></TH>", Wst));
                RoundTotalGoldValue = Math.Round(GoldValue, 2);
                RoundValueAmt = Math.Round(ValueAmt, 2);
                RoundStoneChg = Math.Round(StoneChg, 2);
                RoundtotalGrosAmount = Math.Round(totalGrosAmount, 2);
                sbStart.AppendLine(string.Format("<TH style=\"border-top:thin solid; border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}</b></TH>", Math.Round(RoundTotalGoldValue)));
                sbStart.AppendLine(string.Format("<TH style=\"border-top:thin solid; border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}</b></TH>", Math.Round(RoundValueAmt)));
                sbStart.AppendLine(string.Format("<TH style=\"border-top:thin solid; border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}</b></TH>", Math.Round(RoundStoneChg)));
                sbStart.AppendLine(string.Format("<TH style=\"border-top:thin solid; border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}</b></TH>", Math.Round(RoundtotalGrosAmount)));
                sbStart.AppendLine("</TR>");

                List<KTTU_SALES_STONE_DETAILS> salesStoneDet = db.KTTU_SALES_STONE_DETAILS.Where(stone => stone.company_code == companyCode
                                                                                                && stone.branch_code == branchCode
                                                                                                && stone.est_no == estNo).ToList();

                if (salesStoneDet != null && salesStoneDet.Count > 0) {
                    List<string> Stonelst = null;
                    string stone = string.Empty;
                    for (int j = 0; j < salesStoneDet.Count; j++) {
                        string StoneName = salesStoneDet[j].name;
                        StoneName = StoneName.Trim();
                        string[] arr = StoneName.Split('-');

                        if (j == 0 || (!salesStoneDet[j].est_Srno.Equals(salesStoneDet[j - 1].est_Srno))) {
                            stone += string.Format("{0}{1}{2}{3}{4}{5}{6} ", salesStoneDet[j].est_Srno, ")", arr[0], "\\", salesStoneDet[j].rate, "*", salesStoneDet[j].carrat);
                        }
                        else {
                            stone += string.Format("{0}{1}{2}{3}{4} ", arr[0], "\\", salesStoneDet[j].rate, "*", salesStoneDet[j].carrat);
                        }
                    }

                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=8 ALIGN = \"LEFT\"><b>{0}{1}</b></TD><TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"LEFT\"><b>{1}</b></TD>", stone, "&nbsp"));
                    sbStart.AppendLine("</TR>");

                    if (Stonelst != null && Stonelst.Count > 1) {
                        for (int x = 1; x < Stonelst.Count; x++) {
                            sbStart.AppendLine("<TR>");
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=8 ALIGN = \"LEFT\"><b>{0}{1}</b></TD> <TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"LEFT\"><b></b></TD><TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"LEFT\"><b></b></TD>", "- ", Stonelst[x].Trim()));
                            sbStart.AppendLine("</TR>");
                        }

                        stone = string.Empty;
                        Stonelst = null;
                    }
                }
                decimal carrat = salesStoneDet.Where(st => st.type.StartsWith("D")).Sum(st => st.carrat);
                if (carrat > 0) {
                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  colspan = 8 ALIGN = \"LEFT\"><b>Total Diamond Cts= {0}</b></TD><TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"LEFT\"><b></b></TD>", carrat));
                    sbStart.AppendLine("</TR>");
                }

                dicountMc += Convert.ToDecimal(master.discount_amount);
                if (Convert.ToDecimal(dicountMc) > 0) {
                    RounddicountMc = Math.Round(dicountMc, 2);
                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=8 ALIGN = \"RIGHT\"><b>Discount</b></TD>"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", Math.Round(RounddicountMc)));
                    sbStart.AppendLine("</TR>");
                }


                if (Convert.ToDecimal(master.excise_duty_amount) > 0) {
                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=8 ALIGN = \"RIGHT\"><b>Excise Duty @ {0}%</b></TD>", master.excise_duty_amount));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", master.excise_duty_amount));
                    InvoiceAmount += Convert.ToDecimal(master.excise_duty_amount);
                }
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  colspan = 8 ALIGN = \"RIGHT\"><b>Taxable Value</b></TD>"));
                RoundtotalTaxableAmount = Math.Round(totalTaxableAmount, 2);
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  ALIGN = \"RIGHT\"><b>{0}</b></TD>", Math.Round(RoundtotalTaxableAmount)));
                sbStart.AppendLine("</TR>");

                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-top:thin\" colspan = 8 ALIGN = \"RIGHT\"><b>SGST {0}%</b></TD>", details[0].SGST_Percent));
                decimal RoundTotalSGST = Math.Round(TotalSGST, 2);
                sbStart.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-top:thin\" ALIGN = \"RIGHT\"><b>{0}</b></TD>", Math.Round(RoundTotalSGST)));
                sbStart.AppendLine("</TR>");

                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-top:thin\" colspan = 8 ALIGN = \"RIGHT\"><b>CGST {0}% </b></TD>", details[0].CGST_Percent));
                decimal RoundTotalCGST = Math.Round(TotalCGST, 2);
                sbStart.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-top:thin\" ALIGN = \"RIGHT\"><b>{0}</b></TD>", Math.Round(RoundTotalCGST)));
                sbStart.AppendLine("</TR>");

                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-top:thin\" colspan = 8 ALIGN = \"RIGHT\"><b>IGST {0}%</b></TD>", details[0].IGST_Percent));
                decimal RoundTotalIGST = Math.Round(TotalIGST, 2);
                sbStart.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-top:thin\" ALIGN = \"RIGHT\"><b>{0}</b></TD>", Math.Round(TotalIGST)));
                sbStart.AppendLine("</TR>");

                InvoiceAmount += TotalIGST + TotalSGST + TotalCGST;
                decimal InvoiceAmountInWords = InvoiceAmount;
                RoundInvoiceAmount = Math.Round(InvoiceAmount);
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=8 ALIGN = \"RIGHT\"><b>Sales Amount</b></TD>"));
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", RoundInvoiceAmount));
                sbStart.AppendLine("</TR>");


                if (Convert.ToDecimal(master.purchase_amount) > 0) {

                    KTTU_PURCHASE_EST_MASTER purchase = db.KTTU_PURCHASE_EST_MASTER.Where(p => p.company_code == companyCode && p.branch_code == branchCode && p.est_no == estNo).FirstOrDefault();
                    decimal NewPurAmount = purchase == null ? 0 : Convert.ToDecimal(purchase.grand_total);
                    decimal RoundNewPurAmount = 0;
                    RoundNewPurAmount = Math.Round(NewPurAmount, 2);
                    if (NewPurAmount > 0) {
                        sbStart.AppendLine("</TR>");
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=8 ALIGN = \"RIGHT\"><b>Less Purchase (As Per Est No : {0})</b></TD>", estNo));
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", Math.Round(RoundNewPurAmount)));
                        sbStart.AppendLine("</TR>");
                        RoundInvoiceAmount -= RoundNewPurAmount;
                    }

                    var existingPurchase = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.company_code == companyCode
                                                                            && pay.branch_code == branchCode
                                                                            && pay.trans_type == "A"
                                                                            && pay.pay_mode == "PE" && pay.series_no == estNo).ToList();
                    if (existingPurchase != null && existingPurchase.Count > 0) {
                        for (int i = 0; i < existingPurchase.Count; i++) {
                            sbStart.AppendLine("</TR>");
                            decimal OtherPUrAmt = Convert.ToDecimal(existingPurchase[i].pay_amt);
                            decimal RoundOtherPUrAmt = Math.Round(OtherPUrAmt, 2);
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=8 ALIGN = \"RIGHT\"><b>Less Purchase (As Per Est No : {0})</b></TD>", existingPurchase[i].Ref_BillNo));
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", Math.Round(RoundOtherPUrAmt)));
                            sbStart.AppendLine("</TR>");
                            RoundInvoiceAmount -= RoundOtherPUrAmt;
                        }
                    }
                }
                var existingSR = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.company_code == companyCode
                                                                             && pay.branch_code == branchCode
                                                                             && pay.trans_type == "A"
                                                                             && pay.pay_mode == "SE" && pay.series_no == estNo).ToList();
                if (existingSR != null && existingSR.Count > 0) {
                    for (int i = 0; i < existingSR.Count; i++) {
                        decimal SR_Amt = Convert.ToDecimal(existingSR[i].pay_amt);
                        decimal RoundSR_Amt = Math.Round(SR_Amt, 2);
                        sbStart.AppendLine("</TR>");
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=8 ALIGN = \"RIGHT\"><b>Less SR (As Per Est No: {0}) </b></TD>", existingSR[i].Ref_BillNo));
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", Math.Round(RoundSR_Amt)));
                        sbStart.AppendLine("</TR>");
                        RoundInvoiceAmount -= RoundSR_Amt;
                    }
                }

                if (Convert.ToDecimal(master.order_amount) > 0) {
                    decimal Ord_Amt = Convert.ToDecimal(master.order_amount);
                    decimal RoundOrd_Amt = Math.Round(Ord_Amt, 2);
                    sbStart.AppendLine("</TR>");
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=8 ALIGN = \"RIGHT\"><b>Less Advance (As Per Order No: {0} Dated : {1}) </b></TD>", master.order_no, master.order_date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", Math.Round(RoundOrd_Amt)));
                    sbStart.AppendLine("</TR>");
                    RoundInvoiceAmount -= RoundOrd_Amt;
                }
                var attchedOrder = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.company_code == companyCode
                                                                             && pay.branch_code == branchCode
                                                                             && pay.trans_type == "A"
                                                                             && pay.pay_mode == "OP" && pay.series_no == estNo).ToList();

                if (attchedOrder != null && attchedOrder.Count > 0) {
                    for (int i = 0; i < attchedOrder.Count; i++) {
                        decimal MOrd_amt = Convert.ToDecimal(attchedOrder[i].pay_amt);
                        decimal RoundMOrd_amt = Math.Round(MOrd_amt, 2);

                        sbStart.AppendLine("</TR>");
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=8 ALIGN = \"RIGHT\"><b>Less Advance (As Per Order No: {0}) </b></TD>", attchedOrder[i].Ref_BillNo));
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", Math.Round(RoundMOrd_amt)));
                        sbStart.AppendLine("</TR>");
                        RoundInvoiceAmount -= RoundMOrd_amt;
                    }
                }

                InvoiceAmount = Math.Round(InvoiceAmount, 0, MidpointRounding.ToEven);
                if (RoundInvoiceAmount > 0) {
                    if (RoundOffAmt != 0) {
                        if (RoundOffAmt >= 0) {
                            sbStart.AppendLine("</TR>");
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin solid; border-top:thin \" colspan=8 ALIGN = \"RIGHT\"><b>Amount Receivable(Round.Off {0}Ps.) </b></TD>", RoundOffAmt));
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin solid; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", RoundInvoiceAmount));
                            sbStart.AppendLine("</TR>");

                        }
                        else {
                            sbStart.AppendLine("</TR>");
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin solid; border-top:thin \" colspan=8 ALIGN = \"RIGHT\"><b>Amount Receivable(Round.Off {0}Ps.) </b></TD>", RoundOffAmt));
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin solid; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", RoundInvoiceAmount));
                            sbStart.AppendLine("</TR>");

                        }
                    }
                    else {
                        sbStart.AppendLine("</TR>");
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin solid; border-top:thin \" colspan=8 ALIGN = \"RIGHT\"><b>Amount Receivable</b></TD>"));
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin solid; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", RoundInvoiceAmount));
                        sbStart.AppendLine("</TR>");
                    }
                }
                else {
                    if (RoundOffAmt != 0) {
                        if (RoundOffAmt >= 0) {
                            sbStart.AppendLine("</TR>");
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin solid; border-top:thin \" colspan=8 ALIGN = \"RIGHT\"><b>Amount Payable(Round.Off {0}Ps.) </b></TD>", RoundOffAmt));
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin solid; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", RoundInvoiceAmount));
                            sbStart.AppendLine("</TR>");
                        }
                        else {
                            sbStart.AppendLine("</TR>");
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin solid; border-top:thin \" colspan=8 ALIGN = \"RIGHT\"><b>Amount Payable(Round.Off {0}Ps.) </b></TD>", RoundOffAmt));
                            sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin solid; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", RoundInvoiceAmount));
                            sbStart.AppendLine("</TR>");
                        }
                    }
                    else {
                        sbStart.AppendLine("</TR>");
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin solid; border-top:thin \" colspan=8 ALIGN = \"RIGHT\"><b>Amount Payable</b></TD>"));
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin solid; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", RoundInvoiceAmount));
                        sbStart.AppendLine("</TR>");
                    }
                }

                string strWords = string.Empty; ;
                NumberToWords objNWClass = new NumberToWords();
                objNWClass.ConvertNumberToWords(Convert.ToDouble(RoundInvoiceAmount), out strWords);
                strWords = strWords.Replace("Rupees", "");
                strWords = "Rupees " + strWords;
                decimal RoundOffAmt1 = RoundOffAmt;
                if (RoundOffAmt != 0) {
                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; \" colspan = 9 ALIGN = \"LEFT\"><b>{0}</b></TD>", strWords));
                    sbStart.AppendLine("</TR>");
                }
                else {
                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; \" colspan = 9 ALIGN = \"LEFT\"><b>{0}</b></TD>", strWords));
                    sbStart.AppendLine("</TR>");
                }

                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin ;   border-top:thin \" colspan=9 ALIGN = \"left\"><b>Est no :{0} </b></TD>", master.est_no));
                sbStart.AppendLine("</TR>");

                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin solid; border-top:thin \" colspan=9 ALIGN = \"left\"><b>OPR :{0} / Sal: {1} / Date: {2} </b></TD>", master.operator_code, details[0].sal_code, master.est_date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("</TABLE>");

                // Purchase Print
                string purchasePrint = new OldGoldPurchaseBL().GetThermalPrint40Column(companyCode, branchCode, estNo, true, out error);
                if (error != null)
                    return "";
                if (purchasePrint != "") {
                    sbStart.AppendLine(purchasePrint);
                }

                // Sales Return Print
                string srPrint = new SalesReturnEstimationBL().GetThermalPrint40Column(companyCode, branchCode, estNo, true, out error);
                if (error != null)
                    return "";
                if (srPrint != "") {
                    sbStart.AppendLine(srPrint);
                }
                sbStart.AppendLine("</body>");
                sbStart.AppendLine("</html>");
                sbStart.AppendLine();
                sbStart.Append(esc + Convert.ToChar(100).ToString() + Convert.ToChar(9).ToString());
                sbStart.AppendLine(esc + Convert.ToChar(105).ToString());
                return sbStart.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        public List<SalesEstMasterVM> GetAllSalesEstimations(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            List<SalesEstMasterVM> lstOfMaster = new List<SalesEstMasterVM>();
            try {
                ErrorVM innerError = new ErrorVM();
                int[] estNo = db.KTTU_SALES_MASTER.Where(sal => sal.company_code == companyCode && sal.branch_code == branchCode).Select(s => s.est_no).ToArray();
                List<KTTU_SALES_EST_MASTER> salesEst = db.KTTU_SALES_EST_MASTER.Where(sales => sales.company_code == companyCode
                                                                                        && sales.branch_code == branchCode
                                                                                        && !estNo.Contains(sales.est_no)).ToList();
                if (salesEst != null && salesEst.Count > 0) {
                    foreach (KTTU_SALES_EST_MASTER master in salesEst) {
                        SalesEstMasterVM tempEst = new SalesEstimationBL().GetEstimationDetails(companyCode, branchCode, master.est_no, out innerError);
                        if (tempEst != null) {
                            lstOfMaster.Add(tempEst);
                        }
                    }
                }

                // Estimations which are already billed and Invoice Cancelled those estimations also allow to merge.
                int[] cancelledInvoiceEst = db.KTTU_SALES_MASTER.Where(sal => sal.company_code == companyCode
                                                                        && sal.branch_code == branchCode
                                                                        && sal.cflag == "Y"
                                                                        && sal.est_no != 0).Select(s => s.est_no).ToArray();

                List<KTTU_SALES_EST_MASTER> salesEstInvoiceDel = db.KTTU_SALES_EST_MASTER.Where(sales => sales.company_code == companyCode
                                                                                                && sales.branch_code == branchCode
                                                                                                && cancelledInvoiceEst.Contains(sales.est_no)).ToList();
                if (salesEstInvoiceDel != null && salesEstInvoiceDel.Count > 0) {
                    foreach (KTTU_SALES_EST_MASTER master in salesEstInvoiceDel) {
                        if (master.est_no != 0) {
                            SalesEstMasterVM tempEst = new SalesEstimationBL().GetEstimationDetails(companyCode, branchCode, master.est_no, out innerError);
                            if (tempEst != null) {
                                lstOfMaster.Add(tempEst);
                            }
                        }
                    }
                }


                return lstOfMaster.OrderBy(est => est.EstNo).ToList();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public int MergeEstimation(MergeEstMaster mergeEstMaster, out ErrorVM error)
        {
            error = null;
            string GlobalGSCode = string.Empty;
            int placeOfSupply = 0;
            try {
                string companyCode = mergeEstMaster.CompanyCode;
                string branchCode = mergeEstMaster.BranchCode;
                string glbMetalType = string.Empty;
                int glbOrderCount = 0;
                //bool isReservedOrderAttached = false;

                #region Validation
                // Checking All The Estimations are Exist and Billed.
                int loopCount = 1;
                foreach (MergeEstDet m in mergeEstMaster.EstDet) {
                    // Checking Estimation Numbers Exist or Not
                    int estNo = m.EstNo;
                    KTTU_SALES_EST_MASTER est = db.KTTU_SALES_EST_MASTER.Where(e => e.company_code == companyCode
                                                                               && e.branch_code == branchCode
                                                                               && e.est_no == estNo).FirstOrDefault();

                    if (est == null) {
                        error = new ErrorVM()
                        {
                            field = "Estimation Number",
                            index = 0,
                            description = string.Format("Invalid Estimation Number {0}", estNo),
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return 0;
                    }
                    else {
                        // Checking Estimation Billed Or Not
                        KTTU_SALES_MASTER sales = db.KTTU_SALES_MASTER.Where(sal => sal.company_code == companyCode
                                                                                && sal.branch_code == branchCode
                                                                                && sal.est_no == estNo).FirstOrDefault();
                        if (sales != null) {
                            error = new ErrorVM()
                            {
                                field = "Estimation Number",
                                index = 0,
                                description = string.Format("Already Bill Generated For the Estimation {0}, Bill No {1}", estNo, sales.bill_no),
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                            };
                            return 0;
                        }
                        else {
                            // Checking barcode Details are Exist or not.
                            foreach (MegeEstLineItem line in m.salesEstimatonVM) {
                                int slno = line.SlNo;
                                string barcodeNo = line.BarcodeNo;
                                KTTU_SALES_EST_DETAILS estDet = db.KTTU_SALES_EST_DETAILS.Where(d => d.company_code == companyCode
                                                                                                && d.branch_code == branchCode
                                                                                                && d.est_no == estNo
                                                                                                && d.sl_no == slno
                                                                                                && d.barcode_no == barcodeNo).FirstOrDefault();
                                if (string.IsNullOrEmpty(glbMetalType)) {
                                    KSTS_GS_ITEM_ENTRY ItemEntry = db.KSTS_GS_ITEM_ENTRY.Where(item => item.company_code == companyCode
                                                                   && item.branch_code == branchCode
                                                                   && item.gs_code == estDet.gs_code
                                                                   && item.bill_type == "S").FirstOrDefault();
                                    glbMetalType = ItemEntry.metal_type;
                                }

                                if (estDet == null) {
                                    error = new ErrorVM()
                                    {
                                        field = "Details not Found",
                                        index = 0,
                                        description = string.Format("Specified Item Details Not found For the Estimation no {0}, Serial No {1} , and Barcode No {2} ", estNo, slno, barcodeNo),
                                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                                    };
                                    return 0;
                                }
                                else {
                                    if (barcodeNo != "") {
                                        string validation = new BarcodeBL().ValidateBarcodeDetails(barcodeNo, companyCode, branchCode);
                                        if (validation != "") {
                                            error = new ErrorVM()
                                            {
                                                field = "Barcode",
                                                index = 0,
                                                description = validation,
                                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                                            };
                                            return 0;
                                        }
                                    }
                                    if (glbMetalType != GetMetalTypeFromGS(line.GSCode, companyCode, branchCode)) {
                                        error = new ErrorVM
                                        {
                                            description = "Different metal cannot be billed together.",
                                            ErrorStatusCode = HttpStatusCode.BadRequest
                                        };
                                        return 0;
                                    }
                                }
                            }
                        }
                    }

                    // Validating Different Type of GST Estimations.
                    if (placeOfSupply == 0) {
                        placeOfSupply = Convert.ToInt32(est.pos);
                    }
                    else if (placeOfSupply != est.pos) {
                        error = new ErrorVM()
                        {
                            field = "GST",
                            index = 0,
                            description = "Cannot merge different type of GST Estimations.",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return 0;
                    }

                    // Checking Global Level Order No is there or not, if its there am incrementing the value. Bellow doing the validation RefNO: 1001
                    if (est.order_no != 0) {
                        glbOrderCount++;
                    }

                    // Checking is there any reserved order attached for this estimation.(Initially told us to block now they informed not to block.)
                    //List<KTTU_PAYMENT_DETAILS> payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.company_code == companyCode
                    //                                                                    && pay.branch_code == branchCode
                    //                                                                    && pay.series_no == estNo
                    //                                                                    && pay.trans_type == "A"
                    //                                                                    && pay.pay_mode == "OP"
                    //                                                                    && pay.cflag == "N").ToList();
                    //int orderNo = 0;
                    //if (payment != null) {
                    //    foreach (KTTU_PAYMENT_DETAILS pay in payment) {
                    //        orderNo = Convert.ToInt32(pay.Ref_BillNo);
                    //        KTTU_ORDER_MASTER order = db.KTTU_ORDER_MASTER.Where(o => o.company_code == companyCode
                    //                                                            && o.branch_code == branchCode
                    //                                                            && o.order_no == orderNo
                    //                                                            && o.cflag == "N").FirstOrDefault();
                    //        if (order.order_type == "R") {
                    //            isReservedOrderAttached = true;
                    //        }
                    //        if (loopCount > 1 && order.order_type == "R" && isReservedOrderAttached == true) {
                    //            error = new ErrorVM()
                    //            {
                    //                field = "GST",
                    //                index = 0,
                    //                description = "Cannot merge different order estimation in a single bill.",
                    //                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    //            };
                    //            return 0;
                    //        }
                    //    }
                    //}
                    loopCount++;
                }

                //RefNo:1001: Global Level Order Adjustment Validation
                if (glbOrderCount == mergeEstMaster.EstDet.Count) {
                    error = new ErrorVM()
                    {
                        field = "GST",
                        index = 0,
                        description = "Cannot merge different order estimation in a single bill.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return 0;
                }
                #endregion

                #region Preparing Estimation
                SalesEstMasterVM finalEstMaster = new SalesEstMasterVM();
                List<SalesEstDetailsVM> finalEstDet = new List<SalesEstDetailsVM>();
                foreach (MergeEstDet d in mergeEstMaster.EstDet) {
                    ErrorVM estError = new ErrorVM();
                    SalesEstMasterVM estMaster = new SalesEstimationBL().GetEstimationDetails(companyCode, branchCode, d.EstNo, out estError);
                    if (string.IsNullOrEmpty(finalEstMaster.CompanyCode)) {
                        finalEstMaster = estMaster;
                    }
                    foreach (MegeEstLineItem line in d.salesEstimatonVM) {
                        SalesEstDetailsVM details = estMaster.salesEstimatonVM.Where(det => det.SlNo == line.SlNo && det.BarcodeNo == line.BarcodeNo).FirstOrDefault();
                        finalEstDet.Add(details);
                    }
                }
                #endregion

                #region Generating Estimation
                ErrorVM errorMerge = new ErrorVM();
                finalEstMaster.salesEstimatonVM = new List<SalesEstDetailsVM>();
                finalEstMaster.salesEstimatonVM = finalEstDet;
                finalEstMaster.IsMergeEst = true;
                // Generates New Estimation Number
                int mergedEstNo = new SalesEstimationBL().SaveEstimation(finalEstMaster, out errorMerge);

                // Generates New Estimation Number with Existing number.
                //int mergedEstNo = new SalesEstimationBL().UpdateEstimation(finalEstMaster.EstNo, finalEstMaster, out errorMerge);
                error = errorMerge;
                return mergedEstNo;
                #endregion
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
            }
            return 0;
        }

        public SalesEstMasterVM GetOfferDiscountCalculationWithBarcode(SalesEstMasterVM saleEst, out ErrorVM error)
        {
            error = null;
            decimal totalTaxAmount = 0;
            decimal totalItemAmount = 0;
            decimal SGST = 0.00M, CGST = 0.00M, IGST = 0.00M, SGST_Per = 0.00M, CGST_Per = 0.00M, IGST_Per = 0.00M, Cess_Per = 0.00M, Cess_amt = 0.00M;
            try {
                List<SalesEstDetailsVM> salCoin = saleEst.salesEstimatonVM.Where(det => det.IsEDApplicable == "O").ToList();
                foreach (SalesEstDetailsVM sede in salCoin) {
                    totalTaxAmount = totalTaxAmount + Convert.ToDecimal(sede.SGSTAmount)
                                                                      + Convert.ToDecimal(sede.CGSTAmount)
                                                                      + Convert.ToDecimal(sede.IGSTAmount);
                    totalItemAmount = Convert.ToDecimal(totalItemAmount + sede.TotalAmount);
                }
                List<SalesEstDetailsVM> salItem = saleEst.salesEstimatonVM.Where(det => det.IsEDApplicable != "O").ToList();
                foreach (SalesEstDetailsVM sede in salItem) {
                    sede.OfferValue = Math.Round(((sede.TotalAmount / totalItemAmount) * Convert.ToDecimal(saleEst.DiscountAmount)), 2, MidpointRounding.ToEven);
                    sede.ItemTotalAfterDiscount = sede.TotalAmount - Convert.ToDecimal(sede.OfferValue);

                    if (sede.BarcodeNo == "" || sede.BarcodeNo == null) {
                        sede.GSTGroupCode = db.usp_GetGSTGoodsGroupCode(sede.CompanyCode, sede.BranchCode, sede.GsCode, sede.ItemName).FirstOrDefault();
                    }
                    GetGSTComponentValues(sede.GSTGroupCode, Convert.ToDecimal(sede.ItemTotalAfterDiscount), sede.isInterstate == 1 ? true : false, out SGST_Per, out SGST, out CGST_Per, out CGST, out IGST_Per, out IGST, out Cess_Per, out Cess_amt, sede.CompanyCode, sede.BranchCode);
                    sede.SGSTAmount = SGST;
                    sede.SGSTPercent = SGST_Per;
                    sede.CGSTAmount = CGST;
                    sede.CGSTPercent = CGST_Per;
                    sede.IGSTAmount = IGST;
                    sede.IGSTPercent = IGST_Per;
                    sede.ItemFinalAmount = Math.Round(Convert.ToDecimal(sede.ItemTotalAfterDiscount + SGST + CGST + IGST), 2, MidpointRounding.ToEven);
                }
            }
            catch (Exception excp) {
                error = new ErrorVM()
                {
                    field = "Internal Server Error",
                    index = 0,
                    description = excp.Message,
                    ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError
                };
            }
            return null;
        }

        public bool GetBarcodeValidation(SalesEstMasterVM saleEst, string barcodeNo, string nonTagGS, out ErrorVM valError)
        {
            valError = null;
            string companyCode = string.Empty;
            string branchCode = string.Empty;
            string metalType = string.Empty;
            string gsCode = string.Empty;

            KTTU_SALES_EST_MASTER saleEstMaster = new KTTU_SALES_EST_MASTER();
            List<SalesEstDetailsVM> salEstVm = saleEst.salesEstimatonVM;
            if (salEstVm == null || salEstVm.Count == 0) {
                // This Valdiation is commented because in the UI Can't handle whether barcodes are there or not. so if sales details are not there am returning ok. 01/10/2020
                //valError = new ErrorVM
                //{
                //    description = "Invalid Sales Details",
                //    ErrorStatusCode = HttpStatusCode.BadRequest
                //};
                //return false;
                return true;
            }

            companyCode = salEstVm[0].CompanyCode;
            branchCode = salEstVm[0].BranchCode;
            gsCode = salEstVm[0].GsCode;
            KSTS_GS_ITEM_ENTRY ItemEntry = db.KSTS_GS_ITEM_ENTRY.Where(item => item.company_code == companyCode
                                                                       && item.branch_code == branchCode
                                                                       && item.gs_code == gsCode
                                                                       && item.bill_type == "S").FirstOrDefault();
            if (string.IsNullOrEmpty(barcodeNo)) {
                if (string.IsNullOrEmpty(nonTagGS)) {
                    valError = new ErrorVM
                    {
                        description = "Invalid barcode number.",
                        ErrorStatusCode = HttpStatusCode.BadRequest
                    };
                    return false;
                }
                else {
                    //Different Metal Validation for Barcoded Item
                    if (ItemEntry.metal_type != GetMetalTypeFromGS(nonTagGS, companyCode, branchCode)) {
                        valError = new ErrorVM
                        {
                            description = "Different metal cannot be billed together.",
                            ErrorStatusCode = HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                }
            }
            else {
                KTTU_BARCODE_MASTER barcode = db.KTTU_BARCODE_MASTER.Where(b => b.company_code == companyCode
                                                                            && b.branch_code == branchCode
                                                                            && b.barcode_no == barcodeNo).FirstOrDefault();
                if (barcode == null) {
                    valError = new ErrorVM
                    {
                        description = "Invalid barcode number.",
                        ErrorStatusCode = HttpStatusCode.BadRequest
                    };
                    return false;
                }
                else if (barcode.sold_flag == "Y") {
                    valError = new ErrorVM
                    {
                        description = "Barcode No already Billed.",
                        ErrorStatusCode = HttpStatusCode.BadRequest
                    };
                    return false;
                }
                else if (salEstVm.Exists(bar => bar.BarcodeNo == barcodeNo)) {
                    valError = new ErrorVM
                    {
                        description = "Barcode number already added.",
                        ErrorStatusCode = HttpStatusCode.BadRequest
                    };
                    return false;
                }
                else {
                    //Different Metal Validation for Barcoded Item
                    if (ItemEntry.metal_type != GetMetalTypeFromGS(barcode.gs_code, companyCode, branchCode)) {
                        valError = new ErrorVM
                        {
                            description = "Different metal cannot be billed together.",
                            ErrorStatusCode = HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                }
            }
            return true;
        }

        public bool GetOfferDiscountF12(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            try {
                //var courseList = db.Database.SqlQuery(
                //"exec usp_SalesDiscountChanges @estno, @companycode, @branchcode ", estNo, companyCode, branchCode);

                string sql = "EXEC [dbo].[usp_SalesDiscountChanges] \n"
                             + "  @p0, \n"
                             + "  @p1,\n"
                             + "  @p2";
                List<object> parameterList = new List<object>();
                parameterList.Add(estNo);
                parameterList.Add(companyCode);
                parameterList.Add(branchCode);
                object[] parametersArray = parameterList.ToArray();
                int recordsAffected = SIGlobals.Globals.ExecuteSQL(sql, db, parametersArray);

                //db.usp_SalesDiscountChanges(Convert.ToInt32(estNo), companyCode, branchCode);
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }
        public bool GetCancelOfferDiscountF12(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            try {
                string sql = "EXEC [dbo].[usp_SalesDiscountRevert] \n"
                         + "  @p0, \n"
                         + "  @p1,\n"
                         + "  @p2";
                List<object> parameterList = new List<object>();
                parameterList.Add(estNo);
                parameterList.Add(companyCode);
                parameterList.Add(branchCode);
                object[] parametersArray = parameterList.ToArray();
                int recordsAffected = SIGlobals.Globals.ExecuteSQL(sql, db, parametersArray);

                //db.usp_SalesDiscountRevert(Convert.ToInt32(estNo), companyCode, branchCode);
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public string PrintBillByEstimation(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            string printValue = string.Empty;
            KTTU_SALES_MASTER sales = db.KTTU_SALES_MASTER.Where(est => est.est_no == estNo
                                                                && est.company_code == companyCode
                                                                && est.branch_code == branchCode).FirstOrDefault();
            if (sales == null) {
                error = new ErrorVM()
                {
                    description = "Invalide Estimation Number or Bill Not Yet Generated.",
                    ErrorStatusCode = HttpStatusCode.BadRequest
                };
            }
            printValue = new SalesBillingBL().PrintSalesBilling(companyCode, branchCode, sales.bill_no, out error);
            return printValue;
        }

        public IQueryable GetAllEstimationDetails(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                var data = (from sal in db.KTTU_SALES_EST_MASTER
                            where sal.company_code == companyCode && sal.branch_code == branchCode
                            select new
                            {
                                EstNo = sal.est_no,
                                BillNo = sal.bill_no,
                                CustomerName = sal.Cust_Name,
                                GS = sal.gstype
                            }).OrderBy(d => d.EstNo);
                return data.AsQueryable();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public bool GetSalesEstimateRowVersion(string companyCode, string branchCode, int estNo, out SalesEstimateRowVersion estimateRowVersion, out ErrorVM error)
        {
            error = null;
            estimateRowVersion = null;
            try {
                var estimateDetail = db.KTTU_SALES_EST_MASTER.Where(est => est.est_no == estNo
                                                               && est.company_code == companyCode
                                                               && est.branch_code == branchCode).FirstOrDefault();
                if (estimateDetail == null) {
                    error = new ErrorVM { ErrorStatusCode = HttpStatusCode.NotFound, description = "Estimation " + estNo.ToString() + " is not found." };
                    return false;
                }
                estimateRowVersion = new SalesEstimateRowVersion
                {
                    CompanyCode = estimateDetail.company_code,
                    BranchCode = estimateDetail.branch_code,
                    EstNo = estimateDetail.est_no,
                    RowRevisionString = estimateDetail.RowVersionString,
                    Invoiced = estimateDetail.bill_no > 0 ? true : false
                };
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }

            return true;
        }

        public ProdigyPrintVM GetSalesEstimatePrint(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            //TODO: At present I'm using cst_no field in KSTU_COMPANY_MASTER table to get if the print is RAW or HTML.
            //In future, we should add a new field and get the details from that. It will be done when Proidgy is consolidated in single db.
            PrintConfiguration pc = new PrintConfiguration();
            var printConfig = pc.GetPrintConfigurationForDraftDocuments(companyCode, branchCode, "SAL_EST");
            if (printConfig == "HTML") {
                var htmlPrintData = Estimation60ColumnThermalPrint(companyCode, branchCode, estNo, out error);
                printObject.ContinueNextPrint = false;
                printObject.Data = new PrintConfiguration().Base64Encode(htmlPrintData);
                printObject.PrintType = "HTML";
            }
            else {
                var dotMatrixPrintData = EstimationDotMatrxiPrint80(companyCode, branchCode, estNo, out error);
                printObject.ContinueNextPrint = true;
                printObject.Data = dotMatrixPrintData;
                printObject.PrintType = "RAW";
            }

            return printObject;
        }

        public bool CheckAndGetOfferDiscount(SalesEstModel estimate, out DiscountOutput discountOutput, out ErrorVM error)
        {
            discountOutput = null;
            error = null;
            if (estimate == null) {
                error = new ErrorVM { ErrorStatusCode = HttpStatusCode.BadRequest, description = "Invalid sales estimation data." };
                return false;
            }

            var currentDate = estimate.Date.Date;
            var discountPeriod = db.KSTS_DISCOUNT_PERIOD.Where(x => currentDate >= x.start_date && currentDate <= x.end_date).FirstOrDefault();
            if (discountPeriod == null) {
                error = new ErrorVM { ErrorStatusCode = HttpStatusCode.BadRequest, description = "No offer is available for the selected date." };
                return false;
            }

            bool offerSucceeded = false;
            var config = db.APP_CONFIG_TABLE.Where(x => x.obj_id == "16092019" && x.company_code == estimate.CompanyCode && x.branch_code == estimate.BranchCode).FirstOrDefault();
            if (config == null) {
                error = new ErrorVM { ErrorStatusCode = HttpStatusCode.BadRequest, description = "Offer configuration is not available." };
                return false;
            }
            switch (config.value) {
                case 1:
                    offerSucceeded = GetOfferDiscount2022(estimate, out discountOutput, out error);
                    break;
                case 0:
                case 2:
                default:
                    offerSucceeded = false;
                    error = new ErrorVM { ErrorStatusCode = HttpStatusCode.BadRequest, description = "Offer for case 0, 2/ is not yet available." };
                    break;
            }
            return offerSucceeded;
        }

        public bool GetOfferDiscount2022(SalesEstModel saleEst, out DiscountOutput discountOutput, out ErrorVM error)
        {
           error = null;
            discountOutput = null;
            bool functionResult = false;
            if(saleEst == null) {
                error = new ErrorVM { ErrorStatusCode = HttpStatusCode.BadRequest, description = "Invalid sales estimation data." };
                return false;
            }
            if(saleEst.SalesEstDetail == null) {
                error = new ErrorVM { ErrorStatusCode = HttpStatusCode.BadRequest, description = "Invalid sales estimation data. There is nothing to calculate." };
                return false;
            }
            if (saleEst.SalesEstDetail.Count <= 0) {
                error = new ErrorVM { ErrorStatusCode = HttpStatusCode.BadRequest, description = "Invalid sales estimation data. No estimation lines available." };
                return false;
            }

            try {
                var gsCount = saleEst.SalesEstDetail.Select(x => x.GSCode).Distinct().Count();
                if (gsCount > 1) {
                    error = new ErrorVM { ErrorStatusCode = HttpStatusCode.BadRequest, description = $"For offer discount 2022 only one GS is allowed in the estimation." };
                    return false;
                }
                var gsCode = saleEst.SalesEstDetail[0].GSCode;
                switch (gsCode) {
                    case "NGO":
                        functionResult = CalculateDiscountForNGO(saleEst, out discountOutput, out error);
                        break;
                    case "NGD":
                        functionResult = CalculateDiscountForNGD(saleEst, out discountOutput, out error);
                        break;
                    case "SL":
                        functionResult = CalculateDiscountForSilver(saleEst, out discountOutput, out error);
                        break;
                    case "PTM":
                        functionResult = CalculateDiscountForPTM(saleEst, out discountOutput, out error);
                        break;
                    default:
                        functionResult = false;
                        error = new ErrorVM { ErrorStatusCode = HttpStatusCode.BadRequest, description = $"Offer is not available for the selected GS {gsCode}" };
                        break;
                }
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }

            return functionResult;
        }

        private bool CalculateDiscountForNGO(SalesEstModel saleEst, out DiscountOutput discountOutput, out ErrorVM error)
        {
            error = null;
            discountOutput = null;
            //var discCatGroups = db.KSTU_DISCOUNT_ITEM_CATEGORY_GROUPING.Where(
            //    x => x.company_code == saleEst.CompanyCode 
            //    && x.branch_code == saleEst.BranchCode).ToList();
            var discCatGroups = db.KSTU_DISCOUNT_ITEM_CATEGORY_GROUPING.ToList();

            var categoryList = from d in discCatGroups
                               join s in saleEst.SalesEstDetail
                               on new { GS = d.gs_code, IC = d.Item_code, Cntr = d.counter_code }
                               equals new { GS = s.GSCode, IC = s.ItemCode, Cntr = s.CounterCode }
                               select new { CategoryCode = d.catagory_code };
            var distinctCategoryCount = categoryList == null ? 0 : categoryList.Select(x => x.CategoryCode).Distinct().Count();
            var billingWeight = saleEst.SalesEstDetail.Where(x => x.ItemCode != "CHCN" && x.ItemCode != "SGCN").Sum(y => y.Netwt);
            var amountBeforeTax = saleEst.SalesEstDetail.Where(x => x.ItemCode != "CHCN" && x.ItemCode != "SGCN").Sum(y => y.ItemAmount);

            decimal discountForUnitAmt = 0;
            decimal discountMultiples = 0;
            var discRuleConfig = db.DiscountRuleConfigurations.Where(x => x.NoOfCategories <= distinctCategoryCount
                && billingWeight >= x.FromWeight && billingWeight <= x.ToWeight).FirstOrDefault();
            if (discRuleConfig != null) {
                discountForUnitAmt = Convert.ToDecimal(discRuleConfig.CatDiscountAmt);
                discountMultiples = Convert.ToDecimal(discRuleConfig.MultiplesOf);
            }
            else {
                var nextDiscountRule = db.DiscountRuleConfigurations.Where(x => billingWeight >= x.FromWeight
                    && billingWeight <= x.ToWeight).FirstOrDefault();
                if (nextDiscountRule != null) {
                    discountForUnitAmt = Convert.ToDecimal(nextDiscountRule.WtDiscountAmt);
                    discountMultiples = Convert.ToDecimal(nextDiscountRule.MultiplesOf);
                }
                else {
                    //No discount rule is defined.
                    discountOutput = new DiscountOutput
                    {
                        DiscountAmount = 0,
                        OfferCode = "FOD",
                        OfferName = "Fabulous February Offer",
                        DiscountAfterTax = true,
                        CalculationLogic = $"Total Net Wt: {billingWeight} | Total Amt: {amountBeforeTax} | No. Of Categories: {distinctCategoryCount} | Discount Per Unit: {discountForUnitAmt} | Discount multiples: {discountMultiples} | Amount: 0.00"
                    };
                    return true;
                }
            }
            if (discountMultiples <= 0) {
                error = new ErrorVM { ErrorStatusCode = HttpStatusCode.BadRequest, description = "No discount rule defined for the selected criteria." };
                return false;
            }
            var noOfDiscountMultiples = Math.Floor(amountBeforeTax / discountMultiples);
            var discountAmt = noOfDiscountMultiples * discountForUnitAmt;

            discountOutput = new DiscountOutput
            {
                DiscountAmount = discountAmt,
                OfferCode = "FOD",
                OfferName = "Fabulous February Offer",
                DiscountAfterTax = true,
                CalculationLogic = $"Total Net Wt: {billingWeight} | Total Amt: {amountBeforeTax} | No. Of Categories: {distinctCategoryCount} | Discount Per Unit: {discountForUnitAmt} | Discount multiples: {discountMultiples} | Amount: {discountAmt}"
            };
            return true;
        }

        private bool CalculateDiscountForNGD(SalesEstModel saleEst, out DiscountOutput discountOutput, out ErrorVM error)
        {
            error = null;
            discountOutput = null;
            //Get the diamond amount
            var diamondAmt = saleEst.SalesEstDetail.Sum(x => x.DiamondCharges);

            decimal discountMultiples = 20000;
            decimal discountForUnitAmt = 2022;
            var discRuleConfig = db.DiscountRuleConfigurations.FirstOrDefault();
            if (discRuleConfig != null) {
                discountMultiples = discRuleConfig.MultiplesOf;
            }

            var noOfDiscountMultiples = Math.Floor(diamondAmt / discountMultiples);
            var discountAmt = noOfDiscountMultiples * discountForUnitAmt;
            discountOutput = new DiscountOutput
            {
                DiscountAmount = discountAmt,
                OfferCode = "FOD",
                OfferName = "Fabulous February Offer",
                DiscountAfterTax = true,
                CalculationLogic = $"Diamond Amt: {diamondAmt} | Discount Per Unit: {discountForUnitAmt} | Discount multiples: {discountMultiples} | Amount: {discountAmt}"
            };

            return true;
        }

        private bool CalculateDiscountForPTM(SalesEstModel saleEst, out DiscountOutput discountOutput, out ErrorVM error)
        {
            error = null;
            discountOutput = null;
            //Get the line amount
            var lineAmt = saleEst.SalesEstDetail.Sum(x => x.ItemAmount);

            decimal discountMultiples = 20000;
            decimal discountForUnitAmt = 2022;
            var discRuleConfig = db.DiscountRuleConfigurations.FirstOrDefault();
            if (discRuleConfig != null) {
                discountMultiples = discRuleConfig.MultiplesOf;
            }

            var noOfDiscountMultiples = Math.Floor(lineAmt / discountMultiples);
            var discountAmt = noOfDiscountMultiples * discountForUnitAmt;
            discountOutput = new DiscountOutput
            {
                DiscountAmount = discountAmt,
                OfferCode = "FOD",
                OfferName = "Fabulous February Offer",
                DiscountAfterTax = true,
                CalculationLogic = $"Eligible Amt: {lineAmt} | Discount Per Unit: {discountForUnitAmt} | Discount multiples: {discountMultiples} | Amount: {discountAmt}"
            };

            return true;
        }

        private bool CalculateDiscountForSilver(SalesEstModel saleEst, out DiscountOutput discountOutput, out ErrorVM error)
        {
            error = null;
            discountOutput = null;

            var silverLineAmt = saleEst.SalesEstDetail.Where(x => x.MRPItem == true).Sum(y => y.ItemAmount);
            var silverMetalAmt = saleEst.SalesEstDetail.Where(x => x.MRPItem == false).Sum(y => y.MetalValue);
            var eligibleAmount = silverLineAmt + silverMetalAmt;

            decimal discountMultiples = 20000;
            decimal discountForUnitAmt = 2022;
            var discRuleConfig = db.DiscountRuleConfigurations.FirstOrDefault();
            if (discRuleConfig != null) {
                discountMultiples = discRuleConfig.MultiplesOf;
            }

            var noOfDiscountMultiples = Math.Floor(eligibleAmount / discountMultiples);
            var discountAmt = noOfDiscountMultiples * discountForUnitAmt;
            discountOutput = new DiscountOutput
            {
                DiscountAmount = discountAmt,
                OfferCode = "FOD",
                OfferName = "Fabulous February Offer",
                DiscountAfterTax = true,
                CalculationLogic = $"Elligible Amt: {eligibleAmount} | Discount Per Unit: {discountForUnitAmt} | Discount multiples: {discountMultiples} | Amount: {discountAmt}"
            };

            return true;
        }

        #endregion

        #region Protected Methods
        protected decimal GetItemWiseDiscount(string gscode, string item_name, decimal netwt, decimal totalsalamount, decimal diamondcarat, decimal stoneAmount, string countercode, decimal VAAmount, decimal invoiceTotalCarat, decimal totalnwt, string barcodeno, decimal soilddiamondcarat, decimal diamondAmount, string CompanyCode, string BranchCode)
        {
            decimal discountAmount = 0;

            string query = string.Format("select ISNULL(dbo.ufn_GetSalesDiscountAmount ('{0}','{1}',{2},{3},{4},{5},{6},'{7}','{8}','{9}','{10}',{11},{12},'{13}',{14},{15}),0) AS DISCOUNT"
           , gscode, item_name, netwt, totalsalamount, diamondcarat, stoneAmount, VAAmount, countercode, CompanyCode, BranchCode, DateTime.Now, invoiceTotalCarat, totalnwt, barcodeno, soilddiamondcarat, diamondAmount);
            var cmd = db.Database.Connection.CreateCommand();
            cmd.CommandText = query;
            cmd.Connection.Open();
            discountAmount = Convert.ToDecimal(cmd.ExecuteScalar());
            cmd.Connection.Close();

            //This is the way of calling Scalar functioin in Entity Framework But its throwing an Error (Date time conversion error so we use the above techinique)
            //string sqlQuery = "SELECT dbo.ufn_GetSalesDiscountAmount ('{0}','{1}',{2},{3},{4},{5},{6},'{7}','{8}','{9}','{10}',{11},{12},'{13}',{14},{15})";
            //Object[] parameters = { gscode, item_name, netwt, totalsalamount, diamondcarat, stoneAmount, VAAmount, countercode, CompanyCode, BranchCode, DateTime.Now.Date.ToString("yyyy/MM/dd"), invoiceTotalCarat, totalnwt, barcodeno, soilddiamondcarat, diamondAmount };
            //try
            //{
            //    discountAmount = db.Database.SqlQuery<decimal>(sqlQuery, parameters).FirstOrDefault();
            //}
            //catch (Exception excp)
            //{

            //}
            return decimal.Round(discountAmount, 2);
        }
        public bool GetGSTComponentValues(string gSTGoodsGroupCode, decimal Amount, bool isInterstateAssociate, out decimal sgstPercent, out decimal sgstAmt,
            out decimal cgstPercent, out decimal cgstAmt, out decimal igstPercent, out decimal igstAmt, out decimal cessPercent, out decimal cessAmt, string companyCode, string branchCode)
        {
            sgstPercent = sgstAmt = cgstPercent = cgstAmt = igstPercent = igstAmt = cessPercent = cessAmt = 0;

            string query = string.Format("EXEC dbo.usp_GSTGetGSTTaxValues '{0}','{1}','{2}'", gSTGoodsGroupCode, Amount, isInterstateAssociate.ToString());
            var dtGSTComponents = db.usp_GSTGetGSTTaxValues(gSTGoodsGroupCode, Amount, isInterstateAssociate, companyCode, branchCode).ToList();
            if (dtGSTComponents == null || dtGSTComponents.Count <= 0) {
                return false;
            }
            else {
                if (!isInterstateAssociate) {
                    cgstPercent = dtGSTComponents.Where(gst => gst.GSTComponentCode == "CGST").FirstOrDefault().GSTPercent;
                    sgstPercent = dtGSTComponents.Where(gst => gst.GSTComponentCode == "SGST").FirstOrDefault().GSTPercent;
                    cgstAmt = dtGSTComponents.Where(gst => gst.GSTComponentCode == "CGST").FirstOrDefault().GSTAmount;
                    sgstAmt = dtGSTComponents.Where(gst => gst.GSTComponentCode == "SGST").FirstOrDefault().GSTAmount;
                }
                else {
                    igstPercent = dtGSTComponents.Where(gst => gst.GSTComponentCode == "IGST").FirstOrDefault().GSTPercent;
                    igstAmt = dtGSTComponents.Where(gst => gst.GSTComponentCode == "IGST").FirstOrDefault().GSTAmount;
                }
            }
            return true;
        }
        public void CheckScalarFunction()
        {
            string sqlQuery = "SELECT dbo.ufn_SampleFunction({0},{1})";
            Object[] parameters = { 2500, 3600 };
            decimal activityCount = db.Database.SqlQuery<decimal>(sqlQuery, parameters).FirstOrDefault();
        }

        private void DeleteEstimation(string companyCode, string branchCode, int estNo)
        {
            KTTU_SALES_EST_MASTER estMaster = db.KTTU_SALES_EST_MASTER.Where(est => est.est_no == estNo && est.company_code == companyCode && est.branch_code == branchCode).FirstOrDefault();
            db.KTTU_SALES_EST_MASTER.Remove(estMaster);

            List<KTTU_SALES_EST_DETAILS> estDetails = db.KTTU_SALES_EST_DETAILS.Where(est => est.est_no == estNo && est.company_code == companyCode && est.branch_code == branchCode).ToList();
            foreach (KTTU_SALES_EST_DETAILS ksed in estDetails) {
                db.KTTU_SALES_EST_DETAILS.Remove(ksed);
            }
            List<KTTU_SALES_STONE_DETAILS> estStone = db.KTTU_SALES_STONE_DETAILS.Where(est => est.est_no == estNo && est.company_code == companyCode && est.branch_code == branchCode).ToList();
            foreach (KTTU_SALES_STONE_DETAILS kssd in estStone) {
                db.KTTU_SALES_STONE_DETAILS.Remove(kssd);
            }
        }

        private decimal CoinOfferCalculation(string companyCode, string branchCode, decimal totalAmount, decimal totalMetalWeight, decimal totalStoneWeight, string gsCode)
        {
            decimal offerWeight = 0;
            decimal offerAmount = 0;
            decimal totalCoinOffer = 0;
            DateTime appDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
            KSTS_DISCOUNT_PERIOD discountPeriod = db.KSTS_DISCOUNT_PERIOD.Where(d => d.company_code == companyCode
                                                                                    && d.branch_code == branchCode
                                                                                    && DbFunctions.TruncateTime(d.start_date) <= DbFunctions.TruncateTime(appDate)
                                                                                    && DbFunctions.TruncateTime(d.end_date) >= DbFunctions.TruncateTime(appDate)).FirstOrDefault();
            // Some offer is going on
            if (discountPeriod != null) {

                if (gsCode == "NGO") {
                    // Offer ID is hot coded here because in magna they done same thing. confirmed by Ram sir on 18/08/2020
                    List<KTTU_OFFER_DISCOUNT_DETAILS> offerDiscountOnGold = db.KTTU_OFFER_DISCOUNT_DETAILS.Where(d => d.company_code == companyCode
                                                                                                         && d.branch_code == branchCode
                                                                                                         && d.isActive == "Y"
                                                                                                         && d.Offer_ID == "8").ToList();
                    if (offerDiscountOnGold != null && offerDiscountOnGold.Count > 0) {
                        if (offerDiscountOnGold[0].from_weight != 0 && offerDiscountOnGold[0].to_weight != 0) {
                            //Weight based Need to Implement this.
                        }
                        else if (offerDiscountOnGold[0].from_amount != 0 && offerDiscountOnGold[0].to_amount != 0) {
                            //Offer Based on Amount
                            offerWeight = offerDiscountOnGold[0].freeGoldvalue_per_grm;
                            offerAmount = offerDiscountOnGold[0].to_amount;
                            totalCoinOffer = Convert.ToInt32(Math.Floor(totalAmount / offerAmount));
                        }
                    }
                }
                else if (gsCode == "NGD") {
                    KTTU_OFFER_DISCOUNT_DETAILS offerDiscountOnDiamond = db.KTTU_OFFER_DISCOUNT_DETAILS.Where(d => d.company_code == companyCode
                                                                                                  && d.branch_code == branchCode
                                                                                                  && d.isActive == "Y"
                                                                                                  && d.Offer_ID == "7"
                                                                                                  && d.from_carrat <= totalStoneWeight
                                                                                                  && d.to_carrat >= totalStoneWeight).FirstOrDefault();
                    if (offerDiscountOnDiamond != null) {
                        offerWeight = offerDiscountOnDiamond.freeGoldvalue_per_grm;
                        offerAmount = offerDiscountOnDiamond.to_amount;
                        totalCoinOffer = Convert.ToInt32(Math.Floor(totalAmount / offerAmount));
                    }
                }
            }
            return totalCoinOffer;
        }

        private bool DoEstimationValidation(SalesEstMasterVM saleEst, out ErrorVM valError)
        {
            valError = null;
            string companyCode = string.Empty;
            string branchCode = string.Empty;
            string metalType = string.Empty;
            string gsCode = string.Empty;

            KTTU_SALES_EST_MASTER saleEstMaster = new KTTU_SALES_EST_MASTER();
            List<SalesEstDetailsVM> salEstVm = saleEst.salesEstimatonVM;
            if (salEstVm == null || salEstVm.Count == 0) {
                valError = new ErrorVM
                {
                    description = "Invalid Sales Details",
                    ErrorStatusCode = HttpStatusCode.BadRequest
                };
                return false;
            }

            foreach (SalesEstDetailsVM det in salEstVm) {
                companyCode = det.CompanyCode;
                branchCode = det.BranchCode;

                //Different Metal Validation
                gsCode = det.GsCode;

                List<KSTS_GS_ITEM_ENTRY> lstItemEntry = db.KSTS_GS_ITEM_ENTRY.Where(item => item.company_code == companyCode
                                                                           && item.branch_code == branchCode
                                                                           && item.gs_code == gsCode && item.bill_type == "S").ToList();
                if (lstItemEntry != null && lstItemEntry.Count > 0) {
                    if (metalType == string.Empty) {
                        metalType = lstItemEntry[0].metal_type;
                    }
                    KSTS_GS_ITEM_ENTRY item = lstItemEntry.Where(i => i.metal_type == metalType && gsCode == det.GsCode).FirstOrDefault();
                    if (item == null) {
                        valError = new ErrorVM
                        {
                            description = "Different metal cannot be billed together.",
                            ErrorStatusCode = HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                }

                //Barcode Billed or Not Validation
                if (!string.IsNullOrEmpty(det.BarcodeNo)) {
                    string message = new BarcodeBL().ValidateBarcodeDetails(det.BarcodeNo, companyCode, branchCode);
                    if (!string.IsNullOrEmpty(message)) {
                        valError = new ErrorVM
                        {
                            description = message,
                            ErrorStatusCode = HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                }
            }
            return true;
        }

        public string GetMetalTypeFromGS(string gsCode, string companyCode, string branchCode)
        {
            KSTS_GS_ITEM_ENTRY barcodeMetal = db.KSTS_GS_ITEM_ENTRY.Where(item => item.company_code == companyCode
                                                                          && item.branch_code == branchCode
                                                                          && item.gs_code == gsCode
                                                                          && item.bill_type == "S").FirstOrDefault();
            return barcodeMetal == null ? "" : barcodeMetal.metal_type;
        }
        #endregion
    }
}
