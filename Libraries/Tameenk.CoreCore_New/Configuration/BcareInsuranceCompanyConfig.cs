using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tameenk.Core.Extensions;

namespace Tameenk.Core.Configuration
{
    public class BcareInsuranceCompanyConfig
    {
        public int NIN { get; set; }
        public string VehicleChassisNumber { get; set; }

        public BcareInsuranceCompanyConfig(XmlNode section)
        {
            var bcareInsuranceSection = section.SelectSingleNode("BcareInsuranceCompany");
            NIN = bcareInsuranceSection.GetInteger("NIN");
            VehicleChassisNumber = bcareInsuranceSection.GetString("VehicleChassisNumber");
        }
    }
}
