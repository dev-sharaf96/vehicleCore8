using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Integration.Dto.Providers;

namespace Tameenk.Services.Policy.Components
{
    public class VehicleClaimRegistrationOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            NullResponse,
            UnspecifiedError,
            ServiceError,
            ResFileIsNull,
            ServiceException,
            HttpStatusCodeNotOk,
            InValidData,
            Error,
            AccidentReportNumber,
            InsuredId,
            InsuredMobileNumber,
            InsuredIBAN,
            Failure,
            InsuredBankCode,
            ClaimNo,
            EmptyFilterModel
        }
        /// <summary>
        /// out put error Description
        /// </summary>
        [JsonProperty("errorDescription")]
        public string ErrorDescription { get; set; }

        /// <summary>
        /// out put error Code
        /// </summary>
        [JsonProperty("errorCode")]
        public ErrorCodes ErrorCode { get; set; }

        public ClaimRegistrationServiceResponse ClaimRegistrationServiceResponse { get; set; }
    }
}
