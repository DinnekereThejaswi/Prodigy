using ProdigyAPI.BL.BusinessLayer.Accounts;
using ProdigyAPI.BL.ViewModel.Accounts;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Orders;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

/// <summary>
/// Author: Eshwar 
/// Date: 23/01/2020
/// </summary>
namespace ProdigyAPI.Controllers.Accounts
{
    /// <summary>
    /// Provides API's for Sub Group Module in Accounts.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Accounts/SubGroup")]
    public class SubGroupController : ApiController
    {
        /// <summary>
        /// Get All Master Group (Parent Group) Details.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ParentGroup/{companyCode}/{branchCode}")]
        [Route("ParentGroup")]
        public IHttpActionResult GetMasterGroup(string companyCode, string branchCode)
        {
            return Ok(new SubGroupBL().GetAllMasterGroup(companyCode, branchCode));
        }

        /// <summary>
        /// Get List of All Sub Gorup Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("List/{companyCode}/{branchCode}")]
        [Route("List")]
        public IQueryable GetSubGroupList(string companyCode, string branchCode)
        {
            return new SubGroupBL().GetSubGroupList(companyCode, branchCode);
        }

        /// <summary>
        /// Save Sub Group Details
        /// </summary>
        /// <param name="masterGroup"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult SaveMasterGroupDetails([FromBody] SubGroupVM masterGroup)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            bool created = new SubGroupBL().SaveSubGroupDetails(masterGroup, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok();
        }

        /// <summary>
        /// Update Sub Group Details
        /// </summary>
        /// <param name="objID"></param>
        /// <param name="subGroup"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put")]
        public IHttpActionResult UpdateMasterGroupDetails([FromUri] string objID, [FromBody] SubGroupVM subGroup)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            bool updated = new SubGroupBL().UpdateSubGroupDetails(objID, subGroup, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok();
        }
    }
}
