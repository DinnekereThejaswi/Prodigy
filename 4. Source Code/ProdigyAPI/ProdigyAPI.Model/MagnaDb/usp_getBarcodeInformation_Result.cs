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
    
    public partial class usp_getBarcodeInformation_Result
    {
        public string obj_id { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public int est_no { get; set; }
        public int sl_no { get; set; }
        public int bill_no { get; set; }
        public string barcode_no { get; set; }
        public Nullable<int> Sal_Code { get; set; }
        public string counter_code { get; set; }
        public string item_name { get; set; }
        public Nullable<int> qty { get; set; }
        public Nullable<decimal> gwt { get; set; }
        public Nullable<decimal> swt { get; set; }
        public Nullable<decimal> nwt { get; set; }
        public Nullable<decimal> AddWt { get; set; }
        public Nullable<decimal> DedWt { get; set; }
        public int making_charge_per_rs { get; set; }
        public int wast_percent { get; set; }
        public Nullable<decimal> gold_value { get; set; }
        public Nullable<decimal> va_amount { get; set; }
        public Nullable<decimal> stone_charges { get; set; }
        public Nullable<decimal> diamond_charges { get; set; }
        public Nullable<decimal> total_amount { get; set; }
        public int hallmarcharges { get; set; }
        public Nullable<decimal> mc_amount { get; set; }
        public Nullable<decimal> wastage_grms { get; set; }
        public Nullable<decimal> mc_percent { get; set; }
        public int Addqty { get; set; }
        public int DedQty { get; set; }
        public Nullable<decimal> offer_value { get; set; }
        public System.DateTime UpdateOn { get; set; }
        public string gs_code { get; set; }
        public Nullable<decimal> rate { get; set; }
        public string karat { get; set; }
        public string ad_barcode { get; set; }
        public string ad_counter { get; set; }
        public string ad_item { get; set; }
        public string isEDApplicable { get; set; }
        public string mc_type { get; set; }
        public int Fin_Year { get; set; }
        public Nullable<int> New_Bill_No { get; set; }
        public Nullable<decimal> item_total_after_discount { get; set; }
        public int item_additional_discount { get; set; }
        public int tax_percentage { get; set; }
        public int tax_amount { get; set; }
        public Nullable<decimal> item_final_amount { get; set; }
        public string supplier_code { get; set; }
        public string item_size { get; set; }
        public string img_id { get; set; }
        public string design_code { get; set; }
        public string design_name { get; set; }
        public string batch_id { get; set; }
        public string rf_id { get; set; }
        public Nullable<decimal> mc_per_piece { get; set; }
        public int Discount_Mc { get; set; }
        public Nullable<decimal> Total_sales_mc { get; set; }
        public int Mc_Discount_Amt { get; set; }
        public Nullable<decimal> purchase_mc { get; set; }
        public string GSTGroupCode { get; set; }
        public Nullable<decimal> SGST_Percent { get; set; }
        public Nullable<decimal> SGST_Amount { get; set; }
        public Nullable<decimal> CGST_Percent { get; set; }
        public Nullable<decimal> CGST_Amount { get; set; }
        public Nullable<decimal> IGST_Percent { get; set; }
        public Nullable<decimal> IGST_Amount { get; set; }
        public string HSN { get; set; }
        public string Piece_Rate { get; set; }
        public int DeductSWt { get; set; }
        public int Ord_Discount_Amt { get; set; }
        public string ded_counter { get; set; }
        public string ded_item { get; set; }
    }
}