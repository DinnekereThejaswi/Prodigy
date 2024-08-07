using ProdigyAPI.BL.BusinessLayer.UserManagement;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.UserManagement;
using ProdigyAPI.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace ProdigyAPI.Controllers.UserManagement
{
    /// <summary>
    /// Set of APIs for user management like addition, updation, password change, activate, de-active the user.
    /// </summary>
    [RoutePrefix("api/rights-management/users")]
    [Authorize]
    public class UsersController : SIBaseApiController<UserManagementBL>
    {
        /// <summary>
        /// Gets the user list
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        [Route("list/{companyCode}/{branchCode}")]
        [ResponseType(typeof(List<OperatorViewModel>))]
        public IHttpActionResult GetIssueToList(string companyCode, string branchCode)
        {
            ErrorVM error = null;
            var usersList = new UserManagementBL().List(companyCode, branchCode, out error);
            if (usersList != null)
                return Ok(usersList);
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Gets an user
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="userID">Operator or User ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get")]
        [Route("get/{companyCode}/{branchCode}/{userID}")]
        [ResponseType(typeof(OperatorViewModel))]
        public IHttpActionResult GetUser(string companyCode, string branchCode, string userID)
        {
            ErrorVM error = null;
            var user = new UserManagementBL().Get(companyCode, branchCode, userID, out error);
            if (user != null)
                return Ok(user);
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Posts a new user. Ensure that the password is base64 encoded and meets the password requirements.
        /// The allowed values for Status is one of these: Active or Closed
        /// OperatorType must either be: Admin or Others
        /// DefaultStore is the store to which the user can login by default.
        /// MappedStores are the list of stores to which the user is mapped. When adding a new user, ensure that the
        /// store which creates the user must be one in the mappedstores
        /// </summary>
        /// <param name="user">User object</param>
        /// <returns></returns>
        [HttpPost]
        [Route("post")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult Add(OperatorViewModel user)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            bool success = new UserManagementBL().Add(user, userID, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = user.OperatorCode,
                    Message = "New user " + user.OperatorCode + " has been created successfully."
                };
                return Ok(docCreation);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// Updates an existing user. Password must be base64 encoded and meets the password requirements.
        /// Only active users can be modified.
        /// The allowed values for Status is one of these: Active or Closed
        /// OperatorType must either be: Admin or Others.
        /// DefaultStore is the store to which the user can login by default.
        /// MappedStores are the list of stores to which the user is mapped. When adding a new user, ensure that the
        /// store which creates the user must be one in the mappedstores
        /// </summary>
        /// <param name="user">User object</param>
        /// <returns></returns>
        [HttpPut]
        [Route("put")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult Update(OperatorViewModel user)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            bool success = new UserManagementBL().Update(user, userID, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = user.OperatorCode,
                    Message = "User detail for user " + user.OperatorCode + " has been modified successfully."
                };
                return Ok(docCreation);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }
        
        /// <summary>
        /// Use this API to change users password. The password must be base64 encoded.
        /// </summary>
        /// <param name="passwordChangeViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("change-password")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult ChangePassword(OperatorPasswordViewModel passwordChangeViewModel)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();            
            bool success = new UserManagementBL().ChangePasword(passwordChangeViewModel, userID, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = userID,
                    Message = "Password has been changed successfully for user " + userID
                };
                return Ok(docCreation);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// Activate/open the user. 
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="userID">UserId or operator code</param>
        /// <returns></returns>
        [HttpPost]
        [Route("activate")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult ActivateUser(string companyCode, string branchCode, string userID)
        {
            ErrorVM error = null;
            string authUser = base.GetUserId();
            bool success = new UserManagementBL().Open(companyCode, branchCode, userID, authUser, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = userID,
                    Message = "The user " + userID + " has been activated successfully."
                };
                return Ok(docCreation);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// De-activate or close the user. 
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="userID">UserId or operator code</param>
        /// <returns></returns>
        [HttpPost]
        [Route("de-activate")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult DeActivateUser(string companyCode, string branchCode, string userID)
        {
            ErrorVM error = null;
            string authUser = base.GetUserId();
            bool success = new UserManagementBL().Close(companyCode, branchCode, userID, authUser, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = userID,
                    Message = "The user " + userID + " has been de-activated/closed successfully."
                };
                return Ok(docCreation);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// Get the list of stores/branches
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("branches")]
        [Route("branches/{companyCode}/{branchCode}")]
        [ResponseType(typeof(List<OperatorViewModel>))]
        public IHttpActionResult GetBranchList(string companyCode, string branchCode)
        {
            ErrorVM error = null;
            var data = new UserManagementBL().GetBranches(companyCode, branchCode, out error);
            if (data != null)
                return Ok(data);
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }
    }
}
