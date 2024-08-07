using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Stock;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData.Query;

namespace ProdigyAPI.Controllers
{
    [RoutePrefix("api/Views")]
    public class ViewsController : SIBaseApiController<UOMVM>, IBaseMasterActionController<UOMVM, UOMVM>
    {
        #region Declaration
        MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Controller Methods

        /// <summary>
        /// List Of UOM
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ListOfUOM")]
        public IHttpActionResult GetUOM()
        {
            var uom = db.vGetUOMs
                     .Select(s => new UOMVM()
                     {
                         Code = s.code,
                         Name = s.Name
                     }).ToList();
            return Ok(uom);
        }

        /// <summary>
        /// List Of GetStoneTypes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ListOfStoneTypes")]
        public IHttpActionResult GetStoneTypes()
        {
            var St = db.vGetStoneTypes
                     .Select(s => new StoneTypesVM()
                     {
                         Code = s.code,
                         Name = s.Name
                     }).ToList();
            return Ok(St);
        }

        /// <summary>
        /// List of Types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ListOfTypes")]
        public IHttpActionResult ListOfTypes()
        {
            var Types = db.vTypesinHSNMasters
                  .Select(s => new TypesInHSNMasterVm
                  {
                      Code = s.Code,
                      Type = s.Type
                  }).OrderBy(s => s.Type).ToList();
            return Ok(Types);
        }

        /// <summary>
        /// List Of CardType
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ListOfCardType")]
        public IHttpActionResult listofCardType()
        {
            var results = db.vGetDebitCredits
                 .Select(s => new CardTypeVM
                 {
                     Name = s.Name,
                     Code = s.code
                 }).OrderBy(c => c.Name);
            return Ok(results);
        }

        /// <summary>
        /// List of GroupTypes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ListOfGroupTypes")]
        public IHttpActionResult ListOfGroupTypes()
        {
            var kgt = db.vGroupTypes.Select(s => new GroupTypesVM
            {
                Code = s.Code,
                GroupType = s.GroupType
            }).OrderBy(s => s.GroupType).ToList();
            return Ok(kgt);
        }

        /// <summary>
        /// Flags for to get grossweight or netweight
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ListofFlags")]
        public IHttpActionResult listofFlags()
        {
            var krm = db.vFlags
                 .Select(s => new FlagsVM
                 {
                     Code = s.Code,
                     Type = s.Type
                 }).OrderBy(s => s.Type).ToList();
            return Ok(krm);
        }

        public IQueryable<UOMVM> List()
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Count(ODataQueryOptions<UOMVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Post(UOMVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, UOMVM t)
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
