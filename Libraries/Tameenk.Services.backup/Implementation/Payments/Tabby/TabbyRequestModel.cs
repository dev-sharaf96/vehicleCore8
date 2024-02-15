using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tameenk.Services.Implementation.Payments.Tabby.TabbySharedClasses;

namespace Tameenk.Services.Implementation.Payments.Tabby
{
    public class TabbyRequestModel
    {
        public TabbyPayment payment { get; set; }
        public string lang { get; set; }
        public string merchant_code { get; set; }
        public MerchantUrls merchant_urls { get; set; } = new MerchantUrls();

    }
    public class TabbyPayment
    {
        public string amount { get; set; }
        public string currency { get; set; }
        public string description { get; set; }
        public Buyer buyer { get; set; } = new Buyer();
        public ShippingAddress shipping_address { get; set; }
        public TabbyOrder order { get; set; } = new TabbyOrder();
        public BuyerHistory buyer_history { get; set; } 
        public List<OrderHistory> order_history { get; set; }
        public object meta { get; set; }
        public Attachment attachment { get; set; }
        public InsuranceDetails Insurance_details { get; set; }
    }









}
