using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class StoneMasterVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string Type { get; set; }
        public string StoneType { get; set; }
        public string StoneName { get; set; }
        public string CounterCode { get; set; }
        public string BrandName { get; set; }
        public string Color { get; set; }
        public string Cut { get; set; }
        public string Clarity { get; set; }
        public string Status { get; set; }
        public string Code { get; set; }
        [Key]
        public string Batch { get; set; }
        public string Uom { get; set; }
        public string HSN { get; set; }
        public Nullable<decimal> StoneValue { get; set; }
        public string GSTGroupCode { get; set; }
    }
}
