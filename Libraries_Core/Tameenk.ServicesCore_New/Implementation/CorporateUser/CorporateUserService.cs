using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Services.Core;

namespace Tameenk.Services.Implementation
{
    public class CorporateUserService : ICorporateUserService
    {
        private readonly IRepository<CorporateUsers> _corporateUsersRepository;

        public CorporateUserService(IRepository<CorporateUsers> corporateUsersRepository)
        {
            _corporateUsersRepository = corporateUsersRepository;
        }

        public List<CorporateUserModel> GetCorporateUsersWithFilter(CorporateFilterModel filterModel, int pageIndex, int pageSize, int commandTimeout, bool export, out int totalCount, out string exception)
        {
            totalCount = 0;
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllCorporateUsersFromDBWithFilter";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;

                if (!string.IsNullOrEmpty(filterModel.Email))
                {
                    SqlParameter EmailParameter = new SqlParameter() { ParameterName = "Email", Value = filterModel.Email };
                    command.Parameters.Add(EmailParameter);
                }

                if (!string.IsNullOrEmpty(filterModel.PhoneNumber))
                {
                    SqlParameter PhoneNumberParameter = new SqlParameter() { ParameterName = "PhoneNumber", Value = filterModel.PhoneNumber };
                    command.Parameters.Add(PhoneNumberParameter);
                }

                if (filterModel.AccountId.HasValue)
                {
                    SqlParameter accountIdParameter = new SqlParameter() { ParameterName = "accountId", Value = filterModel.AccountId.Value };
                    command.Parameters.Add(accountIdParameter);
                }

                SqlParameter pageIndexParameter = new SqlParameter() { ParameterName = "pageIndex", Value = pageIndex > 0 ? pageIndex : 1 };
                command.Parameters.Add(pageIndexParameter);

                SqlParameter pageSizeParameter = new SqlParameter() { ParameterName = "pageSize", Value = pageSize };
                command.Parameters.Add(pageSizeParameter);

                SqlParameter exportParameter = new SqlParameter() { ParameterName = "export", Value = export };
                command.Parameters.Add(exportParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                // get data
                List<CorporateUserModel> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<CorporateUserModel>(reader).ToList();

                if (export)
                    totalCount = filteredData.Count();
                else
                {
                    //get data count
                    reader.NextResult();
                    totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                }

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

        public List<CorporateUsers> GetAccountUsers(int accountId)
        {
            return _corporateUsersRepository.Table.Where(a => a.CorporateAccountId == accountId).ToList();
        }

        public void UpdateBulkusers(List<CorporateUsers> users, out string exception)
        {
            exception = string.Empty;
            try
            {
                if (users != null && users.Count > 0)
                    _corporateUsersRepository.Update(users);
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
            }
        }
        public List<CorporateNotificationModel> GetCorporateUsersLessThan2000(out string exception)        {            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();            exception = string.Empty;            try            {                idbContext.DatabaseInstance.CommandTimeout = new int?(60);                var command = idbContext.DatabaseInstance.Connection.CreateCommand();                command.CommandText = "GetCorporateUsersLessThan2000";                command.CommandType = CommandType.StoredProcedure;                idbContext.DatabaseInstance.Connection.Open();                var reader = command.ExecuteReader();                var result = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CorporateNotificationModel>(reader).ToList();                idbContext.DatabaseInstance.Connection.Close();                return result;            }            catch (Exception ex)            {                idbContext.DatabaseInstance.Connection.Close();                exception = ex.ToString();                return null;            }        }
        public bool UpdateCorporateUsersWithLastNotification(string username, out string exception)        {            exception = string.Empty;            try            {                if (string.IsNullOrEmpty(username))                {                    exception = " NULL email ";                    return false;                }
                var corporateUsers = _corporateUsersRepository.Table.Where(c => c.UserName == username).FirstOrDefault();
                if (corporateUsers == null)
                {
                    exception = "can not corporate user ";                    return false;
                }
                corporateUsers.NotificationDate = DateTime.Now;
                _corporateUsersRepository.Update(corporateUsers);
                return true;
            }            catch (Exception exp)            {                exception = exp.ToString();                return false;            }
        }
    }
}
