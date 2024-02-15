using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Providers.Tawuniya.Dtos
{
    public class ProposalHeader
    {

        [JsonProperty("initiatedDateTime")]
        public string InitiatedDateTime { get; set; }

        [JsonProperty("messsageType")]
        public string MesssageType { get; set; }

        [JsonProperty("sender")]
        public string Sender { get; set; }

        [JsonProperty("trackingId")]
        public string TrackingId { get; set; }
    }

    public class ProposalInfo
    {

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("idNumber")]
        public string IdNumber { get; set; }

        [JsonProperty("requiredInceptionDate")]
        public string RequiredInceptionDate { get; set; }

        [JsonProperty("specialSchemeCode")]
        public string SpecialSchemeCode { get; set; }
    }

    public class ProposalChannelDetail
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

    public class ProposalCustomerDetail
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

        [JsonProperty("dobHijri")]
        public string DobHijri { get; set; }

        [JsonProperty("dobGreg")]
        public string DobGreg { get; set; }

        [JsonProperty("nationalityCode")]
        public string NationalityCode { get; set; }

        [JsonProperty("addressCity")]
        public string AddressCity { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }
    }

    public class ProposalHomeAddress
    {

        [JsonProperty("homeCityName")]
        public string HomeCityName { get; set; }
    }

    public class ProposalVehicleDriverDetail
    {

        [JsonProperty("idNo")]
        public string IdNo { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("driverType")]
        public string DriverType { get; set; }

        [JsonProperty("dobHijri")]
        public string DobHijri { get; set; }

        [JsonProperty("dobGreg")]
        public string DobGreg { get; set; }

        [JsonProperty("nationalityCode")]
        public string NationalityCode { get; set; }

        [JsonProperty("relation")]
        public string Relation { get; set; }

        [JsonProperty("NCD")]
        public string NCD { get; set; }

        [JsonProperty("homeAddress")]
        public ProposalHomeAddress HomeAddress { get; set; }
    }

    public class ProposalVehicleDetail
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

        [JsonProperty("vehicleDriverDetails")]
        public IList<ProposalVehicleDriverDetail> VehicleDriverDetails { get; set; }
    }

    public class ProposalRequest
    {

        [JsonProperty("proposalInfo")]
        public ProposalInfo ProposalInfo { get; set; }

        [JsonProperty("channelDetails")]
        public IList<ProposalChannelDetail> ChannelDetails { get; set; }

        [JsonProperty("customerDetails")]
        public IList<ProposalCustomerDetail> CustomerDetails { get; set; }

        [JsonProperty("vehicleDetails")]
        public IList<ProposalVehicleDetail> VehicleDetails { get; set; }
    }

    public class GetProposalsRequest
    {

        [JsonProperty("header")]
        public ProposalHeader Header { get; set; }

        [JsonProperty("proposalRequest")]
        public ProposalRequest ProposalRequest { get; set; }
    }

    public class ProposalsRequestDto
    {

        [JsonProperty("getProposalsRequest")]
        public GetProposalsRequest GetProposalsRequest { get; set; }
    }


}
