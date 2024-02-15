using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Identity;
using Tameenk.Services.Implementation;

namespace Tameenk.Services.Core
{
    public interface IAutoleasingVerifyUsersService
    {
        AutoleasingVerifyUsers GetOtpByUser(string userId, string methodName);

        bool AddOtp(AutoleasingVerifyUsers otp);
        bool UpdateOtp(AutoleasingVerifyUsers otp);
    }
}
