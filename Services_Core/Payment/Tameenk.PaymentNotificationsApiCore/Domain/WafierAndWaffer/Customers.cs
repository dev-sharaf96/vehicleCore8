using System.ComponentModel.DataAnnotations;

namespace Tameenk.PaymentNotificationsApi.Domain.WafierAndWaffer
{
    public class Customers
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string CustomerName { get; set; }
        //[Required]
        public string userAccountID { get; set; }
    }
}