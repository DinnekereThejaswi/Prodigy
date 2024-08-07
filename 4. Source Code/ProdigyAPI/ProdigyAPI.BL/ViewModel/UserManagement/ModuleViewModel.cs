using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.UserManagement
{
    public class MainMenuViewModel
    {
        public int ID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public string Status { get; set; }
        public string UIRoute { get; set; }
        public string Icon { get; set; }
        public string Class { get; set; }
        public string Label { get; set; }
        public string LabelClass { get; set; }
    }

    public class SubMenuViewModel
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public int ModuleID { get; set; }
        public int SortOrder { get; set; }
        public string Status { get; set; }
        public bool AutoApprove { get; set; }
        public string Flag { get; set; }
        public string UIRoute { get; set; }
        public string Icon { get; set; }
        public string Class { get; set; }
        public string Label { get; set; }
        public string LabelClass { get; set; }
        public string FormType { get; set; }
        public string ReportServerType { get; set; }
        public string ReportApiRoute { get; set; }
     
    }
}
