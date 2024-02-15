using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Inquiry.Components
{
    public class OutPutModel
    {
        public enum ErrorCodes
        {
            Success = 1,
            partiaSuccess = 2,
            Failed = 3,
            ExcelIsNull = 4,
            ServiceException = 5,
            ServiceDown = 6
        }

        [JsonProperty("errorDescription")]
        public string ErrorDescription { get; set; }

        [JsonProperty("errorCode")]
        public int ErrorCode { get; set; }

        [JsonProperty("successList")]
        public List<InquiryRequestModel> SuccessList { get; set; }

        [JsonProperty("failedList")]
        public List<FailModel> FailedList { get; set; }
    }
}
