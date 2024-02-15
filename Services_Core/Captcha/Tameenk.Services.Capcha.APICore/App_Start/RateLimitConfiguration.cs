using System.Net;
using WebApiThrottle;

namespace Tameenk.Services.Capcha.API
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