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
    [RoutePrefix("api/masters/state")]
    public class StateController : SIBaseApiController<StateMasterVM>, IBaseMasterActionController<StateMasterVM, StateMasterVM>
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new Model.MagnaDb.MagnaDbEntities();
        #endregion

        #region Controller Methods
        /// <summary>
        /// Get all state details.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        public IQueryable<StateMasterVM> List()
        {
            return db.KSTS_STATE_MASTER.Select(st => new StateMasterVM()
            {
                ID = st.id,
                StateName = st.state_name,
                TINNo = st.tinno,
                ObjectStatus = st.obj_status
            });
        }

        public IHttpActionResult Post(StateMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, StateMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Count(ODataQueryOptions<StateMasterVM> oDataOptions)
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
