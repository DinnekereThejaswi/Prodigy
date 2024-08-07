using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.Handlers;
using ProdigyAPI.BL.ViewModel.Master;
using System.Web.Http.Description;
using ProdigyAPI.BL.ViewModel.Error;

namespace ProdigyAPI.Controllers.Masters
{
    [RoutePrefix("api/Master/StoneDiamondMaster")]
    public class StoneDiamondController : SIBaseApiController<StoneDiamondBL>
    {
        #region Diamond Colour/Cut/Shape/Symmetry/Size etc. All API share common code expect the function name.
        /// <summary>
        /// Gets diamond colour
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <returns>Returns Code and Name object list</returns>
        [HttpGet]
        [Route("get-diamond-colour")]
        [Route("get-diamond-colour/{companyCode}/{branchCode}")]
        [ResponseType(typeof(List<ListOfValue>))]
        public IHttpActionResult GetDiamondColor(string companyCode, string branchCode)
        {
            var listOfValues = new StoneDiamondBL().GetDiamondColor(companyCode, branchCode);
            if (listOfValues != null)
                return Ok(listOfValues);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to load the details." });
            }
        }

        /// <summary>
        /// Gets diamond cut
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <returns>Returns Code and Name object list</returns>
        [HttpGet]
        [Route("get-diamond-cut")]
        [Route("get-diamond-cut/{companyCode}/{branchCode}")]
        [ResponseType(typeof(List<ListOfValue>))]
        public IHttpActionResult GetDiamondCut(string companyCode, string branchCode)
        {
            var listOfValues = new StoneDiamondBL().GetDiamondCut(companyCode, branchCode);
            if (listOfValues != null)
                return Ok(listOfValues);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to load the details." });
            }
        }


        /// <summary>
        /// Gets diamond clarity
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <returns>Returns Code and Name object list</returns>
        [HttpGet]
        [Route("get-diamond-clarity")]
        [Route("get-diamond-clarity/{companyCode}/{branchCode}")]
        [ResponseType(typeof(List<ListOfValue>))]
        public IHttpActionResult GetDiamondClarity(string companyCode, string branchCode)
        {
            var listOfValues = new StoneDiamondBL().GetDiamondClarity(companyCode, branchCode);
            if (listOfValues != null)
                return Ok(listOfValues);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to load the details." });
            }
        }


        /// <summary>
        /// Gets diamond certificate
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <returns>Returns Code and Name object list</returns>
        [HttpGet]
        [Route("get-diamond-certificate")]
        [Route("get-diamond-certificate/{companyCode}/{branchCode}")]
        [ResponseType(typeof(List<ListOfValue>))]
        public IHttpActionResult GetDiamondCertificate(string companyCode, string branchCode)
        {
            var listOfValues = new StoneDiamondBL().GetDiamondCertificate(companyCode, branchCode);
            if (listOfValues != null)
                return Ok(listOfValues);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to load the details." });
            }
        }


        /// <summary>
        /// Gets diamond shape
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <returns>Returns Code and Name object list</returns>
        [HttpGet]
        [Route("get-diamond-shape")]
        [Route("get-diamond-shape/{companyCode}/{branchCode}")]
        [ResponseType(typeof(List<ListOfValue>))]
        public IHttpActionResult GetDiamondShape(string companyCode, string branchCode)
        {
            var listOfValues = new StoneDiamondBL().GetDiamondShape(companyCode, branchCode);
            if (listOfValues != null)
                return Ok(listOfValues);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to load the details." });
            }
        }

        /// <summary>
        /// Gets diamond polish
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <returns>Returns Code and Name object list</returns>
        [HttpGet]
        [Route("get-diamond-polish")]
        [Route("get-diamond-polish/{companyCode}/{branchCode}")]
        [ResponseType(typeof(List<ListOfValue>))]
        public IHttpActionResult GetDiamondPolish(string companyCode, string branchCode)
        {
            var listOfValues = new StoneDiamondBL().GetDiamondPolish(companyCode, branchCode);
            if (listOfValues != null)
                return Ok(listOfValues);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to load the details." });
            }
        }

        /// <summary>
        /// Gets diamond symmetry
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <returns>Returns Code and Name object list</returns>
        [HttpGet]
        [Route("get-diamond-symmetry")]
        [Route("get-diamond-symmetry/{companyCode}/{branchCode}")]
        [ResponseType(typeof(List<ListOfValue>))]
        public IHttpActionResult GetDiamondSymmetry(string companyCode, string branchCode)
        {
            var listOfValues = new StoneDiamondBL().GetDiamondSymmetry(companyCode, branchCode);
            if (listOfValues != null)
                return Ok(listOfValues);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to load the details." });
            }
        }

        /// <summary>
        /// Gets diamond fluorescence
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <returns>Returns Code and Name object list</returns>
        [HttpGet]
        [Route("get-diamond-fluorescence")]
        [Route("get-diamond-fluorescence/{companyCode}/{branchCode}")]
        [ResponseType(typeof(List<ListOfValue>))]
        public IHttpActionResult GetDiamondFluorescence(string companyCode, string branchCode)
        {
            var listOfValues = new StoneDiamondBL().GetDiamondFluorescence(companyCode, branchCode);
            if (listOfValues != null)
                return Ok(listOfValues);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to load the details." });
            }
        }

        /// <summary>
        /// Gets diamond size
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <returns>Returns Code and Name object list</returns>
        [HttpGet]
        [Route("get-diamond-size")]
        [Route("get-diamond-size/{companyCode}/{branchCode}")]
        [ResponseType(typeof(List<ListOfValue>))]
        public IHttpActionResult GetDiamondSize(string companyCode, string branchCode)
        {
            var listOfValues = new StoneDiamondBL().GetDiamondSize(companyCode, branchCode);
            if (listOfValues != null)
                return Ok(listOfValues);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to load the details." });
            }
        }
        #endregion

        #region Stone Master API
        /// <summary>
        /// Gets stone type
        /// </summary>
        /// <returns>Returns Code and Name object list</returns>
        [HttpGet]
        [Route("get-stone-type")]
        [ResponseType(typeof(List<ListOfValue>))]
        public IHttpActionResult GetStoneType()
        {
            var listOfValues = new StoneDiamondBL().GetStoneType();
            if (listOfValues != null)
                return Ok(listOfValues);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to load the details." });
            }
        }

        /// <summary>
        /// Gets the list of all top 1000 stone details for the selected stoneType. 
        /// If the stoneType is null or empty, all stones are returned irrespective of the stoneType
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="stoneType">Output of get-stone-type API</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-stone-list")]
        [Route("get-stone-list/{companyCode}/{branchCode}/{stoneType}")]
        [ResponseType(typeof(IQueryable<StoneMasterVM>))]
        public IHttpActionResult GetStoneList(string companyCode, string branchCode, string stoneType)
        {
            var result = new StoneDiamondBL().GetStoneOrDiamondList(companyCode, branchCode, stoneType, false);
            if (result != null)
                return Ok(result);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to load the details." });
            }
        }

        /// <summary>
        /// Gets single stone detail for the selected code. 
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="code">Stone code/name (Note that code and name are same). Pass stone name</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-stone-detail")]
        [Route("get-stone-detail/{companyCode}/{branchCode}/{code}")]
        [ResponseType(typeof(StoneMasterVM))]
        public IHttpActionResult GetStoneDetail(string companyCode, string branchCode, string code)
        {
            var result = new StoneDiamondBL().GetStoneOrDiamondDetail(companyCode, branchCode, code, false);
            if (result != null) {
                return Ok(result);
            }
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to load the details." });
            }
        }
        #endregion

        #region Diamond Master API
        /// <summary>
        /// Gets the list of all top 1000 diamond details.
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        [HttpGet]
        [Route("get-diamond-list")]
        [Route("get-diamond-list/{companyCode}/{branchCode}")]
        [ResponseType(typeof(IQueryable<StoneMasterVM>))]
        public IHttpActionResult GetDiamondList(string companyCode, string branchCode)
        {
            var result = new StoneDiamondBL().GetStoneOrDiamondList(companyCode, branchCode, string.Empty, true);
            if (result != null)
                return Ok(result);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to load the details." });
            }
        }

        /// <summary>
        /// Gets single diamond detail for the selected code. 
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="code">Stone code/name (Note that code and name are same). Pass stone name</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-diamond-detail")]
        [Route("get-diamond-detail/{companyCode}/{branchCode}/{code}")]
        [ResponseType(typeof(StoneMasterVM))]
        public IHttpActionResult GetDiamondDetail(string companyCode, string branchCode, string code)
        {
            var result = new StoneDiamondBL().GetStoneOrDiamondDetail(companyCode, branchCode, code, true);
            if (result != null) {
                return Ok(result);
            }
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to load the details." });
            }
        }
        #endregion
    }
}
