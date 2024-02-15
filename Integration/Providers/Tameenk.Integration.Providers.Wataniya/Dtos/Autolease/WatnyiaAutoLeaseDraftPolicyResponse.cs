using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Providers.Wataniya.Dtos.Autolease
{
    public class WatnyiaAutoLeaseDraftPolicyResponse
    {
        public AutoLeasingDraftPolicyResponse Policy { get; set; }
        public List<DraftPolicyResponseRiskList> PolicyRiskList { get; set; }
        public int Status { get; set; }
        public List<AutoleaseError> ErrorList { get; set; }
    }

    public class AutoLeasingDraftPolicyResponse
    {
        public decimal ActualPremium { get; set; }
        public decimal FeeAmount { get; set; }
        public string PolicyCurrency { get; set; }
        public DateTime PolicyEffectiveDate { get; set; }
        public DateTime PolicyExpiryDate { get; set; }
        public decimal PremiumDue { get; set; }
        public string QuotationNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public decimal TaxAmount { get; set; }
    }

    public class DraftPolicyResponseRiskList
    {
        public int AntiLockBrakingSystem { get; set; }
        public string ChassisNo { get; set; }
        [JsonProperty("SequenceNo", NullValueHandling = NullValueHandling.Ignore)]
        public string SequenceNo { get; set; }
        [JsonProperty("CustomID", NullValueHandling = NullValueHandling.Ignore)]
        public string CustomID { get; set; }
        public int IsThereAdditionalModification { get; set; }
        public string PlateNo { get; set; }
        public string PlateNoA { get; set; }
        public string PlateNoB { get; set; }
        public string PlateNoC { get; set; }
        public List<DraftPolicyResponseDriverList> PolicyDriverList { get; set; }
        public List<PolicyPlanList> PolicyPlanList { get; set; }
        public int ProductionYear { get; set; }
        public int RepairCondition { get; set; }
        public string RiskID { get; set; }
        public int SumInsured { get; set; }
        public int TransmissionType { get; set; }
        public string UseOfVehicle { get; set; }
        public int VehicleColor { get; set; }
        public int VehicleDefinitionType { get; set; }
        public string VehicleMake { get; set; }
        public int VehicleNightParking { get; set; }
        public string VehicleRegion { get; set; }
        public string VehicleType { get; set; }
        public int VehicleUsage { get; set; }
    }

    public class DraftPolicyResponseDriverList
    {
        public string ArabicName { get; set; }
        public DateTime BirthDate { get; set; }
        public int DriverIdType { get; set; }
        public string DriverName { get; set; }
        public string Gender { get; set; }
        public string Lessee { get; set; }
        public string LicenseNo { get; set; }
        public int LicenseType { get; set; }
        public int LicenseYear { get; set; }
        public int MaritalStatus { get; set; }
        public int Usage { get; set; }
    }
}
