using System;
using System.Collections.Generic;
using System.IO;

namespace Tameenk.Integration.Dto.Providers
{
    public class PolicyScheduleResponse
    {
        public string ReferenceId { get; set; }
        public int StatusCode { get; set; }
        public List<Error> Errors { get; set; }
        //private string policyFileUrl = null;
        public string PolicyFileUrl { get; set; }
        
        public Byte[] PolicyFile { get; set; }
    }
}
