using System;
using System.Collections.Generic;
using Tameenk.Core.Domain.Enums.Messages;

namespace Tameenk.Core.Domain.Entities.Messages
{
    /// <summary>
    /// Represent the system notification.
    /// </summary>
    public class Notification : BaseEntity
    {
        /// <summary>
        /// The contructor.
        /// </summary>
        public Notification()
        {
            Parameters = new HashSet<NotificationParameter>();
        }
        /// <summary>
        /// The identifier.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// The notification group.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// The related item to notification group identifier.
        /// </summary>
        public string GroupReferenceId { get; set; }
        
        /// <summary>
        /// The notification type identifier.
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// The notification extended parameters.
        /// </summary>
        public ICollection<NotificationParameter> Parameters { get; set; }

        /// <summary>
        /// The created date of this notification.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The status identifier.
        /// </summary>
        public int StatusId { get; set; }

        /// <summary>
        /// The notifiation status.
        /// </summary>
        public NotificationStatus Status {
            get {
                return (NotificationStatus)StatusId;
            }
            set { StatusId = (int)value; }
        }

        /// <summary>
        /// The notification type.
        /// </summary>
        public NotificationType Type {
            get {
                return (NotificationType)TypeId;
            }
            set { TypeId = (int)value; }
        }

    }
}
