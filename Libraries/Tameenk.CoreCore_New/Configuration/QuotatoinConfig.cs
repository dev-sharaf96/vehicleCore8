using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tameenk.Core.Extensions;

namespace Tameenk.Core.Configuration
{
    public class QuotatoinConfig
    {
        public QuotatoinConfig(XmlNode section)
        {
            var quotationSection = section.SelectSingleNode("Quotation");
            Url = quotationSection.GetString("Url");
            TestMode = quotationSection.GetBool("TestMode");
            showZeroPremium = quotationSection.GetBool("showZeroPremium");
            UseRandomPlateNumber = quotationSection.GetBool("UseRandomPlateNumber");
        }

        public bool showZeroPremium { get; private set; }
        public string Url { get; private set; }
        public bool TestMode { get; private set; }
        public bool UseRandomPlateNumber { get; private set; }
        
    }
}
