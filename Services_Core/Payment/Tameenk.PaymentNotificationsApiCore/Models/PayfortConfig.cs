using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.PaymentNotificationApi
{
    public class PayfortConfig
    {
        // production  
        public static string MerchantIdentifier = "rIcKeGlw";
        public static string AccessCode = "UhayyFucUefrF2fdx7eJ";
        public static string SHARequestPhrase = "QSXZSE";
        public static string SHAResponsePharse = "WDCXDR";

        //public static string MerchantIdentifier = "XuPWWGSn";
        //public static string AccessCode = "AYlFMpdYRMFXkEDKmvDi";
        //public static string SHARequestPhrase = "SHAREQUESTPHRASE";
        //public static string SHAResponsePharse = "SHARESPONSEPHRASE";
        public static string ReturnURL = System.Configuration.ConfigurationManager.AppSettings["PayfortReturnUrl"];
        public static string Currency = "SAR";
        public static string Command = "PURCHASE";

    }
}
