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
    
    public partial class GetAdjustedPurchaeBill_Result
    {
        public int BillNo { get; set; }
        public string Customer { get; set; }
        public System.DateTime Date { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> GrossWt { get; set; }
        public Nullable<int> Quantity { get; set; }
    }
}