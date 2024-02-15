using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services
{
   public  class WareefCategoryModel
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public int? Id { set; get; }
        [JsonProperty("nameAr")]
        public string NameAr { set; get; }
        [JsonProperty("nameEn")]
        public string NameEn { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
    }
}