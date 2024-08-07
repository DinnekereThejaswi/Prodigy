using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProdigyAPI.BL.ViewModel.OPGProcessing;
using ProdigyAPI.Handlers;
using ProdigyAPI.BL.BusinessLayer.OPGProcessing;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using System.Web.Http.Description;
using ProdigyAPI.BL.ViewModel.Print;

namespace ProdigyAPI.Controllers.OPGProcessing
{
    [RoutePrefix("api/Transfers/OPG-separation")]
    public class OPGSeparationController : SIBaseApiController<OPGSeparationBL>
    {
        [HttpGet]
        [Route("get-item-gs")]
        [Route("get-item-gs/{companyCode}/{branchCode}")]
        [ResponseType(typeof(List<ListOfValue>))]
        public IHttpActionResult GetItemGS(string companyCode, string branchCode)
        {
            var gsList = new OPGSeparationBL().GetMetalGS(companyCode, branchCode);
            if (gsList != null)
                return Ok(gsList);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to load issue-to list" });
            }
        }

        [HttpPost]
        [Route("get-opgseparation-detail")]
        [ResponseType(typeof(OPGSeparationOutputVM))]
        public IHttpActionResult GetOPGDetails(OPGSeparationInputVM opgIssueQuery)
        {
            ErrorVM error = null;
            var result = new OPGSeparationBL().GetOPGDetails(opgIssueQuery, out error);
            if (error == null)
                return Ok(result);
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }
        
        [HttpPost]
        [Route("post")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult PostIssue(OPGSeparationInputVM opgIssueInput)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            int segregationNo = 0;
            bool success = new OPGSeparationBL().SaveOGSeparation(opgIssueInput, userID, out segregationNo, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = segregationNo.ToString(),
                    Message = "OPG Separation No# " + segregationNo.ToString() + " created successfully."
                };
                return Ok(docCreation);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// Get Print by Issue Number and Date.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="issueNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Print")]
        [Route("Print/{companyCode}/{branchCode}/{issueNo}")]
        public IHttpActionResult Print(string companyCode, string branchCode, int issueNo)
        {
            // This code is written by Eshwar on 7th June 2021
            ErrorVM error = null;
            ProdigyPrintVM result = new OPGSeparationBL().Print(companyCode, branchCode, issueNo, out error);
            if (error == null)
                return Ok(result);
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

    }
}
