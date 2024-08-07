using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class PackingMaterialVM
    {
        public string ObjID { get; set; }
        [Required]
        public string CompanyCode { get; set; }
        [Required]
        public string BranchCode { get; set; }
        [Required]
        public string PCode { get; set; }
        [Required]
        public string PName { get; set; }
        public Nullable<decimal> MLength { get; set; }
        public string MLengthUOM { get; set; }
        public Nullable<decimal> MHeight { get; set; }
        public string MHeightUOM { get; set; }
        public Nullable<decimal> MWidth { get; set; }
        public string MWidthUOM { get; set; }
        public string Color { get; set; }
        public Nullable<decimal> MWeight { get; set; }
        public string MWeightUOM { get; set; }
        public string Remarks { get; set; }
        public string ObjStatus { get; set; }
    }
}
