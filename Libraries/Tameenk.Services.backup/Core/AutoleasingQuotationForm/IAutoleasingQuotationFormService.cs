using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Quotations;

namespace Tameenk.Services.Core
{
    public interface IAutoleasingQuotationFormService
    {
        bool Insert(AutoleasingQuotationForm form);
        int GetQuotationFormHitCountByBank(int bankId);
        int GetQuotationFormHitCountByExternalId();
        int GetQuotationFormHitCountByUserId();
        AutoleasingQuotationForm GetQuotationFormByExternalId(string externalId);
    }
}
