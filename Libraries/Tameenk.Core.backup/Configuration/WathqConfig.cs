using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tameenk.Core.Extensions;

namespace Tameenk.Core.Configuration
{
    public  class WathqConfig
    {
        public WathqConfig(XmlNode section)
        {
            var WathqConfigSection = section.SelectSingleNode("Wathq");
            Url = WathqConfigSection.GetString("Url");
            ApiKey = WathqConfigSection.GetString("ApiKey");
        }
        public string Url { get; set; }
        public string UserId { get; set; }
        public string ApiKey { get; set; }
    }
}
