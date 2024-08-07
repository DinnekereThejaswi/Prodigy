using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProdigyAPI.BL.ViewModel.OPGProcessing;
using ProdigyAPI.Handlers;
using ProdigyAPI.BL.BusinessLayer.OPGProcessing;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using System.Web.Http.Description;
using ProdigyAPI.BL.ViewModel.Print;

namespace ProdigyAPI.Controllers.OPGProcessing
{
    [RoutePrefix("api/Transfers/OPG-melting-issue")]
    public class OPGMeltingIssueController : SIBaseApiController<OPGMeltingIssueBL>
    {
        [HttpGet]
        [Route("get-issue-to")]
        [Route("get-issue-to/{companyCode}/{branchCode}")]
        [ResponseType(typeof(List<ListOfValue>))]
        public IHttpActionResult GetIssueToList(string companyCode, string branchCode)
        {
            var partyList = new OPGMeltingIssueBL().GetIssueToList(companyCode, branchCode);
            if (partyList != null)
                return Ok(partyList);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to load issue-to list" });
            }
        }

        [HttpGet]
        [Route("get-item-gs")]
        [Route("get-item-gs/{companyCode}/{branchCode}")]
        [ResponseType(typeof(List<ListOfValue>))]
        public IHttpActionResult GetItemGS(string companyCode, string branchCode)
        {
            string errorMessage = string.Empty;
            var gsList = new OPGMeltingIssueBL().GetMetalGS(companyCode, branchCode, out errorMessage);
            if (gsList != null)
                return Ok(gsList);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = errorMessage });
            }
        }

        [HttpGet]
        [Route("get-batch-list")]
        [ResponseType(typeof(List<OPGMeltingIssueBatchDetailVM>))]
        public IHttpActionResult GetBatchList(string companyCode, string branchCode, string gsCode)
        {
            string errorMessage = string.Empty;
            var result = new OPGMeltingIssueBL().GetBatchList(companyCode, branchCode, gsCode, out errorMessage);
            if (result != null)
                return Ok(result);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM {ErrorStatusCode = HttpStatusCode.BadRequest,
                    description = errorMessage});
            }
        }

        [HttpGet]
        [Route("get-batch-detail")]
        [ResponseType(typeof(OPGMeltingIssueBatchDetailVM))]
        public IHttpActionResult GetBatchDetail(string companyCode, string branchCode, string batchID)
        {
            string errorMessage = string.Empty;
            var result = new OPGMeltingIssueBL().GetBatchInfo(companyCode, branchCode, batchID, out errorMessage);
            if (result != null)
                return Ok(result);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM
                {
                    ErrorStatusCode = HttpStatusCode.BadRequest,
                    description = errorMessage
                });
            }
        }

        [HttpPost]
        [Route("post")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult PostMeltingIssue(OPGMeltingIssueHeaderVM OPGMeltingIssueHeader)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            int issueNo = 0;
            bool success = new OPGMeltingIssueBL().SaveIssue(OPGMeltingIssueHeader, userID, out issueNo, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = issueNo.ToString(),
                    Message = "OPG Melting Issue No# " + issueNo.ToString() + " created successfully."
                };
                return Ok(docCreation);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// Cancels the issue
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="issueNo"></param>
        /// <param name="cancelRemarks"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("cancel")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult CancelMeltingIssue(string companyCode, string branchCode, int issueNo, string cancelRemarks = null)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            bool success = new OPGMeltingIssueBL().CancelIssue(companyCode, branchCode, issueNo, userID, cancelRemarks, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = issueNo.ToString(),
                    Message = "OPG Melting Issue No# " + issueNo.ToString() + " cancelled successfully."
                };
                return Ok(docCreation);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// Get Print by Issue Number and Date.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="issueNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Print")]
        [Route("Print/{companyCode}/{branchCode}/{issueNo}")]
        public IHttpActionResult Print(string companyCode, string branchCode, int issueNo)
        {
            // This code is written by Eshwar on 22nd June 2021
            ErrorVM error = null;
            ProdigyPrintVM result = new OPGMeltingIssueBL().Print(companyCode, branchCode, issueNo, out error);
            if (error == null)
                return Ok(result);
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

    }
}
