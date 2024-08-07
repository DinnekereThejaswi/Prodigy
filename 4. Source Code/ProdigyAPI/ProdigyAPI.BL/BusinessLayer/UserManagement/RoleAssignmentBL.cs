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
    public class RoleAssignmentBL
    {
        MagnaDbEntities db = null;

        public RoleAssignmentBL()
        {
            db = new MagnaDbEntities(true);
        }

        public RoleAssignmentBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }

        public RoleAssignmentViewModel GetRoleAssignment(string companyCode, string branchCode, int roleID, out ErrorVM error)
        {
            error = null;
            RoleAssignmentViewModel roleAssignment = new RoleAssignmentViewModel
            {
                CompanyCode = companyCode,
                BranchCode = branchCode,
                RoleID = roleID,
                Modules = new List<ModuleViewModel>()
            };
           
            try {
                if (roleID > 0) {
                    var roleMaster = db.SDTU_ROLE_MASTER.Where(x => x.company_code == companyCode
                        && x.branch_code == branchCode && x.RoleID == roleID).FirstOrDefault();
                    if(roleMaster == null) {
                        error = new ErrorVM {description = string.Format($"No details for the role: {roleID} ") };
                        return null;
                    }
                    roleAssignment.RoleName = roleMaster.RoleName;
                }
                //I am a performance geek, I don't like database round trips. Therefore I'll get the data and user linq
                //to get what I want. If the relationships were proper between the SDTS_MODULE_MASTER, SDTS_SUBMODULE_MASTER
                //,SDTS_ROLEACTIONS_ROLE table, I would have done this using joins making database server responsible.
                var moduleMaster = db.SDTS_MODULE_MASTER.Where(x => x.company_code == companyCode
                    && x.branch_code == branchCode && x.obj_status == "O").OrderBy(o => o.seq_no).ToList();
                var subModuleMaster = db.SDTS_SUBMODULE_MASTER.Where(x => x.company_code == companyCode
                    && x.branch_code == branchCode && x.obj_status == "O").OrderBy(o => o.obj_id).ToList();
                var roleFunctions = db.SDTS_ROLEACTIONS_ROLE.Where(x => x.company_code == companyCode
                    && x.branch_code == branchCode && x.RoleID == roleID).ToList();
                var roleActions = roleFunctions
                    .Select(d => new { RoleID = d.RoleID, SubModuleID = d.SubModuleId }).Distinct().ToList();
                //roleFunctions = db.SDTS_ROLEACTIONS_ROLE.Where(x => x.company_code == companyCode
                //    && x.branch_code == branchCode && x.RoleID == roleID).ToList();
                if (moduleMaster == null)
                    return roleAssignment;
                #region Simplistic approach to achieve the same result
                //foreach (var mm in moduleMaster) {
                //    ModuleViewModel module = new ModuleViewModel
                //    {
                //        ID = mm.obj_id,
                //        Name = mm.ModuleName,
                //        SortOrder = Convert.ToInt32(mm.seq_no),
                //        SubModules = new List<ModuleViewModel>()
                //    };
                //    var subModuleList = subModuleMaster.Where(x => x.ModuleID == mm.obj_id).ToList();
                //    if (subModuleList == null)
                //        continue;
                //        foreach (var s in subModuleList) {
                //            var sm = new ModuleViewModel
                //            {
                //                ID = Convert.ToInt32(s.obj_id), Name = s.SubModuleName, SortOrder = s.ModuleSqNo,
                //                Checked = roleActions.Where(ra => ra.RoleID == roleID && ra.SubModuleID == Convert.ToInt32(s.obj_id)).FirstOrDefault() != null ? true : false
                //            };
                //            module.SubModules.Add(sm);
                //        }
                //    roleAssignment.Modules.Add(module);
                //} 
                #endregion

                #region Harness the power of Linq & achieve the same result.
                foreach (var mm in moduleMaster) {
                    ModuleViewModel module = new ModuleViewModel
                    {
                        ID = mm.obj_id,
                        Name = mm.ModuleName,
                        SortOrder = Convert.ToInt32(mm.seq_no),
                        SubModules =
                            subModuleMaster.Where(x => x.ModuleID == mm.obj_id).Select(s => new ModuleViewModel
                            {
                                ID = Convert.ToInt32(s.obj_id),
                                Name = s.SubModuleName,
                                SortOrder = s.ModuleSqNo,
                                Functions = GetFunctions(roleFunctions.Where(rf => rf.RoleID == roleID && rf.SubModuleId == Convert.ToInt32(s.obj_id)).ToList()),
                                Checked = roleActions.Where(ra => ra.RoleID == roleID && ra.SubModuleID == Convert.ToInt32(s.obj_id)).FirstOrDefault() != null ? true : false
                            }).ToList()
                    };
                    roleAssignment.Modules.Add(module);
                } 
                #endregion

            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return null;
            }
            return roleAssignment;
        }

        private MuduleActionViewModel GetFunctions(List<SDTS_ROLEACTIONS_ROLE> roleFunctions)
        {
            MuduleActionViewModel mf = new MuduleActionViewModel { };
            foreach(var rf in roleFunctions) {
                switch (rf.FunctionId) {
                    case 1:
                        mf.AddEnabled = true;
                        break;
                    case 2:
                        mf.EditEnabled = true;
                        break;
                    case 3:
                        mf.DeleteEnabled = true;
                        break;
                    case 4:
                        mf.ActivateEnabled = true;
                        break;
                    case 5:
                        mf.DeActivateEnabled = true;
                        break;
                    case 6:
                        mf.ViewEnabled = true;
                        break;
                    case 7:
                        mf.PrintEnabled = true;
                        break;
                    default:
                        break;
                }
            }
            return mf;
        }
        public bool Post(RoleAssignmentViewModel roleAssignment, string userID, out ErrorVM error)
        {
            error = null;
            try {
                if (roleAssignment == null) {
                    error = new ErrorVM { description = "There is nothing to post." };
                    return false;
                }
                if (roleAssignment.RoleID <= 0) {
                    error = new ErrorVM { description = "Invalid role ID." };
                    return false;
                }
                var roleMaster = db.SDTU_ROLE_MASTER.Where(x => x.company_code == roleAssignment.CompanyCode
                            && x.branch_code == roleAssignment.BranchCode && x.RoleID == roleAssignment.RoleID).FirstOrDefault();
                if (roleMaster == null) {
                    error = new ErrorVM { description = string.Format($"No details for the role: {roleAssignment.RoleID} ") };
                    return false;
                }

                var roleActions = db.SDTS_ROLEACTIONS_ROLE.Where(x => x.company_code == roleAssignment.CompanyCode
                    && x.branch_code == roleAssignment.BranchCode && x.RoleID == roleAssignment.RoleID).ToList();
                if (roleActions != null && roleActions.Count > 0) {
                    db.SDTS_ROLEACTIONS_ROLE.RemoveRange(roleActions);
                }

                foreach (var module in roleAssignment.Modules) {
                    foreach (var sm in module.SubModules) {
                        if (sm.Checked == true) {
                            AssignRole(roleAssignment.CompanyCode, roleAssignment.BranchCode, roleAssignment.RoleID, sm);
                        }
                    }
                }
                db.SaveChanges();
                SIGlobals.Globals.WriteTransactionLog(userID,
                    string.Format($"Role permissions for role {roleAssignment.RoleName} updated."), null, db);
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }

            return true;
        }

        private void AssignRole(string companyCode, string branchCode, int roleID, ModuleViewModel sm)
        {
            DateTime timestamp = SIGlobals.Globals.GetDateTime();
            int functionId = 0;

            //Is there any other better way than this repetitive coding? I'm leaving it to your luck!
            if (sm.Functions.AddEnabled) {
                functionId = 1;
                AddRoleObject(companyCode, branchCode, roleID, functionId, sm, timestamp);
            }
            if (sm.Functions.EditEnabled) {
                functionId = 2;
                AddRoleObject(companyCode, branchCode, roleID, functionId, sm, timestamp);
            }
            if (sm.Functions.DeleteEnabled) {
                functionId = 3;
                AddRoleObject(companyCode, branchCode, roleID, functionId, sm, timestamp);
            }
            if (sm.Functions.ActivateEnabled) {
                functionId = 4;
                AddRoleObject(companyCode, branchCode, roleID, functionId, sm, timestamp);
            }
            if (sm.Functions.DeActivateEnabled) {
                functionId = 5;
                AddRoleObject(companyCode, branchCode, roleID, functionId, sm, timestamp);
            }
            if (sm.Functions.ViewEnabled) {
                functionId = 6;
                AddRoleObject(companyCode, branchCode, roleID, functionId, sm, timestamp);
            }
            if (sm.Functions.PrintEnabled) {
                functionId = 7;
                AddRoleObject(companyCode, branchCode, roleID, functionId, sm, timestamp);
            }
        }

        private void AddRoleObject(string companyCode, string branchCode, int roleID, int functionId, ModuleViewModel sm, DateTime timeStamp)
        {
            string objId = SIGlobals.Globals.GetMagnaGUID(new string[] { "SDTS_ROLEACTIONS_ROLE", roleID.ToString(), sm.ID.ToString(), functionId.ToString() },
                    companyCode, branchCode);
            SDTS_ROLEACTIONS_ROLE ra = new SDTS_ROLEACTIONS_ROLE
            {
                obj_id = objId,
                FunctionId = functionId,
                company_code = companyCode,
                branch_code = branchCode,
                RoleID = roleID,
                flag = "N",
                SubModuleId = sm.ID,
                UpdateOn = timeStamp
            };
            db.SDTS_ROLEACTIONS_ROLE.Add(ra);
        }
    }
}
