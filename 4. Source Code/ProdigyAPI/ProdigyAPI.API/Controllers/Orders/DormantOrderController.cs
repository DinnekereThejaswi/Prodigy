using ProdigyAPI.BL.BusinessLayer.Order;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

/// <summary>
/// Author: Mustureswara M M (Eshwar)
/// Date: 23rd July 2021
/// </summary>
namespace ProdigyAPI.Controllers.Orders
{
    /// <summary>
    /// This Provides API's related to Dormant Orders.
    /// </summary>

    [Authorize]
    [RoutePrefix("api/Dormant")]
    public class DormantOrderController : ApiController
    {
        #region Declaration
        ErrorVM error = new ErrorVM();
        DormantOrderBL dormant = DormantOrderBL.GetInstance;
        #endregion

        #region Controller Methods

        /// <summary>
        /// This provides list of Orders bellow the order date which is specified. The orders are non closed,
        /// non cancelled, the is_lock flag is N or null and bill number is zero.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        [ResponseType(typeof(List<OrderMasterVM>))]
        public IHttpActionResult GetOrders(string companyCode, string branchCode, DateTime date)
        {
            var data = dormant.GetOrderList(companyCode, branchCode, date, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// This provides list of Single Order for any date. The order provided is non closed,
        /// non cancelled, the is_lock flag is N or null and bill number is zero.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get")]
        [ResponseType(typeof(OrderMasterVM))]
        public IHttpActionResult GetOrder(string companyCode, string branchCode, int orderNo)
        {
            var data = dormant.GetOrder(companyCode, branchCode, orderNo, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// This provides Creating  orders to Dormant Orders.
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("Lock")]
        public IHttpActionResult CreateDormantOrder([FromBody] List<OrderMasterVM> orders)
        {
            var data = dormant.LockOrder(orders, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Gets the order to be unlocked.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="orderNo">The order number which is locked/dormant</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-locked-order")]
        [ResponseType(typeof(OrderMasterVM))]
        public IHttpActionResult GetLockedOrder(string companyCode, string branchCode, int orderNo)
        {
            var data = dormant.GetOrderToBeUnlocked(companyCode, branchCode, orderNo, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// This provides Unlock of Dormant orders.
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UnLock")]
        public IHttpActionResult UnlockDormantOrder(List<OrderMasterVM> orders)
        {
            var data = dormant.UnlockOrder(orders, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }
        #endregion
    }
}
