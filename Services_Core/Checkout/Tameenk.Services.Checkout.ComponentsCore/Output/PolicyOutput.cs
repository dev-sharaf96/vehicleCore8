using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Dtos;
//using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Models.Checkout;
using Tameenk.Models.Payments.Sadad;
using Tameenk.Payment.Esal.Component;
using Tameenk.Services.Core.Payments;

namespace Tameenk.Services.Checkout.Components.Output
{
    public class PolicyOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            EmptyInputParamter = 2,
            ServiceDown = 3,
            InvalidCaptcha = 4,
            ServiceException = 5,
            EmptyReturnObject = 6,
            NoReturnedDraftPolicy,
            NullResponse
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

        [JsonProperty("Policies")]
        public List<UserPolicyModel> Policies { get; set; }
        public List<FailedPolicyModel> FailedPolicies { get; set; }
        
        [JsonProperty("hashed")]
        public string Hashed { get; set; }
    }
}
