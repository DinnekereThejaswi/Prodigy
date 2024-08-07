using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class CardCommissionVM
    {
        public string ObjID { get; set; }

        [Required]
        [MaxLength(3, ErrorMessage = "Company Code should be two digit or more")]
        public string CompanyCode { get; set; }
        [Required]
        [MaxLength(3, ErrorMessage = "Branch Code should be two digit or more")]
        public string BranchCode { get; set; }
        [Required]
        public string Bank { get; set; }
        [Required]
        public decimal Charge { get; set; }
        [Required]
        public decimal ServiceTax { get; set; }
        [Required]
        public int AccCode { get; set; }
        [Required]
        public int BankAccCode { get; set; }
        [Required]
        public Nullable<int> DispSeq { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        [Required]
        public Nullable<decimal> CustCharge { get; set; }
    }
}
