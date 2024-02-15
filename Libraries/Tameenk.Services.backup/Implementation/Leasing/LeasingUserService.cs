using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Core.Leasing;

namespace Tameenk.Services.Implementation.Leasing
{
    public class LeasingUserService : ILeasingUserService
    {
        private readonly IRepository<LeasingUser> _leasingUserRepository;

        public LeasingUserService(IRepository<LeasingUser> leasingUserRepository)
        {
            _leasingUserRepository = leasingUserRepository;
        }

        public LeasingUser GetUserByEmail(string email)
        {
            return _leasingUserRepository.TableNoTracking.Where(x => x.Email == email).FirstOrDefault();
        }
        public LeasingUser GetUser(string UserId)
        {
            return _leasingUserRepository.TableNoTracking.Where(x => x.UserId == UserId).FirstOrDefault();
        }

        public LeasingUser GetUserByIdForUpdate(string Id)
        {
            return _leasingUserRepository.Table.Where(x => x.UserId == Id).FirstOrDefault();
        }

        public LeasingUser GetUserByIdAndRefId(string Id,string referenceId)
        {
            return _leasingUserRepository.Table.Where(x => x.UserId == Id && x.ReferenceId == referenceId).FirstOrDefault();
        }

        public bool UpdateUserInfo(LeasingUser user, out string exception)
        {
            exception = string.Empty;
            try
            {
                if (user == null)
                    return false;

                _leasingUserRepository.Update(user);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }
    }
}
