namespace Tameenk.Core.Domain.Entities
{
    public class InsuranceCompanyProductTypeConfig : BaseEntity
    {
        public short ProductTypeCode { get; set; }
        
        public int InsuranceCompanyID { get; set; }

        public byte MinDriverAge { get; set; }

        public byte MaxDriverAge { get; set; }

        public byte MaxVehicleAge { get; set; }

        public ProductType ProductType { get; set; }
    }
}
