using ProdigyAPI.BL.BusinessLayer;
using ProdigyAPI.BL.BusinessLayer.Sales;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Payment;
using ProdigyAPI.BL.ViewModel.Sales;
using ProdigyAPI.Controllers.Masters;
using ProdigyAPI.Framework;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData.Query;
using System.Web.Http.Results;

/// <summary>
/// Module: Tag Split
/// Author: Mustureswara M M
/// Date: 16/06/2019
/// </summary>
namespace ProdigyAPI.Controllers.Sales
{
    /// <summary>
    /// This provides API's for Tag Splitting and there functionalities.
    /// </summary>
    //[Authorize]
    [RoutePrefix("api/TagSplit")]
    public class TagSplitController : ApiController
    {
        /// <summary>
        /// Get Design Details.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Design/{companyCode}/{branchCode}")]
        [Route("Design")]
        public IHttpActionResult GetDesignMaster(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            List<GenComboVM> result = new TagSplitBL().GetDesign(companyCode, branchCode, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(result);
        }
        /// <summary>
        /// Provide Size details.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Size/{companyCode}/{branchCode}")]
        [Route("Size")]
        public IHttpActionResult GetSize(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            List<GenComboVM> result = new TagSplitBL().GetSize(companyCode, branchCode, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(result);
        }

        /// <summary>
        /// Provide Stone GS Type details.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("StoneGSType/{companyCode}/{branchCode}")]
        [Route("StoneGSType")]
        public IHttpActionResult GetStoneType(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            List<GenComboVM> result = new TagSplitBL().GetStoneGSType(companyCode, branchCode, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(result);
        }

        /// <summary>
        /// Provide Stone Name details.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="stoneGSType"></param>
        /// <param name="stoneType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("StoneName/{companyCode}/{branchCode}/{stoneGSType}/{stoneType}")]
        [Route("StoneName")]
        public IHttpActionResult GetStoneName(string companyCode, string branchCode, string stoneGSType, string stoneType)
        {
            ErrorVM error = new ErrorVM();
            List<GenComboVM> result = new TagSplitBL().GetStoneName(companyCode, branchCode, stoneGSType, stoneType, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get Tag (Barcode) Information.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="tagNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("TagInfo/{companyCode}/{branchCode}/{tagNo}")]
        public IHttpActionResult GetTagInformation(string companyCode, string branchCode, string tagNo)
        {
            ErrorVM error = new ErrorVM();
            List<TagVM> tag = new TagSplitBL().GetTagDetails(companyCode, branchCode, tagNo, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(tag);
        }

        /// <summary>
        /// This provides Calculation of Second barcode details.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SplitBarcode/{companyCode}/{branchCode}")]
        public IHttpActionResult SplitBarcodeDetailsWithCalculation(string companyCode, string branchCode, [FromBody] List<TagVM> tag)
        {
            ErrorVM error = new ErrorVM();
            List<TagVM> tagDet = new TagSplitBL().SplitBarcodeDetailsWithCalculation(companyCode, branchCode, tag, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(tagDet);
        }

        /// <summary>
        /// Splitting and Save barcode details
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GenerateBarcode")]
        public IHttpActionResult GenerateBarcode([FromBody] List<TagVM> tag)
        {
            //IHttpActionResult actionResult = new BarcodeController().GetBarcodeWithStoneForSales(companyCode, branchCode, "39657819", "");
            //var contentResult = actionResult as OkNegotiatedContentResult<SalesEstDetailsVM>;
            //SalesEstDetailsVM barcode = (SalesEstDetailsVM)contentResult.Content;


            ErrorVM error = new ErrorVM();
            var tagDet = new TagSplitBL().SplitAndSaveBarcode(tag, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok("Tag No: " + tagDet[0].ToString() + " and " + tagDet[1].ToString() + " Saved Successfully.");
        }
    }
}
