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
    [Authorize]
    [RoutePrefix("api/Master/Religion")]
    public class ReligionController : SIBaseApiController<ReligionVM>, IBaseMasterActionController<ReligionVM, ReligionVM>
    {
        #region Declaration
        MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Controller Methods

        /// <summary>
        /// List Of ReligionMaster Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("List")]
        public IHttpActionResult List()
        {
            var krm = db.KSTS_RELIGION_MASTER
                 .Select(s => new ReligionVM
                 {
                     BranchCode = s.branch_code,
                     CompanyCode = s.company_code,
                     DisplaySequence = s.display_sequence,
                     ID = s.id,
                     Religion = s.religion,
                     ObjectStatus = s.obj_status,
                     ObjID = s.obj_id
                 }).OrderBy(s => s.ID).ToList();
            return Ok(krm);
        }

        /// <summary>
        /// Save ReligionMaster Details
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        [ResponseType(typeof(ReligionVM))]
        public IHttpActionResult Post(ReligionVM s)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                KSTS_RELIGION_MASTER krm = new KSTS_RELIGION_MASTER();
                krm.obj_id = Common.GetNewGUID(); ;
                krm.company_code = Common.CompanyCode;
                krm.branch_code = Common.BranchCode;
                krm.display_sequence = s.DisplaySequence;
                krm.id = s.ID;
                krm.obj_status = "O";
                krm.religion = s.Religion;
                db.KSTS_RELIGION_MASTER.Add(krm);
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
        /// Edit ReligionMaster Details
        /// </summary>
        /// <param name="ObjID"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put/{ObjID}")]
        [ResponseType(typeof(ReligionVM))]
        public IHttpActionResult Put(string ObjID, ReligionVM s)
        {
            KSTS_RELIGION_MASTER krm = db.KSTS_RELIGION_MASTER.Where(d => d.obj_id == ObjID).FirstOrDefault();
            if (krm == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (s.ObjID != krm.obj_id)
            {
                return BadRequest();
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                krm.obj_id = krm.obj_id;
                krm.company_code = Common.CompanyCode;
                krm.branch_code = Common.BranchCode;
                krm.display_sequence = s.DisplaySequence;
                krm.religion = s.Religion;
                krm.obj_status = "O";
                krm.id = krm.id;
                db.Entry(krm).State = System.Data.Entity.EntityState.Modified;
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
        ///  Alter ObjectStatus Of ReligionMaster 
        /// </summary>
        /// <param name="ObjID"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("OpenOrClose/{ObjID}")]
        [ResponseType(typeof(ReligionVM))]
        public IHttpActionResult OpenOrClose(string ObjID)
        {
            KSTS_RELIGION_MASTER kdm = db.KSTS_RELIGION_MASTER.Where(d => d.obj_id == ObjID).FirstOrDefault();
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



        #endregion
    }
}
