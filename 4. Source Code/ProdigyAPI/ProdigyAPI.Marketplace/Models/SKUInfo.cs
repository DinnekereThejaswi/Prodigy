using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.Marketplace.Models
{
    public class SKUInfo
    {
        public int ID { get; set; }
        public string MarketPlaceCode { get; set; }
        public string SKU { get; set; }
        public string GsCode { get; set; }
        public string Karat { get; set; }
        public string ListingBranchCode { get; set; }
        public string PoSSKU { get; set; }
        public bool ExportedToExcel { get; set; }
        public string MarketPlaceAsin { get; set; }
        public DateTime AsinUpdatedOn { get; set; }
        public DateTime InsertedOn { get; set; }
        public bool IsActive { get; set; }
        public List<StoreList> StoreList { get; set; }
    }
    public class StoreList
    {
        public string StoreID { get; set; }
        public int Qty { get; set; }
    }

    public class SKUMarked
    {
        public bool Marked { get; set; }
        public bool ExportedToExcel { get; set; }
        public string MarketPlaceASIN { get; set; }
        public bool Active { get; set; }
    }
}
