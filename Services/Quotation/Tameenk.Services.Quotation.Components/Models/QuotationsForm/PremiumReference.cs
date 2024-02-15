using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Quotation.Components
{
    public class PremiumReference
    {
        public string ReferenceId { get; set; }
        public Decimal? Premium { get; set; }
        public decimal BasicPremium { get; set; }
        public string VehicleRepairType { get; set; }
        public decimal VAT { get; internal set; }
        public decimal? InsurancePercentage { get; internal set; }
        public decimal? OtherCodesAndBenifits { get; internal set; }
    }
}
