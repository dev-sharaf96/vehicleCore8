using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Tameenk.Integration.Core.Providers.Configuration
{
   public static class TimeOutConfig
    {
        public static double SetTimeOut()
        {
            double quotationTimeOut = 0;
            double.TryParse(WebConfigurationManager.AppSettings["QuotationClientTimeOut"], out quotationTimeOut);
            if (quotationTimeOut > 1)
                return quotationTimeOut;
            else
                return 10;
        }
    }
}
