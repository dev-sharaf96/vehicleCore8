using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Services.InquiryGateway.Models.YakeenMissingFields
{
    public class YakeenMissingInfoRequestModel : BaseViewModel
    {
        public string QuotationRequestExternalId { get; set; }

        public QuotationRequestRequiredFieldsModel YakeenMissingFields { get; set; }
    }
}