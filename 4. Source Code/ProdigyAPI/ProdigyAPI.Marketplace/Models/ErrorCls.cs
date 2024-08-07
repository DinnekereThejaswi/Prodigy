using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.Marketplace.Models
{
    public class clsError
    {
        public bool blnError { get; set; }
        public string ErrorMsg { get; set; }
        public string InnerException { get; set; }
    }
    public class ErrorCls
    {
        public int statusCode { get; set; }
        public ErrorInfoCls errorInfo { get; set; }

    }
    public class ErrorInfoCls
    {
        public string errorCode { get; set; }
        public string description { get; set; }
        public string errorDescription { get; set; }
        public bool retryable { get; set; }
    }

    public class ErrorInfo
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ErrorCode { get; set; }
        public string Description { get; set; }
        public ErrorInfo()
        {
            StatusCode = HttpStatusCode.BadRequest;
            ErrorCode = string.Empty;
        }
    }
}
