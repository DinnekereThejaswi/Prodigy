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
    /// Provides API's for Salesman.
    /// </summary>
    //[Authorize]
    [RoutePrefix("api/Master/SalesmanMaster")]
    public class SalesmanMasterController : SIBaseApiController<SalesmanMasterVM>, IBaseMasterActionController<SalesmanMasterVM, SalesmanMasterVM>
    {
        #region Declaration
        MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Controller Methods

        /// <summary>
        /// List Of SalesmanMaster Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("List/{companyCode}/{branchCode}")]
        [ResponseType(typeof(List<SalesmanMasterVM>))]
        public IQueryable<SalesmanMasterVM> List(string companyCode, string branchCode)
        {
            return db.KSTU_SALESMAN_MASTER.Where(s => s.company_code == companyCode && s.branch_code == branchCode).Select(s => new SalesmanMasterVM
            {
                ObjID = s.obj_id,
                CompanyCode = s.company_code,
                BranchCode = s.branch_code,
                SalesManCode = s.sal_code,
                SalesManName = s.sal_name,
                UpdateOn = s.UpdateOn,
                ObjStatus = s.obj_status,
                UniqRowID = s.UniqRowID
            }).OrderBy(c => c.SalesManCode);
        }

        /// <summary>
        /// Get the details of the salesman in question
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="salesmanCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Get/{companyCode}/{branchCode}/{salesmanCode}")]
        [ResponseType(typeof(SalesmanMasterVM))]
        public IHttpActionResult Get(string companyCode, string branchCode, string salesmanCode)
        {
            var salesmanInfo = db.KSTU_SALESMAN_MASTER.Where(s => s.company_code == companyCode
                    && s.branch_code == branchCode && s.sal_code == salesmanCode)
                    .Select(x => new SalesmanMasterVM
                    {
                        CompanyCode = x.company_code,
                        BranchCode = x.branch_code,
                        SalesManCode = x.sal_code,
                        SalesManName = x.sal_name,
                        ObjID = x.obj_id,
                        ObjStatus = x.obj_status,
                        UpdateOn = x.UpdateOn
                    }).FirstOrDefault();
            if (salesmanInfo == null) {
                return NotFound();
            }
            else
                return Ok(salesmanInfo);
        }

        /// <summary>
        /// Save SalesmanMaster Details
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
        //[PasswordStampCheckAuthorize]
        [Route("Post")]
        [ResponseType(typeof(SalesmanMasterVM))]
        public IHttpActionResult Post(SalesmanMasterVM s)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var salesman = db.KSTU_SALESMAN_MASTER.Where(x => x.company_code == s.CompanyCode
                && x.branch_code == s.BranchCode && x.sal_code == s.SalesManCode).Select(y => y.sal_code).FirstOrDefault();
            if (salesman != null) {
                ErrorVM error = new ErrorVM { description = "Salesman already exists." };
                return Content(System.Net.HttpStatusCode.BadRequest, error);
            }
            try {
                KSTU_SALESMAN_MASTER ksm = new KSTU_SALESMAN_MASTER();
                ksm.obj_id = Common.GetNewGUID();
                ksm.company_code = s.CompanyCode;
                ksm.branch_code = s.BranchCode;
                ksm.sal_code = s.SalesManCode;
                ksm.sal_name = s.SalesManName;
                ksm.obj_status = "O";
                ksm.UpdateOn = SIGlobals.Globals.GetDateTime();
                ksm.UniqRowID = Guid.NewGuid();
                db.KSTU_SALESMAN_MASTER.Add(ksm);
                db.SaveChanges();

                var salesmanInfo = db.KSTU_SALESMAN_MASTER.Where(x => x.company_code == s.CompanyCode
                    && x.branch_code == s.BranchCode && x.sal_code == s.SalesManCode)
                    .Select(x => new SalesmanMasterVM
                    {
                        CompanyCode = x.company_code,
                        BranchCode = x.branch_code,
                        SalesManCode = x.sal_code,
                        SalesManName = x.sal_name,
                        ObjID = x.obj_id,
                        ObjStatus = x.obj_status,
                        UpdateOn = x.UpdateOn
                    }).FirstOrDefault();

                return Ok(salesmanInfo);
            }
            catch (Exception excp) {
                throw excp;
            }
        }

        /// <summary>
        /// Edit SalesmanMaster Details
        /// </summary>
        /// <param name="code"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put")]
        public IHttpActionResult Put(string code, SalesmanMasterVM s)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            KSTU_SALESMAN_MASTER ksm = db.KSTU_SALESMAN_MASTER.Where(x => x.company_code == s.CompanyCode
                && x.branch_code == s.BranchCode && x.sal_code == code).FirstOrDefault();
            if (ksm == null) {
                return NotFound();
            }
            if (s.SalesManCode != ksm.sal_code) {
                return BadRequest();
            }

            ksm.sal_name = s.SalesManName;
            ksm.obj_status = s.ObjStatus;
            ksm.UpdateOn = SIGlobals.Globals.GetDateTime();
            db.Entry(ksm).State = System.Data.Entity.EntityState.Modified;
            try {
                db.SaveChanges();
            }
            catch (Exception excp) {
                throw excp;
            }

            return Ok();
        }

        /// <summary>
        /// Open or Close a salesman
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="salesmanCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("OpenOrClose/{companyCode}/{branchCode}/{salesmanCode}")]
        public IHttpActionResult OpenOrClose(string companyCode, string branchCode, string salesmanCode)
        {
            KSTU_SALESMAN_MASTER ksm = db.KSTU_SALESMAN_MASTER.Where(s => s.company_code == companyCode
            && s.branch_code == branchCode && s.sal_code == salesmanCode).FirstOrDefault();
            if (ksm == null) {
                return NotFound();
            }

            ksm.obj_status = ksm.obj_status == "O" ? "C" : "O";
            ksm.UpdateOn = SIGlobals.Globals.GetDateTime();
            db.Entry(ksm).State = System.Data.Entity.EntityState.Modified; ;
            try {
                db.SaveChanges();
            }
            catch (Exception excp) {
                throw excp;
            }

            return Ok();
        }

        /// <summary>
        /// Get Count of all Salesman Details
        /// </summary>
        /// <param name="oDataOptions"></param>
        /// <returns></returns>
        [HttpGet]
        //[PasswordStampCheckAuthorize]
        [Route("Count")]
        public IHttpActionResult Count(ODataQueryOptions<SalesmanMasterVM> oDataOptions)
        {
            var query = db.KSTU_SALESMAN_MASTER.Select(s => new SalesmanMasterVM
            {
                ObjID = s.obj_id,
                CompanyCode = s.company_code,
                BranchCode = s.branch_code,
                SalesManCode = s.sal_code,
                SalesManName = s.sal_name,
                UpdateOn = s.UpdateOn,
                ObjStatus = s.obj_status,
                UniqRowID = s.UniqRowID
            }).OrderBy(c => c.SalesManCode);
            return base.GetCount(oDataOptions, query);
        }

        /// <summary>
        /// Get Record Count by Company Code and Branch Code
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("RecordCount")]
        public IHttpActionResult RecordCount(string companyCode, string branchCode)
        {
            var query = db.KSTU_SALESMAN_MASTER.Where(sal => sal.company_code == companyCode && sal.branch_code == branchCode).Select(s => new SalesmanMasterVM
            {
                ObjID = s.obj_id,
                CompanyCode = s.company_code,
                BranchCode = s.branch_code,
                SalesManCode = s.sal_code,
                SalesManName = s.sal_name,
                UpdateOn = s.UpdateOn,
                ObjStatus = s.obj_status,
                UniqRowID = s.UniqRowID
            }).OrderBy(c => c.SalesManCode);
            return Ok(new { RecordCount = query.ToList().Count });
        }
        #endregion

        #region Base Class method - Not in use
        private int ResponseType(Type type)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, SalesmanMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Delete([FromBody] int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotFiniteNumberException();
        }

        public IQueryable<SalesmanMasterVM> List()
        {
            throw new NotImplementedException();
        }
        #endregion
    }

}
