using System;
using System.Collections.Generic;

namespace Tameenk.Services.Inquiry.Components
{
    public class AutoleasingRenewalPoliciesModel
    {
        #region Main Data

        public string ReferenceId { get; set; }
        public DateTime? PolicyExpiryDate { get; set; }
        public int? InsuranceCompanyId { get; set; }
        public string ExternalId { get; set; }
        public int? AutoleasingContractDuration { get; set; }
        public int? DeductableValue { get; set; }
        public bool IsAgencyRepair { get; set; }
        public Guid ProductId { get; set; }
        public string UserId { get; set; }
        public string InsuranceCompanyName { get; set; }
        public string PolicyNo { get; set; }
        public string FullName { get; set; }

        #endregion

        #region Old Policy Selected Benefits

        public List<AutoleasingRenewalPoliciesOldBenefitsModel> Benefits { get; set; }

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
        public int? TransmissionTypeId { get; set; }
        public int? MileageExpectedAnnualId { get; set; }
        public int? ParkingLocationId { get; set; }
        public bool HasModifications { get; set; }
        public string ModificationDetails { get; set; }
        public int VehicleIdTypeId { get; set; }

        #endregion

        #region Main Driver Data

        public string NIN { get; set; }
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
        public string MobileNumber { get; set; }

        #endregion

        #region Additional Driver 1 Data

        public string NINDriver1 { get; set; }
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
        public string MobileNumberDriver1 { get; set; }

        #endregion

        #region Additional Driver 2 Data

        public string NINDriver2 { get; set; }
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
        public string MobileNumberDriver2 { get; set; }

        #endregion

        #region Converted custom to sequence info

        public int? ConvertedSequenceNumber { get; set; }
        public string ConvertedCarPlateText1 { get; set; }
        public string ConvertedCarPlateText2 { get; set; }
        public string ConvertedCarPlateText3 { get; set; }
        public short? ConvertedCarPlateNumber { get; set; }

        #endregion
    }
}
