using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http.Cors;

using System;
using Microsoft.Owin;
using ProdigyAPI.Utils;
using ProdigyAPI.Model.MagnaDb;

namespace ProdigyAPI.Providers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AccessTokenBasedAuthProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            /*
            string clientID;
            string clientSecret;

            if (!context.TryGetBasicCredentials(out clientID, out clientSecret))
            {
                context.TryGetFormCredentials(out clientID, out clientSecret);
            }
            if (clientID == null)
            {
                context.SetError("invalid_client", "Client credentials could not be retrieved from the Authorization header");
                context.Rejected();
                return;
            }

            //Get additional paramters if required
            //string uid = context.Parameters.Where(f => f.Key == "uid").Select(f => f.Value).SingleOrDefault()[0];
            context.Validated();
            */
            string clientId = string.Empty;
            string clientSecret = string.Empty;
            APIClient client = null;

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret)) {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null) {
                //Remove the comments from the below line context.SetError, and invalidate context 
                //if you want to force sending clientId/secrects once obtain access tokens. 
                context.Validated();
                context.SetError("invalid_clientId", "ClientId should be provided.");
                return Task.FromResult<object>(null);
            }

            using (var db = new MagnaDbEntities()) {
                client = db.APIClients.Where(c => c.ClientCode == context.ClientId).FirstOrDefault();
            }

            if (client == null) {
                context.SetError("invalid_clientId", string.Format("Client '{0}' is not registered in the system.", context.ClientId));
                return Task.FromResult<object>(null);
            }

            bool checkClientSecret = true; //client.CheckClientSecret
            if (checkClientSecret) {
                if (string.IsNullOrWhiteSpace(clientSecret)) {
                    context.SetError("invalid_clientId", "Client secret should be sent.");
                    return Task.FromResult<object>(null);
                }
                else {
                    if (client.ClientSecret != clientSecret/*SIGlobals.Globals.GetHash(clientSecret)*/) {
                        context.SetError("invalid_clientId", "Client secret is invalid.");
                        return Task.FromResult<object>(null);
                    }
                }
            }

            if (!client.Active) {
                context.SetError("invalid_clientId", "Client is inactive.");
                return Task.FromResult<object>(null);
            }

            context.OwinContext.Set<string>("allowedClientOrigin", client.AllowedOrigin);
            context.OwinContext.Set<string>("clientRefreshTokenLifeTime", client.RefreshTokenLifeSpan.ToString());

            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //var allowedOrigin = context.OwinContext.Get<string>("allowedClientOrigin");
            //if (allowedOrigin == null)
            //    allowedOrigin = "*";
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });
            string hashedPassword = GlobalUtilities.GetHashcode(context.Password.ToUpper());
            if (string.IsNullOrEmpty(hashedPassword)) {
                context.SetError("invalid_grant", "Provided username or password is incorrect.");
                context.Response.Headers.Add("AuthorizationResponse", new[] { "Failed" });
                return;
            }

            try {
                SDTU_OPERATOR_MASTER user = null;
                using (MagnaDbEntities _repo = new MagnaDbEntities()) {
                    user = _repo.SDTU_OPERATOR_MASTER.ToList().Where(u => u.OperatorCode == context.UserName.ToUpper() && u.Password3 == hashedPassword).FirstOrDefault();
                    if (user == null) {
                        context.SetError("invalid_grant", "The user name or password is incorrect.");
                        return;
                    }
                }

                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                var claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.Name, user.OperatorCode),
                            new Claim("UserID", user.OperatorCode.ToString()),
                            new Claim("RoleID", user.OperatorRole.ToString()),
                            new Claim("PwdStamp", user.PasswordSalt.ToString()),
                            new Claim("RowTimestamp", user.RowTimestamp.ToString()),
                            new Claim("CompanyCode", user.company_code.ToString()),
                            new Claim("BranchCode", user.branch_code.ToString())
                        };
                identity.AddClaims(claims);
                var props = new AuthenticationProperties(new Dictionary<string, string>
                        {
                            {"client_id", (context.ClientId == null) ? string.Empty : context.ClientId},
                            {"userdisplayname", user.OperatorName},
                            {"role", "admin"}
                            //,{"roleID", user.OperatorRole.ToString()}
                        });

                //context.Options.AccessTokenExpireTimeSpan = DateTime.Today.AddDays(1).Subtract(DateTime.Now);
                //context.Options.AccessTokenExpireTimeSpan = TimeSpan.FromHours(4);
                var ticket = new AuthenticationTicket(identity, props);
                context.Validated(ticket);
            }
            catch (Exception) {
                context.SetError("invalid_grant", "Provided username or password is incorrect or authentication failed.");
                context.Response.Headers.Add("AuthorizationResponse", new[] { "Failed" });
            }

        }

        //public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        //{
        //    var identity = new ClaimsIdentity(context.Options.AuthenticationType);
        //    //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

        //    try {
        //        string errorMsg = string.Empty;
        //        string hashedPassword = GlobalUtilities.GetHashcode(context.Password.ToUpper());
        //        if (string.IsNullOrEmpty(hashedPassword)) {
        //            context.SetError("invalid_grant", "Provided username or password is incorrect.");
        //            context.Response.Headers.Add("AuthorizationResponse", new[] { "Failed" });
        //            return;
        //        }
        //        using (var db = new MagnaDbEntities()) {
        //            var user = db.SDTU_OPERATOR_MASTER.ToList().Where(u => u.OperatorCode == context.UserName.ToUpper() && u.Password3 == hashedPassword).FirstOrDefault();
        //            if (user != null) {
        //                if (!string.IsNullOrEmpty(user.OperatorCode)) {
        //                    var claims = new List<Claim>()
        //                            {
        //                                new Claim(ClaimTypes.Name, user.OperatorCode),
        //                                new Claim("UserID", user.OperatorCode.ToString()),
        //                                new Claim("RoleID", user.OperatorRole.ToString()),
        //                                new Claim("PwdStamp", user.PasswordSalt.ToString()),
        //                                new Claim("RowTimestamp", user.RowTimestamp.ToString()),
        //                                new Claim("CompanyCode", user.company_code.ToString()),
        //                                new Claim("BranchCode", user.branch_code.ToString())
        //                            };
        //                    identity.AddClaims(claims);
        //                    var props = new AuthenticationProperties(new Dictionary<string, string>
        //                            {
        //                                {"userdisplayname", user.OperatorName},
        //                                {"role", "admin"},
        //                                {"roleID", user.OperatorRole.ToString()}
        //                             });

        //                    //context.Options.AccessTokenExpireTimeSpan = DateTime.Today.AddDays(1).Subtract(DateTime.Now);
        //                    context.Options.AccessTokenExpireTimeSpan = TimeSpan.FromHours(4);
        //                    var ticket = new AuthenticationTicket(identity, props);
        //                    context.Validated(ticket);

        //                }
        //                else {
        //                    context.SetError("invalid_grant", "Provided username or password is incorrect");
        //                    context.Response.Headers.Add("AuthorizationResponse", new[] { "Failed" });
        //                    //context.Rejected();
        //                }
        //            }
        //            else {
        //                context.SetError("invalid_grant", "Provided username or password is incorrect");
        //                context.Response.Headers.Add("AuthorizationResponse", new[] { "Failed" });
        //                //context.Rejected();
        //            }
        //            return;
        //        }

        //    }
        //    catch (Exception) {
        //        context.SetError("invalid_grant", "Provided username and password is incorrect or error");
        //        context.Response.Headers.Add("AuthorizationResponse", new[] { "Failed" });
        //    }
        //}

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["client_id"];
            var currentClient = context.ClientId;

            if (originalClient != currentClient) {
                context.SetError("invalid_clientId", "Refresh token is issued to a different Client.");
                return Task.FromResult<object>(null);
            }

            // Change auth ticket for refresh token requests
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
            newIdentity.AddClaim(new Claim("newClaim", "newValue"));

            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);

            return Task.FromResult<object>(null);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary) {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
            return Task.FromResult<object>(null);
        }
    }
}