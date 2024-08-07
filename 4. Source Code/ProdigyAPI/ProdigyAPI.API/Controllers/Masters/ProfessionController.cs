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
    [RoutePrefix("api/masters")]
    public class ProfessionController : ApiController
    {

        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new Model.MagnaDb.MagnaDbEntities();
        #endregion

        #region Controller Methods
        /// <summary>
        /// Get List of Professions.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("profession/list")]
        public IQueryable<ProfessionMasterVM> List()
        {
            return db.KSTU_PROFESSION_MASTER.Select(p => new ProfessionMasterVM()
            {
                ObjID = p.obj_id,
                CompanyCode = p.company_code,
                BranchCode = p.branch_code,
                ProfessionID = p.profession_ID,
                ProfessionName = p.profession_Name,
                ObjStatus = p.obj_status
            });
        }
        #endregion
    }
}
