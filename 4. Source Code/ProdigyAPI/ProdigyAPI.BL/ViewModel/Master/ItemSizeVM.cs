using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class ItemSizeVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ObjectStatus { get; set; }
        public string Category { get; set; }
        public string CategoryName { get; set; }
        public System.Guid UniqRowID { get; set; }
        public DateTime? UpdateOn { get; set; }

    }
}
