using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Integration.Dto.Yakeen;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Generic.Components.Models;
using Tameenk.Services.Generic.Components.Output;

namespace Tameenk.Services.Generic.Components
{
    public interface IUserInfoContext
    {
        UserInfoOutput AddUserInfo(UserInfoModel model, Guid userID, string userName);
        UserInfoOutput AddUserInfoMissingFields(UserInfoModel model, Guid userID, string userName);
        VerifyOTPOutput VerifyOTP(VerifyOTPModel model, Guid userID, string userName);
        WinnersOutput<WinnersModel> GetAllWinners(int? weekNumber, string lang, string channel, string userId);
    }
}
