namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PolicyUpdatePayment")]
    public partial class PolicyUpdatePayment
    {
        public int Id { get; set; }

        public int PolicyUpdateRequestId { get; set; }

        public decimal? Amount { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual PolicyUpdateRequest PolicyUpdateRequest { get; set; }
    }
}
