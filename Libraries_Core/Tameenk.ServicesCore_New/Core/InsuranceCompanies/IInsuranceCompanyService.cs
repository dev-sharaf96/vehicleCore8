using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Enums;

namespace Tameenk.Services.Core.InsuranceCompanies
{
    public interface IInsuranceCompanyService
    {
        /// <summary>
        /// take file name and check if there is NameSpace in the same name
        /// because dll file save with name of (NameSpace)
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        bool CheckDllExist(string fileName);


        /// <summary>
        /// get all insurance company only
        /// </summary>
        /// <returns>List<InsuranceCompany></returns>
        IEnumerable<InsuranceCompany> GetAll();
        IEnumerable<InsuranceCompany>  GetAllinsuranceCompany();

        /// <summary>
        /// Get insurance company by id
        /// </summary>
        /// <param name="insuranceCompanyId"></param>
        /// <returns></returns>
        InsuranceCompany GetById(int insuranceCompanyId);

        /// <summary>
        /// Get insurance company name by id
        /// </summary>
        /// <param name="insuranceCompanyId"></param>
        /// <returns></returns>
        string GetInsuranceCompanyName(int insuranceCompanyId, LanguageTwoLetterIsoCode culture);

        /// <summary>
        /// get all insuarance companies
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        IPagedList<InsuranceCompany> GetAllInsuranceCompanies(int pageIndex = 0, int pageSize = int.MaxValue, string sortField = null, bool sortOrder = false,bool showInActive=false);

        /// <summary>
        /// change state of company (Active / inactive)
        /// </summary>
        /// <param name="isActive"></param>
        /// <param name="insuranceCompanyId"></param>
        /// <returns></returns>
        InsuranceCompany ToggleCompanyActivation(bool isActive, int insuranceCompanyId);

        /// <summary>
        /// Add new insurance company
        /// </summary>
        /// <param name="insuranceCompany"></param>
        /// <returns></returns>
        InsuranceCompany AddInsuranceCompany(InsuranceCompany insuranceCompany);

        /// <summary>
        /// Edit insurance company
        /// </summary>
        /// <param name="insuranceCompany"></param>
        /// <returns></returns>
        InsuranceCompany EditInsuranceCompany(InsuranceCompany insuranceCompany);

        /// <summary>
        /// Get company by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        InsuranceCompany GetByKey(string key);

        InsuranceCompany ToggleCompanyActivationByType(bool isActive, int insuranceCompanyId, int insuranceType);
        InsuranceCompany ToggleCompanyAddressValidationActivation(bool isActive, int insuranceCompanyId);
        List<InsuranceCompany> GetInsuranceCompaniesByUserId(string userId, out int totalCount, out string exception);
        InsuranceCompany ToggleCompanyActivationTabby(bool isActive, int insuranceCompanyId, int insuranceType);
        void UpdateCompanyGrade();
    }
}
