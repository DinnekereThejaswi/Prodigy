using ProdigyAPI.BL.BusinessLayer.AccessManagement;
using ProdigyAPI.BL.ViewModel.AccessManagement;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Handlers;
using ProdigyAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace ProdigyAPI.Controllers.Auth
{
    /// <summary>
    /// Controller for IP Address Management. Consists of 3 API, 1) List, 2) Post, 3) Put
    /// </summary>
    [Authorize]
    [RoutePrefix("api/ip-address-mgmt")]
    public class IPAddressManagementController : SIBaseApiController<IPSettingsVM>
    {
        /// <summary>
        /// List all the IPSettings depending on the condition
        /// </summary>
        /// <param name="allowDeny">This should either be Allow or Deny or All</param>
        /// <param name="status">Boolean parameter</param>
        /// <returns></returns>
        [HttpGet]
        [Route("List")]
        [ResponseType(typeof(List<IPSettingsVM>))]
        public IHttpActionResult List(string allowDeny, bool status)
        {
            try {
                var data = new IPAddressManagement().ListAll(allowDeny, status);
                return Ok(data);
            }
            catch (Exception ex) {
                ErrorVM error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = ex.Message };
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Add new IP Settings
        /// </summary>
        /// <param name="ipSeries">IP Series object</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        [ResponseType(typeof(IPSettingsVM))]
        public IHttpActionResult Post(IPSettingsVM ipSeries)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = null;
            string userID = base.GetUserId();
            var success = new IPAddressManagement().Add(ipSeries, userID, out error);
            if (success == true)
                return Ok(ipSeries);
            else
                return Content(error.ErrorStatusCode, error);
        }

        /// <summary>
        /// Update existing IP Setting
        /// </summary>
        /// <param name="ipSeries">IP Series object</param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put")]
        [ResponseType(typeof(IPSettingsVM))]
        public IHttpActionResult Put(IPSettingsVM ipSeries)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = null;
            string userID = base.GetUserId();
            var success = new IPAddressManagement().Modify(ipSeries, userID, out error);
            if (success == true)
                return Ok(ipSeries);
            else
                return Content(error.ErrorStatusCode, error);
        }
        
        [HttpPost]
        [Route("validate-ipv4/{ipAddress}")]
        public IHttpActionResult ValidateIPV4(string ipAddress)
        {
            ErrorInfo error = null;
            string errorMessage = string.Empty;
            IPAddress4Validation ipValidator = new IPAddress4Validation();
            bool ipAddressValidated = ipValidator.ValidateStaticIPAddress(ipAddress, out errorMessage);
            if (ipAddressValidated) {
                return Ok();
            }
            else {
                error = new ErrorInfo
                {
                    ErrorCode = "SI0003",
                    Description = errorMessage,
                    ErrorDescription = errorMessage
                };
                return Content(HttpStatusCode.Forbidden, error);
            }
        }

    }
}
