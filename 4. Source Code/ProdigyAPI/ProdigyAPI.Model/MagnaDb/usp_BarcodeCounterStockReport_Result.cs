//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProdigyAPI.Model.MagnaDb
{
    using System;
    
    public partial class usp_BarcodeCounterStockReport_Result
    {
        public string CounterCode { get; set; }
        public string Item { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<decimal> GrossWt { get; set; }
        public Nullable<decimal> StoneWt { get; set; }
        public Nullable<decimal> NewWt { get; set; }
        public Nullable<decimal> Dcts { get; set; }
    }
}
