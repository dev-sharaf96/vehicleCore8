using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Tameenk.PaymentNotificationsApi.Models
{
    [DataContract(Name = "Message", Namespace = "")]
    public class NotificationMessage
    {
        [Required]
        [DataMember(Order = 1)]
        public virtual NotificationMessageHeader Header { get; set; }

        [Required]
        [DataMember(Order = 7)]
        public virtual NotificationMessageBody Body { get; set; }

    }
}