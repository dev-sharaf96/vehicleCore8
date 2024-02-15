using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// file model as HTTP Posted File Base
    /// </summary>
    [JsonObject("fileModel")]
    public class FileModel
    {
        /// <summary>
        /// file content
        /// </summary>
        [JsonProperty("content")]
        public byte[] Content { get; set; }

        /// <summary>
        /// file name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// file size
        /// </summary>
        [JsonProperty("size")]
        public long Size { get; set; }

        /// <summary>
        /// file extension
        /// </summary>
        [JsonProperty("extension")]
        public string Extension { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }
    }
}