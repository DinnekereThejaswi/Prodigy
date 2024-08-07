using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Framework;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.Entity;
using System.Net;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Handlers;
using System.Web.Http.OData.Query;

namespace ProdigyAPI.Controllers.Masters
{
    [Authorize]
    [RoutePrefix("api/Master/StoneRateMaster")]
    public class StoneRateMasterController : SIBaseApiController<StoneRateMasterVM>, IBaseMasterActionController<StoneRateMasterVM, StoneRateMasterVM>
    {
        #region Declaration

        MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Controller Methods
        /// <summary>
        /// List Of StoneRateMaster Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("List")]
        public IQueryable<StoneRateMasterVM> List()
        {
            return db.KSTU_DIAMOND_RATE_MASTER.Select(s => new StoneRateMasterVM
            {
                ObjID = s.obj_id,
                CompanyCode = s.company_code,
                BranchCode = s.branch_code,
                DmName = s.dm_name,
                SlNo = s.Slno,
                KaratFrom = s.karat_from,
                KaratTo = s.karat_to,
                Rate = s.rate,
                Color = s.color,
                Cut = s.cut,
                Clarity = s.clarity,
                UpdateOn = s.UpdateOn,
                ObjectStatus = s.object_status,
                Uom = s.uom,
                Polish = s.polish,
                Symmetry = s.symmetry,
                Fluorescence = s.fluorescence,
                Certificate = s.certificate,
                MinAmount = s.MinAmt,
                UniqRowID = s.UniqRowID
            }).Take(1000).OrderByDescending(c => c.UpdateOn);
        }

        /// <summary>
        /// List of StoneRateMaster Details with respect to stonename
        /// </summary>
        /// <param name="GsCode"></param>
        /// <param name="StoneName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("List/{GsCode}/{StoneName}")]
        public IHttpActionResult List(string GsCode, string StoneName)
        {

            KSTU_STONE_DIAMOND_MASTER ksdm = new KSTU_STONE_DIAMOND_MASTER();
            List<KSTU_DIAMOND_RATE_MASTER> kdrm = new List<KSTU_DIAMOND_RATE_MASTER>();
            ksdm = db.KSTU_STONE_DIAMOND_MASTER.Where(e => e.type == GsCode && e.stone_name == StoneName).FirstOrDefault();
            if (ksdm == null)
            {
                return Content(HttpStatusCode.NotFound, new ErrorVM { index = 0, field = "", description = "Invalid StoneName." });
            }
            StoneMasterVM sm = new StoneMasterVM();
            List<StoneRateMasterVM> lststoneratevm = new List<StoneRateMasterVM>();
            kdrm = db.KSTU_DIAMOND_RATE_MASTER.Where(i => i.dm_name == ksdm.stone_name).ToList();
            if (ksdm == null)
            {
                return Content(HttpStatusCode.NotFound, new ErrorVM { index = 0, field = "", description = "Invalid StoneName." });
            }
            foreach (KSTU_DIAMOND_RATE_MASTER s in kdrm)
            {
                StoneRateMasterVM srm = new StoneRateMasterVM();
                srm.ObjID = s.obj_id;
                srm.CompanyCode = s.company_code;
                srm.BranchCode = s.branch_code;
                srm.DmName = s.dm_name;
                srm.SlNo = s.Slno;
                srm.KaratFrom = s.karat_from;
                srm.KaratTo = s.karat_to;
                srm.Rate = s.rate;
                srm.Color = s.color;
                srm.Cut = s.cut;
                srm.Clarity = s.clarity;
                srm.UpdateOn = s.UpdateOn;
                srm.ObjectStatus = s.object_status;
                srm.Uom = s.uom;
                srm.Polish = s.polish;
                srm.Symmetry = s.symmetry;
                srm.Fluorescence = s.fluorescence;
                srm.Certificate = s.certificate;
                srm.MinAmount = s.MinAmt;
                srm.UniqRowID = s.UniqRowID;
                lststoneratevm.Add(srm);
            };
            return Ok(lststoneratevm.ToList());
        }

        /// <summary>
        /// Save StoneRateMaster Details
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        [ResponseType(typeof(StoneRateMasterVM))]
        public IHttpActionResult Post(StoneRateMasterVM s)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                KSTU_DIAMOND_RATE_MASTER kdrm = new KSTU_DIAMOND_RATE_MASTER();
                kdrm.obj_id = Common.GetNewGUID(); ;
                kdrm.company_code = Common.CompanyCode;
                kdrm.branch_code = Common.BranchCode;
                kdrm.dm_name = s.DmName;
                kdrm.Slno = s.SlNo;
                kdrm.karat_from = Convert.ToDecimal(Convert.ToDouble(kdrm.karat_to) + Convert.ToDouble(0.001));
                kdrm.karat_to = s.KaratTo;
                kdrm.rate = s.Rate;
                kdrm.color = s.Color;
                kdrm.cut = s.Cut;
                kdrm.clarity = s.Clarity;
                kdrm.UpdateOn = Framework.Common.GetDateTime();
                kdrm.object_status = "O";
                kdrm.uom = s.Uom;
                kdrm.polish = s.Polish;
                kdrm.symmetry = s.Symmetry;
                kdrm.fluorescence = s.Fluorescence;
                kdrm.certificate = s.Certificate;
                kdrm.MinAmt = s.MinAmount;
                kdrm.UniqRowID = Guid.NewGuid();
                db.KSTU_DIAMOND_RATE_MASTER.Add(kdrm);
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
        /// Edit StoneRateMaster details
        /// </summary>
        /// <param name="ObjID"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put/{ObjID}")]
        [ResponseType(typeof(StoneMasterVM))]
        public IHttpActionResult Put(string ObjID, StoneRateMasterVM s)
        {
            KSTU_DIAMOND_RATE_MASTER kdrm = db.KSTU_DIAMOND_RATE_MASTER.Where(d => d.obj_id == ObjID).FirstOrDefault();
            if (kdrm == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (s.ObjID != kdrm.obj_id)
            {
                return BadRequest();
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                kdrm.obj_id = kdrm.obj_id;
                kdrm.company_code = Common.CompanyCode;
                kdrm.branch_code = Common.BranchCode;
                kdrm.dm_name = kdrm.dm_name;
                kdrm.Slno = kdrm.Slno;
                kdrm.karat_from = kdrm.karat_from;
                kdrm.karat_to = kdrm.karat_to;
                kdrm.rate = kdrm.rate;
                kdrm.color = s.Color;
                kdrm.cut = s.Cut;
                kdrm.clarity = s.Clarity;
                kdrm.UpdateOn = kdrm.UpdateOn;
                kdrm.object_status = s.ObjectStatus;
                kdrm.uom = kdrm.uom;
                kdrm.polish = s.Polish;
                kdrm.symmetry = s.Symmetry;
                kdrm.fluorescence = s.Fluorescence;
                kdrm.certificate = s.Certificate;
                kdrm.MinAmt = s.MinAmount;
                kdrm.UniqRowID = kdrm.UniqRowID;
                db.Entry(kdrm).State = System.Data.Entity.EntityState.Modified;
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

        public IHttpActionResult Count(ODataQueryOptions<StoneRateMasterVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, StoneRateMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult delete([FromBody] int id)
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
