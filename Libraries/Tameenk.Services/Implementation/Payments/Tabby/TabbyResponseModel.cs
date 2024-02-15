using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tameenk.Services.Implementation.Payments.Tabby.TabbySharedClasses;

namespace Tameenk.Services.Implementation.Payments.Tabby
{
    public class TabbyResponseModel
    {
        public string id { get; set; }
        public object warnings { get; set; }
        public Configuration configuration { get; set; }
        public ResponsePayment payment { get; set; }
        public string status { get; set; }
        public Customer customer { get; set; }
        public JuicyScore juicyscore { get; set; }
        public MerchantUrls merchant_urls { get; set; }
        public string product_type { get; set; }
        public string lang { get; set; }
        public string sift_session_id { get; set; }
        public Merchant merchant { get; set; }
        public string merchant_code { get; set; }
        public bool terms_accepted { get; set; }
        public string api_url { get; set; }
        public string token { get; set; }
        public string flow { get; set; }

    }
    public class Merchant
    {
        public string name { get; set; }
        public string address { get; set; }
        public Uri logo { get; set; }
    }
    public class JuicyScore
    {
        public string session_id { get; set; }
        public string referrer { get; set; }
        public string time_zone { get; set; }
        public string useragent { get; set; }
    }
    public class Customer
    {
        public string id { get; set; }
    }
    public class Configuration
    {
        public string currency { get; set; }
        public string app_type { get; set; }
        public bool new_customer { get; set; }
        public string available_limit { get; set; }
        public string min_limit { get; set; }
        public AvailableProduct available_products { get; set; }
        public string country { get; set; }
        public string expires_at { get; set; }
        public string is_bank_card_required { get; set; }
        public string blocked_until { get; set; }
        public ResponseProduct products { get; set; }
       

    }
    public class ResponsePayment
    {
        public string id { get; set; }
        public string created_at { get; set; }
        public string expires_at { get; set; }
        public string status { get; set; }
        public bool is_test { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string description { get; set; }
        public Buyer buyer { get; set; }
        public ShippingAddress shipping_address { get; set; }
        public TabbyOrder order { get; set; }
        public List<PaymentCapture> captures { get; set; }
        public List<PaymentRefund> refunds { get; set; }
        public BuyerHistory buyer_history { get; set; }
        public List<OrderHistory> order_history { get; set; }
        public object meta { get; set; }
        public Attachment attachment { get; set; }
        public bool? cancelable { get; set; }
        public bool? is_expired { get; set; }
        public PaymentProduct product { get; set; }
    }
    public class PaymentProduct
    {
        public string type { get; set; }
        public int installments_count { get; set; }
        public string installment_period { get; set; }

    }
    public class PaymentCapture
    {
        public string id { get; set; }
        public string amount { get; set; }
        public string tax_amount { get; set; }
        public string shipping_amount { get; set; }
        public string discount_amount { get; set; }
        public string created_at { get; set; }
        public List<OrderItems> items { get; set; }

    }
    public class PaymentRefund
    {
        public string id { get; set; }
        public string amount { get; set; }
        public string reason { get; set; }
        public string created_at { get; set; }
        public List<OrderItems> items { get; set; }

    }
    public class ResponseProduct
    {
        public ProductAvailability credit_card_installments { get; set; }
        public ProductAvailability installments { get; set; }
        public ProductAvailability monthly_billing { get; set; }

    }

    public class ProductAvailability
    {
        public string type { get; set; }
        public bool is_available { get; set; }
        public string rejection_reason { get; set; }

    }
    public class AvailableProduct
    {
        public List<SharedObject> installments { get; set; }
        public List<SharedObject> monthly_billing { get; set; }
        public List<SharedObject> credit_card_installments { get; set; }
    }
    public class SharedObject
    {
        public string downpayment { get; set; }
        public string downpayment_percent { get; set; }
        public string amount_to_pay { get; set; }
        public DateTime next_payment_date { get; set; }
        public List<Installment> installments { get; set; }
        public bool pay_after_delivery { get; set; }
        public string pay_per_installment { get; set; }
        public string web_url { get; set; }
        public int id { get; set; }
        public int installments_count { get; set; }
        public string installment_period { get; set; }
        public string service_fee { get; set; }

    }
    public class Installment
    {
        public string due_date { get; set; }
        public string amount { get; set; }
    }
}
