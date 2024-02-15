using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Policy.RetrialMechanism
{
    static class Program
    {

        static void RunOnDebug()
        {
            PolicyRetrialMechanismService policyRetrialMechanismService = new PolicyRetrialMechanismService();
            policyRetrialMechanismService.OnDebug();
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
                new PolicyRetrialMechanismService()
            };
            ServiceBase.Run(ServicesToRun);
#endif

        }
    }
}
