using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities.Payments.Esal;

namespace Tameenk.Payment.Esal.Component
{
    public class EsalSearchService: IEsalSearchService
    {
        private readonly IRepository<EsalResponse> _esalResponseRepository;
        public EsalSearchService(IRepository<EsalResponse> esalResponseRepository)
        {
            _esalResponseRepository = esalResponseRepository;
        }
        public SearchResultOutput SearchInvoices(EsalInvoicesFilter filter, int itemsPerPage, int pageNumber)
        {
            try
            {
                return new SearchResultOutput
                {
                    ErrorCode = SearchResultOutput.StatusCode.Success,
                    ErrorDiscribtion = "Success",
                    Result = (from response in _esalResponseRepository.TableNoTracking
                              where filter.InvoiceNumber == null || response.InvoiceNumber == filter.InvoiceNumber
                              where filter.SadadBillsNumber == null || response.SadadBillsId == filter.SadadBillsNumber
                              where filter.ReferenceId == null || response.ReferenceId == filter.ReferenceId
                              where filter.IsPaid == null || response.IsPaid == filter.IsPaid
                              orderby response.CreatedDate descending
                              select response)
                              .Skip(itemsPerPage * (pageNumber - 1))
                              .Take(itemsPerPage)
                              .ToList()
                };


            }
            catch (Exception ex)
            {
                return new SearchResultOutput
                {
                    ErrorCode = SearchResultOutput.StatusCode.Failure,
                    ErrorDiscribtion = ex.ToString()
                };
            }
        }

    }
}
