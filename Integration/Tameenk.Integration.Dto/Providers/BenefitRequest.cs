using Newtonsoft.Json;

namespace Tameenk.Integration.Dto.Providers
{
    public class BenefitRequest
    {
        [JsonIgnore]
        public int? BenefitCode { get; set; }
        public string BenefitId { get; set; }
        [JsonIgnore]
        public string BenefitNameAr { get; set; }
        [JsonIgnore]
        public string BenefitNameEn { get; set; }
        [JsonIgnore]
        public string BenefitDescAr { get; set; }
        [JsonIgnore]
        public string BenefitDescEn { get; set; }
        [JsonIgnore]
        public decimal? BenefitPrice { get; set; }
        [JsonIgnore]
        public bool IsSelected { get; set; }
    }
}
