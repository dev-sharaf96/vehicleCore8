namespace Tameenk.Models.Checkout
{
    public class PrePaymentCheckModel
    {
        public string ReferenceId { get; set; }
        public string ExternalId { get; set; }
        public string PaymentId { get; set; }
        public string hashed { get; set; }
        public string ID { get; set; }
        public string PC { get; set; }
        public string Lang { get; set; }
        public string Channel { get; set; }
        public string ProductId { get; set; }
    }
}