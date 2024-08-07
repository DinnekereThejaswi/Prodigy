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
    [Authorize]
    [RoutePrefix("api/Master/ItemSize")]
    public class ItemSizeController : SIBaseApiController<ItemSizeVM>, IBaseMasterActionController<ItemSizeVM, ItemSizeVM>
    {
        #region Declaration
        MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Controller Methods

        /// <summary>
        /// List Of ItemSize Details
        /// </summary>
        /// <param name="CategoryCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("List/{CategoryCode}")]
        public IQueryable<ItemSizeVM> listofitemsizedetails(string CategoryCode)
        {
            return db.KSTS_ITEMSIZE_MASTER.Where(d => d.category == CategoryCode)
              .Select(s => new ItemSizeVM
              {
                  ObjID = s.obj_id,
                  CompanyCode = s.company_code,
                  BranchCode = s.branch_code,
                  Category = s.category,
                  CategoryName = s.categoryName,
                  ItemCode = s.item_code,
                  ItemName = s.item_name,
                  UniqRowID = s.UniqRowID,
                  ObjectStatus = s.obj_status,
                  UpdateOn = s.UpdateOn
              }).OrderBy(c => c.CategoryName);
        }

        /// <summary>
        /// Save ItemSize Details
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        [ResponseType(typeof(ItemSizeVM))]
        public IHttpActionResult Post(ItemSizeVM s)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                KSTS_ITEMSIZE_MASTER kim = new KSTS_ITEMSIZE_MASTER();
                kim.obj_id = Common.GetNewGUID();
                kim.company_code = Common.CompanyCode;
                kim.branch_code = Common.BranchCode;
                kim.categoryName = s.CategoryName;
                kim.category = s.Category;
                kim.item_code = s.ItemCode;
                kim.item_name = s.ItemName;
                kim.UniqRowID = Guid.NewGuid();
                kim.obj_status = s.ObjectStatus;
                kim.UpdateOn = Framework.Common.GetDateTime();
                db.KSTS_ITEMSIZE_MASTER.Add(kim);
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
        /// Edit ItemSize Details
        /// </summary>
        /// <param name="ObjID"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put/{ObjID}")]
        [ResponseType(typeof(ItemSizeVM))]
        public IHttpActionResult Put(string ObjID, ItemSizeVM s)
        {
            KSTS_ITEMSIZE_MASTER kim = db.KSTS_ITEMSIZE_MASTER.Where(d => d.obj_id == ObjID).FirstOrDefault();
            if (kim == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (s.ObjID != kim.obj_id)
            {
                return BadRequest();
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                kim.obj_id = kim.obj_id;
                kim.company_code = Common.CompanyCode;
                kim.branch_code = Common.BranchCode;
                kim.categoryName = kim.categoryName;
                kim.category = kim.category;
                kim.item_code = kim.item_code;
                kim.item_name = s.ItemName;
                kim.UniqRowID = kim.UniqRowID;
                kim.obj_status = "O";
                kim.UpdateOn = kim.UpdateOn;
                db.Entry(kim).State = System.Data.Entity.EntityState.Modified;
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
        /// Alter ObjectStatus Of ItemSize Details
        /// </summary>
        /// <param name="ObjID"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("OpenOrClose/{ObjID}")]
        [ResponseType(typeof(ItemSizeVM))]
        public IHttpActionResult putOpenOrClose(string ObjID)
        {
            KSTS_ITEMSIZE_MASTER kim = db.KSTS_ITEMSIZE_MASTER.Where(d => d.obj_id == ObjID).FirstOrDefault();
            if (kim == null)
            {
                return NotFound();
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                kim.obj_id = kim.obj_id;
                kim.obj_status = kim.obj_status == "O" ? "C" : "O";
                db.Entry(kim).State = System.Data.Entity.EntityState.Modified;
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

        public IQueryable<ItemSizeVM> List()
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Count(ODataQueryOptions<ItemSizeVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, ItemSizeVM t)
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
