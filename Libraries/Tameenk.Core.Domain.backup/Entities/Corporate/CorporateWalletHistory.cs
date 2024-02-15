using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class CorporateWalletHistory : BaseEntity
    {
        public int Id { get; set; }
        public int? CorporateAccountId { get; set; }
        public string ReferenceId { get; set; }
        public decimal? Amount { get; set; }
        public string MethodName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}