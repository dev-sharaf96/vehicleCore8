using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tameenk.Core.Extensions;

namespace Tameenk.Core.Configuration
{
    public class TabbyConfig
    {
        public TabbyConfig(XmlNode section)
        {
            var TabbyConfigSection = section.SelectSingleNode("Tabby");

            CheckoutUrl = TabbyConfigSection.GetString("CheckoutUrl");// "https://api.tabby.ai/api/v2/checkout";
            PaymentUrl = TabbyConfigSection.GetString("PaymentUrl");// "https://api.tabby.ai/api/v2/checkout";
            Pk = TabbyConfigSection.GetString("Pk"); 
            MerchantCode = TabbyConfigSection.GetString("MerchantCode");// "bcare";// TabbyConfigSection.GetString("MerchantCode");
            SK = TabbyConfigSection.GetString("Sk"); //"pk_test_b8438428-b85b-4ed8-9138-0ba2e1e90bd9";
            SuccessUrl = TabbyConfigSection.GetString("SuccessUrl");
            FailUrl = TabbyConfigSection.GetString("FailUrl");
            CancelUrl = TabbyConfigSection.GetString("CancelUrl");
            CaptureUrl = TabbyConfigSection.GetString("CaptureUrl");
        }
        public string CheckoutUrl { get; set; }
        public string PaymentUrl { get; set; }
        public string Pk { get; set; }
        public string SK { get; set; }
        public string MerchantCode { get; set; }
        public string SuccessUrl { get; set; }
        public string FailUrl { get; set; }
        public string CancelUrl { get; set; }
        public string CaptureUrl { get; set; }
    }
}
