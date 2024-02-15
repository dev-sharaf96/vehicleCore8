using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Payments.Tabby
{
    public class TabbySharedClasses
    {
        public class Buyer
        {
            public string phone { get; set; }
            public string email { get; set; }
            public string name { get; set; }
            public string dob { get; set; }
        }
        public class ShippingAddress
        {
            public string city { get; set; }
            public string address { get; set; }
            public string zip { get; set; }
        }
        public class TabbyOrder
        {
            public string tax_amount { get; set; } = "0.00";
            public string shipping_amount { get; set; } = "0.00";
            public string discount_amount { get; set; } = "0.00";
            public string updated_at { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", new CultureInfo("en-US"));
            public string reference_id { get; set; }
            public List<OrderItems> items { get; set; }
            public BuyerHistory buyer_history { get; set; }

        }
        public class OrderItems
        {
            public string title { get; set; }
            public string description { get; set; }
            public int quantity { get; set; } = 1;
            public string unit_price { get; set; } = "0.00";
            public string discount_amount { get; set; } = "0.00";
            public string reference_id { get; set; }
            public string image_url { get; set; }
            public string product_url { get; set; }
            public string gender { get; set; }
            public string category { get; set; }
            public string color { get; set; }
            public string product_material { get; set; }
            public string size_type { get; set; }
            public string size { get; set; }
            public string brand { get; set; }
        }
        public class BuyerHistory
        {
            public string registered_since { get; set; }
            public int loyalty_level { get; set; } = 0;
            public int wishlist_count { get; set; } = 0;
            public bool? is_social_networks_connected { get; set; }
            public bool? is_phone_number_verified { get; set; }
            public bool? is_email_verified { get; set; }
        }

        public class OrderHistory
        {
            public string purchased_at { get; set; }
            public string amount { get; set; }
            public string payment_method { get; set; }
            public string status { get; set; }
            public string registered_since { get; set; }
            public Buyer buyer { get; set; }
            public ShippingAddress shipping_address { get; set; }
            public List<OrderItemHistory> items { get; set; }

        }
        public class OrderItemHistory : OrderItems
        {
            public int ordered { get; set; } = 0;
            public int captured { get; set; } = 0;
            public int shipped { get; set; } = 0;
            public int refunded { get; set; } = 0;
        }
        public class Attachment
        {
            public object body { get; set; }
            public string content_type { get; set; } = "application / vnd.tabby.v1 + json";
        }
        public class MerchantUrls
        {
            public string success { get; set; }
            public string cancel { get; set; }
            public string failure { get; set; }
        }

        public class CaptureItems
        {
            public string title { get; set; }
            public string description { get; set; }
            public int quantity { get; set; }
            public string unit_price { get; set; }
            public string discount_amount { get; set; }
            public string reference_id { get; set; }
            public string image_url { get; set; }
            public string product_url { get; set; }
            public string gender { get; set; }
            public string category { get; set; }
            public string color { get; set; }
            public string product_material { get; set; }
            public string size_type { get; set; }
            public string size { get; set; }
            public string brand { get; set; }
        }
        public class InsuranceDetails        {            public string Pnr { get; set; }            public InsurancePolicyType Policy_type { get; set; }
        }
        public class InsurancePolicyType        {            public string Type { get; set; }            public string Car_manufacture_year { get; set; }
            public string Car_brand { get; set; }
            public string Car_model { get; set; }
            public int? Duration { get; set; }
            public bool Refundable { get; set; }
        }

    }
}
