using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using System.Timers;
namespace Reports.SamaReport
{
    public partial class SamaService : ServiceBase
    {
        private System.Timers.Timer serviceTimer;
        ErrorLogging logging = new ErrorLogging();
        public SamaService()
        {
            try { 

            logging.LogDebug("Initialize Service");
            InitializeComponent();
            serviceTimer = new System.Timers.Timer();
            string runServiceEvery = Utilities.GetAppSetting("RunServiceEvery");


            if (!string.IsNullOrEmpty(runServiceEvery))
                serviceTimer.Interval = Convert.ToDouble(runServiceEvery);
            else
                serviceTimer.Interval = 3600000;

            serviceTimer.Elapsed += new ElapsedEventHandler(ServiceTimer_Elapsed);
            }
            catch (Exception exp)
            {
                logging.LogError(exp.Message, exp, false);
            }
        }
        public void OnDebug()
        {
            OnStart(null);
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

        private void ServiceTimer_Elapsed(object sender, EventArgs e)
        {
            try
            {
                SamaContext samaContext = new SamaContext();

                DateTime dt = DateTime.Now;

               // if (dt.Hour == 0)
                    samaContext.ExportSamaExcelThenSendMail();
                serviceTimer.AutoReset = true;
            }
            catch (Exception exp)
            {
                logging.LogError(exp.Message, exp, false);
            }
        }

    }
}
