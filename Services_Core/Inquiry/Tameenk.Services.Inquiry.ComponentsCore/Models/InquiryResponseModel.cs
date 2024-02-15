using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Tameenk.Services.Inquiry.Components
{
    /// <summary>
    /// Inquiry Response Model
    /// </summary>
    [JsonObject("inquiryResponse")]
    public class InquiryResponseModel
    {
        /// <summary>
        /// The Constructor .
        /// </summary>
        public InquiryResponseModel()
        {
            Errors = new List<string>();
            IsValidInquiryRequest = true;
        }

        /// <summary>
        /// Quotation Request External Id
        /// </summary>
        [JsonProperty("quotationRequestExternalId")]
        public string QuotationRequestExternalId { get; set; }

        /// <summary>
        /// Vehicle
        /// </summary>
        [JsonProperty("vehicle")]
        public VehicleModel Vehicle { get; set; }

        /// <summary>
        /// Errors
        /// </summary>
        [JsonProperty("errors")]
        public List<string> Errors { get; set; }

    
        /// <summary>
        /// Najm Ncd Free Years
        /// </summary>
        [JsonProperty("najmNcdFreeYears")]
        public string NajmNcdFreeYears { get; set; }

        [JsonProperty("Ncd")]
        public object NajmNcd { get; set; }


        [JsonProperty("isValidInquiryRequest")]
        public bool IsValidInquiryRequest { get; set; }

        [JsonProperty("hashedValue")]
        public string HashedValue { get; internal set; }

        [JsonProperty("yakeenMissingFields")]
        public List<YakeenMissingFieldBase> YakeenMissingFields { get; set; }

        //[NotMapped]
        //[JsonProperty("inquiryOutputModel")]
        //public InquiryOutputModel InquiryOutputModel { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }
        [JsonProperty("userId")]
        public string UserId { get; set; }
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }
    }
}