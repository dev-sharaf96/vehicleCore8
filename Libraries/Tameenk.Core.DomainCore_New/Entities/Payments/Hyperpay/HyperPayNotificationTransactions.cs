using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Core.Domain
{
    public class HyperPayNotificationTransactions : BaseEntity
    {
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UniqueId { get; set; }
        public decimal PayoutTransferAmount { get; set; }
        public string MerchantTransactionId { get; set; }
        public int NotificationId { get; set; }
        public virtual HyperPayNotification Notification { get; set; }
    }
}
