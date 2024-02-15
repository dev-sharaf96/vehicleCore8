using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Integration.Dto.Providers;

namespace Tameenk.Integration.Providers.MedGulf
{
    internal class MedGulfPolicyResponse
    {
        public string ReferenceId { get; set; }
        public int StatusCode { get; set; }
        public List<Error> Errors { get; set; }
        public string PolicyNo { get; set; }
        public string PolicyIssuanceDate { get; set; }
        public string PolicyEffectiveDate { get; set; }
        public string PolicyExpiryDate { get; set; }
        public string PolicyFileUrl { get; set; }
    }
}
