using System;
namespace Tameenk.Core.Domain.Entities
{
    public class PriceDetail : BaseEntity
    {
        public Guid DetailId { get; set; }

        public Guid ProductID { get; set; }

        public byte PriceTypeCode { get; set; }

        public decimal PriceValue { get; set; }

        public decimal? PercentageValue { get; set; }
        public bool IsCheckedOut { get; set; }
        public DateTime CreateDateTime { get; set; }

        public PriceType PriceType { get; set; }

        public Product Product { get; set; }
    }
}
