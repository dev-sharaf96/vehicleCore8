using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Providers.Wataniya.Dtos.Autolease
{
    class WatnyiaAutoLeaseQuotationRequest
    {
        [JsonProperty("Policy")]
        public PolicyRequest Policy { get; set; }
        [JsonProperty("PolicyRiskList")]
        public List<PolicyRiskListRequest> PolicyRiskList { get; set; }
    }

    public class PolicyRequest
    {
        [JsonProperty("PolicyEffectiveDate")]
        public string PolicyEffectiveDate { get; set; }
        [JsonProperty("PolicyExpiryDate")]
        public string PolicyExpiryDate { get; set; }
        [JsonProperty("RequestReferenceNumber")]
        public string RequestReferenceNumber { get; set; }
    }

    public class PolicyRiskListRequest
    {
        [JsonProperty("RiskID")]
        public string RiskID { get; set; }
        [JsonProperty("PolicyDriverList")]
        public List<PolicyDriverListRequest> PolicyDriverList { get; set; }
        [JsonProperty("VehicleDefinitionType")]
        public int VehicleDefinitionType { get; set; }

        [JsonProperty("SequenceNo", NullValueHandling = NullValueHandling.Ignore)]
        public string SequenceNo { get; set; }
        [JsonProperty("CustomID", NullValueHandling = NullValueHandling.Ignore)]
        public string CustomID { get; set; }
        [JsonProperty("SumInsured")]
        public int SumInsured { get; set; }
        public string VehicleMake { get; set; }
        public int VehicleMakeID { get; set; }
        [JsonProperty("VehicleType")]
        public int VehicleType { get; set; }
        [JsonProperty("VehicleRegion")]
        public string VehicleRegion { get; set; }
        [JsonProperty("RepairCondition")]
        public int RepairCondition { get; set; }
        [JsonProperty("ProductionYear")]
        public int ProductionYear { get; set; }
        [JsonProperty("ChassisNo")]
        public string ChassisNo { get; set; }
        [JsonProperty("VehicleColor")]
        public int VehicleColor { get; set; }
        [JsonProperty("VehicleUsage")]
        public int VehicleUsage { get; set; }
        [JsonProperty("PlateType")]
        public int? PlateType { get; set; }
        [JsonProperty("PlateNo")]
        public int? PlateNo { get; set; }
        [JsonProperty("PlateNoA")]
        public string PlateNoA { get; set; }

        [JsonProperty("PlateNoB")]
        public string PlateNoB { get; set; }

        [JsonProperty("PlateNoC")]
        public string PlateNoC { get; set; }
        [JsonProperty("IstmaraExpiryDate")]
        public string IstmaraExpiryDate { get; set; }
        [JsonProperty("VehicleNightParking")]
        public int VehicleNightParking { get; set; }
        [JsonProperty("TransmissionType")]
        public int TransmissionType { get; set; }
        [JsonProperty("IsThereAdditionalModification")]
        public int IsThereAdditionalModification { get; set; }
        [JsonProperty("InterestDescription")]
        public string InterestDescription { get; set; }
        [JsonProperty("AntiLockBrakingSystem")]
        public int AntiLockBrakingSystem { get; set; }
        [JsonProperty("FireExtinguisher")]
        public int FireExtinguisher { get; set; }
        [JsonProperty("Weight")]
        public int Weight { get; set; }
        [JsonProperty("EngineNo")]
        public string EngineNo { get; set; }
        [JsonProperty("EngineCapacity")]
        public string EngineCapacity { get; set; }
        [JsonProperty("VehicleCylinder")]
        public int VehicleCylinder { get; set; }
        [JsonProperty("RegistrationCountry", NullValueHandling = NullValueHandling.Ignore)]
        public string RegistrationCountry { get; set; }
        [JsonProperty("RegistrationCity", NullValueHandling = NullValueHandling.Ignore)]
        public string RegistrationCity { get; set; }
        public string TypeOfChassis { get; set; }
        public string UseOfVehicle { get; set; }
        public int VehicleTypeID { get; set; }
    }

    public class PolicyDriverListRequest
    {
        [JsonProperty("Lessee")]
        public string Lessee { get; set; }
        [JsonProperty("Usage")]
        public float Usage { get; set; }
        [JsonProperty("DriverName")]
        public string DriverName { get; set; }
        [JsonProperty("ArabicName")]
        public string ArabicName { get; set; }
        [JsonProperty("BirthDate")]
        public string BirthDate { get; set; }
        [JsonProperty("DriverIdType")]
        public int DriverIdType { get; set; }
        [JsonProperty("LicenseNo")]
        public string LicenseNo { get; set; }
        [JsonProperty("LicenseType")]
        public int LicenseType { get; set; }
        [JsonProperty("Gender")]
        public string Gender { get; set; }
        [JsonProperty("LicenseYear")]
        public int LicenseYear { get; set; }
        [JsonProperty("MaritalStatus")]
        public int MaritalStatus { get; set; }
        [JsonProperty("DriverEducation")]
        public string DriverEducation { get; set; }
        [JsonProperty("RelationWithPolicyHolder")]
        public string RelationWithPolicyHolder { get; set; }
        [JsonProperty("UnitNumber")]
        public string UnitNumber { get; set; }
        [JsonProperty("BuildingNumber")]
        public string BuildingNumber { get; set; }
        [JsonProperty("StreetName")]
        public string StreetName { get; set; }
        [JsonProperty("DistrictName")]
        public string DistrictName { get; set; }
        [JsonProperty("AdditionalNumber")]
        public string AdditionalNumber { get; set; }
        [JsonProperty("City")]
        public string City { get; set; }
        [JsonProperty("Zone")]
        public string Zone { get; set; }
        [JsonProperty("Country")]
        public string Country { get; set; }
        [JsonProperty("Latitude")]
        public string Latitude { get; set; }
        [JsonProperty("Longitude")]
        public string Longitude { get; set; }
        [JsonProperty("ZipCode")]
        public string ZipCode { get; set; }
        [JsonProperty("DriverOccupation")]
        public string DriverOccupation { get; set; }
        [JsonProperty("MobileNo", NullValueHandling = NullValueHandling.Ignore)]
        public string MobileNo { get; set; }
    }
}
