using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Integration.Dto.Providers;

namespace Tameenk.Services.Policy.Components
{
   public class CompanyAvgResponseTimeOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            NullResponse,
            DatabaseException,
            ServiceException,
        }
        public ErrorCodes ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
    }
}
