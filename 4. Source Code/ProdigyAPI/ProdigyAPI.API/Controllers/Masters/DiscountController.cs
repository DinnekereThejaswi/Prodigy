using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Framework;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData.Query;

/// <summary>
/// Module: Discount Master
/// Author: Mustureswara M M (Eshwar)
/// Date: 10/11/2020
/// </summary>
namespace ProdigyAPI.Controllers.Masters
{
    /// <summary>
    /// This Provides API's Related to Discount Master
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Masters/Discount")]
    public class DiscountController : SIBaseApiController<DiscountMasterVM>, IBaseMasterActionController<DiscountMasterVM, DiscountMasterVM>
    {
        #region Supplementary Controller Methods
        /// <summary>
        /// Get Discount Period
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Get")]
        [Route("Get/{companyCode}/{branchCode}")]
        public IHttpActionResult Get(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var result = new DiscountBL().GetDiscountPeriod(companyCode, branchCode, out error);
            return error == null ? Ok(result) : Ok(error);
        }
        /// <summary>
        /// Save or Update the Discount period Details
        /// </summary>
        /// <param name="discount"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult Post(DiscountMasterVM discount)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            ErrorVM error = new ErrorVM();
            bool result = new DiscountBL().SaveDiscountPeriodDetails(discount, out error);
            if (result) {
                return Ok();
            }
            else {
                return Ok(error);
            }
        }
        #endregion

        #region Default Controller Methods
        public IHttpActionResult Count(ODataQueryOptions<DiscountMasterVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Delete([FromBody] int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<DiscountMasterVM> List()
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, DiscountMasterVM t)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}