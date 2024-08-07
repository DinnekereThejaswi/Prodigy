using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProdigyAPI.BL.ViewModel.Barcoding;
using ProdigyAPI.Handlers;
using ProdigyAPI.BL.BusinessLayer.Barcoding;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using System.Web.Http.Description;
using ProdigyAPI.BL.ViewModel.Print;

namespace ProdigyAPI.Controllers.Barcoding
{

    /// <summary>
    /// API for barcode print
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Barcoding/barcode-print")]
    public class BarcodePrintController : SIBaseApiController<BarcodePrintBL>
    {
        /// <summary>
        /// Use this API to print barcode. PrintType should either be LEFT, RIGHT or DOUBLE.
        /// </summary>
        /// <param name="barcodePrintVM"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("print")]
        [ResponseType(typeof(ProdigyPrintVM))]
        public IHttpActionResult Print(BarcodePrintVM barcodePrintVM)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            string CompanyCode = string.Empty;
            string cc = base.CompanyCode;
            string bc = base.BranchCode;
            ProdigyPrintVM printObject = null;
            ErrorVM error = null;
            bool success = new BarcodePrintBL().GetPrint(barcodePrintVM, out printObject, out error);
            if (success) {
                return Ok(printObject);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }
    }
}
