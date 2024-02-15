using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tameenk.Services.Implementation.Payments.Tabby.TabbySharedClasses;

namespace Tameenk.Services.Implementation.Payments.Tabby
{
    public class TabbyCaptureResponseViewModel
    {
        public string id { get; set; }
        public string created_at { get; set; }
        public string expires_at { get; set; }
        public bool test { get; set; }
        public bool is_expired { get; set; }
        public string status { get; set; }
        public bool cancelable { get; set; }
        public string currency { get; set; }
        public string amount { get; set; }
        public string description { get; set; }

        public Buyer buyer { get; set; }
        public ProductCapture product { get; set; }
        public ShippingAddress shipping_address { get; set; }
        public OrderCapture order { get; set; }
        public List<CaptureCaptures> captures { get; set; }
        public List<CaptureRefunds> refunds { get; set; }
        public BuyerHistory buyer_history { get; set; }
        public List<OrderHistory> order_history { get; set; }
        public object meta { get; set; }
        public object attachment { get; set; }

    }

    public class ProductCapture
    {
        public string type { get; set; }
        public int installments_count { get; set; }
        public string installment_period { get; set; }
    }

    public class OrderCapture
    {
        public string reference_id { get; set; }
        public string updated_at { get; set; }
        public string tax_amount { get; set; }
        public string shipping_amount { get; set; }
        public string discount_amount { get; set; }
        public List<CaptureItems> items { get; set; }
    }
    public class CaptureCaptures
    {
        public string id { get; set; }
        public string amount { get; set; }
        public string tax_amount { get; set; }
        public string shipping_amount { get; set; }
        public string discount_amount { get; set; }
        public string created_at { get; set; }
        public List<CaptureItems> items { get; set; }
    }
    public class CaptureRefunds
    {
        public string id { get; set; }
        public string amount { get; set; }
        public string reason { get; set; }
        public string created_at { get; set; }
        public List<CaptureItems> items { get; set; }
    }

}
