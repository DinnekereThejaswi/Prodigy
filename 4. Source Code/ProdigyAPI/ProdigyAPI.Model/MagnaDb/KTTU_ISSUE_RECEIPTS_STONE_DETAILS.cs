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
    
    public partial class KTTU_ISSUE_RECEIPTS_STONE_DETAILS
    {
        public string obj_id { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public int issue_no { get; set; }
        public int sno { get; set; }
        public int receipt_no { get; set; }
        public int item_sno { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public int qty { get; set; }
        public decimal carrat { get; set; }
        public decimal rate { get; set; }
        public decimal amount { get; set; }
        public string gs_code { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public Nullable<int> Fin_Year { get; set; }
        public string IR_Type { get; set; }
        public string party_code { get; set; }
        public Nullable<decimal> swt { get; set; }
        public Nullable<decimal> stone_damage_carat { get; set; }
        public Nullable<decimal> stone_damage_Gms { get; set; }
        public Nullable<int> sub_Lot_No { get; set; }
        public System.Guid UniqRowID { get; set; }
        public string stonetype { get; set; }
        public string color { get; set; }
        public string cut { get; set; }
        public string symmetry { get; set; }
        public string fluorescence { get; set; }
        public string CertificateNo { get; set; }
        public string seiveSize { get; set; }
        public string clarity { get; set; }
        public string polish { get; set; }
        public string shape { get; set; }
    }
}