using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class VehicleDiscounts : BaseEntity
    {
        public int Id { get; set; }
        public string Nin { get; set; }
        public Guid? VehicleId { get; set; }
        public string DiscountCode { get; set; }
        public string SequenceNumber { get; set; }
        public string CustomCardNumber { get; set; }
        public bool? IsUsed { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string PreviousReferenceId { get; set; }
        public string ReferenceId { get; set; }
    }
}
