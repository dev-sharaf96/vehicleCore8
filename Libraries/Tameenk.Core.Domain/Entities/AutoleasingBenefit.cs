namespace Tameenk.Core.Domain.Entities
{
    public class AutoleasingBenefit : BaseEntity
    {

        public int Id { get; set; }
        public short Code { get; set; }
        public string EnglishDescription { get; set; }
        public string ArabicDescription { get; set; }


    }
}
