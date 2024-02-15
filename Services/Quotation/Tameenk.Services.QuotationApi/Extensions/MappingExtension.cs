using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Services.Quotation.Components;
using Tameenk.Services.QuotationApi.Services;
using BenefitModel = Tameenk.Integration.Dto.Quotation.BenefitModel;
using PriceDetailModel = Tameenk.Integration.Dto.Quotation.PriceDetailModel;
using PriceTypeModel = Tameenk.Integration.Dto.Quotation.PriceTypeModel;
using ProductBenefitModel = Tameenk.Integration.Dto.Quotation.ProductBenefitModel;
using ProductModel = Tameenk.Integration.Dto.Quotation.ProductModel;
using QuotationRequestModel = Tameenk.Integration.Dto.Quotation.QuotationRequestModel;
using QuotationResponseModel = Tameenk.Integration.Dto.Quotation.QuotationResponseModel;

namespace Tameenk.Services.QuotationApi.Extensions
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

        public static QuotationResponseModel ToModel(this QuotationResponse entity)
        {
            return entity.MapTo<QuotationResponse, QuotationResponseModel>();
        }

        public static QuotationRequestModel ToModel(this QuotationRequest entity)
        {
            return entity.MapTo<QuotationRequest, QuotationRequestModel>();
        }

        public static PriceDetailModel ToModel(this PriceDetail entity)
        {
            return entity.MapTo<PriceDetail, PriceDetailModel>();
        }
        public static PriceTypeModel ToModel(this PriceType entity)
        {
            return entity.MapTo<PriceType, PriceTypeModel>();
        }
        public static ProductBenefitModel ToModel(this Product_Benefit entity)
        {
            return entity.MapTo<Product_Benefit, ProductBenefitModel>();
        }
        public static BenefitModel ToModel(this Benefit entity)
        {
            return entity.MapTo<Benefit, BenefitModel>();
        }

        #region Products
        public static ProductDto ToDto(this Product entity)
        {
            return entity.MapTo<Product, ProductDto>();
        }

        public static ProductModel ToModel(this Product entity)
        {
            return entity.MapTo<Product, ProductModel>();
        }

        public static Product ToEntity(this ProductDto dto)
        {
            return dto.MapTo<ProductDto, Product>();
        }

        public static Product ToEntity(this ProductDto dto, Product destination)
        {
            return dto.MapTo(destination);
        }
        #endregion

        #region Quotation
        public static ErrorModel ToModel(this QuotationError entity)
        {
            return entity.MapTo<QuotationError, ErrorModel>();
        }
        #endregion
    }
}