using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Framework;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData.Query;

namespace ProdigyAPI.Controllers
{
    /// <summary>
    /// This Provides API's Related to Stock Group.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Master/StockGroup")]
    public class StockGroupController : SIBaseApiController<StockGroupVM>, IBaseMasterActionController<StockGroupVM, StockGroupVM>
    {
        #region Declaration
        MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Controller Methods

        /// <summary>
        /// List of StockGroup Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("List/{companyCode}/{branchCode}")]
        public IQueryable<StockGroupVM> List(string companyCode, string branchCode)
        {
            List<StockGroupVM> stockGroup = new StockGroupBL().GetAllStockGroup(companyCode, branchCode);
            return stockGroup.AsQueryable<StockGroupVM>();
        }

        /// <summary>
        /// Save StockGroup Details
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        [ResponseType(typeof(StockGroupVM))]
        public IHttpActionResult Post(StockGroupVM s)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            bool saved = new StockGroupBL().Save(s, out error);
            if (error == null) {
                return Ok();
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Edit StockGroup Details
        /// </summary>
        /// <param name="objID"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put/{ObjID}")]
        [ResponseType(typeof(StockGroupVM))]
        public IHttpActionResult Put(string objID, StockGroupVM s)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            bool saved = new StockGroupBL().Update(objID, s, out error);
            if (error == null) {
                return Ok();
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        public IHttpActionResult Count(ODataQueryOptions<StockGroupVM> oDataOptions)
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

        public IHttpActionResult Put(int id, StockGroupVM t)
        {
            throw new NotImplementedException();
        }

        IQueryable<StockGroupVM> IBaseMasterActionController<StockGroupVM, StockGroupVM>.List()
        {
            throw new NotImplementedException();
        }

        IHttpActionResult IBaseMasterActionController<StockGroupVM, StockGroupVM>.Count(ODataQueryOptions<StockGroupVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        IHttpActionResult IBaseMasterActionController<StockGroupVM, StockGroupVM>.Get(int id)
        {
            throw new NotImplementedException();
        }

        IHttpActionResult IBaseMasterActionController<StockGroupVM, StockGroupVM>.Post(StockGroupVM t)
        {
            throw new NotImplementedException();
        }

        IHttpActionResult IBaseMasterActionController<StockGroupVM, StockGroupVM>.Put(int id, StockGroupVM t)
        {
            throw new NotImplementedException();
        }

        IHttpActionResult IBaseMasterActionController<StockGroupVM, StockGroupVM>.Delete(int id)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
