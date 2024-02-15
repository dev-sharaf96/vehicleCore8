using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Exceptions;
using Tameenk.Integration.Core.Providers;
using Tameenk.Integration.Core.Providers.Configuration;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Logging;
using System.Linq;
using Tameenk.Services.Core.Addresses;
using Tameenk.Core.Domain.Entities;
using System.Dynamic;
using Tameenk.Services.Core.Http;
using Tameenk.Core.Infrastructure;
using System.Net.Http;

namespace Tameenk.Integration.Providers.GGI
{

    public class GGIInsuranceProvider : RestfulInsuranceProvider
    {

        #region Fields
        private readonly TameenkConfig _tameenkConfig;
        private readonly IHttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly string _accessTokenBase64;
        private readonly RestfulConfiguration _restfulConfiguration;
        private readonly IRepository<PolicyProcessingQueue> _policyProcessingQueueRepository;
        private readonly IRepository<CheckoutDetail> _checkoutDetail;
        private readonly IAddressService _addressService;
        private readonly int[] Weights = new int[5] { 21, 22, 23, 24, 25 };

        private const string QUOTATION_TPL_URL = "https://bcarel.ggi-sa.com/GGIBcareLive/API/MotorService/Quote";
        private const string QUOTATION_COMPREHENSIVE_URL = "https://bcarel.ggi-sa.com/GGIBcareLive/API/MotorService/Quote";
        private const string POLICY_TPL_URL = "https://bcarel.ggi-sa.com/GGIBcareLive/Api/MotorService/TPLPolicy";
        private const string POLICY_COMPREHENSIVE_URL = "https://bcarel.ggi-sa.com/GGIBcareLive/Api/MotorService/TPLPolicy";


        #endregion

        #region Ctor
        public GGIInsuranceProvider(TameenkConfig tameenkConfig, ILogger logger, IRepository<PolicyProcessingQueue> policyProcessingQueueRepository
            , IAddressService addressService,IRepository<CheckoutDetail> checkoutDetail)
             : base(tameenkConfig, new RestfulConfiguration
             {
                 GenerateQuotationUrl = "",
                 GeneratePolicyUrl = "",
                 AccessToken = "GGCIBcareLive:@GGCIBcarePa$$w0rdLive",
                 ProviderName = "GGI",
                 GenerateAutoleasingQuotationUrl = "https://bcarel.ggi-sa.com/GGIBcareLive/API/Leasing/Quote",
                 GenerateAutoleasingPolicyUrl = "https://bcarel.ggi-sa.com/GGIBcareLive/Api/Leasing/Policy",
                 AutoleasingAccessToken = "GGCIBcareLive:@GGCIBcarePa$$w0rdLive"
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
            _checkoutDetail = checkoutDetail;

        }

        #endregion

        #region Methods

        public object SubmitQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog log)
        {
            return ExecuteQuotationRequest(quotation, log);
        }
        protected override object ExecuteQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            var configuration = Configuration as RestfulConfiguration;
            //change the quotation url to tpl in case product type code = 1
            if (quotation.ProductTypeCode == 1)
            {
                configuration.GenerateQuotationUrl = QUOTATION_TPL_URL;
            }
            else
            {
                configuration.GenerateQuotationUrl = QUOTATION_COMPREHENSIVE_URL;// QUOTATION_COMPREHENSIVE_URL;
            }
            return base.ExecuteQuotationRequest(quotation, predefinedLogInfo);
        }


        public object SubmitPolicyRequest(PolicyRequest request, ServiceRequestLog log)
        {
            return ExecutePolicyRequest(request, log);
        }
        protected override object ExecutePolicyRequest(PolicyRequest request, ServiceRequestLog predefinedLogInfo)
        {
            short productTypeCode = 1;
            var info = _checkoutDetail.Table.Where(a => a.ReferenceId == request.ReferenceId).FirstOrDefault();
            if (info != null)
            {
                if (info.SelectedInsuranceTypeCode.HasValue)
                    productTypeCode = info.SelectedInsuranceTypeCode.Value;
            }
            var configuration = Configuration as RestfulConfiguration;
            //change the quotation url to tpl in case product type code = 1
            if (productTypeCode == 1)
            {
                configuration.GeneratePolicyUrl = POLICY_TPL_URL;
            }
            else
            {
                configuration.GeneratePolicyUrl = POLICY_COMPREHENSIVE_URL;
            }
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
                                //if (benefit.BenefitCode == 5)//change benfit code from 1 to 10 as per waleed 
                                //{
                                //    benefit.IsReadOnly = true;
                                //    benefit.IsSelected = true;
                                //    benefit.BenefitPrice = 0;
                                //}
                            }
                        }
                    }
                }



                HandleFinalProductPrice(responseValue);
            }
            catch (Exception ex)
            {
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

        protected override object ExecuteAutoleasingQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            ServiceOutput output = SubmitAutoleasingQuotationRequest(quotation, predefinedLogInfo);
            if (output.ErrorCode != ServiceOutput.ErrorCodes.Success)
            {
                return null;
            }

            return output.Output;
        }

        protected override object ExecuteAutoleasingPolicyRequest(PolicyRequest policy, ServiceRequestLog predefinedLogInfo)
        {
            ServiceOutput output = SubmitAutoleasingPolicyRequest(policy, predefinedLogInfo);
            if (output.ErrorCode != ServiceOutput.ErrorCodes.Success)
            {
                return null;
            }

            return output.Output;
        }

    }
}
