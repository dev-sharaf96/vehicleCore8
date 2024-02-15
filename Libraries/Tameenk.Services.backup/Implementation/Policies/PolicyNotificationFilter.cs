using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
  public  class PolicyNotificationFilter
    {
        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonProperty("referenceNo")]
        public string ReferenceNo { get; set; }

        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        [JsonProperty("insuranceCompanyId")]
        public int? InsuranceCompanyId { get; set; }

        [JsonProperty("methodName")]
        public string MethodName { get; set; }
    }
}
