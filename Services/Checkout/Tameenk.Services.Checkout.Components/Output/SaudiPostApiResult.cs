using System.Collections.Generic;

namespace Tameenk.Services.Checkout.Components
{
    public class SaudiPostApiResult
    {
        public string totalSearchResults { get; set; }
        public List<SaudiPostAddress> Addresses { get; set; }
        public object PostCode { get; set; }
        public bool success { get; set; }
        public object result { get; set; }
        public string statusdescription { get; set; }
        public object fullexception { get; set; }
    }
}
