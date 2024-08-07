using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Accounts
{
    public class CashBackTotalVM
    {
        [Required]
        [StringLength(5, MinimumLength = 2, ErrorMessage = "Must be at least 2 characters long.")]
        public string CompanyCode { get; set; }

        [Required]
        [StringLength(5, MinimumLength = 2, ErrorMessage = "Must be at least 2 characters long.")]
        public string BranchCode { get; set; }

        [Required]
        [Range(0.00, 9999999, ErrorMessage = "The field {0} must be greater than {1}.")]
        public decimal TotalBillAmount { get; set; }

        [Required]
        [Range(0.00, 9999999, ErrorMessage = "The field {0} must be greater than {1}.")]
        public decimal TotalEligableCashBackAmount { get; set; }

        [Required]
        [Range(0.00, 9999999, ErrorMessage = "The field {0} must be greater than {1}.")]
        public decimal TotalActualCashBackAmount { get; set; }

        [Required]
        public string OperatorCode { get; set; }

        [Required]
        [Range(typeof(bool),"false", "true", ErrorMessage = "This field must be true or false")]
        public bool IsEPayment { get; set; }

        public List<CashBackVM> ListCashBackVM { get; set; }
    }
    public class CashBackVM
    {
        [Required]
        [StringLength(5, MinimumLength = 2, ErrorMessage = "Must be at least 2 characters long.")]
        public string CompanyCode { get; set; }

        [Required]
        [StringLength(5, MinimumLength = 3, ErrorMessage = "Must be at least 3 characters long.")]
        public string BranchCode { get; set; }

        [Required(ErrorMessage = "Bill Number is required")]
        public int BillNo { get; set; }

        [Required]
        [Range(0.00, 9999999, ErrorMessage = "The field {0} must be greater than {1}.")]
        public decimal BillAmt { get; set; }

        [Required]
        [Range(0.00, 9999999, ErrorMessage = "The field {0} must be greater than {1}.")]
        public decimal EligableCashBack { get; set; }

        [Required]
        [Range(0.00, 9999999, ErrorMessage = "The field {0} must be greater than {1}.")]
        public decimal ActualCashBack { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Remarks { get; set; }
    }
    public class CashBackResponseVM
    {
        public int CashBackNo { get; set; }

        public int VoucherNo { get; set; }

        public int AccouuntCodeMaster { get; set; }

        public string TranType { get; set; }

        public string AccType { get; set; }
    }
}
