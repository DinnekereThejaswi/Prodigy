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
    
    public partial class KTTU_RECEIPTS_MASTER
    {
        public string obj_id { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public int receipt_no { get; set; }
        public System.DateTime receipt_date { get; set; }
        public string gs_type { get; set; }
        public string sal_code { get; set; }
        public string operator_code { get; set; }
        public string receipt_type { get; set; }
        public string Ref_no { get; set; }
        public string cflag { get; set; }
        public string cancelled_by { get; set; }
        public Nullable<decimal> grand_total { get; set; }
        public Nullable<int> charges_acc_code { get; set; }
        public string type { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string remarks { get; set; }
        public Nullable<int> issue_no { get; set; }
        public string party_name { get; set; }
        public Nullable<int> no_of_tags { get; set; }
        public Nullable<decimal> tag_weight { get; set; }
        public string batch_id { get; set; }
        public string lot_ref_id { get; set; }
        public string DC_No { get; set; }
        public Nullable<System.DateTime> DC_Date { get; set; }
        public Nullable<System.DateTime> Inv_Date { get; set; }
        public string Is_hallmarked { get; set; }
        public string hallmarked_by { get; set; }
        public string inv_no { get; set; }
        public string cancelled_remarks { get; set; }
        public Nullable<int> ShiftID { get; set; }
        public string New_Bill_No { get; set; }
        public string Ref_Receipt_No { get; set; }
        public string isFixed { get; set; }
        public string new_receipt_no { get; set; }
        public string adv_type { get; set; }
        public Nullable<int> Invoice_Type_Id { get; set; }
        public string C_Form { get; set; }
        public Nullable<decimal> TDSPerc { get; set; }
        public Nullable<decimal> Net_Amount { get; set; }
        public Nullable<decimal> TDS_Amount { get; set; }
        public Nullable<int> version { get; set; }
        public string stk_type { get; set; }
        public Nullable<int> old_receipt_no { get; set; }
        public Nullable<int> import_data_id { get; set; }
        public string import_content { get; set; }
        public Nullable<decimal> final_amount { get; set; }
        public string GSTGroupCode { get; set; }
        public Nullable<decimal> SGST_Percent { get; set; }
        public Nullable<decimal> SGST_Amount { get; set; }
        public Nullable<decimal> CGST_Percent { get; set; }
        public Nullable<decimal> CGST_Amount { get; set; }
        public Nullable<decimal> IGST_Percent { get; set; }
        public Nullable<decimal> IGST_Amount { get; set; }
        public string HSN { get; set; }
        public Nullable<decimal> round_off { get; set; }
        public string pan_no { get; set; }
        public string tin_no { get; set; }
        public System.Guid UniqRowID { get; set; }
        public string isReceived { get; set; }
        public Nullable<decimal> expected_pure_wt { get; set; }
        public string isConfirm { get; set; }
        public Nullable<int> fin_year { get; set; }
        public string isDealer { get; set; }
        public string isNwtBased { get; set; }
        public string isCaratBased { get; set; }
        public Nullable<decimal> TCS_Percent { get; set; }
        public Nullable<decimal> TCS_Amount { get; set; }
        public Nullable<int> store_location_id { get; set; }
        public string rate_type { get; set; }
        public Nullable<decimal> fixedRate { get; set; }
        public Nullable<decimal> fixedRatewithgst { get; set; }
        public Nullable<System.DateTime> due_date { get; set; }
        public Nullable<int> due_days { get; set; }
    }
}
