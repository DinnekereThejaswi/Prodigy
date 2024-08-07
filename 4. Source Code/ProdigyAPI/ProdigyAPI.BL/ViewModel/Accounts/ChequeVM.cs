using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Accounts
{
    public class ChequeVM
    {
        public string ObjID { get; set; }

        [Required]
        [StringLength(5, MinimumLength = 2, ErrorMessage = "Company Code Must be at least 2 characters long.")]
        public string CompanyCode { get; set; }

        [Required]
        [StringLength(5, MinimumLength = 2, ErrorMessage = "Branch Code Must be at least 2 characters long.")]
        public string BranchCode { get; set; }

        [Required]
        public int AccCode { get; set; }

        public int ChqNo { get; set; }

        [Required]
        public int NoOfChqs { get; set; }

        [Required]
        public int ChqStartNo { get; set; }

        [Required]
        public int ChqEndNo { get; set; }
        public System.DateTime ChqIssueDate { get; set; }
        public string ChqIssued { get; set; }
        public string ChqClosed { get; set; }
        public string ChqClosedBy { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string ChqClosedRemarks { get; set; }
        public Nullable<decimal> MaxAmount { get; set; }
        public System.Guid UniqRowID { get; set; }
    }
}
