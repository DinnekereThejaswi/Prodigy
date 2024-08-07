using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Print;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

/// <summary>
/// Author: Mustureswara M M (Eshwar)
/// Date: 5th July 2021
/// </summary>
namespace ProdigyAPI.Controllers.Masters
{
    /// <summary>
    /// This Provides API's related to SKU Master
    /// </summary>
    [Authorize]
    [RoutePrefix("api/SKU")]
    public class SKUController : ApiController
    {
        #region Declaration
        SKUMasterBL skuMaster = SKUMasterBL.GetInstace;
        ErrorVM error = new ErrorVM();
        #endregion

        #region Controller Methods
        /// <summary>
        /// This Method Provides SKU Master Details.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("SKUDet/{companyCode}/{branchCode}")]
        [Route("SKUDet")]
        public IHttpActionResult GetSKUMasterDetails([FromUri] string companyCode, [FromUri] string branchCode)
        {
            var data = skuMaster.GetSKUMasterDetails(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This Method Provides GS Details.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GS/{companyCode}/{branchCode}")]
        [Route("GS")]
        public IHttpActionResult GetGS([FromUri] string companyCode, [FromUri] string branchCode)
        {
            var data = skuMaster.GetGS(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This Method Provides Item Details by GS Code.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Item/{companyCode}/{branchCode}/{gsCode}")]
        [Route("Item")]
        public IHttpActionResult GetItem([FromUri]string companyCode, [FromUri] string branchCode, [FromUri] string gsCode)
        {
            var data = skuMaster.GetItem(companyCode, branchCode, gsCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This Method Provides Item Details by GS Code.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsCode"></param>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Design/{companyCode}/{branchCode}/{gsCode}/{itemCode}")]
        [Route("Design")]
        public IHttpActionResult GetDesigns([FromUri]string companyCode, [FromUri] string branchCode, [FromUri] string gsCode, [FromUri] string itemCode)
        {
            var data = skuMaster.GetDesigns(companyCode, branchCode, gsCode, itemCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Save SKU Master Details.
        /// </summary>
        /// <param name="skum"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult Save([FromBody] SKUVM skum)
        {
            var data = skuMaster.Save(skum, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Save SKU Master Details.
        /// </summary>
        /// <param name="objID"></param>
        /// <param name="skum"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put/{objID}")]
        public IHttpActionResult Save([FromUri] string objID, [FromBody] SKUVM skum)
        {
            var data = skuMaster.Save(skum, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Save SKU Master Details.
        /// </summary>
        /// <param name="objID"></param>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Put/{objID}")]
        public IHttpActionResult Delete([FromUri] string objID, string companyCode, string branchCode)
        {
            var data = skuMaster.Delete(objID,companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This Provides GS
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Print/{companyCode}/{branchCode}")]
        [Route("Print")]
        public IHttpActionResult Print([FromUri] string companyCode, [FromUri] string branchCode)
        {
            ErrorVM error = new ErrorVM();
            ProdigyPrintVM data = skuMaster.Print(companyCode, branchCode, out error);
            if (error == null) return Ok(data); else return Content(error.ErrorStatusCode, error);
        }
        #endregion
    }
}
