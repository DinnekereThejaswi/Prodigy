using ProdigyAPI.BL.ViewModel.Stock;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class GSItemEntryMasterVM
    {
        public string ObjID { get; set; }

        [Required]
        [StringLength(5, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 2)]
        public string CompanyCode { get; set; }

        [Required]
        [StringLength(5, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 2)]
        public string BranchCode { get; set; }
        public int ItemLevel1ID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 2)]
        public string ItemLevel1Name { get; set; }

        [Required]
        [StringLength(5, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 2)]
        public string GsCode { get; set; }

        [Required]
        [StringLength(1, ErrorMessage = "{0} length must be between {2}.", MinimumLength = 1)]
        public string MeasureType { get; set; }

        [Required]
        [StringLength(2, ErrorMessage = "{0} length must be between {2}.", MinimumLength = 2)]
        public string MetalType { get; set; }
        public string Karat { get; set; }

        [Required]
        [StringLength(1, ErrorMessage = "{0} length must be between {2}.", MinimumLength = 1)]
        public string BillType { get; set; }
        public decimal Tax { get; set; }
        public int OpeningUnits { get; set; }
        public decimal OpeningGwt { get; set; }
        public decimal OpeningNwt { get; set; }
        public decimal OpeningGwtValue { get; set; }
        public decimal OpeningNwtValue { get; set; }
        public string ObjectStatus { get; set; }
        public int? DisplayOrder { get; set; }
        public string CommodityCode { get; set; }
        public DateTime? UpdateOn { get; set; }
        public decimal? ExciseDuty { get; set; }
        public decimal? EduCess { get; set; }
        public decimal? HighEle { get; set; }
        public decimal? Tcs { get; set; }
        public string IsStone { get; set; }
        public decimal? Purity { get; set; }
        public decimal? TcsPerc { get; set; }
        public decimal? STax { get; set; }
        public decimal? CTax { get; set; }
        public decimal? ITax { get; set; }
        public string GSTGoodsGroupCode { get; set; }
        public string GSTServicesGroupCode { get; set; }
        public string HSN { get; set; }
        public string GSName { get; set; }

    }
}
