using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TameenkDAL.Models
{
    [JsonObject("DriverExtraLicense")]
    public class InquiryDriverExtraLicenseModel
    {
        [JsonProperty("countryId")]
        public short CountryId { get; set; }

        [JsonProperty("licenseYearsId")]
        public int LicenseYearsId { get; set; }
    }
}
