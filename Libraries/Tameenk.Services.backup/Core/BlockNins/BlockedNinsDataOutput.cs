using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.BlockNins
{
   public  class BlockedNinsDataOutput
    {
        [JsonProperty("data")]
        public List<BlockedUsersDTO> Data { get; set; }
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        public byte[] ExcelSheet { get; set; }
    }
    public class BlockedUsersDTO
    {
        public int Id { get; set; }
        public string NationalId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string BlockReason { get; set; }
    }
}
