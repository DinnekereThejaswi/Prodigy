using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Author: Mustureswara M M
/// Date: 2021-05-19
/// </summary>
namespace ProdigyAPI.BL.ViewModel.Accounts
{
    public class AccountsVoucherEntryVM
    {

    }

    public class TDSExpanseDetailsVM
    {
        public string ObjID { get; set; }
        public int ExpenseNo { get; set; }
        public System.DateTime ExpenseDate { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public int SNo { get; set; }
        public int AccCode { get; set; }
        public string AccName { get; set; }
        public string InvoiceNo { get; set; }
        public System.DateTime InvoiceDate { get; set; }
        public decimal Amount { get; set; }
        public string TaxName { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TDSPercentage { get; set; }
        public decimal TDSAmount { get; set; }
        public string CFlag { get; set; }
        public string CancelledBy { get; set; }
        public string OperatorCode { get; set; }
        public string Description { get; set; }
        public string ObjStatus { get; set; }
        public Nullable<decimal> SGSTPercent { get; set; }
        public Nullable<decimal> SGSTAmount { get; set; }
        public Nullable<decimal> CGSTPercent { get; set; }
        public Nullable<decimal> CGSTAmount { get; set; }
        public Nullable<decimal> IGSTPercent { get; set; }
        public Nullable<decimal> IGSTAmount { get; set; }
        public string HSN { get; set; }
        public string GSTGroupCode { get; set; }
        public string AccType { get; set; }
        public Nullable<int> ChqNo { get; set; }
        public Nullable<System.DateTime> ChqDate { get; set; }
        public string GSTNo { get; set; }
        public string OtherPartyName { get; set; }
        public string PartyCode { get; set; }
        public Nullable<decimal> CESSPercent { get; set; }
        public Nullable<decimal> CESSAmount { get; set; }
        public string Remarks { get; set; }
        public string PANNo { get; set; }
        public string IsEligible { get; set; }
        public string HSNDescription { get; set; }
        public Nullable<decimal> RoundOff { get; set; }
        public Nullable<decimal> FinalAmount { get; set; }

        public int TDSID { get; set; }
        public int CessAccountCode { get; set; }
        public string AccountType { get; set; }
        public bool IsRegistered { get; set; }
    }

    public class TDSExpenseCancelVM
    {
        [Required]
        public int ExpenseNo { get; set; }
        [Required]
        public string CompanyCode { get; set; }
        [Required]
        public string BranchCode { get; set; }
        [Required]
        public string Remarks { get; set; }
    }
}
