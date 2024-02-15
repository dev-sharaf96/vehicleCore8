using Newtonsoft.Json;
using System.Collections.Generic;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Integration.Dto.Providers;

namespace Tameenk.Services.QuotationNew.Components
{
    public class QuotationNewResult
    {
        [JsonProperty("quotationResponseModel")]
        public QuotationNewResponseModel QuotationResponseModel { get; set; }

        public QuotationResponse QuotationResponse { get; set; }
        public List<QuotationNewProductDto> Products { get; set; }
        public QuotationServiceRequest QuotationServiceRequest { get; set; }

        public int VehicleValue { get; set; }
        public bool? IsInitialQuotation { get; set; }

        public string TermsAndConditionsFilePathComp
        {
            get;
            set;
        }
        public string TermsAndConditionsFilePath
        {
            get;
            set;
        }
        public string TermsAndConditionsFilePathSanadPlus
        {
            get;
            set;
        }
        public bool ShowTabby
        {
            get;
            set;
        }
        public bool ActiveTabbyTPL
        {
            get;
            set;
        }
        public bool ActiveTabbyComp
        {
            get;
            set;
        }
        public bool ActiveTabbySanadPlus
        {
            get;
            set;
        }
        public bool ActiveTabbyWafiSmart
        {
            get;
            set;
        }

        public bool IsRenewal
        {
            get;
            set;
        }

        public bool ActiveTabbyMotorPlus
        {
            get;
            set;
        }
    }
}
