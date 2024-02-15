using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Orders
{
    public class CheckoutDetailInfo
    {
        public string Email { get; set; }

        public string Phone { get; set; }
        public string IBAN { get; set; }
        public string NIN { get; set; }
        public int? PolicyStatusId { get; set; }

        //public bool IsYakeenVerified { get; set; }
    }
}
