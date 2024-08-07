using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class CounterMasterVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string CounterCode { get; set; }
        public string CounterName { get; set; }
        public string ObjectStatus { get; set; }
        public DateTime? UpdateOn { get; set; }
        public string MaincounterCode { get; set; }
        public System.Guid UniqRowID { get; set; }
    }
}
