using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Api.Core.Models;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Payments;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;
using Tameenk.Core.Domain.Entities.PromotionPrograms;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Loggin.DAL;

using Tameenk.Services.AdministrationApi.Infrastructure;
using Tameenk.Services.AdministrationApi.Models;
using Tameenk.Services.Implementation.Checkouts;
using Tameenk.Services.Implementation.Drivers;
using Tameenk.Services.Implementation.Najm;
using Tameenk.Services.Implementation.Payments;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.Implementation.Vehicles;
using Tameenk.Loggin.DAL.Dtos;
using Tameenk.Services.Implementation.Invoices;
using Tameenk.Services.Core.Promotions;
using DriverModel = Tameenk.Services.AdministrationApi.Models.DriverModel;

namespace Tameenk.Services.AdministrationApi.Extensions
{
    public static class MappingExtension
    {
        /// <summary>
        /// convert payment method to model
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static PaymentMethodModel ToModel(this PaymentMethod entity)
        {
            return entity.MapTo<PaymentMethod, PaymentMethodModel>();
        }


        /// <summary>
        /// convert payment filter To service Model
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static PaymentFilter ToServiceModel(this PaymentFilterModel entity)
        {
            return entity.MapTo<PaymentFilterModel, PaymentFilter>();
        }

        /// <summary>
        /// convert payment admin to model
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static PaymentAdminModel ToModel(this PaymentAdmin entity)
        {
            return entity.MapTo<PaymentAdmin, PaymentAdminModel>();
        }

        /// <summary>
        /// convert vehicle model to entity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Vehicle ToEntity(this Models.VehicleModel model)
        {
            return model.MapTo<Models.VehicleModel, Vehicle>();
        }

        /// <summary>
        /// Map insurance company to model.
        /// </summary>
        /// <param name="entity">The company instance.</param>
        /// <returns></returns>
        public static InsuranceCompanyModel ToModel(this InsuranceCompany entity)
        {
            return entity.MapTo<InsuranceCompany, InsuranceCompanyModel>();
        }


        /// <summary>
        /// Map service Request Filter model to service model
        /// </summary>
        /// <param name="model">The service Request Filter model instance.</param>
        /// <returns></returns>
        public static ServiceRequestFilter ToServiceModel(this ServiceRequestFilterModel model)
        {
            return model.MapTo<ServiceRequestFilterModel, ServiceRequestFilter>();
        }


        /// <summary>
        /// Map service Request Filter to  model
        /// </summary>
        /// <param name="entity">The service Request instance.</param>
        /// <returns></returns>
        public static ServiceRequestModel ToModel(this ServiceRequestLog entity)
        {
            return entity.MapTo<ServiceRequestLog, ServiceRequestModel>();
        }

        /// <summary>
        /// Map service Request Filter to model
        /// </summary>
        /// <param name="entity">The service Request instance.</param>
        /// <returns></returns>
        public static ServiceRequestModel ToModel(this IServiceRequestLog entity)
        {
            return entity.MapTo<IServiceRequestLog, ServiceRequestModel>();
        }

        /// <summary>
        /// Map Najm Status to model.
        /// </summary>
        /// <param name="entity">The Najm Status instance.</param>
        /// <returns></returns>
        public static NajmStatusModel ToModel(this NajmStatus entity)
        {
            return entity.MapTo<NajmStatus, NajmStatusModel>();
        }
        /// <summary>
        /// Map vehicle to model.
        /// </summary>
        /// <param name="entity">The vehicle instance.</param>
        /// <returns></returns>
        public static Models.VehicleModel ToModel(this Vehicle entity)
        {
            return entity.MapTo<Vehicle, Models.VehicleModel>();
        }


        /// <summary>
        /// Map insurance company to model.
        /// </summary>
        /// <param name="entity">The company instance.</param>
        /// <returns></returns>
        public static PolicyStatusModel ToModel(this PolicyStatus entity)
        {
            return entity.MapTo<PolicyStatus, PolicyStatusModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkOutImage"></param>
        /// <returns></returns>
        public static string ConvertByteImageToString(this CheckoutCarImage checkOutImage)
        {
            return Convert.ToBase64String(checkOutImage.ImageData);
        }

        /// <summary>
        /// Map SuccessPoliciesFilter Model api to  service model
        /// </summary>
        /// <param name="successPoliciesFilterModel"></param>
        /// <returns></returns>
        public static SuccessPoliciesFilter ToModel(this SuccessPoliciesFilterModel successPoliciesFilterModel)
        {
            return successPoliciesFilterModel.MapTo<SuccessPoliciesFilterModel, SuccessPoliciesFilter>();
        }

        public static SamaReport ToServiceModel(this SamaReportModel  samaReportModel)
        {
            return samaReportModel.MapTo<SamaReportModel, SamaReport>();
        }

        /// <summary>
        /// Map fail policy to FailPolicyModel
        /// </summary>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static FailPolicyModel ToModel(this FailPolicy policy)
        {
            return policy.MapTo<FailPolicy, FailPolicyModel>();
        }


        /// <summary>
        /// Map FailPolicyModel to fail policy service model
        /// </summary>
        /// <param name="failPolicyModel"></param>
        /// <returns></returns>
        public static FailPolicy ToServiceModel(this FailPolicyModel failPolicyModel)
        {
            return failPolicyModel.MapTo<FailPolicyModel, FailPolicy>();
        }


        public static SuccessPolicyModel ToServiceModel(this Tameenk.Core.Domain.Entities.Policy entity)
        {
            return entity.MapTo<Tameenk.Core.Domain.Entities.Policy, SuccessPolicyModel>();
        }


        


        /// <summary>
        /// Map FailPoliciesFilter Model api to  service model
        /// </summary>
        /// <param name="failPolicyFilterModel"></param>
        /// <returns></returns>
        public static FailPolicyFilter ToServiceModel(this FailPolicyFilterModel failPolicyFilterModel)
        {
            return failPolicyFilterModel.MapTo<FailPolicyFilterModel, FailPolicyFilter>();
        }


        /// <summary>
        /// Mao from vehicle filter api model to service model
        /// </summary>
        /// <param name="vehicleFilterModel"></param>
        /// <returns></returns>
        public static VehicleFilter ToServiceModel(this VehicleFilterModel vehicleFilterModel)
        {
            return vehicleFilterModel.MapTo<VehicleFilterModel, VehicleFilter>();
        }



        /// <summary>
        /// Map id name pair to model.
        /// </summary>
        /// <param name="entity">The company instance.</param>
        /// <returns></returns>
        public static IdNamePairModel ToModel(this IdNamePair entity)
        {
            return entity.MapTo<IdNamePair, IdNamePairModel>();
        }

       

        /// <summary>
        /// Map vehicle model to entity .
        /// </summary>
        /// <param name="model">The vehicle instance.</param>
        /// <param name="vehicle">vehicle</param>
        /// <returns></returns>
        public static Vehicle ToEntity(this Models.VehicleModel model,Vehicle vehicle)
        {
            return model.MapTo<Models.VehicleModel, Vehicle>(vehicle);
        }

        /// <summary>
        /// Map driver model to entity .
        /// </summary>
        /// <param name="model">The driver instance.</param>
        /// <param name="driver">driver</param>
        /// <returns></returns>
        public static Driver ToEntity(this DriverModel model, Driver driver)
        {
            return model.MapTo<DriverModel, Driver>(driver);
        }

        /// <summary>
        /// Map PromotionProgram model to entity .
        /// </summary>
        /// <param name="model">The PromotionProgram instance.</param>
        /// <returns></returns>
        public static PromotionProgram ToEntity(this PromotionProgramModel model)
        {
            return model.MapTo<PromotionProgramModel, PromotionProgram>();
        }

        public static PromotionProgramDomain ToEntity(this PromotionProgramDomainModel model)
        {
            return model.MapTo<PromotionProgramDomainModel, PromotionProgramDomain>();
        }
        public static PromotionProgramCode ToEntity(this PromotionProgramCodeModel model)
        {
            return model.MapTo<PromotionProgramCodeModel, PromotionProgramCode>();
        }
        /// <summary>
        /// Map driver Filter model to 
        /// driver filter service model
        /// </summary>
        /// <param name="driverFilterModel"></param>
        /// <returns></returns>
        public static DriverFilter ToServiceModel(this DriverFilterModel driverFilterModel)
        {
            return driverFilterModel.MapTo<DriverFilterModel, DriverFilter>();
        }

        public static NajmFilter ToServiceModel(this NajmResponseFilterModel najmResponseFilterModel)
        {
            return najmResponseFilterModel.MapTo<NajmResponseFilterModel, NajmFilter>();
        }

        /// <summary>
        /// map driver to driver model
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static DriverModel ToModel(this Driver driver)
        {
            return driver.MapTo<Driver,DriverModel>();
        }

        /// <summary>
        /// Map invoice to model.
        /// </summary>
        /// <param name="entity">The invoice instance.</param>
        /// <returns></returns>
        public static InvoiceModel ToModel(this Invoice entity)
        {
            return entity.MapTo<Invoice, InvoiceModel>();
        }

      

        /// <summary>
        /// Map product type to model.
        /// </summary>
        /// <param name="entity">The product type instance.</param>
        /// <returns></returns>
        public static ProductTypeModel ToModel(this ProductType entity)
        {
            return entity.MapTo<ProductType, ProductTypeModel>();
        }

        /// <summary>
        /// Map invoice File to model.
        /// </summary>
        /// <param name="entity">The invoice instance.</param>
        /// <returns></returns>
        public static InvoiceFileModel ToModel(this InvoiceFile entity)
        {
            return entity.MapTo<InvoiceFile, InvoiceFileModel>();
        }


        /// <summary>
        /// Map Insurance Company model to Entity
        /// </summary>
        /// <param name="model">The  Insurance Company model.</param>
        /// <returns></returns>
        public static InsuranceCompany ToEntity(this InsuranceCompanyModel model)
        {
            return model.MapTo<InsuranceCompanyModel, InsuranceCompany>();
        }
       
        /// <summary>
        /// Map Insurance Company model to Entity
        /// </summary>
        /// <param name="model">The  Insurance Company model.</param>
        /// <param name="insuranceCompany">The  Insurance Company instance.</param>
        /// <returns></returns>
        public static InsuranceCompany ToEntity(this InsuranceCompanyModel model,InsuranceCompany insuranceCompany)
        {
            return model.MapTo<InsuranceCompanyModel, InsuranceCompany>(insuranceCompany);
        }

        public static PromotionProgramModel ToModel(this PromotionProgram entity)
        {
            return entity.MapTo<PromotionProgram, PromotionProgramModel>();
        }

        public static PromotionProgramCodeModel ToModel(this PromotionProgramCode entity)
        {
            return entity.MapTo<PromotionProgramCode, PromotionProgramCodeModel>();
        }

        public static PromotionProgramDomainModel ToModel(this PromotionProgramDomain entity)
        {
            return entity.MapTo<PromotionProgramDomain, PromotionProgramDomainModel>();
        }

        public static AddressModel ToModel(this Address entity)
        {
            return entity.MapTo<Address, AddressModel>();
        }

        public static CityModel ToModel(this City entity)
        {
            return entity.MapTo<City, CityModel>();
        }

        /// <summary>
        /// map driver to driver model
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static NajmResponseModel ToModel(this NajmResponseEntity najmResponseEntity)
        {
            return najmResponseEntity.MapTo<NajmResponseEntity, NajmResponseModel>();
        }

        /// <summary>
        /// Generic method for mapping source to destination 
        /// with created instance of destination class
        /// </summary>
        /// <typeparam name="TSource">The source class</typeparam>
        /// <typeparam name="TDestination">The destination class</typeparam>
        /// <param name="source">The source instance</param>
        /// <param name="destination">The created destination instance.</param>
        /// <returns></returns>
        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return AutoMapperConfiguration.Mapper.Map(source, destination);
        }

        /// <summary>
        /// Generic method for mapping source to destination.
        /// </summary>
        /// <typeparam name="TSource">The source class</typeparam>
        /// <typeparam name="TDestination">Th destination class.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <returns></returns>
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return AutoMapperConfiguration.Mapper.Map<TSource, TDestination>(source);
        }

        /// <summary>
        /// Map from Checkouts filter api model to service model
        /// </summary>
        /// <param name="checkoutsFilterModel"></param>
        /// <returns></returns>
        public static CheckoutsFilter ToServiceModel(this CheckoutsFilterModel checkoutsFilterModel)
        {
            return checkoutsFilterModel.MapTo<CheckoutsFilterModel, CheckoutsFilter>();
        }

        /// <summary>
        /// Map Checkouts to model.
        /// </summary>
        /// <param name="entity">The Checkouts instance.</param>
        /// <returns></returns>
        public static Models.CheckoutsModel ToModel(this CheckoutDetail entity)
        {
            return entity.MapTo<CheckoutDetail, Models.CheckoutsModel>();
        }
        public static Implementation.Banks.BankModel ToBankModel(this Bank entity)
        {
            return entity.MapTo<Bank, Implementation.Banks.BankModel>();
        }

        #region Checkout Request Log        /// <summary>                                             /// Map Checkout Request Filter model to service model                                             /// </summary>                                             /// <param name="model">The service Request Filter model instance.</param>                                             /// <returns></returns>        public static RequestLogFilter ToServiceModel(this RequestLogsFilterModel model)        {            return model.MapTo<RequestLogsFilterModel, RequestLogFilter>();        }
        /// <summary>        /// Map Checkout Request Filter to  model        /// </summary>        /// <param name="entity">The service Request instance.</param>        /// <returns></returns>        public static CheckoutRequestLogModel ToModel(this CheckoutRequestLog entity)        {            return entity.MapTo<CheckoutRequestLog, CheckoutRequestLogModel>();        }

        #endregion
        #region Inquiry Request Log        /// <summary>         /// Map Inquiry Request Filter to  model         /// </summary>         /// <param name="entity">The service Request instance.</param>         /// <returns></returns>        public static InquiryRequestLogModel ToModel(this InquiryRequestLog entity)        {            return entity.MapTo<InquiryRequestLog, InquiryRequestLogModel>();        }
        #endregion
        #region Quotation Request Log        /// <summary>        /// Map Quotation Request Filter to  model        /// </summary>        /// <param name="entity">The service Request instance.</param>        /// <returns></returns>        public static QuotationRequestLogModel ToModel(this QuotationRequestLog entity)        {            return entity.MapTo<QuotationRequestLog, QuotationRequestLogModel>();        }










        #endregion
        /// <summary>        /// Map Policies Status Filter Model api to  service model        /// </summary>        /// <param name="statusPoliciesFilterModel"></param>        /// <returns></returns>
        public static SuccessPoliciesFilter ToSuccessModel(this StatusPoliciesFilterModel statusPoliciesFilterModel)        {            return statusPoliciesFilterModel.MapTo<StatusPoliciesFilterModel, SuccessPoliciesFilter>();        }











        /// <summary>        /// Map status policy to StatusPolicyModel        /// </summary>        /// <param name="statusPolicy"></param>        /// <returns></returns>        public static StatusPolicyModel ToStatusModel(this FailPolicy statusPolicy)        {            return statusPolicy.MapTo<FailPolicy, StatusPolicyModel>();        }











        /// <summary>        /// Map status policy to StatusPolicyModel        /// </summary>        /// <param name="policy"></param>        /// <returns></returns>        public static StatusPolicyModel ToStatusModel(this Tameenk.Core.Domain.Entities.Policy policy)        {            return policy.MapTo<Tameenk.Core.Domain.Entities.Policy, StatusPolicyModel>();        }
        /// <summary>        /// Map SuccessPoliciesFilter Model api to  service model        /// </summary>        /// <param name="successPoliciesFilterModel"></param>        /// <returns></returns>        public static FailPolicyFilter ToFailModel(this SuccessPoliciesFilterModel successPoliciesFilterModel)        {            return successPoliciesFilterModel.MapTo<SuccessPoliciesFilterModel, FailPolicyFilter>();        }        public static SMSLogFilter ToModel(this SMSLogFilterModel model)        {            return model.MapTo<SMSLogFilterModel, SMSLogFilter>();        }        public static SMSLogModel ToModel(this SMSLog entity)        {            return entity.MapTo<SMSLog, SMSLogModel>();        }        public static HyperpayResponse ToModel(this HyperPayResponseModel entity)        {            return entity.MapTo<HyperPayResponseModel, HyperpayResponse>();        }        public static PolicyCheckoutFilter ToModel(this PolicyCheckoutFilterModel model)
        {
            return model.MapTo<PolicyCheckoutFilterModel, PolicyCheckoutFilter>();
        }

        public static PolicyCheckoutModel ToServiceModel(this CheckoutDriverInfo entity)
        {
            return entity.MapTo<CheckoutDriverInfo, PolicyCheckoutModel>();
        }

        public static CheckoutDriverInfo ToModel(this PolicyCheckoutModel entity)
        {
            return entity.MapTo<PolicyCheckoutModel, CheckoutDriverInfo>();
        }        /// <summary>
        /// convert Channel to model
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static ChannelModel ToModel(this Channels entity)
        {
            return entity.MapTo<Channels, ChannelModel>();
        }

        public static AspNetUserModel ToServiceModel(this AspNetUser entity)
        {
            return entity.MapTo<AspNetUser, AspNetUserModel>();
        }

        public static FailPolicyFilterModel ToFailPolicyModel(this SuccessPoliciesFilterModel policy)
        {
            return policy.MapTo<SuccessPoliciesFilterModel, FailPolicyFilterModel>();
        }

        /// <summary>
        /// convert ticket filter to filter model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static TicketFilert ToTicketModel(this TicketFilterModel model)
        {
            return model.MapTo<TicketFilterModel, TicketFilert>();
        }

        /// <summary>
        /// convert filtered data to angular model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static TicketListingModel ToTicketListingModel(this TicketModel model)
        {
            return model.MapTo<TicketModel, TicketListingModel>();
        }

        /// <summary>
        /// convert ticket to ticket details model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static TicketListingModel ToTicketDetrailsModel(this TicketModel model)
        {
            return model.MapTo<TicketModel, TicketListingModel>();
        }

        /// <summary>
        /// convert sticket history to model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static TicketHistoryModel ToTicketHistoryModel(this UserTicketHistory model)
        {
            return model.MapTo<UserTicketHistory, TicketHistoryModel>();
        }

        /// <summary>
        /// convert ticket log filter to log model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static TicketLogFilter ToTicketLogModel(this TicketLogFilterModel model)
        {
            return model.MapTo<TicketLogFilterModel, TicketLogFilter>();
        }

        public static TicketLogListingModel ToTicketLogListingModel(this TicketLogModel model)
        {
            return model.MapTo<TicketLogModel, TicketLogListingModel>();
        }

        public static Tameenk.Services.AdministrationApi.Models.VehicleMakerModel ToVehicleMakersListingModel(this Tameenk.Loggin.DAL.Dtos.VehicleMakerModel model)
        {
            return model.MapTo<Tameenk.Loggin.DAL.Dtos.VehicleMakerModel, Tameenk.Services.AdministrationApi.Models.VehicleMakerModel>();
        }

        public static Tameenk.Services.AdministrationApi.Models.VehicleMakerModel ToVehicleMakerDetrailsModel(this Tameenk.Loggin.DAL.Dtos.VehicleMakerModel model)
        {
            return model.MapTo<Tameenk.Loggin.DAL.Dtos.VehicleMakerModel, Tameenk.Services.AdministrationApi.Models.VehicleMakerModel>();
        }

        public static Tameenk.Services.AdministrationApi.Models.VehicleMakerModelsModel ToVehicleMakerModelsListingModel(this Tameenk.Loggin.DAL.Dtos.VehicleMakerModelsModel model)
        {
            return model.MapTo<Tameenk.Loggin.DAL.Dtos.VehicleMakerModelsModel, Tameenk.Services.AdministrationApi.Models.VehicleMakerModelsModel>();
        }

        public static Tameenk.Services.AdministrationApi.Models.VehicleMakerModelsModel ToVehicleMakerModelDetrailsModel(this Tameenk.Loggin.DAL.Dtos.VehicleMakerModelsModel model)
        {
            return model.MapTo<Tameenk.Loggin.DAL.Dtos.VehicleMakerModelsModel, Tameenk.Services.AdministrationApi.Models.VehicleMakerModelsModel>();
        }

        public static Tameenk.Services.AdministrationApi.Models.VehicleMakerModelsModel ToVehicleModelsListingModel(this Tameenk.Loggin.DAL.Dtos.VehicleMakerModelsModel model)
        {
            return model.MapTo<Tameenk.Loggin.DAL.Dtos.VehicleMakerModelsModel, Tameenk.Services.AdministrationApi.Models.VehicleMakerModelsModel>();
        }

        public static Tameenk.Services.AdministrationApi.Models.YakeenCityCenter ToYakeenCityCentersListingModel(this YakeenCityCenterModel model)
        {
            return model.MapTo<YakeenCityCenterModel, Tameenk.Services.AdministrationApi.Models.YakeenCityCenter>();
        }


        public static DeservingDiscount ToEntity(this PromotionDiscountModel model)
        {
            return model.MapTo<PromotionDiscountModel, DeservingDiscount>();
        }

        public static PromotionDiscountModel ToEntity(this DeservingDiscount model)
        {
            return model.MapTo<DeservingDiscount, PromotionDiscountModel>();
        }

        public static OccupationModel ToOccupationModel(this Occupation model)
        {
            return model.MapTo<Occupation, OccupationModel>();
        }

        public static PromotionProgramNinsModel ToPromotionProgramNinsModel(this PromotionProgramNins entity)
        {
            return entity.MapTo<PromotionProgramNins, PromotionProgramNinsModel>();
        }

        public static EdaatFilter ToServiceModel(this EdaatFilterModel entity)
        {
            return entity.MapTo<EdaatFilterModel, EdaatFilter>();
        }

        public static EdaatNotificationModel ToModel(this EdaatNotificationOutput entity)
        {
            return entity.MapTo<EdaatNotificationOutput, EdaatNotificationModel>();
        }

        public static OfferModel ToModel(this Offer entity)
        {
            return entity.MapTo<Offer, OfferModel>();
        }

        public static PromotionProgramApprovalsModel ToModel(this PromotionUser entity)
        {
            return entity.MapTo<PromotionUser, PromotionProgramApprovalsModel>();
        }

        public static SamaStatisticsReportFilter ToModel(this SamaStatisticsReportModel entity)
        {
            return entity.MapTo<SamaStatisticsReportModel, SamaStatisticsReportFilter>();
        }
        public static CommissionAndFeesModel ToModel(this CommissionsAndFees entity)
        {
            return entity.MapTo<CommissionsAndFees, CommissionAndFeesModel>();
        }
        public static SMSRenewalLogFilter ToModel(this SMSRenewalLogFilterModel model)
        { 
            return model.MapTo<SMSRenewalLogFilterModel, SMSRenewalLogFilter>(); 
        }


        public static AllTypeSMSRenewalLogOutput ToModel(this AllTypeSMSRenewalLogModel entity)
        {
            return entity.MapTo<AllTypeSMSRenewalLogModel, AllTypeSMSRenewalLogOutput>();
        }


    }
}