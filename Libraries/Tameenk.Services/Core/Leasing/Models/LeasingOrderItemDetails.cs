using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Integration.Dto.Providers;

namespace Tameenk.Services.Core
{
    public class LeasingOrderItemDetails
    {
        public int Id { get; set; }
        public string ReferenceId { get; set; }
        public Guid ProductId { get; set; }
        public decimal Price { get; set; }

        public List<AdditionalBenefit> Benefits { get; set; }

        public AdditionalDriver Driver { get; set; }
    }

    public class AdditionalDriver
    {
        public string ReferenceId { get; set; }
        public decimal PaymentAmount { get; set; }
    }
}
