using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Framework;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData.Query;
using System.Web.Http.Results;

namespace ProdigyAPI.Controllers.Masters
{
    /// <summary>
    /// Customer controller Provides API's for Customer realted information.
    /// </summary>
    //[Authorize]
    [RoutePrefix("api/masters")]
    [ProdigyAPI.Handlers.SIExceptionFilterAttribute]
    public class CustomerController : SIBaseApiController<CustomerMasterVM>, IBaseMasterActionController<CustomerMasterVM, CustomerMasterVM>
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities dbContext = new Model.MagnaDb.MagnaDbEntities();
        private const string TABLE_NAME = "KSTU_CUSTOMER_MASTER";
        string ModuleSeqNo = "12";
        #endregion

        #region Controller Methods
        /// <summary>
        /// Get All ID Proof Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("customer/GetIDProof/{companyCode}/{branchCode}")]
        public IHttpActionResult GetIDProof(string companyCode, string branchCode)
        {
            var data = from proof in dbContext.KSTS_CUSTOMER_ID_PROOF_MASTER
                       where proof.company_code == companyCode && proof.branch_code == branchCode
                       select new
                       {
                           DocCode = proof.Doc_code,
                           DocName = proof.Doc_name
                       };
            return Ok(data);
        }

        /// <summary>
        /// Get all Customer details.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("customer/listAll/{companyCode}/{branchCode}")]
        public HttpResponseMessage ListAll(string companyCode, string branchCode)
        {
            CustomerMasterVM customer = new CustomerMasterVM();
            //APP_CONFIG_TABLE config = dbContext.APP_CONFIG_TABLE.Where(c => c.company_code == companyCode && c.branch_code == branchCode && c.obj_id == "28022019").FirstOrDefault();
            bool UseConsolidatedCustomers = Convert.ToBoolean(ConfigurationManager.AppSettings["UseConsolidatedCustomers"]);
            if (UseConsolidatedCustomers) {
                var cust = dbContext.Customers.Where(m => m.CompanyCode == companyCode && m.BranchCode == branchCode).FirstOrDefault();
                customer.ObjID = cust.ID.ToString();
                customer.CompanyCode = cust.CompanyCode;
                customer.BranchCode = cust.BranchCode;
                customer.ID = cust.ID;
                customer.CustName = cust.Name;
                customer.Address1 = cust.Address1;
                customer.Address2 = cust.Address2;
                customer.Address3 = cust.Address3;
                customer.City = cust.City;
                customer.State = cust.State;
                customer.Pincode = cust.PinCode;
                customer.MobileNo = cust.MobileNo;
                customer.PhoneNo = cust.PhoneNo;
                customer.DateOfBirth = cust.DateOfBirth;
                customer.WeddingDate = cust.WeddingAnnivarsaryDate;
                customer.CustomerType = cust.CustomerType;
                customer.ObjectStatus = cust.ObjectStatus;
                customer.SpouseName = "";
                customer.ChildName1 = "";
                customer.ChildName2 = "";
                customer.ChildName3 = "";
                customer.SpouseDateOfBirth = null;
                customer.Child1DateOfBirth = null;
                customer.Child2DateOfBirth = null;
                customer.Child3DateOfBirth = null;
                customer.PANNo = "";
                customer.IDType = "";
                customer.IDDetails = "";
                customer.UpdateOn = cust.UpdatedDate;
                customer.EmailID = cust.EmailID;
                customer.CreatedDate = cust.CreatedDate;
                customer.Salutation = cust.Salutation;
                customer.CountryCode = cust.CountryCode;
                customer.LoyaltyID = cust.LocalityID;
                customer.ICNo = "";
                customer.PassportNo = "";
                customer.PRNo = "";
                customer.PrevilegeID = cust.PrivilegeID;
                customer.Age = cust.Age;
                customer.CountryName = cust.CountryName;
                customer.CustCode = cust.CustCode;
                customer.CustCreditLimit = null;
                customer.TIN = cust.GSTTIN;
                customer.StateCode = cust.StateCode;
                customer.CorporateID = "";
                customer.CorporateBranchID = "";
                customer.EmployeeID = "";
                customer.RegisteredMN = "";
                customer.ProfessionID = "";
                customer.EmpCorpEmailID = "";
                customer.ImageIDPath = "";
                customer.CorpImageIDPath = "";
                customer.ImageIDPath2 = "";
                customer.AccHolderName = "";
                customer.Accsalutation = "";
                customer.RepoCustId = null;
                customer.UpdatedDate = cust.UpdatedDate;
                return Request.CreateResponse(HttpStatusCode.OK, customer);
            }
            else {
                var cust = dbContext.KSTU_CUSTOMER_MASTER.Where(m => m.customer_type == "R" && m.company_code == companyCode && m.branch_code == branchCode).FirstOrDefault();
                customer.ObjID = cust.obj_id;
                customer.CompanyCode = cust.company_code;
                customer.BranchCode = cust.branch_code;
                customer.ID = cust.cust_id;
                customer.CustName = cust.cust_name;
                customer.Address1 = cust.address1;
                customer.Address2 = cust.address2;
                customer.Address3 = cust.address3;
                customer.City = cust.city;
                customer.State = cust.state;
                customer.Pincode = cust.pin_code;
                customer.MobileNo = cust.mobile_no;
                customer.PhoneNo = cust.phone_no;
                customer.DateOfBirth = cust.dob;
                customer.WeddingDate = cust.wedding_date;
                customer.CustomerType = cust.customer_type;
                customer.ObjectStatus = cust.object_status;
                customer.SpouseName = cust.spouse_name;
                customer.ChildName1 = cust.child_name1;
                customer.ChildName2 = cust.child_name2;
                customer.ChildName3 = cust.child_name3;
                customer.SpouseDateOfBirth = cust.spouse_dob;
                customer.Child1DateOfBirth = cust.child1_dob;
                customer.Child2DateOfBirth = cust.child2_dob;
                customer.Child3DateOfBirth = cust.child3_dob;
                customer.PANNo = cust.pan_no;
                customer.IDType = cust.id_type;
                customer.IDDetails = cust.id_details;
                customer.UpdateOn = cust.UpdatedDate;
                customer.EmailID = cust.Email_ID;
                customer.CreatedDate = cust.Created_Date;
                customer.Salutation = cust.salutation;
                customer.CountryCode = cust.country_code;
                customer.LoyaltyID = cust.loyality_id;
                customer.ICNo = cust.ic_no;
                customer.PassportNo = cust.passport_no;
                customer.PRNo = cust.pr_no;
                customer.PrevilegeID = cust.privilege_id;
                customer.Age = cust.age;
                customer.CountryName = cust.Country_name;
                customer.CustCode = cust.cust_code;
                customer.CustCreditLimit = cust.cust_credit_limit;
                customer.TIN = cust.tin;
                customer.StateCode = cust.state_code;
                customer.CorporateID = cust.Corparate_ID;
                customer.CorporateBranchID = cust.Corporate_Branch_ID;
                customer.EmployeeID = cust.Employee_Id;
                customer.RegisteredMN = cust.Registered_MN;
                customer.ProfessionID = cust.profession_ID;
                customer.EmpCorpEmailID = cust.Empcorp_Email_ID;
                customer.ImageIDPath = cust.imageid_path;
                customer.CorpImageIDPath = cust.corp_imageid_path;
                customer.ImageIDPath2 = cust.imageid_path2;
                customer.AccHolderName = cust.AccHolderName;
                customer.Accsalutation = cust.Accsalutation;
                customer.RepoCustId = cust.RepoCustId;
                customer.UpdatedDate = cust.UpdatedDate;
                return Request.CreateResponse(HttpStatusCode.OK, cust);
            }
        }

        /// <summary>
        /// Get Customer details by Customer ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("customer/GetByID/{id}/{companyCode}/{branchCode}")]
        [ResponseType(typeof(CustomerMasterVM))]
        public IHttpActionResult Get(int id, string companyCode, string branchCode)
        {
            // Bellow Line of code throwing an Error.
            //"Message": "Unhandled exception. The number of primary key values passed must match number 
            // of primary key values defined on the entity.\r\nParameter name: keyValues"

            //KSTU_CUSTOMER_MASTER c = db.KSTU_CUSTOMER_MASTER.Find(id);

            //KSTU_CUSTOMER_MASTER cmr = dbContext.KSTU_CUSTOMER_MASTER.Where(ct => ct.mobile_no == "7411789384").FirstOrDefault();

            //Customer misCustomer = dbContext.Customers.Where(misCust => misCust.ID == id && misCust.BranchCode == branchCode).FirstOrDefault();

            // Commented the bellow code with NR sir guidelines, only picking from local customer master.
            //Customer misCustomer = dbContext.Customers.Where(misCust => misCust.ID == id && misCust.BranchCode == branchCode).FirstOrDefault();
            //List<CustomerDocument> lstOfmisDoc = dbContext.CustomerDocuments.Where(custDoc => custDoc.CustID == id).ToList();
            //if (misCustomer != null) {
            //    CustomerMasterVM cust = new CustomerMasterVM();
            //    cust.ObjID = misCustomer.ID.ToString();
            //    cust.CompanyCode = misCustomer.CompanyCode;
            //    cust.BranchCode = misCustomer.BranchCode;
            //    cust.ID = misCustomer.ID;
            //    cust.CustName = misCustomer.Name;
            //    cust.Address1 = misCustomer.Address1;
            //    cust.Address2 = misCustomer.Address2;
            //    cust.Address3 = misCustomer.Address3;
            //    cust.City = misCustomer.City;
            //    cust.State = misCustomer.State;
            //    cust.Pincode = misCustomer.PinCode;
            //    cust.MobileNo = misCustomer.MobileNo;
            //    cust.PhoneNo = misCustomer.PhoneNo;
            //    cust.DateOfBirth = misCustomer.DateOfBirth;
            //    cust.WeddingDate = misCustomer.WeddingAnnivarsaryDate;
            //    cust.CustomerType = misCustomer.CustomerType;
            //    cust.ObjectStatus = misCustomer.ObjectStatus;
            //    cust.SpouseName = "";
            //    cust.ChildName1 = "";
            //    cust.ChildName2 = "";
            //    cust.ChildName3 = "";
            //    cust.SpouseDateOfBirth = null;
            //    cust.Child1DateOfBirth = null;
            //    cust.Child2DateOfBirth = null;
            //    cust.Child3DateOfBirth = null;
            //    cust.PANNo = "";
            //    cust.IDType = "";
            //    cust.IDDetails = "";
            //    cust.UpdateOn = misCustomer.UpdatedDate;
            //    cust.EmailID = misCustomer.EmailID;
            //    cust.CreatedDate = misCustomer.CreatedDate;
            //    cust.Salutation = misCustomer.Salutation;
            //    cust.CountryCode = misCustomer.CountryCode;
            //    cust.LoyaltyID = misCustomer.LocalityID;
            //    cust.ICNo = "";
            //    cust.PassportNo = "";
            //    cust.PRNo = "";
            //    cust.PrevilegeID = misCustomer.PrivilegeID;
            //    cust.Age = misCustomer.Age;
            //    cust.CountryName = misCustomer.CountryName;
            //    cust.CustCode = misCustomer.CustCode;
            //    cust.CustCreditLimit = null;
            //    cust.TIN = misCustomer.GSTTIN;
            //    cust.StateCode = misCustomer.StateCode;
            //    cust.CorporateID = "";
            //    cust.CorporateBranchID = "";
            //    cust.EmployeeID = "";
            //    cust.RegisteredMN = "";
            //    cust.ProfessionID = "";
            //    cust.EmpCorpEmailID = "";
            //    cust.ImageIDPath = "";
            //    cust.CorpImageIDPath = "";
            //    cust.ImageIDPath2 = "";
            //    cust.AccHolderName = "";
            //    cust.Accsalutation = "";
            //    cust.RepoCustId = null;
            //    cust.UpdatedDate = misCustomer.UpdatedDate;

            //    List<CustomerIDProofDetailsVM> lstOfMISCustDetVM = new List<CustomerIDProofDetailsVM>();
            //    int slno = 1;
            //    foreach (CustomerDocument cipd in lstOfmisDoc) {
            //        CustomerIDProofDetailsVM det = new CustomerIDProofDetailsVM();
            //        det.objID = cipd.ID.ToString();
            //        det.CompanyCode = "";
            //        det.BranchCode = "";
            //        det.SlNo = slno;
            //        det.DocCode = cipd.Type;
            //        det.DocName = cipd.Name;
            //        det.DocNo = cipd.Name;
            //        det.DocImage = null;
            //        det.RepoCustId = null;
            //        det.UpdatedDate = null;
            //        det.RepDocID = null;
            //        det.DocImagePath = cipd.ImagePath;
            //        lstOfMISCustDetVM.Add(det);
            //        slno = slno + 1;
            //    }
            //    cust.lstOfProofs = lstOfMISCustDetVM;
            //    return Ok(cust);
            //}

            KSTU_CUSTOMER_MASTER c = dbContext.KSTU_CUSTOMER_MASTER.Where(cust => cust.cust_id == id && cust.company_code == companyCode && cust.branch_code == branchCode).FirstOrDefault();
            List<KSTU_CUSTOMER_ID_PROOF_DETAILS> proof = dbContext.KSTU_CUSTOMER_ID_PROOF_DETAILS.Where(cd => cd.cust_id == id && cd.company_code == companyCode && cd.branch_code == branchCode).ToList();

            if (c == null) {
                return NotFound();
            }

            CustomerMasterVM customer = new CustomerMasterVM()
            {
                ObjID = c.obj_id,
                CompanyCode = c.company_code,
                BranchCode = c.branch_code,
                ID = c.cust_id,
                CustName = c.cust_name,
                Address1 = c.address1,
                Address2 = c.address2,
                Address3 = c.address3,
                City = c.city,
                State = c.state,
                Pincode = c.pin_code,
                MobileNo = c.mobile_no,
                PhoneNo = c.phone_no,
                DateOfBirth = c.dob,
                WeddingDate = c.wedding_date,
                CustomerType = c.customer_type,
                ObjectStatus = c.object_status,
                SpouseName = c.spouse_name,
                ChildName1 = c.child_name1,
                ChildName2 = c.child_name2,
                ChildName3 = c.child_name3,
                SpouseDateOfBirth = c.spouse_dob,
                Child1DateOfBirth = c.child1_dob,
                Child2DateOfBirth = c.child2_dob,
                Child3DateOfBirth = c.child3_dob,
                PANNo = c.pan_no,
                IDType = c.id_type,
                IDDetails = c.id_details,
                UpdateOn = c.UpdateOn,
                EmailID = c.Email_ID,
                CreatedDate = c.Created_Date,
                Salutation = c.salutation,
                CountryCode = c.country_code,
                LoyaltyID = c.loyality_id,
                ICNo = c.ic_no,
                PassportNo = c.passport_no,
                PRNo = c.pr_no,
                PrevilegeID = c.privilege_id,
                Age = c.age,
                CountryName = c.Country_name,
                CustCode = c.cust_code,
                CustCreditLimit = c.cust_credit_limit,
                TIN = c.tin,
                StateCode = c.state_code,
                //UniqRowID = c.UniqRowID,
                CorporateID = c.Corparate_ID,
                CorporateBranchID = c.Corporate_Branch_ID,
                EmployeeID = c.Employee_Id,
                RegisteredMN = c.Registered_MN,
                ProfessionID = c.profession_ID,
                EmpCorpEmailID = c.Empcorp_Email_ID,
                ImageIDPath = c.imageid_path,
                CorpImageIDPath = c.corp_imageid_path,
                ImageIDPath2 = c.imageid_path2,
                AccHolderName = c.AccHolderName,
                Accsalutation = c.Accsalutation,
                RepoCustId = c.RepoCustId,
                UpdatedDate = c.UpdatedDate
            };
            List<CustomerIDProofDetailsVM> lstOfCustDetVM = new List<CustomerIDProofDetailsVM>();
            foreach (KSTU_CUSTOMER_ID_PROOF_DETAILS cipd in proof) {
                CustomerIDProofDetailsVM det = new CustomerIDProofDetailsVM();
                det.objID = cipd.obj_id;
                det.CompanyCode = cipd.company_code;
                det.BranchCode = cipd.branch_code;
                det.CustID = cipd.cust_id;
                det.SlNo = cipd.Sl_no;
                det.DocCode = cipd.Doc_code;
                det.DocName = cipd.Doc_name;
                det.DocNo = cipd.Doc_No.ToUpper();
                det.DocImage = cipd.Doc_Image;
                det.RepoCustId = cipd.RepoCustId;
                det.UpdatedDate = cipd.UpdatedDate;
                det.RepDocID = cipd.RepDocID;
                det.DocImagePath = cipd.Doc_Image_Path;
                lstOfCustDetVM.Add(det);
            }
            customer.lstOfProofs = lstOfCustDetVM;
            return Ok(customer);
        }

        /// <summary>
        /// Pick Customer from Searched Criteria
        /// </summary>
        /// <param name="custID"></param>
        /// <param name="mobileNo"></param>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("customer/PickCustomer/{custID}/{mobileNo}/{companyCode}/{branchCode}")]
        public IHttpActionResult GetCustomer(int custID, string mobileNo, string companyCode, string branchCode)
        {
            //CustomerMasterVM customer = PickValidCustomerDetails(custID, mobileNo, companyCode, branchCode);
            //return Ok(customer);
            KSTU_CUSTOMER_MASTER c = dbContext.KSTU_CUSTOMER_MASTER.Where(cust => cust.cust_id == custID && cust.company_code == companyCode && cust.branch_code == branchCode).FirstOrDefault();
            List<KSTU_CUSTOMER_ID_PROOF_DETAILS> proof = dbContext.KSTU_CUSTOMER_ID_PROOF_DETAILS.Where(cd => cd.cust_id == custID && cd.company_code == companyCode && cd.branch_code == branchCode).ToList();

            if (c == null) {
                return NotFound();
            }

            CustomerMasterVM customer = new CustomerMasterVM()
            {
                ObjID = c.obj_id,
                CompanyCode = c.company_code,
                BranchCode = c.branch_code,
                ID = c.cust_id,
                CustName = c.cust_name,
                Address1 = c.address1,
                Address2 = c.address2,
                Address3 = c.address3,
                City = c.city,
                State = c.state,
                Pincode = c.pin_code,
                MobileNo = c.mobile_no,
                PhoneNo = c.phone_no,
                DateOfBirth = c.dob,
                WeddingDate = c.wedding_date,
                CustomerType = c.customer_type,
                ObjectStatus = c.object_status,
                SpouseName = c.spouse_name,
                ChildName1 = c.child_name1,
                ChildName2 = c.child_name2,
                ChildName3 = c.child_name3,
                SpouseDateOfBirth = c.spouse_dob,
                Child1DateOfBirth = c.child1_dob,
                Child2DateOfBirth = c.child2_dob,
                Child3DateOfBirth = c.child3_dob,
                PANNo = c.pan_no,
                IDType = c.id_type,
                IDDetails = c.id_details,
                UpdateOn = c.UpdateOn,
                EmailID = c.Email_ID,
                CreatedDate = c.Created_Date,
                Salutation = c.salutation,
                CountryCode = c.country_code,
                LoyaltyID = c.loyality_id,
                ICNo = c.ic_no,
                PassportNo = c.passport_no,
                PRNo = c.pr_no,
                PrevilegeID = c.privilege_id,
                Age = c.age,
                CountryName = c.Country_name,
                CustCode = c.cust_code,
                CustCreditLimit = c.cust_credit_limit,
                TIN = c.tin,
                StateCode = c.state_code,
                CorporateID = c.Corparate_ID,
                CorporateBranchID = c.Corporate_Branch_ID,
                EmployeeID = c.Employee_Id,
                RegisteredMN = c.Registered_MN,
                ProfessionID = c.profession_ID,
                EmpCorpEmailID = c.Empcorp_Email_ID,
                ImageIDPath = c.imageid_path,
                CorpImageIDPath = c.corp_imageid_path,
                ImageIDPath2 = c.imageid_path2,
                AccHolderName = c.AccHolderName,
                Accsalutation = c.Accsalutation,
                RepoCustId = c.RepoCustId,
                UpdatedDate = c.UpdatedDate
            };
            List<CustomerIDProofDetailsVM> lstOfCustDetVM = new List<CustomerIDProofDetailsVM>();
            foreach (KSTU_CUSTOMER_ID_PROOF_DETAILS cipd in proof) {
                CustomerIDProofDetailsVM det = new CustomerIDProofDetailsVM();
                det.objID = cipd.obj_id;
                det.CompanyCode = cipd.company_code;
                det.BranchCode = cipd.branch_code;
                det.CustID = cipd.cust_id;
                det.SlNo = cipd.Sl_no;
                det.DocCode = cipd.Doc_code;
                det.DocName = cipd.Doc_name;
                det.DocNo = cipd.Doc_No.ToUpper();
                det.DocImage = cipd.Doc_Image;
                det.RepoCustId = cipd.RepoCustId;
                det.UpdatedDate = cipd.UpdatedDate;
                det.RepDocID = cipd.RepDocID;
                det.DocImagePath = cipd.Doc_Image_Path;
                lstOfCustDetVM.Add(det);
            }
            customer.lstOfProofs = lstOfCustDetVM;
            return Ok(customer);
        }

        /// <summary>
        /// Get Customer details by Customer MobileNo.
        /// </summary>
        /// <param name="mobileNo"></param>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("customer/Get/{MobileNo}/{companyCode}/{branchCode}")]
        [ResponseType(typeof(CustomerMasterVM))]
        public IHttpActionResult GetByMobileNo(string mobileNo, string companyCode, string branchCode)
        {
            //Customer misCustomer = dbContext.Customers.Where(misCust => misCust.MobileNo == mobileNo).FirstOrDefault();
            //List<CustomerDocument> lstOfmisDoc = new List<CustomerDocument>();
            //if (misCustomer != null) {
            //    CustomerMasterVM cust = new CustomerMasterVM();
            //    cust.ObjID = misCustomer.ID.ToString();
            //    cust.CompanyCode = misCustomer.CompanyCode;
            //    cust.BranchCode = misCustomer.BranchCode;
            //    cust.ID = misCustomer.ID;
            //    cust.CustName = misCustomer.Name;
            //    cust.Address1 = misCustomer.Address1;
            //    cust.Address2 = misCustomer.Address2;
            //    cust.Address3 = misCustomer.Address3;
            //    cust.City = misCustomer.City;
            //    cust.State = misCustomer.State;
            //    cust.Pincode = misCustomer.PinCode;
            //    cust.MobileNo = misCustomer.MobileNo;
            //    cust.PhoneNo = misCustomer.PhoneNo;
            //    cust.DateOfBirth = misCustomer.DateOfBirth;
            //    cust.WeddingDate = misCustomer.WeddingAnnivarsaryDate;
            //    cust.CustomerType = misCustomer.CustomerType;
            //    cust.ObjectStatus = misCustomer.ObjectStatus;
            //    cust.SpouseName = "";
            //    cust.ChildName1 = "";
            //    cust.ChildName2 = "";
            //    cust.ChildName3 = "";
            //    cust.SpouseDateOfBirth = null;
            //    cust.Child1DateOfBirth = null;
            //    cust.Child2DateOfBirth = null;
            //    cust.Child3DateOfBirth = null;
            //    cust.PANNo = "";
            //    cust.IDType = "";
            //    cust.IDDetails = "";
            //    cust.UpdateOn = misCustomer.UpdatedDate;
            //    cust.EmailID = misCustomer.EmailID;
            //    cust.CreatedDate = misCustomer.CreatedDate;
            //    cust.Salutation = misCustomer.Salutation;
            //    cust.CountryCode = misCustomer.CountryCode;
            //    cust.LoyaltyID = misCustomer.LocalityID;
            //    cust.ICNo = "";
            //    cust.PassportNo = "";
            //    cust.PRNo = "";
            //    cust.PrevilegeID = misCustomer.PrivilegeID;
            //    cust.Age = misCustomer.Age;
            //    cust.CountryName = misCustomer.CountryName;
            //    cust.CustCode = misCustomer.CustCode;
            //    cust.CustCreditLimit = null;
            //    cust.TIN = misCustomer.GSTTIN;
            //    cust.StateCode = misCustomer.StateCode;
            //    cust.CorporateID = "";
            //    cust.CorporateBranchID = "";
            //    cust.EmployeeID = "";
            //    cust.RegisteredMN = "";
            //    cust.ProfessionID = "";
            //    cust.EmpCorpEmailID = "";
            //    cust.ImageIDPath = "";
            //    cust.CorpImageIDPath = "";
            //    cust.ImageIDPath2 = "";
            //    cust.AccHolderName = "";
            //    cust.Accsalutation = "";
            //    cust.RepoCustId = null;
            //    cust.UpdatedDate = misCustomer.UpdatedDate;
            //    lstOfmisDoc = dbContext.CustomerDocuments.Where(custDoc => custDoc.CustID == misCustomer.CustID).ToList();
            //    List<CustomerIDProofDetailsVM> lstOfMISCustDetVM = new List<CustomerIDProofDetailsVM>();
            //    int slno = 1;
            //    foreach (CustomerDocument cipd in lstOfmisDoc) {
            //        CustomerIDProofDetailsVM det = new CustomerIDProofDetailsVM();
            //        det.objID = cipd.ID.ToString();
            //        det.CompanyCode = "";
            //        det.BranchCode = "";
            //        det.CustID = cipd.CustID;
            //        det.SlNo = slno;
            //        det.DocCode = cipd.Type;
            //        det.DocName = cipd.Name;
            //        det.DocNo = cipd.Name;
            //        det.DocImage = null;
            //        det.RepoCustId = null;
            //        det.UpdatedDate = null;
            //        det.RepDocID = null;
            //        det.DocImagePath = cipd.ImagePath;
            //        lstOfMISCustDetVM.Add(det);
            //        slno = slno + 1;
            //    }
            //    cust.lstOfProofs = lstOfMISCustDetVM;
            //    return Ok(cust);
            //}

            KSTU_CUSTOMER_MASTER c = dbContext.KSTU_CUSTOMER_MASTER.Where(cust => cust.mobile_no == mobileNo && cust.company_code == companyCode 
                && cust.branch_code == branchCode).OrderByDescending(x => x.UpdateOn).FirstOrDefault();
            if (c == null) {
                return NotFound();
            }
            List<KSTU_CUSTOMER_ID_PROOF_DETAILS> proof = dbContext.KSTU_CUSTOMER_ID_PROOF_DETAILS.Where(cd => cd.cust_id == c.cust_id && cd.company_code == companyCode && cd.branch_code == branchCode).ToList();
            CustomerMasterVM customer = new CustomerMasterVM()
            {
                ObjID = c.obj_id,
                CompanyCode = c.company_code,
                BranchCode = c.branch_code,
                ID = c.cust_id,
                CustName = c.cust_name,
                Address1 = c.address1,
                Address2 = c.address2,
                Address3 = c.address3,
                City = c.city,
                State = c.state,
                Pincode = c.pin_code,
                MobileNo = c.mobile_no,
                PhoneNo = c.phone_no,
                DateOfBirth = c.dob,
                WeddingDate = c.wedding_date,
                CustomerType = c.customer_type,
                ObjectStatus = c.object_status,
                SpouseName = c.spouse_name,
                ChildName1 = c.child_name1,
                ChildName2 = c.child_name2,
                ChildName3 = c.child_name3,
                SpouseDateOfBirth = c.spouse_dob,
                Child1DateOfBirth = c.child1_dob,
                Child2DateOfBirth = c.child2_dob,
                Child3DateOfBirth = c.child3_dob,
                PANNo = c.pan_no,
                IDType = c.id_type,
                IDDetails = c.id_details,
                UpdateOn = c.UpdateOn,
                EmailID = c.Email_ID,
                CreatedDate = c.Created_Date,
                Salutation = c.salutation,
                CountryCode = c.country_code,
                LoyaltyID = c.loyality_id,
                ICNo = c.ic_no,
                PassportNo = c.passport_no,
                PRNo = c.pr_no,
                PrevilegeID = c.privilege_id,
                Age = c.age,
                CountryName = c.Country_name,
                CustCode = c.cust_code,
                CustCreditLimit = c.cust_credit_limit,
                TIN = c.tin,
                StateCode = c.state_code,
                //UniqRowID = c.UniqRowID,
                CorporateID = c.Corparate_ID,
                CorporateBranchID = c.Corporate_Branch_ID,
                EmployeeID = c.Employee_Id,
                RegisteredMN = c.Registered_MN,
                ProfessionID = c.profession_ID,
                EmpCorpEmailID = c.Empcorp_Email_ID,
                ImageIDPath = c.imageid_path,
                CorpImageIDPath = c.corp_imageid_path,
                ImageIDPath2 = c.imageid_path2,
                AccHolderName = c.AccHolderName,
                Accsalutation = c.Accsalutation,
                RepoCustId = c.RepoCustId,
                UpdatedDate = c.UpdatedDate
            };
            List<CustomerIDProofDetailsVM> lstOfCustDetVM = new List<CustomerIDProofDetailsVM>();
            foreach (KSTU_CUSTOMER_ID_PROOF_DETAILS cipd in proof) {
                CustomerIDProofDetailsVM det = new CustomerIDProofDetailsVM();
                det.objID = cipd.obj_id;
                det.CompanyCode = cipd.company_code;
                det.BranchCode = cipd.branch_code;
                det.CustID = cipd.cust_id;
                det.SlNo = cipd.Sl_no;
                det.DocCode = cipd.Doc_code;
                det.DocName = cipd.Doc_name;
                det.DocNo = cipd.Doc_No.ToUpper();
                det.DocImage = cipd.Doc_Image;
                det.RepoCustId = cipd.RepoCustId;
                det.UpdatedDate = cipd.UpdatedDate;
                det.RepDocID = cipd.RepDocID;
                det.DocImagePath = cipd.Doc_Image_Path;
                lstOfCustDetVM.Add(det);
            }
            customer.lstOfProofs = lstOfCustDetVM;
            return Ok(customer);
        }

        /// <summary>
        /// Get List of First 1000 Customers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("customer/list/{companyCode}/{branchCode}")]
        public IQueryable<CustomerMasterVM> List()
        {
            return dbContext.KSTU_CUSTOMER_MASTER.Select(c => new CustomerMasterVM
            {
                ObjID = c.obj_id,
                CompanyCode = c.company_code,
                BranchCode = c.branch_code,
                ID = c.cust_id,
                CustName = c.cust_name,
                Address1 = c.address1,
                Address2 = c.address2,
                Address3 = c.address3,
                City = c.city,
                State = c.state,
                Pincode = c.pin_code,
                MobileNo = c.mobile_no,
                PhoneNo = c.phone_no,
                DateOfBirth = c.dob,
                WeddingDate = c.wedding_date,
                CustomerType = c.customer_type,
                ObjectStatus = c.object_status,
                SpouseName = c.spouse_name,
                ChildName1 = c.child_name1,
                ChildName2 = c.child_name2,
                ChildName3 = c.child_name3,
                SpouseDateOfBirth = c.spouse_dob,
                Child1DateOfBirth = c.child1_dob,
                Child2DateOfBirth = c.child2_dob,
                Child3DateOfBirth = c.child3_dob,
                PANNo = c.pan_no,
                IDType = c.id_type,
                IDDetails = c.id_details,
                UpdateOn = c.UpdateOn,
                EmailID = c.Email_ID,
                CreatedDate = c.Created_Date,
                Salutation = c.salutation,
                CountryCode = c.country_code,
                LoyaltyID = c.loyality_id,
                ICNo = c.ic_no,
                PassportNo = c.passport_no,
                PRNo = c.pr_no,
                PrevilegeID = c.privilege_id,
                Age = c.age,
                CountryName = c.Country_name,
                CustCode = c.cust_code,
                CustCreditLimit = c.cust_credit_limit,
                TIN = c.tin,
                StateCode = c.state_code,
                CorporateID = c.Corparate_ID,
                CorporateBranchID = c.Corporate_Branch_ID,
                EmployeeID = c.Employee_Id,
                RegisteredMN = c.Registered_MN,
                ProfessionID = c.profession_ID,
                EmpCorpEmailID = c.Empcorp_Email_ID,
                ImageIDPath = c.imageid_path,
                CorpImageIDPath = c.corp_imageid_path,
                ImageIDPath2 = c.imageid_path2,
                AccHolderName = c.AccHolderName,
                Accsalutation = c.Accsalutation,
                RepoCustId = c.RepoCustId,
                UpdatedDate = c.UpdatedDate
            }).Take(1000).OrderByDescending(c => c.UpdateOn);
        }

        /// <summary>
        /// Search customer information by search parameters.
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("customer/searchByParam")]
        [ResponseType(typeof(CustomerMasterVM))]
        public IQueryable<CustomerMasterVM> searchByParam(SearchParams search)
        {
            //var key = ConfigurationManager.AppSettings["UseConsolidatedCustomers"];
            //bool UseConsolidatedCustomers = Convert.ToInt32(key) == 0 ? false : true;
            //if (UseConsolidatedCustomers) {
            //    List<Customer> customers = new List<Customer>();
            //    List<CustomerMasterVM> lstOfCustomers = new List<CustomerMasterVM>();
            //    switch (search.type.ToUpper()) {
            //        case "NAME":
            //            customers = dbContext.Customers.Where(c => c.Name.Contains(search.searchParam)).Take(1000).OrderByDescending(c => c.UpdatedDate).ToList();
            //            break;
            //        case "MOBILENO":
            //            customers = dbContext.Customers.Where(c => c.MobileNo.Contains(search.searchParam)).Take(1000).OrderByDescending(c => c.UpdatedDate).ToList();
            //            break;
            //        case "EMAILID":
            //            customers = dbContext.Customers.Where(c => c.EmailID.Contains(search.searchParam)).Take(1000).OrderByDescending(c => c.UpdatedDate).ToList();
            //            break;
            //        default:
            //            customers = null;
            //            break;
            //    }
            //    foreach (Customer cust in customers) {
            //        CustomerMasterVM customer = new CustomerMasterVM();
            //        customer.ObjID = cust.ID.ToString();
            //        customer.CompanyCode = cust.CompanyCode;
            //        customer.BranchCode = cust.BranchCode;
            //        customer.ID = cust.ID;
            //        customer.CustName = cust.Name;
            //        customer.Address1 = cust.Address1;
            //        customer.Address2 = cust.Address2;
            //        customer.Address3 = cust.Address3;
            //        customer.City = cust.City;
            //        customer.State = cust.State;
            //        customer.Pincode = cust.PinCode;
            //        customer.MobileNo = cust.MobileNo;
            //        customer.PhoneNo = cust.PhoneNo;
            //        customer.DateOfBirth = cust.DateOfBirth;
            //        customer.WeddingDate = cust.WeddingAnnivarsaryDate;
            //        customer.CustomerType = cust.CustomerType;
            //        customer.ObjectStatus = cust.ObjectStatus;
            //        customer.SpouseName = "";
            //        customer.ChildName1 = "";
            //        customer.ChildName2 = "";
            //        customer.ChildName3 = "";
            //        customer.SpouseDateOfBirth = null;
            //        customer.Child1DateOfBirth = null;
            //        customer.Child2DateOfBirth = null;
            //        customer.Child3DateOfBirth = null;
            //        customer.PANNo = "";
            //        customer.IDType = "";
            //        customer.IDDetails = "";
            //        customer.UpdateOn = cust.UpdatedDate;
            //        customer.EmailID = cust.EmailID;
            //        customer.CreatedDate = cust.CreatedDate;
            //        customer.Salutation = cust.Salutation;
            //        customer.CountryCode = cust.CountryCode;
            //        customer.LoyaltyID = cust.LocalityID;
            //        customer.ICNo = "";
            //        customer.PassportNo = "";
            //        customer.PRNo = "";
            //        customer.PrevilegeID = cust.PrivilegeID;
            //        customer.Age = cust.Age;
            //        customer.CountryName = cust.CountryName;
            //        customer.CustCode = cust.CustCode;
            //        customer.CustCreditLimit = null;
            //        customer.TIN = cust.GSTTIN;
            //        customer.StateCode = cust.StateCode;
            //        customer.CorporateID = "";
            //        customer.CorporateBranchID = "";
            //        customer.EmployeeID = "";
            //        customer.RegisteredMN = "";
            //        customer.ProfessionID = "";
            //        customer.EmpCorpEmailID = "";
            //        customer.ImageIDPath = "";
            //        customer.CorpImageIDPath = "";
            //        customer.ImageIDPath2 = "";
            //        customer.AccHolderName = "";
            //        customer.Accsalutation = "";
            //        customer.RepoCustId = null;
            //        customer.UpdatedDate = cust.UpdatedDate;
            //        lstOfCustomers.Add(customer);
            //    }
            //    return lstOfCustomers.AsQueryable();
            //}
            //else {
            //    List<KSTU_CUSTOMER_MASTER> customer = new List<KSTU_CUSTOMER_MASTER>();
            //    switch (search.type.ToUpper()) {
            //        case "NAME":
            //            customer = dbContext.KSTU_CUSTOMER_MASTER.Where(c => c.cust_name.Contains(search.searchParam) && c.company_code == search.companyCode && c.branch_code == search.branchCode).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
            //            break;
            //        case "MOBILENO":
            //            customer = dbContext.KSTU_CUSTOMER_MASTER.Where(c => c.mobile_no.Contains(search.searchParam) && c.company_code == search.companyCode && c.branch_code == search.branchCode).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
            //            break;
            //        case "EMAILID":
            //            customer = dbContext.KSTU_CUSTOMER_MASTER.Where(c => c.Email_ID.Contains(search.searchParam) && c.company_code == search.companyCode && c.branch_code == search.branchCode).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
            //            break;
            //        case "PAN":
            //            customer = dbContext.KSTU_CUSTOMER_MASTER.Where(c => c.pan_no.Contains(search.searchParam) && c.company_code == search.companyCode && c.branch_code == search.branchCode).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
            //            break;
            //        default:
            //            customer = null;
            //            break;
            //    }
            //    if (customer == null) {
            //        return null;
            //    }
            //    return customer.Select(c => new CustomerMasterVM
            //    {
            //        ObjID = c.obj_id,
            //        CompanyCode = c.company_code,
            //        BranchCode = c.branch_code,
            //        ID = c.cust_id,
            //        CustName = c.cust_name,
            //        Address1 = c.address1,
            //        Address2 = c.address2,
            //        Address3 = c.address3,
            //        City = c.city,
            //        State = c.state,
            //        Pincode = c.pin_code,
            //        MobileNo = c.mobile_no,
            //        PhoneNo = c.phone_no,
            //        DateOfBirth = c.dob,
            //        WeddingDate = c.wedding_date,
            //        CustomerType = c.customer_type,
            //        ObjectStatus = c.object_status,
            //        SpouseName = c.spouse_name,
            //        ChildName1 = c.child_name1,
            //        ChildName2 = c.child_name2,
            //        ChildName3 = c.child_name3,
            //        SpouseDateOfBirth = c.spouse_dob,
            //        Child1DateOfBirth = c.child1_dob,
            //        Child2DateOfBirth = c.child2_dob,
            //        Child3DateOfBirth = c.child3_dob,
            //        PANNo = c.pan_no,
            //        IDType = c.id_type,
            //        IDDetails = c.id_details,
            //        UpdateOn = c.UpdateOn,
            //        EmailID = c.Email_ID,
            //        CreatedDate = c.Created_Date,
            //        Salutation = c.salutation,
            //        CountryCode = c.country_code,
            //        LoyaltyID = c.loyality_id,
            //        ICNo = c.ic_no,
            //        PassportNo = c.passport_no,
            //        PRNo = c.pr_no,
            //        PrevilegeID = c.privilege_id,
            //        Age = c.age,
            //        CountryName = c.Country_name,
            //        CustCode = c.cust_code,
            //        CustCreditLimit = c.cust_credit_limit,
            //        TIN = c.tin,
            //        StateCode = c.state_code,
            //        //UniqRowID = c.UniqRowID,
            //        CorporateID = c.Corparate_ID,
            //        CorporateBranchID = c.Corporate_Branch_ID,
            //        EmployeeID = c.Employee_Id,
            //        RegisteredMN = c.Registered_MN,
            //        ProfessionID = c.profession_ID,
            //        EmpCorpEmailID = c.Empcorp_Email_ID,
            //        ImageIDPath = c.imageid_path,
            //        CorpImageIDPath = c.corp_imageid_path,
            //        ImageIDPath2 = c.imageid_path2,
            //        AccHolderName = c.AccHolderName,
            //        Accsalutation = c.Accsalutation,
            //        RepoCustId = c.RepoCustId,
            //        UpdatedDate = c.UpdatedDate
            //    }).AsQueryable();
            //}


            // The bellow is working but now usable because in magna they written procedure.
            //List<Customer> customers = new List<Customer>();
            //List<CustomerMasterVM> lstOfCustomers = new List<CustomerMasterVM>();
            //switch (search.type.ToUpper()) {
            //    case "NAME":
            //        customers = dbContext.Customers.Where(c => c.Name.Contains(search.searchParam)).Take(1000).OrderByDescending(c => c.UpdatedDate).ToList();
            //        break;
            //    case "MOBILENO":
            //        customers = dbContext.Customers.Where(c => c.MobileNo.Contains(search.searchParam)).Take(1000).OrderByDescending(c => c.UpdatedDate).ToList();
            //        break;
            //    case "EMAILID":
            //        customers = dbContext.Customers.Where(c => c.EmailID.Contains(search.searchParam)).Take(1000).OrderByDescending(c => c.UpdatedDate).ToList();
            //        break;
            //    default:
            //        customers = null;
            //        break;
            //}
            //if (customers.Count > 0) {
            //    foreach (Customer cust in customers) {
            //        CustomerMasterVM customer = new CustomerMasterVM();
            //        customer.ObjID = cust.ID.ToString();
            //        customer.CompanyCode = cust.CompanyCode;
            //        customer.BranchCode = cust.BranchCode;
            //        customer.ID = cust.ID;
            //        customer.CustName = cust.Name;
            //        customer.Address1 = cust.Address1;
            //        customer.Address2 = cust.Address2;
            //        customer.Address3 = cust.Address3;
            //        customer.City = cust.City;
            //        customer.State = cust.State;
            //        customer.Pincode = cust.PinCode;
            //        customer.MobileNo = cust.MobileNo;
            //        customer.PhoneNo = cust.PhoneNo;
            //        customer.DateOfBirth = cust.DateOfBirth;
            //        customer.WeddingDate = cust.WeddingAnnivarsaryDate;
            //        customer.CustomerType = cust.CustomerType;
            //        customer.ObjectStatus = cust.ObjectStatus;
            //        customer.SpouseName = "";
            //        customer.ChildName1 = "";
            //        customer.ChildName2 = "";
            //        customer.ChildName3 = "";
            //        customer.SpouseDateOfBirth = null;
            //        customer.Child1DateOfBirth = null;
            //        customer.Child2DateOfBirth = null;
            //        customer.Child3DateOfBirth = null;
            //        customer.PANNo = "";
            //        customer.IDType = "";
            //        customer.IDDetails = "";
            //        customer.UpdateOn = cust.UpdatedDate;
            //        customer.EmailID = cust.EmailID;
            //        customer.CreatedDate = cust.CreatedDate;
            //        customer.Salutation = cust.Salutation;
            //        customer.CountryCode = cust.CountryCode;
            //        customer.LoyaltyID = cust.LocalityID;
            //        customer.ICNo = "";
            //        customer.PassportNo = "";
            //        customer.PRNo = "";
            //        customer.PrevilegeID = cust.PrivilegeID;
            //        customer.Age = cust.Age;
            //        customer.CountryName = cust.CountryName;
            //        customer.CustCode = cust.CustCode;
            //        customer.CustCreditLimit = null;
            //        customer.TIN = cust.GSTTIN;
            //        customer.StateCode = cust.StateCode;
            //        customer.CorporateID = "";
            //        customer.CorporateBranchID = "";
            //        customer.EmployeeID = "";
            //        customer.RegisteredMN = "";
            //        customer.ProfessionID = "";
            //        customer.EmpCorpEmailID = "";
            //        customer.ImageIDPath = "";
            //        customer.CorpImageIDPath = "";
            //        customer.ImageIDPath2 = "";
            //        customer.AccHolderName = "";
            //        customer.Accsalutation = "";
            //        customer.RepoCustId = null;
            //        customer.UpdatedDate = cust.UpdatedDate;
            //        lstOfCustomers.Add(customer);
            //    }
            //    return lstOfCustomers.AsQueryable();
            //}
            //else {
            //    List<KSTU_CUSTOMER_MASTER> customer = new List<KSTU_CUSTOMER_MASTER>();
            //    switch (search.type.ToUpper()) {
            //        case "NAME":
            //            customer = dbContext.KSTU_CUSTOMER_MASTER.Where(c => c.cust_name.Contains(search.searchParam) && c.company_code == search.companyCode && c.branch_code == search.branchCode).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
            //            break;
            //        case "MOBILENO":
            //            customer = dbContext.KSTU_CUSTOMER_MASTER.Where(c => c.mobile_no.Contains(search.searchParam) && c.company_code == search.companyCode && c.branch_code == search.branchCode).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
            //            break;
            //        case "EMAILID":
            //            customer = dbContext.KSTU_CUSTOMER_MASTER.Where(c => c.Email_ID.Contains(search.searchParam) && c.company_code == search.companyCode && c.branch_code == search.branchCode).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
            //            break;
            //        case "PAN":
            //            customer = dbContext.KSTU_CUSTOMER_MASTER.Where(c => c.pan_no.Contains(search.searchParam) && c.company_code == search.companyCode && c.branch_code == search.branchCode).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
            //            break;
            //        default:
            //            customer = null;
            //            break;
            //    }
            //    if (customer == null) {
            //        return null;
            //    }
            //    return customer.Select(c => new CustomerMasterVM
            //    {
            //        ObjID = c.obj_id,
            //        CompanyCode = c.company_code,
            //        BranchCode = c.branch_code,
            //        ID = c.cust_id,
            //        CustName = c.cust_name,
            //        Address1 = c.address1,
            //        Address2 = c.address2,
            //        Address3 = c.address3,
            //        City = c.city,
            //        State = c.state,
            //        Pincode = c.pin_code,
            //        MobileNo = c.mobile_no,
            //        PhoneNo = c.phone_no,
            //        DateOfBirth = c.dob,
            //        WeddingDate = c.wedding_date,
            //        CustomerType = c.customer_type,
            //        ObjectStatus = c.object_status,
            //        SpouseName = c.spouse_name,
            //        ChildName1 = c.child_name1,
            //        ChildName2 = c.child_name2,
            //        ChildName3 = c.child_name3,
            //        SpouseDateOfBirth = c.spouse_dob,
            //        Child1DateOfBirth = c.child1_dob,
            //        Child2DateOfBirth = c.child2_dob,
            //        Child3DateOfBirth = c.child3_dob,
            //        PANNo = c.pan_no,
            //        IDType = c.id_type,
            //        IDDetails = c.id_details,
            //        UpdateOn = c.UpdateOn,
            //        EmailID = c.Email_ID,
            //        CreatedDate = c.Created_Date,
            //        Salutation = c.salutation,
            //        CountryCode = c.country_code,
            //        LoyaltyID = c.loyality_id,
            //        ICNo = c.ic_no,
            //        PassportNo = c.passport_no,
            //        PRNo = c.pr_no,
            //        PrevilegeID = c.privilege_id,
            //        Age = c.age,
            //        CountryName = c.Country_name,
            //        CustCode = c.cust_code,
            //        CustCreditLimit = c.cust_credit_limit,
            //        TIN = c.tin,
            //        StateCode = c.state_code,
            //        //UniqRowID = c.UniqRowID,
            //        CorporateID = c.Corparate_ID,
            //        CorporateBranchID = c.Corporate_Branch_ID,
            //        EmployeeID = c.Employee_Id,
            //        RegisteredMN = c.Registered_MN,
            //        ProfessionID = c.profession_ID,
            //        EmpCorpEmailID = c.Empcorp_Email_ID,
            //        ImageIDPath = c.imageid_path,
            //        CorpImageIDPath = c.corp_imageid_path,
            //        ImageIDPath2 = c.imageid_path2,
            //        AccHolderName = c.AccHolderName,
            //        Accsalutation = c.Accsalutation,
            //        RepoCustId = c.RepoCustId,
            //        UpdatedDate = c.UpdatedDate
            //    }).AsQueryable();
            //}

            //try {
            //    List<CustomerMasterVM> lstOfCustomers = new List<CustomerMasterVM>();
            //    var data = new List<usp_GetCustomerList1_Result>();
            //    var dataCust = new List<usp_GetCustomerListCustomer_Result>();
            //    switch (search.type.ToUpper()) {
            //        case "NAME":
            //            dataCust = dbContext.usp_GetCustomerListCustomer(search.companyCode, "N", search.searchParam).ToList();
            //            if (dataCust.Count <= 0) {
            //                data = dbContext.usp_GetCustomerList1(search.companyCode, "N", search.searchParam).ToList();
            //            }
            //            break;
            //        case "MOBILENO":
            //            dataCust = dbContext.usp_GetCustomerListCustomer(search.companyCode, "Mb", search.searchParam).ToList();
            //            if (dataCust.Count <= 0) {
            //                data = dbContext.usp_GetCustomerList1(search.companyCode, "Mb", search.searchParam).ToList();
            //            }
            //            break;
            //        case "EMAILID":
            //            dataCust = dbContext.usp_GetCustomerListCustomer(search.companyCode, "EID", search.searchParam).ToList();
            //            if (dataCust.Count <= 0) {
            //                data = dbContext.usp_GetCustomerList1(search.companyCode, "EID", search.searchParam).ToList();
            //            }
            //            break;
            //        case "PANNO":
            //            dataCust = dbContext.usp_GetCustomerListCustomer(search.companyCode, "PAN", search.searchParam).ToList();
            //            if (dataCust.Count <= 0) {
            //                data = dbContext.usp_GetCustomerList1(search.companyCode, "PAN", search.searchParam).ToList();
            //            }
            //            break;
            //    }
            //    if (dataCust.Count > 0) {
            //        foreach (usp_GetCustomerListCustomer_Result cust in dataCust) {
            //            CustomerMasterVM customer = new CustomerMasterVM();
            //            customer.ObjID = cust.ID.ToString();
            //            customer.CompanyCode = search.companyCode;
            //            customer.BranchCode = search.branchCode;
            //            customer.ID = cust.ID;
            //            customer.CustName = cust.Name;
            //            customer.Address1 = cust.Address1;
            //            customer.Address2 = cust.Address2;
            //            customer.Address3 = cust.Address3;
            //            customer.MobileNo = cust.Mobile;
            //            customer.City = cust.City;
            //            customer.State = cust.State;
            //            customer.SpouseName = "";
            //            customer.ChildName1 = "";
            //            customer.ChildName2 = "";
            //            customer.ChildName3 = "";
            //            customer.SpouseDateOfBirth = null;
            //            customer.Child1DateOfBirth = null;
            //            customer.Child2DateOfBirth = null;
            //            customer.Child3DateOfBirth = null;
            //            customer.EmailID = cust.EmailID;
            //            customer.PANNo = "";
            //            customer.IDType = "";
            //            customer.IDDetails = "";
            //            customer.ICNo = "";
            //            customer.PassportNo = "";
            //            customer.PRNo = "";
            //            customer.CorporateID = "";
            //            customer.CorporateBranchID = "";
            //            customer.EmployeeID = "";
            //            customer.RegisteredMN = "";
            //            customer.ProfessionID = "";
            //            customer.EmpCorpEmailID = "";
            //            customer.ImageIDPath = "";
            //            customer.CorpImageIDPath = "";
            //            customer.ImageIDPath2 = "";
            //            customer.AccHolderName = "";
            //            customer.Accsalutation = "";
            //            customer.RepoCustId = null;
            //            KSTS_STATE_MASTER stateMaster = dbContext.KSTS_STATE_MASTER.Where(state => state.state_name == cust.State).FirstOrDefault();
            //            customer.StateCode = stateMaster == null ? 0 : stateMaster.id;
            //            customer.lstOfProofs = new List<CustomerIDProofDetailsVM>();
            //            lstOfCustomers.Add(customer);
            //        }
            //    }
            //    else {
            //        foreach (usp_GetCustomerList1_Result cust in data) {
            //            CustomerMasterVM customer = new CustomerMasterVM();
            //            customer.ObjID = cust.ID.ToString();
            //            customer.CompanyCode = search.companyCode;
            //            customer.BranchCode = search.branchCode;
            //            customer.ID = cust.ID;
            //            customer.CustName = cust.Name;
            //            customer.Address1 = cust.Address1;
            //            customer.Address2 = cust.Address2;
            //            customer.Address3 = cust.Address3;
            //            customer.MobileNo = cust.Mobile;
            //            customer.City = cust.City;
            //            customer.State = cust.State;
            //            customer.SpouseName = "";
            //            customer.ChildName1 = "";
            //            customer.ChildName2 = "";
            //            customer.ChildName3 = "";
            //            customer.SpouseDateOfBirth = null;
            //            customer.Child1DateOfBirth = null;
            //            customer.Child2DateOfBirth = null;
            //            customer.Child3DateOfBirth = null;
            //            customer.EmailID = cust.EmailID;
            //            customer.PANNo = "";
            //            customer.IDType = "";
            //            customer.IDDetails = "";
            //            customer.ICNo = "";
            //            customer.PassportNo = "";
            //            customer.PRNo = "";
            //            customer.CorporateID = "";
            //            customer.CorporateBranchID = "";
            //            customer.EmployeeID = "";
            //            customer.RegisteredMN = "";
            //            customer.ProfessionID = "";
            //            customer.EmpCorpEmailID = "";
            //            customer.ImageIDPath = "";
            //            customer.CorpImageIDPath = "";
            //            customer.ImageIDPath2 = "";
            //            customer.AccHolderName = "";
            //            customer.Accsalutation = "";
            //            customer.RepoCustId = null;
            //            KSTS_STATE_MASTER stateMaster = dbContext.KSTS_STATE_MASTER.Where(state => state.state_name == cust.State).FirstOrDefault();
            //            customer.StateCode = stateMaster == null ? 0 : stateMaster.id;
            //            customer.lstOfProofs = new List<CustomerIDProofDetailsVM>();
            //            lstOfCustomers.Add(customer);
            //        }
            //    }
            //    return lstOfCustomers.AsQueryable();
            //}
            //catch (Exception excp) { throw excp; }


            // The above code is commented and written this new code on 05/03/2021 (Informed by NR Sir)
            List<KSTU_CUSTOMER_MASTER> customer = new List<KSTU_CUSTOMER_MASTER>();
            switch (search.type.ToUpper()) {
                case "NAME":
                    customer = dbContext.KSTU_CUSTOMER_MASTER.Where(c => c.cust_name.Contains(search.searchParam) && c.company_code == search.companyCode && c.branch_code == search.branchCode).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
                    break;
                case "MOBILENO":
                    customer = dbContext.KSTU_CUSTOMER_MASTER.Where(c => c.mobile_no.Contains(search.searchParam) && c.company_code == search.companyCode && c.branch_code == search.branchCode).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
                    break;
                case "EMAILID":
                    customer = dbContext.KSTU_CUSTOMER_MASTER.Where(c => c.Email_ID.Contains(search.searchParam) && c.company_code == search.companyCode && c.branch_code == search.branchCode).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
                    break;
                case "PAN":
                    customer = dbContext.KSTU_CUSTOMER_MASTER.Where(c => c.pan_no.Contains(search.searchParam) && c.company_code == search.companyCode && c.branch_code == search.branchCode).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
                    break;
                default:
                    customer = null;
                    break;
            }
            if (customer == null) {
                return null;
            }
            return customer.Select(c => new CustomerMasterVM
            {
                ObjID = c.obj_id,
                CompanyCode = c.company_code,
                BranchCode = c.branch_code,
                ID = c.cust_id,
                CustName = c.cust_name,
                Address1 = c.address1,
                Address2 = c.address2,
                Address3 = c.address3,
                City = c.city,
                State = c.state,
                Pincode = c.pin_code,
                MobileNo = c.mobile_no,
                PhoneNo = c.phone_no,
                DateOfBirth = c.dob,
                WeddingDate = c.wedding_date,
                CustomerType = c.customer_type,
                ObjectStatus = c.object_status,
                SpouseName = c.spouse_name,
                ChildName1 = c.child_name1,
                ChildName2 = c.child_name2,
                ChildName3 = c.child_name3,
                SpouseDateOfBirth = c.spouse_dob,
                Child1DateOfBirth = c.child1_dob,
                Child2DateOfBirth = c.child2_dob,
                Child3DateOfBirth = c.child3_dob,
                PANNo = c.pan_no,
                IDType = c.id_type,
                IDDetails = c.id_details,
                UpdateOn = c.UpdateOn,
                EmailID = c.Email_ID,
                CreatedDate = c.Created_Date,
                Salutation = c.salutation,
                CountryCode = c.country_code,
                LoyaltyID = c.loyality_id,
                ICNo = c.ic_no,
                PassportNo = c.passport_no,
                PRNo = c.pr_no,
                PrevilegeID = c.privilege_id,
                Age = c.age,
                CountryName = c.Country_name,
                CustCode = c.cust_code,
                CustCreditLimit = c.cust_credit_limit,
                TIN = c.tin,
                StateCode = c.state_code,
                CorporateID = c.Corparate_ID,
                CorporateBranchID = c.Corporate_Branch_ID,
                EmployeeID = c.Employee_Id,
                RegisteredMN = c.Registered_MN,
                ProfessionID = c.profession_ID,
                EmpCorpEmailID = c.Empcorp_Email_ID,
                ImageIDPath = c.imageid_path,
                CorpImageIDPath = c.corp_imageid_path,
                ImageIDPath2 = c.imageid_path2,
                AccHolderName = c.AccHolderName,
                Accsalutation = c.Accsalutation,
                RepoCustId = c.RepoCustId,
                UpdatedDate = c.UpdatedDate
            }).AsQueryable();
        }

        /// <summary>
        /// Get Count of Customers by Search Parameters.
        /// </summary>
        /// <param name="searchParam"></param>
        /// <param name="type"></param>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("customer/searchByParamCount/{searchParam}/{type}/{companyCode}/{branchCode}")]
        [ResponseType(typeof(CustomerMasterVM))]
        public IHttpActionResult searchByParamCount(string searchParam, string type, string companyCode, string branchCode)
        {
            //List<KSTU_CUSTOMER_MASTER> customer = new List<KSTU_CUSTOMER_MASTER>();
            //switch (type.ToUpper()) {
            //    case "NAME":
            //        customer = dbContext.KSTU_CUSTOMER_MASTER.Where(c => c.cust_name.Contains(searchParam)).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
            //        break;
            //    case "MOBILENO":
            //        customer = dbContext.KSTU_CUSTOMER_MASTER.Where(c => c.mobile_no.Contains(searchParam)).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
            //        break;
            //    case "EMAILID":
            //        customer = dbContext.KSTU_CUSTOMER_MASTER.Where(c => c.Email_ID.Contains(searchParam)).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
            //        break;
            //    case "PAN":
            //        customer = dbContext.KSTU_CUSTOMER_MASTER.Where(c => c.pan_no.Contains(searchParam)).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
            //        break;

            //    default:
            //        customer = null;
            //        break;
            //}

            //if (customer == null) {
            //    return Content(HttpStatusCode.NotFound, new ErrorVM() { field = "Customer", index = 0, description = "Invalid search parameters" });
            //}
            //return Ok(new { RecordCount = customer.Count.ToString() });

            //List<CustomerMasterVM> lstOfCustomers = new List<CustomerMasterVM>();
            //var data = new List<usp_GetCustomerList_Result>();
            //switch (type.ToUpper()) {
            //    case "NAME":
            //        data = dbContext.usp_GetCustomerList(companyCode, branchCode, "N", searchParam).ToList();
            //        break;
            //    case "MOBILENO":
            //        data = dbContext.usp_GetCustomerList(companyCode, branchCode, "Mb", searchParam).ToList();
            //        break;
            //    case "EMAILID":
            //        data = dbContext.usp_GetCustomerList(companyCode, branchCode, "EID", searchParam).ToList();
            //        break;
            //    case "PANNO":
            //        data = dbContext.usp_GetCustomerList(companyCode, branchCode, "PAN", searchParam).ToList();
            //        break;
            //}

            //try {
            //    List<CustomerMasterVM> lstOfCustomers = new List<CustomerMasterVM>();
            //    var data = new List<usp_GetCustomerList_Result>();
            //    var dataCust = new List<usp_GetCustomerListCustomer_Result>();
            //    switch (type.ToUpper()) {
            //        case "NAME":
            //            dataCust = dbContext.usp_GetCustomerListCustomer(companyCode, "N", searchParam).ToList();
            //            if (dataCust.Count <= 0) {
            //                data = dbContext.usp_GetCustomerList(companyCode, branchCode, "N", searchParam).ToList();
            //            }
            //            break;
            //        case "MOBILENO":
            //            dataCust = dbContext.usp_GetCustomerListCustomer(companyCode, "Mb", searchParam).ToList();
            //            if (dataCust.Count <= 0) {
            //                data = dbContext.usp_GetCustomerList(companyCode, branchCode, "Mb", searchParam).ToList();
            //            }
            //            break;
            //        case "EMAILID":
            //            dataCust = dbContext.usp_GetCustomerListCustomer(companyCode, "EID", searchParam).ToList();
            //            if (dataCust.Count <= 0) {
            //                data = dbContext.usp_GetCustomerList(companyCode, branchCode, "EID", searchParam).ToList();
            //            }
            //            break;
            //        case "PANNO":
            //            dataCust = dbContext.usp_GetCustomerListCustomer(companyCode, "PAN", searchParam).ToList();
            //            if (dataCust.Count <= 0) {
            //                data = dbContext.usp_GetCustomerList(companyCode, branchCode, "PAN", searchParam).ToList();
            //            }
            //            break;
            //    }
            //    if (dataCust.Count > 0) {
            //        return Ok(dataCust.Count);
            //    }
            //    else {
            //        return Ok(data.Count);
            //    }
            //}
            //catch (Exception excp) { throw excp; }

            try {
                List<KSTU_CUSTOMER_MASTER> customer = new List<KSTU_CUSTOMER_MASTER>();
                switch (type.ToUpper()) {
                    case "NAME":
                        customer = dbContext.KSTU_CUSTOMER_MASTER.Where(c => c.cust_name.Contains(searchParam) 
                                                                    && c.company_code == companyCode 
                                                                    && c.branch_code == branchCode).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
                        break;
                    case "MOBILENO":
                        customer = dbContext.KSTU_CUSTOMER_MASTER.Where(c => c.mobile_no.Contains(searchParam) 
                                                                    && c.company_code == companyCode 
                                                                    && c.branch_code == branchCode).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
                        break;
                    case "EMAILID":
                        customer = dbContext.KSTU_CUSTOMER_MASTER.Where(c => c.Email_ID.Contains(searchParam) 
                                                                    && c.company_code == companyCode 
                                                                    && c.branch_code == branchCode).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
                        break;
                    case "PAN":
                        customer = dbContext.KSTU_CUSTOMER_MASTER.Where(c => c.pan_no.Contains(searchParam) 
                                                                    && c.company_code == companyCode 
                                                                    && c.branch_code == branchCode).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
                        break;
                    default:
                        customer = null;
                        break;
                }
                if (customer.Count > 0) {
                    return Ok(customer.Count);
                }
                else {
                    return Ok(customer.Count);
                }
            }
            catch (Exception excp) { throw excp; }
        }

        /// <summary>
        /// Get customer Count.
        /// </summary>
        /// <param name="oDataOptions"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("customer/count")]
        public IHttpActionResult Count(ODataQueryOptions<CustomerMasterVM> oDataOptions)
        {
            var key = ConfigurationManager.AppSettings["UseConsolidatedCustomers"];
            bool UseConsolidatedCustomers = Convert.ToInt32(key) == 0 ? false : true;
            if (UseConsolidatedCustomers) {
                var query = dbContext.Customers.ToList();
                return Ok(new { RecordCount = query.Count() });
            }
            else {
                var query = dbContext.KSTU_CUSTOMER_MASTER.Select(c => new CustomerMasterVM
                {
                    ObjID = c.obj_id,
                    CompanyCode = c.company_code,
                    BranchCode = c.branch_code,
                    ID = c.cust_id,
                    CustName = c.cust_name,
                    Address1 = c.address1,
                    Address2 = c.address2,
                    Address3 = c.address3,
                    City = c.city,
                    State = c.state,
                    Pincode = c.pin_code,
                    MobileNo = c.mobile_no,
                    PhoneNo = c.phone_no,
                    DateOfBirth = c.dob,
                    WeddingDate = c.wedding_date,
                    CustomerType = c.customer_type,
                    ObjectStatus = c.object_status,
                    SpouseName = c.spouse_name,
                    ChildName1 = c.child_name1,
                    ChildName2 = c.child_name2,
                    ChildName3 = c.child_name3,
                    SpouseDateOfBirth = c.spouse_dob,
                    Child1DateOfBirth = c.child1_dob,
                    Child2DateOfBirth = c.child2_dob,
                    Child3DateOfBirth = c.child3_dob,
                    PANNo = c.pan_no,
                    IDType = c.id_type,
                    IDDetails = c.id_details,
                    UpdateOn = c.UpdateOn,
                    EmailID = c.Email_ID,
                    CreatedDate = c.Created_Date,
                    Salutation = c.salutation,
                    CountryCode = c.country_code,
                    LoyaltyID = c.loyality_id,
                    ICNo = c.ic_no,
                    PassportNo = c.passport_no,
                    PRNo = c.pr_no,
                    PrevilegeID = c.privilege_id,
                    Age = c.age,
                    CountryName = c.Country_name,
                    CustCode = c.cust_code,
                    CustCreditLimit = c.cust_credit_limit,
                    TIN = c.tin,
                    StateCode = c.state_code,
                    //UniqRowID = c.UniqRowID,
                    CorporateID = c.Corparate_ID,
                    CorporateBranchID = c.Corporate_Branch_ID,
                    EmployeeID = c.Employee_Id,
                    RegisteredMN = c.Registered_MN,
                    ProfessionID = c.profession_ID,
                    EmpCorpEmailID = c.Empcorp_Email_ID,
                    ImageIDPath = c.imageid_path,
                    CorpImageIDPath = c.corp_imageid_path,
                    ImageIDPath2 = c.imageid_path2,
                    AccHolderName = c.AccHolderName,
                    Accsalutation = c.Accsalutation,
                    RepoCustId = c.RepoCustId,
                    UpdatedDate = c.UpdatedDate
                });
                return base.GetCount(oDataOptions, query);
            }
        }

        /// <summary>
        /// Save Customer Information.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("customer/post")]
        [ResponseType(typeof(CustomerMasterVM))]
        public IHttpActionResult Post(CustomerMasterVM c)
        {
            //if (!ModelState.IsValid) {
            //    return BadRequest(ModelState);
            //}
            #region Customer Validation
            ErrorVM error = null;
            if (c == null) {
                error = new ErrorVM() { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Customer details cannot be null." };
                return Content(error.ErrorStatusCode, error);
            }
            c.CustName = c.CustName.Trim();
            c.Address1 = c.Address1.Trim();
            c.MobileNo = c.MobileNo.Trim();
            if (!string.IsNullOrEmpty(c.PANNo)) {
                if(c.PANNo.Length != 10) {
                    error = new ErrorVM() { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "PAN No. should be exactly 10 characters." };
                    return Content(error.ErrorStatusCode, error);
                }
            }
            if (SIGlobals.Globals.GetLetterCount(c.CustName) < 3) {
                error = new ErrorVM() { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Customer name should be at least 3 characters." };
                return Content(error.ErrorStatusCode, error);
            }
            if (SIGlobals.Globals.GetLetterCount(c.Address1) < 3) {
                error = new ErrorVM() { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Customer address1 should be at least 3 characters." };
                return Content(error.ErrorStatusCode, error);
            }
            if (SIGlobals.Globals.GetLetterCount(c.MobileNo) < 10) {
                error = new ErrorVM() { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Invalid mobile number. 10 digit mobile number is expected." };
                return Content(error.ErrorStatusCode, error);
            }
            if (c.StateCode == null || c.StateCode == 0) {
                error = new ErrorVM() { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "A value is expected for State Code." };
                return Content(error.ErrorStatusCode, error);
            }
            var stateName = dbContext.KSTS_STATE_MASTER.Where(x => x.tinno == c.StateCode).Select(y => y.state_name).FirstOrDefault();
            if (string.IsNullOrEmpty(stateName)) {
                error = new ErrorVM()
                {
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                    description = "State code " + c.StateCode.ToString() + " is invalid/not found."
                };
                return Content(error.ErrorStatusCode, error);
            }
            c.State = stateName;
            #endregion

            int customerID = dbContext.KSTS_SEQ_NOS.Where(sq => sq.obj_id == ModuleSeqNo && sq.company_code == c.CompanyCode && sq.branch_code == c.BranchCode).First().nextno;
            KSTU_CUSTOMER_MASTER customer = new KSTU_CUSTOMER_MASTER();
            customer.obj_id = SIGlobals.Globals.GetMagnaGUID(TABLE_NAME, customerID, c.CompanyCode, c.BranchCode);
            customer.company_code = c.CompanyCode;
            customer.branch_code = c.BranchCode;
            customer.cust_id = customerID;
            customer.cust_name = c.CustName;
            customer.address1 = c.Address1;
            customer.address2 = c.Address2 == null ? "" : c.Address2;
            customer.address3 = c.Address3 == null ? "" : c.Address3;
            customer.city = c.City == null ? "" : c.City;
            customer.state = c.State == null ? "" : c.State;
            customer.pin_code = c.Pincode == null ? "" : c.Pincode;
            customer.mobile_no = c.MobileNo == null ? "" : c.MobileNo;
            customer.phone_no = c.PhoneNo == null ? "" : c.PhoneNo;
            customer.dob = c.DateOfBirth == null ? SIGlobals.Globals.GetApplicationDate(c.CompanyCode, c.BranchCode) : c.DateOfBirth;
            customer.wedding_date = c.WeddingDate == null ? SIGlobals.Globals.GetApplicationDate(c.CompanyCode, c.BranchCode) : c.WeddingDate;
            customer.customer_type = "W";
            customer.object_status = c.ObjectStatus == null ? "O" : c.ObjectStatus;
            customer.spouse_name = c.SpouseName;
            customer.child_name1 = c.ChildName1;
            customer.child_name2 = c.ChildName2;
            customer.child_name3 = c.ChildName3;
            customer.spouse_dob = c.SpouseDateOfBirth == null ? SIGlobals.Globals.GetApplicationDate(c.CompanyCode, c.BranchCode) : c.SpouseDateOfBirth;
            customer.child1_dob = c.Child1DateOfBirth == null ? SIGlobals.Globals.GetApplicationDate(c.CompanyCode, c.BranchCode) : c.Child1DateOfBirth;
            customer.child2_dob = c.Child2DateOfBirth == null ? SIGlobals.Globals.GetApplicationDate(c.CompanyCode, c.BranchCode) : c.Child2DateOfBirth;
            customer.child3_dob = c.Child3DateOfBirth == null ? SIGlobals.Globals.GetApplicationDate(c.CompanyCode, c.BranchCode) : c.Child3DateOfBirth;
            customer.pan_no = c.PANNo == null ? "" : c.PANNo; ;
            customer.id_type = c.IDType;
            customer.id_details = c.IDDetails == null ? "" : c.IDDetails;
            customer.UpdateOn = SIGlobals.Globals.GetDateTime();
            customer.Email_ID = c.EmailID == null ? "" : c.EmailID;
            customer.Created_Date = SIGlobals.Globals.GetApplicationDate(c.CompanyCode, c.BranchCode);
            customer.salutation = c.Salutation == null ? "" : c.Salutation;
            customer.country_code = c.CountryCode == null ? "" : c.CountryCode;
            customer.loyality_id = c.LoyaltyID == null ? "" : c.LoyaltyID;
            customer.ic_no = c.ICNo == null ? "" : c.ICNo;
            customer.passport_no = c.PassportNo == null ? "" : c.PassportNo;
            customer.pr_no = c.PRNo == null ? "" : c.PRNo;
            customer.privilege_id = c.PrevilegeID == null ? "" : c.PrevilegeID;
            customer.age = c.Age == null ? 0 : c.Age;
            customer.Country_name = c.CountryName == null ? "" : c.CountryName;
            customer.cust_code = c.CustCode == null ? "" : c.CustCode;
            customer.cust_credit_limit = c.CustCreditLimit == null ? 0 : c.CustCreditLimit;
            customer.tin = c.TIN == null ? "" : c.TIN;
            customer.state_code = c.StateCode;
            customer.UniqRowID = Guid.NewGuid();
            customer.Corparate_ID = c.CorporateID;
            customer.Corporate_Branch_ID = c.CorporateBranchID;
            customer.Employee_Id = c.EmployeeID;
            customer.Registered_MN = c.RegisteredMN;
            customer.profession_ID = c.ProfessionID;
            customer.Empcorp_Email_ID = c.EmpCorpEmailID;
            customer.imageid_path = c.ImageIDPath;
            customer.corp_imageid_path = c.CorpImageIDPath;
            customer.imageid_path2 = c.ImageIDPath2;
            customer.AccHolderName = c.AccHolderName;
            customer.Accsalutation = c.Accsalutation;
            customer.RepoCustId = c.RepoCustId == null ? 0 : c.RepoCustId;
            customer.UpdatedDate = Framework.Common.GetDateTime();
            dbContext.KSTU_CUSTOMER_MASTER.Add(customer);

            if (c.lstOfProofs != null) {
                int slno = 1;
                foreach (CustomerIDProofDetailsVM proof in c.lstOfProofs) {
                    KSTU_CUSTOMER_ID_PROOF_DETAILS IDProof = new KSTU_CUSTOMER_ID_PROOF_DETAILS();
                    IDProof.obj_id = customer.obj_id;
                    IDProof.company_code = proof.CompanyCode;
                    IDProof.branch_code = proof.BranchCode;
                    IDProof.cust_id = customer.cust_id;
                    IDProof.Sl_no = slno;
                    IDProof.Doc_code = proof.DocCode;
                    if (proof.DocName == null) {
                        IDProof.Doc_name = "";
                    }
                    else if (proof.DocName.Length > 30) {
                        IDProof.Doc_name = proof.DocName.Substring(1, 25).ToUpper();
                    }
                    else {
                        IDProof.Doc_name = proof.DocName.ToUpper();
                    }
                    IDProof.Doc_No = proof.DocNo;
                    IDProof.Doc_Image = proof.DocImage;
                    IDProof.RepoCustId = proof.RepoCustId == null ? 0 : proof.RepoCustId;
                    IDProof.UpdatedDate = SIGlobals.Globals.GetDateTime();
                    IDProof.RepDocID = proof.RepDocID == null ? 0 : proof.RepDocID;
                    IDProof.Doc_Image_Path = proof.DocImagePath;
                    dbContext.KSTU_CUSTOMER_ID_PROOF_DETAILS.Add(IDProof);
                    slno = slno + 1;
                }
            }
            try {
                Framework.Common.UpdateSeqenceNumber(dbContext, c.CompanyCode, c.BranchCode, ModuleSeqNo);
                dbContext.SaveChanges();
                c.ID = customer.cust_id;
                return CreatedAtRoute("DefaultApi", new
                {
                    id = customer.cust_id
                }, c);

                //var cust = Get(customer.cust_id);
                //return Content(HttpStatusCode.Accepted, cust);
            }
            catch (Exception excp) {
                return Content(HttpStatusCode.InternalServerError, excp.Message);
            }
        }

        /// <summary>
        /// Update Customer information by Customer ID and Updated customer object.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("customer/put")]
        [ResponseType(typeof(CustomerMasterVM))]
        public IHttpActionResult Put(int id, CustomerMasterVM c)
        {
            //Customer misCustomer = dbContext.Customers.Where(cust => cust.ID == id).FirstOrDefault();
            //Customer misCustomerWithMobile = dbContext.Customers.Where(cust => cust.MobileNo == c.MobileNo).FirstOrDefault();

            //KSTU_CUSTOMER_MASTER customer = new KSTU_CUSTOMER_MASTER();
            //if (misCustomer != null || misCustomerWithMobile != null) {// Customer Exist in mis DB.
            //    customer = dbContext.KSTU_CUSTOMER_MASTER.Where(cust => cust.mobile_no == c.MobileNo && cust.company_code == c.CompanyCode && cust.branch_code == c.BranchCode).FirstOrDefault();
            //    if (customer != null) {
            //        #region Upadate Customer
            //        customer.cust_name = c.CustName;
            //        customer.address1 = c.Address1;
            //        customer.address2 = c.Address2;
            //        customer.address3 = c.Address3;
            //        customer.city = c.City;
            //        customer.state = c.State;
            //        customer.pin_code = c.Pincode;
            //        customer.mobile_no = c.MobileNo;
            //        customer.phone_no = c.PhoneNo;
            //        customer.dob = c.DateOfBirth;
            //        customer.wedding_date = c.WeddingDate;
            //        customer.customer_type = c.CustomerType;
            //        customer.object_status = c.ObjectStatus;
            //        customer.spouse_name = c.SpouseName;
            //        customer.child_name1 = c.ChildName1;
            //        customer.child_name2 = c.ChildName2;
            //        customer.child_name3 = c.ChildName3;
            //        customer.spouse_dob = c.SpouseDateOfBirth;
            //        customer.child1_dob = c.Child1DateOfBirth;
            //        customer.child2_dob = c.Child2DateOfBirth;
            //        customer.child3_dob = c.Child3DateOfBirth;
            //        customer.pan_no = c.PANNo;
            //        customer.id_type = c.IDType;
            //        customer.id_details = c.IDDetails;
            //        customer.UpdateOn = c.UpdateOn;
            //        customer.Email_ID = c.EmailID;
            //        customer.Created_Date = c.CreatedDate;
            //        customer.salutation = c.Salutation;
            //        customer.country_code = c.CountryCode;
            //        customer.loyality_id = c.LoyaltyID;
            //        customer.ic_no = c.ICNo;
            //        customer.passport_no = c.PassportNo;
            //        customer.pr_no = c.PRNo;
            //        customer.privilege_id = c.PrevilegeID;
            //        customer.age = c.Age;
            //        customer.Country_name = c.CountryName;
            //        customer.cust_code = c.CustCode;
            //        customer.cust_credit_limit = c.CustCreditLimit;
            //        customer.tin = c.TIN;
            //        customer.state_code = c.StateCode;
            //        customer.Corparate_ID = c.CorporateID;
            //        customer.Corporate_Branch_ID = c.CorporateBranchID;
            //        customer.Employee_Id = c.EmployeeID;
            //        customer.Registered_MN = c.RegisteredMN;
            //        customer.profession_ID = c.ProfessionID;
            //        customer.Empcorp_Email_ID = c.EmpCorpEmailID;
            //        customer.imageid_path = c.ImageIDPath;
            //        customer.corp_imageid_path = c.CorpImageIDPath;
            //        customer.imageid_path2 = c.ImageIDPath2;
            //        customer.AccHolderName = c.AccHolderName;
            //        customer.Accsalutation = c.Accsalutation;
            //        customer.RepoCustId = misCustomerWithMobile == null ? misCustomer.CustID : misCustomerWithMobile.ID;
            //        customer.UpdatedDate = Framework.Common.GetDateTime();

            //        List<KSTU_CUSTOMER_ID_PROOF_DETAILS> lstOfDet = dbContext.KSTU_CUSTOMER_ID_PROOF_DETAILS.Where(cust => cust.cust_id == c.ID && cust.company_code == c.CompanyCode && cust.branch_code == c.BranchCode).ToList();
            //        if (lstOfDet.Count > 0) {
            //            dbContext.KSTU_CUSTOMER_ID_PROOF_DETAILS.RemoveRange(lstOfDet);
            //        }

            //        if (c.lstOfProofs != null) {
            //            foreach (CustomerIDProofDetailsVM proof in c.lstOfProofs) {
            //                KSTU_CUSTOMER_ID_PROOF_DETAILS IDProof = new KSTU_CUSTOMER_ID_PROOF_DETAILS();
            //                IDProof.obj_id = customer.obj_id;
            //                IDProof.company_code = proof.CompanyCode;
            //                IDProof.branch_code = proof.BranchCode;
            //                IDProof.cust_id = customer.cust_id;
            //                IDProof.Sl_no = proof.SlNo;
            //                IDProof.Doc_code = proof.DocCode;
            //                IDProof.Doc_name = proof.DocName;
            //                IDProof.Doc_No = proof.DocNo;
            //                IDProof.Doc_Image = proof.DocImage;
            //                IDProof.RepoCustId = proof.RepoCustId;
            //                IDProof.UpdatedDate = SIGlobals.Globals.GetDateTime();
            //                IDProof.RepDocID = proof.RepDocID;
            //                IDProof.Doc_Image_Path = proof.DocImagePath;
            //                dbContext.KSTU_CUSTOMER_ID_PROOF_DETAILS.Add(IDProof);
            //            }
            //        }
            //        dbContext.Entry(customer).State = System.Data.Entity.EntityState.Modified;
            //        try {
            //            dbContext.SaveChanges();
            //        }
            //        catch (Exception ex) {
            //            throw ex;
            //        }
            //        #endregion
            //        CustomerMasterVM retObject = GetLocalCustomerByCustomerID(customer.cust_id, customer.company_code, customer.branch_code);
            //        return Content(HttpStatusCode.Accepted, retObject);
            //    }
            //    else {// incase customer Not there. So Create new customer
            //        #region Create New Customer
            //        ErrorVM error = new ErrorVM();
            //        CustomerMasterVM newCsutomer = new CustomerBL().Post(c, out error);
            //        if (error != null) {
            //            return Content(HttpStatusCode.Accepted, newCsutomer);
            //        }
            //        else {
            //            return Content(HttpStatusCode.Accepted, error);
            //        }
            //        #endregion
            //    }
            //}
            //return null;
            #region Customer Validation
            ErrorVM error = null;
            if (c == null) {
                error = new ErrorVM() { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Customer details cannot be null." };
                return Content(error.ErrorStatusCode, error);
            }
            c.CustName = c.CustName.Trim();
            c.Address1 = c.Address1.Trim();
            c.MobileNo = c.MobileNo.Trim();
            if (SIGlobals.Globals.GetLetterCount(c.CustName) < 3) {
                error = new ErrorVM() { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Customer name should be at least 3 characters." };
                return Content(error.ErrorStatusCode, error);
            }
            if (SIGlobals.Globals.GetLetterCount(c.Address1) < 3) {
                error = new ErrorVM() { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Customer address1 should be at least 3 characters." };
                return Content(error.ErrorStatusCode, error);
            }
            if (SIGlobals.Globals.GetLetterCount(c.MobileNo) < 10) {
                error = new ErrorVM() { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Invalid mobile number. 10 digit mobile number is expected." };
                return Content(error.ErrorStatusCode, error);
            }
            if (c.StateCode == null || c.StateCode == 0) {
                error = new ErrorVM() { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "A value is expected for State Code." };
                return Content(error.ErrorStatusCode, error);
            }
            var stateName = dbContext.KSTS_STATE_MASTER.Where(x => x.tinno == c.StateCode).Select(y => y.state_name).FirstOrDefault();
            if (string.IsNullOrEmpty(stateName)) {
                error = new ErrorVM() { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                    description = "State code " + c.StateCode.ToString() + " is invalid/not found." };
                return Content(error.ErrorStatusCode, error);
            }
            c.State = stateName;
            #endregion

            KSTU_CUSTOMER_MASTER customer = new KSTU_CUSTOMER_MASTER();
            customer = dbContext.KSTU_CUSTOMER_MASTER.Where(cust => cust.cust_id == id
                                                        && cust.company_code == c.CompanyCode
                                                        && cust.branch_code == c.BranchCode).FirstOrDefault();
            if (customer != null) {
                #region Upadate Customer
                customer.cust_name = c.CustName;
                customer.address1 = c.Address1;
                customer.address2 = c.Address2;
                customer.address3 = c.Address3;
                customer.city = c.City;
                customer.state = c.State;
                customer.pin_code = c.Pincode;
                customer.mobile_no = c.MobileNo;
                customer.phone_no = c.PhoneNo;
                customer.dob = c.DateOfBirth;
                customer.wedding_date = c.WeddingDate;
                customer.customer_type = c.CustomerType;
                customer.object_status = c.ObjectStatus;
                customer.spouse_name = c.SpouseName;
                customer.child_name1 = c.ChildName1;
                customer.child_name2 = c.ChildName2;
                customer.child_name3 = c.ChildName3;
                customer.spouse_dob = c.SpouseDateOfBirth;
                customer.child1_dob = c.Child1DateOfBirth;
                customer.child2_dob = c.Child2DateOfBirth;
                customer.child3_dob = c.Child3DateOfBirth;
                customer.pan_no = c.PANNo;
                customer.id_type = c.IDType;
                customer.id_details = c.IDDetails;
                customer.UpdateOn = c.UpdateOn;
                customer.Email_ID = c.EmailID;
                customer.Created_Date = c.CreatedDate;
                customer.salutation = c.Salutation;
                customer.country_code = c.CountryCode;
                customer.loyality_id = c.LoyaltyID;
                customer.ic_no = c.ICNo;
                customer.passport_no = c.PassportNo;
                customer.pr_no = c.PRNo;
                customer.privilege_id = c.PrevilegeID;
                customer.age = c.Age;
                customer.Country_name = c.CountryName;
                customer.cust_code = c.CustCode;
                customer.cust_credit_limit = c.CustCreditLimit;
                customer.tin = c.TIN;
                customer.state_code = c.StateCode;
                customer.Corparate_ID = c.CorporateID;
                customer.Corporate_Branch_ID = c.CorporateBranchID;
                customer.Employee_Id = c.EmployeeID;
                customer.Registered_MN = c.RegisteredMN;
                customer.profession_ID = c.ProfessionID;
                customer.Empcorp_Email_ID = c.EmpCorpEmailID;
                customer.imageid_path = c.ImageIDPath;
                customer.corp_imageid_path = c.CorpImageIDPath;
                customer.imageid_path2 = c.ImageIDPath2;
                customer.AccHolderName = c.AccHolderName;
                customer.Accsalutation = c.Accsalutation;
                customer.UpdatedDate = Framework.Common.GetDateTime();

                List<KSTU_CUSTOMER_ID_PROOF_DETAILS> lstOfDet = dbContext.KSTU_CUSTOMER_ID_PROOF_DETAILS.Where(cust => cust.cust_id == c.ID && cust.company_code == c.CompanyCode && cust.branch_code == c.BranchCode).ToList();
                if (lstOfDet.Count > 0) {
                    dbContext.KSTU_CUSTOMER_ID_PROOF_DETAILS.RemoveRange(lstOfDet);
                }

                if (c.lstOfProofs != null) {
                    int slno = 1;
                    foreach (CustomerIDProofDetailsVM proof in c.lstOfProofs) {
                        KSTU_CUSTOMER_ID_PROOF_DETAILS IDProof = new KSTU_CUSTOMER_ID_PROOF_DETAILS();
                        IDProof.obj_id = customer.obj_id;
                        IDProof.company_code = proof.CompanyCode;
                        IDProof.branch_code = proof.BranchCode;
                        IDProof.cust_id = customer.cust_id;
                        IDProof.Sl_no = slno;
                        IDProof.Doc_code = proof.DocCode;
                        if (proof.DocName == null) {
                            IDProof.Doc_name = "";
                        }
                        else if (proof.DocName.Length > 30) {
                            IDProof.Doc_name = proof.DocName.Substring(1, 25);
                        }
                        else {
                            IDProof.Doc_name = proof.DocName;
                        }
                        IDProof.Doc_No = proof.DocNo;
                        IDProof.Doc_Image = proof.DocImage;
                        IDProof.RepoCustId = proof.RepoCustId == null ? 0 : proof.RepoCustId;
                        IDProof.UpdatedDate = SIGlobals.Globals.GetDateTime();
                        IDProof.RepDocID = proof.RepDocID == null ? 0 : proof.RepDocID;
                        IDProof.Doc_Image_Path = proof.DocImagePath;
                        dbContext.KSTU_CUSTOMER_ID_PROOF_DETAILS.Add(IDProof);
                        slno = slno + 1;
                    }
                }
                dbContext.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                try {
                    dbContext.SaveChanges();
                    CustomerMasterVM retObject = GetLocalCustomerByCustomerID(customer.cust_id, customer.company_code, customer.branch_code);
                    return Content(HttpStatusCode.Accepted, retObject);
                }
                catch (Exception ex) {
                    return Content(HttpStatusCode.InternalServerError, ex);
                }
                #endregion
            }
            return null;
        }

        /// <summary>
        /// Provides Uploading Fiels.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("UploadDoc")]
        public async Task<IHttpActionResult> UplodaDocumnet()
        {
            List<string> savedFilePath = new List<string>();
            // Check if the request contains multipart/form-data
            if (!Request.Content.IsMimeMultipartContent()) {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            //Get the path of folder where we want to upload all files.
            string rootPath = HttpContext.Current.Server.MapPath("~/Documents");
            var provider = new MultipartFileStreamProvider(rootPath);
            // Read the form data.
            //If any error(Cancelled or any fault) occurred during file read , return internal server error
            var task = Request.Content.ReadAsMultipartAsync(provider).
                ContinueWith<HttpResponseMessage>(t =>
                {
                    if (t.IsCanceled || t.IsFaulted) {
                        Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
                    }
                    foreach (MultipartFileData dataitem in provider.FileData) {
                        try {
                            //Replace / from file name
                            string name = dataitem.Headers.ContentDisposition.FileName.Replace("\"", "");
                            //Create New file name using GUID to prevent duplicate file name
                            string newFileName = Guid.NewGuid() + Path.GetExtension(name);
                            //Move file from current location to target folder.
                            File.Move(dataitem.LocalFileName, Path.Combine(rootPath, newFileName));
                        }
                        catch (Exception ex) {
                            string message = ex.Message;
                        }
                    }
                    return Request.CreateResponse(HttpStatusCode.Created, savedFilePath);
                });
            return Ok(task);
        }

        /// <summary>
        /// Provides Uploading Fiels.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("UploadDoc2")]
        public async Task<IHttpActionResult> UplodaDocumnet2()
        {
            string destFolder = ConfigurationManager.AppSettings["DocFolder"].ToString();
            try {
                string filePath = "";
                string name = string.Empty;
                string newFileName = string.Empty;
                List<string> savedFilePath = new List<string>();
                // Check if the request contains multipart/form-data
                if (!Request.Content.IsMimeMultipartContent()) {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }
                //Get the path of folder where we want to upload all files.
                string rootPath = HttpContext.Current.Server.MapPath("~/" + destFolder);
                var provider = new MultipartFileStreamProvider(rootPath);
                await Request.Content.ReadAsMultipartAsync(provider);
                // Read the form data.
                foreach (MultipartFileData dataitem in provider.FileData) {
                    try {
                        //Replace / from file name
                        name = dataitem.Headers.ContentDisposition.FileName.Replace("\"", "");
                        //Create New file name using GUID to prevent duplicate file name
                        newFileName = Guid.NewGuid() + Path.GetExtension(name);
                        //Move file from current location to target folder.
                        File.Move(dataitem.LocalFileName, Path.Combine(rootPath, newFileName));
                        //File path returning to the URL
                        filePath = HttpContext.Current.Request.Url.OriginalString.Replace(HttpContext.Current.Request.Url.LocalPath, "") + HttpContext.Current.Request.ApplicationPath + "/" + destFolder + "/" + newFileName;
                    }
                    catch (Exception ex) {
                        string message = ex.Message;
                        return Content(HttpStatusCode.InternalServerError, ex.Message);
                    }
                }
                return Ok(new { Path = filePath });
            }
            catch (Exception excp) {
                return Content(HttpStatusCode.InternalServerError, excp.Message);
            }
        }

        [HttpPost]
        [Route("UploadDoc3")]
        public IHttpActionResult UploadImage3()
        {
            try {
                string originalFilePath = string.Empty;
                string imageName = null;
                var httpRequest = HttpContext.Current.Request;
                //Upload Image
                var postedFile = httpRequest.Files[0];
                //Create custom filename
                if (postedFile != null) {
                    imageName = new String(Path.GetFileNameWithoutExtension(postedFile.FileName).Take(10).ToArray()).Replace(" ", "-");
                    imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(postedFile.FileName);
                    var filePath = HttpContext.Current.Server.MapPath("~/Documents/" + imageName);
                    postedFile.SaveAs(filePath);
                    originalFilePath = HttpContext.Current.Request.Url.OriginalString.Replace(HttpContext.Current.Request.Url.LocalPath, "") + HttpContext.Current.Request.ApplicationPath + "/Documents/" + imageName;
                }
                return Ok(originalFilePath);
            }
            catch (Exception excp) {
                return Content(HttpStatusCode.InternalServerError, excp.Message);
            }
        }

        public IHttpActionResult Delete([FromBody] int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public KSTU_CUSTOMER_MASTER GetActualCustomerDetails(int customerID, string MobileNo, string companyCode, string branchCode)
        {
            Customer customers = new Customer();
            List<CustomerDocument> customerDoc = new List<CustomerDocument>();

            // Checking the customer exist in the mis.Customer Table
            customers = dbContext.Customers.Where(cust => cust.CustID == customerID && cust.MobileNo == MobileNo).FirstOrDefault();
            customerDoc = dbContext.CustomerDocuments.Where(cust => cust.CustID == customerID).ToList();

            // Checking Customer Exist in the Local Master Table by using Mobile Number and Branch.
            KSTU_CUSTOMER_MASTER custMaster = dbContext.KSTU_CUSTOMER_MASTER.Where(cust => cust.mobile_no == MobileNo && cust.branch_code == branchCode).FirstOrDefault();

            // If Exists Send That customer Information.
            if (custMaster != null) {
                return custMaster;
            }
            else { // Need to check this block.
                ErrorVM error = new ErrorVM();
                CustomerMasterVM cust = new CustomerMasterVM();
                List<CustomerDocument> lstOfmisDoc = new List<CustomerDocument>();
                if (customers != null) {
                    cust.ObjID = customers.ID.ToString();
                    cust.CompanyCode = companyCode;
                    cust.BranchCode = branchCode;
                    cust.ID = customers.ID;
                    cust.CustName = customers.Name;
                    cust.Address1 = customers.Address1;
                    cust.Address2 = customers.Address2;
                    cust.Address3 = customers.Address3;
                    cust.City = customers.City;
                    cust.State = customers.State;
                    cust.Pincode = customers.PinCode;
                    cust.MobileNo = customers.MobileNo;
                    cust.PhoneNo = customers.PhoneNo;
                    cust.DateOfBirth = customers.DateOfBirth;
                    cust.WeddingDate = customers.WeddingAnnivarsaryDate;
                    cust.CustomerType = customers.CustomerType;
                    cust.ObjectStatus = customers.ObjectStatus;
                    cust.SpouseName = "";
                    cust.ChildName1 = "";
                    cust.ChildName2 = "";
                    cust.ChildName3 = "";
                    cust.SpouseDateOfBirth = null;
                    cust.Child1DateOfBirth = null;
                    cust.Child2DateOfBirth = null;
                    cust.Child3DateOfBirth = null;
                    cust.PANNo = "";
                    cust.IDType = "";
                    cust.IDDetails = "";
                    cust.UpdateOn = customers.UpdatedDate;
                    cust.EmailID = customers.EmailID;
                    cust.CreatedDate = customers.CreatedDate;
                    cust.Salutation = customers.Salutation;
                    cust.CountryCode = customers.CountryCode;
                    cust.LoyaltyID = customers.LocalityID;
                    cust.ICNo = "";
                    cust.PassportNo = "";
                    cust.PRNo = "";
                    cust.PrevilegeID = customers.PrivilegeID;
                    cust.Age = customers.Age;
                    cust.CountryName = customers.CountryName;
                    cust.CustCode = customers.CustCode;
                    cust.CustCreditLimit = null;
                    cust.TIN = customers.GSTTIN;
                    cust.StateCode = customers.StateCode;
                    cust.CorporateID = "";
                    cust.CorporateBranchID = "";
                    cust.EmployeeID = "";
                    cust.RegisteredMN = "";
                    cust.ProfessionID = "";
                    cust.EmpCorpEmailID = "";
                    cust.ImageIDPath = "";
                    cust.CorpImageIDPath = "";
                    cust.ImageIDPath2 = "";
                    cust.AccHolderName = "";
                    cust.Accsalutation = "";
                    cust.RepoCustId = customers.ID;
                    cust.UpdatedDate = customers.UpdatedDate;
                    lstOfmisDoc = dbContext.CustomerDocuments.Where(custDoc => custDoc.CustID == customers.CustID).ToList();
                    List<CustomerIDProofDetailsVM> lstOfMISCustDetVM = new List<CustomerIDProofDetailsVM>();
                    int slno = 1;
                    foreach (CustomerDocument cipd in lstOfmisDoc) {
                        CustomerIDProofDetailsVM det = new CustomerIDProofDetailsVM();
                        det.objID = cipd.ID.ToString();
                        det.CompanyCode = "";
                        det.BranchCode = "";
                        det.CustID = cipd.CustID;
                        det.SlNo = slno;
                        det.DocCode = cipd.Type;
                        det.DocName = cipd.Name;
                        det.DocNo = cipd.Name;
                        det.DocImage = null;
                        det.RepoCustId = null;
                        det.UpdatedDate = null;
                        det.RepDocID = null;
                        det.DocImagePath = cipd.ImagePath;
                        lstOfMISCustDetVM.Add(det);
                        slno = slno + 1;
                    }
                    cust.lstOfProofs = lstOfMISCustDetVM;
                }
                CustomerMasterVM customer = new CustomerBL().Post(cust, out error);
                KSTU_CUSTOMER_MASTER actualCustomerDet = dbContext.KSTU_CUSTOMER_MASTER.Where(c => c.cust_id == customer.ID && c.company_code == customer.CompanyCode && c.branch_code == customer.BranchCode).FirstOrDefault();
                return actualCustomerDet;
            }
            #endregion
        }

        public CustomerMasterVM GetLocalCustomerByCustomerID(int customerID, string companyCode, string branchCode)
        {
            KSTU_CUSTOMER_MASTER c = dbContext.KSTU_CUSTOMER_MASTER.Where(cust => cust.cust_id == customerID && cust.company_code == companyCode && cust.branch_code == branchCode).FirstOrDefault();
            List<KSTU_CUSTOMER_ID_PROOF_DETAILS> proof = dbContext.KSTU_CUSTOMER_ID_PROOF_DETAILS.Where(cd => cd.cust_id == customerID && cd.company_code == companyCode && cd.branch_code == branchCode).ToList();
            if (c == null) {
                return null;
            }
            CustomerMasterVM customer = new CustomerMasterVM()
            {
                ObjID = c.obj_id,
                CompanyCode = c.company_code,
                BranchCode = c.branch_code,
                ID = c.cust_id,
                CustName = c.cust_name,
                Address1 = c.address1,
                Address2 = c.address2,
                Address3 = c.address3,
                City = c.city,
                State = c.state,
                Pincode = c.pin_code,
                MobileNo = c.mobile_no,
                PhoneNo = c.phone_no,
                DateOfBirth = c.dob,
                WeddingDate = c.wedding_date,
                CustomerType = c.customer_type,
                ObjectStatus = c.object_status,
                SpouseName = c.spouse_name,
                ChildName1 = c.child_name1,
                ChildName2 = c.child_name2,
                ChildName3 = c.child_name3,
                SpouseDateOfBirth = c.spouse_dob,
                Child1DateOfBirth = c.child1_dob,
                Child2DateOfBirth = c.child2_dob,
                Child3DateOfBirth = c.child3_dob,
                PANNo = c.pan_no,
                IDType = c.id_type,
                IDDetails = c.id_details,
                UpdateOn = c.UpdateOn,
                EmailID = c.Email_ID,
                CreatedDate = c.Created_Date,
                Salutation = c.salutation,
                CountryCode = c.country_code,
                LoyaltyID = c.loyality_id,
                ICNo = c.ic_no,
                PassportNo = c.passport_no,
                PRNo = c.pr_no,
                PrevilegeID = c.privilege_id,
                Age = c.age,
                CountryName = c.Country_name,
                CustCode = c.cust_code,
                CustCreditLimit = c.cust_credit_limit,
                TIN = c.tin,
                StateCode = c.state_code,
                CorporateID = c.Corparate_ID,
                CorporateBranchID = c.Corporate_Branch_ID,
                EmployeeID = c.Employee_Id,
                RegisteredMN = c.Registered_MN,
                ProfessionID = c.profession_ID,
                EmpCorpEmailID = c.Empcorp_Email_ID,
                ImageIDPath = c.imageid_path,
                CorpImageIDPath = c.corp_imageid_path,
                ImageIDPath2 = c.imageid_path2,
                AccHolderName = c.AccHolderName,
                Accsalutation = c.Accsalutation,
                RepoCustId = c.RepoCustId,
                UpdatedDate = c.UpdatedDate
            };
            List<CustomerIDProofDetailsVM> lstOfCustDetVM = new List<CustomerIDProofDetailsVM>();
            foreach (KSTU_CUSTOMER_ID_PROOF_DETAILS cipd in proof) {
                CustomerIDProofDetailsVM det = new CustomerIDProofDetailsVM();
                det.objID = cipd.obj_id;
                det.CompanyCode = cipd.company_code;
                det.BranchCode = cipd.branch_code;
                det.CustID = cipd.cust_id;
                det.SlNo = cipd.Sl_no;
                det.DocCode = cipd.Doc_code;
                det.DocName = cipd.Doc_name;
                det.DocNo = cipd.Doc_No.ToUpper();
                det.DocImage = cipd.Doc_Image;
                det.RepoCustId = cipd.RepoCustId;
                det.UpdatedDate = cipd.UpdatedDate;
                det.RepDocID = cipd.RepDocID;
                det.DocImagePath = cipd.Doc_Image_Path;
                lstOfCustDetVM.Add(det);
            }
            customer.lstOfProofs = lstOfCustDetVM;
            return customer;
        }

        public CustomerMasterVM PickValidCustomerDetails(int customerID, string mobileNumber, string companyCode, string branchCode)
        {
            List<usp_GetCustDetailsValid_Result> result = dbContext.usp_GetCustDetailsValid(mobileNumber, companyCode, branchCode, Convert.ToString(customerID), "1").ToList();
            if (result == null) {
                return null;
            }
            CustomerMasterVM customer = new CustomerMasterVM();
            customer.ObjID = result[0].RepoCustId.ToString();
            customer.CompanyCode = companyCode;
            customer.BranchCode = branchCode;
            customer.ID = result[0].RepoCustId;
            customer.CustName = result[0].cust_name;
            customer.Address1 = result[0].address1;
            customer.Address2 = result[0].address2;
            customer.Address3 = result[0].address3;
            customer.MobileNo = result[0].mobile_no;
            customer.City = result[0].city;
            customer.State = result[0].state;
            customer.SpouseName = "";
            customer.ChildName1 = "";
            customer.ChildName2 = "";
            customer.ChildName3 = "";
            customer.SpouseDateOfBirth = null;
            customer.Child1DateOfBirth = null;
            customer.Child2DateOfBirth = null;
            customer.Child3DateOfBirth = null;
            customer.EmailID = result[0].Email_ID;
            customer.PANNo = "";
            customer.IDType = "";
            customer.IDDetails = "";
            customer.ICNo = "";
            customer.PassportNo = "";
            customer.PRNo = "";
            customer.CorporateID = "";
            customer.CorporateBranchID = "";
            customer.EmployeeID = "";
            customer.RegisteredMN = "";
            customer.ProfessionID = "";
            customer.EmpCorpEmailID = "";
            customer.ImageIDPath = "";
            customer.CorpImageIDPath = "";
            customer.ImageIDPath2 = "";
            customer.AccHolderName = "";
            customer.Accsalutation = "";
            customer.RepoCustId = null;
            string stateName = result[0].state;
            KSTS_STATE_MASTER stateMaster = dbContext.KSTS_STATE_MASTER.Where(state => state.state_name == stateName).FirstOrDefault();
            customer.StateCode = stateMaster == null ? 0 : stateMaster.id;
            customer.lstOfProofs = new List<CustomerIDProofDetailsVM>();
            return customer;
        }
    }
}
