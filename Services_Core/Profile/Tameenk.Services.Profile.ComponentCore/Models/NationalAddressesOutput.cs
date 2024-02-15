using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.Profile.Component
{
    public class NationalAddressesOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            ReachedToMaximum,
            ModelIsNull,
            EmptyInputParamter,
            NullResult,
            NoAddressFound,
            NoLookupForZipCode,
            ServiceDown,
            ServiceException
        }

        /// <summary>
        /// ErrorDescription
        /// </summary>
        public string ErrorDescription { get; set; }

        /// <summary>
        /// ErrorCode
        /// </summary>
        public ErrorCodes ErrorCode { get; set; }

        public int TotalCount { get; set; }
        public List<AddressInfoModel> Addresses { get; set; }
        public bool CanUpdate { get; set; }
    }
}
