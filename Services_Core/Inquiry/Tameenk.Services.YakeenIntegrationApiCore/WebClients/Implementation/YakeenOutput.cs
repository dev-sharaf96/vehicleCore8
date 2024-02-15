using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Services.YakeenIntegrationApi.Dto;

namespace Tameenk.Services.YakeenIntegrationApi
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



    }
}