using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tameenk.Core.Extensions;

namespace Tameenk.Core.Configuration
{
    public class HttpClientConfig
    {
        public HttpClientConfig(XmlNode section)
        {
            var httpClientConfigNode = section.SelectSingleNode("HttpClient");

            UseResillientHttpClient = httpClientConfigNode.GetBool("UseResillientHttpClient");

            if (httpClientConfigNode == null || httpClientConfigNode.Attributes["ExceptionsAllowedBeforeBreaking"] == null)
                ExceptionsAllowedBeforeBreaking = 5;
            else
                ExceptionsAllowedBeforeBreaking = httpClientConfigNode.GetInteger("ExceptionsAllowedBeforeBreaking");


            if (httpClientConfigNode == null || httpClientConfigNode.Attributes["RetryCount"] == null)
                ExceptionsAllowedBeforeBreaking = 6;
            else
                ExceptionsAllowedBeforeBreaking = httpClientConfigNode.GetInteger("RetryCount");
        }

        public bool UseResillientHttpClient { get; private set; }
        public int ExceptionsAllowedBeforeBreaking { get; private set; }
        public int RetryCount { get; private set; }
    }
}
