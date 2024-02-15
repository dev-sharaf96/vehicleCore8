using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Infrastructure;
using Tameenk.Integration.Core.Providers;
using Tameenk.Integration.Core.Providers.Configuration;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Logging;

namespace Tameenk.Integration.Providers.Amana
{
    public class AmanaInsuranceProvider : RestfulInsuranceProvider
    {
        #region Fields
        private readonly TameenkConfig _tameenkConfig;
        private readonly IHttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly string _accessTokenBase64;
        private readonly RestfulConfiguration _restfulConfiguration;
        private readonly IRepository<PolicyProcessingQueue> _policyProcessingQueueRepository;
        private const string QUOTATION_TPL_URL = "https://prodbcare.amana-coop.com.sa:443/api/Quotes";
        private const string QUOTATION_COMPREHENSIVE_URL = "https://prodbcare.amana-coop.com.sa:444/api/Quotes";
        private const string POLICY_TPL_URL = "https://prodbcare.amana-coop.com.sa:443/api/PurchaseNotifications";
        private const string POLICY_COMPREHENSIVE_URL = "https://prodbcare.amana-coop.com.sa:444/api/PurchaseNotifications";
        private readonly IRepository<CheckoutDetail> _checkoutDetail;
        #endregion
        public AmanaInsuranceProvider(TameenkConfig tameenkConfig, ILogger logger, IRepository<PolicyProcessingQueue> policyProcessingQueueRepository, IRepository<CheckoutDetail> checkoutDetail)
          : base(tameenkConfig, new RestfulConfiguration
          {
              GenerateQuotationUrl = "https://prodbcare.amana-coop.com.sa/api/Quotes",
              GeneratePolicyUrl= "https://prodbcare.amana-coop.com.sa/api/PurchaseNotifications",
              GenerateAutoleasingQuotationUrl = "https://prodbcare.amana-coop.com.sa:446/api/Quotes",
              GenerateAutoleasingPolicyUrl = "https://prodbcare.amana-coop.com.sa:446/api/PurchaseNotifications",
              AccessToken = "prod:P@ssw0rd",
              AutoleasingAccessToken = "prod:P@ssw0rd",
              ProviderName = "Amana"
            
          }, logger, policyProcessingQueueRepository)
        {
            _restfulConfiguration = Configuration as RestfulConfiguration;
            _tameenkConfig = tameenkConfig;
            _httpClient = EngineContext.Current.Resolve<IHttpClient>();
            _logger = logger;
            _accessTokenBase64 = string.IsNullOrWhiteSpace(_restfulConfiguration.AccessToken) ?
               null : Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(_restfulConfiguration.AccessToken));
            _policyProcessingQueueRepository = policyProcessingQueueRepository;
            _checkoutDetail = checkoutDetail;

        }

        protected override QuotationServiceResponse GetQuotationResponseObject(object response, QuotationServiceRequest request)
        {
            QuotationServiceResponse responseValue = new QuotationServiceResponse();
            string result = string.Empty;
            result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
            responseValue = JsonConvert.DeserializeObject<QuotationServiceResponse>(result);
            if (responseValue != null && responseValue.Products != null)
            {
                foreach (var product in responseValue.Products)
                {
                    if (product != null)
                    {
                        if (product.Benefits != null)
                        {
                            foreach (var benefit in product.Benefits)
                            {
                                if (benefit.BenefitPrice == 0)// added as per waleed 
                                {
                                    benefit.IsReadOnly = true;
                                    benefit.IsSelected = true;
                                }
                            }
                        }
                    }
                }
            }
            return responseValue;
        }

        protected override object ExecuteQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            var configuration = Configuration as RestfulConfiguration;
            if (quotation.ProductTypeCode == 1)
            {
                configuration.GenerateQuotationUrl = QUOTATION_TPL_URL;
            }
            else
            {
                configuration.GenerateQuotationUrl = QUOTATION_COMPREHENSIVE_URL;
            }
            return base.ExecuteQuotationRequest(quotation, predefinedLogInfo);
        }

        protected override object ExecutePolicyRequest(PolicyRequest policy, ServiceRequestLog predefinedLogInfo)
        {

            short productTypeCode = 1;
            var info = _checkoutDetail.TableNoTracking.Where(a => a.ReferenceId == policy.ReferenceId).FirstOrDefault();
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
            return base.ExecutePolicyRequest(policy, predefinedLogInfo);
        }

        protected override QuotationServiceResponse GetAutoleasingQuotationResponseObject(object response, QuotationServiceRequest request)
        {
            QuotationServiceResponse responseValue = new QuotationServiceResponse();
            string result = string.Empty;
            var stringPayload = result;
            var res = string.Empty;
            try
            {
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
                responseValue = JsonConvert.DeserializeObject<QuotationServiceResponse>(result);

                if (responseValue != null && responseValue.Products == null && (responseValue.Errors != null && responseValue.Errors.Count > 0))
                {
                    responseValue.Errors = new List<Error> { new Error { Message = result } };
                }

                else
                {
                    responseValue.Errors = null;
                    if (responseValue != null && responseValue.Products != null)
                    {
                        foreach (var product in responseValue.Products)
                        {
                            if (product != null && product.Benefits != null)
                            {
                                foreach (var benefit in product.Benefits)
                                {
                                    if (benefit.BenefitPrice == 0)
                                    {
                                        benefit.IsReadOnly = true;
                                        benefit.IsSelected = true;
                                    }
                                }
                            }
                        }
                    }
                }
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
                LogIntegrationTransaction($"Test Get Quotation with reference id: {request.ReferenceId} for company: TokioMarine Comprehensive", stringPayload, responseValue, responseValue?.StatusCode);
            }

            return responseValue;
        }
    }
}
