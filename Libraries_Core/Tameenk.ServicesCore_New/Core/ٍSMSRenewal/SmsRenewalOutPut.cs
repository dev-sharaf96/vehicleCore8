using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Loggin.DAL.Dtos;

namespace Tameenk.Services.Core.SMSRenewal
{
   public class SmsRenewalOutPut
    {

        [JsonProperty("result")]
        public AllTypeSMSRenewalLogModel Result { get; set; }

        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        public byte[] ExcelSheet { get; set; }
    }
}
