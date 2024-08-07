using ProdigyAPI.BL.BusinessLayer.Repair;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.BL.ViewModel.Repair;
using ProdigyAPI.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Query;

namespace ProdigyAPI.Controllers.Repair
{
    /// <summary>
    /// This provides API's for Repair
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Repair")]
    [SIExceptionFilter]
    public class RepairController : SIBaseApiController<RepairReceiptMasterVM>, IBaseMasterActionController<RepairReceiptMasterVM, RepairReceiptMasterVM>
    {
        #region Controller Methods

        #region Receipts
        /// <summary>
        /// This provides GS for Repair.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("RepairGS/{companyCode}/{branchCode}")]
        [Route("RepairGS")]
        public IHttpActionResult GetRepairGS(string companyCode, string branchCode)
        {
            List<RepairGSVM> lstOfRepairGS = new RepairBL().GetRepairGS(companyCode, branchCode);
            return Ok(lstOfRepairGS);
        }

        /// <summary>
        /// This returs Repair Items By GSCode
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("RepairItem/{companyCode}/{branchCode}/{gsCode}")]
        [Route("RepairItem")]
        public IHttpActionResult GetRepairItem(string companyCode, string branchCode, string gsCode)
        {
            List<RepairGSItemVM> lstOfRepairGSItems = new RepairBL().GetRepairGSItems(gsCode, companyCode, branchCode);
            return Ok(lstOfRepairGSItems);
        }

        /// <summary>
        /// This returns Receipt Master and there details by repairNo (ReceiptNo).
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="repairNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Reciept/Get/{companyCode}/{branchCode}/{repairNo}")]
        [Route("Reciept/Get")]
        public IHttpActionResult GetReceiptDetails(string companyCode, string branchCode, int repairNo)
        {
            ErrorVM error = new ErrorVM();
            RepairReceiptMasterVM rrvm = new RepairBL().GetReceiptDetails(companyCode, branchCode, repairNo, false, out error);
            if (error == null) {
                return Ok(rrvm);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Receipt Details for Print.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="repairNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Reciept/GetForPrint/{companyCode}/{branchCode}/{repairNo}")]
        [Route("Reciept/GetForPrint")]
        public IHttpActionResult GetReceiptDetailsForPrint(string companyCode, string branchCode, int repairNo)
        {
            ErrorVM error = new ErrorVM();
            RepairReceiptMasterVM rrvm = new RepairBL().GetReceiptDetails(companyCode, branchCode, repairNo, true, out error);
            if (error == null) {
                return Ok(rrvm);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get total details of receipt print.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="repairNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Reciept/GetTotalForPrint/{companyCode}/{branchCode}/{repairNo}")]
        [Route("Reciept/GetTotalForPrint")]
        public IHttpActionResult GetReceiptDetailsTotalForPrint(string companyCode, string branchCode, int repairNo)
        {
            ErrorVM error = new ErrorVM();
            RepairReceiptDetailsVM rrvm = new RepairBL().GetReceiptDetailsTotalForPrint(companyCode, branchCode, repairNo, out error);
            if (error == null) {
                return Ok(rrvm);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Save Receipt Details
        /// </summary>
        /// <param name="repair"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Receipt/Post")]
        public IHttpActionResult SaveReceiptDetails([FromBody] RepairReceiptMasterVM repair)
        {
            if (repair == null) {
                return BadRequest();
            }
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            repair.OperatorCode = base.GetUserId();
            ErrorVM error = new ErrorVM();
            int repairNo = new RepairBL().SaveReceiptDetails(repair, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(new { repairNo = repairNo });
        }

        /// <summary>
        /// Upadate Receipt Details
        /// </summary>
        /// <param name="repairNo"></param>
        /// <param name="repair"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Receipt/Put")]
        public IHttpActionResult UpdateReceiptDetails(int repairNo, [FromBody] RepairReceiptMasterVM repair)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            int retRepairNo = new RepairBL().UpdateReceiptDetails(repairNo, repair, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(retRepairNo);
        }

        /// <summary>
        /// Search parameters for Orders.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Receipt/SearchParams")]
        public IHttpActionResult GetAllSearchParameters(string companyCode, string branchCode)
        {
            List<SearchParamVM> lstSearchParams = new List<SearchParamVM>();
            lstSearchParams.Add(new SearchParamVM() { Value = "REPAIRNO", Key = "Repair No" });
            lstSearchParams.Add(new SearchParamVM() { Value = "NAME", Key = "Name" });
            return Ok(lstSearchParams);
        }

        /// <summary>
        /// Receipt details for Receipt Cancel by search parameters
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="searchType"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Receipt/AllReceipts/{companyCode}/{branchCode}/{searchType}/{searchValue}")]
        public IQueryable<RepairReceiptMasterVM> GetAllRepairReceipts(string companyCode, string branchCode, string searchType, string searchValue)
        {
            IQueryable<RepairReceiptMasterVM> repair = new RepairBL().GetAllRepairDetails(companyCode, branchCode, searchType, searchValue);
            return repair.AsQueryable<RepairReceiptMasterVM>();
        }

        /// <summary>
        /// This is used to Cancel the Repair Receipts.
        /// </summary>
        /// <param name="receiptMaster"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Reciept/CancelReceipt")]
        public IHttpActionResult CancelRepairReceipt(RepairReceiptMasterVM receiptMaster)
        {
            ErrorVM error = new ErrorVM();
            bool cancelled = new RepairBL().CancelRepairReceipt(receiptMaster, out error);
            if (error == null) {
                return Ok();
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Repair Receipt Print (Both HTML and Dotmatrix)
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="repairNo"></param>
        /// <param name="printType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Receipt/RepairReceiptPrint/{companyCode}/{branchCode}/{repairNo}/{printType}")]
        public IHttpActionResult RepairReceiptPrint(string companyCode, string branchCode, int repairNo, string printType)
        {
            ErrorVM error = new ErrorVM();
            BL.ViewModel.Print.ProdigyPrintVM print = new RepairBL().GetRepairReceiptPrint(companyCode, branchCode, repairNo, printType, out error);
            if (error == null) {
                return Ok(print);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }
        #endregion

        #region Issues
        /// <summary>
        /// Get Delivery (Issue) Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="issueNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Issue/Get/{companyCode}/{branchCode}/{issueNo}")]
        [Route("Issue/Get")]
        public IHttpActionResult GetIssuesDetails(string companyCode, string branchCode, int issueNo)
        {
            //RepairIssueMasterVM rimv = new RepairBL().GetRepairIssueDetails(companyCode, branchCode, issueNo);
            //return Ok(rimv);

            ErrorVM error = new ErrorVM();
            RepairIssueMasterVM rimv = new RepairBL().GetRepairIssueDetails(companyCode, branchCode, issueNo, false, out error);
            if (error == null) {
                return Ok(rimv);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get Issue Details For Print.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="issueNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Issue/GetForPrint/{companyCode}/{branchCode}/{issueNo}")]
        [Route("Issue/GetForPrint")]
        public IHttpActionResult GetIssuesDetailsForPrint(string companyCode, string branchCode, int issueNo)
        {
            ErrorVM error = new ErrorVM();
            RepairIssueMasterVM rimv = new RepairBL().GetRepairIssueDetails(companyCode, branchCode, issueNo, true, out error);
            if (error == null) {
                return Ok(rimv);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get Issue Details Total For Print.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="issueNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Issue/GetTotalForPrint/{companyCode}/{branchCode}/{issueNo}")]
        [Route("Issue/GetTotalForPrint")]
        public IHttpActionResult GetIssuesDetailsTotalForPrint(string companyCode, string branchCode, int issueNo)
        {
            ErrorVM error = new ErrorVM();
            RepairIssueMasterVM rimv = new RepairBL().GetRepairIssueDetailsTotalForPrint(companyCode, branchCode, issueNo, out error);
            if (error == null) {
                return Ok(rimv);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Receipt Delivery Details from receiptNo.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="receiptNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Issue/IssuesDetailsWithReceiptDetails/{companyCode}/{branchCode}/{receiptNo}")]
        [Route("Issue/IssuesDetailsWithReceiptDetails")]
        public IHttpActionResult GetIssuesDetailsWithReceiptDetails(string companyCode, string branchCode, int receiptNo)
        {
            ErrorVM error = new ErrorVM();
            RepairIssueMasterVM rrvm = new RepairBL().GetRepairIssueMasterWithReceipt(companyCode, branchCode, receiptNo, false, out error);
            if (error == null) {
                return Ok(rrvm);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get Delivery details for Delivery Cancel.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="issueNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Issue/GetForCancel/{companyCode}/{branchCode}/{issueNo}")]
        public IHttpActionResult GetIssueDetailsForCancel(string companyCode, string branchCode, int issueNo)
        {
            ErrorVM error = new ErrorVM();
            RepairReceiptMasterVM rimv = new RepairBL().GetRepairIssueDetailsForCancel(companyCode, branchCode, issueNo, out error);
            if (error == null) {
                return Ok(rimv);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Save Delivery Details (Issue Details)
        /// </summary>
        /// <param name="repairIssue"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Issue/Post")]
        public IHttpActionResult SaveIssueDetails([FromBody] RepairIssueMasterVM repairIssue)
        {
            ErrorVM error = new ErrorVM();
            int deliveryNo = new RepairBL().SaveRepairIssueDetails(repairIssue, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(new { deliveryNo = deliveryNo });
        }

        /// <summary>
        /// Update Delivery Details (Issue Details)
        /// </summary>
        /// <param name="issueNo"></param>
        /// <param name="repairIssue"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Issue/Put")]
        public IHttpActionResult UpdateIssueDetails(int issueNo, [FromBody] RepairIssueMasterVM repairIssue)
        {
            ErrorVM error = new ErrorVM();
            int deliveryNo = new RepairBL().UpdateRepairIssueDetails(issueNo, repairIssue, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(deliveryNo);
        }

        /// <summary>
        /// This is used to Cancel the Repair Delivery.
        /// </summary>
        /// <param name="receipt"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Issue/CancelDelivery")]
        public IHttpActionResult CancelRepairDelivery(RepairReceiptMasterVM receipt)
        {
            ErrorVM error = new ErrorVM();
            bool cancelled = new RepairBL().CancelRepairDelivery(receipt, out error);
            if (error == null) {
                return Ok();
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Calculaton on lines
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Issue/GetCalculation")]
        public IHttpActionResult CalculationRepair(RepairIssueDetailsVM details)
        {
            RepairIssueDetailsVM retDet = new RepairBL().RepairCalculation(details);
            return Ok(retDet);
        }

        /// <summary>
        /// Repair Delivery Print (Both HTML and Dotmatrix)
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="issueNo"></param>
        /// <param name="printType"></param>
        /// <param name="isDirect"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Issue/RepairIssuetPrint/{companyCode}/{branchCode}/{issueNo}/{printType}/{isDirect}")]
        public IHttpActionResult RepairIssuetPrint(string companyCode, string branchCode, int issueNo, string printType, int isDirect)
        {
            ErrorVM error = new ErrorVM();
            BL.ViewModel.Print.ProdigyPrintVM print = new RepairBL().GetRepairIssuetPrint(companyCode, branchCode, issueNo, printType, isDirect, out error);
            if (error == null) {
                return Ok(print);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }
        public IQueryable<RepairReceiptMasterVM> List()
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Count(ODataQueryOptions<RepairReceiptMasterVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Post(RepairReceiptMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, RepairReceiptMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Delete([FromBody] int id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #endregion
    }
}
