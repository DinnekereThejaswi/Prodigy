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
    
    public partial class KTTU_ISSUE_MASTER
    {
        public string obj_id { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public int issue_no { get; set; }
        public System.DateTime issue_date { get; set; }
        public string sal_code { get; set; }
        public string operator_code { get; set; }
        public string gs_type { get; set; }
        public string party_name { get; set; }
        public string issue_type { get; set; }
        public string obj_status { get; set; }
        public string cflag { get; set; }
        public string cancelled_by { get; set; }
        public string type { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string remarks { get; set; }
        public string batch_id { get; set; }
        public Nullable<int> no_of_tags { get; set; }
        public Nullable<decimal> tag_weight { get; set; }
        public Nullable<int> FT_Ref_No { get; set; }
        public Nullable<int> grn_no { get; set; }
        public string cancelled_remarks { get; set; }
        public Nullable<decimal> total_value { get; set; }
        public string Advance_type { get; set; }
        public string new_bill_no { get; set; }
        public Nullable<int> ShiftID { get; set; }
        public string received_from { get; set; }
        public Nullable<int> Version { get; set; }
        public string stk_type { get; set; }
        public Nullable<int> old_issue_no { get; set; }
        public Nullable<int> import_data_id { get; set; }
        public string import_content { get; set; }
        public System.Guid UniqRowID { get; set; }
        public string U_obj_id { get; set; }
        public string isReceived { get; set; }
        public Nullable<decimal> expected_pure_wt { get; set; }
        public string new_no { get; set; }
        public Nullable<int> store_location_id { get; set; }
    }
}