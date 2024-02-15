using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto
{

    public class Template
    {
        public Template()
        {
            this.Whatsapp = new Whatsapp();
        }
        [JsonProperty("whatsapp")]
        public Whatsapp Whatsapp { get; set; }
    }

    public class Parameters
    {
        public Parameters()
        {
            //this.Media = new Media();
        }
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }

        [JsonProperty("media", NullValueHandling = NullValueHandling.Ignore)]
        public Media Media { get; set; }
    }
    public class Components
    {
        public Components()
        {
            //this.Parameters = new Parameters[1];
        }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("parameters")]
        public Parameters[] Parameters { get; set; }
    }
    public class Whatsapp
    {
        public Whatsapp()
        {
            //this.Components = new Components[1];
            this.Language = new Language();
        }
        [JsonProperty("namespace")]
        public string Namespace { get; set; }
        [JsonProperty("element_name")]
        public string ElementName { get; set; }
        [JsonProperty("language")]
        public Language Language { get; set; }
        [JsonProperty("components")]
        public Components[] Components { get; set; }
    }
    public class Language
    {
        [JsonProperty("policy")]
        public string Policy { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
