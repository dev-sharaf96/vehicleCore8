using Newtonsoft.Json;

namespace Tameenk.Loggin.DAL.Dtos
{
    public class PolicyStatisticsDataModel
    {
        [JsonProperty("createdDateTime")]
        public DateTime? CreatedDateTime { get;  set; }
        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }
        [JsonProperty("insuredCity")]
        public string InsuredCity { get; set; }
        [JsonProperty("selectedInsuranceTypeCode")]
        public short? SelectedInsuranceTypeCode { get; set; }
        [JsonProperty("vehicleBodyType")]
        public string VehicleBodyType { get; set; }
        [JsonProperty("vehicleMaker")]
        public string VehicleMaker { get; set; }
        [JsonProperty("vehicleModel")]
        public string VehicleModel { get; set; }
        [JsonProperty("modelYear")]
        public short? ModelYear { get; set; }
        [JsonProperty("insuredId")]
        public string InsuredId { get; set; }
        [JsonProperty("insuredBirthDate")]
        public DateTime? InsuredBirthDate { get; set; }
        [JsonProperty("insuredBirthDateH")]
        public string InsuredBirthDateH { get;  set; }
        [JsonProperty("genderId")]
        public int? GenderId { get; set; }
        [JsonProperty("driver1_DateOfBirthG")]
        public DateTime? Driver1_DateOfBirthG { get;  set; }
        [JsonProperty("driver1_GenderCode")]
        public int? Driver1_GenderCode { get; set; }
        [JsonProperty("driver2_DateOfBirthG")]
        public DateTime? Driver2_DateOfBirthG { get;  set; }
        [JsonProperty("driver2_GenderCode")]
        public int? Driver2_GenderCode { get; set; }
        [JsonProperty("priceTypeCode_7_Value")]
        public decimal? PriceTypeCode_7_Value { get; set; }
        [JsonProperty("priceTypeCode_2_Value")]
        public decimal? PriceTypeCode_2_Value { get; set; }
        [JsonProperty("priceTypeCode_2_Percentage")]
        public decimal? PriceTypeCode_2_Percentage { get; set; }
        [JsonProperty("priceTypeCode_3_Value")]
        public decimal? PriceTypeCode_3_Value { get; set; }
        [JsonProperty("priceTypeCode_3_Percentage")]
        public decimal? PriceTypeCode_3_Percentage { get; set; }
        [JsonProperty("priceTypeCode_1_Value")]
        public decimal? PriceTypeCode_1_Value { get; set; }
        [JsonProperty("priceTypeCode_1_Percentage")]
        public decimal? PriceTypeCode_1_Percentage { get; set; }
        [JsonProperty("priceTypeCode_9_Value")]
        public decimal? PriceTypeCode_9_Value { get; set; }
        [JsonProperty("priceTypeCode_9_Percentage")]
        public decimal? PriceTypeCode_9_Percentage { get; set; }
        [JsonProperty("priceTypeCode_8_Value")]
        public decimal? PriceTypeCode_8_Value { get; set; }
        [JsonProperty("priceTypeCode_8_Percentage")]
        public decimal? PriceTypeCode_8_Percentage { get; set; }
        [JsonProperty("productPrice")]
        public decimal? ProductPrice { get; set; }
    }
}
