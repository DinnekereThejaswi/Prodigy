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
    /// Provides API's for Design Master
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Master/DesignMaster")]
    public class DesignMasterController : SIBaseApiController<DesignMasterVM>, IBaseMasterActionController<DesignMasterVM, DesignMasterVM>
    {
        #region Declaration
        MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Controller Methods
        /// <summary>
        /// List Of DesingMaster Details
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("List/{Code}/{companyCode}/{branchCode}")]
        public IHttpActionResult List(string Code, string companyCode, string branchCode)
        {
            var kdm = db.KTTU_DESIGN_MASTER.Where(r => r.master_design_code == Code && r.company_code == companyCode && r.branch_code == branchCode)
                  .Select(s => new DesignMasterVM
                  {
                      ObjID = s.obj_id,
                      CompanyCode = s.company_code,
                      BranchCode = s.branch_code,
                      MasterDesignCode = s.master_design_code,
                      DesignCode = s.design_code,
                      ObjectStatus = s.obj_status,
                      DesignName = s.design_name,
                      UniqRowID = s.UniqRowID
                  }).OrderBy(s => s.DesignCode).ToList();
            return Ok(kdm);
        }

        /// <summary>
        /// Save DesingMaster Details
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        [ResponseType(typeof(DesignMasterVM))]
        public IHttpActionResult Post(DesignMasterVM s)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            using (var transaction = db.Database.BeginTransaction()) {
                KTTU_DESIGN_MASTER kdm = new KTTU_DESIGN_MASTER();
                kdm.obj_id = Common.GetNewGUID();
                kdm.company_code = s.CompanyCode;
                kdm.branch_code = s.BranchCode;
                kdm.master_design_code = s.MasterDesignCode;
                kdm.design_name = s.DesignName;
                kdm.obj_status = "O";
                kdm.design_code = s.DesignCode;
                kdm.UniqRowID = Guid.NewGuid();
                db.KTTU_DESIGN_MASTER.Add(kdm);
                try {
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception excp) {
                    transaction.Rollback();
                    throw excp;
                }
            }
            return Ok();
        }

        /// <summary>
        /// Edit DesingMaster Details
        /// </summary>
        /// <param name="ObjID"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put/{ObjID}")]
        [ResponseType(typeof(DesignMasterVM))]
        public IHttpActionResult Put(string ObjID, DesignMasterVM s)
        {
            KTTU_DESIGN_MASTER kdm = db.KTTU_DESIGN_MASTER.Where(d => d.obj_id == ObjID).FirstOrDefault();
            if (kdm == null) {
                return NotFound();
            }
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            if (s.ObjID != kdm.obj_id) {
                return BadRequest();
            }
            using (var transaction = db.Database.BeginTransaction()) {
                kdm.obj_id = kdm.obj_id;
                kdm.company_code = s.CompanyCode;
                kdm.branch_code = s.BranchCode;
                kdm.master_design_code = s.MasterDesignCode;
                kdm.design_name = s.DesignName;
                kdm.obj_status = "O";
                kdm.design_code = kdm.design_code;
                kdm.UniqRowID = kdm.UniqRowID;
                db.Entry(kdm).State = System.Data.Entity.EntityState.Modified;
                try {
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception excp) {
                    transaction.Rollback();
                    throw excp;
                }
            }
            return Ok();
        }

        /// <summary>
        ///  Alter ObjectStatus Of StockGroup Details
        /// </summary>
        /// <param name="ObjID"></param>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("OpenOrClose/{ObjID}/{companyCode}/{branchCode}")]
        [ResponseType(typeof(DesignMasterVM))]
        public IHttpActionResult OpenOrClose(string ObjID, string companyCode, string branchCode)
        {
            KTTU_DESIGN_MASTER kdm = db.KTTU_DESIGN_MASTER.Where(d => d.obj_id == ObjID && d.company_code == companyCode && d.branch_code == branchCode).FirstOrDefault();
            if (kdm == null) {
                return NotFound();
            }
            using (var transaction = db.Database.BeginTransaction()) {
                kdm.obj_id = kdm.obj_id;
                kdm.obj_status = kdm.obj_status == "O" ? "C" : "O";
                db.Entry(kdm).State = System.Data.Entity.EntityState.Modified;
                try {
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception excp) {
                    transaction.Rollback();
                    throw excp;
                }
            }
            return Ok();
        }

        public IHttpActionResult Count(ODataQueryOptions<DesignMasterVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, DesignMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Delete([FromBody] int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<DesignMasterVM> List()
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
