using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Providers
{
    [JsonObject("Attachment")]
    public class ComprehensiveImage
    {
        public int AttachmentCode { get; set; }
        public byte[] AttachmentFile { get; set; }
    }
}
