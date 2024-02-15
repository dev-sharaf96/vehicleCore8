using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Integration.Dto.Yakeen;
using Tameenk.Services.YakeenIntegrationApi.Dto;
using Tameenk.Services.YakeenIntegrationApi.Infrastructure;

namespace Tameenk.Services.YakeenIntegrationApi.Extensions
{
    public static class MappingExtension
    {
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return AutoMapperConfiguration.Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return AutoMapperConfiguration.Mapper.Map(source, destination);
        }

        #region Vehicles
        public static Integration.Dto.Yakeen.VehicleYakeenModel ToModel(this Vehicle entity)
        {
            return entity.MapTo<Vehicle, Integration.Dto.Yakeen.VehicleYakeenModel>();
        }

        //public static Vehicle ToEntity(this Models.VehicleModel dto)
        //{
        //    return dto.MapTo<ProductDto, Product>();
        //}

        //public static Product ToEntity(this ProductDto dto, Product destination)
        //{
        //    return dto.MapTo(destination);
        //}
        #endregion

        #region Driver
        public static Integration.Dto.Yakeen.DriverYakeenInfoModel ToModel(this Driver entity)
        {
            return entity.MapTo<Driver, DriverYakeenInfoModel>();
        }

        public static Integration.Dto.Yakeen.CustomerYakeenInfoModel ToCustomerModel(this Driver entity)
        {
            return entity.MapTo<Driver, Integration.Dto.Yakeen.CustomerYakeenInfoModel>();
        }
        #endregion

        #region YakeenError
        public static YakeenInfoErrorModel ToModel(this YakeenErrorDto dto)
        {
            return dto.MapTo<YakeenErrorDto, YakeenInfoErrorModel>();
        }
        #endregion
    }
}