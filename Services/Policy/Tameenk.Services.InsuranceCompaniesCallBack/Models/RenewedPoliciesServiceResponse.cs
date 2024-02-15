using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.InsuranceCompaniesCallBack.Models
{
    public class RenewedPoliciesServiceResponse:CommonResponseModel
    {
        //public string ReferenceId { get; set; }
        //public int StatusCode { get; set; }

        public enum ErrorCodes
        {
            Success = 1,
            NullResponse,
            NullRequest,
            ServiceError,
            ServiceException,
        }
    }
}