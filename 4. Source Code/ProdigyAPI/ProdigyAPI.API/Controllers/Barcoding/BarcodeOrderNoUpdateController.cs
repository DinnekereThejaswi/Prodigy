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

namespace ProdigyAPI.Controllers.Barcoding
{
    [RoutePrefix("api/Barcoding/order-number-update")]
    public class BarcodeOrderNoUpdateController : SIBaseApiController<CounterToCounterTransferBL>
    {
        [HttpGet]
        [Route("get-barcode-detail")]
        [Route("get-barcode-detail/{companyCode}/{branchCode}/{barcodeNo}")]
        [ResponseType(typeof(TransferBarcodeLine))]
        public IHttpActionResult GetBarcodeInfo(string companyCode, string branchCode, string barcodeNo)
        {
            TransferBarcodeLine ctcBarcodeLine = null;
            string errorMessage = string.Empty;
            var result = new BarcodeOrderNoUpdateBL().GetBarcodeDetail(companyCode, branchCode, barcodeNo, out ctcBarcodeLine, out errorMessage);
            if (result == true)
                return Ok(ctcBarcodeLine);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = errorMessage });
            }
        }

        [HttpPut]
        [Route("put/{companyCode}/{branchCode}")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult PostBarcodeUpdate(string companyCode, string branchCode, [FromBody] TransferBarcodeLine barcodeItem)
        {
            string userID = base.GetUserId();
            string errorMessage = string.Empty;
            bool success = new BarcodeOrderNoUpdateBL().SaveBarcode(companyCode, branchCode, barcodeItem, userID, out errorMessage);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = barcodeItem.BarcodeNo,
                    Message = "Barcode No# " + barcodeItem.BarcodeNo + " updated successfully."
                };
                return Ok(docCreation);
            }
            else {
                ErrorVM error = new ErrorVM { ErrorStatusCode = HttpStatusCode.BadRequest, description = errorMessage };
                return Content(HttpStatusCode.BadRequest, error);
            }
        }
    }
}