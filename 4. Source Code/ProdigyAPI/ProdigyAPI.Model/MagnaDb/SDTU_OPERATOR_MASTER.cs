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
    
    public partial class SDTU_OPERATOR_MASTER
    {
        public string obj_id { get; set; }
        public string OperatorCode { get; set; }
        public string branch_code { get; set; }
        public string company_code { get; set; }
        public int employee_no { get; set; }
        public string OperatorName { get; set; }
        public string Password3 { get; set; }
        public int OperatorRole { get; set; }
        public string object_status { get; set; }
        public Nullable<int> operatorRole2 { get; set; }
        public Nullable<decimal> discount_percent { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string counter_code { get; set; }
        public string OperatorType { get; set; }
        public System.Guid PasswordSalt { get; set; }
        public byte[] RowTimestamp { get; set; }
        public string mobile_no { get; set; }
        public Nullable<System.DateTime> PasswordChangedOn { get; set; }
    }
}