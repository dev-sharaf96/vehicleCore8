using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tameenk.Core.Extensions;

namespace Tameenk.Core.Configuration
{
    public class EdaatConfig
    {
        public EdaatConfig(XmlNode section)
        {
            var edaatConfigSection = section.SelectSingleNode("Edaat");

            Url = edaatConfigSection.GetString("Url");
            LoginUrl = edaatConfigSection.GetString("LoginUrl");
            UserName = edaatConfigSection.GetString("UserName");
            Password = edaatConfigSection.GetString("Password");
            SubBillerRegistrationNo = edaatConfigSection.GetString("SubBillerRegistrationNo");

            TPlProductId= edaatConfigSection.GetInteger("TPlProductId");
            ComprehensiveProductId = edaatConfigSection.GetInteger("ComprehensiveProductId");
            SanadPlusProductId = edaatConfigSection.GetInteger("SanadPlusProductId"); 

            InternalCode = edaatConfigSection.GetString("InternalCode");
            SubBillerUrl= edaatConfigSection.GetString("SubBillerUrl");
            ODProductId = edaatConfigSection.GetInteger("ODProductId");
            MotorPlusProductId = edaatConfigSection.GetInteger("MotorPlusProductId");
        }
        public string Url { get; set; }
        public string LoginUrl { set; get; }
        public string SubBillerUrl { set; get; }

        public string UserName { set; get; }
        public string Password { set; get; }
        public string SubBillerRegistrationNo { set; get; }      
        public int TPlProductId { set; get; }
        public int ComprehensiveProductId { set; get; }
        public int SanadPlusProductId { set; get; }
        public double Price { set; get; }
        public string InternalCode { set; get; }
        public int ODProductId { set; get; }
        public int MotorPlusProductId { set; get; }

    }
}
