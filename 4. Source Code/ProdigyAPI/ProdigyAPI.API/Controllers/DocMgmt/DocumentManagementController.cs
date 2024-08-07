using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProdigyAPI.BL.BusinessLayer.DocumentMgmt;

namespace ProdigyAPI.Controllers.DocuMgmt
{
    [Authorize]
    [RoutePrefix("api/docmgmt")]
    public class DocumentManagementController : ApiController
    {
        [HttpGet]
        [Route("last-series")]
        public IHttpActionResult GetLastDocNo(string companyCode, string branchCode, string docType)
        {
            DocMgmt docMgmt = new DocMgmt();
            Tuple<string, string> docSeries = new DocMgmt().GetLastDocumentNo(companyCode, branchCode, docType.ToUpper());
            if (docSeries == null)
                return NotFound();
            else
                return Ok(new { DocumentType = docSeries.Item1, DocumentNo = docSeries.Item2 });
        }
    }
}
