using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Dtos;
using TameenkDAL.Models;

namespace Tameenk.Services.IdentityApi.Models
{
    public class ProfileNotificationModel
    {
        /// <summary>
        /// User Notes
        /// </summary>
        [JsonProperty("userNotes")]
        public int Id { get; set; }

        /// <summary>
        /// User Notes
        /// </summary>
        [JsonProperty("userNotes")]
        public string UserId { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public int? TypeId { get; set; }
        public int? ModuleId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}