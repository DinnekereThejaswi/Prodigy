using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

/// <summary>
/// Author: Mustureswar M M (Eshwar)
/// Date: 02-06-2021
/// </summary>
namespace ProdigyAPI.Controllers.Masters
{
    /// <summary>
    /// This Provides API's Related to Minimum Stock Master (Reorder Level Master/ ROL Master)
    /// </summary>

    [Authorize]
    [RoutePrefix("api/ROL")]
    public class MinimumStockController : ApiController
    {
        /// <summary>
        /// Get GS Code
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GS/{companyCode}/{branchCode}")]
        public IHttpActionResult GetGS(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = new MinimumStockBL().GetGS(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(System.Net.HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Get Items by GSCode
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Item/{companyCode}/{branchCode}/{gsCode}")]
        public IHttpActionResult GetItem(string companyCode, string branchCode, string gsCode)
        {
            ErrorVM error = new ErrorVM();
            var data = new MinimumStockBL().GetItem(companyCode, branchCode, gsCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(System.Net.HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Get Designs
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Design/{companyCode}/{branchCode}")]
        public IHttpActionResult GetDesign(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = new MinimumStockBL().GetDesigns(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(System.Net.HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Counter code by GSCode and Item Code
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsCode"></param>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Counter/{companyCode}/{branchCode}/{gsCode}/{itemCode}")]
        public IHttpActionResult GetDesign(string companyCode, string branchCode, string gsCode, string itemCode)
        {
            ErrorVM error = new ErrorVM();
            var data = new MinimumStockBL().GetCounter(companyCode, branchCode, gsCode, itemCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(System.Net.HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Get Minimum Stock Details (ROL Details/ Reorder Level Details) by GSCode and ItemCode
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsCode"></param>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Stock/{companyCode}/{branchCode}/{gsCode}/{itemCode}")]
        public IHttpActionResult GetMinStock(string companyCode, string branchCode, string gsCode, string itemCode)
        {
            ErrorVM error = new ErrorVM();
            var data = new MinimumStockBL().GetMinStock(companyCode, branchCode, gsCode, itemCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(System.Net.HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Save Minimum Stock Details (ROL Details/ Reorder Level Details).
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Stock")]
        public IHttpActionResult SaveStockDetails(MinStcokVM stock)
        {
            ErrorVM error = new ErrorVM();
            var data = new MinimumStockBL().Save(stock, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(System.Net.HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Update Minimum Stock Details (ROL Details/ Reorder Level Details).
        /// </summary>
        /// <param name="objID"></param>
        /// <param name="stock"></param>
        /// <returns></returns> [FromUri] string objID,
        [HttpPut]
        [Route("Put")]
        public IHttpActionResult UpdateStockDetails([FromBody] MinStcokVM stock)
        {
            ErrorVM error = new ErrorVM();
            var data = new MinimumStockBL().Update(stock.ObjID, stock, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(System.Net.HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Delete Minimum Stock Details (ROL Details/ Reorder Level Details).
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="objID"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Delete/{companyCode}/{branchCode}/{objID}")]
        public IHttpActionResult DeleteStockDetails(string companyCode, string branchCode, string objID)
        {
            ErrorVM error = new ErrorVM();
            var data = new MinimumStockBL().Delete(objID, companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(System.Net.HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Get Print by GSCode and ItemCode
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsCode"></param>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Print/{companyCode}/{branchCode}/{gsCode}/{itemCode}")]
        public IHttpActionResult GetPrint(string companyCode, string branchCode, string gsCode, string itemCode)
        {
            ErrorVM error = new ErrorVM();
            var data = new MinimumStockBL().PrintMinStock(companyCode, branchCode, gsCode, itemCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(System.Net.HttpStatusCode.BadRequest, error);
            }
        }
    }
}
