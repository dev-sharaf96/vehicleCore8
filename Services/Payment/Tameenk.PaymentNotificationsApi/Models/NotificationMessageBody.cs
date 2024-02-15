using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Tameenk.PaymentNotificationsApi.Models
{
    [DataContract(Name = "Body", Namespace = "")]
    public class NotificationMessageBody
    {
        [Required(AllowEmptyStrings = false)]
        [DataMember(Order = 8)]
        public string Description { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DataMember(Order = 9)]
        public string AccountNo { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DataMember(Order = 10)]
        public string Amount { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DataMember(Order = 11)]
        public string CustomerRefNo { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DataMember(Order = 12)]
        public string TransType { get; set; }
    }
}