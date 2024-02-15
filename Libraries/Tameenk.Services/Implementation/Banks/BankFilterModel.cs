using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Banks
{
    public class BankFilterModel
    {
        [JsonProperty("bankName")]
        public string BankName { get; set; }
    }
}
