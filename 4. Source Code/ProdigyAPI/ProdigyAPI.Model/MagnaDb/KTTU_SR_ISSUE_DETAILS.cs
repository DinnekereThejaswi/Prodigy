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
    
    public partial class KTTU_SR_ISSUE_DETAILS
    {
        public string obj_id { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public int issue_no { get; set; }
        public int sales_bill_no { get; set; }
        public int sl_no { get; set; }
        public string gs_code { get; set; }
        public string item_name { get; set; }
        public int item_no { get; set; }
        public decimal gwt { get; set; }
        public decimal swt { get; set; }
        public decimal nwt { get; set; }
        public decimal stone_charges { get; set; }
        public decimal diamond_charges { get; set; }
        public Nullable<decimal> net_amount { get; set; }
        public Nullable<decimal> va_amount { get; set; }
        public Nullable<int> Fin_Year { get; set; }
        public string barcode_no { get; set; }
        public string supplier_code { get; set; }
        public string batch_id { get; set; }
        public string design_code { get; set; }
        public string counter_code { get; set; }
        public string item_size_code { get; set; }
        public Nullable<decimal> pur_rate { get; set; }
        public Nullable<int> pur_wastage_type { get; set; }
        public Nullable<decimal> pur_wastage_type_value { get; set; }
        public Nullable<int> pur_mc_type { get; set; }
        public Nullable<decimal> pur_mc_gram { get; set; }
        public System.Guid UniqRowID { get; set; }
        public string HSN { get; set; }
    }
}
