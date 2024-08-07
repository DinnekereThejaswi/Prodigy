using ProdigyAPI.BL.BusinessLayer.PrintConfig;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Print;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Author: Mustureswara M M
/// Date: 11th June 2021
/// </summary>
namespace ProdigyAPI.BL.BusinessLayer.Masters
{
    public sealed class VendorMasterBL
    {
        #region Declaration
        private readonly MagnaDbEntities dbContext = new MagnaDbEntities();
        private static readonly Lazy<VendorMasterBL> vendorInstance = new Lazy<VendorMasterBL>(() => new VendorMasterBL());
        public static VendorMasterBL GetInstance
        {
            get
            {
                return vendorInstance.Value;
            }
        }
        #endregion

        #region Constructor

        private VendorMasterBL() { }

        //public VendorMasterBL()
        //{
        //    this.dbContext = new MagnaDbEntities();

        //}
        //public VendorMasterBL(MagnaDbEntities db)
        //{
        //    this.dbContext = db;

        //}
        #endregion

        #region Public Methods

        /// <summary>
        /// Provides List of TDS
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public dynamic GetTDS(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                //The bellow Procedure call thrwoing some Primary Key Rleated Error.
                //var data = dbContext.usp_LoadTDS(companyCode, branchCode, "C");

                DataTable table = SIGlobals.Globals.ExecuteQuery("exec usp_LoadTDS '" + companyCode + "','" + branchCode + "', 'C'");
                return table;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        /// <summary>
        /// Provides Metal Type
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public dynamic GetMetalType(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                return dbContext.usp_SupplyMetalTypes().ToList();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        /// <summary>
        /// Provides GS Type
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public dynamic GetGSType(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                var data = dbContext.KSTS_GS_ITEM_ENTRY.Where(kgi => kgi.company_code == companyCode
                                                            && kgi.branch_code == branchCode
                                                            && kgi.object_status != "C")
                                                            .ToList()
                                                            .OrderBy(kg => kg.display_order)
                                                            .Select(kg => new
                                                            {
                                                                Code = kg.gs_code,
                                                                Name = kg.item_level1_name,
                                                            });
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        /// <summary>
        /// Provides Open Type
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public dynamic GetOpenType(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                //string[] p1 = { "I" };
                //string[] p2 = { "VD", "RB", "IB", "IO", "RG", "VB", "RN", "VT", "RU", "RO", "DS", "VS", "PR" };
                //var data = dbContext.KSTU_ISSUERECEIPTS_TYPES.Where(kit => kit.company_code == companyCode
                //                                                    && kit.branch_code == branchCode
                //                                                    && kit.ir_code.StartsWith("I")
                //                                                    || !p2.Contains(kit.ir_code)).ToList();

                string[] p1 = { "I" };
                string[] p2 = { "VD", "RB", "IB", "IO", "RG", "VB", "RN", "VT", "RU", "RO", "DS", "VS", "PR" };
                var data = dbContext.KSTU_ISSUERECEIPTS_TYPES.Where(kit => kit.company_code == companyCode
                                                                    && kit.branch_code == branchCode
                                                                    && kit.ir_code.StartsWith("I"));
                return data.Where(p => !p2.Contains(p.ir_code));
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        /// <summary>
        /// Provides Group To List
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public dynamic GetGroupTo(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                // We used Left Join in LINQ Query
                var data = (from kit in dbContext.KSTU_ISSUERECEIPTS_TYPES
                            join alm in dbContext.KSTU_ACC_LEDGER_MASTER
                            on new { CompanyCode = kit.company_code, BranchCode = kit.branch_code, AccCode = kit.ac_code ?? 0 }
                            equals new { CompanyCode = alm.company_code, BranchCode = alm.branch_code, AccCode = alm.acc_code }
                into Details
                            from result in Details.DefaultIfEmpty()
                            where kit.voucher_code.Contains("I") || kit.voucher_code.Contains("R")
                            select new
                            {
                                VoucherCode = kit.voucher_code,
                                IRCode = kit.ir_code,
                                IRName = kit.ir_name,
                                AccCode = kit.ac_code,
                                AccName = result.acc_name
                            }).ToList();

                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public IQueryable<SupplierMasterVM> GetVendorList(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                var data = dbContext.KSTU_SUPPLIER_MASTER.Where(sp => sp.company_code == companyCode
                                                            && sp.branch_code == branchCode).Select(
                    sp => new SupplierMasterVM()
                    {
                        ObjID = sp.obj_id,
                        CompanyCode = sp.company_code,
                        BranchCode = sp.branch_code,
                        PartyCode = sp.party_code,
                        PartyName = sp.party_name,
                        VoucherCode = sp.voucher_code,
                        Address1 = sp.address1,
                        Address2 = sp.address2,
                        Address3 = sp.address3,
                        District = sp.district,
                        State = sp.state,
                        StateStatus = sp.state_status,
                        City = sp.city,
                        Country = sp.country,
                        PinCode = sp.pincode,
                        Phone = sp.phone,
                        Mobile = sp.mobile,
                        FaxNo = sp.FaxNo,
                        PAN = sp.pan_no,
                        Website = sp.website,
                        TIN = sp.TIN,
                        TDS = sp.TDS,
                        VAT = sp.VAT,
                        CSTNo = sp.cst_no,
                        ObjectStatus = sp.obj_status,
                        UpdateOn = sp.UpdateOn,
                        PartyRTGScode = sp.Party_RTGScode,
                        PartyNEFTcode = sp.Party_NEFTcode,
                        PartyIFSCcode = sp.Party_IFSCcode,
                        PartyAccountNo = sp.Party_AC_No,
                        PartyMICRNo = sp.Party_MICR_No,
                        PartyBankName = sp.Party_BankName,
                        PartyBankBranch = sp.Party_BankBranch,
                        PartyBankAddress = sp.Party_BankAddress,
                        ContactPerson = sp.Contact_Person,
                        ContactEmail = sp.Contact_Email,
                        Email = sp.Contact_Email,
                        ContactMobileNo = sp.Contact_MobileNo,
                        PartyACName = sp.Party_AC_Name,
                        OpnBal = sp.opn_bal,
                        OpnBalType = sp.opn_bal_type,
                        OpnWeight = sp.opn_wt,
                        OpnWeightType = sp.opn_wt_type,
                        CreditPeriod = sp.credit_period,
                        LeadTime = sp.lead_time,
                        MaxPayment = sp.max_payment,
                        ConvRate = sp.conv_rate,
                        SwiftCode = sp.swift_code,
                        BranchType = sp.branch_type,
                        TDSPercent = sp.tds_percent,
                        CreditWeight = sp.credit_weight,
                        TDSId = sp.tds_id,
                        IsSameEntity = sp.isSameEntity,
                        StateCode = sp.state_code,
                        SchemeType = sp.schemetype,
                        SupplierMetal = sp.supplierMetal,
                        ListGroupTo = dbContext.KSTU_SUPPLIER_GROUP.Where(ksg => ksg.company_code == companyCode
                                                                        && ksg.branch_code == branchCode
                                                                        && ksg.party_code == sp.party_code)
                                                                   .Select(ksg => new SupplierGroupToVM()
                                                                   {
                                                                       ObjID = ksg.obj_id,
                                                                       CompanyCode = ksg.company_code,
                                                                       BranchCode = ksg.branch_code,
                                                                       PartyCode = ksg.party_code,
                                                                       VoucherCode = ksg.voucher_code,
                                                                       IRCode = ksg.ir_code,
                                                                       GroupName = ksg.group_name,
                                                                   }).ToList(),
                        ListOpenDet = dbContext.KSTU_SUPPLIER_OPN_DETAILS.Where(ksod => ksod.company_code == companyCode
                                                                    && ksod.branch_code == branchCode
                                                                    && ksod.party_code == sp.party_code
                                                                    ).Select(ksod => new SupplierOpenDetailsVM()
                                                                    {
                                                                        ObjID = ksod.obj_id,
                                                                        CompanyCode = ksod.company_code,
                                                                        BranchCode = ksod.branch_code,
                                                                        PartyCode = ksod.party_code,
                                                                        VoucherCode = ksod.voucher_code,
                                                                        OpnBal = ksod.opn_bal,
                                                                        BalType = ksod.bal_type,
                                                                        OpnWeight = ksod.opn_weight,
                                                                        WeightType = ksod.weight_type,
                                                                        ObjectStatus = ksod.obj_status,
                                                                        UpdateOn = ksod.UpdateOn,
                                                                        MetalCode = ksod.metal_code,
                                                                        OpnPureWeight = ksod.opn_pure_weight,
                                                                        OpnNetWeight = ksod.opn_net_weight,
                                                                        FinYear = ksod.Fin_Year,
                                                                        GSCode = ksod.gs_code,
                                                                        RefNo = ksod.ref_no
                                                                    }).ToList()
                    });
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        /// <summary>
        /// Provides Vendor Details by Object ID.
        /// </summary>
        /// <param name="objID"></param>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public SupplierMasterVM GetVendor(string objID, string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {

                var data = GetVendorList(companyCode, branchCode, out error).Where(v => v.ObjID == objID).FirstOrDefault();
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        /// <summary>
        /// Save Vendor/Supplier details by SupplierMasterVM
        /// </summary>
        /// <param name="supplier"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool Save(SupplierMasterVM supplier, out ErrorVM error)
        {
            error = null;
            KSTU_SUPPLIER_MASTER sp = new KSTU_SUPPLIER_MASTER();
            if (supplier == null) {
                error = new ErrorVM()
                {
                    description = "Invalid supplier/vendoer details",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }

            string companyCode = supplier.CompanyCode;
            string branchCode = supplier.BranchCode;
            sp = dbContext.KSTU_SUPPLIER_MASTER.Where(ksm => ksm.company_code == companyCode
                                                    && ksm.branch_code == branchCode
                                                    && ksm.party_code == supplier.PartyCode).FirstOrDefault();
            if (sp != null) {
                error = new ErrorVM()
                {
                    description = "Supplier Code already Exist",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            sp = new KSTU_SUPPLIER_MASTER();
            try {
                string objectID = "KSTU_SUPPLIER_MASTER" + SIGlobals.Globals.Separator
                                + supplier.PartyCode + SIGlobals.Globals.Separator
                                + supplier.VoucherCode + SIGlobals.Globals.Separator
                                + supplier.CompanyCode + SIGlobals.Globals.Separator
                                + supplier.BranchCode;
                sp.obj_id = SIGlobals.Globals.GetHashcode(objectID);
                sp.company_code = supplier.CompanyCode;
                sp.branch_code = supplier.BranchCode;
                sp.party_code = supplier.PartyCode;
                sp.party_name = supplier.PartyName;
                sp.voucher_code = supplier.VoucherCode;
                sp.address1 = supplier.Address1;
                sp.address2 = supplier.Address2;
                sp.address3 = supplier.Address3;
                sp.district = supplier.District;
                sp.state = supplier.State;
                sp.state_status = supplier.StateStatus;
                sp.city = supplier.City;
                sp.country = supplier.Country;
                sp.pincode = supplier.PinCode;
                sp.phone = supplier.Phone;
                sp.mobile = supplier.Mobile;
                sp.FaxNo = supplier.FaxNo;
                sp.pan_no = supplier.PAN;
                sp.website = supplier.Website;
                sp.TIN = supplier.TIN;
                sp.TDS = supplier.TDS;
                sp.VAT = supplier.VAT;
                sp.cst_no = supplier.CSTNo;
                sp.obj_status = supplier.ObjectStatus;
                sp.UpdateOn = SIGlobals.Globals.GetDateTime();
                sp.Party_RTGScode = supplier.PartyRTGScode;
                sp.Party_NEFTcode = supplier.PartyNEFTcode;
                sp.Party_IFSCcode = supplier.PartyIFSCcode;
                sp.Party_AC_No = supplier.PartyAccountNo;
                sp.Party_MICR_No = supplier.PartyMICRNo;
                sp.Party_BankName = supplier.PartyBankName;
                sp.Party_BankBranch = supplier.PartyBankBranch;
                sp.Party_BankAddress = supplier.PartyBankAddress;
                sp.Contact_Person = supplier.ContactPerson;
                sp.Contact_Email = supplier.ContactEmail;
                sp.Contact_Email = supplier.Email;
                sp.Contact_MobileNo = supplier.ContactMobileNo;
                sp.Party_AC_Name = supplier.PartyACName;
                sp.opn_bal = supplier.OpnBal;
                sp.opn_bal_type = supplier.OpnBalType;
                sp.opn_wt = supplier.OpnWeight;
                sp.opn_wt_type = supplier.OpnWeightType;
                sp.credit_period = supplier.CreditPeriod;
                sp.lead_time = supplier.LeadTime;
                sp.max_payment = supplier.MaxPayment;
                sp.conv_rate = supplier.ConvRate;
                sp.swift_code = supplier.SwiftCode;
                sp.branch_type = supplier.BranchType;
                sp.tds_percent = supplier.TDSPercent;
                sp.credit_weight = supplier.CreditWeight;
                sp.tds_id = supplier.TDSId;
                sp.isSameEntity = supplier.IsSameEntity;
                sp.state_code = supplier.StateCode;
                sp.schemetype = supplier.SchemeType;
                sp.supplierMetal = supplier.SupplierMetal;
                sp.UniqRowID = Guid.NewGuid();
                dbContext.KSTU_SUPPLIER_MASTER.Add(sp);

                foreach (SupplierGroupToVM g in supplier.ListGroupTo) {
                    KSTU_SUPPLIER_GROUP group = new KSTU_SUPPLIER_GROUP();
                    group.obj_id = sp.obj_id;
                    group.company_code = g.CompanyCode;
                    group.branch_code = g.BranchCode;
                    group.party_code = g.PartyCode;
                    group.voucher_code = g.VoucherCode;
                    group.ir_code = g.IRCode;
                    group.group_name = g.GroupName;
                    group.UniqRowID = Guid.NewGuid();
                    dbContext.KSTU_SUPPLIER_GROUP.Add(group);
                }

                foreach (SupplierOpenDetailsVM open in supplier.ListOpenDet) {
                    KSTU_SUPPLIER_OPN_DETAILS ksod = new KSTU_SUPPLIER_OPN_DETAILS();
                    ksod.obj_id = sp.obj_id;
                    ksod.company_code = open.CompanyCode;
                    ksod.branch_code = open.BranchCode;
                    ksod.party_code = open.PartyCode;
                    ksod.voucher_code = open.VoucherCode;
                    ksod.opn_bal = open.OpnBal;
                    ksod.bal_type = open.BalType;
                    ksod.opn_weight = open.OpnWeight;
                    ksod.weight_type = open.WeightType;
                    ksod.obj_status = open.ObjectStatus;
                    ksod.UpdateOn = SIGlobals.Globals.GetDateTime();
                    ksod.metal_code = open.MetalCode;
                    ksod.opn_pure_weight = open.OpnPureWeight;
                    ksod.opn_net_weight = open.OpnNetWeight;
                    ksod.Fin_Year = open.FinYear;
                    ksod.gs_code = open.GSCode;
                    ksod.ref_no = open.RefNo;
                    ksod.UniqRowID = Guid.NewGuid();
                    dbContext.KSTU_SUPPLIER_OPN_DETAILS.Add(ksod);
                }
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        /// <summary>
        /// Update Vendor/Supplier Details by SupplierMasterVM
        /// </summary>
        /// <param name="objID"></param>
        /// <param name="supplier"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool Update(string objID, SupplierMasterVM supplier, out ErrorVM error)
        {
            error = null;

            using (var transaction = dbContext.Database.BeginTransaction()) {
                try {
                    error = null;
                    KSTU_SUPPLIER_MASTER sp = new KSTU_SUPPLIER_MASTER();
                    if (supplier == null) {
                        error = new ErrorVM()
                        {
                            description = "Invalid supplier/vendoer details",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }

                    string companyCode = supplier.CompanyCode;
                    string branchCode = supplier.BranchCode;




                    sp = dbContext.KSTU_SUPPLIER_MASTER.Where(ksm => ksm.company_code == companyCode
                                                            && ksm.branch_code == branchCode
                                                            && ksm.obj_id == supplier.ObjID).FirstOrDefault();
                    if (sp == null) {
                        error = new ErrorVM()
                        {
                            description = "Supplier Details not exist",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }

                    if (sp.party_code != supplier.PartyCode) {
                        error = new ErrorVM()
                        {
                            description = "Supplier Code Cannot Change",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                    sp.party_name = supplier.PartyName;
                    sp.voucher_code = supplier.VoucherCode;
                    sp.address1 = supplier.Address1;
                    sp.address2 = supplier.Address2;
                    sp.address3 = supplier.Address3;
                    sp.district = supplier.District;
                    sp.state = supplier.State;
                    sp.state_status = supplier.StateStatus;
                    sp.city = supplier.City;
                    sp.country = supplier.Country;
                    sp.pincode = supplier.PinCode;
                    sp.phone = supplier.Phone;
                    sp.mobile = supplier.Mobile;
                    sp.FaxNo = supplier.FaxNo;
                    sp.pan_no = supplier.PAN;
                    sp.website = supplier.Website;
                    sp.TIN = supplier.TIN;
                    sp.TDS = supplier.TDS;
                    sp.VAT = supplier.VAT;
                    sp.cst_no = supplier.CSTNo;
                    sp.obj_status = supplier.ObjectStatus;
                    sp.UpdateOn = SIGlobals.Globals.GetDateTime();
                    sp.Party_RTGScode = supplier.PartyRTGScode;
                    sp.Party_NEFTcode = supplier.PartyNEFTcode;
                    sp.Party_IFSCcode = supplier.PartyIFSCcode;
                    sp.Party_AC_No = supplier.PartyAccountNo;
                    sp.Party_MICR_No = supplier.PartyMICRNo;
                    sp.Party_BankName = supplier.PartyBankName;
                    sp.Party_BankBranch = supplier.PartyBankBranch;
                    sp.Party_BankAddress = supplier.PartyBankAddress;
                    sp.Contact_Person = supplier.ContactPerson;
                    sp.Contact_Email = supplier.ContactEmail;
                    sp.Contact_Email = supplier.Email;
                    sp.Contact_MobileNo = supplier.ContactMobileNo;
                    sp.Party_AC_Name = supplier.PartyACName;
                    sp.opn_bal = supplier.OpnBal;
                    sp.opn_bal_type = supplier.OpnBalType;
                    sp.opn_wt = supplier.OpnWeight;
                    sp.opn_wt_type = supplier.OpnWeightType;
                    sp.credit_period = supplier.CreditPeriod;
                    sp.lead_time = supplier.LeadTime;
                    sp.max_payment = supplier.MaxPayment;
                    sp.conv_rate = supplier.ConvRate;
                    sp.swift_code = supplier.SwiftCode;
                    sp.branch_type = supplier.BranchType;
                    sp.tds_percent = supplier.TDSPercent;
                    sp.credit_weight = supplier.CreditWeight;
                    sp.tds_id = supplier.TDSId;
                    sp.isSameEntity = supplier.IsSameEntity;
                    sp.state_code = supplier.StateCode;
                    sp.schemetype = supplier.SchemeType;
                    sp.supplierMetal = supplier.SupplierMetal;
                    sp.UniqRowID = Guid.NewGuid();
                    dbContext.Entry(sp).State = System.Data.Entity.EntityState.Modified;


                    List<KSTU_SUPPLIER_GROUP> lstOfSupplierGroup = dbContext.KSTU_SUPPLIER_GROUP.Where(sup => sup.obj_id == supplier.ObjID
                                                                                                        && sup.party_code == sp.party_code
                                                                                                        && sup.company_code == sup.company_code
                                                                                                        && sup.branch_code == supplier.BranchCode).ToList();
                    dbContext.KSTU_SUPPLIER_GROUP.RemoveRange(lstOfSupplierGroup);
                    dbContext.SaveChanges();

                    foreach (SupplierGroupToVM g in supplier.ListGroupTo) {
                        KSTU_SUPPLIER_GROUP group = new KSTU_SUPPLIER_GROUP();
                        group.obj_id = g.ObjID;
                        group.company_code = g.CompanyCode;
                        group.branch_code = g.BranchCode;
                        group.party_code = sp.party_code;
                        group.voucher_code = g.VoucherCode;
                        group.ir_code = g.IRCode;
                        group.group_name = g.GroupName;
                        group.UpdateOn = DateTime.Now;
                        group.UniqRowID = Guid.NewGuid();
                        dbContext.KSTU_SUPPLIER_GROUP.Add(group);
                    }

                    List<KSTU_SUPPLIER_OPN_DETAILS> lstOfSupplierOpn = dbContext.KSTU_SUPPLIER_OPN_DETAILS.Where(sup => sup.obj_id == supplier.ObjID
                                                                                                       && sup.party_code == sp.party_code
                                                                                                       && sup.company_code == sup.company_code
                                                                                                       && sup.branch_code == supplier.BranchCode).ToList();
                    dbContext.KSTU_SUPPLIER_OPN_DETAILS.RemoveRange(lstOfSupplierOpn);
                    dbContext.SaveChanges();

                    foreach (SupplierOpenDetailsVM open in supplier.ListOpenDet) {
                        KSTU_SUPPLIER_OPN_DETAILS ksod = new KSTU_SUPPLIER_OPN_DETAILS();
                        ksod.obj_id = sp.obj_id;
                        ksod.company_code = open.CompanyCode;
                        ksod.branch_code = open.BranchCode;
                        ksod.party_code = sp.party_code;
                        ksod.voucher_code = open.VoucherCode;
                        ksod.opn_bal = open.OpnBal;
                        ksod.bal_type = open.BalType;
                        ksod.opn_weight = open.OpnWeight;
                        ksod.weight_type = open.WeightType;
                        ksod.obj_status = open.ObjectStatus;
                        ksod.UpdateOn = DateTime.Now;
                        ksod.metal_code = open.MetalCode;
                        ksod.opn_pure_weight = open.OpnPureWeight;
                        ksod.opn_net_weight = open.OpnNetWeight;
                        ksod.Fin_Year = open.FinYear;
                        ksod.gs_code = open.GSCode;
                        ksod.ref_no = open.RefNo;
                        ksod.UniqRowID = Guid.NewGuid();
                        dbContext.KSTU_SUPPLIER_OPN_DETAILS.Add(ksod);
                    }
                    dbContext.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception excp) {
                    error = new ErrorVM().GetErrorDetails(excp);
                    transaction.Rollback();
                    return false;
                }
            }
        }

        #region Print

        /// <summary>
        /// Provides Print of Vendor Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public ProdigyPrintVM PrintVendor(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            string printData = PrintVendorList(companyCode, branchCode, out error);
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            printObject.PrintType = "HTML";
            printObject.ContinueNextPrint = true;
            printObject.Data = new PrintConfiguration().Base64Encode(printData);
            return printObject;
        }

        /// <summary>
        /// Provides Print of Vendor Full Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public ProdigyPrintVM PrintVendorFullDet(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            string printData = PrintVendorFullDetails(companyCode, branchCode, out error);
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            printObject.PrintType = "HTML";
            printObject.ContinueNextPrint = true;
            printObject.Data = new PrintConfiguration().Base64Encode(printData);
            return printObject;
        }

        #endregion

        #endregion

        #region Protected Methods
        protected string PrintVendorList(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            StringBuilder sb = new StringBuilder();
            try {
                string[] vc = { "VT" };
                List<KSTU_SUPPLIER_MASTER> supplier = dbContext.KSTU_SUPPLIER_MASTER.Where(sup => sup.company_code == companyCode
                                                                                                    && sup.branch_code == branchCode
                                                                                                    && sup.obj_status == "O"
                                                                                                    && !vc.Contains(sup.voucher_code)).ToList();
                sb.AppendLine("<HTML>");
                sb.AppendLine("<HEAD>");
                sb.AppendLine(SIGlobals.Globals.GetStyle());
                sb.AppendLine("</HEAD>");
                sb.AppendLine("<BODY>");

                sb.AppendLine("<table bgcolor=\"#FDFDF0\" class=\"boldText\" frame=\"border\" border=\"2\" width=\"950\" align=\"LEFT\" >");
                sb.AppendLine("<TR height=\"1px\">");
                sb.AppendLine("<ph/>");
                sb.AppendLine(string.Format("<th  class=\"noborder\" colspan=2 align=\"CENTER\" bgcolor=\"EBE7C6\" ><H3 style=\"color:'maroon'\">{0}</H3></th>", SIGlobals.Globals.FillCompanyDetails("Company_Name", companyCode, branchCode)));
                sb.AppendLine("</TR>");

                sb.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\" class = \"fullborderStyle\">");
                sb.AppendLine(string.Format("<ph/><TD ALIGN = \"CENTER\" colspan = 2> <b> {0} - Supplier List</b></td>", branchCode));
                sb.AppendLine("</TR>");
                sb.AppendLine("<tr bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\" class = \"fullborderStyle\">");
                sb.AppendLine("<pth/>");
                sb.AppendLine("<td align=\"LEFT\"><b>Supplier Code</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>Supplier Name</b></td>");
                sb.AppendLine("</tr>");

                for (int i = 0; i < supplier.Count(); i++) {
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD ALIGN = \"LEFT\">{0}</TD>", supplier[i].party_code));
                    sb.AppendLine(string.Format("<TD ALIGN = \"LEFT\">{0}</TD>", supplier[i].party_name));
                    sb.AppendLine("</TR>");
                }
                sb.AppendLine("</table>");
                sb.AppendLine("</BODY>");
                sb.AppendLine("</HTML>");
                return sb.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        protected string PrintVendorFullDetails(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            StringBuilder sb = new StringBuilder();
            try {
                string[] vc = { "VT" };
                List<KSTU_SUPPLIER_MASTER> supplier = dbContext.KSTU_SUPPLIER_MASTER.Where(sup => sup.company_code == companyCode
                                                                                                    && sup.branch_code == branchCode
                                                                                                    && sup.obj_status == "O"
                                                                                                    && !vc.Contains(sup.voucher_code)).ToList();
                sb.AppendLine("<HTML>");
                sb.AppendLine("<HEAD>");
                sb.AppendLine(SIGlobals.Globals.GetStyle());
                sb.AppendLine("</HEAD>");
                sb.AppendLine("<BODY>");

                sb.AppendLine("<table bgcolor=\"#FDFDF0\" class=\"boldText\" frame=\"border\" border=\"2\" width=\"950\" align=\"LEFT\" >");
                sb.AppendLine("<TR height=\"1px\">");
                sb.AppendLine("<ph/>");
                sb.AppendLine(string.Format("<th  class=\"noborder\" colspan=9 align=\"CENTER\" bgcolor=\"EBE7C6\" ><H3 style=\"color:'maroon'\">{0}</H3></th>", SIGlobals.Globals.FillCompanyDetails("Company_Name", companyCode, branchCode)));
                sb.AppendLine("</TR>");

                sb.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\" class = \"fullborderStyle\">");
                sb.AppendLine(string.Format("<ph/><TD ALIGN = \"CENTER\" colspan = 9> <b> {0} - Supplier List</b></td>", branchCode));
                sb.AppendLine("</TR>");
                sb.AppendLine("<tr bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\" class = \"fullborderStyle\">");
                sb.AppendLine("<pth/>");
                sb.AppendLine("<td align=\"LEFT\"><b>SlNo</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>Code</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>Name</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>Phone</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>Mobile</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>Address</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>PAN</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>TIN</b></td>");
                sb.AppendLine("<td align=\"LEFT\"><b>CSTNo</b></td>");
                sb.AppendLine("</tr>");

                for (int i = 0; i < supplier.Count(); i++) {
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD ALIGN = \"LEFT\">{0}</TD>", i + 1));
                    sb.AppendLine(string.Format("<TD ALIGN = \"LEFT\">{0}</TD>", supplier[i].party_code));
                    sb.AppendLine(string.Format("<TD ALIGN = \"LEFT\">{0}</TD>", supplier[i].party_name));
                    sb.AppendLine(string.Format("<TD ALIGN = \"LEFT\">{0}</TD>", supplier[i].phone));
                    sb.AppendLine(string.Format("<TD ALIGN = \"LEFT\">{0}</TD>", supplier[i].mobile));
                    sb.AppendLine(string.Format("<TD ALIGN = \"LEFT\">{0}</TD>", supplier[i].address1 + supplier[i].address2 + supplier[i].address3));
                    sb.AppendLine(string.Format("<TD ALIGN = \"LEFT\">{0}</TD>", supplier[i].pan_no));
                    sb.AppendLine(string.Format("<TD ALIGN = \"LEFT\">{0}</TD>", supplier[i].TIN));
                    sb.AppendLine(string.Format("<TD ALIGN = \"LEFT\">{0}</TD>", supplier[i].cst_no));
                    sb.AppendLine("</TR>");
                }
                sb.AppendLine("</table>");
                sb.AppendLine("</BODY>");
                sb.AppendLine("</HTML>");
                return sb.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }
        #endregion
    }
}
