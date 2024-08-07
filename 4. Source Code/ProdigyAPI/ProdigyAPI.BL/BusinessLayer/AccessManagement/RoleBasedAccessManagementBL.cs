using ProdigyAPI.BL.ViewModel.AccessManagement;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Misc;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace ProdigyAPI.BL.BusinessLayer.AccessManagement
{
    public class RoleBasedAccessManagementBL
    {
        MagnaDbEntities db = new MagnaDbEntities(true);

        public List<GenericList> GetPermissions()
        {
            var q = from ep in db.ElevatedPermissions
                    select new GenericList{ ID = ep.ID, Name = ep.Description, Active = true, Code = ep.Code };
            return q.ToList();
        }

        public List<GenericList> GetRoles(string companyCode, string branchCode)
        {
            var q = from  r in db.SDTU_ROLE_MASTER
                    orderby r.RoleID
                    where r.company_code == companyCode && r.branch_code == branchCode
                        && r.object_status == "O"
                    select new GenericList { ID = r.RoleID, Name = r.RoleName, Active = true, Code = r.RoleName };
            return q.ToList();
        }

        public List<ElevatedPermissionRoleMap> List(string companyCode, string branchCode)
        {
            var q = from epr in db.RoleElevatedPermissionMappings
                    join ep in db.ElevatedPermissions on epr.ElevatedPermissionID equals ep.ID
                    join r in db.SDTU_ROLE_MASTER on epr.RoleID equals r.RoleID
                    orderby ep.ID, r.RoleID
                    where r.object_status == "O"
                        &&r.company_code == companyCode && r.branch_code == branchCode //Can be removed on db consolidation                        
                    select new ElevatedPermissionRoleMap
                    {
                        PermissionID = ep.ID,
                        PermissionName = ep.Description,
                        RoleID = r.RoleID,
                        RoleName = r.RoleName,
                        Active = epr.IsActive
                    };
            return q.ToList();
                
        }

        public bool Post(ElevatedPermissionRoleMap elevatedPermissionRoleMap, string userId, out ErrorVM error)
        {
            error = null;
            try {
                var exists = db.RoleElevatedPermissionMappings.Where(x => x.RoleID == elevatedPermissionRoleMap.RoleID
                        && x.ElevatedPermissionID == elevatedPermissionRoleMap.PermissionID).FirstOrDefault();
                if(exists != null) {
                    error = new ErrorVM
                    {
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                        description = "The permission already exists.",
                        customDescription = "The permission already exists."
                    };
                    return false;
                }
                RoleElevatedPermissionMapping rep = new RoleElevatedPermissionMapping
                {
                    ElevatedPermissionID = elevatedPermissionRoleMap.PermissionID,
                    RoleID = elevatedPermissionRoleMap.RoleID,
                    IsActive = true,
                    InsertedOn = SIGlobals.Globals.GetDateTime(),
                    InsertedBy = userId
                };
                db.RoleElevatedPermissionMappings.Add(rep);
                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM
                {
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                    description = "Failed to post.",
                    customDescription = ex.Message,
                };
                if (ex.InnerException != null)
                    error.InnerException = ex.InnerException.Message;
                error.field = ex.GetType().ToString();
                return false;
            }
            return true;
        }

        public bool Activate(ElevatedPermissionRoleMap elevatedPermissionRoleMap, string userId, out ErrorVM error)
        {
            error = null;
            try {
                var rep = db.RoleElevatedPermissionMappings.Where(x => x.RoleID == elevatedPermissionRoleMap.RoleID
                    && x.ElevatedPermissionID == elevatedPermissionRoleMap.PermissionID).FirstOrDefault();
                if (rep == null) {
                    error = new ErrorVM
                    {
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                        description = "The permission does not exist",
                        customDescription = "The permission does not exist"
                    };
                    return false;
                }
                if(rep.IsActive == true) {
                    error = new ErrorVM
                    {
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                        description = "The permission is already activated.",
                        customDescription = "The permission is already activated."
                    };
                    return false;
                }
                rep.IsActive = true;
                rep.UpdatedBy = userId;
                rep.UpdatedOn = SIGlobals.Globals.GetDateTime();
                db.Entry(rep).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM
                {
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                    description = "Failed to Activate.",
                    customDescription = ex.Message,
                };
                if (ex.InnerException != null)
                    error.InnerException = ex.InnerException.Message;
                error.field = ex.GetType().ToString();
                return false;
            }
            return true;
        }

        public bool DeActivate(ElevatedPermissionRoleMap elevatedPermissionRoleMap, string userId, out ErrorVM error)
        {
            error = null;
            try {
                var rep = db.RoleElevatedPermissionMappings.Where(x => x.RoleID == elevatedPermissionRoleMap.RoleID
                    && x.ElevatedPermissionID == elevatedPermissionRoleMap.PermissionID).FirstOrDefault();
                if (rep == null) {
                    error = new ErrorVM
                    {
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                        description = "The permission does not exist",
                        customDescription = "The permission does not exist"
                    };
                    return false;
                }
                if (rep.IsActive == false) {
                    error = new ErrorVM
                    {
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                        description = "The permission is already in De-activation state.",
                        customDescription = "TThe permission is already in De-activation state."
                    };
                    return false;
                }
                rep.IsActive = false;
                rep.UpdatedBy = userId;
                rep.UpdatedOn = SIGlobals.Globals.GetDateTime();
                db.Entry(rep).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM
                {
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                    description = "Failed to De-activate.",
                    customDescription = ex.Message,
                };
                if (ex.InnerException != null)
                    error.InnerException = ex.InnerException.Message;
                error.field = ex.GetType().ToString();
                return false;
            }
            return true;
        }

    }
}
