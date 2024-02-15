using System;
using System.Collections.Generic;
using Tameenk.Integration.Dto.Providers;

namespace Tameenk.Services.PolicyApi.Models
{
    public class PolicyTemplateGenerationModel
    {
        public string ReferenceNo { get; set; }
        public string DocumentSerialNo { get; set; }
        public string PolicyNo { get; set; }
        public string ProductType { get; set; }
        public string ProductTypeAr { get; set; }

        public string InsuranceStartDate { get; set; }
        public string InsuranceEndDate { get; set; }
        public string InsuranceDuration { get; set; }
        public string PolicyIssueDate { get; set; }
        public string PolicyIssueTime { get; set; }
        public string InsuranceStartDateH { get; set; }
        public string InsuranceEndDateH { get; set; }
        public string PolicyIssueDateH { get; set; }
        public string PolicyCoverTypeEn { get; set; }
        public string PolicyCoverTypeAr { get; set; }
        public string PolicyUserFullName { get; set; }
        public List<string> PolicyCoverAgeLimitEn { get; set; }
        public List<string> PolicyCoverAgeLimitAr { get; set; }
        public List<string> PolicyAdditionalCoversEn { get; set; }
        public List<string> PolicyAdditionalCoversAr { get; set; }
        public string PolicyGeographicalAreaEn { get; set; }
        public string PolicyGeographicalAreaAr { get; set; }
        public string InsuredNameEn { get; set; }
        public string InsuredNameAr { get; set; }
        public string InsuredMobile { get; set; }
        public string InsuredID { get; set; }
        public string InsuredBankName { get; set; }
        public string InsuredIbanNumber { get; set; }
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
        public string VehiclePlateText { get; set; }
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
        public string VehicleWeight { get; set; }
        public string VehicleLoad { get; set; }
        public string VehicleEngineSize { get; set; }
        public string VehicleOdometerReading { get; set; }
        public string VehicleModificationDetails { get; set; }        
        public string VehicleRegistrationType { get; set; }
        public string VehicleValue { get; set; }
        public string OfficePremium { get; set; }
        public string PACover { get; set; }
        public string PACoverForDriverOnly { get; set; }
        public string PACoverForDriverAndPassenger { get; set; }
        public string ValueExcess { get; set; }
        public string TotalPremium { get; set; }
        public string SpecialDiscount { get; set; }
        public string LoyaltyDiscount { get; set; }
        public string LoyaltyDiscountPercentage { get; set; }
        public string NoClaimDiscount { get; set; }
        public string NCDPercentage { get; set; }
        public string NCDAmount { get; set; }
        public string VATPercentage { get; set; }
        public string VATAmount { get; set; }
        public string CommissionPaid { get; set; }
        public string TPLCommissionAmount { get; set; }
        public string ComprehesiveCommissionAmount { get; set; }
        public string PolicyFees { get; set; }
        public string ClalmLoadingPercentage { get; set; }
        public string ClalmLoadingAmount { get; set; }
        public string AgeLoadingAmount { get; set; }
        public string AgeLoadingPercentage { get; set; }
        public string DeductibleValue { get; set; }
        public string VehicleAgencyRepairAr { get; set; }
        public string VehicleAgencyRepairEn { get; set; }        
        public string InsuredAge { get; set; }
        public string NCDFreeYears { get; set; }
        public string AccidentNo { get; set; }
        public string AccidentLoadingPercentage { get; set; }
        public string AccidentLoadingAmount { get; set; }

        public string MainDriverName { get; set; }
        public string MainDriverIDNumber { get; set; }
        public string MainDriverGender { get; set; }
        public string MainDriverDateofBirth { get; set; }
        public string MainDriverNumberofyearseligiblefor { get; set; }
        public string MainDriverNoClaimsDiscount { get; set; }
        public string MainDriverResidentialAddressCity { get; set; }
        public string MainDriverFrequencyofDrivingVehicle { get; set; }

        public string SecondDriverName { get; set; }
        public string SecondDriverIDNumber { get; set; }
        public string SecondDriverGender { get; set; }
        public string SecondDriverDateofBirth { get; set; }
        public string SecondDriverNumberofyearseligiblefor { get; set; }
        public string SecondDriverNoClaimsDiscount { get; set; }
        public string SecondDriverResidentialAddressCity { get; set; }
        public string SecondDriverFrequencyofDrivingVehicle { get; set; }
        public string ThirdDriverName { get; set; }
        public string ThirdDriverIDNumber { get; set; }
        public string ThirdDriverGender { get; set; }
        public string ThirdDriverDateofBirth { get; set; }
        public string ThirdDriverNumberofyearseligiblefor { get; set; }
        public string ThirdDriverNoClaimsDiscount { get; set; }
        public string ThirdDriverResidentialAddressCity { get; set; }
        public string ThirdDriverFrequencyofDrivingVehicle { get; set; }

        public string FourthDriverName { get; set; }
        public string FourthDriverIDNumber { get; set; }
        public string FourthDriverGender { get; set; }
        public string FourthDriverDateofBirth { get; set; }
        public string FourthDriverNumberofyearseligiblefor { get; set; }
        public string FourthDriverNoClaimsDiscount { get; set; }
        public string FourthDriverResidentialAddressCity { get; set; }
        public string FourthDriverFrequencyofDrivingVehicle { get; set; }
        public string PrintingDate{ get; set; }


        public List<DriverPloicyGenerationDto> Drivers { get; set; }
    }
}