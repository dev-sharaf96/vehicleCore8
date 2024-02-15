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
    public class CorporateAccountService : ICorporateAccountService
    {
        private readonly IRepository<CorporateAccount> _corporateAccountRepository;
        private readonly IRepository<CorporateWalletHistory> _corporateWalletHistoryRepository;

        public CorporateAccountService(IRepository<CorporateAccount> corporateAccountRepository, IRepository<CorporateWalletHistory> corporateWalletHistoryRepository)
        {
            _corporateAccountRepository = corporateAccountRepository;
            _corporateWalletHistoryRepository = corporateWalletHistoryRepository;
        }

        public List<CorporateAccount> GetCorporateAccounts()
        {
            return _corporateAccountRepository.TableNoTracking.ToList();
        }

        public CorporateAccount GetCorporateAccountById(int accountId)
        {
            return _corporateAccountRepository.TableNoTracking.Where(a => a.Id == accountId).FirstOrDefault();
        }

        public List<CorporateAccountModel> GetCorporateAccountWithFilter(CorporateAccountFilter filterModel, int pageIndex, int pageSize, int commandTimeout, bool export, out int totalCount, out string exception)
        {
            totalCount = 0;
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllCorporateAccountsFromDBWithFilter";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;

                if (!string.IsNullOrEmpty(filterModel.Name))
                {
                    SqlParameter nameParameter = new SqlParameter() { ParameterName = "name", Value = filterModel.Name };
                    command.Parameters.Add(nameParameter);
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
                List<CorporateAccountModel> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<CorporateAccountModel>(reader).ToList();

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

        public CorporateAccount GetCorporateAccount(int accountId)
        {
            return _corporateAccountRepository.Table.Where(a => a.Id == accountId).FirstOrDefault();
        }

        public void AddOrupdateCororateAccount(CorporateAccount account, out string exception)
        {
            exception = string.Empty;
            try
            {
                if (account.Id == 0)
                    _corporateAccountRepository.Insert(account);
                else
                    _corporateAccountRepository.Update(account);
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
            }
        }

        public WalletOutput AddBalanceToWallet(WalletAddBalanceModel addBalanceModel)
        {
            WalletOutput output = new WalletOutput();
            addBalanceModel.Amount = Math.Round(addBalanceModel.Amount, 2);
            if (addBalanceModel.Amount <= 0)
            {
                output.ErrorCode = WalletOutput.ErrorCodes.CanNotAddNegativeAmount;
                output.ErrorDescription = "Can't Add Negative Amount";
                return output;
            }

            var corporateAccount = _corporateAccountRepository.Table.FirstOrDefault(c => c.Id == addBalanceModel.Id);
            if (corporateAccount == null)
            {
                output.ErrorCode = WalletOutput.ErrorCodes.CorporateAccountNotFound;
                output.ErrorDescription = "Corporate Account Not Found";
                return output;
            }

            if (addBalanceModel.TransactionTypeId == 1)
                corporateAccount.Balance += addBalanceModel.Amount;
            else
                corporateAccount.Balance -= addBalanceModel.Amount;
            corporateAccount.ModifiedBy = addBalanceModel.BalanceAddedBy;
            corporateAccount.ModifiedDate = DateTime.Now;
            _corporateAccountRepository.Update(corporateAccount);

            CorporateWalletHistory corporateWalletHistory = new CorporateWalletHistory();
            corporateWalletHistory.CorporateAccountId = corporateAccount.Id;
            corporateWalletHistory.Amount = addBalanceModel.Amount;
            corporateWalletHistory.MethodName = "AddBalanceToWallet";
            corporateWalletHistory.CreatedDate = DateTime.Now;
            corporateWalletHistory.CreatedBy = addBalanceModel.BalanceAddedBy;
            _corporateWalletHistoryRepository.Insert(corporateWalletHistory);

            output.ErrorCode = WalletOutput.ErrorCodes.Success;
            output.ErrorDescription = "Success";
            output.NewBalance = corporateAccount.Balance.Value;
            return output;
        }
    }
}
