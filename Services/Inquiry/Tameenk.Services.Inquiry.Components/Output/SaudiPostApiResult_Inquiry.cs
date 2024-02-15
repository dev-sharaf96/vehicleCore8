using System.Collections.Generic;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.Inquiry.Components
{
    public class SaudiPostApiResult_Inquiry
    {
        public string totalSearchResults { get; set; }
        public List<SaudiPostAddress_Inquiry> Addresses { get; set; }
        public object PostCode { get; set; }
        public bool success { get; set; }
        public object result { get; set; }
        public string statusdescription { get; set; }
        public object fullexception { get; set; }
    }
}
