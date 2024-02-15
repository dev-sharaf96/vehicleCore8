using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Generic.Components.Output
{
    public class WinnersModel
    {
        [JsonProperty("currentWeekNumber")]
        public int CurrentWeekNumber { get; set; }

        [JsonProperty("1compList")]
        public List<WinnerCategory> CompList { get; set; }

        [JsonProperty("2tPLList")]
        public List<WinnerCategory> TPLList { get; set; }

        [JsonProperty("3registerList")]
        public List<WinnerCategory> RegisterList { get; set; }
    }

    public class WinnerCategory
    {
        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("englishName")]
        public string EnglishName { get; set; }

        [JsonProperty("arabicName")]
        public string ArabicName { get; set; }

        [JsonProperty("prizeType")]
        public int PrizeType { get; set; }
    }
}
