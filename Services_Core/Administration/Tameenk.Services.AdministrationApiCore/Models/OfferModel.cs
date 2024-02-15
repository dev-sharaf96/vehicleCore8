using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// Represent Offer Model
    /// </summary>
    [JsonObject("offer")]
    public class OfferModel
    {
        

        /// <summary>
        /// offer id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; } 
        
        [JsonProperty("textAr")]
        public string TextAr { get; set; }
        [JsonProperty("textEn")]
        public string TextEn { get; set; }

        [JsonProperty("image")]
        public byte[] Image { get; set; } 
        [JsonProperty("isDeleted")]
        public bool IsDeleted { set; get; } 
   


    }
}