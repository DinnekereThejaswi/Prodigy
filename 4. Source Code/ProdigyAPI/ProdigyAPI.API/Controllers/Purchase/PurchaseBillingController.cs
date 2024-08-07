using ProdigyAPI.BL.BusinessLayer.Purchase;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.OldPurchase;
using ProdigyAPI.BL.ViewModel.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProdigyAPI.Controllers.Purchase
{
    /// <summary>
    /// This provides API's for Purchase Billing.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Purchase/Billing")]
    public class PurchaseBillingController : ApiController
    {
        /// <summary>
        /// Get Purcahse Details by Purchase Bill Number.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="billNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Get/{companyCode}/{branchCode}/{billNo}")]
        [Route("Get")]
        public IHttpActionResult GetPurchase(string companyCode, string branchCode, int billNo)
        {
            ErrorVM error = new ErrorVM();
            PurchaseBillMasterVM pruchase = new PurchaseBillingBL().GetPurchaseDetails(companyCode, branchCode, billNo, out error);
            if (error != null)
            {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(pruchase);
        }

        /// <summary>
        /// Save purhase details
        /// </summary>
        /// <param name="purchase"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult SavePurchaseDetails(PurchaseBillingVM purchase)
        {
            ErrorVM error = new ErrorVM();
            int billNo = new PurchaseBillingBL().SavePurchaseBillWithTxn(purchase, out error);
            if (error != null)
            {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(new { billNo = billNo });
        }

        /// <summary>
        /// Print Sales Purchase Bill.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="billNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Print/{companyCode}/{branchCode}/{billNo}")]
        [Route("Print")]
        public IHttpActionResult SalesBillPrint(string companyCode, string branchCode, int billNo)
        {
            ErrorVM error = new ErrorVM();
            BL.ViewModel.Print.ProdigyPrintVM print = new PurchaseBillingBL().PrintPurchaseBill(companyCode, branchCode, billNo, out error);
            if (error != null)
            {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(print);
        }

        /// <summary>
        /// Cancel purchase bill
        /// </summary>
        /// <param name="purchase"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CancelPurchaseBill")]
        public IHttpActionResult CancelPurchaseBill(PurchaseBillingVM purchase)
        {
            ErrorVM error = new ErrorVM();
            bool cancelled = new PurchaseBillingBL().CancelPurchaseBill(purchase, out error);
            if (error != null)
            {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok();
        }

        /// <summary>
        /// Get All Purchase bill whether cancelled or not.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="date"></param>
        /// <param name="isCancelled"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("AllPurchaseBill/{companyCode}/{branchCode}/{date}/{isCancelled}")]
        [Route("AllPurchaseBill")]
        public IHttpActionResult GetAllPurchaseBill(string companyCode, string branchCode, DateTime date, bool isCancelled)
        {
            ErrorVM error = new ErrorVM();
            List<PurchaseBillingVM> lstOfPurchase = new PurchaseBillingBL().GetAllPurchaseBill(companyCode, branchCode, date, isCancelled, out error);
            if (error != null)
            {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(lstOfPurchase);
        }

        /// <summary>
        /// Purchase Bill Search Parameters
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("SearchParams/{companyCode}/{branchCode}")]
        public IHttpActionResult GetSearchParams(string companyCode, string branchCode)
        {
            List<SearchParamVM> lstSearchParams = new PurchaseBillingBL().GetPurchaseBillSearchParams(companyCode, branchCode);
            return Ok(lstSearchParams);
        }

        /// <summary>
        /// Purchase Bill Search by parameters
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="searchBy"></param>
        /// <param name="searchParam"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("PurchaseBillSearch/{companyCode}/{branchCode}/{searchBy}/{searchParam}/{date}")]
        [Route("PurchaseBillSearch")]
        public IHttpActionResult GetOrderDetailsBySearchParameters(string companyCode, string branchCode, string searchBy, string searchParam, DateTime date)
        {
            //IQueryable<PurchaseBillSearchVM> lstOrderMaster = new PurchaseBillingBL().GetPurchaseBillSearchParameters(companyCode, branchCode, searchBy, searchParam, date);
            //return Ok(lstOrderMaster);
            dynamic lstOrderMaster = new PurchaseBillingBL().GetPurchaseBillSearchParameters(companyCode, branchCode, searchBy, searchParam, date);
            return Ok(lstOrderMaster);
        }

        /// <summary>
        /// Dotmatrix Print of Purchase Bill
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="billNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("DotMatrixPrint/{companyCode}/{branchCode}/{billNo}")]
        public IHttpActionResult GetPurchaseBillDotMatrixPrint(string companyCode, string branchCode, int billNo)
        {
            ErrorVM error = new ErrorVM();
            string print = new PurchaseBillingBL().GetPurchaseDotMatrixPrint(companyCode, branchCode, billNo, out error);
            if (error == null)
            {
                return Ok(print);
            }
            else
            {
                return Content(error.ErrorStatusCode, error);
            }
        }
    }
}
