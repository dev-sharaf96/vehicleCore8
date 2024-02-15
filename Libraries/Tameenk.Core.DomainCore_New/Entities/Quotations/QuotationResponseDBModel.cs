using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Quotations
{
    public class QuotationResponseDBModel
    {
        public long QuotationResponseId { get; set; }
        public Guid DriverId { get; set; }
        public string NIN { get; set; }
        public Guid VehicleId { get; set; }
        public int? VehicleIdTypeId { get; set; }
        public string CustomCardNumber { get; set; }
        public string SequenceNumber { get; set; }
        public int CompanyID { get; set; }
        public string CompanyKey { get; set; }
        public DateTime? RequestPolicyEffectiveDate { get; set; }
        public long QuotationResponseCityId { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string ThirdName { get; set; }
        public string LastName { get; set; }
        public short? InsuranceTypeCode { get; set; }
        public string ExternalId { get; set; }
        public long InsuredCityId { get; set; }
        public string EnglishFirstName { get; set; }
        public string EnglishSecondName { get; set; }
        public string EnglishThirdName { get; set; }
        public string EnglishLastName { get; set; }
        public int? ProductInsuranceTypeCode { get; set; }
        public short? VehicleModelYear { get; set; }
        public int? NajmNcdFreeYears { get; set; }
        public List<Guid> AdditionalDriverList { get; set; }
        public bool? AutoleasingInitialOption { get; set; }
        public bool? AutoleasingInitialOptionResponce { get; set; }
        public bool? IsConverted { get; set; }
        public string ReferenceId { get; set; }
        public DateTime? CreatedDate { get; set; }
       

        public DateTime DateOfBirthG { get; set; }
        public string DateOfBirthH { get; set; } 
        public string MobileNumber { set; get; }
        public string InitialExternalId { get; set; }
        public string InsuredNationalId { get; set; }
        public DateTime QuotationResponseCreatedDate { get; set; }
        public string VehicleModel { get; set; }        public string VehicleMaker { get; set; }        public string CityName { get; set; }        public int? AddressId { get; set; }
        public string PostCode { get; set; }        public string District { get; set; }        public string BuildingNumber { get; set; }        public string AddressCityName { get; set; }        public int InsuredTableRowId { get; set; }
    }
}
