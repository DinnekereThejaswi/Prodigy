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
using ProdigyAPI.BL.ViewModel.Print;

namespace ProdigyAPI.Controllers.Transfers
{

    [Authorize]
    [RoutePrefix("api/Transfers/Barcode-receipt")]
    public class BarcodeReceiptController : SIBaseApiController<BarcodeReceiptVM>
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
            bool success = new BarcodeReceiptBL().PostReceipt(barcodeReceipt, userID, true, out receiptNo, out error);
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

        [HttpPost]
        [Route("pending-barcodes")]
        public IHttpActionResult PendingBarcodes(BarcodeReceiptVM barcodeReceipt)
        {
            ErrorVM error = null;
            var result = new BarcodeReceiptBL().PendingBarcodes(barcodeReceipt, out error);
            if (error == null)
                return Ok(result);
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        [HttpPost]
        [Route("imported-barcode-summary")]
        public IHttpActionResult ImportedBarcodeSummary(BarcodeReceiptVM barcodeReceipt)
        {
            ErrorVM error = null;
            var result = new BarcodeReceiptBL().ImportedBarcodeSummary(barcodeReceipt, out error);
            if (error == null)
                return Ok(result);
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        [HttpPost]
        [Route("scanned-barcode-summary")]
        public IHttpActionResult ScannedBarcodeSummary(BarcodeReceiptVM barcodeReceipt)
        {
            ErrorVM error = null;
            var result = new BarcodeReceiptBL().ScannedBarcodeSummary(barcodeReceipt, out error);
            if (error == null)
                return Ok(result);
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// List the Receipt numbers that can be cancelled or printed
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("list/{companyCode}/{branchCode}/{date}")]
        [Route("list")]
        public IHttpActionResult List(string companyCode, string branchCode, DateTime date)
        {
            ErrorVM error = new ErrorVM();
            var data = new BarcodeReceiptBL().List(companyCode, branchCode, date, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }

        }

        /// <summary>
        /// Cancels the receipt (only branch receipt including tagged/non-tagged receipts)
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="receiptNo"></param>
        /// <param name="cancelRemarks"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("cancel-receipt")]
        [Route("cancel-receipt/{companyCode}/{branchCode}/{receiptNo}/{cancelRemarks}")]
        public IHttpActionResult CancelIssue(string companyCode, string branchCode, int receiptNo, string cancelRemarks)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            var result = new BarcodeReceiptBL().CancelReceipt(companyCode, branchCode, receiptNo, userID, cancelRemarks, out error);
            if (result == true)
                return Ok();
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Get Print by Issue Number and Date.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="receiptNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("PrintSummary")]
        [Route("PrintSummary/{companyCode}/{branchCode}/{receiptNo}")]
        public IHttpActionResult Print(string companyCode, string branchCode, int receiptNo)
        {
            // This code is written by Eshwar on 23rd June 2021
            ErrorVM error = null;
            ProdigyPrintVM result = new BarcodeReceiptBL().Print(companyCode, branchCode, receiptNo, out error);
            if (error == null)
                return Ok(result);
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Get Print by Issue Number and Date.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="receiptNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("PrintDet")]
        [Route("PrintDet/{companyCode}/{branchCode}/{receiptNo}")]
        public IHttpActionResult PrintDet(string companyCode, string branchCode, int receiptNo)
        {
            // This code is written by Eshwar on 23rd June 2021
            ErrorVM error = null;
            ProdigyPrintVM result = new BarcodeReceiptBL().PrintDet(companyCode, branchCode, receiptNo, out error);
            if (error == null)
                return Ok(result);
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }
    }
}
