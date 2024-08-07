using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Accounts
{
    public class AccountPostingSetupVM
    {
        [Required]
        [MinLength(2)]
        public string CompanyCode { get; set; }

        [Required]
        [MinLength(2)]
        public string BranchCode { get; set; }

        [Required]
        [MinLength(2)]
        public string GSCode { get; set; }

        [Required]
        public string TransType { get; set; }
        [Required]
        public int AccountCode { get; set; }
        [Required]
        public int OldAccountCode { get; set; }
    }
}
