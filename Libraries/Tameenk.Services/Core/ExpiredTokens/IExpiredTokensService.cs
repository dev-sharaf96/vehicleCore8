using System.Collections.Generic;
using System.Linq;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Loggin.DAL.Dtos;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.Implementation.Vehicles;

namespace Tameenk.Services.Core
{
    public interface IExpiredTokensService
    {

        ExpiredTokens GetFromExpiredCookie(string cookie);


    }
}
