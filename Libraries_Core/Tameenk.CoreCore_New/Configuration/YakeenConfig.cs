using System.Xml;
using Tameenk.Core.Extensions;

namespace Tameenk.Core.Configuration
{
    public class YakeenConfig
    {
        public YakeenConfig(XmlNode section)
        {
            var yakeenNode = section.SelectSingleNode("Yakeen");
            TestMode = yakeenNode.GetBool("TestMode");
        }
        public bool TestMode { get; private set; }
        
    }
}
