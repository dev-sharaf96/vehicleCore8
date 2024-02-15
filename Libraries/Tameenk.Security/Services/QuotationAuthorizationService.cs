using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Core.Caching;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Identity;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Account;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Extensions;
using Tameenk.Services.Logging;

namespace Tameenk.Security.Services
{
    public class QuotationAuthorizationService : IQuotationAuthorizationService
    {

        public QuotationAuthorizationService()
        {

        }

        public string GetUserId(IPrincipal user)
        {
            string userId = string.Empty;
            var claimsIdentity = user.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                var userClaim = claimsIdentity.Claims.FirstOrDefault(e => e.Type == "curent_user_id");
                if (userClaim != null)
                {
                    userId = userClaim.Value;
                }
            }
            return userId;
        }
    }
}
