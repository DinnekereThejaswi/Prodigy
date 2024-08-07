using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class DesignMasterVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string DesignCode { get; set; }
        public string DesignName { get; set; }
        public string MasterDesignCode { get; set; }
        public System.Guid UniqRowID { get; set; }
        public string ObjectStatus { get; set; }
    }
}
