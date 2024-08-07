using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Repair
{
    public class RepairReceiptDetailsVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int RepaireNo { get; set; }
        public int SlNo { get; set; }
        public string Item { get; set; }
        public int Units { get; set; }
        public decimal GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public decimal NetWt { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string GSCode { get; set; }
        public Nullable<int> FinYear { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public string PartyCode { get; set; }
        public Nullable<decimal> Dcts { get; set; }
        public Nullable<decimal> FinishedGwt { get; set; }
        public Nullable<decimal> FinishedSwt { get; set; }
        public Nullable<decimal> FinishedNwt { get; set; }
        public Nullable<decimal> Wastage { get; set; }
        public string RpStatus { get; set; }
    }
}
