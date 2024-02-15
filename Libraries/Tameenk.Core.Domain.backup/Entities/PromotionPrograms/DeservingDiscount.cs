using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.PromotionPrograms
{
    public class DeservingDiscount : BaseEntity
    {
        public int Id { get; set; }
        public string NationalId { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
