using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class StoneRateMasterVM
    {
        [Key]
        public string ObjID { get; set; }
        [Required(ErrorMessage = "CompanyCode is required")]
        [DataType(DataType.Text), MaxLength(5), MinLength(2)]
        public string CompanyCode { get; set; }
        [Required(ErrorMessage = "BranchCode is required")]
        [DataType(DataType.Text), MaxLength(5), MinLength(3)]
        public string BranchCode { get; set; }
        [Required(ErrorMessage = "DmName is required")]
        [DataType(DataType.Text), MaxLength(32), MinLength(3)]
        public string DmName { get; set; }
        [Key]
        public int SlNo { get; set; }
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid price")]
        public decimal KaratFrom { get; set; }
        [RegularExpression(@"\d+(\.\d{1,3})?", ErrorMessage = "Invalid price")]
        public decimal KaratTo { get; set; }
        [RegularExpression(@"\d+(\.\d{1,3})?", ErrorMessage = "Invalid price")]
        public decimal Rate { get; set; }
        [Required(ErrorMessage = "ObjectStatus is required")]
        [DataType(DataType.Text), MaxLength(1), MinLength(1)]
        public string ObjectStatus { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? UpdateOn { get; set; }
        [DataType(DataType.Text), MaxLength(1), MinLength(1)]
        public string Uom { get; set; }
        [DataType(DataType.Text), MaxLength(5)]
        public string Color { get; set; }
        [DataType(DataType.Text), MaxLength(5)]
        public string Cut { get; set; }
        [DataType(DataType.Text), MaxLength(5)]
        public string Clarity { get; set; }
        [DataType(DataType.Text), MaxLength(5)]
        public string Polish { get; set; }
        [DataType(DataType.Text), MaxLength(5)]
        public string Symmetry { get; set; }
        [DataType(DataType.Text), MaxLength(5)]
        public string Fluorescence { get; set; }
        [DataType(DataType.Text), MaxLength(5)]
        public string Certificate { get; set; }
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid price")]
        public decimal? MinAmount { get; set; }
        public System.Guid UniqRowID { get; set; }
    }
}
