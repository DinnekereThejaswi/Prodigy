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
    
    public partial class KTTU_PURCHASE_EST_MASTER_TEST
    {
        public string obj_id { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public int est_no { get; set; }
        public int bill_no { get; set; }
        public string pur_item { get; set; }
        public int cust_id { get; set; }
        public string cust_name { get; set; }
        public System.DateTime p_date { get; set; }
        public decimal tax { get; set; }
        public Nullable<decimal> today_rate { get; set; }
        public string operator_code { get; set; }
        public decimal grand_total { get; set; }
        public string p_type { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public Nullable<decimal> payable_amount { get; set; }
    }
}