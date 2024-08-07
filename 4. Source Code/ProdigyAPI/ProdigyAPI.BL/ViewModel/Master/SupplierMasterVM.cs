using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class SupplierMasterVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string PartyCode { get; set; }
        public string PartyName { get; set; }
        public string VoucherCode { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string District { get; set; }
        public string State { get; set; }
        public string StateStatus { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PinCode { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string FaxNo { get; set; }
        public string PAN { get; set; }
        public string Website { get; set; }
        public string TIN { get; set; }
        public string TDS { get; set; }
        public string VAT { get; set; }
        public string CSTNo { get; set; }
        public string ObjectStatus { get; set; }
        public DateTime? UpdateOn { get; set; }
        public string PartyRTGScode { get; set; }
        public string PartyNEFTcode { get; set; }
        public string PartyIFSCcode { get; set; }
        public string PartyAccountNo { get; set; }
        public string PartyMICRNo { get; set; }
        public string PartyBankName { get; set; }
        public string PartyBankBranch { get; set; }
        public string PartyBankAddress { get; set; }
        public string ContactPerson { get; set; }
        public string ContactEmail { get; set; }
        public string Email { get; set; }
        public string ContactMobileNo { get; set; }
        public string PartyACName { get; set; }
        public decimal? OpnBal { get; set; }
        public string OpnBalType { get; set; }
        public decimal? OpnWeight { get; set; }
        public string OpnWeightType { get; set; }
        public int? CreditPeriod { get; set; }
        public int? LeadTime { get; set; }
        public decimal? MaxPayment { get; set; }
        public decimal? ConvRate { get; set; }
        public string SwiftCode { get; set; }
        public string BranchType { get; set; }
        public decimal? TDSPercent { get; set; }
        public decimal? CreditWeight { get; set; }
        public int TDSId { get; set; }
        public string IsSameEntity { get; set; }
        public int? StateCode { get; set; }
        public string SchemeType { get; set; }
        public string SupplierMetal { get; set; }
        public System.Guid UniqRowID { get; set; }

        public List<SupplierGroupToVM> ListGroupTo { get; set; }
        public List<SupplierOpenDetailsVM> ListOpenDet { get; set; }
    }

    public class SupplierGroupToVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string PartyCode { get; set; }
        public string VoucherCode { get; set; }
        public string IRCode { get; set; }
        public string GroupName { get; set; }
        public DateTime UpdateOn { get; set; }
        public System.Guid UniqRowID { get; set; }
    }
}
