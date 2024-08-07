using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using System;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Infrastructure;
using System.Collections.Concurrent;
using ProdigyAPI.Model.MagnaDb;
using System.Security.Cryptography;

namespace ProdigyAPI.Providers
{
    public class OAuthRefreshTokenProvider : IAuthenticationTokenProvider
    {
        private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens 
            = new ConcurrentDictionary<string, AuthenticationTicket>();
        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            //var allowedOrigin = context.OwinContext.Get<string>("allowedClientOrigin");
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            string hashedTokenId = SIGlobals.Globals.ComputeHash(context.Token);

            try {
                using (var db = new MagnaDbEntities()) {
                    var refreshToken = db.RefreshTokens.Where(r => r.Token == hashedTokenId).FirstOrDefault();

                    if (refreshToken != null) {
                        //Get protectedTicket from refreshToken class
                        context.DeserializeTicket(refreshToken.ProtectedTicket);
                        var result = db.RefreshTokens.Remove(refreshToken);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex) {
                throw(ex);
            }
        }

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var clientID = context.Ticket.Properties.Dictionary["client_id"];

            if (string.IsNullOrEmpty(clientID)) {
                return;
            }

            var refreshTokenId = Guid.NewGuid().ToString("n");

            using (var db = new MagnaDbEntities()) {
                var refreshTokenLifeTime = context.OwinContext.Get<string>("clientRefreshTokenLifeTime");
                double refreshTokenSpan = 1200;//20 minutes
                double.TryParse(refreshTokenLifeTime, out refreshTokenSpan);

                var apiCleint = db.APIClients.Where(c => c.ClientCode == clientID).FirstOrDefault();
                var utcDateNow = DateTime.UtcNow;
                var token = new RefreshToken
                {
                    Token = SIGlobals.Globals.ComputeHash(refreshTokenId),
                    APIClient = apiCleint,
                    UserCode = context.Ticket.Identity.Name,
                    IssuedDate = utcDateNow,
                    ExpiryDate = utcDateNow.AddSeconds(refreshTokenSpan),
                    LastModifiedOn = DateTime.Now
                };

                context.Ticket.Properties.IssuedUtc = token.IssuedDate;
                context.Ticket.Properties.ExpiresUtc = token.ExpiryDate;

                token.ProtectedTicket = context.SerializeTicket();

                try {
                    var result = db.RefreshTokens.Add(token);
                    db.SaveChanges();
                    if (result != null) {
                        context.SetToken(refreshTokenId);
                    }
                }
                catch (Exception ex) {
                    throw (ex);
                }
            }
        }
        
        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }
    }
}