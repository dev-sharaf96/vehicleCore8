using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    [Table("UserTicketType")]
    public class UserTicketType : BaseEntity
    {
        public int Id { get; set; }

        public string TicketTypeNameAr { get; set; }

        public string TicketTypeNameEn { get; set; }

        public int OrderNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
