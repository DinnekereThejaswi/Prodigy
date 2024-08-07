using ProdigyAPI.BL.BusinessLayer.Accounts;
using ProdigyAPI.BL.ViewModel.Accounts;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Orders;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

/// <summary>
/// Author: Eshwar
/// Date: 22/01/2020
/// </summary>
namespace ProdigyAPI.Controllers.Accounts
{
    /// <summary>
    /// This Provieds API's for Mater Group Related API's
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Accounts/MasterGroup")]
    public class MasterGroupController : ApiController
    {
        /// <summary>
        /// Get Group Types.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GroupType/{companyCode}/{branchCode}")]
        [Route("GroupType")]
        public IHttpActionResult GetGroupTypes(string companyCode, string branchCode)
        {
            List<OrderItemVM> groupTypes = new MasterGroupBL().GetGroupType(companyCode, branchCode);
            return Ok(groupTypes);
        }

        /// <summary>
        /// Get All Master Group Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("List/{companyCode}/{branchCode}")]
        [Route("List")]
        public IQueryable GetMasterGroupList(string companyCode, string branchCode)
        {
            return new MasterGroupBL().GetMasterGroupList(companyCode, branchCode);
        }

        /// <summary>
        /// Save Master Group Details
        /// </summary>
        /// <param name="masterGroup"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult SaveMasterGroupDetails([FromBody] MasterGroupVM masterGroup)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            bool created = new MasterGroupBL().SaveMasterGroupDetails(masterGroup, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok();
        }

        /// <summary>
        /// Update Master Group Details
        /// </summary>
        /// <param name="objID"></param>
        /// <param name="masterGroup"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put")]
        public IHttpActionResult UpdateMasterGroupDetails([FromUri] string objID, [FromBody] MasterGroupVM masterGroup)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            bool updated = new MasterGroupBL().UpdateMasterGroupDetails(objID, masterGroup, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok();
        }
    }
}
