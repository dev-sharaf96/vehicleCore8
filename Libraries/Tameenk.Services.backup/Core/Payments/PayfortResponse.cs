namespace Tameenk.Services.Core.Payments
{
    public class PayfortResponse
    {
        public string merchant_reference { get; set; }
        public string signature { get; set; }
        public string __RequestVerificationToken { get; set; }
        public string response_code { get; set; }
        public string response_message { get; set; }
        public string customer_email { get; set; }
        public string amount { get; set; }
        public string card_number { get; set; }
        public string card_holder_name { get; set; }
        public string merchant_identifier { get; set; }
        public string access_code { get; set; }
        public string payment_option { get; set; }
        public string expiry_date { get; set; }
        public string customer_ip { get; set; }
        public string language { get; set; }
        public string eci { get; set; }
        public string fort_id { get; set; }
        public string command { get; set; }
        public string currency { get; set; }
        public string status { get; set; }
    }
}
