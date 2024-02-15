using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace Tameenk.Services.QuotationNew.Api
{
    public class NLogOnfigurations
    {
        public static void Register(HttpConfiguration config)
        {
            SetNLogConfiguration();
        }

        private static void SetNLogConfiguration()
        {
            LoggingConfiguration config = LogManager.Configuration;
            var fileTarget = config?.FindTargetByName<FileTarget>("ratelimits_logs");
            if (fileTarget != null)
            {
                var applicationPoolName = GetApplicationPoolName();
                if (!string.IsNullOrEmpty(applicationPoolName))
                {
                    fileTarget.FileName = string.Format(@"{0}{1}-{2}.log", WebConfigurationManager.AppSettings["NLogBaseFilePath"], applicationPoolName, DateTime.Now.ToString("yyyy-MM-dd"));
                    LogManager.Configuration = config;
                }
            }
        }

        private static string GetApplicationPoolName()
        {
            try
            {
                return Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);
            }
            catch (Exception ex)
            {
                //logger.Error($"{nameof(GetApplicationPoolName)} throw Exception, and exception is: {ex.Message}");
            }

            return null;
        }
    }
}