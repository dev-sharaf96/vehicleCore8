using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.WebResources;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Implementation.Policies;
using System.Data.Entity;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Core;
using Tameenk.Services.YakeenIntegration.Business;
using Tameenk.Integration.Core.Providers;
using Tameenk.Core.Infrastructure;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Services.Core.Checkouts;
using Tameenk.Services.Implementation;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Services.YakeenIntegration.Business.YakeenBCareService;
using Tameenk.Services.Notifications;
using Tameenk.Core.Domain.Enums;
using System.Globalization;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Checkout.Components;
using System.IO;
using Tameenk.Core.Configuration;
using Tameenk.Resources;
using Tameenk.Services.Core.Addresses;
using Tameenk.Data;
using System.Data;
using System.Data.Entity.Infrastructure;

namespace Tameenk.Services.Policy.Components
{
    public class PolicyNotificationContext : IPolicyNotificationContext
    {
        private readonly IPolicyService _policyService;
        private readonly IRepository<InsuranceCompany> _insuranceCompanyRepository;
        private readonly IRepository<CheckoutDetail> _checkoutDetailRepository;
        private readonly IVehicleService _vehicleService;
        private readonly IRepository<PolicyModification> _policyModificationRepository;
        private readonly IRepository<CustomCardInfo> _customCardInfoRepository;
        private readonly ICheckoutsService _checkoutsService;
        private readonly IRepository<NajmStatus> _najmStatusRepo;
        private readonly IYakeenClient _yakeenClient;
        private readonly IInsuranceCompanyService _insuranceCompanyService;
        private readonly INotificationServiceProvider _notification;
        private readonly IPolicyFileContext _policyFileContext;
        private readonly IPolicyEmailService _policyEmailService;
        private readonly INotificationService _notificationService;
        private readonly ICheckoutContext _checkoutContext;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<InvoiceFile> _invoiceFileRepository;
        private readonly TameenkConfig _tameenkConfig;
        private readonly IInvoiceService _invoiceService;
        private readonly IAddressService _addressService;
        private readonly IRepository<OwnDamageQueue> _OwnDamageQueueRepository;

        public PolicyNotificationContext(IPolicyService policyService, IRepository<InsuranceCompany> insuranceCompanyRepository
            , IRepository<CheckoutDetail> checkoutDetailrepository
            , IRepository<NajmStatus> najmStatusRepo
              , IYakeenClient yakeenClient
            , IInsuranceCompanyService insuranceCompanyService
            , ICheckoutsService checkoutsService
            , IRepository<CheckoutDetail> checkoutDetailRepository
              , IVehicleService vehicleService
            , IRepository<PolicyModification> policyModificationRepository
            , IRepository<CustomCardInfo> customCardInfoRepository, INotificationServiceProvider notification
            , IPolicyFileContext policyFileContext, IPolicyEmailService policyEmailService,
            INotificationService notificationService
            , ICheckoutContext checkoutContext,
             IRepository<Invoice> invoiceRepository,
             IRepository<InvoiceFile> invoiceFileRepository,
             TameenkConfig tameenkConfig, IInvoiceService invoiceService, IAddressService addressService,
             IRepository<OwnDamageQueue> OwnDamageQueueRepository)
        {
            this._policyService = policyService;
            _insuranceCompanyRepository = insuranceCompanyRepository;
            _checkoutDetailRepository = checkoutDetailrepository;
            _najmStatusRepo = najmStatusRepo;
            _yakeenClient = yakeenClient;
            _insuranceCompanyService = insuranceCompanyService;
            _checkoutsService = checkoutsService;
            _vehicleService = vehicleService;
            _policyModificationRepository = policyModificationRepository;
            _customCardInfoRepository = customCardInfoRepository;
            _notification = notification;
            _policyFileContext = policyFileContext;
            _policyEmailService = policyEmailService;
            _notificationService = notificationService;
            _checkoutContext = checkoutContext;
            _invoiceRepository = invoiceRepository;
            _invoiceFileRepository = invoiceFileRepository;
            _tameenkConfig = tameenkConfig;
            _invoiceService = invoiceService;
            _addressService = addressService;
            _OwnDamageQueueRepository = OwnDamageQueueRepository;
        }

        public CommonResponseModel NotifyPolicyUploadCompletion(PolicyUploadNotificationModel policyUploadNotificationModel)
        {
            PolicyNotificationLog log = new PolicyNotificationLog();
            CommonResponseModel result = new CommonResponseModel();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.CreatedDate = DateTime.Now;
            log.MethodName = "PolicyUploadNotification";
            log.Requester = Utilities.GetUrlReferrer();
            log.ServiceRequest = JsonConvert.SerializeObject(policyUploadNotificationModel);
            string strNajmStatusDescription = string.Empty;
            try
            {
                if (policyUploadNotificationModel == null)
                {
                    result.StatusCode = 2;
                    result.Errors = new List<ErrorModel>();
                    result.Errors.Add(new ErrorModel { Message = "There is no given parameters" });
                    log.StatusCode = 2;
                    log.StatusDescription = "model is null";
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                    return result;
                }
                if (string.IsNullOrEmpty(policyUploadNotificationModel.ReferenceId))
                {
                    result.StatusCode = 2;
                    result.Errors = new List<ErrorModel>();
                    result.Errors.Add(new ErrorModel { Message = "The ReferenceId can not be empty" });
                    log.StatusCode = 3;
                    log.StatusDescription = "referenceId is null";
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                    return result;
                }
                log.ReferenceId = policyUploadNotificationModel.ReferenceId;
                result.ReferenceId = policyUploadNotificationModel.ReferenceId;
                if (string.IsNullOrEmpty(policyUploadNotificationModel.PolicyNo))
                {
                    result.StatusCode = 2;
                    result.Errors = new List<ErrorModel>();
                    result.Errors.Add(new ErrorModel { Message = "The Policy number can not be empty" });
                    log.StatusCode = 4;
                    log.StatusDescription = "policy number is null";
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                    return result;
                }
                log.PolicyNo= policyUploadNotificationModel.PolicyNo;
                log.UploadedDate = policyUploadNotificationModel.UploadedDate;
                log.UploadedReference = policyUploadNotificationModel.UploadedReference;
                if (policyUploadNotificationModel.StatusCode < 1)
                {
                    result.StatusCode = 2;
                    result.Errors = new List<ErrorModel>();
                    result.Errors.Add(new ErrorModel { Message = "The Status code can not be less than 1" });
                    log.StatusCode = 5;
                    log.StatusDescription = "status code can not be less than 1 as we recieved" + policyUploadNotificationModel.StatusCode.ToString();
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                    return result;
                }
                if (policyUploadNotificationModel.StatusCode == 1 && policyUploadNotificationModel.UploadedDate == null)
                {
                    result.StatusCode = 2;
                    result.Errors = new List<ErrorModel>();
                    result.Errors.Add(new ErrorModel { Message = "Upload date is required when status code is 1." });
                    log.StatusCode = 6;
                    log.StatusDescription = "Upload date is null and is required when status is 1";
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                    return result;
                }
               
                var checkoutDetail = _checkoutDetailRepository.TableNoTracking.Where(x => x.ReferenceId == policyUploadNotificationModel.ReferenceId).FirstOrDefault();
                if (checkoutDetail == null)
                {
                    result.StatusCode = 2;
                    result.Errors = new List<ErrorModel>();
                    result.Errors.Add(new ErrorModel { Message = "There is no defined policy with the given ReferenceId and Policy Number" });
                    log.StatusCode = 7;
                    log.StatusDescription = "checkoutDetail is null as there is no checkout with ReferenceId: " + policyUploadNotificationModel.ReferenceId;
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                    return result;
                }
                log.CompanyId = checkoutDetail?.InsuranceCompanyId;
                log.CompanyName = checkoutDetail?.InsuranceCompanyName;
                log.Channel = checkoutDetail.Channel;
                result.ReferenceId = checkoutDetail.ReferenceId;
                var policyInfo = _policyService.GetPolicyWithReferenceIdAndPolicyNumber(policyUploadNotificationModel);
                if (policyInfo == null)
                {
                    result.StatusCode = 2;
                    result.Errors = new List<ErrorModel>();
                    result.Errors.Add(new ErrorModel { Message = "There is no defined policy with the given ReferenceId and Policy Number" });
                    log.StatusCode = 7;
                    log.StatusDescription = "policy is null as there is no policy with number: "+ policyUploadNotificationModel.PolicyNo;
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                    return result;
                }
                if (policyUploadNotificationModel.StatusCode == 1)
                {
                    policyInfo.NajmStatusId = _najmStatusRepo.Table.FirstOrDefault(n => n.Code == "1").Id;
                    strNajmStatusDescription = WebResources.Active;
                }
                else
                {
                    policyInfo.NajmStatusId = _najmStatusRepo.Table.FirstOrDefault(n => n.Code == "Fail").Id;
                    strNajmStatusDescription = WebResources.Failed;
                }
                strNajmStatusDescription += " : " + policyUploadNotificationModel.StatusDescription;
                policyInfo.ModifiedDate = DateTime.Now;
                if(policyInfo.CreatedDate.HasValue)
                {
                    double NajmResponseTimeInSeconds = Math.Round(DateTime.Now.Subtract(policyInfo.CreatedDate.Value).TotalSeconds, 2);
                    policyInfo.NajmResponseTimeInSeconds = NajmResponseTimeInSeconds;
                }
                else if(policyInfo.PolicyIssueDate.HasValue)
                {
                    double NajmResponseTimeInSeconds = Math.Round(DateTime.Now.Subtract(policyInfo.PolicyIssueDate.Value).TotalSeconds, 2);
                    policyInfo.NajmResponseTimeInSeconds = NajmResponseTimeInSeconds;
                }
             
                policyInfo.NajmStatus = strNajmStatusDescription;
                _policyService.NotifyPolicyUploadCompletion(policyUploadNotificationModel);
                _policyService.SavePolicyWithNajmStatus(policyInfo);
                result.StatusCode = 1;

                //send mobile notification
                string exception = string.Empty;
                string notificationMessage = string.Empty;
                var vehicleInfo = _vehicleService.GetVehicleInfoById(checkoutDetail.VehicleId, out exception);
                if (vehicleInfo != null)
                {
                    if (checkoutDetail.SelectedLanguage == Tameenk.Core.Domain.Enums.LanguageTwoLetterIsoCode.En)
                    {
                        notificationMessage = "Your vehicle " + vehicleInfo.MakerEn + " " + vehicleInfo.ModelEn + " uploaded to Najm successfuly";
                    }
                    else
                    {
                        notificationMessage = "تم ربط مركبتك {0} بشركه نجم بنجاح".Replace("{0}", vehicleInfo.MakerAr + " " + vehicleInfo.ModelAr);
                    }
                    var notificationOutput=_notification.SendFireBaseNotification(checkoutDetail.UserId, "بي كير - Bcare", notificationMessage,"UploadNajm", checkoutDetail.ReferenceId, checkoutDetail.Channel);
                }
                // end of mobile notification

                log.StatusCode = 1;
                //log.StatusDescription = "Success";
                PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                return result;
            }
            catch (Exception ex)
            {
                result.StatusCode = 2;
                result.Errors = new List<ErrorModel>();
                result.Errors.Add(new ErrorModel { Message = "Something went wrong please contact customer service !" });
                log.StatusCode = 2;
                log.StatusDescription = ex.ToString();
                PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                return result;
            }
        }
        public UpdateCustomCardOutput UpdateCustomCard(CustomCardQueue policy)
        {
            UpdateCustomCardOutput output = new UpdateCustomCardOutput();
            string exception = string.Empty;
            try
            {
                if (policy == null)
                {
                    output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.NoPolicies;
                    output.ErrorDescription = "policy is null";
                    output.PolicyStatusId = 5;
                    return output;
                }
                if (string.IsNullOrEmpty(policy.CustomCardNumber))
                {
                    output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "customcard is null";
                    output.PolicyStatusId = 5;
                    return output;
                }
                if (policy.ModelYear == 0)
                {
                    output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "modelYear is null";
                    output.PolicyStatusId = 5;
                    return output;
                }
                Guid userId = Guid.Empty;
                Guid.TryParse(policy.UserId, out userId);
                ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
                predefinedLogInfo.UserID = userId;
                predefinedLogInfo.Channel = policy.Channel;
                predefinedLogInfo.ServerIP = Utilities.GetInternalServerIP();
                predefinedLogInfo.CompanyID = policy.CompanyID.HasValue ? policy.CompanyID.Value : 0;
                predefinedLogInfo.CompanyName = policy.CompanyName;
                predefinedLogInfo.Channel = policy.Channel;
                predefinedLogInfo.VehicleId = policy.CustomCardNumber;
                predefinedLogInfo.VehicleModelYear = policy.ModelYear;
                var yakeenOutput = new YakeenVehicleOutput();
                Guid? vehicleId;
                var customConverted = _customCardInfoRepository.TableNoTracking.Where(x => x.CustomCardNumber == policy.CustomCardNumber).FirstOrDefault();
                if (customConverted != null)
                {
                    yakeenOutput.result = new carInfoByCustomTwoResult();
                    yakeenOutput.result.chassisNumber = customConverted.ChassisNumber;
                    yakeenOutput.result.cylinders = customConverted.Cylinders.Value;
                    yakeenOutput.result.logId = customConverted.LogId.Value;
                    yakeenOutput.result.majorColor = customConverted.MajorColor;
                    yakeenOutput.result.modelYear = customConverted.ModelYear.Value;
                    yakeenOutput.result.ownerName = customConverted.CarOwnerName;
                    yakeenOutput.result.plateNumber = customConverted.CarPlateNumber.Value;
                    yakeenOutput.result.plateText1 = customConverted.CarPlateText1;
                    yakeenOutput.result.plateText2 = customConverted.CarPlateText2;
                    yakeenOutput.result.plateText3 = customConverted.CarPlateText3;
                    yakeenOutput.result.plateType = customConverted.PlateTypeCode.Value;
                    yakeenOutput.result.regPlace = customConverted.RegisterationPlace;
                    yakeenOutput.result.sequenceNumber = customConverted.SequenceNumber.Value;
                    yakeenOutput.result.vehicleCapacity = customConverted.VehicleCapacity.Value;
                    yakeenOutput.result.vehicleMakerCode = customConverted.VehicleMakerCode.Value;
                    yakeenOutput.result.vehicleMaker = customConverted.VehicleMaker;
                    yakeenOutput.result.vehicleModel = customConverted.VehicleModel;
                    yakeenOutput.result.vehicleModelCode = customConverted.VehicleModelCode.Value;
                    yakeenOutput.result.vehicleWeight = customConverted.VehicleWeight.Value;
                    vehicleId = customConverted.VehicleId;
                }
                else
                {
                    // Else call yakeen and insert in intermdiate table (CustomCardInfo)
                    CarInfoCustomTwoDto input = new CarInfoCustomTwoDto()
                    {
                        CustomCrdNumber = policy.CustomCardNumber,
                        ModelYear = policy.ModelYear
                    };
                    yakeenOutput = _yakeenClient.CarInfoByCustomTwo(input, predefinedLogInfo);
                    if (yakeenOutput == null)
                    {
                        output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.VehicleYakeenInfoNull;
                        output.ErrorDescription = "yakeen output is null";
                        output.PolicyStatusId = 5;
                        return output;
                    }
                    if (yakeenOutput.ErrorCode != YakeenVehicleOutput.ErrorCodes.Success)
                    {
                        output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.VehicleYakeenInfoNull;
                        output.ErrorDescription = "failed from yakeen due to " + yakeenOutput.ErrorDescription;
                        output.PolicyStatusId = 5;
                        return output;
                    }
                    if (yakeenOutput.result == null)
                    {
                        output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.YakeenResultIsNull;
                        output.ErrorDescription = "yakeenOutput.result is null";
                        output.PolicyStatusId = 5;
                        return output;
                    }
                    if (yakeenOutput.result.sequenceNumber == 0)
                    {
                        output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.CustomCardNotConverted;
                        output.ErrorDescription = "Custom card still not converted";
                        output.PolicyStatusId = 5;
                        return output;
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
                    customCardInfo.ModelYear = policy.ModelYear;
                    customCardInfo.ReferenceId = policy.ReferenceId;
                    customCardInfo.PolicyNo = policy.PolicyNo;
                    customCardInfo.CustomCardNumber = policy.CustomCardNumber;
                    customCardInfo.VehicleId = policy.VehicleId;
                    customCardInfo.CreatedDate = DateTime.Now;
                    _customCardInfoRepository.Insert(customCardInfo);
                }
                var insuranceCompany = _insuranceCompanyRepository.TableNoTracking.Where(x => x.InsuranceCompanyID == policy.CompanyID).FirstOrDefault();
                if (insuranceCompany == null)
                {
                    output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.ProviderIsNull;
                    output.ErrorDescription = "insuranceCompany is null";
                    output.PolicyStatusId = 5;
                    return output;
                }
                IInsuranceProvider provider = GetProvider(insuranceCompany, policy.InsuranceTypeCode.Value);
                if (provider == null)
                {
                    output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.ProviderIsNull;
                    output.ErrorDescription = "provider is null";
                    output.PolicyStatusId = 5;
                    return output;
                }
                UpdateCustomCardRequest serviceRequest = new UpdateCustomCardRequest();
                if (insuranceCompany.InsuranceCompanyID == 18) // alamia only want (VehicleRegPlaceCode --> to be city code)
                {
                    var cities = _addressService.GetAllCities();
                    var info = _addressService.GetCityByName(cities, Utilities.RemoveWhiteSpaces(yakeenOutput.result.regPlace));
                    serviceRequest.VehicleRegPlaceCode = (info != null) ? info.YakeenCode.ToString() : null;
                }
                else
                    serviceRequest.VehicleRegPlaceCode = yakeenOutput.result.regPlace;
                serviceRequest.CustomCardNumber = policy.CustomCardNumber;
                serviceRequest.PolicyNo = policy.PolicyNo;
                serviceRequest.ReferenceId = policy.ReferenceId;
                serviceRequest.SequenceNumber = yakeenOutput.result.sequenceNumber.ToString();
                serviceRequest.VehiclePlateNumber = yakeenOutput.result.plateNumber.ToString();
                serviceRequest.VehiclePlateText1 = yakeenOutput.result.plateText1;
                serviceRequest.VehiclePlateText2 = yakeenOutput.result.plateText2;
                serviceRequest.VehiclePlateText3 = yakeenOutput.result.plateText3;
                serviceRequest.VehiclePlateTypeCode = yakeenOutput.result.plateType.ToString();
                serviceRequest.VehicleRegPlace = yakeenOutput.result.regPlace;
                output.ServiceRequest = JsonConvert.SerializeObject(serviceRequest);

                CustomCardServiceOutput results = null;
                bool isAutoleasingPolicy = false;
                if (!policy.PolicyStatusId.HasValue || policy.PolicyStatusId == 0 || policy.PolicyStatusId == 5)
                {
                    if (predefinedLogInfo.Channel == Channel.autoleasing.ToString())
                    {
                        isAutoleasingPolicy = true;
                        results = provider.AutoleaseUpdateCustomCard(serviceRequest, predefinedLogInfo);
                    }
                    else
                    {
                        isAutoleasingPolicy = false;
                        results = provider.UpdateCustomCard(serviceRequest, predefinedLogInfo);
                    }
                    if (results == null)
                    {
                        output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.ServiceDown;
                        output.ErrorDescription = "Service Return Null ";
                        output.PolicyStatusId = 5;
                        return output;
                    }
                    output.ServiceResponse = results.ServiceResponse;
                    if (results.ErrorCode != CustomCardServiceOutput.ErrorCodes.Success)
                    {
                        output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.ServiceDown;
                        output.ErrorDescription = "service errors: " + results.ErrorDescription;
                        output.PolicyStatusId = 5;
                        return output;
                    }
                }
                else
                {
                    results = new CustomCardServiceOutput();
                    results.UpdateCustomCardResponse = new UpdateCustomCardResponse();
                    results.UpdateCustomCardResponse = JsonConvert.DeserializeObject<UpdateCustomCardResponse>(policy.ServiceResponse);
                }
                string PolicyFileUrl = results?.UpdateCustomCardResponse?.PolicyFileUrl;
                Byte[] PolicyFile = results.UpdateCustomCardResponse.PolicyFile;

                exception = string.Empty;
                bool updateVehicleInfo = UpdateVehicleWithYakeenData(yakeenOutput.result, policy.VehicleId.ToString(), out exception);
                if (!updateVehicleInfo || !string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.FailedToUpdateDBCustomCard;
                    output.ErrorDescription = "failed to update custom card with info due to " + exception;
                    output.PolicyStatusId = 6;
                    return output;
                }
                var policyInfo = _policyModificationRepository.TableNoTracking.Where(a => a.ReferenceId == policy.ReferenceId&&a.CustomCard==policy.CustomCardNumber).FirstOrDefault();
                if (policyInfo == null)
                {
                    PolicyModification policyModification = new PolicyModification();
                    policyModification.ReferenceId = policy.ReferenceId;
                    policyModification.CreatedDate = DateTime.Now;
                    policyModification.MethodName = "UpdateCustomCard";
                    policyModification.ServerIP = Utilities.GetInternalServerIP();
                    policyModification.Channel = policy.Channel;
                    policyModification.CustomCard = policy.CustomCardNumber;
                    policyModification.PolicyNo = policy.PolicyNo;
                    policyModification.InsuranceCompanyId = policy.CompanyID;
                    policyModification.InsuranceTypeCode = policy.InsuranceTypeCode;
                    policyModification.QuotationReferenceId = policy.ReferenceId;
                    policyModification.ConvertedVehicleId = policy.VehicleId;
                    policyModification.VehicleId = policy.VehicleId;
                    _policyModificationRepository.Insert(policyModification);
                }
                var checkoutDetail = _checkoutDetailRepository.TableNoTracking.Where(x => x.ReferenceId == policy.ReferenceId).FirstOrDefault();
                if (checkoutDetail == null)
                {
                    output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.CheckoutDetailIsNull;
                    output.ErrorDescription = "fcheckoutDetail is null";
                    output.PolicyStatusId = 6;
                    return output;
                }
                var policyData = _policyService.GetPolicyByReferenceId(policy.ReferenceId);
                if (policyData == null)
                {
                    output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.PolicyDataIsNull;
                    output.ErrorDescription = "policyData is null";
                    output.PolicyStatusId = 6;
                    return output;
                }
                PolicyResponse policyResponse = new PolicyResponse();
                policyResponse.ReferenceId = policy.ReferenceId;
                policyResponse.PolicyFileUrl = PolicyFileUrl;
                policyResponse.PolicyEffectiveDate = policyData.PolicyEffectiveDate;
                policyResponse.PolicyExpiryDate = policyData.PolicyExpiryDate;
                policyResponse.PolicyIssuanceDate = policyData.PolicyIssueDate;
                policyResponse.PolicyNo = policy.PolicyNo;
                policyResponse.PolicyFile = PolicyFile;
                var selectedLang = LanguageTwoLetterIsoCode.Ar;
                if (checkoutDetail.SelectedLanguage == LanguageTwoLetterIsoCode.En)
                    selectedLang = LanguageTwoLetterIsoCode.En;

                bool generateTemplateFromOurSide = true;
                if (policy.CompanyID == 11 && policy.InsuranceTypeCode == 1)
                    generateTemplateFromOurSide = false;

                var policyFileStatus = _policyFileContext.GeneratePolicyPdfFile(policyResponse, checkoutDetail.InsuranceCompanyId.Value, checkoutDetail.Channel, selectedLang, generateTemplateFromOurSide);
                if (policyFileStatus.ErrorCode != PolicyGenerationOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.FailedToGeneratePdfFile;
                    output.ErrorDescription = "Failed To Generate Pdf File";
                    output.PolicyStatusId = 6;
                    return output;
                }
                string filePath = Utilities.SaveCompanyFile(policy.ReferenceId, policyFileStatus.File, insuranceCompany.Key, true, isAutoleasingPolicy);
                if (string.IsNullOrEmpty(filePath))
                {
                    output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.FailedToGeneratePdfFile;
                    output.ErrorDescription = "Failed To Generate Pdf File and file path empty";
                    output.PolicyStatusId = 6;
                    return output;
                }
                exception = string.Empty;
                _policyService.UpdatePolicyFile(policyData.PolicyFileId.Value, filePath, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.FailedToGeneratePdfFile;
                    output.ErrorDescription = "Failed To update policy file due to " + exception;
                    output.PolicyStatusId = 7;
                    return output;
                }
                SendPolicyViaMailDto sendPolicyViaMailDto = new SendPolicyViaMailDto()
                {
                    Channel = checkoutDetail.Channel,
                    Module = checkoutDetail.Channel.ToLower() == Channel.autoleasing.ToString().ToLower() ? Module.Autoleasing.ToString() : Module.Vehicle.ToString(),
                    Method = "PolicyFile",
                    PolicyResponseMessage = policyResponse,
                    ReceiverEmailAddress = checkoutDetail.Email,
                    ReferenceId = policy.ReferenceId,
                    UserLanguage = selectedLang,
                    PolicyFileByteArray = policyFileStatus.File,
                    IsPolicyGenerated = true,
                    IsShowErrors = false,
                    TawuniyaFileUrl = insuranceCompany.Key == "Tawuniya" ? policyResponse.PolicyFileUrl : "",
                    InsuranceTypeCode = checkoutDetail.SelectedInsuranceTypeCode
                };
                if (checkoutDetail.IsEmailVerified.HasValue && checkoutDetail.IsEmailVerified.Value)
                {
                    var emailOutput = _policyEmailService.SendPolicyByMail(sendPolicyViaMailDto, insuranceCompany.Key);
                    if (emailOutput.ErrorCode!=EmailOutput.ErrorCodes.Success)
                    {
                        output.ErrorDescription = "Partial Success Failed to send email to client due to "+emailOutput.ErrorDescription;
                    }
                    else
                    {
                        output.ErrorDescription = "Success";
                    }
                }
                else
                {
                    output.ErrorDescription = "Success Email not Verified";
                }
                string smsBody = string.Empty;
                string whatsAppBody = string.Empty;
                string policyFileUrl = "https://bcare.com.sa/Identityapi/api/u/p?r=" + policyResponse.ReferenceId;
                if (insuranceCompany.Key == "Tawuniya")
                    policyFileUrl = policyResponse.PolicyFileUrl;
                string shortUrl = Utilities.GetShortUrl(policyFileUrl);
                if (!string.IsNullOrEmpty(shortUrl))
                    policyFileUrl = shortUrl;
                //string emo = DecodeEncodedNonAsciiCharacters("\uD83E\uDD73");
                string emo_heart = DecodeEncodedNonAsciiCharacters("\uD83D\uDC99");
                string emo_flower = DecodeEncodedNonAsciiCharacters("\uD83C\uDF39");

                string platinfo = string.Empty;
                if (!string.IsNullOrEmpty(yakeenOutput.result.plateText1)
                    || !string.IsNullOrEmpty(yakeenOutput.result.plateText2)
                    || !string.IsNullOrEmpty(yakeenOutput.result.plateText3))
                {
                    platinfo = _checkoutContext.GetCarPlateInfo(yakeenOutput.result.plateText1,
                    yakeenOutput.result.plateText2, yakeenOutput.result.plateText3,
                    yakeenOutput.result.plateNumber, checkoutDetail.SelectedLanguage == LanguageTwoLetterIsoCode.En?"en" :"ar");
                }
                string make = string.Empty;
                string model = string.Empty;
                var vehicleMakers = _vehicleService.VehicleMakers();
                var vehicleModels = _vehicleService.VehicleModels(yakeenOutput.result.vehicleMakerCode);

                if (checkoutDetail.SelectedLanguage == LanguageTwoLetterIsoCode.Ar)
                {
                    smsBody = "تم تحديث وثيقة التأمين بناء على اصدار الاستمارة لمركبة [%Make%] [%Model%] ([%PLATE%])";
                    smsBody += " " + policyFileUrl;
                    whatsAppBody = "تم تحديث وثيقة التأمين بناء على اصدار الاستمارة لمركبة [%Make%] [%Model%] ([%PLATE%]) ";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "بي كير تتمني لك قيادة آمنة " + emo_heart;
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "لست بحاجة للرد علي هذه الرسالة،";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "الخدمة تلقائية ولأول مرة في المملكة" + emo_flower;
                  
                    if(vehicleMakers!=null)
                    {
                        var makerInfo = vehicleMakers.Where(a => a.Code == yakeenOutput.result.vehicleMakerCode).FirstOrDefault();
                        if(makerInfo!=null)
                        {
                            make = makerInfo.ArabicDescription;
                        }
                    }
                    if(vehicleModels!=null)
                    {
                        var modelInfo = vehicleModels.Where(a => a.VehicleMakerCode == yakeenOutput.result.vehicleMakerCode&&a.Code== yakeenOutput.result.vehicleModelCode).FirstOrDefault();
                        if (modelInfo != null)
                        {
                            model = modelInfo.ArabicDescription;
                        }
                    }
                }
                else
                {
                    smsBody = "Your vehicle [%Make%] [%Model%] ([%PLATE%])'s policy details has been updated with Sequence Number ";
                    smsBody += policyFileUrl;
                    whatsAppBody = "Your vehicle [%Make%] [%Model%] ([%PLATE%])'s policy details has been updated with Sequence Number ";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "Bcare wishes you a safe drive " + emo_heart;
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "You do not need to reply to this message،";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "For automatic service for the first time in the Kingdom" + emo_flower;
                    if (vehicleMakers != null)
                    {
                        var makerInfo = vehicleMakers.Where(a => a.Code == yakeenOutput.result.vehicleMakerCode).FirstOrDefault();
                        if (makerInfo != null)
                        {
                            make = makerInfo.EnglishDescription;
                        }
                    }
                    if (vehicleModels != null)
                    {
                        var modelInfo = vehicleModels.Where(a => a.VehicleMakerCode == yakeenOutput.result.vehicleMakerCode && a.Code == yakeenOutput.result.vehicleModelCode).FirstOrDefault();
                        if (modelInfo != null)
                        {
                            model = modelInfo.ArabicDescription;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(platinfo))
                {
                    smsBody = smsBody.Replace("[%PLATE%]", platinfo);
                    whatsAppBody = whatsAppBody.Replace("[%PLATE%]", platinfo);
                }
                else
                {
                    smsBody = smsBody.Replace("([%PLATE%])", string.Empty);
                    whatsAppBody = whatsAppBody.Replace("([%PLATE%])", string.Empty);
                }
                if (!string.IsNullOrEmpty(make))
                {
                    smsBody = smsBody.Replace("[%Make%]", make);
                    whatsAppBody = whatsAppBody.Replace("[%Make%]", make);
                }
                else
                {
                    smsBody = smsBody.Replace("[%Make%]", string.Empty);
                    whatsAppBody = whatsAppBody.Replace("[%Make%]", string.Empty);
                }
                if (!string.IsNullOrEmpty(model))
                {
                    smsBody = smsBody.Replace("[%Model%]", model);
                    whatsAppBody = whatsAppBody.Replace("[%Model%]", model);
                }
                else
                {
                    smsBody = smsBody.Replace("[%Model%]", string.Empty);
                    whatsAppBody = whatsAppBody.Replace("[%Model%]", string.Empty);
                }

                var smsModel = new SMSModel()
                {
                    PhoneNumber = checkoutDetail.Phone,
                    MessageBody = smsBody,
                    Method = SMSMethod.UpdateCustomCardPolicyFile.ToString(),
                    Module = Module.Vehicle.ToString(),
                    Channel = checkoutDetail.Channel,
                    ReferenceId = checkoutDetail.ReferenceId
                };
                _notificationService.SendSmsBySMSProviderSettings(smsModel);
                _notification.SendFireBaseNotification(checkoutDetail.UserId, "بي كير - Bcare", "وثيقتك جاهزة 😍 الرجاء الذهاب إلى الملف الشخصي \"وثائق التأمين\" لتحميل الوثيقة", "UpdateCustomCard", checkoutDetail.ReferenceId, checkoutDetail.Channel);
                _notificationService.SendWhatsAppMessageUpdateCustomCardAsync(checkoutDetail.Phone, whatsAppBody, SMSMethod.UpdateCustomCardPolicyFile.ToString(), checkoutDetail.ReferenceId, Enum.GetName(typeof(LanguageTwoLetterIsoCode), checkoutDetail.SelectedLanguage).ToLower(),make,model,platinfo);

                output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.Success;
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                output.PolicyStatusId = 5;
                return output;
            }
        }
        private IInsuranceProvider GetProvider(InsuranceCompany insuranceCompany, short InsuranceTypeCode)
        {
            var providerFullTypeName = string.Empty;
            providerFullTypeName = insuranceCompany.ClassTypeName + ", " + insuranceCompany.NamespaceTypeName;
            IInsuranceProvider provider = null;
            object instance = Utilities.GetValueFromCache("instance_" + providerFullTypeName + InsuranceTypeCode);
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
                    Utilities.AddValueToCache("instance_" + providerFullTypeName + InsuranceTypeCode, instance, 1440);

                scope.Dispose();
            }
            return provider;
        }

        public UpdateCustomCardOutput GetCustomCardInfo(CustomCardQueue customCard)
        {
            UpdateCustomCardOutput output = new UpdateCustomCardOutput();
            string exception = string.Empty;
            try
            {
                if (customCard == null)
                {
                    output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.NoPolicies;
                    output.ErrorDescription = "policy is null";
                    return output;
                }
                if (string.IsNullOrEmpty(customCard.CustomCardNumber))
                {
                    output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "customcard is null";
                    return output;
                }
                if (customCard.ModelYear == 0)
                {
                    output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "modelYear is null";
                    return output;
                }
                var customConverted = _customCardInfoRepository.TableNoTracking.Where(x => x.CustomCardNumber == customCard.CustomCardNumber).FirstOrDefault();
                if (customConverted != null)
                {
                    output.CarPlateText1 = customConverted.CarPlateText1;
                    output.CarPlateText2 = customConverted.CarPlateText2;
                    output.CarPlateText3 = customConverted.CarPlateText3;
                    output.CarPlateNumber = customConverted.CarPlateNumber;
                    if (customConverted.SequenceNumber.HasValue)
                        output.SequenceNumber = customConverted.SequenceNumber.ToString();
                    output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    return output;
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
                    output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.VehicleYakeenInfoNull;
                    output.ErrorDescription = "yakeen output is null";
                    return output;
                }
                if (yakeenOutput.ErrorCode != YakeenVehicleOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.VehicleYakeenInfoNull;
                    output.ErrorDescription = "failed from yakeen due to " + yakeenOutput.ErrorDescription;
                    return output;
                }
                if (yakeenOutput.result == null)
                {
                    output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.YakeenResultIsNull;
                    output.ErrorDescription = "yakeenOutput.result is null";
                    return output;
                }
                if (yakeenOutput.result.sequenceNumber == 0)
                {
                    output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.CustomCardNotConverted;
                    output.ErrorDescription = "Custom card still not converted";
                    return output;
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
                output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";

                output.CarPlateText1 = customCardInfo.CarPlateText1;
                output.CarPlateText2 = customCardInfo.CarPlateText2;
                output.CarPlateText3 = customCardInfo.CarPlateText3;
                output.CarPlateNumber = customCardInfo.CarPlateNumber;
                if (customCardInfo.SequenceNumber.HasValue)
                    output.SequenceNumber = customCardInfo.SequenceNumber.ToString();
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = UpdateCustomCardOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                return output;
            }
        }
        private bool UpdateVehicleWithYakeenData(carInfoByCustomTwoResult yakeenData, string vehicleId,out string exception)
        {
            try
            {
                exception = string.Empty;
                var vehicle = _vehicleService.GetVehicle(vehicleId);
                if (vehicle != null && string.IsNullOrEmpty(vehicle.SequenceNumber))
                {
                    if (string.IsNullOrEmpty(vehicle.CarOwnerName))
                    {
                        vehicle.CarOwnerName = yakeenData.ownerName;
                    }
                    vehicle.SequenceNumber = yakeenData.sequenceNumber.ToString();
                    vehicle.CarPlateNumber = yakeenData.plateNumber;
                    vehicle.CarPlateText1 = yakeenData.plateText1;
                    vehicle.CarPlateText2 = yakeenData.plateText2;
                    vehicle.CarPlateText3 = yakeenData.plateText3;
                    if (string.IsNullOrEmpty(vehicle.ChassisNumber))
                    {
                        vehicle.ChassisNumber = yakeenData.chassisNumber;
                    }
                    if (string.IsNullOrEmpty(vehicle.MajorColor))
                    {
                        vehicle.MajorColor = yakeenData.majorColor;
                    }
                    if (!vehicle.Cylinders.HasValue)
                    {
                        vehicle.Cylinders = Convert.ToByte(yakeenData.cylinders);
                    }
                    if (!vehicle.PlateTypeCode.HasValue)
                    {
                        vehicle.PlateTypeCode = Convert.ToByte(yakeenData.plateType);
                    }
                    if (vehicle.VehicleWeight!=0)
                    {
                        vehicle.VehicleWeight = yakeenData.vehicleWeight;
                    }
                    if (!vehicle.VehicleModelCode.HasValue)
                    {
                        vehicle.VehicleModelCode = yakeenData.vehicleModelCode;
                    }
                    if (string.IsNullOrEmpty(vehicle.VehicleModel))
                    {
                        vehicle.VehicleModel = yakeenData.vehicleModel;
                    }
                    if (!vehicle.VehicleMakerCode.HasValue)
                    {
                        vehicle.VehicleMakerCode = Convert.ToByte(yakeenData.vehicleMakerCode);
                    }
                    if (string.IsNullOrEmpty(vehicle.VehicleMaker))
                    {
                        vehicle.VehicleMaker = yakeenData.vehicleMaker;
                    }
                    if (string.IsNullOrEmpty(vehicle.RegisterationPlace))
                    {
                        vehicle.RegisterationPlace = yakeenData.regPlace;
                    }
                    vehicle.ModelYear = yakeenData.modelYear;
                    _vehicleService.UpdateVehicle(vehicle);
                }
                return true;
            }
            catch(Exception exp)
            {
                exception = exp.ToString();
                return false;
            }
        }
        string DecodeEncodedNonAsciiCharacters(string value)
        {
            return System.Text.RegularExpressions.Regex.Replace(
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m => {
                    return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
                });
        }
        #region Cancel Policy Notifications
        public PolicyCancellatioNotificationOutPut CancelPolicyNotification(CancelPolicyNotificationRequest request)
        {
            PolicyCancellatioNotificationOutPut output = new PolicyCancellatioNotificationOutPut();
            PolicyNotificationLog log = new PolicyNotificationLog();
            DateTime dtBeforeCalling = DateTime.Now;
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.MethodName = "CancelPolicyNotification";
            log.ServiceRequest = JsonConvert.SerializeObject(request);
            log.ReferenceId = request.ReferenceId;
            log.PolicyNo = request.PolicyNo;
            output.ReferenceId = request.ReferenceId;
            try
            {
                if (request == null)
                {
                    output.StatusCode = 2;
                    output.Errors = new List<ErrorModel>();
                    output.Errors.Add(new ErrorModel { Message = "Request is Null ", Code = ((int)CancellPolicyOutput.ErrorCodes.Failure).ToString(), Field = "Null Request" });
                    log.StatusCode = output.StatusCode;
                    log.StatusDescription = "Request is Null ";
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(request.ReferenceId))
                {
                    output.StatusCode = 2;
                    output.Errors = new List<ErrorModel>();
                    output.Errors.Add(new ErrorModel { Message = "Reference is Null ", Code = ((int)CancellPolicyOutput.ErrorCodes.Failure).ToString(), Field = "Null Reference" });
                    log.StatusCode = output.StatusCode;
                    log.StatusDescription = "Reference is Null ";
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                    return output;
                }

                var policyInfo = _policyService.GetPolicyByReferenceId(request.ReferenceId);
                if (policyInfo == null)
                {
                    output.StatusCode = 2;
                    output.Errors = new List<ErrorModel>();
                    output.Errors.Add(new ErrorModel { Message = "No policy data with this reference: " + request.ReferenceId, Code = ((int)CancellPolicyOutput.ErrorCodes.Failure).ToString(), Field = "Null Reference" });
                    log.StatusCode = output.StatusCode;
                    log.StatusDescription = "No policy data with this reference: " + request.ReferenceId;
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                    return output;
                }

                var companyinfo = _insuranceCompanyRepository.TableNoTracking.FirstOrDefault(i => i.InsuranceCompanyID == policyInfo.InsuranceCompanyID);
                if (companyinfo == null)
                {
                    output.StatusCode = 2;
                    output.Errors = new List<ErrorModel>();
                    output.Errors.Add(new ErrorModel { Message = "No company with this id: " + policyInfo.InsuranceCompanyID, Code = ((int)CancellPolicyOutput.ErrorCodes.Failure).ToString(), Field = "Null Company data" });
                    log.StatusCode = output.StatusCode;
                    log.StatusDescription = "No company with this id: " + policyInfo.InsuranceCompanyID;
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                    return output;
                }
                log.CompanyId = companyinfo.InsuranceCompanyID;
                log.CompanyName = companyinfo.Key;
                CancellationRequest cancellationRequest = new CancellationRequest();
                cancellationRequest.ReferenceId = request.ReferenceId;
                cancellationRequest.PolicyNo = request.PolicyNo;
                cancellationRequest.CancelDate = Convert.ToDateTime(request.CancellationDate);
                cancellationRequest.UserName = companyinfo.Key;
                cancellationRequest.IsAutolease = false;
               // _policyService.InsertCancellationRequest(cancellationRequest);
                var result = _policyService.CancelPolicy(request.ReferenceId, true, companyinfo.Key);
                if (!result)
                {
                    output.StatusCode = 3;
                    output.Errors = new List<ErrorModel>();
                    output.Errors.Add(new ErrorModel { Message = "Faild to cancel policy ", Code = ((int)CancellPolicyOutput.ErrorCodes.Failure).ToString(), Field = "Faild to cancel policy " });
                    log.StatusCode = output.StatusCode;
                    log.StatusDescription = "Faild to cancel policy";
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                    return output;
                }

                output.StatusCode = 1;
                log.StatusCode = output.StatusCode;
                log.StatusDescription = "Successs";
                PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.Message, ex, false);
                output.StatusCode = 2;
                output.Errors = new List<ErrorModel>();
                output.Errors.Add(new ErrorModel { Message = "Services Exception", Code = ((int)CancellPolicyOutput.ErrorCodes.ServiceException).ToString(), Field = "Services Exception" });
                log.StatusCode = output.StatusCode;
                log.StatusDescription = ex.ToString();
                PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                return output;
            }
        }
        #endregion

        public CommonResponseModel GetEInvoice(InvoiceNotificationModel invoiceNotificationModel)        {            PolicyNotificationLog log = new PolicyNotificationLog();            CommonResponseModel result = new CommonResponseModel();            log.UserIP = Utilities.GetUserIPAddress();            log.ServerIP = Utilities.GetInternalServerIP();            log.UserAgent = Utilities.GetUserAgent();            log.CreatedDate = DateTime.Now;            log.MethodName = "InvoiceNotification";            log.Requester = Utilities.GetUrlReferrer();            log.ReferenceId = invoiceNotificationModel.ReferenceId;
            log.PolicyNo = invoiceNotificationModel.PolicyNo;            log.ServiceRequest = JsonConvert.SerializeObject(invoiceNotificationModel);            try            {                if (invoiceNotificationModel == null)                {                    result.StatusCode = 2;                    result.Errors = new List<ErrorModel>();                    result.Errors.Add(new ErrorModel { Message = "There is no given parameters" });                    log.StatusCode = 2;                    log.StatusDescription = "model is null";                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);                    return result;                }                if (string.IsNullOrEmpty(invoiceNotificationModel.ReferenceId))                {                    result.StatusCode = 2;                    result.Errors = new List<ErrorModel>();                    result.Errors.Add(new ErrorModel { Message = "The ReferenceId can not be empty" });                    log.StatusCode = 3;                    log.StatusDescription = "referenceId is null";                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);                    return result;                }                log.ReferenceId = invoiceNotificationModel.ReferenceId;                result.ReferenceId = invoiceNotificationModel.ReferenceId;                if (string.IsNullOrEmpty(invoiceNotificationModel.PolicyNo))                {                    result.StatusCode = 2;                    result.Errors = new List<ErrorModel>();                    result.Errors.Add(new ErrorModel { Message = "The Policy number can not be empty" });                    log.StatusCode = 4;                    log.StatusDescription = "policy number is null";                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);                    return result;                }                if (string.IsNullOrEmpty(invoiceNotificationModel.InvoiceNumber))                {                    result.StatusCode = 2;                    result.Errors = new List<ErrorModel>();                    result.Errors.Add(new ErrorModel { Message = "The InvoiceNumber number can not be empty" });                    log.StatusCode = 4;                    log.StatusDescription = "InvoiceNumber number is null";
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);                    return result;                }                if (invoiceNotificationModel.InvoiceFile == null && string.IsNullOrEmpty(invoiceNotificationModel.InvoiceFileUrl))                {                    result.StatusCode = 2;                    result.Errors = new List<ErrorModel>();                    result.Errors.Add(new ErrorModel { Message = "The InvoiceFile and InvoiceFileUrl are empty" });                    log.StatusCode = 4;                    log.StatusDescription = "The InvoiceFile and InvoiceFileUrl are empty";
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);                    return result;                }
                //get invoice by reference
                var invoice = _invoiceRepository.Table.Where(x => x.ReferenceId == invoiceNotificationModel.ReferenceId).FirstOrDefault();                if (invoice == null)                {
                    result.StatusCode = 2;
                    result.Errors = new List<ErrorModel>();
                    result.Errors.Add(new ErrorModel { Message = "can not find the this policy check the reference number" });
                    log.StatusCode = 4;
                    log.StatusDescription = "can not find the this policy check the reference number ";
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                    return result;                }                var invoiceFileInfo = _invoiceFileRepository.Table.Where(x => x.Id == invoice.Id).FirstOrDefault();                string companyKey = null;                string exception = string.Empty;

                //if (invoiceFileInfo == null)                //{
                //    if (_invoiceService.GenerateInvoicePdf(invoiceNotificationModel.ReferenceId, out exception))
                //    {
                //        invoiceFileInfo = _invoiceFileRepository.Table.Where(x => x.Id == invoice.Id).FirstOrDefault();
                //    }                //}
                if (invoiceFileInfo == null)
                {
                    var isInvoiceFileGenerated = _invoiceService.GenerateInvoicePdf(invoiceNotificationModel.ReferenceId, out exception);
                    if (!string.IsNullOrEmpty(exception) || !isInvoiceFileGenerated)
                    {
                        result.StatusCode = 2;
                        result.Errors = new List<ErrorModel>();
                        result.Errors.Add(new ErrorModel { Message = "this is invoice number is not exist" });
                        log.StatusCode = 4;
                        log.StatusDescription = !string.IsNullOrEmpty(exception) ? exception : "_invoiceService.GenerateInvoicePdf return false";
                        PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                        return result;
                    }

                    invoiceFileInfo = _invoiceFileRepository.Table.Where(x => x.Id == invoice.Id).FirstOrDefault();
                }                exception = string.Empty;                if (invoiceFileInfo == null)                {                    result.StatusCode = 2;                    result.Errors = new List<ErrorModel>();                    result.Errors.Add(new ErrorModel { Message = "this is invoice number is not exist" });                    log.StatusCode = 4;                    log.StatusDescription = "this is invoice number is not exist";                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);                    return result;                }                var checkoutDetail = _checkoutDetailRepository.TableNoTracking.Where(x => x.ReferenceId == invoice.ReferenceId).FirstOrDefault();                if (checkoutDetail == null || string.IsNullOrEmpty(checkoutDetail.Email)) // please advice this case 
                {                    result.StatusCode = 2;                    result.Errors = new List<ErrorModel>();                    result.Errors.Add(new ErrorModel { Message = " can not find the client email to send the invoice " });                    log.StatusCode = 5;                    log.StatusDescription = " can not find the client email ";                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);                    return result;                }
                               companyKey = checkoutDetail.InsuranceCompanyName;

                //68806faea9f04cf_ACIG_101303.pdf
                string generatedReportFileName = checkoutDetail.ReferenceId + "_" + checkoutDetail.InsuranceCompanyName + "_Company_" + invoiceFileInfo.Id + ".pdf";
                string generatedReportDirPath = Path.GetDirectoryName(invoiceFileInfo.FilePath) ;
                byte[] invoiceFile=null;
                if (!string.IsNullOrEmpty(invoiceNotificationModel.InvoiceFile))                {
                    invoiceFile = Convert.FromBase64String(invoiceNotificationModel.InvoiceFile);                }                else if (!string.IsNullOrEmpty(invoiceNotificationModel.InvoiceFileUrl))                {                    string fileURL = invoiceNotificationModel.InvoiceFileUrl;                    fileURL = fileURL.Replace(@"\\", @"//");                    fileURL = fileURL.Replace(@"\", @"/");                    using (System.Net.WebClient client = new System.Net.WebClient())                    {                        invoiceFile = client.DownloadData(fileURL);                    }                }
                if (invoiceFile == null)
                {
                    result.StatusCode = 2;
                    result.Errors = new List<ErrorModel>();
                    result.Errors.Add(new ErrorModel { Message = " Failed to Handle this file Please try to send it again " });
                    log.StatusCode = 5;
                    log.StatusDescription = "Failed to Handle this file ";
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                    return result;
                }
                string filePath  = Utilities.SaveCompanyInvoicePdfFile(invoiceNotificationModel.ReferenceId, invoiceFile, generatedReportFileName, generatedReportDirPath, _tameenkConfig.RemoteServerInfo.UseNetworkDownload, _tameenkConfig.RemoteServerInfo.DomainName, _tameenkConfig.RemoteServerInfo.ServerIP, _tameenkConfig.RemoteServerInfo.ServerUserName, _tameenkConfig.RemoteServerInfo.ServerPassword, out exception);
                if (string.IsNullOrEmpty(filePath))
                {
                    result.StatusCode = 2;
                    result.Errors = new List<ErrorModel>();
                    result.Errors.Add(new ErrorModel { Message = " Failed to save invoice pdf file Please try to send it again " });
                    exception = "Failed to save pdf file on server due to " + exception;
                    log.StatusCode = 5;
                    log.StatusDescription = exception;
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                    return result;
                }
                invoiceFileInfo.CompanyInvoieFilePath = filePath;
                _invoiceFileRepository.Update(invoiceFileInfo);

                invoice.TaxInvoiceNumber = invoiceNotificationModel.InvoiceNumber;                _invoiceRepository.Update(invoice);

                if (!checkoutDetail.IsEmailVerified.HasValue || !checkoutDetail.IsEmailVerified.Value)
                {                    result.StatusCode = 1;                    result.Errors = new List<ErrorModel>();                    result.Errors.Add(new ErrorModel { Message = "Success" });                    log.StatusCode = 5;                    log.StatusDescription = "User Email is not verified, E-Invoice could not be sent.";                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);                    return result;                }                string emailSubject = InvoiceResources.ResourceManager.GetString("EmailSubject", CultureInfo.GetCultureInfo("en"));                StringBuilder emailBody = new StringBuilder();                emailBody.Append(InvoiceResources.ResourceManager.GetString("EmailBody", CultureInfo.GetCultureInfo("ar")));                emailBody.Append(InvoiceResources.ResourceManager.GetString("EmailBody", CultureInfo.GetCultureInfo("en")));

                List<string> to = new List<string>();
                to.Add(checkoutDetail.Email);
                EmailModel model = new EmailModel();
                model.Method = "InvoiceFile";
                model.Module = Module.Vehicle.ToString(); ;
                model.Channel = checkoutDetail.Channel;
                model.To = to;
                model.Subject = emailSubject;
                model.EmailBody = emailBody.ToString();
                model.ReferenceId = checkoutDetail.ReferenceId;
                List<System.Net.Mail.Attachment> MailAttachments = new List<System.Net.Mail.Attachment>();
                MemoryStream strm = new MemoryStream(invoiceFile);
                var data = new System.Net.Mail.Attachment(strm, "E-Invoice");
                MailAttachments.Add(data);
                model.Attachments = MailAttachments;
                var emailOutput= _notificationService.SendEmail(model);
                if (emailOutput.ErrorCode!=EmailOutput.ErrorCodes.Success)                {                    result.StatusCode = 2;                    result.Errors = new List<ErrorModel>();                    result.Errors.Add(new ErrorModel { Message = "Failed to send invoice pdf file to the client email Please try to send it again"});                    log.StatusCode = 5;                    log.StatusDescription = "Failed to send invoice pdf file to the client email due to " + emailOutput.ErrorDescription;                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);                    return result;                }                result.StatusCode = 1;                result.ReferenceId = invoiceNotificationModel.ReferenceId;                log.StatusCode = 1;                log.StatusDescription = "Success";                PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);                return result;            }            catch (Exception ex)            {                result.StatusCode = 2;                result.Errors = new List<ErrorModel>();                result.Errors.Add(new ErrorModel { Message = "Something went wrong please contact customer service !" });                log.StatusCode = 2;                log.StatusDescription = ex.ToString();                PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);                return result;            }        }

        #region OwnDamage

        public List<OwnDamageQueue> GetFromOwnDamageQueue(out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetOwnDamageQueue";
                command.CommandType = CommandType.StoredProcedure;
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var result = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<OwnDamageQueue>(reader).ToList();
                idbContext.DatabaseInstance.Connection.Close();
                return result;
            }
            catch (Exception ex)
            {
                idbContext.DatabaseInstance.Connection.Close();
                exception = ex.ToString();
                return null;
            }
        }

        public bool GetAndUpdateOwnDamageQueue(OwnDamageQueue policy, out string exception)
        {
            try
            {
                exception = string.Empty;
                var processQueue = _OwnDamageQueueRepository.Table.Where(a => a.Id == policy.Id && a.ProcessedOn == null && a.IsLocked == true).FirstOrDefault();
                if (processQueue == null)
                {
                    exception = "processQueue is null id " + policy.Id;
                    return false;
                }
                processQueue.ProcessedOn = policy.ProcessedOn;
                if (!string.IsNullOrEmpty(policy.ErrorDescription))
                {
                    processQueue.ErrorDescription = policy.ErrorDescription;
                }
                processQueue.ProcessingTries = processQueue.ProcessingTries + 1;
                processQueue.IsLocked = false;
                processQueue.ModifiedDate = DateTime.Now;
                _OwnDamageQueueRepository.Update(processQueue);
                return true;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                var processQueue = _OwnDamageQueueRepository.Table.Where(a => a.Id == policy.Id && a.IsLocked == true).FirstOrDefault();
                if (processQueue != null)
                {
                    processQueue.ModifiedDate = DateTime.Now;
                    processQueue.IsLocked = false;
                    processQueue.ProcessingTries = processQueue.ProcessingTries + 1;
                    _OwnDamageQueueRepository.Update(processQueue);
                }
                return false;
            }
        }

        #endregion
    }
}
