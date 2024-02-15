using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Identity;
using Tameenk.Services.Core;
using Tameenk.Services.Implementation;

namespace Tameenk.Services.Implementation
{
    public class AutoleasingVerifyUsersService : IAutoleasingVerifyUsersService
    {
        private readonly IRepository<AutoleasingVerifyUsers> _autoleasingVerifyRepository;

        public AutoleasingVerifyUsersService(IRepository<AutoleasingVerifyUsers> autoleasingVerifyrepository)
        {
            this._autoleasingVerifyRepository = autoleasingVerifyrepository;
        }

        public bool AddOtp(AutoleasingVerifyUsers otp)
        {
            try
            {
                _autoleasingVerifyRepository.Insert(otp);
                return true;
            }
            catch (Exception   )
            {
                return false;
            }
        }

        public AutoleasingVerifyUsers GetOtpByUser(string userId, string methodName)
        {
            return _autoleasingVerifyRepository.Table.Where(x => x.MethodName == methodName && x.UserId == userId).OrderByDescending(x => x.Id).FirstOrDefault();
        }

        public bool UpdateOtp(AutoleasingVerifyUsers otp)
        {
            try
            {
                _autoleasingVerifyRepository.Update(otp);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
