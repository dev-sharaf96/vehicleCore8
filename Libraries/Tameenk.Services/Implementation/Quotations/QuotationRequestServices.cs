using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Exceptions;
using Tameenk.Services.Core.Quotations;

namespace Tameenk.Services.Implementation.Quotations
{
    public class QuotationRequestServices : IQuotationRequestServices
    {
        #region Fields
        private readonly IRepository<QuotationRequest> _quotationRequestRepository;
        #endregion

        #region The ctr

        public QuotationRequestServices(IRepository<QuotationRequest> quotationRequestRepository)
        {

            _quotationRequestRepository = quotationRequestRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<QuotationRequest>));
        }

        #endregion

        /// <summary>
        /// Get quotation Request by external id 
        /// </summary>
        /// <param name="externalId">external Id</param>
        /// <returns></returns>
        public QuotationRequest GetQuotationRequest(string externalId)
        {

          var query= _quotationRequestRepository.Table.Include(q=>q.Vehicle)
                .Include(q=>q.Driver)
                .FirstOrDefault(q => q.ExternalId == externalId);
            return query;
        }
    }
}
