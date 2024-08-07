using Microsoft.IdentityModel.Tokens;
using ProdigyAPI.Model.MagnaDb;
using ProdigyAPI.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Linq;
using System.Web.Http;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Web.Http.Cors;

namespace ProdigyAPI.Controllers.Auth
{
    [RoutePrefix("auth")]
    public class AuthController : ApiController
    {
        MagnaDbEntities db = new MagnaDbEntities();

        [HttpPost]
        [Route("token")]
        [AllowAnonymous]
        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        //[EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials =true, PreflightMaxAge =86400)]
        public HttpResponseMessage GenerateAuthToken([FromBody] TokenRequest tokenRequestBody)
        {
            string password = GlobalUtilities.Base64Decode(tokenRequestBody.Password);
            if (string.IsNullOrEmpty(password)) {
                return ThrowUnauthorizedResponse();
            }

            string hashedPassword = GlobalUtilities.GetHashcode(password);
            if (string.IsNullOrEmpty(hashedPassword)) {
                return ThrowUnauthorizedResponse();
            }

            var client = db.APIClients.Where(c => c.ClientCode == tokenRequestBody.ClientID
                && c.ClientSecret == tokenRequestBody.ClientSecret).FirstOrDefault();
            if (client == null) {
                return ThrowUnauthorizedResponse();
            }

            var user = db.SDTU_OPERATOR_MASTER.Where(u => u.OperatorCode == tokenRequestBody.UserID &&
                u.Password3 == hashedPassword && u.object_status == "O").FirstOrDefault();
            if (user == null) {
                return ThrowUnauthorizedResponse();
            }

            #region IP Address Validation
            string ipAddressError = string.Empty;
            IPAddress4Validation ipValidator = new IPAddress4Validation();
            string ipAddress = ipValidator.GetClientIPAddress(Request);
            if (!ipValidator.ValidateIPAddress(ipAddress, out ipAddressError)) {
                var error = new ErrorInfo
                {
                    ErrorCode = "SI0003",
                    Description = ipAddressError,
                    ErrorDescription = ipAddressError
                };
                HttpResponseMessage responseMsg = new HttpResponseMessage(HttpStatusCode.Forbidden);
                responseMsg.Content = new StringContent(JsonConvert.SerializeObject(error), Encoding.UTF8, "application/json");
                return responseMsg;
            } 
            #endregion

            TokenResponse token = CreateJwt(tokenRequestBody, user);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(token), Encoding.UTF8, "application/json")
            };
        }

        private static HttpResponseMessage ThrowUnauthorizedResponse()
        {
            var error = new ErrorInfo
            {
                ErrorCode = "SI0001",
                Description = "Invalid login Credentials",
                ErrorDescription = "Invalid login Credentials"
            };
            HttpResponseMessage responseMsg = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            responseMsg.Content = new StringContent(JsonConvert.SerializeObject(error), Encoding.UTF8, "application/json");
            return responseMsg;
        }

        [HttpPost]
        [Route("refresh-token")]
        public HttpResponseMessage GenerateRefreshToken(RefreshTokenRequest refreshTokenRequestBody)
        {
            ErrorInfo error = null;
            var tokenReponse = GenerateRefresh(refreshTokenRequestBody, out error);
            if (tokenReponse == null) {
                HttpResponseMessage responseMsg = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                responseMsg.Content = new StringContent(JsonConvert.SerializeObject(error), Encoding.UTF8, "application/json");
                return responseMsg;
            }
            else {
                HttpResponseMessage responseMsg = new HttpResponseMessage(HttpStatusCode.OK);
                responseMsg.Content = new StringContent(JsonConvert.SerializeObject(tokenReponse), Encoding.UTF8, "application/json");
                return responseMsg;
            }
        }

        private TokenResponse CreateJwt(TokenRequest tokenRequestBody, SDTU_OPERATOR_MASTER user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, tokenRequestBody.UserID.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, tokenRequestBody.UserID),
                new Claim(JwtRegisteredClaimNames.Jti, user.OperatorType),
                new Claim("UserID", user.OperatorCode.ToString()),
                new Claim("RoleID", user.OperatorRole.ToString()),
                new Claim("PwdStamp", user.PasswordSalt.ToString()),
                new Claim("RowTimestamp", user.RowTimestamp.ToString()),
                new Claim("CompanyCode", user.company_code.ToString()),
                new Claim("BranchCode", user.branch_code.ToString()),
                new Claim("ClientID", tokenRequestBody.ClientID),
                //new Claim(_options.ClaimsIdentity.UserIdClaimType, user.MobileNo),
                new Claim(ClaimTypes.Name, tokenRequestBody.UserID)
            });

            string jwtKey = ConfigurationManager.AppSettings["JwtKey"].ToString();
            string jwtIssuer = ConfigurationManager.AppSettings["JwtIssuer"].ToString();
            int accessTokenLifespanSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["AccessTokenLifespanSeconds"]);

            DateTime tokenIssueDate = DateTime.UtcNow;
            var tokenExpiry = tokenIssueDate.AddSeconds(accessTokenLifespanSeconds);

            var securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(jwtKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var token = (JwtSecurityToken)tokenHandler.CreateJwtSecurityToken(
                        issuer: jwtIssuer,
                        audience: jwtIssuer,
                        subject: claimsIdentity,
                        notBefore: tokenIssueDate,
                        expires: tokenExpiry,
                        signingCredentials: signingCredentials);
            var tokenString = tokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();
            
            TokenResponse response = new TokenResponse
            {
                Credentials = new TokenCredentials
                {
                    Token = tokenString,
                    RefreshToken = refreshToken,
                    UserID = user.OperatorCode,
                    Expiration = tokenExpiry,
                    TokenType = "bearer",
                    ExpiresIn = accessTokenLifespanSeconds - 1
                },
                ClientInfomation = new ClientInfo
                {
                    ClientID = tokenRequestBody.ClientID
                }
            };
            PersistRefreshToken(response, tokenIssueDate);
            return response;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create()) {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        void PersistRefreshToken(TokenResponse tokenResponse, DateTime dateTime)
        {
            var apiCleint = db.APIClients.Where(c => c.ClientCode == tokenResponse.ClientInfomation.ClientID).FirstOrDefault();
            var token = new RefreshToken
            {
                Token = tokenResponse.Credentials.RefreshToken,
                ClientID = apiCleint.ID,
                UserCode = tokenResponse.Credentials.UserID,
                IssuedDate = dateTime,
                ExpiryDate = dateTime.AddSeconds(apiCleint.RefreshTokenLifeSpan),
                LastModifiedOn = DateTime.Now
            };
            db.RefreshTokens.Add(token);
            db.SaveChanges();
        }

        void DeleteRefreshToken(string refreshToken, string userID)
        {
            var token = db.RefreshTokens.Where(c => c.Token == refreshToken
                && c.UserCode == userID).FirstOrDefault();
            db.RefreshTokens.Remove(token);
            db.SaveChanges();
        }

        private TokenResponse GenerateRefresh(RefreshTokenRequest refreshTokenRequestBody, out ErrorInfo error)
        {
            error = null;
            TokenResponse response = null;
            try {
                string jwtKey = ConfigurationManager.AppSettings["JwtKey"].ToString();
                string jwtIssuer = ConfigurationManager.AppSettings["JwtIssuer"].ToString();
                int accessTokenLifespanSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["AccessTokenLifespanSeconds"]);

                //check the authenticity of the auth token
                var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(refreshTokenRequestBody.AuthToken,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = securitykey,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtIssuer,
                        ValidateLifetime = false
                    }, out validatedToken
                );

                var jwtToken = validatedToken as JwtSecurityToken;
                if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256)) {
                    error = new ErrorInfo
                    {
                        ErrorCode = "SI0002",
                        Description = "Invalid auth token",
                        ErrorDescription = "Invalid auth token"
                    };
                    return null;
                }
                var userName = principal.Identity.Name;

                //Validate refresh token now
                var refreshTokenObj = db.RefreshTokens.Where(r => r.Token == refreshTokenRequestBody.RefreshToken
                    && r.UserCode == userName && DateTime.UtcNow <= r.ExpiryDate).FirstOrDefault();
                if (refreshTokenObj == null) {
                    error = new ErrorInfo
                    {
                        ErrorCode = "SI0002",
                        Description = "Invalid/expired refresh token.",
                        ErrorDescription = "Invalid/expired refresh token."
                    };
                    return null;
                }

                #region IP Address Validation
                string ipAddressError = string.Empty;
                IPAddress4Validation ipValidator = new IPAddress4Validation();
                string ipAddress = ipValidator.GetClientIPAddress(Request);
                if (!ipValidator.ValidateIPAddress(ipAddress, out ipAddressError)) {
                    error = new ErrorInfo
                    {
                        ErrorCode = "SI0003",
                        Description = ipAddressError,
                        ErrorDescription = ipAddressError
                    };
                    HttpResponseMessage responseMsg = new HttpResponseMessage(HttpStatusCode.Forbidden);
                    responseMsg.Content = new StringContent(JsonConvert.SerializeObject(error), Encoding.UTF8, "application/json");
                    return null;
                }
                #endregion

                var claims = principal.Claims;
                var clientID = claims.Where(c => c.Type == "ClientID").FirstOrDefault().Value;

                var tokenIssueDate = DateTime.UtcNow;
                var tokenExpiry = tokenIssueDate.AddSeconds(accessTokenLifespanSeconds);
                var token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: jwtIssuer,
                    claims: claims,
                    expires: tokenExpiry,
                    signingCredentials: credentials);

                var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);
                var refreshToken = GenerateRefreshToken();
                response = new TokenResponse
                {
                    Credentials = new TokenCredentials
                    {
                        Token = encodedToken,
                        RefreshToken = refreshToken,
                        UserID = userName,
                        Expiration = tokenExpiry,
                        TokenType = "bearer",
                        ExpiresIn = accessTokenLifespanSeconds -1
                    },
                    ClientInfomation = new ClientInfo
                    {
                        ClientID = clientID
                    }
                };
                DeleteRefreshToken(refreshTokenRequestBody.RefreshToken, userName);
                PersistRefreshToken(response, tokenIssueDate);
            }
            catch (Exception ex) {
                error = new ErrorInfo
                {
                    ErrorCode = "SI0002",
                    Description = "Failed to generate refresh token.",
                    ErrorDescription = ex.Message
                };
                return null;
            }

            return response;
        }
    }
}
