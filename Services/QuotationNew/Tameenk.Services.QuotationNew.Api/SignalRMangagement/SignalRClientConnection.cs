//using Microsoft.AspNetCore.SignalR.Client;
//using NLog;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Threading.Tasks;
//using System.Timers;
//using System.Web;
//using System.Web.Configuration;

//namespace Tameenk.Services.QuotationNew.Api
//{
//    public class SignalRClientConnection
//    {
//        static SignalRClientConnection _instance;
//        private HubConnection HubConnection;
//        private readonly Logger logger;
//        private string endpoint;
//        private bool watchDogTriggered = false;
//        private bool hubEventsRegistered = false;

//        private SignalRClientConnection()
//        {
//            this.endpoint = WebConfigurationManager.AppSettings["SignalRServerURl"];
//            this.logger = LogManager.GetCurrentClassLogger();
//            this.HubConnection = new HubConnectionBuilder()
//                                    .WithUrl(endpoint)
//                                    .WithAutomaticReconnect(new SignalRRetryPolicy())
//                                    .Build();
//            _ = StartHubConfigurations();
//        }

//        public static SignalRClientConnection Instance
//        {
//            get
//            {
//                if (_instance == null)
//                    _instance = new SignalRClientConnection();
//                return _instance;
//            }
//        }

//        public static bool CheckHubConnectionState
//        {
//            get
//            {
//                if (_instance.HubConnection.State == HubConnectionState.Connected)
//                    return true;
//                return false;
//            }
//        }

//        public async Task StartHubConfigurations()
//        {
//            if (hubEventsRegistered == false)
//            {
//                HubConnection.On<string, string>("ReceiveMessage", (user, message) =>
//                {

//                    logger.Info("HubConnection.On is started");
//                });

//                HubConnection.On<string, ClientQuoteRequestMessage>("ReceiveQuoteRequest", async (user, clientQuoteRequestMessage) =>
//                {
//                    //Quotation request business logic
//                    List<Task> tasks = new List<Task>();
//                    foreach (var quoteRequest in clientQuoteRequestMessage.ClientQuoteRequests)
//                    {
//                        tasks.Add(new Task(async () =>
//                        { //Service business
//                          //
//                        }));
//                    };

//                    await Task.WhenAll(tasks);

//                    ClientQuoteResponseMessage clientQuoteResponseMessage = new ClientQuoteResponseMessage();
//                    await HubConnection.InvokeAsync("ReceiveQuoteResponse", "", clientQuoteResponseMessage);

//                    logger.Info("HubConnection.On is started");
//                });

//                HubConnection.Reconnected += async (error) =>
//                {
//                    logger.Info("HubConnection.Reconnected");
//                    logger.Error($"HubConnection.Closed exception is {error.ToString()}");

//                };

//                HubConnection.Closed += async (error) =>
//                {
//                    logger.Info("HubConnection.Closed is started");
//                    logger.Error($"HubConnection.Closed exception is {error.ToString()}");
//                    await HubConnection.StartAsync();

//                };


//                HubConnection.Reconnecting += (exception) =>
//                {
//                    logger.Warn("HubConnection.Reconnecting is started", exception);
//                    return Task.CompletedTask;
//                };
//                hubEventsRegistered = true;
//            }


//            try
//            {
//                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;

//                await HubConnection.StartAsync();
//                logger.Warn("HubConnection.StartAsync is started");

//                await HubConnection.InvokeAsync("SendMessage", "Nader", "He");
//            }
//            catch (Exception ex)
//            {
//                // here will retry to check connection in case of the specific connection error only
//                if (watchDogTriggered == false)
//                {
//                    //SignarlRConnectionWatchDog signarlRConnectionWatchDog = new SignarlRConnectionWatchDog();
//                    SignarlRConnectionWatchDog.Instance.InitWatchDogTimer();
//                    watchDogTriggered = true;
//                }

//                logger.Error($"Exception is started and exception is {ex.ToString()}");

//            }
//        }
//    }
//}