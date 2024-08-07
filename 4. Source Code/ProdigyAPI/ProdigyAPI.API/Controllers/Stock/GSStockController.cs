using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProdigyAPI.Controllers.Stock
{
    [Authorize]
    [RoutePrefix("api/Stock/GSStock")]
    public class GSStockController : ApiController
    {
        MagnaDbEntities db = new MagnaDbEntities();

        [SetControllerPermission]
        [HttpGet]
        [Route("GSSummary/{companyCode}/{branchCode}/{uomType}")]
        [Route("GSSummary")]
        public IHttpActionResult GSSummary(string companyCode, string branchCode, string uomType)
        {

            db.Configuration.UseDatabaseNullSemantics = true;
            switch (uomType) {
                case "W":
                    var gsStock = from ss in db.KTTU_GS_SALES_STOCK
                                  join ie in db.KSTS_GS_ITEM_ENTRY
                                  on new { CompanyCode = ss.company_code, BranchCode = ss.branch_code, GSCode = ss.gs_code }
                                  equals new { CompanyCode = ie.company_code, BranchCode = ie.branch_code, GSCode = ie.gs_code }
                                  where ss.company_code == companyCode && ss.branch_code == branchCode
                                    && ie.measure_type == uomType
                                  orderby ie.display_order
                                  select new
                                  {
                                      Item = ss.item_name,
                                      OpeningGrossWt = ss.opening_gwt,
                                      OpeningNetWt = ss.opening_nwt,
                                      ReceiptGrossWt = ss.receipt_gwt,
                                      ReceiptNetWt = ss.receipt_nwt,
                                      IssueGrossWt = ss.issue_gwt,
                                      IssueNetWt = ss.issue_nwt,
                                      ClosingGrossWt = ss.closing_gwt,
                                      ClosingNetWt = ss.closing_nwt
                                  };
                    return Ok(gsStock.ToList());
                case "P":
                    //var pieceStock = from ss in db.KTTU_GS_SALES_STOCK
                    //                 join ie in db.KSTS_GS_ITEM_ENTRY
                    //                 on new { CompanyCode = ss.company_code, BranchCode = ss.branch_code, GSCode = ss.gs_code }
                    //                 equals new { CompanyCode = ie.company_code, BranchCode = ie.branch_code, GSCode = ie.gs_code }
                    //                 where ss.company_code == companyCode && ss.branch_code == ss.branch_code
                    //                   && ie.measure_type == uomType
                    //                 orderby ie.display_order
                    //                 select new
                    //                 {
                    //                     Item = ss.item_name,
                    //                     OpeningQty = ss.opening_units,
                    //                     ReceiptQty = ss.receipt_units,
                    //                     IssueQty = ss.issue_units,
                    //                     ClosingQty = ss.closing_units
                    //                 };
                    var ps = from kgss in db.KTTU_GS_SALES_STOCK
                             join kgie in db.KSTS_GS_ITEM_ENTRY
                             on new { Company = kgss.company_code, Branch = kgss.branch_code, GSCode = kgss.gs_code }
                             equals new { Company = kgie.company_code, Branch = kgie.branch_code, GSCode = kgie.gs_code }
                             where kgss.company_code == companyCode && kgss.branch_code == branchCode && kgie.measure_type == uomType
                             orderby kgie.display_order
                             select new
                             {
                                 Item = kgss.item_name,
                                 OpeningQty = kgss.opening_units,
                                 ReceiptQty = kgss.receipt_units,
                                 IssueQty = kgss.issue_units,
                                 ClosingQty = kgss.closing_units
                             };
                    return Ok(ps);
                case "C":
                    var caretStock = from ss in db.KTTU_GS_SALES_STOCK
                                     join ie in db.KSTS_GS_ITEM_ENTRY
                                     on new { CompanyCode = ss.company_code, BranchCode = ss.branch_code, GSCode = ss.gs_code }
                                     equals new { CompanyCode = ie.company_code, BranchCode = ie.branch_code, GSCode = ie.gs_code }
                                     where ss.company_code == companyCode && ss.branch_code == ss.branch_code
                                        && ie.measure_type == uomType
                                     orderby ie.display_order
                                     select new
                                     {
                                         Item = ss.item_name,
                                         OpeningCarets = ss.opening_gwt,
                                         ReceiptCarets = ss.receipt_gwt,
                                         IssueCarets = ss.issue_gwt,
                                         ClosingCarets = ss.closing_gwt
                                     };
                    return Ok(caretStock.ToList());
                default:
                    return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "The UoM Type is not found " + uomType });
            }

        }
    }

}
