using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Providers
{
    public class AlamiaQuotationServiceResponse
    {
        public string ReferenceId { get; set; }
        public int StatusCode { get; set; }
        public List<Error> errors { get; set; }
        public string QuotationNo { get; set; }
        public string QuotationDate { get; set; }
        public string QuotationExpiryDate { get; set; }
        public ProductDto Products { get; set; }
    }

    public class AlamiaQuotationResponseWithErrorObject
    {
        public string ReferenceId { get; set; }
        public int StatusCode { get; set; }
        public Error errors { get; set; }
        public string QuotationNo { get; set; }
        public string QuotationDate { get; set; }
        public string QuotationExpiryDate { get; set; }
        public ProductDto Products { get; set; }
    }
    public class AlamiaQuotationResponseWithErrorObjectComp
    {
        public string ReferenceId { get; set; }
        public int StatusCode { get; set; }
        public Error errors { get; set; }
        public string QuotationNo { get; set; }
        public string QuotationDate { get; set; }
        public string QuotationExpiryDate { get; set; }
        public List<ProductDto> Products { get; set; }
    }
    public class AlamiaQuotationServiceResponseComp
    {
        public string ReferenceId { get; set; }
        public int StatusCode { get; set; }
        public List<Error> errors { get; set; }
        public string QuotationNo { get; set; }
        public string QuotationDate { get; set; }
        public string QuotationExpiryDate { get; set; }
        public List<ProductDto> Products { get; set; }
    }

}
