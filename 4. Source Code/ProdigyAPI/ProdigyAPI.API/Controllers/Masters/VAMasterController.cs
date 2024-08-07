
using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Print;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

/// <summary>
/// Author: Mustureswara M M (Eshwar)
/// Date: 16th June 2021
/// </summary>
namespace ProdigyAPI.Controllers.Masters
{
    /// <summary>
    /// This Provides API's Related to VAMaster in Masters
    /// </summary>
    [Authorize]
    [RoutePrefix("api/VAMaster")]
    public class VAMasterController : ApiController
    {
        #region Declaration
        private readonly VAMasterBL vaMaster = VAMasterBL.GetInstance;
        #endregion

        #region Controller Methods

        /// <summary>
        /// This Provides Vendor Details.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Vendor")]
        [Route("Vendor/{companyCode}/{branchCode}")]
        public IHttpActionResult GetVendor(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = vaMaster.GetVendor(companyCode, branchCode, out error);
            return error == null ? Ok(data) : Content(error.ErrorStatusCode, error);
        }

        /// <summary>
        /// This Provides GS
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GS")]
        [Route("GS/{companyCode}/{branchCode}")]
        public IHttpActionResult GetGS(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = vaMaster.GetGS(companyCode, branchCode, out error);
            return error == null ? Ok(data) : Content(error.ErrorStatusCode, error);
        }

        /// <summary>
        /// This Provides GS
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Items")]
        [Route("Items/{companyCode}/{branchCode}/{gsCode}")]
        public IHttpActionResult GetGS(string companyCode, string branchCode, string gsCode)
        {
            ErrorVM error = new ErrorVM();
            var data = vaMaster.GetItems(companyCode, branchCode, gsCode, out error);
            return error == null ? Ok(data) : Content(error.ErrorStatusCode, error);
        }

        /// <summary>
        /// This Provides GS
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Designs")]
        [Route("Designs/{companyCode}/{branchCode}")]
        public IHttpActionResult GetDesigns(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = vaMaster.GetDesgins(companyCode, branchCode, out error);
            return error == null ? Ok(data) : Content(error.ErrorStatusCode, error);
        }

        /// <summary>
        /// This Provides GS
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("MCTypes")]
        [Route("MCTypes/{companyCode}/{branchCode}")]
        public IHttpActionResult GetMCTypes(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = vaMaster.GetMCTypes(companyCode, branchCode, out error);
            return error == null ? Ok(data) : Content(error.ErrorStatusCode, error);
        }

        /// <summary>
        /// This Provides GS
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsCode"></param>
        /// <param name="itemCode"></param>
        /// <param name="designCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("VA")]
        [Route("VA/{companyCode}/{branchCode}/{gsCode}/{itemCode}/{designCode}")]
        public IHttpActionResult GetVADetails(string companyCode, string branchCode, string gsCode, string itemCode, string designCode)
        {
            ErrorVM error = new ErrorVM();
            var data = vaMaster.GetVADetails(companyCode, branchCode, gsCode, itemCode, designCode, out error);
            return error == null ? Ok(data) : Content(error.ErrorStatusCode, error);
        }

        /// <summary>
        /// This Provides GS
        /// </summary>
        /// <param name="va"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Post")]
        public IHttpActionResult SaveDetails([FromBody] List<VAMasterVM> va)
        {
            ErrorVM error = new ErrorVM();
            bool saved = vaMaster.Save(va, out error);
            if (saved) {
                return Ok();
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This Provides GS
        /// </summary>
        /// <param name="va"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Print")]
        public IHttpActionResult PrintVADetails([FromBody] VAMasterPrintVM va)
        {
            ErrorVM error = new ErrorVM();
            ProdigyPrintVM data = vaMaster.Print(va, out error);
            if (error == null) return Ok(data); else return Content(error.ErrorStatusCode, error);
        }

        /// <summary>
        /// This Provides GS
        /// </summary>
        /// <param name="va"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PrintItemwise")]
        public IHttpActionResult PrintItemWise([FromBody] VAMasterPrintVM va)
        {
            ErrorVM error = new ErrorVM();
            ProdigyPrintVM data = vaMaster.PrintItemWise(va, out error);
            if (error == null) return Ok(data); else return Content(error.ErrorStatusCode, error);
        }

        /// <summary>
        /// This Provides GS
        /// </summary>
        /// <param name="va"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PrintSupplierwise")]
        public IHttpActionResult PrintSupplierwise([FromBody] VAMasterPrintVM va)
        {
            ErrorVM error = new ErrorVM();
            ProdigyPrintVM data = vaMaster.PrintSupplierwise(va, out error);
            if (error == null) return Ok(data); else return Content(error.ErrorStatusCode, error);
        }

        /// <summary>
        /// Copy from One Vendor to Another
        /// </summary>
        /// <param name="va"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Copy")]
        public IHttpActionResult CopySupplier([FromBody] VAMasterCopyVM va)
        {
            ErrorVM error = new ErrorVM();
            var data = vaMaster.Copy(va, out error);
            if (error == null) return Ok(data); else return Content(error.ErrorStatusCode, error);
        }

        #endregion
    }
}
