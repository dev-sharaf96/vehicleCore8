using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services
{
  public  class WareefModel
    {
     [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public int? Id { set; get; }
        [JsonProperty("nameAr")]
        public string NameAr { set; get; }
        [JsonProperty("nameEn")]
        public string NameEn { set; get; }
        [JsonProperty("imageData")]
        public WareefImage ImageData { get; set; }
   
        [JsonProperty("category")]
        public CategoryData category { get; set; }

       
    }
}
