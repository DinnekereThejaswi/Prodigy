using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class MinStcokVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string GSCode { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string DesignCode { get; set; }
        public string DesignName { get; set; }
        public string CounterCode { get; set; }
        public string CounterName { get; set; }
        public int MinQty { get; set; }
        public decimal MinWt { get; set; }
        public Nullable<int> MaxQty { get; set; }
        public Nullable<decimal> MaxWt { get; set; }
        public string IsSved { get; set; }
        public Nullable<System.DateTime> UpdatdOn { get; set; }
        public Nullable<System.Guid> UniqRowID { get; set; }
    }
}
