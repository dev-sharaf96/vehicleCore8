using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.QuotationNew.Api
{
    public class ClientQuoteResponseMessage
    {
        public ClientQuoteResponseMessage() { }
        public string RequesterConnectionId { get; set; }
        public List<ClientQuoteResponse> ClientQuoteResponses { get; set; }
    }
}