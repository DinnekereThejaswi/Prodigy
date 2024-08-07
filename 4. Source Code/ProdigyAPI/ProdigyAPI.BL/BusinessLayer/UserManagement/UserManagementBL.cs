using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.UserManagement;
using ProdigyAPI.Model.MagnaDb;
using ProdigyAPI.BL.ViewModel.Master;

namespace ProdigyAPI.BL.BusinessLayer.UserManagement
{
    public class UserManagementBL : IUserManagement
    {
        MagnaDbEntities db = null;

        public UserManagementBL()
        {
            db = new MagnaDbEntities(true);
        }

        public UserManagementBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }
        private dynamic GetOperatorQuery(string companyCode, string branchCode, string operatorCode)
        {
            var q = (from op in db.SDTU_OPERATOR_MASTER
                    join bm in db.OperatorBranchMappings
                    on new { CC = op.company_code, BC = op.branch_code, OCode = op.OperatorCode }
                    equals new { CC = bm.CompanyCode, BC = bm.BranchCode, OCode = bm.OperatorCode } into ops
                    from om in ops.DefaultIfEmpty()
                    join dm in db.OperatorDefaultBranches
                    on new { CC = op.company_code, BC = op.branch_code, OCode = op.OperatorCode }
                    equals new { CC = dm.CompanyCode, BC = dm.BranchCode, OCode = dm.OperatorCode } into opdms
                    from dms in opdms.DefaultIfEmpty()
                     where op.company_code == companyCode && op.branch_code == branchCode
                     orderby op.OperatorCode, op.UpdateOn
                    select new
                    {
                        op,
                        MappedStores = ops.Select(r => new ListOfValue
                        {
                            Code = r.BranchCode,
                            Name = r.BranchCode
                        }).ToList(),
                        DefaultStore = dms.BranchCode
                    });
            if (operatorCode != null)
                q.Where(x => x.op.OperatorCode == operatorCode);

            return q.ToList();
        }

        public List<OperatorViewModel> List(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            //Food for thought: EF is converting the following query to double left join for OperatorBranchMapping 
            //on table SDTU_OPERATOR_MASTER. Though there wont' be issue in getting the result, I'm concerned with
            //execution speed. Try if you can avoid that.
            try {
                #region Could not make it to work the way I want, will return and look at this later.
                //var users = (from op in db.SDTU_OPERATOR_MASTER
                //            join bm in db.OperatorBranchMappings
                //                on op.OperatorCode equals bm.OperatorCode into ops
                //            from om in ops.DefaultIfEmpty()
                //            join dm in db.OperatorDefaultBranches
                //                on op.OperatorCode equals dm.OperatorCode into opdms
                //            from dms in opdms.DefaultIfEmpty()
                //                where op.company_code == companyCode && op.branch_code == branchCode
                //            orderby op.OperatorCode, op.UpdateOn
                //            select new
                //             {
                //                 op,
                //                 MappedStores = ops.Select(r => new ListOfValue
                //                 {
                //                     Code = r.BranchCode,
                //                     Name = r.BranchCode
                //                 }).ToList(),
                //                 DefaultStore = dms.BranchCode
                //             }).ToList();

                //if (users == null || users.Count <= 0) {
                //    error = new ErrorVM { description = "No details found" };
                //    return null;
                //}

                //var vm = from x in users
                //         group x by new
                //         {
                //             CompanyCode = x.op.company_code,
                //             BranchCode = x.op.branch_code,
                //             CounterCode = x.op.counter_code,
                //             EmployeeID = x.op.employee_no,
                //             OperatorCode = x.op.OperatorCode,
                //             OperatorName = x.op.OperatorName,
                //             MaxDiscountPercentAllowed = Convert.ToDecimal(x.op.discount_percent),
                //             MobileNo = x.op.mobile_no,
                //             RoleID = x.op.OperatorRole,
                //             Status = x.op.object_status == "O" ? "Active" : "Closed",
                //             OperatorType = x.op.OperatorType == "O" ? "Others" : "Admin",
                //             DefaultStore = x.DefaultStore
                //         } into g
                //         select (new OperatorViewModel
                //         {
                //             CompanyCode = g.Key.CompanyCode,
                //             BranchCode = g.Key.BranchCode,
                //             CounterCode = g.Key.CounterCode,
                //             EmployeeID = g.Key.EmployeeID,
                //             OperatorCode = g.Key.OperatorCode,
                //             OperatorName = g.Key.OperatorName,
                //             MaxDiscountPercentAllowed = g.Key.MaxDiscountPercentAllowed,
                //             MobileNo = g.Key.MobileNo,
                //             RoleID = g.Key.RoleID,
                //             Status = g.Key.Status,
                //             OperatorType = g.Key.OperatorType,
                //             DefaultStore = g.Key.DefaultStore,
                //             MappedStores = g.Select(m => m.ops.Select(s => new ListOfValue {Code = s.BranchCode }))
                //         });
                //var viewModel = users.Select(x => new OperatorViewModel
                //{
                //    CompanyCode = x.op.company_code,
                //    BranchCode = x.op.branch_code,
                //    CounterCode = x.op.counter_code,
                //    EmployeeID = x.op.employee_no,
                //    OperatorCode = x.op.OperatorCode,
                //    OperatorName = x.op.OperatorName,
                //    MaxDiscountPercentAllowed = Convert.ToDecimal(x.op.discount_percent),
                //    MobileNo = x.op.mobile_no,
                //    RoleID = x.op.OperatorRole,
                //    Status = x.op.object_status == "O" ? "Active" : "Closed",
                //    OperatorType = x.op.OperatorType == "O" ? "Others" : "Admin",
                //    DefaultStore = x.DefaultStore,
                //    MappedStores = x.MappedStores.Select(m => new ListOfValue()
                //    {
                //        Code = m.Code,
                //        Name = m.Name
                //    }).Distinct().ToList()
                //}).ToList(); 
                #endregion

                List<OperatorViewModel> viewModel = new List<OperatorViewModel>();
                var operators = db.SDTU_OPERATOR_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode).OrderBy(o => o.OperatorCode).ToList();
                var branchMappings = (from bm in db.OperatorBranchMappings
                                     join op in db.SDTU_OPERATOR_MASTER
                                        on bm.OperatorCode equals op.OperatorCode
                                     where op.company_code == companyCode
                                        && op.branch_code == branchCode
                                     select bm).ToList();
                var defaultBranchs = (from odb in db.OperatorDefaultBranches
                                      join op in db.SDTU_OPERATOR_MASTER
                                         on odb.OperatorCode equals op.OperatorCode
                                      where op.company_code == companyCode
                                         && op.branch_code == branchCode
                                      select odb).ToList();

                foreach (var p in operators) {
                    var opm = new OperatorViewModel
                    {
                        CompanyCode = p.company_code,
                        BranchCode = p.branch_code,
                        CounterCode = p.counter_code,
                        EmployeeID = p.employee_no,
                        OperatorCode = p.OperatorCode,
                        OperatorName = p.OperatorName,
                        MaxDiscountPercentAllowed = Convert.ToDecimal(p.discount_percent),
                        MobileNo = p.mobile_no,
                        RoleID = p.OperatorRole,
                        Status = p.object_status == "O" ? "Active" : "Closed",
                        OperatorType = p.OperatorType == "O" ? "Others" : "Admin",
                        DefaultStore = defaultBranchs.Where(d => d.OperatorCode == p.OperatorCode).Select(s => s.BranchCode).FirstOrDefault(),
                        MappedStores = branchMappings.Where(ms => ms.OperatorCode == p.OperatorCode).Select(s => new ListOfValue { Code = s.BranchCode, Name = s.BranchCode }).ToList()
                    };
                    viewModel.Add(opm);
                }
                return viewModel;
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return null;
            }
        }

        public OperatorViewModel Get(string companyCode, string branchCode, string operatorCode, out ErrorVM error)
        {
            error = null;
            try {
                var users = (from op in db.SDTU_OPERATOR_MASTER
                             join bm in db.OperatorBranchMappings
                             on op.OperatorCode equals bm.OperatorCode  into ops
                             from om in ops.DefaultIfEmpty()
                             join dm in db.OperatorDefaultBranches
                             on op.OperatorCode equals dm.OperatorCode into opdms
                             from dms in opdms.DefaultIfEmpty()
                             where op.company_code == companyCode && op.branch_code == branchCode
                                && op.OperatorCode == operatorCode
                             orderby op.OperatorCode, op.UpdateOn
                             select new
                             {
                                 op,
                                 MappedStores = ops.Select(r => new ListOfValue
                                 {
                                     Code = r.BranchCode,
                                     Name = r.BranchCode
                                 }).ToList(),
                                 DefaultStore = dms.BranchCode
                             }).FirstOrDefault();

                if (users == null) {
                    error = new ErrorVM { description = "No details found for the user " + operatorCode };
                    return null;
                }

                var viewModel = new OperatorViewModel
                {
                    CompanyCode = users.op.company_code,
                    BranchCode = users.op.branch_code,
                    CounterCode = users.op.counter_code,
                    EmployeeID = users.op.employee_no,
                    OperatorCode = users.op.OperatorCode,
                    OperatorName = users.op.OperatorName,
                    MaxDiscountPercentAllowed = Convert.ToDecimal(users.op.discount_percent),
                    MobileNo = users.op.mobile_no,
                    RoleID = users.op.OperatorRole,
                    Status = users.op.object_status == "O" ? "Active" : "Closed",
                    OperatorType = users.op.OperatorType == "O" ? "Others" : "Admin",
                    DefaultStore = users.DefaultStore,
                    MappedStores = users.MappedStores.Select(m => new ListOfValue()
                    {
                        Code = m.Code,
                        Name = m.Name
                    }).ToList()
                };
                return viewModel;
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return null;
            }
        }

        private bool ValidateOperatorViewModel(OperatorViewModel vm, out string hashedPassword, out ErrorVM error)
        {
            error = null;
            hashedPassword = string.Empty;
            //Check if operator code already exists
            var operatorMaster = db.SDTU_OPERATOR_MASTER.Where(x => x.OperatorCode == vm.OperatorCode).FirstOrDefault();
            if(operatorMaster != null) {
                error = new ErrorVM { description = string.Format($"Operator Code {vm.OperatorCode} already exists.") };
                return false;
            }

            var role = db.SDTU_ROLE_MASTER.Where(r => r.company_code == vm.CompanyCode && r.branch_code == vm.BranchCode
                    && r.RoleID == vm.RoleID).FirstOrDefault();
            if (role == null) {
                error = new ErrorVM { description = string.Format($"RoleId {vm.RoleID} is not found.") };
                return false;
            }
            if (vm.CounterCode != "ALL") {
                var counter = db.KSTU_COUNTER_MASTER.Where(c => c.company_code == vm.CompanyCode && c.branch_code == vm.BranchCode
                && c.counter_code == vm.CounterCode).FirstOrDefault();
                if (counter == null) {
                    error = new ErrorVM { description = string.Format($"Counter code {vm.CounterCode} is not found.") };
                    return false;
                }
            }
            if (vm.OperatorType == "Admin" || vm.OperatorType == "Others") {
                ;
            }
            else {
                error = new ErrorVM { description = string.Format($"OperatorType should be either Admin or Others") };
                return false;
            }

            if (!GetHashedPassword(vm.Password, out hashedPassword, out error)) {
                return false;
            }
            return true;
        }

        private bool GetHashedPassword(string base64Password, out string hashedPassword, out ErrorVM error)
        {
            error = null;
            hashedPassword = string.Empty;
            string password = SIGlobals.Globals.Base64Decode(base64Password);
            if (string.IsNullOrEmpty(password)) {
                error = new ErrorVM { description = "Invalid password. Password must be base 64 encoded." };
                return false;
            }

            hashedPassword = SIGlobals.Globals.GetHashcode(password);
            if (string.IsNullOrEmpty(hashedPassword)) {
                error = new ErrorVM { description = "Unable to hash the password. Please contact administrator." };
                return false;
            }
            return true;
        }

        public bool Add(OperatorViewModel vm, string userID, out ErrorVM error)
        {
            error = null;
            try {
                #region Validation
                string hashedPassword = string.Empty;
                if (!ValidateOperatorViewModel(vm, out hashedPassword, out error)) {
                    return false;
                } 
                #endregion

                #region Operator Master
                string objId = SIGlobals.Globals.GetMagnaGUID(new string[] { "SDTU_OPERATOR_MASTER", vm.OperatorCode }, vm.CompanyCode, vm.BranchCode);
                SDTU_OPERATOR_MASTER op = new SDTU_OPERATOR_MASTER
                {
                    obj_id = objId,
                    company_code = vm.CompanyCode,
                    branch_code = vm.BranchCode,
                    OperatorCode = vm.OperatorCode,
                    OperatorName = vm.OperatorName,
                    mobile_no = vm.MobileNo,
                    counter_code = vm.CounterCode,
                    OperatorRole = vm.RoleID,
                    object_status = "O",
                    employee_no = vm.EmployeeID,
                    Password3 = hashedPassword,
                    discount_percent = vm.MaxDiscountPercentAllowed,
                    OperatorType = vm.OperatorType == "Admin" ? "A" : "O",
                    UpdateOn = SIGlobals.Globals.GetDateTime(),
                    PasswordSalt = Guid.NewGuid()
                };
                db.SDTU_OPERATOR_MASTER.Add(op); 
                #endregion

                #region Default Branch
                //The created branch is the default branch if defualt branch is not specified.
                var defaultBranch = string.IsNullOrEmpty(vm.DefaultStore) ? vm.BranchCode : vm.DefaultStore;
                OperatorDefaultBranch odb = new OperatorDefaultBranch
                {
                    CompanyCode = vm.CompanyCode,
                    BranchCode = defaultBranch,
                    OperatorCode = vm.OperatorCode,
                    UpdatedOn = SIGlobals.Globals.GetDateTime(),
                    UpdatedBy = userID
                };
                db.OperatorDefaultBranches.Add(odb);
                #endregion

                #region Branch Mappings
                if (vm.MappedStores == null || string.IsNullOrEmpty(vm.MappedStores[0].Code)) {
                    OperatorBranchMapping obm = new OperatorBranchMapping
                    {
                        CompanyCode = vm.CompanyCode,
                        BranchCode = vm.BranchCode,
                        OperatorCode = vm.OperatorCode,
                        IsReportAllowed = true,
                        IsTransactionAllowed = true,
                        UpdatedOn = SIGlobals.Globals.GetDateTime()
                    };
                    db.OperatorBranchMappings.Add(obm);
                }
                else {
                    var ownStoreMappingFound = vm.MappedStores.Where(ms => ms.Code == vm.BranchCode).FirstOrDefault();
                    if (ownStoreMappingFound == null) {
                        OperatorBranchMapping obm = new OperatorBranchMapping
                        {
                            CompanyCode = vm.CompanyCode,
                            BranchCode = vm.BranchCode,
                            OperatorCode = vm.OperatorCode,
                            IsReportAllowed = true,
                            IsTransactionAllowed = true,
                            UpdatedOn = SIGlobals.Globals.GetDateTime()
                        };
                        db.OperatorBranchMappings.Add(obm);
                    }
                    foreach (var row in vm.MappedStores) {
                        OperatorBranchMapping obm = new OperatorBranchMapping
                        {
                            CompanyCode = vm.CompanyCode,
                            BranchCode = row.Code,
                            OperatorCode = vm.OperatorCode,
                            IsReportAllowed = true,
                            IsTransactionAllowed = true,
                            UpdatedOn = SIGlobals.Globals.GetDateTime()
                        };
                        db.OperatorBranchMappings.Add(obm);
                    }
                } 
                #endregion

                db.SaveChanges();
                SIGlobals.Globals.WriteTransactionLog(userID,
                    string.Format($"User {vm.OperatorCode} added."), null, db);
            }
            catch(Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        public bool ChangePasword(OperatorPasswordViewModel vm, string userID, out ErrorVM error)
        {
            error = null;
            try {
                #region Validations
                if (string.IsNullOrEmpty(userID)) {
                    error = new ErrorVM { description = "Failed to get user code. Unable to change password."};
                    return false;
                }

                var operatorMaster = db.SDTU_OPERATOR_MASTER.Where(x => x.company_code == vm.CompanyCode
                       && x.branch_code == vm.BranchCode && x.OperatorCode == userID).FirstOrDefault();
                if (operatorMaster == null) {
                    error = new ErrorVM { description = "No details found for the user: " + userID };
                    return false;
                }
                if (operatorMaster.object_status != "O") {
                    error = new ErrorVM { description = string.Format($"The user {userID} is not active. Only active user details can be modified.") };
                    return false;
                }

                string oldHashedPassword = string.Empty;
                if (!GetHashedPassword(vm.OldPassword, out oldHashedPassword, out error)) {
                    return false;
                }
                if (operatorMaster.Password3 != oldHashedPassword) {
                    error = new ErrorVM { description = "Invalid old password." };
                    return false;
                }
                #endregion

                string newHashedPassword = string.Empty;
                if (!GetHashedPassword(vm.NewPassword, out newHashedPassword, out error)) {
                    return false;
                }
                operatorMaster.Password3 = newHashedPassword;
                operatorMaster.UpdateOn = SIGlobals.Globals.GetDateTime();
                db.Entry(operatorMaster).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                SIGlobals.Globals.WriteTransactionLog(userID,
                    string.Format($"User {userID} changed password."), null, db);
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }

            return true;
        }        
        public bool Update(OperatorViewModel vm, string userID, out ErrorVM error)
        {
            error = null;
            string hashedPassword = string.Empty;
            try {
                #region Validations
                var operatorMaster = db.SDTU_OPERATOR_MASTER.Where(x => x.company_code == vm.CompanyCode
                       && x.branch_code == vm.BranchCode && x.OperatorCode == vm.OperatorCode).FirstOrDefault();
                if (operatorMaster == null) {
                    error = new ErrorVM { description = "No details found for the user: " + vm.OperatorCode };
                    return false;
                }

                if (operatorMaster.object_status != "O") {
                    error = new ErrorVM { description = string.Format($"The user {vm.OperatorCode} is not active. Only active user details can be modified.") };
                    return false;
                }

                if (!ValidateOperatorViewModel(vm, out hashedPassword, out error))
                    return false;
                #endregion

                #region Operator Master Update
                operatorMaster.OperatorName = vm.OperatorName;
                operatorMaster.OperatorRole = vm.RoleID;
                //operatorMaster.Password3 = hashedPassword; //Password should not be re-saved.
                operatorMaster.employee_no = vm.EmployeeID;
                operatorMaster.mobile_no = vm.MobileNo;
                operatorMaster.discount_percent = vm.MaxDiscountPercentAllowed;
                operatorMaster.counter_code = vm.CounterCode;
                operatorMaster.UpdateOn = SIGlobals.Globals.GetDateTime();
                //operatorMaster.UpdateBy = userID; //Missing in Magna
                db.Entry(operatorMaster).State = System.Data.Entity.EntityState.Modified;
                #endregion

                #region Store Mapping
                if (string.IsNullOrEmpty(vm.DefaultStore))
                    vm.DefaultStore = vm.BranchCode;
                var defaultBranch = db.OperatorDefaultBranches.Where(x => x.OperatorCode == vm.OperatorCode).FirstOrDefault();
                if (defaultBranch != null) {
                    if (defaultBranch.BranchCode != vm.DefaultStore) {
                        defaultBranch.CompanyCode = vm.CompanyCode;
                        defaultBranch.BranchCode = vm.DefaultStore;
                        defaultBranch.UpdatedOn = SIGlobals.Globals.GetDateTime();
                        db.Entry(defaultBranch).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                #endregion

                #region Branch Mappings
                var oldBranchMappings = db.OperatorBranchMappings.Where(x => x.OperatorCode == vm.OperatorCode).ToList();
                if (oldBranchMappings != null) {
                    db.OperatorBranchMappings.RemoveRange(oldBranchMappings);
                }

                if (vm.MappedStores == null || string.IsNullOrEmpty(vm.MappedStores[0].Code)) {
                    OperatorBranchMapping obm = new OperatorBranchMapping
                    {
                        CompanyCode = vm.CompanyCode,
                        BranchCode = vm.BranchCode,
                        OperatorCode = vm.OperatorCode,
                        IsReportAllowed = true,
                        IsTransactionAllowed = true,
                        UpdatedOn = SIGlobals.Globals.GetDateTime()
                    };
                    db.OperatorBranchMappings.Add(obm);
                }
                else {
                    var ownStoreMappingFound = vm.MappedStores.Where(ms => ms.Code == vm.BranchCode).FirstOrDefault();
                    if (ownStoreMappingFound == null) {
                        OperatorBranchMapping obm = new OperatorBranchMapping
                        {
                            CompanyCode = vm.CompanyCode,
                            BranchCode = vm.BranchCode,
                            OperatorCode = vm.OperatorCode,
                            IsReportAllowed = true,
                            IsTransactionAllowed = true,
                            UpdatedOn = SIGlobals.Globals.GetDateTime()
                        };
                        db.OperatorBranchMappings.Add(obm);
                    }
                    foreach (var row in vm.MappedStores) {
                        OperatorBranchMapping obm = new OperatorBranchMapping
                        {
                            CompanyCode = vm.CompanyCode,
                            BranchCode = row.Code,
                            OperatorCode = vm.OperatorCode,
                            IsReportAllowed = true,
                            IsTransactionAllowed = true,
                            UpdatedOn = SIGlobals.Globals.GetDateTime()
                        };
                        db.OperatorBranchMappings.Add(obm);
                    }
                }
                #endregion

                db.SaveChanges();
                SIGlobals.Globals.WriteTransactionLog(userID,
                    string.Format($"User {vm.OperatorCode} modified."), null, db);
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }

            return true;
        }

        public bool Close(string companyCode, string branchCode, string operatorCode, string userID, out ErrorVM error)
        {
            error = null;
            try {
                #region Validations
                var operatorMaster = db.SDTU_OPERATOR_MASTER.Where(x => x.company_code == companyCode
                       && x.branch_code == branchCode && x.OperatorCode == operatorCode).FirstOrDefault();
                if (operatorMaster == null) {
                    error = new ErrorVM { description = "No details found for the user: " + operatorCode };
                    return false;
                }
                if (operatorMaster.object_status == "C") {
                    error = new ErrorVM { description = string.Format($"The user {operatorCode} is already closed. Only active user details can be closed.") };
                    return false;
                }
                #endregion
                operatorMaster.object_status = "C";
                operatorMaster.UpdateOn = SIGlobals.Globals.GetDateTime();
                db.Entry(operatorMaster).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                SIGlobals.Globals.WriteTransactionLog(userID,
                    string.Format($"User {operatorCode} closed"), null, db);
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }

            return true;
        }
        public bool Open(string companyCode, string branchCode, string operatorCode, string userID, out ErrorVM error)
        {
            error = null;
            try {
                #region Validations
                var operatorMaster = db.SDTU_OPERATOR_MASTER.Where(x => x.company_code == companyCode
                       && x.branch_code == branchCode && x.OperatorCode == operatorCode).FirstOrDefault();
                if (operatorMaster == null) {
                    error = new ErrorVM { description = "No details found for the user: " + operatorCode };
                    return false;
                }
                if (operatorMaster.object_status == "O") {
                    error = new ErrorVM { description = string.Format($"The user {operatorCode} is already active and therefore it can be activated again.") };
                    return false;
                }
                #endregion

                operatorMaster.object_status = "O";
                operatorMaster.UpdateOn = SIGlobals.Globals.GetDateTime();
                db.Entry(operatorMaster).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                SIGlobals.Globals.WriteTransactionLog(userID, 
                    string.Format($"User {operatorCode} opened"), null, db);
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        public List<ListOfValue> GetBranches(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                var suppliers = db.KSTU_SUPPLIER_MASTER.Where(x => x.company_code == companyCode
                        && x.branch_code == branchCode && x.obj_status != "C" && x.voucher_code == "VB")
                        .OrderBy(m => m.party_code).ToList();
                if (suppliers == null) {
                    return null;
                }

                var storeList = suppliers.Select(x => new ListOfValue { Code = x.party_code, Name = x.party_name }).ToList();
                if (storeList.Where(x => x.Code == branchCode).FirstOrDefault() == null) {
                    storeList.Add(new ListOfValue { Code = branchCode, Name = branchCode });
                }
                return storeList;
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return null;
            }

        }
    }
}
