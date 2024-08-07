using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProdigyAPI.BL.BusinessLayer.Masters
{
    public class ApplicationPasswordBL
    {
        MagnaDbEntities db = null;

        public ApplicationPasswordBL()
        {
            db = new MagnaDbEntities(true);
        }

        public ApplicationPasswordBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }

        public List<ApplicationPasswordVM> List(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                var smm = db.KTTS_APP_PASS.Where(x => x.company_code == companyCode
                        && x.branch_code == branchCode).OrderBy(y => y.pass_no).ToList();
               
                if (smm == null || smm.Count <= 0) {
                    error = new ErrorVM { description = "No details found" };
                    return null;
                }

                var menuVM = smm.Select(p => new ApplicationPasswordVM
                {
                    PasswordNo = Convert.ToInt32(p.pass_no),
                    CompanyCode = p.company_code,
                    BranchCode = p.branch_code,
                    Password = p.pass
                }).ToList();
                return menuVM;
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return null;
            }
        }

        public ApplicationPasswordVM Get(string companyCode, string branchCode, int id, out ErrorVM error)
        {
            error = null;
            try {
                string passwordNo = id.ToString();
                var p = db.KTTS_APP_PASS.Where(x => x.company_code == companyCode
                        && x.branch_code == branchCode && x.pass_no == passwordNo).FirstOrDefault();
                if (p == null) {
                    error = new ErrorVM { description = "No details found" };
                    return null;
                }
                var passwordVM = new ApplicationPasswordVM
                {
                    PasswordNo = Convert.ToInt32(p.pass_no),
                    CompanyCode = p.company_code,
                    BranchCode = p.branch_code,
                    Password = p.pass
                };
                return passwordVM;
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return null;
            }
        }

        public bool Add(ApplicationPasswordVM vm, string userID, out ErrorVM error)
        {
            error = null;
            try {
                #region Validation
                string passwordNo = vm.PasswordNo.ToString();
                var oldRecord = db.KTTS_APP_PASS.Where(x => x.company_code == vm.CompanyCode && x.branch_code == vm.BranchCode
                    && x.pass_no == passwordNo).FirstOrDefault();
                if (oldRecord != null) {
                    error = new ErrorVM { description = "The record with ID " + vm.PasswordNo.ToString() + " already exists." };
                    return false;
                }

                if (string.IsNullOrEmpty(vm.Password)) {
                    error = new ErrorVM { description = "Password cannot be null or empty" };
                    return false;
                }
                #endregion

                #region Master
                string hashedPassword = string.Empty;
                if (!GetHashedPassword(vm.Password, out hashedPassword, out error)) {
                    return false;
                }
                string objId = SIGlobals.Globals.GetMagnaGUID(new string[] { "KTTS_APP_PASS", vm.PasswordNo.ToString()},
                    vm.CompanyCode, vm.BranchCode);

                KTTS_APP_PASS rm = new KTTS_APP_PASS
                {
                    obj_id = objId,
                    company_code = vm.CompanyCode,
                    branch_code = vm.BranchCode,
                    pass_no = passwordNo,
                    pass = hashedPassword,
                    conf_pass = hashedPassword,
                    UpdateOn = SIGlobals.Globals.GetDateTime()
                };
                db.KTTS_APP_PASS.Add(rm);
                #endregion

                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
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

        public bool Modify(ApplicationPasswordVM vm, string userID, out ErrorVM error)
        {
            error = null;
            try {
                #region Validation
                string passwordNo = vm.PasswordNo.ToString();
                var recordToUpdate = db.KTTS_APP_PASS.Where(x => x.company_code == vm.CompanyCode && x.branch_code == vm.BranchCode
                    && x.pass_no == passwordNo).FirstOrDefault();
                if (recordToUpdate == null) {
                    error = new ErrorVM { description = "Password No " + passwordNo + " is not found." };
                    return false;
                }

                if (string.IsNullOrEmpty(vm.Password)) {
                    error = new ErrorVM { description = "Password cannot be null or empty" };
                    return false;
                }
                #endregion

                #region Master
                string hashedPassword = string.Empty;
                if (!GetHashedPassword(vm.Password, out hashedPassword, out error)) {
                    return false;
                }

                recordToUpdate.company_code = vm.CompanyCode;
                recordToUpdate.branch_code = vm.BranchCode;
                recordToUpdate.pass = hashedPassword;
                recordToUpdate.conf_pass = hashedPassword;
                recordToUpdate.UpdateOn = SIGlobals.Globals.GetDateTime();
                db.Entry(recordToUpdate).State = System.Data.Entity.EntityState.Modified;
                #endregion

                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        public bool Delete(string companyCode, string branchCode, int id, string userID, out ErrorVM error)
        {
            error = null;
            try {
                #region Validations
                string passwordNo = id.ToString();
                var record = db.KTTS_APP_PASS.Where(x => x.company_code == companyCode
                       && x.branch_code == branchCode && x.pass_no == passwordNo).FirstOrDefault();
                if (record == null) {
                    error = new ErrorVM { description = "No details found for the ID: " + id };
                    return false;
                }
                #endregion
                
                db.Entry(record).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }
    }
}

