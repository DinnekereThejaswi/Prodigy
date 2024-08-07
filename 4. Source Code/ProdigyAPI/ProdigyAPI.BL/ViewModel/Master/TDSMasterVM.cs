using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class TDSMasterVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string GSCode { get; set; }
        public string TDSName { get; set; }
        public int TDSID { get; set; }
        public decimal TDS { get; set; }
        public DateTime? UpdateOn { get; set; }
        public string ObjectStatus { get; set; }
    }
}
