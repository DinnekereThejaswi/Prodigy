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
    
    public partial class KSTU_ISSUERECEIPTS_TYPES
    {
        public string obj_id { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public string voucher_code { get; set; }
        public string ir_code { get; set; }
        public string ir_name { get; set; }
        public Nullable<int> ac_code { get; set; }
        public string obj_status { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public System.Guid UniqRowID { get; set; }
        public Nullable<int> InputTaxLedgerCode { get; set; }
        public Nullable<int> OutputTaxLedgerCode { get; set; }
        public Nullable<decimal> GSTPercentage { get; set; }
    }
}