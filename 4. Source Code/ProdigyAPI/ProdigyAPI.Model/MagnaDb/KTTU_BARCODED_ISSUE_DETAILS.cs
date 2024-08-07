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
    
    public partial class KTTU_BARCODED_ISSUE_DETAILS
    {
        public string obj_id { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public int issue_no { get; set; }
        public int sno { get; set; }
        public string barcode_no { get; set; }
        public string item_name { get; set; }
        public int quantity { get; set; }
        public decimal gwt { get; set; }
        public decimal swt { get; set; }
        public decimal nwt { get; set; }
        public string counter_code { get; set; }
        public string cflag { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string gs_code { get; set; }
        public Nullable<decimal> amount { get; set; }
        public Nullable<decimal> rate { get; set; }
        public Nullable<decimal> dcts { get; set; }
        public string batch_id { get; set; }
        public string supplier_code { get; set; }
        public Nullable<int> Fin_Year { get; set; }
        public Nullable<int> Batch_no { get; set; }
        public string BReceiptNo { get; set; }
        public Nullable<int> pur_mc_type { get; set; }
        public Nullable<decimal> pur_mc_rate { get; set; }
        public Nullable<decimal> pur_rate { get; set; }
        public Nullable<System.DateTime> barcode_date { get; set; }
        public Nullable<int> BSno { get; set; }
        public string design_code { get; set; }
        public Nullable<decimal> purity_perc { get; set; }
        public Nullable<decimal> pure_wt { get; set; }
        public decimal baud_rate { get; set; }
        public decimal pur_wastage_perc { get; set; }
        public decimal pur_wastage_wt { get; set; }
        public Nullable<decimal> pur_making_charges { get; set; }
        public Nullable<decimal> CGST_Amount { get; set; }
        public Nullable<decimal> CGST_Percent { get; set; }
        public Nullable<decimal> IGST_Amount { get; set; }
        public Nullable<decimal> IGST_Percent { get; set; }
        public Nullable<decimal> SGST_Amount { get; set; }
        public Nullable<decimal> SGST_Percent { get; set; }
        public Nullable<decimal> stone_charges { get; set; }
        public Nullable<decimal> diamond_charges { get; set; }
        public Nullable<decimal> hallmark_charges { get; set; }
        public System.Guid UniqRowID { get; set; }
        public Nullable<int> ReservedEstNo { get; set; }
        public string Reserved_Salcode { get; set; }
        public Nullable<decimal> Reserved_VA { get; set; }
        public Nullable<int> bin_no { get; set; }
        public Nullable<decimal> tolerance_amount { get; set; }
        public string HSN { get; set; }
    }
}
