using System.Collections.Generic;

namespace Tameenk.Core.Domain.Entities
{
    public class BankNins : BaseEntity
    {

        public int Id { get; set; }

        public int BankId { get; set; }
        public string NIN { get; set; }
    }
}
