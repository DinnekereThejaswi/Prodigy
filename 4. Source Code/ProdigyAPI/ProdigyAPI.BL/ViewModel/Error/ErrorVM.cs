using ProdigyAPI.BL.BusinessLayer.ErrorHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Error
{
    public class ErrorVM
    {
        #region Declaration
        public int? index { get; set; }
        public string field { get; set; }
        public string description { get; set; }
        public string customDescription { get; set; }
        public HttpStatusCode ErrorStatusCode { get; set; }
        public string InnerException { get; set; }
        public decimal Value { get; set; }
        public dynamic GenObject { get; set; }
        public List<ModelErrorVM> ModelErrors { get; set; } 
     
        public ErrorVM()
        {
            ErrorStatusCode = HttpStatusCode.BadRequest;
        }
        public ErrorVM(System.Exception excp)
        {
            var exMgr = new ExceptionErrorHandler().ManageException(excp);
            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest;
            description = exMgr.ToString();
            customDescription = exMgr.FullErrorDetail();
        }
        #endregion

        #region Methods
        public ErrorVM GetErrorDetails(System.Exception excp, System.Net.HttpStatusCode statusCode)
        {
            var exMgr = new ExceptionErrorHandler().ManageException(excp);
            return new ErrorVM()
            {
                ErrorStatusCode = statusCode,
                description = exMgr.ToString(),
                customDescription = exMgr.FullErrorDetail()
            };
        }


        public ErrorVM GetErrorDetails(System.Exception excp)
        {
            var exMgr = new ExceptionErrorHandler().ManageException(excp);
            return new ErrorVM()
            {
                ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError,
                description = exMgr.ToString(),
                customDescription = exMgr.FullErrorDetail()
            };
        }
        #endregion
    }


    public class InnerExceptionVM
    {
        public string ErrorCode { get; set; }
        public int LineNumber { get; set; }
        public string Message { get; set; }
        public int Number { get; set; }
        public string Server { get; set; }
        public string Source { get; set; }
    }

    public class ModelErrorVM
    {
        public string Field { get; set; }
        public string Message { get; set; }
    }
}
