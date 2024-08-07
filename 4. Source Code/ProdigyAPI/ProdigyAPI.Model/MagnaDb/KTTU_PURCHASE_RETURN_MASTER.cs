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
    
    public partial class KTTU_PURCHASE_RETURN_MASTER
    {
        public string obj_id { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public int pur_return_no { get; set; }
        public string bill_type { get; set; }
        public string inv_ref_no { get; set; }
        public string dc_ref_no { get; set; }
        public System.DateTime pur_return_date { get; set; }
        public System.DateTime inv_ref_date { get; set; }
        public string gs_type { get; set; }
        public string dealer_name { get; set; }
        public decimal grand_total { get; set; }
        public string operator_code { get; set; }
        public string remarks { get; set; }
        public string cancelled_by { get; set; }
        public Nullable<decimal> total_tax_amount { get; set; }
        public string cflag { get; set; }
        public Nullable<int> pur_dealer_no { get; set; }
        public Nullable<decimal> vat_percent { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public Nullable<decimal> cst_Amount { get; set; }
        public string New_Bill_no { get; set; }
        public Nullable<System.DateTime> dc_ref_date { get; set; }
        public int Invoice_Type_Id { get; set; }
        public string C_Form { get; set; }
        public decimal Round_Off { get; set; }
        public Nullable<int> version { get; set; }
        public System.Guid UniqRowID { get; set; }
        public Nullable<decimal> taxable_amount { get; set; }
        public Nullable<decimal> tcs_percentage { get; set; }
        public Nullable<decimal> tcs_amount { get; set; }
        public string NewDCRefNo { get; set; }
        public Nullable<decimal> TDS_Percent { get; set; }
        public Nullable<decimal> TDS_Amount { get; set; }
        public Nullable<decimal> SGST_Percent { get; set; }
        public Nullable<decimal> IGST_Percent { get; set; }
        public Nullable<decimal> CGST_Percent { get; set; }
    }
}