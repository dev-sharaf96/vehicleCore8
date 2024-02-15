using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Messages;
using Tameenk.Core.Domain.Enums.Messages;

namespace TameenkDAL.Models.Notifications
{
    public class NotificationModel
    {
        /// <summary>
        /// The identifier.
        /// </summary>
        public int Id { get; set; }
        
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
