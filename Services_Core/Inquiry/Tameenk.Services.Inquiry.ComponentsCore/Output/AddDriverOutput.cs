using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Inquiry.Components
{
    public class AddDriverOutput
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


        public decimal TotalAmount { set; get; }
        public decimal TaxableAmount { set; get; }
        public decimal VATAmount { set; get; }
        public string ReferenceId { set; get; }

        [JsonProperty("productDriverId")]
        public int productDriverId { get; set; }
    }
}




