using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Payments
{
    public class PaymentAdmin
    {
        public DateTime? InvoiceDate { get; set; }

        public int? InvoiceNo { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }


        public string Email { get; set; }


        public string InsuranceCompany { get; set; }

        public string PolicyNo { get; set; }

        public string PaymentMethod { get; set; }

        public string CardNumber { get; set; }


        public string InsuranceType { get; set; }

        public decimal? ExtraPremiumPrice { get; set; }

        public decimal? Vat { get; set; }


        public string ReferenceId { get; set; }


        public int? PaymentMethodId { get; set; }

        public Guid? MerchantId { get; set; }

    }
}
