using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Integration.Dto.Yakeen;

namespace Tameenk.Services.Inquiry.Components
{
    public class DriverOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            ServiceFail = 2,
            ServiceException = 3,
            ServiceDown = 4
        }

        public ErrorCodes ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public YakeenInfoErrorModel Error { get; set; }

        public Driver Driver { get; set; }
    }
}