using System;
using System.Collections.Generic;
using Tameenk.Integration.Dto.Providers;

namespace Tameenk.Services.Policy.Components
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
        public string PolicyExpireTime { get; set; }
        public string PolicyEffectiveTime { get; set; }
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
        public string InsuredNationalAddress { get; set; }
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
        public string PACoverForDriverOnlyEn { get; set; }
        public string PACoverForDriverAndPassenger { get; set; }
        public string PACoverForDriverAndPassengerEn { get; set; }
        public bool HasRoadsideAssistanceBenefit { get; set; }

        public bool DriverCovered { get; set; }
        public bool PassengerCovered { get; set; }
        public string PassengerIncluded { get; set; }
        public string PassengerIncludedEn { get; set; }

        public string ValueExcess { get; set; }
        public string TotalPremium { get; set; }
        public string SpecialDiscount { get; set; }
        public string SpecialDiscount2 { get; set; }

        public string SpecialDiscountPercentage { get; set; }
        public string SpecialDiscount2Percentage { get; set; }

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
        public string VehicleAgencyRepairBenfit { get; set; }
        public string VehicleAgencyRepairBenfitEn { get; set; }
        public string VehicleAgencyRepairValue { get; set; }
        public string InsuranceStartDateH { get; set; }
        public string InsuranceEndDateH { get; set; }
        public string PolicyIssueDateH { get; set; }
        public string InsuredAge { get; set; }
        public string NCDFreeYears { get; set; }
        public string AccidentNo { get; set; }
        public string AccidentLoadingPercentage { get; set; }
        public string AccidentLoadingAmount { get; set; }

        public string MainDriverName { get; set; }
        public string MainDriverIDNumber { get; set; }
        public string MainDriverGender { get; set; }
        public string MainDriverDateofBirth { get; set; }
        public string MainDriverDateofBirthH { get; set; }
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

        public string FifthDriverName { get; set; }
        public string FifthDriverIDNumber { get; set; }
        public string FifthDriverGender { get; set; }
        public string FifthDriverDateofBirth { get; set; }
        public string FifthDriverNumberofyearseligiblefor { get; set; }
        public string FifthDriverNoClaimsDiscount { get; set; }
        public string FifthDriverResidentialAddressCity { get; set; }
        public string FifthDriverFrequencyofDrivingVehicle { get; set; }

        public string PrintingDate { get; set; }
        public int? ChildrenBelow16Years { get; set; }
        public int? EducationCode { get; set; }
        public string Education { get; set; }
        public int? OccupationCode { get; set; }
        public string Occupation { get; set; }
        public string AdditionalNumber { get; set; }
        public string UnitNumber { get; set; }
        public int?  SocialStatusCode { get; set; }
        public string SocialStatus { get; set; }
        public string District { get; set; }
        public string RegionName { get; set; }
        public string Email { get; set; }
        public short? NationalityId { get; set; }
        public string Nationality { get; set; }
        public string InsuranceStartDateDay { get; set; }
        public string InsuranceEndDateDay { get; set; }
        public string BenfitPrice { get; set; }
        public string CoverforDriverBelow21Years { get; set; }
        public string RoadsideAssistanceBenefit { get; set; }
        public string HireCarBenefit { get; set; }
        public string GCCCoverBenefit { get; set; }
        public string BasicPremium { get; set; }
        public string AdditionalAgeContribution { get; set; }
        public string RiskPremiumTrafficViolationLoading { get; set; }
        public string Charges { get; set; }
        public string AnnualPremiumIncludingLoadingsCharges { get; set; }
        public string AnnualPremiumbeforeNoClaimsDiscount { get; set; }
        public string BcareCommission { get; set; }
        public string PersonalAccidentBenefit { get; set; }
        public string NaturalHazardsBenefit { get; set; }
        public string WindscreenFireTheftBenefit { get; set; }
        public string HandbagCoverReimbursementBenefit { get; set; }
        public string ChildSeatCoverReimbursementBenefit { get; set; }

        public string TotalAnnualPremiumAfterNoClaimsDiscount { get; set; }
        public string AnnualPremiumAfterLoyaltyDiscount { get; set; }
        public string TotalAnnualPremiumWithoutVAT { get; set; }
        public string MainDriverLicenseExpiryDate { get; set; }
        public string SecondDriverAge { get; set; }
        public string SecondDriverLicenseExpiryDate { get; set; }
        public string ThirdDriverAge { get; set; }
        public string ThirdDriverLicenseExpiryDate { get; set; }
        public string FourthDriverAge { get; set; }
        public string FourthDriverLicenseExpiryDate { get; set; }
        public string FifthDriverAge { get; set; }
        public string FifthDriverLicenseExpiryDate { get; set; }
        public string PartialLossClaimsEn { get; set; }        public string PartialLossClaimsAr { get; set; }        public string TotalLossClaimsEn { get; set; }        public string TotalLossClaimsAr { get; set; }
        public List<DriverPloicyGenerationDto> Drivers { get; set; }
        public string AlternativeCarAr { get; set; }        public string AlternativeCarEn { get; set; }        public string GCC_1MonthAr { get; set; }        public string GCC_1MonthEn { get; set; }        public string GCC_3MonthAr { get; set; }        public string GCC_3MonthEn { get; set; }        public string GCC_6MonthAr { get; set; }        public string GCC_6MonthEn { get; set; }        public string NONGCC_1MonthAr { get; set; }        public string NONGCC_1MonthEn { get; set; }        public string NONGCC_3MonthAr { get; set; }        public string NONGCC_3MonthEn { get; set; }        public string NONGCC_6MonthAr { get; set; }        public string NONGCC_6MonthEn { get; set; }        public string NONDEPRECIATIONCOVERAr { get; set; }        public string NONDEPRECIATIONCOVEREn { get; set; }
        public string DriverLicenseTypeCode { get; set; }
        public string TPLTotalSubscriptionPremium { get; set; }
        public string ComprehensiveTotalSubscriptionPremium { get; set; }
        public string SelectedBenfits { get; set; }
        public string AdditionalPremium { get; set; }
        public string OfficePremiumWithoutVAT { get; set; }
        public string TotalPremiumWithoutVAT { get; set; }
        public string InsuredIdTypeCode { get; set; }
        public string InsuredIdTypeEnglishDescription { get; set; }
        public string InsuredIdTypeArabicDescription { get; set; }
        public string ProductDescription { get; set; }
        public string BcareCommissionValue { get; set; }
        public string VehicleLimitValue { get; set; }
        public string DriverAgeCoverageBlew21Ar { get; set; }
        public string DriverAgeCoverageBlew21En { get; set; }
        public string DriverAgeCoverageFrom18To20Ar { get; set; }
        public string DriverAgeCoverageFrom18To20En { get; set; }
        public string DriverAgeCoverageFrom21To24Ar { get; set; }
        public string DriverAgeCoverageFrom21To24En { get; set; }
        public string DriverAgeCoverageAbove24Ar { get; set; }
        public string DriverAgeCoverageAbove24En { get; set; }
        public string DriverAgeCoverageFrom17To21Ar_Tpl { get; set; }
        public string DriverAgeCoverageFrom17To21En_Tpl { get; set; }
        public string DriverAgeCoverageFrom21To24Ar_Tpl { get; set; }
        public string DriverAgeCoverageFrom21To24En_Tpl { get; set; }
        public string DriverAgeCoverageFrom24To29Ar_Tpl { get; set; }
        public string DriverAgeCoverageFrom24To29En_Tpl { get; set; }
        public string DriverAgeCoverageAbove29Ar_Tpl { get; set; }
        public string DriverAgeCoverageAbove29En_Tpl { get; set; }
        public string GeographicalCoverageAr { get; set; }
        public string GeographicalCoverageEn { get; set; }


        // for tawuniya as per jira task ALP-81
        public string Code { get; set; }
        public string AdminFee { get; set; }
        public string Surplus { get; set; }
        public string VatableAmount { get; set; }

        // for GGI as per felwa
        public string TransmissionType { get; set; }

        // for AICC as per alaa
        public string BankNationalAddress { get; set; }
        public string Discount { get; set; }
        public string TotalContribution { get; set; }
        public string AccessoriesValue { get; set; }
        public string BenefitsAndExtensions { get; set; }
        public string Contribution { get; set; }
        public string VehicleRepairMethod { get; set; }
        public string AnnualDeprecationPercentages { get; set; }
        public string FirstYearDepreciation { get; set; }
        public string SecondYearDepreciation { get; set; }
        public string ThirdYearDepreciation { get; set; }
        public string FourthYearDepreciation { get; set; }
        public string FifthYearDepreciation { get; set; }
        public string GeoGraphicalBenefits { get; set; }
        public string AutoLeasingTemplateName { get; set; }
        public string LessorMobileNo { get; set; }
        public string LesseeMobileNo { get; set; }
        // for TokioMarine as per Mouneera
        public string RoadsideAssistanceBenefitEn { get; set; }
        // for Salama as per Muneera Al Saqr
        public string MonthlyDeprecationPercentages { get; set; }
        // for AlRajhi as per Moneera Al Saqr
        public string OwnerAsDriver { get; set; }

        // for Amana as per Felwa
        public string GeographicalExtensionBahrain { get; set; }
        public string HireLimit2000 { get; set; }
        public string HireLimit3000 { get; set; }
        //Alrajhi
        public string CommissionAmmount { get; set; }

        // for SAICO as per Felwa
        public string BenefitsList { get; set; }
        //public string NationalDayDiscount { get; set; }
        
            //Allianz 
        public string MaxLimit { get; set; }
        public string CarReplacement_150 { get; set; }
        public string CarReplacement_200 { get; set; }
        public string CarReplacement_75 { get; set; }
        public string GCC_12Month { get; set; }
        public string GCC_Jordan_Lebanon_12months { get; set; }
        public string GE_Jordan_Lebanon_12months { get; set; }
        public string GE_Egypt_Sudan_Turky_12months { get; set; }
        public string GCC_Jordan_Lebanon_Egypt_Sudan_12months { get; set; }
        public string OwnDamage { get; set; }
        public string NaturalDisasters { get; set; }
        public string TheftFire { get; set; }
        public string Towing { get; set; }
        public string EmergencyMedical { get; set; }
        public string TotalBenfitPrice { get; set; }
        public string PassengersPersonalAccidentPrice { get; set; }        public string PersonalAccidentForDriverPrice { get; set; }        public string AuthorizedRepairLimitPrice { get; set; }        public string PersonalBelongingsPrice { get; set; }        public string TowingWithoutAccidentPrice { get; set; }        public string ReplacementOfKeysPrice { get; set; }        public string ProvisionOfReplacementVehiclePrice { get; set; }        public string NonApplicationOfDepreciationInCaseOfTotalLossPrice { get; set; }        public string CoverageForTotalOrPartialLossOfVehiclePrice { get; set; }        public string TowingWithAccidentPrice { get; set; }        public string TheftFirePrice { get; set; }
        public string TotalYearlyDepText { get; set; }        public string HasTrailerAr { get; set; }
        public string HasTrailerEn { get; set; }

        public string DriverRelatedToInsured_Above21Ar { get; set; }
        public string DriverRelatedToInsured_Above21En { get; set; }

        public string HireCar_1500_Max15DaysAr { get; set; }
        public string HireCar_1500_Max15DaysEn { get; set; }

        public string DriverRelatedToInsured_FamilyCoverAr { get; set; }
        public string DriverRelatedToInsured_FamilyCoverEn { get; set; }

        public string HireCar_120_Max15DaysAr { get; set; }        public string HireCar_120_Max15DaysEn { get; set; }        public string HireCar_150_Max15DaysAr { get; set; }        public string HireCar_150_Max15DaysEn { get; set; }        public string HireCar_180_Max15DaysAr { get; set; }        public string HireCar_180_Max15DaysEn { get; set; }        public string HireCar_2000_Max20DaysAr { get; set; }        public string HireCar_2000_Max20DaysEn { get; set; }        public string HireCar_4000_Max20DaysAr { get; set; }        public string HireCar_4000_Max20DaysEn { get; set; }        public string HireCar_5000_Max20DaysAr { get; set; }        public string HireCar_5000_Max20DaysEn { get; set; }

        public string OtherUsesAr { get; set; }
        public string OtherUsesEn { get; set; }        public string VehicleModificationAr { get; set; }
        public string VehicleModificationEn { get; set; }        public string DeathAndphysicalInjuriesAndMedicalExpensesForInsuredOrNamedDriverAr { get; set; }        public string DeathAndphysicalInjuriesAndMedicalExpensesForInsuredOrNamedDriverEn { get; set; }

        public string DeathAndphysicalInjuriesAndMedicalExpensesForPassengersAr { get; set; }        public string DeathAndphysicalInjuriesAndMedicalExpensesForPassengersEn { get; set; }

        public string CoverageForAccidentsOccurringOutsideTheTerritoryOfSAAr { get; set; }        public string CoverageForAccidentsOccurringOutsideTheTerritoryOfSAEn { get; set; }
        public string VehicleOvernightParkingLocationCode { get; set; }
        // for Wataniya as per @Mahmoud        public string SubTotal { get; set; }
        public string SchemesDiscount { get; set; }
        public string SchemesDiscountPercentage { get; set; }        public string AddtionalDriverOneLicenseTypeCode { get; set; }        public string AddtionalDriverOneOccupation { get; set; }        public string AddtionalDriverOneSocialStatus { get; set; }
        public string LiabilitiesToThirdPartiesPrice { get; set; }


        // for AlRajhi as per @Mahmoud
        public List<BenfitSummaryModel> BenefitSummary { get; set; }        // for Buruj as per @Mahmoud
        public List<BenfitSummaryModel> CarEndorsmentList { get; set; }        public List<BenfitSummaryModel> AccidentOutsideTerritory { get; set; }        public List<BenfitSummaryModel> DeathPhysicalInjuriesList { get; set; }        public List<BenfitSummaryModel> DeathPhysicalInjuriesListForPassengers { get; set; }        public List<BenfitSummaryModel> RoadAssistanttList { get; set; }        // for MedGulf as per @Mahmoud
        public string HireCarBenefitEn { get; set; }
        public string GCCCoverBenefitEn { get; set; }
        public string UnNamedDriver { get; set; }
        public string UnNamedDriverEn { get; set; }

        // for Salama as per @Mahmoud
        public string DeathInjuryMedic { get; set; }

        public string InsuranceStartDay { get; set; }
        public string InsuranceEndDay { get; set; }

        // for GGI as per Rawan
        public string PersonalAccidentBenefitEn { get; set; }
        public string ProvisionOfReplacementVehiclePriceEn { get; set; }
       
        // for GGI as per @mahmoud
        public string DeathInjuryMedicEn { get; set; }
        public string DeathInjuryMedicPrice { get; set; }        // for Rajhi as per @Ftoon
        public string ActualAmount { get; set; }


        // for Amana as per Rawan
        public string VehicleModificationDetailsEn { get; set; }
        public string CarReplacement_100_10DaysEn { get; set; }
        public string CarReplacement_100_10DaysAr { get; set; }
        public string CarReplacement_200_10DaysEn { get; set; }
        public string CarReplacement_200_10DaysAr { get; set; }
        public string CarReplacement_300_10DaysEn { get; set; }
        public string CarReplacement_300_10DaysAr { get; set; }
        public string Benefit_57_Ar { get; set; }
        public string Benefit_57_En { get; set; }
        public string Benefit_58_Ar { get; set; }
        public string Benefit_58_En { get; set; }        // for AICC as per Rawan        public string SecondDriverRelationship { get; set; }
        public string AddtionalDriverTwoOccupation { get; set; }
        public string ThirdDriverRelationship { get; set; }

        // for Alamia (Live) as per Rawan
        public string AddtionalDriverTwoSocialStatus { get; set; }
        public string AddtionalDriverTwoLicenseTypeCode { get; set; }

        // for Salama as per Moneera
        public string SumAdditionalBenefitPremiumtPrice { get; set; }
        public string SocialStatusEn { get; set; }
        public string MainDriverGenderEn { get; set; }
        public string VehicleRegistrationTypeEn { get; set; }
    }
}