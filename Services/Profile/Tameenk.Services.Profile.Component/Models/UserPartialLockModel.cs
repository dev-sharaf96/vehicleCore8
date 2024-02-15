using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Profile.Component
{
    public class UserPartialLockModel
    {
        /// <summary>
        /// GUID generated first time when user try to login
        /// </summary>
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        /// <summary>
        /// represent hashed of the compination (SessionId_SharedSecret_UserIp)
        /// </summary>
        [JsonProperty("hashed")]
        public string Hashed { get; set; }

        /// <summary>
        /// represent the number of error times that user try to hit the api
        /// </summary>
        [JsonProperty("errorTimesUserTries")]
        public int ErrorTimesUserTries { get; set; }

        /// <summary>
        /// this to indicate that user is locked
        /// </summary>
        [JsonProperty("isLocked")]
        public bool IsLocked { get; set; }

        /// <summary>
        /// this to indicate locked end date
        /// </summary>
        [JsonProperty("lockDueDate")]
        public DateTime LockDueDate { get; set; }


        [JsonIgnore]
        public string LogDescription { get; set; }

        [JsonIgnore]
        public string ErrorDescription { get; set; }
    }
}
