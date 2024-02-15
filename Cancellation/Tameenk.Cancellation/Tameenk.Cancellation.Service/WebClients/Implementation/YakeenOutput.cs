using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Cancellation.Service.Dto;

namespace Tameenk.Cancellation.Service
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

        



    }
}