using System.Collections.Generic;

namespace Tameenk.Core.Domain.Entities
{
    public class PriceType : BaseEntity
    {
        public PriceType()
        {
            PriceDetails = new HashSet<PriceDetail>();
        }
        
        public byte Code { get; set; }
        
        public string EnglishDescription { get; set; }
        
        public string ArabicDescription { get; set; }

        public int Order { get; set; }

        public ICollection<PriceDetail> PriceDetails { get; set; }
    }
}
