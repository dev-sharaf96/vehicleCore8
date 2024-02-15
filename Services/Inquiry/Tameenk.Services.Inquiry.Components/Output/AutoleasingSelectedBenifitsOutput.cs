using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Integration.Dto.Yakeen;

namespace Tameenk.Services.Inquiry.Components
{
    public class AutoleasingSelectedBenifitsOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            ServiceException
        }

        public ErrorCodes ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
    }
}