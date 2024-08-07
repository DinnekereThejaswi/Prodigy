using NumberToWordsINR;
using ProdigyAPI.BL.BusinessLayer.Common;
using ProdigyAPI.BL.BusinessLayer.Order;
using ProdigyAPI.BL.BusinessLayer.PrintConfig;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.BL.ViewModel.Payment;
using ProdigyAPI.BL.ViewModel.Print;
using ProdigyAPI.BL.ViewModel.Sales;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Linq;
using System.Text;


namespace ProdigyAPI.BL.BusinessLayer.Sales
{
    public class ConfirmSalesReturnBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = null;
        private CultureInfo indianDefaultCulture = new CultureInfo("hi-IN");
        private const string MODULE_SEQ_NO = "18";
        private const string TABLE_NAME = "KTTU_SR_MASTER";
        #endregion

        #region Constructor
        public ConfirmSalesReturnBL()
        {
            db = new MagnaDbEntities(true);
        }

        public ConfirmSalesReturnBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }
        #endregion

        #region Methods
        public SalesReturnMasterVM GetSalesReturn(string companyCode, string branchCode, int estNo, bool isEstimation, out ErrorVM error)
        {
            error = null;
            bool isCreditBill = false;
            decimal masterBalaceAmt = 0;
            decimal partialPaidAmt = 0;
            KTTU_SR_MASTER ksm = new KTTU_SR_MASTER();
            List<KTTU_SR_DETAILS> lstOfksd = new List<KTTU_SR_DETAILS>();
            List<KTTU_PAYMENT_DETAILS> kpd = new List<KTTU_PAYMENT_DETAILS>();
            KTTU_SALES_MASTER salesMaster = new KTTU_SALES_MASTER();

            if (isEstimation) {
                ksm = db.KTTU_SR_MASTER.Where(e => e.est_no == estNo
                                                && e.company_code == companyCode
                                                && e.branch_code == e.branch_code).FirstOrDefault();
                lstOfksd = db.KTTU_SR_DETAILS.Where(e => e.est_no == estNo
                                                && e.company_code == companyCode
                                                && e.branch_code == e.branch_code).ToList();
                if (ksm == null) {
                    error = new ErrorVM()
                    {
                        field = "SR Estimation",
                        description = "SR Estimation Not Found.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }

                //Checking SR Estimation is Attached to any other Sales Estimations
                KTTU_PAYMENT_DETAILS payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.company_code == companyCode
                                                                            && pay.branch_code == branchCode
                                                                            && pay.Ref_BillNo == estNo.ToString()
                                                                            && pay.trans_type == "A"
                                                                            && pay.pay_mode == "SE").FirstOrDefault();
                if (payment != null) {
                    error = new ErrorVM()
                    {
                        field = "SR Estimation",
                        description = string.Format("Sales Return Estimation No: {0} is attached with sales Estimaton {1}", estNo, payment.series_no),
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }

                if (ksm.sales_bill_no != 0) {
                    error = new ErrorVM()
                    {
                        field = "SR Estimation Number",
                        description = "SR Estimation Not Found.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }
            }
            else {
                ksm = db.KTTU_SR_MASTER.Where(e => e.sales_bill_no == estNo
                                              && e.company_code == companyCode
                                              && e.branch_code == e.branch_code).FirstOrDefault();
                lstOfksd = db.KTTU_SR_DETAILS.Where(e => e.sales_bill_no == estNo
                                                    && e.company_code == companyCode
                                                    && e.branch_code == e.branch_code).ToList();
                if (ksm == null) {
                    error = new ErrorVM()
                    {
                        field = "SR Estimation",
                        description = "SR Details Not Found.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }

                kpd = db.KTTU_PAYMENT_DETAILS.Where(e => e.series_no == estNo
                                           && e.company_code == companyCode
                                           && e.branch_code == e.branch_code
                                           && e.trans_type == "RS").ToList();


                //salesMaster = db.KTTU_SALES_MASTER.Where(sales => sales.bill_no == estNo
                //                                && sales.company_code == companyCode
                //                                && sales.branch_code == branchCode).FirstOrDefault();

                //if (salesMaster == null) {
                //    error = new ErrorVM()
                //    {
                //        field = "Sales Bill No",
                //        description = "Invalid sales Bill No",
                //        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                //    };
                //    return null;
                //}
                //else if (salesMaster.cflag == "Y") {
                //    error = new ErrorVM()
                //    {
                //        field = "Sales Bill No",
                //        description = "Bill already cancelled.",
                //        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                //    };
                //    return null;
                //}
                if (ksm.cflag == "Y") {
                    error = new ErrorVM()
                    {
                        description = "SR Bill cancelled already",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }
            }

            //Checking whether its a credit bill or not to enable order in the UI
            #region Credit Bill Details
            isCreditBill = CheckIsCreditBillWithBalance(companyCode, branchCode, ksm.voture_bill_no, out masterBalaceAmt, out partialPaidAmt);
            #endregion

            //Checking this Estimation (SR Bill) Attached to Sales or Not
            if (!isEstimation) {
                //string srBillNo = Convert.ToString(estNo);
                //KTTU_PAYMENT_DETAILS isAttached = db.KTTU_PAYMENT_DETAILS.Where(e => e.Ref_BillNo == srBillNo && e.company_code == companyCode && e.branch_code == branchCode && e.trans_type == "S" && e.pay_mode == "SR").FirstOrDefault();
                //if (isAttached != null)
                //{
                //    error = new ErrorVM()
                //    {
                //        description = "SR Bill No: " + estNo + " is already adjusted towords Sales Bill No: " + isAttached.series_no + "",
                //        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                //    };
                //    return null;
                //}

                #region Validating SR its Adjusted or Not
                string srBill = Convert.ToString(ksm.sales_bill_no);
                KTTU_PAYMENT_DETAILS payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.company_code == companyCode
                                                                                && pay.branch_code == branchCode
                                                                                && pay.pay_mode == "SR"
                                                                                && pay.Ref_BillNo == srBill
                                                                                && pay.cflag == "N"
                                                                                && pay.pay_details != "S"
                                                                                && pay.sal_bill_type != "CR").FirstOrDefault();
                if (payment != null) {
                    string tranType = payment.trans_type == "O" ? "Order No: " + payment.series_no : "Bill No:" + payment.series_no;
                    error = new ErrorVM()
                    {
                        description = "SR Bill No: " + ksm.sales_bill_no + " already is adjusted towords " + tranType,
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }
                #endregion
            }

            SalesReturnMasterVM srmv = new SalesReturnMasterVM();
            srmv.ObjID = ksm.obj_id;
            srmv.CompanyCode = ksm.company_code;
            srmv.BranchCode = ksm.branch_code;
            srmv.VotureBillNo = ksm.voture_bill_no;
            srmv.SalesBillNo = ksm.sales_bill_no;
            srmv.EstNo = ksm.est_no;
            srmv.CustomerID = ksm.cust_id;
            srmv.BillDate = ksm.bill_date;
            srmv.SalesReturnDate = ksm.sr_date;
            srmv.GSType = ksm.gs_type;
            srmv.TaxAmount = ksm.tax_amt;
            srmv.TotalSRAmount = ksm.total_sramount;
            srmv.OperatorCode = ksm.operator_code;
            srmv.SalCode = ksm.sal_code;
            srmv.BillCounter = ksm.bill_counter;
            srmv.Remarks = ksm.remarks;
            srmv.UpdateOn = ksm.UpdateOn;
            srmv.ISIns = ksm.is_ins;
            srmv.Rate = ksm.rate;
            srmv.BilledBranch = ksm.billed_branch;
            srmv.CFlag = ksm.cflag;
            srmv.ExciseDutyPercent = ksm.excise_duty_percent;
            srmv.EDCessPercent = ksm.ed_cess_percent;
            srmv.HEDCessPercent = ksm.hed_cess_percent;
            srmv.ExciseDutyAmount = ksm.excise_duty_amount;
            srmv.EDCessAmount = ksm.ed_cess_amount;
            srmv.HEDCessAmount = ksm.hed_cess_amount;
            srmv.DiscountAmount = ksm.discount_amt;
            srmv.IsAdjusted = ksm.is_adjusted;
            srmv.CancelledBy = ksm.cancelled_by;
            srmv.CancelledRemarks = ksm.cancelled_remarks;
            srmv.NewBillNo = ksm.New_Bill_No;
            srmv.RoundOff = ksm.round_off;
            srmv.ShiftID = ksm.ShiftID;
            srmv.InvoiceTypeID = ksm.invoice_type_id;
            srmv.TotalCSTAmount = ksm.total_cst_amount;
            srmv.VPAmount = ksm.vp_amt;
            srmv.PayableAmount = ksm.payable_amt;
            srmv.CustomerName = ksm.cust_name;
            srmv.Address1 = ksm.address1;
            srmv.Address2 = ksm.address2;
            srmv.Address3 = ksm.address3;
            srmv.City = ksm.city;
            srmv.PinCode = ksm.pin_code;
            srmv.MobileNo = ksm.mobile_no;
            srmv.State = ksm.state;
            srmv.StateCode = ksm.state_code;
            srmv.TIN = ksm.tin;
            srmv.PANNo = ksm.pan_no;
            srmv.Type = "SR";
            srmv.IsCreditBill = isCreditBill;
            srmv.VPAmount = masterBalaceAmt;
            srmv.PartialPaidAmount = partialPaidAmt;
            List<SalesReturnDetailsVM> lstOfSRVM = new List<SalesReturnDetailsVM>();
            foreach (KTTU_SR_DETAILS ksd in lstOfksd) {
                SalesReturnDetailsVM srd = new SalesReturnDetailsVM();
                srd.ObjID = ksd.obj_id;
                srd.CompanyCode = ksd.company_code;
                srd.BranchCode = ksd.branch_code;
                srd.SalesBillNo = ksd.sales_bill_no;
                srd.SlNo = ksd.sl_no;
                srd.EstNo = ksd.est_no;
                srd.SalCode = ksd.sal_code;
                srd.ItemName = ksd.item_name;
                srd.CounterCode = ksd.counter_code;
                srd.Quantity = ksd.quantity;
                srd.GrossWt = ksd.gwt;
                srd.StoneWt = ksd.swt;
                srd.NetWt = ksd.nwt;
                srd.WastePercent = ksd.wast_percent;
                srd.MakingChargePerRs = ksd.making_charge_per_rs;
                srd.VAAmount = ksd.va_amount;
                srd.SRAmount = ksd.sr_amount;
                srd.StoneCharges = ksd.stone_charges;
                srd.DiamondCharges = ksd.diamond_charges;
                srd.NetAmount = ksd.net_amount;
                srd.AddQty = ksd.Addqty;
                srd.DeductQty = ksd.Deductqty;
                srd.AddWt = ksd.AddWt;
                srd.DeductWt = ksd.DeductWt;
                srd.UpdateOn = ksd.UpdateOn;
                srd.VAPecent = ksd.va_percent;
                srd.GSCode = ksd.gs_code;
                srd.FinYear = ksd.Fin_Year;
                srd.BarcodeNo = ksd.barcode_no;
                srd.SupplierCode = ksd.supplier_code;
                srd.ItemAdditionalDiscount = ksd.item_additional_discount;
                srd.ItemTotalAfterDiscount = ksd.item_additional_discount;
                srd.TaxPercentage = ksd.tax_percentage;
                srd.TaxAmount = ksd.tax_amount;
                srd.ItemSize = ksd.item_size;
                srd.ImageID = ksd.img_id;
                srd.DesignCode = ksd.design_code;
                srd.DesignName = ksd.design_name;
                srd.BatchID = ksd.batch_id;
                srd.RFID = ksd.rf_id;
                srd.MCPerPiece = ksd.mc_per_piece;
                srd.OfferValue = ksd.offer_value;
                srd.ItemFinalAmount = ksd.item_final_amount;
                srd.MCType = ksd.mc_type;
                srd.RoundOff = ksd.round_off;
                srd.ItemFinalAmountAfterRoundOff = ksd.item_final_amount_after_roundoff;
                srd.OriginalSalesBillNo = ksd.original_sales_bill_no;
                srd.ItemType = ksd.item_type;
                srd.CSTAmount = ksd.cst_amount;
                srd.Dcts = ksd.Dcts;
                srd.GSTGroupCode = ksd.GSTGroupCode;
                srd.SGSTPercent = ksd.SGST_Percent;
                srd.SGSTAmount = ksd.SGST_Amount;
                srd.CGSTPercent = ksd.CGST_Percent;
                srd.CGSTAmount = ksd.CGST_Amount;
                srd.IGSTPercent = ksd.IGST_Percent;
                srd.IGSTAmount = ksd.IGST_Amount;
                srd.HSN = ksd.HSN;
                srd.DiscountMc = ksd.Discount_Mc;
                srd.McDiscountAmt = ksd.Mc_Discount_Amt;

                List<SalesReturnStoneDetailsVM> lstOfSRSD = new List<SalesReturnStoneDetailsVM>();
                List<KTTU_SR_STONE_DETAILS> lstOfkssd = db.KTTU_SR_STONE_DETAILS.Where(e => e.est_no == estNo
                                                                                        && e.barcode_no == srd.BarcodeNo
                                                                                        && e.company_code == companyCode
                                                                                        && e.branch_code == branchCode).ToList();
                foreach (KTTU_SR_STONE_DETAILS kssd in lstOfkssd) {
                    SalesReturnStoneDetailsVM srsd = new SalesReturnStoneDetailsVM();
                    srsd.ObjID = kssd.obj_id;
                    srsd.CompanyCode = kssd.company_code;
                    srsd.BranchCode = kssd.branch_code;
                    srsd.SalesBillNo = kssd.sales_bill_no;
                    srsd.EstNo = kssd.est_no;
                    srsd.ItemSno = kssd.item_sno;
                    srsd.Sno = kssd.sno;
                    srsd.Type = kssd.type;
                    srsd.Name = kssd.name;
                    srsd.Qty = kssd.qty;
                    srsd.Carrat = kssd.carrat;
                    srsd.StoneWt = kssd.stone_wt;
                    srsd.Rate = kssd.rate;
                    srsd.Amount = kssd.amount;
                    srsd.UpdateOn = kssd.UpdateOn;
                    srsd.FinYear = kssd.Fin_Year;
                    srsd.BarcodeNo = kssd.barcode_no;
                    lstOfSRSD.Add(srsd);
                }
                srd.lstOfStoneDetails = lstOfSRSD;
                lstOfSRVM.Add(srd);
            }

            #region Payment Details
            if (kpd != null) {
                List<PaymentVM> lstOfPayment = new List<PaymentVM>();
                foreach (KTTU_PAYMENT_DETAILS paymentDet in kpd) {
                    PaymentVM payment = new PaymentVM();
                    payment.ObjID = paymentDet.obj_id;
                    payment.CompanyCode = paymentDet.company_code;
                    payment.BranchCode = paymentDet.branch_code;
                    payment.SeriesNo = paymentDet.series_no;
                    payment.ReceiptNo = paymentDet.receipt_no;
                    payment.SNo = paymentDet.sno;
                    payment.TransType = paymentDet.trans_type;
                    payment.PayMode = paymentDet.pay_mode;
                    payment.PayDetails = paymentDet.pay_details;
                    payment.PayDate = paymentDet.pay_date;
                    payment.PayAmount = paymentDet.pay_amt;
                    payment.RefBillNo = paymentDet.Ref_BillNo;
                    payment.PartyCode = paymentDet.party_code;
                    payment.BillCounter = paymentDet.bill_counter;
                    payment.IsPaid = paymentDet.is_paid;
                    payment.Bank = paymentDet.bank;
                    payment.BankName = paymentDet.bank_name;
                    payment.ChequeDate = paymentDet.cheque_date;
                    payment.CardType = paymentDet.card_type;
                    payment.ExpiryDate = paymentDet.expiry_date;
                    payment.CFlag = paymentDet.cflag;
                    payment.CardAppNo = paymentDet.card_app_no;
                    payment.SchemeCode = paymentDet.scheme_code;
                    payment.SalBillType = paymentDet.sal_bill_type;
                    payment.OperatorCode = paymentDet.operator_code;
                    payment.SessionNo = paymentDet.session_no;
                    payment.UpdateOn = paymentDet.UpdateOn;
                    payment.GroupCode = paymentDet.group_code;
                    payment.AmtReceived = paymentDet.amt_received;
                    payment.BonusAmt = paymentDet.bonus_amt;
                    payment.WinAmt = paymentDet.win_amt;
                    payment.CTBranch = paymentDet.ct_branch;
                    payment.FinYear = paymentDet.fin_year;
                    payment.CardCharges = paymentDet.CardCharges;
                    payment.ChequeNo = paymentDet.cheque_no;
                    payment.NewBillNo = paymentDet.new_receipt_no;
                    payment.AddDisc = paymentDet.Add_disc;
                    payment.IsOrderManual = paymentDet.isOrdermanual;
                    payment.CurrencyValue = paymentDet.currency_value;
                    payment.ExchangeRate = paymentDet.exchange_rate;
                    payment.CurrencyType = paymentDet.currency_type;
                    payment.TaxPercentage = paymentDet.tax_percentage;
                    payment.CancelledBy = paymentDet.cancelled_by;
                    payment.CancelledRemarks = paymentDet.cancelled_remarks;
                    payment.CancelledDate = paymentDet.cancelled_date;
                    payment.IsExchange = paymentDet.isExchange;
                    payment.ExchangeNo = paymentDet.exchangeNo;
                    payment.NewReceiptNo = paymentDet.new_receipt_no;
                    payment.GiftAmount = paymentDet.Gift_Amount;
                    payment.CardSwipedBy = paymentDet.cardSwipedBy;
                    payment.Version = paymentDet.version;
                    payment.GSTGroupCode = paymentDet.GSTGroupCode;
                    payment.SGSTPercent = paymentDet.SGST_Percent;
                    payment.CGSTPercent = paymentDet.CGST_Percent;
                    payment.IGSTPercent = paymentDet.IGST_Percent;
                    payment.HSN = paymentDet.HSN;
                    payment.SGSTAmount = paymentDet.SGST_Amount;
                    payment.CGSTAmount = paymentDet.CGST_Amount;
                    payment.IGSTAmount = paymentDet.IGST_Amount;
                    payment.PayAmountBeforeTax = paymentDet.pay_amount_before_tax;
                    payment.PayTaxAmount = paymentDet.pay_tax_amount;
                    lstOfPayment.Add(payment);
                }
                srmv.lstOfPayment = lstOfPayment;
            }
            #endregion

            srmv.lstOfSalesReturnDetails = lstOfSRVM;
            return srmv;
        }

        public ConfirmVM ConfirmSalesReturnWithTxn(ConfirmSalesReturnVM confirmSales, out ErrorVM error)
        {
            error = null;
            bool isOwnTransaction = false;
            if (db.Database.CurrentTransaction == null) {
                db.Database.BeginTransaction();
                isOwnTransaction = true;
            }

            ConfirmVM confirm = ConfirmSalesReturn(confirmSales, out error);
            if (confirm == null) {
                if (isOwnTransaction)
                    db.Database.CurrentTransaction.Rollback();
            }
            else if (confirm.SalesBillNo == 0) {
                if (isOwnTransaction)
                    db.Database.CurrentTransaction.Rollback();
            }
            else {
                if (isOwnTransaction)
                    db.Database.CurrentTransaction.Commit();
            }
            return confirm;
        }

        private ConfirmVM ConfirmSalesReturn(ConfirmSalesReturnVM confirmSales, out ErrorVM error)
        {
            error = null;
            int originalBillNo = 0;
            decimal totalPayAmount = 0;

            ConfirmVM confirm = new ConfirmVM();
            int finYear = db.KSTU_ACC_FY_MASTER.Where(f => f.company_code == confirmSales.CompanyCode
                                                       && f.branch_code == confirmSales.BranchCode).FirstOrDefault().fin_year;
            try {

                #region validation
                // Payment Validation According to Masters.
                if (confirmSales.Type == "SR") {
                    if (confirmSales.lstOfPayment != null && confirmSales.lstOfPayment.Count > 0) {
                        foreach (PaymentVM pvm in confirmSales.lstOfPayment) {
                            if (pvm.PayMode == "C") {
                                KSTU_TOLERANCE_MASTER ktm = db.KSTU_TOLERANCE_MASTER.Where(kt => kt.obj_id == 6
                                                                                            && kt.company_code == confirmSales.CompanyCode
                                                                                            && kt.branch_code == confirmSales.BranchCode).FirstOrDefault();
                                if (pvm.PayAmount > ktm.Max_Val) {
                                    error = new ErrorVM()
                                    {
                                        field = "Amount",
                                        index = 0,
                                        description = "Cash should not be more than " + ktm.Max_Val + "/-",
                                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                                    };
                                    return null;
                                }
                            }
                            totalPayAmount = totalPayAmount + Convert.ToDecimal(pvm.PayAmount);
                        }
                    }
                }
                #endregion

                #region Delete
                KTTU_SR_MASTER ksmOriginal = db.KTTU_SR_MASTER.Where(km => km.est_no == confirmSales.EstNo
                                                                    && km.company_code == confirmSales.CompanyCode
                                                                    && km.branch_code == confirmSales.BranchCode).FirstOrDefault();
                originalBillNo = ksmOriginal.voture_bill_no;
                if (ksmOriginal.sales_bill_no != 0) {
                    error = new ErrorVM()
                    {
                        field = "SR Estimation Number",
                        description = "SR Estimation Not Found.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }

                // We are checking here already done any Confirm SR to that sales bill using another estimation.
                List<KTTU_SR_MASTER> srMaster = db.KTTU_SR_MASTER.Where(km => km.est_no == originalBillNo
                                                                        && km.company_code == confirmSales.CompanyCode
                                                                        && km.branch_code == confirmSales.BranchCode).ToList();
                if (srMaster != null) {
                    foreach (KTTU_SR_MASTER sr in srMaster) {
                        if (sr.sales_bill_no != 0) {
                            error = new ErrorVM()
                            {
                                field = "",
                                description = "This sales bill already Cancelled, With estimation no " + sr.est_no + "",
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                            };
                            return null;
                        }
                    }
                }

                db.KTTU_SR_MASTER.Remove(ksmOriginal);

                List<KTTU_SR_DETAILS> ksdOriginal = db.KTTU_SR_DETAILS.Where(kd => kd.est_no == confirmSales.EstNo
                                                                    && kd.company_code == confirmSales.CompanyCode
                                                                    && kd.branch_code == confirmSales.BranchCode).ToList();
                foreach (KTTU_SR_DETAILS ksdb in ksdOriginal) {
                    db.KTTU_SR_DETAILS.Remove(ksdb);
                }

                List<KTTU_SR_STONE_DETAILS> kssdOriginal = db.KTTU_SR_STONE_DETAILS.Where(kd => kd.est_no == confirmSales.EstNo
                                                                    && kd.company_code == confirmSales.CompanyCode
                                                                    && kd.branch_code == confirmSales.BranchCode).ToList();
                foreach (KTTU_SR_STONE_DETAILS kd in kssdOriginal) {
                    db.KTTU_SR_STONE_DETAILS.Remove(kd);
                }
                #endregion

                #region Save SR Master
                DateTime appDate = SIGlobals.Globals.GetApplicationDate(confirmSales.CompanyCode, confirmSales.BranchCode);
                int salesReturnBillNo = Convert.ToInt32(db.KSTU_ACC_FY_MASTER.Where(f => f.company_code == confirmSales.CompanyCode
                                                        && f.branch_code == confirmSales.BranchCode).FirstOrDefault().fin_year.ToString().Remove(0, 1) +
                                                       db.KSTS_SEQ_NOS.Where(sq => sq.obj_id == MODULE_SEQ_NO
                                                                            && sq.company_code == confirmSales.CompanyCode
                                                                            && sq.branch_code == confirmSales.BranchCode).FirstOrDefault().nextno);
                string objectID = SIGlobals.Globals.GetMagnaGUID(TABLE_NAME, salesReturnBillNo, confirmSales.CompanyCode, confirmSales.BranchCode);
                KTTU_SR_MASTER ksm = new KTTU_SR_MASTER();
                ksm.obj_id = objectID;
                ksm.company_code = ksmOriginal.company_code;
                ksm.branch_code = ksmOriginal.branch_code;
                ksm.voture_bill_no = ksmOriginal.voture_bill_no;
                ksm.sales_bill_no = salesReturnBillNo;
                ksm.est_no = ksmOriginal.est_no;
                ksm.cust_id = ksmOriginal.cust_id;
                ksm.bill_date = ksmOriginal.bill_date;
                ksm.sr_date = ksmOriginal.sr_date;
                ksm.gs_type = ksmOriginal.gs_type;
                ksm.tax_amt = ksmOriginal.tax_amt;
                ksm.total_sramount = ksmOriginal.total_sramount;
                ksm.operator_code = ksmOriginal.operator_code;
                ksm.sal_code = ksmOriginal.sal_code;
                ksm.bill_counter = ksmOriginal.bill_counter;
                ksm.remarks = ksmOriginal.remarks;
                ksm.UpdateOn = ksmOriginal.UpdateOn;
                ksm.is_ins = ksmOriginal.is_ins;
                ksm.rate = ksmOriginal.rate;
                ksm.billed_branch = ksmOriginal.billed_branch;
                ksm.cflag = ksmOriginal.cflag;
                ksm.excise_duty_percent = ksmOriginal.excise_duty_percent;
                ksm.ed_cess_percent = ksmOriginal.ed_cess_percent;
                ksm.hed_cess_percent = ksmOriginal.hed_cess_percent;
                ksm.excise_duty_amount = ksmOriginal.excise_duty_amount;
                ksm.ed_cess_amount = ksmOriginal.ed_cess_amount;
                ksm.hed_cess_amount = ksmOriginal.hed_cess_amount;
                ksm.discount_amt = ksmOriginal.discount_amt;
                ksm.is_adjusted = ksmOriginal.is_adjusted;
                ksm.cancelled_by = ksmOriginal.cancelled_by;
                ksm.cancelled_remarks = ksmOriginal.cancelled_remarks;
                ksm.New_Bill_No = ksmOriginal.New_Bill_No;
                ksm.round_off = ksmOriginal.round_off;
                ksm.ShiftID = ksmOriginal.ShiftID;
                ksm.invoice_type_id = ksmOriginal.invoice_type_id;
                ksm.total_cst_amount = ksmOriginal.total_cst_amount;
                ksm.vp_amt = ksmOriginal.vp_amt;
                ksm.payable_amt = ksmOriginal.payable_amt;
                ksm.cust_name = ksmOriginal.cust_name;
                ksm.address1 = ksmOriginal.address1;
                ksm.address2 = ksmOriginal.address2;
                ksm.address3 = ksmOriginal.address3;
                ksm.city = ksmOriginal.city;
                ksm.pin_code = ksmOriginal.pin_code;
                ksm.mobile_no = ksmOriginal.mobile_no;
                ksm.state = ksmOriginal.state;
                ksm.state_code = ksmOriginal.state_code;
                ksm.tin = ksmOriginal.tin;
                ksm.pan_no = ksmOriginal.pan_no;
                ksm.UniqRowID = Guid.NewGuid();
                ksm.store_location_id = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == confirmSales.CompanyCode
                                                                                && c.branch_code == confirmSales.BranchCode).FirstOrDefault().store_location_id;
                db.KTTU_SR_MASTER.Add(ksm);

                #endregion

                #region SR Details
                foreach (KTTU_SR_DETAILS srd in ksdOriginal) {
                    KTTU_SR_DETAILS ksd = new KTTU_SR_DETAILS();
                    ksd.obj_id = objectID;
                    ksd.company_code = srd.company_code;
                    ksd.branch_code = srd.branch_code;
                    ksd.sales_bill_no = ksm.sales_bill_no;
                    ksd.sl_no = srd.sl_no;
                    ksd.est_no = ksm.est_no;
                    ksd.sal_code = srd.sal_code;
                    ksd.item_name = srd.item_name;
                    ksd.counter_code = srd.counter_code;
                    ksd.quantity = srd.quantity;
                    ksd.gwt = srd.gwt;
                    ksd.swt = srd.swt;
                    ksd.nwt = srd.nwt;
                    ksd.wast_percent = srd.wast_percent;
                    ksd.making_charge_per_rs = srd.making_charge_per_rs;
                    ksd.va_amount = srd.va_amount;
                    ksd.sr_amount = srd.sr_amount;
                    ksd.stone_charges = srd.stone_charges;
                    ksd.diamond_charges = srd.diamond_charges;
                    ksd.net_amount = srd.net_amount;
                    ksd.Addqty = srd.Addqty;
                    ksd.Deductqty = srd.Deductqty;
                    ksd.AddWt = srd.AddWt;
                    ksd.DeductWt = srd.DeductWt;
                    ksd.UpdateOn = SIGlobals.Globals.GetDateTime();
                    ksd.va_percent = srd.va_percent;
                    ksd.gs_code = srd.gs_code;
                    ksd.Fin_Year = srd.Fin_Year;
                    ksd.barcode_no = srd.barcode_no;
                    ksd.supplier_code = srd.supplier_code;
                    ksd.item_additional_discount = srd.item_additional_discount;
                    ksd.item_total_after_discount = srd.item_total_after_discount;
                    ksd.tax_percentage = srd.tax_percentage;
                    ksd.tax_amount = srd.tax_amount;
                    ksd.item_size = srd.item_size;
                    ksd.img_id = srd.img_id;
                    ksd.design_code = srd.design_code;
                    ksd.design_name = srd.design_name;
                    ksd.batch_id = srd.batch_id;
                    ksd.rf_id = srd.rf_id;
                    ksd.mc_per_piece = srd.mc_per_piece;
                    ksd.offer_value = srd.offer_value;
                    ksd.item_final_amount = srd.item_final_amount;
                    ksd.mc_type = srd.mc_type;
                    ksd.round_off = srd.round_off;
                    ksd.item_final_amount_after_roundoff = srd.item_final_amount_after_roundoff;
                    ksd.original_sales_bill_no = srd.original_sales_bill_no;
                    ksd.item_type = srd.item_type;
                    ksd.cst_amount = srd.cst_amount;
                    ksd.Dcts = srd.Dcts;
                    ksd.GSTGroupCode = srd.GSTGroupCode;
                    ksd.SGST_Percent = srd.SGST_Percent;
                    ksd.SGST_Amount = srd.SGST_Amount;
                    ksd.CGST_Percent = srd.CGST_Percent;
                    ksd.CGST_Amount = srd.CGST_Amount;
                    ksd.IGST_Percent = srd.IGST_Percent;
                    ksd.IGST_Amount = srd.IGST_Amount;
                    ksd.HSN = srd.HSN;
                    ksd.Discount_Mc = srd.Discount_Mc;
                    ksd.Mc_Discount_Amt = srd.Mc_Discount_Amt;
                    ksd.UniqRowID = Guid.NewGuid();
                    ksd.CESSAmount = Convert.ToDecimal(srd.CESSAmount);
                    ksd.CESSPercent = Convert.ToDecimal(srd.CESSPercent);
                    ksd.isBarcoded = srd.isBarcoded;
                    db.KTTU_SR_DETAILS.Add(ksd);

                    // Counter Stock Update
                    bool couterStockUpdated = CounterStockUpdate(ksd.gs_code, ksd.item_name, ksd.counter_code,
                                                                 ksd.quantity, ksd.gwt, ksd.nwt, ksd.swt,
                                                                Convert.ToDecimal(ksd.AddWt), Convert.ToDecimal(ksd.DeductWt),
                                                                Convert.ToInt32(ksd.Addqty), Convert.ToInt32(ksd.Deductqty), ksd.company_code, ksd.branch_code);
                    if (!couterStockUpdated) {
                        error = new ErrorVM()
                        {
                            description = "Failed in Counter Stock Update",
                            field = "Stock Update",
                            ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError
                        };
                        return null;
                    }

                    //Stock Update
                    bool stockUpdate = StockUpdate(ksd.gs_code, ksd.quantity, ksd.gwt, ksd.nwt, ksd.swt,
                       Convert.ToDecimal(ksd.AddWt), Convert.ToDecimal(ksd.DeductWt), Convert.ToInt32(ksd.Addqty), Convert.ToInt32(ksd.Deductqty), ksd.company_code, ksd.branch_code);
                    if (!stockUpdate) {
                        error = new ErrorVM()
                        {
                            description = "Failed in Stock Update",
                            field = "Stock Update",
                            ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError
                        };
                        return null;
                    }

                    #region SR Stone
                    int SlNo = 1;
                    foreach (KTTU_SR_STONE_DETAILS srsd in kssdOriginal.Where(k => k.item_sno == ksd.sl_no
                                                                              && k.company_code == confirmSales.CompanyCode
                                                                              && k.branch_code == confirmSales.BranchCode).ToList()) {
                        KTTU_SR_STONE_DETAILS kssd = new KTTU_SR_STONE_DETAILS();
                        kssd.obj_id = objectID;
                        kssd.company_code = srsd.company_code;
                        kssd.branch_code = srsd.branch_code;
                        kssd.sales_bill_no = ksm.sales_bill_no;
                        kssd.est_no = ksm.est_no;
                        kssd.item_sno = srsd.item_sno;
                        kssd.sno = SlNo;
                        kssd.type = srsd.type;
                        kssd.name = srsd.name;
                        kssd.qty = srsd.qty;
                        kssd.carrat = srsd.carrat;
                        kssd.stone_wt = srsd.stone_wt;
                        kssd.rate = srsd.rate;
                        kssd.amount = srsd.amount;
                        kssd.UpdateOn = SIGlobals.Globals.GetDateTime();
                        kssd.Fin_Year = srsd.Fin_Year;
                        kssd.barcode_no = srsd.barcode_no;
                        kssd.UniqRowID = Guid.NewGuid();
                        db.KTTU_SR_STONE_DETAILS.Add(kssd);
                        SlNo = SlNo + 1;
                    }
                    #endregion
                }
                #endregion

                #region Payment Information 
                int paySlNo = 1;
                string payGUID = objectID;
                if (confirmSales.Type == "SE") {
                    KTTU_PAYMENT_DETAILS kpd = new KTTU_PAYMENT_DETAILS();
                    kpd.obj_id = confirmSales.ObjectID;
                    kpd.company_code = confirmSales.CompanyCode;
                    kpd.branch_code = confirmSales.BranchCode;
                    kpd.series_no = confirmSales.BillNo;
                    kpd.receipt_no = 0;
                    kpd.sno = confirmSales.SlNo;
                    kpd.trans_type = "S";
                    kpd.pay_mode = "SR";
                    kpd.pay_details = "SR Bill";
                    kpd.pay_date = SIGlobals.Globals.GetDateTime();
                    kpd.pay_amt = confirmSales.PaidAmount;
                    kpd.Ref_BillNo = Convert.ToString(salesReturnBillNo);
                    kpd.party_code = null;
                    kpd.bill_counter = confirmSales.BillCounter;
                    kpd.is_paid = "N";
                    kpd.bank = null;
                    kpd.bank_name = null;
                    kpd.cheque_date = appDate;
                    kpd.card_type = null;
                    kpd.expiry_date = appDate;
                    kpd.cflag = "N";
                    kpd.card_app_no = null;
                    kpd.scheme_code = null;
                    kpd.sal_bill_type = null;
                    kpd.operator_code = confirmSales.OperatorCode;
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
                    kpd.New_Bill_No = Convert.ToString(salesReturnBillNo);
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
                    kpd.pay_amount_before_tax = 0;
                    kpd.pay_tax_amount = 0;
                    kpd.UniqRowID = Guid.NewGuid();
                    if (kpd.pay_mode == "CT") {
                        int memebershipNo = Convert.ToInt32(kpd.Ref_BillNo);
                        CHTU_CHIT_CLOSURE cc = db.CHTU_CHIT_CLOSURE.Where(ccl => ccl.Scheme_Code == kpd.scheme_code
                                                                            && ccl.Group_Code == kpd.group_code
                                                                            && ccl.branch_code == kpd.ct_branch
                                                                            && ccl.Chit_MembShipNo == memebershipNo
                                                                            && ccl.company_code == confirmSales.CompanyCode
                                                                            && ccl.branch_code == confirmSales.BranchCode).FirstOrDefault();
                        kpd.amt_received = cc.Amt_Received;
                        kpd.bonus_amt = cc.Bonus_Amt;
                        kpd.win_amt = cc.win_amt;
                        cc.Bill_Number = Convert.ToString(salesReturnBillNo);
                        db.Entry(cc).State = System.Data.Entity.EntityState.Modified;
                    }
                    db.KTTU_PAYMENT_DETAILS.Add(kpd);
                    paySlNo = paySlNo + 1;
                }
                else if (confirmSales.Type == "SR") {
                    if (confirmSales.lstOfPayment.Count > 0) {
                        foreach (PaymentVM pvm in confirmSales.lstOfPayment) {
                            KTTU_PAYMENT_DETAILS kpd = new KTTU_PAYMENT_DETAILS();
                            kpd.obj_id = payGUID;
                            kpd.company_code = pvm.CompanyCode;
                            kpd.branch_code = pvm.BranchCode;
                            kpd.series_no = ksm.sales_bill_no;
                            kpd.receipt_no = 0;
                            kpd.sno = paySlNo;
                            kpd.trans_type = "RS";
                            kpd.pay_mode = pvm.PayMode;
                            kpd.pay_details = pvm.PayDetails;
                            kpd.pay_date = appDate;
                            kpd.pay_amt = pvm.PayAmount;
                            kpd.Ref_BillNo = pvm.RefBillNo == null ? "" : pvm.RefBillNo;
                            kpd.party_code = pvm.PartyCode == null ? "" : pvm.PartyCode;
                            kpd.bill_counter = pvm.BillCounter == null ? "" : "FF";
                            kpd.is_paid = pvm.IsPaid == null ? "N" : pvm.IsPaid;
                            kpd.bank = SIGlobals.Globals.GetBank(db, pvm.Bank, pvm.PayMode, pvm.CompanyCode, pvm.BranchCode);
                            kpd.bank_name = pvm.BankName;
                            kpd.cheque_date = pvm.ChequeDate == null ? appDate : pvm.ChequeDate;
                            kpd.card_type = pvm.CardType == null ? "" : pvm.CardType;
                            kpd.expiry_date = pvm.ExpiryDate == null ? appDate : pvm.ExpiryDate;
                            kpd.cflag = pvm.CFlag == null ? "N" : pvm.CFlag;
                            kpd.card_app_no = pvm.CardAppNo == null ? "" : pvm.CardAppNo;
                            kpd.scheme_code = pvm.SchemeCode == null ? "" : pvm.SchemeCode;
                            kpd.sal_bill_type = pvm.SalBillType;
                            kpd.operator_code = pvm.OperatorCode;
                            kpd.session_no = pvm.SessionNo == null ? 0 : pvm.SeriesNo;
                            kpd.UpdateOn = SIGlobals.Globals.GetDateTime();
                            kpd.group_code = pvm.GroupCode;
                            kpd.amt_received = Convert.ToDecimal(pvm.AmtReceived);
                            kpd.bonus_amt = Convert.ToDecimal(pvm.BonusAmt);
                            kpd.win_amt = Convert.ToDecimal(pvm.WinAmt);
                            kpd.ct_branch = pvm.CTBranch;
                            kpd.fin_year = finYear;
                            kpd.CardCharges = Convert.ToDecimal(pvm.CardCharges);
                            kpd.cheque_no = pvm.PayMode == "R" ? pvm.Bank : pvm.ChequeNo;
                            kpd.New_Bill_No = pvm.NewBillNo;
                            kpd.Add_disc = Convert.ToDecimal(pvm.AddDisc);
                            kpd.isOrdermanual = pvm.IsOrderManual == null ? "N" : pvm.IsOrderManual;
                            kpd.currency_value = Convert.ToDecimal(pvm.CurrencyValue);
                            kpd.exchange_rate = Convert.ToDecimal(pvm.ExchangeRate);
                            kpd.currency_type = pvm.CurrencyType;
                            kpd.tax_percentage = Convert.ToDecimal(pvm.TaxPercentage);
                            kpd.cancelled_by = pvm.CancelledBy == null ? "" : pvm.CancelledBy;
                            kpd.cancelled_remarks = pvm.CancelledRemarks == null ? "" : pvm.CancelledRemarks;
                            kpd.cancelled_date = pvm.CancelledDate == null ? Convert.ToString(appDate) : pvm.CancelledDate;
                            kpd.isExchange = pvm.IsExchange == null ? "N" : pvm.IsExchange;
                            kpd.exchangeNo = Convert.ToInt32(pvm.ExchangeNo);
                            kpd.new_receipt_no = pvm.NewReceiptNo;
                            kpd.Gift_Amount = Convert.ToDecimal(pvm.GiftAmount);
                            kpd.cardSwipedBy = pvm.CardSwipedBy == null ? "" : pvm.CardSwipedBy;
                            kpd.version = Convert.ToInt32(pvm.Version);
                            kpd.GSTGroupCode = pvm.GSTGroupCode;
                            kpd.SGST_Percent = Convert.ToInt32(pvm.SGSTPercent);
                            kpd.CGST_Percent = Convert.ToInt32(pvm.CGSTPercent);
                            kpd.IGST_Percent = Convert.ToInt32(pvm.IGSTPercent);
                            kpd.pay_amount_before_tax = pvm.PayAmountBeforeTax;
                            kpd.pay_tax_amount = pvm.PayTaxAmount;
                            kpd.UniqRowID = Guid.NewGuid();
                            kpd.cash_marked_Counter = pvm.CashMarkedCounter == null ? "" : pvm.CashMarkedCounter;
                            db.KTTU_PAYMENT_DETAILS.Add(kpd);
                            paySlNo = paySlNo + 1;
                        }
                    }
                    else if (confirmSales.IsCreditBill && confirmSales.IsOrder) {
                        // If The Payable Amount is there then we are creating order.
                        #region Generate Order
                        int customerID = db.KTTU_SR_MASTER.Where(sr => sr.est_no == confirmSales.EstNo
                                                                    && sr.company_code == confirmSales.CompanyCode
                                                                    && sr.branch_code == sr.branch_code).FirstOrDefault().cust_id;
                        KSTU_CUSTOMER_MASTER customer = db.KSTU_CUSTOMER_MASTER.Where(cust => cust.company_code == confirmSales.CompanyCode
                                                                    && cust.branch_code == confirmSales.BranchCode
                                                                    && cust.cust_id == customerID).FirstOrDefault();
                        OrderMasterVM order = new OrderMasterVM();
                        order.CompanyCode = confirmSales.CompanyCode;
                        order.BranchCode = confirmSales.BranchCode;
                        order.CustID = customerID;
                        order.MobileNo = customer.mobile_no;
                        order.OrderType = "A";
                        order.Remarks = "Order Created Through SR Bill Balance";
                        order.OperatorCode = confirmSales.OperatorCode;
                        order.ManagerCode = confirmSales.OperatorCode;
                        order.SalCode = confirmSales.OperatorCode;
                        order.DeliveryDate = appDate.AddMonths(1);
                        order.OrderRateType = "Delivery";
                        order.gsCode = "NGO";
                        order.AdvacnceOrderAmount = confirmSales.PartialPaidAmount;
                        order.Rate = 0;
                        order.Karat = "22K";
                        order.OrderDayRate = 0;

                        order.lstOfOrderItemDetailsVM = new List<OrderItemDetailsVM>();
                        order.lstOfOrderItemDetailsVM.Add(new OrderItemDetailsVM()
                        {
                            CompanyCode = confirmSales.CompanyCode,
                            BranchCode = confirmSales.BranchCode,
                            ItemName = "Advance Amount",
                            Description = "Advance Amount",
                            GSCode = "NGO"
                        });

                        order.lstOfPayment = new List<PaymentVM>();
                        order.lstOfPayment.Add(new PaymentVM()
                        {
                            CompanyCode = confirmSales.CompanyCode,
                            BranchCode = confirmSales.BranchCode,
                            PayMode = "C",
                            PayDetails = "Cash",
                            PayAmount = confirmSales.PartialPaidAmount,
                            RefBillNo = Convert.ToString(ksm.sales_bill_no)
                        });

                        int genOrderNo = 0;
                        int genReceiptNo = 0;
                        ErrorVM orderErrorVM = new ErrorVM();
                        OrderMasterVM newOrder = new OrderBL().SaveOrderInfoWithTran(order, out genOrderNo, out genReceiptNo, out orderErrorVM);
                        if (orderErrorVM != null) {
                            error = new ErrorVM()
                            {
                                description = orderErrorVM.description + orderErrorVM.customDescription,
                                field = "Stock Update",
                                ErrorStatusCode = orderErrorVM.ErrorStatusCode
                            };
                            return null;
                        }
                        confirm.OrderNo = genReceiptNo;
                        confirm.ReceiptNo = genReceiptNo;
                        #endregion

                        #region Generate Credit Receipt


                        int creditReceiptBillNo = Convert.ToInt32(db.KSTU_ACC_FY_MASTER.Where(fy => fy.company_code == confirmSales.CompanyCode
                                                                         && fy.branch_code == confirmSales.BranchCode).FirstOrDefault().fin_year.ToString().Remove(0, 1) +
                                                                         db.KSTS_SEQ_NOS.Where(
                                                                         rs => rs.company_code == confirmSales.CompanyCode
                                                                         && rs.branch_code == confirmSales.BranchCode
                                                                         && rs.obj_id == "24").FirstOrDefault().nextno);

                        KTTU_CREDIT_BILL_DETAILS kcbd = new KTTU_CREDIT_BILL_DETAILS();
                        kcbd.Obj_Id = SIGlobals.Globals.GetNewGUID();
                        kcbd.company_code = ksm.company_code;
                        kcbd.branch_code = ksm.branch_code;
                        kcbd.amount_paid = confirmSales.VPAmount;
                        kcbd.balance_amount = 0; // ToDo
                        kcbd.bill_amount = 0;
                        kcbd.bill_no = originalBillNo;
                        kcbd.credit_bill_receiptNo = creditReceiptBillNo;
                        kcbd.Fin_Year = finYear;
                        kcbd.garunteename = ksm.operator_code;
                        kcbd.Ispaid = "Y";
                        kcbd.New_Bill_No = ksm.New_Bill_No;
                        kcbd.Paid_date = appDate;
                        kcbd.UpdateOn = SIGlobals.Globals.GetDateTime();
                        kcbd.UniqRowID = Guid.NewGuid();
                        db.KTTU_CREDIT_BILL_DETAILS.Add(kcbd);

                        KTTU_PAYMENT_DETAILS kpd = new KTTU_PAYMENT_DETAILS();
                        kpd.obj_id = payGUID;
                        kpd.company_code = confirmSales.CompanyCode;
                        kpd.branch_code = confirmSales.BranchCode;
                        kpd.series_no = originalBillNo;
                        kpd.receipt_no = creditReceiptBillNo;
                        kpd.sno = 1;
                        kpd.trans_type = "S";
                        kpd.pay_mode = "SR";
                        kpd.pay_details = "CRbilltrwSR";
                        kpd.pay_date = appDate;
                        kpd.pay_amt = confirmSales.VPAmount;
                        kpd.Ref_BillNo = Convert.ToString(ksm.sales_bill_no);
                        kpd.party_code = null;
                        kpd.bill_counter = "FF";
                        kpd.is_paid = "N";
                        kpd.bank = "";
                        kpd.bank_name = null;
                        kpd.cheque_date = appDate;
                        kpd.card_type = null;
                        kpd.expiry_date = appDate;
                        kpd.cflag = "N";
                        kpd.card_app_no = null;
                        kpd.scheme_code = null;
                        kpd.sal_bill_type = "CR";
                        kpd.operator_code = confirmSales.OperatorCode;
                        kpd.session_no = 0;
                        kpd.UpdateOn = SIGlobals.Globals.GetDateTime();
                        kpd.group_code = null;
                        kpd.amt_received = 0;
                        kpd.bonus_amt = 0;
                        kpd.win_amt = 0;
                        kpd.ct_branch = null;
                        kpd.fin_year = finYear;
                        kpd.CardCharges = 0;
                        kpd.cheque_no = null;
                        kpd.New_Bill_No = Convert.ToString(originalBillNo);
                        kpd.Add_disc = 0;
                        kpd.isOrdermanual = "N";
                        kpd.currency_value = 0;
                        kpd.exchange_rate = 0;
                        kpd.currency_type = null;
                        kpd.tax_percentage = 0;
                        kpd.cancelled_by = "";
                        kpd.cancelled_remarks = "";
                        kpd.cancelled_date = Convert.ToString(appDate);
                        kpd.isExchange = "N";
                        kpd.exchangeNo = 0;
                        kpd.new_receipt_no = Convert.ToString(creditReceiptBillNo);
                        kpd.Gift_Amount = 0;
                        kpd.cardSwipedBy = "";
                        kpd.version = 0;
                        kpd.GSTGroupCode = null;
                        kpd.SGST_Percent = 0;
                        kpd.CGST_Percent = 0;
                        kpd.IGST_Percent = 0;
                        kpd.pay_amount_before_tax = confirmSales.VPAmount;
                        kpd.pay_tax_amount = 0;
                        kpd.UniqRowID = Guid.NewGuid();
                        kpd.cash_marked_Counter = "";
                        db.KTTU_PAYMENT_DETAILS.Add(kpd);
                        confirm.CreditReceiptNo = creditReceiptBillNo;
                        //Updating Credit Receipt Serial No
                        SIGlobals.Globals.UpdateSeqenceNumber(db, "24", confirmSales.CompanyCode, confirmSales.BranchCode);
                        #endregion
                    }
                    else if (confirmSales.IsCreditBill && confirmSales.IsAdjust && confirmSales.IsPayable) {
                        // If it is a Credit Bill then payable amount is negetive then we generate Credit receipt for that Total SR Amount.
                        #region Generate Credit Receipt
                        int creditReceiptBillNo = Convert.ToInt32(db.KSTU_ACC_FY_MASTER.Where(fy => fy.company_code == confirmSales.CompanyCode
                                                                         && fy.branch_code == confirmSales.BranchCode).FirstOrDefault().fin_year.ToString().Remove(0, 1) +
                                                                         db.KSTS_SEQ_NOS.Where(
                                                                         rs => rs.company_code == confirmSales.CompanyCode
                                                                         && rs.branch_code == confirmSales.BranchCode
                                                                         && rs.obj_id == "24").FirstOrDefault().nextno);

                        KTTU_CREDIT_BILL_DETAILS kcbd = new KTTU_CREDIT_BILL_DETAILS();
                        kcbd.Obj_Id = SIGlobals.Globals.GetNewGUID();
                        kcbd.company_code = ksm.company_code;
                        kcbd.branch_code = ksm.branch_code;
                        kcbd.amount_paid = confirmSales.TotalSRAmount;
                        kcbd.balance_amount = confirmSales.VPAmount - confirmSales.TotalSRAmount;
                        kcbd.bill_amount = 0;
                        kcbd.bill_no = originalBillNo;
                        kcbd.credit_bill_receiptNo = creditReceiptBillNo;
                        kcbd.Fin_Year = finYear;
                        kcbd.garunteename = ksm.operator_code;
                        kcbd.Ispaid = "Y";
                        kcbd.New_Bill_No = ksm.New_Bill_No;
                        kcbd.Paid_date = appDate;
                        kcbd.UpdateOn = SIGlobals.Globals.GetDateTime();
                        kcbd.UniqRowID = Guid.NewGuid();
                        db.KTTU_CREDIT_BILL_DETAILS.Add(kcbd);

                        KTTU_PAYMENT_DETAILS kpd = new KTTU_PAYMENT_DETAILS();
                        kpd.obj_id = payGUID;
                        kpd.company_code = confirmSales.CompanyCode;
                        kpd.branch_code = confirmSales.BranchCode;
                        kpd.series_no = originalBillNo;
                        kpd.receipt_no = creditReceiptBillNo;
                        kpd.sno = 1;
                        kpd.trans_type = "S";
                        kpd.pay_mode = "SR";
                        kpd.pay_details = "CAtrwSR";
                        kpd.pay_date = appDate;
                        kpd.pay_amt = confirmSales.TotalSRAmount;
                        kpd.Ref_BillNo = Convert.ToString(ksm.sales_bill_no);
                        kpd.party_code = null;
                        kpd.bill_counter = "FF";
                        kpd.is_paid = "N";
                        kpd.bank = "";
                        kpd.bank_name = null;
                        kpd.cheque_date = appDate;
                        kpd.card_type = null;
                        kpd.expiry_date = appDate;
                        kpd.cflag = "N";
                        kpd.card_app_no = null;
                        kpd.scheme_code = null;
                        kpd.sal_bill_type = "CR";
                        kpd.operator_code = confirmSales.OperatorCode;
                        kpd.session_no = 0;
                        kpd.UpdateOn = SIGlobals.Globals.GetDateTime();
                        kpd.group_code = null;
                        kpd.amt_received = 0;
                        kpd.bonus_amt = 0;
                        kpd.win_amt = 0;
                        kpd.ct_branch = null;
                        kpd.fin_year = finYear;
                        kpd.CardCharges = 0;
                        kpd.cheque_no = null;
                        kpd.New_Bill_No = Convert.ToString(originalBillNo);
                        kpd.Add_disc = 0;
                        kpd.isOrdermanual = "N";
                        kpd.currency_value = 0;
                        kpd.exchange_rate = 0;
                        kpd.currency_type = null;
                        kpd.tax_percentage = 0;
                        kpd.cancelled_by = "";
                        kpd.cancelled_remarks = "";
                        kpd.cancelled_date = Convert.ToString(appDate);
                        kpd.isExchange = "N";
                        kpd.exchangeNo = 0;
                        kpd.new_receipt_no = Convert.ToString(creditReceiptBillNo);
                        kpd.Gift_Amount = 0;
                        kpd.cardSwipedBy = "";
                        kpd.version = 0;
                        kpd.GSTGroupCode = null;
                        kpd.SGST_Percent = 0;
                        kpd.CGST_Percent = 0;
                        kpd.IGST_Percent = 0;
                        kpd.pay_amount_before_tax = confirmSales.TotalSRAmount;
                        kpd.pay_tax_amount = 0;
                        kpd.UniqRowID = Guid.NewGuid();
                        kpd.cash_marked_Counter = "";
                        db.KTTU_PAYMENT_DETAILS.Add(kpd);
                        confirm.CreditReceiptNo = creditReceiptBillNo;
                        //Updating Credit Receipt Serial No
                        SIGlobals.Globals.UpdateSeqenceNumber(db, "24", confirmSales.CompanyCode, confirmSales.BranchCode);
                        #endregion

                    }
                }
                #endregion

                #region Account Updates
                db.SaveChanges();
                ErrorVM procError = AccountPostingWithProedure(confirmSales.CompanyCode, confirmSales.BranchCode, salesReturnBillNo, db);
                if (procError != null) {
                    error = new ErrorVM()
                    {
                        field = "Account Update",
                        index = 0,
                        description = procError.description,
                        ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError
                    };
                    return null;
                }
                #endregion

                // Updating Sequence Number
                SIGlobals.Globals.UpdateSeqenceNumber(db, MODULE_SEQ_NO, confirmSales.CompanyCode, confirmSales.BranchCode);
                db.SaveChanges();
                confirm.SalesBillNo = ksm.sales_bill_no;

                #region Document Creation Posting, error will not be checked
                //Post to DocumentCreationLog Table
                DateTime billDate = SIGlobals.Globals.GetApplicationDate(confirmSales.CompanyCode, confirmSales.BranchCode);
                new Common.CommonBL().PostDocumentCreation(confirmSales.CompanyCode, confirmSales.BranchCode, 3, confirm.SalesBillNo, billDate, ksmOriginal.operator_code);
                #endregion
                return confirm;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }
        public string PrintSRBilling(string companyCode, string branchCode, int srBillNo, out ErrorVM error)
        {
            error = null;
            StringBuilder sb = new StringBuilder();
            KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(com => com.company_code == companyCode
                                                                          && com.branch_code == branchCode).FirstOrDefault();

            KTTU_SR_MASTER master = db.KTTU_SR_MASTER.Where(sr => sr.sales_bill_no == srBillNo
                                                            && sr.company_code == companyCode
                                                            && sr.branch_code == branchCode).FirstOrDefault();
            if (master == null) {
                error = new ErrorVM()
                {
                    field = "",
                    description = "Invalid Sales Return Bill No",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return "";
            }
            else if (master.cflag == "Y") {
                error = new ErrorVM()
                {
                    field = "",
                    description = "Sales return does not exists.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return "";
            }
            List<KTTU_SR_DETAILS> details = db.KTTU_SR_DETAILS.Where(sr => sr.sales_bill_no == srBillNo
                                                                    && sr.company_code == companyCode
                                                                    && sr.branch_code == branchCode).ToList();
            int finYear = db.KSTU_ACC_FY_MASTER.Where(f => f.company_code == companyCode && f.branch_code == branchCode).FirstOrDefault().fin_year;

            try {
                #region Generate HTML
                sb.AppendLine("<html>");
                sb.AppendLine("<head>");
                sb.AppendLine(SIGlobals.Globals.GetReportStyle());
                sb.AppendLine("</head>");
                sb.AppendLine("<body>");


                #region Report Header
                sb.AppendLine("<Table frame=\"border\" border=\"0\" width=\"900\">");
                for (int j = 0; j < 10; j++) {
                    sb.AppendLine("<TR style=border:0>");
                    sb.AppendLine(string.Format("<TD style=border:0 ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    sb.AppendLine("</TR>");
                }
                sb.AppendLine(string.Format("<TD style=border:0 ALIGN = \"CENTER\"><H3>{0}</H3> </TD>", "CREDIT NOTE"));
                for (int j = 0; j < 2; j++) {
                    sb.AppendLine("<TR style=border:0>");
                    sb.AppendLine(string.Format("<TD style=border:0 ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    sb.AppendLine("</TR>");
                }
                sb.AppendLine("</Table>");
                sb.AppendLine("<Table font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\" border-collapse:collapse; \" width=\"900\">");
                sb.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sb.AppendLine(string.Format("<TD width=\"300\" ALIGN = \"LEFT\"><b>CUSTOMER DETAILS</b></TD>"));
                sb.AppendLine(string.Format("<TD  width=\"300\" style=\"border-top:thin\"  ALIGN = \"CENTER\"><b>GSTIN {0}</b></TD>", company.tin_no.ToString()));
                sb.AppendLine(string.Format("<TD width=\"300\"  ALIGN = \"CENTER\"><b>{0}</b></TD>", "SHOWROOM ADDRESS"));
                sb.AppendLine("</TR>");
                string DateTime = string.Format("{0:dd/MM/yyyy} ", (master.sr_date));
                sb.AppendLine("<tr>");
                sb.AppendLine("<td>");
                sb.AppendLine("<Table>");
                sb.AppendLine("<Table font-size=12pt;  class=\"boldText\"  style=\"border-collapse:collapse;\" >");  //FRAME=BOX RULES=NONE

                sb.AppendLine("<tr style=\"border-right:0\"  >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left \" ><b>Name &nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + master.cust_name + "</b></td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" ><b>Address &nbsp&nbsp </b></td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + master.address1 + "</b></td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + master.address2 + "</b></td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + master.address3 + "</b></td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" ><b>State &nbsp&nbsp </b></td>");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + master.state + "</b></td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>State code &nbsp </b></td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + master.state_code + "</b></td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>Phone &nbsp&nbsp </b></td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + master.mobile_no + "</b></td>");
                sb.AppendLine("</tr>");
                if (!string.IsNullOrEmpty(master.pan_no)) {
                    sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sb.AppendLine("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>PAN &nbsp&nbsp</b></td>");
                    sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + master.pan_no + "</b></td>");
                    sb.AppendLine("</tr>");
                }
                else {
                    List<KSTU_CUSTOMER_ID_PROOF_DETAILS> custIDProf = db.KSTU_CUSTOMER_ID_PROOF_DETAILS.Where(pr => pr.cust_id == master.cust_id
                                                                                                       && pr.company_code == companyCode
                                                                                                       && pr.branch_code == branchCode).ToList();
                    if (custIDProf.Count > 0) {
                        if (!string.IsNullOrEmpty(custIDProf[0].Doc_code.ToString())) {
                            sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                            sb.AppendLine(string.Format("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>{0} &nbsp&nbsp</b></td>", custIDProf[0].Doc_code.ToString()));
                            sb.AppendLine(string.Format("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>{0}</b></td>", custIDProf[0].Doc_No.ToString()));
                            sb.AppendLine("</tr>");
                        }
                    }
                }
                if (!string.IsNullOrEmpty(master.tin)) {
                    sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    sb.AppendLine("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>GSTIN &nbsp&nbsp</b></td>");
                    sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + master.tin + "</b></td>");
                    sb.AppendLine("</tr>");
                }
                sb.AppendLine("</table>");
                sb.AppendLine("</td>");

                sb.AppendLine("<td>");
                sb.AppendLine("<Table font-size=12pt;   class=\"boldText\"  style=\"border-collapse:collapse;\" >");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" align=\"left\" ><b>Invoice No &nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" ><b>" + branchCode + "/" + "R" + "/" + master.sales_bill_no.ToString() + "</b></td>");
                sb.AppendLine("</tr>");


                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" align=\"left\" ><b>Est No &nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" ><b>" + master.est_no.ToString() + "</b></td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" align=\"left\" ><b>Date &nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" ><b>" + DateTime + "</b></td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" ><b> Place of supply &nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" ><b>" + master.state.ToString() + "</b></td>");
                sb.AppendLine("</tr>");


                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" ><b> Tax Payment Mode &nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" ><b>" + "Normal" + "</b></td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-right:0\">");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" align=\"center \" ><b>Against Invoice</b></td>");
                sb.AppendLine(string.Format("<td style=\"border-right:thin ; border-top:thin\" ><b>  {0}/{2}/{1}</b></TD>", master.billed_branch, master.voture_bill_no, "S"));
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-right:0\">");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" align=\"center \" ><b>Invoice Date</b></td>");
                sb.AppendLine(string.Format("<td style=\"border-right:thin ; border-top:thin\" ><b>  {0}/{1}</b></TD>", master.billed_branch, string.Format("{0:dd/MM/yyyy} ", master.bill_date)));
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" ><b>PAN &nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" ><b>" + Convert.ToString(master.pan_no) + "</b></td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("</table>");
                sb.AppendLine("</td>");
                sb.AppendLine("<td>");
                sb.AppendLine("<Table font-size=12pt;   class=\"boldText\"  style=\"border-collapse:collapse;\" >");  //FRAME=BOX RULES=NONE
                sb.AppendLine("<tr style=\"border-right:0\"  >");
                sb.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left \" ><b>Name &nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.company_name.ToString() + "</b></td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>Address &nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.address1.ToString() + "</b></td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin \" style=font-size=\"12pt\" style=\"border-top:thin\" ><b>" + company.address2.ToString() + "</b></td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.address3.ToString() + "</b></td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>City &nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.city.ToString() + "</b></td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>State &nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + company.state.ToString() + "</b></td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>State code &nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin  ;border-top:thin\" ><b>" + company.state_code.ToString() + "</b></td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>Phone &nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + company.phone_no.ToString() + "</b></td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("</table>");
                sb.AppendLine("</td>");
                sb.AppendLine("</TR>");

                sb.AppendLine("</Table>");
                sb.AppendLine("<Table  font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse;\" width=\"900\">");  //FRAME=BOX RULES=NONE

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD COLSPAN=15 ALIGN = \"CENTER\"><b>ORIGINAL / DUPLICATE <br></b></TD>"));
                sb.AppendLine("</TR>");

                sb.AppendLine("<TR bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sb.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                sb.AppendLine("<TH style=\"border-bottom:thin\"  ALIGN = \"CENTER\"><b></b></TH>");
                sb.AppendLine("<TH style=\"border-bottom:thin\"  ALIGN = \"CENTER\"><b>Item</b></TH>");
                sb.AppendLine("<TH style=\"border-bottom:thin\"  ALIGN = \"CENTER\"><b></b></TH>");
                sb.AppendLine("<TH style=\"border-bottom:thin\"  ALIGN = \"CENTER\"><b></b></TH>");
                sb.AppendLine(string.Format("<TH style=\"border-bottom:thin\"  ALIGN = \"CENTER\"><b>Rate /</b></TH>"));
                sb.AppendLine("<TH style=\"border-bottom:thin\"  ALIGN = \"CENTER\"><b></b></TH>");
                sb.AppendLine("<TH colspan = 3 ALIGN = \"CENTER\"><b>[Weight in Grams]</b></TH>");
                sb.AppendLine("<TH style=\"border-bottom:thin\"  ALIGN = \"CENTER\"><b>Gold</b></TH>");
                sb.AppendLine("<TH style=\"border-bottom:thin\"  ALIGN = \"CENTER\"><b>VA</b></TH>");
                sb.AppendLine("<TH style=\"border-bottom:thin\"  ALIGN = \"CENTER\"><b>Stn/Dmd</b></TH>");
                sb.AppendLine("<TH style=\"border-bottom:thin\"  ALIGN = \"CENTER\"><b>Gross</b></TH>");
                sb.AppendLine("<TH style=\"border-bottom:thin\"  ALIGN = \"CENTER\"><b></b></TH>");
                sb.AppendLine("</TR>");
                sb.AppendLine("<TR bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sb.AppendLine("<TH style=\"border-bottom:thin solid; border-top:thin\"  ALIGN = \"CENTER\"><b>Sl.No</b></TH>");
                sb.AppendLine("<TH style=\"border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b>C</b></TH>");
                sb.AppendLine("<TH style=\"border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b>Description</b></TH>");
                sb.AppendLine("<TH style=\"border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b>HSN Code </b></TH>");

                sb.AppendLine("<TH style=\"border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b>Purity</b></TH>");
                sb.AppendLine(string.Format("<TH style=\"border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b>Gram</b></TH>"));
                sb.AppendLine("<TH style=\"border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b>Qty</b></TH>");
                sb.AppendLine("<TH style=\"border-bottom:thin solid;\" ALIGN = \"CENTER\"><b>Gr.Wt</b></TH>");
                sb.AppendLine("<TH style=\"border-bottom:thin solid; \" ALIGN = \"CENTER\"><b>St.Wt</b></TH>");
                sb.AppendLine("<TH style=\"border-bottom:thin solid; \" ALIGN = \"CENTER\"><b>Net Wt</b></TH>");
                sb.AppendLine("<TH style=\"border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b> Amount</b></TH>");
                sb.AppendLine("<TH style=\"border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b>Amount</b></TH>");
                sb.AppendLine("<TH style=\"border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b>Amount</b></TH>");
                sb.AppendLine("<TH style=\"border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b>Amount</b></TH>");
                sb.AppendLine("<TH style=\"border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                sb.AppendLine("</TR>");

                #endregion

                #region Report body
                int MaxPageRow = 10;
                decimal Gwt = 0, Swt = 0, Nwt = 0, Amount = 0, TotalQty = 0, GoldValue = 0, StoneChg = 0, ValueAmt = 0;
                decimal GrossAmount = 0;
                decimal TotalGross = 0;
                //decimal vaPercent = 0, discountVa = 0, disCountAmount = 0;
                decimal SGSTAmt = 0, IGSTAmt = 0, CGSTAmt = 0;
                decimal AmountReceivable = 0;
                decimal finalAmount = 0;
                //decimal taxabaleAmount = 0;
                //decimal ItemAmount = 0, VatPerGarm = 0;
                decimal NetWt = 0;
                decimal va = 0, additionalDiscount = 0;
                //decimal DisCountAmt = 0;
                decimal TotalTaxable = 0;
                decimal TotalDisCount = 0;
                decimal TotalReceivable = 0;
                int slno = 1;
                for (int i = 0; i < details.Count; i++) {
                    sb.AppendLine("<TR ALIGN = \"LEFT\"> ");
                    sb.Append(string.Format("<TD style=\"border-bottom:thin ; border-top:thin\" align=\"center\"><b>{0}{1}{1}</b></TD>", slno, " &nbsp"));
                    sb.Append(string.Format("<TD style=\"border-bottom:thin ; border-top:thin\" ><b>{0}{1}{1}</b></TD>", details[i].counter_code.ToString(), "&nbsp"));

                    decimal _stoneCharges = Convert.ToDecimal(details[i].stone_charges) + Convert.ToDecimal(details[i].diamond_charges);
                    string gsCode = details[i].gs_code;
                    string name = details[i].item_name;
                    ITEM_MASTER itemMaster = db.ITEM_MASTER.Where(it => it.gs_code == gsCode
                                                                    && it.Item_code == name
                                                                    && it.company_code == companyCode
                                                                    && it.branch_code == it.branch_code).FirstOrDefault();
                    string aliasName = itemMaster == null ? "" : itemMaster.Item_Name;
                    sb.Append(string.Format("<TD style=\"border-bottom:thin ; border-top:thin\" ><b>{0}{1}{1}</b></TD>", aliasName, "&nbsp"));
                    sb.Append(string.Format("<TD style=\"border-bottom:thin ; border-top:thin\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", Convert.ToString(details[i].HSN), "&nbsp"));
                    sb.Append(string.Format("<TD style=\"border-bottom:thin ; border-top:thin\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", "91.6", "&nbsp"));


                    sb.Append(string.Format("<TD style=\"border-bottom:thin ; border-top:thin\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", master.rate, "&nbsp"));
                    sb.Append(string.Format("<TD style=\"border-bottom:thin ; border-top:thin\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", details[i].quantity.ToString(), "&nbsp"));
                    sb.Append(string.Format("<TD style=\"border-bottom:thin ; border-top:thin\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", details[i].gwt.ToString(), "&nbsp"));
                    sb.Append(string.Format("<TD style=\"border-bottom:thin ; border-top:thin\" ALIGN = \"RIGHT\" ><b>{0}{1}{1}</b></TD>", details[i].swt.ToString(), "&nbsp"));
                    sb.Append(string.Format("<TD style=\"border-bottom:thin ; border-top:thin\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", details[i].nwt.ToString(), "&nbsp"));
                    NetWt = Convert.ToDecimal(details[i].nwt.ToString());
                    sb.Append(string.Format("<TD style=\"border-bottom:thin ; border-top:thin\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", details[i].sr_amount.ToString(), "&nbsp"));

                    decimal vaAmount = Convert.ToDecimal(details[i].va_amount);
                    decimal vaPerGram = Decimal.Round(vaAmount / Convert.ToDecimal(details[i].nwt), 2);

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

                    sb.Append(string.Format("<TD style=\"border-bottom:thin ; border-top:thin\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", MCAmount, "&nbsp"));
                    sb.Append(string.Format("<TD style=\"border-bottom:thin ; border-top:thin\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", _stoneCharges.ToString(), "&nbsp"));

                    va = Math.Round((Convert.ToDecimal(details[i].va_percent.ToString()) / 100), 2);
                    GrossAmount = Convert.ToDecimal(details[i].sr_amount) + MCAmount + _stoneCharges;

                    sb.Append(string.Format("<TD style=\"border-bottom:thin ; border-top:thin\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", GrossAmount, "&nbsp"));
                    sb.Append(string.Format("<TD style=\"border-bottom:thin ; border-top:thin\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD>", master.sal_code.ToString(), "&nbsp"));

                    TotalQty += Convert.ToInt32(details[i].quantity);
                    Gwt += Convert.ToDecimal(details[i].gwt);
                    Swt += Convert.ToDecimal(details[i].swt);
                    Nwt += Convert.ToDecimal(details[i].nwt);
                    TotalGross += GrossAmount;
                    TotalTaxable += Convert.ToDecimal(details[i].item_total_after_discount);
                    TotalDisCount += Convert.ToDecimal(details[i].Mc_Discount_Amt);

                    if (details[i].SGST_Amount.ToString() != "NULL") {
                        SGSTAmt += Convert.ToDecimal(details[i].SGST_Amount);
                    }
                    if (details[i].CGST_Amount.ToString() != "NULL") {
                        CGSTAmt += Convert.ToDecimal(details[i].CGST_Amount);
                    }
                    if (details[i].IGST_Amount.ToString() != "NULL") {
                        IGSTAmt += Convert.ToDecimal(details[i].IGST_Amount);
                    }

                    ValueAmt += MCAmount;
                    StoneChg += _stoneCharges;
                    GoldValue += Convert.ToDecimal(details[i].sr_amount);
                    Amount += Convert.ToDecimal(details[i].net_amount);
                    TotalReceivable += AmountReceivable;
                    additionalDiscount += Convert.ToDecimal(details[i].item_additional_discount);
                    finalAmount += Convert.ToDecimal(details[i].item_final_amount.ToString());
                    sb.AppendLine("</TR>");
                    slno = slno + 1;
                }
                for (int j = 0; j < MaxPageRow - details.Count; j++) {
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD style=\"border-top:thin;border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=\"border-top:thin;border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=\"border-top:thin;border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=\"border-top:thin;border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=\"border-top:thin;border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=\"border-top:thin;border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=\"border-top:thin;border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=\"border-top:thin;border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=\"border-top:thin;border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=\"border-top:thin;border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=\"border-top:thin;border-bottom:thin\"  ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=\"border-top:thin;border-bottom:thin\"  ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=\"border-top:thin;border-bottom:thin\"  ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=\"border-top:thin;border-bottom:thin\"  ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=\"border-top:thin;border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sb.AppendLine("</TR>");
                }
                sb.AppendLine("<TR style=\"border-bottom:thin solid; \" bgcolor='#FFFACD'  style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sb.AppendLine("<TH style=\"border-bottom:thin solid;border-right:thin\" ALIGN = \"LEFT\"><b>Totals</b></TH>");
                sb.Append(string.Format("<TH style=\"border-bottom:thin solid;border-right:thin\" ALIGN = \"RIGHT\" <b>{0}{1}{1}</b></TH>", "&nbsp;", "&nbsp;"));
                sb.Append(string.Format("<TH  style=\"border-bottom:thin solid;border-right:thin\" ALIGN = \"RIGHT\" <b>{0}{1}{1}</b></TH>", "&nbsp;", "&nbsp;"));
                sb.Append(string.Format("<TH style=\"border-bottom:thin solid;border-right:thin\" ALIGN = \"RIGHT\" <b>{0}{1}{1}</b></TH>", "&nbsp;", "&nbsp;"));
                sb.Append(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\" <b>{0}{1}{1}</b></TH>", "&nbsp;", "&nbsp;"));
                sb.Append(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\"<b>{0}{1}{1}</b></TH>", "&nbsp;", "&nbsp;"));
                sb.Append(string.Format("<TH style=\"border-bottom:thin solid\"  ALIGN = \"RIGHT\" <b>{0}{1}{1}</b></TH>", TotalQty, "&nbsp;"));
                sb.Append(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\" <b>{0}{1}{1}</b></TH>", Gwt, "&nbsp;"));
                sb.Append(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\"<b>{0}{1}{1}</b></TH>", Swt, "&nbsp;"));
                sb.Append(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\"<b>{0}{1}{1}</b></TH>", Nwt, "&nbsp;"));
                sb.Append(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\" <b>{0}{1}{1}</b></TH>", GoldValue, "&nbsp;"));
                sb.Append(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\" <b>{0}{1}{1}</b></TH>", ValueAmt, "&nbsp;"));
                sb.Append(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\"<b>{0}{1}{1}</b></TH>", StoneChg, "&nbsp;"));
                sb.Append(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TH>", TotalGross, "&nbsp;"));
                sb.Append(string.Format("<TH style=\"border-bottom:thin solid\" ALIGN = \"RIGHT\" <b>{0}{1}{1}</b></TH>", "&nbsp;", "&nbsp;"));
                sb.AppendLine("</TR>");

                finalAmount = Math.Round(TotalTaxable + SGSTAmt + CGSTAmt + IGSTAmt, 0, MidpointRounding.AwayFromZero);
                #endregion

                #region Stone Details
                List<KTTU_SR_STONE_DETAILS> stoneDet = db.KTTU_SR_STONE_DETAILS.Where(st => st.sales_bill_no == srBillNo
                                                                                    && st.company_code == companyCode
                                                                                    && st.branch_code == branchCode).ToList();

                if (stoneDet != null && stoneDet.Count > 0) {
                    for (int j = 0; j < stoneDet.Count; j++) {

                        if (string.Compare(stoneDet[j].type.ToString(), "D") == 0 || string.Compare(stoneDet[j].type.ToString(), "DMD") == 0) {
                            sb.AppendLine("<TR>");
                            sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin\" colspan=15 ALIGN = \"LEFT\"><b>{0}){1}/{2}*{3}={4}</b></TD>", j + 1, stoneDet[j].name.ToString(), stoneDet[j].carrat.ToString(), decimal.Round(Convert.ToDecimal(stoneDet[j].rate), 0), stoneDet[j].amount.ToString()));
                            sb.AppendLine("</TR>");

                        }
                        else {
                            decimal stnAmount = Convert.ToDecimal(stoneDet[j].amount);
                            if (stnAmount > 0) {
                                sb.AppendLine("<TR>");
                                sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin\" colspan=15 ALIGN = \"LEFT\"><b>{0}){1}/{2}</b></TD>", j + 1, stoneDet[j].name.ToString(), stnAmount));
                                sb.AppendLine("</TR>");
                            }
                        }
                    }
                }
                #endregion

                #region Footer
                decimal InvoiceRoundOff = Math.Round(TotalReceivable, 2);
                if (TotalDisCount > 0) {
                    sb.AppendLine(string.Format("<TR><TD style=\"border-bottom:thin ; border-top:thin; border-right:thin\" colspan=13 ALIGN = \"RIGHT\"><b>{0}</b></TD> <TD style=\"border-bottom:thin ; border-top:thin \" colspan=1 ALIGN = \"RIGHT\"><b>{1}</b></TD> <TD style=\"border-bottom:thin ; border-top:thin \" > </TD></TR>", "Discount" + " ", decimal.Round(TotalDisCount, 2) + "&nbsp"));
                    sb.AppendLine();
                }
                if ((additionalDiscount) > 0) {
                    sb.AppendLine(string.Format("<TR><TD style=\"border-bottom:thin ; border-top:thin; border-right:thin\" colspan=13 ALIGN = \"RIGHT\"><b>{0}</b></TD> <TD style=\"border-bottom:thin ; border-top:thin \" colspan=1 ALIGN = \"RIGHT\"><b>{1}</b></TD> <TD style=\"border-bottom:thin ; border-top:thin \" > </TD></TR>", "Discount" + " ", additionalDiscount + "&nbsp"));
                    sb.AppendLine();
                }

                sb.AppendLine(string.Format("<TR><TD style=\"border-bottom:thin ; border-top:thin; border-right:thin\" colspan=13 ALIGN = \"RIGHT\"><b>{0}</b></TD> <TD style=\"border-bottom:thin ; border-top:thin \" colspan=1 ALIGN = \"RIGHT\"><b>{1}</b></TD> <TD style=\"border-bottom:thin ; border-top:thin \"> </TD> </TR>", "Taxable Value" + " ", TotalTaxable + "&nbsp"));
                sb.AppendLine();

                sb.AppendLine(string.Format("<TR><TD style=\"border-bottom:thin ; border-top:thin; border-right:thin\" colspan=13 ALIGN = \"RIGHT\"><b>{0}</b></TD> <TD style=\"border-bottom:thin ; border-top:thin \" colspan=1 ALIGN = \"RIGHT\"><b>{1}</b></TD><TD style=\"border-bottom:thin ; border-top:thin \" > </TD>  </TR>", "SGST " + details[0].SGST_Percent + "% ", SGSTAmt + "&nbsp"));
                sb.AppendLine();
                sb.AppendLine(string.Format("<TR><TD style=\"border-bottom:thin ; border-top:thin; border-right:thin\" colspan=13 ALIGN = \"RIGHT\"><b>{0}</b></TD> <TD style=\"border-bottom:thin ; border-top:thin \" colspan=1 ALIGN = \"RIGHT\"><b>{1}</b></TD> <TD style=\"border-bottom:thin ; border-top:thin \"> </TD> </TR>", "CGST " + details[0].CGST_Percent + "% ", CGSTAmt + "&nbsp"));
                sb.AppendLine();
                sb.AppendLine(string.Format("<TR><TD style=\"border-bottom:thin ; border-top:thin; border-right:thin\" colspan=13 ALIGN = \"RIGHT\"><b>{0}</b></TD> <TD style=\"border-bottom:thin ; border-top:thin \" colspan=1 ALIGN = \"RIGHT\"><b>{1}</b></TD> <TD style=\"border-bottom:thin ; border-top:thin \" > </TD> </TR>", "IGST " + details[0].IGST_Percent + "% ", IGSTAmt + "&nbsp"));
                sb.AppendLine();
                sb.AppendLine(string.Format("<TR><TD style=\"border-bottom:thin ; border-top:thin; border-right:thin \" colspan=13 ALIGN = \"RIGHT\"><b>{0}</b></TD> <TD style=\"border-bottom:thin ; border-top:thin \" colspan=1 ALIGN = \"RIGHT\"><b>{1}</b></TD><TD style=\"border-bottom:thin ; border-top:thin \" ></TD>  </TR>", "Invoice Amount" + " ", finalAmount + "&nbsp"));
                sb.AppendLine();

                #region Purchase Attachment
                string refBillNo = Convert.ToString(srBillNo);
                KTTU_PAYMENT_DETAILS payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.Ref_BillNo == refBillNo
                                                                            && pay.company_code == companyCode
                                                                            && pay.branch_code == branchCode
                                                                            && pay.pay_mode == "SR"
                                                                            && pay.fin_year == finYear
                                                                            && (pay.trans_type == "S" || pay.trans_type == "O")).FirstOrDefault();
                if (payment != null) {
                    decimal purAmount = Convert.ToDecimal(payment.pay_amt);
                    purAmount = Math.Round(purAmount, 0, MidpointRounding.ToEven);
                    if ((string.Compare(payment.trans_type.ToString(), "S") == 0) && Convert.ToDecimal(payment.receipt_no) > 0)
                        sb.AppendLine(string.Format("<TR> <TD style=\"border-bottom:thin ; border-top:thin \" colspan=15 ALIGN = \"RIGHT\"><b>{0}</b></TD>  </TR>", "Adjusted Towards Credit Receipt No" + payment.receipt_no + " &nbsp"));

                    else if ((string.Compare(payment.trans_type.ToString(), "O") == 0) && Convert.ToDecimal(payment.receipt_no) > 0)
                        sb.AppendLine(string.Format("<TR><TD style=\"border-bottom:thin ; border-top:thin\" colspan=15 ALIGN = \"RIGHT\"><b>{0}</b></TD> </TR>", "Adjusted Towards Order Receipt No " + payment.receipt_no + " &nbsp"));

                    else
                        sb.AppendLine(string.Format("<TR> <TD style=\"border-bottom:thin ; border-top:thin\" colspan=15 ALIGN = \"RIGHT\"><b>{0}</b></TD></TR>", "Adjusted Towards Sales Bill No " + branchCode + "/" + "S" + "/" + payment.series_no + " &nbsp"));
                    sb.AppendLine();
                }
                #endregion

                #region Payments
                var payments = (from p in db.KTTU_PAYMENT_DETAILS
                                where p.series_no == srBillNo && p.company_code == companyCode
                                && p.branch_code == branchCode && p.trans_type == "RS"
                                && (p.sal_bill_type == "" || p.sal_bill_type != "CR") && p.pay_mode != "CT"
                                group p by new
                                {
                                    p.pay_mode,
                                    p.Ref_BillNo,
                                    p.scheme_code,
                                    p.bank_name,
                                    p.card_type,
                                    p.cheque_no,
                                    p.bank,
                                    p.party_code
                                } into g
                                select new
                                {
                                    PayMode = g.Key.pay_mode,
                                    PayAmount = g.Sum(x => x.pay_amt),
                                    RefBillNo = g.Key.Ref_BillNo,
                                    SchemeCode = g.Key.scheme_code,
                                    BankName = g.Key.bank_name,
                                    CardType = g.Key.card_type,
                                    ChequeNo = g.Key.cheque_no,
                                    Bank = g.Key.bank,
                                    PartyCode = g.Key.party_code
                                }).OrderByDescending(ord => ord.PayMode).ToList();

                if (payments != null && payments.Count > 0) {
                    for (int i = 0; i < payments.Count; i++) {
                        decimal PayAmt = Convert.ToDecimal(payments[i].PayAmount);
                        PayAmt = Math.Round(PayAmt, 0, MidpointRounding.ToEven);
                        if (string.Compare(payments[i].PayMode.ToString(), "Q") == 0) {
                            sb.AppendLine("<TR>");
                            sb.AppendLine(string.Format("<TR><TD style=\"border-bottom:thin ; border-top:thin;border-right:thin \" colspan=13 ALIGN = \"RIGHT\"><b>By Cheque Drawn on {0}/{1}</b></TD> <TD style=\"border-bottom:thin ; border-top:thin \" colspan=1 ALIGN = \"RIGHT\"><b>{2}</b></TD> <TD style=\"border-bottom:thin ; border-top:thin \" > </TD> </TR>", payments[i].Bank.ToString().Trim(), payments[i].ChequeNo + ":", PayAmt.ToString("N") + "&nbsp", Amount + "&nbsp"));
                            sb.AppendLine("</TR>");
                        }
                        else if (string.Compare(payments[i].PayMode.ToString(), "C") == 0) {

                            sb.AppendLine("<TR>");
                            sb.AppendLine(string.Format("<TR><TD style=\"border-bottom:thin ; border-top:thin ;border-right:thin\" colspan=13 ALIGN = \"RIGHT\"><b>Cash paid</b></TD> <TD style=\"border-bottom:thin ; border-top:thin \" colspan=1 ALIGN = \"RIGHT\"><b>{0}</b></TD>  <TD style=\"border-bottom:thin ; border-top:thin \" > </TD></TR>", PayAmt.ToString("N", indianDefaultCulture) + "&nbsp"));
                            sb.AppendLine("</TR>");
                        }
                        else if (string.Compare(payments[i].PayMode.ToString(), "EP") == 0) {

                            sb.AppendLine("<TR>");
                            sb.AppendLine(string.Format("<TR><TD style=\"border-bottom:thin ; border-top:thin ;border-right:thin\" colspan=13 ALIGN = \"RIGHT\"><b>E-Payment</b></TD> <TD style=\"border-bottom:thin ; border-top:thin \" colspan=1 ALIGN = \"RIGHT\"><b>{0}</b></TD>  <TD style=\"border-bottom:thin ; border-top:thin \" > </TD></TR>", PayAmt.ToString("N", indianDefaultCulture) + "&nbsp"));
                            sb.AppendLine("</TR>");
                        }
                    }
                    sb.AppendLine(string.Format("<TR><TD style=\"border-bottom:thin ; border-top:thin;border-right:thin\" colspan=13 ALIGN = \"RIGHT\"><b>Balance</b></TD> <TD style=\"border-bottom:thin ; border-top:thin \" colspan=1 ALIGN = \"RIGHT\"><b>{0}</b></TD> <TD style=\"border-bottom:thin ; border-top:thin \" > </TD>  </TR>", "Nill" + "&nbsp"));
                }
                #endregion


                Amount = decimal.Round(finalAmount, 0);
                string strWords = string.Empty; ;
                NumberToWords objNWClass = new NumberToWords();
                objNWClass.ConvertNumberToWords(Convert.ToDouble(Amount), out strWords);

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-bottom:thin \" colspan = 15 ALIGN = \"LEFT\"><b>{0}</b></TD>", "Rupees " + strWords));
                sb.AppendLine("</TR>");

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin\" colspan=15 ALIGN = \"LEFT\"><b>Operator: {0}</b></TD>", master.operator_code.ToString()));
                sb.AppendLine("</TR>");

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD colspan = 7 style=\"border-right:thin\" ALIGN = \"LEFT\"><b><br><br><br><br>{0}</b></TD>", "Customer Signature"));
                sb.AppendLine(string.Format("<TD colspan = 8 ALIGN = \"RIGHT\"><b>For {0}<br><br><br><br>{1}</b></TD>", company.company_name, "Authorized Signatory"));
                sb.AppendLine("</TR>");
                if (company.Header3.ToString() != string.Empty) {
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD colspan = 15 ALIGN = \"CENTER\"><b>{0}</b></TD>", company.Header3.ToString()));
                    sb.AppendLine("</TR>");
                }
                if (company.Header4.ToString() != string.Empty) {
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD colspan = 15 ALIGN = \"CENTER\"><b>{0}</b></TD>", company.Header4.ToString()));
                    sb.AppendLine("</TR>");
                }
                sb.AppendLine();
                sb.AppendLine();
                #endregion

                sb.AppendLine("</body>");
                sb.AppendLine("<html>");
                #endregion
                return sb.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        public ProdigyPrintVM PrintSRBill(string companyCode, string branchCode, int billNo, out ErrorVM error)
        {
            error = null;
            string printData = PrintSRBilling(companyCode, branchCode, billNo, out error);
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            printObject.PrintType = "HTML";
            printObject.ContinueNextPrint = true;
            printObject.Data = new PrintConfiguration().Base64Encode(printData);
            return printObject;
        }

        public bool CancelSRBill(ConfirmSalesReturnVM confirmSales, out ErrorVM error)
        {
            error = null;
            try {

                #region Validating SR its Adjusted or Not
                string srBill = Convert.ToString(confirmSales.BillNo);
                KTTU_PAYMENT_DETAILS payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.company_code == confirmSales.CompanyCode
                                                                                && pay.branch_code == confirmSales.BranchCode
                                                                                && pay.pay_mode == "SR"
                                                                                && pay.Ref_BillNo == srBill
                                                                                && pay.cflag == "N"
                                                                                && pay.sal_bill_type != "CR").FirstOrDefault();
                if (payment != null) {
                    string tranType = payment.trans_type == "O" ? "Order No: " + payment.series_no : "Bill No:" + payment.series_no;
                    error = new ErrorVM()
                    {
                        description = "SR Bill No: " + confirmSales.BillNo + " already is adjusted towords " + tranType,
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
                #endregion

                #region Update Master and Payment Details

                #region Cancel Master
                KTTU_SR_MASTER master = db.KTTU_SR_MASTER.Where(sr => sr.sales_bill_no == confirmSales.BillNo
                                                                    && sr.branch_code == confirmSales.BranchCode
                                                                    && sr.company_code == confirmSales.CompanyCode).FirstOrDefault();
                if (master.cflag == "Y") {
                    error = new ErrorVM()
                    {
                        description = "SR Bill cancelled already " + confirmSales.BillNo,
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
                master.cflag = "Y";
                master.UpdateOn = SIGlobals.Globals.GetDateTime();
                master.cancelled_by = confirmSales.OperatorCode;
                master.cancelled_remarks = confirmSales.CancleRemarks;
                db.Entry(master).State = System.Data.Entity.EntityState.Modified;
                #endregion

                #region Cancel Payment
                // Cancel with Payment (Cash/Cheque/E-pay. Cancel with Cash)
                List<KTTU_PAYMENT_DETAILS> payments = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == confirmSales.BillNo
                                                                                    && pay.trans_type == "RS"
                                                                                    && pay.company_code == confirmSales.CompanyCode
                                                                                    && pay.branch_code == confirmSales.BranchCode).ToList();
                foreach (KTTU_PAYMENT_DETAILS pay in payments) {
                    pay.cflag = "Y";
                    pay.UpdateOn = SIGlobals.Globals.GetDateTime();
                    db.Entry(pay).State = System.Data.Entity.EntityState.Modified;
                }
                #endregion

                #region Cancel Credit Receipt

                // Cancelling the Credit Receipt if Generated, This Scenario will come when SR done for the Credit Bill with Negetive Payable Amount.
                // This Scenario will come only with the Credit Bill.
                string billNo = confirmSales.BillNo.ToString();
                KTTU_PAYMENT_DETAILS paymentsCR = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.Ref_BillNo == billNo
                                                                                   && pay.trans_type == "S"
                                                                                   && pay.pay_mode == "SR"
                                                                                   && pay.sal_bill_type == "CR"
                                                                                   && pay.company_code == confirmSales.CompanyCode
                                                                                   && pay.branch_code == confirmSales.BranchCode).FirstOrDefault();
                if (paymentsCR != null) {
                    paymentsCR.cflag = "Y";
                    paymentsCR.UpdateOn = SIGlobals.Globals.GetDateTime();
                    db.Entry(paymentsCR).State = System.Data.Entity.EntityState.Modified;
                }

                #endregion

                #region Cancel Order

                //Cancel the Generated Order From Order Master
                // When we generated the Order for the Balance Amount they generate one Credit Receipt that will cancelled in the above Scenario
                // Only need to cancell the order
                KTTU_PAYMENT_DETAILS genOrder = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.Ref_BillNo == billNo
                                                                                  && pay.trans_type == "O"
                                                                                  && pay.company_code == confirmSales.CompanyCode
                                                                                  && pay.branch_code == confirmSales.BranchCode).FirstOrDefault();
                if (paymentsCR != null) {
                    genOrder.cflag = "Y";
                    genOrder.UpdateOn = SIGlobals.Globals.GetDateTime();
                    db.Entry(paymentsCR).State = System.Data.Entity.EntityState.Modified;
                }

                if (genOrder != null) {
                    KTTU_ORDER_MASTER kom = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == genOrder.series_no
                                                              && ord.company_code == confirmSales.CompanyCode
                                                              && ord.branch_code == confirmSales.BranchCode
                                                             ).FirstOrDefault();
                    if (kom != null) {
                        kom.cflag = "Y";
                        kom.UpdateOn = SIGlobals.Globals.GetDateTime();
                        db.Entry(kom).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                #endregion

                #endregion

                #region Stock Update
                decimal grossWt = 0;
                decimal netWt = 0;
                decimal stoneWt = 0;
                int quantity = 0;
                List<KTTU_SR_DETAILS> details = db.KTTU_SR_DETAILS.Where(det => det.sales_bill_no == confirmSales.BillNo
                                                                            && det.company_code == confirmSales.CompanyCode
                                                                            && det.branch_code == confirmSales.BranchCode).ToList();

                foreach (KTTU_SR_DETAILS sr in details) {
                    grossWt = Convert.ToDecimal((sr.gwt + sr.AddWt) - sr.DeductWt);
                    netWt = Convert.ToDecimal((sr.nwt + sr.AddWt) - sr.DeductWt);
                    stoneWt = Convert.ToDecimal(sr.swt);
                    quantity = Convert.ToInt32((sr.quantity + sr.Addqty) - sr.Deductqty);

                    bool updated = GSIssueStock(sr.gs_code, quantity, grossWt, netWt, sr.company_code, sr.branch_code);
                    if (!updated) {
                        error = new ErrorVM() { description = "Error While Updating Stock", ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError };
                        return false;
                    }
                }
                foreach (KTTU_SR_DETAILS detCounter in details) {
                    KSTS_GS_ITEM_ENTRY gsItem = db.KSTS_GS_ITEM_ENTRY.Where(gs => gs.gs_code == detCounter.gs_code
                                                                        && gs.company_code == detCounter.company_code
                                                                        && gs.branch_code == detCounter.branch_code).FirstOrDefault();
                    if (gsItem.bill_type == "S") {

                        bool counterUpdated = CancelCounterIssueStock(detCounter.gs_code, detCounter.item_name, detCounter.counter_code, detCounter.quantity, detCounter.gwt,
                               detCounter.nwt, detCounter.swt, Convert.ToDecimal(detCounter.AddWt), Convert.ToDecimal(detCounter.DeductWt), Convert.ToInt32(detCounter.Addqty),
                             Convert.ToInt32(detCounter.Deductqty), detCounter.company_code, detCounter.branch_code);
                        if (!counterUpdated) {
                            error = new ErrorVM() { description = "Error While Updating Couner Stock", ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError };
                            return false;
                        }
                    }
                }

                #endregion

                #region Acccount Updates
                string sql = " UPDATE KSTU_ACC_VOUCHER_TRANSACTIONS SET cflag = 'Y' " +
                             " WHERE receipt_no = @p0 AND trans_type = @p1 AND company_code = @p2 AND branch_code = @p3";
                List<object> parameterList = new List<object>();
                //parameterList.Add(Convert.ToString(master.sales_bill_no + "," + master.voture_bill_no));
                parameterList.Add(Convert.ToString(master.sales_bill_no));
                parameterList.Add("SR");
                parameterList.Add(master.company_code);
                parameterList.Add(master.branch_code);
                object[] parametersArray = parameterList.ToArray();
                int recordsAffected = SIGlobals.Globals.ExecuteSQL(sql, db, parametersArray);
                #endregion

                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }
        public dynamic GetAdjustedSRBill(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                return db.GetAdjustedSRBill(companyCode, branchCode, SIGlobals.Globals.GetApplicationDate(companyCode, branchCode));
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }
        public dynamic GetAllSearchResult(string companyCode, string branchCode, string searchType, string searchValue)
        {
            ErrorVM searchError = new ErrorVM();
            System.Data.Entity.Core.Objects.ObjectResult<GetAdjustedSRBill_Result> data = GetAdjustedSRBill(companyCode, branchCode, out searchError);
            List<AttacheSRVM> result = GetObjectDetails(data);
            switch (searchType.ToUpper()) {
                case "SRNO":
                    return result.Where(search => search.SRNo == Convert.ToInt32(searchValue));
                case "DATE":
                    return result.Where(search => search.Date == Convert.ToString(searchValue));
                case "AMOUNT":
                    return result.Where(search => search.Amount == Convert.ToDecimal(searchValue));
            }
            return data;
        }
        public string SRBillDotMatrixPrint(string companyCode, string branchCode, int srBillNo, out ErrorVM error)
        {
            error = null;
            KTTU_SR_MASTER srMaster = db.KTTU_SR_MASTER.Where(sr => sr.sales_bill_no == srBillNo
                                                            && sr.company_code == companyCode
                                                            && sr.branch_code == branchCode).FirstOrDefault();
            if (srMaster == null) {
                error = new ErrorVM()
                {
                    field = "",
                    description = "Invalid Sales Return Bill No",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return "";
            }
            else if (srMaster.cflag == "Y") {
                error = new ErrorVM()
                {
                    field = "",
                    description = "Sales return does not exists.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return "";
            }
            List<KTTU_SR_DETAILS> srDet = db.KTTU_SR_DETAILS.Where(sr => sr.sales_bill_no == srBillNo
                                                                    && sr.company_code == companyCode
                                                                    && sr.branch_code == branchCode).ToList();
            KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(com => com.company_code == companyCode
                                                                    && com.branch_code == branchCode).FirstOrDefault();
            KSTU_CUSTOMER_MASTER customer = db.KSTU_CUSTOMER_MASTER.Where(cust => cust.cust_id == srMaster.cust_id
                                                                    && cust.company_code == companyCode
                                                                    && cust.branch_code == branchCode).FirstOrDefault();
            try {
                StringBuilder strLine = new StringBuilder();
                int width = 90;
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
                ;
                sb = SIGlobals.Globals.GetCompanyname(db, companyCode, branchCode);
                sb.AppendLine(string.Format("{0,-2}{1}", "", "Sales Return "));

                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));

                string metaltype = SIGlobals.Globals.GetMetalType(db, srMaster.gs_type.ToString(), companyCode, branchCode);
                sb.AppendLine(string.Format("{0,-15}{1,-10}{2,55}"
                 , "SR BiLL No", ": " + srMaster.branch_code.ToString() + "/" + "R" + "/" + srMaster.sales_bill_no.ToString()
                 , "Bill Date " + ":" + srMaster.bill_date + " SR Date " + ":" + srMaster.sr_date));
                sb.AppendLine(string.Format("{0,-15}{1,-65}"
                                   , "Customer Name", ": " + customer.salutation.ToString().Trim() + customer.cust_name.ToString().Trim()));

                if (!string.IsNullOrEmpty(customer.address1))
                    sb.AppendLine(string.Format("{0,-15}{1,-65}", "Address ", ": " + customer.address1.ToString().Trim()));
                if (!string.IsNullOrEmpty(customer.address2))
                    sb.AppendLine(string.Format("{0,-15}{1,-65}", "", ": " + customer.address2.ToString().Trim()));
                if (!string.IsNullOrEmpty(customer.address3))
                    sb.AppendLine(string.Format("{0,-15}{1,-65}", "", ": " + customer.address3.ToString().Trim()));
                if (!string.IsNullOrEmpty(customer.mobile_no))
                    sb.AppendLine(string.Format("{0,-15}{1,-65}", "", ": " + customer.mobile_no.ToString().Trim()));

                if (!string.IsNullOrEmpty(customer.Email_ID))
                    sb.AppendLine(string.Format("{0,-15}{1,-65}", "", ": " + customer.Email_ID.ToString().Trim()));

                if (!string.IsNullOrEmpty(customer.phone_no))
                    sb.AppendLine(string.Format("{0,-15}{1,-65}", "Phone", ": " + customer.phone_no.ToString().Trim()));


                if (!string.IsNullOrEmpty(customer.city) && !string.IsNullOrEmpty(customer.pin_code))
                    sb.AppendLine(string.Format("{0,-15}{1,-65}", "City", ": " + customer.city.Trim() + " - " + customer.pin_code.ToString().Trim()));
                else if (!string.IsNullOrEmpty(customer.city) && string.IsNullOrEmpty(customer.pin_code))
                    sb.AppendLine(string.Format("{0,-15}{1,-65}", "City", ": " + customer.city.ToString().Trim()));

                if (!string.IsNullOrEmpty(customer.state))
                    sb.AppendLine(string.Format("{0,-15}{1,-65}", "State", ": " + customer.state.ToString().Trim()));
                if (!string.IsNullOrEmpty(customer.state_code.ToString()))
                    sb.AppendLine(string.Format("{0,-15}{1,-65}", "State Code", ": " + customer.state_code.ToString().Trim()));


                if (!string.IsNullOrEmpty(customer.pan_no))
                    sb.AppendLine(string.Format("{0,-15}{1,-65}", "PAN", ": " + customer.pan_no.ToString().Trim()));
                else {
                    //string idquery = string.Format("select top(1) Doc_code,Doc_No from KSTU_CUSTOMER_ID_PROOF_DETAILS where cust_id='{0}' and company_code='{1}' and branch_code='{2}'", customer.cust_id.ToString(), companyCode, branchCode);
                    //DataTable IdDt = SIGlobals.Globals.ExecuteQuery(idquery);

                    KSTU_CUSTOMER_ID_PROOF_DETAILS idProof = db.KSTU_CUSTOMER_ID_PROOF_DETAILS.Where(id => id.cust_id == srMaster.cust_id
                                                                                                    && id.company_code == companyCode
                                                                                                    && id.branch_code == branchCode).FirstOrDefault();

                    if (idProof != null) {
                        if (!string.IsNullOrEmpty(idProof.Doc_code)) {
                            sb.AppendLine(string.Format("{0,-15}{1,-65}", idProof.Doc_code.ToString(), ": " + idProof.Doc_No.ToString()));
                        }
                    }
                }
                if (!string.IsNullOrEmpty(customer.tin))
                    sb.AppendLine(string.Format("{0,-15}{1,-65}", "GSTIN", ": " + customer.tin.ToString().Trim()));

                sb.Append(string.Format("{0,-14}{1,-10}", "Sales Bill No", " : " + srMaster.billed_branch.ToString() + "/" + "S" + "/" + srMaster.voture_bill_no.ToString()));
                if (string.Compare(metaltype, "GL") == 0)
                    sb.Append(string.Format("{0,15}{1}{2}", "Gold Rate : ", srMaster.rate.ToString(), "/gm"));
                else if (string.Compare(metaltype, "SL") == 0)
                    sb.Append(string.Format("{0,13}{1}{2}", "Silver Rate : ", srMaster.rate.ToString(), "/gm"));
                else if (string.Compare(metaltype, "PT") == 0)
                    sb.Append(string.Format("{0,10}{1}{2}", "Rate : ", srMaster.rate.ToString(), "/gm"));
                else if (string.Compare(metaltype, "DM") == 0)
                    sb.Append(string.Format("{0,10}{1}{2}", "Rate : ", srMaster.rate.ToString(), "   "));
                sb.AppendLine(string.Format("{0,10} ", ":#" + srMaster.operator_code.ToString()));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));
                sb.AppendLine(string.Format("{0,-13}{1,5}{2,3}{3,8}{4,8}{5,8}{6,10}{7,10}{8,10}{9,10}", "Item", "HSN ", "Qty", "Gr.Wt", "St.Wt", " N.Wt", "VA Amt", "St.Amt", "Gold Amt", "Amount"));
                sb.AppendLine(string.Format("{0,-13}{1,5}{2,3}{3,8}{4,8}{5,8}{6,10}{7,10}{8,10}{9,10}", "Name", "", "", "(gms)", "(gms)", "(gms)", "(Rs.Ps)", "(Rs.Ps)", "(Rs.Ps)", "(Rs.Ps)"));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));

                decimal taxAmount = 0;
                if (srMaster.tax_amt == 0) {
                    taxAmount = 0;
                }
                else {
                    taxAmount = srDet.Sum(sr => sr.net_amount);
                }

                decimal Amount = 0; decimal GrossAmount = 0, additionalDiscount = 0, mcdiscount = 0, SGSTAmt = 0, CGSTAmt = 0, IGSTAmt = 0;
                int Pcs = 0;
                decimal Gwt = 0, Swt = 0, Nwt = 0, GoldValue = 0, StoneChg = 0, ValueAmt = 0, TotalTaxable = 0, finalAmount = 0;
                decimal tax = taxAmount;

                for (int i = 0; i < srDet.Count; i++) {
                    sb.Append(string.Format("{0,-13}", SIGlobals.Globals.GetAliasName(db, srDet[i].gs_code.ToString(), srDet[i].item_name.ToString(), companyCode, branchCode)));
                    sb.Append(string.Format("{0,5}", srDet[i].HSN.ToString()));
                    sb.Append(string.Format("{0,3}", srDet[i].quantity.ToString()));
                    sb.Append(string.Format("{0,8}", (srDet[i].gwt + srDet[i].AddWt - srDet[i].DeductWt).ToString()));
                    sb.Append(string.Format("{0,8}", srDet[i].swt.ToString()));
                    sb.Append(string.Format("{0,8}", (srDet[i].nwt + srDet[i].AddWt - srDet[i].DeductWt).ToString()));

                    decimal MCAmount = 0;
                    if (Convert.ToDecimal(srDet[i].making_charge_per_rs) > 0) {
                        MCAmount = Convert.ToDecimal(srDet[i].nwt) * Convert.ToDecimal(srDet[i].making_charge_per_rs);
                    }
                    else if (Convert.ToDecimal(srDet[i].va_percent) > 0) {
                        MCAmount = (Convert.ToDecimal(srDet[i].sr_amount) * Convert.ToDecimal(srDet[i].va_percent)) / 100;
                    }
                    else {
                        MCAmount = Convert.ToDecimal(srDet[i].va_amount);
                    }
                    MCAmount = decimal.Round(MCAmount, 2);
                    ValueAmt += MCAmount;
                    GrossAmount = Convert.ToDecimal(srDet[i].sr_amount) + MCAmount + Convert.ToDecimal((srDet[i].stone_charges));
                    additionalDiscount += Convert.ToDecimal((srDet[i].item_additional_discount));
                    mcdiscount += Convert.ToDecimal(srDet[i].Mc_Discount_Amt);
                    TotalTaxable += Convert.ToDecimal(srDet[i].item_total_after_discount);

                    sb.Append(string.Format("{0,10}", MCAmount));
                    sb.Append(string.Format("{0,10}", (srDet[i].stone_charges + srDet[i].diamond_charges).ToString()));
                    sb.Append(string.Format("{0,10}", srDet[i].sr_amount.ToString()));
                    sb.Append(string.Format("{0,10}", GrossAmount));

                    sb.AppendLine(string.Format("{0}", " " + srMaster.sal_code));
                    Pcs += Convert.ToInt32(srDet[i].quantity);
                    Gwt += Convert.ToDecimal(srDet[i].gwt);
                    Swt += Convert.ToDecimal(srDet[i].swt);
                    Nwt += Convert.ToDecimal(srDet[i].nwt);
                    StoneChg += Convert.ToDecimal(srDet[i].stone_charges);
                    GoldValue += Convert.ToDecimal(srDet[i].sr_amount);
                    Amount += GrossAmount;
                    finalAmount += Convert.ToDecimal(srDet[i].item_final_amount.ToString());
                    if (srDet[i].SGST_Amount.ToString() != "NULL") {
                        SGSTAmt += Convert.ToDecimal(srDet[i].SGST_Amount);
                    }
                    if (srDet[i].CGST_Amount.ToString() != "NULL") {
                        CGSTAmt += Convert.ToDecimal(srDet[i].CGST_Amount);
                    }
                    if (srDet[i].IGST_Amount.ToString() != "NULL") {
                        IGSTAmt += Convert.ToDecimal(srDet[i].IGST_Amount);
                    }
                }
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                sb.AppendLine(string.Format("{0,-18}{1,3}{2,8}{3,8}{4,8}{5,10}{6,10}{7,10}{8,10}", "Total", Pcs, Gwt, Swt, Nwt, ValueAmt, StoneChg, GoldValue, Amount));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));

                //string DiamondDetails = string.Format("select qty,type,name,carrat,stone_wt,rate,amount from KTTU_SR_STONE_DETAILS where sales_bill_no = {0} and company_code = '{1}' and branch_code = '{2}'  ", Convert.ToInt32(srBillNo), companyCode, branchCode); //and type like 'D%'
                //DataTable dtStoneDiamondDetails = SIGlobals.Globals.ExecuteQuery(DiamondDetails);

                List<KTTU_SR_STONE_DETAILS> stoneDet = db.KTTU_SR_STONE_DETAILS.Where(st => st.sales_bill_no == srBillNo
                                                                                    && st.company_code == companyCode
                                                                                    && st.branch_code == branchCode).ToList();
                if (stoneDet != null && stoneDet.Count > 0) {
                    for (int j = 0; j < stoneDet.Count; j++) {
                        if (string.Compare(stoneDet[j].type.ToString(), "D") == 0 || string.Compare(stoneDet[j].type.ToString(), "DMD") == 0) {
                            sb.AppendLine(string.Format("{0}){1}/{2}*{3}={4}", j + 1, stoneDet[j].name.ToString(), stoneDet[j].carrat.ToString(), decimal.Round(Convert.ToDecimal(stoneDet[j].rate), 0), stoneDet[j].amount.ToString()));
                        }
                        else {
                            decimal stnAmount = Convert.ToDecimal(stoneDet[j].amount);
                            if (stnAmount > 0) {
                                sb.AppendLine(string.Format("{0}){1}/{2}", j + 1, stoneDet[j].name.ToString(), stnAmount));
                            }
                        }
                    }
                }
                if (Convert.ToDecimal(mcdiscount) > 0) {
                    sb.AppendLine(string.Format("{0,75}{1,13}", "Less  :", mcdiscount));
                }
                if (Convert.ToDecimal(additionalDiscount) > 0) {
                    sb.AppendLine(string.Format("{0,75}{1,13}", "Less  :", additionalDiscount));
                }
                if (Convert.ToDecimal(srMaster.excise_duty_amount) > 0) {
                    sb.AppendLine(string.Format("{0,75}{1,15}", "ED Amount :", srMaster.excise_duty_amount.ToString()));
                    Amount = Amount + Convert.ToDecimal(srMaster.excise_duty_amount);
                    if (Convert.ToDecimal(srMaster.ed_cess_amount) > 0) {
                        sb.AppendLine(string.Format("{0,75}{1,15}", "ED Cess :", srMaster.ed_cess_amount.ToString()));
                        Amount = Amount + Convert.ToDecimal(srMaster.ed_cess_amount);
                    }
                    if (Convert.ToDecimal(srMaster.hed_cess_amount) > 0) {
                        sb.AppendLine(string.Format("{0,75}{1,15}", "HED Cess :", srMaster.hed_cess_amount.ToString()));
                        Amount = Amount + Convert.ToDecimal(srMaster.hed_cess_amount);
                    }
                }
                sb.AppendLine(string.Format("{0,75}{1,13}", "Taxable Value :", TotalTaxable));
                sb.AppendLine(string.Format("{0,75}{1,13}", " SGST " + srDet[0].SGST_Percent + " %:", SGSTAmt));
                sb.AppendLine(string.Format("{0,75}{1,13}", " CGST " + srDet[0].CGST_Percent + " %:", CGSTAmt));
                sb.AppendLine(string.Format("{0,75}{1,13}", " IGST " + srDet[0].IGST_Percent + " %:", IGSTAmt));
                Amount = Math.Round(Amount, 0);
                sb.AppendLine(string.Format("{0,75}{1,13}", "Invoice Amount" + " :", Math.Round(finalAmount, 0, MidpointRounding.ToEven).ToString("F")));
                sb.AppendLine();

                int finYear = SIGlobals.Globals.GetFinancialYear(db, companyCode, branchCode);
                string refBilNo = Convert.ToString(srBillNo);
                List<KTTU_PAYMENT_DETAILS> payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.Ref_BillNo == refBilNo
                                                                                    && pay.company_code == pay.company_code
                                                                                    && pay.branch_code == branchCode
                                                                                    && pay.pay_mode == "SR"
                                                                                    && pay.fin_year == finYear).ToList();
                if (payment != null && payment.Count > 0) {
                    decimal purAmount = Convert.ToDecimal(payment[0].pay_amt);
                    purAmount = Math.Round(purAmount, 0, MidpointRounding.ToEven);
                    if ((string.Compare(payment[0].trans_type.ToString(), "S") == 0) && Convert.ToDecimal(payment[0].receipt_no) > 0)
                        sb.Append(string.Format("{0,70}", "Adjusted Towards Credit Receipt No: " + payment[0].receipt_no));
                    else if ((string.Compare(payment[0].trans_type.ToString(), "O") == 0) && Convert.ToDecimal(payment[0].receipt_no) > 0)
                        sb.Append(string.Format("{0,70}", "Adjusted Towards Order Receipt No: " + payment[0].receipt_no));
                    else
                        sb.AppendLine(string.Format("{0,70}", "Adjusted Towards Sales Bill No: " + companyCode + "/" + "S" + "/" + payment[0].series_no));
                    sb.AppendLine();
                }
                List<KTTU_PAYMENT_DETAILS> payDet = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == srBillNo
                                                                                && pay.trans_type == "RS"
                                                                                && pay.company_code == companyCode
                                                                                && pay.branch_code == branchCode
                                                                                && (pay.sal_bill_type == null || pay.sal_bill_type != "CR")
                                                                                && pay.pay_mode != "CT").ToList();
                if (payDet != null && payDet.Count > 0) {
                    for (int i = 0; i < payDet.Count; i++) {
                        decimal PayAmt = Convert.ToDecimal(payDet[i].pay_amt);
                        PayAmt = Math.Round(PayAmt, 0, MidpointRounding.ToEven);
                        if (string.Compare(payDet[i].pay_mode.ToString(), "Q") == 0) {
                            sb.AppendLine(string.Format("{0,75}{1,13}", "By Cheque Drawn on:/" + payDet[i].bank.ToString().Trim() + "/" + payDet[i].cheque_no.ToString().Trim() + ":", PayAmt.ToString("N")));
                        }
                        else if (string.Compare(payDet[i].pay_mode.ToString(), "C") == 0) {
                            sb.AppendLine(string.Format("{0,75}{1,13}", "Cash Paid : ", PayAmt.ToString("N")));
                        }
                        else if (string.Compare(payDet[i].pay_mode.ToString(), "EP") == 0) {
                            sb.AppendLine(string.Format("{0,75}{1,13}", "E-Payment : ", PayAmt.ToString("N")));
                        }
                    }
                    sb.AppendLine(string.Format("{0,75}{1,13}", "Balance: ", "Nill"));
                }
                sb.AppendLine();
                string strWords = string.Empty;
                NumberToWords objNWClass = new NumberToWords();
                objNWClass.ConvertNumberToWords(Convert.ToDouble(finalAmount), out strWords);
                strWords = strWords.Replace("Rupees", "");
                strWords = "Rupees " + strWords;
                sb.AppendLine(string.Format("{0,-95}", "" + strWords));

                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));
                sb.AppendLine();
                return sb.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        #endregion

        #region Private Methods
        private ErrorVM AccountPostingWithProedure(string companyCode, string branchCode, int billNo, MagnaDbEntities db)
        {
            //try {
            //    ObjectParameter errorMessage = new ObjectParameter("errorMsg", typeof(string));
            //    ObjectParameter outVal = new ObjectParameter("outValue", typeof(int));
            //    var data = db.usp_createSRPostingVouchers(companyCode, branchCode, Convert.ToString(billNo), "SR", outVal, errorMessage);
            //    string message = Convert.ToString(errorMessage.Value);
            //    if (data.Count() <= 0 || message != "") {
            //        return new ErrorVM() { description = "Error Occurred while Updating Accounts. " + errorMessage, field = "Account Update", index = 0 };
            //    }
            //}
            //catch (Exception excp) {
            //    return new ErrorVM() { description = excp.InnerException.Message, field = "Account Update", index = 0 };
            //}
            //return null;

            try {
                string errorFromProc = string.Empty;
                ObjectParameter errorMessage = new ObjectParameter("errorMsg", typeof(string));
                ObjectParameter outputVal = new ObjectParameter("outValue", typeof(int));
                var result = db.usp_createSRPostingVouchers_FuncImport(companyCode, branchCode, Convert.ToString(billNo), "SR", outputVal, errorMessage);
                return new CommonBL().HandleAccountPostingProcs(outputVal, errorMessage);
            }
            catch (Exception excp) {
                return new ErrorVM() { description = excp.InnerException.Message, field = "Account Update", index = 0 };
            }
        }
        private bool StockUpdate(string gsCode, int qty, decimal gwt, decimal nwt, decimal swt, decimal addWt,
            decimal deductWt, int addQty, int dedQty, string companyCode, string branchCode)
        {
            decimal grossWt = 0;
            decimal netWt = 0;
            decimal stoneWt = 0;
            int quantity = 0;
            KTTU_GS_SALES_STOCK gs = db.KTTU_GS_SALES_STOCK.Where(gst => gst.gs_code == gsCode
                                                                    && gst.branch_code == branchCode
                                                                    && gst.company_code == companyCode).FirstOrDefault();
            if (gs != null) {
                grossWt = gwt + (addWt - deductWt);
                netWt = nwt + (addWt - deductWt);
                stoneWt = swt + (addWt - deductWt);
                quantity = qty + (addQty - dedQty);


                gs.receipt_units += qty;
                gs.receipt_gwt += grossWt;
                gs.receipt_nwt += netWt;

                gs.closing_units += qty;
                gs.closing_gwt += gwt;
                gs.closing_nwt += nwt;
                db.Entry(gs).State = System.Data.Entity.EntityState.Modified;
                return true;
            }
            return false;
        }

        private bool CounterStockUpdate(string gsCode, string itemName, string Countercode, int qty, decimal gwt, decimal nwt, decimal swt, decimal addWt,
            decimal deductWt, int addQty, int dedQty, string companyCode, string branchCode)
        {
            KSTS_GS_ITEM_ENTRY gs = db.KSTS_GS_ITEM_ENTRY.Where(gst => gst.gs_code == gsCode
                                                                && gst.company_code == companyCode
                                                                && gst.branch_code == gst.branch_code).FirstOrDefault();
            if (gs != null && gs.bill_type == "S") {

                KTTU_COUNTER_STOCK kcs = db.KTTU_COUNTER_STOCK.Where(cs => cs.gs_code == gsCode
                                                                    && cs.item_name == itemName
                                                                    && cs.counter_code == Countercode
                                                                    && cs.company_code == companyCode
                                                                    && cs.branch_code == branchCode).FirstOrDefault();
                kcs.receipt_units += qty;
                kcs.receipt_gwt += gwt + (addWt - deductWt);
                kcs.receipt_swt += swt;
                kcs.receipt_nwt += nwt + (addWt - deductWt);
                kcs.closing_units += qty;
                kcs.closing_gwt += gwt + (addWt - deductWt);
                kcs.closing_swt += swt;
                kcs.closing_nwt += nwt + (addWt - deductWt);
                db.Entry(kcs).State = System.Data.Entity.EntityState.Modified;
            }
            return true;
        }

        private bool GSIssueStock(string gsCode, int qty, decimal gwt, decimal nwt, string companyCode, string branchCode)
        {
            KTTU_GS_SALES_STOCK gs = db.KTTU_GS_SALES_STOCK.Where(gst => gst.gs_code == gsCode && gst.company_code == companyCode && gst.branch_code == branchCode).FirstOrDefault();
            if (gs != null) {
                //gs.receipt_units -= qty;
                //gs.receipt_gwt -= gwt;
                //gs.receipt_nwt -= nwt;

                gs.issue_units += qty;
                gs.issue_gwt += gwt;
                gs.issue_nwt += nwt;

                gs.closing_units -= qty;
                gs.closing_gwt -= gwt;
                gs.closing_nwt -= nwt;
                db.Entry(gs).State = System.Data.Entity.EntityState.Modified;
            }
            else {
                return false;
            }
            return true;
        }

        private bool CancelCounterIssueStock(string gsCode, string itemName, string Countercode, int qty, decimal gwt, decimal nwt, decimal swt, decimal addWt,
           decimal deductWt, int addQty, int dedQty, string companyCode, string branchCode)
        {
            KSTS_GS_ITEM_ENTRY gs = db.KSTS_GS_ITEM_ENTRY.Where(gst => gst.gs_code == gsCode
                                                                && gst.company_code == companyCode
                                                                && gst.branch_code == gst.branch_code).FirstOrDefault();
            if (gs != null && gs.bill_type == "S") {

                KTTU_COUNTER_STOCK kcs = db.KTTU_COUNTER_STOCK.Where(cs => cs.gs_code == gsCode && cs.company_code == companyCode && cs.branch_code == branchCode).FirstOrDefault();
                kcs.receipt_units += qty;
                kcs.receipt_gwt += gwt + (addWt - deductWt);
                kcs.receipt_swt += swt;
                kcs.receipt_nwt += nwt + (addWt - deductWt);
                kcs.closing_units -= qty;
                kcs.closing_gwt -= gwt + (addWt - deductWt);
                kcs.closing_swt -= swt;
                kcs.closing_nwt -= nwt + (addWt - deductWt);
                db.Entry(kcs).State = System.Data.Entity.EntityState.Modified;
            }
            return true;
        }

        //public bool CounterStockUpdate(int billNo, string companyCode, string branchCode, out ErrorVM error)
        //{
        //    error = null;
        //    var data = from pay in db.KTTU_SR_DETAILS
        //               where pay.sales_bill_no == billNo && pay.company_code == companyCode && pay.branch_code == branchCode
        //               group pay by new
        //               {
        //                   pay.gs_code,
        //                   pay.item_name

        //               } into g
        //               select new StockJournalVM()
        //               {
        //                   StockTransType = StockTransactionType.Issue,
        //                   CompanyCode = companyCode,
        //                   BranchCode = branchCode,
        //                   GS = g.Key.gs_code,
        //                   Item = g.Key.item_name,
        //                   Qty = g.Sum(x => x.quantity),
        //                   GrossWt = g.Sum(x => x.gwt),
        //                   StoneWt = g.Sum(x => x.swt),
        //                   NetWt = g.Sum(x => x.nwt)
        //               };
        //    string errorMsg = string.Empty;
        //    bool stockUpdated = new StockPostBL().GSStockPost(db, data.ToList(), out errorMsg);
        //    if (!stockUpdated) {
        //        error = new ErrorVM()
        //        {
        //            description = errorMsg,
        //            ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError,
        //            customDescription = "Error while updting Purchase Stock"
        //        };
        //        return false;
        //    }
        //    return true;
        //}

        private List<AttacheSRVM> GetObjectDetails(System.Data.Entity.Core.Objects.ObjectResult<GetAdjustedSRBill_Result> data)
        {
            List<AttacheSRVM> lstAttachSR = new List<AttacheSRVM>();
            foreach (GetAdjustedSRBill_Result result in data) {
                AttacheSRVM attach = new AttacheSRVM();
                attach.SRNo = result.SRNo;
                attach.Date = result.Date;
                attach.Amount = Convert.ToDecimal(result.Amount);
                lstAttachSR.Add(attach);
            }
            return lstAttachSR;
        }

        public bool CheckIsCreditBillWithBalance(string companyCode, string branchCode, int billNo, out decimal masterBalaceAmt, out decimal partialPaidAmt)
        {
            KTTU_SALES_MASTER sales = db.KTTU_SALES_MASTER.Where(sal => sal.company_code == companyCode && sal.branch_code == branchCode
                                                                     && sal.bill_no == billNo && sal.cflag != "Y").FirstOrDefault();
            masterBalaceAmt = 0;
            partialPaidAmt = 0;
            bool isCreditBill = false;
            if (sales != null) {
                isCreditBill = sales.iscredit == "Y" ? true : false;
                if (isCreditBill) {
                    //masterBalaceAmt = Convert.ToDecimal(sales.balance_amt);
                    masterBalaceAmt = Convert.ToDecimal(sales.grand_total);
                    List<KTTU_PAYMENT_DETAILS> partialPayment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.company_code == companyCode
                                                                            && pay.branch_code == branchCode
                                                                            && pay.series_no == billNo
                                                                            && pay.trans_type == "S"
                                                                            && pay.cflag == "N").ToList();
                    if (partialPayment != null) {
                        partialPaidAmt = Convert.ToDecimal(partialPayment.Sum(pay => pay.pay_amt));
                        if (partialPaidAmt > 0) {
                            masterBalaceAmt = masterBalaceAmt - partialPaidAmt;
                        }
                    }
                }
            }
            return isCreditBill;
        }
        #endregion

        #region Public Methods
        public bool ValidateSalesReturnBillNo(int billNo, string companyCode, string branchCode, out ErrorVM error)
        {
            // This is alternate to [dbo].[usp_ValidateSRBillNo] Procedure.
            error = null;
            if (billNo > 0) {
                string strBillNo = Convert.ToString(billNo);
                KTTU_SR_MASTER salesReturn = db.KTTU_SR_MASTER.Where(sr => sr.sales_bill_no == billNo
                                                                            && sr.company_code == companyCode
                                                                            && sr.branch_code == branchCode).FirstOrDefault();

                var payments = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.Ref_BillNo == strBillNo
                                                                            && pay.company_code == companyCode
                                                                            && pay.branch_code == branchCode).ToList();
                if (salesReturn == null) {
                    error = new ErrorVM();
                    error.description = string.Format("Sales Return Bill No: {0} is invalid.", billNo);
                    error.ErrorStatusCode = System.Net.HttpStatusCode.NotFound;
                    return false;
                }

                if (salesReturn.cflag == "Y") {
                    error = new ErrorVM();
                    error.description = string.Format("Sales Return Bill No: {0} is already cancelled.", billNo);
                    error.ErrorStatusCode = System.Net.HttpStatusCode.NotFound;
                    return false;
                }

                if (payments.Count > 0) {
                    var payDone = payments.Where(pay => pay.trans_type == "S" && pay.pay_mode == "SR" && pay.cflag == "N").FirstOrDefault();
                    if (payDone != null) {
                        if (payDone.series_no != 0) {
                            error.description = string.Format("Sales Return Bill No: {0} is already adjusted towards {1} Sales Bill No: ", billNo, payDone.series_no);
                            error.ErrorStatusCode = System.Net.HttpStatusCode.NotFound;
                            return false;
                        }
                    }
                    payDone = payments.Where(pay => pay.trans_type == "O" && pay.pay_mode == "SR" && pay.cflag == "N").FirstOrDefault();
                    if (payDone != null) {
                        if (payDone.series_no != 0) {
                            error = new ErrorVM();
                            error.description = string.Format("Sales Return Bill No: {0} is already adjusted towards {1} Order No: ", billNo, payDone.series_no);
                            error.ErrorStatusCode = System.Net.HttpStatusCode.NotFound;
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        #endregion
    }
}
