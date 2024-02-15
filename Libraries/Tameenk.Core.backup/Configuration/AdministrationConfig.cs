using System.Xml;
using Tameenk.Core.Extensions;

namespace Tameenk.Core.Configuration
{
    public  class AdministrationConfig
    {
        public AdministrationConfig(XmlNode section)
        {
            var AdministrationConfigSection = section.SelectSingleNode("Administration");
            Url = AdministrationConfigSection.GetString("Url");
        }

        public string Url { get; private set; }

    }
}
