using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.BusinessLayer.Receipts;
using ProdigyAPI.Handlers;
using System.Web.Http.Description;

namespace ProdigyAPI.Controllers.Transfers
{
    /// <summary>
    /// API to cancel the receipt documents. Use this for issue transactions other than Barcoded receipt and Non Tag receipt.
    /// </summary>
    [RoutePrefix("api/Transfers/receipt-cancel")]
    public class ReceiptCancelController : SIBaseApiController<ReceiptCancelBL>
    {
        /// <summary>
        /// Get Receipt numbers to cancel. receiptDate parameter can be passed as null if you wish to load all receipt number.
        /// If the receiptDate parameter is sent, receipts related to receiptDate are loaded.
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="receiptType">Receipt Type. Can be: RB for branch receipt, RL for melting receipt, 
        /// RP for purification receipt, RM for manufacturing etc.. Refer IRSetup master for details of codes</param>
        /// <param name="receiptDate">Receipt Date. null can be passed to retrieve all receipt number. It is recommended to pass date.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-receipt-numbers")]
        [Route("get-receipt-numbers/{companyCode}/{branchCode}/{receiptType}/{receiptDate}")]
        [ResponseType(typeof(List<ListOfValue>))]
        public IHttpActionResult GetReceiptToList(string companyCode, string branchCode, string receiptType, DateTime? receiptDate = null)
        {
            ErrorVM error = null;
            var result = new ReceiptCancelBL().GetReceiptsToCancel(companyCode, branchCode, receiptType, out error, receiptDate);
            if (result != null)
                return Ok(result);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to load receipt-numbers. " });
            }
        }

        /// <summary>
        /// Cancels the receipt
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="receiptType">Receipt Type. Can be: IO for OPG Segregation, IL for melting receipt, 
        /// IB for branch receipt, IM for manufacturing. Refer IRSetup master for details of codes</param>
        /// <param name="receiptNo">Receipt number to cancel</param>
        /// <param name="cancelRemarks">Cancellation remarks</param>
        /// <returns></returns>
        [HttpPost]
        [Route("cancel")]
        public IHttpActionResult CancelReceipt(string companyCode, string branchCode, string receiptType, int receiptNo, string cancelRemarks)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            string errorMessage = string.Empty;
            bool success = new ReceiptCancelBL().CancelReceipt(companyCode, branchCode, receiptType, receiptNo, cancelRemarks, userID, out error);
            if (success) {
                return Ok();
            }
            else {
                error = new ErrorVM { ErrorStatusCode = HttpStatusCode.BadRequest, description = errorMessage };
                return Content(HttpStatusCode.BadRequest, error);
            }
        }
    }
}
