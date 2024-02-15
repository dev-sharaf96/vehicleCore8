using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
  public  class AutoleasingRenewalPolicyStatistics : BaseEntity
    {
        public int Id { get; set; }
        public string SequenceNumber { get; set; }
        public string ExternalId { get; set; }
        public string ParentExternalId { get; set; }
        public int? Year { get; set; }
        public decimal? PaymentAmount  { get; set; }
        public string UserId { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ReferenceId { get; set; }
        public string ParentReferenceId { get; set; }
        public int ? InsurancePercentage { get; set; }
    }
}
