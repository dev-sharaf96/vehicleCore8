using System.Xml;
using Tameenk.Core.Extensions;

namespace Tameenk.Core.Configuration
{
    public class NajmConfig
    {
        public NajmConfig(XmlNode section)
        {
            var najmNode = section.SelectSingleNode("Najm");
            TestMode = najmNode.GetBool("TestMode");
            Username = najmNode.GetString("Username");
            Password = najmNode.GetString("Password");
        }
        public bool TestMode { get; private set; }

        public string Username { get; private set; }
        public string Password { get; private set; }
        
    }

    public class NajmNewServiceConfig
    {
        public NajmNewServiceConfig(XmlNode section)
        {
            var najmNode = section.SelectSingleNode("NajmNewService");
            TestMode = najmNode.GetBool("TestMode");
            Username = najmNode.GetString("Username");
            Password = najmNode.GetString("Password");
        }
        public bool TestMode { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

    }
}
