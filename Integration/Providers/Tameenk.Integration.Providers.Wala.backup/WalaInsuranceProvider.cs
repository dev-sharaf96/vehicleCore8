using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Infrastructure;
using Tameenk.Integration.Core.Providers;
using Tameenk.Integration.Core.Providers.Configuration;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Integration.Providers.Wala.Resources;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Quotations;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Logging;

namespace Tameenk.Integration.Providers.Wala
{
    public class WalaInsuranceProvider : RestfulInsuranceProvider
    {
        #region Fields
        private readonly TameenkConfig _tameenkConfig;
        private readonly RestfulConfiguration _restfulConfiguration;
        private readonly IRepository<CheckoutDetail> _checkoutDetail;
        private readonly IRepository<PolicyProcessingQueue> _policyProcessingQueueRepository;
        private readonly ILogger _logger;
        //private const string QUOTATION_TPL_URL = "https://online.walaa.com/bcaremotor/Walaa/TPL/QuotationRequest";
        //private const string POLICY_TPL_URL = "https://online.walaa.com/bcaremotor/Walaa/TPL/PolicyRequest";
        private const string QUOTATION_TPL_URL = "https://bcaresvc01.walaa.com:8083/Walaa/TPL/QuotationRequest";
        private const string POLICY_TPL_URL = "https://bcaresvc01.walaa.com:8083/Walaa/TPL/PolicyRequest";

        //private const string QUOTATION_COMPREHENSIVE_URL = "https://online.walaa.com/BCareMotor/Walaa/CMP/QuotationRequest";
        //private const string POLICY_COMPREHENSIVE_URL = "https://online.walaa.com/BCareMotor/Walaa/CMP/PolicyRequest";
        private const string QUOTATION_COMPREHENSIVE_URL = "https://bcaresvc01.walaa.com:8083/Walaa/CMP/QuotationRequest";
        private const string POLICY_COMPREHENSIVE_URL = "https://bcaresvc01.walaa.com:8083/Walaa/CMP/PolicyRequest";
        #endregion
        public WalaInsuranceProvider(TameenkConfig tameenkConfig, ILogger logger, IRepository<PolicyProcessingQueue> policyProcessingQueueRepository, IRepository<CheckoutDetail> checkoutDetail)
            : base(tameenkConfig, new RestfulConfiguration
            {

                GenerateQuotationUrl = "",
                GeneratePolicyUrl = "",
                AccessToken = "BcareWalaa@Motor:Bcar3PRW@1aa",
                ProviderName = "Walaa",
                GenerateClaimRegistrationUrl = "",
                GenerateClaimNotificationUrl = "",
                AutoleasingCancelPolicyUrl = "https://online.walaa.com/Motorbcarelease/Walaa/CMP/PolicyCancellation",
                GenerateAutoleasingQuotationUrl = "https://online.walaa.com/MotorBcareLease/Walaa/CMP/QuotationRequest",
                GenerateAutoleasingPolicyUrl = "https://online.walaa.com/MotorBcareLease/Walaa/CMP/PolicyRequest",
                SchedulePolicyUrl = "https://online.walaa.com/MotorBcareLease/Walaa/CMP/PolicyScheduleService",
                AutoleasingAccessToken = "BcareLease:PRBcare@Motor",
                AutoleaseUpdateCustomCardUrl = "https://online.walaa.com/Motorbcarelease/Walaa/CMP/UpdateCustomCard",
                AddDriverUrl= "https://online.walaa.com/Motorbcarelease/Walaa/CMP/AddDriverService",
                PurchaseDriverUrl= "https://online.walaa.com/Motorbcarelease/Walaa/CMP/PurchaseAddDriverService",
                AddBenifitUrl= "https://online.walaa.com/Motorbcarelease/Walaa/CMP/AddBenefitService",
                PurchaseBenifitUrl= "https://online.walaa.com/Motorbcarelease/Walaa/CMP/PurchaseBenefitService",
                CancelPolicyAccessTokenAutoLeasing = "BcareLease:PRBcare@Motor",
            },  logger , policyProcessingQueueRepository)
        {
            _restfulConfiguration = Configuration as RestfulConfiguration;
            _tameenkConfig = tameenkConfig;
            _logger = logger;
            _checkoutDetail = checkoutDetail;
            _policyProcessingQueueRepository = policyProcessingQueueRepository;
        }

        public override bool ValidateQuotationBeforeCheckout(QuotationRequest quotationRequest, out List<string> errors)
        {
            var addressService = EngineContext.Current.Resolve<IAddressService>();
            errors = new List<string>();
            var mainDriverAddress = quotationRequest.Driver.Addresses.FirstOrDefault();
            if (mainDriverAddress != null)
            {
                var nationalAddressCity = addressService.GetCityCenterById(mainDriverAddress.CityId);
                //var insuredCityCenter = addressService.GetCityCenterCenterByElmCode(quotationRequest.Insured.CityId.ToString());
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
                configuration.GenerateQuotationUrl = QUOTATION_COMPREHENSIVE_URL;
            }
            return base.ExecuteQuotationRequest(quotation, predefinedLogInfo);
        }

        protected override object ExecutePolicyRequest(PolicyRequest request, ServiceRequestLog predefinedLogInfo)
        {
            short productTypeCode = 1;
            var info = _checkoutDetail.TableNoTracking.Where(a => a.ReferenceId == request.ReferenceId).FirstOrDefault();
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

    }
}
