using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    [Table("UserTicketStatus")]
    public class UserTicketStatus: BaseEntity
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string StatusNameAr { get; set; }

        [StringLength(50)]
        public string StatusNameEn { get; set; }
    }
}
