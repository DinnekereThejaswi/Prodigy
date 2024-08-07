using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Accounts
{
    public class NarrationVM
    {
        public string ObjID { get; set; }
        [Required]
        [StringLength(maximumLength: 2, MinimumLength = 2, ErrorMessage = "Should be 2 Letters")]
        public string CompanyCode { get; set; }

        [Required]
        [StringLength(maximumLength: 3, MinimumLength = 3, ErrorMessage = "Should be 3 Letters")]
        public string BranchCode { get; set; }
        public int NarrID { get; set; }
        public string Narration { get; set; }
    }
}
