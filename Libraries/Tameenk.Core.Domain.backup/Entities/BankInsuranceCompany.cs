using System.Collections.Generic;

namespace Tameenk.Core.Domain.Entities
{
    public class BankInsuranceCompany : BaseEntity
    {
        public int Id { get; set; }
        public int BankId { get; set; }
        public int CompanyId { get; set; }
        public bool IsActive { get; set; }
    }
}
