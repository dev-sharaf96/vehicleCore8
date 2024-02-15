using System;
using System.Collections.Generic;

namespace Tameenk.Cancellation.Service.Models
{
    public class PolicyActiveResponse : BaseResponse
    {
        public PolicyActiveResponse()
        {
            Policies = new List<Policy>();
        }
        public string RequestNo { get; set; }
        public DateTime RequestExpiryDate { get; set; }
        public List<Policy> Policies { get; set; }
    }
}
