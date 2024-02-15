using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Integration.Dto.Quotation;

namespace Tameenk.Services.Quotation.Components
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
        //public static ErrorModel ToModel(this QuotationError entity)
        //{
        //    return entity.MapTo<QuotationError, ErrorModel>();
        //}
        #endregion

        /// <summary>
        /// Map insurance company to model.
        /// </summary>
        /// <param name="entity">The company instance.</param>
        /// <returns></returns>
        public static InsuranceCompanyModel ToModel(this InsuranceCompany entity)
        {
            return entity.MapTo<InsuranceCompany, InsuranceCompanyModel>();
        }
    }
}