using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProdigyAPI.BL.ViewModel.Receipts;
using ProdigyAPI.Handlers;
using ProdigyAPI.BL.BusinessLayer.Receipts;
using ProdigyAPI.BL.ViewModel.Error;
using System.Web.Http.Description;
using ProdigyAPI.BL.ViewModel.Master;

namespace ProdigyAPI.Controllers.Transfers
{

    //[Authorize]
    [RoutePrefix("api/Transfers/Non-tag-receipt")]
    public class NonTagReceiptController : SIBaseApiController<BarcodeReceiptVM>
    {
        [HttpGet]
        [Route("get-receipt-from")]
        [Route("get-receipt-from/{companyCode}/{branchCode}")]
        public IHttpActionResult GetReceiptFromList(string companyCode, string branchCode)
        {
            var partyList = new BarcodeReceiptBL().GetReceiptFromList(companyCode, branchCode);
            if (partyList != null)
                return Ok(partyList);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to load issue-to list" });
            }
        }

        [HttpGet]
        [Route("get-issue-detail")]
        [Route("get-issue-detail/{requestCompanyCode}/{requestBranchCode}/{issuedBranchCode}/{issueNo}")]
        [ResponseType(typeof(BarcodeReceiptVM))]
        public IHttpActionResult GetIssueDetail(string requestCompanyCode, string requestBranchCode, string issuedBranchCode, int issueNo)
        {
            BarcodeReceiptVM barcodeReceipt = new BarcodeReceiptVM();
            ErrorVM error = null;
            bool success = new BarcodeReceiptBL().GetIssueDetails(requestCompanyCode, requestBranchCode, issuedBranchCode, issueNo, out barcodeReceipt, out error);
            if (success)
                return Ok(barcodeReceipt);
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        [HttpPost]
        [Route("post")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult PostReceipt(BarcodeReceiptVM barcodeReceipt)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            int receiptNo = 0;
            bool success = new BarcodeReceiptBL().PostReceipt(barcodeReceipt, userID, false, out receiptNo, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = receiptNo.ToString(),
                    Message = "Receipt No# " + receiptNo.ToString() + " created successfully."
                };
                return Ok(docCreation);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }
       
    }
}
