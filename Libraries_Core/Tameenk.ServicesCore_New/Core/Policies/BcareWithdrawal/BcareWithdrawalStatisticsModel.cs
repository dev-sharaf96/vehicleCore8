using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Policies
{
    public class BcareWithdrawalStatisticsModel
    {
        [JsonProperty("tPLNumber")]
        public int TPLNumber { get; set; }

        [JsonProperty("compNumber")]
        public int CompNumber { get; set; }

        [JsonProperty("registerNumber")]
        public int RegisterNumber { get; set; }
    }
}
