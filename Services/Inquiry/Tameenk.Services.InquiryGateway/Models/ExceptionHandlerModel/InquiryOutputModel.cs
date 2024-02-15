using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.InquiryGateway.Models.ExceptionHandlerModel
{
    public class InquiryOutputModel
    {
        public int StatusCode { get; set; }
        public string Description { get; set; }

        public string MethodName { get; set; }
        [JsonProperty("inquiryResponseModel")]
        public InquiryResponseModel InquiryResponseModel { get; set; }
        [JsonProperty("initInquiryResponseModel")]
        public InitInquiryResponseModel InitInquiryResponseModel { get; set; }

    }
}