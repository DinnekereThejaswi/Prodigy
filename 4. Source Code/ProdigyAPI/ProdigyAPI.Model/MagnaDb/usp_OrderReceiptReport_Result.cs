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
    
    public partial class usp_OrderReceiptReport_Result
    {
        public string branch_code { get; set; }
        public int order_no { get; set; }
        public int cust_id { get; set; }
        public string cust_name { get; set; }
        public string sal_code { get; set; }
        public string gs_code { get; set; }
        public int series_no { get; set; }
        public int receipt_no { get; set; }
        public string pay_mode { get; set; }
        public Nullable<decimal> pay_amt { get; set; }
        public string pay_details { get; set; }
        public string pay_date { get; set; }
        public string Ord_date { get; set; }
        public string Ref_BillNo { get; set; }
        public Nullable<decimal> CardCharges { get; set; }
        public string TotalOrderAmt { get; set; }
        public string orderAdvance { get; set; }
        public string SGST_Percent { get; set; }
        public string SGSTAmountSubTotal { get; set; }
        public string CGST_Percent { get; set; }
        public string CGSTAmountSubTotal { get; set; }
        public string IGST_Percent { get; set; }
        public string IGSTAmountSubTotal { get; set; }
    }
}