using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies.leasingportal
{
   public class ClientDataModel
    {
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }
        [JsonProperty("externalId")]
        public string ExternalId { get; set; }
        [JsonProperty("parentExternalId")]
        public string ParentExternalId { get; set; }
        [JsonProperty("parentReferenceId")]
        public string ParentReferenceId { get; set; }
    }
}
