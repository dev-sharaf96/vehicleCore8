using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tameenk.Core.Extensions;

namespace Tameenk.Core.Configuration
{
    public class SaudiPostConfig
    {
        public SaudiPostConfig(XmlNode section)
        {
            var saudiPostSection = section.SelectSingleNode("SaudiPost");
            Url = saudiPostSection.GetString("Url");
            ApiKey = saudiPostSection.GetString("ApiKey");
            TestMode = saudiPostSection.GetBool("TestMode");
        }

        public string Url { get; private set; }
        public string ApiKey { get; private set; }
        public bool TestMode { get; private set; }
    }
}
