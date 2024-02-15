using System;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Core.Domain.Entities
{
    public class CheckoutAdditionalDriver : BaseEntity
    {
        public string CheckoutDetailsId { get; set; }

        public Guid DriverId { get; set; }

        public int Id { get; set; }

        public CheckoutDetail CheckoutDetail { get; set; }

        public Driver Driver { get; set; }
    }
}
