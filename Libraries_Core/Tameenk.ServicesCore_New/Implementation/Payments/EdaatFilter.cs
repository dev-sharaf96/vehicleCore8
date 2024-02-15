using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Payments
{

    public class EdaatFilter
    {
        public string ReferenceId { get; set; }         
        public string InvoiceNumber { get; set; }
        public bool? Export { get; set; }
    }
}
