using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class RolMasterVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string ItemSize { get; set; }
        public string ItemName { get; set; }
        public string GSCode { get; set; }
        public string TypeT { get; set; }
        public int RollQty { get; set; }
        public decimal RollWeight { get; set; }
        public int? MAxQty { get; set; }
        public decimal? MAxWeight { get; set; }
        public decimal Range { get; set; }
        public int SLNO { get; set; }
        public string CategoryCode { get; set; }
        public System.Guid UniqRowID { get; set; }

    }
}
