using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Loggin.DAL;

namespace Tameenk.Services.Inquiry.Components
{
    public class ValidationOutput
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
            OldMobileVersion,
            AccessTokenResultNull,
            BlockedNationalId,
            CompanyDriverExceedsInsuranceNumberLimitPerYear,
            CompanyDriverIsAssignedToAnotherCompany
        }

        public string ErrorDescription
        {
            get;
            set;
        }
        

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public ErrorCodes ErrorCode
        {
            get;
            set;
        }
        public InquiryRequestLog Log
        {
            get;
            set;
        }
        public long DriverNin
        {
            get;
            set;
        }
        public string VehicleId
        {
            get;
            set;
        }
        public InquiryRequestModel RequestModel
        {
            get;
            set;
        }

        public string UserName { get; set; }
        public string UserId { get; set; }
        public string AccessToken { get; set; }
    }
}
