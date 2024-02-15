using System;
using System.Collections.Generic;
using System.IO;

namespace Tameenk.Integration.Dto.Providers
{
    public class ComprehensiveImageResponse
    {
        public string ReferenceId { get; set; }
        public int StatusCode { get; set; }
        public List<Error> Errors { get; set; }
    }
}
