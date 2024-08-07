using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using ProdigyAPI.Utils;
using ProdigyAPI.Providers;
using System.Web.Http.Description;
using System.Reflection;

namespace ProdigyAPI.Controllers
{
    //[Authorize]
    //[PasswordStampCheckAuthorizeAttribute]
    public class ValuesController : ApiController
    {
        // GET api/values
        private IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// For Testing Purpose. Encrypt the data using this API. In the live server this method
        /// will be removed.
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/values/Encrypt")]
        public string EncryptString(string plainText)
        {
            string encryptedString = string.Empty;
            CryptoLibrary crypt = new CryptoLibrary();
            encryptedString = crypt.EncryptWithRijndael(plainText);
            return encryptedString;
        }

        /// <summary>
        /// For Testing Purpose. Decrypt the data using this API. In the live server this method
        /// will be removed.
        /// </summary>
        /// <param name="encryptedText"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/values/Decrypt")]
        public string DecryptString(string encryptedText)
        {
            string plainText = string.Empty;
            CryptoLibrary crypt = new CryptoLibrary();
            plainText = crypt.DecryptWithRijndael(encryptedText);
            return plainText;
        }

        /// <summary>
        /// For Testing Purpose. In the test server since the SMS gateway is not configured, 
        /// there is no way to know the SMS. This API allows you to get the SMS. 
        /// In the live server this method will be removed.
        /// </summary>
        /// <param name="mobileNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/values/otp")]
        public HttpResponseMessage FetchOtp(string mobileNo)
        {
            /*
            using (VrudhiDBEntities entities = new VrudhiDBEntities()) {
                var q = from m in entities.MobileOTPs
                        join p in entities.PortalUsers on m.UserID equals p.ID
                        where p.MobileNo == mobileNo && m.IsUsed == false
                        select new
                        {
                            TypeOfOTp =
                                        (
                                            m.OTPType == 1 ? "New Registration Otp" :
                                            m.OTPType == 2 ? "Reset Password Otp" :
                                            m.OTPType == 3 ? "Map existing Account" :
                                            "Unknown"
                                        ),
                            OTP = m.OTP,
                            GeneratedOn = m.ValidTill
                        };
                if(q != null)
                    return Request.CreateResponse(HttpStatusCode.OK, q.ToList(), JsonMediaTypeFormatter.DefaultMediaType);
                else
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Not found!");
                    
            }
            */
            return Request.CreateResponse(HttpStatusCode.OK, mobileNo, JsonMediaTypeFormatter.DefaultMediaType);
        }

        [HttpGet]
        [Route("api/values/base64encode")]
        [ResponseType(typeof(string))]
        public string Base64Encode(string plainText)
        {
            string encodedText = string.Empty;
            try {
                encodedText = GlobalUtilities.Base64Encode(plainText);
            }
            catch (Exception) {
                return string.Empty;
            }
            return encodedText;
        }

        [HttpGet]
        [Route("api/values/base64decode")]
        [ResponseType(typeof(string))]
        public string Base64Decode(string encodedText)
        {
            string plainText = string.Empty;
            try {
                plainText = GlobalUtilities.Base64Decode(encodedText);
            }
            catch (Exception) {
                return string.Empty;
            }
            return plainText;
        }
        public class ControllerActions
        {
            public string Controller { get; set; }
            public string Action { get; set; }
            public string RouteName { get; set; }
            public string Attributes { get; set; }
            public string ReturnType { get; set; }
        }

        [HttpGet]
        [Route("api/values/contact")]
        public IHttpActionResult GetAllControllerActions()
        {
            Assembly asm = typeof(ValuesController).Assembly;

            var controlleractionlist = asm.GetTypes()
                    .Where(type => typeof(System.Web.Http.ApiController).IsAssignableFrom(type))
                    .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                    .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any())
                    .Select(x => new {
                        Controller = x.DeclaringType.Name,
                        CustomAtrributes = x.GetCustomAttributesData(),
                        Action = x.Name,
                        ReturnType = x.ReturnType.Name,
                        Attributes = String.Join(",", x.GetCustomAttributes().Select(a => a.GetType().Name.Replace("Attribute", "")))
                    })
                    .OrderBy(x => x.Controller).ThenBy(x => x.Action).ToList();
            var list = new List<ControllerActions>();
            foreach (var item in controlleractionlist) {
                string routeName = string.Empty;
                bool routeAdded = false;
                foreach (var ca in item.CustomAtrributes) {
                    if (ca.AttributeType.ToString() == "System.Web.Http.RouteAttribute") {
                        if (ca.ConstructorArguments.Count > 0) {
                            routeName = ca.ConstructorArguments[0].Value.ToString();
                            list.Add(new ControllerActions()
                            {
                                Controller = item.Controller,
                                Action = item.Action,
                                Attributes = item.Attributes,
                                ReturnType = item.ReturnType,
                                RouteName = routeName
                            });
                            routeAdded = true;
                        }
                    }
                }

                if (!routeAdded) {
                    list.Add(new ControllerActions()
                    {
                        Controller = item.Controller,
                        Action = item.Action,
                        Attributes = item.Attributes,
                        ReturnType = item.ReturnType,
                        RouteName = routeName
                    });
                }
            }
            return Ok(list);
        }
    }
}
