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
    
    public partial class KSTU_PACKING_DETAILS
    {
        public string obj_id { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public string p_code { get; set; }
        public string p_name { get; set; }
        public Nullable<decimal> m_length { get; set; }
        public string m_length_uom { get; set; }
        public Nullable<decimal> m_height { get; set; }
        public string m_height_uom { get; set; }
        public Nullable<decimal> m_width { get; set; }
        public string m_width_uom { get; set; }
        public string color { get; set; }
        public Nullable<decimal> m_weight { get; set; }
        public string m_weight_uom { get; set; }
        public string remarks { get; set; }
        public string obj_status { get; set; }
        public string OTLNo { get; set; }
    }
}