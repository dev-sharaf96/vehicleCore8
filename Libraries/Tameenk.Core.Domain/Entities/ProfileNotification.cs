using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    [Table("ProfileNotification")]
    public class ProfileNotification : BaseEntity
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public int? TypeId { get; set; }
        public int? TicketStatusId { get; set; }
        public int? ModuleId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
