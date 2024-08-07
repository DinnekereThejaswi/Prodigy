using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Author: Mustureswara M M (Eshwar)
/// Dated: 20th April 2021
/// Module Type: View Model
/// </summary>
namespace ProdigyAPI.BL.ViewModel.Accounts
{
    /// <summary>
    /// View Model Represents Account Code Master
    /// </summary>
    public class AccountCodeMasterVM
    {
        public int ObjID { get; set; }

        [Required]
        [StringLength(5,
            MinimumLength = 2,
            ErrorMessage = "Company Code be at least 2 characters long.")]
        public string CompanyCode { get; set; }

        [Required]
        [StringLength(5,
            MinimumLength = 3,
            ErrorMessage = "Branch Code be at least 3 characters long.")]
        public string BranchCode { get; set; }

        [Required (ErrorMessage ="Account Code is required.")]
        public int AccCode { get; set; }

        public string AccName { get; set; }
        public Nullable<int> SubGroupID { get; set; }
        public string SubGroupName { get; set; }
        public Nullable<int> GroupID { get; set; }
        public string GroupName { get; set; }

        [Required]
        public string Description { get; set; }
        public string ObjectStatus { get; set; }
    }
}
