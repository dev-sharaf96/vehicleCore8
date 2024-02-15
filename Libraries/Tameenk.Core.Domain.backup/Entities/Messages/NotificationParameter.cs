namespace Tameenk.Core.Domain.Entities.Messages
{
    /// <summary>
    /// Represent the notification parameters to extend the notifiction table.
    /// </summary>
    public class NotificationParameter : BaseEntity
    {
        /// <summary>
        /// The identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The notification parameter name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The notification parameter value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The related notification identifier.
        /// </summary>
        public int NotificationId { get; set; }

        /// <summary>
        /// The related notification.
        /// </summary>
        public Notification Notification { get; set; }
    }
}
