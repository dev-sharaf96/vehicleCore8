using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tameenk.Core.Domain.Entities.Orders;
using Tameenk.Core.Domain.Entities.Payments;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;

namespace Tameenk.Core.Domain.Entities
{
    public class VehicleRequests : BaseEntity
    {
        public VehicleRequests(){}
        
        public int ID { get; set; }
        public string VehicleId { get; set; }
        public string DriverNin  { get; set; }
        public DateTime? CreatedDate { get; set; }
        public long CityId { get; set; }
    }
}
