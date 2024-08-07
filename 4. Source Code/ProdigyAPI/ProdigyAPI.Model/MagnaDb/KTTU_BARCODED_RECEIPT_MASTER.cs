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
    
    public partial class KTTU_BARCODED_RECEIPT_MASTER
    {
        public string obj_id { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public int receipt_no { get; set; }
        public int issue_no { get; set; }
        public string receipt_type { get; set; }
        public System.DateTime receipt_date { get; set; }
        public string received_from_branch { get; set; }
        public string received_from { get; set; }
        public string gs_type { get; set; }
        public string sal_code { get; set; }
        public string operator_code { get; set; }
        public string obj_status { get; set; }
        public string cflag { get; set; }
        public string cancelled_by { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public Nullable<decimal> total_amount { get; set; }
        public Nullable<decimal> tolerance_percent { get; set; }
        public Nullable<decimal> final_amount { get; set; }
        public Nullable<int> ShiftID { get; set; }
        public string remarks { get; set; }
        public string Transfer_Type { get; set; }
        public Nullable<decimal> SGST_Percent { get; set; }
        public Nullable<decimal> IGST_Percent { get; set; }
        public Nullable<decimal> CGST_Percent { get; set; }
        public Nullable<decimal> IGST_Amount { get; set; }
        public Nullable<decimal> CGST_Amount { get; set; }
        public Nullable<decimal> SGST_Amount { get; set; }
        public string GSTGroupCode { get; set; }
        public string HSN { get; set; }
        public Nullable<decimal> total_mc { get; set; }
        public Nullable<decimal> TDSPerc { get; set; }
        public Nullable<decimal> TDS_Amount { get; set; }
        public Nullable<decimal> Net_Amount { get; set; }
        public Nullable<decimal> stone_charges { get; set; }
        public Nullable<decimal> diamond_charges { get; set; }
        public string material_type { get; set; }
        public Nullable<decimal> other_charges { get; set; }
        public System.Guid UniqRowID { get; set; }
        public Nullable<decimal> round_off { get; set; }
        public string isReservedReceipt { get; set; }
        public Nullable<int> store_location_id { get; set; }
    }
}