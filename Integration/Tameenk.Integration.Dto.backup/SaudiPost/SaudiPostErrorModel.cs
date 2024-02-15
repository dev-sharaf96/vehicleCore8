using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.SaudiPost
{
    public class SaudiPostErrorModel
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
