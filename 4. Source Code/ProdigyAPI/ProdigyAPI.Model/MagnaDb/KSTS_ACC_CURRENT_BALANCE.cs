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
    
    public partial class KSTS_ACC_CURRENT_BALANCE
    {
        public string obj_id { get; set; }
        public string branch_code { get; set; }
        public string company_code { get; set; }
        public int acc_code { get; set; }
        public decimal curr_cr_bal { get; set; }
        public decimal curr_dr_bal { get; set; }
        public decimal curr_net_bal { get; set; }
        public string curr_bal_type { get; set; }
        public System.DateTime updated_time_stamp { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
    }
}
