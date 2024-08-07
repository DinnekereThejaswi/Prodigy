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
    public class SubModuleMasterBL
    {
        MagnaDbEntities db = null;

        public SubModuleMasterBL()
        {
            db = new MagnaDbEntities(true);
        }

        public SubModuleMasterBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }

        public List<SubMenuViewModel> List(string companyCode, string branchCode, int? moduleId, out ErrorVM error)
        {
            error = null;
            try {
                var query = db.SDTS_SUBMODULE_MASTER.Where(x => x.company_code == companyCode
                        && x.branch_code == branchCode);
                List<SDTS_SUBMODULE_MASTER> smm;
                if(moduleId != null && moduleId > 0) {
                    int nonNullableModuleId = Convert.ToInt32(moduleId);
                    smm = query.Where(x => x.ModuleID == nonNullableModuleId).OrderBy(y => y.ModuleSqNo).ToList();
                }
                else {
                    smm = query.OrderBy(y => y.obj_id).ToList();
                }
                if (smm == null || smm.Count <= 0) {
                    error = new ErrorVM { description = "No details found" };
                    return null;
                }

                var menuVM = smm.Select(p => new SubMenuViewModel
                {
                    ID = Convert.ToInt32(p.obj_id),
                    ModuleID = p.ModuleID,
                    CompanyCode = p.company_code,
                    BranchCode = p.branch_code,
                    Name = p.SubModuleName,
                    Status = p.obj_status == "O" ? "Active" : "Closed",
                    SortOrder = Convert.ToInt32(p.ModuleSqNo),
                    UIRoute = p.AngularUIRoute,
                    Class = p.Class,
                    Icon = p.Icon,
                    Label = p.Label,
                    LabelClass = p.LabelClass,
                    AutoApprove = p.IsAutoApp == "Y" ? true : false,
                    Flag = "N",
                    FormType = p.FormType,
                    ReportApiRoute = p.ReportApiRoute,
                    ReportServerType = p.ReportServerType
                }).ToList();
                return menuVM;
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return null;
            }
        }

        public SubMenuViewModel Get(string companyCode, string branchCode, int id, out ErrorVM error)
        {
            error = null;
            try {
                string subModuleId = id.ToString();
                var p = db.SDTS_SUBMODULE_MASTER.Where(x => x.company_code == companyCode
                        && x.branch_code == branchCode && x.obj_id == subModuleId)
                        .OrderBy(y => y.ModuleSqNo).FirstOrDefault();
                if (p == null) {
                    error = new ErrorVM { description = "No details found" };
                    return null;
                }
                var menuVM = new SubMenuViewModel
                {
                    ID = Convert.ToInt32(p.obj_id),
                    ModuleID = p.ModuleID,
                    CompanyCode = p.company_code,
                    BranchCode = p.branch_code,
                    Name = p.SubModuleName,
                    Status = p.obj_status == "O" ? "Active" : "Closed",
                    SortOrder = Convert.ToInt32(p.ModuleSqNo),
                    UIRoute = p.AngularUIRoute,
                    Class = p.Class,
                    Icon = p.Icon,
                    Label = p.Label,
                    LabelClass = p.LabelClass,
                    AutoApprove = p.IsAutoApp == "Y" ? true : false,
                    Flag = "N",
                    FormType = p.FormType,
                    ReportApiRoute = p.ReportApiRoute,
                    ReportServerType = p.ReportServerType
                };
                return menuVM;
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return null;
            }
        }

        public bool Add(SubMenuViewModel vm, string userID, out ErrorVM error)
        {
            error = null;
            try {
                #region Validation
                var oldRecord = db.SDTS_SUBMODULE_MASTER.Where(x => x.company_code == vm.CompanyCode && x.branch_code == vm.BranchCode
                    && x.SubModuleName == vm.Name).FirstOrDefault();
                if (oldRecord != null) {
                    error = new ErrorVM { description = "Module name " + vm.Name + " already exists." };
                    return false;
                }
                #endregion

                #region Role Master
                var maxId = db.SDTS_SUBMODULE_MASTER.Where(x => x.company_code == vm.CompanyCode
                    && x.branch_code == vm.BranchCode).Select(y => y.obj_id).DefaultIfEmpty().Max();
                int newId = Convert.ToInt32(maxId) + 1;

                SDTS_SUBMODULE_MASTER rm = new SDTS_SUBMODULE_MASTER
                {
                    obj_id = newId.ToString(),
                    company_code = vm.CompanyCode,
                    branch_code = vm.BranchCode,
                    SubModuleName = vm.Name,
                    obj_status = "O",
                    AngularUIRoute = vm.UIRoute,
                    Class = vm.Class,
                    Icon = vm.Icon,
                    Label = vm.Label,
                    LabelClass = vm.LabelClass,
                    ModuleSqNo = vm.SortOrder,
                    ModuleID = vm.ModuleID,
                    flag = "N",
                    IsAutoApp = vm.AutoApprove == true ? "Y" : "N",
                    FormType = vm.FormType,
                    ReportApiRoute = vm.ReportApiRoute,
                    ReportServerType = vm.ReportServerType,
                    UpdateOn = SIGlobals.Globals.GetDateTime()
                };
                db.SDTS_SUBMODULE_MASTER.Add(rm);
                #endregion

                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        public bool Modify(SubMenuViewModel vm, string userID, out ErrorVM error)
        {
            error = null;
            try {
                #region Validation
                string subModuleId = vm.ID.ToString();
                var subModuleToUpdate = db.SDTS_SUBMODULE_MASTER.Where(x => x.company_code == vm.CompanyCode && x.branch_code == vm.BranchCode
                    && x.obj_id == subModuleId).FirstOrDefault();
                if (subModuleToUpdate == null) {
                    error = new ErrorVM { description = "Module Id " + vm.ID + " doesn't exist exist." };
                    return false;
                }
                if (subModuleToUpdate.SubModuleName != vm.Name) {
                    var existingRole = db.SDTS_SUBMODULE_MASTER.Where(x => x.company_code == vm.CompanyCode && x.branch_code == vm.BranchCode
                       && x.SubModuleName == vm.Name).FirstOrDefault();
                    if (existingRole != null) {
                        error = new ErrorVM { description = "Module name " + vm.Name + " you are updating already exists. Please try with different role name." };
                        return false;
                    }
                }
                #endregion

                #region Module Master

                subModuleToUpdate.SubModuleName = vm.Name;
                subModuleToUpdate.obj_status = vm.Status == "Active" ? "O" : "C";
                subModuleToUpdate.AngularUIRoute = vm.UIRoute;
                subModuleToUpdate.Class = vm.Class;
                subModuleToUpdate.Icon = vm.Icon;
                subModuleToUpdate.Label = vm.Label;
                subModuleToUpdate.LabelClass = vm.LabelClass;
                subModuleToUpdate.ModuleSqNo = vm.SortOrder;
                subModuleToUpdate.UpdateOn = SIGlobals.Globals.GetDateTime();
                subModuleToUpdate.ModuleID = vm.ModuleID;
                subModuleToUpdate.FormType = vm.FormType;
                subModuleToUpdate.IsAutoApp = vm.AutoApprove == true ? "Y" : "N";
                subModuleToUpdate.ReportApiRoute = vm.ReportApiRoute;
                subModuleToUpdate.ReportServerType = vm.ReportServerType;
                db.Entry(subModuleToUpdate).State = System.Data.Entity.EntityState.Modified;
                #endregion

                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        public bool Open(string companyCode, string branchCode, string id, string userID, out ErrorVM error)
        {
            error = null;
            try {
                #region Validations
                var sbMaster = db.SDTS_SUBMODULE_MASTER.Where(x => x.company_code == companyCode
                       && x.branch_code == branchCode && x.obj_id == id).FirstOrDefault();
                if (sbMaster == null) {
                    error = new ErrorVM { description = "No details found for the ID: " + id };
                    return false;
                }
                if (sbMaster.obj_status == "O") {
                    error = new ErrorVM { description = string.Format($"The id {id} is already active and therefore it can be activated again.") };
                    return false;
                }
                #endregion

                sbMaster.obj_status = "O";
                sbMaster.UpdateOn = SIGlobals.Globals.GetDateTime();
                db.Entry(sbMaster).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        public bool Close(string companyCode, string branchCode, string id, string userID, out ErrorVM error)
        {
            error = null;
            try {
                #region Validations
                var sbMaster = db.SDTS_SUBMODULE_MASTER.Where(x => x.company_code == companyCode
                       && x.branch_code == branchCode && x.obj_id == id).FirstOrDefault();
                if (sbMaster == null) {
                    error = new ErrorVM { description = "No details found for the ID: " + id };
                    return false;
                }
                if (sbMaster.obj_status == "C") {
                    error = new ErrorVM { description = string.Format($"The id {id} is already de-active and therefore it can be de-activated again.") };
                    return false;
                }
                #endregion

                sbMaster.obj_status = "C";
                sbMaster.UpdateOn = SIGlobals.Globals.GetDateTime();
                db.Entry(sbMaster).State = System.Data.Entity.EntityState.Modified;
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
