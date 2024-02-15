using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Services.Administration.Identity.Core.Domain
{
    [Table("UserPages")]
    public class UserPage
    {
        public int UserId { get; set; }
        public int PageId { get; set; }

        public virtual Page Page { get; set; }
        public virtual AppUser User { get; set; }
    }
}
