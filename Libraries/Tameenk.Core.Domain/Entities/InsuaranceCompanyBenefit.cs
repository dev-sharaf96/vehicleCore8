using System;

namespace Tameenk.Core.Domain.Entities
{
    public class InsuaranceCompanyBenefit : BaseEntity
    {
        public Guid BenifitID { get; set; }

        public int InsurnaceCompanyID { get; set; }

        public short BenifitCode { get; set; }

        public decimal BenifitPrice { get; set; }

        public Benefit Benefit { get; set; }

        public InsuranceCompany InsuranceCompany { get; set; }
    }
}
