using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.AccessManagement
{
    public class ElevatedPermissionRoleMap
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int PermissionID { get; set; }
        public string PermissionName { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public bool Active { get; set; }
    }
}
