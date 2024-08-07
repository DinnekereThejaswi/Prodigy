using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class SKUVM
    {
        public string ObjID { get; set; }
        [Required]
        [MaxLength(3,ErrorMessage ="Company Code should be max 3 digits")]
        [MinLength(2, ErrorMessage = "Company Code should be Min 2 digits")]
        public string CompanyCode { get; set; }

        [Required]
        [MaxLength(3, ErrorMessage = "Branch Code should be max 3 digits")]
        [MinLength(2, ErrorMessage = "Branch Code should be Min 2 digits")]
        public string BranchCode { get; set; }

        [Required]
        public string GSCode { get; set; }

        [Required]
        public string SKUID { get; set; }

        [Required]
        public string DesignCode { get; set; }
        public Nullable<decimal> Weight { get; set; }

        [Required]
        public string ItemCode { get; set; }
        public string ObjStatus { get; set; } = "O";
        public System.Guid UniqRowID { get; set; }
    }
}
