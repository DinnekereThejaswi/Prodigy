using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Framework;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData.Query;


namespace ProdigyAPI.Controllers.Masters
{
    /// <summary>
    /// Provides API's for GST Item Entry Master
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Master/GSItemEntryMaster")]
    public class GSItemEntryMasterController : SIBaseApiController<GSItemEntryMasterVM>
    {

        #region Declaration
        MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Controller Methods
        /// <summary>
        /// List of GSItemEntryMaster Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("List/{companyCode}/{branchCode}")]
        [Route("List")]
        public IHttpActionResult List(string companyCode, string branchCode)
        {
            var gsievm = db.KSTS_GS_ITEM_ENTRY
                   .Where(g => g.company_code == companyCode && g.branch_code == branchCode)
                   .Select(g => new GSItemEntryMasterVM
                   {
                       ObjID = g.obj_id,
                       CompanyCode = g.company_code,
                       BranchCode = g.branch_code,
                       ItemLevel1ID = g.item_level1_id,
                       ItemLevel1Name = g.item_level1_name,
                       GsCode = g.gs_code,
                       MeasureType = g.measure_type,
                       MetalType = g.metal_type,
                       Karat = g.karat,
                       BillType = g.bill_type,
                       Tax = g.tax,
                       OpeningGwt = g.opening_gwt,
                       OpeningGwtValue = g.opening_gwt_value,
                       OpeningNwt = g.opening_nwt,
                       OpeningNwtValue = g.opening_nwt_value,
                       OpeningUnits = g.opening_units,
                       DisplayOrder = g.display_order,
                       CommodityCode = g.commodity_code,
                       ObjectStatus = g.object_status,
                       UpdateOn = g.UpdateOn,
                       EduCess = g.edu_cess,
                       HighEle = g.high_ele,
                       Tcs = g.tcs,
                       IsStone = g.isStone,
                       Purity = g.purity,
                       TcsPerc = g.tcs_perc,
                       STax = g.S_tax,
                       CTax = g.C_tax,
                       ITax = g.I_tax,
                       GSTGoodsGroupCode = g.GSTGoodsGroupCode,
                       GSTServicesGroupCode = g.GSTServicesGroupCode,
                       HSN = g.HSN
                   }).Take(100).OrderBy(c => c.ItemLevel1Name);
            return Ok(gsievm);
        }

        /// <summary>
        /// Get Single GS
        /// </summary>
        /// <param name="id"></param>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GSDet/{id}/{companyCode}/{branchCode}")]
        [Route("GSDet")]
        public IHttpActionResult SingleGS(int id, string companyCode, string branchCode)
        {
            var gsievm = db.KSTS_GS_ITEM_ENTRY
                   .Where(g => g.company_code == companyCode
                            && g.branch_code == branchCode
                            && g.item_level1_id == id)
                   .Select(g => new GSItemEntryMasterVM
                   {
                       ObjID = g.obj_id,
                       CompanyCode = g.company_code,
                       BranchCode = g.branch_code,
                       ItemLevel1ID = g.item_level1_id,
                       ItemLevel1Name = g.item_level1_name,
                       GsCode = g.gs_code,
                       MeasureType = g.measure_type,
                       MetalType = g.metal_type,
                       Karat = g.karat,
                       BillType = g.bill_type,
                       Tax = g.tax,
                       OpeningGwt = g.opening_gwt,
                       OpeningGwtValue = g.opening_gwt_value,
                       OpeningNwt = g.opening_nwt,
                       OpeningNwtValue = g.opening_nwt_value,
                       OpeningUnits = g.opening_units,
                       DisplayOrder = g.display_order,
                       CommodityCode = g.commodity_code,
                       ObjectStatus = g.object_status,
                       UpdateOn = g.UpdateOn,
                       EduCess = g.edu_cess,
                       HighEle = g.high_ele,
                       Tcs = g.tcs,
                       IsStone = g.isStone,
                       Purity = g.purity,
                       TcsPerc = g.tcs_perc,
                       STax = g.S_tax,
                       CTax = g.C_tax,
                       ITax = g.I_tax,
                       GSTGoodsGroupCode = g.GSTGoodsGroupCode,
                       GSTServicesGroupCode = g.GSTServicesGroupCode,
                       HSN = g.HSN
                   }).FirstOrDefault();
            return Ok(gsievm);
        }

        /// <summary>
        /// List Of GsNames Contains Only STN,DMD,OD,OST
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ListofGSNames/{companyCode}/{branchCode}")]
        [Route("ListofGSNames")]
        public IHttpActionResult listofGSNames(string companyCode, string branchCode)
        {
            return Ok(new GSEntryBL().GetAllGS(companyCode, branchCode));
        }

        /// <summary>
        /// Get Goods Type
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GoodsType")]
        public IHttpActionResult GetGoodsType(string companyCode, string branchCode)
        {
            return Ok(new GSEntryBL().GetGoodsServiceType(companyCode, branchCode, "Goods"));
        }

        /// <summary>
        /// Get Service Type
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ServiceType")]
        public IHttpActionResult GetServiceType(string companyCode, string branchCode)
        {
            return Ok(new GSEntryBL().GetGoodsServiceType(companyCode, branchCode, "Service"));
        }

        /// <summary>
        /// Save GSItemEntryMaster Details
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        [ResponseType(typeof(GSItemEntryMasterVM))]
        public IHttpActionResult Post(GSItemEntryMasterVM g)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            bool saved = new GSEntryBL().Save(g, out error);
            if (error == null) {
                return Ok();
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Save GSItemEntryMaster Details
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put/{Id}")]
        [ResponseType(typeof(GSItemEntryMasterVM))]
        public IHttpActionResult Put(int Id, GSItemEntryMasterVM g)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            bool saved = new GSEntryBL().Update(Id, g, out error);
            if (error == null) {
                return Ok();
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        public IHttpActionResult Count(ODataQueryOptions<GSItemEntryMasterVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
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
