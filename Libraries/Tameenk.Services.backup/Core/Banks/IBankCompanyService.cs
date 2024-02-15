using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System.Collections.Generic;
using System.Linq;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Services.Implementation;
using Tameenk.Services.Implementation.Drivers;

namespace Tameenk.Services.Core
{
    public interface IBankCompanyService
    {
        List<InsuranceCompany> GetBankCompanies(int? BankId);
        List<BankInsuranceCompanyOutput> GetBankCompanies(int? BankId, string lang);
        List<int> GetBankCompanyIds(int? BankId);
        bool IsBankCompanyExist(int Bankid, int CompanyId);
        bool AddBankCompany(int Bankid, List<int> companyIds);
        bool DeleteBankCompany(int Bankid, List<int> companyIds);
        bool ActivateBankCompany(int companyId, int bankId, bool status);
    }
}
