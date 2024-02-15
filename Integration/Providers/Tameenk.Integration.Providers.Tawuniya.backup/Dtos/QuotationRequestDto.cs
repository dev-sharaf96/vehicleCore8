using Newtonsoft.Json;
using System.Collections.Generic;

namespace Tameenk.Integration.Providers.Tawuniya.Dtos
{
    public class Header
    {

        [JsonProperty("initiatedDateTime")]
        public string InitiatedDateTime { get; set; }

        [JsonProperty("messsageType")]
        public string MesssageType { get; set; }

        [JsonProperty("routingIdentifier")]
        public string RoutingIdentifier { get; set; }

        [JsonProperty("sender")]
        public string Sender { get; set; }

        [JsonProperty("trackingId")]
        public string TrackingId { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }

    public class QuotationInfo
    {

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("idNumber")]
        public string IdNumber { get; set; }

        [JsonProperty("specialSchemeCode")]
        public string SpecialSchemeCode { get; set; }

        [JsonProperty("requiredInceptionDate")]
        public string RequiredInceptionDate { get; set; }

        [JsonProperty("languageCode")]
        public string LanguageCode { get; set; }

        [JsonProperty("proposalNumber")]
        public string ProposalNumber { get; set; }
    }

    public class ChannelDetail
    {

        [JsonProperty("consumerApplicationTypeReference")]
        public string ConsumerApplicationTypeReference { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("sourceCode")]
        public string SourceCode { get; set; }

        [JsonProperty("channelCode")]
        public string ChannelCode { get; set; }
    }

    public class CustomerDetail
    {

        [JsonProperty("fullNameInEnglish")]
        public string FullNameInEnglish { get; set; }

        [JsonProperty("firstNameEnglish")]
        public string FirstNameEnglish { get; set; }

        [JsonProperty("midNameEnglish")]
        public string MidNameEnglish { get; set; }

        [JsonProperty("lastNameEnglish")]
        public string LastNameEnglish { get; set; }

        [JsonProperty("fullNameInArabic")]
        public string FullNameInArabic { get; set; }

        [JsonProperty("firstNameArabic")]
        public string FirstNameArabic { get; set; }

        [JsonProperty("midNameArabic")]
        public string MidNameArabic { get; set; }

        [JsonProperty("lastNameArabic")]
        public string LastNameArabic { get; set; }

        [JsonProperty("dobGreg")]
        public string DobGreg { get; set; }

        [JsonProperty("dobHijri")]
        public string DobHijri { get; set; }

        [JsonProperty("nationalityCode")]
        public string NationalityCode { get; set; }

        [JsonProperty("addressCity")]
        public string AddressCity { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("occupation")]
        public string Occupation { get; set; }

        [JsonProperty("maritalStatus")]
        public string MaritalStatus { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("mailAddressType")]
        public string MailAddressType { get; set; }

        [JsonProperty("zipCode")]
        public string ZipCode { get; set; }

        [JsonProperty("waselPostalCode")]
        public string WaselPostalCode { get; set; }

        [JsonProperty("buildingNumber")]
        public string BuildingNumber { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("district")]
        public string District { get; set; }

        [JsonProperty("insuredEducationCode")]
        public string InsuredEducationCode { get; set; }

        [JsonProperty("insuredChildrenBelow16Years")]
        public string InsuredChildrenBelow16Years { get; set; }
    }

    public class VehicleMoreInfo
    {

        [JsonProperty("engineSizeCode")]
        public string EngineSizeCode { get; set; }

        [JsonProperty("mileage")]
        public string Mileage { get; set; }

        [JsonProperty("annualMileage")]
        public string AnnualMileage { get; set; }

        [JsonProperty("axleWeightCode")]
        public string AxleWeightCode { get; set; }

        [JsonProperty("overNightParkCode")]
        public string OverNightParkCode { get; set; }
    }

    public class HomeAddress
    {

        [JsonProperty("homeCityName")]
        public string HomeCityName { get; set; }

        [JsonProperty("homeBuildingNum")]
        public string HomeBuildingNum { get; set; }

        [JsonProperty("homeStreetNameAR")]
        public string HomeStreetNameAR { get; set; }

        [JsonProperty("homeStreetNameEN")]
        public string HomeStreetNameEN { get; set; }

        [JsonProperty("homeDistrictNameAR")]
        public string HomeDistrictNameAR { get; set; }

        [JsonProperty("homeDistrictNameEN")]
        public string HomeDistrictNameEN { get; set; }

        [JsonProperty("homePostalCode")]
        public string HomePostalCode { get; set; }

        [JsonProperty("homeAdditionalNo")]
        public string HomeAdditionalNo { get; set; }
    }

    public class VehicleDriverDetail
    {

        [JsonProperty("idNo")]
        public string IdNo { get; set; }

        [JsonProperty("eName")]
        public string EName { get; set; }

        [JsonProperty("eFirstName")]
        public string EFirstName { get; set; }

        [JsonProperty("eMiddleName")]
        public string EMiddleName { get; set; }

        [JsonProperty("eLastName")]
        public string ELastName { get; set; }

        [JsonProperty("aName")]
        public string AName { get; set; }

        [JsonProperty("aFirstName")]
        public string AFirstName { get; set; }

        [JsonProperty("aMiddleName")]
        public string AMiddleName { get; set; }

        [JsonProperty("aLastName")]
        public string ALastName { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("mobileNum")]
        public string MobileNum { get; set; }

        [JsonProperty("driverType")]
        public string DriverType { get; set; }

        [JsonProperty("dobGreg")]
        public string DobGreg { get; set; }

        [JsonProperty("dobHijri")]
        public string DobHijri { get; set; }

        [JsonProperty("nationalityCode")]
        public string NationalityCode { get; set; }

        [JsonProperty("relation")]
        public string Relation { get; set; }

        [JsonProperty("socialStatusCode")]
        public string SocialStatusCode { get; set; }

        [JsonProperty("educationCode")]
        public string EducationCode { get; set; }

        [JsonProperty("medicalConditionCode")]
        public string MedicalConditionCode { get; set; }

        [JsonProperty("childrenBelow16Years")]
        public string ChildrenBelow16Years { get; set; }

        [JsonProperty("NCD")]
        public string NCD { get; set; }

        [JsonProperty("homeAddress")]
        public HomeAddress HomeAddress { get; set; }
    }

    public class Feature
    {

        [JsonProperty("featureCode")]
        public string FeatureCode { get; set; }
    }

    public class VehicleDetail
    {

        [JsonProperty("plateType")]
        public string PlateType { get; set; }

        [JsonProperty("makeCode")]
        public string MakeCode { get; set; }

        [JsonProperty("modelCode")]
        public string ModelCode { get; set; }

        [JsonProperty("insurancePeriod")]
        public string InsurancePeriod { get; set; }

        [JsonProperty("plateNumber")]
        public string PlateNumber { get; set; }

        [JsonProperty("plateText1")]
        public string PlateText1 { get; set; }

        [JsonProperty("plateText2")]
        public string PlateText2 { get; set; }

        [JsonProperty("plateText3")]
        public string PlateText3 { get; set; }

        [JsonProperty("yearOfManufacture")]
        public string YearOfManufacture { get; set; }

        [JsonProperty("chassisNumber")]
        public string ChassisNumber { get; set; }

        [JsonProperty("serialNumber")]
        public string SerialNumber { get; set; }

        [JsonProperty("customCardNumber")]
        public string CustomCardNumber { get; set; }


        [JsonProperty("vehicleValue")]
        public string VehicleValue { get; set; }

        [JsonProperty("deductible")]
        public string Deductible { get; set; }

        [JsonProperty("agencyRepairRequired")]
        public string AgencyRepairRequired { get; set; }

        [JsonProperty("registrationExpiryDate")]
        public string RegistrationExpiryDate { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("usageType")]
        public string UsageType { get; set; }

        [JsonProperty("registrationOfficeCity")]
        public string RegistrationOfficeCity { get; set; }

        [JsonProperty("majorDrivingCity")]
        public string MajorDrivingCity { get; set; }

        [JsonProperty("vehicleTransferFlag")]
        public string VehicleTransferFlag { get; set; }

        [JsonProperty("trassferCaseOwnerId")]
        public string TrassferCaseOwnerId { get; set; }

        [JsonProperty("NCDYears")]
        public string NCDYears { get; set; }

        [JsonProperty("vehicleMoreInfo")]
        public IList<VehicleMoreInfo> VehicleMoreInfo { get; set; }

        [JsonProperty("vehicleDriverDetails")]
        public IList<VehicleDriverDetail> VehicleDriverDetails { get; set; }

        [JsonProperty("selectedFeatures")]
        public SelectedFeatures SelectedFeatures { get; set; }
    }

    public class SelectedFeatures
    {

        [JsonProperty("selectedFeatures")]
        public IList<Feature> Features { get; set; }
    }

    public class QuotationRequest
    {

        [JsonProperty("quotationInfo")]
        public QuotationInfo QuotationInfo { get; set; }

        [JsonProperty("channelDetails")]
        public IList<ChannelDetail> ChannelDetails { get; set; }

        [JsonProperty("customerDetails")]
        public IList<CustomerDetail> CustomerDetails { get; set; }

        [JsonProperty("vehicleDetails")]
        public IList<VehicleDetail> VehicleDetails { get; set; }
    }

    public class CreateQuoteRequest
    {

        [JsonProperty("header")]
        public Header Header { get; set; }

        [JsonProperty("quotationRequest")]
        public QuotationRequest QuotationRequest { get; set; }

        public CreateQuoteRequest()
        {
            Header = new Header();
            QuotationRequest = new QuotationRequest();
        }
    }

    public class QuotationRequestDto
    {

        [JsonProperty("createQuoteRequest")]
        public CreateQuoteRequest CreateQuoteRequest { get; set; }

        public QuotationRequestDto()
        {
            CreateQuoteRequest = new CreateQuoteRequest();
        }
    }


}
