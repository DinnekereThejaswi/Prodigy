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
    
    public partial class usp_GetSRItemsToBeBarcoded_Result
    {
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public int sales_bill_no { get; set; }
        public int sl_no { get; set; }
        public string barcode_no { get; set; }
        public string gs_code { get; set; }
        public string counter_code { get; set; }
        public string item_name { get; set; }
        public Nullable<decimal> GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public Nullable<decimal> NetWt { get; set; }
    }
}