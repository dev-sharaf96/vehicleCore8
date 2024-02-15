using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Infrastructure;
using Tameenk.Integration.Core.Providers;
using Tameenk.Integration.Core.Providers.Configuration;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Logging;

namespace Tameenk.Integration.Providers.GGI
{
    internal class GGIInsuranceComprehensiveProvider : RestfulInsuranceProvider
    {

        #region Fields
        private readonly TameenkConfig _tameenkConfig;
        private readonly IHttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly string _accessTokenBase64;
        private readonly RestfulConfiguration _restfulConfiguration;
        private readonly IRepository<PolicyProcessingQueue> _policyProcessingQueueRepository;
        private readonly IAddressService _addressService;
        private readonly int[] Weights = new int[5] { 21, 22, 23, 24, 25 };

        #endregion

        #region Ctor
        public GGIInsuranceComprehensiveProvider(TameenkConfig tameenkConfig, ILogger logger, IRepository<PolicyProcessingQueue> policyProcessingQueueRepository
            , IAddressService addressService)
             : base(tameenkConfig, new RestfulConfiguration
             {
                 GenerateQuotationUrl = "https://eportal.ggi-sa.com/eportal/tameenk/Quotation/Comprehensive/Request",
                 GeneratePolicyUrl = "https://eportal.ggi-sa.com/eportal/tameenk/Policy/Comprehensive/Request",
                 AccessToken = "ggmotor:ggmotor@123#",
                 ProviderName = "GGI"
             }, logger, policyProcessingQueueRepository)
        {
            _restfulConfiguration = Configuration as RestfulConfiguration;
            _tameenkConfig = tameenkConfig;
            _httpClient = EngineContext.Current.Resolve<IHttpClient>();
            _logger = logger;
            _accessTokenBase64 = string.IsNullOrWhiteSpace(_restfulConfiguration.AccessToken) ?
               null : Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(_restfulConfiguration.AccessToken));
            _policyProcessingQueueRepository = policyProcessingQueueRepository;
            _addressService = addressService ?? throw new TameenkArgumentNullException(nameof(IAddressService));

        }

        #endregion

        #region Methods

        public object SubmitQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog log)
        {
            return ExecuteQuotationRequest(quotation, log);
        }
        protected override object ExecuteQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            return base.ExecuteQuotationRequest(quotation, predefinedLogInfo);
        }


        public object SubmitPolicyRequest(PolicyRequest request, ServiceRequestLog log)
        {
            return ExecutePolicyRequest(request, log);
        }
        protected override object ExecutePolicyRequest(PolicyRequest request, ServiceRequestLog predefinedLogInfo)
        {
            return base.ExecutePolicyRequest(request, predefinedLogInfo);
        }

        public PolicyResponse GetPolicyResponse(object response, PolicyRequest request = null)
        {
            return GetPolicyResponseObject(response, request);
        }

        protected override PolicyResponse GetPolicyResponseObject(object response, PolicyRequest request = null)
        {
            return base.GetPolicyResponseObject(response, request);
        }

        public QuotationServiceResponse GetComprehensiveQuotationResponseObject(object response, QuotationServiceRequest request)
        {
            return GetQuotationResponseObject(response, request);
        }
        protected override QuotationServiceResponse GetQuotationResponseObject(object response, QuotationServiceRequest request)
        {
            QuotationServiceResponse responseValue = new QuotationServiceResponse();
            string result = string.Empty;
            var stringPayload = result;
            var res = string.Empty;

            try
            {

                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;

                responseValue = JsonConvert.DeserializeObject<QuotationServiceResponse>(result);


                if (responseValue != null && responseValue.Products != null)
                {
                    foreach (var product in responseValue.Products)
                    {
                        if (product != null && product.Benefits != null)
                        {
                            foreach (var benefit in product.Benefits)
                            {
                                if (benefit.BenefitPrice == 0)// added as per waleed 
                                {
                                    benefit.IsReadOnly = true;
                                    benefit.IsSelected = true;
                                }
                                if (benefit.BenefitCode == 10 || benefit.BenefitCode == 5)//change benfit code from 1 to 10 as per waleed 
                                {
                                    benefit.IsReadOnly = true;
                                    benefit.IsSelected = true;
                                    benefit.BenefitPrice = 0;
                                }
                            }
                        }
                    }
                }



                HandleFinalProductPrice(responseValue);



                //if (responseValue.Errors != null)
                //    responseValue.Errors.Insert(0, new Error { Message = stringPayload });
            }
            catch (Exception ex)
            {
                _logger.Log("GGIInsuranceComprehensiveProvider -> GetQuotationResponseObject", ex, LogLevel.Error);
                responseValue.StatusCode = 2;
                if (responseValue.Errors == null)
                    responseValue.Errors = new List<Error>();

                responseValue.Errors.Add(new Error { Message = ex.GetBaseException().Message });
            }
            finally
            {
                LogIntegrationTransaction($"Test Get Quotation with reference id: {request.ReferenceId} for company: GGI Comprehensive", stringPayload, responseValue, responseValue?.StatusCode);
            }

            return responseValue;
        }

        protected override QuotationServiceRequest HandleQuotationRequestObjectMapping(QuotationServiceRequest quotation)
        {
            quotation.VehicleWeight = Weights[quotation.VehicleWeight / 1000];
            return quotation;
        }




        #endregion

    }
}
