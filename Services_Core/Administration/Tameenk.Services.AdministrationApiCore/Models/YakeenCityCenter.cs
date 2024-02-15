using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    [JsonObject("yakeenCityCenter")]
    public class YakeenCityCenter
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("cityId")]
        public int CityId { get; set; }

        [JsonProperty("cityName")]
        public string CityName { get; set; }

        [JsonProperty("englishName")]
        public string EnglishName { get; set; }

        [JsonProperty("zipCode")]
        public int ZipCode { get; set; }

        [JsonProperty("regionId")]
        public int RegionId { get; set; }

        [JsonProperty("regionArabicName")]
        public string RegionArabicName { get; set; }

        [JsonProperty("regionEnglishName")]
        public string RegionEnglishName { get; set; }

        [JsonProperty("elmCode")]
        public int ElmCode { get; set; }
    }
}