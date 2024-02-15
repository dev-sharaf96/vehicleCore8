using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Quotations
{
    public class AutoleasingQuotationForm : BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ExternalId { get; set; }
        public int? BankId { get; set; }
        public string BankName { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string FilePath { get; set; }
    }
}
