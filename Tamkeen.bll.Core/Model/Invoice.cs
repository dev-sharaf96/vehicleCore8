using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamkeen.bll.Model
{
    public class Invoice
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TotalAmount { get; set; }
        public int PaymentMethodId { get; set; }
        public int PaymentId { get; set; }

    }
}
