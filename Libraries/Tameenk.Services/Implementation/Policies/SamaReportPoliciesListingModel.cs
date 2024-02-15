using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
    public class SamaReportPoliciesListingModel
    {
        /// <summary>
        /// referenceNo
        /// </summary>
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        public short? InsuranceTypeCode { get; set; }

        /// <summary>
        /// policyNo
        /// </summary>
        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        /// <summary>
        /// Policy Issue Date
        /// </summary>
        [JsonProperty("policyIssueDate")]
        public DateTime? PolicyIssueDate { get; set; }

        public DateTime? PolicyEffectiveDate { get; set; }

        public DateTime? PolicyExpiryDate { get; set; }

        /// <summary>
        /// najmStatusNameEn
        /// </summary>
        [JsonProperty("najmStatus")]
        public string NajmStatus { get; set; }

        /// <summary>
        /// insuranceCompanyNameEn
        /// </summary>
        [JsonProperty("insuranceCompanyName")]
        public string InsuranceCompanyName { get; set; }

        /// <summary>
        /// Checkout Email
        /// </summary>
        [JsonProperty("checkoutEmail")]
        public string CheckoutEmail { get; set; }

        public string CheckoutPhone { get; set; }

        public string IBAN { get; set; }

        /// <summary>
        /// invoiceNo
        /// </summary>
        [JsonProperty("invoiceNo")]
        public int InvoiceNo { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime InvoiceDueDate { get; set; }

        /// <summary>
        /// Product Price
        /// </summary>
        [JsonProperty("productPrice")]
        public decimal? ProductPrice { get; set; }

        public decimal? fees { get; set; }

        public decimal? vat { get; set; }

        public decimal? SubTotalPrice { get; set; }

        /// <summary>
        /// Total Price id
        /// </summary>
        [JsonProperty("totalPrice")]
        public decimal? TotalPrice { get; set; }

        public decimal? ExtraPremiumPrice { get; set; }

        public decimal? DiscountPercentageValue { get; set; }

        public decimal? SchemeDiscount { get; set; }

        public decimal? SchemeDiscountPercentage { get; set; }

        public decimal? LoyaltyDiscountValue { get; set; }

        public decimal? LoyaltyDiscountPercentage { get; set; }

        /// <summary>
        /// Insured Id
        /// </summary>
        [JsonProperty("insuredNIN")]
        public string MainDriverNin { get; set; }

        /// <summary>
        /// Insured Id
        /// </summary>
        [JsonProperty("insuredFullName")]
        public string MainDriverName { get; set; }

        public string City { get; set; }

        public string MainDriverOccupation { get; set; }

        public string MainDriverOccupationCode { get; set; }

        public string PromotionProgram { get; set; }

        public string InsuredID { get; set; }

        public string InsuredNationality { get; set; }

        public DateTime InsuredBirthDate { get; set; }

        public string InsuredBirthDateH { get; set; }

        public string InsuredOccupation { get; set; }

        public string InsuredOccupationCode { get; set; }

        public int? InsuredGender { get; set; }

        public int InsuredEducationId { get; set; }

        public int? InsuredSocialStatus { get; set; }

        public int? InsuredChildrenBelow16Years { get; set; }

        public string InsuredNationalAddress { get; set; }

        public string InsuredWorkCity { get; set; }

        public int? NajmNcdFreeYears { get; set; }

        public int? EXcess { get; set; }

        public bool? IsVehicleAgencyRepair { get; set; }

        public int? MileageExpectedPerYearId { get; set; }

        public short? CarPlateNumber { get; set; }

        public string CustomCardNumber { get; set; }

        public string SequenceNumber { get; set; }

        public string VehicleMakeModel { get; set; }

        public short? ManufactureYear { get; set; }

        public int? SumInsured { get; set; }

        public int VehicleUseId { get; set; }

        public int? TransmissionTypeId { get; set; }

        public int? ParkingLocationId { get; set; }

        public string VehicleModificationDetails { get; set; }

        public string AddditionalDriverOneName { get; set; }

        public string AddditionalDriverOneNin { get; set; }

        public DateTime? AddditionalDriverOneDateOfBirthG { get; set; }

        public string AddditionalDriverOneDateOfBirthH { get; set; }

        public int? AddditionalDriverOneGenderId { get; set; }

        public int? AddditionalDriverOneEducationId { get; set; }

        public int? AddditionalDriverOneSocialStatusId { get; set; }

        public int? AddditionalDriverOneChildrenBelow16Years { get; set; }

        public int? AddditionalDriverOneOccupationId { get; set; }

        public string AddditionalDriverOneOccupation { get; set; }

        public string AddditionalDriverOneOccupationCode { get; set; }

        public int? AddditionalDriverOneNumOfFaultAccidentInLast5Years { get; set; }

        public int? AddditionalDriverOneNCDFreeYears { get; set; }

        public string AddditionalDriverOneWorkCity { get; set; }

        public short? AddditionalDriverOneNationalityCode { get; set; }

        public string AddditionalDriverOneMedicalCondition { get; set; }

        public string AddditionalDriverOneRoadConvictions { get; set; }

        public int? AddditionalDriverOneSaudiLicenseHeldYears { get; set; }

        public string AddditionalDriverOneAddress { get; set; }

        public string AddditionalDriverOneDriverLicense { get; set; }

        public string AddditionalDriverOneDriverExtraLicense { get; set; }

        public string AddditionalDriverTwoName { get; set; }

        public string AddditionalDriverTwoNin { get; set; }

        public DateTime? AddditionalDriverTwoDateOfBirthG { get; set; }

        public string AddditionalDriverTwoDateOfBirthH { get; set; }

        public int? AddditionalDriverTwoGenderId { get; set; }

        public int? AddditionalDriverTwoEducationId { get; set; }

        public int? AddditionalDriverTwoSocialStatusId { get; set; }

        public int? AddditionalDriverTwoChildrenBelow16Years { get; set; }

        public string AddditionalDriverTwoOccupation { get; set; }

        public string AddditionalDriverTwoOccupationCode { get; set; }

        public int? AddditionalDriverTwoNumOfFaultAccidentInLast5Years { get; set; }

        public int? AddditionalDriverTwoNCDFreeYears { get; set; }

        public string AddditionalDriverTwoWorkCity { get; set; }

        public short? AddditionalDriverTwoNationalityCode { get; set; }

        public string AddditionalDriverTwoMedicalCondition { get; set; }

        public string AddditionalDriverTwoRoadConvictions { get; set; }

        public int? AddditionalDriverTwoSaudiLicenseHeldYears { get; set; }

        public string AddditionalDriverTwoAddress { get; set; }

        public string AddditionalDriverTwoDriverLicense { get; set; }

        public string AddditionalDriverTwoDriverExtraLicense { get; set; }

        public string AddditionalDriverThreeName { get; set; }

        public string AddditionalDriverThreeNin { get; set; }

        public DateTime? AddditionalDriverThreeDateOfBirthG { get; set; }

        public string AddditionalDriverThreeDateOfBirthH { get; set; }

        public int? AddditionalDriverThreeGenderId { get; set; }

        public int? AddditionalDriverThreeEducationId { get; set; }

        public int? AddditionalDriverThreeSocialStatusId { get; set; }

        public int? AddditionalDriverThreeChildrenBelow16Years { get; set; }

        public string AddditionalDriverThreeOccupation { get; set; }

        public string AddditionalDriverThreeOccupationCode { get; set; }

        public int? AddditionalDriverThreeNumOfFaultAccidentInLast5Years { get; set; }

        public int? AddditionalDriverThreeNCDFreeYears { get; set; }

        public string AddditionalDriverThreeorkCity { get; set; }

        public short? AddditionalDriverThreeNationalityCode { get; set; }

        public string AddditionalDriverThreeMedicalCondition { get; set; }

        public string AddditionalDriverThreeRoadConvictions { get; set; }

        public int? AddditionalDriverThreeSaudiLicenseHeldYears { get; set; }

        public string AddditionalDriverThreeAddress { get; set; }

        public string AddditionalDriverThreeDriverLicense { get; set; }

        public string AddditionalDriverThreeDriverExtraLicense { get; set; }

        public string InsuranceType { get; set; }
        public int? DeductibleValue { get; set; }
        public string MileageExpectedAnnual { get; set; }
        public string Covarage { get; set; }
        public string DriverLicenseType { get; set; }
        public int? SaudiLicenseHeldYears { get; set; }
        public string DriverExtraLicenses { get; set; }
        public int? NOALast5Years { get; set; }
        public int? NOCLast5Years { get; set; }
        public string DriverViolations { get; set; }
        public string DriverMedicalCondition { get; set; }
        public int VehicleLoad { get; set; }
        public string EngineSize { get; set; }
        public byte VehicleBodyCode { get; set; }
        public string ArabicVehicleBody { get; set; }
        public string EnglishVehicleBody { get; set; }
        public string ChassisNumber { get; set; }
        public string AddditionalDriverOneNOCLast5Years { get; set; }
        public string AddditionalDriverTwoNOCLast5Years { get; set; }
        public string Channel { get; set; }
    }
}
