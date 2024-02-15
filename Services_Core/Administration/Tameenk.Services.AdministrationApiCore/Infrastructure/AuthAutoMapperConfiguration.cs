using AutoMapper;
using Tameenk.Api.Core.Models;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.PromotionPrograms;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Services.Administration.Identity.Core.Domain;
using Tameenk.Services.Administration.Identity.Core.ViewModels;
using Tameenk.Services.AdministrationApi.Extensions;
using Tameenk.Services.AdministrationApi.Models;
using Tameenk.Services.Implementation.Drivers;
using Tameenk.Services.Implementation.Policies;

namespace Tameenk.Services.AdministrationApi.Infrastructure
{
    public static class AuthAutoMapperConfiguration
    {
        public static void AuthMapperConfiguration(this IMapperConfigurationExpression mapper)
        {
            mapper.CreateMap<AppUser, AppUserViewModel>(MemberList.None);
            mapper.CreateMap<AppUserViewModel, AppUser>(MemberList.None);
        }
       
    }
}