using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Services
{
    public class CorporateFilterModel : BaseViewModel
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

        /// <summary>
        /// Account Id
        /// </summary>
        [JsonProperty("accountId")]
        public int? AccountId { get; set; }
    }
}
