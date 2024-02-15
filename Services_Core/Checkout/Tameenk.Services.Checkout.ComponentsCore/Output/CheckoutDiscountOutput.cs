using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.Checkout.Components.Output
{
    public class CheckoutDiscountOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            EmptyInputParamter,
            ServiceException,
            InvalidCode,
            CodeExpired
        }

        public string ErrorDescription { get; set; }

        public ErrorCodes ErrorCode { get; set; }

        [JsonProperty("discountData")]
        public RenewalDiscount DiscountData { get; set; }
    }
}
