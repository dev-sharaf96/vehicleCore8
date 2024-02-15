using System;
using System.Collections.Generic;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Core.Domain.Entities.Quotations
{
    public class QuotationShares : BaseEntity
    {
        public int Id { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string ExternalId { get; set; }
        public string UserId { get; set; }
        public string Channel { get; set; }
        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string ShareType { get; set; }
        public string Url { get; set; }
        public string ServerIP { get; set; }
        public string UserIP { get; set; }
        public string UserAgent { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
