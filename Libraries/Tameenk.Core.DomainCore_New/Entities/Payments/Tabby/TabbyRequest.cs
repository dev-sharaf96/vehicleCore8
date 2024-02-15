using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Payments.Tabby
{
    public class TabbyRequest: BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        public string UserId { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string RefrenceId { get; set; }
        public string Channel { get; set; }
        public double? InsuranceCompanyAmount { get; set; }
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }

    }
}
