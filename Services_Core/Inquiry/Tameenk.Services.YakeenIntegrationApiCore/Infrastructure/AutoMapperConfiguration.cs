using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Integration.Dto.Yakeen;
using Tameenk.Services.YakeenIntegrationApi.Dto;

namespace Tameenk.Services.YakeenIntegrationApi.Infrastructure
{
    public static class AutoMapperConfiguration
    {
        public static void Init()
        {
            MapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<YakeenErrorDto, YakeenInfoErrorModel>();
                cfg.CreateMap<DriverLicense, DriverLicenseYakeenInfoModel>()
                    .ForMember(a => a.TypeCode, b => b.ResolveUsing(c => c.TypeDesc))
                    .ForMember(a => a.ExpiryDateH, b => b.ResolveUsing(c => c.ExpiryDateH));

                cfg.CreateMap<Vehicle, Integration.Dto.Yakeen.VehicleYakeenModel>()
                    .ForMember(a => a.TameenkId, b => b.ResolveUsing(c => c.ID))
                    .ForMember(a => a.BodyCode, b => b.ResolveUsing(c => c.VehicleBodyCode))
                    .ForMember(a => a.Weight, b => b.ResolveUsing(c => c.VehicleWeight))
                    .ForMember(a => a.Load, b => b.ResolveUsing(c => c.VehicleLoad))
                    .ForMember(a => a.Maker, b => b.ResolveUsing(c => c.VehicleMaker))
                    .ForMember(a => a.Model, b => b.ResolveUsing(c => c.VehicleModel))
                    .ForMember(a => a.MakerCode, b => b.ResolveUsing(c => c.VehicleMakerCode))
                    .ForMember(a => a.ModelCode, b => b.ResolveUsing(c => c.VehicleModelCode))
                    .ForMember(a => a.Value, b => b.ResolveUsing(c => c.VehicleValue));

                cfg.CreateMap<Driver, CustomerYakeenInfoModel>()
                    .ForMember(a => a.TameenkId, b => b.ResolveUsing(c => c.DriverId));

                cfg.CreateMap<Driver, Integration.Dto.Yakeen.DriverYakeenInfoModel>()
                    .ForMember(a => a.TameenkId, b => b.ResolveUsing(c => c.DriverId))
                    .ForMember(a => a.Licenses, b => b.ResolveUsing(c => c.DriverLicenses));
            });
            Mapper = MapperConfiguration.CreateMapper();
        }


        /// <summary>
        /// Mapper
        /// </summary>
        public static IMapper Mapper { get; private set; }

        /// <summary>
        /// Mapper configuration
        /// </summary>
        public static MapperConfiguration MapperConfiguration { get; private set; }
    }
}