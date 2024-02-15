using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tameenk.Core.Extensions;

namespace Tameenk.Core.Configuration
{
    public class InquiryConfig
    {
        public InquiryConfig(XmlNode section)
        {
            var inquiryConfigSection = section.SelectSingleNode("Inquiry");
            Url = inquiryConfigSection.GetString("Url");
            TestMode = inquiryConfigSection.GetBool("TestMode");
        }

        public string Url { get; private set; }
        public bool TestMode { get; private set; }
        
    }
}
