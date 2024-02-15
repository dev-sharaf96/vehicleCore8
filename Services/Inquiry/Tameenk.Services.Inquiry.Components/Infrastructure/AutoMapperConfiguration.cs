using AutoMapper;
using System.Linq;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Domain.Enums.Vehicles;
using Tameenk.Integration.Dto.Yakeen;
using Tameenk.Services.YakeenIntegration.Business.Dto;

namespace Tameenk.Services.Inquiry.Components
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
                //cfg.CreateMap<IdNamePairModel, IdNamePair>(MemberList.None);
                cfg.CreateMap<CityModel, City>(MemberList.None);
                cfg.CreateMap<CountryModel, Country>(MemberList.None);
                cfg.CreateMap<AddressModel, Address>(MemberList.None);
                //cfg.CreateMap<SaudiPostAddressModel, SaudiPostAddress>(MemberList.None);
                //cfg.CreateMap<SaudiPostApiResultModel, SaudiPostApiResult>(MemberList.None);
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
                 .ForMember(s => s.AdditionalDriverOneSocialStatusCode, mo => mo.MapFrom(dst => dst.Drivers.ToList().Count > 1 ? dst.Drivers.ToList()[1].SocialStatus : 0))
                .ForMember(s => s.AdditionalDriverTwoSocialStatusCode, mo => mo.MapFrom(dst => dst.Drivers.ToList().Count > 2 ? dst.Drivers.ToList()[2].SocialStatus : 0))
                .ForMember(s => s.VehicleRegisterationPlace, mo => mo.MapFrom(dst => dst.Vehicle.RegisterationPlace)).ReverseMap();
              

                cfg.CreateMap<VehicleModel, Vehicle>(MemberList.None)
                .ForMember(d => d.ModificationDetails, mo => mo.MapFrom(s => s.Modification))
                .ForMember(d => d.HasModifications, mo => mo.MapFrom(s => s.HasModification));

                cfg.CreateMap<DriverModel, Driver>(MemberList.None).ReverseMap()
                .ForMember(s => s.NationalId, mo => mo.MapFrom(ds => ds.NIN))
                .ForMember(s => s.ViolationIds, mo => mo.MapFrom(ds => ds.DriverViolations.Select(e => e.ViolationId)));


                cfg.CreateMap<Vehicle, VehicleModel>(MemberList.None)
                .ForMember(s => s.Model, mo => mo.MapFrom(ds => ds.VehicleModel))
                .ForMember(s => s.VehicleId, mo => mo.MapFrom(ds =>
                               ds.VehicleIdTypeId == (int)VehicleIdType.SequenceNumber ? ds.SequenceNumber : ds.CustomCardNumber))
                .ForMember(s => s.VehicleModelYear, mo => mo.MapFrom(ds => ds.ModelYear))
                .ForMember(s => s.ApproximateValue, mo => mo.MapFrom(ds => ds.VehicleValue))
                .ForMember(s => s.Modification, mo => mo.MapFrom(ds => ds.ModificationDetails))
                .ForMember(s => s.HasModification, mo => mo.MapFrom(ds => ds.HasModifications))
                .ForMember(s => s.ManufactureYear, mo => mo.MapFrom(ds => ds.ModelYear))
                .ForMember(s => s.ApproximateTrailerSumInsured, mo => mo.MapFrom(ds => ds.TrailerSumInsured));

                //cfg.CreateMap<QuotationRequest, Models.InquiryResponseModel>(MemberList.None)
                ////.ForMember(s => s.NajmNcdFreeYears, mo => mo.MapFrom(ds => ds.Driver.NCDFreeYears))
                //.ForMember(s => s.QuotationRequestExternalId, mo => mo.MapFrom(ds => ds.ExternalId));

                cfg.CreateMap<QuotationRequest, InitInquiryResponseModel>(MemberList.None)
                .ForMember(s => s.PolicyEffectiveDate, mo => mo.MapFrom(ds => ds.RequestPolicyEffectiveDate))
                .ForMember(s => s.IsVehicleUsedCommercially, mo => mo.MapFrom(ds => ds.Vehicle.IsUsedCommercially))
                .ForMember(s => s.IsCustomerCurrentOwner, mo => mo.MapFrom(ds => !ds.Vehicle.OwnerTransfer))
                .ForMember(s => s.OldOwnerNin, mo => mo.MapFrom(ds => ds.Vehicle.CarOwnerNIN))
                .ForMember(s => s.IsCustomerSpecialNeed, mo => mo.MapFrom(ds => ds.Driver.IsSpecialNeed));


                cfg.CreateMap<YakeenErrorDto, YakeenInfoErrorModel>();
                //cfg.CreateMap<DriverLicense, DriverLicenseYakeenInfoModel>()
                //    .ForMember(a => a.TypeCode, b => b.ResolveUsing(c => c.TypeDesc))
                //    .ForMember(a => a.ExpiryDateH, b => b.ResolveUsing(c => c.ExpiryDateH));

                cfg.CreateMap<Vehicle, Integration.Dto.Yakeen.VehicleYakeenModel>()
                    .ForMember(a => a.TameenkId, b => b.ResolveUsing(c => c.ID))
                    .ForMember(a => a.BodyCode, b => b.ResolveUsing(c => c.VehicleBodyCode))
                    .ForMember(a => a.Weight, b => b.ResolveUsing(c => c.VehicleWeight))
                    .ForMember(a => a.Load, b => b.ResolveUsing(c => c.VehicleLoad))
                    .ForMember(a => a.Maker, b => b.ResolveUsing(c => c.VehicleMaker))
                    .ForMember(a => a.Model, b => b.ResolveUsing(c => c.VehicleModel))
                    .ForMember(a => a.MakerCode, b => b.ResolveUsing(c => c.VehicleMakerCode))
                    .ForMember(a => a.ModelCode, b => b.ResolveUsing(c => c.VehicleModelCode))
                    .ForMember(a => a.Value, b => b.ResolveUsing(c => c.VehicleValue));

                cfg.CreateMap<Driver, CustomerYakeenInfoModel>()
                    .ForMember(a => a.TameenkId, b => b.ResolveUsing(c => c.DriverId));

                //cfg.CreateMap<Driver, Integration.Dto.Yakeen.DriverYakeenInfoModel>()
                //    .ForMember(a => a.TameenkId, b => b.ResolveUsing(c => c.DriverId))
                //    .ForMember(a => a.Licenses, b => b.ResolveUsing(c => c.DriverLicenses));
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