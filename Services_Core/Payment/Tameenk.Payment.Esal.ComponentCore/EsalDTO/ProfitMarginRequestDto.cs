using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Payment.Esal.Component
{
    public class ProfitMarginRequestDto
    {
        //public string Description { get; set; }
        [JsonProperty("applied")]
        public Boolean Applied { get; set; }
        [JsonProperty("procedureArabic")]
        public string ProcedureArabic { get; set; }
        [JsonProperty("procedureEnglish")]
        public string procedureEnglish { get; set; }

    }
}
