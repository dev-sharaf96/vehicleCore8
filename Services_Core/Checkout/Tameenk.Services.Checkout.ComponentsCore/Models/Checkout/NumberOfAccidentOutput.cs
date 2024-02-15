using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;
using Tameenk.Models.Checkout;
using Tameenk.Models.Payments.Sadad;
using Tameenk.Services.Core.Payments;

namespace Tameenk.Services.Checkout.Components
{
    public class NumberOfAccidentOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            EmptyInputParamter = 2,
            ServiceDown = 3,
            ServiceException = 4,
            QuotationServiceDown = 5,
            NoCompanyReturned = 6,
            DifferentPrice = 7,
            MainDriverAddressIsNull = 8,
            NationalAddressCityIsNull = 9,
            quotationResponseIsNull = 10,
            SuccessSamePrice = 11,
            QuotationExpired = 12,
            DifferentCity = 13,
            NcdFreeYearsIsNull = 14,
            HaveAccidents = 15,
            NoReturnedQuotation=16,
            CurrentProductIsNull=17,
            ProductListCountIsZero=18,
            NotHaveAccidents
        }

        public string ErrorDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public ErrorCodes ErrorCode
        {
            get;
            set;
        }
        public string NewReferenceId
        {
            get;
            set;
        }
        public string newProductId
        {
            get;
            set;
        }
    }
}
