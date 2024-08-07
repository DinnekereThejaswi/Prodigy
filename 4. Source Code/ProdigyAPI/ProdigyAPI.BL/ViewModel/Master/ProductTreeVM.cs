using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class ProductTreeVM
    {
        public int ItemLevelID { get; set; }
        public string ItemLevelName { get; set; }

        public List<ProductTreeVM> innerLevelItems = new List<ProductTreeVM>();

        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int ItemLevel1ID { get; set; }
        public string ItemLevel1Name { get; set; }
        [Required]
        public string GSCode { get; set; }
        public int ItemLevel2Id { get; set; }
        public string ItemLevel2Name { get; set; }
        public int ItemLevel3Id { get; set; }
        public string ItemLevel3Name { get; set; }
        public int ItemLevel4Id { get; set; }
        public string ItemLevel4Name { get; set; }
        public int ItemLevel5Id { get; set; }
        public string ItemLevel5Name { get; set; }
        public Nullable<int> ItemLevel6Id { get; set; }
        public string ItemLevel6Name { get; set; }
        public string ShortDescription { get; set; }

        public string IsChild { get; set; }
        public Nullable<int> MinUnits { get; set; }
        public Nullable<decimal> MinStockLevel { get; set; }
        public string CatalogId { get; set; }
        public string Grade { get; set; }
        public string Tagged { get; set; }
        public string Stone { get; set; }
        public string Diamond { get; set; }
        public string CounterCode { get; set; }
        public string karat { get; set; }
        public string PieceItem { get; set; }
        public string ObjStatus { get; set; }
        public string AliasName { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public Nullable<decimal> MinProfitPercent { get; set; }
        public string QtyLock { get; set; }
        public string Hallmark { get; set; }
        public string Certification { get; set; }
        public Nullable<decimal> TcsPerc { get; set; }
        public string GSTGoodsGroupCode { get; set; }
        public string GSTServicesGroupCode { get; set; }
        public string HSN { get; set; }
        public System.Guid UniqRowID { get; set; }
        public List<ItemCounterListVM> CounterList { get; set; }
    }

    public partial class ItemCounterListVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string GSCode { get; set; }
        public string ItemName { get; set; }
        public string CounterCode { get; set; }

        public string CounterName { get; set; }
    }
}
