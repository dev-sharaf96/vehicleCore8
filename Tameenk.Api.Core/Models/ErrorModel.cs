using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Api.Core.Models
{
    /// <summary>
    /// Class for error info.
    /// </summary>
    [JsonObject("error")]
    public class ErrorModel
    {
        #region Ctor
        public ErrorModel(string description = null, string code = null)
        {
            Description = description;
            Code = code;
        }

        #endregion

        /// <summary>
        /// Get or set the error code.
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// Get or set the error description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

    }
}