using ProdigyAPI.BL.BusinessLayer.BatchPosting;
using ProdigyAPI.BL.ViewModel.BatchPosting;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Handlers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace ProdigyAPI.Controllers.BatchPosting
{
    /// <summary>
    ///  Provides API's for scheme download/updation module.
    ///  Savings scheme data will be downloaded into the PoS db and then it is updated back to the scheme database.
    /// </summary>
    //[Authorize]
    [RoutePrefix("api/batch-posting/chit-update")]
    public class ChitUpdateController : SIBaseApiController<ChitUpdateBL>
    {
        /// <summary>
        /// Gets the schemes that are downloaded
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="txnDate">Date of the transaction</param>
        /// <param name="pendingStatus">Pending status</param>
        /// <returns></returns>
        [HttpGet]
        [Route("pending-chits")]
        [Route("pending-chits/{companyCode}/{branchCode}/{txnDate}/{pendingStatus}")]
        [ResponseType(typeof(List<SchemeInfoVM>))]
        public IHttpActionResult PendingChits(string companyCode, string branchCode, DateTime txnDate, bool pendingStatus)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            List<SchemeInfoVM> pendingSchemes;
            var result = new ChitUpdateBL().GetPendingDetails(companyCode, branchCode, txnDate, pendingStatus, out pendingSchemes, out error);
            if (result == true) {
                return Ok(pendingSchemes);
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Download schemes
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="txnDate">Date of the transaction</param>
        /// <returns></returns>
        [HttpPost]
        [Route("download")]
        [Route("download/{companyCode}/{branchCode}/{txnDate}")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult DownloadSchemes(string companyCode, string branchCode, DateTime txnDate)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            int recordsDownloaded = 0;
            var result = new ChitUpdateBL().DownloadSchemes(companyCode, branchCode, txnDate, userID, out recordsDownloaded, out error);
            if (result == true) {
                var docCreation = new DocumentCreationVM { DocumentNo = "Scheme-download", Message = recordsDownloaded.ToString() + " records downloaded!" };
                return Ok(docCreation);
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }
        /// <summary>
        /// Post the bill information back to the scheme server
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="txnDate">Date of the transaction</param>
        /// <returns></returns>
        [HttpPost]
        [Route("bill-update")]
        [Route("bill-update/{companyCode}/{branchCode}/{txnDate}")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult UpdateBills(string companyCode, string branchCode, DateTime txnDate)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            int recordsDownloaded = 0;
            var result = new ChitUpdateBL().UpdateBillNumbers(companyCode, branchCode, txnDate, userID, out recordsDownloaded, out error);
            if (result == true) {
                var docCreation = new DocumentCreationVM { DocumentNo = "Bill Updation", Message = recordsDownloaded.ToString() + " records updated!" };
                return Ok(docCreation);
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }        
    }
}
