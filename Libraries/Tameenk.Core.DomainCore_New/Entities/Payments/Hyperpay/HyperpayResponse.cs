using System;

namespace Tameenk.Core.Domain.Entities.Payments.RiyadBank
{
    public class HyperpayResponse : BaseEntity
    {
        public int Id { get; set; }
        public int HyperpayRequestId { get; set; }
        public string HyperpayResponseId { get; set; }
        public string ResponseCode { get; set; }
        public string ReferenceId { get; set; }
        public decimal Amount { get; set; }
        public string BuildNumber { get; set; }
        public string Ndc { get; set; }
        public string Timestamp { get; set; }
        public string Descriptor { get; set; }
        public string PaymentBrand { get; set; }
        public string CardBin { get; set; }
        public string Last4Digits { get; set; }
        public string Holder { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string Message { get; set; }
        public string ServiceResponse { get; set; }
        public DateTime? CreatedDate { get; set; }
        public HyperpayRequest HyperpayRequest { get; set; }

        public bool? IsCancelled { get; set; } = false;
        public DateTime? CancelationDate { get; set; }
        public string CancelledBy { get; set; }
    }
}
