using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Providers
{
    public class QuotationServiceResponse
    {
        public string ReferenceId { get; set; }
        public int StatusCode { get; set; }
        public List<Error> Errors { get; set; }
        public string QuotationNo { get; set; }
        public string QuotationDate { get; set; }
        public string QuotationExpiryDate { get; set; }
        public List<ProductDto> Products { get; set; }
    }
}
