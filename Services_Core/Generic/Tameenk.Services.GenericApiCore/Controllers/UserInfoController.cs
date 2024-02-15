using Swashbuckle.Swagger.Annotations;
using System;
using System.Net;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Generic.Component;
using Tameenk.Services.Generic.Components.Output;
using Tameenk.Services.Generic.Components.Models;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Services.Core.Notifications;
using Tameenk.Resources.Account;
using Tameenk.Services.Implementation;
using Tameenk.Loggin.DAL;
using Tameenk.Core.Configuration;
using Tameenk.Services.Generic.Components;
using System.Web.Script.Serialization;
using Tameenk.Resources.WebResources;
using System.Globalization;
using Tameenk.Integration.Dto.Yakeen;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.Generic.Controllers
{
    [AllowAnonymous]
    public class UserInfoController : BaseApiController
    {
        private readonly IUserInfoContext _userInfoContext;

        public UserInfoController(IUserInfoContext userInfoContext)
        {
            _userInfoContext = userInfoContext;
        }

        [HttpPost]
        [Route("api/user-info/add")]
        public IActionResult AddUserInfo(UserInfoModel model)
        {
            string userIdInfo = User?.Identity?.GetUserId();
            string userName = User?.Identity?.GetUserName();
            Guid userId = Guid.Empty;
            Guid.TryParse(userIdInfo, out userId);
            var output = _userInfoContext.AddUserInfo(model, userId, userName);
            if (output.ErrorCode == UserInfoOutput.ErrorCodes.Success)
                return Ok(output);
            else
                return Error(output);
        }

        [HttpPost]
        [Route("api/user-info/add-missing-info")]
        public IActionResult AddMissingUserInfo(UserInfoModel model)
        {
            string userIdInfo = User?.Identity?.GetUserId();
            string userName = User?.Identity?.GetUserName();
            Guid userId = Guid.Empty;
            Guid.TryParse(userIdInfo, out userId);
            var output = _userInfoContext.AddUserInfoMissingFields(model, userId, userName);
            if (output.ErrorCode == UserInfoOutput.ErrorCodes.Success)
                return Ok(output);
            else
                return Error(output);
        }


        [HttpPost]
        [Route("api/user-info/verify-otp")]
        [AllowAnonymous]
        public IActionResult VerifyUserInfoOTP(VerifyOTPModel model)
        {
            string userIdInfo = User?.Identity?.GetUserId();
            string userName = User?.Identity?.GetUserName();
            Guid userId = Guid.Empty;
            Guid.TryParse(userIdInfo, out userId);
            var output = _userInfoContext.VerifyOTP(model, userId, userName);
            if (output.ErrorCode == VerifyOTPOutput.ErrorCodes.Success)
                return Ok(output);
            else
                return Error(output);
        }

        #region Winners

        [HttpGet]
        [Route("api/user-info/get-winners")]
        public IActionResult GetAllWinners(int? weekNumber, string lang, string channel)
        {
            string userIdInfo = User?.Identity?.GetUserId();
            string userName = User?.Identity?.GetUserName();
            Guid userId = Guid.Empty;
            Guid.TryParse(userIdInfo, out userId);
            var output = _userInfoContext.GetAllWinners(weekNumber, lang, channel, userId.ToString());
            if (output.ErrorCode == WinnersOutput<WinnersModel>.ErrorCodes.Success)
                return Ok(output);
            else
                return Error(output);
        }

        #endregion
    }
}
