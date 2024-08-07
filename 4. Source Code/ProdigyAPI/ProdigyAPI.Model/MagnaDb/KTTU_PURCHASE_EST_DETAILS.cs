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
    
    public partial class KTTU_PURCHASE_EST_DETAILS
    {
        public string obj_id { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public int bill_no { get; set; }
        public int est_no { get; set; }
        public int sl_no { get; set; }
        public string item_name { get; set; }
        public int item_no { get; set; }
        public decimal gwt { get; set; }
        public decimal swt { get; set; }
        public decimal nwt { get; set; }
        public decimal melting_percent { get; set; }
        public Nullable<decimal> melting_loss { get; set; }
        public decimal purchase_rate { get; set; }
        public decimal diamond_amount { get; set; }
        public decimal gold_amount { get; set; }
        public decimal item_amount { get; set; }
        public string sal_code { get; set; }
        public string gs_code { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public Nullable<decimal> purity_per { get; set; }
        public Nullable<decimal> convertion_wt { get; set; }
        public Nullable<int> Fin_Year { get; set; }
        public string item_description { get; set; }
        public Nullable<decimal> itemwise_tax_percentage { get; set; }
        public Nullable<decimal> itemwise_tax_amount { get; set; }
        public Nullable<decimal> itemwise_purchase_amount { get; set; }
        public Nullable<int> invoice_type { get; set; }
        public Nullable<decimal> ratededuct { get; set; }
        public string GSTGroupCode { get; set; }
        public Nullable<decimal> SGST_Percent { get; set; }
        public Nullable<decimal> SGST_Amount { get; set; }
        public Nullable<decimal> CGST_Percent { get; set; }
        public Nullable<decimal> CGST_Amount { get; set; }
        public Nullable<decimal> IGST_Percent { get; set; }
        public Nullable<decimal> IGST_Amount { get; set; }
        public string HSN { get; set; }
        public Nullable<decimal> dcts { get; set; }
        public string item_type { get; set; }
        public string category_type { get; set; }
        public Nullable<decimal> va_amt { get; set; }
    }
}