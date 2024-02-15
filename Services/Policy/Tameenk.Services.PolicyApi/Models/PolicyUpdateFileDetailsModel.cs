using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Enums.Policies;

namespace Tameenk.Services.PolicyApi.Models
{
    /// <summary>
    /// Policy Update File Details Model
    /// </summary>
    [JsonObject("PolicyUpdateFileDetails")]
    public class PolicyUpdateFileDetailsModel
    {
        /// <summary>
        /// File name
        /// </summary>
        [JsonProperty("fileName")]
        public string FileName { get; set; }

        /// <summary>
        /// File as byte array
        /// </summary>
        [JsonProperty("fileByteArray")]
        public Byte[] FileByteArray { get; set; }

        /// <summary>
        /// Document type (ex: Policy, user id, Driver license ...)
        /// </summary>
        [JsonProperty("docType")]
        public PolicyUpdateRequestDocumentType DocType { get; set; }

        /// <summary>
        /// File mime type 
        /// </summary>
        [JsonProperty("fileMimeType")]
        public string FileMimeType { get; set; }

        

    }
}