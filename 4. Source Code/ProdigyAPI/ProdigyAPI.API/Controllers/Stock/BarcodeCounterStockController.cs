using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ProdigyAPI.Model.MagnaDb;

namespace ProdigyAPI.Controllers.Stock
{
    [RoutePrefix("api/Stock/BarcodeCounterStock")]
    public class BarcodeCounterStockController : ApiController
    {
        private MagnaDbEntities db = new MagnaDbEntities();
        [HttpGet]
        [Route("Get/{companyCode}/{branchCode}/{gSCode}/{counterCode}")]
        [Route("Get")]
        [ResponseType(typeof(usp_BarcodeCounterStockReport_Result))]
        public IQueryable<usp_BarcodeCounterStockReport_Result> Get(string companyCode, string branchCode, string gSCode, string counterCode)
        {
            return db.usp_BarcodeCounterStockReport(companyCode, branchCode, gSCode, counterCode).AsQueryable<usp_BarcodeCounterStockReport_Result>();
        }
    }
}
