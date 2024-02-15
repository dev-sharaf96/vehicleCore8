using System.Xml;
using Tameenk.Core.Extensions;

namespace Tameenk.Core.Configuration
{
    public class PayFortConfig
    {
        public PayFortConfig(XmlNode section)
        {
            var payFortNode = section.SelectSingleNode("PayFort");
            MerchantIdentifier = payFortNode.GetString("MerchantIdentifier");
            AccessCode = payFortNode.GetString("AccessCode");
            SHARequestPhrase = payFortNode.GetString("SHARequestPhrase");
            SHAResponsePharse = payFortNode.GetString("SHAResponsePharse");
            ReturnUrl = payFortNode.GetString("ReturnUrl");
            Currency = payFortNode.GetString("Currency");
        }

        public string MerchantIdentifier { get; private set; }
        public string AccessCode { get; private set; }
        public string SHARequestPhrase { get; private set; }
        public string SHAResponsePharse { get; private set; }
        public string ReturnUrl { get; private set; }
        public string Currency { get; private set; }
        public bool ForAngualr { get; private set; }
    }
}
