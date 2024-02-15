using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Services.YakeenIntegration.Business.Dto;
using Tameenk.Services.YakeenIntegration.Business.YakeenBCareService;

namespace Tameenk.Services.YakeenIntegration.Business
{
    public class YakeenOutput
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
            NoAddressFound,
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
        public VehicleYakeenInfoDto Output
        {
            get;
            set;
        }
        public CustomerNameYakeenInfoDto CustomerYakeenInfo
        {
            get;
            set;
        }
        public CustomerIdYakeenInfoDto CustomerIdYakeenInfoDto
        {
            get;
            set;
        }
        public DriverYakeenInfoDto DriverYakeenInfoDto  
        {
            get;
            set;
        }

        public VehiclePlateYakeenInfoDto VehiclePlateYakeenInfoDto
        {
            get;
            set;
        }



        public AlienYakeenInfoDto AlienYakeenInfoDto { get; set; }
        public citizenNatAddressResult CitizenNatAddressResult { get; set; }
        public alienNatAddressResult AlienNatAddressResult { get; set; }
        public CommonErrorObject CommonErrorObject { get; set; }

    }
}