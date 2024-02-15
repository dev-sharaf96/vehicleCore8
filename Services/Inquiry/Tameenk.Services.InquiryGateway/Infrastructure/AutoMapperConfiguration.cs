using AutoMapper;
using System.Linq;
using Tameenk.Api.Core.Models;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Domain.Enums.Vehicles;
using Tameenk.Services.InquiryGateway.Models;
using Tameenk.Services.InquiryGateway.Services.Core.SaudiPost;

namespace Tameenk.Services.InquiryGateway.Infrastructure
{
    /// <summary>
    /// Auto Mapper Configuration
    /// </summary>
    public static class AutoMapperConfiguration
    {

        /// <summary>
        /// Initiate the mapper. 
        /// </summary>
        public static void Init()
        {
            MapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IdNamePairModel, IdNamePair>(MemberList.None);
                cfg.CreateMap<CityModel, City>(MemberList.None);
                cfg.CreateMap<CountryModel, Country>(MemberList.None);
                cfg.CreateMap<AddressModel, Address>(MemberList.None);
                cfg.CreateMap<SaudiPostAddressModel, SaudiPostAddress>(MemberList.None);
                cfg.CreateMap<SaudiPostApiResultModel, SaudiPostApiResult>(MemberList.None);
                cfg.CreateMap<QuotationRequest, QuotationRequestRequiredFieldsModel>(MemberList.None)
                .ForMember(s => s.MainDriverDateOfBirthG, mo => mo.MapFrom(dst => dst.Driver.DateOfBirthG))
                .ForMember(s => s.MainDriverDateOfBirthH, mo => mo.MapFrom(dst => dst.Driver.DateOfBirthH))
                .ForMember(s => s.MainDriverEnglishFirstName, mo => mo.MapFrom(dst => dst.Driver.EnglishFirstName))
                .ForMember(s => s.MainDriverEnglishLastName, mo => mo.MapFrom(dst => dst.Driver.EnglishLastName))
                .ForMember(s => s.MainDriverFirstName, mo => mo.MapFrom(dst => dst.Driver.FirstName))
                .ForMember(s => s.MainDriverGenderCode, mo => mo.MapFrom(dst => dst.Driver.GenderId))
                .ForMember(s => s.MainDriverIdIssuePlace, mo => mo.MapFrom(dst => dst.Driver.IdIssuePlace))
                .ForMember(s => s.MainDriverLastName, mo => mo.MapFrom(dst => dst.Driver.LastName))
                .ForMember(s => s.MainDriverNationalityCode, mo => mo.MapFrom(dst => dst.Driver.NationalityCode))
                .ForMember(s => s.MainDriverOccupationCode, mo => mo.MapFrom(dst => dst.Driver.OccupationId))
                .ForMember(s => s.MainDriverSocialStatusCode, mo => mo.MapFrom(dst => dst.Driver.SocialStatusId))
                .ForMember(s => s.VehicleBodyCode, mo => mo.MapFrom(dst => dst.Vehicle.VehicleBodyCode))
                .ForMember(s => s.VehicleChassisNumber, mo => mo.MapFrom(dst => dst.Vehicle.ChassisNumber))
                .ForMember(s => s.VehicleLicenseExpiry, mo => mo.MapFrom(dst => dst.Vehicle.LicenseExpiryDate))
                .ForMember(s => s.VehicleLoad, mo => mo.MapFrom(dst => dst.Vehicle.VehicleLoad))
                .ForMember(s => s.VehicleMajorColor, mo => mo.MapFrom(dst => dst.Vehicle.MajorColor))
                .ForMember(s => s.VehicleMaker, mo => mo.MapFrom(dst => dst.Vehicle.VehicleMaker))
                .ForMember(s => s.VehicleMakerCode, mo => mo.MapFrom(dst => dst.Vehicle.VehicleMakerCode))
                .ForMember(s => s.VehicleModel, mo => mo.MapFrom(dst => dst.Vehicle.VehicleModel))
                .ForMember(s => s.VehicleModelCode, mo => mo.MapFrom(dst => dst.Vehicle.VehicleModelCode))
                .ForMember(s => s.VehicleModelYear, mo => mo.MapFrom(dst => dst.Vehicle.ModelYear))
                .ForMember(s => s.VehiclePlateTypeCode, mo => mo.MapFrom(dst => dst.Vehicle.PlateTypeCode))
                .ForMember(s => s.VehicleRegisterationPlace, mo => mo.MapFrom(dst => dst.Vehicle.RegisterationPlace)).ReverseMap();


                cfg.CreateMap<Models.VehicleModel, Vehicle>(MemberList.None)
                .ForMember(d => d.ModificationDetails, mo => mo.MapFrom(s => s.Modification))
                .ForMember(d => d.HasModifications, mo => mo.MapFrom(s => s.HasModification));

                cfg.CreateMap<Models.DriverModel, Driver>(MemberList.None).ReverseMap()
                .ForMember(s => s.NationalId, mo => mo.MapFrom(ds => ds.NIN))
                .ForMember(s => s.ViolationIds, mo => mo.MapFrom(ds => ds.DriverViolations.Select(e => e.ViolationId)));


                cfg.CreateMap<Vehicle, Models.VehicleModel>(MemberList.None)
                .ForMember(s => s.Model, mo => mo.MapFrom(ds => ds.VehicleModel))
                .ForMember(s => s.VehicleId, mo => mo.MapFrom(ds =>
                               ds.VehicleIdTypeId == (int)VehicleIdType.SequenceNumber ? ds.SequenceNumber : ds.CustomCardNumber))
                .ForMember(s => s.VehicleModelYear, mo => mo.MapFrom(ds => ds.ModelYear))
                .ForMember(s => s.ApproximateValue, mo => mo.MapFrom(ds => ds.VehicleValue))
                .ForMember(s => s.Modification, mo => mo.MapFrom(ds => ds.ModificationDetails))
                .ForMember(s => s.HasModification, mo => mo.MapFrom(ds => ds.HasModifications))
                .ForMember(s => s.ManufactureYear, mo => mo.MapFrom(ds => ds.ModelYear));
                cfg.CreateMap<QuotationRequest, Models.InquiryResponseModel>(MemberList.None)
                //.ForMember(s => s.NajmNcdFreeYears, mo => mo.MapFrom(ds => ds.Driver.NCDFreeYears))
                .ForMember(s => s.QuotationRequestExternalId, mo => mo.MapFrom(ds => ds.ExternalId));

                cfg.CreateMap<QuotationRequest, InitInquiryResponseModel>(MemberList.None)
                .ForMember(s => s.PolicyEffectiveDate, mo => mo.MapFrom(ds => ds.RequestPolicyEffectiveDate))
                .ForMember(s => s.IsVehicleUsedCommercially, mo => mo.MapFrom(ds => ds.Vehicle.IsUsedCommercially))
                .ForMember(s => s.IsCustomerCurrentOwner, mo => mo.MapFrom(ds => !ds.Vehicle.OwnerTransfer))
                .ForMember(s => s.OldOwnerNin, mo => mo.MapFrom(ds => ds.Vehicle.CarOwnerNIN))
                .ForMember(s => s.IsCustomerSpecialNeed, mo => mo.MapFrom(ds => ds.Driver.IsSpecialNeed));



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