using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.Inquiry.Components
{
    public class YakeenMissingInfoRequestModel
    {
        public string QuotationRequestExternalId { get; set; }

        public QuotationRequestRequiredFieldsModel YakeenMissingFields { get; set; }
    }
}