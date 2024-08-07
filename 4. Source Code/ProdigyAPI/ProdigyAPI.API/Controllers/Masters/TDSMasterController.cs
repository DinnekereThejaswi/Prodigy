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

namespace ProdigyAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/Master/TDSMaster")]
    public class TDSMasterController : SIBaseApiController<TDSMasterVM>, IBaseMasterActionController<TDSMasterVM, TDSMasterVM>
    {
        #region Declaration
        MagnaDbEntities db = new MagnaDbEntities();
        string ModuleSeqNo = "22";
        #endregion

        #region Controller Methods

        /// <summary>
        /// List of TDSMaster Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("List")]
        public IHttpActionResult List()
        {
            var ktm = db.KSTS_TDS_MASTER
                  .Select(s => new TDSMasterVM
                  {
                      ObjID = s.obj_id,
                      CompanyCode = s.company_code,
                      BranchCode = s.branch_code,
                      TDSName = s.tds_name,
                      TDSID = s.tds_id,
                      TDS = s.tds,
                      ObjectStatus = s.obj_status,
                      UpdateOn = s.UpdateOn
                  }).OrderBy(c => c.TDSID);
            return Ok(ktm);
        }

        /// <summary>
        /// Save TDSMaster Details
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        [ResponseType(typeof(TDSMasterVM))]
        public IHttpActionResult Post(TDSMasterVM s)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                KSTS_TDS_MASTER ktm = new KSTS_TDS_MASTER ();
                ktm.obj_id = Common.GetNewGUID();
                ktm.company_code = Common.CompanyCode;
                ktm.branch_code = Common.BranchCode;
                ktm.tds_name = s.TDSName;
                ktm.tds_id = Convert.ToInt32(db.KSTU_ACC_FY_MASTER.FirstOrDefault().fin_year.ToString().Remove(0, 1) + db.KSTS_SEQ_NOS.Where(sq => sq.obj_id == ModuleSeqNo).FirstOrDefault().nextno);
                ktm.tds = s.TDS;
                ktm.obj_status = "O";
                ktm.UpdateOn = Framework.Common.GetDateTime();
                db.KSTS_TDS_MASTER.Add(ktm);
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
        /// Edit TDSMaster Details
        /// </summary>
        /// <param name="ObjID"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put/{ObjID}")]
        [ResponseType(typeof(TDSMasterVM))]
        public IHttpActionResult Put(string ObjID, TDSMasterVM s)
        {
            KSTS_TDS_MASTER ktm = db.KSTS_TDS_MASTER.Where(d => d.obj_id == ObjID).FirstOrDefault();
            if (ktm == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (s.ObjID != ktm.obj_id)
            {
                return BadRequest();
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                ktm.obj_id = ktm.obj_id;
                ktm.company_code = Common.CompanyCode;
                ktm.branch_code = Common.BranchCode;
                ktm.tds_name = s.TDSName;
                ktm.tds_id = ktm.tds_id;
                ktm.tds = ktm.tds;
                ktm.obj_status = "O";
                ktm.UpdateOn = ktm.UpdateOn;
                db.Entry(ktm).State = System.Data.Entity.EntityState.Modified;
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
        ///  Alter ObjectStatus Of TDSMaster Details
        /// </summary>
        /// <param name="ObjID"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("OpenOrClose/{ObjID}")]
        [ResponseType(typeof(TDSMasterVM))]
        public IHttpActionResult OpenOrClose(string ObjID)
        {
            KSTS_TDS_MASTER kdm = db.KSTS_TDS_MASTER.Where(d => d.obj_id == ObjID).FirstOrDefault();
            if (kdm == null)
            {
                return NotFound();
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                kdm.obj_id = kdm.obj_id;
                kdm.obj_status = kdm.obj_status == "O" ? "C" : "O";
                db.Entry(kdm).State = System.Data.Entity.EntityState.Modified;
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

        public IHttpActionResult Put(int id, TDSMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Count(ODataQueryOptions<TDSMasterVM> oDataOptions)
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

        IQueryable<TDSMasterVM> IBaseMasterActionController<TDSMasterVM, TDSMasterVM>.List()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

