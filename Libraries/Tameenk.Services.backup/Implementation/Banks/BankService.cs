using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using Tameenk.Core;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.AutoleasingWallet;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Services.Core;
using Tameenk.Services.Core.Drivers;
using Tameenk.Services.Implementation.Banks;

namespace Tameenk.Services.Implementation
{
    public class BankService : IBankService
    {
        #region Fields
        private readonly IRepository<Bank> _bankRepository;
       // private readonly IRepository<AspNetUser> _userRepository;
        #endregion

        #region the Ctro
        public BankService(IRepository<Bank> bankRepository)
        {
            _bankRepository = bankRepository ?? throw new TameenkArgumentNullException(nameof(bankRepository));
            //_userRepository= userRepository ?? throw new TameenkArgumentNullException(nameof(userRepository));
        }

        public Bank AddBank(string bankNameAr, string bankNameEn, string Iban, string NationalAddress, string PhoneNumber, string Email)
        {
            try
            {
                Bank bank = new Bank() { NameAr = bankNameAr, NameEn = bankNameEn, IBAN = Iban, NationalAddress = NationalAddress, PhoneNumber = PhoneNumber, Email = Email };
                _bankRepository.Insert(bank);
                return bank;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool DeleteBank(Bank bank)
        {
            try
            {
                var bankId = _bankRepository.Table.Where(x => x.Id == bank.Id).FirstOrDefault();
                _bankRepository.Delete(bankId);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool EditBank(Bank bank)
        {
            try
            {
                var bankUpdated = _bankRepository.Table.Where(x => x.Id == bank.Id).FirstOrDefault();
                bankUpdated.NameAr = string.IsNullOrEmpty(bank.NameAr) ? bankUpdated.NameAr : bank.NameAr;
                bankUpdated.NameEn = string.IsNullOrEmpty(bank.NameEn) ? bankUpdated.NameAr : bank.NameEn;
                bankUpdated.NationalAddress = string.IsNullOrEmpty(bank.NationalAddress) ? bankUpdated.NationalAddress : bank.NationalAddress;
                bankUpdated.Email = string.IsNullOrEmpty(bank.Email) ? bankUpdated.Email : bank.Email;
                bankUpdated.IBAN = string.IsNullOrEmpty(bank.IBAN) ? bankUpdated.IBAN : bank.IBAN;
                bankUpdated.PhoneNumber = string.IsNullOrEmpty(bank.PhoneNumber) ? bankUpdated.PhoneNumber : bank.PhoneNumber;

                _bankRepository.Update(bankUpdated);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool EditBankIban(int bankId, string Iban)
        {
            try
            {
                var bankUpdated = _bankRepository.Table.Where(x => x.Id == bankId).FirstOrDefault();
                bankUpdated.IBAN = Iban;
                _bankRepository.Update(bankUpdated);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public List<AutoleasingWalletReportModel> GetAutoleasingWalletReport(AutoleasingWalletHistoryFilterModel filter, int bankId, out int totalCount, out string exception)
        {
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            totalCount = 0;
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAutoleasingWalletReport";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60;

                if (filter.InsuranceCompanyId.HasValue)
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@insuranceCompanyId", Value = filter.InsuranceCompanyId.Value });
                }
                if (filter.StartDate.HasValue)
                {
                    DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@startDate", Value = dtStart });
                }
                if (filter.EndDate.HasValue)
                {
                    DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@endDate", Value = dtEnd });
                }
                if (!string.IsNullOrEmpty(filter.Email))
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@email", Value = filter.Email });
                }
                command.Parameters.Add(new SqlParameter() { ParameterName = "@bankId", Value = bankId });
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<AutoleasingWalletReportModel> data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<AutoleasingWalletReportModel>(reader).ToList();

                //get data count
                if (!filter.IsExcel)
                {
                    reader.NextResult();
                    totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                }

                dbContext.DatabaseInstance.Connection.Close();
                return data;
            }
            catch (Exception ex)
            {
                dbContext.DatabaseInstance.Connection.Close();
                exception = ex.ToString();
                return null;
            }
        }


        public List<AutoleasingWalletHistory> GetAutoleasingWalletHistory(AutoleasingWalletHistoryFilterModel filter, int bankId, int pageIndex, int pageSize, out int totalCount, out string exception)
        {
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            totalCount = 0;
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAutoleasingWalletHistory";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60;

                if (filter.InsuranceCompanyId.HasValue)
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@insuranceCompanyId", Value = filter.InsuranceCompanyId.Value });
                }
                if (filter.StartDate.HasValue)
                {
                    DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@startDate", Value = dtStart });
                }
                if (filter.EndDate.HasValue)
                {
                    DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@endDate", Value = dtEnd });
                }
                if (!string.IsNullOrEmpty(filter.Email))
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@email", Value = filter.Email });
                }
                command.Parameters.Add(new SqlParameter() { ParameterName = "@bankId", Value = bankId });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@pageNumber", Value = pageIndex });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@pageSize", Value = pageSize });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@isExcel", Value = filter.IsExcel ? 1 : 0 });
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<AutoleasingWalletHistory> data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<AutoleasingWalletHistory>(reader).ToList();

                //get data count
                if (!filter.IsExcel)
                {
                    reader.NextResult();
                    totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                }

                dbContext.DatabaseInstance.Connection.Close();
                return data;
            }
            catch (Exception ex)
            {
                dbContext.DatabaseInstance.Connection.Close();
                exception = ex.ToString();
                return null;
            }
        }
        public Bank GetBank(int id)
        {
            return _bankRepository.Table.FirstOrDefault(d => d.Id == id);
        }
        public IList<Bank> GetBankByName(string bankname)
        {
            var query = _bankRepository.Table;
            if (!string.IsNullOrEmpty(bankname))
            {
                query = query.Where(x => x.NameEn.Contains(bankname.Trim()) || x.NameAr.Contains(bankname.Trim()));
            }
            return query.ToList();
        }
        public IPagedList<Bank> GetBanksWithFilter(IList<Bank> query, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            pageIndex = pageIndex == 1 ? 0 : pageIndex;
            return new PagedList<Bank>(query, pageIndex, pageSize);
        }
        public void updateBankForWallet(Bank bank, out string exception)
        {
            exception = string.Empty;
            try
            {
                    _bankRepository.Update(bank);
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
            }
        }
        public List<Bank> GetBanks()
        {
            return _bankRepository.Table.ToList();
        }

        //public Bank GetUserBank(string bankId)
        //{
        //    try
        //    {
        //        var userManager = _authorizationService.GetUser(log.UserId.ToString());
        //        //var user = _userRepository.TableNoTracking.Where(x => x.Id == userId).FirstOrDefault();
        //        //var bank = _bankRepository.TableNoTracking.Where(x => x.Id == user.AutoLeasingBankId).FirstOrDefault();
        //        //return bank;
        //        return null;

        //    }
        //    catch(Exception ex)
        //    {

        //        return null;
        //    }
        //}
        #endregion
    }
}
