using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class AutoleasingInitialQuotationCompanies : BaseEntity
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public int CompanyId { get; set; }
        public int BankId { get; set; }
        public string UserId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
