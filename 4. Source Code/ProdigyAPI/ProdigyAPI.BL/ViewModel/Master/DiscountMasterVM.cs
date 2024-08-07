using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class DiscountMasterVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string SRDiscount { get; set; }
        public string StWtDeduction { get; set; }
        public string PurDiscount { get; set; }
        public string OrderDiscount { get; set; }
        public string DisDCounter { get; set; }
        public string DisRCounter { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public string DiscountType { get; set; }
    }
}
