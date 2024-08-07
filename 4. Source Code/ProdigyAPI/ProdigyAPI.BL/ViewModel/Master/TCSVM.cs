using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class TCSVM
    {
        [Required]
        [MaxLength(3, ErrorMessage = "Company code length is less than or equal to 3 Digits")]
        [MinLength(2, ErrorMessage = "Company code length is Minimum 2 Digits")]
        [Company]
        public string CompanyCode { get; set; }

        [Required]
        [MaxLength(3, ErrorMessage = "Branch code length is less than or equal to 3 Digits")]
        [MinLength(2, ErrorMessage = "Branch code length is Minimum 2 Digits")]
        [Branch]
        public string BranchCode { get; set; }

        [Required]
        [RegularExpression("Y|N", ErrorMessage = "The value must be either 'Y' or 'N' only.")]
        public string IsWithKYC { get; set; }

        [Required]
        public decimal AmountLimit { get; set; }

        [Required]
        public decimal TCSPercent { get; set; }

        [Required]
        public int AccCode { get; set; }

        [Required]
        public string AccName { get; set; }

        [Required]
        public string CalculatedOn { get; set; }

        [Required]
        public Nullable<System.DateTime> EffectiveDate { get; set; }        
        [Required]
        public string TransactionType { get; set; }

        [Required]
        [RegularExpression("Y|N", ErrorMessage = "The value must be either 'Y' or 'N' only.")]
        public string IsTDS { get; set; }
    }
}
