using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tameenk.Core.Extensions;

namespace Tameenk.Core.Configuration
{
    public class PolicyConfig
    {
        public PolicyConfig(XmlNode section)
        {
            var policyNode = section.SelectSingleNode("Policy");
            PolicyAndInvoiceGeneratorApiUrl = policyNode.GetString("GenerationUrl");
            Url = policyNode.GetString("Url");
            TestMode = policyNode.GetBool("TestMode");
        }

        public string Url { get; private set; }
        public string PolicyAndInvoiceGeneratorApiUrl { get; private set; }
        public bool TestMode { get; private set; }
    }
}
