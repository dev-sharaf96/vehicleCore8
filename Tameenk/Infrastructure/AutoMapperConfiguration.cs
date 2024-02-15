using AutoMapper;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Messages;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Implementation.Orders;
using Tameenk.Services.Implementation.Policies;
//using Tameenk.Services.Policy.Components;
//using VehicleModel = Tameenk.Services.Policy.Components.VehicleModel;

namespace Tameenk.Services.PolicyApi.Infrastructure
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
        //public static void Init()
        //{
        //    MapperConfiguration = new MapperConfiguration(cfg =>
        //    {
        //        cfg.CreateMap<NajmStatusHistory, NajmStatusHistoryModel>(MemberList.None);

        //        cfg.CreateMap<Vehicle, VehicleModel>(MemberList.None)
        //              .ForMember(s => s.CarPlateTextAr, mo => mo.MapFrom(dst => dst.CarPlateText1 + " " + dst.CarPlateText2 + " " + dst.CarPlateText3))
        //              .ReverseMap();

        //        cfg.CreateMap<PolicyStatus, PolicyStatusModel>(MemberList.None).ReverseMap();

        //        cfg.CreateMap<ProductType, ProductTypeModel>(MemberList.None).ReverseMap();

        //        cfg.CreateMap<Tameenk.Core.Domain.Entities.Policy, PolicyModel>(MemberList.None)
        //         .ForMember(s => s.UserEmail, mo => mo.MapFrom(dst => dst.CheckoutDetail.Email))
        //         .ForMember(s => s.UserName, mo => mo.MapFrom(dst => dst.CheckoutDetail.Driver.FullEnglishName))
        //         .ForMember(s => s.CompanyNameEn, mo => mo.MapFrom(dst => dst.InsuranceCompany.NameEN))
        //         .ForMember(s => s.CompanyNameAr, mo => mo.MapFrom(dst => dst.InsuranceCompany.NameAR))
        //         .ForMember(s => s.InsuredFullNameAr, mo => mo.MapFrom(dst => dst.CheckoutDetail.Driver.FullArabicName))
        //         .ForMember(s => s.InsuredFullNameEn, mo => mo.MapFrom(dst => dst.CheckoutDetail.Driver.FullEnglishName))
        //         .ForMember(s => s.InsuredNIN, mo => mo.MapFrom(dst => dst.CheckoutDetail.Driver.NIN))
        //         .ForMember(s => s.VehiclePlateNumber, mo => mo.MapFrom(dst => dst.CheckoutDetail.Vehicle.CarPlateNumber))
        //         .ForMember(s => s.VehicleModelName, mo => mo.MapFrom(dst => dst.CheckoutDetail.Vehicle.VehicleModel + " " + dst.CheckoutDetail.Vehicle.VehicleMaker + " " + dst.CheckoutDetail.Vehicle.ModelYear))
        //          .ForMember(s => s.PolicyStatusNameEn, mo => mo.MapFrom(dst => dst.CheckoutDetail.PolicyStatus.NameEn))
        //         .ForMember(s => s.PolicyStatusNameAr, mo => mo.MapFrom(dst => dst.CheckoutDetail.PolicyStatus.NameAr))
        //        .ForMember(s => s.PolicyFileByte, mo => mo.MapFrom(dst => dst.PolicyFile.PolicyFileByte))
        //         .ForMember(s => s.Vehicle, mo => mo.MapFrom(dst => dst.CheckoutDetail.Vehicle.ToModel()))
        //       .ForMember(s => s.NajmStatusObj, mo => mo.MapFrom(dst => dst.NajmStatusObj.ToModel()))
        //       .ForMember(s => s.PolicyStatus, mo => mo.MapFrom(dst => dst.CheckoutDetail.PolicyStatus.ToModel()));




        //        cfg.CreateMap<PolicyUpdateRequestAttachment, PolicyUpdateRequestAttachmentModel>(MemberList.None);
        //        cfg.CreateMap<PolicyUpdatePayment, PolicyUpdatePaymentModel>(MemberList.None);
        //        cfg.CreateMap<PolicyUpdateRequest, PolicyUpdateRequestModel>(MemberList.None)
        //        .ForMember(s => s.Policy, mo => mo.MapFrom(dst => dst.Policy.ToModel()));


        //        cfg.CreateMap<InvoiceModel, Invoice>(MemberList.None);
        //        cfg.CreateMap<Invoice, InvoiceModel>(MemberList.None)
        //        .ForMember(s => s.InsuranceCompanyNameAr, mo => mo.MapFrom(dst => dst.InsuranceCompany.NameAR))
        //        .ForMember(s => s.InsuranceCompanyNameEn, mo => mo.MapFrom(dst => dst.InsuranceCompany.NameEN));

        //        cfg.CreateMap<BankModel, UserBank>(MemberList.None);
        //        cfg.CreateMap<UserBank, BankModel>(MemberList.None);


        //        cfg.CreateMap<PolicyUpdateFileDetailsModel, PolicyUpdateFileDetails>(MemberList.None);
        //        cfg.CreateMap<PolicyUpdateFileDetails, PolicyUpdateFileDetailsModel>(MemberList.None);

        //        // cfg.CreateMap<ProductDto, Product>(MemberList.None)
        //        // .ForMember(s => s.ExternalProductId, mo => mo.MapFrom(dst => dst.ProductId))
        //        // .ForMember(s => s.Product_Benefits, mo => mo.MapFrom(dst => dst.Benefits));
        //        cfg.CreateMap<NajmStatistics, NajmStatisticsModel>(MemberList.None);
        //        cfg.CreateMap<NotificationParameter, NotificationParameterModel>(MemberList.None);
        //        cfg.CreateMap<Notification, NotificationModel>(MemberList.None);
        //        cfg.CreateMap<NotificationModel, Notification>(MemberList.None);
        //        cfg.CreateMap<PolicyFilterModel, PolicyFilter>(MemberList.None);

        //        cfg.CreateMap<PolicyDetails, PolicyTemplateGenerationModel>(MemberList.None);

        //    });
        //    Mapper = MapperConfiguration.CreateMapper();
        //}


        ///// <summary>
        ///// Mapper
        ///// </summary>
        //public static IMapper Mapper { get; private set; }

        ///// <summary>
        ///// Mapper configuration
        ///// </summary>
        //public static MapperConfiguration MapperConfiguration { get; private set; }
    }
}