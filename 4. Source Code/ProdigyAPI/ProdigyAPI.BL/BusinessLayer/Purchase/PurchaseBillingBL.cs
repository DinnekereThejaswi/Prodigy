using NumberToWordsINR;
using ProdigyAPI.BL.BusinessLayer.Common;
using ProdigyAPI.BL.BusinessLayer.PrintConfig;
using ProdigyAPI.BL.BusinessLayer.Stock;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.OldPurchase;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.BL.ViewModel.Payment;
using ProdigyAPI.BL.ViewModel.Print;
using ProdigyAPI.Model.MagnaDb;
using ProdigyAPI.SIGlobals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Purchase
{
    public class PurchaseBillingBL
    {
        #region Declaration
        MagnaDbEntities db = null;
        private CultureInfo indianDefaultCulture = new CultureInfo("hi-IN");
        private const string MODULE_SEQ_NO = "9";
        private const string TABLE_NAME = "KTTU_PURCHASE_MASTER";
        #endregion

        #region Constructor
        public PurchaseBillingBL()
        {
            db = new MagnaDbEntities(true);
        }

        public PurchaseBillingBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }
        #endregion

        #region Methods
        public List<PurchaseBillingVM> GetAllPurchaseBill(string companyCode, string branchCode, DateTime date, bool isCancelled, out ErrorVM error)
        {
            error = null;
            try {
                IQueryable<PurchaseBillingVM> data = null;
                if (isCancelled) {
                    data = from pur in db.KTTU_PURCHASE_MASTER
                           where pur.company_code == companyCode
                           && pur.branch_code == branchCode
                           && pur.cflag == "Y"
                           && System.Data.Entity.DbFunctions.TruncateTime(pur.p_date) == System.Data.Entity.DbFunctions.TruncateTime(date)
                           select new PurchaseBillingVM()
                           {
                               BillNo = pur.bill_no,
                               CustName = pur.cust_name
                           };
                }
                else {
                    data = from pur in db.KTTU_PURCHASE_MASTER
                           where pur.company_code == companyCode && pur.branch_code == branchCode && pur.cflag != "Y"
                           && System.Data.Entity.DbFunctions.TruncateTime(pur.p_date) == System.Data.Entity.DbFunctions.TruncateTime(date)
                           select new PurchaseBillingVM()
                           {
                               BillNo = pur.bill_no,
                               CustName = pur.cust_name
                           };
                }
                return data.OrderByDescending(d => d.BillNo).ToList();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }
        public PurchaseBillMasterVM GetPurchaseDetails(string companyCode, string branchCode, int billNo, out ErrorVM error)
        {
            error = null;
            try {
                KTTU_PURCHASE_MASTER master = db.KTTU_PURCHASE_MASTER.Where(p => p.company_code == companyCode
                                                                            && p.branch_code == branchCode
                                                                            && p.bill_no == billNo).FirstOrDefault();
                if (master == null || master.cflag == "Y") {
                    error = new ErrorVM()
                    {
                        index = 0,
                        description = "Invalid Bill Number.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                        field = "Blll Number."
                    };
                    return null;
                }
                PurchaseBillMasterVM purchase = new PurchaseBillMasterVM();
                purchase.PDate = master.p_date;
                purchase.CustName = master.cust_name;
                purchase.Address1 = master.address1;
                purchase.Address2 = master.address2;
                purchase.Address3 = master.address3;
                purchase.Mobile_no = master.mobile_no;
                purchase.TotalPurchaseAmount = master.total_purchase_amount;
                purchase.TotalTaxAmount = master.total_tax_amount;
                return purchase;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public int SavePurchaseBillWithTxn(PurchaseBillingVM purchase, out ErrorVM error)
        {
            error = null;
            bool isOwnTransaction = false;
            if (db.Database.CurrentTransaction == null) {
                db.Database.BeginTransaction();
                isOwnTransaction = true;
            }

            int functionResult = SavePurchaseBill(purchase, out error);
            if (functionResult == 0) {
                if (isOwnTransaction)
                    db.Database.CurrentTransaction.Rollback();
            }
            else {
                if (isOwnTransaction)
                    db.Database.CurrentTransaction.Commit();
            }

            return functionResult;


        }

        private int SavePurchaseBill(PurchaseBillingVM purchase, out ErrorVM error)
        {
            error = null;
            int billNo = 0;
            int finYear = SIGlobals.Globals.GetFinancialYear(db, purchase.CompanyCode, purchase.BranchCode);
            ErrorVM purchaseError = new ErrorVM();
            KTTU_PURCHASE_EST_MASTER purchaseEst = db.KTTU_PURCHASE_EST_MASTER.Where(pur => pur.est_no == purchase.EstNo
                                                                                    && pur.company_code == purchase.CompanyCode
                                                                                    && pur.branch_code == purchase.BranchCode).FirstOrDefault();
            if (purchaseEst == null) {
                error = new ErrorVM()
                {
                    index = 0,
                    description = "Invalid Estimation Number.",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound,
                    field = "Estimation Number."
                };
                return 0;
            }
            if (purchaseEst.bill_no != 0) {
                error = new ErrorVM()
                {
                    index = 0,
                    description = "Purchase Estimation No. " + purchaseEst.est_no + " is already Billed. Bill No. " + purchaseEst.bill_no,
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                    field = "Estimation Number."
                };
                return 0;
            }

            if (purchase.lstOfPayment != null && purchase.lstOfPayment.Count > 0) {
                decimal totalPaidAmt = 0;
                foreach (PaymentVM pay in purchase.lstOfPayment) {
                    totalPaidAmt = totalPaidAmt + Convert.ToDecimal(pay.PayAmount);
                }

                if (purchase.PaidAmount > 0) {
                    if (purchase.PaidAmount != totalPaidAmt) {
                        error = new ErrorVM()
                        {
                            index = 0,
                            description = "Paid Amount should be equal to Trade in Amount",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                        };
                        return 0;
                    }
                }


            }

            // Validating of Cash Payment to Customer with Tollerance Master
            if (purchase.lstOfPayment != null && purchase.lstOfPayment.Count > 0) {
                foreach (PaymentVM pvm in purchase.lstOfPayment) {
                    if (pvm.PayMode == "C") {
                        KSTU_TOLERANCE_MASTER ktm = db.KSTU_TOLERANCE_MASTER.Where(kt => kt.obj_id == 6
                                                                                    && kt.company_code == purchase.CompanyCode
                                                                                    && kt.branch_code == purchase.BranchCode).FirstOrDefault();
                        if (pvm.PayAmount > ktm.Max_Val) {
                            error = new ErrorVM()
                            {
                                field = "Amount",
                                index = 0,
                                description = "Cash payment should not be more than " + ktm.Max_Val + "/-",
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                            };
                            return 0;
                        }
                    }
                }
            }

            billNo = Convert.ToInt32(SIGlobals.Globals.GetFinancialYear(db, purchase.CompanyCode, purchase.BranchCode).ToString().Remove(0, 1) +
                                     db.KSTS_SEQ_NOS.Where(sq => sq.obj_id == MODULE_SEQ_NO
                                                            && sq.company_code == purchase.CompanyCode
                                                            && sq.branch_code == purchase.BranchCode).FirstOrDefault().nextno);
            string objectID = string.Empty;
            objectID = SIGlobals.Globals.GetMagnaGUID(TABLE_NAME, billNo, purchase.CompanyCode, purchase.BranchCode);
            List<KTTU_PURCHASE_EST_DETAILS> lstOfPurchaseDet = db.KTTU_PURCHASE_EST_DETAILS.Where(pur => pur.est_no == purchase.EstNo
                                                                                    && pur.company_code == purchase.CompanyCode
                                                                                    && pur.branch_code == purchase.BranchCode).ToList();

            // Checking the Purchase is going to happen on Only Purchase Screen,If its true need to create new begin transaction
            // else its coming from sales then it uses the existing transaction only.
            //bool isUnderOwnTran = false;
            //if (db.Database.CurrentTransaction == null) {
            //    isUnderOwnTran = true;
            //}
            //var transaction = db.Database.CurrentTransaction == null ? db.Database.BeginTransaction() : db.Database.CurrentTransaction;

            //if (isUnderOwnTran) {
            //    transaction.Commit();
            //}
            //if (isUnderOwnTran) {
            //    transaction.Rollback();
            //}

            try {
                #region Purchase Master
                KTTU_PURCHASE_MASTER kpm = new KTTU_PURCHASE_MASTER();
                kpm.obj_id = objectID;
                kpm.company_code = purchaseEst.company_code;
                kpm.branch_code = purchaseEst.branch_code;
                kpm.bill_no = billNo;
                kpm.est_no = purchaseEst.est_no;
                kpm.cust_id = purchaseEst.cust_id;
                kpm.cust_name = purchaseEst.cust_name;
                kpm.p_date = purchaseEst.p_date;
                kpm.tax = purchaseEst.tax;
                kpm.operator_code = purchaseEst.operator_code;
                kpm.grand_total = purchaseEst.grand_total;
                kpm.p_type = purchaseEst.p_type;
                kpm.today_rate = 0;
                kpm.cflag = "N";
                kpm.cancelled_by = null;
                kpm.bill_counter = purchase.BillCounter;
                kpm.balance_amt = 0;
                kpm.UpdateOn = SIGlobals.Globals.GetDateTime();
                kpm.cancelled_remarks = "";
                kpm.CustomerAddress1 = "";
                kpm.CustomerAddress2 = "";
                kpm.pur_item = purchaseEst.pur_item;
                kpm.scheme_duratation = purchaseEst.scheme_duratation;
                kpm.salutation = purchaseEst.salutation;
                kpm.round_off = 0;
                kpm.total_tax_amount = 0;
                kpm.total_purchase_amount = purchaseEst.total_purchase_amount;
                kpm.invoice_type = 0;
                kpm.New_Bill_No = Convert.ToString(billNo);
                kpm.ShiftID = 0;
                kpm.isPAN = purchaseEst.isPAN;
                kpm.purchase_amount = purchaseEst.total_purchase_amount;
                kpm.UniqRowID = Guid.NewGuid();
                kpm.address1 = purchaseEst.address1;
                kpm.address2 = purchaseEst.address2;
                kpm.address3 = purchaseEst.address3;
                kpm.city = purchaseEst.city;
                kpm.pin_code = purchaseEst.pin_code;
                kpm.mobile_no = purchaseEst.mobile_no;
                kpm.state = purchaseEst.state;
                kpm.state_code = purchaseEst.state_code;
                kpm.tin = purchaseEst.tin;
                kpm.pan_no = purchaseEst.pan_no;
                kpm.store_location_id = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == purchaseEst.company_code
                                                                                && c.branch_code == purchaseEst.branch_code).FirstOrDefault().store_location_id;
                db.KTTU_PURCHASE_MASTER.Add(kpm);
                #endregion

                #region Purchase Details
                int slno = 1;
                foreach (KTTU_PURCHASE_EST_DETAILS kped in lstOfPurchaseDet) {
                    KTTU_PURCHASE_DETAILS ped = new KTTU_PURCHASE_DETAILS();
                    ped.obj_id = objectID;
                    ped.company_code = kped.company_code;
                    ped.branch_code = kped.branch_code;
                    ped.bill_no = billNo;
                    ped.est_no = kped.est_no;
                    ped.sl_no = slno;
                    ped.item_name = kped.item_name;
                    ped.item_no = kped.item_no;
                    ped.gwt = kped.gwt;
                    ped.swt = kped.swt;
                    ped.nwt = kped.nwt;
                    ped.melting_percent = kped.melting_percent;
                    ped.melting_loss = kped.melting_loss;
                    ped.purchase_rate = kped.purchase_rate;
                    ped.diamond_amount = kped.diamond_amount;
                    ped.gold_amount = kped.gold_amount;
                    ped.item_amount = kped.item_amount;
                    ped.sal_code = kped.sal_code;
                    ped.gs_code = kped.gs_code;
                    ped.UpdateOn = SIGlobals.Globals.GetDateTime();
                    ped.va_amount = 0;
                    ped.purity_per = kped.purity_per;
                    ped.convertion_wt = kped.convertion_wt;
                    ped.item_description = "";
                    ped.Fin_Year = kped.Fin_Year;
                    ped.item_description = kped.item_description;
                    ped.itemwise_tax_percentage = kped.itemwise_tax_percentage;
                    ped.itemwise_tax_amount = kped.itemwise_tax_amount;
                    ped.itemwise_purchase_amount = kped.itemwise_purchase_amount;
                    ped.invoice_type = kped.invoice_type;
                    ped.ratededuct = kped.ratededuct;
                    ped.GSTGroupCode = kped.GSTGroupCode;
                    ped.SGST_Percent = kped.SGST_Percent;
                    ped.SGST_Amount = kped.SGST_Amount;
                    ped.CGST_Percent = kped.CGST_Percent;
                    ped.CGST_Amount = kped.CGST_Amount;
                    ped.IGST_Percent = kped.IGST_Percent;
                    ped.IGST_Amount = kped.IGST_Amount;
                    ped.HSN = kped.HSN;
                    ped.UniqRowID = Guid.NewGuid();
                    db.KTTU_PURCHASE_DETAILS.Add(ped);

                    //Stock Update
                    bool stockUpdate = StockUpdate(ped.gs_code, ped.item_no, ped.gwt, ped.nwt, ped.company_code, ped.branch_code);
                    if (!stockUpdate) {
                        error = new ErrorVM()
                        {
                            description = "Failed in Stock Update",
                            field = "Stock Update",
                            ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError
                        };
                        return 0;
                    }

                    if (purchase.EstNo <= 0) {
                        error = new ErrorVM()
                        {
                            description = "Estimation number cannot be ZERO.",
                            field = "Stock Update",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return 0;
                    }
                    List<KTTU_PURCHASE_STONE_DETAILS> lstOfPurchaseStone = db.KTTU_PURCHASE_STONE_DETAILS.Where(pur => pur.est_no == purchase.EstNo
                                                                                       && pur.item_sno == ped.sl_no
                                                                                       && pur.company_code == purchase.CompanyCode
                                                                                       && pur.branch_code == purchase.BranchCode).ToList();
                    // Removing Existing 
                    foreach (KTTU_PURCHASE_STONE_DETAILS kpsd in lstOfPurchaseStone) {
                        db.KTTU_PURCHASE_STONE_DETAILS.Remove(kpsd);
                    }
                    // Adding Again
                    int sno = 1;
                    foreach (KTTU_PURCHASE_STONE_DETAILS kpsd in lstOfPurchaseStone) {
                        KTTU_PURCHASE_STONE_DETAILS psd = new KTTU_PURCHASE_STONE_DETAILS();
                        psd.obj_id = objectID;
                        psd.company_code = kpsd.company_code;
                        psd.branch_code = kpsd.branch_code;
                        psd.bill_no = billNo;
                        psd.sno = sno;
                        psd.est_no = kpsd.est_no;
                        psd.item_sno = kpsd.item_sno;
                        psd.type = kpsd.type;
                        psd.name = kpsd.name;
                        psd.qty = kpsd.qty;
                        psd.carrat = kpsd.carrat;
                        psd.rate = kpsd.rate;
                        psd.amount = kpsd.amount;
                        psd.pur_dealer_bill_no = kpsd.pur_dealer_bill_no;
                        psd.gs_code = kpsd.gs_code;
                        psd.pur_return_no = kpsd.pur_return_no;
                        psd.UpdateOn = kpsd.UpdateOn;
                        psd.Fin_Year = kpsd.Fin_Year;
                        psd.UniqRowID = Guid.NewGuid();
                        db.KTTU_PURCHASE_STONE_DETAILS.Add(psd);

                        //Updating to Stock
                        bool stoneStock = StoneStockUpdate(psd.name, psd.gs_code, psd.qty, psd.carrat, psd.carrat, purchase.CompanyCode, purchase.BranchCode);
                        if (!stoneStock) {
                            error = new ErrorVM()
                            {
                                description = "Failed in Stock Update",
                                field = "Stock Update",
                                ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError
                            };
                            return 0;
                        }
                        sno = sno + 1;
                    }
                    db.KTTU_PURCHASE_DETAILS.Add(ped);
                    slno = slno + 1;
                }
                #endregion

                #region PaymentDetails
                int paySlNo = 1;
                if (purchase.Type == "SE") {
                    KTTU_PAYMENT_DETAILS kpd = new KTTU_PAYMENT_DETAILS();
                    kpd.obj_id = purchase.ObjID; //Object ID should be same as Sales Object ID otherwise attached Purchase details wil won't display in report.
                    kpd.company_code = purchase.CompanyCode;
                    kpd.branch_code = purchase.BranchCode;
                    kpd.series_no = purchase.BillNo;
                    kpd.receipt_no = 0;
                    kpd.sno = purchase.SlNo;
                    kpd.trans_type = "S";
                    kpd.pay_mode = "PB";
                    kpd.pay_details = "Purchase Bill";
                    kpd.pay_date = SIGlobals.Globals.GetDateTime();
                    kpd.pay_amt = purchase.PaidAmount;
                    kpd.Ref_BillNo = Convert.ToString(billNo);
                    kpd.party_code = null;
                    kpd.bill_counter = purchase.BillCounter;
                    kpd.is_paid = "N";
                    kpd.bank = null;
                    kpd.bank_name = null;
                    kpd.cheque_date = SIGlobals.Globals.GetApplicationDate(purchase.CompanyCode, purchase.BranchCode);
                    kpd.card_type = null;
                    kpd.expiry_date = SIGlobals.Globals.GetApplicationDate(purchase.CompanyCode, purchase.BranchCode);
                    kpd.cflag = "N";
                    kpd.card_app_no = null;
                    kpd.scheme_code = null;
                    kpd.sal_bill_type = null;
                    kpd.operator_code = purchase.OperatorCode;
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
                    kpd.pay_amount_before_tax = 0;
                    kpd.pay_tax_amount = 0;
                    kpd.UniqRowID = Guid.NewGuid();
                    if (kpd.pay_mode == "CT") {
                        int memebershipNo = Convert.ToInt32(kpd.Ref_BillNo);
                        CHTU_CHIT_CLOSURE cc = db.CHTU_CHIT_CLOSURE.Where(ccl => ccl.Scheme_Code == kpd.scheme_code && ccl.Group_Code == kpd.group_code && ccl.branch_code == kpd.ct_branch && ccl.Chit_MembShipNo == memebershipNo).FirstOrDefault();
                        kpd.amt_received = cc.Amt_Received;
                        kpd.bonus_amt = cc.Bonus_Amt;
                        kpd.win_amt = cc.win_amt;
                        cc.Bill_Number = Convert.ToString(billNo);
                        db.Entry(cc).State = System.Data.Entity.EntityState.Modified;
                    }
                    db.KTTU_PAYMENT_DETAILS.Add(kpd);
                    paySlNo = paySlNo + 1;
                }
                else if (purchase.Type == "PE") {
                    foreach (PaymentVM pvm in purchase.lstOfPayment) {
                        KTTU_PAYMENT_DETAILS kpd = new KTTU_PAYMENT_DETAILS();
                        kpd.obj_id = objectID;
                        kpd.company_code = pvm.CompanyCode;
                        kpd.branch_code = pvm.BranchCode;
                        kpd.series_no = billNo;
                        kpd.receipt_no = 0;
                        kpd.sno = paySlNo;
                        kpd.trans_type = "P";
                        kpd.pay_mode = pvm.PayMode;
                        kpd.pay_details = pvm.PayDetails;
                        kpd.pay_date = SIGlobals.Globals.GetApplicationDate(purchase.CompanyCode, purchase.BranchCode);
                        kpd.pay_amt = pvm.PayAmount;
                        kpd.Ref_BillNo = pvm.RefBillNo;
                        kpd.party_code = pvm.CTBranch;
                        kpd.bill_counter = pvm.BillCounter;
                        kpd.is_paid = pvm.IsPaid;
                        kpd.bank = SIGlobals.Globals.GetBank(db, pvm.Bank, pvm.PayMode, pvm.CompanyCode, pvm.BranchCode);
                        kpd.bank_name = pvm.BankName;
                        kpd.cheque_date = pvm.ChequeDate;
                        kpd.card_type = pvm.CardType;
                        kpd.expiry_date = pvm.ExpiryDate;
                        kpd.cflag = "N";
                        kpd.card_app_no = pvm.CardAppNo;
                        kpd.scheme_code = pvm.SchemeCode;
                        kpd.sal_bill_type = pvm.SalBillType;
                        kpd.operator_code = pvm.OperatorCode;
                        kpd.session_no = pvm.SessionNo;
                        kpd.UpdateOn = SIGlobals.Globals.GetDateTime();
                        kpd.group_code = pvm.GroupCode;
                        kpd.amt_received = pvm.AmtReceived;
                        kpd.bonus_amt = pvm.BonusAmt == null ? 0 : pvm.BonusAmt;
                        kpd.win_amt = pvm.WinAmt == null ? 0 : pvm.BonusAmt; ;
                        kpd.ct_branch = pvm.CTBranch;
                        kpd.fin_year = finYear;
                        kpd.CardCharges = pvm.CardCharges;
                        kpd.cheque_no = pvm.ChequeNo;
                        kpd.cheque_no = pvm.PayMode == "R" ? pvm.Bank : pvm.ChequeNo;
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

                        if (kpd.pay_mode == "CT") {
                            int memebershipNo = Convert.ToInt32(kpd.Ref_BillNo);
                            CHTU_CHIT_CLOSURE cc = db.CHTU_CHIT_CLOSURE.Where(ccl => ccl.Scheme_Code == kpd.scheme_code && ccl.Group_Code == kpd.group_code && ccl.branch_code == kpd.ct_branch && ccl.Chit_MembShipNo == memebershipNo).FirstOrDefault();
                            kpd.amt_received = cc.Amt_Received;
                            kpd.bonus_amt = cc.Bonus_Amt;
                            kpd.win_amt = cc.win_amt;
                            cc.Bill_Number = Convert.ToString(billNo);
                            db.Entry(cc).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                }
                #endregion

                #region Account Updates
                db.SaveChanges();
                // After doing db.SaveChanges() only we can get Issue Number in the db to do account posting.
                // Account Update
                ErrorVM procError = AccountPostingWithProedure(purchase.CompanyCode, purchase.BranchCode, billNo, db);
                if (procError != null) {
                    error = new ErrorVM()
                    {
                        field = "Account Update",
                        index = 0,
                        description = procError.description,
                        ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError
                    };
                    return 0;
                }
                #endregion

                #region Updating to Estimation
                //Updaing to Purchase Estimation Master
                purchaseEst.bill_no = billNo;
                purchaseEst.UpdateOn = SIGlobals.Globals.GetDateTime();
                db.Entry(purchaseEst).State = System.Data.Entity.EntityState.Modified;
                #endregion

                #region Updating Order Sequence
                // Updating Sequence Number
                Globals.UpdateSeqenceNumber(db, MODULE_SEQ_NO, purchase.CompanyCode, purchase.BranchCode);
                db.SaveChanges();
                #endregion
                

                #region Document Creation Posting, error will not be checked
                //Post to DocumentCreationLog Table
                DateTime billDate = SIGlobals.Globals.GetApplicationDate(purchase.CompanyCode, purchase.BranchCode);
                new Common.CommonBL().PostDocumentCreation(purchase.CompanyCode, purchase.BranchCode, 2, billNo, billDate, purchaseEst.operator_code);
                #endregion

                return billNo;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return 0;
            }
            finally {

            }
        }
        public bool CancelPurchaseBill(PurchaseBillingVM purchase, out ErrorVM error)
        {
            error = null;
            return CancelPurchaseBill(purchase.CompanyCode, purchase.BranchCode, purchase.BillNo,
                purchase.CancelRemarks, purchase.OperatorCode, out error);
        }
        public bool CancelPurchaseBill(string companyCode, string branchCode, int billNo, string remarks, string userId, out ErrorVM error)
        {
            error = null;
            try {

                #region Updating to Purchase Master
                KTTU_PURCHASE_MASTER cancelPurchase = db.KTTU_PURCHASE_MASTER.Where(pur => pur.bill_no == billNo
                                                                               && pur.company_code == companyCode
                                                                               && pur.branch_code == branchCode).FirstOrDefault();
                if (cancelPurchase.cflag == "Y") {
                    error = new ErrorVM()
                    {
                        description = "Invalid Purchase Bill Number, the purchase bill is already cancelled.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
                cancelPurchase.cflag = "Y";
                cancelPurchase.cancelled_by = userId;
                cancelPurchase.cancelled_remarks = remarks;
                cancelPurchase.UpdateOn = SIGlobals.Globals.GetDateTime();
                db.Entry(cancelPurchase).State = System.Data.Entity.EntityState.Modified;
                #endregion

                #region Delete Stone Details
                // Deleting Purchased Stone Details.
                List<KTTU_PURCHASE_STONE_DETAILS> stoneDet = db.KTTU_PURCHASE_STONE_DETAILS.Where(stone => stone.bill_no == billNo
                                                                                             && stone.company_code == companyCode
                                                                                             && stone.branch_code == branchCode).ToList();
                if (stoneDet != null && stoneDet.Count > 0) {
                    db.KTTU_PURCHASE_STONE_DETAILS.RemoveRange(stoneDet);
                }
                #endregion

                #region Update to Payment Details
                // Payment Details
                List<KTTU_PAYMENT_DETAILS> lstOfPayments = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == billNo
                                                                                && pay.trans_type == "P"
                                                                                && pay.company_code == companyCode
                                                                                && pay.branch_code == branchCode).ToList();
                foreach (KTTU_PAYMENT_DETAILS pay in lstOfPayments) {
                    pay.cflag = "Y";
                    pay.cancelled_by = userId;
                    pay.cancelled_remarks = remarks;
                    pay.UpdateOn = SIGlobals.Globals.GetDateTime();
                    db.Entry(cancelPurchase).State = System.Data.Entity.EntityState.Modified;
                }

                #endregion

                #region Update to Estimation
                //Check If Estimation Exists, Enable it by set bill number to 0.
                KTTU_PURCHASE_EST_MASTER kpem = db.KTTU_PURCHASE_EST_MASTER.Where(psm => psm.est_no == cancelPurchase.est_no
                                                                                    && psm.company_code == companyCode
                                                                                    && psm.branch_code == branchCode).FirstOrDefault();

                kpem.cflag = "N";
                kpem.bill_no = 0;
                db.Entry(kpem).State = System.Data.Entity.EntityState.Modified;
                #endregion

                #region Do Stock Update
                //Stock Update
                ErrorVM stockErrror = new ErrorVM();
                bool updated = CancelStockUpdate(billNo, companyCode, branchCode, true, out stockErrror);
                if (!updated) {
                    error = stockErrror;
                    return false;
                }
                #endregion

                #region Account Updates
                db.SaveChanges();
                // After doing db.SaveChanges() only we can get Issue Number in the db to do account posting.
                // Account Update
                //ErrorVM procError = CancelAccountPostingWithProcedure(companyCode, branchCode, purchase.BillNo);
                //if (procError != null) {
                //    error = new ErrorVM()
                //    {
                //        field = "Account Update",
                //        index = 0,
                //        description = procError.description,
                //        ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError
                //    };
                //    return false;
                //}

                //string sql = "DELETE  \n"
                //           + "FROM   KTTU_STOCK_TAKING \n"
                //           + "WHERE  company_code = @p0 \n"
                //           + "       AND branch_code = @p1 \n"
                //           + "       AND batch_no = @p2";

                string sql = " UPDATE KSTU_ACC_SUBSIDIARY_VOUCHER_TRANSACTIONS SET cflag ='Y',cancelled_by=@p0,cancelled_remarks =@p1" +
                             " where bill_no=@p2 and trans_type='Pur-C' and company_code =@p3 and branch_code=@p4";
                List<object> parameterList = new List<object>();
                parameterList.Add(userId);
                parameterList.Add(remarks);
                parameterList.Add(billNo);
                parameterList.Add(companyCode);
                parameterList.Add(branchCode);
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
        public string PrintPurchaseHTML(string companyCode, string branchCode, int billNo, out ErrorVM error)
        {
            error = null;
            StringBuilder report = new StringBuilder();
            try {

                KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(com => com.company_code == companyCode
                                                                           && com.branch_code == branchCode).FirstOrDefault();
                KTTU_PURCHASE_MASTER master = db.KTTU_PURCHASE_MASTER.Where(m => m.bill_no == billNo
                                                                            && m.company_code == companyCode
                                                                            && m.branch_code == branchCode).FirstOrDefault();
                List<KTTU_PURCHASE_DETAILS> details = db.KTTU_PURCHASE_DETAILS.Where(d => d.bill_no == billNo
                                                                             && d.company_code == companyCode
                                                                             && d.branch_code == branchCode).ToList();
                if (master == null) {
                    error = new ErrorVM()
                    {
                        description = "Invalid Purchase Bill Number",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return "";
                }

                #region HTML Generation
                report.AppendLine("<html>");
                report.AppendLine("<head>");
                report.AppendLine(SIGlobals.Globals.GetReportStyle());
                report.AppendLine("</head>");
                report.AppendLine("<body>");

                #region Report Header
                string DateTime = string.Format("{0:dd/MM/yyyy} {0:T}", Convert.ToDateTime(master.p_date));
                report.AppendLine("<Table frame=\"border\" border=\"0\" width=\"1000\">");
                for (int j = 0; j < 10; j++) {
                    report.AppendLine("<TR style=border:0>");
                    report.AppendLine(string.Format("<TD style=border:0 colspan = 11 ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    report.AppendLine("</TR>");
                }
                report.AppendLine("<TR style=border:0;\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                if (string.Compare(master.cflag.ToString(), "Y") == 0) {
                    report.AppendLine("<TD style=border:0 width=\"900\" colspan=0 ALIGN = \"CENTER\"><b><h3>PURCHASE INVOICE CANCELLED</h3></b></TD>");
                }
                else {
                    report.AppendLine("<TD style=border:0 width=\"900\" colspan=0 ALIGN = \"CENTER\"><b><h3>PURCHASE INVOICE </h3></b></TD>");
                }
                report.AppendLine("</TR>");

                for (int j = 0; j < 2; j++) {
                    report.AppendLine("<TR style=border:0>");
                    report.AppendLine(string.Format("<TD style=border:0 colspan = 11 ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    report.AppendLine("</TR>");
                }
                //int printHeader = CGlobals.GetEditOption("400");
                //if (printHeader == 0) {
                //    report.AppendLine("<TR>");
                //    report.AppendLine(string.Format("<TD style=border:0 colspan=3 ALIGN = \"CENTER\"><h5>For purchase from persons other than registered dealers<br>   <b>THE KERALA VALUE ADDED TAX RULES, 2005 <br> FORM NO. 8E<br><i>[See rule 58(10)]</i></h5></b> </b></TD>"));
                //    report.AppendLine("</TR>");
                //}
                report.AppendLine("</Table>");

                string metalType = db.KSTS_GS_ITEM_ENTRY.Where(gs => gs.gs_code == master.pur_item
                                                                && gs.company_code == companyCode
                                                                && gs.branch_code == branchCode).FirstOrDefault().metal_type;

                report.AppendLine("<Table font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse;\" width=\"1000\">");
                report.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                report.AppendLine(string.Format("<TD width=\"300\" ALIGN = \"center\"><b>CUSTOMER DETAILS</b></TD>"));
                report.AppendLine(string.Format("<TD width=\"300\"  ALIGN = \"center\"><b>GSTIN &nbsp&nbsp&nbsp&nbsp {0}</b></TD>", company.tin_no.ToString()));
                report.AppendLine(string.Format("<TD width=\"300\"  ALIGN = \"center\"><b>{0}</b></TD>", "SHOWROOM ADDRESS"));
                report.AppendLine("</TR>");
                report.AppendLine("<TR>");
                report.AppendLine("<td>");
                report.AppendLine("<Table font-size=12pt;  class=\"boldText\"  style=\"border-collapse:collapse;\" >");
                report.AppendLine("<tr style=\"border-right:0\"  >");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left \" ><b>Name &nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + master.cust_name + "</b></td>");
                report.AppendLine("</tr>");
                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" ><b>Address &nbsp&nbsp </b></td>");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + master.address1 + "</b></td>");
                report.AppendLine("</tr>");
                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + master.address2 + "</b></td>");
                report.AppendLine("</tr>");
                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + master.address3 + "</b></td>");
                report.AppendLine("</tr>");
                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \" tyle=\"border-top:thin\" align=\"left\" ><b>City &nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + master.city + "</b></td>");
                report.AppendLine("</tr>");
                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" ><b>State &nbsp&nbsp </b></td>");
                report.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + master.state + "</b></td>");
                report.AppendLine("</tr>");
                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>State code &nbsp&nbsp </b></td>");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + master.state_code + "</b></td>");
                report.AppendLine("</tr>");
                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>Phone &nbsp&nbsp </b></td>");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + master.mobile_no + "</b></td>");
                report.AppendLine("</tr>");
                if (!string.IsNullOrEmpty(master.pan_no)) {
                    report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    report.AppendLine("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>PAN &nbsp&nbsp</b></td>");
                    report.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + master.pan_no + "</b></td>");
                    report.AppendLine("</tr>");
                }
                else {
                    List<KSTU_CUSTOMER_ID_PROOF_DETAILS> custIDProf = db.KSTU_CUSTOMER_ID_PROOF_DETAILS.Where(pr => pr.cust_id == master.cust_id
                                                                                                        && pr.company_code == companyCode
                                                                                                        && pr.branch_code == branchCode).ToList();
                    if (custIDProf.Count > 0) {
                        if (!string.IsNullOrEmpty(custIDProf[0].Doc_code.ToString())) {
                            report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                            report.AppendLine(string.Format("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>{0} &nbsp&nbsp</b></td>", custIDProf[0].Doc_code.ToString()));
                            report.AppendLine(string.Format("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>{0}</b></td>", custIDProf[0].Doc_No.ToString()));
                            report.AppendLine("</tr>");
                        }
                    }
                }
                if (!string.IsNullOrEmpty(master.tin)) {
                    report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                    report.AppendLine("<td style=\"border-right:thin\"  style=\"border-top:thin\" align=\"left\" ><b>GSTIN &nbsp&nbsp</b></td>");
                    report.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + master.tin + "</b></td>");
                    report.AppendLine("</tr>");
                }
                report.AppendLine("</table>");
                report.AppendLine("</td>");



                report.AppendLine("<td>");
                report.AppendLine("<Table font-size=12pt;   class=\"boldText\"  style=\"border-collapse:collapse;\" >");

                report.AppendLine("<tr style=\"border-right:0\">");
                report.AppendLine("<td style=\"border-right:thin ; border-top:thin\" align=\"center \" ><b>INVOICE No</b></td>");
                report.AppendLine(string.Format("<td style=\"border-right:thin ; border-top:thin\" ><b>  {0}/{1}/{2}</b></TD>", branchCode, "P", master.bill_no));
                report.AppendLine("</tr>");

                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin ; border-top:thin\" align=\"left\" ><b>Date &nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin;border-top:thin\" ><b>" + DateTime + "</b></td>");
                report.AppendLine("</tr>");


                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" ><b> Place of supply &nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin;border-top:thin\" ><b>" + company.state.ToString() + "</b></td>");
                report.AppendLine("</tr>");

                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" ><b>PAN &nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin ; border-top:thin\" ><b>" + company.pan_no.ToString() + "</b></td>");
                report.AppendLine("</tr>");
                report.AppendLine("</table>");
                report.AppendLine("</td>");
                report.AppendLine("<td>");

                report.AppendLine("<Table font-size=12pt;   class=\"boldText\"  style=\"border-collapse:collapse;\" >");
                report.AppendLine("<tr style=\"border-right:0\"  >");
                report.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left \" ><b>Name &nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.company_name.ToString() + "</b></td>");
                report.AppendLine("</tr>");

                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>Address &nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.address1.ToString() + "</b></td>");
                report.AppendLine("</tr>");
                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.address2.ToString() + "</b></td>");
                report.AppendLine("</tr>");
                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.address3.ToString() + "</b></td>");
                report.AppendLine("</tr>");

                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" align=\"left\" ><b>City &nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin \" style=font-size=\"9pt\" style=\"border-top:thin\" ><b>" + company.city.ToString() + "</b></td>");
                report.AppendLine("</tr>");

                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>State &nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" ><b>" + company.state.ToString() + "</b></td>");
                report.AppendLine("</tr>");

                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>State code &nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin  ;border-top:thin\" ><b>" + company.state_code.ToString() + "</b></td>");
                report.AppendLine("</tr>");

                report.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>Phone &nbsp&nbsp</b></td>");
                report.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" ><b>" + company.phone_no.ToString() + "</b></td>");
                report.AppendLine("</tr>");
                report.AppendLine("</table>");
                report.AppendLine("</td>");
                report.AppendLine("</TR>");

                #endregion

                #region Report Body
                report.AppendLine("</Table>");
                report.AppendLine("<Table  font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse;\" width=\"1000\">");
                report.AppendLine("<TR>");
                report.AppendLine(string.Format("<TD style=\"border-top:thin\" colspan=11 ALIGN = \"CENTER\"><b>ORIGINAL / DUPLICATE <br></b></TD>"));
                report.AppendLine("</TR>");
                report.AppendLine("<TR bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b>Item</b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                report.AppendLine("<TH colspan = 3 ALIGN = \"CENTER\"><b>[Weight in Grams]</b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b>Jewel</b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b>Stone/Diamond</b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b>Gross</b></TH>");
                report.AppendLine("</TR>");
                report.AppendLine("<TR bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                report.AppendLine("<TH style=\"border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\"><b>Sl.No</b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin solid; border-top:thin \"  ALIGN = \"CENTER\"><b>Description</b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin solid; border-top:thin \"  ALIGN = \"CENTER\"><b>HSN Code</b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\"><b>Rate/Gram</b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\"><b>Qty</b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin solid;  \" ALIGN = \"CENTER\"><b>Gr.Wt</b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin solid; \" ALIGN = \"CENTER\"><b>St.Wt</b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin solid; \" ALIGN = \"CENTER\"><b>Net Wt</b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\"><b>Amount</b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\"><b>Amount</b></TH>");
                report.AppendLine("<TH style=\"border-bottom:thin solid; border-top:thin \" ALIGN = \"CENTER\"><b>Amount</b></TH>");
                report.AppendLine("</TR>");

                decimal DepPerGram = 0;
                decimal Grosswt = 0;
                decimal Stonewt = 0, MeltingLossWt = 0;
                decimal Netwt = 0;
                decimal GoldAmount = 0, StoneChg = 0, ItemAmount = 0;
                decimal PaidAmount = 0, RateDed = 0;
                int MaxPageRow = 5;
                decimal taxableAmount = 0, netAmount = 0, totalTAxablevalue = 0, totalSgst = 0, totalcgst = 0, totaligst = 0, totalNetAmount = 0;
                int units = 0;
                for (int i = 0; i < details.Count; i++) {
                    decimal Meltingloss = Convert.ToDecimal(details[i].melting_loss.ToString());
                    decimal nwt = Convert.ToDecimal(details[i].nwt.ToString());
                    if (Meltingloss != 0) {
                        DepPerGram = (Meltingloss / nwt);
                    }
                    DepPerGram = decimal.Round(Convert.ToDecimal(DepPerGram), 2);
                    taxableAmount = (Convert.ToDecimal(details[i].purchase_rate) * Convert.ToDecimal(details[i].nwt)) - Convert.ToDecimal(details[i].ratededuct);
                    netAmount = Math.Round(Convert.ToDecimal(Convert.ToDecimal(details[i].item_amount.ToString()) + Convert.ToDecimal(details[i].SGST_Amount) + Convert.ToDecimal(details[i].CGST_Amount) + Convert.ToDecimal(details[i].IGST_Amount)), 2, MidpointRounding.AwayFromZero);
                    report.AppendLine("<TR>");
                    string gsCode = details[i].gs_code;
                    string name = details[i].item_name;
                    ITEM_MASTER itemMaster = db.ITEM_MASTER.Where(it => it.gs_code == gsCode && it.Item_code == name && it.company_code == companyCode && it.branch_code == it.branch_code).FirstOrDefault();
                    string aliasName = itemMaster == null ? details[i].item_name.ToString() : itemMaster.Item_Name;
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"center\"><b>{0}{1}{1} </b></TD>", i + 1, "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"LEFT\"><b>{0}{1}{1} </b></TD>"
                        , aliasName, "", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"center\"><b>{0}{1}{1} </b></TD>", Convert.ToString(details[i].HSN), "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1} </b></TD>", details[i].purchase_rate.ToString(), "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1} </b></TD>", details[i].item_no.ToString(), "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1} </b></TD>", details[i].gwt.ToString(), "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1} </b></TD>", details[i].swt.ToString(), "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1} </b></TD>", details[i].nwt.ToString(), "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1} </b></TD>", details[i].gold_amount.ToString(), "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1} </b></TD>", details[i].diamond_amount.ToString(), "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1} </b></TD>", details[i].item_amount.ToString(), "&nbsp"));
                    report.AppendLine("</TR>");

                    totalTAxablevalue += taxableAmount;
                    totalNetAmount += netAmount;

                    taxableAmount = 0;
                    netAmount = 0;
                    MeltingLossWt += Convert.ToDecimal(details[i].melting_loss);
                    totalcgst += Convert.ToDecimal(details[i].CGST_Amount);
                    totaligst += Convert.ToDecimal(details[i].IGST_Amount);
                    totalSgst += Convert.ToDecimal(details[i].SGST_Amount);
                    units += Convert.ToInt32(details[i].item_no);
                    RateDed += Convert.ToDecimal(details[i].ratededuct);
                    Grosswt += Convert.ToDecimal(details[i].gwt);
                    Stonewt += Convert.ToDecimal(details[i].swt);
                    Netwt += Convert.ToDecimal(details[i].nwt);
                    GoldAmount += Convert.ToDecimal(details[i].gold_amount);
                    StoneChg += Convert.ToDecimal(details[i].diamond_amount);
                    ItemAmount += Convert.ToDecimal(details[i].item_amount);
                }

                for (int j = 0; j < MaxPageRow - details.Count; j++) {
                    report.AppendLine("<TR>");
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    report.AppendLine("</TR>");
                }
                ItemAmount = Math.Round(ItemAmount, 2, MidpointRounding.ToEven);
                report.AppendLine("<TR  bgcolor='#FFFACD'  style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                report.AppendLine("<TH  style=\"border-top:thin solid ; border-right:thin \" ALIGN = \"LEFT\"><b>Totals</b></TH>");
                report.AppendLine(string.Format("<TH style=\"border-top:thin solid ; border-right:thin \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TH>", "&nbsp", "&nbsp"));
                report.AppendLine(string.Format("<TH style=\"border-top:thin solid ; border-right:thin\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TH>", "&nbsp", "&nbsp"));
                report.AppendLine(string.Format("<TH style=\"border-top:thin solid ; border-right:thin\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TH>", "&nbsp", "&nbsp"));
                report.AppendLine(string.Format("<TH style=\"border-top:thin solid \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TH>", units, "&nbsp"));
                report.AppendLine(string.Format("<TH style=\"border-top:thin solid \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TH>", Grosswt, "&nbsp"));
                report.AppendLine(string.Format("<TH style=\"border-top:thin solid \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TH>", Stonewt, "&nbsp"));
                report.AppendLine(string.Format("<TH style=\"border-top:thin solid \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TH>", Netwt, "&nbsp"));
                report.AppendLine(string.Format("<TH style=\"border-top:thin solid \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TH>", GoldAmount, "&nbsp"));
                report.AppendLine(string.Format("<TH style=\"border-top:thin solid \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TH>", StoneChg, "&nbsp"));
                report.AppendLine(string.Format("<TH style=\"border-top:thin solid \" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TH>", ItemAmount.ToString("n", indianDefaultCulture), "&nbsp"));
                report.AppendLine("</TR>");

                #region If Purchase Attached
                string strBillNo = Convert.ToString(billNo);
                int finYear = SIGlobals.Globals.GetFinancialYear(db, companyCode, branchCode);
                KTTU_PAYMENT_DETAILS payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.Ref_BillNo == strBillNo
                                                                            && pay.company_code == companyCode
                                                                            && pay.branch_code == branchCode
                                                                            && (pay.trans_type == "S" || pay.trans_type == "O")
                                                                            && pay.pay_mode == "PB"
                                                                            && pay.fin_year == finYear).FirstOrDefault();
                decimal Purchaseamt = decimal.Round(ItemAmount, 0); ;
                if (payment != null)
                    Purchaseamt = ItemAmount;
                decimal roundoff = Purchaseamt - ItemAmount;
                decimal Inword = Purchaseamt;

                report.AppendLine(string.Format("<TR><TD style=\"border-bottom:thin ; border-top:thin solid \" colspan=10 ALIGN = \"RIGHT\"><b>Net Amount{1}</b></TD><TD style=\"border-bottom:thin ; border-top:thin  dolid\" ALIGN = \"RIGHT\"><b>{0}{1}{1}</b></TD></TR>"
                , Purchaseamt.ToString("n", indianDefaultCulture), "&nbsp"));

                List<KTTU_PURCHASE_STONE_DETAILS> stones = db.KTTU_PURCHASE_STONE_DETAILS.Where(st => st.bill_no == billNo
                                                                             && st.company_code == companyCode
                                                                             && st.branch_code == branchCode).ToList();
                if (stones != null && stones.Count > 0) {
                    for (int j = 0; j < stones.Count; j++) {
                        report.AppendLine(string.Format("<TR><TD style=\"border-bottom:thin ; border-top:thin;border-right:thin \" colspan=10 ALIGN = \"LEFT\"><b>{0}</b></TD><TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"></b></TD></TR>"
                                , j + 1 + ")" + stones[j].name + "\\" + stones[j].rate
                                + "*" + stones[j].carrat));
                    }
                }

                if (payment != null) {
                    decimal purAmount = Convert.ToDecimal(payment.pay_amt);
                    purAmount = Math.Round(purAmount, 0, MidpointRounding.ToEven);
                    if ((string.Compare(payment.trans_type.ToString(), "S") == 0) && Convert.ToDecimal(payment.receipt_no) > 0) {
                        report.AppendLine(string.Format("<TR><TD colspan=10 style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"><b>Adjusted towards Credit Receipt No: {3}/{0}{2}{2} </b></TD><TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"><b>{1}{2}{2}</b></TD></TR>"
                                , payment.receipt_no, payment.pay_amt.ToString(), "&nbsp", branchCode));
                    }
                    else if ((string.Compare(payment.trans_type.ToString(), "O") == 0) && Convert.ToDecimal(payment.receipt_no) > 0) {
                        report.AppendLine(string.Format("<TR><TD colspan=10 style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"><b>Adjusted towards Order Receipt No: {3}/{0}{2}{2} </b></TD><TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"><b>{1}{2}{2}</b></TD></TR>"
                                , payment.receipt_no, payment.pay_amt.ToString(), "&nbsp", branchCode));
                    }
                    else {
                        report.AppendLine(string.Format("<TR><TD colspan=10 style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"><b>Adjusted towards Sales Bill No: {3}/{4}/{0}{2}{2} </b></TD><TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"><b>{1}{2}{2}</b></TD></TR>"
                                , payment.series_no, payment.pay_amt.ToString(), "&nbsp", branchCode, "S"));
                    }
                    report.AppendLine();
                    ItemAmount -= purAmount;
                }

                #endregion

                #region Payment Details
                List<KTTU_PAYMENT_DETAILS> lstOfPay = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == billNo
                                                                                    && pay.trans_type == "P"
                                                                                    && pay.company_code == companyCode
                                                                                    && pay.branch_code == branchCode).ToList();

                if (lstOfPay != null && lstOfPay.Count > 0) {
                    for (int i = 0; i < lstOfPay.Count; i++) {
                        string mode = lstOfPay[i].pay_mode.ToString();
                        decimal PayAmt = Convert.ToDecimal(lstOfPay[i].pay_amt);
                        PayAmt = Math.Round(PayAmt, 0, MidpointRounding.ToEven);
                        if (string.Compare(mode, "C") == 0) {
                            report.AppendLine("<TR>");
                            report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"RIGHT\"><b>Cash paid{1}{1}</b></TD><TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"> <b>{0}{1}{1}</b></TD>"
                                , PayAmt.ToString("N"), "&nbsp"));
                            report.AppendLine("</TR>");
                        }
                        else if (string.Compare(mode, "Q") == 0) {
                            int bankCode = Convert.ToInt32(lstOfPay[i].bank_name);
                            string bankname = db.KSTU_ACC_LEDGER_MASTER.Where(ac => ac.acc_code == bankCode && ac.company_code == companyCode && ac.branch_code == branchCode).FirstOrDefault().acc_name;
                            report.AppendLine(string.Format("<TR><TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"RIGHT\"><b>{0}{2}{2}</b></TD><TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"> <b>{1}{2}{2}</b></TD></TR>"
                                  , "Cheque issued " + bankname + " / " + lstOfPay[i].cheque_no.ToString().Trim()
                                , PayAmt.ToString("n", indianDefaultCulture), "&nbsp"));
                        }
                        else if (string.Compare(mode, "EP") == 0) {
                            if (!string.IsNullOrEmpty(Convert.ToString(lstOfPay[i].pay_details))) {
                                report.AppendLine(string.Format("<TR><TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"RIGHT\"><b>{0}{2}{2}</b></TD><TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"> <b>{1}{2}{2}</b></TD></TR>"
                                 , "NEFT Payment ( " + lstOfPay[i].pay_details.ToString().Trim() + ")"
                               , PayAmt.ToString("n", indianDefaultCulture), "&nbsp"));
                            }
                            else {
                                report.AppendLine(string.Format("<TR><TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"RIGHT\"><b>{0}{2}{2}</b></TD><TD style=\"border-bottom:thin ; border-top:thin \" ALIGN = \"RIGHT\"> <b>{1}{2}{2}</b></TD></TR>"
                                , "NEFT Payment "
                               , PayAmt.ToString("n", indianDefaultCulture), "&nbsp"));
                            }
                        }
                        PaidAmount += PayAmt;
                    }
                }
                ItemAmount -= ItemAmount;
                report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin solid \" colspan=11 ALIGN = \"lef\"><b>SM Code : {0} </b></TD>", details[0].sal_code));
                report.AppendLine("<TR>");
                report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=11 ALIGN = \"left\"><b>Est no :{0} </b></TD>", details[0].est_no.ToString(), "&nbsp"));
                report.AppendLine("</TR>");
                report.AppendLine("<TR>");
                report.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=11 ALIGN = \"left\"><b>Operator Code :{0} </b></TD>", master.operator_code.ToString(), "&nbsp"));
                report.AppendLine("</TR>");
                string strWords = string.Empty; ;
                NumberToWords objNWClass = new NumberToWords();
                objNWClass.ConvertNumberToWords(Convert.ToDouble(Inword), out strWords);
                strWords = strWords.Replace("Rupees", "");
                if (roundoff != 0)
                    strWords = "Rupees " + strWords + " (Round-off: " + roundoff + ")";
                else
                    strWords = "Rupees " + strWords;
                report.AppendLine(string.Format("<TR><TD style=\"border-bottom:thin  ; border-top:thin \" colspan=11 ALIGN = \"LEFT\"><b>{0}</b></TD></TR>", strWords));

                string footer = string.Empty;
                KTTU_NOTE note = db.KTTU_NOTE.Where(n => n.note_type == "Sal").FirstOrDefault();
                if (note != null) {
                    footer = note.line1 + note.line2 + note.line3 + note.line4 + note.line5 + note.line6 + note.line7 + note.line8 + note.line9 + note.line10;
                }
                report.AppendLine("<TR>");
                report.AppendLine(string.Format("<TD colspan = 6  style=\"border-right:thin\" ALIGN = \"LEFT\"> <b><br>{0}<br><br><br><br>{1}</b></TD>", footer, "Customer Signature"));
                
                report.AppendLine(string.Format("<TD colspan = 6 ALIGN = \"RIGHT\"><b>For {0}<br><br><br><br><br>{1}</b></TD>", company.company_name, "Authorized Signatory"));
                report.AppendLine("</TR>");
                #endregion

                #endregion

                report.AppendLine("</body>");
                report.AppendLine("</body>");
                #endregion

                return report.ToString();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return "";
            }
        }

        public ProdigyPrintVM PrintPurchaseBill(string companyCode, string branchCode, int billNo, out ErrorVM error)
        {
            error = null;
            string printData = PrintPurchaseHTML(companyCode, branchCode, billNo, out error);
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            printObject.PrintType = "HTML";
            printObject.ContinueNextPrint = true;
            printObject.Data = new PrintConfiguration().Base64Encode(printData);
            return printObject;
        }

        public dynamic GetAdjustedPurchaseBills(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            DateTime date = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
            int finYear = SIGlobals.Globals.GetFinancialYear(db, companyCode, branchCode);
            //var refBillNo = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.pay_mode == "PB" && pay.cflag != "Y" && pay.company_code == companyCode && pay.branch_code == branchCode && pay.fin_year == finYear).Select(p => p.Ref_BillNo).ToList();
            //var refBillNo2 = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.trans_type == "P" && pay.cflag != "Y" && pay.company_code == companyCode && pay.branch_code == branchCode && pay.fin_year == finYear).Select(p => p.series_no).ToList();
            //var data = (from pm in db.KTTU_PURCHASE_MASTER
            //            join pd in db.KTTU_PURCHASE_DETAILS
            //            on new
            //            {
            //                Company = pm.company_code,
            //                Branch = pm.branch_code,
            //                BillNo = pm.bill_no
            //            }
            //            equals new
            //            {
            //                Company = pd.company_code,
            //                Branch = pd.branch_code,
            //                BillNo = pd.bill_no
            //            }
            //            where System.Data.Entity.DbFunctions.TruncateTime(pm.p_date) == System.Data.Entity.DbFunctions.TruncateTime(date)
            //            && pm.company_code == companyCode
            //            && pm.branch_code == branchCode
            //            && pm.cflag != "Y"
            //            && !refBillNo.Contains(pm.bill_no.ToString())
            //            && !refBillNo2.Contains(pm.bill_no)
            //            group new
            //            {
            //                pm,
            //                pd
            //            } by new
            //            {
            //                pm.bill_no,
            //                pm.p_date,
            //                pm.cust_name,
            //                pm.purchase_amount
            //            } into g
            //            select new
            //            {
            //                BillNo = g.Key.bill_no,
            //                Customer = g.Key.cust_name,
            //                Date = g.Key.p_date,
            //                Amount = g.Key.purchase_amount,
            //                GrossWt = g.Sum(x => x.pd.gwt),
            //                Quantity = g.Sum(x => x.pd.item_no)
            //            }).OrderBy(o => o.BillNo);

            ObjectResult<GetAdjustedPurchaeBill_Result> data = db.GetAdjustedPurchaeBill(companyCode, branchCode, date, finYear);
            return data.ToList();
        }
        public List<SearchParamVM> GetPurchaseBillSearchParams(string companyCode, string branchCode)
        {
            List<SearchParamVM> lstSearchParams = new List<SearchParamVM>();
            lstSearchParams.Add(new SearchParamVM() { Value = "Bill No", Key = "BILLNO" });
            lstSearchParams.Add(new SearchParamVM() { Value = "Customer", Key = "CUSTOMER" });
            lstSearchParams.Add(new SearchParamVM() { Value = "Date", Key = "DATE" });
            lstSearchParams.Add(new SearchParamVM() { Value = "Amount", Key = "AMOUNT" });
            lstSearchParams.Add(new SearchParamVM() { Value = "Gross Wt", Key = "GWT" });
            lstSearchParams.Add(new SearchParamVM() { Value = "Quantity", Key = "QTY" });
            return lstSearchParams;
        }
        public dynamic GetPurchaseBills(string companyCode, string branchCode, DateTime date)
        {
            int finYear = SIGlobals.Globals.GetFinancialYear(db, companyCode, branchCode);
            var data = db.GetAdjustedPurchaeBill(companyCode, branchCode, date, finYear);
            return data.ToList();

            // Bellow LINQ query also works fine
            //var result = (from purchase in db.KTTU_PURCHASE_MASTER
            //              join purchaseDet in db.KTTU_PURCHASE_DETAILS
            //              on new { CompanCode = purchase.company_code, BranchCode = purchase.branch_code, BillNo = purchase.bill_no }
            //              equals new { CompanCode = purchaseDet.company_code, BranchCode = purchaseDet.branch_code, BillNo = purchaseDet.bill_no }
            //              where purchase.cflag != "Y" && purchase.company_code == companyCode && purchase.branch_code == branchCode && purchaseDet.Fin_Year == finYear
            //               && System.Data.Entity.DbFunctions.TruncateTime(purchase.p_date) == System.Data.Entity.DbFunctions.TruncateTime(date)
            //              group new
            //              {
            //                  purchase,
            //                  purchaseDet
            //              } by new
            //              {
            //                  purchase.bill_no,
            //                  purchase.cust_name,
            //                  purchase.p_date,
            //                  purchase.purchase_amount,
            //              } into g
            //              select new PurchaseBillSearchVM()
            //              {
            //                  BillNo = g.Key.bill_no,
            //                  Customer = g.Key.cust_name,
            //                  Date = g.Key.p_date,
            //                  Amount = g.Key.purchase_amount,
            //                  Gwt = g.Sum(x => x.purchaseDet.gwt),
            //                  Qty = g.Sum(x => x.purchaseDet.item_no)
            //              });
            //var seriesNo = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.trans_type == "P"
            //                                                && pay.company_code == companyCode
            //                                                && pay.branch_code == branchCode
            //                                                && pay.fin_year == finYear
            //                                                && pay.cflag != "Y").Select(s => s.series_no).ToList();
            //var refBillNo = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.trans_type == "PB"
            //                                                && pay.company_code == companyCode
            //                                                && pay.branch_code == branchCode
            //                                                && pay.fin_year == finYear
            //                                                && pay.cflag != "Y").Select(s => s.series_no).ToList();
            //var data = (from r in result
            //            where !seriesNo.Contains(r.BillNo) && !refBillNo.Contains(r.BillNo)
            //            select new PurchaseBillSearchVM()
            //            {
            //                BillNo = r.BillNo,
            //                Customer = r.Customer,
            //                Date = r.Date,
            //                Amount = r.Amount,
            //                Gwt = r.Gwt,
            //                Qty = r.Qty
            //            });
            //return data;
        }
        public dynamic GetPurchaseBillSearchParameters(string companyCode, string branchCode, string searchBy, string searchParam, DateTime date)
        {
            // This functionality also work fine with above method commented code.
            //IQueryable<PurchaseBillSearchVM> lstPurchaseBill = new PurchaseBillingBL().GetPurchaseBills(companyCode, branchCode, date);
            //switch (searchBy.ToUpper()) {
            //    case "BILLNO":
            //        int billNo = Convert.ToInt32(searchParam);
            //        return lstPurchaseBill.Where(search => search.BillNo == billNo).AsQueryable<PurchaseBillSearchVM>();
            //    case "CUSTOMER":
            //        return lstPurchaseBill.Where(search => search.Customer.Contains(searchParam)).AsQueryable<PurchaseBillSearchVM>();
            //    case "DATE":
            //        return lstPurchaseBill.Where(search => search.Date == date).AsQueryable<PurchaseBillSearchVM>();
            //    case "AMOUNT":
            //        decimal amount = Convert.ToDecimal(searchParam);
            //        return lstPurchaseBill.Where(search => search.Amount == amount).AsQueryable<PurchaseBillSearchVM>();
            //    case "GWT":
            //        decimal gwt = Convert.ToDecimal(searchParam);
            //        return lstPurchaseBill.Where(search => search.GrossWt == gwt).AsQueryable<PurchaseBillSearchVM>();
            //    case "QTY":
            //        int qty = Convert.ToInt32(searchParam);
            //        return lstPurchaseBill.Where(search => search.Quantity == qty).AsQueryable<PurchaseBillSearchVM>();
            //}
            //return lstPurchaseBill;

            List<GetAdjustedPurchaeBill_Result> lstPurchaseBill = new PurchaseBillingBL().GetPurchaseBills(companyCode, branchCode, date);
            switch (searchBy.ToUpper()) {
                case "BILLNO":
                    int billNo = Convert.ToInt32(searchParam);
                    return lstPurchaseBill.Where(search => search.BillNo == billNo).ToList();
                case "CUSTOMER":
                    return lstPurchaseBill.Where(search => search.Customer.Contains(searchParam.ToUpper())).ToList();
                case "DATE":
                    return lstPurchaseBill.Where(search => search.Date == date).ToList();
                case "AMOUNT":
                    decimal amount = Convert.ToDecimal(searchParam);
                    return lstPurchaseBill.Where(search => search.Amount == amount).ToList();
                case "GWT":
                    decimal gwt = Convert.ToDecimal(searchParam);
                    return lstPurchaseBill.Where(search => search.GrossWt == gwt).ToList();
                case "QTY":
                    int qty = Convert.ToInt32(searchParam);
                    return lstPurchaseBill.Where(search => search.Quantity == qty).ToList();
            }
            return lstPurchaseBill;
        }
        public string GetPurchaseDotMatrixPrint(string companyCode, string branchCode, int billNo, out ErrorVM error)
        {
            error = null;
            KTTU_PURCHASE_MASTER purchase = db.KTTU_PURCHASE_MASTER.Where(pur => pur.bill_no == billNo && pur.company_code == companyCode && pur.branch_code == branchCode).FirstOrDefault();
            if (purchase == null) return "";
            KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(com => com.company_code == companyCode && com.branch_code == branchCode).FirstOrDefault();
            KSTU_CUSTOMER_MASTER customer = db.KSTU_CUSTOMER_MASTER.Where(cust => cust.cust_id == purchase.cust_id && cust.company_code == companyCode && cust.branch_code == branchCode).FirstOrDefault();
            try {
                //string strPurchaseBilling = string.Empty;
                //strPurchaseBilling = string.Format("Select PM.est_no,PM.bill_no,PM.p_date as bill_date,PM.cust_name,PM.cust_id,PM.address1,PM.address2,PM.address3,CM.Email_ID,\n"
                //+ "PM.city,PM.mobile_no,PM.tin,CM.salutation,PM.state,PM.state_code,CM.phone_no,PM.pin_code,PM.pan_no,PM.operator_code,PM.pur_item,\n"
                //+ "PM.est_no,km.company_name,km.Header1,km.Header2,km.Header3,km.Header4,km.phone_no,km.Header5,km.Header6,km.Header7,km.tin_no,cflag,PM.pin_code \n"
                //+ "from KSTU_COMPANY_MASTER km,KTTU_PURCHASE_MASTER PM,KSTU_CUSTOMER_MASTER CM with(nolock) \n"
                //+ "where PM.cust_id = CM.cust_id and bill_no = {0} and PM.company_code = '{1}' and PM.branch_code = '{2}' \n"
                //+ "and CM.company_code = '{1}' and CM.branch_code = '{2}' and km.company_code = '{1}' and km.branch_code = '{2}'"
                //, billNo, companyCode, branchCode);
                //DataTable dtPurchaseBilling = SIGlobals.Globals.ExecuteQuery(strPurchaseBilling);
                //if (dtPurchaseBilling == null || dtPurchaseBilling.Rows.Count == 0)
                //{
                //    return "";
                //}
                StringBuilder sb = new StringBuilder();
                int width = 80;
                StringBuilder strdoubleTransLine = new StringBuilder();
                strdoubleTransLine.Append('=', width);
                StringBuilder strTransLine = new StringBuilder();
                strTransLine.Append('-', width);
                decimal Spaces = 0M;
                StringBuilder strSpaces = new StringBuilder();
                Spaces = ((width - strTransLine.Length) / 3);
                strSpaces.Append(' ', Convert.ToInt32(Spaces));
                string esc = Convert.ToChar(27).ToString();

                string DateTime = string.Format("{0:dd/MM/yyyy} {0:T}", Convert.ToDateTime(purchase.p_date));

                sb = SIGlobals.Globals.GetCompanyname(db, companyCode, branchCode);
                if (string.Compare(purchase.cflag.ToString(), "Y") == 0) {
                    sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), "PURCHASE INVOICE / CANCELLED"));
                }
                else {
                    sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), " PURCHASE INVOICE"));
                }
                sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), "ORIGINAL / DUPLICATE"));
                sb.Append(Convert.ToChar(27).ToString() + Convert.ToChar(70).ToString());
                sb.AppendLine(string.Format("{0}{1}{2}", esc + Convert.ToChar(112).ToString(), strSpaces, strdoubleTransLine));
                sb.AppendLine(string.Format("{0,-20}{1,-5}{2,-5}{3,50}"
                    , "INVOICE NO ", ": " + branchCode, "/" + "P" + "/" + purchase.bill_no.ToString()
                    , "Date :" + DateTime));
                sb.AppendLine(string.Format("{0,-20}{1,-60}", "Customer Name ", ": " + customer.salutation.ToString() + customer.cust_name.ToString()));
                if (!string.IsNullOrEmpty(customer.address1.ToString()))
                    sb.AppendLine(string.Format("{0,-20}{1,-60}", "Address ", ": " + customer.address1.ToString()));
                if (!string.IsNullOrEmpty(customer.address2.ToString()))
                    sb.AppendLine(string.Format("{0,-20}{1,-60}", "", ": " + customer.address2.ToString()));
                if (!string.IsNullOrEmpty(customer.address3.ToString()))
                    sb.AppendLine(string.Format("{0,-20}{1,-60}", "", ": " + customer.address3.ToString()));
                if (!string.IsNullOrEmpty(customer.city.ToString()) && !string.IsNullOrEmpty(customer.pin_code.ToString()))
                    sb.AppendLine(string.Format("{0,-20}{1,-60}", "", ": " + customer.city.ToString() + " - " + customer.pin_code.ToString()));
                else if (!string.IsNullOrEmpty(customer.city.ToString()))
                    sb.AppendLine(string.Format("{0,-20}{1,-60}", "", ": " + customer.city.ToString()));
                if (!string.IsNullOrEmpty(customer.Email_ID.ToString()))
                    sb.AppendLine(string.Format("{0,-20}{1,-60}", "", ": " + customer.Email_ID.ToString()));

                if (!string.IsNullOrEmpty(customer.mobile_no.ToString()) && !string.IsNullOrEmpty(customer.phone_no.ToString()))
                    sb.AppendLine(string.Format("{0,-20}{1,-15}{2,-5}{3,-40}", "Mobile", ": " + customer.mobile_no.ToString(), "PH", ": " + customer.phone_no.ToString()));
                else if (!string.IsNullOrEmpty(customer.mobile_no.ToString()) && string.IsNullOrEmpty(customer.phone_no.ToString()))
                    sb.AppendLine(string.Format("{0,-20}{1,-60}", "Mobile", ": " + customer.mobile_no.ToString()));
                else if (string.IsNullOrEmpty(customer.mobile_no.ToString()) && !string.IsNullOrEmpty(customer.phone_no.ToString()))
                    sb.AppendLine(string.Format("{0,-20}{1,-60}", "PH", ": " + customer.phone_no.ToString()));
                if (!string.IsNullOrEmpty(customer.state.ToString()))
                    sb.AppendLine(string.Format("{0,-20}{1,-60}", "State", ": " + customer.state.ToString()));

                if (!string.IsNullOrEmpty(customer.state_code.ToString()))
                    sb.AppendLine(string.Format("{0,-20}{1,-60}", "State code", ": " + customer.state_code.ToString()));

                if (!string.IsNullOrEmpty(customer.pan_no.ToString()))
                    sb.AppendLine(string.Format("{0,-20}{1,-60}", "PAN", ": " + customer.pan_no.ToString()));
                else {
                    //string idquery = string.Format("select top(1) Doc_code,Doc_No from KSTU_CUSTOMER_ID_PROOF_DETAILS where cust_id='{0}' and company_code = '{1}' and branch_code = '{2}'", customer.cust_id.ToString(), companyCode, branchCode);
                    //DataTable IdDt = SIGlobals.Globals.ExecuteQuery(idquery);

                    KSTU_CUSTOMER_ID_PROOF_DETAILS idProof = db.KSTU_CUSTOMER_ID_PROOF_DETAILS.Where(id => id.cust_id == customer.cust_id && id.company_code == companyCode && id.branch_code == branchCode).FirstOrDefault();

                    if (idProof != null) {
                        if (!string.IsNullOrEmpty(idProof.Doc_code.ToString())) {
                            sb.AppendLine(string.Format("{0,-20}{1,-60}", idProof.Doc_code.ToString(), ": " + idProof.Doc_No.ToString()));
                        }
                    }
                }

                if (!string.IsNullOrEmpty(company.tin_no.ToString()))
                    sb.AppendLine(string.Format("{0,-20}{1,-60}", "GSTIN", ": " + company.tin_no.ToString()));

                //string purdetails = string.Format("Select item_name,gwt,item_amount,melting_loss,purchase_rate,swt,nwt,gold_amount,isnull(HSN,'') as HSN,\n"
                //        + "sal_code,gs_code from KTTU_PURCHASE_DETAILS with(nolock)  where bill_no = {0} and company_code = '{1}' and branch_code = '{2}'"
                //        , billNo, companyCode, branchCode);
                //DataTable dtPurDetails = SIGlobals.Globals.ExecuteQuery(purdetails);

                List<KTTU_PURCHASE_DETAILS> purchaseDet = db.KTTU_PURCHASE_DETAILS.Where(pur => pur.bill_no == billNo
                                                                                            && pur.company_code == companyCode
                                                                                            && pur.branch_code == branchCode).ToList();

                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));
                sb.AppendLine(string.Format("{0,-18}{1,-10}{2,10}{3,10}{4,10}{5,12}{6,10}",
                    "Description", "HSN", "Rate", "Gr.Wt", "St.Wt", "Net Wt", "Amount"));
                sb.AppendLine(string.Format("{0,-18}{1,-10}{2,10}{3,10}{4,10}{5,12}{6,10}"
                    , "", "Code", "(Rs.Ps)", "(Grams)", "(Grams)", "(Grams)", "(Rs.Ps)"));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));

                decimal Grosswt = 0;
                decimal Stonewt = 0;
                decimal Netwt = 0;
                decimal Amount = 0;
                decimal PaidAmount = 0;

                for (int i = 0; i < purchaseDet.Count; i++) {
                    string gsCode = purchaseDet[i].gs_code.ToString();
                    string itemName = purchaseDet[i].item_name.ToString();
                    ITEM_MASTER itemMaster = db.ITEM_MASTER.Where(item => item.gs_code == gsCode && item.Item_code == itemName && item.company_code == companyCode && item.branch_code == branchCode).FirstOrDefault();

                    sb.AppendLine(string.Format("{0,-18}{1,-10}{2,10}{3,10}{4,10}{5,12}{6,10}"
                        , itemMaster.Item_Name.ToString()
                        , purchaseDet[i].HSN.ToString()
                        , purchaseDet[i].purchase_rate.ToString()
                        , purchaseDet[i].gwt.ToString()
                        , purchaseDet[i].swt.ToString()
                        , purchaseDet[i].nwt.ToString()
                        , purchaseDet[i].gold_amount.ToString()));

                    Grosswt += Convert.ToDecimal(purchaseDet[i].gwt);
                    Stonewt += Convert.ToDecimal(purchaseDet[i].swt);
                    Netwt += Convert.ToDecimal(purchaseDet[i].nwt);
                    Amount += Convert.ToDecimal(purchaseDet[i].gold_amount);
                }
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                sb.AppendLine(string.Format("{0,-18}{1,-10}{2,10}{3,10}{4,10}{5,12}{6,10}"
                    , "Grand Total: ", "", "", Grosswt, Stonewt, Netwt, Amount.ToString("F")));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));


                //string query = string.Format("select est_no,qty,type,name,carrat,rate,amount \n"
                //+ "from KTTU_PURCHASE_STONE_DETAILS where bill_no = {0} and company_code = '{1}' and branch_code = '{2}'"
                //, billNo, companyCode, branchCode);
                //DataTable dtStoneDetails = SIGlobals.Globals.ExecuteQuery(query);

                List<KTTU_PURCHASE_STONE_DETAILS> purStone = db.KTTU_PURCHASE_STONE_DETAILS.Where(stone => stone.bill_no == billNo && stone.company_code == companyCode && stone.branch_code == branchCode).ToList();

                if (purStone != null && purStone.Count > 0) {
                    for (int j = 0; j < purStone.Count; j++)
                        sb.AppendLine(string.Format("{0,-92}", j + 1 + ")" + purStone[j].name + "\\" + purStone[j].rate + "*" + purStone[j].carrat));
                }

                //decimal DiamondAmount = Convert.ToDecimal(SIGlobals.Globals.ExecuteQuery(string.Format("select sum(amount) as Amount from KTTU_PURCHASE_STONE_DETAILS with(nolock)  \n"
                //+ "where bill_no = {0} and company_code = '{1}' and branch_code = '{2}' and type = 'P'"
                //, billNo, companyCode, branchCode)).Rows[0]["Amount"]);

                decimal DiamondAmount = purStone.Where(p => p.type == "P").Sum(s => s.amount);
                if (DiamondAmount > 0) {
                    string temp = string.Format(string.Format("{0,60}{1,20}", "Diamond Amount", DiamondAmount.ToString("n", indianDefaultCulture)));
                    sb.AppendLine(string.Format("{0}", temp));
                    Amount += DiamondAmount;
                }

                decimal Purchaseamt = Amount;
                Amount = Math.Round(Amount, 0, MidpointRounding.ToEven);
                decimal roundoff = Purchaseamt - Amount;
                decimal Inword = Amount;

                string temp1 = string.Format(string.Format("{0,60}{1,20}", "Net Amount", Amount.ToString("n", indianDefaultCulture)));
                sb.AppendLine(string.Format("{0}", temp1));

                //string PurAttach = string.Format("Select series_no,pay_amt,receipt_no,trans_type from KTTU_PAYMENT_DETAILS with(nolock) \n"
                //+ "where Ref_BillNo = '{0}' and company_code = '{1}' and branch_code = '{2}' and (trans_type = 'S' or trans_type = 'O') and  pay_mode = 'PB' and fin_year = {3}"
                //, billNo, companyCode, branchCode, SIGlobals.Globals.GetFinancialYear(db, companyCode, branchCode));
                //DataTable dtPurAttach = SIGlobals.Globals.ExecuteQuery(PurAttach);

                string refBillNo = billNo.ToString();
                int finYear = SIGlobals.Globals.GetFinancialYear(db, companyCode, branchCode);
                KTTU_PAYMENT_DETAILS purchaseAttach = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.company_code == companyCode && pay.branch_code == branchCode
                                                              && pay.Ref_BillNo == refBillNo
                                                              && pay.pay_mode == "PB"
                                                              && pay.fin_year == finYear
                                                              && (pay.trans_type == "S" || pay.trans_type == "O")).FirstOrDefault();

                if (purchaseAttach != null) {
                    decimal purAmount = Convert.ToDecimal(purchaseAttach.pay_amt);
                    purAmount = Math.Round(purAmount, 0, MidpointRounding.ToEven);
                    if ((string.Compare(purchaseAttach.trans_type.ToString(), "S") == 0) && Convert.ToDecimal(purchaseAttach.receipt_no) > 0)
                        sb.Append(string.Format("{0,60}", "Adjusted Towards Credit Receipt No: " + branchCode + "/" + purchaseAttach.receipt_no));
                    else if ((string.Compare(purchaseAttach.trans_type.ToString(), "O") == 0) && Convert.ToDecimal(purchaseAttach.receipt_no) > 0)
                        sb.Append(string.Format("{0,60}", "Adjusted Towards Order Receipt No: " + branchCode + "/" + purchaseAttach.receipt_no));
                    else
                        sb.AppendLine(string.Format("{0,60}", "Adjusted Towards Sales Bill No: " + branchCode + "/" + "S" + "/" + purchaseAttach.series_no));
                    sb.AppendLine();
                    Amount = Amount - purAmount;
                }

                //string paymentdetails = string.Format("select pay_mode,pay_amt,bank,cheque_no,pay_details from KTTU_PAYMENT_DETAILS with(nolock)  \n"
                //+ "where series_no = {0} and trans_type = 'P' and company_code = '{1}' and branch_code = '{2}' order by sno"
                //, billNo, companyCode, branchCode);
                //DataTable dtpaymentdetails = SIGlobals.Globals.ExecuteQuery(paymentdetails);

                List<KTTU_PAYMENT_DETAILS> payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.company_code == companyCode && pay.branch_code == branchCode
                                                              && pay.series_no == billNo
                                                              && pay.trans_type == "P").OrderBy(pay => pay.sno).ToList();

                if (payment != null && payment.Count > 0) {
                    for (int i = 0; i < payment.Count; i++) {
                        string mode = payment[i].pay_mode.ToString();
                        decimal PayAmt = Convert.ToDecimal(payment[i].pay_amt);
                        PayAmt = Math.Round(PayAmt, 0, MidpointRounding.ToEven);
                        if (string.Compare(mode, "C") == 0)
                            sb.AppendLine(string.Format("{0,60}{1,20}", "Cash paid ", PayAmt.ToString("N")));
                        else if (string.Compare(mode, "Q") == 0) {
                            int accCode = Convert.ToInt32(payment[i].bank);
                            KSTU_ACC_LEDGER_MASTER accLedMaster = db.KSTU_ACC_LEDGER_MASTER.Where(acc => acc.acc_code == accCode && acc.company_code == companyCode && acc.branch_code == branchCode).FirstOrDefault();

                            string bankname = accLedMaster.Party_BankName;
                            sb.AppendLine(string.Format("{0, 60}{1, 20}"
                                , "Cheque Issued " + bankname + "/" + payment[i].cheque_no.ToString().Trim()
                                , PayAmt.ToString("N")));
                        }
                        else if (string.Compare(mode, "EP") == 0) {
                            if (!string.IsNullOrEmpty(payment[i].pay_details.ToString())) {
                                sb.AppendLine(string.Format("{0, 60}{1, 20}"
                                    , "NEFT Payment ( " + payment[i].pay_details.ToString() + ")"
                                    , PayAmt.ToString("N")));
                            }
                            else {
                                sb.AppendLine(string.Format("{0, 60}{1, 20}"
                                   , "NEFT Payment "
                                   , PayAmt.ToString("N")));
                            }
                        }
                        PaidAmount += PayAmt;
                    }
                }
                Amount = Amount - PaidAmount;
                if (Amount != 0) {
                    sb.Append(string.Format("{0,8}", ""));
                    sb.Append(esc + Convert.ToChar(69).ToString());
                    sb.Append(esc + Convert.ToChar(14).ToString() + Convert.ToChar(14).ToString());
                    sb.Append(esc + Convert.ToChar(52).ToString());
                    string temp = string.Format("{0,-35}", "Balance: " + Amount.ToString("n", indianDefaultCulture));
                    sb.Append(string.Format("{0}", temp));
                    sb.Append(esc + Convert.ToChar(53).ToString());
                    sb.Append(Convert.ToChar(20).ToString() + Convert.ToChar(10).ToString());
                    sb.Append(esc + Convert.ToChar(70).ToString());
                }
                else if (Amount == 0) {
                    sb.Append(string.Format("{0,20}", ""));
                    sb.Append(esc + Convert.ToChar(69).ToString());
                    sb.Append(esc + Convert.ToChar(14).ToString() + Convert.ToChar(14).ToString());
                    sb.Append(esc + Convert.ToChar(52).ToString());
                    string temp = string.Format("{0,-40}", "Balance: " + "NIL");
                    sb.Append(string.Format("{0}", temp));
                    sb.Append(esc + Convert.ToChar(53).ToString());
                    sb.Append(Convert.ToChar(20).ToString() + Convert.ToChar(10).ToString());
                    sb.Append(esc + Convert.ToChar(70).ToString());
                }
                string strWords = string.Empty; ;
                NumberToWords objNWClass = new NumberToWords();
                objNWClass.ConvertNumberToWords(Convert.ToDouble(Inword), out strWords);
                strWords = strWords.Replace("Rupees", "");
                sb.AppendLine(string.Format("{0,-80}", "Rupees " + strWords));
                if (roundoff != 0)
                    sb.AppendLine(string.Format("{0,-80}", "(Round-off:" + roundoff + ")"));
                sb.AppendLine(string.Format("{0,-10}{1,-10}{2,20}{3,20}{4,20}"
                             , "", ""
                             , "# OPR :" + purchase.operator_code
                             , "SAL :" + purchaseDet[0].sal_code
                             , "Est No :" + purchase.est_no));

                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine(string.Format("{0,-48}{1,32}", "Customer Signature", "Authorised Signatory"));

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
            //    //ObjectParameter errorMessage = new ObjectParameter("errorMessage", typeof(string));
            //    //var data = db.usp_createPurchasePostingVouchers(companyCode, branchCode, Convert.ToString(billNo), "PB", errorMessage);
            //    //string message = Convert.ToString(errorMessage.Value);
            //    //if (data.Count() <= 0 || message != "") {
            //    //    return new ErrorVM() { description = "Error Occurred while Updating Accounts. " + errorMessage, field = "Account Update", index = 0 };
            //    //}

            //    string sql = "EXEC [dbo].[usp_createPurchasePostingVouchers] \n"
            //                + "  @p0, \n"
            //                + "  @p1,\n"
            //                + "  @p2,\n"
            //                + "  @p3,\n"
            //                + "  @p4";
            //    List<object> parameterList = new List<object>();
            //    parameterList.Add(companyCode);
            //    parameterList.Add(branchCode);
            //    parameterList.Add(Convert.ToString(billNo));
            //    parameterList.Add("PB");
            //    parameterList.Add("");
            //    object[] parametersArray = parameterList.ToArray();
            //    int recordsAffected = SIGlobals.Globals.ExecuteSQL(sql, db, parametersArray);
            //}
            //catch (Exception excp) {
            //    return new ErrorVM() { description = excp.InnerException.Message, field = "Account Update", index = 0 };
            //}
            //return null;

            try {
                string errorFromProc = string.Empty;
                ObjectParameter errorMessage = new ObjectParameter("errorMsg", typeof(string));
                ObjectParameter outputVal = new ObjectParameter("outValue", typeof(int));
                var result = db.usp_createPurchasePostingVouchers_FuncImport(companyCode, branchCode, Convert.ToString(billNo), "PB", outputVal, errorMessage);
                return new CommonBL().HandleAccountPostingProcs(outputVal, errorMessage);
            }
            catch (Exception excp) {
                return new ErrorVM() { description = excp.Message, field = "Account Update", index = 0 };
            }
        }

        // Need to update with procedure once given by account team.
        private ErrorVM CancelAccountPostingWithProcedure(string companyCode, string branchCode, int billNo)
        {
            try {
                ObjectParameter errorMessage = new ObjectParameter("errorMsg", typeof(string));
                ObjectParameter outVal = new ObjectParameter("outValue", typeof(int));
                var data = db.usp_createPurchasePostingVouchers(companyCode, branchCode, Convert.ToString(billNo), "PB", outVal, errorMessage);
                string message = Convert.ToString(errorMessage.Value);
                if (data.Count() <= 0 || message != "") {
                    return new ErrorVM() { description = "Error Occurred while Updating Accounts. " + errorMessage, field = "Account Update", index = 0 };
                }
            }
            catch (Exception excp) {
                return new ErrorVM() { description = excp.InnerException.Message, field = "Account Update", index = 0 };
            }
            return null;
        }
        private bool StoneStockUpdate(string stoneName, string gsCode, int itemNo, decimal gwt, decimal nwt, string companyCode, string branchCode)
        {
            if (gsCode == "") {
                KSTS_GS_ITEM_ENTRY gs = db.KSTS_GS_ITEM_ENTRY.Where(gst => gst.item_level1_name == stoneName && gst.company_code == companyCode && gst.branch_code == branchCode).FirstOrDefault();
                return StockUpdate(gs.gs_code, itemNo, gwt, nwt, companyCode, branchCode);
            }
            else {
                return StockUpdate(gsCode, itemNo, gwt, nwt, companyCode, branchCode);
            }
        }
        #endregion

        #region Public Methods
        public bool StockUpdate(string gsCode, int itemNo, decimal gwt, decimal nwt, string companyCode, string branchCode)
        {
            KTTU_GS_SALES_STOCK gs = db.KTTU_GS_SALES_STOCK.Where(gst => gst.gs_code == gsCode && gst.company_code == companyCode && gst.branch_code == branchCode).FirstOrDefault();
            if (gs != null) {
                gs.receipt_units += itemNo;
                gs.receipt_gwt += gwt;
                gs.receipt_nwt += nwt;
                gs.closing_units += itemNo;
                gs.closing_gwt += gwt;
                gs.closing_nwt += nwt;
                db.Entry(gs).State = System.Data.Entity.EntityState.Modified;
            }
            else {
                return false;
            }
            return true;
        }

        public bool CancelStockUpdate(int billNo, string companyCode, string branchCode, bool reverseEntries, out ErrorVM error)
        {
            error = null;
            var data = from pay in db.KTTU_PURCHASE_DETAILS
                       where pay.bill_no == billNo && pay.company_code == companyCode && pay.branch_code == branchCode
                       group pay by new
                       {
                           pay.gs_code,

                       } into g
                       select new StockJournalVM()
                       {
                           StockTransType = StockTransactionType.Issue,
                           CompanyCode = companyCode,
                           BranchCode = branchCode,
                           GS = g.Key.gs_code,
                           Qty = g.Sum(x => x.item_no),
                           GrossWt = g.Sum(x => x.gwt),
                           StoneWt = g.Sum(x => x.swt),
                           NetWt = g.Sum(x => x.nwt)
                       };
            string errorMsg = string.Empty;
            bool stockUpdated = new StockPostBL().GSStockPost(db, data.ToList(), reverseEntries, out errorMsg);
            if (!stockUpdated) {
                error = new ErrorVM()
                {
                    description = errorMsg,
                    ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError,
                    customDescription = "Error while updating Purchase Stock"
                };
                return false;
            }
            return true;
        }

        public bool ValidatePurchaeBill(int billNo, string companyCode, string branchCode, out ErrorVM error)
        {
            // This is the Magna Procedure of : [dbo].[usp_ValidatePurchaseBillNo]
            error = null;
            if (billNo > 0) {
                string strBillNo = Convert.ToString(billNo);
                KTTU_PURCHASE_MASTER purchase = db.KTTU_PURCHASE_MASTER.Where(pur => pur.bill_no == billNo
                                                                            && pur.company_code == companyCode
                                                                            && pur.branch_code == branchCode).FirstOrDefault();
                var payments = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.Ref_BillNo == strBillNo
                                                                            && pay.company_code == companyCode
                                                                            && pay.branch_code == branchCode).ToList();
                if (purchase == null) {
                    error.description = string.Format("Purchase Bill No: {0} is invalide", billNo);
                    error.ErrorStatusCode = System.Net.HttpStatusCode.NotFound;
                    return false;
                }

                if (purchase.cflag == "Y") {
                    error.description = string.Format("Purchase Bill No: {0} is already cancelled.", billNo);
                    error.ErrorStatusCode = System.Net.HttpStatusCode.NotFound;
                    return false;
                }

                if (payments.Count > 0) {
                    var payDone = payments.Where(pay => pay.trans_type == "S" && pay.pay_mode == "PB" && pay.cflag != "Y").FirstOrDefault();
                    if (payDone != null) {
                        if (payDone.series_no != 0) {
                            error.description = string.Format("Purchase Bill No: {0} is already adjusted towards {1} Sales Bill No: ", billNo, payDone.series_no);
                            error.ErrorStatusCode = System.Net.HttpStatusCode.NotFound;
                            return false;
                        }
                    }
                    payDone = payments.Where(pay => pay.trans_type == "O" && pay.pay_mode == "PB" && pay.cflag != "Y").FirstOrDefault();
                    if (payDone != null) {
                        if (payDone.series_no != 0) {
                            error.description = string.Format("Purchase Bill No: {0} is already adjusted towards {1} Order No: ", billNo, payDone.series_no);
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
