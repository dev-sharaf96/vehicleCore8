using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Reports.Policy
{
    static class Program
    {
        static void RunOnDebug()
        {
            PolicyService policyService = new PolicyService();
            policyService.OnDebug();
        }


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG
            Program.RunOnDebug();
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new PolicyService()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
