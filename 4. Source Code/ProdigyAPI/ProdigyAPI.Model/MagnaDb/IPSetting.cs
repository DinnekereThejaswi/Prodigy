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
    
    public partial class IPSetting
    {
        public int ID { get; set; }
        public string AllowDenyFlag { get; set; }
        public string FromIP { get; set; }
        public string ToIP { get; set; }
        public Nullable<System.DateTime> InsertedOn { get; set; }
        public string InsertedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public string BranchCode { get; set; }
    }
}