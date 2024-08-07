using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Framework;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData.Query;

namespace ProdigyAPI.Controllers.Masters
{
    /// <summary>
    ///  BranchMaster controller contains all API's related to BranchDetails and SupplierOpenDetails.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Master/BranchMaster")]
    public class BranchMasterController : SIBaseApiController<SupplierMasterVM>, IBaseMasterActionController<SupplierMasterVM, SupplierMasterVM>
    {

        #region Declaration
        MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Controller Methods

        /// <summary>
        /// List Of BranchMaster Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("List/{companyCode}/{branchCode}")]
        public IHttpActionResult List(string companyCode, string branchCode)
        {
            var results = db.KSTU_SUPPLIER_MASTER
                    .Where(s => s.voucher_code == "VB" && s.company_code == companyCode && s.branch_code == branchCode)
                    .Select(s => new SupplierMasterVM
                    {
                        ObjID = s.obj_id,
                        CompanyCode = s.company_code,
                        BranchCode = s.branch_code,
                        Address1 = s.address1,
                        Address2 = s.address2,
                        Address3 = s.address3,
                        BranchType = s.branch_type,
                        City = s.city,
                        ContactEmail = s.Contact_Email,
                        ContactMobileNo = s.Contact_MobileNo,
                        ContactPerson = s.Contact_Person,
                        ConvRate = s.conv_rate,
                        Country = s.country,
                        CreditPeriod = s.credit_period,
                        CreditWeight = s.credit_weight,
                        CSTNo = s.cst_no,
                        District = s.district,
                        Email = s.e_mail,
                        FaxNo = s.FaxNo,
                        IsSameEntity = s.isSameEntity,
                        LeadTime = s.lead_time,
                        MaxPayment = s.max_payment,
                        Mobile = s.mobile,
                        ObjectStatus = s.obj_status,
                        OpnBal = s.opn_bal,
                        OpnBalType = s.opn_bal_type,
                        OpnWeight = s.opn_wt,
                        OpnWeightType = s.opn_wt_type,
                        PAN = s.pan_no,
                        PartyACName = s.Party_AC_Name,
                        PartyAccountNo = s.Party_AC_No,
                        PartyBankAddress = s.Party_BankAddress,
                        PartyBankBranch = s.Party_BankBranch,
                        PartyCode = s.party_code,
                        PartyIFSCcode = s.Party_IFSCcode,
                        PartyMICRNo = s.Party_MICR_No,
                        PartyName = s.party_name,
                        PartyNEFTcode = s.Party_NEFTcode,
                        PartyRTGScode = s.Party_RTGScode,
                        Phone = s.phone,
                        PinCode = s.pincode,
                        SchemeType = s.schemetype,
                        State = s.state,
                        StateCode = s.state_code,
                        StateStatus = s.state_status,
                        SupplierMetal = s.supplierMetal,
                        SwiftCode = s.swift_code,
                        TDS = s.TDS,
                        TDSId = s.tds_id,
                        TDSPercent = s.tds_percent,
                        TIN = s.TIN,
                        UniqRowID = s.UniqRowID,
                        UpdateOn = s.UpdateOn,
                        VAT = s.VAT,
                        VoucherCode = s.voucher_code,
                        Website = s.website,
                    }).OrderBy(c => c.BranchCode);
            return Ok(results);
        }

        /// <summary>
        /// Search Branch Information by Search Parameters.
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Branch/searchByParam/{companyCode}/{branchCode}")]
        [ResponseType(typeof(CustomerMasterVM))]
        public IHttpActionResult searchByParam(SearchParams search)
        {
            List<KSTU_SUPPLIER_MASTER> ksm = new List<KSTU_SUPPLIER_MASTER>();
            switch (search.type.ToUpper()) {
                case "NAME":
                    ksm = db.KSTU_SUPPLIER_MASTER.Where(c => c.party_name.Contains(search.searchParam) && c.company_code == search.companyCode && c.branch_code == search.branchCode).Take(1000).OrderBy(c => c.party_name).ToList();
                    break;
                case "MOBILENO":
                    ksm = db.KSTU_SUPPLIER_MASTER.Where(c => c.mobile.Contains(search.searchParam) && c.company_code == search.companyCode && c.branch_code == search.branchCode).Take(1000).OrderBy(c => c.UpdateOn).ToList();
                    break;
                case "PHONENO":
                    ksm = db.KSTU_SUPPLIER_MASTER.Where(c => c.phone.Contains(search.searchParam) && c.company_code == search.companyCode && c.branch_code == search.branchCode).Take(1000).OrderBy(c => c.UpdateOn).ToList();
                    break;
                default:
                    ksm = null;
                    break;
            }
            if (ksm == null) {
                return null;
            }
            var results = db.KSTU_SUPPLIER_MASTER
                    .Where(s => s.voucher_code == "VB" && s.company_code == search.companyCode && s.branch_code == search.branchCode)
                    .Select(s => new SupplierMasterVM
                    {
                        ObjID = s.obj_id,
                        CompanyCode = s.company_code,
                        BranchCode = s.branch_code,
                        Address1 = s.address1,
                        Address2 = s.address2,
                        Address3 = s.address3,
                        BranchType = s.branch_type,
                        City = s.city,
                        ContactEmail = s.Contact_Email,
                        ContactMobileNo = s.Contact_MobileNo,
                        ContactPerson = s.Contact_Person,
                        ConvRate = s.conv_rate,
                        Country = s.country,
                        CreditPeriod = s.credit_period,
                        CreditWeight = s.credit_weight,
                        CSTNo = s.cst_no,
                        District = s.district,
                        Email = s.e_mail,
                        FaxNo = s.FaxNo,
                        IsSameEntity = s.isSameEntity,
                        LeadTime = s.lead_time,
                        MaxPayment = s.max_payment,
                        Mobile = s.mobile,
                        ObjectStatus = s.obj_status,
                        OpnBal = s.opn_bal,
                        OpnBalType = s.opn_bal_type,
                        OpnWeight = s.opn_wt,
                        OpnWeightType = s.opn_wt_type,
                        PAN = s.pan_no,
                        PartyACName = s.Party_AC_Name,
                        PartyAccountNo = s.Party_AC_No,
                        PartyBankAddress = s.Party_BankAddress,
                        PartyBankBranch = s.Party_BankBranch,
                        PartyCode = s.party_code,
                        PartyIFSCcode = s.Party_IFSCcode,
                        PartyMICRNo = s.Party_MICR_No,
                        PartyName = s.party_name,
                        PartyNEFTcode = s.Party_NEFTcode,
                        PartyRTGScode = s.Party_RTGScode,
                        Phone = s.phone,
                        PinCode = s.pincode,
                        SchemeType = s.schemetype,
                        State = s.state,
                        StateCode = s.state_code,
                        StateStatus = s.state_status,
                        SupplierMetal = s.supplierMetal,
                        SwiftCode = s.swift_code,
                        TDS = s.TDS,
                        TDSId = s.tds_id,
                        TDSPercent = s.tds_percent,
                        TIN = s.TIN,
                        UniqRowID = s.UniqRowID,
                        UpdateOn = s.UpdateOn,
                        VAT = s.VAT,
                        VoucherCode = s.voucher_code,
                        Website = s.website,
                    }).OrderBy(c => c.BranchCode);
            return Ok(results);
        }

        /// <summary>
        /// Save BranchMaster Details
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        [ResponseType(typeof(SupplierMasterVM))]
        public IHttpActionResult postBranchMasterdetails([FromBody]SupplierMasterVM s)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            using (var transaction = db.Database.BeginTransaction()) {
                KSTU_SUPPLIER_MASTER ksm = new KSTU_SUPPLIER_MASTER();
                ksm.obj_id = Common.GetNewGUID();
                ksm.company_code = s.CompanyCode;
                ksm.branch_code = s.BranchCode;
                ksm.address1 = s.Address1;
                ksm.address2 = s.Address2;
                ksm.address3 = s.Address3;
                ksm.branch_code = s.BranchCode;
                ksm.branch_type = s.BranchType;
                ksm.city = s.City;
                ksm.Contact_Email = s.ContactEmail;
                ksm.Contact_MobileNo = s.ContactMobileNo;
                ksm.Contact_Person = s.ContactPerson;
                ksm.conv_rate = s.ConvRate;
                ksm.country = s.Country;
                ksm.credit_period = s.CreditPeriod;
                ksm.credit_weight = s.CreditWeight;
                ksm.cst_no = s.CSTNo;
                ksm.district = s.District;
                ksm.e_mail = s.Email;
                ksm.FaxNo = s.FaxNo;
                ksm.isSameEntity = "Y";
                ksm.lead_time = s.LeadTime;
                ksm.max_payment = s.MaxPayment;
                ksm.mobile = s.Mobile;
                ksm.obj_status = "O";
                ksm.opn_bal = s.OpnBal;
                ksm.opn_bal_type = s.OpnBalType;
                ksm.opn_wt = s.OpnWeight;
                ksm.opn_wt_type = s.OpnWeightType;
                ksm.pan_no = s.PAN;
                ksm.Party_AC_Name = s.PartyACName;
                ksm.Party_AC_No = s.PartyAccountNo;
                ksm.Party_BankAddress = s.PartyBankAddress;
                ksm.Party_BankBranch = s.PartyBankBranch;
                ksm.party_code = s.PartyCode;
                ksm.Party_IFSCcode = s.PartyIFSCcode;
                ksm.Party_MICR_No = s.PartyMICRNo;
                ksm.party_name = s.PartyName;
                ksm.Party_NEFTcode = s.PartyNEFTcode;
                ksm.Party_RTGScode = s.PartyRTGScode;
                ksm.phone = s.Phone;
                ksm.pincode = s.PinCode;
                ksm.schemetype = "N";
                ksm.state = s.State;
                KSTS_STATE_MASTER ktms = db.KSTS_STATE_MASTER.Where(ktm => ktm.state_name == ksm.state).FirstOrDefault();
                ksm.state_code = ktms.tinno;
                ksm.state_status = s.StateStatus;
                ksm.supplierMetal = s.SupplierMetal;
                ksm.swift_code = s.SwiftCode;
                ksm.tds_id = s.TDSId;
                KSTS_TDS_MASTER ktmd = db.KSTS_TDS_MASTER.Where(ktm => ktm.tds_id == ksm.tds_id && ktm.company_code == s.CompanyCode && ktm.branch_code == s.BranchCode).FirstOrDefault();
                ksm.tds_percent = ktmd.tds;
                ksm.TDS = "Y";
                ksm.TIN = s.TIN;
                ksm.UniqRowID = Guid.NewGuid();
                ksm.UpdateOn = Framework.Common.GetDateTime();
                ksm.VAT = s.VAT;
                ksm.voucher_code = "VB";
                ksm.website = s.Website;
                db.KSTU_SUPPLIER_MASTER.Add(ksm);
                try {
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception excp) {
                    transaction.Rollback();
                    throw excp;
                }
            }
            return Ok();
        }

        /// <summary>
        /// Edit BranchMaster Details
        /// </summary>
        /// <param name="ObjID"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put/{ObjID}")]
        [ResponseType(typeof(SupplierMasterVM))]
        public IHttpActionResult Put(string ObjID, SupplierMasterVM s)
        {
            KSTU_SUPPLIER_MASTER ksm = db.KSTU_SUPPLIER_MASTER.Where(d => d.obj_id == ObjID
                                                && d.company_code == s.CompanyCode
                                                && d.branch_code == s.BranchCode).FirstOrDefault();
            if (ksm == null) {
                return NotFound();
            }
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            if (s.ObjID != ksm.obj_id) {
                return BadRequest();
            }
            using (var transaction = db.Database.BeginTransaction()) {
                ksm.obj_id = ksm.obj_id;
                ksm.company_code = Common.CompanyCode;
                ksm.branch_code = ksm.branch_code;
                ksm.address1 = s.Address1;
                ksm.address2 = s.Address2;
                ksm.address3 = s.Address3;
                ksm.branch_type = s.BranchType;
                ksm.city = s.City;
                ksm.Contact_Email = s.ContactEmail;
                ksm.Contact_MobileNo = s.ContactMobileNo;
                ksm.Contact_Person = s.ContactPerson;
                ksm.conv_rate = s.ConvRate;
                ksm.country = s.Country;
                ksm.credit_period = s.CreditPeriod;
                ksm.credit_weight = s.CreditWeight;
                ksm.cst_no = s.CSTNo;
                ksm.district = s.District;
                ksm.e_mail = s.Email;
                ksm.FaxNo = s.FaxNo;
                ksm.isSameEntity = s.IsSameEntity;
                ksm.lead_time = s.LeadTime;
                ksm.max_payment = s.MaxPayment;
                ksm.mobile = s.Mobile;
                ksm.obj_status = s.ObjectStatus;
                ksm.opn_bal = s.OpnBal;
                ksm.opn_bal_type = s.OpnBalType;
                ksm.opn_wt = s.OpnWeight;
                ksm.opn_wt_type = s.OpnWeightType;
                ksm.pan_no = s.PAN;
                ksm.Party_AC_Name = s.PartyACName;
                ksm.Party_AC_No = s.PartyAccountNo;
                ksm.Party_BankAddress = s.PartyBankAddress;
                ksm.Party_BankBranch = s.PartyBankBranch;
                ksm.party_code = s.PartyCode;
                ksm.Party_IFSCcode = s.PartyIFSCcode;
                ksm.Party_MICR_No = s.PartyMICRNo;
                ksm.party_name = s.PartyName;
                ksm.Party_NEFTcode = s.PartyNEFTcode;
                ksm.Party_RTGScode = s.PartyRTGScode;
                ksm.phone = s.Phone;
                ksm.pincode = s.PinCode;
                ksm.schemetype = s.SchemeType;
                ksm.supplierMetal = s.SupplierMetal;
                ksm.swift_code = s.SwiftCode;
                ksm.state = s.State;
                KSTS_STATE_MASTER ktms = db.KSTS_STATE_MASTER.Where(ktm => ktm.state_name == ksm.state).FirstOrDefault();
                ksm.state_code = ktms.tinno;
                ksm.state_status = s.StateStatus;
                ksm.tds_id = s.TDSId;
                KSTS_TDS_MASTER ktmd = db.KSTS_TDS_MASTER.Where(ktm => ktm.tds_id == ksm.tds_id).FirstOrDefault();
                ksm.tds_percent = ktmd.tds;
                ksm.TDS = "Y";
                ksm.TIN = s.TIN;
                ksm.UpdateOn = ksm.UpdateOn;
                ksm.VAT = s.VAT;
                ksm.voucher_code = "VB";
                ksm.website = s.Website;
                ksm.UniqRowID = ksm.UniqRowID;
                db.Entry(ksm).State = System.Data.Entity.EntityState.Modified;
                try {
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception excp) {
                    throw excp;
                }
            }
            return Ok();
        }

        //opening details 

        /// <summary>
        /// List Of SuppliersOpen Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ListOfSuppliersOpenDetails/{companyCode}/{branchCode}")]
        [Route("ListOfSuppliersOpenDetails")]
        public IQueryable<SupplierOpenDetailsVM> ListOfSuppliers(string companyCode, string branchCode)
        {
            using (var transaction = db.Database.BeginTransaction()) {
                return db.KSTU_SUPPLIER_OPN_DETAILS.Where(s => s.company_code == companyCode && s.branch_code == branchCode).Select(s => new SupplierOpenDetailsVM
                {
                    ObjID = s.obj_id,
                    CompanyCode = s.company_code,
                    BranchCode = s.branch_code,
                    PartyCode = s.party_code,
                    FinYear = s.Fin_Year,
                    GSCode = s.gs_code,
                    MetalCode = s.metal_code,
                    OpnBal = s.opn_bal,
                    WeightType = s.weight_type,
                    VoucherCode = s.voucher_code,
                    BalType = s.bal_type,
                    UniqRowID = s.UniqRowID,
                    RefNo = s.ref_no,
                    OpnNetWeight = s.opn_net_weight,
                    OpnPureWeight = s.opn_pure_weight,
                    OpnWeight = s.opn_weight,
                    ObjectStatus = s.obj_status,
                    UpdateOn = s.UpdateOn,
                }).Take(100).OrderByDescending(c => c.BranchCode);
            }
        }

        /// <summary>
        /// Save SuppliersOpen Details
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostSuppliersOpenDetails")]
        [ResponseType(typeof(SupplierOpenDetailsVM))]
        public IHttpActionResult PostSuppliersOpenDetails([FromBody]SupplierOpenDetailsVM s)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            using (var transaction = db.Database.BeginTransaction()) {
                KSTU_SUPPLIER_OPN_DETAILS ksod = new KSTU_SUPPLIER_OPN_DETAILS();
                ksod.obj_id = Common.GetNewGUID();
                ksod.company_code = s.CompanyCode;
                ksod.branch_code = s.BranchCode;
                ksod.bal_type = s.BalType;
                ksod.Fin_Year = s.FinYear;
                ksod.gs_code = s.GSCode;
                ksod.metal_code = s.MetalCode;
                ksod.obj_status = s.ObjectStatus;
                ksod.opn_bal = s.OpnBal;
                ksod.opn_net_weight = s.OpnNetWeight;
                ksod.opn_pure_weight = Convert.ToDecimal(s.OpnWeight * (93 / 100));
                ksod.opn_weight = s.OpnWeight;
                ksod.party_code = s.PartyCode;
                ksod.ref_no = s.RefNo;
                ksod.UniqRowID = Guid.NewGuid();
                ksod.UpdateOn = Framework.Common.GetDateTime();
                ksod.voucher_code = "VB";
                ksod.weight_type = s.WeightType;
                db.KSTU_SUPPLIER_OPN_DETAILS.Add(ksod);
                try {
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception excp) {
                    transaction.Rollback();
                    throw excp;
                }
            }
            return Ok();
        }

        /// <summary>
        /// Edit SuppliersOpen Details
        /// </summary>
        /// <param name="ObjID"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("PutSuppliersOpenDetails/{ObjID}")]
        [ResponseType(typeof(SupplierOpenDetailsVM))]
        public IHttpActionResult putsupplieropndetails(string ObjID, SupplierOpenDetailsVM s)
        {
            KSTU_SUPPLIER_OPN_DETAILS ksod = db.KSTU_SUPPLIER_OPN_DETAILS.Where(d => d.obj_id == ObjID && d.company_code == s.CompanyCode && d.branch_code == s.BranchCode).FirstOrDefault();
            if (ksod == null) {
                return NotFound();
            }
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            if (s.ObjID != ksod.obj_id) {
                return BadRequest();
            }
            using (var transaction = db.Database.BeginTransaction()) {
                ksod.obj_id = ksod.obj_id;
                ksod.company_code = Common.CompanyCode;
                ksod.branch_code = ksod.branch_code;
                ksod.bal_type = s.BalType;
                ksod.Fin_Year = s.FinYear;
                ksod.gs_code = s.GSCode;
                ksod.metal_code = s.MetalCode;
                ksod.obj_status = s.ObjectStatus;
                ksod.opn_bal = s.OpnBal;
                ksod.opn_net_weight = s.OpnNetWeight;
                ksod.opn_pure_weight = s.OpnPureWeight;
                ksod.opn_weight = s.OpnWeight;
                ksod.party_code = ksod.party_code;
                ksod.ref_no = s.RefNo;
                ksod.UniqRowID = ksod.UniqRowID;
                ksod.UpdateOn = ksod.UpdateOn;
                ksod.voucher_code = "VB";
                ksod.weight_type = s.WeightType;
                ksod.UpdateOn = ksod.UpdateOn;
                db.Entry(ksod).State = System.Data.Entity.EntityState.Modified;
                try {
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception excp) {
                    transaction.Rollback();
                    throw excp;
                }
            }
            return Ok();
        }

        public IHttpActionResult Count(ODataQueryOptions<SupplierMasterVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Post(SupplierMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, SupplierMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Delete([FromBody] int id)
        {
            throw new NotImplementedException();
        }

        IQueryable<SupplierMasterVM> IBaseMasterActionController<SupplierMasterVM, SupplierMasterVM>.List()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
