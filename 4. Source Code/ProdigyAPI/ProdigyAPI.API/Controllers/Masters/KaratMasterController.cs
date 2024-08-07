using ProdigyAPI.BL.ViewModel.Error;
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
    //[Authorize]
    [RoutePrefix("api/Master/KaratMaster")]
    public class KaratMasterController : SIBaseApiController<KaratVM>, IBaseMasterActionController<KaratVM, KaratVM>
    {

        #region Declaration
        MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Controller Methods

        /// <summary>
        /// List Of KaratMaster Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("List")]
        public IQueryable<KaratVM> List()
        {
            return db.KSTS_KARAT_MASTER.Select(k => new KaratVM
            {
                ObjID = k.obj_id,
                CompanyCode = k.company_code,
                BranchCode = k.branch_code,
                Karat = k.karat,
                UpdateOn = k.UpdateOn,
                ObjStatus = k.obj_status,
                UniqRowID = Guid.NewGuid()
            }).OrderByDescending(c => c.Karat);
        }

        /// <summary>
        /// Save KaratMaster Details
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        [ResponseType(typeof(KaratVM))]
        public IHttpActionResult Post(KaratVM k)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            KSTS_KARAT_MASTER karat = db.KSTS_KARAT_MASTER.Where(kt => kt.company_code == k.CompanyCode
                                                                    && kt.branch_code == k.BranchCode
                                                                    && kt.karat == k.Karat).FirstOrDefault();
            if (karat != null) {
                return Content(System.Net.HttpStatusCode.BadRequest, new ErrorVM()
                {
                    description = "Karat already Exitst",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            using (var transaction = db.Database.BeginTransaction()) {
                KSTS_KARAT_MASTER kkm = new KSTS_KARAT_MASTER();
                kkm.obj_id = Common.GetNewGUID();
                kkm.company_code = k.CompanyCode;
                kkm.branch_code = k.BranchCode;
                kkm.karat = k.Karat;
                kkm.obj_status = "O";
                kkm.UpdateOn = Framework.Common.GetDateTime();
                kkm.UniqRowID = Guid.NewGuid();
                db.KSTS_KARAT_MASTER.Add(kkm);
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
        /// Alter ObjectStatus Of KaratMaster Details
        /// </summary>
        /// <param name="ObjID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("OpenOrClose/{ObjID}")]
        [ResponseType(typeof(KaratVM))]
        public IHttpActionResult OpenOrClose(string ObjID)
        {
            KSTS_KARAT_MASTER kkm = db.KSTS_KARAT_MASTER.Where(kar => kar.obj_id == ObjID).FirstOrDefault();
            if (kkm == null) {
                return NotFound();
            }
            using (var transaction = db.Database.BeginTransaction()) {
                kkm.obj_id = kkm.obj_id;
                kkm.obj_status = kkm.obj_status == "O" ? "C" : "O";
                db.Entry(kkm).State = System.Data.Entity.EntityState.Modified;
                try {
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex) {
                    transaction.Rollback();
                    throw ex;
                }
            }
            return Ok();
        }

        public IHttpActionResult Count(ODataQueryOptions<KaratVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, KaratVM t)
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


