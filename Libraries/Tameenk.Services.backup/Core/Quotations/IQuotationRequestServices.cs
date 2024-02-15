using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Quotations;

namespace Tameenk.Services.Core.Quotations
{
    public interface IQuotationRequestServices
    {
        /// <summary>
        /// Get Quotation Request by external id 
        /// </summary>
        /// <param name="externalId">external Id</param>
        /// <returns></returns>
        QuotationRequest GetQuotationRequest(string externalId);
    }
}
