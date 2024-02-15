using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using WebApiThrottle;

namespace Tameenk.Services.QuotationNew.Api
{
    public class RateLimitConfiguration
    {
        public static void Register(HttpConfiguration config)
        {
            config.MessageHandlers.Add(new ThrottlingHandler()
            {
                //// Generic rate limit applied to ALL APIs
                //Policy = new ThrottlePolicy(perSecond: 1 * limit, perMinute:  60 * limit, perHour: 60 * 60 * limit)
                //{
                //    IpThrottling = true,
                //    ClientThrottling = true,
                //    EndpointThrottling = true,
                //    StackBlockedRequests = true,
                //    IpWhitelist = new List<string> {  },
                //    //EndpointRules = new Dictionary<string, RateLimits>
                //    //{
                //    //    //{ "QuotationApiSagr/api/quote", new RateLimits{ PerSecond= 1, PerMinute= 5, PerHour= 200 } }
                //    //    //, { "QuotationApiAlRajhi/api/quote", new RateLimits{ PerSecond= 1, PerMinute= 5, PerHour= 200 } }
                //    //    //, { "QuotationApiMedGulf/api/quote", new RateLimits{ PerSecond= 1, PerMinute= 5, PerHour= 200 } }
                //    //    //, { "QuotationApiUCA/api/quote", new RateLimits{ PerSecond= 1, PerMinute= 5, PerHour= 200 } }
                //    //    //, { "QuotationApiTUIC/api/quote", new RateLimits{ PerSecond= 1, PerMinute= 5, PerHour= 200 } }
                //    //    //, { "QuotationApiGulfUnion/api/quote", new RateLimits{ PerSecond= 1, PerMinute= 5, PerHour= 200 } }
                //    //    //, { "QuotationApiAlalamiya/api/quote", new RateLimits{ PerSecond= 1, PerMinute= 5, PerHour= 200 } }
                //    //    //, { "QuotationApiSolidarity/api/quote", new RateLimits{ PerSecond= 1, PerMinute= 5, PerHour= 200 } }
                //    //    //, { "QuotationApiAICC/api/quote", new RateLimits{ PerSecond= 1, PerMinute= 5, PerHour= 200 } }
                //    //    //, { "QuotationApiGGI/api/quote", new RateLimits{ PerSecond= 1, PerMinute= 5, PerHour= 200 } }
                //    //    //, { "QuotationApiAmana/api/quote", new RateLimits{ PerSecond= 1, PerMinute= 5, PerHour= 200 } }
                //    //    //, { "QuotationApiArabianShield/api/quote", new RateLimits{ PerSecond= 1, PerMinute= 5, PerHour= 200 } }
                //    //    //, { "QuotationApiBuruj/api/quote", new RateLimits{ PerSecond= 1, PerMinute= 5, PerHour= 200 } }
                //    //    //, { "QuotationApiACIG/api/quote", new RateLimits{ PerSecond= 1, PerMinute= 5, PerHour= 200 } }
                //    //    //, { "QuotationApiMalath/api/quote", new RateLimits{ PerSecond= 1, PerMinute= 5, PerHour= 200 } }
                //    //    //, { "QuotationApiAXA/api/quote", new RateLimits{ PerSecond= 1, PerMinute= 5, PerHour= 200 } }
                //    //    //, { "QuotationApiTawuniya/api/quote", new RateLimits{ PerSecond= 1, PerMinute= 5, PerHour= 200 } }
                //    //    //, { "QuotationApiSalama/api/quote", new RateLimits{ PerSecond= 1, PerMinute= 5, PerHour= 200 } }
                //    //    //, { "QuotationApiWalaa/api/quote", new RateLimits{ PerSecond= 1, PerMinute= 5, PerHour= 200 } }
                //    //    //, { "QuotationApiAllianz/api/quote", new RateLimits{ PerSecond= 1, PerMinute= 5, PerHour= 200 } }
                //    //    //, { "QuotationApiWataniya/api/quote", new RateLimits{ PerSecond= 1, PerMinute= 5, PerHour= 200 } }
                //    //    //, { "QuotationApiSAICO/api/quote", new RateLimits{ PerSecond= 1, PerMinute= 5, PerHour= 200 } }
                //    //},
                //},
                Policy = ThrottlePolicy.FromStore(new PolicyConfigurationProvider()),
                Repository = new CacheRepository(),
                QuotaExceededResponseCode = (HttpStatusCode)RateLimitCustomHttpStatusCodeEnum.ExceedRateLimit,
                QuotaExceededMessage = "Access is not allowed please try again later",
                Logger = new CustomThrottleLogger()
            });
        }
    }

    public enum RateLimitCustomHttpStatusCodeEnum
    {
        ExceedRateLimit = 420
    }
}