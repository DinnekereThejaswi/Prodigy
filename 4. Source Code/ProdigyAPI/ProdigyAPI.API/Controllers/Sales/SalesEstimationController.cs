using ProdigyAPI.BL.BusinessLayer.Sales;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.BL.ViewModel.Payment;
using ProdigyAPI.BL.ViewModel.Print;
using ProdigyAPI.BL.ViewModel.Sales;
using ProdigyAPI.Controllers.Masters;
using ProdigyAPI.Framework;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using ProdigyAPI.Providers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData.Query;
using System.Web.Http.Results;

namespace ProdigyAPI.Controllers.Sales
{
    /// <summary>
    /// Sales Estimation controller provieds API's for Sales Estimation, Attachment etc.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/sales/estimation")]
    public class SalesEstimationController : SIBaseApiController<SalesEstMasterVM>
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        private const string TABLE_NAME = "KTTU_SALES_EST_MASTER";
        #endregion

        #region Other Controller Methods
        
        /// <summary>
        /// Get sales Estimation Details by Estimation No.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Get/{companyCode}/{branchCode}/{estNo}")]
        [Route("Get")]
        public IHttpActionResult getEstimationDetails(string companyCode, string branchCode, int estNo)
        {
            ErrorVM error = new ErrorVM();
            SalesEstMasterVM result = new SalesEstimationBL().GetEstimationDetails(companyCode, branchCode, estNo, out error);
            if (error == null) {
                return Ok(result);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get sales Estimation Details by Estimation No.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetForPrint/{companyCode}/{branchCode}/{estNo}")]
        [Route("GetForPrint")]
        public IHttpActionResult GetEstimationDetailsForPrint(string companyCode, string branchCode, int estNo)
        {
            ErrorVM error = new ErrorVM();
            SalesEstMasterVM details = new SalesEstimationBL().GetEstimationDetailsForPrint(companyCode, branchCode, estNo, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(details);
        }

        /// <summary>
        /// Get sales Estimation Details by Estimation No.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetForPrintTotal/{companyCode}/{branchCode}/{estNo}")]
        [Route("GetForPrintTotal")]
        public IHttpActionResult GetEstimationDetailsForPrintTotal(string companyCode, string branchCode, int estNo)
        {
            ErrorVM error = new ErrorVM();
            SalesEstDetailsVM details = new SalesEstimationBL().GetEstimationDetailsForPrintTotal(companyCode, branchCode, estNo, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(details);
        }

        /// <summary>
        /// Save Sales Estimation details.
        /// </summary>
        /// <param name="saleEst"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult SaveEstimation([FromBody] SalesEstMasterVM saleEst)
        {
            //if (ModelState.IsValid) {
            //    return null;
            //}

            string user = base.GetUserId();
            if (saleEst == null) {
                return Content(HttpStatusCode.BadRequest, "Valid data is required.");
            }
            else {
                saleEst.OperatorCode = user;
            }
            ErrorVM error = new ErrorVM();
            int estNo = new SalesEstimationBL().SaveEstimation(saleEst, out error);
            if (error == null) {
                return Ok(new { EstMationNo = estNo });
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Update Sales Estimation details.
        /// </summary>
        /// <param name="estNo"></param>
        /// <param name="saleEst"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put")]
        public IHttpActionResult UpdateEstimation(int estNo, [FromBody] SalesEstMasterVM saleEst)
        {
            string user = base.GetUserId();
            if (saleEst == null) {
                return Content(HttpStatusCode.BadRequest, "Valid data is required.");
            }
            else {
                saleEst.OperatorCode = user;
            }
            ErrorVM error = new ErrorVM();
            int retEstNo = new SalesEstimationBL().UpdateEstimation(estNo, saleEst, out error);
            if (error == null) {
                return Ok(new { EstMationNo = retEstNo });
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get Stone Types for Tagged/Non Tageed Items Screen.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetStoneType/{companyCode}/{branchCode}")]
        [Route("GetStoneType")]
        public IHttpActionResult GetStoneType(string companyCode, string branchCode)
        {
            return Ok(db.usp_LoadGS(companyCode, branchCode, "SC"));
        }

        /// <summary>
        /// Get Stone/Diamond Details by StoneType.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="StoneType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetStoneDiamondName/{companyCode}/{branchCode}")]
        [Route("GetStoneDiamondName")]
        public IHttpActionResult GetStoneOrDiamondDetails(string companyCode, string branchCode, string StoneType)
        {
            List<KSTU_STONE_DIAMOND_MASTER> lstOfStoneDiamond = db.KSTU_STONE_DIAMOND_MASTER.Where(ksdm => ksdm.type == StoneType
            && ksdm.obj_status == "O" && ksdm.company_code == companyCode
            && ksdm.branch_code == branchCode).OrderBy(ksdm => ksdm.stone_name).ToList();
            return Ok(lstOfStoneDiamond);
        }

        /// <summary>
        /// Get Order Information (barcode) for the sales estimation
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetOrderForSales/{companyCode}/{branchCode}/{orderNo}")]
        [Route("GetOrderForSales")]
        public IHttpActionResult GetOrderForSales(string companyCode, string branchCode, int orderNo)
        {
            SalesEstMasterVM salesEst = new SalesEstMasterVM();
            try {
                string barcodeNo = string.Empty;
                KTTU_ORDER_MASTER kom = new KTTU_ORDER_MASTER();
                List<KTTU_ORDER_DETAILS> kod = new List<KTTU_ORDER_DETAILS>();
                List<SalesEstDetailsVM> lstOfSalesEstDet = new List<SalesEstDetailsVM>();

                kom = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == orderNo
                                                       && ord.company_code == companyCode
                                                       && ord.branch_code == branchCode).FirstOrDefault();

                KTTU_SALES_EST_MASTER est = db.KTTU_SALES_EST_MASTER.Where(s => s.company_code == companyCode
                                                                            && s.branch_code == branchCode
                                                                            && s.order_no == orderNo).FirstOrDefault();
                if (est != null && est.order_no != orderNo) {
                    return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "Order No " + orderNo + " Already Adjusted towords Est.No " + est.est_no + "" });
                }
                if (kom == null) {
                    return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "Invalid Order Number." });
                }
                else if (kom.cflag == "Y") {
                    return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = string.Format("Order No: {0} already Cancelled.", orderNo) });
                }
                else if (kom.closed_flag == "Y") {
                    return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = string.Format("Order No: {0} already Closed.", orderNo) });
                }
                else if (kom.bill_no != 0) {
                    return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = string.Format("Order No {0} Already Adjusted towords BillNo {1} Updated Date {2}", orderNo, kom.bill_no, Convert.ToDateTime(kom.UpdateOn).ToString("dd/MM/yyyy")) });
                }
                else if(kom.is_lock == "Y") {
                    return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = string.Format("Order No: {0} is dormant.", orderNo) });
                }

                salesEst.CustID = kom.Cust_Id;
                salesEst.ObjID = kom.obj_id;
                salesEst.CompanyCode = kom.company_code;
                salesEst.BranchCode = kom.branch_code;
                salesEst.OrderNo = kom.order_no;
                salesEst.OrderAmount = Convert.ToDecimal(kom.advance_ord_amount);
                salesEst.CustID = kom.Cust_Id;
                salesEst.CustName = kom.cust_name;
                salesEst.Remarks = kom.remarks;
                salesEst.OrderDate = kom.order_date;
                salesEst.OperatorCode = kom.operator_code;
                salesEst.GSType = kom.gs_code;
                salesEst.Rate = kom.rate;
                salesEst.GrandTotal = kom.grand_total;
                salesEst.BillNo = kom.bill_no;
                salesEst.UpdateOn = kom.UpdateOn;
                salesEst.Karat = kom.karat;
                salesEst.NewBillNo = kom.New_Bill_No;
                salesEst.TotalTaxAmount = Convert.ToDecimal(kom.total_tax_amount);
                salesEst.IsPAN = kom.isPAN;
                salesEst.Address1 = kom.address1;
                salesEst.Address2 = kom.address2;
                salesEst.Address3 = kom.address3;
                salesEst.City = kom.city;
                salesEst.Pincode = kom.pin_code;
                salesEst.MobileNo = kom.mobile_no;
                salesEst.State = kom.state;
                salesEst.StateCode = kom.state_code;
                salesEst.TIN = kom.tin;
                salesEst.PANNo = kom.pan_no;
                salesEst.PhoneNo = kom.phone_no;
                salesEst.EmailID = kom.Email_ID;
                salesEst.Salutation = kom.salutation;
                salesEst.Pos = kom.state_code;
                // Checking is it reserverd order or Not if its reserve order sending barcode information
                if (kom != null && kom.order_type == "R") {
                    kod = db.KTTU_ORDER_DETAILS.Where(ord => ord.order_no == orderNo
                                                           && ord.company_code == companyCode
                                                           && ord.branch_code == branchCode).ToList();
                    if (kod != null) {
                        foreach (KTTU_ORDER_DETAILS orderDet in kod) {
                            barcodeNo = orderDet.item_name;
                            IHttpActionResult actionResult = new BarcodeController().GetBarcodeWithStoneForSales(companyCode, branchCode, barcodeNo, orderNo.ToString());
                            var contentResult = actionResult as OkNegotiatedContentResult<SalesEstDetailsVM>;
                            SalesEstDetailsVM barcode = (SalesEstDetailsVM)contentResult.Content;
                            barcode.OrderNo = orderNo;
                            lstOfSalesEstDet.Add(barcode);
                        }
                    }
                    salesEst.salesEstimatonVM = lstOfSalesEstDet;
                }
                return Ok(salesEst);
            }
            catch (Exception excp) {
                throw excp;
            }

        }


        /// <summary>
        /// Get Order Information (barcode) for the sales estimation for suresh request
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetOrderForAddBarcode/{companyCode}/{branchCode}/{orderNo}")]
        [Route("GetOrderForAddBarcode")]
        public IHttpActionResult GetOrderForAddBarcode(string companyCode, string branchCode, int orderNo)
        {
            SalesEstMasterVM salesEst = new SalesEstMasterVM();
            try {
                string barcodeNo = string.Empty;
                KTTU_ORDER_MASTER kom = new KTTU_ORDER_MASTER();
                List<KTTU_ORDER_DETAILS> kod = new List<KTTU_ORDER_DETAILS>();
                List<SalesEstDetailsVM> lstOfSalesEstDet = new List<SalesEstDetailsVM>();

                kom = db.KTTU_ORDER_MASTER.Where(ord => ord.order_no == orderNo
                                                       && ord.company_code == companyCode
                                                       && ord.branch_code == branchCode).FirstOrDefault();
                
                if (kom == null) {
                    return null;
                }
                else if (kom.cflag == "Y") {
                    return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = string.Format("Order No: {0} already Cancelled.", orderNo) });
                }
                else if (kom.closed_flag == "Y") {
                    return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = string.Format("Order No: {0} already Closed.", orderNo) });
                }
                else if (kom.bill_no != 0) {
                    return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = string.Format("Order No {0} Already Adjusted towords BillNo {1} Updated Date {2}", orderNo, kom.bill_no, Convert.ToDateTime(kom.UpdateOn).ToString("dd/MM/yyyy")) });
                }
                else if (kom.is_lock == "Y") {
                    return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = string.Format("Order No: {0} is dormant.", orderNo) });
                }

                KTTU_SALES_EST_MASTER est = db.KTTU_SALES_EST_MASTER.Where(s => s.company_code == companyCode && s.branch_code == branchCode && s.order_no == orderNo).FirstOrDefault();
                if (est != null) {
                    return null;
                }

                salesEst.CustID = kom.Cust_Id;
                salesEst.ObjID = kom.obj_id;
                salesEst.CompanyCode = kom.company_code;
                salesEst.BranchCode = kom.branch_code;
                salesEst.OrderNo = kom.order_no;
                salesEst.OrderAmount = Convert.ToDecimal(kom.advance_ord_amount);
                salesEst.CustID = kom.Cust_Id;
                salesEst.CustName = kom.cust_name;
                salesEst.Remarks = kom.remarks;
                salesEst.OrderDate = kom.order_date;
                salesEst.OperatorCode = kom.operator_code;
                salesEst.GSType = kom.gs_code;
                salesEst.Rate = kom.rate;
                salesEst.GrandTotal = kom.grand_total;
                salesEst.BillNo = kom.bill_no;
                salesEst.UpdateOn = kom.UpdateOn;
                salesEst.Karat = kom.karat;
                salesEst.NewBillNo = kom.New_Bill_No;
                salesEst.TotalTaxAmount = Convert.ToDecimal(kom.total_tax_amount);
                salesEst.IsPAN = kom.isPAN;
                salesEst.Address1 = kom.address1;
                salesEst.Address2 = kom.address2;
                salesEst.Address3 = kom.address3;
                salesEst.City = kom.city;
                salesEst.Pincode = kom.pin_code;
                salesEst.MobileNo = kom.mobile_no;
                salesEst.State = kom.state;
                salesEst.StateCode = kom.state_code;
                salesEst.TIN = kom.tin;
                salesEst.PANNo = kom.pan_no;
                salesEst.PhoneNo = kom.phone_no;
                salesEst.EmailID = kom.Email_ID;
                salesEst.Salutation = kom.salutation;
                // Checking is it reserverd order or Not if its reserve order sending barcode information
                if (kom != null && kom.order_type == "R") {
                    kod = db.KTTU_ORDER_DETAILS.Where(ord => ord.order_no == orderNo
                                                           && ord.company_code == companyCode
                                                           && ord.branch_code == branchCode).ToList();
                    if (kod != null) {
                        foreach (KTTU_ORDER_DETAILS orderDet in kod) {
                            barcodeNo = orderDet.item_name;
                            IHttpActionResult actionResult = new BarcodeController().GetBarcodeWithStoneForSales(companyCode, branchCode, barcodeNo, orderNo.ToString());
                            var contentResult = actionResult as OkNegotiatedContentResult<SalesEstDetailsVM>;
                            SalesEstDetailsVM barcode = (SalesEstDetailsVM)contentResult.Content;
                            barcode.OrderNo = orderNo;
                            lstOfSalesEstDet.Add(barcode);
                        }
                    }
                    salesEst.salesEstimatonVM = lstOfSalesEstDet;
                }
                return Ok(salesEst);
            }
            catch (Exception excp) {
                throw excp;
            }

        }

        /// <summary>
        /// get Estimation details with discount.
        /// </summary>
        /// <param name="saleEst"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetOfferDiscount")]
        public IHttpActionResult CalculateOfferDiscount(SalesEstMasterVM saleEst)
        {
            SalesEstMasterVM discount = new SalesEstimationBL().GetOfferDiscountCalculationModel(saleEst);
            return Ok(discount);
        }

        /// <summary>
        /// Individual barcode Calculation
        /// </summary>
        /// <param name="barcodeDet"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("BarcodeCalculation")]
        public IHttpActionResult BarcodeCalculaton(SalesEstDetailsVM barcodeDet)
        {
            ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            string operatorCode = principal.Claims.Where(c => c.Type == "UserID").FirstOrDefault().Value;

            ErrorVM error = new ErrorVM();
            SalesEstDetailsVM addWt = new SalesEstimationBL().BarcodeCalculation(barcodeDet, operatorCode, out error);
            if (error == null) {
                return Ok(addWt);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// To validate Mc% is valid or not for the given item.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsCode"></param>
        /// <param name="item"></param>
        /// <param name="mcPercent"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ValidateMC/{companyCode}/{branchCode}/{gsCode}/{item}/{mcPercent}")]
        [Route("ValidateMC")]
        public IHttpActionResult ValidateMinMC(string companyCode, string branchCode, string gsCode, string item, decimal mcPercent)
        {
            ErrorVM error = new ErrorVM();
            bool isValid = new SalesEstimationBL().GetValidateMinMc(companyCode, branchCode, gsCode, item, mcPercent, out error);
            if (error != null) {
                return Ok(error);
            }
            return Ok(isValid);
        }

        /// <summary>
        /// This returns Total Stock Weight for the selected GS Code and Counter
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsCode"></param>
        /// <param name="counterCode"></param>
        /// <param name="itemName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAddWightItemGrossWeight/{companyCode}/{branchCode}/{gsCode}/{counterCode}/{itemName}")]
        [Route("GetAddWightItemGrossWeight")]
        public IHttpActionResult GetAddWightItemGrossWeight(string companyCode, string branchCode, string gsCode, string counterCode, string itemName)
        {
            decimal closingWeight = new SalesEstimationBL().GetAddWightItemGrossWeight(companyCode, branchCode, gsCode, counterCode, itemName);
            return Ok(new { wight = closingWeight });
        }

        /// <summary>
        /// Get Multiple Barcode Details.
        /// </summary>
        /// <param name="salesMaster"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ExchangeTax")]
        public IHttpActionResult GetMultiBarcodeDetails(SalesEstMasterVM salesMaster)
        {
            KSTU_COMPANY_MASTER branch = db.KSTU_COMPANY_MASTER.Where(company => company.company_code == salesMaster.CompanyCode && company.branch_code == salesMaster.BranchCode).FirstOrDefault();
            int branchStateCode = Convert.ToInt32(branch.state_code);
            int isInterstate = branchStateCode == salesMaster.Pos ? 0 : 1;

            List<SalesEstDetailsVM> barcodeDetails = salesMaster.salesEstimatonVM;
            salesMaster.salesEstimatonVM = null;
            salesMaster.salesEstimatonVM = new List<SalesEstDetailsVM>();
            foreach (SalesEstDetailsVM sales in barcodeDetails) {
                sales.isInterstate = isInterstate;
                IHttpActionResult actionResult = BarcodeCalculaton(sales);
                var contentResult = actionResult as OkNegotiatedContentResult<SalesEstDetailsVM>;
                SalesEstDetailsVM salesVM = contentResult.Content;
                salesMaster.salesEstimatonVM.Add(salesVM);
            }
            return Ok(salesMaster);
        }

        /// <summary>
        /// Estimation DotMatrix Printing.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("DotMatrixPrint/{companyCode}/{branchCode}/{estNo}")]
        public IHttpActionResult EstimationDotMatrixPrint(string companyCode, string branchCode, int estNo)
        {
            ErrorVM error = new ErrorVM();
            ProdigyPrintVM print = new SalesEstimationBL().GetSalesEstimatePrint(companyCode, branchCode, estNo, out error);
            if (error == null) {
                return Ok(print);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Estimation DotMatrix Printing.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ThermalPrint60Column/{companyCode}/{branchCode}/{estNo}")]
        public IHttpActionResult Estimation60ColumnThermalPrint(string companyCode, string branchCode, int estNo)
        {
            ErrorVM error = new ErrorVM();
            string print = new SalesEstimationBL().Estimation60ColumnThermalPrint(companyCode, branchCode, estNo, out error);
            if (error == null) {
                return Ok(System.Text.Encoding.UTF8.GetBytes(print));
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get All the Estimation Details.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("AllEstimations/{companyCode}/{branchCode}")]
        public IHttpActionResult GetAllEstimations(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            List<SalesEstMasterVM> result = new SalesEstimationBL().GetAllSalesEstimations(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(result);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Mege Estimations
        /// </summary>
        /// <param name="mergEstimate"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("MergeEstimation")]
        public IHttpActionResult MergeEstimate(MergeEstMaster mergEstimate)
        {
            ErrorVM error = new ErrorVM();
            int estNo = new SalesEstimationBL().MergeEstimation(mergEstimate, out error);
            if (error == null) {
                return Ok(new { NewEstimationNo = estNo });
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }


        /// <summary>
        /// Offer Discount Calcuation with Barcode Details.
        /// </summary>
        /// <param name="saleEst"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("OfferDiscountCalculationWithBarcode")]
        public IHttpActionResult OfferDiscountCalculationWithBarcode([FromBody] SalesEstMasterVM saleEst)
        {
            ErrorVM error = new ErrorVM();
            SalesEstMasterVM estDet = new SalesEstimationBL().GetOfferDiscountCalculationWithBarcode(saleEst, out error);
            if (error == null) {
                return Ok(estDet);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This is the Method used to Validate Different Metal Validation.
        /// </summary>
        /// <param name="saleEst"></param>
        /// <param name="barcodeNo"></param>
        /// <param name="gsCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("BarcodeValidation/{barcodeNo}/{gsCode}")]
        [Route("BarcodeValidation")]
        public IHttpActionResult GetBarcodeValidation([FromBody] SalesEstMasterVM saleEst, [FromUri] string barcodeNo, string gsCode)
        {
            ErrorVM error = new ErrorVM();
            bool isValid = new SalesEstimationBL().GetBarcodeValidation(saleEst, barcodeNo, gsCode, out error);
            if (isValid) {
                return Ok();
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }


        /// <summary>
        /// Calculate offer Discount.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("OfferDiscount/{companyCode}/{branchCode}/{estNo}")]
        [Route("OfferDiscount")]
        public IHttpActionResult GetOfferDiscountF12(string companyCode, string branchCode, int estNo)
        {
            ErrorVM error = new ErrorVM();
            bool discountApplied = new SalesEstimationBL().GetOfferDiscountF12(companyCode, branchCode, estNo, out error);
            if (discountApplied) {
                return Ok();
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Cancel offer Discount.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CancelOfferDiscount/{companyCode}/{branchCode}/{estNo}")]
        [Route("CancelOfferDiscount")]
        public IHttpActionResult GetCancelOfferDiscountF12(string companyCode, string branchCode, int estNo)
        {
            ErrorVM error = new ErrorVM();
            bool discountApplied = new SalesEstimationBL().GetCancelOfferDiscountF12(companyCode, branchCode, estNo, out error);
            if (discountApplied) {
                return Ok();
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Print Bill by Estimation No.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("PrintBillByEstimation/{companyCode}/{branchCode}/{estNo}")]
        public IHttpActionResult PrintBillByEstimation(string companyCode, string branchCode, int estNo)
        {
            ErrorVM error = new ErrorVM();
            string print = new SalesEstimationBL().PrintBillByEstimation(companyCode, branchCode, estNo, out error);
            if (error == null) {
                return Ok(System.Text.Encoding.UTF8.GetBytes(print));
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get All Estimation Details.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("AllEstimationDet/{companyCode}/{branchCode}")]
        public IQueryable GetAllEstimationDetails(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            IQueryable data = new SalesEstimationBL().GetAllEstimationDetails(companyCode, branchCode, out error);
            if (error == null) {
                return data;
            }
            return null;
        }

        /// <summary>
        /// Use this method to get the row version or Row Revision, which is the SQL Server timestamp data. 
        /// This row version can be used to check if the estimate is changed by another user in a OLTP system.
        /// This API provides minimum data without affecting the traffic.
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="estNo">The estimate number in question</param>
        /// <returns>Returns SalesEstimateRowVersion object on success. On error you will get Error object.</returns>
        [HttpGet]
        [Route("RowVersion/{companyCode}/{branchCode}/{estNo}")]
        [Route("RowVersion")]
        [ResponseType(typeof(SalesEstimateRowVersion))]
        public IHttpActionResult GetEstimateRowVersion(string companyCode, string branchCode, int estNo)
        {
            ErrorVM error = new ErrorVM();
            SalesEstimateRowVersion estimateRowVersion = null;
            var succeeded = new SalesEstimationBL().GetSalesEstimateRowVersion(companyCode, branchCode, estNo, out estimateRowVersion, out error);
            if (succeeded) {
                return Ok(estimateRowVersion);
            }
            else
                return Content(error.ErrorStatusCode, error);
        }

        /// <summary>
        /// 2022 Offer
        /// </summary>
        /// <param name="estimate"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("2022Offer")]
        [ResponseType(typeof(DiscountOutput))]
        public IHttpActionResult OfferDiscount2022(SalesEstModel estimate)
        {
            ErrorVM error = new ErrorVM();
            DiscountOutput discountOutput = null;
            var result = new SalesEstimationBL().GetOfferDiscount2022(estimate, out discountOutput, out error);
            if (result == true) {
                return Ok(discountOutput);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("NewOfferDiscount")]
        [ResponseType(typeof(DiscountOutput))]
        public IHttpActionResult GetNewOfferDiscount(SalesEstModel estimate)
        {
            ErrorVM error = new ErrorVM();
            DiscountOutput discountOutput = null;
            var result = new SalesEstimationBL().CheckAndGetOfferDiscount(estimate, out discountOutput, out error);
            if (result == true) {
                return Ok(discountOutput);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }
        #endregion

        #region Private Methods
        private void DeleteEstimation(string companyCode, string branchCode, int estNo)
        {
            KTTU_SALES_EST_MASTER estMaster = db.KTTU_SALES_EST_MASTER.Where(est => est.est_no == estNo && est.company_code == companyCode && est.branch_code == branchCode).FirstOrDefault();
            db.KTTU_SALES_EST_MASTER.Remove(estMaster);

            List<KTTU_SALES_EST_DETAILS> estDetails = db.KTTU_SALES_EST_DETAILS.Where(est => est.est_no == estNo && est.company_code == companyCode && est.branch_code == branchCode).ToList();
            foreach (KTTU_SALES_EST_DETAILS ksed in estDetails) {
                db.KTTU_SALES_EST_DETAILS.Remove(ksed);
            }
            List<KTTU_SALES_STONE_DETAILS> estStone = db.KTTU_SALES_STONE_DETAILS.Where(est => est.est_no == estNo && est.company_code == companyCode && est.branch_code == branchCode).ToList();
            foreach (KTTU_SALES_STONE_DETAILS kssd in estStone) {
                db.KTTU_SALES_STONE_DETAILS.Remove(kssd);
            }
        }

        private decimal CoinOfferCalculation(string companyCode, string branchCode, decimal totalAmount, decimal totalMetalWeight, decimal totalStoneWeight, string gsCode)
        {
            decimal offerWeight = 0;
            decimal offerAmount = 0;
            decimal totalCoinOffer = 0;
            DateTime appDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
            KSTS_DISCOUNT_PERIOD discountPeriod = db.KSTS_DISCOUNT_PERIOD.Where(d => d.company_code == companyCode
                                                                                    && d.branch_code == branchCode
                                                                                    && DbFunctions.TruncateTime(d.start_date) <= DbFunctions.TruncateTime(appDate)
                                                                                    && DbFunctions.TruncateTime(d.end_date) >= DbFunctions.TruncateTime(appDate)).FirstOrDefault();
            // Some offer is going on
            if (discountPeriod != null) {

                if (gsCode == "NGO") {
                    // Offer ID is hot coded here because in magna they done same thing. confirmed by Ram sir on 18/08/2020
                    List<KTTU_OFFER_DISCOUNT_DETAILS> offerDiscountOnGold = db.KTTU_OFFER_DISCOUNT_DETAILS.Where(d => d.company_code == companyCode
                                                                                                         && d.branch_code == branchCode
                                                                                                         && d.isActive == "Y"
                                                                                                         && d.Offer_ID == "8").ToList();
                    if (offerDiscountOnGold != null) {
                        if (offerDiscountOnGold[0].from_weight != 0 && offerDiscountOnGold[0].to_weight != 0) {
                            //Weight based Need to Implement this.
                        }
                        else if (offerDiscountOnGold[0].from_amount != 0 && offerDiscountOnGold[0].to_amount != 0) {
                            //Offer Based on Amount
                            offerWeight = offerDiscountOnGold[0].freeGoldvalue_per_grm;
                            offerAmount = offerDiscountOnGold[0].to_amount;
                            totalCoinOffer = Convert.ToInt32(Math.Floor(totalAmount / offerAmount));
                        }
                    }
                }
                else if (gsCode == "NGD") {
                    KTTU_OFFER_DISCOUNT_DETAILS offerDiscountOnDiamond = db.KTTU_OFFER_DISCOUNT_DETAILS.Where(d => d.company_code == companyCode
                                                                                                  && d.branch_code == branchCode
                                                                                                  && d.isActive == "Y"
                                                                                                  && d.Offer_ID == "7"
                                                                                                  && d.from_carrat <= totalStoneWeight
                                                                                                  && d.to_carrat >= totalStoneWeight).FirstOrDefault();
                    if (offerDiscountOnDiamond != null) {
                        offerWeight = offerDiscountOnDiamond.freeGoldvalue_per_grm;
                        offerAmount = offerDiscountOnDiamond.to_amount;
                        totalCoinOffer = Convert.ToInt32(Math.Floor(totalAmount / offerAmount));
                    }
                }
            }
            return totalCoinOffer;
        }
        #endregion

        #region Default Controller Methods
        public IQueryable<SalesEstMasterVM> List()
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Count(ODataQueryOptions<SalesEstMasterVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Post(SalesEstMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, SalesEstMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Delete([FromBody] int id)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
