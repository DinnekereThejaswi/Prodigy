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
    
    public partial class RoleMethodPermission
    {
        public int ID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int RoleID { get; set; }
        public int MethodID { get; set; }
        public bool IsEnabled { get; set; }
        public System.DateTime InsertedOn { get; set; }
        public string InsertedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual Method Method { get; set; }
    }
}
