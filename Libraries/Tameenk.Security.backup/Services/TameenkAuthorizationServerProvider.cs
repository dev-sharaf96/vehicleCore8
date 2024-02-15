using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Identity;
using Tameenk.Core.Domain.Enums.Identity;
using Tameenk.Loggin.DAL;


namespace Tameenk.Security.Services
{
    public class TameenkAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private readonly IAuthorizationService _authorizationService;

        public TameenkAuthorizationServerProvider(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            try
            {
                string clientId = string.Empty;
                string clientSecret = string.Empty;
                Client client = null;

                if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
                {
                    context.TryGetFormCredentials(out clientId, out clientSecret);
                }

                if (context.ClientId == null)
                {
                    //Remove the comments from the below line context.SetError, and invalidate context 
                    //if you want to force sending clientId/secrects once obtain access tokens. 
                    //context.Validated();
                    context.SetError("invalid_clientId", "ClientId should be sent.");
                    return Task.FromResult<object>(null);
                }

                // find client
                var clientInfo = Utilities.GetValueFromCache("ClientsTable" + context.ClientId);
                if (clientInfo == null)
                {
                    client = _authorizationService.GetClientById(context.ClientId);
                    if (client != null)
                        Utilities.AddValueToCache("ClientsTable" + context.ClientId, client, 1440);
                }
                else
                {
                    client = (Client)clientInfo;
                }

                if (client == null)
                {
                    context.SetError("invalid_clientId", string.Format("Client '{0}' is not registered in the system.", context.ClientId));
                    return Task.FromResult<object>(null);
                }
                if (client.ApplicationType == ApplicationTypes.NativeConfidential)
                {
                    if (string.IsNullOrWhiteSpace(clientSecret))
                    {
                        context.SetError("invalid_clientId", "Client secret should be sent.");
                        return Task.FromResult<object>(null);
                    }
                    else
                    {
                        if (client.Secret != GetHash(clientSecret))
                        {
                            context.SetError("invalid_clientId", "Client secret is invalid.");
                            return Task.FromResult<object>(null);
                        }
                    }
                }

                if (!client.Active)
                {
                    context.SetError("invalid_clientId", "Client is inactive.");
                    return Task.FromResult<object>(null);
                }
                context.OwinContext.Set<string>("as:clientAllowedOrigin", client.AllowedOrigin);
                context.OwinContext.Set<string>("as:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());
                context.Validated();
                return Task.FromResult<object>(null);
            }
            catch (Exception exp)
            {
                context.SetError(exp.ToString());
                return Task.FromResult<object>(null);
            }
        }


        public override async Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            try
            {
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                var requestForm = await context.Request.ReadFormAsync();
               
                if (requestForm != null)
                {
                    string userId = requestForm["curent_user_id"];
                    if (!string.IsNullOrWhiteSpace(userId))
                    {
                        identity.AddClaim(new Claim("curent_user_id", userId));
                    }
                    else
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Anonymous, Guid.NewGuid().ToString()));
                    }
                }

                identity.AddClaim(new Claim(ClaimTypes.Name, "Anonymous"));
                identity.AddClaim(new Claim("IpAddress", context?.Request?.RemoteIpAddress));
                identity.AddClaim(new Claim("UserSessionId", Guid.NewGuid().ToString()));

                var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                        "client_id", (context.ClientId == null) ? string.Empty : context.ClientId
                    }
                });

                var ticket = new AuthenticationTicket(identity, props);
                var result = context.Validated(ticket);
               
            }
            catch (Exception ex)
            {
            }
        }



        //public override async Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        //{
        //    var identity = new ClaimsIdentity(context.Options.AuthenticationType);

        //    var requestForm = await context.Request.ReadFormAsync();
        //    if (requestForm != null)
        //    {
        //        string userId = requestForm["curent_user_id"];
        //        if (!string.IsNullOrWhiteSpace(userId))
        //        {
        //            identity.AddClaim(new Claim("curent_user_id", userId));
        //        }
        //        else
        //        {
        //            identity.AddClaim(new Claim(ClaimTypes.Anonymous, Guid.NewGuid().ToString()));
        //        }
        //    }

        //    identity.AddClaim(new Claim(ClaimTypes.Name, "Anonymous"));
        //    identity.AddClaim(new Claim("IpAddress", context.Request.RemoteIpAddress));

        //    var props = new AuthenticationProperties(new Dictionary<string, string>
        //        {
        //            {
        //                "client_id", (context.ClientId == null) ? string.Empty : context.ClientId
        //            }
        //        });

        //    var ticket = new AuthenticationTicket(identity, props);
        //    context.Validated(ticket);
        //}

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");

            if (allowedOrigin == null) allowedOrigin = "*";

            if (context.OwinContext.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
            {
                context.OwinContext.Response.Headers.SetValues("Access-Control-Allow-Origin", new[] { allowedOrigin });
            }
            else
            {
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });
            }


            AspNetUser user = await _authorizationService.FindUser(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            var client = _authorizationService.GetClientById(context.ClientId);

            if (client == null)
            {
                context.SetError("invalid_clientId", "ClientId should be sent.");
                return;
            }

            // check if the logged in user has access to admin app or not , otherwise return error
            if (client.Id.Trim().ToLower() == "bc82b619-f456-42e3-b049-0126ceb44c74" && user.RoleId != Guid.Parse("36E1ECCA-FE83-4ACD-BEE3-112D14D1D74E"))
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
            identity.AddClaim(new Claim("IpAddress", context.Request.RemoteIpAddress));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));

            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                        "client_id", (context.ClientId == null) ? string.Empty : context.ClientId
                    },
                    {
                        "userName", user.FullName
                    }
                });

            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);

        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
            var currentClient = context.ClientId;

            if (originalClient != currentClient)
            {
                context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
                return Task.FromResult<object>(null);
            }

            // Change auth ticket for refresh token requests
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);

            var newClaim = newIdentity.Claims.Where(c => c.Type == "newClaim").FirstOrDefault();
            if (newClaim != null)
            {
                newIdentity.RemoveClaim(newClaim);
            }
            newIdentity.AddClaim(new Claim("newClaim", "newValue"));

            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);

            return Task.FromResult<object>(null);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        private static string GetHash(string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }



    }
}