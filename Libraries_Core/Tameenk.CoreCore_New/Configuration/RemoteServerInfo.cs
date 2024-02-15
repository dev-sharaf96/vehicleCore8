using System.Xml;
using Tameenk.Core.Extensions;

namespace Tameenk.Core.Configuration
{
    public class RemoteServerInfo
    {
        public RemoteServerInfo(XmlNode section)
        {
            var RemoteServerInfoConfigSection = section.SelectSingleNode("RemoteServerInfo");
            UseNetworkDownload = RemoteServerInfoConfigSection.GetBool("UseNetworkDownload");
            DomainName = RemoteServerInfoConfigSection.GetString("DomainName");
            ServerIP = RemoteServerInfoConfigSection.GetString("ServerIP");
            ServerUserName = RemoteServerInfoConfigSection.GetString("ServerUserName");
            ServerPassword = RemoteServerInfoConfigSection.GetString("ServerPassword");
        }

        #region AdminServerInfo
        public bool UseNetworkDownload { get; set; }
        public string DomainName { get; set; }
        public string ServerIP { get; set; }
        public string ServerUserName { get; set; }
        public string ServerPassword { get; set; }
        #endregion
    }
}