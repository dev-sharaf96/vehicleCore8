using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Leasing
{
    public class LoginDataDecryptionModel
    {
        public string UserId { get; set; }
        public string BankName { get; set; }
        public string Email { get; set; }
        public string NationalId { get; set; }
        public string PhoneNo { get; set; }
    }
}
