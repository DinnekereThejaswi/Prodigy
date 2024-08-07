using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Orders
{
    public class OrderRateMasterVM
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public string CompanyCode { get; set; }
        [Required]
        public string BranchCode { get; set; }

        [Required]
        public string BookingCode { get; set; }

        [Required]
        public string BookingName { get; set; }

        public string Description { get; set; }
        [Required]
        public decimal AdvAmountPer { get; set; }
        [Required]
        public int FixedDays { get; set; }
        public string Cflag { get; set; }
        public string OperatorCode { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public decimal MinWeight { get; set; }
    }

    public class FixedOrderVM
    {
        public string ObjID { get; set; }
        [Required]
        public string CompanyCode { get; set; }
        [Required]
        public string BranchCode { get; set; }
        [Required]
        public string BookingCode { get; set; }
        public string BookingName { get; set; }
        [Required]
        public int AccCode { get; set; }
        public string AccName { get; set; }
        [Required]
        public string OperatorCode { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string ObjStatus { get; set; }
        [Required]
        public string OrderRateType { get; set; }
    }

    public class ReceiveOrderVM
    {
        public string CompanyCode { get; set; }

        public string BranchCode { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int MarketplaceID { get; set; }

        public string Type { get; set; }
    }

    public class ReceivedOrderForGridVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int SlNo { get; set; }
        public int OrderNo { get; set; }
        public string OrderRefNo { get; set; }
        public int? OrderSourceMarket { get; set; }
        public string OrderSource { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public string ItemName { get; set; }
        public string BarcodeNo { get; set; }
        public decimal Gwt { get; set; }
        public string Cflag { get; set; }
        public bool? IsShipped { get; set; }
        public bool? IsScheduleForPickUp { get; set; }
        public bool? IsPacked { get; set; }
        public bool? IsPicked { get; set; }
        public bool? IsProcessed { get; set; }
        public int? CentralRefNo { get; set; }
        public int Qty { get; set; }
        public string ItemCode { get; set; }
        public string Status { get; set; }

        public string GSCode { get; set; }
        public string CounterCode { get; set; }
        public string DesignCode { get; set; }
    }

    public class OrderItemPickListVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int AssignmentNo { get; set; }
        public int OrderNo { get; set; }
        public int OrderItemSlno { get; set; }
        public string GSCode { get; set; }
        public string ItemName { get; set; }
        public string CounterCode { get; set; }
        public string BarcodeNo { get; set; }
        public decimal Gwt { get; set; }
        public bool IsPicked { get; set; }
        public System.DateTime PickedDate { get; set; }
        public string PickedBy { get; set; }
        public string SKU { get; set; }
    }

    public class PackingItemVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int OrderNo { get; set; }
        public string OrderReferenceNo { get; set; }
        public string ItemName { get; set; }
        public string BarcodeNo { get; set; }
        public int Qty { get; set; }
        public int MarketplaceSlNo { get; set; }
        public int CentralRefNo { get; set; }
        public string PackageCode { get; set; }
        public string OTLNo { get; set; }
        public decimal Length { get; set; }
        public string LengthUom { get; set; }
        public decimal Width { get; set; }
        public string WidthUom { get; set; }
        public decimal Height { get; set; }
        public string HeightUom { get; set; }
        public decimal Weight { get; set; }
        public string WeightUom { get; set; }
        public string PackageID { get; set; }
        public string OrderSource { get; set; }
    }

    public class MarketplaceOrdersToBeProcessed
    {
        public string OrderSource { get; set; }
        public string OrderReferenceNo { get; set; }
        public int? CentralRefNo { get; set; }
        public int OrderNo { get; set; }
        public int BillNo { get; set; }
        public string SKU { get; set; }
        public decimal? GSTAmt { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PackageWeight { get; set; }
        public string AwbNo { get; set; }
    }

    public class ShipOrderInput
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }

        public string PickupAgentName { get; set; }
        public string PickupAgentMobileNo { get; set; }
        public string PickupRemarks { get; set; }
        public List<MarketplaceOrdersToBeProcessed> OrderList { get; set; }
    }
}
