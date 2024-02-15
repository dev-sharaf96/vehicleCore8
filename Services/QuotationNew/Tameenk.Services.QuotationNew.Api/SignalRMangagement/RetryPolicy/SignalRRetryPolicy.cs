using Microsoft.AspNetCore.SignalR.Client;
using NLog;
using System;
using System.Web.Configuration;

namespace Tameenk.Services.QuotationNew.Api
{
    public class SignalRRetryPolicy : IRetryPolicy
    {

        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            double _seconds = 2;
            double.TryParse(WebConfigurationManager.AppSettings["SignalRServerReconnectAfterSeconds"], out _seconds);
            logger.Info(string.Format("SignalR server is not connected and it will re-connect after {0} seconds", _seconds));
            return TimeSpan.FromSeconds(_seconds);
        }
    }
}