using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class GSTMasterVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string GSTCode { get; set; }
        public string Description { get; set; }
        public decimal GSTPercentage { get; set; }
        public string ObjectStatus { get; set; }
        public DateTime? UpdateOn { get; set; }
        public int? GSTId { get; set; }

    }
}
