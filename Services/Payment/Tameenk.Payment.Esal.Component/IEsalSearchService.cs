using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Payment.Esal.Component
{
    public interface IEsalSearchService
    {
        SearchResultOutput SearchInvoices(EsalInvoicesFilter filter, int itemsPerPage, int pageNumber);
    }
}
