using ProdigyAPI.BL.BusinessLayer;
using ProdigyAPI.BL.BusinessLayer.Purchase;
using ProdigyAPI.BL.BusinessLayer.Sales;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Payment;
using ProdigyAPI.BL.ViewModel.Sales;
using ProdigyAPI.Framework;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData.Query;

namespace ProdigyAPI.Controllers.Sales
{
    /// <summary>
    /// Sales Billing Controller provides API's for Sales Billing.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/SalesBilling")]
    public class SalesBillingController : SIBaseApiController<SalesMasterVM>, IBaseMasterActionController<SalesMasterVM, SalesMasterVM>
    {
        #region Controller Methods

        /// <summary>
        /// Get Sales Estimation information by Estimation Number.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Get/{companyCode}/{branchCode}/{estNo}")]
        public IHttpActionResult GetSalesBillingDetails(string companyCode, string branchCode, int estNo)
        {
            ErrorVM error = new ErrorVM();
            SalesMasterVM smv = new SalesBillingBL().GetSalesBillingDetails(companyCode, branchCode, estNo, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(smv);
        }

        /// <summary>
        /// Get Sales Details by Bill Number.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="billNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetByBillNo/{companyCode}/{branchCode}/{billNo}")]
        public IHttpActionResult GetSalesBillingDetailsByBillNo(string companyCode, string branchCode, int billNo)
        {
            ErrorVM error = new ErrorVM();
            SalesMasterVM smv = new SalesBillingBL().GetBillInfoByBillNo(companyCode, branchCode, billNo, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(smv);
        }

        /// <summary>
        /// Save Billing Information (Estimation to Billing).
        /// </summary>
        /// <param name="sales"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult SaveSalesBilling([FromBody] SalesBillingVM sales)
        {
            //if (!ModelState.IsValid) {
            //    return BadRequest(ModelState);
            //}
            ErrorVM error = new ErrorVM();
            List<OrderOTPAttributes> otpAttributes = new List<OrderOTPAttributes>();
            int orderNo = 0;
            int receiptNo = 0;
            if (sales != null)
                sales.OperatorCode = base.GetUserId(); ;
            int billNo = new SalesBillingBL().NewSaveSalesBilling(sales, out orderNo, out receiptNo, out error, out otpAttributes);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            if (orderNo == 0 && receiptNo == 0)
                return Ok(new { billNo = billNo });
            else
                return Ok(new { billNo = billNo, orderNo = orderNo, receiptNo = receiptNo });
        }

        /// <summary>
        /// This method is used to validate the Payment mode on sales Billing.
        /// </summary>
        /// <param name="sales"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ValidatePayMode")]
        public IHttpActionResult ValidateSalesBillingPayments([FromBody] SalesBillingVM sales)
        {
            ErrorVM error = new ErrorVM();
            bool isValidBill = new SalesBillingBL().ValidationSalesBillingPayment(sales, out error, null);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Content(HttpStatusCode.OK, "Valid Bill");
        }

        /// <summary>
        /// Calculates Offer discount.
        /// </summary>
        /// <param name="sales"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("DiscountCalculation")]
        public IHttpActionResult DiscountCalculation(SalesMasterVM sales)
        {
            ErrorVM error = new ErrorVM();
            SalesMasterVM salesDet = new SalesBillingBL().GetDiscountCalculationModel(sales, out error);
            return Ok();
        }

        /// <summary>
        /// Print Sales Bill.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="billNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Print/{companyCode}/{branchCode}/{billNo}")]
        [Route("Print")]
        public IHttpActionResult SalesBillPrint(string companyCode, string branchCode, int billNo)
        {
            ErrorVM error = new ErrorVM();
            BL.ViewModel.Print.ProdigyPrintVM print = new SalesBillingBL().PrintSalesBill(companyCode, branchCode, billNo, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(print);
        }

        /// <summary>
        /// Discount Calculation
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estimateNo"></param>
        /// <param name="receivedAmount"></param>
        /// <param name="paidAmount"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("DiscountCalc")]
        public IHttpActionResult CalculateDiscount(string companyCode, string branchCode, int estimateNo, decimal receivedAmount, decimal paidAmount)
        {
            MagnaDbEntities db = new MagnaDbEntities();
            var estimateMaster = db.KTTU_SALES_EST_MASTER.Where(e => e.company_code == companyCode
                   && e.branch_code == branchCode && e.est_no == estimateNo).FirstOrDefault();
            if (estimateMaster == null) {
                return Content(HttpStatusCode.NotFound, new ErrorVM
                {
                    ErrorStatusCode = HttpStatusCode.NotFound,
                    description = "Estimation does not exist or is invalid."
                });
            }
            decimal orderAmt = estimateMaster.order_amount;

            var q = (from sed in db.KTTU_SALES_EST_DETAILS
                     where sed.company_code == companyCode
                           && sed.branch_code == branchCode && sed.est_no == estimateNo
                     group sed by (sed.CGST_Percent + sed.SGST_Percent + sed.IGST_Percent + sed.CESSPercent) into g
                     select new
                     {
                         TaxPercent = g.Sum(x => x.SGST_Percent + x.CGST_Percent + x.IGST_Percent + x.CESSPercent)
                     }).ToList();
            if (q.Count > 1) {
                return Content(HttpStatusCode.NotFound, new ErrorVM
                {
                    ErrorStatusCode = HttpStatusCode.NotFound,
                    description = "Estimation is having items with multiple GST Rates and this is not supported in this version."
                });
            }
            var salesTotals = (from sed in db.KTTU_SALES_EST_DETAILS
                               where sed.company_code == companyCode
                                     && sed.branch_code == branchCode && sed.est_no == estimateNo
                               group sed by 1 into g
                               select new
                               {
                                   ItemTotal = g.Sum(x => x.item_total_after_discount),
                                   CGSTAmount = g.Sum(x => x.CGST_Amount),
                                   SGSTAmount = g.Sum(x => x.SGST_Amount),
                                   IGSTAmount = g.Sum(x => x.IGST_Amount),
                                   CessAmount = g.Sum(x => x.CESSAmount),
                                   GSTAmt = (g.Sum(x => x.CGST_Amount))
                                     + (g.Sum(x => x.SGST_Amount))
                                     + (g.Sum(x => x.IGST_Amount))
                                     + (g.Sum(x => x.CESSAmount)),
                                   ItemFinalAmt = g.Sum(x => x.item_final_amount),
                                   MCDiscountAmt = g.Sum(x => x.Mc_Discount_Amt),
                                   OrderDiscountAmt = g.Sum(x => x.Ord_Discount_Amt),
                                   OfferDiscountAmt = g.Sum(x => x.offer_value),
                                   AdditionalDiscountAmt = g.Sum(x => x.item_additional_discount),
                                   TotalDiscountAmt = (g.Sum(x => x.item_additional_discount))
                                     + (g.Sum(x => x.Mc_Discount_Amt))
                                     + (g.Sum(x => x.Ord_Discount_Amt))
                                     + (g.Sum(x => x.offer_value))
                               }).FirstOrDefault();

            var q2 = db.KTTU_SALES_EST_DETAILS.Where(sd => sd.company_code == companyCode && sd.branch_code == branchCode
                && sd.est_no == estimateNo).FirstOrDefault();
            decimal gstPercent = Convert.ToDecimal(q2.CGST_Percent) + Convert.ToDecimal(q2.SGST_Percent) + Convert.ToDecimal(q2.IGST_Percent);
            decimal gstCessPercent = Convert.ToDecimal(q2.CESSPercent);

            decimal totalGSTInclCessPercent = gstPercent + gstCessPercent;
            decimal inherentPurchaseAmt = Convert.ToDecimal(db.KTTU_PURCHASE_EST_DETAILS.Where(e => e.company_code == companyCode
                   && e.branch_code == branchCode && e.est_no == estimateNo).Sum(e => e.itemwise_purchase_amount));
            decimal inherentSRAmt = Convert.ToDecimal(db.KTTU_SR_DETAILS.Where(e => e.company_code == companyCode
                   && e.branch_code == branchCode && e.est_no == estimateNo).Sum(e => e.item_final_amount));

            decimal otherPaymentAmt = Convert.ToDecimal(db.KTTU_PAYMENT_DETAILS.Where(e => e.company_code == companyCode
                   && e.branch_code == branchCode && e.series_no == estimateNo && e.trans_type == "A").Sum(e => e.pay_amt));
            decimal totalPaymentAmt = orderAmt + inherentPurchaseAmt + inherentSRAmt + otherPaymentAmt + paidAmount;

            decimal itemTotal = Convert.ToDecimal(salesTotals.ItemTotal);
            decimal itemFinalAmt = Convert.ToDecimal(salesTotals.ItemFinalAmt);

            decimal discountAmtWithTax = 0;
            if (receivedAmount > 0)
                discountAmtWithTax = itemFinalAmt - totalPaymentAmt - receivedAmount;
            else
                discountAmtWithTax = 0;
            decimal newItemTotalWithTax = itemFinalAmt - (decimal)discountAmtWithTax;
            decimal newItemTotal = Math.Round(newItemTotalWithTax / (1 + totalGSTInclCessPercent / 100.00M), 2);
            decimal newGSTAmtInclCess = Math.Round(newItemTotal * (totalGSTInclCessPercent / 100.00M), 2);
            decimal newGSTAmt = Math.Round(newItemTotal * (gstPercent / 100.00M), 2);
            decimal newGSTCessAmt = Math.Round(newItemTotal * (gstCessPercent / 100.00M), 2);
            decimal newSalesTotal = newItemTotal + newGSTAmtInclCess;
            decimal newDiscountAmt = itemTotal - newItemTotal;
            decimal discountPercent = Math.Round(newDiscountAmt / itemTotal * 100, 2);

            return Ok(new SalesInvoiceAttributeVM
            {
                SalesAmountExclTax = newItemTotal,
                DiscountAmount = newDiscountAmt,
                GSTAmount = newGSTAmt,
                GSTCessAmount = newGSTCessAmt,
                GSTAmountInclCess = newGSTAmtInclCess,
                SalesAmountInclTax = newSalesTotal,
                DiscountPercent = discountPercent,
                OldPurchaseAmount = inherentPurchaseAmt,
                OrderAmount = orderAmt
            });
        }

        /// <summary>
        /// Calculation of Receivable Amount
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estimateNo"></param>
        /// <param name="differenceDiscountAmt"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ReceivableCalc")]
        public IHttpActionResult CalculateReceivable(string companyCode, string branchCode, int estimateNo, decimal differenceDiscountAmt)
        {
            ErrorVM error = new ErrorVM();
            ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            var claimValue = principal.Claims.Where(c => c.Type == "UserID").FirstOrDefault();
            if (claimValue == null) {
                return Content(HttpStatusCode.BadRequest, "Unable to get the user information.");
            }
            string userID = claimValue.Value;

            SalesInvoiceAttributeVM salesInvoiceAttributes =
                new SalesBillingBL().CalculateReceivable(companyCode, branchCode, estimateNo, differenceDiscountAmt, userID, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(salesInvoiceAttributes);
        }

        [HttpPost]
        [Route("SalesStockPost")]
        public IHttpActionResult SalesStockPost(string companyCode, string branchCode, int billNo)
        {
            string errorMsg = string.Empty;
            bool flag = new SalesBillingBL().SalesStockPost(null, companyCode, branchCode, billNo, false, out errorMsg);
            if (!flag) {
                return Content(HttpStatusCode.BadRequest, errorMsg);
            }
            return Ok();
        }

        /// <summary>
        /// Cancel sales invoice
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="billNo"></param>
        /// <param name="remarks"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Cancel")]
        [Route("Cancel/{companyCode}/{branchCode}/{billNo}/{remarks}")]
        public IHttpActionResult CancelSalesInvoice(string companyCode, string branchCode, int billNo, string remarks)
        {
            string userId = GetUserId();
            ErrorVM error = new ErrorVM();
            bool isbillCancelled = new SalesBillingBL().CancelSalesInvoice(companyCode, branchCode, billNo, remarks, userId, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok();
        }

        /// <summary>
        /// Get Adjusted Bills for the day.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("AdjustedPurchaseBill/{companyCode}/{branchCode}")]
        [Route("AdjustedPurchaseBill")]
        public IHttpActionResult GetAdjustedPurchaseBills(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            dynamic data = new PurchaseBillingBL().GetAdjustedPurchaseBills(companyCode, branchCode, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(data);
        }

        /// <summary>
        /// Get All Bill Numbers based on Date and Cancelled flag.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="date"></param>
        /// <param name="isCancelled"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("AllBill/{companyCode}/{branchCode}/{date}/{isCancelled}")]
        [Route("AllBill")]
        public IHttpActionResult GetAdjustedPurchaseBills(string companyCode, string branchCode, DateTime date, bool isCancelled)
        {
            ErrorVM error = new ErrorVM();
            dynamic data = new SalesBillingBL().GetAllSalesBillNO(companyCode, branchCode, date, isCancelled, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(data);
        }

        /// <summary>
        /// Getting Bill Information for do Cancel.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="billNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("BillInfo/{companyCode}/{branchCode}/{billNo}")]
        [Route("BillInfo")]
        public IHttpActionResult GetBillInformation(string companyCode, string branchCode, int billNo)
        {
            ErrorVM error = new ErrorVM();
            dynamic data = new SalesBillingBL().GetBillInformation(companyCode, branchCode, billNo, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(data);
        }

        /// <summary>
        /// Get Bill Type for Sales Billing.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("BillType/{companyCode}/{BranchCode}")]
        [Route("BillType")]
        public IHttpActionResult GetBillType(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            dynamic data = new SalesBillingBL().GetBillType(companyCode, branchCode, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(data);
        }

        /// <summary>
        /// Get Attached Other Information of Bill No.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="billNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("AttachedInfoBill/{companyCode}/{branchCode}/{billNo}")]
        [Route("AttachedInfoBill")]
        public IHttpActionResult GetAttachementInformationOfBill(string companyCode, string branchCode, int billNo)
        {
            ErrorVM error = new ErrorVM();
            dynamic data = new SalesBillingBL().GetAttachementInformationOfBill(companyCode, branchCode, billNo, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(data);
        }

        /// <summary>
        /// Calculates the final Balance Amount including discounts if any.
        /// Note: 1. Ensure that you pass CalculateFinalAmount as false to fetch the details of the estimation.
        /// 2. And pass CalculateFinalAmount as true to calculate discounts.
        /// 3. The RowVersion must be passed as null to get the estimate detail, but to calculate the discount, a value
        /// which is obtained by the previous call of Derive-Estimation-Balances must be passed.
        /// 4. Either of difference DifferenceDiscountAmt or ReceivableAmount must be passed, not both.
        /// 5. In case of Receivable Amount ensure that you pass positive value for ReceivableAmount. In case of payble, it must be negative.
        /// </summary>
        /// <param name="estimationQuery">The details of the estimation in question</param>
        /// <returns>returns SalesCompleteInfoVM on success</returns>
        [HttpPost]
        [Route("Derive-Estimation-Balances")]
        [ResponseType(typeof(SalesDerivationVM))]
        public IHttpActionResult DeriveEstimateBalances(SalesInfoQueryVM estimationQuery)
        {
            if (!ModelState.IsValid) {               
                StringBuilder sb = new StringBuilder();
                sb.Append("Model validation error(s) found: " + Environment.NewLine);
                int modelErrorNo = 0;
                foreach (var state in ModelState) {
                    foreach (var error in state.Value.Errors) {
                        sb.Append((++modelErrorNo).ToString() + ". ");
                        sb.Append(string.IsNullOrEmpty(error.ErrorMessage) ? error.Exception.Message : error.ErrorMessage);
                        sb.Append(Environment.NewLine);
                    }
                }
                ErrorVM modelErrors = new ErrorVM();
                modelErrors.description = sb.ToString();
                return Content(HttpStatusCode.BadRequest, modelErrors);
            }

            SalesDerivationVM salesInfo = null;
            string errorMessage = string.Empty;
            string userID = base.GetUserId();
            var isSuccess = new SalesBillingBL().DeriveEstimateBalances(estimationQuery, userID, out salesInfo, out errorMessage);
            if (!isSuccess) {
                ErrorVM error = new ErrorVM();
                error.ErrorStatusCode = HttpStatusCode.BadRequest;
                error.description = errorMessage;
                return Content(HttpStatusCode.BadRequest, error);
            }
            return Ok(salesInfo);
        }

        #region Order OTP
        /// <summary>
        /// Send OTP to Customer
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="orderNo"></param>
        /// <param name="mobileNo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ReSendOrderOTP/{companyCode}/{branchCode}/{orderNo}/{mobileNo}")]
        public IHttpActionResult ReSendOrderOTP(string companyCode, string branchCode, int orderNo, string mobileNo)
        {
            string userId = GetUserId();
            ErrorVM error = new ErrorVM();
            bool sent = new SalesBillingBL().OrderCloseOTPSMS(companyCode, branchCode, "", userId, "1", 4, 10, orderNo, "1", userId, mobileNo, out error);
            if (sent) {
                return Ok();
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Validate OTP
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="mobileNo"></param>
        /// <param name="orderNo"></param>
        /// <param name="smsID"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ValidateOTP/{companyCode}/{branchCode}/{mobileNo}/{orderNo}/{smsID}/{password}")]
        public IHttpActionResult ValidateOTP(string companyCode, string branchCode, string mobileNo, int orderNo, string smsID, string password)
        {
            string userId = GetUserId();
            ErrorVM error = new ErrorVM();
            bool sent = new SalesBillingBL().ValidateOrderClosingOTP(companyCode, branchCode, mobileNo, Convert.ToInt32(password), "1", "", orderNo, userId);
            if (sent) {
                return Ok();
            }
            else {
                return Content(HttpStatusCode.BadRequest, new ErrorVM() { description = "Invalide OTP" });
            }
        }
        #endregion

        public IHttpActionResult Count(ODataQueryOptions<SalesMasterVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Delete([FromBody] int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<SalesMasterVM> List()
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Post(SalesMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, SalesMasterVM t)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
