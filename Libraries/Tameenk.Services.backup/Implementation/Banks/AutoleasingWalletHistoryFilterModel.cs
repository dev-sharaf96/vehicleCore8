using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Tameenk.Services.Implementation.Banks
{
  public  class AutoleasingWalletHistoryFilterModel
    {
        [JsonProperty("insuranceCompanyId")]
        public int? InsuranceCompanyId { get; set; }
        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }
        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("Language")]
        public string Language { get; set; }
        [JsonProperty("isExcel")]
        public bool IsExcel { get; set; }
      

    }
}
