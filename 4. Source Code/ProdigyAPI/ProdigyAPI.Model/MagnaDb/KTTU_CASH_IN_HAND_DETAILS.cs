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
    
    public partial class KTTU_CASH_IN_HAND_DETAILS
    {
        public string obj_id { get; set; }
        public string branch_code { get; set; }
        public string company_code { get; set; }
        public int sl_no { get; set; }
        public Nullable<decimal> Cash_Balance { get; set; }
        public Nullable<decimal> Cash_In_Hand { get; set; }
        public System.DateTime bill_date { get; set; }
        public Nullable<int> Fin_Year { get; set; }
    }
}
