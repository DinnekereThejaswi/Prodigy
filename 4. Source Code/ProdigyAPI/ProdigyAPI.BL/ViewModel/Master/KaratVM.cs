using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class KaratVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string Karat { get; set; }
        public DateTime? UpdateOn { get; set; }
        public string ObjStatus { get; set; }
        public System.Guid UniqRowID { get; set; }
    }
}
