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
    
    public partial class KSTU_CUSTOMER_ID_PROOF_DETAILS
    {
        public string obj_id { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public int cust_id { get; set; }
        public int Sl_no { get; set; }
        public string Doc_code { get; set; }
        public string Doc_name { get; set; }
        public string Doc_No { get; set; }
        public byte[] Doc_Image { get; set; }
        public Nullable<int> RepoCustId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> RepDocID { get; set; }
        public string Doc_Image_Path { get; set; }
    }
}