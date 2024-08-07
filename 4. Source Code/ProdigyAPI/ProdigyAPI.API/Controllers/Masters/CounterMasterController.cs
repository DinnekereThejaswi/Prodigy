using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Framework;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData.Query;

namespace ProdigyAPI.Controllers.Masters
{
    /// <summary>
    /// Provides API's Related to Counter
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Master/CounterMaster")]
    public class CounterMasterController : SIBaseApiController<CounterMasterVM>, IBaseMasterActionController<CounterMasterVM, CounterMasterVM>
    {

        #region Declaration
        MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Controller Methods

        /// <summary>
        /// List Of CounterMaster Details With Respect To MainCounterCode
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("List/{companyCode}/{branchCode}")]
        public IHttpActionResult List(string companyCode, string branchCode)
        {

            var counter = db.KSTU_COUNTER_MASTER
                      .Where(s => s.company_code == companyCode && s.branch_code == branchCode)
                      .Select(c => new CounterMasterVM()
                      {
                          ObjID = c.obj_id,
                          CompanyCode = c.company_code,
                          BranchCode = c.branch_code,
                          CounterCode = c.counter_code,
                          CounterName = c.counter_name,
                          UpdateOn = c.UpdateOn,
                          MaincounterCode = c.Maincounter_code,
                          UniqRowID = c.UniqRowID,
                          ObjectStatus = c.obj_status
                      }).ToList<CounterMasterVM>();
            return Ok(counter);
        }

        /// <summary>
        /// List Of CounterMaster Details With Respect To MainCounterCode
        /// </summary>
        /// <param name="mainCounterCode"></param>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ListByMainCounter/{mainCounterCode}/{companyCode}/{branchCode}")]
        public IHttpActionResult List(string mainCounterCode, string companyCode, string branchCode)
        {

            var counter = db.KSTU_COUNTER_MASTER
                      .Where(s => s.Maincounter_code.ToLower() == mainCounterCode && s.company_code == companyCode && s.branch_code == branchCode)
                      .Select(c => new CounterMasterVM()
                      {
                          ObjID = c.obj_id,
                          CompanyCode = c.company_code,
                          BranchCode = c.branch_code,
                          CounterCode = c.counter_code,
                          CounterName = c.counter_name,
                          UpdateOn = c.UpdateOn,
                          MaincounterCode = c.Maincounter_code,
                          UniqRowID = c.UniqRowID,
                          ObjectStatus = c.obj_status
                      }).ToList<CounterMasterVM>();
            return Ok(counter);
        }

        /// <summary>
        /// Save CounterMaster Details
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        [ResponseType(typeof(CounterMasterVM))]
        public IHttpActionResult Post(CounterMasterVM c)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            KSTU_COUNTER_MASTER counter = db.KSTU_COUNTER_MASTER.Where(count => count.counter_code == c.CounterCode).FirstOrDefault();
            if (counter != null) {
                return Content(System.Net.HttpStatusCode.BadRequest, new ErrorVM()
                {
                    description = "Counter Code already Exist.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            using (var transaction = db.Database.BeginTransaction()) {
                KSTU_COUNTER_MASTER kcm = new KSTU_COUNTER_MASTER();
                kcm.obj_id = SIGlobals.Globals.GetNewGUID();
                kcm.company_code = c.CompanyCode;
                kcm.branch_code = c.BranchCode;
                kcm.counter_code = c.CounterCode;
                kcm.counter_name = c.CounterName;
                kcm.Maincounter_code = c.MaincounterCode;
                kcm.obj_status = "O";
                kcm.UpdateOn = SIGlobals.Globals.GetDateTime();
                kcm.UniqRowID = Guid.NewGuid();
                db.KSTU_COUNTER_MASTER.Add(kcm);
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
        /// Edit CounterMaster Details
        /// </summary>
        /// <param name="ObjID"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put/{ObjID}")]
        [ResponseType(typeof(CounterMasterVM))]
        public IHttpActionResult Put(string ObjID, CounterMasterVM c)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            KSTU_COUNTER_MASTER kcm = db.KSTU_COUNTER_MASTER.Where(d => d.obj_id == ObjID
                                                                    && d.company_code == c.CompanyCode
                                                                    && d.branch_code == c.BranchCode).FirstOrDefault();
            if (kcm == null) {
                return NotFound();
            }
          
            if (c.ObjID != kcm.obj_id) {
                return BadRequest();
            }

            kcm.company_code = c.CompanyCode;
            kcm.branch_code = c.BranchCode;
            kcm.counter_code = c.CounterCode;
            kcm.counter_name = c.CounterName;
            kcm.Maincounter_code = c.MaincounterCode;
            kcm.obj_status = c.ObjectStatus;
            kcm.UpdateOn = SIGlobals.Globals.GetDateTime();
            kcm.UniqRowID = kcm.UniqRowID;
            db.Entry(kcm).State = System.Data.Entity.EntityState.Modified;
            try {
                db.SaveChanges();
            }
            catch (Exception excp) {
                throw excp;
            }
            return Ok();
        }

        public IHttpActionResult Count(ODataQueryOptions<CounterMasterVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, CounterMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Delete([FromBody] int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<CounterMasterVM> List()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
