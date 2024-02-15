using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class PolicyRenewedByCompany : BaseEntity
    {
        public int Id { get; set; }
        public string ReferenceId { get; set; }
        public string OldPolicyNo { get; set; }
        public string NewPolicyNo { get; set; }
        public int ProductTypeCode { get; set; }
        public string RenewalDate { get; set; }
        public decimal PolicyAmount { get; set; }
        public decimal PolicyVAT { get; set; }
        public decimal PolicyTotalAmount { get; set; }
        public decimal PolicyRenewalCommission { get; set; }
        public Guid? VehicleId { get; set; }
        public string SequanceNo { get; set; }
        public int? InsuranceCompanyId { get; set; }
        
    }
}