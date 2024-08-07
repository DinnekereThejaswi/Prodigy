using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace ProdigyAPI.Utils
{
    public class IPAddress4Validation
    {
        MagnaDbEntities db = new MagnaDbEntities(true);

        public string GetClientIPAddress(HttpRequestMessage request)
        {
            string ipAddress = ((System.Web.HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            return ipAddress;
            #region RemoteEndpointMessageProperty is not available, so this is commented
            //if (request.Properties.ContainsKey("MS_HttpContext")) {
            //    return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            //}
            //else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name)) {
            //    RemoteEndpointMessageProperty prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
            //    return prop.Address;
            //}
            //else if (HttpContext.Current != null) {
            //    return HttpContext.Current.Request.UserHostAddress;
            //}
            //else {
            //    return null;
            //} 
            #endregion
        }
        public bool ValidateIPAddress(string ipAddress, out string errorMessage)
        {
            errorMessage = string.Empty;
            var validateIPAddress = ConfigurationManager.AppSettings["ValidateIPAddress"].ToString().ToUpper() == "TRUE" ? true : false;
            if (!validateIPAddress)
                return true;
            if (string.IsNullOrEmpty(ipAddress))
                return true;
            if (ipAddress == "::1" || ipAddress == "127.0.0.1") {
                return true;
            }
            var ipList = db.IPSettings.Where(x => x.IsActive == true).ToList();
            var allowedIPs = ipList.Where(x => x.AllowDenyFlag == "A").ToList();
            var deniedIPs = ipList.Where(x => x.AllowDenyFlag == "D").ToList();
            if (ipList != null) {
                if (IsIPAddressDenied(deniedIPs, ipAddress)) {
                    errorMessage = "Access is denied to your IP address " + ipAddress + ".";
                    return false;
                }
                if (!IsIPAddressAllowed(allowedIPs, ipAddress)) {
                    errorMessage = "Your IP address " + ipAddress + " is not authorized. Please contact technical team.";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// For this method, the IP address is sent from the UI
        /// </summary>
        /// <param name="ipAddress">IP Address sent by the UI</param>
        /// <param name="errorMessage">Validation error</param>
        /// <returns>true if successfull, else false</returns>
        public bool ValidateStaticIPAddress(string ipAddress, out string errorMessage)
        {
            errorMessage = string.Empty;
            var validateIPAddress = ConfigurationManager.AppSettings["ValidateStaticIPAddress"].ToString().ToUpper() == "TRUE" ? true : false;
            if (!validateIPAddress)
                return true;
            if (!SIGlobals.Globals.ValidateIPv4(ipAddress)) {
                errorMessage = "The IP address " + ipAddress + " is not a valid IP4 Address.";
                return false;
            }
            var ipList = db.IPSettings.Where(x => x.IsActive == true).ToList();
            var allowedIPs = ipList.Where(x => x.AllowDenyFlag == "A").ToList();
            var deniedIPs = ipList.Where(x => x.AllowDenyFlag == "D").ToList();
            if (ipList != null) {
                if (IsIPAddressDenied(deniedIPs, ipAddress)) {
                    errorMessage = "Access is denied to your IP address " + ipAddress + ".";
                    return false;
                }
                if (!IsIPAddressAllowed(allowedIPs, ipAddress)) {
                    errorMessage = "Your IP address " + ipAddress + " is not authorized. Please contact technical team.";
                    return false;
                }
            }

            return true;
        }

        private bool IsIPAddressAllowed(List<IPSetting> ipAddressList, string currentIp)
        {
            bool isValid = false;
            if (currentIp == "::1" || currentIp == "127.0.0.1") {
                return true;
            }

            //When allowed range is empty, means allow all!
            if (ipAddressList == null || ipAddressList.Count == 0)
                return true;
            foreach (var dr in ipAddressList) {
                string startIP = dr.FromIP;
                string endIP = dr.ToIP;
                bool isInRange = IsIPInRange(startIP, endIP, currentIp);
                if (isInRange) {
                    isValid = true;
                    break;
                }
            }
            return isValid;

        }
        private bool IsIPAddressDenied(List<IPSetting> ipAddressArray, string currentIp)
        {
            bool isValid = false;
            if (currentIp == "::1" || currentIp == "127.0.0.1") {
                return false;
            }
            //When denied range is empty, means none is denied
            if (ipAddressArray == null || ipAddressArray.Count == 0)
                return false;
            foreach (var dr in ipAddressArray) {
                string startIP = dr.FromIP;
                string endIP = dr.ToIP;
                bool isInRange = IsIPInRange(startIP, endIP, currentIp);
                if (isInRange) {
                    isValid = true;
                    break;
                }
            }
            return isValid;
        }

        private bool IsIPAddressDenied(string deniedIPRange, string currentIp)
        {
            bool isValid = false;
            if (currentIp == "::1" || currentIp == "127.0.0.1") {
                return false;
            }
            deniedIPRange = deniedIPRange.Trim().Replace(" ", "");
            //When denied range is empty, means none is denied
            if (deniedIPRange == null || deniedIPRange == "")
                return false;
            List<string> ipList = new List<string>(deniedIPRange.Split(','));
            foreach (string ipRange in ipList) {
                int indexOfMinus = ipRange.IndexOf("-");
                string startIP = ipRange.Substring(0, indexOfMinus);
                string endIP = ipRange.Substring(indexOfMinus + 1, ipRange.Length - indexOfMinus - 1);
                bool isInRange = IsIPInRange(startIP, endIP, currentIp);
                if (isInRange) {
                    isValid = true;
                    break;
                }
            }
            return isValid;
        }

        private bool IsIPInRange(string startIpAddr, string endIpAddr, string address)
        {
            long ipStart = BitConverter.ToInt32(IPAddress.Parse(startIpAddr).GetAddressBytes().Reverse().ToArray(), 0);
            long ipEnd = BitConverter.ToInt32(IPAddress.Parse(endIpAddr).GetAddressBytes().Reverse().ToArray(), 0);
            long ip = BitConverter.ToInt32(IPAddress.Parse(address).GetAddressBytes().Reverse().ToArray(), 0);
            bool b = (ip >= ipStart) && (ip <= ipEnd);
            return b;
        }
    }
}