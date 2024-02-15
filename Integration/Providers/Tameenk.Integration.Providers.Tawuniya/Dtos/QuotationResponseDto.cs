using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Providers.Tawuniya.Dtos
{
    public class QuotationResponseInfo
    {

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("idNumber")]
        public string IdNumber { get; set; }

        [JsonProperty("specialSchemeCode")]
        public string SpecialSchemeCode { get; set; }

        [JsonProperty("requiredInceptionDate")]
        public string RequiredInceptionDate { get; set; }

        [JsonProperty("languageCode")]
        public string LanguageCode { get; set; }

        [JsonProperty("quotationNumber")]
        public string QuotationNumber { get; set; }

        [JsonProperty("policyFee")]
        public decimal PolicyFee { get; set; }

        [JsonProperty("VATRate")]
        public decimal VATRate { get; set; }

        [JsonProperty("VATAmount")]
        public decimal VATAmount { get; set; }

        [JsonProperty("totalVehiclePremium")]
        public string TotalVehiclePremium { get; set; }

        [JsonProperty("paymentAmount")]
        public decimal PaymentAmount { get; set; }

        [JsonProperty("NCDRate")]
        public decimal NCDRate { get; set; }

        [JsonProperty("NCDAmount")]
        public decimal NCDAmount { get; set; }

        [JsonProperty("totalLoading")]
        public decimal TotalLoading { get; set; }
        [JsonProperty("promoAmount")]
        public decimal? PromoAmount { get; set; }

        [JsonProperty("promoPercentage")]
        public decimal? PromoPercentage { get; set; }

    }

    public class QuotationResult
    {

        [JsonProperty("resultCode")]
        public string ResultCode { get; set; }

        [JsonProperty("resultMessage")]
        public string ResultMessage { get; set; }
    }

    public class QuotationResponse
    {

        [JsonProperty("quotationInfo")]
        public QuotationResponseInfo QuotationInfo { get; set; }

        [JsonProperty("quotationResult")]
        public QuotationResult QuotationResult { get; set; }
    }

    public class QuotationError
    {

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("errorDescription")]
        public string ErrorDescription { get; set; }

        [JsonProperty("errorType")]
        public string ErrorType { get; set; }
    }

    public class CreateQuoteResponse
    {

        [JsonProperty("quotationResponse")]
        public QuotationResponse QuotationResponse { get; set; }

        [JsonProperty("exception")]
        public IList<QuotationError> Errors { get; set; }
    }

    public class QuotationResponseDto
    {

        [JsonProperty("createQuoteResponse")]
        public CreateQuoteResponse CreateQuoteResponse { get; set; }
    }


}
