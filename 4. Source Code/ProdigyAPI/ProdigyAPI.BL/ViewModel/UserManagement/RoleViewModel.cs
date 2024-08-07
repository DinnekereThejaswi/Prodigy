using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.UserManagement
{
    public class RoleViewModel
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public bool OTPValidationEnabled { get; set; }
    }
}
