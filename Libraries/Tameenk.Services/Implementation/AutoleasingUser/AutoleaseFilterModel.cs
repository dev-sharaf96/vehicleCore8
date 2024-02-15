using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services
{
    public class AutoleaseFilterModel
    {
        /// <summary>
        /// user Email
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// user PhoneNumber
        /// </summary>
        [JsonProperty("mobile")]
        public string PhoneNumber { get; set; }
    }
}
