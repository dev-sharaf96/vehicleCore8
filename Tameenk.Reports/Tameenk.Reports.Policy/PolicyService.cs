using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using Tameenk.Common.Utilities;

namespace Tameenk.Reports.Policy
{
    partial class PolicyService : ServiceBase
    {
        private Timer serviceTimer;

        public PolicyService()
        {
            InitializeComponent();

            serviceTimer = new Timer();
            string RunServiceEvery = Utilities.GetAppSetting("RunServiceEvery");
            if (!string.IsNullOrEmpty(RunServiceEvery))
                serviceTimer.Interval = Convert.ToDouble(RunServiceEvery);
            else
                serviceTimer.Interval = 3600000;

#if DEBUG
            serviceTimer_Elapsed(null, null);
#else
            ErrorLogger.LogDebug("This service will run every" + serviceTimer.Interval);
            serviceTimer.Elapsed += new ElapsedEventHandler(serviceTimer_Elapsed);
#endif
        }
        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
            ErrorLogger.LogDebug("Service started");
            serviceTimer.Enabled = true;
            serviceTimer.AutoReset = true;
            serviceTimer.Start();
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            ErrorLogger.LogDebug("Service stopped");
            serviceTimer.Stop();
        }

        private void serviceTimer_Elapsed(object sender, EventArgs e)
        {
            try
            {
                int runAt = 4;
                int runAtTemp;
                if (int.TryParse(Utilities.GetAppSetting("RunAt"), out runAtTemp))
                {
                    runAt = runAtTemp;
                }
                if (DateTime.Now.Hour == runAt)
                {
                    ErrorLogger.LogDebug("ExportPaymentsReports");
                    PolicyContext policyContext = new PolicyContext();
                    policyContext.ExportPolicyExcelThenSendMail();
                }
                serviceTimer.AutoReset = true;
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
            }
        }
    }
}
