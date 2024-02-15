using System.Collections.Generic;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.PromotionPrograms;
using Tameenk.Core.Domain.Entities.Quotations;

namespace Tameenk.Services
{
    public interface IInsuredService
    {
        /// <summary>
        /// Get First Setting
        /// </summary>
        /// <returns></returns>
        Insured GetIInsuredByNationalId(string nationalId);
    }
}
