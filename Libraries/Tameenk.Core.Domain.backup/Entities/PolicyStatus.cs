using System.Collections.Generic;

namespace Tameenk.Core.Domain.Entities
{
    public class PolicyStatus : BaseEntity
    {
        public PolicyStatus()
        {
            CheckoutDetails = new HashSet<CheckoutDetail>();
        }

        public int Id { get; set; }
        
        public string Key { get; set; }
        
        public string NameAr { get; set; }

        public string NameEn { get; set; }

        public ICollection<CheckoutDetail> CheckoutDetails { get; set; }
    }
}
