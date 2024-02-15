using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Policies
{
    public class EndorsmentBenefit : BaseEntity
    {
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ReferenceId { set; get; }
        public int? EndorsmentId { set; get; }
        public string QuotationReferenceId { get; set; }
        public string BenefitId { get; set; }
    }
}
