using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TameenkDAL.Models
{
    [JsonObject("insured")]
    public class InquiryInsuredModel
    {
        /// <summary>
        /// National ID / Iqama ID
        /// </summary>
        [JsonProperty("nationalId")]
        public string NationalId { get; set; }

        /// <summary>
        /// Customer birthdate month.
        /// </summary>
        [JsonProperty("birthDateMonth")]
        public byte BirthDateMonth { get; set; }

        /// <summary>
        /// Customer birth date year.
        /// </summary>
        [JsonProperty("birthDateYear")]
        public short BirthDateYear { get; set; }

        /// <summary>
        /// Insured Education identifier.
        /// </summary>
        [JsonProperty("edcuationId")]
        public int EducationId { get; set; }

        /// <summary>
        /// Number of children under age 16 years.
        /// </summary>
        [JsonProperty("childrenBelow16Years")]
        public int ChildrenBelow16Years { get; set; }
        /// <summary>
        /// Insured Work City Code
        /// </summary>
        [JsonProperty("insuredWorkCityCode")]
        public long? InsuredWorkCityCode { get; set; }

        /// <summary>
        /// Insured Work City
        /// </summary>
        [JsonProperty("insuredWorkCity")]
        public string InsuredWorkCity { get; set; }

        [JsonProperty("driverExtraLicenses")]
        public InquiryDriverExtraLicenseModel DriverExtraLicense { get; set; }
    }
}
