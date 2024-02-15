using System;

namespace Tameenk.Log4Net
{
    public interface ILog4NetManager
    {
        void Log(string message, Exception exception, LogLevel level = LogLevel.Error);
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