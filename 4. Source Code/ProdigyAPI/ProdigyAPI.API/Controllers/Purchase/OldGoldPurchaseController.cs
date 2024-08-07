using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.OldPurchase;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.BL.ViewModel.Payment;
using ProdigyAPI.Framework;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.OData.Query;
using ProdigyAPI.BL.BusinessLayer.Purchase;

namespace ProdigyAPI.Controllers.Purchase
{
    /// <summary>
    /// Purchase controler provides API's for Old Gold Purchase Estimation and Attachment.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/purchase")]
    public class OldGoldPurchaseController : SIBaseApiController<PurchaseEstMasterVM>, IBaseMasterActionController<PurchaseEstMasterVM, PurchaseEstMasterVM>
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        string ModuleSeqNo = "3";
        #endregion

        #region Controller Methods

        #region Purchase
        /// <summary>
        /// Get karat details from Item code and GS Code
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="itemCode"></param>
        /// <param name="gsCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetKarat/{companyCode}/{branchCode}/{itemCode}/{gsCode}")]
        [Route("GetKarat")]
        public IHttpActionResult GetKaratDetailsFromItemCode(string companyCode, string branchCode, string itemCode, string gsCode)
        {
            string karat = new OldGoldPurchaseBL().GetKaratDetailsFromItemCode(companyCode, branchCode, itemCode, gsCode);
            return Ok(karat);
        }

        /// <summary>
        /// GS for Old gold purchase
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GS/{companyCode}/{branchCode}")]
        public IHttpActionResult GetGS(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            dynamic result = new OldGoldPurchaseBL().GetGS(companyCode, branchCode, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get Purchase Estimation Details by Estimation No.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Get/{companyCode}/{branchCode}/{estNo}")]
        [Route("Get")]
        public IHttpActionResult GetPurchaseEstimationDetails(string companyCode, string branchCode, int estNo)
        {
            ErrorVM error = new ErrorVM();
            PurchaseEstMasterVM purchase = new OldGoldPurchaseBL().GetPurchaseEstimationDetails(companyCode, branchCode, estNo, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(purchase);
        }

        /// <summary>
        /// Get Purchase Estimation Details by Estimation No.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAttachedPurchaseEstimationDetailsForPrint/{companyCode}/{branchCode}/{estNo}")]
        [Route("GetAttachedPurchaseEstimationDetailsForPrint")]
        public IHttpActionResult GetAttachedPurchaseEstimationDetails(string companyCode, string branchCode, int estNo)
        {
            ErrorVM error = new ErrorVM();
            PurchaseEstMasterVM purchseEstDet = new OldGoldPurchaseBL().GetAttachedPurchaseEstimationDetailsForPrint(companyCode, branchCode, estNo, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(purchseEstDet);
        }

        /// <summary>
        /// Get Purchase Estimation Details by Estimation No.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetForPrintTotal/{companyCode}/{branchCode}/{estNo}")]
        [Route("GetForPrintTotal")]
        public IHttpActionResult GetPurchaseEstimationDetailsForPrintTotal(string companyCode, string branchCode, int estNo)
        {
            ErrorVM error = new ErrorVM();
            PurchaseEstMasterVM purchseEstDet = new OldGoldPurchaseBL().GetPurchaseEstimationDetailsForTotal(companyCode, branchCode, estNo, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(purchseEstDet);
        }

        /// <summary>
        /// Get Purchase Estimation Details by Estimation No.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAttachedPurchaseEstimationTotalPrint/{companyCode}/{branchCode}/{estNo}")]
        [Route("GetAttachedPurchaseEstimationTotalPrint/")]
        public IHttpActionResult GetAttachedPurchaseEstimationDetailsForPrintTotal(string companyCode, string branchCode, int estNo)
        {
            ErrorVM error = new ErrorVM();
            PurchaseEstMasterVM purchseEstDet = new OldGoldPurchaseBL().GetAttachedPurchaseEstimationDetailsForPrintTotal(companyCode, branchCode, estNo, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(purchseEstDet);
        }

        /// <summary>
        /// Save pruchase Details.
        /// </summary>
        /// <param name="purchase"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult SavePurchaseEstimation([FromBody] PurchaseEstMasterVM purchase)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            ErrorVM error = new ErrorVM();
            int estNo = new OldGoldPurchaseBL().SavePurchaseEstimation(purchase, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(new { EstMationNo = estNo });
        }

        /// <summary>
        /// Update pruchase estimation details.
        /// </summary>
        /// <param name="EstNo"></param>
        /// <param name="purchase"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put")]
        public IHttpActionResult UpdatePurchaseEstimation(int EstNo, [FromBody] PurchaseEstMasterVM purchase)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            bool updated = new OldGoldPurchaseBL().UpdatePurchaseEstimation(EstNo, purchase, out error);
            if (error != null && updated == false) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(new { EstMationNo = EstNo });
        }

        /// <summary>
        /// Dropdown content for Old Stone purchase.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("OldStone/{companyCode}/{branchCode}")]
        public IHttpActionResult GetOldStonePurchaseItem(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            List<OrderItemVM> result = new OldGoldPurchaseBL().GetOldStonePurchaseItem(companyCode, branchCode, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get Stone or Dimaond Names based on Type (Stone, Diamond, Old Diamond etc)
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("StoneDiamondName/{companyCode}/{branchCode}/{type}")]
        public IHttpActionResult StoneDiamondName(string companyCode, string branchCode, string type)
        {
            ErrorVM error = new ErrorVM();
            var result = new OldGoldPurchaseBL().StoneDiamondName(companyCode, branchCode, type, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(result);
        }

        /// <summary>
        /// Rate Calculation Based on Karat.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="type"></param>
        /// <param name="karat"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("StoneDiamondRate/{companyCode}/{branchCode}/{type}/{karat}")]
        public IHttpActionResult StoneDiamondRate(string companyCode, string branchCode, string type, decimal karat)
        {
            ErrorVM error = new ErrorVM();
            var result = new OldGoldPurchaseBL().StoneDiamondRate(companyCode, branchCode, type, karat, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(result);
        }

        /// <summary>
        /// Dropdown content for Old Diamond purchase.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("OldDiamond/{companyCode}/{branchCode}")]
        public IHttpActionResult GetOldDiamondPurchaseItem(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            List<OrderItemVM> result = new OldGoldPurchaseBL().GetOldDiamondPurchaseItem(companyCode, branchCode, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(result);
        }

        /// <summary>
        /// Dotmatrix print of Purchase Estimation
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("DotMatrixPrint/{companyCode}/{branchCode}/{estNo}")]
        public IHttpActionResult IssueDotMatrixPrint(string companyCode, string branchCode, int estNo)
        {
            ErrorVM error = new ErrorVM();
            BL.ViewModel.Print.ProdigyPrintVM print = new OldGoldPurchaseBL().GetOldPurchaseEstimatePrint(companyCode, branchCode, estNo, out error);
            if (error == null) {
                return Ok(print);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get HTML Thermal Printing Format
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ThermalPrint60Column/{companyCode}/{branchCode}/{estNo}")]
        public IHttpActionResult ThermalPrint60Column(string companyCode, string branchCode, int estNo)
        {
            ErrorVM error = new ErrorVM();
            string print = new OldGoldPurchaseBL().GetThermalPrint40Column(companyCode, branchCode, estNo, false, out error);
            if (error == null) {
                return Ok(print);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }
        public IQueryable<PurchaseEstMasterVM> List()
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Count(ODataQueryOptions<PurchaseEstMasterVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Post(PurchaseEstMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, PurchaseEstMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Delete([FromBody] int id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Attachement
        /// <summary>
        /// Get all Purchase estimation Details.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("AllPurchaseEst/{companyCode}/{branchCode}")]
        [Route("AllPurchaseEst")]
        public IQueryable<AllPurchaseVM> GetAllPurchaseEstimations(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            IQueryable<AllPurchaseVM> purchase = new OldGoldPurchaseBL().GetAllPurchaseEstimations(companyCode, branchCode);
            return purchase;
        }

        /// <summary>
        /// Get Purrchase Estimation Count.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("AllPurchaseEstCount/{companyCode}/{branchCode}")]
        [Route("AllPurchaseEstCount")]
        public IHttpActionResult GetAllOrdersCount(string companyCode, string branchCode)
        {
            int count = new OldGoldPurchaseBL().GetAllOrdersCount(companyCode, branchCode);
            return Ok(new { RecordCount = count });
        }

        /// <summary>
        /// Get Search parameters for Purchase Estimation Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SearchParams")]
        public IHttpActionResult GetAllSearchParams()
        {
            List<SearchParamVM> lstSearchParams = new List<SearchParamVM>();
            lstSearchParams.Add(new SearchParamVM() { Key = "ESTNO", Value = "Est No" });
            lstSearchParams.Add(new SearchParamVM() { Key = "CUSTOMER", Value = "Customer" });
            lstSearchParams.Add(new SearchParamVM() { Key = "DATE", Value = "Date" });
            lstSearchParams.Add(new SearchParamVM() { Key = "AMOUNT", Value = "Amount" });
            lstSearchParams.Add(new SearchParamVM() { Key = "GROSSWT", Value = "Gross Wt" });
            lstSearchParams.Add(new SearchParamVM() { Key = "QTY", Value = "Quantity" });
            lstSearchParams.Add(new SearchParamVM() { Key = "CUSTID", Value = "Customer ID" });
            lstSearchParams.Add(new SearchParamVM() { Key = "GSTAMOUNT", Value = "GST Amount" });
            return Ok(lstSearchParams);
        }

        /// <summary>
        /// Get search results by sending search parameters.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="searchType"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Search/{companyCode}/{branchCode}/{searchType}/{searchValue}")]
        [Route("Search")]
        public IQueryable<AllPurchaseVM> GetAllSearchResult(string companyCode, string branchCode, string searchType, string searchValue)
        {
            IQueryable<AllPurchaseVM> purchase = new OldGoldPurchaseBL().GetAllSearchResult(companyCode, branchCode, searchType, searchValue);
            return purchase;
        }

        /// <summary>
        /// Get Attached purchase estimation Details by Estimation Number.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAttachment/{companyCode}/{branchCode}/{estNo}")]
        [Route("GetAttachment")]
        public IHttpActionResult GetAttachedOldGoldPurchase(string companyCode, string branchCode, int estNo)
        {
            ErrorVM error = new ErrorVM();
            List<PurchaseEstMasterVM> lstAllOldGoldPurchase = new OldGoldPurchaseBL().GetAttachedOldGoldPurchase(companyCode, branchCode, estNo, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(lstAllOldGoldPurchase);
        }

        /// <summary>
        /// Save/Attach Purchase Estimation details.
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostAttachment")]
        public IHttpActionResult AttachOldGoldPurchaseEstimation(List<PaymentVM> payment)
        {
            ErrorVM error = new ErrorVM();
            bool attached = new OldGoldPurchaseBL().AttachOldGoldPurchaseEstimation(payment, out error);
            if (error != null && attached == false) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok();
        }

        /// <summary>
        /// Remove Attachments to Estimation Number
        /// </summary>
        /// <param name="estNo"></param>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("RemoveAttachment/{companyCode}/{branchCode}/{EstNo}")]
        [Route("RemoveAttachment")]
        public IHttpActionResult RemoveOldGoldAttachement(string companyCode, string branchCode, int estNo)
        {
            ErrorVM error = new ErrorVM();
            bool attached = new OldGoldPurchaseBL().RemoveOldGoldAttachement(companyCode, branchCode, estNo, out error);
            if (error != null && attached == false) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok();
        }

        /// <summary>
        /// Get Attached Purchase Estimation information by Estimation Number.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAttachedOGEst/{companyCode}/{branchCode}/{estNo}")]
        [Route("GetAttachedOGEst")]
        public IHttpActionResult GetAttachedEstimation(string companyCode, string branchCode, int estNo)
        {
            ErrorVM error = new ErrorVM();
            List<AllPurchaseVM> purchase = new OldGoldPurchaseBL().GetAttachedEstimation(companyCode, branchCode, estNo, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(purchase);
        }
        #endregion
        #endregion
    }
}
