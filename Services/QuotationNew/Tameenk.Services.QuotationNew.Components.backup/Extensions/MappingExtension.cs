using Tameenk.Core.Domain;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Integration.Dto.Quotation;

namespace Tameenk.Services.QuotationNew.Components
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

        #region Products

        public static Product ToEntity(this ProductDto dto)
        {
            return dto.MapTo<ProductDto, Product>();
        }

        #endregion

        public static QuotationNewRequestDetails ToModel(this QuotationRequestInfoModel entity)
        {
            return entity.MapTo<QuotationRequestInfoModel, QuotationNewRequestDetails>();
        }
    }
}