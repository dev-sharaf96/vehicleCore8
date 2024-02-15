
namespace Tameenk.Core.Domain.Entities
{
    public class PolicyDetail : BaseEntity
    {
        public int Id { get; set; }

        public string DocumentSerialNo { get; set; }

        public string PolicyNo { get; set; }

        public string InsuranceStartDate { get; set; }

        public string InsuranceEndDate { get; set; }

        public string PolicyIssueDate { get; set; }

        public string PolicyIssueTime { get; set; }

        public string PolicyCoverTypeEn { get; set; }

        public string PolicyCoverTypeAr { get; set; }

        public string PolicyCoverAgeLimitEn { get; set; }

        public string PolicyCoverAgeLimitAr { get; set; }

        public string PolicyAdditionalCoversEn { get; set; }

        public string PolicyAdditionalCoversAr { get; set; }

        public string PolicyGeographicalAreaEn { get; set; }

        public string PolicyGeographicalAreaAr { get; set; }

        public string InsuredNameEn { get; set; }

        public string InsuredNameAr { get; set; }

        public string InsuredMobile { get; set; }

        public string InsuredID { get; set; }

        public string InsuredCity { get; set; }

        public string InsuredDisctrict { get; set; }

        public string InsuredStreet { get; set; }

        public string InsuredBuildingNo { get; set; }

        public string InsuredZipCode { get; set; }

        public string InsuredAdditionalNo { get; set; }

        public string VehicleMakeEn { get; set; }

        public string VehicleMakeAr { get; set; }

        public string VehicleModelEn { get; set; }

        public string VehicleModelAr { get; set; }

        public string VehiclePlateTypeEn { get; set; }

        public string VehiclePlateTypeAr { get; set; }

        public string VehiclePlateNoEn { get; set; }

        public string VehiclePlateNoAr { get; set; }

        public string VehicleChassis { get; set; }

        public string VehicleBodyEn { get; set; }

        public string VehicleBodyAr { get; set; }

        public string VehicleYear { get; set; }

        public string VehicleColorEn { get; set; }

        public string VehicleColorAr { get; set; }

        public string VehicleCapacity { get; set; }

        public string VehicleSequenceNo { get; set; }

        public string VehicleCustomNo { get; set; }

        public string VehicleOwnerName { get; set; }

        public string VehicleOwnerID { get; set; }

        public string VehicleUsingPurposesEn { get; set; }

        public string VehicleUsingPurposesAr { get; set; }

        public string VehicleRegistrationExpiryDate { get; set; }

        public string VehicleValue { get; set; }

        public string OfficePremium { get; set; }

        public string PACover { get; set; }

        public string ValueExcess { get; set; }

        public string TotalPremium { get; set; }

        public string NCDPercentage { get; set; }

        public string NCDAmount { get; set; }

        public string VATPercentage { get; set; }

        public string VATAmount { get; set; }

        public string CommissionPaid { get; set; }

        public string PolicyFees { get; set; }

        public string ClalmLoadingPercentage { get; set; }

        public string ClalmLoadingAmount { get; set; }

        public string AgeLoadingAmount { get; set; }

        public string AgeLoadingPercentage { get; set; }

        public string DeductibleValue { get; set; }

        public string InsuranceStartDateH { get; set; }

        public string InsuranceEndDateH { get; set; }

        public string InsuredAge { get; set; }

        public string NCDFreeYears { get; set; }

        public string AccidentNo { get; set; }

        public string AccidentLoadingPercentage { get; set; }

        public string AccidentLoadingAmount { get; set; }

        public Policy Policy { get; set; }
    }
}
