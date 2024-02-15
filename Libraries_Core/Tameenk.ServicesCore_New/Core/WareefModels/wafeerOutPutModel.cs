using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services
{
    public class wafeerOutPutModel
    { 
        [JsonProperty("nameAr")]
        public string NameAr { set; get; }

        [JsonProperty("nameEn")]
        public string NameEn { get; set; }

        [JsonProperty("imageBytes")]
        public string ImageBytes { get; set; }

        [JsonProperty("wareefCategoryId")]
        public int WareefCategoryId { get; set; }

        [JsonProperty("categoryAR")]
        public string CategoryAR { get; set; }

        [JsonProperty("categoryEN")]
        public string CategoryEN { get; set; }

        [JsonProperty("benefitsDetails")]

        public List<Benefits> BenefitsDetails { get; set; }

        [JsonProperty("ItemCategory")]

        public Category ItemCategory { get; set; }
        public class Category
        {
            [JsonProperty("nameAr")]
            public string NameAr { get; set; }

            [JsonProperty("nameEn")]
            public string NameEn { get; set; }
        }
        public class Benefits
        {
            [JsonProperty("benefitDescriptionAr")]
            public string BenefitDescriptionAr { get; set; }

            [JsonProperty("benefitDescriptionEn")]
            public string BenefitDescriptionEn { get; set; }
            [JsonProperty("discoutType")]
            public string DiscoutType { get; set; }

            [JsonProperty("discountValue")]
            public string DiscountValue { get; set; }
        }
       
    }
}
