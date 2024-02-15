using Newtonsoft.Json;
using System;

namespace Tameenk.Services.QuotationDependancy.Component
{
    public class QuoteRequestVehicleInfo
    {
        [JsonProperty("externalId")]
        public string ExternalId { get; set; }

        [JsonProperty("najmNcdFreeYears")]
        public int? NajmNcdFreeYears { get; set; }

        [JsonProperty("iD")]
        public Guid ID { get; set; }

        [JsonProperty("vehicleMaker")]
        public string VehicleMaker { get; set; }

        [JsonProperty("vehicleMakerCode")]
        public short? VehicleMakerCode { get; set; }

        [JsonProperty("vehicleModel")]
        public string VehicleModel { get; set; }

        [JsonProperty("vehicleBodyCode")]
        public byte VehicleBodyCode { get; set; }

        [JsonProperty("modelYear")]
        public short? ModelYear { get; set; }

        [JsonProperty("plateTypeCode")]
        public byte? PlateTypeCode { get; set; }

        [JsonProperty("vehicleIdTypeId")]
        public int VehicleIdTypeId { get; set; }

        [JsonProperty("sequenceNumber")]
        public string SequenceNumber { get; set; }

        [JsonProperty("customCardNumber")]
        public string CustomCardNumber { get; set; }

        [JsonProperty("cehicleModelCode")]
        public long? VehicleModelCode { get; set; }

        [JsonProperty("carPlateText1")]
        public string CarPlateText1 { get; set; }

        [JsonProperty("carPlateText2")]
        public string CarPlateText2 { get; set; }

        [JsonProperty("carPlateText3")]
        public string CarPlateText3 { get; set; }

        [JsonProperty("carPlateNumber")]
        public short? CarPlateNumber { get; set; }

        [JsonProperty("isRenewal")]
        public bool? IsRenewal { get; set; }

        [JsonProperty("previousReferenceId")]
        public string PreviousReferenceId { get; set; }

        [JsonProperty("nCDFreeYearsAr")]
        public string NCDFreeYearsAr { get; set; }

        [JsonProperty("nCDFreeYearsEn")]
        public string NCDFreeYearsEn { get; set; }

        [JsonProperty("quotationCreatedDate")]
        public DateTime QuotationCreatedDate { get; set; }
    }

}
