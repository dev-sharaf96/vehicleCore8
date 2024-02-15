using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tameenk.Core.Extensions;

namespace Tameenk.Core.Configuration
{
    public class SadadConfig
    {
        public SadadConfig(XmlNode section)
        {
            var sadadConfigSection = section.SelectSingleNode("Sadad");
            Url = sadadConfigSection.GetString("Url");
            BillerId = sadadConfigSection.GetInteger("BillerId");
            ExactFlag = sadadConfigSection.GetInteger("ExactFlag");
            BillslAccountStatus = sadadConfigSection.GetString("BillslAccountStatus");
            KeyRelativePath = sadadConfigSection.GetString("KeyRelativePath");
            KeyPassword = sadadConfigSection.GetString("KeyPassword");
            SoapEnvelopNameSpace = sadadConfigSection.GetString("SoapEnvelopNameSpace");
        }

        public string Url { get; private set; }
        public int BillerId { get; private set; }
        public int ExactFlag { get; private set; }
        public string BillslAccountStatus { get; private set; }
        public string KeyRelativePath { get; private set; }
        public string KeyPassword { get; private set; }
        public string SoapEnvelopNameSpace { get; private set; }

    }
}
