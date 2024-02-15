using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tameenk.Services.InquiryGateway.Models
{
    [JsonObject("DriverExtraLicense")]
    public class DriverExtraLicenseModel
    {
        //[Required]
        [JsonProperty("countryId")]
        public short CountryId { get; set; }

        [JsonProperty("licenseYearsId")]
        //[Required]
        public int LicenseYearsId { get; set; }
    }
}