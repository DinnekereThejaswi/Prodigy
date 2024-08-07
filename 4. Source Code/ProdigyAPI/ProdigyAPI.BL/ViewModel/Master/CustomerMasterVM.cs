using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class CustomerMasterVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int ID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [DataType(DataType.Text), MaxLength(100), MinLength(3)]
        public string CustName { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [DataType(DataType.Text), MaxLength(100), MinLength(3)]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Pincode { get; set; }
        public string MobileNo { get; set; }
        public string PhoneNo { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? WeddingDate { get; set; }
        public string CustomerType { get; set; }
        public string ObjectStatus { get; set; }
        public string SpouseName { get; set; }
        public string ChildName1 { get; set; }
        public string ChildName2 { get; set; }
        public string ChildName3 { get; set; }
        public DateTime? SpouseDateOfBirth { get; set; }
        public DateTime? Child1DateOfBirth { get; set; }
        public DateTime? Child2DateOfBirth { get; set; }
        public DateTime? Child3DateOfBirth { get; set; }
        public string PANNo { get; set; }
        public string IDType { get; set; }
        public string IDDetails { get; set; }
        public DateTime? UpdateOn { get; set; }
        public string EmailID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Salutation { get; set; }
        public string CountryCode { get; set; }
        public string LoyaltyID { get; set; }
        public string ICNo { get; set; }
        public string PassportNo { get; set; }
        public string PRNo { get; set; }
        public string PrevilegeID { get; set; }
        public int? Age { get; set; }
        public string CountryName { get; set; }
        public string CustCode { get; set; }
        public decimal? CustCreditLimit { get; set; }
        public string TIN { get; set; }
        public int? StateCode { get; set; }
        //public Guid UniqRowID { get; set; }
        public string CorporateID { get; set; }
        public string CorporateBranchID { get; set; }
        public string EmployeeID { get; set; }
        public string RegisteredMN { get; set; }
        public string ProfessionID { get; set; }
        public string EmpCorpEmailID { get; set; }
        public string ImageIDPath { get; set; }
        public string CorpImageIDPath { get; set; }
        public string ImageIDPath2 { get; set; }
        public string AccHolderName { get; set; }
        public string Accsalutation { get; set; }
        public int? RepoCustId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<CustomerIDProofDetailsVM> lstOfProofs { get; set; }
    }

    public class CustomerIDProofDetailsVM
    {
        public string objID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int CustID { get; set; }
        public int SlNo { get; set; }
        public string DocCode { get; set; }
        public string DocName { get; set; }
        public string DocNo { get; set; }
        public byte[] DocImage { get; set; }
        public Nullable<int> RepoCustId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> RepDocID { get; set; }
        public string DocImagePath { get; set; }
    }
}
