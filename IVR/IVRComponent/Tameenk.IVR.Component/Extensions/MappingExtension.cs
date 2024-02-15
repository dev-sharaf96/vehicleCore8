using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Integration.Dto.Quotation;
using QuotationComponent = Tameenk.Services.Quotation.Components;

namespace Tameenk.IVR.Component
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
            try
            {
                return entity.MapTo<QuotationResponse, QuotationResponseModel>();
            }
            catch (System.Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog\AddIVRQuotationToCach_ToModel_Exception.txt", ex.ToString());
                return null;
            }
        }
    }
}