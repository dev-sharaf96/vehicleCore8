using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Infrastructure;
using Tameenk.Integration.Core.Providers;
using Tameenk.Integration.Core.Providers.Configuration;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Logging;


namespace Tameenk.Integration.Providers.Malath
{
    public class MalathInsuranceProvider : RestfulInsuranceProvider
    {
   

        public MalathInsuranceProvider(TameenkConfig tameenkConfig, ILogger logger, IRepository<PolicyProcessingQueue> policyProcessingQueueRepository)
           : base(tameenkConfig, new RestfulConfiguration
           {
               GenerateQuotationUrl = "https://mig.malath.com.sa/BCareMotorService/Api/Quotation",
               GeneratePolicyUrl = "https://mig.malath.com.sa/BCareMotorService/Api/Policy",
               SchedulePolicyUrl = "https://mig.malath.com.sa/BCareMotorService/Api/PolicySchedule",
               AccessToken = "tameenkuser:ffjdkslKd@$k",
               ProviderName = "Malath",
               GenerateClaimRegistrationUrl = "",
               GenerateClaimNotificationUrl = "",
               GenerateAutoleasingQuotationUrl = "https://mig.malath.com.sa/BCareMotorService/api/Quotation",
               GenerateAutoleasingPolicyUrl = "https://mig.malath.com.sa/BCareMotorService/api/Policy",
               AutoleasingAccessToken = "TAMEENKUSER:ffjdkslKd@$k"
           }, logger, policyProcessingQueueRepository)
        {
        }

        protected override QuotationServiceRequest HandleQuotationRequestObjectMapping(QuotationServiceRequest quotation)
        {
           if(quotation!=null&quotation.VehicleEngineSizeCode==0)
            {
                quotation.VehicleEngineSizeCode = null;
            }
            if (quotation != null & (quotation.VehicleMajorColorCode == "99" || quotation.VehicleMajorColorCode.Contains("غير متوفر")))
            {
                quotation.VehicleMajorColorCode = "1";
                quotation.VehicleMajorColor = "أبيض";
            }
            return quotation;
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
                        if (product.Benefits.Count() == 0 && product.DeductibleValue == 2000)
                        {
                            BenefitDto agencyRepairs = new BenefitDto();
                            agencyRepairs.BenefitCode = 7;
                            agencyRepairs.BenefitId = "7";
                            agencyRepairs.BenefitNameAr = "أصلاح وكالة";
                            agencyRepairs.BenefitNameEn = "Agency Repairs";
                            agencyRepairs.BenefitPrice = 0;
                            agencyRepairs.IsReadOnly = true;
                            agencyRepairs.IsSelected = true;
                            agencyRepairs.BenefitDescEn = "Agency Repairs";
                            agencyRepairs.BenefitDescAr = "أصلاح وكالة";
                            product.Benefits.Add(agencyRepairs);
                        }
                        if (product.Benefits.Count() == 0 && product.DeductibleValue == 1000)
                        {
                            BenefitDto nonAgencyRepairs = new BenefitDto();
                            nonAgencyRepairs.BenefitCode = 15;
                            nonAgencyRepairs.BenefitId = "15";
                            nonAgencyRepairs.BenefitNameAr = "اصلاح ورش";
                            nonAgencyRepairs.BenefitNameEn = "NON AGENCY REPAIR";
                            nonAgencyRepairs.BenefitPrice = 0;
                            nonAgencyRepairs.IsReadOnly = true;
                            nonAgencyRepairs.IsSelected = true;
                            nonAgencyRepairs.BenefitDescEn = "NON AGENCY REPAIR";
                            nonAgencyRepairs.BenefitDescAr = "اصلاح ورش";
                            product.Benefits.Add(nonAgencyRepairs);
                        }
                        foreach (var benefit in product.Benefits)
                        {
                            if (benefit.BenefitPrice == 0)
                            {
                                benefit.IsReadOnly = true;
                                benefit.IsSelected = true;
                            }
                            if (benefit.BenefitCode == 7 && request.ProductTypeCode == 2 
                                && request.VehicleAgencyRepair.HasValue && request.VehicleAgencyRepair.Value)
                            {
                                benefit.IsReadOnly = true;
                                benefit.IsSelected = true;
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

            return responseValue;
        }
    }
}
