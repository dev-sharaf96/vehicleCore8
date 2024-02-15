using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Enums;

namespace Tameenk.Services.Core.Provider
{
    public interface IProviderMappingService
    {
        string GetProviderCompanyName(Company company);
        string GetProviderCompanyNameAr(Company company);
        string GetProviderCompanyNameEn(Company company);
    }
}
