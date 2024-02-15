using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums.Quotations;
using Tameenk.Core.Domain.Enums.Vehicles;
using Tameenk.Core.Infrastructure;
using Tameenk.Integration.Core.Providers;
using Tameenk.Integration.Core.Providers.Configuration;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Integration.Providers.Wataniya.Dtos;
using Tameenk.Integration.Providers.Wataniya.Dtos.Autolease;
using Tameenk.Integration.Providers.Wataniya.MappingResources;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.WebResources;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.Checkouts;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Extensions;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.Inquiry.Components;
using Tameenk.Services.Logging;
using Tameenk.Services.Notifications;

namespace Tameenk.Integration.Providers.Wataniya
{
    public class WataniyaInsuranceProvider : RestfulInsuranceProvider
    {
        private readonly RestfulConfiguration _restfulConfiguration;
        private readonly TameenkConfig _tameenkConfig;
        private readonly IHttpClient _httpClient;
        private readonly string _accessTokenBase64;
        private readonly IRepository<QuotationResponse> _quotationResponseRepository;
        private readonly IRepository<PolicyProcessingQueue> _policyProcessingQueueRepository;
        private readonly IRepository<WataniyaDraftPolicy> _wataniyaDraftPolicyRepository;
        private readonly IRepository<WataniyaMotorPolicyInfo> _wataniyaMotorPolicyInfoRepository;
        private readonly IQuotationService _quotationService;
        private readonly IRepository<BankNins> _bankNinsRepository;

        private readonly HashSet<int> CentralRegions = new HashSet<int> { 1, 4 };
        private readonly HashSet<int> WesternRegions = new HashSet<int> { 2, 3 };
        private readonly HashSet<int> EasternRegions = new HashSet<int> { 5 };
        private readonly HashSet<int> SouthernRegions = new HashSet<int> { 6, 10, 11, 12 };
        private readonly HashSet<int> NorthernRegions = new HashSet<int> { 7, 8, 9, 13 };
        private readonly IWataniyaNajmQueueService _wataniyaNajmQueueService;

        private readonly HashSet<WataniyaBenefitMmodel> BenefitsLUT = new HashSet<WataniyaBenefitMmodel>() {
            new WataniyaBenefitMmodel(){ BenefitId = 116, BenefitCode = "", BenefitDescEn = "", BenefitDescAr = "" },
            new WataniyaBenefitMmodel(){ BenefitId = 117, BenefitCode = "MPAP", BenefitDescEn = "Personal Accident - Passenger Cover", BenefitDescAr = "تغطية الحوادث الشخصية للركاب" },
            new WataniyaBenefitMmodel(){ BenefitId = 118, BenefitCode = "MPAD", BenefitDescEn = "Personal Accident - Driver Cover", BenefitDescAr = "تغطية الحوادث الشخصية للسائق " },
            new WataniyaBenefitMmodel(){ BenefitId = 119, BenefitCode = "MGAE", BenefitDescEn = "Geographical Area Extension for GCC country", BenefitDescAr = "التغطية الجغرافية لدول مجلس التعاون الخليجي" },
            new WataniyaBenefitMmodel(){ BenefitId = 120, BenefitCode = "MRCR", BenefitDescEn = "Replacement Car Whilst Under Repair", BenefitDescAr = "سيارة بديلة عند دخول المركبة للاصلاح" },
            new WataniyaBenefitMmodel(){ BenefitId = 121, BenefitCode = "MTB", BenefitDescEn = "Towing Benefit", BenefitDescAr = "خدمة السحب" },
            new WataniyaBenefitMmodel(){ BenefitId = 122, BenefitCode = "MME", BenefitDescEn = "Medical Expenses", BenefitDescAr = "مصاريف طبية" },
            //new WataniyaBenefitMmodel(){ BenefitId = 259, BenefitCode = "MCOM", BenefitDescEn = "TPL Limit SAR 10,000,000", BenefitDescAr = "المسؤولية المدنية تجاه الغير بحد أقصى 10,000,000 ريال" },
            new WataniyaBenefitMmodel(){ BenefitId = 259, BenefitCode = "MCOM", BenefitDescEn = "Liabilities to Third Parties:maximum liability of the Company and damages shall not exceed a combined single limit of SAR10,000,000 (ten million Saudi Riyals)", BenefitDescAr = "المسؤولية تجاه الغير (الطرف الثالث):الحد الأقصى لمسؤولية الشركة في الواقعة الواحدة وخلال فترة سريان وثيقة التأمين بالنسبة للأضرار الجسدية (بما في ذلك الديات والمبالغ المقدرة عن الإصابات والمصاريف الطبية) والأضرار المادية معاً لن تتجاوز مبلغاً إجمالياً قدره 10,000,000ريال (عشرة ملايين ريال سعودي) حداً أقصى لمسئولية الشركة." },
            new WataniyaBenefitMmodel(){ BenefitId = 260, BenefitCode = "MCOM", BenefitDescEn = "Coverage for total or partial loss of Vehicle", BenefitDescAr = "تغطية الخسارة الكلية أو الجزئية للمركبة" },
            new WataniyaBenefitMmodel(){ BenefitId = 261, BenefitCode = "MNHS", BenefitDescEn = "Natural Disasters", BenefitDescAr = "الكوارث الطبيعية" },
            new WataniyaBenefitMmodel(){ BenefitId = 262, BenefitCode = "MCOM", BenefitDescEn = "Theft & Fire", BenefitDescAr = "تغطية السرقة والحرائق" },
            new WataniyaBenefitMmodel(){ BenefitId = 265, BenefitCode = "MTB", BenefitDescEn = "Towing Expenses (Actual Charges, subject to maximum of SAR 500/- per incident within the city and SAR 1,000/- outside the city; provided that the transportation receipt is submitted when filing the claim..)", BenefitDescAr = "مصاريف السحب (الرسوم الفعلية ، بحد أقصى 500 ريال سعودي / - لكل حادث داخل المدينة و 1،000 ريال سعودي / - خارج المدينة ؛ بشرط تقديم إيصال النقل عند تقديم المطالبة ..)" },
            new WataniyaBenefitMmodel(){ BenefitId = 266, BenefitCode = "MME", BenefitDescEn = "Emergency Medical Expenses (SR 25,000 any one occurrence)", BenefitDescAr = "المصاريف الطبية الطارئة (25.000 ريال سعودي لكل حالة واحدة)" },
            new WataniyaBenefitMmodel(){ BenefitId = 489, BenefitCode = "489", BenefitDescEn = "Roadside assistance", BenefitDescAr = " المساعدة على الطريق" },
            new WataniyaBenefitMmodel(){ BenefitId = 490, BenefitCode = "489", BenefitDescEn = "Unnamed Driver ", BenefitDescAr = " سائق غير مسمى" }
        };

        private const string GET_COMP_QUOTATION_URL = "https://aggregator.wataniya.com.sa/Aggregator/rest/Aggregator/CompareQuotesRMCOM";
        private const string GET_TPL_QUOTATION_URL = "https://aggregator.wataniya.com.sa/Aggregator/rest/Aggregator/CompareQuotesRMTPL";
        // draft policy
        private const string GET_DRAFT_POLICY_URL = "https://aggregator.wataniya.com.sa/Aggregator/rest/Aggregator/DraftRMCOM";
        // issue policy
        private const string GET_PURCHASE_NOTIFICATION_URL = "https://aggregator.wataniya.com.sa/Aggregator/rest/Aggregator/IssuePolicyRMTPL";

        // issue policy COMP
        private const string GET_PURCHASE_NOTIFICATION_URL_COM = "https://aggregator.wataniya.com.sa/Aggregator/rest/Aggregator/IssuePolicyRMCOM";

        // autolease urls
        private const string AUTOLEASE_CERTIFCATE_BATH = "C:/inetpub/wwwroot/AutoleasingQuotationApi/watinyacert/browser.pfx";
        private const string AUTOLEASE_CERTIFCATE_PASSWORD = "rZ7dzXq60L3lf3E";
        private const string GET_AUTOLEASE_DRAFTPOLICY_URL = "https://api.wataniya.com.sa/AutoLease_API/rest/AutoleaseExposing/Draft";
        private const string GET_AUTOLEASE_POLICY_URL = "https://api.wataniya.com.sa/AutoLease_API/rest/AutoleaseExposing/IssuePolicy";

        private const string NAJM_Status_URL = "https://b2b.wataniya.com.sa/GetPolicy/rest/GetPolicyInfo/NajmStatus";
        private readonly string NAJM_Status_URL_BASE46_TOKEN = "Tameenk:Tameenk@123";

        private const string Najm_CERTIFCATE_BATH = "C:/inetpub/wwwroot/AutoleasingQuotationApi/watinyacert/browser.pfx";
        private const string Najm_CERTIFCATE_PASSWORD = "rZ7dzXq60L3lf3E";

        private const string CERTIFCATE_BATH = "C:/inetpub/wwwroot/QuotationNewApi/watinyacert/browser.pfx";
       // private const string CERTIFCATE_PASSWORD = "W@t15An2020";
        private const string CERTIFCATE_PASSWORD = "rZ7dzXq60L3lf3E";

        public WataniyaInsuranceProvider(TameenkConfig tameenkConfig, ILogger logger, IRepository<PolicyProcessingQueue> policyProcessingQueueRepository,
            IRepository<QuotationResponse> quotationResponseRepository, IRepository<WataniyaDraftPolicy> wataniyaDraftPolicyRepository,
            IQuotationService quotationService, IRepository<WataniyaMotorPolicyInfo> wataniyaMotorPolicyInfoRepository
            , IRepository<BankNins> bankNinsRepository, IWataniyaNajmQueueService wataniyaNajmQueueService)
             : base(tameenkConfig, new RestfulConfiguration
             {
                 //GenerateQuotationUrl = "https://ncd.wataniya.com.sa:2021/api/Quotation/RequestQuotation",
                 //GeneratePolicyUrl = "https://ncd.wataniya.com.sa:2021/api/Policy/RequestPolicy",
                 GenerateQuotationUrl = "",
                 GeneratePolicyUrl = "",
                 GenerateClaimRegistrationUrl = "",
                 GenerateClaimNotificationUrl = "",
                 CancelPolicyUrl = "",
                 GenerateAutoleasingQuotationUrl = "https://api.wataniya.com.sa/AutoLease_API/rest/AutoleaseExposing/CompareQuotes",
                 GenerateAutoleasingPolicyUrl = "",
                 //AutoleasingAccessToken = "Tajeer_BCARE:TB@123456",
                 AutoleasingAccessToken = "Yusr.BacreAutolease:Yusr@123",
                 AccessToken = "Tameenk:Tameenk@123",
                 ProviderName = "Wataniya"
             }, logger, policyProcessingQueueRepository)
        {
            _restfulConfiguration = Configuration as RestfulConfiguration;
            _tameenkConfig = tameenkConfig;
            _httpClient = EngineContext.Current.Resolve<IHttpClient>();
            _accessTokenBase64 = string.IsNullOrWhiteSpace(_restfulConfiguration.AccessToken) ?
                null :
                Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(_restfulConfiguration.AccessToken));
            _quotationResponseRepository = quotationResponseRepository;
            _policyProcessingQueueRepository = policyProcessingQueueRepository;
            _wataniyaDraftPolicyRepository = wataniyaDraftPolicyRepository;
            _quotationService = quotationService;
            _wataniyaMotorPolicyInfoRepository = wataniyaMotorPolicyInfoRepository;
            _bankNinsRepository = bankNinsRepository;
            _wataniyaNajmQueueService = wataniyaNajmQueueService;

        }

        #region Quotation

        protected override object ExecuteQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            //in case test mode execute the code from the base.
            if (_tameenkConfig.Quotatoin.TestMode)
                return base.ExecuteQuotationRequest(quotation, predefinedLogInfo);

            ServiceOutput output = SubmitQuotationRequest(quotation, predefinedLogInfo);
            if (output.ErrorCode != ServiceOutput.ErrorCodes.Success)
            {
                return null;
            }
            return output.Output;
        }

        protected override object ExecuteAutoleasingQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            //in case test mode execute the code from the base.
            if (_tameenkConfig.Quotatoin.TestMode)
                return base.ExecuteAutoleasingQuotationRequest(quotation, predefinedLogInfo);

            ServiceOutput output = SubmitAutLoeasingQuotationRequest(quotation, predefinedLogInfo);
            if (output.ErrorCode != ServiceOutput.ErrorCodes.Success)
            {
                return null;
            }
            return output.Output;
        }

        protected override ServiceOutput SubmitQuotationRequest(QuotationServiceRequest quoteModel, ServiceRequestLog log)
        {
            ServiceOutput output = new ServiceOutput();
            if (string.IsNullOrEmpty(log.Channel))
                log.Channel = "Portal";
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Quotation";
            log.CompanyName = _restfulConfiguration.ProviderName;
            log.VehicleMaker = quoteModel?.VehicleMaker;
            log.VehicleMakerCode = quoteModel?.VehicleMakerCode.ToString();
            log.VehicleModel = quoteModel?.VehicleModel;
            log.VehicleModelCode = quoteModel?.VehicleModelCode.ToString();
            log.VehicleModelYear = quoteModel?.VehicleModelYear;
            log.ReferenceId = quoteModel.ReferenceId;
            var stringPayload = string.Empty;
            DateTime dtBeforeCalling = DateTime.Now;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                var testMode = _tameenkConfig.Quotatoin.TestMode;
                if (testMode)
                {
                    const string nameOfFile = ".TestData.quotationTestData.json";
                    string responseData = ReadResource(GetType().Namespace, nameOfFile);
                    HttpResponseMessage message = new HttpResponseMessage();
                    message.Content = new StringContent(responseData);
                    message.StatusCode = System.Net.HttpStatusCode.OK;

                    output.Output = message;
                    output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";

                    return output;
                }
                dtBeforeCalling = DateTime.Now;
                if (quoteModel.ProductTypeCode == 1)
                {
                    log.ServiceURL = GET_TPL_QUOTATION_URL;
                    log.ServiceRequest = JsonConvert.SerializeObject(quoteModel);
                    var quotation = MappingWataniyaTplQuotationRequest(quoteModel);
                    log.ServiceRequest = JsonConvert.SerializeObject(quotation);
                    var postTask = _httpClient.PostAsyncWithCertificate(GET_TPL_QUOTATION_URL, quotation,CERTIFCATE_BATH,CERTIFCATE_PASSWORD, _accessTokenBase64, authorizationMethod: "Basic");
                    postTask.Wait();
                    response = postTask.Result;
                }
                else
                {
                    log.ServiceURL = GET_COMP_QUOTATION_URL;
                    log.ServiceRequest = JsonConvert.SerializeObject(quoteModel);
                    var quotation = MappingWataniyaCompQuotationRequest(quoteModel);
                    log.ServiceRequest = JsonConvert.SerializeObject(quotation);
                    var postTask = _httpClient.PostAsyncWithCertificate(GET_COMP_QUOTATION_URL, quotation, CERTIFCATE_BATH, CERTIFCATE_PASSWORD, _accessTokenBase64, authorizationMethod: "Basic");
                    postTask.Wait();
                    response = postTask.Result;
                }
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null)
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
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
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
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
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content result return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;

                if (quoteModel.ProductTypeCode == 1)
                {
                    var desrialisedObj = JsonConvert.DeserializeObject<WataniyaTplQuotationResponseDto>(response.Content.ReadAsStringAsync().Result);
                    if (desrialisedObj != null && desrialisedObj.errors != null)
                    {
                        StringBuilder servcieErrors = new StringBuilder();
                        StringBuilder servcieErrorsCodes = new StringBuilder();

                        foreach (var error in desrialisedObj.errors)
                        {
                            servcieErrors.AppendLine("Error Code: " + error.code + " and the error message : " + error.message);
                            servcieErrorsCodes.AppendLine(error.code);
                        }

                        output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                        output.ErrorDescription = "Quotation Service response error is : " + servcieErrors.ToString();
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        log.ServiceErrorCode = servcieErrorsCodes.ToString();
                        log.ServiceErrorDescription = servcieErrors.ToString();
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return output;
                    }

                    var initialDraftModel = new WataniyaMotorPolicyInfo();
                    initialDraftModel.ReferenceId = quoteModel.ReferenceId;
                    initialDraftModel.QuoteReferenceNo = desrialisedObj.QuoteReferenceNo.ToString();
                    initialDraftModel.PolicyEffectiveDate = desrialisedObj.Details.PolicyEffectiveDate;
                    initialDraftModel.PolicyExpiryDate = desrialisedObj.Details.PolicyExpiryDate;
                    initialDraftModel.Channel = log.Channel;
                    initialDraftModel.Method = log.Method;
                    initialDraftModel.UserName = log.UserName;
                    initialDraftModel.UserID = log.UserID;
                    initialDraftModel.CreatedDate = DateTime.Now;

                    string exception = string.Empty;
                    var _quotationService = EngineContext.Current.Resolve<IQuotationService>();
                    _quotationService.InsertOrupdateWataniyaMotorPolicyInfo(initialDraftModel, out exception);
                    if (!string.IsNullOrEmpty(exception))
                    {
                        output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                        output.ErrorDescription = "Error happend wheb try to insert Initial policy info " + exception;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        log.ServiceErrorCode = log.ErrorDescription.ToString();
                        log.ServiceErrorDescription = log.ErrorDescription;
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return output;
                    }
                }
                else
                {
                    var desrialisedObj = JsonConvert.DeserializeObject<WataniyaCompQuotationResponseDto>(response.Content.ReadAsStringAsync().Result);
                    if (desrialisedObj != null && desrialisedObj.errors != null)
                    {
                        StringBuilder servcieErrors = new StringBuilder();
                        StringBuilder servcieErrorsCodes = new StringBuilder();

                        foreach (var error in desrialisedObj.errors)
                        {
                            servcieErrors.AppendLine("Error Code: " + error.code + " and the error message : " + error.message);
                            servcieErrorsCodes.AppendLine(error.code);
                        }

                        output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                        output.ErrorDescription = "Quotation Service response error is : " + servcieErrors.ToString();
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        log.ServiceErrorCode = servcieErrorsCodes.ToString();
                        log.ServiceErrorDescription = servcieErrors.ToString();
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return output;
                    }

                    var initialDraftModel = new WataniyaMotorPolicyInfo();
                    initialDraftModel.ReferenceId = quoteModel.ReferenceId;
                    initialDraftModel.QuoteReferenceNo = desrialisedObj.QuoteReferenceNo.ToString();
                    initialDraftModel.PolicyEffectiveDate = DateTime.ParseExact(desrialisedObj.Details.PolicyEffectiveDate, "yyyy-MM-dd", new CultureInfo("en-US"));
                    initialDraftModel.PolicyExpiryDate = DateTime.ParseExact(desrialisedObj.Details.PolicyExpiryDate, "yyyy-MM-dd", new CultureInfo("en-US"));
                    initialDraftModel.Channel = log.Channel;
                    initialDraftModel.Method = log.Method;
                    initialDraftModel.UserName = log.UserName;
                    initialDraftModel.UserID = log.UserID;
                    initialDraftModel.CreatedDate = DateTime.Now;

                    string exception = string.Empty;
                    var _quotationService = EngineContext.Current.Resolve<IQuotationService>();
                    _quotationService.InsertOrupdateWataniyaMotorPolicyInfo(initialDraftModel, out exception);
                    if (!string.IsNullOrEmpty(exception))
                    {
                        output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                        output.ErrorDescription = "Error happend wheb try to insert Initial policy info " + exception;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        log.ServiceErrorCode = log.ErrorDescription.ToString();
                        log.ServiceErrorDescription = log.ErrorDescription;
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return output;
                    }
                }

                output.Output = response;
                output.ErrorCode = ServiceOutput.ErrorCodes.Success;
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
                output.ErrorCode = ServiceOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }

        private ServiceOutput SubmitAutLoeasingQuotationRequest(QuotationServiceRequest quoteModel, ServiceRequestLog log)
        {
            ServiceOutput output = new ServiceOutput();
            if (string.IsNullOrEmpty(log.Channel))
                log.Channel = "AutoLease";
            log.ServiceURL = _restfulConfiguration.GenerateAutoleasingQuotationUrl;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "AutoleasingQuotation";
            log.ReferenceId = quoteModel.ReferenceId;
            log.CompanyName = "Wataniya";
            log.VehicleId = quoteModel.VehicleId.ToString();
            log.VehicleMaker = quoteModel?.VehicleMaker;
            log.VehicleMakerCode = quoteModel?.VehicleMakerCode;
            log.VehicleModel = quoteModel?.VehicleModel;
            log.VehicleModelCode = quoteModel?.VehicleModelCode;
            log.VehicleModelYear = quoteModel?.VehicleModelYear;

            var stringPayload = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                var testMode = _tameenkConfig.Quotatoin.TestMode;
                if (testMode)
                {
                    const string nameOfFile = ".TestData.quotationTestData.json";
                    string responseData = ReadResource(GetType().Namespace, nameOfFile);
                    HttpResponseMessage message = new HttpResponseMessage();
                    message.Content = new StringContent(responseData);
                    message.StatusCode = System.Net.HttpStatusCode.OK;

                    output.Output = message;
                    output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";

                    return output;
                }

                // quotation service call
                var quotation = MappingWataniyaAutoLeasingQuotationRequest(quoteModel);
                //if (quoteModel.BankId == 2)// Alyusr
                //    _restfulConfiguration.AutoleasingAccessToken = "Yusr.BacreAutolease:Yusr@123";
              if (quoteModel.BankId == 3)//Aljaber
                    _restfulConfiguration.AutoleasingAccessToken = "Aljabr_Bcare:Aljabr@123";

                log.ServiceRequest = JsonConvert.SerializeObject(quotation);
                var _autoLeaseAccesToken = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(_restfulConfiguration.AutoleasingAccessToken));
                DateTime dtBeforeCalling = DateTime.Now;
                var postTask = _httpClient.PostAsyncWithCertificate(_restfulConfiguration.GenerateAutoleasingQuotationUrl, quotation, AUTOLEASE_CERTIFCATE_BATH, AUTOLEASE_CERTIFCATE_PASSWORD, _autoLeaseAccesToken, authorizationMethod: "Basic");
                postTask.Wait();
                DateTime dtAfterCalling = DateTime.Now;
                response = postTask.Result;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null)
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
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
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
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
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content result return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;

                var desrialisedObj = JsonConvert.DeserializeObject<WatnyiaAutoLeaseQuotationResponse>(response.Content.ReadAsStringAsync().Result);
                if (desrialisedObj != null && desrialisedObj.Status != 1 && desrialisedObj.ErrorList != null && desrialisedObj.ErrorList.Count > 0)
                {
                    StringBuilder servcieErrors = new StringBuilder();
                    StringBuilder servcieErrorsCodes = new StringBuilder();

                    foreach (var error in desrialisedObj.ErrorList)
                    {
                        servcieErrors.AppendLine("Error Code: " + error.ErrorCode + " and the error message : " + error.ErrorMessage);
                        servcieErrorsCodes.AppendLine(error.ErrorCode);
                    }

                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Quotation Service response error is : " + servcieErrors.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = servcieErrorsCodes.ToString();
                    log.ServiceErrorDescription = servcieErrors.ToString();
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                if (desrialisedObj.Policy == null)
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content result return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                var initialDraftModel = new WataniyaDraftPolicy();
                initialDraftModel.ReferenceId = quoteModel.ReferenceId;
                //initialDraftModel.ReferenceId = quotation.Policy.RequestReferenceNumber.ToString();
                initialDraftModel.QuotationNumber = desrialisedObj.Policy.ReferenceNumber;
                initialDraftModel.PolicyEffectiveDate = quotation.Policy.PolicyEffectiveDate;
                initialDraftModel.PolicyExpiryDate = quotation.Policy.PolicyExpiryDate;
                initialDraftModel.Channel = log.Channel;
                initialDraftModel.Method = log.Method;
                initialDraftModel.UserName = log.UserName;
                initialDraftModel.UserID = log.UserID;
                initialDraftModel.CreatedDate = log.CreatedDate;

                string exception = string.Empty;
                var _quotationService = EngineContext.Current.Resolve<IQuotationService>();
                _quotationService.InsertOrupdateWataniyaAutoleasePolicyInfo(initialDraftModel, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Error happend when try to insert Initial policy info " + exception;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorDescription.ToString();
                    log.ServiceErrorDescription = log.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                output.Output = response;
                output.ErrorCode = ServiceOutput.ErrorCodes.Success;
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
                output.ErrorCode = ServiceOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;

                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }

        #region Common Methods

        #region Tpl(Request / Response)

        protected WataniyaTplQuotationRequestDto MappingWataniyaTplQuotationRequest(QuotationServiceRequest quotation)
        {
            var wataniyaQuotation = new WataniyaTplQuotationRequestDto();
            wataniyaQuotation.Details = HandleTplRequestDetails(quotation);
            wataniyaQuotation.RequestReferenceNo = quotation.ReferenceId;
            wataniyaQuotation.InsuranceTypeID = quotation.ProductTypeCode;
            wataniyaQuotation.InsuranceCompanyCode = (short)quotation.InsuranceCompanyCode;
            if (!string.IsNullOrEmpty(quotation.PromoCode))
            {
                wataniyaQuotation.Details.IsScheme = true;
                wataniyaQuotation.Details.SchemeDetails = new List<SchemeDetails>() { new SchemeDetails() { SchemeRef = quotation.PromoCode, IcSchemeRef = quotation.PromoCode } };
            }
            return wataniyaQuotation;
        }

        protected WataniyaTplQuotationRequesDetailstDto HandleTplRequestDetails(QuotationServiceRequest quotation)
        {
            var details = new WataniyaTplQuotationRequesDetailstDto();

            details.PolicyholderIdentityTypeCode = quotation.InsuredIdTypeCode;
            details.PolicyHolderID = quotation.InsuredId;
            if (!string.IsNullOrEmpty(quotation.IdExpiryDate))
            {
                var IdExpirySplits = quotation.IdExpiryDate.Split('-');
                details.PolicyholderIDExpiry = IdExpirySplits[1] + "-" + IdExpirySplits[2];
            }
            else
            {
                UmAlQuraCalendar hijri = new UmAlQuraCalendar();
                var nextYear = hijri.GetYear(DateTime.Today) + 1;
                details.PolicyholderIDExpiry = "01-" + nextYear;
            }

            details.PurposeofVehicleUseID = quotation.VehicleUseCode;
            details.QuoteRequestSourceID = 1; //1 refer to (Aggregator)
            details.FullName = $"{quotation.InsuredFirstNameEn} {quotation.InsuredMiddleNameEn} {quotation.InsuredLastNameEn}";
            details.ArabicFirstName = quotation.InsuredFirstNameAr;
            details.ArabicMiddleName = quotation.InsuredMiddleNameAr;
            details.ArabicLastName = quotation.InsuredLastNameAr;
            details.EnglishFirstName = quotation.InsuredFirstNameEn;
            details.EnglishMiddleName = quotation.InsuredMiddleNameEn;
            details.EnglishLastName = quotation.InsuredLastNameEn;
            //if (quotation.InsuredIdTypeCode == 2)
            //    details.DateOfBirthG = quotation.InsuredBirthDateG;
            //if (quotation.InsuredIdTypeCode == 1)
            //    details.DateOfBirthH = quotation.InsuredBirthDateH;
            details.DateOfBirthG = quotation.InsuredBirthDateG;
            details.DateOfBirthH = quotation.InsuredBirthDateH;
            details.Occupation = quotation.InsuredOccupation;
            details.Cylinders = quotation.VehicleCylinders;
            details.VehicleCapacity = quotation.VehicleCapacity;
            if (quotation.InsuredIdTypeCode == 1 || quotation.InsuredIdTypeCode == 2)
                details.PolicyholderNationalityID = Int16.Parse(quotation.InsuredNationalityCode == "113" ? Nationality.ResourceManager.GetString("Saudi") : quotation.InsuredNationalityCode);
            details.VehicleUniqueTypeID = quotation.VehicleIdTypeCode;
            if (quotation.VehicleIdTypeCode == 1)
                details.VehicleSequenceNumber = quotation.VehicleId;
            else if (quotation.VehicleIdTypeCode == 2)
                details.VehicleCustomID = quotation.VehicleId;
            //details.IsDriverDisabled = quotation.DriverDisabled;
            details.PolicyholderGender = (quotation.InsuredGenderCode == "F") ? 2 : 1;
            if (quotation.InsuredAddressRegionID.HasValue)
                details.VehicleDriveRegionID = (short)quotation.InsuredAddressRegionID;
            short vehicleDriveCityID = 0;
            short.TryParse(quotation.InsuredCityCode, out vehicleDriveCityID);
            details.VehicleDriveCityID = vehicleDriveCityID;
            if (quotation.VehicleIdTypeCode == 1 && !string.IsNullOrEmpty(quotation.VehiclePlateTypeCode))
                details.VehiclePlateTypeID = Int32.Parse(quotation.VehiclePlateTypeCode);
            else
                details.VehiclePlateTypeID = 3;

            if (quotation.VehiclePlateNumber.HasValue)
                details.VehiclePlateNumber = quotation.VehiclePlateNumber.Value;

            //if (!string.IsNullOrEmpty(quotation.VehiclePlateText1))
            //    details.FirstPlateLetterID = Int16.Parse(VehiclePlateLetter.ResourceManager.GetString(quotation.VehiclePlateText1));
            //if (!string.IsNullOrEmpty(quotation.VehiclePlateText2))
            //    details.SecondPlateLetterID = Int16.Parse((VehiclePlateLetter.ResourceManager.GetString(quotation.VehiclePlateText2)));//4; // Int32.Parse(quotation.VehiclePlateText2);
            //if (!string.IsNullOrEmpty(quotation.VehiclePlateText3))
            //    details.ThirdPlateLetterID = Int16.Parse((VehiclePlateLetter.ResourceManager.GetString(quotation.VehiclePlateText3)));//2; // Int32.Parse(quotation.VehiclePlateText3);
            details.FirstPlateLetterID = quotation.WataniyaFirstPlateLetterID;
            details.SecondPlateLetterID = quotation.WataniyaSecondPlateLetterID;
            details.ThirdPlateLetterID = quotation.WataniyaThirdPlateLetterID;

            details.ChildrenBelow16 = quotation.InsuredChildrenBelow16Years;
            if (quotation.InsuredEducationCode.HasValue)
            {
                var educationEnumString = Enum.GetName(typeof(EducationAr), quotation.InsuredEducationCode);
                if (!string.IsNullOrEmpty(educationEnumString))
                    details.Education = WataniyaEducation.ResourceManager.GetString(educationEnumString);
            }
            int maritalStatus = 0;
            int.TryParse(quotation.InsuredSocialStatusCode, out maritalStatus);
            details.MaritalStatus = maritalStatus;
            details.PolicyholderNCDCode = quotation.NCDFreeYears;
            details.PolicyholderNCDReference = quotation.NCDReference;
            details.Vehicle360Camera = (quotation.CameraTypeId == null) ? 3 : ((quotation.CameraTypeId == 4) ? 1 : 2);
            details.VehicleABS = (quotation.BrakeSystemId == null) ? 3 : ((quotation.BrakeSystemId == 2) ? 1 : 2);
            details.VehicleAdaptiveCruiseControl = (quotation.BrakeSystemId == null) ? 3 : ((quotation.BrakeSystemId == 3) ? 1 : 2);
            details.VehicleAntitheftAlarm = (quotation.HasAntiTheftAlarm == null) ? 3 : ((quotation.HasAntiTheftAlarm == true) ? 1 : 2);
            details.VehicleAutoBraking = (quotation.BrakeSystemId == null) ? 3 : ((quotation.BrakeSystemId == 3) ? 1 : 2);
            details.VehicleCruiseControl = (quotation.BrakeSystemId == null) ? 3 : ((quotation.BrakeSystemId == 2) ? 1 : 2);
            details.VehicleExpectedMileageYear = (quotation.VehicleMileageExpectedAnnualCode == 1)
                ? 20000 : ((quotation.VehicleMileageExpectedAnnualCode == 2) ? 40000 : 60000);
            details.VehicleFrontCamera = (quotation.CameraTypeId == null) ? 3 : ((quotation.CameraTypeId == 3) ? 1 : 2);
            details.VehicleFrontSensors = (quotation.ParkingSensorId == null) ? 3 : ((quotation.ParkingSensorId == 3) ? 1 : 2);
            details.VehicleNightParking = quotation.VehicleOvernightParkingLocationCode;
            details.VehicleRearCamera = (quotation.CameraTypeId == null) ? 3 : ((quotation.CameraTypeId == 2) ? 1 : 2);
            details.VehicleRearSensors = (quotation.ParkingSensorId == null) ? 3 : ((quotation.ParkingSensorId == 2) ? 1 : 2);
            if (quotation.VehicleTransmissionTypeCode.HasValue)
            {
                if(quotation.VehicleTransmissionTypeCode==1)
                    details.VehicleTransmission = 2;
                if(quotation.VehicleTransmissionTypeCode==2)
                    details.VehicleTransmission = 1;
            }
            int WorkCityID = 0;
            int.TryParse(quotation.InsuredWorkCityCode, out WorkCityID);
            details.WorkCityID = WorkCityID;
            details.VehicleEngineSizeCC = quotation.VehicleEngineSizeCode;
            short vehicleMakerCode = 0;
            short.TryParse(quotation.VehicleMakerCode, out vehicleMakerCode);
            details.VehicleMakeCodeNIC = vehicleMakerCode;

            short wataniyaVehicleMakerCode = 0;
            short.TryParse(quotation.WataniyaVehicleMakerCode, out wataniyaVehicleMakerCode);
            details.VehicleMakeCode = wataniyaVehicleMakerCode;

            details.VehicleMakeTextNIC = quotation.VehicleMaker;
            short VehicleModelCode = 0;
            short.TryParse(quotation.VehicleModelCode, out VehicleModelCode);
            details.VehicleModelCodeNIC = VehicleModelCode;

            short wataniyaVehicleModelCode = 0;
            short.TryParse(quotation.WataniyaVehicleModelCode, out wataniyaVehicleModelCode);
            details.VehicleModelCode = wataniyaVehicleModelCode;

            details.VehicleModelTextNIC = quotation.VehicleModel;
            //////HandleTPLMakerAndModel(details, quotation);
            details.ManufactureYear = quotation.VehicleModelYear;
            //details.VehicleColorCode = HandleVehicleColorId(quotation.VehicleMajorColor);//    Int16.Parse(quotation.VehicleMajorColorCode);
            if (!string.IsNullOrEmpty(quotation.VehicleMajorColorCode))
                details.VehicleColorCode = short.Parse(quotation.VehicleMajorColorCode);
            else
                details.VehicleColorCode = 99;

            short vehicleRegPlaceCode = 0;
            short.TryParse(quotation.VehicleRegPlaceCode, out vehicleRegPlaceCode);
            details.VehicleRegistrationCityCode = vehicleRegPlaceCode;

            details.VehicleVIN = quotation.VehicleChassisNumber;
            details.VehicleRegistrationExpiryDate = quotation.VehicleRegExpiryDate;
            var milliSeconds = quotation.PolicyEffectiveDate.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            details.PolicyEffectiveDate = "/Date(" + milliSeconds.ToString().Substring(0, 13) + ")/";
            details.BuildingNumber = quotation.BuildingNumber;
            details.Street = quotation.Street;
            details.District = quotation.District;
            details.City = quotation.City;
            details.ZipCode = quotation.ZipCode;
            details.AdditionalNumber = quotation.AdditionalNumber;
            //////HandleTPLQuotationAddressFields(details);
            details.VehicleWeight = quotation.VehicleWeight;
            short vehicleBodyCode = 0;
            Int16.TryParse(quotation.VehicleBodyTypeCode, out vehicleBodyCode);
            details.VehicleBodyCode = vehicleBodyCode;
            if (quotation.InsuredId.ToString().StartsWith("7"))
            {
                var mainDriver = quotation.Drivers.Where(a => a.DriverTypeCode == 1).FirstOrDefault();
                if (mainDriver != null)
                {
                    if (quotation.InsuredIdTypeCode == 1)
                        details.CoverAgeLimitID = HandleQuotationCoverAgeLimitID(DateTime.ParseExact(mainDriver.DriverBirthDate, "dd-MM-yyyy", new CultureInfo("en-US")));
                    else
                        details.CoverAgeLimitID = HandleQuotationCoverAgeLimitID(DateTime.ParseExact(HandleDriverBirthDateG(mainDriver.DriverBirthDateG), "dd-MM-yyyy", new CultureInfo("en-US")));
                }
            }
            else
            {
                if (quotation.InsuredIdTypeCode == 1)
                    details.CoverAgeLimitID = HandleQuotationCoverAgeLimitID(DateTime.ParseExact(quotation.InsuredBirthDateH, "dd-MM-yyyy", new CultureInfo("en-US")));
                else
                    details.CoverAgeLimitID = HandleQuotationCoverAgeLimitID(DateTime.ParseExact(quotation.InsuredBirthDateG, "dd-MM-yyyy", new CultureInfo("en-US")));
            }
            //if (details.CoverAgeLimitID >= 6)
            details.DriverDetails = GetDrivers(quotation);
            if (quotation.IsRenewal.HasValue)
                details.IsRenewal = quotation.IsRenewal.Value;
            else
                details.IsRenewal = false;

            // new properties maapping
            details.VehicleAxleWeight = quotation.VehicleAxleWeight;
            details.VehicleFireExtinguisher = (quotation.HasFireExtinguisher.HasValue && quotation.HasFireExtinguisher.Value) ? 1 : 2;
            details.VehicleMileage = quotation.VehicleMileage;
            details.VehicleModifications = quotation.VehicleModificationDetails;

            if (!string.IsNullOrEmpty(quotation.MobileNo))
                details.MobileNo = quotation.MobileNo;
            else
                details.MobileNo = "";

            details.VehicleMakeCodeNajm = 0;
            details.VehicleModelCodeNajm = 0;
            details.WorkCompanyName = "";
            //details.IsPrimaryDriver = (quotation.Drivers.Count > 0) ? false : true;
            //details.IsScheme = 
            //details.SchemeDetails = 

            HandleTPLQuotationCustomizedParameter(quotation, details);

            return details;
        }

        private QuotationServiceResponse QuotationResponseObjMappingTpl(WataniyaTplQuotationResponseDto quotationServiceResponse, int deductibleValue)
        {
            var response = new QuotationServiceResponse();

            if (quotationServiceResponse == null)
                return response;

            // as per fayssal
            response.ReferenceId = quotationServiceResponse.RequestReferenceNo;
            response.StatusCode = (quotationServiceResponse.Status) ? 1 : 2;
            response.QuotationNo = quotationServiceResponse.QuoteReferenceNo.ToString();
            response.QuotationDate = DateTime.Now.ToString();
            response.QuotationExpiryDate = DateTime.Now.AddDays(1).ToString();
            if (quotationServiceResponse.Details != null)
                response.Products = GetWataniyaTplProducts(quotationServiceResponse, deductibleValue);

            if (quotationServiceResponse.errors != null && quotationServiceResponse.errors.Count > 0)
                response.Errors = HandleWataniyaQuotationResponseErrors(quotationServiceResponse.errors);

            return response;
        }

        private List<ProductDto> GetWataniyaTplProducts(WataniyaTplQuotationResponseDto quotationServiceResponse, int deductibleValue)
        {
            List<ProductDto> products = new List<ProductDto>();
            var product = new ProductDto()
            {
                ProductId = deductibleValue.ToString(),
                InsuranceTypeCode = 1,
                ProductNameAr = "تأمين مركبات طرف ثالث(ضد الغير)",
                ProductNameEn = "Third Part Liability",
                DeductibleValue = deductibleValue,
                Benefits = GetTplProductBenfits(quotationServiceResponse.Details.PolicyFeatures),
            };

            var basicPremiumAfterDiscount = quotationServiceResponse.Details.PolicyAmount;
            var vat = Math.Round(basicPremiumAfterDiscount * (decimal).15, 2);
            var basicPremiumBeforeDiscount = quotationServiceResponse.Details.PremiumBreakDown.Where(a => a.BreakDownTypeId == 2).FirstOrDefault(); // as per MUbarak Al Mutlak 16-9-2021
            product.PriceDetails = GetProductPriceDetails(quotationServiceResponse.Details.Discounts, (basicPremiumBeforeDiscount != null ? basicPremiumBeforeDiscount.BreakDownAmount : 0), vat);
            product.PolicyPremium = quotationServiceResponse.Details.PolicyAmount;
            product.ProductPrice = quotationServiceResponse.Details.PolicyAmount + vat; // quotationServiceResponse.PolicyTaxableAmount

            products.Add(product);

            return products;
        }

        private List<BenefitDto> GetTplProductBenfits(List<int> policyFeatures)
        {
            List<BenefitDto> benfits = new List<BenefitDto>();
            foreach (var feature in policyFeatures)
            {
                var selectedBenfit = BenefitsLUT.Where(a => a.BenefitId == feature)?.FirstOrDefault();
                if (selectedBenfit != null)
                {
                    var benfitModel = new BenefitDto(0, 0);
                    benfitModel.BenefitId = feature.ToString();
                    benfitModel.BenefitDescAr = benfitModel.BenefitNameAr = selectedBenfit.BenefitDescAr;
                    benfitModel.BenefitDescEn = benfitModel.BenefitNameEn = selectedBenfit.BenefitDescEn;
                    benfitModel.IsSelected = benfitModel.IsReadOnly = true;
                    benfits.Add(benfitModel);
                }
            }
            return benfits;
        }

        //private void HandleTPLQuotationAddressFields(WataniyaTplQuotationRequesDetailstDto model)
        //{
        //    var addressService = EngineContext.Current.Resolve<IAddressService>();
        //    var address = addressService.GetAddressesByNin(model.PolicyHolderID.ToString());
        //    if (address != null && !string.IsNullOrEmpty(address.City))
        //    {
        //        model.Street = string.IsNullOrEmpty(address.Street) ? "" : address.Street;
        //        model.District = string.IsNullOrEmpty(address.District) ? "" : address.District;
        //        model.City = string.IsNullOrEmpty(address.City) ? "" : address.City;
        //        if (!string.IsNullOrEmpty(address.BuildingNumber))
        //        {
        //            int buildingNumber = 0;
        //            int.TryParse(address.BuildingNumber, out buildingNumber);
        //            model.BuildingNumber = buildingNumber;
        //        }
        //        if (!string.IsNullOrEmpty(address.PostCode))
        //        {
        //            int postCode = 0;
        //            int.TryParse(address.PostCode, out postCode);
        //            model.ZipCode = postCode;
        //        }
        //        if (!string.IsNullOrEmpty(address.AdditionalNumber))
        //        {
        //            int additionalNumber = 0;
        //            int.TryParse(address.AdditionalNumber, out additionalNumber);
        //            model.AdditionalNumber = additionalNumber;
        //        }
        //    }
        //}

        private void HandleTPLQuotationCustomizedParameter(QuotationServiceRequest quotation, WataniyaTplQuotationRequesDetailstDto details)
        {
            var customizeParams = new List<CustomizedParameter>();
            if (quotation.ManualEntry == "false")
            {
                customizeParams.Add(new CustomizedParameter() { Key = "ModelSource", Value1 = "1", Value2 = "", Value3 = "", Value4 = "" });
                customizeParams.Add(new CustomizedParameter() { Key = "MakeSource", Value1 = "1", Value2 = "", Value3 = "", Value4 = "" });
                customizeParams.Add(new CustomizedParameter() { Key = "ManufactureYearSource", Value1 = "1", Value2 = "", Value3 = "", Value4 = "" });
                details.CustomizedParameter = customizeParams;
            }
            else
            {
                List<YakeenMissingField> missingFields = JsonConvert.DeserializeObject<List<YakeenMissingField>>(quotation.MissingFields);
                foreach (var missingField in missingFields)
                {
                    customizeParams.Add(new CustomizedParameter() { Key = missingField.Key, Value1 = "2", Value2 = "", Value3 = "", Value4 = "" });
                }

                if (!customizeParams.Any(a => a.Key.ToLower().Contains("VehicleModel")))
                    customizeParams.Add(new CustomizedParameter() { Key = "ModelSource", Value1 = "1", Value2 = "", Value3 = "", Value4 = "" });

                if (!customizeParams.Any(a => a.Key.ToLower().Contains("VehicleMaker")))
                    customizeParams.Add(new CustomizedParameter() { Key = "MakeSource", Value1 = "1", Value2 = "", Value3 = "", Value4 = "" });

                if (!customizeParams.Any(a => a.Key.ToLower().Contains("year")))
                    customizeParams.Add(new CustomizedParameter() { Key = "ManufactureYearSource", Value1 = "1", Value2 = "", Value3 = "", Value4 = "" });
            }

            details.CustomizedParameter = customizeParams;
        }

        #endregion

        #region  Comp(Request / Response)

        protected WataniyaCompQuotationRequestDto MappingWataniyaCompQuotationRequest(QuotationServiceRequest quotation)
        {
            var wataniyaQuotation = new WataniyaCompQuotationRequestDto();
            wataniyaQuotation.Details = HandleCompRequestDetails(quotation);
            //wataniyaQuotation.RequestReferenceNo = CreateWataniyaReference();
            wataniyaQuotation.RequestReferenceNo = quotation.ReferenceId;
            wataniyaQuotation.InsuranceTypeID = quotation.ProductTypeCode;
            wataniyaQuotation.InsuranceCompanyCode = (short)quotation.InsuranceCompanyCode;
            if (!string.IsNullOrEmpty(quotation.PromoCode))
            {
                wataniyaQuotation.Details.IsScheme = true;
                wataniyaQuotation.Details.SchemeDetails = new List<SchemeDetails>() { new SchemeDetails() { SchemeRef = quotation.PromoCode, IcSchemeRef = quotation.PromoCode } };
            }
            return wataniyaQuotation;
        }

        protected WataniyaCompQuotationRequesDetailstDto HandleCompRequestDetails(QuotationServiceRequest quotation)
        {
            var details = new WataniyaCompQuotationRequesDetailstDto();

            details.PolicyholderIdentityTypeCode = quotation.InsuredIdTypeCode;
            details.PolicyHolderID = quotation.InsuredId;
            if (!string.IsNullOrEmpty(quotation.IdExpiryDate))
            {
                var IdExpirySplits = quotation.IdExpiryDate.Split('-');
                details.PolicyholderIDExpiry = IdExpirySplits[1] + "-" + IdExpirySplits[2];
            }
            else
            {
                UmAlQuraCalendar hijri = new UmAlQuraCalendar();
                var nextYear = hijri.GetYear(DateTime.Today) + 1;
                details.PolicyholderIDExpiry = "01-" + nextYear;
            }
            details.PurposeofVehicleUseID = quotation.VehicleUseCode;
            details.QuoteRequestSourceID = 1; //1 refer to (Aggregator)
            details.FullName = $"{quotation.InsuredFirstNameEn} {quotation.InsuredMiddleNameEn} {quotation.InsuredLastNameEn}";
            details.ArabicFirstName = quotation.InsuredFirstNameAr;
            details.ArabicMiddleName = quotation.InsuredMiddleNameAr;
            details.ArabicLastName = quotation.InsuredLastNameAr;
            details.EnglishFirstName = quotation.InsuredFirstNameEn;
            details.EnglishMiddleName = quotation.InsuredMiddleNameEn;
            details.EnglishLastName = quotation.InsuredLastNameEn;
            details.DateOfBirthG = quotation.InsuredBirthDateG;
            details.DateOfBirthH = quotation.InsuredBirthDateH;
            details.Occupation = quotation.InsuredOccupation;
            details.Cylinders = quotation.VehicleCylinders;
            details.VehicleCapacity = quotation.VehicleCapacity;
            if (quotation.InsuredIdTypeCode == 1 || quotation.InsuredIdTypeCode == 2)
                details.PolicyholderNationalityID = Int16.Parse(quotation.InsuredNationalityCode == "113" ? Nationality.ResourceManager.GetString("Saudi") : quotation.InsuredNationalityCode);
            details.VehicleUniqueTypeID = quotation.VehicleIdTypeCode;
            if (quotation.VehicleIdTypeCode == 1)
                details.VehicleSequenceNumber = quotation.VehicleId;
            else if (quotation.VehicleIdTypeCode == 2)
                details.VehicleCustomID = quotation.VehicleId;
            details.PolicyholderGender = (quotation.InsuredGenderCode == "F") ? 2 : 1;
            if (quotation.InsuredAddressRegionID.HasValue)
            {
                short insuredAddressRegionID = 0;
                short.TryParse(quotation.InsuredAddressRegionID.ToString(), out insuredAddressRegionID);
                details.VehicleDriveRegionID = insuredAddressRegionID;
            }

            short insuredCityCode;
            short.TryParse(quotation.InsuredCityCode, out insuredCityCode);
            details.VehicleDriveCityID = insuredCityCode;
            if (quotation.VehicleIdTypeCode == 1 && !string.IsNullOrEmpty(quotation.VehiclePlateTypeCode))
                details.VehiclePlateTypeID = Int32.Parse(quotation.VehiclePlateTypeCode);
            else
                details.VehiclePlateTypeID = 3;

            if (quotation.VehiclePlateNumber.HasValue)
                details.VehiclePlateNumber = quotation.VehiclePlateNumber.Value;
            //if (!string.IsNullOrEmpty(quotation.VehiclePlateText1))
            //    details.FirstPlateLetterID = Int16.Parse((VehiclePlateLetter.ResourceManager.GetString(quotation.VehiclePlateText1)));//Int32.Parse(quotation.VehiclePlateText1);
            //if (!string.IsNullOrEmpty(quotation.VehiclePlateText2))
            //    details.SecondPlateLetterID = Int16.Parse((VehiclePlateLetter.ResourceManager.GetString(quotation.VehiclePlateText2)));//Int32.Parse(quotation.VehiclePlateText2);
            //if (!string.IsNullOrEmpty(quotation.VehiclePlateText3))
            //    details.ThirdPlateLetterID = Int16.Parse((VehiclePlateLetter.ResourceManager.GetString(quotation.VehiclePlateText3)));//Int32.Parse(quotation.VehiclePlateText3);
            details.FirstPlateLetterID = quotation.WataniyaFirstPlateLetterID;
            details.SecondPlateLetterID = quotation.WataniyaSecondPlateLetterID;
            details.ThirdPlateLetterID = quotation.WataniyaThirdPlateLetterID;

            details.ChildrenBelow16 = quotation.InsuredChildrenBelow16Years;
            if (quotation.InsuredEducationCode.HasValue)
            {
                var educationEnumString = Enum.GetName(typeof(EducationAr), quotation.InsuredEducationCode);
                details.Education = WataniyaEducation.ResourceManager.GetString(educationEnumString);
            }
            int insuredSocialStatusCode = 0;
            int.TryParse(quotation.InsuredSocialStatusCode, out insuredSocialStatusCode);
            details.MaritalStatus = insuredSocialStatusCode;
            details.PolicyholderNCDCode = quotation.NCDFreeYears;
            details.PolicyholderNCDReference = quotation.NCDReference;
            details.Vehicle360Camera = (quotation.CameraTypeId == null) ? 3 : ((quotation.CameraTypeId == 4) ? 1 : 2);
            details.VehicleABS = (quotation.BrakeSystemId == null) ? 3 : ((quotation.BrakeSystemId == 2) ? 1 : 2);
            details.VehicleAdaptiveCruiseControl = (quotation.BrakeSystemId == null) ? 3 : ((quotation.BrakeSystemId == 3) ? 1 : 2);
            details.VehicleAntitheftAlarm = (quotation.HasAntiTheftAlarm == null) ? 3 : ((quotation.HasAntiTheftAlarm == true) ? 1 : 2);
            details.VehicleAutoBraking = (quotation.BrakeSystemId == null) ? 3 : ((quotation.BrakeSystemId == 3) ? 1 : 2);
            details.VehicleCruiseControl = (quotation.BrakeSystemId == null) ? 3 : ((quotation.BrakeSystemId == 2) ? 1 : 2);
            details.VehicleExpectedMileageYear = (quotation.VehicleMileageExpectedAnnualCode == 1) ? 20000 : (quotation.VehicleMileageExpectedAnnualCode == 2) ? 40000 : 40000;
            details.VehicleFrontCamera = (quotation.CameraTypeId == null) ? 3 : ((quotation.CameraTypeId == 3) ? 1 : 2);
            details.VehicleFrontSensors = (quotation.ParkingSensorId == null) ? 3 : ((quotation.ParkingSensorId == 3) ? 1 : 2);
            details.VehicleNightParking = quotation.VehicleOvernightParkingLocationCode;
            details.VehicleRearCamera = (quotation.CameraTypeId == null) ? 3 : ((quotation.CameraTypeId == 2) ? 1 : 2);
            details.VehicleRearSensors = (quotation.ParkingSensorId == null) ? 3 : ((quotation.ParkingSensorId == 2) ? 1 : 2);
           if (quotation.VehicleTransmissionTypeCode.HasValue)
            {
                if(quotation.VehicleTransmissionTypeCode==1)
                    details.VehicleTransmission = 2;
                if(quotation.VehicleTransmissionTypeCode==2)
                    details.VehicleTransmission = 1;
            }
            int insuredWorkCityCode = 0;
            int.TryParse(quotation.InsuredWorkCityCode, out insuredWorkCityCode);
            details.WorkCityID = insuredWorkCityCode;
            details.VehicleEngineSizeCC = quotation.VehicleEngineSizeCode;
            details.VehicleColorText = quotation.VehicleMajorColor;
            details.VehicleModifications = quotation.VehicleModificationDetails;
            short VehicleMakerCode = 0;
            short.TryParse(quotation.VehicleMakerCode, out VehicleMakerCode);
            details.VehicleMakeCodeNIC = VehicleMakerCode;
            details.VehicleMakeTextNIC = quotation.VehicleMaker;
            details.VehicleModelCodeNIC = Int16.Parse(quotation.VehicleModelCode);
            details.VehicleModelTextNIC = quotation.VehicleModel;

            short wataniyaVehicleMakerCode = 0;
            short.TryParse(quotation.WataniyaVehicleMakerCode, out wataniyaVehicleMakerCode);
            details.VehicleMakeCode = wataniyaVehicleMakerCode;

            short wataniyaVehicleModelCode = 0;
            short.TryParse(quotation.WataniyaVehicleModelCode, out wataniyaVehicleModelCode);
            details.VehicleModelCode = wataniyaVehicleModelCode;
            //////HandleCOMPMakerAndModel(details, quotation);

            details.ManufactureYear = quotation.VehicleModelYear;
            //details.VehicleColorCode = HandleVehicleColorId(quotation.VehicleMajorColor); //Int16.Parse(Colors.ResourceManager.GetString(quotation.VehicleMajorColor));//Int16.Parse(quotation.VehicleMajorColorCode);
            if (!string.IsNullOrEmpty(quotation.VehicleMajorColorCode))
                details.VehicleColorCode = short.Parse(quotation.VehicleMajorColorCode);
            else
                details.VehicleColorCode = 99;

            short vehicleRegPlaceCode;
            short.TryParse(quotation.VehicleRegPlaceCode, out vehicleRegPlaceCode);
            details.VehicleRegistrationCityCode = vehicleRegPlaceCode;
            details.VehicleVIN = quotation.VehicleChassisNumber;
            details.VehicleRegistrationExpiryDate = quotation.VehicleRegExpiryDate;

            // handle day to start with "0" if it <= 9
            var dayLength = quotation.PolicyEffectiveDate.Day.ToString().Length;
            var day = (dayLength > 1) ? quotation.PolicyEffectiveDate.Day.ToString() : "0" + quotation.PolicyEffectiveDate.Day;
            // handle month to start with "0" if it <= 9
            var monthLength = quotation.PolicyEffectiveDate.Month.ToString().Length;
            var month = (monthLength > 1) ? quotation.PolicyEffectiveDate.Month.ToString() : "0" + quotation.PolicyEffectiveDate.Month;
            details.PolicyEffectiveDate = $"{ quotation.PolicyEffectiveDate.Year }-{ month }-{ day }";

            details.BuildingNumber = quotation.BuildingNumber;
            details.Street = quotation.Street;
            details.District = quotation.District;
            details.City = quotation.City;
            details.ZipCode = quotation.ZipCode;
            details.AdditionalNumber = quotation.AdditionalNumber;
            //////HandleCOMPQuotationAddressFields(details);

            details.VehicleWeight = quotation.VehicleWeight;
            short vehicleBodyTypeCode = 0;
            short.TryParse(quotation.VehicleBodyTypeCode, out vehicleBodyTypeCode);
            details.VehicleBodyCode = vehicleBodyTypeCode;
            if (quotation.InsuredId.ToString().StartsWith("7"))
            {
                var mainDriver = quotation.Drivers.Where(a => a.DriverTypeCode == 1).FirstOrDefault();
                if (mainDriver != null)
                {
                    if (quotation.InsuredIdTypeCode == 1)
                        details.CoverAgeLimitID = HandleQuotationCoverAgeLimitID(DateTime.ParseExact(mainDriver.DriverBirthDate, "dd-MM-yyyy", new CultureInfo("en-US")));
                    else
                        details.CoverAgeLimitID = HandleQuotationCoverAgeLimitID(DateTime.ParseExact(HandleDriverBirthDateG(mainDriver.DriverBirthDateG), "dd-MM-yyyy", new CultureInfo("en-US")));
                }
            }
            else
            {
                if (quotation.InsuredIdTypeCode == 1)
                    details.CoverAgeLimitID = HandleQuotationCoverAgeLimitID(DateTime.ParseExact(quotation.InsuredBirthDateH, "dd-MM-yyyy", new CultureInfo("en-US")));
                else
                    details.CoverAgeLimitID = HandleQuotationCoverAgeLimitID(DateTime.ParseExact(quotation.InsuredBirthDateG, "dd-MM-yyyy", new CultureInfo("en-US")));
            }
            
            //if (details.CoverAgeLimitID >= 6)
            details.DriverDetails = GetDrivers(quotation);
            details.NajmCaseDetails = GetNajmCaseDetails(quotation);
            details.NCDFreeYears = quotation.NCDFreeYears.ToString();
            details.NCDReference = quotation.NCDReference;
            details.VehicleSumInsured = quotation.VehicleValue.Value;
            details.RepairMethod = (quotation.VehicleAgencyRepair.HasValue && quotation.VehicleAgencyRepair.Value == true) ? 1 : 2;
            if (quotation.IsRenewal.HasValue)
                details.IsRenewal = quotation.IsRenewal.Value;
            else
                details.IsRenewal = false;

            // new properties maapping
            details.VehicleAxleWeight = quotation.VehicleAxleWeight;
            //details.VehicleFireExtinguisher = 
            details.VehicleMileage = quotation.VehicleMileage;
            details.VehicleModifications = quotation.VehicleModificationDetails;
            //details.VehicleUsagePercentage = (Int16)quotation.Drivers.FirstOrDefault(a => a.DriverId == quotation.InsuredId)?.DriverDrivingPercentage.Value;

            // keep all blew properties null as fayssal
            details.VehicleMakeCodeNajm = 0;
            details.VehicleModelCodeNajm = 0;
            //details.PolicyTitleID = quotation.
            if (!string.IsNullOrEmpty(quotation.MobileNo))
                details.MobileNo = quotation.MobileNo;
            else
                details.MobileNo = "";

            HandleCOMPQuotationCustomizedParameter(quotation, details);
            return details;
        }

        //private void HandleCOMPMakerAndModel(WataniyaCompQuotationRequesDetailstDto details, QuotationServiceRequest quoteModel)
        //{
        //    var _vehicleService = EngineContext.Current.Resolve<IVehicleService>();

        //    short makerId = 0;
        //    short.TryParse(quoteModel.VehicleMakerCode, out makerId);
        //    long modelId = 0;
        //    long.TryParse(quoteModel.VehicleModelCode, out modelId);
        //    var vehicleModel = _vehicleService.GetVehicleModelByMakerCodeAndModelCode(makerId, modelId);
        //    if (vehicleModel != null)
        //    {
        //        if (vehicleModel.WataniyaMakerCode.HasValue)
        //        {
        //            details.VehicleMakeCode = short.Parse(vehicleModel.WataniyaMakerCode.Value.ToString());
        //            //details.VehicleMakeCodeNIC = short.Parse(vehicleModel.WataniyaMakerCode.Value.ToString());
        //        }
        //        if (vehicleModel.WataniyaModelCode.HasValue)
        //        {
        //            details.VehicleModelCode = short.Parse(vehicleModel.WataniyaModelCode.Value.ToString());
        //            //details.VehicleModelCodeNIC = short.Parse(vehicleModel.WataniyaModelCode.Value.ToString());
        //        }
        //    }
        //}

        private QuotationServiceResponse QuotationResponseObjMappingComprehensive(WataniyaCompQuotationResponseDto quotationServiceResponse)
        {
            var response = new QuotationServiceResponse();

            if (quotationServiceResponse == null)
                return response;

            // sa per fayssal
            response.ReferenceId = quotationServiceResponse.RequestReferenceNo;
            response.StatusCode = (quotationServiceResponse.Status) ? 1 : 2;
            response.QuotationNo = quotationServiceResponse.QuoteReferenceNo.ToString();
            response.QuotationDate = DateTime.Now.ToString();
            response.QuotationExpiryDate = DateTime.Now.AddDays(1).ToString();

            if (quotationServiceResponse.Details != null && quotationServiceResponse.Details.Deductibles.Any())
                response.Products = GetWataniyaComprehensiveProducts(quotationServiceResponse);

            if (quotationServiceResponse.errors != null && quotationServiceResponse.errors.Any())
                response.Errors = HandleWataniyaQuotationResponseErrors(quotationServiceResponse.errors);

            return response;
        }

        private List<ProductDto> GetWataniyaComprehensiveProducts(WataniyaCompQuotationResponseDto quotationServiceResponse)
        {
            List<ProductDto> products = new List<ProductDto>();
            foreach (var deductible in quotationServiceResponse.Details.Deductibles)
            {
                var product = new ProductDto()
                {
                    //ProductId = deductible.DeductibleReferenceNo,
                    ProductId = deductible.DeductibleAmount.ToString(),
                    InsuranceTypeCode = 2,
                    ProductNameAr = "تأمين مركبات شامل",
                    ProductNameEn = "Comprehensive Vehicle Insurance",
                    DeductibleValue = deductible.DeductibleAmount,
                    Benefits = GetProductBenfits(quotationServiceResponse.Details.PolicyPremiumFeatures),
                };

                var basicPremiumAfterDiscount = deductible.PolicyPremium;
                var vat = Math.Round(basicPremiumAfterDiscount * (decimal).15, 2);
                var basicPremiumBeforeDiscount = deductible.PremiumBreakDown.Where(a => a.BreakDownTypeId == 2).FirstOrDefault(); // as per MUbarak Al Mutlak 16-9-2021
                product.PriceDetails = GetProductPriceDetails(deductible.Discounts, (basicPremiumBeforeDiscount != null ? basicPremiumBeforeDiscount.BreakDownAmount : 0), vat);
                product.PolicyPremium = deductible.PolicyPremium;
                product.ProductPrice = deductible.PolicyPremium + vat;

                products.Add(product);
            }
            return products;
        }

        //private void HandleCOMPQuotationAddressFields(WataniyaCompQuotationRequesDetailstDto model)
        //{
        //    var addressService = EngineContext.Current.Resolve<IAddressService>();
        //    var address = addressService.GetAddressesByNin(model.PolicyHolderID.ToString());
        //    if (address != null && !string.IsNullOrEmpty(address.City))
        //    {
        //        model.Street = string.IsNullOrEmpty(address.Street) ? "" : address.Street;
        //        model.District = string.IsNullOrEmpty(address.District) ? "" : address.District;
        //        model.City = string.IsNullOrEmpty(address.City) ? "" : address.City;
        //        if (!string.IsNullOrEmpty(address.BuildingNumber))
        //            model.BuildingNumber = int.Parse(address.BuildingNumber);
        //        if (!string.IsNullOrEmpty(address.PostCode))
        //            model.ZipCode = int.Parse(address.PostCode);
        //        if (!string.IsNullOrEmpty(address.AdditionalNumber))
        //            model.AdditionalNumber = int.Parse(address.AdditionalNumber);
        //    }
        //}

        private void HandleCOMPQuotationCustomizedParameter(QuotationServiceRequest quotation, WataniyaCompQuotationRequesDetailstDto details)
        {
            var customizeParams = new List<CustomizedParameter>();
            if (quotation.ManualEntry == "false")
            {
                customizeParams.Add(new CustomizedParameter() { Key = "ModelSource", Value1 = "1", Value2 = "", Value3 = "", Value4 = "" });
                customizeParams.Add(new CustomizedParameter() { Key = "MakeSource", Value1 = "1", Value2 = "", Value3 = "", Value4 = "" });
                customizeParams.Add(new CustomizedParameter() { Key = "ManufactureYearSource", Value1 = "1", Value2 = "", Value3 = "", Value4 = "" });
                details.CustomizedParameter = customizeParams;
            }
            else
            {
                List<YakeenMissingField> missingFields = JsonConvert.DeserializeObject<List<YakeenMissingField>>(quotation.MissingFields);
                foreach (var missingField in missingFields)
                {
                    customizeParams.Add(new CustomizedParameter() { Key = missingField.Key, Value1 = "2", Value2 = "", Value3 = "", Value4 = "" });

                    if (!customizeParams.Any(a => a.Key.ToLower().Contains("VehicleModel")))
                        customizeParams.Add(new CustomizedParameter() { Key = "ModelSource", Value1 = "1", Value2 = "", Value3 = "", Value4 = "" });

                    if (!customizeParams.Any(a => a.Key.ToLower().Contains("VehicleMaker")))
                        customizeParams.Add(new CustomizedParameter() { Key = "MakeSource", Value1 = "1", Value2 = "", Value3 = "", Value4 = "" });

                    if (!customizeParams.Any(a => a.Key.ToLower().Contains("year")))
                        customizeParams.Add(new CustomizedParameter() { Key = "ManufactureYearSource", Value1 = "1", Value2 = "", Value3 = "", Value4 = "" });
                }
            }

            details.CustomizedParameter = customizeParams;
        }

        #endregion


        private int HandleQuotationCoverAgeLimitID(DateTime birthdate, bool isHijriDate = false)
        {
            int driverAge = 0;
            int coverAgeLimitID = 0;

            // Save today's date.
            var today = DateTime.Today;
            UmAlQuraCalendar hijri = new UmAlQuraCalendar();
            // Calculate the age.
            // 1 if driver is citizen
            if (!isHijriDate)
                driverAge = today.Year - birthdate.Year;
            else
                driverAge = hijri.GetYear(today) - hijri.GetYear(birthdate);


            if (driverAge <= 17)
                coverAgeLimitID = 9;
            else if (driverAge == 18)
                coverAgeLimitID = 8;
            else if (driverAge == 19)
                coverAgeLimitID = 7;
            else if (driverAge == 20)
                coverAgeLimitID = 6;
            else if (driverAge == 21)
                coverAgeLimitID = 5;
            else if (driverAge == 22)
                coverAgeLimitID = 4;
            else if (driverAge == 23)
                coverAgeLimitID = 3;
            else if (driverAge == 24)
                coverAgeLimitID = 2;
            else if (driverAge >= 25)
                coverAgeLimitID = 1;

            return coverAgeLimitID;
        }

        private List<DriverDetails> GetDrivers(QuotationServiceRequest request)
        {
            var drivers = new List<DriverDetails>();
            foreach (var driver in request.Drivers) // .Where(a => a.DriverId != request.InsuredId)
            {
                if (!driver.DriverDrivingPercentage.HasValue || driver.DriverDrivingPercentage == null || driver.DriverDrivingPercentage.Value == 0)
                    continue;

                var driverModel = new DriverDetails();
                driverModel.DriverID = driver.DriverId;
                driverModel.DriverName = $"{driver.DriverFirstNameAr} {driver.DriverMiddleNameAr} {driver.DriverLastNameAr}";
                driverModel.DriverGender = (driver.DriverGenderCode == "M") ? 1 : ((driver.DriverGenderCode == "F") ? 2 : 1);
                driverModel.DriverDateOfBirthG = (driver.DriverId.ToString().StartsWith("2")) ? HandleDriverBirthDateG(driver.DriverBirthDateG) : null;
                driverModel.DriverDateOfBirthH = (driver.DriverId.ToString().StartsWith("1")) ? driver.DriverBirthDate : null;
                driverModel.DriverChildrenBelow16 = driver.DriverChildrenBelow16Years;
                var educationEnumString = Enum.GetName(typeof(EducationAr), driver.DriverEducationCode);
                driverModel.DriverEducation = WataniyaEducation.ResourceManager.GetString(educationEnumString);
                //int driverSocialStatusCode = 0;
                //int.TryParse(driver.DriverSocialStatusCode, out driverSocialStatusCode);
                //driverModel.DriverMaritalStatus = driverSocialStatusCode;
                driverModel.DriverMaritalStatus = HandleDriverMaritalStatus(driver.DriverSocialStatusCode);
                driverModel.DriverNCDCode = driver.DriverNCDFreeYears;
                driverModel.DriverNCDReference = driver.DriverNCDReference;
                driverModel.DriverNoOfAccidents = (driver.DriverIdTypeCode == 1) ? request.NoOfAccident : 0;
                driverModel.DriverNoOfClaims = (driver.DriverIdTypeCode == 1) ? request.NoOfAccident : null;
                driverModel.DriverOccupation = driver.DriverOccupation;
                //driverModel.DriverRelation = driver.DriverRelationship;
                driverModel.IsPolicyHolder = (driver.DriverId == request.InsuredId) ? true : false;
                driverModel.DriverHomeAddressCity = driver.DriverHomeCity;

                if (driver.DriverLicenses != null && driver.DriverLicenses.Count > 0)
                {
                    if (driver.DriverLicenses.Any(a => a.DriverLicenseTypeCode == "3"))
                    {
                        var license = driver.DriverLicenses.Where(a => a.DriverLicenseTypeCode == "3").FirstOrDefault();
                        driverModel.DriverLicenseOwnYears = license.LicenseNumberYears;
                        driverModel.DriverLicenseType = int.Parse(license.DriverLicenseTypeCode);
                    }

                    else
                    {
                        var license = driver.DriverLicenses.FirstOrDefault();
                        driverModel.DriverLicenseOwnYears = license.LicenseNumberYears;
                        driverModel.DriverLicenseType = int.Parse(license.DriverLicenseTypeCode);
                    }
                }

                if (!string.IsNullOrEmpty(driver.DriverHomeCityCode) && !string.IsNullOrEmpty(request.InsuredCityCode))
                    driverModel.IsSamePolicyholderAddress = (int.Parse(driver.DriverHomeCityCode) == int.Parse(request.InsuredCityCode)) ? true : false;
                else
                    driverModel.IsSamePolicyholderAddress = false;

                driverModel.VehicleUsagePercentage = (driver.DriverDrivingPercentage.HasValue) ? driver.DriverDrivingPercentage.Value : 0;
                driverModel.IsUser = (request.IsUser.HasValue && request.IsUser.Value) ? true : false;

                //var address = GetDriverAddressByDriverNin(driver.DriverId.ToString());
                //if (address != null)
                //{
                //    driverModel.DriverHomeAddress = address.BuildingNumber + " " + address.AdditionalNumber + " " + address.PostCode + " " + address.City;
                //}
                driverModel.DriverHomeAddress = driver.DriverHomeAddress;

                int driverWorkCityCode = 0;
                int.TryParse(driver.DriverWorkCityCode, out driverWorkCityCode);
                driverModel.DriverWorkCityID = driverWorkCityCode;

                //driverModel.CountriesValidDrivingLicense = 0;
                driverModel.DriverWorkCompanyName = "test";

                drivers.Add(driverModel);
            }

            return drivers;
        }

        private int HandleDriverMaritalStatus(string driverSocialStatusCode)
        {

            int statusCode = 0;
            switch (driverSocialStatusCode)
            {
                case "1":
                case "3":
                    statusCode = 1;
                    break;

                case "2":
                case "4":
                    statusCode = 2;
                    break;

                case "5":
                    statusCode = 3;
                    break;

                case "6":
                    statusCode = 5;
                    break;

                case "7":
                    statusCode = 6;
                    break;

                default:
                    break;
            }

            return statusCode;
        }

        private string HandleDriverBirthDateG(DateTime date)
        {
            string birthDate = string.Empty;
            var year = date.Year;
            var month = (date.Month.ToString().Length == 1) ? "0" + date.Month : date.Month.ToString();
            var day = (date.Day.ToString().Length == 1) ? "0" + date.Day : date.Day.ToString();
            return $"{day}-{month}-{year}";
        }

        private Address GetDriverAddressByDriverNin(string nin)
        {
            var addressService = EngineContext.Current.Resolve<IAddressService>();
            var address = addressService.GetAddressesByNin(nin);
            if (address == null)
                return null;

            return address;
        }

        //private void HandleTPLMakerAndModel(WataniyaTplQuotationRequesDetailstDto details, QuotationServiceRequest quoteModel)
        //{
        //    var _vehicleService = EngineContext.Current.Resolve<IVehicleService>();

        //    var makerId = short.Parse(quoteModel.VehicleMakerCode);
        //    var modelId = long.Parse(quoteModel.VehicleModelCode);
        //    var vehicleModel = _vehicleService.GetVehicleModelByMakerCodeAndModelCode(makerId, modelId);
        //    if (vehicleModel != null)
        //    {
        //        if (vehicleModel.WataniyaMakerCode.HasValue)
        //        {
        //            details.VehicleMakeCode = short.Parse(vehicleModel.WataniyaMakerCode.Value.ToString());
        //            //details.VehicleMakeCodeNIC = short.Parse(vehicleModel.WataniyaMakerCode.Value.ToString());
        //        }
        //        if (vehicleModel.WataniyaModelCode.HasValue)
        //        {
        //            details.VehicleModelCode = short.Parse(vehicleModel.WataniyaModelCode.Value.ToString());
        //            //details.VehicleModelCodeNIC = short.Parse(vehicleModel.WataniyaModelCode.Value.ToString());
        //        }
        //    }
        //}

        private List<NajmCaseDetails> GetNajmCaseDetails(QuotationServiceRequest quotation)
        {
            List<NajmCaseDetails> najmCases = null;
            var accidents = quotation.Accidents;


            var driver = quotation.Drivers.FirstOrDefault(a => a.DriverId == quotation.InsuredId);
            if (accidents == null || accidents.Count == 0)
                return najmCases;

            foreach (var accident in accidents)
            {
                var caseModel = new NajmCaseDetails()
                {
                    CaseNumber = accident.CaseNumber,
                    AccidentDate = accident.AccidentDate.ToString(),
                    Liability = accident.Liability.ToString(),
                    DriverAge = accident.DriverAge.ToString(),
                    CarModel = accident.CarModel.FirstOrDefault().ToString(),
                    CarType = accident.CarType,
                    DriverID = accident.DriverID,
                    SequenceNumber = accident.SequenceNumber,
                    OwnerID = accident.OwnerID,
                    EstimatedAmount = accident.EstimatedAmount.FirstOrDefault().ToString(),
                    DamageParts = accident.DamageParts,
                    CauseOfAccident = accident.CauseOfAccident.FirstOrDefault().ToString()
                };
                najmCases.Add(caseModel);
            }
            return najmCases;
        }

        private List<PriceDto> GetProductPriceDetails(List<Discounts> discounts, decimal basicPremium, decimal vat)
        {
            List<PriceDto> priceDetails = new List<PriceDto>();
            priceDetails.Add(new PriceDto()
            {
                PriceTypeCode = 7,
                PriceValue = (!basicPremium.Equals(null) && basicPremium > 0) ? basicPremium : 0,
                PercentageValue = (decimal)0.0
            });

            priceDetails.Add(new PriceDto()
            {
                PriceTypeCode = 8,
                PriceValue = (!vat.Equals(null) && vat > 0) ? vat : 0,
                PercentageValue = (decimal)0.15
            });

            foreach (var discount in discounts)
            {
                priceDetails.Add(new PriceDto()
                {
                    PriceTypeCode = (discount.DiscountTypeId == 99) ? 11 : (discount.DiscountTypeId == 134) ? 1 : discount.DiscountTypeId, // as per Mubarak Al Mutlak   15-9-2021
                    PriceValue = (decimal)discount.DiscountAmount,
                    PercentageValue = (decimal)discount.DiscountPercentage

                });
            }
            return priceDetails;
        }

        private List<BenefitDto> GetProductBenfits(List<PolicyPremiunFeatures> policyPremiunFeatures)
        {
            List<BenefitDto> benfits = new List<BenefitDto>();
            foreach (var feature in policyPremiunFeatures)
            {
                if (feature.FeatureID == 116)
                    continue;

                var selectedBenfit = BenefitsLUT.Where(a => a.BenefitId == feature.FeatureID)?.FirstOrDefault();
                if (selectedBenfit != null)
                {
                    var benfitModel = new BenefitDto(0, feature.FeatureAmount);
                    benfitModel.BenefitId = feature.FeatureID.ToString();
                    benfitModel.BenefitDescAr = benfitModel.BenefitNameAr = selectedBenfit.BenefitDescAr;
                    benfitModel.BenefitDescEn = benfitModel.BenefitNameEn = selectedBenfit.BenefitDescEn;
                    benfitModel.IsSelected = benfitModel.IsReadOnly = (feature.FeatureTypeID == 2) ? true : false;
                    benfits.Add(benfitModel);
                }
            }
            return benfits;
        }

        private List<Error> HandleWataniyaQuotationResponseErrors(List<Errors> Errors)
        {
            List<Error> errors = new List<Error>();
            foreach (var error in Errors)
            {
                var errorModel = new Error()
                {
                    Code = error.code,
                    Field = error.field,
                    Message = error.message
                };
                errors.Add(errorModel);
            }
            return errors;
        }

        protected override QuotationServiceResponse GetQuotationResponseObject(object response, QuotationServiceRequest request)
        {
            var quotationServiceResponse = new QuotationServiceResponse();
            string result = string.Empty;
            result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;

            if (request.ProductTypeCode == 1)
            {
                var desrialisedObj = JsonConvert.DeserializeObject<WataniyaTplQuotationResponseDto>(result);
                quotationServiceResponse = QuotationResponseObjMappingTpl(desrialisedObj, (request.DeductibleValue.HasValue ? request.DeductibleValue.Value : 2000));
            }
            else
            {
                var desrialisedObj = JsonConvert.DeserializeObject<WataniyaCompQuotationResponseDto>(result);
                quotationServiceResponse = QuotationResponseObjMappingComprehensive(desrialisedObj);
            }
            if (quotationServiceResponse != null && quotationServiceResponse.Products == null && quotationServiceResponse.Errors == null)
            {
                quotationServiceResponse.Errors = new List<Error>
                {
                    new Error { Message = result }
                };
            }

            return quotationServiceResponse;
        }

        #endregion

        #endregion

        #region Policy

        protected override object ExecutePolicyRequest(Dto.Providers.PolicyRequest policy, ServiceRequestLog predefinedLogInfo)
        {
            //in case test mode execute the code from the base.
            if (_tameenkConfig.Quotatoin.TestMode)
                return base.ExecutePolicyRequest(policy, predefinedLogInfo);

            //ServiceOutput output = SubmitPolicyRequest(policy, predefinedLogInfo);
            ServiceOutput output = SubmitIssuePolicyRequest(policy, predefinedLogInfo);
            if (output.ErrorCode != ServiceOutput.ErrorCodes.Success)
            {
                return null;
            }

            string exception = string.Empty;
            var wataniyaNajmQueueService = EngineContext.Current.Resolve<IWataniyaNajmQueueService>();
            bool value = wataniyaNajmQueueService.AddWataniyaNajmQueue(policy.ReferenceId, predefinedLogInfo.PolicyNo, out exception);
            if (!value || !string.IsNullOrEmpty(exception))
            {
                predefinedLogInfo.ErrorCode = (int)ServiceOutput.ErrorCodes.ServiceError;
                predefinedLogInfo.ErrorDescription = "Error happend when try to insert wataniya najm queue, and the error is: " + exception;
                predefinedLogInfo.ServiceErrorCode = predefinedLogInfo.ErrorDescription.ToString();
                predefinedLogInfo.ServiceErrorDescription = predefinedLogInfo.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(predefinedLogInfo);
            }
            return output.Output;
        }

        private ServiceOutput SubmitIssuePolicyRequest(Dto.Providers.PolicyRequest policy, ServiceRequestLog log)
        {
            ServiceOutput output = new ServiceOutput();
            log.ReferenceId = policy.ReferenceId;
            if (string.IsNullOrEmpty(log.Channel))
                log.Channel = "Portal";
            log.ServiceURL = log.InsuranceTypeCode == 1 ? GET_PURCHASE_NOTIFICATION_URL : GET_PURCHASE_NOTIFICATION_URL_COM;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Policy";
            log.CompanyName = _restfulConfiguration.ProviderName;
            var request = _policyProcessingQueueRepository.Table.Where(a => a.ReferenceId == policy.ReferenceId).FirstOrDefault();
            if (request == null)
            {
                output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
                output.ErrorDescription = "No Processing queue created for this reference " + policy.ReferenceId;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            try
            {
                request.RequestID = log.RequestId;
                request.CompanyName = log.CompanyName;
                request.CompanyID = log.CompanyID;
                request.InsuranceTypeCode = log.InsuranceTypeCode;
                request.DriverNin = log.DriverNin;
                request.VehicleId = log.VehicleId;

                var _quotationService = EngineContext.Current.Resolve<IQuotationService>();
                var initialDraftModel = _quotationService.GetWataniyaMotorPolicyInfoByReference(policy.ReferenceId);
                if (initialDraftModel == null)
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "No Wataniya Policy Info created for this reference " + policy.ReferenceId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                //if (!string.IsNullOrEmpty(initialDraftModel.PolicyNo))
                //{
                //    HandleFailedPoliciesAndSuccessResponse(initialDraftModel, output, log, request);
                //    return output;
                //}
                WataniyaIssuePolicyResponseDto serviceResponse = null;
                if (!string.IsNullOrEmpty(request.ServiceResponse) && !request.ServiceResponse.Contains("<html>"))
                {
                    serviceResponse = HandleWataniyaFirstResponse(request.ServiceResponse);
                }
                if (serviceResponse != null && serviceResponse.Status)
                {
                    if (!string.IsNullOrEmpty(serviceResponse.PolicyNO))
                    {
                        //output = HandleFailedPoliciesAndSuccessResponse(initialDraftModel);

                        PolicyResponse policyResponse = new PolicyResponse();
                        policyResponse.ReferenceId = initialDraftModel.ReferenceId.ToString();
                        policyResponse.StatusCode = 1;
                        policyResponse.PolicyNo = serviceResponse.PolicyNO;
                        policyResponse.PolicyIssuanceDate = DateTime.Now;
                        policyResponse.PolicyEffectiveDate = (initialDraftModel.PolicyEffectiveDate != null) ? initialDraftModel.PolicyEffectiveDate : DateTime.Now.AddDays(1);
                        policyResponse.PolicyExpiryDate = (initialDraftModel.PolicyExpiryDate != null)
                                                    ? initialDraftModel.PolicyExpiryDate
                                                    : policyResponse.PolicyEffectiveDate.Value.AddYears(1).AddDays(-1);

                        HttpResponseMessage response = new HttpResponseMessage()
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(policyResponse), System.Text.Encoding.UTF8, "application/json")
                        };

                        output.Output = response;
                        output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                        output.ErrorDescription = "Success";
                        if (request != null)
                        {
                            request.ErrorDescription = output.ErrorDescription;
                            _policyProcessingQueueRepository.Update(request);
                        }
                        return output;
                    }
                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "No PolicyNo returned from company, and waiting for Policy details to be sent from Watnia site";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceRequest = request.ServiceRequest;
                    log.ServiceResponse = request.ServiceResponse;
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    if (request != null)
                    {
                        request.ErrorDescription = output.ErrorDescription;
                        _policyProcessingQueueRepository.Update(request);
                    }
                    return output;
                }
                if (log.InsuranceTypeCode == 1)
                    return SubmitTplPolicyRequest(policy, request, initialDraftModel, log);

                else
                    return SendCompPolicyRequest(policy, request, initialDraftModel, log);
            }
            catch (Exception ex)
            {
                output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
                output.ErrorDescription = ex.ToString();
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
        }
        


        #region Tpl Policy

        private ServiceOutput SubmitTplPolicyRequest(Dto.Providers.PolicyRequest policy, PolicyProcessingQueue request, WataniyaMotorPolicyInfo initialDraftModel, ServiceRequestLog log)
        {
            ServiceOutput output = new ServiceOutput();

            var stringPayload = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                var testMode = _tameenkConfig.Policy.TestMode;
                if (testMode)
                {
                    const string nameOfFile = ".TestData.policyTestData.json";
                    string responseData = ReadResource(GetType().Namespace, nameOfFile);
                    HttpResponseMessage message = new HttpResponseMessage();
                    message.Content = new StringContent(responseData);
                    message.StatusCode = System.Net.HttpStatusCode.OK;
                    output.Output = message;
                    output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    return output;
                }

                var wataniyaPolicyRequest = new WataniyaTplissuePolicyRequestDto()
                {
                    Details = HandleWataniyaTplPolicyDetails(policy),
                    InsuranceCompanyCode = 14,
                    InsuranceTypeID = 1,
                    IsPurchased = true,
                    QuoteReferenceNo = long.Parse(policy.QuotationNo), // returned in quotation response
                    RequestReferenceNo = policy.ReferenceId
                };
                stringPayload = JsonConvert.SerializeObject(wataniyaPolicyRequest);
                log.ServiceRequest = stringPayload;
                request.ServiceRequest = stringPayload;

                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                //var client = new HttpClient();
                //client.Timeout = TimeSpan.FromMinutes(4);
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _accessTokenBase64);
                //var postTask = client.PostAsync(GET_PURCHASE_NOTIFICATION_URL, httpContent);
                var postTask = _httpClient.PostAsyncWithCertificate(GET_PURCHASE_NOTIFICATION_URL, httpContent, CERTIFCATE_BATH, CERTIFCATE_PASSWORD, _accessTokenBase64, authorizationMethod: "Basic");
                postTask.Wait();
                response = postTask.Result;

                DateTime dtBeforeCalling = DateTime.Now;
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null)
                {
                    if (request != null)
                    {
                        request.ErrorDescription = " service Return null";
                        _policyProcessingQueueRepository.Update(request);
                    }
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
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
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
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
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content result return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;
                request.ServiceResponse = log.ServiceResponse;

                var policyServiceResponse = WataniyaDeserializeIssuePolicyResponse(response.Content.ReadAsStringAsync().Result, initialDraftModel);
                if (policyServiceResponse != null && policyServiceResponse.Errors != null && policyServiceResponse.Errors.Count > 0)
                {
                    StringBuilder servcieErrors = new StringBuilder();
                    StringBuilder servcieErrorsCodes = new StringBuilder();

                    foreach (var error in policyServiceResponse.Errors)
                    {
                        servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);
                        servcieErrorsCodes.AppendLine(error.Code);
                    }

                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
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
                if (string.IsNullOrEmpty(policyServiceResponse.PolicyNo))
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
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

                string exception = string.Empty;
                initialDraftModel.PolicyNo = policyServiceResponse.PolicyNo;
                _quotationService.InsertOrupdateWataniyaMotorPolicyInfo(initialDraftModel, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Error happend wheb try to insert Initial policy info " + exception;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorDescription.ToString();
                    log.ServiceErrorDescription = log.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                HttpResponseMessage httpResponse = new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(policyServiceResponse), System.Text.Encoding.UTF8, "application/json"),
                };
                output.Output = httpResponse;
                output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.PolicyNo = policyServiceResponse.PolicyNo;
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
                // _logger.Log($"RestfulInsuranceProvider -> ExecuteQuotationRequest - (Provider name: {Configuration.ProviderName})", ex, LogLevel.Error);
                output.ErrorCode = ServiceOutput.ErrorCodes.ServiceException;
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

        private WataniyaTplissuePolicyRequestDeatilsDto HandleWataniyaTplPolicyDetails(Dto.Providers.PolicyRequest policy)
        {
            var wataniyaPolicyRequestDeatils = new WataniyaTplissuePolicyRequestDeatilsDto();
            wataniyaPolicyRequestDeatils.AdditionalNumber = policy.InsuredAdditionalNumber;
            wataniyaPolicyRequestDeatils.BankCode = policy.InsuredBankCode;
            wataniyaPolicyRequestDeatils.BuildingNumber = policy.InsuredBuildingNo;
            wataniyaPolicyRequestDeatils.City = policy.InsuredCity;
            wataniyaPolicyRequestDeatils.CreditAmount = policy.PaymentAmount; // will be (net value) for wataniya as per Mohamed Ghawati
            wataniyaPolicyRequestDeatils.CreditNoteNumber = policy.PaymentBillNumber; // as per Mohamed Ghawati
            wataniyaPolicyRequestDeatils.CustomizedParameter = HandleIssuePolicyCustomizedParameter(policy.PaymentMethodCode);
            wataniyaPolicyRequestDeatils.District = policy.InsuredDistrict;
            wataniyaPolicyRequestDeatils.Email = policy.InsuredEmail;
            wataniyaPolicyRequestDeatils.IBANNumber = policy.InsuredIBAN;
            wataniyaPolicyRequestDeatils.MobileNo = policy.InsuredMobileNumber;
            wataniyaPolicyRequestDeatils.Street = policy.InsuredStreet;
            wataniyaPolicyRequestDeatils.ZipCode = policy.InsuredZipCode;

            return wataniyaPolicyRequestDeatils;
        }

        #endregion

        #region Comp Policy

        public override ServiceOutput GetWataniyaMotorDraftpolicy(Dto.Providers.PolicyRequest policy, ServiceRequestLog log)
        {
            ServiceOutput serviceOutput = new ServiceOutput();
            var output = SubmitCompDraftPolicyRequest(policy, log);
            if (output.ErrorCode == ServiceOutput.ErrorCodes.Success)
            {
                serviceOutput.ErrorCode = ServiceOutput.ErrorCodes.Success;
                serviceOutput.Output = output.Output;
                return serviceOutput;
            }
            else
            {
                serviceOutput.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                serviceOutput.ErrorDescription = output.ErrorDescription;
                return serviceOutput;
            }
        }

        private ServiceOutput SubmitCompDraftPolicyRequest(Dto.Providers.PolicyRequest policy, ServiceRequestLog log)
        {
            ServiceOutput output = new ServiceOutput();
            log.ReferenceId = policy.ReferenceId;
            if (string.IsNullOrEmpty(log.Channel))
                log.Channel = "Portal";
            log.ServiceURL = GET_DRAFT_POLICY_URL;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Draft Policy";
            log.CompanyName = _restfulConfiguration.ProviderName;
            var stringPayload = string.Empty;

            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                var testMode = _tameenkConfig.Policy.TestMode;
                if (testMode)
                {
                    const string nameOfFile = ".TestData.policyTestData.json";
                    string responseData = ReadResource(GetType().Namespace, nameOfFile);
                    HttpResponseMessage message = new HttpResponseMessage();
                    message.Content = new StringContent(responseData);
                    message.StatusCode = System.Net.HttpStatusCode.OK;
                    output.Output = message;
                    output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    return output;
                }

                var wataniyaDraftPolicyRequest = new WataniyaCompDraftPolicyRequestDto();
                wataniyaDraftPolicyRequest.Details = HandleWataniyaCompDraftPolicyDetails(policy);
                wataniyaDraftPolicyRequest.InsuranceCompanyCode = 14;
                wataniyaDraftPolicyRequest.PolicyRequestReferenceNo = CreateWataniyaDraftPolicyRequestReferenceNo(); // must be saved in any place
                wataniyaDraftPolicyRequest.QuoteReferenceNo = long.Parse(policy.QuotationNo); // returned in quotation response
                wataniyaDraftPolicyRequest.RequestReferenceNo = policy.ReferenceId;

                stringPayload = JsonConvert.SerializeObject(wataniyaDraftPolicyRequest);
                log.ServiceRequest = stringPayload;

                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                //var client = new HttpClient();
                //client.Timeout = TimeSpan.FromMinutes(4);
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _accessTokenBase64);
                DateTime dtBeforeCalling = DateTime.Now;
                var postTask = _httpClient.PostAsyncWithCertificate(GET_DRAFT_POLICY_URL, httpContent, CERTIFCATE_BATH, CERTIFCATE_PASSWORD, _accessTokenBase64, authorizationMethod: "Basic");
                postTask.Wait();
                DateTime dtAfterCalling = DateTime.Now;
                response = postTask.Result;

                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null)
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = " Draft Policy Service return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (response.Content == null)
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = " Draft Policy Service response content return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = " Draft Policy Service response content result return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;

                var policyServiceResponse = JsonConvert.DeserializeObject<WataniyaCompDraftPolicyResponseDto>(response.Content.ReadAsStringAsync().Result);
                if (policyServiceResponse != null && policyServiceResponse.errors != null && policyServiceResponse.errors.Count > 0)
                {
                    StringBuilder servcieErrors = new StringBuilder();
                    StringBuilder servcieErrorsCodes = new StringBuilder();

                    foreach (var error in policyServiceResponse.errors)
                    {
                        servcieErrors.AppendLine("Error Code: " + error.code + " and the error message : " + error.message);
                        servcieErrorsCodes.AppendLine(error.code);
                    }

                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Draft Policy Service response error is : " + servcieErrors.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = servcieErrorsCodes.ToString();
                    log.ServiceErrorDescription = servcieErrors.ToString();
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                var _quotationService = EngineContext.Current.Resolve<IQuotationService>();
                var initialDraftModel = _quotationService.GetWataniyaMotorPolicyInfoByReference(policy.ReferenceId);
                if (initialDraftModel == null)
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "No Wataniya Policy Info created for this reference " + policy.ReferenceId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                string exception = string.Empty;
                initialDraftModel.PolicyRequestReferenceNo = wataniyaDraftPolicyRequest.PolicyRequestReferenceNo;
                initialDraftModel.PolicyReferenceNo = policyServiceResponse.PolicyReferenceNo.ToString();
                _quotationService.InsertOrupdateWataniyaMotorPolicyInfo(initialDraftModel, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Error happend wheb try to update Initial policy info " + exception;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorDescription.ToString();
                    log.ServiceErrorDescription = log.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                var serializedResponse = JsonConvert.SerializeObject(policyServiceResponse);
                output.Output = serializedResponse;
                output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = ServiceOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }

        private WataniyaCompDraftPolicyRequestDeatilsDto HandleWataniyaCompDraftPolicyDetails(Dto.Providers.PolicyRequest policy)
        {
            var wataniyaPolicyRequestDeatils = new WataniyaCompDraftPolicyRequestDeatilsDto();
            wataniyaPolicyRequestDeatils.DeductibleAmount = int.Parse(policy.DeductibleAmount);
            wataniyaPolicyRequestDeatils.DeductibleReferenceNo = policy.ReferenceId;
            if (policy.Benefits != null && policy.Benefits.Count > 0)
                wataniyaPolicyRequestDeatils.PolicyPremiumFeatures = HandleWataniyaCompDraftPolicyPolicyPremiumFeatures(policy.Benefits);

            //var totalPremium = Convert.ToDecimal(policy.PolicyPremium) + policy.Benefits.Sum(a => a.BenefitPrice);
            //wataniyaPolicyRequestDeatils.PolicyPremium = Math.Round(totalPremium.Value, 2);
            wataniyaPolicyRequestDeatils.PolicyPremium = Math.Round(Convert.ToDecimal(policy.PolicyPremium), 2);

            return wataniyaPolicyRequestDeatils;
        }

        private List<PolicyPremiunFeatures> HandleWataniyaCompDraftPolicyPolicyPremiumFeatures(List<BenefitRequest> benefits)
        {
            List<PolicyPremiunFeatures> policyPremiumFeatures = new List<PolicyPremiunFeatures>();
            policyPremiumFeatures.Add(new PolicyPremiunFeatures()
            {
                FeatureID = 116,
                FeatureAmount = 0,
                FeatureTaxableAmount = 0,
                FeatureTypeID = 2
            });

            foreach (var benefit in benefits)
            {
                long featureAmount = 0;
                if (benefit.BenefitPrice.HasValue)
                    featureAmount = Decimal.ToInt64(benefit.BenefitPrice.Value);

                policyPremiumFeatures.Add(new PolicyPremiunFeatures()
                {
                    FeatureID = short.Parse(benefit.BenefitId),
                    FeatureAmount = featureAmount,
                    FeatureTaxableAmount = featureAmount,
                    FeatureTypeID = (benefit.BenefitPrice.HasValue && benefit.BenefitPrice.Value > 0) ? 1 : 2
                });
            }

            return policyPremiumFeatures;
        }

        private ServiceOutput SendCompPolicyRequest(Dto.Providers.PolicyRequest policy, PolicyProcessingQueue request, WataniyaMotorPolicyInfo initialDraftModel, ServiceRequestLog log)
        {
            ServiceOutput output = new ServiceOutput();

            var stringPayload = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                var testMode = _tameenkConfig.Policy.TestMode;
                if (testMode)
                {
                    const string nameOfFile = ".TestData.policyTestData.json";
                    string responseData = ReadResource(GetType().Namespace, nameOfFile);
                    HttpResponseMessage message = new HttpResponseMessage();
                    message.Content = new StringContent(responseData);
                    message.StatusCode = System.Net.HttpStatusCode.OK;
                    output.Output = message;
                    output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    return output;
                }

                var wataniyaIssuePolicyRequest = new WataniyaCompIssuePolicyRequestDto()
                {
                    Details = HandleWataniyaCompIssuePolicyDetails(policy),
                    InsuranceCompanyCode = 14,
                    InsuranceTypeID = 2,
                    IsPurchased = true,
                    PolicyReferenceNo = long.Parse(initialDraftModel.PolicyReferenceNo), // returned from draft policy response
                    PolicyRequestReferenceNo = initialDraftModel.PolicyRequestReferenceNo, // created by BCare and send it draft policy request
                    QuoteReferenceNo = long.Parse(policy.QuotationNo), // returned in quotation response
                    RequestReferenceNo = policy.ReferenceId,
                    RequestType = 1
                };

                stringPayload = JsonConvert.SerializeObject(wataniyaIssuePolicyRequest);
                log.ServiceRequest = stringPayload;
                if (request != null)
                    request.ServiceRequest = log.ServiceRequest;
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                //var client = new HttpClient();
                //client.Timeout = TimeSpan.FromMinutes(4);
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _accessTokenBase64);
                //var postTask = client.PostAsync(GET_PURCHASE_NOTIFICATION_URL_COM, httpContent);
                var postTask = _httpClient.PostAsyncWithCertificate(GET_PURCHASE_NOTIFICATION_URL_COM, httpContent, CERTIFCATE_BATH, CERTIFCATE_PASSWORD, _accessTokenBase64, authorizationMethod: "Basic");
                postTask.Wait();
                response = postTask.Result;

                DateTime dtBeforeCalling = DateTime.Now;
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null)
                {
                    if (request != null)
                    {
                        request.ErrorDescription = " Issue Policy service Return null";
                        _policyProcessingQueueRepository.Update(request);
                    }
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = " Issue Policy Service return null";
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
                        request.ErrorDescription = "  Issue Policy service response content return null";
                        _policyProcessingQueueRepository.Update(request);
                    }
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = " Issue Policy Service response content return null";
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
                        request.ErrorDescription = "  Issue Policy Service response content result return null";
                        _policyProcessingQueueRepository.Update(request);
                    }
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = " Issue Policy Service response content result return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;
                request.ServiceResponse = log.ServiceResponse;

                var policyServiceResponse = WataniyaDeserializeIssuePolicyResponse(response.Content.ReadAsStringAsync().Result, initialDraftModel);
                if (policyServiceResponse != null && policyServiceResponse.Errors != null && policyServiceResponse.Errors.Count > 0)
                {
                    StringBuilder servcieErrors = new StringBuilder();
                    StringBuilder servcieErrorsCodes = new StringBuilder();

                    foreach (var error in policyServiceResponse.Errors)
                    {
                        servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);
                        servcieErrorsCodes.AppendLine(error.Code);
                    }

                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Issue Policy Service response error is : " + servcieErrors.ToString();
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
                if (string.IsNullOrEmpty(policyServiceResponse.PolicyNo))
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
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

                string exception = string.Empty;
                initialDraftModel.PolicyNo = policyServiceResponse.PolicyNo;
                _quotationService.InsertOrupdateWataniyaMotorPolicyInfo(initialDraftModel, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Error happend wheb try to insert Initial policy info " + exception;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorDescription.ToString();
                    log.ServiceErrorDescription = log.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                HttpResponseMessage httpResponse = new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(policyServiceResponse), System.Text.Encoding.UTF8, "application/json"),
                };
                output.Output = httpResponse;
                output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.PolicyNo = policyServiceResponse.PolicyNo;
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
                // _logger.Log($"RestfulInsuranceProvider -> ExecuteQuotationRequest - (Provider name: {Configuration.ProviderName})", ex, LogLevel.Error);
                output.ErrorCode = ServiceOutput.ErrorCodes.ServiceException;
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


        private WataniyaCompIssuePolicyRequestDeatilsDto HandleWataniyaCompIssuePolicyDetails(Dto.Providers.PolicyRequest policy)
        {
            var wataniyaPolicyRequestDeatils = new WataniyaCompIssuePolicyRequestDeatilsDto();
            wataniyaPolicyRequestDeatils.AdditionalNumber = policy.InsuredAdditionalNumber;
            wataniyaPolicyRequestDeatils.BankCode = policy.InsuredBankCode;
            wataniyaPolicyRequestDeatils.BuildingNumber = policy.InsuredBuildingNo;
            wataniyaPolicyRequestDeatils.City = policy.InsuredCity;
            wataniyaPolicyRequestDeatils.CreditAmount = Convert.ToDecimal(policy.PolicyPremium);
            wataniyaPolicyRequestDeatils.CreditNoteNumber = policy.PaymentBillNumber; // ask fayssal for confirmation
            wataniyaPolicyRequestDeatils.CustomizedParameter = HandleIssuePolicyCustomizedParameter(policy.PaymentMethodCode);
            wataniyaPolicyRequestDeatils.District = policy.InsuredDistrict;
            wataniyaPolicyRequestDeatils.Email = policy.InsuredEmail;
            wataniyaPolicyRequestDeatils.IBANNumber = policy.InsuredIBAN;
            wataniyaPolicyRequestDeatils.MobileNo = policy.InsuredMobileNumber;
            wataniyaPolicyRequestDeatils.Street = policy.InsuredStreet;
            wataniyaPolicyRequestDeatils.ZipCode = policy.InsuredZipCode;
            wataniyaPolicyRequestDeatils.PurchaseStatus = 1;

            return wataniyaPolicyRequestDeatils;
        }

        #endregion

        private string CreateWataniyaDraftPolicyRequestReferenceNo()
        {
            Random random = new Random();
            string stringReference = string.Empty;
            for (int i = 1; i <= 10; i++)
                stringReference += random.Next(0, 9).ToString();

            return stringReference;
        }

        private List<CustomizedParameter> HandleIssuePolicyCustomizedParameter(int paymentMethodCode)
        {
            var customizeParams = new List<CustomizedParameter>();
            customizeParams.Add(new CustomizedParameter() { Key = "PaymentMethod", Value1 = "17", Value2 = "", Value3 = "", Value4 = "" });
            //customizeParams.Add(new CustomizedParameter() { Key = "PaymentMethod", Value1 = paymentMethodCode.ToString(), Value2 = "", Value3 = "", Value4 = "" });

            var time = DateTime.Now;
            var _purchaseDateTime = $"{ time.Year }-{ time.Month }-{ time.Day } { time.Hour }:{ time.Minute }:{ time.Second }";
            customizeParams.Add(new CustomizedParameter() { Key = "PurchaseDateTime", Value1 = _purchaseDateTime, Value2 = "", Value3 = "", Value4 = "" });

            return customizeParams;
        }

        private PolicyResponse WataniyaDeserializeIssuePolicyResponse(string result, WataniyaMotorPolicyInfo initialDraftModel)
        {
            try
            {
                PolicyResponse policy = null;
                var deserializeResult = JsonConvert.DeserializeObject<WataniyaIssuePolicyResponseDto>(result);
                if (deserializeResult == null)
                    return policy;

                if (deserializeResult.Errors != null && deserializeResult.Errors.Count > 0)
                {
                    policy = new PolicyResponse();
                    policy.Errors = HandleWataniyaQuotationResponseErrors(deserializeResult.Errors);
                    return policy;
                }

                policy = new PolicyResponse();
                policy.ReferenceId = initialDraftModel.ReferenceId;
                policy.StatusCode = (deserializeResult.Status == true) ? 1 : 2;
                policy.PolicyNo = deserializeResult.PolicyNO;
                policy.PolicyIssuanceDate = DateTime.Now;
                policy.PolicyEffectiveDate = (initialDraftModel.PolicyEffectiveDate.HasValue) ? initialDraftModel.PolicyEffectiveDate.Value : DateTime.Now.AddDays(1);
                policy.PolicyExpiryDate = (initialDraftModel.PolicyExpiryDate != null)
                                            ? initialDraftModel.PolicyExpiryDate
                                            : policy.PolicyEffectiveDate.Value.AddYears(1).AddDays(-1);

                return policy;
            }
            catch (Exception ex)
            {
                return new PolicyResponse();
            }
        }

        //protected override PolicyResponse GetPolicyResponseObject(object response, Dto.Providers.PolicyRequest request = null)
        //{
        //    PolicyResponse policy = null;
        //    string result = string.Empty;

        //    try
        //    {
        //        var httpResponse = (response as HttpResponseMessage);
        //        if (httpResponse != null && httpResponse.IsSuccessStatusCode)
        //        {
        //            result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
        //            var deserializeResult = JsonConvert.DeserializeObject<PolicyResponse>(result);
        //            if (deserializeResult != null)
        //                policy = deserializeResult;
        //        }

        //        return policy;
        //    }
        //    catch (Exception ex)
        //    {
        //        //_logger.Log($"WataniyaInsuranceProvider -> GetPolicyResponseObject", ex, LogLevel.Error);
        //        return policy;
        //    }
        //}


        private ServiceOutput HandleFailedPoliciesAndSuccessResponse(WataniyaMotorPolicyInfo initialDraftModel)
        {
            ServiceOutput output = new ServiceOutput();
            try
            {
                PolicyResponse policyResponse = new PolicyResponse();
                policyResponse.ReferenceId = initialDraftModel.ReferenceId.ToString();
                policyResponse.StatusCode = 1;
                policyResponse.PolicyNo = initialDraftModel.PolicyNo;
                policyResponse.PolicyIssuanceDate = DateTime.Now;
                policyResponse.PolicyEffectiveDate = (initialDraftModel.PolicyEffectiveDate != null) ? initialDraftModel.PolicyEffectiveDate : DateTime.Now.AddDays(1);
                policyResponse.PolicyExpiryDate = (initialDraftModel.PolicyExpiryDate != null)
                                            ? initialDraftModel.PolicyExpiryDate
                                            : policyResponse.PolicyEffectiveDate.Value.AddYears(1).AddDays(-1);

                HttpResponseMessage response = new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(policyResponse), System.Text.Encoding.UTF8, "application/json")
                };

                output.Output = response;
                output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return output;
            }
            catch(Exception exp)
            {
                output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                output.ErrorDescription = exp.ToString();
                return output;
            }
        }

        private WataniyaIssuePolicyResponseDto HandleWataniyaFirstResponse(string response)
        {
            try
            {
                return JsonConvert.DeserializeObject<WataniyaIssuePolicyResponseDto>(response);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion


        #region AutoLeasing

        private WatnyiaAutoLeaseQuotationRequest MappingWataniyaAutoLeasingQuotationRequest(QuotationServiceRequest quoteModel)
        {
            var wataniyaAutoLeasingQuotation = new WatnyiaAutoLeaseQuotationRequest();
            wataniyaAutoLeasingQuotation.Policy = HandleAutoLeasingQuotationPolicyDetails(quoteModel);
            wataniyaAutoLeasingQuotation.PolicyRiskList = HandleAutoLeasingQuotationPolicyRiskListDetails(quoteModel);

            return wataniyaAutoLeasingQuotation;
        }

        private Dtos.Autolease.PolicyRequest HandleAutoLeasingQuotationPolicyDetails(QuotationServiceRequest quoteModel)
        {
            var wataniyaAutoLeasingQuotationPolicyDetails = new Dtos.Autolease.PolicyRequest();
            wataniyaAutoLeasingQuotationPolicyDetails.PolicyEffectiveDate = $"{ ConvertDatetimeToStringDate(quoteModel.PolicyEffectiveDate) } 00:00:00";
            wataniyaAutoLeasingQuotationPolicyDetails.PolicyExpiryDate = $"{ ConvertDatetimeToStringDate(quoteModel.PolicyEffectiveDate.AddYears(1).AddDays(-1)) } 00:00:00";
            if (!string.IsNullOrEmpty(quoteModel.QuotationNo))
            {
                wataniyaAutoLeasingQuotationPolicyDetails.RequestReferenceNumber = quoteModel.QuotationNo;
            }
            else
            {
                wataniyaAutoLeasingQuotationPolicyDetails.RequestReferenceNumber = quoteModel.ReferenceId;
            }
            return wataniyaAutoLeasingQuotationPolicyDetails;
        }

        private List<PolicyRiskListRequest> HandleAutoLeasingQuotationPolicyRiskListDetails(QuotationServiceRequest quoteModel)
        {
            var wataniyaAutoLeasingQuotationPolicyRiskList = new List<PolicyRiskListRequest>();
            var wataniyaAutoLeasingQuotationPolicyRiskObjectDetails = new PolicyRiskListRequest();
            wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.RiskID = "R00001";
            wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.PolicyDriverList = HandleWataniyaAutoLeasingPolicyRiskDriversList(quoteModel);
            wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.VehicleDefinitionType = quoteModel.VehicleIdTypeCode;
            if (quoteModel.VehicleIdTypeCode == 1)
                wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.SequenceNo = quoteModel.VehicleId.ToString();
            else if (quoteModel.VehicleIdTypeCode == 2)
                wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.CustomID = quoteModel.VehicleId.ToString();

            wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.SumInsured = quoteModel.VehicleValue.Value;
            wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.VehicleMake = quoteModel.VehicleMaker;

            if (!string.IsNullOrEmpty(quoteModel.VehicleMakerCode) && !string.IsNullOrEmpty(quoteModel.VehicleModelCode))
                HandleAutoleaseMakerAndModel(wataniyaAutoLeasingQuotationPolicyRiskObjectDetails, quoteModel);

            wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.VehicleType = quoteModel.VehicleIdTypeCode;
            wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.VehicleRegion = (quoteModel.InsuredAddressRegionID.HasValue)
                ? ((CentralRegions.Contains(quoteModel.InsuredAddressRegionID.Value))
                    ? "Central"
                    : ((WesternRegions.Contains(quoteModel.InsuredAddressRegionID.Value))
                        ? "Western"
                        : ((EasternRegions.Contains(quoteModel.InsuredAddressRegionID.Value))
                            ? "Eastern"
                            : ((SouthernRegions.Contains(quoteModel.InsuredAddressRegionID.Value))
                                ? "Southern"
                                : ((NorthernRegions.Contains(quoteModel.InsuredAddressRegionID.Value)) ? "Northern" : "")))))
                : "";
            wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.RepairCondition = (quoteModel.VehicleAgencyRepair.HasValue && quoteModel.VehicleAgencyRepair.Value) ? 1 : 2;
            wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.ProductionYear = quoteModel.VehicleModelYear;
            wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.ChassisNo = quoteModel.VehicleChassisNumber;
            if (!string.IsNullOrEmpty(quoteModel.VehicleMajorColorCode))
                wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.VehicleColor = int.Parse(quoteModel.VehicleMajorColorCode); // HandleVehicleColorId(quoteModel.VehicleMajorColor);
            else
                wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.VehicleColor = 1;

            wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.VehicleUsage = 3;

            if (!string.IsNullOrEmpty(quoteModel.VehiclePlateTypeCode))
                wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.PlateType = int.Parse(quoteModel.VehiclePlateTypeCode);

            if (quoteModel.VehiclePlateNumber.HasValue)
                wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.PlateNo = quoteModel.VehiclePlateNumber.Value;

            wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.PlateNoA = quoteModel.VehiclePlateText1;
            wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.PlateNoB = quoteModel.VehiclePlateText2;
            wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.PlateNoC = quoteModel.VehiclePlateText3;
            wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.IstmaraExpiryDate = "";
            if (quoteModel.VehicleOvernightParkingLocationCode.HasValue)
                wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.VehicleNightParking = quoteModel.VehicleOvernightParkingLocationCode.Value;
            if (quoteModel.VehicleTransmissionTypeCode.HasValue)
                wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.TransmissionType = quoteModel.VehicleTransmissionTypeCode.Value;
            wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.IsThereAdditionalModification = (quoteModel.VehicleModification) ? 1 : 0;
            wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.InterestDescription = (!string.IsNullOrEmpty(quoteModel.VehicleModificationDetails)) ? quoteModel.VehicleModificationDetails : "";
            wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.AntiLockBrakingSystem = (quoteModel.HasAntiTheftAlarm.HasValue && quoteModel.HasAntiTheftAlarm.Value) ? 1 : 2;
            wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.FireExtinguisher = (quoteModel.HasFireExtinguisher.HasValue && quoteModel.HasFireExtinguisher.Value) ? 1 : 2;
            wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.Weight = quoteModel.VehicleWeight;
            wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.EngineNo = "";
            wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.EngineCapacity = quoteModel.VehicleCapacity.ToString();
            wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.VehicleCylinder = quoteModel.VehicleCylinders;
            wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.TypeOfChassis = "";
            wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.UseOfVehicle = "Private / Commercial but not for Domestic Drivers or Transportation of Public & Goods"; // as per felwa and wataniya said

            wataniyaAutoLeasingQuotationPolicyRiskList.Add(wataniyaAutoLeasingQuotationPolicyRiskObjectDetails);
            return wataniyaAutoLeasingQuotationPolicyRiskList;
        }

        private void HandleAutoleaseMakerAndModel(PolicyRiskListRequest wataniyaAutoLeasingQuotationPolicyRiskObjectDetails, QuotationServiceRequest quoteModel)
        {
            var _vehicleService = EngineContext.Current.Resolve<IVehicleService>();

            var makerId = short.Parse(quoteModel.VehicleMakerCode);
            var modelId = long.Parse(quoteModel.VehicleModelCode);
            var vehicleModel = _vehicleService.GetVehicleModelByMakerCodeAndModelCode(makerId, modelId);
            if (vehicleModel != null)
            {
                if (vehicleModel.WataniyaMakerCode.HasValue)
                    wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.VehicleMakeID = int.Parse(vehicleModel.WataniyaMakerCode.Value.ToString());
                if (vehicleModel.WataniyaModelCode.HasValue)
                    wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.VehicleTypeID = int.Parse(vehicleModel.WataniyaModelCode.Value.ToString());
            }
        }

        private List<PolicyDriverListRequest> HandleWataniyaAutoLeasingPolicyRiskDriversList(QuotationServiceRequest quoteModel)
        {
            var drivers = new List<PolicyDriverListRequest>();

            int i = 0;
            foreach (var driver in quoteModel.Drivers)
            {
                i++;
                if (driver.DriverDrivingPercentage == null || !driver.DriverDrivingPercentage.HasValue || driver.DriverDrivingPercentage.Value == 0)
                    continue;

                var driverModel = new PolicyDriverListRequest();
                driverModel.Lessee = (i == 1) ? "Y" : "N";
                driverModel.Usage = (float)driver.DriverDrivingPercentage.Value / 100;
                driverModel.DriverName = $"{ driver.DriverFirstNameEn } { driver.DriverMiddleNameEn } { driver.DriverLastNameEn }";
                driverModel.ArabicName = $"{ driver.DriverFirstNameAr } { driver.DriverMiddleNameAr } { driver.DriverLastNameAr }";
                var year = driver.DriverBirthDateG.Year;
                var month = (driver.DriverBirthDateG.Month.ToString().Length == 1) ? "0" + driver.DriverBirthDateG.Month : driver.DriverBirthDateG.Month.ToString();
                var day = (driver.DriverBirthDateG.Day.ToString().Length == 1) ? "0" + driver.DriverBirthDateG.Day : driver.DriverBirthDateG.Day.ToString();
                driverModel.BirthDate = $"{ year }-{ month }-{ day }";

                if (driver.DriverIdTypeCode.HasValue)
                    driverModel.DriverIdType = driver.DriverIdTypeCode.Value;

                if (driver.DriverLicenses != null && driver.DriverLicenses.Count > 0)
                {
                    if (driver.DriverLicenses.Any(a => a.DriverLicenseTypeCode == "3"))
                    {
                        var licenseTypeCode3 = driver.DriverLicenses.Where(a => a.DriverLicenseTypeCode == "3").FirstOrDefault();
                        driverModel.LicenseNo = driver.DriverId.ToString();
                        driverModel.LicenseType = int.Parse(licenseTypeCode3.DriverLicenseTypeCode);
                        driverModel.LicenseYear = licenseTypeCode3.LicenseNumberYears;
                    }

                    else
                    {
                        driverModel.LicenseNo = driver.DriverId.ToString();
                        driverModel.LicenseType = int.Parse(driver.DriverLicenses.FirstOrDefault().DriverLicenseTypeCode);
                        driverModel.LicenseYear = driver.DriverLicenses.FirstOrDefault().LicenseNumberYears;
                    }
                }

                driverModel.Gender = driver.DriverGenderCode;
                if (!string.IsNullOrEmpty(driver.DriverSocialStatusCode))
                    driverModel.MaritalStatus = int.Parse(driver.DriverSocialStatusCode);
                var educationEnumString = Enum.GetName(typeof(EducationAr), driver.DriverEducationCode);
                driverModel.DriverEducation = WataniyaEducation.ResourceManager.GetString(educationEnumString);
                driverModel.DriverOccupation = driver.DriverOccupation;
                driverModel.RelationWithPolicyHolder = "";

                var address = GetDriverAddressByDriverNin(driver.DriverId.ToString());
                if (address != null)
                {
                    driverModel.UnitNumber = (!string.IsNullOrEmpty(address.UnitNumber)) ? address.UnitNumber : "";
                    driverModel.BuildingNumber = (!string.IsNullOrEmpty(address.BuildingNumber)) ? address.BuildingNumber : "";
                    driverModel.StreetName = (!string.IsNullOrEmpty(address.Street)) ? address.Street : "";
                    driverModel.DistrictName = (!string.IsNullOrEmpty(address.District)) ? address.District : "";
                    driverModel.AdditionalNumber = (!string.IsNullOrEmpty(address.AdditionalNumber)) ? address.AdditionalNumber : "";
                    driverModel.City = (!string.IsNullOrEmpty(address.City)) ? address.City : "";
                    driverModel.Zone = "";
                    driverModel.Country = "";
                    driverModel.Latitude = (!string.IsNullOrEmpty(address.Latitude)) ? address.Latitude : "";
                    driverModel.Longitude = (!string.IsNullOrEmpty(address.Longitude)) ? address.Longitude : "";
                    driverModel.ZipCode = (!string.IsNullOrEmpty(address.PostCode)) ? address.PostCode : "";
                }

                drivers.Add(driverModel);
            }

            return drivers;
        }

        protected override QuotationServiceResponse GetAutoleasingQuotationResponseObject(object response, QuotationServiceRequest request)
        {
            QuotationServiceResponse quotationServiceResponse = null;

            try
            {
                var result = string.Empty;
                if (response is HttpResponseMessage)
                    result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
                else
                    result = JsonConvert.SerializeObject(response);

                WatnyiaAutoLeaseQuotationResponse wataniyaQuotationReposne = JsonConvert.DeserializeObject<WatnyiaAutoLeaseQuotationResponse>(result);
                if (wataniyaQuotationReposne == null)
                    return quotationServiceResponse;

                else if (wataniyaQuotationReposne.ErrorList != null && wataniyaQuotationReposne.ErrorList.Count > 0)
                {
                    quotationServiceResponse = new QuotationServiceResponse();
                    quotationServiceResponse.Errors = HandleWataniyaAutoLeasingQuotationResponseErrors(wataniyaQuotationReposne.ErrorList);
                    return quotationServiceResponse;
                }

                else
                    quotationServiceResponse = AUtoLeasingQuotationResponseObjMapping(wataniyaQuotationReposne, request.ReferenceId);

                return quotationServiceResponse;
            }
            catch (Exception ex)
            {
                return quotationServiceResponse;
            }
        }

        private string ConvertDatetimeToStringDate(DateTime date)
        {
            // handle date day to start with "0" if it <= 9
            var dayLength = date.Day.ToString().Length;
            var _day = (dayLength > 1) ? date.Day.ToString() : "0" + date.Day;
            // handle date month to start with "0" if it <= 9
            var monthLength = date.Month.ToString().Length;
            var _month = (monthLength > 1) ? date.Month.ToString() : "0" + date.Month;

            var stringDate = $"{ date.Year }-{ _month }-{ _day }";
            return stringDate;
        }

        private QuotationServiceResponse AUtoLeasingQuotationResponseObjMapping(WatnyiaAutoLeaseQuotationResponse quotationServiceResponse, string referenceId)
        {
            var response = new QuotationServiceResponse();

            // as per fayssal
            response.ReferenceId = referenceId;
            response.StatusCode = quotationServiceResponse.Status;
            response.QuotationNo = quotationServiceResponse.Policy.ReferenceNumber.ToString();
            response.QuotationDate = DateTime.Now.ToString();
            response.QuotationExpiryDate = quotationServiceResponse.Policy.QuoteExpiryDate; ; // DateTime.Now.AddDays(1).ToString();
            response.Products = HandleWataniyaAutoleaseProducts(quotationServiceResponse);

            return response;
        }

        private List<ProductDto> HandleWataniyaAutoleaseProducts(WatnyiaAutoLeaseQuotationResponse quotationServiceResponse)
        {
            List<ProductDto> productDtos = new List<ProductDto>();

            foreach (var policyRiskList in quotationServiceResponse.PolicyRiskList)
            {
                foreach (var plan in policyRiskList.PolicyPlanList)
                {
                    var product = new ProductDto();
                    product.ProductId = plan.PlanCode;
                    product.DeductibleType = plan.DeductibleType;
                    product.InsuranceTypeCode = 2;
                    product.DeductibleValue = plan.MinDeductibleForPartialLoss;
                    product.PriceDetails = GetProductPriceDetails(plan.PolicyCoverageList, quotationServiceResponse.PolicyRiskList[0].NcdDiscount);
                    product.Benefits = GetAutoleaseProductBenfits(plan.PolicyCoverageList);
                    product.ProductPrice = HandleAutoleasingQuotationProductPrice(product.PriceDetails);

                    var MCOM = plan.PolicyCoverageList.Where(a => a.CoverageCode == "MCOM").FirstOrDefault();
                    if (MCOM != null)
                    {
                        product.PolicyPremium = MCOM.ActualPremium;
                        product.AnnualPremiumBFD = MCOM.AnnualPremiumBFD;
                        product.ODLimit = MCOM.ODLimit;
                        product.TPLLimit = MCOM.TPLLimit;
                    }

                    productDtos.Add(product);
                }
            }

            return productDtos;
        }

        private List<PriceDto> GetProductPriceDetails(List<PolicyCoverage> policyCoverageList, string ncdDiscount)
        {
            List<PriceDto> priceDetails = new List<PriceDto>();

            var ncd = (!string.IsNullOrEmpty(ncdDiscount)) ? decimal.Parse(ncdDiscount) : 0;
            var MCOM = policyCoverageList.Where(a => a.CoverageCode == "MCOM").FirstOrDefault();
            if (MCOM != null)
            {
                priceDetails.Add(new PriceDto { PriceValue = MCOM.AnnualPremiumBFD.Value, PriceTypeCode = 7 });
                priceDetails.Add(new PriceDto { PriceValue = Math.Round((MCOM.AnnualPremiumBFD.Value + 25 - ncd) * (decimal).15, 2), PriceTypeCode = 8 }); // here wataniya must return admin fee in quotation response
                priceDetails.Add(new PriceDto { PriceValue = ncd, PriceTypeCode = 2 });
            }

            else
            {
                priceDetails.Add(new PriceDto { PriceValue = 0, PriceTypeCode = 7 });
                priceDetails.Add(new PriceDto { PriceValue = 0, PriceTypeCode = 8 });
            }

            return priceDetails;
        }

        private List<BenefitDto> GetAutoleaseProductBenfits(List<PolicyCoverage> policyCoverageList)
        {
            List<BenefitDto> benfits = new List<BenefitDto>();
            foreach (var feature in policyCoverageList)
            {
                if (feature.CoverageCode == "MCOM")
                {
                    var MCOMbenefits = BenefitsLUT.Where(a => a.BenefitCode == "MCOM").ToList();
                    if (MCOMbenefits == null)
                        continue;

                    foreach (var item in MCOMbenefits)
                    {
                        benfits.Add(new BenefitDto
                        {
                            BenefitId = "MCOM-" + item.BenefitId,
                            BenefitCode = 0,
                            BenefitPrice = 0,
                            BenefitDescAr = item.BenefitDescAr,
                            BenefitNameAr = item.BenefitDescAr,
                            BenefitDescEn = item.BenefitDescEn,
                            BenefitNameEn = item.BenefitDescEn,
                            IsSelected = true,
                            IsReadOnly = true
                        });
                    }
                }

                else if (feature.CoverageCode == "MME")
                {
                    var MMEbenefits = BenefitsLUT.Where(a => a.BenefitCode == "MME").ToList();
                    if (MMEbenefits == null)
                        continue;

                    foreach (var item in MMEbenefits)
                    {
                        benfits.Add(new BenefitDto
                        {
                            BenefitId = "MME-" + item.BenefitId,
                            BenefitCode = 0,
                            BenefitPrice = 0,
                            BenefitDescAr = item.BenefitDescAr,
                            BenefitNameAr = item.BenefitDescAr,
                            BenefitDescEn = item.BenefitDescEn,
                            BenefitNameEn = item.BenefitDescEn,
                            IsSelected = true,
                            IsReadOnly = true,
                            Limit = (feature.LIMIT.HasValue) ? feature.LIMIT : null
                        });
                    }
                }

                else if (feature.CoverageCode == "MTB")
                {
                    var MTBbenefits = BenefitsLUT.Where(a => a.BenefitCode == "MTB").ToList();
                    if (MTBbenefits == null)
                        continue;

                    foreach (var item in MTBbenefits)
                    {
                        benfits.Add(new BenefitDto
                        {
                            BenefitId = "MTB-" + item.BenefitId,
                            BenefitCode = 0,
                            BenefitPrice = 0,
                            BenefitDescAr = item.BenefitDescAr,
                            BenefitNameAr = item.BenefitDescAr,
                            BenefitDescEn = item.BenefitDescEn,
                            BenefitNameEn = item.BenefitDescEn,
                            IsSelected = true,
                            IsReadOnly = true,
                            Limit = (feature.LIMIT.HasValue) ? feature.LIMIT : null
                        });
                    }
                }

                else
                {
                    var selectedBenfit = BenefitsLUT.Where(a => a.BenefitCode == feature.CoverageCode)?.FirstOrDefault();
                    if (selectedBenfit != null)
                    {
                        benfits.Add(new BenefitDto
                        {
                            BenefitId = feature.CoverageCode,
                            BenefitCode = 0,
                            BenefitPrice = (feature.ActualPremium != null) ? feature.ActualPremium.Value : 0,
                            BenefitDescAr = selectedBenfit.BenefitDescAr,
                            BenefitNameAr = selectedBenfit.BenefitDescAr,
                            BenefitDescEn = selectedBenfit.BenefitDescEn,
                            BenefitNameEn = selectedBenfit.BenefitDescEn,
                            IsSelected = (!feature.ActualPremium.Equals(null) && feature.ActualPremium > 0) ? false : true,
                            IsReadOnly = (!feature.ActualPremium.Equals(null) && feature.ActualPremium > 0) ? false : true,
                            CoveredCountry = feature.CoveredCountry,
                            AveragePremium = feature.AveragePremium,
                            Limit = (feature.LIMIT.HasValue) ? feature.LIMIT : null
                        });
                    }
                }
            }

            return benfits;
        }

        private decimal HandleAutoleasingQuotationProductPrice(List<PriceDto> priceDetails)
        {
            decimal totalPrice = priceDetails.Where(a => a.PriceTypeCode == 7).FirstOrDefault().PriceValue + 25;
            if (priceDetails.Any(a => a.PriceTypeCode == 2))
                totalPrice -= priceDetails.Where(a => a.PriceTypeCode == 2).FirstOrDefault().PriceValue;
            if (priceDetails.Any(a => a.PriceTypeCode == 8))
                totalPrice += priceDetails.Where(a => a.PriceTypeCode == 8).FirstOrDefault().PriceValue;

            return totalPrice;
        }

        private List<Error> HandleWataniyaAutoLeasingQuotationResponseErrors(List<AutoleaseError> Errors)
        {
            List<Error> errors = new List<Error>();
            foreach (var error in Errors)
            {
                var errorModel = new Error()
                {
                    Code = error.ErrorCode,
                    Message = error.ErrorMessage
                };
                errors.Add(errorModel);
            }
            return errors;
        }

        #region Autolease Draft Policy

        public override ServiceOutput GetWataniyaAutoleasingDraftpolicy(QuotationServiceRequest quotation, Product selectedProduct, ServiceRequestLog predefinedLogInfo)
        {
            ServiceOutput serviceOutput = new ServiceOutput();
            var output = SubmitAutLoeasingDraftPolicyRequest(quotation, selectedProduct, predefinedLogInfo);
            if (output.ErrorCode == ServiceOutput.ErrorCodes.Success)
            {
                serviceOutput.ErrorCode = ServiceOutput.ErrorCodes.Success;
                serviceOutput.Output = output.Output;
                return serviceOutput;
            }
            else
            {
                serviceOutput.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                serviceOutput.ErrorDescription = output.ErrorDescription;
                return serviceOutput;
            }
        }

        private ServiceOutput SubmitAutLoeasingDraftPolicyRequest(QuotationServiceRequest quoteModel, Product selectedProduct, ServiceRequestLog log)
        {
            ServiceOutput output = new ServiceOutput();
            if (string.IsNullOrEmpty(log.Channel))
                log.Channel = "AutoLease";
            log.ServiceURL = GET_AUTOLEASE_DRAFTPOLICY_URL;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "AutoleasingDraftPolicy";
            log.CompanyName = _restfulConfiguration.ProviderName;
            log.VehicleMaker = quoteModel?.VehicleMaker;
            log.VehicleMakerCode = quoteModel?.VehicleMakerCode.ToString();
            log.VehicleModel = quoteModel?.VehicleModel;
            log.VehicleModelCode = quoteModel?.VehicleModelCode.ToString();
            log.VehicleModelYear = quoteModel?.VehicleModelYear;
            log.DriverNin = quoteModel.Drivers.FirstOrDefault().DriverId.ToString();
            log.VehicleId = quoteModel.VehicleId.ToString();
            log.ReferenceId = quoteModel.ReferenceId;

            var stringPayload = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                WatnyiaAutoLeaseDraftPolicyRequest draftPolicyRequest = HandleAutoleasingDreaftPolicyRequest(quoteModel, selectedProduct);
                log.ServiceRequest = JsonConvert.SerializeObject(draftPolicyRequest);

                //if (quoteModel.BankId == 2)// Alyusr
                //    _restfulConfiguration.AutoleasingAccessToken = "Yusr.BacreAutolease:Yusr@123";
                 if (quoteModel.BankId == 3)//Aljaber
                    _restfulConfiguration.AutoleasingAccessToken = "Aljabr_Bcare:Aljabr@123";

                var _autoLeaseAccesToken = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(_restfulConfiguration.AutoleasingAccessToken));
                DateTime dtBeforeCalling = DateTime.Now;
                var postTask = _httpClient.PostAsyncWithCertificate(GET_AUTOLEASE_DRAFTPOLICY_URL, draftPolicyRequest, AUTOLEASE_CERTIFCATE_BATH, AUTOLEASE_CERTIFCATE_PASSWORD, _autoLeaseAccesToken, authorizationMethod: "Basic");
                postTask.Wait();
                response = postTask.Result;
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null)
                {
                    log.ErrorCode = (int)ServiceOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = "Draft Policy Service return null";
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (response.Content == null)
                {
                    log.ErrorCode = (int)ServiceOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = "Draft Policy Service response content return null";
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    log.ErrorCode = (int)ServiceOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = "Draft Policy Service response content result return null";
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;

                var desrialisedObj = JsonConvert.DeserializeObject<WatnyiaAutoLeaseDraftPolicyResponse>(response.Content.ReadAsStringAsync().Result);
                if (desrialisedObj != null && desrialisedObj.ErrorList != null)
                {
                    StringBuilder servcieErrors = new StringBuilder();
                    StringBuilder servcieErrorsCodes = new StringBuilder();

                    foreach (var error in desrialisedObj.ErrorList)
                    {
                        servcieErrors.AppendLine("Error Code: " + error.ErrorCode + " and the error message : " + error.ErrorMessage);
                        servcieErrorsCodes.AppendLine(error.ErrorCode);
                    }

                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Autolease Draft Policy Service response error is : " + servcieErrors.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = servcieErrorsCodes.ToString();
                    log.ServiceErrorDescription = servcieErrors.ToString();
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                var _quotationService = EngineContext.Current.Resolve<IQuotationService>();
                var initialDraftModel = _quotationService.GetWataniyaAutoleasePolicyInfoByReference(quoteModel.ReferenceId);
                if (initialDraftModel == null)
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "No Wataniya Draft Policy Info created for this reference " + quoteModel.ReferenceId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                string exception = string.Empty;
                initialDraftModel.QuotationNumber = desrialisedObj.Policy.QuotationNumber;
                initialDraftModel.ReferenceNumber = desrialisedObj.Policy.ReferenceNumber;
                _quotationService.InsertOrupdateWataniyaAutoleasePolicyInfo(initialDraftModel, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Error happend wheb try to insert Initial policy info " + exception;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorDescription.ToString();
                    log.ServiceErrorDescription = log.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                output.Output = response;
                output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = ServiceOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }

        private WatnyiaAutoLeaseDraftPolicyRequest HandleAutoleasingDreaftPolicyRequest(QuotationServiceRequest quoteModel, Product selectedProduct)
        {
            var wataniyaAutoLeasingQuotation = new WatnyiaAutoLeaseDraftPolicyRequest();
            wataniyaAutoLeasingQuotation.Policy = HandleAutoLeasingDraftPolicyDetails(quoteModel);
            wataniyaAutoLeasingQuotation.PolicyRiskList = HandleAutoLeasingDraftPolicyRiskListDetails(quoteModel, selectedProduct);

            return wataniyaAutoLeasingQuotation;
        }

        private DraftPolicyRequest HandleAutoLeasingDraftPolicyDetails(QuotationServiceRequest quoteModel)
        {
            var wataniyaAutoLeasingQuotationPolicyDetails = new DraftPolicyRequest();

            var draftPolicyInitialData = _quotationService.GetWataniyaDraftPolicyInitialData(quoteModel.ReferenceId);
            if (draftPolicyInitialData != null)
            {
                wataniyaAutoLeasingQuotationPolicyDetails.ReferenceNumber = draftPolicyInitialData.QuotationNumber; // will be from WataniyaDraftPolicy (QuoteReferenceNo)
                wataniyaAutoLeasingQuotationPolicyDetails.PolicyEffectiveDate = draftPolicyInitialData.PolicyEffectiveDate; // will be from WataniyaDraftPolicy (PolicyEffectiveDate)
                wataniyaAutoLeasingQuotationPolicyDetails.PolicyExpiryDate = draftPolicyInitialData.PolicyExpiryDate; // will be from WataniyaDraftPolicy (PolicyExpiryDate)
            }

            return wataniyaAutoLeasingQuotationPolicyDetails;
        }

        private List<DraftPolicyRiskListRequest> HandleAutoLeasingDraftPolicyRiskListDetails(QuotationServiceRequest quoteModel, Product selectedProduct)
        {
            var wataniyaAutoLeasingDraftPolicyRiskList = new List<DraftPolicyRiskListRequest>();
            var wataniyaAutoLeasingDraftPolicyRiskObjectDetails = new DraftPolicyRiskListRequest();
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.SequenceNo = quoteModel.VehicleId.ToString();
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.CustomID = quoteModel.VehicleId.ToString();
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.UseOfVehicle = "Private / Commercial but not for Domestic Drivers or Transportation of Public & Goods"; // as per felwa and wataniya said
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.RepairCondition = (quoteModel.VehicleAgencyRepair.HasValue && quoteModel.VehicleAgencyRepair.Value) ? 1 : 2;
            if (!string.IsNullOrEmpty(quoteModel.VehiclePlateTypeCode))
                wataniyaAutoLeasingDraftPolicyRiskObjectDetails.PlateType = int.Parse(quoteModel.VehiclePlateTypeCode);
            if (quoteModel.VehiclePlateNumber.HasValue)
                wataniyaAutoLeasingDraftPolicyRiskObjectDetails.PlateNo = quoteModel.VehiclePlateNumber.Value;
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.PlateNoA = quoteModel.VehiclePlateText1;
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.PlateNoB = quoteModel.VehiclePlateText2;
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.PlateNoC = quoteModel.VehiclePlateText3;
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.IstmaraExpiryDate = "";
            if (quoteModel.VehicleOvernightParkingLocationCode.HasValue)
                wataniyaAutoLeasingDraftPolicyRiskObjectDetails.VehicleNightParking = quoteModel.VehicleOvernightParkingLocationCode.Value;
            if (quoteModel.VehicleTransmissionTypeCode.HasValue)
                wataniyaAutoLeasingDraftPolicyRiskObjectDetails.TransmissionType = quoteModel.VehicleTransmissionTypeCode.Value;
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.IsThereAdditionalModification = (quoteModel.VehicleModification) ? 1 : 0;
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.InterestDescription = (!string.IsNullOrEmpty(quoteModel.VehicleModificationDetails)) ? quoteModel.VehicleModificationDetails : "";
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.AntiLockBrakingSystem = (quoteModel.HasAntiTheftAlarm.HasValue && quoteModel.HasAntiTheftAlarm.Value) ? 1 : 2;
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.FireExtinguisher = (quoteModel.HasFireExtinguisher.HasValue && quoteModel.HasFireExtinguisher.Value) ? 1 : 2;
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.Weight = quoteModel.VehicleWeight;
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.EngineNo = "";
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.EngineCapacity = quoteModel.VehicleCapacity.ToString();
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.VehicleCylinder = quoteModel.VehicleCylinders;
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.TypeOfChassis = "";
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.SumInsured = quoteModel.VehicleValue.Value;
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.VehicleMake = quoteModel.VehicleMaker;
            if (!string.IsNullOrEmpty(quoteModel.VehicleMakerCode) && !string.IsNullOrEmpty(quoteModel.VehicleModelCode))
                HandleAutoleaseDraftPolicyMakerAndModel(wataniyaAutoLeasingDraftPolicyRiskObjectDetails, quoteModel);
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.VehicleType = quoteModel.VehicleIdTypeCode;
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.ChassisNo = quoteModel.VehicleChassisNumber;
            if (!string.IsNullOrEmpty(quoteModel.VehicleMajorColor))
                wataniyaAutoLeasingDraftPolicyRiskObjectDetails.VehicleColor = int.Parse(quoteModel.VehicleMajorColorCode); //HandleVehicleColorId(quoteModel.VehicleMajorColor);
            else
                wataniyaAutoLeasingDraftPolicyRiskObjectDetails.VehicleColor = 1;
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.VehicleUsage = 3;
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.ProductionYear = quoteModel.VehicleModelYear;
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.VehicleDefinitionType = quoteModel.VehicleIdTypeCode;
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.VehicleRegion = (quoteModel.InsuredAddressRegionID.HasValue)
                ? ((CentralRegions.Contains(quoteModel.InsuredAddressRegionID.Value))
                    ? "Central"
                    : ((WesternRegions.Contains(quoteModel.InsuredAddressRegionID.Value))
                        ? "Western"
                        : ((EasternRegions.Contains(quoteModel.InsuredAddressRegionID.Value))
                            ? "Eastern"
                            : ((SouthernRegions.Contains(quoteModel.InsuredAddressRegionID.Value))
                                ? "Southern"
                                : ((NorthernRegions.Contains(quoteModel.InsuredAddressRegionID.Value)) ? "Northern" : "")))))
                : "";
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.AntiLockBrakingSystem = (quoteModel.HasAntiTheftAlarm.HasValue && quoteModel.HasAntiTheftAlarm.Value) ? 1 : 2;
            if (quoteModel.Drivers != null && quoteModel.Drivers.Count > 0)
                wataniyaAutoLeasingDraftPolicyRiskObjectDetails.PolicyDriverList = HandleWataniyaAutoLeasingDraftPolicyRiskDriversList(quoteModel.Drivers);
            wataniyaAutoLeasingDraftPolicyRiskObjectDetails.PolicyPlanList = HandleWataniyaAutoleasingDraftPolicyPolicyPlanList(quoteModel, selectedProduct);

            wataniyaAutoLeasingDraftPolicyRiskList.Add(wataniyaAutoLeasingDraftPolicyRiskObjectDetails);
            return wataniyaAutoLeasingDraftPolicyRiskList;
        }

        private void HandleAutoleaseDraftPolicyMakerAndModel(DraftPolicyRiskListRequest wataniyaAutoLeasingQuotationPolicyRiskObjectDetails, QuotationServiceRequest quoteModel)
        {
            var _vehicleService = EngineContext.Current.Resolve<IVehicleService>();

            var makerId = short.Parse(quoteModel.VehicleMakerCode);
            var modelId = long.Parse(quoteModel.VehicleModelCode);
            var vehicleModel = _vehicleService.GetVehicleModelByMakerCodeAndModelCode(makerId, modelId);
            if (vehicleModel != null)
            {
                if (vehicleModel.WataniyaMakerCode.HasValue)
                    wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.VehicleMakeID = int.Parse(vehicleModel.WataniyaMakerCode.Value.ToString());
                if (vehicleModel.WataniyaModelCode.HasValue)
                    wataniyaAutoLeasingQuotationPolicyRiskObjectDetails.VehicleTypeID = int.Parse(vehicleModel.WataniyaModelCode.Value.ToString());
            }
        }

        private List<PolicyPlanList> HandleWataniyaAutoleasingDraftPolicyPolicyPlanList(QuotationServiceRequest quoteModel, Product selectedProduct)
        {
            List<PolicyPlanList> policyPlanList = new List<PolicyPlanList>();
            var plan = new PolicyPlanList();
            plan.ActualPremium = HandlePlanActualPremium(selectedProduct);
            plan.DeductibleType = selectedProduct.DeductibleType;
            plan.MinDeductibleForPartialLoss = selectedProduct.DeductableValue.Value;
            plan.PlanCode = selectedProduct.ExternalProductId;
            plan.PolicyCoverageList = HandleAutoleasingDraftPolicyCoverageList(selectedProduct);

            policyPlanList.Add(plan);
            return policyPlanList;
        }

        private decimal HandlePlanActualPremium(Product selectedProduct)
        {
            foreach (var benefit in selectedProduct.Product_Benefits)
            {
                if (benefit.BenefitExternalId.Contains('-'))
                    benefit.BenefitExternalId = benefit.BenefitExternalId.Split('-')[0];
            }
            var planActualPremium = selectedProduct.Product_Benefits.Where(a => a.BenefitExternalId != "MCOM" && a.IsSelected == true).Sum(a => a.BenefitPrice.Value);
            return planActualPremium;
        }

        private List<PolicyCoverage> HandleAutoleasingDraftPolicyCoverageList(Product product)
        {
            List<PolicyCoverage> coverageList = new List<PolicyCoverage>();
            foreach (var benefit in product.Product_Benefits)
            {
                if (!benefit.IsSelected.Value)
                    continue;

                if (benefit.BenefitExternalId.Contains('-'))
                    benefit.BenefitExternalId = benefit.BenefitExternalId.Split('-')[0];

                if (coverageList.Any(a => a.CoverageCode == benefit.BenefitExternalId))
                    continue;

                if (benefit.BenefitExternalId == "MCOM")
                {
                    coverageList.Add(new PolicyCoverage
                    {
                        ActualPremium = product.PolicyPremium,
                        AnnualPremiumBFD = product.PriceDetails.Where(a => a.PriceTypeCode == 7).FirstOrDefault().PriceValue,
                        CoverageCode = "MCOM",
                        ODLimit = product.ODLimit,
                        TPLLimit = product.TPLLimit
                    });
                }

                else if (benefit.BenefitExternalId == "MME")
                {
                    coverageList.Add(new PolicyCoverage
                    {
                        CoverageCode = "MME",
                        LIMIT = (benefit.Limit.HasValue) ? benefit.Limit : null
                    });
                }

                else if (benefit.BenefitExternalId == "MNHS")
                {
                    coverageList.Add(new PolicyCoverage
                    {
                        CoverageCode = "MNHS",
                        LIMIT = (benefit.Limit.HasValue) ? benefit.Limit : null
                    });
                }

                else if (benefit.BenefitExternalId == "MTB")
                {
                    coverageList.Add(new PolicyCoverage
                    {
                        CoverageCode = "MTB",
                        LIMIT = (benefit.Limit.HasValue) ? benefit.Limit : null
                    });
                }

                else
                {
                    coverageList.Add(new PolicyCoverage
                    {
                        CoverageCode = (benefit.BenefitExternalId.Contains('-')) ? benefit.BenefitExternalId.Split('-')[0] : benefit.BenefitExternalId,
                        ActualPremium = benefit.BenefitPrice,
                        CoveredCountry = benefit.CoveredCountry,
                        AveragePremium = benefit.AveragePremium,
                        LIMIT = (benefit.Limit.HasValue) ? benefit.Limit : null
                    });
                }
            }

            return coverageList;
        }

        private List<DraftPolicyDriverListRequest> HandleWataniyaAutoLeasingDraftPolicyRiskDriversList(List<DriverDto> driversList)
        {
            var drivers = new List<DraftPolicyDriverListRequest>();

            int i = 0;
            foreach (var driver in driversList) // .Where(a => a.DriverId != request.InsuredId)
            {
                i++;
                if (!driver.DriverDrivingPercentage.HasValue || driver.DriverDrivingPercentage == null || driver.DriverDrivingPercentage.Value == 0)
                    continue;

                var driverModel = new DraftPolicyDriverListRequest();
                driverModel.Lessee = (i == 1) ? "Y" : "N";
                driverModel.Usage = (float)driver.DriverDrivingPercentage.Value / 100;
                driverModel.DriverName = $"{ driver.DriverFirstNameEn } { driver.DriverMiddleNameEn } { driver.DriverLastNameEn }";
                driverModel.ArabicName = $"{ driver.DriverFirstNameAr } { driver.DriverMiddleNameAr } { driver.DriverLastNameAr }";
                var year = driver.DriverBirthDateG.Year;
                var month = (driver.DriverBirthDateG.Month.ToString().Length == 1) ? "0" + driver.DriverBirthDateG.Month : driver.DriverBirthDateG.Month.ToString();
                var day = (driver.DriverBirthDateG.Day.ToString().Length == 1) ? "0" + driver.DriverBirthDateG.Day : driver.DriverBirthDateG.Day.ToString();
                driverModel.BirthDate = $"{ year }-{ month }-{ day }";

                if (driver.DriverIdTypeCode.HasValue)
                    driverModel.DriverIdType = driver.DriverIdTypeCode.Value;
                driverModel.LicenseNo = driver.DriverId.ToString();
                if (driver.DriverLicenses != null && driver.DriverLicenses.Count > 0)
                {
                    if (driver.DriverLicenses.Any(a => a.DriverLicenseTypeCode == "3"))
                    {
                        var licenseTypeCode3 = driver.DriverLicenses.Where(a => a.DriverLicenseTypeCode == "3").FirstOrDefault();
                        driverModel.LicenseType = "9";
                        driverModel.LicenseYear = licenseTypeCode3.LicenseNumberYears;
                    }
                    else
                    {
                        var license = driver.DriverLicenses.FirstOrDefault();
                        driverModel.LicenseType = HandleDriverLicenseType(license.DriverLicenseTypeCode);
                        driverModel.LicenseYear = license.LicenseNumberYears;
                    }
                }

                else
                    driverModel.LicenseType = "9";

                driverModel.Gender = driver.DriverGenderCode;
                driverModel.MaritalStatus = int.Parse(driver.DriverSocialStatusCode);
                driverModel.DriverEducation = Enum.GetName(typeof(EducationAr), driver.DriverEducationCode);
                driverModel.DriverOccupation = driver.DriverOccupation;
                driverModel.RelationWithPolicyHolder = "";
                driverModel.Mobile = driver.MobileNo;

                var address = GetDriverAddressByDriverNin(driver.DriverId.ToString());
                if (address != null)
                {
                    driverModel.UnitNumber = (!string.IsNullOrEmpty(address.UnitNumber)) ? address.UnitNumber : "";
                    driverModel.BuildingNumber = (!string.IsNullOrEmpty(address.BuildingNumber)) ? address.BuildingNumber : "";
                    driverModel.StreetName = (!string.IsNullOrEmpty(address.Street)) ? address.Street : "";
                    driverModel.DistrictName = (!string.IsNullOrEmpty(address.District)) ? address.District : "";
                    driverModel.AdditionalNumber = (!string.IsNullOrEmpty(address.AdditionalNumber)) ? address.AdditionalNumber : "";
                    driverModel.City = (!string.IsNullOrEmpty(address.City)) ? address.City : "";
                    driverModel.Zone = "";
                    driverModel.Country = "";
                    driverModel.Latitude = (!string.IsNullOrEmpty(address.Latitude)) ? address.Latitude : "";
                    driverModel.Longitude = (!string.IsNullOrEmpty(address.Longitude)) ? address.Longitude : "";
                    driverModel.ZipCode = (!string.IsNullOrEmpty(address.PostCode)) ? address.PostCode : "";
                }

                drivers.Add(driverModel);
            }

            return drivers;
        }

        private string HandleDriverLicenseType(string driverLicenseTypeCode)
        {
            string license = "";
            var licenseType = _quotationService.GetWataniyaDriverLicenseType(driverLicenseTypeCode);
            if (licenseType != null && licenseType.AutoleasingWataniyaCode != null)
                license = licenseType.AutoleasingWataniyaCode.ToString();

            return license;
        }

        #endregion

        #region Autolease Issue Policy

        protected override object ExecuteAutoleasingPolicyRequest(Dto.Providers.PolicyRequest policy, ServiceRequestLog predefinedLogInfo)
        {
            ServiceOutput output = SubmitWataniyaAutoleasingPolicyRequest(policy, predefinedLogInfo);
            if (output.ErrorCode != ServiceOutput.ErrorCodes.Success)
                return null;

            return output.Output;
        }

        protected ServiceOutput SubmitWataniyaAutoleasingPolicyRequest(Dto.Providers.PolicyRequest policy, ServiceRequestLog log)
        {
            ServiceOutput output = new ServiceOutput();
            log.ReferenceId = policy.ReferenceId;
            if (string.IsNullOrEmpty(log.Channel))
                log.Channel = "Portal";
            log.ServiceURL = GET_AUTOLEASE_POLICY_URL;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "AutoleasingPolicy";
            log.CompanyName = _restfulConfiguration.ProviderName;
            var stringPayload = string.Empty;

            var request = _policyProcessingQueueRepository.Table.Where(a => a.ReferenceId == policy.ReferenceId).FirstOrDefault();
            if (request != null)
            {
                request.RequestID = log.RequestId;
                request.CompanyName = log.CompanyName;
                request.CompanyID = log.CompanyID;
                request.InsuranceTypeCode = log.InsuranceTypeCode;
                request.DriverNin = log.DriverNin;
                request.VehicleId = log.VehicleId;
            }

            var _quotationService = EngineContext.Current.Resolve<IQuotationService>();
            var initialDraftModel = _quotationService.GetWataniyaAutoleasePolicyInfoByReference(policy.ReferenceId);
            if (initialDraftModel == null)
            {
                output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
                output.ErrorDescription = "No Wataniya Draft Policy Info created for this reference " + policy.ReferenceId;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }

            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                var testMode = _tameenkConfig.Policy.TestMode;
                if (testMode)
                {
                    const string nameOfFile = ".TestData.policyTestData.json";

                    string responseData = ReadResource(GetType().Namespace, nameOfFile);
                    HttpResponseMessage message = new HttpResponseMessage();
                    message.Content = new StringContent(responseData);
                    message.StatusCode = System.Net.HttpStatusCode.OK;
                    output.Output = message;
                    output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";

                    return output;
                }

                var wataniyaPolicyRequest = new WataniyaAutoleasePolicyRequest()
                {
                    QuotationNo = initialDraftModel.QuotationNumber
                };

                log.ServiceRequest = JsonConvert.SerializeObject(wataniyaPolicyRequest);
                request.ServiceRequest = log.ServiceRequest;

                stringPayload = JsonConvert.SerializeObject(wataniyaPolicyRequest);
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                var bankInfo = _bankNinsRepository.TableNoTracking.Where(b => b.NIN == policy.InsuredId.ToString()).FirstOrDefault();
                if (bankInfo != null)
                {
                    //if (bankInfo.BankId == 2)// Alyusr
                    //    _restfulConfiguration.AutoleasingAccessToken = "Yusr.BacreAutolease:Yusr@123";
                   if (bankInfo.BankId == 3)//Aljaber
                        _restfulConfiguration.AutoleasingAccessToken = "Aljabr_Bcare:Aljabr@123";
                }
                var _autoLeaseAccesToken = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(_restfulConfiguration.AutoleasingAccessToken));
                var postTask = _httpClient.PostAsyncWithCertificate(GET_AUTOLEASE_POLICY_URL, wataniyaPolicyRequest, AUTOLEASE_CERTIFCATE_BATH, AUTOLEASE_CERTIFCATE_PASSWORD, _autoLeaseAccesToken, authorizationMethod: "Basic");
                postTask.Wait();

                response = postTask.Result;

                DateTime dtBeforeCalling = DateTime.Now;
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null)
                {
                    if (request != null)
                    {
                        request.ErrorDescription = " service Return null";
                        _policyProcessingQueueRepository.Update(request);
                    }
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
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
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
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
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content result return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;
                request.ServiceResponse = log.ServiceResponse;

                var deserializeIssuePolicyResponse = JsonConvert.DeserializeObject<WataniyaAutoleasePolicyResponse>(response.Content.ReadAsStringAsync().Result);
                if (deserializeIssuePolicyResponse != null && deserializeIssuePolicyResponse.ErrorList != null && deserializeIssuePolicyResponse.ErrorList.Count > 0)
                {
                    StringBuilder servcieErrors = new StringBuilder();
                    StringBuilder servcieErrorsCodes = new StringBuilder();

                    foreach (var error in deserializeIssuePolicyResponse.ErrorList)
                    {
                        servcieErrors.AppendLine("Error Code: " + error.ErrorCode + " and the error message : " + error.ErrorMessage);
                        servcieErrorsCodes.AppendLine(error.ErrorCode);
                    }

                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
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
                if (string.IsNullOrEmpty(deserializeIssuePolicyResponse.PolicyNO))
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
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

                output.Output = HandleAutoleaseIssuePolicyResposne(initialDraftModel, deserializeIssuePolicyResponse);
                output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.PolicyNo = deserializeIssuePolicyResponse.PolicyNO;
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
                output.ErrorCode = ServiceOutput.ErrorCodes.ServiceException;
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

        private HttpResponseMessage HandleAutoleaseIssuePolicyResposne(WataniyaDraftPolicy initialDraftModel, WataniyaAutoleasePolicyResponse deserializeIssuePolicyResponse)
        {
            try
            {
                PolicyResponse policyResponse = new PolicyResponse();
                policyResponse.ReferenceId = initialDraftModel.ReferenceId.ToString();
                policyResponse.StatusCode = 1;
                policyResponse.PolicyNo = deserializeIssuePolicyResponse.PolicyNO;
                policyResponse.PolicyIssuanceDate = DateTime.Now;
                policyResponse.PolicyEffectiveDate = (initialDraftModel.PolicyEffectiveDate != null)
                                            ? Utilities.ConvertStringToDateTimeFromAllianz(initialDraftModel.PolicyEffectiveDate)
                                            : DateTime.Now.AddDays(1);
                policyResponse.PolicyExpiryDate = (initialDraftModel.PolicyExpiryDate != null)
                                            ? Utilities.ConvertStringToDateTimeFromAllianz(initialDraftModel.PolicyExpiryDate)
                                            : policyResponse.PolicyEffectiveDate.Value.AddYears(1).AddDays(-1);

                HttpResponseMessage response = new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(policyResponse), System.Text.Encoding.UTF8, "application/json")
                };

                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        #endregion

        #region Wataniya Najm Status
        public override ServiceOutput WataniyaNajmStatus(string policyNo, string referenceId, string customId, string sequenceNo)
        {
            ServiceOutput output = new ServiceOutput();
            ServiceRequestLog log = new ServiceRequestLog();
            log.PolicyNo = policyNo;
            if (string.IsNullOrEmpty(log.Channel))
                log.Channel = "Portal";
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "WataniyaNajmStatus";
            log.VehicleId = (!string.IsNullOrEmpty(customId)) ? customId : sequenceNo;
            log.CompanyName = _restfulConfiguration.ProviderName;
            log.CompanyID = 14;
            log.ServiceURL = NAJM_Status_URL;
            log.ServiceRequest = $"PolicyNo={policyNo}&CustomID={customId}&SequenceNo={sequenceNo}";
            try
            {
                var queryParameters = (!string.IsNullOrEmpty(customId)) ? $"?PolicyNo={policyNo}&CustomID={customId}" : $"?PolicyNo={policyNo}&SequenceNo={sequenceNo}";
                var najmServiceFullUrl = NAJM_Status_URL + queryParameters;
                var _najmStatusTokenBase46 = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(NAJM_Status_URL_BASE46_TOKEN));
                DateTime dtBeforeCalling = DateTime.Now;
                var response = _httpClient.GetWithCertificateAsync(najmServiceFullUrl, Najm_CERTIFCATE_BATH, Najm_CERTIFCATE_PASSWORD, _najmStatusTokenBase46, "Basic", null).Result;
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null)
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
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
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
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
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content result return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;
                var najmResponse = JsonConvert.DeserializeObject<WataniyaNajmStatusResponse>(response.Content.ReadAsStringAsync().Result);
                if (najmResponse != null && najmResponse.NajmStatus != "1" && najmResponse.ErrorList != null)
                {
                    StringBuilder servcieErrors = new StringBuilder();
                    StringBuilder servcieErrorsCodes = new StringBuilder();

                    servcieErrors.AppendLine("Error Code: " + najmResponse.ErrorList.ErrorCode + " and the error message : " + najmResponse.ErrorList.ErrorMessage);
                    servcieErrorsCodes.AppendLine(najmResponse.ErrorList.ErrorCode);

                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Quotation Service response error is : " + servcieErrors.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = servcieErrorsCodes.ToString();
                    log.ServiceErrorDescription = servcieErrors.ToString();
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                output.Output = najmResponse;
                output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                File.WriteAllText(@"C:\inetpub\WataniyaLog\wataniy_najm_" + policyNo + "_error.txt", JsonConvert.SerializeObject(ex.ToString()));
                output.ErrorCode = ServiceOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }

        #endregion

    }
}
