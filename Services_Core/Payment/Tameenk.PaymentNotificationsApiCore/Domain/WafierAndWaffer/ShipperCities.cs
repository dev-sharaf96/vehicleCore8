using System.ComponentModel.DataAnnotations;

namespace Tameenk.PaymentNotificationsApi.Domain.WafierAndWaffer
{
    public class ShipperCities
    {
        [Key]
        public string ID { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
    }
}