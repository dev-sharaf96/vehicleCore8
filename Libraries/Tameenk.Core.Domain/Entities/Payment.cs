
namespace Tameenk.Core.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public int BillNumber { get; set; }
        
        public string ReferenceID { get; set; }
        
        public string UserID { get; set; }
        
        public string IBNA { get; set; }

        public int? BankCode { get; set; }

        public int? PaymentStatus { get; set; }
    }
}
