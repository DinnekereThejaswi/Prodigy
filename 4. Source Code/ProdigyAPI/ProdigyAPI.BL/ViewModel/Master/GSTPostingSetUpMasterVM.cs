using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class GSTPostingSetUpMasterVM
    {
        public int ID { get; set; }
        [Required]
        public string GSTGroupCode { get; set; }
        [Required]
        public string GSTComponentCode { get; set; }
        [Required]
        public DateTime EffectiveDate { get; set; }
        [Required]
        public decimal GSTPercent { get; set; }
        [Required]
        public int CalculationOrder { get; set; }
        public int? ReceivableAccount { get; set; }
        public string ReceivableAccountName { get; set; }
        public int? PayableAccount { get; set; }
        public string PaybaleAccountName { get; set; }
        public int? ExpenseAccount { get; set; }
        public int? RefundAccount { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public bool? IsRegistered { get; set; }
        [Required]
        public string CompanyCode { get; set; }
        [Required]
        public string BranchCode { get; set; }
    }
}
