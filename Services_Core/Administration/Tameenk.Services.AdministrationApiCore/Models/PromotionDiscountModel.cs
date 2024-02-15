using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// promotions discount class
    /// use this clas for filtering and listing data
    /// </summary>
    [JsonObject("promotionDiscountSheetModel")]
    public class PromotionDiscountModel
    {
        /// <summary>
        /// record id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// person national id
        /// </summary>
        [JsonProperty("nin")]
        public string NationalId { get; set; }

        /// <summary>
        /// person name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// person mobile
        /// </summary>
        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// person's policy expiry date
        /// </summary>
        [JsonProperty("expiryDate")]
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// mark record as deleted
        /// </summary>
        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// creation date
        /// </summary>
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }
    }
}