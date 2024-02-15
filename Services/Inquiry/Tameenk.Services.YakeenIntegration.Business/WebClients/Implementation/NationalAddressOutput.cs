using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.YakeenIntegration.Business.WebClients.Implementation
{
    public class NationalAddressOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            SuccessAdded = 1,
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
            NationalIdIsNull,
            ChannelIsEmpty,
            HashedIsEmpty,
            HashedNotMatched,
            SaudiPostNoResultReturned,
            SaudiPostResultFailed,
            YakeenResultFailed,
            NoAddressesFoundInSaudiPostOrInDB,
            NationalAddressCityIsNull,
            ParseElmCodeError
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
        public List<Address> Addresses { get; set; }
    }
}
