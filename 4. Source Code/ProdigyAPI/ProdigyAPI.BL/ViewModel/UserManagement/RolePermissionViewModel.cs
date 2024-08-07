using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.UserManagement
{
    public class RolePermissionViewModel
    {
        public int ID { get; set; }
        public int RoleID { get; set; }
        public int MenuID { get; set; }
        public string MenuName { get; set; }
        public bool IsHeaderMenu { get; set; }
        public bool CreatePermission { get; set; }
        public bool EditPermission { get; set; }
        public bool ViewPermission { get; set; }
        public bool DeletePermission { get; set; }
        public bool PrintPermission { get; set; }
        public bool ExportPermission { get; set; }
        public bool ClosePermission { get; set; }
        public bool ReopenPermission { get; set; }
        public bool ImportPermission { get; set; }
        public bool ReservedPermission1 { get; set; }
        public bool ReservedPermission2 { get; set; }
        public int InsertedBy { get; set; }
        public DateTime InsertedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
    }

    public class RoleAssignmentViewModel
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public List<ModuleViewModel> Modules { get; set; }
    }
    public class ModuleViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public bool Checked { get; set; }
        public MuduleActionViewModel Functions { get; set; }
        public List<ModuleViewModel> SubModules { get; set; }

    }

    public class MuduleActionViewModel
    {
        public bool AddEnabled { get; set; }//1
        public bool EditEnabled { get; set; }//2
        public bool DeleteEnabled { get; set; }//3
        public bool ViewEnabled { get; set; }//6
        public bool PrintEnabled { get; set; }//7
        public bool ActivateEnabled { get; set; }//4
        public bool DeActivateEnabled { get; set; }//5

    }
}
