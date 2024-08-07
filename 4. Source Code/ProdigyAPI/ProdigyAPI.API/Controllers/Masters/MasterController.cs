using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.Framework;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData.Query;

namespace ProdigyAPI.Controllers.Masters
{
    /// <summary>
    /// Master Controller provides API's for all Masters.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Masters")]
    public class MasterController : ApiController
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region controller Methods
        /// <summary>
        /// Get all Master Counters.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsName"></param>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Counter/{companyCode}/{branchCode}/{gsName}/{itemCode}")]
        [Route("Counter")]
        public IHttpActionResult Counterlist(string companyCode, string branchCode, string gsName, string itemCode)
        {
            return Ok(db.usp_Counters(companyCode, branchCode, gsName, itemCode));
        }

        /// <summary>
        /// Get all Master Items.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Item/{companyCode}/{branchCode}/{gsName}")]
        [Route("Item")]
        public IHttpActionResult Itemlist(string companyCode, string branchCode, string gsName)
        {
            return Ok(db.usp_ItemList(companyCode, branchCode, gsName));
        }

        /// <summary>
        /// Get Item Details by Counter
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="counterCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ItemByCounter/{companyCode}/{branchCode}/{counterCode}")]
        [Route("ItemByCounter")]
        public IHttpActionResult ItemlistByCounter(string companyCode, string branchCode, string counterCode)
        {
            List<GenComboVM> lstGenCombo = new List<GenComboVM>();
            List<KTTU_COUNTER_STOCK> lstOfStock = db.KTTU_COUNTER_STOCK.Where(ks => ks.counter_code == counterCode
                                && ks.company_code == companyCode
                                && ks.branch_code == branchCode
                                && (ks.op_gwt > 0
                                               || ks.op_units > 0
                                               || ks.barcoded_units > 0
                                               || ks.barcoded_gwt > 0
                                               || ks.sales_units > 0
                                               || ks.sales_gwt > 0
                                               || ks.issues_units > 0
                                               || ks.issues_gwt > 0
                                               || ks.receipt_units > 0
                                               || ks.receipt_gwt > 0
                                               || ks.closing_gwt > 0
                                               || ks.closing_units > 0)).OrderBy(k => k.item_name).ToList();
            foreach (KTTU_COUNTER_STOCK kcs in lstOfStock) {
                GenComboVM gen = new GenComboVM();
                gen.Code = kcs.item_name;
                gen.Name = kcs.item_name;
                lstGenCombo.Add(gen);
            }
            return Ok(lstGenCombo);
        }

        /// <summary>
        /// Get all Item properties by GS Name and Item Code.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsName"></param>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ItemProperties/{companyCode}/{branchCode}/{gsName}/{itemCode}")]
        [Route("ItemProperties")]
        public IHttpActionResult ItemPropertiesList(string companyCode, string branchCode, string gsName, string itemCode)
        {
            return Ok(db.usp_ItemProperties(companyCode, branchCode, gsName, itemCode));
        }

        /// <summary>
        /// Get Rate by GS Name.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("RateFromKarat/{companyCode}/{branchCode}/{gsName}")]
        [Route("RateFromKarat")]
        public IHttpActionResult RateFromKarat(string companyCode, string branchCode, string gsName)
        {
            return Ok(db.usp_RateFromKarat(companyCode, branchCode, gsName));
        }

        /// <summary>
        /// Get Master Salesman details.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Salesman/{companyCode}/{branchCode}")]
        [Route("Salesman")]
        public IHttpActionResult Salesman(string companyCode, string branchCode)
        {
            return Ok(db.usp_Salesman(companyCode, branchCode));
        }

        /// <summary>
        /// Get Stone diamond details.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("StoneDiamond/{companyCode}/{branchCode}")]
        [Route("StoneDiamond")]
        public IHttpActionResult StoneDiamond(string companyCode, string branchCode)
        {
            return Ok(db.usp_StoneDiamond(companyCode, branchCode));
        }

        /// <summary>
        /// Get Master MC Types.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("MCTypes/{companyCode}/{branchCode}")]
        [Route("MCTypes")]
        public IHttpActionResult MCTypes(string companyCode, string branchCode)
        {
            return Ok(db.usp_LoadMCTypes(companyCode, branchCode, ""));
        }

        /// <summary>
        /// Get Karat Master.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Karat/{companyCode}/{branchCode}")]
        [Route("Karat")]
        public IHttpActionResult karat(string companyCode, string branchCode)
        {
            return Ok(db.KSTS_KARAT_MASTER.Where(kt => kt.company_code == companyCode && kt.branch_code == branchCode).ToList());
        }

        /// <summary>
        /// Get GS details.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("LoadGS/{companyCode}/{branchCode}/{type}")]
        [Route("LoadGS")]
        public IHttpActionResult LoadGS(string companyCode, string branchCode, string type)
        {
            return Ok(db.usp_LoadGS(companyCode, branchCode, type));
        }

        /// <summary>
        /// Get GST group code by GS Name and Item.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsName"></param>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetGSTGoodsGroupCode/{companyCode}/{branchCode}/{gsName}/{itemCode}")]
        [Route("GetGSTGoodsGroupCode")]
        public IHttpActionResult GetGSTGoodsGroupCode(string companyCode, string branchCode, string gsName, string itemCode)
        {
            return Ok(db.usp_GetGSTGoodsGroupCode(companyCode, branchCode, gsName, itemCode));
        }

        /// <summary>
        /// Get Items by GS Code.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetItems/{companyCode}/{branchCode}/{gsCode}")]
        [Route("GetItems")]
        public IHttpActionResult GetItems(string companyCode, string branchCode, string gsCode)
        {
            DataTable tblData = new DataTable();
            if (gsCode == "OGO") {
                tblData = Common.ExecuteQuery("SELECT * FROM ITEM_MASTER im WHERE im.gs_code='"
                    + gsCode + "' AND im.obj_status='O' AND im.item_code!='OGCN24K' AND im.company_code ='"
                    + companyCode + "' AND im.branch_code ='"
                    + branchCode + "' ORDER BY ITEM_CODE  ");
            }
            else {
                tblData = Common.ExecuteQuery("SELECT * FROM ITEM_MASTER im WHERE im.gs_code='" + gsCode + "' AND im.obj_status='O' AND im.company_code ='" + companyCode + "' AND im.branch_code ='" + branchCode + "' ORDER BY ITEM_CODE  ");
            }
            return Ok(tblData);

            // The bellow code returns wrong values so we are getting data through query
            //var lstOfItems = db.ITEM_MASTER.Where(i => i.gs_code == gsCode && i.obj_status == "O" && i.company_code == companyCode && i.branch_code == branchCode).Select(m => new { Item_code = m.Item_code, Item = m.Item, Item_Name = m.Item_Name });
            //return Ok(lstOfItems);

            //var lstOfItems = db.ITEM_MASTER.Where(i => i.gs_code == gsCode && i.obj_status == "O" && i.company_code == companyCode && i.branch_code == branchCode).ToList();
            //return Ok(lstOfItems);

            //var query = db.ITEM_MASTER.Distinct().Where(i => i.gs_code == gsCode && i.obj_status == "O");
            //var lstOfItems = db.ITEM_MASTER.Where(i => i.gs_code == gsCode && i.obj_status == "O").ToList();
            //return Ok(lstOfItems);
        }

        /// <summary>
        /// Get All Item Details By gsCode and ItemCode
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsCode"></param>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetItemDet/{companyCode}/{branchCode}/{gsCode}/{itemCode}")]
        [Route("GetItemDet")]
        public IHttpActionResult GetItemDet(string companyCode, string branchCode, string gsCode, string itemCode)
        {
            return Ok(db.ITEM_MASTER.Where(item => item.gs_code == gsCode && item.Item_code == itemCode
            && item.company_code == companyCode && item.branch_code == branchCode).ToList());
        }

        /// <summary>
        /// Get Rate per gram For Sales by passing GS Code and Karat.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsCode"></param>
        /// <param name="karat"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetRatePerGramForSales/{companyCode}/{branchCode}/{gsCode}/{karat}")]
        [Route("GetRatePerGramForSales")]
        public IHttpActionResult GetRatePerGramForSales(string companyCode, string branchCode, string gsCode, string karat)
        {
            decimal rate;
            KSTU_RATE_MASTER rateMaster = db.KSTU_RATE_MASTER.Where(r => r.gs_code == gsCode && r.karat == karat
            && r.company_code == companyCode && r.branch_code == branchCode).FirstOrDefault();
            if (rateMaster == null) {
                rate = 0;
            }
            else {
                rate = rateMaster.rate;
            }
            return Ok(new { Rate = rate });
        }

        /// <summary>
        ///  Get Rate amount per Gram by passing GS Code, Karat and Type(Exchange or Cash) For Old Gold.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsCode"></param>
        /// <param name="karat"></param>
        /// <param name="rateType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("RatePerGram/{companyCode}/{branchCode}/{gsCode}/{karat}/{rateType}")]
        [Route("RatePerGram")]
        public IHttpActionResult RatePerGram(string companyCode, string branchCode, string gsCode, string karat, string rateType)
        {
            decimal rate = 0;
            KSTU_RATE_MASTER rateMaster = db.KSTU_RATE_MASTER.Where(r => r.gs_code == gsCode && r.karat == karat && r.company_code == companyCode && r.branch_code == branchCode).FirstOrDefault();
            if (rateMaster == null) {
                return Content(HttpStatusCode.BadRequest, new ErrorVM() { field = "", index = 0, description = "Invalid rateType/gscode/karat" });
            }
            if (rateType.ToUpper() == "E") {
                rate = rateMaster.exchange_rate;
            }
            else if (rateType.ToUpper() == "C") {
                rate = rateMaster.cash_rate;
            }
            return Ok(new { Rate = rate });
        }

        /// <summary>
        /// Get Metal type by GS Code.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMetalType/{companyCode}/{branchCode}/{gsCode}")]
        [Route("GetMetalType")]
        public IHttpActionResult GetMetalType(string companyCode, string branchCode, string gsCode)
        {
            return Ok(new { MetalType = db.KSTS_GS_ITEM_ENTRY.Where(gs => gs.gs_code == gsCode && gs.company_code == companyCode && gs.branch_code == branchCode).FirstOrDefault().metal_type });
        }

        /// <summary>
        /// Get payment modes based on Module names
        /// "SL" Sales
        /// "PR" Purchase
        /// "NO" New Order
        /// "OR" Order Receipt
        /// "OC" Order Close
        /// "SR" Sales Return
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("PayMode/{companyCode}/{branchCode}/{moduleName}")]
        [Route("PayMode")]
        public IHttpActionResult GetPayMode(string companyCode, string branchCode, string moduleName)
        {
            IEnumerable<KTTS_PAYMENT_MASTER> data = new List<KTTS_PAYMENT_MASTER>();
            switch (moduleName.ToUpper()) {
                case "SL"://New Order
                    data = db.KTTS_PAYMENT_MASTER.Where(pay => pay.company_code == companyCode && pay.branch_code == branchCode).OrderBy(pay => pay.seq_no).ToList();
                    break;
                case "NO"://New Order
                case "OR"://Order Receipt
                    List<string> filters = new List<string>() { "PE", "OP", "SE" };
                    data = db.KTTS_PAYMENT_MASTER.Where(pay => !filters.Contains(pay.payment_code) && !pay.payment_code.StartsWith("B") && pay.company_code == companyCode && pay.branch_code == branchCode).OrderBy(pay => pay.seq_no).ToList();
                    List<KTTS_PAYMENT_MASTER> values = db.KTTS_PAYMENT_MASTER.Where(kpm => kpm.company_code == companyCode && kpm.branch_code == branchCode && kpm.payment_code == "BC").ToList();
                    data = data.Union(values);
                    break;
                case "OC"://Order Close
                case "PR"://Purchase
                case "SR"://Sales Return
                    List<string> orderCloseFilter = new List<string>() { "C", "Q", "EP" };
                    data = db.KTTS_PAYMENT_MASTER.Where(pay => orderCloseFilter.Contains(pay.payment_code) && pay.company_code == companyCode && pay.branch_code == branchCode).OrderBy(pay => pay.seq_no).ToList();
                    break;
                case "RD":// Repair Delivery
                    List<string> repairDeliveryFilter = new List<string>() { "C", "Q", "R", "EP", "UPI" };
                    data = db.KTTS_PAYMENT_MASTER.Where(pay => repairDeliveryFilter.Contains(pay.payment_code) && pay.company_code == companyCode && pay.branch_code == branchCode).OrderBy(pay => pay.seq_no).ToList();
                    break;
                case "CR":// Credit Receipt
                    List<string> creditReceiptFilter = new List<string>() { "OP", "SE", "PE", "DD", "BC" };
                    data = db.KTTS_PAYMENT_MASTER.Where(pay => !creditReceiptFilter.Contains(pay.payment_code) && pay.company_code == companyCode && pay.branch_code == branchCode).OrderBy(pay => pay.seq_no).ToList();
                    break;
                case "S":// Sales Billing
                    //data = db.KTTS_PAYMENT_MASTER.Where(pay => pay.company_code == companyCode && pay.branch_code == branchCode).OrderBy(pay => pay.seq_no).ToList();
                    //break;
                    List<string> salesInvFilter = new List<string>() { "PE"};//Purchase estimation excluded
                    data = db.KTTS_PAYMENT_MASTER.Where(pay => !salesInvFilter.Contains(pay.payment_code) && pay.company_code == companyCode && pay.branch_code == branchCode).OrderBy(pay => pay.seq_no).ToList();
                    break;
            }

            List<OrderPayModeVM> lstOrderGSType = new List<OrderPayModeVM>();
            foreach (KTTS_PAYMENT_MASTER kgie in data) {
                OrderPayModeVM ogt = new OrderPayModeVM();
                ogt.PayMode = kgie.payment_code;
                ogt.PayName = kgie.payment_name;
                lstOrderGSType.Add(ogt);
            }
            return Ok(lstOrderGSType);
        }

        /// <summary>
        /// Get Application Date.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetApplicationDate/{companyCode}/{branchCode}")]
        [Route("GetApplicationDate")]
        public IHttpActionResult GetApplicationDate(string companyCode, string branchCode)
        {
            return Ok(new { applcationDate = db.KSTU_RATE_MASTER.Where(r => r.company_code == companyCode && r.branch_code == branchCode).ToList()[0].ondate.ToString("yyyy-MM-dd"), appViewDate = db.KSTU_RATE_MASTER.Where(r => r.company_code == companyCode && r.branch_code == branchCode).ToList()[0].ondate.ToString("dd-MM-yyyy") });
        }

        /// <summary>
        /// Get GST Percentage
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsCode"></param>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetGSTPercent/{companyCode}/{branchCode}/{gsCode}/{itemCode}")]
        [Route("GetGSTPercent")]
        public IHttpActionResult GetGSTPercentage(string companyCode, string branchCode, string gsCode, string itemCode)
        {
            List<GSTComponentVM> lstOfGSTComp = new List<GSTComponentVM>();
            var gstGroupCode = db.usp_GetGSTGoodsGroupCode(companyCode, branchCode, gsCode, itemCode);
            String gstGroupCodeValue = gstGroupCode.FirstOrDefault().ToString();
            List<GSTPostingSetup> gp = db.GSTPostingSetups.Where(gps => gps.GSTGroupCode == gstGroupCodeValue).ToList();
            if (gp != null && gp.Count > 0) {
                foreach (GSTPostingSetup gps in gp) {
                    GSTComponentVM gc = new GSTComponentVM();
                    gc.GSTComponentCode = gps.GSTComponentCode;
                    gc.GSTPercent = gps.GSTPercent;
                    lstOfGSTComp.Add(gc);
                }
            }
            return Ok(lstOfGSTComp);
        }


        [HttpGet]
        [Route("CardType/{companyCode}/{branchCode}")]
        [Route("CardType")]
        public IHttpActionResult GetCardType(string companyCode, string branchCode)
        {
            return Ok(db.KSTS_CARD_MASTER.Where(card => card.company_code == companyCode && card.branch_code == branchCode).OrderBy(card => card.card_name).ToList());
        }

        /// <summary>
        /// Get Financial Year
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("AccFinYear/{companyCode}/{branchCode}")]
        [Route("AccFinYear")]
        public IHttpActionResult GetAccountFinYear(string companyCode, string branchCode)
        {
            KSTU_ACC_FY_MASTER accMaster = db.KSTU_ACC_FY_MASTER.Where(f => f.company_code == companyCode && f.branch_code == branchCode).FirstOrDefault();
            if (accMaster == null) {
                return Content(HttpStatusCode.NotFound, "");
            }
            return Ok(new { FinYear = accMaster.fin_year });
        }
        #endregion
    }
}
