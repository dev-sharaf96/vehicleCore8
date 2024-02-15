using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Orders;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums.Quotations;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Infrastructure;
using Tameenk.Integration.Core.Providers;
using Tameenk.Integration.Core.Providers.Configuration;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Integration.Providers.Tawuniya.Dtos;
using Tameenk.Integration.Providers.Tawuniya.Resources;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Quotations;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Logging;
using QuotationEntity = Tameenk.Core.Domain.Entities.Quotations;

namespace Tameenk.Integration.Providers.Tawuniya
{
    public class TawuniyaInsuranceProvider : RestfulInsuranceProvider
    {
        #region Fields
        private readonly TameenkConfig _tameenkConfig;
        private readonly IHttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly IRepository<PolicyProcessingQueue> _policyProcessingQueueRepository;
        private readonly IRepository<QuotationEntity.TawuniyaProposal> _tawuniyaProposalRepository;
        private readonly IQuotationService _quotationService;
        private readonly IRepository<QuotationEntity.QuotationRequest> _quotationRequestRepository;
        private readonly IRepository<QuotationEntity.QuotationResponse> _quotationResponseRepository;
        private readonly IRepository<Product> _productRepository;
        private const string GET_PROPOSAL_URL = "https://webapis.tawuniya.com.sa:5556/gateway/MotorProposal/TawnMotorQuotation/v01/restful/getProposals";

        private const string GET_PROPOSAL_AutoLease_URL = "https://webapis.tawuniya.com.sa:5556/gateway/TawnMotorLeasing/1/TawnMotorLeasing.V01.Business.restful.quoteRequest";
        private const string GET_QUOTATION_AutoLease_URL = "https://webapis.tawuniya.com.sa:5556/gateway/TawnMotorLeasing/1/TawnMotorLeasing.V01.Business.restful.policy";
        private const string GET_POLICY_AutoLease_URL = "https://webapis.tawuniya.com.sa:5556/gateway/TawnMotorLeasing/1/TawnMotorLeasing.V01.Business.restful.finalizePolicy";
        private const string TAWUNIYA_API_KEY = "twanapikey";
        private const string TAWUNIYA_API_ACCESS_KEY = "71b428e6-0bea-46dd-b5ec-5306acd5dd7c";
        private const string COMPREHENSIVE_CODE = "PRMotorImtiazeCO";
        private const string TPL_CODE = "MotorImtiazeTP";
        private const string SANAD_PLUS_CODE = "PRMotorImtiazeSP";
        private readonly IRepository<Insured> insuredRepo;
        private readonly IRepository<Address> addressRepo;
        private readonly IRepository<Bank> bankRepo;
        private readonly IRepository<BankInsuranceCompany> insuranceBankRepo;
        private readonly IRepository<BankCode> bankCodesRepo;
        private readonly IRepository<NCDFreeYear> ncdRepository;
        private readonly IRepository<BankNins> bankNinRepo;
        private readonly IRepository<Driver> driverRepo;
        private readonly IRepository<Vehicle> _vehicleRepository;
        private readonly IRepository<VehicleModel> _vehicleModelRepository;
        private readonly IRepository<ShoppingCartItem> _shoppingCartItemRepository;
        private readonly IRepository<Product_Benefit> _productBenefitRepository;
        #endregion

        #region Ctor

        public TawuniyaInsuranceProvider(TameenkConfig tameenkConfig, ILogger logger, IRepository<PolicyProcessingQueue> policyProcessingQueueRepository
            , IRepository<QuotationEntity.QuotationRequest> quotationRequestRepository
            , IRepository<QuotationEntity.TawuniyaProposal> tawuniyaProposalRepository
            , IRepository<QuotationEntity.QuotationResponse> quotationResponseRepository
            , IRepository<Product> productRepository
            , IQuotationService quotationService,
             IRepository<Insured> _insuredRepo
            , IRepository<Address> _addressRepo
            , IRepository<Bank> _bankRepo
            , IRepository<BankInsuranceCompany> _insuranceBankRepo
            , IRepository<BankCode> _bankCodesRepo
            , IRepository<NCDFreeYear> _ncdRepository
            , IRepository<BankNins> _bankNinRepo
            , IRepository<Driver> _driverRepo
            , IRepository<Vehicle> vehicleRepository
            , IRepository<VehicleModel> vehicleModelRepository
            , IRepository<ShoppingCartItem> shoppingCartItemRepository
            , IRepository<Product_Benefit> productBenefitRepository)
            : base(tameenkConfig, new RestfulConfiguration()
            {
                GenerateAutoleasingQuotationUrl = GET_QUOTATION_AutoLease_URL,
                GenerateAutoleasingPolicyUrl = GET_POLICY_AutoLease_URL,
                GenerateQuotationUrl = "https://webapis.tawuniya.com.sa:5556/rest/TawnMotorQuotation/v01/restful/createQuote",
                GeneratePolicyUrl = "https://webapis.tawuniya.com.sa:5556/rest/TawnMotorPolicy/v01/restful/createPolicy",
                AccessToken = "bCaRe:BcArE",
                ProviderName = "Tawuniya"
            }, logger, policyProcessingQueueRepository)
        {
            _tameenkConfig = tameenkConfig ?? throw new TameenkArgumentNullException(nameof(TameenkConfig));
            _httpClient = EngineContext.Current.Resolve<IHttpClient>();
            _logger = logger;
            _policyProcessingQueueRepository = policyProcessingQueueRepository;
            _quotationRequestRepository = quotationRequestRepository;
            _quotationResponseRepository = quotationResponseRepository;
            _tawuniyaProposalRepository = tawuniyaProposalRepository;
            _quotationService = quotationService;
            _productRepository = productRepository;
            insuredRepo = _insuredRepo;
            bankRepo = _bankRepo;
            insuranceBankRepo = _insuranceBankRepo;
            bankCodesRepo = _bankCodesRepo;
            ncdRepository = _ncdRepository;
            addressRepo = _addressRepo;
            bankNinRepo = _bankNinRepo;
            driverRepo = _driverRepo;
            _vehicleRepository = vehicleRepository;
            _vehicleModelRepository = vehicleModelRepository;
            _shoppingCartItemRepository = shoppingCartItemRepository;
            _productBenefitRepository = productBenefitRepository;
        }

        #endregion

        #region Methods
        protected override object ExecuteQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            try
            {
                // var output = GetTawuniyaQuotation(quotation, predefinedLogInfo);
                var output = GetTawuniyaProposal(quotation, predefinedLogInfo);
                return output.Output;
            }
            catch (Exception ex)
            {
                _logger.Log($"TawuniyaInsuranceProvider -> ExecuteQuotationRequest - (Provider name: {Configuration.ProviderName})", ex, LogLevel.Error);
                return null;
            }

        }

        protected override object ExecutePolicyRequest(PolicyRequest policy, ServiceRequestLog predefinedLogInfo)
        {
            PolicyOutput output = SubmitPolicyRequest(policy, predefinedLogInfo);
            return output.Output;

        }

        protected PolicyOutput SubmitPolicyRequest(PolicyRequest policy, ServiceRequestLog log)
        {

            PolicyOutput output = new PolicyOutput();
            var configuration = Configuration as RestfulConfiguration;
            log.ReferenceId = policy.ReferenceId;
            log.Channel = "Portal";
            log.ServiceURL = configuration.GeneratePolicyUrl;

            log.ServerIP = ServicesUtilities.GetServerIP();

            log.CompanyName = configuration.ProviderName;
            var stringPayload = string.Empty;
            var policyRequest = CreatePolicyRequest(policy);
            log.ServiceRequest = JsonConvert.SerializeObject(policyRequest);
            var request = _policyProcessingQueueRepository.Table.Where(a => a.ReferenceId == policy.ReferenceId).FirstOrDefault();
            if (request != null)
            {
                request.RequestID = log.RequestId;
                request.CompanyName = log.CompanyName;
                request.CompanyID = log.CompanyID;
                request.InsuranceTypeCode = log.InsuranceTypeCode;
                request.DriverNin = log.DriverNin;
                request.VehicleId = log.VehicleId;
                request.ServiceRequest = log.ServiceRequest;
            }
            HttpResponseMessage message = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK
            };
            try
            {
                var testMode = _tameenkConfig.Policy.TestMode;
                if (testMode)
                {
                    string nameOfFileJson = ".TestData.policyTestData.json";
                    string responseDataJson = ReadResource(GetType().Namespace, nameOfFileJson);
                    var handledResponse = JsonConvert.SerializeObject(HandleTawuniyaPolicyResponse(responseDataJson));
                    message.Content = new StringContent(handledResponse);
                    output.Output = message;
                    output.ErrorCode = PolicyOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";

                    return output;
                }

                //if (request != null
                //    && policy!=null
                //    && (policy.QuotationNo == "CheckProductDetail" 
                //    || (!string.IsNullOrEmpty(request.ErrorDescription)&&request.ErrorDescription.Contains("quotation service is down"))))
                //{
                //    var quotationOutput = GetTawuniyaQuotationTemp(policy, log);
                //    if (quotationOutput.ErrorCode != QuotationOutput.ErrorCodes.Success)
                //    {
                //        log.Method = "Policy";
                //        output.ErrorCode = PolicyOutput.ErrorCodes.ServiceError;
                //        output.ErrorDescription = "quotation service " + quotationOutput.ErrorDescription;
                //        log.ErrorCode = (int)output.ErrorCode;
                //        log.ErrorDescription = output.ErrorDescription;
                //        if (request != null)
                //        {
                //            request.ErrorDescription = " quotation service is down " + quotationOutput.ErrorDescription;
                //            request.ServiceRequest = quotationOutput.ServiceRequest;
                //            request.ServiceResponse = quotationOutput.ServiceResponse;
                //            _policyProcessingQueueRepository.Update(request);
                //        }
                //        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                //        return output;
                //    }
                //}

                policyRequest = CreatePolicyRequest(policy);
                stringPayload = JsonConvert.SerializeObject(policyRequest);
                log.ServiceRequest = stringPayload;
                log.Method = "Policy";
                if (request != null)
                {
                    request.ServiceRequest = log.ServiceRequest;
                }
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                //string authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "Bcare", "manage")));
                string authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes(configuration.AccessToken));

                DateTime dtBeforeCalling = DateTime.Now;
                var response = _httpClient.PostAsync(configuration.GeneratePolicyUrl, httpContent, authToken, null, "Basic").Result;
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null)
                {
                    //update policyProcessingQueue Table;
                    if (request != null)
                    {
                        request.ErrorDescription = " service Return null";
                        _policyProcessingQueueRepository.Update(request);
                    }
                    output.ErrorCode = PolicyOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (response.Content == null)
                {
                    if (request != null)
                    {
                        request.ErrorDescription = " service response content return null";
                        _policyProcessingQueueRepository.Update(request);
                    }
                    output.ErrorCode = PolicyOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    if (request != null)
                    {
                        request.ErrorDescription = " Service response content result return null";
                        _policyProcessingQueueRepository.Update(request);
                    }
                    output.ErrorCode = PolicyOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content result return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                var value = response.Content.ReadAsStringAsync().Result;
                log.ServiceResponse = value;
                var tawuniyaModifiedResponse = HandleTawuniyaPolicyResponse(value);
                var tawuniyaJson = JsonConvert.SerializeObject(tawuniyaModifiedResponse);
                message.Content = new StringContent(tawuniyaJson);
                request.ServiceResponse = log.ServiceResponse;

                if (tawuniyaModifiedResponse.StatusCode != 1)
                {
                    StringBuilder servcieErrors = new StringBuilder();
                    StringBuilder servcieErrorsCodes = new StringBuilder();

                    foreach (var error in tawuniyaModifiedResponse.Errors)
                    {
                        servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);
                        servcieErrorsCodes.AppendLine(error.Code);
                    }
                    output.ErrorCode = PolicyOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Policy Service response error is : " + servcieErrors.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = servcieErrorsCodes.ToString();
                    log.ServiceErrorDescription = servcieErrors.ToString();
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    if (request != null)
                    {
                        request.ErrorDescription = output.ErrorDescription;
                        _policyProcessingQueueRepository.Update(request);
                    }

                    return output;
                }
                if (string.IsNullOrEmpty(tawuniyaModifiedResponse?.PolicyNo))
                {
                    output.ErrorCode = PolicyOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "No PolicyNo returned from company";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    if (request != null)
                    {
                        request.ErrorDescription = output.ErrorDescription;
                        _policyProcessingQueueRepository.Update(request);
                    }
                    return output;
                }


                output.Output = message;
                output.ErrorCode = PolicyOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.PolicyNo = tawuniyaModifiedResponse?.PolicyNo;

                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                if (request != null)
                {
                    request.ErrorDescription = output.ErrorDescription;
                    _policyProcessingQueueRepository.Update(request);
                }
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = PolicyOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                if (request != null)
                {
                    request.ErrorDescription = output.ErrorDescription;
                    _policyProcessingQueueRepository.Update(request);
                }
                return output;
            }
        }

        public override bool ValidateQuotationBeforeCheckout(QuotationEntity.QuotationRequest quotationRequest, out List<string> errors)
        {
            var addressService = EngineContext.Current.Resolve<IAddressService>();
            errors = new List<string>();
            var mainDriverAddress = quotationRequest.Driver.Addresses.FirstOrDefault();
            if (mainDriverAddress != null)
            {
                var nationalAddressCity = addressService.GetCityCenterById(mainDriverAddress.CityId);
                // var insuredCityCenter = addressService.GetCityCenterCenterByElmCode(quotationRequest.Insured.CityId.ToString());
                if (nationalAddressCity.IsActive && nationalAddressCity?.ELM_Code != quotationRequest.Insured.CityId.ToString())
                {
                    errors.Add(InsuranceProvidersResource.InsuredCityDoesNotMatchAddressCity);
                    long cityId = 0;
                    long.TryParse(nationalAddressCity.ELM_Code, out cityId);
                    quotationRequest.Insured.CityId = cityId;
                    quotationRequest.Insured.WorkCityId = cityId;
                    return false;
                }
            }
            return true;
        }

        #region Autoleasing
        protected override object ExecuteAutoleasingQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            try
            {
                var output = GetAutoLeaseProposalsRequest(quotation, predefinedLogInfo);
                return output.Output;
            }
            catch (Exception ex)
            {
                _logger.Log($"TawuniyaInsuranceProvider -> ExecuteAutoleasingQuotationRequest - (Provider name: {Configuration.ProviderName})", ex, LogLevel.Error);
                return null;
            }
        }

        protected override object ExecuteAutoleasingPolicyRequest(PolicyRequest policy, ServiceRequestLog predefinedLogInfo)
        {
            PolicyOutput output = SubmitAutoleasingPolicyRequest(policy, predefinedLogInfo);
            return output.Output;
        }

        private QuotationOutput GetAutoLeaseProposalsRequest(QuotationServiceRequest quotation, ServiceRequestLog log)
        {
            var configuration = Configuration as RestfulConfiguration;
            HttpResponseMessage message = new HttpResponseMessage();
            message.StatusCode = System.Net.HttpStatusCode.OK;
            QuotationOutput output = new QuotationOutput();
            log.ReferenceId = quotation.ReferenceId;
            log.ServiceURL = GET_PROPOSAL_AutoLease_URL;
            if (string.IsNullOrEmpty(log.Channel))
                log.Channel = Channel.autoleasing.ToString();
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "AutoleasingProposal";
            log.CompanyName = "Tawuniya";
            log.VehicleMaker = quotation?.VehicleMaker;
            log.VehicleMakerCode = quotation?.VehicleMakerCode;
            log.VehicleModel = quotation?.VehicleModel;
            log.VehicleModelCode = quotation?.VehicleModelCode;
            log.VehicleModelYear = quotation?.VehicleModelYear;

            try
            {
                if (_tameenkConfig.Quotatoin.TestMode)
                {
                    string nameOfFileJson = ".TestData.quotationTestData.json";
                    string tawuniyaResponse = ReadResource(GetType().Namespace, nameOfFileJson);
                    var handledResponse = JsonConvert.SerializeObject(HandleProposalResponse(tawuniyaResponse, quotation));
                    message.Content = new StringContent(handledResponse);
                    output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    return output;
                }

                AutoLeaseProposalsRequest request = HandleAutoLeaseProposalsRequest(quotation);
                string stringPayload = JsonConvert.SerializeObject(request);
                log.ServiceRequest = stringPayload;
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                //string authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes(configuration.AutoleasingAccessToken));
                DateTime dtBeforeCalling = DateTime.Now;

                HttpResponseMessage response = null;
                using (var httpClient = new HttpClient())
                {
                    var requestHeader = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(GET_PROPOSAL_AutoLease_URL),
                        Content = new StringContent(stringPayload, Encoding.UTF8, "application/json")
                    };
                    requestHeader.Headers.Add("Accept", "application/json");
                    requestHeader.Headers.Add(TAWUNIYA_API_KEY, TAWUNIYA_API_ACCESS_KEY);
                    httpClient.DefaultRequestHeaders.ExpectContinue = false;
                    response = httpClient.SendAsync(requestHeader).Result;
                }

                //Dictionary<string, string> headers = new Dictionary<string, string>
                //{
                //    { TAWUNIYA_API_KEY, TAWUNIYA_API_ACCESS_KEY }
                //};
                //HttpResponseMessage response = _httpClient.PostAsync(GET_PROPOSAL_AutoLease_URL, httpContent, null, null, "Basic", headers).Result;

                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                if (response.Content == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content result return null and response.StatusCode is " + response.StatusCode + " response.Content.ReadAsStringAsync() " + response.Content.ReadAsStringAsync().Status.ToString() + " response.Content.ReadAsStringAsync().Result " + response.Content.ReadAsStringAsync().Result;
                    log.ServiceResponse = response.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                var value = response.Content.ReadAsStringAsync().Result;
                log.ServiceResponse = value;

                var tawuniyaModifiedResponse = HandleAutoLeaseProposalsResponse(value, quotation.DeductibleValue);
                var tawuniyaJson = JsonConvert.SerializeObject(tawuniyaModifiedResponse);

                message.Content = new StringContent(tawuniyaJson);

                if (tawuniyaModifiedResponse.StatusCode != 1)
                {
                    StringBuilder servcieErrors = new StringBuilder();
                    StringBuilder servcieErrorsCodes = new StringBuilder();

                    foreach (var error in tawuniyaModifiedResponse.Errors)
                    {
                        servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);
                        servcieErrorsCodes.AppendLine(error.Code);
                    }

                    output.ErrorCode = QuotationOutput.ErrorCodes.ServiceError;
                    string _ErrorDescription = "Quotation Service response error is : " + servcieErrors.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = servcieErrorsCodes.ToString();
                    log.ServiceErrorDescription = servcieErrors.ToString();
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                output.Output = message;
                output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                //log.ServiceResponse = response.Content.ReadAsStringAsync().Result;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }

        private AutoLeaseProposalsRequest HandleAutoLeaseProposalsRequest(QuotationServiceRequest quotation)
        {
            var bank = bankRepo.Table.FirstOrDefault(e => e.Id == quotation.BankId);

            var driver = quotation.Drivers.FirstOrDefault();
            var insured = insuredRepo.TableNoTracking.OrderByDescending(e => e.NationalId == driver.DriverId.ToString()).FirstOrDefault();
            var addressService = EngineContext.Current.Resolve<IAddressService>();
            //var address = addressRepo.TableNoTracking.OrderByDescending(e => e.NationalId == driver.DriverId.ToString()).FirstOrDefault();
            var address = addressService.GetAddressesByNin(driver.DriverId.ToString());

            AutoLeaseProposalsRequest proposalsautoleaserequest = new AutoLeaseProposalsRequest();
            if (!string.IsNullOrEmpty(quotation.QuotationNo))
            {
                proposalsautoleaserequest.RequestReferenceNo = quotation.QuotationNo;
            }
            else
            {
                proposalsautoleaserequest.RequestReferenceNo = quotation.ReferenceId;
            }
            proposalsautoleaserequest.SourceCode = 757701;
            proposalsautoleaserequest.UserName = "BCA001";
            proposalsautoleaserequest.SchemeCode = "AGGREGATOR";

            //Lessor data
            proposalsautoleaserequest.Details = new AutoLeaseProposalRequestDetails();
            proposalsautoleaserequest.Details.LessorNameEN = bank.NameAr; //"Auto Leasing";//need to get bank name from DB 
            proposalsautoleaserequest.Details.LessorID = long.Parse(bankNinRepo.Table.FirstOrDefault(e => e.BankId == bank.Id).NIN);
            proposalsautoleaserequest.Details.LessorBranch = 123; //need to get bank name from DB 
            proposalsautoleaserequest.Details.LessorNationalAddress = new List<NationalAddress>() // need to get address from bank DB     
            {
                 new NationalAddress()
                 {
                     BuildingNumber=124,
                     Street="1",
                     District="test",
                     City="Riyadh",
                     ZipCode=12345,
                     AdditionalNumber = 7825
                 }
            };

            proposalsautoleaserequest.Details.LessorContactPerson = bank.NameAr;
            proposalsautoleaserequest.Details.LessorContactNumber = bank.PhoneNumber;
            proposalsautoleaserequest.Details.LessorIBAN = bank.IBAN;
            proposalsautoleaserequest.Details.BankCode = bank.IBAN.ToLower().Replace("sa", "").Substring(2, 2);
            proposalsautoleaserequest.Details.IsRenewal = false; // quotation.IsRenewal.HasValue ? quotation.IsRenewal.Value : false; ;//quotation.IsRenewal; // “True”: means Lessee already have an active policy or expired policy (within 30 days)  with Tawuniya “False”: it is Not a renewal
            proposalsautoleaserequest.Details.NajmCaseDetails = null;  //If IsRenewal:true, then Yes else No

            proposalsautoleaserequest.Details.PurposeofVehicleUseID = byte.Parse(quotation.VehicleUseCode.ToString());
            proposalsautoleaserequest.Details.Cylinders = quotation.VehicleCylinders.ToString();
            proposalsautoleaserequest.Details.VehicleMileage = quotation.VehicleMileage;
            proposalsautoleaserequest.Details.VehicleExpectedMileageYear = null; //quotation.VehicleMileageExpectedAnnualCode;
            proposalsautoleaserequest.Details.VehicleEngineSizeCC = null; //User Input
            proposalsautoleaserequest.Details.VehicleTransmission = quotation.VehicleTransmissionTypeCode;
            proposalsautoleaserequest.Details.VehicleCapacity = quotation.VehicleCapacity;
            if (quotation.VehicleOvernightParkingLocationCode.HasValue)
                proposalsautoleaserequest.Details.VehicleNightParking = byte.Parse(quotation.VehicleOvernightParkingLocationCode.Value.ToString());
            else
                proposalsautoleaserequest.Details.VehicleNightParking = byte.Parse("1");

            // Will be used after Mapping

            //if (!string.IsNullOrEmpty(quotation.VehicleMakerCode) && !string.IsNullOrEmpty(quotation.VehicleModelCode))
            //{
            //    short vehicleMakerCode = -1;
            //    long vehicleModelCode = -1;
            //    short.TryParse(quotation.VehicleMakerCode, out vehicleMakerCode);
            //    long.TryParse(quotation.VehicleModelCode, out vehicleModelCode);

            //    var vehicleModel = GetTawuniyaModelCode(vehicleMakerCode, vehicleModelCode);
            //    if (vehicleModel != null)
            //    {
            //        if (vehicleModel.TawuniyaMakerCode.HasValue && vehicleModel.TawuniyaModelCode.HasValue)
            //        {
            //            proposalsautoleaserequest.Details.VehicleMakeCodeNIC = vehicleModel.TawuniyaMakerCode.Value;
            //            proposalsautoleaserequest.Details.VehicleMakeTextNIC = vehicleModel.VehicleMaker.ArabicDescription;
            //            proposalsautoleaserequest.Details.VehicleModelCodeNIC = (short)vehicleModel.TawuniyaModelCode.Value;
            //            proposalsautoleaserequest.Details.VehicleModelTextNIC = vehicleModel.ArabicDescription;
            //        }
            //        else
            //        {
            //            proposalsautoleaserequest.Details.VehicleMakeCodeNIC = string.IsNullOrEmpty(quotation.VehicleMakerCode) ? (short)0 : short.Parse(quotation.VehicleMakerCode);
            //            proposalsautoleaserequest.Details.VehicleMakeTextNIC = quotation.VehicleMaker;
            //            proposalsautoleaserequest.Details.VehicleModelCodeNIC = string.IsNullOrEmpty(quotation.VehicleModelCode) ? (short)0 : short.Parse(quotation.VehicleModelCode);
            //            proposalsautoleaserequest.Details.VehicleModelTextNIC = quotation.VehicleModel;
            //        }
            //    }
            //}

            proposalsautoleaserequest.Details.VehicleMakeCodeNIC = string.IsNullOrEmpty(quotation.VehicleMakerCode) ? (short)0 : short.Parse(quotation.VehicleMakerCode);
            proposalsautoleaserequest.Details.VehicleMakeTextNIC = quotation.VehicleMaker;
            proposalsautoleaserequest.Details.VehicleModelCodeNIC = string.IsNullOrEmpty(quotation.VehicleModelCode) ? (short)0 : short.Parse(quotation.VehicleModelCode);
            proposalsautoleaserequest.Details.VehicleModelTextNIC = quotation.VehicleModel;

            proposalsautoleaserequest.Details.ManufactureYear = quotation.VehicleModelYear;
            proposalsautoleaserequest.Details.VehicleColorCode = string.IsNullOrEmpty(quotation.VehicleMajorColorCode) ? (short)1 : short.Parse(quotation.VehicleMajorColorCode);
            proposalsautoleaserequest.Details.VehicleModifications = quotation.VehicleModificationDetails;
            proposalsautoleaserequest.Details.VehicleSumInsured = quotation.VehicleValue.Value;
            proposalsautoleaserequest.Details.RepairMethod = (quotation.VehicleAgencyRepair.HasValue && quotation.VehicleAgencyRepair.Value) ? 1 : 2; //int.Parse(quotation.VehicleAgencyRepair.Value.ToString());

            //Lessee data= main driver data
            proposalsautoleaserequest.Details.LesseeID = driver.DriverId;
            proposalsautoleaserequest.Details.FullName = string.Format("{0} {1} {2}", driver.DriverFirstNameAr, driver.DriverMiddleNameAr, driver.DriverLastNameAr);
            proposalsautoleaserequest.Details.ArabicFirstName = driver.DriverFirstNameAr;
            proposalsautoleaserequest.Details.ArabicMiddleName = driver.DriverMiddleNameAr;
            proposalsautoleaserequest.Details.ArabicLastName = driver.DriverLastNameAr;
            proposalsautoleaserequest.Details.EnglishFirstName = driver.DriverFirstNameEn;
            proposalsautoleaserequest.Details.EnglishMiddleName = driver.DriverMiddleNameEn;
            proposalsautoleaserequest.Details.EnglishLastName = driver.DriverLastNameEn;
            proposalsautoleaserequest.Details.LesseeNationalityID = driver.DriverId.ToString().StartsWith("1") ? (short?)0 : short.Parse(driver.DriverNationalityCode);
            proposalsautoleaserequest.Details.VehicleUsagePercentage = driver.DriverDrivingPercentage.HasValue ? short.Parse(driver.DriverDrivingPercentage.ToString()) : (short)0;
            proposalsautoleaserequest.Details.LesseeOccupation = driver.DriverOccupation;
            proposalsautoleaserequest.Details.LesseeEducation = driver.DriverEducationCode.HasValue ? Enum.GetNames(typeof(EducationAr))[driver.DriverEducationCode.Value - 1] : "";  //((EducationAr)Enum.Parse(typeof(EducationAr),driver.DriverEducationCode.ToString())).ToString();
            proposalsautoleaserequest.Details.LesseeChildrenBelow16 = quotation.InsuredChildrenBelow16Years.HasValue ? quotation.InsuredChildrenBelow16Years.Value : 0;
            proposalsautoleaserequest.Details.LesseeWorkCompanyName = null;
            proposalsautoleaserequest.Details.LesseeWorkCityID = quotation.InsuredWorkCityCode;
            proposalsautoleaserequest.Details.CountriesValidDrivingLicense = null;//new List<CountriesValidDrivingLicense>() { new CountriesValidDrivingLicense { DriverLicenseYears = 12, DrivingLicenseCountryID = 11 } };
            proposalsautoleaserequest.Details.LesseeNoOfClaims = driver.DriverNOCLast5Years.HasValue ? driver.DriverNOCLast5Years.Value.ToString() : null;
            proposalsautoleaserequest.Details.LesseeTrafficViolationsCode = driver.DriverViolations.FirstOrDefault()?.ViolationCode.ToString();
            proposalsautoleaserequest.Details.LesseeHealthConditionsCode = driver.DriverMedicalConditionCode.HasValue ? driver.DriverMedicalConditionCode.ToString() : null;
            proposalsautoleaserequest.Details.LesseeDateOfBirthG = driver.DriverId.ToString().StartsWith("1") ? null : driver.DriverBirthDateG.ToString("dd-MM-yyyy", new CultureInfo("en-US"));
            proposalsautoleaserequest.Details.LesseeDateOfBirthH = driver.DriverId.ToString().StartsWith("1") ? driver.DriverBirthDate : null;
            proposalsautoleaserequest.Details.LesseeGender = driver.DriverGenderCode == "M" ? byte.Parse("1") : byte.Parse("2");
            proposalsautoleaserequest.Details.LesseeMaritalStatus = string.IsNullOrEmpty(driver.DriverSocialStatusCode) ? (byte?)null : byte.Parse(driver.DriverSocialStatusCode);
            proposalsautoleaserequest.Details.LesseeLicenseType = driver.DriverLicenses.Count() != 0 ? byte.Parse(driver.DriverLicenses.FirstOrDefault().DriverLicenseTypeCode) : new byte();
            proposalsautoleaserequest.Details.LesseeLicenseOwnYears = driver.DriverLicenses.Count() != 0 ? byte.Parse(driver.DriverLicenses.FirstOrDefault().LicenseNumberYears.ToString()) : new byte();
            proposalsautoleaserequest.Details.LesseeNCDCode = quotation.NCDFreeYears;//driver.DriverNCDFreeYears.HasValue? driver.DriverNCDFreeYears.Value:0;
            proposalsautoleaserequest.Details.LesseeNCDReference = driver.DriverNCDReference;
            proposalsautoleaserequest.Details.LesseeNoOfAccidents = quotation.NoOfAccident.HasValue ? quotation.NoOfAccident.Value : 0;

            if (address != null)
            {
                var lesseeNationalAddress = new NationalAddress();
                lesseeNationalAddress.City = address.City;
                lesseeNationalAddress.BuildingNumber = string.IsNullOrEmpty(address.BuildingNumber) ? (int?)null : int.Parse(address.BuildingNumber);
                lesseeNationalAddress.ZipCode = string.IsNullOrEmpty(address.PostCode) ? (int?)null : int.Parse(address.PostCode);
                lesseeNationalAddress.AdditionalNumber = string.IsNullOrEmpty(address.AdditionalNumber) ? (int?)null : int.Parse(address.AdditionalNumber);
                lesseeNationalAddress.Street = address.Street;
                lesseeNationalAddress.District = address.District;
                proposalsautoleaserequest.Details.LesseeNationalAddress = new List<NationalAddress>() { lesseeNationalAddress };
            }
            else
            {
                proposalsautoleaserequest.Details.LesseeNationalAddress = null;
            }
            proposalsautoleaserequest.Details.DriverDetails = quotation.Drivers.Skip(1).Count() != 0 ?
            quotation.Drivers.Skip(1).Select(e => new DriverDetails
            {
                DriverID = e.DriverId,
                DriverFullName = string.Format("{0} {1} {2}", e.DriverFirstNameAr, e.DriverMiddleNameAr, e.DriverLastNameAr),
                ArabicFirstName = e.DriverFirstNameAr,
                ArabicMiddleName = e.DriverMiddleNameAr,
                ArabicLastName = e.DriverLastNameAr,
                EnglishFirstName = e.DriverFirstNameEn,
                EnglishMiddleName = e.DriverMiddleNameEn,
                EnglishLastName = e.DriverLastNameEn,
                DriverChildrenBelow16 = e.DriverChildrenBelow16Years is null ? "" : e.DriverChildrenBelow16Years.ToString(),
                DriverDateOfBirthG = e.DriverId.ToString().StartsWith("1") ? null : e.DriverBirthDateG.ToString("dd-MM-yyyy", new CultureInfo("en-US")),
                DriverDateOfBirthH = e.DriverId.ToString().StartsWith("1") ? e.DriverBirthDate : null,
                DriverGender = e.DriverGenderCode == "M" ? byte.Parse("1") : byte.Parse("2"),
                DriverEducation = e.DriverEducationCode.HasValue ? Enum.GetNames(typeof(EducationAr))[e.DriverEducationCode.Value - 1] : "",
                DriverMaritalStatus = string.IsNullOrEmpty(e.DriverSocialStatusCode) ? 0 : int.Parse(e.DriverSocialStatusCode),
                DriverNationalityID = e.DriverId.ToString().StartsWith("1") ? 0 : int.Parse(e.DriverNationalityCode),
                DriverNoOfClaims = driver.DriverNOCLast5Years.HasValue ? driver.DriverNOCLast5Years.Value.ToString() : null,
                DriverOccupation = e.DriverOccupation,
                DriverWorkCompanyName = e.DriverWorkCity,
                DriverWorkCityID = e.DriverWorkCityCode,
                CountriesValidDrivingLicense = null,
                DriverRelation = null,
                VehicleUsagePercentage = e.DriverDrivingPercentage is null ? 0 : e.DriverDrivingPercentage.Value,
                DriverHealthConditionsCode = null,
                DriverHomeAddress = e.DriverHomeAddress,
                DriverHomeAddressCity = e.DriverHomeCity,
                DriverLicenseType = e.DriverLicenses.Count() != 0 ? byte.Parse(driver.DriverLicenses.FirstOrDefault()?.DriverLicenseTypeCode) : new byte(),
                DriverLicenseOwnYears = e.DriverLicenses.FirstOrDefault().LicenseNumberYears,
                DriverNoOfAccidents = 0
            }).ToList() : null;

            return proposalsautoleaserequest;
        }

        private VehicleModel GetTawuniyaModelCode(short elmMakerCode, long elmModelCode)
        {
            return _vehicleModelRepository.TableNoTracking.Include(m => m.VehicleMaker).FirstOrDefault(m => m.VehicleMakerCode == elmMakerCode && m.Code == elmModelCode);
        }

        private QuotationServiceResponse HandleAutoLeaseProposalsResponse(string value, int? deductibleValue)
        {
            AutoLeaseProposalsResponse Tawniya_AutoLease_response = JsonConvert.DeserializeObject<AutoLeaseProposalsResponse>(value);

            QuotationServiceResponse response = new QuotationServiceResponse();
            response.QuotationDate = null;
            response.Errors = Tawniya_AutoLease_response.Errors;
            response.StatusCode = Tawniya_AutoLease_response.Status == "true" ? 1 : 2;
            response.ReferenceId = Tawniya_AutoLease_response.RequestReferenceNo;

            var deductValue = deductibleValue.HasValue ? deductibleValue.Value.ToString() : "2000";
            if (response.StatusCode == 1)
            {
                if (!string.IsNullOrEmpty(Tawniya_AutoLease_response.CompQuotes[0].QuoteReferenceNo.ToString()))
                    response.QuotationNo = Tawniya_AutoLease_response.CompQuotes[0].QuoteReferenceNo.ToString();
                else
                    response.QuotationNo = "";

                List<BenefitDto> list_benefits = new List<BenefitDto>();
                List<ProductDto> list_products = new List<ProductDto>();
                List<PriceDto> list_pricedetails = new List<PriceDto>();

                foreach (CompQuotes product in Tawniya_AutoLease_response.CompQuotes)
                {
                    Deductibles deduct = product.Deductibles.FirstOrDefault(e => e.DeductibleAmount == deductValue);

                    list_products.Add(new ProductDto
                    {
                        ProductId = product.PolicyTitleID,
                        ProductPrice = Convert.ToDecimal(deduct.PolicyPremium),
                        DeductibleValue = int.Parse(deduct.DeductibleAmount)
                    });

                    if (deduct.PremiumBreakdown != null)
                    {
                        var priceDetail = deduct.PremiumBreakdown.FirstOrDefault(e => e.BreakdownTypeID == "2");
                        if (priceDetail != null)
                            list_pricedetails.Add(new PriceDto
                            {
                                PriceValue = Convert.ToDecimal(priceDetail.BreakdownAmount),
                                PercentageValue = 0,
                                PriceTypeCode = 7
                            });

                        priceDetail = deduct.PremiumBreakdown.FirstOrDefault(e => e.BreakdownTypeID == "20");
                        if (priceDetail != null)
                            list_pricedetails.Add(new PriceDto
                            {
                                PriceValue = Convert.ToDecimal(priceDetail.BreakdownAmount) * 0.15M,
                                PercentageValue = 15,
                                PriceTypeCode = 8
                            });

                        priceDetail = deduct.PremiumBreakdown.FirstOrDefault(e => e.BreakdownTypeID == "3");
                        if (priceDetail != null)
                            list_pricedetails.Add(new PriceDto
                            {
                                PriceValue = Convert.ToDecimal(priceDetail.BreakdownAmount),
                                PercentageValue = 15,
                                PriceTypeCode = 4
                            });

                        priceDetail = deduct.PremiumBreakdown.FirstOrDefault(e => e.BreakdownTypeID == "1");
                        if (priceDetail != null)
                            list_pricedetails.Add(new PriceDto
                            {
                                PriceValue = Convert.ToDecimal(priceDetail.BreakdownAmount),
                                PercentageValue = 0,
                                PriceTypeCode = 6
                            });
                    }

                    if (deduct.Discounts != null)
                    {
                        var discount = deduct.Discounts.FirstOrDefault(e => e.DiscountTypeID == "1");
                        if (discount != null)
                            list_pricedetails.Add(new PriceDto
                            {
                                PriceValue = Convert.ToDecimal(discount.DiscountAmount),
                                PercentageValue = Convert.ToDecimal(discount.DiscountPercentage),
                                PriceTypeCode = 3
                            });

                        discount = deduct.Discounts.FirstOrDefault(e => e.DiscountTypeID == "2");
                        if (discount != null)
                            list_pricedetails.Add(new PriceDto
                            {
                                PriceValue = Convert.ToDecimal(discount.DiscountAmount),
                                PercentageValue = Convert.ToDecimal(discount.DiscountPercentage),
                                PriceTypeCode = 2
                            });

                        discount = deduct.Discounts.FirstOrDefault(e => e.DiscountTypeID == "5");
                        if (discount != null)
                            list_pricedetails.Add(new PriceDto
                            {
                                PriceValue = Convert.ToDecimal(discount.DiscountAmount),
                                PercentageValue = Convert.ToDecimal(discount.DiscountPercentage),
                                PriceTypeCode = 1
                            });

                        discount = deduct.Discounts.FirstOrDefault(e => e.DiscountTypeID == "6");
                        if (discount != null)
                            list_pricedetails.Add(new PriceDto
                            {
                                PriceValue = Convert.ToDecimal(discount.DiscountAmount),
                                PercentageValue = Convert.ToDecimal(discount.DiscountPercentage),
                                PriceTypeCode = 11
                            });
                    }

                    var benefits = product.PolicyPremiumFeatures;
                    if (benefits != null)
                    {
                        var feature2 = benefits.FirstOrDefault(e => e.FeatureID == "2");
                        if (feature2 != null)
                            list_benefits.Add(new BenefitDto
                            {
                                BenefitId = "2",
                                BenefitPrice = Convert.ToDecimal(feature2.FeatureAmount),
                                BenefitCode = 3,
                                IsSelected = feature2.FeatureTypeID.Equals("2") ? true : false,
                                IsReadOnly = feature2.FeatureTypeID.Equals("2") ? true : false
                            });

                        var feature3 = benefits.FirstOrDefault(e => e.FeatureID == "3");
                        if (feature3 != null)
                            list_benefits.Add(new BenefitDto
                            {
                                BenefitId = "3",
                                BenefitPrice = Convert.ToDecimal(feature3.FeatureAmount),
                                BenefitCode = 4,
                                IsSelected = feature3.FeatureTypeID.Equals("2") ? true : false,
                                IsReadOnly = feature3.FeatureTypeID.Equals("2") ? true : false
                            });

                        var feature4 = benefits.FirstOrDefault(e => e.FeatureID == "4");
                        if (feature4 != null)
                            list_benefits.Add(new BenefitDto
                            {
                                BenefitId = "4",
                                BenefitPrice = Convert.ToDecimal(feature4.FeatureAmount),
                                BenefitCode = 14,
                                IsSelected = feature4.FeatureTypeID.Equals("2") ? true : false,
                                IsReadOnly = feature4.FeatureTypeID.Equals("2") ? true : false
                            });

                        var feature5 = benefits.FirstOrDefault(e => e.FeatureID == "5");
                        if (feature5 != null)
                            list_benefits.Add(new BenefitDto
                            {
                                BenefitId = "5",
                                BenefitPrice = Convert.ToDecimal(feature5.FeatureAmount),
                                BenefitCode = 1,
                                IsSelected = feature5.FeatureTypeID.Equals("2") ? true : false,
                                IsReadOnly = feature5.FeatureTypeID.Equals("2") ? true : false
                            });

                        var feature6 = benefits.FirstOrDefault(e => e.FeatureID == "6");
                        if (feature6 != null)
                            list_benefits.Add(new BenefitDto
                            {
                                BenefitId = "6",
                                BenefitPrice = Convert.ToDecimal(feature6.FeatureAmount),
                                BenefitCode = 2,
                                IsSelected = feature6.FeatureTypeID.Equals("2") ? true : false,
                                IsReadOnly = feature6.FeatureTypeID.Equals("2") ? true : false
                            });

                        var feature7 = benefits.FirstOrDefault(e => e.FeatureID == "7");
                        if (feature7 != null)
                            list_benefits.Add(new BenefitDto
                            {
                                BenefitId = "7",
                                BenefitPrice = Convert.ToDecimal(feature7.FeatureAmount),
                                BenefitCode = 9,
                                IsSelected = feature7.FeatureTypeID.Equals("2") ? true : false,
                                IsReadOnly = feature7.FeatureTypeID.Equals("2") ? true : false
                            });

                        var feature8 = benefits.FirstOrDefault(e => e.FeatureID == "8");
                        if (feature8 != null)
                            list_benefits.Add(new BenefitDto
                            {
                                BenefitId = "8",
                                BenefitPrice = Convert.ToDecimal(feature8.FeatureAmount),
                                BenefitCode = 10,
                                IsSelected = feature8.FeatureTypeID.Equals("2") ? true : false,
                                IsReadOnly = feature8.FeatureTypeID.Equals("2") ? true : false
                            });

                        var feature9 = benefits.FirstOrDefault(e => e.FeatureID == "9");
                        if (feature9 != null)
                            list_benefits.Add(new BenefitDto
                            {
                                BenefitId = "9",
                                BenefitPrice = Convert.ToDecimal(feature9.FeatureAmount),
                                BenefitCode = 12,
                                IsSelected = feature9.FeatureTypeID.Equals("2") ? true : false,
                                IsReadOnly = feature9.FeatureTypeID.Equals("2") ? true : false
                            });

                        var feature10 = benefits.FirstOrDefault(e => e.FeatureID == "10");
                        if (feature10 != null)
                            list_benefits.Add(new BenefitDto
                            {
                                BenefitId = "10",
                                BenefitPrice = Convert.ToDecimal(feature10.FeatureAmount),
                                BenefitCode = 11,
                                IsSelected = feature10.FeatureTypeID.Equals("2") ? true : false,
                                IsReadOnly = feature10.FeatureTypeID.Equals("2") ? true : false
                            });
                    }
                    response.Products = list_products;
                    response.Products[0].PriceDetails = list_pricedetails;
                    response.Products[0].Benefits = list_benefits;
                }
                return response;
            }
            return response;
        }

        protected PolicyOutput SubmitAutoleasingPolicyRequest(PolicyRequest policy, ServiceRequestLog log)
        {

            PolicyOutput output = new PolicyOutput();
            var configuration = Configuration as RestfulConfiguration;
            log.ReferenceId = policy.ReferenceId;
            log.Channel = "autoleasing";
            log.ServiceURL = configuration.GeneratePolicyUrl;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.CompanyName = configuration.ProviderName;
            var stringPayload = string.Empty;
            var policyRequest = HandleAutoLeasePolicyRequest(policy);
            log.ServiceRequest = JsonConvert.SerializeObject(policyRequest);
            var request = _policyProcessingQueueRepository.Table.Where(a => a.ReferenceId == policy.ReferenceId).FirstOrDefault();
            if (request != null)
            {
                request.RequestID = log.RequestId;
                request.CompanyName = log.CompanyName;
                request.CompanyID = log.CompanyID;
                request.InsuranceTypeCode = log.InsuranceTypeCode;
                request.DriverNin = log.DriverNin;
                request.VehicleId = log.VehicleId;
                request.ServiceRequest = log.ServiceRequest;
            }
            HttpResponseMessage message = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK
            };
            try
            {
                var testMode = _tameenkConfig.Policy.TestMode;
                if (testMode)
                {
                    string nameOfFileJson = ".TestData.policyTestData.json";
                    string responseDataJson = ReadResource(GetType().Namespace, nameOfFileJson);
                    var handledResponse = JsonConvert.SerializeObject(HandleTawuniyaPolicyResponse(responseDataJson));
                    message.Content = new StringContent(handledResponse);
                    output.Output = message;
                    output.ErrorCode = PolicyOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";

                    return output;
                }

                policyRequest = HandleAutoLeasePolicyRequest(policy);
                stringPayload = JsonConvert.SerializeObject(policyRequest);
                log.ServiceRequest = stringPayload;
                log.Method = "AutoleasingPolicy";
                if (request != null)
                {
                    request.ServiceRequest = log.ServiceRequest;
                }
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                string authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes(configuration.AccessToken));

                DateTime dtBeforeCalling = DateTime.Now;
                HttpResponseMessage response = null;

                using (var httpClient = new HttpClient())
                {
                    var requestHeader = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(GET_POLICY_AutoLease_URL),
                        Content = new StringContent(stringPayload, Encoding.UTF8, "application/json")
                    };

                    requestHeader.Headers.Add("Accept", "application/json");
                    requestHeader.Headers.Add(TAWUNIYA_API_KEY, TAWUNIYA_API_ACCESS_KEY);
                    httpClient.DefaultRequestHeaders.ExpectContinue = false;

                    response = httpClient.SendAsync(requestHeader).Result;
                }
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null)
                {
                    //update policyProcessingQueue Table;
                    if (request != null)
                    {
                        request.ErrorDescription = " service Return null";
                        _policyProcessingQueueRepository.Update(request);
                    }
                    output.ErrorCode = PolicyOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (response.Content == null)
                {
                    if (request != null)
                    {
                        request.ErrorDescription = " service response content return null";
                        _policyProcessingQueueRepository.Update(request);
                    }
                    output.ErrorCode = PolicyOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    if (request != null)
                    {
                        request.ErrorDescription = " Service response content result return null";
                        _policyProcessingQueueRepository.Update(request);
                    }
                    output.ErrorCode = PolicyOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content result return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;
                PolicyResponse tawuniyaModifiedResponse = HandleAutoLeasePolicyResponse(response, policy);
                var tawuniyaJson = JsonConvert.SerializeObject(tawuniyaModifiedResponse);
                message.Content = new StringContent(tawuniyaJson);
                request.ServiceResponse = log.ServiceResponse;

                if (tawuniyaModifiedResponse.StatusCode != 1)
                {
                    StringBuilder servcieErrors = new StringBuilder();
                    StringBuilder servcieErrorsCodes = new StringBuilder();

                    foreach (var error in tawuniyaModifiedResponse.Errors)
                    {
                        servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);
                        servcieErrorsCodes.AppendLine(error.Code);
                    }
                    output.ErrorCode = PolicyOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Policy Service response error is : " + servcieErrors.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = servcieErrorsCodes.ToString();
                    log.ServiceErrorDescription = servcieErrors.ToString();
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    if (request != null)
                    {
                        request.ErrorDescription = output.ErrorDescription;
                        _policyProcessingQueueRepository.Update(request);
                    }

                    return output;
                }
                if (string.IsNullOrEmpty(tawuniyaModifiedResponse?.PolicyNo))
                {
                    output.ErrorCode = PolicyOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "No PolicyNo returned from company";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    if (request != null)
                    {
                        request.ErrorDescription = output.ErrorDescription;
                        _policyProcessingQueueRepository.Update(request);
                    }
                    return output;
                }


                output.Output = message;
                output.ErrorCode = PolicyOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.PolicyNo = tawuniyaModifiedResponse?.PolicyNo;

                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                if (request != null)
                {
                    request.ErrorDescription = output.ErrorDescription;
                    _policyProcessingQueueRepository.Update(request);
                }
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = PolicyOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                if (request != null)
                {
                    request.ErrorDescription = output.ErrorDescription;
                    _policyProcessingQueueRepository.Update(request);
                }
                return output;
            }
        }

        private PolicyResponse HandleAutoLeasePolicyResponse(object response, PolicyRequest request = null)
        {
            PolicyResponse policyResponse = null;

            string result = string.Empty;
            var stringPayload = JsonConvert.SerializeObject(request);
            try
            {
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
                AutoLeasingPolicyResponse responseValue = JsonConvert.DeserializeObject<AutoLeasingPolicyResponse>(result);
                if (IsValidPolicyResponseAutoleasing(responseValue))
                {
                    policyResponse = new PolicyResponse
                    {
                        Errors = responseValue.errors,
                        PolicyNo = responseValue.PolicyNumber,
                        ReferenceId = request.ReferenceId,
                        StatusCode = (responseValue.Status.ToLower() == "true") ? 1 : 2,
                        PolicyEffectiveDate = Utilities.ConvertStringToDateTimeFromAllianz(responseValue.PolicyEffectiveDate + " 00:00:00"),
                        PolicyExpiryDate = Utilities.ConvertStringToDateTimeFromAllianz(responseValue.PolicyExpiryDate + " 23:59:59"),
                        PolicyIssuanceDate = DateTime.Now
                    };
                }

                return policyResponse;
            }
            catch (Exception ex)
            {
                return policyResponse;
            }
        }

        private AutoLeasingPolicyRequest HandleAutoLeasePolicyRequest(PolicyRequest policy)
        {
            var vehicle_id = _quotationRequestRepository.TableNoTracking.Where(e => e.ID == policy.QuotationRequestId).Select(e => e.VehicleId).FirstOrDefault();
            var vehicle = _vehicleRepository.TableNoTracking.Where(e => e.ID == vehicle_id).FirstOrDefault();
            var mainDriver = _quotationRequestRepository.TableNoTracking.Include(q => q.Driver).FirstOrDefault(q => q.ID == policy.QuotationRequestId).Driver;
            AutoLeasingPolicyRequest policyautoleaserequest = new AutoLeasingPolicyRequest();

            policyautoleaserequest.RequestReferenceNo = policy.ReferenceId;
            policyautoleaserequest.PolicyRequestReferenceNo = policy.ReferenceId;
            policyautoleaserequest.QuoteReferenceNo = int.Parse(policy.QuotationNo);
            policyautoleaserequest.SourceCode = 757701;
            policyautoleaserequest.UserName = "BCA001";

            policyautoleaserequest.Details = new AutoleasingPolicyRequestDetails();
            policyautoleaserequest.Details.Email = policy.InsuredEmail;
            policyautoleaserequest.Details.MobileNo = policy.InsuredMobileNumber.TrimStart('0');
            policyautoleaserequest.Details.LesseeID = long.Parse(mainDriver.NIN);
            policyautoleaserequest.Details.PolicyEffectiveDate = policy.PolicyEffectiveDate.Value.ToString("yyyy-MM-dd", new CultureInfo("en-US"));
            policyautoleaserequest.Details.PolicyNumber = string.Empty; //Not Mandatory
            policyautoleaserequest.Details.PaidAmount = policy.PaymentAmount.ToString();
            policyautoleaserequest.Details.VehicleUniqueTypeID = vehicle.VehicleIdTypeId.ToString();

            if (vehicle.VehicleIdTypeId == 1)
            {
                policyautoleaserequest.Details.VehicleSequenceNumber = vehicle.SequenceNumber;
                policyautoleaserequest.Details.VehiclePlateTypeID = vehicle.PlateTypeCode.ToString();
                policyautoleaserequest.Details.VehiclePlateNumber = vehicle.CarPlateNumber.ToString();
                policyautoleaserequest.Details.FirstPlateLetterID = PlateCharacterMapping(vehicle.CarPlateText1);
                policyautoleaserequest.Details.SecondPlateLetterID = PlateCharacterMapping(vehicle.CarPlateText2);
                policyautoleaserequest.Details.ThirdPlateLetterID = PlateCharacterMapping(vehicle.CarPlateText3);
                policyautoleaserequest.Details.VehicleRegistrationExpiryDate = vehicle.LicenseExpiryDate;
            }
            else
            {
                policyautoleaserequest.Details.VehicleCustomID = vehicle.CustomCardNumber;
            }

            policyautoleaserequest.Details.VehicleVIN = vehicle.ChassisNumber;
            return policyautoleaserequest;
        }

        private string PlateCharacterMapping(string plateCharacter)
        {
            string mappedCharacter = null;
            if (plateCharacter == "أ" || plateCharacter == "ا" || plateCharacter == "A")
            {
                mappedCharacter = "1";
            }
            else if (plateCharacter == "ب" || plateCharacter == "B")
            {
                mappedCharacter = "2";
            }
            else if (plateCharacter == "د" || plateCharacter == "D")
            {
                mappedCharacter = "3";
            }
            else if (plateCharacter == "ح" || plateCharacter == "J")
            {
                mappedCharacter = "4";
            }
            else if (plateCharacter == "ه" || plateCharacter == "H")
            {
                mappedCharacter = "5";
            }
            else if (plateCharacter == "ع" || plateCharacter == "E")
            {
                mappedCharacter = "6";
            }
            else if (plateCharacter == "ق" || plateCharacter == "G")
            {
                mappedCharacter = "7";
            }
            else if (plateCharacter == "ص" || plateCharacter == "X")
            {
                mappedCharacter = "8";
            }
            else if (plateCharacter == "ط" || plateCharacter == "T")
            {
                mappedCharacter = "9";
            }
            else if (plateCharacter == "ك" || plateCharacter == "K")
            {
                mappedCharacter = "10";
            }
            else if (plateCharacter == "م" || plateCharacter == "Z")
            {
                mappedCharacter = "11";
            }
            else if (plateCharacter == "ن" || plateCharacter == "N")
            {
                mappedCharacter = "12";
            }
            else if (plateCharacter == "ل" || plateCharacter == "L")
            {
                mappedCharacter = "13";
            }
            else if (plateCharacter == "ى" || plateCharacter == "ي" || plateCharacter == "V")
            {
                mappedCharacter = "14";
            }
            else if (plateCharacter == "س" || plateCharacter == "S")
            {
                mappedCharacter = "15";
            }
            else if (plateCharacter == "و" || plateCharacter == "U")
            {
                mappedCharacter = "16";
            }
            else if (plateCharacter == "ر" || plateCharacter == "R")
            {
                mappedCharacter = "17";
            }
            return mappedCharacter;
        }

        private bool IsValidPolicyResponseAutoleasing(AutoLeasingPolicyResponse policyResponse)
        {
            if (policyResponse == null) return false;
            if (policyResponse.Status.ToLower() != "true")
                return false;
            if (policyResponse.PolicyNumber == null)
                return false;

            return true;
        }

        public override ServiceOutput GetTawuniyaAutoleasingQuotation(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            ServiceOutput serviceOutput = new ServiceOutput();
            var output = GetAutoLeaseQuotationRequest(quotation, predefinedLogInfo);
            if (output.ErrorCode == QuotationOutput.ErrorCodes.Success)
            {
                serviceOutput.ErrorCode = ServiceOutput.ErrorCodes.Success;
                serviceOutput.Output = output.ServiceResponse;
                return serviceOutput;
            }
            else
            {
                serviceOutput.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                serviceOutput.ErrorDescription = output.ErrorDescription;
                return serviceOutput;
            }
        }

        private QuotationOutput GetAutoLeaseQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog log)
        {
            var configuration = Configuration as RestfulConfiguration;
            HttpResponseMessage message = new HttpResponseMessage();
            message.StatusCode = System.Net.HttpStatusCode.OK;
            QuotationOutput output = new QuotationOutput();
            log.ReferenceId = quotation.ReferenceId;
            log.ServiceURL = configuration.GenerateAutoleasingQuotationUrl;
            log.Channel = Channel.autoleasing.ToString();
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "AutoleasingQuotation";
            log.CompanyName = "Tawuniya";
            log.VehicleMaker = quotation?.VehicleMaker;
            log.VehicleMakerCode = quotation?.VehicleMakerCode;
            log.VehicleModel = quotation?.VehicleModel;
            log.VehicleModelCode = quotation?.VehicleModelCode;
            log.VehicleModelYear = quotation?.VehicleModelYear;
            try
            {
                if (_tameenkConfig.Quotatoin.TestMode)
                {
                    string nameOfFileJson = ".TestData.quotationTestData.json";
                    string tawuniyaResponse = ReadResource(GetType().Namespace, nameOfFileJson);
                    var handledResponse = JsonConvert.SerializeObject(HandleProposalResponse(tawuniyaResponse, quotation));
                    message.Content = new StringContent(handledResponse);
                    output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    return output;
                }

                AutoLeaseQuotationRequest request = HandleAutoLeaseQuotationRequest(quotation);
                string stringPayload = JsonConvert.SerializeObject(request);
                log.ServiceRequest = stringPayload;
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                //string authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes(configuration.AutoleasingAccessToken));
                DateTime dtBeforeCalling = DateTime.Now;

                HttpResponseMessage response = null;
                using (var httpClient = new HttpClient())
                {
                    var requestHeader = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(GET_QUOTATION_AutoLease_URL),
                        Content = new StringContent(stringPayload, Encoding.UTF8, "application/json")
                    };

                    requestHeader.Headers.Add("Accept", "application/json");
                    requestHeader.Headers.Add(TAWUNIYA_API_KEY, TAWUNIYA_API_ACCESS_KEY);
                    httpClient.DefaultRequestHeaders.ExpectContinue = false;

                    response = httpClient.SendAsync(requestHeader).Result;
                }

                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                if (response.Content == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.NullResponse;
                    //int _ErrorCode = (int)
                    output.ErrorDescription = "Service response content result return null and response.StatusCode is " + response.StatusCode + " response.Content.ReadAsStringAsync() " + response.Content.ReadAsStringAsync().Status.ToString() + " response.Content.ReadAsStringAsync().Result " + response.Content.ReadAsStringAsync().Result;
                    log.ServiceResponse = response.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                var value = response.Content.ReadAsStringAsync().Result;
                //QuotationServiceResponse _quotation = new QuotationServiceResponse();
                var tawuniyaModifiedResponse = HandleAutoLeaseQuotationResponse(value);
                var tawuniyaJson = JsonConvert.SerializeObject(tawuniyaModifiedResponse);
                message.Content = new StringContent(tawuniyaJson);
                log.ServiceResponse = value;

                if (tawuniyaModifiedResponse.StatusCode != 1)
                {
                    StringBuilder servcieErrors = new StringBuilder();
                    StringBuilder servcieErrorsCodes = new StringBuilder();

                    foreach (var error in tawuniyaModifiedResponse.Errors)
                    {
                        servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);
                        servcieErrorsCodes.AppendLine(error.Code);
                    }

                    output.ErrorCode = QuotationOutput.ErrorCodes.ServiceError;
                    string _ErrorDescription = "Quotation Service response error is : " + servcieErrors.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = servcieErrorsCodes.ToString();
                    log.ServiceErrorDescription = servcieErrors.ToString();
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                output.Output = message;
                output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }

        private AutoLeaseQuotationRequest HandleAutoLeaseQuotationRequest(QuotationServiceRequest quotation)
        {
            //get user benifits
            var shoppingCartItemBenefits = _shoppingCartItemRepository.TableNoTracking
                .Include(s => s.ShoppingCartItemBenefits)
                .FirstOrDefault(e => e.ReferenceId == quotation.ReferenceId)?.ShoppingCartItemBenefits;

            List<Product_Benefit> productBenefits = null;
            if (shoppingCartItemBenefits != null && shoppingCartItemBenefits.Any())
            {
                var shoppingCartItemBenefits_ProductBenefitId = shoppingCartItemBenefits.Select(s => s.ProductBenefitId);
                productBenefits = _productBenefitRepository.TableNoTracking
                .Where(e => shoppingCartItemBenefits_ProductBenefitId.Contains(e.Id)).ToList();
            }

            AutoLeaseQuotationRequest request = new AutoLeaseQuotationRequest();
            request.PolicyRequestReferenceNo = quotation.ReferenceId;
            request.RequestReferenceNo = quotation.ReferenceId;
            request.SourceCode = 757701;
            request.UserName = "BCA001";
            request.QuoteReferenceNo = int.Parse(quotation.QuotationNo);
            request.Details = new Details()
            {
                DeductibleAmount = quotation.DeductibleValue.ToString(),
                //DeductibleReferenceNo = null,
                PolicyPremiumFeatures = productBenefits != null ? productBenefits.Select(e => new PolicyPremiumFeatures()
                {
                    FeatureID = e.BenefitExternalId,
                    FeatureAmount = (e.BenefitPrice * 1.15M).Value.ToString("F0", CultureInfo.InvariantCulture),
                    FeatureTaxableAmount = (e.BenefitPrice * 1.15M).Value.ToString("F0", CultureInfo.InvariantCulture),
                    FeatureTypeID = e.BenefitPrice == 0 ? "2" : "1"

                }).ToList() : null
            };
            request.CustomizedParameter = null;
            return request;
        }

        private QuotationServiceResponse HandleAutoLeaseQuotationResponse(string value)
        {
            AutoLeaseQuotationResponse Tawniya_AutoLease_response = JsonConvert.DeserializeObject<AutoLeaseQuotationResponse>(value);
            QuotationServiceResponse response = new QuotationServiceResponse();
            response.Errors = Tawniya_AutoLease_response.Errors;
            response.StatusCode = Tawniya_AutoLease_response.Status == "True" ? 1 : 2;
            return response;
        }

        #endregion

        #region Private Methods

        public override ServiceOutput GetTawuniyaQuotation(QuotationServiceRequest quotation, Product product, string proposalNumber, ServiceRequestLog predefinedLogInfo, List<string> selectedBenefits)
        {
            ServiceOutput serviceOutput = new ServiceOutput();
            var output = GetQuotation(quotation, product, proposalNumber, predefinedLogInfo, selectedBenefits);
            if (output.ErrorCode == QuotationOutput.ErrorCodes.Success)
            {
                serviceOutput.ErrorCode = ServiceOutput.ErrorCodes.Success;
                serviceOutput.Output = output.ServiceResponse;
                return serviceOutput;
            }
            else
            {
                serviceOutput.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                serviceOutput.ErrorDescription = output.ErrorDescription;
                return serviceOutput;
            }
        }
        private QuotationOutput GetQuotation(QuotationServiceRequest quotation, Product product, string proposalNumber, ServiceRequestLog log, List<string> selectedbenefits)
        {
            //var quotationLog = new ServiceRequestLog
            //{
            //    CompanyID = log.CompanyID,
            //    CreatedDate = log.CreatedDate,
            //    InsuranceTypeCode = log.InsuranceTypeCode,
            //    RequestId = log.RequestId,
            //    ServiceURL = log.ServiceURL,
            //    UserID = log.UserID,
            //    UserName = log.UserName,
            //    VehicleId = log.VehicleId,
            //    DriverNin = log.DriverNin
            //};

            var configuration = Configuration as RestfulConfiguration;
            HttpResponseMessage message = new HttpResponseMessage();
            message.StatusCode = System.Net.HttpStatusCode.OK;
            QuotationOutput output = new QuotationOutput();
            log.ReferenceId = quotation.ReferenceId;
            log.ServiceURL = configuration.GenerateQuotationUrl;
            if (string.IsNullOrEmpty(log.Channel))
                log.Channel = "Portal";
            //log.Channel = "Portal";
            //log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Quotation";
            log.CompanyName = "Tawuniya";
            log.VehicleMaker = quotation?.VehicleMaker;
            log.VehicleMakerCode = quotation?.VehicleMakerCode;
            log.VehicleModel = quotation?.VehicleModel;
            log.VehicleModelCode = quotation?.VehicleModelCode;
            log.VehicleModelYear = quotation?.VehicleModelYear;

            //HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                if (_tameenkConfig.Quotatoin.TestMode)
                {
                    string nameOfFileJson = ".TestData.quotationTestData.json";
                    string tawuniyaResponse = ReadResource(GetType().Namespace, nameOfFileJson);
                    var handledResponse = JsonConvert.SerializeObject(HandleTawuniyaQuotationResponse(tawuniyaResponse));
                    message.Content = new StringContent(handledResponse);
                    output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    return output;
                }

                var request = CreateQuotationRequest(quotation, product, proposalNumber, selectedbenefits);
                string stringPayload = JsonConvert.SerializeObject(request);
                log.ServiceRequest = stringPayload;
                output.ServiceRequest = stringPayload;
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                string authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes(configuration.AccessToken));
                DateTime dtBeforeCalling = DateTime.Now;
                Utilities.InitiateSSLTrust();
                var response = _httpClient.Post(configuration.GenerateQuotationUrl, httpContent, authToken, null, "Basic");
                //postTask.Wait();
                //response = postTask.Content?.ReadAsStringAsync().Result;
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = output.ErrorCode.ToString();
                    log.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (response.Content == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content result return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var value = response.Content.ReadAsStringAsync().Result;
                log.ServiceResponse = value;
                output.ServiceResponse = value;
                var tawuniyaModifiedResponse = HandleTawuniyaQuotationResponse(value);
                var tawuniyaJson = JsonConvert.SerializeObject(tawuniyaModifiedResponse);
                message.Content = new StringContent(tawuniyaJson);

                //if (tawuniyaModifiedResponse.StatusCode == 1)
                //{
                //    UpdateProductQuotationNo(tawuniyaModifiedResponse.QuotationNo, policy.ProductInternalId);
                //    policy.QuotationNo = tawuniyaModifiedResponse.QuotationNo;
                //}
                if (tawuniyaModifiedResponse.StatusCode != 1)
                {
                    StringBuilder servcieErrors = new StringBuilder();
                    StringBuilder servcieErrorsCodes = new StringBuilder();

                    foreach (var error in tawuniyaModifiedResponse.Errors)
                    {
                        servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);
                        servcieErrorsCodes.AppendLine(error.Code);
                    }
                    output.ErrorCode = QuotationOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Quotation Service response error is : " + servcieErrors.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = servcieErrorsCodes.ToString();
                    log.ServiceErrorDescription = servcieErrors.ToString();
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                output.Output = message;
                output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                //log.ServiceResponse = response.Content.ReadAsStringAsync().Result;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }

        private QuotationOutput GetTawuniyaProposal(QuotationServiceRequest quotation, ServiceRequestLog log)
        {
            var configuration = Configuration as RestfulConfiguration;
            HttpResponseMessage message = new HttpResponseMessage();
            message.StatusCode = System.Net.HttpStatusCode.OK;
            QuotationOutput output = new QuotationOutput();
            log.ReferenceId = quotation.ReferenceId;
            log.ServiceURL = GET_PROPOSAL_URL;
            if (string.IsNullOrEmpty(log.Channel))
                log.Channel = "Portal";
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Proposal";
            log.CompanyName = "Tawuniya";
            log.VehicleMaker = quotation?.VehicleMaker;
            log.VehicleMakerCode = quotation?.VehicleMakerCode;
            log.VehicleModel = quotation?.VehicleModel;
            log.VehicleModelCode = quotation?.VehicleModelCode;
            log.VehicleModelYear = quotation?.VehicleModelYear;
            try
            {
                if (_tameenkConfig.Quotatoin.TestMode)
                {
                    string nameOfFileJson = ".TestData.quotationTestData.json";
                    string tawuniyaResponse = ReadResource(GetType().Namespace, nameOfFileJson);
                    var handledResponse = JsonConvert.SerializeObject(HandleProposalResponse(tawuniyaResponse, quotation));
                    message.Content = new StringContent(handledResponse);
                    output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    return output;
                }

                var request = CreateProposalRequest(quotation);
                string stringPayload = JsonConvert.SerializeObject(request);
                log.ServiceRequest = stringPayload;
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                string authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes(configuration.AccessToken));
                DateTime dtBeforeCalling = DateTime.Now;
                Utilities.InitiateSSLTrust();
                var response = _httpClient.PostAsync(GET_PROPOSAL_URL, httpContent, authToken, null, "Basic").Result;
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                if (response.Content == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content result return null and response.StatusCode is "+ response.StatusCode + " response.Content.ReadAsStringAsync() "+ response.Content.ReadAsStringAsync().Status.ToString()+ " response.Content.ReadAsStringAsync().Result " + response.Content.ReadAsStringAsync().Result;
                    log.ServiceResponse = response.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
               
                var value = response.Content.ReadAsStringAsync().Result;
                log.ServiceResponse = value;
                var tawuniyaModifiedResponse = HandleProposalResponse(value, quotation);
                var tawuniyaJson = JsonConvert.SerializeObject(tawuniyaModifiedResponse);
                message.Content = new StringContent(tawuniyaJson);
             

                if (tawuniyaModifiedResponse.StatusCode != 1)
                {
                    StringBuilder servcieErrors = new StringBuilder();
                    StringBuilder servcieErrorsCodes = new StringBuilder();

                    foreach (var error in tawuniyaModifiedResponse.Errors)
                    {
                        servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);
                        servcieErrorsCodes.AppendLine(error.Code);
                    }
                    output.ErrorCode = QuotationOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Quotation Service response error is : " + servcieErrors.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = servcieErrorsCodes.ToString();
                    log.ServiceErrorDescription = servcieErrors.ToString();
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                output.Output = message;
                output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                //log.ServiceResponse = response.Content.ReadAsStringAsync().Result;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (TimeoutException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }

        private QuotationRequestDto CreateQuotationRequest(QuotationServiceRequest quotation, Product product, string proposalNumber, List<string> selectedbenefits)        {            var request = new QuotationRequestDto();
            #region Header            request.CreateQuoteRequest.Header.InitiatedDateTime = DateTime.Now.ToString();            request.CreateQuoteRequest.Header.MesssageType = "createQuote";            request.CreateQuoteRequest.Header.TrackingId = quotation.ReferenceId;            request.CreateQuoteRequest.Header.RoutingIdentifier = "TAWN";            request.CreateQuoteRequest.Header.Sender = "TAWN";            request.CreateQuoteRequest.Header.Version = "1";
            #endregion
            #region QuotationInfo            request.CreateQuoteRequest.QuotationRequest.QuotationInfo = new QuotationInfo
                                              {
                                                  ///ProductCode = tawuniyaProposal.ProposalTypeCode == 1 ? "MotorImtiazeTP" : tawuniyaProposal.ProposalTypeCode == 2 ? "PRMotorImtiazeCO" : "PRMotorImtiazeSP",                ProductCode = product.InsuranceTypeCode == 1 ? "MotorImtiazeTP" : product.InsuranceTypeCode == 2 ? "PRMotorImtiazeCO" : "PRMotorImtiazeSP",
                                                  IdNumber = quotation.InsuredId.ToString(),
                                                  LanguageCode = GetCurrentLanguageCode(),
                                                  RequiredInceptionDate = quotation.PolicyEffectiveDate.ToString("yyyy-MM-dd", new CultureInfo("en-US")),
                                                  SpecialSchemeCode = string.IsNullOrEmpty(quotation.PromoCode) ? "AGGREGATOR" : quotation.PromoCode,
                                                  ProposalNumber = proposalNumber
                                              };
            #endregion
            #region ChannelDetails            request.CreateQuoteRequest.QuotationRequest.ChannelDetails = new List<ChannelDetail> {                new ChannelDetail                {                    ConsumerApplicationTypeReference ="TMNK",                    UserName =/*"FRS2050002"*/"FRS2050002",                    SourceCode ="755927",                    ChannelCode ="205"                }            };
            #endregion
            #region customerDetails            var customerDetail = new CustomerDetail
                                                {
                                                    FullNameInEnglish = $"{quotation.InsuredFirstNameEn} {quotation.InsuredMiddleNameEn} {quotation.InsuredLastNameEn}",
                                                    FirstNameEnglish = quotation.InsuredFirstNameEn,
                                                    MidNameEnglish = quotation.InsuredMiddleNameEn,
                                                    LastNameEnglish = quotation.InsuredLastNameEn,
                                                    FullNameInArabic = $"{quotation.InsuredFirstNameAr} {quotation.InsuredMiddleNameAr} {quotation.InsuredLastNameAr}",
                                                    FirstNameArabic = quotation.InsuredFirstNameAr,
                                                    MidNameArabic = quotation.InsuredMiddleNameAr,
                                                    LastNameArabic = quotation.InsuredLastNameAr,
                                                    NationalityCode = quotation.InsuredNationalityCode,
                                                    AddressCity = quotation.InsuredCity,
                                                    Occupation = quotation.InsuredOccupationCode,
                                                    MaritalStatus = quotation.InsuredSocialStatusCode,
                                                    Gender = quotation.InsuredGenderCode == "M" ? "1" : "2",
                                                    InsuredEducationCode = quotation.InsuredEducationCode?.ToString()
                                                };            if (!string.IsNullOrEmpty(quotation.InsuredBirthDateG))            {                var birthDateG = quotation.InsuredBirthDateG.Split('-');                customerDetail.DobGreg = $"{birthDateG[2]}-{birthDateG[1]}-{birthDateG[0]}";            }            if (quotation.InsuredIdTypeCode == 1)            {                if (!string.IsNullOrEmpty(quotation.InsuredBirthDateH))                {                    var birthDateH = quotation.InsuredBirthDateH.Split('-');                    customerDetail.DobHijri = $"{birthDateH[2]}-{birthDateH[1]}-{birthDateH[0]}";                }            }            request.CreateQuoteRequest.QuotationRequest.CustomerDetails = new List<CustomerDetail>            {                customerDetail            };
            #endregion
            #region vehicleDetails            string registrationExpiryDate = null;            if (!string.IsNullOrWhiteSpace(quotation.VehicleRegExpiryDate))            {                var registrationExpiryDateParts = quotation.VehicleRegExpiryDate.Split('-');                registrationExpiryDate = $"{registrationExpiryDateParts[2]}-{registrationExpiryDateParts[1]}-{registrationExpiryDateParts[0]}";            }            bool isVehicleRegistered = quotation.VehicleIdTypeCode == 1 ? true : false;            var vehicleDetail = new VehicleDetail            {                //PlateType = !string.IsNullOrWhiteSpace(quotation.VehiclePlateTypeCode) ? quotation.VehiclePlateTypeCode : "11",                MakeCode = quotation.VehicleMakerCode,                ModelCode = quotation.VehicleModelCode,                PlateNumber = quotation.VehiclePlateNumber.ToString(),                PlateText1 = quotation.VehiclePlateText1,                PlateText2 = quotation.VehiclePlateText2,                PlateText3 = quotation.VehiclePlateText3,                YearOfManufacture = quotation.VehicleModelYear.ToString(),                ChassisNumber = quotation.VehicleChassisNumber,                RegistrationExpiryDate = registrationExpiryDate,                Color = quotation.VehicleMajorColorCode,                RegistrationOfficeCity = quotation.VehicleRegPlace,                MajorDrivingCity = quotation.InsuredCity,                NCDYears = quotation.NCDFreeYears.ToString(),                InsurancePeriod = "1",                AgencyRepairRequired = quotation.VehicleAgencyRepair.Value ? "Y" : "N",                UsageType = "PRIVATE",                Deductible = quotation.DeductibleValue.HasValue ? quotation.DeductibleValue.Value.ToString() : "",                VehicleValue = quotation.VehicleValue.HasValue ? quotation.VehicleValue.Value.ToString() : ""            };            if (!string.IsNullOrEmpty(quotation.VehiclePlateTypeCode))
                vehicleDetail.PlateType = (quotation.VehiclePlateTypeCode == "10") ? "13" : quotation.VehiclePlateTypeCode;
            else
                vehicleDetail.PlateType = "11";            if (isVehicleRegistered)            {                vehicleDetail.SerialNumber = quotation.VehicleId.ToString();            }            else            {                vehicleDetail.CustomCardNumber = quotation.VehicleId.ToString();            }            if (quotation.VehicleOwnerTransfer)            {                vehicleDetail.VehicleTransferFlag = "Y";                vehicleDetail.TrassferCaseOwnerId = quotation.VehicleOwnerId.ToString();            }            else                vehicleDetail.VehicleTransferFlag = "N";
            #region VehicleMoreInfo            vehicleDetail.VehicleMoreInfo = new List<VehicleMoreInfo>()            {                new VehicleMoreInfo                {                    AnnualMileage=quotation.VehicleMileageExpectedAnnualCode?.ToString(),                    AxleWeightCode = quotation.VehicleAxleWeightCode.ToString(),                    EngineSizeCode=quotation.VehicleEngineSizeCode?.ToString(),                    Mileage = quotation.VehicleMileage?.ToString(),                    OverNightParkCode=quotation.VehicleOvernightParkingLocationCode?.ToString()                }            };
            #endregion
            #region VehicleDriverDetails            if (quotation.Drivers != null)            {                vehicleDetail.VehicleDriverDetails = new List<VehicleDriverDetail>();                foreach (var driver in quotation.Drivers)                {                    var diverBirthDate = driver.DriverBirthDate.Split('-');                    var vehicleDriver = new VehicleDriverDetail                    {                        IdNo = driver.DriverId.ToString(),                        EName = $"{driver.DriverFirstNameEn} {driver.DriverMiddleNameEn} {driver.DriverLastNameEn}",                        EFirstName = driver.DriverFirstNameEn,                        EMiddleName = driver.DriverMiddleNameEn,                        ELastName = driver.DriverLastNameEn,                        AName = $"{driver.DriverFirstNameAr} {driver.DriverMiddleNameAr} {driver.DriverLastNameAr}",                        AFirstName = driver.DriverFirstNameAr,                        AMiddleName = driver.DriverMiddleNameAr,                        ALastName = driver.DriverLastNameAr,                        Gender = driver.DriverGenderCode == "M" ? "1" : "2",                        DriverType = driver.DriverTypeCode == 1 ? "MD" : "AD",                        NationalityCode = driver.DriverNationalityCode,                        SocialStatusCode = driver.DriverSocialStatusCode,                        EducationCode = driver.DriverEducationCode?.ToString(),                        MedicalConditionCode = driver.DriverMedicalConditionCode?.ToString(),                        ChildrenBelow16Years = driver.DriverChildrenBelow16Years?.ToString(),                        NCD = driver.DriverNCDFreeYears?.ToString(),                        Relation = "PH"                    };                    if (driver.DriverIdTypeCode == 1)                    {                        vehicleDriver.DobHijri = $"{diverBirthDate[2]}-{diverBirthDate[1]}-{diverBirthDate[0]}";                    }                    else                    {                        vehicleDriver.DobGreg = driver.DriverBirthDateG.ToString("yyyy-MM-dd", new CultureInfo("en-US"));//$"{diverBirthDate[2]}-{diverBirthDate[1]}-{diverBirthDate[0]}";
                    }                    vehicleDriver.HomeAddress = new HomeAddress                    {                        HomeCityName = driver.DriverHomeCity                    };                    vehicleDetail.VehicleDriverDetails.Add(vehicleDriver);                }            }








            #endregion
            #region Selected Benefits

            if (selectedbenefits != null && selectedbenefits.Count > 0)
            {
                vehicleDetail.SelectedFeatures = new SelectedFeatures();
                vehicleDetail.SelectedFeatures.Features = new List<Feature>();
                foreach (var item in selectedbenefits)
                {
                    vehicleDetail.SelectedFeatures.Features.Add(new Feature() { FeatureCode = item });
                }
            }

            #endregion
            request.CreateQuoteRequest.QuotationRequest.VehicleDetails = new List<VehicleDetail>            {                vehicleDetail,            };
            #endregion            return request;        }

        private ProposalsRequestDto CreateProposalRequest(QuotationServiceRequest quotation)
        {
            var request = new ProposalsRequestDto
            {
                GetProposalsRequest = new GetProposalsRequest()
            };

            #region Header

            request.GetProposalsRequest.Header = new ProposalHeader()
            {
                InitiatedDateTime = DateTime.UtcNow.ToString(),
                MesssageType = "GetProposal",
                Sender = "TAWN",
                TrackingId = quotation.ReferenceId
            };
            #endregion

            #region ProposalRequest

            request.GetProposalsRequest.ProposalRequest = new ProposalRequest
            {
                ChannelDetails = new List<ProposalChannelDetail>()
            };

            #region ChannelDetails


            request.GetProposalsRequest.ProposalRequest.ChannelDetails.Add(
                new ProposalChannelDetail
                {
                    ChannelCode = "205",
                    ConsumerApplicationTypeReference = "TMNK",
                    SourceCode = "755927",
                    UserName = "FRS2050002"

                });

            #endregion

            #region ProposalInfo


            request.GetProposalsRequest.ProposalRequest.ProposalInfo = new ProposalInfo
            {
                IdNumber = quotation.InsuredId.ToString(),
                ProductCode = "ALL" /*quotation.ProductTypeCode == 1 ? "ALL" : "MotorImtiazeTP"*/,
                RequiredInceptionDate = quotation.PolicyEffectiveDate.ToString("yyyy-MM-dd", new CultureInfo("en-US")),
                SpecialSchemeCode = string.IsNullOrEmpty(quotation.PromoCode) ? "AGGREGATOR" : quotation.PromoCode
            };

            #endregion

            #region CustomerDetails

            var customer = new ProposalCustomerDetail
            {
                FullNameInEnglish = $"{quotation.InsuredFirstNameEn} {quotation.InsuredMiddleNameEn} {quotation.InsuredLastNameEn}",
                FirstNameEnglish = quotation.InsuredFirstNameEn,
                MidNameEnglish = quotation.InsuredMiddleNameEn,
                LastNameEnglish = quotation.InsuredLastNameEn,
                FullNameInArabic = $"{quotation.InsuredFirstNameAr} {quotation.InsuredMiddleNameAr} {quotation.InsuredLastNameAr}",
                FirstNameArabic = quotation.InsuredFirstNameAr,
                MidNameArabic = quotation.InsuredMiddleNameAr,
                LastNameArabic = quotation.InsuredLastNameAr,
                NationalityCode = quotation.InsuredNationalityCode,
                AddressCity = quotation.InsuredCity,
                Gender = quotation.InsuredGenderCode == "M" ? "1" : "2"
            };

            if (!string.IsNullOrEmpty(quotation.InsuredBirthDateG))
            {
                var birthDateG = quotation.InsuredBirthDateG.Split('-');
                customer.DobGreg = $"{birthDateG[2]}-{birthDateG[1]}-{birthDateG[0]}";
            }

            if (quotation.InsuredIdTypeCode == 1)
            {
                if (!string.IsNullOrEmpty(quotation.InsuredBirthDateH))
                {
                    var birthDateH = quotation.InsuredBirthDateH.Split('-');
                    customer.DobHijri = $"{birthDateH[2]}-{birthDateH[1]}-{birthDateH[0]}";
                }
            }
            request.GetProposalsRequest.ProposalRequest.CustomerDetails = new List<ProposalCustomerDetail>
            {
                customer
            };

            #endregion

            #region VehicleDetails

            string registrationExpiryDate = null;
            if (!string.IsNullOrWhiteSpace(quotation.VehicleRegExpiryDate))
            {
                var registrationExpiryDateParts = quotation.VehicleRegExpiryDate.Split('-');
                registrationExpiryDate = $"{registrationExpiryDateParts[2]}-{registrationExpiryDateParts[1]}-{registrationExpiryDateParts[0]}";
            }
            bool isVehicleRegistered = quotation.VehicleIdTypeCode == 1 ? true : false;
            var vehicleDetail = new ProposalVehicleDetail
            {
                //PlateType = !string.IsNullOrWhiteSpace(quotation.VehiclePlateTypeCode) ? quotation.VehiclePlateTypeCode : "10",
                MakeCode = quotation.VehicleMakerCode,
                ModelCode = quotation.VehicleModelCode,
                PlateNumber = quotation.VehiclePlateNumber.ToString(),
                PlateText1 = quotation.VehiclePlateText1,
                PlateText2 = quotation.VehiclePlateText2,
                PlateText3 = quotation.VehiclePlateText3,
                YearOfManufacture = quotation.VehicleModelYear.ToString(),
                ChassisNumber = quotation.VehicleChassisNumber,
                VehicleValue = quotation.VehicleValue?.ToString(),
                RegistrationExpiryDate = registrationExpiryDate,
                Color = quotation.VehicleMajorColorCode,
                RegistrationOfficeCity = quotation.VehicleRegPlace,
                MajorDrivingCity = quotation.InsuredCity,
                NCDYears = quotation.NCDFreeYears.ToString(),
                InsurancePeriod = "1",
                AgencyRepairRequired = quotation.VehicleAgencyRepair.Value? "Y": "N",
                UsageType = "PRIVATE"
            };

            if (!string.IsNullOrEmpty(quotation.VehiclePlateTypeCode))
                vehicleDetail.PlateType = (quotation.VehiclePlateTypeCode == "10") ? "13" : quotation.VehiclePlateTypeCode;
            else
                vehicleDetail.PlateType = "11";

            if (isVehicleRegistered)
            {
                vehicleDetail.SerialNumber = quotation.VehicleId.ToString();
            }
            else
            {
                vehicleDetail.CustomCardNumber = quotation.VehicleId.ToString();
            }

            if (quotation.VehicleOwnerTransfer)
            {
                vehicleDetail.VehicleTransferFlag = "Y";
                vehicleDetail.TrassferCaseOwnerId = quotation.VehicleOwnerId.ToString();
            }
            else
                vehicleDetail.VehicleTransferFlag = "N";

            if (quotation.ProductTypeCode == 2 && quotation.DeductibleValue.HasValue)
            {
                vehicleDetail.Deductible = quotation.DeductibleValue.Value.ToString();
            }

            #region VehicleDriverDetails

            if (quotation.Drivers != null)
            {
                vehicleDetail.VehicleDriverDetails = new List<ProposalVehicleDriverDetail>();
                foreach (var driver in quotation.Drivers)
                {
                    var diverBirthDate = driver.DriverBirthDate.Split('-');
                    var vehicleDriver = new ProposalVehicleDriverDetail
                    {
                        IdNo = driver.DriverId.ToString(),
                        Gender = driver.DriverGenderCode == "M" ? "1" : "2",
                        DriverType = driver.DriverTypeCode == 1 ? "MD" : "AD",
                        NationalityCode = driver.DriverNationalityCode,
                        NCD = driver.DriverNCDFreeYears?.ToString(),
                        Relation = "PH"
                    };
                    if (driver.DriverIdTypeCode == 1 && diverBirthDate.Count() == 3)//06-03-1422
                    {
                        vehicleDriver.DobHijri = $"{diverBirthDate[2]}-{diverBirthDate[1]}-{diverBirthDate[0]}";
                    }
                    else
                    {
                        vehicleDriver.DobGreg = driver.DriverBirthDateG.ToString("yyyy-MM-dd", new CultureInfo("en-US"));//$"{diverBirthDate[2]}-{diverBirthDate[1]}-{diverBirthDate[0]}";
                    }

                    vehicleDriver.HomeAddress = new ProposalHomeAddress
                    {
                        HomeCityName = driver.DriverHomeCity
                    };
                    vehicleDetail.VehicleDriverDetails.Add(vehicleDriver);
                }

            }

            #endregion

            request.GetProposalsRequest.ProposalRequest.VehicleDetails = new List<ProposalVehicleDetail>
            {
                vehicleDetail
            };


            #endregion

            #endregion

            return request;
        }

        private PolicyRequestDto CreatePolicyRequest(PolicyRequest policy)
        {
            var request = new PolicyRequestDto();
            request.CreatePolicyRequest = new CreatePolicyRequest();

            #region Header
            request.CreatePolicyRequest.Header = new PolicyRequestHeader
            {
                InitiatedDateTime = DateTime.Now.ToString(),
                MesssageType = "createPolicy",
                TrackingId = policy.ReferenceId,
                RoutingIdentifier = "TAWN",
                Version = "1",
                Sender = "TAWN"
            };
            #endregion

            #region PolicyInfo
            request.CreatePolicyRequest.PolicyRequest = new TawuniyaPolicyRequest();
            request.CreatePolicyRequest.PolicyRequest.PolicyInfo = new PolicyInfo
            {
                QuotationNumber = policy.QuotationNo,
                IdNumber = policy.InsuredId.ToString(),
                LanguageCode = (!string.IsNullOrEmpty(policy.Language) && policy.Language.ToLower() == "en") ? "1" : "2", // GetCurrentLanguageCode(),
                PolicyInceptionDate = policy.PolicyEffectiveDate.Value.ToString("yyyy-MM-dd", new CultureInfo("en-US"))
            };
            #endregion

            #region ChannelDetails
            request.CreatePolicyRequest.PolicyRequest.ChannelDetails = new List<PolicyChannelDetail> {
                new PolicyChannelDetail
                {
                    //ApplicationType ="TMNK",//Production
                    ConsumerApplicationTypeReference ="TMNK",// UAT
                    //UserName ="FRS2050002", // Production
                    UserName = "FRS2050002", // UAT
                    SourceCode ="755927",
                    ChannelCode ="205"
                }
            };
            #endregion

            #region PaymentDetails

            request.CreatePolicyRequest.PolicyRequest.PaymentDetails = new List<PaymentDetail>
            {
                new PaymentDetail
                {
                    PaymentAmount = policy.PaymentAmount.ToString(),
                    BillNumber =policy.PaymentBillNumber,
                    PaymentDate = DateTime.Now.ToString("yyyy-MM-dd")
                }
            };

            #endregion

            #region CustomerDetails
            request.CreatePolicyRequest.PolicyRequest.CustomerDetails = new CustomerDetails
            {
                Email = policy.InsuredEmail,
                Mobile = policy.InsuredMobileNumber,
                AdditionalNumber = policy.InsuredAdditionalNumber.ToString(),
                BuildingNo = policy.InsuredBuildingNo.ToString(),
                DistrictName = policy.InsuredDistrict,
                StreetName = policy.InsuredStreet,
                UnitNumber = policy.InsuredUnitNo.ToString(),
                Zipcode = policy.InsuredZipCode.ToString()
            };
            #endregion

            return request;
        }

        protected override QuotationServiceResponse GetTawuniyaQuotationResponseObject(object response, QuotationServiceRequest request)        {            var result = response.ToString();            QuotationServiceResponse quotationServiceResponse = new QuotationServiceResponse();

            var quotationResponseDto = JsonConvert.DeserializeObject<QuotationResponseDto>(result);            if (quotationResponseDto != null)            {                if (quotationResponseDto.CreateQuoteResponse?.QuotationResponse?.QuotationInfo?.PaymentAmount == null)                {                    quotationServiceResponse.Errors = new List<Error>()                    {                        new Error {Message = result }                    };                }                else                {                    quotationServiceResponse.QuotationNo = quotationResponseDto.CreateQuoteResponse?.QuotationResponse?.QuotationInfo?.QuotationNumber;                    quotationServiceResponse.Products = new List<ProductDto>();                    var products = GetProductDtosFromQuotationResponse(quotationResponseDto);                    quotationServiceResponse.Products.AddRange(products);                }            }            return quotationServiceResponse;        }
        private QuotationServiceResponse HandleTawuniyaQuotationResponse(string value)
        {
            var quotationResponse = new QuotationServiceResponse();
            var result = JsonConvert.DeserializeObject<QuotationResponseDto>(value);
            if (IsValidQuotationResponse(result))
            {
                quotationResponse.StatusCode = 1;
                quotationResponse.QuotationNo = result.CreateQuoteResponse.QuotationResponse.QuotationInfo.QuotationNumber;
                quotationResponse.Products = GetProductDtosFromQuotationResponse(result);
            }
            else
            {
                quotationResponse.StatusCode = 2;
                quotationResponse.Errors = new List<Error>
                    {
                        new Error{Message = value}
                    };

                if (result.CreateQuoteResponse != null && result.CreateQuoteResponse.Errors != null)
                {
                    quotationResponse.Errors.AddRange(result.CreateQuoteResponse.Errors
                        .Select(x => new Error { Code = x.ErrorCode, Field = x.ErrorType, Message = x.ErrorDescription }));
                }

            }

            return quotationResponse;
        }

        private QuotationServiceResponse HandleProposalResponse(string value, QuotationServiceRequest quotation)
        {
            var quotationResponse = new QuotationServiceResponse();
            var result = JsonConvert.DeserializeObject<ProposalsResponseDto>(value);

            //in success we save all the products in the db
            if (IsValidProposalResponse(result))
            {
                quotationResponse.StatusCode = 1;
                quotationResponse.QuotationNo = "CheckProductDetail";
                quotationResponse.Products = GetProductsFromProposalResponse(result, quotation);
            }
            else
            {
                quotationResponse.StatusCode = 2;
                quotationResponse.Errors = new List<Error>
                    {
                        new Error{Message = value}
                    };

                if (result.GetProposalsResponse?.Errors != null)
                {
                    quotationResponse.Errors.AddRange(result.GetProposalsResponse.Errors
                        .Select(x => new Error { Code = x.ErrorCode, Field = x.ErrorType, Message = x.ErrorDescription }));
                }

            }

            return quotationResponse;
        }

        private bool IsValidQuotationResponse(QuotationResponseDto result)
        {
            if (result?.CreateQuoteResponse?.Errors != null)
                return false;

            if (result?.CreateQuoteResponse?.QuotationResponse?.QuotationResult != null)
            {
                if (result.CreateQuoteResponse.QuotationResponse.QuotationResult.ResultCode == "S")
                {
                    if (result.CreateQuoteResponse.QuotationResponse.QuotationInfo != null)
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        private bool IsValidProposalResponse(ProposalsResponseDto result)
        {
            if (result?.GetProposalsResponse?.Errors != null)
                return false;

            if (result?.GetProposalsResponse?.ProposalResponse?.ProposalResult != null)
            {
                if (result.GetProposalsResponse.ProposalResponse.ProposalResult.ResultCode == "S")
                {
                    if (result.GetProposalsResponse.ProposalResponse.ProposalInfo != null)
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        private PolicyResponse HandleTawuniyaPolicyResponse(string response)
        {
            var policyResponse = new PolicyResponse();
            var result = JsonConvert.DeserializeObject<PolicyResponseDto>(response);
            if (IsValidPolicyResponse(result))
            {
                policyResponse.StatusCode = 1;
                policyResponse.PolicyNo = result.CreatePolicyResponse.PolicyResponse.PolicyInfo.PolicyNumber;
                policyResponse.PolicyFileUrl = result.CreatePolicyResponse.PolicyResponse.PolicyInfo.PolicyDocPrintingLink;
                policyResponse.PolicyExpiryDate = DateTime.ParseExact(result.CreatePolicyResponse.PolicyResponse.PolicyInfo.PolicyExpiryDate, "yyyy-MM-dd", new CultureInfo("en-US"));
                policyResponse.PolicyIssuanceDate = DateTime.ParseExact(result.CreatePolicyResponse.PolicyResponse.PolicyInfo.PolicyInceptionDate, "yyyy-MM-dd", new CultureInfo("en-US"));
                policyResponse.PolicyEffectiveDate = DateTime.ParseExact(result.CreatePolicyResponse.PolicyResponse.PolicyInfo.PolicyInceptionDate, "yyyy-MM-dd", new CultureInfo("en-US"));
            }
            else
            {
                policyResponse.StatusCode = 2;
                policyResponse.Errors = new List<Error>
                    {
                        new Error{Message = response}
                    };

                if (result?.CreatePolicyResponse?.Errors != null && result?.CreatePolicyResponse?.Errors.Count > 0)
                {
                    policyResponse.Errors.AddRange(result.CreatePolicyResponse.Errors
                        .Select(x => new Error { Code = x.ErrorCode, Field = x.ErrorType, Message = x.ErrorDescription }));
                }
            }
            return policyResponse;
        }

        private bool IsValidPolicyResponse(PolicyResponseDto policyResponse)
        {
            if (policyResponse == null) return false;
            if (policyResponse.CreatePolicyResponse?.PolicyResponse?.PolicyResult?.ResultCode != "Success")
                return false;
            if (policyResponse.CreatePolicyResponse?.PolicyResponse?.PolicyInfo == null)
                return false;

            return true;
        }


        /// <summary>
        /// Return product list if the response has products
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private List<ProductDto> GetProductDtosFromQuotationResponse(QuotationResponseDto response)
        {
            if (response != null && response.CreateQuoteResponse != null && response.CreateQuoteResponse.QuotationResponse != null && response.CreateQuoteResponse.QuotationResponse.QuotationInfo != null)
            {
                List<ProductDto> productDtos = new List<ProductDto>();

                var product = new ProductDto();
                product.ProductPrice = response.CreateQuoteResponse.QuotationResponse.QuotationInfo.PaymentAmount;
                product.PriceDetails = GetProductPriceDetails(response.CreateQuoteResponse.QuotationResponse.QuotationInfo);
                if (response.CreateQuoteResponse.QuotationResponse.QuotationInfo.PromoAmount.HasValue
                    && response.CreateQuoteResponse.QuotationResponse.QuotationInfo.PromoPercentage.HasValue)
                {
                    product.PriceDetails.Add(new PriceDto
                    {
                        PercentageValue = response.CreateQuoteResponse.QuotationResponse.QuotationInfo.PromoPercentage.Value,
                        PriceValue = response.CreateQuoteResponse.QuotationResponse.QuotationInfo.PromoAmount.Value,
                        PriceTypeCode = 1 // Driver Safe Discount
                    });
                }

                //product.Benefits = GetProductBenefits(response.CreateQuoteResponse.QuotationResponse.QuotationInfo);
                productDtos.Add(product);
                return productDtos;
            }
            return null;
        }

        private List<ProductDto> GetProductsFromProposalResponse(ProposalsResponseDto proposals, QuotationServiceRequest quotation)
        {
            List<ProductDto> productDtos = new List<ProductDto>();

            if (proposals?.GetProposalsResponse?.ProposalResponse?.ProposalInfo != null)
            {
                //save tpl and Sanad plus as products for the same response
                IEnumerable<ProposalResponseInfo> proposalInfo = null;
                //if (quotation.ProductTypeCode == 1)
                //{
                //    proposalInfo = proposals.GetProposalsResponse.ProposalResponse.ProposalInfo.Where(e => e.ProductCode == TPL_CODE || e.ProductCode == SANAD_PLUS_CODE);
                //}
                //else
                //{
                //    //proposalInfo = proposals.GetProposalsResponse.ProposalResponse.ProposalInfo.Where(e => e.ProductCode == COMPREHENSIVE_CODE && e.Deductible.Value == quotation.DeductibleValue);
                //    proposalInfo = proposals.GetProposalsResponse.ProposalResponse.ProposalInfo.Where(e => e.ProductCode == COMPREHENSIVE_CODE);
                //}

                proposalInfo = proposals.GetProposalsResponse.ProposalResponse.ProposalInfo.Where(e => e.ProductCode == TPL_CODE || e.ProductCode == SANAD_PLUS_CODE || e.ProductCode == COMPREHENSIVE_CODE);

                if (proposalInfo != null)
                {
                    foreach (var item in proposalInfo)
                    {
                        var product = new ProductDto();
                        product.ProductPrice = item.PaymentAmount;
                        product.DeductibleValue = item.Deductible;
                        product.PriceDetails = GetProductPriceDetails(item);
                        if (item.PromoAmount.HasValue && item.PromoPercentage.HasValue)
                        {
                            product.PriceDetails.Add(new PriceDto
                            {
                                PercentageValue = item.PromoPercentage.Value,
                                PriceValue = item.PromoAmount.Value,
                                PriceTypeCode = 1 // Driver Safe Discount
                            });
                        }
                        bool isSandPlus = false;
                        if (item.ProductCode == SANAD_PLUS_CODE)
                        {
                            isSandPlus = true;
                        }
                        //product.Benefits = GetProductBenefits(proposals.GetProposalsResponse.ProposalResponse, isSandPlus);
                        product.Benefits = GetProductBenefitsNew(item.VehicleArray?.FirstOrDefault()?.ApplicableFeatureGroup, isSandPlus);
                        //product.InsuranceTypeCode = item.ProductCode == SANAD_PLUS_CODE ? 7 : 1;
                        product.InsuranceTypeCode = item.ProductCode == COMPREHENSIVE_CODE ? 2 : (item.ProductCode == SANAD_PLUS_CODE ? 7 : 1);
                        product.ProductId = item.ProposalNumber;
                        if (item.ProductCode == COMPREHENSIVE_CODE && item.VehicleLimitValue.HasValue)
                        {
                            int limitValue = 0;
                            int.TryParse(item.VehicleLimitValue.ToString(), out limitValue);

                            product.VehicleLimitValue = limitValue;
                        }
                        productDtos.Add(product);
                        int proposalTypeCode = 0;
                        if (item.ProductCode == TPL_CODE)
                        {
                            proposalTypeCode = 1;
                        }
                        else if (item.ProductCode == COMPREHENSIVE_CODE)
                        {
                            proposalTypeCode = 2;
                        }
                        else
                        {
                            proposalTypeCode = 7;
                        }
                        var tawuniyaProposal = new QuotationEntity.TawuniyaProposal()
                        {
                            ProposalNumber = item.ProposalNumber,
                            ReferenceId = quotation.ReferenceId,
                            ProposalTypeCode = proposalTypeCode
                        };
                        _tawuniyaProposalRepository.Insert(tawuniyaProposal);

                        if (product.Benefits != null && product.Benefits.Count() > 0)
                        {
                            if (product != null && product.Benefits != null)
                            {
                                foreach (var benefit in product.Benefits)
                                {
                                    if (benefit.BenefitPrice <= 1)
                                    {
                                        benefit.IsSelected = true;
                                    }
                                }
                                if (product.Benefits != null && product.Benefits.Count > 0)
                                {
                                    product.Benefits = product.Benefits.OrderBy(a => a.BenefitPrice).ToList();
                                }
                            }
                        }
                    }
                }
            }

            return productDtos;
        }
        


        private List<BenefitDto> GetProductBenefits(ProposalResponse proposalResponse,bool isSandPlus)
        {
            List<BenefitDto> benefits = new List<BenefitDto>();
            if (proposalResponse.Features != null && proposalResponse.Features.Count > 0)
            {
                foreach (var feature in proposalResponse.Features)
                {
                    int benefitCode = -1;
                    switch (feature.FeatureOptionId)
                    {
                        case "157339": benefitCode = 1; break; // driver only 
                        case "157342": benefitCode = 2; break; // driver and passenger
                        case "134377": benefitCode = 9; break; // Geo Extn Bahrain
                        case "134378": benefitCode = 10; break; // Geo Extn GCC
                        case "134379": benefitCode = 11; break; // Geo Extn Lebanon, Syria, Egypt, Jordan
                        case "134392": benefitCode = 12; break; // Waiver of Depreciation Clause
                        case "134390": benefitCode = 6; break; // Hire Car
                    }
                    if (benefitCode > -1)
                    {
                        var benfit = new BenefitDto(benefitCode, feature.FeaturePrice);
                        benefits.Add(benfit);
                    }

                }
            }
            if (isSandPlus)
            {
                var benfit1 = new BenefitDto()
                {
                    IsReadOnly = true,
                    IsSelected = true,
                    BenefitCode = 13,
                    BenefitPrice = 0

                };
                benefits.Add(benfit1);
            }
            return benefits;
        }
        /// <summary>
        /// Return product's price details from company product data
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        private List<PriceDto> GetProductPriceDetails(QuotationResponseInfo product)
        {
            if (product == null)
                return null;
            return new List<PriceDto> {
                new PriceDto
                {
                    PercentageValue = product.VATRate,
                    PriceValue = product.VATAmount,
                    PriceTypeCode = 8 // vat type
                },
                new PriceDto
                {
                    PriceValue = product.PolicyFee,
                    PriceTypeCode = 6 // vat type
                },
                new PriceDto
                {
                    PriceValue = product.NCDAmount,
                    PriceTypeCode=2 ,//No Claim Discount
                    PercentageValue = product.NCDRate
                },
                new PriceDto
                {
                    PriceValue = product.TotalLoading,
                    PriceTypeCode=4

                },
                new PriceDto
                {
                    PriceValue = decimal.Parse(product.TotalVehiclePremium),
                    PriceTypeCode = 7
                }
            };
        }


        /// <summary>
        /// Return product's price details from company product data
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        private List<PriceDto> GetProductPriceDetails(ProposalResponseInfo product)
        {
            if (product == null)
                return null;
            return new List<PriceDto> {
                new PriceDto
                {
                    PercentageValue = product.VATRate,
                    PriceValue = product.VATAmount,
                    PriceTypeCode = 8 // vat type
                },
                new PriceDto
                {
                    PriceValue = product.PolicyFee,
                    PriceTypeCode = 6 // vat type
                },
                new PriceDto
                {
                    PriceValue = product.NCDAmount,
                    PriceTypeCode=2 ,//No Claim Discount
                    PercentageValue = product.NCDRate
                },
                new PriceDto
                {
                    PriceValue = product.TotalVehiclePremium,
                    PriceTypeCode = 7
                },
                new PriceDto
                {
                    PriceValue = product.TotalLoading,
                    PriceTypeCode=4

                }
            };
        }

        private string GetCurrentLanguageCode()
        {
            string langCode = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "en" ? "1" : "2";
            return langCode;
        }

        private void UpdateProductQuotationNo(string quotationNo, Guid productId)
        {
            var p = _productRepository.Table.FirstOrDefault(e => e.Id == productId);
            if (p == null)
                throw new TameenkArgumentException("No product found with this id");
            p.QuotaionNo = quotationNo;
            _productRepository.Update(p);
        }

        #endregion

        #endregion

        private QuotationOutput GetTawuniyaQuotationTemp(PolicyRequest policy, ServiceRequestLog log)
        {
            //var quotationLog = new ServiceRequestLog
            //{
            //    CompanyID = log.CompanyID,
            //    CreatedDate = log.CreatedDate,
            //    InsuranceTypeCode = log.InsuranceTypeCode,
            //    RequestId = log.RequestId,
            //    ServiceURL = log.ServiceURL,
            //    UserID = log.UserID,
            //    UserName = log.UserName,
            //    VehicleId = log.VehicleId,
            //    DriverNin = log.DriverNin
            //};

            var configuration = Configuration as RestfulConfiguration;
            HttpResponseMessage message = new HttpResponseMessage();
            message.StatusCode = System.Net.HttpStatusCode.OK;
            QuotationOutput output = new QuotationOutput();
            log.ReferenceId = policy.ReferenceId;
            log.ServiceURL = configuration.GenerateQuotationUrl;
            //log.Channel = "Portal";
            //log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Quotation";
            log.CompanyName = "Tawuniya";

            try
            {
                if (_tameenkConfig.Quotatoin.TestMode)
                {
                    string nameOfFileJson = ".TestData.quotationTestData.json";
                    string tawuniyaResponse = ReadResource(GetType().Namespace, nameOfFileJson);
                    var handledResponse = JsonConvert.SerializeObject(HandleTawuniyaQuotationResponse(tawuniyaResponse));
                    message.Content = new StringContent(handledResponse);
                    output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    return output;
                }

                var request = CreateQuotationRequestTemp(policy, log);
                string stringPayload = JsonConvert.SerializeObject(request);
                log.ServiceRequest = stringPayload;
                output.ServiceRequest = stringPayload;
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                string authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes(configuration.AccessToken));
                DateTime dtBeforeCalling = DateTime.Now;
                var response = _httpClient.PostAsync(configuration.GenerateQuotationUrl, httpContent, authToken, null, "Basic").Result;
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = output.ErrorCode.ToString();
                    log.ServiceErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                if (response.Content == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content result return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                var value = response.Content.ReadAsStringAsync().Result;
                log.ServiceResponse = value;
                output.ServiceResponse = value;
                var tawuniyaModifiedResponse = HandleTawuniyaQuotationResponse(value);
                var tawuniyaJson = JsonConvert.SerializeObject(tawuniyaModifiedResponse);
                message.Content = new StringContent(tawuniyaJson);

                if (tawuniyaModifiedResponse.StatusCode == 1)
                {
                    UpdateProductQuotationNo(tawuniyaModifiedResponse.QuotationNo, policy.ProductInternalId);
                    policy.QuotationNo = tawuniyaModifiedResponse.QuotationNo;
                }
                if (tawuniyaModifiedResponse.StatusCode != 1)
                {
                    StringBuilder servcieErrors = new StringBuilder();
                    StringBuilder servcieErrorsCodes = new StringBuilder();

                    foreach (var error in tawuniyaModifiedResponse.Errors)
                    {
                        servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);
                        servcieErrorsCodes.AppendLine(error.Code);
                    }
                    output.ErrorCode = QuotationOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Quotation Service response error is : " + servcieErrors.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = servcieErrorsCodes.ToString();
                    log.ServiceErrorDescription = servcieErrors.ToString();
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                output.Output = message;
                output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                //log.ServiceResponse = response.Content.ReadAsStringAsync().Result;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }
        private QuotationRequestDto CreateQuotationRequestTemp(PolicyRequest policy, ServiceRequestLog log)
        {
            var quotationRequest = _quotationRequestRepository.Table
                .Include(e => e.Insured)
                .Include(e => e.Vehicle)
                .Include(e => e.QuotationResponses)
                .Include(e => e.Driver)
                .Include(e => e.Insured.Occupation)
                .Include(e => e.Drivers.Select(d => d.DriverViolations))
                .Include(e => e.Driver.Occupation)
                .Include(e => e.Insured.IdIssueCity)
                .Include(e => e.Insured.City)
                .FirstOrDefault(e => e.ID == policy.QuotationRequestId);

            if (quotationRequest == null)
            {
                throw new TameenkArgumentException("There is no quotation request with this id");
            }

            var quotationResponse = quotationRequest.QuotationResponses.FirstOrDefault(e => e.ReferenceId == policy.ReferenceId);

            var quotation = _quotationService.GetQuotationRequestData(quotationRequest,
                quotationResponse, quotationResponse.InsuranceTypeCode.Value, false, log.UserID?.ToString(), quotationResponse.DeductibleValue);

            var p = _productRepository.Table.FirstOrDefault(e => e.Id == policy.ProductInternalId);
            if (p == null)
            {
                throw new TameenkArgumentException("There is no product  with this id " + policy.ProductInternalId);
            }
            var tawuniyaProposal = _tawuniyaProposalRepository.Table.FirstOrDefault(e => e.ReferenceId == policy.ReferenceId && e.ProposalTypeCode == p.InsuranceTypeCode);
            if (tawuniyaProposal == null)
            {
                throw new TameenkArgumentException("There is no proposal for this reference id");
            }

            var request = new QuotationRequestDto();

            #region Header
            request.CreateQuoteRequest.Header.InitiatedDateTime = DateTime.Now.ToString();
            request.CreateQuoteRequest.Header.MesssageType = "createQuote";
            request.CreateQuoteRequest.Header.TrackingId = policy.ReferenceId;
            request.CreateQuoteRequest.Header.RoutingIdentifier = "TAWN";
            request.CreateQuoteRequest.Header.Sender = "TAWN";
            request.CreateQuoteRequest.Header.Version = "1";
            #endregion

            #region QuotationInfo
            request.CreateQuoteRequest.QuotationRequest.QuotationInfo = new QuotationInfo
            {
                ///ProductCode = tawuniyaProposal.ProposalTypeCode == 1 ? "MotorImtiazeTP" : tawuniyaProposal.ProposalTypeCode == 2 ? "PRMotorImtiazeCO" : "PRMotorImtiazeSP",
                ProductCode = p.InsuranceTypeCode == 1 ? "MotorImtiazeTP" : p.InsuranceTypeCode == 2 ? "PRMotorImtiazeCO" : "PRMotorImtiazeSP",
                IdNumber = quotation.InsuredId.ToString(),
                LanguageCode = GetCurrentLanguageCode(),
                RequiredInceptionDate = quotation.PolicyEffectiveDate.ToString("yyyy-MM-dd", new CultureInfo("en-US")),
                SpecialSchemeCode = string.IsNullOrEmpty(quotation.PromoCode) ? "AGGREGATOR" : quotation.PromoCode,
                ProposalNumber = tawuniyaProposal.ProposalNumber
            };
            #endregion

            #region ChannelDetails
            request.CreateQuoteRequest.QuotationRequest.ChannelDetails = new List<ChannelDetail> {
                new ChannelDetail
                {
                    ConsumerApplicationTypeReference ="TMNK",
                    UserName =/*"FRS2050002"*/"FRS2050002",
                    SourceCode ="755927",
                    ChannelCode ="205"
                }
            };
            #endregion

            #region customerDetails

            var customerDetail = new CustomerDetail
            {
                FullNameInEnglish = $"{quotation.InsuredFirstNameEn} {quotation.InsuredMiddleNameEn} {quotation.InsuredLastNameEn}",
                FirstNameEnglish = quotation.InsuredFirstNameEn,
                MidNameEnglish = quotation.InsuredMiddleNameEn,
                LastNameEnglish = quotation.InsuredLastNameEn,
                FullNameInArabic = $"{quotation.InsuredFirstNameAr} {quotation.InsuredMiddleNameAr} {quotation.InsuredLastNameAr}",
                FirstNameArabic = quotation.InsuredFirstNameAr,
                MidNameArabic = quotation.InsuredMiddleNameAr,
                LastNameArabic = quotation.InsuredLastNameAr,
                NationalityCode = quotation.InsuredNationalityCode,
                AddressCity = quotation.InsuredCity,
                Occupation = quotation.InsuredOccupationCode,
                MaritalStatus = quotation.InsuredSocialStatusCode,
                Gender = quotation.InsuredGenderCode == "M" ? "1" : "2",
                InsuredEducationCode = quotation.InsuredEducationCode?.ToString()
            };
            if (!string.IsNullOrEmpty(quotation.InsuredBirthDateG))
            {
                var birthDateG = quotation.InsuredBirthDateG.Split('-');
                customerDetail.DobGreg = $"{birthDateG[2]}-{birthDateG[1]}-{birthDateG[0]}";
            }

            if (quotation.InsuredIdTypeCode == 1)
            {
                if (!string.IsNullOrEmpty(quotation.InsuredBirthDateH))
                {
                    var birthDateH = quotation.InsuredBirthDateH.Split('-');
                    customerDetail.DobHijri = $"{birthDateH[2]}-{birthDateH[1]}-{birthDateH[0]}";
                }
            }

            request.CreateQuoteRequest.QuotationRequest.CustomerDetails = new List<CustomerDetail>
            {
                customerDetail
            };
            #endregion

            #region vehicleDetails
            string registrationExpiryDate = null;
            if (!string.IsNullOrWhiteSpace(quotation.VehicleRegExpiryDate))
            {
                var registrationExpiryDateParts = quotation.VehicleRegExpiryDate.Split('-');
                registrationExpiryDate = $"{registrationExpiryDateParts[2]}-{registrationExpiryDateParts[1]}-{registrationExpiryDateParts[0]}";
            }
            bool isVehicleRegistered = quotation.VehicleIdTypeCode == 1 ? true : false;

            var vehicleDetail = new VehicleDetail
            {
                PlateType = !string.IsNullOrWhiteSpace(quotation.VehiclePlateTypeCode) ? quotation.VehiclePlateTypeCode : "10",
                MakeCode = quotation.VehicleMakerCode,
                ModelCode = quotation.VehicleModelCode,
                PlateNumber = quotation.VehiclePlateNumber.ToString(),
                PlateText1 = quotation.VehiclePlateText1,
                PlateText2 = quotation.VehiclePlateText2,
                PlateText3 = quotation.VehiclePlateText3,
                YearOfManufacture = quotation.VehicleModelYear.ToString(),
                ChassisNumber = quotation.VehicleChassisNumber,
                RegistrationExpiryDate = registrationExpiryDate,
                Color = quotation.VehicleMajorColorCode,
                RegistrationOfficeCity = quotation.VehicleRegPlace,
                MajorDrivingCity = quotation.InsuredCity,
                NCDYears = quotation.NCDFreeYears.ToString(),
                InsurancePeriod = "1",
                AgencyRepairRequired = quotation.VehicleAgencyRepair.Value ? "Y":"N",
                UsageType = "PRIVATE",
                Deductible = quotation.DeductibleValue.HasValue ? quotation.DeductibleValue.Value.ToString() : "",
                VehicleValue = quotation.VehicleValue.HasValue ? quotation.VehicleValue.Value.ToString() : ""
            };

            if (isVehicleRegistered)
            {
                vehicleDetail.SerialNumber = quotation.VehicleId.ToString();
            }
            else
            {
                vehicleDetail.CustomCardNumber = quotation.VehicleId.ToString();
            }

            if (quotation.VehicleOwnerTransfer)
            {
                vehicleDetail.VehicleTransferFlag = "Y";
                vehicleDetail.TrassferCaseOwnerId = quotation.VehicleOwnerId.ToString();
            }
            else
                vehicleDetail.VehicleTransferFlag = "N";

            #region VehicleMoreInfo

            vehicleDetail.VehicleMoreInfo = new List<VehicleMoreInfo>()
            {
                new VehicleMoreInfo
                {
                    AnnualMileage=quotation.VehicleMileageExpectedAnnualCode?.ToString(),
                    AxleWeightCode = quotation.VehicleAxleWeightCode.ToString(),
                    EngineSizeCode=quotation.VehicleEngineSizeCode?.ToString(),
                    Mileage = quotation.VehicleMileage?.ToString(),
                    OverNightParkCode=quotation.VehicleOvernightParkingLocationCode?.ToString()
                }
            };
            #endregion

            #region VehicleDriverDetails

            if (quotation.Drivers != null)
            {
                vehicleDetail.VehicleDriverDetails = new List<VehicleDriverDetail>();
                foreach (var driver in quotation.Drivers)
                {
                    var diverBirthDate = driver.DriverBirthDate.Split('-');
                    var vehicleDriver = new VehicleDriverDetail
                    {
                        IdNo = driver.DriverId.ToString(),
                        EName = $"{driver.DriverFirstNameEn} {driver.DriverMiddleNameEn} {driver.DriverLastNameEn}",
                        EFirstName = driver.DriverFirstNameEn,
                        EMiddleName = driver.DriverMiddleNameEn,
                        ELastName = driver.DriverLastNameEn,
                        AName = $"{driver.DriverFirstNameAr} {driver.DriverMiddleNameAr} {driver.DriverLastNameAr}",
                        AFirstName = driver.DriverFirstNameAr,
                        AMiddleName = driver.DriverMiddleNameAr,
                        ALastName = driver.DriverLastNameAr,
                        Gender = driver.DriverGenderCode == "M" ? "1" : "2",
                        DriverType = driver.DriverTypeCode == 1 ? "MD" : "AD",
                        NationalityCode = driver.DriverNationalityCode,
                        SocialStatusCode = driver.DriverSocialStatusCode,
                        EducationCode = driver.DriverEducationCode?.ToString(),
                        MedicalConditionCode = driver.DriverMedicalConditionCode?.ToString(),
                        ChildrenBelow16Years = driver.DriverChildrenBelow16Years?.ToString(),
                        NCD = driver.DriverNCDFreeYears?.ToString(),
                        Relation = "PH"
                    };
                    if (driver.DriverIdTypeCode == 1)
                    {
                        vehicleDriver.DobHijri = $"{diverBirthDate[2]}-{diverBirthDate[1]}-{diverBirthDate[0]}";
                    }
                    else
                    {
                        vehicleDriver.DobGreg = driver.DriverBirthDateG.ToString("yyyy-MM-dd", new CultureInfo("en-US"));//$"{diverBirthDate[2]}-{diverBirthDate[1]}-{diverBirthDate[0]}";
                    }

                    vehicleDriver.HomeAddress = new HomeAddress
                    {
                        HomeCityName = driver.DriverHomeCity
                    };
                    vehicleDetail.VehicleDriverDetails.Add(vehicleDriver);
                }

            }

            #endregion

            request.CreateQuoteRequest.QuotationRequest.VehicleDetails = new List<VehicleDetail>
            {
                vehicleDetail,
            };

            #endregion

            return request;
        }

        private static List<BenefitDto> GetProductBenefitsNew(List<ApplicableFeatureGroup> applicableFeatureGroup, bool isSandPlus)
        {
            List<BenefitDto> benfits = new List<BenefitDto>();
            foreach (var benfitGroup in applicableFeatureGroup)
            {
                if (benfitGroup == null || benfitGroup.FeatureItems == null || benfitGroup.FeatureItems.FeatureItemsArray == null)
                    continue;

                var groupBenfits = benfitGroup.FeatureItems.FeatureItemsArray.ToList();
                foreach (var benfit in groupBenfits)
                {
                    var benfitPrice = benfit.PriceDetails?.PriceDetailsArray.FirstOrDefault()?.PriceValue;
                    var benfitModel = new BenefitDto(0, (benfitPrice != null && benfitPrice.HasValue) ? benfitPrice.Value : 0);
                    benfitModel.BenefitId = benfit.FeatureCode;
                    benfitModel.BenefitDescAr = benfitModel.BenefitNameAr = benfit.FeatureDescriptionAr;
                    benfitModel.BenefitDescEn = benfitModel.BenefitNameEn = benfit.FeatureDescriptionEn;
                    benfits.Add(benfitModel);
                }
            }

            if (isSandPlus)
            {
                var benfit1 = new BenefitDto()
                {
                    IsReadOnly = true,
                    IsSelected = true,
                    BenefitCode = 13,
                    BenefitPrice = 0

                };
                benfits.Add(benfit1);
            }

            return benfits;
        }
    }
}
