namespace Tameenk.Core.Domain.Entities.VehicleInsurance
{
    public class VehicleColor : BaseEntity
    {
        public byte Code { get; set; }
        
        public string EnglishDescription { get; set; }
        
        public string ArabicDescription { get; set; }
        public long YakeenCode { get; set; }
        public string YakeenColor { get; set; }
        public long? TawuniyaCode { get; set; }
        public long? WataniyaCode { get; set; }
        public long? WataniyaAutoleaseCode { get; set; }
    }
}
