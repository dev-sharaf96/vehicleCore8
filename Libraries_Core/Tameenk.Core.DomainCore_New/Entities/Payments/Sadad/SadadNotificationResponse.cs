using System;

namespace Tameenk.Core.Domain.Entities.Payments.Sadad
{
    public class SadadNotificationResponse : BaseEntity
    {
        public int ID { get; set; }

        public int NotificationMessageId { get; set; }
        
        public string HeadersReceiver { get; set; }
        
        public string HeadersSender { get; set; }
        
        public string HeadersMessageType { get; set; }

        public DateTime? HeadersTimeStamp { get; set; }
        
        public string Status { get; set; }

        public SadadNotificationMessage SadadNotificationMessage { get; set; }
    }
}
