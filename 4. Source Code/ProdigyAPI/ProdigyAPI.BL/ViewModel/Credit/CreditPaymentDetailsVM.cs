using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Credit
{
    public class CreditPaymentDetailsVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int SeriesNo { get; set; }
        public int ReceiptNo { get; set; }
        public int SNo { get; set; }
        public string TransType { get; set; }
        public string PayMode { get; set; }
        public string PayDetails { get; set; }
        public Nullable<System.DateTime> PayDate { get; set; }
        public Nullable<decimal> PayAmount { get; set; }
        public string RefBillNo { get; set; }
        public string PartyCode { get; set; }
        public string BillCounter { get; set; }
        public string IsPaid { get; set; }
        public string Bank { get; set; }
        public string BankName { get; set; }
        public Nullable<System.DateTime> ChequeDate { get; set; }
        public string CardType { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public string CFlag { get; set; }
        public string CardAppNo { get; set; }
        public string SchemeCode { get; set; }
        public string SalBillType { get; set; }
        public string OperatorCode { get; set; }
        public Nullable<int> SessionNo { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string GroupCode { get; set; }
        public Nullable<decimal> AmtReceived { get; set; }
        public Nullable<decimal> BonusAmt { get; set; }
        public Nullable<decimal> WinAmt { get; set; }
        public string CTBranch { get; set; }
        public Nullable<int> FinYear { get; set; }
        public Nullable<decimal> CardCharges { get; set; }
        public string ChequeNo { get; set; }
        public string NewBillNo { get; set; }
        public Nullable<decimal> AddDisc { get; set; }
        public string IsOrderManual { get; set; }
        public Nullable<decimal> CurrencyValue { get; set; }
        public Nullable<decimal> ExchangeRate { get; set; }
        public string CurrencyType { get; set; }
        public Nullable<decimal> TaxPercentage { get; set; }
        public string CancelledBy { get; set; }
        public string CancelledRemarks { get; set; }
        public string CancelledDate { get; set; }
        public string IsExchange { get; set; }
        public Nullable<int> ExchangeNo { get; set; }
        public string NewReceiptNo { get; set; }
        public Nullable<decimal> GiftAmount { get; set; }
        public string CardSwipedBy { get; set; }
        public Nullable<int> Version { get; set; }
        public string GSTGroupCode { get; set; }
        public Nullable<decimal> SGSTPercent { get; set; }
        public Nullable<decimal> CGSTPercent { get; set; }
        public Nullable<decimal> IGSTPercent { get; set; }
        public string HSN { get; set; }
        public Nullable<decimal> SGSTAmount { get; set; }
        public Nullable<decimal> CGSTAmount { get; set; }
        public Nullable<decimal> IGSTAmount { get; set; }
        public Nullable<decimal> PayAmountBeforeTax { get; set; }
        public Nullable<decimal> PayTaxAmount { get; set; }
        public string IsCashMarking { get; set; }
        public Nullable<decimal> ReceivedCash { get; set; }
        public Nullable<decimal> AdditionalDiscount { get; set; }

    }
    public class CreditBankVm
    {
        public string Bank { get; set; }
        public string BankName { get; set; }
    }
}
