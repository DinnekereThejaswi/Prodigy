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
    
    public partial class KSTS_DISCOUNT_PERIOD
    {
        public string obj_id { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public string sr_discount { get; set; }
        public string st_wt_deduction { get; set; }
        public string pur_discount { get; set; }
        public string order_discount { get; set; }
        public string dis_dcounter { get; set; }
        public string dis_rcounter { get; set; }
        public System.DateTime start_date { get; set; }
        public System.DateTime end_date { get; set; }
        public string discount_type { get; set; }
    }
}