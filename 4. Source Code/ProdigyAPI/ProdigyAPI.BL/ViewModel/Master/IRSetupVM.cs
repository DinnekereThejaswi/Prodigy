using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class IRSetupVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string VoucherCode { get; set; }
        public string IRCode { get; set; }
        public string IRName { get; set; }
        public Nullable<int> ACCode { get; set; }
        public string AccName { get; set; }
        public string ObjStatus { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public System.Guid UniqRowID { get; set; }
    }
}
