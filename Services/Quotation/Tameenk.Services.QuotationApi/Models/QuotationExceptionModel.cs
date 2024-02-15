using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.QuotationApi.Models
{
    public class QuotationExceptionModel
    {
        public int ErrorCode { get; set; }
        public string Description { get; set; }

        public string MethodName { get; set; }

        [JsonProperty("QuotationRequestModel")]
        public QuotationRequestModel QuotationRequestModel { get; set; }
        [JsonProperty("QuotationResponseModel")]
        public QuotationResponseModel QuotationResponseModel { get; set; }
    }
}