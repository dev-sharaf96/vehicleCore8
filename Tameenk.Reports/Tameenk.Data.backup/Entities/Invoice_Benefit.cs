namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Invoice_Benefit
    {
        public int Id { get; set; }

        public int? InvoiceId { get; set; }

        public short? BenefitId { get; set; }

        public decimal? BenefitPrice { get; set; }

        public virtual Benefit Benefit { get; set; }

        public virtual Invoice Invoice { get; set; }
    }
}
