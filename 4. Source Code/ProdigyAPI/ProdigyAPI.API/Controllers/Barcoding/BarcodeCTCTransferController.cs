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
    [RoutePrefix("api/Barcoding/ctc-transfer")]
    public class BarcodeCTCTransferController : SIBaseApiController<CounterToCounterTransferBL>
    {
        [HttpGet]
        [Route("get-barcode-detail")]
        [Route("get-barcode-detail/{companyCode}/{branchCode}/{transferToCounterCode}/{barcodeNo}")]
        [ResponseType(typeof(TransferBarcodeLine))]
        public IHttpActionResult GetBarcodeDetailForCTCTransfer(string companyCode, string branchCode, string transferToCounterCode, string barcodeNo)
        {
            TransferBarcodeLine ctcBarcodeLine = null;
            string errorMessage = string.Empty;
            var result = new CounterToCounterTransferBL().GetBarcodeDetailForCTCTransfer(companyCode, branchCode, transferToCounterCode, barcodeNo, out ctcBarcodeLine, out errorMessage);
            if (result == true)
                return Ok(ctcBarcodeLine);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = errorMessage });
            }
        }

        [HttpPost]
        [Route("post")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult PostCTCTransfer(CounterToCounterTransferVM counterToCounterTransferVM)
        {
            string userID = base.GetUserId();
            int documentNo = 0;
            string errorMessage = string.Empty;
            bool success = new CounterToCounterTransferBL().PostCTCTransfer(counterToCounterTransferVM, userID, out documentNo, out errorMessage);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = documentNo.ToString(),
                    Message = "Counter-To-Counter Transfer No# " + documentNo.ToString() + " created successfully."
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