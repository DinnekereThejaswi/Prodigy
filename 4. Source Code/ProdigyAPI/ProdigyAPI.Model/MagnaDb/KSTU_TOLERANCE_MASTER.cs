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
    
    public partial class KSTU_TOLERANCE_MASTER
    {
        public int obj_id { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public string Details { get; set; }
        public decimal Min_val { get; set; }
        public decimal Max_Val { get; set; }
        public Nullable<System.Guid> UniqRowID { get; set; }
    }
}
