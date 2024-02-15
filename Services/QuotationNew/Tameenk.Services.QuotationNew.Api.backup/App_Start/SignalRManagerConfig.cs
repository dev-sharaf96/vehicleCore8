//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Timers;
//using System.Web;
//using System.Web.Configuration;

//namespace Tameenk.Services.QuotationNew.Api
//{
//    public class SignalRManagerConfig
//    {
//        private static Timer timer;
//        private static System.Threading.Timer serverStateCheckTimer;


//        public static void InitializeHubConnection()
//        {
//            _ = SignalRClientConnection.Instance;
//            //CheckConnection();
//        }

//        //private static void CheckConnection()
//        //{
//        //    timer = new Timer();
//        //    //timer.Elapsed += OnTimerElapsed;
//        //    timer.Elapsed += CheckServerState;
//        //    timer.Interval = int.Parse(WebConfigurationManager.AppSettings["SignalRServerCheckStateAfterSeconds"]);
//        //    timer.Enabled = true;
//        //}

//        //private static async void OnTimerElapsed(object sender, ElapsedEventArgs e)
//        //{
//        //    if (!SignalRClientConnection.CheckHubConnectionState)
//        //        SignalRClientConnection.Instance.StartHubConfigurations();
//        //}

//        //private static async void CheckServerState(object sender, ElapsedEventArgs e) //(object state)
//        //{
//        //    bool isServerAlive = await IsServerAlive();
//        //    if (isServerAlive)
//        //    {
//        //        // If the server is alive, stop the timer and attempt reconnection
//        //        serverStateCheckTimer?.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
//        //        serverStateCheckTimer?.Dispose();
//        //        serverStateCheckTimer = null;
//        //    }
//        //    else
//        //        SignalRClientConnection.Instance.StartHubConfigurations();
//        //}

//        //private static async Task<bool> IsServerAlive()
//        //{
//        //    if (!SignalRClientConnection.CheckHubConnectionState)
//        //        return true;

//        //    return false;
//        //}
//    }
//}