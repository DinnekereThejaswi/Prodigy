using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
     public class ItemMasterVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string GSCode { get; set; }
        public string Item { get; set; }
        public int? MinnimumUnits { get; set; }
        public string Tagged { get; set; }
        public string Karat { get; set; }
        public decimal? MinVApercent { get; set; }
        public string IsPiece { get; set; }
        public string Stone { get; set; }
        public string Diamond { get; set; }
        public string CounterCode { get; set; }
        public string Ischild { get; set; }
        public string Hallmark { get; set; }
        public string Certification { get; set; }
        public string ObjectStatus { get; set; }
        public string ItemCategory { get; set; }
        public string ItemCategoryName { get; set; }
        public string GSTGoodsGroupCode { get; set; }
        public string GSName { get; set; }
        public string GSTServicesGroupCode { get; set; }
        public string HSN { get; set; }
        public ICollection<RolMasterVM> rollmastervm { get; set; }
        public ICollection<ItemListGroupVM> itemlistgroupvm { get; set; }
        public ICollection<BarcodeMasterVM> barcodemastervm { get; set; }
    }
}
