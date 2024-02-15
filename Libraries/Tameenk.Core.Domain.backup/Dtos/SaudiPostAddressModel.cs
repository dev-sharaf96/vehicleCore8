using Newtonsoft.Json;

namespace Tameenk.Core.Domain.Dtos
{
    /// <summary>
    /// Saudi post address model.
    /// </summary>
    [JsonObject("address")]
    public class SaudiPostAddressModel
    {
        /// <summary>
        /// Title
        /// </summary>
        [JsonProperty("title")]
        public object Title { get; set; }
        /// <summary>
        /// Address 1
        /// </summary>
        [JsonProperty("address1")]
        public string Address1 { get; set; }
        /// <summary>
        /// Address 2
        /// </summary>
        [JsonProperty("address2")]
        public string Address2 { get; set; }
        /// <summary>
        /// Lat & long
        /// </summary>
        [JsonProperty("objLatLng")]
        public string ObjLatLng { get; set; }
        /// <summary>
        /// Building number
        /// </summary>
        [JsonProperty("buildingNumber")]
        public string BuildingNumber { get; set; }
        /// <summary>
        /// Street
        /// </summary>
        [JsonProperty("street")]
        public string Street { get; set; }
        /// <summary>
        /// District
        /// </summary>
        [JsonProperty("district")]
        public string District { get; set; }
        /// <summary>
        /// City
        /// </summary>
        [JsonProperty("city")]
        public string City { get; set; }
        /// <summary>
        /// Post code
        /// </summary>
        [JsonProperty("postCode")]
        public string PostCode { get; set; }
        /// <summary>
        /// Additional number
        /// </summary>
        [JsonProperty("additionalNumber")]
        public string AdditionalNumber { get; set; }
        /// <summary>
        /// Region Name
        /// </summary>
        [JsonProperty("regionName")]
        public string RegionName { get; set; }
        /// <summary>
        /// Polygon String
        /// </summary>
        [JsonProperty("polygonString")]
        public object PolygonString { get; set; }
        /// <summary>
        /// Is primary address.
        /// </summary>
        [JsonProperty("isPrimaryAddress")]
        public string IsPrimaryAddress { get; set; }
        /// <summary>
        /// Unit number.
        /// </summary>
        [JsonProperty("unitNumber")]
        public string UnitNumber { get; set; }
        /// <summary>
        /// Latitude
        /// </summary>
        [JsonProperty("latitude")]
        public string Latitude { get; set; }
        /// <summary>
        /// Longitude
        /// </summary>
        [JsonProperty("longitude")]
        public string Longitude { get; set; }
        /// <summary>
        /// City Id.
        /// </summary>
        [JsonProperty("cityId")]
        public string CityId { get; set; }
        /// <summary>
        /// Region id.
        /// </summary>
        [JsonProperty("regionId")]
        public string RegionId { get; set; }
        /// <summary>
        /// Restriction
        /// </summary>
        [JsonProperty("restriction")]
        public string Restriction { get; set; }
        /// <summary>
        /// Primary key address id.
        /// </summary>
        [JsonProperty("pkAddressID")]
        public string PKAddressID { get; set; }
    }
}