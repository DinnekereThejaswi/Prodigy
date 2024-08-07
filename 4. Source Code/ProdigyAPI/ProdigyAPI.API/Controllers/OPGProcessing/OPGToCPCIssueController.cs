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
    [RoutePrefix("api/Transfers/OPG-cpc-issue")]
    public class OPGToCPCIssueController : SIBaseApiController<OPGCPCIssueBL>
    {
        /// <summary>
        /// Get issue to list
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-issue-to")]
        [Route("get-issue-to/{companyCode}/{branchCode}")]
        [ResponseType(typeof(List<ListOfValue>))]
        public IHttpActionResult GetIssueToList(string companyCode, string branchCode)
        {
            var partyList = new OPGCPCIssueBL().GetIssueToList(companyCode, branchCode);
            if (partyList != null)
                return Ok(partyList);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to load issue-to list" });
            }
        }

        [HttpGet]
        [Route("get-document-numbers")]
        [Route("get-document-numbers/{companyCode}/{branchCode}")]
        [ResponseType(typeof(List<ListOfValue>))]
        public IHttpActionResult GetDocumentNumbers(string companyCode, string branchCode)
        {
            string errorMessage = string.Empty;
            var gsList = new OPGCPCIssueBL().GetReceiptList(companyCode, branchCode, out errorMessage);
            if (gsList != null)
                return Ok(gsList);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = errorMessage });
            }
        }

        [HttpGet]
        [Route("get-batch-list")]
        [ResponseType(typeof(List<OPGCPCIssueLineVM>))]
        public IHttpActionResult GetBatchList(string companyCode, string branchCode, int documentNo)
        {
            string errorMessage = string.Empty;
            var result = new OPGCPCIssueBL().GetBatchList(companyCode, branchCode, documentNo, out errorMessage);
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

        [HttpGet]
        [Route("get-batch-detail")]
        [ResponseType(typeof(OPGMeltingIssueBatchDetailVM))]
        public IHttpActionResult GetBatchDetail(string companyCode, string branchCode, int documentNo, string batchID)
        {
            string errorMessage = string.Empty;
            var result = new OPGCPCIssueBL().GetBatchInfo(companyCode, branchCode, documentNo, batchID, out errorMessage);
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
        public IHttpActionResult PostCPCIssue(OPGCPCIssueHeaderVM OPGCPCIssueHeader)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            int issueNo = 0;
            bool success = new OPGCPCIssueBL().SaveIssue(OPGCPCIssueHeader, userID, out issueNo, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = issueNo.ToString(),
                    Message = "OPG CPC Issue No.# " + issueNo.ToString() + " created successfully."
                };
                return Ok(docCreation);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        [HttpPost]
        [Route("generate-xml")]
        [Route("generate-xml/{companyCode}/{branchCode}/{issueNo}")]
        public IHttpActionResult UploadOPGXml(string companyCode, string branchCode, int issueNo)
        {
            string errorMessage = string.Empty;
            var result = new OPGCPCIssueBL().GenerateXMLFile(companyCode, branchCode, issueNo, out errorMessage);
            if (result == true)
                return Ok();
            else {
                ErrorVM error = new ErrorVM
                {
                    description = "Failed to generate & upload XML. Error " + errorMessage,
                    ErrorStatusCode = HttpStatusCode.BadRequest
                };
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Cancells the CPC issue.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="issueNo"></param>
        /// <param name="cancelRemarks"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("cancel")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult CancelCPCIssue(string companyCode, string branchCode, int issueNo, string cancelRemarks = null)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            bool success = new OPGCPCIssueBL().CancelIssue(companyCode, branchCode, issueNo, userID, cancelRemarks, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = issueNo.ToString(),
                    Message = "OPG Melting Issue to CPC No# " + issueNo.ToString() + " cancelled successfully."
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
            // This code is written by Eshwar on 28th June 2021
            ErrorVM error = null;
            ProdigyPrintVM result = new OPGCPCIssueBL().Print(companyCode, branchCode, issueNo, out error);
            if (error == null)
                return Ok(result);
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }
    }
}
