using Tameenk.Api.Core.Models;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Services.InquiryGateway.Infrastructure;
using Tameenk.Services.InquiryGateway.Models;
using Tameenk.Services.InquiryGateway.Services.Core.SaudiPost;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Entities.Quotations;

namespace Tameenk.Services.InquiryGateway.Extensions
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

        public static IdNamePairModel ToModel(this IdNamePair entity)
        {
            return entity.MapTo<IdNamePair, IdNamePairModel>();
        }


        public static InquiryResponseModel ToModel(this QuotationRequest entity)
        {
            return entity.MapTo<QuotationRequest, InquiryResponseModel>();
        }

        public static InitInquiryResponseModel ToInquiryResponseModel(this QuotationRequest entity)
        {
            return entity.MapTo<QuotationRequest, InitInquiryResponseModel>();
        }


        public static CityModel ToModel(this City entity)
        {
            return entity.MapTo<City, CityModel>();
        }

        public static CountryModel ToModel(this Country entity)
        {
            return entity.MapTo<Country, CountryModel>();
        }
        public static SaudiPostApiResultModel ToModel(this SaudiPostApiResult entity)
        {
            return entity.MapTo<SaudiPostApiResult, SaudiPostApiResultModel>();
        }

      


        /// <summary>
        /// Map Address to model.
        /// </summary>
        /// <param name="entity">The Address instance.</param>
        /// <returns></returns>
        public static AddressModel ToModel(this Address entity)
        {
            return entity.MapTo<Address, AddressModel>();
        }

        public static QuotationRequestRequiredFieldsModel ToQuotationRequestRequiredFieldsModel(this QuotationRequest entity)
        {
            return entity.MapTo<QuotationRequest, QuotationRequestRequiredFieldsModel>();
        }

        public static QuotationRequest ToEntity(this QuotationRequestRequiredFieldsModel model,QuotationRequest quotationRequest)
        {
            return model.MapTo<QuotationRequestRequiredFieldsModel, QuotationRequest>(quotationRequest);
        }


        /// <summary>
        /// Map Address to model.
        /// </summary>
        /// <param name="entity">The Address instance.</param>
        /// <returns></returns>
        public static Models.VehicleModel ToModel(this Vehicle entity)
        {
            return entity.MapTo<Vehicle, Models.VehicleModel>();
        }
    }
}