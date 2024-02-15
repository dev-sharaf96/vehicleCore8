using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Dtos.Profile
{
    public class InvoiceModel
    {
        public int Id { get; set; }
        public int InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal? TotalPrice { get; set; }
        public string UserEmail { get; set; }
        public string InsuranceCompanyNameAR { get; set; }
        public string InsuranceCompanyNameEN { get; set; }
        public string TaxInvoiceNumber { get; set; }
    }
}
