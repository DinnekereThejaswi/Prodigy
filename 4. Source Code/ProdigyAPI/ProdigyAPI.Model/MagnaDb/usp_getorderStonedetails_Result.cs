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
    
    public partial class usp_getorderStonedetails_Result
    {
        public int order_no { get; set; }
        public int item_sl_no { get; set; }
        public int sl_no { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public int qty { get; set; }
        public Nullable<decimal> carrat { get; set; }
        public decimal rate { get; set; }
        public decimal amount { get; set; }
        public Nullable<int> Fin_Year { get; set; }
        public Nullable<decimal> weight { get; set; }
    }
}