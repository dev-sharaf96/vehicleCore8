using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tameenk.Core.Domain.Entities.Orders;
using Tameenk.Core.Domain.Entities.Payments;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;

namespace Tameenk.Core.Domain.Entities
{
    public class AutoleasingQuotationResponseCache : BaseEntity
    {
        public AutoleasingQuotationResponseCache(){}
        public int ID { get; set; }
        public int InsuranceCompanyId { get; set; }
        public bool? VehicleAgencyRepair { get; set; }
        public int? DeductibleValue { get; set; }
        public string ExternalId { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public string QuotationResponse { get; set; }
        public Guid UserId { get; set; }
    }
}
