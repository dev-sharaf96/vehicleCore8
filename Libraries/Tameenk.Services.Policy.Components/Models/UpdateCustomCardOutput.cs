using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Policy.Components
{
    public class UpdateCustomCardOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            EmptyInputParamter = 2,
            ServiceDown = 3,
            ServiceException = 4,
            NoPolicies = 5,
            Faile = 6,
            YakeenFailure = 7,
            NullResponse = 8,
            VehicleYakeenInfoNull = 9,
            ProviderIsNull = 10,
            InvalidData = 11,
            YakeenResultIsNull=12,
            CustomCardNotConverted =13,
            FailedToUpdateDBCustomCard,
            CheckoutDetailIsNull,
            PolicyDataIsNull,
            FailedToGeneratePdfFile
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
        public string ServiceRequest
        {
            get;
            set;
        }
        public string ServiceResponse
        {
            get;
            set;
        }
        public string CarPlateText1
        {
            get;
            set;
        }
        public string CarPlateText2
        {
            get;
            set;
        }
        public string CarPlateText3
        {
            get;
            set;
        }
        public short? CarPlateNumber
        {
            get;
            set;
        }
        public string SequenceNumber
        {
            get;
            set;
        }
        public int PolicyStatusId
        {
            get;
            set;
        }
    }
}
