using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Tameenk.Core;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Exceptions;
using Tameenk.Services.Core;
using Tameenk.Services.Core.Drivers;

namespace Tameenk.Services.Implementation
{
    public class BankCompanyService : IBankCompanyService
    {
        #region Fields
        private readonly IRepository<BankInsuranceCompany> _bankInsuranceRepository;
        private readonly IRepository<InsuranceCompany> _insuranceCompanyRepository;
        #endregion


        #region the Ctro
        public BankCompanyService(IRepository<BankInsuranceCompany> bankInsuranceRepository, IRepository<InsuranceCompany> insuranceCompanyRepository)
        {
            _bankInsuranceRepository = bankInsuranceRepository ?? throw new TameenkArgumentNullException(nameof(bankInsuranceRepository));
            _insuranceCompanyRepository = insuranceCompanyRepository ?? throw new TameenkArgumentNullException(nameof(insuranceCompanyRepository));
        }

        public List<BankInsuranceCompanyOutput> GetBankCompanies(int? BankId, string lang)
        {
            var companies = _bankInsuranceRepository.Table.Where(x => x.BankId == BankId).ToList();
            List<BankInsuranceCompanyOutput> companyInfos = new List<BankInsuranceCompanyOutput>();
            if (companies != null)
            {
                foreach (var company in companies)
                {
                    var companyInfo = _insuranceCompanyRepository.Table.Where(x => x.InsuranceCompanyID == company.CompanyId && x.IsActiveAutoleasing).FirstOrDefault();
                    BankInsuranceCompanyOutput output = new BankInsuranceCompanyOutput();
                    if (companyInfo != null)
                    {
                        output.Id = company.CompanyId;
                        output.IsActive = company.IsActive;
                        if (lang == "en")
                            output.Name = companyInfo.NameEN;
                        if (lang == "ar")
                            output.Name = companyInfo.NameAR;

                        companyInfos.Add(output);
                    }
                }
            }
            return companyInfos;
        }

        public List<InsuranceCompany> GetBankCompanies(int? BankId)
        {
            var companies = _bankInsuranceRepository.Table.Where(x => x.BankId == BankId && x.IsActive==true);
            List<InsuranceCompany> companyInfos = new List<InsuranceCompany>();
            if (companies != null)
            {
                foreach (var company in companies)
                {
                    var companyInfo = _insuranceCompanyRepository.Table.Where(x => x.InsuranceCompanyID == company.CompanyId && x.IsActiveAutoleasing).FirstOrDefault();
                    if (companyInfo != null)
                    {
                        companyInfos.Add(companyInfo);
                    }
                }
            }
            return companyInfos;
        }
        public bool IsBankCompanyExist(int Bankid, int CompanyId)
        {
            return _bankInsuranceRepository.Table.Any(x => x.BankId == Bankid && x.CompanyId == CompanyId);
        }
        public bool AddBankCompany(int Bankid, List<int> companyIds)
        {
            try
            {
                List<BankInsuranceCompany> bankInsuranceCompanies = new List<BankInsuranceCompany>();

                foreach (var id in companyIds)
                {
                    BankInsuranceCompany bankInsuranceCompany = new BankInsuranceCompany() { BankId = Bankid, CompanyId = id, IsActive = true };
                    bankInsuranceCompanies.Add(bankInsuranceCompany);
                }
                _bankInsuranceRepository.Insert(bankInsuranceCompanies);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteBankCompany(BankInsuranceCompany insuranceCompany)
        {
            try
            {
                _bankInsuranceRepository.Delete(insuranceCompany);
                return true;
            }
            catch (Exception)
            {
                return false;

            }
        }

        public bool DeleteBankCompany(int Bankid, List<int> companyIds)
        {
            try
            {
                List<BankInsuranceCompany> bankComapnies = new List<BankInsuranceCompany>();

                foreach (var companyId in companyIds)
                {
                    var company = _bankInsuranceRepository.Table.Where(x => x.BankId == Bankid && x.CompanyId == companyId).FirstOrDefault();
                    bankComapnies.Add(company);
                }
                _bankInsuranceRepository.Delete(bankComapnies);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public List<int> GetBankCompanyIds(int? BankId)
        {
            var companies = _bankInsuranceRepository.Table.Where(x => x.BankId == BankId);
            List<int> companyInfos = new List<int>();
            foreach (var company in companies)
            {
                var companyInfo = _insuranceCompanyRepository.Table.Where(x => x.InsuranceCompanyID == company.CompanyId).Select(x => x.InsuranceCompanyID).FirstOrDefault();
                companyInfos.Add(companyInfo);
            }
            return companyInfos;
        }

        public bool ActivateBankCompany(int companyId, int bankId, bool status)
        {
            try
            {
                var company = _bankInsuranceRepository.Table.Where(x => x.CompanyId == companyId && x.BankId == bankId).FirstOrDefault();
                if (company != null)
                {
                    company.IsActive = status;
                    _bankInsuranceRepository.Update(company);
                }
                else
                    return false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        #endregion



    }
}
