using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services
{
    public class DeleteCategoryModel
    {
        [JsonProperty("categoryId")]
        public int categoryId { set; get; }
    }
}
