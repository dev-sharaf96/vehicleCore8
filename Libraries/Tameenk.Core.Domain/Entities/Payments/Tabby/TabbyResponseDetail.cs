using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Payments.Tabby
{
    public class TabbyResponseDetail : BaseEntity
    {
        [Key]
        public Int64 Id { get; set; }
        public virtual Int64 TabbyResponseId { get; set; }
        public TabbyResponse TabbyResponse { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string Warnings { get; set; }
        public string Configuration { get; set; }
        public string ApiUrl { get; set; }
        public string Token { get; set; }
        public string Flow { get; set; }
        public string Status { get; set; }
        public string Customer { get; set; }
        public string JuicyScore { get; set; }
        public string ProductType { get; set; }
        public string SiftSessionId { get; set; }
        public string Merchant { get; set; }
        public bool? TermsAccepted { get; set; }
        public string PaymentID { get; set; }
        public string PaymentCreatedAt { get; set; }
        public string PaymentExpireAt { get; set; }
        public bool? PaymentTest { get; set; }
        public bool? PaymentIsExpire { get; set; }
        public string PaymentStatus { get; set; }
        public bool? PaymentCancelable { get; set; }
        public string PaymentCurrency { get; set; }
        public double PaymentAmount { get; set; }
        public string PaymentDescription { get; set; }
        public string PaymentBuyer { get; set; }
        public string PaymentProduct { get; set; }
        public string PaymentShippingAddress { get; set; }
        public string PaymentOrder { get; set; }
        public string PaymentBuyerHistory { get; set; }
        public string PaymentOrderHistory { get; set; }
        public string PaymentMeta { get; set; }
        public string PaymentCaptures { get; set; }
        public string PaymentRefunds { get; set; }
        public string Lang { get; set; }
        public string MerchantCode { get; set; }
        public string MerchantURL { get; set; }
    }
}
