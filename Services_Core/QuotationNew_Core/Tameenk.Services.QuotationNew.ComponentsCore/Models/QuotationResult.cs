using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Entities.Quotations;

namespace Tameenk.Services.QuotationNew.Components
{
    public class QuotationResult
    {
        public QuotationResponse QuotationResponse { get; set; }
        public List<QuotationError> Errors { get; set; }
    }
}