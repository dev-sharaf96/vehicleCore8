namespace Tameenk.Core.Domain.Entities
{
    public class ErrorCode : BaseEntity
    {
        public short Code { get; set; }
        
        public string EnglishText { get; set; }
        
        public string EnglishDescription { get; set; }
        
        public string ArabicText { get; set; }
        
        public string ArabicDescription { get; set; }
    }
}
