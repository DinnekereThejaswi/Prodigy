﻿using ProdigyAPI.BL.BusinessLayer.UserManagement;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.UserManagement;
using ProdigyAPI.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace ProdigyAPI.Controllers.UserManagement
{
    /// <summary>
    /// Set of APIs for application main module.
    /// </summary>
    [RoutePrefix("api/rights-management/main-modules")]
    [Authorize]
    public class MainModulesController : SIBaseApiController<ModuleMasterBL>
    {
        /// <summary>
        /// Gets the list of main modules
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        [Route("list/{companyCode}/{branchCode}")]
        [ResponseType(typeof(List<MainMenuViewModel>))]
        public IHttpActionResult List(string companyCode, string branchCode)
        {
            ErrorVM error = null;
            var data = new ModuleMasterBL().List(companyCode, branchCode, out error);
            if (data != null)
                return Ok(data);
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Gets a main module
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="id">The ID of the module</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get")]
        [Route("get/{companyCode}/{branchCode}/{id}")]
        [ResponseType(typeof(MainMenuViewModel))]
        public IHttpActionResult Get(string companyCode, string branchCode, int id)
        {
            ErrorVM error = null;
            var role = new ModuleMasterBL().Get(companyCode, branchCode, id, out error);
            if (role != null)
                return Ok(role);
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Posts a new module
        /// The allowed values for Status is one of these: Active or Closed
        /// </summary>
        /// <param name="item">Object to be psted</param>
        /// <returns></returns>
        [HttpPost]
        [Route("post")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult Post(MainMenuViewModel item)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            bool success = new ModuleMasterBL().Add(item, userID, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = item.Name,
                    Message = "New record " + item.Name + " has been created successfully."
                };
                return Ok(docCreation);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// Updates an existing record.
        /// Only active items can be modified.
        /// </summary>
        /// <param name="item">Object data to be modified</param>
        /// <returns></returns>
        [HttpPut]
        [Route("put")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult Put(MainMenuViewModel item)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            bool success = new ModuleMasterBL().Modify(item, userID, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = item.Name,
                    Message = "The record " + item.Name + " has been modified successfully."
                };
                return Ok(docCreation);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// Activate/opens a module. 
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="id">The ID which of the module you want to update.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("activate")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult Activate(string companyCode, string branchCode, int id)
        {
            ErrorVM error = null;
            string authUser = base.GetUserId();
            bool success = new ModuleMasterBL().Open(companyCode, branchCode, id, authUser, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = id.ToString(),
                    Message = "The record " + id + " has been activated successfully."
                };
                return Ok(docCreation);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// De-Activates/closes a module. 
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="id">The ID which of the module you want to update.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("de-activate")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult DeActivate(string companyCode, string branchCode, int id)
        {
            ErrorVM error = null;
            string authUser = base.GetUserId();
            bool success = new ModuleMasterBL().Close(companyCode, branchCode, id, authUser, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = id.ToString(),
                    Message = "The record " + id + " has been de-activated successfully."
                };
                return Ok(docCreation);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }
    }
}