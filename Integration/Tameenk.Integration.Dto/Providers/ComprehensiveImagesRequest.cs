using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Providers
{
    public class ComprehensiveImagesRequest
    {
        public string ReferenceId { get; set; }
        [JsonProperty("InsuredId")]
        public string Nin { get; set; }
        public List<ComprehensiveImage> Attachments { get; set; }
    }
}
