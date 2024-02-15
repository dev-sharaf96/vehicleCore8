using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class WataniyaDraftPolicy : BaseEntity
    {
        public int Id { get; set; }
        public string ReferenceId { get; set; }
        public string ReferenceNumber { get; set; }
        public string QuotationNumber { get; set; }
        public string PolicyNo { get; set; }
        public string Channel { get; set; }
        public string Method { get; set; }
        public string UserName { get; set; }
        public Guid? UserID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string PolicyEffectiveDate { get; set; }
        public string PolicyExpiryDate { get; set; }
    }
}
