using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ProdigyAPI.Utils;

namespace ProdigyAPI.Handlers
{
    public class RequestAndResponseLoggingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // log request body
            DateTime requestTimeStamp = DateTime.Now;
            string requestBody = await request.Content.ReadAsStringAsync();
 
            GlobalUtilities.TraceLine(@"{ RequestBody: ""REQUEST BODY ==> ""}");
            GlobalUtilities.TraceLine(requestBody);

            // let other handlers process the request
            var result = await base.SendAsync(request, cancellationToken);
            DateTime responseTimeStamp = DateTime.Now;
            string responseBody = string.Empty;
            if (result.Content != null) {
                // once response body is ready, log it
                try {
                    var respBody = await result.Content.ReadAsStringAsync();
                    responseBody = respBody;
                }
                catch (Exception ex) {
                    responseBody = "Unable to log the request due to error: " + ex.Message;
                }
                GlobalUtilities.TraceLine(responseBody);
            }
            ProdigyAPI.Utils.GlobalUtilities globals = new Utils.GlobalUtilities();
            globals.WriteAPITraceToDatabase(request, requestBody, requestTimeStamp, result, responseBody, responseTimeStamp);
            return result;
        }
    }
}

namespace System.Web.Http
{
    //The following extension class is un-used right now. It is reserved for future.
    public static class HttpRequestMessageExtensions
    {
        public static Dictionary<string, string> GetQueryStrings(this HttpRequestMessage request)
        {
            return request.GetQueryNameValuePairs()
                          .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
        }
        
        public static string GetQueryString(this HttpRequestMessage request, string key)
        {
            // IEnumerable<KeyValuePair<string,string>> - right!
            var queryStrings = request.GetQueryNameValuePairs();
            if (queryStrings == null)
                return null;

            var match = queryStrings.FirstOrDefault(kv => string.Compare(kv.Key, key, true) == 0);
            if (string.IsNullOrEmpty(match.Value))
                return null;

            return match.Value;
        }
        
        public static string GetHeader(this HttpRequestMessage request, string key)
        {
            IEnumerable<string> keys = null;
            if (!request.Headers.TryGetValues(key, out keys))
                return null;

            return keys.First();
        }
        
        public static string GetCookie(this HttpRequestMessage request, string cookieName)
        {
            CookieHeaderValue cookie = request.Headers.GetCookies(cookieName).FirstOrDefault();
            if (cookie != null)
                return cookie[cookieName].Value;

            return null;
        }
    }
}