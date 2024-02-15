using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services
{
    public class CorporateAccountFilter
    {
        /// <summary>
        /// corporate name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
