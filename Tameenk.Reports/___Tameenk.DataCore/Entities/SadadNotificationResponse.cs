namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SadadNotificationResponse")]
    public partial class SadadNotificationResponse
    {
        public int ID { get; set; }

        public int NotificationMessageId { get; set; }

        [StringLength(50)]
        public string HeadersReceiver { get; set; }

        [StringLength(50)]
        public string HeadersSender { get; set; }

        [StringLength(10)]
        public string HeadersMessageType { get; set; }

        public DateTime? HeadersTimeStamp { get; set; }

        [Required]
        [StringLength(10)]
        public string Status { get; set; }

        public virtual SadadNotificationMessage SadadNotificationMessage { get; set; }
    }
}
