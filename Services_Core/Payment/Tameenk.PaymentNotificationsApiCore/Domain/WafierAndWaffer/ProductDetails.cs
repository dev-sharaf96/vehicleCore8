using System.ComponentModel.DataAnnotations;

namespace Tameenk.PaymentNotificationsApi.Domain.WafierAndWaffer
{
    public class ProductDetails
    {
        [Key]
        public int ID { get; set; }
        public string LabelAr { get; set; }
        public string LabelEn { get; set; }
        public string ValulAr { get; set; }
        public string ValulEn { get; set; }
        public int ProductsID { get; set; }
        public virtual Products Products { get; set; }
    }
}