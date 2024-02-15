using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Providers
{
    public class AddDriverResponse
    {
        public string ReferenceId { set; get; }
        public int StatusCode { set; get; }
        public decimal TotalAmount { set; get; }
        public decimal TaxableAmount { set; get; }
        public decimal VATAmount { set; get; }
        public List<Error> Errors { get; set; }
    }
}
