namespace Tameenk.PaymentNotificationsApi.Domain.WafierAndWaffer
{
    public class PaymentResponseDetails
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int PaymentResponseID { get; set; }
        public virtual PaymentResponse PaymentResponse { get; set; }
    }
}