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

namespace Tameenk.Report.QuotationPolicyEachCompany
{
    public partial class QuotationPolicyService : ServiceBase
    {
        private Timer serviceTimer;
        ErrorLogging logging = new ErrorLogging();
        public QuotationPolicyService()
        {
            try
            {
                logging.LogDebug("Initialize Service");
                InitializeComponent();

            serviceTimer = new Timer();
            string RunServiceEvery = Utilities.GetAppSetting("RunServiceEvery");
            if (!string.IsNullOrEmpty(RunServiceEvery))
                serviceTimer.Interval = Convert.ToDouble(RunServiceEvery);
            else
                serviceTimer.Interval = 3600000;


            ErrorLogger.LogDebug("This service will run every" + serviceTimer.Interval);
            serviceTimer.Elapsed += new ElapsedEventHandler(serviceTimer_Elapsed);
            }
            catch (Exception exp)
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
               // if (DateTime.Now.Hour == runAt)
                {
                    ErrorLogger.LogDebug("PolicyQuotationForEachCompany");
                    ServiceContext serviceContext = new ServiceContext();
                    serviceContext.ExportExcelThenSendMail();
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
