using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Reports.Quotation
{
    static class Program
    {

        static void RunOnDebug()
        {
           Quotation quotation = new Quotation();
            quotation.OnDebug();
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
                new Quotation()
            };
            ServiceBase.Run(ServicesToRun); 
#endif
        }
    }
}
