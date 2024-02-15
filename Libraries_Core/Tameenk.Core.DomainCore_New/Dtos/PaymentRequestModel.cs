namespace Tameenk.Core.Domain.Dtos
{
    public class PaymentRequestModel
    {
        public string UserId { get; set; }

        public string UserEmail { get; set; }

        public string CustomerNameAr { get; set; }

        public string CustomerNameEn { get; set; }

        public decimal PaymentAmount { get; set; }

        public int InvoiceNumber { get; set; }
        public string BaseUrl { get; set; }
        public string ReturnUrl { get; set; }
        public string RequestId { get; set; }
        public string ReferenceId { get; set; }

    }
}
