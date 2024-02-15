using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Integration.Dto.Najm;

namespace Tameenk.Services.InquiryGateway
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
            ServiceException
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
    }
}