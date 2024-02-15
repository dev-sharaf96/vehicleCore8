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
    public class CheckoutDriverInfo : BaseEntity
    {
        public CheckoutDriverInfo(){}
        
        public int ID { get; set; }
        public string Nin { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string IBAN { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

        public bool? IsDeleted { get; set; } = false;
        public DateTime? DeletedDate { get; set; }
        public string DeletedBy { get; set; }
    }
}
