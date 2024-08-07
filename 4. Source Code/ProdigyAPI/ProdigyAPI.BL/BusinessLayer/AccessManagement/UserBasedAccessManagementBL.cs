using ProdigyAPI.BL.ViewModel.AccessManagement;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Misc;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.AccessManagement
{
    public class UserBasedAccessManagementBL
    {
        MagnaDbEntities db = new MagnaDbEntities(true);
        public List<GenericList> GetUsers(string companyCode, string branchCode)
        {
            var q = from r in db.SDTU_OPERATOR_MASTER
                    orderby r.OperatorName
                    where r.company_code == companyCode && r.branch_code == branchCode
                        && r.object_status == "O"
                    select new GenericList { ID = r.employee_no, Name = r.OperatorName, Active = true, Code = r.OperatorCode };
            return q.ToList();
        }

        public List<ElevatedPermissionUserMap> List(string companyCode, string branchCode)
        {
            var q = from epr in db.UserElevatedPermissionMappings
                    join ep in db.ElevatedPermissions on epr.ElevatedPermissionID equals ep.ID
                    join r in db.SDTU_OPERATOR_MASTER on epr.UserCode equals r.OperatorCode
                    orderby ep.ID, r.OperatorName
                    where r.object_status == "O"
                        && r.company_code == companyCode && r.branch_code == branchCode //Can be removed on db consolidation                        
                    select new ElevatedPermissionUserMap
                    {
                        PermissionID = ep.ID,
                        PermissionName = ep.Description,
                        UserCode = r.OperatorCode,
                        UserName = r.OperatorName,
                        Active = epr.IsActive
                    };
            return q.ToList();

        }

        public bool Post(ElevatedPermissionUserMap epUserMap, string userId, out ErrorVM error)
        {
            error = null;
            try {
                var exists = db.UserElevatedPermissionMappings.Where(x => x.UserCode == epUserMap.UserCode
                        && x.ElevatedPermissionID == epUserMap.PermissionID).FirstOrDefault();
                if (exists != null) {
                    error = new ErrorVM
                    {
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                        description = "The permission already exists.",
                        customDescription = "The permission already exists."
                    };
                    return false;
                }
                UserElevatedPermissionMapping rep = new UserElevatedPermissionMapping
                {
                    ElevatedPermissionID = epUserMap.PermissionID,
                    UserCode = epUserMap.UserCode,
                    IsActive = true,
                    InsertedOn = SIGlobals.Globals.GetDateTime(),
                    InsertedBy = userId
                };
                db.UserElevatedPermissionMappings.Add(rep);
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

        public bool Activate(ElevatedPermissionUserMap epUserMap, string userId, out ErrorVM error)
        {
            error = null;
            try {
                var rep = db.UserElevatedPermissionMappings.Where(x => x.UserCode == epUserMap.UserCode
                        && x.ElevatedPermissionID == epUserMap.PermissionID).FirstOrDefault();
                if (rep == null) {
                    error = new ErrorVM
                    {
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                        description = "The permission does not exist",
                        customDescription = "The permission does not exist"
                    };
                    return false;
                }
                if (rep.IsActive == true) {
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
                    description = "Failed to activate.",
                    customDescription = ex.Message,
                };
                if (ex.InnerException != null)
                    error.InnerException = ex.InnerException.Message;
                error.field = ex.GetType().ToString();
                return false;
            }
            return true;
        }

        public bool DeActivate(ElevatedPermissionUserMap epUserMap, string userId,  out ErrorVM error)
        {
            error = null;
            try {
                var rep = db.UserElevatedPermissionMappings.Where(x => x.UserCode == epUserMap.UserCode
                        && x.ElevatedPermissionID == epUserMap.PermissionID).FirstOrDefault();
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
