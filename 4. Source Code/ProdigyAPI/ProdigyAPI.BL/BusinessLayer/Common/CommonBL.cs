using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Common
{
    public class CommonBL
    {
        public bool SendSMS(string message, string mobileNo, int smsID)
        {
            try {
                if (string.IsNullOrEmpty(mobileNo) || string.IsNullOrEmpty(message))
                    return false;
                string resp = string.Empty;
                string smsURL = ConfigurationManager.AppSettings["SMSHTTPUrl"].ToString();
                string path = string.Format(smsURL, mobileNo, Uri.EscapeDataString(message));
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                WebRequest request = WebRequest.Create(path);
                request.Method = "POST"; //Instead of POST, GET can be used. When GET is used no need to specify the ContentLength property.
                request.ContentLength = 0; //Credits from StackOverflow http://stackoverflow.com/questions/18352190/why-i-get-411-length-required-error and http://robertgreiner.com/2013/01/the-remote-server-returned-an-error-411-length-required/
                using (Stream stream = request.GetResponse().GetResponseStream()) {
                    using (StreamReader sr = new StreamReader(stream)) {
                        resp = sr.ReadToEnd();
                        UpdateSMSStatus(smsID, true, resp);
                    }
                }
            }
            catch (Exception ex) {
                //new Common().LogApplicationError(ex);
                if (ex.InnerException != null)
                    UpdateSMSStatus(smsID, false, ex.InnerException.StackTrace);
                else
                    UpdateSMSStatus(smsID, false, ex.StackTrace);
            }
            return true;
        }

        private void UpdateSMSStatus(int smsID, bool status, string responseMessage)
        {
            try {
                using (MagnaDbEntities db = new MagnaDbEntities()) {
                    var smsLog = db.SMSLogs.Where(x => x.ID == smsID).FirstOrDefault();
                    smsLog.GatewayStatus = LeftN(responseMessage, 1999);
                    smsLog.IsMessageSent = status;
                    db.Entry(smsLog).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch  {
                //new Common().LogApplicationError(ex);
            }
        }
        public string LeftN(string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            int lengthToTruncate = maxLength;
            if (str.Length < maxLength)
                lengthToTruncate = str.Length;
            return str.Substring(0, lengthToTruncate);
        }

        public ErrorVM CommonErrorHandler(Exception exception)
        {
            ErrorVM error = new ErrorVM();
            switch ((exception.GetType().Name)) {
                case "System.NullReferenceException":
                    break;
                case "System.IndexOutOfRangeException":
                    break;
                case "System.IO.IOException":
                    break;
                case "System.Net.WebException":
                    break;
                case "System.Data.SqlClient.SqlException":
                    break;
                case "System.OutOfMemoryException":
                    break;
                case "System.InvalidCastException":
                    break;
                case "System.InvalidOperationException":
                    break;
                case "System.ObjectDisposedException":
                    break;
                case "EntityCommandExecutionException":
                    break;
                default:
                    break;
            }
            return error;
        }
        /// <summary>
        /// Use this method to post the transaction log
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="documentyType">1 for sales invoice, 2 purchase invoice, 3 for SR confirmation, 4 for order, 5 for repair</param>
        /// <param name="documentNo">Document number, bill number</param>
        /// <param name="postingDate">Document date</param>
        /// <param name="userID">Operator Code</param>
        public void PostDocumentCreation(string companyCode, string branchCode, int documentType, int documentNo, DateTime postingDate, string userID)
        {
            try {
                MagnaDbEntities dbContext = new MagnaDbEntities(true);
                DocumentCreationLog doc = new DocumentCreationLog()
                {
                    ApplicationID = 1,
                    CompanyCode = companyCode,
                    BranchCode = branchCode,
                    DocumentDate = postingDate,
                    PostingDate = DateTime.Now,
                    DocumentNo = documentNo.ToString(),
                    OperatorCode = userID,
                    TransTypeID = documentType
                };
                dbContext.DocumentCreationLogs.Add(doc);
                dbContext.SaveChanges();
            }
            catch (Exception) {
                //Do nothing, this is not a transaction entry
            }


        }

        public ErrorVM HandleAccountPostingProcs(ObjectParameter outputVal, ObjectParameter errorMessage)
        {
            ErrorVM error = null;
            string errorFromProc = string.Empty;
            if (outputVal != null) {
                if (outputVal.Value != null) {
                    var procResult = outputVal.Value;
                    if (errorMessage != null)
                        errorFromProc = errorMessage.Value.ToString();
                    if (Convert.ToInt32(procResult) != 0) {
                        error = new ErrorVM()
                        {
                            description = "Error Occurred while Updating Accounts. " + errorFromProc,
                            field = "Account Update",
                            index = 0,
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                    }
                }
            }
            return error;
        }
    }
}
