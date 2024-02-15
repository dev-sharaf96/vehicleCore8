namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TawuniyaTempTable")]
    public partial class TawuniyaTempTable
    {
        public int Id { get; set; }

        public string QtServiceRequestMessage { get; set; }

        public string PorposalResponse { get; set; }

        [StringLength(200)]
        public string ReferenceId { get; set; }
    }
}
