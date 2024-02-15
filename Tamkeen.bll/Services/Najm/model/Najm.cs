using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Tameenk.bll.Najm.model
{
    public class NajmRequestMessage
    {
        [Required]
        public long policyHolderId { get; set; }
        [Required]
        public long vehicleId { get; set; }
        public bool IsVehicleRegistered { get; set; }
        public String userName { get; set; }
        public String password { get; set; }
    }

    [XmlRoot(ElementName = "ResponseData")]
    public class NajmResponseMessage
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
    }
}