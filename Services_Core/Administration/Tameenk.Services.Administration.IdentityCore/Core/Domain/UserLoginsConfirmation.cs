using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Administration.Identity.Core.Domain
{
    [Table("UserLoginsPhoneConfirmation")]
    public class UserLoginsConfirmation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ConfirmationCode { get; set; }
        public bool IsCodeConfirmed { get; set; }
        public DateTime CodeExpiryDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
