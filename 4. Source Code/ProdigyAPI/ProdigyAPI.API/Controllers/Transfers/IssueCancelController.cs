using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.BusinessLayer.Issues;
using ProdigyAPI.Handlers;
using System.Web.Http.Description;

namespace ProdigyAPI.Controllers.Transfers
{
    /// <summary>
    /// API to cancel the issues documents. Use this for issue transactions other than Barcoded Issue and Non Tag issue.
    /// </summary>
    [RoutePrefix("api/Transfers/issue-cancel")]
    public class IssueCancelController : SIBaseApiController<IssueCancelBL>
    {
        /// <summary>
        /// Get Issue numbers to cancel. issueDate parameter can be passed as null if you wish to load all issue number.
        /// If the issueDate parameter is sent, issues related to issueDate are loaded.
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="issueType">Issue Type. Can be: IO for OPG Segregation, IL for melting issue, 
        /// IB for branch issue, IM for manufacturing. Refer IRSetup master for details of codes</param>
        /// <param name="issueDate">Issue Date. null can be passed to retrieve all issue number. It is recommended to pass date.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-issue-numbers")]
        [Route("get-issue-numbers/{companyCode}/{branchCode}/{issueType}/{issueDate}")]
        [ResponseType(typeof(List<ListOfValue>))]
        public IHttpActionResult GetIssueToList(string companyCode, string branchCode, string issueType, DateTime? issueDate = null)
        {
            ErrorVM error = null;
            var result = new IssueCancelBL().GetIssueToCancel(companyCode, branchCode, issueType, out error, issueDate);
            if (result != null)
                return Ok(result);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to load issue-numbers. " });
            }
        }

        /// <summary>
        /// Cancel the issue
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="issueType">Issue Type. Can be: IO for OPG Segregation, IL for melting issue, 
        /// IB for branch issue, IM for manufacturing. Refer IRSetup master for details of codes</param>
        /// <param name="issueNo">Issue number to cancel</param>
        /// <param name="cancelRemarks">Cancellation remarks</param>
        /// <returns></returns>
        [HttpPost]
        [Route("cancel")]
        public IHttpActionResult CancelIssue(string companyCode, string branchCode, string issueType, int issueNo, string cancelRemarks)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            bool success = new IssueCancelBL().CancelIssue(companyCode, branchCode, issueType, issueNo, cancelRemarks, userID, out error);
            if (success) {
                return Ok();
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }
    }
}
