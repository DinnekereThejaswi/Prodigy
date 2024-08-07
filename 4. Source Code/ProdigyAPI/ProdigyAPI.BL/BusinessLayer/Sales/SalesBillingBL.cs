using NumberToWordsINR;
using ProdigyAPI.BL.BusinessLayer.Purchase;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.OldPurchase;
using ProdigyAPI.BL.ViewModel.Payment;
using ProdigyAPI.BL.ViewModel.Sales;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Linq;
using System.Text;
using ProdigyAPI.BL.BusinessLayer.Stock;
using ProdigyAPI.BL.BusinessLayer.Order;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.BL.BusinessLayer.Masters;
using System.Data;
using ProdigyAPI.BL.BusinessLayer.Common;
using ProdigyAPI.BL.ViewModel.Print;
using ProdigyAPI.BL.BusinessLayer.PrintConfig;

namespace ProdigyAPI.BL.BusinessLayer.Sales
{
    public class SalesBillingBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities(true);
        private const string MODULE_SEQ_NO = "4";
        private const string TABLE_NAME = "KTTU_SALES_MASTER";
        private CultureInfo indianDefaultCulture = new CultureInfo("hi-IN");
        List<OrderOTPAttributes> ORDER_OTP_ATTRIBUTES = new List<OrderOTPAttributes>();
        #endregion

        #region Methods
        public SalesMasterVM GetSalesBillingDetails(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            try {
                KTTU_SALES_EST_MASTER ksem = db.KTTU_SALES_EST_MASTER.Where(est => est.est_no == estNo
                                                                            && est.company_code == companyCode
                                                                            && est.branch_code == branchCode).FirstOrDefault();
                List<KTTU_SALES_EST_DETAILS> lstOfKSED = db.KTTU_SALES_EST_DETAILS.Where(est => est.est_no == estNo
                                                                            && est.company_code == companyCode
                                                                            && est.branch_code == branchCode).ToList();
                KSTU_CUSTOMER_MASTER customer = db.KSTU_CUSTOMER_MASTER.Where(cust => cust.cust_id == ksem.Cust_Id).FirstOrDefault();

                if (ksem == null) {
                    error = new ErrorVM()
                    {
                        index = 0,
                        description = "Invalid Estimation Details",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound,
                    };
                    return null;
                }

                if (ksem.bill_no != 0) {
                    error = new ErrorVM()
                    {
                        index = 0,
                        description = "Sales Estimation No: " + estNo + " already billed. Bill No: " + ksem.bill_no,
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound,
                    };
                    return null;
                }
                SalesMasterVM smv = new SalesMasterVM();
                smv.ObjID = ksem.obj_id;
                smv.CompanyCode = ksem.company_code;
                smv.BranchCode = ksem.branch_code;
                smv.BillNo = ksem.bill_no;
                smv.EstimationNo = ksem.est_no;
                smv.CustomerID = ksem.Cust_Id;
                smv.CustomerName = ksem.Cust_Name;
                smv.OrderDate = ksem.order_date;
                smv.OrderNo = ksem.order_no;
                smv.OrderAmount = ksem.order_amount;
                smv.BillDate = SIGlobals.Globals.GetDateTime();
                smv.OperatorCode = ksem.operator_code;
                smv.Karat = ksem.karat;
                smv.Rate = ksem.rate;
                smv.Tax = ksem.tax;
                smv.CessTax = 0;
                smv.DiscountAmount = Convert.ToDecimal(ksem.TotalDiscountVoucherAmt);
                smv.TotalCessAmount = 0;
                smv.TotalTaxAmount = ksem.total_tax_amount;
                smv.TotalBillAmount = ksem.grand_total - ksem.total_tax_amount - Convert.ToDecimal(ksem.TotalDiscountVoucherAmt); // Logical value.
                smv.GrandTotal = ksem.grand_total;
                smv.PurchaseAmount = ksem.purchase_amount;
                smv.SpurchaseAmount = ksem.spurchase_amount;
                smv.Stype = ksem.s_type;
                smv.Cflag = ksem.item_set;
                smv.CancelledBy = "";
                smv.DiscountAutharity = "";
                smv.PurchaseNo = 0;
                smv.SpurchaseNo = 0;
                smv.GStype = ksem.gstype;
                smv.IsCredit = "N";
                smv.BillCounter = ""; // Need to send from UI
                smv.Is_Ins = ksem.is_ins;
                smv.CancelledRemarks = ksem.remarks;
                smv.GuranteeName = "";
                smv.DueDate = SIGlobals.Globals.GetDateTime();
                smv.BalanceAmt = 0;// Need to send from UI
                smv.NewCust = null;
                smv.SalesBillPK = null;
                smv.UpdateOn = SIGlobals.Globals.GetDateTime();
                smv.ItemSet = ksem.item_set;
                smv.TotalSaleAmount = ksem.grand_total - ksem.total_tax_amount; // Logical value.
                smv.EdCess = ksem.ed_cess;
                smv.HedCess = ksem.hed_cess;
                smv.EdCessPercent = ksem.ed_cess_percent;
                smv.HedCessPercent = ksem.hed_cess_percent;
                smv.ExciseDutyPercent = ksem.excise_duty_percent;
                smv.ExciseDutyAmount = ksem.excise_duty_amount;
                smv.FinalYear = SIGlobals.Globals.GetFinancialYear(db, companyCode, branchCode);
                smv.CancelledRemarks = "";
                smv.HISchemeAmount = ksem.hi_scheme_amount;
                smv.HISchemeNo = ksem.hi_scheme_no;
                smv.HIBonusAmount = ksem.hi_bonus_amount;
                smv.Salutation = ksem.salutation;
                smv.NewBillNo = smv.BillNo.ToString();
                smv.RoundOff = ksem.round_off;
                smv.ReferedBy = lstOfKSED[0].sal_code;// Logical Value.
                smv.ShiftID = 0;
                smv.BookNo = "";
                smv.RefInvoiceNo = "";
                smv.FlightNo = "";
                smv.Race = "";
                smv.MangerCode = "";
                smv.BillType = "N";
                smv.IsPAN = ksem.pan_no == "" ? "N" : "Y";
                smv.Address1 = ksem.address1;
                smv.Address2 = ksem.address2;
                smv.Address3 = ksem.address3;
                smv.City = ksem.city;
                smv.PinCode = ksem.pin_code;
                smv.MobileNo = ksem.mobile_no;
                smv.State = ksem.state;
                smv.StateCode = ksem.state_code;
                smv.Tin = ksem.tin;
                smv.PANNo = ksem.pan_no;
                smv.pos = ksem.pos;
                smv.CorparateID = null;
                smv.CorporateBranchID = null;
                smv.EmployeeID = null;
                smv.RegisteredMN = null;
                smv.ProfessionID = "";
                smv.EmpcorpEmailID = null;

                List<SalesDetailsVM> lstOfSalesDetails = new List<SalesDetailsVM>();
                foreach (KTTU_SALES_EST_DETAILS ksd in lstOfKSED) {
                    SalesDetailsVM sdv = new SalesDetailsVM();
                    sdv.ObjID = ksd.obj_id;
                    sdv.CompanyCode = ksd.company_code;
                    sdv.BranchCode = ksd.branch_code;
                    sdv.BillNo = Convert.ToInt32(ksd.bill_no);
                    sdv.SlNo = ksd.sl_no;
                    sdv.EstimationNo = ksd.est_no;
                    sdv.BarcodeNo = ksd.barcode_no;
                    sdv.SalCode = ksd.sal_code;
                    sdv.ItemName = ksd.item_name;
                    sdv.CounterCode = ksd.counter_code;
                    sdv.ItemNo = ksd.item_no;
                    sdv.GrossWt = ksd.gwt;
                    sdv.StoneWt = ksd.swt;
                    sdv.NetWt = ksd.nwt;
                    sdv.WastpPercent = ksd.wast_percent;
                    sdv.MakingChargePerRS = ksd.making_charge_per_rs;
                    sdv.VaAmount = ksd.va_amount;
                    sdv.StoneCharges = ksd.stone_charges;
                    sdv.TotalAmount = ksd.total_amount;
                    sdv.OfferValue = ksd.offer_value;
                    sdv.GoldValue = ksd.gold_value;
                    sdv.DiamondCharges = ksd.diamond_charges;
                    sdv.AddWt = ksd.AddWt;
                    sdv.DeductWt = ksd.DeductWt;
                    sdv.HallMarCharges = ksd.hallmarcharges;
                    sdv.McAmount = ksd.mc_amount;
                    sdv.WastageGrms = ksd.wastage_grms;
                    sdv.McPercent = ksd.mc_percent;
                    sdv.AddQty = ksd.Addqty;
                    sdv.DeductQty = ksd.Deductqty;
                    sdv.OofferValue = ksd.offer_value;
                    sdv.UpdateOn = ksd.UpdateOn;
                    sdv.GsCode = ksd.gs_code;
                    sdv.Rate = ksd.rate;
                    sdv.karat = ksd.karat;
                    sdv.AdBarcode = ksd.ad_barcode;
                    sdv.AdCounter = ksd.ad_counter;
                    sdv.AdItem = ksd.ad_item;
                    sdv.isEDApplicable = ksd.isEDApplicable;
                    sdv.McType = ksd.mc_type;
                    sdv.FinalYear = ksd.Fin_Year;
                    sdv.NewBillNo = ksd.New_Bill_No;
                    sdv.ItemAdditionalDiscount = ksd.item_additional_discount;
                    sdv.ItemTotalAfterDiscount = ksd.item_total_after_discount;
                    sdv.TaxPercentage = ksd.tax_percentage;
                    sdv.TaxAmount = ksd.tax_amount;
                    sdv.ItemFinalAmount = ksd.item_final_amount;
                    sdv.SupplierCode = ksd.supplier_code;
                    sdv.ItemSize = ksd.item_size;
                    sdv.ImgId = ksd.img_id;
                    sdv.DesignCode = ksd.design_code;
                    sdv.DesignName = ksd.design_name;
                    sdv.BatchId = ksd.batch_id;
                    sdv.RfID = ksd.rf_id;
                    sdv.McPerPiece = ksd.mc_per_piece;
                    sdv.RoundOff = 0; // May be some logic
                    sdv.ItemFinalAmountAfterRoundoff = 0;// May be some logic
                    sdv.PurityPer = 0;
                    sdv.MeltingPercent = 0;
                    sdv.MeltingLoss = 0;
                    sdv.ItemType = "S"; // May be some logic
                    sdv.DiscountMc = ksd.Discount_Mc;
                    sdv.TotalSalesMc = ksd.Total_sales_mc;
                    sdv.McDiscountAmt = ksd.Mc_Discount_Amt;
                    sdv.PurchaseMc = ksd.purchase_mc;
                    sdv.GSTGroupCode = ksd.GSTGroupCode;
                    sdv.SGSTPercent = ksd.SGST_Percent;
                    sdv.SGSTAmount = ksd.SGST_Amount;
                    sdv.CGSTPercent = ksd.CGST_Percent;
                    sdv.CGSTAmount = ksd.CGST_Amount;
                    sdv.IGSTPercent = ksd.IGST_Percent;
                    sdv.IGSTAmount = ksd.IGST_Amount;
                    sdv.HSN = ksd.HSN;
                    sdv.PieceRate = "N";// May be some logic
                    sdv.DeductSWt = ksd.DeductSWt;
                    sdv.OrdDiscountAmt = ksd.Ord_Discount_Amt;
                    sdv.DedCounter = ksd.ded_counter;
                    sdv.DedItem = ksd.ded_item;

                    List<SalesStoneVM> lstOfSalesStone = new List<SalesStoneVM>();
                    List<KTTU_SALES_STONE_DETAILS> lstOfKSSD = db.KTTU_SALES_STONE_DETAILS.Where(est => est.est_no == estNo
                                                                                            && est.barcode_no == sdv.BarcodeNo
                                                                                            && est.company_code == companyCode
                                                                                            && est.branch_code == branchCode).ToList();
                    foreach (KTTU_SALES_STONE_DETAILS kssd in lstOfKSSD) {
                        SalesStoneVM svm = new SalesStoneVM();
                        svm.ObjID = kssd.obj_id;
                        svm.CompanyCode = kssd.company_code;
                        svm.BranchCode = kssd.branch_code;
                        svm.BillNo = kssd.bill_no;
                        svm.SlNo = kssd.sl_no;
                        svm.EstNo = kssd.est_no;
                        svm.EstSrNo = kssd.est_Srno;
                        svm.BarcodeNo = kssd.barcode_no;
                        svm.Type = kssd.type;
                        svm.Name = kssd.name;
                        svm.Qty = kssd.qty;
                        svm.Carrat = kssd.carrat;
                        svm.StoneWt = kssd.stone_wt;
                        svm.Rate = kssd.rate;
                        svm.Amount = kssd.amount;
                        svm.Tax = kssd.tax;
                        svm.TaxAmount = kssd.tax_amount;
                        svm.TotalAmount = kssd.total_amount;
                        svm.BillType = kssd.bill_type;
                        svm.DealerSalesNo = kssd.dealer_sales_no;
                        svm.BillDet11PK = kssd.BILL_DET11PK;
                        svm.UpdateOn = kssd.UpdateOn;
                        svm.FinYear = kssd.Fin_Year;
                        svm.Color = kssd.color;
                        svm.Clarity = kssd.clarity;
                        svm.Shape = kssd.shape;
                        svm.Cut = kssd.cut;
                        svm.Polish = kssd.polish;
                        svm.Symmetry = kssd.symmetry;
                        svm.Fluorescence = kssd.fluorescence;
                        svm.Certificate = kssd.certificate;
                        lstOfSalesStone.Add(svm);
                    }
                    sdv.salesstoneVM = lstOfSalesStone;
                    lstOfSalesDetails.Add(sdv);
                }
                smv.salesDetailsVM = lstOfSalesDetails;
                //smv.salesInvoiceAttribute = new SalesInvoiceAttributeVM()
                //{
                //    OfferDiscount = Convert.ToDecimal(lstOfKSED.Sum(d => d.offer_value))
                //};
                return smv;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public int SaveSalesBilling(SalesMasterVM sales, out ErrorVM error)
        {
            error = null;
            try {

                #region Sales Master
                int billNumber = Convert.ToInt32(SIGlobals.Globals.GetFinancialYear(db, sales.CompanyCode, sales.BranchCode).ToString().Remove(0, 1) +
                                 db.KSTS_SEQ_NOS.Where(sq => sq.obj_id == MODULE_SEQ_NO
                                                        && sq.company_code == sales.CompanyCode
                                                        && sq.branch_code == sales.BranchCode).FirstOrDefault().nextno);
                string objectID = SIGlobals.Globals.GetMagnaGUID(TABLE_NAME, billNumber, sales.CompanyCode, sales.BranchCode);
                KTTU_SALES_MASTER salesMaster = new KTTU_SALES_MASTER();
                salesMaster.obj_id = objectID;
                salesMaster.company_code = sales.CompanyCode;
                salesMaster.branch_code = sales.BranchCode;
                salesMaster.bill_no = billNumber;
                salesMaster.est_no = sales.EstimationNo;
                salesMaster.cust_Id = sales.CustomerID;
                salesMaster.cust_name = sales.CustomerName;
                salesMaster.order_no = sales.OrderNo;
                salesMaster.order_date = sales.OrderDate;
                salesMaster.order_amount = sales.OrderAmount;
                salesMaster.bill_date = sales.BillDate;
                salesMaster.operator_code = sales.OperatorCode;
                salesMaster.karat = sales.Karat;
                salesMaster.rate = sales.Rate;
                salesMaster.tax = sales.Tax;
                salesMaster.cess_tax = sales.CessTax;
                salesMaster.discount_amount = sales.DiscountAmount;
                salesMaster.total_cess_amount = sales.TotalCessAmount;
                salesMaster.total_tax_amount = sales.TotalTaxAmount;
                salesMaster.total_bill_amount = sales.TotalBillAmount;
                salesMaster.grand_total = sales.GrandTotal;
                salesMaster.purchase_amount = sales.PurchaseAmount;
                salesMaster.spurchase_amount = sales.SpurchaseAmount;
                salesMaster.s_type = sales.Stype;
                salesMaster.cflag = sales.Cflag;
                salesMaster.cancelled_by = sales.CancelledBy;
                salesMaster.discount_autharity = sales.DiscountAutharity;
                salesMaster.purchase_no = sales.PurchaseNo;
                salesMaster.spurchase_no = sales.SpurchaseNo;
                salesMaster.gstype = sales.GStype;
                salesMaster.iscredit = sales.IsCredit;
                salesMaster.bill_counter = sales.BillCounter;
                salesMaster.is_ins = sales.Is_Ins;
                salesMaster.remarks = sales.CancelledRemarks;
                salesMaster.guranteename = sales.GuranteeName;
                salesMaster.duedate = sales.DueDate;
                salesMaster.balance_amt = sales.BalanceAmt;
                salesMaster.new_cust = sales.NewCust;
                salesMaster.SalesbillPK = sales.SalesBillPK;
                salesMaster.UpdateOn = SIGlobals.Globals.GetDateTime();
                salesMaster.item_set = sales.ItemSet;
                salesMaster.total_sale_amount = sales.TotalSaleAmount;
                salesMaster.ed_cess = sales.EdCess;
                salesMaster.hed_cess = sales.HedCess;
                salesMaster.ed_cess_percent = sales.EdCessPercent;
                salesMaster.hed_cess_percent = sales.HedCessPercent;
                salesMaster.excise_duty_percent = sales.ExciseDutyPercent;
                salesMaster.excise_duty_amount = sales.ExciseDutyAmount;
                salesMaster.fin_year = sales.FinalYear;
                salesMaster.cancelled_remarks = sales.CancelledBy;
                salesMaster.hi_scheme_amount = sales.HISchemeAmount;
                salesMaster.hi_scheme_no = sales.HISchemeNo;
                salesMaster.hi_bonus_amount = sales.HIBonusAmount;
                salesMaster.salutation = sales.Salutation;
                salesMaster.New_Bill_No = sales.NewBillNo;
                salesMaster.round_off = sales.RoundOff;
                salesMaster.refered_by = sales.ReferedBy;
                salesMaster.ShiftID = sales.ShiftID;
                salesMaster.book_no = sales.BookNo;
                salesMaster.ref_invoice_no = sales.RefInvoiceNo;
                salesMaster.flight_no = sales.FlightNo;
                salesMaster.race = sales.Race;
                salesMaster.manger_code = sales.MangerCode;
                salesMaster.bill_type = sales.BillType;
                salesMaster.isPAN = sales.IsPAN;
                salesMaster.UniqRowID = Guid.NewGuid();
                salesMaster.address1 = sales.Address1;
                salesMaster.address2 = sales.Address2;
                salesMaster.address3 = sales.Address3;
                salesMaster.city = sales.City;
                salesMaster.pin_code = sales.PinCode;
                salesMaster.mobile_no = sales.MobileNo;
                salesMaster.state = sales.State;
                salesMaster.state_code = sales.StateCode;
                salesMaster.tin = sales.Tin;
                salesMaster.pan_no = sales.PANNo;
                salesMaster.pos = sales.pos;
                salesMaster.Corparate_ID = sales.CorparateID;
                salesMaster.Corporate_Branch_ID = sales.CorporateBranchID;
                salesMaster.Employee_Id = sales.EmployeeID;
                salesMaster.Registered_MN = sales.RegisteredMN;
                salesMaster.profession_ID = sales.ProfessionID;
                salesMaster.Empcorp_Email_ID = sales.EmpcorpEmailID;
                db.KTTU_SALES_MASTER.Add(salesMaster);
                #endregion

                #region Sales Details
                foreach (SalesDetailsVM sdvm in sales.salesDetailsVM) {
                    KTTU_SALES_DETAILS salesDetails = new KTTU_SALES_DETAILS();
                    salesDetails.obj_id = objectID;
                    salesDetails.company_code = sdvm.CompanyCode;
                    salesDetails.branch_code = sdvm.BranchCode;
                    salesDetails.bill_no = salesMaster.bill_no;
                    salesDetails.sl_no = sdvm.SlNo;
                    salesDetails.est_no = sdvm.EstimationNo;
                    salesDetails.barcode_no = sdvm.BarcodeNo;
                    salesDetails.sal_code = sdvm.SalCode;
                    salesDetails.item_name = sdvm.ItemName;
                    salesDetails.counter_code = sdvm.CounterCode;
                    salesDetails.item_no = sdvm.ItemNo;
                    salesDetails.gwt = sdvm.GrossWt;
                    salesDetails.swt = sdvm.StoneWt;
                    salesDetails.nwt = sdvm.NetWt;
                    salesDetails.wast_percent = sdvm.WastpPercent;
                    salesDetails.making_charge_per_rs = sdvm.MakingChargePerRS;
                    salesDetails.va_amount = sdvm.VaAmount;
                    salesDetails.stone_charges = sdvm.StoneCharges;
                    salesDetails.total_amount = sdvm.TotalAmount;
                    salesDetails.gold_value = sdvm.GoldValue;
                    salesDetails.diamond_charges = sdvm.DiamondCharges;
                    salesDetails.AddWt = sdvm.AddWt;
                    salesDetails.DeductWt = sdvm.DeductWt;
                    salesDetails.hallmarcharges = sdvm.HallMarCharges;
                    salesDetails.mc_amount = sdvm.McAmount;
                    salesDetails.wastage_grms = sdvm.WastageGrms;
                    salesDetails.mc_percent = sdvm.McPercent;
                    salesDetails.Addqty = sdvm.AddQty;
                    salesDetails.Deductqty = sdvm.DeductQty;
                    salesDetails.offer_value = sdvm.OfferValue;
                    salesDetails.UpdateOn = SIGlobals.Globals.GetDateTime();
                    salesDetails.gs_code = sdvm.GsCode;
                    salesDetails.rate = sdvm.Rate;
                    salesDetails.karat = sdvm.karat;
                    salesDetails.ad_barcode = sdvm.AdBarcode;
                    salesDetails.ad_counter = sdvm.AdCounter;
                    salesDetails.ad_item = sdvm.AdItem;
                    salesDetails.isEDApplicable = sdvm.isEDApplicable;
                    salesDetails.mc_type = sdvm.McType;
                    salesDetails.Fin_Year = sdvm.FinalYear;
                    salesDetails.New_Bill_No = sdvm.NewBillNo;
                    salesDetails.item_additional_discount = sdvm.ItemAdditionalDiscount;
                    salesDetails.item_total_after_discount = sdvm.ItemTotalAfterDiscount;
                    salesDetails.tax_percentage = sdvm.TaxPercentage;
                    salesDetails.tax_amount = sdvm.TaxAmount;
                    salesDetails.item_final_amount = sdvm.ItemFinalAmount;
                    salesDetails.supplier_code = sdvm.SupplierCode;
                    salesDetails.item_size = sdvm.ItemSize;
                    salesDetails.img_id = sdvm.ImgId;
                    salesDetails.design_code = sdvm.DesignCode;
                    salesDetails.design_name = sdvm.DesignName;
                    salesDetails.batch_id = sdvm.BatchId;
                    salesDetails.rf_id = sdvm.RfID;
                    salesDetails.mc_per_piece = sdvm.McPerPiece;
                    salesDetails.round_off = sdvm.RoundOff;
                    salesDetails.item_final_amount_after_roundoff = sdvm.ItemFinalAmountAfterRoundoff;
                    salesDetails.purity_per = sdvm.PurityPer;
                    salesDetails.melting_percent = sdvm.MeltingPercent;
                    salesDetails.melting_loss = sdvm.MeltingLoss;
                    salesDetails.item_type = sdvm.ItemType;
                    salesDetails.Discount_Mc = sdvm.DiscountMc;
                    salesDetails.Total_sales_mc = sdvm.TotalSalesMc;
                    salesDetails.Mc_Discount_Amt = sdvm.McDiscountAmt;
                    salesDetails.purchase_mc = sdvm.PurchaseMc;
                    salesDetails.GSTGroupCode = sdvm.GSTGroupCode;
                    salesDetails.SGST_Percent = sdvm.SGSTPercent;
                    salesDetails.SGST_Amount = sdvm.SGSTAmount;
                    salesDetails.CGST_Percent = sdvm.CGSTPercent;
                    salesDetails.CGST_Amount = sdvm.CGSTAmount;
                    salesDetails.IGST_Percent = sdvm.IGSTPercent;
                    salesDetails.IGST_Amount = sdvm.IGSTAmount;
                    salesDetails.HSN = sdvm.HSN;
                    salesDetails.Piece_Rate = sdvm.PieceRate;
                    salesDetails.UniqRowID = Guid.NewGuid();
                    salesDetails.DeductSWt = sdvm.DeductSWt;
                    salesDetails.Ord_Discount_Amt = sdvm.OrdDiscountAmt;
                    salesDetails.ded_counter = sdvm.DedCounter;
                    salesDetails.ded_item = sdvm.DedItem;
                    db.KTTU_SALES_DETAILS.Add(salesDetails);

                    // Incase weight added from other barcode, then we mark that barcode as sold.
                    if (salesDetails.ad_barcode != "") {
                        KTTU_BARCODE_MASTER kbm = db.KTTU_BARCODE_MASTER.Where(bm => bm.barcode_no == salesDetails.ad_barcode
                                                                                && bm.company_code == sales.CompanyCode
                                                                                && bm.branch_code == sales.BranchCode).FirstOrDefault();
                        kbm.sold_flag = "Y";
                        db.Entry(kbm).State = System.Data.Entity.EntityState.Modified;
                    }

                    #region Stone Details
                    foreach (SalesStoneVM kssd in sdvm.salesstoneVM) {
                        KTTU_SALES_STONE_DETAILS salesStoneDetails = new KTTU_SALES_STONE_DETAILS();
                        salesStoneDetails.obj_id = objectID;
                        salesStoneDetails.company_code = kssd.CompanyCode;
                        salesStoneDetails.branch_code = kssd.BranchCode;
                        salesStoneDetails.bill_no = salesMaster.bill_no;
                        salesStoneDetails.sl_no = kssd.SlNo;
                        salesStoneDetails.est_no = kssd.EstNo;
                        salesStoneDetails.est_Srno = kssd.EstSrNo;
                        salesStoneDetails.barcode_no = kssd.BarcodeNo;
                        salesStoneDetails.type = kssd.Type;
                        salesStoneDetails.name = kssd.Name;
                        salesStoneDetails.qty = kssd.Qty;
                        salesStoneDetails.carrat = kssd.Carrat;
                        salesStoneDetails.stone_wt = kssd.StoneWt;
                        salesStoneDetails.rate = kssd.Rate;
                        salesStoneDetails.amount = kssd.Amount;
                        salesStoneDetails.tax = kssd.Tax;
                        salesStoneDetails.tax_amount = kssd.TaxAmount;
                        salesStoneDetails.total_amount = kssd.TotalAmount;
                        salesStoneDetails.bill_type = kssd.BillType;
                        salesStoneDetails.dealer_sales_no = kssd.DealerSalesNo;
                        salesStoneDetails.BILL_DET11PK = kssd.BillDet11PK;
                        salesStoneDetails.UpdateOn = SIGlobals.Globals.GetDateTime();
                        salesStoneDetails.Fin_Year = kssd.FinYear;
                        salesStoneDetails.color = kssd.Color;
                        salesStoneDetails.clarity = kssd.Clarity;
                        salesStoneDetails.shape = kssd.Shape;
                        salesStoneDetails.cut = kssd.Cut;
                        salesStoneDetails.polish = kssd.Polish;
                        salesStoneDetails.symmetry = kssd.Symmetry;
                        salesStoneDetails.fluorescence = kssd.Fluorescence;
                        salesStoneDetails.certificate = kssd.Certificate;
                        salesStoneDetails.UniqRowID = Guid.NewGuid();
                        db.KTTU_SALES_STONE_DETAILS.Add(salesStoneDetails);
                    }
                    #endregion
                }
                #endregion

                #region Payment Information 
                int paySlNo = 1;
                foreach (PaymentVM pvm in sales.PaymentVM) {
                    KTTU_PAYMENT_DETAILS kpd = new KTTU_PAYMENT_DETAILS();
                    kpd.obj_id = objectID;
                    kpd.company_code = pvm.CompanyCode;
                    kpd.branch_code = pvm.BranchCode;
                    kpd.series_no = salesMaster.bill_no;
                    kpd.receipt_no = 0;
                    kpd.sno = paySlNo;
                    kpd.trans_type = "S";
                    kpd.pay_mode = pvm.PayMode;
                    kpd.pay_details = pvm.PayDetails;
                    kpd.pay_date = SIGlobals.Globals.GetApplicationDate(pvm.CompanyCode, pvm.BranchCode);
                    kpd.pay_amt = pvm.PayAmount;
                    kpd.Ref_BillNo = pvm.RefBillNo;
                    kpd.party_code = pvm.PartyCode;
                    kpd.bill_counter = pvm.BillCounter;
                    kpd.is_paid = pvm.IsPaid;
                    kpd.bank = pvm.Bank;
                    kpd.bank_name = pvm.BankName;
                    kpd.cheque_date = pvm.ChequeDate;
                    kpd.card_type = pvm.CardType;
                    kpd.expiry_date = pvm.ExpiryDate;
                    kpd.cflag = pvm.CFlag;
                    kpd.card_app_no = pvm.CardAppNo;
                    kpd.scheme_code = pvm.SchemeCode;
                    kpd.sal_bill_type = pvm.SalBillType;
                    kpd.operator_code = pvm.OperatorCode;
                    kpd.session_no = pvm.SessionNo;
                    kpd.UpdateOn = SIGlobals.Globals.GetDateTime();
                    kpd.group_code = pvm.GroupCode;
                    kpd.amt_received = pvm.AmtReceived;
                    kpd.bonus_amt = pvm.BonusAmt;
                    kpd.win_amt = pvm.WinAmt;
                    kpd.ct_branch = pvm.CTBranch;
                    kpd.fin_year = pvm.FinYear;
                    kpd.CardCharges = pvm.CardCharges;
                    kpd.cheque_no = pvm.ChequeNo;
                    kpd.New_Bill_No = pvm.NewBillNo;
                    kpd.Add_disc = pvm.AddDisc;
                    kpd.isOrdermanual = pvm.IsOrderManual;
                    kpd.currency_value = pvm.CurrencyValue;
                    kpd.exchange_rate = pvm.ExchangeRate;
                    kpd.currency_type = pvm.CurrencyType;
                    kpd.tax_percentage = pvm.TaxPercentage;
                    kpd.cancelled_by = pvm.CancelledBy;
                    kpd.cancelled_remarks = pvm.CancelledRemarks;
                    kpd.cancelled_date = pvm.CancelledDate;
                    kpd.isExchange = pvm.IsExchange;
                    kpd.exchangeNo = pvm.ExchangeNo;
                    kpd.new_receipt_no = pvm.NewReceiptNo;
                    kpd.Gift_Amount = pvm.GiftAmount;
                    kpd.cardSwipedBy = pvm.CardSwipedBy;
                    kpd.version = pvm.Version;
                    kpd.GSTGroupCode = pvm.GSTGroupCode;
                    kpd.SGST_Percent = pvm.SGSTPercent;
                    kpd.CGST_Percent = pvm.CGSTPercent;
                    kpd.IGST_Percent = pvm.IGSTPercent;
                    kpd.HSN = pvm.HSN;
                    kpd.SGST_Amount = pvm.SGSTAmount;
                    kpd.CGST_Amount = pvm.CGSTAmount;
                    kpd.IGST_Amount = pvm.IGSTAmount;
                    kpd.pay_amount_before_tax = pvm.PayAmountBeforeTax;
                    kpd.pay_tax_amount = pvm.PayTaxAmount;
                    kpd.UniqRowID = Guid.NewGuid();
                    db.KTTU_PAYMENT_DETAILS.Add(kpd);
                    paySlNo = paySlNo + 1;
                }
                #endregion

                #region Updating To Estimation
                KTTU_SALES_EST_MASTER ksem = db.KTTU_SALES_EST_MASTER.Where(sem => sem.est_no == sales.EstimationNo
                                                                            && sem.company_code == sales.CompanyCode
                                                                            && sem.branch_code == sales.BranchCode).FirstOrDefault();
                ksem.bill_no = salesMaster.bill_no;
                db.Entry(ksem).State = System.Data.Entity.EntityState.Modified;
                #endregion

                SIGlobals.Globals.UpdateSeqenceNumber(db, MODULE_SEQ_NO, sales.CompanyCode, sales.BranchCode);
                db.SaveChanges();
                return salesMaster.bill_no;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return 0;
            }
        }

        public int NewSaveSalesBilling(SalesBillingVM sales, out int genOrderNo, out int genReceiptNo, out ErrorVM error, out List<OrderOTPAttributes> otpAttributes)
        {
            error = null;
            otpAttributes = null;
            genOrderNo = 0;
            genReceiptNo = 0;
            bool IsSalesWithPurchase = false;
            int finYear = 0;

            if (sales == null) {
                error = new ErrorVM()
                {
                    index = 0,
                    description = "Invalid Data.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                };
                return 0;
            }
            DateTime appDate = SIGlobals.Globals.GetApplicationDate(sales.CompanyCode, sales.BranchCode);
            finYear = SIGlobals.Globals.GetFinancialYear(db, sales.CompanyCode, sales.BranchCode);
            KTTU_SALES_EST_MASTER ksem = db.KTTU_SALES_EST_MASTER.Where(est => est.est_no == sales.EstNo
                                                                        && est.company_code == sales.CompanyCode
                                                                        && est.branch_code == sales.BranchCode).FirstOrDefault();
            List<KTTU_SALES_EST_DETAILS> ksed = db.KTTU_SALES_EST_DETAILS.Where(est => est.est_no == sales.EstNo
                                                                        && est.company_code == sales.CompanyCode
                                                                        && est.branch_code == sales.BranchCode).ToList();
            #region Validation
            if (ksem == null) {
                error = new ErrorVM()
                {
                    index = 0,
                    description = "Invalid Estimation Details",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound,
                };
                return 0;
            }

            if (ksem.bill_no != 0) {
                error = new ErrorVM()
                {
                    index = 0,
                    description = "Sales Estimation No: " + sales.EstNo + " already billed. Bill No: " + ksem.bill_no,
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound,
                };
                return 0;
            }

            // Validating PAN Based on Estimation Amount Even without Payment (Credit Receipt). With Payment Validation handled, Here handled only without pan validation.
            if (sales.PANNo == null || sales.PANNo == "") {
                bool panValid = ValidationOnPAN(db, sales.CustID, null, ksem.total_est_amount, sales.CompanyCode, sales.BranchCode, out error);
                if (error != null) {
                    return 0;
                }
            }

            // Validating Is Credit Bill with Guranter Details.
            if (sales.IsCreditNote && sales.GuaranteeName == "") {
                error = new ErrorVM()
                {
                    index = 0,
                    description = "Guaranter details are required to generate Credit Bill.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                };
                return 0;
            }

            // Validating if it is not a credit bill and checking Receipt details.
            if (sales.IsCreditNote == false && sales.PayableAmt == 0 && sales.lstOfPayment.Count == 0) {
                error = new ErrorVM()
                {
                    index = 0,
                    description = "Please enter Receipt details.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                };
                return 0;
            }

            // Validating if its not a order and Payble amount is there then checking for Payment details (Payble Amount is negetive in this case so we are checking <0)
            if (sales.IsOrder == false && sales.PayableAmt < 0 && sales.lstOfPayToCustomer.Count == 0) {
                error = new ErrorVM()
                {
                    index = 0,
                    description = "Please enter payment details.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                };
                return 0;
            }

            string errMsg = string.Empty;
            if (!ValidateSalesBillObject(sales, out errMsg)) {
                error = new ErrorVM()
                {
                    description = errMsg,
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                };
                return 0;
            }

            #region Amount Validation
            decimal totalPaidAmt = 0;
            decimal totalPaidAmtToCustomer = 0;
            foreach (PaymentVM pay in sales.lstOfPayment) {
                totalPaidAmt = totalPaidAmt + Convert.ToDecimal(pay.PayAmount);
            }

            // Validating Receipt Amount should equal to Billing Amount.
            if (sales.ReceivableAmt > 0) {
                if (sales.ReceivableAmt != totalPaidAmt) {
                    error = new ErrorVM()
                    {
                        index = 0,
                        description = "Receipt Amount should be equal to Billing Amount",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound,
                    };
                    return 0;
                }
            }

            // Checking Amount Payable to Customer.
            if (sales.PayableAmt < 0 && sales.IsOrder == false) {
                List<PaymentVM> payToCustomer = sales.lstOfPayToCustomer;
                if (payToCustomer != null && payToCustomer.Count > 0) {
                    foreach (PaymentVM pay in payToCustomer) {
                        totalPaidAmtToCustomer = totalPaidAmtToCustomer + Convert.ToDecimal(pay.PayAmount);
                    }

                    if ((sales.PayableAmt * -1) != totalPaidAmtToCustomer) {
                        error = new ErrorVM()
                        {
                            index = 0,
                            description = "Payment Amount should be equal to Balance Amount",
                            ErrorStatusCode = System.Net.HttpStatusCode.NotFound,
                        };
                        return 0;
                    }
                }
                else {
                    if ((sales.PayableAmt * -1) != totalPaidAmt) {
                        error = new ErrorVM()
                        {
                            index = 0,
                            description = "Payment Amount should be equal to Billed Amount",
                            ErrorStatusCode = System.Net.HttpStatusCode.NotFound,
                        };
                        return 0;
                    }
                }

            }
            #endregion

            #region Bhima Validation from Magna

            #region Before Save Validation in Magna
            ErrorVM valError = new ErrorVM();

            // Bellow Validation is not required because Customer Handled Separately in Prodigy
            //if (!ValidateCustomer(sales.CustID, sales.MobileNo, sales.CompanyCode, sales.BranchCode, out valError, db)) {
            //    error = valError;
            //    return 0;
            //}

            if (!ValidationSalesBillingPayment(sales, out valError, db)) {
                error = valError;
                return 0;
            }

            if (!ValidateSpecialDiscount(sales, out valError, db)) {
                error = valError;
                return 0;
            }

            if (!ValidationOperatorDiscount(db, sales, out valError)) {
                error = valError;
                return 0;
            }

            if (!new OrderBL().OrderCashValidation(db, sales.lstOfPayment, sales.CustID, ksem.order_no, sales.CompanyCode, sales.BranchCode, out valError)) {
                error = valError;
                return 0;
            }
            if (!ValidationOfferOnOrderCreatedThroughChit(db, sales, ksem.order_no, out valError)) {
                error = valError;
                return 0;
            }

            if (!ValidationCashTransactionAmount(db, sales.CustID, sales.CompanyCode, sales.BranchCode, out valError)) {
                error = valError;
                return 0;
            }

            if (sales.PANNo == null || sales.PANNo == "") {
                if (!ValidationOnPAN(db, sales.CustID, sales.lstOfPayment, totalPaidAmt, sales.CompanyCode, sales.BranchCode, out valError)) {
                    error = valError;
                    return 0;
                }
            }
            else {
                KSTU_CUSTOMER_MASTER customer = new CustomerBL().GetActualCustomerDetails(sales.CustID, sales.MobileNo, sales.CompanyCode, sales.BranchCode);
                if (customer != null) {
                    customer.pan_no = sales.PANNo;
                    db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                }
            }

            if (!ValidateAdditionalandSpecialDscountPersentage(db, sales, out valError)) {
                error = valError;
                return 0;
            }
            if (sales.IsTCSBill && sales.TCSAmt > 0) {
                error = new ErrorVM()
                {
                    index = 0,
                    description = "TCS is Applicable for this bill.TCS Amount {0} \n Do you want to generate bill?",
                    ErrorStatusCode = System.Net.HttpStatusCode.Continue,
                };
                return 0;
            }

            if (sales.salesInvoiceAttribute != null && sales.salesInvoiceAttribute.AdditionalDiscount < 0 && sales.salesInvoiceAttribute.OfferDiscount < 0 && ksed.Sum(s => s.Mc_Discount_Amt) < 0) {
                error = new ErrorVM()
                {
                    index = 0,
                    description = "Discount amount should not be negative.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                };
                return 0;
            }
            #endregion

            #region Validation After Click on Save in Magna Handled Here only, before save in Prodigy

            if (!ValidateExcessPaymentAmount(db, totalPaidAmt, sales.CompanyCode, sales.BranchCode, out valError)) {
                error = valError;
                return 0;
            }

            // Validation is done on Counter Stock, GS Stock and Valdation of Barcodes.
            foreach (KTTU_SALES_EST_DETAILS sed in ksed) {
                #region Check Counter Stock
                KTTU_COUNTER_STOCK counterStock = db.KTTU_COUNTER_STOCK.Where(s => s.company_code == sed.company_code && s.branch_code == sed.branch_code && s.item_name == sed.item_name && s.counter_code == sed.counter_code && s.gs_code == sed.gs_code).FirstOrDefault();
                if (counterStock.closing_gwt <= 0) {
                    error = new ErrorVM()
                    {
                        field = "Stock Check",
                        index = 0,
                        description = counterStock.item_name + "  - Has no stock",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return 0;
                }
                #endregion

                #region GS Stock
                KTTU_GS_SALES_STOCK gsstock = db.KTTU_GS_SALES_STOCK.Where(s => s.company_code == sed.company_code && s.branch_code == sed.branch_code && s.gs_code == sed.gs_code).FirstOrDefault();
                if (gsstock.closing_gwt < 0) {
                    error = new ErrorVM()
                    {
                        field = "Stock Check",
                        index = 0,
                        description = "Please check GS -" + sed.gs_code + " Stock. GS - " + sed.gs_code + " Closing Gross Wt. Stock is  less than " + sed.gwt + "",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return 0;
                }
                #endregion

                #region Validating Barcode Details
                string message = string.Empty;
                if (sed.barcode_no != "") {
                    message = SIGlobals.Globals.ValidateBarcodeDetails(db, sed.barcode_no, sed.company_code, sed.branch_code, ksem.order_no.ToString());
                    if (message != "") {
                        error = new ErrorVM()
                        {
                            field = "Barcode",
                            index = 0,
                            description = message,
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return 0;
                    }
                }
                if (sed.ad_barcode != "") {
                    message = SIGlobals.Globals.ValidateBarcodeDetails(db, sed.ad_barcode, sed.company_code, sed.branch_code);
                    if (message != "") {
                        error = new ErrorVM()
                        {
                            field = "Barcode",
                            index = 0,
                            description = message,
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return 0;
                    }
                }
                #endregion
            }

            // Validate Orders (Global Attacched Order)
            bool orderVal = new OrderBL().ValidateOrder(ksem.order_no, sales.CompanyCode, sales.BranchCode, "SB", sales.EstNo, out valError);
            if (!orderVal) {
                error = valError;
                return 0;
            }

            // Validating Attached Payment Details
            if (!ValidateAttachedPaymentdetails(db, sales, out valError)) {
                error = valError;
                return 0;
            }

            // Order OTP Valdiation as per the Settings.
            if (SIGlobals.Globals.GetConfigurationValue(db, "28022020", sales.CompanyCode, sales.BranchCode) == 1) {
                bool otpValDone = false;
                if (sales.orderOTPAttributes == null || sales.orderOTPAttributes.Count == 0) {
                    if (!ValidateOrderOTP(sales, ksem.order_no, out valError, out otpValDone)) {
                        error = valError;
                        return 0;
                    }
                    else {
                        if (!otpValDone) {
                            error = new ErrorVM()
                            {
                                description = "Please verfiy Order OTP",
                                ErrorStatusCode = System.Net.HttpStatusCode.Forbidden,
                                GenObject = ORDER_OTP_ATTRIBUTES
                            };
                            return 0;
                        }
                    }
                }
                else {
                    // Checking How Many Orders are Attached and How many OTP are validated. Both should be equal Otherwise throw an error.
                    List<OrderOTPPaymentVM> orderDictionary = GetListOfOrderAttached(sales, ksem.order_no, out error);
                    if (orderDictionary != null && sales.orderOTPAttributes != null && orderDictionary.Count != sales.orderOTPAttributes.Count) {
                        error = new ErrorVM()
                        {
                            description = "Please verfiy OTP for all Orders",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                        };
                        return 0;
                    }
                    foreach (OrderOTPAttributes orderOTP in sales.orderOTPAttributes) {
                        MobileOTP mobileOTP = db.MobileOTPs.Where(otp => otp.smsID == orderOTP.SMSID && otp.docno == orderOTP.OrderNo).FirstOrDefault();
                        if (mobileOTP != null && mobileOTP.IsUsed == "Y") {

                        }
                        else {
                            error = new ErrorVM()
                            {
                                description = "Please Verify OTP Validation for the Order " + orderOTP.OrderNo,
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                            };
                            return 0;
                        }
                    }
                }
            }
            #endregion

            #endregion

            #endregion

            using (var transaction = db.Database.BeginTransaction()) {
                db.Database.CommandTimeout = 5;
                try {

                    #region Sales Discount Calculations Old
                    //decimal totalItemAmt = ksed.Sum(x => x.total_amount);
                    //decimal totalGstAmt = Convert.ToDecimal(ksed.Sum(x => x.CGST_Amount))
                    //    + Convert.ToDecimal(ksed.Sum(x => x.SGST_Amount)) + Convert.ToDecimal(ksed.Sum(x => x.IGST_Amount)) +
                    //    +Convert.ToDecimal(ksed.Sum(x => x.CESSAmount));
                    //decimal netGSTPercent = totalGstAmt / totalItemAmt;
                    //decimal discountAmtInclGST = sales.DifferenceDiscountAmt;// 1087.84M for estimtion no 210;
                    //decimal discountAmtWithoutGST = Math.Round(discountAmtInclGST / (1 + netGSTPercent), 2);
                    //foreach (KTTU_SALES_EST_DETAILS sd in ksed) {
                    //    sd.LineItemContribution = Math.Round(sd.total_amount / totalItemAmt, 8);
                    //    sd.NewItemDiscountAmt = Math.Round(sd.LineItemContribution * discountAmtWithoutGST, 2);
                    //    sd.NewItemTotalAfterDiscExclGST = Math.Round(sd.total_amount - sd.NewItemDiscountAmt, 2);
                    //    sd.NewCGST_Amount = Math.Round(Convert.ToDecimal(sd.NewItemTotalAfterDiscExclGST * sd.CGST_Percent / 100), 2);
                    //    sd.NewSGST_Amount = Math.Round(Convert.ToDecimal(sd.NewItemTotalAfterDiscExclGST * sd.SGST_Percent / 100), 2);
                    //    sd.NewIGST_Amount = Math.Round(Convert.ToDecimal(sd.NewItemTotalAfterDiscExclGST * sd.IGST_Percent / 100), 2);
                    //    sd.NewCESSAmount = Math.Round(Convert.ToDecimal(sd.NewItemTotalAfterDiscExclGST * sd.CESSPercent / 100), 2);
                    //    sd.NewItemTotalInclGST = sd.NewItemTotalAfterDiscExclGST + sd.NewCGST_Amount + sd.NewSGST_Amount +
                    //        sd.NewIGST_Amount + sd.NewCESSAmount;
                    //}
                    #endregion

                    #region Sales Discount Calculations
                    decimal additionalDiscountPercent = 0;
                    if (sales.salesInvoiceAttribute != null) {
                        additionalDiscountPercent = sales.salesInvoiceAttribute.AdditionalDiscount;
                    }
                    decimal totalAmountIncludingGst = 0;
                    decimal totalAmountExcludingGST = 0;
                    foreach (KTTU_SALES_EST_DETAILS sd in ksed) {
                        sd.NewItemDiscountAmt = Math.Round(sd.total_amount * additionalDiscountPercent / 100, 2);
                        sd.NewItemTotalAfterDiscExclGST = Math.Round(sd.total_amount - sd.NewItemDiscountAmt - Convert.ToDecimal(sd.offer_value) - Convert.ToDecimal(sd.Mc_Discount_Amt), 2);
                        sd.NewCGST_Amount = Math.Round(Convert.ToDecimal(sd.NewItemTotalAfterDiscExclGST * sd.CGST_Percent / 100), 2);
                        sd.NewSGST_Amount = Math.Round(Convert.ToDecimal(sd.NewItemTotalAfterDiscExclGST * sd.SGST_Percent / 100), 2);
                        sd.NewIGST_Amount = Math.Round(Convert.ToDecimal(sd.NewItemTotalAfterDiscExclGST * sd.IGST_Percent / 100), 2);
                        sd.NewCESSAmount = Math.Round(Convert.ToDecimal(sd.NewItemTotalAfterDiscExclGST * sd.CESSPercent / 100), 2);
                        sd.NewItemTotalInclGST = sd.NewItemTotalAfterDiscExclGST + sd.NewCGST_Amount + sd.NewSGST_Amount +
                            sd.NewIGST_Amount + sd.NewCESSAmount;
                        totalAmountIncludingGst += sd.NewItemTotalInclGST;
                        totalAmountExcludingGST += sd.NewItemTotalAfterDiscExclGST;
                    }
                    #endregion

                    #region Credit Note Balance Calculation
                    decimal balanceAmt = 0;
                    decimal paidAmt = 0;
                    if (sales.IsCreditNote == true && sales.lstOfPayment.Count > 0) {
                        paidAmt = Convert.ToDecimal(sales.lstOfPayment.Sum(pay => pay.PayAmount));
                        balanceAmt = totalAmountIncludingGst - paidAmt;
                    }
                    #endregion

                    #region Sales Master
                    int billNo = Convert.ToInt32(SIGlobals.Globals.GetFinancialYear(db, sales.CompanyCode, sales.BranchCode).ToString().Remove(0, 1)
                                                + db.KSTS_SEQ_NOS.Where(sq => sq.obj_id == MODULE_SEQ_NO
                                                                        && sq.company_code == sales.CompanyCode
                                                                        && sq.branch_code == sales.BranchCode).FirstOrDefault().nextno);
                    string objectID = SIGlobals.Globals.GetMagnaGUID(TABLE_NAME, billNo, sales.CompanyCode, sales.BranchCode);

                    KTTU_SALES_MASTER salesMaster = new KTTU_SALES_MASTER();
                    salesMaster.obj_id = objectID;
                    salesMaster.company_code = sales.CompanyCode;
                    salesMaster.branch_code = sales.BranchCode;
                    salesMaster.bill_no = billNo;
                    salesMaster.est_no = ksem.est_no;
                    salesMaster.cust_Id = ksem.Cust_Id;
                    salesMaster.cust_name = ksem.Cust_Name;
                    salesMaster.order_no = ksem.order_no;
                    salesMaster.order_date = ksem.order_date;
                    salesMaster.order_amount = ksem.order_amount;
                    salesMaster.bill_date = SIGlobals.Globals.GetApplicationDate(sales.CompanyCode, sales.BranchCode);
                    salesMaster.operator_code = sales.OperatorCode;
                    salesMaster.karat = ksem.karat;
                    salesMaster.rate = ksem.rate;
                    salesMaster.tax = ksem.tax;
                    salesMaster.cess_tax = 0;
                    salesMaster.discount_amount = ksem.discount_amount;
                    salesMaster.total_cess_amount = 0;
                    salesMaster.total_tax_amount = ksed.Sum(sed => sed.NewSGST_Amount + sed.NewCGST_Amount + sed.NewIGST_Amount + sed.NewCESSAmount);
                    salesMaster.total_bill_amount = totalAmountExcludingGST;// totalAmountIncludingGst; // Convert.ToDecimal(ksed.Sum(sed => sed.va_amount + sed.gold_value + sed.stone_charges + sed.diamond_charges));
                    salesMaster.grand_total = ksem.grand_total;
                    salesMaster.purchase_amount = ksem.purchase_amount;
                    salesMaster.spurchase_amount = ksem.spurchase_amount;
                    salesMaster.s_type = ksem.s_type;
                    salesMaster.cflag = "N";
                    salesMaster.cancelled_by = "";
                    salesMaster.discount_autharity = "";
                    salesMaster.purchase_no = 0;
                    salesMaster.spurchase_no = 0;
                    salesMaster.gstype = ksem.gstype;
                    salesMaster.iscredit = sales.IsCreditNote ? "Y" : "N";
                    salesMaster.bill_counter = sales.BillCounter;
                    salesMaster.is_ins = ksem.is_ins;
                    salesMaster.remarks = "";
                    salesMaster.guranteename = sales.GuaranteeName == null ? "" : sales.GuaranteeName;
                    salesMaster.mobile_no = sales.GuaranteeMobileNo;
                    salesMaster.duedate = SIGlobals.Globals.GetApplicationDate(sales.CompanyCode, sales.BranchCode);
                    salesMaster.balance_amt = sales.PayableAmt < 0 ? sales.PayableAmt : balanceAmt;  //sales.PayableAmt < 0 ? sales.PayableAmt : sales.BalanceAmt;
                    salesMaster.new_cust = null;
                    salesMaster.SalesbillPK = null;
                    salesMaster.UpdateOn = SIGlobals.Globals.GetDateTime();
                    salesMaster.item_set = ksem.item_set;
                    salesMaster.total_sale_amount = salesMaster.total_bill_amount;
                    salesMaster.ed_cess = ksem.ed_cess;
                    salesMaster.hed_cess = ksem.hed_cess;
                    salesMaster.ed_cess_percent = ksem.ed_cess_percent;
                    salesMaster.hed_cess_percent = ksem.hed_cess_percent;
                    salesMaster.excise_duty_percent = ksem.excise_duty_percent;
                    salesMaster.excise_duty_amount = ksem.excise_duty_amount;
                    salesMaster.fin_year = finYear;
                    salesMaster.cancelled_remarks = "";
                    salesMaster.hi_scheme_amount = ksem.hi_scheme_amount;
                    salesMaster.hi_scheme_no = ksem.hi_scheme_no;
                    salesMaster.hi_bonus_amount = ksem.hi_bonus_amount;
                    salesMaster.salutation = ksem.salutation;
                    salesMaster.New_Bill_No = Convert.ToString(billNo);
                    salesMaster.round_off = ksem.round_off;
                    salesMaster.refered_by = "";
                    salesMaster.ShiftID = 0;
                    salesMaster.book_no = "";
                    salesMaster.ref_invoice_no = "";
                    salesMaster.flight_no = "";
                    salesMaster.race = "";
                    salesMaster.manger_code = "";
                    salesMaster.bill_type = sales.BillType;
                    salesMaster.isPAN = ksem.isPAN;
                    salesMaster.UniqRowID = Guid.NewGuid();
                    salesMaster.address1 = ksem.address1;
                    salesMaster.address2 = ksem.address2;
                    salesMaster.address3 = ksem.address3;
                    salesMaster.city = ksem.city;
                    salesMaster.pin_code = ksem.pin_code;
                    salesMaster.mobile_no = ksem.mobile_no;
                    salesMaster.state = ksem.state;
                    salesMaster.state_code = ksem.state_code;
                    salesMaster.tin = ksem.tin;
                    salesMaster.pan_no = string.IsNullOrEmpty(sales.PANNo) ? ksem.pan_no : sales.PANNo;
                    salesMaster.pos = ksem.pos;
                    salesMaster.Corparate_ID = ksem.Corparate_ID;
                    salesMaster.Corporate_Branch_ID = ksem.Corporate_Branch_ID;
                    salesMaster.Employee_Id = ksem.Employee_Id;
                    salesMaster.Registered_MN = ksem.Registered_MN;
                    salesMaster.profession_ID = ksem.profession_ID == null ? "" : ksem.profession_ID;
                    salesMaster.Empcorp_Email_ID = ksem.Empcorp_Email_ID;
                    salesMaster.store_location_id = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == sales.CompanyCode
                                                                                && c.branch_code == sales.BranchCode).FirstOrDefault().store_location_id;

                    // Customer is Created At the Time of Billing.
                    KSTU_CUSTOMER_MASTER customer = new KSTU_CUSTOMER_MASTER();
                    if (sales.CustID != 0) {
                        customer = new CustomerBL().GetActualCustomerDetails(sales.CustID, sales.MobileNo, sales.CompanyCode, sales.BranchCode);
                        salesMaster.cust_Id = customer.cust_id;
                        salesMaster.cust_name = customer.cust_name;
                        salesMaster.UniqRowID = Guid.NewGuid();
                        salesMaster.address1 = customer.address1;
                        salesMaster.address2 = customer.address2;
                        salesMaster.address3 = customer.address3;
                        salesMaster.city = customer.city;
                        salesMaster.pin_code = customer.pin_code;
                        salesMaster.mobile_no = customer.mobile_no;
                        salesMaster.state = customer.state;
                        salesMaster.state_code = customer.state_code;
                        salesMaster.tin = customer.tin;
                        salesMaster.pan_no = string.IsNullOrEmpty(sales.PANNo) ? customer.pan_no : sales.PANNo;
                        salesMaster.Corparate_ID = customer.Corparate_ID;
                        salesMaster.Corporate_Branch_ID = customer.Corporate_Branch_ID;
                        salesMaster.Employee_Id = customer.Employee_Id;
                        salesMaster.Registered_MN = customer.Registered_MN;
                        salesMaster.profession_ID = customer.profession_ID == null ? "" : customer.profession_ID;
                        salesMaster.Empcorp_Email_ID = customer.Empcorp_Email_ID;
                    }
                    db.KTTU_SALES_MASTER.Add(salesMaster);
                    #endregion

                    #region Sales Details
                    foreach (KTTU_SALES_EST_DETAILS sed in ksed) {
                        #region Adding Sales Details
                        KTTU_SALES_DETAILS salesDetails = new KTTU_SALES_DETAILS();
                        salesDetails.obj_id = objectID;
                        salesDetails.company_code = sed.company_code;
                        salesDetails.branch_code = sed.branch_code;
                        salesDetails.bill_no = salesMaster.bill_no;
                        salesDetails.sl_no = sed.sl_no;
                        salesDetails.est_no = sed.est_no;
                        salesDetails.barcode_no = sed.barcode_no;
                        salesDetails.sal_code = sed.sal_code;
                        salesDetails.item_name = sed.item_name;
                        salesDetails.counter_code = sed.counter_code;
                        salesDetails.item_no = sed.item_no;

                        salesDetails.gwt = sed.gwt;
                        salesDetails.swt = sed.swt;
                        salesDetails.nwt = sed.nwt;
                        salesDetails.wast_percent = sed.wast_percent;
                        salesDetails.making_charge_per_rs = sed.making_charge_per_rs;
                        salesDetails.va_amount = sed.va_amount;
                        salesDetails.stone_charges = sed.stone_charges;
                        salesDetails.total_amount = sed.NewItemTotalInclGST;// sed.total_amount;
                        salesDetails.gold_value = sed.gold_value;

                        salesDetails.diamond_charges = sed.diamond_charges;
                        salesDetails.AddWt = sed.AddWt;
                        salesDetails.DeductWt = sed.DeductWt;
                        salesDetails.hallmarcharges = sed.hallmarcharges;
                        salesDetails.mc_amount = sed.mc_amount;
                        salesDetails.wastage_grms = sed.wastage_grms;
                        salesDetails.mc_percent = sed.mc_percent;
                        salesDetails.Addqty = sed.Addqty;
                        salesDetails.Deductqty = sed.Deductqty;
                        salesDetails.offer_value = sed.offer_value;
                        salesDetails.UpdateOn = SIGlobals.Globals.GetDateTime();
                        salesDetails.gs_code = sed.gs_code;
                        salesDetails.rate = sed.rate;
                        salesDetails.karat = sed.karat;
                        salesDetails.ad_barcode = sed.ad_barcode;
                        salesDetails.ad_counter = sed.ad_counter;
                        salesDetails.ad_item = sed.ad_item;
                        salesDetails.isEDApplicable = sed.isEDApplicable;
                        salesDetails.mc_type = sed.mc_type;
                        salesDetails.Fin_Year = sed.Fin_Year;
                        salesDetails.New_Bill_No = sed.New_Bill_No;
                        salesDetails.item_additional_discount = sed.NewItemDiscountAmt; //sed.item_additional_discount;
                        salesDetails.item_total_after_discount = sed.NewItemTotalAfterDiscExclGST;// sed.item_total_after_discount;
                        salesDetails.tax_percentage = sed.tax_percentage;
                        salesDetails.tax_amount = sed.tax_amount;
                        salesDetails.item_final_amount = sed.NewItemTotalInclGST;// sed.item_final_amount;
                        salesDetails.supplier_code = sed.supplier_code;
                        salesDetails.item_size = sed.item_size;
                        salesDetails.img_id = sed.img_id;
                        salesDetails.design_code = sed.design_code;
                        salesDetails.design_name = sed.design_name;
                        salesDetails.batch_id = sed.batch_id;
                        salesDetails.rf_id = sed.rf_id;
                        salesDetails.mc_per_piece = sed.mc_per_piece;
                        salesDetails.round_off = 0;// sed.round_off;
                        salesDetails.item_final_amount_after_roundoff = 0;// sed.item_final_amount_after_roundoff;
                        salesDetails.purity_per = 0;// sed.purity_per;
                        salesDetails.melting_percent = 0;// sed.melting_percent;
                        salesDetails.melting_loss = 0;// sed.melting_loss;
                        salesDetails.item_type = "";// sed.item_type;
                        salesDetails.Discount_Mc = sed.Discount_Mc;
                        salesDetails.Total_sales_mc = sed.Total_sales_mc;
                        salesDetails.Mc_Discount_Amt = sed.Mc_Discount_Amt;
                        salesDetails.purchase_mc = sed.purchase_mc;
                        salesDetails.GSTGroupCode = sed.GSTGroupCode;
                        salesDetails.SGST_Percent = sed.SGST_Percent;
                        salesDetails.SGST_Amount = sed.NewSGST_Amount;// sed.SGST_Amount;
                        salesDetails.CGST_Percent = sed.CGST_Percent;
                        salesDetails.CGST_Amount = sed.NewCGST_Amount;// sed.CGST_Amount;
                        salesDetails.IGST_Percent = sed.IGST_Percent;
                        salesDetails.IGST_Amount = sed.NewIGST_Amount;// sed.IGST_Amount;
                        salesDetails.HSN = sed.HSN;
                        salesDetails.Piece_Rate = sed.Piece_Rate;
                        salesDetails.UniqRowID = Guid.NewGuid();
                        salesDetails.DeductSWt = sed.DeductSWt;
                        salesDetails.Ord_Discount_Amt = sed.Ord_Discount_Amt;
                        salesDetails.ded_counter = sed.ded_counter;
                        salesDetails.ded_item = sed.ded_item;
                        salesDetails.vaporLoss = sed.vaporLoss;
                        salesDetails.vaporAmount = sed.vaporAmount;
                        db.KTTU_SALES_DETAILS.Add(salesDetails);

                        // Incase weight added from other barcode, then we mark that barcode as sold.
                        if (salesDetails.ad_barcode != null && salesDetails.ad_barcode != "") {
                            KTTU_BARCODE_MASTER kbm = db.KTTU_BARCODE_MASTER.Where(bm => bm.barcode_no == salesDetails.ad_barcode && bm.branch_code == sales.BranchCode && bm.company_code == sales.CompanyCode).FirstOrDefault();
                            kbm.sold_flag = "Y";
                            kbm.UpdateOn = SIGlobals.Globals.GetDateTime();
                            db.Entry(kbm).State = System.Data.Entity.EntityState.Modified;
                        }
                        #endregion

                        #region Stone Details
                        List<KTTU_SALES_STONE_DETAILS> salesStoneDetails = db.KTTU_SALES_STONE_DETAILS.Where(est => est.est_no == sales.EstNo && est.est_Srno == sed.sl_no
                                                                              && est.company_code == sales.CompanyCode
                                                                              && est.branch_code == sales.BranchCode).ToList();
                        //Need to delte the existing stone details and again with bill_No. becuase we get the bellow error
                        //The property 'bill_no' is part of the object's key information and cannot be modified.
                        List<KTTU_SALES_STONE_DETAILS> estStone = db.KTTU_SALES_STONE_DETAILS.Where(est => est.est_no == sales.EstNo && est.est_Srno == sed.sl_no
                                                                                                    && est.company_code == sales.CompanyCode
                                                                                                    && est.branch_code == sales.BranchCode).ToList();
                        foreach (KTTU_SALES_STONE_DETAILS kssd in estStone) {
                            db.KTTU_SALES_STONE_DETAILS.Remove(kssd);
                        }
                        int stoneSlNo = 1;
                        foreach (KTTU_SALES_STONE_DETAILS ses in salesStoneDetails) {
                            KTTU_SALES_STONE_DETAILS ssd = new KTTU_SALES_STONE_DETAILS();
                            ssd.obj_id = objectID;
                            ssd.company_code = ses.company_code;
                            ssd.branch_code = ses.branch_code;
                            ssd.bill_no = billNo;
                            ssd.sl_no = stoneSlNo;
                            ssd.est_no = sales.EstNo;
                            ssd.est_Srno = ses.est_Srno;
                            ssd.barcode_no = ses.barcode_no;
                            ssd.type = ses.type;
                            ssd.name = ses.name;
                            ssd.qty = ses.qty;
                            ssd.carrat = ses.carrat;
                            ssd.stone_wt = ses.stone_wt;
                            ssd.rate = ses.rate;
                            ssd.amount = ses.amount;
                            ssd.tax = ses.tax;
                            ssd.tax_amount = ses.tax_amount;
                            ssd.total_amount = ses.total_amount;
                            ssd.bill_type = ses.bill_type;
                            ssd.dealer_sales_no = ses.dealer_sales_no;
                            ssd.BILL_DET11PK = ses.BILL_DET11PK;
                            ssd.UpdateOn = SIGlobals.Globals.GetDateTime();
                            ssd.Fin_Year = ses.Fin_Year;
                            ssd.color = ses.color;
                            ssd.clarity = ses.clarity;
                            ssd.shape = ses.shape;
                            ssd.cut = ses.cut;
                            ssd.polish = ses.polish;
                            ssd.symmetry = ses.symmetry;
                            ssd.fluorescence = ses.fluorescence;
                            ssd.certificate = ses.certificate;
                            ssd.UniqRowID = Guid.NewGuid();
                            db.KTTU_SALES_STONE_DETAILS.Add(ssd);
                            stoneSlNo = stoneSlNo + 1;
                        }
                        #endregion
                    }
                    #endregion

                    #region Purchase Details
                    KTTU_PURCHASE_EST_MASTER kpem = db.KTTU_PURCHASE_EST_MASTER.Where(pur => pur.est_no == sales.EstNo
                                                                                                    && pur.company_code == sales.CompanyCode
                                                                                                    && pur.branch_code == sales.BranchCode
                                                                                                    && pur.cflag != "Y").FirstOrDefault();

                    if (kpem != null) {
                        IsSalesWithPurchase = true;
                        ErrorVM purchaseError = new ErrorVM();
                        // Customer is Created at the time of billing, Need to update purchase Estimation, 
                        // otherwise purchae invoice customer details not come.
                        if (kpem.cust_id == 0) {
                            customer = new CustomerBL().GetActualCustomerDetails(sales.CustID, sales.MobileNo, sales.CompanyCode, sales.BranchCode);
                            kpem.cust_id = customer.cust_id;
                            kpem.cust_name = customer.cust_name;
                            kpem.address1 = customer.address1;
                            kpem.address2 = customer.address2;
                            kpem.address3 = customer.address3;
                            kpem.city = customer.city;
                            kpem.pin_code = customer.pin_code;
                            kpem.mobile_no = customer.mobile_no;
                            kpem.state = customer.state;
                            kpem.state_code = customer.state_code;
                            kpem.tin = customer.tin;
                            kpem.pan_no = customer.pan_no;
                            db.Entry(kpem).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        PurchaseBillingVM pb = new PurchaseBillingVM();
                        pb.CompanyCode = sales.CompanyCode;
                        pb.BranchCode = sales.BranchCode;
                        pb.EstNo = sales.EstNo;
                        pb.BillCounter = sales.BillCounter;
                        pb.OperatorCode = sales.OperatorCode;
                        pb.Type = "SE";
                        pb.BillNo = billNo;
                        pb.CustNo = Convert.ToInt32(salesMaster.cust_Id);
                        pb.PaidAmount = Convert.ToDecimal(kpem.total_purchase_amount);
                        pb.ObjID = objectID;
                        pb.SlNo = 1;
                        int purchaseBillNo = new PurchaseBillingBL(db).SavePurchaseBillWithTxn(pb, out purchaseError);
                        if (purchaseError != null) {
                            transaction.Rollback();
                            error = purchaseError;
                            return 0;
                        }
                    }
                    #endregion

                    #region Payment Information : Sales level Payment Attachemtns and Other Pay Modes.

                    // We are initialising Serial Number by 1 if there is Purchase Details
                    // If Purchase Details are there so we assign Serial Number by 2 because in payment details SerialNo Primary Key validation issue will come because 
                    // Purchase attachment will be there in Serial No 1
                    int paySlNo = IsSalesWithPurchase ? 2 : 1;

                    #region If Global Level Order Attached
                    if (ksem.order_no != 0) {
                        bool generated = GenerateOrderAsPayment(db, ksem.order_no, billNo, appDate, objectID, paySlNo, finYear, sales.OperatorCode, sales.CompanyCode, sales.BranchCode);
                        paySlNo = paySlNo + 1;
                    }
                    #endregion

                    #region Payment Modes
                    foreach (PaymentVM pvm in sales.lstOfPayment) {
                        KTTU_PAYMENT_DETAILS kpd = new KTTU_PAYMENT_DETAILS();
                        if (pvm.TransType == "S" && pvm.PayMode == "OP") {
                            #region Attached Order
                            int orderNo = Convert.ToInt32(pvm.RefBillNo);
                            bool generated = GenerateOrderAsPayment(db, orderNo, billNo, appDate, objectID, paySlNo, finYear, sales.OperatorCode, pvm.CompanyCode, pvm.BranchCode);
                            paySlNo = paySlNo + 1;
                            #endregion
                        }
                        else if (pvm.TransType == "S" && pvm.PayMode == "PE") {
                            #region Attached  Purchase Estimation
                            int purchaseEstNo = Convert.ToInt32(pvm.RefBillNo);
                            KTTU_PURCHASE_EST_MASTER attachedEst = db.KTTU_PURCHASE_EST_MASTER.Where(pur => pur.est_no == purchaseEstNo
                                                                                                        && pur.company_code == sales.CompanyCode
                                                                                                        && pur.branch_code == sales.BranchCode).FirstOrDefault();
                            if (attachedEst != null) {
                                ErrorVM purchaseError = new ErrorVM();
                                PurchaseBillingVM pb = new PurchaseBillingVM();
                                pb.CompanyCode = sales.CompanyCode;
                                pb.BranchCode = sales.BranchCode;
                                pb.EstNo = Convert.ToInt32(pvm.RefBillNo);
                                pb.BillCounter = sales.BillCounter;
                                pb.OperatorCode = sales.OperatorCode;
                                pb.Type = "SE";
                                pb.BillNo = billNo;
                                pb.PaidAmount = Convert.ToDecimal(attachedEst.total_purchase_amount);
                                pb.ObjID = objectID;
                                pb.SlNo = paySlNo;
                                int purchaseBillNo = new PurchaseBillingBL(db).SavePurchaseBillWithTxn(pb, out purchaseError);
                                if (purchaseError != null) {
                                    transaction.Rollback();
                                    error = purchaseError;
                                    return 0;
                                }
                            }
                            #endregion
                        }
                        else if (pvm.TransType == "S" && pvm.PayMode == "SE") {
                            #region Attached Sales Return Estimation
                            int srEstNo = Convert.ToInt32(pvm.RefBillNo);
                            KTTU_SR_MASTER attachedSR = db.KTTU_SR_MASTER.Where(sr => sr.est_no == srEstNo
                                                                                 && sr.company_code == sales.CompanyCode
                                                                                 && sr.branch_code == sales.BranchCode).FirstOrDefault();
                            if (attachedSR != null) {
                                ErrorVM srError = new ErrorVM();
                                ConfirmSalesReturnVM pb = new ConfirmSalesReturnVM();
                                pb.CompanyCode = sales.CompanyCode;
                                pb.BranchCode = sales.BranchCode;
                                pb.EstNo = Convert.ToInt32(pvm.RefBillNo);
                                pb.BillCounter = sales.BillCounter;
                                pb.OperatorCode = sales.OperatorCode;
                                pb.Type = "SE";
                                pb.BillNo = billNo;
                                pb.PaidAmount = Convert.ToDecimal(attachedSR.total_sramount);
                                pb.ObjectID = objectID;
                                pb.SlNo = paySlNo;
                                ConfirmVM confirm = new ConfirmSalesReturnBL(db).ConfirmSalesReturnWithTxn(pb, out srError);
                                int purchaseBillNo = confirm.SalesBillNo;
                                if (srError != null) {
                                    transaction.Rollback();
                                    error = srError;
                                    return 0;
                                }
                            }
                            #endregion
                        }
                        else {
                            #region Other Payment Modes Like Cash/Card/Cheque/DD/E-payment/Scheme Adjustment

                            if (sales.PayableAmt < 0 && pvm.PayMode == "C") {
                                ErrorVM payValError = new ErrorVM();
                                bool validation = PayToCustomerValidation(Convert.ToDecimal(pvm.PayAmount), pvm.CompanyCode, pvm.BranchCode, out payValError);
                                if (!validation) {
                                    error = payValError;
                                    return 0;
                                }
                            }

                            #region When the paymode is Scheme, we should get the bonus amount and amount received separately & then update the bill number to chit table.
                            if (pvm.PayMode == "CT") {
                                int schemeMembershipNo = Convert.ToInt32(pvm.RefBillNo);
                                CHTU_CHIT_CLOSURE cc = db.CHTU_CHIT_CLOSURE.Where(ccl => ccl.Scheme_Code == pvm.SchemeCode
                                                && ccl.Group_Code == pvm.GroupCode && ccl.Closed_At == pvm.CTBranch
                                                && ccl.Chit_MembShipNo == schemeMembershipNo).FirstOrDefault();
                                if (cc != null) {
                                    pvm.AmtReceived = cc.Amt_Received;
                                    pvm.BonusAmt = cc.Bonus_Amt;
                                    cc.Bill_Number = salesMaster.bill_no.ToString();
                                    cc.Bill_Date = appDate;
                                    cc.UpdateOn = SIGlobals.Globals.GetDateTime();
                                    db.Entry(cc).State = System.Data.Entity.EntityState.Modified;
                                }
                            }
                            #endregion

                            kpd.obj_id = objectID;
                            kpd.company_code = pvm.CompanyCode;
                            kpd.branch_code = pvm.BranchCode;
                            kpd.series_no = salesMaster.bill_no;
                            kpd.receipt_no = 0;
                            kpd.sno = paySlNo;
                            kpd.trans_type = "S";
                            kpd.pay_mode = pvm.PayMode;
                            kpd.pay_details = pvm.PayDetails;
                            kpd.pay_date = appDate;
                            kpd.pay_amt = pvm.PayAmount;
                            kpd.Ref_BillNo = pvm.RefBillNo == null ? "0" : pvm.RefBillNo;
                            kpd.party_code = pvm.PartyCode;
                            kpd.bill_counter = pvm.BillCounter;
                            kpd.is_paid = "N";
                            if (pvm.PayMode == "R") {
                                kpd.bank = SIGlobals.Globals.GetBank(db, pvm.Bank, pvm.PayMode, pvm.CompanyCode, pvm.BranchCode);
                                kpd.cheque_no = pvm.Bank;
                            }
                            else {
                                kpd.bank = pvm.Bank == null ? "" : pvm.Bank;
                                kpd.cheque_no = pvm.ChequeNo == null ? Convert.ToString(0) : pvm.ChequeNo;
                            }
                            kpd.bank_name = pvm.BankName == null ? "" : pvm.BankName;
                            kpd.cheque_date = pvm.ChequeDate == null ? SIGlobals.Globals.GetApplicationDate(pvm.CompanyCode, pvm.BranchCode) : pvm.ChequeDate;
                            kpd.card_type = pvm.CardType == null ? "" : pvm.CardType;
                            kpd.expiry_date = pvm.ExpiryDate == null ? SIGlobals.Globals.GetApplicationDate(pvm.CompanyCode, pvm.BranchCode) : pvm.ExpiryDate;
                            kpd.cflag = "N";
                            kpd.card_app_no = pvm.CardAppNo == null ? "" : pvm.CardAppNo;
                            kpd.scheme_code = pvm.SchemeCode;
                            kpd.sal_bill_type = pvm.SalBillType;
                            kpd.operator_code = pvm.OperatorCode;
                            kpd.session_no = pvm.SessionNo == null ? 0 : pvm.SessionNo;
                            kpd.UpdateOn = SIGlobals.Globals.GetDateTime();
                            kpd.group_code = pvm.GroupCode;
                            kpd.amt_received = pvm.AmtReceived == null ? 0 : pvm.AmtReceived;
                            kpd.bonus_amt = pvm.BonusAmt == null ? 0 : pvm.BonusAmt;
                            kpd.win_amt = pvm.WinAmt == null ? 0 : pvm.WinAmt;
                            kpd.ct_branch = pvm.CTBranch;
                            kpd.fin_year = finYear;
                            kpd.CardCharges = pvm.CardCharges == null ? 0 : pvm.CardCharges;

                            kpd.New_Bill_No = pvm.NewBillNo;
                            kpd.Add_disc = pvm.AddDisc == null ? 0 : pvm.AddDisc;
                            kpd.isOrdermanual = "N";
                            kpd.currency_value = pvm.CurrencyValue == null ? 0 : pvm.CurrencyValue;
                            kpd.exchange_rate = pvm.ExchangeRate == null ? 0 : pvm.ExchangeRate;
                            kpd.currency_type = pvm.CurrencyType == null ? "INR" : pvm.CurrencyType;
                            kpd.tax_percentage = pvm.TaxPercentage == null ? 0 : pvm.TaxPercentage;
                            kpd.cancelled_by = pvm.CancelledBy == null ? "" : pvm.CancelledBy;
                            kpd.cancelled_remarks = pvm.CancelledRemarks == null ? "" : pvm.CancelledRemarks;
                            kpd.cancelled_date = pvm.CancelledDate == null ? SIGlobals.Globals.GetApplicationDate(pvm.CompanyCode, pvm.BranchCode).ToString() : pvm.CancelledDate;
                            kpd.isExchange = pvm.IsExchange == null ? "N" : pvm.IsExchange;
                            kpd.exchangeNo = pvm.ExchangeNo == null ? 0 : pvm.ExchangeNo;
                            kpd.new_receipt_no = pvm.NewReceiptNo == null ? Convert.ToString(0) : pvm.NewReceiptNo;
                            kpd.Gift_Amount = pvm.GiftAmount == null ? 0 : pvm.GiftAmount; ;
                            kpd.cardSwipedBy = pvm.CardSwipedBy == null ? "" : pvm.CardSwipedBy; ;
                            kpd.version = pvm.Version == null ? 0 : pvm.Version; ;
                            kpd.GSTGroupCode = pvm.GSTGroupCode;
                            kpd.SGST_Percent = pvm.SGSTPercent == null ? 0 : pvm.SGSTPercent;
                            kpd.CGST_Percent = pvm.CGSTPercent == null ? 0 : pvm.CGSTPercent;
                            kpd.IGST_Percent = pvm.IGSTPercent == null ? 0 : pvm.IGSTPercent;
                            kpd.HSN = pvm.HSN;
                            kpd.SGST_Amount = pvm.SGSTAmount == null ? 0 : pvm.SGSTAmount;
                            kpd.CGST_Amount = pvm.CGSTAmount == null ? 0 : pvm.CGSTAmount;
                            kpd.IGST_Amount = pvm.IGSTAmount == null ? 0 : pvm.IGSTAmount;
                            kpd.pay_amount_before_tax = pvm.PayAmount;
                            kpd.pay_tax_amount = pvm.PayTaxAmount;
                            kpd.UniqRowID = Guid.NewGuid();
                            db.KTTU_PAYMENT_DETAILS.Add(kpd);
                            #endregion
                        }
                        paySlNo = paySlNo + 1;
                    }
                    #endregion

                    #region Order from Billing
                    // Generate Order from Billing When Payable Amount.
                    if ((sales.PayableAmt * -1) > 0) {
                        if (sales.lstOfPayToCustomer.Count == 0 && sales.IsOrder) {
                            decimal orderAmountBySales = sales.PayableAmt * -1;
                            ErrorVM orderError = new ErrorVM();

                            #region Creating Order

                            #region Order Master Details
                            OrderMasterVM om = new OrderMasterVM();
                            om.ObjID = "";
                            om.CompanyCode = sales.CompanyCode;
                            om.BranchCode = sales.BranchCode;
                            om.OrderNo = 0;
                            om.CustID = Convert.ToInt32(salesMaster.cust_Id);
                            om.CustName = salesMaster.cust_name;
                            om.OrderType = "A";
                            om.Remarks = "Order Created Through Credit Note";
                            om.OrderDate = SIGlobals.Globals.GetApplicationDate(sales.CompanyCode, sales.BranchCode);
                            om.OperatorCode = sales.OperatorCode;
                            om.SalCode = ksem.operator_code;
                            om.DeliveryDate = SIGlobals.Globals.GetApplicationDate(sales.CompanyCode, sales.BranchCode);
                            om.OrderRateType = "Delivery";
                            om.gsCode = ksem.gstype;
                            om.Rate = 0;// ksem.rate; //for delivery order
                            om.AdvacnceOrderAmount = orderAmountBySales;
                            om.GrandTotal = orderAmountBySales;
                            om.ObjectStatus = "A";
                            om.BillNo = 0;
                            om.CFlag = "N";
                            om.CancelledBy = "";
                            om.BillCounter = "";
                            om.UpdateOn = SIGlobals.Globals.GetDateTime();
                            om.ClosedBranch = "";
                            om.ClosedFlag = "N";
                            om.ClosedBy = "";
                            om.ClosedDate = SIGlobals.Globals.GetApplicationDate(sales.CompanyCode, sales.BranchCode);
                            om.Karat = ksem.karat;
                            om.OrderDayRate = ksem.rate;
                            om.NewBillNo = "";
                            om.TaxPercentage = 0;
                            om.TotalAmountBeforeTax = 0;
                            om.TotalTaxAmount = 0;
                            om.ShiftID = 0;
                            om.IsPAN = "N";
                            om.OldOrderNo = null;
                            om.Address1 = salesMaster.address1;
                            om.Address2 = salesMaster.address2;
                            om.Address3 = salesMaster.address3;
                            om.City = salesMaster.city;
                            om.PinCode = salesMaster.pin_code;
                            om.MobileNo = salesMaster.mobile_no;
                            om.State = salesMaster.state;
                            om.StateCode = salesMaster.state_code;
                            om.TIN = salesMaster.tin;
                            om.PANNo = salesMaster.pan_no;
                            om.ESTNo = 0;// ksem.est_no;
                            om.IDType = "";
                            om.IDDetails = "";
                            om.BookingType = "N";//ksem.booking_type;
                            om.OrderAdvanceRateType = "N";
                            om.PhoneNo = "";
                            om.EmailID = "";
                            om.Salutation = salesMaster.salutation;

                            //TODO: Please transfer the following fields when billing is implemented.
                            //om.OrderAdvanceRateType = ksem.booking_type;
                            //om.AdvancePercent = ksem.adv_percent;
                            //om.RateValidityTill = ksem.rate_fixed_till;

                            #endregion

                            #region Order Details
                            List<OrderItemDetailsVM> lstOfOrderDetils = new List<OrderItemDetailsVM>();
                            OrderItemDetailsVM oid = new OrderItemDetailsVM();
                            oid.ObjID = "";
                            oid.CompanyCode = sales.CompanyCode;
                            oid.BranchCode = sales.BranchCode;
                            oid.OrderNo = 0;
                            oid.SlNo = 1;
                            oid.ItemName = "Customer Order";
                            oid.Quantity = 0;
                            oid.Description = "Default Order Created through Credit Note";
                            oid.FromGrossWt = 0;
                            oid.ToGrossWt = 0;
                            oid.MCPer = 0;
                            oid.WastagePercent = 0;
                            oid.Amount = orderAmountBySales;
                            oid.SFlag = "N";
                            oid.SParty = "";
                            oid.BillNo = 0;
                            oid.ImgID = "";
                            oid.UpdateOn = SIGlobals.Globals.GetDateTime();
                            oid.VAPercent = 0;
                            oid.ItemAmount = orderAmountBySales;
                            oid.GSCode = "";
                            oid.FinYear = SIGlobals.Globals.GetFinancialYear(db, sales.CompanyCode, sales.BranchCode);
                            oid.ItemwiseTaxPercentage = 0;
                            oid.ItemwiseTaxAmount = 0;
                            oid.MCPerPiece = 0;
                            oid.ProductID = "";
                            oid.IsIssued = "";
                            oid.CounterCode = "";
                            oid.SalCode = "";
                            oid.MCPercent = 0;
                            oid.MCType = 0;
                            oid.EstNo = 0;
                            lstOfOrderDetils.Add(oid);
                            om.lstOfOrderItemDetailsVM = lstOfOrderDetils;
                            #endregion

                            #region Payment Details
                            List<PaymentVM> lstOfPayment = new List<PaymentVM>();
                            PaymentVM payment = new PaymentVM();
                            payment.ObjID = "";
                            payment.CompanyCode = sales.CompanyCode;
                            payment.BranchCode = sales.BranchCode;
                            payment.SeriesNo = 0;
                            payment.ReceiptNo = 0;
                            payment.SNo = 1;
                            payment.TransType = "O";
                            payment.PayMode = "CN";
                            payment.PayDetails = "Order Created Through Bill";
                            payment.PayDate = SIGlobals.Globals.GetApplicationDate(sales.CompanyCode, sales.BranchCode);
                            payment.PayAmount = orderAmountBySales;
                            payment.RefBillNo = Convert.ToString(billNo);
                            payment.PartyCode = "";
                            payment.BillCounter = "";
                            payment.IsPaid = "N";
                            payment.Bank = "";
                            payment.BankName = "";
                            payment.ChequeDate = SIGlobals.Globals.GetApplicationDate(sales.CompanyCode, sales.BranchCode);
                            payment.CardType = "";
                            payment.ExpiryDate = SIGlobals.Globals.GetApplicationDate(sales.CompanyCode, sales.BranchCode);
                            payment.CFlag = "N";
                            payment.CardAppNo = "";
                            payment.SchemeCode = "";
                            payment.SalBillType = null;
                            payment.OperatorCode = sales.OperatorCode;
                            payment.SessionNo = 0;
                            payment.UpdateOn = SIGlobals.Globals.GetDateTime();
                            payment.GroupCode = null;
                            payment.AmtReceived = 0;
                            payment.BonusAmt = 0;
                            payment.WinAmt = 0;
                            payment.CTBranch = null;
                            payment.FinYear = SIGlobals.Globals.GetFinancialYear(db, sales.CompanyCode, sales.BranchCode);
                            payment.CardCharges = 0;
                            payment.ChequeNo = "0";
                            payment.NewBillNo = "";
                            payment.AddDisc = 0;
                            payment.IsOrderManual = "N";
                            payment.CurrencyValue = 0;
                            payment.ExchangeRate = 0;
                            payment.CurrencyType = null;
                            payment.TaxPercentage = 0;
                            payment.CancelledBy = "";
                            payment.CancelledRemarks = "";
                            payment.CancelledDate = Convert.ToString(SIGlobals.Globals.GetApplicationDate(sales.CompanyCode, sales.BranchCode));
                            payment.IsExchange = "N";
                            payment.ExchangeNo = 0;
                            payment.NewReceiptNo = "0";
                            payment.GiftAmount = 0;
                            payment.CardSwipedBy = "";
                            payment.Version = 0;
                            payment.GSTGroupCode = null;
                            payment.SGSTPercent = 0;
                            payment.CGSTPercent = 0;
                            payment.IGSTPercent = 0;
                            payment.HSN = null;
                            payment.SGSTAmount = 0;
                            payment.CGSTAmount = 0;
                            payment.IGSTAmount = 0;
                            payment.PayAmountBeforeTax = sales.PayableAmt;
                            payment.PayTaxAmount = 0;
                            lstOfPayment.Add(payment);
                            om.lstOfPayment = lstOfPayment;
                            #endregion

                            #endregion

                            #region Save

                            new OrderBL().SaveOrderInfoWithTran(om, out genOrderNo, out genReceiptNo, out orderError);
                            if (orderError != null) {
                                transaction.Rollback();
                                error = orderError;
                                return 0;
                            }
                            int retOrderNo = genOrderNo;
                            db.SaveChanges();
                            #endregion

                            #region Updation 
                            // After generating Order Updating OrderNumber to that order master and payment details.
                            KTTU_ORDER_MASTER updOrderMaster = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == retOrderNo && ord.company_code == sales.CompanyCode && ord.branch_code == sales.BranchCode).FirstOrDefault();
                            updOrderMaster.New_Bill_No = Convert.ToString(genOrderNo);
                            db.Entry(updOrderMaster).State = System.Data.Entity.EntityState.Modified;

                            KTTU_PAYMENT_DETAILS updPayment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == retOrderNo && pay.company_code == sales.CompanyCode && pay.branch_code == sales.BranchCode).FirstOrDefault();
                            updPayment.New_Bill_No = Convert.ToString(genOrderNo);
                            db.Entry(updPayment).State = System.Data.Entity.EntityState.Modified;
                            #endregion

                            #region Add Order as payment mode
                            KTTU_PAYMENT_DETAILS opd = new KTTU_PAYMENT_DETAILS();
                            opd.obj_id = objectID;
                            opd.company_code = sales.CompanyCode;
                            opd.branch_code = sales.BranchCode;
                            opd.series_no = billNo;
                            opd.receipt_no = 0;
                            opd.sno = paySlNo;
                            opd.trans_type = "PC"; ;
                            opd.pay_mode = "OP";
                            opd.pay_details = "Adjusted Towards Order";
                            opd.pay_date = appDate;
                            opd.pay_amt = orderAmountBySales;
                            opd.Ref_BillNo = retOrderNo.ToString();
                            opd.party_code = "";
                            opd.bill_counter = "FF";
                            opd.is_paid = "N";
                            opd.bank = "";
                            opd.cheque_no = "0";
                            opd.bank_name = "";
                            opd.cheque_date = appDate;
                            opd.card_type = "";
                            opd.expiry_date = appDate;
                            opd.cflag = "N";
                            opd.card_app_no = "";
                            opd.scheme_code = "";
                            opd.sal_bill_type = null;
                            opd.operator_code = sales.OperatorCode;
                            opd.session_no = 0;
                            opd.UpdateOn = SIGlobals.Globals.GetDateTime();
                            opd.group_code = "";
                            opd.amt_received = 0;
                            opd.bonus_amt = 0;
                            opd.win_amt = 0;
                            opd.ct_branch = null;
                            opd.fin_year = finYear;
                            opd.CardCharges = 0;
                            opd.New_Bill_No = billNo.ToString();
                            opd.Add_disc = 0;
                            opd.isOrdermanual = "N";
                            opd.currency_value = 0;
                            opd.exchange_rate = 0;
                            opd.currency_type = "INR";
                            opd.tax_percentage = 0;
                            opd.cancelled_by = "";
                            opd.cancelled_remarks = "";
                            opd.cancelled_date = "2000-01-01";
                            opd.isExchange = "N";
                            opd.exchangeNo = 0;
                            opd.new_receipt_no = "0";
                            opd.Gift_Amount = 0;
                            opd.cardSwipedBy = "";
                            opd.version = 0;
                            opd.GSTGroupCode = null;
                            opd.SGST_Percent = 0;
                            opd.CGST_Percent = 0;
                            opd.IGST_Percent = 0;
                            opd.HSN = null;
                            opd.SGST_Amount = 0;
                            opd.CGST_Amount = 0;
                            opd.IGST_Amount = 0;
                            opd.UniqRowID = Guid.NewGuid();
                            db.KTTU_PAYMENT_DETAILS.Add(opd);
                            ++paySlNo;
                            #endregion

                        }
                    }

                    #endregion

                    #endregion

                    #region Payment Information: Estimation Level Attachments Like Purchae Estimation / Order / Sales Return Estimations
                    // Update to Sales Estimation
                    KTTU_SALES_EST_MASTER updEstMaster = db.KTTU_SALES_EST_MASTER.Where(sem => sem.est_no == sales.EstNo
                                                                                && sem.company_code == sales.CompanyCode
                                                                                && sem.branch_code == sales.BranchCode).FirstOrDefault();
                    updEstMaster.bill_no = salesMaster.bill_no;
                    db.Entry(updEstMaster).State = System.Data.Entity.EntityState.Modified;

                    //Update to Barcode Master
                    foreach (KTTU_SALES_EST_DETAILS sed in ksed) {
                        KTTU_BARCODE_MASTER barcode = db.KTTU_BARCODE_MASTER.Where(bar => bar.barcode_no == sed.barcode_no
                                                                                    && bar.company_code == sed.company_code
                                                                                    && bar.branch_code == sed.branch_code).FirstOrDefault();
                        if (barcode != null && barcode.barcode_no != "" && barcode.Tagging_Type != "G") {
                            barcode.sold_flag = "Y";
                            barcode.ExitDate = SIGlobals.Globals.GetApplicationDate(sales.CompanyCode, sales.BranchCode);
                            barcode.ExitDocNo = salesMaster.bill_no.ToString();
                            barcode.ExitDocType = "S";
                            barcode.UpdateOn = SIGlobals.Globals.GetDateTime();
                            db.Entry(barcode).State = System.Data.Entity.EntityState.Modified;
                        }

                        // If Tag is Grouped Barcode (Sundry Item) Then No need to set the flag "Y". And Update the Sundry stock;
                        if (barcode != null && barcode.barcode_no != "" && barcode.Tagging_Type == "G") {
                            barcode.sold_flag = "N";
                            barcode.gwt = barcode.gwt - sed.gwt;
                            barcode.swt = barcode.swt - sed.swt;
                            barcode.nwt = barcode.nwt - sed.nwt;
                            barcode.UpdateOn = SIGlobals.Globals.GetDateTime();
                            db.Entry(barcode).State = System.Data.Entity.EntityState.Modified;
                        }
                    }

                    //Checking is there any Attachements
                    List<KTTU_PAYMENT_DETAILS> lstOfAttachments = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == sales.EstNo
                                                                                                && pay.company_code == sales.CompanyCode
                                                                                                && pay.branch_code == sales.BranchCode
                                                                                                && pay.cflag != "Y").ToList();
                    foreach (KTTU_PAYMENT_DETAILS pay in lstOfAttachments) {

                        //Checking Order Attachment and Updating Bill No
                        if (pay.trans_type == "A" && pay.pay_mode == "OP") {
                            #region Attached Order
                            int orderNo = Convert.ToInt32(pay.Ref_BillNo);
                            bool generated = GenerateOrderAsPayment(db, orderNo, billNo, appDate, objectID, paySlNo, finYear, sales.OperatorCode, pay.company_code, pay.branch_code);
                            paySlNo = paySlNo + 1;
                            #endregion
                        }
                        else if (pay.trans_type == "A" && pay.pay_mode == "PE") {
                            #region Attached  Purchase Estimation
                            int purchaseEstNo = Convert.ToInt32(pay.Ref_BillNo);
                            KTTU_PURCHASE_EST_MASTER attachedEst = db.KTTU_PURCHASE_EST_MASTER.Where(pur => pur.est_no == purchaseEstNo
                                                                                                        && pur.company_code == sales.CompanyCode
                                                                                                        && pur.branch_code == sales.BranchCode).FirstOrDefault();
                            if (attachedEst != null) {
                                ErrorVM purchaseError = new ErrorVM();
                                PurchaseBillingVM pb = new PurchaseBillingVM();
                                pb.CompanyCode = sales.CompanyCode;
                                pb.BranchCode = sales.BranchCode;
                                pb.EstNo = Convert.ToInt32(pay.Ref_BillNo);
                                pb.BillCounter = sales.BillCounter;
                                pb.OperatorCode = sales.OperatorCode;
                                pb.Type = "SE";
                                pb.BillNo = billNo;
                                pb.PaidAmount = Convert.ToDecimal(attachedEst.total_purchase_amount);
                                pb.ObjID = objectID;
                                pb.SlNo = paySlNo;
                                int purchaseBillNo = new PurchaseBillingBL(db).SavePurchaseBillWithTxn(pb, out purchaseError);
                                if (purchaseError != null) {
                                    transaction.Rollback();
                                    error = purchaseError;
                                    return 0;
                                }
                            }
                            paySlNo = paySlNo + 1;
                            #endregion
                        }
                        else if (pay.trans_type == "A" && pay.pay_mode == "SE") {
                            #region Attached Sales Return
                            int srEstNo = Convert.ToInt32(pay.Ref_BillNo);
                            KTTU_SR_MASTER attachedSR = db.KTTU_SR_MASTER.Where(sr => sr.est_no == srEstNo
                                                                                                        && sr.company_code == sales.CompanyCode
                                                                                                        && sr.branch_code == sales.BranchCode).FirstOrDefault();
                            if (attachedSR != null) {
                                ErrorVM srError = new ErrorVM();
                                ConfirmSalesReturnVM pb = new ConfirmSalesReturnVM();
                                pb.CompanyCode = sales.CompanyCode;
                                pb.BranchCode = sales.BranchCode;
                                pb.EstNo = Convert.ToInt32(pay.Ref_BillNo);
                                pb.BillCounter = sales.BillCounter;
                                pb.OperatorCode = sales.OperatorCode;
                                pb.Type = "SE";
                                pb.BillNo = billNo;
                                pb.PaidAmount = Convert.ToDecimal(attachedSR.total_sramount);
                                pb.ObjectID = objectID;
                                pb.SlNo = paySlNo;
                                ConfirmVM confirm = new ConfirmSalesReturnBL(db).ConfirmSalesReturnWithTxn(pb, out srError);
                                if (srError != null) {
                                    transaction.Rollback();
                                    error = srError;
                                    return 0;
                                }
                                int purchaseBillNo = confirm.SalesBillNo;
                            }
                            paySlNo = paySlNo + 1;
                            #endregion
                        }
                    }
                    #endregion

                    #region PayToCustomer
                    List<PaymentVM> lstOfPayToCustomer = sales.lstOfPayToCustomer;
                    if (lstOfPayToCustomer != null && lstOfPayToCustomer.Count > 0 && sales.IsOrder == false) {
                        foreach (PaymentVM pvm in lstOfPayToCustomer) {
                            KTTU_PAYMENT_DETAILS kpd = new KTTU_PAYMENT_DETAILS();
                            kpd.obj_id = objectID;
                            kpd.company_code = pvm.CompanyCode;
                            kpd.branch_code = pvm.BranchCode;
                            kpd.series_no = salesMaster.bill_no;
                            kpd.receipt_no = 0;
                            kpd.sno = paySlNo;
                            kpd.trans_type = "PC";
                            kpd.pay_mode = pvm.PayMode;
                            kpd.pay_details = pvm.PayDetails;
                            kpd.pay_date = SIGlobals.Globals.GetApplicationDate(pvm.CompanyCode, pvm.BranchCode);
                            kpd.pay_amt = pvm.PayAmount;
                            kpd.Ref_BillNo = pvm.RefBillNo == null ? "0" : pvm.RefBillNo;
                            kpd.party_code = pvm.PartyCode;
                            kpd.bill_counter = pvm.BillCounter;
                            kpd.is_paid = "N";
                            kpd.bank = pvm.Bank == null ? "" : pvm.Bank;
                            kpd.bank_name = pvm.BankName == null ? "" : pvm.BankName; ;
                            kpd.cheque_date = pvm.ChequeDate == null ? SIGlobals.Globals.GetApplicationDate(pvm.CompanyCode, pvm.BranchCode) : pvm.ChequeDate;
                            kpd.card_type = pvm.CardType == null ? "" : pvm.CardType;
                            kpd.expiry_date = pvm.ExpiryDate == null ? SIGlobals.Globals.GetApplicationDate(pvm.CompanyCode, pvm.BranchCode) : pvm.ExpiryDate;
                            kpd.cflag = "N";
                            kpd.card_app_no = pvm.CardAppNo == null ? "" : pvm.CardAppNo;
                            kpd.scheme_code = pvm.SchemeCode;
                            kpd.sal_bill_type = pvm.SalBillType;
                            kpd.operator_code = pvm.OperatorCode;
                            kpd.session_no = pvm.SessionNo == null ? 0 : pvm.SessionNo;
                            kpd.UpdateOn = SIGlobals.Globals.GetDateTime();
                            kpd.group_code = pvm.GroupCode;
                            kpd.amt_received = pvm.AmtReceived == null ? 0 : pvm.AmtReceived;
                            kpd.bonus_amt = pvm.BonusAmt == null ? 0 : pvm.BonusAmt;
                            kpd.win_amt = pvm.WinAmt == null ? 0 : pvm.WinAmt;
                            kpd.ct_branch = pvm.CTBranch;
                            kpd.fin_year = finYear;
                            kpd.CardCharges = pvm.CardCharges == null ? 0 : pvm.CardCharges;
                            kpd.cheque_no = pvm.ChequeNo == null ? Convert.ToString(0) : pvm.ChequeNo;
                            kpd.New_Bill_No = pvm.NewBillNo;
                            kpd.Add_disc = pvm.AddDisc == null ? 0 : pvm.AddDisc;
                            kpd.isOrdermanual = "N";
                            kpd.currency_value = pvm.CurrencyValue == null ? 0 : pvm.CurrencyValue;
                            kpd.exchange_rate = pvm.ExchangeRate == null ? 0 : pvm.ExchangeRate;
                            kpd.currency_type = pvm.CurrencyType == null ? "INR" : pvm.CurrencyType;
                            kpd.tax_percentage = pvm.TaxPercentage == null ? 0 : pvm.TaxPercentage;
                            kpd.cancelled_by = pvm.CancelledBy == null ? "" : pvm.CancelledBy;
                            kpd.cancelled_remarks = pvm.CancelledRemarks == null ? "" : pvm.CancelledRemarks;
                            kpd.cancelled_date = pvm.CancelledDate == null ? SIGlobals.Globals.GetApplicationDate(pvm.CompanyCode, pvm.BranchCode).ToString() : pvm.CancelledDate;
                            kpd.isExchange = pvm.IsExchange == null ? "N" : pvm.IsExchange;
                            kpd.exchangeNo = pvm.ExchangeNo == null ? 0 : pvm.ExchangeNo;
                            kpd.new_receipt_no = pvm.NewReceiptNo == null ? Convert.ToString(0) : pvm.NewReceiptNo;
                            kpd.Gift_Amount = pvm.GiftAmount == null ? 0 : pvm.GiftAmount; ;
                            kpd.cardSwipedBy = pvm.CardSwipedBy == null ? "" : pvm.CardSwipedBy; ;
                            kpd.version = pvm.Version == null ? 0 : pvm.Version; ;
                            kpd.GSTGroupCode = pvm.GSTGroupCode;
                            kpd.SGST_Percent = pvm.SGSTPercent == null ? 0 : pvm.SGSTPercent;
                            kpd.CGST_Percent = pvm.CGSTPercent == null ? 0 : pvm.CGSTPercent;
                            kpd.IGST_Percent = pvm.IGSTPercent == null ? 0 : pvm.IGSTPercent;
                            kpd.HSN = pvm.HSN;
                            kpd.SGST_Amount = pvm.SGSTAmount == null ? 0 : pvm.SGSTAmount;
                            kpd.CGST_Amount = pvm.CGSTAmount == null ? 0 : pvm.CGSTAmount;
                            kpd.IGST_Amount = pvm.IGSTAmount == null ? 0 : pvm.IGSTAmount;
                            kpd.pay_amount_before_tax = pvm.PayAmount;
                            kpd.pay_tax_amount = pvm.PayTaxAmount;
                            kpd.UniqRowID = Guid.NewGuid();
                            db.KTTU_PAYMENT_DETAILS.Add(kpd);
                            paySlNo = paySlNo + 1;
                        }
                    }
                    #endregion

                    #region Post Sales Stock
                    db.SaveChanges();
                    string errorMessage = string.Empty;
                    if (!SalesStockPost(db, sales.CompanyCode, sales.BranchCode, billNo, false, out errorMessage)) {
                        error = new ErrorVM()
                        {
                            field = "Failure in stock posting.",
                            index = 0,
                            description = errorMessage,
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        transaction.Rollback();
                        return 0;
                    }
                    #endregion

                    #region Account Updates
                    db.SaveChanges();
                    // After doing db.SaveChanges() only we can get Issue Number in the db to do account posting.
                    // Account Update
                    ErrorVM procError = AccountPostingWithProedure(db, sales.CompanyCode, sales.BranchCode, salesMaster.bill_no);
                    if (procError != null) {
                        error = new ErrorVM()
                        {
                            field = "Account Update",
                            index = 0,
                            description = procError.description,
                            ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError
                        };
                        transaction.Rollback();
                        return 0;
                    }
                    #endregion

                    #region Updating Serial No
                    SIGlobals.Globals.UpdateSeqenceNumber(db, MODULE_SEQ_NO, sales.CompanyCode, sales.BranchCode);
                    #endregion

                    #region Remove Stock Taking Entries if any
                    if (!DeleteStockTakingEntries(db, sales.CompanyCode, sales.BranchCode, billNo, out errorMessage)) {
                    }
                    #endregion

                    #region Save
                    db.SaveChanges();
                    transaction.Commit();
                    #endregion

                    #region Document Creation Posting, error will not be checked
                    //Post to DocumentCreationLog Table
                    DateTime billDate = SIGlobals.Globals.GetApplicationDate(sales.CompanyCode, sales.BranchCode);
                    new Common.CommonBL().PostDocumentCreation(sales.CompanyCode, sales.BranchCode, 1, billNo, billDate, sales.OperatorCode);
                    #endregion

                    #region Updating Marketplace
                    bool updated = new Common.MarketplaceBL().UpdateMarketplaceInventory(sales.CompanyCode,
                                        sales.BranchCode, billNo, SIGlobals.TransactionsType.Sales, SIGlobals.ActionType.Block, out error);
                    error = null;
                    #endregion

                    return salesMaster.bill_no;
                }
                catch (Exception excp) {
                    transaction.Rollback();
                    error = new ErrorVM().GetErrorDetails(excp);
                    return 0;
                }
            }
        }

        private bool ValidateSalesBillObject(SalesBillingVM sales, out string errorMessage)
        {
            errorMessage = "All good so far";
            bool revalidateEstimateValuesInBilling = false;
            var configValue = System.Configuration.ConfigurationManager.AppSettings["RevalidateEstimateValuesInBilling"];
            if (configValue != null) {
                if (configValue.ToUpper() == "TRUE")
                    revalidateEstimateValuesInBilling = true;
            }

            if (!revalidateEstimateValuesInBilling)
                return true;

            try {
                #region Sales Totals

                #region Since difference discount amount is no more used, this code is commented.
                //var salesLineTotals = (from sed in db.KTTU_SALES_EST_DETAILS
                //                       where sed.company_code == sales.CompanyCode
                //                             && sed.branch_code == sales.BranchCode && sed.est_no == sales.EstNo
                //                       group sed by 1 into g
                //                       select new
                //                       {
                //                           ItemTotal = g.Sum(x => x.total_amount),
                //                           CGSTAmount = g.Sum(x => x.CGST_Amount),
                //                           SGSTAmount = g.Sum(x => x.SGST_Amount),
                //                           IGSTAmount = g.Sum(x => x.IGST_Amount),
                //                           CessAmount = g.Sum(x => x.CESSAmount),
                //                           GSTAmt = (g.Sum(x => x.CGST_Amount))
                //                             + (g.Sum(x => x.SGST_Amount))
                //                             + (g.Sum(x => x.IGST_Amount))
                //                             + (g.Sum(x => x.CESSAmount)),
                //                           ItemFinalAmt = g.Sum(x => x.item_final_amount),
                //                           MCDiscountAmt = g.Sum(x => x.Mc_Discount_Amt),
                //                           OrderDiscountAmt = g.Sum(x => x.Ord_Discount_Amt),
                //                           OfferDiscountAmt = g.Sum(x => x.offer_value),
                //                           AdditionalDiscountAmt = g.Sum(x => x.item_additional_discount),
                //                           TotalDiscountAmt = (g.Sum(x => x.item_additional_discount))
                //                             + (g.Sum(x => x.Mc_Discount_Amt))
                //                             + (g.Sum(x => x.Ord_Discount_Amt))
                //                             + (g.Sum(x => x.offer_value)),
                //                           GSTAmtOriginal = (g.Sum(x => x.CGST_Percent * x.total_amount / 100))
                //                             + (g.Sum(x => x.SGST_Percent * x.total_amount / 100))
                //                             + (g.Sum(x => x.IGST_Percent * x.total_amount / 100))
                //                             + (g.Sum(x => x.CESSPercent * x.total_amount / 100))
                //                       }).FirstOrDefault();
                //if (salesLineTotals == null) {
                //    errorMessage = "There is no estimation line detail.";
                //    return false;
                //}
                //sales.DifferenceDiscountAmt = 0;
                //decimal itemTotal = Convert.ToDecimal(salesLineTotals.ItemTotal);
                //decimal gstAmountBeforeDiscounts = Math.Round(Convert.ToDecimal(salesLineTotals.GSTAmtOriginal), 2);
                //decimal gstPercent = gstAmountBeforeDiscounts / itemTotal;

                //decimal discountAmount = Math.Round(sales.DifferenceDiscountAmt / (1 + gstPercent), 2);
                //decimal itemTotalAfterAdditionalDiscount = itemTotal - discountAmount;
                //decimal itemTotalAfterOfferDiscount = itemTotalAfterAdditionalDiscount - Convert.ToDecimal(salesLineTotals.OfferDiscountAmt);
                //decimal newGSTAmount = gstPercent * itemTotalAfterOfferDiscount;

                //decimal gstAmountExlCess = 0;
                //decimal gSTCessAmt = 0;
                //decimal discountPercent = Math.Round(discountAmount / itemTotal * 100.00M, 6);
                //decimal offerDiscountPercent = Math.Round(Convert.ToDecimal(salesLineTotals.OfferDiscountAmt) / itemTotal * 100.00M, 6);
                //decimal totalDiscountPercent = discountPercent + offerDiscountPercent; 
                #endregion

                var salesLineFinalAmt = db.KTTU_SALES_EST_DETAILS.Where(x => x.company_code == sales.CompanyCode
                    && x.branch_code == sales.BranchCode && x.est_no == sales.EstNo).Sum(y => y.item_final_amount);
                if (salesLineFinalAmt == null) {
                    errorMessage = "Failed to get sales lines. Error: " + errorMessage;
                    return false;
                }
                decimal salesAmtInclTax = Convert.ToDecimal(salesLineFinalAmt);
                #endregion

                #region Inline Order Amount
                decimal inlineOrderAmount = 0.00M;
                var salesEstMaster = db.KTTU_SALES_EST_MASTER.Where(x => x.company_code == sales.CompanyCode && x.branch_code == sales.BranchCode
                   && x.est_no == sales.EstNo).FirstOrDefault();
                if (salesEstMaster != null) {
                    int inlineOrderNo = salesEstMaster.order_no;
                    if (inlineOrderNo > 0) {
                        var ordTotals = db.KTTU_ORDER_MASTER.Where(x => x.company_code == sales.CompanyCode && x.branch_code == sales.BranchCode
                            && x.order_no == inlineOrderNo).FirstOrDefault();
                        if (ordTotals != null) {
                            if (ordTotals.bill_no != 0) {
                                errorMessage = string.Format("The order# {0} is already billed with invoice# {1}."
                                    , ordTotals.order_no.ToString(), ordTotals.bill_no.ToString());
                                return false;
                            }
                            inlineOrderAmount = Convert.ToDecimal(ordTotals.advance_ord_amount);
                        }
                    }
                }
                #endregion

                #region Inline Old Purchase Amount
                decimal inlinePurchaseLineAmount = 0.00M;
                var purEstMaster = db.KTTU_PURCHASE_EST_MASTER.Where(x => x.company_code == sales.CompanyCode && x.branch_code == sales.BranchCode
                        && x.est_no == sales.EstNo).FirstOrDefault();
                if (purEstMaster != null) {
                    if (purEstMaster.bill_no != 0) {
                        errorMessage = string.Format("The purchase estimation# {0} is already billed with invoice# {1}."
                                        , purEstMaster.est_no.ToString(), purEstMaster.bill_no.ToString());
                        return false;
                    }

                    var purchaseTotals = db.KTTU_PURCHASE_EST_DETAILS.Where(x => x.company_code == sales.CompanyCode && x.branch_code == sales.BranchCode
                            && x.est_no == sales.EstNo);
                    if (purchaseTotals != null) {
                        inlinePurchaseLineAmount = Convert.ToDecimal(purchaseTotals.Sum(x => x.item_amount));
                    }
                }
                #endregion

                #region Inline SR Amount
                decimal inlineSRLineAmount = 0.00M;
                var inlineSrEstMaster = db.KTTU_SR_MASTER.Where(x => x.company_code == sales.CompanyCode && x.branch_code == sales.BranchCode
                        && x.est_no == sales.EstNo).FirstOrDefault();
                if (inlineSrEstMaster != null) {
                    if (inlineSrEstMaster.sales_bill_no != 0) {
                        errorMessage = string.Format("The sales return estimation# {0} is already billed with invoice# {1}."
                                        , inlineSrEstMaster.est_no.ToString(), inlineSrEstMaster.sales_bill_no.ToString());
                        return false;
                    }

                    var srTotals = db.KTTU_SR_DETAILS.Where(x => x.company_code == sales.CompanyCode && x.branch_code == sales.BranchCode
                           && x.est_no == sales.EstNo);
                    if (srTotals != null) {
                        inlineSRLineAmount = Convert.ToDecimal(srTotals.Sum(x => x.item_final_amount));
                    }
                }
                #endregion

                #region Attached Document Amounts
                #region Cannot use this
                // Because KTTU_PAYMENT_DETAILS.Ref_BillNo is string column and all other document numbers are in, we cannot use the following query.
                //var attachedOrders = from pd in db.KTTU_PAYMENT_DETAILS
                //                     join ord in db.KTTU_ORDER_MASTER
                //                     on new { CC = pd.company_code, BC = pd.branch_code, DocNo = pd.Ref_BillNo }
                //                     equals new { CC = ord.company_code, BC = ord.branch_code, DocNo = ord.order_no.ToString() }
                //                     where pd.company_code == sales.CompanyCode && pd.branch_code == sales.BranchCode
                //                        && pd.series_no == sales.EstNo && pd.trans_type == "A" && pd.pay_mode == "OP"
                //                     select new { OrderNo = ord.order_no, Amount = ord.advance_ord_amount }; 
                #endregion

                decimal attachedOrderAmount = 0.00M;
                decimal attachedPurchaseAmount = 0.00M;
                decimal attachedSRAmount = 0.00M;
                var payInfo = db.KTTU_PAYMENT_DETAILS.Where(pd => pd.company_code == sales.CompanyCode && pd.branch_code == sales.BranchCode
                    && pd.series_no == sales.EstNo && pd.trans_type == "A" && pd.cflag != "Y");
                foreach (var pay in payInfo) {
                    int docNo = Convert.ToInt32(pay.Ref_BillNo);
                    switch (pay.pay_mode) {
                        case "OP":
                            var ordInfo = db.KTTU_ORDER_MASTER.Where(x => x.company_code == pay.company_code && x.branch_code == pay.branch_code
                                && x.order_no == docNo).FirstOrDefault();
                            if (ordInfo == null) {
                                errorMessage = string.Format("The attached order# {0} does not exist or is invalid.", docNo.ToString());
                                return false;
                            }
                            if (ordInfo.cflag == "Y") {
                                errorMessage = string.Format("The attached order# {0} is cancelled."
                                    , ordInfo.order_no.ToString());
                                return false;
                            }
                            if (ordInfo.bill_no != 0) {
                                errorMessage = string.Format("The attached order# {0} is already billed with invoice# {1}."
                                    , ordInfo.order_no.ToString(), ordInfo.bill_no.ToString());
                                return false;
                            }

                            attachedOrderAmount += Convert.ToDecimal(ordInfo.advance_ord_amount);

                            break;
                        case "PE":
                            var attachedPurEstMast = db.KTTU_PURCHASE_EST_MASTER.Where(x => x.company_code == sales.CompanyCode && x.branch_code == sales.BranchCode
                                && x.est_no == docNo).FirstOrDefault();
                            if (attachedPurEstMast == null) {
                                errorMessage = string.Format("The attached purchase estimation# {0} does not exist or is invalid.", docNo.ToString());
                                return false;
                            }
                            if (attachedPurEstMast.bill_no != 0) {
                                errorMessage = string.Format("The attached purchase estimation# {0} is already billed with invoice# {1}."
                                                , docNo.ToString(), attachedPurEstMast.bill_no.ToString());
                                return false;
                            }

                            var oldPurLineInfo = db.KTTU_PURCHASE_EST_DETAILS.Where(x => x.company_code == pay.company_code && x.branch_code == pay.branch_code
                                && x.est_no == docNo);
                            if (oldPurLineInfo != null) {
                                attachedPurchaseAmount += Convert.ToDecimal(oldPurLineInfo.Sum(s => s.item_amount));
                            }
                            break;
                        case "SE":
                            var attachedSREstMast = db.KTTU_SR_MASTER.Where(x => x.company_code == sales.CompanyCode && x.branch_code == sales.BranchCode
                                && x.est_no == docNo).FirstOrDefault();
                            if (attachedSREstMast == null) {
                                errorMessage = string.Format("The attached SR estimation# {0} does not exist or is invalid.", docNo.ToString());
                                return false;
                            }
                            if (attachedSREstMast.sales_bill_no != 0) {
                                errorMessage = string.Format("The attached SR estimation# {0} is already billed with invoice# {1}."
                                                , docNo.ToString(), attachedSREstMast.sales_bill_no.ToString());
                                return false;
                            }
                            var srInfo = db.KTTU_SR_DETAILS.Where(x => x.company_code == pay.company_code && x.branch_code == pay.branch_code
                                && x.est_no == docNo);
                            if (srInfo != null) {
                                attachedSRAmount += Convert.ToDecimal(srInfo.Sum(s => s.item_final_amount));
                            }
                            break;
                    }
                }
                #endregion

                if (!ValidateDocumentPaymentModes(sales.CompanyCode, sales.BranchCode, sales.lstOfPayment, out errorMessage)) {
                    return false;
                }

                decimal receiptAmountInJson = 0.00M;
                if (sales.lstOfPayment != null) {
                    foreach (var lp in sales.lstOfPayment)
                        receiptAmountInJson += Convert.ToDecimal(lp.PayAmount);
                }

                decimal paymentAmountInJson = 0.00M;
                if (sales.lstOfPayToCustomer != null) {
                    foreach (var lp in sales.lstOfPayToCustomer)
                        paymentAmountInJson += Convert.ToDecimal(lp.PayAmount);
                }

                decimal inlineAttachmentSum = inlineOrderAmount + inlinePurchaseLineAmount + inlineSRLineAmount;
                decimal outboundAttachementSum = attachedOrderAmount + attachedPurchaseAmount + attachedSRAmount;
                decimal balanceAmtCalculated = salesAmtInclTax - inlineAttachmentSum - outboundAttachementSum;

                decimal netReceivableOrPayable = balanceAmtCalculated - receiptAmountInJson + paymentAmountInJson;

                //The netReceivableOrPayable is the amount to be received/paid. If the amount is incoice is rounded off, then we have to recalculate the sales amounts and then re-validate it.                
                var differenceDiscountAmount = netReceivableOrPayable - Convert.ToDecimal(sales.BalanceAmt);
                ErrorVM error = null;
                string userID = sales.OperatorCode;
                var salesInvAttr = CalculateReceivable(sales.CompanyCode, sales.BranchCode, sales.EstNo, differenceDiscountAmount, userID, out error);
                if (error != null) {
                    errorMessage = error.description;
                    return false;
                }
                salesAmtInclTax = salesInvAttr.SalesAmountInclTaxWithoutRoundOff;
                balanceAmtCalculated = salesAmtInclTax - inlineAttachmentSum - outboundAttachementSum;
                netReceivableOrPayable = balanceAmtCalculated - receiptAmountInJson + paymentAmountInJson;

                netReceivableOrPayable = Math.Round(netReceivableOrPayable, 2);
                if (netReceivableOrPayable >= -1 && netReceivableOrPayable <= 1) {
                    //If the Net Amount is in between -1 and +1, no need to do anything.
                    ;
                }
                else if (netReceivableOrPayable < -1) {
                    //Convert to order must be available.
                    if (sales.IsOrder != true) {
                        errorMessage = "Since the Balance amount is " + netReceivableOrPayable.ToString() + ", the balance should be converted to order.";
                        return false;
                    }
                }
                else if (netReceivableOrPayable > 1) {
                    //Credit should be available
                    if (sales.IsCreditNote != true) {
                        errorMessage = "Since the Balance amount is " + netReceivableOrPayable.ToString() + ", the pending should be converted to credit note.";
                        return false;
                    }
                }

                //Validate Balance Amount
                decimal balanceAmountFromJson = sales.BalanceAmt;
                decimal differenceAmt = netReceivableOrPayable - balanceAmountFromJson;
                if (netReceivableOrPayable - balanceAmountFromJson >= 1 || netReceivableOrPayable - balanceAmountFromJson <= -1) {
                    errorMessage = "Invalid Blance amount. The expected balance Amount is " + Math.Round(balanceAmtCalculated).ToString("#.##")
                        + ". Please reload the estimation and try billing it.";
                    return false;
                }

                if (netReceivableOrPayable >= -1 && netReceivableOrPayable <= 1) {
                    ;
                }
                else if (netReceivableOrPayable < -1) {
                    if (sales.IsOrder == false) {
                        errorMessage = string.Format("The balance amount {0:0.00} should be either converted to Order or it should be paid.", netReceivableOrPayable);
                        return false;
                    }
                }
                else if (netReceivableOrPayable > 1) {
                    if (!sales.IsCreditNote) {
                        errorMessage = string.Format("The balance amount {0:0.00} should be either converted to Credit note.", netReceivableOrPayable);
                        return false;
                    }
                }
            }

            catch (Exception ex) {
                errorMessage = new ErrorVM().GetErrorDetails(ex).description;
                return false;
            }
            return true;
        }

        public bool DeriveEstimateBalances(SalesInfoQueryVM sales, string userID, out SalesDerivationVM salesInfo, out string errorMessage)
        {
            errorMessage = "All good so far";
            salesInfo = new SalesDerivationVM();
            try {
                //if (sales.CalculateFinalAmount == true) {
                //    if (string.IsNullOrEmpty(sales.RowVersion)) {
                //        errorMessage = "When CalculateFinalAmount is set to true, a value of RowVersion is required.";
                //        return false;
                //    }
                //}

                #region Sales Master with validation
                var salesEstMaster = db.KTTU_SALES_EST_MASTER.Where(x => x.company_code == sales.CompanyCode && x.branch_code == sales.BranchCode
                           && x.est_no == sales.EstNo).FirstOrDefault();
                if (salesEstMaster == null) {
                    errorMessage = "The sales estimation " + sales.EstNo.ToString() + " does not exist or is invalid.";
                    return false;
                }
                if (salesEstMaster.bill_no > 0) {
                    errorMessage = "The estimation number is already billed. The bill number is: " + salesEstMaster.bill_no.ToString();
                    return false;

                }
                if (!string.IsNullOrEmpty(sales.RowVersion)) {
                    if (salesEstMaster.RowVersionString != sales.RowVersion) {
                        errorMessage = "Some other user has edited the sales estimation " + sales.EstNo.ToString() + ". Please reload and try again.";
                        return false;
                    }
                }
                salesInfo.RowVersion = salesEstMaster.RowVersionString;
                salesInfo.CustomerInfo = new CustomerVM
                {
                    CompanyCode = salesEstMaster.company_code,
                    BranchCode = salesEstMaster.branch_code,
                    CustId = salesEstMaster.Cust_Id,
                    MobileNo = salesEstMaster.mobile_no,
                    StateCode = salesEstMaster.state_code,
                    Name = salesEstMaster.Cust_Name
                };
                if (salesEstMaster.Cust_Id > 0) {
                    var custMast = db.KSTU_CUSTOMER_MASTER.Where(x => x.company_code == salesEstMaster.company_code
                        && x.branch_code == salesEstMaster.company_code && x.cust_id == salesEstMaster.Cust_Id).FirstOrDefault();
                    if (custMast != null) {
                        salesInfo.CustomerInfo.RepoCustId = custMast.RepoCustId;
                    }
                }
                #endregion

                #region Inline Order Amount
                int inlineOrderNo = salesEstMaster.order_no;
                decimal inlineOrderAmount = 0.00M;
                if (inlineOrderNo > 0) {
                    var ordTotals = db.KTTU_ORDER_MASTER.Where(x => x.company_code == sales.CompanyCode && x.branch_code == sales.BranchCode
                        && x.order_no == inlineOrderNo).FirstOrDefault();
                    inlineOrderAmount = Convert.ToDecimal(ordTotals.advance_ord_amount);
                    if (ordTotals != null) {
                        salesInfo.InlineOrder = new InlineOrderVM
                        {
                            CompanyCode = ordTotals.company_code,
                            BranchCode = ordTotals.branch_code,
                            OrderNo = ordTotals.order_no,
                            Amount = Convert.ToDecimal(ordTotals.advance_ord_amount)
                        };
                    }
                }

                #endregion

                #region Sales Totals
                decimal salesAmtInclTax = 0.00M;
                //var salesLineTotals = (from sed in db.KTTU_SALES_EST_DETAILS
                //                       where sed.company_code == sales.CompanyCode
                //                             && sed.branch_code == sales.BranchCode && sed.est_no == sales.EstNo
                //                       group sed by 1 into g
                //                       select new
                //                       {
                //                           Qty = g.Sum(x => x.item_no),
                //                           GrossWt = g.Sum(x => x.gwt),
                //                           StoneWt = g.Sum(x => x.swt),
                //                           NetWt = g.Sum(x => x.nwt),
                //                           AddWt = g.Sum(x => x.AddWt),
                //                           DeductWt = g.Sum(x => x.DeductWt),
                //                           VaporLossWt = g.Sum(x => x.vaporLoss),
                //                           ItemTotalBeforeTax = g.Sum(x => x.item_total_after_discount),
                //                           GSTAmt = (g.Sum(x => x.CGST_Amount))
                //                             + (g.Sum(x => x.SGST_Amount))
                //                             + (g.Sum(x => x.IGST_Amount))
                //                             + (g.Sum(x => x.CESSAmount)),
                //                           ItemFinalAmt = g.Sum(x => x.item_final_amount)
                //                           //ItemTotal = g.Sum(x => x.total_amount),
                //                           //CGSTAmount = g.Sum(x => x.CGST_Amount),
                //                           //SGSTAmount = g.Sum(x => x.SGST_Amount),
                //                           //IGSTAmount = g.Sum(x => x.IGST_Amount),
                //                           //CessAmount = g.Sum(x => x.CESSAmount),
                //                           //MCDiscountAmt = g.Sum(x => x.Mc_Discount_Amt),
                //                           //OrderDiscountAmt = g.Sum(x => x.Ord_Discount_Amt),
                //                           //OfferDiscountAmt = g.Sum(x => x.offer_value),
                //                           //AdditionalDiscountAmt = g.Sum(x => x.item_additional_discount),
                //                           //TotalDiscountAmt = (g.Sum(x => x.item_additional_discount))
                //                           //                             + (g.Sum(x => x.Mc_Discount_Amt))
                //                           //                             + (g.Sum(x => x.Ord_Discount_Amt))
                //                           //                             + (g.Sum(x => x.offer_value)),
                //                           //GSTAmtOriginal = (g.Sum(x => x.CGST_Percent * x.total_amount / 100))
                //                           //                             + (g.Sum(x => x.SGST_Percent * x.total_amount / 100))
                //                           //                             + (g.Sum(x => x.IGST_Percent * x.total_amount / 100))
                //                           //                             + (g.Sum(x => x.CESSPercent * x.total_amount / 100))
                //                       }).FirstOrDefault();

                //if (salesLineTotals == null) {
                //    errorMessage = "There is no estimation line detail.";
                //    return false;
                //}
                //salesInfo.CompanyCode = sales.CompanyCode;
                //salesInfo.BranchCode = sales.BranchCode;
                //salesInfo.EstNo = sales.EstNo;
                //salesInfo.Qty = Convert.ToInt32(salesLineTotals.Qty);
                //var addDedWt = Convert.ToDecimal(salesLineTotals.AddWt) - Convert.ToDecimal(salesLineTotals.DeductWt) - Convert.ToDecimal(salesLineTotals.VaporLossWt);
                //salesInfo.GrossWt = Convert.ToDecimal(salesLineTotals.GrossWt) + addDedWt;
                //salesInfo.StoneWt = Convert.ToDecimal(salesLineTotals.StoneWt);
                //salesInfo.NetWt = Convert.ToDecimal(salesLineTotals.NetWt) + addDedWt;
                //salesInfo.LineAmountBeforeTax = Convert.ToDecimal(salesLineTotals.ItemTotalBeforeTax);
                //salesInfo.TaxAmount = Convert.ToDecimal(salesLineTotals.GSTAmt);
                //salesInfo.LineAmountAfterTax = Convert.ToDecimal(salesLineTotals.ItemFinalAmt);
                //salesAmtInclTax = Convert.ToDecimal(salesLineTotals.ItemFinalAmt);

                List<SalesLineVM> salesLines = new List<SalesLineVM>();
                bool salesLinesQuerySucceeded = GetSalesLines(sales.CompanyCode, sales.BranchCode, sales.EstNo, out salesLines, out errorMessage);
                if (!salesLinesQuerySucceeded) {
                    errorMessage = "Failed to get sales lines. Error: " + errorMessage;
                    return false;
                }
                salesInfo.SalesLines = salesLines;
                salesInfo.CompanyCode = sales.CompanyCode;
                salesInfo.BranchCode = sales.BranchCode;
                salesInfo.EstNo = sales.EstNo;
                salesInfo.Qty = Convert.ToInt32(salesLines.Sum(x => x.Qty));
                salesInfo.GrossWt = Convert.ToDecimal(salesLines.Sum(x => x.GrossWt));
                salesInfo.StoneWt = Convert.ToDecimal(salesLines.Sum(x => x.StoneWt));
                salesInfo.NetWt = Convert.ToDecimal(salesLines.Sum(x => x.NetWt));
                salesInfo.LineAmountBeforeTax = Convert.ToDecimal(salesLines.Sum(x => x.LineAmountBeforeTax));
                salesInfo.OfferDiscount = Convert.ToDecimal(salesLines.Sum(x => x.OfferDiscount));
                salesInfo.TaxAmount = Convert.ToDecimal(salesLines.Sum(x => x.TotalGSTAmount));
                salesInfo.LineAmountAfterTax = Convert.ToDecimal(salesLines.Sum(x => x.LineAmountAfterTax));
                salesAmtInclTax = Convert.ToDecimal(salesLines.Sum(x => x.LineAmountAfterTax));

                if (sales.CalculateFinalAmount && Convert.ToDecimal(sales.DifferenceDiscountAmt) > 0) {
                    ErrorVM error = null;
                    salesInfo.SalesInvoiceAttribute = CalculateReceivable(sales.CompanyCode, sales.BranchCode, sales.EstNo, sales.DifferenceDiscountAmt, userID, out error);
                    if (error != null) {
                        errorMessage = error.description;
                        return false;
                    }
                    if (salesInfo.SalesInvoiceAttribute != null) {
                        salesAmtInclTax = salesInfo.SalesInvoiceAttribute.SalesAmountInclTaxWithoutRoundOff;
                        salesInfo.LineAmountBeforeTax = salesInfo.SalesInvoiceAttribute.SalesAmountExclTax;
                        salesInfo.TaxAmount = salesInfo.SalesInvoiceAttribute.GSTAmount;
                        salesInfo.LineAmountAfterTax = salesAmtInclTax;
                    }
                }
                #endregion

                #region Inline Old Purchase Amount
                decimal inlinePurchaseLineAmount = 0.00M;
                var purEstMaster = db.KTTU_PURCHASE_EST_MASTER.Where(x => x.company_code == sales.CompanyCode && x.branch_code == sales.BranchCode
                        && x.est_no == sales.EstNo).FirstOrDefault();
                if (purEstMaster != null) {
                    if (purEstMaster.bill_no == 0) {
                        var purchaseTotals = db.KTTU_PURCHASE_EST_DETAILS.Where(x => x.company_code == sales.CompanyCode && x.branch_code == sales.BranchCode
                             && x.est_no == sales.EstNo).OrderBy(y => y.sl_no).ToList();
                        if (purchaseTotals != null) {
                            inlinePurchaseLineAmount = Convert.ToDecimal(purchaseTotals.Sum(x => x.item_amount));
                            salesInfo.InlinePurchase = new InlineOldPurchaseVM
                            {
                                CompanyCode = purEstMaster.company_code,
                                BranchCode = purEstMaster.branch_code,
                                EstNo = purEstMaster.est_no,
                                Qty = Convert.ToInt32(purchaseTotals.Sum(x => x.item_no)),
                                GrossWt = Convert.ToDecimal(purchaseTotals.Sum(x => x.gwt)),
                                StoneWt = Convert.ToDecimal(purchaseTotals.Sum(x => x.swt)),
                                NetWt = Convert.ToDecimal(purchaseTotals.Sum(x => x.nwt)),
                                LineAmountBeforeTax = Convert.ToDecimal(purchaseTotals.Sum(x => x.item_amount)),
                                TaxAmount = 0.00M,
                                LineAmountAfterTax = Convert.ToDecimal(purchaseTotals.Sum(x => x.item_amount)),
                            };
                            salesInfo.InlinePurchase.PurchaseLines = new List<PurchaseLineVM>();
                            foreach (var pl in purchaseTotals) {
                                PurchaseLineVM newPurLne = new PurchaseLineVM
                                {
                                    EstNo = pl.est_no,
                                    SerialNo = pl.sl_no,
                                    Item = pl.item_name,
                                    GrossWt = pl.gwt,
                                    StoneWt = pl.swt,
                                    DiamondWt = Convert.ToDecimal(pl.dcts),
                                    NetWt = pl.nwt,
                                    PurityPercent = Convert.ToDecimal(pl.purity_per),
                                    MeltingPercent = pl.melting_percent,
                                    MeltingLoss = Convert.ToDecimal(pl.melting_loss),
                                    PurhaseRate = pl.purchase_rate,
                                    MetalAmount = pl.gold_amount,
                                    StoneDiamondAmount = pl.diamond_amount,
                                    Qty = pl.item_no,
                                    LineAmountBeforeTax = pl.item_amount,
                                    LineAmountAfterTax = pl.item_amount
                                };
                                salesInfo.InlinePurchase.PurchaseLines.Add(newPurLne);
                            }
                        }
                    }


                }
                #endregion

                #region Inline SR Amount
                decimal inlineSRLineAmount = 0.00M;
                var inlineSrEstMaster = db.KTTU_SR_MASTER.Where(x => x.company_code == sales.CompanyCode && x.branch_code == sales.BranchCode
                        && x.est_no == sales.EstNo).FirstOrDefault();
                if (inlineSrEstMaster != null) {
                    if (inlineSrEstMaster.sales_bill_no != 0) {
                        errorMessage = string.Format("The sales return estimation# {0} is already billed with invoice# {1}."
                                        , inlineSrEstMaster.est_no.ToString(), inlineSrEstMaster.sales_bill_no.ToString());
                        return false;
                    }

                    var srTotals = db.KTTU_SR_DETAILS.Where(x => x.company_code == sales.CompanyCode && x.branch_code == sales.BranchCode
                           && x.est_no == sales.EstNo);
                    if (srTotals != null) {
                        inlineSRLineAmount = Convert.ToDecimal(srTotals.Sum(x => x.item_final_amount));
                    }
                }
                #endregion

                #region Attached Document Amounts


                //salesInfo.AttachedOrders = new List<SalesAttachmentVM>();
                //salesInfo.AttachedPurchases = new List<SalesAttachmentVM>();
                //salesInfo.AttachedSRs = new List<SalesAttachmentVM>();
                salesInfo.AttachedOldPurchase = new SalesAttachmentInfoVM { DocumentInfo = "Old Purchases attached to sales estimate", AttachmentLines = new List<SalesAttachmentLinesVM>() };
                salesInfo.AttachedSalesReturn = new SalesAttachmentInfoVM { DocumentInfo = "Sales Returns attached to sales estimate", AttachmentLines = new List<SalesAttachmentLinesVM>() };
                salesInfo.AttachedCustomerOrder = new SalesAttachmentInfoVM { DocumentInfo = "Customer orders attached to sales estimate", AttachmentLines = new List<SalesAttachmentLinesVM>() };
                salesInfo.AttachedOthers = new SalesAttachmentInfoVM { DocumentInfo = "Other attachements including schemes, gift vouchers, sales promotions, etc.", AttachmentLines = new List<SalesAttachmentLinesVM>() };
                decimal attachedOrderAmount = 0.00M;
                decimal attachedPurchaseAmount = 0.00M;
                decimal attachedSRAmount = 0.00M;
                decimal attachedOtherAmount = 0.00M;
                var payInfo = db.KTTU_PAYMENT_DETAILS.Where(pd => pd.company_code == sales.CompanyCode && pd.branch_code == sales.BranchCode
                    && pd.series_no == sales.EstNo && pd.trans_type == "A" && pd.cflag != "Y");
                foreach (var pay in payInfo) {
                    int docNo = Convert.ToInt32(pay.Ref_BillNo);
                    switch (pay.pay_mode) {
                        case "OP":
                            var ordInfo = db.KTTU_ORDER_MASTER.Where(x => x.company_code == pay.company_code && x.branch_code == pay.branch_code
                                && x.order_no == docNo).FirstOrDefault();
                            if (ordInfo == null) {
                                errorMessage = string.Format("The attached order# {0} does not exist or is invalid.", docNo.ToString());
                                return false;
                            }
                            if (ordInfo.cflag == "Y") {
                                errorMessage = string.Format("The attached order# {0} is cancelled."
                                    , ordInfo.order_no.ToString());
                                return false;
                            }
                            if (ordInfo.bill_no != 0) {
                                errorMessage = string.Format("The attached order# {0} is already billed with invoice# {1}."
                                    , ordInfo.order_no.ToString(), ordInfo.bill_no.ToString());
                                return false;
                            }

                            attachedOrderAmount += Convert.ToDecimal(ordInfo.advance_ord_amount);
                            //SalesAttachmentVM attachedOrder = new SalesAttachmentVM
                            //{
                            //    CompanyCode = ordInfo.company_code,
                            //    BranchCode = ordInfo.branch_code,
                            //    DocumentNo = ordInfo.order_no,
                            //    Amount = Convert.ToDecimal(ordInfo.advance_ord_amount)
                            //};
                            //salesInfo.AttachedOrders.Add(attachedOrder);
                            SalesAttachmentLinesVM attachedCustOrder = new SalesAttachmentLinesVM
                            {
                                CompanyCode = ordInfo.company_code,
                                BranchCode = ordInfo.branch_code,
                                DocumentNo = ordInfo.order_no,
                                DocumentDetail = "Cust. Order# " + ordInfo.order_no.ToString(),
                                Amount = Convert.ToDecimal(ordInfo.advance_ord_amount)
                            };
                            salesInfo.AttachedCustomerOrder.AttachmentLines.Add(attachedCustOrder);
                            salesInfo.AttachedCustomerOrder.TotalAmount = attachedOrderAmount;

                            break;
                        case "PE":
                            var attachedPurEstMast = db.KTTU_PURCHASE_EST_MASTER.Where(x => x.company_code == sales.CompanyCode && x.branch_code == sales.BranchCode
                                && x.est_no == docNo).FirstOrDefault();
                            if (attachedPurEstMast == null) {
                                errorMessage = string.Format("The attached purchase estimation# {0} does not exist or is invalid.", docNo.ToString());
                                return false;
                            }
                            if (attachedPurEstMast.bill_no != 0) {
                                errorMessage = string.Format("The attached purchase estimation# {0} is already billed with invoice# {1}."
                                                , docNo.ToString(), attachedPurEstMast.bill_no.ToString());
                                return false;
                            }

                            var oldPurLineInfo = db.KTTU_PURCHASE_EST_DETAILS.Where(x => x.company_code == pay.company_code && x.branch_code == pay.branch_code
                                && x.est_no == docNo).ToList();
                            if (oldPurLineInfo != null) {
                                attachedPurchaseAmount += Convert.ToDecimal(oldPurLineInfo.Sum(s => s.item_amount));
                                //SalesAttachmentVM attachedPurchase = new SalesAttachmentVM
                                //{
                                //    CompanyCode = attachedPurEstMast.company_code,
                                //    BranchCode = attachedPurEstMast.branch_code,
                                //    DocumentNo = attachedPurEstMast.est_no,
                                //    Qty = Convert.ToInt32(oldPurLineInfo.Sum(s => s.item_no)),
                                //    GrossWt = Convert.ToDecimal(oldPurLineInfo.Sum(s => s.gwt)),
                                //    StoneWt = Convert.ToDecimal(oldPurLineInfo.Sum(s => s.swt)),
                                //    NetWt = Convert.ToDecimal(oldPurLineInfo.Sum(s => s.nwt)),
                                //    Amount = Convert.ToDecimal(oldPurLineInfo.Sum(s => s.item_amount))
                                //};
                                //salesInfo.AttachedPurchases.Add(attachedPurchase);
                                SalesAttachmentLinesVM attachedOldPurhase = new SalesAttachmentLinesVM
                                {
                                    CompanyCode = attachedPurEstMast.company_code,
                                    BranchCode = attachedPurEstMast.branch_code,
                                    DocumentNo = attachedPurEstMast.est_no,
                                    DocumentDetail = "Doc. No# " + attachedPurEstMast.est_no.ToString(),
                                    Qty = Convert.ToInt32(oldPurLineInfo.Sum(s => s.item_no)),
                                    GrossWt = Convert.ToDecimal(oldPurLineInfo.Sum(s => s.gwt)),
                                    StoneWt = Convert.ToDecimal(oldPurLineInfo.Sum(s => s.swt)),
                                    NetWt = Convert.ToDecimal(oldPurLineInfo.Sum(s => s.nwt)),
                                    Amount = Convert.ToDecimal(oldPurLineInfo.Sum(s => s.item_amount))
                                };
                                salesInfo.AttachedOldPurchase.AttachmentLines.Add(attachedOldPurhase);
                                salesInfo.AttachedOldPurchase.TotalAmount = attachedPurchaseAmount;
                            }
                            break;
                        case "SE":
                            var attachedSREstMast = db.KTTU_SR_MASTER.Where(x => x.company_code == sales.CompanyCode && x.branch_code == sales.BranchCode
                                && x.est_no == docNo).FirstOrDefault();
                            if (attachedSREstMast == null) {
                                errorMessage = string.Format("The attached SR estimation# {0} does not exist or is invalid.", docNo.ToString());
                                return false;
                            }
                            if (attachedSREstMast.sales_bill_no != 0) {
                                errorMessage = string.Format("The attached SR estimation# {0} is already billed with invoice# {1}."
                                                , docNo.ToString(), attachedSREstMast.sales_bill_no.ToString());
                                return false;
                            }
                            var srInfo = db.KTTU_SR_DETAILS.Where(x => x.company_code == pay.company_code && x.branch_code == pay.branch_code
                                && x.est_no == docNo).ToList();
                            if (srInfo != null) {
                                attachedSRAmount += Convert.ToDecimal(srInfo.Sum(s => s.item_final_amount));
                                //SalesAttachmentVM attachedSR = new SalesAttachmentVM
                                //{
                                //    CompanyCode = attachedSREstMast.company_code,
                                //    BranchCode = attachedSREstMast.branch_code,
                                //    DocumentNo = attachedSREstMast.est_no,
                                //    Qty = Convert.ToInt32(srInfo.Sum(s => s.quantity)),
                                //    GrossWt = Convert.ToDecimal(srInfo.Sum(s => s.gwt)),
                                //    StoneWt = Convert.ToDecimal(srInfo.Sum(s => s.swt)),
                                //    NetWt = Convert.ToDecimal(srInfo.Sum(s => s.nwt)),
                                //    Amount = Convert.ToDecimal(srInfo.Sum(s => s.item_final_amount))
                                //};
                                //salesInfo.AttachedSRs.Add(attachedSR);
                                SalesAttachmentLinesVM attachedSalesReturn = new SalesAttachmentLinesVM
                                {
                                    CompanyCode = attachedSREstMast.company_code,
                                    BranchCode = attachedSREstMast.branch_code,
                                    DocumentNo = attachedSREstMast.est_no,
                                    DocumentDetail = "Doc. No# " + attachedSREstMast.est_no.ToString(),
                                    Qty = Convert.ToInt32(srInfo.Sum(s => s.quantity)),
                                    GrossWt = Convert.ToDecimal(srInfo.Sum(s => s.gwt)),
                                    StoneWt = Convert.ToDecimal(srInfo.Sum(s => s.swt)),
                                    NetWt = Convert.ToDecimal(srInfo.Sum(s => s.nwt)),
                                    Amount = Convert.ToDecimal(srInfo.Sum(s => s.item_final_amount))
                                };
                                salesInfo.AttachedSalesReturn.AttachmentLines.Add(attachedSalesReturn);
                                salesInfo.AttachedSalesReturn.TotalAmount = attachedSRAmount;
                            }
                            break;
                    }
                }
                #endregion

                #region Final Calculations

                decimal inlineAttachmentSum = inlineOrderAmount + inlinePurchaseLineAmount + inlineSRLineAmount;
                decimal outboundAttachementSum = attachedOrderAmount + attachedPurchaseAmount + attachedSRAmount + attachedOtherAmount;
                decimal balanceAmtCalculated = salesAmtInclTax - inlineAttachmentSum - outboundAttachementSum;
                decimal netReceivableOrPayable = balanceAmtCalculated - sales.TotalReceiptAmount + sales.TotalPaymentAmount;
                //if (sales.CalculateFinalAmount && Convert.ToDecimal(sales.ReceivableAmount) != 0) {
                if (sales.CalculateFinalAmount) {
                    if (netReceivableOrPayable < Convert.ToDecimal(sales.ReceivableAmount)) {
                        errorMessage = string.Format("The Receivable/Payable amount {0:0.00} should be lesser(smaller) than equal to Balance amount {1:0.00}",
                            Convert.ToDecimal(sales.ReceivableAmount), balanceAmtCalculated);
                        return false;
                    }
                    var differenceDiscountAmount = netReceivableOrPayable - Convert.ToDecimal(sales.ReceivableAmount);
                    ErrorVM error = null;
                    salesInfo.SalesInvoiceAttribute = CalculateReceivable(sales.CompanyCode, sales.BranchCode, sales.EstNo, differenceDiscountAmount, userID, out error);
                    if (error != null) {
                        errorMessage = error.description;
                        return false;
                    }
                    salesAmtInclTax = salesInfo.SalesInvoiceAttribute.SalesAmountInclTaxWithoutRoundOff;
                    balanceAmtCalculated = salesAmtInclTax - inlineAttachmentSum - outboundAttachementSum;
                    netReceivableOrPayable = balanceAmtCalculated - sales.TotalReceiptAmount + sales.TotalPaymentAmount;

                }
                salesInfo.GrossAmount = balanceAmtCalculated;
                salesInfo.RoundedOffAmount = Math.Round(salesInfo.GrossAmount, 0) - salesInfo.GrossAmount;
                salesInfo.GrossAmount = salesInfo.GrossAmount + salesInfo.RoundedOffAmount; //To eleminate 1255.0, I need consistent Zero's

                salesInfo.ReceivedAmount = sales.TotalReceiptAmount;
                salesInfo.PaidAmount = sales.TotalPaymentAmount;
                salesInfo.Balance = netReceivableOrPayable + salesInfo.RoundedOffAmount;
                #endregion

            }
            catch (Exception ex) {
                errorMessage = new ErrorVM().GetErrorDetails(ex).description;
                return false;
            }
            return true;
        }

        private bool GetSalesLines(string companyCode, string branchCode, int estNo, out List<SalesLineVM> salesLines, out string errorMessage)
        {
            salesLines = new List<SalesLineVM>();
            errorMessage = string.Empty;
            try {
                var sLineX = (from x in db.KTTU_SALES_EST_DETAILS
                              where x.company_code == companyCode
                                      && x.branch_code == branchCode && x.est_no == estNo
                              orderby x.sl_no
                              select new
                              {
                                  SerialNo = x.sl_no,
                                  Item = x.item_name,
                                  Counter = x.counter_code,
                                  BarcodeNo = x.barcode_no,
                                  Qty = x.item_no,
                                  GrossWt = x.gwt,
                                  StoneWt = x.swt,
                                  NetWt = x.nwt,
                                  AddWt = x.AddWt,
                                  DeductWt = x.DeductWt,
                                  VaporLossWt = x.vaporLoss,
                                  RatePerGram = x.rate,
                                  MCPercent = x.mc_percent,
                                  MCAmount = x.mc_amount,
                                  MetalAmount = x.gold_value,
                                  StoneAmount = x.stone_charges,
                                  DiamondAmount = x.diamond_charges,
                                  LineAmountBeforeTax = x.item_total_after_discount,
                                  OfferDiscount = x.offer_value,
                                  CGSTAmount = x.CGST_Amount,
                                  SGSTAmount = x.SGST_Amount,
                                  IGSTAmount = x.IGST_Amount,
                                  LineAmountAfterTax = x.item_final_amount
                              }).ToList();

                foreach (var sl in sLineX) {
                    var newSalesLine = new SalesLineVM
                    {
                        EstNo = estNo,
                        SerialNo = sl.SerialNo,
                        Item = sl.Item,
                        Counter = sl.Counter,
                        BarcodeNo = sl.BarcodeNo,
                        Qty = sl.Qty,
                        GrossWt = sl.GrossWt + Convert.ToDecimal(sl.AddWt) - Convert.ToDecimal(sl.DeductWt) - Convert.ToDecimal(sl.VaporLossWt),
                        StoneWt = sl.StoneWt,
                        NetWt = sl.NetWt + Convert.ToDecimal(sl.AddWt) - Convert.ToDecimal(sl.DeductWt) - Convert.ToDecimal(sl.VaporLossWt),
                        RatePerGram = Convert.ToDecimal(sl.RatePerGram),
                        MCPercent = Convert.ToDecimal(sl.MCPercent),
                        MCAmount = Convert.ToDecimal(sl.MCAmount),
                        MetalAmount = Convert.ToDecimal(sl.MetalAmount),
                        StoneAmount = Convert.ToDecimal(sl.StoneAmount),
                        DiamondAmount = Convert.ToDecimal(sl.DiamondAmount),
                        LineAmountBeforeTax = Convert.ToDecimal(sl.LineAmountBeforeTax),
                        OfferDiscount = Convert.ToDecimal(sl.OfferDiscount),
                        CGSTAmount = Convert.ToDecimal(sl.CGSTAmount),
                        SGSTAmount = Convert.ToDecimal(sl.SGSTAmount),
                        IGSTAmount = Convert.ToDecimal(sl.IGSTAmount),
                        LineAmountAfterTax = Convert.ToDecimal(sl.LineAmountAfterTax)
                    };
                    salesLines.Add(newSalesLine);
                }
            }
            catch (Exception ex) {
                errorMessage = new ErrorVM().GetErrorDetails(ex).description;
                return false;
            }
            return true;
        }

        private bool ValidateDocumentPaymentModes(string companyCode, string branchCode, List<PaymentVM> invoiceReceipts, out string errorMessage)
        {
            errorMessage = "All is well";
            if (invoiceReceipts == null)
                return true;
            foreach (var rec in invoiceReceipts) {
                int docNo = Convert.ToInt32(rec.RefBillNo);
                switch (rec.PayMode) {
                    case "OP":
                        var ordInfo = db.KTTU_ORDER_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                            && x.order_no == docNo).FirstOrDefault();
                        if (ordInfo == null) {
                            errorMessage = string.Format("The receipt mode as order# {0} does not exist or is invalid.", docNo.ToString());
                            return false;
                        }
                        if (ordInfo.bill_no != 0) {
                            errorMessage = string.Format("The receipt mode as order# {0} is already billed with invoice# {1}. Please remove the receipt mode and try again."
                                , ordInfo.order_no.ToString(), ordInfo.bill_no.ToString());
                            return false;
                        }
                        if (Math.Ceiling(Convert.ToDecimal(rec.PayAmount)) != Math.Ceiling(Convert.ToDecimal(ordInfo.advance_ord_amount))) {
                            errorMessage = string.Format("The actual order advance amount for receipt mode as order# {0} is {1}. The amount {2} is invalid"
                                , ordInfo.order_no.ToString(), Convert.ToDecimal(ordInfo.advance_ord_amount), Convert.ToDecimal(rec.PayAmount));
                            return false;

                        }

                        break;
                    case "PB":
                        var purBillMast = db.KTTU_PURCHASE_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                            && x.bill_no == docNo).FirstOrDefault();
                        if (purBillMast == null) {
                            errorMessage = string.Format("The receipt mode as purchase invoice# {0} does not exist or is invalid. Please remove the receipt mode and try again.", docNo.ToString());
                            return false;
                        }
                        if (purBillMast.cflag == "Y") {
                            errorMessage = string.Format("The receipt mode as purchase invoice# {0} is cancelled. Please remove the receipt mode and try again.", docNo.ToString());
                            return false;
                        }
                        var postedPurLines = db.KTTU_PURCHASE_DETAILS.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                                && x.bill_no == docNo);
                        if (postedPurLines == null) {
                            errorMessage = string.Format("The receipt mode as purchase invoice# {0} does not exist or is invalid. No line details are found.", docNo.ToString());
                            return false;
                        }
                        decimal attachedPurchaseAmount = Convert.ToDecimal(postedPurLines.Sum(s => s.item_amount));
                        if (Math.Ceiling(Convert.ToDecimal(rec.PayAmount)) != Math.Ceiling(attachedPurchaseAmount)) {
                            errorMessage = string.Format("The actual purchase amount for receipt mode as purchase# {0} is {1}. The amount {2} is invalid"
                                , docNo, Convert.ToDecimal(attachedPurchaseAmount), Convert.ToDecimal(rec.PayAmount));
                            return false;
                        }

                        break;
                    case "SR":
                        var srConfMast = db.KTTU_SR_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                            && x.sales_bill_no == docNo).FirstOrDefault();
                        if (srConfMast == null) {
                            errorMessage = string.Format("The receipt mode as SR bill# {0} does not exist or is invalid. Please remove the receipt mode and try again.", docNo.ToString());
                            return false;
                        }
                        if (srConfMast.cflag == "Y") {
                            errorMessage = string.Format("The receipt mode as SR bill# {0} is cancelled. Please remove the receipt mode and try again.", docNo.ToString());
                            return false;
                        }
                        //if (srConfMast.sales_bill_no != 0) {
                        //    errorMessage = string.Format("The receipt mode as SR bill# {0} is cancelled or invalid. Please remove the receipt mode and try again.", docNo.ToString());
                        //    return false;
                        //}
                        #region Since the detail amount does not match with master amount, these lines are commented.
                        //var srInfo = db.KTTU_SR_DETAILS.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                        //    && x.sales_bill_no == docNo);
                        //if (srInfo == null) {
                        //    errorMessage = string.Format("The receipt mode as SR invoice# {0} does not exist or is invalid. No line details are found.", docNo.ToString());
                        //    return false;
                        //}
                        //decimal attachedSRAmount = Convert.ToDecimal(srInfo.Sum(s => s.item_final_amount)); 
                        #endregion
                        decimal attachedSRAmount = Convert.ToDecimal(srConfMast.total_sramount);
                        if (Math.Ceiling(Convert.ToDecimal(rec.PayAmount)) != Math.Ceiling(attachedSRAmount)) {
                            errorMessage = string.Format("The actual SR amount for receipt mode as SR# {0} is {1}. The amount {2} is invalid"
                                , docNo, Convert.ToDecimal(attachedSRAmount), Convert.ToDecimal(rec.PayAmount));
                            return false;
                        }
                        break;
                    case "CT":
                        CHTU_CHIT_CLOSURE cc = db.CHTU_CHIT_CLOSURE.Where(ccl => ccl.Scheme_Code == rec.SchemeCode
                                                && ccl.Group_Code == rec.GroupCode && ccl.Closed_At == rec.CTBranch
                                                && ccl.Chit_MembShipNo == docNo).FirstOrDefault();
                        if (cc == null) {
                            errorMessage = string.Format("The receipt mode as Scheme # {0} does not exist or is invalid. No details are found. Please remove the receipt mode and try again.", rec.PayDetails);
                            return false;
                        }
                        if (cc.Bill_Number != "0") {
                            errorMessage = string.Format("The receipt mode as Scheme # {0} is already adjusted to invoice number {2}", rec.PayDetails, cc.Bill_Number);
                            return false;
                        }
                        if (rec.PayAmount != cc.Chit_Amt) {
                            errorMessage = string.Format("The actual Scheme amount for receipt mode as Scheme# {0} is {1}. The amount {2} is invalid"
                               , docNo, Convert.ToDecimal(cc.Amt_Received), Convert.ToDecimal(rec.PayAmount));
                            return false;
                        }
                        break;
                }
            }
            return true;
        }

        private bool DeleteStockTakingEntries(MagnaDbEntities dbContext, string companyCode, string branchCode, int salesBillNo, out string errorMessage)
        {
            errorMessage = string.Empty;
            try {
                if (dbContext == null)
                    dbContext = new MagnaDbEntities(true);
                var salesLines = dbContext.KTTU_SALES_DETAILS.Where(sed => sed.company_code == companyCode
                               && sed.branch_code == branchCode && sed.bill_no == salesBillNo);
                foreach (var x in salesLines) {
                    if (!string.IsNullOrEmpty(x.barcode_no)) {
                        string barcodeNo = x.barcode_no;
                        var stockTaking = dbContext.KTTU_STOCK_TAKING.Where(s => s.company_code == companyCode
                            && s.branch_code == branchCode && s.barcode_no == barcodeNo).FirstOrDefault();
                        if (stockTaking != null) {
                            db.KTTU_STOCK_TAKING.Remove(stockTaking);
                        }
                    }
                    if (!string.IsNullOrEmpty(x.ad_barcode)) {
                        string adBarcodeNo = x.barcode_no;
                        var stockTaking = dbContext.KTTU_STOCK_TAKING.Where(s => s.company_code == companyCode
                            && s.branch_code == branchCode && s.barcode_no == adBarcodeNo).FirstOrDefault();
                        if (stockTaking != null) {
                            db.KTTU_STOCK_TAKING.Remove(stockTaking);
                        }
                    }
                }
            }
            catch (Exception excp) {
                errorMessage = excp.Message;
                return false;
            }
            return true;
        }

        public string PrintSalesBilling(string companyCode, string branchCode, int billNo, out ErrorVM error)
        {
            error = null;
            if (billNo < 0) {
                error = new ErrorVM()
                {
                    description = "Invalid Bill No",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                };
                return "";
            }
            StringBuilder report = new StringBuilder();
            try {

                KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(com => com.company_code == companyCode
                                                                            && com.branch_code == branchCode).FirstOrDefault();
                KTTU_SALES_MASTER ksm = db.KTTU_SALES_MASTER.Where(sm => sm.bill_no == billNo
                                                                    && sm.company_code == companyCode
                                                                    && sm.branch_code == branchCode).FirstOrDefault();
                List<KTTU_SALES_DETAILS> lstOfSalesDet = db.KTTU_SALES_DETAILS.Where(sm => sm.bill_no == billNo
                                                                     && sm.company_code == companyCode
                                                                     && sm.branch_code == branchCode).OrderBy(x => x.sl_no).ToList();
                if (ksm == null) {
                    error = new ErrorVM()
                    {
                        description = "Invalid Bill Number",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound,
                        field = "Bill Number"
                    };
                    return null;
                }
                var billDate = ksm.bill_date.ToString("dd/MM/yyyy HH:mm:ss tt", CultureInfo.InvariantCulture);
                report.AppendLine("<html>");
                report.AppendLine("<head>");
                report.AppendLine(SIGlobals.Globals.GetReportStyle());
                report.AppendLine("</head>");
                report.AppendLine("<body>");

                #region Report type
                report.AppendLine("<Table frame=\"border\" border=\"0\" width=\"900\">");
                for (int j = 0; j < 9; j++) {
                    report.AppendLine("<TR style=border:0>");
                    report.AppendLine(string.Format("<TD style=border:0 colspan = 15 ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    report.AppendLine("</TR>");
                }
                report.AppendLine("<TR style=border:0;\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                if (string.Compare(ksm.cflag.ToString(), "Y") == 0) {
                    report.AppendLine("<TD style=border:0 width=\"900\" colspan=0 ALIGN = \"CENTER\"><b><h3>GST INVOICE/CANCELLED</h3></b></TD>");
                }
                else {
                    report.AppendLine("<TD style=border:0 width=\"900\" colspan=0 ALIGN = \"CENTER\"><b><h3>GST INVOICE</h3></b></TD>");
                }
                report.AppendLine("</TR>");

                for (int j = 0; j < 2; j++) {
                    report.AppendLine("<TR style=border:0>");
                    report.AppendLine(string.Format("<TD style=border:0 colspan = 15 ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    report.AppendLine("</TR>");
                }
                report.AppendLine("</Table>");
                #endregion

                #region Header: Customer Details, GSTIN and Showroom Address
                report.AppendLine("<Table font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\";  style=\"border-collapse:collapse;\" width=\"900\">");
                report.AppendLine("<tr style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                report.AppendLine("<TD width=\"300\" ALIGN = \"center\"><b>CUSTOMER DETAILS</b></TD>");
                report.AppendLine(string.Format("<TD width=\"300\"  ALIGN = \"center\"><b>GSTIN &nbsp&nbsp&nbsp&nbsp {0}</b></TD>", company.tin_no.ToString()));
                report.AppendLine(string.Format("<TD width=\"300\"  ALIGN = \"center\"><b>{0}</b></TD>", "SHOWROOM ADDRESS"));
                report.AppendLine("</tr>");
                #endregion

                #region Customer Details
                report.AppendLine("<tr>");
                report.AppendLine("<td>");
                report.AppendLine("<Table font-size=12pt;  class=\"boldText\"  style=\"border-collapse:collapse;\" >");  //FRAME=BOX RULES=NONE

                report.AppendLine("<tr style=\"border-right:0\"  >");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left \" ><b>Name &nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + ksm.cust_name + "</b></td>");
                report.AppendLine("</tr>");

                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" ><b>Address &nbsp&nbsp </b></td>");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + ksm.address1 + "</b></td>");
                report.AppendLine("</tr>");
                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + ksm.address2 + "</b></td>");
                report.AppendLine("</tr>");
                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + ksm.address3 + "</b></td>");
                report.AppendLine("</tr>");

                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \" tyle=\"border-top:thin\" align=\"left\" ><b>City &nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + ksm.city + "</b></td>");
                report.AppendLine("</tr>");

                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" ><b>State &nbsp&nbsp </b></td>");
                report.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + ksm.state + "</b></td>");
                report.AppendLine("</tr>");

                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>State code &nbsp&nbsp </b></td>");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + ksm.state_code + "</b></td>");
                report.AppendLine("</tr>");

                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>Phone &nbsp&nbsp </b></td>");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + ksm.mobile_no + "</b></td>");
                report.AppendLine("</tr>");
                if (!string.IsNullOrEmpty(ksm.pan_no)) {
                    report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    report.AppendLine("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>PAN &nbsp&nbsp</b></td>");
                    report.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + ksm.pan_no + "</b></td>");
                    report.AppendLine("</tr>");
                }
                else {
                    KSTU_CUSTOMER_ID_PROOF_DETAILS custDoc = db.KSTU_CUSTOMER_ID_PROOF_DETAILS.Where(kcpd => kcpd.cust_id == ksm.cust_Id && kcpd.company_code == companyCode && kcpd.branch_code == branchCode).FirstOrDefault();
                    if (custDoc != null) {
                        if (!string.IsNullOrEmpty(custDoc.Doc_code)) {
                            report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                            report.AppendLine(string.Format("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>{0} &nbsp&nbsp</b></td>", custDoc.Doc_code.ToString()));
                            report.AppendLine(string.Format("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>{0}</b></td>", custDoc.Doc_code.ToString()));
                            report.AppendLine("</tr>");
                        }
                    }
                }
                if (!string.IsNullOrEmpty(ksm.tin)) {
                    report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    report.AppendLine("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>GSTIN &nbsp&nbsp</b></td>");
                    report.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + ksm.tin + "</b></td>");
                    report.AppendLine("</tr>");
                }
                report.AppendLine("</table>");
                report.AppendLine("</td>");
                report.AppendLine("<td>");
                #endregion

                #region Invoice Details
                report.AppendLine("<Table font-size=12pt;   class=\"boldText\"  style=\"border-collapse:collapse;\" >");
                report.AppendLine("<tr style=\"border-right:0\">");
                report.AppendLine("<td style=\"border-right:thin ; border-top:thin\" align=\"left \" ><b>INVOICE No</b></td>");
                report.AppendLine(string.Format("<td style=\"border-right:thin ; border-top:thin\" ><b>  {0}/{1}/{2}</b></TD>", branchCode, "S", ksm.bill_no));
                report.AppendLine("</tr>");

                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin ; border-top:thin\" align=\"left\" ><b>Date &nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin;border-top:thin\" ><b>" + billDate + "</b></td>");
                report.AppendLine("</tr>");

                int tinNo = Convert.ToInt32(ksm.pos);
                KSTS_STATE_MASTER state = db.KSTS_STATE_MASTER.Where(s => s.tinno == tinNo).FirstOrDefault();
                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" ><b> Place of supply &nbsp </b></td>");
                report.AppendLine("<td style=\"border-right:thin;border-top:thin\" ><b>");
                if (state == null) {
                    report.Append("" + "" + "</b></td>");
                }
                else {
                    report.Append("" + state.state_name + "</b></td>");
                }
                report.AppendLine("</tr>");
                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" ><b>Tax Payment mode &nbsp&nbsp </b></td>");
                report.AppendLine("<td style=\"border-right:thin; border-top:thin\" ><b>" + "Normal" + "</b></td>");
                report.AppendLine("</tr>");
                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" ><b>PAN &nbsp&nbsp </b></td>");
                report.AppendLine("<td style=\"border-right:thin ; border-top:thin\" ><b>" + company.pan_no + "</b></td>");
                report.AppendLine("</tr>");
                report.AppendLine("</table>");
                report.AppendLine("</td>");
                #endregion

                #region Showroom Details
                report.AppendLine("<td>");
                report.AppendLine("<Table font-size=12pt;   class=\"boldText\"  style=\"border-collapse:collapse;\" >");

                report.AppendLine("<tr style=\"border-right:0\"  >");
                report.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left \" ><b>Name &nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.company_name + "</b></td>");
                report.AppendLine("</tr>");

                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>Address &nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.address1 + "</b></td>");
                report.AppendLine("</tr>");
                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.address2 + "</b></td>");
                report.AppendLine("</tr>");
                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.address3 + "</b></td>");
                report.AppendLine("</tr>");

                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>City &nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.city + "</b></td>");
                report.AppendLine("</tr>");

                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>State &nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + company.state + "</b></td>");
                report.AppendLine("</tr>");

                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>State code &nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin  ;border-top:thin\" ><b>" + company.state_code + "</b></td>");
                report.AppendLine("</tr>");

                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>Phone &nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + company.phone_no + "</b></td>");
                report.AppendLine("</tr>");
                report.AppendLine("</table>");
                report.AppendLine("</td>");
                report.AppendLine("</TR>");
                report.AppendLine("</Table>");
                #endregion

                #region Report Header
                report.AppendLine("<Table  font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse; border-top:thin\" width=\"900\">");
                report.AppendLine("<TR>");
                report.AppendLine(string.Format("<TD  style=\"border-right:thin; border-top:thin \" colspan=15 ALIGN = \"CENTER\"><b>ORIGINAL / DUPLICATE <br></b></TD>"));
                report.AppendLine("</TR>");
                report.AppendLine("<TR bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b>Item</b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                report.AppendLine("<TH colspan = 3 ALIGN = \"CENTER\"><b>[Weight in Grams]</b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b>Jewel</b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b>VA</b></TH>");
                report.AppendLine("<TH  style=\"border-bottom:thin\"  ALIGN = \"CENTER\"><b>Stone/Diamond</b></TH>");
                report.AppendLine("<TH  style=\"border-bottom:thin;\"  ALIGN = \"CENTER\"><b>Gross</b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                report.AppendLine("</TR>");
                report.AppendLine("<TR bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                report.AppendLine("<TH style=\" border-bottom:thin solid ;border-top:thin \" ALIGN = \"CENTER\"><b>Sl.No</b></TH>");
                report.AppendLine("<TH style=\" border-bottom:thin solid ; border-top:thin \" ALIGN = \"CENTER\"><b>C</b></TH>");
                report.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b>Description</b></TH>");
                report.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b>HSN Code</b></TH>");
                report.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\"><b>Purity</b></TH>");
                report.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\"><b>Rate/Gram</b></TH>");
                report.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\"><b>Qty</b></TH>");
                report.AppendLine("<TH style=\" border-bottom:thin solid\" ALIGN = \"CENTER\"><b>Gr.Wt</b></TH>");
                report.AppendLine("<TH style=\" border-bottom:thin solid\" ALIGN = \"CENTER\"><b>St.Wt</b></TH>");
                report.AppendLine("<TH style=\" border-bottom:thin solid\" ALIGN = \"CENTER\"><b>Net Wt</b></TH>");
                report.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b>Amount</b></TH>");
                report.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\"><b>Amount</b></TH>");
                report.AppendLine("<TH style=\" border-bottom:thin solid ; border-top:thin\" ALIGN = \"CENTER\"><b>Amount</b></TH>");
                report.AppendLine("<TH style=\" border-bottom:thin solid ; border-top:thin\" ALIGN = \"CENTER\"><b>Amount</b></TH>");
                report.AppendLine("<TH style=\" border-bottom:thin solid ; border-top:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                report.AppendLine("</TR>");
                #endregion

                #region Report Content
                int Pcs = 0, MaxPageRow = 16;
                decimal Gwt = 0, Swt = 0, Nwt = 0, GoldValue = 0, StoneChg = 0, ValueAmt = 0, VAPercent = 0, InvoiceAmount = 0, TotalSGST = 0, TotalCGST = 0, TotalIGST = 0, dicountMc = 0, totalTaxableAmount = 0, AdditonalDiscAmount = 0, totalGrosAmount = 0, ItemGrosAmount = 0, offerDiscountAmount = 0;
                decimal netBillingGwt = 0, netBillingNwt = 0; //To take care of add wt, ded wt & vapor weight if any.
                int netBillingQty = 0;
                string VA = string.Empty;
                int slNo = 1;
                int rowCount = 0;
                foreach (KTTU_SALES_DETAILS saleItem in lstOfSalesDet) {
                    string IsPieceItem = db.ITEM_MASTER.Where(item => item.gs_code == saleItem.gs_code && item.company_code == companyCode && item.branch_code == branchCode).FirstOrDefault().IsPiece;
                    if (saleItem.va_amount > 0) {
                        VAPercent = (Convert.ToDecimal(saleItem.va_amount) * 100) / Convert.ToDecimal(saleItem.gold_value);
                        VAPercent = decimal.Round(VAPercent, 2);
                    }
                    else {
                        VAPercent = 0.00M;
                    }
                    decimal MCAmount = 0;
                    if (saleItem.making_charge_per_rs > 0) {
                        MCAmount = Convert.ToDecimal(saleItem.nwt) * Convert.ToDecimal(saleItem.making_charge_per_rs);
                    }
                    else if (Convert.ToDecimal(saleItem.mc_percent) > 0) {
                        MCAmount = (Convert.ToDecimal(saleItem.gold_value) * Convert.ToDecimal(saleItem.mc_percent)) / 100;
                    }
                    else {
                        MCAmount = Convert.ToDecimal(saleItem.mc_amount);
                    }
                    MCAmount = decimal.Round(MCAmount, 2);

                    string ItemName1 = SIGlobals.Globals.GetAliasName(db, saleItem.gs_code, saleItem.item_name, companyCode, branchCode);// saleItem.item_name.ToString();

                    int qty = Convert.ToInt32(saleItem.item_no);

                    report.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"center\"><b>{0}</b></TD>", slNo));

                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"LEFT\"><b>{0}</b></TD>", saleItem.counter_code));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"LEFT\"><b>{0}</b></TD>", ItemName1));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"LEFT\"><b>{0}</b></TD>", saleItem.HSN));


                    if (!saleItem.karat.ToString().Equals("NA"))
                        report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"CENTER\"><b>{0}</b></TD>", saleItem.karat));
                    else
                        report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"CENTER\"><b>{0}</b></TD>", "&nbsp"));

                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>", saleItem.rate, " &nbsp"));

                    if (qty == 0 && string.Compare(saleItem.item_name.ToString(), "CHSU") != 0 && string.Compare(saleItem.item_name.ToString(), "CHSU_18K") != 0) {
                        report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>"
                            , "1", "&nbsp"));
                    }
                    else {
                        report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>"
                           , saleItem.item_no, " &nbsp"));
                    }


                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", Convert.ToDecimal(saleItem.gwt) + Convert.ToDecimal(saleItem.AddWt) - Convert.ToDecimal(saleItem.DeductWt) - Convert.ToDecimal(saleItem.vaporLoss), " &nbsp"));

                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", saleItem.swt, " &nbsp"));

                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", Convert.ToDecimal(saleItem.nwt) + Convert.ToDecimal(saleItem.AddWt) - Convert.ToDecimal(saleItem.DeductWt) - Convert.ToDecimal(saleItem.vaporLoss), " &nbsp"));

                    decimal vaAmount = Convert.ToDecimal(saleItem.va_amount);
                    if (IsPieceItem.Equals("Y")) {
                        decimal goldValue = 0;
                        report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", goldValue, "&nbsp"));
                        GoldValue += goldValue;
                    }
                    else {
                        decimal wastAmt = 0;
                        decimal goldValue = Convert.ToDecimal(saleItem.gold_value);
                        decimal wastGrams = Convert.ToDecimal(saleItem.wastage_grms);
                        if (wastGrams > 0) {
                            wastAmt = decimal.Round((wastGrams * Convert.ToDecimal(saleItem.rate)), 2);
                        }
                        GoldValue += goldValue;
                        report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", goldValue, "&nbsp"));
                    }

                    decimal StoneAmt = Convert.ToDecimal(saleItem.stone_charges)
                        + Convert.ToDecimal(saleItem.diamond_charges)
                        + Convert.ToDecimal(saleItem.hallmarcharges);

                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", Convert.ToDecimal(saleItem.va_amount), "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>", StoneAmt, "&nbsp"));

                    Gwt += Convert.ToDecimal(saleItem.gwt);
                    Swt += Convert.ToDecimal(saleItem.swt);
                    Nwt += Convert.ToDecimal(saleItem.nwt);
                    netBillingGwt += (Convert.ToDecimal(saleItem.gwt) + Convert.ToDecimal(saleItem.AddWt) - Convert.ToDecimal(saleItem.DeductWt) - Convert.ToDecimal(saleItem.vaporLoss));
                    netBillingNwt += (Convert.ToDecimal(saleItem.nwt) + Convert.ToDecimal(saleItem.AddWt) - Convert.ToDecimal(saleItem.DeductWt) - Convert.ToDecimal(saleItem.vaporLoss));
                    netBillingQty += (Convert.ToInt32(saleItem.item_no) + Convert.ToInt32(saleItem.Addqty) - Convert.ToInt32(saleItem.Deductqty));
                    dicountMc += Convert.ToDecimal(saleItem.Mc_Discount_Amt);
                    StoneChg += StoneAmt;
                    ValueAmt += Convert.ToDecimal(saleItem.va_amount);
                    if (IsPieceItem.Equals("Y"))
                        ItemGrosAmount = Convert.ToDecimal(saleItem.gold_value) + Convert.ToDecimal(saleItem.va_amount);
                    else
                        ItemGrosAmount = Convert.ToDecimal(saleItem.gold_value) + StoneAmt + Convert.ToDecimal(saleItem.va_amount);

                    totalGrosAmount += ItemGrosAmount;
                    AdditonalDiscAmount += Convert.ToDecimal(saleItem.item_additional_discount);
                    offerDiscountAmount += Convert.ToDecimal(saleItem.offer_value);
                    totalTaxableAmount += Convert.ToDecimal(saleItem.item_total_after_discount);
                    TotalSGST += Convert.ToDecimal(saleItem.SGST_Amount);
                    TotalCGST += Convert.ToDecimal(saleItem.CGST_Amount);
                    TotalIGST += Convert.ToDecimal(saleItem.IGST_Amount);

                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", ItemGrosAmount, "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0} </b></TD>", saleItem.sal_code));
                    Pcs += Convert.ToInt32(saleItem.item_no);
                    InvoiceAmount += Convert.ToDecimal(saleItem.item_total_after_discount);

                    if (rowCount > MaxPageRow && (rowCount % MaxPageRow) == 0 && rowCount < lstOfSalesDet.Count) {
                        MaxPageRow = 30;
                        report.AppendLine("<TR>");
                        report.AppendLine(string.Format("<TD colspan =10 style=\"border-right:thin\" ALIGN = \"LEFT\"><b>{0}<br><br><br><br><br><br>{1}</b></TD>", "Received ornaments in good condition", "Customer Signature"));
                        report.AppendLine(string.Format("<TD colspan =11 ALIGN = \"RIGHT\"><b>For {0}<br><br><br><br><br><br>{1}</b></TD>", company.company_name, "Authorized Signatory"));
                        report.AppendLine("</TR>");
                        report.AppendLine("</Table >");
                        report.AppendLine("<DIV style=\"page-break-after:always\"></DIV>");
                        report.AppendLine("<Table frame=\"border\" border=\"0\" width=\"1100\">");

                        for (int j = 0; j < 10; j++) {
                            report.AppendLine("<TR style=border:0>");
                            report.AppendLine(string.Format("<TD style=border:0 colspan = 12 ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                            report.AppendLine("</TR>");
                        }

                        report.AppendLine("</table>");
                        report.AppendLine("<Table  font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse; border-top:thin\" width=\"900\">");  //FRAME=BOX RULES=NONE

                        report.AppendLine("<TR bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                        report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                        report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                        report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b>Item</b></TH>");
                        report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                        report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                        report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                        report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                        report.AppendLine("<TH colspan = 3 ALIGN = \"CENTER\"><b>[Weight in Grams]</b></TH>");

                        report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b>Jewel</b></TH>");
                        report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b>VA</b></TH>");
                        report.AppendLine("<TH  style=\"border-bottom:thin\"  ALIGN = \"CENTER\"><b>Stone/Diamond</b></TH>");
                        report.AppendLine("<TH  style=\"border-bottom:thin;\"  ALIGN = \"CENTER\"><b>Gross</b></TH>");

                        report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b></b></TH>");

                        report.AppendLine("</TR>");

                        report.AppendLine("<TR bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                        report.AppendLine("<TH style=\" border-bottom:thin solid ;border-top:thin \" ALIGN = \"CENTER\"><b>Sl.No</b></TH>");
                        report.AppendLine("<TH style=\" border-bottom:thin solid ; border-top:thin \" ALIGN = \"CENTER\"><b>C</b></TH>");
                        report.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b>Description</b></TH>");
                        report.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b>HSN Code</b></TH>");
                        report.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\"><b>Purity</b></TH>");
                        report.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\"><b>Rate/Gram</b></TH>");
                        report.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\"><b>Qty</b></TH>");
                        report.AppendLine("<TH style=\" border-bottom:thin solid\" ALIGN = \"CENTER\"><b>Gr.Wt</b></TH>");
                        report.AppendLine("<TH style=\" border-bottom:thin solid\" ALIGN = \"CENTER\"><b>St.Wt</b></TH>");
                        report.AppendLine("<TH style=\" border-bottom:thin solid\" ALIGN = \"CENTER\"><b>Net Wt</b></TH>");

                        report.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b>Amount</b></TH>");
                        report.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\"><b>Amount</b></TH>");

                        report.AppendLine("<TH style=\" border-bottom:thin solid ; border-top:thin\" ALIGN = \"CENTER\"><b>Amount</b></TH>");
                        report.AppendLine("<TH style=\" border-bottom:thin solid ; border-top:thin\" ALIGN = \"CENTER\"><b>Amount</b></TH>");
                        report.AppendLine("<TH style=\" border-bottom:thin solid ; border-top:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                        report.AppendLine("</TR>");
                    }
                    rowCount = rowCount + 1;
                    slNo = slNo + 1;
                }

                for (int j = 0; j < MaxPageRow - lstOfSalesDet.Count; j++) {
                    report.AppendLine("<TR>");
                    report.AppendLine(string.Format("<TD  style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\"  ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine("</TR>");
                }
                report.AppendLine("<TR  bgcolor='#FFFACD'  style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                report.AppendLine("<TH  style=\"border-bottom:thin solid\" colspan=6 ALIGN = \"LEFT\"><b>Totals</b></TH>");
                report.AppendLine(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TH>", Pcs, "&nbsp"));
                report.AppendLine(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TH>", netBillingGwt, "&nbsp"));
                report.AppendLine(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TH>", Swt, "&nbsp"));
                report.AppendLine(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TH>", netBillingNwt, "&nbsp"));
                report.AppendLine(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TH>", GoldValue, "&nbsp"));
                report.AppendLine(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TH>", ValueAmt, "&nbsp"));
                report.AppendLine(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TH>", StoneChg, "&nbsp"));
                report.AppendLine(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TH>", totalGrosAmount, "&nbsp"));
                report.AppendLine("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\"><b></b></TH>");
                report.AppendLine("</TR>");

                #region Stone Details
                List<KTTU_SALES_STONE_DETAILS> lstOfStoneDet = db.KTTU_SALES_STONE_DETAILS.Where(ssd => ssd.bill_no == billNo
                                                                && ssd.company_code == companyCode
                                                                && ssd.branch_code == branchCode).OrderBy(s => s.est_Srno).ToList();

                if (lstOfStoneDet != null && lstOfStoneDet.Count > 0) {
                    List<string> Stonelst = null;
                    string stone = string.Empty;
                    for (int j = 0; j < lstOfStoneDet.Count; j++) {
                        string StoneName = lstOfStoneDet[j].name.ToString();
                        StoneName = StoneName.Trim();
                        string[] arr = StoneName.Split('-');

                        if (j == 0 || (!lstOfStoneDet[j].est_Srno.Equals(lstOfStoneDet[j - 1].est_Srno))) {
                            stone += string.Format("{0}{1}{2}{3}{4}{5}{6} ", lstOfStoneDet[j].est_Srno, ")", arr[0], "\\", lstOfStoneDet[j].rate, "*", lstOfStoneDet[j].carrat);
                        }
                        else {
                            stone += string.Format("{0}{1}{2}{3}{4} ", arr[0], "\\", lstOfStoneDet[j].rate, "*", lstOfStoneDet[j].carrat);
                        }
                    }
                    report.AppendLine("<TR>");
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"LEFT\"><b>{0}{1}</b></TD><TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"LEFT\"><b>{1}</b></TD><TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"LEFT\"><b>{1}</b></TD>", stone, "&nbsp"));
                    report.AppendLine("</TR>");
                    if (Stonelst != null && Stonelst.Count > 1) {
                        for (int x = 1; x < Stonelst.Count; x++) {
                            report.AppendLine("<TR>");
                            report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"LEFT\"><b>{0}{1}</b></TD> <TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"LEFT\"><b></b></TD><TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"LEFT\"><b></b></TD>", "- ", Stonelst[x].Trim()));
                            report.AppendLine("</TR>");
                        }

                        stone = string.Empty;
                        Stonelst = null;
                    }
                }
                #endregion

                if (Convert.ToDecimal(AdditonalDiscAmount) > 0 || offerDiscountAmount > 0) {
                    report.AppendLine("<TR>");
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><b>Discount</b></TD>"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", AdditonalDiscAmount + offerDiscountAmount, "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    report.AppendLine("</TR>");
                }
                if (Convert.ToDecimal(dicountMc) > 0) {
                    report.AppendLine("<TR>");
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><b>Discount</b></TD>"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", decimal.Round(dicountMc, 2), "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    report.AppendLine("</TR>");
                }

                if (Convert.ToDecimal(ksm.excise_duty_amount) > 0) {
                    report.AppendLine("<TR>");
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><b>Excise Duty @ {0}%</b></TD>", ksm.excise_duty_percent));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", ksm.excise_duty_amount, "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    report.AppendLine("</TR>");
                    InvoiceAmount += Convert.ToDecimal(ksm.excise_duty_amount);
                }
                report.AppendLine("<TR>");
                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  colspan = 13 ALIGN = \"RIGHT\"><b>Taxable Value</b></TD>"));
                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \"  ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", totalTaxableAmount, "&nbsp"));
                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                report.AppendLine("</TR>");

                report.AppendLine("<TR>");
                report.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-top:thin\" colspan = 13 ALIGN = \"RIGHT\"><b>SGST {0}%</b></TD>", lstOfSalesDet[0].SGST_Percent));
                report.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-top:thin\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", decimal.Round(TotalSGST, 2), "&nbsp"));
                report.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-top:thin\" ALIGN = \"LEFT\"><b>{0}</b></TD>", "&nbsp"));
                report.AppendLine("</TR>");

                report.AppendLine("<TR>");
                report.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-top:thin\" colspan = 13 ALIGN = \"RIGHT\"><b>CGST {0}% </b></TD>", lstOfSalesDet[0].CGST_Percent));
                report.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-top:thin\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", decimal.Round(TotalCGST, 2), "&nbsp"));
                report.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-top:thin\" ALIGN = \"LEFT\"><b>{0}</b></TD>", "&nbsp"));
                report.AppendLine("</TR>");

                report.AppendLine("<TR>");
                report.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-top:thin\" colspan = 13 ALIGN = \"RIGHT\"><b>IGST {0}%</b></TD>", lstOfSalesDet[0].IGST_Percent));
                report.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-top:thin\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", decimal.Round(TotalIGST, 2), "&nbsp"));
                report.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-top:thin\" ALIGN = \"LEFT\"><b>{0}</b></TD>", "&nbsp"));
                report.AppendLine("</TR>");

                InvoiceAmount += TotalIGST + TotalSGST + TotalCGST;
                decimal round_off = Convert.ToDecimal(lstOfSalesDet[0].round_off);
                InvoiceAmount += round_off;
                decimal rndOff = Math.Round(InvoiceAmount, 0, MidpointRounding.ToEven) - InvoiceAmount;
                if (Math.Abs(rndOff) <= 0.02M) {
                    round_off += rndOff;
                    InvoiceAmount = Math.Round(InvoiceAmount, 0, MidpointRounding.ToEven);
                }
                decimal InvoiceAmountInWords = InvoiceAmount;
                report.AppendLine("<TR>");
                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><b>Invoice Amount</b></TD>"));
                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", Math.Round(InvoiceAmountInWords, 0, MidpointRounding.ToEven).ToString("F"), "&nbsp"));
                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                report.AppendLine("</TR>");

                decimal PaidAmount = 0;
                List<KTTU_PAYMENT_DETAILS> lstOfPayment = db.KTTU_PAYMENT_DETAILS.Where(pay => (pay.trans_type == "S" || pay.trans_type == "PC")
                                                                    && (pay.sal_bill_type == null || pay.sal_bill_type != "CR")
                                                                    && pay.pay_mode != "CT"
                                                                    && pay.pay_mode != "C"
                                                                    && pay.company_code == companyCode
                                                                    && pay.branch_code == branchCode
                                                                    && pay.series_no == billNo).ToList();

                if (lstOfPayment.Count > 0) {
                    for (int i = 0; i < lstOfPayment.Count; i++) {
                        if (string.Compare(lstOfPayment[i].trans_type.ToString(), "S") == 0) {
                            decimal PayAmt = Convert.ToDecimal(lstOfPayment[i].pay_amt);
                            string payMode = lstOfPayment[i].pay_mode.ToString();
                            string branch = payMode.Substring(0, 1);
                            if (string.Compare(payMode, "Q") == 0) {
                                report.AppendLine("<TR>");
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><b>By Cheque Drawn on/{0}/{1} </b></TD>"
                                    , lstOfPayment[i].bank_name.ToString().Trim(), lstOfPayment[i].cheque_no.ToString().Trim()));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", PayAmt.ToString("N"), "&nbsp"));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                                report.AppendLine("</TR>");

                            }
                            else if (string.Compare(payMode, "R") == 0) {
                                string Bank = string.Empty;
                                if (lstOfPayment[i].bank.ToString() != string.Empty) {
                                    Bank = lstOfPayment[i].bank.ToString().Trim();
                                }

                                string approvalNo = lstOfPayment[i].card_app_no.ToString().Trim();
                                string card = lstOfPayment[i].Ref_BillNo.ToString().Trim();
                                if (card.Length == 16)
                                    card = "XXXX XXXX XXXX " + card.Substring(12, 4);
                                else if (card.Length == 19)
                                    card = "XXXX XXXX XXXX " + card.Substring(15, 4);
                                else
                                    card = lstOfPayment[i].Ref_BillNo.ToString().Trim();

                                report.AppendLine("<TR>");
                                report.AppendLine(string.Format("<TD colspan=13 ALIGN = \"RIGHT\"><b>Card/{0}/{1}/{2}/{3}/{4} </b></TD>"
                                    , lstOfPayment[i].bank_name.ToString().Trim(), Bank, lstOfPayment[i].card_type.ToString().Trim()
                                    , card, approvalNo));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", PayAmt.ToString("N"), "&nbsp"));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                                report.AppendLine("</TR>");
                            }
                            else if (string.Compare(payMode, "D") == 0) {
                                report.AppendLine("<TR>");
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><b>DD NO/{0} </b></TD>"
                                    , lstOfPayment[i].Ref_BillNo));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", PayAmt.ToString("N"), "&nbsp"));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                                report.AppendLine("</TR>");
                            }
                            else if (string.Compare(payMode, "OP") == 0) {
                                if (PayAmt > 0) {
                                    int orderNo = Convert.ToInt32(lstOfPayment[i].Ref_BillNo);
                                    KTTU_ORDER_MASTER order = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == orderNo
                                                                        && ord.company_code == companyCode
                                                                        && ord.branch_code == branchCode).FirstOrDefault();

                                    report.AppendLine("<TR>");
                                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><b>Less Advance As per Order No.{0}/{3}/{1} Dated/ {2} </b></TD>"
                                       , order.branch_code, lstOfPayment[i].Ref_BillNo, order.order_date.ToString("dd/MM/yyyy"), "O"));
                                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", PayAmt.ToString("N"), "&nbsp"));
                                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                                    report.AppendLine("</TR>");

                                }
                            }
                            else if (string.Compare(payMode, "PB") == 0) {
                                report.AppendLine("<TR>");
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><b>Purchase (Bill No: {0}/{2}/{1} Adjusted) </b></TD>"
                                        , branchCode, lstOfPayment[i].Ref_BillNo, "P"));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", PayAmt.ToString("N"), "&nbsp"));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                                report.AppendLine("</TR>");

                            }
                            else if (string.Compare(payMode, "SR") == 0) {
                                report.AppendLine("<TR>");
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><b>Sales Return (CR Note No: {0} Adjusted)</b></TD>"
                                       , lstOfPayment[i].Ref_BillNo));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", PayAmt.ToString("N"), "&nbsp"));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                                report.AppendLine("</TR>");
                            }
                            else if (string.Compare(branch, "B") == 0) {
                                report.AppendLine("<TR>");
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><b> {0}- Branch (Order No: {1} Adjusted)</b></TD>"
                                        , lstOfPayment[i].party_code, lstOfPayment[i].Ref_BillNo));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", PayAmt.ToString("N"), "&nbsp"));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                                report.AppendLine("</TR>");
                            }
                            else if (string.Compare(lstOfPayment[i].pay_mode.ToString(), "EP") == 0) {
                                report.AppendLine("<TR>");
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><b>E-Payment ({0})</b></TD>", lstOfPayment[i].pay_details));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", PayAmt.ToString("N"), "&nbsp"));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                                report.AppendLine("</TR>");
                            }
                            else if (string.Compare(lstOfPayment[i].pay_mode.ToString(), "GV") == 0) {
                                report.AppendLine("<TR>");
                                report.AppendLine(string.Format("<TD colspan=13 ALIGN = \"RIGHT\"><b>Gift Voucher ({0}) </b></TD>"
                                       , lstOfPayment[i].pay_details));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", PayAmt.ToString("N"), "&nbsp"));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                                report.AppendLine("</TR>");
                            }
                            //else {
                            //    report.AppendLine("<TR>");
                            //    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><b>Others Adjusted    </b></TD>"));
                            //    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", PayAmt.ToString("N"), "&nbsp"));
                            //    report.AppendLine(string.Format("<TD  style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                            //    report.AppendLine("</TR>");
                            //}

                            else if (string.Compare(Convert.ToString(lstOfPayment[i].pay_mode), "QC") == 0) {
                                report.AppendLine("<TR>");
                                report.AppendLine(string.Format("<TD colspan=13 ALIGN = \"RIGHT\"><b>Gift Card ({0}) </b></TD>"
                                       , lstOfPayment[i].Ref_BillNo));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", PayAmt.ToString("N"), "&nbsp"));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                                report.AppendLine("</TR>");
                            }
                            else if (string.Compare(Convert.ToString(lstOfPayment[i].pay_mode), "SP") == 0) {
                                report.AppendLine("<TR>");
                                report.AppendLine(string.Format("<TD colspan=13 ALIGN = \"RIGHT\"><b>Sales Promotion {0} </b></TD>"
                                       , lstOfPayment[i].Ref_BillNo));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", PayAmt.ToString("N"), "&nbsp"));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                                report.AppendLine("</TR>");
                            }
                            else {
                                //string payname = CGlobals.GetStringValue(string.Format("select payment_name from ktts_payment_master where payment_code='{0}' and company_code='{1}' and branch_code ='{2}'", payMode, CGlobals.CompanyCode, CGlobals.BranchCode));
                                string otherPayMode = Convert.ToString(lstOfPayment[i].pay_mode);
                                KTTS_PAYMENT_MASTER paymentMaster = db.KTTS_PAYMENT_MASTER.Where(p => p.payment_code == otherPayMode && p.company_code == companyCode && p.branch_code == branchCode).FirstOrDefault();
                                if (paymentMaster != null) {
                                    report.AppendLine("<TR>");
                                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><b> {0} - {1} </b></TD>", paymentMaster.payment_name, lstOfPayment[i].pay_details));
                                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", PayAmt.ToString("N"), "&nbsp"));
                                    report.AppendLine(string.Format("<TD  style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                                    report.AppendLine("</TR>");
                                }
                            }
                            PaidAmount += PayAmt;
                        }
                    }
                }

                List<KTTU_PAYMENT_DETAILS> lstOfPaymentChit = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.trans_type == "S"
                                                                    && (pay.sal_bill_type == null || pay.sal_bill_type != "CR")
                                                                    && pay.pay_mode == "CT"
                                                                    && pay.company_code == companyCode
                                                                    && pay.branch_code == branchCode
                                                                    && pay.series_no == billNo).ToList();
                if (lstOfPaymentChit.Count > 0) {
                    bool print = false;
                    decimal chitAmount = 0;
                    decimal bonusAmount = 0;
                    decimal winerAmount = 0;
                    string definition = string.Empty;
                    for (int k = 0; k < lstOfPaymentChit.Count; k++) {
                        definition += string.Format("{0}/{1}/{2} ", lstOfPaymentChit[k].scheme_code, lstOfPaymentChit[k].group_code, lstOfPaymentChit[k].Ref_BillNo);
                        chitAmount += Convert.ToDecimal(lstOfPaymentChit[k].amt_received);
                        bonusAmount += Convert.ToDecimal(lstOfPaymentChit[k].bonus_amt);
                        winerAmount += Convert.ToDecimal(lstOfPaymentChit[k].win_amt);

                        if (lstOfPaymentChit.Count == 1 || (lstOfPaymentChit.Count > 1 && k > 0)) {
                            if ((lstOfPaymentChit.Count == 1) || (lstOfPaymentChit.Count == k + 1)) {
                                print = true;
                            }
                        }

                        if (k < lstOfPaymentChit.Count - 1 && (Convert.ToInt32(lstOfPaymentChit[k].Ref_BillNo) != (Convert.ToInt32(lstOfPaymentChit[k + 1].Ref_BillNo) - 1)
                            || !(lstOfPaymentChit[k + 1].scheme_code.Equals(lstOfPaymentChit[k].scheme_code)) || !(lstOfPaymentChit[k + 1].group_code.Equals(lstOfPaymentChit[k].group_code)))) {
                            print = true;
                        }

                        if (print) {
                            string[] chiNoPatch = definition.Split(' ');
                            definition = chiNoPatch[0];
                            if (chiNoPatch.Length > 2) {
                                definition += " To " + chiNoPatch[chiNoPatch.Length - 2];
                            }
                            report.AppendLine("<TR>");
                            report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><b>Jewel Savings Scheme {0} </b></TD>"
                                , definition.Trim()));
                            report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", chitAmount.ToString("N"), "&nbsp"));
                            report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                            report.AppendLine("</TR>");
                            if (bonusAmount > 0) {
                                report.AppendLine("<TR>");
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><b> Additional Discount  </b></TD>"));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", bonusAmount.ToString("N"), "&nbsp"));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                                report.AppendLine("</TR>");
                            }
                            if (winerAmount > 0) {
                                report.AppendLine("<TR>");
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><b> Scheme Winner  </b></TD>"));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", winerAmount.ToString("N"), "&nbsp"));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                                report.AppendLine("</TR>");
                            }

                            definition = string.Empty;
                            chitAmount = 0;
                            bonusAmount = 0;
                            winerAmount = 0;
                            print = false;
                        }
                        PaidAmount += Convert.ToDecimal(lstOfPaymentChit[k].amt_received);
                        PaidAmount += Convert.ToDecimal(lstOfPaymentChit[k].bonus_amt);
                        PaidAmount += Convert.ToDecimal(lstOfPaymentChit[k].win_amt);
                    }
                }

                List<KTTU_PAYMENT_DETAILS> lstOfPayDet = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.pay_mode == "C"
                                                                            && pay.trans_type == "S"
                                                                            && pay.series_no == billNo
                                                                            && pay.company_code == companyCode
                                                                            && pay.branch_code == branchCode).ToList();
                if (lstOfPayDet.Count > 0) {
                    for (int i = 0; i < lstOfPayDet.Count; i++) {
                        if (string.Compare(lstOfPayDet[i].trans_type.ToString(), "S") == 0) {
                            decimal PayAmt = Convert.ToDecimal(lstOfPayDet[i].pay_amt);
                            report.AppendLine("<TR>");
                            report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><h3>Cash Received</h3></TD>"));
                            report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><h3>{0}{1}{1}</h3></TD>", PayAmt.ToString("N"), "&nbsp"));
                            report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                            report.AppendLine("</TR>");
                            PaidAmount += PayAmt;
                        }
                    }
                }
                InvoiceAmount = InvoiceAmount - PaidAmount;
                InvoiceAmount = Math.Round(InvoiceAmount, 0, MidpointRounding.ToEven);

                if (InvoiceAmount < 0 && InvoiceAmount != -1) {
                    report.AppendLine("<TR>");
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><b>Payable Amount </b></TD>"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", (-InvoiceAmount).ToString("N", indianDefaultCulture), "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    report.AppendLine("</TR>");

                    lstOfPayment = null;
                    lstOfPayment = db.KTTU_PAYMENT_DETAILS.Where(pay => (pay.trans_type == "S" || pay.trans_type == "PC")
                                                    && (pay.sal_bill_type == null || pay.sal_bill_type != "CR")
                                                    && pay.company_code == companyCode
                                                    && pay.branch_code == branchCode
                                                    && pay.series_no == billNo).ToList();

                    if (lstOfPayment != null && lstOfPayment.Count > 0) {
                        for (int i = 0; i < lstOfPayment.Count; i++) {
                            string transtype = lstOfPayment[i].trans_type.ToString();
                            string paymode = lstOfPayment[i].pay_mode.ToString();
                            decimal PayAmt = Convert.ToDecimal(lstOfPayment[i].pay_amt.ToString());
                            PayAmt = Math.Round(PayAmt, 0, MidpointRounding.ToEven);
                            if (string.Compare(transtype, "PC") == 0 && string.Compare(paymode, "OP") == 0) {
                                report.AppendLine("<TR>");
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><b>Adjusted Towards Order No {0} </b></TD>", lstOfPayment[i].Ref_BillNo));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", PayAmt.ToString("N", indianDefaultCulture), "&nbsp"));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                                report.AppendLine("</TR>");
                            }
                            else if (string.Compare(transtype, "PC") == 0 && string.Compare(paymode, "C") == 0) {
                                report.AppendLine("<TR>");
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><b>(Paid to Customer) Cash paid </b></TD>"));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", PayAmt.ToString("N", indianDefaultCulture), "&nbsp"));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                                report.AppendLine("</TR>");
                            }
                            else if (string.Compare(transtype, "PC") == 0 && string.Compare(paymode, "Q") == 0) {
                                int bankCode = Convert.ToInt32(lstOfPayment[i].bank);
                                string bankname = db.KSTU_ACC_LEDGER_MASTER.Where(ac => ac.acc_code == bankCode && ac.branch_code == branchCode && ac.company_code == companyCode).FirstOrDefault().acc_name;
                                report.AppendLine("<TR>");
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><b> (Paid to Customer) Cheque Issued {0}/{1} </b></TD>", bankname, lstOfPayment[i].cheque_no));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", PayAmt.ToString("N", indianDefaultCulture), "&nbsp"));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                                report.AppendLine("</TR>");
                            }
                            else if (string.Compare(transtype, "PC") == 0 && string.Compare(paymode, "EP") == 0) {
                                report.AppendLine("<TR>");
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><b>(Paid to Customer) E Payment </b></TD>"));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", PayAmt.ToString("N", indianDefaultCulture), "&nbsp"));
                                report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                                report.AppendLine("</TR>");
                            }
                        }
                    }

                    // Added Code on July 1, 2021
                    //if ((-InvoiceAmount) >= 1) {
                    //    report.AppendLine("<TR>");
                    //    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><b>Balance</b></TD>"));
                    //    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", Math.Round(-InvoiceAmount, 2, MidpointRounding.ToEven), "&nbsp"));
                    //    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    //    report.AppendLine("</TR>");
                    //}
                    //else if (InvoiceAmount == 0 || InvoiceAmount <= 1) {
                    //    report.AppendLine("<TR>");
                    //    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><b>Balance</b></TD>"));
                    //    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", "NIL", "&nbsp"));
                    //    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    //    report.AppendLine("</TR>");
                    //}
                }
                else if (InvoiceAmount > 0 && InvoiceAmount > 1) {
                    report.AppendLine("<TR>");
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><b>Balance</b></TD>"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", Math.Round(InvoiceAmount, 0, MidpointRounding.ToEven), "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    report.AppendLine("</TR>");
                }
                else if (InvoiceAmount == 0 || InvoiceAmount <= 1) {
                    report.AppendLine("<TR>");
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" colspan=13 ALIGN = \"RIGHT\"><b>Balance</b></TD>"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", "NIL", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    report.AppendLine("</TR>");
                }

                string strWords = string.Empty;
                NumberToWords objNWClass = new NumberToWords();
                //objNWClass.ConvertNumberToWords(Convert.ToDouble(InvoiceAmountInWords), out strWords);
                objNWClass.ConvertNumberToWords(Convert.ToDouble(Math.Round(InvoiceAmountInWords, 0, MidpointRounding.ToEven)), out strWords);
                strWords = strWords.Replace("Rupees", "");
                strWords = "Rupees " + strWords;
                decimal RoundOffAmt = rndOff;
                if (RoundOffAmt != 0) {
                    report.AppendLine("<TR>");
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; \" colspan = 15 ALIGN = \"LEFT\"><b>{0} (Round.Off {1} Rs.Ps) </b></TD>", strWords, RoundOffAmt));
                    report.AppendLine("</TR>");

                }
                else {
                    report.AppendLine("<TR>");
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin; \" colspan = 15 ALIGN = \"LEFT\"><b>{0}</b></TD>", strWords));
                    report.AppendLine("</TR>");
                }
                if (Convert.ToInt32(ksm.est_no.ToString()) > 0) {
                    report.AppendLine("<TR>");
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin ;   border-top:thin \" colspan=15 ALIGN = \"left\"><b>Est no :{0} </b></TD>", ksm.est_no.ToString(), " &nbsp"));
                    report.AppendLine("</TR>");
                }
                report.AppendLine("<TR>");
                report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=15 ALIGN = \"left\"><b>OPR :{0} </b></TD>", ksm.operator_code.ToString(), " &nbsp"));
                report.AppendLine("</TR>");
                report.AppendLine("<TR>");
                report.AppendLine(string.Format("<TD  style=\"border-right:thin\"  colspan = 7 ALIGN = \"LEFT\"><b>{0}<br><br><br><br><br>{1}</b></TD>", "Received ornaments in good condition", "Customer Signature"));
                report.AppendLine(string.Format("<TD colspan = 8 ALIGN = \"RIGHT\"><b>For {0}<br><br><br><br><br>{1}</b></TD>", company.company_name, "Authorized Signatory"));
                report.AppendLine("</TR>");

                if (company.Header3.ToString() != string.Empty) {
                    report.AppendLine("<TR>");
                    report.AppendLine(string.Format("<TD   style=\"border-bottom:thin; \"  colspan = 15 ALIGN = \"CENTER\"><b>{0}</b></TD>", company.Header3.ToString()));
                    report.AppendLine("</TR>");
                }
                if (company.Header4.ToString() != string.Empty) {
                    report.AppendLine("<TR>");
                    report.AppendLine(string.Format("<TD   style=\"border-bottom:thin; border-top:thin \"  colspan = 15 ALIGN = \"CENTER\"><b>{0}</b></TD>", company.Header4.ToString()));
                    report.AppendLine("</TR>");
                }

                string footer = string.Empty;
                KTTU_NOTE note = db.KTTU_NOTE.Where(n => n.note_type == "Sal").FirstOrDefault();
                if (note != null) {
                    footer = note.line1 + note.line2 + note.line3 + note.line4 + note.line5 + note.line6 + note.line7 + note.line8 + note.line9 + note.line10;
                }
                report.AppendLine("<TR>");
                report.AppendLine(string.Format("<TD colspan = 15;font-size: 8pt; ALIGN = \"CENTER\"><b>{0}</b></TD>", footer));

                report.AppendLine("</TR>");
                report.AppendLine("</TABLE>");
                #endregion

                report.AppendLine("</body>");
                report.AppendLine("</html>");
                return report.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        public ProdigyPrintVM PrintSalesBill(string companyCode, string branchCode, int billNo, out ErrorVM error)
        {
            error = null;
            string printData = PrintSalesBilling(companyCode, branchCode, billNo, out error);
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            printObject.PrintType = "HTML";
            printObject.ContinueNextPrint = true;
            printObject.Data = new PrintConfiguration().Base64Encode(printData);
            return printObject;
        }

        public SalesMasterVM GetDiscountCalculationModel(SalesMasterVM sales, out ErrorVM error)
        {
            error = null;
            using (var transaction = db.Database.BeginTransaction()) {
                try {

                    #region Sales Master
                    KTTU_SALES_MASTER salesMaster = new KTTU_SALES_MASTER();
                    salesMaster.est_no = sales.EstimationNo;
                    salesMaster.cust_Id = sales.CustomerID;
                    salesMaster.cust_name = sales.CustomerName;
                    salesMaster.order_no = sales.OrderNo;
                    salesMaster.order_date = sales.OrderDate;
                    salesMaster.order_amount = sales.OrderAmount;
                    salesMaster.bill_date = sales.BillDate;
                    salesMaster.operator_code = sales.OperatorCode;
                    salesMaster.karat = sales.Karat;
                    salesMaster.rate = sales.Rate;
                    salesMaster.tax = sales.Tax;
                    salesMaster.cess_tax = sales.CessTax;
                    salesMaster.discount_amount = sales.DiscountAmount;
                    salesMaster.total_cess_amount = sales.TotalCessAmount;
                    salesMaster.total_tax_amount = sales.TotalTaxAmount;
                    salesMaster.total_bill_amount = sales.TotalBillAmount;
                    salesMaster.grand_total = sales.GrandTotal;
                    salesMaster.purchase_amount = sales.PurchaseAmount;
                    salesMaster.spurchase_amount = sales.SpurchaseAmount;
                    salesMaster.s_type = sales.Stype;
                    salesMaster.cflag = sales.Cflag;
                    salesMaster.cancelled_by = sales.CancelledBy;
                    salesMaster.discount_autharity = sales.DiscountAutharity;
                    salesMaster.purchase_no = sales.PurchaseNo;
                    salesMaster.spurchase_no = sales.SpurchaseNo;
                    salesMaster.gstype = sales.GStype;
                    salesMaster.iscredit = sales.IsCredit;
                    salesMaster.bill_counter = sales.BillCounter;
                    salesMaster.is_ins = sales.Is_Ins;
                    salesMaster.remarks = sales.CancelledRemarks;
                    salesMaster.guranteename = sales.GuranteeName;
                    salesMaster.duedate = sales.DueDate;
                    salesMaster.balance_amt = sales.BalanceAmt;
                    salesMaster.new_cust = sales.NewCust;
                    salesMaster.SalesbillPK = sales.SalesBillPK;
                    salesMaster.item_set = sales.ItemSet;
                    salesMaster.total_sale_amount = sales.TotalSaleAmount;
                    salesMaster.ed_cess = sales.EdCess;
                    salesMaster.hed_cess = sales.HedCess;
                    salesMaster.ed_cess_percent = sales.EdCessPercent;
                    salesMaster.hed_cess_percent = sales.HedCessPercent;
                    salesMaster.excise_duty_percent = sales.ExciseDutyPercent;
                    salesMaster.excise_duty_amount = sales.ExciseDutyAmount;
                    salesMaster.fin_year = sales.FinalYear;
                    salesMaster.cancelled_remarks = sales.CancelledBy;
                    salesMaster.hi_scheme_amount = sales.HISchemeAmount;
                    salesMaster.hi_scheme_no = sales.HISchemeNo;
                    salesMaster.hi_bonus_amount = sales.HIBonusAmount;
                    salesMaster.salutation = sales.Salutation;
                    salesMaster.New_Bill_No = sales.NewBillNo;
                    salesMaster.round_off = sales.RoundOff;
                    salesMaster.refered_by = sales.ReferedBy;
                    salesMaster.ShiftID = sales.ShiftID;
                    salesMaster.book_no = sales.BookNo;
                    salesMaster.ref_invoice_no = sales.RefInvoiceNo;
                    salesMaster.flight_no = sales.FlightNo;
                    salesMaster.race = sales.Race;
                    salesMaster.manger_code = sales.MangerCode;
                    salesMaster.bill_type = sales.BillType;
                    salesMaster.isPAN = sales.IsPAN;
                    salesMaster.UniqRowID = Guid.NewGuid();
                    salesMaster.address1 = sales.Address1;
                    salesMaster.address2 = sales.Address2;
                    salesMaster.address3 = sales.Address3;
                    salesMaster.city = sales.City;
                    salesMaster.pin_code = sales.PinCode;
                    salesMaster.mobile_no = sales.MobileNo;
                    salesMaster.state = sales.State;
                    salesMaster.state_code = sales.StateCode;
                    salesMaster.tin = sales.Tin;
                    salesMaster.pan_no = sales.PANNo;
                    salesMaster.pos = sales.pos;
                    salesMaster.Corparate_ID = sales.CorparateID;
                    salesMaster.Corporate_Branch_ID = sales.CorporateBranchID;
                    salesMaster.Employee_Id = sales.EmployeeID;
                    salesMaster.Registered_MN = sales.RegisteredMN;
                    salesMaster.profession_ID = sales.ProfessionID;
                    salesMaster.Empcorp_Email_ID = sales.EmpcorpEmailID;
                    db.KTTU_SALES_MASTER.Add(salesMaster);
                    #endregion

                    #region Sales Details
                    foreach (SalesDetailsVM sdvm in sales.salesDetailsVM) {
                        KTTU_SALES_DETAILS salesDetails = new KTTU_SALES_DETAILS();
                        salesDetails.bill_no = salesMaster.bill_no;
                        salesDetails.sl_no = sdvm.SlNo;
                        salesDetails.est_no = sdvm.EstimationNo;
                        salesDetails.barcode_no = sdvm.BarcodeNo;
                        salesDetails.sal_code = sdvm.SalCode;
                        salesDetails.item_name = sdvm.ItemName;
                        salesDetails.counter_code = sdvm.CounterCode;
                        salesDetails.item_no = sdvm.ItemNo;
                        salesDetails.gwt = sdvm.GrossWt;
                        salesDetails.swt = sdvm.StoneWt;
                        salesDetails.nwt = sdvm.NetWt;
                        salesDetails.wast_percent = sdvm.WastpPercent;
                        salesDetails.making_charge_per_rs = sdvm.MakingChargePerRS;
                        salesDetails.va_amount = sdvm.VaAmount;
                        salesDetails.stone_charges = sdvm.StoneCharges;
                        salesDetails.total_amount = sdvm.TotalAmount;
                        salesDetails.gold_value = sdvm.GoldValue;
                        salesDetails.diamond_charges = sdvm.DiamondCharges;
                        salesDetails.AddWt = sdvm.AddWt;
                        salesDetails.DeductWt = sdvm.DeductWt;
                        salesDetails.hallmarcharges = sdvm.HallMarCharges;
                        salesDetails.mc_amount = sdvm.McAmount;
                        salesDetails.wastage_grms = sdvm.WastageGrms;
                        salesDetails.mc_percent = sdvm.McPercent;
                        salesDetails.Addqty = sdvm.AddQty;
                        salesDetails.Deductqty = sdvm.DeductQty;
                        salesDetails.offer_value = sdvm.OfferValue;
                        salesDetails.gs_code = sdvm.GsCode;
                        salesDetails.rate = sdvm.Rate;
                        salesDetails.karat = sdvm.karat;
                        salesDetails.ad_barcode = sdvm.AdBarcode;
                        salesDetails.ad_counter = sdvm.AdCounter;
                        salesDetails.ad_item = sdvm.AdItem;
                        salesDetails.isEDApplicable = sdvm.isEDApplicable;
                        salesDetails.mc_type = sdvm.McType;
                        salesDetails.Fin_Year = sdvm.FinalYear;
                        salesDetails.New_Bill_No = sdvm.NewBillNo;
                        salesDetails.item_additional_discount = sdvm.ItemAdditionalDiscount;
                        salesDetails.item_total_after_discount = sdvm.ItemTotalAfterDiscount;
                        salesDetails.tax_percentage = sdvm.TaxPercentage;
                        salesDetails.tax_amount = sdvm.TaxAmount;
                        salesDetails.item_final_amount = sdvm.ItemFinalAmount;
                        salesDetails.supplier_code = sdvm.SupplierCode;
                        salesDetails.item_size = sdvm.ItemSize;
                        salesDetails.img_id = sdvm.ImgId;
                        salesDetails.design_code = sdvm.DesignCode;
                        salesDetails.design_name = sdvm.DesignName;
                        salesDetails.batch_id = sdvm.BatchId;
                        salesDetails.rf_id = sdvm.RfID;
                        salesDetails.mc_per_piece = sdvm.McPerPiece;
                        salesDetails.round_off = sdvm.RoundOff;
                        salesDetails.item_final_amount_after_roundoff = sdvm.ItemFinalAmountAfterRoundoff;
                        salesDetails.purity_per = sdvm.PurityPer;
                        salesDetails.melting_percent = sdvm.MeltingPercent;
                        salesDetails.melting_loss = sdvm.MeltingLoss;
                        salesDetails.item_type = sdvm.ItemType;
                        salesDetails.Discount_Mc = sdvm.DiscountMc;
                        salesDetails.Total_sales_mc = sdvm.TotalSalesMc;
                        salesDetails.Mc_Discount_Amt = sdvm.McDiscountAmt;
                        salesDetails.purchase_mc = sdvm.PurchaseMc;
                        salesDetails.GSTGroupCode = sdvm.GSTGroupCode;
                        salesDetails.SGST_Percent = sdvm.SGSTPercent;
                        salesDetails.SGST_Amount = sdvm.SGSTAmount;
                        salesDetails.CGST_Percent = sdvm.CGSTPercent;
                        salesDetails.CGST_Amount = sdvm.CGSTAmount;
                        salesDetails.IGST_Percent = sdvm.IGSTPercent;
                        salesDetails.IGST_Amount = sdvm.IGSTAmount;
                        salesDetails.HSN = sdvm.HSN;
                        salesDetails.Piece_Rate = sdvm.PieceRate;
                        salesDetails.UniqRowID = Guid.NewGuid();
                        salesDetails.DeductSWt = sdvm.DeductSWt;
                        salesDetails.Ord_Discount_Amt = sdvm.OrdDiscountAmt;
                        salesDetails.ded_counter = sdvm.DedCounter;
                        salesDetails.ded_item = sdvm.DedItem;
                        db.KTTU_SALES_DETAILS.Add(salesDetails);

                        // Incase weight added from other barcode, then we mark that barcode as sold.
                        if (salesDetails.ad_barcode != "") {
                            KTTU_BARCODE_MASTER kbm = db.KTTU_BARCODE_MASTER.Where(bm => bm.barcode_no == salesDetails.ad_barcode && bm.company_code == sales.CompanyCode && bm.branch_code == sales.BranchCode).FirstOrDefault();
                            kbm.sold_flag = "Y";
                            db.Entry(kbm).State = System.Data.Entity.EntityState.Modified;
                        }

                        #region Stone Details
                        foreach (SalesStoneVM kssd in sdvm.salesstoneVM) {
                            KTTU_SALES_STONE_DETAILS salesStoneDetails = new KTTU_SALES_STONE_DETAILS();
                            salesStoneDetails.bill_no = salesMaster.bill_no;
                            salesStoneDetails.sl_no = kssd.SlNo;
                            salesStoneDetails.est_no = kssd.EstNo;
                            salesStoneDetails.est_Srno = kssd.EstSrNo;
                            salesStoneDetails.barcode_no = kssd.BarcodeNo;
                            salesStoneDetails.type = kssd.Type;
                            salesStoneDetails.name = kssd.Name;
                            salesStoneDetails.qty = kssd.Qty;
                            salesStoneDetails.carrat = kssd.Carrat;
                            salesStoneDetails.stone_wt = kssd.StoneWt;
                            salesStoneDetails.rate = kssd.Rate;
                            salesStoneDetails.amount = kssd.Amount;
                            salesStoneDetails.tax = kssd.Tax;
                            salesStoneDetails.tax_amount = kssd.TaxAmount;
                            salesStoneDetails.total_amount = kssd.TotalAmount;
                            salesStoneDetails.bill_type = kssd.BillType;
                            salesStoneDetails.dealer_sales_no = kssd.DealerSalesNo;
                            salesStoneDetails.BILL_DET11PK = kssd.BillDet11PK;
                            salesStoneDetails.Fin_Year = kssd.FinYear;
                            salesStoneDetails.color = kssd.Color;
                            salesStoneDetails.clarity = kssd.Clarity;
                            salesStoneDetails.shape = kssd.Shape;
                            salesStoneDetails.cut = kssd.Cut;
                            salesStoneDetails.polish = kssd.Polish;
                            salesStoneDetails.symmetry = kssd.Symmetry;
                            salesStoneDetails.fluorescence = kssd.Fluorescence;
                            salesStoneDetails.certificate = kssd.Certificate;
                            salesStoneDetails.UniqRowID = Guid.NewGuid();
                            db.KTTU_SALES_STONE_DETAILS.Add(salesStoneDetails);
                        }
                        #endregion
                    }
                    #endregion
                }
                catch (Exception excp) {
                    error = new ErrorVM().GetErrorDetails(excp);
                    return null;
                }
            }
            return sales;
        }

        public SalesInvoiceAttributeVM CalculateReceivable(string companyCode, string branchCode, int estimateNo, decimal differenceDiscountAmt, string userID, out ErrorVM error)
        {
            error = null;
            var estimateMaster = db.KTTU_SALES_EST_MASTER.Where(e => e.company_code == companyCode
                   && e.branch_code == branchCode && e.est_no == estimateNo).FirstOrDefault();
            if (estimateMaster == null) {
                error = new ErrorVM
                {
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound,
                    description = "Estimation does not exist or is invalid."
                };
                return null;
            }

            #region Sales Calc Commented
            //List<KTTU_SALES_EST_DETAILS> ksed = db.KTTU_SALES_EST_DETAILS.Where(est => est.est_no == estimateNo
            //                                                                 && est.company_code == companyCode
            //                                                                 && est.branch_code == branchCode).ToList();

            //decimal totalItemAmt = ksed.Sum(x => x.total_amount);
            //decimal totalGstAmt = Convert.ToDecimal(ksed.Sum(x => x.CGST_Amount + x.SGST_Amount + x.IGST_Amount + x.CESSAmount));
            //decimal netGSTPercent = totalGstAmt / totalItemAmt;
            //decimal discountAmtInclGST = differenceDiscountAmt;
            //decimal discountAmtWithoutGST = Math.Round(discountAmtInclGST / (1 + netGSTPercent), 2);
            //foreach (KTTU_SALES_EST_DETAILS sd in ksed) {
            //    sd.LineItemContribution = Math.Round(sd.total_amount / totalItemAmt, 8);
            //    sd.NewItemDiscountAmt = Math.Round(sd.LineItemContribution * discountAmtWithoutGST, 2);
            //    sd.NewItemTotalAfterDiscExclGST = Math.Round(sd.total_amount - sd.NewItemDiscountAmt, 2);
            //    sd.NewCGST_Amount = Math.Round(Convert.ToDecimal(sd.NewItemTotalAfterDiscExclGST * sd.CGST_Percent / 100), 2);
            //    sd.NewSGST_Amount = Math.Round(Convert.ToDecimal(sd.NewItemTotalAfterDiscExclGST * sd.SGST_Percent / 100), 2);
            //    sd.NewIGST_Amount = Math.Round(Convert.ToDecimal(sd.NewItemTotalAfterDiscExclGST * sd.IGST_Percent / 100), 2);
            //    sd.NewCESSAmount = Math.Round(Convert.ToDecimal(sd.NewItemTotalAfterDiscExclGST * sd.CESSPercent / 100), 2);
            //    sd.NewItemTotalInclGST = sd.NewItemTotalAfterDiscExclGST + sd.NewCGST_Amount + sd.NewSGST_Amount +
            //        sd.NewIGST_Amount + sd.NewCESSAmount;
            //} 
            #endregion
            db.Configuration.UseDatabaseNullSemantics = true;
            var salesTotals = (from sed in db.KTTU_SALES_EST_DETAILS
                               where sed.company_code == companyCode
                                     && sed.branch_code == branchCode && sed.est_no == estimateNo
                               group sed by 1 into g
                               select new
                               {
                                   ItemTotal = g.Sum(x => x.total_amount),
                                   CGSTAmount = g.Sum(x => x.CGST_Amount),
                                   SGSTAmount = g.Sum(x => x.SGST_Amount),
                                   IGSTAmount = g.Sum(x => x.IGST_Amount),
                                   CessAmount = g.Sum(x => x.CESSAmount),
                                   GSTAmt = (g.Sum(x => x.CGST_Amount))
                                     + (g.Sum(x => x.SGST_Amount))
                                     + (g.Sum(x => x.IGST_Amount))
                                     + (g.Sum(x => x.CESSAmount)),
                                   ItemFinalAmt = g.Sum(x => x.item_final_amount),
                                   MCDiscountAmt = g.Sum(x => x.Mc_Discount_Amt),
                                   OrderDiscountAmt = g.Sum(x => x.Ord_Discount_Amt),
                                   OfferDiscountAmt = g.Sum(x => x.offer_value),
                                   AdditionalDiscountAmt = g.Sum(x => x.item_additional_discount),
                                   TotalDiscountAmt = (g.Sum(x => x.item_additional_discount))
                                     + (g.Sum(x => x.Mc_Discount_Amt))
                                     + (g.Sum(x => x.Ord_Discount_Amt))
                                     + (g.Sum(x => x.offer_value)),
                                   GSTAmtOriginal = (g.Sum(x => x.CGST_Percent * x.total_amount / 100))
                                     + (g.Sum(x => x.SGST_Percent * x.total_amount / 100))
                                     + (g.Sum(x => x.IGST_Percent * x.total_amount / 100))
                                     + (g.Sum(x => x.CESSPercent * x.total_amount / 100))
                               }).FirstOrDefault();
            if (salesTotals == null) {
                error = new ErrorVM
                {
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound,
                    description = "There is no estimation line detail."
                };
                return null;
            }
            #region Old Code for reference
            //decimal gstAmt = Convert.ToDecimal(salesTotals.CGSTAmount) + Convert.ToDecimal(salesTotals.SGSTAmount) +
            //    Convert.ToDecimal(salesTotals.IGSTAmount) + Convert.ToDecimal(salesTotals.CessAmount);
            //decimal totalGSTInclCessPercent = Math.Round(Convert.ToDecimal(gstAmt / salesTotals.ItemTotal), 6);
            //decimal gstCessPercent = Math.Round(Convert.ToDecimal(salesTotals.CessAmount / salesTotals.ItemTotal), 6);
            //decimal gstPercentExclCess = totalGSTInclCessPercent - gstCessPercent;

            //decimal itemTotal = Convert.ToDecimal(salesTotals.ItemTotal);

            //decimal discountAmount = Math.Round(differenceDiscountAmt / (1 + totalGSTInclCessPercent), 2);
            //decimal newItemTotal = itemTotal - discountAmount;
            //decimal gstAmountExlCess = Math.Round(newItemTotal * gstPercentExclCess, 2);
            //decimal gSTCessAmt = Math.Round(newItemTotal * gstCessPercent, 2); 
            #endregion

            //decimal totalGSTInclCessPercent = Math.Round(Convert.ToDecimal(salesTotals.GSTAmtOriginal / salesTotals.ItemTotal), 6);
            //decimal gstCessPercent = Math.Round(Convert.ToDecimal(salesTotals.CessAmount / salesTotals.ItemTotal), 6);
            //decimal gstPercentExclCess = totalGSTInclCessPercent - gstCessPercent;

            decimal itemTotal = Convert.ToDecimal(salesTotals.ItemTotal);
            decimal gstAmountBeforeDiscounts = Math.Round(Convert.ToDecimal(salesTotals.GSTAmtOriginal), 2);
            decimal gstPercent = gstAmountBeforeDiscounts / itemTotal;

            decimal discountAmount = Math.Round(differenceDiscountAmt / (1 + gstPercent), 2);
            decimal itemTotalAfterAdditionalDiscount = itemTotal - discountAmount;
            decimal itemTotalAfterOfferDiscount = itemTotalAfterAdditionalDiscount - Convert.ToDecimal(salesTotals.OfferDiscountAmt);
            decimal newGSTAmount = gstPercent * itemTotalAfterOfferDiscount;

            decimal gstAmountExlCess = 0;
            decimal gSTCessAmt = 0;
            decimal discountPercent = Math.Round(discountAmount / itemTotal * 100.00M, 6);
            decimal offerDiscountPercent = Math.Round(Convert.ToDecimal(salesTotals.OfferDiscountAmt) / itemTotal * 100.00M, 6);
            decimal totalDiscountPercent = discountPercent + offerDiscountPercent;

            var operatorAllowedDiscPercent = db.SDTU_OPERATOR_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                && x.OperatorCode == userID && x.object_status != "C").Select(y => y.discount_percent).FirstOrDefault();
            if (discountPercent > Convert.ToDecimal(operatorAllowedDiscPercent)) {
                error = new ErrorVM
                {
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                    description = "Operator discount limit exceeds the discount given. Please contact your administrator."
                };
                return null;
            }

            //if(Math.Round(itemTotalAfterOfferDiscount + newGSTAmount, 2) - Math.Round(itemTotalAfterOfferDiscount + newGSTAmount, 0) <= 0.02M || ) {
            //    Math.Round(itemTotalAfterOfferDiscount + newGSTAmount, 2) - Math.Round(itemTotalAfterOfferDiscount + newGSTAmount, 0) <= 0.02M
            //}
            SalesInvoiceAttributeVM salesInvoiceAttributes = new SalesInvoiceAttributeVM
            {
                SalesAmountExclTax = itemTotalAfterOfferDiscount,
                DiscountAmount = discountAmount,
                GSTAmount = Math.Round(newGSTAmount, 2),
                GSTCessAmount = gSTCessAmt,
                GSTAmountInclCess = gstAmountExlCess + gSTCessAmt,
                SalesAmountInclTax = Math.Round(itemTotalAfterOfferDiscount + newGSTAmount, 0),
                DiscountPercent = totalDiscountPercent,
                OfferDiscount = Convert.ToDecimal(salesTotals.OfferDiscountAmt),
                MCDiscount = Convert.ToDecimal(salesTotals.MCDiscountAmt),
                AdditionalDiscount = discountPercent,
                SalesAmountInclTaxWithoutRoundOff = Math.Round(itemTotalAfterOfferDiscount + newGSTAmount, 2)
            };
            return salesInvoiceAttributes;
        }

        public bool CancelSalesInvoice(SalesMasterVM sales, out ErrorVM error)
        {
            error = null;
            try {
                // Sales Billing
                KTTU_SALES_MASTER ksm = db.KTTU_SALES_MASTER.Where(sm => sm.bill_no == sales.BillNo
                                                                            && sm.company_code == sales.CompanyCode
                                                                            && sm.branch_code == sales.BranchCode).FirstOrDefault();
                if (ksm != null && ksm.cflag == "Y") {
                    error = new ErrorVM()
                    {
                        description = "Sales invoice already cancelled.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }

                ksm.cflag = "Y";
                ksm.cancelled_by = sales.OperatorCode;
                ksm.cancelled_remarks = sales.CancelledRemarks;
                ksm.UpdateOn = SIGlobals.Globals.GetDateTime();
                db.Entry(ksm).State = System.Data.Entity.EntityState.Modified;

                //Payment Details
                List<KTTU_PAYMENT_DETAILS> payments = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == sales.BillNo
                                                                                 && pay.company_code == sales.CompanyCode
                                                                                 && pay.branch_code == sales.BranchCode
                                                                                 && (pay.trans_type == "PC" || pay.trans_type == "S")).ToList();
                foreach (KTTU_PAYMENT_DETAILS pay in payments) {
                    pay.cflag = "Y";
                    pay.cancelled_by = sales.OperatorCode;
                    pay.cancelled_remarks = sales.CancelledRemarks;
                    pay.cancelled_date = SIGlobals.Globals.GetApplicationDate(sales.CompanyCode, sales.BranchCode).ToString();
                    pay.UpdateOn = SIGlobals.Globals.GetDateTime();
                    db.Entry(pay).State = System.Data.Entity.EntityState.Modified;

                    //Purchase Master
                    if (pay.pay_mode == "PB") {
                        int purchaseBillNo = Convert.ToInt32(pay.Ref_BillNo);
                        KTTU_PURCHASE_MASTER purchase = db.KTTU_PURCHASE_MASTER.Where(pur => pur.bill_no == purchaseBillNo
                                                                                       && pur.company_code == sales.CompanyCode
                                                                                       && pur.branch_code == sales.BranchCode).FirstOrDefault();
                        purchase.cflag = "Y";
                        purchase.cancelled_by = sales.OperatorCode;
                        purchase.cancelled_remarks = sales.CancelledRemarks;
                        purchase.UpdateOn = SIGlobals.Globals.GetDateTime();
                        db.Entry(purchase).State = System.Data.Entity.EntityState.Modified;

                        //Note: Attached Order and Sales Return we are not going to set any flags as per magna windows application.
                    }

                    // Sales Estimation
                    KTTU_SALES_EST_MASTER ksem = db.KTTU_SALES_EST_MASTER.Where(sem => sem.bill_no == sales.BillNo
                                                                                && sem.company_code == sales.CompanyCode
                                                                                && sem.branch_code == sales.BranchCode).FirstOrDefault();
                    ksem.bill_no = 0;
                    ksem.UpdateOn = SIGlobals.Globals.GetDateTime();
                    db.Entry(ksem).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public dynamic GetAllSalesBillNO(string companyCode, string branchCode, DateTime date, bool isCanelled, out ErrorVM error)
        {
            error = null;
            string cFlag = isCanelled == true ? "Y" : "N";
            var data = db.KTTU_SALES_MASTER.Where(sale => sale.company_code == companyCode
                                                            && sale.branch_code == branchCode
                                                            && sale.cflag == cFlag
                                                            && System.Data.Entity.DbFunctions.TruncateTime(sale.bill_date) == System.Data.Entity.DbFunctions.TruncateTime(date))
                                                            .OrderByDescending(sale => sale.bill_no).Select(s => new SalesEstMasterVM() { BillNo = s.bill_no, CustName = s.cust_name });
            if(data == null) {
                error = new ErrorVM { description = $"No data found.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return null;
            }
            return data;
        }

        public dynamic GetBillInformation(string companyCode, string branchCode, int billNo, out ErrorVM error)
        {
            error = null;
            try {
                var billInfo = db.KTTU_SALES_MASTER.Where(sale => sale.company_code == companyCode
                                                                    && sale.branch_code == branchCode
                                                                    && sale.bill_no == billNo).FirstOrDefault();
                if (billInfo == null) {
                    error = new ErrorVM { description = $"Bill detail for bill No. {billNo} is not found.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return null;
                }
                if (billInfo.cflag == "Y") {
                    error = new ErrorVM { description = $"The bill {billNo} is already cancelled.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return null;
                }

                return new SalesMasterVM
                {
                    BillNo = billInfo.bill_no,
                    BillDate = billInfo.bill_date,
                    CustomerID = billInfo.cust_Id,
                    CustomerName = billInfo.cust_name,
                    Address1 = billInfo.address1,
                    Address2 = billInfo.address2,
                    Address3 = billInfo.address3,
                    MobileNo = billInfo.mobile_no,
                    TotalSaleAmount = billInfo.total_tax_amount + billInfo.total_sale_amount
                };
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return null;
            }
        }

        public dynamic GetBillType(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                var data = db.vBilltypes.ToList();
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public dynamic GetAttachementInformationOfBill(string companyCode, string branchCode, int billNo, out ErrorVM error)
        {
            error = null;
            string[] types = { "PB", "OP", "SR" };
            SalesBillInfo billInfo = new SalesBillInfo();
            billInfo.Companycode = companyCode;
            billInfo.BranchCode = branchCode;
            billInfo.BillNo = billNo;
            billInfo.Purchase = new List<int>();
            billInfo.Orders = new List<int>();
            billInfo.SalesReturn = new List<int>();
            try {
                var data = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == billNo
                                            && pay.company_code == companyCode
                                            && pay.branch_code == branchCode
                                            && pay.trans_type == "S"
                                            && pay.cflag != "Y"
                                            && types.Contains(pay.pay_mode)).ToList();
                if (data.Count > 0) {
                    foreach (var d in data) {
                        switch (d.pay_mode) {
                            case "PB":
                                int pb = Convert.ToInt32(d.Ref_BillNo);
                                billInfo.Purchase.Add(pb);
                                break;
                            case "OP":
                                int op = Convert.ToInt32(d.Ref_BillNo);
                                billInfo.Orders.Add(op);
                                break;
                            case "SR":
                                int sr = Convert.ToInt32(d.Ref_BillNo);
                                billInfo.SalesReturn.Add(sr);
                                break;
                        }
                    }
                }
                string refBillNo = Convert.ToString(billNo);
                var creditNote = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.company_code == companyCode
                                                               && pay.branch_code == branchCode
                                                               && pay.trans_type == "O"
                                                               && pay.pay_mode == "CN"
                                                               && pay.Ref_BillNo == refBillNo).FirstOrDefault();
                if (creditNote != null) {
                    CreditNoteInfo cn = new CreditNoteInfo();
                    cn.OrderNo = creditNote.series_no;
                    cn.ReceiptNo = creditNote.receipt_no;
                    billInfo.CreditNote = cn;
                }
                return billInfo;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        #endregion

        #region Public Methods
        public bool SalesStockPost(MagnaDbEntities dbContext, string companyCode, string branchCode, int salesBillNo, bool postReverse, out string errorMessage)
        {
            if (dbContext == null)
                dbContext = new MagnaDbEntities(true);
            var salesLines = dbContext.KTTU_SALES_DETAILS.Where(sed => sed.company_code == companyCode
                           && sed.branch_code == branchCode && sed.bill_no == salesBillNo);

            #region Stock Validation (check if there is necessary inventory)
            var generalStockJournal =
                   from sl in salesLines
                   where sl.rate > 0 && sl.gwt > 0
                   group sl by new
                   {
                       CompanyCode = sl.company_code,
                       BranchCode = sl.branch_code,
                       EstimateNo = sl.est_no,
                       GS = sl.gs_code,
                       Counter = sl.counter_code,
                       Item = sl.item_name
                   } into g
                   select new StockJournalVM
                   {
                       StockTransType = StockTransactionType.Sales,
                       CompanyCode = g.Key.CompanyCode,
                       BranchCode = g.Key.BranchCode,
                       DocumentNo = g.Key.EstimateNo,
                       GS = g.Key.GS,
                       Counter = g.Key.Counter,
                       Item = g.Key.Item,
                       Qty = g.Sum(x => x.item_no) + (g.Sum(y => y.Addqty) == null ? 0 : (int)g.Sum(y => y.Addqty)) - (g.Sum(y => y.Deductqty) == null ? 0 : (int)g.Sum(y => y.Deductqty)),
                       GrossWt = g.Sum(x => x.gwt) == null ? 0 : (decimal)g.Sum(x => x.gwt) + (g.Sum(y => y.AddWt) == null ? 0 : (decimal)g.Sum(y => y.AddWt)) - (decimal)(g.Sum(y => y.DeductWt) == null ? 0 : (decimal)g.Sum(y => y.DeductWt)),
                       StoneWt = g.Sum(x => x.swt),
                       NetWt = g.Sum(x => x.nwt) + (g.Sum(y => y.AddWt) == null ? 0 : (decimal)g.Sum(y => y.AddWt)) - (decimal)(g.Sum(y => y.DeductWt) == null ? 0 : (decimal)g.Sum(y => y.DeductWt)),
                   };

            var pieceRateStockJournal =
                from sl in salesLines
                where sl.rate == 0 && sl.item_no > 0 //The only change for piece rate stock check
                group sl by new
                {
                    CompanyCode = sl.company_code,
                    BranchCode = sl.branch_code,
                    EstimateNo = sl.est_no,
                    GS = sl.gs_code,
                    Counter = sl.counter_code,
                    Item = sl.item_name
                } into g
                select new StockJournalVM
                {
                    StockTransType = StockTransactionType.Sales,
                    CompanyCode = g.Key.CompanyCode,
                    BranchCode = g.Key.BranchCode,
                    DocumentNo = g.Key.EstimateNo,
                    GS = g.Key.GS,
                    Counter = g.Key.Counter,
                    Item = g.Key.Item,
                    Qty = g.Sum(x => x.item_no) + (g.Sum(y => y.Addqty) == null ? 0 : (int)g.Sum(y => y.Addqty)) - (g.Sum(y => y.Deductqty) == null ? 0 : (int)g.Sum(y => y.Deductqty)),
                    GrossWt = g.Sum(x => x.gwt) == null ? 0 : (decimal)g.Sum(x => x.gwt) + (g.Sum(y => y.AddWt) == null ? 0 : (decimal)g.Sum(y => y.AddWt)) - (decimal)(g.Sum(y => y.DeductWt) == null ? 0 : (decimal)g.Sum(y => y.DeductWt)),
                    StoneWt = g.Sum(x => x.swt),
                    NetWt = g.Sum(x => x.nwt) + (g.Sum(y => y.AddWt) == null ? 0 : (decimal)g.Sum(y => y.AddWt)) - (g.Sum(y => y.DeductWt) == null ? 0 : (decimal)g.Sum(y => y.DeductWt)),
                };

            //General stock validation
            var generalCounterStock =
                (from cs in dbContext.KTTU_COUNTER_STOCK
                 join sl in generalStockJournal
                 on new { CompanyCode = cs.company_code, BranchCode = cs.branch_code, GS = cs.gs_code, Counter = cs.counter_code, Item = cs.item_name }
                 equals new { CompanyCode = sl.CompanyCode, BranchCode = sl.BranchCode, GS = sl.GS, Counter = sl.Counter, Item = sl.Item }
                 where cs.closing_gwt - sl.GrossWt < 0
                 select new
                 {
                     GS = cs.gs_code,
                     Counter = cs.counter_code,
                     Item = cs.item_name,
                     StockOutWarning = "There is no stock for the item (" + cs.item_name + ") @ counter " + cs.counter_code + ".",
                     ClosingQty = cs.closing_units - sl.Qty,
                     ClosingGrossWt = cs.closing_gwt - sl.GrossWt,
                     ClosingStoneWt = cs.closing_swt - sl.StoneWt,
                     ClosingNewWt = cs.closing_nwt - sl.NetWt
                 }).ToArray();

            bool stockValidationSucceeded = true;
            errorMessage = string.Empty;
            foreach (var entry in generalCounterStock) {
                errorMessage += entry.StockOutWarning + Environment.NewLine;
                stockValidationSucceeded = false;
            }

            if (!stockValidationSucceeded)
                return false;

            //piece rate stock validation
            var pieceRateCounterStock =
                (from cs in dbContext.KTTU_COUNTER_STOCK
                 join sl in pieceRateStockJournal
                 on new { CompanyCode = cs.company_code, BranchCode = cs.branch_code, GS = cs.gs_code, Counter = cs.counter_code, Item = cs.item_name }
                 equals new { CompanyCode = sl.CompanyCode, BranchCode = sl.BranchCode, GS = sl.GS, Counter = sl.Counter, Item = sl.Item }
                 where cs.closing_units - sl.Qty < 0 //difference condition for validating piece stock
                 select new
                 {
                     GS = cs.gs_code,
                     Counter = cs.counter_code,
                     Item = cs.item_name,
                     StockOutWarning = "There is no stock for the item (" + cs.item_name + ") @ counter " + cs.counter_code + ".",
                     ClosingQty = cs.closing_units - sl.Qty,
                     ClosingGrossWt = cs.closing_gwt - sl.GrossWt,
                     ClosingStoneWt = cs.closing_swt - sl.StoneWt,
                     ClosingNewWt = cs.closing_nwt - sl.NetWt
                 }).ToArray();

            stockValidationSucceeded = true;
            errorMessage = string.Empty;
            foreach (var entry in pieceRateCounterStock) {
                errorMessage += entry.StockOutWarning + Environment.NewLine;
                stockValidationSucceeded = false;
            }

            if (!stockValidationSucceeded)
                return false;
            #endregion

            #region Post Counter Stock
            StockPostBL stockPost = new StockPostBL();
            //Let's create a consolidated stock journal
            var consolidatedStockJournal = generalStockJournal.ToList();
            foreach (StockJournalVM sj in pieceRateStockJournal)
                consolidatedStockJournal.Add(sj);

            //Post counter stock
            bool counterStockPostSuccess = stockPost.CounterStockPost(dbContext, consolidatedStockJournal, postReverse, out errorMessage);
            if (!counterStockPostSuccess)
                return false;
            #endregion

            #region Post GS Stock
            //Update GS stock
            var summarizedGSStockJournal =
                from cj in consolidatedStockJournal
                group cj by new { Company = cj.CompanyCode, Branch = cj.BranchCode, GS = cj.GS, EstimateNo = cj.DocumentNo } into g
                select new StockJournalVM
                {
                    StockTransType = StockTransactionType.Sales,
                    CompanyCode = g.Key.Company,
                    BranchCode = g.Key.Branch,
                    DocumentNo = g.Key.EstimateNo,
                    GS = g.Key.GS,
                    Counter = "",
                    Item = "",
                    Qty = g.Sum(x => x.Qty),
                    GrossWt = g.Sum(x => x.GrossWt),
                    StoneWt = g.Sum(x => x.StoneWt),
                    NetWt = g.Sum(x => x.NetWt)
                };
            bool gsStockPostSuccess = stockPost.GSStockPost(dbContext, summarizedGSStockJournal.ToList(), postReverse, out errorMessage);
            if (!gsStockPostSuccess)
                return false;
            #endregion

            #region Post Add-Weight/Deduct-Wt 

            #region Add Weight posting
            //Logic: When add weight is there then deduct the add weight item from counter stock and 
            //add base item to counter stock.
            var addStockJournal =
                from sl in salesLines
                where sl.AddWt > 0 || sl.Addqty > 0
                group sl by new
                {
                    CompanyCode = sl.company_code,
                    BranchCode = sl.branch_code,
                    EstimateNo = sl.est_no,
                    GS = sl.gs_code,
                    Counter = sl.ad_counter,
                    Item = sl.ad_item
                } into g
                select new StockJournalVM
                {
                    StockTransType = StockTransactionType.Issue,
                    CompanyCode = g.Key.CompanyCode,
                    BranchCode = g.Key.BranchCode,
                    DocumentNo = g.Key.EstimateNo,
                    GS = g.Key.GS,
                    Counter = g.Key.Counter,
                    Item = g.Key.Item,
                    Qty = g.Sum(y => y.Addqty) == null ? 0 : (int)g.Sum(y => y.Addqty),
                    GrossWt = g.Sum(y => y.AddWt) == null ? 0 : (decimal)g.Sum(y => y.AddWt),
                    StoneWt = 0,
                    NetWt = g.Sum(y => y.AddWt) == null ? 0 : (decimal)g.Sum(y => y.AddWt),
                };
            //Post counter stock
            counterStockPostSuccess = stockPost.CounterStockPost(dbContext, addStockJournal.ToList(), postReverse, out errorMessage);
            if (!counterStockPostSuccess)
                return false;

            var addStockReceiptJournal =
                from sl in salesLines
                where sl.AddWt > 0 || sl.Addqty > 0
                group sl by new
                {
                    CompanyCode = sl.company_code,
                    BranchCode = sl.branch_code,
                    EstimateNo = sl.est_no,
                    GS = sl.gs_code,
                    Counter = sl.counter_code,
                    Item = sl.item_name
                } into g
                select new StockJournalVM
                {
                    StockTransType = StockTransactionType.Receipt,
                    CompanyCode = g.Key.CompanyCode,
                    BranchCode = g.Key.BranchCode,
                    DocumentNo = g.Key.EstimateNo,
                    GS = g.Key.GS,
                    Counter = g.Key.Counter,
                    Item = g.Key.Item,
                    Qty = g.Sum(y => y.Addqty) == null ? 0 : (int)g.Sum(y => y.Addqty),
                    GrossWt = g.Sum(y => y.AddWt) == null ? 0 : (decimal)g.Sum(y => y.AddWt),
                    StoneWt = 0,
                    NetWt = g.Sum(y => y.AddWt) == null ? 0 : (decimal)g.Sum(y => y.AddWt),
                };

            counterStockPostSuccess = stockPost.CounterStockPost(dbContext, addStockReceiptJournal.ToList(), postReverse, out errorMessage);
            if (!counterStockPostSuccess)
                return false;
            #endregion

            #region Deduct Wt Posting
            /* //Nothing to be done in this section since the weight deducted is getting deducted automatically from the item itself.
            var deductStockJournal =
                from sl in salesLines
                where sl.DeductWt > 0 || sl.Deductqty > 0
                group sl by new
                {
                    CompanyCode = sl.company_code,
                    BranchCode = sl.branch_code,
                    EstimateNo = sl.est_no,
                    GS = sl.gs_code,
                    //Counter = sl.ded_counter == null || sl.ded_counter == "" ? "M"   : sl.ded_counter,
                    //Item = sl.ded_item == null || sl.ded_item == "" ? "SUN" : sl.ded_item,
                    Counter = sl.ded_counter,
                    Item = sl.ded_item
                } into g
                select new StockJournalVM
                {
                    StockTransType = StockTransactionType.Receipt,
                    CompanyCode = g.Key.CompanyCode,
                    BranchCode = g.Key.BranchCode,
                    DocumentNo = g.Key.EstimateNo,
                    GS = g.Key.GS,
                    Counter = g.Key.Counter,
                    Item = g.Key.Item,
                    Qty = g.Sum(y => y.Deductqty) == null ? 0 : (int)g.Sum(y => y.Deductqty),
                    GrossWt = g.Sum(y => y.DeductWt) == null ? 0 : (decimal)g.Sum(y => y.DeductWt),
                    StoneWt = g.Sum(y => y.DeductSWt) == null ? 0 : (decimal)g.Sum(y => y.DeductSWt),
                    NetWt = g.Sum(y => y.DeductWt) == null ? 0 : (decimal)g.Sum(y => y.DeductWt),
                };

            //counterStockPostSuccess = stopPosting.CounterStockPost(db, deductStockJournal.ToList(), out errorMessage);
            //if (!counterStockPostSuccess)
            //    return false;


            var deductStockIssueJournal =
                from sl in salesLines
                where sl.DeductWt > 0 || sl.Deductqty > 0
                group sl by new
                {
                    CompanyCode = sl.company_code,
                    BranchCode = sl.branch_code,
                    EstimateNo = sl.est_no,
                    GS = sl.gs_code,
                    Counter = sl.counter_code,
                    Item = sl.item_name
                } into g
                select new StockJournalVM
                {
                    StockTransType = StockTransactionType.Issue,
                    CompanyCode = g.Key.CompanyCode,
                    BranchCode = g.Key.BranchCode,
                    DocumentNo = g.Key.EstimateNo,
                    GS = g.Key.GS,
                    Counter = g.Key.Counter,
                    Item = g.Key.Item,
                    Qty = g.Sum(y => y.Deductqty) == null ? 0 : (int)g.Sum(y => y.Deductqty),
                    GrossWt = g.Sum(y => y.DeductWt) == null ? 0 : (decimal)g.Sum(y => y.DeductWt),
                    StoneWt = g.Sum(y => y.DeductSWt) == null ? 0 : (decimal)g.Sum(y => y.DeductSWt),
                    NetWt = g.Sum(y => y.DeductWt) == null ? 0 : (decimal)g.Sum(y => y.DeductWt),
                };

            //counterStockPostSuccess = stopPosting.CounterStockPost(db, deductStockIssueJournal.ToList(), out errorMessage);
            //if (!counterStockPostSuccess)
            //    return false;

            //Post counter stock
            string deductItemPostingConfig = "Update Sundries"; //possible value: "Do Nothing", "Update Base Item"
            switch (deductItemPostingConfig) {
                case "Update Sundries":
                    var modifiedDedStockJournal = deductStockJournal.ToList();
                    var deductStkJrnl = modifiedDedStockJournal.Select(dsj => { dsj.Counter = "M"; dsj.Item = "SUN"; return dsj; });
                    counterStockPostSuccess = stockPost.CounterStockPost(dbContext, deductStkJrnl.ToList(), postReverse, out errorMessage);
                    if (!counterStockPostSuccess)
                        return false;
                    counterStockPostSuccess = stockPost.CounterStockPost(dbContext, deductStockIssueJournal.ToList(), postReverse, out errorMessage);
                    if (!counterStockPostSuccess)
                        return false;
                    break;
                case "Update Base Item":
                    counterStockPostSuccess = stockPost.CounterStockPost(dbContext, deductStockJournal.ToList(), postReverse, out errorMessage);
                    if (!counterStockPostSuccess)
                        return false;
                    counterStockPostSuccess = stockPost.CounterStockPost(dbContext, deductStockIssueJournal.ToList(), postReverse, out errorMessage);
                    if (!counterStockPostSuccess)
                        return false;
                    break;
                case "Do Nothing":
                    break;
            }
            */
            #endregion

            #endregion

            return true;
        }

        public bool CancelSalesInvoice(string companyCode, string branchCode, int billNo, string remarks, string operatorCode, out ErrorVM error)
        {
            error = null;

            #region Validate the sales invoice and check if it is valid, else throw error
            KTTU_SALES_MASTER sales = db.KTTU_SALES_MASTER.Where(bill => bill.company_code == companyCode && bill.branch_code == branchCode && bill.bill_no == billNo).FirstOrDefault();

            if (sales == null) {
                error = new ErrorVM { description = "Invalid Bill Details.", ErrorStatusCode = System.Net.HttpStatusCode.NotFound };
                return false;
            }
            if (sales.cflag == "Y") {
                error = new ErrorVM { description = "Sales Bill already Cancelled.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return false;
            }
            #endregion
            db.Database.CommandTimeout = 5;
            var transaction = db.Database.BeginTransaction();
            try {

                #region Stock post - Reverse the stock
                string errorMessage = string.Empty;
                if (!SalesStockPost(db, companyCode, branchCode, billNo, true, out errorMessage)) {
                    error = new ErrorVM { description = "Stock posting error found: " + errorMessage, ErrorStatusCode = System.Net.HttpStatusCode.NotFound, customDescription = "Stock posting error found: " + errorMessage };
                    transaction.Rollback();
                    return false;
                }
                #endregion

                #region Cancel Payments
                if (!CancelSalesPayments(db, companyCode, branchCode, billNo, remarks, operatorCode, out errorMessage)) {
                    error = new ErrorVM { description = "Failed in payment posting. Error: " + errorMessage, ErrorStatusCode = System.Net.HttpStatusCode.NotFound, customDescription = "Failed in payment posting. Error:" + errorMessage };
                    transaction.Rollback();
                    return false;
                }
                #endregion

                #region Cancel the Order which is created from sales invoice
                var ordersCreated = db.KTTU_PAYMENT_DETAILS.Where(x => x.company_code == companyCode
                    && x.branch_code == branchCode && x.Ref_BillNo == billNo.ToString() && x.trans_type == "O"
                    && x.pay_mode == "CN");
                foreach (var oc in ordersCreated) {
                    OrderMasterVM orderTobeCancelled = new OrderMasterVM
                    {
                        CompanyCode = oc.company_code,
                        BranchCode = oc.branch_code,
                        OrderNo = oc.series_no
                    };
                    bool isOrderCancelSuccess = new OrderBL(db).CancelOrder(orderTobeCancelled, out error);
                    if (!isOrderCancelSuccess) {
                        transaction.Rollback();
                        error.description = "Failed to cancel the attached order. Error :" + error.description;
                        return false;
                    }
                }
                #endregion

                #region Mark the bill as cancelled & delete sales-stone details. Update estimate as well

                #region Sales Bill Master Update
                var salesMaster = db.KTTU_SALES_MASTER.Where(sm => sm.company_code == companyCode &&
                        sm.branch_code == branchCode && sm.bill_no == billNo).FirstOrDefault();
                if (salesMaster == null) {
                    error = new ErrorVM { description = "Sales details not found.", ErrorStatusCode = System.Net.HttpStatusCode.NotFound, customDescription = "Invoice detail is not found." };
                    transaction.Rollback();
                    return false;
                }
                salesMaster.cflag = "Y";
                salesMaster.cancelled_remarks = remarks;
                salesMaster.cancelled_by = operatorCode;
                salesMaster.discount_autharity = "";
                salesMaster.UpdateOn = SIGlobals.Globals.GetDateTime();
                db.Entry(salesMaster).State = System.Data.Entity.EntityState.Modified;
                #endregion

                #region Sales Estimation Master & Estimation Payment Update
                var salesEstMaster = db.KTTU_SALES_EST_MASTER.Where(sm => sm.company_code == companyCode &&
                       sm.branch_code == branchCode && sm.bill_no == billNo).FirstOrDefault();
                if (salesEstMaster == null) {
                    error = new ErrorVM { description = "Sales estimation details not found.", ErrorStatusCode = System.Net.HttpStatusCode.NotFound, customDescription = "Sales estimation is not found." };
                    transaction.Rollback();
                    return false;
                }
                salesEstMaster.bill_no = 0;
                salesEstMaster.UpdateOn = SIGlobals.Globals.GetDateTime();
                salesEstMaster.spurchase_amount = 0;
                db.Entry(salesEstMaster).State = System.Data.Entity.EntityState.Modified;

                var tagInfo = db.KTTU_SALES_DETAILS.Where(sd => sd.company_code == companyCode && sd.branch_code == branchCode
                && sd.bill_no == billNo && sd.barcode_no != "").Select(t => new { TagNo = t.barcode_no, Gwt = t.gwt, Swt = t.swt, Nwt = t.nwt });

                foreach (var tagNo in tagInfo) {
                    var barcodeMaster = db.KTTU_BARCODE_MASTER.Where(bm => bm.company_code == companyCode && bm.branch_code == branchCode
                    && bm.barcode_no == tagNo.TagNo).FirstOrDefault();

                    if (barcodeMaster != null && barcodeMaster.barcode_no != "" && barcodeMaster.Tagging_Type != "G") {
                        barcodeMaster.sold_flag = "N";
                        barcodeMaster.ExitDocNo = string.Empty;
                        barcodeMaster.ExitDocType = string.Empty;
                        db.Entry(barcodeMaster).State = System.Data.Entity.EntityState.Modified;
                    }

                    if (barcodeMaster != null && barcodeMaster.barcode_no != "" && barcodeMaster.Tagging_Type == "G") {
                        barcodeMaster.sold_flag = "N";
                        barcodeMaster.gwt = barcodeMaster.gwt + tagNo.Gwt;
                        barcodeMaster.swt = barcodeMaster.swt + tagNo.Swt;
                        barcodeMaster.nwt = barcodeMaster.nwt + tagNo.Nwt;
                        barcodeMaster.UpdateOn = SIGlobals.Globals.GetDateTime();
                        db.Entry(barcodeMaster).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                var salesPayments = db.KTTU_PAYMENT_DETAILS.Where(sm => sm.company_code == companyCode &&
                       sm.branch_code == branchCode && sm.series_no == salesEstMaster.est_no &&
                       sm.trans_type == "A" && sm.pay_mode == "SE").ToList();
                foreach (KTTU_PAYMENT_DETAILS sp in salesPayments) {
                    sp.cflag = "Y";
                    sp.UpdateOn = SIGlobals.Globals.GetDateTime();
                    db.Entry(sp).State = System.Data.Entity.EntityState.Modified;
                }
                #endregion

                #region Sales Stone Details Update
                var salesStoneDetail = db.KTTU_SALES_STONE_DETAILS.Where(st => st.company_code == companyCode &&
                        st.branch_code == branchCode && st.bill_no == billNo).ToList();
                foreach (KTTU_SALES_STONE_DETAILS st in salesStoneDetail) {
                    st.bill_no = 0;
                    st.UpdateOn = SIGlobals.Globals.GetDateTime();
                    db.Entry(st).State = System.Data.Entity.EntityState.Modified;
                }
                #endregion

                #endregion

                #region Accounts Update - Cancel Posting
                string stringBillNo = billNo.ToString();
                var subsidaryVouchers = db.KSTU_ACC_SUBSIDIARY_VOUCHER_TRANSACTIONS.Where(
                    vc => vc.company_code == companyCode && vc.branch_code == branchCode &&
                    vc.bill_no == stringBillNo && vc.trans_type == "BILL").ToList();
                foreach (var vt in subsidaryVouchers) {
                    vt.cflag = "Y";
                    vt.UpdateOn = SIGlobals.Globals.GetDateTime();
                    vt.Cancelled_remarks = remarks;
                    vt.Cancelled_by = operatorCode;
                    db.Entry(vt).State = System.Data.Entity.EntityState.Modified;
                }
                #endregion

                #region Updating Marketplace
                // We are not loggin any error here incase if updated is false, that error is logged already before response come to this layer.
                bool updated = new Common.MarketplaceBL().UpdateMarketplaceInventory(companyCode,
                                    branchCode, billNo, SIGlobals.TransactionsType.Sales, SIGlobals.ActionType.UnBlock, out error);
                error = null;
                #endregion

                db.SaveChanges();
                transaction.Commit();
            }
            catch (Exception excp) {
                transaction.Rollback();
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }

            return true;
        }

        public ErrorVM AccountPostingWithProedure(MagnaDbEntities dbContext, string companyCode, string branchCode, int billNo)
        {
            try {
                string errorFromProc = string.Empty;
                ObjectParameter errorMessage = new ObjectParameter("errorMsg", typeof(string));
                ObjectParameter outputVal = new ObjectParameter("outValue", typeof(int));
                var result = dbContext.usp_createSubsidiarySalesVouchers_FuncImport(companyCode, branchCode, Convert.ToString(billNo), outputVal, errorMessage);
                return new CommonBL().HandleAccountPostingProcs(outputVal, errorMessage);
            }
            catch (Exception excp) {
                return new ErrorVM() { description = excp.InnerException.Message, field = "Account Update", index = 0 };
            }
        }

        public bool ValidateCustomer(int custID, string mobileNo, string companyCode, string branchCode, out ErrorVM error, MagnaDbEntities db)
        {
            error = null;
            if (SIGlobals.Globals.GetConfigurationValue(db, "28022019", companyCode, branchCode) == 1) {

                Customer misMaster = db.Customers.Where(cust => cust.MobileNo == mobileNo).FirstOrDefault();
                if (misMaster != null && misMaster.MobileNo == mobileNo) {
                    var customer = LoadCustomerDetails(misMaster.MobileNo, companyCode, branchCode);
                }
            }
            else if (custID == 0) {
                KSTU_CUSTOMER_MASTER customerMaster = db.KSTU_CUSTOMER_MASTER.Where(cust => cust.cust_id == custID
                                                                           && cust.company_code == companyCode
                                                                           && cust.branch_code == branchCode).FirstOrDefault();
                if (customerMaster != null && customerMaster.RepoCustId == 0 && !string.IsNullOrEmpty(customerMaster.mobile_no)) {
                    Customer misCustomer = db.Customers.Where(cust => cust.MobileNo == customerMaster.mobile_no && cust.CompanyCode == companyCode && cust.BranchCode == branchCode).FirstOrDefault();
                    if (customerMaster.mobile_no == misCustomer.MobileNo) {
                        var customer = LoadCustomerDetails(customerMaster.mobile_no, companyCode, branchCode);
                    }
                }
            }
            // Here am returning true there should be clarification required from Magna Team that which ID need to use for saving sales details.
            return true;
        }
        public bool ValidateSpecialDiscount(SalesBillingVM sales, out ErrorVM error, MagnaDbEntities db)
        {
            error = null;
            if (SIGlobals.Globals.GetConfigurationValue(db, "35", sales.CompanyCode, sales.BranchCode) == 0) {
                SalesInvoiceAttributeVM salesInvoice = sales.salesInvoiceAttribute;
                if (salesInvoice != null) {
                    decimal tollerenceAmount = Convert.ToDecimal(SIGlobals.Globals.GetTollerance(db, 120, sales.CompanyCode, sales.BranchCode).Max_Val);
                    if (salesInvoice.AdditionalDiscount > 0 && salesInvoice.AdditionalDiscount > tollerenceAmount) {

                        error = new ErrorVM()
                        {
                            description = "Additional Discount Limit Exceeded",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                }
            }
            return true;
        }

        public bool ValidationSalesBillingPayment(SalesBillingVM sales, out ErrorVM error, MagnaDbEntities db = null)
        {
            if (db == null) {
                db = new MagnaDbEntities();
            }
            string companyCode = sales.CompanyCode;
            string branchCode = sales.BranchCode;
            decimal totalCashPaid = 0;
            decimal totalAmtPaid = 0;
            error = null;

            if (sales.lstOfPayment.Count > 0) {
                var lstOfTotalPayments = sales.lstOfPayment.Where(pay => pay.CompanyCode == companyCode
                                                                                && pay.BranchCode == branchCode);
                #region Cash Validation
                totalAmtPaid = Convert.ToDecimal(lstOfTotalPayments.Sum(amt => amt.PayAmount));
                var lstOfCashPay = lstOfTotalPayments.Where(pay => pay.PayMode == "C");
                if (lstOfCashPay.Count() > 1) {
                    error = new ErrorVM()
                    {
                        description = "Cash payment already added",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
                totalCashPaid = Convert.ToDecimal(lstOfCashPay.Sum(amt => amt.PayAmount));
                if (totalCashPaid > 0) {
                    KSTU_TOLERANCE_MASTER tollerance = db.KSTU_TOLERANCE_MASTER.Where(toll => toll.obj_id == 201
                                                                                        && toll.company_code == companyCode
                                                                                        && toll.branch_code == branchCode).FirstOrDefault();
                    if (tollerance != null) {
                        if (totalCashPaid >= tollerance.Max_Val) {
                            error = new ErrorVM()
                            {
                                description = string.Format("Cash receipt is not allowed for more than {0}", tollerance.Max_Val),
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                            };
                            return false;
                        }
                    }
                }
                #endregion

                #region Card Validation
                List<PaymentVM> cardPayment = lstOfTotalPayments.Where(pay => pay.PayMode == "R").ToList();
                if (cardPayment.Count > 0) {
                    foreach (PaymentVM pay in cardPayment) {
                        if (string.IsNullOrEmpty(pay.CardType)) {
                            error = new ErrorVM()
                            {
                                description = "Please enter valid card type",
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                            };
                            return false;
                        }
                        if (pay.Bank == null || pay.Bank == "") {
                            error = new ErrorVM()
                            {
                                description = "Please Select Bank",
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                            };
                            return false;
                        }
                        if (string.IsNullOrEmpty(pay.RefBillNo) || pay.RefBillNo.Equals("0") || pay.RefBillNo.Length < 4) {
                            error = new ErrorVM()
                            {
                                description = "Please enter valid card no",
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                            };
                            return false;
                        }
                        if (string.IsNullOrEmpty(pay.CardAppNo) || pay.CardAppNo.Equals("0")) {
                            error = new ErrorVM()
                            {
                                description = "Please enter card approval no",
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                            };
                            return false;
                        }
                        if (pay.ExpiryDate < DateTime.Now.Date) {
                            error = new ErrorVM()
                            {
                                description = string.Format("Card Validity Expired."),
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                            };
                            return false;
                        }
                        if (string.IsNullOrEmpty(pay.CardSwipedBy)) {
                            error = new ErrorVM()
                            {
                                description = string.Format("Select Card Swiped By "),
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                            };
                            return false;
                        }
                    }
                }
                #endregion

                #region Cheque and DD Validation
                List<PaymentVM> chequePayment = lstOfTotalPayments.Where(pay => pay.PayMode == "Q" || pay.PayMode == "D").ToList();
                foreach (PaymentVM pay in chequePayment) {
                    if (string.IsNullOrEmpty(pay.ChequeNo) || Convert.ToDecimal(pay.ChequeNo) == 0) {
                        error = new ErrorVM()
                        {
                            description = "Please enter valid Cheque/DD no",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                    if (pay.ChequeNo.Length != 6) {
                        error = new ErrorVM()
                        {
                            description = "Enter valid cheque or DD no. Cheque no sholud be 6 digits.",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                    if (string.IsNullOrEmpty(pay.BankName)) {
                        error = new ErrorVM()
                        {
                            description = "Please enter valid Bank name",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                    if (pay.ChequeDate < DateTime.Today.Date) {
                        error = new ErrorVM()
                        {
                            description = string.Format("Cheque date should not be less than the {0}", DateTime.Now.Date),
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }

                    if (pay.ChequeDate > DateTime.Today.Date.AddMonths(3)) {
                        error = new ErrorVM()
                        {
                            description = "Cheque date cannot be greater than 3 months",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                }
                #endregion

                #region Gift Voucher
                List<PaymentVM> giftVoucherPayment = lstOfTotalPayments.Where(pay => pay.PayMode == "GV").ToList();
                foreach (PaymentVM pay in giftVoucherPayment) {
                    if (string.IsNullOrEmpty(pay.PayDetails)) {
                        error = new ErrorVM()
                        {
                            description = "Please enter narration",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                }
                #endregion

                #region Order
                List<PaymentVM> orderPayments = lstOfTotalPayments.Where(pay => pay.PayMode == "OP").ToList();
                foreach (PaymentVM pay in orderPayments) {
                    int orderNo = Convert.ToInt32(pay.RefBillNo);
                    int estNo = Convert.ToInt32(sales.EstNo);
                    int glbOrderNo = db.KTTU_SALES_EST_MASTER.Where(sal => sal.company_code == companyCode
                                                                    && sal.branch_code == branchCode
                                                                    && sal.est_no == estNo).FirstOrDefault().order_no;

                    if (orderNo == glbOrderNo) {
                        error = new ErrorVM()
                        {
                            description = "Order No. already added to payment details",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                    if (!new OrderBL().ValidateOrder(orderNo, companyCode, branchCode, "", 0, out error)) {
                        return false;
                    }
                }
                #endregion

                #region Manual Chit
                // This is not required for Bhima informed by Ramkumar on 10/12/2020 2:40 PM
                //var manualChitPay = lstOfTotalPayments.Where(sal => sal.PayMode == "OCT");

                #endregion

                #region Other Branch Orders
                var otherBranchPayments = lstOfTotalPayments.Where(pay => pay.PayMode == "B");
                foreach (PaymentVM pay in otherBranchPayments) {
                    {
                        if (string.IsNullOrEmpty(pay.RefBillNo) || Convert.ToInt32(pay.RefBillNo) == 0) {
                            error = new ErrorVM()
                            {
                                description = "Enter valid Ref no",
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                            };
                            return false;
                        }
                        if (pay.RefBillNo.Length > 8) {
                            error = new ErrorVM()
                            {
                                description = "enter valid Ref no",
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                            };
                            return false;
                        }
                        if (string.IsNullOrEmpty(pay.PayDetails)) {
                            error = new ErrorVM()
                            {
                                description = "Please enter narration",
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                            };
                            return false;
                        }
                    }
                    // Order Manual Validation function not included. Not required informed by Ramkumar on 10/12/2020 3:11 PM
                }

                #endregion

                #region Checking Duplcate Attached Bill
                IDictionary<string, string> duplicatePay = new Dictionary<string, string>();
                duplicatePay.Add("PB", "Purchase Bill");
                duplicatePay.Add("PE", "Purchase Est.");
                duplicatePay.Add("SR", "SR Bill");
                duplicatePay.Add("SE", "SR Est.");
                duplicatePay.Add("OP", "Order");
                duplicatePay.Add("Q", "ChequeNo.");
                duplicatePay.Add("OCT", "Chit.");

                foreach (KeyValuePair<string, string> kvp in duplicatePay) {
                    var purchaseBillPayments = lstOfTotalPayments.Where(pay => pay.PayMode == kvp.Key);
                    foreach (PaymentVM pay in purchaseBillPayments) {
                        int count = purchaseBillPayments.Count(p => p.RefBillNo == pay.RefBillNo);
                        if (count > 1) {
                            error = new ErrorVM()
                            {
                                description = kvp.Value + " already added to payment details",
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                            };
                            return false;
                        }
                    }
                }
                #endregion

                #region Other Branch Duplicate
                foreach (PaymentVM branch in lstOfTotalPayments) {
                    if (branch.PayMode.Substring(0, 1) == "B") {
                        int count = lstOfTotalPayments.Count(p => p.PayMode == branch.PayMode && p.RefBillNo == branch.RefBillNo);
                        if (count > 1) {
                            error = new ErrorVM()
                            {
                                description = "Ref No. is already added to payment details",
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                            };
                            return false;
                        }
                    }
                }
                #endregion

                #region Chit Adjust Validation
                if (SIGlobals.Globals.GetConfigurationValue(db, "08012018", sales.CompanyCode, sales.BranchCode) == 0) {
                    var lstOfChitPayments = sales.lstOfPayment.Where(p => p.PayMode == "CT").ToList();
                    if (lstOfChitPayments.Count > 0) {
                        var offerDiscount = Convert.ToDecimal(db.KTTU_SALES_EST_DETAILS.Where(sal => sal.est_no == sales.EstNo
                                                                        && sal.company_code == sales.CompanyCode
                                                                        && sal.branch_code == sales.BranchCode).Sum(det => det.offer_value));
                        if (offerDiscount > 0) {
                            error = new ErrorVM()
                            {
                                description = "Offer discount wil not be applicable for Chit Adjustment.",
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                            };
                            return false;
                        }
                    }
                }
                #endregion
            }

            if (sales.lstOfPayToCustomer.Count > 0) {
                #region Cash Validation
                totalCashPaid = Convert.ToDecimal(sales.lstOfPayToCustomer.Where(pay => pay.PayMode == "C").Sum(amt => amt.PayAmount));
                if (totalCashPaid > 0) {
                    KSTU_TOLERANCE_MASTER ktm = db.KSTU_TOLERANCE_MASTER.Where(kt => kt.obj_id == 6
                                                                                   && kt.company_code == companyCode
                                                                                   && kt.branch_code == branchCode).FirstOrDefault();
                    if (totalCashPaid > ktm.Max_Val) {
                        error = new ErrorVM()
                        {
                            field = "Amount",
                            index = 0,
                            description = "Cash should not be more than " + ktm.Max_Val + "/-",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                }
                #endregion
            }
            return true;
        }

        public bool ValidationSalesBillingPaymentToCustomer(SalesBillingVM sales, out ErrorVM error, MagnaDbEntities db = null)
        {
            if (db == null) {
                db = new MagnaDbEntities();
            }

            string companyCode = sales.CompanyCode;
            string branchCode = sales.BranchCode;
            //decimal totalCashPaid = 0;
            error = new ErrorVM();


            return true;
        }

        public bool UpdateUnusedOTP(string companyCode, string branchCode, string mobileno, int otp, string otpType, string machineName, int docno, string operatorcode)
        {
            // This method will reset the Mobile Number otp incase already sent and but not used.
            // This is the implementation of procedure [dbo].[USP_UPDATEUNUSEDOTP]
            MobileOTP mobileOTP = db.MobileOTPs.Where(o => o.company_code == companyCode
                                                        && o.branch_code == branchCode
                                                        && o.OTPtype == otpType
                                                        && o.computer_name == machineName
                                                        && o.docno == docno
                                                        && o.authenticated_by == operatorcode
                                                        && o.IsUsed == "N"
                                                        && o.UsedTime == null
                                                        && (o.isexpired == null || o.isexpired == "N")).FirstOrDefault();
            if (mobileOTP != null) {
                mobileOTP.isexpired = "Y";
                mobileOTP.UsedTime = SIGlobals.Globals.GetDateTime();
                db.Entry(mobileOTP).State = System.Data.Entity.EntityState.Modified;
            }
            return true;
        }

        public bool OrderCloseOTPSMS(string companyCode, string branchCode, string computerName, string operatorCode, string otptype, int otpLength, int otpValidityMinutes, int invoiceNo, string type, string autendicatedby, string custmobileNo, out ErrorVM error)
        {
            error = null;
            // Bellow is the code to call Stored Procedure with parameter but its not working with the existing Procedure so written bellow code.
            //ObjectParameter smsText = new ObjectParameter("smsText", typeof(string));
            //ObjectParameter mobileNo = new ObjectParameter("mobileNo", typeof(string));
            //ObjectParameter smsID = new ObjectParameter("smsID", typeof(int));
            //ObjectParameter errorMsg = new ObjectParameter("errorMsg", typeof(string));
            //var result = db.SendOTPToMobile(companyCode, branchCode, computerName, operatorCode, otptype, otpLength, otpValidityMinutes, invoiceNo, autendicatedby, custmobileNo, smsText, mobileNo, smsID, errorMsg);
            //if (result != null && smsText.Value.ToString() != "" && Convert.ToInt32(smsID.Value) != 0) {
            //    if (!new Common.CommonBL().SendSMS(Convert.ToString(smsText.Value), Convert.ToString(mobileNo.Value), Convert.ToInt32(smsID.Value))) {
            //        error.description = errorMsg.Value.ToString();
            //        error.ErrorStatusCode = System.Net.HttpStatusCode.BadRequest;
            //        return false;
            //    }
            //    ORDER_OTP_ATTRIBUTES.Add(new OrderOTPAttributes() { IsValidated = false, MobileNo = custmobileNo, OrderNo = invoiceNo, SMSID = Convert.ToInt32(smsID.Value) });
            //}


            var result = db.SendOTPToMobile1(companyCode, branchCode, computerName, operatorCode, otptype, otpLength, otpValidityMinutes, invoiceNo, autendicatedby, custmobileNo);
            SendOTPToMobile1_Result otpDet = result.FirstOrDefault();
            if (otpDet != null) {
                if (!new Common.CommonBL().SendSMS(otpDet.smsText, otpDet.MobileNo, Convert.ToInt32(otpDet.smsID))) {
                    error.description = "Error Occurred while sending an OTP";
                    error.ErrorStatusCode = System.Net.HttpStatusCode.BadRequest;
                    return false;
                }
                int intSMSID = Convert.ToInt32(otpDet.smsID);
                ORDER_OTP_ATTRIBUTES.Add(new OrderOTPAttributes() { IsValidated = false, MobileNo = custmobileNo, OrderNo = invoiceNo, SMSID = intSMSID });
            }
            return true;
        }

        public bool ValidateOrderClosingOTP(string companyCode, string branchCode, string mobileNo, int password, string otpType, string machineName, int docNo, string operatorCode)
        {
            //ObjectParameter errorMsg = new ObjectParameter("errorMsg", typeof(string));
            //int validate = db.usp_ValidateOTPUserApprove(companyCode, branchCode, mobileNo, password, otpType, machineName, docNo, operatorCode, errorMsg);
            //if (errorMsg.Value.ToString() == "") {
            //    return false;
            //}

            MobileOTP mobileOTP = db.MobileOTPs.Where(otp => otp.company_code == companyCode
                                                     && otp.branch_code == branchCode
                                                     && otp.OTPtype == otpType
                                                     && otp.OTP == password
                                                     && otp.computer_name == machineName
                                                     && otp.operator_code == operatorCode
                                                     && otp.docno == docNo
                                                     && otp.IsUsed == "N"
                                                     && (otp.isexpired == null || otp.isexpired == "N")
                                                     && otp.UsedTime == null).FirstOrDefault();
            if (mobileOTP == null) {
                return false;
            }

            //mobileOTP.IsUsed = "Y";
            //mobileOTP.UsedTime = SIGlobals.Globals.GetDateTime();
            //db.Entry(mobileOTP).State = System.Data.Entity.EntityState.Modified;
            //db.SaveChanges();

            string query = "UPDATE dbo.MobileOTP " +
                                        "SET" +
                                        "    MobileOTP.IsUsed = 'Y'," +
                                        "    MobileOTP.UsedTime = GETDATE()" +
                                        "WHERE MobileOTP.company_code = '" + companyCode + "'" +
                                         "     AND MobileOTP.branch_code = '" + branchCode + "'" +
                                         "     AND MobileOTP.OTPtype = '" + otpType + "'" +
                                         "     AND MobileOTP.OTP = '" + password + "'" +
                                         "     AND MobileOTP.computer_name = '" + machineName + "'" +
                                         "     AND MobileOTP.docno = " + docNo + "" +
                                         "     AND MobileOTP.UsedTime IS NULL" +
                                         "     AND(MobileOTP.isexpired IS NULL" +
                                         "          OR MobileOTP.isexpired = 'N'); ";
            db.Database.ExecuteSqlCommand(query);
            return true;
        }

        public List<OrderOTPPaymentVM> GetListOfOrderAttached(SalesBillingVM sales, int globalOrderNo, out ErrorVM error)
        {
            error = null;
            List<int> lstOfOrdes = new List<int>();
            List<OrderOTPPaymentVM> orderDictionary = new List<OrderOTPPaymentVM>();

            // Taking any Order attached Globally
            if (globalOrderNo != 0) {
                lstOfOrdes.Add(globalOrderNo);
            }
            //Taking orders attached at the time of Estimation
            List<KTTU_PAYMENT_DETAILS> attachedOrders = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.company_code == sales.CompanyCode
                                                                           && pay.branch_code == sales.BranchCode
                                                                           && pay.trans_type == "A"
                                                                           && pay.pay_mode == "OP"
                                                                           && pay.cflag != "Y"
                                                                           && pay.series_no == sales.EstNo).ToList();
            foreach (KTTU_PAYMENT_DETAILS pay in attachedOrders) {
                lstOfOrdes.Add(Convert.ToInt32(pay.Ref_BillNo));
            }

            // Taking Orders attached at the time of Biling.
            if (sales.lstOfPayment.Count > 0) {
                List<PaymentVM> lstOfOrderPayment = sales.lstOfPayment.Where(pay => pay.PayMode == "OP").ToList();
                if (lstOfOrderPayment.Count > 0) {
                    foreach (PaymentVM pay in lstOfOrderPayment) {
                        lstOfOrdes.Add(Convert.ToInt32(pay.RefBillNo));
                    }
                    List<KTTU_ORDER_MASTER> orderMaster = db.KTTU_ORDER_MASTER.Where(o => o.company_code == sales.CompanyCode
                                                                                && o.branch_code == sales.BranchCode
                                                                                && lstOfOrdes.Contains(o.order_no)).ToList();
                    foreach (KTTU_ORDER_MASTER order in orderMaster) {
                        orderDictionary.Add(new OrderOTPPaymentVM() { MobileNo = order.mobile_no, OrderNo = order.order_no });
                    }
                }
            }
            return orderDictionary;
        }

        public SalesMasterVM GetBillInfoByBillNo(string companyCode, string branchCode, int billNo, out ErrorVM error)
        {
            error = null;
            SalesMasterVM sales = new SalesMasterVM();
            KTTU_SALES_MASTER salesMaster = db.KTTU_SALES_MASTER.Where(sal => sal.company_code == companyCode
                                                                        && sal.branch_code == branchCode
                                                                        && sal.bill_no == billNo).FirstOrDefault();
            DateTime appDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);

            if (appDate.Date != salesMaster.bill_date.Date) {
                error = new ErrorVM()
                {
                    description = "Only Today's Bill is allowed for Payment Edit. ",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return null;
            }
            if (salesMaster == null) {
                error = new ErrorVM()
                {
                    description = "Invalid Bill No",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return null;
            }
            var paymentDet = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.company_code == companyCode
                                                                            && pay.branch_code == branchCode
                                                                            && pay.series_no == billNo
                                                                            && pay.trans_type == "S" && pay.cflag != "Y");
            decimal paidAmount = 0;
            if (paymentDet != null) {
                paidAmount = Convert.ToDecimal(paymentDet.Sum(pay => pay.pay_amt));
            }

            var salesDetails = db.KTTU_SALES_DETAILS.Where(sal => sal.company_code == companyCode
                                                                       && sal.branch_code == branchCode
                                                                       && sal.bill_no == billNo);
            var totalLineAmt = Convert.ToDecimal(salesDetails.Sum(x => x.total_amount));
            sales.BillNo = salesMaster.bill_no;
            sales.CompanyCode = salesMaster.company_code;
            sales.BranchCode = salesMaster.branch_code;
            sales.Salutation = salesMaster.salutation;
            sales.CustomerName = salesMaster.cust_name;
            sales.Address1 = salesMaster.address1;
            sales.Address2 = salesMaster.address2;
            sales.Address3 = salesMaster.address3;
            sales.City = salesMaster.city;
            sales.State = salesMaster.state;
            sales.StateCode = salesMaster.state_code;
            sales.PinCode = salesMaster.pin_code;
            sales.PANNo = salesMaster.pan_no;
            sales.MobileNo = salesMaster.mobile_no;
            sales.BillDate = salesMaster.bill_date;
            sales.GrandTotal = totalLineAmt;
            sales.BillAmount = Math.Round(totalLineAmt, 0, MidpointRounding.ToEven);
            sales.PaidAmount = Math.Round(paidAmount, 0, MidpointRounding.ToEven);
            sales.BalanceAmount = sales.BillAmount - sales.PaidAmount;

            List<PaymentVM> payment = new List<PaymentVM>();
            foreach (KTTU_PAYMENT_DETAILS pvm in paymentDet) {
                PaymentVM kpd = new PaymentVM();
                kpd.CompanyCode = pvm.company_code;
                kpd.BranchCode = pvm.branch_code;
                kpd.SeriesNo = pvm.series_no;
                kpd.ReceiptNo = pvm.receipt_no;
                kpd.SNo = pvm.sno;
                kpd.TransType = pvm.trans_type;
                kpd.PayMode = pvm.pay_mode;
                kpd.PayDetails = pvm.pay_details;
                kpd.PayDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                kpd.PayAmount = pvm.pay_amt;
                kpd.RefBillNo = pvm.Ref_BillNo;
                kpd.PartyCode = pvm.party_code;
                kpd.BillCounter = pvm.bill_counter;
                kpd.IsPaid = pvm.is_paid;
                kpd.Bank = pvm.bank;
                kpd.BankName = pvm.bank_name;
                kpd.ChequeDate = pvm.cheque_date;
                kpd.CardType = pvm.card_type;
                kpd.ExpiryDate = pvm.expiry_date;
                kpd.CFlag = pvm.cflag;
                kpd.CardAppNo = pvm.card_app_no;
                kpd.SchemeCode = pvm.scheme_code;
                kpd.SalBillType = pvm.sal_bill_type;
                kpd.OperatorCode = pvm.operator_code;
                kpd.UpdateOn = pvm.UpdateOn;
                kpd.GroupCode = pvm.group_code;
                kpd.AmtReceived = pvm.amt_received;
                kpd.BonusAmt = pvm.bonus_amt;
                kpd.WinAmt = pvm.win_amt;
                kpd.CTBranch = pvm.ct_branch;
                kpd.FinYear = pvm.fin_year;
                kpd.CardCharges = pvm.CardCharges;
                kpd.ChequeNo = pvm.cheque_no;
                kpd.NewBillNo = pvm.New_Bill_No;
                kpd.AddDisc = pvm.Add_disc;
                kpd.IsOrderManual = pvm.isOrdermanual;
                kpd.CurrencyValue = pvm.currency_value;
                kpd.ExchangeRate = pvm.exchange_rate;
                kpd.CurrencyType = pvm.currency_type;
                kpd.TaxPercentage = pvm.tax_percentage;
                kpd.CancelledBy = pvm.cancelled_by;
                kpd.CancelledRemarks = pvm.cancelled_remarks;
                kpd.CancelledDate = pvm.cancelled_date;
                kpd.IsExchange = pvm.isExchange;
                kpd.ExchangeNo = pvm.exchangeNo;
                kpd.NewReceiptNo = pvm.new_receipt_no;
                kpd.GiftAmount = pvm.Gift_Amount;
                kpd.CardSwipedBy = pvm.cardSwipedBy;
                kpd.Version = pvm.version;
                kpd.GSTGroupCode = pvm.GSTGroupCode;
                kpd.SGSTPercent = pvm.SGST_Percent;
                kpd.CGSTPercent = pvm.CGST_Percent;
                kpd.IGSTPercent = pvm.IGST_Percent;
                kpd.HSN = pvm.HSN;
                kpd.SGSTAmount = pvm.SGST_Amount;
                kpd.CGSTAmount = pvm.CGST_Amount;
                kpd.IGSTAmount = pvm.IGST_Amount;
                kpd.PayAmountBeforeTax = pvm.pay_amount_before_tax;
                kpd.PayTaxAmount = pvm.pay_tax_amount;
                payment.Add(kpd);
            }
            sales.PaymentVM = payment;
            return sales;
        }

        public bool PayToCustomerValidation(decimal cashAmtPaid, string companyCode, string branchCode, out ErrorVM error)
        {
            error = new ErrorVM();
            if (cashAmtPaid > 0) {
                KSTU_TOLERANCE_MASTER ktm = db.KSTU_TOLERANCE_MASTER.Where(kt => kt.obj_id == 6
                                                                               && kt.company_code == companyCode
                                                                               && kt.branch_code == branchCode).FirstOrDefault();
                if (cashAmtPaid >= ktm.Max_Val) {
                    error = new ErrorVM()
                    {
                        field = "Amount",
                        index = 0,
                        description = "Cash should not be more than " + ktm.Max_Val + "/-",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
            }
            return true;
        }

        public bool GenerateOrderAsPayment(MagnaDbEntities db, int orderNo, int billNo, DateTime appDate, string objectID, int paySlNo, int finYear, string operatorCode, string companyCode, string branchCode)
        {
            KTTU_PAYMENT_DETAILS kpd = new KTTU_PAYMENT_DETAILS();
            KTTU_ORDER_MASTER order = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == orderNo
                                                                 && ord.company_code == companyCode
                                                                 && ord.branch_code == branchCode).FirstOrDefault();
            order.bill_no = billNo;
            order.UpdateOn = SIGlobals.Globals.GetDateTime();
            order.closed_date = appDate;
            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
            // Adding Order details as a payment.
            kpd.obj_id = objectID;
            kpd.company_code = order.company_code;
            kpd.branch_code = order.branch_code;
            kpd.series_no = billNo;
            kpd.receipt_no = 0;
            kpd.sno = paySlNo;
            kpd.trans_type = "S";
            kpd.pay_mode = "OP";
            kpd.pay_details = "Ord Adv";
            kpd.pay_date = SIGlobals.Globals.GetApplicationDate(order.company_code, order.branch_code);
            kpd.pay_amt = order.advance_ord_amount;
            kpd.Ref_BillNo = Convert.ToString(order.order_no);
            kpd.party_code = null;
            kpd.bill_counter = "FF";
            kpd.is_paid = "N";
            kpd.bank = null;
            kpd.bank_name = null;
            kpd.cheque_date = null;
            kpd.card_type = null;
            kpd.expiry_date = null;
            kpd.cflag = "N";
            kpd.card_app_no = null;
            kpd.scheme_code = null;
            kpd.sal_bill_type = null;
            kpd.operator_code = operatorCode;
            kpd.session_no = null;
            kpd.UpdateOn = SIGlobals.Globals.GetDateTime();
            kpd.group_code = null;
            kpd.amt_received = 0;
            kpd.bonus_amt = 0;
            kpd.win_amt = 0;
            kpd.ct_branch = null;
            kpd.fin_year = finYear;
            kpd.CardCharges = null;
            kpd.cheque_no = null;
            kpd.New_Bill_No = Convert.ToString(billNo);
            kpd.Add_disc = null;
            kpd.isOrdermanual = "N";
            kpd.currency_value = 0;
            kpd.exchange_rate = 0;
            kpd.currency_type = "INR";
            kpd.tax_percentage = 0;
            kpd.cancelled_by = "";
            kpd.cancelled_remarks = "";
            kpd.cancelled_date = null;
            kpd.isExchange = "N";
            kpd.exchangeNo = 0;
            kpd.new_receipt_no = null;
            kpd.Gift_Amount = null;
            kpd.cardSwipedBy = null;
            kpd.version = null;
            kpd.GSTGroupCode = null;
            kpd.SGST_Percent = 0;
            kpd.CGST_Percent = 0;
            kpd.IGST_Percent = 0;
            kpd.HSN = null;
            kpd.SGST_Amount = 0;
            kpd.CGST_Amount = 0;
            kpd.IGST_Amount = 0;
            kpd.pay_amount_before_tax = order.advance_ord_amount;
            kpd.pay_tax_amount = 0;
            kpd.UniqRowID = Guid.NewGuid();
            db.KTTU_PAYMENT_DETAILS.Add(kpd);
            return true;
        }
        #endregion

        #region Private Methods
        private bool CancelSalesPayments(MagnaDbEntities dbContext, string companyCode, string branchCode, int salesBillNo, string cancellationRemarks, string userId, out string errorMessage)
        {
            errorMessage = string.Empty;
            var paymentDetails = dbContext.KTTU_PAYMENT_DETAILS.Where(
                pd => pd.company_code == companyCode && pd.branch_code == branchCode &&
                pd.series_no == salesBillNo &&
                (pd.trans_type == "S" || pd.trans_type == "PC")).ToList();
            foreach (var pd in paymentDetails) {
                string payBranch = pd.pay_mode.Substring(0, 1);
                int documentNo = 0;
                if (!string.IsNullOrEmpty(pd.Ref_BillNo) &&
                    (string.Compare(pd.pay_mode, "PB") == 0 || string.Compare(pd.pay_mode, "SE") == 0 ||
                    string.Compare(pd.pay_mode, "OP") == 0 || string.Compare(payBranch, "B") == 0)) {
                    documentNo = Convert.ToInt32(pd.Ref_BillNo);
                }
                if (string.Compare(pd.pay_mode, "CT") == 0) {
                    if (!CancelScheme(dbContext, companyCode, branchCode, pd.ct_branch, pd.party_code,
                        pd.scheme_code, pd.group_code, Convert.ToInt32(pd.Ref_BillNo), out errorMessage))
                        return false;
                }
                if (string.Compare(pd.pay_mode, "PB") == 0 && documentNo > 0) {
                    ErrorVM error = null;
                    PurchaseBillingBL purchaseBL = new PurchaseBillingBL(dbContext);
                    bool cancelResult = purchaseBL.CancelPurchaseBill(companyCode, branchCode, Convert.ToInt32(pd.Ref_BillNo), cancellationRemarks, userId, out error);
                    if (cancelResult == false) {
                        if (error != null) {
                            errorMessage = error.description;
                        }
                        return false;
                    }
                }
                if (string.Compare(pd.pay_mode, "SE") == 0 && documentNo > 0) {
                    ErrorVM error = null;
                    ConfirmSalesReturnVM confirmSR = new ConfirmSalesReturnVM
                    {
                        CompanyCode = companyCode,
                        BranchCode = branchCode,
                        BillNo = documentNo,
                        OperatorCode = userId,
                        CancleRemarks = cancellationRemarks
                    };
                    ConfirmSalesReturnBL srBL = new ConfirmSalesReturnBL(dbContext);
                    bool cancelResult = srBL.CancelSRBill(confirmSR, out error);
                    if (cancelResult == false) {
                        if (error != null) {
                            errorMessage = error.description;
                        }
                        return false;
                    }
                }
                if (string.Compare(pd.trans_type, "S") == 0 && string.Compare(pd.pay_mode, "OP") == 0 && documentNo > 0) {
                    if (!CancelAttachedOrderPayment(dbContext, companyCode, branchCode, documentNo, out errorMessage))
                        return false;
                }

                if (string.Compare(pd.trans_type, "PC") == 0 && string.Compare(pd.pay_mode, "Q") == 0) {
                    if (!UpdateChequeNo(dbContext, companyCode, branchCode, Convert.ToInt32(pd.cheque_no), Convert.ToInt32(pd.bank)))
                        return false;
                }

                if (string.Compare(pd.trans_type, "PC") == 0 && string.Compare(pd.pay_mode, "OP") == 0) {
                    if (!CancelBillOrder(dbContext, companyCode, branchCode, documentNo, pd.series_no, cancellationRemarks, out errorMessage))
                        return false;
                }
                if (string.Compare(payBranch, "B") == 0) {
                    //if (!CancelOtherBranchOrder(dbContext, companyCode, branchCode, documentNo, paymentList.party_code, paymentList.isOrdermanual))
                    //    return false;

                    #region Update Manual Order
                    pd.isOrdermanual = "N";
                    #endregion

                }
                pd.cflag = "Y";
                pd.UpdateOn = SIGlobals.Globals.GetDateTime();
                dbContext.Entry(pd).State = System.Data.Entity.EntityState.Modified;
            }
            return true;
        }

        private bool UpdateChequeNo(MagnaDbEntities dbContext, string companyCode, string branchCode, int chequeNo, int accountCode)
        {
            var chequeMaster = dbContext.KSTU_ACC_CHEQUE_MASTER.Where(
                cm => cm.company_code == companyCode && cm.branch_code == branchCode &&
                cm.chq_no == chequeNo && cm.acc_code == accountCode).FirstOrDefault();
            if (chequeMaster != null) {
                chequeMaster.chq_issued = "N";
                chequeMaster.chq_issue_date = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode).Date;
                chequeMaster.UpdateOn = SIGlobals.Globals.GetDateTime();
                dbContext.Entry(chequeMaster).State = System.Data.Entity.EntityState.Modified;
            }
            return true;
        }

        private bool CancelBillOrder(MagnaDbEntities dbContext, string companyCode, string branchCode, int orderNo, int salesBillNo, string cancelRemarks, out string errorMessage)
        {
            errorMessage = string.Empty;
            var orderMaster = dbContext.KTTU_ORDER_MASTER.Where(
                    x => x.company_code == companyCode &&
                    x.branch_code == branchCode && x.order_no == orderNo).FirstOrDefault();
            if (orderMaster == null) {
                errorMessage = "Order attached to the sales invoice is not found.";
                return false;
            }
            if (orderMaster.closed_flag == "Y") {
                errorMessage = "Order No. " + orderNo.ToString() + " is closed. Sales invoice cannot be cancelled.";
                return false;
            }
            if (orderMaster.bill_no != 0) {
                errorMessage = string.Format("Order No. {0} is adjusted against sales bill No. {1} and therefore sales bill No. {2} cannot be cancelled."
                            , orderMaster.order_no, orderMaster.bill_no, salesBillNo);
                return false;
            }

            var paymentDetails = dbContext.KTTU_PAYMENT_DETAILS.Where(
                pd => pd.company_code == companyCode && pd.branch_code == branchCode &&
                pd.trans_type == "O" && pd.series_no == orderNo).ToList();
            decimal orderAmount = 0;
            foreach (var pd in paymentDetails) {
                if (pd.Ref_BillNo.Equals(salesBillNo.ToString())) {
                    pd.cflag = "Y";
                    orderAmount += pd.pay_amt == null ? 0 : (decimal)pd.pay_amt;
                    dbContext.Entry(pd).State = System.Data.Entity.EntityState.Modified;
                }
            }

            string operatorCode = "SI"; //TODO: Lets get this from token
            orderMaster.advance_ord_amount -= orderAmount;
            if (orderMaster.advance_ord_amount == 0) {
                orderMaster.cancelled_by = operatorCode;
                orderMaster.closed_date = orderMaster.order_date;
                orderMaster.remarks = cancelRemarks;
                orderMaster.cflag = "Y";
                dbContext.Entry(orderMaster).State = System.Data.Entity.EntityState.Modified;
            }
            return true;
        }

        private bool CancelScheme(MagnaDbEntities dbContext, string companyCode, string branchCode, string closingBranch,
            string schemeBranch, string schemeCode, string groupCode, int membershipNo, out string errorMessage)
        {
            errorMessage = string.Empty;
            var schemeInfo = dbContext.CHTU_CHIT_CLOSURE.Where(
                x => x.company_code == companyCode && x.branch_code == schemeBranch &&
                x.Closed_At == closingBranch && x.Group_Code == groupCode && x.Scheme_Code == schemeCode &&
                x.Chit_MembShipNo == membershipNo).ToList();
            foreach (var sc in schemeInfo) {
                sc.Bill_Number = "0";
                sc.Bill_Date = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                sc.IsUpdated = false;
                sc.Closed_At = closingBranch;
                dbContext.Entry(sc).State = System.Data.Entity.EntityState.Modified;
            }
            return true;
        }

        private bool CancelAttachedOrderPayment(MagnaDbEntities dbContext, string companyCode, string branchCode, int orderNo, out string errorMessage)
        {
            errorMessage = string.Empty;
            #region Order Master Cancellation
            var orderMaster = dbContext.KTTU_ORDER_MASTER.Where
                    (x => x.company_code == companyCode &&
                    x.branch_code == branchCode && x.order_no == orderNo).FirstOrDefault();
            if (orderMaster == null) {
                errorMessage = "Order attached to the sales invoice is not found.";
                return false;
            }
            orderMaster.bill_no = 0;
            orderMaster.UpdateOn = SIGlobals.Globals.GetDateTime();
            dbContext.Entry(orderMaster).State = System.Data.Entity.EntityState.Modified;
            #endregion

            #region Order Detail Cancellation
            var orderDetails = dbContext.KTTU_ORDER_DETAILS.Where
                    (x => x.company_code == companyCode &&
                    x.branch_code == branchCode && x.order_no == orderNo).FirstOrDefault();
            if (orderDetails == null) {
                errorMessage = "Order details attached to the sales invoice is not found.";
                return false;
            }
            orderDetails.bill_no = 0;
            orderDetails.s_flag = "Y";
            orderDetails.UpdateOn = SIGlobals.Globals.GetDateTime();
            dbContext.Entry(orderDetails).State = System.Data.Entity.EntityState.Modified;
            #endregion

            return true;
        }

        private bool ValidationOperatorDiscount(MagnaDbEntities db, SalesBillingVM sales, out ErrorVM error)
        {
            error = null;
            SalesInvoiceAttributeVM invoiceAttribute = sales.salesInvoiceAttribute;
            if (invoiceAttribute == null) {
                return true;
            }

            SDTU_OPERATOR_MASTER operater = db.SDTU_OPERATOR_MASTER.Where(op => op.company_code == sales.CompanyCode
                                                                            && op.branch_code == op.branch_code
                                                                            && op.OperatorCode == sales.OperatorCode).FirstOrDefault();
            if (operater != null) {
                if (invoiceAttribute.AdditionalDiscount > operater.discount_percent) {
                    error = new ErrorVM()
                    {
                        description = "Invalid discount! amount exceeded for this operator",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
            }
            return true;
        }

        private bool ValidationOfferOnOrderCreatedThroughChit(MagnaDbEntities db, SalesBillingVM sales, int orderNo, out ErrorVM error)
        {
            error = null;
            List<int> orders = new List<int>();
            orders.Add(orderNo);
            if (sales.salesInvoiceAttribute != null) {
                if (sales.salesInvoiceAttribute.OfferDiscount > 0) {
                    if (sales.lstOfPayment.Count > 0) {
                        foreach (PaymentVM payments in sales.lstOfPayment) {
                            if (payments.PayMode == "OP") {
                                orders.Add(Convert.ToInt32(payments.RefBillNo));
                            }
                        }
                    }
                    var paymentDetails = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.company_code == sales.CompanyCode
                                                                                                 && pay.branch_code == sales.BranchCode
                                                                                                 && pay.cflag != "Y"
                                                                                                 && pay.trans_type == "O"
                                                                                                 && orders.Contains(pay.series_no)).GroupBy(d => d.pay_mode).Select(x => x.FirstOrDefault());
                    foreach (var payment in paymentDetails) {
                        if (payment.pay_mode == "CT") {
                            error = new ErrorVM()
                            {
                                description = "Order is created through scheme adjustment.Offer discount is not applicable",
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                            };
                            return false;
                        }
                    }
                }
                return true;
            }
            else
                return true;
        }

        private bool ValidationCashTransactionAmount(MagnaDbEntities db, int custID, string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            decimal totalCashPaid = 0;
            decimal totalCashTran = 0;
            decimal maxCash = 0;
            maxCash = SIGlobals.Globals.GetTollerance(db, 201, companyCode, branchCode).Max_Val;

            DateTime billDate = db.KSTU_RATE_MASTER.FirstOrDefault().ondate;
            //Bellow is the logic written instead of this procedure "usp_ValidateForCashTransactions"
            var salesData = (from sal in db.KTTU_SALES_MASTER
                             join pay in db.KTTU_PAYMENT_DETAILS on
                             new { CompanyCode = sal.company_code, BranchCode = sal.branch_code, BillNo = sal.bill_no }
                             equals
                             new { CompanyCode = pay.company_code, BranchCode = pay.branch_code, BillNo = pay.series_no }
                             where sal.cflag != "Y"
                                   && pay.cflag != "Y"
                                   && pay.trans_type == "S"
                                   && pay.pay_mode == "C"
                                   && sal.cust_Id == custID
                                && System.Data.Entity.DbFunctions.TruncateTime(sal.bill_date) == System.Data.Entity.DbFunctions.TruncateTime(billDate)
                             group pay by new { pay } into g
                             select new
                             {
                                 PayAmount = g.Sum(x => x.pay_amt)
                             });

            var orderData = (from ord in db.KTTU_ORDER_MASTER
                             join pay in db.KTTU_PAYMENT_DETAILS on
                             new { CompanyCode = ord.company_code, BranchCode = ord.branch_code, BillNo = ord.order_no }
                             equals
                             new { CompanyCode = pay.company_code, BranchCode = pay.branch_code, BillNo = pay.series_no }
                             where ord.cflag != "Y"
                                    && pay.cflag != "Y"
                                    && pay.trans_type == "O"
                                    && pay.pay_mode == "C"
                                    && ord.Cust_Id == custID
                             && System.Data.Entity.DbFunctions.TruncateTime(ord.order_date) == System.Data.Entity.DbFunctions.TruncateTime(billDate)
                             group pay by new { pay } into g
                             select new
                             {
                                 PayAmount = g.Sum(x => x.pay_amt)
                             });
            totalCashTran = Convert.ToDecimal(salesData.Sum(p => p.PayAmount));
            totalCashTran = totalCashPaid + Convert.ToDecimal(salesData.Sum(p => p.PayAmount)) + Convert.ToDecimal(orderData.Sum(p => p.PayAmount));

            if (totalCashTran >= maxCash) {
                error = new ErrorVM()
                {
                    description = string.Format("Cash transaction limit exceeds. Cash transaction is not allowed for more than {0}", maxCash),
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            return true;
        }

        private bool ValidationOnPAN(MagnaDbEntities db, int custID, List<PaymentVM> payments, decimal totalBillAmount, string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            decimal totalSaleAmt = 0;
            decimal totalPurchaseAmt = 0;
            bool isPan = false;
            KSTU_TOLERANCE_MASTER panToll = SIGlobals.Globals.GetTollerance(db, 500, companyCode, branchCode);
            KSTU_RATE_MASTER rateMaster = db.KSTU_RATE_MASTER.FirstOrDefault();
            var data = db.KTTU_SALES_MASTER.Where(sal => sal.company_code == companyCode
                                              && sal.branch_code == branchCode
                                              && sal.cust_Id == custID
                                              && sal.cflag != "Y"
                                              && System.Data.Entity.DbFunctions.TruncateTime(sal.bill_date) == System.Data.Entity.DbFunctions.TruncateTime(rateMaster.ondate)).ToList();

            KSTU_CUSTOMER_MASTER customer = db.KSTU_CUSTOMER_MASTER.Where(cust => cust.cust_id == custID && cust.company_code == companyCode && cust.branch_code == branchCode).FirstOrDefault();
            if (data != null) {
                totalSaleAmt = totalBillAmount + Convert.ToDecimal(data.Sum(s => s.total_sale_amount)) + Convert.ToDecimal(data.Sum(s => s.total_tax_amount));
                if (panToll != null && totalSaleAmt > panToll.Max_Val) {
                    if (customer != null && (customer.pan_no == null || customer.pan_no == "")) {
                        error = new ErrorVM()
                        {
                            description = string.Format("Sales transaction limit exceeds, Please Update Valid PAN details"),
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                    else {
                        isPan = true;
                    }
                }
            }

            if (payments != null && payments.Count > 0) {
                totalPurchaseAmt = Convert.ToDecimal(payments.Where(pay => pay.PayMode == "PE").Sum(pay => pay.PayAmount));
            }
            var purchase = (from pur in db.KTTU_PURCHASE_MASTER
                            join pay in db.KTTU_PAYMENT_DETAILS on
                            new { CompanyCode = pur.company_code, BranchCode = pur.branch_code, BillNo = pur.bill_no } equals
                            new { CompanyCode = pay.company_code, BranchCode = pay.branch_code, BillNo = pay.series_no }
                            where pur.cflag != "Y" && pay.cflag != "Y" && pay.trans_type == "P" && pay.pay_mode == "C" && pur.cust_id == custID
                            select new { pay }
                          );
            totalPurchaseAmt = totalPurchaseAmt + Convert.ToDecimal(purchase.Sum(p => p.pay.pay_amt));
            if (totalPurchaseAmt >= panToll.Max_Val) {
                if (panToll != null && totalSaleAmt > panToll.Max_Val) {
                    error = new ErrorVM()
                    {
                        description = string.Format("Purchase transaction limit exceeds, Please Update Valid PAN details"),
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
            }
            return true;
        }

        private bool ValidateAdditionalandSpecialDscountPersentage(MagnaDbEntities db, SalesBillingVM sales, out ErrorVM error)
        {
            error = null;
            SalesInvoiceAttributeVM invoiceAttribute = sales.salesInvoiceAttribute;
            if (invoiceAttribute == null) {
                return true;
            }
            KSTU_TOLERANCE_MASTER toll = SIGlobals.Globals.GetTollerance(db, 14112018, sales.CompanyCode, sales.BranchCode);
            if (toll != null && toll.Max_Val > 0 && invoiceAttribute.DiscountPercent > toll.Max_Val) {
                error = new ErrorVM()
                {
                    description = "Discount percentage exceeded than the fixed discount percentage. Please enter valid remarks.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            return true;
        }

        private bool ValidateExcessPaymentAmount(MagnaDbEntities db, decimal totalPaidAmt, string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            KSTU_TOLERANCE_MASTER tollerance = SIGlobals.Globals.GetTollerance(db, 6032019, companyCode, branchCode);
            decimal maxPaidAmount = Convert.ToDecimal(tollerance == null ? 0 : tollerance.Max_Val);
            if (maxPaidAmount > 0 && totalPaidAmt > 0 && totalPaidAmt > maxPaidAmount) {
                error = new ErrorVM()
                {
                    index = 0,
                    description = string.Format("Paid amount should be less than the - {0}", maxPaidAmount),
                    ErrorStatusCode = System.Net.HttpStatusCode.Continue,
                };
                return false;
            }
            return true;
        }

        private bool ValidateAttachedPaymentdetails(MagnaDbEntities db, SalesBillingVM sales, out ErrorVM error)
        {
            error = null;
            bool returnValue = true;
            int refNo = 0;
            List<PaymentVM> lstOfPayments = sales.lstOfPayment;
            foreach (PaymentVM payment in lstOfPayments) {
                if (payment.PayMode != "R") {
                    refNo = Convert.ToInt32(payment.RefBillNo);
                }
                switch (payment.PayMode) {
                    case "PB":
                        returnValue = new PurchaseBillingBL().ValidatePurchaeBill(refNo, sales.CompanyCode, sales.BranchCode, out error);
                        break;
                    case "SR":
                        returnValue = new ConfirmSalesReturnBL().ValidateSalesReturnBillNo(refNo, sales.CompanyCode, sales.BranchCode, out error);
                        break;
                    case "OP":
                        returnValue = new OrderBL().ValidateOrder(refNo, sales.CompanyCode, sales.BranchCode, "SB", 0, out error);
                        break;
                    case "PE":
                        returnValue = new OldGoldPurchaseBL().ValidationPurchaseEstimation(refNo, sales.CompanyCode, sales.BranchCode, "SB", 0, out error);
                        break;
                    case "SE":
                        returnValue = new SalesReturnEstimationBL().ValidateSalesReturnEstimation(refNo, sales.CompanyCode, sales.BranchCode, "SB", 0, out error);
                        break;
                    case "B":
                        KTTU_BRANCH_ORDER_MASTER orderMaster = db.KTTU_BRANCH_ORDER_MASTER.Where(ord => ord.order_no == refNo
                                                                                                    && ord.company_code == sales.CompanyCode
                                                                                                    && ord.branch_code == sales.BranchCode
                                                                                                    && ord.cflag != "Y").FirstOrDefault();
                        if (orderMaster != null && orderMaster.bill_no != 0) {
                            error = new ErrorVM()
                            {
                                index = 0,
                                description = string.Format("Ref No {0} is already billed", refNo),
                                ErrorStatusCode = System.Net.HttpStatusCode.Continue,
                            };
                        }
                        break;
                    case "CT":
                        string schemecode = payment.PayDetails;
                        string branchCode = payment.CTBranch;
                        KTTU_PAYMENT_DETAILS chitPayment = db.KTTU_PAYMENT_DETAILS.Where(p => p.Ref_BillNo == payment.RefBillNo
                                                                                        && p.scheme_code == schemecode
                                                                                        && p.trans_type == "S"
                                                                                        && p.party_code == branchCode
                                                                                        && p.company_code == sales.CompanyCode
                                                                                        && p.branch_code == sales.BranchCode).FirstOrDefault();
                        if (chitPayment != null) {
                            error = new ErrorVM()
                            {
                                index = 0,
                                description = string.Format("Attached MSNo. is already billed."),
                                ErrorStatusCode = System.Net.HttpStatusCode.Continue,
                            };
                        }
                        break;
                }
            }
            return returnValue;
        }

        private dynamic LoadCustomerDetails(string mobileNO, string companyCode, string branchCode)
        {
            var customerMaster = (from mis in db.Customers
                                  join cust in db.KSTU_CUSTOMER_MASTER
                                  on mis.ID equals cust.RepoCustId
                                  where mis.MobileNo == mobileNO
                                  select new { mis }).Count();
            var misMaster = db.Customers.Where(c => c.MobileNo == mobileNO && c.CompanyCode == companyCode).Count();
            if (customerMaster == 0 && misMaster > 0) {
                Customer customer = db.Customers.Where(c => c.MobileNo == mobileNO && c.CompanyCode == companyCode).FirstOrDefault();
                return customer;
            }
            else {
                KSTU_CUSTOMER_MASTER master = db.KSTU_CUSTOMER_MASTER.Where(c => c.mobile_no == mobileNO
                                                                            && c.company_code == companyCode
                                                                            && c.branch_code == branchCode).FirstOrDefault();
                return master;
            }
        }

        private bool ValidateOrderOTP(SalesBillingVM sales, int globalOrderNo, out ErrorVM error, out bool otpValDone)
        {
            error = null;
            otpValDone = false;
            List<OrderOTPAttributes> orderOtp = sales.orderOTPAttributes;
            int missedCount = 0;
            if (orderOtp != null && orderOtp.Count > 0) {
                foreach (OrderOTPAttributes order in orderOtp) {
                    MobileOTP mobileOTP = db.MobileOTPs.Where(otp => otp.smsID == order.SMSID && otp.docno == order.OrderNo && otp.IsUsed == "N").FirstOrDefault();
                    if (mobileOTP == null && order.IsValidated) {
                        missedCount = missedCount + 1;
                    }
                }
                if (missedCount == 0) {
                    otpValDone = true;
                    return true;
                }
            }

            // Getting List Of Orders To Send OTP
            List<OrderOTPPaymentVM> orderDictionary = GetListOfOrderAttached(sales, globalOrderNo, out error);
            // Sending OTP:
            foreach (var ord in orderDictionary) {
                UpdateUnusedOTP(sales.CompanyCode, sales.BranchCode, ord.MobileNo, 0, "1", "", ord.OrderNo, sales.OperatorCode);
                if (!OrderCloseOTPSMS(sales.CompanyCode, sales.BranchCode, "", sales.OperatorCode, "1", 4, 10, ord.OrderNo, "1", sales.OperatorCode, ord.MobileNo, out error)) {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}