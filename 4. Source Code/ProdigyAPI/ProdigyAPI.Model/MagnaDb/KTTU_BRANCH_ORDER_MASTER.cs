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
    
    public partial class KTTU_BRANCH_ORDER_MASTER
    {
        public string obj_id { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public int order_no { get; set; }
        public int Cust_Id { get; set; }
        public string cust_name { get; set; }
        public string order_type { get; set; }
        public string remarks { get; set; }
        public System.DateTime order_date { get; set; }
        public string operator_code { get; set; }
        public string sal_code { get; set; }
        public System.DateTime delivery_date { get; set; }
        public string order_rate_type { get; set; }
        public string gs_code { get; set; }
        public decimal rate { get; set; }
        public decimal advance_ord_amount { get; set; }
        public decimal grand_total { get; set; }
        public string object_status { get; set; }
        public int bill_no { get; set; }
        public string cflag { get; set; }
        public string cancelled_by { get; set; }
        public string bill_counter { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string closed_branch { get; set; }
        public string closed_flag { get; set; }
        public string closed_by { get; set; }
        public Nullable<System.DateTime> closed_date { get; set; }
        public string karat { get; set; }
        public Nullable<decimal> order_day_rate { get; set; }
    }
}