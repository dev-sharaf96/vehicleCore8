using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
//using System.Web.Caching;
using Tameenk.Core.Caching;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Loggin.DAL;
using Tameenk.Redis;

namespace Tameenk.Services.QuotationDependancy.Component
{
    public class CompaniesGradesService : ICompaniesGradesService
    {
        private readonly string companiesWithGradesCachKey = "GeT_aLl_CoM_wItHgRaDeS";
        private const int cach_TiMe = 4 * 60 * 60;

        public async Task<List<AllComapanyWithGrade>> GetCompaniesWithGrades()
        {
            QuotationRequestLog log = new QuotationRequestLog();
            log.Channel = "QuotationDependancy";
            log.CreatedDate = DateTime.Now;
            //string exception = string.Empty;
            try
            {
                var companies = await RedisCacheManager.Instance.GetAsync<List<AllComapanyWithGrade>>($"{companiesWithGradesCachKey}");
                if (companies != null && companies.Count > 0)
                    return companies;

                companies = GetAllCompaniesWithGrade();
                if (companies != null && companies.Count > 0)
                    RedisCacheManager.Instance.SetAsync($"{companiesWithGradesCachKey}", companies, cach_TiMe);

                return companies;
            }
            catch (Exception ex)
            {
                log.ErrorCode = 500;
                log.ErrorDescription = "Exception happened: " + ex;
                QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                return null;
            }
        }

        #region helper

        public List<AllComapanyWithGrade> GetAllCompaniesWithGrade() //out string exceptoion
        {
            //exceptoion = string.Empty;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60 * 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetInsuranceCompanyWithGrades";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                List<AllComapanyWithGrade> responseModel = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<AllComapanyWithGrade>(reader).ToList();
                return responseModel;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog\GetAllCompaniesWithGrade_Exception_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", ex.ToString());
                //exceptoion = ex.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == System.Data.ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        #endregion
    }
}
