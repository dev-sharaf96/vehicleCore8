using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tameenk.Core.Extensions;

namespace Tameenk.Core.Configuration
{
    public class GoogleCaptchaConfig
    {
        public GoogleCaptchaConfig(XmlNode section)
        {
            var googleCaptchaSection = section.SelectSingleNode("GoogleCaptcha");
            Url = googleCaptchaSection.GetString("Url");
            Secret = googleCaptchaSection.GetString("Secret");
        }

        public string Secret { get; private set; }
        public string Url { get; private set; }
    }
}
