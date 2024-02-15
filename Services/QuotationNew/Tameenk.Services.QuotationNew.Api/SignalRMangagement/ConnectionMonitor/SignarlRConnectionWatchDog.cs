//using NLog;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Timers;
//using System.Web;
//using System.Web.Configuration;

//namespace Tameenk.Services.QuotationNew.Api
//{
//    public class SignarlRConnectionWatchDog
//    {
//        static SignarlRConnectionWatchDog _instance;
//        private readonly Logger logger;
//        private readonly Timer timer;

//        public SignarlRConnectionWatchDog()
//        {
//            this.logger = LogManager.GetCurrentClassLogger();
//            timer = new Timer();
//        }


//        public static SignarlRConnectionWatchDog Instance
//        {
//            get
//            {
//                if (_instance == null)
//                    _instance = new SignarlRConnectionWatchDog();
//                return _instance;
//            }
//        }





//        public void InitWatchDogTimer()
//        {
//            timer.Elapsed += CheckServerState;
//            timer.Interval = 1000 * int.Parse(WebConfigurationManager.AppSettings["SignalRServerCheckStateAfterSeconds"]);
//            timer.Enabled = true;
//            timer.Start();
//        }

//        private void CheckServerState(object sender, ElapsedEventArgs e)
//        {
//            bool isServerConnected = IsServerConnected();
//            if (isServerConnected)
//            {
//                logger.Error($"CheckServerState: isServerConnected is {isServerConnected} ");
//                KillWatchDog();
//                return;
//            }
//            else
//                Task.Run(async () => { await SignalRClientConnection.Instance.StartHubConfigurations(); });
//        }

//        private bool IsServerConnected()
//        {
//            if (SignalRClientConnection.CheckHubConnectionState)
//                return true;

//            return false;
//        }

//        public void KillWatchDog()
//        {
//            timer.Enabled = false;
//            timer.Stop();
//            timer.Dispose();
//        }
//    }
//}