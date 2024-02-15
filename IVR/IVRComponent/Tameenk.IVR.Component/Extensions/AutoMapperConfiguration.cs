using System;
using System.Collections.Generic;
using AutoMapper;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Integration.Dto.Quotation;

namespace Tameenk.IVR.Component
{
    /// <summary>
    /// Represent the AutoMapper configuration.
    /// For more info look at https://automapper.org/
    /// </summary>
    public static class AutoMapperConfiguration
    {
        /// <summary>
        /// Initialize the configuration of auto mapper.
        /// </summary>
        public static void Init()
        {
            MapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<QuotationResponse, QuotationResponseModel>(MemberList.None)
                    .ForMember(a => a.AllowToPurchase, a => a.Ignore());

                cfg.CreateMap<QuotationRequest, QuotationRequestModel>(MemberList.None)
                    .ForMember(a => a.RemainingTimeToExpireInSeconds, a => a.Ignore());

                cfg.CreateMap<Vehicle, Tameenk.Integration.Dto.Quotation.VehicleModel>(MemberList.None)
                    .ForMember(dst => dst.Model, mo => mo.MapFrom(s => s.VehicleModel))
                    .ForMember(s => s.VehicleModelYear, mo => mo.MapFrom(dst => dst.ModelYear))
                    .ForMember(s => s.PlateColor, mo => mo.Ignore());

                cfg.CreateMap<Product, ProductModel>(MemberList.None)
                    .ForMember(s => s.TermsFilePathAr, opt => opt.Ignore())
                    .ForMember(s => s.TermsFilePathEn, opt => opt.Ignore())
                    .ForMember(s => s.ShowTabby, opt => opt.Ignore());
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