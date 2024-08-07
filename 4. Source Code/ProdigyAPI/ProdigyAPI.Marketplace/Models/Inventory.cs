using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.Marketplace.Models
{
    public class Inventory
    {
        public List<Inventorylist> inventorylist { get; set; }
    }
    public class Inventorylist
    {
        public string MarketPlaceCode { get; set; }
        public string BranchCode { get; set; }
        public string BarcodeNo { get; set; }
        public string GsCode { get; set; }
        public string Karat { get; set; }
        public int Qty { get; set; }
        public decimal GrossWt { get; set; }
        public decimal NetWt { get; set; }
        public DateTime PostingDate { get; set; }
        public string DocumentNo { get; set; }
        public string PoSSKU { get; set; }
        public decimal? Rate { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal MaxRetailPrice { get; set; }
        public decimal MinSellingPrice { get; set; }
        public decimal MaxSellingPrice { get; set; }
    }
    public class MarketplaceInventories
    {
        public int SellableQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public List<MarketplaceChannelInventories> marketplaceChannelInventories { get; set; }

    }
    public class MarketplaceChannelInventories
    {
        public int SellableQuantity { get; set; }
        public int BufferedQuantity { get; set; }
        public MarketplaceAttributes marketplaceAttributes { get; set; }
    }
    public class MarketplaceAttributes
    {
        public string MarketplaceName { get; set; }
        public string Channelname { get; set; }
    }
}
