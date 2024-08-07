using Newtonsoft.Json;
using ProdigyAPI.BL.ViewModel.Error;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace ProdigyAPI.Utils
{
    /// <summary>
    /// This is Controller level Exception Handling class.
    /// </summary>
    public class ExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            Exception excp = actionExecutedContext.Exception;
            ErrorVM error = new ErrorVM().GetErrorDetails(excp);
            StringContent strContent = new StringContent(JsonConvert.SerializeObject(error));

            //actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.BadRequest,error);

            //throw new HttpResponseException(new HttpResponseMessage(error.ErrorStatusCode)
            //{
            //    Content = strContent
            //});


            //if (actionExecutedContext.Exception is NotImplementedException) {
            //    actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            //}

            actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(error.ErrorStatusCode, error.description);
        }
    }

    /// <summary>
    /// Custom Model Level Validation.
    /// </summary>
    public class BranchAttribute : ValidationAttribute
    {
        /// <summary>
        /// Checking Valid Branch or Not
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string branch = (string)value;
            if (SIGlobals.Globals.IsValidBranch(branch)) {
                return ValidationResult.Success;
            }
            else {
                return new ValidationResult("Invalid Branch");
            }
        }
    }
}