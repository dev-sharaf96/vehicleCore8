using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services
{
    public class DeleteDiscountBenefitModel
    {
        [JsonProperty("discountId")]
        public int discountId { get; set; }

    }
}
