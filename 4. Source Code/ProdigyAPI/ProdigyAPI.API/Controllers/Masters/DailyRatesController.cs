using ProdigyAPI.BL.BusinessLayer.BatchPosting;
using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Framework;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData.Query;

namespace ProdigyAPI.Controllers.Masters
{
    /// <summary>
    /// Daily rates controller for managing rates (both for selling and purchase rate management)
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Master/DailyRates")]
    public class DailyRatesController : SIBaseApiController<DailyRatesBL>
    {
        MagnaDbEntities db = new MagnaDbEntities(true);

        /// <summary>
        /// Use this method to get the selling and purchase rates
        /// </summary>
        /// <param name="companyCode">Company code query string</param>
        /// <param name="branchCode">Branch code query string</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Get")]
        [ResponseType(typeof(DailyRateVM))]
        public IHttpActionResult Get(string companyCode, string branchCode)
        {
            DailyRateVM dailyRates = null;
            ErrorVM error = null;
            bool success = new DailyRatesBL().GetDailyRates(companyCode, branchCode, out dailyRates, out error);
            if (success)
                return Ok(dailyRates);
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// This API should be used to update rates. Please note that all rates will be updated irrespective of whether 
        /// the rate is changed or not. It is required to submit the rates if any one of the rates is changed.
        /// Note: This version of API does not support date change.
        /// </summary>
        /// <param name="performDayend">Indicates if the client proceeds with the day end.</param>
        /// <param name="dailyRates">Daily Rates view model body</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post/{performDayend}")]
        public IHttpActionResult Post(bool performDayend, [FromBody] DailyRateVM dailyRates)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            string userId = base.GetUserId();
            ErrorVM error = null;

            DailyRatesBL blObject = new DailyRatesBL();
            if(blObject.Post(dailyRates, userId, performDayend, out error))
                return Ok();
            else
                return Content(error.ErrorStatusCode, error);
        }

        private string GetLoggedInUser()
        {
            string userId = "--";
            try {
                var claimsIdentity = RequestContext.Principal.Identity as ClaimsIdentity;
                if (claimsIdentity == null) {
                    return userId;
                }
                var userIdClaim = claimsIdentity.Claims.Where(c => c.Type == "UserID").FirstOrDefault();
                if (userIdClaim != null)
                    userId = userIdClaim.Value;
            }
            catch (Exception) {
            }
            return userId;
        }
    }
}
