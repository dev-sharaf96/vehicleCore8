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

namespace Policy.RetrialMechanism
{
    partial class PolicyRetrialMechanismService : ServiceBase
    {
        private Timer serviceTimer;
     
        public PolicyRetrialMechanismService()
        {
            InitializeComponent();
            serviceTimer = new Timer();
            string runServiceEvery = Utilities.GetAppSetting("RunServiceEvery");
            
#if DEBUG
            serviceTimer_Elapsed(null, null);
#else
            if (!string.IsNullOrEmpty(runServiceEvery))
                serviceTimer.Interval = Convert.ToDouble(runServiceEvery);
            else
                serviceTimer.Interval = 3600000;

            serviceTimer.Elapsed += new ElapsedEventHandler(serviceTimer_Elapsed);
#endif
        }


        public void OnDebug()
        {
            OnStart(null);
        }


        protected override void OnStart(string[] args)
        {
            ErrorLogger.LogDebug("Service started");
            serviceTimer.Enabled = true;
            serviceTimer.AutoReset = true;
            serviceTimer.Start();
        }

        protected override void OnStop()
        {
            ErrorLogger.LogDebug("Service stopped");
            serviceTimer.Stop();
        }

        private void serviceTimer_Elapsed(object sender, EventArgs e)
        {
            try
            {
                PolicyRetrialMechanismContext.GetAndSubmitFailedTransactions();

                DateTime dt = DateTime.Now;

                if (dt.Hour == 0)
                   // PolicyRetrialMechanismContext.SendMailWithReportOfFailedTransactionsSinceAday(.MailBody, BillPaymentRetrialMechanismResource.MailSubject);
               
                serviceTimer.AutoReset = true;
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
            }
        }
    }
}
