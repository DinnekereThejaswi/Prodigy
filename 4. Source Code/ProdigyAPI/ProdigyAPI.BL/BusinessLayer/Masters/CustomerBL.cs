using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.BL.ViewModel.Payment;
using ProdigyAPI.BL.ViewModel.Repair;
using ProdigyAPI.BL.ViewModel.Sales;
using ProdigyAPI.Model.MagnaDb;
using ProdigyAPI.SIGlobals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Core.Objects;
using ProdigyAPI.BL.ViewModel.Master;

namespace ProdigyAPI.BL.BusinessLayer.Masters
{
    public class CustomerBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        private const string MODULE_SEQ_NO = "12";
        #endregion

        #region Methods
        public List<KSTU_CUSTOMER_MASTER> GetAllCustomerDetails()
        {
            var cust = db.KSTU_CUSTOMER_MASTER.Where(m => m.customer_type == "R").ToList();
            return cust;
        }

        public CustomerMasterVM Get(int id, out ErrorVM error)
        {
            // Bellow Line of code throwing an Error.
            //"Message": "Unhandled exception. The number of primary key values passed must match number 
            // of primary key values defined on the entity.\r\nParameter name: keyValues"

            error = null;
            KSTU_CUSTOMER_MASTER c = db.KSTU_CUSTOMER_MASTER.Where(cust => cust.cust_id == id).FirstOrDefault();
            if (c == null) {
                error.field = "Customer";
                error.index = 0;
                error.description = "Invalid Customer Number";
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
            return customer;
        }

        public IQueryable<CustomerMasterVM> List()
        {
            return db.KSTU_CUSTOMER_MASTER.Select(c => new CustomerMasterVM
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

        public IQueryable<CustomerMasterVM> searchByParam(SearchParams search)
        {
            List<KSTU_CUSTOMER_MASTER> customer = new List<KSTU_CUSTOMER_MASTER>();
            switch (search.type.ToUpper()) {
                case "NAME":
                    customer = db.KSTU_CUSTOMER_MASTER.Where(c => c.cust_name.Contains(search.searchParam)).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
                    break;
                case "MOBILENO":
                    customer = db.KSTU_CUSTOMER_MASTER.Where(c => c.mobile_no.Contains(search.searchParam)).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
                    break;
                case "EMAILID":
                    customer = db.KSTU_CUSTOMER_MASTER.Where(c => c.Email_ID.Contains(search.searchParam)).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
                    break;
                case "PAN":
                    customer = db.KSTU_CUSTOMER_MASTER.Where(c => c.pan_no.Contains(search.searchParam)).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
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
            }).AsQueryable();
        }

        public string SearchByParamCount(string searchParam, string type, out ErrorVM error)
        {
            error = null;
            List<KSTU_CUSTOMER_MASTER> customer = new List<KSTU_CUSTOMER_MASTER>();
            switch (type.ToUpper()) {
                case "NAME":
                    customer = db.KSTU_CUSTOMER_MASTER.Where(c => c.cust_name.Contains(searchParam)).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
                    break;
                case "MOBILENO":
                    customer = db.KSTU_CUSTOMER_MASTER.Where(c => c.mobile_no.Contains(searchParam)).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
                    break;
                case "EMAILID":
                    customer = db.KSTU_CUSTOMER_MASTER.Where(c => c.Email_ID.Contains(searchParam)).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
                    break;
                case "PAN":
                    customer = db.KSTU_CUSTOMER_MASTER.Where(c => c.pan_no.Contains(searchParam)).Take(1000).OrderByDescending(c => c.UpdateOn).ToList();
                    break;

                default:
                    customer = null;
                    break;
            }

            if (customer == null) {
                error = new ErrorVM() { field = "Customer", index = 0, description = "Invalid search parameters" };
            }
            return customer.Count.ToString();
        }

        public int Count()
        {
            var query = db.KSTU_CUSTOMER_MASTER.Select(c => new CustomerMasterVM
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
            return query.Count();
        }

        public CustomerMasterVM Post(CustomerMasterVM c, out ErrorVM error)
        {
            error = null;
            if (c == null) {
                error = new ErrorVM() { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Customer details cannot be null." };
                return null;
            }
            c.CustName = c.CustName.Trim();
            c.Address1 = c.Address1.Trim();
            c.MobileNo = c.MobileNo.Trim();
            if (SIGlobals.Globals.GetLetterCount(c.CustName) < 3) {
                error = new ErrorVM() { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Customer name should be at least 3 characters." };
                return null;
            }
            if (SIGlobals.Globals.GetLetterCount(c.Address1) < 3) {
                error = new ErrorVM() { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Customer address1 should be at least 3 characters." };
                return null;
            }

            if (SIGlobals.Globals.GetLetterCount(c.MobileNo) < 10) {
                error = new ErrorVM() { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Invalid mobile number. 10 digit mobile number is expected." };
                return null;
            }

            KSTU_CUSTOMER_MASTER customer = new KSTU_CUSTOMER_MASTER();
            customer.obj_id = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
            customer.company_code = c.CompanyCode;
            customer.branch_code = c.BranchCode;
            customer.cust_id = db.KSTS_SEQ_NOS.Where(sq => sq.obj_id == MODULE_SEQ_NO && sq.company_code == c.CompanyCode && sq.branch_code == c.BranchCode).First().nextno;
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
            customer.customer_type = "W";
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
            customer.Created_Date = SIGlobals.Globals.GetApplicationDate(c.CompanyCode, c.BranchCode);
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
            customer.RepoCustId = c.RepoCustId;
            customer.UpdatedDate = SIGlobals.Globals.GetDateTime();
            db.KSTU_CUSTOMER_MASTER.Add(customer);

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
                    IDProof.Doc_name = proof.DocName;
                    IDProof.Doc_No = proof.DocNo;
                    IDProof.Doc_Image = proof.DocImage;
                    IDProof.RepoCustId = proof.RepoCustId == null ? 0 : proof.RepoCustId;
                    IDProof.UpdatedDate = SIGlobals.Globals.GetDateTime();
                    IDProof.RepDocID = proof.RepDocID == null ? 0 : proof.RepDocID;
                    IDProof.Doc_Image_Path = proof.DocImagePath;
                    db.KSTU_CUSTOMER_ID_PROOF_DETAILS.Add(IDProof);
                    slno = slno + 1;
                }
            }
            try {
                SIGlobals.Globals.UpdateSeqenceNumber(db, MODULE_SEQ_NO, c.CompanyCode, c.BranchCode);
                db.SaveChanges();
                c.ID = customer.cust_id;
                return c;
            }
            catch (Exception excp) {
                error = new ErrorVM() { description = excp.Message, ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError };
                return null;
            }
        }

        public KSTU_CUSTOMER_MASTER Put(int id, CustomerMasterVM c, out ErrorVM error)
        {
            error = null;
            if (c == null) {
                error = new ErrorVM() { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Customer details cannot be null." };
                return null;
            }
            c.CustName = c.CustName.Trim();
            c.Address1 = c.Address1.Trim();
            c.MobileNo = c.MobileNo.Trim();
            if (SIGlobals.Globals.GetLetterCount(c.CustName) < 3) {
                error = new ErrorVM() { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Customer name should be at least 3 characters." };
                return null;
            }
            if (SIGlobals.Globals.GetLetterCount(c.Address1) < 3) {
                error = new ErrorVM() { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Customer address1 should be at least 3 characters." };
                return null;
            }
            if (SIGlobals.Globals.GetLetterCount(c.MobileNo) < 10) {
                error = new ErrorVM() { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Invalid mobile number. 10 digit mobile number is expected." };
                return null;
            }

            KSTU_CUSTOMER_MASTER customer = db.KSTU_CUSTOMER_MASTER.Where(cust => cust.cust_id == id && cust.company_code == c.CompanyCode && cust.branch_code == c.BranchCode).FirstOrDefault();
            if (customer == null) {
                error = new ErrorVM() { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Invalid Customer ID" };
                return null;
            }

            customer.cust_id = c.ID;
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
            customer.RepoCustId = c.RepoCustId;
            customer.UpdatedDate = SIGlobals.Globals.GetDateTime();

            db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
            try {
                db.SaveChanges();
            }
            catch (Exception ex) {
                throw ex;
            }
            return customer;
        }

        public KSTU_CUSTOMER_MASTER GetActualCustomerDetails(int customerID, string MobileNo, string companyCode, string branchCode)
        {
            //Customer customers = new Customer();
            //List<CustomerDocument> customerDoc = new List<CustomerDocument>();

            //// Checking the customer exist in the mis.Customer Table
            //customers = db.Customers.Where(cust => cust.ID == customerID).FirstOrDefault();
            //if (customers == null) {
            //    customers = db.Customers.Where(cust => cust.MobileNo == MobileNo).FirstOrDefault();
            //}
            //customerDoc = db.CustomerDocuments.Where(cust => cust.CustID == customerID).ToList();

            //// Checking Customer Exist in the Local Master Table by using Mobile Number and Branch.
            //KSTU_CUSTOMER_MASTER custMaster = db.KSTU_CUSTOMER_MASTER.Where(cust => cust.mobile_no == MobileNo && cust.branch_code == branchCode).FirstOrDefault();

            //// If Exists Send That customer Information.
            //if (custMaster != null) {
            //    return custMaster;
            //}
            //else { // Need to check this block.
            //    ErrorVM error = new ErrorVM();
            //    CustomerMasterVM cust = new CustomerMasterVM();
            //    List<CustomerDocument> lstOfmisDoc = new List<CustomerDocument>();
            //    if (customers != null) {
            //        cust.ObjID = customers.ID.ToString();
            //        cust.CompanyCode = companyCode;
            //        cust.BranchCode = branchCode;
            //        cust.ID = customers.ID;
            //        cust.CustName = customers.Name;
            //        cust.Address1 = customers.Address1;
            //        cust.Address2 = customers.Address2;
            //        cust.Address3 = customers.Address3;
            //        cust.City = customers.City;
            //        cust.State = customers.State;
            //        cust.Pincode = customers.PinCode;
            //        cust.MobileNo = customers.MobileNo;
            //        cust.PhoneNo = customers.PhoneNo;
            //        cust.DateOfBirth = customers.DateOfBirth;
            //        cust.WeddingDate = customers.WeddingAnnivarsaryDate;
            //        cust.CustomerType = customers.CustomerType;
            //        cust.ObjectStatus = customers.ObjectStatus;
            //        cust.SpouseName = "";
            //        cust.ChildName1 = "";
            //        cust.ChildName2 = "";
            //        cust.ChildName3 = "";
            //        cust.SpouseDateOfBirth = null;
            //        cust.Child1DateOfBirth = null;
            //        cust.Child2DateOfBirth = null;
            //        cust.Child3DateOfBirth = null;
            //        cust.PANNo = "";
            //        cust.IDType = "";
            //        cust.IDDetails = "";
            //        cust.UpdateOn = customers.UpdatedDate;
            //        cust.EmailID = customers.EmailID;
            //        cust.CreatedDate = customers.CreatedDate;
            //        cust.Salutation = customers.Salutation;
            //        cust.CountryCode = customers.CountryCode;
            //        cust.LoyaltyID = customers.LocalityID;
            //        cust.ICNo = "";
            //        cust.PassportNo = "";
            //        cust.PRNo = "";
            //        cust.PrevilegeID = customers.PrivilegeID;
            //        cust.Age = customers.Age;
            //        cust.CountryName = customers.CountryName;
            //        cust.CustCode = customers.CustCode;
            //        cust.CustCreditLimit = null;
            //        cust.TIN = customers.GSTTIN;
            //        cust.StateCode = customers.StateCode;
            //        cust.CorporateID = "";
            //        cust.CorporateBranchID = "";
            //        cust.EmployeeID = "";
            //        cust.RegisteredMN = "";
            //        cust.ProfessionID = "";
            //        cust.EmpCorpEmailID = "";
            //        cust.ImageIDPath = "";
            //        cust.CorpImageIDPath = "";
            //        cust.ImageIDPath2 = "";
            //        cust.AccHolderName = "";
            //        cust.Accsalutation = "";
            //        cust.RepoCustId = customers.ID;
            //        cust.UpdatedDate = customers.UpdatedDate;
            //        lstOfmisDoc = db.CustomerDocuments.Where(custDoc => custDoc.CustID == customers.CustID).ToList();
            //        List<CustomerIDProofDetailsVM> lstOfMISCustDetVM = new List<CustomerIDProofDetailsVM>();
            //        int slno = 1;
            //        foreach (CustomerDocument cipd in lstOfmisDoc) {
            //            CustomerIDProofDetailsVM det = new CustomerIDProofDetailsVM();
            //            det.objID = cipd.ID.ToString();
            //            det.CompanyCode = "";
            //            det.BranchCode = "";
            //            det.CustID = cipd.CustID;
            //            det.SlNo = slno;
            //            det.DocCode = cipd.Type;
            //            det.DocName = cipd.Name;
            //            det.DocNo = cipd.Name;
            //            det.DocImage = null;
            //            det.RepoCustId = null;
            //            det.UpdatedDate = null;
            //            det.RepDocID = null;
            //            det.DocImagePath = cipd.ImagePath;
            //            lstOfMISCustDetVM.Add(det);
            //            slno = slno + 1;
            //        }
            //        cust.lstOfProofs = lstOfMISCustDetVM;
            //    }
            //    CustomerMasterVM customer = new CustomerBL().Post(cust, out error);
            //KSTU_CUSTOMER_MASTER actualCustomerDet = db.KSTU_CUSTOMER_MASTER.Where(c => c.cust_id == customer.ID && c.company_code == customer.CompanyCode && c.branch_code == customer.BranchCode).FirstOrDefault();
            //return actualCustomerDet;

            KSTU_CUSTOMER_MASTER actualCustomerDet = db.KSTU_CUSTOMER_MASTER.Where(c => c.cust_id == customerID && c.company_code == companyCode && c.branch_code == branchCode).FirstOrDefault();
            return actualCustomerDet;
        }
    }
    #endregion
}

