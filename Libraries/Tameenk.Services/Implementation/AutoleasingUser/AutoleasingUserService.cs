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
using Tameenk.Services.Core.Addresses;

namespace Tameenk.Services.Implementation
{
    public class AutoleasingUserService : IAutoleasingUserService
    {
        private readonly IRepository<AutoleasingUser> _autoleasingUserRepository;
        private readonly IAddressService _addressService;

        public AutoleasingUserService(IRepository<AutoleasingUser> autoleasingUserrepository,
            IAddressService addressService)
        {
            _autoleasingUserRepository = autoleasingUserrepository;
            _addressService = addressService;
        }
        public AutoleasingUser GetUserByPhone(string phoneNumber)
        {
            return _autoleasingUserRepository.Table.Where(x => x.PhoneNumber == phoneNumber).FirstOrDefault();
        }
        public AutoleasingUser GetUserByEmail(string email)
        {
            return _autoleasingUserRepository.Table.Where(x => x.Email == email).FirstOrDefault();
        }

        public bool UpdateUser(AutoleasingUser user)
        {
            try
            {
                var userUpdated = _autoleasingUserRepository.Table.Where(x => x.Id == user.Id).FirstOrDefault();
                userUpdated.BankId = user.BankId == 0 ? userUpdated.BankId : user.BankId;
                userUpdated.AdminId = string.IsNullOrEmpty(user.AdminId) ? userUpdated.AdminId : user.AdminId;
                userUpdated.BankName = string.IsNullOrEmpty(user.BankName) ? userUpdated.BankName : user.BankName;
                userUpdated.CreatedBy = string.IsNullOrEmpty(user.CreatedBy) ? userUpdated.CreatedBy : user.CreatedBy;
                userUpdated.CreatedDate = userUpdated.CreatedDate;
                userUpdated.ModifiedBy = string.IsNullOrEmpty(user.ModifiedBy) ? userUpdated.ModifiedBy : user.ModifiedBy;
                userUpdated.LastModifiedDate = DateTime.Now;
                userUpdated.PasswordHash = string.IsNullOrEmpty(user.PasswordHash) ? userUpdated.PasswordHash : user.PasswordHash;
                userUpdated.PhoneNumber = string.IsNullOrEmpty(user.PhoneNumber) ? userUpdated.PhoneNumber : user.PhoneNumber;
                userUpdated.UserId = string.IsNullOrEmpty(user.UserId) ? userUpdated.UserId : user.UserId;
                userUpdated.UserName = string.IsNullOrEmpty(user.UserName) ? userUpdated.UserName : user.UserName;
                userUpdated.IsDeleted = !user.IsDeleted.HasValue ? userUpdated.IsDeleted : user.IsDeleted;
                userUpdated.DeletedDate = !user.DeletedDate.HasValue ? userUpdated.DeletedDate : user.DeletedDate;
                userUpdated.LockoutEnabled = !user.LockoutEnabled.HasValue ? userUpdated.LockoutEnabled : user.LockoutEnabled;
                userUpdated.LockoutEndDateUtc = string.IsNullOrEmpty(user.LockoutEndDateUtc.ToString()) ? userUpdated.LockoutEndDateUtc : user.LockoutEndDateUtc;
                userUpdated.CityCode = user.CityCode == 0 ? userUpdated.CityCode : user.CityCode;
                userUpdated.FullName = string.IsNullOrEmpty(user.FullName) ? userUpdated.FullName : user.FullName;
                userUpdated.IsFirstLogin = !user.IsFirstLogin.HasValue ? userUpdated.IsFirstLogin : user.IsFirstLogin;
                userUpdated.Email = string.IsNullOrEmpty(user.Email) ? userUpdated.Email : user.Email;
                userUpdated.IsSuperAdmin = !user.IsSuperAdmin.HasValue ? userUpdated.IsSuperAdmin : user.IsSuperAdmin;
                _autoleasingUserRepository.Update(userUpdated);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool AddUser(AutoleasingUser user)
        {
            try
            {
                _autoleasingUserRepository.Insert(user);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<AutoleasingUser> GetUsers(string parentId)
        {
            return _autoleasingUserRepository.Table.Where(x => x.AdminId == parentId && x.LockoutEndDateUtc.HasValue && x.LockoutEndDateUtc.Value.Year.ToString() != "3000").ToList();
        }

        public AutoleasingUser GetUser(string id)
        {

            return _autoleasingUserRepository.Table.Where(x => x.Id == id).FirstOrDefault();
        }

        public List<AutoleasingUser> GetSuperAdmins()
        {
            return _autoleasingUserRepository.TableNoTracking.Where(x => x.IsSuperAdmin == true && x.LockoutEndDateUtc.HasValue && x.LockoutEndDateUtc.Value.Year.ToString() != "3000").ToList();
        }

        public List<AutoleaseUserModel> GetAutoleaseUsersWithFilter(AutoleaseFilterModel filterModel, int pageIndex, int pageSize, int commandTimeout, out int totalCount, out string exception)
        {
            totalCount = 0;
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllAutoleaseUsersFromDBWithFilter";
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

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                // get data
                List<AutoleaseUserModel> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<AutoleaseUserModel>(reader).ToList();

                ////get data count
                //reader.NextResult();
                //totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                totalCount = filteredData.Count;

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

        public List<AutoleasingUsersModel> GetModelUsers(int bankId)
        {
            List<AutoleasingUsersModel> users = new List<AutoleasingUsersModel>();
            var allUsers = _autoleasingUserRepository.TableNoTracking.Where(x => x.BankId == bankId && (x.IsSuperAdmin == false || x.IsSuperAdmin == null) && (x.IsDeleted == null || x.IsDeleted == false)).ToList();
            foreach (var user in allUsers)
            {
                AutoleasingUsersModel autoleasingUser = new AutoleasingUsersModel();
                autoleasingUser.CityNameAr = user.CityCode == null ? "" : _addressService.GetCityById((long)user.CityCode)?.ArabicDescription;
                autoleasingUser.CityNameEn = user.CityCode == null ? "" : _addressService.GetCityById((long)user.CityCode)?.EnglishDescription;
                autoleasingUser.AdminId = user.AdminId;
                autoleasingUser.BankId = user.BankId;
                autoleasingUser.BankName = user.BankName;
                autoleasingUser.CityCode = user.CityCode;
                autoleasingUser.CreatedBy = user.CreatedBy;
                autoleasingUser.Email = user.Email;
                autoleasingUser.FullName = user.FullName;
                autoleasingUser.Id = user.Id;
                autoleasingUser.IsDeleted = user.IsDeleted.HasValue ? user.IsDeleted : false;
                autoleasingUser.LockoutEnabled = user.LockoutEnabled.HasValue ? user.LockoutEnabled : false;
                autoleasingUser.PhoneNumber = user.PhoneNumber;
                autoleasingUser.UserName = user.UserName;
                autoleasingUser.CanPurchase = user.CanPurchase;

                users.Add(autoleasingUser);
            }
            return users;
        }

        public AutoleasingUsersModel GetModelUser(string userId)
        {
            AutoleasingUsersModel model = new AutoleasingUsersModel();
            var user = _autoleasingUserRepository.TableNoTracking.Where(x => x.Id == userId).FirstOrDefault();
            model.CityNameAr = user.CityCode == null ? "" : _addressService.GetCityById((long)user.CityCode)?.ArabicDescription;
            model.CityNameEn = user.CityCode == null ? "" : _addressService.GetCityById((long)user.CityCode)?.EnglishDescription;
            model.AdminId = user.AdminId;
            model.BankId = user.BankId;
            model.BankName = user.BankName;
            model.CityCode = user.CityCode;
            model.CreatedBy = user.CreatedBy;
            model.Email = user.Email; ;
            model.FullName = user.FullName;
            model.Id = user.Id;
            model.IsSuperAdmin = user.IsSuperAdmin;
            model.PhoneNumber = user.PhoneNumber;
            model.UserId = user.UserId;
            model.UserName = user.UserName;
            model.IsDeleted = user.IsDeleted.HasValue ? user.IsDeleted : false;
            model.LockoutEnabled = user.LockoutEnabled.HasValue ? user.LockoutEnabled : false;
            return model;
        }
    }
}
