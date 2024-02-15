using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Payment.Esal.Component
{
    public class CustomerDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("nameArabic")]
        public string NameArabic { get; set; }
        [JsonProperty("nameEnglish")]
        public string NameEnglish { get; set; }
        [JsonProperty("addressArabic1")]
        public string AddressArabic1 { get; set; }
        [JsonProperty("addressArabic2")]
        public string AddressArabic2 { get; set; }
        [JsonProperty("addressEnglish1")]
        public string AddressEnglish1 { get; set; }
        [JsonProperty("addressEnglish2")]
        public string AddressEnglish2 { get; set; }
        [JsonProperty("vatRegisterationNumber")]
        public int VatRegisterationNumber { get; set; }
        [JsonProperty("branch")]
        public string Branch { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("contactNo")]
        public string ContactNo { get; set; }
        [JsonProperty("regionEnglish")]
        public string RegionEnglish { get; set; }
        [JsonProperty("regionArabic")]
        public string RegionArabic { get; set; }
        [JsonProperty("cityEnglish")]
        public string CityEnglish { get; set; }
        [JsonProperty("cityArabic")]
        public string CityArabic { get; set; }
        [JsonProperty("localityEnglish")]
        public string LocalityEnglish { get; set; }
        [JsonProperty("localityArabic")]
        public string LocalityArabic { get; set; }
        [JsonProperty("branchArabic")]
        public string BranchArabic { get; set; }
        [JsonProperty("plantEnglish")]
        public string PlantEnglish { get; set; }
        [JsonProperty("plantArabic")]
        public string PlantArabic { get; set; }
        [JsonProperty("segmentEnglish")]
        public string SegmentEnglish { get; set; }
        [JsonProperty("segmentArabic")]
        public string SegmentArabic { get; set; }
        [JsonProperty("divisionEnglish")]
        public string DivisionEnglish { get; set; }
        [JsonProperty("divisionArabic")]
        public string DivisionArabic { get; set; }
        [JsonProperty("prodLineEnglish")]
        public string ProdLineEnglish { get; set; }
        [JsonProperty("prodLineArabic")]
        public string ProdLineArabic { get; set; }


    }
}
