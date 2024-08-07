using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class SellingMCMasterVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string GSCode { get; set; }
        public string ItemName { get; set; }
        public string DesignName { get; set; }
        public Nullable<decimal> FromWt { get; set; }
        public Nullable<decimal> ToWt { get; set; }
        public Nullable<decimal> McPerPiece { get; set; }
        public Nullable<decimal> McPerGram { get; set; }
        public Nullable<decimal> McPercent { get; set; }
        public Nullable<decimal> McAmount { get; set; }
        public Nullable<decimal> WastageGrms { get; set; }
        public Nullable<decimal> WastPercentage { get; set; }
        public string TypeID { get; set; }
        public decimal? range { get; set; }
        public string PartyCode { get; set; }
        public string DesignCode { get; set; }
        public string ObjectStatus { get; set; }
        public decimal ValueAdded { get; set; }
    }
    public class SupplerVAmasterVM
    {
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
    }
    public class GSVAmasterVM
    {
        public string GSCode { get; set; }
        public string GSName { get; set; }
    }
    public class ItemVAmasterVM
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
    }
    public class DesignVAmasterVM
    {
        public string DesignCode { get; set; }
        public string DesignName { get; set; }
    }
    public class MCTypesVM
    {
        public int MCTypeID { get; set; }
        public string MCTypeName { get; set; }
    }
    public class FromweightVM
    {
        public Nullable<decimal> FromWt { get; set; }
    }
}
