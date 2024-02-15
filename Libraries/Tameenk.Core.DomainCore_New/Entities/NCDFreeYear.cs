namespace Tameenk.Core.Domain.Entities
{
    public class NCDFreeYear : BaseEntity
    {
        public byte Code { get; set; }
        
        public string EnglishDescription { get; set; }
        
        public string ArabicDescription { get; set; }
    }
}
