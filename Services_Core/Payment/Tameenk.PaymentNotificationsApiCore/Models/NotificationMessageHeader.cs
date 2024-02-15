using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Tameenk.PaymentNotificationsApi.Models
{
    [DataContract(Name = "Header", Namespace = "")]
    public class NotificationMessageHeader
    {
        [Required(AllowEmptyStrings = false)]
        [DataMember(Order = 2)]
        public string Sender { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DataMember(Order = 3)]
        public string Receiver { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DataMember(Order = 4)]
        public string MessageType { get; set; }

        [DataMember(Order = 5)]
        public DateTime TimeStamp { get; set; }
    }
}