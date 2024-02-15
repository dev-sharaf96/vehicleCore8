using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Implementation;
using Tameenk.Services.Profile.Component.Models;

namespace Tameenk.Services.Profile.Component
{
    public interface IAuthenticationContext
    {
        Task<ProfileOutput<LoginOutput>> Login(LoginViewModel model, string returnUrl = null);
        Task<ProfileOutput<RegisterOutput>> Register(RegisterationModel model);
        bool SetUserAuthenticationCookies(AspNetUser userObject, Channel channel, out string exception);
        bool SetCorporateUserAuthenticationCookies(CorporateUsers userObject, out string exception);
        Task<ProfileOutput<RegisterOutput>> ForgetPassword(ForgetPasswordModel model);
        Task<ProfileOutput<bool>> ConfirmResetPassword(ConfirmResetPasswordModel model);
        Task<ProfileOutput<bool>> ChangePassword(ChangePasswordViewModel model);
        Task<ProfileOutput<bool>> ChangePasswordConfirm(ChangePasswordViewModel model);
        Task<ProfileOutput<bool>> ChangePasswordReSendOTP(ChangePasswordViewModel model);
        Task<ConfirmOutPutModel> ConfirmPhoneAndEmail(ConfirmModel model);
        //Task<ProfileOutput<RegisterOutput>> BeginRegister(RegisterationModel model);
        //Task<ProfileOutput<RegisterOutput>> EndRegister(RegisterationModel model);
        //Task<ProfileOutput<LoginOutput>> BeginLogin(LoginViewModel model, string returnUrl = null);
        //Task<ProfileOutput<LoginOutput>> VerifyYakeenMobile(LoginViewModel model);
        //Task<ProfileOutput<LoginOutput>> EndLogin(LoginViewModel model, string returnUrl = null);
        //SMSOutput ReSendOTPCode(LoginViewModel model);
        UserInfoOutput GetUserInfo(string userId);

        #region New (Register / login) logic

        Task<ProfileOutput<RegisterOutput>> BeginRegister(RegisterationModel model);
        Task<ProfileOutput<RegisterOutput>> EndRegister(RegisterationModel model);
        Task<ProfileOutput<RegisterOutput>> VerifyRegisterOTP(RegisterationModel model);
        Task<ProfileOutput<LoginOutput>> BeginLogin(LoginViewModel model, string returnUrl = null);
        Task<ProfileOutput<LoginOutput>> VerifyYakeenMobile(LoginViewModel model);
        Task<ProfileOutput<LoginOutput>> EndLogin(LoginViewModel model, string returnUrl = null);
        Task<ProfileOutput<LoginOutput>> VerifyLoginOTP(LoginViewModel model);
        SMSOutput ReSendOTPCode(ResendOTPModel model);

        #endregion

        //Task<ProfileOutput<bool>> HandleUserSession(string userId, string sessionId);
        Task<ProfileOutput<ForgotPasswordResponseModel>> BeginForgetPassword(ForgotPasswordRequestViewModel model);
        Task<ProfileOutput<ForgotPasswordResponseModel>> EndForgetPassword(ForgotPasswordRequestViewModel model);
        Task<ProfileOutput<ForgotPasswordResponseModel>> VerifyForgetPasswordOTP(VerifyForgotPasswordOTPRequestModel model);
    }
}
