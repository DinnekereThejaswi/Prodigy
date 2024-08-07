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
    [RoutePrefix("api/Barcoding/item-to-item-transfer")]
    public class BarcodeItemToItemTransferController : SIBaseApiController<ItemToItemTransferBL>
    {
        [HttpGet]
        [Route("get-barcode-detail")]
        [Route("get-barcode-detail/{companyCode}/{branchCode}/{transferToGsCode}/{transferToItemCode}/{barcodeNo}")]
        [ResponseType(typeof(TransferBarcodeLine))]
        public IHttpActionResult GetBarcodeDetail(string companyCode, string branchCode, string transferToGsCode, string transferToItemCode, string barcodeNo)
        {
            TransferBarcodeLine ctcBarcodeLine = null;
            string errorMessage = string.Empty;
            var result = new ItemToItemTransferBL().GetBarcodeDetailForItemToItemTransfer(companyCode, branchCode, transferToGsCode, transferToItemCode, barcodeNo, out ctcBarcodeLine, out errorMessage);
            if (result == true)
                return Ok(ctcBarcodeLine);
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = errorMessage });
            }
        }

        [HttpPost]
        [Route("post")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult PostItemTransfer(ItemToItemTransferVM itemToItemTransferVM)
        {
            string userID = base.GetUserId();
            int documentNo = 0;
            string errorMessage = string.Empty;
            bool success = new ItemToItemTransferBL().PostTransfer(itemToItemTransferVM, userID, out documentNo, out errorMessage);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = documentNo.ToString(),
                    Message = "Item to Item Transfer No# " + documentNo.ToString() + " created successfully."
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