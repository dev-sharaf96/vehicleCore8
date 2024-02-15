using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Promotions
{
    public class EnrollUserToProgramModel
    {
        [JsonProperty("userEndrollerd")]
        public bool UserEndrollerd { get; set; }

        [JsonProperty("errors")]
        public List<ErrorModel> Errors { get; set; }

        public string Key { get; set; }
    }

    public class ErrorModel
    {

        /// <summary>
        /// Get or set the error code.
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// Get or set the error description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
