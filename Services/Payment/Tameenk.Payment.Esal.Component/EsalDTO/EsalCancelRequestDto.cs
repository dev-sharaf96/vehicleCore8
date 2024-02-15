
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Payment.Esal.Component
{
    public class EsalCancelRequestDto
    {
        public string supplierId { get; set; }
        public List<InvoiceCanceledDto> invoices { get; set; }
    }
}
