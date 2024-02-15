using AutoMapper;
using System;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Services.QuotationApi.Models;
using Tameenk.Services.QuotationApi.Services;

namespace Tameenk.Services.QuotationApi.Infrastructure
{
    public static class AutoMapperConfiguration
    {


        public static void Init()
        {
            MapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PriceDto, PriceDetail>(MemberList.None)
               .ForMember(dst => dst.DetailId, mo => mo.MapFrom(src => Guid.NewGuid()));

                cfg.CreateMap<PriceDetail, PriceDto>(MemberList.None);


                cfg.CreateMap<BenefitDto, Product_Benefit>(MemberList.None)
                .ForMember(dst => dst.BenefitId, mo => mo.MapFrom(src => src.BenefitCode.GetValueOrDefault()))
                .ForMember(dst => dst.BenefitExternalId, mo => mo.MapFrom(src => src.BenefitId));


                cfg.CreateMap<Product_Benefit, BenefitDto>(MemberList.None)
                .ForMember(dst => dst.BenefitCode, mo => mo.MapFrom(src => src.BenefitId))
                .ForMember(dst => dst.BenefitId, mo => mo.MapFrom(src => src.BenefitExternalId));

                cfg.CreateMap<ProductDto, Product>(MemberList.None)
                .ForMember(s => s.ExternalProductId, mo => mo.MapFrom(dst => dst.ProductId))
                .ForMember(s => s.Product_Benefits, mo => mo.MapFrom(dst => dst.Benefits))
                .ForMember(s => s.DeductableValue, mo => mo.MapFrom(dst => dst.DeductibleValue));


                cfg.CreateMap<Product, ProductDto>(MemberList.None)
                .ForMember(s => s.ProductId, mo => mo.MapFrom(dst => dst.ExternalProductId))
                .ForMember(s => s.Benefits, mo => mo.MapFrom(dst => dst.Product_Benefits))
                .ForMember(s => s.PriceDetails, mo => mo.MapFrom(dst => dst.PriceDetails))
                .ForMember(s => s.DeductibleValue, mo => mo.MapFrom(dst => dst.DeductableValue));

                cfg.CreateMap<QuotationResponseModel, QuotationResponse>(MemberList.None);
                cfg.CreateMap<QuotationRequestModel, QuotationRequest>(MemberList.None);
                cfg.CreateMap<QuotationRequest, QuotationRequestModel>(MemberList.None);
                cfg.CreateMap<ProductModel, Product>(MemberList.None);
                cfg.CreateMap<PriceDetailModel, PriceDetail>(MemberList.None);
                cfg.CreateMap<PriceTypeModel, PriceType>(MemberList.None);
                cfg.CreateMap<ProductBenefitModel, Product_Benefit>(MemberList.None);
                cfg.CreateMap<BenefitModel, Benefit>(MemberList.None);
                cfg.CreateMap<ErrorModel, QuotationError>(MemberList.None);
                cfg.CreateMap<Models.VehicleModel, Vehicle>(MemberList.None)
                .ForMember(s => s.VehicleModel, mo => mo.MapFrom(dst => dst.Model))
               .ForMember(s => s.ModelYear, mo => mo.MapFrom(dst => dst.VehicleModelYear));


                cfg.CreateMap<Vehicle, Models.VehicleModel>(MemberList.None)
                .ForMember(dst => dst.Model,mo => mo.MapFrom(s => s.VehicleModel))
               .ForMember(s => s.VehicleModelYear, mo => mo.MapFrom(dst => dst.ModelYear));
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



