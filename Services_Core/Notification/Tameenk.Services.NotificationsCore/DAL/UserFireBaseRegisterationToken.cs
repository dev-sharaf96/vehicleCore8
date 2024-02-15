using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Tameenk.Services.Notifications
{
    public class UserFireBaseRegisterationToken
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        [Required]
        public string RegisterationToken { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
    }
}
