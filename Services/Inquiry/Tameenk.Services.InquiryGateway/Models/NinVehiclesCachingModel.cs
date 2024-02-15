using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Tameenk.Services.InquiryGateway
{
    public class NinVehiclesCachingModel
    {
        /// <summary>
        /// GUID generated first time when user try to login
        /// </summary>
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        /// <summary>
        /// represent the number of times that user try to hit the api
        /// </summary>
        [JsonProperty("triesCount")]
        public int TriesCount { get; set; }

        [JsonProperty("vehicles")]
        public List<VehicleInfo> Vehicles { get; set; }

        [JsonProperty("expirationDate")]
        public DateTime ExpirationDate { get; set; }
    }
}