using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Integration.Dto.Providers;

namespace Tameenk.Services.Policy.Components
{
    [JsonObject("CancellPolicyOutput")]
   public class CancellPolicyOutput
    {
        [JsonProperty("ErrorDescription")]
        public string ErrorDescription { get; set; }
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
        public CancelPolicyResponse CancelPolicyResponse { get; set; }

        [JsonProperty("ErrorCode")]
        public int ErrorCode { get; set; }
    }
}
