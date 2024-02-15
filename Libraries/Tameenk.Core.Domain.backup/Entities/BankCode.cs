namespace Tameenk.Core.Domain.Entities
{
    public class BankCode : BaseEntity
    {
        public int Id { get; set; }

        public string Code { get; set; }
        
        public string EnglishDescription { get; set; }
        
        public string ArabicDescription { get; set; }

        public int? ValidationCode { get; set; }
    }
}
