using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Report.QuotationPolicyEachCompany
{
    static class Program
    {
       
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new QuotationPolicyService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
