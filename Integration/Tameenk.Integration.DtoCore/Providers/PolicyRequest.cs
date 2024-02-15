using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Tameenk.Integration.Dto.Providers
{
    public class PolicyRequest
    {

        public string ReferenceId { get; set; }
        public string QuotationNo { get; set; }
        public string ProductId { get; set; }
        public List<BenefitRequest> Benefits { get; set; }
        public long InsuredId { get; set; }
        public string InsuredMobileNumber { get; set; }
        public string InsuredEmail { get; set; }
        public int InsuredBuildingNo { get; set; }
        public int InsuredZipCode { get; set; }
        public int InsuredAdditionalNumber { get; set; }
        public int InsuredUnitNo { get; set; }
        public string InsuredCity { get; set; }
        public string InsuredDistrict { get; set; }
        public string InsuredStreet { get; set; }
        public int PaymentMethodCode { get; set; }
        public string PaymentMethod { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentBillNumber { get; set; }
        public string InsuredBankCode { get; set; }
        public string PaymentUsername { get; set; }
        public string InsuredIBAN { get; set; }

        //[JsonIgnore]
        [JsonProperty("PolicyEffectiveDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? PolicyEffectiveDate { get; set; }

        [JsonIgnore]
        public int QuotationRequestId { get; set; }

        [JsonIgnore]
        public Guid ProductInternalId { get; set; }
        [JsonProperty("InsuredCityCode", NullValueHandling = NullValueHandling.Ignore)]
        public string InsuredCityCode { get; set; }

        [JsonProperty("VehicleIdTypeCode", NullValueHandling = NullValueHandling.Ignore)]
        public int? VehicleIdTypeCode { get; set; }

        [JsonProperty("VehicleId", NullValueHandling = NullValueHandling.Ignore)]
        public string VehicleId { get; set; }

        [JsonProperty("VehiclePlateTypeCode", NullValueHandling = NullValueHandling.Ignore)]
        public string VehiclePlateTypeCode { get; set; }

        [JsonProperty("VehicleMajorColor", NullValueHandling = NullValueHandling.Ignore)]
        public string VehicleMajorColor { get; set; }

        [JsonProperty("VehicleMajorColorCode", NullValueHandling = NullValueHandling.Ignore)]
        public string VehicleMajorColorCode { get; set; }

        [JsonProperty("VehicleBodyTypeCode", NullValueHandling = NullValueHandling.Ignore)]
        public string VehicleBodyTypeCode { get; set; }

        [JsonProperty("VehicleRegPlaceCode", NullValueHandling = NullValueHandling.Ignore)]
        public string VehicleRegPlaceCode { get; set; }

        [JsonProperty("VehicleRegPlace", NullValueHandling = NullValueHandling.Ignore)]
        public string VehicleRegPlace { get; set; }

        [JsonProperty("VehicleRegExpiryDate", NullValueHandling = NullValueHandling.Ignore)]
        public string VehicleRegExpiryDate { get; set; }

        [JsonProperty("VehicleCylinders", NullValueHandling = NullValueHandling.Ignore)]
        public int? VehicleCylinders { get; set; }

        [JsonProperty("VehicleWeight", NullValueHandling = NullValueHandling.Ignore)]
        public int? VehicleWeight { get; set; }

        [JsonProperty("VehicleLoad", NullValueHandling = NullValueHandling.Ignore)]
        public int? VehicleLoad { get; set; }

        [JsonProperty("NCDFreeYears", NullValueHandling = NullValueHandling.Ignore)]
        public int? NCDFreeYears { get; set; }

        [JsonProperty("NCDReference", NullValueHandling = NullValueHandling.Ignore)]
        public string NCDReference { get; set; }

        [JsonProperty("VehicleChassisNumber", NullValueHandling = NullValueHandling.Ignore)]
        public string VehicleChassisNumber { get; set; }

        [JsonProperty("VehiclePlateNumber", NullValueHandling = NullValueHandling.Ignore)]
        public int? VehiclePlateNumber { get; set; }

        [JsonProperty("VehiclePlateText1", NullValueHandling = NullValueHandling.Ignore)]
        public string VehiclePlateText1 { get; set; }

        [JsonProperty("VehiclePlateText2", NullValueHandling = NullValueHandling.Ignore)]
        public string VehiclePlateText2 { get; set; }

        [JsonProperty("VehiclePlateText3", NullValueHandling = NullValueHandling.Ignore)]
        public string VehiclePlateText3 { get; set; }

        [JsonProperty("VehicleOvernightParkingLocationCode", NullValueHandling = NullValueHandling.Ignore)]
        public int? VehicleOvernightParkingLocationCode { get; set; }

        [JsonProperty("DeductibleAmount", NullValueHandling = NullValueHandling.Ignore)]
        public string DeductibleAmount { get; set; }

        [JsonProperty("PolicyPremium", NullValueHandling = NullValueHandling.Ignore)]
        public string PolicyPremium { get; set; }

        [JsonProperty("Netpremium", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Netpremium { get; set; }

        [JsonIgnore]
        public string Language { get; set; }
    }
}
