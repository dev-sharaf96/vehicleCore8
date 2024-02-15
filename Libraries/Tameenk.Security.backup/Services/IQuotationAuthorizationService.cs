using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Identity;

namespace Tameenk.Security.Services
{
    public interface IQuotationAuthorizationService
    {
        /// <summary>
        /// Get user id from user associated with the request.
        /// </summary>
        /// <param name="user">User principal</param>
        /// <returns></returns>
        string GetUserId(IPrincipal user);
    }
}
