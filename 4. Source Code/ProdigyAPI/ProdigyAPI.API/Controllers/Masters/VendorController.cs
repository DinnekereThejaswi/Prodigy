using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Controllers.Masters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

/// <summary>
/// Author: Mustureswara M M (Eshwar)
/// Date: 14th Jun 2021
/// </summary>
namespace ProdigyAPI.Controllers.Masters
{
    /// <summary>
    /// This Provides API's Related to Vendor Master/ Supplier Master
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Vendor")]
    public class VendorController : ApiController
    {
        #region Declaration
        VendorMasterBL vendor = VendorMasterBL.GetInstance;
        #endregion

        #region Controler Methods
        /// <summary>
        /// This Proveds TDS in Vendor Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("TDS")]
        [Route("TDS/{companyCode}/{branchCode}")]
        public IHttpActionResult GetTDS(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = vendor.GetTDS(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This Proveds Metal Type in Vendor Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("MetalType")]
        [Route("MetalType/{companyCode}/{branchCode}")]
        public IHttpActionResult GetMetalType(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = vendor.GetMetalType(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This Proveds GS Type in Vendor Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GSType")]
        [Route("GSType/{companyCode}/{branchCode}")]
        public IHttpActionResult GetGSType(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = vendor.GetGSType(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This Proveds Open Type in Vendor Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("OpenType")]
        [Route("OpenType/{companyCode}/{branchCode}")]
        public IHttpActionResult GetOpenType(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = vendor.GetOpenType(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This Proveds Group To in Vendor Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GroupTo")]
        [Route("GroupTo/{companyCode}/{branchCode}")]
        public IHttpActionResult GetGroupTo(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = vendor.GetGroupTo(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This Proveds Vendor Details
        /// </summary>
        /// <param name="objID"></param>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Vendor")]
        [Route("Vendor/{objID}/{companyCode}/{branchCode}")]
        public IHttpActionResult GetVendor(string objID, string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = vendor.GetVendor(objID, companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This Proveds Vendor Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("VendorList")]
        [Route("VendorList/{companyCode}/{branchCode}")]
        public IHttpActionResult GetVendorList(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = vendor.GetVendorList(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This provides to Save the Supplier Details
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult Post(SupplierMasterVM supplier)
        {
            ErrorVM error = new ErrorVM();
            var data = vendor.Save(supplier, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This provides to Save the Supplier Details
        /// </summary>
        /// <param name="objID"></param>
        /// <param name="supplier"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put")]
        public IHttpActionResult Put(string objID, SupplierMasterVM supplier)
        {
            ErrorVM error = new ErrorVM();
            var data = vendor.Update(objID, supplier, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This Proveds Vendor Details
        /// </summary>
        /// <param name="objID"></param>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("PrintList")]
        [Route("PrintList/{objID}/{companyCode}/{branchCode}")]
        public IHttpActionResult GetVendorList(string objID, string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = vendor.PrintVendor(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This Proveds Vendor Details
        /// </summary>
        /// <param name="objID"></param>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("PrintFullDet")]
        [Route("PrintFullDet/{objID}/{companyCode}/{branchCode}")]
        public IHttpActionResult GetPrintFullDet(string objID, string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = vendor.PrintVendorFullDet(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }
        #endregion
    }
}