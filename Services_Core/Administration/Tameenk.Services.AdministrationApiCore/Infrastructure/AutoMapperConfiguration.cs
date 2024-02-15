using AutoMapper;
using Tameenk.Api.Core.Models;
using Tameenk.Common.Utilities;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.PromotionPrograms;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Services.AdministrationApi.Extensions;
using Tameenk.Services.AdministrationApi.Models;
using Tameenk.Services.Implementation.Checkouts;
using Tameenk.Services.Implementation.Drivers;
using Tameenk.Services.Implementation.Najm;
using Tameenk.Services.Implementation.Policies;

namespace Tameenk.Services.AdministrationApi.Infrastructure
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

                cfg.CreateMap<Driver, DriverModel>(MemberList.None)
                .ForMember(s => s.City, mo => mo.MapFrom(dst => dst.City.ArabicDescription))
                .ForMember(s => s.Region, mo => mo.MapFrom(dst => dst.City.Region.Name))
                .ForMember(s => s.WorkCity, mo => mo.MapFrom(dst => dst.WorkCity.ArabicDescription)).ReverseMap();

                cfg.CreateMap<Vehicle, Models.VehicleModel>(MemberList.None)
                    .ForMember(s => s.CarPlateTextAr, mo => mo.MapFrom(dst => dst.CarPlateText1 + " " + dst.CarPlateText2 + " " + dst.CarPlateText3))
                    .ForMember(s => s.Model, mo => mo.MapFrom(dst => dst.VehicleModel)).ReverseMap();



                cfg.CreateMap<DriverFilterModel, DriverFilter>(MemberList.None).ReverseMap();
                cfg.CreateMap<NajmResponseFilterModel, NajmFilter>(MemberList.None).ReverseMap();


                cfg.CreateMap<PolicyStatusModel, PolicyStatus>(MemberList.None).ReverseMap();

                cfg.CreateMap<Address, AddressModel>(MemberList.None);
                cfg.CreateMap<Contact, ContactModel>(MemberList.None);
                cfg.CreateMap<AddressModel, Address>(MemberList.None);
                cfg.CreateMap<ContactModel, Contact>(MemberList.None);

                cfg.CreateMap<InsuranceCompany, InsuranceCompanyModel>(MemberList.None)
                .ForMember(s => s.CompanyLogo, mo => mo.MapFrom(dst => $"{Utilities.SiteURL}/resources/imgs/insurerlogos/{dst.Key}.png"));
                cfg.CreateMap<InsuranceCompanyModel, InsuranceCompany>(MemberList.None);

                cfg.CreateMap<ProductTypeModel, ProductType>(MemberList.None);
                cfg.CreateMap<ProductType, ProductTypeModel>(MemberList.None);

                cfg.CreateMap<InvoiceModel, Invoice>(MemberList.None);

                cfg.CreateMap<InvoiceModel, InsuranceCompany>(MemberList.None)
                .ForMember(s => s.NameEN, mo => mo.MapFrom(dst => dst.InsuranceCompanyNameEN))
                .ForMember(s => s.NameAR, mo => mo.MapFrom(dst => dst.InsuranceCompanyNameAR));

                cfg.CreateMap<Invoice, InvoiceModel>(MemberList.None)
                   .ForMember(s => s.ProductTypeAR, mo => mo.MapFrom(dst => dst.ProductType.ArabicDescription))
                   .ForMember(s => s.ProductTypeEN, mo => mo.MapFrom(dst => dst.ProductType.EnglishDescription))
                   .ForMember(s => s.PolicyNo, mo => mo.MapFrom(dst => dst.Policy.PolicyNo))
                   .ForMember(s => s.CreaterName, mo => mo.MapFrom(dst => dst.AspNetUser.FullName));




                cfg.CreateMap<InvoiceFile, InvoiceFileModel>(MemberList.None);
                cfg.CreateMap<InvoiceFileModel, InvoiceFile>(MemberList.None);

                cfg.CreateMap<Tameenk.Core.Domain.Entities.Policy, SuccessPolicyModel>(MemberList.None)
                .ForMember(s => s.UserEmail, mo => mo.MapFrom(dst => dst.CheckoutDetail.Email))
                .ForMember(s => s.InsuredFullNameAr, mo => mo.MapFrom(dst => dst.CheckoutDetail.Driver.FullArabicName))
                .ForMember(s => s.InsuredFullNameEn, mo => mo.MapFrom(dst => dst.CheckoutDetail.Driver.FullEnglishName))
                .ForMember(s => s.InsuredNIN, mo => mo.MapFrom(dst => dst.CheckoutDetail.Driver.NIN))
                .ForMember(s => s.VehiclePlateNumber, mo => mo.MapFrom(dst => dst.CheckoutDetail.Vehicle.CarPlateNumber))
                .ForMember(s => s.VehicleModelName, mo => mo.MapFrom(dst => dst.CheckoutDetail.Vehicle.VehicleModel + " " + dst.CheckoutDetail.Vehicle.VehicleMaker + " " + dst.CheckoutDetail.Vehicle.ModelYear))
                .ForMember(s => s.Vehicle, mo => mo.MapFrom(dst => dst.CheckoutDetail.Vehicle.ToModel()))
              .ForMember(s => s.NajmStatusObj, mo => mo.MapFrom(dst => dst.NajmStatusObj.ToModel()))
              .ForMember(s => s.PolicyStatus, mo => mo.MapFrom(dst => dst.CheckoutDetail.PolicyStatus.ToModel()))
               .ForMember(s => s.InsuredPhone, mo => mo.MapFrom(dst => dst.CheckoutDetail.Phone))
              .ForMember(s => s.InsuranceCompanyNameAr, mo => mo.MapFrom(dst => dst.InsuranceCompany.NameAR))
              .ForMember(s => s.InsuranceCompanyNameEn, mo => mo.MapFrom(dst => dst.InsuranceCompany.NameEN))
              .ForMember(s => s.ProductTypeModel, mo => mo.MapFrom(dst => dst.CheckoutDetail.ProductType.ToModel()))
              .ForMember(s => s.ImageBody, mo => mo.MapFrom(dst => dst.CheckoutDetail.ImageBody.ConvertByteImageToString()))
              .ForMember(s => s.ImageFront, mo => mo.MapFrom(dst => dst.CheckoutDetail.ImageFront.ConvertByteImageToString()))
              .ForMember(s => s.ImageLeft, mo => mo.MapFrom(dst => dst.CheckoutDetail.ImageLeft.ConvertByteImageToString()))
              .ForMember(s => s.ImageRight, mo => mo.MapFrom(dst => dst.CheckoutDetail.ImageRight.ConvertByteImageToString()))
              .ForMember(s => s.ImageBack, mo => mo.MapFrom(dst => dst.CheckoutDetail.ImageBack.ConvertByteImageToString()));

                cfg.CreateMap<FailPolicy, FailPolicyModel>(MemberList.None)
                     .ForMember(s => s.UserEmail, mo => mo.MapFrom(dst => dst.CheckoutDetail.Email))
                     .ForMember(s => s.InsuranceCompanyEn, mo => mo.MapFrom(dst => dst.Invoice.InsuranceCompany.NameEN))
                     .ForMember(s => s.InsuranceCompanyAr, mo => mo.MapFrom(dst => dst.Invoice.InsuranceCompany.NameAR))
                     .ForMember(s => s.InsuredFullNameAr, mo => mo.MapFrom(dst => dst.CheckoutDetail.Driver.FullArabicName))
                     .ForMember(s => s.InsuredFullNameEn, mo => mo.MapFrom(dst => dst.CheckoutDetail.Driver.FullEnglishName))
                     .ForMember(s => s.InsuredNIN, mo => mo.MapFrom(dst => dst.CheckoutDetail.Driver.NIN))
                     .ForMember(s => s.VehiclePlateNumber, mo => mo.MapFrom(dst => dst.CheckoutDetail.Vehicle.CarPlateNumber))
                     .ForMember(s => s.VehicleModelName, mo => mo.MapFrom(dst => dst.CheckoutDetail.Vehicle.VehicleModel + " " + dst.CheckoutDetail.Vehicle.VehicleMaker + " " + dst.CheckoutDetail.Vehicle.ModelYear))
                     .ForMember(s => s.MaxTries, mo => mo.MapFrom(dst => dst.PolicyProcessingQueue.ProcessingTries))
                     .ForMember(s => s.Vehicle, mo => mo.MapFrom(dst => dst.CheckoutDetail.Vehicle.ToModel()))
                     .ForMember(s => s.ReferenceId, mo => mo.MapFrom(dst => dst.CheckoutDetail.ReferenceId))
                     .ForMember(s => s.PolicyStatus, mo => mo.MapFrom(dst => dst.CheckoutDetail.PolicyStatus.ToModel()))
                    .ForMember(s => s.InsuredPhone, mo => mo.MapFrom(dst => dst.CheckoutDetail.Phone))
                    .ForMember(s => s.CreateDate, mo => mo.MapFrom(dst => dst.CheckoutDetail.CreatedDateTime))
                   .ForMember(s => s.ProductTypeModel, mo => mo.MapFrom(dst => dst.CheckoutDetail.ProductType.ToModel()))
                   .ForMember(s => s.ErrorDescription, mo => mo.MapFrom(dst => dst.PolicyProcessingQueue.ErrorDescription))
                    .ForMember(s => s.ServiceRequest, mo => mo.MapFrom(dst => dst.PolicyProcessingQueue.ServiceRequest))
                     .ForMember(s => s.ServiceResponse, mo => mo.MapFrom(dst => dst.PolicyProcessingQueue.ServiceResponse))
                    .ForMember(s => s.IBAN, mo => mo.MapFrom(dst => dst.CheckoutDetail.IBAN)).ReverseMap();

                cfg.CreateMap<IdNamePairModel, IdNamePair>(MemberList.None);
                cfg.CreateMap<IdNamePair, IdNamePairModel>(MemberList.None);

                cfg.CreateMap<PromotionProgram, PromotionProgramModel>(MemberList.None);
                cfg.CreateMap<PromotionProgramModel, PromotionProgram>(MemberList.None);
                cfg.CreateMap<PromotionProgramCode, PromotionProgramCodeModel>()
              .ForMember(dst => dst.IsComperhensive, mo => mo.Ignore());
                //cfg.CreateMap<OfferModel, Offer>(MemberList.None);
                //cfg.CreateMap<Offer, OfferModel>(MemberList.None);

                cfg.CreateMap<CheckoutsFilterModel, CheckoutsFilter>(MemberList.None).ReverseMap();

                cfg.CreateMap<SuccessPoliciesFilterModel, FailPolicyFilter>(MemberList.None)
                    .ForMember(s => s.InsuranceCompanyId, mo => mo.MapFrom(dst => dst.InsuranceCompanyId))
                    .ForMember(s => s.InvoiceNo, mo => mo.MapFrom(dst => dst.InvoiceNo))
                    .ForMember(s => s.NationalId, mo => mo.MapFrom(dst => dst.NationalId))
                    .ForMember(s => s.InsuredFirstNameAr, mo => mo.MapFrom(dst => dst.InsuredFirstNameAr))
                    .ForMember(s => s.InsuredLastNameAr, mo => mo.MapFrom(dst => dst.InsuredLastNameAr))
                    .ForMember(s => s.InsuredEmail, mo => mo.MapFrom(dst => dst.InsuredEmail))
                    .ForMember(s => s.SequenceNo, mo => mo.MapFrom(dst => dst.SequenceNo))
                    .ForMember(s => s.CustomNo, mo => mo.MapFrom(dst => dst.CustomNo))
                    .ForMember(s => s.ReferenceNo, mo => mo.MapFrom(dst => dst.ReferenceNo))
               .ReverseMap();

                cfg.CreateMap<FailPolicy, StatusPolicyModel>(MemberList.None)                     .ForMember(s => s.Vehicle, mo => mo.MapFrom(dst => dst.Vehicle))                     .ForMember(s => s.Invoice, mo => mo.MapFrom(dst => dst.Invoice))                     .ForMember(s => s.ProductTypeModel, mo => mo.MapFrom(dst => dst.ProductType))                     .ForMember(s => s.PolicyIssueDate, mo => mo.MapFrom(dst => dst.PolicyProcessingQueue.CreatedDate))                     .ForMember(s => s.InsuredFullNameAr, mo => mo.MapFrom(dst => dst.CheckoutDetail.Driver.FullArabicName))                     .ForMember(s => s.InsuredFullNameEn, mo => mo.MapFrom(dst => dst.CheckoutDetail.Driver.FullEnglishName))                     .ForMember(s => s.InsuredNIN, mo => mo.MapFrom(dst => dst.CheckoutDetail.Driver.NIN))                     .ForMember(s => s.CheckOutDetailsId, mo => mo.MapFrom(dst => dst.CheckoutDetail.ReferenceId))                .ReverseMap();                cfg.CreateMap<Tameenk.Core.Domain.Entities.Policy, StatusPolicyModel>(MemberList.None)                .ForMember(s => s.UserEmail, mo => mo.MapFrom(dst => dst.CheckoutDetail.Email))                .ForMember(s => s.InsuredFullNameAr, mo => mo.MapFrom(dst => dst.CheckoutDetail.Driver.FullArabicName))                .ForMember(s => s.InsuredFullNameEn, mo => mo.MapFrom(dst => dst.CheckoutDetail.Driver.FullEnglishName))                .ForMember(s => s.InsuredNIN, mo => mo.MapFrom(dst => dst.CheckoutDetail.Driver.NIN))                .ForMember(s => s.VehiclePlateNumber, mo => mo.MapFrom(dst => dst.CheckoutDetail.Vehicle.CarPlateNumber))                .ForMember(s => s.VehicleModelName, mo => mo.MapFrom(dst => dst.CheckoutDetail.Vehicle.VehicleModel + " " + dst.CheckoutDetail.Vehicle.VehicleMaker + " " + dst.CheckoutDetail.Vehicle.ModelYear))                .ForMember(s => s.Vehicle, mo => mo.MapFrom(dst => dst.CheckoutDetail.Vehicle.ToModel()))              .ForMember(s => s.NajmStatusObj, mo => mo.MapFrom(dst => dst.NajmStatusObj.ToModel()))              .ForMember(s => s.PolicyStatus, mo => mo.MapFrom(dst => dst.CheckoutDetail.PolicyStatus.ToModel()))               .ForMember(s => s.InsuredPhone, mo => mo.MapFrom(dst => dst.CheckoutDetail.Phone))              .ForMember(s => s.InsuranceCompanyNameAr, mo => mo.MapFrom(dst => dst.InsuranceCompany.NameAR))              .ForMember(s => s.InsuranceCompanyNameEn, mo => mo.MapFrom(dst => dst.InsuranceCompany.NameEN))              .ForMember(s => s.ProductTypeModel, mo => mo.MapFrom(dst => dst.CheckoutDetail.ProductType.ToModel()))              .ForMember(s => s.ImageBody, mo => mo.MapFrom(dst => dst.CheckoutDetail.ImageBody.ConvertByteImageToString()))              .ForMember(s => s.ImageFront, mo => mo.MapFrom(dst => dst.CheckoutDetail.ImageFront.ConvertByteImageToString()))              .ForMember(s => s.ImageLeft, mo => mo.MapFrom(dst => dst.CheckoutDetail.ImageLeft.ConvertByteImageToString()))              .ForMember(s => s.ImageRight, mo => mo.MapFrom(dst => dst.CheckoutDetail.ImageRight.ConvertByteImageToString()))              .ForMember(s => s.ImageBack, mo => mo.MapFrom(dst => dst.CheckoutDetail.ImageBack.ConvertByteImageToString()));

                cfg.CreateMap<SuccessPoliciesFilterModel, FailPolicyFilterModel>(MemberList.None)
                   .ForMember(s => s.InsuranceCompanyId, mo => mo.MapFrom(dst => dst.InsuranceCompanyId))
                   .ForMember(s => s.InvoiceNo, mo => mo.MapFrom(dst => dst.InvoiceNo))
                   .ForMember(s => s.NationalId, mo => mo.MapFrom(dst => dst.NationalId))
                   .ForMember(s => s.InsuredFirstNameAr, mo => mo.MapFrom(dst => dst.InsuredFirstNameAr))
                   .ForMember(s => s.InsuredLastNameAr, mo => mo.MapFrom(dst => dst.InsuredLastNameAr))
                   .ForMember(s => s.InsuredEmail, mo => mo.MapFrom(dst => dst.InsuredEmail))
                   .ForMember(s => s.SequenceNo, mo => mo.MapFrom(dst => dst.SequenceNo))
                   .ForMember(s => s.CustomNo, mo => mo.MapFrom(dst => dst.CustomNo))
                   .ForMember(s => s.ReferenceNo, mo => mo.MapFrom(dst => dst.ReferenceNo))
              .ReverseMap();
                //cfg.AuthMapperConfiguration();

            });
            Mapper = MapperConfiguration.CreateMapper();
        }
        // <summary>
        /// Mapper
        /// </summary>
        public static IMapper Mapper { get; private set; }

        /// <summary>
        /// Mapper configuration
        /// </summary>
        public static MapperConfiguration MapperConfiguration { get; private set; }
    }
}