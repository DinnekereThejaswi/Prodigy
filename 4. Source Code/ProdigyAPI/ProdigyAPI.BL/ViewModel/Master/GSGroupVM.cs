using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
   public class GSGroupVM
    {
        public string ObjID { get; set; }
        public int SNO { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string IRCode { get; set; }
        public string GSCode { get; set; }
        public System.Guid UniqRowID { get; set; }
        public string ObjectStatus { get; set; }
    }
}
