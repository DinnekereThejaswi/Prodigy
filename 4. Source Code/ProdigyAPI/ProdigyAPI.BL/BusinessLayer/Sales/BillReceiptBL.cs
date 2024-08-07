using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Payment;
using ProdigyAPI.BL.ViewModel.Sales;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Sales
{
    /// <summary>
    /// This is Bill Receipt Business Layer contains all the methods related to it.
    /// </summary>
    public class BillReceiptBL
    {
        #region Declaration
        MagnaDbEntities db = null;
        #endregion

        #region Constructors
        public BillReceiptBL()
        {
            db = new MagnaDbEntities();
        }

        public BillReceiptBL(MagnaDbEntities dbContext)
        {
            db = dbContext;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get All Payment Modes related to Bill Receipt Details.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        public dynamic GetBillReceiptPayModes(string companyCode, string branchCode)
        {
            string[] notInPayModes = { "OP", "SE", "PE", "DD", "BC" };
            var payModes = db.KTTS_PAYMENT_MASTER.Where(m => m.company_code == companyCode
                                                        && m.branch_code == branchCode
                                                        && !notInPayModes.Contains(m.payment_code))
                                                        .Select(modes => new { PayMode = modes.payment_code, PayName = modes.payment_name });
            return payModes;
        }

        /// <summary>
        /// This method used to Save Bill Receipt Details.
        /// </summary>
        /// <param name="lstOfPayment"></param>
        /// <returns>boolean true or false</returns>
        public bool SaveBillReceiptDetails(BillReceiptVM billReceiptPay, out ErrorVM error)
        {
            error = null;
            decimal delAmt = 0;
            decimal paidAmt = 0;

            if (billReceiptPay == null) {
                error = new ErrorVM()
                {
                    description = "Invalid Receipt Details",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }

            string companyCode = billReceiptPay.CompanyCode;
            string branchCode = billReceiptPay.BranchCode;
            int billNo = billReceiptPay.BillNo;

            DateTime appDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
            int finYear = SIGlobals.Globals.GetFinancialYear(db, companyCode, branchCode);
            string objectID = SIGlobals.Globals.GetMagnaGUID("KTTU_SALES_MASTER", billNo, companyCode, branchCode);


            #region Validation
            KTTU_SALES_MASTER salesMaster = db.KTTU_SALES_MASTER.Where(sal => sal.company_code == companyCode && sal.branch_code == branchCode && sal.bill_no == billNo).FirstOrDefault();
            if (salesMaster.iscredit == "Y") {
                error = new ErrorVM()
                {
                    description = "Cannot change payment mode of a credit bill.",
                    ErrorStatusCode = System.Net.HttpStatusCode.Continue
                };
                return false;
            }
            foreach (PaymentVM pay in billReceiptPay.AddPayments) {
                paidAmt = paidAmt + Convert.ToDecimal(pay.PayAmount);
            }
            List<KTTU_PAYMENT_DETAILS> payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == billReceiptPay.BillNo
                                                                            && pay.company_code == billReceiptPay.CompanyCode
                                                                            && pay.branch_code == billReceiptPay.BranchCode
                                                                            && pay.trans_type == "S").ToList();
            delAmt = Convert.ToDecimal(payment.Sum(p => p.pay_amt));

            if (paidAmt != delAmt) {
                error = new ErrorVM()
                {
                    description = "Payment Amount missmatch found.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            #endregion

            try {

                #region Deleting and Saving
                int slNo = 1;
                if (billReceiptPay.AddPayments.Count > 0) {
                    db.KTTU_PAYMENT_DETAILS.RemoveRange(payment);
                    foreach (PaymentVM pvm in billReceiptPay.AddPayments) {
                        KTTU_PAYMENT_DETAILS kpd = new KTTU_PAYMENT_DETAILS();
                        kpd.obj_id = objectID;
                        kpd.company_code = pvm.CompanyCode;
                        kpd.branch_code = pvm.BranchCode;
                        kpd.series_no = billReceiptPay.BillNo;
                        kpd.receipt_no = 0;
                        kpd.sno = slNo;
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
                        kpd.bank_name = pvm.BankName == null ? "" : pvm.BankName; ;
                        kpd.cheque_date = pvm.ChequeDate == null ? appDate : pvm.ChequeDate;
                        kpd.card_type = pvm.CardType == null ? "" : pvm.CardType;
                        kpd.expiry_date = pvm.ExpiryDate == null ? appDate : pvm.ExpiryDate;
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
                        slNo = slNo + 1;
                    }
                }
                #endregion

                #region Account Posting
                List<KSTU_ACC_SUBSIDIARY_VOUCHER_TRANSACTIONS> accountsEntry = db.KSTU_ACC_SUBSIDIARY_VOUCHER_TRANSACTIONS.Where(acc => acc.company_code == billReceiptPay.CompanyCode
                                                                            && acc.branch_code == billReceiptPay.BranchCode
                                                                            && acc.bill_no == billReceiptPay.BillNo.ToString() && acc.trans_type == "BILL").ToList();
                foreach (KSTU_ACC_SUBSIDIARY_VOUCHER_TRANSACTIONS voucher in accountsEntry) {
                    voucher.cflag = "Y";
                    voucher.UpdateOn = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                    db.Entry(voucher).State = System.Data.Entity.EntityState.Modified;
                }
                ErrorVM procError = new SalesBillingBL().AccountPostingWithProedure(db, companyCode, branchCode, billNo);
                if (procError != null) {
                    error = new ErrorVM()
                    {
                        field = "Account Update",
                        index = 0,
                        description = procError.description,
                        ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError
                    };
                    return false;
                }
                #endregion

                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }
        #endregion
    }
}
