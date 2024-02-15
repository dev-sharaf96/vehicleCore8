using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Services.Profile.Component.Models
{
    public class ODPoliciesWithFilterFromDBModel
    {
        #region Policy Master Data.
        public int Id { get; set; }
        public string CheckOutDetailsId { get; set; }
        public int? InsuranceCompanyID { get; set; }
        public DateTime? PolicyEffectiveDate { get; set; }
        public DateTime? PolicyExpiryDate { get; set; }
        public Guid? PolicyFileId { get; set; }
        public DateTime? PolicyIssueDate { get; set; }
        public string PolicyNo { get; set; }
        public byte StatusCode { get; set; }
        public string CheckOutDetailsEmail { set; get; }
        public bool? CheckOutDetailsIsEmailVerified { set; get; }
        public string NajmStatusNameAr { get; set; }
        public string NajmStatusNameEn { get; set; }
        public string PolicyStatusKey { get; set; }
        public string PolicyStatusNameAr { set; get; }
        public string PolicyStatusNameEn { set; get; }
        public string InsuranceCompanyNameAR { set; get; }
        public string InsuranceCompanyNameEN { set; get; }
        public string PolicyExternalId { get; set; }
        #endregion

        #region Vehicle Data
        public Guid ID { get; set; }
        public string SequenceNumber { get; set; }
        public string CustomCardNumber { get; set; }
        public byte? Cylinders { get; set; }
        public string LicenseExpiryDate { get; set; }
        public string MajorColor { get; set; }
        public string MinorColor { get; set; }
        public short? ModelYear { get; set; }
        public byte? PlateTypeCode { get; set; }
        public string RegisterationPlace { get; set; }
        public byte VehicleBodyCode { get; set; }
        public int VehicleWeight { get; set; }
        public int VehicleLoad { get; set; }
        public string VehicleMaker { get; set; }
        public string VehicleModel { get; set; }
        public string ChassisNumber { get; set; }
        public short? VehicleMakerCode { get; set; }
        public long? VehicleModelCode { get; set; }
        public string CarPlateText1 { get; set; }
        public string CarPlateText2 { get; set; }
        public string CarPlateText3 { get; set; }
        public short? CarPlateNumber { get; set; }
        public string CarOwnerNIN { get; set; }
        public int? VehicleValue { get; set; }
        public bool OwnerTransfer { get; set; }
        public string OwnerNationalId { get; set; }
        public int? TransmissionTypeId { get; set; }
        public int? MileageExpectedAnnualId { get; set; }
        public int? ParkingLocationId { get; set; }
        public bool HasModifications { get; set; }
        public string ModificationDetails { get; set; }
        public int VehicleIdTypeId { get; set; }
        public bool? IsUsedCommercially { get; set; }
        #endregion

        #region Main Driver Data
        public string NIN { get; set; }
        public bool IsCitizen { get; set; }
        public DateTime DateOfBirthG { get; set; }
        public string DateOfBirthH { get; set; }
        public int? DrivingPercentage { get; set; }
        public int? ChildrenBelow16Years { get; set; }
        public int EducationId { get; set; }
        public int? MedicalConditionId { get; set; }
        public long? CityId { get; set; }
        public long? WorkCityId { get; set; }
        public int? NOALast5Years { get; set; }
        public string CityName { get; set; }
        public string WorkCityName { get; set; }
        public int? RelationShipId { get; set; }
        public bool? IsSpecialNeed { get; set; }
        public string MainDriverLicense { get; set; }
        public string MainDriverExtraLicense { get; set; }
        public string MainDriverViolations { get; set; }
        #endregion

        #region Additional Driver 1 Data
        public string NINDriver1 { get; set; }
        public bool? IsCitizenDriver1 { get; set; }
        public DateTime? DateOfBirthGDriver1 { get; set; }
        public string DateOfBirthHDriver1 { get; set; }
        public int? DrivingPercentageDriver1 { get; set; }
        public int? ChildrenBelow16YearsDriver1 { get; set; }
        public int? EducationIdDriver1 { get; set; }
        public int? MedicalConditionIdDriver1 { get; set; }
        public long? CityIdDriver1 { get; set; }
        public long? WorkCityIdDriver1 { get; set; }
        public int? NOALast5YearsDriver1 { get; set; }
        public string CityNameDriver1 { get; set; }
        public string WorkCityNameDriver1 { get; set; }
        public int? RelationShipIdDriver1 { get; set; }
        public string LicenseDriver1 { get; set; }
        public string ExtraLicenseDriver1 { get; set; }
        public string ViolationsDriver1 { get; set; }
        #endregion

        #region Additional Driver 2 Data
        public string NINDriver2 { get; set; }
        public bool? IsCitizenDriver2 { get; set; }
        public DateTime? DateOfBirthGDriver2 { get; set; }
        public string DateOfBirthHDriver2 { get; set; }
        public int? DrivingPercentageDriver2 { get; set; }
        public int? ChildrenBelow16YearsDriver2 { get; set; }
        public int? EducationIdDriver2 { get; set; }
        public int? MedicalConditionIdDriver2 { get; set; }
        public long? CityIdDriver2 { get; set; }
        public long? WorkCityIdDriver2 { get; set; }
        public int? NOALast5YearsDriver2 { get; set; }
        public string CityNameDriver2 { get; set; }
        public string WorkCityNameDriver2 { get; set; }
        public int? RelationShipIdDriver2 { get; set; }
        public string LicenseDriver2 { get; set; }
        public string ExtraLicenseDriver2 { get; set; }
        public string ViolationsDriver2 { get; set; }
        #endregion
    }
}
