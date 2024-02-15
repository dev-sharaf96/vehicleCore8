namespace Tameenk.Core.Domain.Entities
{
    public class Invoice_Benefit : BaseEntity
    {
        public int Id { get; set; }

        public int? InvoiceId { get; set; }

        public short? BenefitId { get; set; }

        public decimal BenefitPrice { get; set; }

        public Benefit Benefit { get; set; }

        public Invoice Invoice { get; set; }
    }
}
