using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Tameenk.Integration.Dto.Najm
{
    [XmlRoot(ElementName = "ResponseData")]
    public class NajmResponse
    {
        public int StatusCode { get; set; }
        public string NCDReference { get; set; }

        [XmlIgnore]
        public int? NCDFreeYears
        {
            get
            {
                int parsedValue = -1;
                if (!string.IsNullOrEmpty(NCDFreeYearsText) &&
                    int.TryParse(NCDFreeYearsText, out parsedValue))
                {
                    return parsedValue;
                }
                return null;
            }

        }

        [XmlElement("NCDFreeYears")]
        public string NCDFreeYearsText { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMsg { get; set; }

        [XmlIgnore]
        public int? SaudiLicenseHeldYears
        {
            get
            {
                int parsedValue = -1;
                if (!string.IsNullOrEmpty(SaudiLicenseHeldYearsText) &&
                    int.TryParse(SaudiLicenseHeldYearsText, out parsedValue))
                {
                    return parsedValue;
                }
                return null;
            }

        }

        [XmlElement("SaudiLicenseHeldYears")]
        public string SaudiLicenseHeldYearsText { get; set; }

        [XmlIgnore]
        public int? NumOfFaultAccidentInLast5Years
        {
            get
            {
                int parsedValue = -1;
                if (!string.IsNullOrEmpty(NumOfFaultAccidentInLast5YearsText) &&
                    int.TryParse(NumOfFaultAccidentInLast5YearsText, out parsedValue))
                {
                    return parsedValue;
                }
                return null;
            }

        }

        [XmlElement("NumOfFaultAccidentInLast5Years")]
        public string NumOfFaultAccidentInLast5YearsText { get; set; }


        [XmlIgnore]
        public int? EligibleForNoClaimsDiscountYears
        {
            get
            {
                int parsedValue = -1;
                if (!string.IsNullOrEmpty(EligibleForNoClaimsDiscountYearsText) &&
                    int.TryParse(EligibleForNoClaimsDiscountYearsText, out parsedValue))
                {
                    return parsedValue;
                }
                return null;
            }

        }

        [XmlElement("EligibleForNoClaimsDiscountYears")]
        public string EligibleForNoClaimsDiscountYearsText { get; set; }


    }
}
