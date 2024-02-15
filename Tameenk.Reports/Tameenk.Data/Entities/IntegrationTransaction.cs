namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("IntegrationTransaction")]
    public partial class IntegrationTransaction
    {
        public int Id { get; set; }

        public Guid? MessageId { get; set; }

        [StringLength(200)]
        public string Method { get; set; }

        public string InputParams { get; set; }

        public string OutputResults { get; set; }

        public int? Status { get; set; }

        public DateTime? TransactionDate { get; set; }
    }
}
