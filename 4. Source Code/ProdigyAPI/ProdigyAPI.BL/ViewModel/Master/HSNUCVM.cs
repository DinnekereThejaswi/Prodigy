using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public partial class HSNUCVM
    {
        public string ComapnyCode { get; set; }

        public string BranchCode { get; set; }
        public string Code { get; set; }
        public string GSTGroupCode { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public bool? IsActive { get; set; }

    }
}
