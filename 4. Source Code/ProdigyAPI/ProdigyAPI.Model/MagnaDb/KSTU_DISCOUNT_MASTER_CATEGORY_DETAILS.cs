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
    
    public partial class KSTU_DISCOUNT_MASTER_CATEGORY_DETAILS
    {
        public string obj_id { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public int offer_id { get; set; }
        public string catagory_code { get; set; }
        public string catagory_name { get; set; }
        public decimal add_disc_rate_per_gram { get; set; }
        public decimal add_disc_rate_per_carat { get; set; }
        public Nullable<decimal> add_disc_va_perentage { get; set; }
    }
}