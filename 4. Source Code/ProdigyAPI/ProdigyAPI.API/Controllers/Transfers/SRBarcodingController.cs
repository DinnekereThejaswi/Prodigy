using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProdigyAPI.BL.ViewModel.SRBarcode;
using ProdigyAPI.BL.BusinessLayer.SRBarcode;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Handlers;
using System.Web.Http.Description;
using ProdigyAPI.BL.ViewModel.Print;

namespace ProdigyAPI.Controllers.Transfers
{
    [RoutePrefix("api/Transfers/SR-barcoding")]
    public class SRBarcodingController : SIBaseApiController<SRBarcodingBL>
    {
        [HttpGet]
        [Route("get-sritems-to-barcode")]
        [Route("get-sritems-to-barcode/{companyCode}/{branchCode}")]
        [ResponseType(typeof(List<SRItemToBeBarcodedVM>))]
        public IHttpActionResult GetSRItemsTobeBarcoded(string companyCode, string branchCode)
        {
            List<SRItemToBeBarcodedVM> SRItemToBeBarcoded = new List<SRItemToBeBarcodedVM>();
            string errorMessage = string.Empty;
            var result = new SRBarcodingBL().GetSRItemsToBeBarcoded(companyCode, branchCode, out SRItemToBeBarcoded, out errorMessage);
            if (result == true)
                return Ok(SRItemToBeBarcoded);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = errorMessage });
            }
        }

        [HttpPost]
        [Route("barcode-detail")]
        [Route("barcode-detail/{companyCode}/{branchCode}")]
        [ResponseType(typeof(BarcodeMasterVM))]
        public IHttpActionResult GetBarcodeDetail(string companyCode, string branchCode, [FromBody] SRItemToBeBarcodedVM srItemsToBeBarcoded)
        {
            BarcodeMasterVM barcodeVM = null;
            string errorMessage = string.Empty;
            var result = new SRBarcodingBL().GetBarcodeDetailForReBarcoding(companyCode, branchCode, srItemsToBeBarcoded,
                out barcodeVM, out errorMessage);
            if (result == true)
                return Ok(barcodeVM);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = errorMessage });
            }
        }
        

        [HttpPost]
        [Route("post")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult PostSRBarcode(string companyCode, string branchCode, int billNo, string barcodeNo, [FromBody] BarcodeMasterVM barcodeInfo)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            string errorMessage = string.Empty;
            string newBarcodeNo = string.Empty;
            bool success = new SRBarcodingBL().PostBarcode(companyCode, branchCode, billNo, barcodeNo, barcodeInfo, userID, out newBarcodeNo, out errorMessage);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = newBarcodeNo,
                    Message = "New Barcode No# " + newBarcodeNo + " generated successfully."
                };
                return Ok(docCreation);
            }
            else {
                error = new ErrorVM { ErrorStatusCode = HttpStatusCode.BadRequest, description = errorMessage };
                return Content(HttpStatusCode.BadRequest, error);
            }
        }
    }
}
