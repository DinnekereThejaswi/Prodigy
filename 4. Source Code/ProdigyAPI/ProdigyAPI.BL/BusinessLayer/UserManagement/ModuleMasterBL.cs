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
    public class ModuleMasterBL
    {
        MagnaDbEntities db = null;

        public ModuleMasterBL()
        {
            db = new MagnaDbEntities(true);
        }

        public ModuleMasterBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }

        public List<MainMenuViewModel> List(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                var q = db.SDTS_MODULE_MASTER.Where(x => x.company_code == companyCode
                        && x.branch_code == branchCode).OrderBy(y => y.obj_id).ToList();
                if (q == null || q.Count <= 0) {
                    error = new ErrorVM { description = "No details found" };
                    return null;
                }
                var menuVM = q.Select(p => new MainMenuViewModel
                {
                    ID = p.obj_id,
                    CompanyCode = p.company_code,
                    BranchCode = p.branch_code,
                    Name = p.ModuleName,
                    Status = p.obj_status == "O" ? "Active" : "Closed",
                    SortOrder = Convert.ToInt32(p.seq_no),
                    UIRoute = p.AngularUIRoute,
                    Class = p.Class,
                    Icon = p.Icon,
                    Label = p.Label,
                    LabelClass = p.LabelClass
                }).ToList();
                return menuVM;
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return null;
            }
        }

        public MainMenuViewModel Get(string companyCode, string branchCode, int id, out ErrorVM error)
        {
            error = null;
            try {
                var p = db.SDTS_MODULE_MASTER.Where(x => x.company_code == companyCode
                        && x.branch_code == branchCode && x.obj_id == id).OrderBy(y => y.obj_id).FirstOrDefault();
                if (p == null) {
                    error = new ErrorVM { description = "No details found" };
                    return null;
                }
                var menuVM = new MainMenuViewModel
                {
                    ID = p.obj_id,
                    CompanyCode = p.company_code,
                    BranchCode = p.branch_code,
                    Name = p.ModuleName,
                    Status = p.obj_status == "O" ? "Active" : "Closed",
                    SortOrder = Convert.ToInt32(p.seq_no),
                    UIRoute = p.AngularUIRoute,
                    Class = p.Class,
                    Icon = p.Icon,
                    Label = p.Label,
                    LabelClass = p.LabelClass
                };
                return menuVM;
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return null;
            };
        }

        public bool Add(MainMenuViewModel vm, string userID, out ErrorVM error)
        {
            error = null;
            try {
                #region Validation
                var oldRecord = db.SDTS_MODULE_MASTER.Where(x => x.company_code == vm.CompanyCode && x.branch_code == vm.BranchCode
                    && x.ModuleName == vm.Name).FirstOrDefault();
                if (oldRecord != null) {
                    error = new ErrorVM { description = "Module name " + vm.Name + " already exists." };
                    return false;
                }
                #endregion

                #region Master
                var maxId = db.SDTS_MODULE_MASTER.Where(x => x.company_code == vm.CompanyCode
                    && x.branch_code == vm.BranchCode).Select(y => y.obj_id).DefaultIfEmpty().Max();
                int newId = maxId + 1;

                SDTS_MODULE_MASTER rm = new SDTS_MODULE_MASTER
                {
                    obj_id = newId,
                    company_code = vm.CompanyCode,
                    branch_code = vm.BranchCode,
                    ModuleName = vm.Name,
                    obj_status = "O",
                    AngularUIRoute = vm.UIRoute,
                    Class = vm.Class,
                    Icon = vm.Icon,
                    Label = vm.Label,
                    LabelClass = vm.LabelClass,
                    seq_no = vm.SortOrder,
                    UpdateOn = SIGlobals.Globals.GetDateTime()
                };
                db.SDTS_MODULE_MASTER.Add(rm);
                #endregion

                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        public bool Modify(MainMenuViewModel vm, string userID, out ErrorVM error)
        {
            error = null;
            try {
                #region Validation
                var recToUpdate = db.SDTS_MODULE_MASTER.Where(x => x.company_code == vm.CompanyCode && x.branch_code == vm.BranchCode
                    && x.obj_id == vm.ID).FirstOrDefault();
                if (recToUpdate == null) {
                    error = new ErrorVM { description = "Module Id " + vm.ID + " doesn't exist exist." };
                    return false;
                }
                if (recToUpdate.ModuleName != vm.Name) {
                    var existingRole = db.SDTS_MODULE_MASTER.Where(x => x.company_code == vm.CompanyCode && x.branch_code == vm.BranchCode
                       && x.ModuleName == vm.Name).FirstOrDefault();
                    if (existingRole != null) {
                        error = new ErrorVM { description = "Module name " + vm.Name + " you are updating already exists. Please try with different role name." };
                        return false;
                    }
                }
                #endregion

                #region Module Master

                recToUpdate.ModuleName = vm.Name;
                recToUpdate.obj_status = vm.Status == "Active" ? "O" : "C";
                recToUpdate.AngularUIRoute = vm.UIRoute;
                recToUpdate.Class = vm.Class;
                recToUpdate.Icon = vm.Icon;
                recToUpdate.Label = vm.Label;
                recToUpdate.LabelClass = vm.LabelClass;
                recToUpdate.seq_no = vm.SortOrder;
                recToUpdate.UpdateOn = SIGlobals.Globals.GetDateTime();
                db.Entry(recToUpdate).State = System.Data.Entity.EntityState.Modified;
                #endregion

                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        public bool Open(string companyCode, string branchCode, int id, string userID, out ErrorVM error)
        {
            error = null;
            try {
                #region Validations
                var record = db.SDTS_MODULE_MASTER.Where(x => x.company_code == companyCode
                       && x.branch_code == branchCode && x.obj_id == id).FirstOrDefault();
                if (record == null) {
                    error = new ErrorVM { description = "No details found for the ID: " + id };
                    return false;
                }
                if (record.obj_status == "O") {
                    error = new ErrorVM { description = string.Format($"The id {id} is already active and therefore it can be activated again.") };
                    return false;
                }
                #endregion

                record.obj_status = "O";
                record.UpdateOn = SIGlobals.Globals.GetDateTime();
                db.Entry(record).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        public bool Close(string companyCode, string branchCode, int id, string userID, out ErrorVM error)
        {
            error = null;
            try {
                #region Validations
                var record = db.SDTS_MODULE_MASTER.Where(x => x.company_code == companyCode
                       && x.branch_code == branchCode && x.obj_id == id).FirstOrDefault();
                if (record == null) {
                    error = new ErrorVM { description = "No details found for the ID: " + id };
                    return false;
                }
                if (record.obj_status == "C") {
                    error = new ErrorVM { description = string.Format($"The id {id} is already de-active and therefore it can be de-activated again.") };
                    return false;
                }
                #endregion

                record.obj_status = "C";
                record.UpdateOn = SIGlobals.Globals.GetDateTime();
                db.Entry(record).State = System.Data.Entity.EntityState.Modified;
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
