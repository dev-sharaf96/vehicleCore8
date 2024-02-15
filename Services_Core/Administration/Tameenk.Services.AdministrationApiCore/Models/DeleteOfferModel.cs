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
    [JsonObject("deleteOffer")]
    public class DeleteOfferModel
    {
        

        /// <summary>
        /// offer id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }  
        [JsonProperty("isDeleted")]
        public bool IsDeleted { set; get; } 


    }
}