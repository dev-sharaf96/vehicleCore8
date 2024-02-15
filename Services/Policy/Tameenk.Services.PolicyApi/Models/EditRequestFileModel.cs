using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Enums.Policies;

namespace Tameenk.Services.PolicyApi.Models
{
    /// <summary>
    /// Edit Request File Model
    /// </summary>
    [JsonObject("EditRequestFile")]
    public class EditRequestFileModel
    {
        /// <summary>
        /// Policy Id
        /// </summary>
        [JsonProperty("policyId")]
       public int PolicyId { get; set; }

        /// <summary>
        /// policy update file details
        /// </summary>
        [JsonProperty("policyUpdateFileDetails")]
        public  ICollection<PolicyUpdateFileDetailsModel> PolicyUpdateFileDetails { get; set; }

        /// <summary>
        /// Edit Request type such as ( FixPolicyError , ChangeLicense , .. etc )
        /// </summary>
        [JsonProperty("editRequestType")]
        public PolicyUpdateRequestType EditRequestType { get; set; }

    }
}