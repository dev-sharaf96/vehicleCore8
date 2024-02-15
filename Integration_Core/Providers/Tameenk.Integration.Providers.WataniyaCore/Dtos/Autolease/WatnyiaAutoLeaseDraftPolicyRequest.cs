using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Providers.Wataniya.Dtos.Autolease
{
    public class WatnyiaAutoLeaseDraftPolicyRequest
    {
        public DraftPolicyRequest Policy { get; set; }
        public List<DraftPolicyRiskListRequest> PolicyRiskList { get; set; }
    }

    public class DraftPolicyRequest
    {
        public string ReferenceNumber { get; set; }
        public string PolicyEffectiveDate { get; set; }
        public string PolicyExpiryDate { get; set; }
    }

    public class DraftPolicyRiskListRequest
    {
        [JsonProperty("SequenceNo", NullValueHandling = NullValueHandling.Ignore)]
        public string SequenceNo { get; set; }
        public string UseOfVehicle { get; set; }
        public int RepairCondition { get; set; }
        public int VehicleColor { get; set; }
        public int? PlateType { get; set; }
        public int? PlateNo { get; set; }
        public string PlateNoA { get; set; }
        public string PlateNoB { get; set; }
        public string PlateNoC { get; set; }
        public string IstmaraExpiryDate { get; set; }
        public int VehicleNightParking { get; set; }
        public int TransmissionType { get; set; }
        public int IsThereAdditionalModification { get; set; }
        public string InterestDescription { get; set; }
        public int FireExtinguisher { get; set; }
        public int Weight { get; set; }
        public string EngineNo { get; set; }
        public string EngineCapacity { get; set; }
        public string TypeOfChassis { get; set; }
        public int VehicleCylinder { get; set; }
        public string VehicleMake { get; set; }
        public int VehicleMakeID { get; set; }
        public string ChassisNo { get; set; }
        public int ProductionYear { get; set; }
        public int SumInsured { get; set; }
        [JsonProperty("CustomID", NullValueHandling = NullValueHandling.Ignore)]
        public string CustomID { get; set; }
        public int VehicleDefinitionType { get; set; }
        public int VehicleUsage { get; set; }
        public string VehicleRegion { get; set; }
        public int VehicleType { get; set; }
        public int VehicleTypeID { get; set; }
        public int AntiLockBrakingSystem { get; set; }
        public List<DraftPolicyDriverListRequest> PolicyDriverList { get; set; }
        public List<PolicyPlanList> PolicyPlanList { get; set; }
    }

    public class DraftPolicyDriverListRequest
    {
        public string Lessee { get; set; }
        public float Usage { get; set; }
        public string DriverName { get; set; }
        public string ArabicName { get; set; }
        public string BirthDate { get; set; }
        public int DriverIdType { get; set; }
        public string LicenseNo { get; set; }
        public string LicenseType { get; set; }
        public string Gender { get; set; }
        public int LicenseYear { get; set; }
        public int MaritalStatus { get; set; }
        public string DriverEducation { get; set; }
        public string RelationWithPolicyHolder { get; set; }
        public string UnitNumber { get; set; }
        public string BuildingNumber { get; set; }
        public string StreetName { get; set; }
        public string DistrictName { get; set; }
        public string AdditionalNumber { get; set; }
        public string City { get; set; }
        public string Zone { get; set; }
        public string Country { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ZipCode { get; set; }
        public string DriverOccupation { get; set; }
        public string Mobile { get; set; }
    }
}
