using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System.Linq;
using System.Web.Http;
using System;
using System.Web.Http.OData.Query;

namespace ProdigyAPI.Controllers.Masters
{
    [Authorize]
    [RoutePrefix("api/masters/idproof")]
    public class IDProofController : SIBaseApiController<IDProofVM>, IBaseMasterActionController<IDProofVM, IDProofVM>
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new Model.MagnaDb.MagnaDbEntities();
        #endregion

        #region Controller Methods
        /// <summary>
        /// Get list of all ID Proofs.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        public IQueryable<IDProofVM> List()
        {
            return db.KSTS_CUSTOMER_ID_PROOF_MASTER.Select(st => new IDProofVM()
            {
                ObjID = st.obj_id,
                CompanyCode = st.company_code,
                BrnachCode = st.branch_code,
                DocCode = st.Doc_code,
                DocName = st.Doc_name,
                ObjStatus = st.obj_status
            });
        }

        public IHttpActionResult Post(IDProofVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, IDProofVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Count(ODataQueryOptions<IDProofVM> oDataOptions)
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
        #endregion
    }
}
