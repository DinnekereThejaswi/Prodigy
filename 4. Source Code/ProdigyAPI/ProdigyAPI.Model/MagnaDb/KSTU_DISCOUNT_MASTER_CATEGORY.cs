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
    
    public partial class KSTU_DISCOUNT_MASTER_CATEGORY
    {
        public string obj_id { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public int offer_id { get; set; }
        public System.DateTime start_date { get; set; }
        public System.DateTime end_date { get; set; }
        public decimal basic_disc_rate_per_gram { get; set; }
        public decimal basic_disc_rate_per_carat { get; set; }
        public string is_active { get; set; }
        public string operator_code { get; set; }
        public System.DateTime entry_date { get; set; }
        public Nullable<decimal> basic_disc_va_perentage { get; set; }
    }
}
