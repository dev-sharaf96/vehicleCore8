namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DriverViolation")]
    public partial class DriverViolation
    {
        public int Id { get; set; }

        public Guid DriverId { get; set; }

        public int ViolationId { get; set; }

        public virtual Driver Driver { get; set; }
    }
}
