using ProdigyAPI.BL.ViewModel.Error;
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
    /// <summary>
    /// Provides API Related to GST Master
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Master/GSTMaster")]
    public class GSTMasterController : SIBaseApiController<GSTGroupVM>, IBaseMasterActionController<GSTGroupVM, GSTGroupVM>
    {

        #region Declaration
        MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Controller Methods

        /// <summary>
        /// List Of GSTMaster details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("List/{companyCode}/{branchCode}")]
        [Route("List")]
        public IQueryable<GSTGroupVM> List(string companyCode, string branchCode)
        {
            return db.GSTGroups.Where(s => s.company_code == companyCode && s.branch_code == branchCode).Select(s => new GSTGroupVM
            {
                Code = s.Code,
                CompanyCode = s.company_code,
                BranchCode = s.branch_code,
                Description = s.Description,
                GSTGroupType = s.GSTGroupType,
                IsActive = s.IsActive,
                LastModifiedBy = s.LastModifiedBy,
                LastModifiedOn = s.LastModifiedOn,
                SortOrder = s.SortOrder
            }).OrderBy(c => c.Code);
        }

        /// <summary>
        /// List of GSTTypes for Goodstype
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetGSTTypes/{companyCode}/{branchCode}")]
        public IHttpActionResult GetGSTTypes(string companyCode, string branchCode)
        {
            var GSTgrp = db.GSTGroups
                .Where(s => s.GSTGroupType == "Goods" && s.company_code == companyCode && s.branch_code == branchCode)
                   .Select(s => new GSTGroupVM
                   {
                       Code = s.Code,
                       Description = s.Description,
                       GSTGroupType = s.GSTGroupType,
                       IsActive = s.IsActive,
                       LastModifiedBy = s.LastModifiedBy,
                       LastModifiedOn = s.LastModifiedOn,
                       SortOrder = s.SortOrder
                   }).OrderBy(s => s.Code).ToList();
            return Ok(GSTgrp);

        }

        /// <summary>
        /// Save GSTMaster details
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        [ResponseType(typeof(GSTGroupVM))]
        public IHttpActionResult Post(GSTGroupVM s)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            GSTGroup checkExist = db.GSTGroups.Where(gst => gst.Code == s.Code && gst.company_code == s.CompanyCode && gst.branch_code == s.BranchCode).FirstOrDefault();
            if (checkExist != null) {
                return Content(HttpStatusCode.BadRequest, new ErrorVM() { description = checkExist.Code + " already Exist.", ErrorStatusCode = HttpStatusCode.BadRequest });
            }
            GSTGroup kgm = new GSTGroup();
            kgm.company_code = s.CompanyCode;
            kgm.branch_code = s.BranchCode;
            kgm.Code = s.Code;
            kgm.Description = s.Description;
            kgm.GSTGroupType = s.GSTGroupType;
            kgm.IsActive = s.IsActive;
            kgm.LastModifiedBy = s.LastModifiedBy;
            kgm.LastModifiedOn = SIGlobals.Globals.GetDateTime();
            kgm.SortOrder = s.SortOrder;
            db.GSTGroups.Add(kgm);
            db.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// Edit GSTMaster details
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put/{Code}")]
        [ResponseType(typeof(GSTGroup))]
        public IHttpActionResult Put(string Code, GSTGroupVM s)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            GSTGroup kgm = db.GSTGroups.Where(gst => gst.Code == Code 
                                                && gst.company_code == s.CompanyCode 
                                                && gst.branch_code == s.BranchCode).FirstOrDefault();
            if (kgm == null) {
                return NotFound();
            }
            if (s.Code != kgm.Code) {
                return BadRequest();
            }
            kgm.company_code = s.CompanyCode;
            kgm.branch_code = s.BranchCode;
            kgm.Code = kgm.Code;
            kgm.Description = s.Description;
            kgm.GSTGroupType = s.GSTGroupType;
            kgm.IsActive = s.IsActive;
            kgm.LastModifiedBy = kgm.LastModifiedBy;
            kgm.LastModifiedOn = SIGlobals.Globals.GetDateTime();
            kgm.SortOrder = s.SortOrder;
            db.Entry(kgm).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Ok();
        }

        public IHttpActionResult Put(int id, GSTGroupVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Count(ODataQueryOptions<GSTGroupVM> oDataOptions)
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

        public IQueryable<GSTGroupVM> List()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
