using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Integration.Dto.Providers;

namespace Tameenk.Integration.Providers.Tawuniya.Dtos
{
    public class AutoLeaseQuotationResponse
    {
        [JsonProperty("Status")]
        public string Status{ get; set; }
       
        [JsonProperty("errors")]
        public List<Error> Errors { get; set; }
    }


    //public class Error
    //{
    //    [JsonProperty("field")]
    //    public string Field { get; set; }
    //    [JsonProperty("message")]
    //    public string Message { get; set; }
    //    [JsonProperty("code")]
    //    public int Code { get; set; }
        
    //}
}
