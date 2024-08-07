using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Framework;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData.Query;

namespace ProdigyAPI.Controllers.Masters
{
    [RoutePrefix("api/Master/StoneMaster")]
    public class StoneMasterController : SIBaseApiController<StoneMasterVM>
    {
        #region Declaration
        MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Controller Methods

        /// <summary>
        /// List of StoneMaster Details
        /// </summary>
        /// <param name="GsCode"></param>
        /// <param name="StoneTypes"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("List/{GsCode}/{StoneTypes}")]
        public IQueryable<StoneMasterVM> List(string GsCode, string StoneTypes)
        {
            return db.KSTU_STONE_DIAMOND_MASTER
           .Where(s => s.type == GsCode && s.stone_types == StoneTypes)
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
           }).Take(1000).OrderByDescending(c => c.StoneName);
        }

        /// <summary>
        /// Save StoneMaster Details
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        [ResponseType(typeof(StoneMasterVM))]
        public IHttpActionResult Post(StoneMasterVM s)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (var transaction = db.Database.BeginTransaction())
            {
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
                try
                {
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception excp)
                {
                    transaction.Rollback();
                    throw excp;
                }
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
            if (stn.obj_status != "O") {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { ErrorStatusCode = HttpStatusCode.BadRequest, description = "The stone is not active. To edit, you must activate it first." });
            }
            try {
                stn.obj_id = stn.obj_id;
                stn.company_code = stn.company_code;
                stn.branch_code = stn.branch_code;
                stn.type = stn.type;
                stn.stone_types = stn.stone_types;
                stn.counter_code = stn.counter_code;
                stn.brand_name = stn.brand_name;
                stn.color = stn.color;
                stn.cut = stn.cut;
                stn.clarity = stn.clarity;
                stn.code = stn.code;
                stn.batch = stn.batch;
                stn.uom = stn.uom;
                stn.stone_value = stn.stone_value;
                stn.obj_status = "O";
                stn.UpdateOn = Framework.Common.GetDateTime();
                stn.UniqRowID = Guid.NewGuid();
                stn.stone_name = s.StoneName;
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

        #endregion
    }
}
