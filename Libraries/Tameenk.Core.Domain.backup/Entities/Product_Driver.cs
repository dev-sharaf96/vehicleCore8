using System;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Core.Domain.Entities
{
    public class Product_Driver : BaseEntity
    {
        public int Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid DriverId { get; set; }
        public decimal DriverPrice { get; set; }
        public string DriverExternalId { get; set; }
        public Driver Driver { get; set; }
        public Product Product { get; set; }
    }
}
