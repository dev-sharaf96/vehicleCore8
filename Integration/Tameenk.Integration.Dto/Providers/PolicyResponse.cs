using System;
using System.Collections.Generic;

namespace Tameenk.Integration.Dto.Providers
{
    public class PolicyResponse
    {
        public string ReferenceId { get; set; }
        public int StatusCode { get; set; }
        public List<Error> Errors { get; set; }
        public string PolicyNo { get; set; }
        public DateTime? PolicyIssuanceDate { get; set; }
        public DateTime? PolicyEffectiveDate { get; set; }
        public DateTime? PolicyExpiryDate { get; set; }
        public string PolicyFileUrl { get; set; }
       
        public Byte[] PolicyFile { get; set; }
        public PolicyDetails PolicyDetails { get; set; }
    }
}
