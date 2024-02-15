using System.Xml;
using Tameenk.Core.Extensions;

namespace Tameenk.Core.Configuration
{
    public class ClientUrlConfig
    {
        public ClientUrlConfig(XmlNode section)
        {
            var ClientUrlSection = section.SelectSingleNode("ClientUrl");
            Url = ClientUrlSection.GetString("Url");
        }
        public string Url { get; private set; }

    }
}
