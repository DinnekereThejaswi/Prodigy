using ProdigyAPI.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProdigyAPI.BL.BusinessLayer.Order;
using System.Web.Http.Description;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Orders;

namespace ProdigyAPI.Controllers.Orders
{
    [RoutePrefix("api/order/cpc-issue")]
    public class OrderCPCIssueController : SIBaseApiController<OrderCPCIssueBL>
    {
        /// <summary>
        /// Get Metal Code
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-metals")]
        [Route("get-metals/{companyCode}/{branchCode}")]
        [ResponseType(typeof(List<ListOfValue>))]
        public IHttpActionResult GetMetals(string companyCode, string branchCode)
        {
            var metals = new OrderCPCIssueBL().GetMetalCodes(companyCode, branchCode);
            if (metals != null)
                return Ok(metals);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to load metals." });
            }
        }

        /// <summary>
        /// Get GS for the metal. Pass metalCode as ALL to get all GS codes irrespective of the metal code.
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="metalCode">The metal code. </param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-gs")]
        [Route("get-gs/{companyCode}/{branchCode}/{metalCode}")]
        [ResponseType(typeof(List<ListOfValue>))]
        public IHttpActionResult GetGSForMetal(string companyCode, string branchCode, string metalCode)
        {
            var gsList = new OrderCPCIssueBL().GetGSForMetalCode(companyCode, branchCode, metalCode);
            if (gsList != null)
                return Ok(gsList);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to load metals." });
            }
        }

        /// <summary>
        /// Get the list of orders. Order Type should be O since we have to load only customized orders.
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="orderType">Order Type should be O. O is for customized orders.</param>
        /// <param name="gsCode">GS Code. Pass ALL for all GS</param>
        /// <param name="counterCode">Counter Code. Pass ALL for all counter</param>
        /// <param name="karat">Karat Code. Pass ALL for all Karat</param>
        /// <returns>List of CPC Lines.</returns>
        [HttpGet]
        [Route("get-order-list")]
        [ResponseType(typeof(List<OrderCPCIssueLineVM>))]
        public IHttpActionResult GetOrderList(string companyCode, string branchCode, string orderType, string gsCode,
            string counterCode, string karat)
        {
            string errorMessage = string.Empty;
            List<OrderCPCIssueLineVM> orderList = null;
            var success = new OrderCPCIssueBL().GetOrderDetail(companyCode, branchCode, orderType, gsCode, counterCode, karat, out orderList, out errorMessage);
            if (success)
                return Ok(orderList);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM
                {
                    ErrorStatusCode = HttpStatusCode.BadRequest,
                    description = errorMessage
                });
            }
        }

        /// <summary>
        /// Save the order
        /// </summary>
        /// <param name="OrderCPCIssues">Order issue lines. Remarks attribute to be provided.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("post")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult PostIssue(OrderCPCIssuesVM OrderCPCIssues)
        {
            string errorMessage = string.Empty;
            string userID = base.GetUserId();
            int issueNo = 0;
            bool success = new OrderCPCIssueBL().SaveOrderIssue(OrderCPCIssues, userID, out issueNo, out errorMessage);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = issueNo.ToString(),
                    Message = "Order Issue No# " + issueNo.ToString() + " created successfully."
                };
                return Ok(docCreation);
            }
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to post. Error: " + errorMessage });
            }
        }

        /// <summary>
        /// Generates xml and uploads to FTP. This API should be called after successul post.
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="issueNo">Issue number</param>
        /// <returns></returns>
        [HttpPost]
        [Route("generate-xml")]
        [Route("generate-xml/{companyCode}/{branchCode}/{issueNo}")]
        public IHttpActionResult UploadOPGXml(string companyCode, string branchCode, int issueNo)
        {
            string errorMessage = string.Empty;
            var result = new OrderCPCIssueBL().GenerateXMLFile(companyCode, branchCode, issueNo, out errorMessage);
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
    }
}
