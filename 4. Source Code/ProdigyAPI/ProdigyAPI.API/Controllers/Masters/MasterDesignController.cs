using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Framework;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData.Query;

namespace ProdigyAPI.Controllers.Masters
{
    [Authorize]
    [RoutePrefix("api/Master/MasterDesign")]
    public class MasterDesignController : SIBaseApiController<MasterDesignVM>, IBaseMasterActionController<MasterDesignVM, MasterDesignVM>
    {
        #region Declaration
        MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Controller Methods

        /// <summary>
        /// List of MasterDesign Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("List")]
        public IQueryable<MasterDesignVM> List()
        {
            return db.KSTU_MASTER_DESIGN.Select(s => new MasterDesignVM
            {
                ObjID = s.obj_id,
                CompanyCode = s.company_code,
                BranchCode = s.branch_code,
                MasterDesignCode = s.master_design_code,
                MasterDesignName = s.master_design_name,
                ObjectStatus = s.obj_status,
                UpdateOn = s.UpdateOn,
            }).OrderBy(c => c.MasterDesignCode);
        }

        /// <summary>
        /// Save MasterDesign Details
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        [ResponseType(typeof(MasterDesignVM))]
        public IHttpActionResult Post(MasterDesignVM s)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                KSTU_MASTER_DESIGN kmd = new KSTU_MASTER_DESIGN();
                kmd.obj_id = Common.GetNewGUID();
                kmd.company_code = Common.CompanyCode;
                kmd.branch_code = Common.BranchCode;
                kmd.master_design_code = s.MasterDesignCode;
                kmd.master_design_name = s.MasterDesignName;
                kmd.obj_status = "O";
                kmd.UpdateOn = Framework.Common.GetDateTime();
                db.KSTU_MASTER_DESIGN.Add(kmd);
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
        /// Edit MasterDesign Details
        /// </summary>
        /// <param name="ObjID"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put/{ObjID}")]
        [ResponseType(typeof(MasterDesignVM))]
        public IHttpActionResult Put(string ObjID, MasterDesignVM s)
        {
            KSTU_MASTER_DESIGN kmd = db.KSTU_MASTER_DESIGN.Where(d => d.obj_id == ObjID).FirstOrDefault();
            if (kmd == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (s.ObjID != kmd.obj_id)
            {
                return BadRequest();
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                kmd.obj_id = kmd.obj_id;
                kmd.company_code = Common.CompanyCode;
                kmd.branch_code = Common.BranchCode;
                kmd.master_design_name = s.MasterDesignName;
                kmd.master_design_code = kmd.master_design_code;
                kmd.obj_status = "O";
                kmd.UpdateOn = kmd.UpdateOn;
                db.Entry(kmd).State = System.Data.Entity.EntityState.Modified;
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
        /// Alter ObjectStatus Of  MasterDesign Details
        /// </summary>
        /// <param name="ObjID"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("OpenOrClose/{ObjID}")]
        [ResponseType(typeof(MasterDesignVM))]
        public IHttpActionResult OpenOrClose(string ObjID)
        {
            KSTU_MASTER_DESIGN kmd = db.KSTU_MASTER_DESIGN.Where(d => d.obj_id == ObjID).FirstOrDefault();
            if (kmd == null)
            {
                return NotFound();
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                kmd.obj_id = kmd.obj_id;
                kmd.obj_status = kmd.obj_status == "O" ? "C" : "O";
                db.Entry(kmd).State = System.Data.Entity.EntityState.Modified;
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

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, MasterDesignVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Delete([FromBody] int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Count(ODataQueryOptions<MasterDesignVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
