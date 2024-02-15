using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Integration.Dto.Providers;

namespace Tameenk.Services.Checkout.Components
{
    public class AddBenefitOutput
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
            InvalidData,
            DriverDataError,
            StillMissed,
            OldMobileVersion,
            NotAutoleasingAuthorized,
            MobileNumberIsEmpty,
            MobileNumberNotValid,
            MissingFields
        }

        public string ErrorDescription
        {
            get;
            set;
        }

        public ErrorCodes ErrorCode
        {
            get;
            set;
        }

        public string ReferenceId { set; get; }
        public List<AdditionalBenefitDto> Benefits { get; set; }
    }
}
