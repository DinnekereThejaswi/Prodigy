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
    
    public partial class usp_createOrderPostingVouchers_Result
    {
        public string gs_code { get; set; }
        public string transType { get; set; }
        public string pay_mode { get; set; }
        public Nullable<decimal> pay_amt { get; set; }
        public string bank { get; set; }
        public string cheque_no { get; set; }
        public Nullable<System.DateTime> cheque_date { get; set; }
        public Nullable<int> accCode { get; set; }
    }
}
