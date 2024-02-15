using System.Xml;
using Tameenk.Core.Extensions;

namespace Tameenk.Core.Configuration
{
    public class HyperpayConfig
    {
        public HyperpayConfig(XmlNode section)
        {
            var HyperpayConfigSection = section.SelectSingleNode("Hyperpay");
            Url = HyperpayConfigSection.GetString("Url");
            UserId = HyperpayConfigSection.GetString("UserId");
            Password = HyperpayConfigSection.GetString("Password");
            EntityId = HyperpayConfigSection.GetString("EntityId");
            PaymentType = HyperpayConfigSection.GetString("PaymentType");
            Currency = HyperpayConfigSection.GetString("Currency");
            AccessToken = HyperpayConfigSection.GetString("AccessToken");
            TestMode = HyperpayConfigSection.GetString("TestMode");
            NotificationURL = HyperpayConfigSection.GetString("NotificationURL");
            QueryURL = HyperpayConfigSection.GetString("QueryURL");
            SplitLoginUrl = HyperpayConfigSection.GetString("SplitLoginUrl");
            SplitLoginEmail = HyperpayConfigSection.GetString("SplitLoginEmail");
            SplitLoginPassword = HyperpayConfigSection.GetString("SplitLoginPassword");
            SplitOrderUrl = HyperpayConfigSection.GetString("SplitOrderUrl");
            UpdateOrderUrl = HyperpayConfigSection.GetString("UpdateOrderUrl");
            CreateOrderUrl = HyperpayConfigSection.GetString("CreateOrderUrl");
            ApplePayEntityId = HyperpayConfigSection.GetString("ApplePayEntityId");

            ApplePaySessionUrl = HyperpayConfigSection.GetString("ApplePaySessionUrl");
            ApplePayMerchantIdentifier = HyperpayConfigSection.GetString("ApplePayMerchantIdentifier");
            ApplePayDomainName = HyperpayConfigSection.GetString("ApplePayDomainName");
            ApplePaySslCertPath = HyperpayConfigSection.GetString("ApplePaySslCertPath");
            ApplePaySslCertPWD = HyperpayConfigSection.GetString("ApplePaySslCertPWD");
            ApplePayPaymentUrl = HyperpayConfigSection.GetString("ApplePayPaymentUrl");
        }
        public string Url { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string EntityId { get; set; }
        public string PaymentType { get; set; }
        public string Currency { get; set; }
        public string AccessToken { get; set; }
        public string TestMode { get; set; }
        public string NotificationURL { get; set; }
        public string QueryURL { get; set; }
        public string SplitLoginUrl { get; set; }
        public string SplitLoginEmail { get; set; }
        public string SplitLoginPassword { get; set; }
        public string SplitOrderUrl { get; set; }
        public string ApplePayEntityId { get; set; }

        public string UpdateOrderUrl { get; set; }
        public string CreateOrderUrl { get; set; }

        public string ApplePaySessionUrl { get; set; }
        public string ApplePayMerchantIdentifier { get; set; }
        public string ApplePayDomainName { get; set; }
        public string ApplePaySslCertPath { get; set; }
        public string ApplePaySslCertPWD { get; set; }
        public string ApplePayPaymentUrl { get; set; }

    }
}
