using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class ItemListGroupVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string ItemLevel1ID { get; set; }
        public string ItemLevel1Name { get; set; }
        public string ItemLevel2ID { get; set; }
        public string ItemLevel2Name { get; set; }
        public string ItemLevel3ID { get; set; }
        public string ItemLevel3Name { get; set; }
        public string ItemLevel4ID { get; set; }
        public string ItemLevel4Name { get; set; }
        public string ItemLevel5ID { get; set; }
        public string ItemLevel5Name { get; set; }
        public string ItemLevel6ID { get; set; }
        public string ItemLevel6Name { get; set; }
        public string ShortDescription { get; set; }
        public string IsChild { get; set; }
        public string MinUnits { get; set; }
        public string MinStockLevel { get; set; }
        public string CatalogID { get; set; }
        public string Grade { get; set; }
        public string Tagged { get; set; }
        public string Stone { get; set; }
        public string Diamond { get; set; }
        public string CounterCode { get; set; }
        public string Karat { get; set; }
        public string PieceItem { get; set; }
        public string ObjStatus { get; set; }
        public string AliasName { get; set; }
        public DateTime? UpdateOn { get; set; }
        public string MinProfitPercent { get; set; }
        public string QtyLock { get; set; }
        public string Hallmark { get; set; }
        public string Certification { get; set; }
        public string TCSPercentage { get; set; }
        public string GSTGoodsGroupCode { get; set; }
        public string GSTServicesGroupCode { get; set; }
        public string HSN { get; set; }
        public System.Guid UniqRowID { get; set; }
        public ICollection<ItemSizeVM> itemsizevm { get; set; }


    }
}
