using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services
{
    public class WareefDiscountBenefits
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("item")]
        public ItemData item { get; set; }

        [JsonProperty("discountValue")]
        public string DiscountValue { get; set; }

        [JsonProperty("wareefDiscountCode")]
        public string WDiscountCode { get; set; }

        [JsonProperty("category")]
        public CategoryData category { get; set; }
        
        [JsonProperty("wareefDiscountBenefits")]
        public List<WaeefDicscountItemsDetails> wareefDiscountBenefits { get; set; }

    }
    public class CategoryData
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class ItemData
    {
        public int id { get; set; }
        public string name { get; set; }   

    }
}
