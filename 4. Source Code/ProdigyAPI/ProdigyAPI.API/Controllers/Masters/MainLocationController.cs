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
    /// <summary>
    /// This Provides API's Related to Main Counter Or Main Location.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Masters/MainLocation")]
    public class MainLocationController : SIBaseApiController<MainLocationVM>, IBaseMasterActionController<MainLocationVM, MainLocationVM>
    {

        #region Declaration
        MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Controller Methods

        /// <summary>
        /// List Of MainLocation details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("List/{companyCode}/{branchCode}")]
        public IQueryable<MainLocationVM> List(string companyCode, string branchCode)
        {
            return db.KSTU_COUNTER_MASTER_MAIN.Where(counter => counter.company_code == companyCode && counter.branch_code == branchCode).Select(s => new MainLocationVM
            {
                ObjID = s.obj_id,
                CompanyCode = s.company_code,
                BranchCode = s.branch_code,
                MainCounterCode = s.main_counter_code,
                MainCounterName = s.main_counter_name,
                ObjectStatus = s.obj_status,
            }).OrderByDescending(c => c.MainCounterCode);
        }

        /// <summary>
        /// Save MainLocation Details
        /// </summary>
        /// <param name="s"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        [ResponseType(typeof(MainLocationVM))]
        public IHttpActionResult Post(MainLocationVM s)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            KSTU_COUNTER_MASTER_MAIN kcmm = new KSTU_COUNTER_MASTER_MAIN();
            kcmm.obj_id = SIGlobals.Globals.GetNewGUID();
            kcmm.company_code = s.CompanyCode;
            kcmm.branch_code = s.BranchCode;
            kcmm.main_counter_code = s.MainCounterCode;
            kcmm.main_counter_name = s.MainCounterName;
            kcmm.obj_status = "O";
            kcmm.UpdateOn = Framework.Common.GetDateTime();
            kcmm.UniqRowID = Guid.NewGuid();
            db.KSTU_COUNTER_MASTER_MAIN.Add(kcmm);
            try {
                db.SaveChanges();
            }
            catch (Exception excp) {
                ErrorVM error = new ErrorVM() { description = excp.Message, ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError };
                return Content(System.Net.HttpStatusCode.InternalServerError, error);
            }
            return Ok();
        }

        /// <summary>
        /// Edit MainLocation Details
        /// </summary>
        /// <param name="ObjID"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put/{objid}")]
        [ResponseType(typeof(MainLocationVM))]
        public IHttpActionResult Put(string ObjID, MainLocationVM s)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            KSTU_COUNTER_MASTER_MAIN kcmm = db.KSTU_COUNTER_MASTER_MAIN.Where(d => d.obj_id == ObjID
                                                        && d.company_code == s.CompanyCode
                                                        && d.branch_code == s.BranchCode).FirstOrDefault();
            if (kcmm == null) {
                return NotFound();
            }
            if (s.ObjID != kcmm.obj_id) {
                return BadRequest();
            }
            kcmm.company_code = s.CompanyCode;
            kcmm.branch_code = s.BranchCode;
            kcmm.main_counter_code = kcmm.main_counter_code;
            kcmm.main_counter_name = s.MainCounterName;
            kcmm.obj_status = s.ObjectStatus;
            kcmm.UpdateOn = kcmm.UpdateOn;
            kcmm.UniqRowID = kcmm.UniqRowID;
            db.Entry(kcmm).State = System.Data.Entity.EntityState.Modified;
            try {
                db.SaveChanges();
            }
            catch (Exception excp) {
                ErrorVM error = new ErrorVM() { description = excp.Message, ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError };
                return Content(System.Net.HttpStatusCode.InternalServerError, error);
            }
            return Ok();
        }

        public IHttpActionResult Count(ODataQueryOptions<MainLocationVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Delete([FromBody] int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, MainLocationVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<MainLocationVM> List()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
