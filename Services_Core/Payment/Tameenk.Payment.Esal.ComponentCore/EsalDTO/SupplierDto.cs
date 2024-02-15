using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Payment.Esal.Component
{
    public class SupplierDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }
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
        [JsonProperty("branchEnglish")]
        public string BranchEnglish { get; set; }
        [JsonProperty("branchArabic")]
        public string BranchArabic { get; set; }
        [JsonProperty("plantEnglish")]
        public string PlantEnglish { get; set; }
        [JsonProperty("plantArabic")]
        public string PlantArabic { get; set; }
        [JsonProperty("bankSwiftCode")]
        public string BankSwiftCode { get; set; }
        [JsonProperty("iban")]
        public string Iban { get; set; }
    }
}
