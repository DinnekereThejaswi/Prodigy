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
    /// Provides methods to manage stones
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Master/StoneMasterNew")]
    public class StoneMasterNewController : SIBaseApiController<StoneMasterVM>
    {
        #region Declaration

        MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Controller Methods
        /// <summary>
        /// Gets the list of stones
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="type">Type can be STN, DMD, etc</param>
        /// <param name="stoneType">Should be: Precious, Semi Precious, Ordinary</param>
        /// <returns></returns>
        [HttpGet]
        [Route("List/{companyCode}/{branchCode}/{type}/{stoneType}")]
        [Route("List")]
        public IHttpActionResult List(string companyCode, string branchCode, string type, string stoneType)
        {
            var ksdm = db.KSTU_STONE_DIAMOND_MASTER
                   .Where(s => s.company_code == companyCode && s.branch_code == branchCode 
                   && s.type == type && s.stone_types == stoneType)
                   .Select(s => new StoneMasterVM
                   {
                       CompanyCode = s.company_code,
                       BranchCode = s.branch_code,
                       Type = s.type,
                       StoneType = s.stone_types,
                       StoneName = s.stone_name,
                       CounterCode = s.counter_code,
                       BrandName = s.brand_name,
                       Color = s.color,
                       Cut = s.cut,
                       Clarity = s.clarity,
                       Status = s.obj_status == "O" ? "Active" : "Closed",
                       Code = s.code,
                       Batch = s.batch,
                       Uom = s.uom,
                       HSN = s.HSN,
                       StoneValue = s.stone_value,
                       GSTGroupCode = s.GSTGroupCode
                   }).Take(10000).OrderByDescending(c => c.StoneName);
            return Ok(ksdm);
        }

        /// <summary>
        /// Gets the stone master
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="type">The type of the stone: example: STN, DMD</param>
        /// <param name="stoneName">Name of the stone</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get/{companyCode}/{branchCode}/{type}/{stoneName}")]
        [Route("get")]
        [ResponseType(typeof(StoneMasterVM))]
        public IHttpActionResult Get(string companyCode, string branchCode, string type, string stoneName)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            try {
                var stoneMaster = db.KSTU_STONE_DIAMOND_MASTER
                   .Where(s => s.company_code == companyCode && s.branch_code == branchCode
                   && s.type == type && s.stone_name == stoneName)
                   .Select(s => new StoneMasterVM
                   {
                       CompanyCode = s.company_code,
                       BranchCode = s.branch_code,
                       Type = s.type,
                       StoneType = s.stone_types,
                       StoneName = s.stone_name,
                       CounterCode = s.counter_code,
                       BrandName = s.brand_name,
                       Color = s.color,
                       Cut = s.cut,
                       Clarity = s.clarity,
                       Status = s.obj_status == "O" ? "Active" : "Closed",
                       Code = s.code,
                       Batch = s.batch,
                       Uom = s.uom,
                       HSN = s.HSN,
                       StoneValue = s.stone_value,
                       GSTGroupCode = s.GSTGroupCode
                   }).FirstOrDefault();


                if (stoneMaster == null) {
                    return NotFound();
                }
                else
                    return Ok(stoneMaster);

            }
            catch (Exception ex) {
                return Content(HttpStatusCode.BadRequest, new ErrorVM().GetErrorDetails(ex));
            }

            return Ok();
        }

        /// <summary>
        /// Save StoneMasterNew Details
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        [ResponseType(typeof(StoneMasterVM))]
        public IHttpActionResult Post(StoneMasterVM s)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            try {
                KSTU_STONE_DIAMOND_MASTER ksdm = new KSTU_STONE_DIAMOND_MASTER();
                ksdm.obj_id = Common.GetNewGUID();
                ksdm.company_code = s.CompanyCode;
                ksdm.branch_code = s.BranchCode;
                ksdm.type = s.Type;
                ksdm.stone_types = s.StoneType;
                ksdm.stone_name = s.StoneName;
                ksdm.counter_code = s.CounterCode;
                ksdm.brand_name = s.BrandName;
                ksdm.color = s.Color;
                ksdm.cut = s.Cut;
                ksdm.clarity = s.Clarity;
                ksdm.code = s.Code;
                ksdm.batch = s.Batch;
                ksdm.uom = "C";
                ksdm.HSN = s.HSN;
                ksdm.stone_value = s.StoneValue;
                ksdm.obj_status = "O";
                ksdm.UpdateOn = Framework.Common.GetDateTime();
                ksdm.UniqRowID = Guid.NewGuid();
                db.KSTU_STONE_DIAMOND_MASTER.Add(ksdm);

                db.SaveChanges();
            }
            catch (Exception ex) {
                return Content(HttpStatusCode.BadRequest, new ErrorVM().GetErrorDetails(ex));
            }
            return Ok();
        }

        /// <summary>
        /// Update stone master
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="type">The type of the stone: example: STN, DMD</param>
        /// <param name="stoneName">Name of the stone</param>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("put/{companyCode}/{branchCode}/{type}/{stoneName}")]
        [ResponseType(typeof(StoneMasterVM))]
        public IHttpActionResult Put(string companyCode, string branchCode, string type, string stoneName, [FromBody] StoneMasterVM s)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            KSTU_STONE_DIAMOND_MASTER stn = db.KSTU_STONE_DIAMOND_MASTER.Where(x => x.company_code == companyCode
                && x.branch_code == branchCode && x.type == type && x.stone_name == stoneName).FirstOrDefault();
            if (stn == null) {
                return NotFound();
            }
            if(stn.obj_status != "O") {
                return Content(HttpStatusCode.BadRequest, new ErrorVM {ErrorStatusCode = HttpStatusCode.BadRequest, description = "The stone is not active. To edit, you must activate it first." });
            }
            try {
                stn.type = s.Type;
                stn.stone_name = s.StoneName;
                stn.stone_types = s.StoneType;
                stn.counter_code = s.CounterCode;
                stn.brand_name = s.BrandName;
                stn.color = s.Color;
                stn.cut = s.Cut;
                stn.clarity = stn.clarity;
                stn.code = s.Code;
                stn.batch = s.Batch;
                stn.uom = s.Uom;
                stn.stone_value = s.StoneValue;
                stn.UpdateOn = Framework.Common.GetDateTime();
                stn.UniqRowID = Guid.NewGuid();
                stn.HSN = s.HSN;
                stn.GSTGroupCode = s.GSTGroupCode;
                db.Entry(stn).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex) {
                return Content(HttpStatusCode.BadRequest, new ErrorVM().GetErrorDetails(ex));
            }

            return Ok();
        }

        /// <summary>
        /// Activates an inactive stone
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="type">The type of the stone: example: STN, DMD</param>
        /// <param name="stoneName">Name of the stone</param>
        /// <returns></returns>
        [HttpPut]
        [Route("Activate/{companyCode}/{branchCode}/{type}/{stoneName}")]
        [ResponseType(typeof(StoneMasterVM))]
        public IHttpActionResult Activate(string companyCode, string branchCode, string type, string stoneName)
        {
            KSTU_STONE_DIAMOND_MASTER stn = db.KSTU_STONE_DIAMOND_MASTER.Where(x => x.company_code == companyCode
                 && x.branch_code == branchCode && x.type == type && x.stone_name == stoneName).FirstOrDefault();
            if (stn == null) {
                return NotFound();
            }           
            if(stn.obj_status == "O") {
                var error = new ErrorVM { description = "The stone is already active." };
                return Content(HttpStatusCode.BadRequest, error);
            }
            try {
                stn.obj_status = "O";
                stn.UpdateOn = Framework.Common.GetDateTime();
                db.Entry(stn).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex) {
                return Content(HttpStatusCode.BadRequest, new ErrorVM().GetErrorDetails(ex));
            }

            return Ok();
        }

        /// <summary>
        /// De-Activates an inactive stone
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="type">The type of the stone: example: STN, DMD</param>
        /// <param name="stoneName">Name of the stone</param>
        /// <returns></returns>
        [HttpPut]
        [Route("Deactivate/{companyCode}/{branchCode}/{type}/{stoneName}")]
        [ResponseType(typeof(StoneMasterVM))]
        public IHttpActionResult DeActivate(string companyCode, string branchCode, string type, string stoneName)
        {
            KSTU_STONE_DIAMOND_MASTER stn = db.KSTU_STONE_DIAMOND_MASTER.Where(x => x.company_code == companyCode
                 && x.branch_code == branchCode && x.type == type && x.stone_name == stoneName).FirstOrDefault();
            if (stn == null) {
                return NotFound();
            }
            if (stn.obj_status == "C") {
                var error = new ErrorVM { description = "The stone is already Inactive." };
                return Content(HttpStatusCode.BadRequest, error);
            }
            try {
                stn.obj_status = "C";
                stn.UpdateOn = Framework.Common.GetDateTime();
                db.Entry(stn).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex) {
                return Content(HttpStatusCode.BadRequest, new ErrorVM().GetErrorDetails(ex));
            }

            return Ok();
        }

        #endregion
    }
}
