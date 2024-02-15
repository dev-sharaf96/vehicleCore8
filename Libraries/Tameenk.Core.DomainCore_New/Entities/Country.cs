namespace Tameenk.Core.Domain.Entities
{
    public class Country : BaseEntity
    {
        public short Code { get; set; }
        
        public string EnglishDescription { get; set; }
        
        public string ArabicDescription { get; set; }
    }
}
