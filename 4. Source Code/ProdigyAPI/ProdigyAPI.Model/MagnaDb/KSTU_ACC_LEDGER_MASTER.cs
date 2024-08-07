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
    
    public partial class KSTU_ACC_LEDGER_MASTER
    {
        public string obj_id { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public int acc_code { get; set; }
        public string acc_name { get; set; }
        public string acc_type { get; set; }
        public int group_id { get; set; }
        public decimal opn_bal { get; set; }
        public string opn_bal_type { get; set; }
        public int cust_id { get; set; }
        public string party_code { get; set; }
        public string gs_code { get; set; }
        public string obj_status { get; set; }
        public Nullable<int> gs_seq_no { get; set; }
        public string scheme_code { get; set; }
        public string vat_id { get; set; }
        public Nullable<decimal> od_limit { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string jflag { get; set; }
        public string pancard { get; set; }
        public string TIN { get; set; }
        public string ledger_type { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string district { get; set; }
        public string country { get; set; }
        public string pincode { get; set; }
        public string phone { get; set; }
        public string mobile { get; set; }
        public string FaxNo { get; set; }
        public string website { get; set; }
        public string cst_no { get; set; }
        public Nullable<decimal> budget_amt { get; set; }
        public Nullable<int> tds_id { get; set; }
        public string email_id { get; set; }
        public string HSN { get; set; }
        public string GSTGoodsGroupCode { get; set; }
        public string GSTServicesGroupCode { get; set; }
        public Nullable<int> state_code { get; set; }
        public string V_TYPE { get; set; }
        public System.Guid UniqRowID { get; set; }
        public string transType { get; set; }
        public string IsAutomatic { get; set; }
        public string Schedule_Name { get; set; }
        public string NewAccCode { get; set; }
        public string Party_AC_No { get; set; }
        public string Party_AC_Name { get; set; }
        public string Party_MICR_No { get; set; }
        public string Party_BankName { get; set; }
        public string Party_BankBranch { get; set; }
        public string Party_BankAddress { get; set; }
        public string Party_RTGScode { get; set; }
        public string Party_NEFTcode { get; set; }
        public string Party_IFSCcode { get; set; }
        public string swift_code { get; set; }
        public Nullable<int> cess_id { get; set; }
        public string IsLocked { get; set; }
        public string bankAcc_type { get; set; }
        public Nullable<decimal> wcdl_limit { get; set; }
        public Nullable<int> wcdl_acccode { get; set; }
        public Nullable<decimal> tax_percentage { get; set; }
        public string IsCrDrNote { get; set; }
        public string CentralAccCode { get; set; }
        public string CentralAccName { get; set; }
    }
}
