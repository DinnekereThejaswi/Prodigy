﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProdigyAPI.BL.ViewModel.Issues;
using ProdigyAPI.Handlers;
using ProdigyAPI.BL.BusinessLayer.Issues;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using System.Web.Http.Description;
using ProdigyAPI.BL.ViewModel.Print;

namespace ProdigyAPI.Controllers.Transfers
{
    [RoutePrefix("api/Transfers/Barcode-issue")]
    public class BarcodeIssueController : SIBaseApiController<BarcodedIssueBL>
    {
        [HttpGet]
        [Route("get-issue-to")]
        [Route("get-issue-to/{companyCode}/{branchCode}")]
        public IHttpActionResult GetIssueToList(string companyCode, string branchCode)
        {
            var partyList = new BarcodedIssueBL().GetIssueToList(companyCode, branchCode);
            if (partyList != null)
                return Ok(partyList);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to load issue-to list" });
            }
        }

        [HttpGet]
        [Route("get-barcode-detail")]
        [Route("get-barcode-detail/{companyCode}/{branchCode}/{issueTo}/{gsCode}/{barcodeNo}")]
        public IHttpActionResult GetBarcodeDetail(string companyCode, string branchCode, string issueTo, string gsCode, string barcodeNo)
        {
            BarcodeIssueLineVM barcodeIssueLine;
            ErrorVM error = null;
            var result = new BarcodedIssueBL().GetBarcodeInfoforIssue(companyCode, branchCode, issueTo, gsCode, barcodeNo,
                out barcodeIssueLine, out error);
            if (result == true)
                return Ok(barcodeIssueLine);
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        [HttpPost]
        [Route("post")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult PostIssue(BarcodeIssueVM barcodeissue)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            int issueNo = 0;
            bool success = new BarcodedIssueBL().PostIssue(barcodeissue, userID, out issueNo, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = issueNo.ToString(),
                    Message = "Issue No# " + issueNo.ToString() + " created successfully."
                };
                return Ok(docCreation);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        [HttpPost]
        [Route("generate-xml")]
        [Route("generate-xml/{companyCode}/{branchCode}/{issueNo}")]
        public IHttpActionResult UploadOPGXml(string companyCode, string branchCode, int issueNo)
        {
            string errorMessage = string.Empty;
            var result = new BarcodedIssueBL().GenerateXMLFile(companyCode, branchCode, issueNo, out errorMessage);
            if (result == true)
                return Ok();
            else {
                ErrorVM error = new ErrorVM
                {
                    description = "Failed to generate & upload XML. Error " + errorMessage,
                    ErrorStatusCode = HttpStatusCode.BadRequest
                };
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        [HttpPost]
        [Route("cancel-issue")]
        [Route("cancel-issue/{companyCode}/{branchCode}/{issueNo}/{cancelRemarks}")]
        public IHttpActionResult CancelIssue(string companyCode, string branchCode, int issueNo, string cancelRemarks)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            var result = new BarcodedIssueBL().CancelIssue(companyCode, branchCode, issueNo, userID, cancelRemarks, out error);
            if (result == true)
                return Ok();
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Get List of All Tagged and Non-Tagged Issue Numbers.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("List/{companyCode}/{branchCode}/{date}")]
        public IHttpActionResult List(string companyCode, string branchCode, DateTime date)
        {
            ErrorVM error = new ErrorVM();
            var data = new BarcodedIssueBL().List(companyCode, branchCode, date, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }

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
            ProdigyPrintVM result = new BarcodedIssueBL().Print(companyCode, branchCode, issueNo, out error);
            if (error == null)
                return Ok(result);
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

    }
}