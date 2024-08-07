using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.UserManagement;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.UserManagement
{
    public class RoleBL
    {
        MagnaDbEntities db = null;

        public RoleBL()
        {
            db = new MagnaDbEntities(true);
        }

        public RoleBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }

        public List<RoleViewModel> List(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            var q = db.SDTU_ROLE_MASTER.Where(x => x.company_code == companyCode 
                && x.branch_code == branchCode && x.object_status != "A").OrderBy(y => y.RoleID).ToList();
            if(q == null || q.Count <= 0) {
                error = new ErrorVM { description = "No details found" };
                return null;
            }
            var roleVM = q.Select(p => new RoleViewModel
            {
                ID = p.RoleID,
                CompanyCode = p.company_code,
                BranchCode = p.branch_code,
                Name = p.RoleName,
                OTPValidationEnabled = p.isOTPValidation == "Y" ? true : false,
                Status = p.object_status == "O" ? "Active" : "Closed"
            }).ToList();
            return roleVM;
        }

        public RoleViewModel Get(string companyCode, string branchCode, int id, out ErrorVM error)
        {
            error = null;
            var q = db.SDTU_ROLE_MASTER.Where(x => x.company_code == companyCode
                && x.branch_code == branchCode && x.RoleID == id && x.object_status != "A").FirstOrDefault();
            if (q == null) {
                error = new ErrorVM { description = "No details found" };
                return null;
            }
            var roleVM = new RoleViewModel
            {
                ID = q.RoleID,
                CompanyCode = q.company_code,
                BranchCode = q.branch_code,
                Name = q.RoleName,
                OTPValidationEnabled = q.isOTPValidation == "Y" ? true : false,
                Status = q.object_status == "O" ? "Active" : "Closed"
            };
            return roleVM;
        }

        public bool Add(RoleViewModel vm, string userID, out ErrorVM error)
        {
            error = null;
            try {
                #region Validation
                var roleExist = db.SDTU_ROLE_MASTER.Where(x => x.company_code == vm.CompanyCode && x.branch_code == vm.BranchCode
                    && x.RoleName == vm.Name).FirstOrDefault();
                if(roleExist != null) {
                    error = new ErrorVM { description = "Role name " + vm.Name + " already exists." };
                    return false;
                }
                #endregion

                #region Role Master
                var maxRoleId = db.SDTU_ROLE_MASTER.Where(x => x.company_code == vm.CompanyCode
                    && x.branch_code == vm.BranchCode).Select(y => y.RoleID).DefaultIfEmpty().Max();
                int roleId = maxRoleId + 1;

                string objId = SIGlobals.Globals.GetMagnaGUID(new string[] { "SDTU_ROLE_MASTER", roleId.ToString() }, vm.CompanyCode, vm.BranchCode);
                SDTU_ROLE_MASTER rm = new SDTU_ROLE_MASTER
                {
                    obj_id = objId,
                    RoleID = roleId,
                    company_code = vm.CompanyCode,
                    branch_code = vm.BranchCode, RoleName = vm.Name,
                    object_status = "O", flag = "N",
                    isOTPValidation = vm.OTPValidationEnabled == true ? "Y" : "N",
                    UpdateOn = SIGlobals.Globals.GetDateTime()
                };
                db.SDTU_ROLE_MASTER.Add(rm);
                #endregion

                db.SaveChanges();
                SIGlobals.Globals.WriteTransactionLog(userID,
                    string.Format($"Role {vm.Name} is added."), null, db);
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        public bool Modify(RoleViewModel vm, string userID, out ErrorVM error)
        {
            error = null;
            try {
                #region Validation
                var roleToUpdate = db.SDTU_ROLE_MASTER.Where(x => x.company_code == vm.CompanyCode && x.branch_code == vm.BranchCode
                    && x.RoleID == vm.ID).FirstOrDefault();
                if (roleToUpdate == null) {
                    error = new ErrorVM { description = "Role Id " + vm.ID + " doesn't exist exist." };
                    return false;
                }
                if (roleToUpdate.RoleName != vm.Name) {
                    var existingRole = db.SDTU_ROLE_MASTER.Where(x => x.company_code == vm.CompanyCode && x.branch_code == vm.BranchCode
                       && x.RoleName == vm.Name).FirstOrDefault();
                    if (existingRole != null) {
                        error = new ErrorVM { description = "Role name " + vm.Name + " you are updating already exists. Please try with different role name." };
                        return false;
                    }
                }
                #endregion

                #region Role Master

                roleToUpdate.RoleName = vm.Name;
                roleToUpdate.UpdateOn = SIGlobals.Globals.GetDateTime();
                roleToUpdate.isOTPValidation = vm.OTPValidationEnabled == true ? "Y" : "N";
                db.Entry(roleToUpdate).State = System.Data.Entity.EntityState.Modified;
                #endregion

                db.SaveChanges();
                SIGlobals.Globals.WriteTransactionLog(userID,
                    string.Format($"Role {vm.Name} is modified."), null, db);
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        public bool Open(string companyCode, string branchCode, int roleId, string userID, out ErrorVM error)
        {
            error = null;
            try {
                #region Validations
                var roleMaster = db.SDTU_ROLE_MASTER.Where(x => x.company_code == companyCode
                       && x.branch_code == branchCode && x.RoleID == roleId).FirstOrDefault();
                if (roleMaster == null) {
                    error = new ErrorVM { description = "No details found for the role ID: " + roleId };
                    return false;
                }
                if (roleMaster.object_status == "O") {
                    error = new ErrorVM { description = string.Format($"The role id {roleId} is already active and therefore it can be activated again.") };
                    return false;
                }
                #endregion

                roleMaster.object_status = "O";
                roleMaster.UpdateOn = SIGlobals.Globals.GetDateTime();
                db.Entry(roleMaster).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                SIGlobals.Globals.WriteTransactionLog(userID,
                    string.Format($"Role {roleMaster.RoleName} is opened."), null, db);
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        public bool Close(string companyCode, string branchCode, int roleId, string userID, out ErrorVM error)
        {
            error = null;
            try {
                #region Validations
                var roleMaster = db.SDTU_ROLE_MASTER.Where(x => x.company_code == companyCode
                       && x.branch_code == branchCode && x.RoleID == roleId).FirstOrDefault();
                if (roleMaster == null) {
                    error = new ErrorVM { description = "No details found for the role ID: " + roleId };
                    return false;
                }
                if (roleMaster.object_status == "C") {
                    error = new ErrorVM { description = string.Format($"The role id {roleId} is already de-active and therefore it can be de-activated again.") };
                    return false;
                }
                #endregion

                roleMaster.object_status = "C";
                roleMaster.UpdateOn = SIGlobals.Globals.GetDateTime();
                db.Entry(roleMaster).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                SIGlobals.Globals.WriteTransactionLog(userID,
                    string.Format($"Role {roleMaster.RoleName} is closed."), null, db);
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }
    }
}
