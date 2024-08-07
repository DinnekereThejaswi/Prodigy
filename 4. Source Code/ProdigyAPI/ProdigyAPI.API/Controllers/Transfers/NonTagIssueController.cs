using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProdigyAPI.BL.ViewModel.Issues;
using ProdigyAPI.Handlers;
using ProdigyAPI.BL.BusinessLayer.Issues;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using System.Web.Http.Description;
namespace ProdigyAPI.Controllers.Transfers
{

    [RoutePrefix("api/Transfers/Non-tag-issue")]
    public class NonTagIssueController : SIBaseApiController<BarcodedIssueBL>
    {
        [HttpGet]
        [Route("get-issue-to")]
        [Route("get-issue-to/{companyCode}/{branchCode}")]
        public IHttpActionResult GetIssueToList(string companyCode, string branchCode)
        {
            var partyList = new NonTagIssueBL().GetIssueToList(companyCode, branchCode);
            if (partyList != null)
                return Ok(partyList);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to load issue-to list" });
            }
        }

        [HttpGet]
        [Route("item")]
        [Route("item/{companyCode}/{branchCode}/{gsCode}/{counterCode}")]
        public IHttpActionResult GetItem(string companyCode, string branchCode, string gsCode, string counterCode)
        {
            ErrorVM error = null;
            var result = new NonTagIssueBL().GetItems(companyCode, branchCode, gsCode, counterCode);
            if (result != null)
                return Ok(result);
            else {
                error = new ErrorVM { description = "Items not found", ErrorStatusCode = HttpStatusCode.NotFound };
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        [HttpGet]
        [Route("CounterStock")]
        [Route("CounterStock/{companyCode}/{branchCode}/{gsCode}/{counterCode}/{itemCode}")]
        [ResponseType(typeof(ClosingStockVM))]
        public IHttpActionResult GetCounterStock(string companyCode, string branchCode, string gsCode, string counterCode, string itemCode)
        {
            ErrorVM error = null;
            var result = new NonTagIssueBL().GetClosingCounterStock(companyCode, branchCode, gsCode, counterCode, itemCode);
            if (result != null)
                return Ok(result);
            else {
                error = new ErrorVM { description = "Counter-stock not found", ErrorStatusCode = HttpStatusCode.NotFound };
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        [HttpPost]
        [Route("post")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult PostIssue(NonTagIssueVM nonTagIssue)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            int issueNo = 0;
            bool success = new NonTagIssueBL().PostIssue(nonTagIssue, userID, out issueNo, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = issueNo.ToString(),
                    Message = "Issue No# " + issueNo.ToString() + " created successfully."
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
            var result = new NonTagIssueBL().GenerateXMLFile(companyCode, branchCode, issueNo, out errorMessage);
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

        [HttpPost]
        [Route("cancel-issue")]
        [Route("cancel-issue/{companyCode}/{branchCode}/{issueNo}/{cancelRemarks}")]
        public IHttpActionResult CancelIssue(string companyCode, string branchCode, int issueNo, string cancelRemarks)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            var result = new BarcodedIssueBL().CancelIssue(companyCode, branchCode, issueNo, userID, cancelRemarks, out error);
            if (result == true)
                return Ok();
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

    }
}
