using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class VAMasterVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string GSCode { get; set; }
        public int SNO { get; set; }
        public string CategoryCode { get; set; }
        public Nullable<decimal> FromWt { get; set; }
        public Nullable<decimal> ToWt { get; set; }
        public Nullable<decimal> McAmount { get; set; }
        public Nullable<decimal> McPerGram { get; set; }
        public Nullable<decimal> WastageGrms { get; set; }
        public Nullable<decimal> WastPercentage { get; set; }
        public string TypeID { get; set; }
        public decimal? range { get; set; }
        public Nullable<decimal> McPercent { get; set; }
        public string PartyCode { get; set; }
        public string DesignCode { get; set; }
        public string ObjectStatus { get; set; }
        public Nullable<decimal> McPerPiece { get; set; }
        public decimal? ValueAdded { get; set; }
    }

    public class VAMasterPrintVM
    {
        [Required]
        public string CompanyCode { get; set; }

        [Required]
        public string BranchCode { get; set; }

        [Required]
        public string SupplierCode { get; set; }

        [Required]
        public string GSCode { get; set; }

        [Required]
        public string ItemCode { get; set; }

        [Required]
        public string DesignCode { get; set; }
    }

    public class VAMasterCopyVM
    {
        [Required]
        public string CompanyCode { get; set; }

        [Required]
        public string BranchCode { get; set; }

        [Required]
        public string FromSupplierCode { get; set; }

        public string ToSupplierCode { get; set; }

        [Required]
        public string GSCode { get; set; }

        [Required]
        public string ItemCode { get; set; }

        [Required]
        public string DesignCode { get; set; }

    }
}
