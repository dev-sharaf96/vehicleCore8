using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tameenk.Core.Extensions;

namespace Tameenk.Core.Configuration
{
    public class IdentityConfig
    {
        public IdentityConfig(XmlNode section)
        {
            var identityConfigSection = section.SelectSingleNode("Identity");
            Url = identityConfigSection.GetString("Url");
            ClientId = identityConfigSection.GetString("ClientId");
            ClientSecret = identityConfigSection.GetString("ClientSecret");
        }

        public string Url { get; private set; }
        public string ClientId { get; private set; }
        public string ClientSecret { get; private set; }
    }
}
