using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Accounts
{
    public class MasterGroupVM
    {
        public string ObjID { get; set; }

        [Required]
        [StringLength(maximumLength: 2, MinimumLength = 2, ErrorMessage = "Should be 2 Letters")]
        public string CompanyCode { get; set; }

        [Required]
        [StringLength(maximumLength: 3, MinimumLength = 3, ErrorMessage = "Should be 3 Letters")]
        public string BranchCode { get; set; }
        public int GroupID { get; set; }

        [Required]
        [StringLength(maximumLength: 50, MinimumLength = 3)]
        public string GroupName { get; set; }

        [Required]
        [StringLength(maximumLength: 1, MinimumLength = 1, ErrorMessage = "Should be 1 Letter")]
        public string GroupType { get; set; }
        public string Under { get; set; }

        public string GroupDescription { get; set; }
        public string IsTrading { get; set; }

        [Required]
        [StringLength(maximumLength: 1, MinimumLength = 1, ErrorMessage = "Either 'O' or 'C'")]
        public string ObjStatus { get; set; } = "O";
        public Nullable<int> ParentGroupID { get; set; }
        public string NewGroupCode { get; set; }
        public string NewSubGroupCode { get; set; }
    }
}
