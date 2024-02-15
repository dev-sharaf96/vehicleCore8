using Newtonsoft.Json;
using System.Collections.Generic;

namespace Tameenk.Services.Inquiry.Components
{
    public class InquiryLookupsOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            EmptyInputParamter,
            ServiceDown,
            InvalidCaptcha,
            ServiceException,
            OwnerNationalIdAndNationalIdAreEqual
        }

        public string ErrorDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public ErrorCodes ErrorCode
        {
            get;
            set;
        }

        [JsonProperty("transimissionTypes")]
        public List<Tameenk.Core.Domain.Dtos.InquiryLookup> TransimissionTypes { get; set; }

        [JsonProperty("parkingLocations")]
        public List<Tameenk.Core.Domain.Dtos.InquiryLookup> ParkingLocations { get; set; }

        [JsonProperty("vehicleUsages")]
        public List<Tameenk.Core.Domain.Dtos.InquiryLookup> VehicleUsages { get; set; }

        [JsonProperty("brakingSystems")]
        public List<Tameenk.Core.Domain.Dtos.InquiryLookup> BrakingSystems { get; set; }

        [JsonProperty("cruiseControlTypes")]
        public List<Tameenk.Core.Domain.Dtos.InquiryLookup> CruiseControlTypes { get; set; }

        [JsonProperty("parkingSensors")]
        public List<Tameenk.Core.Domain.Dtos.InquiryLookup> ParkingSensors { get; set; }

        [JsonProperty("vehicleCameraTypes")]
        public List<Tameenk.Core.Domain.Dtos.InquiryLookup> VehicleCameraTypes { get; set; }

        [JsonProperty("mileages")]
        public List<Tameenk.Core.Domain.Dtos.InquiryLookup> Mileages { get; set; }

        [JsonProperty("medicalConditions")]
        public List<Tameenk.Core.Domain.Dtos.InquiryLookup> MedicalConditions { get; set; }

        [JsonProperty("licenseYears")]
        public List<Tameenk.Core.Domain.Dtos.InquiryLookup> LicenseYears { get; set; }

        [JsonProperty("lastAccident")]
        public List<Tameenk.Core.Domain.Dtos.InquiryLookup> LastAccident { get; set; }

        [JsonProperty("violation")]
        public List<Tameenk.Core.Domain.Dtos.InquiryLookup> Violation { get; set; }

        [JsonProperty("education")]
        public List<Tameenk.Core.Domain.Dtos.InquiryLookup> Education { get; set; }

        [JsonProperty("relationShips")]
        public List<Tameenk.Core.Domain.Dtos.InquiryLookup> RelationShips { get; set; }

        [JsonProperty("cities")]
        public List<CityModel> Cities { get; set; }

        [JsonProperty("countries")]
        public List<CountryModel> Countries { get; set; }
    }
}