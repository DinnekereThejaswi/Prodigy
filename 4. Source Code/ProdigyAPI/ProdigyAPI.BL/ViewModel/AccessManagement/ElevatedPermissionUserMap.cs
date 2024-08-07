using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.AccessManagement
{
    public class ElevatedPermissionUserMap
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int PermissionID { get; set; }
        public string PermissionName { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public bool Active { get; set; }
    }
}
