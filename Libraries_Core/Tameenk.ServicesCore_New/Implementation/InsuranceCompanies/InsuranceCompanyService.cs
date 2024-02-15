using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using Tameenk.Core;
using Tameenk.Core.Caching;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Services.Core.InsuranceCompanies;

namespace Tameenk.Services.Implementation.InsuranceCompanies
{
    public class
        InsuranceCompanyService : IInsuranceCompanyService
    {

        #region fields

        private const string INSURANCE_COMPANIES_ALL_KEY = "tameenk.insurance.companies.all";
        private const string INSURANCE_COMPANIES_PATTERN = "tameenk.insurance.companies";


        private readonly IRepository<InsuranceCompany> _insuranceCompanyRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<InsuranceCompanyGrade> _insuranceCopanyGradeRepo;
        #endregion

        #region constructor
        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="insuranceCompanyRepository"></param>
        public InsuranceCompanyService(IRepository<InsuranceCompany> insuranceCompanyRepository, ICacheManager cacheManager, IRepository<InsuranceCompanyGrade> insuranceCopanyGradeRepo)
        {
            _insuranceCompanyRepository = insuranceCompanyRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<InsuranceCompany>));
            _cacheManager = cacheManager ?? throw new TameenkArgumentNullException(nameof(ICacheManager));
            _insuranceCopanyGradeRepo = insuranceCopanyGradeRepo;

        }
        #endregion region 


        #region methods




        /// <summary>
        /// get all name of insurance company only
        /// </summary>
        /// <returns>List<InsuranceCompany></returns>
        public IEnumerable<InsuranceCompany> GetAll()
        {
            return _cacheManager.Get(INSURANCE_COMPANIES_ALL_KEY,180, () =>
            {
                return _insuranceCompanyRepository.TableNoTracking.Where(a => a.InsuranceCompanyID <= 200).Include(c => c.Address).Include(c => c.Contact).ToList();
            });
        }
        public IEnumerable<InsuranceCompany> GetAllinsuranceCompany()
        {
            return _cacheManager.Get(INSURANCE_COMPANIES_ALL_KEY, 20, () =>
            {
                return _insuranceCompanyRepository.TableNoTracking.Include(c => c.Contact).ToList();
            });
        }
        /// <summary>
        /// take file name and check if there is NameSpace in the same name
        /// because dll file save with name of (NameSpace)
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool CheckDllExist(string fileName)
        {
            InsuranceCompany res = _insuranceCompanyRepository.Table.FirstOrDefault(ic => ic.NamespaceTypeName == fileName);

            if (res != null)
                return true;

            return false;
        }

        /// <summary>
        /// Add new insurance company
        /// </summary>
        /// <param name="insuranceCompany">insurance company</param>
        /// <returns>  </returns>
        public InsuranceCompany AddInsuranceCompany(InsuranceCompany insuranceCompany)
        {
            try
            {
                InsuranceCompany res = _insuranceCompanyRepository.Table.FirstOrDefault(ic => ic.NamespaceTypeName == insuranceCompany.NamespaceTypeName &&
               ic.ClassTypeName == insuranceCompany.ClassTypeName);

                if (res == null)
                {
                    insuranceCompany.CreatedDate = DateTime.Now;
                    _insuranceCompanyRepository.Insert(insuranceCompany);
                    _cacheManager.RemoveByPattern(INSURANCE_COMPANIES_PATTERN);
                    return insuranceCompany;
                }
                else
                    throw new TameenkArgumentException("Name Space & class must be unique in DB");
            }
            catch (Exception ex)
            {
                throw new TameenkArgumentException(ex.Message);
            }

        }




        /// <summary>
        /// Edit insurance Company. 
        /// </summary>
        /// <param name="insuranceCompany">insurance Company</param>
        /// <returns>Insurance Company
        /// </returns>
        public InsuranceCompany EditInsuranceCompany(InsuranceCompany newInsuranceCompany)
        {
            try
            {

                newInsuranceCompany.LastModifiedDate = DateTime.Now;
                var result = _insuranceCompanyRepository.Table.AsNoTracking()
                    .Any(ic => ic.NamespaceTypeName == newInsuranceCompany.NamespaceTypeName
                && ic.ClassTypeName == newInsuranceCompany.ClassTypeName
           && ic.InsuranceCompanyID != newInsuranceCompany.InsuranceCompanyID);


                if (!result)
                {
                    _insuranceCompanyRepository.Update(newInsuranceCompany);
                    _cacheManager.RemoveByPattern(INSURANCE_COMPANIES_PATTERN);
                }
                else
                    throw new TameenkArgumentException("Name Space & Class not unique in DB");

                return newInsuranceCompany;
            }
            catch (Exception ex)
            {
                throw new TameenkArgumentException(ex.Message);
            }

        }


        /// <summary>
        /// Change state of company (Active / inactive)
        /// </summary>
        /// <param name="isActive">is Active</param>
        /// <param name="insuranceCompanyId">Insurance Company identifier</param>
        /// <returns></returns>
        public InsuranceCompany ToggleCompanyActivation(bool isActive, int insuranceCompanyId)
        {


            var insuranceCompany = _insuranceCompanyRepository.Table.FirstOrDefault(ic => ic.InsuranceCompanyID == insuranceCompanyId);

            if (insuranceCompany == null)
                throw new TameenkArgumentException("The insurance company not found in DB", "insuranceCompanyId");


            insuranceCompany.IsActive = isActive;
            _insuranceCompanyRepository.Update(insuranceCompany);
            _cacheManager.RemoveByPattern(INSURANCE_COMPANIES_PATTERN);
            insuranceCompany = _insuranceCompanyRepository.Table.FirstOrDefault(ic => ic.InsuranceCompanyID == insuranceCompanyId);
            return insuranceCompany;
        }

        /// <summary>
        /// Get insurance company by id
        /// </summary>
        /// <param name="insuranceCompanyId">Insurance Company identifier</param>
        /// <returns></returns>
        public virtual InsuranceCompany GetById(int insuranceCompanyId)
        {
            return GetAllinsuranceCompany().FirstOrDefault(ic => ic.InsuranceCompanyID == insuranceCompanyId);
        }



        /// <summary>
        /// get all insuarance company
        /// </summary>
        /// <param name="pageIndex">page Index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="sortField">sort field</param>
        /// <param name="sortOrder">sort order</param>
        /// <param name="showInActive">show in Active company or not show them</param>
        /// <returns></returns>
        public IPagedList<InsuranceCompany> GetAllInsuranceCompanies(int pageIndex = 0, int pageSize = int.MaxValue, string sortField = null, bool sortOrder = false, bool showInActive = false)
        {
            var query = _insuranceCompanyRepository.Table
                .Include(e => e.Address)
                .Include(e => e.Contact);
            if (!showInActive)
            {
                query = query.Where(ic => ic.IsActive != showInActive);
            }
            return new PagedList<InsuranceCompany>(query, pageIndex, pageSize, "Order", sortOrder);
        }

        /// <summary>
        /// Get insurance company name
        /// </summary>
        /// <param name="insuranceCompanyId"></param>
        /// <returns></returns>
        public virtual string GetInsuranceCompanyName(int insuranceCompanyId, LanguageTwoLetterIsoCode culture)
        {
            if (insuranceCompanyId <= 0)
                throw new TameenkArgumentException("The insurance company id is invalid.", nameof(insuranceCompanyId));

            if (culture == LanguageTwoLetterIsoCode.Ar)
            {
                return GetAll().Where(ic => ic.InsuranceCompanyID == insuranceCompanyId)
               .Select(c => c.NameAR).FirstOrDefault();
            }
            else
            {
                return GetAll().Where(ic => ic.InsuranceCompanyID == insuranceCompanyId)
              .Select(c => c.NameEN).FirstOrDefault();
            }
           

        }

        public InsuranceCompany GetByKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new TameenkArgumentException("The insurance company key invalid.", nameof(key));

            return _insuranceCompanyRepository.Table
                .Where(ic => ic.Key == key).FirstOrDefault();
        }

        /// <summary>
        /// Change state of company (Active / inactive)
        /// </summary>
        /// <param name="isActive">is Active</param>
        /// <param name="insuranceCompanyId">Insurance Company identifier</param>
        /// <param name="insuranceType">Insurance Type</param>
        /// <returns></returns>
        public InsuranceCompany ToggleCompanyActivationByType(bool isActive, int insuranceCompanyId, int insuranceType)
        {
            var insuranceCompany = _insuranceCompanyRepository.Table.FirstOrDefault(ic => ic.InsuranceCompanyID == insuranceCompanyId);

            if (insuranceCompany == null)
                throw new TameenkArgumentException("The insurance company not found in DB", "insuranceCompanyId");

            if (insuranceType == 1) // TPL
            {
                insuranceCompany.IsActiveTPL = isActive;
            }
            else if (insuranceType == 2) // Comprehensive
            {
                insuranceCompany.IsActiveComprehensive = isActive;
            }

            _insuranceCompanyRepository.Update(insuranceCompany);
            _cacheManager.RemoveByPattern(INSURANCE_COMPANIES_PATTERN);
            insuranceCompany = _insuranceCompanyRepository.Table.FirstOrDefault(ic => ic.InsuranceCompanyID == insuranceCompanyId);
            return insuranceCompany;
        }

        /// <summary>        /// Change state of company address validation (Active / inactive)        /// </summary>        /// <param name="isActive">is Active</param>        /// <param name="insuranceCompanyId">Insurance Company identifier</param>        /// <returns></returns>        public InsuranceCompany ToggleCompanyAddressValidationActivation(bool isActive, int insuranceCompanyId)        {            var insuranceCompany = _insuranceCompanyRepository.Table.FirstOrDefault(ic => ic.InsuranceCompanyID == insuranceCompanyId);            if (insuranceCompany == null)                throw new TameenkArgumentException("The insurance company not found in DB", "insuranceCompanyId");            insuranceCompany.IsAddressValidationEnabled = isActive;            _insuranceCompanyRepository.Update(insuranceCompany);            _cacheManager.RemoveByPattern(INSURANCE_COMPANIES_PATTERN);            insuranceCompany = _insuranceCompanyRepository.Table.FirstOrDefault(ic => ic.InsuranceCompanyID == insuranceCompanyId);            return insuranceCompany;        }


        public List<InsuranceCompany> GetInsuranceCompaniesByUserId(string userId, out int totalCount, out string exception)
        {
            totalCount = 0;
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAutoleasingCompaniesByUserId";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60 * 60 * 60;

                SqlParameter UserIdParameter = new SqlParameter() { ParameterName = "UserId", Value = userId };
                command.Parameters.Add(UserIdParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                List<InsuranceCompany> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<InsuranceCompany>(reader).ToList();
                return filteredData;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }


        public InsuranceCompany ToggleCompanyActivationTabby(bool isActive, int insuranceCompanyId, int insuranceType)        {            var insuranceCompany = _insuranceCompanyRepository.Table.FirstOrDefault(ic => ic.InsuranceCompanyID == insuranceCompanyId);            if (insuranceCompany == null)                throw new TameenkArgumentException("The insurance company not found in DB", "insuranceCompanyId");            if (insuranceType == 1) // TPL
            {                insuranceCompany.ActiveTabbyTPL = isActive;            }            else if (insuranceType == 2) // Comprehensive
            {                insuranceCompany.ActiveTabbyComp = isActive;            }            _insuranceCompanyRepository.Update(insuranceCompany);            _cacheManager.RemoveByPattern(INSURANCE_COMPANIES_PATTERN);            insuranceCompany = _insuranceCompanyRepository.Table.FirstOrDefault(ic => ic.InsuranceCompanyID == insuranceCompanyId);            return insuranceCompany;        }
        #endregion


        #region Get Insurance Companies With Grades

        public void UpdateCompanyGrade()
        {
            try
            {
                List<InsuranseCompaniesNajmResponseTime> CompaniesNajmResponseTimes = GetCompaniesWithNajmResponseTime();
                if (CompaniesNajmResponseTimes == null || CompaniesNajmResponseTimes.Count < 1)
                    return;

                foreach (var companyGrade in CompaniesNajmResponseTimes)
                {
                    if (companyGrade.AverageResponseTime < 15)
                        companyGrade.NajmGrade = 4;
                    else if (companyGrade.AverageResponseTime >= 15 && companyGrade.AverageResponseTime <= 30)
                        companyGrade.NajmGrade = 3;
                    else if (companyGrade.AverageResponseTime > 30 && companyGrade.AverageResponseTime <= 60)
                        companyGrade.NajmGrade = 2;
                    else if (companyGrade.AverageResponseTime >= 60 && companyGrade.AverageResponseTime <= 120)
                        companyGrade.NajmGrade = 1;
                    else
                        companyGrade.NajmGrade = 0;
                }

                var insuranceCopanyGrade = _insuranceCopanyGradeRepo.Table.ToList();
                foreach (var row in insuranceCopanyGrade)
                {
                    if (!CompaniesNajmResponseTimes.Any(a => a.CompanyId == row.CompanyId))
                        continue;
                    row.Grade = CompaniesNajmResponseTimes.FirstOrDefault(a => a.CompanyId == row.CompanyId).NajmGrade;
                    row.ModifyDate = DateTime.Now;
                }
                _insuranceCopanyGradeRepo.Update(insuranceCopanyGrade);
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\CompanyGradeTask_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + ex.ToString());
                return;
            }
        }


        public List<InsuranseCompaniesNajmResponseTime> GetCompaniesWithNajmResponseTime()
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 240;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetNajmAverageResponseTime";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var companiesInfo = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<InsuranseCompaniesNajmResponseTime>(reader).ToList();
                idbContext.DatabaseInstance.Connection.Close();                return companiesInfo;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\GetCompaniesWithNajmResponseTime_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", ex.ToString());

                return null;
            }
            finally
            {
                if (idbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    idbContext.DatabaseInstance.Connection.Close();
            }
        }

        #endregion
    }
}
