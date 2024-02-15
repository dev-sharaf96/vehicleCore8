using log4net;
using System;
using System.Threading.Tasks;

namespace Tameenk.Services.Logging
{
    public class Log4netLogger : ILogger
    {
        private readonly ILog _log;
        private readonly ILog _IntegrationTransactionLog;

        public Log4netLogger()
        {
            _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            _IntegrationTransactionLog = LogManager.GetLogger("IntegrationTransaction");
        }


        public void Log(string message, LogLevel level = LogLevel.Info)
        {
            Task.Run(() =>
            {
                switch (level)
                {
                    case LogLevel.Debug: _log.Debug(message); break;
                    case LogLevel.Info: _log.Info(message); break;
                    case LogLevel.Warning: _log.Warn(message); break;
                    case LogLevel.Error: _log.Error(message); break;
                    case LogLevel.Fatal: _log.Fatal(message); break;
                }
            });
           
        }

        public void Log(string message, Exception exception, LogLevel level = LogLevel.Error)
        {
            Task.Run(() =>
            {
                switch (level)
                {
                    case LogLevel.Debug: _log.Debug(message, exception); break;
                    case LogLevel.Info: _log.Info(message, exception); break;
                    case LogLevel.Warning: _log.Warn(message, exception); break;
                    case LogLevel.Error: _log.Error(message, exception); break;
                    case LogLevel.Fatal: _log.Fatal(message, exception); break;
                }
            });
           
        }

        public void LogIntegrationTransaction(string message, string inputParams, string outputParams, int? status=null)
        {
            //Task.Run(() =>
            //{
            //    LogicalThreadContext.Properties["InputParams"] = inputParams;
            //    LogicalThreadContext.Stacks["OutputParams"].Push(outputParams);
            //    LogicalThreadContext.Properties["StatusId"] = status;
            //    _IntegrationTransactionLog.Info(message);

            //});

        }
    }
}
