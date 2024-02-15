using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tameenk.Core.Configuration;
using Tameenk.Services.Core.Http;

namespace Tameenk.Services.Implementation.Http
{
    public class ResilientHttpClientFactory : IResilientHttpClientFactory
    {
        //private readonly ILogger<ResilientHttpClient> _logger;
        private readonly int _retryCount;
        private readonly int _exceptionsAllowedBeforeBreaking;

        public ResilientHttpClientFactory(TameenkConfig config)
        {
            //_logger = logger;
            _exceptionsAllowedBeforeBreaking = config.HttpClient.ExceptionsAllowedBeforeBreaking;
            _retryCount = config.HttpClient.RetryCount;
        }


        public IHttpClient CreateResilientHttpClient()
            => new ResilientHttpClient((origin) => CreatePolicies());

        private Policy[] CreatePolicies()
            => new Policy[]
            {
                Policy.Handle<HttpRequestException>()
                .WaitAndRetryAsync(
                    // number of retries
                    _retryCount,
                    // exponential backofff
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    // on retry
                    (exception, timeSpan, retryCount, context) =>
                    {
                        var msg = $"Retry {retryCount} implemented with Polly's RetryPolicy " +
                            $"of {context.PolicyKey} " +
                            $"at {context.OperationKey}, " +
                            $"due to: {exception}.";
                        //_logger.LogWarning(msg);
                        //_logger.LogDebug(msg);
                    }),
                Policy.Handle<HttpRequestException>()
                .CircuitBreakerAsync( 
                   // number of exceptions before breaking circuit
                   _exceptionsAllowedBeforeBreaking,
                   // time circuit opened before retry
                   TimeSpan.FromMinutes(1),
                   (exception, duration) =>
                   {
                        // on circuit opened
                        //_logger.LogTrace("Circuit breaker opened");
                   },
                   () =>
                   {
                        // on circuit closed
                        //_logger.LogTrace("Circuit breaker reset");
                   })
            };
    }
}
