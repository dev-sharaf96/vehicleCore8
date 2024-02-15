using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Enums.Messages;

namespace Tameenk.Services.PolicyApi.Models
{
    /// <summary>
    /// Represent The notification model .
    /// </summary>
    [JsonObject("notification")]
    public class NotificationModel
    {
        /// <summary>
        /// The identifier.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// The notification group.
        /// </summary>
        [JsonProperty("group")]
        public string Group { get; set; }

        /// <summary>
        /// The related item to notification group identifier.
        /// </summary>
        [JsonProperty("groupReferenceId")]
        public string GroupReferenceId { get; set; }

        /// <summary>
        /// The notification type identifier.
        /// </summary>
        [JsonProperty("typeId")]
        public int TypeId { get; set; }


        /// <summary>
        /// The notification type name identifier.
        /// </summary>
        [JsonProperty("typeName")]
        public string TypeName { get; set; }

         /// <summary>
        /// The notification type.
        /// </summary>
        [JsonProperty("type")]
        public NotificationType Type
        {
            get
            {
                return (NotificationType)TypeId;
            }
            set { TypeId = (int)value;
                TypeName = value.ToString();
  //value.ToString().First().ToString().ToUpper() + value.ToString().Substring(1); value.ToString();
            }
        }
        

        /// <summary>
        /// The notification extended parameters.
        /// </summary>
        [JsonProperty("parameters")]
        public ICollection<NotificationParameterModel> Parameters { get; set; }

        /// <summary>
        /// The created date  of this notification.
        /// </summary>
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The status identifier.
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }
        
    }
}