using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class RenewalDiscount : BaseEntity
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int DiscountType { get; set; }
        public int CodeType { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Percentage { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public int? UserId { get; set; }
        public int? MessageType { get; set; }
        public bool IsActive { get; set; }
    }
}
