using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.Inquiry.Components
{
    public class DriverCityInfoOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            NationalAddressCityIsNull = 2,
            SaudiPostResultFailed = 3,
            SaudiPostNoResultReturned = 4,
            NullResponse = 5,
            NoAddressesFoundInSaudiPostOrInDB = 6,
            ParseElmCodeError =7,
            InvalidPublicID=8,
            NoAddressFound=9,
            YakeenResultFailed = 10,
            NoLookupForZipCode=11,
            ServiceException = 12
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
        public DriverCityInfoModel Output
        {
            get;
            set;
        }
        public List<Address> Addresses { get; set; }
    }
}
