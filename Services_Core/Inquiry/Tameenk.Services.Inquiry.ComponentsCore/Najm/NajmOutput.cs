using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Integration.Dto.Najm;

namespace Tameenk.Services.Inquiry.Components
{
    public class NajmOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            NullResponse,
            UnspecifiedError,
            ServiceError,
            ResFileIsNull,
            ServiceException,
            CorporatePolicyHolderIDIsNOTAllowed,
            ErrorInprocessingYourRequest,
            InvalidCredential,
            InvalidPolicyholderID,
            InvalidVehicleID,
            CommunicationException,
            TimeoutException

        }
        public ErrorCodes ErrorCode
        {
            get;
            set;
        }
        public string ErrorDescription
        {
            get;
            set;
        }
        public NajmResponse Output
        {
            get;
            set;
        }
        public ResponseData NajmDriverCaseResponse
        {
            get;
            set;
        }
    }
}