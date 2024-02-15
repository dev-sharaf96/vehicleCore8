using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.IVR
{
    public class UserModel
    {
        [JsonProperty("nationalId")]
        public string NationalId { get; set; }

        [JsonProperty("nameAr")]
        public string NameAr { get; set; }

        [JsonProperty("nameEn")]
        public string NameEn { get; set; }
    }
}
