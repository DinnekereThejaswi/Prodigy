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
    
    public partial class KSTS_ITEMSIZE_MASTER
    {
        public string obj_id { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public string item_code { get; set; }
        public string item_name { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string obj_status { get; set; }
        public string category { get; set; }
        public string categoryName { get; set; }
        public System.Guid UniqRowID { get; set; }
    }
}
