using System.Collections.Generic;
using System.Linq;
using Tameenk.Core.Configuration;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Infrastructure;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Infrastructure;
using Tameenk.Integration.Core.Providers;
using Tameenk.Integration.Core.Providers.Configuration;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Resources.Quotations;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Logging;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Loggin.DAL;
using System.Net.Http;
using Newtonsoft.Json;

namespace Tameenk.Integration.Providers.Salama
{
    public class SalamaInsuranceProvider : RestfulInsuranceProvider
    {
        private const string QUOTATION_COMPREHENSIVE_URL = "http://37.99.174.44:6800/api/CompQuotation/RequestQuotation";
        private const string POLICY_COMPREHENSIVE_URL = "http://37.99.174.44:6800/api/CompPolicy/RequestPolicy";
        private readonly IRepository<CheckoutDetail> _checkoutDetail;
        public SalamaInsuranceProvider(TameenkConfig tameenkConfig, ILogger logger, IRepository<PolicyProcessingQueue> policyProcessingQueueRepository, IRepository<CheckoutDetail> checkoutDetail)
             : base(tameenkConfig, new RestfulConfiguration
             {
                 GenerateQuotationUrl = "http://37.99.174.44:6800/api/Quotation/RequestQuotation",
                 GeneratePolicyUrl = "http://37.99.174.44:6800/api/Policy/RequestPolicy",
                 AccessToken = "",
                 ProviderName = "Salama"
             }, logger, policyProcessingQueueRepository)
        {
            _checkoutDetail = checkoutDetail;
        }

        protected override QuotationServiceRequest HandleQuotationRequestObjectMapping(QuotationServiceRequest quotation)
        {
            if (quotation.VehicleValue.HasValue&& quotation.VehicleValue.Value <20000)
                quotation.VehicleValue = 20000;

            if (quotation.InsuredOccupationCode == "G")
                quotation.InsuredOccupationCode = "01";

            else if (quotation.InsuredOccupationCode == "O")
                quotation.InsuredOccupationCode = "02";

            foreach (var driver in quotation.Drivers)
            {
                if (driver.DriverOccupationCode == "G")
                    driver.DriverOccupationCode = "01";

                else if (driver.DriverOccupationCode == "O")
                    driver.DriverOccupationCode = "02";
            }
            return quotation;
        }

        public override bool ValidateQuotationBeforeCheckout(QuotationRequest quotationRequest, out List<string> errors)
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
        protected override object ExecuteQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            var configuration = Configuration as RestfulConfiguration;
            //change the quotation url to tpl in case product type code = 1
            if (quotation.ProductTypeCode ==2)
            {
                configuration.GenerateQuotationUrl = QUOTATION_COMPREHENSIVE_URL;
            }
            return base.ExecuteQuotationRequest(quotation, predefinedLogInfo);
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
            if (productTypeCode == 2)
            {
                configuration.GeneratePolicyUrl = POLICY_COMPREHENSIVE_URL;
            }
            return base.ExecutePolicyRequest(request, predefinedLogInfo);
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
        

    }
}
