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
    [RoutePrefix("api/Transfers/OPG-melting-receipt")]
    public class OPGMeltingReceiptController : SIBaseApiController<OPGMeltingReceiptBL>
    {
        [HttpGet]
        [Route("get-receipt-from")]
        [Route("get-receipt-from/{companyCode}/{branchCode}")]
        [ResponseType(typeof(List<ListOfValue>))]
        public IHttpActionResult GetIssueToList(string companyCode, string branchCode)
        {
            var partyList = new OPGMeltingReceiptBL().GetReceiptFromList(companyCode, branchCode);
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
            var gsList = new OPGMeltingReceiptBL().GetMetalGS(companyCode, branchCode, out errorMessage);
            if (gsList != null)
                return Ok(gsList);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = errorMessage });
            }
        }

        [HttpGet]
        [Route("get-pendingissue-list")]
        [ResponseType(typeof(List<ListOfValue>))]
        public IHttpActionResult GetBatchList(string companyCode, string branchCode, string receivedFrom)
        {
            string errorMessage = string.Empty;
            var result = new OPGMeltingReceiptBL().GetPendingIssues(companyCode, branchCode, receivedFrom, out errorMessage);
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
        [Route("get-batch-for-issue")]
        [ResponseType(typeof(List<OPGMeltingReceiptBatchDetailVM>))]
        public IHttpActionResult GetBatchDetail(string companyCode, string branchCode, int issueNo)
        {
            string errorMessage = string.Empty;
            var result = new OPGMeltingReceiptBL().GetAllBatchDetailForGivenIssue(companyCode, branchCode, issueNo, out errorMessage);
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
        [Route("get-batchinfo")]
        [Route("get-batchinfo/{companyCode}/{branchCode}/{issueNo}/{batchId}")]
        [ResponseType(typeof(OPGMeltingReceiptBatchDetailVM))]
        public IHttpActionResult GetBatchInfo(string companyCode, string branchCode, int issueNo, string batchId)
        {
            string errorMessage = string.Empty;
            var result = new OPGMeltingReceiptBL().GetSingleBatchInfo(companyCode, branchCode, issueNo, batchId, out errorMessage);
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
        public IHttpActionResult PostMeltingIssue(OPGMeltingReceiptHeaderVM OPGMeltingReceipt)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            int receiptNo = 0;
            bool success = new OPGMeltingReceiptBL().SaveReceipt(OPGMeltingReceipt, userID, out receiptNo, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = receiptNo.ToString(),
                    Message = "OPG Melting Receipt No# " + receiptNo.ToString() + " created successfully."
                };
                return Ok(docCreation);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// Cancels the Melting receipt
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="receiptNo"></param>
        /// <param name="cancelRemarks"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("cancel")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult CancelMeltingReceipt(string companyCode, string branchCode, int receiptNo, string cancelRemarks = null)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            bool success = new OPGMeltingReceiptBL().CancelReceipt(companyCode, branchCode, receiptNo, userID, cancelRemarks, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = receiptNo.ToString(),
                    Message = "OPG Melting Receipt No# " + receiptNo.ToString() + " cancelled successfully."
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
            // This code is written by Eshwar on 7th June 2021
            ErrorVM error = null;
            ProdigyPrintVM result = new OPGMeltingReceiptBL().Print(companyCode, branchCode, issueNo, out error);
            if (error == null)
                return Ok(result);
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }
    }
}
