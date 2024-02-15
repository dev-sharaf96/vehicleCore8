using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class AutoleasingAgencyRepairHistory : BaseEntity
    {
        public int Id { get; set; }
        public int BankId { get; set; }
        public string ExternalId { get; set; }
        public string FirstYear { get; set; }
        public string SecondYear { get; set; }
        public string ThirdYear { get; set; }
        public string FourthYear { get; set; }
        public string FifthYear { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
