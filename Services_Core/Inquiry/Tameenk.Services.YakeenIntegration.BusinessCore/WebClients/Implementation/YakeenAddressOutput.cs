using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.YakeenIntegration.Business.YakeenBCareService;

namespace Tameenk.Services.YakeenIntegration.Business
{
    public class YakeenAddressOutput
    {
        public enum ErrorCodes
        {
            Success= 1,
            SuccessAdded=1,
            NullResponse,
            UnspecifiedError,
            ServiceError,
            NullRequest,
            DriverIsNull,
            ResultIsNull,
            ResultIsFailed,
            ServiceException,
            TotalSearchResultsIsZero,
            InvalidPublicID,
            StatusdescriptionIsNotSuccess,
            YakeenServiceException,
            DateOfBirthGIsEmpty,
            NoAddressFound,
            NoLookupForZipCode,
            CommunicationException,
            TimeoutException
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
        public List<YakeenAddressResult> Addresses { get; set; }
        public int LogId { get; set; }
        public CommonErrorObject CommonErrorObject { get; set; }
        public List<Address> DriverAddresses { get; set; }
    }
}