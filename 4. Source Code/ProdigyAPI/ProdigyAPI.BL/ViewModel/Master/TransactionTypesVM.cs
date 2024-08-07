using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
   public class TransactionTypesVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string TransactionCode { get; set; }
        public string TransactionName { get; set; }
        public string ObjectStatus { get; set; }
        public DateTime? UpdateOn { get; set; }
        public int OrderBy { get; set; }
        public string AccPostingenable { get; set; }
        public System.Guid UniqRowID { get; set; }
    }
}
