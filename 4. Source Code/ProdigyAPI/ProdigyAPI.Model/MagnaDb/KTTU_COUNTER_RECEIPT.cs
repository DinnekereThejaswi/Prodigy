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
    using System.Collections.Generic;
    
    public partial class KTTU_COUNTER_RECEIPT
    {
        public string obj_id { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public int receipt_no { get; set; }
        public string counter_code { get; set; }
        public string item_name { get; set; }
        public int receipt_units { get; set; }
        public decimal receipt_gwt { get; set; }
        public decimal receipt_nwt { get; set; }
        public string operator_code { get; set; }
        public string gs_code { get; set; }
        public Nullable<System.DateTime> receipt_date { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string remarks { get; set; }
        public Nullable<decimal> receipt_swt { get; set; }
        public System.Guid UniqRowID { get; set; }
    }
}