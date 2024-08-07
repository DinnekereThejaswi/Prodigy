using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Filters;

namespace ProdigyAPI.Handlers
{
    public class SIExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            HttpStatusCode status = HttpStatusCode.InternalServerError;
            string message = string.Empty;
            var exceptionType = actionExecutedContext.Exception.GetType();
            var exception = actionExecutedContext.Exception;

            if (exceptionType == typeof(UnauthorizedAccessException)) {
                message = FormatMessageToJSON("Access to the Web API is not authorized.");
                status = HttpStatusCode.Unauthorized;
            }
            else if (exceptionType == typeof(DivideByZeroException)) {
                message = FormatMessageToJSON("Divide by zero error.");
                status = HttpStatusCode.BadRequest; ;
            }
            else if (exceptionType == typeof(DbUpdateConcurrencyException)) {
                message = FormatMessageToJSON("Concurrency error has occurred. " + GetInnerExceptionMessage(exception));
                status = HttpStatusCode.BadRequest;
            }
            else if(exceptionType == typeof(DbUpdateException)) {
                message = FormatMessageToJSON(GetSqlServerExceptionMessage(exception));
                status = HttpStatusCode.BadRequest;
            }
            else {
                message = FormatMessageToJSON("Unhandled exception. " + GetInnerExceptionMessage(exception));
                status = HttpStatusCode.NotFound;
            }

            actionExecutedContext.Response = new HttpResponseMessage()
            {
                Content = new StringContent(message, System.Text.Encoding.UTF8, "application/json"),
                StatusCode = status
            };

            base.OnException(actionExecutedContext);

        }

        private string GetInnerExceptionMessage(Exception ex)
        {
            string exceptionMessage = string.Empty;
            if(ex.InnerException != null && ex.InnerException.InnerException != null) {
                exceptionMessage = ex.InnerException.InnerException.Message;
            }
            else {
                exceptionMessage = ex.Message;
            }
            return exceptionMessage;
        }

        private string GetSqlServerExceptionMessage(Exception exception)
        {
            string exceptionMessage = exception.Message;

            if (exception.InnerException != null && exception.InnerException.InnerException != null) {
                exceptionMessage = exception.InnerException.InnerException.Message;
                if (exception.InnerException.InnerException is System.Data.SqlClient.SqlException) {
                    var sqlException = exception.InnerException.InnerException as System.Data.SqlClient.SqlException;
                    switch (sqlException.Number) {
                        case 2627:  // Unique constraint error
                            exceptionMessage = "Unique constraint error. " + sqlException.Message;
                            break;
                        case 547:   // Constraint check violation
                            exceptionMessage = "Constraint check violation. " + sqlException.Message;
                            break;
                        case 2601:  // Duplicated key error
                            exceptionMessage = "Duplicate key error. " + sqlException.Message;
                            break;
                    }
                }
            }
            
            return exceptionMessage;
        }

        private string FormatMessageToJSON(string messageDetail)
        {
            messageDetail = messageDetail.Replace('"', '`');
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.AppendLine().Append('"').Append("Message").Append('"').Append(": ");
            sb.Append('"').Append(messageDetail).Append('"').AppendLine();
            sb.Append("}");            
            JObject json = JObject.Parse(sb.ToString());
            return json.ToString();
        }
    }
}