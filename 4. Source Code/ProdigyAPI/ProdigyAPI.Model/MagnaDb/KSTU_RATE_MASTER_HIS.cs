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
    
    public partial class KSTU_RATE_MASTER_HIS
    {
        public string obj_id { get; set; }
        public System.DateTime ondate { get; set; }
        public string bill_type { get; set; }
        public string gs_code { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public decimal rate { get; set; }
        public decimal exchange_rate { get; set; }
        public decimal cash_rate { get; set; }
        public string karat { get; set; }
        public string isDayEnd { get; set; }
        public string operator_code { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public int SNO { get; set; }
        public string UniqRowID { get; set; }
    }
}
