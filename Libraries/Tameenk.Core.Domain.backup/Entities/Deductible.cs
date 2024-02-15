namespace Tameenk.Core.Domain.Entities
{
    public class Deductible : BaseEntity
    {
        public int ID { get; set; }

        public int InsuranceCompanyID { get; set; }

        public decimal Name { get; set; }

        public InsuranceCompany InsuranceCompany { get; set; }
    }
}
