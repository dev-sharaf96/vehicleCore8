﻿using log4net;
using System;
using System.Reflection;


namespace Tameenk.Log4Net
{
    public class Log4NetManager : ILog4NetManager
    {
        static Log4NetManager? _instance;
        private readonly ILog _log;

        private Log4NetManager()
        {
            this._log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        }

        public static Log4NetManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Log4NetManager();
                }

                return _instance;
            }
        }

        public void Log(string message, Exception? exception = null, LogLevel level = LogLevel.Error)
        {
            try
            {
                switch (level)
                {
                    case LogLevel.Debug: _log.Debug(message, exception); break;
                    case LogLevel.Info: _log.Info(message, exception); break;
                    case LogLevel.Warning: _log.Warn(message, exception); break;
                    case LogLevel.Error: _log.Error(message, exception); break;
                    case LogLevel.Fatal: _log.Fatal(message, exception); break;
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }

       //public void Log(string message, Exception exception, LogLevel level = LogLevel.Error)
       //{
       //    throw new NotImplementedException();
       //}
    }
}