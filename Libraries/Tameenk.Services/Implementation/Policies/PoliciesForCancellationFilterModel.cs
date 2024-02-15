using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
   public class PoliciesForCancellationFilterModel
    {
        /// <summary>
        /// Vehicle Id
        /// </summary>
        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }

        /// <summary>
        /// National id ( NIN)
        /// </summary>
        [JsonProperty("nationalId")]
        public string NationalId { get; set; }

        /// <summary>
        /// policy No
        /// </summary>
        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }
        /// <summary>
        /// referenceId 
        /// </summary>
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

    }
}
