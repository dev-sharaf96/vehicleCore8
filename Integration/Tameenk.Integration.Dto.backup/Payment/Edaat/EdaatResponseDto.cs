using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Dtos.Edaat
{
    public class EdaatResponseDto
    {
        public Status Status { set; get; }
        public Body Body { set; get; }

    }
    public class Status
    {
        public bool Success { set; get; }
        public string Code { set; get; }
        public List<string> Message { set; get; }
    }
    public class Body
    {
        public string InvoiceNo { set; get; }
        public string InternalCode { set; get; }
    }
}
