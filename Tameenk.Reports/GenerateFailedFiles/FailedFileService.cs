using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Tameenk.Common.Utilities;

namespace GenerateFailedFiles
{
    partial class FailedFileService : ServiceBase
    {
        private Timer serviceTimer;
        ErrorLogging logging = new ErrorLogging();
        public FailedFileService()
        {
            try
            {
                logging.LogDebug("Initialize Service");
                InitializeComponent();
                logging.LogDebug("after Initialize Service");
                serviceTimer = new Timer();
                logging.LogDebug("after timer");
                string runServiceEvery = Utilities.GetAppSetting("RunServiceEvery");
                logging.LogDebug("RunServiceEvery " + runServiceEvery);
                if (!string.IsNullOrEmpty(runServiceEvery))
                {
                    serviceTimer.Interval = Convert.ToDouble(runServiceEvery);
                    logging.LogDebug("serviceTimer.Interval " + serviceTimer.Interval);
                }
                else
                {
                    serviceTimer.Interval = 3600000;
                }
                logging.LogDebug("before serviceTimer.Elapsed");
                serviceTimer.Elapsed += new ElapsedEventHandler(serviceTimer_Elapsed);
                logging.LogDebug("after serviceTimer.Elapsed");
            }
            catch(Exception exp)
            {
                logging.LogError(exp.Message, exp, false);
            }
        }

        protected override void OnStart(string[] args)
        {
            logging.LogDebug("Service started");
            serviceTimer.Enabled = true;
            serviceTimer.AutoReset = true;
            serviceTimer.Start();
        }

        protected override void OnStop()
        {
            logging.LogDebug("Service stopped");
            serviceTimer.Stop();
        }


        private void serviceTimer_Elapsed(object sender, EventArgs e)
        {
            try
            {
                logging.LogDebug("before GenerateFailedFiles");
                FailedFileContext.GenerateFailedFiles();
                logging.LogDebug("after GenerateFailedFiles");
               // DateTime dt = DateTime.Now;
                //if (dt.Hour == 0)
                   // BillPaymentDataAccess.SendMailWithReportOfEAIRequestsSinceAday(EAIRequestsServiceResource.MailBody, EAIRequestsServiceResource.MailSubject, EAIRequestsServiceResource.NoRecordsMailBody, EAIRequestsServiceResource.MailSubject);
                serviceTimer.AutoReset = true;
            }
            catch (Exception exp)
            {
                logging.LogError(exp.Message, exp, false);
            }
        }
    }
}
