using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Enums;

namespace Tameenk.Services.Core.Leasing.Models
{
    public class ClaimsUpdateModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// claimStatusId
        /// </summary>
        [JsonProperty("claimStatusId")]
        public int ClaimStatusId { get; set; }

        /// <summary>
        /// notes
        /// </summary>
        [JsonProperty("notes")]
        public string notes { get; set; }

        /// <summary>
        /// File Bytes
        /// </summary>
        [JsonProperty("fileBytes")]
        public byte[] FileBytes { get; set; }

        /// <summary>
        /// File Name
        /// </summary>
        [JsonProperty("fileName")]
        public string FileName { get; set; }

        /// <summary>
        /// File Extension
        /// </summary>
        [JsonProperty("fileExtension")]
        public string FileExtension { get; set; }
    }
}
