using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Services.Inquiry.Components
{
    public class DriversOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            EmptyInputParamter = 2,
            ServiceDown = 3,
            InvalidCaptcha = 4,
            ServiceException = 5,
            OwnerNationalIdAndNationalIdAreEqual = 6,
            DrivingPercentageMoreThan100,
            FailedToSaveInVehicleRequests,
            VehicleExceededLimit,
            VehicleYakeenInfoNull,
            VehicleYakeenInfoError,
            ExpiredCaptcha,
            WrongInputCaptcha,
            VehicleOwnerNinIsNull,
            NationalIdIsNull,
            SequenceNumberIsNull,
            VehicleIdTypeIdIsNull,
            AdditionalDriverYakeenError,
            NajmInsuredResponseError,
            NajmAdditionalResponseError,
            DrivingPercentageLessThan100,
            GetDriverCityElmCodeError,
            ElmNoResultReturned,
            ElmCodeEmpty,
            ElmCodeParseError,
            ElmSuccess,
            SaudiPostNoResultReturned,
            YakeenCodeEmpty,
            YakeenAddressSuccess,
            SaudiPostNullResponse,
            InvalidPublicID,
            SaudiPostError,
            EducationIdIsNull,
            ParkingLocationIdIsNull,
            MileageExpectedAnnualIdIsNull,
            TransmissionTypeIdIsNull,
            MedicalConditionIdIsNull,
            ChildrenBelow16YearsIsNull,
            DriverNOALast5YearsIsNull,
            EducationIdIsNullAdditionalDriver,
            MedicalConditionIdIsNullAdditionalDriver,
            DriverNOALast5YearsIsNullAdditionalDriver,
            VehicleValueLessThan10K,
            DriverDriverExtraLicenseNumberOfYearsError,
            DriverDriverExtraLicenseCountryCodeError
        }

        public ErrorCodes ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string LogErrorDescription { get; set; }

        public Driver MainDriver { get; set; }
        public Driver AdditionalDriverOne { get; set; }
        public Driver AdditionalDriverTwo { get; set; }
        public Driver AdditionalDriverThree { get; set; }
        public Driver AdditionalDriverFour { get; set; }
        public List<Driver> AdditionalDrivers { get; set; }
    }
}