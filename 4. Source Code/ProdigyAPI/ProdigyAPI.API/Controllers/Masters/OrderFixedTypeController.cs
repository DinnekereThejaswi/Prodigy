using ProdigyAPI.BL.BusinessLayer.Order;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData.Query;

/// <summary>
/// Author: Mustureswara M M (MMM Eshwar)
/// Date: 18/01/2021
/// </summary>
namespace ProdigyAPI.Controllers.Masters
{
    /// <summary>
    /// This Provides API's Related to Order Types (Fixed order Types.)
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Masters/OrderFixedType")]
    public class OrderFixedTypeController : SIBaseApiController<OrderRateMasterVM>, IBaseMasterActionController<OrderRateMasterVM, OrderRateMasterVM>
    {
        #region Supplymentary Methods
        /// <summary>
        /// Get All Order Plan Names
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("PlanNames/{companyCode}/{branchCode}")]
        public IHttpActionResult PlanNames(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = new OrderBL().GetPlanNames(companyCode, branchCode, out error);
            if (error.ErrorStatusCode == HttpStatusCode.OK) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }

        }

        /// <summary>
        /// Get All Order Type Details by Plan Name
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("OrderTypesDetails/{companyCode}/{branchCode}/{type}")]
        public IHttpActionResult GetOrderTypeDetails(string companyCode, string branchCode, string type)
        {
            ErrorVM error = new ErrorVM();
            var data = new OrderBL().GetOrderTypeDetails(companyCode, branchCode, type, out error);
            if (error.ErrorStatusCode == HttpStatusCode.OK) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }

        }

        /// Save Order Type Details.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult SaveOrderTypeDetails(OrderRateMasterVM order)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            order.OperatorCode = base.GetUserId();
            ErrorVM error = new ErrorVM();
            var data = new OrderBL().SaveOrderTypeDetails(order, out error);
            if (error.ErrorStatusCode == HttpStatusCode.OK) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }

        }

        /// <summary>
        /// Update Order Type Details.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put/{id}")]
        [Route("Put")]
        public IHttpActionResult UpdateOrderTypeDetails([FromUri] int id, [FromBody] OrderRateMasterVM order)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            order.OperatorCode = base.GetUserId();
            ErrorVM error = new ErrorVM();
            var data = new OrderBL().UpdateOrderTypeDetails(id, order, out error);
            if (error.ErrorStatusCode == HttpStatusCode.OK) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }

        }

        #endregion

        #region Default Controller Methods
        public IQueryable<OrderRateMasterVM> List()
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Count(ODataQueryOptions<OrderRateMasterVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Post(OrderRateMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, OrderRateMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Delete([FromBody] int id)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
