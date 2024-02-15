using System.Collections.Generic;
using Tameenk.Core.Domain.Entities.Quotations;

namespace Tameenk.Core.Domain.Entities
{
    public class City : BaseEntity, ILookupTable
    {
        public City()
        {
            QuotationRequests = new HashSet<QuotationRequest>();
        }
        
        public long Code { get; set; }
        
        public string EnglishDescription { get; set; }
        
        public string ArabicDescription { get; set; }

        public int? RegionId { get; set; }

        public Region Region { get; set; }
        public long YakeenCode { get; set; }

        public ICollection<QuotationRequest> QuotationRequests { get; set; }
    }
}
