using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Accounts
{
    public class LedgerTypeVM
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class TDSVM
    {
        public string TDSName { get; set; }
        public int TDSID { get; set; }
    }

    public class GSTGroupCodeVM
    {
        public string Code { get; set; }
        public string Type { get; set; }
    }

    public class LedgerMasterVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int AccCode { get; set; }
        public string AccName { get; set; }
        public string AccType { get; set; }
        public int GroupID { get; set; }
        public decimal OpeBal { get; set; }
        public string OpnBalType { get; set; }
        public int CustID { get; set; }
        public string PartyCode { get; set; }
        public string GSCode { get; set; }
        public string ObjStatus { get; set; }
        public Nullable<int> GSSeqNo { get; set; }
        public string SchemeCode { get; set; }
        public string VATID { get; set; }
        public Nullable<decimal> OdLimit { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string JFlag { get; set; }
        public string PANCard { get; set; }
        public string TIN { get; set; }
        public string LedgerType { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string District { get; set; }
        public string Country { get; set; }
        public string PinCode { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string FaxNo { get; set; }
        public string WebSite { get; set; }
        public string CSTNo { get; set; }
        public Nullable<decimal> BudgetAmt { get; set; }
        public Nullable<int> TDSID { get; set; }
        public string EmailID { get; set; }
        public string HSN { get; set; }
        public string GSTGoodsGroupCode { get; set; }
        public string GSTServicesGroupCode { get; set; }
        public Nullable<int> StateCode { get; set; }
        public string VTYPE { get; set; }
        public System.Guid UniqRowID { get; set; }
        public string TransType { get; set; }
        public string IsAutomatic { get; set; }
        public string Schedule_Name { get; set; }
        public string NewAccCode { get; set; }
        public string PartyACNo { get; set; }
        public string PartyACName { get; set; }
        public string PartyMICR_No { get; set; }
        public string PartyBankName { get; set; }
        public string PartyBankBranch { get; set; }
        public string PartyBankAddress { get; set; }
        public string PartyRTGScode { get; set; }
        public string PartyNEFTcode { get; set; }
        public string PartyIFSCcode { get; set; }
        public string swiftcode { get; set; }
    }
}
