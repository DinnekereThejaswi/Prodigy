using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

/// <summary>
/// Author: Mustureswara M M (Eshwar)
/// Date: 12-01-2021
/// </summary>
namespace ProdigyAPI.Controllers.Masters
{
    /// <summary>
    /// This Provides API's realted to Packing Material.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Masters/PackingMaterial")]
    public class PackingMaterialController : ApiController
    {
        /// <summary>
        /// Provides the List of all Package Material Information.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("List/{companyCode}/{branchCode}")]
        public IHttpActionResult List(string companyCode, string branchCode)
        {
            return Ok(new PackingMaterialBL().GetAllPackingMaterial(companyCode, branchCode));
        }

        /// <summary>
        /// Save the Packing Material Information.
        /// </summary>
        /// <param name="pm"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult Save(PackingMaterialVM pm)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            ErrorVM error = new ErrorVM();
            bool saved = new PackingMaterialBL().SavePackingMaterial(pm, out error);
            if (saved) {
                return Ok();
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Save the Packing Material Information.
        /// </summary>
        /// <param name="objID"></param>
        /// <param name="pm"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put/{objID}")]
        public IHttpActionResult Save([FromUri] string objID, [FromBody] PackingMaterialVM pm)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            bool saved = new PackingMaterialBL().UpdatePackingMaterial(objID, pm, out error);
            if (saved) {
                return Ok();
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Provides UOM for Height and Length.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("HLW")]
        public IHttpActionResult GetHeightLengthWidthUOM()
        {
            List<dynamic> uom = new List<dynamic>();
            uom.Add(new { Code = "CM", Value = "CM" });
            uom.Add(new { Code = "M", Value = "M" });
            return Ok(uom);
        }

        /// <summary>
        /// Provides UOM for Height and Length.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Weight")]
        public IHttpActionResult GetWeightUOM()
        {
            List<dynamic> uom = new List<dynamic>();
            uom.Add(new { Code = "g", Value = "grams" });
            uom.Add(new { Code = "kg", Value = "kilograms" });
            return Ok(uom);
        }
    }
}
