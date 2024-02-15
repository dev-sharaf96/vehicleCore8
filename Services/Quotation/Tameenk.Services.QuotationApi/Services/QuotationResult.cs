using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;

namespace Tameenk.Services.QuotationApi.Services
{
    public class QuotationResult
    {
        public QuotationResponse QuotationResponse { get; set; }
        public List<QuotationError> Errors { get; set; }

    }

    public class QuotationError
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Field { get; set; }
    }
}