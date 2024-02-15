using NLog;
using WebApiThrottle;

namespace Tameenk.Services.Capcha.API
{
    public class CustomThrottleLogger : IThrottleLogger
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public void Log(ThrottleLogEntry entry)
        {
            logger.Warn(string.Format("{0} Request {1} from {2} has been throttled (blocked), quota {3}/{4} exceeded by {5} on URL {6}",
                                                    entry.LogDate, entry.RequestId, entry.ClientIp, entry.RateLimit, entry.RateLimitPeriod, entry.TotalRequests, entry.Request.RequestUri));
        }
    }
}