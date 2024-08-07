using ProdigyAPI.BL.BusinessLayer.ErrorHandler;
using ProdigyAPI.BL.ViewModel.AccessManagement;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.AccessManagement
{
    public class IPAddressManagement
    {
        private MagnaDbEntities db = new MagnaDbEntities(true);
        public List<IPSettingsVM> ListAll(string allowDenyFlag, bool status)
        {
            List<IPSettingsVM> ipList = new List<IPSettingsVM>();
            if (allowDenyFlag == "Allow" || allowDenyFlag == "Deny" || allowDenyFlag == "All") {
                ;
            }
            else {
                throw new Exception("The AllowDeny state should either be Allow or Deny or All.");
            }
            var filterFlag = allowDenyFlag == "Allow" ? "A" : "D";
            var ipSettings = db.IPSettings.Where(x => x.IsActive == status);
            if (allowDenyFlag != "All")
                ipSettings = ipSettings.Where(m => m.AllowDenyFlag == filterFlag);
            if (ipSettings != null) {
                foreach (var i in ipSettings) {
                    IPSettingsVM ip = new IPSettingsVM
                    {
                        ID = i.ID,
                        FromIP = i.FromIP,
                        ToIP = i.ToIP,
                        AllowDeny = i.AllowDenyFlag == "A" ? "Allow" : "Deny",
                        Remarks = i.Remarks,
                        Active = i.IsActive,
                        BranchCode = i.BranchCode
                    };
                    ipList.Add(ip);
                }
            }
            return ipList;
        }

        public bool Add(IPSettingsVM ipAddress, string userID, out ErrorVM error)
        {
            error = null;
            #region Model Validations
            if (ipAddress == null) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Invalid data, the object is null." };
                return false;
            }
            bool isFromIPValidIP4 = SIGlobals.Globals.ValidateIPv4(ipAddress.FromIP);
            if (!isFromIPValidIP4) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "The From IP is not a valid IP4 IP Address." };
                return false;
            }
            bool isToIPValidIP4 = SIGlobals.Globals.ValidateIPv4(ipAddress.ToIP);
            if (!isToIPValidIP4) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "The To IP is not a valid IP4 IP Address." };
                return false;
            }
            if (ipAddress.AllowDeny == "Allow" || ipAddress.AllowDeny == "Deny") {
                ;
            }
            else {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "The AllowDeny state should either be Allow or Deny" };
                return false;
            } 
            #endregion
            try {
                ipAddress.AllowDeny = ipAddress.AllowDeny == "Allow" ? "A" : "D";
                IPSetting ipSetting = new IPSetting
                {
                    AllowDenyFlag = ipAddress.AllowDeny,
                    FromIP = ipAddress.FromIP,
                    ToIP = ipAddress.ToIP,
                    Remarks = ipAddress.Remarks,
                    InsertedOn = SIGlobals.Globals.GetDateTime(),
                    InsertedBy = userID,
                    IsActive = true,
                    BranchCode = ipAddress.BranchCode
                };
                db.IPSettings.Add(ipSetting);
                db.SaveChanges();
                ipAddress.ID = ipSetting.ID;
                ipAddress.AllowDeny = ipSetting.AllowDenyFlag == "A" ? "Allow" : "Deny";
            }
            catch (Exception ex) {
                var exMgr = new ExceptionErrorHandler().ManageException(ex);
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = exMgr.ToString(), customDescription = exMgr.FullErrorDetail() };
                return false;
            }
            return true;
        }

        public bool Modify(IPSettingsVM ipAddress, string userID, out ErrorVM error)
        {
            error = null;
            #region Model Validations
            if (ipAddress == null) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Invalid data, the object is null." };
                return false;
            }
            if (ipAddress.ID <= 0) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Invalid ID to update." };
                return false;
            }

            bool isFromIPValidIP4 = SIGlobals.Globals.ValidateIPv4(ipAddress.FromIP);
            if (!isFromIPValidIP4) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "The From IP is not a valid IP4 IP Address." };
                return false;
            }
            bool isToIPValidIP4 = SIGlobals.Globals.ValidateIPv4(ipAddress.ToIP);
            if (!isToIPValidIP4) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "The To IP is not a valid IP4 IP Address." };
                return false;
            }
            if (ipAddress.AllowDeny == "Allow" || ipAddress.AllowDeny == "Deny") {
                ;
            }
            else {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "The AllowDeny state should either be Allow or Deny" };
                return false;
            } 
            #endregion

            try {
                var ipSetting = db.IPSettings.Where(x => x.ID == ipAddress.ID).FirstOrDefault();
                if (ipSetting == null) {
                    error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "No record found." };
                    return false;
                }
                ipSetting.AllowDenyFlag = ipAddress.AllowDeny == "Allow" ? "A" : "D";
                ipSetting.FromIP = ipAddress.FromIP;
                ipSetting.ToIP = ipAddress.ToIP;
                ipSetting.Remarks = ipAddress.Remarks;
                ipSetting.UpdatedOn = SIGlobals.Globals.GetDateTime();
                ipSetting.UpdatedBy = userID;
                ipSetting.IsActive = ipAddress.Active;
                ipSetting.BranchCode = ipAddress.BranchCode;

                db.Entry(ipSetting).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex) {
                var exMgr = new ExceptionErrorHandler().ManageException(ex);
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = exMgr.ToString(), customDescription = exMgr.FullErrorDetail() };
                return false;
            }
            return true;
        }
    }
}
