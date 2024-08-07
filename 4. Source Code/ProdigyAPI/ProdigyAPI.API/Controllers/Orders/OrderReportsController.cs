using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.Framework;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProdigyAPI.Controllers.Orders
{
    /// <summary>
    /// Order Reports Controller Provides API's for Order Related Reports such as 
    /// Open Order Details.
    /// Order Received Details.
    /// Order Receipts.
    /// Closed Order Details.
    /// Booking Typewise Opern order Details.
    /// </summary>
    //[Authorize]
    [RoutePrefix("api/order/reports")]
    public class OrderReportsController : ApiController
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Controller Methods
        /// <summary>
        /// Get Opern order details based on selected criteria.
        /// "OPRD" Open order details.
        /// "ORCD" Order Received details.
        /// "ORD" Order Receipt Details.
        /// </summary>
        /// <param name="orderParams"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("orderDetailReports")]
        //public IHttpActionResult GetOpenOrderDetails([FromBody] OrderReportVM orderParams)
        //{
        //    object report;
        //    try
        //    {
        //        switch (orderParams.ReportType.ToUpper())
        //        {
        //            case "OPRD":
        //                if (orderParams.Type == "F")
        //                {
        //                    orderParams.Type = "Today";
        //                }
        //                else if (orderParams.Type == "D")
        //                {
        //                    orderParams.Type = "Delivery";
        //                }
        //                else
        //                {
        //                    orderParams.Type = "ALL";
        //                }
        //                report = db.usp_OrderReport(Convert.ToDateTime(orderParams.StartDate.ToString("MM/dd/yyyy")),
        //                                                 Convert.ToDateTime(orderParams.EndDate.ToString("MM/dd/yyyy")),
        //                                                 orderParams.ItemType, Common.CompanyCode, Common.BranchCode, orderParams.Type);
        //                return Ok(report);
        //            case "ORCD":
        //                report = (from kom in db.KTTU_ORDER_MASTER
        //                          join kod in db.KTTU_ORDER_DETAILS on kom.order_no equals kod.order_no
        //                          join kpd in db.KTTU_PAYMENT_DETAILS on kom.order_no equals kpd.series_no
        //                          where DbFunctions.TruncateTime(kom.order_date) >= DbFunctions.TruncateTime(orderParams.StartDate)
        //                          where DbFunctions.TruncateTime(kom.order_date) <= DbFunctions.TruncateTime(orderParams.EndDate)
        //                          where kom.cflag == "N" && kom.branch_code == Common.BranchCode
        //                                                 && kom.company_code == Common.CompanyCode
        //                                                 && kpd.cflag == "N"
        //                          group new
        //                          {
        //                              kom,
        //                              kod,
        //                              kpd
        //                          } by kom into g
        //                          select new
        //                          {
        //                              BranchCode = g.Key.branch_code,
        //                              OrderNo = g.Key.order_no,
        //                              OrderDate = g.Key.order_date,
        //                              OrderType = g.Key.order_type,
        //                              CustomerID = g.Key.Cust_Id,
        //                              Name = g.Key.cust_name,
        //                              MobileNo = g.Key.mobile_no,
        //                              SalCode = g.Key.sal_code,
        //                              InvNo = g.Key.closed_flag == "Y" || g.Key.bill_no > 0 ? g.Key.closed_branch + "/" + g.Key.bill_no : "",
        //                              Rate = g.Key.rate,
        //                              Karat = g.Key.karat,
        //                              OrderRateType = g.Key.order_rate_type,
        //                              ClosedDate = g.Key.closed_date,
        //                              Status = g.Key.closed_flag == "Y" || g.Key.bill_no > 0 ? "Closed" : "Open",
        //                              Description = g.FirstOrDefault().kod.description,
        //                              OrderDayRate = g.Key.order_day_rate,
        //                              GSCode = g.Key.gs_code,
        //                              AdvanceOrderAmount = g.Key.advance_ord_amount,
        //                              PayAmount = g.Sum(x => x.kpd.pay_amt),
        //                              SGSTPercent = g.FirstOrDefault().kpd.SGST_Percent,
        //                              CGSTPercent = g.FirstOrDefault().kpd.CGST_Percent,
        //                              IGSTPercent = g.FirstOrDefault().kpd.IGST_Percent,
        //                              SGSTAmount = g.Sum(x => x.kpd.SGST_Amount),
        //                              CGSTAmount = g.Sum(x => x.kpd.CGST_Amount),
        //                              IGSTAmount = g.Sum(x => x.kpd.IGST_Amount),
        //                              OrderAdvance = g.Sum(x => x.kpd.pay_amount_before_tax),
        //                              GrandTotal = g.Key.grand_total,
        //                              ItemName = g.FirstOrDefault().kod.item_name,
        //                              Quantity = g.FirstOrDefault().kod.quantity,
        //                              ToGwt = g.FirstOrDefault().kod.to_gwt,
        //                              Amount = g.FirstOrDefault().kod.amt,
        //                              DeliveryDate = g.Key.delivery_date,
        //                              ClosedDate2 = g.Key.closed_flag == "Y" || g.Key.bill_no > 0 ? g.Key.closed_date.ToString() : "",
        //                              ClosedBranch = (g.Key.closed_flag == "Y" || g.Key.bill_no > 0) && g.Key.closed_branch == "" ? g.Key.closed_date.ToString() : "",
        //                          }).OrderBy(result => result.OrderDate).OrderBy(res => res.OrderNo).ToList();
        //                return Ok(report);

        //            case "ORD":
        //                report = db.usp_OrderReceiptReport(Convert.ToDateTime(orderParams.StartDate.ToString("MM/dd/yyyy")),
        //                                                Convert.ToDateTime(orderParams.EndDate.ToString("MM/dd/yyyy")),
        //                                                orderParams.ItemType, Common.CompanyCode, Common.BranchCode);
        //                return Ok(report);
        //        }
        //    }
        //    catch (Exception excp)
        //    {
        //        throw excp;
        //    }
        //    return Ok();
        //}
        #endregion
    }
}
