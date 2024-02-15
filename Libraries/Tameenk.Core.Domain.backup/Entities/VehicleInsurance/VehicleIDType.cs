namespace Tameenk.Core.Domain.Entities.VehicleInsurance
{
    public class VehicleIDType : BaseEntity
    {
        public byte Code { get; set; }
        
        public string EnglishDescription { get; set; }
        
        public string ArabicDescription { get; set; }
    }
}
