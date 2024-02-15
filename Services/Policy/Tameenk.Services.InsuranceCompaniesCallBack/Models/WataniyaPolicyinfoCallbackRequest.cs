using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.InsuranceCompaniesCallBack.Models
{
    public class WataniyaPolicyinfoCallbackRequest
    {
        public short InsuranceCompanyCode { get; set; }
        public int? InsuranceTypeID { get; set; } // in case of TPL only
        public string NewPolicyNumber { get; set; }
        public string OldPolicyNumber { get; set; }

        // for TPL only
        public long QuoteReferenceNo { get; set; }
        public string RequestReferenceNo { get; set; }

        // for COMP only
        public long PolicyReferenceNo { get; set; }
        public string PolicyRequestReferenceNo { get; set; }
    }

    public class WataniyaCallBackOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            NullResponse,
            NullRequest,
            ServiceError,
            ServiceException,
        }

        public ErrorCodes ErrorCode { get; set; }

        public string ErrorDescription { get; set; }

        public WataniyaPolicyinfoCallbackResponse CallbackResponse { get; set; }
    }

    public class WataniyaPolicyinfoCallbackResponse
    {
        public bool Status { get; set; }
        public List<Errors> errors { get; set; }
    }

    public class Errors
    {
        public string field { get; set; }
        public string message { get; set; }
        public string code { get; set; }
    }
}