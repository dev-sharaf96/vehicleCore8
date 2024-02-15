using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums.Vehicles;
using Tameenk.Loggin.DAL;

using Tameenk.Resources.Inquiry;
using Tameenk.Services.Core.Vehicles;
using System.Data.Entity;
using System.Data.Linq;
using Tameenk.Services.Core.Http;
using Tameenk.Loggin.DAL;
using Tameenk.Integration.Dto.Yakeen;
using Tameenk.Services.YakeenIntegration.Business;
using Tameenk.Services.YakeenIntegration.Business.Services.Core;
using Tameenk.Resources.WebResources;
using Tameenk.Services.Core.Addresses;
using Tameenk.Core.Domain.Entities.PromotionPrograms;
using Tameenk.Integration.Dto.Najm;
using Tameenk.Api.Core.Models;
using Tameenk.Core.Domain.Enums.Quotations;
using Tameenk.Services.Core.Occupations;
using Tameenk.Api.Core.Context;
using System.ComponentModel.DataAnnotations;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.YakeenIntegration.Business.Dto.YakeenOutputModels;
using Tameenk.Services.Extensions;
using Tameenk.Services.YakeenIntegration.Business.Dto;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Core;
using Tameenk.Services.YakeenIntegration.Business.Repository;
using Tameenk.Integration.Dto.Yakeen.Enums;
using Tameenk.Core;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Implementation;
using Tameenk.Core.Domain.Enums;
using Tameenk.Services.YakeenIntegration.Business.YakeenBCareService;
using OfficeOpenXml;
using System.IO;
using System.Text.RegularExpressions;
using Tameenk.Services.Core;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Integration.Core.Providers;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using Tameenk.Core.Domain.Entities.Orders;
using Tameenk.Services.Core.Leasing.Models;
using Tameenk.Services.Core.Leasing;
//using DocumentFormat.OpenXml.Packaging;

namespace Tameenk.Services.Inquiry.Components
{
    public class AutoleasingInquiryContext: IAutoleasingInquiryContext
    {

     
        private readonly IRepository<Insured> insuredRepository;
        private readonly IVehicleService vehicleService;
        private readonly IRepository<Driver> driverRepository;
        private readonly IAddressService addressService;
        private readonly IRepository<QuotationRequest> quotationRequestRepository;
        private readonly IYakeenVehicleServices yakeenVehicleServices;
        private readonly IOccupationService occupationService;
        private readonly IWebApiContext webApiContext;
        private readonly IRepository<NCDFreeYear> nCDFreeYearRepository;
        private readonly IAutoleasingUserService _autoleasingUserService;
        private readonly IBankService _bankService;
        private readonly IRepository<BankNins> _bankNinsRepository;
        private readonly IRepository<AutoleasingDepreciationSetting> _autoleasingDepreciationSettingRepository;
        private readonly IRepository<AutoleasingDepreciationSettingHistory> _autoleasingDepreciationSettingRepositoryHistory;
        private readonly IRepository<AutoleasingAgencyRepairHistory> _autoleasingRepairMethodRepositoryHistory;
        private readonly IRepository<AutoleasingAgencyRepair> _autoleasingRepairMethodRepository;
        private readonly IRepository<AutoleasingMinimumPremium> _autoleasingMinimumPremiumRepository;
        private readonly IRepository<AutoleasingMinimumPremiumHistory> _autoleasingMinimumPremiumRepositoryHistory;
        private readonly IRepository<AutoleasingSelectedBenifits> _autoleasingSelectedBenifitsRepository;
        private readonly IRepository<AutoleasingQuotationResponseCache> _autoleasingQuotationResponseCache;
        private readonly IInquiryUtilities _inquiryUtilities;
        private readonly IInsuranceCompanyService _insuranceCompanyService;
        private readonly IRepository<PolicyModification> _policyModificationrepository;
        private readonly IRepository<QuotationResponse> _quotationResponserepository;
        private readonly IRepository<CheckoutDetail> _checkoutDetailrepository;
        private readonly IRepository<QuotationRequest> _quotationRequestrepository;
        private readonly IRepository<Endorsment> _endormentRepository;
        private readonly IRepository<PolicyAdditionalBenefit> _policyAdditionalBenefitRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<OrderItemBenefit> _orderItemBenefitRepository;
        private readonly IRepository<EndorsmentBenefit> _endormentBenefitRepository;
        private readonly TameenkConfig _tameenkConfig;
        private readonly IYakeenClient _yakeenClient;
        private readonly IRepository<CustomCardInfo> _customCardInfoRepository;
        private readonly HashSet<string> requiredFieldsInCustomOnly = new HashSet<string>
            {
                "VehicleLoad",
                "VehicleModel",
                "VehicleModelCode",
                "VehicleMajorColor",
                "VehicleBodyCode",
                "VehicleMaker",
                "VehicleMakerCode",
                "VehicleChassisNumber"
            };
        private readonly HashSet<string> FieldsAllowNullValue = new HashSet<string>
            {
                "AdditionDriver1ID",
                "AdditionDriver1DOB",
                "AdditionDriver2ID",
                "AdditionDriver2DOB"
            };
        private readonly IRepository<Benefit> _benefitRepository;
        private readonly IRepository<Vehicle> _vehicleRepository;
        private readonly IRepository<Quotation_Product_Benefit> _quotationProductBenefitRepository;
        private readonly IRepository<Vehicle> _vehicle;
        private readonly IRepository<Product> _productRepository;
        private readonly ILeasingUserService _leasingUserService;
        private readonly IRepository<Product_Driver> _productDriverRepository;
        private readonly IRepository<PolicyProcessingQueue> _policyProcessingQueue;

        public AutoleasingInquiryContext(IRepository<Insured> insuredRepository, IVehicleService vehicleService,
            IRepository<Driver> driverRepository,IAddressService addressService,IRepository<QuotationRequest> quotationRequestRepository,
            IYakeenVehicleServices yakeenVehicleServices,IOccupationService occupationService,IWebApiContext webApiContext,
            IRepository<NCDFreeYear> NCDFreeYearRepository,IRepository<Policy> policyRepository,
            IAutoleasingUserService autoleasingUserService, IBankService bankService
            , IRepository<BankNins> bankNinsRepository, IRepository<AutoleasingDepreciationSetting> autoleasingDepreciationSettingRepository
            , IRepository<AutoleasingDepreciationSettingHistory> autoleasingDepreciationSettingRepositoryHistory
            , IRepository<AutoleasingAgencyRepairHistory> autoleasingRepairMethodRepositoryHistory
            , IRepository<AutoleasingAgencyRepair> autoleasingRepairMethodRepository
            , IRepository<AutoleasingMinimumPremium> autoleasingMinimumPremiumRepository
            , IRepository<AutoleasingMinimumPremiumHistory> autoleasingMinimumPremiumRepositoryHistory
            , IRepository<AutoleasingSelectedBenifits> autoleasingSelectedBenifitsRepository
            , IRepository<AutoleasingQuotationResponseCache> autoleasingQuotationResponseCache
            , IInquiryUtilities inquiryUtilities
             , IInsuranceCompanyService insuranceCompanyService
            ,   IRepository<PolicyModification> policyModificationrepository
            , IRepository<QuotationResponse> quotationResponserepository
            , IRepository<CheckoutDetail> checkoutDetailrepository
             , IRepository<QuotationRequest> quotationRequestrepository
             , IRepository<Endorsment> endormentRepository
              , TameenkConfig tameenkConfig
             , IRepository<PolicyAdditionalBenefit> policyAdditionalBenefitRepository
              , IRepository<OrderItem> orderItemRepository
            , IRepository<OrderItemBenefit> orderItemBenefitRepository
             , IRepository<EndorsmentBenefit> endormentBenefitRepository, IYakeenClient yakeenClient, IRepository<CustomCardInfo> customCardInfoRepository
            , IRepository<Benefit> benefitRepository
            , IRepository<Vehicle> vehicleRepository
            , IRepository<Quotation_Product_Benefit> quotationProductBenefitRepository
            , IRepository<Vehicle> vehicle
            , IRepository<Product> productRepository
            , ILeasingUserService leasingUserService
            , IRepository<Product_Driver> productDriverRepository
            , IRepository<PolicyProcessingQueue> policyProcessingQueue)
        {
            this.insuredRepository = insuredRepository;
            this.vehicleService = vehicleService;
            this.driverRepository = driverRepository;
            this.addressService = addressService;
            this.quotationRequestRepository = quotationRequestRepository;
            this.yakeenVehicleServices = yakeenVehicleServices;
            this.occupationService = occupationService;
            this.webApiContext = webApiContext;
            nCDFreeYearRepository = NCDFreeYearRepository;
            _autoleasingUserService = autoleasingUserService;
            _bankService = bankService;
            _bankNinsRepository = bankNinsRepository;
            _autoleasingDepreciationSettingRepository = autoleasingDepreciationSettingRepository;
            _autoleasingDepreciationSettingRepositoryHistory = autoleasingDepreciationSettingRepositoryHistory;
            _autoleasingRepairMethodRepositoryHistory = autoleasingRepairMethodRepositoryHistory;
            _autoleasingRepairMethodRepository = autoleasingRepairMethodRepository;
            _autoleasingMinimumPremiumRepository = autoleasingMinimumPremiumRepository;
            _autoleasingMinimumPremiumRepositoryHistory = autoleasingMinimumPremiumRepositoryHistory;
            _autoleasingSelectedBenifitsRepository = autoleasingSelectedBenifitsRepository;
            _autoleasingQuotationResponseCache = autoleasingQuotationResponseCache;
            _inquiryUtilities = inquiryUtilities;
            _insuranceCompanyService = insuranceCompanyService;
            _policyModificationrepository = policyModificationrepository;
            _quotationResponserepository = quotationResponserepository;
            _checkoutDetailrepository = checkoutDetailrepository;
            _quotationRequestrepository = quotationRequestrepository;
            _endormentRepository = endormentRepository;
            _tameenkConfig = tameenkConfig;
            _policyAdditionalBenefitRepository = policyAdditionalBenefitRepository;
            _orderItemRepository = orderItemRepository;
            _orderItemBenefitRepository = orderItemBenefitRepository;
            _endormentBenefitRepository = endormentBenefitRepository;
            _yakeenClient = yakeenClient;
            _customCardInfoRepository = customCardInfoRepository;
            _benefitRepository = benefitRepository;
            _vehicleRepository = vehicleRepository;
            _quotationProductBenefitRepository = quotationProductBenefitRepository;
            _vehicle = vehicle;
            _productRepository = productRepository;
            _leasingUserService = leasingUserService;
            _productDriverRepository = productDriverRepository;
            _policyProcessingQueue = policyProcessingQueue;
        }

        public InquiryOutput AutoleasingInitInquiryRequest(InitInquiryRequestModel model, InquiryRequestLog log)
        {
            InquiryOutput output = new InquiryOutput();
            try
            {

                if (string.IsNullOrEmpty(model.NationalId))
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.NationalIdIsNull;
                    output.ErrorDescription = SubmitInquiryResource.NationalIdRequired;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "NationalId is empty";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.SequenceNumber))
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.SequenceNumberIsNull;
                    output.ErrorDescription = SubmitInquiryResource.SequenceNumberRequired;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Sequence Number is empty";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                if (model.VehicleIdTypeId == 0)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.VehicleIdTypeIdIsNull;
                    output.ErrorDescription = SubmitInquiryResource.VehicleIdTypeIdRequired;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Vehicle Id type is Zero";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }

                if (model.OwnerTransfer && model.OwnerNationalId == model.NationalId)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.OwnerNationalIdAndNationalIdAreEqual;
                    output.ErrorDescription = SubmitInquiryResource.InvaildOldOwnerNin;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "OwnerTransfer and Vehicle.OwnerNationalId and Insured.NationalId are the same";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                if (model.OwnerTransfer && string.IsNullOrEmpty(model.OwnerNationalId) && model.Channel == Channel.Portal)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.VehicleOwnerNinIsNull;
                    output.ErrorDescription = SubmitInquiryResource.VehicleOwnerNinIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "OwnerTransfer and Owner National Id is empty";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }

                if (model.PolicyEffectiveDate < DateTime.Now.Date.AddDays(1) || model.PolicyEffectiveDate > DateTime.Now.AddDays(30))
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.autoleasing_InvalidPolicyEffectiveDate;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Policy effective date should be within 30 days starts from toworrow as user enter " + model.PolicyEffectiveDate.ToString();
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }

                if (string.IsNullOrEmpty(model.MobileNo))
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.MobileNumberIsEmpty;
                    output.ErrorDescription = SubmitInquiryResource.MobileNumberIsEmpty;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "main driver Mobile Number is empty";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }

                if (!Utilities.IsValidPhoneNo(model.MobileNo))
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.MobileNumberNotValid;
                    output.ErrorDescription = SubmitInquiryResource.MobileNumberNotValid;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "main driver Mobile Number is not valid : " + model.MobileNo;
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                InitInquiryResponseModel responseModel = new InitInquiryResponseModel();
                responseModel.PolicyEffectiveDate = model.PolicyEffectiveDate;
                responseModel.IsCustomerCurrentOwner = !model.OwnerTransfer;
                if (model.OwnerTransfer)
                {
                    long oldOwnerNin = 0;
                    long.TryParse(model.OwnerNationalId, out oldOwnerNin);
                    responseModel.OldOwnerNin = oldOwnerNin;
                }
                if (model.OwnerTransfer && (!responseModel.OldOwnerNin.HasValue || responseModel.OldOwnerNin == 0) && model.Channel == Channel.Portal)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.VehicleOwnerNinIsNull;
                    output.ErrorDescription = SubmitInquiryResource.VehicleOwnerNinIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "OwnerTransfer and OldOwnerNin is zero";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                responseModel.IsVehicleExist = false;
                responseModel.IsMainDriverExist = false;
                output.InitInquiryResponseModel = responseModel;
                output.ErrorCode = InquiryOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                //log.ErrorCode = (int)output.ErrorCode;
                //log.ErrorDescription = output.ErrorDescription;
                //InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = SubmitInquiryResource.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = exp.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                return output;
            }
        }
        public InquiryOutput AutoleasingSubmitInquiryRequest(InquiryRequestModel requestModel, string channel, Guid userId, string userName, Guid? parentRequestId = null)
        {
            requestModel.InquiryOutputModel = new InquiryOutputModel();
            InquiryOutput output = new InquiryOutput();
            InquiryRequestLog log = new InquiryRequestLog();
            log.Channel = channel.ToString();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.Channel = channel;
            log.UserId = userId.ToString();
            log.UserName = userName;
            log.MethodName = "AutoleasingSubmitInquiry";
            log.NIN = requestModel.Insured.NationalId.Trim();
            log.VehicleId = requestModel.Vehicle.VehicleId.ToString();
            log.RequestId = requestModel.ParentRequestId;
            log.ServiceRequest = JsonConvert.SerializeObject(requestModel);
            log.MobileVersion = requestModel.MobileVersion;
            try
            {
                if (userId == Guid.Empty || string.IsNullOrEmpty(userId.ToString()))
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = SubmitInquiryResource.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User id  is null";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                var validationOutput = AutoleasingValidateData(requestModel, log, channel);
                if (validationOutput.ErrorCode != ValidationOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = validationOutput.ErrorDescription;
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(validationOutput.Log);
                    return output;
                }
                // apply phone validation
                if (!requestModel.IsInitialOption)
                {
                    var mainDriver = requestModel.Drivers.FirstOrDefault(e => e.NationalId == validationOutput.DriverNin.ToString());
                    if (string.IsNullOrEmpty(mainDriver.MobileNo))
                    {
                        output.ErrorCode = InquiryOutput.ErrorCodes.MobileNumberIsEmpty;
                        output.ErrorDescription = SubmitInquiryResource.MobileNumberIsEmpty;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "main driver Mobile Number is empty";
                        InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                        return output;
                    }

                    if (!Utilities.IsValidPhoneNo(mainDriver.MobileNo))
                    {
                        output.ErrorCode = InquiryOutput.ErrorCodes.MobileNumberNotValid;
                        output.ErrorDescription = SubmitInquiryResource.MobileNumberNotValid;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "main driver Mobile Number is not valid : " + mainDriver.MobileNo;
                        InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                        return output;
                    }
                }

                requestModel = validationOutput.RequestModel;

                var drivers = requestModel.Drivers.Where(d => d.NationalId != requestModel.Insured.NationalId);
                int numberOfDriver = drivers.Count();
                if (numberOfDriver > 1)
                {
                    int percentage = 0;
                    int mainPercentage = 0;

                    if (numberOfDriver == 3)
                    {
                        percentage = 25;
                        mainPercentage = 50;
                    }
                    else if (numberOfDriver == 2)
                    {
                        percentage = mainPercentage = 50;
                    }

                    foreach (var d in requestModel.Drivers.Where(d => d.NationalId != requestModel.Insured.NationalId))
                    {
                        if (d.IsCompanyMainDriver)
                            d.DrivingPercentage = mainPercentage;
                        else
                            d.DrivingPercentage = percentage;
                    }
                }
                var result = AutoleasingHandleQuotationRequest(requestModel, validationOutput.DriverNin, ref log);
                output.InquiryResponseModel = result.InquiryResponseModel;
                if (!(string.IsNullOrEmpty(result.ErrorDescription)) && result.ErrorCode != InquiryOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = result.ErrorCode;
                    output.ErrorDescription = result.ErrorDescription;
                    return output;
                }
                if (!requestModel.IsInitialOption)
                {
                    var yakeenOutput = _inquiryUtilities.HandleYakeenMissingFields(output, channel.ToLower() != Channel.Portal.ToString().ToLower() ? requestModel.Language : "");
                }
                output.ErrorCode = InquiryOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = SubmitInquiryResource.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = exp.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                return output;
            }
        }
        public InquiryOutput AutoleasingHandleQuotationRequest(InquiryRequestModel requestModel, long mainDriverNin, ref InquiryRequestLog log)
        {
            string exception = string.Empty;
            InquiryOutput output = new InquiryOutput
            {
                InquiryResponseModel = new InquiryResponseModel()
            };
            try
            {
                Guid userID = Guid.Empty;
                Guid.TryParse(log.UserId, out userID);
                ServiceRequestLog predefinedLogInfo = new ServiceRequestLog
                {
                    UserID = userID,
                    UserName = log.UserName,
                    Channel = log.Channel,
                    ServerIP = log.ServerIP
                };
                predefinedLogInfo.RequestId = log.RequestId;
                predefinedLogInfo.DriverNin = mainDriverNin.ToString();
                predefinedLogInfo.VehicleId = requestModel?.Vehicle?.VehicleId.ToString();

                log.NIN = mainDriverNin.ToString();
                log.VehicleId = requestModel?.Vehicle?.VehicleId.ToString();

                var qtRqstExtrnlId = _inquiryUtilities.GetNewRequestExternalId();
                log.ExternalId = qtRqstExtrnlId;
                predefinedLogInfo.ExternalId = qtRqstExtrnlId;

                //1) Vehicle
                VehicleYakeenModel vehicleYakeenInfo;
                string bankName = "";
                var userInfo = _autoleasingUserService.GetUser(log.UserId.ToString());
                if (userInfo != null && userInfo.BankId != 0)
                {
                    var bank = _bankService.GetBank(userInfo.BankId);
                    if (bank != null)
                    {
                        bankName = bank.NameAr;
                    }
                }
                if (requestModel.IsInitialOption) // No option
                {
                    Vehicle vehicleInfo = new Vehicle();
                    vehicleInfo.ID = Guid.NewGuid();
                    vehicleInfo.VehicleValue = (int)requestModel.Vehicle.ApproximateValue;
                    vehicleInfo.TransmissionTypeId = 1;
                    vehicleInfo.ParkingLocationId = 1;
                    vehicleInfo.MileageExpectedAnnualId = 1;

                    if (requestModel != null && requestModel.Vehicle != null && !string.IsNullOrEmpty(requestModel.Vehicle.MajorColor))
                    {
                        vehicleInfo.ColorCode = requestModel.Vehicle.ColorCode;
                        vehicleInfo.MajorColor = requestModel.Vehicle.MajorColor;
                    }
                    else
                    {
                        vehicleInfo.MajorColor = "أبيض";
                        vehicleInfo.ColorCode = 1;
                    }

                    vehicleInfo.ModelYear = requestModel.Vehicle.ModelYear;
                    vehicleInfo.VehicleMaker = requestModel.Vehicle.VehicleMaker;
                    vehicleInfo.VehicleModel = requestModel.Vehicle.Model;
                    vehicleInfo.VehicleMakerCode = requestModel.Vehicle.VehicleMakerCode;
                    vehicleInfo.VehicleModelCode = requestModel.Vehicle.VehicleModelCode;

                    if (!string.IsNullOrEmpty(bankName))
                    {
                        vehicleInfo.CarOwnerName = bankName;
                    }
                    exception = string.Empty;
                    if (!yakeenVehicleServices.InsertVehicleIntoDb(vehicleInfo, out exception))
                    {
                        output.ErrorCode = InquiryOutput.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Failed to insert vehicle to Db due to " + exception;
                        InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                        return output;
                    }
                    vehicleYakeenInfo = vehicleInfo.ToVehicleYakeenModel();
                }
                else
                {

                    vehicleYakeenInfo = _inquiryUtilities.GetVehicleYakeenInfo(requestModel, predefinedLogInfo);
                    if (vehicleYakeenInfo == null)
                    {
                        output.ErrorCode = InquiryOutput.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "GetVehicleYakeenInfo returned null";
                        InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                        return output;
                    }
                    if (vehicleYakeenInfo != null && vehicleYakeenInfo.Error != null && vehicleYakeenInfo.Error.ErrorCode == "0")
                    {
                        output.ErrorCode = InquiryOutput.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "GetVehicleYakeenInfo returned " + vehicleYakeenInfo.Error.ErrorDescription;
                        InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                        return output;
                    }
                    if (!vehicleYakeenInfo.Success)
                    {
                        if (vehicleYakeenInfo.Error != null && vehicleYakeenInfo.Error.ErrorMessage != null
                            && ((vehicleYakeenInfo.Error.Type == EErrorType.YakeenError ||
                            (vehicleYakeenInfo.Error.Type == EErrorType.LocalError && RepositoryConstants.ShowLocalErrorDetailsInResponse))))
                        {
                            string ErrorMessage = SubmitInquiryResource.ResourceManager.GetString($"YakeenError_{vehicleYakeenInfo.Error?.ErrorCode}", CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.Name));
                            var GenericErrorMessage = !string.IsNullOrEmpty(ErrorMessage) ? ErrorMessage : SubmitInquiryResource.YakeenError_100;
                            output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                            output.ErrorDescription = GenericErrorMessage;
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = $"GetVehicleYakeenInfo returned code:{vehicleYakeenInfo.Error?.ErrorCode} | message:{vehicleYakeenInfo.Error.ErrorMessage} | description: {vehicleYakeenInfo.Error.ErrorDescription}";
                        }
                        else
                        {
                            output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                            output.ErrorDescription = "";
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = "Service Exception for GetVehicleYakeenInfo";
                        }
                        InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                        return output;
                    }
                    if (string.IsNullOrEmpty(vehicleYakeenInfo.CarOwnerName))
                    {
                        var vehicleInfo = vehicleService.GetVehicle(vehicleYakeenInfo.TameenkId.ToString());
                        if (vehicleInfo != null && vehicleInfo.CarOwnerName != bankName)
                        {
                            vehicleInfo.CarOwnerName = bankName;
                            vehicleService.UpdateVehicle(vehicleInfo);

                        }
                    }
                }
                //2) Drivers
                foreach(var d in requestModel.Drivers)
                {
                    d.DriverNOALast5Years = 0;
                }
                var driversoutput = _inquiryUtilities.GetDriversData(requestModel.Drivers, requestModel.Insured, requestModel.IsCustomerSpecialNeed, mainDriverNin, predefinedLogInfo);
                if (driversoutput.ErrorCode != DriversOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.DriverDataError;
                    output.ErrorDescription = driversoutput.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = driversoutput.LogErrorDescription;
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                var customerYakeenInfo = driversoutput.MainDriver.ToCustomerModel(); //-- to be handeled
                //3)Validate Address
                int addressId = 0;
                string postCode = string.Empty;
                #region Validate Address
                string birthDate = string.Format("{0}-{1}", requestModel.Insured.BirthDateMonth.ToString("00"), requestModel.Insured.BirthDateYear);
                var driverCityInfoOutput = _inquiryUtilities.GetDriverCityInfo(driversoutput.MainDriver.DriverId, mainDriverNin.ToString(), log.VehicleId, log.Channel, birthDate, true, qtRqstExtrnlId);
                if (driverCityInfoOutput.ErrorCode == DriverCityInfoOutput.ErrorCodes.SaudiPostNoResultReturned
                    || driverCityInfoOutput.ErrorCode == DriverCityInfoOutput.ErrorCodes.NoAddressFound)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.SaudiPostNoResultReturned;
                    output.ErrorDescription = SubmitInquiryResource.MissingAddress;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = driverCityInfoOutput.ErrorDescription;
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                if (driverCityInfoOutput.ErrorCode == DriverCityInfoOutput.ErrorCodes.NullResponse)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.SaudiPostNullResponse;
                    output.ErrorDescription = SubmitInquiryResource.SaudiPostError;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = driverCityInfoOutput.ErrorDescription;
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                if (driverCityInfoOutput.ErrorCode == DriverCityInfoOutput.ErrorCodes.InvalidPublicID)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.InvalidPublicID;
                    output.ErrorDescription = SubmitInquiryResource.SaudiPostInvalidPublicID.Replace("{0}", mainDriverNin.ToString());
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = driverCityInfoOutput.ErrorDescription;
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                if (driverCityInfoOutput.ErrorCode != DriverCityInfoOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.SaudiPostError;
                    output.ErrorDescription = SubmitInquiryResource.SaudiPostError;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = driverCityInfoOutput.ErrorDescription;
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                var cities = addressService.GetAllCities();
                long elmCode = driverCityInfoOutput.Output.ElmCode;
                addressId = driverCityInfoOutput.Output.AddressId;
                postCode = driverCityInfoOutput.Output.PostCode;

                requestModel.CityCode = elmCode;
                //Main Driver
                var mainDriver = requestModel.Drivers.FirstOrDefault(e => e.NationalId == mainDriverNin.ToString());
                var mainDriverDB = driverRepository.Table.Where(d => d.DriverId == driversoutput.MainDriver.DriverId).FirstOrDefault();

                mainDriverDB.CityId = elmCode;
                mainDriverDB.CityName = driverCityInfoOutput.Output.CityNameAr;
                mainDriverDB.AddressId = addressId;
                mainDriverDB.PostCode = postCode;
                if (mainDriver.DriverWorkCityCode.HasValue && !requestModel.Insured.InsuredWorkCityCode.HasValue)
                {
                    mainDriverDB.WorkCityName = mainDriver.DriverWorkCity;
                    mainDriverDB.WorkCityId = mainDriver.DriverWorkCityCode;
                }
                else if (requestModel.Insured.InsuredWorkCityCode.HasValue)
                {
                    mainDriverDB.WorkCityId = requestModel.Insured.InsuredWorkCityCode;
                    mainDriverDB.WorkCityName = cities.FirstOrDefault(c => c.Code == requestModel.Insured.InsuredWorkCityCode)?.ArabicDescription;
                }
                else
                {
                    mainDriverDB.WorkCityName = driverCityInfoOutput.Output.CityNameAr;
                    mainDriverDB.WorkCityId = elmCode;
                }
                if (driversoutput.AdditionalDrivers != null && driversoutput.AdditionalDrivers.Any())
                {
                    foreach (var additionalDriver in driversoutput.AdditionalDrivers)
                    {
                        var additionalDriverDB = driverRepository.Table.Where(d => d.DriverId == additionalDriver.DriverId).FirstOrDefault();

                        if (!additionalDriver.CityId.HasValue)
                        {
                            int cityId = 0;
                            int.TryParse(driversoutput.MainDriver.CityId?.ToString(), out cityId);
                            additionalDriver.CityName = driversoutput.MainDriver.CityName;
                            additionalDriver.CityId = cityId;
                        }
                        if (!additionalDriver.WorkCityId.HasValue)
                        {
                            int workCtyId = 0;
                            int.TryParse(driversoutput.MainDriver.WorkCityId?.ToString(), out workCtyId);
                            additionalDriver.WorkCityName = driversoutput.MainDriver.WorkCityName;
                            additionalDriver.WorkCityId = workCtyId;
                        }

                        driverRepository.Update(additionalDriverDB);
                    }
                }
                #endregion
                //GetNajmResponse
                NajmResponse najmResponse = new NajmResponse();
                if (!requestModel.IsInitialOption)
                {
                    NajmRequest najmRequest = new NajmRequest()
                    {
                        IsVehicleRegistered = requestModel.Vehicle.VehicleIdTypeId == (int)VehicleIdType.SequenceNumber,
                        PolicyHolderNin = mainDriverNin,
                        VehicleId = requestModel.Vehicle.VehicleId
                    };
                    najmResponse = _inquiryUtilities.GetNajmResponse(najmRequest, predefinedLogInfo);

                    if (najmResponse == null || najmResponse.StatusCode != (int)NajmOutput.ErrorCodes.Success)
                    {
                        driverRepository.Update(mainDriverDB);
                    }
                    if (najmResponse == null)
                    {
                        output.ErrorCode = InquiryOutput.ErrorCodes.NajmInsuredResponseError;
                        output.ErrorDescription = NajmExceptionResource.GeneralError;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Najm Response Is null";
                        InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                        return output;
                    }
                    if (najmResponse.StatusCode == (int)NajmOutput.ErrorCodes.CorporatePolicyHolderIDIsNOTAllowed)
                    {
                        output.ErrorCode = InquiryOutput.ErrorCodes.NajmInsuredResponseError;
                        output.ErrorDescription = NajmExceptionResource.CorporatePolicyHolderIDIsNOTAllowed;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "GetNajmResponse return " + najmResponse.ErrorMsg;
                        InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                        return output;
                    }
                    if (najmResponse.StatusCode == (int)NajmOutput.ErrorCodes.InvalidPolicyholderID)
                    {
                        output.ErrorCode = InquiryOutput.ErrorCodes.NajmInsuredResponseError;
                        output.ErrorDescription = NajmExceptionResource.InvalidPolicyholderID;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "GetNajmResponse return " + najmResponse.ErrorMsg;
                        InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                        return output;
                    }
                    if (najmResponse.StatusCode == (int)NajmOutput.ErrorCodes.InvalidVehicleID)
                    {
                        output.ErrorCode = InquiryOutput.ErrorCodes.NajmInsuredResponseError;
                        output.ErrorDescription = NajmExceptionResource.InvalidVehicleID;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "GetNajmResponse return " + najmResponse.ErrorMsg;
                        InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                        return output;
                    }
                    if (najmResponse.StatusCode != (int)NajmOutput.ErrorCodes.Success)
                    {
                        output.ErrorCode = InquiryOutput.ErrorCodes.NajmInsuredResponseError;
                        output.ErrorDescription = NajmExceptionResource.GeneralError;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "GetNajmResponse return " + najmResponse.ErrorMsg;
                        InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                        return output;
                    }
                    mainDriverDB.NCDFreeYears = najmResponse.NCDFreeYears;
                    mainDriverDB.NCDReference = najmResponse.NCDReference;
                }
                else
                {
                    mainDriverDB.NCDFreeYears = 0;
                    mainDriverDB.NCDReference = "0";
                    najmResponse.NCDFreeYearsText = "0";
                    najmResponse.NCDReference = "0";
                }
                driverRepository.Update(mainDriverDB);
                // Insured
                customerYakeenInfo.NIN = requestModel.Insured.NationalId;
                if (!string.IsNullOrEmpty(bankName))
                {
                    customerYakeenInfo.FirstName = bankName;
                }
                else if (!string.IsNullOrEmpty(vehicleYakeenInfo.CarOwnerName))
                {
                    customerYakeenInfo.FirstName = vehicleYakeenInfo.CarOwnerName;
                    customerYakeenInfo.EnglishFirstName = vehicleYakeenInfo.CarOwnerName;
                }

                customerYakeenInfo.SecondName = string.Empty;
                customerYakeenInfo.ThirdName = string.Empty;
                customerYakeenInfo.LastName = string.Empty;
                customerYakeenInfo.EnglishSecondName = string.Empty;
                customerYakeenInfo.EnglishThirdName = string.Empty;
                customerYakeenInfo.EnglishLastName = string.Empty;

                var insured = SaveInsured(customerYakeenInfo, requestModel, addressId);
                DateTime dtNow = DateTime.Now;
                var quotationRequest = new QuotationRequest()
                {
                    ExternalId = qtRqstExtrnlId,
                    MainDriverId = customerYakeenInfo.TameenkId,
                    CityCode = requestModel.CityCode,
                    RequestPolicyEffectiveDate = new DateTime(requestModel.PolicyEffectiveDate.Year,
                    requestModel.PolicyEffectiveDate.Month, requestModel.PolicyEffectiveDate.Day, dtNow.Hour, dtNow.Minute, dtNow.Second),
                    VehicleId = vehicleYakeenInfo.TameenkId,
                    UserId = userID == Guid.Empty ? null : userID.ToString(),
                    NajmNcdRefrence = najmResponse.NCDReference,
                    NajmNcdFreeYears = najmResponse.NCDFreeYears,
                    CreatedDateTime = DateTime.Now,
                    Insured = insured,
                    IsRenewal = requestModel.IsRenewalRequest,
                    PostCode = postCode,
                    PreviousReferenceId = requestModel.PreviousReferenceId,
                    AutoleasingTransactionId = log.RequestId,
                    AutoleasingInitialOption = requestModel.IsInitialOption,
                    AutoleasingBulkOption = requestModel.IsBulkOption,
                    AutoleasingContractDuration = requestModel.ContractDuration
                };
                log.CityCode = Convert.ToInt32(quotationRequest.CityCode);
                log.PolicyEffectiveDate = requestModel.PolicyEffectiveDate;
                log.NajmNcdRefrence = najmResponse.NCDReference;
                log.NajmNcdFreeYears = najmResponse.NCDFreeYears;

                //Add Main Driver to additionalDriversYakeenInfo
                quotationRequest.Drivers = new List<Driver>();
                quotationRequest.Drivers.Add(driversoutput.MainDriver);
                int index = 0;
                if (driversoutput.AdditionalDrivers != null && driversoutput.AdditionalDrivers.Any())
                {
                    foreach (var additionalDriver in driversoutput.AdditionalDrivers)
                    {
                        index++;
                        if (index == 1)
                        {
                            quotationRequest.AdditionalDriverIdOne = additionalDriver.DriverId;
                        }
                        else if (index == 2)
                        {
                            quotationRequest.AdditionalDriverIdTwo = additionalDriver.DriverId;
                        }
                        else if (index == 3)
                        {
                            quotationRequest.AdditionalDriverIdThree = additionalDriver.DriverId;
                        }
                        else if (index == 4)
                        {
                            quotationRequest.AdditionalDriverIdFour = additionalDriver.DriverId;
                        }
                        quotationRequest.Drivers.Add(additionalDriver);
                    }
                }
                quotationRequestRepository.Insert(quotationRequest);
                // for (depreciation / repair method / menimum premium) history
                string handleQuotationRequestSettingsHistoryException = string.Empty;
                short makerCode = 0;
                short modelCode = 0;
                if (requestModel.IsInitialOption)
                {
                    if (requestModel.Vehicle.VehicleMakerCode.HasValue)
                        makerCode = requestModel.Vehicle.VehicleMakerCode.Value;
                    if (requestModel.Vehicle.VehicleModelCode.HasValue)
                        modelCode = (short)requestModel.Vehicle.VehicleModelCode.Value;
                }
                else
                {
                    if (vehicleYakeenInfo.MakerCode.HasValue)
                        makerCode = vehicleYakeenInfo.MakerCode.Value;
                    if (vehicleYakeenInfo.ModelCode.HasValue)
                        modelCode = (short)vehicleYakeenInfo.ModelCode.Value;
                }

                HandleQuotationRequestSettingsHistory(qtRqstExtrnlId, requestModel.Insured?.NationalId, makerCode, modelCode, quotationRequest.Vehicle.VehicleValue.Value, quotationRequest.Vehicle.ModelYear, (int)quotationRequest.Vehicle.VehicleIdType, out handleQuotationRequestSettingsHistoryException);
                if (!string.IsNullOrEmpty(handleQuotationRequestSettingsHistoryException))
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = SubmitInquiryResource.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "HandleQuotationRequestSettingsHistory returned exception: " + handleQuotationRequestSettingsHistoryException;
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }

                if (!requestModel.IsInitialOption)
                {
                    var NCDObject = nCDFreeYearRepository.TableNoTracking.FirstOrDefault(x => x.Code == quotationRequest.Driver.NCDFreeYears);
                    output.InquiryResponseModel.NajmNcd = NCDObject;
                    output.InquiryResponseModel.NajmNcdFreeYears = webApiContext.CurrentLanguage == Tameenk.Core.Domain.Enums.LanguageTwoLetterIsoCode.Ar ? NCDObject?.ArabicDescription : NCDObject?.EnglishDescription;
                }
                output.InquiryResponseModel.QuotationRequestExternalId = qtRqstExtrnlId;
                output.InquiryResponseModel.Vehicle =_inquiryUtilities.ConvertVehicleYakeenToVehicle(vehicleYakeenInfo);
                return output;
            }
            catch (NajmErrorException ex)
            {
                _inquiryUtilities.HandleNajmException(ex, ref output);
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = SubmitInquiryResource.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                return output;
            }
            catch (Exception ex)
            {
                _inquiryUtilities.HandleNajmException(ex, ref output);
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = SubmitInquiryResource.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                return output;
            }
        }
        private void HandleQuotationRequestSettingsHistory(string externalId, string nationalId, short vehicleMakerCode, long vehicleModelCode, int vehicleValue, short? vehicleModelYear,int VehicleIdType, out string exception)
        {
            exception = string.Empty;
            try
            {
                var bankNinData = _bankNinsRepository.TableNoTracking.FirstOrDefault(a => a.NIN.Trim() == nationalId.Trim());
                if (bankNinData != null)
                {
                    #region Depreciation Setting

                    var requestDepreciationSetting = _autoleasingDepreciationSettingRepository.TableNoTracking.FirstOrDefault(a => a.BankId == bankNinData.BankId && a.MakerCode == vehicleMakerCode && a.ModelCode == vehicleModelCode);
                    if (requestDepreciationSetting == null)
                        requestDepreciationSetting = _autoleasingDepreciationSettingRepository.TableNoTracking.FirstOrDefault(a => a.BankId == bankNinData.BankId && a.MakerCode == 0 && a.ModelCode == 0);

                    if (requestDepreciationSetting != null)
                    {
                        var requestDepreciationSettingHistory = new AutoleasingDepreciationSettingHistory()
                        {
                            BankId = bankNinData.BankId,
                            ExternalId = externalId,
                            MakerCode = requestDepreciationSetting.MakerCode,
                            ModelCode = requestDepreciationSetting.ModelCode,
                            MakerName = requestDepreciationSetting.MakerName,
                            ModelName = requestDepreciationSetting.ModelName,
                            Percentage = requestDepreciationSetting.Percentage,
                            IsDynamic = requestDepreciationSetting.IsDynamic,
                            FirstYear = requestDepreciationSetting.FirstYear,
                            SecondYear = requestDepreciationSetting.SecondYear,
                            ThirdYear = requestDepreciationSetting.ThirdYear,
                            FourthYear = requestDepreciationSetting.FourthYear,
                            FifthYear = requestDepreciationSetting.FifthYear,
                            AnnualDepreciationPercentage = requestDepreciationSetting.AnnualDepreciationPercentage,
                            CreatedDate = DateTime.Now
                        };

                        _autoleasingDepreciationSettingRepositoryHistory.Insert(requestDepreciationSettingHistory);
                    }

                    #endregion

                    #region RepairMethod Setting
                    if (vehicleValue >= 200000 && bankNinData.BankId == 4) // Taajeer bank Id
                    {
                        var requestRepairMethodSettingHistory = new AutoleasingAgencyRepairHistory()
                        {
                            BankId = bankNinData.BankId,
                            ExternalId = externalId,
                            FirstYear = "Agency",
                            SecondYear = "Agency",
                            ThirdYear = "Agency",
                            FourthYear = "Agency",
                            FifthYear = "Agency",
                            CreatedDate = DateTime.Now
                        };
                        _autoleasingRepairMethodRepositoryHistory.Insert(requestRepairMethodSettingHistory);
                    }
                    else if (bankNinData.BankId == 2 && vehicleModelYear.HasValue) // Al Yusr bank Id
                    {
                        var firstYear = string.Empty;
                        if (vehicleModelYear >= DateTime.Now.Year - 1)
                        {
                            firstYear = "Agency";
                        }
                        else
                        {
                            firstYear = "Workshop";
                        }
                        var requestRepairMethodSetting = _autoleasingRepairMethodRepository.TableNoTracking.FirstOrDefault(a => a.BankId == bankNinData.BankId);
                        if (requestRepairMethodSetting != null)
                        {
                            var requestRepairMethodSettingHistory = new AutoleasingAgencyRepairHistory()
                            {
                                BankId = bankNinData.BankId,
                                ExternalId = externalId,
                                FirstYear = (VehicleIdType == 1) ? "Workshop" : firstYear,
                                SecondYear = (VehicleIdType == 1) ? "Workshop" : requestRepairMethodSetting.SecondYear,
                                ThirdYear = (VehicleIdType == 1) ? "Workshop" : requestRepairMethodSetting.ThirdYear,
                                FourthYear = (VehicleIdType == 1) ? "Workshop" : requestRepairMethodSetting.FourthYear,
                                FifthYear = (VehicleIdType == 1) ? "Workshop" : requestRepairMethodSetting.FifthYear,
                                CreatedDate = DateTime.Now
                            };
                            _autoleasingRepairMethodRepositoryHistory.Insert(requestRepairMethodSettingHistory);
                        }
                    }
                    else
                    {
                        var requestRepairMethodSetting = _autoleasingRepairMethodRepository.TableNoTracking.FirstOrDefault(a => a.BankId == bankNinData.BankId);
                        if (requestRepairMethodSetting != null)
                        {
                            var requestRepairMethodSettingHistory = new AutoleasingAgencyRepairHistory()
                            {
                                BankId = bankNinData.BankId,
                                ExternalId = externalId,
                                FirstYear = requestRepairMethodSetting.FirstYear,
                                SecondYear = requestRepairMethodSetting.SecondYear,
                                ThirdYear = requestRepairMethodSetting.ThirdYear,
                                FourthYear = requestRepairMethodSetting.FourthYear,
                                FifthYear = requestRepairMethodSetting.FifthYear,
                                CreatedDate = DateTime.Now
                            };

                            _autoleasingRepairMethodRepositoryHistory.Insert(requestRepairMethodSettingHistory);
                        }
                    }
                    #endregion

                    #region MinimumPremium Setting

                    var requestMinimumPremiumSetting = _autoleasingMinimumPremiumRepository.TableNoTracking.FirstOrDefault(a => a.BankId == bankNinData.BankId);
                    if (requestMinimumPremiumSetting != null)
                    {
                        var requestMinimumPremiumSettingHistory = new AutoleasingMinimumPremiumHistory()
                        {
                            BankId = bankNinData.BankId,
                            ExternalId = externalId,
                            FirstYear = requestMinimumPremiumSetting.FirstYear,
                            SecondYear = requestMinimumPremiumSetting.SecondYear,
                            ThirdYear = requestMinimumPremiumSetting.ThirdYear,
                            FourthYear = requestMinimumPremiumSetting.FourthYear,
                            FifthYear = requestMinimumPremiumSetting.FifthYear,
                            CreatedDate = DateTime.Now
                        };

                        _autoleasingMinimumPremiumRepositoryHistory.Insert(requestMinimumPremiumSettingHistory);
                    }

                    #endregion
                }

                else
                    exception = "No bank data foun for NIN = " + nationalId;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
            }
        }

        private void HandleConvertInitialQuotationRequestSettingsHistory(string externalId, string initialExternalId, out string exception)
        {
            exception = string.Empty;
            try
            {
                #region Depreciation Setting
                var initialDepreciationSetting = _autoleasingDepreciationSettingRepositoryHistory.TableNoTracking.FirstOrDefault(a => a.ExternalId == initialExternalId);

                if (initialDepreciationSetting != null)
                {
                    var requestDepreciationSettingHistory = new AutoleasingDepreciationSettingHistory()
                    {
                        BankId = initialDepreciationSetting.BankId,
                        ExternalId = externalId,
                        MakerCode = initialDepreciationSetting.MakerCode,
                        ModelCode = initialDepreciationSetting.ModelCode,
                        MakerName = initialDepreciationSetting.MakerName,
                        ModelName = initialDepreciationSetting.ModelName,
                        Percentage = initialDepreciationSetting.Percentage,
                        IsDynamic = initialDepreciationSetting.IsDynamic,
                        FirstYear = initialDepreciationSetting.FirstYear,
                        SecondYear = initialDepreciationSetting.SecondYear,
                        ThirdYear = initialDepreciationSetting.ThirdYear,
                        FourthYear = initialDepreciationSetting.FourthYear,
                        FifthYear = initialDepreciationSetting.FifthYear,
                        AnnualDepreciationPercentage = initialDepreciationSetting.AnnualDepreciationPercentage,
                        CreatedDate = DateTime.Now
                    };

                    _autoleasingDepreciationSettingRepositoryHistory.Insert(requestDepreciationSettingHistory);
                }

                #endregion

                #region RepairMethod Setting
                var initialRepairMethodSetting = _autoleasingRepairMethodRepositoryHistory.TableNoTracking.FirstOrDefault(a => a.ExternalId == initialExternalId);
                if (initialRepairMethodSetting != null)
                {
                    var requestRepairMethodSettingHistory = new AutoleasingAgencyRepairHistory()
                    {
                        BankId = initialRepairMethodSetting.BankId,
                        ExternalId = externalId,
                        FirstYear = initialRepairMethodSetting.FirstYear,
                        SecondYear = initialRepairMethodSetting.SecondYear,
                        ThirdYear = initialRepairMethodSetting.ThirdYear,
                        FourthYear = initialRepairMethodSetting.FourthYear,
                        FifthYear = initialRepairMethodSetting.FifthYear,
                        CreatedDate = DateTime.Now
                    };

                    _autoleasingRepairMethodRepositoryHistory.Insert(requestRepairMethodSettingHistory);
                }
                #endregion

                #region MinimumPremium Setting
                var initialMinimumPremiumSetting = _autoleasingMinimumPremiumRepositoryHistory.TableNoTracking.FirstOrDefault(a => a.ExternalId == initialExternalId);
                if (initialMinimumPremiumSetting != null)
                {
                    var requestMinimumPremiumSettingHistory = new AutoleasingMinimumPremiumHistory()
                    {
                        BankId = initialMinimumPremiumSetting.BankId,
                        ExternalId = externalId,
                        FirstYear = initialMinimumPremiumSetting.FirstYear,
                        SecondYear = initialMinimumPremiumSetting.SecondYear,
                        ThirdYear = initialMinimumPremiumSetting.ThirdYear,
                        FourthYear = initialMinimumPremiumSetting.FourthYear,
                        FifthYear = initialMinimumPremiumSetting.FifthYear,
                        CreatedDate = DateTime.Now
                    };

                    _autoleasingMinimumPremiumRepositoryHistory.Insert(requestMinimumPremiumSettingHistory);
                }
                #endregion
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
            }
        }
        public void InsertAutoleasingSelectedBenifits(Guid parentRequestId, string ExternalId, List<short> benifitsIds)
        {
            List<AutoleasingSelectedBenifits> autoleasingSelectedBenifits = new List<AutoleasingSelectedBenifits>();
            AutoleasingSelectedBenifitsOutput output = new AutoleasingSelectedBenifitsOutput();
            foreach (var benifitId in benifitsIds)
            {
                autoleasingSelectedBenifits.Add(new AutoleasingSelectedBenifits() { ParentRequestId = parentRequestId, ExternalId = ExternalId, BenifitId = benifitId });
            }
            _autoleasingSelectedBenifitsRepository.Insert(autoleasingSelectedBenifits);
        }

        public ValidationOutput AutoleasingValidateData(InquiryRequestModel requestModel, InquiryRequestLog log, string channel)
        {
            ValidationOutput output = new ValidationOutput();
            output.Log = log;
            try
            {
                if (requestModel == null)
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.RequestModelIsNullException;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "requestModel Is null";
                    output.Log = log;
                    return output;
                }
                if (requestModel.Vehicle.TransmissionTypeId == 0)
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.TransmissionTypeIdIsNull;
                    output.ErrorDescription = SubmitInquiryResource.TransmissionTypeIdIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "TransmissionTypeId is zero"; ;
                    output.Log = log;
                    return output;
                }
                if (!requestModel.Vehicle.MileageExpectedAnnualId.HasValue || requestModel.Vehicle.MileageExpectedAnnualId == 0)
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.MileageExpectedAnnualIdIsNull;
                    output.ErrorDescription = SubmitInquiryResource.MileageExpectedAnnualIdIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "MileageExpectedAnnualId is zero"; ;
                    output.Log = log;
                    return output;
                }
                if (requestModel.Vehicle.ParkingLocationId == 0)
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.ParkingLocationIdIsNull;
                    output.ErrorDescription = SubmitInquiryResource.ParkingLocationIdIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "ParkingLocationId is zero"; ;
                    output.Log = log;
                    return output;
                }
                int vehicleValue = 0;
                string comingValue = requestModel.Vehicle.ApproximateValue.ToString();
                if (requestModel.Vehicle.ApproximateValue.ToString().Contains("."))
                {
                    string value = requestModel.Vehicle.ApproximateValue.ToString().Split('.')[0];
                    int.TryParse(value, out vehicleValue);
                    requestModel.Vehicle.ApproximateValue = vehicleValue;
                }
                if (requestModel.Vehicle.ApproximateValue <= 0)
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.VehicleValueZero;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Vehicle value can't be zero requestModel.Vehicle.ApproximateValue is 0 as we recive " + comingValue;
                    output.Log = log;
                    return output;
                }
                if (!int.TryParse(requestModel.Vehicle.ApproximateValue.ToString(), out vehicleValue))
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.VehicleValueInvalid;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "requestModel.Vehicle.ApproximateValue is invalid as we recive " + requestModel.Vehicle.ApproximateValue.ToString();
                    output.Log = log;
                    return output;
                }
                if (requestModel.Vehicle.ApproximateValue < 10000) //this condition addded by Fayssal 
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.VehicleValueLessThan10K;
                    output.ErrorDescription = SubmitInquiryResource.VehicleValueLessThan10K;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Vehicle value less than 10000 as we recived " + requestModel.Vehicle.ApproximateValue;
                    output.Log = log;
                    return output;
                }
                if (requestModel.Vehicle.OwnerTransfer && requestModel.Vehicle.OwnerNationalId == requestModel.Insured.NationalId)
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.OwnerNationalIdAndNationalIdAreEqual;
                    output.ErrorDescription = SubmitInquiryResource.InvaildOldOwnerNin;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "OwnerTransfer and Vehicle.OwnerNationalId and Insured.NationalId are the same";
                    output.Log = log;
                    return output;
                }

                if (requestModel.PolicyEffectiveDate < DateTime.Now.Date.AddDays(1) || requestModel.PolicyEffectiveDate > DateTime.Now.AddDays(30))
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.InvalidPolicyEffectiveDate;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Policy effective date should be within 30 days starts from toworrow as we received " + requestModel.PolicyEffectiveDate.ToString();
                    output.Log = log;
                    return output;
                }
                long mainDriverNin = 0;
                //Driver Validations 
                DriverModel mainDriver = null;
                mainDriver = requestModel.Drivers.Where(a => a.IsCompanyMainDriver).FirstOrDefault();
                mainDriverNin = long.Parse(mainDriver.NationalId);
                if (mainDriver == null)
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.RequestModelIsNullException;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Main Driver Not Exist";
                    output.Log = log;
                    return output;
                }
                requestModel.Insured.BirthDateMonth = mainDriver.BirthDateMonth;
                requestModel.Insured.BirthDateYear = mainDriver.BirthDateYear;
                if (requestModel.Insured.BirthDateMonth == 0 || requestModel.Insured.BirthDateYear == 0)
                {
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "BirthDate can't be zero month:" + mainDriver.BirthDateMonth + " Year:" + mainDriver.BirthDateYear;
                    output.Log = log;
                    output.ErrorDescription = SubmitInquiryResource.ErrorGeneric;
                    return output;
                }
                output.DriverNin = mainDriverNin;
                output.VehicleId = requestModel?.Vehicle?.VehicleId.ToString();
                //validation added as per fayssal 
                if (mainDriver.EducationId == 0)
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.EducationIdIsNull;
                    output.ErrorDescription = SubmitInquiryResource.EducationIdIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "main driver EducationId is zero"; ;
                    output.Log = log;
                    return output;
                }
                if (mainDriver.MedicalConditionId == 0)
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.MedicalConditionIdIsNull;
                    output.ErrorDescription = SubmitInquiryResource.MedicalConditionIdIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "main driver MedicalConditionId is zero"; ;
                    output.Log = log;
                    return output;
                }
                if (!mainDriver.DriverNOALast5Years.HasValue)
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.DriverNOALast5YearsIsNull;
                    output.ErrorDescription = SubmitInquiryResource.DriverNOALast5YearsIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "DriverNOALast5Years is zero"; ;
                    output.Log = log;
                    return output;
                }
                int additionalDrivingPercentage = 0;
                foreach (var driver in requestModel.Drivers)
                {
                    if (driver.NationalId.StartsWith("7"))
                        continue;
                    if (driver.NationalId == mainDriver.NationalId)
                        continue;
                    if (driver.EducationId == 0)
                    {
                        output.ErrorCode = ValidationOutput.ErrorCodes.EducationIdIsNullAdditionalDriver;
                        output.ErrorDescription = SubmitInquiryResource.EducationIdIsNullAdditionalDriver.Replace("{0}", driver.NationalId);
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Additional Driver EducationId is zero for Additional Driver " + driver.NationalId;
                        output.Log = log;
                        return output;
                    }
                    if (driver.MedicalConditionId == 0)
                    {
                        output.ErrorCode = ValidationOutput.ErrorCodes.MedicalConditionIdIsNullAdditionalDriver;
                        output.ErrorDescription = SubmitInquiryResource.MedicalConditionIdIsNullAdditionalDriver.Replace("{0}", driver.NationalId);
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Additional Driver MedicalConditionId is zero for Additional Driver " + driver.NationalId;
                        output.Log = log;
                        return output;
                    }
                    if (!driver.DriverNOALast5Years.HasValue)
                    {
                        output.ErrorCode = ValidationOutput.ErrorCodes.DriverNOALast5YearsIsNullAdditionalDriver;
                        output.ErrorDescription = SubmitInquiryResource.DriverNOALast5YearsIsNullAdditionalDriver.Replace("{0}", driver.NationalId);
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Additional Driver DriverNOALast5Years is zero for Additional Driver " + driver.NationalId;
                        output.Log = log;
                        return output;
                    }
                    additionalDrivingPercentage += driver.DrivingPercentage;

                }
                if (additionalDrivingPercentage > 100)
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.DrivingPercentageMoreThan100;
                    output.ErrorDescription = SubmitInquiryResource.DrivingPercentageErrorInvalid;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Driving Percentage exceeded 100% for Additional Drivers as it's with total " + additionalDrivingPercentage;
                    output.Log = log;
                    return output;
                }
                int remainingPercentage = 100 - additionalDrivingPercentage;
                requestModel.Drivers.Remove(mainDriver);
                mainDriver.DrivingPercentage = remainingPercentage >= 0 ? remainingPercentage : 0;
                requestModel.Drivers.Insert(0, mainDriver);

                if (additionalDrivingPercentage + mainDriver.DrivingPercentage != 100)
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.DrivingPercentageMoreThan100;
                    output.ErrorDescription = SubmitInquiryResource.DrivingPercentageErrorInvalid;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "DrivingPercentage is not 100% as additionalDrivingPercentage is " + additionalDrivingPercentage + " and main is " + mainDriver.DrivingPercentage;
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }

                output.ErrorCode = ValidationOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.Log = log;
                output.RequestModel = requestModel;
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = ValidationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = SubmitInquiryResource.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = exp.ToString();
                output.Log = log;
                return output;
            }
        }

        public InquiryOutput ConvertInitialQuotation(string externalId, InquiryRequestModel requestModel, Guid userId, string userName)
        {
            InquiryOutput output = new InquiryOutput();
            InquiryRequestLog log = new InquiryRequestLog();
            log.Channel = Channel.autoleasing.ToString();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.UserId = userId.ToString();
            log.UserName = userName;
            log.MethodName = "ConvertInitialQuotation";
            log.NIN = requestModel.Insured.NationalId;
            log.VehicleId = requestModel.Vehicle.VehicleId.ToString();
            log.RequestId = requestModel.ParentRequestId;
            log.ServiceRequest = JsonConvert.SerializeObject(requestModel);
            log.ExternalId = externalId;
            CultureInfo cultureInfo = CultureInfo.GetCultureInfo(requestModel.Language == null ? "en" : requestModel.Language);
            SubmitInquiryResource.Culture = cultureInfo;
            string exception = string.Empty;
            ServiceRequestLog predefinedLogInfo = new ServiceRequestLog
            {
                UserID = userId,
                UserName = userName,
                Channel = Channel.autoleasing.ToString(),
                ServerIP = Utilities.GetInternalServerIP()
            };
            predefinedLogInfo.RequestId = log.RequestId;
            predefinedLogInfo.VehicleId = requestModel?.Vehicle?.VehicleId.ToString();
            try
            {
                if (requestModel == null)
                {
                    output.ErrorCode = (InquiryOutput.ErrorCodes)ValidationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.ResourceManager.GetString($"RequestModelIsNullException", CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.Name));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "requestModel Is null";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                if (requestModel.Vehicle.TransmissionTypeId == 0)
                {
                    output.ErrorCode = (InquiryOutput.ErrorCodes)ValidationOutput.ErrorCodes.TransmissionTypeIdIsNull;
                    output.ErrorDescription = SubmitInquiryResource.ResourceManager.GetString($"TransmissionTypeIdIsNull", CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.Name));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "TransmissionTypeId is zero";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;

                }
                if (!requestModel.Vehicle.MileageExpectedAnnualId.HasValue || requestModel.Vehicle.MileageExpectedAnnualId == 0)
                {
                    output.ErrorCode = (InquiryOutput.ErrorCodes)ValidationOutput.ErrorCodes.MileageExpectedAnnualIdIsNull;
                    output.ErrorDescription = SubmitInquiryResource.ResourceManager.GetString($"MileageExpectedAnnualIdIsNull", CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.Name));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "MileageExpectedAnnualId is zero";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                if (requestModel.Vehicle.ParkingLocationId == 0)
                {
                    output.ErrorCode = (InquiryOutput.ErrorCodes)ValidationOutput.ErrorCodes.ParkingLocationIdIsNull;
                    output.ErrorDescription = SubmitInquiryResource.ResourceManager.GetString($"ParkingLocationIdIsNull", CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.Name));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "ParkingLocationId is zero";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                VehicleYakeenModel vehicleYakeenInfo = _inquiryUtilities.GetVehicleYakeenInfo(requestModel, predefinedLogInfo);
                if (vehicleYakeenInfo == null)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString($"SerivceIsCurrentlyDown", CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.Name));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "GetVehicleYakeenInfo returned null";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                if (vehicleYakeenInfo != null && vehicleYakeenInfo.Error != null && vehicleYakeenInfo.Error.ErrorCode == "0")
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString($"SerivceIsCurrentlyDown", CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.Name));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "GetVehicleYakeenInfo returned " + vehicleYakeenInfo.Error.ErrorDescription;
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                if (!vehicleYakeenInfo.Success)
                {
                    if (vehicleYakeenInfo.Error != null && vehicleYakeenInfo.Error.ErrorMessage != null
                        && ((vehicleYakeenInfo.Error.Type == EErrorType.YakeenError ||
                        (vehicleYakeenInfo.Error.Type == EErrorType.LocalError && RepositoryConstants.ShowLocalErrorDetailsInResponse))))
                    {
                        string ErrorMessage = SubmitInquiryResource.ResourceManager.GetString($"YakeenError_{vehicleYakeenInfo.Error?.ErrorCode}", CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.Name));
                        var GenericErrorMessage = !string.IsNullOrEmpty(ErrorMessage) ? ErrorMessage : SubmitInquiryResource.YakeenError_100;
                        output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                        output.ErrorDescription = GenericErrorMessage;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = $"GetVehicleYakeenInfo returned code:{vehicleYakeenInfo.Error?.ErrorCode} | message:{vehicleYakeenInfo.Error.ErrorMessage} | description: {vehicleYakeenInfo.Error.ErrorDescription}";
                    }
                    else
                    {
                        output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                        output.ErrorDescription = SubmitInquiryResource.ResourceManager.GetString($"ErrorGeneric", CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.Name));
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Service Exception for GetVehicleYakeenInfo";
                    }
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }

                var intialQuotationInfo = quotationRequestRepository.TableNoTracking.Where(x => x.ExternalId == externalId).FirstOrDefault();
                if (intialQuotationInfo == null)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = SubmitInquiryResource.ResourceManager.GetString($"ErrorGeneric", CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.Name));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "intialQuotationInfo is null";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }

                int initialQuotationId = intialQuotationInfo.ID;
                //update intial quotation with vehicle data and set intial option = true
                output.InquiryResponseModel = new InquiryResponseModel();
                string newExternalId = _inquiryUtilities.GetNewRequestExternalId();
                output.InquiryResponseModel.QuotationRequestExternalId = newExternalId;
                output.InquiryResponseModel.Vehicle = _inquiryUtilities.ConvertVehicleYakeenToVehicle(vehicleYakeenInfo);
                intialQuotationInfo.ID = 0;
                intialQuotationInfo.InitialExternalId = externalId;
                intialQuotationInfo.ExternalId = newExternalId;
                //intialQuotationInfo.AutoleasingInitialOption = false;
                intialQuotationInfo.ShowInitial = false;
                intialQuotationInfo.UserId = userId.ToString();
                //update intial quotation with vehicle data and set intial option = true
                intialQuotationInfo.VehicleId = vehicleYakeenInfo.TameenkId;
                //intialQuotationInfo.IsConverted = true;
                intialQuotationInfo.RequestPolicyEffectiveDate = DateTime.Now.AddDays(1);
                quotationRequestRepository.Insert(intialQuotationInfo);
                //var response = _autoleasingQuotationResponseCache.Table.Where(x => x.ExternalId == externalId).ToList();
                //if (response != null)
                //{
                //    _autoleasingQuotationResponseCache.Delete(response);
                //}


                exception = string.Empty;
                var resultCount = _inquiryUtilities.CopyAdditionalDriversToNewQuotationRequest(intialQuotationInfo.ID, initialQuotationId, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.ServiceDown;
                    log.ErrorCode = (int)InquiryOutput.ErrorCodes.ServiceDown;
                    log.ErrorDescription = $"ConvertInitialQuotation --> CopyAdditionalDriversToNewQuotationRequest error : {exception}";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }

                var handleYakeenMissingFieldsResult = _inquiryUtilities.HandleYakeenMissingFields(output, "");
                //if (!handleYakeenMissingFieldsResult.InquiryResponseModel.IsValidInquiryRequest)
                //{
                //    output.ErrorCode = InquiryOutput.ErrorCodes.MissingFields;
                //    log.ErrorCode = (int)InquiryOutput.ErrorCodes.MissingFields;
                //    log.ErrorDescription = "ConvertInitialQuotation --> Missing Fields";
                //    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                //    return output;
                //}

                exception = string.Empty;
                //HandleConvertInitialQuotationRequestSettingsHistory(intialQuotationInfo.ExternalId, intialQuotationInfo.InitialExternalId, requestModel.Insured?.NationalId,
                //    vehicleYakeenInfo.MakerCode.Value, vehicleYakeenInfo.ModelCode.Value, vehicleYakeenInfo.Value.Value, out exception);
                HandleConvertInitialQuotationRequestSettingsHistory(intialQuotationInfo.ExternalId, intialQuotationInfo.InitialExternalId, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = SubmitInquiryResource.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Convert Initial Quotation --> HandleQuotationRequestSettingsHistory returned exception: " + exception;
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }

                string clearText = externalId + "_" + newExternalId + "_"  + SecurityUtilities.HashKey;
                string hashed = SecurityUtilities.HashData(clearText, null);
                output.InquiryResponseModel.HashedValue = hashed;

                output.ErrorCode = InquiryOutput.ErrorCodes.Success;
                output.ErrorDescription = SubmitInquiryResource.ResourceManager.GetString($"Success", CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.Name));
                log.ErrorCode = (int)InquiryOutput.ErrorCodes.Success;
                log.ErrorDescription = "Success";
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString($"ErrorGeneric", CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.Name));
                log.ErrorCode = (int)InquiryOutput.ErrorCodes.ServiceException;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                return output;
            }
        }

        public InquiryOutput AutoleasingSubmitMissingFeilds(YakeenMissingInfoRequestModel model, string userId, string userName)
        {
            InquiryResponseModel result = new InquiryResponseModel
            {
                QuotationRequestExternalId = model.QuotationRequestExternalId
            };
            InquiryOutput output = new InquiryOutput();
            InquiryRequestLog log = new InquiryRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.Channel = Channel.autoleasing.ToString();
            log.UserId = userId.ToString();
            log.UserName = userName;
            log.MethodName = "SubmitMissingFields";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            try
            {
                if (model == null)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model is null";
                    result.IsValidInquiryRequest = false;
                    output.InquiryResponseModel = result;
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                log.ExternalId = model.QuotationRequestExternalId;
                if (model.YakeenMissingFields.VehicleMakerCode == null && ((!string.IsNullOrEmpty(model.YakeenMissingFields.VehicleMaker) && model.YakeenMissingFields.VehicleMaker != "غير متوفر")))
                {
                    model.YakeenMissingFields.VehicleMakerCode = int.Parse(model.YakeenMissingFields.VehicleMaker);
                }
                if (model.YakeenMissingFields.VehicleModelCode == null && ((!string.IsNullOrEmpty(model.YakeenMissingFields.VehicleModel) && model.YakeenMissingFields.VehicleModel != "غير متوفر")))
                {
                    model.YakeenMissingFields.VehicleModelCode = int.Parse(model.YakeenMissingFields.VehicleModel);
                }
                if (model.YakeenMissingFields.VehicleMakerCode.HasValue && model.YakeenMissingFields.VehicleMakerCode.Value > 0)
                {
                    model.YakeenMissingFields.VehicleMaker = model.YakeenMissingFields.VehicleMakerCode.Value.ToString();
                }
                var quotationRequest = _inquiryUtilities.GetQuotationRequest(model.QuotationRequestExternalId);
                if (quotationRequest == null)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "quotationRequest is null";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    result.IsValidInquiryRequest = false;
                    output.InquiryResponseModel = result;
                    return output;
                }
                log.NIN = quotationRequest.Driver.NIN;
                bool isCustomCard = false;
                if (quotationRequest.Vehicle.VehicleIdType == VehicleIdType.CustomCard)
                {
                    isCustomCard = true;
                    log.VehicleId = quotationRequest.Vehicle.CustomCardNumber;

                }
                else
                {
                    log.VehicleId = quotationRequest.Vehicle.SequenceNumber;
                }

                var quotationRequiredFieldsModel = quotationRequest.ToQuotationRequestRequiredFieldsModel();
                if (quotationRequiredFieldsModel.VehicleModel == "غير متوفر" || string.IsNullOrEmpty(quotationRequiredFieldsModel.VehicleModel))
                {
                    quotationRequiredFieldsModel.VehicleModel = null;
                }
                if (quotationRequiredFieldsModel.VehicleMaker == "غير متوفر" || string.IsNullOrEmpty(quotationRequiredFieldsModel.VehicleMaker))
                {
                    quotationRequiredFieldsModel.VehicleMaker = null;
                }
                if (quotationRequiredFieldsModel.VehicleModelCode == null || quotationRequiredFieldsModel.VehicleModelCode.Value < 1)
                {
                    quotationRequiredFieldsModel.VehicleModelCode = null;
                }
                if (quotationRequiredFieldsModel.VehicleMakerCode == null || quotationRequiredFieldsModel.VehicleMakerCode.Value < 1)
                {
                    quotationRequiredFieldsModel.VehicleMakerCode = null;
                }
                if (quotationRequiredFieldsModel.VehicleBodyCode == null || quotationRequiredFieldsModel.VehicleBodyCode.Value < 1 || quotationRequiredFieldsModel.VehicleBodyCode.Value > 21)
                {
                    quotationRequiredFieldsModel.VehicleBodyCode = null;
                }
                if (quotationRequiredFieldsModel.VehicleLoad == null || quotationRequiredFieldsModel.VehicleLoad.Value < 1)
                {
                    quotationRequiredFieldsModel.VehicleLoad = null;
                }
                if (model.YakeenMissingFields.VehicleModel == null && model.YakeenMissingFields.VehicleModelCode.HasValue)
                {
                    model.YakeenMissingFields.VehicleModel = model.YakeenMissingFields.VehicleModelCode.Value.ToString();
                }
                var missingPropertiesNames = _inquiryUtilities.GetYakeenMissingPropertiesName(quotationRequiredFieldsModel, isCustomCard);
                if (missingPropertiesNames == null || missingPropertiesNames.Count() == 0)
                {
                    var quotationRequestInsuredNIN = quotationRequest.Insured?.NationalId;
                    var quotationRequestVehicleMakerCode = quotationRequest.Vehicle.VehicleMakerCode.Value;
                    var quotationRequestVehicleModelCode = quotationRequest.Vehicle.VehicleModelCode.Value;
                    var quotationRequestVehicleValue = quotationRequest.Vehicle.VehicleValue.Value;
                    string exception = string.Empty;
                    HandleConvertInitialQuotationRequestSettingsHistory(quotationRequest.ExternalId, quotationRequest.InitialExternalId, out exception);
                    if (!string.IsNullOrEmpty(exception))
                    {
                        output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                        output.ErrorDescription = SubmitInquiryResource.ErrorGeneric;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Convert Initial Quotation --> HandleQuotationRequestSettingsHistory returned exception: " + exception;
                        InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                        return output;
                    }

                    output.ErrorCode = InquiryOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Success";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    result.IsValidInquiryRequest = true;
                    output.InquiryResponseModel = result;
                    return output;
                }
                if (!_inquiryUtilities.IsUserEnteredAllYakeenMissingFields(model.YakeenMissingFields, missingPropertiesNames))
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.StillMissed;
                    output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User didn't entered all required data";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    result = _inquiryUtilities.CheckYakeenMissingFields(result, quotationRequiredFieldsModel, isCustomCard); ;
                    output.InquiryResponseModel = result;
                    return output;
                }
                var userData = model.YakeenMissingFields;
                List<YakeenMissingField> missingFields = new List<YakeenMissingField>();
                foreach (var propertyName in missingPropertiesNames)
                {
                    YakeenMissingField field = new YakeenMissingField();
                    var userValue = userData.GetType().GetProperty(propertyName).GetValue(userData);
                    string value = userValue.ToString();
                    if (propertyName == "VehicleMaker")
                    {
                        int.TryParse(userValue.ToString(), out int makerCode);
                        var makerName = vehicleService.GetMakerName(makerCode, "");
                        value = makerName;
                        quotationRequiredFieldsModel.GetType().GetProperty(propertyName).SetValue(quotationRequiredFieldsModel, makerName);
                    }
                    else if (propertyName == "VehicleModel")
                    {
                        int makerCode = 0;
                        if (!quotationRequiredFieldsModel.VehicleMakerCode.HasValue || quotationRequiredFieldsModel.VehicleMakerCode.Value < 1)
                        {
                            var maker = userData.GetType().GetProperty("VehicleMaker").GetValue(userData);
                            int.TryParse(maker.ToString(), out makerCode);
                        }
                        else
                        {
                            makerCode = quotationRequiredFieldsModel.VehicleMakerCode.Value;
                        }
                        //userValue = userData.GetType().GetProperty(propertyName).GetValue(userData);
                        int modelCode = 0;
                        int.TryParse(userValue as string, out modelCode);
                        var modelName = vehicleService.GetModelName(modelCode, short.Parse(makerCode.ToString()), "");
                        value = modelName;
                        quotationRequiredFieldsModel.GetType().GetProperty(propertyName).SetValue(quotationRequiredFieldsModel, modelName);
                    }
                    else
                    {
                        quotationRequiredFieldsModel.GetType().GetProperty(propertyName).SetValue(quotationRequiredFieldsModel, userValue);
                    }
                    field.Key = propertyName;
                    field.Value = value;
                    missingFields.Add(field);
                }
                var updatedQuotationRequest = quotationRequiredFieldsModel.ToEntity(quotationRequest);
                updatedQuotationRequest.ManualEntry = true;
                if (missingFields.Count > 0)
                    updatedQuotationRequest.MissingFields = JsonConvert.SerializeObject(missingFields);
                quotationRequestRepository.Update(updatedQuotationRequest);

                var insuredNIN = updatedQuotationRequest.Insured?.NationalId;
                var vehicleMakerCode = updatedQuotationRequest.Vehicle.VehicleMakerCode.Value;
                var vehicleModelCode = updatedQuotationRequest.Vehicle.VehicleModelCode.Value;
                var vehicleValue = updatedQuotationRequest.Vehicle.VehicleValue.Value;
                string exp = string.Empty;
                HandleConvertInitialQuotationRequestSettingsHistory(updatedQuotationRequest.ExternalId, updatedQuotationRequest.InitialExternalId, out exp);
                if (!string.IsNullOrEmpty(exp))
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = SubmitInquiryResource.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Convert Initial Quotation --> HandleQuotationRequestSettingsHistory returned exception: " + exp;
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }

                result.IsValidInquiryRequest = true;
                output.ErrorCode = InquiryOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                result.IsValidInquiryRequest = true;
                output.InquiryResponseModel = result;
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = SubmitInquiryResource.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = exp.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                return output;
            }
        }
        public OutPutModel UploadExcel(byte[] bytes, string channel, string lang, Guid userId, string userName)
        {
            OutPutModel output = new OutPutModel();
            output.SuccessList = new List<InquiryRequestModel>();
            output.FailedList = new List<FailModel>();

            InquiryRequestLog log = new InquiryRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.Channel = channel;
            log.UserId = userId.ToString();
            log.UserName = userName;
            log.MethodName = "UploadExcel";

            if (bytes.Length == 0)
            {
                output.ErrorCode = (int)OutPutModel.ErrorCodes.ExcelIsNull;
                output.ErrorDescription = AutoLeasingResources.EmptyFile;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Excel File Is empty";
                return output;
            }

            try
            {
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    // Write the data to the stream, byte by byte.
                    for (int i = 0; i < bytes.Length; i++)
                        stream.WriteByte(bytes[i]);

                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (ExcelPackage excelPackage = new ExcelPackage(stream))
                    {
                        List<InquiryRequestModel> successList = new List<InquiryRequestModel>();
                        List<FailModel> fialedList = new List<FailModel>();

                        foreach (var worksheet in excelPackage.Workbook.Worksheets)
                        {
                            if (worksheet.Dimension == null || worksheet.Dimension.Rows < 2)
                            {
                                output.ErrorCode = (int)OutPutModel.ErrorCodes.ExcelIsNull;
                                output.ErrorDescription = AutoLeasingResources.EmptyFile;
                                log.ErrorCode = (int)output.ErrorCode;
                                log.ErrorDescription = "Excel File Is empty";
                                return output;
                            }

                            int rowCount = worksheet.Dimension.Rows;
                            int ColCount = worksheet.Dimension.Columns;

                            //loop all rows
                            StringBuilder rowErrorDescription = new StringBuilder();
                            for (int row = 2; row <= rowCount; row++)
                            {
                                var failedObj = new FailModel();
                                var successObj = new InquiryRequestModel() { PolicyEffectiveDate = DateTime.Now.AddDays(1), CityCode = 1 };
                                successObj.Insured = new InsuredModel()
                                {
                                    ChildrenBelow16Years = 0,
                                    EducationId = 0
                                };

                                successObj.Drivers = new List<DriverModel>();
                                DriverModel insuredDriver = new DriverModel
                                {
                                    ChildrenBelow16Years = 0,
                                    DriverNOALast5Years = 0,
                                    DrivingPercentage = 0,
                                    EducationId = 4,
                                    MedicalConditionId = 1,
                                    RelationShipId = 0
                                };
                                successObj.Drivers.Add(insuredDriver);
                                DriverModel mainDriver = new DriverModel
                                {
                                    IsCompanyMainDriver = true,
                                    ChildrenBelow16Years = 0,
                                    DriverNOALast5Years = 0,
                                    DrivingPercentage = 100,
                                    EducationId = 4,
                                    MedicalConditionId = 1
                                };
                                successObj.Drivers.Add(mainDriver);
                                DriverModel additionalDriver1 = new DriverModel
                                {
                                    ChildrenBelow16Years = 0,
                                    DriverNOALast5Years = 0,
                                    DrivingPercentage = 0,
                                    EducationId = 4,
                                    MedicalConditionId = 1,
                                    RelationShipId = 0
                                };
                                DriverModel additionalDriver2 = new DriverModel
                                {
                                    ChildrenBelow16Years = 0,
                                    DriverNOALast5Years = 0,
                                    DrivingPercentage = 0,
                                    EducationId = 4,
                                    MedicalConditionId = 1,
                                    RelationShipId = 0
                                };

                                successObj.Vehicle = new VehicleModel()
                                {
                                    TransmissionTypeId = 2,
                                    MileageExpectedAnnualId = 1,
                                    ParkingLocationId = 1,
                                    HasModification = false,
                                    OwnerTransfer = false
                                };
                                successObj.Benfits = new List<int>();
                                successObj.VehicleAgencyRepair = true;

                                if (!string.IsNullOrEmpty(rowErrorDescription.ToString()))
                                    rowErrorDescription.Append("\n");

                                if (EmptyRow(worksheet, row, ColCount))
                                {
                                    failedObj.RowNumber = row;
                                    failedObj.ErrorDescription = "Row is empty";
                                    output.FailedList.Add(failedObj);
                                    break;
                                }

                                int emptycol = 0;
                                int probCount = 0;
                                //loop all columns in a row
                                for (int col = 1; col <= ColCount; col++)
                                {
                                    if (probCount == ColCount)
                                        break;

                                    var invalidCellException = string.Empty;
                                    var cellName = worksheet.Cells[1, col, 1, col].Text?.Trim();
                                    var cellValue = worksheet.Cells[row, col].Value;
                                    if (worksheet.Cells[row, col].Value != null)
                                    {
                                        if (!ValidateNationalIdAndExpiryDate(cellName, cellValue, out invalidCellException))
                                        {
                                            //if (col == 1)
                                            //    rowErrorDescription.Append("Row number " + row + " \n");
                                            rowErrorDescription.AppendLine("cell: " + cellName + " is invalid, and the error is : " + invalidCellException + " \n");
                                            break;
                                        }

                                        switch (cellName)
                                        {
                                            case "InsuredID":
                                                successObj.Insured.NationalId = cellValue.ToString();
                                                insuredDriver.NationalId = cellValue.ToString();
                                                break;

                                            case "MainDriverMobileNo":
                                                mainDriver.MobileNo = cellValue.ToString();
                                                break;

                                            case "MainDriverID":
                                                mainDriver.NationalId = cellValue.ToString();
                                                break;

                                            case "MainDriverDOB":
                                                var mainDriverBirthMonth = cellValue.ToString().Split('/')[0];
                                                var mainDriverBirthYear = cellValue.ToString().Split('/')[1];
                                                mainDriver.BirthDateMonth = Convert.ToByte(mainDriverBirthMonth);
                                                mainDriver.BirthDateYear = short.Parse(mainDriverBirthYear);
                                                break;

                                            case "VehicleSequenceNumber":
                                                successObj.Vehicle.VehicleIdTypeId = 1;
                                                successObj.Vehicle.SequenceNumber = cellValue.ToString();
                                                successObj.Vehicle.VehicleId = long.Parse(cellValue.ToString());
                                                break;

                                            case "VehicleCustomCardNumber":
                                                successObj.Vehicle.VehicleIdTypeId = 2;
                                                successObj.Vehicle.CustomCardNumber = cellValue.ToString();
                                                successObj.Vehicle.VehicleId = long.Parse(cellValue.ToString());
                                                break;

                                            case "VehicleModelYear":
                                                successObj.Vehicle.ModelYear = short.Parse(cellValue.ToString());
                                                successObj.Vehicle.VehicleModelYear = short.Parse(cellValue.ToString());
                                                successObj.Vehicle.ManufactureYear = short.Parse(cellValue.ToString());
                                                break;

                                            //case "VehicleAgencyRepair":
                                            //    successObj.VehicleAgencyRepair = cellValue.ToString();
                                            //    break;

                                            case "VehicleValue":
                                                int vehicleValue = 0;
                                                if (cellValue.ToString().Contains(".") && channel.ToLower() != "portal")
                                                {
                                                    string value = cellValue.ToString().Split('.')[0];
                                                    int.TryParse(value, out vehicleValue);
                                                    successObj.Vehicle.ApproximateValue = vehicleValue;
                                                }
                                                else
                                                    successObj.Vehicle.ApproximateValue = int.Parse(cellValue.ToString());
                                                break;

                                            case "Additionalbenifit":
                                                if (cellValue != null && !string.IsNullOrEmpty(cellValue.ToString()))
                                                {
                                                    var benfits = cellValue.ToString().Split(',');
                                                    if (benfits != null && benfits.Length > 0)
                                                    {
                                                        foreach (var benfitId in benfits)
                                                            successObj.Benfits.Add(int.Parse(benfitId));
                                                    }
                                                }
                                                break;

                                            case "DeductibleValue":
                                                successObj.DeductibleValue = int.Parse(cellValue.ToString());
                                                break;


                                            case "AdditionDriver1ID":
                                                successObj.Drivers.Add(additionalDriver1);
                                                additionalDriver1.NationalId = cellValue.ToString();
                                                mainDriver.DrivingPercentage = 50;
                                                additionalDriver1.DrivingPercentage = 50;
                                                break;

                                            case "AdditionDriver1DOB":
                                                var Driver1BirthMonth = cellValue.ToString().Split('/')[0];
                                                var Driver1BirthYear = cellValue.ToString().Split('/')[1];
                                                additionalDriver1.BirthDateMonth = Convert.ToByte(Driver1BirthMonth);
                                                additionalDriver1.BirthDateYear = short.Parse(Driver1BirthYear);
                                                break;

                                            case "AdditionDriver2ID":
                                                successObj.Drivers.Add(additionalDriver2);
                                                additionalDriver2.NationalId = cellValue.ToString();
                                                mainDriver.DrivingPercentage = 50;
                                                additionalDriver1.DrivingPercentage = 25;
                                                additionalDriver2.DrivingPercentage = 25;
                                                break;

                                            case "AdditionDriver2DOB":
                                                var Driver2BirthMonth = cellValue.ToString().Split('/')[0];
                                                var Driver2BirthYear = cellValue.ToString().Split('/')[1];
                                                additionalDriver2.BirthDateMonth = Convert.ToByte(Driver2BirthMonth);
                                                additionalDriver2.BirthDateYear = short.Parse(Driver2BirthYear);
                                                break;
                                        }
                                    }
                                    else if (!FieldsAllowNullValue.Contains(cellName))
                                    {
                                        rowErrorDescription.AppendLine("cell: " + cellName + " is invalid, and the error is cell value is empty");
                                        emptycol++;
                                    }

                                    probCount++;
                                }
                                if (emptycol < ColCount)
                                {
                                    if (string.IsNullOrEmpty(rowErrorDescription.ToString()))
                                        output.SuccessList.Add(successObj);
                                    else
                                    {
                                        failedObj.RowNumber = row;
                                        failedObj.Nin = successObj.Insured.NationalId ?? null;
                                        failedObj.VehicleId = (successObj.Vehicle.SequenceNumber != null)
                                            ? successObj.Vehicle.SequenceNumber
                                            : (successObj.Vehicle.CustomCardNumber != null)
                                                        ? successObj.Vehicle.CustomCardNumber
                                                        : null;
                                        failedObj.ErrorDescription = rowErrorDescription.ToString();
                                        output.FailedList.Add(failedObj);
                                    }
                                }
                            }
                        }
                    }
                }

                output.ErrorCode = (output.FailedList != null && output.FailedList.Any())
                    ? ((output.SuccessList != null && output.SuccessList.Any())
                            ? (int)OutPutModel.ErrorCodes.partiaSuccess
                            : (int)OutPutModel.ErrorCodes.Failed)
                    : (int)OutPutModel.ErrorCodes.Success;
                output.ErrorDescription = (output.FailedList != null && output.FailedList.Any())
                    ? ((output.SuccessList != null && output.SuccessList.Any())
                            ? "Partiel Success"
                            : "Failed")
                     : "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;

                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = (int)OutPutModel.ErrorCodes.ServiceException;
                output.ErrorDescription = AutoLeasingResources.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                return output;
            }
        }
        private static bool EmptyRow(ExcelWorksheet worksheet, int row, int colcount)
        {
            int emptyCount = 1;
            for (int col = 1; col < colcount; col++)
            {
                if (worksheet.Cells[row, col].Value == null)
                {
                    emptyCount++;
                }
            }
            if (emptyCount == colcount)
                return true;
            return false;
        }

        private Insured SaveInsured(CustomerYakeenInfoModel customerYakeenInfo, InquiryRequestModel requestModel, int addressId)
        {
            var insured = new Insured();
            if (string.IsNullOrEmpty(customerYakeenInfo.DateOfBirthH.ToString()))
            {
                string month = requestModel.Insured.BirthDateMonth.ToString();
                if (month.Length == 1)
                    month = "0" + month;
                insured.BirthDateH = "01-" + month + "-" + requestModel.Insured.BirthDateYear;
            }
            else
            {
                insured.BirthDateH = customerYakeenInfo.DateOfBirthH;
            }
            insured.BirthDate = customerYakeenInfo.DateOfBirthG;
            insured.NationalId = customerYakeenInfo.NIN;
            //insured.CardIdTypeId = customerYakeenInfo.IsCitizen ? 1 : 2;
            if (insured.NationalId.StartsWith("7") && requestModel.Channel.ToLower() == Channel.autoleasing.ToString())
            {
                insured.CardIdTypeId = 4;
            }
            else if (insured.NationalId.StartsWith("7"))
            {
                insured.CardIdTypeId = 3;
            }
            else
            {
                insured.CardIdTypeId = customerYakeenInfo.IsCitizen ? 1 : 2;
            }
            int genderId = (int)customerYakeenInfo.Gender;
            if (genderId == 1 || genderId == 2)
            {
                insured.Gender = customerYakeenInfo.Gender;
            }
            else
            {
                insured.Gender = Gender.Male;
            }

            insured.NationalityCode = customerYakeenInfo.NationalityCode.GetValueOrDefault().ToString();
            insured.FirstNameAr = customerYakeenInfo.FirstName;
            insured.MiddleNameAr = customerYakeenInfo.SecondName;
            insured.LastNameAr = $"{customerYakeenInfo.ThirdName} {customerYakeenInfo.LastName}";
            insured.FirstNameEn = customerYakeenInfo.EnglishFirstName;
            insured.MiddleNameEn = customerYakeenInfo.EnglishSecondName;
            insured.LastNameEn = $"{customerYakeenInfo.EnglishThirdName} {customerYakeenInfo.EnglishLastName}";
            insured.CityId = requestModel.CityCode;
            if (requestModel.Insured.InsuredWorkCityCode.HasValue)
            {
                insured.WorkCityId = requestModel.Insured.InsuredWorkCityCode;
            }
            else
            {
                insured.WorkCityId = requestModel.CityCode;
            }
            insured.ChildrenBelow16Years = requestModel.Insured.ChildrenBelow16Years;
            insured.SocialStatusId = customerYakeenInfo.SocialStatusId.HasValue ? customerYakeenInfo.SocialStatusId.Value : 0;

            insured.OccupationId = customerYakeenInfo.OccupationId;
            if (customerYakeenInfo.OccupationId.HasValue)
            {
                var occupation = occupationService.GetOccupations().Where(x => x.ID == customerYakeenInfo.OccupationId).FirstOrDefault();
                if (occupation != null)
                {
                    insured.OccupationName = occupation?.NameAr;
                    insured.OccupationCode = occupation?.Code;
                }
            }

            insured.IdIssueCityId = addressService.GetCityByName(addressService.GetAllCities(), Utilities.Removemultiplespaces(customerYakeenInfo.IdIssuePlace))?.Code;
            insured.EducationId = requestModel.Insured.EducationId > 0 ? requestModel.Insured.EducationId : 1;
            insured.CreatedDateTime = DateTime.Now;
            insured.ModifiedDateTime = DateTime.Now;
            insured.UserSelectedCityId = requestModel.CityCode;
            insured.AddressId = addressId;
            if (requestModel.Insured.InsuredWorkCityCode.HasValue)
            {
                insured.UserSelectedWorkCityId = requestModel.Insured.InsuredWorkCityCode;
            }
            else
            {
                insured.UserSelectedWorkCityId = requestModel.CityCode;
            }

            foreach (var driver in requestModel.Drivers)
            {
                var driverExtraLicenses = driver.DriverExtraLicenses;
                if (driverExtraLicenses != null && driverExtraLicenses.Any())
                {
                    InsuredExtraLicenses insuredExtraLicenses;
                    foreach (var item in driverExtraLicenses)
                    {
                        insuredExtraLicenses = new InsuredExtraLicenses();
                        insuredExtraLicenses.DriverNin = driver.NationalId;
                        insuredExtraLicenses.IsMainDriver = (driver.NationalId == requestModel.Insured.NationalId) ? true : false;
                        insuredExtraLicenses.LicenseCountryCode = item.CountryId;
                        insuredExtraLicenses.LicenseNumberYears = item.LicenseYearsId;

                        insured.InsuredExtraLicenses.Add(insuredExtraLicenses);
                    }
                }
            }

            insuredRepository.Insert(insured);
            return insured;
        }
        private static bool ValidateNationalIdAndExpiryDate(string cell, object value, out string invalidCellException)
        {
            invalidCellException = string.Empty;

            if (cell == "InsuredID")
            {
                // check if InsuredID is empty
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    invalidCellException = "Insured ID value is empty";
                    return false;
                }

                // check if InsuredID not number
                foreach (char c in value.ToString())
                {
                    if (!char.IsDigit(c))
                    {
                        invalidCellException = "Insured ID value contains character";
                        return false;
                    }
                }

                // check if InsuredID begin with 7
                if (!value.ToString().StartsWith("7"))
                {
                    invalidCellException = "Insured ID is invalid";
                    return false;
                }

                // check if InsuredID length not = 10 number
                if (value.ToString().Length != 10)
                {
                    invalidCellException = "Insured ID length is invalid";
                    return false;
                }
            }

            else if (cell == "MainDriverMobileNo")
            {
                var mobileNo = value.ToString();
                // check if MainDriverMobileNo is empty
                if (string.IsNullOrEmpty(mobileNo))
                {
                    invalidCellException = "Main Driver MobileNo value is empty";
                    return false;
                }

                // check if MainDriverMobileNo not number
                foreach (char c in mobileNo)
                {
                    if (!char.IsDigit(c))
                    {
                        invalidCellException = "Main Driver MobileNo value contains character";
                        return false;
                    }
                }

                // check if mobile no is valid
                Regex regexDial = new Regex(@"^(009665|9665|\+9665|05|5)(5|0|3|6|4|9|1|8|7)([0-9]{7})$");
                if (!regexDial.IsMatch(mobileNo))
                {
                    invalidCellException = "Main Driver MobileNo is invalid";
                    return false;
                }
            }

            else if (cell == "MainDriverID")
            {
                // check if nationalId is empty
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    if (cell == "MainDriverID")
                        invalidCellException = "MainDriver ID value is empty";
                    else if (cell == "AdditionDriver1ID")
                        invalidCellException = "AdditionDriver 1 ID value is empty";
                    else if (cell == "AdditionDriver2ID")
                        invalidCellException = "AdditionDriver 2 ID value is empty";
                    return false;
                }

                // check if not number
                foreach (char c in value.ToString())
                {
                    if (!char.IsDigit(c))
                    {
                        if (cell == "MainDriverID")
                            invalidCellException = "MainDriver ID value contains character";
                        else if (cell == "AdditionDriver1ID")
                            invalidCellException = "AdditionDriver 1 ID value contains character";
                        else if (cell == "AdditionDriver2ID")
                            invalidCellException = "AdditionDriver 2 ID value contains character";
                        return false;
                    }
                }

                // check if nationalId begin with 1, 2
                if (!(value.ToString().StartsWith("1") || value.ToString().StartsWith("2")))
                {
                    if (cell == "MainDriverID")
                        invalidCellException = "MainDriver ID is invalid";
                    else if (cell == "AdditionDriver1ID")
                        invalidCellException = "AdditionDriver 1 ID is invalid";
                    else if (cell == "AdditionDriver2ID")
                        invalidCellException = "AdditionDriver 2 ID is invalid";
                    return false;
                }

                // check if nationalId length not = 10 number
                if (value.ToString().Length != 10)
                {
                    if (cell == "MainDriverID")
                        invalidCellException = "MainDriver ID length is invalid";
                    else if (cell == "AdditionDriver1ID")
                        invalidCellException = "AdditionDriver 1 ID length is invalid";
                    else if (cell == "AdditionDriver2ID")
                        invalidCellException = "AdditionDriver 2 ID length is invalid";
                    return false;
                }
            }

            else if (cell == "MainDriverDOB")
            {
                var birthDateSplits = value.ToString().Split('/');

                // check if DOB is length valid
                if (birthDateSplits.Length != 2)
                {
                    if (cell == "MainDriverDOB")
                        invalidCellException = "MainDriver DOB value is invalid";
                    else if (cell == "AdditionDriver1DOB")
                        invalidCellException = "AdditionDriver 1 DOB value is invalid";
                    else if (cell == "AdditionDriver2DOB")
                        invalidCellException = "AdditionDriver 2 DOB value is invalid";
                    return false;
                }

                #region Birth Month Validation

                var birttMonth = birthDateSplits[0];

                // check if birth month is empty
                if (string.IsNullOrEmpty(birttMonth))
                {
                    if (cell == "MainDriverID")
                        invalidCellException = "MainDriver DOB month value is empty";
                    else if (cell == "AdditionDriver1ID")
                        invalidCellException = "AdditionDriver 1 DOB month value is empty";
                    else if (cell == "AdditionDriver2ID")
                        invalidCellException = "AdditionDriver 2 DOB month value is empty";
                    return false;
                }

                // check if birth month not number
                foreach (char c in birttMonth)
                {
                    if (!char.IsDigit(c))
                    {
                        if (cell == "MainDriverID")
                            invalidCellException = "MainDriver DOB month value contains character";
                        else if (cell == "AdditionDriver1ID")
                            invalidCellException = "AdditionDriver 1 DOB month value contains character";
                        else if (cell == "AdditionDriver2ID")
                            invalidCellException = "AdditionDriver 2 DOB month value contains character";
                        return false;
                    }
                }

                // check if birth month is valid
                if (!Enumerable.Range(1, 12).Contains(int.Parse(birttMonth)))
                {
                    if (cell == "MainDriverDOB")
                        invalidCellException = "MainDriver DOB month value is invalid";
                    else if (cell == "AdditionDriver1DOB")
                        invalidCellException = "AdditionDriver 1 DOB month value is invalid";
                    else if (cell == "AdditionDriver2DOB")
                        invalidCellException = "AdditionDriver 2 DOB month value is invalid";
                    return false;
                }

                #endregion

                #region Birth Year Validation

                var birthYear = birthDateSplits[1];

                // check if birth year is empty
                if (string.IsNullOrEmpty(birthYear))
                {
                    if (cell == "MainDriverID")
                        invalidCellException = "MainDriver DOB year value is empty";
                    else if (cell == "AdditionDriver1ID")
                        invalidCellException = "AdditionDriver 1 DOB year value is empty";
                    else if (cell == "AdditionDriver2ID")
                        invalidCellException = "AdditionDriver 2 DOB year value is empty";
                    return false;
                }

                // check if birth year not number
                foreach (char c in birthYear)
                {
                    if (!char.IsDigit(c))
                    {
                        if (cell == "MainDriverID")
                            invalidCellException = "MainDriver DOB year value contains character";
                        else if (cell == "AdditionDriver1ID")
                            invalidCellException = "AdditionDriver 1 DOB year value contains character";
                        else if (cell == "AdditionDriver2ID")
                            invalidCellException = "AdditionDriver 2 DOB year value contains character";
                        return false;
                    }
                }

                // check if birth year is valid
                if (int.Parse(birthYear) < 1)
                {
                    if (cell == "MainDriverDOB")
                        invalidCellException = "MainDriver DOB year is invalid";
                    else if (cell == "AdditionDriver1DOB")
                        invalidCellException = "AdditionDriver 1 DOB year value is invalid";
                    else if (cell == "AdditionDriver2DOB")
                        invalidCellException = "AdditionDriver 2 DOB year value is invalid";
                    return false;
                }

                #endregion
            }

            else if (cell == "VehicleSequenceNumber")
            {
                // check if VehicleSequenceNumber is empty
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    invalidCellException = "Vehicle SequenceNumber value is empty";
                    return false;
                }

                // check if VehicleSequenceNumber not number
                foreach (char c in value.ToString())
                {
                    if (!char.IsDigit(c))
                    {
                        invalidCellException = "Vehicle SequenceNumber value contains character";
                        return false;
                    }
                }

                // check if VehicleSequenceNumber length not = 10 number
                if (value.ToString().Length > 10)
                {
                    invalidCellException = "Vehicle SequenceNumber length is invalid";
                    return false;
                }
            }

            else if (cell == "VehicleCustomCardNumber")
            {
                // check if VehicleCustomCardNumber is empty
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    invalidCellException = "Vehicle CustomCardNumber value is empty";
                    return false;
                }

                // check if VehicleCustomCardNumber not number
                foreach (char c in value.ToString())
                {
                    if (!char.IsDigit(c))
                    {
                        invalidCellException = "Vehicle CustomCardNumber value contains character";
                        return false;
                    }
                }

                // check if VehicleCustomCardNumber length not = 10 number
                if (value.ToString().Length > 10)
                {
                    invalidCellException = "Vehicle CustomCardNumber length is invalid";
                    return false;
                }
            }

            else if (cell == "VehicleModelYear")
            {
                // check if VehicleModelYear is empty
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    invalidCellException = "Vehicle Model Year value is empty";
                    return false;
                }

                // check if VehicleModelYear not number
                foreach (char c in value.ToString())
                {
                    if (!char.IsDigit(c))
                    {
                        invalidCellException = "Vehicle Model Year value contains character";
                        return false;
                    }
                }

                // check if VehicleModelYear is valid
                if (int.Parse(value.ToString()) <= 0)
                {
                    invalidCellException = "Vehicle Model Year is invalid";
                    return false;
                }
            }

            else if (cell == "VehicleValue")
            {
                // check if VehicleValue is empty
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    invalidCellException = "Vehicle Value value is empty";
                    return false;
                }

                // check if VehicleValue not number
                foreach (char c in value.ToString())
                {
                    if (!char.IsDigit(c))
                    {
                        invalidCellException = "Vehicle value contains character";
                        return false;
                    }
                }

                // check if VehicleValue is valid
                if (int.Parse(value.ToString()) < 10000)
                {
                    invalidCellException = "Vehicle Value is invalid";
                    return false;
                }
            }

            else if (cell == "Additionalbenifit" && !string.IsNullOrEmpty(value.ToString()))
            {
                //// check if Additionalbenifit is empty
                //if (string.IsNullOrEmpty(value.ToString()))
                //{
                //    invalidCellException = "Additional benifits value is empty";
                //    return false;
                //}

                // check if Additionalbenifit not number
                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    int benefitId = 0;
                    foreach (string benefitIdInput in value.ToString().Split(',').ToList())
                    {
                        benefitId = 0;
                        int.TryParse(benefitIdInput, out benefitId);
                        if (benefitId == 0)
                        {
                            invalidCellException = string.Format("benfits value contains invalid character ({0})", benefitIdInput);
                            return false;
                        }
                    }
                }
            }

            else if (cell == "DeductibleValue")
            {
                // check if DeductibleValue is empty
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    invalidCellException = "Deductible Value value is empty";
                    return false;
                }

                // check if DeductibleValue not number
                foreach (char c in value.ToString())
                {
                    if (!char.IsDigit(c))
                    {
                        invalidCellException = "Deductible value contains character";
                        return false;
                    }
                }
            }

            else if (cell == "AdditionDriver1ID" || cell == "AdditionDriver2ID" && !string.IsNullOrEmpty(value.ToString()))
            {
                // check if not number
                foreach (char c in value.ToString())
                {
                    if (!char.IsDigit(c))
                    {
                        if (cell == "AdditionDriver1ID")
                            invalidCellException = "AdditionDriver 1 ID value contains character";
                        else if (cell == "AdditionDriver2ID")
                            invalidCellException = "AdditionDriver 2 ID value contains character";
                        return false;
                    }
                }

                // check if nationalId begin with 1, 2
                if (!(value.ToString().StartsWith("1") || value.ToString().StartsWith("2")))
                {
                    if (cell == "AdditionDriver1ID")
                        invalidCellException = "AdditionDriver 1 ID is invalid";
                    else if (cell == "AdditionDriver2ID")
                        invalidCellException = "AdditionDriver 2 ID is invalid";
                    return false;
                }

                // check if nationalId length not = 10 number
                if (value.ToString().Length != 10)
                {
                    if (cell == "AdditionDriver1ID")
                        invalidCellException = "AdditionDriver 1 ID length is invalid";
                    else if (cell == "AdditionDriver2ID")
                        invalidCellException = "AdditionDriver 2 ID length is invalid";
                    return false;
                }
            }

            else if (cell == "AdditionDriver1DOB" || cell == "AdditionDriver2DOB" && !string.IsNullOrEmpty(value.ToString()))
            {
                var birthDateSplits = value.ToString().Split('/');

                // check if DOB is length valid
                if (birthDateSplits.Length != 2)
                {
                    if (cell == "AdditionDriver1DOB")
                        invalidCellException = "AdditionDriver 1 DOB value is invalid";
                    else if (cell == "AdditionDriver2DOB")
                        invalidCellException = "AdditionDriver 2 DOB value is invalid";
                    return false;
                }

                #region Birth Month Validation

                var birttMonth = birthDateSplits[0];

                // check if birth month is empty
                if (string.IsNullOrEmpty(birttMonth))
                {
                    if (cell == "AdditionDriver1ID")
                        invalidCellException = "AdditionDriver 1 DOB month value is empty";
                    else if (cell == "AdditionDriver2ID")
                        invalidCellException = "AdditionDriver 2 DOB month value is empty";
                    return false;
                }

                // check if birth month not number
                foreach (char c in birttMonth)
                {
                    if (!char.IsDigit(c))
                    {
                        if (cell == "AdditionDriver1ID")
                            invalidCellException = "AdditionDriver 1 DOB month value contains character";
                        else if (cell == "AdditionDriver2ID")
                            invalidCellException = "AdditionDriver 2 DOB month value contains character";
                        return false;
                    }
                }

                // check if birth month is valid
                if (!Enumerable.Range(1, 12).Contains(int.Parse(birttMonth)))
                {
                    if (cell == "AdditionDriver1DOB")
                        invalidCellException = "AdditionDriver 1 DOB month value is invalid";
                    else if (cell == "AdditionDriver2DOB")
                        invalidCellException = "AdditionDriver 2 DOB month value is invalid";
                    return false;
                }

                #endregion

                #region Birth Year Validation

                var birthYear = birthDateSplits[1];

                // check if birth year is empty
                if (string.IsNullOrEmpty(birthYear))
                {
                    if (cell == "AdditionDriver1ID")
                        invalidCellException = "AdditionDriver 1 DOB year value is empty";
                    else if (cell == "AdditionDriver2ID")
                        invalidCellException = "AdditionDriver 2 DOB year value is empty";
                    return false;
                }

                // check if birth year not number
                foreach (char c in birthYear)
                {
                    if (!char.IsDigit(c))
                    {
                        if (cell == "AdditionDriver1ID")
                            invalidCellException = "AdditionDriver 1 DOB year value contains character";
                        else if (cell == "AdditionDriver2ID")
                            invalidCellException = "AdditionDriver 2 DOB year value contains character";
                        return false;
                    }
                }

                // check if birth year is valid
                if (int.Parse(birthYear) < 1)
                {
                    if (cell == "AdditionDriver1DOB")
                        invalidCellException = "AdditionDriver 1 DOB year value is invalid";
                    else if (cell == "AdditionDriver2DOB")
                        invalidCellException = "AdditionDriver 2 DOB year value is invalid";
                    return false;
                }

                #endregion
            }

            return true;
        }


        #region Admin Add Driver 
        public AddDriverOutput AddDriver(AddDriverModel model, string UserId, string userName, bool leasing = false, bool automatedTest = false)
        {
            AddDriverOutput output = new AddDriverOutput();
            PolicyModificationLog log = new PolicyModificationLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.Channel = model?.Channel.ToString();
            log.UserId = UserId.ToString();
            log.UserName = userName;
            log.MethodName = model.MethodName;
            log.NIN = model?.Driver?.NationalId;
            log.PolicyNo = model?.PolicyNo;
            try
            {
                model.AdditionStartDate = model.AdditionStartDate.AddDays(1);
                var validationOutput = ValidateAddDriverData(model, log);
                if (validationOutput.ErrorCode != AddDriverOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = validationOutput.ErrorCode;
                    output.ErrorDescription = validationOutput.ErrorDescription;
                    return output;
                }

                string ex = string.Empty;
                var policy = new Tameenk.Services.Inquiry.Components.Models.PolicyModel();
                if (leasing)
                    policy = GetLeasingPolicy(model.PolicyNo, model.ReferenceId, out ex); /// will changed
                else
                    policy = GetPolicyByPolicyNo(model.PolicyNo, model.ReferenceId, out ex);

                //var policy = GetPolicyByPolicyNo(model.PolicyNo, model.ReferenceId, out string ex);
                if (policy == null || !string.IsNullOrEmpty(ex))
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "Policy Not exist";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Policy Not exist , ex = " + ex;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (policy.Drivers.Where(x => x.IsMainDriver == 0).ToList().Count == 2)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "Can't Add  new driver as the policy already reached max number which is 2";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (policy.InsuranceCompanyId == 0)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.insuranceCompanyId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "insurance Company Id is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (policy.InsuranceTypeCode == 0)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.insuranceCompanyId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Insurance Type Code  is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }

                string referenceId = string.Empty;
                if (policy.InsuranceCompanyId == 12)
                    referenceId = CreateWataniyaReference();
                else
                    referenceId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 15);
                log.RefrenceId = referenceId;
                log.InsuranceTypeCode = policy.InsuranceTypeCode;

                log.CompanyId = policy.InsuranceCompanyId;
                bool isExist = policy.Drivers.Any(x => x.Nin == model.Driver.NationalId);
                if (isExist)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "Driver Already Exist on this policy";
                    log.ErrorDescription = "Driver with nin" + model.Driver.NationalId + " Already Exist on this policy ";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                var insuranceCompany = _insuranceCompanyService.GetById(policy.InsuranceCompanyId);
                if (insuranceCompany == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.insuranceCompanyId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "insurance Company is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                log.CompanyName = insuranceCompany.Key;
                log.CompanyId = insuranceCompany.InsuranceCompanyID;
                Guid userId = Guid.Empty;
                Guid.TryParse(UserId, out userId);
                ServiceRequestLog predefinedLogInfo = new ServiceRequestLog
                {
                    UserID = userId,
                    UserName = userName,
                    Channel = log.Channel,
                    ServerIP = log.ServerIP,
                    CompanyID = log.CompanyId,
                    CompanyName = log.CompanyName,
                    ReferenceId = policy.ReferenceId
                };
                predefinedLogInfo.DriverNin = model.Driver.NationalId.ToString();
                var mainDriver = policy.Drivers.FirstOrDefault(x => x.IsMainDriver == 1);
                var drivers = new List<DriverModel>
                {
                    model.Driver
                };

                var yakeenOutput = _inquiryUtilities.GetDriversData(drivers, policy.Insured, false, Convert.ToInt64(mainDriver.Nin), predefinedLogInfo);
                if (yakeenOutput.ErrorCode != DriversOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.DriverDataError;
                    output.ErrorDescription = yakeenOutput.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = yakeenOutput.LogErrorDescription;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                var driver = yakeenOutput.AdditionalDrivers.FirstOrDefault();
                var checkdriverexsitance = _policyModificationrepository.Table.Where(x => x.Nin == model.Driver.NationalId && x.MethodName == "AddDriver" && x.QuotationReferenceId == model.ReferenceId).ToList();
                if (checkdriverexsitance.Count > 0)
                {
                    _policyModificationrepository.Delete(checkdriverexsitance);
                }
                PolicyModification policyModification = new PolicyModification()
                {
                    Channel = model.Channel.ToString(),
                    CreatedDate = DateTime.Now,
                    UserIP = log.UserId,
                    CreatedBy = log.UserId,
                    InsuranceCompanyId = policy.InsuranceCompanyId,
                    InsuranceTypeCode = policy.InsuranceTypeCode,
                    MethodName = "AddDriver",
                    Nin = model.Driver.NationalId,
                    PolicyNo = policy.PolicyNo,
                    ReferenceId = referenceId,
                    ServerIP = log.ServerIP,
                    UserAgent = log.Headers["User-Agent"].ToString(),
                    QuotationReferenceId = model.ReferenceId,
                    DriverId = driver.DriverId,
                    InvoiceNo = GetNewInvoiceNumber(),
                    IsLeasing = leasing,
                    IsCheckedkOut = false,
                    IsDeleted = false,
                    ProviderServiceId = CheckoutProviderServicesCodes.PurchaseDriver
                };

                var oldPolicyModificationToDelete = _policyModificationrepository.Table
                    .Where(a => a.QuotationReferenceId == model.ReferenceId && a.IsLeasing
                            && (a.ProviderServiceId != null && a.ProviderServiceId == CheckoutProviderServicesCodes.PurchaseDriver)
                            && !a.IsCheckedkOut && !a.IsDeleted).ToList();
                File.WriteAllText(@"C:\inetpub\WataniyaLog\oldPolicyModificationToDelete_AddDrivers.txt", JsonConvert.SerializeObject(oldPolicyModificationToDelete));

                if (oldPolicyModificationToDelete.Any() && oldPolicyModificationToDelete.Count > 0)
                    _policyModificationrepository.Delete(oldPolicyModificationToDelete);

                this._policyModificationrepository.Insert(policyModification);
                if (policyModification.Id < 1)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.DriverDataError;
                    output.ErrorDescription = "Failed to insert policy modification Request";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Failed to insert policy modification Request";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (driver == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.DriverDataError;
                    output.ErrorDescription = "driver is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "driver is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                var quotation = _quotationResponserepository.TableNoTracking.Where(x => x.ReferenceId == model.ReferenceId).FirstOrDefault();
                var quotationRequest = quotationRequestRepository.Table.Where(e => e.ID == quotation.RequestId).FirstOrDefault();
                var vehicle = _vehicle.TableNoTracking.Where(e => e.ID == quotationRequest.VehicleId).FirstOrDefault();
                var product = _productRepository.TableNoTracking.Where(x => x.ReferenceId == quotation.ReferenceId).FirstOrDefault();

                AddDriverRequest request = new AddDriverRequest();
                request.AdditionStartDate = model.AdditionStartDate.ToString("dd-MM-yyyy", new CultureInfo("en-US"));
                if (policy.InsuranceCompanyId == 12)
                {
                    request.MainDriverNin = mainDriver.Nin;
                    //request.VehicleUsagePercentage =driver.DrivingPercentage;
                    request.Vehicle = vehicle;
                }
                request.DriverBirthDate = driver.DateOfBirthH;
                request.DriverBirthDateG = driver.DateOfBirthG;
                request.DriverChildrenBelow16Years = driver.ChildrenBelow16Years.Value;
                request.DriverDrivingPercentage = driver.DrivingPercentage.Value;
                request.DriverEducationCode = driver.EducationId;
                request.DriverFirstNameAr = driver.FirstName;
                request.DriverFirstNameEn = driver.EnglishFirstName;
                if (driver.GenderId == (int)Gender.Male)
                    request.DriverGenderCode = "M";
                else if (driver.GenderId == (int)Gender.Female)
                    request.DriverGenderCode = "F";
                else
                    request.DriverGenderCode = "M";
                request.DriverHomeCity = driver.CityName;
                request.DriverHomeCityCode = driver.CityId.ToString();
                request.DriverId = Convert.ToInt64(driver.NIN);
                request.DriverIdTypeCode = driver.IsCitizen ? 1 : 2;
                request.DriverLastNameAr = driver.LastName;
                request.DriverLastNameEn = driver.EnglishLastName;
                request.DriverMedicalConditionCode = driver.MedicalConditionId.Value;
                request.DriverMiddleNameAr = driver.SecondName;
                request.DriverMiddleNameEn = driver.EnglishSecondName;
                request.DriverNationalityCode = driver.NationalityCode.HasValue ? driver.NationalityCode.Value.ToString() : "113";
                request.DriverNOALast5Years = driver.NOALast5Years;
                request.DriverNOCLast5Years = driver.NOCLast5Years;
                var additionalDriverOccupation = driver.Occupation;
                if (additionalDriverOccupation == null && request.DriverIdTypeCode == 1)
                {
                    request.DriverOccupationCode = "O";
                    request.DriverOccupation = "غير ذالك";
                }
                else if (additionalDriverOccupation == null && request.DriverIdTypeCode == 2)
                {
                    request.DriverOccupationCode = "31010";
                    request.DriverOccupation = "موظف اداري";
                }
                else
                {
                    if ((string.IsNullOrEmpty(additionalDriverOccupation.Code) || additionalDriverOccupation.Code == "o") && request.DriverIdTypeCode == 1)
                    {
                        request.DriverOccupationCode = "O";
                        request.DriverOccupation = "غير ذالك";
                    }
                    else if ((string.IsNullOrEmpty(additionalDriverOccupation.Code) || additionalDriverOccupation.Code == "o") && request.DriverIdTypeCode == 2)
                    {
                        request.DriverOccupationCode = "31010";
                        request.DriverOccupation = "موظف اداري";
                    }
                    else
                    {
                        request.DriverOccupationCode = additionalDriverOccupation.Code;
                        request.DriverOccupation = additionalDriverOccupation.NameAr.Trim();
                    }
                }
                request.DriverRelationship = driver.RelationShipId.Value;
                request.DriverSocialStatusCode = driver.SocialStatusId.Value.ToString();
                request.DriverWorkCity = driver.WorkCityName;
                if (driver.WorkCityId.HasValue)
                    request.DriverWorkCityCode = driver.WorkCityId.Value.ToString();
                //var bank = _checkoutDetailrepository.TableNoTracking.Where(x => x.ReferenceId == model.ReferenceId).FirstOrDefault();
                request.PolicyNo = policy.PolicyNo;
                request.ReferenceId = referenceId;
                request.PolicyReferenceId = policy.ReferenceId;
                request.QuotationRequestId = quotation.RequestId; // quotation.RequestId;
                var bankId = 0;
                if (leasing)
                    bankId = _leasingUserService.GetUser(UserId).BankId;
                else
                    bankId = _autoleasingUserService.GetUser(UserId).BankId;

                if (!string.IsNullOrEmpty(bankId.ToString()))
                {
                    var bankNin = _bankNinsRepository.TableNoTracking.FirstOrDefault(e => e.BankId == bankId).NIN;
                    File.WriteAllText(@"C:\inetpub\WataniyaLog\request.bankNin.txt", JsonConvert.SerializeObject(bankNin.ToString()));

                    request.BankNin = bankNin;
                }
                var LicenseDtos = new List<LicenseDto>();
                if (driver.DriverLicenses != null && driver.DriverLicenses.Count() > 0)
                {
                    foreach (var item in driver.DriverLicenses)
                    {
                        LicenseDtos.Add(new LicenseDto()
                        {
                            DriverLicenseExpiryDate = item.ExpiryDateH,
                            DriverLicenseTypeCode = item.TypeDesc.ToString(),
                            LicenseCountryCode = 113,
                            LicenseNumberYears = (DateTime.Now.Year - DateExtension.ConvertHijriStringToDateTime(item.IssueDateH).Year)
                        });
                    }
                }
                request.DriverLicenses = LicenseDtos;
                request.DriverViolations = new List<ViolationDto>();
                if (driver.DriverViolations != null && driver.DriverViolations.Count > 0)
                {
                    request.DriverViolations = driver.DriverViolations.Select(e => new ViolationDto() { ViolationCode = e.ViolationId }).ToList();
                }
                IInsuranceProvider provider = GetProvider(insuranceCompany, policy.InsuranceTypeCode);
                if (provider == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "provider is null";
                    log.ErrorDescription = "provider is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                var results = provider.AddDriver(request, predefinedLogInfo, automatedTest);
                if (results == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = "Service Return Null ";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Service Return Null ";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (results.StatusCode != (int)AddDriverOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = "Failed to get Response as status code =  " + results.StatusCode;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Failed to get Response as status code =  " + results.StatusCode;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                policyModification.TotalAmount = results.TotalAmount;
                policyModification.TaxableAmount = results.TaxableAmount;
                policyModification.VATAmount = results.VATAmount;
                _policyModificationrepository.Update(policyModification);

                var productDriver = new Product_Driver()
                {
                    ProductId = product.Id,
                    DriverId = policyModification.DriverId.Value,
                    DriverExternalId = quotationRequest.ExternalId,
                    DriverPrice = results.TotalAmount
                };
                _productDriverRepository.Insert(productDriver);

                var checkoutDetail = _checkoutDetailrepository.TableNoTracking.Where(x => x.ReferenceId == model.ReferenceId).FirstOrDefault();
                bool updated = false;
                if (!checkoutDetail.AdditionalDriverIdOne.HasValue)
                {
                    quotationRequest.AdditionalDriverIdOne = policyModification.DriverId;
                    updated = true;
                }
                else if (!checkoutDetail.AdditionalDriverIdTwo.HasValue)
                {
                    quotationRequest.AdditionalDriverIdTwo = policyModification.DriverId;
                    updated = true;
                }

                if (updated)
                {
                    if (quotationRequest.Drivers == null || quotationRequest.Drivers.Count == 0)
                        quotationRequest.Drivers = new List<Driver>();
                    quotationRequest.Drivers.Add(driver);
                    _quotationRequestrepository.Update(quotationRequest);
                }

                //output.Drivers = new List<Product_Driver>();
                //output.Drivers.Add(productDriver);
                output.productDriverId = productDriver.Id;
                output.TotalAmount = results.TotalAmount;
                output.TaxableAmount = results.TaxableAmount;
                output.VATAmount = results.VATAmount;
                output.ReferenceId = referenceId;
                output.ErrorCode = AddDriverOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = AddDriverOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = "Failed to Request Add Driver Service";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                return output;
            }
        }

        private int GetNewInvoiceNumber()
        {
            Random rnd = new Random(System.Environment.TickCount);
            int invoiceNumber = rnd.Next(111111111, 999999999);

            if (_policyModificationrepository.Table.Any(i => i.InvoiceNo == invoiceNumber))
                return GetNewInvoiceNumber();

            return invoiceNumber;
        }
        private AddDriverOutput ValidateAddDriverData(AddDriverModel model, PolicyModificationLog log)
        {
            AddDriverOutput output = new AddDriverOutput();
            try
            {
                if (model == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.RequestModelIsNullException;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "requestModel Is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (string.IsNullOrWhiteSpace(model.PolicyNo))
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.ParkingLocationIdIsNull;
                    output.ErrorDescription = "PolicyNo is required";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "PolicyNo is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (string.IsNullOrWhiteSpace(model.ReferenceId))
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.ParkingLocationIdIsNull;
                    output.ErrorDescription = "ReferenceId is required";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "ReferenceId is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (model.AdditionStartDate == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.ParkingLocationIdIsNull;
                    output.ErrorDescription = "AdditionStartDate is required";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "AdditionStartDate is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (model.AdditionStartDate < DateTime.Now.Date.AddDays(1))
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.ParkingLocationIdIsNull;
                    output.ErrorDescription = "AdditionStartDate must be in future";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "AdditionStartDate must be in future";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                DriverModel driver = null;
                driver = model.Driver;
                if (driver.BirthDateMonth == 0 || driver.BirthDateYear == 0)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.ParkingLocationIdIsNull;
                    output.ErrorDescription = "BirthDate can't be zero month:" + driver.BirthDateMonth + " Year:" + driver.BirthDateYear;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "BirthDate can't be zero month:" + driver.BirthDateMonth + " Year:" + driver.BirthDateYear;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (driver.EducationId == 0)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.EducationIdIsNullAdditionalDriver;
                    output.ErrorDescription = SubmitInquiryResource.EducationIdIsNullAdditionalDriver.Replace("{0}", driver.NationalId);
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Additional Driver EducationId is zero for Additional Driver " + driver.NationalId;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (driver.MedicalConditionId == 0)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.MedicalConditionIdIsNullAdditionalDriver;
                    output.ErrorDescription = SubmitInquiryResource.MedicalConditionIdIsNullAdditionalDriver.Replace("{0}", driver.NationalId);
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Additional Driver MedicalConditionId is zero for Additional Driver " + driver.NationalId;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (!driver.DriverNOALast5Years.HasValue)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.DriverNOALast5YearsIsNullAdditionalDriver;
                    output.ErrorDescription = SubmitInquiryResource.DriverNOALast5YearsIsNullAdditionalDriver.Replace("{0}", driver.NationalId);
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Additional Driver DriverNOALast5Years is zero for Additional Driver " + driver.NationalId;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                //int additionalDrivingPercentage = driver.DrivingPercentage;
                //foreach (var d in oldDrivers)
                //{                    
                //    additionalDrivingPercentage += d.DrivingPercentage;
                //}                
                //if (additionalDrivingPercentage > 100)
                //{
                //    output.ErrorCode = AddDriverOutput.ErrorCodes.DrivingPercentageMoreThan100;
                //    output.ErrorDescription = SubmitInquiryResource.DrivingPercentageErrorInvalid;
                //    log.ErrorCode = (int)output.ErrorCode;
                //    log.ErrorDescription = "Driving Percentage exceeded 100% for Additional Drivers as it's with total " + additionalDrivingPercentage;
                //    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                //    return output;
                //}
                output.ErrorCode = AddDriverOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = AddDriverOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = SubmitInquiryResource.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = exp.ToString();
                PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                return output;
            }
        }
        private IInsuranceProvider GetProvider(InsuranceCompany insuranceCompany, int type)
        {
            var providerFullTypeName = string.Empty;
            providerFullTypeName = insuranceCompany.ClassTypeName + ", " + insuranceCompany.NamespaceTypeName;
            IInsuranceProvider provider = null;
            object instance = Utilities.GetValueFromCache("instance_" + providerFullTypeName + type);
            if (instance != null && insuranceCompany.Key != "Tawuniya")
            {
                provider = instance as IInsuranceProvider;
            }
            if (instance == null)
            {
                var scope = EngineContext.Current.ContainerManager.Scope();
                var providerType = Type.GetType(providerFullTypeName);

                if (providerType != null)
                {
                    if (!EngineContext.Current.ContainerManager.TryResolve(providerType, scope, out instance))
                    {
                        instance = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope);
                    }
                    provider = instance as IInsuranceProvider;
                }
                if (provider == null)
                {
                    throw new Exception("Unable to find provider.");
                }
                if (insuranceCompany.Key != "Tawuniya")
                    Utilities.AddValueToCache("instance_" + providerFullTypeName + type, instance, 1440);

                scope.Dispose();
            }
            return provider;
        }
        private Models.PolicyModel GetPolicyByPolicyNo(string policyNo, string referenceId, out string exception)
        {
            exception = string.Empty;
            int commandTimeout = 120;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetPolicyData";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;
                SqlParameter policyNoParameter = new SqlParameter()
                {
                    ParameterName = "policyNo",
                    Value = policyNo
                };
                SqlParameter refNoParameter = new SqlParameter()
                {
                    ParameterName = "referenceId",
                    Value = referenceId
                };
                command.Parameters.Add(policyNoParameter);
                command.Parameters.Add(refNoParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<Models.PolicyModel>(reader).FirstOrDefault();
                reader.NextResult();
                data.Drivers = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<Models.AdditionalDriver>(reader).ToList();
                if (data.Drivers.Any())
                {
                    data.Insured = new InsuredModel
                    {
                        NationalId = data.Drivers.FirstOrDefault().Insured
                    };
                }
                dbContext.DatabaseInstance.Connection.Close();
                return data;
            }
            catch (Exception exp)
            {
                dbContext.DatabaseInstance.Connection.Close();
                exception = exp.ToString();
                return null;
            }
        }

        private Models.PolicyModel GetLeasingPolicy(string policyNo, string referenceId, out string exception)
        {
            exception = string.Empty;
            int commandTimeout = 120;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetLeasingPolicyData";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;
                SqlParameter policyNoParameter = new SqlParameter()
                {
                    ParameterName = "policyNo",
                    Value = policyNo
                };
                SqlParameter refNoParameter = new SqlParameter()
                {
                    ParameterName = "referenceId",
                    Value = referenceId
                };
                command.Parameters.Add(policyNoParameter);
                command.Parameters.Add(refNoParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<Models.PolicyModel>(reader).FirstOrDefault();
                reader.NextResult();
                data.Drivers = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<Models.AdditionalDriver>(reader).ToList();
                if (data.Drivers.Any())
                {
                    data.Insured = new InsuredModel
                    {
                        NationalId = data.Drivers.FirstOrDefault().Insured
                    };
                }
                dbContext.DatabaseInstance.Connection.Close();
                return data;
            }
            catch (Exception exp)
            {
                dbContext.DatabaseInstance.Connection.Close();
                exception = exp.ToString();
                return null;
            }
        }

        public AddDriverOutput PurchaseDriver(PurchaseDriverModel model, string UserId, string userName)
        {
            AddDriverOutput output = new AddDriverOutput();
            PolicyModificationLog log = new PolicyModificationLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.Channel = model?.Channel.ToString();
            if (!string.IsNullOrEmpty(userName))
            {
                log.UserId = UserId.ToString();
                log.UserName = userName;
            }
            try
            {
                if (model == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.RequestModelIsNullException;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "requestModel Is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (string.IsNullOrWhiteSpace(model.ReferenceId))
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.ParkingLocationIdIsNull;
                    output.ErrorDescription = "ReferenceId is required";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "ReferenceId is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                var request = _policyModificationrepository.Table.FirstOrDefault(x => x.ReferenceId == model.ReferenceId);
                if (request == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "request  Not exist";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "request  Not exist ";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                var insuranceCompany = _insuranceCompanyService.GetById(request.InsuranceCompanyId.Value);
                if (insuranceCompany == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.insuranceCompanyId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "insurance Company is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                log.CompanyName = insuranceCompany.Key;
                log.CompanyId = insuranceCompany.InsuranceCompanyID;
                Guid userId = Guid.Empty;
                Guid.TryParse(UserId, out userId);
                ServiceRequestLog predefinedLogInfo = new ServiceRequestLog
                {
                    UserID = userId,
                    UserName = userName,
                    Channel = log.Channel,
                    ServerIP = log.ServerIP,
                    CompanyID = log.CompanyId,
                    CompanyName = log.CompanyName,
                    ReferenceId = request.QuotationReferenceId
                };
                predefinedLogInfo.DriverNin = request.Nin;
                PurchaseDriverRequest serviceRequest = new PurchaseDriverRequest();
                serviceRequest.PolicyNo = request.PolicyNo;
                serviceRequest.ReferenceId = request.ReferenceId;
                serviceRequest.PaymentAmount = request.TotalAmount.Value;
                serviceRequest.PaymentBillNumber = request.InvoiceNo.ToString();
                IInsuranceProvider provider = GetProvider(insuranceCompany, request.InsuranceTypeCode.Value);
                if (provider == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "provider is null";
                    log.ErrorDescription = "provider is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                var results = provider.PurchaseDriver(serviceRequest, predefinedLogInfo);
                if (results == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = "Service Return Null ";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Service Return Null ";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (results.StatusCode != (int)AddDriverOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.ServiceDown;
                    if(results.Errors != null && results.Errors.Any())
                    {
                        output.ErrorDescription = string.Join(",", results.Errors.Select(x => x.Message));
                    }
                    else
                    {
                        output.ErrorDescription = "Failed to get Response as status code =  " + results.StatusCode;
                    }

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                QuotationRequest quotationRequest = GetQuotationRequest(request.PolicyNo, request.InsuranceCompanyId, out string ex);
                if (quotationRequest == null || !string.IsNullOrEmpty(ex))
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "quotation request  Not exist";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "quotation request  Not exist " + ex;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (quotationRequest.AdditionalDriverIdOne == null)
                {
                    quotationRequest.AdditionalDriverIdOne = request.DriverId;
                }
                else if (quotationRequest.AdditionalDriverIdTwo == null)
                {
                    quotationRequest.AdditionalDriverIdTwo = request.DriverId;
                }
                else if (quotationRequest.AdditionalDriverIdThree == null)
                {
                    quotationRequest.AdditionalDriverIdThree = request.DriverId;
                }
                else if (quotationRequest.AdditionalDriverIdFour == null)
                {
                    quotationRequest.AdditionalDriverIdFour = request.DriverId;
                }
                _quotationRequestrepository.Update(quotationRequest);
                CheckoutDetail c = _checkoutDetailrepository.Table.FirstOrDefault(x => x.ReferenceId == request.QuotationReferenceId);
                if (c.AdditionalDriverIdOne == null)
                {
                    c.AdditionalDriverIdOne = request.DriverId;
                }
                else if (c.AdditionalDriverIdTwo == null)
                {
                    c.AdditionalDriverIdTwo = request.DriverId;
                }
                else if (c.AdditionalDriverIdThree == null)
                {
                    c.AdditionalDriverIdThree = request.DriverId;
                }
                else if (c.AdditionalDriverIdFour == null)
                {
                    c.AdditionalDriverIdFour = request.DriverId;
                }
                //CheckoutAdditionalDriver additionalDriver = new CheckoutAdditionalDriver
                //{
                //    CheckoutDetailsId = c.ReferenceId,
                //    DriverId = request.DriverId.Value,
                //};
                //_checkoutAdditionalDriverrepository.Insert(additionalDriver);
                string fileURL = results.EndorsementFileUrl;
                fileURL = fileURL.Replace(@"\\", @"//");
                fileURL = fileURL.Replace(@"\", @"/");
                byte[] bytes = null;
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    bytes = client.DownloadData(fileURL);
                }

                var isAutoleasingPolicy = c.Channel == Channel.autoleasing.ToString().ToLower() ? true : false;
                string filePath = Utilities.SaveCompanyFileFromDashboard(c.ReferenceId, bytes, insuranceCompany.Key, false,
                        _tameenkConfig.RemoteServerInfo.UseNetworkDownload,
                        _tameenkConfig.RemoteServerInfo.DomainName,
                        _tameenkConfig.RemoteServerInfo.ServerIP,
                        _tameenkConfig.RemoteServerInfo.ServerUserName,
                        _tameenkConfig.RemoteServerInfo.ServerPassword,
                        isAutoleasingPolicy,
                        out string exception);
                if (!string.IsNullOrWhiteSpace(exception))
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "failed to save file";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "failed to save file duo to exception : " + ex;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                Endorsment driverFile = new Endorsment
                {
                    FilePath = filePath,
                    InsurranceCompanyId = insuranceCompany.InsuranceCompanyID,
                    PolicyModificationRequestId = request.Id,
                    ReferenceId = c.ReferenceId,
                    Channel = log.Channel,
                    ServerIP = log.ServerIP,
                    UserAgent = log.Headers["User-Agent"].ToString(),
                    UserIP = log.UserIP,
                    CreatedDate = DateTime.Now,
                    QuotationReferenceId = request.QuotationReferenceId
                };
                _endormentRepository.Insert(driverFile);
                output.ErrorCode = AddDriverOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = AddDriverOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = "Failed to Request Add Driver Service";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                return output;
            }
        }

        private QuotationRequest GetQuotationRequest(string policyNo, int? insuranceCompanyId, out string exception)
        {
            exception = string.Empty;
            int commandTimeout = 120;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GeQuotationByPolicyNo";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;
                SqlParameter policyNoParameter = new SqlParameter()
                {
                    ParameterName = "policyNo",
                    Value = policyNo
                };
                SqlParameter companyIdParameter = new SqlParameter()
                {
                    ParameterName = "companyId",
                    Value = insuranceCompanyId.Value
                };
                command.Parameters.Add(policyNoParameter);
                command.Parameters.Add(companyIdParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<QuotationRequest>(reader).FirstOrDefault();
                reader.NextResult();
                dbContext.DatabaseInstance.Connection.Close();
                return data;
            }
            catch (Exception exp)
            {
                dbContext.DatabaseInstance.Connection.Close();
                exception = exp.ToString();
                return null;
            }
        }
        #endregion

        #region Admin Add Benefit

        public AddBenefitOutput AddBenefit(AddBenefitModel model, string UserId, string userName, bool leasing = false)
        {
            AddBenefitOutput output = new AddBenefitOutput();
            PolicyModificationLog log = new PolicyModificationLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.Channel = model?.Channel.ToString();
            log.UserId = UserId.ToString();
            log.UserName = userName;
            log.RefrenceId = model.ReferenceId;
            log.PolicyNo = model?.PolicyNo;
            log.MethodName = model.MethodName;
            try
            {
                model.BenefitStartDate = model.BenefitStartDate.AddDays(1);
                var validationOutput = ValidateAddBenefitData(model, log);
                if (validationOutput.ErrorCode != AddBenefitOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = validationOutput.ErrorCode;
                    output.ErrorDescription = validationOutput.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = validationOutput.ErrorDescription;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }

                var checkoutDetails = _checkoutDetailrepository.TableNoTracking.Where(x => x.ReferenceId == model.ReferenceId).FirstOrDefault();
                if (checkoutDetails == null)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "checkoutDetails not exist with the given referenceId: " + model.ReferenceId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "checkoutDetails not exist with the given referenceId: " + model.ReferenceId;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }

                var policy = GetPolicyByPolicyNo(model.PolicyNo, model.ReferenceId, out string ex);
                if (policy == null || !string.IsNullOrEmpty(ex))
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "Policy Not exist";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Policy Not exist , ex = " + ex;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (policy.InsuranceCompanyId == 0)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.insuranceCompanyId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "insurance Company Id is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (policy.InsuranceTypeCode == 0)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.insuranceCompanyId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Insurance Type Code  is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }

                string referenceId = string.Empty;
                if (policy.InsuranceCompanyId == 12)
                    referenceId = CreateWataniyaReference();
                else
                    referenceId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 15);
                log.RefrenceId = referenceId;
                log.InsuranceTypeCode = policy.InsuranceTypeCode;

                var insuranceCompany = _insuranceCompanyService.GetById(policy.InsuranceCompanyId);
                if (insuranceCompany == null)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.insuranceCompanyId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "insurance Company is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                log.CompanyName = insuranceCompany.Key;
                log.CompanyId = insuranceCompany.InsuranceCompanyID;
                Guid userId = Guid.Empty;
                Guid.TryParse(UserId, out userId);
                ServiceRequestLog predefinedLogInfo = new ServiceRequestLog
                {
                    UserID = userId,
                    UserName = userName,
                    Channel = log.Channel,
                    ServerIP = log.ServerIP,
                    CompanyID = log.CompanyId,
                    CompanyName = log.CompanyName,
                    ReferenceId = model.ReferenceId,
                    Method = log.MethodName
                };
                PolicyModification policyModification = new PolicyModification()
                {
                    Channel = model.Channel.ToString(),
                    CreatedDate = DateTime.Now,
                    UserIP = log.UserId,
                    CreatedBy = log.UserId,
                    InsuranceCompanyId = policy.InsuranceCompanyId,
                    InsuranceTypeCode = policy.InsuranceTypeCode,
                    MethodName = "AddBenefit",
                    PolicyNo = policy.PolicyNo,
                    ReferenceId = referenceId,
                    ServerIP = log.ServerIP,
                    UserAgent = log.Headers["User-Agent"].ToString(),
                    QuotationReferenceId = model.ReferenceId,
                    InvoiceNo = GetNewInvoiceNumber(),
                    IsLeasing = leasing,
                    IsCheckedkOut = false,
                    IsDeleted = false,
                    ProviderServiceId = CheckoutProviderServicesCodes.PurchaseBenefit
                };

                var oldPolicyModificationToDelete = _policyModificationrepository.Table
                    .Where(a => a.QuotationReferenceId == model.ReferenceId && a.IsLeasing
                            && (a.ProviderServiceId != null && a.ProviderServiceId == CheckoutProviderServicesCodes.PurchaseBenefit)
                            && !a.IsCheckedkOut && !a.IsDeleted).ToList();

                if (oldPolicyModificationToDelete != null && oldPolicyModificationToDelete.Count > 0)
                    _policyModificationrepository.Delete(oldPolicyModificationToDelete);

                this._policyModificationrepository.Insert(policyModification);

                if (policyModification.Id < 1)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.DriverDataError;
                    output.ErrorDescription = "Failed to insert policy modification Request";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Failed to insert policy modification Request";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                var quotation = _quotationResponserepository.TableNoTracking.Where(x => x.ReferenceId == model.ReferenceId).FirstOrDefault();
                var bank = _checkoutDetailrepository.TableNoTracking.Where(x => x.ReferenceId == model.ReferenceId).FirstOrDefault();
                AddBenefitRequest request = new AddBenefitRequest();
                request.ReferenceId = referenceId;
                request.PolicyNo = policy.PolicyNo;
                request.BenefitStartDate = model.BenefitStartDate.ToString("dd-MM-yyyy", new CultureInfo("en-US"));
                request.QuotationReferenceId = model.ReferenceId;
                request.QuotationRequestId = quotation.RequestId;
                request.BankId = bank.BankId;
                request.CompanyId = bank.InsuranceCompanyId;
                IInsuranceProvider provider = GetProvider(insuranceCompany, policy.InsuranceTypeCode);
                if (provider == null)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "provider is null";
                    log.ErrorDescription = "provider is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }

                //var results = provider.AddBenefit(request, predefinedLogInfo);
                var results = provider.AutoleasingAddBenefit(request, predefinedLogInfo);

                if (results == null || (results.Benefits == null || results.Benefits.Count < 1))
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = "Service Return Null ";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Service Return Null ";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (results.StatusCode != (int)AddDriverOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = "Failed to get Response as status code =  " + results.StatusCode;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Failed to get Response as status code =  " + results.StatusCode;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }

                string exception = string.Empty;
                var benefits = HandleAddBenifitResponseObject(results.Benefits, policyModification, checkoutDetails.SelectedProductId.Value, quotation.ReferenceId, out exception);
                if (benefits == null || !string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = "Error happend, please try again later";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"Errror happend while inserting AddBenefit results, and error is: {exception}";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }

                output.LeasingBenefits = benefits;
                output.ReferenceId = referenceId;
                output.ErrorCode = AddBenefitOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = AddBenefitOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = "Failed to Request Add benefit Service";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                return output;
            }
        }
        private AddBenefitOutput ValidateAddBenefitData(AddBenefitModel model, PolicyModificationLog log)
        {
            AddBenefitOutput output = new AddBenefitOutput();
            try
            {
                if (model == null)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.RequestModelIsNullException;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "requestModel Is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (string.IsNullOrWhiteSpace(model.PolicyNo))
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "PolicyNo is required";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "PolicyNo is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (string.IsNullOrWhiteSpace(model.ReferenceId))
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "ReferenceId is required";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "ReferenceId is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (model.BenefitStartDate == null)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "BenefitStartDate is required";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "BenefitStartDate is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (model.BenefitStartDate < DateTime.Now.Date.AddDays(1))
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "AdditionStartDate must be in future";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "AdditionStartDate must be in future";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                output.ErrorCode = AddBenefitOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = AddBenefitOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = SubmitInquiryResource.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = exp.ToString();
                PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                return output;
            }
        }

        private List<QuotationProductBenefitModel> HandleAddBenifitResponseObject(List<AdditionalBenefitDto> benefits, PolicyModification policyModification, Guid productId, string referenceId, out string exception)
        {
            exception = string.Empty;

            try
            {
                List<QuotationProductBenefitModel> resultBenefits = new List<QuotationProductBenefitModel>();
                List<PolicyAdditionalBenefit> additionalBenefits = new List<PolicyAdditionalBenefit>();
                List<Quotation_Product_Benefit> productBenefits = new List<Quotation_Product_Benefit>();
                var allBenefitst = _benefitRepository.TableNoTracking.ToList();
                var oldSelectedBenefits = _orderItemRepository.TableNoTracking.Include(a => a.OrderItemBenefits)
                                                                .Where(a => a.ProductId == productId).SelectMany(a => a.OrderItemBenefits).ToList();

                foreach (var benefit in benefits)
                {
                    PolicyAdditionalBenefit policyAdditionalBenefit = new PolicyAdditionalBenefit();
                    policyAdditionalBenefit.ReferenceId = policyModification.ReferenceId;
                    policyAdditionalBenefit.InsuranceCompanyId = policyModification.InsuranceCompanyId;
                    policyAdditionalBenefit.InsuranceTypeCode = policyModification.InsuranceTypeCode;
                    policyAdditionalBenefit.QuotationReferenceId = policyModification.QuotationReferenceId;
                    policyAdditionalBenefit.BenefitCode = benefit.BenefitCode;
                    policyAdditionalBenefit.BenefitDescAr = benefit.BenefitDescAr = (benefit.BenefitCode == 0) ? benefit.BenefitDescAr : allBenefitst.Where(a => a.Code == benefit.BenefitCode).FirstOrDefault().ArabicDescription;
                    policyAdditionalBenefit.BenefitDescEn = benefit.BenefitDescEn = (benefit.BenefitCode == 0) ? benefit.BenefitDescEn : allBenefitst.Where(a => a.Code == benefit.BenefitCode).FirstOrDefault().EnglishDescription;
                    //benefit.BenefitDescAr= _benefit.TableNoTracking.Where(a => a.Code == benefit.BenefitCode).Select(a => a.ArabicDescription).FirstOrDefault();
                    //benefit.BenefitDescEn= _benefit.TableNoTracking.Where(a => a.Code == benefit.BenefitCode).Select(a => a.EnglishDescription).FirstOrDefault();
                    policyAdditionalBenefit.BenefitEffectiveDate = benefit.BenefitEffectiveDate;
                    policyAdditionalBenefit.BenefitExpiryDate = benefit.BenefitExpiryDate;
                    policyAdditionalBenefit.BenefitId = benefit.BenefitId;
                    policyAdditionalBenefit.BenefitNameAr = benefit.BenefitNameAr;
                    policyAdditionalBenefit.BenefitNameEn = benefit.BenefitNameEn;
                    policyAdditionalBenefit.BenefitPrice = benefit.BenefitPrice;
                    policyAdditionalBenefit.DeductibleValue = (decimal)benefit.DeductibleValue;
                    policyAdditionalBenefit.TaxableAmount = (decimal)benefit.TaxableAmount;
                    policyAdditionalBenefit.VATAmount = (decimal)benefit.VATAmount;
                    //additionalBenefits.Add(policyAdditionalBenefit);
                    _policyAdditionalBenefitRepository.Insert(policyAdditionalBenefit);

                    if (oldSelectedBenefits != null && oldSelectedBenefits.Any(a => a.BenefitExternalId == benefit.BenefitId))
                    {
                        var oldBenefitId = oldSelectedBenefits.Where(a => a.BenefitExternalId == benefit.BenefitId).FirstOrDefault().Id;
                        var oldBenefit = _quotationProductBenefitRepository.TableNoTracking.Where(x => x.Id == oldBenefitId).FirstOrDefault();
                        resultBenefits.Add(HandleLeasingAddBenefitsModel(oldBenefit, benefit, true));
                        continue;
                    }

                    short benefitId;
                    short.TryParse(benefit.BenefitCode.ToString(), out benefitId);

                    var product_Benefit = new Quotation_Product_Benefit();
                    product_Benefit.ProductId = productId;
                    product_Benefit.BenefitId = benefitId;
                    product_Benefit.IsSelected = benefit.BenefitPrice == 0 ? true : false;
                    product_Benefit.BenefitPrice = (decimal)benefit.TaxableAmount; // benefit.BenefitPrice;
                    product_Benefit.BenefitExternalId = benefit.BenefitId;
                    //product_Benefit.IsReadOnly = benefit.IsReadOnly;
                    product_Benefit.IsReadOnly = benefit.BenefitPrice == 0 ? true : false;
                    product_Benefit.BenefitNameAr = benefit.BenefitNameAr;
                    product_Benefit.BenefitNameEn = benefit.BenefitNameEn;
                    product_Benefit.CoveredCountry = benefit.CoveredCountry;
                    product_Benefit.AveragePremium = benefit.AveragePremium;

                    _quotationProductBenefitRepository.Insert(product_Benefit);
                    resultBenefits.Add(HandleLeasingAddBenefitsModel(product_Benefit, benefit));
                }
                return resultBenefits;
            }

            catch (Exception ex)
            {
                exception = ex.ToString();
                return null;
            }
        }

        private QuotationProductBenefitModel HandleLeasingAddBenefitsModel(Quotation_Product_Benefit benefit, AdditionalBenefitDto benefitDto, bool isOld = false)
        {
            var benefitModel = new QuotationProductBenefitModel()
            {
                Id = benefit.Id,
                ProductId = benefit.ProductId,
                BenefitId = benefit.BenefitId,
                BenefitExternalId = benefit.BenefitExternalId,
                BenefitPrice = benefitDto.BenefitPrice,
                TaxableAmount = benefitDto.TaxableAmount,
                VATAmount = benefitDto.VATAmount,
                IsSelected = isOld ? true : benefit.IsSelected,
                IsReadOnly = isOld ? true : benefit.IsReadOnly,
                IsOld = isOld,
                BenefitNameAr = String.IsNullOrEmpty(benefit.BenefitNameAr) ? benefitDto.BenefitNameAr : benefit.BenefitNameAr,
                BenefitNameEn = String.IsNullOrEmpty(benefit.BenefitNameEn) ? benefitDto.BenefitNameEn : benefit.BenefitNameEn
            };
            return benefitModel;
        }

        private string CreateWataniyaReference()
        {
            Random random = new Random();
            string stringReference = string.Empty;
            for (int i = 1; i <= 10; i++)
                stringReference += random.Next(1, 9).ToString();

            return stringReference;
        }

        public AddBenefitOutput PurchaseBenefit(PurchaseBenefitModel model, string UserId, string userName)
        {
            AddBenefitOutput output = new AddBenefitOutput();
            PolicyModificationLog log = new PolicyModificationLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.Channel = model?.Channel.ToString();
            log.MethodName = "PurchaseBenefit";
            if (!string.IsNullOrEmpty(userName))
            {
                log.UserId = UserId.ToString();
                log.UserName = userName;
            }
            try
            {
                if (model == null)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.RequestModelIsNullException;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "requestModel Is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (string.IsNullOrWhiteSpace(model.ReferenceId))
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.ParkingLocationIdIsNull;
                    output.ErrorDescription = "ReferenceId is required";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "ReferenceId is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                //var request = _policyModificationrepository.Table.FirstOrDefault(x => x.QuotationReferenceId == model.ReferenceId);
                var request = _policyModificationrepository.Table.FirstOrDefault(x => x.ReferenceId == model.ReferenceId);
                if (request == null)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "request  Not exist";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "request  Not exist ";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                var insuranceCompany = _insuranceCompanyService.GetById(request.InsuranceCompanyId.Value);
                if (insuranceCompany == null)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.insuranceCompanyId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "insurance Company is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                log.CompanyName = insuranceCompany.Key;
                log.CompanyId = insuranceCompany.InsuranceCompanyID;
                Guid userId = Guid.Empty;
                Guid.TryParse(UserId, out userId);
                ServiceRequestLog predefinedLogInfo = new ServiceRequestLog
                {
                    UserID = userId,
                    UserName = userName,
                    Channel = log.Channel,
                    ServerIP = log.ServerIP,
                    CompanyID = log.CompanyId,
                    CompanyName = log.CompanyName,
                    ReferenceId = request.QuotationReferenceId
                };

                CheckoutDetail checkoutdet = _checkoutDetailrepository.Table.Include(a => a.Driver).FirstOrDefault(x => x.ReferenceId == request.ReferenceId);
                var isAutoleasingPolicy = checkoutdet.Channel == Channel.autoleasing.ToString().ToLower() ? true : false;

                var orderItems = _orderItemRepository.TableNoTracking.Where(x => x.CheckoutDetailReferenceId == request.ReferenceId).Include(x => x.OrderItemBenefits).FirstOrDefault();
                decimal totalAmmount = 0;
                if (orderItems != null)
                {
                    if (model.Benefits != null && model.Benefits.Count() > 0)
                    {
                        foreach (var benefit in model.Benefits.ToList())
                        {
                            totalAmmount += _policyAdditionalBenefitRepository.TableNoTracking.Where(x => x.Id == benefit.Id).Select(x => x.BenefitPrice).FirstOrDefault().Value * 1.15M;
                        }
                        File.WriteAllText(@"C:\inetpub\WataniyaLog\PurchaseBenefit_totalAmmount.txt", JsonConvert.SerializeObject(totalAmmount.ToString()));
                    }
                }
                PurchaseBenefitRequest serviceRequest = new PurchaseBenefitRequest();
                serviceRequest.PolicyNo = request.PolicyNo;
                serviceRequest.ReferenceId = request.ReferenceId;
                serviceRequest.PaymentAmount = (double)totalAmmount;
                serviceRequest.PaymentBillNumber = request.InvoiceNo.ToString();
                if (request.InsuranceCompanyId.Value == 12)
                {
                    serviceRequest.LesseeID = checkoutdet.Driver.NIN;
                    serviceRequest.LessorID = _autoleasingUserService.GetUser(UserId).BankId.ToString();
                }
                serviceRequest.Benefits = model.Benefits;
                IInsuranceProvider provider = GetProvider(insuranceCompany, request.InsuranceTypeCode.Value);
                if (provider == null)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "provider is null";
                    log.ErrorDescription = "provider is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                var results = provider.AutoleasingPurchaseBenefit(serviceRequest, predefinedLogInfo);
                if (results == null)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = "Service Return Null ";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Service Return Null ";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (results.StatusCode != (int)AddBenefitOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = "Failed to get Response as status code =  " + results.StatusCode;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Failed to get Response as status code =  " + results.StatusCode;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }

                string filePath = string.Empty;
                string exception = string.Empty;
                Endorsment benefitFile = new Endorsment();
                if (!string.IsNullOrEmpty(results.EndorsementFileUrl))
                {
                    string fileURL = results.EndorsementFileUrl;
                    fileURL = fileURL.Replace(@"\\", @"//");
                    fileURL = fileURL.Replace(@"\", @"/");
                    byte[] bytes = null;
                    using (System.Net.WebClient client = new System.Net.WebClient())
                    {
                        bytes = client.DownloadData(fileURL);
                    }
                    filePath = Utilities.SaveCompanyFileFromDashboard(checkoutdet.ReferenceId, bytes, insuranceCompany.Key, true,
                           _tameenkConfig.RemoteServerInfo.UseNetworkDownload,
                           _tameenkConfig.RemoteServerInfo.DomainName,
                           _tameenkConfig.RemoteServerInfo.ServerIP,
                           _tameenkConfig.RemoteServerInfo.ServerUserName,
                           _tameenkConfig.RemoteServerInfo.ServerPassword,
                           isAutoleasingPolicy,
                           out exception);
                    if (!string.IsNullOrWhiteSpace(exception))
                    {
                        output.ErrorCode = AddBenefitOutput.ErrorCodes.InvalidData;
                        output.ErrorDescription = "failed to save file";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "failed to save file duo to exception : " + exception;
                        PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                        return output;
                    }
                    return output;
                }

                else if (results.EndorsementFile != null)
                {
                    filePath = Utilities.SaveCompanyFileFromDashboard(checkoutdet.ReferenceId, results.EndorsementFile, insuranceCompany.Key, false,
                      _tameenkConfig.RemoteServerInfo.UseNetworkDownload,
                      _tameenkConfig.RemoteServerInfo.DomainName,
                      _tameenkConfig.RemoteServerInfo.ServerIP,
                      _tameenkConfig.RemoteServerInfo.ServerUserName,
                      _tameenkConfig.RemoteServerInfo.ServerPassword,
                      isAutoleasingPolicy,
                      out exception);
                    if (!string.IsNullOrWhiteSpace(exception))
                    {
                        output.ErrorCode = AddBenefitOutput.ErrorCodes.InvalidData;
                        output.ErrorDescription = "failed to save file";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "failed to save file duo to exception : ";
                        PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                        return output;
                    }
                    benefitFile.FilePath = filePath;
                    benefitFile.InsurranceCompanyId = insuranceCompany.InsuranceCompanyID;
                    benefitFile.PolicyModificationRequestId = request.Id;
                    benefitFile.ReferenceId = checkoutdet.ReferenceId;
                    benefitFile.Channel = log.Channel;
                    benefitFile.ServerIP = log.ServerIP;
                    benefitFile.Headers["User-Agent"].ToString() = log.Headers["User-Agent"].ToString();
                    benefitFile.UserIP = log.UserIP;
                    benefitFile.CreatedDate = DateTime.Now;
                    benefitFile.QuotationReferenceId = request.QuotationReferenceId;
                    _endormentRepository.Insert(benefitFile);
                    return output;

                }
                else
                {
                    var orderBenefits = _orderItemBenefitRepository.TableNoTracking.Where(x => x.OrderItemId == orderItems.Id);
                    Endorsment endorsment = new Endorsment()
                    {
                        FilePath = filePath,
                        InsurranceCompanyId = insuranceCompany.InsuranceCompanyID,
                        PolicyModificationRequestId = request.Id,
                        ReferenceId = request.ReferenceId,
                        Channel = log.Channel,
                        ServerIP = log.ServerIP,
                        UserAgent = log.Headers["User-Agent"].ToString(),
                        UserIP = log.UserIP,
                        CreatedDate = DateTime.Now,
                        QuotationReferenceId = request.QuotationReferenceId
                    };
                    _endormentRepository.Insert(endorsment);
                    if (endorsment.Id < 1)
                    {
                        output.ErrorCode = AddBenefitOutput.ErrorCodes.DriverDataError;
                        output.ErrorDescription = "Failed to insert Endorsment request";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Failed to insert Endorsment request";
                        PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                        return output;
                    }
                    List<EndorsmentBenefit> endorsmentBenefits = new List<EndorsmentBenefit>();
                    List<OrderItemBenefit> orderItemBenefits = new List<OrderItemBenefit>();

                    List<PolicyAdditionalBenefit> selected_benefits = new List<PolicyAdditionalBenefit>();
                    foreach (var benefit in model.Benefits.ToList())
                    {
                        var benfit = _policyAdditionalBenefitRepository.TableNoTracking.Where(x => x.BenefitId == benefit.BenefitId).FirstOrDefault();
                        if (benfit != null)
                        {
                            selected_benefits.Add(benfit);
                        }
                    }

                    foreach (var item in selected_benefits)
                    {
                        OrderItemBenefit orderItemBenefit = new OrderItemBenefit();
                        short BenefitIdconvert;
                        short.TryParse(item.BenefitCode.ToString(), out BenefitIdconvert);
                        orderItemBenefit.Price = (item.BenefitPrice != null) ? (decimal)item.BenefitPrice : 0.0M;
                        orderItemBenefit.BenefitExternalId = item.BenefitId;
                        orderItemBenefit.OrderItemId = orderItems.Id;
                        orderItemBenefit.BenefitId = (short)item.BenefitCode;
                        orderItemBenefit.BenefitId = BenefitIdconvert;

                        orderItemBenefits.Add(orderItemBenefit);
                    }
                    foreach (var benefit in model.Benefits)
                    {

                        EndorsmentBenefit endorsmentBenefit = new EndorsmentBenefit()
                        {
                            BenefitId = benefit.BenefitId,
                            CreatedDate = DateTime.Now,
                            EndorsmentId = endorsment.Id,
                            QuotationReferenceId = endorsment.QuotationReferenceId,
                            ReferenceId = endorsment.ReferenceId
                        };
                        endorsmentBenefits.Add(endorsmentBenefit);
                    }
                    _orderItemBenefitRepository.Insert(orderItemBenefits);
                    _endormentBenefitRepository.Insert(endorsmentBenefits);

                    checkoutdet.PolicyStatusId = 7;
                    checkoutdet.IsCancelled = false;
                    _checkoutDetailrepository.Update(checkoutdet);
                    var processingqueue = _policyProcessingQueue.Table.FirstOrDefault(x => x.ReferenceId == checkoutdet.ReferenceId);
                    processingqueue.ProcessedOn = null;
                    processingqueue.ProcessingTries = 0;
                    processingqueue.ServiceResponse = null;
                    processingqueue.ServiceResponseTimeInSeconds = null;
                    processingqueue.ServerIP = null;
                    processingqueue.IsCancelled = false;
                    processingqueue.IsLocked = false;
                    processingqueue.ModifiedDate = null;
                    processingqueue.CreatedDate = null;
                    processingqueue.ErrorDescription = null;
                    processingqueue.DriverNin = null;
                    processingqueue.ServiceRequest = null;
                    processingqueue.VehicleId = null;
                    _policyProcessingQueue.Update(processingqueue);
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
            }
            catch (Exception ex)
            {
                output.ErrorCode = AddBenefitOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = "Failed to Request Purchase benefits";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                return output;
            }
        }

        #endregion

        //#region Admin Purchase Benefit
        //public AddBenefitOutput PurchaseBenefit(PurchaseBenefitModel model, string UserId, string userName)
        //{
        //    AddBenefitOutput output = new AddBenefitOutput();
        //    PolicyModificationLog log = new PolicyModificationLog();
        //    log.UserIP = Utilities.GetUserIPAddress();
        //    log.ServerIP = Utilities.GetInternalServerIP();
        //    log.UserAgent = Utilities.GetUserAgent();
        //    log.Channel = model?.Channel.ToString();
        //    log.MethodName = "PurchaseBenefit";
        //    if (!string.IsNullOrEmpty(userName))
        //    {
        //        log.UserId = UserId.ToString();
        //        log.UserName = userName;
        //    }
        //    try
        //    {
        //        if (model == null)
        //        {
        //            output.ErrorCode = AddBenefitOutput.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = SubmitInquiryResource.RequestModelIsNullException;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "requestModel Is null";
        //            PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
        //            return output;
        //        }
        //        if (string.IsNullOrWhiteSpace(model.ReferenceId))
        //        {
        //            output.ErrorCode = AddBenefitOutput.ErrorCodes.ParkingLocationIdIsNull;
        //            output.ErrorDescription = "ReferenceId is required";
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "ReferenceId is required";
        //            PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
        //            return output;
        //        }
        //        var request = _policyModificationrepository.Table.FirstOrDefault(x => x.ReferenceId == model.ReferenceId);
        //        if (request == null)
        //        {
        //            output.ErrorCode = AddBenefitOutput.ErrorCodes.InvalidData;
        //            output.ErrorDescription = "request  Not exist";
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "request  Not exist ";
        //            PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
        //            return output;
        //        }
        //        var insuranceCompany = _insuranceCompanyService.GetById(request.InsuranceCompanyId.Value);
        //        if (insuranceCompany == null)
        //        {
        //            output.ErrorCode = AddBenefitOutput.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = SubmitInquiryResource.insuranceCompanyId;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "insurance Company is null";
        //            PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
        //            return output;
        //        }
        //        log.CompanyName = insuranceCompany.Key;
        //        log.CompanyId = insuranceCompany.InsuranceCompanyID;
        //        Guid userId = Guid.Empty;
        //        Guid.TryParse(UserId, out userId);
        //        ServiceRequestLog predefinedLogInfo = new ServiceRequestLog
        //        {
        //            UserID = userId,
        //            UserName = userName,
        //            Channel = log.Channel,
        //            ServerIP = log.ServerIP,
        //            CompanyID = log.CompanyId,
        //            CompanyName = log.CompanyName,
        //            ReferenceId = request.QuotationReferenceId
        //        };
        //        //Calculate total ammount of benefits 
        //        CheckoutDetail c = _checkoutDetailrepository.Table.FirstOrDefault(x => x.ReferenceId == request.QuotationReferenceId);
        //        decimal totalAmmount = 0;
        //        foreach (var benefit in model.Benefits)
        //        {
        //            var orderItems = _orderItemRepository.TableNoTracking.Where(x => x.CheckoutDetailReferenceId == request.QuotationReferenceId);
        //            if (orderItems != null && orderItems.Count() != 0)
        //            {
        //                var orderBenefits = _orderItemBenefitRepository.TableNoTracking.Where(x => x.OrderItemId == orderItems.FirstOrDefault().Id);
        //                if (orderBenefits != null && orderBenefits.Count() != 0)
        //                {
        //                    if (orderBenefits.Any(x => x.BenefitExternalId == benefit.BenefitId))
        //                    {
        //                        model.Benefits.Remove(benefit);
        //                    }
        //                    else
        //                    {
        //                        totalAmmount += _policyAdditionalBenefitRepository.TableNoTracking.Where(x => x.BenefitId == benefit.BenefitId).Select(x => x.BenefitPrice).FirstOrDefault().Value;
        //                    }
        //                }
        //            }

        //        }
        //        PurchaseBenefitRequest serviceRequest = new PurchaseBenefitRequest();
        //        serviceRequest.PolicyNo = request.PolicyNo;
        //        serviceRequest.ReferenceId = request.ReferenceId;
        //        serviceRequest.PaymentAmount = (double)totalAmmount;
        //        serviceRequest.PaymentBillNumber = request.InvoiceNo.ToString();
        //        serviceRequest.Benefits = model.Benefits;
        //        IInsuranceProvider provider = GetProvider(insuranceCompany, request.InsuranceTypeCode.Value);
        //        if (provider == null)
        //        {
        //            output.ErrorCode = AddBenefitOutput.ErrorCodes.InvalidData;
        //            output.ErrorDescription = "provider is null";
        //            log.ErrorDescription = "provider is null";
        //            PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
        //            return output;
        //        }
        //        var results = provider.PurchaseBenefit(serviceRequest, predefinedLogInfo);
        //        if (results == null)
        //        {
        //            output.ErrorCode = AddBenefitOutput.ErrorCodes.ServiceDown;
        //            output.ErrorDescription = "Service Return Null ";
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "Service Return Null ";
        //            PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
        //            return output;
        //        }
        //        if (results.StatusCode != (int)AddBenefitOutput.ErrorCodes.Success)
        //        {
        //            output.ErrorCode = AddBenefitOutput.ErrorCodes.ServiceDown;
        //            output.ErrorDescription = "Failed to get Response as status code =  " + results.StatusCode;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "Failed to get Response as status code =  " + results.StatusCode;
        //            PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
        //            return output;
        //        }
        //        string fileURL = results.EndorsementFileUrl;
        //        fileURL = fileURL.Replace(@"\\", @"//");
        //        fileURL = fileURL.Replace(@"\", @"/");
        //        byte[] bytes = null;
        //        using (System.Net.WebClient client = new System.Net.WebClient())
        //        {
        //            bytes = client.DownloadData(fileURL);
        //        }
        //        string filePath = Utilities.SaveCompanyFileFromDashboard(c.ReferenceId, bytes, insuranceCompany.Key, false,
        //                _tameenkConfig.RemoteServerInfo.UseNetworkDownload,
        //                _tameenkConfig.RemoteServerInfo.DomainName,
        //                _tameenkConfig.RemoteServerInfo.ServerIP,
        //                _tameenkConfig.RemoteServerInfo.ServerUserName,
        //                _tameenkConfig.RemoteServerInfo.ServerPassword,
        //                out string exception);
        //        if (!string.IsNullOrWhiteSpace(exception))
        //        {
        //            output.ErrorCode = AddBenefitOutput.ErrorCodes.InvalidData;
        //            output.ErrorDescription = "failed to save file";
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "failed to save file duo to exception : " + exception;
        //            PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
        //            return output;
        //        }
        //        Endorsment endorsment = new Endorsment()
        //        {
        //            FilePath = filePath,
        //            InsurranceCompanyId = insuranceCompany.InsuranceCompanyID,
        //            PolicyModificationRequestId = request.Id,
        //            ReferenceId = request.ReferenceId,
        //            Channel = log.Channel,
        //            ServerIP = log.ServerIP,
        //            UserAgent = log.UserAgent,
        //            UserIP = log.UserIP,
        //            CreatedDate = DateTime.Now,
        //            QuotationReferenceId = request.QuotationReferenceId
        //        };
        //        _endormentRepository.Insert(endorsment);
        //        if (endorsment.Id < 1)
        //        {
        //            output.ErrorCode = AddBenefitOutput.ErrorCodes.DriverDataError;
        //            output.ErrorDescription = "Failed to insert Endorsment request";
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "Failed to insert Endorsment request";
        //            PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
        //            return output;
        //        }
        //        List<EndorsmentBenefit> endorsmentBenefits = new List<EndorsmentBenefit>();
        //        foreach (var benefit in model.Benefits)
        //        {
        //            EndorsmentBenefit endorsmentBenefit = new EndorsmentBenefit()
        //            {
        //                BenefitId = benefit.BenefitId,
        //                CreatedDate = DateTime.Now,
        //                EndorsmentId = endorsment.Id,
        //                QuotationReferenceId = endorsment.QuotationReferenceId,
        //                ReferenceId = endorsment.ReferenceId
        //            };
        //            endorsmentBenefits.Add(endorsmentBenefit);
        //        }
        //        _endormentBenefitRepository.Insert(endorsmentBenefits);
        //        output.ErrorCode = AddBenefitOutput.ErrorCodes.Success;
        //        output.ErrorDescription = "Success";
        //        log.ErrorCode = (int)output.ErrorCode;
        //        log.ErrorDescription = output.ErrorDescription;
        //        PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
        //        return output;
        //    }
        //    catch (Exception ex)
        //    {
        //        output.ErrorCode = AddBenefitOutput.ErrorCodes.ServiceException;
        //        output.ErrorDescription = "Failed to Request Purchase benefits";
        //        log.ErrorCode = (int)output.ErrorCode;
        //        log.ErrorDescription = ex.ToString();
        //        PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
        //        return output;
        //    }
        //}

        //#endregion

        #region Autoleasing Bulk Renewal

        public List<AutoleasingRenewalPoliciesModel> GetBankRenewalPolicies(int bankId, string nin, DateTime? startDate, DateTime? endDate, out string exception ,int channel=(int)Channel.Portal, bool isExcel = false)
        {
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAutoleasingBankRenewalPolicies";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 120;
                SqlParameter bankIdParameter = new SqlParameter() { ParameterName = "bankId", Value = bankId };
                command.Parameters.Add(bankIdParameter);

                if (!string.IsNullOrEmpty(nin))
                {
                    SqlParameter nationalIdParameter = new SqlParameter() { ParameterName = "nationalId", Value = nin };
                    command.Parameters.Add(nationalIdParameter);
                }
                if (startDate != null && startDate.HasValue)
                {
                    DateTime dtStart = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 0, 0, 0);
                    SqlParameter dtStartParameter = new SqlParameter() { ParameterName = "dateFrom", Value = dtStart };
                    command.Parameters.Add(dtStartParameter);
                }
                if (endDate != null && endDate.HasValue)
                {
                    DateTime dtEnd = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 23, 59, 59);
                    SqlParameter dtEndParameter = new SqlParameter() { ParameterName = "dateTo", Value = dtEnd };
                    command.Parameters.Add(dtEndParameter);
                }
                if (channel >= 0)
                {
                    SqlParameter channelParameter = new SqlParameter() { ParameterName = "channel", Value = channel };
                    command.Parameters.Add(channelParameter);
                }
                if (isExcel)
                {
                    SqlParameter isExportParameter = new SqlParameter() { ParameterName = "isExcel", Value = isExcel };
                    command.Parameters.Add(isExportParameter);
                }
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<AutoleasingRenewalPoliciesModel> data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<AutoleasingRenewalPoliciesModel>(reader).ToList();
                if (data == null || data.Count == 0)
                {
                    dbContext.DatabaseInstance.Connection.Close();
                    return new List<AutoleasingRenewalPoliciesModel>();
                }

                reader.NextResult();
                List<AutoleasingRenewalPoliciesOldBenefitsModel> benefits = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<AutoleasingRenewalPoliciesOldBenefitsModel>(reader).ToList();
                dbContext.DatabaseInstance.Connection.Close();

                if (benefits != null && benefits.Count > 0)
                {
                    foreach (var item in data)
                    {
                        item.Benefits = benefits.Where(a => a.ProductId == item.ProductId).ToList();
                    }
                }

                return data;
            }
            catch (Exception exp)
            {
                dbContext.DatabaseInstance.Connection.Close();
                exception = exp.ToString();
                return new List<AutoleasingRenewalPoliciesModel>();
            }
        }

        public List<InquiryRequestModel> HandleRenewalBulkData(int bankId, List<AutoleasingRenewalPoliciesModel> data, out string exception)
        {
            exception = string.Empty;
            try
            {
                List<InquiryRequestModel> result = new List<InquiryRequestModel>();
                foreach (var item in data)
                {
                    long vehicleId = 0;
                    bool isCustomCardConverted = false;
                    if (item.ConvertedSequenceNumber != null && item.ConvertedSequenceNumber.HasValue)
                    {
                        isCustomCardConverted = true;
                        long.TryParse(item.ConvertedSequenceNumber.Value.ToString(), out vehicleId);
                    }
                    else if (!string.IsNullOrEmpty(item.CustomCardNumber))
                    {
                        CustomCardQueue customCardInfo = new CustomCardQueue();
                        customCardInfo.UserId = item.UserId;
                        customCardInfo.CustomCardNumber = item.CustomCardNumber;
                        customCardInfo.ModelYear = item.ModelYear;
                        customCardInfo.Channel = "autoleasing";
                        customCardInfo.CompanyID = item.InsuranceCompanyId;
                        customCardInfo.CompanyName = item.InsuranceCompanyName;
                        customCardInfo.VehicleId = item.ID;
                        customCardInfo.ReferenceId = item.ReferenceId;
                        customCardInfo.PolicyNo = item.PolicyNo;
                        var customCardOutput = GetCustomCardInfo(customCardInfo, item, out exception);
                        if (!customCardOutput || !string.IsNullOrEmpty(exception))
                            continue;

                        isCustomCardConverted = true;
                        long.TryParse(item.SequenceNumber, out vehicleId);
                    }
                    else
                        long.TryParse(item.SequenceNumber, out vehicleId);

                    var policyInfo = vehicleService.GetVehiclePolicy(vehicleId.ToString(), out exception);
                    if (!string.IsNullOrEmpty(exception))
                        continue;

                    if (policyInfo == null && isCustomCardConverted)
                        policyInfo = vehicleService.GetVehiclePolicy(item.CustomCardNumber, out exception);

                    if (policyInfo == null || !policyInfo.PolicyExpiryDate.HasValue)
                        continue;

                    DateTime end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddDays(28);
                    if (policyInfo.PolicyExpiryDate.Value.Date.Year > end.Year) // policy already valid or renewed with reference " + policyInfo.CheckOutDetailsId
                        continue;

                    var singleModel = new InquiryRequestModel();
                    singleModel.PolicyExpiryDate = item.PolicyExpiryDate;
                    singleModel.PolicyEffectiveDate = (item.PolicyExpiryDate.HasValue && DateTime.Compare(DateTime.Now, item.PolicyExpiryDate.Value) < 0) 
                                                        ? item.PolicyExpiryDate.Value.AddDays(1)
                                                        : DateTime.Now.AddDays(1);
                    singleModel.CityCode = 1;
                    singleModel.IsBulkOption = true;
                    singleModel.IsEditRequest = true;
                    singleModel.IsRenewalRequest = true;
                    singleModel.ExternalId = item.ExternalId;
                    singleModel.PreviousReferenceId = item.ReferenceId;
                    singleModel.ContractDuration = item.AutoleasingContractDuration.Value;
                    singleModel.IsCustomerSpecialNeed = false;
                    singleModel.DeductibleValue = (item.DeductableValue.HasValue) ? item.DeductableValue.Value : 2000;
                    singleModel.VehicleAgencyRepair = item.IsAgencyRepair;
                    singleModel.PurchasedBenefits = item.Benefits;

                    long ownerNin = 0;
                    long.TryParse(item.CarOwnerNIN, out ownerNin);
                    if (item.OwnerTransfer && ownerNin > 0)
                        singleModel.OldOwnerNin = ownerNin;

                    #region Insured

                    singleModel.Insured = new InsuredModel() { NationalId = "0" };

                    #endregion

                    #region Drivers

                    // default driver
                    singleModel.Drivers = new List<DriverModel> { new DriverModel() { NationalId = "0" } };

                    // main driver data
                    DriverModel mainDriver = new DriverModel();
                    mainDriver.NationalId = item.NIN;
                    mainDriver.MedicalConditionId = item.MedicalConditionId.Value;
                    mainDriver.EducationId = item.EducationId;
                    mainDriver.ChildrenBelow16Years = item.ChildrenBelow16Years.Value;
                    mainDriver.DrivingPercentage = 100; // item.DrivingPercentage.Value;
                    HandleDriverBirthMonth(mainDriver, item.DateOfBirthH, item.DateOfBirthG);
                    mainDriver.EducationId = item.EducationId;
                    mainDriver.DriverNOALast5Years = item.NOALast5Years;

                    if (item.WorkCityId.HasValue)
                    {
                        int workCity = 0;
                        int.TryParse(item.WorkCityId.Value.ToString(), out workCity);
                        mainDriver.DriverWorkCityCode = workCity;
                    }
                    if (item.CityId.HasValue)
                    {
                        int homeCity = 0;
                        int.TryParse(item.CityId.Value.ToString(), out homeCity);
                        mainDriver.DriverHomeCityCode = homeCity;
                    }

                    mainDriver.DriverWorkCity = item.WorkCityName;
                    mainDriver.DriverHomeCity = item.CityName;
                    mainDriver.IsCompanyMainDriver = true;
                    mainDriver.RelationShipId = item.RelationShipId;
                    mainDriver.MobileNo = item.MobileNumber;
                    singleModel.Drivers.Add(mainDriver);

                    // additional driver 1 data
                    if (!string.IsNullOrEmpty(item.NINDriver1))
                    {
                        DriverModel driver1 = new DriverModel();
                        driver1.NationalId = item.NINDriver1;
                        driver1.MedicalConditionId = item.MedicalConditionIdDriver1.Value;
                        if (item.EducationIdDriver1.HasValue)
                            driver1.EducationId = item.EducationIdDriver1.Value;
                        driver1.ChildrenBelow16Years = item.ChildrenBelow16YearsDriver1.Value;
                        driver1.DrivingPercentage = item.DrivingPercentageDriver1.Value;
                        if (!string.IsNullOrEmpty(item.DateOfBirthHDriver1) && item.DateOfBirthGDriver1.HasValue)
                            HandleDriverBirthMonth(driver1, item.DateOfBirthHDriver1, item.DateOfBirthGDriver1.Value);
                        if (item.EducationIdDriver1.HasValue)
                            driver1.EducationId = item.EducationIdDriver1.Value;
                        driver1.DriverNOALast5Years = item.NOALast5YearsDriver1;

                        if (item.WorkCityIdDriver1.HasValue)
                        {
                            int workCity = 0;
                            int.TryParse(item.WorkCityIdDriver1.Value.ToString(), out workCity);
                            driver1.DriverWorkCityCode = workCity;
                        }
                        if (item.CityIdDriver1.HasValue)
                        {
                            int homeCity = 0;
                            int.TryParse(item.CityIdDriver1.Value.ToString(), out homeCity);
                            driver1.DriverHomeCityCode = homeCity;
                        }

                        driver1.DriverWorkCity = item.WorkCityNameDriver1;
                        driver1.DriverHomeCity = item.CityNameDriver1;
                        driver1.RelationShipId = item.RelationShipIdDriver1;
                        driver1.MobileNo = item.MobileNumberDriver1;
                        singleModel.Drivers.Add(driver1);
                    }

                    // additional driver 2 data
                    if (!string.IsNullOrEmpty(item.NINDriver2))
                    {
                        DriverModel driver2 = new DriverModel();
                        driver2.NationalId = item.NINDriver2;
                        driver2.MedicalConditionId = item.MedicalConditionIdDriver2.Value;
                        if (item.EducationIdDriver2.HasValue)
                            driver2.EducationId = item.EducationIdDriver2.Value;
                        driver2.ChildrenBelow16Years = item.ChildrenBelow16YearsDriver2.Value;
                        driver2.DrivingPercentage = item.DrivingPercentageDriver2.Value;
                        if (!string.IsNullOrEmpty(item.DateOfBirthHDriver2) && item.DateOfBirthGDriver2.HasValue)
                            HandleDriverBirthMonth(driver2, item.DateOfBirthHDriver2, item.DateOfBirthGDriver2.Value);
                        if (item.EducationIdDriver2.HasValue)
                            driver2.EducationId = item.EducationIdDriver2.Value;
                        driver2.DriverNOALast5Years = item.NOALast5YearsDriver2;

                        if (item.WorkCityIdDriver2.HasValue)
                        {
                            int workCity = 0;
                            int.TryParse(item.WorkCityIdDriver2.Value.ToString(), out workCity);
                            driver2.DriverWorkCityCode = workCity;
                        }
                        if (item.CityIdDriver2.HasValue)
                        {
                            int homeCity = 0;
                            int.TryParse(item.CityIdDriver2.Value.ToString(), out homeCity);
                            driver2.DriverHomeCityCode = homeCity;
                        }

                        driver2.DriverWorkCity = item.WorkCityNameDriver2;
                        driver2.DriverHomeCity = item.CityNameDriver2;
                        driver2.RelationShipId = item.RelationShipIdDriver2;
                        driver2.MobileNo = item.MobileNumberDriver2;
                        singleModel.Drivers.Add(driver2);
                    }

                    HandleDriversDrivingpercentage(singleModel, item);

                    #endregion

                    #region Vehicle

                    decimal vaehicleValue = 0;
                    decimal.TryParse(item.VehicleValue.ToString(), out vaehicleValue);

                    singleModel.Vehicle = new VehicleModel();
                    singleModel.Vehicle.VehicleId = vehicleId;
                    singleModel.Vehicle.vehiclePrice = vaehicleValue;
                    singleModel.Vehicle.VehicleMaker = item.VehicleMaker;
                    singleModel.Vehicle.VehicleMakerCode = item.VehicleMakerCode;
                    singleModel.Vehicle.Model = item.VehicleModel;
                    singleModel.Vehicle.VehicleModelCode = item.VehicleModelCode;
                    singleModel.Vehicle.VehicleModelYear = item.ModelYear;
                    singleModel.Vehicle.CarPlateText1 = item.CarPlateText1;
                    singleModel.Vehicle.CarPlateText2 = item.CarPlateText2;
                    singleModel.Vehicle.CarPlateText3 = item.CarPlateText3;
                    singleModel.Vehicle.CarPlateNumber = item.CarPlateNumber;
                    singleModel.Vehicle.PlateTypeCode = item.PlateTypeCode;
                    singleModel.Vehicle.ID = item.ID;
                    singleModel.Vehicle.VehicleIdTypeId = item.VehicleIdTypeId;
                    singleModel.Vehicle.Cylinders = item.Cylinders;
                    singleModel.Vehicle.LicenseExpiryDate = item.LicenseExpiryDate;
                    singleModel.Vehicle.MajorColor = item.MajorColor;
                    singleModel.Vehicle.MinorColor = item.MinorColor;
                    singleModel.Vehicle.ModelYear = item.ModelYear;
                    singleModel.Vehicle.ManufactureYear = item.ModelYear;
                    singleModel.Vehicle.RegisterationPlace = item.RegisterationPlace;
                    singleModel.Vehicle.VehicleBodyCode = item.VehicleBodyCode;
                    singleModel.Vehicle.VehicleWeight = item.VehicleWeight;
                    singleModel.Vehicle.VehicleLoad = item.VehicleLoad;
                    singleModel.Vehicle.ChassisNumber = item.ChassisNumber;
                    singleModel.Vehicle.HasModification = item.HasModifications;
                    singleModel.Vehicle.Modification = item.ModificationDetails;
                    singleModel.Vehicle.OwnerTransfer = item.OwnerTransfer;
                    singleModel.Vehicle.OwnerNationalId = item.CarOwnerNIN;

                    AutoleasingDepreciationSetting depreciationSetting = null;
                    var depreciationSettingHistory = _autoleasingDepreciationSettingRepositoryHistory.TableNoTracking.FirstOrDefault(a => a.ExternalId == item.ExternalId);
                    if (depreciationSettingHistory != null)
                    {
                        depreciationSetting = new AutoleasingDepreciationSetting()
                        {
                            BankId = depreciationSettingHistory.BankId,
                            MakerCode = depreciationSettingHistory.MakerCode,
                            ModelCode = depreciationSettingHistory.ModelCode,
                            MakerName = depreciationSettingHistory.MakerName,
                            ModelName = depreciationSettingHistory.ModelName,
                            Percentage = depreciationSettingHistory.Percentage,
                            IsDynamic = depreciationSettingHistory.IsDynamic,
                            FirstYear = depreciationSettingHistory.FirstYear,
                            SecondYear = depreciationSettingHistory.SecondYear,
                            ThirdYear = depreciationSettingHistory.ThirdYear,
                            FourthYear = depreciationSettingHistory.FourthYear,
                            FifthYear = depreciationSettingHistory.FifthYear,
                            AnnualDepreciationPercentage = depreciationSettingHistory.AnnualDepreciationPercentage,
                        };
                    }

                    if (depreciationSetting == null)
                        depreciationSetting = _autoleasingDepreciationSettingRepository.TableNoTracking.FirstOrDefault(a => a.BankId == bankId && a.MakerCode == item.VehicleMakerCode && a.ModelCode == item.VehicleModelCode);
                    if (depreciationSetting == null)
                        depreciationSetting = _autoleasingDepreciationSettingRepository.TableNoTracking.FirstOrDefault(a => a.BankId == bankId && a.MakerCode == 0 && a.ModelCode == 0);

                    if (depreciationSetting != null)
                    {
                        exception = string.Empty;
                        var _vehicelId = (item.VehicleIdTypeId == (int)VehicleIdType.CustomCard) ? item.CustomCardNumber : item.SequenceNumber;
                        //int previousPoliciesCount = GetVehiclePoliciesCount(_vehicelId, item.NIN, bankId);
                        int previousPoliciesCount = GetVehiclePoliciesCount(_vehicelId, null, null);
                        singleModel.Vehicle.PoliciesCount = previousPoliciesCount;
                        var approximateValue = HandleRenewalVehicelValue(item.ExternalId, vaehicleValue, depreciationSetting, previousPoliciesCount + 1, out exception);
                        if (approximateValue == 0 || !string.IsNullOrEmpty(exception))
                            singleModel.Vehicle.ApproximateValue = vaehicleValue;
                        else
                            singleModel.Vehicle.ApproximateValue = approximateValue;
                    }
                    else
                        singleModel.Vehicle.ApproximateValue = vaehicleValue;

                    if (item.TransmissionTypeId.HasValue)
                        singleModel.Vehicle.TransmissionTypeId = item.TransmissionTypeId.Value;
                    if (item.ParkingLocationId.HasValue)
                        singleModel.Vehicle.ParkingLocationId = item.ParkingLocationId.Value;

                    if (item.MileageExpectedAnnualId.HasValue)
                        singleModel.Vehicle.MileageExpectedAnnualId = item.MileageExpectedAnnualId.Value;

                    if (item.ConvertedSequenceNumber != null && item.ConvertedSequenceNumber.HasValue)
                    {
                        singleModel.Vehicle.IsCustomCardConverted = true;
                        singleModel.Vehicle.VehicleIdTypeId = (int)VehicleIdType.SequenceNumber;
                        singleModel.Vehicle.SequenceNumber = item.ConvertedSequenceNumber.Value.ToString();
                        singleModel.Vehicle.CustomCardNumber = string.Empty;

                        var CarPlate = new Tamkeen.bll.Model.CarPlateInfo(item.ConvertedCarPlateText1, item.ConvertedCarPlateText2, item.ConvertedCarPlateText3, item.ConvertedCarPlateNumber.HasValue ? item.ConvertedCarPlateNumber.Value : 0);
                        singleModel.Vehicle.CarPlateNumberAr = CarPlate.CarPlateNumberAr;
                        singleModel.Vehicle.CarPlateNumberEn = CarPlate.CarPlateNumberEn;
                        singleModel.Vehicle.CarPlateTextAr = CarPlate.CarPlateTextAr;
                        singleModel.Vehicle.CarPlateTextEn = CarPlate.CarPlateTextEn;
                    }
                    else
                    {
                        singleModel.Vehicle.IsCustomCardConverted = false;
                        singleModel.Vehicle.SequenceNumber = (item.VehicleIdTypeId == (int)VehicleIdType.SequenceNumber) ? item.SequenceNumber : string.Empty;
                        singleModel.Vehicle.CustomCardNumber = (item.VehicleIdTypeId == (int)VehicleIdType.CustomCard) ? item.CustomCardNumber : string.Empty;

                        var CarPlate = new Tamkeen.bll.Model.CarPlateInfo(item.CarPlateText1, item.CarPlateText2, item.CarPlateText3, item.CarPlateNumber.HasValue ? item.CarPlateNumber.Value : 0);
                        singleModel.Vehicle.CarPlateNumberAr = CarPlate.CarPlateNumberAr;
                        singleModel.Vehicle.CarPlateNumberEn = CarPlate.CarPlateNumberEn;
                        singleModel.Vehicle.CarPlateTextAr = CarPlate.CarPlateTextAr;
                        singleModel.Vehicle.CarPlateTextEn = CarPlate.CarPlateTextEn;
                    }

                    #endregion

                    result.Add(singleModel);
                }

                return result;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return null;
            }
        }

        private void HandleDriverBirthMonth(DriverModel result, string birthDataH, DateTime birthDateG)
        {
            if (result.NationalId.StartsWith("1"))
            {
                var splitedData = birthDataH.Split('-');

                byte month = 0;
                byte.TryParse(splitedData[1], out month);
                result.BirthDateMonth = month;

                short year = 0;
                short.TryParse(splitedData[2], out year);
                result.BirthDateYear = year;
            }
            else if (result.NationalId.StartsWith("2"))
            {
                byte month = 0;
                byte.TryParse(birthDateG.Month.ToString(), out month);
               // result.BirthDateMonth = month;
                result.BirthDateMonth = month;

                short year = 0;
                short.TryParse(birthDateG.Year.ToString(), out year);
                result.BirthDateYear = year;
            }
        }

        private void HandleDriversDrivingpercentage(InquiryRequestModel singleModel, AutoleasingRenewalPoliciesModel item)
        {
            var drivers = singleModel.Drivers.Where(a => a.NationalId != "0").ToList();
            int numberOfDriver = drivers.Count;
            if (numberOfDriver > 1)
            {
                int percentage = 0;
                int mainPercentage = 0;
                if (numberOfDriver == 3)
                {
                    mainPercentage = 50;
                    percentage = 25;
                }
                else if (numberOfDriver == 2)
                {
                    mainPercentage = percentage = 50;
                }

                foreach (var d in drivers)
                {
                    if (d.IsCompanyMainDriver)
                        d.DrivingPercentage = mainPercentage;
                    else
                        d.DrivingPercentage = percentage;
                }
            }
        }

        private int GetVehiclePoliciesCount(string vehicelId, string nin, int? bankId)
        {
            int data = 0;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAutoleasingVehiclePoliciesCount";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 120;

                SqlParameter vehicelIdParameter = new SqlParameter() { ParameterName = "vehicleId", Value = vehicelId };
                command.Parameters.Add(vehicelIdParameter);

                if (!string.IsNullOrEmpty(nin))
                {
                    SqlParameter ninParameter = new SqlParameter() { ParameterName = "nin", Value = nin };
                    command.Parameters.Add(ninParameter);
                }

                if (bankId.HasValue)
                {
                    SqlParameter bankIdParameter = new SqlParameter() { ParameterName = "bankId", Value = bankId };
                    command.Parameters.Add(bankIdParameter);
                }

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();

                return data;
            }
            catch (Exception exp)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\GetAutoleasingVehiclePoliciesCount" + vehicelId + "_error.txt", JsonConvert.SerializeObject(exp.ToString()));
                dbContext.DatabaseInstance.Connection.Close();
                return data;
            }
        }

        private decimal HandleRenewalVehicelValue(string externalId, decimal vehicleValue, AutoleasingDepreciationSetting depreciationSetting, int currentYear, out string exception)
        {
            exception = string.Empty;
            try
            {
                Decimal? DepreciationValue1 = 0;
                Decimal? DepreciationValue2 = 0;
                Decimal? DepreciationValue3 = 0;
                Decimal? DepreciationValue4 = 0;
                Decimal? DepreciationValue5 = 0;

                if (depreciationSetting.AnnualDepreciationPercentage == "Reducing Balance")
                {
                    DepreciationValue1 = vehicleValue;

                    if (depreciationSetting.IsDynamic)
                    {
                        if (depreciationSetting.SecondYear != 0)
                            DepreciationValue2 = DepreciationValue1 - (DepreciationValue1 * depreciationSetting.SecondYear / 100);

                        if (depreciationSetting.ThirdYear != 0)
                            DepreciationValue3 = DepreciationValue2 - (DepreciationValue2 * depreciationSetting.ThirdYear / 100);

                        if (depreciationSetting.FourthYear != 0)
                            DepreciationValue4 = DepreciationValue3 - (DepreciationValue3 * depreciationSetting.FourthYear / 100);

                        if (depreciationSetting.FifthYear != 0)
                            DepreciationValue5 = DepreciationValue4 - (DepreciationValue4 * depreciationSetting.FifthYear / 100);
                    }
                    else
                    {
                        DepreciationValue2 = DepreciationValue1 - (DepreciationValue1 * depreciationSetting.Percentage / 100);
                        DepreciationValue3 = DepreciationValue2 - (DepreciationValue2 * depreciationSetting.Percentage / 100);
                        DepreciationValue4 = DepreciationValue3 - (DepreciationValue3 * depreciationSetting.Percentage / 100);
                        DepreciationValue5 = DepreciationValue4 - (DepreciationValue4 * depreciationSetting.Percentage / 100);
                    }
                }
                else if (depreciationSetting.AnnualDepreciationPercentage == "Straight Line")
                {
                    if (depreciationSetting.IsDynamic)
                    {

                        if (depreciationSetting.SecondYear != 0)
                            DepreciationValue2 = vehicleValue - (vehicleValue * depreciationSetting.SecondYear / 100);

                        if (depreciationSetting.ThirdYear != 0)
                            DepreciationValue3 = vehicleValue - (vehicleValue * depreciationSetting.ThirdYear / 100);

                        if (depreciationSetting.FourthYear != 0)
                            DepreciationValue4 = vehicleValue - (vehicleValue * depreciationSetting.FourthYear / 100);

                        if (depreciationSetting.FifthYear != 0)
                            DepreciationValue5 = vehicleValue - (vehicleValue * depreciationSetting.FifthYear / 100);
                    }
                    else
                    {
                        DepreciationValue2 = vehicleValue - (vehicleValue * depreciationSetting.Percentage / 100);
                        DepreciationValue3 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 2 / 100);
                        DepreciationValue4 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 3 / 100);
                        DepreciationValue5 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 4 / 100);
                    }
                }

                switch (currentYear)
                {
                    case 1:
                        return DepreciationValue1.Value;

                    case 2:
                        return DepreciationValue2.Value;

                    case 3:
                        return DepreciationValue3.Value;

                    case 4:
                        return DepreciationValue4.Value;

                    case 5:
                        return DepreciationValue5.Value;

                    default:
                        return DepreciationValue1.Value;
                }
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\HandleRenewalVehicelValue_" + externalId + "_error.txt", JsonConvert.SerializeObject(ex.ToString()));
                return 0;
            }
        }

        private bool GetCustomCardInfo(CustomCardQueue customCard, AutoleasingRenewalPoliciesModel item, out string exception)
        {
            exception = string.Empty;
            try
            {
                var customConverted = _customCardInfoRepository.TableNoTracking.Where(x => x.CustomCardNumber == customCard.CustomCardNumber).FirstOrDefault();
                if (customConverted != null)
                {
                    item.CarPlateText1 = customConverted.CarPlateText1;
                    item.CarPlateText2 = customConverted.CarPlateText2;
                    item.CarPlateText3 = customConverted.CarPlateText3;
                    item.CarPlateNumber = customConverted.CarPlateNumber;
                    if (customConverted.SequenceNumber.HasValue)
                    {
                        item.ConvertedSequenceNumber = customConverted.SequenceNumber.Value;
                        item.SequenceNumber = customConverted.SequenceNumber.Value.ToString();
                    }
                    return true;
                }

                Guid userId = Guid.Empty;
                Guid.TryParse(customCard.UserId, out userId);
                ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
                predefinedLogInfo.UserID = userId;
                predefinedLogInfo.Channel = customCard.Channel;
                predefinedLogInfo.ServerIP = Utilities.GetInternalServerIP();
                predefinedLogInfo.CompanyID = customCard.CompanyID.HasValue ? customCard.CompanyID.Value : 0;
                predefinedLogInfo.CompanyName = customCard.CompanyName;
                predefinedLogInfo.Channel = customCard.Channel;
                predefinedLogInfo.VehicleId = customCard.CustomCardNumber;
                predefinedLogInfo.VehicleModelYear = customCard.ModelYear;

                var yakeenOutput = new YakeenVehicleOutput();
                CarInfoCustomTwoDto input = new CarInfoCustomTwoDto()
                {
                    CustomCrdNumber = customCard.CustomCardNumber,
                    ModelYear = customCard.ModelYear
                };
                yakeenOutput = _yakeenClient.CarInfoByCustomTwo(input, predefinedLogInfo);
                if (yakeenOutput == null)
                {
                    exception = "yakeen output is null";
                    return false;
                }
                if (yakeenOutput.ErrorCode != YakeenVehicleOutput.ErrorCodes.Success)
                {
                    exception = "failed from yakeen due to " + yakeenOutput.ErrorDescription;
                    return false;
                }
                if (yakeenOutput.result == null)
                {
                    exception = "yakeenOutput.result is null";
                    return false;
                }
                if (yakeenOutput.result.sequenceNumber == 0)
                {
                    exception = "Custom card still not converted";
                    return false;
                }

                CustomCardInfo customCardInfo = new CustomCardInfo();
                customCardInfo.Id = Guid.NewGuid();
                customCardInfo.CarOwnerName = yakeenOutput.result.ownerName;
                customCardInfo.CarPlateNumber = yakeenOutput.result.plateNumber;
                customCardInfo.CarPlateText1 = yakeenOutput.result.plateText1;
                customCardInfo.CarPlateText2 = yakeenOutput.result.plateText2;
                customCardInfo.CarPlateText3 = yakeenOutput.result.plateText3;
                customCardInfo.ChassisNumber = yakeenOutput.result.chassisNumber;
                customCardInfo.MajorColor = yakeenOutput.result.majorColor;
                customCardInfo.Cylinders = yakeenOutput.result.cylinders;
                customCardInfo.LogId = yakeenOutput.result.logId;
                customCardInfo.PlateTypeCode = yakeenOutput.result.plateType;
                customCardInfo.VehicleCapacity = yakeenOutput.result.vehicleCapacity;
                customCardInfo.VehicleWeight = yakeenOutput.result.vehicleWeight;
                customCardInfo.VehicleModelCode = yakeenOutput.result.vehicleModelCode;
                customCardInfo.VehicleModel = yakeenOutput.result.vehicleModel;
                customCardInfo.VehicleMaker = yakeenOutput.result.vehicleMaker;
                customCardInfo.VehicleMakerCode = yakeenOutput.result.vehicleMakerCode;
                customCardInfo.RegisterationPlace = yakeenOutput.result.regPlace;
                customCardInfo.SequenceNumber = yakeenOutput.result.sequenceNumber;
                customCardInfo.ModelYear = customCard.ModelYear;
                customCardInfo.ReferenceId = customCard.ReferenceId;
                customCardInfo.PolicyNo = customCard.PolicyNo;
                customCardInfo.CustomCardNumber = customCard.CustomCardNumber;
                customCardInfo.VehicleId = customCard.VehicleId;
                customCardInfo.CreatedDate = DateTime.Now;
                _customCardInfoRepository.Insert(customCardInfo);

                item.CarPlateText1 = customCardInfo.CarPlateText1;
                item.CarPlateText2 = customCardInfo.CarPlateText2;
                item.CarPlateText3 = customCardInfo.CarPlateText3;
                item.CarPlateNumber = customCardInfo.CarPlateNumber;
                if (customCardInfo.SequenceNumber.HasValue)
                {
                    item.ConvertedSequenceNumber = customCardInfo.SequenceNumber.Value;
                    item.SequenceNumber = customCardInfo.SequenceNumber.Value.ToString();
                }
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\GetCustomCardInfo_" + customCard.CustomCardNumber + "_error.txt", JsonConvert.SerializeObject(ex.ToString()));
                return false;
            }
        }

        #endregion

        #region leasing
        public void SetExistingRefPolicyModificationAsDeleted(string ReferenceId, CheckoutProviderServicesCodes serviceCode)
        {
            List<PolicyModification> policyModificationLst = _policyModificationrepository.Table
                   .Where(p => p.QuotationReferenceId == ReferenceId
                                   && p.IsLeasing
                                   && p.ProviderServiceId.Value == serviceCode
                                   && !p.IsCheckedkOut
                                   && !p.IsDeleted).ToList();

            policyModificationLst.ForEach(x => x.IsDeleted = true);
            _policyModificationrepository.Update(policyModificationLst);
        }
        #endregion

        #region Autoleasing Export
        public byte[] GenerateRenewalPoliciesDetailsExcel(List<AutoleasingRenewalPoliciesModel> CheckOutDetails)
        {
            //if (CheckOutDetails != null)
            //{
            //    DateTime dt = DateTime.Now;
            //    string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PoliciesDetails" + dt.ToString("dd-MM-yyyy"));
            //    SPREADSHEET_NAME = SPREADSHEET_NAME + ".xlsx";
            //    using (var workbook = SpreadsheetDocument.Create(SPREADSHEET_NAME, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            //    {
            //        var workbookPart = workbook.AddWorkbookPart();
            //        {
            //            var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
            //            var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
            //            sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);
            //            workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();
            //            workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();
            //            DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
            //            string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

            //            uint sheetId = 1;
            //            if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
            //                sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;

            //            DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "PoliciesDetails" };
            //            sheets.Append(sheet);

            //            DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
            //            List<String> columns = new List<string>();

            //            columns.Add("Name");
            //            DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
            //            cell1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
            //            cell1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Name");
            //            headerRow.AppendChild(cell1);

            //            columns.Add("VehicleId");
            //            DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
            //            cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
            //            cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleId");
            //            headerRow.AppendChild(cell2);

            //            columns.Add("Date Of Birth");
            //            DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
            //            cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
            //            cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Date Of Birth");
            //            headerRow.AppendChild(cell3);

            //            columns.Add("Reference");
            //            DocumentFormat.OpenXml.Spreadsheet.Cell cell4 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
            //            cell4.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
            //            cell4.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Reference");
            //            headerRow.AppendChild(cell4);

            //            columns.Add("Policy No");
            //            DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
            //            cell5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
            //            cell5.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Policy No");
            //            headerRow.AppendChild(cell5);

            //            columns.Add("Expiry date");
            //            DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
            //            cell6.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
            //            cell6.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Expiry date");
            //            headerRow.AppendChild(cell6);

            //            columns.Add("Main Driver Id");
            //            DocumentFormat.OpenXml.Spreadsheet.Cell cell7 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
            //            cell7.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
            //            cell7.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Main Driver Id");
            //            headerRow.AppendChild(cell7);

            //            columns.Add("Insurance Company");
            //            DocumentFormat.OpenXml.Spreadsheet.Cell cell8 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
            //            cell8.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
            //            cell8.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Insurance Company");
            //            headerRow.AppendChild(cell8);

            //            sheetData.AppendChild(headerRow);
            //            workbook.WorkbookPart.Workbook.Save();

            //            foreach (var item in CheckOutDetails)
            //            {
            //                DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
            //                foreach (string col in columns)
            //                {
            //                    if (col == "Name")
            //                    {
            //                        DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
            //                        cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
            //                        cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.FullName);
            //                        newRow.AppendChild(cell11);
            //                    }
            //                    if (col == "VehicleId")
            //                    {
            //                        DocumentFormat.OpenXml.Spreadsheet.Cell cell12 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
            //                        cell12.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
            //                        string vehicleIdNum = string.IsNullOrEmpty(item.SequenceNumber) ? item.CustomCardNumber : item.SequenceNumber;
            //                        cell12.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(vehicleIdNum);
            //                        newRow.AppendChild(cell12);
            //                    }
            //                    else if (col == "Date Of Birth")
            //                    {
            //                        DocumentFormat.OpenXml.Spreadsheet.Cell cell33 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
            //                        cell33.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
            //                        cell33.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.DateOfBirthH);
            //                        newRow.AppendChild(cell33);
            //                    }
            //                    else if (col == "Reference")
            //                    {
            //                        DocumentFormat.OpenXml.Spreadsheet.Cell cell34 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
            //                        cell34.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
            //                        cell34.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.ReferenceId);
            //                        newRow.AppendChild(cell34);
            //                    }
            //                    else if (col == "Policy No")
            //                    {
            //                        DocumentFormat.OpenXml.Spreadsheet.Cell cell44 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
            //                        cell44.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
            //                        cell44.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PolicyNo);
            //                        newRow.AppendChild(cell44);
            //                    }
            //                    else if (col == "Expiry date")
            //                    {
            //                        DocumentFormat.OpenXml.Spreadsheet.Cell cell55 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
            //                        cell55.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
            //                        cell55.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PolicyExpiryDate.ToString());
            //                        newRow.AppendChild(cell55);
            //                    }
            //                    else if (col == "Main Driver Id")
            //                    {
            //                        DocumentFormat.OpenXml.Spreadsheet.Cell cell16 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
            //                        cell16.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
            //                        cell16.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.NIN);
            //                        newRow.AppendChild(cell16);
            //                    }
            //                    else if (col == "Insurance Company")
            //                    {
            //                        DocumentFormat.OpenXml.Spreadsheet.Cell cell17 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
            //                        cell17.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
            //                        cell17.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.InsuranceCompanyName);
            //                        newRow.AppendChild(cell17);
            //                    }
            //                }

            //                sheetData.AppendChild(newRow);
            //                workbook.WorkbookPart.Workbook.Save();
            //            }

            //        }

            //        workbook.Close();
            //    }

            //    return GetFileAsByte(SPREADSHEET_NAME);
            //}
            return null;
        }

        private byte[] GetFileAsByte(string fileName)
        {
            if (File.Exists(fileName))
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                var memoryStream = new MemoryStream();
                file.CopyTo(memoryStream);

                file.Dispose();
                file.Close();

                DeleteFile(fileName);

                return memoryStream.ToArray();
            }



            return null;
        }
        private void DeleteFile(string fileName)
        {
            if (File.Exists(@fileName))
            {
                File.Delete(@fileName);
            }
        }
        #endregion
    }
}
