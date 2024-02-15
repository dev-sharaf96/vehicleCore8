using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Tameenk.PaymentNotificationsApi.Models
{
    [DataContract(Name = "Message", Namespace = "")]
    public class ResponseMessage
    {
        [DataMember(Order = 1)]
        public virtual NotificationMessageHeader Header { get; set; }
        [DataMember(Order = 2)]
        public string Status { get; set; }
    }
}