using NumberToWordsINR;
using ProdigyAPI.BL.BusinessLayer.Common;
using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.BL.BusinessLayer.PrintConfig;
using ProdigyAPI.BL.ViewModel.Credit;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Payment;
using ProdigyAPI.BL.ViewModel.Print;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;

namespace ProdigyAPI.BL.BusinessLayer.CreditReceipt
{
    public class CreditReceiptBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        public const string MODULE_SEQ_NO = "24";
        #endregion

        #region Methods
        public CreditReceiptVM GetCreditBillDetails(string companyCode, string branchCode, int finYear, int billNo, out ErrorVM error)
        {
            error = null;
            try {


                int lastDocumentNo = db.KSTS_SEQ_NOS.Where(rs => rs.company_code == companyCode
                                                           && rs.branch_code == branchCode
                                                           && rs.obj_id == MODULE_SEQ_NO).FirstOrDefault().nextno - 1;

                KTTU_SALES_MASTER ksm = db.KTTU_SALES_MASTER.Where(sale => sale.bill_no == billNo
                                                                    && sale.company_code == companyCode
                                                                    && sale.branch_code == branchCode).FirstOrDefault();
                List<KTTU_SALES_DETAILS> ksd = db.KTTU_SALES_DETAILS.Where(sd => sd.bill_no == billNo
                                                                         && sd.company_code == companyCode
                                                                         && sd.branch_code == branchCode).ToList();
                //if (ksm == null) {
                //    error = new ErrorVM()
                //    {
                //        field = "Bill No",
                //        description = "Invalid Bill Number",
                //        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                //    };
                //    return null;
                //}
                //else if (ksm.cflag == "Y") {
                //    error = new ErrorVM()
                //    {
                //        field = "Bill No",
                //        description = "Bill No already cancelled.",
                //        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                //    };
                //    return null;
                //}

                CreditReceiptVM cr = new CreditReceiptVM();
                if (ksm != null) {
                    if (ksm.cflag == "Y") {
                        error = new ErrorVM()
                        {
                            field = "Bill No",
                            description = "Bill No already cancelled.",
                            ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                        };
                        return null;
                    }

                    //if (ksm.bill_date.Date != SIGlobals.Globals.GetApplicationDate().Date) {
                    //    error = new ErrorVM()
                    //    {
                    //        description = "Only todays credit receipts can be cancelled.",
                    //        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    //    };
                    //    return null;
                    //}
                    #region As per Magna, we no need to get payment details. Just query the balance amount from salesmaster table.
                    //List<KTTU_PAYMENT_DETAILS> kpd = db.KTTU_PAYMENT_DETAILS.Where(i => i.company_code == companyCode
                    //                                                     && i.branch_code == branchCode
                    //                                                     && i.fin_year == finYear && i.series_no == billNo
                    //                                                     && i.cflag != "Y"
                    //                                                     && i.trans_type == "S").ToList(); 
                    #endregion

                    var balanceAmount = Convert.ToDecimal(ksm.balance_amt);

                    decimal amountNeedToPay = 0;
                    if (ksd != null) {
                        amountNeedToPay = Convert.ToDecimal(ksd.Sum(sd => sd.item_final_amount));
                    }
                    else {
                        amountNeedToPay = Convert.ToDecimal(ksm.total_bill_amount + ksm.total_tax_amount - ksm.discount_amount) + Convert.ToDecimal(ksm.round_off);
                    }

                    decimal paidAmt = 0;
                    paidAmt = amountNeedToPay - balanceAmount;
                    if (paidAmt == 0) {
                        paidAmt = 0;
                    }
                    cr.BillNo = ksm.bill_no;
                    cr.FinancialYear = ksm.fin_year;
                    cr.CustID = Convert.ToInt32(ksm.cust_Id);
                    cr.CustomerName = ksm.cust_name;
                    //cr.SalesAmount = Math.Round(Convert.ToDecimal(ksm.total_bill_amount + ksm.total_tax_amount - ksm.discount_amount) + Convert.ToDecimal(ksm.round_off), 2, MidpointRounding.ToEven);
                    cr.SalesAmount = Math.Round(amountNeedToPay, 0, MidpointRounding.ToEven);
                    cr.PaidAmount = Math.Round(paidAmt, 0, MidpointRounding.ToEven);
                    cr.BalanceAmount = balanceAmount;
                    cr.MobileNo = ksm.mobile_no;
                    cr.Address1 = ksm.address1;
                    cr.Address2 = ksm.address2;
                    cr.Address3 = ksm.address3;
                    cr.City = ksm.city;
                    cr.State = ksm.state;
                    cr.Pincode = ksm.pin_code;
                    cr.PANNo = ksm.pan_no;
                    cr.LastReceiptNo = lastDocumentNo;
                }
                else {
                    KTTU_SALES_MASTER_OLD oldSales = db.KTTU_SALES_MASTER_OLD.Where(sal => sal.bill_no == billNo
                                                                                    && sal.fin_year == finYear
                                                                                    && sal.company_code == companyCode
                                                                                    && sal.branch_code == branchCode).FirstOrDefault();
                    List<KTTU_PAYMENT_DETAILS_OLD> oldPayment = db.KTTU_PAYMENT_DETAILS_OLD.Where(i => i.company_code == companyCode
                                                            && i.branch_code == branchCode
                                                            && i.fin_year == finYear && i.series_no == billNo
                                                            && i.cflag != "Y"
                                                            && i.trans_type == "S").ToList();
                    List<KTTU_PAYMENT_DETAILS> newPayment = db.KTTU_PAYMENT_DETAILS.Where(i => i.company_code == companyCode
                                                             && i.branch_code == branchCode
                                                             && i.fin_year == finYear && i.series_no == billNo
                                                             && i.cflag != "Y"
                                                             && i.trans_type == "S").ToList();
                    decimal oldPaidAmt = 0;
                    decimal newPaidAmt = 0;
                    oldPaidAmt = oldPayment.Count == 0 ? 0 : Convert.ToDecimal(oldPayment.Sum(r => r.pay_amt));
                    newPaidAmt = newPayment.Count == 0 ? 0 : Convert.ToDecimal(newPayment.Sum(r => r.pay_amt));
                    if (oldSales != null) {
                        cr.BillNo = oldSales.bill_no;
                        cr.FinancialYear = oldSales.fin_year;
                        cr.CustID = Convert.ToInt32(oldSales.cust_Id);
                        cr.CustomerName = oldSales.cust_name;
                        cr.SalesAmount = Math.Round(Convert.ToDecimal(oldSales.total_bill_amount + oldSales.total_tax_amount) + Convert.ToDecimal(oldSales.round_off), 2, MidpointRounding.ToEven);
                        cr.PaidAmount = Math.Round(Convert.ToDecimal(oldPaidAmt + newPaidAmt), 0, MidpointRounding.ToEven);
                        cr.BalanceAmount = cr.SalesAmount - cr.PaidAmount;
                        cr.MobileNo = oldSales.mobile_no;
                        cr.Address1 = oldSales.address1;
                        cr.Address2 = oldSales.address2;
                        cr.Address3 = oldSales.address3;
                        cr.City = oldSales.city;
                        cr.State = oldSales.state;
                        cr.Pincode = oldSales.pin_code;
                        cr.PANNo = oldSales.pan_no;
                        cr.LastReceiptNo = lastDocumentNo;
                    }
                    else {
                        error = new ErrorVM()
                        {
                            field = "Bill No",
                            description = "Invalid Bill Number",
                            ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                        };
                        return null;
                    }
                }

                // This validation thought that at the time of billing, need to validate.
                if (cr.BalanceAmount < 0) {
                    error = new ErrorVM()
                    {
                        field = "Bill No",
                        description = string.Format("Invoice: {0} has no credit amount to pay", billNo),
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                    return null;
                }

                cr.lstOfPayment = new List<PaymentVM>();
                return cr;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public int SaveCreditReceiptDetails(List<CreditPaymentDetailsVM> payment, int finYear, int billNo, out ErrorVM error)
        {
            error = null;
            decimal balanceAmount = 0;
            decimal paidAmount = 0;
            decimal cashPaidAmount = 0;
            int creditReceiptBillNo = 0;
            string companyCode = payment[0].CompanyCode;
            string branchCode = payment[0].BranchCode;
            decimal grandTotal = 0;

            KTTU_CREDIT_BILL_DETAILS kcbd = new KTTU_CREDIT_BILL_DETAILS();
            List<KTTU_PAYMENT_DETAILS> kpayd = new List<KTTU_PAYMENT_DETAILS>();
            KTTU_SALES_MASTER_OLD oldSales = new KTTU_SALES_MASTER_OLD();



            #region Basic validation
            decimal totalPaidAmount = Convert.ToDecimal(db.KTTU_PAYMENT_DETAILS.Where(i => i.company_code == companyCode
                                                         && i.branch_code == branchCode
                                                         && i.fin_year == finYear && i.series_no == billNo
                                                         && i.cflag != "Y"
                                                         && i.trans_type == "S").Sum(r => r.pay_amt));

            var ksm = db.KTTU_SALES_MASTER.Where(m => m.company_code == companyCode
                                                    && m.branch_code == branchCode
                                                    && m.fin_year == finYear && m.bill_no == billNo).FirstOrDefault();
            List<KTTU_SALES_DETAILS> ksd = db.KTTU_SALES_DETAILS.Where(sd => sd.bill_no == billNo
                                                                     && sd.company_code == companyCode
                                                                     && sd.branch_code == branchCode).ToList();
            if (ksm == null) {
                oldSales = db.KTTU_SALES_MASTER_OLD.Where(sal => sal.bill_no == billNo
                                                                               && sal.fin_year == finYear
                                                                               && sal.company_code == companyCode
                                                                               && sal.branch_code == branchCode).FirstOrDefault();
                if (oldSales == null) {
                    error = new ErrorVM
                    {
                        index = 0,
                        field = "",
                        description = "Bill detail does not exist.",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                    return 0;
                }
                grandTotal = Convert.ToDecimal(Convert.ToDecimal(oldSales.grand_total));
                balanceAmount = Convert.ToDecimal(grandTotal - totalPaidAmount);

            }
            else {
                if (ksm.cflag == "Y") {
                    error = new ErrorVM
                    {
                        index = 0,
                        field = "",
                        description = "Invoice is cancelled already.",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                    return 0;
                }

                //grandTotal = Convert.ToDecimal(Convert.ToDecimal(ksm.grand_total));
                if (ksd != null) {
                    grandTotal = Convert.ToDecimal(ksd.Sum(sd => sd.item_final_amount));
                }
                else {
                    grandTotal = Convert.ToDecimal(ksm.total_bill_amount + ksm.total_tax_amount - ksm.discount_amount) + Convert.ToDecimal(ksm.round_off);
                }
                balanceAmount = Convert.ToDecimal(Math.Round(grandTotal, 0, MidpointRounding.ToEven) - totalPaidAmount);
            }

            // Checking Balance Amount Validation
            if (balanceAmount == 0) {
                error = new ErrorVM
                {
                    index = 0,
                    field = "",
                    description = "Balance amount is Zero",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return 0;
            }

            // Cash Tollerance Validation
            foreach (CreditPaymentDetailsVM r in payment) {
                paidAmount = Convert.ToDecimal(paidAmount + r.PayAmount);
                if (r.PayMode == "C") {
                    cashPaidAmount = cashPaidAmount + Convert.ToDecimal(r.PayAmount);
                }
            }
            decimal tollAmount = db.KSTU_TOLERANCE_MASTER.Where(tm => tm.obj_id == 201
                                                    && tm.company_code == companyCode
                                                    && tm.branch_code == branchCode).FirstOrDefault().Max_Val;

            if (cashPaidAmount >= tollAmount) {
                error = new ErrorVM
                {
                    index = 0,
                    field = "",
                    description = "Cash transaction limit exceeds. Cash transaction is not allowed for more than " + tollAmount,
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return 0;
            }

            if (paidAmount > Math.Round(balanceAmount, MidpointRounding.ToEven)) {
                error = new ErrorVM
                {
                    index = 0,
                    field = "",
                    description = "Paid Amount is greater than balance amount",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return 0;
            }
            #endregion
            using (var transaction = db.Database.BeginTransaction()) {
                try {
                    #region Save Master and Payments

                    creditReceiptBillNo = Convert.ToInt32(db.KSTU_ACC_FY_MASTER.Where(fy => fy.company_code == companyCode
                                                                                && fy.branch_code == branchCode).FirstOrDefault().fin_year.ToString().Remove(0, 1) +
                                                                                db.KSTS_SEQ_NOS.Where(
                                                                                rs => rs.company_code == companyCode
                                                                                && rs.branch_code == branchCode
                                                                                && rs.obj_id == MODULE_SEQ_NO).FirstOrDefault().nextno);
                    if (ksm != null) {
                        kcbd.Obj_Id = SIGlobals.Globals.GetNewGUID();
                        kcbd.company_code = ksm.company_code;
                        kcbd.branch_code = ksm.branch_code;
                        kcbd.amount_paid = paidAmount;
                        kcbd.balance_amount = balanceAmount - paidAmount;
                        kcbd.bill_amount = grandTotal;
                        kcbd.bill_no = billNo;
                        kcbd.credit_bill_receiptNo = creditReceiptBillNo;
                        kcbd.Fin_Year = ksm.fin_year;
                        kcbd.garunteename = ksm.guranteename;
                        kcbd.Ispaid = "Y";
                        kcbd.New_Bill_No = ksm.New_Bill_No;
                        kcbd.Paid_date = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                        kcbd.UpdateOn = SIGlobals.Globals.GetDateTime();
                        kcbd.UniqRowID = Guid.NewGuid();
                        db.KTTU_CREDIT_BILL_DETAILS.Add(kcbd);
                    }
                    else {
                        kcbd.Obj_Id = SIGlobals.Globals.GetNewGUID();
                        kcbd.company_code = oldSales.company_code;
                        kcbd.branch_code = oldSales.branch_code;
                        kcbd.amount_paid = paidAmount;
                        kcbd.balance_amount = balanceAmount - paidAmount;
                        kcbd.bill_amount = grandTotal;
                        kcbd.bill_no = billNo;
                        kcbd.credit_bill_receiptNo = creditReceiptBillNo;
                        kcbd.Fin_Year = oldSales.fin_year;
                        kcbd.garunteename = oldSales.guranteename;
                        kcbd.Ispaid = "Y";
                        kcbd.New_Bill_No = oldSales.New_Bill_No;
                        kcbd.Paid_date = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                        kcbd.UpdateOn = SIGlobals.Globals.GetDateTime();
                        kcbd.UniqRowID = Guid.NewGuid();
                        db.KTTU_CREDIT_BILL_DETAILS.Add(kcbd);
                    }

                    int SlNo = 1;
                    foreach (CreditPaymentDetailsVM s in payment) {
                        KTTU_PAYMENT_DETAILS kpd = new KTTU_PAYMENT_DETAILS();
                        kpd.obj_id = SIGlobals.Globals.GetNewGUID();
                        kpd.company_code = s.CompanyCode;
                        kpd.branch_code = s.BranchCode;
                        kpd.series_no = ksm == null ? oldSales.bill_no : ksm.bill_no;
                        kpd.receipt_no = kcbd.credit_bill_receiptNo;
                        kpd.sno = SlNo;
                        kpd.trans_type = "S";
                        kpd.pay_mode = s.PayMode;
                        kpd.pay_details = s.PayDetails;
                        kpd.pay_date = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                        kpd.pay_amt = s.PayAmount;
                        kpd.Ref_BillNo = s.RefBillNo == null ? "" : s.RefBillNo;
                        kpd.party_code = s.CTBranch;
                        kpd.bill_counter = s.BillCounter == null ? "FF" : s.BillCounter;
                        kpd.is_paid = "N";
                        //kpd.bank = s.Bank;
                        if (s.PayMode == "R") {
                            int accCode = Convert.ToInt32(s.Bank);
                            kpd.bank = db.KTTS_CARD_COMMISSION.Where(cc => cc.company_code == companyCode && cc.branch_code == branchCode && cc.acc_code == accCode).FirstOrDefault().bank;
                        }
                        else {
                            kpd.bank = s.Bank;
                        }
                        kpd.bank_name = s.BankName == null ? "" : s.BankName;
                        kpd.cheque_date = s.ChequeDate == null ? SIGlobals.Globals.GetApplicationDate(companyCode, branchCode) : s.ChequeDate;
                        kpd.card_type = s.CardType == null ? "" : s.CardType;
                        kpd.expiry_date = s.ExpiryDate == null ? SIGlobals.Globals.GetApplicationDate(companyCode, branchCode) : s.ExpiryDate;
                        kpd.cflag = "N";
                        kpd.card_app_no = s.CardAppNo;
                        kpd.scheme_code = s.SchemeCode;
                        kpd.sal_bill_type = "CR";
                        kpd.operator_code = s.OperatorCode;
                        kpd.session_no = s.SessionNo == null ? 0 : s.SessionNo;
                        kpd.UpdateOn = SIGlobals.Globals.GetDateTime();
                        kpd.group_code = s.GroupCode == null ? "" : s.GroupCode;
                        kpd.amt_received = s.AmtReceived == null ? 0 : s.AmtReceived;
                        kpd.bonus_amt = s.BonusAmt == null ? 0 : s.BonusAmt;
                        kpd.win_amt = s.WinAmt == null ? 0 : s.BonusAmt; ;
                        kpd.ct_branch = s.CTBranch;
                        kpd.fin_year = finYear;
                        kpd.CardCharges = s.CardCharges == null ? 0 : s.CardCharges;
                        kpd.cheque_no = s.ChequeNo == null ? "0" : s.ChequeNo;
                        //kpd.cheque_no = s.PayMode == "R" ? db.KSTU_ACC_LEDGER_MASTER.Where(kalm => kalm.acc_name == s.Bank && kalm.acc_type == "R" && kalm.company_code == companyCode && kalm.branch_code == branchCode).FirstOrDefault().acc_code.ToString() : s.ChequeNo;
                        kpd.cheque_no = s.PayMode == "R" ? s.Bank : s.ChequeNo;
                        kpd.New_Bill_No = s.NewBillNo;
                        kpd.Add_disc = s.AddDisc == null ? 0 : s.AddDisc;
                        kpd.isOrdermanual = s.IsOrderManual == null ? "N" : s.IsOrderManual;
                        kpd.currency_value = s.CurrencyValue == null ? 0 : s.CurrencyValue;
                        kpd.exchange_rate = s.ExchangeRate == null ? 0 : s.ExchangeRate;
                        kpd.currency_type = s.CurrencyType == null ? "INR" : s.CurrencyType;
                        kpd.tax_percentage = s.TaxPercentage == null ? 0 : s.TaxPercentage;
                        kpd.cancelled_by = s.CancelledBy == null ? "" : s.CancelledBy;
                        kpd.cancelled_remarks = s.CancelledRemarks == null ? "" : s.CancelledRemarks;
                        kpd.cancelled_date = s.CancelledDate == null ? SIGlobals.Globals.GetApplicationDate(companyCode, branchCode).ToString() : s.CancelledDate;
                        kpd.isExchange = s.IsExchange == null ? "N" : s.IsExchange;
                        kpd.exchangeNo = s.ExchangeNo == null ? 0 : s.ExchangeNo;
                        kpd.new_receipt_no = s.NewReceiptNo;
                        kpd.Gift_Amount = s.GiftAmount == null ? 0 : s.GiftAmount;
                        kpd.cardSwipedBy = s.CardSwipedBy == null ? "" : s.CardSwipedBy;
                        kpd.version = s.Version == null ? 0 : s.Version;
                        kpd.GSTGroupCode = s.GSTGroupCode;
                        kpd.SGST_Percent = s.SGSTPercent == null ? 0 : s.SGSTPercent;
                        kpd.CGST_Percent = s.CGSTPercent == null ? 0 : s.CGSTPercent;
                        kpd.IGST_Percent = s.IGSTPercent == null ? 0 : s.IGSTPercent;
                        kpd.HSN = s.HSN;
                        kpd.UniqRowID = Guid.NewGuid();
                        db.KTTU_PAYMENT_DETAILS.Add(kpd);
                        SlNo = SlNo + 1;
                        if (kpd.pay_mode == "CT") {
                            int memebershipNo = Convert.ToInt32(kpd.Ref_BillNo);
                            CHTU_CHIT_CLOSURE chit = db.CHTU_CHIT_CLOSURE.Where(ccl => ccl.Scheme_Code == kpd.scheme_code && ccl.Group_Code == kpd.group_code && ccl.branch_code == kpd.ct_branch && ccl.Chit_MembShipNo == memebershipNo).FirstOrDefault();
                            kpd.amt_received = chit.Amt_Received;
                            kpd.bonus_amt = chit.Bonus_Amt;
                            kpd.win_amt = chit.win_amt;
                            chit.Bill_Number = Convert.ToString(ksm == null ? oldSales.bill_no : ksm.bill_no);
                            db.Entry(chit).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                    SIGlobals.Globals.UpdateSeqenceNumber(db, MODULE_SEQ_NO, companyCode, branchCode);
                    db.SaveChanges();

                    // After doing db.SaveChanges() only we can get Receipt Number in the db to do account posting.
                    // Account Update
                    ErrorVM procError = AccountPostingWithProedure(companyCode, branchCode, creditReceiptBillNo);
                    if (procError != null) {
                        error = new ErrorVM()
                        {
                            field = "Delivery Number",
                            index = 0,
                            description = procError.description,
                            ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError
                        };
                        return 0;
                    }
                    #endregion

                    #region Updating Balance Amount to Sales Master 
                    //We should not do this because, if we take a print out of duplicate invoice the updated value will come, original value we should not change.
                    //KTTU_SALES_MASTER sales = db.KTTU_SALES_MASTER.Where(sal => sal.company_code == companyCode && sal.branch_code == branchCode && sal.bill_no == billNo).FirstOrDefault();
                    //if (sales != null) {
                    //    sales.balance_amt = sales.balance_amt - paidAmount;
                    //    db.Entry(sales).State = System.Data.Entity.EntityState.Modified;
                    //}
                    #endregion

                    db.SaveChanges();

                    #region Document Creation Posting, error will not be checked
                    //Post to DocumentCreationLog Table
                    DateTime billDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                    new Common.CommonBL().PostDocumentCreation(companyCode, branchCode, 5, billNo, billDate, payment[0].OperatorCode);
                    transaction.Commit();
                    #endregion
                }
                catch (Exception excp) {
                    transaction.Rollback();
                    error = new ErrorVM().GetErrorDetails(excp);
                    return 0;
                }
                return creditReceiptBillNo;
            }
        }

        public CancelCreditReceiptVM GetCreditReceiptDetails(string companyCode, string branchCode, int receiptNo, bool isPrint, out ErrorVM error)
        {
            error = null;
            CancelCreditReceiptVM receipt = new CancelCreditReceiptVM();
            try {
                KTTU_PAYMENT_DETAILS kpd = db.KTTU_PAYMENT_DETAILS.Where(pd => pd.receipt_no == receiptNo
                                                                        && pd.company_code == companyCode
                                                                        && pd.branch_code == branchCode
                                                                        && pd.trans_type == "S").FirstOrDefault();

                List<KTTU_PAYMENT_DETAILS> kpdp = db.KTTU_PAYMENT_DETAILS.Where(pd => pd.receipt_no == receiptNo
                                                                        && pd.company_code == companyCode
                                                                        && pd.branch_code == branchCode
                                                                        && pd.trans_type == "S").ToList();

                if (kpd == null) {
                    error = new ErrorVM()
                    {
                        index = 0,
                        description = "Invalide Receipt Number.",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                    return null;
                }
                if (!isPrint) {
                    if (kpd.cflag == "Y") {
                        error = new ErrorVM()
                        {
                            index = 0,
                            description = "Receipt cancelled already.",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return null;
                    }
                }

                KTTU_SALES_MASTER ksm = db.KTTU_SALES_MASTER.Where(sale => sale.bill_no == kpd.series_no
                                                                   && sale.company_code == companyCode
                                                                   && sale.branch_code == branchCode).FirstOrDefault();
                if (ksm != null) {
                    receipt.CompanyCode = kpd.company_code;
                    receipt.BranchCode = kpd.branch_code;
                    receipt.ReceiptNo = kpd.receipt_no;
                    receipt.FinYear = Convert.ToInt32(kpd.fin_year);
                    receipt.BillNo = kpd.series_no;
                    receipt.BilledDate = ksm.bill_date;
                    receipt.CFlag = kpd.cflag;
                    receipt.ReceiptDate = kpd.pay_date;
                    receipt.CustomerName = ksm.cust_name;
                    receipt.CustID = Convert.ToInt32(ksm.cust_Id);
                    receipt.Address1 = ksm.address1;
                    receipt.Address2 = ksm.address2;
                    receipt.Address3 = ksm.address3;
                    receipt.City = ksm.city;
                    receipt.State = ksm.state;
                    receipt.Pincode = ksm.pin_code;
                    receipt.MobileNo = ksm.mobile_no;
                    receipt.Remarks = "";
                }
                else {
                    KTTU_SALES_MASTER_OLD ksmOld = db.KTTU_SALES_MASTER_OLD.Where(sal => sal.bill_no == kpd.series_no
                                                                                    && sal.company_code == companyCode
                                                                                    && sal.branch_code == branchCode).FirstOrDefault();
                    receipt.CompanyCode = ksmOld.company_code;
                    receipt.BranchCode = ksmOld.branch_code;
                    receipt.ReceiptNo = receiptNo;
                    receipt.FinYear = Convert.ToInt32(ksmOld.fin_year);
                    receipt.BillNo = ksmOld.bill_no;
                    receipt.BilledDate = ksmOld.bill_date;
                    receipt.CFlag = kpd.cflag;
                    receipt.ReceiptDate = kpd.pay_date;
                    receipt.CustomerName = ksmOld.cust_name;
                    receipt.CustID = Convert.ToInt32(ksmOld.cust_Id);
                    receipt.Address1 = ksmOld.address1;
                    receipt.Address2 = ksmOld.address2;
                    receipt.Address3 = ksmOld.address3;
                    receipt.City = ksmOld.city;
                    receipt.State = ksmOld.state;
                    receipt.Pincode = ksmOld.pin_code;
                    receipt.MobileNo = ksmOld.mobile_no;
                    receipt.Remarks = "";
                }
                List<PaymentVM> lstOfPayment = new List<PaymentVM>();
                foreach (KTTU_PAYMENT_DETAILS paymentDet in kpdp) {
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
                    receipt.Amount = Convert.ToDecimal(receipt.Amount + paymentDet.pay_amt);
                }
                receipt.lstOfPayment = lstOfPayment;
                return receipt;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public CancelCreditReceiptVM GetCreditReceiptDetailsForPrint(string companyCode, string branchCode, int receiptNo, out ErrorVM error)
        {
            error = null;
            int billNo = 0;
            //decimal initialPaidAmount = 0;
            CancelCreditReceiptVM receipt = new CancelCreditReceiptVM();
            try {
                List<KTTU_PAYMENT_DETAILS> kpdp = db.KTTU_PAYMENT_DETAILS.Where(pd => pd.receipt_no == receiptNo
                                                                        && pd.company_code == companyCode
                                                                        && pd.branch_code == branchCode
                                                                        && pd.trans_type == "S").ToList();
                if (kpdp.Count > 0 && kpdp[0].cflag == "Y") {
                    error = new ErrorVM()
                    {
                        index = 0,
                        description = "Receipt cancelled already.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }
                billNo = kpdp[0].series_no;
                KTTU_SALES_MASTER ksm = db.KTTU_SALES_MASTER.Where(sale => sale.bill_no == billNo
                                                                   && sale.company_code == companyCode
                                                                   && sale.branch_code == branchCode).FirstOrDefault();
                if (ksm != null) {
                    receipt.CompanyCode = kpdp[0].company_code;
                    receipt.BranchCode = kpdp[0].branch_code;
                    receipt.ReceiptNo = kpdp[0].receipt_no;
                    receipt.FinYear = Convert.ToInt32(kpdp[0].fin_year);
                    receipt.BillNo = kpdp[0].series_no;
                    receipt.BilledDate = ksm.bill_date;
                    receipt.CFlag = kpdp[0].cflag;
                    receipt.ReceiptDate = kpdp[0].pay_date;
                    receipt.CustomerName = ksm.cust_name;
                    receipt.CustID = Convert.ToInt32(ksm.cust_Id);
                    receipt.Address1 = ksm.address1;
                    receipt.Address2 = ksm.address2;
                    receipt.Address3 = ksm.address3;
                    receipt.City = ksm.city;
                    receipt.State = ksm.state;
                    receipt.Pincode = ksm.pin_code;
                    receipt.MobileNo = ksm.mobile_no;
                    receipt.Remarks = "";
                    //receipt.GrandTotal = Convert.ToDecimal(ksm.grand_total);
                    receipt.GrandTotal = Convert.ToDecimal(ksm.total_bill_amount + ksm.total_tax_amount);
                }
                else {
                    KTTU_SALES_MASTER_OLD ksmOld = db.KTTU_SALES_MASTER_OLD.Where(sal => sal.bill_no == billNo
                                                                                    && sal.company_code == companyCode
                                                                                    && sal.branch_code == branchCode).FirstOrDefault();
                    receipt.CompanyCode = ksmOld.company_code;
                    receipt.BranchCode = ksmOld.branch_code;
                    receipt.ReceiptNo = receiptNo;
                    receipt.FinYear = Convert.ToInt32(ksmOld.fin_year);
                    receipt.BillNo = ksmOld.bill_no;
                    receipt.BilledDate = ksmOld.bill_date;
                    receipt.CFlag = kpdp[0].cflag;
                    receipt.ReceiptDate = kpdp[0].pay_date;
                    receipt.CustomerName = ksmOld.cust_name;
                    receipt.CustID = Convert.ToInt32(ksmOld.cust_Id);
                    receipt.Address1 = ksmOld.address1;
                    receipt.Address2 = ksmOld.address2;
                    receipt.Address3 = ksmOld.address3;
                    receipt.City = ksmOld.city;
                    receipt.State = ksmOld.state;
                    receipt.Pincode = ksmOld.pin_code;
                    receipt.MobileNo = ksmOld.mobile_no;
                    receipt.Remarks = "";
                    //receipt.GrandTotal = Convert.ToDecimal(ksmOld.grand_total);
                    receipt.GrandTotal = Convert.ToDecimal(ksmOld.total_bill_amount + ksmOld.total_tax_amount);
                }
                List<PaymentVM> lstOfPayment = new List<PaymentVM>();
                foreach (KTTU_PAYMENT_DETAILS paymentDet in kpdp) {
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
                    receipt.Amount = Convert.ToDecimal(receipt.Amount + paymentDet.pay_amt);
                }
                receipt.lstOfPayment = lstOfPayment;


                receipt.PaidAmount = receipt.Amount;
                var tillNowPaid = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == billNo && pay.trans_type == "S" && pay.cflag == "N" && pay.receipt_no < receiptNo);
                receipt.CreditBalance = receipt.GrandTotal - Convert.ToDecimal(tillNowPaid.Sum(p => p.pay_amt));
                receipt.BalanceAmount = receipt.CreditBalance - receipt.PaidAmount;
                return receipt;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public bool CancelCreditReceipt(CancelCreditReceiptVM cancelCR, out ErrorVM error)
        {
            error = null;
            //CancelCreditReceiptVM receipt = new CancelCreditReceiptVM();
            try {
                List<KTTU_PAYMENT_DETAILS> kpd = db.KTTU_PAYMENT_DETAILS.Where(pd => pd.receipt_no == cancelCR.ReceiptNo
                                                                        && pd.company_code == cancelCR.CompanyCode
                                                                        && pd.branch_code == cancelCR.BranchCode
                                                                        && pd.trans_type == "S").ToList();
                foreach (KTTU_PAYMENT_DETAILS k in kpd) {
                    if (k.cflag == "Y") {
                        error = new ErrorVM()
                        {
                            description = "Receipt cancelled already.",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                    k.cflag = "Y";
                    k.cancelled_remarks = cancelCR.Remarks;
                    k.cancelled_date = SIGlobals.Globals.GetApplicationDate(cancelCR.CompanyCode, cancelCR.BranchCode).ToString();
                    k.UpdateOn = SIGlobals.Globals.GetDateTime();
                    db.Entry(k).State = System.Data.Entity.EntityState.Modified;
                }
                //Account Posting
                int billNo = kpd[0].series_no;
                List<KSTU_ACC_VOUCHER_TRANSACTIONS> lstOfKAVT = db.KSTU_ACC_VOUCHER_TRANSACTIONS.Where(kavt => kavt.receipt_no == billNo + "," + cancelCR.ReceiptNo
                                                                                            && kavt.company_code == cancelCR.CompanyCode
                                                                                            && kavt.branch_code == cancelCR.BranchCode
                                                                                            && kavt.trans_type == "BREC").ToList();
                foreach (KSTU_ACC_VOUCHER_TRANSACTIONS kavt in lstOfKAVT) {
                    kavt.cflag = "Y";
                    kavt.UpdateOn = SIGlobals.Globals.GetDateTime();
                    db.Entry(kavt).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM()
                {
                    description = excp.Message,
                    ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError
                };
                return false;
            }
        }

        public ProdigyPrintVM PrintCreditReceipt(string companyCode, string branchCode, int receiptNo, out ErrorVM error)
        {
            error = null;
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            PrintConfiguration pc = new PrintConfiguration();
            var printConfig = pc.GetPrintConfigurationForDraftDocuments(companyCode, branchCode, "CRD_REC");
            if (printConfig == "HTML") {
                var htmlPrintData = GetCreditReceiptHTMLPrint(companyCode, branchCode, receiptNo, out error);
                printObject.ContinueNextPrint = false;
                printObject.Data = new PrintConfiguration().Base64Encode(htmlPrintData);
                printObject.PrintType = "HTML";
            }
            else {
                // Right now there is no Dotmatrix print for Credit Receipt
                var dotMatrixPrintData = "";
                printObject.ContinueNextPrint = true;
                printObject.Data = dotMatrixPrintData;
                printObject.PrintType = "RAW";
            }
            return printObject;
        }

        public string GetCreditReceiptHTMLPrint(string companyCode, string branchCode, int receiptNo, out ErrorVM error)
        {
            error = null;
            try {
                KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode && c.branch_code == branchCode).FirstOrDefault();
                KTTU_CREDIT_BILL_DETAILS creditBill = db.KTTU_CREDIT_BILL_DETAILS.Where(crd => crd.company_code == companyCode
                                                                                        && crd.branch_code == branchCode
                                                                                        && crd.credit_bill_receiptNo == receiptNo).FirstOrDefault();
                if(creditBill == null) {
                    error = new ErrorVM { description = $"No credit bill details found for the document no. {receiptNo}.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return string.Empty;
                }

                List<KTTU_PAYMENT_DETAILS> payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.company_code == companyCode
                                                                                        && pay.branch_code == branchCode
                                                                                        && pay.trans_type == "S"
                                                                                        && pay.receipt_no == receiptNo).ToList();
                if (payment == null) {
                    error = new ErrorVM { description = $"No payment information found for the document no. {receiptNo}.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return string.Empty;
                }

                KTTU_SALES_MASTER sales = db.KTTU_SALES_MASTER.Where(s => s.company_code == companyCode
                                                                                        && s.branch_code == branchCode
                                                                                        && s.bill_no == creditBill.bill_no).FirstOrDefault();
                if(sales == null) {
                    error = new ErrorVM { description = $"Sales details not found for the sales invoice No. {creditBill.bill_no}.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return string.Empty;
                }

                //KSTU_CUSTOMER_MASTER customer = new CustomerBL().GetActualCustomerDetails(Convert.ToInt32(sales.cust_Id), sales.mobile_no, companyCode, branchCode);

                string CompanyAddress = string.Empty;
                CompanyAddress = company.company_name;
                CompanyAddress = CompanyAddress + "<br>" + company.address1.ToString();
                CompanyAddress = CompanyAddress + "<br>" + company.address2.ToString();
                CompanyAddress = CompanyAddress + "<br>" + company.address3.ToString();
                CompanyAddress = CompanyAddress + "<br>" + "City:" + company.city.ToString() + " - " + company.pin_code.ToString();
                CompanyAddress = CompanyAddress + "<br>" + "City:" + company.city.ToString();
                CompanyAddress = CompanyAddress + "<br>" + "Phone No: " + company.phone_no.ToString();
                CompanyAddress = CompanyAddress + "<br>" + "TIN: " + company.tin_no.ToString();
                CompanyAddress = CompanyAddress + "<br> E-mail : " + company.email_id.ToString();
                CompanyAddress = CompanyAddress + "<br> Website : " + company.website.ToString();

                StringBuilder sbStart = new StringBuilder();
                sbStart.AppendLine("<html>");
                sbStart.AppendLine("<head>");
                sbStart.AppendLine(SIGlobals.Globals.GetStyleOrder());
                sbStart.AppendLine("</head>");
                sbStart.AppendLine("<body>");
                string strWords = string.Empty;
                sbStart.AppendLine("<Table frame=\"border\" border=\"0\" width=\"700\">");
                for (int j = 0; j < 8; j++) {
                    sbStart.AppendLine("<TR style=border:0>");
                    sbStart.AppendLine(string.Format("<TD style=border:0 colspan = 10 ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    sbStart.AppendLine("</TR>");
                }

                sbStart.AppendLine("<TR>");
                sbStart.AppendLine("<TD style=border:0 width=\"800\" colspan=0 ALIGN = \"CENTER\"><b><h4>CREDIT RECEIPT </h4></b></TD>");
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("</Table>");
                sbStart.AppendLine("<Table font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; border-collapse:collapse; width=\"700\">");

                string Sultation = sales.salutation;
                string Name = sales.cust_name;
                string Address1 = sales.address1;
                string Address2 = sales.address2;
                string Address3 = sales.address3;
                string Email = "";
                string Mobile = sales.mobile_no;
                string Phone = sales.mobile_no;
                string pan = sales.pan_no;
                string custGSTIN = sales.tin;

                if (!string.IsNullOrEmpty(Mobile) && !string.IsNullOrEmpty(Phone))
                    Mobile = Mobile + "/" + Phone;
                if (string.IsNullOrEmpty(Mobile))
                    Mobile = Phone;

                string City = sales.city;
                string pincode = sales.pin_code;
                if (!string.IsNullOrEmpty(pincode))
                    City = City + " - " + pincode;

                string Address = string.Empty;

                if (Address1 != string.Empty)
                    Address = Address + "<br>" + Address1;
                if (Address2 != string.Empty)
                    Address = Address + "<br>" + Address2;
                if (Address3 != string.Empty)
                    Address = Address + "<br>" + Address3;
                if (Email != string.Empty)
                    Email = Email + "<br>";
                sbStart.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sbStart.AppendLine(string.Format("<TD width=\"400\" colspan=3 ALIGN = \"LEFT\"><b>CUSTOMER DETAILS</b></TD>"));
                sbStart.AppendLine("<TD width=\"350\" colspan=3 ALIGN = \"LEFT\"><b>BILL RECEIPT</b></TD>");
                sbStart.AppendLine(string.Format("<TD width=\"250\" colspan=4 ALIGN = \"LEFT\"><b>{0}</b></TD>", "SHOWROOM ADDRESS"));
                sbStart.AppendLine("</TR>");

                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD colspan=3 ALIGN = \"LEFT\"><b>{0} <BR> {1} <br> {2}  <br>  {3} <br>  {4} </b>", Name, Address, Email, City, Mobile));
                if (pan != string.Empty)
                    sbStart.AppendLine(string.Format("<br><b> PAN : {0}</b>", pan));
                if (custGSTIN != string.Empty)
                    sbStart.AppendLine(string.Format("<br><b> GSTIN : {0}</b>", custGSTIN));
                sbStart.AppendLine("</td>");
                sbStart.AppendLine(string.Format("<TD colspan=3 ALIGN = \"LEFT\"><b> Receipt No  : {0} <br> Date    : {1} <br></b></TD>", receiptNo, Convert.ToDateTime(creditBill.Paid_date).ToString("dd/MM/yyyy") ));

                sbStart.AppendLine(string.Format("<TD colspan=4 ALIGN = \"LEFT\"><b>{0}</b></TD>", CompanyAddress));
                sbStart.AppendLine("</TR>");

                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD colspan=11 ALIGN = \"CENTER\"><b>ORIGINAL / DUPLICATE <br></b></TD>"));
                sbStart.AppendLine("</TR>");

                NumberToWords objNWClass = new NumberToWords();
                objNWClass.ConvertNumberToWords(Convert.ToDouble(creditBill.amount_paid), out strWords);
                strWords = strWords.Replace("Rupees", "");
                strWords = "Rupees " + strWords;

                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD colspan=10 ALIGN = \"LEFT\"><b>Received with thanks from Mr./Mrs./M/s.{0}{1}{1}</b></TD>", sales.cust_name, " &nbsp"));
                sbStart.AppendLine("</TR>");

                sbStart.AppendLine(string.Format("<TR><TD colspan=10 ALIGN = \"LEFT\"><b>The sum of Rs.{0}[{1}]</b></TD></TR>", Convert.ToString(creditBill.amount_paid), strWords + "&nbsp"));
                sbStart.AppendLine(string.Format("<TR><TD colspan=10 ALIGN = \"LEFT\"><b>Towards the BillNo:{0}   Dated:{1}</b></TD></TR>", Convert.ToString(creditBill.bill_no), "&nbsp" + sales.bill_date.ToString("dd/MM/yyyy") + "&nbsp"));

                decimal BalAmt1 = Convert.ToDecimal(creditBill.balance_amount);
                decimal PaidAmt1 = Convert.ToDecimal(creditBill.amount_paid);
                decimal PaidBalAmt1 = (PaidAmt1 + BalAmt1);

                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD colspan=10 ALIGN = \"LEFT\"><b>Customer Credit Balance Amount Rs : {0}{1}{1}</b></TD>", PaidBalAmt1, "&nbsp"));
                sbStart.AppendLine("</TR>");

                for (int i = 0; i < payment.Count; i++) {
                    string payMode = payment[i].pay_mode;
                    decimal PayAmt = Convert.ToDecimal(payment[i].pay_amt);
                    string branch = payMode.Substring(0, 1);
                    sbStart.AppendLine("<TR>");
                    if (string.Compare(payMode, "C") == 0) {
                        sbStart.AppendLine(string.Format("<TD colspan=10 ALIGN = \"LEFT\"><b>{0}{1}</b></TD>", "Cash Received :" + PayAmt.ToString("N"), "&nbsp"));
                    }
                    else if (string.Compare(payMode, "Q") == 0 || string.Compare(payMode, "D") == 0) {
                        sbStart.AppendLine(string.Format("<TD colspan=10 ALIGN = \"LEFT\"><b>By Cheque/DD :{0}{1}</b></TD>", PayAmt.ToString("N") + "/" + "Cheque/DD No :" + payment[i].cheque_no + "/" + "Dated :" + payment[i].cheque_date + "/" + "Drawn On:" + payment[i].bank, "&nbsp"));
                    }
                    else if (string.Compare(payMode, "R") == 0) {
                        string Bank = string.Empty;
                        if (payment[i].bank != string.Empty) {
                            Bank = payment[i].bank.Trim();
                        }
                        sbStart.AppendLine(string.Format("<TD colspan=10 ALIGN = \"LEFT\"><b>By CC :{0}{1}</b></TD>", PayAmt.ToString("N") + "/" + payment[i].bank_name + "/" + Bank + "/" + payment[i].card_type + "/" + payment[i].card_app_no, "&nbsp"));
                    }
                    else if (string.Compare(payMode, "PB") == 0) {
                        sbStart.AppendLine(string.Format("<TD colspan=10 ALIGN = \"LEFT\"><b>By Purchase :{0}{1}</b></TD>", PayAmt.ToString("N") + "/" + "Adjusted Towards Purchase Bill No:" + payment[i].Ref_BillNo, "&nbsp"));
                    }
                    else if (string.Compare(payMode, "SR") == 0) {
                        sbStart.AppendLine(string.Format("<TD colspan=10 ALIGN = \"LEFT\"><b>By Sales Return :{0}{1}</b></TD>", PayAmt.ToString("N") + "/" + " Adjusted Towards Sales Return No:" + payment[i].Ref_BillNo, "&nbsp"));
                    }
                    else if (string.Compare(branch, "B") == 0) {
                        sbStart.AppendLine(string.Format("<TD colspan=10 ALIGN = \"LEFT\"><b>By Inter-Branch  : {0}{1}</b></TD>", PayAmt.ToString("N") + "/" + " Received At  : " + payment[i].party_code, "&nbsp"));
                    }
                    else if (string.Compare(payMode, "EP") == 0) {
                        sbStart.AppendLine(string.Format("<TD colspan=10 ALIGN = \"LEFT\"><b>E-Payment  : {0}{1}</b></TD>", PayAmt.ToString("N"), "&nbsp"));
                    }
                    else {
                        string payCode = payment[i].pay_mode;
                        KTTS_PAYMENT_MASTER paymentMaster = db.KTTS_PAYMENT_MASTER.Where(p => p.company_code == companyCode && p.branch_code == branchCode && p.payment_code == payCode).FirstOrDefault();
                        sbStart.AppendLine(string.Format("<TD colspan=10 ALIGN = \"LEFT\"><b>{0} - {3} : {1}{2}</b></TD>", paymentMaster.payment_name, PayAmt.ToString("N"), "&nbsp", payment[i].pay_details.ToString()));
                    }
                    sbStart.AppendLine("</TR>");
                }

                List<KTTU_PAYMENT_DETAILS> chitPay = payment.Where(pay => pay.pay_mode == "CT").ToList();
                if (chitPay.Count > 0) {
                    bool print = false;
                    decimal chitAmount = 0;
                    string definition = string.Empty;

                    for (int k = 0; k < chitPay.Count; k++) {
                        definition += string.Format("{0}/{1}/{2} ", chitPay[k].scheme_code, chitPay[k].group_code, chitPay[k].Ref_BillNo);
                        chitAmount += Convert.ToDecimal(chitPay[k].pay_amt);

                        if (chitPay.Count == 1 || (chitPay.Count > 1 && k > 0)) {
                            if ((chitPay.Count == 1) || (chitPay.Count == k + 1)) {
                                print = true;
                            }
                        }
                        if (k < chitPay.Count - 1 && (Convert.ToInt32(chitPay[k].Ref_BillNo) != (Convert.ToInt32(chitPay[k + 1].Ref_BillNo) - 1)
                            || !(chitPay[k + 1].scheme_code.Equals(chitPay[k].scheme_code)) || !(chitPay[k + 1].group_code.Equals(chitPay[k].group_code)))) {
                            print = true;
                        }
                        if (print) {
                            string[] chiNoPatch = definition.Split(' ');
                            definition = chiNoPatch[0];
                            if (chiNoPatch.Length > 2) {
                                definition += " To " + chiNoPatch[chiNoPatch.Length - 2];
                            }
                            sbStart.AppendLine("<TR>");
                            sbStart.AppendLine(string.Format("<TD colspan=10 ALIGN = \"LEFT\"><b> {0}</b></TD>", "Jewel Savings Scheme " + definition.Trim() + " :" + chitAmount + "&nbsp"));
                            sbStart.AppendLine("</TR>");
                            definition = string.Empty;
                            chitAmount = 0;
                            print = false;
                        }
                    }
                }

                if (payment != null && payment.Count > 0) {
                    decimal TotCardChg = 0;
                    object tempTotCardChg = payment.Sum(pay => pay.CardCharges);
                    if (tempTotCardChg != null && tempTotCardChg != DBNull.Value)
                        TotCardChg = (decimal)tempTotCardChg;
                    if (TotCardChg > 0)
                        sbStart.AppendLine(string.Format("<TR><TD colspan=10 ALIGN = \"LEFT\"><b>Card Charges :{0}{1}</b></TD>", TotCardChg.ToString("N"), "&nbsp"));
                }

                decimal BalAmt = Convert.ToDecimal(creditBill.balance_amount);
                decimal PaidAmt = Convert.ToDecimal(creditBill.amount_paid);
                decimal PaidBalAmt = (PaidAmt + BalAmt);
                if (BalAmt >= 0) {
                    sbStart.AppendLine(string.Format("<TR><TD colspan=10 ALIGN = \"LEFT\"><b>Balance Amount :{0}{1}{1}</b></TD></TR>", Convert.ToString(creditBill.balance_amount), "&nbsp"));
                    sbStart.AppendLine("<TR>");
                }
                else {
                    sbStart.AppendLine(string.Format("<TR><TD colspan=10 ALIGN = \"LEFT\"><b>Balance Amount :{0}{1}{1}</b></TD></TR>", "0.00", "&nbsp"));
                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TR><TD colspan=10 ALIGN = \"LEFT\"><b>Payable Amount :{0}{1}{1}</b></TD></TR>", -BalAmt, "&nbsp"));
                    sbStart.AppendLine("<TR>");
                }

                // I think bellow code is not required for Credit Receipt.
                //string strPayDetails1 = string.Format("SELECT PD.pay_mode,PD.pay_amt,PD.trans_type, \n" +
                //                        " convert(int,PD.Ref_BillNo) as Ref_BillNo,PD.scheme_code,PD.group_code from KTTU_PAYMENT_DETAILS PD \n" +
                //                        " where PD.trans_type in ('PC') and PD.receipt_no ={0} and PD.company_code='{1}' and PD.branch_code='{2}' \n" +
                //                        " ORDER BY PD.pay_mode,PD.scheme_code,PD.GROUP_code,Ref_BillNo \n"
                //                        , CreditbillReceiptNo, CGlobals.CompanyCode, CGlobals.BranchCode);

                //DataTable dtPayment1 = CGlobals.GetDataTable(strPayDetails1);

                //DataRow[] drChitDetails1 = dtPayment1.Select("trans_type = 'PC'");


                //if (drChitDetails1.Length > 0) {
                //    for (int i = 0; i < drChitDetails1.Length; i++) {
                //        string payMode1 = dtPayment1.Rows[i]["pay_mode"].ToString();
                //        decimal PayAmt1 = Convert.ToDecimal(dtPayment1.Rows[i]["pay_amt"]);
                //        string branch = payMode1.Substring(0, 1);
                //        PayAmt1 = Math.Round(PayAmt1, 2, MidpointRounding.ToEven);
                //        string transtype1 = dtPayment1.Rows[i]["trans_type"].ToString();

                //        if (string.Compare(transtype1, "PC") == 0 && string.Compare(payMode1, "C") == 0) {
                //            sbStart.AppendLine(string.Format("<TR><TD colspan=10 ALIGN = \"LEFT\"><b>{0}{1}</b></TD>", "Cash paid :" + PayAmt1.ToString("N"), "&nbsp"));
                //            sbStart.AppendLine("<TR>");
                //        }
                //        if (string.Compare(transtype1, "PC") == 0 && string.Compare(payMode1, "EP") == 0) {
                //            sbStart.AppendLine(string.Format("<TR><TD colspan=10 ALIGN = \"LEFT\"><b>{0}{1}</b></TD>", "E-PAY to Customer :" + PayAmt1.ToString("N"), "&nbsp"));
                //            sbStart.AppendLine("<TR>");
                //        }
                //        else if (string.Compare(transtype1, "PC") == 0 && string.Compare(payMode1, "Q") == 0) {
                //            sbStart.AppendLine(string.Format("<TR><TD colspan=10 ALIGN = \"LEFT\"><b>{0}{1}</b></TD>", "Cheque Issued :" + PayAmt1.ToString("N"), "&nbsp"));
                //            sbStart.AppendLine("<TR>");
                //        }
                //        else if (string.Compare(transtype1, "PC") == 0 && string.Compare(payMode1, "OP") == 0) {
                //            sbStart.AppendLine(string.Format("<TR><TD colspan=10 ALIGN = \"LEFT\"><b>{0}: Rs={1}{2}</b></TD>", "Adjusted Towards Order No -" + dtPayment1.Rows[i]["Ref_BillNo"], PayAmt1.ToString("N"), "&nbsp"));
                //            sbStart.AppendLine("<TR>");
                //        }
                //    }
                //}
                sbStart.AppendLine(string.Format("<TD colspan=10 ALIGN = \"left\"><b>Operator Code :{0} </b></TD>", payment[0].operator_code.ToString(), "&nbsp"));
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD colspan = 5 ALIGN = \"LEFT\"><b><br><br><br>{0}</b></TD>", "Customer Signature"));
                sbStart.AppendLine(string.Format("<TD colspan = 5 ALIGN = \"RIGHT\"><b>For {0}<br><br><br>{1}</b></TD>", company.company_name, "Authorized Signatory"));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine();
                sbStart.AppendLine();
                sbStart.AppendLine("</TABLE> </BODY> </HTML>");
                return sbStart.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }
        #endregion

        #region Private Methods
        private ErrorVM AccountPostingWithProedure(string companyCode, string branchCode, int receiptNo)
        {
            //try {
            //    //ObjectParameter errorMessage = new ObjectParameter("errorMessage", typeof(string));
            //    //var data = db.usp_createCreditReceiptPostingVouchers(companyCode, branchCode, Convert.ToString(receiptNo), errorMessage);
            //    //string message = Convert.ToString(errorMessage.Value);
            //    //if (data.Count() <= 0 || message != "") {
            //    //    return new ErrorVM() { description = "Error Occurred while Updating Accounts. " + errorMessage, field = "Account Update", index = 0 };
            //    //}

            //    string sql = "EXEC [dbo].[usp_createCreditReceiptPostingVouchers] \n"
            //                + "  @p0, \n"
            //                + "  @p1,\n"
            //                + "  @p2,\n"
            //                + "  @p3";
            //    List<object> parameterList = new List<object>();
            //    parameterList.Add(companyCode);
            //    parameterList.Add(branchCode);
            //    parameterList.Add(Convert.ToString(receiptNo));
            //    parameterList.Add("");
            //    object[] parametersArray = parameterList.ToArray();
            //    int recordsAffected = SIGlobals.Globals.ExecuteSQL(sql, db, parametersArray);
            //}
            //catch (Exception excp) {
            //    return new ErrorVM() { description = excp.InnerException.Message, field = "Account Update", index = 0, ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError };
            //}
            //return null;

            try {
                string errorFromProc = string.Empty;
                ObjectParameter errorMessage = new ObjectParameter("errorMsg", typeof(string));
                ObjectParameter outputVal = new ObjectParameter("outValue", typeof(int));
                var result = db.usp_createCreditReceiptPostingVouchers_FuncImport(companyCode, branchCode, Convert.ToString(receiptNo), outputVal, errorMessage);
                return new CommonBL().HandleAccountPostingProcs(outputVal, errorMessage);
            }
            catch (Exception excp) {
                return new ErrorVM() { description = excp.InnerException.Message, field = "Account Update", index = 0 };
            }
        }
        #endregion
    }
}
