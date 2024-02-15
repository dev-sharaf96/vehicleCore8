namespace Tameenk.Core.Domain.Entities
{
    public class PayfortPaymentResponse : BaseEntity
    {
        public int ID { get; set; }

        public int RequestId { get; set; }

        public int ResponseCode { get; set; }
        
        public string ResponseMessage { get; set; }

        public decimal? Amount { get; set; }
        
        public string PaymentOption { get; set; }
        
        public string CardNumber { get; set; }
        
        public string CardHolerName { get; set; }
        
        public string CardExpiryDate { get; set; }
        
        public string CustomerIP { get; set; }
        
        public string FortId { get; set; }

        public short? status { get; set; }


        public string Signature { get; set; }
        public string CustomerEmail { get; set; }

        public PayfortPaymentRequest PayfortPaymentRequest { get; set; }
    }
}
