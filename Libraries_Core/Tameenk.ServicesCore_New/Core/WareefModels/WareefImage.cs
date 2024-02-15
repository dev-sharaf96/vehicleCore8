using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services
{
    public class WareefImage
    {
        [JsonProperty("imageData")]
        public string ImageData { get; set; }

        [JsonProperty("newImageData")]
        public string NewImageData { get; set; }
    }
}
