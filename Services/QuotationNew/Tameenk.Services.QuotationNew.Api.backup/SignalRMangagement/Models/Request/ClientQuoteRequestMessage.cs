using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.QuotationNew.Api
{
    public class ClientQuoteRequestMessage
    {
        public ClientQuoteRequestMessage() { }
        public string RequesterConnectionId { get; set; }
        public List<ClientQuoteRequest> ClientQuoteRequests { get; set; }
    }
}