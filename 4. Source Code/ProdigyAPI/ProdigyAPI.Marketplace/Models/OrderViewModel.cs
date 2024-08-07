using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.Marketplace.Models
{
    public class OrderViewModel
    {
        public int ID { get; set; }
        public string MarketPlaceCode { get; set; }
        public string MarketPlaceOrderID { get; set; }
        public string MarketPlaceLocationID { get; set; }
        public string StoreID { get; set; }
        public DateTime CreationTimestamp { get; set; }
        public string CreationTimestampInEpoch { get; set; }
        public string OrderStatus { get; set; }
        public string CustomerOrderID { get; set; }
        public string ShipmentID { get; set; }
        public string MerchantID { get; set; }
        public string MarketplaceName { get; set; }
        public string MarketplaceChannel { get; set; }
        public string Currency { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalTax { get; set; }
        public string ShipToAddress { get; set; }
        public string RecommendedShippingMethod { get; set; }
        public DateTime? ExpectedShippingTimestamp { get; set; }
        public decimal? OrderChargesTotal { get; set; }
        public decimal? OrderChargesTax { get; set; }
        public List<OrderDetailViewModel> orderDetails { get; set; }

    }
    public class OrderDetailViewModel
    {
        public int ID { get; set; }
        public string MarketPlaceLineItemID { get; set; }
        public string OrderItem { get; set; }
        public int Qty { get; set; }
        public bool? GiftWrapRequired { get; set; }
        public bool? GiftMessagePresent { get; set; }
        public decimal LineAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal? ProductPromotionCharges { get; set; }
        public decimal? ProductPromotionTaxCharges { get; set; }
    }

    #region Order Reject view models
    public class RejectOrderInput
    {
        public string referenceId { get; set; }
        public List<RejectedLineItem> rejectedLineItems { get; set; }
    }

    public class RejectedLineItem
    {
        public OrderLineItem lineItem { get; set; }
        public string reason { get; set; } //Enum: OUT_OF_STOCK
        public int quantity { get; set; }
    } 
    #endregion
}
