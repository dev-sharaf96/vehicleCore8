using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Services.Core;

namespace Tameenk.Services.Implementation
{
    public class AutoleasingQuotationFormService : IAutoleasingQuotationFormService
    {
        private readonly IRepository<AutoleasingQuotationForm> _autoleasingQuotationFormRepository;

        public AutoleasingQuotationFormService(IRepository<AutoleasingQuotationForm> autoleasingQuotationFormRepository)
        {
            this._autoleasingQuotationFormRepository = autoleasingQuotationFormRepository;
        }
        public int GetQuotationFormHitCountByBank(int bankId)
        {
            return _autoleasingQuotationFormRepository.TableNoTracking.Where(x => x.BankId == bankId).Count();
        }

        public int GetQuotationFormHitCountByExternalId()
        {
            return _autoleasingQuotationFormRepository.TableNoTracking.GroupBy(x => x.ExternalId).Count();
        }

        public int GetQuotationFormHitCountByUserId()
        {
            return _autoleasingQuotationFormRepository.TableNoTracking.GroupBy(x => x.UserId).Count();
        }

        public bool Insert(AutoleasingQuotationForm form)
        {
            try
            {
                _autoleasingQuotationFormRepository.Insert(form);
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
        public AutoleasingQuotationForm GetQuotationFormByExternalId(string externalId)
        {
            try
            {
                return _autoleasingQuotationFormRepository.TableNoTracking.Where(x => x.ExternalId == externalId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;

            }

        }

    }
}
