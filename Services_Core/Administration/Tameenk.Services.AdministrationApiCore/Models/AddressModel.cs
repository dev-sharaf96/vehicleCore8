using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    [JsonObject("address")]
    public class AddressModel
    {
        /// <summary>
        /// Id 
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Title 
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Address1 
        /// </summary>
        [JsonProperty("address1")]
        public string Address1 { get; set; }


        /// <summary>
        /// Address2 
        /// </summary>
        [JsonProperty("address2")]
        public string Address2 { get; set; }

        /// <summary>
        /// ObjLatLng
        /// </summary>
        [JsonProperty("objLatLng")]
        public string ObjLatLng { get; set; }

        /// <summary>
        /// buildingNumber
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
        /// PostCode
        /// </summary>
        [JsonProperty("postCode")]
        public string PostCode { get; set; }

        /// <summary>
        /// AdditionalNumber
        /// </summary>
        [JsonProperty("additionalNumber")]
        public string AdditionalNumber { get; set; }

        /// <summary>
        /// RegionName
        /// </summary>
        [JsonProperty("regionName")]
        public string RegionName { get; set; }

        /// <summary>
        /// PolygonString
        /// </summary>
        [JsonProperty("polygonString")]
        public string PolygonString { get; set; }

        /// <summary>
        /// IsPrimaryAddress
        /// </summary>
        [JsonProperty("isPrimaryAddress")]
        public string IsPrimaryAddress { get; set; }

        /// <summary>
        /// UnitNumber
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
        /// CityId
        /// </summary>
        [JsonProperty("cityId")]
        public string CityId { get; set; }

        /// <summary>
        /// RegionId
        /// </summary>
        [JsonProperty("regionId")]
        public string RegionId { get; set; }

        /// <summary>
        /// restriction
        /// </summary>
        [JsonProperty("restriction")]
        public string Restriction { get; set; }

        /// <summary>
        /// PKAddressID
        /// </summary>
        [JsonProperty("pKAddressID")]
        public string PKAddressID { get; set; }

        /// <summary>
        /// DriverId
        /// </summary>
        [JsonProperty("driverId")]
        public Guid? DriverId { get; set; }

        /// <summary>
        /// AddressLoction
        /// </summary>
        [JsonProperty("addressLoction")]
        public string AddressLoction { get; set; }

    }
}