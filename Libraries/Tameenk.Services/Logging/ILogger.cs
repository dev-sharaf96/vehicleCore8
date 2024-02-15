using System;

namespace Tameenk.Services.Logging
{
    public interface ILogger
    {
        void Log(string message, LogLevel level = LogLevel.Info);

        void Log(string message, Exception exception, LogLevel level = LogLevel.Error);

        /// <summary>
        /// Log into integration transaction table
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inputParams"></param>
        /// <param name="outputParams"></param>
        void LogIntegrationTransaction(string message, string inputParams, string outputParams, int? status=null);




    }
    public enum LogLevel
    {
        Debug = 1,
        Info = 10,
        Warning = 20,
        Error = 30,
        Fatal = 40
    }
}
