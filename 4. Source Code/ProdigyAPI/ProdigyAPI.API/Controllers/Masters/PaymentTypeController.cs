using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Handlers;
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
    /// This controller handles CRUD operations of Payment Modes
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Masters/PaymentType")]
    public class PaymentTypeController : SIBaseApiController<PaymentMasterBL>
    {
        /// <summary>
        /// List API - Returns list of payment modes
        /// </summary>
        /// <param name="companyCode">The company code in question</param>
        /// <param name="branchCode">The branch code in question</param>
        /// <returns></returns>
        [Route("list")]
        [HttpGet]
        [ResponseType(typeof(List<PaymentMasterVM>))]
        public IHttpActionResult List(string companyCode, string branchCode)
        {
            PaymentMasterBL paymentsBL = new PaymentMasterBL();
            var list = paymentsBL.List(companyCode, branchCode);
            return Ok(list);
        }

        /// <summary>
        /// Gets a payment type
        /// </summary>
        /// <param name="companyCode">The company code in question</param>
        /// <param name="branchCode">The branch code in question</param>
        /// <param name="paymentCode">The payment code</param>
        /// <returns></returns>
        [Route("get")]
        [HttpGet]
        [ResponseType(typeof(PaymentMasterVM))]
        public IHttpActionResult Get(string companyCode, string branchCode, string paymentCode)
        {
            PaymentMasterBL paymentsBL = new PaymentMasterBL();
            var data = paymentsBL.Get(companyCode, branchCode, paymentCode);
            if (data != null)
                return Ok(data);
            else
                return NotFound();
        }

        /// <summary>
        /// Adds a new payment type
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="payment"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("post")]
        public IHttpActionResult Post(string companyCode, string branchCode, PaymentMasterVM payment)
        {
            PaymentMasterBL paymentsBL = new PaymentMasterBL();
            ErrorVM error = null;
            var success = paymentsBL.Add(companyCode, branchCode, payment, out error);
            if (success)
                return Ok();
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// Modifies an existing payment type
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="payment"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("put")]
        public IHttpActionResult Put(string companyCode, string branchCode, PaymentMasterVM payment)
        {
            PaymentMasterBL paymentsBL = new PaymentMasterBL();
            ErrorVM error = null;
            var success = paymentsBL.Modify(companyCode, branchCode, payment, out error);
            if (success)
                return Ok();
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// Deletes payment type
        /// </summary>
        /// <param name="companyCode">The company code in question</param>
        /// <param name="branchCode">The branch code in question</param>
        /// <param name="paymentCode">The payment code</param>
        /// <returns></returns>
        [Route("delete")]
        [HttpPost]
        [ResponseType(typeof(PaymentMasterVM))]
        public IHttpActionResult Delete(string companyCode, string branchCode, string paymentCode)
        {
            PaymentMasterBL paymentsBL = new PaymentMasterBL();
            ErrorVM error = null;
            var result = paymentsBL.Delete(companyCode, branchCode, paymentCode, out error);
            if (result)
                return Ok();
            else
                return Content(HttpStatusCode.BadRequest, error);
        }        
    }
}
