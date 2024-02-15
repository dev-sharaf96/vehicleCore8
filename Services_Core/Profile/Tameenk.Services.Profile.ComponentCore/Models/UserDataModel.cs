using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Profile.Component.Models
{
    public class UserDataModel
    {
        [JsonProperty("iS_FullName_Exist")]
        public bool IsExist { get; set; }

        [JsonProperty("aR_FullName")]
        public string FullNameAr { get; set; }

        [JsonProperty("en_FullName")]
        public string FullNameEn { get; set; }
    }
}
