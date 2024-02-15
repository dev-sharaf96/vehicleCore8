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

namespace SendFailedPolices
{
    public partial class FailedPolicies : ServiceBase
    {
        private Timer serviceTimer;
        ErrorLogging logging = new ErrorLogging();
        public FailedPolicies()
        {
            try
            {
                logging.LogDebug("Initialize Service");
                InitializeComponent();
                serviceTimer = new Timer();
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
            ErrorLogger.LogDebug("Service stopped");
            serviceTimer.Stop();
        }

        private void ServiceTimer_Elapsed(object sender, EventArgs e)
        {

            PoliciesContext policiesContext = new PoliciesContext();

            try
            {
                

               int runAt = 4;
                int runAtTemp;
                if (int.TryParse(Utilities.GetAppSetting("RunAt"), out runAtTemp))
                {
                    runAt = runAtTemp;
                }
               // if (DateTime.Now.Hour == runAt)
                {
                    ErrorLogger.LogDebug("Send Fail Policy in ServiceTimer_Elapsed");
                    policiesContext.ExportPolicyExcelThenSendMail();
              
                }
                serviceTimer.AutoReset = true;
            }
            catch (Exception exp)
            {
                logging.LogError(exp.Message, exp, false);
            }
        }
    }
}
