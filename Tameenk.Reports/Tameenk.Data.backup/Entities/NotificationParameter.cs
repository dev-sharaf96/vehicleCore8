namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NotificationParameter")]
    public partial class NotificationParameter
    {
        public int Id { get; set; }

        [Required]
        [StringLength(512)]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }

        public int NotificationId { get; set; }

        public virtual Notification Notification { get; set; }
    }
}
