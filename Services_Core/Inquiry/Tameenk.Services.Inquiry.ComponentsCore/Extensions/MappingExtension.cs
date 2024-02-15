using Tameenk.Api.Core.Models;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Integration.Dto.Yakeen;
using Tameenk.Services.YakeenIntegration.Business.Dto;

namespace Tameenk.Services.Inquiry.Components
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


        public static InitInquiryResponseModel ToInquiryResponseModel(this QuotationRequest entity)
        {
            return entity.MapTo<QuotationRequest, InitInquiryResponseModel>();
        }

        public static IdNamePairModel ToModel(this IdNamePair entity)
        {
            return entity.MapTo<IdNamePair, IdNamePairModel>();
        }

        public static CityModel ToModel(this City entity)
        {
            return entity.MapTo<City, CityModel>();
        }

        public static AddressModel ToModel(this Address entity)
        {
            return entity.MapTo<Address, AddressModel>();
        }

        public static QuotationRequestRequiredFieldsModel ToQuotationRequestRequiredFieldsModel(this QuotationRequest entity)
        {
            return entity.MapTo<QuotationRequest, QuotationRequestRequiredFieldsModel>();
        }
        public static QuotationRequest ToEntity(this QuotationRequestRequiredFieldsModel model, QuotationRequest quotationRequest)
        {
            return model.MapTo<QuotationRequestRequiredFieldsModel, QuotationRequest>(quotationRequest);
        }

        public static CountryModel ToModel(this Country entity)
        {
            return entity.MapTo<Country, CountryModel>();
        }
        public static InquiryResponseModel ToModel(this QuotationRequest entity)
        {
            return entity.MapTo<QuotationRequest, InquiryResponseModel>();
        }
        /// <summary>
        /// Map Address to model.
        /// </summary>
        /// <param name="entity">The Address instance.</param>
        /// <returns></returns>
        public static VehicleModel ToModel(this Vehicle entity)
        {
            return entity.MapTo<Vehicle, VehicleModel>();
        }

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

        public static YakeenInfoErrorModel ToModel(this YakeenIntegration.Business.Dto.YakeenErrorDto dto)
        {
            return dto.MapTo<YakeenErrorDto, YakeenInfoErrorModel>();
        }
        #endregion

        #region Vehicles
        public static Integration.Dto.Yakeen.VehicleYakeenModel ToVehicleYakeenModel(this Vehicle entity)
        {
            return entity.MapTo<Vehicle, Integration.Dto.Yakeen.VehicleYakeenModel>();
        }
        #endregion
    }
}