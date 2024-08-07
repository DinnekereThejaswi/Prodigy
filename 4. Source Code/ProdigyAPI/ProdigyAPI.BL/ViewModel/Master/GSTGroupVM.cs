using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class GSTGroupVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string Code { get; set; }
        public string GSTGroupType { get; set; }
        public string Description { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public bool? IsActive { get; set; }
        public int? SortOrder { get; set; }
    }
}
