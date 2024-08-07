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
using System.Web.Http.OData.Query;
using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.BL.ViewModel.Error;

namespace ProdigyAPI.Controllers.Masters
{
    /// <summary>
    /// This Provides API's realated to Product Tree.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Masters/ProductTree")]
    public class ProductTreeController : SIBaseApiController<ProductTreeVM>, IBaseMasterActionController<ProductTreeVM, ProductTreeVM>
    {
        #region Core Controller Methods
        public IQueryable<ProductTreeVM> List()
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, ProductTreeVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Post(ProductTreeVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Count(ODataQueryOptions<ProductTreeVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Delete([FromBody] int id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Supplymentary Controller Methods
        ///// <summary>
        ///// Get All Product Tree information.
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("Get/{companyCode}/{branchCode}")]
        //public IHttpActionResult GetAllProductTreeDetails(string companyCode, string branchCode)
        //{
        //    List<ProductTreeVM> lstOfItemLevel = new ProductTreeBL().GetAllProductTreeDetails(companyCode, branchCode);
        //    return Ok(lstOfItemLevel);
        //}


        /// <summary>
        /// Get All Product Tree information.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Get/{companyCode}/{branchCode}")]
        public IHttpActionResult GetAllProductTreeDetails(string companyCode, string branchCode)
        {
            List<ProductTreeVM> lstOfItemLevel = new ProductTreeBL().GetAllProductTreeDetails2(companyCode, branchCode);
            return Ok(lstOfItemLevel);
        }

        /// <summary>
        /// Save Product Tree Details
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult Save(ProductTreeVM product)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            ErrorVM error = new ErrorVM();
            bool saved = new ProductTreeBL().SaveProduct(product, out error);
            if (error == null) {
                return Ok();
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Update Product Details.
        /// </summary>
        /// <param name="objID"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put/{objID}")]
        public IHttpActionResult Update(string objID, ProductTreeVM product)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            ErrorVM error = new ErrorVM();
            bool saved = new ProductTreeBL().Update(objID, product, out error);
            if (error == null) {
                return Ok();
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Delete Product Details.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Delete")]
        public IHttpActionResult Delete(ProductTreeVM product)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            bool saved = new ProductTreeBL().Delete(product.ObjID, product.CompanyCode, product.BranchCode, out error);
            if (error == null) {
                return Ok();
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Delete Product Details.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="id"></param>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("DeleteByID/{level}/{id}/{companyCode}/{branchCode}")]
        public IHttpActionResult DeleteByID(int level, int id, string companyCode, string branchCode)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            bool saved = new ProductTreeBL().Delete(level, id, companyCode, branchCode, out error);
            if (error == null) {
                return Ok();
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }
        #endregion
    }
}
