using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ProdigyAPI.Utils
{
    public class PortalUserInfo
    {
        public string Address { get; set; }
        public string MobileNo { get; set; }
        [Required, MinLength(3), MaxLength(100)]
        public string Name { get; set; }
        public string City { get; set; }
        [Required, MaxLength(100), DataType(DataType.EmailAddress)]
        public string EMail { get; set; }
        public bool? IsGCMRequired { get; set; }
        public string Password { get; set; }
        public string ImageUrl { get; set; }
    }

    public class SalesmanDetail
    {
        public int ID { get; set; }
        public int BranchID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string JobTittle { get; set; }
        public string ContactNo { get; set; }
        public string EmailID { get; set; }
        public Nullable<decimal> IncentivePercent { get; set; }
        public string Remarks { get; set; }
        public bool Blocked { get; set; }
        public System.DateTime InsertedOn { get; set; }
        public int InsertedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public string BranchCode { get; set; }
    }
}