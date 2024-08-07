using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class MenuViewModel
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [DataType(DataType.Text), MaxLength(50), MinLength(2)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [DataType(DataType.Text), MaxLength(50), MinLength(2)]
        public string DisplayTitle { get; set; }

        public Nullable<int> ParentID { get; set; }
        public int Level { get; set; }
        public int Sequence { get; set; }
        public string Path { get; set; }
        public string IconFont { get; set; }
        public string ToolTip { get; set; }
        public bool? ExtraLink { get; set; }
        public bool IsActive { get; set; }
        public string Icon { get; set; }
        public Nullable<int> CompanyID { get; set; }
        public int InsertedBy { get; set; }
        public DateTime InsertedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<DateTime> UpdatedOn { get; set; }

        public string Class { get; set; }
        public string LabelClass { get; set; }
        public string Label { get; set; }
    }

    public class ApplicationMenu
    {
        public int ID { get; set; }
        public string Tittle { get; set; }
        public int? SortOrder { get; set; }
        public string Path { get; set; }
        public string Icon { get; set; }
        public string Class { get; set; }
        public string Label { get; set; }
        public string LabelClass { get; set; }
        public bool ExtraLink { get; set; }
        public int? ParentID { get; set; }
        public string FormType { get; set; }
        public string ReportServerType { get; set; }
        public string ReportApiRoute { get; set; }
        public virtual List<ApplicationMenu> lstOfMainMenuWithSubMenu { set; get; }
    }
}
