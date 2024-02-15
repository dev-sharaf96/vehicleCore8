namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SadadResponse")]
    public partial class SadadResponse
    {
        public int Id { get; set; }

        public int SadadRequestId { get; set; }

        [Required]
        [StringLength(10)]
        public string Status { get; set; }

        public int ErrorCode { get; set; }

        [Required]
        public string Description { get; set; }

        public int TrackingId { get; set; }

        public virtual SadadRequest SadadRequest { get; set; }
    }
}
