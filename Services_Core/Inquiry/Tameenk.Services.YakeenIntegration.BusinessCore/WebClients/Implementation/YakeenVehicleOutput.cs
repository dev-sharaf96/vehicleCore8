using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Services.YakeenIntegration.Business.Dto;
using Tameenk.Services.YakeenIntegration.Business.YakeenBCareService;

namespace Tameenk.Services.YakeenIntegration.Business
{
    public class YakeenVehicleOutput
    {
        public enum ErrorCodes 
        {
            Success = 1,
            NullResponse,
            UnspecifiedError,
            ServiceError,
            NullRequest,
            ServiceException,
            YakeenServiceException,
            DateOfBirthGIsEmpty,
            NoAddressFound
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
        public carInfoByCustomTwoResult result { get; set; }
    }
}