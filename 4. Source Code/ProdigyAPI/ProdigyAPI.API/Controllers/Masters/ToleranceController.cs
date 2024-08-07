﻿using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Framework;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData.Query;

namespace ProdigyAPI.Controllers.Masters
{
    /// <summary>
    /// Use these API to maintain tolerance
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Master/Tolerance")]
    public class ToleranceController : SIBaseApiController<ToleranceMasterVM>
    {
        ToleranceMasterBL bl = new ToleranceMasterBL();
        /// <summary>
        /// Gets the list of tolerances for the given companyCode and branchCode
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        [ResponseType(typeof(List<ToleranceMasterVM>))]
        public IHttpActionResult List(string companyCode, string branchCode)
        {
            ErrorVM error = null;
            var data = bl.List(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);

        }

        /// <summary>
        /// Retrives a tolerance
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="toleranceId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get")]
        [ResponseType(typeof(ToleranceMasterVM))]
        public IHttpActionResult Get(string companyCode, string branchCode, int toleranceId)
        {
            ErrorVM error = null;
            var data = bl.Get(companyCode, branchCode, toleranceId, out error);
            if (data != null) {
                return Ok(data);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// Posts a tolerance
        /// </summary>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("post")]
        public IHttpActionResult Post(ToleranceMasterVM tolerance)
        {
            ErrorVM error = null;
            var result = bl.Add(tolerance, base.GetUserId(), out error);
            if (result == true) {
                return Ok();
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// Updates a tolerance
        /// </summary>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("put")]
        public IHttpActionResult Put(ToleranceMasterVM tolerance)
        {
            ErrorVM error = null;
            var result = bl.Modify(tolerance, base.GetUserId(), out error);
            if (result == true) {
                return Ok();
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// Removes/deletes a tolerance
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="toleranceId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("delete/{companyCode}/{branchCode}/{toleranceId}")]
        public IHttpActionResult Delete(string companyCode, string branchCode, int toleranceId)
        {
            ErrorVM error = null;
            var result = bl.Delete(companyCode, branchCode, toleranceId, base.GetUserId(), out error);
            if (result == true) {
                return Ok();
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }
        
        
    }
}