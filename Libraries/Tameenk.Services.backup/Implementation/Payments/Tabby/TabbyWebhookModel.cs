using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tameenk.Services.Implementation.Payments.Tabby.TabbySharedClasses;

namespace Tameenk.Services.Implementation.Payments.Tabby
{
    public class TabbyWebhookModel
    {
        public string id { get; set; }
        public string customer_id { get; set; }
        public string merchant_id { get; set; }
        public bool is_test { get; set; }
        public string created_at { get; set; }
        public string expires_at { get; set; }
        public string closed_at { get; set; }
        public string status { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string description { get; set; }
        public Buyer buyer { get; set; }
        public TabbyOrder order { get; set; }
        public List<CaptureCaptures> captures { get; set; }
        public List<CaptureRefunds> refunds { get; set; }
        public ShippingAddress shipping_address { get; set; }
        public BuyerHistory buyer_history { get; set; }
        public OrderHistory order_history { get; set; }
        public object meta { get; set; }
        public bool cancelable { get; set; }
        public string product_option_id { get; set; }
    }
}
